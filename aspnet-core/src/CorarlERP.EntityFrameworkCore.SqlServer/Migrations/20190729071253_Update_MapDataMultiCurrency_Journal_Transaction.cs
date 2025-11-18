using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class Update_MapDataMultiCurrency_Journal_Transaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(
                @"
                    UPDATE CarlErpJournals
                    SET MultiCurrencyId = CurrencyId
                ");

                // update path of vendor 
                migrationBuilder.Sql(
                @"
                    UPDATE CarlErpBills
                    SET 
                        MultiCurrencySubTotal = SubTotal,
                        MultiCurrencyTax = Tax,
                        MultiCurrencyTotal = Total,
                        MultiCurrencyOpenBalance = OpenBalance,
                        MultiCurrencyTotalPaid = TotalPaid
                ");

                migrationBuilder.Sql(
                @"
                    UPDATE CarlErpBillItems
                    SET 
                        MultiCurrencyTotal = Total,
                        MultiCurrencyUnitCost = UnitCost
                ");

                migrationBuilder.Sql(
                @"
                    UPDATE CarlErpVendorCredit
                    SET 
                        MultiCurrencySubTotal = SubTotal,
                        MultiCurrencyTax = Tax,
                        MultiCurrencyTotal = Total,
                        MultiCurrencyOpenBalance = OpenBalance,
                        MultiCurrencyTotalPaid = TotalPaid
                ");
                migrationBuilder.Sql(
                @"
                    UPDATE CarlErpVendorCreditDetails
                    SET 
                        MultiCurrencyTotal = Total,
                        MultiCurrencyUnitCost = UnitCost
                ");


                migrationBuilder.Sql(
                @"
                    UPDATE CarlErpPayBills
                    SET 
                        MultiCurrencyTotalPayment = TotalPayment
                ");

                migrationBuilder.Sql(
                @"
                    UPDATE CarlErpPayBillDeail
                    SET 
                        MultiCurrencyOpenBalance = OpenBalance,
                        MultiCurrencyPayment = Payment,
                        MultiCurrencyTotalAmount = TotalAmount
                ");

                // update path of customer
                migrationBuilder.Sql(
                @"
                    UPDATE CarlErpInvoices
                    SET 
                        MultiCurrencySubTotal = SubTotal,
                        MultiCurrencyTax = Tax,
                        MultiCurrencyTotal = Total,
                        MultiCurrencyOpenBalance = OpenBalance,
                        MultiCurrencyTotalPaid = TotalPaid
                ");

                migrationBuilder.Sql(
                @"
                    UPDATE CarlErpInvoiceItems
                    SET 
                        MultiCurrencyTotal = Total,
                        MultiCurrencyUnitCost = UnitCost
                ");
                migrationBuilder.Sql(
                @"
                    UPDATE CarlErpCustomerCredits
                    SET 
                        MultiCurrencySubTotal = SubTotal,
                        MultiCurrencyTax = Tax,
                        MultiCurrencyTotal = Total,
                        MultiCurrencyOpenBalance = OpenBalance,
                        MultiCurrencyTotalPaid = TotalPaid
                ");
                migrationBuilder.Sql(
                @"
                    UPDATE CarlErpCusotmerCreditDetails
                    SET 
                        MultiCurrencyTotal = Total,
                        MultiCurrencyUnitCost = UnitCost
                ");

                migrationBuilder.Sql(
                @"
                    UPDATE CarlErpReceivePayments
                    SET 
                        MultiCurrencyTotalPayment = TotalPayment
                ");

                migrationBuilder.Sql(
                @"
                    UPDATE CarlErpReceivePaymentDeails
                    SET 
                        MultiCurrencyOpenBalance = OpenBalance,
                        MultiCurrencyPayment = Payment,
                        MultiCurrencyTotalAmount = TotalAmount
                ");
            }

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
