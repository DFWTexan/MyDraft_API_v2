﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDraftAPI_v2.Migrations
{
    /// <inheritdoc />
    public partial class _20231120B : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDraftSelections",
                table: "UserDraftSelections");

            migrationBuilder.AlterColumn<int>(
                name: "PlayerID",
                table: "UserDraftSelections",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDraftSelections",
                table: "UserDraftSelections",
                columns: new[] { "LeagueID", "TeamID", "Round" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDraftSelections",
                table: "UserDraftSelections");

            migrationBuilder.AlterColumn<int>(
                name: "PlayerID",
                table: "UserDraftSelections",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDraftSelections",
                table: "UserDraftSelections",
                columns: new[] { "LeagueID", "PlayerID", "TeamID", "Round" });
        }
    }
}