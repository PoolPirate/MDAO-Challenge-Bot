using Common.Configuration;

namespace MDAO_Challenge_Bot.Options;
public class GoogleOptions : Option
{
    public required string AccessToken { get; set; }

    public required bool EnableSpreadSheetSync { get; set; }
    public required string SpreadSheetId { get; set; }
}
