namespace MDAO_Challenge_Bot.Models;
public class LaborMarket
{
    public required long Id { get; init; }
    public required string Address { get; init; }
    public required string Name { get; init; }

    public required ulong LastUpdatedAtBlockHeight { get; set; }

    public virtual List<LaborMarketRequest>? Requests { get; init; } //Navigation Property
}
