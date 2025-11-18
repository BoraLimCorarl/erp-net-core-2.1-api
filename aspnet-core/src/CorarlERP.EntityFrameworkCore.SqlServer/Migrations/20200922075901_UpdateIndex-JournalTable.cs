using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateIndexJournalTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_Date",
                table: "CarlErpJournals",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_JournalType",
                table: "CarlErpJournals",
                column: "JournalType");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_TenantId_Date",
                table: "CarlErpJournals",
                columns: new[] { "TenantId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_TenantId_JournalType",
                table: "CarlErpJournals",
                columns: new[] { "TenantId", "JournalType" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CarlErpJournals_Date",
                table: "CarlErpJournals");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpJournals_JournalType",
                table: "CarlErpJournals");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpJournals_TenantId_Date",
                table: "CarlErpJournals");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpJournals_TenantId_JournalType",
                table: "CarlErpJournals");
        }
    }
}
