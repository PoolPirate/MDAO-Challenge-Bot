using Common.Configuration;

namespace MDAO_Challenge_Bot.Options;
public class TelegramOptions : Option
{
    public required string Token { get; set; }
    public required bool EnableSyncNotification { get; set; }
    public required long SyncNotificationChatId { get; set; }
}
