using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTableTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TagID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TagName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.TagID);
                });

            migrationBuilder.CreateTable(
                name: "ProductTags",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    TagID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTags", x => new { x.ProductID, x.TagID });
                    table.ForeignKey(
                        name: "FK_ProductTags_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductTags_Tags_TagID",
                        column: x => x.TagID,
                        principalTable: "Tags",
                        principalColumn: "TagID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "808e47f5-a733-42ab-8e31-b6af349bfd90",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f457c1db-cfc7-4e7b-9c0a-b388a465ea18", "AQAAAAIAAYagAAAAEAWSkd8dEL6t1epL9s6Eo+QIs2cV/3ZGEP8qu6NruX73lQuy9jczLKJ9aJJ+HZLZUw==", "900bbf8b-6bf8-4f34-bba5-dffe06db66f5" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductTags_TagID",
                table: "ProductTags",
                column: "TagID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductTags");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "808e47f5-a733-42ab-8e31-b6af349bfd90",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d61d4d13-16c7-436e-a81d-2436c5984027", "AQAAAAIAAYagAAAAEG3zoUeGzXJ3HLpnCBcwgcnC2sjC4JLXwN36v2q+g8SLxk4R9/X5ID4Gec2vX7jKjQ==", "861b4da2-9a74-4742-a6ee-fe6348ac66fa" });
        }
    }
}
