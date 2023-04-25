using Common.Services;
using Discord;
using Discord.Webhook;
using MDAO_Challenge_Bot.Entities;
using MDAO_Challenge_Bot.Models;
using MDAO_Challenge_Bot.Utils;

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
        //await WebhookClient.SendMessageAsync(MakeAirtableDiscordMessage(challenge));
    }

    private static Embed MakeLaborMarketEmbed(LaborMarket laborMarket, LaborMarketRequest request, TokenContract paymentToken)
    {
        return new EmbedBuilder()
            .WithColor(Color.Gold)
            .WithTitle(request.Title)
            .AddField(
                "Reward Pool",
                $"{MathUtils.RoundToSignificantDigits(paymentToken.DecimalsAdjust(request.PaymentTokenAmount), 4)} {paymentToken.Symbol}")
            .AddField("Deadline to Claim", $"<t:{request.ClaimSubmitExpiration}:R>")
            .AddField("Marketplace", laborMarket.Name, true)
            .AddField("Claim Now", $"https://metricsdao.xyz/app/market/{laborMarket.Address}/request/{request.RequestId}", true)
            .Build();
    }

    public async Task ShareAsync(LaborMarket laborMarket, LaborMarketRequest request, TokenContract paymentToken)
    {
        await WebhookClient.SendMessageAsync(embeds: new[] { MakeLaborMarketEmbed(laborMarket, request, paymentToken) });
    }


}
