using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateCloseReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpVendorCustomerOpenBalaces");

            migrationBuilder.CreateTable(
                name: "CarlErpCustomerOpenBalances",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    TransactionId = table.Column<Guid>(nullable: false),
                    Key = table.Column<int>(nullable: false),
                    AccountCycleId = table.Column<long>(nullable: false),
                    Balance = table.Column<decimal>(nullable: false),
                    MuliCurrencyBalance = table.Column<decimal>(nullable: false),
                    LocationId = table.Column<long>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCustomerOpenBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomerOpenBalances_CarlErpAccountCycles_AccountCycleId",
                        column: x => x.AccountCycleId,
                        principalTable: "CarlErpAccountCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpVendorOpenBalances",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    TransactionId = table.Column<Guid>(nullable: false),
                    Key = table.Column<int>(nullable: false),
                    AccountCycleId = table.Column<long>(nullable: false),
                    Balance = table.Column<decimal>(nullable: false),
                    MuliCurrencyBalance = table.Column<decimal>(nullable: false),
                    LocationId = table.Column<long>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpVendorOpenBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorOpenBalances_CarlErpAccountCycles_AccountCycleId",
                        column: x => x.AccountCycleId,
                        principalTable: "CarlErpAccountCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerOpenBalances_AccountCycleId",
                table: "CarlErpCustomerOpenBalances",
                column: "AccountCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerOpenBalances_Key",
                table: "CarlErpCustomerOpenBalances",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerOpenBalances_TransactionId",
                table: "CarlErpCustomerOpenBalances",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorOpenBalances_AccountCycleId",
                table: "CarlErpVendorOpenBalances",
                column: "AccountCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorOpenBalances_Key",
                table: "CarlErpVendorOpenBalances",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorOpenBalances_TransactionId",
                table: "CarlErpVendorOpenBalances",
                column: "TransactionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpCustomerOpenBalances");

            migrationBuilder.DropTable(
                name: "CarlErpVendorOpenBalances");

            migrationBuilder.CreateTable(
                name: "CarlErpVendorCustomerOpenBalaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountCycleId = table.Column<long>(nullable: false),
                    Balance = table.Column<decimal>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    Key = table.Column<int>(nullable: false),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    LocationId = table.Column<long>(nullable: false),
                    MuliCurrencyBalance = table.Column<decimal>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    TransactionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpVendorCustomerOpenBalaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorCustomerOpenBalaces_CarlErpAccountCycles_AccountCycleId",
                        column: x => x.AccountCycleId,
                        principalTable: "CarlErpAccountCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCustomerOpenBalaces_AccountCycleId",
                table: "CarlErpVendorCustomerOpenBalaces",
                column: "AccountCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCustomerOpenBalaces_Key",
                table: "CarlErpVendorCustomerOpenBalaces",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCustomerOpenBalaces_TenantId_CreatorUserId",
                table: "CarlErpVendorCustomerOpenBalaces",
                columns: new[] { "TenantId", "CreatorUserId" });
        }
    }
}
