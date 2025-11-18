using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class Update_table_ItemIssue_ItemReceipt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductionOrderId",
                table: "CarlErpItemReceipts",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductionOrderId",
                table: "CarlErpItemIssues",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceipts_ProductionOrderId",
                table: "CarlErpItemReceipts",
                column: "ProductionOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssues_ProductionOrderId",
                table: "CarlErpItemIssues",
                column: "ProductionOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemIssues_CarlErpTransProductions_ProductionOrderId",
                table: "CarlErpItemIssues",
                column: "ProductionOrderId",
                principalTable: "CarlErpTransProductions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemReceipts_CarlErpTransProductions_ProductionOrderId",
                table: "CarlErpItemReceipts",
                column: "ProductionOrderId",
                principalTable: "CarlErpTransProductions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemIssues_CarlErpTransProductions_ProductionOrderId",
                table: "CarlErpItemIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemReceipts_CarlErpTransProductions_ProductionOrderId",
                table: "CarlErpItemReceipts");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemReceipts_ProductionOrderId",
                table: "CarlErpItemReceipts");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemIssues_ProductionOrderId",
                table: "CarlErpItemIssues");

            migrationBuilder.DropColumn(
                name: "ProductionOrderId",
                table: "CarlErpItemReceipts");

            migrationBuilder.DropColumn(
                name: "ProductionOrderId",
                table: "CarlErpItemIssues");
        }
    }
}
