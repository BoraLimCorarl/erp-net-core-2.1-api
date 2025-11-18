using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddFeild_AmountMultiCurrency_IntoExpenseDetailOfPayBill : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyAmount",
                table: "CarlErpPayBillExpense",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MultiCurrencyAmount",
                table: "CarlErpPayBillExpense");
        }
    }
}
