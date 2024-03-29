﻿using Common.Services;
using MDAO_Challenge_Bot.Entities;
using MDAO_Challenge_Bot.Models;
using MDAO_Challenge_Bot.Options;
using MDAO_Challenge_Bot.Persistence;
using MDAO_Challenge_Bot.Services.Twitter;
using MDAO_Challenge_Bot.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Numerics;
using System.Text;

namespace MDAO_Challenge_Bot.Services.Sharing;
public class TwitterSharingRunner : Scoped
{
    [Inject]
    private readonly TweetsV2Poster TweetsV2Poster = null!;
    [Inject]
    private readonly ChallengeDBContext DbContext = null!;
    [Inject]
    private readonly TwitterOptions TwitterOptions = null!;
    [Inject]
    private readonly ILogger<TwitterSharingRunner> Logger = null!;

    private static string HeadingTemplate(int count, (TokenContract PaymentToken, BigInteger Amount)[] pricePools)
    {
        var sb = new StringBuilder();

        foreach (var (PaymentToken, Amount) in pricePools)
        {
            sb.AppendLine(
                $"- {MathUtils.DecimalAdjustAndRoundToSignificantDigits(
                  Amount,
                  PaymentToken.Decimals,
                  4
                )} {PaymentToken.Symbol}");
        }

        return $"""
            👀 {count} new web3 analytics challenge{(count > 1 ? 's' : "")} who dis 

            💸 Total bounty rewards: 
            {sb}

            👇 Learn more below and get analyzing!
            """;
    }

    private static string LaborMarketRequestTemplate(LaborMarket laborMarket, LaborMarketRequest request, TokenContract paymentToken)
    {
        return $"""
            📜 Title: {request.Title}

            🔒 Claims: {request.ProviderLimit}

            💰 Value: {MathUtils.DecimalAdjustAndRoundToSignificantDigits(
                                         request.ProviderPaymentAmount,
                                         paymentToken.Decimals,
                                         4)} {paymentToken.Symbol} ({MathUtils.DecimalAdjustAndRoundToSignificantDigits(
                                         request.ProviderPaymentAmount / request.ProviderLimit,
                                         paymentToken.Decimals,
                                         4)} {paymentToken.Symbol} each)

            🔗 Link: https://metricsdao.xyz/app/market/{laborMarket.Address}/request/{request.RequestId}

            🗓 Deadlines
            
            Claim to submit: {request.SignalExpiration:ddd, dd MMM HH:mm UTC}

            Final submission: {request.SubmissionExpiration:ddd, dd MMM HH:mm UTC}
            """;
    }

    public async Task ShareRecentChallengesAsync()
    {
        if (!TwitterOptions.EnableAutoPost)
        {
            Logger.LogWarning("Skipping twitter post: Disabled");
            return;
        }

        var now = DateTimeOffset.UtcNow;

        var requests = await DbContext.LaborMarketRequests
            .Include(x => x.LaborMarket)
            .Include(x => x.ProviderPaymentToken)
            .Include(x => x.ReviewerPaymentToken)
            .Where(x => x.TweetId == null && x.SignalExpiration > now)
            .ToListAsync();

        if (requests.Count == 0)
        {
            Logger.LogWarning("Skipping twitter post: No requests found");
            return;
        }

        var pricePools = requests.GroupBy(x => x.ProviderPaymentToken)
            .Select(x => (
                PaymentToken: x.Key!,
                Amount: x.Aggregate(BigInteger.Zero, (last, request) => last + request.ProviderPaymentAmount)
            )).ToArray();

        long latestTweetId = await TweetsV2Poster.PostTweetAsync(
            HeadingTemplate(requests.Count, pricePools)
        );

        foreach (var request in requests)
        {
            latestTweetId = await TweetsV2Poster.PostTweetAsync(
                    LaborMarketRequestTemplate(request.LaborMarket!, request, request.ProviderPaymentToken!),
                    latestTweetId
                );

            request.SetTweetId(latestTweetId);
        }

        try
        {
            await DbContext.SaveChangesAsync();
        }
        catch
        {
            await DbContext.SaveChangesAsync();
        }

        Logger.LogInformation("Successfully published twitter post");
    }
}
