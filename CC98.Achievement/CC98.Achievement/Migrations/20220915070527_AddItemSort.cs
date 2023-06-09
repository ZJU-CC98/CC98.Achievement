using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC98.Achievement.Migrations
{
    public partial class AddItemSort : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Items");
        }
    }
}
