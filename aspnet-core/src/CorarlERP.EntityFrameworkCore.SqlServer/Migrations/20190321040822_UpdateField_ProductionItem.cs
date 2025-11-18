using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateField_ProductionItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "CarlErpRawMaterialItems",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitCost",
                table: "CarlErpRawMaterialItems",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "CarlErpFinishItems",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitCost",
                table: "CarlErpFinishItems",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Total",
                table: "CarlErpRawMaterialItems");

            migrationBuilder.DropColumn(
                name: "UnitCost",
                table: "CarlErpRawMaterialItems");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "CarlErpFinishItems");

            migrationBuilder.DropColumn(
                name: "UnitCost",
                table: "CarlErpFinishItems");
        }
    }
}
