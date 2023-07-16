using System.Globalization;
using System.Numerics;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MDAO_Challenge_Bot.Migrations
{
    /// <inheritdoc />
    public partial class LaborMarketV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaborMarketRequests_TokenContracts_PaymentTokenAddress",
                table: "LaborMarketRequests");

            migrationBuilder.RenameColumn(
                name: "SubmitExpiration",
                table: "LaborMarketRequests",
                newName: "SubmissionExpiration");

            migrationBuilder.RenameColumn(
                name: "ReviewExpiration",
                table: "LaborMarketRequests",
                newName: "SignalExpiration");

            migrationBuilder.RenameColumn(
                name: "PaymentTokenAmount",
                table: "LaborMarketRequests",
                newName: "ReviewerPaymentAmount");

            migrationBuilder.RenameColumn(
                name: "PaymentTokenAddress",
                table: "LaborMarketRequests",
                newName: "ReviewerPaymentTokenAddress");

            migrationBuilder.RenameColumn(
                name: "ClaimSubmitExpiration",
                table: "LaborMarketRequests",
                newName: "EnforcementExpiration");

            migrationBuilder.RenameIndex(
                name: "IX_LaborMarketRequests_PaymentTokenAddress",
                table: "LaborMarketRequests",
                newName: "IX_LaborMarketRequests_ReviewerPaymentTokenAddress");

            migrationBuilder.AlterColumn<BigInteger>(
                name: "RequestId",
                table: "LaborMarketRequests",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<decimal>(
                name: "ProviderLimit",
                table: "LaborMarketRequests",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<BigInteger>(
                name: "ProviderPaymentAmount",
                table: "LaborMarketRequests",
                type: "numeric",
                nullable: false,
                defaultValue: BigInteger.Parse("0", NumberFormatInfo.InvariantInfo));

            migrationBuilder.AddColumn<string>(
                name: "ProviderPaymentTokenAddress",
                table: "LaborMarketRequests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ReviewerLimit",
                table: "LaborMarketRequests",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_LaborMarketRequests_ProviderPaymentTokenAddress",
                table: "LaborMarketRequests",
                column: "ProviderPaymentTokenAddress");

            migrationBuilder.AddForeignKey(
                name: "FK_LaborMarketRequests_TokenContracts_ProviderPaymentTokenAddr~",
                table: "LaborMarketRequests",
                column: "ProviderPaymentTokenAddress",
                principalTable: "TokenContracts",
                principalColumn: "Address",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_LaborMarketRequests_TokenContracts_ReviewerPaymentTokenAddr~",
                table: "LaborMarketRequests",
                column: "ReviewerPaymentTokenAddress",
                principalTable: "TokenContracts",
                principalColumn: "Address",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaborMarketRequests_TokenContracts_ProviderPaymentTokenAddr~",
                table: "LaborMarketRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_LaborMarketRequests_TokenContracts_ReviewerPaymentTokenAddr~",
                table: "LaborMarketRequests");

            migrationBuilder.DropIndex(
                name: "IX_LaborMarketRequests_ProviderPaymentTokenAddress",
                table: "LaborMarketRequests");

            migrationBuilder.DropColumn(
                name: "ProviderLimit",
                table: "LaborMarketRequests");

            migrationBuilder.DropColumn(
                name: "ProviderPaymentAmount",
                table: "LaborMarketRequests");

            migrationBuilder.DropColumn(
                name: "ProviderPaymentTokenAddress",
                table: "LaborMarketRequests");

            migrationBuilder.DropColumn(
                name: "ReviewerLimit",
                table: "LaborMarketRequests");

            migrationBuilder.RenameColumn(
                name: "SubmissionExpiration",
                table: "LaborMarketRequests",
                newName: "SubmitExpiration");

            migrationBuilder.RenameColumn(
                name: "SignalExpiration",
                table: "LaborMarketRequests",
                newName: "ReviewExpiration");

            migrationBuilder.RenameColumn(
                name: "ReviewerPaymentTokenAddress",
                table: "LaborMarketRequests",
                newName: "PaymentTokenAddress");

            migrationBuilder.RenameColumn(
                name: "ReviewerPaymentAmount",
                table: "LaborMarketRequests",
                newName: "PaymentTokenAmount");

            migrationBuilder.RenameColumn(
                name: "EnforcementExpiration",
                table: "LaborMarketRequests",
                newName: "ClaimSubmitExpiration");

            migrationBuilder.RenameIndex(
                name: "IX_LaborMarketRequests_ReviewerPaymentTokenAddress",
                table: "LaborMarketRequests",
                newName: "IX_LaborMarketRequests_PaymentTokenAddress");

            migrationBuilder.AlterColumn<long>(
                name: "RequestId",
                table: "LaborMarketRequests",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(BigInteger),
                oldType: "numeric");

            migrationBuilder.AddForeignKey(
                name: "FK_LaborMarketRequests_TokenContracts_PaymentTokenAddress",
                table: "LaborMarketRequests",
                column: "PaymentTokenAddress",
                principalTable: "TokenContracts",
                principalColumn: "Address",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
