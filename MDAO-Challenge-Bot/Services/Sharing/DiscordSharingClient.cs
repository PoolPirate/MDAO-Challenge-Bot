using Common.Services;
using Discord.Webhook;
using MDAO_Challenge_Bot.Models;

namespace MDAO_Challenge_Bot.Services.Sharing;
public class DiscordSharingClient : Singleton
{
    [Inject]
    private readonly DiscordWebhookClient WebhookClient = null!;

    private static string MakeAirtableDiscordMessage(AirtableChallenge challenge)
    {
        return $"""
    {challenge.Name} | Deadline to submit: <t:{challenge.EndDate.ToUnixTimeSeconds()}:R> 

    https://metricsdao.notion.site/Bounty-Programs-d4bac7f1908f412f8bf4ed349198e5fe
    """;
    }

    public async Task ShareAsync(AirtableChallenge challenge)
    {
        await WebhookClient.SendMessageAsync(MakeAirtableDiscordMessage(challenge));
    }

    private static string MakeLaborMarketDiscordMessage(LaborMarket laborMarket, LaborMarketRequest request)
    {
        return $"""
    {request.Title} | Deadline to claim: <t:{request.ClaimSubmitExpiration}:R> 

    https://metricsdao.xyz/app/market/{laborMarket.Address}/request/{request.RequestId}
    """;
    }

    public async Task ShareAsync(LaborMarket laborMarket, LaborMarketRequest request)
    {
        await WebhookClient.SendMessageAsync(MakeLaborMarketDiscordMessage(laborMarket, request));
    }
}
