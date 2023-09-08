using System.Numerics;

namespace MDAO_Challenge_Bot.Entities;
public class StatusValue
{
    public const string LastRefreshBlockHeight = "last-refresh-block-height";

    public required string Name { get; init; }
    public required BigInteger Value { get; set; }
}
