using Common.Services;
using MDAO_Challenge_Bot.Entities;
using MDAO_Challenge_Bot.Entities.LaborMarketSubscriptions;
using MDAO_Challenge_Bot.Models;
using MDAO_Challenge_Bot.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MDAO_Challenge_Bot.Services.Sharing;
public class SubscriptionSharingClient : Scoped
{
    [Inject]
    private readonly ChallengeDBContext DbContext = null!;

    [Inject]
    private readonly DiscordSharingClient DiscordSharingClient = null!;

    public async Task ShareAsync(LaborMarket laborMarket, LaborMarketRequest request, TokenContract paymentToken)
    {
        var subscriptions = await DbContext.LaborMarketSubscriptions
            .Where(x => x.LaborMarketId == laborMarket.Id)
            .ToArrayAsync();

        foreach(var subscription in subscriptions)
        {
            switch (subscription.Type)
            {
                case LaborMarketSubscriptionType.DiscordWebhook:
                    await DiscordSharingClient.ShareToWebhookAsync(((DiscordWebhookSubscription)subscription).DiscordWebhookURL, laborMarket, request, paymentToken);
                    return;
                default:
                    throw new InvalidOperationException("Subscription type not supported!");
            }
        }
    }
}
