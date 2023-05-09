using Common.Configuration;

namespace MDAO_Challenge_Bot.Options;
public class GoogleOptions : Option
{
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }

    public required string UserId { get; set; }
    public required string RefreshToken { get; set; }

    public required bool EnableSpreadSheetSync { get; set; }
    public required string SpreadSheetId { get; set; }
}
