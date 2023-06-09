using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC98.Achievement.Migrations
{
    public partial class SetAppIdUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categories_AppId",
                table: "Categories");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_AppId",
                table: "Categories",
                column: "AppId",
                unique: true,
                filter: "[AppId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categories_AppId",
                table: "Categories");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_AppId",
                table: "Categories",
                column: "AppId");
        }
    }
}
