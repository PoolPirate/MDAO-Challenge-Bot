using Common.Configuration;

namespace MDAO_Challenge_Bot.Options;
public class AirtableOptions : Option
{
    public required bool Enabled { get; set; }
    public required string APIKey { get; set; }
    public required string BaseId { get; set; }
    public required string TableName { get; set; }
}
