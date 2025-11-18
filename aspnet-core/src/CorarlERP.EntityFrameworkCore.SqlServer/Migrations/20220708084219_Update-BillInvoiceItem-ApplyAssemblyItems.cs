using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateBillInvoiceItemApplyAssemblyItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowSubItems",
                table: "CarlErpItems",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "CarlErpInvoiceItems",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "CarlErpBillItems",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CarlErpItemLots",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    LotId = table.Column<long>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemLots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemLots_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemLots_CarlErpLots_LotId",
                        column: x => x.LotId,
                        principalTable: "CarlErpLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoiceItems_TenantId_ParentId",
                table: "CarlErpInvoiceItems",
                columns: new[] { "TenantId", "ParentId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBillItems_TenantId_ParentId",
                table: "CarlErpBillItems",
                columns: new[] { "TenantId", "ParentId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemLots_ItemId",
                table: "CarlErpItemLots",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemLots_LotId",
                table: "CarlErpItemLots",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemLots_TenantId_ItemId_LotId",
                table: "CarlErpItemLots",
                columns: new[] { "TenantId", "ItemId", "LotId" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpItemLots");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpInvoiceItems_TenantId_ParentId",
                table: "CarlErpInvoiceItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpBillItems_TenantId_ParentId",
                table: "CarlErpBillItems");

            migrationBuilder.DropColumn(
                name: "ShowSubItems",
                table: "CarlErpItems");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "CarlErpInvoiceItems");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "CarlErpBillItems");
        }
    }
}
