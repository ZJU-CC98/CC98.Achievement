using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC98.Achievement.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CodeName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AppId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserCount = table.Column<int>(type: "int", nullable: false),
                    AppIconUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultIconUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultHideIconUri = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CodeName);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    face = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.UniqueConstraint("AK_Users_UserName", x => x.UserName);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    CodeName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IconUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reward = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    MaxValue = table.Column<int>(type: "int", nullable: true),
                    IsDynamic = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => new { x.CategoryName, x.CodeName });
                    table.ForeignKey(
                        name: "FK_Items_Categories_CategoryName",
                        column: x => x.CategoryName,
                        principalTable: "Categories",
                        principalColumn: "CodeName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Records",
                columns: table => new
                {
                    CategoryName = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    AchievementName = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CurrentValue = table.Column<int>(type: "int", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    Time = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Records", x => new { x.CategoryName, x.AchievementName, x.UserName });
                    table.ForeignKey(
                        name: "FK_Records_Items_CategoryName_AchievementName",
                        columns: x => new { x.CategoryName, x.AchievementName },
                        principalTable: "Items",
                        principalColumns: new[] { "CategoryName", "CodeName" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_AppId",
                table: "Categories",
                column: "AppId",
                unique: true,
                filter: "[AppId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_DisplayName",
                table: "Categories",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CategoryName_SortOrder",
                table: "Items",
                columns: new[] { "CategoryName", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Records_CategoryName_AchievementName_IsCompleted_Time",
                table: "Records",
                columns: new[] { "CategoryName", "AchievementName", "IsCompleted", "Time" });

            migrationBuilder.CreateIndex(
                name: "IX_Records_UserName",
                table: "Records",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_Records_UserName_IsCompleted_Time",
                table: "Records",
                columns: new[] { "UserName", "IsCompleted", "Time" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Records");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
