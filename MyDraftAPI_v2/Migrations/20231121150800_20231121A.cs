using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDraftAPI_v2.Migrations
{
    /// <inheritdoc />
    public partial class _20231121A : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "UserDraftSelections",
                newName: "TiemStamp");

            migrationBuilder.AlterColumn<int>(
                name: "PlayerID",
                table: "UserDraftSelections",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TiemStamp",
                table: "UserDraftSelections",
                newName: "CreateDate");

            migrationBuilder.AlterColumn<int>(
                name: "PlayerID",
                table: "UserDraftSelections",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
