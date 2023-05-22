using Common.Services;
using MDAO_Challenge_Bot.Entities;
using MDAO_Challenge_Bot.Models;
using MDAO_Challenge_Bot.Options;
using MDAO_Challenge_Bot.Utils;
using Microsoft.Extensions.Logging;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;

namespace MDAO_Challenge_Bot.Services.Sharing;
public class TelegramSharingClient : Singleton
{
    [Inject]
    private readonly BotClient TelegramClient = null!;

    [Inject]
    private readonly TelegramOptions TelegramOptions = null!;

    private static string SyncNotificationTemplate(int requestCount)
    {
        return
        $"""
        Spreadsheet sync completed at {DateTimeOffset.UtcNow:f}
        {requestCount} challenges inserted.
        """;
    }

    public async Task ShareSyncNotificationAsync(int requestCount)
    {
        if (!TelegramOptions.EnableSpreadSheetSyncNotification)
        {
            Logger.LogWarning("Skipping Telegram sync notification: Disabled");
            return;
        }

        string message = SyncNotificationTemplate(requestCount);
        await TelegramClient.SendMessageAsync(TelegramOptions.SpreadSheetSyncNotificationChatId, message);

        Logger.LogInformation("Successfully shared sync notification");
    }

    private static string LaborMarketRequestTemplate(LaborMarket laborMarket, LaborMarketRequest request, TokenContract paymentToken)
    {
        return
        $""" 
        @sovsignal New challenge posted:
        Title: {request.Title}
        Value: {MathUtils.DecimalAdjustAndRoundToSignificantDigits(
                                         request.PaymentTokenAmount,
                                         paymentToken.Decimals,
                                         4)} {paymentToken.Symbol}
        Link: https://metricsdao.xyz/app/market/{laborMarket.Address}/request/{request.RequestId}
        
        Deadlines
        Claim to submit: {request.ClaimSubmitExpiration:ddd, dd MMM HH:mm UTC}
        Final submission: {request.SubmitExpiration:ddd, dd MMM HH:mm UTC}
        """;
    }

    public async Task ShareLaborMarketRequestAsync(LaborMarket laborMarket, LaborMarketRequest request, TokenContract paymentToken)
    {
        if (!TelegramOptions.EnableLaborMarketRequestNotification)
        {
            Logger.LogWarning("Skipping sharing LaborMarketRequest: Sharing Disabled. Market={marketId}, Id={id}", laborMarket.Id, request.Id);
            return;
        }

        string message = LaborMarketRequestTemplate(laborMarket, request, paymentToken);
        await TelegramClient.SendMessageAsync(TelegramOptions.LaborMarketRequestNotificationChatId, message);

        Logger.LogInformation("Successfully shared LaborMarketRequest notification");
    }
}
