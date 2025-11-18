using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateBatchNos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"Delete from CarlErpInventoryCostCloseItemBatchNos");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpBatchNos_AbpUsers_CreatorUserId",
                table: "CarlErpBatchNos");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpBatchNos_AbpUsers_LastModifierUserId",
                table: "CarlErpBatchNos");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpBillItems_CarlErpBatchNos_BatchNoId",
                table: "CarlErpBillItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpCustomerCreditDetails_CarlErpBatchNos_BatchNoId",
                table: "CarlErpCustomerCreditDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpFinishItems_CarlErpBatchNos_BatchNoId",
                table: "CarlErpFinishItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpInvoiceItems_CarlErpBatchNos_BatchNoId",
                table: "CarlErpInvoiceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemIssueItems_CarlErpBatchNos_BatchNoId",
                table: "CarlErpItemIssueItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemIssueVendorCreditItem_CarlErpBatchNos_BatchNoId",
                table: "CarlErpItemIssueVendorCreditItem");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemReceiptCustomerCreditItem_CarlErpBatchNos_BatchNoId",
                table: "CarlErpItemReceiptCustomerCreditItem");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemReceiptItems_CarlErpBatchNos_BatchNoId",
                table: "CarlErpItemReceiptItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpRawMaterialItems_CarlErpBatchNos_BatchNoId",
                table: "CarlErpRawMaterialItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpTransferOrderItems_CarlErpBatchNos_BatchNoId",
                table: "CarlErpTransferOrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpVendorCreditDetails_CarlErpBatchNos_BatchNoId",
                table: "CarlErpVendorCreditDetails");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpVendorCreditDetails_BatchNoId",
                table: "CarlErpVendorCreditDetails");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpTransferOrderItems_BatchNoId",
                table: "CarlErpTransferOrderItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpRawMaterialItems_BatchNoId",
                table: "CarlErpRawMaterialItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemReceiptItems_BatchNoId",
                table: "CarlErpItemReceiptItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemReceiptCustomerCreditItem_BatchNoId",
                table: "CarlErpItemReceiptCustomerCreditItem");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemIssueVendorCreditItem_BatchNoId",
                table: "CarlErpItemIssueVendorCreditItem");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemIssueItems_BatchNoId",
                table: "CarlErpItemIssueItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpInvoiceItems_BatchNoId",
                table: "CarlErpInvoiceItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpFinishItems_BatchNoId",
                table: "CarlErpFinishItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpCustomerCreditDetails_BatchNoId",
                table: "CarlErpCustomerCreditDetails");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpBillItems_BatchNoId",
                table: "CarlErpBillItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpBatchNos_CreatorUserId",
                table: "CarlErpBatchNos");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpBatchNos_LastModifierUserId",
                table: "CarlErpBatchNos");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpBatchNos_TenantId_Code_ItemId",
                table: "CarlErpBatchNos");

            migrationBuilder.DropColumn(
                name: "BatchNoId",
                table: "CarlErpVendorCreditDetails");

            migrationBuilder.DropColumn(
                name: "BatchNoId",
                table: "CarlErpTransferOrderItems");

            migrationBuilder.DropColumn(
                name: "BatchNoId",
                table: "CarlErpRawMaterialItems");

            migrationBuilder.DropColumn(
                name: "BatchNoId",
                table: "CarlErpItemReceiptItems");

            migrationBuilder.DropColumn(
                name: "BatchNoId",
                table: "CarlErpItemReceiptCustomerCreditItem");

            migrationBuilder.DropColumn(
                name: "BatchNoId",
                table: "CarlErpItemIssueVendorCreditItem");

            migrationBuilder.DropColumn(
                name: "BatchNoId",
                table: "CarlErpItemIssueItems");

            migrationBuilder.DropColumn(
                name: "BatchNoId",
                table: "CarlErpInvoiceItems");

            migrationBuilder.DropColumn(
                name: "BatchNoId",
                table: "CarlErpFinishItems");

            migrationBuilder.DropColumn(
                name: "BatchNoId",
                table: "CarlErpCustomerCreditDetails");

            migrationBuilder.DropColumn(
                name: "BatchNoId",
                table: "CarlErpBillItems");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOnly",
                table: "CarlErpJournals",
                type: "Date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<Guid>(
                name: "BatchNoId",
                table: "CarlErpInventoryCostCloseItemBatchNos",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "CarlErpBatchNos",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BalanceQty",
                table: "CarlErpBatchNos",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsStandard",
                table: "CarlErpBatchNos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "IssueQty",
                table: "CarlErpBatchNos",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReceiptQty",
                table: "CarlErpBatchNos",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "CarlErpCustomerCreditItemBatchNos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    CustomerCreditItemId = table.Column<Guid>(nullable: false),
                    BatchNoId = table.Column<Guid>(nullable: false),
                    Qty = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCustomerCreditItemBatchNos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomerCreditItemBatchNos_CarlErpBatchNos_BatchNoId",
                        column: x => x.BatchNoId,
                        principalTable: "CarlErpBatchNos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomerCreditItemBatchNos_CarlErpCustomerCreditDetails_CustomerCreditItemId",
                        column: x => x.CustomerCreditItemId,
                        principalTable: "CarlErpCustomerCreditDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemIssueItemBatchNos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    ItemIssueItemId = table.Column<Guid>(nullable: false),
                    BatchNoId = table.Column<Guid>(nullable: false),
                    Qty = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemIssueItemBatchNos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueItemBatchNos_CarlErpBatchNos_BatchNoId",
                        column: x => x.BatchNoId,
                        principalTable: "CarlErpBatchNos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueItemBatchNos_CarlErpItemIssueItems_ItemIssueItemId",
                        column: x => x.ItemIssueItemId,
                        principalTable: "CarlErpItemIssueItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemIssueVendorCreditItemBatchNos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    ItemIssueVendorCreditItemId = table.Column<Guid>(nullable: false),
                    BatchNoId = table.Column<Guid>(nullable: false),
                    Qty = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemIssueVendorCreditItemBatchNos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueVendorCreditItemBatchNos_CarlErpBatchNos_BatchNoId",
                        column: x => x.BatchNoId,
                        principalTable: "CarlErpBatchNos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueVendorCreditItemBatchNos_CarlErpItemIssueVendorCreditItem_ItemIssueVendorCreditItemId",
                        column: x => x.ItemIssueVendorCreditItemId,
                        principalTable: "CarlErpItemIssueVendorCreditItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemReceiptCustomerCreditItemBatchNos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    ItemReceiptCustomerCreditItemId = table.Column<Guid>(nullable: false),
                    BatchNoId = table.Column<Guid>(nullable: false),
                    Qty = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemReceiptCustomerCreditItemBatchNos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptCustomerCreditItemBatchNos_CarlErpBatchNos_BatchNoId",
                        column: x => x.BatchNoId,
                        principalTable: "CarlErpBatchNos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptCustomerCreditItemBatchNos_CarlErpItemReceiptCustomerCreditItem_ItemReceiptCustomerCreditItemId",
                        column: x => x.ItemReceiptCustomerCreditItemId,
                        principalTable: "CarlErpItemReceiptCustomerCreditItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemReceiptItemBatchNos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    ItemReceiptItemId = table.Column<Guid>(nullable: false),
                    BatchNoId = table.Column<Guid>(nullable: false),
                    Qty = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemReceiptItemBatchNos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptItemBatchNos_CarlErpBatchNos_BatchNoId",
                        column: x => x.BatchNoId,
                        principalTable: "CarlErpBatchNos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptItemBatchNos_CarlErpItemReceiptItems_ItemReceiptItemId",
                        column: x => x.ItemReceiptItemId,
                        principalTable: "CarlErpItemReceiptItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpVendorCreditItemBatchNos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    VendorCreditItemId = table.Column<Guid>(nullable: false),
                    BatchNoId = table.Column<Guid>(nullable: false),
                    Qty = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpVendorCreditItemBatchNos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorCreditItemBatchNos_CarlErpBatchNos_BatchNoId",
                        column: x => x.BatchNoId,
                        principalTable: "CarlErpBatchNos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorCreditItemBatchNos_CarlErpVendorCreditDetails_VendorCreditItemId",
                        column: x => x.VendorCreditItemId,
                        principalTable: "CarlErpVendorCreditDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_DateOnly",
                table: "CarlErpJournals",
                column: "DateOnly");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBatchNos_IsStandard",
                table: "CarlErpBatchNos",
                column: "IsStandard");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBatchNos_TenantId_Code_ItemId",
                table: "CarlErpBatchNos",
                columns: new[] { "TenantId", "Code", "ItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerCreditItemBatchNos_BatchNoId",
                table: "CarlErpCustomerCreditItemBatchNos",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerCreditItemBatchNos_CustomerCreditItemId",
                table: "CarlErpCustomerCreditItemBatchNos",
                column: "CustomerCreditItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueItemBatchNos_BatchNoId",
                table: "CarlErpItemIssueItemBatchNos",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueItemBatchNos_ItemIssueItemId",
                table: "CarlErpItemIssueItemBatchNos",
                column: "ItemIssueItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCreditItemBatchNos_BatchNoId",
                table: "CarlErpItemIssueVendorCreditItemBatchNos",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCreditItemBatchNos_ItemIssueVendorCreditItemId",
                table: "CarlErpItemIssueVendorCreditItemBatchNos",
                column: "ItemIssueVendorCreditItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCreditItemBatchNos_BatchNoId",
                table: "CarlErpItemReceiptCustomerCreditItemBatchNos",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCreditItemBatchNos_ItemReceiptCustomerCreditItemId",
                table: "CarlErpItemReceiptCustomerCreditItemBatchNos",
                column: "ItemReceiptCustomerCreditItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptItemBatchNos_BatchNoId",
                table: "CarlErpItemReceiptItemBatchNos",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptItemBatchNos_ItemReceiptItemId",
                table: "CarlErpItemReceiptItemBatchNos",
                column: "ItemReceiptItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCreditItemBatchNos_BatchNoId",
                table: "CarlErpVendorCreditItemBatchNos",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCreditItemBatchNos_VendorCreditItemId",
                table: "CarlErpVendorCreditItemBatchNos",
                column: "VendorCreditItemId");

            migrationBuilder.Sql(@"Update CarlErpJournals set DateOnly = DATEADD(dd, 0, DATEDIFF(dd, 0, Date))");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpCustomerCreditItemBatchNos");

            migrationBuilder.DropTable(
                name: "CarlErpItemIssueItemBatchNos");

            migrationBuilder.DropTable(
                name: "CarlErpItemIssueVendorCreditItemBatchNos");

            migrationBuilder.DropTable(
                name: "CarlErpItemReceiptCustomerCreditItemBatchNos");

            migrationBuilder.DropTable(
                name: "CarlErpItemReceiptItemBatchNos");

            migrationBuilder.DropTable(
                name: "CarlErpVendorCreditItemBatchNos");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpJournals_DateOnly",
                table: "CarlErpJournals");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpBatchNos_IsStandard",
                table: "CarlErpBatchNos");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpBatchNos_TenantId_Code_ItemId",
                table: "CarlErpBatchNos");

            migrationBuilder.DropColumn(
                name: "DateOnly",
                table: "CarlErpJournals");

            migrationBuilder.DropColumn(
                name: "BalanceQty",
                table: "CarlErpBatchNos");

            migrationBuilder.DropColumn(
                name: "IsStandard",
                table: "CarlErpBatchNos");

            migrationBuilder.DropColumn(
                name: "IssueQty",
                table: "CarlErpBatchNos");

            migrationBuilder.DropColumn(
                name: "ReceiptQty",
                table: "CarlErpBatchNos");

            migrationBuilder.AddColumn<Guid>(
                name: "BatchNoId",
                table: "CarlErpVendorCreditDetails",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BatchNoId",
                table: "CarlErpTransferOrderItems",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BatchNoId",
                table: "CarlErpRawMaterialItems",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BatchNoId",
                table: "CarlErpItemReceiptItems",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BatchNoId",
                table: "CarlErpItemReceiptCustomerCreditItem",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BatchNoId",
                table: "CarlErpItemIssueVendorCreditItem",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BatchNoId",
                table: "CarlErpItemIssueItems",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BatchNoId",
                table: "CarlErpInvoiceItems",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "BatchNoId",
                table: "CarlErpInventoryCostCloseItemBatchNos",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<Guid>(
                name: "BatchNoId",
                table: "CarlErpFinishItems",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BatchNoId",
                table: "CarlErpCustomerCreditDetails",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BatchNoId",
                table: "CarlErpBillItems",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "CarlErpBatchNos",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCreditDetails_BatchNoId",
                table: "CarlErpVendorCreditDetails",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrderItems_BatchNoId",
                table: "CarlErpTransferOrderItems",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpRawMaterialItems_BatchNoId",
                table: "CarlErpRawMaterialItems",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptItems_BatchNoId",
                table: "CarlErpItemReceiptItems",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCreditItem_BatchNoId",
                table: "CarlErpItemReceiptCustomerCreditItem",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCreditItem_BatchNoId",
                table: "CarlErpItemIssueVendorCreditItem",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueItems_BatchNoId",
                table: "CarlErpItemIssueItems",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoiceItems_BatchNoId",
                table: "CarlErpInvoiceItems",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpFinishItems_BatchNoId",
                table: "CarlErpFinishItems",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerCreditDetails_BatchNoId",
                table: "CarlErpCustomerCreditDetails",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBillItems_BatchNoId",
                table: "CarlErpBillItems",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBatchNos_CreatorUserId",
                table: "CarlErpBatchNos",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBatchNos_LastModifierUserId",
                table: "CarlErpBatchNos",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBatchNos_TenantId_Code_ItemId",
                table: "CarlErpBatchNos",
                columns: new[] { "TenantId", "Code", "ItemId" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpBatchNos_AbpUsers_CreatorUserId",
                table: "CarlErpBatchNos",
                column: "CreatorUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpBatchNos_AbpUsers_LastModifierUserId",
                table: "CarlErpBatchNos",
                column: "LastModifierUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpBillItems_CarlErpBatchNos_BatchNoId",
                table: "CarlErpBillItems",
                column: "BatchNoId",
                principalTable: "CarlErpBatchNos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpCustomerCreditDetails_CarlErpBatchNos_BatchNoId",
                table: "CarlErpCustomerCreditDetails",
                column: "BatchNoId",
                principalTable: "CarlErpBatchNos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpFinishItems_CarlErpBatchNos_BatchNoId",
                table: "CarlErpFinishItems",
                column: "BatchNoId",
                principalTable: "CarlErpBatchNos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpInvoiceItems_CarlErpBatchNos_BatchNoId",
                table: "CarlErpInvoiceItems",
                column: "BatchNoId",
                principalTable: "CarlErpBatchNos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemIssueItems_CarlErpBatchNos_BatchNoId",
                table: "CarlErpItemIssueItems",
                column: "BatchNoId",
                principalTable: "CarlErpBatchNos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemIssueVendorCreditItem_CarlErpBatchNos_BatchNoId",
                table: "CarlErpItemIssueVendorCreditItem",
                column: "BatchNoId",
                principalTable: "CarlErpBatchNos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemReceiptCustomerCreditItem_CarlErpBatchNos_BatchNoId",
                table: "CarlErpItemReceiptCustomerCreditItem",
                column: "BatchNoId",
                principalTable: "CarlErpBatchNos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemReceiptItems_CarlErpBatchNos_BatchNoId",
                table: "CarlErpItemReceiptItems",
                column: "BatchNoId",
                principalTable: "CarlErpBatchNos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpRawMaterialItems_CarlErpBatchNos_BatchNoId",
                table: "CarlErpRawMaterialItems",
                column: "BatchNoId",
                principalTable: "CarlErpBatchNos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpTransferOrderItems_CarlErpBatchNos_BatchNoId",
                table: "CarlErpTransferOrderItems",
                column: "BatchNoId",
                principalTable: "CarlErpBatchNos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpVendorCreditDetails_CarlErpBatchNos_BatchNoId",
                table: "CarlErpVendorCreditDetails",
                column: "BatchNoId",
                principalTable: "CarlErpBatchNos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
