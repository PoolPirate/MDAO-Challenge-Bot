﻿using Common.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using MDAO_Challenge_Bot.Options;
using MDAO_Challenge_Bot.Persistence;
using MDAO_Challenge_Bot.Services.Sharing;
using MDAO_Challenge_Bot.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MDAO_Challenge_Bot.Services.Docs;
public class SheetsSyncRunner : Scoped
{
    [Inject]
    private readonly SheetsService SheetsService = null!;
    [Inject]
    private readonly TelegramSharingClient TelegramSharingClient = null!;
    [Inject]
    private readonly SpreadSheetSyncOptions SyncOptions = null!;
    [Inject]
    private readonly ChallengeDBContext DbContext = null!;
    [Inject]
    private readonly ILogger<SheetsSyncRunner> Logger = null!;

    public async Task SyncSpreadSheetAsync()
    {
        if (!SyncOptions.Enabled)
        {
            Logger.LogWarning("Skipping spreadsheet sync: Disabled");
            return;
        }

        Logger.LogInformation("Syncing spreadsheet...");

        var requests = await DbContext.LaborMarketRequests
            .Include(x => x.LaborMarket)
            .Include(x => x.PaymentToken)
            .Where(x => x.ClaimSubmitExpiration > DateTimeOffset.UtcNow)
            .ToListAsync();

        Logger.LogDebug("Clearing sheet...");
        await SheetsService.Spreadsheets.Values.Clear(new ClearValuesRequest(), SyncOptions.SpreadSheetId, "A1:Z32")
            .ExecuteAsync();

        if (requests.Count > 0)
        {
            var request = SheetsService.Spreadsheets.Values.Update(new ValueRange()
            {
                Values = requests.Select(request => (IList<object>)new List<object>()
                {
                    request.Title,
                    request.Description!,
                    $"https://metricsdao.xyz/app/market/{request.LaborMarket!.Address}/request/{request.RequestId}",
                    request.SubmitExpiration.Date.ToShortDateString(),
                    $"{MathUtils.DecimalAdjustAndRoundToSignificantDigits(
                    request.PaymentTokenAmount,
                    request.PaymentToken!.Decimals,
                    4)} {request.PaymentToken.Symbol}"
                }).ToList()
            }, SyncOptions.SpreadSheetId, "A1:Z32");

            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            await request.ExecuteAsync();
        }

        Logger.LogInformation("Sync successful");

        await TelegramSharingClient.ShareSyncNotificationAsync(requests.Count);
    }
}