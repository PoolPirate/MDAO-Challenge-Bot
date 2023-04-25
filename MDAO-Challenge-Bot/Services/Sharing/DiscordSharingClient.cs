using Common.Services;
using Discord.Webhook;
using MDAO_Challenge_Bot.Models;

namespace MDAO_Challenge_Bot.Services.Sharing;
public class DiscordSharingClient : Singleton
{
    [Inject]
    private readonly DiscordWebhookClient WebhookClient = null!;

    private string MakeDiscordTemplate(string challengeName) => 
    $"""
        A new challenge was just created on Notion!
        {challengeName}
    """;

    public async Task ShareAsync(AirtableChallenge challenge)
    {
        await WebhookClient.SendMessageAsync(MakeDiscordTemplate(challenge.Name));
    }
}
