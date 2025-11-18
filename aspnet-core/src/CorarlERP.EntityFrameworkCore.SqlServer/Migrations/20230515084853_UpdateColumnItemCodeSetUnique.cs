using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateColumnItemCodeSetUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CarlErpVendors_TenantId_CreatorUserId_VendorName_VendorCode",
                table: "CarlErpVendors");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpTransactionTypes_TenantId_CreatorUserId_TransactionTypeName",
                table: "CarlErpTransactionTypes");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpLots_TenantId_CreatorUserId_LotName",
                table: "CarlErpLots");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpLocations_TenantId_CreatorUserId_LocationName",
                table: "CarlErpLocations");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpCustomers_TenantId_CreatorUserId_CustomerName_CustomerCode",
                table: "CarlErpCustomers");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpClasses_TenantId_CreatorUserId_ClassName",
                table: "CarlErpClasses");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendors_TenantId_VendorCode",
                table: "CarlErpVendors",
                columns: new[] { "TenantId", "VendorCode" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendors_TenantId_VendorName",
                table: "CarlErpVendors",
                columns: new[] { "TenantId", "VendorName" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransactionTypes_TenantId_TransactionTypeName",
                table: "CarlErpTransactionTypes",
                columns: new[] { "TenantId", "TransactionTypeName" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTaxes_TenantId_TaxName",
                table: "CarlErpTaxes",
                columns: new[] { "TenantId", "TaxName" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpLots_TenantId_LotName",
                table: "CarlErpLots",
                columns: new[] { "TenantId", "LotName" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpLocations_TenantId_LocationName",
                table: "CarlErpLocations",
                columns: new[] { "TenantId", "LocationName" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_TenantId_ItemCode",
                table: "CarlErpItems",
                columns: new[] { "TenantId", "ItemCode" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_TenantId_ItemName",
                table: "CarlErpItems",
                columns: new[] { "TenantId", "ItemName" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomers_TenantId_CustomerCode",
                table: "CarlErpCustomers",
                columns: new[] { "TenantId", "CustomerCode" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomers_TenantId_CustomerName",
                table: "CarlErpCustomers",
                columns: new[] { "TenantId", "CustomerName" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpClasses_TenantId_ClassName",
                table: "CarlErpClasses",
                columns: new[] { "TenantId", "ClassName" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpChartOfAccounts_TenantId_AccountCode",
                table: "CarlErpChartOfAccounts",
                columns: new[] { "TenantId", "AccountCode" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpChartOfAccounts_TenantId_AccountName",
                table: "CarlErpChartOfAccounts",
                columns: new[] { "TenantId", "AccountName" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CarlErpVendors_TenantId_VendorCode",
                table: "CarlErpVendors");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpVendors_TenantId_VendorName",
                table: "CarlErpVendors");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpTransactionTypes_TenantId_TransactionTypeName",
                table: "CarlErpTransactionTypes");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpTaxes_TenantId_TaxName",
                table: "CarlErpTaxes");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpLots_TenantId_LotName",
                table: "CarlErpLots");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpLocations_TenantId_LocationName",
                table: "CarlErpLocations");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItems_TenantId_ItemCode",
                table: "CarlErpItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItems_TenantId_ItemName",
                table: "CarlErpItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpCustomers_TenantId_CustomerCode",
                table: "CarlErpCustomers");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpCustomers_TenantId_CustomerName",
                table: "CarlErpCustomers");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpClasses_TenantId_ClassName",
                table: "CarlErpClasses");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpChartOfAccounts_TenantId_AccountCode",
                table: "CarlErpChartOfAccounts");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpChartOfAccounts_TenantId_AccountName",
                table: "CarlErpChartOfAccounts");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendors_TenantId_CreatorUserId_VendorName_VendorCode",
                table: "CarlErpVendors",
                columns: new[] { "TenantId", "CreatorUserId", "VendorName", "VendorCode" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransactionTypes_TenantId_CreatorUserId_TransactionTypeName",
                table: "CarlErpTransactionTypes",
                columns: new[] { "TenantId", "CreatorUserId", "TransactionTypeName" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpLots_TenantId_CreatorUserId_LotName",
                table: "CarlErpLots",
                columns: new[] { "TenantId", "CreatorUserId", "LotName" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpLocations_TenantId_CreatorUserId_LocationName",
                table: "CarlErpLocations",
                columns: new[] { "TenantId", "CreatorUserId", "LocationName" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomers_TenantId_CreatorUserId_CustomerName_CustomerCode",
                table: "CarlErpCustomers",
                columns: new[] { "TenantId", "CreatorUserId", "CustomerName", "CustomerCode" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpClasses_TenantId_CreatorUserId_ClassName",
                table: "CarlErpClasses",
                columns: new[] { "TenantId", "CreatorUserId", "ClassName" });
        }
    }
}
