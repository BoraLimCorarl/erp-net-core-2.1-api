using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class HostCalculationSchedule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpInventoryCalculationScheduleItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpInventoryCalculationSchedules_TranactionId",
                table: "CarlErpInventoryCalculationSchedules");

            migrationBuilder.DropColumn(
                name: "CalculationFromDate",
                table: "CarlErpInventoryCalculationSchedules");

            migrationBuilder.DropColumn(
                name: "MaxTry",
                table: "CarlErpInventoryCalculationSchedules");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CarlErpInventoryCalculationSchedules");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "CarlErpInventoryCalculationSchedules");

            migrationBuilder.DropColumn(
                name: "TranactionId",
                table: "CarlErpInventoryCalculationSchedules");

            migrationBuilder.DropColumn(
                name: "TryCount",
                table: "CarlErpInventoryCalculationSchedules");

            migrationBuilder.CreateTable(
                name: "CarlErpInventoryCalculationItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpInventoryCalculationItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpInventoryCalculationItems_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInventoryCalculationItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInventoryCalculationItems_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCalculationItems_CreatorUserId",
                table: "CarlErpInventoryCalculationItems",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCalculationItems_ItemId",
                table: "CarlErpInventoryCalculationItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCalculationItems_LastModifierUserId",
                table: "CarlErpInventoryCalculationItems",
                column: "LastModifierUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpInventoryCalculationItems");

            migrationBuilder.AddColumn<DateTime>(
                name: "CalculationFromDate",
                table: "CarlErpInventoryCalculationSchedules",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "MaxTry",
                table: "CarlErpInventoryCalculationSchedules",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "CarlErpInventoryCalculationSchedules",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "CarlErpInventoryCalculationSchedules",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TranactionId",
                table: "CarlErpInventoryCalculationSchedules",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TryCount",
                table: "CarlErpInventoryCalculationSchedules",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CarlErpInventoryCalculationScheduleItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ScheduleId = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
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
                name: "IX_CarlErpInventoryCalculationSchedules_TranactionId",
                table: "CarlErpInventoryCalculationSchedules",
                column: "TranactionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCalculationScheduleItems_ScheduleId",
                table: "CarlErpInventoryCalculationScheduleItems",
                column: "ScheduleId");
        }
    }
}
