using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class updatevendorcredit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ConvertToItemIssueVendor",
                table: "CarlErpVendorCredit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "IssueDate",
                table: "CarlErpVendorCredit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConvertToItemIssueVendor",
                table: "CarlErpVendorCredit");

            migrationBuilder.DropColumn(
                name: "IssueDate",
                table: "CarlErpVendorCredit");
        }
    }
}
