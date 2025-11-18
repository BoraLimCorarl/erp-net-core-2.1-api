using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateClosePeriodMultiCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CurrencyId",
                table: "CarlErpAccountTransactionCloses",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyBalance",
                table: "CarlErpAccountTransactionCloses",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyCredit",
                table: "CarlErpAccountTransactionCloses",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyDebit",
                table: "CarlErpAccountTransactionCloses",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpAccountTypes_AccountTypeName",
                table: "CarlErpAccountTypes",
                column: "AccountTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpAccountTransactionCloses_CurrencyId",
                table: "CarlErpAccountTransactionCloses",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpAccountTransactionCloses_CarlErpCurrencies_CurrencyId",
                table: "CarlErpAccountTransactionCloses",
                column: "CurrencyId",
                principalTable: "CarlErpCurrencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(
                @"                     
                    update CarlErpAccountTransactionCloses 
                    set CurrencyId = t.CurrencyId, MultiCurrencyDebit = Debit, MultiCurrencyCredit = Credit, MultiCurrencyBalance = Balance
                    from CarlErpAccountTransactionCloses c
                    join AbpTenants t
                    on t.Id = c.TenantId                 
                ");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpAccountTransactionCloses_CarlErpCurrencies_CurrencyId",
                table: "CarlErpAccountTransactionCloses");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpAccountTypes_AccountTypeName",
                table: "CarlErpAccountTypes");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpAccountTransactionCloses_CurrencyId",
                table: "CarlErpAccountTransactionCloses");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "CarlErpAccountTransactionCloses");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyBalance",
                table: "CarlErpAccountTransactionCloses");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyCredit",
                table: "CarlErpAccountTransactionCloses");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyDebit",
                table: "CarlErpAccountTransactionCloses");
        }
    }
}
