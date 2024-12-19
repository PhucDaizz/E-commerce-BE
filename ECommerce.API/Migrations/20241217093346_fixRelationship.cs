using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.API.Migrations
{
    /// <inheritdoc />
    public partial class fixRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Products_ProductsProductID",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_ProductsProductID",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "ProductsProductID",
                table: "CartItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductsProductID",
                table: "CartItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductsProductID",
                table: "CartItems",
                column: "ProductsProductID");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Products_ProductsProductID",
                table: "CartItems",
                column: "ProductsProductID",
                principalTable: "Products",
                principalColumn: "ProductID");
        }
    }
}
