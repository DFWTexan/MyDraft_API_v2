using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDraftAPI_v2.Migrations
{
    /// <inheritdoc />
    public partial class emf_01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Abbr = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserLeague",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Abbr = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Mode = table.Column<int>(type: "int", nullable: false),
                    DraftType = table.Column<int>(type: "int", nullable: false),
                    DraftOrder = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    NumberOfTeams = table.Column<int>(type: "int", nullable: false),
                    NumberOfRounds = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLeague", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Points_LeagueID",
                table: "Points",
                column: "LeagueID");

            migrationBuilder.AddForeignKey(
                name: "FK_Points_UserLeague_LeagueID",
                table: "Points",
                column: "LeagueID",
                principalTable: "UserLeague",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Points_UserLeague_LeagueID",
                table: "Points");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "UserLeague");

            migrationBuilder.DropIndex(
                name: "IX_Points_LeagueID",
                table: "Points");
        }
    }
}
