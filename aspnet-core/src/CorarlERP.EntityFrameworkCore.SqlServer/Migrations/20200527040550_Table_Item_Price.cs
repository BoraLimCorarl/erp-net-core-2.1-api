using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class Table_Item_Price : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarlErpItemPrices",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    TransactionTypeSaleId = table.Column<long>(nullable: true),
                    LocationId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemPrices_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemPrices_CarlErpTransactionTypes_TransactionTypeSaleId",
                        column: x => x.TransactionTypeSaleId,
                        principalTable: "CarlErpTransactionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemPriceItems",
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
                    Price = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    ItemPriceId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemPriceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemPriceItems_CarlErpCurrencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemPriceItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemPriceItems_CarlErpItemPrices_ItemPriceId",
                        column: x => x.ItemPriceId,
                        principalTable: "CarlErpItemPrices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemPriceItems_CurrencyId",
                table: "CarlErpItemPriceItems",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemPriceItems_ItemId",
                table: "CarlErpItemPriceItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemPriceItems_ItemPriceId",
                table: "CarlErpItemPriceItems",
                column: "ItemPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemPriceItems_TenantId_CreatorUserId",
                table: "CarlErpItemPriceItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemPrices_LocationId",
                table: "CarlErpItemPrices",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemPrices_TransactionTypeSaleId",
                table: "CarlErpItemPrices",
                column: "TransactionTypeSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemPrices_TenantId_CreatorUserId",
                table: "CarlErpItemPrices",
                columns: new[] { "TenantId", "CreatorUserId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpItemPriceItems");

            migrationBuilder.DropTable(
                name: "CarlErpItemPrices");
        }
    }
}
