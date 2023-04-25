using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MDAO_Challenge_Bot.Migrations
{
    /// <inheritdoc />
    public partial class RequestGeneratedIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LaborMarketRequests",
                table: "LaborMarketRequests");

            migrationBuilder.AlterColumn<long>(
                name: "RequestId",
                table: "LaborMarketRequests",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "LaborMarketRequests",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LaborMarketRequests",
                table: "LaborMarketRequests",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_LaborMarketRequests_RequestId_LaborMarketId",
                table: "LaborMarketRequests",
                columns: new[] { "RequestId", "LaborMarketId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LaborMarketRequests",
                table: "LaborMarketRequests");

            migrationBuilder.DropIndex(
                name: "IX_LaborMarketRequests_RequestId_LaborMarketId",
                table: "LaborMarketRequests");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "LaborMarketRequests");

            migrationBuilder.AlterColumn<long>(
                name: "RequestId",
                table: "LaborMarketRequests",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LaborMarketRequests",
                table: "LaborMarketRequests",
                column: "RequestId");
        }
    }
}
