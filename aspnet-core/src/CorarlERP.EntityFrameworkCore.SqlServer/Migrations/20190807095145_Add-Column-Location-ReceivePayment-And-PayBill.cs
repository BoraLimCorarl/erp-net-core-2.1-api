using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddColumnLocationReceivePaymentAndPayBill : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "CarlErpReceivePayments",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "CarlErpPayBills",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReceivePayments_LocationId",
                table: "CarlErpReceivePayments",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPayBills_LocationId",
                table: "CarlErpPayBills",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpPayBills_CarlErpLocations_LocationId",
                table: "CarlErpPayBills",
                column: "LocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpReceivePayments_CarlErpLocations_LocationId",
                table: "CarlErpReceivePayments",
                column: "LocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpPayBills_CarlErpLocations_LocationId",
                table: "CarlErpPayBills");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpReceivePayments_CarlErpLocations_LocationId",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpReceivePayments_LocationId",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpPayBills_LocationId",
                table: "CarlErpPayBills");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "CarlErpPayBills");
        }
    }
}
