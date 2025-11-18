using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class SubSomeColumnInTableReceivePayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalOpenBalance",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalPaymentDue",
                table: "CarlErpReceivePayments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalOpenBalance",
                table: "CarlErpReceivePayments",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalPaymentDue",
                table: "CarlErpReceivePayments",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
