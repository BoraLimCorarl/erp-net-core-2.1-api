using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class RenameTableCustomerCreditDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpCusotmerCreditDetails_CarlErpCustomerCredits_CustomerCreditId",
                table: "CarlErpCusotmerCreditDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpCusotmerCreditDetails_CarlErpItems_ItemId",
                table: "CarlErpCusotmerCreditDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpCusotmerCreditDetails_CarlErpItemIssueItems_ItemIssueSaleItemId",
                table: "CarlErpCusotmerCreditDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpCusotmerCreditDetails_CarlErpLots_LotId",
                table: "CarlErpCusotmerCreditDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpCusotmerCreditDetails_CarlErpTaxes_TaxId",
                table: "CarlErpCusotmerCreditDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemReceiptCustomerCreditItem_CarlErpCusotmerCreditDetails_CustomerCreditItemId",
                table: "CarlErpItemReceiptCustomerCreditItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CarlErpCusotmerCreditDetails",
                table: "CarlErpCusotmerCreditDetails");

            migrationBuilder.RenameTable(
                name: "CarlErpCusotmerCreditDetails",
                newName: "CarlErpCustomerCreditDetails");

            migrationBuilder.RenameIndex(
                name: "IX_CarlErpCusotmerCreditDetails_TenantId_CreatorUserId",
                table: "CarlErpCustomerCreditDetails",
                newName: "IX_CarlErpCustomerCreditDetails_TenantId_CreatorUserId");

            migrationBuilder.RenameIndex(
                name: "IX_CarlErpCusotmerCreditDetails_TaxId",
                table: "CarlErpCustomerCreditDetails",
                newName: "IX_CarlErpCustomerCreditDetails_TaxId");

            migrationBuilder.RenameIndex(
                name: "IX_CarlErpCusotmerCreditDetails_LotId",
                table: "CarlErpCustomerCreditDetails",
                newName: "IX_CarlErpCustomerCreditDetails_LotId");

            migrationBuilder.RenameIndex(
                name: "IX_CarlErpCusotmerCreditDetails_ItemIssueSaleItemId",
                table: "CarlErpCustomerCreditDetails",
                newName: "IX_CarlErpCustomerCreditDetails_ItemIssueSaleItemId");

            migrationBuilder.RenameIndex(
                name: "IX_CarlErpCusotmerCreditDetails_ItemId",
                table: "CarlErpCustomerCreditDetails",
                newName: "IX_CarlErpCustomerCreditDetails_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_CarlErpCusotmerCreditDetails_CustomerCreditId",
                table: "CarlErpCustomerCreditDetails",
                newName: "IX_CarlErpCustomerCreditDetails_CustomerCreditId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CarlErpCustomerCreditDetails",
                table: "CarlErpCustomerCreditDetails",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpCustomerCreditDetails_CarlErpCustomerCredits_CustomerCreditId",
                table: "CarlErpCustomerCreditDetails",
                column: "CustomerCreditId",
                principalTable: "CarlErpCustomerCredits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpCustomerCreditDetails_CarlErpItems_ItemId",
                table: "CarlErpCustomerCreditDetails",
                column: "ItemId",
                principalTable: "CarlErpItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpCustomerCreditDetails_CarlErpItemIssueItems_ItemIssueSaleItemId",
                table: "CarlErpCustomerCreditDetails",
                column: "ItemIssueSaleItemId",
                principalTable: "CarlErpItemIssueItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpCustomerCreditDetails_CarlErpLots_LotId",
                table: "CarlErpCustomerCreditDetails",
                column: "LotId",
                principalTable: "CarlErpLots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpCustomerCreditDetails_CarlErpTaxes_TaxId",
                table: "CarlErpCustomerCreditDetails",
                column: "TaxId",
                principalTable: "CarlErpTaxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemReceiptCustomerCreditItem_CarlErpCustomerCreditDetails_CustomerCreditItemId",
                table: "CarlErpItemReceiptCustomerCreditItem",
                column: "CustomerCreditItemId",
                principalTable: "CarlErpCustomerCreditDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpCustomerCreditDetails_CarlErpCustomerCredits_CustomerCreditId",
                table: "CarlErpCustomerCreditDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpCustomerCreditDetails_CarlErpItems_ItemId",
                table: "CarlErpCustomerCreditDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpCustomerCreditDetails_CarlErpItemIssueItems_ItemIssueSaleItemId",
                table: "CarlErpCustomerCreditDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpCustomerCreditDetails_CarlErpLots_LotId",
                table: "CarlErpCustomerCreditDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpCustomerCreditDetails_CarlErpTaxes_TaxId",
                table: "CarlErpCustomerCreditDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemReceiptCustomerCreditItem_CarlErpCustomerCreditDetails_CustomerCreditItemId",
                table: "CarlErpItemReceiptCustomerCreditItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CarlErpCustomerCreditDetails",
                table: "CarlErpCustomerCreditDetails");

            migrationBuilder.RenameTable(
                name: "CarlErpCustomerCreditDetails",
                newName: "CarlErpCusotmerCreditDetails");

            migrationBuilder.RenameIndex(
                name: "IX_CarlErpCustomerCreditDetails_TenantId_CreatorUserId",
                table: "CarlErpCusotmerCreditDetails",
                newName: "IX_CarlErpCusotmerCreditDetails_TenantId_CreatorUserId");

            migrationBuilder.RenameIndex(
                name: "IX_CarlErpCustomerCreditDetails_TaxId",
                table: "CarlErpCusotmerCreditDetails",
                newName: "IX_CarlErpCusotmerCreditDetails_TaxId");

            migrationBuilder.RenameIndex(
                name: "IX_CarlErpCustomerCreditDetails_LotId",
                table: "CarlErpCusotmerCreditDetails",
                newName: "IX_CarlErpCusotmerCreditDetails_LotId");

            migrationBuilder.RenameIndex(
                name: "IX_CarlErpCustomerCreditDetails_ItemIssueSaleItemId",
                table: "CarlErpCusotmerCreditDetails",
                newName: "IX_CarlErpCusotmerCreditDetails_ItemIssueSaleItemId");

            migrationBuilder.RenameIndex(
                name: "IX_CarlErpCustomerCreditDetails_ItemId",
                table: "CarlErpCusotmerCreditDetails",
                newName: "IX_CarlErpCusotmerCreditDetails_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_CarlErpCustomerCreditDetails_CustomerCreditId",
                table: "CarlErpCusotmerCreditDetails",
                newName: "IX_CarlErpCusotmerCreditDetails_CustomerCreditId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CarlErpCusotmerCreditDetails",
                table: "CarlErpCusotmerCreditDetails",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpCusotmerCreditDetails_CarlErpCustomerCredits_CustomerCreditId",
                table: "CarlErpCusotmerCreditDetails",
                column: "CustomerCreditId",
                principalTable: "CarlErpCustomerCredits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpCusotmerCreditDetails_CarlErpItems_ItemId",
                table: "CarlErpCusotmerCreditDetails",
                column: "ItemId",
                principalTable: "CarlErpItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpCusotmerCreditDetails_CarlErpItemIssueItems_ItemIssueSaleItemId",
                table: "CarlErpCusotmerCreditDetails",
                column: "ItemIssueSaleItemId",
                principalTable: "CarlErpItemIssueItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpCusotmerCreditDetails_CarlErpLots_LotId",
                table: "CarlErpCusotmerCreditDetails",
                column: "LotId",
                principalTable: "CarlErpLots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpCusotmerCreditDetails_CarlErpTaxes_TaxId",
                table: "CarlErpCusotmerCreditDetails",
                column: "TaxId",
                principalTable: "CarlErpTaxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemReceiptCustomerCreditItem_CarlErpCusotmerCreditDetails_CustomerCreditItemId",
                table: "CarlErpItemReceiptCustomerCreditItem",
                column: "CustomerCreditItemId",
                principalTable: "CarlErpCusotmerCreditDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
