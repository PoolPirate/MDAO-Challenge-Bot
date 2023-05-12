using Common.Services;
using MDAO_Challenge_Bot.Options;
using Microsoft.Extensions.Logging;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;

namespace MDAO_Challenge_Bot.Services.Sharing;
public class TelegramSharingClient : Singleton
{
    [Inject]
    private readonly BotClient TelegramClient = null!;

    [Inject]
    private readonly TelegramOptions TelegramOptions = null!;

    private static string SyncNotificationTemplate(int requestCount)
    {
        return 
        $"""
        Spreadsheet sync completed at {DateTimeOffset.UtcNow:f}
        {requestCount} challenges inserted.
        """;
    }

    public async Task ShareSyncNotificationAsync(int requestCount)
    {
        if (!TelegramOptions.EnableSyncNotification)
        {
            Logger.LogWarning("Skipping Telegram sync notification: Disabled");
            return;
        }

        var message = SyncNotificationTemplate(requestCount);
        await TelegramClient.SendMessageAsync(TelegramOptions.SyncNotificationChatId, message);

        Logger.LogInformation("Successfully shared sync notification");
    }
}
