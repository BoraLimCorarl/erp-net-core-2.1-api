using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class adddefaulttaxinventoryreporttemplateontenant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "CarlErpPropertyValues",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequired",
                table: "CarlErpProperties",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsStatic",
                table: "CarlErpProperties",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DefaultInventoryReportTemplate",
                table: "AbpTenants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "TaxId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_TaxId",
                table: "AbpTenants",
                column: "TaxId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpTaxes_TaxId",
                table: "AbpTenants",
                column: "TaxId",
                principalTable: "CarlErpTaxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpTaxes_TaxId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_TaxId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "CarlErpPropertyValues");

            migrationBuilder.DropColumn(
                name: "IsRequired",
                table: "CarlErpProperties");

            migrationBuilder.DropColumn(
                name: "IsStatic",
                table: "CarlErpProperties");

            migrationBuilder.DropColumn(
                name: "DefaultInventoryReportTemplate",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "TaxId",
                table: "AbpTenants");
        }
    }
}
