using Common.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using MDAO_Challenge_Bot.Options;
using MDAO_Challenge_Bot.Persistence;
using MDAO_Challenge_Bot.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MDAO_Challenge_Bot.Services.Docs;
public class SheetsSyncService : Singleton
{
    [Inject]
    private readonly SheetsService SheetsService = null!;
    [Inject]
    private readonly GoogleOptions GoogleOptions = null!;
    [Inject]
    private readonly IHostApplicationLifetime Lifetime = null!;

    private readonly PeriodicTimer SyncTimer = new PeriodicTimer(TimeSpan.FromHours(1));

    protected override async ValueTask InitializeAsync()
    {
        if (!GoogleOptions.EnableSpreadSheetSync)
        {
            Logger.LogWarning("Spreadsheet sync is disabled! Exiting...");
            return;
        }

        await SyncSpreadSheetAsync();
    }

    protected override async ValueTask RunAsync()
    {
        while (await SyncTimer.WaitForNextTickAsync(Lifetime.ApplicationStopping))
        {
            try
            {
                await SyncSpreadSheetAsync();
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex, "There was an exception while trying to sync spreadsheet");
            }
        }
    }

    private async Task SyncSpreadSheetAsync()
    {
        Logger.LogInformation("Syncing spreadsheet...");

        using var scope = Provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ChallengeDBContext>();

        var requests = await dbContext.LaborMarketRequests
            .Include(x => x.LaborMarket)
            .Include(x => x.PaymentToken)
            .Where(x => x.ClaimSubmitExpiration > DateTimeOffset.UtcNow)
            .ToListAsync();

        Logger.LogDebug("Clearing sheet...");
        await SheetsService.Spreadsheets.Values.Clear(new ClearValuesRequest(), GoogleOptions.SpreadSheetId, "A1:Z32")
            .ExecuteAsync();

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
        }, GoogleOptions.SpreadSheetId, "A1:Z32");

        request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
        await request.ExecuteAsync();

        Logger.LogInformation("Sync successful");
    }
}
