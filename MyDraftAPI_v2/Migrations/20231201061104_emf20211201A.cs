using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDraftAPI_v2.Migrations
{
    /// <inheritdoc />
    public partial class emf20211201A : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PlayerID_LeagueID",
                table: "UserDraftSelections",
                columns: new[] { "PlayerID", "LeagueID" },
                unique: true,
                filter: "[PlayerID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ID",
                table: "Players",
                column: "ID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlayerID_LeagueID",
                table: "UserDraftSelections");

            migrationBuilder.DropIndex(
                name: "IX_ID",
                table: "Players");
        }
    }
}
