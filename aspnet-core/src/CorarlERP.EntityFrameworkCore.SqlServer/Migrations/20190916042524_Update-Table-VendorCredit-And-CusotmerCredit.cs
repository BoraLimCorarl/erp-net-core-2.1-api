using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateTableVendorCreditAndCusotmerCredit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpCusotmerCreditDetails_CarlErpItemReceiptItems_ItemReceiptPurchaseItemId",
                table: "CarlErpCusotmerCreditDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpCustomerCredits_CarlErpItemReceipts_ItemReceiptPurchaseId",
                table: "CarlErpCustomerCredits");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpVendorCredit_CarlErpItemIssues_ItemIssueSaletId",
                table: "CarlErpVendorCredit");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpVendorCreditDetails_CarlErpItemIssueItems_ItemIssueSaleItemId",
                table: "CarlErpVendorCreditDetails");

            migrationBuilder.RenameColumn(
                name: "ItemIssueSaleItemId",
                table: "CarlErpVendorCreditDetails",
                newName: "ItemReceiptItemId");

            migrationBuilder.RenameIndex(
                name: "IX_CarlErpVendorCreditDetails_ItemIssueSaleItemId",
                table: "CarlErpVendorCreditDetails",
                newName: "IX_CarlErpVendorCreditDetails_ItemReceiptItemId");

            migrationBuilder.RenameColumn(
                name: "ItemIssueSaletId",
                table: "CarlErpVendorCredit",
                newName: "ItemReceiptId");

            migrationBuilder.RenameIndex(
                name: "IX_CarlErpVendorCredit_ItemIssueSaletId",
                table: "CarlErpVendorCredit",
                newName: "IX_CarlErpVendorCredit_ItemReceiptId");

            migrationBuilder.RenameColumn(
                name: "ItemReceiptPurchaseId",
                table: "CarlErpCustomerCredits",
                newName: "ItemIssueSaleId");

            migrationBuilder.RenameIndex(
                name: "IX_CarlErpCustomerCredits_ItemReceiptPurchaseId",
                table: "CarlErpCustomerCredits",
                newName: "IX_CarlErpCustomerCredits_ItemIssueSaleId");

            migrationBuilder.RenameColumn(
                name: "ItemReceiptPurchaseItemId",
                table: "CarlErpCusotmerCreditDetails",
                newName: "ItemIssueSaleItemId");

            migrationBuilder.RenameIndex(
                name: "IX_CarlErpCusotmerCreditDetails_ItemReceiptPurchaseItemId",
                table: "CarlErpCusotmerCreditDetails",
                newName: "IX_CarlErpCusotmerCreditDetails_ItemIssueSaleItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpCusotmerCreditDetails_CarlErpItemIssueItems_ItemIssueSaleItemId",
                table: "CarlErpCusotmerCreditDetails",
                column: "ItemIssueSaleItemId",
                principalTable: "CarlErpItemIssueItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpCustomerCredits_CarlErpItemIssues_ItemIssueSaleId",
                table: "CarlErpCustomerCredits",
                column: "ItemIssueSaleId",
                principalTable: "CarlErpItemIssues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpVendorCredit_CarlErpItemReceipts_ItemReceiptId",
                table: "CarlErpVendorCredit",
                column: "ItemReceiptId",
                principalTable: "CarlErpItemReceipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpVendorCreditDetails_CarlErpItemReceiptItems_ItemReceiptItemId",
                table: "CarlErpVendorCreditDetails",
                column: "ItemReceiptItemId",
                principalTable: "CarlErpItemReceiptItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpCusotmerCreditDetails_CarlErpItemIssueItems_ItemIssueSaleItemId",
                table: "CarlErpCusotmerCreditDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpCustomerCredits_CarlErpItemIssues_ItemIssueSaleId",
                table: "CarlErpCustomerCredits");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpVendorCredit_CarlErpItemReceipts_ItemReceiptId",
                table: "CarlErpVendorCredit");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpVendorCreditDetails_CarlErpItemReceiptItems_ItemReceiptItemId",
                table: "CarlErpVendorCreditDetails");

            migrationBuilder.RenameColumn(
                name: "ItemReceiptItemId",
                table: "CarlErpVendorCreditDetails",
                newName: "ItemIssueSaleItemId");

            migrationBuilder.RenameIndex(
                name: "IX_CarlErpVendorCreditDetails_ItemReceiptItemId",
                table: "CarlErpVendorCreditDetails",
                newName: "IX_CarlErpVendorCreditDetails_ItemIssueSaleItemId");

            migrationBuilder.RenameColumn(
                name: "ItemReceiptId",
                table: "CarlErpVendorCredit",
                newName: "ItemIssueSaletId");

            migrationBuilder.RenameIndex(
                name: "IX_CarlErpVendorCredit_ItemReceiptId",
                table: "CarlErpVendorCredit",
                newName: "IX_CarlErpVendorCredit_ItemIssueSaletId");

            migrationBuilder.RenameColumn(
                name: "ItemIssueSaleId",
                table: "CarlErpCustomerCredits",
                newName: "ItemReceiptPurchaseId");

            migrationBuilder.RenameIndex(
                name: "IX_CarlErpCustomerCredits_ItemIssueSaleId",
                table: "CarlErpCustomerCredits",
                newName: "IX_CarlErpCustomerCredits_ItemReceiptPurchaseId");

            migrationBuilder.RenameColumn(
                name: "ItemIssueSaleItemId",
                table: "CarlErpCusotmerCreditDetails",
                newName: "ItemReceiptPurchaseItemId");

            migrationBuilder.RenameIndex(
                name: "IX_CarlErpCusotmerCreditDetails_ItemIssueSaleItemId",
                table: "CarlErpCusotmerCreditDetails",
                newName: "IX_CarlErpCusotmerCreditDetails_ItemReceiptPurchaseItemId");

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
    }
}
