using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDraftAPI_v2.Migrations
{
    /// <inheritdoc />
    public partial class ef20231125TableReorg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDraftStatus",
                table: "UserDraftStatus");

            migrationBuilder.DropIndex(
                name: "IX_UserDraftStatus_LeagueID",
                table: "UserDraftStatus");

            migrationBuilder.DropColumn(
                name: "UniverseID",
                table: "UserLeague");

            migrationBuilder.DropColumn(
                name: "UniverseID",
                table: "UserDraftStatus");

            migrationBuilder.DropColumn(
                name: "UserUniqueID",
                table: "UserDraftStatus");

            migrationBuilder.DropColumn(
                name: "UniverseID",
                table: "UserDraftSelections");

            migrationBuilder.DropColumn(
                name: "UserUniqueID",
                table: "UserDraftSelections");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "MyDraftUser",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDraftStatus",
                table: "UserDraftStatus",
                column: "LeagueID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDraftSelections_UserLeague_LeagueID",
                table: "UserDraftSelections",
                column: "LeagueID",
                principalTable: "UserLeague",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDraftSelections_UserLeague_LeagueID",
                table: "UserDraftSelections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDraftStatus",
                table: "UserDraftStatus");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "MyDraftUser");

            migrationBuilder.AddColumn<int>(
                name: "UniverseID",
                table: "UserLeague",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UniverseID",
                table: "UserDraftStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserUniqueID",
                table: "UserDraftStatus",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UniverseID",
                table: "UserDraftSelections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserUniqueID",
                table: "UserDraftSelections",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDraftStatus",
                table: "UserDraftStatus",
                columns: new[] { "UniverseID", "LeagueID" });

            migrationBuilder.CreateIndex(
                name: "IX_UserDraftStatus_LeagueID",
                table: "UserDraftStatus",
                column: "LeagueID");
        }
    }
}
