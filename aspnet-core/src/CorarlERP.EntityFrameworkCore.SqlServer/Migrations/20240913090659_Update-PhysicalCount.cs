using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdatePhysicalCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QtyChange",
                table: "CarlErpPhysicalCountItems",
                newName: "UnitCost");

            migrationBuilder.AddColumn<Guid>(
                name: "BatchNoId",
                table: "CarlErpPhysicalCountItems",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CountQty",
                table: "CarlErpPhysicalCountItems",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CarlErpPhysicalCountItems",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiffQty",
                table: "CarlErpPhysicalCountItems",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "LotId",
                table: "CarlErpPhysicalCountItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "No",
                table: "CarlErpPhysicalCountItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "PhysicalCountItemId",
                table: "CarlErpItemReceiptItems",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PhysicalCountItemId",
                table: "CarlErpItemIssueItems",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CarlErpPhysicalCountItemFilters",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    PhysicalCountId = table.Column<Guid>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPhysicalCountItemFilters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPhysicalCountItemFilters_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPhysicalCountItemFilters_CarlErpPhysicalCounts_PhysicalCountId",
                        column: x => x.PhysicalCountId,
                        principalTable: "CarlErpPhysicalCounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPhysicalCountSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    No = table.Column<bool>(nullable: false),
                    ItemCode = table.Column<bool>(nullable: false),
                    LotName = table.Column<bool>(nullable: false),
                    DiffQty = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPhysicalCountSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPhysicalCountItems_BatchNoId",
                table: "CarlErpPhysicalCountItems",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPhysicalCountItems_LotId",
                table: "CarlErpPhysicalCountItems",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptItems_PhysicalCountItemId",
                table: "CarlErpItemReceiptItems",
                column: "PhysicalCountItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueItems_PhysicalCountItemId",
                table: "CarlErpItemIssueItems",
                column: "PhysicalCountItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPhysicalCountItemFilters_ItemId",
                table: "CarlErpPhysicalCountItemFilters",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPhysicalCountItemFilters_PhysicalCountId",
                table: "CarlErpPhysicalCountItemFilters",
                column: "PhysicalCountId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemIssueItems_CarlErpPhysicalCountItems_PhysicalCountItemId",
                table: "CarlErpItemIssueItems",
                column: "PhysicalCountItemId",
                principalTable: "CarlErpPhysicalCountItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemReceiptItems_CarlErpPhysicalCountItems_PhysicalCountItemId",
                table: "CarlErpItemReceiptItems",
                column: "PhysicalCountItemId",
                principalTable: "CarlErpPhysicalCountItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpPhysicalCountItems_CarlErpBatchNos_BatchNoId",
                table: "CarlErpPhysicalCountItems",
                column: "BatchNoId",
                principalTable: "CarlErpBatchNos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpPhysicalCountItems_CarlErpLots_LotId",
                table: "CarlErpPhysicalCountItems",
                column: "LotId",
                principalTable: "CarlErpLots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemIssueItems_CarlErpPhysicalCountItems_PhysicalCountItemId",
                table: "CarlErpItemIssueItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemReceiptItems_CarlErpPhysicalCountItems_PhysicalCountItemId",
                table: "CarlErpItemReceiptItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpPhysicalCountItems_CarlErpBatchNos_BatchNoId",
                table: "CarlErpPhysicalCountItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpPhysicalCountItems_CarlErpLots_LotId",
                table: "CarlErpPhysicalCountItems");

            migrationBuilder.DropTable(
                name: "CarlErpPhysicalCountItemFilters");

            migrationBuilder.DropTable(
                name: "CarlErpPhysicalCountSettings");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpPhysicalCountItems_BatchNoId",
                table: "CarlErpPhysicalCountItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpPhysicalCountItems_LotId",
                table: "CarlErpPhysicalCountItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemReceiptItems_PhysicalCountItemId",
                table: "CarlErpItemReceiptItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemIssueItems_PhysicalCountItemId",
                table: "CarlErpItemIssueItems");

            migrationBuilder.DropColumn(
                name: "BatchNoId",
                table: "CarlErpPhysicalCountItems");

            migrationBuilder.DropColumn(
                name: "CountQty",
                table: "CarlErpPhysicalCountItems");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CarlErpPhysicalCountItems");

            migrationBuilder.DropColumn(
                name: "DiffQty",
                table: "CarlErpPhysicalCountItems");

            migrationBuilder.DropColumn(
                name: "LotId",
                table: "CarlErpPhysicalCountItems");

            migrationBuilder.DropColumn(
                name: "No",
                table: "CarlErpPhysicalCountItems");

            migrationBuilder.DropColumn(
                name: "PhysicalCountItemId",
                table: "CarlErpItemReceiptItems");

            migrationBuilder.DropColumn(
                name: "PhysicalCountItemId",
                table: "CarlErpItemIssueItems");

            migrationBuilder.RenameColumn(
                name: "UnitCost",
                table: "CarlErpPhysicalCountItems",
                newName: "QtyChange");
        }
    }
}
