using Common.Services;
using Hangfire;
using Hangfire.Server;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MDAO_Challenge_Bot.Services.Hangfire;
public class HangfireProcessorService : Singleton, IDisposable
{
    [Inject]
    private readonly IHostApplicationLifetime HostApplicationLifetime = null!;

    private IBackgroundProcessingServer? ProcessingServer;

    protected override ValueTask InitializeAsync()
    {
        HostApplicationLifetime.ApplicationStopping.Register(Stop);
        return base.InitializeAsync();
    }

    protected override ValueTask RunAsync()
    {
        Logger.LogDebug("Starting Hangfire Runner...");
        ProcessingServer = new BackgroundJobServer(new BackgroundJobServerOptions());
        Logger.LogInformation("Hangfire Runner started!");
        return ValueTask.CompletedTask;
    }

    public void Stop()
    {
        Logger.LogDebug("Stopping Hangfire Runner...");
        ProcessingServer?.SendStop();
        ProcessingServer?.WaitForShutdown(TimeSpan.FromSeconds(3));
        Logger.LogInformation("Hangfire Runner stopped!");
    }

    public void Dispose()
    {
        ProcessingServer?.Dispose();
        GC.SuppressFinalize(this);
    }
}
