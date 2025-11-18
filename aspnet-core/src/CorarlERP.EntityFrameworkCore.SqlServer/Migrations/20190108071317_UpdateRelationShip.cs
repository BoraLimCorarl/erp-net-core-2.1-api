using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateRelationShip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpJournals_CarlErpPhysicalCounts_PhysicalCountItemIssueId",
                table: "CarlErpJournals");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpJournals_CarlErpPhysicalCounts_PhysicalCountItemReceiptId",
                table: "CarlErpJournals");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpJournals_PhysicalCountItemIssueId",
                table: "CarlErpJournals");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpJournals_PhysicalCountItemReceiptId",
                table: "CarlErpJournals");

            migrationBuilder.DropColumn(
                name: "PhysicalCountItemIssueId",
                table: "CarlErpJournals");

            migrationBuilder.DropColumn(
                name: "PhysicalCountItemReceiptId",
                table: "CarlErpJournals");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PhysicalCountItemIssueId",
                table: "CarlErpJournals",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PhysicalCountItemReceiptId",
                table: "CarlErpJournals",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_PhysicalCountItemIssueId",
                table: "CarlErpJournals",
                column: "PhysicalCountItemIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_PhysicalCountItemReceiptId",
                table: "CarlErpJournals",
                column: "PhysicalCountItemReceiptId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpJournals_CarlErpPhysicalCounts_PhysicalCountItemIssueId",
                table: "CarlErpJournals",
                column: "PhysicalCountItemIssueId",
                principalTable: "CarlErpPhysicalCounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpJournals_CarlErpPhysicalCounts_PhysicalCountItemReceiptId",
                table: "CarlErpJournals",
                column: "PhysicalCountItemReceiptId",
                principalTable: "CarlErpPhysicalCounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
