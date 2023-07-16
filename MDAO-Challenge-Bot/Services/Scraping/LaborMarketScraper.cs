using Common.Services;
using Hangfire;
using MDAO_Challenge_Bot.Contracts;
using MDAO_Challenge_Bot.Entities;
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
using System.Numerics;
using System.Transactions;

namespace MDAO_Challenge_Bot.Services.Scraping;
public class LaborMarketScraper : Singleton
{
    private const int UpdateInterval = 20000;

    [Inject]
    private readonly SmartContractService SmartContractService = null!;
    [Inject]
    private readonly Contracts.ERC20ContractService ERC20ContractService = null!;
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
            try
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
                        var peakBlockHeight = await SmartContractService.GetPeakBlockHeightAsync();
                        await UpdateRequests(laborMarket, peakBlockHeight);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogCritical(ex, "There was an exception trying to scrape labor market with id {id}", laborMarket.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "There was an exception refreshing LaborMarketRequests");
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

    private async Task UpdateRequests(LaborMarket laborMarket, BigInteger peakBlockHeight)
    {
        var contract = SmartContractService.GetLaborMarket(laborMarket.Address);
        var configuredEvent = contract.GetEvent<LaborMarketContract.RequestConfiguredEventDTO>();

        var filter = configuredEvent.CreateFilterInput(
            fromBlock: new BlockParameter(laborMarket.LastUpdatedAtBlockHeight + 1),
            toBlock: new BlockParameter(new HexBigInteger(peakBlockHeight - 150)));
        var logs = await configuredEvent.GetAllChangesAsync(filter);

        if (logs.Count == 0)
        {
            return;
        }

        ulong minimumFailedHeight = 0;

        foreach (var log in logs)
        {
            if (!await ProcessLogAsync(laborMarket.Id, log))
            {
                minimumFailedHeight = minimumFailedHeight > 0
                    ? Math.Min((ulong)log.Log.BlockNumber.Value, minimumFailedHeight)
                    : (ulong)log.Log.BlockNumber.Value;
            }
        }

        ulong processingPeakHeight = minimumFailedHeight > 0
            ? minimumFailedHeight - 1
            : logs.Max(x => x.Log.BlockNumber.ToUlong());

        using var scope = Provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ChallengeDBContext>();

        dbContext.LaborMarkets.Attach(laborMarket);
        laborMarket.LastUpdatedAtBlockHeight = processingPeakHeight;

        await dbContext.SaveChangesAsync();
    }

    private async Task<bool> ProcessLogAsync(long laborMarketId, EventLog<LaborMarketContract.RequestConfiguredEventDTO> log)
    {
        var transactionScope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            using var scope = Provider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ChallengeDBContext>();

            var metadata = await IPFSClient.GetJsonAsync<LaborMarketRequestMetadata>(log.Event.Uri);

            var providerPaymentToken = await dbContext.TokenContracts
                .Where(x => x.Address == log.Event.ProviderPaymentTokenAddress)
                .SingleOrDefaultAsync()
                ?? await LoadTokenContractAsync(log.Event.ProviderPaymentTokenAddress);

            var reviewerPaymentToken = await dbContext.TokenContracts
                .Where(x => x.Address == log.Event.ReviewerPaymentTokenAddress)
                .SingleOrDefaultAsync()
                ?? await LoadTokenContractAsync(log.Event.ReviewerPaymentTokenAddress);

            var request = new LaborMarketRequest()
            {
                RequestId = log.Event.RequestId,
                LaborMarketId = laborMarketId,
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
                ProviderPaymentToken = providerPaymentToken,
                ReviewerPaymentToken = reviewerPaymentToken
            };

            dbContext.LaborMarketRequests.Add(request);
            await dbContext.SaveChangesAsync();

            BackgroundJob.Enqueue<SharingTaskRunner>(runner => runner.ShareLaborMarketRequest(request.Id));

            transactionScope.Complete();
            return true;
        }
        catch (TaskCanceledException)
        {
            Logger.LogWarning("There was a timeout while processing tx logs");
            return false;
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, "There was an exception while processing logs!");
            return false;
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
}
