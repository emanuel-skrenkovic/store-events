using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store.Order.Infrastructure.Migrations
{
    public partial class initial_order_tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "order_display",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    data = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_display", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionCheckpoint",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<string>(type: "text", nullable: true),
                    Position = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionCheckpoint", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_display",
                schema: "public");

            migrationBuilder.DropTable(
                name: "SubscriptionCheckpoint",
                schema: "public");
        }
    }
}
