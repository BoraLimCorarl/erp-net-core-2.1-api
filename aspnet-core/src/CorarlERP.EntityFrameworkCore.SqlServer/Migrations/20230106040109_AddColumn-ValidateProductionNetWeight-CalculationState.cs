using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddColumnValidateProductionNetWeightCalculationState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CalculationState",
                table: "CarlErpTransProductions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ValidateProductionNetWeight",
                table: "AbpTenants",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalculationState",
                table: "CarlErpTransProductions");

            migrationBuilder.DropColumn(
                name: "ValidateProductionNetWeight",
                table: "AbpTenants");
        }
    }
}
