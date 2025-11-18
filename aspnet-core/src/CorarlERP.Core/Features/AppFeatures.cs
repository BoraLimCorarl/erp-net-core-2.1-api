namespace CorarlERP.Features
{
    public static class AppFeatures
    {
        public const string MaxLocationCount = "App.MaxLocationCount";
        public const string MaxItemCount = "App.MaxItemCount";
        public const string MaxUserCount = "App.MaxUserCount";
        public const string ChatFeature = "App.ChatFeature";
        public const string MaxVendorCount = "App.MaxVendorCount";
        public const string MaxCustomerCount = "App.MaxCustomerCount";
        public const string TenantToTenantChatFeature = "App.ChatFeature.TenantToTenant";
        public const string TenantToHostChatFeature = "App.ChatFeature.TenantToHost";
        //public const string TestCheckFeature = "App.TestCheckFeature";
        //public const string TestCheckFeature2 = "App.TestCheckFeature2";

        public const string VendorsFeature = "App.VendorsFeature";
        public const string VendorsFeaturePurchases = "App.VendorsFeaturePurchaseOrders";
        public const string VendorsFeatureItemReceipts = "App.VendorsFeatureItemReceipt";
        //public const string VendorsFeatureItemReceipts_Transfer = "App.VendorsFeatureItemReceipt_Transfer";
        //public const string VendorsFeatureItemReceipts_Other = "App.VendorsFeatureItemReceipt_Other";
        //public const string VendorsFeatureItemReceipts_Adjustment = "App.VendorsFeatureItemReceipt_Adjustment";
        //public const string VendorsFeatureItemReceipts_CustomerCredit = "App.VendorsFeatureItemReceipt_CustomerCredit";
        public const string VendorsFeatureBills = "App.VendorsFeatureBills";
        //public const string VendorsFeatureVendorCredits = "App.VendorsFeatureVendorCredits";
        public const string VendorsFeaturePayBills = "App.VendorsFeaturePayBills";
        public const string VendorsFeaturePurchasePrices = "App.VendorsFeaturePurchasePrices";
        
        public const string CleanRoundingFeature = "App.CleanRoundingFeature";
        public const string ClearDefaultValueFeature = "App.ClearDefaultValueFeature";
        public const string ChangeJournalDateFeature = "App.ChangeJournalDateFeature";

       
        public const string CustomersFeature = "App.CustomersFeature";
        public const string CustomersFeatureSaleOrders = "App.CustomersFeatureSaleOrders";
        public const string CustomersFeatureInvoices = "App.CustomersFeatureInvoices";
        public const string CustomersFeatureItemIssues = "App.CustomersFeatureItemIssues";
        public const string CustomersFeatureReceivePayments = "App.CustomersFeatureReceivePayments";
        public const string CustomersFeaturePOS = "App.CustomersFeaturePOS";
        public const string CustomerFeatureLayaltyAndMembership = "App.CustomerFeatureLayaltyAndMembership";
        public const string CustomersFeatureDeliverySchedule = "App.CustomersFeatureDeliverySchedule";

        public const string ProductionFeature = "App.ProductionFeature";
        public const string ProductionFeatureProductionOrder = "App.ProductionFeatureProductionOrder";
        public const string ProductionFeatureProductionProcess = "App.ProductionFeatureProductionProcess";
        public const string ProductionFeatureProductionPlan = "App.ProductionFeatureProductionPlan";
        public const string ProductionFeatureProductionLine = "App.ProductionFeatureProductionLine";

        public const string InventoryFeature = "App.InventoryFeature";
        public const string InventoryFeatureInventoryTransactions = "App.CustomersFeatureInventoryTransactions";
        //public const string InventoryFeatureInventoryTransactionsItemIssues = "App.InventoryFeatureItemIssues";
        //public const string InventoryFeatureInventoryTransactionsItemReceipts = "App.InventoryFeatureItemReceipt";
        
        public const string InventoryFeatureTransferOrders = "App.CustomersFeatureTransferOrders";
        public const string InventoryFeaturePhysicalCounts = "App.CustomersFeaturePhysicalCounts";
        public const string InventoryFeatureKitchenOrders = "App.InventoryFeatureKitchenOrders";

        public const string BankFeature = "App.BankFeature";
        public const string CommonFeature = "App.CommonFeature";
        public const string BankFeatureBankTransaction = "App.BankFeatureBankTransactions";
        public const string BankFeatureBankTransfer = "App.BankFeatureBankTransfer";

        public const string SetupFeature = "App.SetupFeature";
        public const string SetupFeatureChartOfAccounts = "App.SetupFeatureChartOfAccounts";
        public const string SetupFeatureTaxs = "App.SetupFeatureTaxs";
        public const string SetupFeatureItems = "App.SetupFeatureItems";
        public const string SetupFeatureItemProperties = "App.SetupFeatureItemProperties";
        public const string SetupFeatureVendors = "App.SetupFeatureVendors";
        public const string SetupFeatureCustomers = "App.SetupFeatureCustomers";
        public const string SetupFeatureClasss = "App.SetupFeatureClass";
        public const string SetupFeatureLocations = "App.SetupFeatureLocations";
        public const string SetupFeatureCompanyProfile = "App.SetupFeatureCompanyProfile";
        public const string SetupFeatureExchangeRate = "App.SetupFeatureExchangeRate";
        
        public const string SetupFeatureItemPrices = "App.SetupFeatureItemPrices";
        public const string ItemFeatureTrackSerials = "App.TrackSerials";
        public const string ItemFeatureTrackExpiations = "App.SetupFeatureTrackExpiations";
        public const string ItemFeatureTrackBatchs = "App.SetupFeatureTrackBatchs";
        public const string ItemFeatureTrackReorderPoints = "App.TrackReorderPoints";
        public const string CompanyProfileFeatureMultiCurrencies = "App.SetupFeatureMultiCurrencies";

        public const string QCTestFeature = "App.QCTestFeature";


        public const string AccountingFeature = "App.AccountingFeature";
        public const string AccountingFeatureGeneralJournal = "App.AccountingFeatureGeneralJournal";
        public const string AccountingFeatureClosePeriod = "App.AccountingFeatureClosePeriod";

        public const string ClosePeriodFeature = "App.ClosePeriodFeature";

        public const string ReportFeature = "App.ReportFeature";
        public const string ReportFeatureAutoCalculation = "App.ReportFeature.AutoCalculation";

        public const string UserActivitesFeature = "App.UserActivitiesFeature";

        public const string ReportFeatureJournal = "App.ReportFeatureJournal";
        public const string ReportFeatureLedger = "App.ReportFeatureLedger";
        public const string ReportFeatureInventory = "App.ReportFeatureInventory";
        public const string ReportFeatureBalanceSheet = "App.ReportFeatureBalanceSheet";
        public const string ReportFeatureTrialBalance = "App.ReportFeatureTrialBalance";
        public const string ReportFeatureProfitAndLoss = "App.ReportFeatureProfitAndLoss";
        public const string ReportFeatureCash = "App.ReportFeatureCash";
        public const string ReportFeatureCashFlow = "App.ReportFeatureCashFlow";
        public const string ReportFeaturePurchaseOrder = "App.ReportFeaturePurchaseOrder";
        public const string ReportFeatureSaleOrder = "App.ReportFeatureSaleOrder";
        public const string ReportFeatureDeliverySchedule = "App.ReportFeatureDeliverySchedule";

        public const string ReportFeatureInventoryValuationSummaryReport = "App.ReportFeatureInventoryValuationSummaryReport";
        public const string ReportFeatureInventoryValuationDetailReport = "App.ReportFeatureInventoryValuationDetailReport";
        public const string ReportFeatureInventoryTransactionReport = "App.ReportFeatureInventoryTransactionReport";
        public const string ReportFeatureStokBalanceReport = "App.ReportFeatureStokBalanceReport";
        public const string ReportFeatureAssetBalanceReport = "App.ReportFeatureAssetBalanceReport";
        public const string ReportFeatureTraceabilityReport = "App.ReportFeatureTraceabilityReport";
        public const string ReportFeatureBatchNoBalanceReport = "App.ReportFeatureBatchNoBalanceReport";
        public const string ReportFeatureAccounting = "App.ReportFeatureAccounting";

        public const string ReportFeatureProductionReport = "App.ReportFeatureProductoinReport";
        public const string ReportFeatureProductionPlanReport = "App.ReportFeatureProductoinPlanReport";
        public const string ReportFeatureProductionOrderReport = "App.ReportFeatureProductoinOrderReport";

        public const string ReportFeatureCustomer = "App.ReportFeatureCustomer";
        public const string ReportFeatureSaleInvoice = "App.ReportFeatureSaleInvoice";
        public const string ReportFeatureCustomerAging = "App.ReportFeatureCustomerAging";
        public const string ReportFeatureCustomerByInvoice = "App.ReportFeatureCustomerByInvoice";
        public const string ReportFeatureCustomerByInvoiceWithPayment = "App.ReportFeatureCustomerByInvoiceWithPayment";
        public const string ReportFeatureProfitByInvoice = "App.ReportFeatureProfitByInvoice";

        public const string ReportFeatureVendor = "App.ReportFeatureVendor";
        public const string ReportFeaturePurchasing = "App.ReportFeaturePurchasing";
        public const string ReportFeatureVendorAging = "App.ReportFeatureVendorAging";
        public const string ReportFeatureVendorByBill = "App.ReportFeatureVendorByBill";

        public const string ReportFeatureQCTest = "App.ReportFeatureQCTest";


    }
}