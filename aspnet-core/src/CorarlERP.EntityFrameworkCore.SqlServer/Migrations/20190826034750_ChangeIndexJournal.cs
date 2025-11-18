using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class ChangeIndexJournal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CarlErpJournals_TenantId_CreatorUserId_JournalNo",
                table: "CarlErpJournals");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpJournalItems_TenantId_CreatorUserId_Key_Identifier",
                table: "CarlErpJournalItems");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_JournalNo",
                table: "CarlErpJournals",
                column: "JournalNo");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_TenantId_CreatorUserId",
                table: "CarlErpJournals",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournalItems_Identifier_Key",
                table: "CarlErpJournalItems",
                columns: new[] { "Identifier", "Key" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournalItems_TenantId_CreatorUserId",
                table: "CarlErpJournalItems",
                columns: new[] { "TenantId", "CreatorUserId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CarlErpJournals_JournalNo",
                table: "CarlErpJournals");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpJournals_TenantId_CreatorUserId",
                table: "CarlErpJournals");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpJournalItems_Identifier_Key",
                table: "CarlErpJournalItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpJournalItems_TenantId_CreatorUserId",
                table: "CarlErpJournalItems");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_TenantId_CreatorUserId_JournalNo",
                table: "CarlErpJournals",
                columns: new[] { "TenantId", "CreatorUserId", "JournalNo" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournalItems_TenantId_CreatorUserId_Key_Identifier",
                table: "CarlErpJournalItems",
                columns: new[] { "TenantId", "CreatorUserId", "Key", "Identifier" });
        }
    }
}
