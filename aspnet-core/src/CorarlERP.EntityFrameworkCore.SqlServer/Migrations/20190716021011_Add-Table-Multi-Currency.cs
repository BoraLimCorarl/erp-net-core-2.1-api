using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddTableMultiCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "CreatorUserId",
                table: "CarlErpTransferOrders",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CreatorUserId",
                table: "CarlErpBankTransfers",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CarlErpMultiCurrencies",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    CurrencyId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpMultiCurrencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpMultiCurrencies_CarlErpCurrencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransProductions_CreatorUserId",
                table: "CarlErpTransProductions",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransProductions_LastModifierUserId",
                table: "CarlErpTransProductions",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrders_CreatorUserId",
                table: "CarlErpTransferOrders",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrders_LastModifierUserId",
                table: "CarlErpTransferOrders",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrders_CreatorUserId",
                table: "CarlErpSaleOrders",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrders_LastModifierUserId",
                table: "CarlErpSaleOrders",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrders_CreatorUserId",
                table: "CarlErpPurchaseOrders",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrders_LastModifierUserId",
                table: "CarlErpPurchaseOrders",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBankTransfers_CreatorUserId",
                table: "CarlErpBankTransfers",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBankTransfers_LastModifierUserId",
                table: "CarlErpBankTransfers",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpMultiCurrencies_CreatorUserId",
                table: "CarlErpMultiCurrencies",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpMultiCurrencies_CurrencyId",
                table: "CarlErpMultiCurrencies",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpBankTransfers_AbpUsers_CreatorUserId",
                table: "CarlErpBankTransfers",
                column: "CreatorUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpBankTransfers_AbpUsers_LastModifierUserId",
                table: "CarlErpBankTransfers",
                column: "LastModifierUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpPurchaseOrders_AbpUsers_CreatorUserId",
                table: "CarlErpPurchaseOrders",
                column: "CreatorUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpPurchaseOrders_AbpUsers_LastModifierUserId",
                table: "CarlErpPurchaseOrders",
                column: "LastModifierUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpSaleOrders_AbpUsers_CreatorUserId",
                table: "CarlErpSaleOrders",
                column: "CreatorUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpSaleOrders_AbpUsers_LastModifierUserId",
                table: "CarlErpSaleOrders",
                column: "LastModifierUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpTransferOrders_AbpUsers_CreatorUserId",
                table: "CarlErpTransferOrders",
                column: "CreatorUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpTransferOrders_AbpUsers_LastModifierUserId",
                table: "CarlErpTransferOrders",
                column: "LastModifierUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpTransProductions_AbpUsers_CreatorUserId",
                table: "CarlErpTransProductions",
                column: "CreatorUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpTransProductions_AbpUsers_LastModifierUserId",
                table: "CarlErpTransProductions",
                column: "LastModifierUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpBankTransfers_AbpUsers_CreatorUserId",
                table: "CarlErpBankTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpBankTransfers_AbpUsers_LastModifierUserId",
                table: "CarlErpBankTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpPurchaseOrders_AbpUsers_CreatorUserId",
                table: "CarlErpPurchaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpPurchaseOrders_AbpUsers_LastModifierUserId",
                table: "CarlErpPurchaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpSaleOrders_AbpUsers_CreatorUserId",
                table: "CarlErpSaleOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpSaleOrders_AbpUsers_LastModifierUserId",
                table: "CarlErpSaleOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpTransferOrders_AbpUsers_CreatorUserId",
                table: "CarlErpTransferOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpTransferOrders_AbpUsers_LastModifierUserId",
                table: "CarlErpTransferOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpTransProductions_AbpUsers_CreatorUserId",
                table: "CarlErpTransProductions");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpTransProductions_AbpUsers_LastModifierUserId",
                table: "CarlErpTransProductions");

            migrationBuilder.DropTable(
                name: "CarlErpMultiCurrencies");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpTransProductions_CreatorUserId",
                table: "CarlErpTransProductions");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpTransProductions_LastModifierUserId",
                table: "CarlErpTransProductions");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpTransferOrders_CreatorUserId",
                table: "CarlErpTransferOrders");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpTransferOrders_LastModifierUserId",
                table: "CarlErpTransferOrders");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpSaleOrders_CreatorUserId",
                table: "CarlErpSaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpSaleOrders_LastModifierUserId",
                table: "CarlErpSaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpPurchaseOrders_CreatorUserId",
                table: "CarlErpPurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpPurchaseOrders_LastModifierUserId",
                table: "CarlErpPurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpBankTransfers_CreatorUserId",
                table: "CarlErpBankTransfers");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpBankTransfers_LastModifierUserId",
                table: "CarlErpBankTransfers");

            migrationBuilder.AlterColumn<long>(
                name: "CreatorUserId",
                table: "CarlErpTransferOrders",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "CreatorUserId",
                table: "CarlErpBankTransfers",
                nullable: true,
                oldClrType: typeof(long));
        }
    }
}
