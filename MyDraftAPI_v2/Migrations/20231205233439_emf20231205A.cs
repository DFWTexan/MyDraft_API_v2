using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDraftAPI_v2.Migrations
{
    /// <inheritdoc />
    public partial class emf20231205A : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerProjectionSeason",
                columns: table => new
                {
                    PlayerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PassYards = table.Column<int>(type: "int", nullable: false),
                    PassTD = table.Column<int>(type: "int", nullable: false),
                    PassInt = table.Column<int>(type: "int", nullable: false),
                    RushYards = table.Column<int>(type: "int", nullable: false),
                    RushTD = table.Column<int>(type: "int", nullable: false),
                    RushAttempts = table.Column<int>(type: "int", nullable: false),
                    RecYards = table.Column<int>(type: "int", nullable: false),
                    RecTD = table.Column<int>(type: "int", nullable: false),
                    Rec = table.Column<int>(type: "int", nullable: false),
                    FgMade = table.Column<int>(type: "int", nullable: false),
                    FgAtt = table.Column<int>(type: "int", nullable: false),
                    XpMade = table.Column<int>(type: "int", nullable: false),
                    PointsAllowed = table.Column<int>(type: "int", nullable: false),
                    DefInts = table.Column<int>(type: "int", nullable: false),
                    DefFumRec = table.Column<int>(type: "int", nullable: false),
                    Sacks = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerProjectionSeason", x => x.PlayerID);
                });

            migrationBuilder.CreateTable(
                name: "ProjectionsSeason",
                columns: table => new
                {
                    PlayerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatID = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectionsSeason", x => x.PlayerID);
                });

            migrationBuilder.CreateTable(
                name: "ProjectionsWeekly",
                columns: table => new
                {
                    PlayerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatID = table.Column<int>(type: "int", nullable: false),
                    Week = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectionsWeekly", x => x.PlayerID);
                });

            migrationBuilder.CreateTable(
                name: "RosterConfigReference",
                columns: table => new
                {
                    PositionKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    KeyAbbr = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    KeyFullName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    SortValue = table.Column<int>(type: "int", nullable: false),
                    TypeValue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RosterConfigReference", x => x.PositionKey);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerProjectionSeason");

            migrationBuilder.DropTable(
                name: "ProjectionsSeason");

            migrationBuilder.DropTable(
                name: "ProjectionsWeekly");

            migrationBuilder.DropTable(
                name: "RosterConfigReference");
        }
    }
}
