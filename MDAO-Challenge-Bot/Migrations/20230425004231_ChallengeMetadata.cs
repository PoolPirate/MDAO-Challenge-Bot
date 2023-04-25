using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MDAO_Challenge_Bot.Migrations
{
    /// <inheritdoc />
    public partial class ChallengeMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "LaborMarketRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "LaborMarketRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ProjectSlugs",
                table: "LaborMarketRequests",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "LaborMarketRequests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AirtableChallenges_Batch_Name",
                table: "AirtableChallenges",
                columns: new[] { "Batch", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AirtableChallenges_Batch_Name",
                table: "AirtableChallenges");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "LaborMarketRequests");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "LaborMarketRequests");

            migrationBuilder.DropColumn(
                name: "ProjectSlugs",
                table: "LaborMarketRequests");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "LaborMarketRequests");
        }
    }
}
