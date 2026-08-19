using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sportify.Migrations
{
    /// <inheritdoc />
    public partial class AddVariantImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "ProductVariants",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "ProductVariantId",
                keyValue: 1,
                column: "ImageURL",
                value: null);

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "ProductVariantId",
                keyValue: 2,
                column: "ImageURL",
                value: null);

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "ProductVariantId",
                keyValue: 3,
                column: "ImageURL",
                value: null);

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "ProductVariantId",
                keyValue: 4,
                column: "ImageURL",
                value: null);

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "ProductVariantId",
                keyValue: 5,
                column: "ImageURL",
                value: null);

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "ProductVariantId",
                keyValue: 6,
                column: "ImageURL",
                value: null);

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "ProductVariantId",
                keyValue: 7,
                column: "ImageURL",
                value: null);

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "ProductVariantId",
                keyValue: 8,
                column: "ImageURL",
                value: null);

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "ProductVariantId",
                keyValue: 9,
                column: "ImageURL",
                value: null);

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "ProductVariantId",
                keyValue: 10,
                column: "ImageURL",
                value: null);

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "ProductVariantId",
                keyValue: 11,
                column: "ImageURL",
                value: null);

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "ProductVariantId",
                keyValue: 12,
                column: "ImageURL",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "ProductVariants");
        }
    }
}
