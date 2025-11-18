using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateRelationShip_PhysicalCount_CompanyProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemIssuePhysicalCountId1",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemRecieptPhysicalCountId1",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_ItemIssuePhysicalCountId1",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_ItemRecieptPhysicalCountId1",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "ItemIssuePhysicalCountId1",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "ItemRecieptPhysicalCountId1",
                table: "AbpTenants");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ItemIssuePhysicalCountId",
                table: "AbpTenants",
                column: "ItemIssuePhysicalCountId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ItemRecieptPhysicalCountId",
                table: "AbpTenants",
                column: "ItemRecieptPhysicalCountId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemIssuePhysicalCountId",
                table: "AbpTenants",
                column: "ItemIssuePhysicalCountId",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemRecieptPhysicalCountId",
                table: "AbpTenants",
                column: "ItemRecieptPhysicalCountId",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemIssuePhysicalCountId",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemRecieptPhysicalCountId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_ItemIssuePhysicalCountId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_ItemRecieptPhysicalCountId",
                table: "AbpTenants");

            migrationBuilder.AddColumn<Guid>(
                name: "ItemIssuePhysicalCountId1",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ItemRecieptPhysicalCountId1",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ItemIssuePhysicalCountId1",
                table: "AbpTenants",
                column: "ItemIssuePhysicalCountId1");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ItemRecieptPhysicalCountId1",
                table: "AbpTenants",
                column: "ItemRecieptPhysicalCountId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemIssuePhysicalCountId1",
                table: "AbpTenants",
                column: "ItemIssuePhysicalCountId1",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemRecieptPhysicalCountId1",
                table: "AbpTenants",
                column: "ItemRecieptPhysicalCountId1",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
