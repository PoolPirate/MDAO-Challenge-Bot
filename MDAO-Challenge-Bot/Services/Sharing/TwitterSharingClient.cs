using Common.Services;
using Hangfire;
using MDAO_Challenge_Bot.Entities;
using MDAO_Challenge_Bot.Models;
using MDAO_Challenge_Bot.Options;
using MDAO_Challenge_Bot.Services.Twitter;
using Microsoft.Extensions.Logging;
using Tweetinvi;

namespace MDAO_Challenge_Bot.Services.Sharing;
public class TwitterSharingClient : Singleton
{
    public const string TwitterTaskName = "Twitter-Sharing";

    [Inject]
    private readonly MessageV2Poster MessageV2Poster = null!;
    [Inject]
    private readonly TwitterOptions TwitterOptions = null!;

    protected override ValueTask RunAsync()
    {
        RecurringJob.AddOrUpdate<TwitterSharingRunner>(
            TwitterTaskName,
            client => client.ShareRecentChallengesAsync(),
            Cron.Daily(TwitterOptions.PostTime.Hour, TwitterOptions.PostTime.Minute));

        return base.RunAsync();
    }

    public async Task ShareLaborMarketRequestViaDMAsync(
        LaborMarket laborMarket, LaborMarketRequest request, TokenContract paymentToken)
    {
        if (!TwitterOptions.EnableDMSharing)
        {
            Logger.LogWarning("Skipping sharing LaborMarketRequest with Id={id} via Twitter DM: Feature disabled", request.Id);
            return;
        }

        await MessageV2Poster.PostMessageAsync(
            $"New request posted: https://metricsdao.xyz/app/market/{laborMarket.Address}/request/{request.RequestId}", 
            TwitterOptions.DMRecipient);
    }
}
