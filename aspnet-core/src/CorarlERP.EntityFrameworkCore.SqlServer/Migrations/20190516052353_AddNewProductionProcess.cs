using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddNewProductionProcess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProductionProcessId",
                table: "CarlErpTransProductions",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProductionProcessId",
                table: "CarlErpItemReceipts",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProductionProcessId",
                table: "CarlErpItemIssues",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CarlErpProductionProcess",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    ProcessName = table.Column<string>(maxLength: 512, nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    Memo = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpProductionProcess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpProductionProcess_CarlErpChartOfAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransProductions_ProductionProcessId",
                table: "CarlErpTransProductions",
                column: "ProductionProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceipts_ProductionProcessId",
                table: "CarlErpItemReceipts",
                column: "ProductionProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssues_ProductionProcessId",
                table: "CarlErpItemIssues",
                column: "ProductionProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionProcess_AccountId",
                table: "CarlErpProductionProcess",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionProcess_TenantId_CreatorUserId_ProcessName",
                table: "CarlErpProductionProcess",
                columns: new[] { "TenantId", "CreatorUserId", "ProcessName" });

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemIssues_CarlErpProductionProcess_ProductionProcessId",
                table: "CarlErpItemIssues",
                column: "ProductionProcessId",
                principalTable: "CarlErpProductionProcess",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemReceipts_CarlErpProductionProcess_ProductionProcessId",
                table: "CarlErpItemReceipts",
                column: "ProductionProcessId",
                principalTable: "CarlErpProductionProcess",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpTransProductions_CarlErpProductionProcess_ProductionProcessId",
                table: "CarlErpTransProductions",
                column: "ProductionProcessId",
                principalTable: "CarlErpProductionProcess",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemIssues_CarlErpProductionProcess_ProductionProcessId",
                table: "CarlErpItemIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemReceipts_CarlErpProductionProcess_ProductionProcessId",
                table: "CarlErpItemReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpTransProductions_CarlErpProductionProcess_ProductionProcessId",
                table: "CarlErpTransProductions");

            migrationBuilder.DropTable(
                name: "CarlErpProductionProcess");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpTransProductions_ProductionProcessId",
                table: "CarlErpTransProductions");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemReceipts_ProductionProcessId",
                table: "CarlErpItemReceipts");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemIssues_ProductionProcessId",
                table: "CarlErpItemIssues");

            migrationBuilder.DropColumn(
                name: "ProductionProcessId",
                table: "CarlErpTransProductions");

            migrationBuilder.DropColumn(
                name: "ProductionProcessId",
                table: "CarlErpItemReceipts");

            migrationBuilder.DropColumn(
                name: "ProductionProcessId",
                table: "CarlErpItemIssues");
        }
    }
}
