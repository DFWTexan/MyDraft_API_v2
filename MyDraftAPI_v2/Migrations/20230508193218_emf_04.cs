using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDraftAPI_v2.Migrations
{
    /// <inheritdoc />
    public partial class emf_04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProTeamID",
                table: "Players",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_ProTeamID",
                table: "Players",
                column: "ProTeamID");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_ProTeams_ProTeamID",
                table: "Players",
                column: "ProTeamID",
                principalTable: "ProTeams",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_ProTeams_ProTeamID",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_ProTeamID",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "ProTeamID",
                table: "Players");
        }
    }
}
