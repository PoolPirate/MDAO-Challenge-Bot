using Common.Services;
using Hangfire;
using MDAO_Challenge_Bot.Entities;
using MDAO_Challenge_Bot.Models;
using MDAO_Challenge_Bot.Options;
using MDAO_Challenge_Bot.Persistence;
using MDAO_Challenge_Bot.Services.Twitter;
using MDAO_Challenge_Bot.Utils;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Text;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace MDAO_Challenge_Bot.Services.Sharing;
public class TwitterSharingRunner : Scoped
{
    [Inject]
    private readonly TweetsV2Poster TweetsV2Poster = null!;
    [Inject]
    private readonly ChallengeDBContext DbContext = null!;

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

            💰 Value: {MathUtils.DecimalAdjustAndRoundToSignificantDigits(
                                         request.PaymentTokenAmount,
                                         paymentToken.Decimals,
                                         4)} {paymentToken.Symbol}

            🔗 Link: https://metricsdao.xyz/app/market/{laborMarket.Address}/request/{request.RequestId}

            🗓 Deadlines
            Claim to submit: {request.ClaimSubmitExpiration:ddd, dd MMM HH:mm UTC}
            Final submission: {request.SubmitExpiration:ddd, dd MMM HH:mm UTC}
            """;
    }

    [AutomaticRetry(Attempts = 1)]
    public async Task ShareRecentChallengesAsync()
    {
        var now = DateTimeOffset.UtcNow;

        var requests = await DbContext.LaborMarketRequests
            .Include(x => x.LaborMarket)
            .Include(x => x.PaymentToken)
            .Where(x => x.TweetId == null && x.ClaimSubmitExpiration > now)
            .ToListAsync();

        if (requests.Count == 0)
        {
            return;
        }

        var pricePools = requests.GroupBy(x => x.PaymentToken)
            .Select(x => (
                PaymentToken: x.Key!,
                Amount: x.Aggregate(BigInteger.Zero, (last, request) => last + request.PaymentTokenAmount)
            )).ToArray();

        long latestTweetId = await TweetsV2Poster.PostTweetAsync(
            HeadingTemplate(requests.Count, pricePools)
        );

        foreach (var request in requests)
        {
            latestTweetId = await TweetsV2Poster.PostTweetAsync(
                    LaborMarketRequestTemplate(request.LaborMarket!, request, request.PaymentToken!),
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
    }
}
