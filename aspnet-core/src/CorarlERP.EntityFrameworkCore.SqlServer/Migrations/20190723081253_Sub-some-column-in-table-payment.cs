using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class Subsomecolumnintablepayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalOpenBalance",
                table: "CarlErpPayBills");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalPaymentDue",
                table: "CarlErpPayBills");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalOpenBalance",
                table: "CarlErpPayBills",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalPaymentDue",
                table: "CarlErpPayBills",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
