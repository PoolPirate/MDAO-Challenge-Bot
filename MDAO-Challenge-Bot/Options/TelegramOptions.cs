using Common.Configuration;

namespace MDAO_Challenge_Bot.Options;
public class TelegramOptions : Option
{
    public required string Token { get; set; }

    public required bool EnableSpreadSheetSyncNotification { get; set; }
    public required long SpreadSheetSyncNotificationChatId { get; set; }

    public required bool EnableLaborMarketRequestNotification { get; set; }
    public required long LaborMarketRequestNotificationChatId { get; set; }
}
