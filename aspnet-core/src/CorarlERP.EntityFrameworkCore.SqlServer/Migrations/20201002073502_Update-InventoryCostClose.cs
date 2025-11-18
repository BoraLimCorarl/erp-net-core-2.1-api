using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateInventoryCostClose : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalCost",
                table: "CarlErpInventoryCostCloses",
                type: "decimal(19,6)",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalCost",
                table: "CarlErpInventoryCostCloses",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,6)");
        }
    }
}
