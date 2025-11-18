using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class update_tenant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AccountCycleId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BillPaymentAccountId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessId",
                table: "AbpTenants",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ClassId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CurrencyId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "AbpTenants",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FormatDateId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FormatNumberId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ItemIssueAdjustmentId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ItemIssueOtherId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ItemIssueTransferId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ItemIssueVendorCreditId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ItemRecieptAdjustmentId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ItemRecieptCustomerCreditId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ItemRecieptOtherId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ItemRecieptTransferId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalName",
                table: "AbpTenants",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "AbpTenants",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SaleAllowanceAccountId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SameAsCompanyAddress",
                table: "AbpTenants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TransitAccountId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "AbpTenants",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyAddress_CityTown",
                table: "AbpTenants",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyAddress_Country",
                table: "AbpTenants",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyAddress_PostalCode",
                table: "AbpTenants",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyAddress_Province",
                table: "AbpTenants",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyAddress_Street",
                table: "AbpTenants",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalAddress_CityTown",
                table: "AbpTenants",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalAddress_Country",
                table: "AbpTenants",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalAddress_PostalCode",
                table: "AbpTenants",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalAddress_Province",
                table: "AbpTenants",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalAddress_Street",
                table: "AbpTenants",
                maxLength: 512,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_AccountCycleId",
                table: "AbpTenants",
                column: "AccountCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_BillPaymentAccountId",
                table: "AbpTenants",
                column: "BillPaymentAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ClassId",
                table: "AbpTenants",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_CurrencyId",
                table: "AbpTenants",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_FormatDateId",
                table: "AbpTenants",
                column: "FormatDateId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_FormatNumberId",
                table: "AbpTenants",
                column: "FormatNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ItemIssueAdjustmentId",
                table: "AbpTenants",
                column: "ItemIssueAdjustmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ItemIssueOtherId",
                table: "AbpTenants",
                column: "ItemIssueOtherId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ItemIssueTransferId",
                table: "AbpTenants",
                column: "ItemIssueTransferId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ItemIssueVendorCreditId",
                table: "AbpTenants",
                column: "ItemIssueVendorCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ItemRecieptAdjustmentId",
                table: "AbpTenants",
                column: "ItemRecieptAdjustmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ItemRecieptCustomerCreditId",
                table: "AbpTenants",
                column: "ItemRecieptCustomerCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ItemRecieptOtherId",
                table: "AbpTenants",
                column: "ItemRecieptOtherId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ItemRecieptTransferId",
                table: "AbpTenants",
                column: "ItemRecieptTransferId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_LocationId",
                table: "AbpTenants",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_SaleAllowanceAccountId",
                table: "AbpTenants",
                column: "SaleAllowanceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_TransitAccountId",
                table: "AbpTenants",
                column: "TransitAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpAccountCycles_AccountCycleId",
                table: "AbpTenants",
                column: "AccountCycleId",
                principalTable: "CarlErpAccountCycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_BillPaymentAccountId",
                table: "AbpTenants",
                column: "BillPaymentAccountId",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpClasses_ClassId",
                table: "AbpTenants",
                column: "ClassId",
                principalTable: "CarlErpClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpCurrencies_CurrencyId",
                table: "AbpTenants",
                column: "CurrencyId",
                principalTable: "CarlErpCurrencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpFormats_FormatDateId",
                table: "AbpTenants",
                column: "FormatDateId",
                principalTable: "CarlErpFormats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpFormats_FormatNumberId",
                table: "AbpTenants",
                column: "FormatNumberId",
                principalTable: "CarlErpFormats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemIssueAdjustmentId",
                table: "AbpTenants",
                column: "ItemIssueAdjustmentId",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemIssueOtherId",
                table: "AbpTenants",
                column: "ItemIssueOtherId",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemIssueTransferId",
                table: "AbpTenants",
                column: "ItemIssueTransferId",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemIssueVendorCreditId",
                table: "AbpTenants",
                column: "ItemIssueVendorCreditId",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemRecieptAdjustmentId",
                table: "AbpTenants",
                column: "ItemRecieptAdjustmentId",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemRecieptCustomerCreditId",
                table: "AbpTenants",
                column: "ItemRecieptCustomerCreditId",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemRecieptOtherId",
                table: "AbpTenants",
                column: "ItemRecieptOtherId",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemRecieptTransferId",
                table: "AbpTenants",
                column: "ItemRecieptTransferId",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpLocations_LocationId",
                table: "AbpTenants",
                column: "LocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_SaleAllowanceAccountId",
                table: "AbpTenants",
                column: "SaleAllowanceAccountId",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_TransitAccountId",
                table: "AbpTenants",
                column: "TransitAccountId",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpAccountCycles_AccountCycleId",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_BillPaymentAccountId",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpClasses_ClassId",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpCurrencies_CurrencyId",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpFormats_FormatDateId",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpFormats_FormatNumberId",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemIssueAdjustmentId",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemIssueOtherId",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemIssueTransferId",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemIssueVendorCreditId",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemRecieptAdjustmentId",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemRecieptCustomerCreditId",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemRecieptOtherId",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemRecieptTransferId",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpLocations_LocationId",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_SaleAllowanceAccountId",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_TransitAccountId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_AccountCycleId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_BillPaymentAccountId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_ClassId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_CurrencyId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_FormatDateId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_FormatNumberId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_ItemIssueAdjustmentId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_ItemIssueOtherId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_ItemIssueTransferId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_ItemIssueVendorCreditId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_ItemRecieptAdjustmentId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_ItemRecieptCustomerCreditId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_ItemRecieptOtherId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_ItemRecieptTransferId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_LocationId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_SaleAllowanceAccountId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_TransitAccountId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "AccountCycleId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "BillPaymentAccountId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "BusinessId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "FormatDateId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "FormatNumberId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "ItemIssueAdjustmentId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "ItemIssueOtherId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "ItemIssueTransferId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "ItemIssueVendorCreditId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "ItemRecieptAdjustmentId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "ItemRecieptCustomerCreditId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "ItemRecieptOtherId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "ItemRecieptTransferId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "LegalName",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "SaleAllowanceAccountId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "SameAsCompanyAddress",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "TransitAccountId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "CompanyAddress_CityTown",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "CompanyAddress_Country",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "CompanyAddress_PostalCode",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "CompanyAddress_Province",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "CompanyAddress_Street",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "LegalAddress_CityTown",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "LegalAddress_Country",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "LegalAddress_PostalCode",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "LegalAddress_Province",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "LegalAddress_Street",
                table: "AbpTenants");
        }
    }
}
