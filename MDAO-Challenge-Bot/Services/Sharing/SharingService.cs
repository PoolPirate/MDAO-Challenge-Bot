﻿using Common.Services;
using MDAO_Challenge_Bot.Entities;
using MDAO_Challenge_Bot.Models;
using MDAO_Challenge_Bot.Options;
using Microsoft.Extensions.Logging;

namespace MDAO_Challenge_Bot.Services.Sharing;
public class SharingService : Singleton
{
    [Inject]
    private readonly DiscordSharingClient DiscordSharingClient = null!;

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

        if (request.ReviewExpiration < DateTimeOffset.UtcNow)
        {
            Logger.LogWarning("Skipping sharing LaborMarketRequest: Expired. Market={marketId}, Id={id}", laborMarket.Id, request.Id);
            return;
        }

        await DiscordSharingClient.ShareAsync(laborMarket, request, paymentToken);
    }
}
