using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store.Order.Infrastructure.Migrations
{
    public partial class cart_customer_number : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerNumber",
                schema: "public",
                table: "cart",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerNumber",
                schema: "public",
                table: "cart");
        }
    }
}
