using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDraftAPI_v2.Migrations
{
    /// <inheritdoc />
    public partial class emf_06 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AwayTeam",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "HomeTeam",
                table: "Schedule");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "UserDraftSelections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AwayTeamID",
                table: "Schedule",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HomeTeamID",
                table: "Schedule",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_AwayTeamID",
                table: "Schedule",
                column: "AwayTeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_HomeTeamID",
                table: "Schedule",
                column: "HomeTeamID");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_ProTeams_AwayTeamID",
                table: "Schedule",
                column: "AwayTeamID",
                principalTable: "ProTeams",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_ProTeams_HomeTeamID",
                table: "Schedule",
                column: "HomeTeamID",
                principalTable: "ProTeams",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_ProTeams_AwayTeamID",
                table: "Schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_ProTeams_HomeTeamID",
                table: "Schedule");

            migrationBuilder.DropIndex(
                name: "IX_Schedule_AwayTeamID",
                table: "Schedule");

            migrationBuilder.DropIndex(
                name: "IX_Schedule_HomeTeamID",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "UserDraftSelections");

            migrationBuilder.DropColumn(
                name: "AwayTeamID",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "HomeTeamID",
                table: "Schedule");

            migrationBuilder.AddColumn<string>(
                name: "AwayTeam",
                table: "Schedule",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HomeTeam",
                table: "Schedule",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);
        }
    }
}
