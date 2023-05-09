using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System.Numerics;

#nullable disable

namespace MDAO_Challenge_Bot.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AirtableChallenges",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    StartTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TweetId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table => table.PrimaryKey("PK_AirtableChallenges", x => x.Id));

            migrationBuilder.CreateTable(
                name: "LaborMarket",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LastUpdatedAtBlockHeight = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_LaborMarket", x => x.Id));

            migrationBuilder.CreateTable(
                name: "TokenContracts",
                columns: table => new
                {
                    Address = table.Column<string>(type: "text", nullable: false),
                    Symbol = table.Column<string>(type: "text", nullable: false),
                    Decimals = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_TokenContracts", x => x.Address));

            migrationBuilder.CreateTable(
                name: "LaborMarketRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    RequestId = table.Column<long>(type: "bigint", nullable: false),
                    LaborMarketId = table.Column<long>(type: "bigint", nullable: false),
                    Requester = table.Column<string>(type: "text", nullable: false),
                    IPFSUri = table.Column<string>(type: "text", nullable: false),
                    PaymentTokenAddress = table.Column<string>(type: "text", nullable: false),
                    PaymentTokenAmount = table.Column<BigInteger>(type: "numeric", nullable: false),
                    ClaimSubmitExpiration = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SubmitExpiration = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ReviewExpiration = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Language = table.Column<string>(type: "text", nullable: true),
                    ProjectSlugs = table.Column<string[]>(type: "text[]", nullable: true),
                    TweetId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaborMarketRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaborMarketRequests_LaborMarket_LaborMarketId",
                        column: x => x.LaborMarketId,
                        principalTable: "LaborMarket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaborMarketRequests_TokenContracts_PaymentTokenAddress",
                        column: x => x.PaymentTokenAddress,
                        principalTable: "TokenContracts",
                        principalColumn: "Address",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AirtableChallenges_Title",
                table: "AirtableChallenges",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LaborMarketRequests_LaborMarketId",
                table: "LaborMarketRequests",
                column: "LaborMarketId");

            migrationBuilder.CreateIndex(
                name: "IX_LaborMarketRequests_PaymentTokenAddress",
                table: "LaborMarketRequests",
                column: "PaymentTokenAddress");

            migrationBuilder.CreateIndex(
                name: "IX_LaborMarketRequests_RequestId_LaborMarketId",
                table: "LaborMarketRequests",
                columns: new[] { "RequestId", "LaborMarketId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AirtableChallenges");

            migrationBuilder.DropTable(
                name: "LaborMarketRequests");

            migrationBuilder.DropTable(
                name: "LaborMarket");

            migrationBuilder.DropTable(
                name: "TokenContracts");
        }
    }
}
