using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDraftAPI_v2.Migrations
{
    /// <inheritdoc />
    public partial class emf20211201D : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserDraftSelections_LeagueID",
                table: "UserDraftSelections");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerID_LeagueID",
                table: "UserDraftSelections",
                columns: new[] { "LeagueID", "TeamID", "OverallPick" },
                unique: true,
                filter: "[TeamID] IS NOT NULL AND [OverallPick] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlayerID_LeagueID",
                table: "UserDraftSelections");

            migrationBuilder.CreateIndex(
                name: "IX_UserDraftSelections_LeagueID",
                table: "UserDraftSelections",
                column: "LeagueID");
        }
    }
}
