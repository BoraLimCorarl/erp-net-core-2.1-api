using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class BillInvoiceSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarlErpBillInvoiceSettings",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    SettingType = table.Column<int>(nullable: false),
                    ReferenceSameAsGoodsMovement = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpBillInvoiceSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBillInvoiceSettings_SettingType",
                table: "CarlErpBillInvoiceSettings",
                column: "SettingType");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBillInvoiceSettings_TenantId_CreatorUserId_SettingType",
                table: "CarlErpBillInvoiceSettings",
                columns: new[] { "TenantId", "CreatorUserId", "SettingType" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpBillInvoiceSettings");
        }
    }
}
