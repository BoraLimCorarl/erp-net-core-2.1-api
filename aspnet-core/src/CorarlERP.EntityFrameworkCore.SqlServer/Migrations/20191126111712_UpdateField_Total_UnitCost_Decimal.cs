using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateField_Total_UnitCost_Decimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Debit",
                table: "CarlErpJournals",
                type: "decimal(28,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Credit",
                table: "CarlErpJournals",
                type: "decimal(28,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Debit",
                table: "CarlErpJournalItems",
                type: "decimal(28,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Credit",
                table: "CarlErpJournalItems",
                type: "decimal(28,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "CarlErpItemReceipts",
                type: "decimal(28,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                table: "CarlErpItemReceiptItems",
                type: "decimal(28,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "CarlErpItemReceiptItems",
                type: "decimal(28,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                table: "CarlErpItemReceiptCustomerCreditItem",
                type: "decimal(28,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "CarlErpItemReceiptCustomerCreditItem",
                type: "decimal(28,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "CarlErpItemReceiptCustomerCredit",
                type: "decimal(28,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                table: "CarlErpItemIssueVendorCreditItem",
                type: "decimal(28,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "CarlErpItemIssueVendorCreditItem",
                type: "decimal(28,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "CarlErpItemIssueVendorCredit",
                type: "decimal(28,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "CarlErpItemIssues",
                type: "decimal(28,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                table: "CarlErpItemIssueItems",
                type: "decimal(28,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "CarlErpItemIssueItems",
                type: "decimal(28,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,6)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Debit",
                table: "CarlErpJournals",
                type: "decimal(19,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Credit",
                table: "CarlErpJournals",
                type: "decimal(19,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Debit",
                table: "CarlErpJournalItems",
                type: "decimal(19,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Credit",
                table: "CarlErpJournalItems",
                type: "decimal(19,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "CarlErpItemReceipts",
                type: "decimal(19,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                table: "CarlErpItemReceiptItems",
                type: "decimal(19,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "CarlErpItemReceiptItems",
                type: "decimal(19,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                table: "CarlErpItemReceiptCustomerCreditItem",
                type: "decimal(19,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "CarlErpItemReceiptCustomerCreditItem",
                type: "decimal(19,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "CarlErpItemReceiptCustomerCredit",
                type: "decimal(19,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                table: "CarlErpItemIssueVendorCreditItem",
                type: "decimal(19,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "CarlErpItemIssueVendorCreditItem",
                type: "decimal(19,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "CarlErpItemIssueVendorCredit",
                type: "decimal(19,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "CarlErpItemIssues",
                type: "decimal(19,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                table: "CarlErpItemIssueItems",
                type: "decimal(19,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "CarlErpItemIssueItems",
                type: "decimal(19,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,6)");
        }
    }
}
