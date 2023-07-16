using MDAO_Challenge_Bot.Entities;
using MDAO_Challenge_Bot.Entities.LaborMarketSubscriptions;
using MDAO_Challenge_Bot.Models;
using Microsoft.EntityFrameworkCore;

namespace MDAO_Challenge_Bot.Persistence;
public class ChallengeDBContext : DbContext
{
    public DbSet<LaborMarket> LaborMarkets { get; set; }
    public DbSet<LaborMarketRequest> LaborMarketRequests { get; set; }
    public DbSet<LaborMarketSubscription> LaborMarketSubscriptions { get; set; }

    public DbSet<AirtableChallenge> AirtableChallenges { get; set; }

    public DbSet<TokenContract> TokenContracts { get; set; }

    public ChallengeDBContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LaborMarket>(b =>
        {
            b.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityAlwaysColumn();
            b.HasKey(x => x.Id);

            b.Property(x => x.Address);
            b.Property(x => x.Name);

            b.Property(x => x.LastUpdatedAtBlockHeight);

            b.HasMany(x => x.Requests)
            .WithOne(x => x.LaborMarket)
            .HasForeignKey(x => x.LaborMarketId);

            b.HasMany(x => x.Subscriptions)
            .WithOne(x => x.LaborMarket)
            .HasForeignKey(x => x.LaborMarketId);

            b.ToTable("LaborMarket");
        });

        modelBuilder.Entity<LaborMarketRequest>(b =>
        {
            b.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityAlwaysColumn();
            b.HasKey(x => x.Id);

            b.Property(x => x.RequestId);

            b.HasIndex(x => new { x.RequestId, x.LaborMarketId })
            .IsUnique();

            b.Property(x => x.Requester);
            b.Property(x => x.IPFSUri);

            b.Property(x => x.ProviderLimit);
            b.Property(x => x.ReviewerLimit);

            b.Property(x => x.ProviderPaymentTokenAddress);
            b.Property(x => x.ProviderPaymentAmount);

            b.Property(x => x.ReviewerPaymentTokenAddress);
            b.Property(x => x.ReviewerPaymentAmount);

            b.Property(x => x.SignalExpiration);
            b.Property(x => x.SubmissionExpiration);
            b.Property(x => x.EnforcementExpiration);

            b.Property(x => x.Title);
            b.Property(x => x.Description);
            b.Property(x => x.Language);
            b.Property(x => x.ProjectSlugs);

            b.Property(x => x.TweetId)
            .IsRequired(false);

            b.HasOne(x => x.ProviderPaymentToken)
            .WithMany()
            .HasForeignKey(x => x.ProviderPaymentTokenAddress)
            .OnDelete(DeleteBehavior.SetNull);

            b.HasOne(x => x.ReviewerPaymentToken)
            .WithMany()
            .HasForeignKey(x => x.ReviewerPaymentTokenAddress)
            .OnDelete(DeleteBehavior.SetNull);

            b.ToTable("LaborMarketRequests");
        });

        modelBuilder.Entity<LaborMarketSubscription>(b =>
        {
            b.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityAlwaysColumn();
            b.HasKey(x => x.Id);

            b.Property(x => x.LaborMarketId);

            b.Property(x => x.Type);

            b.HasDiscriminator(x => x.Type)
            .HasValue<DiscordWebhookSubscription>(LaborMarketSubscriptionType.DiscordWebhook)
            .IsComplete();

            b.ToTable("LaborMarketSubscriptions");
        });

        modelBuilder.Entity<DiscordWebhookSubscription>(b =>
        {
            b.Property(x => x.DiscordWebhookURL);
        });

        modelBuilder.Entity<AirtableChallenge>(b =>
        {
            b.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityAlwaysColumn();
            b.HasKey(x => x.Id);

            b.HasIndex(x => new { x.Title })
            .IsUnique();

            b.Ignore(x => x.Status);

            b.Property(x => x.StartTimestamp);
            b.Property(x => x.EndTimestamp);

            b.Property(x => x.TweetId)
            .IsRequired(false);

            b.ToTable("AirtableChallenges");
        });

        modelBuilder.Entity<TokenContract>(b =>
        {
            b.Property(x => x.Address);
            b.HasKey(x => x.Address);

            b.Property(x => x.Symbol);
            b.Property(x => x.Decimals);

            b.ToTable("TokenContracts");
        });
    }
}
