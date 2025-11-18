using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateTableVendorCreditItemIssueVendorCredit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ItemIssueSaleItemId",
                table: "CarlErpVendorCreditDetails",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ItemIssueSaletId",
                table: "CarlErpVendorCredit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceiveFrom",
                table: "CarlErpVendorCredit",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "VendorCreditId",
                table: "CarlErpItemIssueVendorCredit",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<Guid>(
                name: "ItemReceiptPuchaseId",
                table: "CarlErpItemIssueVendorCredit",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ItemReceiptPurchaseId",
                table: "CarlErpItemIssueVendorCredit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCreditDetails_ItemIssueSaleItemId",
                table: "CarlErpVendorCreditDetails",
                column: "ItemIssueSaleItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCredit_ItemIssueSaletId",
                table: "CarlErpVendorCredit",
                column: "ItemIssueSaletId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpVendorCredit_CarlErpItemIssues_ItemIssueSaletId",
                table: "CarlErpVendorCredit",
                column: "ItemIssueSaletId",
                principalTable: "CarlErpItemIssues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpVendorCreditDetails_CarlErpItemIssueItems_ItemIssueSaleItemId",
                table: "CarlErpVendorCreditDetails",
                column: "ItemIssueSaleItemId",
                principalTable: "CarlErpItemIssueItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemIssueVendorCredit_CarlErpItemReceipts_ItemReceiptPuchaseId",
                table: "CarlErpItemIssueVendorCredit");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpVendorCredit_CarlErpItemIssues_ItemIssueSaletId",
                table: "CarlErpVendorCredit");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpVendorCreditDetails_CarlErpItemIssueItems_ItemIssueSaleItemId",
                table: "CarlErpVendorCreditDetails");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpVendorCreditDetails_ItemIssueSaleItemId",
                table: "CarlErpVendorCreditDetails");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpVendorCredit_ItemIssueSaletId",
                table: "CarlErpVendorCredit");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemIssueVendorCredit_ItemReceiptPuchaseId",
                table: "CarlErpItemIssueVendorCredit");

            migrationBuilder.DropColumn(
                name: "ItemIssueSaleItemId",
                table: "CarlErpVendorCreditDetails");

            migrationBuilder.DropColumn(
                name: "ItemIssueSaletId",
                table: "CarlErpVendorCredit");

            migrationBuilder.DropColumn(
                name: "ReceiveFrom",
                table: "CarlErpVendorCredit");

            migrationBuilder.DropColumn(
                name: "ItemReceiptPuchaseId",
                table: "CarlErpItemIssueVendorCredit");

            migrationBuilder.DropColumn(
                name: "ItemReceiptPurchaseId",
                table: "CarlErpItemIssueVendorCredit");

            migrationBuilder.AlterColumn<Guid>(
                name: "VendorCreditId",
                table: "CarlErpItemIssueVendorCredit",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
