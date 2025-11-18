using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class ProductionStandardCostGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalIssueNetWeight",
                table: "CarlErpTransProductions",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalIssueQty",
                table: "CarlErpTransProductions",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalReceiptNetWeight",
                table: "CarlErpTransProductions",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalReceiptQty",
                table: "CarlErpTransProductions",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsStandardCostGroup",
                table: "CarlErpProperties",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalIssueNetWeight",
                table: "CarlErpProductionPlans",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalIssueQty",
                table: "CarlErpProductionPlans",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalReceiptNetWeight",
                table: "CarlErpProductionPlans",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalReceiptQty",
                table: "CarlErpProductionPlans",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "ProductionSummaryNetWeight",
                table: "AbpTenants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ProductionSummaryQty",
                table: "AbpTenants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CarlErpProductionStandardCosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    ProductionPlanId = table.Column<Guid>(nullable: false),
                    StandardCostGroupId = table.Column<long>(nullable: true),
                    TotalQty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalNetWeight = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    QtyPercentage = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    NetWeightPercentage = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpProductionStandardCosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpProductionStandardCosts_CarlErpProductionPlans_ProductionPlanId",
                        column: x => x.ProductionPlanId,
                        principalTable: "CarlErpProductionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpProductionStandardCosts_CarlErpPropertyValues_StandardCostGroupId",
                        column: x => x.StandardCostGroupId,
                        principalTable: "CarlErpPropertyValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionStandardCosts_ProductionPlanId",
                table: "CarlErpProductionStandardCosts",
                column: "ProductionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionStandardCosts_StandardCostGroupId",
                table: "CarlErpProductionStandardCosts",
                column: "StandardCostGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpProductionStandardCosts");

            migrationBuilder.DropColumn(
                name: "TotalIssueNetWeight",
                table: "CarlErpTransProductions");

            migrationBuilder.DropColumn(
                name: "TotalIssueQty",
                table: "CarlErpTransProductions");

            migrationBuilder.DropColumn(
                name: "TotalReceiptNetWeight",
                table: "CarlErpTransProductions");

            migrationBuilder.DropColumn(
                name: "TotalReceiptQty",
                table: "CarlErpTransProductions");

            migrationBuilder.DropColumn(
                name: "IsStandardCostGroup",
                table: "CarlErpProperties");

            migrationBuilder.DropColumn(
                name: "TotalIssueNetWeight",
                table: "CarlErpProductionPlans");

            migrationBuilder.DropColumn(
                name: "TotalIssueQty",
                table: "CarlErpProductionPlans");

            migrationBuilder.DropColumn(
                name: "TotalReceiptNetWeight",
                table: "CarlErpProductionPlans");

            migrationBuilder.DropColumn(
                name: "TotalReceiptQty",
                table: "CarlErpProductionPlans");

            migrationBuilder.DropColumn(
                name: "ProductionSummaryNetWeight",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "ProductionSummaryQty",
                table: "AbpTenants");
        }
    }
}
