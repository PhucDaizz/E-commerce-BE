using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.API.Migrations
{
    /// <inheritdoc />
    public partial class FixconflictV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_OrderID",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_ProductsProductID",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethods_Orders_OrdersOrderID",
                table: "PaymentMethods");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_CartItems_CartItemsCartItemID",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Shippings_Orders_OrderID",
                table: "Shippings");

            migrationBuilder.DropIndex(
                name: "IX_Products_CartItemsCartItemID",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_PaymentMethods_OrdersOrderID",
                table: "PaymentMethods");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProductsProductID",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "CartItemsCartItemID",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OrdersOrderID",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "ProductsProductID",
                table: "OrderDetails");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductID",
                table: "OrderDetails",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductID",
                table: "CartItems",
                column: "ProductID");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Products_ProductID",
                table: "CartItems",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_OrderID",
                table: "OrderDetails",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "OrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_ProductID",
                table: "OrderDetails",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shippings_Orders_OrderID",
                table: "Shippings",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "OrderID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Products_ProductID",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_OrderID",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_ProductID",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Shippings_Orders_OrderID",
                table: "Shippings");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProductID",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_ProductID",
                table: "CartItems");

            migrationBuilder.AddColumn<int>(
                name: "CartItemsCartItemID",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrdersOrderID",
                table: "PaymentMethods",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "ProductsProductID",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CartItemsCartItemID",
                table: "Products",
                column: "CartItemsCartItemID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_OrdersOrderID",
                table: "PaymentMethods",
                column: "OrdersOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductsProductID",
                table: "OrderDetails",
                column: "ProductsProductID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_OrderID",
                table: "OrderDetails",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_ProductsProductID",
                table: "OrderDetails",
                column: "ProductsProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethods_Orders_OrdersOrderID",
                table: "PaymentMethods",
                column: "OrdersOrderID",
                principalTable: "Orders",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CartItems_CartItemsCartItemID",
                table: "Products",
                column: "CartItemsCartItemID",
                principalTable: "CartItems",
                principalColumn: "CartItemID");

            migrationBuilder.AddForeignKey(
                name: "FK_Shippings_Orders_OrderID",
                table: "Shippings",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
