using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerce.API.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "808e47f5-a733-42ab-8e31-b6af349bfd90",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5f59eb51-1e22-4775-96c8-ff7aeb37021c", "AQAAAAIAAYagAAAAEBru+WVcXs+HFaS2Evg5emf382srBXAlYQ3GQuYQcQHzKI/tnlLq5RNcAyWOkqIIag==", "23ba7956-6a82-415e-8de0-877befe0e537" });

            migrationBuilder.InsertData(
                table: "PaymentMethods",
                columns: new[] { "PaymentMethodID", "Description", "MethodName" },
                values: new object[,]
                {
                    { 1, "Ngân hàng, thanh toán trước khi đăt", "VNPAY" },
                    { 2, "Thông thường, thanh toán khi nhận hàng", "COD" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PaymentMethods",
                keyColumn: "PaymentMethodID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PaymentMethods",
                keyColumn: "PaymentMethodID",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "808e47f5-a733-42ab-8e31-b6af349bfd90",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2b6ea6a1-f816-419c-873f-3437cc0f9e35", "AQAAAAIAAYagAAAAEFvF0ZXVxxozTCawkpaDP5m9OKr5tzs0iftUZ61igkDlQIkCw0c7gJWw/+A03S6T7g==", "cf64777e-8191-4190-a7a6-3413daa7b031" });
        }
    }
}
