using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateSaleOrderAddSaleType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SaleTransactionTypeId",
                table: "CarlErpSaleOrders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrders_SaleTransactionTypeId",
                table: "CarlErpSaleOrders",
                column: "SaleTransactionTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpSaleOrders_CarlErpTransactionTypes_SaleTransactionTypeId",
                table: "CarlErpSaleOrders",
                column: "SaleTransactionTypeId",
                principalTable: "CarlErpTransactionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpSaleOrders_CarlErpTransactionTypes_SaleTransactionTypeId",
                table: "CarlErpSaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpSaleOrders_SaleTransactionTypeId",
                table: "CarlErpSaleOrders");

            migrationBuilder.DropColumn(
                name: "SaleTransactionTypeId",
                table: "CarlErpSaleOrders");
        }
    }
}
