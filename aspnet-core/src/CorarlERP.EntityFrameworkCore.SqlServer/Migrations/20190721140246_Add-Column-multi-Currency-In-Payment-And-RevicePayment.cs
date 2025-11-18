using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddColumnmultiCurrencyInPaymentAndRevicePayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalOpenBalance",
                table: "CarlErpReceivePayments",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalPayment",
                table: "CarlErpReceivePayments",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalPaymentDue",
                table: "CarlErpReceivePayments",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyOpenBalance",
                table: "CarlErpReceivePaymentDeails",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyPayment",
                table: "CarlErpReceivePaymentDeails",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalAmount",
                table: "CarlErpReceivePaymentDeails",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalOpenBalance",
                table: "CarlErpPayBills",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalPayment",
                table: "CarlErpPayBills",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalPaymentDue",
                table: "CarlErpPayBills",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyOpenBalance",
                table: "CarlErpPayBillDeail",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyPayment",
                table: "CarlErpPayBillDeail",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalAmount",
                table: "CarlErpPayBillDeail",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalOpenBalance",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalPayment",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalPaymentDue",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyOpenBalance",
                table: "CarlErpReceivePaymentDeails");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyPayment",
                table: "CarlErpReceivePaymentDeails");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalAmount",
                table: "CarlErpReceivePaymentDeails");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalOpenBalance",
                table: "CarlErpPayBills");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalPayment",
                table: "CarlErpPayBills");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalPaymentDue",
                table: "CarlErpPayBills");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyOpenBalance",
                table: "CarlErpPayBillDeail");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyPayment",
                table: "CarlErpPayBillDeail");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalAmount",
                table: "CarlErpPayBillDeail");
        }
    }
}
