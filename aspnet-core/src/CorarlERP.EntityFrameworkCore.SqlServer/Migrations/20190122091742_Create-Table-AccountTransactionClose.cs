using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class CreateTableAccountTransactionClose : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarlErpAccountTransactionCloses",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    AccountId = table.Column<Guid>(nullable: false),
                    Debit = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpAccountTransactionCloses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpAccountTransactionCloses_CarlErpChartOfAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpAccountTransactionCloses_AccountId",
                table: "CarlErpAccountTransactionCloses",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpAccountTransactionCloses_TenantId_CreatorUserId_Balance_Debit_Credit",
                table: "CarlErpAccountTransactionCloses",
                columns: new[] { "TenantId", "CreatorUserId", "Balance", "Debit", "Credit" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpAccountTransactionCloses");
        }
    }
}
