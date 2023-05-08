using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDraftAPI_v2.Migrations
{
    /// <inheritdoc />
    public partial class emf_03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DepthChart",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerID = table.Column<int>(type: "int", nullable: false),
                    PositionID = table.Column<int>(type: "int", nullable: false),
                    TeamID = table.Column<int>(type: "int", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    TeamAbbr = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepthChart", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DepthChart_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepthChart_Positions_PositionID",
                        column: x => x.PositionID,
                        principalTable: "Positions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepthChart_ProTeams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "ProTeams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Schedule",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Season = table.Column<int>(type: "int", nullable: true),
                    Week = table.Column<int>(type: "int", nullable: true),
                    HomeTeam = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    AwayTeam = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    GameDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedule", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserDraftSelections",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeagueID = table.Column<int>(type: "int", nullable: false),
                    PlayerID = table.Column<int>(type: "int", nullable: false),
                    TeamID = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsKeeper = table.Column<bool>(type: "bit", nullable: true),
                    Round = table.Column<int>(type: "int", nullable: true),
                    Pick = table.Column<int>(type: "int", nullable: true),
                    OverallPick = table.Column<int>(type: "int", nullable: true),
                    PositionPick = table.Column<int>(type: "int", nullable: true),
                    PositionRound = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDraftSelections", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserLeagueTeams",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeagueID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Abbreviation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DraftPosition = table.Column<int>(type: "int", nullable: false),
                    Owner = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLeagueTeams", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserLeagueTeams_UserLeague_LeagueID",
                        column: x => x.LeagueID,
                        principalTable: "UserLeague",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepthChart_PlayerID",
                table: "DepthChart",
                column: "PlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_DepthChart_PositionID",
                table: "DepthChart",
                column: "PositionID");

            migrationBuilder.CreateIndex(
                name: "IX_DepthChart_TeamID",
                table: "DepthChart",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_UserLeagueTeams_LeagueID",
                table: "UserLeagueTeams",
                column: "LeagueID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepthChart");

            migrationBuilder.DropTable(
                name: "Schedule");

            migrationBuilder.DropTable(
                name: "UserDraftSelections");

            migrationBuilder.DropTable(
                name: "UserLeagueTeams");
        }
    }
}
