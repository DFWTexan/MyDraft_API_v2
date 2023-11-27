using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDraftAPI_v2.Migrations
{
    /// <inheritdoc />
    public partial class ef20231127B : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDraftSelections_UserLeagueTeams_TeamID",
                table: "UserDraftSelections");

            migrationBuilder.DropIndex(
                name: "IX_UserDraftSelections_TeamID",
                table: "UserDraftSelections");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserDraftSelections_TeamID",
                table: "UserDraftSelections",
                column: "TeamID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDraftSelections_UserLeagueTeams_TeamID",
                table: "UserDraftSelections",
                column: "TeamID",
                principalTable: "UserLeagueTeams",
                principalColumn: "ID");
        }
    }
}
