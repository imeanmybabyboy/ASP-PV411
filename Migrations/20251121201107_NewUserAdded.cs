using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_PV411.Migrations
{
    /// <inheritdoc />
    public partial class NewUserAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Birthdate", "DeleteAt", "Dk", "Email", "Login", "Name", "RegisterAt", "RoleId", "Salt" },
                values: new object[] { new Guid("5588b536-186b-436c-aa1a-5875d913075f"), null, null, "6029C527F8F6BDE1", "newUser@example.com", "User", "user", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "User", "5875D913075F" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5588b536-186b-436c-aa1a-5875d913075f"));
        }
    }
}
