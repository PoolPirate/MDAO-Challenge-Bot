using Common.Services;
using Nethereum.Contracts.Standards.ERC20.ContractDefinition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDAO_Challenge_Bot.Services.Contracts;
public class ERC20ContractService : Singleton
{
    [Inject]
    private readonly SmartContractService SmartContractService = null!;

    public async Task<string> GetSymbolAsync(string tokenAddress)
        => await SmartContractService.CallAsync<SymbolFunction, string>(tokenAddress);

    public async Task<byte> GetDecimalsAsync(string tokenAddress)
        => await SmartContractService.CallAsync<DecimalsFunction, byte>(tokenAddress);
}
