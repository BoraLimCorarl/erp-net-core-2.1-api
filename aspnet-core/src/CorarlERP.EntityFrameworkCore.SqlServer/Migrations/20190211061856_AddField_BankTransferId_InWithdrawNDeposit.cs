using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddField_BankTransferId_InWithdrawNDeposit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BankTransferId",
                table: "CarlErpWithdraws",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BankTransferId",
                table: "CarlErpDeposits",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankTransferId",
                table: "CarlErpWithdraws");

            migrationBuilder.DropColumn(
                name: "BankTransferId",
                table: "CarlErpDeposits");
        }
    }
}
