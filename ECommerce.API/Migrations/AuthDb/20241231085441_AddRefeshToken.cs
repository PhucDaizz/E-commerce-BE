using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.API.Migrations.AuthDb
{
    /// <inheritdoc />
    public partial class AddRefeshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "808e47f5-a733-42ab-8e31-b6af349bfd90");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiry",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RefreshToken", "RefreshTokenExpiry", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "808e47f5-a733-42ab-8e31-b6af349bfd90", 0, "c341c80f-e733-4fe3-b25a-fbe7bea37290", "ExtendedIdentityUser", "superadmin@ecommerce.com", false, false, null, "SUPERADMIN@ECOMMERCE.COM", "SUPERADMIN@ECOMMERCE.COM", "AQAAAAIAAYagAAAAEDURJAW+sD4rsgPKb0rd/lLpyyXOEmZSE87F0ulARiujaWs9o/i+Tp6OiKOK1+ILVw==", null, false, null, null, "2d6d8de3-7730-4c2d-8043-e4cbc49d7cfc", false, "superadmin@ecommerce.com" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiry",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "808e47f5-a733-42ab-8e31-b6af349bfd90",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f65ab9be-f225-4513-a07e-c389e8195254", "AQAAAAIAAYagAAAAEKkR8tlCwgPxuiqqeP5yK1dn39gkWrigDzFFtCafZyauPjagNmk9ggtT8O8NnFuHtw==", "c83c94f6-063e-4c4f-af31-7fef0b62c7be" });
        }
    }
}
