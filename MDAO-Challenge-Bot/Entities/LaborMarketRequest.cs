using MDAO_Challenge_Bot.Entities;
using System.Numerics;

namespace MDAO_Challenge_Bot.Models;
public class LaborMarketRequest
{
    public long Id { get; init; }

    public required long RequestId { get; init; }
    public required long LaborMarketId { get; init; }

    public required string Requester { get; init; }
    public required string IPFSUri { get; init; }

    public required string PaymentTokenAddress { get; init; }
    public required BigInteger PaymentTokenAmount { get; init; }

    public required DateTimeOffset ClaimSubmitExpiration { get; init; }
    public required DateTimeOffset SubmitExpiration { get; init; }
    public required DateTimeOffset ReviewExpiration { get; init; }

    public required string Title { get; init; }
    public required string? Description { get; init; }
    public required string? Language { get; init; }
    public required string[]? ProjectSlugs { get; init; }

    public long? TweetId { get; private set; }

    public virtual LaborMarket? LaborMarket { get; init; } //Navigation Property
    public virtual TokenContract? PaymentToken { get; init; } //Navigation Property

    public void SetTweetId(long tweetId)
    {
        if (TweetId is not null)
        {
            throw new InvalidOperationException("TweetId already set");
        }

        TweetId = tweetId;
    }
}
