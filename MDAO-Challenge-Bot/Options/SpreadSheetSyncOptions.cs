using Common.Configuration;

namespace MDAO_Challenge_Bot.Options;
public class SpreadSheetSyncOptions : Option
{
    public required bool Enabled { get; set; }
    public required string SpreadSheetId { get; set; }
    public required DayOfWeek SyncDay { get; set; }
    public required TimeOnly SyncTime { get; set; }

    public required bool EnableNotification { get; set; }
    public required long NotificationChatId { get; set; }
}
