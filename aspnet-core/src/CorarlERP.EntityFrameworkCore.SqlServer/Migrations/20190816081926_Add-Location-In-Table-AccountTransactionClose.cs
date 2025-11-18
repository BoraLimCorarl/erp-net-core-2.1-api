using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddLocationInTableAccountTransactionClose : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "CarlErpAccountTransactionCloses",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpAccountTransactionCloses_LocationId",
                table: "CarlErpAccountTransactionCloses",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpAccountTransactionCloses_CarlErpLocations_LocationId",
                table: "CarlErpAccountTransactionCloses",
                column: "LocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpAccountTransactionCloses_CarlErpLocations_LocationId",
                table: "CarlErpAccountTransactionCloses");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpAccountTransactionCloses_LocationId",
                table: "CarlErpAccountTransactionCloses");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "CarlErpAccountTransactionCloses");
        }
    }
}
