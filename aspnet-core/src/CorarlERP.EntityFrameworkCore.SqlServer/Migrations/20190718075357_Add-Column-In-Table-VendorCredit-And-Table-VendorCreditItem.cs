using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddColumnInTableVendorCreditAndTableVendorCreditItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotal",
                table: "CarlErpVendorCreditDetails",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyUnitCost",
                table: "CarlErpVendorCreditDetails",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyOpenBalance",
                table: "CarlErpVendorCredit",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencySubTotal",
                table: "CarlErpVendorCredit",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTax",
                table: "CarlErpVendorCredit",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotal",
                table: "CarlErpVendorCredit",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalPaid",
                table: "CarlErpVendorCredit",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotal",
                table: "CarlErpVendorCreditDetails");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyUnitCost",
                table: "CarlErpVendorCreditDetails");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyOpenBalance",
                table: "CarlErpVendorCredit");

            migrationBuilder.DropColumn(
                name: "MultiCurrencySubTotal",
                table: "CarlErpVendorCredit");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTax",
                table: "CarlErpVendorCredit");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotal",
                table: "CarlErpVendorCredit");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalPaid",
                table: "CarlErpVendorCredit");
        }
    }
}
