using Common.Configuration;

namespace MDAO_Challenge_Bot.Options;
public class SharingOptions : Option
{
    public required string DiscordWebhookURL { get; set; }
}
