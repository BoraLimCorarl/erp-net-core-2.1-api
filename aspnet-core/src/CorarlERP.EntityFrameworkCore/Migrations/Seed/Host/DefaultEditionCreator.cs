using System.Linq;
using Abp.Application.Features;
using Microsoft.EntityFrameworkCore;
using CorarlERP.Editions;
using CorarlERP.EntityFrameworkCore;
using CorarlERP.Features;

namespace CorarlERP.Migrations.Seed.Host
{
    public class DefaultEditionCreator
    {
        private readonly CorarlERPDbContext _context;

        public DefaultEditionCreator(CorarlERPDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            //CreateEditions();
            CreateSimpleEditions();
            CreateAdvanceEditions();
           
        }

        //private void CreateEditions()
        //{
        //    var defaultEdition = _context.Editions.IgnoreQueryFilters().FirstOrDefault(e => e.Name == EditionManager.DefaultEditionName);
        //    if (defaultEdition == null)
        //    {
        //        defaultEdition = new SubscribableEdition { Name = EditionManager.DefaultEditionName, DisplayName = EditionManager.DefaultEditionName };
        //        _context.Editions.Add(defaultEdition);
        //        _context.SaveChanges();

        //        /* Add desired features to the standard edition, if wanted... */
        //    }

        //    if (defaultEdition.Id > 0)
        //    {
        //        CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ChatFeature, true);
        //        CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.TenantToTenantChatFeature, true);
        //        CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.TenantToHostChatFeature, true);
        //    }
        //}

        private void CreateSimpleEditions()
        {
            var defaultEdition = _context.Editions.IgnoreQueryFilters().FirstOrDefault(e => e.Name == EditionManager.SimpleEditName);
            if (defaultEdition == null)
            {
                defaultEdition = new SubscribableEdition { Name = EditionManager.SimpleEditName, DisplayName = EditionManager.SimpleEditName };
                _context.Editions.Add(defaultEdition);
                _context.SaveChanges();

                /* Add desired features to the standard edition, if wanted... */
            }

            if (defaultEdition.Id > 0)
            {
                //CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ChatFeature, true);
                //CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.TenantToTenantChatFeature, true);
                //CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.TenantToHostChatFeature, true);

                //sample feature

                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.VendorsFeature, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.VendorsFeatureBills, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.VendorsFeaturePayBills, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.VendorsFeaturePurchases, true);

                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.CustomersFeature, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.CustomersFeatureInvoices, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.CustomersFeatureSaleOrders, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.CustomersFeatureReceivePayments, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.CustomersFeaturePOS, true);

                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.InventoryFeature, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.InventoryFeatureInventoryTransactions, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.InventoryFeaturePhysicalCounts, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.InventoryFeatureTransferOrders, true);

                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.BankFeature, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.BankFeatureBankTransaction, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.BankFeatureBankTransfer, true);

                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.SetupFeature, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.SetupFeatureChartOfAccounts, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.SetupFeatureClasss, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.SetupFeatureCompanyProfile, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.SetupFeatureCustomers, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.SetupFeatureItemProperties, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.SetupFeatureItems, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.SetupFeatureLocations, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.SetupFeatureTaxs, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.SetupFeatureVendors, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.SetupFeatureExchangeRate, true);

                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.AccountingFeature, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.AccountingFeatureClosePeriod, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.AccountingFeatureGeneralJournal, true);

                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeature, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.UserActivitesFeature, true);

                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.CommonFeature, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureJournal, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureLedger, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureCash, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureCashFlow, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureInventory, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureBalanceSheet, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureProfitAndLoss, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureInventoryValuationSummaryReport, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureInventoryValuationDetailReport, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureInventoryTransactionReport, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureStokBalanceReport, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureAssetBalanceReport, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureAccounting, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureCustomer, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureSaleInvoice, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureCustomerAging, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureCustomerByInvoice, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureVendor, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeaturePurchasing, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureVendorAging, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureVendorByBill, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeaturePurchaseOrder, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureSaleOrder, true);
            }
        }

        private void CreateAdvanceEditions()
        {
            var defaultEdition = _context.Editions.IgnoreQueryFilters().FirstOrDefault(e => e.Name == EditionManager.AdvanceEditName);
            if (defaultEdition == null)
            {
                defaultEdition = new SubscribableEdition { Name = EditionManager.AdvanceEditName, DisplayName = EditionManager.AdvanceEditName };
                _context.Editions.Add(defaultEdition);
                _context.SaveChanges();

                /* Add desired features to the standard edition, if wanted... */
            }

            if (defaultEdition.Id > 0)
            {
                //advance feature

                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.VendorsFeature, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.VendorsFeatureBills, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.VendorsFeatureItemReceipts, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.VendorsFeaturePayBills, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.VendorsFeaturePurchases, true);

                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.CustomersFeature, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.CustomersFeatureInvoices, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.CustomersFeatureItemIssues, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.CustomersFeatureSaleOrders, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.CustomersFeatureReceivePayments, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.CustomersFeaturePOS, true);

                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.InventoryFeature, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.InventoryFeatureInventoryTransactions, true);
                //CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.InventoryFeatureInventoryTransactionsItemIssues, true);
                //CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.InventoryFeatureInventoryTransactionsItemReceipts, true);

                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.InventoryFeaturePhysicalCounts, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.InventoryFeatureTransferOrders, true);

                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.BankFeature, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.BankFeatureBankTransaction, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.BankFeatureBankTransfer, true);

                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ProductionFeature, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ProductionFeatureProductionOrder, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ProductionFeatureProductionPlan, true);

                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.SetupFeature, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.SetupFeatureChartOfAccounts, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.SetupFeatureClasss, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.SetupFeatureCompanyProfile, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.SetupFeatureCustomers, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.SetupFeatureItemProperties, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.SetupFeatureItems, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.SetupFeatureLocations, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.SetupFeatureTaxs, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.SetupFeatureVendors, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.SetupFeatureExchangeRate, true);

                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.AccountingFeature, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.AccountingFeatureClosePeriod, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.AccountingFeatureGeneralJournal, true);

                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeature, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.UserActivitesFeature, true);


                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.CommonFeature, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureJournal, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureLedger, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureCash, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureCashFlow, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureInventory, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureBalanceSheet, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureProfitAndLoss, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureInventoryValuationSummaryReport, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureInventoryValuationDetailReport, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureInventoryTransactionReport, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureStokBalanceReport, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureAssetBalanceReport, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureAccounting, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureCustomer, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureSaleInvoice, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureCustomerAging, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureCustomerByInvoice, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureVendor, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeaturePurchasing, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureVendorAging, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureVendorByBill, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeaturePurchaseOrder, true);
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ReportFeatureSaleOrder, true);


            }
}

        private void CreateFeatureIfNotExists(int editionId, string featureName, bool isEnabled)
        {
            var defaultEditionChatFeature = _context.EditionFeatureSettings.IgnoreQueryFilters()
                                                        .FirstOrDefault(ef => ef.EditionId == editionId && ef.Name == featureName);

            if (defaultEditionChatFeature == null)
            {
                _context.EditionFeatureSettings.Add(new EditionFeatureSetting
                {
                    Name = featureName,
                    Value = isEnabled.ToString().ToLower(),
                    EditionId = editionId
                });
            }
        }
    }
}