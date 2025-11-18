using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class Addlotinproduction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FromLotId",
                table: "CarlErpRawMaterialItems",
                nullable: true,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ToLotId",
                table: "CarlErpFinishItems",
                nullable: true,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpRawMaterialItems_FromLotId",
                table: "CarlErpRawMaterialItems",
                column: "FromLotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpFinishItems_ToLotId",
                table: "CarlErpFinishItems",
                column: "ToLotId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpFinishItems_CarlErpLots_ToLotId",
                table: "CarlErpFinishItems",
                column: "ToLotId",
                principalTable: "CarlErpLots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpRawMaterialItems_CarlErpLots_FromLotId",
                table: "CarlErpRawMaterialItems",
                column: "FromLotId",
                principalTable: "CarlErpLots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpFinishItems_CarlErpLots_ToLotId",
                table: "CarlErpFinishItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpRawMaterialItems_CarlErpLots_FromLotId",
                table: "CarlErpRawMaterialItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpRawMaterialItems_FromLotId",
                table: "CarlErpRawMaterialItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpFinishItems_ToLotId",
                table: "CarlErpFinishItems");

            migrationBuilder.DropColumn(
                name: "FromLotId",
                table: "CarlErpRawMaterialItems");

            migrationBuilder.DropColumn(
                name: "ToLotId",
                table: "CarlErpFinishItems");
        }
    }
}
