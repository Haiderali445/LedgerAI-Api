using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LedgerAI.Api.src.Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class SyncValueConvertersAndModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_ledger_invoice",
                table: "ledger_entries");

            migrationBuilder.DropIndex(
                name: "idx_ledger_order",
                table: "ledger_entries");

            migrationBuilder.RenameIndex(
                name: "idx_invoices_order",
                table: "invoices",
                newName: "ix_invoices_order_id");

            migrationBuilder.AlterColumn<string>(
                name: "movement_type",
                table: "stock_movements",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "processing_status",
                table: "inbound_messages",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "idx_ledger_invoice",
                table: "ledger_entries",
                columns: new[] { "tenant_id", "invoice_id" });

            migrationBuilder.CreateIndex(
                name: "idx_ledger_order",
                table: "ledger_entries",
                columns: new[] { "tenant_id", "order_id" });

            migrationBuilder.CreateIndex(
                name: "idx_invoices_order",
                table: "invoices",
                columns: new[] { "tenant_id", "order_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_ledger_invoice",
                table: "ledger_entries");

            migrationBuilder.DropIndex(
                name: "idx_ledger_order",
                table: "ledger_entries");

            migrationBuilder.DropIndex(
                name: "idx_invoices_order",
                table: "invoices");

            migrationBuilder.RenameIndex(
                name: "ix_invoices_order_id",
                table: "invoices",
                newName: "idx_invoices_order");

            migrationBuilder.AlterColumn<string>(
                name: "movement_type",
                table: "stock_movements",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "processing_status",
                table: "inbound_messages",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.CreateIndex(
                name: "idx_ledger_invoice",
                table: "ledger_entries",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "idx_ledger_order",
                table: "ledger_entries",
                column: "order_id");
        }
    }
}
