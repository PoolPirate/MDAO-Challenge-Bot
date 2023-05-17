using Common.Services;
using Hangfire;
using MDAO_Challenge_Bot.Options;

namespace MDAO_Challenge_Bot.Services.Docs;
public class SheetsSyncScheduler : Singleton
{
    public const string SyncTaskName = "Google-Sheets-Sync";

    [Inject]
    private readonly SpreadSheetSyncOptions SyncOptions = null!;

    protected override ValueTask RunAsync()
    {
        RecurringJob.AddOrUpdate<SheetsSyncRunner>(
            SyncTaskName,
            client => client.SyncSpreadSheetAsync(),
            Cron.Weekly(SyncOptions.SyncDay, SyncOptions.SyncTime.Hour, SyncOptions.SyncTime.Minute));

        return base.RunAsync();
    }
}
