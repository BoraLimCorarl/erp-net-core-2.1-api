using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdatePhysicalCount_ItemIssueNReceipt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PhysicalCountItemIssueId",
                table: "CarlErpJournals",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PhysicalCountItemReceiptId",
                table: "CarlErpJournals",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PhysicalCountId",
                table: "CarlErpItemReceipts",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PhysicalCountId",
                table: "CarlErpItemIssues",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_PhysicalCountItemIssueId",
                table: "CarlErpJournals",
                column: "PhysicalCountItemIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_PhysicalCountItemReceiptId",
                table: "CarlErpJournals",
                column: "PhysicalCountItemReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceipts_PhysicalCountId",
                table: "CarlErpItemReceipts",
                column: "PhysicalCountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssues_PhysicalCountId",
                table: "CarlErpItemIssues",
                column: "PhysicalCountId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemIssues_CarlErpPhysicalCounts_PhysicalCountId",
                table: "CarlErpItemIssues",
                column: "PhysicalCountId",
                principalTable: "CarlErpPhysicalCounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemReceipts_CarlErpPhysicalCounts_PhysicalCountId",
                table: "CarlErpItemReceipts",
                column: "PhysicalCountId",
                principalTable: "CarlErpPhysicalCounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemIssues_CarlErpPhysicalCounts_PhysicalCountId",
                table: "CarlErpItemIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemReceipts_CarlErpPhysicalCounts_PhysicalCountId",
                table: "CarlErpItemReceipts");

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

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemReceipts_PhysicalCountId",
                table: "CarlErpItemReceipts");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemIssues_PhysicalCountId",
                table: "CarlErpItemIssues");

            migrationBuilder.DropColumn(
                name: "PhysicalCountItemIssueId",
                table: "CarlErpJournals");

            migrationBuilder.DropColumn(
                name: "PhysicalCountItemReceiptId",
                table: "CarlErpJournals");

            migrationBuilder.DropColumn(
                name: "PhysicalCountId",
                table: "CarlErpItemReceipts");

            migrationBuilder.DropColumn(
                name: "PhysicalCountId",
                table: "CarlErpItemIssues");
        }
    }
}
