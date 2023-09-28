using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDraftAPI_v2.Migrations
{
    /// <inheritdoc />
    public partial class emf20230927B : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vw_PlayerListItem");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vw_PlayerListItem",
                columns: table => new
                {
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ID = table.Column<int>(type: "int", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PPRVal = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    PhotoURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PointsVal = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });
        }
    }
}
