using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class ProductionPlanAndBatchNo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CarlErpTransProductions_TenantId_CreatorUserId_ProductionNo",
                table: "CarlErpTransProductions");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpTransferOrders_TenantId_CreatorUserId_TransferNo",
                table: "CarlErpTransferOrders");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpSaleOrders_TenantId_OrderNumber",
                table: "CarlErpSaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpPurchaseOrders_TenantId_OrderNumber",
                table: "CarlErpPurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpJournals_TenantId_JournalType",
                table: "CarlErpJournals");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpBankTransfers_TenantId_CreatorUserId_Amount_BankTransferNo",
                table: "CarlErpBankTransfers");

            migrationBuilder.AddColumn<Guid>(
                name: "BatchNoId",
                table: "CarlErpVendorCreditDetails",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductionPlanId",
                table: "CarlErpTransProductions",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BatchNoId",
                table: "CarlErpTransferOrderItems",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BatchNoId",
                table: "CarlErpRawMaterialItems",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UseStandard",
                table: "CarlErpProductionProcess",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AutoBatchNo",
                table: "CarlErpItems",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "BatchNoFormulaId",
                table: "CarlErpItems",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UseBatchNo",
                table: "CarlErpItems",
                nullable: false,
                defaultValue: false);

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

            migrationBuilder.CreateTable(
                name: "CarlErpBatchNoFormulas",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    StandardPrePos = table.Column<int>(nullable: false),
                    PrePosCode = table.Column<string>(nullable: true),
                    DateFormat = table.Column<string>(nullable: false),
                    FieldName = table.Column<string>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpBatchNoFormulas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpBatchNos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Code = table.Column<string>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpBatchNos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpBatchNos_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpBatchNos_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpProductionPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    DocumentNo = table.Column<string>(nullable: false),
                    Reference = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    LocationId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpProductionPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpProductionPlans_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpProductionPlans_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpProductionPlans_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpInventoryCostCloseItemBatchNos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    LotId = table.Column<long>(nullable: false),
                    BatchNoId = table.Column<Guid>(nullable: true),
                    Qty = table.Column<decimal>(nullable: false),
                    InventoryCostCloseId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpInventoryCostCloseItemBatchNos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpInventoryCostCloseItemBatchNos_CarlErpBatchNos_BatchNoId",
                        column: x => x.BatchNoId,
                        principalTable: "CarlErpBatchNos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInventoryCostCloseItemBatchNos_CarlErpInventoryCostCloses_InventoryCostCloseId",
                        column: x => x.InventoryCostCloseId,
                        principalTable: "CarlErpInventoryCostCloses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInventoryCostCloseItemBatchNos_CarlErpLots_LotId",
                        column: x => x.LotId,
                        principalTable: "CarlErpLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCreditDetails_BatchNoId",
                table: "CarlErpVendorCreditDetails",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransProductions_ProductionPlanId",
                table: "CarlErpTransProductions",
                column: "ProductionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransProductions_TenantId_ProductionNo",
                table: "CarlErpTransProductions",
                columns: new[] { "TenantId", "ProductionNo" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrders_TenantId_TransferNo",
                table: "CarlErpTransferOrders",
                columns: new[] { "TenantId", "TransferNo" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrderItems_BatchNoId",
                table: "CarlErpTransferOrderItems",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrders_TenantId_OrderNumber",
                table: "CarlErpSaleOrders",
                columns: new[] { "TenantId", "OrderNumber" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpRawMaterialItems_BatchNoId",
                table: "CarlErpRawMaterialItems",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrders_TenantId_OrderNumber",
                table: "CarlErpPurchaseOrders",
                columns: new[] { "TenantId", "OrderNumber" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_TenantId_JournalType_JournalNo",
                table: "CarlErpJournals",
                columns: new[] { "TenantId", "JournalType", "JournalNo" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_BatchNoFormulaId",
                table: "CarlErpItems",
                column: "BatchNoFormulaId");

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
                name: "IX_CarlErpBankTransfers_TenantId_BankTransferNo",
                table: "CarlErpBankTransfers",
                columns: new[] { "TenantId", "BankTransferNo" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

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

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCostCloseItemBatchNos_BatchNoId",
                table: "CarlErpInventoryCostCloseItemBatchNos",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCostCloseItemBatchNos_InventoryCostCloseId",
                table: "CarlErpInventoryCostCloseItemBatchNos",
                column: "InventoryCostCloseId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCostCloseItemBatchNos_LotId",
                table: "CarlErpInventoryCostCloseItemBatchNos",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionPlans_CreatorUserId",
                table: "CarlErpProductionPlans",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionPlans_LastModifierUserId",
                table: "CarlErpProductionPlans",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionPlans_LocationId",
                table: "CarlErpProductionPlans",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionPlans_Reference",
                table: "CarlErpProductionPlans",
                column: "Reference");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionPlans_TenantId_DocumentNo",
                table: "CarlErpProductionPlans",
                columns: new[] { "TenantId", "DocumentNo" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

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
                name: "FK_CarlErpItems_CarlErpBatchNoFormulas_BatchNoFormulaId",
                table: "CarlErpItems",
                column: "BatchNoFormulaId",
                principalTable: "CarlErpBatchNoFormulas",
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
                name: "FK_CarlErpTransProductions_CarlErpProductionPlans_ProductionPlanId",
                table: "CarlErpTransProductions",
                column: "ProductionPlanId",
                principalTable: "CarlErpProductionPlans",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "FK_CarlErpItems_CarlErpBatchNoFormulas_BatchNoFormulaId",
                table: "CarlErpItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpRawMaterialItems_CarlErpBatchNos_BatchNoId",
                table: "CarlErpRawMaterialItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpTransferOrderItems_CarlErpBatchNos_BatchNoId",
                table: "CarlErpTransferOrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpTransProductions_CarlErpProductionPlans_ProductionPlanId",
                table: "CarlErpTransProductions");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpVendorCreditDetails_CarlErpBatchNos_BatchNoId",
                table: "CarlErpVendorCreditDetails");

            migrationBuilder.DropTable(
                name: "CarlErpBatchNoFormulas");

            migrationBuilder.DropTable(
                name: "CarlErpInventoryCostCloseItemBatchNos");

            migrationBuilder.DropTable(
                name: "CarlErpProductionPlans");

            migrationBuilder.DropTable(
                name: "CarlErpBatchNos");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpVendorCreditDetails_BatchNoId",
                table: "CarlErpVendorCreditDetails");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpTransProductions_ProductionPlanId",
                table: "CarlErpTransProductions");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpTransProductions_TenantId_ProductionNo",
                table: "CarlErpTransProductions");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpTransferOrders_TenantId_TransferNo",
                table: "CarlErpTransferOrders");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpTransferOrderItems_BatchNoId",
                table: "CarlErpTransferOrderItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpSaleOrders_TenantId_OrderNumber",
                table: "CarlErpSaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpRawMaterialItems_BatchNoId",
                table: "CarlErpRawMaterialItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpPurchaseOrders_TenantId_OrderNumber",
                table: "CarlErpPurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpJournals_TenantId_JournalType_JournalNo",
                table: "CarlErpJournals");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItems_BatchNoFormulaId",
                table: "CarlErpItems");

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
                name: "IX_CarlErpBankTransfers_TenantId_BankTransferNo",
                table: "CarlErpBankTransfers");

            migrationBuilder.DropColumn(
                name: "BatchNoId",
                table: "CarlErpVendorCreditDetails");

            migrationBuilder.DropColumn(
                name: "ProductionPlanId",
                table: "CarlErpTransProductions");

            migrationBuilder.DropColumn(
                name: "BatchNoId",
                table: "CarlErpTransferOrderItems");

            migrationBuilder.DropColumn(
                name: "BatchNoId",
                table: "CarlErpRawMaterialItems");

            migrationBuilder.DropColumn(
                name: "UseStandard",
                table: "CarlErpProductionProcess");

            migrationBuilder.DropColumn(
                name: "AutoBatchNo",
                table: "CarlErpItems");

            migrationBuilder.DropColumn(
                name: "BatchNoFormulaId",
                table: "CarlErpItems");

            migrationBuilder.DropColumn(
                name: "UseBatchNo",
                table: "CarlErpItems");

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

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransProductions_TenantId_CreatorUserId_ProductionNo",
                table: "CarlErpTransProductions",
                columns: new[] { "TenantId", "CreatorUserId", "ProductionNo" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrders_TenantId_CreatorUserId_TransferNo",
                table: "CarlErpTransferOrders",
                columns: new[] { "TenantId", "CreatorUserId", "TransferNo" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrders_TenantId_OrderNumber",
                table: "CarlErpSaleOrders",
                columns: new[] { "TenantId", "OrderNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrders_TenantId_OrderNumber",
                table: "CarlErpPurchaseOrders",
                columns: new[] { "TenantId", "OrderNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_TenantId_JournalType",
                table: "CarlErpJournals",
                columns: new[] { "TenantId", "JournalType" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBankTransfers_TenantId_CreatorUserId_Amount_BankTransferNo",
                table: "CarlErpBankTransfers",
                columns: new[] { "TenantId", "CreatorUserId", "Amount", "BankTransferNo" });
        }
    }
}
