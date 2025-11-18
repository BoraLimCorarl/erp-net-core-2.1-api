using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddColumnInTableInvoiceMultiCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyOpenBalance",
                table: "CarlErpInvoices",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencySubTotal",
                table: "CarlErpInvoices",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTax",
                table: "CarlErpInvoices",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotal",
                table: "CarlErpInvoices",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalPaid",
                table: "CarlErpInvoices",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotal",
                table: "CarlErpInvoiceItems",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyUnitCost",
                table: "CarlErpInvoiceItems",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MultiCurrencyOpenBalance",
                table: "CarlErpInvoices");

            migrationBuilder.DropColumn(
                name: "MultiCurrencySubTotal",
                table: "CarlErpInvoices");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTax",
                table: "CarlErpInvoices");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotal",
                table: "CarlErpInvoices");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalPaid",
                table: "CarlErpInvoices");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotal",
                table: "CarlErpInvoiceItems");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyUnitCost",
                table: "CarlErpInvoiceItems");
        }
    }
}
