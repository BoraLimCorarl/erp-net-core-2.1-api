using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddlotIdInTableReceiptIssue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FromLotId",
                table: "CarlErpTransferOrderItems",
                nullable: true,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ToLotId",
                table: "CarlErpTransferOrderItems",
                nullable: true,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "LotId",
                table: "CarlErpItemReceiptItems",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LotId",
                table: "CarlErpItemReceiptCustomerCreditItem",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LotId",
                table: "CarlErpItemIssueVendorCreditItem",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LotId",
                table: "CarlErpItemIssueItems",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LotId",
                table: "CarlErpInvoiceItems",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LotId",
                table: "CarlErpBillItems",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrderItems_FromLotId",
                table: "CarlErpTransferOrderItems",
                column: "FromLotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrderItems_ToLotId",
                table: "CarlErpTransferOrderItems",
                column: "ToLotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptItems_LotId",
                table: "CarlErpItemReceiptItems",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCreditItem_LotId",
                table: "CarlErpItemReceiptCustomerCreditItem",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCreditItem_LotId",
                table: "CarlErpItemIssueVendorCreditItem",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueItems_LotId",
                table: "CarlErpItemIssueItems",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoiceItems_LotId",
                table: "CarlErpInvoiceItems",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBillItems_LotId",
                table: "CarlErpBillItems",
                column: "LotId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpBillItems_CarlErpLots_LotId",
                table: "CarlErpBillItems",
                column: "LotId",
                principalTable: "CarlErpLots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpInvoiceItems_CarlErpLots_LotId",
                table: "CarlErpInvoiceItems",
                column: "LotId",
                principalTable: "CarlErpLots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemIssueItems_CarlErpLots_LotId",
                table: "CarlErpItemIssueItems",
                column: "LotId",
                principalTable: "CarlErpLots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemIssueVendorCreditItem_CarlErpLots_LotId",
                table: "CarlErpItemIssueVendorCreditItem",
                column: "LotId",
                principalTable: "CarlErpLots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemReceiptCustomerCreditItem_CarlErpLots_LotId",
                table: "CarlErpItemReceiptCustomerCreditItem",
                column: "LotId",
                principalTable: "CarlErpLots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemReceiptItems_CarlErpLots_LotId",
                table: "CarlErpItemReceiptItems",
                column: "LotId",
                principalTable: "CarlErpLots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpTransferOrderItems_CarlErpLots_FromLotId",
                table: "CarlErpTransferOrderItems",
                column: "FromLotId",
                principalTable: "CarlErpLots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpTransferOrderItems_CarlErpLots_ToLotId",
                table: "CarlErpTransferOrderItems",
                column: "ToLotId",
                principalTable: "CarlErpLots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpBillItems_CarlErpLots_LotId",
                table: "CarlErpBillItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpInvoiceItems_CarlErpLots_LotId",
                table: "CarlErpInvoiceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemIssueItems_CarlErpLots_LotId",
                table: "CarlErpItemIssueItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemIssueVendorCreditItem_CarlErpLots_LotId",
                table: "CarlErpItemIssueVendorCreditItem");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemReceiptCustomerCreditItem_CarlErpLots_LotId",
                table: "CarlErpItemReceiptCustomerCreditItem");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemReceiptItems_CarlErpLots_LotId",
                table: "CarlErpItemReceiptItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpTransferOrderItems_CarlErpLots_FromLotId",
                table: "CarlErpTransferOrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpTransferOrderItems_CarlErpLots_ToLotId",
                table: "CarlErpTransferOrderItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpTransferOrderItems_FromLotId",
                table: "CarlErpTransferOrderItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpTransferOrderItems_ToLotId",
                table: "CarlErpTransferOrderItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemReceiptItems_LotId",
                table: "CarlErpItemReceiptItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemReceiptCustomerCreditItem_LotId",
                table: "CarlErpItemReceiptCustomerCreditItem");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemIssueVendorCreditItem_LotId",
                table: "CarlErpItemIssueVendorCreditItem");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemIssueItems_LotId",
                table: "CarlErpItemIssueItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpInvoiceItems_LotId",
                table: "CarlErpInvoiceItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpBillItems_LotId",
                table: "CarlErpBillItems");

            migrationBuilder.DropColumn(
                name: "FromLotId",
                table: "CarlErpTransferOrderItems");

            migrationBuilder.DropColumn(
                name: "ToLotId",
                table: "CarlErpTransferOrderItems");

            migrationBuilder.DropColumn(
                name: "LotId",
                table: "CarlErpItemReceiptItems");

            migrationBuilder.DropColumn(
                name: "LotId",
                table: "CarlErpItemReceiptCustomerCreditItem");

            migrationBuilder.DropColumn(
                name: "LotId",
                table: "CarlErpItemIssueVendorCreditItem");

            migrationBuilder.DropColumn(
                name: "LotId",
                table: "CarlErpItemIssueItems");

            migrationBuilder.DropColumn(
                name: "LotId",
                table: "CarlErpInvoiceItems");

            migrationBuilder.DropColumn(
                name: "LotId",
                table: "CarlErpBillItems");
        }
    }
}
