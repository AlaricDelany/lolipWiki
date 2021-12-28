using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LolipWikiWebApplication.DataAccess.Migrations
{
    public partial class AddConfigurationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TAB_CONFIGURATION",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReadArticleControlType = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: false),
                    WriteArticleControlType = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TAB_CONFIGURATION", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TAB_CONFIGURATION");
        }
    }
}
