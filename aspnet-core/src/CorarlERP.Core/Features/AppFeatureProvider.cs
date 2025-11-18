using Abp.Application.Features;
using Abp.Localization;
using Abp.Runtime.Validation;
using Abp.UI.Inputs;

namespace CorarlERP.Features
{
    public class AppFeatureProvider : FeatureProvider
    {
        public override void SetFeatures(IFeatureDefinitionContext context)
        {
            context.Create(
                AppFeatures.MaxUserCount,
                defaultValue: "0", //0 = unlimited
                displayName: L("MaximumUserCount"),
                description: L("MaximumUserCount_Description"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue))
            )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
            {
                ValueTextNormalizer = value => value == "0" ? L("Unlimited") : new FixedLocalizableString(value),
                IsVisibleOnPricingTable = true
            };


            context.Create(
               AppFeatures.MaxLocationCount,
               defaultValue: "0", //0 = unlimited
               displayName: L("MaximumLocationCount"),
               description: L("MaximumLocationCount_Description"),
               inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue))
           )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
           {
               ValueTextNormalizer = value => value == "0" ? L("Unlimited") : new FixedLocalizableString(value),
               IsVisibleOnPricingTable = true
           };


            context.Create(
               AppFeatures.MaxVendorCount,
               defaultValue: "0", //0 = unlimited
               displayName: L("MaximumVendorCount"),
               description: L("MaximumVendorCount_Description"),
               inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue))
           )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
           {
               ValueTextNormalizer = value => value == "0" ? L("Unlimited") : new FixedLocalizableString(value),
               IsVisibleOnPricingTable = true
           };

            context.Create(
                  AppFeatures.MaxCustomerCount,
                  defaultValue: "0", //0 = unlimited
                  displayName: L("MaximumCustomerCount"),
                  description: L("MaximumCustomerCount_Description"),
                  inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue))
              )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
              {
                  ValueTextNormalizer = value => value == "0" ? L("Unlimited") : new FixedLocalizableString(value),
                  IsVisibleOnPricingTable = true
              };

            context.Create(
               AppFeatures.MaxItemCount,
               defaultValue: "0", //0 = unlimited
               displayName: L("MaximumItemCount"),
               description: L("MaximumItemCount_Description"),
               inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue))
           )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
           {
               ValueTextNormalizer = value => value == "0" ? L("Unlimited") : new FixedLocalizableString(value),
               IsVisibleOnPricingTable = true
           };

            var cleanRoundingFeature = context.Create(
                AppFeatures.CleanRoundingFeature,
                defaultValue: "false",
                displayName: L("CleanRounding"),
                inputType: new CheckboxInputType()
            );

            var clearDefaultValueFeature = context.Create(
                AppFeatures.ClearDefaultValueFeature,
                defaultValue: "false",
                displayName: L("ClearDefaultValue"),
                inputType: new CheckboxInputType()
            );

            var changeJournalDateFeature = context.Create(
                AppFeatures.ChangeJournalDateFeature,
                defaultValue: "false",
                displayName: L("ChangeJournalDate"),
                inputType: new CheckboxInputType()
            );

            context.Create(
               AppFeatures.ReportFeatureAutoCalculation,
               defaultValue: "false",
               displayName: L("AutoCalculation"),
               inputType: new CheckboxInputType());

            #region ######## Example Features #########

            var ProductionFeature = context.Create(
                AppFeatures.ProductionFeature,
                defaultValue: "false",
                displayName: L("Productions"),
                inputType: new CheckboxInputType()
            );
            ProductionFeature.CreateChildFeature(
                AppFeatures.ProductionFeatureProductionOrder,
                defaultValue: "false",
                displayName: L("ProductionOrder"),
                inputType: new CheckboxInputType()
            );
            ProductionFeature.CreateChildFeature(
                AppFeatures.ProductionFeatureProductionProcess,
                defaultValue: "false",
                displayName: L("ProductionProcess"),
                inputType: new CheckboxInputType()
            );
            ProductionFeature.CreateChildFeature(
               AppFeatures.ProductionFeatureProductionPlan,
               defaultValue: "false",
               displayName: L("ProductionPlan"),
               inputType: new CheckboxInputType()
            );
            ProductionFeature.CreateChildFeature(
             AppFeatures.ProductionFeatureProductionLine,
             defaultValue: "false",
             displayName: L("ProductionLine"),
             inputType: new CheckboxInputType()
          );
            #region Vendors
            var vendorFeature = context.Create(
                AppFeatures.VendorsFeature,
                defaultValue: "false",
                displayName: L("Vendors"),
                inputType: new CheckboxInputType()
            );
            vendorFeature.CreateChildFeature(
                AppFeatures.VendorsFeaturePurchases,
                defaultValue: "false",
                displayName: L("PurchaseOrders"),
                inputType: new CheckboxInputType()
            );

            vendorFeature.CreateChildFeature(
                AppFeatures.VendorsFeatureBills,
                defaultValue: "false",
                displayName: L("Bills"),
                inputType: new CheckboxInputType()
            );
            vendorFeature.CreateChildFeature(
                AppFeatures.VendorsFeaturePayBills,
                defaultValue: "false",
                displayName: L("PayBills"),
                inputType: new CheckboxInputType()
            );
            vendorFeature.CreateChildFeature(
               AppFeatures.VendorsFeaturePurchasePrices,
               defaultValue: "false",
               displayName: L("PurchasePrice"),
               inputType: new CheckboxInputType()
           );

           var qcTestFeatureFeature = context.Create(
                 AppFeatures.QCTestFeature,
                 defaultValue: "false",
                 displayName: L("QCTest"),
                 inputType: new CheckboxInputType()
            );

            //vendorFeature.CreateChildFeature(
            //    AppFeatures.VendorsFeatureVendorCredits,
            //    defaultValue: "false",
            //    displayName: L("VendorCredit"),
            //    inputType: new CheckboxInputType()
            //);
            var itemReceipt = vendorFeature.CreateChildFeature(
                AppFeatures.VendorsFeatureItemReceipts,
                defaultValue: "false",
                displayName: L("ItemReceipts"),
                inputType: new CheckboxInputType()
            );

            //itemReceipt.CreateChildFeature(
            //    AppFeatures.VendorsFeatureItemReceipts_Other,
            //    defaultValue: "false",
            //    displayName: L("Other"),
            //    inputType: new CheckboxInputType()
            //);
            //itemReceipt.CreateChildFeature(
            //    AppFeatures.VendorsFeatureItemReceipts_CustomerCredit,
            //    defaultValue: "false",
            //    displayName: L("CustomerCredit"),
            //    inputType: new CheckboxInputType()
            //);
            //itemReceipt.CreateChildFeature(
            //    AppFeatures.VendorsFeatureItemReceipts_Transfer,
            //    defaultValue: "false",
            //    displayName: L("Transfer"),
            //    inputType: new CheckboxInputType()
            //);
            //itemReceipt.CreateChildFeature(
            //    AppFeatures.VendorsFeatureItemReceipts_Adjustment,
            //    defaultValue: "false",
            //    displayName: L("Adjustment"),
            //    inputType: new CheckboxInputType()
            //);
            //itemReceipt.CreateChildFeature(
            //  AppFeatures.CustomersFeatureItemReceipts_ProductionOrder,
            //  defaultValue: "false",
            //  displayName: L("Production"),
            //  inputType: new CheckboxInputType()
            //);
            #endregion

            #region POS
            context.Create(
               AppFeatures.CustomersFeaturePOS,
               defaultValue: "false",
               displayName: L("POS"),
               inputType: new CheckboxInputType()
            );
            #endregion

            #region customers
            var customerFeature = context.Create(
                AppFeatures.CustomersFeature,
                defaultValue: "false",
                displayName: L("Customers"),
                inputType: new CheckboxInputType()
            );
            customerFeature.CreateChildFeature(
                AppFeatures.CustomersFeatureSaleOrders,
                defaultValue: "false",
                displayName: L("SaleOrders"),
                inputType: new CheckboxInputType()
            );
            customerFeature.CreateChildFeature(
                 AppFeatures.CustomersFeatureDeliverySchedule,
                defaultValue: "false",
                displayName: L("DeliverySchedule"),
                inputType: new CheckboxInputType()
                );
            customerFeature.CreateChildFeature(
                AppFeatures.CustomersFeatureInvoices,
                defaultValue: "false",
                displayName: L("Invoices"),
                inputType: new CheckboxInputType()
            );
            customerFeature.CreateChildFeature(
                AppFeatures.CustomersFeatureReceivePayments,
                defaultValue: "false",
                displayName: L("ReceivePayments"),
                inputType: new CheckboxInputType()
            );
            customerFeature.CreateChildFeature(
               AppFeatures.CustomerFeatureLayaltyAndMembership,
               defaultValue: "false",
               displayName: L("LayaltyAndMemberships"),
               inputType: new CheckboxInputType()
           );
            //customerFeature.CreateChildFeature(
            //    AppFeatures.CustomersFeatureCustomerCredits,
            //    defaultValue: "false",
            //    displayName: L("CustomerCredit"),
            //    inputType: new CheckboxInputType()
            //);
            var itemIssue = customerFeature.CreateChildFeature(
                AppFeatures.CustomersFeatureItemIssues,
                defaultValue: "false",
                displayName: L("ItemIssues"),
                inputType: new CheckboxInputType()
            );
            //itemIssue.CreateChildFeature(
            //    AppFeatures.CustomersFeatureItemIssues_Transfer,
            //    defaultValue: "false",
            //    displayName: L("Transfer"),
            //    inputType: new CheckboxInputType()
            //);
            //itemIssue.CreateChildFeature(
            //    AppFeatures.CustomersFeatureItemIssues_Other,
            //    defaultValue: "false",
            //    displayName: L("Other"),
            //    inputType: new CheckboxInputType()
            //);
            //itemIssue.CreateChildFeature(
            //    AppFeatures.CustomersFeatureItemIssues_Adjustment,
            //    defaultValue: "false",
            //    displayName: L("Adjustment"),
            //    inputType: new CheckboxInputType()
            //);
            //itemIssue.CreateChildFeature(
            //    AppFeatures.CustomersFeatureItemIssues_VendorCredit,
            //    defaultValue: "false",
            //    displayName: L("VendorCredit"),
            //    inputType: new CheckboxInputType()
            //);
            //itemIssue.CreateChildFeature(
            //    AppFeatures.CustomersFeatureItemIssues_ProductionOrder,
            //    defaultValue: "false",
            //    displayName: L("Production"),
            //    inputType: new CheckboxInputType()
            //);
            #endregion

            #region inventory transaction 
            var inventoryFeature = context.Create(
                AppFeatures.InventoryFeature,
                defaultValue: "false",
                displayName: L("Inventory"),
                inputType: new CheckboxInputType()
            );
            var subInventory = inventoryFeature.CreateChildFeature(
                AppFeatures.InventoryFeatureInventoryTransactions,
                defaultValue: "false",
                displayName: L("InventoryTransactions"),
                inputType: new CheckboxInputType()
            );

            //subInventory.CreateChildFeature(
            //    AppFeatures.InventoryFeatureInventoryTransactionsItemIssues,
            //    defaultValue: "false",
            //    displayName: L("ItemIssueInventorySale"),
            //    inputType: new CheckboxInputType()
            //);
            //subInventory.CreateChildFeature(
            //    AppFeatures.InventoryFeatureInventoryTransactionsItemReceipts,
            //    defaultValue: "false",
            //    displayName: L("ItemReceiptInventoryPurchase"),
            //    inputType: new CheckboxInputType()
            //);

            inventoryFeature.CreateChildFeature(
                AppFeatures.InventoryFeatureTransferOrders,
                defaultValue: "false",
                displayName: L("TransferOrders"),
                inputType: new CheckboxInputType()
            );

            inventoryFeature.CreateChildFeature(
              AppFeatures.InventoryFeatureKitchenOrders,
              defaultValue: "false",
              displayName: L("KitchenOrder"),
              inputType: new CheckboxInputType()
          );

            inventoryFeature.CreateChildFeature(
                AppFeatures.InventoryFeaturePhysicalCounts,
                defaultValue: "false",
                displayName: L("PhysicalCounts"),
                inputType: new CheckboxInputType()
            );
            #endregion

            #region Bank Transaction 
            var bankFeature = context.Create(
               AppFeatures.BankFeature,
               defaultValue: "false",
               displayName: L("Bank"),
               inputType: new CheckboxInputType()
            );
            bankFeature.CreateChildFeature(
                AppFeatures.BankFeatureBankTransaction,
                defaultValue: "false",
                displayName: L("BankTransactions"),
                inputType: new CheckboxInputType()
            );
            bankFeature.CreateChildFeature(
                AppFeatures.BankFeatureBankTransfer,
                defaultValue: "false",
                displayName: L("BankTransfer"),
                inputType: new CheckboxInputType()
            );
            #endregion



            #region Commons Feature 
            var commonFeature = context.Create(
               AppFeatures.CommonFeature,
               defaultValue: "false",
               displayName: L("Commons"),
               inputType: new CheckboxInputType()
            );

            #endregion


            #region Setup
            var setupFeature = context.Create(
               AppFeatures.SetupFeature,
               defaultValue: "false",
               displayName: L("Setup"),
               inputType: new CheckboxInputType()
           );
            setupFeature.CreateChildFeature(
                AppFeatures.SetupFeatureChartOfAccounts,
                defaultValue: "false",
                displayName: L("ChartOfAccounts"),
                inputType: new CheckboxInputType()
            );
            setupFeature.CreateChildFeature(
                AppFeatures.SetupFeatureTaxs,
                defaultValue: "false",
                displayName: L("Taxs"),
                inputType: new CheckboxInputType()
            );

            setupFeature.CreateChildFeature(
                AppFeatures.SetupFeatureItemProperties,
                defaultValue: "false",
                displayName: L("ItemProperties"),
                inputType: new CheckboxInputType()
            );
            setupFeature.CreateChildFeature(
               AppFeatures.SetupFeatureItemPrices,
               defaultValue: "false",
               displayName: L("ItemPrices"),
               inputType: new CheckboxInputType()
           );
            setupFeature.CreateChildFeature(
               AppFeatures.SetupFeatureExchangeRate,
               defaultValue: "false",
               displayName: L("ExchangeRate"),
               inputType: new CheckboxInputType()
           );
            setupFeature.CreateChildFeature(
                AppFeatures.SetupFeatureVendors,
                defaultValue: "false",
                displayName: L("Vendors"),
                inputType: new CheckboxInputType()
            );
            setupFeature.CreateChildFeature(
                AppFeatures.SetupFeatureCustomers,
                defaultValue: "false",
                displayName: L("Customers"),
                inputType: new CheckboxInputType()
            );
            setupFeature.CreateChildFeature(
                AppFeatures.SetupFeatureClasss,
                defaultValue: "false",
                displayName: L("Class"),
                inputType: new CheckboxInputType()
            );
            setupFeature.CreateChildFeature(
                AppFeatures.SetupFeatureLocations,
                defaultValue: "false",
                displayName: L("Locations"),
                inputType: new CheckboxInputType()
            );
            var companyProfile = setupFeature.CreateChildFeature(
                 AppFeatures.SetupFeatureCompanyProfile,
                 defaultValue: "false",
                 displayName: L("CompanyProfile"),
                 inputType: new CheckboxInputType()
             );
            companyProfile.CreateChildFeature(
              AppFeatures.CompanyProfileFeatureMultiCurrencies,
              defaultValue: "false",
              displayName: L("MultiCurrency"),
              inputType: new CheckboxInputType()
          );

            var itemFeatures = setupFeature.CreateChildFeature(
                AppFeatures.SetupFeatureItems,
                defaultValue: "false",
                displayName: L("Items"),
                inputType: new CheckboxInputType()
            );

            itemFeatures.CreateChildFeature(
               AppFeatures.ItemFeatureTrackSerials,
               defaultValue: "false",
               displayName: L("TrackSerial"),
               inputType: new CheckboxInputType()
           );

            itemFeatures.CreateChildFeature(
              AppFeatures.ItemFeatureTrackExpiations,
              defaultValue: "false",
              displayName: L("TrackExpiation"),
              inputType: new CheckboxInputType()
          );
            itemFeatures.CreateChildFeature(
                  AppFeatures.ItemFeatureTrackBatchs,
                  defaultValue: "false",
                  displayName: L("TrackBatch"),
                  inputType: new CheckboxInputType()
              );
            itemFeatures.CreateChildFeature(
              AppFeatures.ItemFeatureTrackReorderPoints,
              defaultValue: "false",
              displayName: L("TrackReorderPoint"),
              inputType: new CheckboxInputType()
          );

            #endregion

            #region Accounting
            var accountingFeature = context.Create(
               AppFeatures.AccountingFeature,
               defaultValue: "false",
               displayName: L("Accounting"),
               inputType: new CheckboxInputType()
            );
            accountingFeature.CreateChildFeature(
                AppFeatures.AccountingFeatureGeneralJournal,
                defaultValue: "false",
                displayName: L("GeneralJournal"),
                inputType: new CheckboxInputType()
            );
            accountingFeature.CreateChildFeature(
                AppFeatures.AccountingFeatureClosePeriod,
                defaultValue: "false",
                displayName: L("LockTransaction"),
                inputType: new CheckboxInputType()
            );
            #endregion

            #region close Period
            var closePeriodFeature = context.Create(
              AppFeatures.ClosePeriodFeature,
              defaultValue: "false",
              displayName: L("ClosePeriod"),
              inputType: new CheckboxInputType()
           );
            #endregion

            #region UserActivities
            var userActivitiesFeature = context.Create(
               AppFeatures.UserActivitesFeature,
               defaultValue: "false",
               displayName: L("UserActivities"),
               inputType: new CheckboxInputType()
            );

            #endregion


            #region Report
            var reportFeature = context.Create(
              AppFeatures.ReportFeature,
              defaultValue: "false",
              displayName: L("Report"),
              inputType: new CheckboxInputType()
            );

            reportFeature.CreateChildFeature(
                AppFeatures.ReportFeatureQCTest,
                defaultValue: "false",
                displayName: L("QCTest"),
                inputType: new CheckboxInputType());

            var reportPurchaseOrderFeature = reportFeature.CreateChildFeature(
                AppFeatures.ReportFeaturePurchaseOrder,
                defaultValue: "false",
                displayName: L("PurchaseOrder"),
                inputType: new CheckboxInputType());

            var reportSaleOrderFeature = reportFeature.CreateChildFeature(
               AppFeatures.ReportFeatureSaleOrder,
               defaultValue: "false",
               displayName: L("SaleOrder"),
               inputType: new CheckboxInputType());

            var reportDeliveryScheduleFeature = reportFeature.CreateChildFeature(
              AppFeatures.ReportFeatureDeliverySchedule,
              defaultValue: "false",
              displayName: L("DeliverySchedule"),
              inputType: new CheckboxInputType());

            var reportAccounting = reportFeature.CreateChildFeature(
               AppFeatures.ReportFeatureAccounting,
               defaultValue: "false",
               displayName: L("Accounting"),
               inputType: new CheckboxInputType()
           );

            reportAccounting.CreateChildFeature(
                AppFeatures.ReportFeatureJournal,
                defaultValue: "false",
                displayName: L("Journal"),
                inputType: new CheckboxInputType()
            );
            reportAccounting.CreateChildFeature(
                AppFeatures.ReportFeatureBalanceSheet,
                defaultValue: "false",
                displayName: L("BalanceSheet"),
                inputType: new CheckboxInputType()
            );
            reportAccounting.CreateChildFeature(
               AppFeatures.ReportFeatureTrialBalance,
               defaultValue: "false",
               displayName: L("TrialBalance"),
               inputType: new CheckboxInputType()
           );
            reportAccounting.CreateChildFeature(
                AppFeatures.ReportFeatureProfitAndLoss,
                defaultValue: "false",
                displayName: L("ProfitAndLoss"),
                inputType: new CheckboxInputType()
            );

            reportAccounting.CreateChildFeature(
               AppFeatures.ReportFeatureLedger,
               defaultValue: "false",
               displayName: L("Ledger"),
               inputType: new CheckboxInputType()
           );

            reportAccounting.CreateChildFeature(
               AppFeatures.ReportFeatureCash,
               defaultValue: "false",
               displayName: L("CashReport"),
               inputType: new CheckboxInputType()
             );

            reportAccounting.CreateChildFeature(
                AppFeatures.ReportFeatureCashFlow,
                defaultValue: "false",
                displayName: L("CashFlowReport"),
                inputType: new CheckboxInputType()
            );

            var reportInventory = reportFeature.CreateChildFeature(
                AppFeatures.ReportFeatureInventory,
                defaultValue: "false",
                displayName: L("Inventory"),
                inputType: new CheckboxInputType()
            );

            reportInventory.CreateChildFeature(
               AppFeatures.ReportFeatureInventoryValuationSummaryReport,
               defaultValue: "false",
               displayName: L("InventoryValuationSummary"),
               inputType: new CheckboxInputType()
           );

            reportInventory.CreateChildFeature(
               AppFeatures.ReportFeatureInventoryTransactionReport,
               defaultValue: "false",
               displayName: L("InventoryTransaction"),
               inputType: new CheckboxInputType()
           );

            reportInventory.CreateChildFeature(
               AppFeatures.ReportFeatureInventoryValuationDetailReport,
               defaultValue: "false",
               displayName: L("InventoryValuationDetail"),
               inputType: new CheckboxInputType()
           );
            reportInventory.CreateChildFeature(
              AppFeatures.ReportFeatureStokBalanceReport,
              defaultValue: "false",
              displayName: L("StokBalance"),
              inputType: new CheckboxInputType()
          );

            reportInventory.CreateChildFeature(
                AppFeatures.ReportFeatureAssetBalanceReport,
                defaultValue: "false",
                displayName: L("AssetBalance"),
                inputType: new CheckboxInputType()
            );

            reportInventory.CreateChildFeature(
               AppFeatures.ReportFeatureTraceabilityReport,
               defaultValue: "false",
               displayName: L("BatchNoTraceability"),
               inputType: new CheckboxInputType()
           );

            reportInventory.CreateChildFeature(
               AppFeatures.ReportFeatureBatchNoBalanceReport,
               defaultValue: "false",
               displayName: L("BatchNoBalance"),
               inputType: new CheckboxInputType()
           );

           var productionReport = reportFeature.CreateChildFeature(
             AppFeatures.ReportFeatureProductionReport,
             defaultValue: "false",
             displayName: L("Production"),
             inputType: new CheckboxInputType()
            );

            productionReport.CreateChildFeature(
                AppFeatures.ReportFeatureProductionPlanReport,
                defaultValue: "false",
                displayName: L("ProductionPlan"),
                inputType: new CheckboxInputType()
            );

            productionReport.CreateChildFeature(
              AppFeatures.ReportFeatureProductionOrderReport,
              defaultValue: "false",
              displayName: L("ProductionOrder"),
              inputType: new CheckboxInputType()
          );

            var reportCustomer = reportFeature.CreateChildFeature(
              AppFeatures.ReportFeatureCustomer,
              defaultValue: "false",
              displayName: L("Customer"),
              inputType: new CheckboxInputType()
          );

            reportCustomer.CreateChildFeature(
               AppFeatures.ReportFeatureCustomerAging,
               defaultValue: "false",
               displayName: L("CustomerAging"),
               inputType: new CheckboxInputType()
           );

            reportCustomer.CreateChildFeature(
               AppFeatures.ReportFeatureCustomerByInvoice,
               defaultValue: "false",
               displayName: L("CustomerByInvoice"),
               inputType: new CheckboxInputType()
           );

            reportCustomer.CreateChildFeature(
              AppFeatures.ReportFeatureCustomerByInvoiceWithPayment,
              defaultValue: "false",
              displayName: L("ARByInvoiceWithPayment"),
              inputType: new CheckboxInputType()
            );

            reportCustomer.CreateChildFeature(
               AppFeatures.ReportFeatureSaleInvoice,
               defaultValue: "false",
               displayName: L("SaleInvoice"),
               inputType: new CheckboxInputType()
           );
            reportCustomer.CreateChildFeature(
            AppFeatures.ReportFeatureProfitByInvoice,
            defaultValue: "false",
            displayName: L("ProfitByInvoice"),
            inputType: new CheckboxInputType()
        );

            var reportVendor = reportFeature.CreateChildFeature(
            AppFeatures.ReportFeatureVendor,
            defaultValue: "false",
            displayName: L("Vendor"),
            inputType: new CheckboxInputType()
        );

            reportVendor.CreateChildFeature(
               AppFeatures.ReportFeatureVendorAging,
               defaultValue: "false",
               displayName: L("VendorAging"),
               inputType: new CheckboxInputType()
           );

            reportVendor.CreateChildFeature(
               AppFeatures.ReportFeatureVendorByBill,
               defaultValue: "false",
               displayName: L("VendorByBill"),
               inputType: new CheckboxInputType()
           );

            reportVendor.CreateChildFeature(
               AppFeatures.ReportFeaturePurchasing,
               defaultValue: "false",
               displayName: L("Purchasing"),
               inputType: new CheckboxInputType()
           );

            #endregion
            //context.Create("TestTenantScopeFeature", "false", L("TestTenantScopeFeature"), scope: FeatureScopes.Tenant);
            //context.Create("TestEditionScopeFeature", "false", L("TestEditionScopeFeature"), scope: FeatureScopes.Edition);

            //context.Create(
            //    AppFeatures.TestCheckFeature,
            //    defaultValue: "false",
            //    displayName: L("TestCheckFeature"),
            //    inputType: new CheckboxInputType()
            //)[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
            //{
            //    IsVisibleOnPricingTable = true,
            //    TextHtmlColor = value => value == "true" ? "#5cb85c" : "#d9534f"
            //};

            //context.Create(
            //    AppFeatures.TestCheckFeature2,
            //    defaultValue: "true",
            //    displayName: L("TestCheckFeature2"),
            //    inputType: new CheckboxInputType()
            //)[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
            //{
            //    IsVisibleOnPricingTable = true,
            //    TextHtmlColor = value => value == "true" ? "#5cb85c" : "#d9534f"
            //};

            #endregion

            var chatFeature = context.Create(
                AppFeatures.ChatFeature,
                defaultValue: "false",
                displayName: L("ChatFeature"),
                inputType: new CheckboxInputType()
            );

            chatFeature.CreateChildFeature(
                AppFeatures.TenantToTenantChatFeature,
                defaultValue: "false",
                displayName: L("TenantToTenantChatFeature"),
                inputType: new CheckboxInputType()
            );

            chatFeature.CreateChildFeature(
                AppFeatures.TenantToHostChatFeature,
                defaultValue: "false",
                displayName: L("TenantToHostChatFeature"),
                inputType: new CheckboxInputType()
            );
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, CorarlERPConsts.LocalizationSourceName);
        }
    }
}
