using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddColLatestCostForInventoryTransactionItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LatestCost",
                table: "CarlErpInventoryTransactionItems",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LatestCost",
                table: "CarlErpInventoryTransactionItems");
        }
    }
}
