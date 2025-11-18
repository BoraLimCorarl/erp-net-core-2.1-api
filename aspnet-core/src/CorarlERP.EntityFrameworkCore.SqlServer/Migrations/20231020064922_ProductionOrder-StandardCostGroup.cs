using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class ProductionOrderStandardCostGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarlErpProductionStandardCostGroups",
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
                    table.PrimaryKey("PK_CarlErpProductionStandardCostGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpProductionStandardCostGroups_CarlErpTransProductions_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "CarlErpTransProductions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpProductionStandardCostGroups_CarlErpPropertyValues_StandardCostGroupId",
                        column: x => x.StandardCostGroupId,
                        principalTable: "CarlErpPropertyValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionStandardCostGroups_ProductionId",
                table: "CarlErpProductionStandardCostGroups",
                column: "ProductionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionStandardCostGroups_StandardCostGroupId",
                table: "CarlErpProductionStandardCostGroups",
                column: "StandardCostGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpProductionStandardCostGroups");
        }
    }
}
