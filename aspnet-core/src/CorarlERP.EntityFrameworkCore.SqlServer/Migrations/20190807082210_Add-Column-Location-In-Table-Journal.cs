using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddColumnLocationInTableJournal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "CarlErpJournals",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_LocationId",
                table: "CarlErpJournals",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpJournals_CarlErpLocations_LocationId",
                table: "CarlErpJournals",
                column: "LocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpJournals_CarlErpLocations_LocationId",
                table: "CarlErpJournals");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpJournals_LocationId",
                table: "CarlErpJournals");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "CarlErpJournals");
        }
    }
}
