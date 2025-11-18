using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddLocationAndDateInVendorCustomerOpenBalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "CarlErpVendorCustomerOpenBalaces",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "CarlErpVendorCustomerOpenBalaces",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "CarlErpVendorCustomerOpenBalaces");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "CarlErpVendorCustomerOpenBalaces");
        }
    }
}
