using Common.Configuration;

namespace MDAO_Challenge_Bot.Options;
public class EthereumRPCOptions : Option
{
    public required string ProviderURL { get; set; }
}
