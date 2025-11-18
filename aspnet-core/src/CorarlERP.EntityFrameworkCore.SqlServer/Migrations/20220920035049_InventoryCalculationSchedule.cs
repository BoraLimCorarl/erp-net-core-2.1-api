using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class InventoryCalculationSchedule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarlErpInventoryCalculationSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    ScheduleTime = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    TranactionId = table.Column<Guid>(nullable: true),
                    CalculationFromDate = table.Column<DateTime>(nullable: false),
                    MaxTry = table.Column<int>(nullable: false),
                    TryCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpInventoryCalculationSchedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpInventoryCalculationScheduleItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    ScheduleId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpInventoryCalculationScheduleItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpInventoryCalculationScheduleItems_CarlErpInventoryCalculationSchedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "CarlErpInventoryCalculationSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCalculationScheduleItems_ScheduleId",
                table: "CarlErpInventoryCalculationScheduleItems",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCalculationSchedules_TranactionId",
                table: "CarlErpInventoryCalculationSchedules",
                column: "TranactionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpInventoryCalculationScheduleItems");

            migrationBuilder.DropTable(
                name: "CarlErpInventoryCalculationSchedules");
        }
    }
}
