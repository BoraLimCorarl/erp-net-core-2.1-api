using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdatePOSOAddColReceiveCountIssueCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IssueCount",
                table: "CarlErpSaleOrders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReceiveCount",
                table: "CarlErpPurchaseOrders",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IssueCount",
                table: "CarlErpSaleOrders");

            migrationBuilder.DropColumn(
                name: "ReceiveCount",
                table: "CarlErpPurchaseOrders");
        }
    }
}
