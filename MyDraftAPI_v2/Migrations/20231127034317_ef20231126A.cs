using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDraftAPI_v2.Migrations
{
    /// <inheritdoc />
    public partial class ef20231126A : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDraftSelections",
                table: "UserDraftSelections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MyDraftUser",
                table: "MyDraftUser");

            migrationBuilder.DropColumn(
                name: "UserUniqueID",
                table: "UserLeague");

            migrationBuilder.AddColumn<int>(
                name: "MyDraftUserID",
                table: "UserLeague",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfStarters",
                table: "UserLeague",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "PlayerID",
                table: "UserDraftSelections",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Round",
                table: "UserDraftSelections",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TeamID",
                table: "UserDraftSelections",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "UserDraftSelections",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActiveDate",
                table: "MyDraftUser",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDraftSelections",
                table: "UserDraftSelections",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MyDraftUser",
                table: "MyDraftUser",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_UserLeague_MyDraftUserID",
                table: "UserLeague",
                column: "MyDraftUserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserDraftSelections_LeagueID",
                table: "UserDraftSelections",
                column: "LeagueID");

            migrationBuilder.CreateIndex(
                name: "IX_UserDraftSelections_PlayerID",
                table: "UserDraftSelections",
                column: "PlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_UserDraftSelections_TeamID",
                table: "UserDraftSelections",
                column: "TeamID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDraftSelections_Players_PlayerID",
                table: "UserDraftSelections",
                column: "PlayerID",
                principalTable: "Players",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDraftSelections_UserLeagueTeams_TeamID",
                table: "UserDraftSelections",
                column: "TeamID",
                principalTable: "UserLeagueTeams",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLeague_MyDraftUser_MyDraftUserID",
                table: "UserLeague",
                column: "MyDraftUserID",
                principalTable: "MyDraftUser",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDraftSelections_Players_PlayerID",
                table: "UserDraftSelections");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDraftSelections_UserLeagueTeams_TeamID",
                table: "UserDraftSelections");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLeague_MyDraftUser_MyDraftUserID",
                table: "UserLeague");

            migrationBuilder.DropIndex(
                name: "IX_UserLeague_MyDraftUserID",
                table: "UserLeague");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDraftSelections",
                table: "UserDraftSelections");

            migrationBuilder.DropIndex(
                name: "IX_UserDraftSelections_LeagueID",
                table: "UserDraftSelections");

            migrationBuilder.DropIndex(
                name: "IX_UserDraftSelections_PlayerID",
                table: "UserDraftSelections");

            migrationBuilder.DropIndex(
                name: "IX_UserDraftSelections_TeamID",
                table: "UserDraftSelections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MyDraftUser",
                table: "MyDraftUser");

            migrationBuilder.DropColumn(
                name: "MyDraftUserID",
                table: "UserLeague");

            migrationBuilder.DropColumn(
                name: "NumberOfStarters",
                table: "UserLeague");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "UserDraftSelections");

            migrationBuilder.DropColumn(
                name: "LastActiveDate",
                table: "MyDraftUser");

            migrationBuilder.AddColumn<string>(
                name: "UserUniqueID",
                table: "UserLeague",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TeamID",
                table: "UserDraftSelections",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Round",
                table: "UserDraftSelections",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
                columns: new[] { "LeagueID", "TeamID", "Round" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_MyDraftUser",
                table: "MyDraftUser",
                column: "UserUniqueID");
        }
    }
}
