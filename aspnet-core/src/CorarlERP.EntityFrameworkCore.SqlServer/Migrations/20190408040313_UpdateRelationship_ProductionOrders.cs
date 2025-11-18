using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateRelationship_ProductionOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransProductions_ProductionAccountId",
                table: "CarlErpTransProductions",
                column: "ProductionAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpTransProductions_CarlErpChartOfAccounts_ProductionAccountId",
                table: "CarlErpTransProductions",
                column: "ProductionAccountId",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpTransProductions_CarlErpChartOfAccounts_ProductionAccountId",
                table: "CarlErpTransProductions");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpTransProductions_ProductionAccountId",
                table: "CarlErpTransProductions");
        }
    }
}
