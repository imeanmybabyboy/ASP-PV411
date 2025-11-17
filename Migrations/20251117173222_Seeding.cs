using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ASP_PV411.Migrations
{
    /// <inheritdoc />
    public partial class Seeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "CanCreate", "CanDelete", "CanRead", "CanUpdate", "Description" },
                values: new object[,]
                {
                    { "Admin", 1, 1, 1, 1, "Full access role" },
                    { "User", 0, 0, 0, 0, "Self registered user" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Birthdate", "DeleteAt", "Dk", "Email", "Login", "Name", "RegisterAt", "RoleId", "Salt" },
                values: new object[] { new Guid("85fe8311-df9c-4322-b093-f94049e318a2"), null, null, "5874CE7F0FB68522", "admin@change.me", "Admin", "Administrator", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Admin", "F94049E318A2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: "User");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("85fe8311-df9c-4322-b093-f94049e318a2"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: "Admin");
        }
    }
}
