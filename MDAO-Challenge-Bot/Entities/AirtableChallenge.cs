using MDAO_Challenge_Bot.Entities;
using System.Text.Json.Serialization;

namespace MDAO_Challenge_Bot.Models;
public class AirtableChallenge
{
    public long Id { get; init; }
    [JsonPropertyName("Bounty Program")]
    public required string BountyProgram { get; init; }
    public required string Batch { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Level { get; init; }

    public string? PaymentTokenAddress { get; init; }

    [JsonPropertyName("Start Date")]
    public required DateTimeOffset StartDate { get; init; }
    [JsonPropertyName("End Date")]
    public required DateTimeOffset EndDate { get; init; }

    public virtual TokenContract? PaymentToken { get; set; } //Navigation Property
}
