using Common.Services;
using Discord;
using Discord.Webhook;
using MDAO_Challenge_Bot.Entities;
using MDAO_Challenge_Bot.Models;
using MDAO_Challenge_Bot.Options;
using MDAO_Challenge_Bot.Utils;
using Microsoft.Extensions.Logging;

namespace MDAO_Challenge_Bot.Services.Sharing;
public class DiscordSharingClient : Singleton
{
    [Inject]
    private readonly DiscordWebhookClient WebhookClient = null!;
    [Inject]
    private readonly DiscordOptions DiscordOptions = null!;

    private static Embed MakeAirtableChallengeEmbed(AirtableChallenge challenge)
    {
        return new EmbedBuilder()
            .WithColor(Color.LightOrange)
            .WithTitle(challenge.Title)
            .AddField("Submission Deadline", $"<t:{challenge.EndTimestamp.ToUnixTimeSeconds()}:R>")
            .AddField("Submit now", "https://metricsdao.notion.site/Bounty-Programs-d4bac7f1908f412f8bf4ed349198e5fe")
            .Build();
    }

    public async Task ShareAsync(AirtableChallenge challenge)
    {
        if (!DiscordOptions.ShareAirtable)
        {
            Logger.LogWarning("Skipping sharing AirtableChallenge: Sharing Disabled. Id={id}", challenge.Id);
            return;
        }

        await WebhookClient.SendMessageAsync(
            username: "Notion Challenges",
            embeds: new[] { MakeAirtableChallengeEmbed(challenge) });
    }

    private Embed MakeLaborMarketRequestEmbed(LaborMarket laborMarket, LaborMarketRequest request, TokenContract paymentToken)
    {
        return new EmbedBuilder()
            .WithColor(Color.Gold)
            .WithTitle(request.Title)
            .AddField("Marketplace", laborMarket.Name, true)
            .AddField("Claims", $"""
                Analysts: {request.ProviderLimit}
                Reviewers: {request.ReviewerLimit}
            """, true)
            .AddField(
                "Rewards",
                $"""
                Analysts: {MathUtils.DecimalAdjustAndRoundToSignificantDigits(
                request.ProviderPaymentAmount,
                request.ProviderPaymentToken!.Decimals, 4)} {request.ProviderPaymentToken.Symbol} ({MathUtils.DecimalAdjustAndRoundToSignificantDigits(
                request.ProviderPaymentAmount / request.ProviderLimit,
                request.ProviderPaymentToken!.Decimals, 4)} {request.ProviderPaymentToken.Symbol} each)
                Reviewers: {MathUtils.DecimalAdjustAndRoundToSignificantDigits(
                request.ReviewerPaymentAmount,
                request.ReviewerPaymentToken!.Decimals, 4)} {request.ReviewerPaymentToken.Symbol} ({MathUtils.DecimalAdjustAndRoundToSignificantDigits(
                request.ReviewerPaymentAmount / request.ReviewerLimit,
                request.ReviewerPaymentToken!.Decimals, 4)} {request.ReviewerPaymentToken.Symbol} each)
                """, true)
            .AddField("\u200b", "\u200b")
            .AddField("Claim to submit deadline", $"<t:{request.SignalExpiration.ToUnixTimeSeconds()}:R>", true)
            .AddField("Submission deadline", $"<t:{request.SubmissionExpiration.ToUnixTimeSeconds()}:R>", true)
            .AddField("Reviewer deadline", $"<t:{request.EnforcementExpiration.ToUnixTimeSeconds()}:R>", true)
            .AddField("Claim Now", $"https://metricsdao.xyz/app/market/{laborMarket.Address}/request/{request.RequestId}")
            .Build();
    }

    public async Task ShareAsync(LaborMarket laborMarket, LaborMarketRequest request, TokenContract paymentToken)
    {
        if (!DiscordOptions.ShareLaborMarkets)
        {
            Logger.LogWarning("Skipping sharing LaborMarketRequest: Sharing Disabled. Market={market}, Id={id}", laborMarket.Address, request.Id);
            return;
        }

        await WebhookClient.SendMessageAsync(
            username: laborMarket.Name,
            embeds: new[] { MakeLaborMarketRequestEmbed(laborMarket, request, paymentToken) });
    }

    public async Task ShareToWebhookAsync(string webhookUrl, LaborMarket laborMarket, LaborMarketRequest request, TokenContract paymentToken)
    {
        var client = new DiscordWebhookClient(webhookUrl);

        await client.SendMessageAsync(
        username: laborMarket.Name,
        embeds: new[] { MakeLaborMarketRequestEmbed(laborMarket, request, paymentToken) });
    }
}
