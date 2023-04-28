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

    private static Embed MakeAirtableChallengeEmbed(AirtableChallenge challenge)
    {
        return new EmbedBuilder()
            .WithColor(Color.LightOrange)
            .WithTitle(challenge.Title)
            .AddField("Submission Deadline", $"<t:{challenge.EndTimestamp.ToUnixTimeSeconds()}:R>")
            .AddField("Claim now", "https://metricsdao.notion.site/Bounty-Programs-d4bac7f1908f412f8bf4ed349198e5fe")
            .Build();
    }

    public async Task ShareAsync(AirtableChallenge challenge)
    {
        await WebhookClient.SendMessageAsync(
            username: "Notion Challenges",
            embeds: new[] { MakeAirtableChallengeEmbed(challenge) });
    }

    private static Embed MakeLaborMarketRequestEmbed(LaborMarket laborMarket, LaborMarketRequest request, TokenContract paymentToken)
    {
        return new EmbedBuilder()
            .WithColor(Color.Gold)
            .WithTitle(request.Title)
            .AddField(
                "Reward Pool",
                $"{MathUtils.RoundToSignificantDigits(paymentToken.DecimalsAdjust(request.PaymentTokenAmount), 4)} {paymentToken.Symbol}", true)
            .AddField("Marketplace", laborMarket.Name, true)
            .AddField("\u200b", "\u200b")
            .AddField("Claim to submit deadline", $"<t:{request.ClaimSubmitExpiration}:R>", true)
            .AddField("Submission deadline", $"<t:{request.SubmitExpiration}:R>", true)
            .AddField("Reviewer deadline", $"<t:{request.ReviewExpiration}:R>", true)
            .AddField("Claim Now", $"https://metricsdao.xyz/app/market/{laborMarket.Address}/request/{request.RequestId}")
            .Build();
    }

    public async Task ShareAsync(LaborMarket laborMarket, LaborMarketRequest request, TokenContract paymentToken)
    {
        await WebhookClient.SendMessageAsync(
            username: laborMarket.Name,
            embeds: new[] { MakeLaborMarketRequestEmbed(laborMarket, request, paymentToken) });
    }
}
