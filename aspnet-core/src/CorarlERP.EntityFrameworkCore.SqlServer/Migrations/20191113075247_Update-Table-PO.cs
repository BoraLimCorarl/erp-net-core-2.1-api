using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateTablePO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MultiCurrencyUnitCust",
                table: "CarlErpPurchaseOrderItems",
                newName: "MultiCurrencyUnitCost");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MultiCurrencyUnitCost",
                table: "CarlErpPurchaseOrderItems",
                newName: "MultiCurrencyUnitCust");
        }
    }
}
