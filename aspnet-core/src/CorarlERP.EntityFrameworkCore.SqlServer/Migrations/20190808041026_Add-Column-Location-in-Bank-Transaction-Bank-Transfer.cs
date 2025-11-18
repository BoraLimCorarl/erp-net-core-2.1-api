using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddColumnLocationinBankTransactionBankTransfer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "CarlErpWithdraws",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "CarlErpDeposits",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FromLocationId",
                table: "CarlErpBankTransfers",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ToLocationId",
                table: "CarlErpBankTransfers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpWithdraws_LocationId",
                table: "CarlErpWithdraws",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpDeposits_LocationId",
                table: "CarlErpDeposits",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBankTransfers_FromLocationId",
                table: "CarlErpBankTransfers",
                column: "FromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBankTransfers_ToLocationId",
                table: "CarlErpBankTransfers",
                column: "ToLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpBankTransfers_CarlErpLocations_FromLocationId",
                table: "CarlErpBankTransfers",
                column: "FromLocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpBankTransfers_CarlErpLocations_ToLocationId",
                table: "CarlErpBankTransfers",
                column: "ToLocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpDeposits_CarlErpLocations_LocationId",
                table: "CarlErpDeposits",
                column: "LocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpWithdraws_CarlErpLocations_LocationId",
                table: "CarlErpWithdraws",
                column: "LocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpBankTransfers_CarlErpLocations_FromLocationId",
                table: "CarlErpBankTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpBankTransfers_CarlErpLocations_ToLocationId",
                table: "CarlErpBankTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpDeposits_CarlErpLocations_LocationId",
                table: "CarlErpDeposits");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpWithdraws_CarlErpLocations_LocationId",
                table: "CarlErpWithdraws");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpWithdraws_LocationId",
                table: "CarlErpWithdraws");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpDeposits_LocationId",
                table: "CarlErpDeposits");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpBankTransfers_FromLocationId",
                table: "CarlErpBankTransfers");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpBankTransfers_ToLocationId",
                table: "CarlErpBankTransfers");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "CarlErpWithdraws");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "CarlErpDeposits");

            migrationBuilder.DropColumn(
                name: "FromLocationId",
                table: "CarlErpBankTransfers");

            migrationBuilder.DropColumn(
                name: "ToLocationId",
                table: "CarlErpBankTransfers");
        }
    }
}
