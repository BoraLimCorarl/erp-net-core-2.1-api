using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class PermissionLockTransactionLockSchedule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {            
            migrationBuilder.CreateTable(
                name: "CarlErpPermissionLocks",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    LockTransaction = table.Column<int>(nullable: false),
                    LockAction = table.Column<int>(nullable: false),
                    PermissionDate = table.Column<DateTime>(nullable: true),
                    LocationId = table.Column<long>(nullable: true),
                    TransactionNo = table.Column<string>(nullable: true),
                    PermissionCode = table.Column<string>(nullable: true),
                    ExpiredDuration = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPermissionLocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPermissionLocks_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPermissionLockSchedules",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ScheduleType = table.Column<int>(nullable: false),
                    ScheduleTime = table.Column<DateTime>(nullable: false),
                    ScheduleDate = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPermissionLockSchedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPermissionLockScheduleItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    TransactionLockSheduleId = table.Column<long>(nullable: false),
                    LockTransaction = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPermissionLockScheduleItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPermissionLockScheduleItems_CarlErpPermissionLockSchedules_TransactionLockSheduleId",
                        column: x => x.TransactionLockSheduleId,
                        principalTable: "CarlErpPermissionLockSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPermissionLocks_LocationId",
                table: "CarlErpPermissionLocks",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPermissionLocks_LockTransaction",
                table: "CarlErpPermissionLocks",
                column: "LockTransaction");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPermissionLocks_TransactionNo",
                table: "CarlErpPermissionLocks",
                column: "TransactionNo");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPermissionLockScheduleItems_TransactionLockSheduleId",
                table: "CarlErpPermissionLockScheduleItems",
                column: "TransactionLockSheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPermissionLockSchedules_ScheduleDate",
                table: "CarlErpPermissionLockSchedules",
                column: "ScheduleDate");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPermissionLockSchedules_ScheduleType",
                table: "CarlErpPermissionLockSchedules",
                column: "ScheduleType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpPermissionLocks");

            migrationBuilder.DropTable(
                name: "CarlErpPermissionLockScheduleItems");

            migrationBuilder.DropTable(
                name: "CarlErpPermissionLockSchedules");
                       
        }
    }
}
