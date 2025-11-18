using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddColumnInCustomerCreditMultiCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyOpenBalance",
                table: "CarlErpCustomerCredits",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencySubTotal",
                table: "CarlErpCustomerCredits",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTax",
                table: "CarlErpCustomerCredits",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotal",
                table: "CarlErpCustomerCredits",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalPaid",
                table: "CarlErpCustomerCredits",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotal",
                table: "CarlErpCusotmerCreditDetails",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyUnitCost",
                table: "CarlErpCusotmerCreditDetails",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MultiCurrencyOpenBalance",
                table: "CarlErpCustomerCredits");

            migrationBuilder.DropColumn(
                name: "MultiCurrencySubTotal",
                table: "CarlErpCustomerCredits");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTax",
                table: "CarlErpCustomerCredits");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotal",
                table: "CarlErpCustomerCredits");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalPaid",
                table: "CarlErpCustomerCredits");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotal",
                table: "CarlErpCusotmerCreditDetails");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyUnitCost",
                table: "CarlErpCusotmerCreditDetails");
        }
    }
}
