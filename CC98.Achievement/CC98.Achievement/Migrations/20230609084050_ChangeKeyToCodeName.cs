using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC98.Achievement.Migrations
{
    /// <inheritdoc />
    public partial class ChangeKeyToCodeName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Categories_CategoryId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Records_Items_AchievementId",
                table: "Records");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Records",
                table: "Records");

            migrationBuilder.DropIndex(
                name: "IX_Records_AchievementId",
                table: "Records");

            migrationBuilder.DropIndex(
                name: "IX_Records_AchievementId_IsCompleted_Time",
                table: "Records");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Items_CategoryId_CodeName",
                table: "Items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Items",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_CategoryId",
                table: "Items");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Categories_CodeName",
                table: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "AchievementId",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Categories");

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "Records",
                type: "nvarchar(256)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AchievementName",
                table: "Records",
                type: "nvarchar(256)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "Items",
                type: "nvarchar(256)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDynamic",
                table: "Items",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Records",
                table: "Records",
                columns: new[] { "CategoryName", "AchievementName", "UserName" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Items",
                table: "Items",
                columns: new[] { "CategoryName", "CodeName" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "CodeName");

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

            migrationBuilder.CreateIndex(
                name: "IX_Records_CategoryName_AchievementName_IsCompleted_Time",
                table: "Records",
                columns: new[] { "CategoryName", "AchievementName", "IsCompleted", "Time" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_CategoryName_SortOrder",
                table: "Items",
                columns: new[] { "CategoryName", "SortOrder" });

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Categories_CategoryName",
                table: "Items",
                column: "CategoryName",
                principalTable: "Categories",
                principalColumn: "CodeName",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Items_CategoryName_AchievementName",
                table: "Records",
                columns: new[] { "CategoryName", "AchievementName" },
                principalTable: "Items",
                principalColumns: new[] { "CategoryName", "CodeName" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Categories_CategoryName",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Records_Items_CategoryName_AchievementName",
                table: "Records");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Records",
                table: "Records");

            migrationBuilder.DropIndex(
                name: "IX_Records_CategoryName_AchievementName_IsCompleted_Time",
                table: "Records");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Items",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_CategoryName_SortOrder",
                table: "Items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "AchievementName",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IsDynamic",
                table: "Items");

            migrationBuilder.AddColumn<int>(
                name: "AchievementId",
                table: "Records",
                type: "int",
                maxLength: 256,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Records",
                table: "Records",
                columns: new[] { "AchievementId", "UserName" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Items_CategoryId_CodeName",
                table: "Items",
                columns: new[] { "CategoryId", "CodeName" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Items",
                table: "Items",
                column: "Id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Categories_CodeName",
                table: "Categories",
                column: "CodeName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Records_AchievementId",
                table: "Records",
                column: "AchievementId");

            migrationBuilder.CreateIndex(
                name: "IX_Records_AchievementId_IsCompleted_Time",
                table: "Records",
                columns: new[] { "AchievementId", "IsCompleted", "Time" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_CategoryId",
                table: "Items",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Categories_CategoryId",
                table: "Items",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Items_AchievementId",
                table: "Records",
                column: "AchievementId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
