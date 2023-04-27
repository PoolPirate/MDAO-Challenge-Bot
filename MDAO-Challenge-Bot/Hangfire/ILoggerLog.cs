using Hangfire.Logging;
using Microsoft.Extensions.Logging;
using Logging = Hangfire.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace MDAO_Challenge_Bot.Hangfire;
public class ILoggerLog : ILog
{
    private static readonly Func<object, Exception, string?> MessageFormatterFunc = MessageFormatter;
    public readonly ILogger Logger;

    public ILoggerLog(ILogger logger)
    {
        Logger = logger;
    }

    public bool Log(Logging.LogLevel logLevel, Func<string> messageFunc, Exception? exception = null)
    {
        var targetLogLevel = ToTargetLogLevel(logLevel);

        if (messageFunc == null)
        {
            return Logger.IsEnabled(targetLogLevel);
        }

        Logger.Log(targetLogLevel, 0, messageFunc(), exception, MessageFormatterFunc!);
        return true;
    }

    private static LogLevel ToTargetLogLevel(Logging.LogLevel logLevel)
    {
        return logLevel switch
        {
            Logging.LogLevel.Trace => LogLevel.Trace,
            Logging.LogLevel.Debug => LogLevel.Debug,
            Logging.LogLevel.Info => LogLevel.Information,
            Logging.LogLevel.Warn => LogLevel.Warning,
            Logging.LogLevel.Error => LogLevel.Error,
            Logging.LogLevel.Fatal => LogLevel.Critical,
            _ => LogLevel.None,
        };
    }

    private static string? MessageFormatter(object state, Exception exception)
    {
        return state.ToString();
    }
}
