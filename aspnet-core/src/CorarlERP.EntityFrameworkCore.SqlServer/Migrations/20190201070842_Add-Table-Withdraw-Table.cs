using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddTableWithdrawTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WithdrawId",
                table: "CarlErpJournals",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CarlErpWithdraws",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    VendorId = table.Column<Guid>(nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpWithdraws", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpWithdraws_CarlErpVendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "CarlErpVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpWithdrawItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    WithdrawId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpWithdrawItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpWithdrawItems_CarlErpWithdraws_WithdrawId",
                        column: x => x.WithdrawId,
                        principalTable: "CarlErpWithdraws",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_WithdrawId",
                table: "CarlErpJournals",
                column: "WithdrawId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpWithdrawItems_WithdrawId",
                table: "CarlErpWithdrawItems",
                column: "WithdrawId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpWithdrawItems_TenantId_CreatorUserId",
                table: "CarlErpWithdrawItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpWithdraws_VendorId",
                table: "CarlErpWithdraws",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpWithdraws_TenantId_CreatorUserId",
                table: "CarlErpWithdraws",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpJournals_CarlErpWithdraws_WithdrawId",
                table: "CarlErpJournals",
                column: "WithdrawId",
                principalTable: "CarlErpWithdraws",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpJournals_CarlErpWithdraws_WithdrawId",
                table: "CarlErpJournals");

            migrationBuilder.DropTable(
                name: "CarlErpWithdrawItems");

            migrationBuilder.DropTable(
                name: "CarlErpWithdraws");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpJournals_WithdrawId",
                table: "CarlErpJournals");

            migrationBuilder.DropColumn(
                name: "WithdrawId",
                table: "CarlErpJournals");
        }
    }
}
