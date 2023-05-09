using Common.Extensions;
using Discord.Webhook;
using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Sheets.v4;
using Hangfire;
using Hangfire.PostgreSql;
using MDAO_Challenge_Bot.Hangfire;
using MDAO_Challenge_Bot.Options;
using MDAO_Challenge_Bot.Persistence;
using MDAO_Challenge_Bot.Services.Docs;
using MDAO_Challenge_Bot.Services.Scraping;
using MDAO_Challenge_Bot.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nethereum.Web3;
using Polly;
using Polly.Contrib.WaitAndRetry;
using System.Reflection;
using Tweetinvi;

namespace InsolvencyTracker.Crawler;

public class Program
{
    public static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
    private static IHost Host = null!;

    public static async Task Main(string[] args)
    {
        Host = CreateHost(args);

        ValidateOptions();
        ConfigureHangfire();

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
        services.AddHttpClient<IPFSClient>(
            client => client.Timeout = TimeSpan.FromSeconds(300))
           .AddTransientHttpErrorPolicy(policy =>
            policy.WaitAndRetryAsync(
                Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromMilliseconds(500), 4)));
        services.AddHttpClient<AirtableChallengeClient>()
           .AddTransientHttpErrorPolicy(policy =>
            policy.WaitAndRetryAsync(
                Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromMilliseconds(500), 4)));
        services.AddHttpClient<SheetsSyncService>()
           .AddTransientHttpErrorPolicy(policy =>
            policy.WaitAndRetryAsync(
                Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromMilliseconds(500), 4)));

        services.AddSingleton(provider =>
        {
            var googleOptions = provider.GetRequiredService<GoogleOptions>();
            var logger = provider.GetRequiredService<ILogger<UserCredential>>();
            var factory = provider.GetRequiredService<ILoggerFactory>();

            ApplicationContext.RegisterLogger(new GoogleLogger(factory, logger));

            return new UserCredential(new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer()
            {
                ClientSecrets =
                    new ClientSecrets()
                    {
                        ClientId = googleOptions.ClientId,
                        ClientSecret = googleOptions.ClientSecret
                    },
                Scopes = new List<string>()
                {
                    "https://www.googleapis.com/auth/spreadsheets"
                }

            }),
            googleOptions.UserId,
            new TokenResponse()
            {
                AccessToken = null,
                IssuedUtc = DateTime.UtcNow.AddDays(-7),
                TokenType = "Bearer",
                ExpiresInSeconds = 3600,
                RefreshToken = googleOptions.RefreshToken,
            });
        });

        services.AddSingleton(provider =>
        {
            var userCredential = provider.GetRequiredService<UserCredential>();

            return new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = userCredential
            });
        });

        services.AddApplication(configuration, Assembly);

        services.AddSingleton(provider =>
        {
            var rpcOptions = provider.GetRequiredService<EthereumRPCOptions>();
            return new Web3(url: rpcOptions.ProviderURL);
        });

        services.AddSingleton(provider =>
        {
            var discordOptions = provider.GetRequiredService<DiscordOptions>();
            return new DiscordWebhookClient(discordOptions.WebhookURL);
        });

        services.AddSingleton<ITwitterClient>(provider =>
        {
            var twitterOptions = provider.GetRequiredService<TwitterOptions>();
            return new TwitterClient(
                twitterOptions.APIKey,
                twitterOptions.APIKeySecret,
                twitterOptions.AccessToken,
                twitterOptions.AccessTokenSecret);
        });

        services.AddDbContextPool<ChallengeDBContext>((provider, options) =>
        {
            var dbOptions = provider.GetRequiredService<DatabaseOptions>();
            options.UseNpgsql(dbOptions.PostgresConnectionString);
        }, 6);
    }

    private static void ValidateOptions()
    {
        _ = Assembly.GetApplicationOptionTypes()
            .Select(x => Host.Services.GetRequiredService(x))
            .ToArray();
    }

    private static void ConfigureHangfire()
    {
        var dbOptions = Host.Services.GetRequiredService<DatabaseOptions>();
        var loggerFac = Host.Services.GetRequiredService<ILoggerFactory>();
        var scopeFac = Host.Services.GetRequiredService<IServiceScopeFactory>();
        var storage = new PostgreSqlStorage(dbOptions.HangfireConnectionString);

        GlobalConfiguration.Configuration
        .UseLogProvider(new ILoggerLogProvider(loggerFac))
        .UseActivator(new IServiceProviderJobActivator(scopeFac))
        .UseStorage(storage);
    }

    private static async Task MigrateDatabaseAsync()
    {
        using var scope = Host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ChallengeDBContext>();

        await dbContext.Database.MigrateAsync();
    }
}
