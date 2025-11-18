using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddColumnMultiCurrencyOpenBalanceAndMultiCurrencyTotalPaid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyOpenBalance",
                table: "CarlErpBills",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalPaid",
                table: "CarlErpBills",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MultiCurrencyOpenBalance",
                table: "CarlErpBills");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalPaid",
                table: "CarlErpBills");
        }
    }
}
