using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class Updatetableproperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "GrossWeight",
                table: "CarlErpPropertyValues",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NetWeight",
                table: "CarlErpPropertyValues",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnit",
                table: "CarlErpProperties",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrossWeight",
                table: "CarlErpPropertyValues");

            migrationBuilder.DropColumn(
                name: "NetWeight",
                table: "CarlErpPropertyValues");

            migrationBuilder.DropColumn(
                name: "IsUnit",
                table: "CarlErpProperties");
        }
    }
}
