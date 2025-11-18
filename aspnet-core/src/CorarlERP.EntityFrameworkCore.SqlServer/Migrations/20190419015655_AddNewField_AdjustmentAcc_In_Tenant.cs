using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddNewField_AdjustmentAcc_In_Tenant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RoundedAdjustmentJournalId",
                table: "CarlErpJournals",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_RoundedAdjustmentJournalId",
                table: "CarlErpJournals",
                column: "RoundedAdjustmentJournalId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpJournals_CarlErpJournals_RoundedAdjustmentJournalId",
                table: "CarlErpJournals",
                column: "RoundedAdjustmentJournalId",
                principalTable: "CarlErpJournals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpJournals_CarlErpJournals_RoundedAdjustmentJournalId",
                table: "CarlErpJournals");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpJournals_RoundedAdjustmentJournalId",
                table: "CarlErpJournals");

            migrationBuilder.DropColumn(
                name: "RoundedAdjustmentJournalId",
                table: "CarlErpJournals");
        }
    }
}
