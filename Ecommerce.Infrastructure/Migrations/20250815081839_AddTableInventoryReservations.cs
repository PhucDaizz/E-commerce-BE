using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTableInventoryReservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryReservations",
                columns: table => new
                {
                    ReservationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductSizeID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReservedQuantity = table.Column<int>(type: "int", nullable: false),
                    ReservationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsExpired = table.Column<bool>(type: "bit", nullable: false),
                    TransactionID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryReservations", x => x.ReservationID);
                    table.ForeignKey(
                        name: "FK_InventoryReservations_ProductSizes_ProductSizeID",
                        column: x => x.ProductSizeID,
                        principalTable: "ProductSizes",
                        principalColumn: "ProductSizeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "808e47f5-a733-42ab-8e31-b6af349bfd90",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ba1647a2-895c-4fd3-93fe-cb0e9b5581c4", "AQAAAAIAAYagAAAAEPSKMJhRQ3lF4ta49RV3mToq/qthkk0eRhLM0x1paPtpasjKSwmpWHi+q9alt4ICVw==", "ba922f17-287c-4266-b845-720e5185b32e" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservations_ProductSizeID",
                table: "InventoryReservations",
                column: "ProductSizeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryReservations");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "808e47f5-a733-42ab-8e31-b6af349bfd90",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cf0c2932-e057-44f2-bfb5-1c9766de30b0", "AQAAAAIAAYagAAAAEADhZyEAh/nHMvEdJ53zUdJb59dokczpBrnjH5RWGvXJv+JHrslGnaU5DhZYQQAOzg==", "8bd5aa50-7bf6-4c4a-a673-f959f8c3b30b" });
        }
    }
}
