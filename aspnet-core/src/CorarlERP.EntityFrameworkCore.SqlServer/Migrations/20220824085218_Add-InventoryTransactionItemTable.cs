using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddInventoryTransactionItemTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarlErpInventoryTransactionItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    TransactionId = table.Column<Guid>(nullable: false),
                    TransferOrProductionId = table.Column<Guid>(nullable: true),
                    TransferOrProductionItemId = table.Column<Guid>(nullable: true),
                    JournalId = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    OrderIndex = table.Column<string>(nullable: true),
                    JournalType = table.Column<int>(nullable: false),
                    JournalRef = table.Column<string>(nullable: true),
                    JournalNo = table.Column<string>(nullable: true),
                    CreationTimeIndex = table.Column<long>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    InventoryAccountId = table.Column<Guid>(nullable: false),
                    LocationId = table.Column<long>(nullable: false),
                    LotId = table.Column<long>(nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    LineCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    QtyOnHand = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    AvgCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    AdjustmentCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    IsItemReceipt = table.Column<bool>(nullable: false),
                    LastSyncTime = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpInventoryTransactionItems", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryTransactionItems_Date",
                table: "CarlErpInventoryTransactionItems",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryTransactionItems_InventoryAccountId",
                table: "CarlErpInventoryTransactionItems",
                column: "InventoryAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryTransactionItems_IsItemReceipt",
                table: "CarlErpInventoryTransactionItems",
                column: "IsItemReceipt");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryTransactionItems_ItemId",
                table: "CarlErpInventoryTransactionItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryTransactionItems_JournalId",
                table: "CarlErpInventoryTransactionItems",
                column: "JournalId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryTransactionItems_JournalType",
                table: "CarlErpInventoryTransactionItems",
                column: "JournalType");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryTransactionItems_LocationId",
                table: "CarlErpInventoryTransactionItems",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryTransactionItems_LotId",
                table: "CarlErpInventoryTransactionItems",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryTransactionItems_OrderIndex",
                table: "CarlErpInventoryTransactionItems",
                column: "OrderIndex");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryTransactionItems_TransactionId",
                table: "CarlErpInventoryTransactionItems",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryTransactionItems_TransferOrProductionId",
                table: "CarlErpInventoryTransactionItems",
                column: "TransferOrProductionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryTransactionItems_TransferOrProductionItemId",
                table: "CarlErpInventoryTransactionItems",
                column: "TransferOrProductionItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpInventoryTransactionItems");
        }
    }
}
