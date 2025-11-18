using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddNewField_Description_To_ExpenseAccount_Table_Payment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CarlErpReceivePaymentExpense",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CarlErpPayBillExpense",
                maxLength: 256,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "CarlErpReceivePaymentExpense");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CarlErpPayBillExpense");
        }
    }
}
