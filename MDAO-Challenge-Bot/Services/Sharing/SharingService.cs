using Common.Services;
using MDAO_Challenge_Bot.Entities;
using MDAO_Challenge_Bot.Models;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Ocsp;

namespace MDAO_Challenge_Bot.Services.Sharing;
public class SharingService : Singleton
{
    [Inject]
    private readonly DiscordSharingClient DiscordSharingClient = null!;

    public async Task ShareAirtableChallengeAsync(AirtableChallenge challenge)
    {
        if (challenge.EndDate < DateTimeOffset.UtcNow)
        {
            Logger.LogWarning("Skipping sharing AirtableChallenge: Expired. Id={id}", challenge.Id);
            return;
        }

        await DiscordSharingClient.ShareAsync(challenge);
    }

    public async Task ShareLaborMarketRequestAsync(LaborMarket laborMarket, LaborMarketRequest request, TokenContract paymentToken)
    {
        if (request.ReviewExpiration < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        {
            Logger.LogWarning("Skipping sharing LaborMarketRequest: Expired. Id={id}", request.Id);
            return;
        }

        await DiscordSharingClient.ShareAsync(laborMarket, request, paymentToken);
    }
}
