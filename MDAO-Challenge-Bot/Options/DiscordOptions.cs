using Common.Configuration;

namespace MDAO_Challenge_Bot.Options;
public class DiscordOptions : Option
{
    public required string WebhookURL { get; set; }
    public required bool ShareAirtable { get; set; }
    public required bool ShareLaborMarkets { get; set; }
}
