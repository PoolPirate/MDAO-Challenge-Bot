namespace MDAO_Challenge_Bot.Models;
public class LaborMarketRequestMetadata
{
    public required string Title { get; init; }
    public required string? Description { get; init; }
    public required string? Language { get; init; }
    public required string[]? ProjectSlugs { get; init; }
}
