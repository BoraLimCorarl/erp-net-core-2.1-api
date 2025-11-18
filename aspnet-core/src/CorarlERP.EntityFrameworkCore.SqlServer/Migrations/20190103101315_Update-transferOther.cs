using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdatetransferOther : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ConvertToIssueAndReceiptTransfer",
                table: "CarlErpTransferOrders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ItemIssueTransferDate",
                table: "CarlErpTransferOrders",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ItemReceiptTransferDate",
                table: "CarlErpTransferOrders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConvertToIssueAndReceiptTransfer",
                table: "CarlErpTransferOrders");

            migrationBuilder.DropColumn(
                name: "ItemIssueTransferDate",
                table: "CarlErpTransferOrders");

            migrationBuilder.DropColumn(
                name: "ItemReceiptTransferDate",
                table: "CarlErpTransferOrders");
        }
    }
}
