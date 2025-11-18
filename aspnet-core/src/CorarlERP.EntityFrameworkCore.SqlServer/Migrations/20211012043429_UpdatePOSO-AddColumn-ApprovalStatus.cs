using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdatePOSOAddColumnApprovalStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "CarlErpSaleOrders",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "CarlErpPurchaseOrders",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "CarlErpSaleOrders");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "CarlErpPurchaseOrders");
        }
    }
}
