using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MDAO_Challenge_Bot.Migrations
{
    /// <inheritdoc />
    public partial class PaymentTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentTokenAddress",
                table: "AirtableChallenges",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TokenContracts",
                columns: table => new
                {
                    Address = table.Column<string>(type: "text", nullable: false),
                    Symbol = table.Column<string>(type: "text", nullable: false),
                    Decimals = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_TokenContracts", x => x.Address));

            migrationBuilder.CreateIndex(
                name: "IX_LaborMarketRequests_PaymentTokenAddress",
                table: "LaborMarketRequests",
                column: "PaymentTokenAddress");

            migrationBuilder.CreateIndex(
                name: "IX_AirtableChallenges_PaymentTokenAddress",
                table: "AirtableChallenges",
                column: "PaymentTokenAddress");

            migrationBuilder.AddForeignKey(
                name: "FK_AirtableChallenges_TokenContracts_PaymentTokenAddress",
                table: "AirtableChallenges",
                column: "PaymentTokenAddress",
                principalTable: "TokenContracts",
                principalColumn: "Address",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_LaborMarketRequests_TokenContracts_PaymentTokenAddress",
                table: "LaborMarketRequests",
                column: "PaymentTokenAddress",
                principalTable: "TokenContracts",
                principalColumn: "Address",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AirtableChallenges_TokenContracts_PaymentTokenAddress",
                table: "AirtableChallenges");

            migrationBuilder.DropForeignKey(
                name: "FK_LaborMarketRequests_TokenContracts_PaymentTokenAddress",
                table: "LaborMarketRequests");

            migrationBuilder.DropTable(
                name: "TokenContracts");

            migrationBuilder.DropIndex(
                name: "IX_LaborMarketRequests_PaymentTokenAddress",
                table: "LaborMarketRequests");

            migrationBuilder.DropIndex(
                name: "IX_AirtableChallenges_PaymentTokenAddress",
                table: "AirtableChallenges");

            migrationBuilder.DropColumn(
                name: "PaymentTokenAddress",
                table: "AirtableChallenges");
        }
    }
}
