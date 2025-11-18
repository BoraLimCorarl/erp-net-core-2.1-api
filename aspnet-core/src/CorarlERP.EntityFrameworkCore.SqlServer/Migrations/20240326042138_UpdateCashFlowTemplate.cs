using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateCashFlowTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OutAccountGroupId",
                table: "CarlErpCashFlowTemplateAccounts",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "CarlErpCashFlowCategories",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCashFlowTemplateAccounts_OutAccountGroupId",
                table: "CarlErpCashFlowTemplateAccounts",
                column: "OutAccountGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCashFlowCategories_IsDefault",
                table: "CarlErpCashFlowCategories",
                column: "IsDefault");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpCashFlowTemplateAccounts_CarlErpCashFlowAccountGroups_OutAccountGroupId",
                table: "CarlErpCashFlowTemplateAccounts",
                column: "OutAccountGroupId",
                principalTable: "CarlErpCashFlowAccountGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpCashFlowTemplateAccounts_CarlErpCashFlowAccountGroups_OutAccountGroupId",
                table: "CarlErpCashFlowTemplateAccounts");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpCashFlowTemplateAccounts_OutAccountGroupId",
                table: "CarlErpCashFlowTemplateAccounts");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpCashFlowCategories_IsDefault",
                table: "CarlErpCashFlowCategories");

            migrationBuilder.DropColumn(
                name: "OutAccountGroupId",
                table: "CarlErpCashFlowTemplateAccounts");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "CarlErpCashFlowCategories");
        }
    }
}
