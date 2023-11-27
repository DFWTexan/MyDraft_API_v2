using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDraftAPI_v2.Migrations
{
    /// <inheritdoc />
    public partial class ef20231127A : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDraftSelections_Players_PlayerID",
                table: "UserDraftSelections");

            migrationBuilder.DropIndex(
                name: "IX_UserDraftSelections_PlayerID",
                table: "UserDraftSelections");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserDraftSelections_PlayerID",
                table: "UserDraftSelections",
                column: "PlayerID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDraftSelections_Players_PlayerID",
                table: "UserDraftSelections",
                column: "PlayerID",
                principalTable: "Players",
                principalColumn: "ID");
        }
    }
}
