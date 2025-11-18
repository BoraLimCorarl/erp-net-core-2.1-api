using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddFieldTotalOfVendorCredit_In_PayBill : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalPaymentVendorCredit",
                table: "CarlErpPayBills",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalOpenBalanceVendorCredit",
                table: "CarlErpPayBills",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPaymentDueVendorCredit",
                table: "CarlErpPayBills",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPaymentVendorCredit",
                table: "CarlErpPayBills",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalPaymentVendorCredit",
                table: "CarlErpPayBills");

            migrationBuilder.DropColumn(
                name: "TotalOpenBalanceVendorCredit",
                table: "CarlErpPayBills");

            migrationBuilder.DropColumn(
                name: "TotalPaymentDueVendorCredit",
                table: "CarlErpPayBills");

            migrationBuilder.DropColumn(
                name: "TotalPaymentVendorCredit",
                table: "CarlErpPayBills");
        }
    }
}
