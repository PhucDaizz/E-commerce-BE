using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddColumCreatedAtProductTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ProductTags",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "808e47f5-a733-42ab-8e31-b6af349bfd90",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cf0c2932-e057-44f2-bfb5-1c9766de30b0", "AQAAAAIAAYagAAAAEADhZyEAh/nHMvEdJ53zUdJb59dokczpBrnjH5RWGvXJv+JHrslGnaU5DhZYQQAOzg==", "8bd5aa50-7bf6-4c4a-a673-f959f8c3b30b" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ProductTags");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "808e47f5-a733-42ab-8e31-b6af349bfd90",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f457c1db-cfc7-4e7b-9c0a-b388a465ea18", "AQAAAAIAAYagAAAAEAWSkd8dEL6t1epL9s6Eo+QIs2cV/3ZGEP8qu6NruX73lQuy9jczLKJ9aJJ+HZLZUw==", "900bbf8b-6bf8-4f34-bba5-dffe06db66f5" });
        }
    }
}
