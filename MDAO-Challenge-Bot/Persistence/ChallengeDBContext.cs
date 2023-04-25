using MDAO_Challenge_Bot.Entities;
using MDAO_Challenge_Bot.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace MDAO_Challenge_Bot.Persistence;
public class ChallengeDBContext : DbContext
{
    public DbSet<LaborMarket> LaborMarkets { get; set; }
    public DbSet<LaborMarketRequest> LaborMarketRequests { get; set; }

    public DbSet<AirtableChallenge> AirtableChallenges { get; set; }

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

            b.Property(x => x.PaymentTokenAddress);
            b.Property(x => x.PaymentTokenAmount);

            b.Property(x => x.ClaimSubmitExpiration);
            b.Property(x => x.SubmitExpiration);
            b.Property(x => x.ReviewExpiration);

            b.Property(x => x.Title);
            b.Property(x => x.Description);
            b.Property(x => x.Language);
            b.Property(x => x.ProjectSlugs);

            b.ToTable("LaborMarketRequests");
        });

        modelBuilder.Entity<AirtableChallenge>(b =>
        {
            b.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityAlwaysColumn();
            b.HasKey(x => x.Id);

            b.Property(x => x.BountyProgram);
            b.Property(x => x.Batch);
            b.Property(x => x.Name);
            b.Property(x => x.Description);
            b.Property(x => x.Level);

            b.Property(x => x.PaymentTokenAddress);

            b.HasIndex(x => new { x.Batch, x.Name })
            .IsUnique();

            b.Property(x => x.StartDate);
            b.Property(x => x.EndDate);

            b.ToTable("AirtableChallenges");
        });

        modelBuilder.Entity<TokenContract>(b =>
        {
            b.Property(x => x.Address);
            b.HasKey(x => x.Address);

            b.Property(x => x.Symbol);
            b.Property(x => x.Decimals);

            b.HasMany(x => x.AirtableChallengeUsages)
            .WithOne(x => x.PaymentToken)
            .HasForeignKey(x => x.PaymentTokenAddress)
            .OnDelete(DeleteBehavior.SetNull);

            b.HasMany(x => x.LaborMarketRequestUsages)
            .WithOne(x => x.PaymentToken)
            .HasForeignKey(x => x.PaymentTokenAddress)
            .OnDelete(DeleteBehavior.SetNull);

            b.ToTable("TokenContracts");
        });
    }
}
