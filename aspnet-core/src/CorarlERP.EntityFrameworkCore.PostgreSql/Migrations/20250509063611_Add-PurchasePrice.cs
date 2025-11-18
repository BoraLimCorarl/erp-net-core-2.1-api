using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddPurchasePrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductionProcess",
                table: "CarlErpProductionPlans",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CarlErpPurchasePrices",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    LocationId = table.Column<long>(nullable: true),
                    SpecificVendor = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPurchasePrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPurchasePrices_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPurchasePriceItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    CurrencyId = table.Column<long>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    VendorId = table.Column<Guid>(nullable: true),
                    Price = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    PurchasePriceId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPurchasePriceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPurchasePriceItems_CarlErpCurrencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPurchasePriceItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPurchasePriceItems_CarlErpPurchasePrices_PurchasePri~",
                        column: x => x.PurchasePriceId,
                        principalTable: "CarlErpPurchasePrices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPurchasePriceItems_CarlErpVendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "CarlErpVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchasePriceItems_CurrencyId",
                table: "CarlErpPurchasePriceItems",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchasePriceItems_ItemId",
                table: "CarlErpPurchasePriceItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchasePriceItems_VendorId",
                table: "CarlErpPurchasePriceItems",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchasePriceItems_PurchasePriceId_ItemId_VendorId_C~",
                table: "CarlErpPurchasePriceItems",
                columns: new[] { "PurchasePriceId", "ItemId", "VendorId", "CurrencyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchasePrices_LocationId",
                table: "CarlErpPurchasePrices",
                column: "LocationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpPurchasePriceItems");

            migrationBuilder.DropTable(
                name: "CarlErpPurchasePrices");

            migrationBuilder.DropColumn(
                name: "ProductionProcess",
                table: "CarlErpProductionPlans");
        }
    }
}
