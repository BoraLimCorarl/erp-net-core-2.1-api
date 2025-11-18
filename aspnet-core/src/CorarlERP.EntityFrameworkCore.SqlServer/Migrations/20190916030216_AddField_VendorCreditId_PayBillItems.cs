using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddField_VendorCreditId_PayBillItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "BillId",
                table: "CarlErpPayBillDeail",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<Guid>(
                name: "VendorCreditId",
                table: "CarlErpPayBillDeail",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPayBillDeail_VendorCreditId",
                table: "CarlErpPayBillDeail",
                column: "VendorCreditId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpPayBillDeail_CarlErpVendorCredit_VendorCreditId",
                table: "CarlErpPayBillDeail",
                column: "VendorCreditId",
                principalTable: "CarlErpVendorCredit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpPayBillDeail_CarlErpVendorCredit_VendorCreditId",
                table: "CarlErpPayBillDeail");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpPayBillDeail_VendorCreditId",
                table: "CarlErpPayBillDeail");

            migrationBuilder.DropColumn(
                name: "VendorCreditId",
                table: "CarlErpPayBillDeail");

            migrationBuilder.AlterColumn<Guid>(
                name: "BillId",
                table: "CarlErpPayBillDeail",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
