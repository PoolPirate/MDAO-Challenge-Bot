using AirtableApiClient;
using Common.Services;
using MDAO_Challenge_Bot.Models;
using MDAO_Challenge_Bot.Options;
using MDAO_Challenge_Bot.Persistence;
using MDAO_Challenge_Bot.Services.Sharing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MDAO_Challenge_Bot.Services.Scraping;
public class AirtableScraper : Singleton
{
    private const int UpdateInterval = 10000;

    [Inject]
    private readonly AirtableOptions AirtableOptions = null!;
    [Inject]
    private readonly SharingService SharingService = null!;
    [Inject]
    private readonly AirtableBase AirtableClient = null!;

    private readonly PeriodicTimer UpdateTimer;

    public AirtableScraper()
    {
        UpdateTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(UpdateInterval));
    }

    protected override async ValueTask RunAsync()
    {
        while (await UpdateTimer.WaitForNextTickAsync())
        {
            try
            {
                var result = await AirtableClient.ListRecords<AirtableChallenge>(AirtableOptions.TableName);

                if (!result.Success)
                {
                    Logger.LogCritical(result.AirtableApiError, "Airtable response unsuccessful!");
                    continue;
                }

                await ProcessChallengesAsync(result.Records.Select(x => x.Fields).ToArray());
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "There was an exception refreshing Airtable Challenges");
            }
        }
    }

    private async Task ProcessChallengesAsync(AirtableChallenge[] challenges)
    {
        using var scope = Provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ChallengeDBContext>();

        foreach (var challenge in challenges)
        {
            if (await dbContext.AirtableChallenges.AnyAsync(x => x.Batch == challenge.Batch && x.Name == challenge.Name))
            {
                continue;
            }

            dbContext.AirtableChallenges.Add(challenge);
            await dbContext.SaveChangesAsync();

            await HandleNewChallengeAsync(challenge);
        }
    }

    private async Task HandleNewChallengeAsync(AirtableChallenge challenge)
    {
        await SharingService.ShareAirtableChallengeAsync(challenge);
    }
}
