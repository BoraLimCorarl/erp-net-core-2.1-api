using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddCustomerTo_DepositAndWithdrawal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "CarlErpWithdraws",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReceiveFromCustomerId",
                table: "CarlErpDeposits",
                nullable: true);
            

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpWithdraws_CustomerId",
                table: "CarlErpWithdraws",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpDeposits_ReceiveFromCustomerId",
                table: "CarlErpDeposits",
                column: "ReceiveFromCustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpDeposits_CarlErpCustomers_ReceiveFromCustomerId",
                table: "CarlErpDeposits",
                column: "ReceiveFromCustomerId",
                principalTable: "CarlErpCustomers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpWithdraws_CarlErpCustomers_CustomerId",
                table: "CarlErpWithdraws",
                column: "CustomerId",
                principalTable: "CarlErpCustomers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpDeposits_CarlErpCustomers_ReceiveFromCustomerId",
                table: "CarlErpDeposits");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpWithdraws_CarlErpCustomers_CustomerId",
                table: "CarlErpWithdraws");
            
            migrationBuilder.DropIndex(
                name: "IX_CarlErpWithdraws_CustomerId",
                table: "CarlErpWithdraws");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpDeposits_ReceiveFromCustomerId",
                table: "CarlErpDeposits");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "CarlErpWithdraws");

            migrationBuilder.DropColumn(
                name: "ReceiveFromCustomerId",
                table: "CarlErpDeposits");
        }
    }
}
