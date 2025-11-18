using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddLotIdInTableInventoryClose : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LotId",
                table: "CarlErpInventoryCostCloses",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCostCloses_LotId",
                table: "CarlErpInventoryCostCloses",
                column: "LotId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpInventoryCostCloses_CarlErpLots_LotId",
                table: "CarlErpInventoryCostCloses",
                column: "LotId",
                principalTable: "CarlErpLots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpInventoryCostCloses_CarlErpLots_LotId",
                table: "CarlErpInventoryCostCloses");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpInventoryCostCloses_LotId",
                table: "CarlErpInventoryCostCloses");

            migrationBuilder.DropColumn(
                name: "LotId",
                table: "CarlErpInventoryCostCloses");
        }
    }
}
