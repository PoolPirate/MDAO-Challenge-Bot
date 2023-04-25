using Nethereum.ABI.FunctionEncoding.Attributes;

namespace MDAO_Challenge_Bot.Contracts;
public static class ERC20Contract
{
    [Function(Name, "string")]
    public class SymbolFunction
    {
        public const string Name = "symbol";
    }

    [Function(Name, "uint8")]
    public class DecimalsFunction
    {
        public const string Name = "decimals";
    }
}
