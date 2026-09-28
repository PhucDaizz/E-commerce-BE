using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionRefToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransactionRef",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "808e47f5-a733-42ab-8e31-b6af349bfd90",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b2761679-37a5-4efb-a22b-f110cefd66e2", "AQAAAAIAAYagAAAAEAXAfSeL5LANfxhinTnwmq9Jqv96A106/SY61O/7Bc26kFf05NYKuOSUdY4AQlyaCA==", "534ea21d-d0c5-446f-b299-5085cdec67ff" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TransactionRef",
                table: "Orders",
                column: "TransactionRef");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_TransactionRef",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TransactionRef",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "808e47f5-a733-42ab-8e31-b6af349bfd90",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "091ce850-0d7f-464b-aeb0-1de1aa742e2f", "AQAAAAIAAYagAAAAEOJgyhWyLcE1pcJRBSdgjPYZkUkPEAW8cMoDl05NbccoiT3MonO4oq8zwfD2RfT9uA==", "82f4c2af-8962-44fd-9dc2-28cf651f1ade" });
        }
    }
}
