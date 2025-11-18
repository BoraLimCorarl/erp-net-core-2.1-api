using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class Add_Table_InventoryCostCloseItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpInventoryCostCloses_CarlErpLots_LotId",
                table: "CarlErpInventoryCostCloses");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpInventoryCostCloses_LotId",
                table: "CarlErpInventoryCostCloses");

            migrationBuilder.DropColumn(
                name: "LotId",
                table: "CarlErpInventoryCostCloses");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCost",
                table: "CarlErpInventoryCostCloses",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "CarlErpInventoryCostCloseItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    LotId = table.Column<long>(nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    InventoryCostCloseId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpInventoryCostCloseItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpInventoryCostCloseItems_CarlErpInventoryCostCloses_InventoryCostCloseId",
                        column: x => x.InventoryCostCloseId,
                        principalTable: "CarlErpInventoryCostCloses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInventoryCostCloseItems_CarlErpLots_LotId",
                        column: x => x.LotId,
                        principalTable: "CarlErpLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCostCloseItems_InventoryCostCloseId",
                table: "CarlErpInventoryCostCloseItems",
                column: "InventoryCostCloseId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCostCloseItems_LotId",
                table: "CarlErpInventoryCostCloseItems",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCostCloseItems_TenantId_CreatorUserId",
                table: "CarlErpInventoryCostCloseItems",
                columns: new[] { "TenantId", "CreatorUserId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpInventoryCostCloseItems");

            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "CarlErpInventoryCostCloses");

            migrationBuilder.AddColumn<long>(
                name: "LotId",
                table: "CarlErpInventoryCostCloses",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCostCloses_LotId",
                table: "CarlErpInventoryCostCloses",
                column: "LotId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpInventoryCostCloses_CarlErpLots_LotId",
                table: "CarlErpInventoryCostCloses",
                column: "LotId",
                principalTable: "CarlErpLots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
