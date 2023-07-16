using Common.Services;
using MDAO_Challenge_Bot.Entities;
using MDAO_Challenge_Bot.Models;
using Microsoft.Extensions.Logging;

namespace MDAO_Challenge_Bot.Services.Sharing;
public class SharingService : Scoped
{
    [Inject]
    private readonly ILogger<SharingService> Logger = null!;
    [Inject]
    private readonly DiscordSharingClient DiscordSharingClient = null!;
    [Inject]
    private readonly TelegramSharingClient TelegramSharingClient = null!;
    [Inject]
    private readonly SubscriptionSharingClient SubscriptionSharingClient = null!;

    public async Task ShareAirtableChallengeAsync(AirtableChallenge challenge)
    {
        Logger.LogInformation("Sharing AirtableChallenge: Id={id}", challenge.Id);

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

        if (request.EnforcementExpiration < DateTimeOffset.UtcNow)
        {
            Logger.LogWarning("Skipping sharing LaborMarketRequest: Expired. Market={marketId}, Id={id}", laborMarket.Id, request.Id);
            return;
        }

        await DiscordSharingClient.ShareAsync(laborMarket, request, paymentToken);
        await TelegramSharingClient.ShareLaborMarketRequestAsync(laborMarket, request, paymentToken);
        await SubscriptionSharingClient.ShareAsync(laborMarket, request, paymentToken);
    }
}
