using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class CreateTableInventoryCostClose : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarlErpInventoryCostCloses",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    LocationId = table.Column<long>(nullable: false),
                    AccountCycleId = table.Column<long>(nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    QtyOnhand = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpInventoryCostCloses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpInventoryCostCloses_CarlErpAccountCycles_AccountCycleId",
                        column: x => x.AccountCycleId,
                        principalTable: "CarlErpAccountCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInventoryCostCloses_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInventoryCostCloses_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCostCloses_AccountCycleId",
                table: "CarlErpInventoryCostCloses",
                column: "AccountCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCostCloses_ItemId",
                table: "CarlErpInventoryCostCloses",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCostCloses_LocationId",
                table: "CarlErpInventoryCostCloses",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCostCloses_TenantId_CreatorUserId",
                table: "CarlErpInventoryCostCloses",
                columns: new[] { "TenantId", "CreatorUserId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpInventoryCostCloses");
        }
    }
}
