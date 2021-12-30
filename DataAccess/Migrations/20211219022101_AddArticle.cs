using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LolipWikiWebApplication.DataAccess.Migrations
{
    public partial class AddArticle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TAB_ARTICLE",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TAB_ARTICLE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TAB_ARTICLE_TAB_USER_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "TAB_USER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TAB_ARTICLE_VERSION",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticleId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: false),
                    Revision = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChangedById = table.Column<long>(type: "bigint", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TAB_ARTICLE_VERSION", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TAB_ARTICLE_VERSION_TAB_ARTICLE_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "TAB_ARTICLE",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TAB_ARTICLE_VERSION_TAB_USER_ChangedById",
                        column: x => x.ChangedById,
                        principalTable: "TAB_USER",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TAB_ARTICLE_VERSION_TAB_USER_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "TAB_USER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TAB_ARTICLE_CreatorUserId",
                table: "TAB_ARTICLE",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TAB_ARTICLE_VERSION_ArticleId",
                table: "TAB_ARTICLE_VERSION",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_TAB_ARTICLE_VERSION_ChangedById",
                table: "TAB_ARTICLE_VERSION",
                column: "ChangedById");

            migrationBuilder.CreateIndex(
                name: "IX_TAB_ARTICLE_VERSION_CreatedById",
                table: "TAB_ARTICLE_VERSION",
                column: "CreatedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TAB_ARTICLE_VERSION");

            migrationBuilder.DropTable(
                name: "TAB_ARTICLE");
        }
    }
}
