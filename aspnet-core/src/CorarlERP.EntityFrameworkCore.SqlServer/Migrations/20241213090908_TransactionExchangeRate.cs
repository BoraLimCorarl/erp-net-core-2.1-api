using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class TransactionExchangeRate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UseExchangeRate",
                table: "CarlErpVendorCredit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseExchangeRate",
                table: "CarlErpSaleOrders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalCashCustomerCredit",
                table: "CarlErpReceivePayments",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalCashInvoice",
                table: "CarlErpReceivePayments",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalCreditCustomerCredit",
                table: "CarlErpReceivePayments",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalCreditInvoice",
                table: "CarlErpReceivePayments",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalExpenseCustomerCredit",
                table: "CarlErpReceivePayments",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalExpenseInvoice",
                table: "CarlErpReceivePayments",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalOpenBalance",
                table: "CarlErpReceivePayments",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalOpenBalanceCustomerCredit",
                table: "CarlErpReceivePayments",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalPaymentDue",
                table: "CarlErpReceivePayments",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalPaymentDueCustomerCredit",
                table: "CarlErpReceivePayments",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "UseExchangeRate",
                table: "CarlErpReceivePayments",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLossGain",
                table: "CarlErpReceivePaymentExpense",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "CashInPaymentCurrency",
                table: "CarlErpReceivePaymentDeails",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CreditInPaymentCurrency",
                table: "CarlErpReceivePaymentDeails",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ExpenseInPaymentCurrency",
                table: "CarlErpReceivePaymentDeails",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LossGain",
                table: "CarlErpReceivePaymentDeails",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OpenBalanceInPaymentCurrency",
                table: "CarlErpReceivePaymentDeails",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PaymentInPaymentCurrency",
                table: "CarlErpReceivePaymentDeails",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmountInPaymentCurrency",
                table: "CarlErpReceivePaymentDeails",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "UseExchangeRate",
                table: "CarlErpPurchaseOrders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalOpenBalance",
                table: "CarlErpPayBills",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalOpenBalanceVendorCredit",
                table: "CarlErpPayBills",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalPaymentDue",
                table: "CarlErpPayBills",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MultiCurrencyTotalPaymentDueVendorCredit",
                table: "CarlErpPayBills",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "UseExchangeRate",
                table: "CarlErpPayBills",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLossGain",
                table: "CarlErpPayBillExpense",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "LossGain",
                table: "CarlErpPayBillDeail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OpenBalanceInPaymentCurrency",
                table: "CarlErpPayBillDeail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PaymentInPaymentCurrency",
                table: "CarlErpPayBillDeail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmountInPaymentCurrency",
                table: "CarlErpPayBillDeail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "UseExchangeRate",
                table: "CarlErpInvoices",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseExchangeRate",
                table: "CarlErpCustomerCredits",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseExchangeRate",
                table: "CarlErpBills",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseExchangeRate",
                table: "AbpTenants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CarlBillExchangeRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    BillId = table.Column<Guid>(nullable: false),
                    FromCurrencyId = table.Column<long>(nullable: false),
                    ToCurrencyId = table.Column<long>(nullable: false),
                    Bid = table.Column<decimal>(nullable: false),
                    Ask = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlBillExchangeRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlBillExchangeRates_CarlErpBills_BillId",
                        column: x => x.BillId,
                        principalTable: "CarlErpBills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlBillExchangeRates_CarlErpCurrencies_FromCurrencyId",
                        column: x => x.FromCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlBillExchangeRates_CarlErpCurrencies_ToCurrencyId",
                        column: x => x.ToCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlCustomerCreditExchangeRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    CustomerCreditId = table.Column<Guid>(nullable: false),
                    FromCurrencyId = table.Column<long>(nullable: false),
                    ToCurrencyId = table.Column<long>(nullable: false),
                    Bid = table.Column<decimal>(nullable: false),
                    Ask = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlCustomerCreditExchangeRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlCustomerCreditExchangeRates_CarlErpCustomerCredits_CustomerCreditId",
                        column: x => x.CustomerCreditId,
                        principalTable: "CarlErpCustomerCredits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlCustomerCreditExchangeRates_CarlErpCurrencies_FromCurrencyId",
                        column: x => x.FromCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlCustomerCreditExchangeRates_CarlErpCurrencies_ToCurrencyId",
                        column: x => x.ToCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlInvoiceExchangeRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    InvoiceId = table.Column<Guid>(nullable: false),
                    FromCurrencyId = table.Column<long>(nullable: false),
                    ToCurrencyId = table.Column<long>(nullable: false),
                    Bid = table.Column<decimal>(nullable: false),
                    Ask = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlInvoiceExchangeRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlInvoiceExchangeRates_CarlErpCurrencies_FromCurrencyId",
                        column: x => x.FromCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlInvoiceExchangeRates_CarlErpInvoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "CarlErpInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlInvoiceExchangeRates_CarlErpCurrencies_ToCurrencyId",
                        column: x => x.ToCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlPayBillExchangeRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    PayBillId = table.Column<Guid>(nullable: false),
                    FromCurrencyId = table.Column<long>(nullable: false),
                    ToCurrencyId = table.Column<long>(nullable: false),
                    Bid = table.Column<decimal>(nullable: false),
                    Ask = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlPayBillExchangeRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlPayBillExchangeRates_CarlErpCurrencies_FromCurrencyId",
                        column: x => x.FromCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlPayBillExchangeRates_CarlErpPayBills_PayBillId",
                        column: x => x.PayBillId,
                        principalTable: "CarlErpPayBills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlPayBillExchangeRates_CarlErpCurrencies_ToCurrencyId",
                        column: x => x.ToCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlPayBillItemExchangeRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    PayBillItemId = table.Column<Guid>(nullable: false),
                    FromCurrencyId = table.Column<long>(nullable: false),
                    ToCurrencyId = table.Column<long>(nullable: false),
                    Bid = table.Column<decimal>(nullable: false),
                    Ask = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlPayBillItemExchangeRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlPayBillItemExchangeRates_CarlErpCurrencies_FromCurrencyId",
                        column: x => x.FromCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlPayBillItemExchangeRates_CarlErpPayBillDeail_PayBillItemId",
                        column: x => x.PayBillItemId,
                        principalTable: "CarlErpPayBillDeail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlPayBillItemExchangeRates_CarlErpCurrencies_ToCurrencyId",
                        column: x => x.ToCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlPurchaseOrderExchangeRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    PurchaseOrderId = table.Column<Guid>(nullable: false),
                    FromCurrencyId = table.Column<long>(nullable: false),
                    ToCurrencyId = table.Column<long>(nullable: false),
                    Bid = table.Column<decimal>(nullable: false),
                    Ask = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlPurchaseOrderExchangeRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlPurchaseOrderExchangeRates_CarlErpCurrencies_FromCurrencyId",
                        column: x => x.FromCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlPurchaseOrderExchangeRates_CarlErpPurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "CarlErpPurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlPurchaseOrderExchangeRates_CarlErpCurrencies_ToCurrencyId",
                        column: x => x.ToCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlReceivePaymentExchangeRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ReceivePaymentId = table.Column<Guid>(nullable: false),
                    FromCurrencyId = table.Column<long>(nullable: false),
                    ToCurrencyId = table.Column<long>(nullable: false),
                    Bid = table.Column<decimal>(nullable: false),
                    Ask = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlReceivePaymentExchangeRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlReceivePaymentExchangeRates_CarlErpCurrencies_FromCurrencyId",
                        column: x => x.FromCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlReceivePaymentExchangeRates_CarlErpReceivePayments_ReceivePaymentId",
                        column: x => x.ReceivePaymentId,
                        principalTable: "CarlErpReceivePayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlReceivePaymentExchangeRates_CarlErpCurrencies_ToCurrencyId",
                        column: x => x.ToCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlReceivePaymentItemExchangeRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ReceivePaymentItemId = table.Column<Guid>(nullable: false),
                    FromCurrencyId = table.Column<long>(nullable: false),
                    ToCurrencyId = table.Column<long>(nullable: false),
                    Bid = table.Column<decimal>(nullable: false),
                    Ask = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlReceivePaymentItemExchangeRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlReceivePaymentItemExchangeRates_CarlErpCurrencies_FromCurrencyId",
                        column: x => x.FromCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlReceivePaymentItemExchangeRates_CarlErpReceivePaymentDeails_ReceivePaymentItemId",
                        column: x => x.ReceivePaymentItemId,
                        principalTable: "CarlErpReceivePaymentDeails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlReceivePaymentItemExchangeRates_CarlErpCurrencies_ToCurrencyId",
                        column: x => x.ToCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlSaleOrderExchangeRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    SaleOrderId = table.Column<Guid>(nullable: false),
                    FromCurrencyId = table.Column<long>(nullable: false),
                    ToCurrencyId = table.Column<long>(nullable: false),
                    Bid = table.Column<decimal>(nullable: false),
                    Ask = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlSaleOrderExchangeRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlSaleOrderExchangeRates_CarlErpCurrencies_FromCurrencyId",
                        column: x => x.FromCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlSaleOrderExchangeRates_CarlErpSaleOrders_SaleOrderId",
                        column: x => x.SaleOrderId,
                        principalTable: "CarlErpSaleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlSaleOrderExchangeRates_CarlErpCurrencies_ToCurrencyId",
                        column: x => x.ToCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlVendorCreditExchangeRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    VendorCreditId = table.Column<Guid>(nullable: false),
                    FromCurrencyId = table.Column<long>(nullable: false),
                    ToCurrencyId = table.Column<long>(nullable: false),
                    Bid = table.Column<decimal>(nullable: false),
                    Ask = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlVendorCreditExchangeRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlVendorCreditExchangeRates_CarlErpCurrencies_FromCurrencyId",
                        column: x => x.FromCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlVendorCreditExchangeRates_CarlErpCurrencies_ToCurrencyId",
                        column: x => x.ToCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlVendorCreditExchangeRates_CarlErpVendorCredit_VendorCreditId",
                        column: x => x.VendorCreditId,
                        principalTable: "CarlErpVendorCredit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlBillExchangeRates_BillId",
                table: "CarlBillExchangeRates",
                column: "BillId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlBillExchangeRates_FromCurrencyId",
                table: "CarlBillExchangeRates",
                column: "FromCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlBillExchangeRates_ToCurrencyId",
                table: "CarlBillExchangeRates",
                column: "ToCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlCustomerCreditExchangeRates_CustomerCreditId",
                table: "CarlCustomerCreditExchangeRates",
                column: "CustomerCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlCustomerCreditExchangeRates_FromCurrencyId",
                table: "CarlCustomerCreditExchangeRates",
                column: "FromCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlCustomerCreditExchangeRates_ToCurrencyId",
                table: "CarlCustomerCreditExchangeRates",
                column: "ToCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlInvoiceExchangeRates_FromCurrencyId",
                table: "CarlInvoiceExchangeRates",
                column: "FromCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlInvoiceExchangeRates_InvoiceId",
                table: "CarlInvoiceExchangeRates",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlInvoiceExchangeRates_ToCurrencyId",
                table: "CarlInvoiceExchangeRates",
                column: "ToCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlPayBillExchangeRates_FromCurrencyId",
                table: "CarlPayBillExchangeRates",
                column: "FromCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlPayBillExchangeRates_PayBillId",
                table: "CarlPayBillExchangeRates",
                column: "PayBillId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlPayBillExchangeRates_ToCurrencyId",
                table: "CarlPayBillExchangeRates",
                column: "ToCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlPayBillItemExchangeRates_FromCurrencyId",
                table: "CarlPayBillItemExchangeRates",
                column: "FromCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlPayBillItemExchangeRates_PayBillItemId",
                table: "CarlPayBillItemExchangeRates",
                column: "PayBillItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlPayBillItemExchangeRates_ToCurrencyId",
                table: "CarlPayBillItemExchangeRates",
                column: "ToCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlPurchaseOrderExchangeRates_FromCurrencyId",
                table: "CarlPurchaseOrderExchangeRates",
                column: "FromCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlPurchaseOrderExchangeRates_PurchaseOrderId",
                table: "CarlPurchaseOrderExchangeRates",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlPurchaseOrderExchangeRates_ToCurrencyId",
                table: "CarlPurchaseOrderExchangeRates",
                column: "ToCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlReceivePaymentExchangeRates_FromCurrencyId",
                table: "CarlReceivePaymentExchangeRates",
                column: "FromCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlReceivePaymentExchangeRates_ReceivePaymentId",
                table: "CarlReceivePaymentExchangeRates",
                column: "ReceivePaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlReceivePaymentExchangeRates_ToCurrencyId",
                table: "CarlReceivePaymentExchangeRates",
                column: "ToCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlReceivePaymentItemExchangeRates_FromCurrencyId",
                table: "CarlReceivePaymentItemExchangeRates",
                column: "FromCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlReceivePaymentItemExchangeRates_ReceivePaymentItemId",
                table: "CarlReceivePaymentItemExchangeRates",
                column: "ReceivePaymentItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlReceivePaymentItemExchangeRates_ToCurrencyId",
                table: "CarlReceivePaymentItemExchangeRates",
                column: "ToCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlSaleOrderExchangeRates_FromCurrencyId",
                table: "CarlSaleOrderExchangeRates",
                column: "FromCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlSaleOrderExchangeRates_SaleOrderId",
                table: "CarlSaleOrderExchangeRates",
                column: "SaleOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlSaleOrderExchangeRates_ToCurrencyId",
                table: "CarlSaleOrderExchangeRates",
                column: "ToCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlVendorCreditExchangeRates_FromCurrencyId",
                table: "CarlVendorCreditExchangeRates",
                column: "FromCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlVendorCreditExchangeRates_ToCurrencyId",
                table: "CarlVendorCreditExchangeRates",
                column: "ToCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlVendorCreditExchangeRates_VendorCreditId",
                table: "CarlVendorCreditExchangeRates",
                column: "VendorCreditId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlBillExchangeRates");

            migrationBuilder.DropTable(
                name: "CarlCustomerCreditExchangeRates");

            migrationBuilder.DropTable(
                name: "CarlInvoiceExchangeRates");

            migrationBuilder.DropTable(
                name: "CarlPayBillExchangeRates");

            migrationBuilder.DropTable(
                name: "CarlPayBillItemExchangeRates");

            migrationBuilder.DropTable(
                name: "CarlPurchaseOrderExchangeRates");

            migrationBuilder.DropTable(
                name: "CarlReceivePaymentExchangeRates");

            migrationBuilder.DropTable(
                name: "CarlReceivePaymentItemExchangeRates");

            migrationBuilder.DropTable(
                name: "CarlSaleOrderExchangeRates");

            migrationBuilder.DropTable(
                name: "CarlVendorCreditExchangeRates");

            migrationBuilder.DropColumn(
                name: "UseExchangeRate",
                table: "CarlErpVendorCredit");

            migrationBuilder.DropColumn(
                name: "UseExchangeRate",
                table: "CarlErpSaleOrders");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalCashCustomerCredit",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalCashInvoice",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalCreditCustomerCredit",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalCreditInvoice",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalExpenseCustomerCredit",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalExpenseInvoice",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalOpenBalance",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalOpenBalanceCustomerCredit",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalPaymentDue",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalPaymentDueCustomerCredit",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "UseExchangeRate",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "IsLossGain",
                table: "CarlErpReceivePaymentExpense");

            migrationBuilder.DropColumn(
                name: "CashInPaymentCurrency",
                table: "CarlErpReceivePaymentDeails");

            migrationBuilder.DropColumn(
                name: "CreditInPaymentCurrency",
                table: "CarlErpReceivePaymentDeails");

            migrationBuilder.DropColumn(
                name: "ExpenseInPaymentCurrency",
                table: "CarlErpReceivePaymentDeails");

            migrationBuilder.DropColumn(
                name: "LossGain",
                table: "CarlErpReceivePaymentDeails");

            migrationBuilder.DropColumn(
                name: "OpenBalanceInPaymentCurrency",
                table: "CarlErpReceivePaymentDeails");

            migrationBuilder.DropColumn(
                name: "PaymentInPaymentCurrency",
                table: "CarlErpReceivePaymentDeails");

            migrationBuilder.DropColumn(
                name: "TotalAmountInPaymentCurrency",
                table: "CarlErpReceivePaymentDeails");

            migrationBuilder.DropColumn(
                name: "UseExchangeRate",
                table: "CarlErpPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalOpenBalance",
                table: "CarlErpPayBills");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalOpenBalanceVendorCredit",
                table: "CarlErpPayBills");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalPaymentDue",
                table: "CarlErpPayBills");

            migrationBuilder.DropColumn(
                name: "MultiCurrencyTotalPaymentDueVendorCredit",
                table: "CarlErpPayBills");

            migrationBuilder.DropColumn(
                name: "UseExchangeRate",
                table: "CarlErpPayBills");

            migrationBuilder.DropColumn(
                name: "IsLossGain",
                table: "CarlErpPayBillExpense");

            migrationBuilder.DropColumn(
                name: "LossGain",
                table: "CarlErpPayBillDeail");

            migrationBuilder.DropColumn(
                name: "OpenBalanceInPaymentCurrency",
                table: "CarlErpPayBillDeail");

            migrationBuilder.DropColumn(
                name: "PaymentInPaymentCurrency",
                table: "CarlErpPayBillDeail");

            migrationBuilder.DropColumn(
                name: "TotalAmountInPaymentCurrency",
                table: "CarlErpPayBillDeail");

            migrationBuilder.DropColumn(
                name: "UseExchangeRate",
                table: "CarlErpInvoices");

            migrationBuilder.DropColumn(
                name: "UseExchangeRate",
                table: "CarlErpCustomerCredits");

            migrationBuilder.DropColumn(
                name: "UseExchangeRate",
                table: "CarlErpBills");

            migrationBuilder.DropColumn(
                name: "UseExchangeRate",
                table: "AbpTenants");
        }
    }
}
