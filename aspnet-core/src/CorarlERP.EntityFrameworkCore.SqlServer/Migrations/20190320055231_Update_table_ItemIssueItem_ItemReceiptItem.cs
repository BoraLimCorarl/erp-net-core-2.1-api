using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class Update_table_ItemIssueItem_ItemReceiptItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FinishItemId",
                table: "CarlErpItemReceiptItems",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RawMaterialItemId",
                table: "CarlErpItemIssueItems",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptItems_FinishItemId",
                table: "CarlErpItemReceiptItems",
                column: "FinishItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueItems_RawMaterialItemId",
                table: "CarlErpItemIssueItems",
                column: "RawMaterialItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemIssueItems_CarlErpRawMaterialItems_RawMaterialItemId",
                table: "CarlErpItemIssueItems",
                column: "RawMaterialItemId",
                principalTable: "CarlErpRawMaterialItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemReceiptItems_CarlErpFinishItems_FinishItemId",
                table: "CarlErpItemReceiptItems",
                column: "FinishItemId",
                principalTable: "CarlErpFinishItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemIssueItems_CarlErpRawMaterialItems_RawMaterialItemId",
                table: "CarlErpItemIssueItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemReceiptItems_CarlErpFinishItems_FinishItemId",
                table: "CarlErpItemReceiptItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemReceiptItems_FinishItemId",
                table: "CarlErpItemReceiptItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemIssueItems_RawMaterialItemId",
                table: "CarlErpItemIssueItems");

            migrationBuilder.DropColumn(
                name: "FinishItemId",
                table: "CarlErpItemReceiptItems");

            migrationBuilder.DropColumn(
                name: "RawMaterialItemId",
                table: "CarlErpItemIssueItems");
        }
    }
}
