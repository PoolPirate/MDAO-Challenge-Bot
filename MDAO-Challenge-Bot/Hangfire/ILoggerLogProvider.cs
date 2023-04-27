using Hangfire.Logging;
using Microsoft.Extensions.Logging;

namespace MDAO_Challenge_Bot.Hangfire;
public class ILoggerLogProvider : ILogProvider
{
    private readonly ILoggerFactory LoggerFactory;

    public ILoggerLogProvider(ILoggerFactory loggerFactory)
    {
        LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    public ILog GetLogger(string name)
    {
        return new ILoggerLog(LoggerFactory.CreateLogger(name));
    }
}
