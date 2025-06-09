using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.API.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUsernameSuperAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "808e47f5-a733-42ab-8e31-b6af349bfd90",
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "d5248e89-3d71-4d04-b859-36516a2d3a53", "SUPERADMIN", "AQAAAAIAAYagAAAAEN7neWVrBDA/4n5PNITtkNUeJg1FnLyASmFgMLWe6CSdMj/FAIUvTNFWCsqAIBYxAw==", "f6c01d97-2c97-4f13-ab2e-86edf943d0d1", "superadmin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "808e47f5-a733-42ab-8e31-b6af349bfd90",
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "319aefbc-0d3a-4bc5-a2ea-989f6eacf6a2", "SUPERADMIN@ECOMMERCE.COM", "AQAAAAIAAYagAAAAEN0arBcpykIRxk+eQNK5ST63YfI8GDl6t9ormzsmnb1pS+0x+VbWIAX1Z4Kk0ajx7w==", "ddc4d6b0-6737-4e22-a676-2cd6f0fea490", "superadmin@ecommerce.com" });
        }
    }
}
