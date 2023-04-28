using Common.Services;
using MDAO_Challenge_Bot.Models;
using MDAO_Challenge_Bot.Options;
using System.Net.Http.Json;

namespace MDAO_Challenge_Bot.Services.Scraping;
public class AirtableChallengeClient : Singleton
{
    [Inject]
    private readonly HttpClient Client = null!;
    [Inject]
    private readonly AirtableOptions AirtableOptions = null!;

    public async Task<AirtableChallenge[]> GetChallengesAsync()
    {
        var challenges = await Client.GetFromJsonAsync<AirtableChallenge[]>(AirtableOptions.APIUrl)
            ?? throw new Exception("Airtable API returned null!");

        return challenges.Where(x => 
            x.Title != "Bounty Question Template - 1. Question Example" &&
            x.StartTimestamp != DateTimeOffset.MinValue &&
            x.EndTimestamp != DateTimeOffset.MaxValue)
        .ToArray();
    }
}
