using Common.Services;
using Hangfire;
using MDAO_Challenge_Bot.Contracts;
using MDAO_Challenge_Bot.Entities;
using MDAO_Challenge_Bot.Migrations;
using MDAO_Challenge_Bot.Models;
using MDAO_Challenge_Bot.Persistence;
using MDAO_Challenge_Bot.Services.Contracts;
using MDAO_Challenge_Bot.Services.Sharing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nethereum.Contracts;
using Nethereum.Contracts.Standards.ERC20;
using Nethereum.Hex.HexTypes;
using Nethereum.Model;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System.Numerics;
using System.Transactions;

namespace MDAO_Challenge_Bot.Services.Scraping;
public class LaborMarketScraper : Singleton
{
    private const int UpdateInterval = 5000;
    private const int FinalizationBlocks = 3;

    [Inject]
    private readonly SmartContractService SmartContractService = null!;
    [Inject]
    private readonly Contracts.ERC20ContractService ERC20ContractService = null!;
    [Inject]
    private readonly IPFSClient IPFSClient = null!;
    [Inject]
    private readonly Web3 Web3 = null!;

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
            try
            {
                await UpdateRequests();

            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "There was an exception refreshing LaborMarketRequests");
            }

        }
    }

    private async Task UpdateRequests()
    {
        var transactionScope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        try
        {

            using var scope = Provider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ChallengeDBContext>();

            var peakBlockHeight = await SmartContractService.GetPeakBlockHeightAsync();
            var lastUpdatedStatus = await dbContext.StatusValues
                .SingleOrDefaultAsync(x => x.Name == StatusValue.LastRefreshBlockHeight);
            var lastUpdatedHeight = lastUpdatedStatus?.Value ?? 1;

            if (peakBlockHeight - lastUpdatedHeight < FinalizationBlocks + 1)
            {
                return;
            }

            if (lastUpdatedStatus is null)
            {
                lastUpdatedStatus = new StatusValue()
                {
                    Name = StatusValue.LastRefreshBlockHeight,
                    Value = peakBlockHeight
                };
                dbContext.StatusValues.Add(lastUpdatedStatus);
            }
            else
            {
                lastUpdatedStatus.Value = peakBlockHeight;
            }


            var configuredEvent = Web3.Eth.GetEvent<LaborMarketContract.RequestConfiguredEventDTO>();

            var filter = configuredEvent.CreateFilterInput(
                fromBlock: new BlockParameter(new HexBigInteger(lastUpdatedHeight + 1)),
                toBlock: new BlockParameter(new HexBigInteger(peakBlockHeight - FinalizationBlocks)));
            var logs = await configuredEvent.GetAllChangesAsync(filter);

            if (logs.Count == 0)
            {
                return;
            }

            var addedRequests = new List<LaborMarketRequest>();

            foreach (var log in logs)
            {
                var metadata = await IPFSClient.GetJsonAsync<LaborMarketRequestMetadata>(log.Event.Uri);

                var providerPaymentToken = 
                    dbContext.TokenContracts.Local.SingleOrDefault(x => x.Address == log.Event.ProviderPaymentTokenAddress)
                    ?? await dbContext.TokenContracts
                    .Where(x => x.Address == log.Event.ProviderPaymentTokenAddress)
                    .SingleOrDefaultAsync()
                    ?? await LoadTokenContractAsync(log.Event.ProviderPaymentTokenAddress);

                var reviewerPaymentToken =
                    dbContext.TokenContracts.Local.SingleOrDefault(x => x.Address == log.Event.ReviewerPaymentTokenAddress)
                    ?? await dbContext.TokenContracts
                    .Where(x => x.Address == log.Event.ReviewerPaymentTokenAddress)
                    .SingleOrDefaultAsync()
                    ?? await LoadTokenContractAsync(log.Event.ReviewerPaymentTokenAddress);

                var laborMarket = 
                    dbContext.LaborMarkets.Local.SingleOrDefault(x => x.Address == log.Log.Address)
                    ?? await dbContext.LaborMarkets
                    .Where(x => x.Address == log.Log.Address)
                    .SingleOrDefaultAsync()
                    ?? await LoadLaborMarketAsync(log.Log.Address);

                var request = new LaborMarketRequest()
                {
                    RequestId = log.Event.RequestId,
                    LaborMarketAddress = laborMarket.Address,
                    Requester = log.Event.Requester,
                    IPFSUri = log.Event.Uri,
                    ProviderLimit = log.Event.ProviderLimit,
                    ReviewerLimit = log.Event.ReviewerLimit,
                    ProviderPaymentTokenAddress = log.Event.ProviderPaymentTokenAddress,
                    ProviderPaymentAmount = log.Event.ProviderPaymentAmount,
                    ReviewerPaymentTokenAddress = log.Event.ReviewerPaymentTokenAddress,
                    ReviewerPaymentAmount = log.Event.ReviewerPaymentAmount,
                    SignalExpiration = DateTimeOffset.FromUnixTimeSeconds((long)log.Event.SignalExpiration),
                    SubmissionExpiration = DateTimeOffset.FromUnixTimeSeconds((long)log.Event.SubmissionExpiration),
                    EnforcementExpiration = DateTimeOffset.FromUnixTimeSeconds((long)log.Event.EnforcementExpiration),
                    Title = metadata.Title,
                    Description = metadata.Description,
                    Language = metadata.Language,
                    ProjectSlugs = metadata.ProjectSlugs,

                    LaborMarket = laborMarket, //These 3 are recursively auto inserted
                    ProviderPaymentToken = providerPaymentToken,
                    ReviewerPaymentToken = reviewerPaymentToken
                };

                dbContext.LaborMarketRequests.Add(request);
                addedRequests.Add(request);
            }

            await dbContext.SaveChangesAsync();

            foreach(var request in addedRequests)
            {
                BackgroundJob.Enqueue<SharingTaskRunner>(runner => runner.ShareLaborMarketRequest(request.Id));
            }

            transactionScope.Complete();
        }
        finally
        {
            transactionScope.Dispose();
        }
    }

    private async Task<TokenContract> LoadTokenContractAsync(string tokenAddress)
    {
        return new TokenContract()
        {
            Address = tokenAddress,
            Symbol = await ERC20ContractService.GetSymbolAsync(tokenAddress),
            Decimals = await ERC20ContractService.GetDecimalsAsync(tokenAddress),
        };
    }

    private async Task<LaborMarket> LoadLaborMarketAsync(string marketAddress)
    {
        var configuredInitEvent = Web3.Eth.GetEvent<LaborMarketContract.ConfiguredEventDTO>(marketAddress);
        var filterInput = configuredInitEvent.CreateFilterInput();

        var initEvent = (await configuredInitEvent.GetAllChangesAsync(filterInput)).Single();

        var metadata = await IPFSClient.GetJsonAsync<LaborMarketMetadata>(initEvent.Event.Uri);

        return new LaborMarket()
        {
            Address = marketAddress,
            Name = metadata.Title,
            Description = metadata.Description
        };
    }
}
