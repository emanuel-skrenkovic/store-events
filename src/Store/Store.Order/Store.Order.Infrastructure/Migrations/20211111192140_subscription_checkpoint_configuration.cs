using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store.Order.Infrastructure.Migrations
{
    public partial class subscription_checkpoint_configuration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SubscriptionCheckpoint",
                schema: "public",
                table: "SubscriptionCheckpoint");

            migrationBuilder.RenameTable(
                name: "SubscriptionCheckpoint",
                schema: "public",
                newName: "subscription_checkpoint",
                newSchema: "public");

            migrationBuilder.RenameColumn(
                name: "Position",
                schema: "public",
                table: "subscription_checkpoint",
                newName: "position");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "public",
                table: "subscription_checkpoint",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "SubscriptionId",
                schema: "public",
                table: "subscription_checkpoint",
                newName: "subscription_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_subscription_checkpoint",
                schema: "public",
                table: "subscription_checkpoint",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_checkpoint_subscription_id",
                schema: "public",
                table: "subscription_checkpoint",
                column: "subscription_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_subscription_checkpoint",
                schema: "public",
                table: "subscription_checkpoint");

            migrationBuilder.DropIndex(
                name: "IX_subscription_checkpoint_subscription_id",
                schema: "public",
                table: "subscription_checkpoint");

            migrationBuilder.RenameTable(
                name: "subscription_checkpoint",
                schema: "public",
                newName: "SubscriptionCheckpoint",
                newSchema: "public");

            migrationBuilder.RenameColumn(
                name: "position",
                schema: "public",
                table: "SubscriptionCheckpoint",
                newName: "Position");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "public",
                table: "SubscriptionCheckpoint",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "subscription_id",
                schema: "public",
                table: "SubscriptionCheckpoint",
                newName: "SubscriptionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubscriptionCheckpoint",
                schema: "public",
                table: "SubscriptionCheckpoint",
                column: "Id");
        }
    }
}
