using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC98.Achievement.Migrations
{
    /// <inheritdoc />
    public partial class AddGrayedIcon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GrayedIconUri",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultGrayedIconUri",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrayedIconUri",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "DefaultGrayedIconUri",
                table: "Categories");
        }
    }
}
