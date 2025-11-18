using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddSalePricePurchaseCostInItemReceiptCustomerCreditItemIssueVendorCredit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PurchaseCost",
                table: "CarlErpVendorCreditDetails",
                type: "decimal(19,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SalePrice",
                table: "CarlErpCusotmerCreditDetails",
                type: "decimal(19,6)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchaseCost",
                table: "CarlErpVendorCreditDetails");

            migrationBuilder.DropColumn(
                name: "SalePrice",
                table: "CarlErpCusotmerCreditDetails");
        }
    }
}
