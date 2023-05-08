using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDraftAPI_v2.Migrations
{
    /// <inheritdoc />
    public partial class emf_05 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDraftSelections",
                table: "UserDraftSelections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DepthChart",
                table: "DepthChart");

            migrationBuilder.DropIndex(
                name: "IX_DepthChart_PlayerID",
                table: "DepthChart");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "UserDraftSelections");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "DepthChart");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDraftSelections",
                table: "UserDraftSelections",
                columns: new[] { "LeagueID", "PlayerID", "TeamID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DepthChart",
                table: "DepthChart",
                columns: new[] { "PlayerID", "PositionID", "TeamID" });

            migrationBuilder.CreateTable(
                name: "PlayerPosition",
                columns: table => new
                {
                    PlayerID = table.Column<int>(type: "int", nullable: false),
                    PositionID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPosition", x => new { x.PlayerID, x.PositionID });
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerNews_PlayerID",
                table: "PlayerNews",
                column: "PlayerID");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerNews_Players_PlayerID",
                table: "PlayerNews",
                column: "PlayerID",
                principalTable: "Players",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerNews_Players_PlayerID",
                table: "PlayerNews");

            migrationBuilder.DropTable(
                name: "PlayerPosition");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDraftSelections",
                table: "UserDraftSelections");

            migrationBuilder.DropIndex(
                name: "IX_PlayerNews_PlayerID",
                table: "PlayerNews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DepthChart",
                table: "DepthChart");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "UserDraftSelections",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "DepthChart",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDraftSelections",
                table: "UserDraftSelections",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DepthChart",
                table: "DepthChart",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_DepthChart_PlayerID",
                table: "DepthChart",
                column: "PlayerID");
        }
    }
}
