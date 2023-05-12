using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDraftAPI_v2.Migrations
{
    /// <inheritdoc />
    public partial class emf_10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AAV",
                columns: table => new
                {
                    PlayerID = table.Column<int>(type: "int", nullable: true),
                    StandardValue = table.Column<int>(type: "int", nullable: true),
                    PPRValue = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_AAV_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ADP",
                columns: table => new
                {
                    PlayerID = table.Column<int>(type: "int", nullable: true),
                    StandardValue = table.Column<int>(type: "int", nullable: true),
                    PPRValue = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_ADP_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "DVDB",
                columns: table => new
                {
                    PlayerID = table.Column<int>(type: "int", nullable: true),
                    LeagueID = table.Column<int>(type: "int", nullable: true),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: true),
                    Segment = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_DVDB_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AAV_PlayerID",
                table: "AAV",
                column: "PlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_ADP_PlayerID",
                table: "ADP",
                column: "PlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_DVDB_PlayerID",
                table: "DVDB",
                column: "PlayerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AAV");

            migrationBuilder.DropTable(
                name: "ADP");

            migrationBuilder.DropTable(
                name: "DVDB");
        }
    }
}
