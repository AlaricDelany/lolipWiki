using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LolipWikiWebApplication.DataAccess.Migrations
{
    public partial class AddLockedUserMembers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LockedBy",
                table: "TAB_USER",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockedDateTime",
                table: "TAB_USER",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TAB_USER_LockedBy",
                table: "TAB_USER",
                column: "LockedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_TAB_USER_TAB_USER_LockedBy",
                table: "TAB_USER",
                column: "LockedBy",
                principalTable: "TAB_USER",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TAB_USER_TAB_USER_LockedBy",
                table: "TAB_USER");

            migrationBuilder.DropIndex(
                name: "IX_TAB_USER_LockedBy",
                table: "TAB_USER");

            migrationBuilder.DropColumn(
                name: "LockedBy",
                table: "TAB_USER");

            migrationBuilder.DropColumn(
                name: "LockedDateTime",
                table: "TAB_USER");
        }
    }
}
