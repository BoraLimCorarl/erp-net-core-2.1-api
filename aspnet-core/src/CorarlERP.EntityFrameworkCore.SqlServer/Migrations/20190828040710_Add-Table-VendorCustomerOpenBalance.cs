using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddTableVendorCustomerOpenBalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarlErpVendorCustomerOpenBalaces",
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
                    AccountCycleId = table.Column<long>(nullable: false)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpVendorCustomerOpenBalaces");
        }
    }
}
