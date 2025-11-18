using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class SplitCashCreditPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalCashCustomerCredit",
                table: "CarlErpReceivePayments",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCashInvoice",
                table: "CarlErpReceivePayments",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCreditCustomerCredit",
                table: "CarlErpReceivePayments",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCreditInvoice",
                table: "CarlErpReceivePayments",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalExpenseCustomerCredit",
                table: "CarlErpReceivePayments",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalExpenseInvoice",
                table: "CarlErpReceivePayments",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Cash",
                table: "CarlErpReceivePaymentDeails",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Credit",
                table: "CarlErpReceivePaymentDeails",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Expense",
                table: "CarlErpReceivePaymentDeails",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyCash",
                table: "CarlErpReceivePaymentDeails",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyCredit",
                table: "CarlErpReceivePaymentDeails",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyExpense",
                table: "CarlErpReceivePaymentDeails",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "SplitCashCreditPayment",
                table: "AbpTenants",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCashCustomerCredit",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "TotalCashInvoice",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "TotalCreditCustomerCredit",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "TotalCreditInvoice",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "TotalExpenseCustomerCredit",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "TotalExpenseInvoice",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "Cash",
                table: "CarlErpReceivePaymentDeails");

            migrationBuilder.DropColumn(
                name: "Credit",
                table: "CarlErpReceivePaymentDeails");

            migrationBuilder.DropColumn(
                name: "Expense",
                table: "CarlErpReceivePaymentDeails");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyCash",
                table: "CarlErpReceivePaymentDeails");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyCredit",
                table: "CarlErpReceivePaymentDeails");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyExpense",
                table: "CarlErpReceivePaymentDeails");

            migrationBuilder.DropColumn(
                name: "SplitCashCreditPayment",
                table: "AbpTenants");
        }
    }
}
