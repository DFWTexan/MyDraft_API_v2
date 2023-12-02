using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDraftAPI_v2.Migrations
{
    /// <inheritdoc />
    public partial class emf20211202A : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_PlayerID_LeagueID",
                table: "UserDraftSelections",
                newName: "IX_LeagueID_TeamID_OverallPick");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_LeagueID_TeamID_OverallPick",
                table: "UserDraftSelections",
                newName: "IX_PlayerID_LeagueID");
        }
    }
}
