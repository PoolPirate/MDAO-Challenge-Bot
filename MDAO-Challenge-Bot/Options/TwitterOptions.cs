using Common.Configuration;

namespace MDAO_Challenge_Bot.Options;
public class TwitterOptions : Option
{
    public required TimeOnly PostTime { get; set; }

    public required string APIKey { get; set; }
    public required string APIKeySecret { get; set; }
    public required string AccessToken { get; set; }
    public required string AccessTokenSecret { get; set; }
}
