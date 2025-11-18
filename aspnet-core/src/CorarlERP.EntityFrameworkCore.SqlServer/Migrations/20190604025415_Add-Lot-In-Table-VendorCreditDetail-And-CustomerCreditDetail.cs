using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddLotInTableVendorCreditDetailAndCustomerCreditDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LotId",
                table: "CarlErpVendorCreditDetails",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LotId",
                table: "CarlErpCusotmerCreditDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCreditDetails_LotId",
                table: "CarlErpVendorCreditDetails",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCusotmerCreditDetails_LotId",
                table: "CarlErpCusotmerCreditDetails",
                column: "LotId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpCusotmerCreditDetails_CarlErpLots_LotId",
                table: "CarlErpCusotmerCreditDetails",
                column: "LotId",
                principalTable: "CarlErpLots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpVendorCreditDetails_CarlErpLots_LotId",
                table: "CarlErpVendorCreditDetails",
                column: "LotId",
                principalTable: "CarlErpLots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpCusotmerCreditDetails_CarlErpLots_LotId",
                table: "CarlErpCusotmerCreditDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpVendorCreditDetails_CarlErpLots_LotId",
                table: "CarlErpVendorCreditDetails");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpVendorCreditDetails_LotId",
                table: "CarlErpVendorCreditDetails");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpCusotmerCreditDetails_LotId",
                table: "CarlErpCusotmerCreditDetails");

            migrationBuilder.DropColumn(
                name: "LotId",
                table: "CarlErpVendorCreditDetails");

            migrationBuilder.DropColumn(
                name: "LotId",
                table: "CarlErpCusotmerCreditDetails");
        }
    }
}
