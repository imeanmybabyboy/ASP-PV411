using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_PV411.Migrations
{
    /// <inheritdoc />
    public partial class Slugs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Manufacturers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Manufacturers");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Groups");
        }
    }
}
