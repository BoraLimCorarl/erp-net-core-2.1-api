using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddNewTable_Deposit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarlErpDeposits",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ReceiveFromVendorId = table.Column<Guid>(nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpDeposits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpDeposits_CarlErpVendors_ReceiveFromVendorId",
                        column: x => x.ReceiveFromVendorId,
                        principalTable: "CarlErpVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpDepositItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    DepositId = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    Qty = table.Column<decimal>(nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpDepositItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpDepositItems_CarlErpChartOfAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpDepositItems_CarlErpDeposits_DepositId",
                        column: x => x.DepositId,
                        principalTable: "CarlErpDeposits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpDepositItems_AccountId",
                table: "CarlErpDepositItems",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpDepositItems_DepositId",
                table: "CarlErpDepositItems",
                column: "DepositId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpDepositItems_TenantId_CreatorUserId",
                table: "CarlErpDepositItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpDeposits_ReceiveFromVendorId",
                table: "CarlErpDeposits",
                column: "ReceiveFromVendorId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpDeposits_TenantId_CreatorUserId",
                table: "CarlErpDeposits",
                columns: new[] { "TenantId", "CreatorUserId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpDepositItems");

            migrationBuilder.DropTable(
                name: "CarlErpDeposits");
        }
    }
}
