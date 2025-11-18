using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddNewField_PhysicalCount_CompanyProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ItemIssuePhysicalCountId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ItemIssuePhysicalCountId1",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ItemRecieptPhysicalCountId",
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "ItemIssuePhysicalCountId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "ItemIssuePhysicalCountId1",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "ItemRecieptPhysicalCountId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "ItemRecieptPhysicalCountId1",
                table: "AbpTenants");
        }
    }
}
