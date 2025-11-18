using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateUnitAddBaseUnitFactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GrossWeight",
                table: "CarlErpPropertyValues",
                newName: "Factor");

            migrationBuilder.AddColumn<long>(
                name: "BaseUnitId",
                table: "CarlErpPropertyValues",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBaseUnit",
                table: "CarlErpPropertyValues",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPropertyValues_BaseUnitId",
                table: "CarlErpPropertyValues",
                column: "BaseUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpPropertyValues_CarlErpPropertyValues_BaseUnitId",
                table: "CarlErpPropertyValues",
                column: "BaseUnitId",
                principalTable: "CarlErpPropertyValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpPropertyValues_CarlErpPropertyValues_BaseUnitId",
                table: "CarlErpPropertyValues");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpPropertyValues_BaseUnitId",
                table: "CarlErpPropertyValues");

            migrationBuilder.DropColumn(
                name: "BaseUnitId",
                table: "CarlErpPropertyValues");

            migrationBuilder.DropColumn(
                name: "IsBaseUnit",
                table: "CarlErpPropertyValues");

            migrationBuilder.RenameColumn(
                name: "Factor",
                table: "CarlErpPropertyValues",
                newName: "GrossWeight");
        }
    }
}
