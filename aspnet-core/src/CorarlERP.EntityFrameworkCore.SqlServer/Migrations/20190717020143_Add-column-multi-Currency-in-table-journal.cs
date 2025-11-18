using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddcolumnmultiCurrencyintablejournal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MultiCurrencyId",
                table: "CarlErpJournals",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencySubTotal",
                table: "CarlErpBills",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTax",
                table: "CarlErpBills",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotal",
                table: "CarlErpBills",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotal",
                table: "CarlErpBillItems",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyUnitCost",
                table: "CarlErpBillItems",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_MultiCurrencyId",
                table: "CarlErpJournals",
                column: "MultiCurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpJournals_CarlErpCurrencies_MultiCurrencyId",
                table: "CarlErpJournals",
                column: "MultiCurrencyId",
                principalTable: "CarlErpCurrencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpJournals_CarlErpCurrencies_MultiCurrencyId",
                table: "CarlErpJournals");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpJournals_MultiCurrencyId",
                table: "CarlErpJournals");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyId",
                table: "CarlErpJournals");

            migrationBuilder.DropColumn(
                name: "MultiCurrencySubTotal",
                table: "CarlErpBills");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTax",
                table: "CarlErpBills");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotal",
                table: "CarlErpBills");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotal",
                table: "CarlErpBillItems");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyUnitCost",
                table: "CarlErpBillItems");
        }
    }
}
