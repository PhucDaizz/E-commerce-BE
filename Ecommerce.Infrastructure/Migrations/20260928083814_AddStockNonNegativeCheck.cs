using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStockNonNegativeCheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "808e47f5-a733-42ab-8e31-b6af349bfd90",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1f727562-63b1-4cf6-848a-c3742b0ed30b", "AQAAAAIAAYagAAAAEHRnueD6A3CfVbt4pjspI0bsLNC9JFCYQ8ObleF6c3DaUSh2t2ZdIVgg5/nvI2ewzg==", "2d523c27-a429-46db-96bc-6f1c24647c86" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_ProductSizes_Stock_NonNegative",
                table: "ProductSizes",
                sql: "[Stock] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_ProductSizes_Stock_NonNegative",
                table: "ProductSizes");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "808e47f5-a733-42ab-8e31-b6af349bfd90",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b2761679-37a5-4efb-a22b-f110cefd66e2", "AQAAAAIAAYagAAAAEAXAfSeL5LANfxhinTnwmq9Jqv96A106/SY61O/7Bc26kFf05NYKuOSUdY4AQlyaCA==", "534ea21d-d0c5-446f-b299-5085cdec67ff" });
        }
    }
}
