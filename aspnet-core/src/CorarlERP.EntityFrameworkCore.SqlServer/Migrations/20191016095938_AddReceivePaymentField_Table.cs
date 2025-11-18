using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddReceivePaymentField_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Change",
                table: "CarlErpReceivePayments",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyChange",
                table: "CarlErpReceivePayments",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalPaymentCustomerCredit",
                table: "CarlErpReceivePayments",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalPaymentInvoice",
                table: "CarlErpReceivePayments",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalOpenBalanceCustomerCredit",
                table: "CarlErpReceivePayments",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPaymentCustomerCredit",
                table: "CarlErpReceivePayments",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPaymentDueCustomerCredit",
                table: "CarlErpReceivePayments",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPaymentInvoice",
                table: "CarlErpReceivePayments",
                type: "decimal(19,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<Guid>(
                name: "InvoiceId",
                table: "CarlErpReceivePaymentDeails",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerCreditId",
                table: "CarlErpReceivePaymentDeails",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CarlErpReceivePaymentExpense",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    ReceivePaymentId = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyAmount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpReceivePaymentExpense", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpReceivePaymentExpense_CarlErpChartOfAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpReceivePaymentExpense_CarlErpReceivePayments_ReceivePaymentId",
                        column: x => x.ReceivePaymentId,
                        principalTable: "CarlErpReceivePayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReceivePaymentDeails_CustomerCreditId",
                table: "CarlErpReceivePaymentDeails",
                column: "CustomerCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReceivePaymentExpense_AccountId",
                table: "CarlErpReceivePaymentExpense",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReceivePaymentExpense_ReceivePaymentId",
                table: "CarlErpReceivePaymentExpense",
                column: "ReceivePaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReceivePaymentExpense_TenantId_CreatorUserId",
                table: "CarlErpReceivePaymentExpense",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpReceivePaymentDeails_CarlErpCustomerCredits_CustomerCreditId",
                table: "CarlErpReceivePaymentDeails",
                column: "CustomerCreditId",
                principalTable: "CarlErpCustomerCredits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpReceivePaymentDeails_CarlErpCustomerCredits_CustomerCreditId",
                table: "CarlErpReceivePaymentDeails");

            migrationBuilder.DropTable(
                name: "CarlErpReceivePaymentExpense");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpReceivePaymentDeails_CustomerCreditId",
                table: "CarlErpReceivePaymentDeails");

            migrationBuilder.DropColumn(
                name: "Change",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyChange",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalPaymentCustomerCredit",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalPaymentInvoice",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "TotalOpenBalanceCustomerCredit",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "TotalPaymentCustomerCredit",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "TotalPaymentDueCustomerCredit",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "TotalPaymentInvoice",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "CustomerCreditId",
                table: "CarlErpReceivePaymentDeails");

            migrationBuilder.AlterColumn<Guid>(
                name: "InvoiceId",
                table: "CarlErpReceivePaymentDeails",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
