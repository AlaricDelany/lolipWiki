using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LolipWikiWebApplication.DataAccess.Migrations
{
    public partial class AddUserRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TAB_ROLE",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TAB_ROLE", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TAB_USER_ROLE",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TAB_USER_ROLE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TAB_USER_ROLE_TAB_ROLE_RoleId",
                        column: x => x.RoleId,
                        principalTable: "TAB_ROLE",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TAB_USER_ROLE_TAB_USER_UserId",
                        column: x => x.UserId,
                        principalTable: "TAB_USER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TAB_USER_ROLE_RoleId",
                table: "TAB_USER_ROLE",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TAB_USER_ROLE_UserId",
                table: "TAB_USER_ROLE",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TAB_USER_ROLE");

            migrationBuilder.DropTable(
                name: "TAB_ROLE");
        }
    }
}
