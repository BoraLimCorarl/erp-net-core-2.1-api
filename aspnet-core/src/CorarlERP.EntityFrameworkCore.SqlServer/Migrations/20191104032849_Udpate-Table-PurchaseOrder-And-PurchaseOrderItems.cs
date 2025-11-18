using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UdpateTablePurchaseOrderAndPurchaseOrderItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckStatus",
                table: "CarlErpPurchaseOrderItems");

            migrationBuilder.DropColumn(
                name: "TotalBillQty",
                table: "CarlErpPurchaseOrderItems");

            migrationBuilder.DropColumn(
                name: "TotalReceiptBillQty",
                table: "CarlErpPurchaseOrderItems");

            migrationBuilder.DropColumn(
                name: "TotalReceiptQty",
                table: "CarlErpPurchaseOrderItems");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CheckStatus",
                table: "CarlErpPurchaseOrderItems",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalBillQty",
                table: "CarlErpPurchaseOrderItems",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalReceiptBillQty",
                table: "CarlErpPurchaseOrderItems",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalReceiptQty",
                table: "CarlErpPurchaseOrderItems",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
