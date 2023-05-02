using Common.Services;
using MDAO_Challenge_Bot.Entities;
using MDAO_Challenge_Bot.Models;
using MDAO_Challenge_Bot.Options;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Cmp;

namespace MDAO_Challenge_Bot.Services.Sharing;
public class SharingService : Singleton
{
    [Inject]
    private readonly DiscordSharingClient DiscordSharingClient = null!;
    [Inject]
    private readonly SharingOptions SharingOptions = null!;

    public async Task ShareAirtableChallengeAsync(AirtableChallenge challenge)
    {
        Logger.LogInformation("Sharing AirtableChallenge: Id={id}", challenge.Id);

        if (!SharingOptions.ShareAirtable)
        {
            Logger.LogWarning("Skipping sharing AirtableChallenge: Sharing Disabled. Id={id}", challenge.Id);
            return;
        }
        if (challenge.EndTimestamp < DateTimeOffset.UtcNow)
        {
            Logger.LogWarning("Skipping sharing AirtableChallenge: Expired. Id={id}", challenge.Id);
            return;
        }

        await DiscordSharingClient.ShareAsync(challenge);
    }

    public async Task ShareLaborMarketRequestAsync(LaborMarket laborMarket, LaborMarketRequest request, TokenContract paymentToken)
    {
        Logger.LogInformation("Sharing LaborMarketRequest: Market={marketId}, Id={id}", laborMarket.Id, request.Id);

        if (!SharingOptions.ShareAirtable)
        {
            Logger.LogWarning("Skipping sharing LaborMarketRequest: Sharing Disabled. Market={marketId}, Id={id}", laborMarket.Id, request.Id);
            return;
        }
        if (request.ReviewExpiration < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        {
            Logger.LogWarning("Skipping sharing LaborMarketRequest: Expired. Market={marketId}, Id={id}", laborMarket.Id, request.Id);
            return;
        }

        await DiscordSharingClient.ShareAsync(laborMarket, request, paymentToken);
    }
}
