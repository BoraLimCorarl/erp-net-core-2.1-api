using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateInvoice_FieldReceiveDateAndConvert : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ConvertToItemIssue",
                table: "CarlErpInvoices",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceiveDate",
                table: "CarlErpInvoices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConvertToItemIssue",
                table: "CarlErpInvoices");

            migrationBuilder.DropColumn(
                name: "ReceiveDate",
                table: "CarlErpInvoices");
        }
    }
}
