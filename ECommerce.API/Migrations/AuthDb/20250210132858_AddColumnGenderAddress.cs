using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.API.Migrations.AuthDb
{
    /// <inheritdoc />
    public partial class AddColumnGenderAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Gender",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "808e47f5-a733-42ab-8e31-b6af349bfd90",
                columns: new[] { "Address", "ConcurrencyStamp", "Gender", "PasswordHash", "SecurityStamp" },
                values: new object[] { null, "13196130-173b-4033-b106-75a94f5a89fc", null, "AQAAAAIAAYagAAAAEJDGYSth1KaULHLkMGZ9TqVuXLbZYiKkWd7qD/wJYvDuuF09SKh1x4mZJm/xl5TF6A==", "db437515-38da-4737-863a-8245b5bf70c0" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "808e47f5-a733-42ab-8e31-b6af349bfd90",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c341c80f-e733-4fe3-b25a-fbe7bea37290", "AQAAAAIAAYagAAAAEDURJAW+sD4rsgPKb0rd/lLpyyXOEmZSE87F0ulARiujaWs9o/i+Tp6OiKOK1+ILVw==", "2d6d8de3-7730-4c2d-8043-e4cbc49d7cfc" });
        }
    }
}
