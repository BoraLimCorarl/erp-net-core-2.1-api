using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class add_Table_Payment_Method : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarlErpPaymentMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    AccountId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPaymentMethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPaymentMethods_CarlErpChartOfAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPaymentMethods_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPaymentMethods_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPaymentMethods_AccountId",
                table: "CarlErpPaymentMethods",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPaymentMethods_CreatorUserId",
                table: "CarlErpPaymentMethods",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPaymentMethods_LastModifierUserId",
                table: "CarlErpPaymentMethods",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPaymentMethods_TenantId_CreatorUserId_Name",
                table: "CarlErpPaymentMethods",
                columns: new[] { "TenantId", "CreatorUserId", "Name" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpPaymentMethods");
        }
    }
}
