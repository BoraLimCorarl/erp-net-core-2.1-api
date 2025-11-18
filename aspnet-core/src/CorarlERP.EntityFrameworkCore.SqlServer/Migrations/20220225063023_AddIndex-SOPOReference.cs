using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddIndexSOPOReference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrders_TenantId_OrderNumber",
                table: "CarlErpSaleOrders",
                columns: new[] { "TenantId", "OrderNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrders_TenantId_Reference",
                table: "CarlErpSaleOrders",
                columns: new[] { "TenantId", "Reference" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrders_TenantId_OrderNumber",
                table: "CarlErpPurchaseOrders",
                columns: new[] { "TenantId", "OrderNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrders_TenantId_Reference",
                table: "CarlErpPurchaseOrders",
                columns: new[] { "TenantId", "Reference" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CarlErpSaleOrders_TenantId_OrderNumber",
                table: "CarlErpSaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpSaleOrders_TenantId_Reference",
                table: "CarlErpSaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpPurchaseOrders_TenantId_OrderNumber",
                table: "CarlErpPurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpPurchaseOrders_TenantId_Reference",
                table: "CarlErpPurchaseOrders");
        }
    }
}
