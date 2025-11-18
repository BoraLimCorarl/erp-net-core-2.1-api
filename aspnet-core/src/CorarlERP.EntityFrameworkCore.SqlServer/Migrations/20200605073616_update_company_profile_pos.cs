using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class update_company_profile_pos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPOS",
                table: "CarlErpInvoices");

            migrationBuilder.AddColumn<bool>(
                name: "IsPOS",
                table: "CarlErpTransactionTypes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "POSCurrencyId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_POSCurrencyId",
                table: "AbpTenants",
                column: "POSCurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpCurrencies_POSCurrencyId",
                table: "AbpTenants",
                column: "POSCurrencyId",
                principalTable: "CarlErpCurrencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpCurrencies_POSCurrencyId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_POSCurrencyId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "IsPOS",
                table: "CarlErpTransactionTypes");

            migrationBuilder.DropColumn(
                name: "POSCurrencyId",
                table: "AbpTenants");

            migrationBuilder.AddColumn<bool>(
                name: "IsPOS",
                table: "CarlErpInvoices",
                nullable: false,
                defaultValue: false);
        }
    }
}
