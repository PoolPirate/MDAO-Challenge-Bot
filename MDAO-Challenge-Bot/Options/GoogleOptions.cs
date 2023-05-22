using Common.Configuration;

namespace MDAO_Challenge_Bot.Options;
public class GoogleOptions : Option
{
    public required string ServiceAccountCredentialFile { get; set; }
}
