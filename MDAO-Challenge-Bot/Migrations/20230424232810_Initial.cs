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
                name: "LaborMarketRequests",
                columns: table => new
                {
                    RequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LaborMarketId = table.Column<long>(type: "bigint", nullable: false),
                    Requester = table.Column<string>(type: "text", nullable: false),
                    IPFSUri = table.Column<string>(type: "text", nullable: false),
                    PaymentTokenAddress = table.Column<string>(type: "text", nullable: false),
                    PaymentTokenAmount = table.Column<BigInteger>(type: "numeric", nullable: false),
                    ClaimSubmitExpiration = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    SubmitExpiration = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    ReviewExpiration = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaborMarketRequests", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_LaborMarketRequests_LaborMarket_LaborMarketId",
                        column: x => x.LaborMarketId,
                        principalTable: "LaborMarket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LaborMarketRequests_LaborMarketId",
                table: "LaborMarketRequests",
                column: "LaborMarketId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LaborMarketRequests");

            migrationBuilder.DropTable(
                name: "LaborMarket");
        }
    }
}
