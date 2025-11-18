using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class DeleteAdjustRoundingCost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(
                @"                 
                    delete from CarlErpJournalItems where JournalId in (select Id from CarlErpJournals where RoundedAdjustmentJournalId is not null)
                    delete from CarlErpJournals where RoundedAdjustmentJournalId is not null          
                ");
            }

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

        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
