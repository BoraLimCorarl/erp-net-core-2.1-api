using CorarlERP.Authorization.Roles;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Authorization
{
    public class AppStaticRolePermissions
    {
        public static IDictionary<string, List<string>> StaticPermissions
        {
            get
            {

                var dic = new Dictionary<string, List<string>>();

                dic.Add(StaticRoleNames.Tenants.AccountingManager, new List<string>(){

                       AppPermissions.Pages,
                       AppPermissions.Pages_Tenant_CleanRounding,
                        AppPermissions.Pages_Tenant_BOMs_Find,
                        AppPermissions.Pages_Tenant_ChangeJournalDate,

                        AppPermissions.Pages_Tenant_Commons,

                        AppPermissions.Pages_Tenant_Common_Inventory_JournalTransactionTypes_Find,

                        AppPermissions.Pages_Tenant_Accounting_Locks_Find,

                        AppPermissions.Pages_Tenant_UserActivities_FindActivity,

                        AppPermissions.Pages_Tenant_UserActivities_FindTrasaction,

                        AppPermissions.Pages_Host_Client_PackagePromotions_Find,

                        AppPermissions.Pages_Host_Client_Packages_Find,

                        AppPermissions.Pages_Tenant_Production_ProductionOrder_Find,

                        AppPermissions.Pages_Tenant_Production_Process_Find,

                        AppPermissions.Pages_Tenant_Setup_Commons_FindTaxes,

                        AppPermissions.Pages_Tenant_Setup_Commons_FindAccountTypes,

                        AppPermissions.Pages_Tenant_Customer_Find,

                        AppPermissions.Pages_Tenant_Setup_Locations_Find,

                        AppPermissions.Pages_Tenant_Setup_Lots_Find,

                        AppPermissions.Pages_Tenant_Setup_Classes_Find,

                        AppPermissions.Pages_Tenant_Setup_PaymentMethods_Find,

                        AppPermissions.Pages_Tenant_Setup_CustomerTypes_Find,

                        AppPermissions.Pages_Tenant_Setup_VendorTypes_Find,

                        AppPermissions.Pages_Tenant_Setup_TransactionTypes_Find,

                        AppPermissions.Pages_Tenant_Setup_Commons_FindAccounts,

                        AppPermissions.Pages_Tenant_ItemType_Find,

                        AppPermissions.Pages_Tenant_Currencies_Find,

                        AppPermissions.Pages_Tenant_MultiCurrencies_Find,

                        AppPermissions.Pages_Tenant_MultiCurrencies_GetList,

                        AppPermissions.Pages_Tenant_Formats_FindNumber,

                        AppPermissions.Pages_Tenant_Formats_FindDate,

                        AppPermissions.Pages_Tenant_Properties_Find,

                        AppPermissions.Pages_Tenant_Item_Find,

                       AppPermissions.Pages_Tenant_ItemCodeFormulas_Find,
                       AppPermissions.Pages_Host_Client_BatchNoFormula_Find,
                        AppPermissions.Pages_Tenant_Common_BatchNo_Find,


                       AppPermissions.Pages_Tenant_Production_Plan_Find,

                     AppPermissions.Pages_Tenant_Production_Line_Find,

                        AppPermissions.Pages_Tenant_ItemPrices_Find,

                        AppPermissions.Pages_Tenant_Exchanges_Find,

                        AppPermissions.Pages_Tenant_PropertyValue_FindValue,

                        AppPermissions.Pages_Tenant_Vendor_Find,

                        AppPermissions.Pages_Tenant_Customer_SaleOrder_Find,

                        AppPermissions.Pages_Tenant_Customers_ReceivePayments_Find,

                        AppPermissions.Pages_Tenant_Customers_Credit_Find,



                        AppPermissions.Pages_Tenant_PurchaseOrder_Find,

                        AppPermissions.Pages_Tenant_Customer_Invoice_Find,

                        AppPermissions.Pages_Tenant_Common_Inventory,

                        AppPermissions.Pages_Tenant_Common_DeleteVoid,



                        AppPermissions.Pages_Tenant_Common_Stock_Balance,

                        AppPermissions.Pages_Tenant_Customers_ItemIssues_Find,


                        AppPermissions.Pages_Tenant_Vendors_Bills_Find,

                        AppPermissions.Pages_Tenant_Vendors_Credit_Find,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_Find,

                        AppPermissions.Pages_Tenant_Accounting_Journals_Find,

                        AppPermissions.Pages_Tenant_Vendors_PayBills_Find,

                        AppPermissions.Pages_Tenant_Banks_Transfers_Find,


                        AppPermissions.Pages_Tenant_Inventory_PhysicalCount_Find,

                        AppPermissions.Pages_Tenant_Inventory_TransferOrder_Find,

                        AppPermissions.Pages_Tenant_Production_ProductionOrder_GetListProductionOrderForItemIssue,

                        AppPermissions.Pages_Tenant_Production_ProductionOrder_GetListProductionOrderForItemReceipt,

                        AppPermissions.Pages_Tenant_Vendors_Bills_GetBills,

                        AppPermissions.Pages_Tenant_Vendors_Bills_GetBillItems,

                        AppPermissions.Pages_Tenant_Exchanges_GetExChangeCurrency,

                        AppPermissions.Pages_Tenant_PurchaseOrder_GetTotalPurchaseOrderForItemReceipts,

                        AppPermissions.Pages_Tenant_PurchaseOrder_GetlistPuchaseOrderForItemReceipts,

                        AppPermissions.Pages_Tenant_PurchaseOrder_GetTotalPurchaseOrderForBills,

                        AppPermissions.Pages_Tenant_PurchaseOrder_GetlistPuchaseOrderForBills,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceiptItems,

                        AppPermissions.Pages_Tenant_Customer_SaleOrder_GetItemSaleOrderForInvoices,

                        AppPermissions.Pages_Tenant_Customer_SaleOrder_GetTotalSaleOrderForInvoice,



                        AppPermissions.Pages_Tenant_Vendors_Bills_GetListForPayBill,

                        AppPermissions.Pages_Tenant_Customer_Invoice_GetListInvoiceForReceivePayment,


                        AppPermissions.Pages_Tenant_Inventory_TransferOrder_ForItemIssue,

                        AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssues,

                        AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssueItems,



                        AppPermissions.Pages_Tenant_Customer_SaleOrder_GetItemSaleOrderForItemIssues,

                        AppPermissions.Pages_Tenant_Customer_SaleOrder_GetTotalSaleOrder,

                        AppPermissions.Pages_Tenant_Customer_Invoice_GetInvoiceItems,

                        AppPermissions.Pages_Tenant_Customer_Invoice_GetInvoiceSummaryForItemIssue,

                        AppPermissions.Pages_Tenant_Common_Tenant_Find,

                        AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Find,


                        AppPermissions.Pages_Tenant_Dashboard,
                          AppPermissions.Pages_Tenant_App,


                        AppPermissions.Pages_Tenant_Customer_POS,

                        AppPermissions.Pages_Tenant_Customer_POS_CanSeePrice,

                        AppPermissions.Pages_Tenant_Customer_POS_GetList,

                        AppPermissions.Pages_Tenant_Customer_POS_Create,

                        AppPermissions.Pages_Tenant_Customer_POS_GetDetail,



                        AppPermissions.Pages_Tenant_Customer_POS_ViewAll,

                        AppPermissions.Pages_Tenant_Customer_POS_Print,

                        AppPermissions.Pages_Tenant_Customer_POS_UpdateToVoid,
                     AppPermissions.Pages_Tenant_Customers_Invoice_SaleReturn,


                        AppPermissions.Pages_Tenant_Customers_Invoice_ListSaleReturn,

                        AppPermissions.Pages_Tenant_Customers_Invoice_CreateSaleReturn,

                        AppPermissions.Pages_Tenant_Customers_Invoice_ViewSaleReturn,

                        AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssues_SaleReturn,



                        AppPermissions.Pages_Tenant_Customers_Invoice_UpdateSaleReturn,

                        AppPermissions.Pages_Tenant_Customers_Invoice_VoidSaleReturn,



                        AppPermissions.Pages_Tenant_Customers_Invoice_DeleteVoideSaleReturn,

                        AppPermissions.Pages_Tenant_Customer_Credit_POS_Find,

                        AppPermissions.Pages_Tenant_Customers_Invoice_ListSaleReturn_Unpaid,


                        AppPermissions.Pages_Tenant_Set_Customer_POS,

                        AppPermissions.Pages_Tenant_Customer_GetListPOS,

                        AppPermissions.Pages_Tenant_Customer_CreatePOS,

                        AppPermissions.Pages_Tenant_Customer_GetDetailPOS,

                        AppPermissions.Pages_Tenant_Customer_UpdatePOS,

                        AppPermissions.Pages_Tenant_Customer_DeletePOS,



                        AppPermissions.Pages_Tenant_Customer_DisablePOS,


                        AppPermissions.Pages_Tenant_Customer_EnablePOS,



                        AppPermissions.Pages_Tenant_Vendors,



                       AppPermissions.Pages_Tenant_Vendor_GetList,

                         AppPermissions.Pages_Tenant_Vendor_Create,



                        AppPermissions.Pages_Tenant_Vendor_GetDetail,



                        AppPermissions.Pages_Tenant_Vendor_Update,

                        AppPermissions.Pages_Tenant_Vendor_Delete,

                        AppPermissions.Pages_Tenant_Vendor_Disable,



                        AppPermissions.Pages_Tenant_Vendor_Enable,



                        AppPermissions.Pages_Tenant_Vendor_ExportExcel,



                        AppPermissions.Pages_Tenant_Vendor_ImportExcel,

                        AppPermissions.Pages_Tenant_PurchaseOrders,



                        AppPermissions.Pages_Tenant_PurchaseOrder_Common,

                        AppPermissions.Pages_Tenant_PurchaseOrder_CanSeePrice,

                        AppPermissions.Pages_Tenant_PurchaseOrder_GetList,

                         AppPermissions.Pages_Tenant_PurchaseOrder_Create,

                        AppPermissions.Pages_Tenant_PurchaseOrder_GetDetail,

                        AppPermissions.Pages_Tenant_PurchaseOrder_Update,

                        AppPermissions.Pages_Tenant_PurchaseOrder_Delete,

                        AppPermissions.Pages_Tenant_PurchaseOrder_UpdateStatusToPublish,

                        AppPermissions.Pages_Tenant_PurchaseOrder_UpdateStatusToClose,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_common,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CanSeePrice,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CanEditAccount,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CanViewAccount,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetList,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_Create,



                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CreatePurchases,



                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CanPO,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CanBill,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CreateTransfers,

                        AppPermissions.Pages_Tenant_Inventory_TransferOrder_FindItemReceiptTrasfer,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CreateCustomerCredits,

                        AppPermissions.Pages_Tenant_Customers_Credit_GetCustomerCreditHeader,


                        AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssues_ForItemReceiptCustomerCrdit,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CreateProductionOrder,

                        AppPermissions.Pages_Tenant_Production_ProductionOrder_GetProductionOrderForItemReceipt,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CreateAdjustments,


                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CreateOthers,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetDetail,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_Update,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_Delete,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_importExcel,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_importExcelItemReceiptOther,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_importExcelItemReceiptAdjustment,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_exportExcel,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_exportExcelItemReceiptOther,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_exportExcelItemReceiptAdjustment,

                        AppPermissions.Pages_Tenant_Vendors_Bills,

                        AppPermissions.Pages_Tenant_Vendors_Bills_Common,

                        AppPermissions.Pages_Tenant_Vendors_Bills_EditQty,

                        AppPermissions.Pages_Tenant_Vendors_Bills_CanSeePrice,

                        AppPermissions.Pages_Tenant_Vendors_Bills_CanEditAccount,

                        AppPermissions.Pages_Tenant_Vendors_Bills_CanViewAccount,

                        AppPermissions.Pages_Tenant_Vendors_Bills_Setting,

                        AppPermissions.Pages_Tenant_Vendors_Bills_GetList,

                        AppPermissions.Pages_Tenant_Vendors_Bills_Create,

                        AppPermissions.Pages_Tenant_Vendors_Bills_CanPO,

                        AppPermissions.Pages_Tenant_Vendors_Bills_CanItemReceipt,

                        AppPermissions.Pages_Tenant_Vendors_Bills_ChooseAccounts,

                        AppPermissions.Pages_Tenant_Vendors_Bills_GetDetail,

                        AppPermissions.Pages_Tenant_Vendors_PayBills_ViewBillHistory,

                       AppPermissions.Pages_Tenant_Vendors_PayBills_ViewVendorCreditHistory,

                        AppPermissions.Pages_Tenant_Vendors_Bills_Update,



                        AppPermissions.Pages_Tenant_Vendors_Bills_Delete,

                       AppPermissions.Pages_Tenant_Vendors_Bills_MultiDelete,


                    AppPermissions.Pages_Tenant_Vendors_Bills_ImportExcel,
                     AppPermissions.Pages_Tenant_Vendors_Bills_ImportExcelBill,
                     AppPermissions.Pages_Tenant_Vendors_Credit_Import,
                     AppPermissions.Pages_Tenant_Vendors_Bills_ExportExcel,
                      AppPermissions.Pages_Tenant_Vendors_Bills_ExportExcelBillTamplate,
                      AppPermissions.Pages_Tenant_Vendors_Credit_ExportVendorCreditTamplate,




                        AppPermissions.Pages_Tenant_Vendors_Bills_CanCreateVendorCredit,

                        AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssues_ForVendorCredit,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts_ItemIssueVendorCredit,

                       AppPermissions.Pages_Tenant_Vendors_Bills_VendorCreditChooseAccounts,

                        AppPermissions.Pages_Tenant_Vendors_PayBills,


                        AppPermissions.Pages_Tenant_Vendors_PayBills_Common,

                        AppPermissions.Pages_Tenant_Vendors_PayBills_CanEditAccount,

                        AppPermissions.Pages_Tenant_Vendors_PayBills_CanViewAccount,

                        AppPermissions.Pages_Tenant_Vendors_PayBills_CanPayByCredit,

                       AppPermissions.Pages_Tenant_Vendors_PayBills_GetList,



                        AppPermissions.Pages_Tenant_Vendors_PayBills_Create,

                        AppPermissions.Pages_Tenant_Vendors_PayBills_GetDetail,

                        AppPermissions.Pages_Tenant_Vendors_PayBills_Update,

                        AppPermissions.Pages_Tenant_Vendors_PayBills_Delete,

                      AppPermissions.Pages_Tenant_Vendors_PayBills_MultiDelete,



                        AppPermissions.Pages_Tenant_Customers,

                        AppPermissions.Pages_Tenant_Customer_GetList,

                        AppPermissions.Pages_Tenant_Customer_Create,

                        AppPermissions.Pages_Tenant_Customer_GetDetail,

                        AppPermissions.Pages_Tenant_Customer_Update,

                        AppPermissions.Pages_Tenant_Customer_Delete,

                        AppPermissions.Pages_Tenant_Customer_Disable,

                        AppPermissions.Pages_Tenant_Customer_Enable,



                        AppPermissions.Pages_Tenant_Customer_ExportExcel,



                        AppPermissions.Pages_Tenant_Customer_ImportExcel,


                        AppPermissions.Pages_Tenant_Customer_SaleOrder,

                        AppPermissions.Pages_Tenant_Customer_SaleOrder_Common,



                        AppPermissions.Pages_Tenant_Customer_SaleOrder_CanSeePrice,

                        AppPermissions.Pages_Tenant_Customer_SaleOrder_GetList,

                        AppPermissions.Pages_Tenant_Customer_SaleOrder_Create,

                        AppPermissions.Pages_Tenant_Customer_SaleOrder_GetDetail,

                        AppPermissions.Pages_Tenant_Customer_SaleOrder_Update,



                        AppPermissions.Pages_Tenant_Customer_SaleOrder_Delete,

                        AppPermissions.Pages_Tenant_Customer_SaleOrder_UpdateToClose,

                        AppPermissions.Pages_Tenant_Customer_SaleOrder_UpdateInventoryStatus,

                        AppPermissions.Pages_Tenant_Customer_SaleOrder_UpdateToPublish,

                        AppPermissions.Pages_Tenant_Customer_SaleOrder_UpdateToVoid,

                        AppPermissions.Pages_Tenant_Customer_Invoices,



                        AppPermissions.Pages_Tenant_Customer_Invoices_Common,



                        AppPermissions.Pages_Tenant_Customer_Invoice_EditQty,

                        AppPermissions.Pages_Tenant_Customer_Invoice_CanEditAccount,

                        AppPermissions.Pages_Tenant_Customer_Invoice_CanViewAccount,

                        AppPermissions.Pages_Tenant_Customer_Invoice_CanSeePrice,

                        AppPermissions.Pages_Tenant_Customer_Invoice_Setting,

                        AppPermissions.Pages_Tenant_Customer_Invoice_GetList,

                       AppPermissions.Pages_Tenant_Customer_Invoice_MultiDelete,

                        AppPermissions.Pages_Tenant_Customer_Invoice_Create,

                        AppPermissions.Pages_Tenant_Customer_Invoice_CanItemIssue,

                        AppPermissions.Pages_Tenant_Customer_Invoice_CanSaleOrder,

                        AppPermissions.Pages_Tenant_Customer_Invoice_ChooseAccounts,

                        AppPermissions.Pages_Tenant_Customer_Invoice_GetDetail,

                        AppPermissions.Pages_Tenant_Customer_Invoice_Update,

                        AppPermissions.Pages_Tenant_Customer_Invoice_Delete,

                        AppPermissions.Pages_Tenant_Customer_Invoice_ImportExcel,


                        AppPermissions.Pages_Tenant_Customer_Invoice_ImportExcelInvoice,
                      AppPermissions.Pages_Tenant_Customers_Credit_Import,

                        AppPermissions.Pages_Tenant_Customer_Invoice_ExportExcel,

                        AppPermissions.Pages_Tenant_Customer_Invoice_ExportExcelTamplate,
                       AppPermissions.Pages_Tenant_Customers_Credit_ExportCustomerCreditTamplate,

                        AppPermissions.Pages_Tenant_Customers_Invoice_CreateCustomerCredit,

                        AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssues_ForCustomerCredit,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts_CustomerCredit,

                       AppPermissions.Pages_Tenant_Customer_Invoice_CustomerCreditChooseAccounts,

                        AppPermissions.Pages_Tenant_Customers_ItemIssues,

                        AppPermissions.Pages_Tenant_Customers_ItemIssues_Common,

                        AppPermissions.Pages_Tenant_Customers_ItemIssues_CanEditAccount,

                        AppPermissions.Pages_Tenant_Customers_ItemIssues_CanViewAccount,

                        AppPermissions.Pages_Tenant_Customers_ItemIssues_CanSeePrice,

                        AppPermissions.Pages_Tenant_Customers_ItemIssues_GetList,


                    AppPermissions.Pages_Tenant_Customers_ItemIssues_Create,

                        AppPermissions.Pages_Tenant_Customers_ItemIssues_CanSaleOrder,

                        AppPermissions.Pages_Tenant_Customers_ItemIssues_CanInvoice,
                         AppPermissions.Pages_Tenant_Customers_ItemIssues_CanDelivery,

                        AppPermissions.Pages_Tenant_Customers_ItemIssues_ShowConvertInvoice,

                        AppPermissions.Pages_Tenant_Customers_ItemIssues_EditConvertInvoice,

                        AppPermissions.Pages_Tenant_Customers_ItemIssues_CanCreateVendorCredit,

                        AppPermissions.Pages_Tenant_Vendors_Bills_GetListForItemReceipt,

                        AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts_VendorCredit,

                        AppPermissions.Pages_Tenant_Customers_ItemIssues_CanCreateTransfer,

                        AppPermissions.Pages_Tenant_Inventory_TransferOrder_ForItemIssueTransfer,

                        AppPermissions.Pages_Tenant_Customers_ItemIssues_CanCreateProductionOrder,

                        AppPermissions.Pages_Tenant_Production_ProductionOrder_GetProductionOrderForItemIssue,

                        AppPermissions.Pages_Tenant_Customers_ItemIssues_CanCreateAdjustment,


                        AppPermissions.Pages_Tenant_Customers_ItemIssues_CanCreateOther,

                        AppPermissions.Pages_Tenant_Customers_ItemIssues_GetDetail,

                        AppPermissions.Pages_Tenant_Customers_ItemIssues_Update,

                        AppPermissions.Pages_Tenant_Customers_ItemIssues_Delete,

                        AppPermissions.Pages_Tenant_Customers_ItemIssues_ImportExcelItemIssueAdjustment,

                        AppPermissions.Pages_Tenant_Customers_ItemIssues_ExportExcelTemplateItemIssueAdjustment,

                        AppPermissions.Pages_Tenant_Customers_ReceivePayments,

                        AppPermissions.Pages_Tenant_Customers_ReceivePayments_Common,

                        AppPermissions.Pages_Tenant_Customers_ReceivePayments_CanEditAccount,

                        AppPermissions.Pages_Tenant_Customers_ReceivePayments_CanViewAccount,

                        AppPermissions.Pages_Tenant_Customers_ReceivePayments_GetList,

                        AppPermissions.Pages_Tenant_Customers_ReceivePayments_Create,

                        AppPermissions.Pages_Tenant_Customers_ReceivePayments_GetDetail,

                        AppPermissions.Pages_Tenant_Customers_ReceivePayments_ViewInvoiceHistory,

                        AppPermissions.Pages_Tenant_Customers_ReceivePayments_ViewCustomerCreditHistory,

                        AppPermissions.Pages_Tenant_Customers_ReceivePayments_Update,

                        AppPermissions.Pages_Tenant_Customers_ReceivePayments_Delete,

                       AppPermissions.Pages_Tenant_Customers_ReceivePayments_MultiDelete,

                     AppPermissions.Pages_Tenant_Customers_ReceivePayments_Export_Excel,



                        AppPermissions.Pages_Tenant_Production,


                        AppPermissions.Pages_Tenant_Production_ProductionOrder,

                        AppPermissions.Pages_Tenant_Production_ProductionOrder_Common,

                        AppPermissions.Pages_Tenant_Production_ProductionOrder_CanSeePrice,

                        AppPermissions.Pages_Tenant_Production_ProductionOrder_CanEditAcccount,

                        AppPermissions.Pages_Tenant_Production_ProductionOrder_CanViewAcccount,

                        AppPermissions.Pages_Tenant_Production_ProductionOrder_CanViewCalculationState,




                        AppPermissions.Pages_Tenant_Production_ProductionOrder_AutoInventory,

                       AppPermissions.Pages_Tenant_Production_ProductionOrder_GetList,

                        AppPermissions.Pages_Tenant_Production_ProductionOrder_Create,

                        AppPermissions.Pages_Tenant_Production_ProductionOrder_GetDetail,

                        AppPermissions.Pages_Tenant_Production_ProductionOrder_Update,

                        AppPermissions.Pages_Tenant_Production_ProductionOrder_Delete,

                      AppPermissions.Pages_Tenant_Production_ProductionOrder_Calculate,

                        AppPermissions.Pages_Tenant_Production_Process,

                       AppPermissions.Pages_Tenant_Production_Process_GetList,

                        AppPermissions.Pages_Tenant_Production_Process_Create,

                        AppPermissions.Pages_Tenant_Production_Process_GetDetail,

                        AppPermissions.Pages_Tenant_Production_Process_Update,

                        AppPermissions.Pages_Tenant_Production_Process_Delete,

                        AppPermissions.Pages_Tenant_Production_Process_UpdateStatusToDisable,


                        AppPermissions.Pages_Tenant_Production_Process_UpdateStatusToEnable,

                        AppPermissions.Pages_Tenant_Production_Plan,

                       AppPermissions.Pages_Tenant_Production_Plan_GetList,

                        AppPermissions.Pages_Tenant_Production_Plan_Create,

                        AppPermissions.Pages_Tenant_Production_Plan_GetDetail,

                        AppPermissions.Pages_Tenant_Production_Plan_Update,

                        AppPermissions.Pages_Tenant_Production_Plan_Delete,

                         AppPermissions.Pages_Tenant_Production_Plan_Close,

                       AppPermissions.Pages_Tenant_Production_Plan_Open,


                      AppPermissions.Pages_Tenant_Production_Plan_Calculate,

                        AppPermissions.Pages_Tenant_Production_Line,

                       AppPermissions.Pages_Tenant_Production_Line_GetList,

                        AppPermissions.Pages_Tenant_Production_Line_Create,

                        AppPermissions.Pages_Tenant_Production_Line_GetDetail,

                        AppPermissions.Pages_Tenant_Production_Line_Update,

                        AppPermissions.Pages_Tenant_Production_Line_Delete,

                         AppPermissions.Pages_Tenant_Production_Line_Enable,

                       AppPermissions.Pages_Tenant_Production_Line_Disable,

                          AppPermissions.Pages_Tenant_Inventorys,
                          AppPermissions.Pages_Tenant_Inventory_JournalTransactionTypes,
                         AppPermissions.Pages_Tenant_Inventory_JournalTransactionTypes_Create,

                         AppPermissions.Pages_Tenant_Inventory_JournalTransactionTypes_Update,

                         AppPermissions.Pages_Tenant_Inventory_JournalTransactionTypes_GetList,

                       AppPermissions.Pages_Tenant_Inventory_JournalTransactionTypes_Enable,

                       AppPermissions.Pages_Tenant_Inventory_JournalTransactionTypes_Disable,

                       AppPermissions.Pages_Tenant_Inventory_JournalTransactionTypes_GetDetail,

                       AppPermissions.Pages_Tenant_Inventory_JournalTransactionTypes_Delete,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Common,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CanSeePrice,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CanEditAccount,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CanViewAccount,



                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_GetList,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Create,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptPurchase,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptPurchase_CanBill,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptPurchase_CanPurchaseOrder,


                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptCustomerCredit,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptCustomerCredit_CanCustomerCredit,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptCustomerCredit_CanIssueSale,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptTransfer,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptTransfer_CanTransferOrder,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptProduction,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptPurchase_CanProductionOrder,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptAdjustment,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptOther,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueSale,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ItemIssuesSale_ShowConvertInvoice,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ItemIssuesSale_CanSaleOrder,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ItemIssuesSale_CanInvoice,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ItemIssuesSale_EditConvertInvoice,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueVendorCredit,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueVendorCredit_CanVendorCredit,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueVendorCredit_CanItemReceiptPurchase,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueTransfer,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueTransfer_CanTransferOrder,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueProduction,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueProduction_CanProductionOrder,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueAdjustment,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueOther,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ViewDetail,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Update,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Delete,
                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ImportExcel,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ImportItemReceiptOther,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ImportItemReceiptAdjustment,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ImportItemIssueAdjustment,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ExportExcel,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ExportItemReceiptOther,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ExportItemReceiptAdjustment,

                        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ExportItemIssueAdjustment,

                        AppPermissions.Pages_Tenant_Inventory_TransferOrder,

                        AppPermissions.Pages_Tenant_Inventory_TransferOrder_Common,



                        AppPermissions.pages_Tenant_Inventory_TransferOrder_AutoInventory,

                        AppPermissions.Pages_Tenant_Inventory_TransferOrder_GetList,

                        AppPermissions.Pages_Tenant_Inventory_TransferOrder_Create,

                        AppPermissions.Pages_Tenant_Inventory_TransferOrder_GetDetail,

                        AppPermissions.Pages_Tenant_Inventory_TransferOrder_Update,

                        AppPermissions.Pages_Tenant_Inventory_TransferOrder_Delete,

                        AppPermissions.Pages_Tenant_Inventory_PhysicalCount,

                        AppPermissions.Pages_Tenant_Inventory_PhysicalCount_GetList,

                        AppPermissions.Pages_Tenant_Inventory_PhysicalCount_Create,

                        AppPermissions.Pages_Tenant_Inventory_PhysicalCount_GetDetail,

                        AppPermissions.Pages_Tenant_Inventory_PhysicalCount_Update,

                        AppPermissions.Pages_Tenant_Inventory_PhysicalCount_Delete,



                        AppPermissions.Pages_Tenant_Banks,

                         AppPermissions.Pages_Tenant_Banks_BankTransactions,

                        AppPermissions.Pages_Tenant_Banks_BankTransactions_GetList,

                          AppPermissions.Pages_Tenant_Banks_Transactions_Create,

                        AppPermissions.Pages_Tenant_Banks_Deposits_Create,

                        AppPermissions.Pages_Tenant_Banks_Withdraws_Create,

                        AppPermissions.Pages_Tenant_Banks_BankTransactions_Receivepayment_Create,

                        AppPermissions.Pages_Tenant_Banks_BankTransactions_PayBill_Create,

                        AppPermissions.Pages_Tenant_Banks_BankTransactions_View,

                        AppPermissions.Pages_Tenant_Banks_Deposits_GetDetail,

                        AppPermissions.Pages_Tenant_Banks_Withdraws_GetDetail,

                        AppPermissions.Pages_Tenant_Banks_BankTransactions_ReceivePayment_View,

                        AppPermissions.Pages_Tenant_Banks_BankTransactions_PayBill_View,

                        AppPermissions.Pages_Tenant_Banks_Transactions_Update,

                        AppPermissions.Pages_Tenant_Banks_Deposits_Update,

                        AppPermissions.Pages_Tenant_Banks_Withdraws_Update,

                       AppPermissions.Pages_Tenant_Banks_BankTransactions_Receivepayment_Update,

                        AppPermissions.Pages_Tenant_Banks_BankTransactions_PayBill_Update,

                        AppPermissions.Pages_Tenant_Banks_BankTransactions_Delete,

                        AppPermissions.Pages_Tenant_Banks_Deposits_Delete,

                        AppPermissions.Pages_Tenant_Banks_Withdraws_Delete,

                        AppPermissions.Pages_Tenant_Banks_BankTransactions_ReceivePayment_Delete,

                        AppPermissions.Pages_Tenant_Banks_BankTransactions_PayBill_Delete,

                        AppPermissions.Pages_Tenant_Banks_Transfers,

                        AppPermissions.Pages_Tenant_Banks_Transfers_GetList,

                        AppPermissions.Pages_Tenant_Banks_Transfers_Create,

                       AppPermissions.Pages_Tenant_Banks_Transfers_GetDetail,

                        AppPermissions.Pages_Tenant_Banks_Transfers_Update,

                        AppPermissions.Pages_Tenant_Banks_Transfers_Delete,

                        AppPermissions.Pages_Tenant_Setup,

                        AppPermissions.Pages_Tenant_Setup_ChartOfAccounts,

                        AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_Create,

                        AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_Edit,

                        AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_Delete,

                        AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_Enable,

                        AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_Disable,

                        AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_ImportExcel,

                        AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_ExportExcel,

                        AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_ExportPdf,

                        AppPermissions.Pages_Tenant_Setup_Taxes,

                        AppPermissions.Pages_Tenant_Setup_Taxes_Create,

                        AppPermissions.Pages_Tenant_Setup_Taxes_Delete,

                        AppPermissions.Pages_Tenant_Setup_Taxes_Edit,

                        AppPermissions.Pages_Tenant_Setup_Taxes_Enable,

                        AppPermissions.Pages_Tenant_Setup_Taxes_Disable,

                         AppPermissions.Pages_Tenant_ItemCodeFormulas,

                        AppPermissions.Pages_Tenant_ItemCodeFormulas_GetList,

                        AppPermissions.Pages_Tenant_ItemCodeFormulas_GetDetail,

                        AppPermissions.Pages_Tenant_ItemCodeFormulas_Create,

                        AppPermissions.Pages_Tenant_ItemCodeFormulas_Update,

                        AppPermissions.Pages_Tenant_ItemCodeFormulas_Delete,

                        AppPermissions.Pages_Tenant_ItemCodeFormulas_Disable,

                       AppPermissions.Pages_Tenant_ItemCodeFormulas_UpdateAutoItemCode,

                       AppPermissions.Pages_Tenant_ItemCodeFormulas_UpdateItemCodeSetting,

                        AppPermissions.Pages_Tenant_ItemCodeFormulas_Enable,

                        AppPermissions.Pages_Tenant_Items,

                        AppPermissions.Pages_Tenant_Items_Common,

                        AppPermissions.Pages_Tenant_SubItem_Delete,

                        AppPermissions.Pages_Tenant_Item_CanFilterAccount,

                        AppPermissions.Pages_Tenant_Item_CanSeePrice,

                        AppPermissions.Pages_Tenant_Item_GetList,

                        AppPermissions.Pages_Tenant_Item_GetDetail,

                        AppPermissions.Pages_Tenant_Item_Create,

                        AppPermissions.Pages_Tenant_Item_Update,

                        AppPermissions.Pages_Tenant_Item_Delete,

                        AppPermissions.Pages_Tenant_Item_Disable,

                        AppPermissions.Pages_Tenant_Item_Enable,

                        AppPermissions.Pages_Tenant_Item_Export,

                        AppPermissions.Pages_Tenant_Item_Import,

                       AppPermissions.Pages_Tenant_Item_Print_Barcode,

                        AppPermissions.Pages_Tenant_Item_ExportPdf,

                        AppPermissions.Pages_Tenant_Properties,

                         AppPermissions.Pages_Tenant_Properties_Property_Value,

                        AppPermissions.Pages_Tenant_PropertyValue_Delete,

                        AppPermissions.Pages_Tenant_PropertyValue_AddValue,

                        AppPermissions.Pages_Tenant_PropertyValue_EditValue,

                        AppPermissions.Pages_Tenant_PropertyValue_GetValue,

                        AppPermissions.Pages_Tenant_Properties_GetList,

                        AppPermissions.Pages_Tenant_Properties_Create,

                        AppPermissions.Pages_Tenant_Properties_Update,

                        AppPermissions.Pages_Tenant_Properties_Delete,

                        AppPermissions.Pages_Tenant_Properties_Disable,

                        AppPermissions.Pages_Tenant_Properties_Enable,

                        AppPermissions.Pages_Tenant_Properties_Export_Excel,

                        AppPermissions.Pages_Tenant_Properties_Import_Excel,

                        AppPermissions.Pages_Tenant_ItemPrices,

                        AppPermissions.Pages_Tenant_ItemPrices_GetList,

                        AppPermissions.Pages_Tenant_ItemPrices_Create,

                       AppPermissions.Pages_Tenant_ItemPrices_Export,

                        AppPermissions.Pages_Tenant_ItemPrices_GetDetail,

                        AppPermissions.Pages_Tenant_ItemPrices_Update,


                        AppPermissions.Pages_Tenant_ItemPrices_Delete,

                        AppPermissions.Pages_Tenant_SetUp_Vendor,

                        AppPermissions.Pages_Tenant_SetUp_Vendor_GetList,

                         AppPermissions.Pages_Tenant_SetUp_Vendor_Create,

                        AppPermissions.Pages_Tenant_SetUp_Vendor_GetDetail,

                        AppPermissions.Pages_Tenant_SetUp_Vendor_Update,

                        AppPermissions.Pages_Tenant_SetUp_Vendor_Delete,

                        AppPermissions.Pages_Tenant_SetUp_Vendor_Disable,

                        AppPermissions.Pages_Tenant_SetUp_Vendor_Enable,

                        AppPermissions.Pages_Tenant_SetUp_Vendor_ExportExcel,

                        AppPermissions.Pages_Tenant_SetUp_Vendor_ImportExcel,

                        AppPermissions.Pages_Tenant_Setup_VendorTypes,

                        AppPermissions.Pages_Tenant_Setup_VendorTypes_GetList,

                        AppPermissions.Pages_Tenant_Setup_VendorTypes_Create,

                        AppPermissions.Pages_Tenant_Setup_VendorTypes_GetDetail,

                        AppPermissions.Pages_Tenant_Setup_VendorTypes_Update,
                        AppPermissions.Pages_Tenant_Setup_VendorTypes_Delete,
                        AppPermissions.Pages_Tenant_Setup_VendorTypes_Disable,
                        AppPermissions.Pages_Tenant_Setup_VendorTypes_Enable,
                        AppPermissions.Pages_Tenant_SetUp_Customer,

                        AppPermissions.Pages_Tenant_SetUp_Customer_GetList,

                        AppPermissions.Pages_Tenant_SetUp_Customer_Create,

                        AppPermissions.Pages_Tenant_SetUp_Customer_GetDetail,

                        AppPermissions.Pages_Tenant_SetUp_Customer_Update,

                        AppPermissions.Pages_Tenant_SetUp_Customer_Delete,

                        AppPermissions.Pages_Tenant_SetUp_Customer_Disable,

                        AppPermissions.Pages_Tenant_SetUp_Customer_Enable,

                        AppPermissions.Pages_Tenant_SetUp_Customer_ExportExcel,

                        AppPermissions.Pages_Tenant_SetUp_Customer_ImportExcel,

                        AppPermissions.Pages_Tenant_Setup_CustomerTypes,

                        AppPermissions.Pages_Tenant_Setup_CustomerTypes_GetList,

                        AppPermissions.Pages_Tenant_Setup_CustomerTypes_Create,

                        AppPermissions.Pages_Tenant_Setup_CustomerTypes_GetDetail,

                        AppPermissions.Pages_Tenant_Setup_CustomerTypes_Update,

                        AppPermissions.Pages_Tenant_Setup_CustomerTypes_Delete,

                        AppPermissions.Pages_Tenant_Setup_CustomerTypes_Disable,

                        AppPermissions.Pages_Tenant_Setup_CustomerTypes_Enable,

                        AppPermissions.Pages_Tenant_Setup_TransactionTypes,

                        AppPermissions.Pages_Tenant_Setup_TransactionTypes_GetList,

                        AppPermissions.Pages_Tenant_Setup_TransactionTypes_Create,

                        AppPermissions.Pages_Tenant_Setup_TransactionTypes_GetDetail,

                        AppPermissions.Pages_Tenant_Setup_TransactionTypes_Update,

                        AppPermissions.Pages_Tenant_Setup_TransactionTypes_Delete,

                        AppPermissions.Pages_Tenant_Setup_TransactionTypes_Disable,

                        AppPermissions.Pages_Tenant_Setup_TransactionTypes_Enable,

                        AppPermissions.Pages_Tenant_Setup_Classes,

                        AppPermissions.Pages_Tenant_Setup_Classes_GetList,

                        AppPermissions.Pages_Tenant_Setup_Classes_Create,

                        AppPermissions.Pages_Tenant_Setup_Classes_GetDetail,

                        AppPermissions.Pages_Tenant_Setup_Classes_Update,

                        AppPermissions.Pages_Tenant_Setup_Classes_Delete,

                        AppPermissions.Pages_Tenant_Setup_Classes_Disable,

                        AppPermissions.Pages_Tenant_Setup_Classes_Enable,

                        AppPermissions.Pages_Tenant_Setup_Locations,

                        AppPermissions.Pages_Tenant_Setup_Locations_GetList,

                        AppPermissions.Pages_Tenant_Setup_Locations_Create,

                        AppPermissions.Pages_Tenant_Setup_Locations_GetDetail,

                        AppPermissions.Pages_Tenant_Setup_Locations_Update,

                        AppPermissions.Pages_Tenant_Setup_Locations_Delete,

                        AppPermissions.Pages_Tenant_Setup_Locations_Disable,

                        AppPermissions.Pages_Tenant_Setup_Locations_Enable,

                        AppPermissions.Pages_Tenant_Setup_Lots,

                        AppPermissions.Pages_Tenant_Setup_Lots_GetList,

                        AppPermissions.Pages_Tenant_Setup_Lots_Create,

                        AppPermissions.Pages_Tenant_Setup_Lots_Import,

                        AppPermissions.Pages_Tenant_Setup_Lots_Export,

                        AppPermissions.Pages_Tenant_Setup_Lots_GetDetail,

                        AppPermissions.Pages_Tenant_Setup_Lots_Update,

                        AppPermissions.Pages_Tenant_Setup_Lots_Delete,

                        AppPermissions.Pages_Tenant_Setup_Lots_Disable,

                        AppPermissions.Pages_Tenant_Setup_Lots_Enable,

                        AppPermissions.Pages_Tenant_CompanyProfile_Organazition,

                        AppPermissions.Pages_Tenants_CompanyProfile_GetDetail,

                       AppPermissions.Pages_Tenants_CompanyProfile_ClearDefaultValue,

                        AppPermissions.Pages_Tenants_CompanyProfile_Update,

                        AppPermissions.Pages_Tenant_Setup_PaymentMethods,

                        AppPermissions.Pages_Tenant_Setup_PaymentMethods_GetList,

                        AppPermissions.Pages_Tenant_Setup_PaymentMethods_GetDetail,


                       AppPermissions.Pages_Tenant_Setup_PaymentMethods_Create,

                        AppPermissions.Pages_Tenant_Setup_PaymentMethods_Update,

                        AppPermissions.Pages_Tenant_Setup_PaymentMethods_Delete,

                        AppPermissions.Pages_Tenant_Setup_PaymentMethods_Disable,

                        AppPermissions.Pages_Tenant_Setup_PaymentMethods_Enable,

                        AppPermissions.Pages_Tenant_Exchanges,

                        AppPermissions.Pages_Tenant_Exchanges_GetList,


                        AppPermissions.Pages_Tenant_Exchanges_Create,

                        AppPermissions.Pages_Tenant_Exchanges_GetDetail,

                        AppPermissions.Pages_Tenant_Exchanges_Update,

                        AppPermissions.Pages_Tenant_Exchanges_Delete,

                        AppPermissions.Pages_Tenant_Exchanges_ApplyRate,

                        AppPermissions.Pages_Tenant_Customer_InvoiceTemplates,

                        AppPermissions.Pages_Tenant_Customer_InvoiceTemplates_GetList,

                        AppPermissions.Pages_Tenant_Customer_InvoiceTemplates_Create,

                        AppPermissions.Pages_Tenant_Customer_InvoiceTemplates_GetDetail,

                        AppPermissions.Pages_Tenant_Customer_InvoiceTemplates_Update,

                        AppPermissions.Pages_Tenant_Customer_InvoiceTemplates_Delete,

                        AppPermissions.Pages_Tenant_Customer_InvoiceTemplates_Enable,

                        AppPermissions.Pages_Tenant_Customer_InvoiceTemplates_Disable,

                        AppPermissions.Pages_Tenant_Report_CashFlowTemplate,

                        AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Create,

                        AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Update,

                        AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Delete,

                        AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Enable,

                        AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Disable,

                        AppPermissions.Pages_Tenant_Accounting,

                        AppPermissions.Pages_Tenant_Accounting_Journals,

                        AppPermissions.Pages_Tenant_Accounting_Journals_GetList,

                        AppPermissions.Pages_Tenant_Accounting_Journals_Create,

                        AppPermissions.Pages_Tenant_Accounting_Journals_GetDetail,

                        AppPermissions.Pages_Tenant_Accounting_Journals_Update,

                        AppPermissions.Pages_Tenant_Accounting_Journals_Delete,

                       AppPermissions.Pages_Tenant_Accounting_Journals_ImportExcel,

                       AppPermissions.Pages_Tenant_Accounting_Journals_ExportExcel,

                       AppPermissions.Pages_Tenant_Close_Period,

                        AppPermissions.Pages_Tenant_Close_Period_GetList,

                        AppPermissions.Pages_Tenant_Close_Period_Create,

                        AppPermissions.Pages_Tenant_Close_Period_Delete,


                        AppPermissions.Pages_Tenant_Accounting_Locks_GetList,

                        AppPermissions.Pages_Tenant_Accounting_Locks_CreateOrUpdate,

                        AppPermissions.Pages_Tenant_Accounting_Locks_GeneratePasswork,
                        AppPermissions.Pages_Tenant_Report,

                        AppPermissions.Pages_Tenant_Report_Common,

                        AppPermissions.Pages_Tenant_Report_ViewTemplate,
                        AppPermissions.Pages_Tenant_Report_ViewOriginal,

                        AppPermissions.Pages_Tenant_Report_Template_Create,

                        AppPermissions.Pages_Tenant_Report_Template_Update,

                        AppPermissions.Pages_Tenant_Report_Template_Delete,

                        AppPermissions.Pages_Tenant_Report_CanEditAllTemplate,

                        AppPermissions.Pages_Tenant_Report_Inventory,

                        AppPermissions.Pages_Tenant_Report_Stock_Balance,

                        AppPermissions.Pages_Tenant_Report_Stock_Balance_Export_Excel,

                        AppPermissions.Pages_Tenant_Report_Stock_Balance_Export_Pdf,

                        AppPermissions.Pages_Tenant_Report_Asset_Balance,

                        AppPermissions.Pages_Tenant_Report_Asset_Balance_Export_Excel,

                        AppPermissions.Pages_Tenant_Report_Asset_Balance_Export_Pdf,

                        AppPermissions.Pages_Tenant_Report_Inventory_InventoryTransaction,

                        AppPermissions.Pages_Tenant_Report_Inventory_InventoryTransaction_Export_Excel,

                        AppPermissions.Pages_Tenant_Report_Inventory_InventoryTransaction_Export_Pdf,

                        AppPermissions.Pages_Tenant_Report_Inventory_Summary,

                        AppPermissions.Pages_Tenant_Report_Inventory_Summary_Export_Excel,

                        AppPermissions.Pages_Tenant_Report_Inventory_Summary_RecalculateAvg,

                        AppPermissions.Pages_Tenant_Report_Inventory_Summary_Export_Pdf,

                        AppPermissions.Pages_Tenant_Report_Inventory_Detail,

                        AppPermissions.Pages_Tenant_Report_Inventory_Detail_Export_Excel,

                        AppPermissions.Pages_Tenant_Report_Inventory_Detail_Export_Pdf,

                        AppPermissions.Pages_Tenant_Report_Inventory_Detail_RecalculateAvg,

                        AppPermissions.Pages_Tenant_Report_Traceability,

                        AppPermissions.Pages_Tenant_Report_Traceability_Print,

                        AppPermissions.Pages_Tenant_Report_Traceability_ViewCustomer,


                        AppPermissions.Pages_Tenant_Report_Traceability_Download,


                        AppPermissions.Pages_Tenant_Report_BatchNoBalance,

                        AppPermissions.Pages_Tenant_Report_BatchNoBalance_ExportPdf,

                        AppPermissions.Pages_Tenant_Report_BatchNoBalance_ExportExcel,

                        AppPermissions.Pages_Tenant_Report_Production,


                        AppPermissions.Pages_Tenant_Report_ProductionPlan,

                        AppPermissions.Pages_Tenant_Report_ProductionPlan_ExportPdf,

                        AppPermissions.Pages_Tenant_Report_ProductionPlan_ExportExcel,

                       AppPermissions.Pages_Tenant_Report_ProductionPlan_Calculate,

                        AppPermissions.Pages_Tenant_Report_ProductionOrder,

                        AppPermissions.Pages_Tenant_Report_ProductionOrder_ExportPdf,

                        AppPermissions.Pages_Tenant_Report_ProductionOrder_ExportExcel,

                       AppPermissions.Pages_Tenant_Report_ProductionOrder_Calculate,

                        AppPermissions.Pages_Tenant_Report_Vendor,

                        AppPermissions.Pages_Tenant_Report_Purchasing,

                        AppPermissions.Pages_Tenant_Report_Purchasing_Export_Excel,

                        AppPermissions.Pages_Tenant_Report_Purchasing_Export_Pdf,

                        AppPermissions.Pages_Tenant_Report_Vendor_VendorAging,

                        AppPermissions.Pages_Tenant_Report_Vendor_VendorAging_Export_Excel,

                        AppPermissions.Pages_Tenant_Report_Vendor_VendorAging_Export_Pdf,


                        AppPermissions.Pages_Tenant_Report_Vendor_VendorByBill,

                        AppPermissions.Pages_Tenant_Report_Vendor_VendorByBill_Export_Excel,

                        AppPermissions.Pages_Tenant_Report_Vendor_VendorByBill_Export_Pdf,

                        AppPermissions.Pages_Tenant_Report_Customer,

                        AppPermissions.Pages_Tenant_Report_SaleInvoice,

                        AppPermissions.Pages_Tenant_Report_SaleInvoice_Export_Excel,

                        AppPermissions.Pages_Tenant_Report_SaleInvoice_Export_Pdf,

                        AppPermissions.Pages_Tenant_Report_SaleInvoiceDetail,

                        AppPermissions.Pages_Tenant_Report_SaleInvoiceDetail_Export_Excel,

                        AppPermissions.Pages_Tenant_Report_SaleInvoiceDetail_Export_Pdf,



                        AppPermissions.Pages_Tenant_Report_ProfitByInvoice,

                        AppPermissions.Pages_Tenant_Report_ProfitByInvoice_Export_Excel,

                        AppPermissions.Pages_Tenant_Report_ProfitByInvoice_Export_Pdf,



                        AppPermissions.Pages_Tenant_Report_Customer_CustomerAging,

                        AppPermissions.Pages_Tenant_Report_Customer_CustomerAging_Export_Excel,

                        AppPermissions.Pages_Tenant_Report_Customer_CustomerAging_Export_Pdf,

                        AppPermissions.Pages_Tenant_Report_Customer_CustomerByInvoice,

                        AppPermissions.Pages_Tenant_Report_Customer_CustomerByInvoice_Export_Excel,

                        AppPermissions.Pages_Tenant_Report_Customer_CustomerByInvoice_Export_Pdf,

                        AppPermissions.Pages_Tenant_Report_Customer_CustomerByInvoiceWithPayment,

                        AppPermissions.Pages_Tenant_Report_Customer_CustomerByInvoiceWithPayment_Export_Excel,

                        AppPermissions.Pages_Tenant_Report_Customer_CustomerByInvoiceWithPayment_Export_Pdf,

                        AppPermissions.Pages_Tenant_Report_Accounting,

                        AppPermissions.Pages_Tenant_Report_Journal,

                        AppPermissions.Pages_Tenant_Report_Journal_Update,

                        AppPermissions.Pages_Tenant_Report_Journal_Export_Excel,

                        AppPermissions.Pages_Tenant_Report_Journal_Export_Pdf,

                        AppPermissions.Pages_Tenant_Report_Income,

                        AppPermissions.Pages_Tenant_Report_Income_Export_Excel,

                        AppPermissions.Pages_Tenant_Report_Income_Export_Pdf,

                        AppPermissions.Pages_Tenant_Report_BalanceSheet,

                        AppPermissions.Pages_Tenant_Report_BalanceSheet_Export_Excel,

                        AppPermissions.Pages_Tenant_Report_BalanceSheet_Export_Pdf,

                        AppPermissions.Pages_Tenant_Report_Ledger,

                        AppPermissions.Pages_Tenant_Report_Ledger_Export_Excel,

                        AppPermissions.Pages_Tenant_Report_Ledger_Export_Pdf,

                        AppPermissions.Pages_Tenant_Report_Cash,

                        AppPermissions.Pages_Tenant_Report_Cash_Export_Excel,

                        AppPermissions.Pages_Tenant_Report_Cash_Export_Pdf,

                        AppPermissions.Pages_Tenant_Report_CashFlow,

                        AppPermissions.Pages_Tenant_Report_CashFlow_Export_Excel,

                        AppPermissions.Pages_Tenant_Report_CashFlow_Export_Pdf,

                        AppPermissions.Pages_Tenant_Report_PurchaseOrder,

                        AppPermissions.Pages_Tenant_Report_PurchaseOrder_Export_Excel,

                        AppPermissions.Pages_Tenant_Report_PurchaseOrder_Export_Pdf,

                        AppPermissions.Pages_Tenant_Report_SaleOrder,


                        AppPermissions.Pages_Tenant_Report_SaleOrder_Export_Excel,

                        AppPermissions.Pages_Tenant_Report_SaleOrder_Export_Pdf,


                        AppPermissions.Pages_Tenant_Report_SaleOrderDetail,

                        AppPermissions.Pages_Tenant_Report_SaleOrderDetail_Export_Excel,

                        AppPermissions.Pages_Tenant_Report_SaleOrderDetail_Export_Pdf,

                        AppPermissions.Pages_Tenant_UserActivities,

                        AppPermissions.Pages_Tenant_UserActivities_GetList,

                        AppPermissions.Pages_Tenant_UserActivities_SeeAll,

                        AppPermissions.Pages_Tenant_UserActivities_SeeMine,

                        AppPermissions.Pages_Tenant_UserActivities_ExportExcel,

                        AppPermissions.Pages_Tenant_Report_DeliverySchedule,
                        AppPermissions.Pages_Tenant_Report_DeliverySchedule_Detail,
                        AppPermissions.Pages_Tenant_Report_DeliverySchedule_Summary,
                        AppPermissions.Pages_Tenant_Report_DeliverySchedule_Summary_Export_Excel,
                        AppPermissions.Pages_Tenant_Report_DeliverySchedule_Summary_Export_Pdf,
                        AppPermissions.Pages_Tenant_Report_DeliverySchedule_Detail_Export_Excel,
                        AppPermissions.Pages_Tenant_Report_DeliverySchedule_Summary_Export_Pdf,


                });

                dic.Add(StaticRoleNames.Tenants.APAccountant, new List<string>()
                {
                    #region Vendor
                    // Start Permission for Vendor
                    AppPermissions.Pages,
                    AppPermissions.Pages_Tenant_App,
                    AppPermissions.Pages_Tenant_PurchaseOrder_Delete,
                    AppPermissions.Pages_Tenant_PurchaseOrder_Disable,
                    AppPermissions.Pages_Tenant_PurchaseOrder_Enable,
                    AppPermissions.Pages_Tenant_PurchaseOrder_Find,
                    AppPermissions.Pages_Tenant_PurchaseOrder_GetDetail,
                    AppPermissions.Pages_Tenant_PurchaseOrder_GetList,
                    AppPermissions.Pages_Tenant_PurchaseOrder_Update,
                    AppPermissions.Pages_Tenant_PurchaseOrder_Create,
                    AppPermissions.Pages_Tenant_PurchaseOrder_GetTotalPurchaseOrderForItemReceipts,
                    AppPermissions.Pages_Tenant_PurchaseOrder_GetTotalPurchaseOrderForBills,
                    AppPermissions.Pages_Tenant_PurchaseOrder_GetlistPuchaseOrderForItemReceipts,
                    AppPermissions.Pages_Tenant_PurchaseOrder_GetlistPuchaseOrderForBills,
                    AppPermissions.Pages_Tenant_PurchaseOrder_CanSeePrice,
                    AppPermissions.Pages_Tenant_PurchaseOrder_UpdateStatusToDraft,
                    AppPermissions.Pages_Tenant_PurchaseOrder_UpdateStatusToVoid,
                    AppPermissions.Pages_Tenant_PurchaseOrder_UpdateStatusToPublish,
                    AppPermissions.Pages_Tenant_PurchaseOrder_UpdateStatusToClose,
                    AppPermissions.Pages_Tenant_Accounting_Locks_Find,
                    AppPermissions.Pages_Tenant_Common_BatchNo_Find,
                    AppPermissions.Pages_Tenant_Vendors,
                    AppPermissions.Pages_Tenant_Vendors_Bills,
                    AppPermissions.Pages_Tenant_Vendors_Bills_Delete,
                    AppPermissions.Pages_Tenant_Vendors_Bills_Create,
                    AppPermissions.Pages_Tenant_Vendors_Bills_CanPO,
                    AppPermissions.Pages_Tenant_Vendors_Bills_CanItemReceipt,
                    AppPermissions.Pages_Tenant_Vendors_Bills_CanEditAccount,
                    AppPermissions.Pages_Tenant_Vendors_Bills_CanSeePrice,
                    AppPermissions.Pages_Tenant_Vendors_Bills_Update,
                    AppPermissions.Pages_Tenant_Vendors_Bills_UpdateStatusToPublish,
                    AppPermissions.Pages_Tenant_Vendors_Bills_UpdateStatusToVoid,
                    AppPermissions.Pages_Tenant_Vendors_Bills_UpdateStatusToDraft,
                    AppPermissions.Pages_Tenant_Vendors_Bills_GetList,
                    AppPermissions.Pages_Tenant_Vendors_Bills_GetListForPayBill,
                    AppPermissions.Pages_Tenant_Vendors_Bills_GetDetail,
                    AppPermissions.Pages_Tenant_Vendors_Bills_Find,
                    AppPermissions.Pages_Tenant_Vendors_Bills_GetBills,
                    AppPermissions.Pages_Tenant_Vendors_Bills_GetBillItems,

                    // vendor Credit
                    AppPermissions.Pages_Tenant_Vendors_Bills_CanCreateVendorCredit,
                    AppPermissions.Pages_Tenant_Vendors_Credit_Find,

                    AppPermissions.Pages_Tenant_Vendors_PayBills,
                    AppPermissions.Pages_Tenant_Vendors_PayBills_Delete,
                    AppPermissions.Pages_Tenant_Vendors_PayBills_Create,
                    AppPermissions.Pages_Tenant_Vendors_PayBills_Update,
                    AppPermissions.Pages_Tenant_Vendors_PayBills_GetList,
                    AppPermissions.Pages_Tenant_Vendors_PayBills_Find,
                    AppPermissions.Pages_Tenant_Vendors_PayBills_CanEditAccount,
                    AppPermissions.Pages_Tenant_Vendors_PayBills_CanPayByCredit,
                    AppPermissions.Pages_Tenant_Vendors_PayBills_GetDetail,
                    AppPermissions.Pages_Tenant_Vendors_PayBills_UpdateStatusToDraft,
                    AppPermissions.Pages_Tenant_Vendors_PayBills_UpdateStatusToPublish,
                    AppPermissions.Pages_Tenant_Vendors_PayBills_UpdateStatusToVoid,
                    AppPermissions.Pages_Tenant_Vendors_PayBills_ViewBillHistory,
                    AppPermissions.Pages_Tenant_Vendors_PayBills_ViewVendorCreditHistory,

                    AppPermissions.Pages_Tenant_Vendors_ItemReceipts,
                    AppPermissions.Pages_Tenant_Vendors_ItemReceipts_Delete,
                    AppPermissions.Pages_Tenant_Vendors_ItemReceipts_Create,
                    AppPermissions.Pages_Tenant_Vendors_ItemReceipts_Update,
                    AppPermissions.Pages_Tenant_Vendors_ItemReceipts_UpdateStatusToPublish,
                    AppPermissions.Pages_Tenant_Vendors_ItemReceipts_UpdateStatusToVoid,
                    AppPermissions.Pages_Tenant_Vendors_ItemReceipts_UpdateStatusToDraft,
                    AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetList,
                    AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetDetail,
                    AppPermissions.Pages_Tenant_Vendors_ItemReceipts_Find,
                    AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CanSeePrice,
                    AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CanPO,
                    AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CanBill,
                    AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CanEditAccount,
                    AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts,
                    AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceiptItems,

                    AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CreateAdjustments,
                    AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CreateCustomerCredits,
                    AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CreateOthers,
                    AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CreateProductionOrder,
                    AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CreateTransfers,

                     //journalTrasactionType
                     AppPermissions.Pages_Tenant_Common_Inventory_JournalTransactionTypes_Find,
                 
                    #endregion

                    #region Production
                    AppPermissions.Pages_Tenant_Production_Process_Find,
                    AppPermissions.Pages_Tenant_BOMs_Find,
                    #endregion

                    #region Setups
                    //Class and location
                    AppPermissions.Pages_Tenant_CompanyProfile_Organazition,
                    AppPermissions.Pages_Tenants_CompanyProfile_GetDetail,
                    AppPermissions.Pages_Tenant_Dashboard,
                    AppPermissions.Pages_Tenant_Setup_Classes_Find,
                    AppPermissions.Pages_Tenant_Setup_Locations_Find,
                    AppPermissions.Pages_Tenant_Setup_Lots_Find,
                    AppPermissions.Pages_Tenant_Properties_Find,
                    AppPermissions.Pages_Tenant_PropertyValue_FindValue,
                    AppPermissions.Pages_Tenant_Item_GetList,
                    AppPermissions.Pages_Tenant_Items,
                    AppPermissions.Pages_Tenant_Item_Find,
                    AppPermissions.Pages_Tenant_Item_GetDetail,
                    AppPermissions.Pages_Tenant_Vendor,
                    AppPermissions.Pages_Tenant_Vendor_GetList,
                    AppPermissions.Pages_Tenant_Vendor_Find,
                    AppPermissions.Pages_Tenant_Vendor_GetDetail,
                    AppPermissions.Pages_Tenant_Vendor_ExportExcel,
                    AppPermissions.Pages_Tenant_Customer_Find,
                    AppPermissions.Pages_Tenant_Setup_CustomerTypes_Find,
                    AppPermissions.Pages_Tenant_Setup_VendorTypes_Find,
                    AppPermissions.Pages_Tenant_Currencies_Find,
                    AppPermissions.Pages_Tenant_MultiCurrencies_Find,
                    AppPermissions.Pages_Tenant_Formats_FindNumber,
                    AppPermissions.Pages_Tenant_Formats_FindDate,
                    AppPermissions.Pages_Tenant_ItemType_Find,
                    AppPermissions.Pages_Tenant_Setup,
                    AppPermissions.Pages_Tenant_Commons,
                    AppPermissions.Pages_Tenant_Setup_Commons_FindAccountTypes,
                    AppPermissions.Pages_Tenant_Setup_Commons_FindTaxes,
                    AppPermissions.Pages_Tenant_Setup_Commons_FindAccounts,
                    AppPermissions.Pages_Tenant_Setup_ChartOfAccounts,
                    AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_Detail,
                    // Start permission for accounting 
                    AppPermissions.Pages_Tenant_Accounting_Journals_Find,
                   AppPermissions.Pages_Tenant_Setup_PaymentMethods_Find,
                    // End permission for Vendor


                    #endregion

                    #region Reports

                    // Start Report permission
                    AppPermissions.Pages_Tenant_Report,
                    AppPermissions.Pages_Tenant_Report_ViewTemplate,
                    AppPermissions.Pages_Tenant_Report_Accounting,
                    AppPermissions.Pages_Tenant_Report_Ledger,
                    AppPermissions.Pages_Tenant_Report_Ledger_Export_Pdf,
                    AppPermissions.Pages_Tenant_Report_Ledger_Export_Excel,
                    AppPermissions.Pages_Tenant_Report_Vendor,
                    AppPermissions.Pages_Tenant_Report_Purchasing_Export_Excel,
                    AppPermissions.Pages_Tenant_Report_Purchasing_Export_Pdf,
                    AppPermissions.Pages_Tenant_Report_Purchasing,
                    AppPermissions.Pages_Tenant_Report_Vendor_VendorAging,
                    AppPermissions.Pages_Tenant_Report_Vendor_VendorAging_Export_Excel,
                    AppPermissions.Pages_Tenant_Report_Vendor_VendorAging_Export_Pdf,
                    AppPermissions.Pages_Tenant_Report_Vendor_VendorByBill,
                    AppPermissions.Pages_Tenant_Report_Vendor_VendorByBill_Export_Excel,
                    AppPermissions.Pages_Tenant_Report_Vendor_VendorByBill_Export_Pdf,
                    AppPermissions.Pages_Tenant_Report_ViewOriginal,
                    AppPermissions.Pages_Tenant_Report_Template_Update,
                    AppPermissions.Pages_Tenant_Report_Template_Delete,
                    AppPermissions.Pages_Tenant_Report_Template_Create,


                    #endregion
                });

                dic.Add(StaticRoleNames.Tenants.ARAccountant, new List<string>()
                {
                    #region Production
                    AppPermissions.Pages,
                    AppPermissions.Pages_Tenant_App,
                    AppPermissions.Pages_Tenant_Production_Process_Find,
                    AppPermissions.Pages_Tenant_BOMs_Find,
                    #endregion
                    #region Customers
                    AppPermissions.Pages_Tenant_Customers,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_Delete,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_Disable,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_Enable,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_Find,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_CanSeePrice,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_GetDetail,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_GetList,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_Update,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_Create,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_UpdateToDraft,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_UpdateToVoid,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_UpdateToPublish,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_UpdateToClose,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_GetTotalSaleOrder,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_GetTotalSaleOrderForInvoice,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_GetItemSaleOrderForInvoices,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_GetItemSaleOrderForItemIssues,
                    AppPermissions.Pages_Tenant_Accounting_Locks_Find,

                    AppPermissions.Pages_Tenant_Customer_Invoices,
                    AppPermissions.Pages_Tenant_Customer_Invoice_Delete,
                    AppPermissions.Pages_Tenant_Customer_Invoice_Create,
                    AppPermissions.Pages_Tenant_Customer_Invoice_Update,
                    AppPermissions.Pages_Tenant_Customer_Invoice_UpdateToPublish,
                    AppPermissions.Pages_Tenant_Customer_Invoice_UpdateToVoid,
                    AppPermissions.Pages_Tenant_Customer_Invoice_UpdateToDraft,
                    AppPermissions.Pages_Tenant_Customer_Invoice_GetList,
                    AppPermissions.Pages_Tenant_Customer_Invoice_CanSeePrice,
                    AppPermissions.Pages_Tenant_Customer_Invoice_CanEditAccount,
                    AppPermissions.Pages_Tenant_Customer_Invoice_CanSaleOrder,
                    AppPermissions.Pages_Tenant_Customer_Invoice_CanItemIssue,
                    AppPermissions.Pages_Tenant_Customer_Invoice_GetDetail,
                    AppPermissions.Pages_Tenant_Customer_Invoice_Find,
                    AppPermissions.Pages_Tenant_Customer_Invoice_GetInvoiceSummaryForItemIssue,
                    AppPermissions.Pages_Tenant_Customer_Invoice_GetInvoiceItems,
                    AppPermissions.Pages_Tenant_Customer_Invoice_GetListInvoiceForReceivePayment,
                    // customer Credit
                    AppPermissions.Pages_Tenant_Customers_Invoice_CreateCustomerCredit,
                    AppPermissions.Pages_Tenant_Customers_Credit_Find,
                    
                     //journalTrasactionType
                     AppPermissions.Pages_Tenant_Common_Inventory_JournalTransactionTypes_Find,                    
                    //item issues
                    AppPermissions.Pages_Tenant_Customers_ItemIssues,
                    AppPermissions.Pages_Tenant_Customers_ItemIssues_Delete,
                    AppPermissions.Pages_Tenant_Customers_ItemIssues_Create,
                    AppPermissions.Pages_Tenant_Customers_ItemIssues_Update,
                    AppPermissions.Pages_Tenant_Customers_ItemIssues_CanSeePrice,
                    AppPermissions.Pages_Tenant_Customers_ItemIssues_CanEditAccount,
                    AppPermissions.Pages_Tenant_Customers_ItemIssues_CanSaleOrder,
                    AppPermissions.Pages_Tenant_Customers_ItemIssues_CanInvoice,
                     AppPermissions.Pages_Tenant_Customers_ItemIssues_CanDelivery,
                    AppPermissions.Pages_Tenant_Customers_ItemIssues_UpdateStatusToPublish,
                    AppPermissions.Pages_Tenant_Customers_ItemIssues_UpdateStatusToVoid,
                    AppPermissions.Pages_Tenant_Customers_ItemIssues_UpdateStatusToDraft,
                    AppPermissions.Pages_Tenant_Customers_ItemIssues_GetList,
                    AppPermissions.Pages_Tenant_Customers_ItemIssues_GetDetail,
                    AppPermissions.Pages_Tenant_Customers_ItemIssues_Find,
                    AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssues,
                    AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssueItems,

                    AppPermissions.Pages_Tenant_Customers_ItemIssues_CanCreateAdjustment,
                    AppPermissions.Pages_Tenant_Customers_ItemIssues_CanCreateOther,
                    AppPermissions.Pages_Tenant_Customers_ItemIssues_CanCreateProductionOrder,
                    AppPermissions.Pages_Tenant_Customers_ItemIssues_CanCreateTransfer,
                    AppPermissions.Pages_Tenant_Customers_ItemIssues_CanCreateVendorCredit,


                    AppPermissions.Pages_Tenant_Customers_ReceivePayments,
                    AppPermissions.Pages_Tenant_Customers_ReceivePayments_Delete,
                    AppPermissions.Pages_Tenant_Customers_ReceivePayments_Create,
                    AppPermissions.Pages_Tenant_Customers_ReceivePayments_Update,
                    AppPermissions.Pages_Tenant_Customers_ReceivePayments_GetList,
                    AppPermissions.Pages_Tenant_Customers_ReceivePayments_Find,
                    AppPermissions.Pages_Tenant_Customers_ReceivePayments_GetDetail,
                    AppPermissions.Pages_Tenant_Customers_ReceivePayments_CanEditAccount,
                    AppPermissions.Pages_Tenant_Customers_ReceivePayments_UpdateStatusToDraft,
                    AppPermissions.Pages_Tenant_Customers_ReceivePayments_UpdateStatusToPublish,
                    AppPermissions.Pages_Tenant_Customers_ReceivePayments_UpdateStatusToVoid,
                    AppPermissions.Pages_Tenant_Customers_ReceivePayments_ViewInvoiceHistory,
                    AppPermissions.Pages_Tenant_Customers_ReceivePayments_ViewCustomerCreditHistory,


                    #endregion
                    #region Setups
                    //Class and location
                    AppPermissions.Pages_Tenant_CompanyProfile_Organazition,
                    AppPermissions.Pages_Tenants_CompanyProfile_GetDetail,
                    AppPermissions.Pages_Tenant_Dashboard,
                    AppPermissions.Pages_Tenant_Setup_Classes_Find,
                    AppPermissions.Pages_Tenant_Common_BatchNo_Find,
                    AppPermissions.Pages_Tenant_Setup_Locations_Find,
                    AppPermissions.Pages_Tenant_Setup_Lots_Find,

                    AppPermissions.Pages_Tenant_Properties_Find,
                    AppPermissions.Pages_Tenant_PropertyValue_FindValue,

                    AppPermissions.Pages_Tenant_Items,
                    AppPermissions.Pages_Tenant_Item_Find,
                    AppPermissions.Pages_Tenant_Item_GetDetail,
                    AppPermissions.Pages_Tenant_Item_GetList,
                    AppPermissions.Pages_Tenant_Vendor_Find,



                    AppPermissions.Pages_Tenant_Customer,
                    AppPermissions.Pages_Tenant_Customer_GetList,
                    AppPermissions.Pages_Tenant_Customer_Find,
                    AppPermissions.Pages_Tenant_Customer_GetDetail,
                    AppPermissions.Pages_Tenant_Customer_ExportExcel,

                    AppPermissions.Pages_Tenant_Setup_CustomerTypes_Find,
                    AppPermissions.Pages_Tenant_Setup_VendorTypes_Find,

                    AppPermissions.Pages_Tenant_Setup_TransactionTypes_Find,

                    AppPermissions.Pages_Tenant_Currencies_Find,
                    AppPermissions.Pages_Tenant_MultiCurrencies_Find,


                    AppPermissions.Pages_Tenant_Formats_FindNumber,
                    AppPermissions.Pages_Tenant_Formats_FindDate,



                    AppPermissions.Pages_Tenant_ItemType_Find,

                    AppPermissions.Pages_Tenant_Setup,
                    AppPermissions.Pages_Tenant_Commons,
                    AppPermissions.Pages_Tenant_Setup_Commons_FindAccountTypes,
                    AppPermissions.Pages_Tenant_Setup_Commons_FindTaxes,
                    AppPermissions.Pages_Tenant_Setup_Commons_FindAccounts,


                    AppPermissions.Pages_Tenant_Setup_ChartOfAccounts,
                    AppPermissions.Pages_Tenant_Setup_ChartOfAccounts_Detail,
                    AppPermissions.Pages_Tenant_Setup_PaymentMethods_Find,
                  
                    // End permission for Vendor


                    #endregion

                    #region Reports
                    // Start Report permission
                    AppPermissions.Pages_Tenant_Report,
                    AppPermissions.Pages_Tenant_Report_ViewTemplate,
                    AppPermissions.Pages_Tenant_Report_Accounting,
                    AppPermissions.Pages_Tenant_Report_Ledger,
                    AppPermissions.Pages_Tenant_Report_Ledger_Export_Pdf,
                    AppPermissions.Pages_Tenant_Report_Ledger_Export_Excel,
                    AppPermissions.Pages_Tenant_Report_Customer,
                    AppPermissions.Pages_Tenant_Report_SaleInvoice_Export_Excel,
                    AppPermissions.Pages_Tenant_Report_SaleInvoice_Export_Pdf,
                    AppPermissions.Pages_Tenant_Report_SaleInvoice,
                    AppPermissions.Pages_Tenant_Report_Customer_CustomerAging,
                    AppPermissions.Pages_Tenant_Report_Customer_CustomerAging_Export_Excel,
                    AppPermissions.Pages_Tenant_Report_Customer_CustomerAging_Export_Pdf,
                    AppPermissions.Pages_Tenant_Report_Customer_CustomerByInvoice,
                    AppPermissions.Pages_Tenant_Report_Customer_CustomerByInvoice_Export_Excel,
                    AppPermissions.Pages_Tenant_Report_Customer_CustomerByInvoice_Export_Pdf,
                    AppPermissions.Pages_Tenant_Report_ViewOriginal,
                    AppPermissions.Pages_Tenant_Report_Template_Update,
                    AppPermissions.Pages_Tenant_Report_Template_Delete,
                    AppPermissions.Pages_Tenant_Report_Template_Create,
                    #endregion
                });

                dic.Add(StaticRoleNames.Tenants.SaleManager, new List<string>() {
                    #region Customer
                    AppPermissions.Pages,
                     AppPermissions.Pages_Tenant_App,
                    AppPermissions.Pages_Tenant_Customer,
                    AppPermissions.Pages_Tenant_Customer_Delete,
                    AppPermissions.Pages_Tenant_Customer_GetList,
                    AppPermissions.Pages_Tenant_Customer_Find,
                    AppPermissions.Pages_Tenant_Customer_Update,
                    AppPermissions.Pages_Tenant_Customer_GetDetail,
                    AppPermissions.Pages_Tenant_Customer_Disable,
                    AppPermissions.Pages_Tenant_Customer_Enable,
                    AppPermissions.Pages_Tenant_Customer_Create,
                    AppPermissions.Pages_Tenant_Setup_CustomerTypes,
                    AppPermissions.Pages_Tenant_Setup_CustomerTypes_Delete,
                    AppPermissions.Pages_Tenant_Setup_CustomerTypes_Create,
                    AppPermissions.Pages_Tenant_Setup_CustomerTypes_Update,
                    AppPermissions.Pages_Tenant_Setup_CustomerTypes_Disable,
                    AppPermissions.Pages_Tenant_Setup_CustomerTypes_Enable,
                    AppPermissions.Pages_Tenant_Setup_CustomerTypes_GetList,
                    AppPermissions.Pages_Tenant_Setup_CustomerTypes_GetDetail,
                    AppPermissions.Pages_Tenant_Setup_CustomerTypes_Find,
                    AppPermissions.Pages_Tenant_Accounting_Locks_Find,
                    AppPermissions.Pages_Tenant_Setup_VendorTypes,
                    AppPermissions.Pages_Tenant_Setup_VendorTypes_Delete,
                    AppPermissions.Pages_Tenant_Setup_VendorTypes_Create,
                    AppPermissions.Pages_Tenant_Setup_VendorTypes_Update,
                    AppPermissions.Pages_Tenant_Setup_VendorTypes_Disable,
                    AppPermissions.Pages_Tenant_Setup_VendorTypes_Enable,
                    AppPermissions.Pages_Tenant_Setup_VendorTypes_GetList,
                    AppPermissions.Pages_Tenant_Setup_VendorTypes_GetDetail,
                    AppPermissions.Pages_Tenant_Setup_VendorTypes_Find,
                    AppPermissions.Pages_Tenant_Setup_Commons_FindAccounts,

                    #endregion
                    AppPermissions.Pages_Tenant_Dashboard,

                    #region Customer Auth
                    AppPermissions.Pages_Tenant_Customers,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_Delete,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_Disable,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_Enable,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_Find,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_CanSeePrice,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_GetDetail,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_GetList,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_Update,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_Create,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_UpdateToDraft,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_UpdateToVoid,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_UpdateToPublish,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_UpdateToClose,
                    #endregion
                     AppPermissions.Pages_Tenant_BOMs_Find,
                    AppPermissions.Pages_Tenant_Report,
                    AppPermissions.Pages_Tenant_Report_ViewTemplate,
                    AppPermissions.Pages_Tenant_Report_SaleOrder,
                    AppPermissions.Pages_Tenant_Report_SaleOrder_Export_Excel,
                    AppPermissions.Pages_Tenant_Report_SaleOrder_Export_Pdf,
                    AppPermissions.Pages_Tenant_Report_SaleOrderDetail,
                    AppPermissions. Pages_Tenant_Report_SaleOrderDetail_Export_Excel,
                    AppPermissions. Pages_Tenant_Report_SaleOrderDetail_Export_Pdf,
                    AppPermissions.Pages_Tenant_Setup_Locations_Find,
                    AppPermissions.Pages_Tenant_Item_Find,
                    AppPermissions.Pages_Tenant_Report_ViewOriginal,
                    AppPermissions.Pages_Tenant_Report_Template_Update,
                    AppPermissions.Pages_Tenant_Report_Template_Delete,
                    AppPermissions.Pages_Tenant_Report_Template_Create,
                    AppPermissions.Pages_Tenant_Setup_TransactionTypes_Find,
                     //journalTrasactionType
                    AppPermissions.Pages_Tenant_Common_Inventory_JournalTransactionTypes_Find,
                    AppPermissions.Pages_Tenant_Customer_Cards_GetCustomerIdByCardId



    });

                dic.Add(StaticRoleNames.Tenants.PurchaseManager, new List<string>() {
                    #region Setups
                    AppPermissions.Pages,
                    AppPermissions.Pages_Tenant_App,
                    AppPermissions.Pages_Tenant_Dashboard,
                    AppPermissions.Pages_Tenant_Vendor,
                    AppPermissions.Pages_Tenant_Vendor_Delete,
                    AppPermissions.Pages_Tenant_Vendor_GetList,
                    AppPermissions.Pages_Tenant_Vendor_Find,
                    AppPermissions.Pages_Tenant_Vendor_Update,
                    AppPermissions.Pages_Tenant_Vendor_GetDetail,
                    AppPermissions.Pages_Tenant_Vendor_Disable,
                    AppPermissions.Pages_Tenant_Vendor_Enable,
                    AppPermissions.Pages_Tenant_Vendor_Create,
                    AppPermissions.Pages_Tenant_Accounting_Locks_Find,
                    // Start Permission for Vendor
                    AppPermissions.Pages_Tenant_Vendors,
                    AppPermissions.Pages_Tenant_PurchaseOrder_Delete,
                    AppPermissions.Pages_Tenant_PurchaseOrder_Disable,
                    AppPermissions.Pages_Tenant_PurchaseOrder_Enable,
                    AppPermissions.Pages_Tenant_PurchaseOrder_Find,
                    AppPermissions.Pages_Tenant_PurchaseOrder_GetDetail,
                    AppPermissions.Pages_Tenant_PurchaseOrder_GetList,
                    AppPermissions.Pages_Tenant_PurchaseOrder_Update,
                    AppPermissions.Pages_Tenant_PurchaseOrder_Create,
                    AppPermissions.Pages_Tenant_PurchaseOrder_CanSeePrice,
                    AppPermissions.Pages_Tenant_PurchaseOrder_UpdateStatusToDraft,
                    AppPermissions.Pages_Tenant_PurchaseOrder_UpdateStatusToVoid,
                    AppPermissions.Pages_Tenant_PurchaseOrder_UpdateStatusToPublish,
                    AppPermissions.Pages_Tenant_PurchaseOrder_UpdateStatusToClose,
                    AppPermissions.Pages_Tenant_Setup_Commons_FindAccounts,
                    AppPermissions.Pages_Tenant_Setup_Commons_FindTaxes,
                    #endregion

                    AppPermissions.Pages_Tenant_Report,
                    AppPermissions.Pages_Tenant_Report_ViewTemplate,
                    AppPermissions.Pages_Tenant_Report_PurchaseOrder,
                    AppPermissions.Pages_Tenant_Report_PurchaseOrder_Export_Excel,
                    AppPermissions.Pages_Tenant_Report_PurchaseOrder_Export_Pdf,
                    AppPermissions.Pages_Tenant_Report_PurchaseOrder,
                    AppPermissions.Pages_Tenant_Setup_Locations_Find,
                    AppPermissions.Pages_Tenant_Item_Find,
                    AppPermissions.Pages_Tenant_Report_ViewOriginal,
                    AppPermissions.Pages_Tenant_Report_Template_Update,
                    AppPermissions.Pages_Tenant_Report_Template_Delete,
                    AppPermissions.Pages_Tenant_Report_Template_Create,
                    AppPermissions.Pages_Tenant_Setup_TransactionTypes_Find,
                    AppPermissions.Pages_Tenant_Setup_VendorTypes_Find,
                    AppPermissions.Pages_Tenant_BOMs_Find
                });

                dic.Add(StaticRoleNames.Tenants.WarehouseManager, new List<string>() {

                    AppPermissions.Pages,
                    AppPermissions.Pages_Tenant_Dashboard,
                    AppPermissions.Pages_Tenant_App,
                    //AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CanBill,
                    AppPermissions.Pages_Tenant_Accounting_Locks_Find,
                    AppPermissions.Pages_Tenant_PurchaseOrder_GetlistPuchaseOrderForItemReceipts,
                    //AppPermissions.Pages_Tenant_Vendors_Bills_GetBillItems,
                    //AppPermissions.Pages_Tenant_Vendors_Bills_GetBills,
                    AppPermissions.Pages_Tenant_PurchaseOrder_GetTotalPurchaseOrderForItemReceipts,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_GetTotalSaleOrder,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_GetItemSaleOrderForItemIssues,
                    //AppPermissions.Pages_Tenant_Customer_Invoice_GetInvoiceSummaryForItemIssue,
                    //AppPermissions.Pages_Tenant_Customer_Invoice_GetInvoiceItems,
                     AppPermissions.Pages_Tenant_BOMs_Find,
                     AppPermissions.Pages_Tenant_Production_ProductionOrder_Find,
                    AppPermissions.Pages_Tenant_Production_Process_Find,
                    AppPermissions.Pages_Tenant_Report_Production,
                    AppPermissions.Pages_Tenant_Report_ProductionPlan,
                    AppPermissions.Pages_Tenant_Report_ProductionPlan_ExportPdf,
                    AppPermissions.Pages_Tenant_Report_ProductionPlan_ExportExcel,
                    AppPermissions.Pages_Tenant_Report_ProductionPlan_Calculate,
                    AppPermissions.Pages_Tenant_Report_ProductionOrder,
                    AppPermissions.Pages_Tenant_Report_ProductionOrder_ExportPdf,
                    AppPermissions.Pages_Tenant_Report_ProductionOrder_ExportExcel,
                    AppPermissions.Pages_Tenant_Report_ProductionOrder_Calculate,

                    #region Find
                    AppPermissions.Pages_Tenant_MultiCurrencies_Find,
                    AppPermissions.Pages_Tenant_Currencies_Find,
                    AppPermissions.Pages_Tenant_Customer_Find,
                    AppPermissions.Pages_Tenant_ItemType_Find,
                    AppPermissions.Pages_Tenant_Item_Find,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_Find,
                    AppPermissions.Pages_Tenant_Production_Process_Find,
                    AppPermissions.Pages_Tenant_Properties_Find,
                    AppPermissions.Pages_Tenant_PropertyValue_FindValue,
                    AppPermissions.Pages_Tenant_PurchaseOrder_Find,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_Find,
                    AppPermissions.Pages_Tenant_Setup_Classes_Find,
                    AppPermissions.Pages_Tenant_Setup_Commons_FindAccounts,
                    AppPermissions.Pages_Tenant_Setup_Commons_FindTaxes,
                    AppPermissions.Pages_Tenant_Setup_Commons_FindAccountTypes,
                    AppPermissions.Pages_Tenant_Setup_Locations_Find,
                    AppPermissions.Pages_Tenant_Setup_Lots_Find,
                    AppPermissions.Pages_Tenant_Vendor_Find,


                     //journalTrasactionType
                     AppPermissions.Pages_Tenant_Common_Inventory_JournalTransactionTypes_Find,                     
                    #endregion

                    // End permission for Vendor

                    AppPermissions.Pages_Tenant_Inventorys,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_GetList,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptPurchase,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptTransfer,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptCustomerCredit,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptOther,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptProduction,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueSale,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueTransfer,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueVendorCredit,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueOther,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueProduction,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ViewDetail,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Update,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Delete,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Void,
                    AppPermissions.Pages_Tenant_Inventory_TransferOrder,
                    AppPermissions.Pages_Tenant_Inventory_TransferOrder_Delete,
                    AppPermissions.Pages_Tenant_Inventory_TransferOrder_GetDetail,
                    AppPermissions.Pages_Tenant_Inventory_TransferOrder_ForItemIssue,
                    AppPermissions.Pages_Tenant_Inventory_TransferOrder_ForItemReceipt,
                    AppPermissions.Pages_Tenant_Inventory_TransferOrder_GetList,
                    AppPermissions.Pages_Tenant_Inventory_TransferOrder_Update,
                    AppPermissions.Pages_Tenant_Inventory_TransferOrder_Create,
                    AppPermissions.pages_Tenant_Inventory_TransferOrder_AutoInventory,
                    AppPermissions.Pages_Tenant_Inventory_TransferOrder_EditDelete48hour,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueAdjustment,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ExportItemIssueAdjustment,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ImportItemIssueAdjustment,
                    AppPermissions.Pages_Tenant_Customers_ItemIssues_ExportExcelTemplateItemIssueAdjustment,
                    AppPermissions.Pages_Tenant_Vendors_ItemReceipts_importExcelItemReceiptAdjustment,
                    AppPermissions.Pages_Tenant_Vendors_ItemReceipts_exportExcelItemReceiptAdjustment,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ImportItemReceiptAdjustment,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptAdjustment,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ImportItemReceiptOther,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ExportItemReceiptOther,
                    AppPermissions.Pages_Tenant_Items,
                    AppPermissions.Pages_Tenant_Item_GetList,
                    AppPermissions.Pages_Tenant_Item_GetDetail,
                    AppPermissions.Pages_Tenant_Setup_Lots,
                    AppPermissions.Pages_Tenant_Setup_Lots_Delete,
                    AppPermissions.Pages_Tenant_Setup_Lots_Create,
                    AppPermissions.Pages_Tenant_Setup_Lots_Update,
                    AppPermissions.Pages_Tenant_Setup_Lots_Disable,
                    AppPermissions.Pages_Tenant_Setup_Lots_Enable,
                    AppPermissions.Pages_Tenant_Setup_Lots_Find,
                    AppPermissions.Pages_Tenant_Setup_Lots_GetList,
                    AppPermissions.Pages_Tenant_Setup_Lots_GetDetail,
                    AppPermissions.Pages_Tenant_Setup_Lots_Export,
                    AppPermissions.Pages_Tenant_Setup_Lots_Import,
                    AppPermissions.Pages_Tenant_Setup,
                    AppPermissions.Pages_Tenant_Setup_Locations,
                    AppPermissions.Pages_Tenant_Setup_Locations_GetDetail,
                    AppPermissions.Pages_Tenant_Setup_Locations_GetList,

                    AppPermissions.Pages_Tenant_Report,
                    AppPermissions.Pages_Tenant_Report_Inventory,

                    AppPermissions.Pages_Tenant_Report_Inventory_InventoryTransaction,
                    AppPermissions.Pages_Tenant_Report_Inventory_InventoryTransaction_Export_Excel,
                    AppPermissions.Pages_Tenant_Report_Inventory_InventoryTransaction_Export_Pdf,
                    AppPermissions.Pages_Tenant_Report_Inventory_Summary,
                    AppPermissions.Pages_Tenant_Report_Inventory_Summary_Export_Excel,
                    AppPermissions.Pages_Tenant_Report_Inventory_Summary_Export_Pdf,
                    AppPermissions.Pages_Tenant_Report_ViewTemplate,

                    AppPermissions.Pages_Tenant_Production,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_Common,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_CanSeePrice,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_CanEditAcccount,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_CanViewAcccount,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_CanViewCalculationState,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_AutoInventory,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_GetList,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_Create,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_GetDetail,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_Update,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_Delete,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_Calculate,
                    AppPermissions.Pages_Tenant_Production_Process,
                    AppPermissions.Pages_Tenant_Production_Process_GetList,
                    AppPermissions.Pages_Tenant_Production_Process_Create,
                    AppPermissions.Pages_Tenant_Production_Process_GetDetail,
                    AppPermissions.Pages_Tenant_Production_Process_Update,
                    AppPermissions.Pages_Tenant_Production_Process_Delete,
                    AppPermissions.Pages_Tenant_Production_Process_UpdateStatusToDisable,
                    AppPermissions.Pages_Tenant_Production_Process_UpdateStatusToEnable,
                    AppPermissions.Pages_Tenant_Production_Plan,
                    AppPermissions.Pages_Tenant_Production_Plan_GetList,
                    AppPermissions.Pages_Tenant_Production_Plan_Create,
                    AppPermissions.Pages_Tenant_Production_Plan_GetDetail,
                    AppPermissions.Pages_Tenant_Production_Plan_Update,
                    AppPermissions.Pages_Tenant_Production_Plan_Delete,
                    AppPermissions.Pages_Tenant_Production_Plan_Close,
                    AppPermissions.Pages_Tenant_Production_Plan_Open,
                    AppPermissions.Pages_Tenant_Production_Plan_Calculate,
                    AppPermissions.Pages_Tenant_Production_Line,
                    AppPermissions.Pages_Tenant_Production_Line_GetList,
                    AppPermissions.Pages_Tenant_Production_Line_Create,
                    AppPermissions.Pages_Tenant_Production_Line_GetDetail,
                    AppPermissions.Pages_Tenant_Production_Line_Update,
                    AppPermissions.Pages_Tenant_Production_Line_Delete,
                    AppPermissions.Pages_Tenant_Production_Line_Enable,
                    AppPermissions.Pages_Tenant_Production_Line_Disable,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptPurchase_CanProductionOrder,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueProduction_CanProductionOrder,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_GetProductionOrderForItemIssue,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_GetProductionOrderForItemReceipt,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueProduction_CanProductionOrder,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueProduction,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptProduction,
                    AppPermissions.Pages_Tenant_Customers_ItemIssues_CanCreateProductionOrder,
                    AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CreateProductionOrder,
                    AppPermissions.Pages_Tenant_Production_Line_Find,
                    AppPermissions.Pages_Tenant_Production_Plan_Find,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_GetListProductionOrderForItemIssue,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_GetListProductionOrderForItemReceipt,
                });

                dic.Add(StaticRoleNames.Tenants.StockController, new List<string>() {

                    AppPermissions.Pages,
                    AppPermissions.Pages_Tenant_Dashboard,
                    AppPermissions.Pages_Tenant_App,
                    //AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CanBill,

                    AppPermissions.Pages_Tenant_PurchaseOrder_GetlistPuchaseOrderForItemReceipts,
                    //AppPermissions.Pages_Tenant_Vendors_Bills_GetBillItems,
                    //AppPermissions.Pages_Tenant_Vendors_Bills_GetBills,
                    AppPermissions.Pages_Tenant_PurchaseOrder_GetTotalPurchaseOrderForItemReceipts,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_GetTotalSaleOrder,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_GetItemSaleOrderForItemIssues,
                    //AppPermissions.Pages_Tenant_Customer_Invoice_GetInvoiceSummaryForItemIssue,
                    //AppPermissions.Pages_Tenant_Customer_Invoice_GetInvoiceItems,



                    #region Find
                    AppPermissions.Pages_Tenant_BOMs_Find,
                    AppPermissions.Pages_Tenant_MultiCurrencies_Find,
                    AppPermissions.Pages_Tenant_Currencies_Find,
                    AppPermissions.Pages_Tenant_Customer_Find,
                    AppPermissions.Pages_Tenant_ItemType_Find,
                    AppPermissions.Pages_Tenant_Item_Find,
                    AppPermissions.Pages_Tenant_Setup_TransactionTypes_Find,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_Find,
                    AppPermissions.Pages_Tenant_Production_Process_Find,
                    AppPermissions.Pages_Tenant_Properties_Find,
                    AppPermissions.Pages_Tenant_PropertyValue_FindValue,
                    AppPermissions.Pages_Tenant_PurchaseOrder_Find,
                    AppPermissions.Pages_Tenant_Customer_SaleOrder_Find,
                    AppPermissions.Pages_Tenant_Setup_Classes_Find,
                    AppPermissions.Pages_Tenant_Setup_Commons_FindAccounts,
                    AppPermissions.Pages_Tenant_Setup_Commons_FindTaxes,
                    AppPermissions.Pages_Tenant_Setup_Commons_FindAccountTypes,
                    AppPermissions.Pages_Tenant_Setup_Locations_Find,
                    AppPermissions.Pages_Tenant_Setup_Lots_Find,
                    AppPermissions.Pages_Tenant_Vendor_Find,
                    AppPermissions.Pages_Tenant_Accounting_Locks_Find,
                     //journalTrasactionType
                    AppPermissions.Pages_Tenant_Common_Inventory_JournalTransactionTypes_Find,                    
                    #endregion

                    // End permission for Vendor

                    AppPermissions.Pages_Tenant_Inventorys,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_GetList,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptPurchase,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptTransfer,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptCustomerCredit,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptOther,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptProduction,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueSale,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueTransfer,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueVendorCredit,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueOther,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueProduction,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ViewDetail,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ImportItemReceiptOther,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ExportItemReceiptOther,

                    AppPermissions.Pages_Tenant_Inventory_TransferOrder,
                    AppPermissions.Pages_Tenant_Inventory_TransferOrder_GetDetail,
                    AppPermissions.Pages_Tenant_Inventory_TransferOrder_ForItemIssue,
                    AppPermissions.Pages_Tenant_Inventory_TransferOrder_ForItemReceipt,
                    AppPermissions.Pages_Tenant_Inventory_TransferOrder_GetList,
                    AppPermissions.Pages_Tenant_Inventory_TransferOrder_Create,
                    AppPermissions.pages_Tenant_Inventory_TransferOrder_AutoInventory,
                    AppPermissions.Pages_Tenant_Inventory_TransferOrder_EditDelete48hour,
                    AppPermissions.Pages_Tenant_Setup,
                    AppPermissions.Pages_Tenant_Items,
                    AppPermissions.Pages_Tenant_Item_GetList,
                    AppPermissions.Pages_Tenant_Item_GetDetail,


                    AppPermissions.Pages_Tenant_Report,
                    AppPermissions.Pages_Tenant_Report_Inventory,

                    AppPermissions.Pages_Tenant_Report_Inventory_InventoryTransaction,
                    AppPermissions.Pages_Tenant_Report_Inventory_InventoryTransaction_Export_Excel,
                    AppPermissions.Pages_Tenant_Report_Inventory_InventoryTransaction_Export_Pdf,

                    AppPermissions.Pages_Tenant_Report_Inventory_Summary,
                    AppPermissions.Pages_Tenant_Report_Inventory_Summary_Export_Excel,
                    AppPermissions.Pages_Tenant_Report_Inventory_Summary_Export_Pdf,
                    AppPermissions.Pages_Tenant_Report_ViewTemplate,

                    AppPermissions.Pages_Tenant_Production,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_Common,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_CanSeePrice,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_CanEditAcccount,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_CanViewAcccount,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_CanViewCalculationState,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_AutoInventory,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_GetList,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_Create,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_GetDetail,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_Update,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_Delete,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_Calculate,
                    AppPermissions.Pages_Tenant_Production_Process,
                    AppPermissions.Pages_Tenant_Production_Process_GetList,
                    AppPermissions.Pages_Tenant_Production_Process_Create,
                    AppPermissions.Pages_Tenant_Production_Process_GetDetail,
                    AppPermissions.Pages_Tenant_Production_Process_Update,
                    AppPermissions.Pages_Tenant_Production_Process_Delete,
                    AppPermissions.Pages_Tenant_Production_Process_UpdateStatusToDisable,
                    AppPermissions.Pages_Tenant_Production_Process_UpdateStatusToEnable,
                    AppPermissions.Pages_Tenant_Production_Plan,
                    AppPermissions.Pages_Tenant_Production_Plan_GetList,
                    AppPermissions.Pages_Tenant_Production_Plan_Create,
                    AppPermissions.Pages_Tenant_Production_Plan_GetDetail,
                    AppPermissions.Pages_Tenant_Production_Plan_Update,
                    AppPermissions.Pages_Tenant_Production_Plan_Delete,
                    AppPermissions.Pages_Tenant_Production_Plan_Close,
                    AppPermissions.Pages_Tenant_Production_Plan_Open,
                    AppPermissions.Pages_Tenant_Production_Plan_Calculate,
                    AppPermissions.Pages_Tenant_Production_Line,
                    AppPermissions.Pages_Tenant_Production_Line_GetList,
                    AppPermissions.Pages_Tenant_Production_Line_Create,
                    AppPermissions.Pages_Tenant_Production_Line_GetDetail,
                    AppPermissions.Pages_Tenant_Production_Line_Update,
                    AppPermissions.Pages_Tenant_Production_Line_Delete,
                    AppPermissions.Pages_Tenant_Production_Line_Enable,
                    AppPermissions.Pages_Tenant_Production_Line_Disable,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_Find,
                    AppPermissions.Pages_Tenant_Production_Process_Find,
                    AppPermissions.Pages_Tenant_Report_Production,
                    AppPermissions.Pages_Tenant_Report_ProductionPlan,
                    AppPermissions.Pages_Tenant_Report_ProductionPlan_ExportPdf,
                    AppPermissions.Pages_Tenant_Report_ProductionPlan_ExportExcel,
                    AppPermissions.Pages_Tenant_Report_ProductionPlan_Calculate,
                    AppPermissions.Pages_Tenant_Report_ProductionOrder,
                    AppPermissions.Pages_Tenant_Report_ProductionOrder_ExportPdf,
                    AppPermissions.Pages_Tenant_Report_ProductionOrder_ExportExcel,
                    AppPermissions.Pages_Tenant_Report_ProductionOrder_Calculate,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptPurchase_CanProductionOrder,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueProduction_CanProductionOrder,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_GetProductionOrderForItemIssue,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_GetProductionOrderForItemReceipt,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueProduction_CanProductionOrder,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueProduction,
                    AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptProduction,
                    AppPermissions.Pages_Tenant_Customers_ItemIssues_CanCreateProductionOrder,
                    AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CreateProductionOrder,
                    AppPermissions.Pages_Tenant_Production_Line_Find,
                    AppPermissions.Pages_Tenant_Production_Plan_Find,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_GetListProductionOrderForItemIssue,
                    AppPermissions.Pages_Tenant_Production_ProductionOrder_GetListProductionOrderForItemReceipt,

                });

                return dic;


            }
        }
    }


}
