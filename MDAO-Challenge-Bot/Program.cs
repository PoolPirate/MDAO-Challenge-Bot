using AirtableApiClient;
using Common.Extensions;
using Discord.Webhook;
using MDAO_Challenge_Bot.Options;
using MDAO_Challenge_Bot.Persistence;
using MDAO_Challenge_Bot.Services.Scraping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nethereum.Web3;
using Polly;
using Polly.Contrib.WaitAndRetry;
using System.Reflection;

namespace InsolvencyTracker.Crawler;

public class Program
{
    public static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
    private static IHost Host = null!;

    public static async Task Main(string[] args)
    {
        Host = CreateHost(args);

        ValidateOptions();

        await MigrateDatabaseAsync();

        await Host.Services.InitializeApplicationAsync(Assembly.GetExecutingAssembly());

        Host.Services.RunApplication(Assembly.GetExecutingAssembly());

        await Host.RunAsync();
    }

    private static IHost CreateHost(string[] args)
    {
        var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args);

        ConfigureServices(builder.Services, builder.Configuration);

        return builder.Build();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IPFSClient>()
           .AddTransientHttpErrorPolicy(policy =>
            policy.WaitAndRetryAsync(
                Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromMilliseconds(500), 4)));

        services.AddApplication(configuration, Assembly);

        services.AddSingleton(provider =>
        {
            var rpcOptions = provider.GetRequiredService<EthereumRPCOptions>();
            return new Web3(url: rpcOptions.ProviderURL);
        });

        services.AddSingleton(provider =>
        {
            var sharingOptions = provider.GetRequiredService<SharingOptions>();
            return new DiscordWebhookClient(sharingOptions.DiscordWebhookURL);
        });

        services.AddSingleton(provider =>
        {
            var airtableOptions = provider.GetRequiredService<AirtableOptions>();
            return new AirtableBase(airtableOptions.APIKey, airtableOptions.BaseId);
        });

        services.AddDbContextPool<ChallengeDBContext>((provider, options) =>
        {
            var dbOptions = provider.GetRequiredService<DatabaseOptions>();
            options.UseNpgsql(dbOptions.PostgresConnectionString);
        });
    }

    private static void ValidateOptions()
    {
        Program.Assembly.GetApplicationOptionTypes()
            .Select(x => Host.Services.GetRequiredService(x))
            .ToArray();
    }

    private static async Task MigrateDatabaseAsync()
    {
        using var scope = Host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ChallengeDBContext>();

        await dbContext.Database.MigrateAsync();
    }
}
