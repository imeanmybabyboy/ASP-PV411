using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_PV411.Migrations
{
    /// <inheritdoc />
    public partial class NaviGrpProd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Products_GroupId",
                table: "Products",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Groups_GroupId",
                table: "Products",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Groups_GroupId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_GroupId",
                table: "Products");
        }
    }
}
