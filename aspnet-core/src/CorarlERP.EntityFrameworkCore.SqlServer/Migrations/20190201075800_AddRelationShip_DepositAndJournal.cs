using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddRelationShip_DepositAndJournal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepositId",
                table: "CarlErpJournals",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_DepositId",
                table: "CarlErpJournals",
                column: "DepositId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpJournals_CarlErpDeposits_DepositId",
                table: "CarlErpJournals",
                column: "DepositId",
                principalTable: "CarlErpDeposits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpJournals_CarlErpDeposits_DepositId",
                table: "CarlErpJournals");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpJournals_DepositId",
                table: "CarlErpJournals");

            migrationBuilder.DropColumn(
                name: "DepositId",
                table: "CarlErpJournals");
        }
    }
}
