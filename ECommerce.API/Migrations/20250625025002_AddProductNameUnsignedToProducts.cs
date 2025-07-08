using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.API.Migrations
{
    /// <inheritdoc />
    public partial class AddProductNameUnsignedToProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductNameUnsigned",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "808e47f5-a733-42ab-8e31-b6af349bfd90",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3599ee7c-cc02-4028-acdf-f85682193b6f", "AQAAAAIAAYagAAAAEF8MXbZLoWIFp7C/P2XnVEbrp9Trca6ofhnzfO9I2GrdEzi9Rcxj2cNs5E3ohPfxYw==", "e46bafb1-c4ca-4d7d-855b-10f31af4c4aa" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductNameUnsigned",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "808e47f5-a733-42ab-8e31-b6af349bfd90",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d5248e89-3d71-4d04-b859-36516a2d3a53", "AQAAAAIAAYagAAAAEN7neWVrBDA/4n5PNITtkNUeJg1FnLyASmFgMLWe6CSdMj/FAIUvTNFWCsqAIBYxAw==", "f6c01d97-2c97-4f13-ab2e-86edf943d0d1" });
        }
    }
}
