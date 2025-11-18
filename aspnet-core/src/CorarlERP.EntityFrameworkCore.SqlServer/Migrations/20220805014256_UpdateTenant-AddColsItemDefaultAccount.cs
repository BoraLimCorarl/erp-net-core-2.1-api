using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateTenantAddColsItemDefaultAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "COGSAccountId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ExpenseAccountId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InventoryAccountId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RevenueAccountId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_COGSAccountId",
                table: "AbpTenants",
                column: "COGSAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ExpenseAccountId",
                table: "AbpTenants",
                column: "ExpenseAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_InventoryAccountId",
                table: "AbpTenants",
                column: "InventoryAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_RevenueAccountId",
                table: "AbpTenants",
                column: "RevenueAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_COGSAccountId",
                table: "AbpTenants",
                column: "COGSAccountId",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ExpenseAccountId",
                table: "AbpTenants",
                column: "ExpenseAccountId",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_InventoryAccountId",
                table: "AbpTenants",
                column: "InventoryAccountId",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_RevenueAccountId",
                table: "AbpTenants",
                column: "RevenueAccountId",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_COGSAccountId",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ExpenseAccountId",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_InventoryAccountId",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_RevenueAccountId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_COGSAccountId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_ExpenseAccountId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_InventoryAccountId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_RevenueAccountId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "COGSAccountId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "ExpenseAccountId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "InventoryAccountId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "RevenueAccountId",
                table: "AbpTenants");
        }
    }
}
