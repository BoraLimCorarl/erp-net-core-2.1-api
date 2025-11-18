using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddProcutionIssueStandardCostGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarlErpProductionIssueStandardCostGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    ProductionId = table.Column<Guid>(nullable: false),
                    StandardCostGroupId = table.Column<long>(nullable: true),
                    TotalQty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalNetWeight = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpProductionIssueStandardCostGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpProductionIssueStandardCostGroups_CarlErpTransProductions_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "CarlErpTransProductions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpProductionIssueStandardCostGroups_CarlErpPropertyValues_StandardCostGroupId",
                        column: x => x.StandardCostGroupId,
                        principalTable: "CarlErpPropertyValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpProductionIssueStandardCosts",
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
                    table.PrimaryKey("PK_CarlErpProductionIssueStandardCosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpProductionIssueStandardCosts_CarlErpProductionPlans_ProductionPlanId",
                        column: x => x.ProductionPlanId,
                        principalTable: "CarlErpProductionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpProductionIssueStandardCosts_CarlErpPropertyValues_StandardCostGroupId",
                        column: x => x.StandardCostGroupId,
                        principalTable: "CarlErpPropertyValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionIssueStandardCostGroups_ProductionId",
                table: "CarlErpProductionIssueStandardCostGroups",
                column: "ProductionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionIssueStandardCostGroups_StandardCostGroupId",
                table: "CarlErpProductionIssueStandardCostGroups",
                column: "StandardCostGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionIssueStandardCosts_ProductionPlanId",
                table: "CarlErpProductionIssueStandardCosts",
                column: "ProductionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionIssueStandardCosts_StandardCostGroupId",
                table: "CarlErpProductionIssueStandardCosts",
                column: "StandardCostGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpProductionIssueStandardCostGroups");

            migrationBuilder.DropTable(
                name: "CarlErpProductionIssueStandardCosts");
        }
    }
}
