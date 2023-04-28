using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MDAO_Challenge_Bot.Migrations
{
    /// <inheritdoc />
    public partial class AirtableChallengeRework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AirtableChallenges_TokenContracts_PaymentTokenAddress",
                table: "AirtableChallenges");

            migrationBuilder.DropIndex(
                name: "IX_AirtableChallenges_Batch_Name",
                table: "AirtableChallenges");

            migrationBuilder.DropIndex(
                name: "IX_AirtableChallenges_PaymentTokenAddress",
                table: "AirtableChallenges");

            migrationBuilder.DropColumn(
                name: "Batch",
                table: "AirtableChallenges");

            migrationBuilder.DropColumn(
                name: "BountyProgram",
                table: "AirtableChallenges");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AirtableChallenges");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "AirtableChallenges");

            migrationBuilder.DropColumn(
                name: "PaymentTokenAddress",
                table: "AirtableChallenges");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "AirtableChallenges",
                newName: "StartTimestamp");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "AirtableChallenges",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "AirtableChallenges",
                newName: "EndTimestamp");

            migrationBuilder.AlterColumn<long>(
                name: "SubmitExpiration",
                table: "LaborMarketRequests",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AlterColumn<long>(
                name: "ReviewExpiration",
                table: "LaborMarketRequests",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AlterColumn<long>(
                name: "ClaimSubmitExpiration",
                table: "LaborMarketRequests",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.CreateIndex(
                name: "IX_AirtableChallenges_Title",
                table: "AirtableChallenges",
                column: "Title",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AirtableChallenges_Title",
                table: "AirtableChallenges");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "AirtableChallenges",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "StartTimestamp",
                table: "AirtableChallenges",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "EndTimestamp",
                table: "AirtableChallenges",
                newName: "EndDate");

            migrationBuilder.AlterColumn<decimal>(
                name: "SubmitExpiration",
                table: "LaborMarketRequests",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<decimal>(
                name: "ReviewExpiration",
                table: "LaborMarketRequests",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<decimal>(
                name: "ClaimSubmitExpiration",
                table: "LaborMarketRequests",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "Batch",
                table: "AirtableChallenges",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BountyProgram",
                table: "AirtableChallenges",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AirtableChallenges",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "AirtableChallenges",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentTokenAddress",
                table: "AirtableChallenges",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AirtableChallenges_Batch_Name",
                table: "AirtableChallenges",
                columns: new[] { "Batch", "Name" },
                unique: true);

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
        }
    }
}
