using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class Add_Field_VendorType_InVendorTable_and_VendorAccount_CustomerAccount_On_Table_Tenant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VendorType",
                table: "CarlErpVendors",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                @"
                    UPDATE CarlErpVendors
                    SET VendorType = 1;
                ");


            migrationBuilder.AddColumn<Guid>(
                name: "CustomerAccountId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VendorAccountId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_CustomerAccountId",
                table: "AbpTenants",
                column: "CustomerAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_VendorAccountId",
                table: "AbpTenants",
                column: "VendorAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_CustomerAccountId",
                table: "AbpTenants",
                column: "CustomerAccountId",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_VendorAccountId",
                table: "AbpTenants",
                column: "VendorAccountId",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_CustomerAccountId",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_VendorAccountId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_CustomerAccountId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_VendorAccountId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "VendorType",
                table: "CarlErpVendors");

            migrationBuilder.DropColumn(
                name: "CustomerAccountId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "VendorAccountId",
                table: "AbpTenants");
        }
    }
}
