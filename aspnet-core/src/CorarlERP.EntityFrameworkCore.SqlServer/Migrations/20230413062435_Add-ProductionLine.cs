using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddProductionLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProductionLineId",
                table: "CarlErpProductionPlans",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CarlErpProductionLines",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    ProductionLineName = table.Column<string>(maxLength: 512, nullable: false),
                    Memo = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpProductionLines", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionPlans_ProductionLineId",
                table: "CarlErpProductionPlans",
                column: "ProductionLineId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionLines_TenantId_ProductionLineName",
                table: "CarlErpProductionLines",
                columns: new[] { "TenantId", "ProductionLineName" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpProductionPlans_CarlErpProductionLines_ProductionLineId",
                table: "CarlErpProductionPlans",
                column: "ProductionLineId",
                principalTable: "CarlErpProductionLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpProductionPlans_CarlErpProductionLines_ProductionLineId",
                table: "CarlErpProductionPlans");

            migrationBuilder.DropTable(
                name: "CarlErpProductionLines");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpProductionPlans_ProductionLineId",
                table: "CarlErpProductionPlans");

            migrationBuilder.DropColumn(
                name: "ProductionLineId",
                table: "CarlErpProductionPlans");
        }
    }
}
