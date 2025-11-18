using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class SubscriptionAndAutoItemCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deactivate",
                table: "AbpUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AutoItemCode",
                table: "AbpTenants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ItemCode",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prifix",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SubscriptionId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UseBatchNo",
                table: "AbpTenants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CarlErpSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    Duration = table.Column<int>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EditionId = table.Column<int>(nullable: true),
                    Endate = table.Column<DateTime>(nullable: true),
                    DurationType = table.Column<int>(nullable: true),
                    Unlimited = table.Column<bool>(nullable: false),
                    SubScriptionEndDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpSubscriptions_AbpEditions_EditionId",
                        column: x => x.EditionId,
                        principalTable: "AbpEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpSubscriptions_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_SubscriptionId",
                table: "AbpTenants",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubscriptions_CreatorUserId",
                table: "CarlErpSubscriptions",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubscriptions_EditionId",
                table: "CarlErpSubscriptions",
                column: "EditionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubscriptions_TenantId",
                table: "CarlErpSubscriptions",
                column: "TenantId");

            migrationBuilder.Sql(@"insert into CarlErpSubscriptions (Id, TenantId, EditionId, CreatorUserId, CreationTime, Unlimited)
                select newid(), Id, EditionId, CreatorUserId, CreationTime, 1 from AbpTenants
                update AbpTenants set SubscriptionId = s.Id
                from AbpTenants t join CarlErpSubscriptions s on t.Id = s.TenantId");
            migrationBuilder.Sql(@"UPDATE CarlErpSubscriptions SET StartDate = CreationTime select * from CarlErpSubscriptions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_SubscriptionId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "Deactivate",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "AutoItemCode",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "ItemCode",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "Prifix",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "SubscriptionId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "UseBatchNo",
                table: "AbpTenants");
        }
    }
}
