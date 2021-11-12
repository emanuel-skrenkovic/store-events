using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store.Order.Infrastructure.Migrations
{
    public partial class cart_customer_number_column_name : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomerNumber",
                schema: "public",
                table: "cart",
                newName: "customer_number");

            migrationBuilder.CreateTable(
                name: "CartEntryEntity",
                schema: "public",
                columns: table => new
                {
                    CatalogueNumber = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartEntryEntity",
                schema: "public");

            migrationBuilder.RenameColumn(
                name: "customer_number",
                schema: "public",
                table: "cart",
                newName: "CustomerNumber");
        }
    }
}
