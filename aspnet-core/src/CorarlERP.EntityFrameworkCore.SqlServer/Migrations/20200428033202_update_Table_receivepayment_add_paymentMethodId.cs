using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class update_Table_receivepayment_add_paymentMethodId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PaymentMethodId",
                table: "CarlErpReceivePayments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReceivePayments_PaymentMethodId",
                table: "CarlErpReceivePayments",
                column: "PaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpReceivePayments_CarlErpPaymentMethods_PaymentMethodId",
                table: "CarlErpReceivePayments",
                column: "PaymentMethodId",
                principalTable: "CarlErpPaymentMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpReceivePayments_CarlErpPaymentMethods_PaymentMethodId",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpReceivePayments_PaymentMethodId",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "CarlErpReceivePayments");
        }
    }
}
