using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImageForCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "808e47f5-a733-42ab-8e31-b6af349bfd90",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "091ce850-0d7f-464b-aeb0-1de1aa742e2f", "AQAAAAIAAYagAAAAEOJgyhWyLcE1pcJRBSdgjPYZkUkPEAW8cMoDl05NbccoiT3MonO4oq8zwfD2RfT9uA==", "82f4c2af-8962-44fd-9dc2-28cf651f1ade" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "Categories");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "808e47f5-a733-42ab-8e31-b6af349bfd90",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ba1647a2-895c-4fd3-93fe-cb0e9b5581c4", "AQAAAAIAAYagAAAAEPSKMJhRQ3lF4ta49RV3mToq/qthkk0eRhLM0x1paPtpasjKSwmpWHi+q9alt4ICVw==", "ba922f17-287c-4266-b845-720e5185b32e" });
        }
    }
}
