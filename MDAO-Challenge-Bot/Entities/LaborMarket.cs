using MDAO_Challenge_Bot.Entities;

namespace MDAO_Challenge_Bot.Models;
public class LaborMarket
{
    public required string Address { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }

    public virtual List<LaborMarketRequest>? Requests { get; private set; } //Navigation Property
    public virtual List<LaborMarketSubscription>? Subscriptions { get; private set; } //Navigation Property
}
