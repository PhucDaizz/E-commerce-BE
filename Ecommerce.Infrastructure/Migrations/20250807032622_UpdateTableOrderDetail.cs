using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableOrderDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductSizeId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "808e47f5-a733-42ab-8e31-b6af349bfd90",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ffd50550-9e54-4dc5-9403-8531d82e0dd3", "AQAAAAIAAYagAAAAEDb46I7SELPJukPemeQ6UCvRnyEQ5kz7oivsgUHV2nyVYYqTyiSrVGxu7dj7ZAoWmw==", "608e1e85-6e06-4c94-af7c-50dad3589a3c" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductSizeId",
                table: "OrderDetails",
                column: "ProductSizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_ProductSizes_ProductSizeId",
                table: "OrderDetails",
                column: "ProductSizeId",
                principalTable: "ProductSizes",
                principalColumn: "ProductSizeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_ProductSizes_ProductSizeId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProductSizeId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ProductSizeId",
                table: "OrderDetails");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "808e47f5-a733-42ab-8e31-b6af349bfd90",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b28b3dc4-7cd9-4497-a9ff-d5e00eaffa79", "AQAAAAIAAYagAAAAECbmlHNWTbUOowlOWcaqG7HykR5+iU4dmpkE3pUB0wmAE0DGHEsRcfueJV9ymv4TOg==", "a59847d2-73d8-49c2-ad97-bcfd5082bc73" });
        }
    }
}
