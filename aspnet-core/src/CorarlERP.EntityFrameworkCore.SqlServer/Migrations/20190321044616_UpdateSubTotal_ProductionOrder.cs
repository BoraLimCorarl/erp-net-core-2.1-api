using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateSubTotal_ProductionOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalFinishProduction",
                table: "CarlErpTransProductions",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalRawProduction",
                table: "CarlErpTransProductions",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubTotalFinishProduction",
                table: "CarlErpTransProductions");

            migrationBuilder.DropColumn(
                name: "SubTotalRawProduction",
                table: "CarlErpTransProductions");
        }
    }
}
