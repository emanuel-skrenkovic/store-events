using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Store.Catalogue.Infrastructure.Migrations
{
    public partial class subscription_checkpoint_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_subscription_checkpoint_subscription_id",
                schema: "public",
                table: "subscription_checkpoint",
                column: "subscription_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "subscription_checkpoint",
                schema: "public");
        }
    }
}
