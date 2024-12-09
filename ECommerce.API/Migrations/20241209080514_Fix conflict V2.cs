using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.API.Migrations
{
    /// <inheritdoc />
    public partial class FixconflictV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_ProductID",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Discounts_DiscountsDiscountID",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Shippings_ShippingID",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Products_ProductsProductID",
                table: "ProductImages");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductReviews_Products_ProductsProductID",
                table: "ProductReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSizes_ProductColors_ProductColorsProductColorID",
                table: "ProductSizes");

            migrationBuilder.DropForeignKey(
                name: "FK_Shippings_Orders_OrdersOrderID",
                table: "Shippings");

            migrationBuilder.DropIndex(
                name: "IX_Shippings_OrdersOrderID",
                table: "Shippings");

            migrationBuilder.DropIndex(
                name: "IX_ProductSizes_ProductColorsProductColorID",
                table: "ProductSizes");

            migrationBuilder.DropIndex(
                name: "IX_ProductReviews_ProductsProductID",
                table: "ProductReviews");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_ProductsProductID",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ShippingID",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProductID",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "OrdersOrderID",
                table: "Shippings");

            migrationBuilder.DropColumn(
                name: "ProductColorsProductColorID",
                table: "ProductSizes");

            migrationBuilder.DropColumn(
                name: "ProductsProductID",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "ProductsProductID",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "ShippingID",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "DiscountsDiscountID",
                table: "Orders",
                newName: "DiscountID");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_DiscountsDiscountID",
                table: "Orders",
                newName: "IX_Orders_DiscountID");

            migrationBuilder.AddColumn<int>(
                name: "ProductsProductID",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Shippings_OrderID",
                table: "Shippings",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSizes_ProductColorID",
                table: "ProductSizes",
                column: "ProductColorID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_ProductID",
                table: "ProductReviews",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductID",
                table: "ProductImages",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductsProductID",
                table: "OrderDetails",
                column: "ProductsProductID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_ProductsProductID",
                table: "OrderDetails",
                column: "ProductsProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Discounts_DiscountID",
                table: "Orders",
                column: "DiscountID",
                principalTable: "Discounts",
                principalColumn: "DiscountID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Products_ProductID",
                table: "ProductImages",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReviews_Products_ProductID",
                table: "ProductReviews",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSizes_ProductColors_ProductColorID",
                table: "ProductSizes",
                column: "ProductColorID",
                principalTable: "ProductColors",
                principalColumn: "ProductColorID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shippings_Orders_OrderID",
                table: "Shippings",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_ProductsProductID",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Discounts_DiscountID",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Products_ProductID",
                table: "ProductImages");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductReviews_Products_ProductID",
                table: "ProductReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSizes_ProductColors_ProductColorID",
                table: "ProductSizes");

            migrationBuilder.DropForeignKey(
                name: "FK_Shippings_Orders_OrderID",
                table: "Shippings");

            migrationBuilder.DropIndex(
                name: "IX_Shippings_OrderID",
                table: "Shippings");

            migrationBuilder.DropIndex(
                name: "IX_ProductSizes_ProductColorID",
                table: "ProductSizes");

            migrationBuilder.DropIndex(
                name: "IX_ProductReviews_ProductID",
                table: "ProductReviews");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_ProductID",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProductsProductID",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ProductsProductID",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "DiscountID",
                table: "Orders",
                newName: "DiscountsDiscountID");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_DiscountID",
                table: "Orders",
                newName: "IX_Orders_DiscountsDiscountID");

            migrationBuilder.AddColumn<Guid>(
                name: "OrdersOrderID",
                table: "Shippings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "ProductColorsProductColorID",
                table: "ProductSizes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductsProductID",
                table: "ProductReviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductsProductID",
                table: "ProductImages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ShippingID",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Shippings_OrdersOrderID",
                table: "Shippings",
                column: "OrdersOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSizes_ProductColorsProductColorID",
                table: "ProductSizes",
                column: "ProductColorsProductColorID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_ProductsProductID",
                table: "ProductReviews",
                column: "ProductsProductID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductsProductID",
                table: "ProductImages",
                column: "ProductsProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShippingID",
                table: "Orders",
                column: "ShippingID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductID",
                table: "OrderDetails",
                column: "ProductID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_ProductID",
                table: "OrderDetails",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Discounts_DiscountsDiscountID",
                table: "Orders",
                column: "DiscountsDiscountID",
                principalTable: "Discounts",
                principalColumn: "DiscountID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Shippings_ShippingID",
                table: "Orders",
                column: "ShippingID",
                principalTable: "Shippings",
                principalColumn: "ShippingID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Products_ProductsProductID",
                table: "ProductImages",
                column: "ProductsProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReviews_Products_ProductsProductID",
                table: "ProductReviews",
                column: "ProductsProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSizes_ProductColors_ProductColorsProductColorID",
                table: "ProductSizes",
                column: "ProductColorsProductColorID",
                principalTable: "ProductColors",
                principalColumn: "ProductColorID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shippings_Orders_OrdersOrderID",
                table: "Shippings",
                column: "OrdersOrderID",
                principalTable: "Orders",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
