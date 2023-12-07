using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDraftAPI_v2.Migrations
{
    /// <inheritdoc />
    public partial class emf20231207A : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyPart",
                table: "Injuries");

            migrationBuilder.RenameColumn(
                name: "Details",
                table: "Injuries",
                newName: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Injuries",
                newName: "Details");

            migrationBuilder.AddColumn<string>(
                name: "BodyPart",
                table: "Injuries",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
