using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddNewField_ConvertToItemReceipt_In_CustomerCredit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ConvertToItemReceipt",
                table: "CarlErpCustomerCredits",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceiveDate",
                table: "CarlErpCustomerCredits",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConvertToItemReceipt",
                table: "CarlErpCustomerCredits");

            migrationBuilder.DropColumn(
                name: "ReceiveDate",
                table: "CarlErpCustomerCredits");
        }
    }
}
