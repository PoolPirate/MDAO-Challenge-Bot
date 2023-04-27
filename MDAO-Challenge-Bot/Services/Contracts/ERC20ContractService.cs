using Common.Services;
using Nethereum.Contracts.Standards.ERC20.ContractDefinition;

namespace MDAO_Challenge_Bot.Services.Contracts;
public class ERC20ContractService : Singleton
{
    [Inject]
    private readonly SmartContractService SmartContractService = null!;

    public async Task<string> GetSymbolAsync(string tokenAddress)
    {
        return await SmartContractService.CallAsync<SymbolFunction, string>(tokenAddress);
    }

    public async Task<byte> GetDecimalsAsync(string tokenAddress)
    {
        return await SmartContractService.CallAsync<DecimalsFunction, byte>(tokenAddress);
    }
}
