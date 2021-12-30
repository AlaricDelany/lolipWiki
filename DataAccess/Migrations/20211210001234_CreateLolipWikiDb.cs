using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LolipWikiWebApplication.DataAccess.Migrations
{
    public partial class CreateLolipWikiDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TAB_USER",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TwitchUserId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ProfilePictureImagePath = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    SubscriptionState = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TAB_USER", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TAB_USERNAME",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ArchivedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TAB_USERNAME", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TAB_USERNAME_TAB_USER_UserId",
                        column: x => x.UserId,
                        principalTable: "TAB_USER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TAB_USERNAME_UserId",
                table: "TAB_USERNAME",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TAB_USERNAME");

            migrationBuilder.DropTable(
                name: "TAB_USER");
        }
    }
}
