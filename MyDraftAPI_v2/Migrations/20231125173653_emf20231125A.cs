using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDraftAPI_v2.Migrations
{
    /// <inheritdoc />
    public partial class emf20231125A : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserUniqueID",
                table: "UserLeague",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUniqueID",
                table: "UserDraftStatus",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUniqueID",
                table: "UserDraftSelections",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "MyDraftUser",
                columns: table => new
                {
                    UserUniqueID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UserEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyDraftUser", x => x.UserUniqueID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MyDraftUser");

            migrationBuilder.DropColumn(
                name: "UserUniqueID",
                table: "UserLeague");

            migrationBuilder.DropColumn(
                name: "UserUniqueID",
                table: "UserDraftStatus");

            migrationBuilder.DropColumn(
                name: "UserUniqueID",
                table: "UserDraftSelections");
        }
    }
}
