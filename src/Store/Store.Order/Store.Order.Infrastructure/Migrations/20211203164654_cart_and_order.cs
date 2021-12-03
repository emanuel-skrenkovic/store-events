using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Store.Order.Infrastructure.Migrations
{
    public partial class cart_and_order : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "order",
                schema: "public",
                columns: table => new
                {
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    customer_number = table.Column<string>(type: "text", nullable: true),
                    total_amount = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order", x => x.order_id);
                });

            migrationBuilder.CreateTable(
                name: "product",
                schema: "public",
                columns: table => new
                {
                    catalogue_number = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    available = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product", x => x.catalogue_number);
                });

            migrationBuilder.CreateTable(
                name: "subscription_checkpoint",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    subscription_id = table.Column<string>(type: "text", nullable: true),
                    position = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_checkpoint", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "shipping_information",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    country_code = table.Column<int>(type: "integer", nullable: false),
                    full_name = table.Column<string>(type: "text", nullable: true),
                    street_address = table.Column<string>(type: "text", nullable: true),
                    city = table.Column<string>(type: "text", nullable: true),
                    state_province = table.Column<string>(type: "text", nullable: true),
                    postcode = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shipping_information", x => x.id);
                    table.ForeignKey(
                        name: "FK_shipping_information_order_order_id",
                        column: x => x.order_id,
                        principalSchema: "public",
                        principalTable: "order",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cart_entry",
                schema: "public",
                columns: table => new
                {
                    customer_number = table.Column<string>(type: "text", nullable: false),
                    session_id = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    quantity = table.Column<long>(type: "bigint", nullable: false),
                    product_catalogue_number = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cart_entry", x => new { x.customer_number, x.session_id });
                    table.ForeignKey(
                        name: "FK_cart_entry_product_product_catalogue_number",
                        column: x => x.product_catalogue_number,
                        principalSchema: "public",
                        principalTable: "product",
                        principalColumn: "catalogue_number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_line",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    product_catalogue_number = table.Column<string>(type: "text", nullable: false),
                    count = table.Column<long>(type: "bigint", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_line", x => x.id);
                    table.ForeignKey(
                        name: "FK_order_line_order_order_id",
                        column: x => x.order_id,
                        principalSchema: "public",
                        principalTable: "order",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_line_product_product_catalogue_number",
                        column: x => x.product_catalogue_number,
                        principalSchema: "public",
                        principalTable: "product",
                        principalColumn: "catalogue_number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cart_entry_product_catalogue_number",
                schema: "public",
                table: "cart_entry",
                column: "product_catalogue_number");

            migrationBuilder.CreateIndex(
                name: "IX_order_line_order_id",
                schema: "public",
                table: "order_line",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_line_product_catalogue_number",
                schema: "public",
                table: "order_line",
                column: "product_catalogue_number");

            migrationBuilder.CreateIndex(
                name: "IX_shipping_information_order_id",
                schema: "public",
                table: "shipping_information",
                column: "order_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_subscription_checkpoint_subscription_id",
                schema: "public",
                table: "subscription_checkpoint",
                column: "subscription_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cart_entry",
                schema: "public");

            migrationBuilder.DropTable(
                name: "order_line",
                schema: "public");

            migrationBuilder.DropTable(
                name: "shipping_information",
                schema: "public");

            migrationBuilder.DropTable(
                name: "subscription_checkpoint",
                schema: "public");

            migrationBuilder.DropTable(
                name: "product",
                schema: "public");

            migrationBuilder.DropTable(
                name: "order",
                schema: "public");
        }
    }
}
