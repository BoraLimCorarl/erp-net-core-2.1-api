using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateTableSaleOrderAndSaleOrderItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckStatus",
                table: "CarlErpSaleOrderItems");

            migrationBuilder.DropColumn(
                name: "TotalInvoiceQty",
                table: "CarlErpSaleOrderItems");

            migrationBuilder.DropColumn(
                name: "TotalIssueInvoiceQty",
                table: "CarlErpSaleOrderItems");

            migrationBuilder.DropColumn(
                name: "TotalIssueQty",
                table: "CarlErpSaleOrderItems");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CheckStatus",
                table: "CarlErpSaleOrderItems",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalInvoiceQty",
                table: "CarlErpSaleOrderItems",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalIssueInvoiceQty",
                table: "CarlErpSaleOrderItems",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalIssueQty",
                table: "CarlErpSaleOrderItems",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
