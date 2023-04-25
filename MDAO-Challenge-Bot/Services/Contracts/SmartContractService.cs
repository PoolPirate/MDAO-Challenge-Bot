using Common.Services;
using MDAO_Challenge_Bot.Contracts;
using Nethereum.Contracts;
using Nethereum.Web3;

namespace MDAO_Challenge_Bot.Services.Contracts;
public class SmartContractService : Singleton
{
    [Inject]
    private readonly Web3 Web3 = null!;

    public Contract GetLaborMarket(string address)
    {
        return Web3.Eth.GetContract(LaborMarketContract.ABI, address);
    }

    public async Task<T> DecodeContractCallAsync<T>(string transactionHash)
        where T : FunctionMessage, new()
    {

        var receipt = await Web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionHash);
        return receipt.DecodeTransactionToFunctionMessage<T>();
    }

    public async Task<TResult> CallAsync<TFunction, TResult>(string contractAddress, TFunction? message = null)
        where TFunction : FunctionMessage, new()
    {
        var handler = Web3.Eth.GetContractQueryHandler<TFunction>();
        return await handler.QueryAsync<TResult>(contractAddress, message!);
    }
}
