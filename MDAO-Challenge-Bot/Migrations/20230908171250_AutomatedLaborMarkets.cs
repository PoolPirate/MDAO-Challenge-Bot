using System.Numerics;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MDAO_Challenge_Bot.Migrations
{
    /// <inheritdoc />
    public partial class AutomatedLaborMarkets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaborMarketRequests_LaborMarket_LaborMarketId",
                table: "LaborMarketRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_LaborMarketSubscriptions_LaborMarket_LaborMarketId",
                table: "LaborMarketSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_LaborMarketSubscriptions_LaborMarketId",
                table: "LaborMarketSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_LaborMarketRequests_LaborMarketId",
                table: "LaborMarketRequests");

            migrationBuilder.DropIndex(
                name: "IX_LaborMarketRequests_RequestId_LaborMarketId",
                table: "LaborMarketRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LaborMarket",
                table: "LaborMarket");

            migrationBuilder.DropColumn(
                name: "LaborMarketId",
                table: "LaborMarketSubscriptions");

            migrationBuilder.DropColumn(
                name: "LaborMarketId",
                table: "LaborMarketRequests");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "LaborMarket");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAtBlockHeight",
                table: "LaborMarket");

            migrationBuilder.AddColumn<string>(
                name: "LaborMarketAddress",
                table: "LaborMarketSubscriptions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LaborMarketAddress",
                table: "LaborMarketRequests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "LaborMarket",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LaborMarket",
                table: "LaborMarket",
                column: "Address");

            migrationBuilder.CreateTable(
                name: "StatusValues",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<BigInteger>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusValues", x => x.Name);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LaborMarketSubscriptions_LaborMarketAddress",
                table: "LaborMarketSubscriptions",
                column: "LaborMarketAddress");

            migrationBuilder.CreateIndex(
                name: "IX_LaborMarketRequests_LaborMarketAddress",
                table: "LaborMarketRequests",
                column: "LaborMarketAddress");

            migrationBuilder.CreateIndex(
                name: "IX_LaborMarketRequests_RequestId_LaborMarketAddress",
                table: "LaborMarketRequests",
                columns: new[] { "RequestId", "LaborMarketAddress" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LaborMarketRequests_LaborMarket_LaborMarketAddress",
                table: "LaborMarketRequests",
                column: "LaborMarketAddress",
                principalTable: "LaborMarket",
                principalColumn: "Address",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LaborMarketSubscriptions_LaborMarket_LaborMarketAddress",
                table: "LaborMarketSubscriptions",
                column: "LaborMarketAddress",
                principalTable: "LaborMarket",
                principalColumn: "Address",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaborMarketRequests_LaborMarket_LaborMarketAddress",
                table: "LaborMarketRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_LaborMarketSubscriptions_LaborMarket_LaborMarketAddress",
                table: "LaborMarketSubscriptions");

            migrationBuilder.DropTable(
                name: "StatusValues");

            migrationBuilder.DropIndex(
                name: "IX_LaborMarketSubscriptions_LaborMarketAddress",
                table: "LaborMarketSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_LaborMarketRequests_LaborMarketAddress",
                table: "LaborMarketRequests");

            migrationBuilder.DropIndex(
                name: "IX_LaborMarketRequests_RequestId_LaborMarketAddress",
                table: "LaborMarketRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LaborMarket",
                table: "LaborMarket");

            migrationBuilder.DropColumn(
                name: "LaborMarketAddress",
                table: "LaborMarketSubscriptions");

            migrationBuilder.DropColumn(
                name: "LaborMarketAddress",
                table: "LaborMarketRequests");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "LaborMarket");

            migrationBuilder.AddColumn<long>(
                name: "LaborMarketId",
                table: "LaborMarketSubscriptions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "LaborMarketId",
                table: "LaborMarketRequests",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "LaborMarket",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.AddColumn<decimal>(
                name: "LastUpdatedAtBlockHeight",
                table: "LaborMarket",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LaborMarket",
                table: "LaborMarket",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_LaborMarketSubscriptions_LaborMarketId",
                table: "LaborMarketSubscriptions",
                column: "LaborMarketId");

            migrationBuilder.CreateIndex(
                name: "IX_LaborMarketRequests_LaborMarketId",
                table: "LaborMarketRequests",
                column: "LaborMarketId");

            migrationBuilder.CreateIndex(
                name: "IX_LaborMarketRequests_RequestId_LaborMarketId",
                table: "LaborMarketRequests",
                columns: new[] { "RequestId", "LaborMarketId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LaborMarketRequests_LaborMarket_LaborMarketId",
                table: "LaborMarketRequests",
                column: "LaborMarketId",
                principalTable: "LaborMarket",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LaborMarketSubscriptions_LaborMarket_LaborMarketId",
                table: "LaborMarketSubscriptions",
                column: "LaborMarketId",
                principalTable: "LaborMarket",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
