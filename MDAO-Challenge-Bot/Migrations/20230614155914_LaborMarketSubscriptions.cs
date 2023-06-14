using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MDAO_Challenge_Bot.Migrations
{
    /// <inheritdoc />
    public partial class LaborMarketSubscriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LaborMarketSubscriptions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    LaborMarketId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    DiscordWebhookURL = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaborMarketSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaborMarketSubscriptions_LaborMarket_LaborMarketId",
                        column: x => x.LaborMarketId,
                        principalTable: "LaborMarket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LaborMarketSubscriptions_LaborMarketId",
                table: "LaborMarketSubscriptions",
                column: "LaborMarketId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LaborMarketSubscriptions");
        }
    }
}
