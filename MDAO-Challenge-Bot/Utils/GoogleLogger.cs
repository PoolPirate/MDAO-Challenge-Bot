using Microsoft.Extensions.Logging;

namespace MDAO_Challenge_Bot.Utils;
internal class GoogleLogger : Google.Apis.Logging.ILogger
{
    private readonly ILoggerFactory Factory;
    private readonly Microsoft.Extensions.Logging.ILogger Logger;

    public GoogleLogger(ILoggerFactory factory, Microsoft.Extensions.Logging.ILogger logger)
    {
        Factory = factory;
        Logger = logger;
    }

    public bool IsDebugEnabled => true;

    public void Debug(string message, params object[] formatArgs)
    {
        Logger.LogDebug(message, formatArgs);
    }

    public void Error(Exception exception, string message, params object[] formatArgs)
    {
        Logger.LogError(exception, message, formatArgs);
    }

    public void Error(string message, params object[] formatArgs)
    {
        Logger.LogError(message, formatArgs);
    }

    public Google.Apis.Logging.ILogger ForType(Type type)
    {
        return new GoogleLogger(Factory, Factory.CreateLogger(type));
    }

    public Google.Apis.Logging.ILogger ForType<T>()
    {
        return new GoogleLogger(Factory, Factory.CreateLogger<T>());
    }

    public void Info(string message, params object[] formatArgs)
    {
        Logger.LogInformation(message, formatArgs);
    }

    public void Warning(string message, params object[] formatArgs)
    {
        Logger.LogWarning(message, formatArgs);
    }
}
