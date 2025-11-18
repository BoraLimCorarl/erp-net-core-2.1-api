using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateTableItemIssueVendorCredit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemIssueVendorCredit_CarlErpItemReceipts_ItemReceiptPuchaseId",
                table: "CarlErpItemIssueVendorCredit");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemIssueVendorCredit_ItemReceiptPuchaseId",
                table: "CarlErpItemIssueVendorCredit");

            migrationBuilder.DropColumn(
                name: "ItemReceiptPuchaseId",
                table: "CarlErpItemIssueVendorCredit");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCredit_ItemReceiptPurchaseId",
                table: "CarlErpItemIssueVendorCredit",
                column: "ItemReceiptPurchaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemIssueVendorCredit_CarlErpItemReceipts_ItemReceiptPurchaseId",
                table: "CarlErpItemIssueVendorCredit",
                column: "ItemReceiptPurchaseId",
                principalTable: "CarlErpItemReceipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemIssueVendorCredit_CarlErpItemReceipts_ItemReceiptPurchaseId",
                table: "CarlErpItemIssueVendorCredit");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemIssueVendorCredit_ItemReceiptPurchaseId",
                table: "CarlErpItemIssueVendorCredit");

            migrationBuilder.AddColumn<Guid>(
                name: "ItemReceiptPuchaseId",
                table: "CarlErpItemIssueVendorCredit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCredit_ItemReceiptPuchaseId",
                table: "CarlErpItemIssueVendorCredit",
                column: "ItemReceiptPuchaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemIssueVendorCredit_CarlErpItemReceipts_ItemReceiptPuchaseId",
                table: "CarlErpItemIssueVendorCredit",
                column: "ItemReceiptPuchaseId",
                principalTable: "CarlErpItemReceipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
