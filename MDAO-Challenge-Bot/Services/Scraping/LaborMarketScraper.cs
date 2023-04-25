using Common.Services;
using MDAO_Challenge_Bot.Contracts;
using MDAO_Challenge_Bot.Entities;
using MDAO_Challenge_Bot.Models;
using MDAO_Challenge_Bot.Persistence;
using MDAO_Challenge_Bot.Services.Contracts;
using MDAO_Challenge_Bot.Services.Sharing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nethereum.Contracts.Standards.ERC20;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace MDAO_Challenge_Bot.Services.Scraping;
public class LaborMarketScraper : Singleton
{
    private const int UpdateInterval = 10000;

    [Inject]
    private readonly SmartContractService SmartContractService = null!;
    [Inject]
    private readonly Contracts.ERC20ContractService ERC20ContractService = null!;
    [Inject]
    private readonly SharingService SharingService = null!;
    [Inject]
    private readonly IPFSClient IPFSClient = null!;

    private readonly PeriodicTimer UpdateTimer;

    public LaborMarketScraper()
    {
        UpdateTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(UpdateInterval));
    }

    protected override ValueTask InitializeAsync()
    {
        return base.InitializeAsync();
    }

    protected override async ValueTask RunAsync()
    {
        while (await UpdateTimer.WaitForNextTickAsync())
        {
            var laborMarkets = await TryGetLaborMarketsAsync();

            if (laborMarkets is null)
            {
                continue;
            }

            foreach (var laborMarket in laborMarkets)
            {
                try
                {
                    await UpdateRequests(laborMarket);
                }
                catch (Exception ex)
                {
                    Logger.LogCritical(ex, "There was an exception trying to scrape labor market with id {id}", laborMarket.Id);
                }
            }
        }
    }

    private async Task<LaborMarket[]?> TryGetLaborMarketsAsync()
    {
        try
        {
            using var scope = Provider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ChallengeDBContext>();

            return await dbContext.LaborMarkets.ToArrayAsync();
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, "There was an exception trying to load labor markets!");
            return null;
        }
    }

    private async Task UpdateRequests(LaborMarket laborMarket)
    {
        var contract = SmartContractService.GetLaborMarket(laborMarket.Address);
        var configuredEvent = contract.GetEvent<LaborMarketContract.RequestConfiguredEventDTO>();
        var filter = configuredEvent.CreateFilterInput(
            fromBlock: new BlockParameter(laborMarket.LastUpdatedAtBlockHeight + 1), toBlock: BlockParameter.CreateLatest());
        var logs = await configuredEvent.GetAllChangesAsync(filter);

        if (logs.Count == 0)
        {
            Logger.LogDebug("Successfully refreshed requests for LaborMarket {id} ({name})", laborMarket.Id, laborMarket.Name);
            return;
        }

        ulong peakBlockHeight = logs.Max(x => x.Log.BlockNumber.ToUlong());

        using var scope = Provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ChallengeDBContext>();

        dbContext.LaborMarkets.Attach(laborMarket);
        laborMarket.LastUpdatedAtBlockHeight = peakBlockHeight;

        var requests = new List<LaborMarketRequest>();

        foreach (var log in logs)
        {
            var contractCall = await SmartContractService.DecodeContractCallAsync<LaborMarketContract.SubmitRequestFunctionDTO>(
                log.Log.TransactionHash);

            var metadata = await IPFSClient.GetJsonAsync<LaborMarketRequestMetadata>(contractCall.Uri);

            var paymentToken = await dbContext.TokenContracts
                .FindAsync(contractCall.PaymentToken)
                ?? new Entities.TokenContract()
                {
                    Address = contractCall.PaymentToken,
                    Symbol = await ERC20ContractService.GetSymbolAsync(contractCall.PaymentToken),
                    Decimals = await ERC20ContractService.GetDecimalsAsync(contractCall.PaymentToken),
                };

            requests.Add(new LaborMarketRequest()
            {
                RequestId = (long)log.Event.RequestId,
                LaborMarketId = laborMarket.Id,
                Requester = log.Event.Requester,
                IPFSUri = contractCall.Uri,
                PaymentTokenAddress = log.Event.PaymentToken,
                PaymentTokenAmount = log.Event.PaymentAmount,
                ClaimSubmitExpiration = (ulong)log.Event.SignalExpiration,
                SubmitExpiration = (ulong)log.Event.SubmissionExpiration,
                ReviewExpiration = (ulong)log.Event.ReviewExpiration,
                Title = metadata.Title,
                Description = metadata.Description,
                Language = metadata.Language,
                ProjectSlugs = metadata.ProjectSlugs,
                PaymentToken = paymentToken
            });
        }

        dbContext.LaborMarketRequests.AddRange(requests);
        await dbContext.SaveChangesAsync();

        foreach (var request in requests)
        {
            await HandleNewRequestAsync(laborMarket, request, request.PaymentToken!);
        }
    }

    private async Task HandleNewRequestAsync(LaborMarket laborMarket, LaborMarketRequest request, TokenContract paymentToken)
    {
        await SharingService.ShareLaborMarketRequestAsync(laborMarket, request, paymentToken);
    }
}
