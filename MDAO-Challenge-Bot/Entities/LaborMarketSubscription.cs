using MDAO_Challenge_Bot.Models;

namespace MDAO_Challenge_Bot.Entities;
public class LaborMarketSubscription
{
    public required long Id { get; init; }
    public required long LaborMarketId { get; init; }
    public required LaborMarketSubscriptionType Type { get; init; }

    public virtual LaborMarket? LaborMarket { get; set; } //Navigation Property
}
