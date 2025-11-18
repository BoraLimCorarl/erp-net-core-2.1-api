using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddMultiCurrencyInTablePOSO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MultiCurrencyId",
                table: "CarlErpSaleOrders",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencySubTotal",
                table: "CarlErpSaleOrders",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotal",
                table: "CarlErpSaleOrders",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotal",
                table: "CarlErpSaleOrderItems",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyUnitCost",
                table: "CarlErpSaleOrderItems",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "MultiCurrencyId",
                table: "CarlErpPurchaseOrders",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencySubTotal",
                table: "CarlErpPurchaseOrders",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotal",
                table: "CarlErpPurchaseOrders",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotal",
                table: "CarlErpPurchaseOrderItems",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyUnitCust",
                table: "CarlErpPurchaseOrderItems",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrders_MultiCurrencyId",
                table: "CarlErpSaleOrders",
                column: "MultiCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrders_MultiCurrencyId",
                table: "CarlErpPurchaseOrders",
                column: "MultiCurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpPurchaseOrders_CarlErpCurrencies_MultiCurrencyId",
                table: "CarlErpPurchaseOrders",
                column: "MultiCurrencyId",
                principalTable: "CarlErpCurrencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpSaleOrders_CarlErpCurrencies_MultiCurrencyId",
                table: "CarlErpSaleOrders",
                column: "MultiCurrencyId",
                principalTable: "CarlErpCurrencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpPurchaseOrders_CarlErpCurrencies_MultiCurrencyId",
                table: "CarlErpPurchaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpSaleOrders_CarlErpCurrencies_MultiCurrencyId",
                table: "CarlErpSaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpSaleOrders_MultiCurrencyId",
                table: "CarlErpSaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpPurchaseOrders_MultiCurrencyId",
                table: "CarlErpPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyId",
                table: "CarlErpSaleOrders");

            migrationBuilder.DropColumn(
                name: "MultiCurrencySubTotal",
                table: "CarlErpSaleOrders");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotal",
                table: "CarlErpSaleOrders");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotal",
                table: "CarlErpSaleOrderItems");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyUnitCost",
                table: "CarlErpSaleOrderItems");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyId",
                table: "CarlErpPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "MultiCurrencySubTotal",
                table: "CarlErpPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotal",
                table: "CarlErpPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotal",
                table: "CarlErpPurchaseOrderItems");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyUnitCust",
                table: "CarlErpPurchaseOrderItems");
        }
    }
}
