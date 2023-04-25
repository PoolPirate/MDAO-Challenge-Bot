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

    public required ulong ClaimSubmitExpiration { get; init; }
    public required ulong SubmitExpiration { get; init; }
    public required ulong ReviewExpiration { get; init; }

    public required string Title { get; init; }
    public required string? Description { get; init; }
    public required string? Language { get; init; }
    public required string[]? ProjectSlugs { get; init; }

    public virtual LaborMarket? LaborMarket { get; set; } //Navigation Property
    public virtual TokenContract? PaymentToken { get; set; } //Navigation Property
}
