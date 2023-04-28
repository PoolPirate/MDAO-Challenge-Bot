using Common.Services;
using Hangfire;
using MDAO_Challenge_Bot.Models;
using MDAO_Challenge_Bot.Options;
using MDAO_Challenge_Bot.Persistence;
using MDAO_Challenge_Bot.Services.Sharing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Transactions;

namespace MDAO_Challenge_Bot.Services.Scraping;
public class AirtableScraper : Singleton
{
    private const int UpdateInterval = 5000;

    [Inject]
    private readonly AirtableChallengeClient ChallengeClient = null!;
    [Inject]
    private readonly AirtableOptions AirtableOptions = null!;

    private readonly PeriodicTimer UpdateTimer;

    public AirtableScraper()
    {
        UpdateTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(UpdateInterval));
    }

    protected override async ValueTask RunAsync()
    {
        if (!AirtableOptions.Enabled)
        {
            Logger.LogWarning("Airtable Scraping Disabled!");
            return;
        }

        while (await UpdateTimer.WaitForNextTickAsync())
        {
            try
            {
                var challenges = await ChallengeClient.GetChallengesAsync();
                await ProcessChallengesAsync(challenges);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "There was an exception refreshing Airtable Challenges");
            }
        }
    }

    private async Task ProcessChallengesAsync(AirtableChallenge[] challenges)
    {
        using var transactionScope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);
        using var scope = Provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ChallengeDBContext>();

        var processingCutOff = await dbContext.AirtableChallenges.AnyAsync()
            ? (
                await dbContext.AirtableChallenges
                .MaxAsync(x => x.StartTimestamp))
                .AddDays(-5)
            : DateTimeOffset.MinValue;

        foreach (var challenge in challenges
            .Where(x => x.StartTimestamp <= DateTimeOffset.UtcNow && x.EndTimestamp >= processingCutOff))
        {
            if (await dbContext.AirtableChallenges.AnyAsync(x => x.Title == challenge.Title))
            {
                continue;
            }

            dbContext.AirtableChallenges.Add(challenge);
            await dbContext.SaveChangesAsync();

            BackgroundJob.Enqueue<SharingTaskRunner>(runner => runner.ShareAirtableChallengeAsync(challenge.Id));
        }

        transactionScope.Complete();
    }
}
