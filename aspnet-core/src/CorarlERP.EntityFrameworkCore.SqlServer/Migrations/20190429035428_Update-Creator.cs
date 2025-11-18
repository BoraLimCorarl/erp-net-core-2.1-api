using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateCreator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_CreatorUserId",
                table: "CarlErpJournals",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_LastModifierUserId",
                table: "CarlErpJournals",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournalItems_CreatorUserId",
                table: "CarlErpJournalItems",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournalItems_LastModifierUserId",
                table: "CarlErpJournalItems",
                column: "LastModifierUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpJournalItems_AbpUsers_CreatorUserId",
                table: "CarlErpJournalItems",
                column: "CreatorUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpJournalItems_AbpUsers_LastModifierUserId",
                table: "CarlErpJournalItems",
                column: "LastModifierUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpJournals_AbpUsers_CreatorUserId",
                table: "CarlErpJournals",
                column: "CreatorUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpJournals_AbpUsers_LastModifierUserId",
                table: "CarlErpJournals",
                column: "LastModifierUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpJournalItems_AbpUsers_CreatorUserId",
                table: "CarlErpJournalItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpJournalItems_AbpUsers_LastModifierUserId",
                table: "CarlErpJournalItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpJournals_AbpUsers_CreatorUserId",
                table: "CarlErpJournals");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpJournals_AbpUsers_LastModifierUserId",
                table: "CarlErpJournals");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpJournals_CreatorUserId",
                table: "CarlErpJournals");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpJournals_LastModifierUserId",
                table: "CarlErpJournals");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpJournalItems_CreatorUserId",
                table: "CarlErpJournalItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpJournalItems_LastModifierUserId",
                table: "CarlErpJournalItems");
        }
    }
}
