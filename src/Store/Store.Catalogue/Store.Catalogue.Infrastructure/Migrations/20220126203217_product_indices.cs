using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store.Catalogue.Infrastructure.Migrations
{
    public partial class product_indices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_product_catalogue_id",
                schema: "public",
                table: "product",
                column: "catalogue_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_id",
                schema: "public",
                table: "product",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_product_catalogue_id",
                schema: "public",
                table: "product");

            migrationBuilder.DropIndex(
                name: "IX_product_id",
                schema: "public",
                table: "product");
        }
    }
}
