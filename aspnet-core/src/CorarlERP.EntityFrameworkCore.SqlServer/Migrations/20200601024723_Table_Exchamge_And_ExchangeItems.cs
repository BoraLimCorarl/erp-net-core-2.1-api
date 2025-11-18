using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class Table_Exchamge_And_ExchangeItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarlErpExchanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    FromDate = table.Column<DateTime>(nullable: false),
                    ToDate = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpExchanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpExchangeItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    FromCurrencyId = table.Column<long>(nullable: false),
                    ToCurencyId = table.Column<long>(nullable: false),
                    Bid = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Ask = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    ExchangeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpExchangeItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpExchangeItems_CarlErpExchanges_ExchangeId",
                        column: x => x.ExchangeId,
                        principalTable: "CarlErpExchanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpExchangeItems_CarlErpCurrencies_FromCurrencyId",
                        column: x => x.FromCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpExchangeItems_CarlErpCurrencies_ToCurencyId",
                        column: x => x.ToCurencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpExchangeItems_ExchangeId",
                table: "CarlErpExchangeItems",
                column: "ExchangeId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpExchangeItems_FromCurrencyId",
                table: "CarlErpExchangeItems",
                column: "FromCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpExchangeItems_ToCurencyId",
                table: "CarlErpExchangeItems",
                column: "ToCurencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpExchangeItems_TenantId_CreatorUserId",
                table: "CarlErpExchangeItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpExchanges_TenantId_CreatorUserId",
                table: "CarlErpExchanges",
                columns: new[] { "TenantId", "CreatorUserId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpExchangeItems");

            migrationBuilder.DropTable(
                name: "CarlErpExchanges");
        }
    }
}
