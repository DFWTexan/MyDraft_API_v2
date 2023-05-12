using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDraftAPI_v2.Migrations
{
    /// <inheritdoc />
    public partial class emf_13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ID",
                table: "UserDraftSelections");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "UserDraftSelections",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
