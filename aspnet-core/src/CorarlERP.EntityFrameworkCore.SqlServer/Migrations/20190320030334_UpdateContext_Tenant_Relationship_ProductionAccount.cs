using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateContext_Tenant_Relationship_ProductionAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_FinishProductionAccountId1",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_RawProductionAccountId1",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_FinishProductionAccountId1",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_RawProductionAccountId1",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "FinishProductionAccountId1",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "RawProductionAccountId1",
                table: "AbpTenants");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_FinishProductionAccountId",
                table: "AbpTenants",
                column: "FinishProductionAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_RawProductionAccountId",
                table: "AbpTenants",
                column: "RawProductionAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_FinishProductionAccountId",
                table: "AbpTenants",
                column: "FinishProductionAccountId",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_RawProductionAccountId",
                table: "AbpTenants",
                column: "RawProductionAccountId",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_FinishProductionAccountId",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_RawProductionAccountId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_FinishProductionAccountId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_RawProductionAccountId",
                table: "AbpTenants");

            migrationBuilder.AddColumn<Guid>(
                name: "FinishProductionAccountId1",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RawProductionAccountId1",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_FinishProductionAccountId1",
                table: "AbpTenants",
                column: "FinishProductionAccountId1");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_RawProductionAccountId1",
                table: "AbpTenants",
                column: "RawProductionAccountId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_FinishProductionAccountId1",
                table: "AbpTenants",
                column: "FinishProductionAccountId1",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_RawProductionAccountId1",
                table: "AbpTenants",
                column: "RawProductionAccountId1",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
