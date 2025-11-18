using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddNewField_BankTransferId_InTenant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BankTransferAccountId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_BankTransferAccountId",
                table: "AbpTenants",
                column: "BankTransferAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_BankTransferAccountId",
                table: "AbpTenants",
                column: "BankTransferAccountId",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_BankTransferAccountId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_BankTransferAccountId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "BankTransferAccountId",
                table: "AbpTenants");
        }
    }
}
