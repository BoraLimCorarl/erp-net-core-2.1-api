using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class updatecompanyProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ExchangeLossAndGainId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ExchangeLossAndGainId",
                table: "AbpTenants",
                column: "ExchangeLossAndGainId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ExchangeLossAndGainId",
                table: "AbpTenants",
                column: "ExchangeLossAndGainId",
                principalTable: "CarlErpChartOfAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpChartOfAccounts_ExchangeLossAndGainId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_ExchangeLossAndGainId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "ExchangeLossAndGainId",
                table: "AbpTenants");
        }
    }
}
