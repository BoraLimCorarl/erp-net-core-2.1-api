using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateProductionProcess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRequiredProductionPlan",
                table: "CarlErpProductionProcess",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ProductionProcessType",
                table: "CarlErpProductionProcess",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionProcess_ProductionProcessType",
                table: "CarlErpProductionProcess",
                column: "ProductionProcessType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CarlErpProductionProcess_ProductionProcessType",
                table: "CarlErpProductionProcess");

            migrationBuilder.DropColumn(
                name: "IsRequiredProductionPlan",
                table: "CarlErpProductionProcess");

            migrationBuilder.DropColumn(
                name: "ProductionProcessType",
                table: "CarlErpProductionProcess");
        }
    }
}
