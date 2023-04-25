using Common.Configuration;

namespace MDAO_Challenge_Bot.Options;
public class DatabaseOptions : Option
{
    public required string PostgresConnectionString { get; set; }
}
