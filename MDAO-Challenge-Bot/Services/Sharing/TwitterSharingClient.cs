using Common.Services;
using Hangfire;
using MDAO_Challenge_Bot.Options;

namespace MDAO_Challenge_Bot.Services.Sharing;
public class TwitterSharingClient : Singleton
{
    public const string TwitterTaskName = "Twitter-Sharing";

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
}
