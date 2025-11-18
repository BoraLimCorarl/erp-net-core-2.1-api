using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class SubscriptionPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PackagePrice",
                table: "CarlErpSubscriptions",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "CarlErpSubscriptions",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "CarlErpSubscriptionPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    SubscriptionDate = table.Column<DateTime>(nullable: false),
                    AffectedDate = table.Column<DateTime>(nullable: false),
                    Duration = table.Column<int>(nullable: false),
                    DurationType = table.Column<int>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    PaymentMethod = table.Column<int>(nullable: false),
                    PackagePrice = table.Column<decimal>(nullable: false),
                    TotalPrice = table.Column<decimal>(nullable: false),
                    EditionId = table.Column<int>(nullable: false),
                    SubscriptionType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpSubscriptionPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpSubscriptionPayments_AbpEditions_EditionId",
                        column: x => x.EditionId,
                        principalTable: "AbpEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpSubscriptionPayments_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubscriptionPayments_EditionId",
                table: "CarlErpSubscriptionPayments",
                column: "EditionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubscriptionPayments_SubscriptionType",
                table: "CarlErpSubscriptionPayments",
                column: "SubscriptionType");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubscriptionPayments_TenantId",
                table: "CarlErpSubscriptionPayments",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpSubscriptionPayments");

            migrationBuilder.DropColumn(
                name: "PackagePrice",
                table: "CarlErpSubscriptions");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "CarlErpSubscriptions");
        }
    }
}
