using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddmultiCurrencyTaxInPOSO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTax",
                table: "CarlErpSaleOrders",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTax",
                table: "CarlErpPurchaseOrders",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MultiCurrencyTax",
                table: "CarlErpSaleOrders");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTax",
                table: "CarlErpPurchaseOrders");
        }
    }
}
