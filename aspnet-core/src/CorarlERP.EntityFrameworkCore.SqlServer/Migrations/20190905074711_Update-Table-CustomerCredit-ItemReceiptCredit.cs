using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateTableCustomerCreditItemReceiptCredit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ItemIssueSaleItemId",
                table: "CarlErpItemReceiptCustomerCreditItem",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerCreditId",
                table: "CarlErpItemReceiptCustomerCredit",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<Guid>(
                name: "ItemIssueSaleId",
                table: "CarlErpItemReceiptCustomerCredit",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ItemReceiptPurchaseItemId",
                table: "CarlErpItemIssueVendorCreditItem",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ItemReceiptPurchaseId",
                table: "CarlErpCustomerCredits",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceiveFrom",
                table: "CarlErpCustomerCredits",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ItemReceiptPurchaseItemId",
                table: "CarlErpCusotmerCreditDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCreditItem_ItemIssueSaleItemId",
                table: "CarlErpItemReceiptCustomerCreditItem",
                column: "ItemIssueSaleItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCredit_ItemIssueSaleId",
                table: "CarlErpItemReceiptCustomerCredit",
                column: "ItemIssueSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCreditItem_ItemReceiptPurchaseItemId",
                table: "CarlErpItemIssueVendorCreditItem",
                column: "ItemReceiptPurchaseItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerCredits_ItemReceiptPurchaseId",
                table: "CarlErpCustomerCredits",
                column: "ItemReceiptPurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCusotmerCreditDetails_ItemReceiptPurchaseItemId",
                table: "CarlErpCusotmerCreditDetails",
                column: "ItemReceiptPurchaseItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpCusotmerCreditDetails_CarlErpItemReceiptItems_ItemReceiptPurchaseItemId",
                table: "CarlErpCusotmerCreditDetails",
                column: "ItemReceiptPurchaseItemId",
                principalTable: "CarlErpItemReceiptItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpCustomerCredits_CarlErpItemReceipts_ItemReceiptPurchaseId",
                table: "CarlErpCustomerCredits",
                column: "ItemReceiptPurchaseId",
                principalTable: "CarlErpItemReceipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemIssueVendorCreditItem_CarlErpItemReceiptItems_ItemReceiptPurchaseItemId",
                table: "CarlErpItemIssueVendorCreditItem",
                column: "ItemReceiptPurchaseItemId",
                principalTable: "CarlErpItemReceiptItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemReceiptCustomerCredit_CarlErpItemIssues_ItemIssueSaleId",
                table: "CarlErpItemReceiptCustomerCredit",
                column: "ItemIssueSaleId",
                principalTable: "CarlErpItemIssues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemReceiptCustomerCreditItem_CarlErpItemIssueItems_ItemIssueSaleItemId",
                table: "CarlErpItemReceiptCustomerCreditItem",
                column: "ItemIssueSaleItemId",
                principalTable: "CarlErpItemIssueItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpCusotmerCreditDetails_CarlErpItemReceiptItems_ItemReceiptPurchaseItemId",
                table: "CarlErpCusotmerCreditDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpCustomerCredits_CarlErpItemReceipts_ItemReceiptPurchaseId",
                table: "CarlErpCustomerCredits");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemIssueVendorCreditItem_CarlErpItemReceiptItems_ItemReceiptPurchaseItemId",
                table: "CarlErpItemIssueVendorCreditItem");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemReceiptCustomerCredit_CarlErpItemIssues_ItemIssueSaleId",
                table: "CarlErpItemReceiptCustomerCredit");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemReceiptCustomerCreditItem_CarlErpItemIssueItems_ItemIssueSaleItemId",
                table: "CarlErpItemReceiptCustomerCreditItem");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemReceiptCustomerCreditItem_ItemIssueSaleItemId",
                table: "CarlErpItemReceiptCustomerCreditItem");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemReceiptCustomerCredit_ItemIssueSaleId",
                table: "CarlErpItemReceiptCustomerCredit");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemIssueVendorCreditItem_ItemReceiptPurchaseItemId",
                table: "CarlErpItemIssueVendorCreditItem");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpCustomerCredits_ItemReceiptPurchaseId",
                table: "CarlErpCustomerCredits");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpCusotmerCreditDetails_ItemReceiptPurchaseItemId",
                table: "CarlErpCusotmerCreditDetails");

            migrationBuilder.DropColumn(
                name: "ItemIssueSaleItemId",
                table: "CarlErpItemReceiptCustomerCreditItem");

            migrationBuilder.DropColumn(
                name: "ItemIssueSaleId",
                table: "CarlErpItemReceiptCustomerCredit");

            migrationBuilder.DropColumn(
                name: "ItemReceiptPurchaseItemId",
                table: "CarlErpItemIssueVendorCreditItem");

            migrationBuilder.DropColumn(
                name: "ItemReceiptPurchaseId",
                table: "CarlErpCustomerCredits");

            migrationBuilder.DropColumn(
                name: "ReceiveFrom",
                table: "CarlErpCustomerCredits");

            migrationBuilder.DropColumn(
                name: "ItemReceiptPurchaseItemId",
                table: "CarlErpCusotmerCreditDetails");

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerCreditId",
                table: "CarlErpItemReceiptCustomerCredit",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
