using MDAO_Challenge_Bot.Entities;
using System.Numerics;

namespace MDAO_Challenge_Bot.Models;
public class LaborMarketRequest
{
    public long Id { get; init; }

    public required BigInteger RequestId { get; init; }
    public required long LaborMarketId { get; init; }

    public required string Requester { get; init; }
    public required string IPFSUri { get; init; }

    public required ulong ProviderLimit { get; init; }
    public required ulong ReviewerLimit { get; init; }

    public required string ProviderPaymentTokenAddress { get; init; }
    public required BigInteger ProviderPaymentAmount { get; init; }

    public required string ReviewerPaymentTokenAddress { get; init; }
    public required BigInteger ReviewerPaymentAmount { get; init; }

    public required DateTimeOffset SignalExpiration { get; init; }
    public required DateTimeOffset SubmissionExpiration { get; init; }
    public required DateTimeOffset EnforcementExpiration { get; init; }

    public required string Title { get; init; }
    public required string? Description { get; init; }
    public required string? Language { get; init; }
    public required string[]? ProjectSlugs { get; init; }

    public long? TweetId { get; private set; }

    public virtual LaborMarket? LaborMarket { get; init; } //Navigation Property
    public virtual TokenContract? ProviderPaymentToken { get; init; } //Navigation Property
    public virtual TokenContract? ReviewerPaymentToken { get; init; } //Navigation Property

    public void SetTweetId(long tweetId)
    {
        if (TweetId is not null)
        {
            throw new InvalidOperationException("TweetId already set");
        }

        TweetId = tweetId;
    }
}
