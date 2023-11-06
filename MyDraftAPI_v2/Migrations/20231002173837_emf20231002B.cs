using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDraftAPI_v2.Migrations
{
    /// <inheritdoc />
    public partial class emf20231002B : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Pick",
                table: "UserDraftSelections",
                newName: "PickInRound");

            migrationBuilder.AddColumn<int>(
                name: "UniverseID",
                table: "UserDraftSelections",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UniverseID",
                table: "UserDraftSelections");

            migrationBuilder.RenameColumn(
                name: "PickInRound",
                table: "UserDraftSelections",
                newName: "Pick");
        }
    }
}
