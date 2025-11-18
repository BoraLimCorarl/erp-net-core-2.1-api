using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateVendorCustomerOpenBalanceAddBalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "CarlErpVendorCustomerOpenBalaces",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MuliCurrencyBalance",
                table: "CarlErpVendorCustomerOpenBalaces",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "CarlErpVendorCustomerOpenBalaces");

            migrationBuilder.DropColumn(
                name: "MuliCurrencyBalance",
                table: "CarlErpVendorCustomerOpenBalaces");
        }
    }
}
