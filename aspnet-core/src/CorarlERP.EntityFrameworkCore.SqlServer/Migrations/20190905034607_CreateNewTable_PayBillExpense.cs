using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class CreateNewTable_PayBillExpense : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Change",
                table: "CarlErpPayBills",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "CarlErpPayBillExpense",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    PayBillId = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPayBillExpense", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPayBillExpense_CarlErpChartOfAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPayBillExpense_CarlErpPayBills_PayBillId",
                        column: x => x.PayBillId,
                        principalTable: "CarlErpPayBills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPayBillExpense_AccountId",
                table: "CarlErpPayBillExpense",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPayBillExpense_PayBillId",
                table: "CarlErpPayBillExpense",
                column: "PayBillId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPayBillExpense_TenantId_CreatorUserId",
                table: "CarlErpPayBillExpense",
                columns: new[] { "TenantId", "CreatorUserId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpPayBillExpense");

            migrationBuilder.DropColumn(
                name: "Change",
                table: "CarlErpPayBills");
        }
    }
}
