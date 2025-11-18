using Abp.Application.Features;
using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.MultiTenancy;
using CorarlERP.Features;

namespace CorarlERP.Authorization
{
    /// <summary>
    /// Application's authorization provider.
    /// Defines permissions for the application.
    /// See <see cref="AppPermissions"/> for all permission names.
    /// </summary>
    public class AppAuthorizationProvider : AuthorizationProvider
    {
        private readonly bool _isMultiTenancyEnabled;

        public AppAuthorizationProvider(bool isMultiTenancyEnabled)
        {
            _isMultiTenancyEnabled = isMultiTenancyEnabled;
        }

        public AppAuthorizationProvider(IMultiTenancyConfig multiTenancyConfig)
        {
            _isMultiTenancyEnabled = multiTenancyConfig.IsEnabled;
        }

        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            //COMMON PERMISSIONS (FOR BOTH OF TENANTS AND HOST)

            var pages = context.GetPermissionOrNull(AppPermissions.Pages) ?? context.CreatePermission(AppPermissions.Pages, L("Pages"));

            pages.CreateChildPermission(
               AppPermissions.Pages_Authrization,
               L("Authrization"),
               multiTenancySides: MultiTenancySides.Host);

            #region Clean Rounding
            pages.CreateChildPermission(
                AppPermissions.Pages_Tenant_CleanRounding,
                L("CleanRounding"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CleanRoundingFeature));

            pages.CreateChildPermission(
               AppPermissions.Pages_Tenant_ChangeJournalDate,
               L("ChangeJournalDate"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.ChangeJournalDateFeature));

            #endregion

            #region Common

            var commons = pages.CreateChildPermission(
                AppPermissions.Pages_Tenant_Commons,
                L("Commons"));
            commons.CreateChildPermission(
               AppPermissions.Pages_Tenant_Common_Inventory_JournalTransactionTypes_Find,
               L("FindJournalTransactionType"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature, AppFeatures.InventoryFeature));

            commons.CreateChildPermission(
                    AppPermissions.Pages_Tenant_Customer_POSSetting_GetDetail,
                    L("FindPOSSetting"),
                    multiTenancySides: MultiTenancySides.Tenant,
                    featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));
            commons.CreateChildPermission(
            AppPermissions.Pages_Tenant_Customer_DeliverySchedules_Find,
            L("FindDeliverySchedule"),
            multiTenancySides: MultiTenancySides.Tenant,
            featureDependency: new SimpleFeatureDependency(AppFeatures.CustomerFeatureLayaltyAndMembership));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Accounting_Locks_Find,
                L("FindLockTransaction"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature, AppFeatures.AccountingFeatureClosePeriod));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_UserActivities_FindActivity,
                L("FindActivity"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CommonFeature, AppFeatures.UserActivitesFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_UserActivities_FindTrasaction,
                L("FindTransaction"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CommonFeature, AppFeatures.UserActivitesFeature));

            commons.CreateChildPermission(
                AppPermissions.Pages_Host_Client_PackagePromotions_Find,
                L("FindPackagePromotion"),
                multiTenancySides: MultiTenancySides.Host);

            commons.CreateChildPermission(
              AppPermissions.Pages_Host_Client_Packages_Find,
              L("FindPackage"),
              multiTenancySides: MultiTenancySides.Host);

            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_ProductionOrder_Find,
                L("FindProductionOrder"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CommonFeature, AppFeatures.ProductionFeatureProductionOrder));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_Process_Find,
                L("FindProductionProcess"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CommonFeature, AppFeatures.ProductionFeatureProductionProcess));

            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Commons_FindTaxes,
                L("FindTaxes"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature, AppFeatures.SetupFeatureTaxs));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Commons_FindAccountTypes,
                L("FindAccountTypes"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature, AppFeatures.AccountingFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Find,
                L("FindCustomers"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Locations_Find,
                L("FindLocations"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Lots_Find,
                L("FindLots"),
                multiTenancySides: MultiTenancySides.Tenant,
                 featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Classes_Find,
                L("FindClasses"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature, AppFeatures.SetupFeatureClasss));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_PaymentMethods_Find,
                L("FindPaymentMethods"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature, AppFeatures.CustomersFeaturePOS));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_CustomerTypes_Find,
                L("FindCustomerType"),
                multiTenancySides: MultiTenancySides.Tenant, featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));

            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_VendorTypes_Find,
                L("FindVendorType"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));

            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_TransactionTypes_Find,
                L("FindTransactionType"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));

            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Commons_FindAccounts,
                L("FindAccounts"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature, AppFeatures.AccountingFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_ItemType_Find,
                L("FindItemType"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Currencies_Find,
                L("FindCurrency"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));

            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_MultiCurrencies_Find,
                L("FindMultiCurrency"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_MultiCurrencies_GetList,
                L("GetListMultiCurrency"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Formats_FindNumber,
                L("FindFormatNumber"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Formats_FindDate,
                L("FindFormatDate"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Properties_Find,
                L("FindProperties"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));


            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Item_Find,
                L("FindItems"), multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));
            commons.CreateChildPermission(
               AppPermissions.Pages_Tenant_ItemCodeFormulas_Find,
               L("FindItemCodeFormula"), multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));

            commons.CreateChildPermission(AppPermissions.Pages_Host_Client_BatchNoFormula_Find, L("FindBatchNoFormula"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CommonFeature, AppFeatures.SetupFeatureItems));

            commons.CreateChildPermission(AppPermissions.Pages_Tenant_Common_BatchNo_Find, L("FindBatchNo"),
              multiTenancySides: MultiTenancySides.Tenant,
              featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CommonFeature, AppFeatures.SetupFeatureItems));

            commons.CreateChildPermission(
               AppPermissions.Pages_Tenant_Production_Plan_Find,
               L("FindProductionPlan"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CommonFeature, AppFeatures.ProductionFeatureProductionPlan));

            commons.CreateChildPermission(
             AppPermissions.Pages_Tenant_Production_Line_Find,
             L("FindProductionLine"),
             multiTenancySides: MultiTenancySides.Tenant,
             featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CommonFeature, AppFeatures.ProductionFeatureProductionLine));


            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_ItemPrices_Find,
                L("FindItemPrice"), multiTenancySides: MultiTenancySides.Tenant); 
            
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_PurchasePrices_Find,
                L("FindPurchasePrice"), multiTenancySides: MultiTenancySides.Tenant);

            commons.CreateChildPermission(
               AppPermissions.Pages_Tenant_Customer_Cards_Find,
               L("FindCard"), multiTenancySides: MultiTenancySides.Tenant, featureDependency: new SimpleFeatureDependency(AppFeatures.CustomerFeatureLayaltyAndMembership));

            commons.CreateChildPermission(
              AppPermissions.Pages_Tenant_Customer_Cards_GetCustomerIdByCardId,
              L("GetCustomerIdByCardId"), multiTenancySides: MultiTenancySides.Tenant, featureDependency: new SimpleFeatureDependency(AppFeatures.CustomerFeatureLayaltyAndMembership));

            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Exchanges_Find,
                L("FindExchange"), multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureExchangeRate));


            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_PropertyValue_FindValue,
                L("FindPropertyValue"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendor_Find,
                L("FindVendors"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_SaleOrder_Find,
                L("FindSaleOrders"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ReceivePayments_Find,
                L("FindReceivePayments"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CommonFeature, AppFeatures.CustomersFeatureReceivePayments));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_Credit_Find,
                L("FindCustomerCredits"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));

            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_PurchaseOrder_Find,
                L("FindPurchaseOrders"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Invoice_Find,
                L("FindInvoices"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Common_Inventory,
                L("GetInventoryStock"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));

            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Common_DeleteVoid,
                L("DeleteVoid"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));

            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Common_Stock_Balance,
                L("GetStockBalance"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_Find,
                L("FindItemIssues"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CommonFeature, AppFeatures.CustomersFeatureItemIssues));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_Bills_Find,
                L("FindBills"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_Credit_Find,
                L("FindVendorCredits"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));

            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_Find,
                L("FindItemReceipts"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Accounting_Journals_Find,
                L("FindJournals"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_PayBills_Find,
                L("FindPayBills"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks_Transfers_Find,
                L("FindBankTransfers"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_PhysicalCount_Find,
                L("FindInventoryPhysicalCount"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_TransferOrder_Find,
                L("FindTransferOrders"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_ProductionOrder_GetListProductionOrderForItemIssue,
                L("GetListProductionOrderForItemIssue"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_ProductionOrder_GetListProductionOrderForItemReceipt,
                L("GetListProductionOrderForItemReceipt"),
                multiTenancySides: MultiTenancySides.Tenant);
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_Bills_GetBills,
                L("GetBillsSummaryForItemReceipt"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_Bills_GetBillItems,
                L("GetBillItemsForItemReceipt"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));

            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Exchanges_GetExChangeCurrency,
                L("GetExchangeCurrency"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureExchangeRate, AppFeatures.SetupFeatureExchangeRate));

            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_PurchaseOrder_GetTotalPurchaseOrderForItemReceipts,
                L("GetTotalPurchaseOrderForItemReceipts"),
                multiTenancySides: MultiTenancySides.Tenant);
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_PurchaseOrder_GetlistPuchaseOrderForItemReceipts,
                L("GetlistPuchaseOrderForItemReceipts"),
                multiTenancySides: MultiTenancySides.Tenant);

            //bill
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_PurchaseOrder_GetTotalPurchaseOrderForBills,
                L("GetTotalPurchaseOrderForBill"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CommonFeature, AppFeatures.VendorsFeatureBills));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_PurchaseOrder_GetlistPuchaseOrderForBills,
                L("GetlistPuchaseOrderForBills"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CommonFeature, AppFeatures.VendorsFeatureBills));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts,
                L("GetItemReceipts"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CommonFeature, AppFeatures.VendorsFeatureItemReceipts));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceiptItems,
                L("GetItemReceiptItems"),
                multiTenancySides: MultiTenancySides.Tenant);

            //invoice 
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_SaleOrder_GetItemSaleOrderForInvoices,
                L("GetItemSaleOrderForInvoice"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CommonFeature, AppFeatures.CustomersFeatureItemIssues));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_SaleOrder_GetTotalSaleOrderForInvoice,
                L("GetTotalSaleOrderForInvoice"),
                multiTenancySides: MultiTenancySides.Tenant);

            // paybill
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_Bills_GetListForPayBill,
                L("GetListBillForPayBillList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CommonFeature, AppFeatures.CustomersFeatureInvoices));

            //receive payment
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Invoice_GetListInvoiceForReceivePayment,
                L("GetListInvoiceForReceivePayment"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CommonFeature, AppFeatures.CustomersFeatureInvoices));

            // transfe order
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_TransferOrder_ForItemReceipt,
                L("GetItemReceiptItemDetail"),
                multiTenancySides: MultiTenancySides.Tenant);
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_TransferOrder_ForItemIssue,
                L("GetItemIssueItemDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CommonFeature, AppFeatures.CustomersFeatureItemIssues));

            //item issue
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssues,
                L("GetItemIssueSummaryForInvoice"),
                multiTenancySides: MultiTenancySides.Tenant);
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssueItems,
                L("GetItemIssueItemsForInvoice"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CommonFeature, AppFeatures.CustomersFeatureItemIssues));

            //ggg
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_SaleOrder_GetItemSaleOrderForItemIssues,
                L("GetItemSaleOrderForItemIssue"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CommonFeature, AppFeatures.CustomersFeatureItemIssues));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_SaleOrder_GetTotalSaleOrder,
                L("GetTotalSaleOrder"),
                multiTenancySides: MultiTenancySides.Tenant);
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Invoice_GetInvoiceItems,
                L("GetInvoiceItems"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CommonFeature, AppFeatures.CustomersFeatureInvoices));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Invoice_GetInvoiceSummaryForItemIssue,
                L("GetInvoiceSummaryForItemIssue"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CommonFeature, AppFeatures.CustomersFeatureItemIssues));
            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Common_Tenant_Find,
                L("FindTenant"),
                multiTenancySides: MultiTenancySides.Host);

            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Find,
                L("FindCashFlowTemplate"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));

            commons.CreateChildPermission(
               AppPermissions.Pages_Tenant_Setup_TestParameters_Find,
               L("FindTestParameter"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));

            commons.CreateChildPermission(
               AppPermissions.Pages_Tenant_Setup_QCTestTemplates_Find,
               L("FindQCTestTemplate"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));

            commons.CreateChildPermission(
             AppPermissions.Pages_Tenant_QCSamples_Find,
             L("FindQCSample"),
             multiTenancySides: MultiTenancySides.Tenant,
             featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));

            commons.CreateChildPermission(
                AppPermissions.Pages_Tenant_LabTestRequests_Find,
                L("FindLabTestRequests"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature));

            #endregion

            #region Dashboard
            pages.CreateChildPermission(AppPermissions.Pages_Tenant_Dashboard, L("Dashboard"), multiTenancySides: MultiTenancySides.Tenant);
            pages.CreateChildPermission(AppPermissions.Pages_Tenant_App, L("App"), multiTenancySides: MultiTenancySides.Tenant);

            #endregion

            #region POS

            //POS
            var pos = pages.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_POS,
                L("POS"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));

            pos.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_POS_CanSeePrice,
                L("CanSeePrice"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));

            pos.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_POS_GetList,
                L("POSList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));

            pos.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_POS_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));

            pos.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_POS_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));

            pos.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_POS_ViewAll,
                L("ViewAll"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));

            pos.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_POS_Print,
                L("Print"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));

            pos.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_POS_UpdateToVoid,
                L("Void"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));



            //POS - Setting
            var posSetting = pos.CreateChildPermission(AppPermissions.Pages_Tenant_Customer_POSSetting,
                L("Setting"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));

            posSetting.CreateChildPermission(
               AppPermissions.Pages_Tenant_Customer_POSSetting_CreateOrUpdate,
               L("CreateOrUpdate"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));
            posSetting.CreateChildPermission(
             AppPermissions.Pages_Tenant_Customer_POSSetting_Delete,
             L("Delete"),
             multiTenancySides: MultiTenancySides.Tenant,
             featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));

            //POS - Sale Return
            var saleReturn = pos.CreateChildPermission(AppPermissions.Pages_Tenant_Customers_Invoice_SaleReturn,
                L("SaleReturn"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));

            saleReturn.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_Invoice_ListSaleReturn,
                L("SaleReturnList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));

            saleReturn.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_Invoice_CreateSaleReturn,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));

            saleReturn.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_Invoice_ViewSaleReturn,
                L("GetDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));

            saleReturn.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssues_SaleReturn,
                L("GetListInvoiceForPOS"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CommonFeature, AppFeatures.CustomersFeaturePOS));

            saleReturn.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_Invoice_UpdateSaleReturn,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));

            saleReturn.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_Invoice_VoidSaleReturn,
                L("Void"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));

            saleReturn.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_Invoice_DeleteVoideSaleReturn,
                L("DeleteVoid"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));

            saleReturn.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Credit_POS_Find,
                L("Find"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));

            saleReturn.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_Invoice_ListSaleReturn_Unpaid,
                L("UnpaidList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));


            //POS - Customer
            var customerPOS = pos.CreateChildPermission(
                AppPermissions.Pages_Tenant_Set_Customer_POS,
                L("Customer"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));

            customerPOS.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_GetListPOS,
                L("CustomerList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));

            customerPOS.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_CreatePOS,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));

            customerPOS.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_GetDetailPOS,
                L("GetDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));

            customerPOS.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_UpdatePOS,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));

            customerPOS.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_DeletePOS,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));

            customerPOS.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_DisablePOS,
                L("Disable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));

            customerPOS.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_EnablePOS,
                L("Enable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS));

            #endregion

            #region Vendors
            var vendors = pages.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors,
                L("Vendors"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeature));


            // vendor  list
            var vendorList = vendors.CreateChildPermission(
               AppPermissions.Pages_Tenant_Vendor_GetList,
               L("VendorList"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeature));

            vendorList.CreateChildPermission(
                 AppPermissions.Pages_Tenant_Vendor_Create,
                 L("Create"),
                 multiTenancySides: MultiTenancySides.Tenant,
                 featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeature));

            vendorList.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendor_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeature));

            vendorList.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendor_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeature));

            vendorList.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendor_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeature));

            vendorList.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendor_Disable,
                L("Disable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeature));

            vendorList.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendor_Enable,
                L("Enable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeature));

            vendorList.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendor_ExportExcel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeature));

            vendorList.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendor_ImportExcel,
                L("ImportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeature));

            //Vendors - Purchase Order
            var purchaseOrders = vendors.CreateChildPermission(
                AppPermissions.Pages_Tenant_PurchaseOrders,
                L("PruchaseOrders"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePurchases));

            var commonPurhaserOrder = purchaseOrders.CreateChildPermission(
                AppPermissions.Pages_Tenant_PurchaseOrder_Common,
                L("Commons"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePurchases));

            commonPurhaserOrder.CreateChildPermission(
                AppPermissions.Pages_Tenant_PurchaseOrder_CanSeePrice,
                L("CanSeePrice"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePurchases));
            
            commonPurhaserOrder.CreateChildPermission(
                AppPermissions.Pages_Tenant_PurchaseOrder_EditExchangeRate,
                L("EditExchangeRate"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.VendorsFeaturePurchases, AppFeatures.SetupFeatureExchangeRate));

            purchaseOrders.CreateChildPermission(
                AppPermissions.Pages_Tenant_PurchaseOrder_GetList,
                L("PurchaseOrderList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePurchases));

            purchaseOrders.CreateChildPermission(
                 AppPermissions.Pages_Tenant_PurchaseOrder_Create,
                 L("Create"),
                 multiTenancySides: MultiTenancySides.Tenant,
                 featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePurchases));

            purchaseOrders.CreateChildPermission(
                AppPermissions.Pages_Tenant_PurchaseOrder_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePurchases));

            purchaseOrders.CreateChildPermission(
                AppPermissions.Pages_Tenant_PurchaseOrder_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePurchases));

            purchaseOrders.CreateChildPermission(
                AppPermissions.Pages_Tenant_PurchaseOrder_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePurchases));

            purchaseOrders.CreateChildPermission(
                AppPermissions.Pages_Tenant_PurchaseOrder_UpdateStatusToPublish,
                L("Publish"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePurchases));

            purchaseOrders.CreateChildPermission(
                AppPermissions.Pages_Tenant_PurchaseOrder_UpdateStatusToClose,
                L("Close"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePurchases));


            //Vendors - Item Receipts
            var ItemReceipts = vendors.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts,
                L("ItemReceipts"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureItemReceipts));

            var commonItemReceipt = ItemReceipts.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_common,
                L("Commons"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureItemReceipts));

            commonItemReceipt.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CanSeePrice,
                L("CanSeePrice"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureItemReceipts));
            commonItemReceipt.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CanEditAccount,
                L("CanEditAccount"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.VendorsFeatureItemReceipts, AppFeatures.AccountingFeature));

            commonItemReceipt.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CanViewAccount,
                L("CanViewAccount"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.VendorsFeatureItemReceipts, AppFeatures.AccountingFeature));

            ItemReceipts.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetList,
                L("ItemReceiptList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureItemReceipts));

            var createItemReceipt = ItemReceipts.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureItemReceipts));

            var createItemReceiptPurchase = createItemReceipt.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CreatePurchases,
                L("Purchase"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureItemReceipts));

            createItemReceiptPurchase.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CanPO,
                L("FromPurchaseOrder"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.VendorsFeaturePurchases, AppFeatures.VendorsFeatureItemReceipts));

            createItemReceiptPurchase.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CanBill,
                L("FromBill"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.VendorsFeatureBills, AppFeatures.VendorsFeatureItemReceipts));

            var createItemReceiptTransfer = createItemReceipt.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CreateTransfers,
                L("Transfer"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.InventoryFeatureTransferOrders, AppFeatures.VendorsFeatureItemReceipts));
            createItemReceiptTransfer.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_TransferOrder_FindItemReceiptTrasfer,
                L("FromTransferOrder"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.InventoryFeatureTransferOrders, AppFeatures.VendorsFeatureItemReceipts));

            var createitemReceiptSaleReturn = createItemReceipt.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CreateCustomerCredits,
                L("SaleReturn"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeature, AppFeatures.VendorsFeatureItemReceipts));

            createitemReceiptSaleReturn.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_Credit_GetCustomerCreditHeader,
                L("FromCustomerCredit"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.VendorsFeatureItemReceipts, AppFeatures.CustomersFeatureInvoices));

            createitemReceiptSaleReturn.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssues_ForItemReceiptCustomerCrdit,
                L("FromSale"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureItemIssues, AppFeatures.VendorsFeatureItemReceipts));

            var createItemreceiptProduction = createItemReceipt.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CreateProductionOrder,
                L("Production"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.VendorsFeatureItemReceipts, AppFeatures.ProductionFeature));

            createItemreceiptProduction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_ProductionOrder_GetProductionOrderForItemReceipt,
                L("FromProductionOrder"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.VendorsFeatureItemReceipts, AppFeatures.ProductionFeatureProductionOrder));


            createItemReceipt.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CreateAdjustments,
                L("Adjustment"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureItemReceipts));

            createItemReceipt.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CreateOthers,
                L("Other"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureItemReceipts));

            ItemReceipts.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureItemReceipts));

            ItemReceipts.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureItemReceipts));

            ItemReceipts.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureItemReceipts));

            var importExcelItemReceipt = ItemReceipts.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_importExcel,
                L("ImportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureItemReceipts));

            importExcelItemReceipt.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_importExcelItemReceiptOther,
                L("Other"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureItemReceipts));

            importExcelItemReceipt.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_importExcelItemReceiptAdjustment,
                L("Adjustment"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureItemReceipts));

            var exportExcelItemReceipt = ItemReceipts.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_exportExcel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureItemReceipts));

            exportExcelItemReceipt.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_exportExcelItemReceiptOther,
                L("Other"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureItemReceipts));

            exportExcelItemReceipt.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_exportExcelItemReceiptAdjustment,
                L("Adjustment"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureItemReceipts));


            //Vendors - Bills
            var Bills = vendors.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_Bills,
                L("Bill"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureBills));

            var billCommon = Bills.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_Bills_Common,
                L("Commons"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureBills));
            billCommon.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_Bills_EditQty,
                L("EditQty"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureBills));
            billCommon.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_Bills_CanSeePrice,
                L("CanSeePrice"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureBills));
            billCommon.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_Bills_CanEditAccount,
                L("CanEditAccount"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.VendorsFeatureBills, AppFeatures.AccountingFeature));
            billCommon.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_Bills_CanViewAccount,
                L("CanViewAccount"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.VendorsFeatureBills, AppFeatures.AccountingFeature));
            billCommon.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_Bills_EditExchangeRate,
                L("EditExchangeRate"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.VendorsFeatureBills, AppFeatures.SetupFeatureExchangeRate));

            billCommon.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_Bills_Setting,
                L("Setting"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureBills));

            Bills.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_Bills_GetList,
                L("BillList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureBills));

            var createBill = Bills.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_Bills_Create,
                L("CreateBill"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureBills));

            createBill.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_Bills_CanPO,
                L("FromPurchaseOrder"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.VendorsFeatureBills, AppFeatures.VendorsFeaturePurchases));

            createBill.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_Bills_CanItemReceipt,
                L("FromItemReceiptPurchase"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.VendorsFeatureBills, AppFeatures.VendorsFeatureItemReceipts));

            createBill.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_Bills_ChooseAccounts,
                L("ChooseAccounts"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.VendorsFeatureBills, AppFeatures.AccountingFeature));

            var viewBill = Bills.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_Bills_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureBills));

            viewBill.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_PayBills_ViewBillHistory,
                L("ViewPayBillHistory"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureBills));
            viewBill.CreateChildPermission(
               AppPermissions.Pages_Tenant_Vendors_PayBills_ViewVendorCreditHistory,
               L("ViewVendorCreditHistory"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureBills));

            Bills.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_Bills_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureBills));

            Bills.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_Bills_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureBills));

            Bills.CreateChildPermission(
               AppPermissions.Pages_Tenant_Vendors_Bills_MultiDelete,
               L("MultiDelete"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureBills));


            var importExcelBill = Bills.CreateChildPermission(AppPermissions.Pages_Tenant_Vendors_Bills_ImportExcel,
                L("ImportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureBills));

            importExcelBill.CreateChildPermission(AppPermissions.Pages_Tenant_Vendors_Bills_ImportExcelBill,
                L("Bill"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureBills));

            importExcelBill.CreateChildPermission(AppPermissions.Pages_Tenant_Vendors_Credit_Import,
                L("VendorCredit"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureBills));


            var exportExcelBill = Bills.CreateChildPermission(AppPermissions.Pages_Tenant_Vendors_Bills_ExportExcel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureBills));

            exportExcelBill.CreateChildPermission(AppPermissions.Pages_Tenant_Vendors_Bills_ExportExcelBillTamplate,
                L("Bill"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureBills, AppFeatures.VendorsFeatureBills));

            exportExcelBill.CreateChildPermission(AppPermissions.Pages_Tenant_Vendors_Credit_ExportVendorCreditTamplate,
                L("VendorCredit"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureBills, AppFeatures.VendorsFeatureBills));


            //Vendors - Vendor Credits
            var vendorcredit = Bills.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_Bills_CanCreateVendorCredit,
                L("CreateVendorCredit"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeatureBills));

            vendorcredit.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssues_ForVendorCredit,
                L("FromItemIssuePurchaseReturn"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureItemIssues, AppFeatures.VendorsFeatureBills));

            vendorcredit.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts_ItemIssueVendorCredit,
                L("FromItemReceiptPurchase"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.VendorsFeatureItemReceipts, AppFeatures.VendorsFeatureBills));

            vendorcredit.CreateChildPermission(
               AppPermissions.Pages_Tenant_Vendors_Bills_VendorCreditChooseAccounts,
               L("ChooseAccounts"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.VendorsFeatureBills, AppFeatures.AccountingFeature));

            //Vendors - Pay Bills
            var PayBills = vendors.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_PayBills,
                L("PayBill"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePayBills));

            var paybillCommon = PayBills.CreateChildPermission(

                AppPermissions.Pages_Tenant_Vendors_PayBills_Common,
                L("Commons"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePayBills));

            paybillCommon.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_PayBills_CanEditAccount,
                L("CanEditAccount"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.VendorsFeaturePayBills, AppFeatures.AccountingFeature));

            paybillCommon.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_PayBills_CanViewAccount,
                L("CanViewAccount"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.VendorsFeaturePayBills, AppFeatures.AccountingFeature));
            
            paybillCommon.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_PayBills_EditExchangeRate,
                L("EditExchangeRate"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.VendorsFeaturePayBills, AppFeatures.SetupFeatureExchangeRate));

            paybillCommon.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_PayBills_CanPayByCredit,
                L("CanPayByCredit"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePayBills));

            PayBills.CreateChildPermission(
               AppPermissions.Pages_Tenant_Vendors_PayBills_GetList,
               L("PayBillList"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePayBills));

            PayBills.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_PayBills_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePayBills));

            PayBills.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_PayBills_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePayBills));

            PayBills.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_PayBills_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePayBills));

            PayBills.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_PayBills_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePayBills));
            PayBills.CreateChildPermission(
              AppPermissions.Pages_Tenant_Vendors_PayBills_MultiDelete,
              L("MultiDelete"),
              multiTenancySides: MultiTenancySides.Tenant,
              featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePayBills));

            //Purchase Prices
            var purchasePrices = vendors.CreateChildPermission(
                AppPermissions.Pages_Tenant_PurchasePrices,
                L("PurchasePrices"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePurchasePrices));

            purchasePrices.CreateChildPermission(
                AppPermissions.Pages_Tenant_PurchasePrices_GetList,
                L("PurchasePriceList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePurchasePrices));

            purchasePrices.CreateChildPermission(
                AppPermissions.Pages_Tenant_PurchasePrices_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePurchasePrices));

            purchasePrices.CreateChildPermission(
               AppPermissions.Pages_Tenant_PurchasePrices_Export,
               L("Export"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePurchasePrices));

            purchasePrices.CreateChildPermission(
                AppPermissions.Pages_Tenant_PurchasePrices_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePurchasePrices));

            purchasePrices.CreateChildPermission(
                AppPermissions.Pages_Tenant_PurchasePrices_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePurchasePrices));

            purchasePrices.CreateChildPermission(
                AppPermissions.Pages_Tenant_PurchasePrices_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.VendorsFeaturePurchasePrices));


            #endregion

            #region Customers
            var customers = pages.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers,
                L("Customers"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeature));
            // Customer List
            var customerList = customers.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_GetList,
                L("CustomerList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeature));

            customerList.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeature));

            customerList.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeature));

            customerList.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeature));

            customerList.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeature));

            customerList.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Disable,
                L("Disable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeature));

            customerList.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Enable,
                L("Enable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeature));

            customerList.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_ExportExcel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeature));

            customerList.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_ImportExcel,
                L("ImportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeature));

            //Customers - Sale Orders
            var saleOrders = customers.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_SaleOrder,
                L("SaleOrders"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureSaleOrders));

            var deliverySchedules = customers.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_DeliverySchedules,
                L("DeliverySchedule"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureDeliverySchedule));
            deliverySchedules.CreateChildPermission(
               AppPermissions.Pages_Tenant_Customer_DeliverySchedules_Create,
               L("Create"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.CustomerFeatureLayaltyAndMembership));

            deliverySchedules.CreateChildPermission(
              AppPermissions.Pages_Tenant_Customer_DeliverySchedules_GetList,
              L("GetList"),
              multiTenancySides: MultiTenancySides.Tenant,
              featureDependency: new SimpleFeatureDependency(AppFeatures.CustomerFeatureLayaltyAndMembership));
          
            deliverySchedules.CreateChildPermission(
             AppPermissions.Pages_Tenant_Customer_DeliverySchedules_GetDetail,
             L("View"),
             multiTenancySides: MultiTenancySides.Tenant,
             featureDependency: new SimpleFeatureDependency(AppFeatures.CustomerFeatureLayaltyAndMembership));
           
            deliverySchedules.CreateChildPermission(
             AppPermissions.Pages_Tenant_Customer_DeliverySchedules_Update,
             L("Update"),
             multiTenancySides: MultiTenancySides.Tenant,
             featureDependency: new SimpleFeatureDependency(AppFeatures.CustomerFeatureLayaltyAndMembership));
           
            deliverySchedules.CreateChildPermission(
             AppPermissions.Pages_Tenant_Customer_DeliverySchedules_UpdateToPublish,
              L("Publish"),
             multiTenancySides: MultiTenancySides.Tenant,
             featureDependency: new SimpleFeatureDependency(AppFeatures.CustomerFeatureLayaltyAndMembership));
            
            deliverySchedules.CreateChildPermission(
             AppPermissions.Pages_Tenant_Customer_DeliverySchedules_UpdateToClose,
             L("Close"),
             multiTenancySides: MultiTenancySides.Tenant,
             featureDependency: new SimpleFeatureDependency(AppFeatures.CustomerFeatureLayaltyAndMembership));
           
            deliverySchedules.CreateChildPermission(
             AppPermissions.Pages_Tenant_Customer_DeliverySchedules_UpdateToVoid,
             L("Void"),
             multiTenancySides: MultiTenancySides.Tenant,
             featureDependency: new SimpleFeatureDependency(AppFeatures.CustomerFeatureLayaltyAndMembership));
           
            deliverySchedules.CreateChildPermission(
            AppPermissions.Pages_Tenant_Customer_DeliverySchedules_UpdateToDraft,
            L("Draft"),
            multiTenancySides: MultiTenancySides.Tenant,
            featureDependency: new SimpleFeatureDependency(AppFeatures.CustomerFeatureLayaltyAndMembership));
           
            deliverySchedules.CreateChildPermission(
           AppPermissions.Pages_Tenant_Customer_DeliverySchedules_Delete,
           L("Delete"),
           multiTenancySides: MultiTenancySides.Tenant,
           featureDependency: new SimpleFeatureDependency(AppFeatures.CustomerFeatureLayaltyAndMembership));
            //Layalty  & Membership
            var cards = customers.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Cards,
                L("Card"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomerFeatureLayaltyAndMembership));
            cards.CreateChildPermission(
              AppPermissions.Pages_Tenant_Customer_Cards_GetList,
              L("GetList"),
              multiTenancySides: MultiTenancySides.Tenant,
              featureDependency: new SimpleFeatureDependency(AppFeatures.CustomerFeatureLayaltyAndMembership));
            cards.CreateChildPermission(
               AppPermissions.Pages_Tenant_Customer_Cards_Update,
               L("Update"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.CustomerFeatureLayaltyAndMembership));

            cards.CreateChildPermission(
              AppPermissions.Pages_Tenant_Customer_Cards_Create,
              L("Create"),
              multiTenancySides: MultiTenancySides.Tenant,
              featureDependency: new SimpleFeatureDependency(AppFeatures.CustomerFeatureLayaltyAndMembership));
            cards.CreateChildPermission(
              AppPermissions.Pages_Tenant_Customer_Cards_Delete,
              L("Delete"),
              multiTenancySides: MultiTenancySides.Tenant,
              featureDependency: new SimpleFeatureDependency(AppFeatures.CustomerFeatureLayaltyAndMembership));
            cards.CreateChildPermission(
              AppPermissions.Pages_Tenant_Customer_Cards_View,
              L("View"),
              multiTenancySides: MultiTenancySides.Tenant,
              featureDependency: new SimpleFeatureDependency(AppFeatures.CustomerFeatureLayaltyAndMembership));
            cards.CreateChildPermission(
              AppPermissions.Pages_Tenant_Customer_Cards_Enable,
              L("Enable"),
              multiTenancySides: MultiTenancySides.Tenant,
              featureDependency: new SimpleFeatureDependency(AppFeatures.CustomerFeatureLayaltyAndMembership));
            cards.CreateChildPermission(
              AppPermissions.Pages_Tenant_Customer_Cards_Disable,
              L("Disable"),
              multiTenancySides: MultiTenancySides.Tenant,
              featureDependency: new SimpleFeatureDependency(AppFeatures.CustomerFeatureLayaltyAndMembership));
            cards.CreateChildPermission(
             AppPermissions.Pages_Tenant_Customer_Cards_Deactivate,
             L("Deactivate"),
             multiTenancySides: MultiTenancySides.Tenant,
             featureDependency: new SimpleFeatureDependency(AppFeatures.CustomerFeatureLayaltyAndMembership));

            cards.CreateChildPermission(
              AppPermissions.Pages_Tenant_Customer_Cards_Import,
              L("ImportExcel"),
              multiTenancySides: MultiTenancySides.Tenant,
              featureDependency: new SimpleFeatureDependency(AppFeatures.CustomerFeatureLayaltyAndMembership));
            cards.CreateChildPermission(
            AppPermissions.Pages_Tenant_Customer_Cards_Export,
            L("ExportExcel"),
            multiTenancySides: MultiTenancySides.Tenant,
            featureDependency: new SimpleFeatureDependency(AppFeatures.CustomerFeatureLayaltyAndMembership));


            var commonSaleOrder = saleOrders.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_SaleOrder_Common,
                L("Commons"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureSaleOrders));

            commonSaleOrder.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_SaleOrder_CanSeePrice,
                L("CanSeePrice"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureSaleOrders));

            saleOrders.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_SaleOrder_GetList,
                L("SaleOrderList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureSaleOrders));

            saleOrders.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_SaleOrder_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureSaleOrders));

            saleOrders.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_SaleOrder_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureSaleOrders));

            saleOrders.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_SaleOrder_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureSaleOrders));

            saleOrders.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_SaleOrder_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureSaleOrders));

            saleOrders.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_SaleOrder_UpdateToClose,
                L("Close"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureSaleOrders));

            saleOrders.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_SaleOrder_UpdateInventoryStatus,
                L("UpdateDeliveryStatus"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureSaleOrders));

            saleOrders.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_SaleOrder_UpdateToPublish,
                L("Publish"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureSaleOrders));

            saleOrders.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_SaleOrder_UpdateToVoid,
                L("Void"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureSaleOrders));
            
            saleOrders.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_SaleOrder_EditExchangeRate,
                L("EditExchangeRate"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureSaleOrders, AppFeatures.SetupFeatureExchangeRate));


            //Customers - Invoices
            var invoice = customers.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Invoices,
                L("Invoices"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices));

            var commonInvoice = invoice.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Invoices_Common,
                L("Commons"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices));

            commonInvoice.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Invoice_EditQty,
                L("EditQty"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices));
            commonInvoice.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Invoice_CanEditAccount,
                L("CanEditAccount"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureInvoices, AppFeatures.AccountingFeature));

            commonInvoice.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Invoice_CanViewAccount,
                L("CanViewAccount"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureInvoices, AppFeatures.AccountingFeature));
            
            commonInvoice.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Invoice_EditExchangeRate,
                L("EditExchangeRate"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureInvoices, AppFeatures.SetupFeatureExchangeRate));

            commonInvoice.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Invoice_CanSeePrice,
                L("CanSeePrice"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices));

            commonInvoice.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Invoice_Setting,
                L("Setting"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices));

            invoice.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Invoice_GetList,
                L("InvoiceList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices));


            invoice.CreateChildPermission(
               AppPermissions.Pages_Tenant_Customer_Invoice_MultiDelete,
               L("MultiDelete"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices));

            var createInvoice = invoice.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Invoice_Create,
                L("CreateInvoice"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices));



            createInvoice.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Invoice_CanItemIssue,
                L("FromItemIssue"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureInvoices, AppFeatures.CustomersFeatureItemIssues));

            createInvoice.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Invoice_CanSaleOrder,
                L("FromSaleOrder"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureInvoices, AppFeatures.CustomersFeatureSaleOrders));

            createInvoice.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Invoice_ChooseAccounts,
                L("ChooseAccounts"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureInvoices, AppFeatures.AccountingFeature));

            invoice.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Invoice_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices));

            invoice.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Invoice_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices));

            invoice.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Invoice_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices));


            var invoiceImportExcel = invoice.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Invoice_ImportExcel,
                L("ImportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices));

            invoiceImportExcel.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Invoice_ImportExcelInvoice,
                L("Invoice"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices));

            invoiceImportExcel.CreateChildPermission(AppPermissions.Pages_Tenant_Customers_Credit_Import,
                L("CustomerCredit"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices));

            var invoiceExportExcel = invoice.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Invoice_ExportExcel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices));

            invoiceExportExcel.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_Invoice_ExportExcelTamplate,
                L("Invoice"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices));

            invoiceExportExcel.CreateChildPermission(AppPermissions.Pages_Tenant_Customers_Credit_ExportCustomerCreditTamplate,
                L("CustomerCredit"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices));

            // customer credit
            var createCustomerCredit = invoice.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_Invoice_CreateCustomerCredit,
                L("CreateCustomerCredit"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices));

            createCustomerCredit.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssues_ForCustomerCredit,
                L("FromItemIssueSale"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureInvoices, AppFeatures.CustomersFeatureItemIssues));

            createCustomerCredit.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts_CustomerCredit,
                L("FromItemReceiptSaleReturn"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.VendorsFeatureItemReceipts, AppFeatures.CustomersFeatureInvoices));

            createCustomerCredit.CreateChildPermission(
               AppPermissions.Pages_Tenant_Customer_Invoice_CustomerCreditChooseAccounts,
               L("ChooseAccounts"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureInvoices, AppFeatures.AccountingFeature));

            //Customers - Item Issues
            var ItemIssues = customers.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues,
                L("ItemIssues"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureItemIssues));

            var commonItemIssue = ItemIssues.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_Common,
                L("Commons"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureItemIssues));

            commonItemIssue.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_CanEditAccount,
                L("CanEditAccount"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureItemIssues, AppFeatures.AccountingFeature));

            commonItemIssue.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_CanViewAccount,
                L("CanViewAccount"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureItemIssues, AppFeatures.AccountingFeature));

            commonItemIssue.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_CanSeePrice,
                L("CanSeePrice"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureItemIssues));

            ItemIssues.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_GetList,
                L("ItemIssueList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureItemIssues));

            var createItemIssue = ItemIssues.CreateChildPermission(AppPermissions.Pages_Tenant_Customers_ItemIssues_Create,
                L("CreateItemIssueSale"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureItemIssues));

            createItemIssue.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_CanSaleOrder,
                L("FromSaleOrder"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureItemIssues, AppFeatures.CustomersFeatureSaleOrders));

            createItemIssue.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_CanInvoice,
                L("FromInvoice"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureInvoices, AppFeatures.CustomersFeatureItemIssues));

            createItemIssue.CreateChildPermission(
               AppPermissions.Pages_Tenant_Customers_ItemIssues_CanDelivery,
               L("FromDelivery"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureDeliverySchedule, AppFeatures.CustomersFeatureItemIssues));


            createItemIssue.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_ShowConvertInvoice,
                L("ShowConvertToInvoice"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureItemIssues, AppFeatures.CustomersFeatureInvoices));

            createItemIssue.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_EditConvertInvoice,
                L("CanEditConvertToInvoice"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureItemIssues, AppFeatures.CustomersFeatureInvoices));

            var itemIssueVendorCredit = ItemIssues.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_CanCreateVendorCredit,
                L("CreateItemIssueVendorCredit"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureItemIssues));

            itemIssueVendorCredit.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_Bills_GetListForItemReceipt,
                L("FromPurchaseReturn"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.VendorsFeature, AppFeatures.CustomersFeatureItemIssues));

            itemIssueVendorCredit.CreateChildPermission(
                AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts_VendorCredit,
                L("FromItemReceiptPurchase"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureItemIssues, AppFeatures.VendorsFeatureItemReceipts));

            var transferOrder = ItemIssues.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_CanCreateTransfer,
                L("CreateItemIssueTransfer"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.InventoryFeatureTransferOrders, AppFeatures.CustomersFeatureItemIssues));

            transferOrder.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_TransferOrder_ForItemIssueTransfer,
                L("FromTransferOrder"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.InventoryFeatureTransferOrders, AppFeatures.CustomersFeatureItemIssues));


            var productionOrder = ItemIssues.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_CanCreateProductionOrder,
                L("CreateItemIssueProductionOrder"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureItemIssues, AppFeatures.ProductionFeature));

            productionOrder.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_ProductionOrder_GetProductionOrderForItemIssue,
                L("FromProductionOrder"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureItemIssues, AppFeatures.ProductionFeatureProductionOrder));

            ItemIssues.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_CanCreateAdjustment,
                L("CreateItemIssueAdjustment"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureItemIssues));

            ItemIssues.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_CanCreateOther,
                L("CreateItemIssueOther"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureItemIssues));


            ItemIssues.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureItemIssues));

            ItemIssues.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureItemIssues));

            ItemIssues.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureItemIssues));


            ItemIssues.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_ImportExcelItemIssueAdjustment,
                L("ImportExcelItemIssueAdjustment"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureItemIssues));


            ItemIssues.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ItemIssues_ExportExcelTemplateItemIssueAdjustment,
                L("ExportExcelTemplateItemIssueAdjustment"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureItemIssues));


            //Customers - Receive Payment
            var ReceivePayment = customers.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ReceivePayments,
                L("ReceivePayments"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureReceivePayments));

            var commonReceivePayment = ReceivePayment.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ReceivePayments_Common,
                L("Commons"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureReceivePayments));
            commonReceivePayment.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ReceivePayments_CanEditAccount,
                L("CanEditAccount"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureReceivePayments, AppFeatures.AccountingFeature));

            commonReceivePayment.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ReceivePayments_CanViewAccount,
                L("CanViewAccount"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureReceivePayments, AppFeatures.AccountingFeature));
            
            commonReceivePayment.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ReceivePayments_EditExchangeRate,
                L("EditExchangeRate"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureReceivePayments, AppFeatures.SetupFeatureExchangeRate));

            ReceivePayment.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ReceivePayments_GetList,
                L("ReceivePaymentList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureReceivePayments));

            ReceivePayment.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ReceivePayments_Create,
                L("CreateReceivePayment"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureReceivePayments));

            var viewReceivePayment = ReceivePayment.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ReceivePayments_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureReceivePayments));

            viewReceivePayment.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ReceivePayments_ViewInvoiceHistory,
                L("ViewPayInvocieHistory"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureReceivePayments));

            viewReceivePayment.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ReceivePayments_ViewCustomerCreditHistory,
                L("ViewCustomerCreditHistory"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureReceivePayments));

            ReceivePayment.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ReceivePayments_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureReceivePayments));

            ReceivePayment.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customers_ReceivePayments_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureReceivePayments));
            ReceivePayment.CreateChildPermission(
               AppPermissions.Pages_Tenant_Customers_ReceivePayments_MultiDelete,
               L("MultiDelete"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureReceivePayments));
            ReceivePayment.CreateChildPermission(
             AppPermissions.Pages_Tenant_Customers_ReceivePayments_Export_Excel,
             L("ExportExcel"),
             multiTenancySides: MultiTenancySides.Tenant,
             featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureReceivePayments));

            #endregion

            #region Productions
            var Productions = pages.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production,
                L("Productions"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeature));

            //Producion - Production Orders
            var ProductionOrder = Productions.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_ProductionOrder,
                L("ProductionOrder"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionOrder));

            var commonProductionOrder = ProductionOrder.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_ProductionOrder_Common,
                L("Commons"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionOrder));

            commonProductionOrder.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_ProductionOrder_CanSeePrice,
                L("CanSeePrice"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionOrder));
            commonProductionOrder.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_ProductionOrder_CanEditAcccount,
                L("CanEditAccount"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.ProductionFeatureProductionOrder, AppFeatures.AccountingFeature));

            commonProductionOrder.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_ProductionOrder_CanViewAcccount,
                L("CanViewAccount"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.ProductionFeatureProductionOrder, AppFeatures.AccountingFeature));

            commonProductionOrder.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_ProductionOrder_CanViewCalculationState,
                L("CanViewCalculationState"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.ProductionFeatureProductionOrder, AppFeatures.AccountingFeature));

            commonProductionOrder.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_ProductionOrder_EditDeleteBy48hour,
                L("EditDeleteby48hour"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionOrder));

            commonProductionOrder.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_ProductionOrder_AutoInventory,
                L("AutoInventory"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionOrder));

            ProductionOrder.CreateChildPermission(
               AppPermissions.Pages_Tenant_Production_ProductionOrder_GetList,
               L("ProductionOrderList"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionOrder));

            ProductionOrder.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_ProductionOrder_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionOrder));

            ProductionOrder.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_ProductionOrder_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionOrder));

            ProductionOrder.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_ProductionOrder_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionOrder));

            ProductionOrder.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_ProductionOrder_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionOrder));

            ProductionOrder.CreateChildPermission(
              AppPermissions.Pages_Tenant_Production_ProductionOrder_Calculate,
              L("Calculate"),
              multiTenancySides: MultiTenancySides.Tenant,
              featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionOrder));


            //Producion - Production Process
            var ProductionProcess = Productions.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_Process,
                L("ProductionProcess"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionProcess));

            ProductionProcess.CreateChildPermission(
               AppPermissions.Pages_Tenant_Production_Process_GetList,
               L("ProductionProcessList"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionProcess));

            ProductionProcess.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_Process_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionProcess));

            ProductionProcess.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_Process_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionProcess));

            ProductionProcess.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_Process_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionProcess));

            ProductionProcess.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_Process_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionProcess));

            ProductionProcess.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_Process_UpdateStatusToDisable,
                L("Disable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionProcess));

            ProductionProcess.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_Process_UpdateStatusToEnable,
                L("Enable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionProcess));

            //Producion - Production Plan
            var ProductionPlan = Productions.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_Plan,
                L("ProductionPlan"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionPlan));

            ProductionPlan.CreateChildPermission(
               AppPermissions.Pages_Tenant_Production_Plan_GetList,
               L("ProductionPlanList"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionPlan));

            ProductionPlan.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_Plan_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionPlan));

            ProductionPlan.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_Plan_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionPlan));

            ProductionPlan.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_Plan_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionPlan));

            ProductionPlan.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_Plan_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionPlan));

            ProductionPlan.CreateChildPermission(
                 AppPermissions.Pages_Tenant_Production_Plan_Close,
                 L("Close"),
                 multiTenancySides: MultiTenancySides.Tenant,
                 featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionPlan));

            ProductionPlan.CreateChildPermission(
               AppPermissions.Pages_Tenant_Production_Plan_Open,
               L("Open"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionPlan));

            ProductionPlan.CreateChildPermission(
              AppPermissions.Pages_Tenant_Production_Plan_Calculate,
              L("Calculate"),
              multiTenancySides: MultiTenancySides.Tenant,
              featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionPlan));

            //Producion - Production Line
            var ProductionLine = Productions.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_Line,
                L("ProductionLine"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionLine));

            ProductionLine.CreateChildPermission(
               AppPermissions.Pages_Tenant_Production_Line_GetList,
               L("ProductionLineList"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionLine));

            ProductionLine.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_Line_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionLine));

            ProductionLine.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_Line_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionLine));

            ProductionLine.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_Line_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionLine));

            ProductionLine.CreateChildPermission(
                AppPermissions.Pages_Tenant_Production_Line_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionLine));

            ProductionLine.CreateChildPermission(
                 AppPermissions.Pages_Tenant_Production_Line_Enable,
                 L("Enable"),
                 multiTenancySides: MultiTenancySides.Tenant,
                 featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionLine));

            ProductionLine.CreateChildPermission(
               AppPermissions.Pages_Tenant_Production_Line_Disable,
               L("Disable"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.ProductionFeatureProductionLine));
            #endregion

            #region Inventory 
            var inventoryModule = pages.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys,
                L("Inventory"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeature));


            var inventoryTransactions = inventoryModule.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions,
                L("InventoryTransaction"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));

            var commonTransaction = inventoryTransactions.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Common,
                L("Commons"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));

            commonTransaction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CanSeePrice,
                L("CanSeePrice"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));

            commonTransaction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CanEditAccount,
                L("CanEditAccount"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.InventoryFeatureInventoryTransactions, AppFeatures.AccountingFeature));

            commonTransaction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CanViewAccount,
                L("CanViewAccount"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.InventoryFeatureInventoryTransactions, AppFeatures.AccountingFeature));

            commonTransaction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour,
                L("EditDeleteby48hour"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));

            inventoryTransactions.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_GetList,
                L("InventoryTransactionList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));


            var createInventory = inventoryTransactions.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));

            var itemReceiptPurchase = createInventory.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptPurchase,
                L("CreateItemReceiptPurchase"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));

            itemReceiptPurchase.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptPurchase_CanBill,
                L("FromBill"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.InventoryFeatureInventoryTransactions, AppFeatures.VendorsFeatureBills));

            itemReceiptPurchase.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptPurchase_CanPurchaseOrder,
                L("FromPurchaseOrder"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.InventoryFeatureInventoryTransactions, AppFeatures.VendorsFeaturePurchases));

            var createItemReceiptCustomerCredit = createInventory.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptCustomerCredit,
                L("CreateNewItemReceiptCustomerCredit"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));

            createItemReceiptCustomerCredit.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptCustomerCredit_CanCustomerCredit,
                L("FromSaleReturn"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureInvoices, AppFeatures.InventoryFeatureInventoryTransactions));

            createItemReceiptCustomerCredit.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptCustomerCredit_CanIssueSale,
                L("FromItemIssueSale"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));

            var createInventoryItemReceiptTransfer = createInventory.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptTransfer,
                L("CreateNewItemReceiptTransfer"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.InventoryFeatureInventoryTransactions, AppFeatures.InventoryFeatureTransferOrders));

            createInventoryItemReceiptTransfer.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptTransfer_CanTransferOrder,
                L("FromTransferOrder"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.InventoryFeatureTransferOrders, AppFeatures.InventoryFeatureInventoryTransactions));

            var createReceiptProduction = createInventory.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptProduction,
                L("CreateItemReceiptProductionOrder"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.InventoryFeatureInventoryTransactions, AppFeatures.ProductionFeature));

            createReceiptProduction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptPurchase_CanProductionOrder,
                L("FromProductionOrder"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.ProductionFeatureProductionOrder, AppFeatures.InventoryFeatureInventoryTransactions));

            createInventory.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptAdjustment,
                L("CreateNewItemReceiptAdjustment"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));

            createInventory.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptOther,
                L("CreateItemReceiptOther"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));


            var itemIssue = createInventory.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueSale,
                L("CreateItemIssueSale"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));

            itemIssue.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ItemIssuesSale_ShowConvertInvoice,
                L("ShowConvertToInvoice"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.InventoryFeatureInventoryTransactions, AppFeatures.CustomersFeatureInvoices));

            itemIssue.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ItemIssuesSale_CanSaleOrder,
                L("FromSaleOrder"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureSaleOrders, AppFeatures.InventoryFeatureInventoryTransactions));


            itemIssue.CreateChildPermission(
               AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ItemIssuesSale_CanDelivery,
               L("FromDelivery"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureDeliverySchedule, AppFeatures.InventoryFeatureInventoryTransactions));

            itemIssue.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ItemIssuesSale_CanInvoice,
                L("FromInvoice"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureInvoices, AppFeatures.InventoryFeatureInventoryTransactions));

            itemIssue.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ItemIssuesSale_EditConvertInvoice,
                L("CanEditConvertToInvoice"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.InventoryFeatureInventoryTransactions, AppFeatures.CustomersFeatureInvoices));

            var createItemIssueVendorCredit = createInventory.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueVendorCredit,
                L("CreateItemIssueVendorCredit"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));

            createItemIssueVendorCredit.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueVendorCredit_CanVendorCredit,
                L("FromPurchaseReturn"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.InventoryFeatureInventoryTransactions, AppFeatures.VendorsFeatureBills));

            createItemIssueVendorCredit.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueVendorCredit_CanItemReceiptPurchase,
                L("FromItemReceiptPurchase"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));

            var createItemIssueTransfer = createInventory.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueTransfer,
                L("CreateItemIssueTransfer"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.InventoryFeatureInventoryTransactions, AppFeatures.InventoryFeatureTransferOrders));

            createItemIssueTransfer.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueTransfer_CanTransferOrder,
                L("FromTransferOrder"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.InventoryFeatureTransferOrders, AppFeatures.InventoryFeatureInventoryTransactions));

            var createItemIssueProduction = createInventory.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueProduction,
                L("CreateItemIssueProductionOrder"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.InventoryFeatureInventoryTransactions, AppFeatures.ProductionFeature));

            createItemIssueProduction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueProduction_CanProductionOrder,
                L("FromProductionOrder"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.ProductionFeatureProductionOrder, AppFeatures.InventoryFeatureInventoryTransactions));

            createInventory.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueAdjustment,
                L("CreateItemIssueAdjustment"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));

            createInventory.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueOther,
                L("CreateItemIssueOther"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));


            inventoryTransactions.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ViewDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));

            inventoryTransactions.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));

            inventoryTransactions.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));

            var inventoryTransactionImportExcel = inventoryTransactions.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ImportExcel,
                L("ImportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));

            inventoryTransactionImportExcel.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ImportItemReceiptOther,
                L("ItemReceipt_Other"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));

            inventoryTransactionImportExcel.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ImportItemReceiptAdjustment,
                L("ItemReceipt_Adjustment"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));

            inventoryTransactionImportExcel.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ImportItemIssueAdjustment,
                L("ItemIssue_Adjustment"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));

            var inventoryTransactionExportExcel = inventoryTransactions.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ExportExcel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));

            inventoryTransactionExportExcel.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ExportItemReceiptOther,
                L("ItemReceipt_Other"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));

            inventoryTransactionExportExcel.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ExportItemReceiptAdjustment,
                L("ItemReceipt_Adjustment"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));

            inventoryTransactionExportExcel.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ExportItemIssueAdjustment,
                L("ItemIssue_Adjustment"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureInventoryTransactions));


            //Inventory - Transfer Orders
            var transferOrders = inventoryModule.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_TransferOrder,
                L("TransferOrders"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureTransferOrders));

            var commonTrasferOrder = transferOrders.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_TransferOrder_Common,
                L("Commons"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureTransferOrders));

            commonTrasferOrder.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_TransferOrder_EditDelete48hour,
                L("EditDeleteby48hour"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureTransferOrders));

            commonTrasferOrder.CreateChildPermission(
                AppPermissions.pages_Tenant_Inventory_TransferOrder_AutoInventory,
                L("AutoInventory"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureTransferOrders));

            transferOrders.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_TransferOrder_GetList,
                L("TransferOrderList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureTransferOrders));

            transferOrders.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_TransferOrder_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureTransferOrders));

            transferOrders.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_TransferOrder_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureTransferOrders));

            transferOrders.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_TransferOrder_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureTransferOrders));

            transferOrders.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_TransferOrder_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureTransferOrders));


            //Inventory - Physical Count
            var physicalCount = inventoryModule.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_PhysicalCount,
                L("PhysicalCount"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeaturePhysicalCounts));

            physicalCount.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_PhysicalCount_GetList,
                L("PhysicalCountList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeaturePhysicalCounts));

            physicalCount.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_PhysicalCount_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeaturePhysicalCounts));

            physicalCount.CreateChildPermission(
               AppPermissions.Pages_Tenant_Inventory_PhysicalCount_Print,
               L("Print"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeaturePhysicalCounts));

            physicalCount.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_PhysicalCount_ImportExcel,
                L("ImportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeaturePhysicalCounts));

            physicalCount.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_PhysicalCount_ExportPdf,
                L("ExportPdf"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeaturePhysicalCounts));

            physicalCount.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_PhysicalCount_Setting,
                L("Setting"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeaturePhysicalCounts));

            physicalCount.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_PhysicalCount_Close,
                L("Close"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeaturePhysicalCounts));

            physicalCount.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_PhysicalCount_CanSeePrice,
                L("CanSeePrice"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeaturePhysicalCounts));

            physicalCount.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_PhysicalCount_UpdateStatusToPublish,
                L("Open"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeaturePhysicalCounts));

            physicalCount.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_PhysicalCount_FillIn,
                L("FillInPhysicalCount"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeaturePhysicalCounts));

            physicalCount.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_PhysicalCount_SyncInventory,
                L("SyncInventory"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeaturePhysicalCounts));

            physicalCount.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_PhysicalCount_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeaturePhysicalCounts));

            physicalCount.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_PhysicalCount_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeaturePhysicalCounts));

            physicalCount.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_PhysicalCount_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeaturePhysicalCounts));

            var ktichenOrders = inventoryModule.CreateChildPermission(
              AppPermissions.Pages_Tenant_Inventory_KitchenOrders,
              L("KitchenOrder"),
              multiTenancySides: MultiTenancySides.Tenant,
              featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureKitchenOrders));
            ktichenOrders.CreateChildPermission(
               AppPermissions.Pages_Tenant_Inventory_KitchenOrders_Update,
               L("Update"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureKitchenOrders));
            ktichenOrders.CreateChildPermission(
                  AppPermissions.Pages_Tenant_Inventory_KitchenOrders_Create,
                  L("Create"),
                  multiTenancySides: MultiTenancySides.Tenant,
                  featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureKitchenOrders));
            ktichenOrders.CreateChildPermission(
               AppPermissions.Pages_Tenant_Inventory_KitchenOrders_Detail,
               L("View"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureKitchenOrders));
            ktichenOrders.CreateChildPermission(
              AppPermissions.Pages_Tenant_Inventory_KitchenOrders_Delete,
              L("Delete"),
              multiTenancySides: MultiTenancySides.Tenant,
              featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureKitchenOrders));
            ktichenOrders.CreateChildPermission(
             AppPermissions.Pages_Tenant_Inventory_KitchenOrders_Import,
             L("ImportExcel"),
             multiTenancySides: MultiTenancySides.Tenant,
             featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureKitchenOrders));
            ktichenOrders.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_KitchenOrders_GetList,
                L("GetList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureKitchenOrders));
            ktichenOrders.CreateChildPermission(
               AppPermissions.Pages_Tenant_Inventory_KitchenOrders_SeePrice,
               L("CanSeePrice"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeatureKitchenOrders));


            #endregion

            #region Bank
            var bankModule = pages.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks,
                L("Bank"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.BankFeature));

            var banksTrasaction = bankModule.CreateChildPermission(
                 AppPermissions.Pages_Tenant_Banks_BankTransactions,
                 L("BanksTrasaction"),
                 multiTenancySides: MultiTenancySides.Tenant,
                 featureDependency: new SimpleFeatureDependency(AppFeatures.BankFeatureBankTransaction));

            banksTrasaction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks_BankTransactions_GetList,
                L("BankTransactionList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.BankFeatureBankTransaction));

            var createBankTrasaction = banksTrasaction.CreateChildPermission(
                  AppPermissions.Pages_Tenant_Banks_Transactions_Create,
                  L("Create"),
                  multiTenancySides: MultiTenancySides.Tenant,
                  featureDependency: new SimpleFeatureDependency(AppFeatures.BankFeatureBankTransaction));

            createBankTrasaction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks_Deposits_Create,
                L("CreateDeposit"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.BankFeatureBankTransaction));

            createBankTrasaction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks_Withdraws_Create,
                L("CreateWithdraw"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.BankFeatureBankTransaction));

            createBankTrasaction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks_BankTransactions_Receivepayment_Create,
                L("CreateReceivePayment"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureReceivePayments, AppFeatures.BankFeatureBankTransaction));

            createBankTrasaction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks_BankTransactions_PayBill_Create,
                L("CreatePayBill"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.VendorsFeaturePayBills, AppFeatures.BankFeatureBankTransaction));

            var ViewTransaction = banksTrasaction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks_BankTransactions_View,
                L("View"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.BankFeatureBankTransaction));

            ViewTransaction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks_Deposits_GetDetail,
                L("ViewDeposit"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.BankFeatureBankTransaction));

            ViewTransaction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks_Withdraws_GetDetail,
                L("ViewWithdraw"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.BankFeatureBankTransaction));

            ViewTransaction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks_BankTransactions_ReceivePayment_View,
                L("ViewReceivePayment"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureReceivePayments, AppFeatures.BankFeatureBankTransaction));

            ViewTransaction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks_BankTransactions_PayBill_View,
                L("ViewPayBill"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.VendorsFeaturePayBills, AppFeatures.BankFeatureBankTransaction));

            var updateBankTrasaction = banksTrasaction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks_Transactions_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.BankFeatureBankTransaction));

            updateBankTrasaction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks_Deposits_Update,
                L("UpdateDeposit"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.BankFeatureBankTransaction));

            updateBankTrasaction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks_Withdraws_Update,
                L("UpdateWithdraw"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.BankFeatureBankTransaction));

            updateBankTrasaction.CreateChildPermission(
               AppPermissions.Pages_Tenant_Banks_BankTransactions_Receivepayment_Update,
               L("UpdateReceivePayment"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureReceivePayments, AppFeatures.BankFeatureBankTransaction));

            updateBankTrasaction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks_BankTransactions_PayBill_Update,
                L("UpdatePayBill"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.VendorsFeaturePayBills, AppFeatures.BankFeatureBankTransaction));

            var deleteTrasction = banksTrasaction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks_BankTransactions_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.BankFeatureBankTransaction));

            deleteTrasction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks_Deposits_Delete,
                L("DeleteDeposit"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.BankFeatureBankTransaction));

            deleteTrasction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks_Withdraws_Delete,
                L("DeleteWithdraw"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.BankFeatureBankTransaction));

            deleteTrasction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks_BankTransactions_ReceivePayment_Delete,
                L("DeleteReceivePayment"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.CustomersFeatureReceivePayments, AppFeatures.BankFeatureBankTransaction));

            deleteTrasction.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks_BankTransactions_PayBill_Delete,
                L("DeletePayBill"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.VendorsFeaturePayBills, AppFeatures.BankFeatureBankTransaction));



            var bankTransfer = bankModule.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks_Transfers,
                L("BankTransfer"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.BankFeatureBankTransfer));

            bankTransfer.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks_Transfers_GetList,
                L("BankTransferList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.BankFeatureBankTransfer));

            bankTransfer.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks_Transfers_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.BankFeatureBankTransfer));

            bankTransfer.CreateChildPermission(
               AppPermissions.Pages_Tenant_Banks_Transfers_GetDetail,
               L("ViewDetail"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.BankFeatureBankTransfer));

            bankTransfer.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks_Transfers_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.BankFeatureBankTransfer));

            bankTransfer.CreateChildPermission(
                AppPermissions.Pages_Tenant_Banks_Transfers_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.BankFeatureBankTransfer));


            #endregion

            #region Setup


            var setup = pages.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup,
                L("Setup"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeature));

            //Setup - Chart of Accounts
            var chartOfAccounts = setup.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_ChartOfAccounts,
                L("ChartOfAccounts"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(requiresAll: true, AppFeatures.SetupFeatureChartOfAccounts, AppFeatures.AccountingFeature));

            chartOfAccounts.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureChartOfAccounts));

            chartOfAccounts.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_Edit,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureChartOfAccounts));

            chartOfAccounts.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureChartOfAccounts));

            chartOfAccounts.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_Enable,
                L("Enable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureChartOfAccounts));

            chartOfAccounts.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_Disable,
                L("Disable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureChartOfAccounts));

            chartOfAccounts.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_ImportExcel,
                L("ImportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureChartOfAccounts));

            chartOfAccounts.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_ExportExcel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureChartOfAccounts));

            chartOfAccounts.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_ExportPdf,
                L("ExportPdf"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureChartOfAccounts));

            //Setup Taxs
            var taxes = setup.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Taxes,
                L("Taxes"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureTaxs));

            taxes.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Taxes_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureTaxs));

            taxes.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Taxes_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureTaxs));

            taxes.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Taxes_Edit,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureTaxs));

            taxes.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Taxes_Enable,
                L("Enable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureTaxs));

            taxes.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Taxes_Disable,
                L("Disable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureTaxs));

            //Setup - Items
            var itemCodeFomulas = setup.CreateChildPermission(
                 AppPermissions.Pages_Tenant_ItemCodeFormulas,
                 L("ItemCodeFormula"),
                 multiTenancySides: MultiTenancySides.Tenant,
                 featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));
            itemCodeFomulas.CreateChildPermission(
               AppPermissions.Pages_Tenant_ItemCodeFormulas_GetList,
               L("ItemCodeFormulaList"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));

            itemCodeFomulas.CreateChildPermission(
                AppPermissions.Pages_Tenant_ItemCodeFormulas_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));

            itemCodeFomulas.CreateChildPermission(
                AppPermissions.Pages_Tenant_ItemCodeFormulas_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));

            itemCodeFomulas.CreateChildPermission(
                AppPermissions.Pages_Tenant_ItemCodeFormulas_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));

            itemCodeFomulas.CreateChildPermission(
                AppPermissions.Pages_Tenant_ItemCodeFormulas_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));

            itemCodeFomulas.CreateChildPermission(
                AppPermissions.Pages_Tenant_ItemCodeFormulas_Disable,
                L("Disable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));

            itemCodeFomulas.CreateChildPermission(
              AppPermissions.Pages_Tenant_ItemCodeFormulas_UpdateAutoItemCode,
              L("UpdateAutoItemCode"),
              multiTenancySides: MultiTenancySides.Tenant,
              featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));

            itemCodeFomulas.CreateChildPermission(
            AppPermissions.Pages_Tenant_ItemCodeFormulas_UpdateItemCodeSetting,
            L("UpdateItemCodeSetting"),
            multiTenancySides: MultiTenancySides.Tenant,
            featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));

            itemCodeFomulas.CreateChildPermission(
                AppPermissions.Pages_Tenant_ItemCodeFormulas_Enable,
                L("Enable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));

            var boms = setup.CreateChildPermission(
                AppPermissions.Pages_Tenant_BOMs,
                L("BOM"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeature));
            boms.CreateChildPermission(
                AppPermissions.Pages_Tenant_BOMs_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeature));

            boms.CreateChildPermission(
                AppPermissions.Pages_Tenant_BOMs_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeature));

            boms.CreateChildPermission(
                AppPermissions.Pages_Tenant_BOMs_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeature));

            boms.CreateChildPermission(
                AppPermissions.Pages_Tenant_BOMs_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeature));

            boms.CreateChildPermission(
                AppPermissions.Pages_Tenant_BOMs_Disable,
                L("Disable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeature));


            boms.CreateChildPermission(
               AppPermissions.Pages_Tenant_BOMs_Export,
               L("ExportExcel"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeature));
            boms.CreateChildPermission(
              AppPermissions.Pages_Tenant_BOMs_Import,
              L("ImportExcel"),
              multiTenancySides: MultiTenancySides.Tenant,
              featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeature));
            boms.CreateChildPermission(
                AppPermissions.Pages_Tenant_BOMs_Enable,
                L("Enable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeature));
            commons.CreateChildPermission(
               AppPermissions.Pages_Tenant_BOMs_Find,
               L("FindBOM"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeature, AppFeatures.CommonFeature));


            var items = setup.CreateChildPermission(
                AppPermissions.Pages_Tenant_Items,
                L("Items"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));

            var commonItem = items.CreateChildPermission(
                AppPermissions.Pages_Tenant_Items_Common,
                L("Commons"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));

            commonItem.CreateChildPermission(
                AppPermissions.Pages_Tenant_SubItem_Delete,
                L("DeleteSubItem"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));

            commonItem.CreateChildPermission(
                AppPermissions.Pages_Tenant_Item_CanFilterAccount,
                L("CanFilterAccount"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));

            commonItem.CreateChildPermission(
                AppPermissions.Pages_Tenant_Item_CanSeePrice,
                L("CanSeePrice"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));

            items.CreateChildPermission(
                AppPermissions.Pages_Tenant_Item_GetList,
                L("ItemList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));

            items.CreateChildPermission(
                AppPermissions.Pages_Tenant_Item_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));

            items.CreateChildPermission(
                AppPermissions.Pages_Tenant_Item_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));

            items.CreateChildPermission(
                AppPermissions.Pages_Tenant_Item_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));

            items.CreateChildPermission(
                AppPermissions.Pages_Tenant_Item_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));

            items.CreateChildPermission(
                AppPermissions.Pages_Tenant_Item_Disable,
                L("Disable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));

            items.CreateChildPermission(
                AppPermissions.Pages_Tenant_Item_Enable,
                L("Enable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));

            items.CreateChildPermission(
                AppPermissions.Pages_Tenant_Item_Export,
                L("Export"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));

            items.CreateChildPermission(
                AppPermissions.Pages_Tenant_Item_Import,
                L("ImportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));
            items.CreateChildPermission(
              AppPermissions.Pages_Tenant_Item_Print_Barcode,
              L("PrintBarcode"),
              multiTenancySides: MultiTenancySides.Tenant,
              featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));

            items.CreateChildPermission(
                AppPermissions.Pages_Tenant_Item_ExportPdf,
                L("ExportPdf"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems));

            //Setup - Properties
            var properties = setup.CreateChildPermission(
                AppPermissions.Pages_Tenant_Properties,
                L("ItemProperty"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems, AppFeatures.SetupFeatureItemProperties));

            var commonProperty = properties.CreateChildPermission(
                 AppPermissions.Pages_Tenant_Properties_Property_Value,
                 L("PropertyValue"),
                 multiTenancySides: MultiTenancySides.Tenant,
                 featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems, AppFeatures.SetupFeatureItemProperties));

            commonProperty.CreateChildPermission(
                AppPermissions.Pages_Tenant_PropertyValue_Delete,
                L("DeletePropertyValue"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems, AppFeatures.SetupFeatureItemProperties));

            commonProperty.CreateChildPermission(
                AppPermissions.Pages_Tenant_PropertyValue_AddValue,
                L("AddPropertyValue"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems, AppFeatures.SetupFeatureItemProperties));

            commonProperty.CreateChildPermission(
                AppPermissions.Pages_Tenant_PropertyValue_EditValue,
                L("EditPropertyValue"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems, AppFeatures.SetupFeatureItemProperties));

            commonProperty.CreateChildPermission(
                AppPermissions.Pages_Tenant_PropertyValue_GetValue,
                L("GetPropertyValue"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems, AppFeatures.SetupFeatureItemProperties));

            properties.CreateChildPermission(
                AppPermissions.Pages_Tenant_Properties_GetList,
                L("ItemPropertyList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems, AppFeatures.SetupFeatureItemProperties));

            properties.CreateChildPermission(
                AppPermissions.Pages_Tenant_Properties_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems, AppFeatures.SetupFeatureItemProperties));

            properties.CreateChildPermission(
                AppPermissions.Pages_Tenant_Properties_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems, AppFeatures.SetupFeatureItemProperties));

            properties.CreateChildPermission(
                AppPermissions.Pages_Tenant_Properties_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems, AppFeatures.SetupFeatureItemProperties));

            properties.CreateChildPermission(
                AppPermissions.Pages_Tenant_Properties_Disable,
                L("Disable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems, AppFeatures.SetupFeatureItemProperties));

            properties.CreateChildPermission(
                AppPermissions.Pages_Tenant_Properties_Enable,
                L("Enable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems, AppFeatures.SetupFeatureItemProperties));

            properties.CreateChildPermission(
                AppPermissions.Pages_Tenant_Properties_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems, AppFeatures.SetupFeatureItemProperties));

            properties.CreateChildPermission(
                AppPermissions.Pages_Tenant_Properties_Import_Excel,
                L("ImportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItems, AppFeatures.SetupFeatureItemProperties));

            //Setup - Item Prices
            var itemPrices = customers.CreateChildPermission(
                AppPermissions.Pages_Tenant_ItemPrices,
                L("ItemPrices"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItemPrices));

            itemPrices.CreateChildPermission(
                AppPermissions.Pages_Tenant_ItemPrices_GetList,
                L("ItemPriceList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItemPrices));

            itemPrices.CreateChildPermission(
                AppPermissions.Pages_Tenant_ItemPrices_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItemPrices));

            itemPrices.CreateChildPermission(
               AppPermissions.Pages_Tenant_ItemPrices_Export,
               L("Export"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItemPrices));

            itemPrices.CreateChildPermission(
                AppPermissions.Pages_Tenant_ItemPrices_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItemPrices));

            itemPrices.CreateChildPermission(
                AppPermissions.Pages_Tenant_ItemPrices_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItemPrices));

            itemPrices.CreateChildPermission(
                AppPermissions.Pages_Tenant_ItemPrices_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureItemPrices));

            //Setup - Vendor
            var vendor = setup.CreateChildPermission(
                AppPermissions.Pages_Tenant_SetUp_Vendor,
                L("Vendors"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureVendors));

            vendor.CreateChildPermission(
                AppPermissions.Pages_Tenant_SetUp_Vendor_GetList,
                L("VendorList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureVendors));

            vendor.CreateChildPermission(
                 AppPermissions.Pages_Tenant_SetUp_Vendor_Create,
                 L("Create"),
                 multiTenancySides: MultiTenancySides.Tenant,
                 featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureVendors));

            vendor.CreateChildPermission(
                AppPermissions.Pages_Tenant_SetUp_Vendor_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureVendors));

            vendor.CreateChildPermission(
                AppPermissions.Pages_Tenant_SetUp_Vendor_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureVendors));

            vendor.CreateChildPermission(
                AppPermissions.Pages_Tenant_SetUp_Vendor_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureVendors));

            vendor.CreateChildPermission(
                AppPermissions.Pages_Tenant_SetUp_Vendor_Disable,
                L("Disable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureVendors));

            vendor.CreateChildPermission(
                AppPermissions.Pages_Tenant_SetUp_Vendor_Enable,
                L("Enable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureVendors));

            vendor.CreateChildPermission(
                AppPermissions.Pages_Tenant_SetUp_Vendor_ExportExcel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureVendors));

            vendor.CreateChildPermission(
                AppPermissions.Pages_Tenant_SetUp_Vendor_ImportExcel,
                L("ImportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureVendors));


            //Setup - Vendor Type
            var vendorType = vendor.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_VendorTypes,
                L("VendorType"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureVendors));

            vendorType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_VendorTypes_GetList,
                L("VendorTypeList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureVendors));

            vendorType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_VendorTypes_Create,
                L("CreateVendorType"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureVendors));

            vendorType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_VendorTypes_GetDetail,
                L("ViewDetailVendorType"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureVendors));

            vendorType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_VendorTypes_Update,
                L("UpdateVendorType"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureVendors));

            vendorType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_VendorTypes_Delete,
                L("DeleteVendorType"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureVendors));

            vendorType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_VendorTypes_Disable,
                L("DisableVendorType"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureVendors));

            vendorType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_VendorTypes_Enable,
                L("EnableVendorType"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureVendors));

            //Setup - Customer
            var customer = setup.CreateChildPermission(
                AppPermissions.Pages_Tenant_SetUp_Customer,
                L("Customers"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            customer.CreateChildPermission(
                AppPermissions.Pages_Tenant_SetUp_Customer_GetList,
                L("CustomerList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            customer.CreateChildPermission(
                AppPermissions.Pages_Tenant_SetUp_Customer_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            customer.CreateChildPermission(
                AppPermissions.Pages_Tenant_SetUp_Customer_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            customer.CreateChildPermission(
                AppPermissions.Pages_Tenant_SetUp_Customer_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            customer.CreateChildPermission(
                AppPermissions.Pages_Tenant_SetUp_Customer_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            customer.CreateChildPermission(
                AppPermissions.Pages_Tenant_SetUp_Customer_Disable,
                L("Disable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            customer.CreateChildPermission(
                AppPermissions.Pages_Tenant_SetUp_Customer_Enable,
                L("Enable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            customer.CreateChildPermission(
                AppPermissions.Pages_Tenant_SetUp_Customer_ExportExcel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            customer.CreateChildPermission(
                AppPermissions.Pages_Tenant_SetUp_Customer_ImportExcel,
                L("ImportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            //Setup - Customer Type
            var customerType = customer.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_CustomerTypes,
                L("CustomerTypes"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            customerType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_CustomerTypes_GetList,
                L("CustomerTypeList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            customerType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_CustomerTypes_Create,
                L("CreateCustomerType"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            customerType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_CustomerTypes_GetDetail,
                L("ViewDetailCustomerType"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            customerType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_CustomerTypes_Update,
                L("UpdateCustomerType"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            customerType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_CustomerTypes_Delete,
                L("DeleteCustomerType"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            customerType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_CustomerTypes_Disable,
                L("DisableCustomerType"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            customerType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_CustomerTypes_Enable,
                L("EnableCustomerType"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            //Setup - Sale Type
            var transactionType = setup.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_TransactionTypes,
                L("SaleType"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            transactionType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_TransactionTypes_GetList,
                L("SaleTypeList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            transactionType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_TransactionTypes_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            transactionType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_TransactionTypes_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            transactionType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_TransactionTypes_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            transactionType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_TransactionTypes_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            transactionType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_TransactionTypes_Disable,
                L("Disable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            transactionType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_TransactionTypes_Enable,
                L("Enable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCustomers));

            //Inventory Transaction Type
            var journalTransactionType = setup.CreateChildPermission(AppPermissions.Pages_Tenant_Inventory_JournalTransactionTypes, L("InventoryTransactionType"),
               multiTenancySides: MultiTenancySides.Tenant, featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeature));

            journalTransactionType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_JournalTransactionTypes_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeature));

            journalTransactionType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_JournalTransactionTypes_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeature));

            journalTransactionType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_JournalTransactionTypes_GetList,
                L("GetList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeature));

            journalTransactionType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_JournalTransactionTypes_Enable,
                L("Enable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeature));

            journalTransactionType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_JournalTransactionTypes_Disable,
                L("Disable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeature));

            journalTransactionType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_JournalTransactionTypes_GetDetail,
                L("View"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeature));

            journalTransactionType.CreateChildPermission(
                AppPermissions.Pages_Tenant_Inventory_JournalTransactionTypes_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.InventoryFeature));

            //Setup - Class
            var classes = setup.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Classes,
                L("Class"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureClasss));

            classes.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Classes_GetList,
                L("ClassList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureClasss));

            classes.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Classes_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureClasss));

            classes.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Classes_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureClasss));

            classes.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Classes_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureClasss));

            classes.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Classes_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureClasss));

            classes.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Classes_Disable,
                L("Disable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureClasss));

            classes.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Classes_Enable,
                L("Enable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureClasss));
            //Setup - Locations
            var locations = setup.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Locations,
                L("Locations"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureLocations));

            locations.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Locations_GetList,
                L("LocationList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureLocations));

            locations.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Locations_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureLocations));

            locations.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Locations_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureLocations));

            locations.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Locations_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureLocations));

            locations.CreateChildPermission(
               AppPermissions.Pages_Tenant_Setup_Locations_Delete,
               L("Delete"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureLocations));

            locations.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Locations_Disable,
                L("Disable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureLocations));

            locations.CreateChildPermission(
               AppPermissions.Pages_Tenant_Setup_Locations_Enable,
               L("Enable"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureLocations));

            //Setup - Lots
            var lots = setup.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Lots,
                L("Lots"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureLocations));

            lots.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Lots_GetList,
                L("LotList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureLocations));

            lots.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Lots_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureLocations));
            lots.CreateChildPermission(
               AppPermissions.Pages_Tenant_Setup_Lots_Import,
               L("ImportExcel"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureLocations));
            lots.CreateChildPermission(
              AppPermissions.Pages_Tenant_Setup_Lots_Export,
              L("ExportExcel"),
              multiTenancySides: MultiTenancySides.Tenant,
              featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureLocations));

            lots.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Lots_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureLocations));

            lots.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Lots_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureLocations));

            lots.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Lots_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureLocations));

            lots.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Lots_Disable,
                L("Disable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureLocations));

            lots.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_Lots_Enable,
                L("Enable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureLocations));

            // company profile
            var companyProfile = setup.CreateChildPermission(
                AppPermissions.Pages_Tenant_CompanyProfile_Organazition,
                L("CompanyProfile"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCompanyProfile));

            companyProfile.CreateChildPermission(
                AppPermissions.Pages_Tenants_CompanyProfile_GetDetail,
                L("ViewCompanyProfile"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCompanyProfile));

            companyProfile.CreateChildPermission(
              AppPermissions.Pages_Tenants_CompanyProfile_ClearDefaultValue,
              L("ClearDefaultValue"),
              multiTenancySides: MultiTenancySides.Tenant,
              featureDependency: new SimpleFeatureDependency(true, AppFeatures.SetupFeatureCompanyProfile, AppFeatures.ClearDefaultValueFeature));

            companyProfile.CreateChildPermission(
                AppPermissions.Pages_Tenants_CompanyProfile_Update,
                L("UpdateCompanyProfile"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureCompanyProfile));


            //Setup - Payment Method
            var paymentMethod = setup.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_PaymentMethods,
                L("PaymentMethod"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS, AppFeatures.VendorsFeaturePayBills, AppFeatures.CustomersFeatureReceivePayments));

            paymentMethod.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_PaymentMethods_GetList,
                L("PaymentMethodList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS, AppFeatures.VendorsFeaturePayBills, AppFeatures.CustomersFeatureReceivePayments));

            paymentMethod.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_PaymentMethods_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS, AppFeatures.VendorsFeaturePayBills, AppFeatures.CustomersFeatureReceivePayments));

            paymentMethod.CreateChildPermission(
               AppPermissions.Pages_Tenant_Setup_PaymentMethods_Create,
               L("Create"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS, AppFeatures.VendorsFeaturePayBills, AppFeatures.CustomersFeatureReceivePayments));

            paymentMethod.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_PaymentMethods_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS, AppFeatures.VendorsFeaturePayBills, AppFeatures.CustomersFeatureReceivePayments));

            paymentMethod.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_PaymentMethods_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS, AppFeatures.VendorsFeaturePayBills, AppFeatures.CustomersFeatureReceivePayments));

            paymentMethod.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_PaymentMethods_Disable,
                L("Disable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS, AppFeatures.VendorsFeaturePayBills, AppFeatures.CustomersFeatureReceivePayments));

            paymentMethod.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_PaymentMethods_Enable,
                L("Enable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeaturePOS, AppFeatures.VendorsFeaturePayBills, AppFeatures.CustomersFeatureReceivePayments));

            //Setup - Exchanges
            var exchanges = setup.CreateChildPermission(
                AppPermissions.Pages_Tenant_Exchanges,
                L("Exchanges"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureExchangeRate));

            exchanges.CreateChildPermission(
                AppPermissions.Pages_Tenant_Exchanges_GetList,
                L("ExchangeRateList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureExchangeRate));

            exchanges.CreateChildPermission(
                AppPermissions.Pages_Tenant_Exchanges_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureExchangeRate));

            exchanges.CreateChildPermission(
                AppPermissions.Pages_Tenant_Exchanges_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureExchangeRate));

            exchanges.CreateChildPermission(
                AppPermissions.Pages_Tenant_Exchanges_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureExchangeRate));

            exchanges.CreateChildPermission(
               AppPermissions.Pages_Tenant_Exchanges_Delete,
               L("Delete"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureExchangeRate));

            exchanges.CreateChildPermission(
                AppPermissions.Pages_Tenant_Exchanges_ApplyRate,
                L("ApplyRate"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.SetupFeatureExchangeRate));

            //Setup - Form Tempaltes
            var invoiceTemplate = setup.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_InvoiceTemplates,
                L("FormTemplates"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices, AppFeatures.CustomersFeatureSaleOrders));

            invoiceTemplate.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_InvoiceTemplates_GetList,
                L("FormTemplateList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices, AppFeatures.CustomersFeatureSaleOrders));

            invoiceTemplate.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_InvoiceTemplates_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices, AppFeatures.CustomersFeatureSaleOrders));

            invoiceTemplate.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_InvoiceTemplates_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices, AppFeatures.CustomersFeatureSaleOrders));

            invoiceTemplate.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_InvoiceTemplates_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices, AppFeatures.CustomersFeatureSaleOrders));

            invoiceTemplate.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_InvoiceTemplates_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices, AppFeatures.CustomersFeatureSaleOrders));

            invoiceTemplate.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_InvoiceTemplates_Enable,
                L("Enable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices, AppFeatures.CustomersFeatureSaleOrders));

            invoiceTemplate.CreateChildPermission(
                AppPermissions.Pages_Tenant_Customer_InvoiceTemplates_Disable,
                L("Disable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.CustomersFeatureInvoices, AppFeatures.CustomersFeatureSaleOrders));


            //Report - Accounting - Cash Flow
            var cashFlowTemplate = setup.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_CashFlowTemplate,
                L("CashFlowTemplate"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureCashFlow));

            cashFlowTemplate.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureCashFlow));

            cashFlowTemplate.CreateChildPermission(
               AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Update,
               L("Update"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureCashFlow));

            cashFlowTemplate.CreateChildPermission(
               AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Delete,
               L("Delete"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureCashFlow));

            cashFlowTemplate.CreateChildPermission(
              AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Enable,
              L("Enable"),
              multiTenancySides: MultiTenancySides.Tenant,
              featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureCashFlow));

            cashFlowTemplate.CreateChildPermission(
              AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Disable,
              L("Disable"),
              multiTenancySides: MultiTenancySides.Tenant,
              featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureCashFlow));

            
            #endregion

            #region QC Test
            var qcTests = pages.CreateChildPermission(
                  AppPermissions.Pages_Tenant_QCTests,
                  L("QCTests"),
                  multiTenancySides: MultiTenancySides.Tenant,
                  featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            //QC Sample
            var qcSamples = qcTests.CreateChildPermission(
                AppPermissions.Pages_Tenant_QCSamples,
                L("QCSamples"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            qcSamples.CreateChildPermission(
                AppPermissions.Pages_Tenant_QCSamples_GetList,
                L("QCSampleList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            qcSamples.CreateChildPermission(
                AppPermissions.Pages_Tenant_QCSamples_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            qcSamples.CreateChildPermission(
                AppPermissions.Pages_Tenant_QCSamples_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            qcSamples.CreateChildPermission(
                AppPermissions.Pages_Tenant_QCSamples_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            qcSamples.CreateChildPermission(
               AppPermissions.Pages_Tenant_QCSamples_Delete,
               L("Delete"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            //Lab Test Request
            var labTestRequests = qcTests.CreateChildPermission(
               AppPermissions.Pages_Tenant_LabTestRequests,
               L("LabTestRequests"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            labTestRequests.CreateChildPermission(
                AppPermissions.Pages_Tenant_LabTestRequests_GetList,
                L("LabTestRequestList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            labTestRequests.CreateChildPermission(
                AppPermissions.Pages_Tenant_LabTestRequests_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            labTestRequests.CreateChildPermission(
                AppPermissions.Pages_Tenant_LabTestRequests_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            labTestRequests.CreateChildPermission(
                AppPermissions.Pages_Tenant_LabTestRequests_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            labTestRequests.CreateChildPermission(
                AppPermissions.Pages_Tenant_LabTestRequests_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            labTestRequests.CreateChildPermission(
                AppPermissions.Pages_Tenant_LabTestRequests_ChangeStatus,
                L("ChangeStatus"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            //Lab Test Result
            var labTestResults = qcTests.CreateChildPermission(
               AppPermissions.Pages_Tenant_LabTestResults,
               L("LabTestResults"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            labTestResults.CreateChildPermission(
                AppPermissions.Pages_Tenant_LabTestResults_GetList,
                L("LabTestResultList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            labTestResults.CreateChildPermission(
                AppPermissions.Pages_Tenant_LabTestResults_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            labTestResults.CreateChildPermission(
                AppPermissions.Pages_Tenant_LabTestResults_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            labTestResults.CreateChildPermission(
                AppPermissions.Pages_Tenant_LabTestResults_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            labTestResults.CreateChildPermission(
               AppPermissions.Pages_Tenant_LabTestResults_Delete,
               L("Delete"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            //Setup - TestParameters
            var testParameters = qcTests.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_TestParameters,
                L("TestParameters"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            testParameters.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_TestParameters_GetList,
                L("TestParameterList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            testParameters.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_TestParameters_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            testParameters.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_TestParameters_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            testParameters.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_TestParameters_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            testParameters.CreateChildPermission(
               AppPermissions.Pages_Tenant_Setup_TestParameters_Delete,
               L("Delete"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            testParameters.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_TestParameters_Disable,
                L("Disable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            testParameters.CreateChildPermission(
               AppPermissions.Pages_Tenant_Setup_TestParameters_Enable,
               L("Enable"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            //Setup - QCTestTemplates
            var qcTestTemplates = qcTests.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_QCTestTemplates,
                L("QCTestTemplates"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            qcTestTemplates.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_QCTestTemplates_GetList,
                L("QCTestTemplateList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            qcTestTemplates.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_QCTestTemplates_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            qcTestTemplates.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_QCTestTemplates_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            qcTestTemplates.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_QCTestTemplates_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            qcTestTemplates.CreateChildPermission(
               AppPermissions.Pages_Tenant_Setup_QCTestTemplates_Delete,
               L("Delete"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            qcTestTemplates.CreateChildPermission(
                AppPermissions.Pages_Tenant_Setup_QCTestTemplates_Disable,
                L("Disable"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            qcTestTemplates.CreateChildPermission(
               AppPermissions.Pages_Tenant_Setup_QCTestTemplates_Enable,
               L("Enable"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.QCTestFeature));

            #endregion

            #region Accounting
            var Accounting = pages.CreateChildPermission(
                AppPermissions.Pages_Tenant_Accounting,
                L("Accounting"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.AccountingFeature));

            //Accounting - General Journal
            var Journals = Accounting.CreateChildPermission(
                AppPermissions.Pages_Tenant_Accounting_Journals,
                L("GeneralJournal"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.AccountingFeatureGeneralJournal));

            Journals.CreateChildPermission(
                AppPermissions.Pages_Tenant_Accounting_Journals_GetList,
                L("JournalList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.AccountingFeatureGeneralJournal));

            Journals.CreateChildPermission(
                AppPermissions.Pages_Tenant_Accounting_Journals_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.AccountingFeatureGeneralJournal));

            Journals.CreateChildPermission(
                AppPermissions.Pages_Tenant_Accounting_Journals_GetDetail,
                L("ViewDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.AccountingFeatureGeneralJournal));

            Journals.CreateChildPermission(
                AppPermissions.Pages_Tenant_Accounting_Journals_Update,
                L("Update"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.AccountingFeatureGeneralJournal));

            Journals.CreateChildPermission(
                AppPermissions.Pages_Tenant_Accounting_Journals_Delete,
                L("Delete"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.AccountingFeatureGeneralJournal));

            Journals.CreateChildPermission(
               AppPermissions.Pages_Tenant_Accounting_Journals_ImportExcel,
               L("ImportExcel"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.AccountingFeatureGeneralJournal));

            Journals.CreateChildPermission(
               AppPermissions.Pages_Tenant_Accounting_Journals_ExportExcel,
               L("ExportExcelTemplate"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.AccountingFeatureGeneralJournal));


            // Close Periods          
            var closePeriod = pages.CreateChildPermission(
               AppPermissions.Pages_Tenant_Close_Period,
               L("ClosePeriods"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.ClosePeriodFeature));

            closePeriod.CreateChildPermission(
                AppPermissions.Pages_Tenant_Close_Period_GetList,
                L("ClosePeriodList"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ClosePeriodFeature));

            closePeriod.CreateChildPermission(
                AppPermissions.Pages_Tenant_Close_Period_Create,
                L("Create"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ClosePeriodFeature));

            closePeriod.CreateChildPermission(
                AppPermissions.Pages_Tenant_Close_Period_Delete,
                L("ReversePeriod"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ClosePeriodFeature));


            //Accounting - Lock Transaction
            var Locks = Accounting.CreateChildPermission(
                AppPermissions.Pages_Tenant_Accounting_Locks_GetList,
                L("LockTransaction"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.AccountingFeatureClosePeriod));

            Locks.CreateChildPermission(
                AppPermissions.Pages_Tenant_Accounting_Locks_CreateOrUpdate,
                L("CreateOrUpdate"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.AccountingFeatureClosePeriod));

            Locks.CreateChildPermission(
                AppPermissions.Pages_Tenant_Accounting_Locks_GeneratePasswork,
                L("GeneratePasswork"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.AccountingFeatureClosePeriod));

            #endregion

            #region Report
            var reports = pages.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report,
                L("Report"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeature, AppFeatures.ReportFeature));

            var commonReports = reports.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Common,
                L("Commons"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeature, AppFeatures.ReportFeature));

            commonReports.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_ViewTemplate,
                L("ViewTemplate"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeature, AppFeatures.ReportFeature));

            commonReports.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_ViewOriginal,
                L("ViewAllTemplate"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeature, AppFeatures.ReportFeature));

            commonReports.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Template_Create,
                L("CreateTemplate"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeature, AppFeatures.ReportFeature));

            commonReports.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Template_Update,
                L("UpdateTemplate"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeature, AppFeatures.ReportFeature));

            commonReports.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Template_Delete,
                L("DeleteTemplate"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeature, AppFeatures.ReportFeature));

            commonReports.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_CanEditAllTemplate,
                L("CanEditAllTemplate"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeature, AppFeatures.ReportFeature));


            //Report - Inventory
            var inventoryReport = reports.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Inventory,
                L("Inventory"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureInventory, AppFeatures.ReportFeatureInventory));

            //Report - Inventory - Stock Balance
            var stockBalanceReport = inventoryReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Stock_Balance,
                L("StockBalance"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureStokBalanceReport, AppFeatures.ReportFeatureStokBalanceReport));

            stockBalanceReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Stock_Balance_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureStokBalanceReport, AppFeatures.ReportFeatureStokBalanceReport));

            stockBalanceReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Stock_Balance_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureStokBalanceReport, AppFeatures.ReportFeatureStokBalanceReport));

            //Report - Inventory - Asset Balance
            var assetBalanceReport = inventoryReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Asset_Balance,
                L("AssetBalance"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureAssetBalanceReport));

            assetBalanceReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Asset_Balance_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureAssetBalanceReport));

            assetBalanceReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Asset_Balance_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureAssetBalanceReport));

            //Report - Inventory - Inventory Transaction
            var inventoryTransactionReport = inventoryReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Inventory_InventoryTransaction,
                L("InventoryTransaction"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureInventoryTransactionReport));

            inventoryTransactionReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Inventory_InventoryTransaction_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureInventoryTransactionReport));

            inventoryTransactionReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Inventory_InventoryTransaction_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureInventoryTransactionReport));

            //Report - Inventory - Inentory Valuation Summary
            var inventorySummaryReport = inventoryReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Inventory_Summary,
                L("InventoryValuationSummary"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureInventoryValuationSummaryReport));

            inventorySummaryReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Inventory_Summary_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureInventoryValuationSummaryReport));

            inventorySummaryReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Inventory_Summary_RecalculateAvg,
                L("RecalculateAvgCost"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureInventoryValuationSummaryReport));

            inventorySummaryReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Inventory_Summary_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureInventoryValuationSummaryReport));

            //Report - Inventory - Inventory Valuation Detail
            var inventoryDetailReport = inventoryReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Inventory_Detail,
                L("InventoryValuationDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureInventoryValuationDetailReport));

            inventoryDetailReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Inventory_Detail_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureInventoryValuationDetailReport));

            inventoryDetailReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Inventory_Detail_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureInventoryValuationDetailReport));

            inventoryDetailReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Inventory_Detail_RecalculateAvg,
                L("RecalculateAvgCost"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeature));

            //Report - Traceability
            var traceabilityReport = inventoryReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Traceability,
                L("BatchNoTraceability"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureTraceabilityReport));

            traceabilityReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Traceability_Print,
                L("Print"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureTraceabilityReport));

            traceabilityReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Traceability_ViewCustomer,
                L("ViewCustomer"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureTraceabilityReport));

            traceabilityReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Traceability_Download,
                L("Download"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureTraceabilityReport));

            //Report - BatchNo Balance
            var batchNoBalanceReport = inventoryReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_BatchNoBalance,
                L("BatchNoBalance"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureBatchNoBalanceReport));

            batchNoBalanceReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_BatchNoBalance_ExportPdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureBatchNoBalanceReport));

            batchNoBalanceReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_BatchNoBalance_ExportExcel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureBatchNoBalanceReport));


            var productionReport = reports.CreateChildPermission(
               AppPermissions.Pages_Tenant_Report_Production,
               L("Production"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureProductionReport));

            //Report - Production
            var productionPlanReport = productionReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_ProductionPlan,
                L("ProductionPlan"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureProductionPlanReport));

            productionPlanReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_ProductionPlan_ExportPdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureProductionPlanReport));

            productionPlanReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_ProductionPlan_ExportExcel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureProductionPlanReport));

            productionPlanReport.CreateChildPermission(
               AppPermissions.Pages_Tenant_Report_ProductionPlan_Calculate,
               L("Calculate"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureProductionPlanReport));

            //Report - Production Order
            var productionOrderReport = productionReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_ProductionOrder,
                L("ProductionOrder"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureProductionOrderReport));

            productionOrderReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_ProductionOrder_ExportPdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureProductionOrderReport));

            productionOrderReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_ProductionOrder_ExportExcel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureProductionOrderReport));

            productionOrderReport.CreateChildPermission(
               AppPermissions.Pages_Tenant_Report_ProductionOrder_Calculate,
               L("Calculate"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureProductionOrderReport));

            //Report - Vendor 
            var vendorReport = reports.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Vendor,
                L("Vendor"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureVendor));

            //Report - Vendor - Purchasing
            var purchaseReport = vendorReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Purchasing,
                L("Purchasing"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeaturePurchasing));

            purchaseReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Purchasing_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeaturePurchasing));

            purchaseReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Purchasing_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeaturePurchasing));

            //Report - Vendor - Vendor Aging
            var vendorAgingReport = vendorReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Vendor_VendorAging,
                L("VendorAging"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureVendorAging));

            vendorAgingReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Vendor_VendorAging_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureVendorAging));

            vendorAgingReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Vendor_VendorAging_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureVendorAging));

            //Report - Vendor - Vendor By Bill
            var vendorByBillReport = vendorReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Vendor_VendorByBill,
                L("VendorByBill"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureVendorByBill));

            vendorByBillReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Vendor_VendorByBill_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureVendorByBill));

            vendorByBillReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Vendor_VendorByBill_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureVendorByBill));

            //Report - Vendor - Vendor By Bill
            var purchaseByItemPropertyReport = vendorReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Vendor_PurchaseByItemProperty,
                L("PurchaseByItemProperty"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeaturePurchasing));

            purchaseByItemPropertyReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Vendor_PurchaseByItemProperty_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeaturePurchasing));

            purchaseByItemPropertyReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Vendor_PurchaseByItemProperty_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeaturePurchasing));

            //Report - Vendor - Vendor By Bill
            var qcTestReport = vendorReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_QCTest,
                L("QCTest"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureQCTest));

            qcTestReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_QCTest_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureQCTest));

            qcTestReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_QCTest_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureQCTest));

            //Report - Customer
            var customerReport = reports.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Customer,
                L("Customer"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureCustomer));

            //Report - Customer - Sale Invoice
            var saleInvoice = customerReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_SaleInvoice,
                L("SaleInvoice"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureSaleInvoice));

            saleInvoice.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_SaleInvoice_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureSaleInvoice));

            saleInvoice.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_SaleInvoice_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureSaleInvoice));


            //Report Customer - Sale Invoice Detail
            var saleInvoiceDetail = customerReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_SaleInvoiceDetail,
                L("SaleInvoiceDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureSaleInvoice));

            saleInvoiceDetail.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_SaleInvoiceDetail_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureSaleInvoice));

            saleInvoiceDetail.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_SaleInvoiceDetail_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureSaleInvoice));

            //Report - Customer - Sale Invoice
            var saleInvoiceByProperty = customerReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_SaleInvoiceByProperty,
                L("SaleInvoiceByItemProperty"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureSaleInvoice));

            saleInvoiceByProperty.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_SaleInvoiceByProperty_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureSaleInvoice));

            saleInvoiceByProperty.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_SaleInvoiceByProperty_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureSaleInvoice));

            //Report - Profit by Invoice
            var profitByInvoiceReport = customerReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_ProfitByInvoice,
                L("ProfitByInvoice"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureProfitByInvoice));

            profitByInvoiceReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_ProfitByInvoice_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureProfitByInvoice));

            profitByInvoiceReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_ProfitByInvoice_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureProfitByInvoice));

            //var saleReturn = customerReport.CreateChildPermission(
            //        AppPermissions.Pages_Tenant_Report_SaleReturn,
            //        L("SaleReturn"),
            //        multiTenancySides: MultiTenancySides.Tenant,
            //        featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeature, AppFeatures.ReportFeature));
            //saleReturn.CreateChildPermission(
            //       AppPermissions.Pages_Tenant_Report_SaleReturn_Export_Excel,
            //       L("ExportExcel"),
            //       multiTenancySides: MultiTenancySides.Tenant,
            //       featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeature, AppFeatures.ReportFeature));
            //saleReturn.CreateChildPermission(
            //     AppPermissions.Pages_Tenant_Report_SaleReturn_Export_Pdf,
            //     L("ExportPDF"),
            //     multiTenancySides: MultiTenancySides.Tenant,
            //     featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeature, AppFeatures.ReportFeature));

            //Report - Customer - Customer Aging
            var customerAging = customerReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Customer_CustomerAging,
                L("CustomerAging"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureCustomerAging));

            customerAging.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Customer_CustomerAging_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureCustomerAging));

            customerAging.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Customer_CustomerAging_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureCustomerAging));

            //Report - Customer - Custmoer By Invoice
            var customerByInvoice = customerReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Customer_CustomerByInvoice,
                L("CustomerByInvoice"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureCustomerByInvoice));

            customerByInvoice.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Customer_CustomerByInvoice_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureCustomerByInvoice));

            customerByInvoice.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Customer_CustomerByInvoice_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureCustomerByInvoice));

            //Report - Customer - Custmoer By Invoice With Payment
            var customerByInvoiceWithPayment = customerReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Customer_CustomerByInvoiceWithPayment,
                L("ARByInvoiceWithPayment"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureCustomerByInvoiceWithPayment));

            customerByInvoiceWithPayment.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Customer_CustomerByInvoiceWithPayment_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureCustomerByInvoiceWithPayment));

            customerByInvoiceWithPayment.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Customer_CustomerByInvoiceWithPayment_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureCustomerByInvoiceWithPayment));

            //Report - Accounting
            var accountingReport = reports.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Accounting,
                L("Accounting"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureAccounting, AppFeatures.ReportFeatureAccounting));

            //Report - Accounting - Journal
            var journal = accountingReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Journal,
                L("Journal"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureJournal, AppFeatures.ReportFeatureJournal));

            journal.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Journal_Update,
                L("JournalUpdate"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureJournal, AppFeatures.ReportFeatureJournal));

            journal.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Journal_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureJournal, AppFeatures.ReportFeatureJournal));

            journal.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Journal_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureJournal, AppFeatures.ReportFeatureJournal));


            //Report - Accounting - Profit & Loss
            var incomeReport = accountingReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Income,
                L("ProfitAndLoss"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureProfitAndLoss, AppFeatures.ReportFeatureProfitAndLoss));

            incomeReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Income_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeature, AppFeatures.ReportFeature));

            incomeReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Income_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeature, AppFeatures.ReportFeature));

            //Report - Accounting - Balance Sheet
            var balanceSheetReport = accountingReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_BalanceSheet,
                L("BalanceSheet"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureBalanceSheet));

            balanceSheetReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_BalanceSheet_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureBalanceSheet));

            balanceSheetReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_BalanceSheet_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureBalanceSheet));

            //Report - Accounting - Balance Sheet
            var trialBalanceReport = accountingReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_TrialBalance,
                L("TrialBalance"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureTrialBalance));

            trialBalanceReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_TrialBalance_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureTrialBalance));

            trialBalanceReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_TrialBalance_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureTrialBalance));

            //Report - Accounting - Ledger
            var ledgerReport = accountingReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Ledger,
                L("Ledger"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureLedger, AppFeatures.ReportFeatureLedger));

            ledgerReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Ledger_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureLedger, AppFeatures.ReportFeatureLedger));

            ledgerReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Ledger_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureLedger, AppFeatures.ReportFeatureLedger));

            //Report - Accounting - Cash
            var cashReport = accountingReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Cash,
                L("CashReport"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureCash, AppFeatures.ReportFeatureCash));

            cashReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Cash_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureCash, AppFeatures.ReportFeatureCash));

            cashReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_Cash_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureCash, AppFeatures.ReportFeatureCash));

            //Report - Accounting - Cash Flow
            var cashFlowReport = accountingReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_CashFlow,
                L("CashFlowReport"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureCashFlow, AppFeatures.ReportFeatureCashFlow));

            cashFlowReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_CashFlow_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureCashFlow, AppFeatures.ReportFeatureCashFlow));

            cashFlowReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_CashFlow_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureCashFlow, AppFeatures.ReportFeatureCashFlow));

            //Report - Purchase Order
            var purchaseOrderReport = reports.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_PurchaseOrder,
                L("PurchaseOrder"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeaturePurchaseOrder));

            purchaseOrderReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_PurchaseOrder_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureSaleOrder));

            purchaseOrderReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_PurchaseOrder_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureSaleOrder));

            //Report - Sale Order
            var saleOrderReport = reports.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_SaleOrder,
                L("SaleOrder"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureSaleOrder));

            saleOrderReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_SaleOrder_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureSaleOrder));

            saleOrderReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_SaleOrder_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureSaleOrder));


            //Report - Sale Order Detail
            var saleOrderDetailReport = reports.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_SaleOrderDetail,
                L("SaleOrderDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureSaleOrder));

            saleOrderDetailReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_SaleOrderDetail_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureSaleOrder));

            saleOrderDetailReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_SaleOrderDetail_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureSaleOrder));

            //Report - Sale Order Detail
            var saleOrderByItemPropertyReport = reports.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_SaleOrderByItemProperty,
                L("SaleOrderByItemProperty"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureSaleOrder));

            saleOrderByItemPropertyReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_SaleOrderByItemProperty_Export_Excel,
                L("ExportExcel"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureSaleOrder));

            saleOrderByItemPropertyReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_SaleOrderByItemProperty_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureSaleOrder));

            //Report - Sale Order Detail
            var deliveryScheduleReport = reports.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_DeliverySchedule,
                L("DeliverySchedule"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureDeliverySchedule));
           
            var deliveryScheduleDetail = deliveryScheduleReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_DeliverySchedule_Detail,
                L("DeliveryScheduleDetail"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureSaleOrder));

            deliveryScheduleDetail.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_DeliverySchedule_Detail_Export_Pdf,
                L("ExportPDF"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureDeliverySchedule));
           
            deliveryScheduleDetail.CreateChildPermission(
               AppPermissions.Pages_Tenant_Report_DeliverySchedule_Detail_Export_Excel,
               L("ExportExcel"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureDeliverySchedule));


            var deliveryScheduleSummary = deliveryScheduleReport.CreateChildPermission(
              AppPermissions.Pages_Tenant_Report_DeliverySchedule_Summary,
              L("DeliveryScheduleSummary"),
              multiTenancySides: MultiTenancySides.Tenant,
              featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureSaleOrder));

            deliveryScheduleSummary.CreateChildPermission(
               AppPermissions.Pages_Tenant_Report_DeliverySchedule_Summary_Export_Pdf,
               L("ExportPDF"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureDeliverySchedule));

            deliveryScheduleSummary.CreateChildPermission(
               AppPermissions.Pages_Tenant_Report_DeliverySchedule_Summary_Export_Excel,
               L("ExportExcel"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureDeliverySchedule));

            var deliveryScheduleByItemProperty = deliveryScheduleReport.CreateChildPermission(
                AppPermissions.Pages_Tenant_Report_DeliveryScheduleByItemProperty,
                L("DeliveryScheduleByItemProperty"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureSaleOrder));

            deliveryScheduleByItemProperty.CreateChildPermission(
               AppPermissions.Pages_Tenant_Report_DeliveryScheduleByItemProperty_Export_Pdf,
               L("ExportPDF"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureDeliverySchedule));

            deliveryScheduleByItemProperty.CreateChildPermission(
               AppPermissions.Pages_Tenant_Report_DeliveryScheduleByItemProperty_Export_Excel,
               L("ExportExcel"),
               multiTenancySides: MultiTenancySides.Tenant,
               featureDependency: new SimpleFeatureDependency(AppFeatures.ReportFeatureDeliverySchedule));

            #endregion

            #region User Activities
            var userActivities = pages.CreateChildPermission(
                AppPermissions.Pages_Tenant_UserActivities,
                L("UserActivites"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.UserActivitesFeature));

            userActivities.CreateChildPermission(
                AppPermissions.Pages_Tenant_UserActivities_GetList,
                L("GetListUserActivity"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.UserActivitesFeature));

            userActivities.CreateChildPermission(
                AppPermissions.Pages_Tenant_UserActivities_SeeAll,
                L("SeeAll"),
                multiTenancySides: MultiTenancySides.Tenant,
                featureDependency: new SimpleFeatureDependency(AppFeatures.UserActivitesFeature));

            userActivities.CreateChildPermission(
                AppPermissions.Pages_Tenant_UserActivities_SeeMine,
                L("SeeMine"),
                featureDependency: new SimpleFeatureDependency(AppFeatures.UserActivitesFeature));

            userActivities.CreateChildPermission(
                AppPermissions.Pages_Tenant_UserActivities_ExportExcel,
                L("ExportExcel"),
                featureDependency: new SimpleFeatureDependency(AppFeatures.UserActivitesFeature));

            #endregion

            //HOST-SPECIFIC PERMISSIONS
            #region Dash Board
            pages.CreateChildPermission(AppPermissions.Pages_Administration_Host_Dashboard, L("Dashboard"), multiTenancySides: MultiTenancySides.Host);
            #endregion

            #region Tenants

            var tenants = pages.CreateChildPermission(AppPermissions.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Create, L("CreatingNewTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Edit, L("EditingTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_ChangeFeatures, L("ChangingFeatures"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Delete, L("DeletingTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Impersonation, L("LoginForTenants"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_GetList, L("GetListTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_EnableDebugMode, L("EnableDebugMode"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_DisableDebugMode, L("DisableDebugMode"), multiTenancySides: MultiTenancySides.Host);
            #endregion
            #region Host SignUp
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_SignUps_Disable, L("DisableSignUp"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_SignUps_Enable, L("EnableSignUp"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_SignUps_GetList, L("GetListSignUp"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_SignUps_Delete, L("DeleteSignUp"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_SignUps_Find, L("FindSignUp"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_SignUps_GetDetail, L("ViewSignUp"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_SignUps_Edit, L("EditSignUp"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_SignUps_Create, L("CreateSignUp"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_SignUps_LinkTenant, L("LinkTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_SignUps_UpdateStatus, L("UpdateStatus"), multiTenancySides: MultiTenancySides.Host);
            #endregion


            #region Editions

            var editions = pages.CreateChildPermission(AppPermissions.Pages_Editions, L("Editions"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_Create, L("CreatingNewEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_Edit, L("EditingEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_Delete, L("DeletingEdition"), multiTenancySides: MultiTenancySides.Host);

            #endregion

            #region Administration
            var administration = pages.CreateChildPermission(AppPermissions.Pages_Administration, L("Administration"), multiTenancySides: MultiTenancySides.Host | MultiTenancySides.Tenant);

            //Administration - OrganizationUnits
            var organizationUnits = administration.CreateChildPermission(
                AppPermissions.Pages_Administration_OrganizationUnits,
                L("OrganizationUnits"),
                multiTenancySides: MultiTenancySides.Host);

            organizationUnits.CreateChildPermission(
                AppPermissions.Pages_Administration_OrganizationUnits_ManageOrganizationTree,
                L("ManagingOrganizationTree"),
                multiTenancySides: MultiTenancySides.Host);

            organizationUnits.CreateChildPermission(
                AppPermissions.Pages_Administration_OrganizationUnits_ManageMembers,
                L("ManagingMembers"),
                multiTenancySides: MultiTenancySides.Host);

            //Administration - Roles
            var roles = administration.CreateChildPermission(AppPermissions.Pages_Administration_Roles, L("Roles"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Create, L("CreatingNewRole"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Edit, L("EditingRole"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Delete, L("DeletingRole"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_GetList, L("RoleList"));

            //Administration - User Groups
            var userGroups = administration.CreateChildPermission(AppPermissions.Pages_Administration_UserGroups, L("UserGroups"), multiTenancySides: MultiTenancySides.Tenant);
            userGroups.CreateChildPermission(AppPermissions.Pages_Administration_UserGroups_Create, L("Create"), multiTenancySides: MultiTenancySides.Tenant);
            userGroups.CreateChildPermission(AppPermissions.Pages_Administration_UserGroups_Update, L("Update"), multiTenancySides: MultiTenancySides.Tenant);
            userGroups.CreateChildPermission(AppPermissions.Pages_Administration_UserGroups_Delete, L("Delete"), multiTenancySides: MultiTenancySides.Tenant);
            userGroups.CreateChildPermission(AppPermissions.Pages_Administration_UserGroups_Disable, L("Disable"), multiTenancySides: MultiTenancySides.Tenant);
            userGroups.CreateChildPermission(AppPermissions.Pages_Administration_UserGroups_Enable, L("Enable"), multiTenancySides: MultiTenancySides.Tenant);
            userGroups.CreateChildPermission(AppPermissions.Pages_Administration_UserGroups_GetDetail, L("GetDetail"), multiTenancySides: MultiTenancySides.Tenant);
            userGroups.CreateChildPermission(AppPermissions.Pages_Administration_UserGroups_Find, L("Find"), multiTenancySides: MultiTenancySides.Tenant);
            userGroups.CreateChildPermission(AppPermissions.Pages_Administration_UserGroups_GetList, L("UserGroupList"), multiTenancySides: MultiTenancySides.Tenant);

            //Administration - Users
            var users = administration.CreateChildPermission(AppPermissions.Pages_Administration_Users, L("Users"));

            var commonUsers = users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Common, L("Commons"));

            commonUsers.CreateChildPermission(AppPermissions.Pages_Administration_Users_ChangePermissions, L("ChangingPermissions"));
            commonUsers.CreateChildPermission(AppPermissions.Pages_Administration_Users_Impersonation, L("LoginForUsers"));

            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_GetList, L("UserList"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Create, L("CreatingNewUser"));

            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Edit, L("EditingUser"));
            users.CreateChildPermission(
                AppPermissions.Pages_Administration_Users_Delete,
                L("DeletingUser"));
            users.CreateChildPermission(
               AppPermissions.Pages_Administration_Users_Deactivate,
               L("DeactivateUser"));

            //Administration - Languages
            var languages = administration.CreateChildPermission(
                AppPermissions.Pages_Administration_Languages,
                L("Languages"),
                multiTenancySides: MultiTenancySides.Host);
            languages.CreateChildPermission(
                AppPermissions.Pages_Administration_Languages_Create,
                L("CreatingNewLanguage"),
                multiTenancySides: MultiTenancySides.Host);
            languages.CreateChildPermission(
                AppPermissions.Pages_Administration_Languages_Edit,
                L("EditingLanguage"),
                multiTenancySides: MultiTenancySides.Host);
            languages.CreateChildPermission(
                AppPermissions.Pages_Administration_Languages_Delete,
                L("DeletingLanguages"),
                multiTenancySides: MultiTenancySides.Host);
            languages.CreateChildPermission(
                AppPermissions.Pages_Administration_Languages_ChangeTexts,
                L("ChangingTexts"),
                multiTenancySides: MultiTenancySides.Host);

            //Administration - AuditLogs
            administration.CreateChildPermission(
                AppPermissions.Pages_Administration_AuditLogs,
                L("AuditLogs"), multiTenancySides: MultiTenancySides.Host);

            //Administration - Maintenance
            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Maintenance, L("Maintenance"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);

            //Administration - Subscription
            administration.CreateChildPermission(
                AppPermissions.Pages_Administration_Tenant_SubscriptionManagement,
                L("Subscription"),
                multiTenancySides: MultiTenancySides.Host);

            //Administration - Visual Settings
            administration.CreateChildPermission(
                AppPermissions.Pages_Administration_UiCustomization,
                L("VisualSettings"),
                multiTenancySides: MultiTenancySides.Host);

            //Administration - Settings
            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Settings, L("Settings"), multiTenancySides: MultiTenancySides.Host);

            administration.CreateChildPermission(AppPermissions.Pages_Administration_HangfireDashboard, L("HangfireDashboard"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);



            #endregion



            #region AutoLog
            pages.CreateChildPermission(AppPermissions.Pages_Host_Auto_Log_Transacction, L("AutoLogTransaction"), multiTenancySides: MultiTenancySides.Host);
            pages.CreateChildPermission(AppPermissions.Pages_Host_RefreshToken, L("RefreshToken"), multiTenancySides: MultiTenancySides.Host);

            #endregion

            #region Clients

            var client = pages.CreateChildPermission(AppPermissions.Pages_Host_Client, L("Client"), multiTenancySides: MultiTenancySides.Host);

            #region Account Type
            var accountTypes = client.CreateChildPermission(AppPermissions.Pages_Host_Client_AccountTypes, L("AccountTypes"), multiTenancySides: MultiTenancySides.Host);
            accountTypes.CreateChildPermission(AppPermissions.Pages_Host_Client_AccountTypes_Create, L("CreateAccountType"), multiTenancySides: MultiTenancySides.Host);
            accountTypes.CreateChildPermission(AppPermissions.Pages_Host_Client_AccountTypes_Delete, L("DeleteAccountType"), multiTenancySides: MultiTenancySides.Host);
            accountTypes.CreateChildPermission(AppPermissions.Pages_Host_Client_AccountTypes_Edit, L("UpdateAccountType"), multiTenancySides: MultiTenancySides.Host);
            accountTypes.CreateChildPermission(AppPermissions.Pages_Host_Client_AccountTypes_Enable, L("EnableAccountType"), multiTenancySides: MultiTenancySides.Host);
            accountTypes.CreateChildPermission(AppPermissions.Pages_Host_Client_AccountTypes_Disable, L("DisableAccountType"), multiTenancySides: MultiTenancySides.Host);
            accountTypes.CreateChildPermission(AppPermissions.Pages_Host_Client_AccountTypes_ExportExcel, L("ExportExcel"), multiTenancySides: MultiTenancySides.Host);
            accountTypes.CreateChildPermission(AppPermissions.Pages_Host_Client_AccountTypes_ImportExcel, L("ImportExcel"), multiTenancySides: MultiTenancySides.Host);
            #endregion

            #region Host Referalls
            var referalls = client.CreateChildPermission(AppPermissions.Pages_Host_Referrals, L("Referrals"), multiTenancySides: MultiTenancySides.Host);
            referalls.CreateChildPermission(AppPermissions.Pages_Host_Referrals_GetList, L("GetList"), multiTenancySides: MultiTenancySides.Host);
            referalls.CreateChildPermission(AppPermissions.Pages_Host_Referrals_Delete, L("Delete"), multiTenancySides: MultiTenancySides.Host);
            referalls.CreateChildPermission(AppPermissions.Pages_Host_Referrals_Find, L("Find"), multiTenancySides: MultiTenancySides.Host);
            referalls.CreateChildPermission(AppPermissions.Pages_Host_Referrals_GetDetail, L("View"), multiTenancySides: MultiTenancySides.Host);
            referalls.CreateChildPermission(AppPermissions.Pages_Host_Referrals_Edit, L("Edit"), multiTenancySides: MultiTenancySides.Host);
            referalls.CreateChildPermission(AppPermissions.Pages_Host_Referrals_Create, L("Create"), multiTenancySides: MultiTenancySides.Host);
            referalls.CreateChildPermission(AppPermissions.Pages_Host_Referrals_Enable, L("Enable"), multiTenancySides: MultiTenancySides.Host);
            referalls.CreateChildPermission(AppPermissions.Pages_Host_Referrals_Disable, L("Disable"), multiTenancySides: MultiTenancySides.Host);

            #endregion

            #region Currencies 
            var currency = client.CreateChildPermission(AppPermissions.Pages_Host_Currencies_GetList, L("Currencies"), multiTenancySides: MultiTenancySides.Host);
            currency.CreateChildPermission(AppPermissions.Pages_Host_Client_Currencies_Sync, L("CurrencySync"), multiTenancySides: MultiTenancySides.Host);
            #endregion

            #region Item Types
            var itemtype = client.CreateChildPermission(AppPermissions.Pages_Host_ItemType_GetList, L("ItemType"), multiTenancySides: MultiTenancySides.Host);
            itemtype.CreateChildPermission(AppPermissions.Pages_Host_Client_ItemType_Sync, L("ItemTypeSync"), multiTenancySides: MultiTenancySides.Host);
            #endregion

            #region Payment Method

            var paymentMethodBase = client.CreateChildPermission(AppPermissions.Pages_Host_Client_PaymentMethods, L("PaymentMethod"), multiTenancySides: MultiTenancySides.Host);
            paymentMethodBase.CreateChildPermission(AppPermissions.Pages_Host_Client_PaymentMethods_Create, L("Create"), multiTenancySides: MultiTenancySides.Host);
            paymentMethodBase.CreateChildPermission(AppPermissions.Pages_Host_Client_PaymentMethods_Update, L("Update"), multiTenancySides: MultiTenancySides.Host);
            paymentMethodBase.CreateChildPermission(AppPermissions.Pages_Host_Client_PaymentMethods_Delete, L("Delete"), multiTenancySides: MultiTenancySides.Host);
            paymentMethodBase.CreateChildPermission(AppPermissions.Pages_Host_Client_PaymentMethods_Enable, L("Enable"), multiTenancySides: MultiTenancySides.Host);
            paymentMethodBase.CreateChildPermission(AppPermissions.Pages_Host_Client_PaymentMethods_Disable, L("Disable"), multiTenancySides: MultiTenancySides.Host);
            paymentMethodBase.CreateChildPermission(AppPermissions.Pages_Host_Client_PaymentMethods_GetDetail, L("GetDetail"), multiTenancySides: MultiTenancySides.Host);
            paymentMethodBase.CreateChildPermission(AppPermissions.Pages_Host_Client_PaymentMethods_GetList, L("PaymentMethodList"), multiTenancySides: MultiTenancySides.Host);

            #endregion

            #region Inventory Calculation Schedule
            var calculationSchedule = client.CreateChildPermission(AppPermissions.Pages_Host_Client_CalculationSchedule, L("CalculationSchedule"), multiTenancySides: MultiTenancySides.Host);
            calculationSchedule.CreateChildPermission(AppPermissions.Pages_Host_Client_CalculationSchedule_GetList, L("CalculationScheduleList"), multiTenancySides: MultiTenancySides.Host);
            calculationSchedule.CreateChildPermission(AppPermissions.Pages_Host_Client_CalculationSchedule_GetDetail, L("ViewDetail"), multiTenancySides: MultiTenancySides.Host);
            calculationSchedule.CreateChildPermission(AppPermissions.Pages_Host_Client_CalculationSchedule_Create, L("Create"), multiTenancySides: MultiTenancySides.Host);
            calculationSchedule.CreateChildPermission(AppPermissions.Pages_Host_Client_CalculationSchedule_Edit, L("Edit"), multiTenancySides: MultiTenancySides.Host);
            calculationSchedule.CreateChildPermission(AppPermissions.Pages_Host_Client_CalculationSchedule_Delete, L("Delete"), multiTenancySides: MultiTenancySides.Host);
            calculationSchedule.CreateChildPermission(AppPermissions.Pages_Host_Client_CalculationSchedule_Disable, L("Disable"), multiTenancySides: MultiTenancySides.Host);
            calculationSchedule.CreateChildPermission(AppPermissions.Pages_Host_Client_CalculationSchedule_Enable, L("Enable"), multiTenancySides: MultiTenancySides.Host);
            #endregion

            #region BatchNo Formula
            var batchNoFormula = client.CreateChildPermission(AppPermissions.Pages_Host_Client_BatchNoFormula, L("BatchNoFormula"), multiTenancySides: MultiTenancySides.Host);
            batchNoFormula.CreateChildPermission(AppPermissions.Pages_Host_Client_BatchNoFormula_GetList, L("BatchNoFormulaList"), multiTenancySides: MultiTenancySides.Host);
            batchNoFormula.CreateChildPermission(AppPermissions.Pages_Host_Client_BatchNoFormula_GetDetail, L("ViewDetail"), multiTenancySides: MultiTenancySides.Host);
            batchNoFormula.CreateChildPermission(AppPermissions.Pages_Host_Client_BatchNoFormula_Create, L("Create"), multiTenancySides: MultiTenancySides.Host);
            batchNoFormula.CreateChildPermission(AppPermissions.Pages_Host_Client_BatchNoFormula_Edit, L("Edit"), multiTenancySides: MultiTenancySides.Host);
            batchNoFormula.CreateChildPermission(AppPermissions.Pages_Host_Client_BatchNoFormula_Delete, L("Delete"), multiTenancySides: MultiTenancySides.Host);
            batchNoFormula.CreateChildPermission(AppPermissions.Pages_Host_Client_BatchNoFormula_Disable, L("Disable"), multiTenancySides: MultiTenancySides.Host);
            batchNoFormula.CreateChildPermission(AppPermissions.Pages_Host_Client_BatchNoFormula_Enable, L("Enable"), multiTenancySides: MultiTenancySides.Host);
            #endregion

            #region Package Promotion
            var packagePromotion = client.CreateChildPermission(AppPermissions.Pages_Host_Client_PackagePromotions, L("PackagePromotions"), multiTenancySides: MultiTenancySides.Host);
            packagePromotion.CreateChildPermission(AppPermissions.Pages_Host_Client_PackagePromotions_GetList, L("PackagePromotionList"), multiTenancySides: MultiTenancySides.Host);
            packagePromotion.CreateChildPermission(AppPermissions.Pages_Host_Client_PackagePromotions_GetDetail, L("ViewDetail"), multiTenancySides: MultiTenancySides.Host);
            packagePromotion.CreateChildPermission(AppPermissions.Pages_Host_Client_PackagePromotions_Create, L("Create"), multiTenancySides: MultiTenancySides.Host);
            packagePromotion.CreateChildPermission(AppPermissions.Pages_Host_Client_PackagePromotions_Update, L("Update"), multiTenancySides: MultiTenancySides.Host);
            packagePromotion.CreateChildPermission(AppPermissions.Pages_Host_Client_PackagePromotions_Delete, L("Delete"), multiTenancySides: MultiTenancySides.Host);
            packagePromotion.CreateChildPermission(AppPermissions.Pages_Host_Client_PackagePromotions_Disable, L("Disable"), multiTenancySides: MultiTenancySides.Host);
            packagePromotion.CreateChildPermission(AppPermissions.Pages_Host_Client_PackagePromotions_Enable, L("Enable"), multiTenancySides: MultiTenancySides.Host);
            #endregion

            #region Package
            var package = client.CreateChildPermission(AppPermissions.Pages_Host_Client_Packages, L("Packages"), multiTenancySides: MultiTenancySides.Host);
            package.CreateChildPermission(AppPermissions.Pages_Host_Client_Packages_GetList, L("PackageList"), multiTenancySides: MultiTenancySides.Host);
            package.CreateChildPermission(AppPermissions.Pages_Host_Client_Packages_GetDetail, L("ViewDetail"), multiTenancySides: MultiTenancySides.Host);
            package.CreateChildPermission(AppPermissions.Pages_Host_Client_Packages_Create, L("Create"), multiTenancySides: MultiTenancySides.Host);
            package.CreateChildPermission(AppPermissions.Pages_Host_Client_Packages_Update, L("Update"), multiTenancySides: MultiTenancySides.Host);
            package.CreateChildPermission(AppPermissions.Pages_Host_Client_Packages_Delete, L("Delete"), multiTenancySides: MultiTenancySides.Host);
            package.CreateChildPermission(AppPermissions.Pages_Host_Client_Packages_Disable, L("Disable"), multiTenancySides: MultiTenancySides.Host);
            package.CreateChildPermission(AppPermissions.Pages_Host_Client_Packages_Enable, L("Enable"), multiTenancySides: MultiTenancySides.Host);
            #endregion

            #region Format Setting

            var format = client.CreateChildPermission(AppPermissions.Pages_Host_Formats_GetList, L("FormatSetting"), multiTenancySides: MultiTenancySides.Host);
            format.CreateChildPermission(AppPermissions.Pages_Host_Client_Formats_Sync, L("FormatSync"), multiTenancySides: MultiTenancySides.Host);

            #endregion

            #endregion

            #region DemoUiComponents
            pages.CreateChildPermission(AppPermissions.Pages_DemoUiComponents, L("DemoUiComponents"), multiTenancySides: MultiTenancySides.Host);
            #endregion


            var developer = administration.CreateChildPermission(AppPermissions.Pages_Administration_Developer, L("Developer"), multiTenancySides: MultiTenancySides.Host);
            var apiClient = developer.CreateChildPermission(AppPermissions.Pages_Administration_Developer_ApiClients, L("ApiClient"), multiTenancySides: MultiTenancySides.Host);
            apiClient.CreateChildPermission(AppPermissions.Pages_Administration_Developer_ApiClients_Create, L("Create"), multiTenancySides: MultiTenancySides.Host);
            apiClient.CreateChildPermission(AppPermissions.Pages_Administration_Developer_ApiClients_Delete, L("Delete"), multiTenancySides: MultiTenancySides.Host);
            apiClient.CreateChildPermission(AppPermissions.Pages_Administration_Developer_ApiClients_Disable, L("Disable"), multiTenancySides: MultiTenancySides.Host);
            apiClient.CreateChildPermission(AppPermissions.Pages_Administration_Developer_ApiClients_Enable, L("Enable"), multiTenancySides: MultiTenancySides.Host);
            apiClient.CreateChildPermission(AppPermissions.Pages_Administration_Developer_ApiClients_Update, L("Update"), multiTenancySides: MultiTenancySides.Host);
            apiClient.CreateChildPermission(AppPermissions.Pages_Administration_Developer_ApiClients_Detail, L("View"), multiTenancySides: MultiTenancySides.Host);

        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, CorarlERPConsts.LocalizationSourceName);
        }
    }
}
