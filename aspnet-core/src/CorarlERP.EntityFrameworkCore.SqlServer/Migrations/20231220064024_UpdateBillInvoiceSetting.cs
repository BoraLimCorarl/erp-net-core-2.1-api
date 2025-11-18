using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateBillInvoiceSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TurnOffStockValidationForImportExcel",
                table: "CarlErpBillInvoiceSettings",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TurnOffStockValidationForImportExcel",
                table: "CarlErpBillInvoiceSettings");
        }
    }
}
