namespace CorarlERP.Authorization
{
    /// <summary>
    /// Defines string constants for application's permission names.
    /// <see cref="AppAuthorizationProvider"/> for permission definitions.
    /// </summary>
    public static class AppPermissions
    {
        //COMMON PERMISSIONS (FOR BOTH OF TENANTS AND HOST)

        public const string Pages = "Pages";

        public const string Pages_DemoUiComponents = "Pages.DemoUiComponents";
        public const string Pages_Administration = "Pages.Administration";
        public const string Pages_Authrization = "Pages.Authrization";

        public const string Pages_Administration_Roles = "Pages.Administration.Roles";
        public const string Pages_Administration_Roles_Create = "Pages.Administration.Roles.Create";
        public const string Pages_Administration_Roles_Edit = "Pages.Administration.Roles.Edit";
        public const string Pages_Administration_Roles_Delete = "Pages.Administration.Roles.Delete";
        public const string Pages_Administration_Roles_GetList = "Pages.Administration.Roles.GetList";

        public const string Pages_Administration_UserGroups = "Pages.Administration.UserGroups";
        public const string Pages_Administration_UserGroups_Create = "Pages.Administration.UserGroups.Create";
        public const string Pages_Administration_UserGroups_Update = "Pages.Administration.UserGroups.Update";
        public const string Pages_Administration_UserGroups_Delete = "Pages.Administration.UserGroups.Delete";
        public const string Pages_Administration_UserGroups_Enable = "Pages.Administration.UserGroups.Enable";
        public const string Pages_Administration_UserGroups_Disable = "Pages.Administration.UserGroups.Disable";
        public const string Pages_Administration_UserGroups_GetList = "Pages.Administration.UserGroups.GetList";
        public const string Pages_Administration_UserGroups_GetDetail = "Pages.Administration.UserGroups.GetDetail";
        public const string Pages_Administration_UserGroups_Find = "Pages.Administration.Roles.UserGroups.Find";

        public const string Pages_Tenant_Setup_PaymentMethods = "Pages.Tenant.Setup.PaymentMethods";
        public const string Pages_Tenant_Setup_PaymentMethods_Create = "Pages.Tenant.Setup.PaymentMethods.Create";
        public const string Pages_Tenant_Setup_PaymentMethods_Update = "Pages.Tenant.Setup.PaymentMethods.Update";
        public const string Pages_Tenant_Setup_PaymentMethods_Delete = "Pages.Tenant.Setup.PaymentMethods.Delete";
        public const string Pages_Tenant_Setup_PaymentMethods_Enable = "Pages.Tenant.Setup.PaymentMethods.Enable";
        public const string Pages_Tenant_Setup_PaymentMethods_Disable = "Pages.Tenant.Setup.PaymentMethods.Disable";
        public const string Pages_Tenant_Setup_PaymentMethods_GetList = "Pages.Tenant.Setup.PaymentMethods.GetList";
        public const string Pages_Tenant_Setup_PaymentMethods_GetDetail = "Pages.Tenant.Setup.PaymentMethods.GetDetail";
        public const string Pages_Tenant_Setup_PaymentMethods_Find = "Pages.Tenant.Setup.PaymentMethods.Find";



        public const string Pages_Administration_Developer = "Pages.Administration.Developer";
        public const string Pages_Administration_Developer_ApiClients = "Pages.Administration.Developer.ApiClients";
        public const string Pages_Administration_Developer_ApiClients_Create = "Pages.Administration.Developer.ApiClients.Create";
        public const string Pages_Administration_Developer_ApiClients_Delete = "Pages.Administration.Developer.ApiClients.Delete";
        public const string Pages_Administration_Developer_ApiClients_Enable = "Pages.Administration.Developer.ApiClients.Enable";
        public const string Pages_Administration_Developer_ApiClients_Disable = "Pages.Administration.Developer.ApiClients.Disable";
        public const string Pages_Administration_Developer_ApiClients_Update = "Pages.Administration.Developer.ApiClients.Update";
        public const string Pages_Administration_Developer_ApiClients_Detail = "Pages.Administration.Developer.ApiClients.Detail";

        //public const string Pages_Administration_UserActivities = "Pages.Administration.UserActivities";
        //public const string Pages_Administration_UserActivities_FindActivity = "Pages.Administration.UserActivities.FindActivity";
        //public const string Pages_Administration_UserActivities_FindTrasaction = "Pages.Administration.UserActivities.FindTrasaction";
        //public const string Pages_Administration_UserActivities_ExportExcel = "Pages.Administration.UserActivities.ExportExcel";

        public const string Pages_Administration_Users = "Pages.Administration.Users";
        public const string Pages_Administration_Users_GetList = "Pages.Administration.Users.GetList";
        public const string Pages_Administration_Users_Common = "Pages.Administration.Users.Common";
        public const string Pages_Administration_Users_Create = "Pages.Administration.Users.Create";
        public const string Pages_Administration_Users_Edit = "Pages.Administration.Users.Edit";
        public const string Pages_Administration_Users_Delete = "Pages.Administration.Users.Delete";
        public const string Pages_Administration_Users_Deactivate = "Pages.Administration.Users.Deactivate";
        public const string Pages_Administration_Users_ChangePermissions = "Pages.Administration.Users.ChangePermissions";
        public const string Pages_Administration_Users_Impersonation = "Pages.Administration.Users.Impersonation";

        public const string Pages_Administration_Languages = "Pages.Administration.Languages";
        public const string Pages_Administration_Languages_Create = "Pages.Administration.Languages.Create";
        public const string Pages_Administration_Languages_Edit = "Pages.Administration.Languages.Edit";
        public const string Pages_Administration_Languages_Delete = "Pages.Administration.Languages.Delete";
        public const string Pages_Administration_Languages_ChangeTexts = "Pages.Administration.Languages.ChangeTexts";

        public const string Pages_Administration_AuditLogs = "Pages.Administration.AuditLogs";

        public const string Pages_Administration_OrganizationUnits = "Pages.Administration.OrganizationUnits";
        public const string Pages_Administration_OrganizationUnits_ManageOrganizationTree = "Pages.Administration.OrganizationUnits.ManageOrganizationTree";
        public const string Pages_Administration_OrganizationUnits_ManageMembers = "Pages.Administration.OrganizationUnits.ManageMembers";

        public const string Pages_Administration_HangfireDashboard = "Pages.Administration.HangfireDashboard";

        public const string Pages_Administration_UiCustomization = "Pages.Administration.UiCustomization";

        //TENANT-SPECIFIC PERMISSIONS

        public const string Pages_Tenant_Dashboard = "Pages.Tenant.Dashboard";
        public const string Pages_Tenant_App = "Pages.Tenant.App";

        public const string Pages_Administration_Tenant_Settings = "Pages.Administration.Tenant.Settings";

        public const string Pages_Administration_Tenant_SubscriptionManagement = "Pages.Administration.Tenant.SubscriptionManagement";

        #region common name 

        public const string Pages_Tenant_Setup_CustomerTypes_Find = "Pages.Tenant.Setup.CustomerTypes.Find";
        public const string Pages_Tenant_Setup_TransactionTypes_Find = "Pages.Tenant.Setup.TransactionTypes.Find";
        public const string Pages_Tenant_Setup_Classes_Find = "Pages.Tenant.Setup.Classes.Find";

       // public const string Pages_Tenant_Setup_PaymentMethods_Find = "Pages.Tenant.Setup.PaymentMethods.Find";

        public const string Pages_Tenant_ItemType_Find = "Pages.Tenant.ItemType.Find";
        public const string Pages_Tenant_Customer_Cards_Find = "Pages.Tenant.Customer.Cards.Find";
        public const string Pages_Tenant_Customer_Cards_GetCustomerIdByCardId = "Pages.Tenant.Customer.Cards.GetCustomerIdByCardId";
        public const string Pages_Tenant_Setup_Locations_Find = "Pages.Tenant.Setup.Locations.Find";
        public const string Pages_Tenant_Setup_Lots_Find = "Pages.Tenant.Setup.Lots.Find";
        public const string Pages_Tenant_Properties_Find = "Pages.Tenant.Properties.Find";
        public const string Pages_Tenant_PurchaseOrder_Find = "Pages.Tenant.Vendors.PurchaseOrder.Find";
        public const string Pages_Tenant_Vendor_Find = "Pages.Tenant.Vendor.Find";
        public const string Pages_Tenant_PropertyValue_FindValue = "Pages.Tenant.Properties.FindValue";
        public const string Pages_Tenant_Item_Find = "Pages.Tenant.Item.Find";
        public const string Pages_Tenant_Customer_Find = "Pages.Tenant.Customer.Find";
        public const string Pages_Tenant_Common_Inventory = "Pages.Tenant.Common.Inventory";
        public const string Pages_Tenant_Common_Stock_Balance = "Pages.Tenant.Common.Stock.Balance";
        public const string Pages_Tenant_Currencies_Find = "Pages.Tenant.Currencies.Find";
        public const string Pages_Tenant_MultiCurrencies_GetList = "Pages.Tenant.MultiCurrencies.GetList";
        public const string Pages_Tenant_MultiCurrencies_Find = "Pages.Tenant.MultiCurrencies.Find";
        public const string Pages_Tenant_Formats_FindNumber = "Pages.Tenant.Formats.FindNumber";
        public const string Pages_Tenant_Formats_FindDate = "Pages.Tenant.Formats.FindDate";
        public const string Pages_Tenant_Setup_Commons_FindAccountTypes = "Pages.Tenant.Setup.Commons.FindAccountTypes";
        public const string Pages_Tenant_Setup_Commons_FindTaxes = "Pages.Tenant.Setup.Commons.FindTaxes";
        public const string Pages_Tenant_Vendors_Credit_Find = "Pages.Tenant.Vendors.Credit.Find";
        public const string Pages_Tenant_Vendors_Credit_Import = "Pages.Tenant.Vendors.Credit.Import";
        public const string Pages_Tenant_Vendors_Credit_ExportVendorCreditTamplate = "Pages.Tenant.Vendors.Credit.ExportVendorCreditTamplate";

        public const string Pages_Tenant_Accounting_Journals_Find = "Pages.Tenant.Accounting.Journals.Find";
        public const string Pages_Tenant_Setup_Commons_FindAccounts = "Pages.Tenant.Setup.Commons.FindAccounts";
        public const string Pages_Tenant_Vendors_Bills_Find = "Pages.Tenant.Vendors.Bills.Find";
        public const string Pages_Tenant_Customer_SaleOrder_Find = "Pages.Tenant.Customers.SaleOrder.Find";
        public const string Pages_Tenant_Banks_Transfers_Find = "Pages.Tenant.Banks.Transfers.Find";
        public const string Pages_Tenant_Customers_ItemIssues_Find = "Pages.Tenant.Customers.ItemIssues.Find";
        public const string Pages_Tenant_Vendors_PayBills_Find = "Pages.Tenant.Vendors.PayBills.Find";
        public const string Pages_Tenant_Vendors_ItemReceipts_Find = "Pages.Tenant.Vendors.ItemReceipts.Find";
        public const string Pages_Tenant_Customer_Invoice_Find = "Pages.Tenant.Customers.Invoices.Find";
        public const string Pages_Tenant_Customers_Credit_Find = "Pages.Tenant.Customers.Credit.Find";
        public const string Pages_Tenant_Customers_Credit_Import = "Pages.Tenant.Customers.Credit.Import";

        public const string Pages_Tenant_Customers_Credit_ExportCustomerCreditTamplate = "Pages.Tenant.Customers.Credit.ExportCustomerCreditTamplate";

        public const string Pages_Tenant_Inventory_PhysicalCount_Find = "Pages.Tenant.Inventory.PhysicalCount.Find";
        public const string Pages_Tenant_Inventory_TransferOrder_Find = "Pages.Tenant.Inventory.TransferOrder.Find";
        public const string Pages_Tenant_Customers_ReceivePayments_Find = "Pages.Tenant.Customers.ReceivePayments.Find";
        public const string pages_Tenant_Inventory_TransferOrder_AutoInventory = "pages.Tenant.Inventory.TransferOrder.AutoInventory";

        public const string Pages_Tenant_Inventory_TransferOrder_ForItemIssueTransfer = "Pages.Tenant.Inventory.TransferOrder.ForItemIssueTransfer";
        public const string Pages_Tenant_Inventory_TransferOrder_FindItemReceiptTrasfer = "Pages.Tenant.Inventory.TransferOrder.FindItemReceiptTrasfer";

        public const string Pages_Tenant_Common_DeleteVoid = "Pages.Tenant.Common.DeleteVoid";
        public const string Pages_Tenant_Common_BatchNo_Find = "Pages.Tenant.Common.BatchNo.Find";
        public const string Pages_Tenant_Common_Tenant_Find = "Pages.Tenant.Common.Tenant.Find";

        public const string Pages_Tenant_Report_CashFlowTemplate_Find = "Pages.Tenant.Report.CashFlowTemplate.Find";

        public const string Pages_Tenant_Common_Inventory_JournalTransactionTypes_Find = "Pages.Tenant.Common.Inventory.JournalTransactionTypes.Find";

        public const string Pages_Tenant_PurchasePrices = "Pages.Tenant.PurchasePrices";
        public const string Pages_Tenant_PurchasePrices_Delete = "Pages.Tenant.PurchasePrices.Delete";
        public const string Pages_Tenant_PurchasePrices_GetDetail = "Pages.Tenant.PurchasePrices.GetDetail";
        public const string Pages_Tenant_PurchasePrices_GetList = "Pages.Tenant.PurchasePrices.GetList";
        public const string Pages_Tenant_PurchasePrices_Update = "Pages.Tenant.PurchasePrices.Update";
        public const string Pages_Tenant_PurchasePrices_Create = "Pages.Tenant.PurchasePrices.Create";
        public const string Pages_Tenant_PurchasePrices_Find = "Pages.Tenant.PurchasePrices.Find";
        public const string Pages_Tenant_PurchasePrices_Export = "Pages.Tenant.PurchasePrices.Export";

        #endregion

        #region Setups
        //Class and location
        public const string Pages_Tenant_Setup_Classes = "Pages.Tenant.Setup.Classes";
        public const string Pages_Tenant_Setup_Classes_Delete = "Pages.Tenant.Setup.Classes.Delete";
        public const string Pages_Tenant_Setup_Classes_Create = "Pages.Tenant.Setup.Classes.Create";
        public const string Pages_Tenant_Setup_Classes_Update = "Pages.Tenant.Setup.Classes.Update";
        public const string Pages_Tenant_Setup_Classes_Disable = "Pages.Tenant.Setup.Classes.Disable";
        public const string Pages_Tenant_Setup_Classes_Enable = "Pages.Tenant.Setup.Classes.Enable";
        public const string Pages_Tenant_Setup_Classes_GetList = "Pages.Tenant.Setup.Classes.GetList";
        public const string Pages_Tenant_Setup_Classes_GetDetail = "Pages.Tenant.Setup.Classes.GetDetail";
     

        //public const string Pages_Tenant_Setup_PaymentMethods = "Pages.Tenant.Setup.PaymentMethods";
        //public const string Pages_Tenant_Setup_PaymentMethods_Delete = "Pages.Tenant.Setup.PaymentMethods.Delete";
        //public const string Pages_Tenant_Setup_PaymentMethods_Create = "Pages.Tenant.Setup.PaymentMethods.Create";
        //public const string Pages_Tenant_Setup_PaymentMethods_Update = "Pages.Tenant.Setup.PaymentMethods.Update";
        //public const string Pages_Tenant_Setup_PaymentMethods_Disable = "Pages.Tenant.Setup.PaymentMethods.Disable";
        //public const string Pages_Tenant_Setup_PaymentMethods_Enable = "Pages.Tenant.Setup.PaymentMethods.Enable";
        //public const string Pages_Tenant_Setup_PaymentMethods_GetList = "Pages.Tenant.Setup.PaymentMethods.GetList";
        //public const string Pages_Tenant_Setup_PaymentMethods_GetDetail = "Pages.Tenant.Setup.PaymentMethods.GetDetail";



        public const string Pages_Tenant_Setup_CustomerTypes = "Pages.Tenant.Setup.CustomerTypes";
        public const string Pages_Tenant_Setup_CustomerTypes_Delete = "Pages.Tenant.Setup.CustomerTypes.Delete";
        public const string Pages_Tenant_Setup_CustomerTypes_Create = "Pages.Tenant.Setup.CustomerTypes.Create";
        public const string Pages_Tenant_Setup_CustomerTypes_Update = "Pages.Tenant.Setup.CustomerTypes.Update";
        public const string Pages_Tenant_Setup_CustomerTypes_Disable = "Pages.Tenant.Setup.CustomerTypes.Disable";
        public const string Pages_Tenant_Setup_CustomerTypes_Enable = "Pages.Tenant.Setup.CustomerTypes.Enable";
        public const string Pages_Tenant_Setup_CustomerTypes_GetList = "Pages.Tenant.Setup.CustomerTypes.GetList";
        public const string Pages_Tenant_Setup_CustomerTypes_GetDetail = "Pages.Tenant.Setup.CustomerTypes.GetDetail";

        public const string Pages_Tenant_Setup_VendorTypes = "Pages.Tenant.Setup.VendorTypes";
        public const string Pages_Tenant_Setup_VendorTypes_Find = "Pages.Tenant.Setup.VendorTypes.Find";
        public const string Pages_Tenant_Setup_VendorTypes_Delete = "Pages.Tenant.Setup.VendorTypes.Delete";
        public const string Pages_Tenant_Setup_VendorTypes_Create = "Pages.Tenant.Setup.VendorTypes.Create";
        public const string Pages_Tenant_Setup_VendorTypes_Update = "Pages.Tenant.Setup.VendorTypes.Update";
        public const string Pages_Tenant_Setup_VendorTypes_Disable = "Pages.Tenant.Setup.VendorTypes.Disable";
        public const string Pages_Tenant_Setup_VendorTypes_Enable = "Pages.Tenant.Setup.VendorTypes.Enable";
        public const string Pages_Tenant_Setup_VendorTypes_GetList = "Pages.Tenant.Setup.VendorTypes.GetList";
        public const string Pages_Tenant_Setup_VendorTypes_GetDetail = "Pages.Tenant.Setup.VendorTypes.GetDetail";



        public const string Pages_Tenant_Setup_TransactionTypes = "Pages.Tenant.Setup.TransactionTypes";
        public const string Pages_Tenant_Setup_TransactionTypes_Delete = "Pages.Tenant.Setup.TransactionTypes.Delete";
        public const string Pages_Tenant_Setup_TransactionTypes_Create = "Pages.Tenant.Setup.TransactionTypes.Create";
        public const string Pages_Tenant_Setup_TransactionTypes_Update = "Pages.Tenant.Setup.TransactionTypes.Update";
        public const string Pages_Tenant_Setup_TransactionTypes_Disable = "Pages.Tenant.Setup.TransactionTypes.Disable";
        public const string Pages_Tenant_Setup_TransactionTypes_Enable = "Pages.Tenant.Setup.TransactionTypes.Enable";
        public const string Pages_Tenant_Setup_TransactionTypes_GetList = "Pages.Tenant.Setup.TransactionTypes.GetList";
        public const string Pages_Tenant_Setup_TransactionTypes_GetDetail = "Pages.Tenant.Setup.TransactionTypes.GetDetail";

        public const string Pages_Tenant_Setup_Locations = "Pages.Tenant.Setup.Locations";
        public const string Pages_Tenant_Setup_Locations_Delete = "Pages.Tenant.Setup.Locations.Delete";
        public const string Pages_Tenant_Setup_Locations_Create = "Pages.Tenant.Setup.Locations.Create";
        public const string Pages_Tenant_Setup_Locations_Update = "Pages.Tenant.Setup.Locations.Update";
        public const string Pages_Tenant_Setup_Locations_Disable = "Pages.Tenant.Setup.Locations.Disable";
        public const string Pages_Tenant_Setup_Locations_Enable = "Pages.Tenant.Setup.Locations.Enable";
        public const string Pages_Tenant_Setup_Locations_GetList = "Pages.Tenant.Setup.Locations.GetList";
        public const string Pages_Tenant_Setup_Locations_GetDetail = "Pages.Tenant.Setup.Locations.GetDetail";

        public const string Pages_Tenant_Setup_Lots = "Pages.Tenant.Setup.Lots";
        public const string Pages_Tenant_Setup_Lots_Delete = "Pages.Tenant.Setup.Lots.Delete";
        public const string Pages_Tenant_Setup_Lots_Create = "Pages.Tenant.Setup.Lots.Create";
        public const string Pages_Tenant_Setup_Lots_Import = "Pages.Tenant.Setup.Lots.ImportExcel";
        public const string Pages_Tenant_Setup_Lots_Export = "Pages.Tenant.Setup.Lots.ExportExcel";
        public const string Pages_Tenant_Setup_Lots_Update = "Pages.Tenant.Setup.Lots.Update";
        public const string Pages_Tenant_Setup_Lots_Disable = "Pages.Tenant.Setup.Lots.Disable";
        public const string Pages_Tenant_Setup_Lots_Enable = "Pages.Tenant.Setup.Lots.Enable";
        public const string Pages_Tenant_Setup_Lots_GetList = "Pages.Tenant.Setup.Lots.GetList";
        public const string Pages_Tenant_Setup_Lots_GetDetail = "Pages.Tenant.Setup.Lots.GetDetail";

        // Property and PropertyValue
        public const string Pages_Tenant_Properties = "Pages.Tenant.Properties";
        public const string Pages_Tenant_Properties_Property_Value = "Pages.Tenant.Properties.Property.Value";
        public const string Pages_Tenant_Properties_Create = "Pages.Tenant.Properties.Create";
        public const string Pages_Tenant_Properties_Import_Excel = "Pages.Tenant.Properties.Import.Excel";
        public const string Pages_Tenant_Properties_Export_Excel = "Pages.Tenant.Properties.Export.Excel";
        public const string Pages_Tenant_Properties_Update = "Pages.Tenant.Properties.Update";
        public const string Pages_Tenant_Properties_Delete = "Pages.Tenant.Properties.Delete";
        public const string Pages_Tenant_Properties_Enable = "Pages.Tenant.Properties.Enable";
        public const string Pages_Tenant_Properties_GetList = "Pages.Tenant.Properties.GetList";
        public const string Pages_Tenant_Properties_Disable = "Pages.Tenant.Properties.Disable";

        public const string Pages_Tenant_PropertyValue_Delete = "Pages.Tenant.Properties.DeleteValue";
        public const string Pages_Tenant_PropertyValue_AddValue = "Pages.Tenant.Properties.AddValue";
        public const string Pages_Tenant_PropertyValue_EditValue = "Pages.Tenant.Properties.EditValue";
        public const string Pages_Tenant_PropertyValue_GetValue = "Pages.Tenant.Properties.GetValue";

        public const string Pages_Tenant_Items = "Pages.Tenant.Items";

        public const string Pages_Tenant_Items_Common = "Pages.Tenant.Items.Common";
        public const string Pages_Tenant_Item_Delete = "Pages.Tenant.Item.Delete";
        public const string Pages_Tenant_Item_Disable = "Pages.Tenant.Item.Disable";
        public const string Pages_Tenant_Item_Enable = "Pages.Tenant.Item.Enable";
        public const string Pages_Tenant_Item_GetDetail = "Pages.Tenant.Item.GetDetail";
        public const string Pages_Tenant_Item_GetList = "Pages.Tenant.Item.GetList";
        public const string Pages_Tenant_Item_Update = "Pages.Tenant.Item.Update";
        public const string Pages_Tenant_Item_Create = "Pages.Tenant.Item.Create";
        public const string Pages_Tenant_SubItem_Delete = "Pages.Tenant.ItemDelete.Create";
        public const string Pages_Tenant_Item_Export = "Pages.Tenant.Item.Export";
        public const string Pages_Tenant_Item_Import = "Pages.Tenant.Item.Import";
        public const string Pages_Tenant_Item_Print_Barcode = "Pages.Tenant.Item.Print.Barcode";
        public const string Pages_Tenant_Item_CanFilterAccount = "Pages.Tenant.Item.AccountFilter";
        public const string Pages_Tenant_Item_CanSeePrice = "Pages.Tenant.Item.CanSeePrice";
        public const string Pages_Tenant_Item_ExportPdf = "Pages.Tenant.Item.ExportPdf";


        // BOM

        public const string Pages_Tenant_BOMs = "Pages.Tenant.BOMs";
        public const string Pages_Tenant_BOMs_Delete = "Pages.Tenant.BOMs.Delete";
        public const string Pages_Tenant_BOMs_Disable = "Pages.Tenant.BOMs.Disable";
        public const string Pages_Tenant_BOMs_Enable = "Pages.Tenant.BOMs.Enable";
        public const string Pages_Tenant_BOMs_GetDetail = "Pages.Tenant.BOMs.GetDetail";
        public const string Pages_Tenant_BOMs_GetList = "Pages.Tenant.BOMs.GetList";
        public const string Pages_Tenant_BOMs_Update = "Pages.Tenant.BOMs.Update";
        public const string Pages_Tenant_BOMs_Create = "Pages.Tenant.BOMs.Create";
        public const string Pages_Tenant_BOMs_Find = "Pages.Tenant.BOMs.Find";
        public const string Pages_Tenant_BOMs_Export = "Pages.Tenant.BOMs.Export";
        public const string Pages_Tenant_BOMs_Import = "Pages.Tenant.BOMs.Import";

        public const string Pages_Tenant_ItemPrices = "Pages.Tenant.ItemPrices";
        public const string Pages_Tenant_ItemPrices_Delete = "Pages.Tenant.ItemPrices.Delete";
        public const string Pages_Tenant_ItemPrices_GetDetail = "Pages.Tenant.ItemPrices.GetDetail";
        public const string Pages_Tenant_ItemPrices_GetList = "Pages.Tenant.ItemPrices.GetList";
        public const string Pages_Tenant_ItemPrices_Update = "Pages.Tenant.ItemPrices.Update";
        public const string Pages_Tenant_ItemPrices_Create = "Pages.Tenant.ItemPrices.Create";
        public const string Pages_Tenant_ItemPrices_Find = "Pages.Tenant.ItemPrices.Find";
        public const string Pages_Tenant_ItemPrices_Export = "Pages.Tenant.ItemPrices.Export";


        public const string Pages_Tenant_Exchanges = "Pages.Tenant.Exchanges";
        public const string Pages_Tenant_Exchanges_Delete = "Pages.Tenant.Exchanges.Delete";
        public const string Pages_Tenant_Exchanges_GetDetail = "Pages.Tenant.Exchanges.GetDetail";
        public const string Pages_Tenant_Exchanges_GetList = "Pages.Tenant.Exchanges.GetList";
        public const string Pages_Tenant_Exchanges_Update = "Pages.Tenant.Exchanges.Update";
        public const string Pages_Tenant_Exchanges_Create = "Pages.Tenant.Exchanges.Create";
        public const string Pages_Tenant_Exchanges_Find = "Pages.Tenant.Exchanges.Find";
        public const string Pages_Tenant_Exchanges_GetExChangeCurrency = "Pages.Tenant.Exchanges.GetExChangeCurrency";
        public const string Pages_Tenant_Exchanges_ApplyRate = "Pages.Tenant.Exchanges.ApplyRate";


        public const string Pages_Tenant_ItemCodeFormulas = "Pages.Tenant.ItemCodeFormulas";
        public const string Pages_Tenant_ItemCodeFormulas_Delete = "Pages.Tenant.ItemCodeFormulas.Delete";
        public const string Pages_Tenant_ItemCodeFormulas_GetDetail = "Pages.Tenant.ItemCodeFormulas.GetDetail";
        public const string Pages_Tenant_ItemCodeFormulas_GetList = "Pages.Tenant.ItemCodeFormulas.GetList";
        public const string Pages_Tenant_ItemCodeFormulas_Update = "Pages.Tenant.ItemCodeFormulas.Update";
        public const string Pages_Tenant_ItemCodeFormulas_Create = "Pages.Tenant.ItemCodeFormulas.Create";
        public const string Pages_Tenant_ItemCodeFormulas_Find = "Pages.Tenant.ItemCodeFormulas.Find";
        public const string Pages_Tenant_ItemCodeFormulas_Enable = "Pages.Tenant.ItemCodeFormulas.Enable";
        public const string Pages_Tenant_ItemCodeFormulas_Disable = "Pages.Tenant.ItemCodeFormulas.Disable";

        public const string Pages_Tenant_ItemCodeFormulas_UpdateAutoItemCode = "Pages.Tenant.ItemCodeFormulas.UpdateAutoItemCode";
        public const string Pages_Tenant_ItemCodeFormulas_UpdateItemCodeSetting = "Pages.Tenant.ItemCodeFormulas.UpdateItemCodeSetting";

        public const string Pages_Tenant_Vendor = "Pages.Tenant.Vendor";
        public const string Pages_Tenant_Vendor_Delete = "Pages.Tenant.Vendor.Delete";
        public const string Pages_Tenant_Vendor_GetList = "Pages.Tenant.Vendor.GetList";
        public const string Pages_Tenant_Vendor_Update = "Pages.Tenant.Vendor.Update";
        public const string Pages_Tenant_Vendor_GetDetail = "Pages.Tenant.Vendor.GetDetail";
        public const string Pages_Tenant_Vendor_Disable = "Pages.Tenant.Vendor.Disable";
        public const string Pages_Tenant_Vendor_Enable = "Pages.Tenant.Vendor.Enable";
        public const string Pages_Tenant_Vendor_Create = "Pages.Tenant.Vendor.Create";

        public const string Pages_Tenant_Vendor_ExportExcel = "Pages.Tenant.Vendor.ExportExcel";
        public const string Pages_Tenant_Vendor_ImportExcel = "Pages.Tenant.Vendor.ImportExcel";


        public const string Pages_Tenant_SetUp_Vendor = "Pages.Tenant.Vendor.SetUp";
        public const string Pages_Tenant_SetUp_Vendor_Delete = "Pages.Tenant.Vendor.SetUp.Delete";
        public const string Pages_Tenant_SetUp_Vendor_GetList = "Pages.Tenant.Vendor.SetUp.GetList";
        public const string Pages_Tenant_SetUp_Vendor_Update = "Pages.Tenant.Vendor.SetUp.Update";
        public const string Pages_Tenant_SetUp_Vendor_GetDetail = "Pages.Tenant.Vendor.SetUp.GetDetail";
        public const string Pages_Tenant_SetUp_Vendor_Disable = "Pages.Tenant.Vendor.SetUp.Disable";
        public const string Pages_Tenant_SetUp_Vendor_Enable = "Pages.Tenant.Vendor.SetUp.Enable";
        public const string Pages_Tenant_SetUp_Vendor_Create = "Pages.Tenant.Vendor.SetUp.Create";
        public const string Pages_Tenant_SetUp_Vendor_ExportExcel = "Pages.Tenant.Vendor.SetUp.ExportExcel";
        public const string Pages_Tenant_SetUp_Vendor_ImportExcel = "Pages.Tenant.Vendor.SetUp.ImportExcel";


        public const string Pages_Tenant_Customer = "Pages.Tenant.Customer";
        public const string Pages_Tenant_Customer_Delete = "Pages.Tenant.Customer.Delete";     
        public const string Pages_Tenant_Customer_GetList = "Pages.Tenant.Customer.GetList";
        public const string Pages_Tenant_Customer_Update = "Pages.Tenant.Customer.Update";
        public const string Pages_Tenant_Customer_GetDetail = "Pages.Tenant.Customer.GetDetail";
        public const string Pages_Tenant_Customer_Disable = "Pages.Tenant.Customer.Disable";
        public const string Pages_Tenant_Customer_Enable = "Pages.Tenant.Customer.Enable";
        public const string Pages_Tenant_Customer_Create = "Pages.Tenant.Customer.Create";


        public const string Pages_Tenant_SetUp_Customer = "Pages.Tenant.Customer.SetUp";
        public const string Pages_Tenant_SetUp_Customer_Delete = "Pages.Tenant.Customer.SetUp.Delete";
        public const string Pages_Tenant_SetUp_Customer_GetList = "Pages.Tenant.Customer.SetUp.GetList";
        public const string Pages_Tenant_SetUp_Customer_Update = "Pages.Tenant.Customer.SetUp.Update";
        public const string Pages_Tenant_SetUp_Customer_GetDetail = "Pages.Tenant.Customer.SetUp.GetDetail";
        public const string Pages_Tenant_SetUp_Customer_Disable = "Pages.Tenant.Customer.SetUp.Disable";
        public const string Pages_Tenant_SetUp_Customer_Enable = "Pages.Tenant.Customer.SetUp.Enable";
        public const string Pages_Tenant_SetUp_Customer_Create = "Pages.Tenant.Customer.SetUp.Create";


        public const string Pages_Tenant_Customer_Credit_POS_Find = "Pages.Tenant.Customer.Credit.POS.Find";
        public const string Pages_Tenant_Customer_GetListPOS = "Pages.Tenant.Customer.GetListPOS";
        public const string Pages_Tenant_Customer_DeletePOS = "Pages.Tenant.Customer.DeletePOS";
        public const string Pages_Tenant_Customer_GetDetailPOS = "Pages.Tenant.Customer.GetDetailPOS";
        public const string Pages_Tenant_Customer_DisablePOS = "Pages.Tenant.Customer.DisablePOS";
        public const string Pages_Tenant_Customer_EnablePOS = "Pages.Tenant.Customer.EnablePOS";
        public const string Pages_Tenant_Customer_CreatePOS = "Pages.Tenant.Customer.CreatePOS";
        public const string Pages_Tenant_Customer_UpdatePOS = "Pages.Tenant.Customer.UpdatePOS";
        public const string Pages_Tenant_Set_Customer_POS = "Pages_Tenant_Set_Customer_POS";

        public const string Pages_Tenant_Customers_Invoice_SaleReturn = "Pages.Tenant.Customers.Invoice.SaleReturn";
        public const string Pages_Tenant_Customers_Invoice_CreateSaleReturn = "Pages.Tenant.Customers.Invoice.CreateSaleReturn";
        public const string Pages_Tenant_Customers_Invoice_ListSaleReturn = "Pages.Tenant.Customers.Invoice.ListSaleReturn";

        public const string Pages_Tenant_Customers_Invoice_ListSaleReturn_Unpaid = "Pages.Tenant.Customers.Invoice.ListSaleReturn.Unpaid";

        public const string Pages_Tenant_Customers_Invoice_VoidSaleReturn = "Pages.Tenant.Customers.Invoice.VoidSaleReturn";
        public const string Pages_Tenant_Customers_Invoice_UpdateSaleReturn = "Pages.Tenant.Customers.Invoice.UpdateSaleReturn";
        public const string Pages_Tenant_Customers_Invoice_DeleteVoideSaleReturn = "Pages.Tenant.Customers.Invoice.DeleteVoideSaleReturn";
        public const string Pages_Tenant_Customers_Invoice_ViewSaleReturn = "Pages.Tenant.Customers.Invoice.ViewSaleReturn";

        public const string Pages_Tenant_Customer_ExportExcel = "Pages.Tenant.Customer.ExportExcel";
        public const string Pages_Tenant_Customer_ImportExcel = "Pages.Tenant.Customer.ImportExcel";

        public const string Pages_Tenant_SetUp_Customer_ExportExcel = "Pages.Tenant.Customer.SetUp.ExportExcel";
        public const string Pages_Tenant_SetUp_Customer_ImportExcel = "Pages.Tenant.Customer.SetUp.ImportExcel";

        public const string Pages_Host_Client_Currencies_Sync = "Pages.Host.Client.Currencies.Sync";
        public const string Pages_Host_Currencies_GetList = "Pages.Host.Client.Currencies.GetList";

        public const string Pages_Host_Client_PaymentMethods = "Pages.Host.Client.PaymentMethods";
        public const string Pages_Host_Client_PaymentMethods_Create = "Pages.Host.Client.PaymentMethods.Create";
        public const string Pages_Host_Client_PaymentMethods_Update = "Pages.Host.Client.PaymentMethods.Update";
        public const string Pages_Host_Client_PaymentMethods_Delete = "Pages.Host.Client.PaymentMethods.Delete";
        public const string Pages_Host_Client_PaymentMethods_Enable = "Pages.Host.Client.PaymentMethods.Enable";
        public const string Pages_Host_Client_PaymentMethods_Disable = "Pages.Host.Client.PaymentMethods.Disable";
        public const string Pages_Host_Client_PaymentMethods_GetList = "Pages.Host.Client.PaymentMethods.GetList";
        public const string Pages_Host_Client_PaymentMethods_GetDetail = "Pages.Host.Client.PaymentMethods.GetDetail";

        public const string Pages_Host_Client_PackagePromotions = "Pages.Host.Client.PackagePromotions";
        public const string Pages_Host_Client_PackagePromotions_Create = "Pages.Host.Client.PackagePromotions.Create";
        public const string Pages_Host_Client_PackagePromotions_Update = "Pages.Host.Client.PackagePromotions.Update";
        public const string Pages_Host_Client_PackagePromotions_Delete = "Pages.Host.Client.PackagePromotions.Delete";
        public const string Pages_Host_Client_PackagePromotions_Enable = "Pages.Host.Client.PackagePromotions.Enable";
        public const string Pages_Host_Client_PackagePromotions_Disable = "Pages.Host.Client.PackagePromotions.Disable";
        public const string Pages_Host_Client_PackagePromotions_GetList = "Pages.Host.Client.PackagePromotions.GetList";
        public const string Pages_Host_Client_PackagePromotions_GetDetail = "Pages.Host.Client.PackagePromotions.GetDetail";
        public const string Pages_Host_Client_PackagePromotions_Find = "Pages.Host.Client.PackagePromotions.Find";

        public const string Pages_Host_Client_Packages = "Pages.Host.Client.Packages";
        public const string Pages_Host_Client_Packages_Create = "Pages.Host.Client.Packages.Create";
        public const string Pages_Host_Client_Packages_Update = "Pages.Host.Client.Packages.Update";
        public const string Pages_Host_Client_Packages_Delete = "Pages.Host.Client.Packages.Delete";
        public const string Pages_Host_Client_Packages_Enable = "Pages.Host.Client.Packages.Enable";
        public const string Pages_Host_Client_Packages_Disable = "Pages.Host.Client.Packages.Disable";
        public const string Pages_Host_Client_Packages_GetList = "Pages.Host.Client.Packages.GetList";
        public const string Pages_Host_Client_Packages_GetDetail = "Pages.Host.Client.Packages.GetDetail";
        public const string Pages_Host_Client_Packages_Find = "Pages.Host.Client.Packages.Find";


        public const string Pages_Host_Client_Formats_Sync = "Pages.Host.Client.Formats.Sync";
        public const string Pages_Host_Formats_GetList = "Pages.Host.Client.Formats.GetList";

        public const string Pages_Host_Client_ItemType_Sync = "Pages.Host.Client.ItemType.Sync";
        public const string Pages_Host_ItemType_GetList = "Pages.Host.Client.ItemType.GetList";


        public const string Pages_Tenant_Commons = "Pages.Tenant.Commons";

        public const string Pages_Tenant_Setup = "Pages.Tenant.Setup";

        public const string Pages_Tenant_Setup_Taxes = "Pages.Tenant.Setup.Taxes";
        public const string Pages_Tenant_Setup_Taxes_Create = "Pages.Tenant.Setup.Taxes.Create";
        public const string Pages_Tenant_Setup_Taxes_Delete = "Pages.Tenant.Setup.Taxes.Delete";
        public const string Pages_Tenant_Setup_Taxes_Edit = "Pages.Tenant.Setup.Taxes.Edit";
        public const string Pages_Tenant_Setup_Taxes_Enable = "Pages.Tenant.Setup.Taxes.Enable";
        public const string Pages_Tenant_Setup_Taxes_Disable = "Pages.Tenant.Setup.Taxes.Disable";

        public const string Pages_Tenant_Setup_ChartOfAccounts = "Pages.Tenant.Setup.ChartOfAccounts";
        public const string Pages_Tenant_Setup_ChartOfAccounts_Create = "Pages.Tenant.Setup.ChartOfAccounts.Create";
        public const string Pages_Tenant_Setup_ChartOfAccounts_Detail = "Pages.Tenant.Setup.ChartOfAccounts.Detail";
        public const string Pages_Tenant_Setup_ChartOfAccounts_Delete = "Pages.Tenant.Setup.ChartOfAccounts.Delete";
        public const string Pages_Tenant_Setup_ChartOfAccounts_Edit = "Pages.Tenant.Setup.ChartOfAccounts.Edit";
        public const string Pages_Tenant_Setup_ChartOfAccounts_Enable = "Pages.Tenant.Setup.ChartOfAccounts.Enable";
        public const string Pages_Tenant_Setup_ChartOfAccounts_Disable = "Pages.Tenant.Setup.ChartOfAccounts.Disable";
        public const string Pages_Tenant_Setup_ChartOfAccounts_ImportExcel = "Pages.Tenant.Setup.ChartOfAccounts.ImportExcel";
        public const string Pages_Tenant_Setup_ChartOfAccounts_ExportExcel = "Pages.Tenant.Setup.ChartOfAccounts.ExportExcel";
        public const string Pages_Tenant_Setup_ChartOfAccounts_ExportPdf = "Pages.Tenant.Setup.ChartOfAccounts.ExportPdf";

        public const string Pages_Tenant_Setup_TestParameters = "Pages.Tenant.Setup.TestParameters";
        public const string Pages_Tenant_Setup_TestParameters_Delete = "Pages.Tenant.Setup.TestParameters.Delete";
        public const string Pages_Tenant_Setup_TestParameters_Create = "Pages.Tenant.Setup.TestParameters.Create";
        public const string Pages_Tenant_Setup_TestParameters_Update = "Pages.Tenant.Setup.TestParameters.Update";
        public const string Pages_Tenant_Setup_TestParameters_Disable = "Pages.Tenant.Setup.TestParameters.Disable";
        public const string Pages_Tenant_Setup_TestParameters_Enable = "Pages.Tenant.Setup.TestParameters.Enable";
        public const string Pages_Tenant_Setup_TestParameters_GetList = "Pages.Tenant.Setup.TestParameters.GetList";
        public const string Pages_Tenant_Setup_TestParameters_GetDetail = "Pages.Tenant.Setup.TestParameters.GetDetail";
        public const string Pages_Tenant_Setup_TestParameters_Find = "Pages.Tenant.Setup.TestParameters.Find";

        public const string Pages_Tenant_Setup_QCTestTemplates = "Pages.Tenant.Setup.QCTestTemplates";
        public const string Pages_Tenant_Setup_QCTestTemplates_Delete = "Pages.Tenant.Setup.QCTestTemplates.Delete";
        public const string Pages_Tenant_Setup_QCTestTemplates_Create = "Pages.Tenant.Setup.QCTestTemplates.Create";
        public const string Pages_Tenant_Setup_QCTestTemplates_Update = "Pages.Tenant.Setup.QCTestTemplates.Update";
        public const string Pages_Tenant_Setup_QCTestTemplates_Disable = "Pages.Tenant.Setup.QCTestTemplates.Disable";
        public const string Pages_Tenant_Setup_QCTestTemplates_Enable = "Pages.Tenant.Setup.QCTestTemplates.Enable";
        public const string Pages_Tenant_Setup_QCTestTemplates_GetList = "Pages.Tenant.Setup.QCTestTemplates.GetList";
        public const string Pages_Tenant_Setup_QCTestTemplates_GetDetail = "Pages.Tenant.Setup.QCTestTemplates.GetDetail";
        public const string Pages_Tenant_Setup_QCTestTemplates_Find = "Pages.Tenant.Setup.QCTestTemplates.Find";


        #endregion

        #region QC Test 
        public const string Pages_Tenant_QCTests = "Pages.Tenant.QCTests";

        public const string Pages_Tenant_QCSamples = "Pages.Tenant.QCSamples";
        public const string Pages_Tenant_QCSamples_Delete = "Pages.Tenant.QCSamples.Delete";
        public const string Pages_Tenant_QCSamples_Create = "Pages.Tenant.QCSamples.Create";
        public const string Pages_Tenant_QCSamples_Update = "Pages.Tenant.QCSamples.Update";
        public const string Pages_Tenant_QCSamples_GetList = "Pages.Tenant.QCSamples.GetList";
        public const string Pages_Tenant_QCSamples_GetDetail = "Pages.Tenant.QCSamples.GetDetail";
        public const string Pages_Tenant_QCSamples_Find = "Pages.Tenant.QCSamples.Find";

        public const string Pages_Tenant_LabTestRequests = "Pages.Tenant.LabTestRequests";
        public const string Pages_Tenant_LabTestRequests_Delete = "Pages.Tenant.LabTestRequests.Delete";
        public const string Pages_Tenant_LabTestRequests_Create = "Pages.Tenant.LabTestRequests.Create";
        public const string Pages_Tenant_LabTestRequests_Update = "Pages.Tenant.LabTestRequests.Update";
        public const string Pages_Tenant_LabTestRequests_GetList = "Pages.Tenant.LabTestRequests.GetList";
        public const string Pages_Tenant_LabTestRequests_GetDetail = "Pages.Tenant.LabTestRequests.GetDetail";
        public const string Pages_Tenant_LabTestRequests_ChangeStatus = "Pages.Tenant.LabTestRequests.ChangeStatus";
        public const string Pages_Tenant_LabTestRequests_Find = "Pages.Tenant.LabTestRequests.Find";

        public const string Pages_Tenant_LabTestResults = "Pages.Tenant.LabTestResults";
        public const string Pages_Tenant_LabTestResults_Delete = "Pages.Tenant.LabTestResults.Delete";
        public const string Pages_Tenant_LabTestResults_Create = "Pages.Tenant.LabTestResults.Create";
        public const string Pages_Tenant_LabTestResults_Update = "Pages.Tenant.LabTestResults.Update";
        public const string Pages_Tenant_LabTestResults_GetList = "Pages.Tenant.LabTestResults.GetList";
        public const string Pages_Tenant_LabTestResults_GetDetail = "Pages.Tenant.LabTestResults.GetDetail";
        #endregion


        // Start permission for accounting 
        #region Accounting
        public const string Pages_Tenant_Accounting = "Pages.Tenant.Accounting";
        public const string Pages_Tenant_Accounting_Journals = "Pages.Tenant.Accounting.Journals";
        public const string Pages_Tenant_Accounting_Journals_Delete = "Pages.Tenant.Accounting.Journals.Delete";
        public const string Pages_Tenant_Accounting_Journals_Disable = "Pages.Tenant.Accounting.Journals.Disable";
        public const string Pages_Tenant_Accounting_Journals_Enable = "Pages.Tenant.Accounting.Journals.Enable";
        public const string Pages_Tenant_Accounting_Journals_GetDetail = "Pages.Tenant.Accounting.Journals.GetDetail";
        public const string Pages_Tenant_Accounting_Journals_GetList = "Pages.Tenant.Accounting.Journals.GetList";
        public const string Pages_Tenant_Accounting_Journals_Update = "Pages.Tenant.Accounting.Journals.Update";
        public const string Pages_Tenant_Accounting_Journals_Create = "Pages.Tenant.Accounting.Journals.Create";
      
        public const string Pages_Tenant_Accounting_Locks_CreateOrUpdate = "Pages.Tenant.Accounting.Locks.CreateOrUpdate";
        public const string Pages_Tenant_Accounting_Locks_GetList = "Pages.Tenant.Accounting.Locks.GetList";
        public const string Pages_Tenant_Accounting_Locks = "Pages_Tenant_Accounting_Locks_GetList";
        public const string Pages_Tenant_Accounting_Locks_GeneratePasswork = "Pages.Tenant.Accounting.Locks.GeneratePasswork";
        public const string Pages_Tenant_Accounting_Locks_Find = "Pages.Tenant.Accounting.Locks.Find";

        public const string Pages_Tenant_Accounting_Journals_UpdateStatusToVoid = "Pages.Tenant.Accounting.Journals.UpdateStatusToVoid";
        public const string Pages_Tenant_Accounting_Journals_UpdateStatusToDraft = "Pages.Tenant.Accounting.Journals.UpdateStatusToDraft";
        public const string Pages_Tenant_Accounting_Journals_UpdateStatusToPublish = "Pages.Tenant.Accounting.Journals.UpdateStatusToPublish";


        public const string Pages_Tenant_Accounting_Journals_ImportExcel = "Pages.Tenant.Accounting.Journals.ImportExcel";
        public const string Pages_Tenant_Accounting_Journals_ExportExcel = "Pages.Tenant.Accounting.Journals.ExportExcel";


        public const string Pages_Tenant_Close_Period_Create = "Pages.Tenant.Close.Period.Create";
        public const string Pages_Tenant_Close_Period_GetList = "Pages.Tenant.Close.Period.GetList";
        public const string Pages_Tenant_Close_Period = "Pages.Tenant.Close.Period";
        public const string Pages_Tenant_Close_Period_Delete = "Pages.Tenant.Close.Period.Delete";



        // End permission for accounting
        #endregion
        // Start Permission for Vendor
        #region Vendor
        public const string Pages_Tenant_Vendors = "Pages.Tenant.Vendors";
        public const string Pages_Tenant_PurchaseOrders = "Pages.Tenant.Vendors.PurchaseOrders";
        public const string Pages_Tenant_PurchaseOrder_Delete = "Pages.Tenant.Vendors.PurchaseOrder.Delete";
        public const string Pages_Tenant_PurchaseOrder_Disable = "Pages.Tenant.Vendors.PurchaseOrder.Disable";
        public const string Pages_Tenant_PurchaseOrder_Enable = "Pages.Tenant.Vendors.PurchaseOrder.Enable";
        public const string Pages_Tenant_PurchaseOrder_GetDetail = "Pages.Tenant.Vendors.PurchaseOrder.GetDetail";
        public const string Pages_Tenant_PurchaseOrder_GetList = "Pages.Tenant.Vendors.PurchaseOrder.GetList";
        public const string Pages_Tenant_PurchaseOrder_Update = "Pages.Tenant.Vendors.PurchaseOrder.Update";
        public const string Pages_Tenant_PurchaseOrder_Create = "Pages.Tenant.Vendors.PurchaseOrder.Create";
        public const string Pages_Tenant_PurchaseOrder_GetTotalPurchaseOrderForItemReceipts = "Pages.Tenant.Vendors.PurchaseOrder.GetTotalPurchaseOrderForItemReceipts";
        public const string Pages_Tenant_PurchaseOrder_GetTotalPurchaseOrderForBills = "Pages.Tenant.Vendors.PurchaseOrder.GetTotalPurchaseOrderForBills";
        public const string Pages_Tenant_PurchaseOrder_GetlistPuchaseOrderForItemReceipts = "Pages.Tenant.Vendors.PurchaseOrder.GetlistPuchaseOrderForItemReceipts";
        public const string Pages_Tenant_PurchaseOrder_GetlistPuchaseOrderForBills = "Pages.Tenant.Vendors.PurchaseOrder.GetlistPuchaseOrderForBills";
        public const string Pages_Tenant_PurchaseOrder_EditExchangeRate = "Pages.Tenant.Vendors.PurchaseOrder.EditExchangeRate";

        public const string Pages_Tenant_PurchaseOrder_Common = "Pages.Tenant.Vendors.PurchaseOrder.Common";
        public const string Pages_Tenant_PurchaseOrder_CanSeePrice = "Pages.Tenant.Vendors.PurchaseOrder.CanSeePrice";
        public const string Pages_Tenant_PurchaseOrder_UpdateStatusToDraft = "Pages.Tenant.Vendors.PurchaseOrder.UpdateStatusToDraft";
        public const string Pages_Tenant_PurchaseOrder_UpdateStatusToVoid = "Pages.Tenant.Vendors.PurchaseOrder.UpdateStatusToVoid";
        public const string Pages_Tenant_PurchaseOrder_UpdateStatusToPublish = "Pages.Tenant.Vendors.PurchaseOrder.UpdateStatusToPublish";
        public const string Pages_Tenant_PurchaseOrder_UpdateStatusToClose = "Pages.Tenant.Vendors.PurchaseOrder.UpdateStatusToClose";

        public const string Pages_Tenant_Vendors_Bills = "Pages.Tenant.Vendors.Bills";
        public const string Pages_Tenant_Vendors_Bills_Common = "Pages.Tenant.Vendors.Bills.Common";
        public const string Pages_Tenant_Vendors_Bills_Delete = "Pages.Tenant.Vendors.Bills.Delete";
        public const string Pages_Tenant_Vendors_Bills_MultiDelete = "Pages.Tenant.Vendors.Bills.MultiDelete";
        public const string Pages_Tenant_Vendors_Bills_Create = "Pages.Tenant.Vendors.Bills.Create";      
        public const string Pages_Tenant_Vendors_Bills_EditQty = "Pages.Tenant.Vendors.Bills.EditQty";
        public const string Pages_Tenant_Vendors_Bills_CanPO = "Pages.Tenant.Vendors.Bills.CanPO";
        public const string Pages_Tenant_Vendors_Bills_CanUpdatePO = "Pages.Tenant.Vendors.Bills.CanUpdatePO";
        public const string Pages_Tenant_Vendors_Bills_CanItemReceipt = "Pages.Tenant.Vendors.Bills.CanItemReceipt";
        public const string Pages_Tenant_Vendors_Bills_ChooseAccounts = "Pages.Tenant.Vendors.Bills.ChooseAccounts";
        public const string Pages_Tenant_Vendors_Bills_VendorCreditChooseAccounts = "Pages.Tenant.Vendors.Bills.VendorCreditChooseAccounts";
        public const string Pages_Tenant_Vendors_Bills_CanUpdateItemReceipt = "Pages.Tenant.Vendors.Bills.CanUpdateItemReceipt";

        public const string Pages_Tenant_Vendors_Bills_CanEditAccount = "Pages.Tenant.Vendors.Bills.CanEditAccount";
        public const string Pages_Tenant_Vendors_Bills_CanViewAccount = "Pages.Tenant.Vendors.Bills.CanViewAccount";
        public const string Pages_Tenant_Vendors_Bills_CanSeePrice = "Pages.Tenant.Vendors.Bills.CanSeePrice";
        public const string Pages_Tenant_Vendors_Bills_EditExchangeRate = "Pages.Tenant.Vendors.Bills.EditExchangeRate";

        public const string Pages_Tenant_Vendors_Bills_Update = "Pages.Tenant.Vendors.Bills.Update";
       
        public const string Pages_Tenant_Vendors_Bills_UpdateStatusToPublish = "Pages.Tenant.Vendors.Bills.UpdateStatusToPublish";
        public const string Pages_Tenant_Vendors_Bills_UpdateStatusToVoid = "Pages.Tenant.Vendors.Bills.UpdateStatusToVoid";
        public const string Pages_Tenant_Vendors_Bills_UpdateStatusToDraft = "Pages.Tenant.Vendors.Bills.UpdateStatusToDraft";
        public const string Pages_Tenant_Vendors_Bills_GetList = "Pages.Tenant.Vendors.Bills.GetList";
        public const string Pages_Tenant_Vendors_Bills_GetListForPayBill = "Pages.Tenant.Vendors.Bills.GetListForPayBill";
        public const string Pages_Tenant_Vendors_Bills_GetDetail = "Pages.Tenant.Vendors.Bills.GetDetail";
        public const string Pages_Tenant_Vendors_Bills_GetBills = "Pages.Tenant.Vendors.Bills.GetBills";
        public const string Pages_Tenant_Vendors_Bills_GetBillItems = "Pages.Tenant.Vendors.Bills.GetBillItems";
        public const string Pages_Tenant_Vendors_Bills_ImportExcel = "Pages.Tenant.Vendors.Bills.ImportExcel";
        public const string Pages_Tenant_Vendors_Bills_ImportExcelBill = "Pages.Tenant.Vendors.Bills.ImportExcelBill";
        public const string Pages_Tenant_Vendors_Bills_ExportExcel = "Pages.Tenant.Vendors.Bills.ExportExcel";
        public const string Pages_Tenant_Vendors_Bills_ExportExcelBillTamplate = "Pages.Tenant.Vendors.Bills.ExportExcelBillTamplate";
        public const string Pages_Tenant_Vendors_Bills_Setting = "Pages.Tenant.Vendors.Bills.Setting";
        
        public const string Pages_Tenant_CleanRounding = "Pages.Tenant.CleanRounding";

        public const string Pages_Tenant_Vendors_Bills_GetListForItemReceipt = "Pages.Tenant.Vendors.Bills.GetListForItemReceipt";

        public const string Pages_Tenant_UserActivities = "Pages.Tenant.UserActivities";
        public const string Pages_Tenant_UserActivities_FindActivity = "Pages.Tenant.UserActivities.FindActivity";
        public const string Pages_Tenant_UserActivities_FindTrasaction = "Pages.Tenant.UserActivities.FindTrasaction";
        public const string Pages_Tenant_UserActivities_ExportExcel = "Pages.Tenant.UserActivities.ExportExcel";
        public const string Pages_Tenant_UserActivities_GetList = "Pages.Tenant.UserActivities.GetList";
        public const string Pages_Tenant_UserActivities_SeeAll = "Pages.Tenant.UserActivities.SeeAll";
        public const string Pages_Tenant_UserActivities_SeeMine = "Pages.Tenant.UserActivities.SeeMine";
       
        // vendor Credit
        public const string Pages_Tenant_Vendors_Bills_CanCreateVendorCredit = "Pages.Tenant.Vendors.Bills.CanCreateVendorVeCredit";

        public const string Pages_Tenant_Vendors_PayBills = "Pages.Tenant.Vendors.PayBills";
        public const string Pages_Tenant_Vendors_PayBills_Common = "Pages.Tenant.Vendors.PayBills.Common";
        public const string Pages_Tenant_Vendors_PayBills_Delete = "Pages.Tenant.Vendors.PayBills.Delete";
        public const string Pages_Tenant_Vendors_PayBills_Create = "Pages.Tenant.Vendors.PayBills.Create";
        public const string Pages_Tenant_Vendors_PayBills_Update = "Pages.Tenant.Vendors.PayBills.Update";
        public const string Pages_Tenant_Vendors_PayBills_GetList = "Pages.Tenant.Vendors.PayBills.GetList";
        public const string Pages_Tenant_Vendors_PayBills_MultiDelete = "Pages.Tenant.Vendors.PayBills.MultiDelete";
        public const string Pages_Tenant_Vendors_PayBills_CanEditAccount = "Pages.Tenant.Vendors.PayBills.CanEditAccount";
        public const string Pages_Tenant_Vendors_PayBills_CanViewAccount = "Pages.Tenant.Vendors.PayBills.CanViewAccount";
        public const string Pages_Tenant_Vendors_PayBills_CanPayByCredit = "Pages.Tenant.Vendors.PayBills.CanPayByCredit";
        public const string Pages_Tenant_Vendors_PayBills_GetDetail = "Pages.Tenant.Vendors.PayBills.GetDetail";
        public const string Pages_Tenant_Vendors_PayBills_UpdateStatusToDraft = "Pages.Tenant.Vendors.PayBills.UpdateStatusToDraft";
        public const string Pages_Tenant_Vendors_PayBills_UpdateStatusToPublish = "Pages.Tenant.Vendors.PayBills.UpdateStatusToPublish";
        public const string Pages_Tenant_Vendors_PayBills_UpdateStatusToVoid = "Pages.Tenant.Vendors.PayBills.UpdateStatusToVoid";
        public const string Pages_Tenant_Vendors_PayBills_ViewBillHistory = "Pages.Tenant.Vendors.PayBills.ViewBillHistory";
        public const string Pages_Tenant_Vendors_PayBills_ViewVendorCreditHistory = "Pages.Tenant.Vendors.PayBills.ViewVendorCreditHistory";
        public const string Pages_Tenant_Vendors_PayBills_EditExchangeRate = "Pages.Tenant.Vendors.PayBills.EditExchangeRate";

        public const string Pages_Tenant_Vendors_ItemReceipts = "Pages.Tenant.Vendors.ItemReceipts";
        public const string Pages_Tenant_Vendors_ItemReceipts_common = "Pages.Tenant.Vendors.ItemReceipts.common";
        public const string Pages_Tenant_Vendors_ItemReceipts_importExcel = "Pages.Tenant.Vendors.ItemReceipts.ImportExcel";
        public const string Pages_Tenant_Vendors_ItemReceipts_importExcelItemReceiptOther = "Pages.Tenant.Vendors.ItemReceipts.ImportExcelItemReceiptOther";
        public const string Pages_Tenant_Vendors_ItemReceipts_importExcelItemReceiptAdjustment = "Pages.Tenant.Vendors.ItemReceipts.ImportExcelItemReceiptAdjustment";
        public const string Pages_Tenant_Vendors_ItemReceipts_exportExcel = "Pages.Tenant.Vendors.ItemReceipts.ExportExcel";
        public const string Pages_Tenant_Vendors_ItemReceipts_exportExcelItemReceiptOther = "Pages.Tenant.Vendors.ItemReceipts.ExportExcelItemReceiptOther";
        public const string Pages_Tenant_Vendors_ItemReceipts_exportExcelItemReceiptAdjustment = "Pages.Tenant.Vendors.ItemReceipts.ExportExcelItemReceiptAdjustment";
        public const string Pages_Tenant_Vendors_ItemReceipts_Delete = "Pages.Tenant.Vendors.ItemReceipts.Delete";

        public const string Pages_Tenant_Vendors_ItemReceipts_Create = "Pages.Tenant.Vendors.ItemReceipts.Create";
        public const string Pages_Tenant_Vendors_ItemReceipts_CreatePurchases = "Pages.Tenant.Vendors.ItemReceipts.CreatePurchases";
        public const string Pages_Tenant_Vendors_ItemReceipts_CreateCustomerCredits = "Pages.Tenant.Vendors.ItemReceipts.CreateCustomerCredits";
        public const string Pages_Tenant_Vendors_ItemReceipts_CreateTransfers = "Pages.Tenant.Vendors.ItemReceipts.CreateTransfers";
        public const string Pages_Tenant_Vendors_ItemReceipts_CreateOthers = "Pages.Tenant.Vendors.ItemReceipts.CreateOthers";
        public const string Pages_Tenant_Vendors_ItemReceipts_CreateAdjustments = "Pages.Tenant.Vendors.ItemReceipts.CreateAdjustments";
        public const string Pages_Tenant_Vendors_ItemReceipts_CreateProductionOrder = "Pages.Tenant.Vendors.ItemReceipts.CreateProductionOrder";

        public const string Pages_Tenant_Vendors_ItemReceipts_Update = "Pages.Tenant.Vendors.ItemReceipts.Update";

        public const string Pages_Tenant_Vendors_ItemReceipts_UpdateStatusToPublish = "Pages.Tenant.Vendors.ItemReceipts.UpdateStatusToPublish";
        public const string Pages_Tenant_Vendors_ItemReceipts_UpdateStatusToVoid = "Pages.Tenant.Vendors.ItemReceipts.UpdateStatusToVoid";
        public const string Pages_Tenant_Vendors_ItemReceipts_UpdateStatusToDraft = "Pages.Tenant.Vendors.ItemReceipts.UpdateStatusToDraft";
        public const string Pages_Tenant_Vendors_ItemReceipts_GetList = "Pages.Tenant.Vendors.ItemReceipts.GetList";
        public const string Pages_Tenant_Vendors_ItemReceipts_GetDetail = "Pages.Tenant.Vendors.ItemReceipts.GetDetail";
        public const string Pages_Tenant_Vendors_ItemReceipts_CanSeePrice = "Pages.Tenant.Vendors.ItemReceipts.CanSeePrice";
        public const string Pages_Tenant_Vendors_ItemReceipts_CanPO = "Pages.Tenant.Vendors.ItemReceipts.CanPO";
        public const string Pages_Tenant_Vendors_ItemReceipts_CanBill = "Pages.Tenant.Vendors.ItemReceipts.CanBill";
        public const string Pages_Tenant_Vendors_ItemReceipts_CanEditAccount = "Pages.Tenant.Vendors.ItemReceipts.CanEditAccount";
        public const string Pages_Tenant_Vendors_ItemReceipts_CanViewAccount = "Pages.Tenant.Vendors.ItemReceipts.CanViewAccount";
        public const string Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts = "Pages.Tenant.Vendors.ItemReceipts.GetItemReceipts";

        public const string Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts_VendorCredit = "Pages.Tenant.Vendors.ItemReceipts.GetItemReceipts.VendorCredit";

        public const string Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts_ItemIssueVendorCredit = "Pages.Tenant.Vendors.ItemReceipts.GetItemReceipts.ItemIssueVendorCredit";

        public const string Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts_CustomerCredit = "Pages.Tenant.Vendors.ItemReceipts.GetItemReceipts.CustomerCredit";
        public const string Pages_Tenant_Vendors_ItemReceipts_GetItemReceiptItems = "Pages.Tenant.Vendors.ItemReceipts.GetItemReceiptItems";

      

        #endregion
        // End permission for Vendor

        // Start Report permission
        #region Report 
        public const string Pages_Tenant_Report = "Pages.Tenant.Report";

        public const string Pages_Tenant_Report_Common = "Pages.Tenant.Report.Common";
        public const string Pages_Tenant_Report_CanEditAllTemplate = "Pages.Tenant.Report.CanEditAllTemplate";
        public const string Pages_Tenant_Report_ViewOriginal = "Pages.Tenant.Report.ViewOriginal";
        public const string Pages_Tenant_Report_ViewTemplate = "Pages.Tenant.Report.ViewTemplate";
        public const string Pages_Tenant_Report_Accounting = "Pages.Tenant.Report.Accounting";    
        public const string Pages_Tenant_Report_Inventory = "Pages.Tenant.Report.Inventory";
        public const string Pages_Tenant_Report_Stock_Balance = "Pages.Tenant.Report.Stock.Balance";
        public const string Pages_Tenant_Report_Stock_Balance_Export_Pdf = "Pages.Tenant.Report.Stock.Balance.Export.Pdf";
        public const string Pages_Tenant_Report_Stock_Balance_Export_Excel = "Pages.Tenant.Report.Stock.Balance.Export.Excel";
        public const string Pages_Tenant_Report_Inventory_Summary = "Pages.Tenant.Report.Inventory.Summary";
        public const string Pages_Tenant_Report_Inventory_Summary_RecalculateAvg = "Pages.Tenant.Report.Inventory.Summary.RecalculateAvg";
        public const string Pages_Tenant_Report_Inventory_Summary_Export_Excel = "Pages.Tenant.Report.Inventory.Summary.Export_Excel";
        public const string Pages_Tenant_Report_Inventory_Summary_Export_Pdf = "Pages.Tenant.Report.Inventory.Summary.Export.Pdf";
        public const string Pages_Tenant_Report_Inventory_Detail = "Pages.Tenant.Report.Inventory.Detail";
        public const string Pages_Tenant_Report_Inventory_InventoryTransaction = "Pages.Tenant.Report.Inventory.InventoryTransaction";
        public const string Pages_Tenant_Report_Inventory_InventoryTransaction_Export_Excel = "Pages.Tenant.Report.Inventory.InventoryTransaction.Export_Excel";
        public const string Pages_Tenant_Report_Inventory_InventoryTransaction_Export_Pdf = "Pages.Tenant.Report.Inventory.InventoryTransaction.Export.Pdf";

        public const string Pages_Tenant_Report_Asset_Balance = "Pages.Tenant.Report.Asset.Balance";
        public const string Pages_Tenant_Report_Asset_Balance_Export_Pdf = "Pages.Tenant.Report.Asset.Balance.Export.Pdf";
        public const string Pages_Tenant_Report_Asset_Balance_Export_Excel = "Pages.Tenant.Report.Asset.Balance.Export.Excel";

        public const string Pages_Tenant_Report_Inventory_Detail_Export_Excel = "Pages.Tenant.Report.Inventory.Detail.Export_Excel";
        public const string Pages_Tenant_Report_Inventory_Detail_Export_Pdf = "Pages.Tenant.Report.Inventory.Detail.Export.Pdf";
        public const string Pages_Tenant_Report_Inventory_Detail_RecalculateAvg = "Pages.Tenant.Report.Inventory.Detail.RecalculateAvg";

        public const string Pages_Tenant_Report_Traceability = "Pages.Tenant.Report.Traceability";
        public const string Pages_Tenant_Report_Traceability_Print = "Pages.Tenant.Report.Traceability.Print";
        public const string Pages_Tenant_Report_Traceability_ViewCustomer = "Pages.Tenant.Report.Traceability.ViewCustomer";
        public const string Pages_Tenant_Report_Traceability_Download = "Pages.Tenant.Report.Traceability.Download";

        public const string Pages_Tenant_Report_BatchNoBalance = "Pages.Tenant.Report.BatchNoBalance";
        public const string Pages_Tenant_Report_BatchNoBalance_ExportPdf = "Pages.Tenant.Report.BatchNoBalance.ExportPdf";
        public const string Pages_Tenant_Report_BatchNoBalance_ExportExcel = "Pages.Tenant.Report.BatchNoBalance.ExportExcel";

        public const string Pages_Tenant_Report_Production = "Pages.Tenant.Report.Production";

        public const string Pages_Tenant_Report_ProductionPlan = "Pages.Tenant.Report.ProductionPlan";
        public const string Pages_Tenant_Report_ProductionPlan_ExportPdf = "Pages.Tenant.Report.ProductionPlan.ExportPdf";
        public const string Pages_Tenant_Report_ProductionPlan_ExportExcel = "Pages.Tenant.Report.ProductionPlan.ExportExcel";
        public const string Pages_Tenant_Report_ProductionPlan_Calculate = "Pages.Tenant.Report.ProductionPlan.Calculate";

        public const string Pages_Tenant_Report_ProductionOrder = "Pages.Tenant.Report.ProductionOrder";
        public const string Pages_Tenant_Report_ProductionOrder_ExportPdf = "Pages.Tenant.Report.ProductionOrder.ExportPdf";
        public const string Pages_Tenant_Report_ProductionOrder_ExportExcel = "Pages.Tenant.Report.ProductionOrder.ExportExcel";
        public const string Pages_Tenant_Report_ProductionOrder_Calculate = "Pages.Tenant.Report.ProductionOrder.Calculate";

        public const string Pages_Tenant_Report_Template_Update = "Pages.Tenant.Report.Update";
        public const string Pages_Tenant_Report_Template_Delete = "Pages.Tenant.Report.Delete";
        public const string Pages_Tenant_Report_Template_Create = "Pages.Tenant.Report.Create";
        public const string Pages_Tenant_Report_Ledger = "Pages.Tenant.Report.Ledger";
        public const string Pages_Tenant_Report_Ledger_Export_Excel = "Pages.Tenant.Report.Ledger.ExportExcel";
        public const string Pages_Tenant_Report_Ledger_Export_Pdf = "Pages.Tenant.Report.Ledger.ExportPdf";

        public const string Pages_Tenant_Report_Cash = "Pages.Tenant.Report.Cash";
        public const string Pages_Tenant_Report_Cash_Export_Excel = "Pages.Tenant.Report.Cash.ExportExcel";
        public const string Pages_Tenant_Report_Cash_Export_Pdf = "Pages.Tenant.Report.Cash.ExportPdf";

        public const string Pages_Tenant_Report_CashFlow = "Pages.Tenant.Report.CashFlow";
        public const string Pages_Tenant_Report_CashFlow_Export_Excel = "Pages.Tenant.Report.CashFlow.ExportExcel";
        public const string Pages_Tenant_Report_CashFlow_Export_Pdf = "Pages.Tenant.Report.CashFlow.ExportPdf";

        public const string Pages_Tenant_Report_CashFlowTemplate = "Pages.Tenant.Report.CashFlowTemplate";
        public const string Pages_Tenant_Report_CashFlowTemplate_Create = "Pages.Tenant.Report.CashFlowTemplate.Create";
        public const string Pages_Tenant_Report_CashFlowTemplate_Update = "Pages.Tenant.Report.CashFlowTemplate.Update";
        public const string Pages_Tenant_Report_CashFlowTemplate_Delete = "Pages.Tenant.Report.CashFlowTemplate.Delete";
        public const string Pages_Tenant_Report_CashFlowTemplate_Enable = "Pages.Tenant.Report.CashFlowTemplate.Enable";
        public const string Pages_Tenant_Report_CashFlowTemplate_Disable = "Pages.Tenant.Report.CashFlowTemplate.Disable";

        public const string Pages_Tenant_Report_Journal = "Pages.Tenant.Report.Journals";
        public const string Pages_Tenant_Report_Journal_Export_Excel = "Pages.Tenant.Report.Journals.ExportExcel";
        public const string Pages_Tenant_Report_Journal_Export_Pdf = "Pages.Tenant.Report.Journals.ExportPdf";
        public const string Pages_Tenant_Report_Journal_Update = "Pages.Tenant.Report.Journals.Update";
        public const string Pages_Tenant_Report_Journal_Export = "Pages.Tenant.Report.Journals.Export";
        public const string Pages_Tenant_Report_Journal_Import = "Pages.Tenant.Report.Journals.Import";
        public const string Pages_Tenant_Report_Income = "Pages.Tenant.Report.Income";
        public const string Pages_Tenant_Report_Income_Export_Excel = "Pages.Tenant.Report.Income.Export_Excel";
        public const string Pages_Tenant_Report_Income_Export_Pdf = "Pages.Tenant.Report.Income.Export.Pdf";
        public const string Pages_Tenant_Report_BalanceSheet = "Pages.Tenant.Report.BalanceSheet";
        public const string Pages_Tenant_Report_BalanceSheet_Export_Excel = "Pages.Tenant.Report.BalanceSheet.ExportExcel";
        public const string Pages_Tenant_Report_BalanceSheet_Export_Pdf = "Pages.Tenant.Report.BalanceSheet.ExportPdf";
        public const string Pages_Tenant_Report_TrialBalance = "Pages.Tenant.Report.TrialBalance";
        public const string Pages_Tenant_Report_TrialBalance_Export_Excel = "Pages.Tenant.Report.TrialBalance.ExportExcel";
        public const string Pages_Tenant_Report_TrialBalance_Export_Pdf = "Pages.Tenant.Report.TrialBalance.ExportPdf";
        public const string Pages_Tenant_ChangeJournalDate = "Pages.Tenant.ChangeJournalDate";

        public const string Pages_Tenant_Report_Vendor = "Pages.Tenant.Report.Vendor";
        public const string Pages_Tenant_Report_Purchasing_Export_Excel = "Pages.Tenant.Report.Purchasing.Export_Excel";
        public const string Pages_Tenant_Report_Purchasing_Export_Pdf = "Pages.Tenant.Report.Purchasing.Export.Pdf";
        public const string Pages_Tenant_Report_Purchasing = "Pages.Tenant.Report.Purchasing";

        public const string Pages_Tenant_Report_Vendor_VendorAging_Export_Excel = "Pages.Tenant.Report.Vendor.VendorAging.Export_Excel";
        public const string Pages_Tenant_Report_Vendor_VendorAging_Export_Pdf = "Pages.Tenant.Report.Vendor.VendorAging.Export_Pdf";
        public const string Pages_Tenant_Report_Vendor_VendorAging = "Pages.Tenant.Report.Vendor.VendorAging";

        public const string Pages_Tenant_Report_Vendor_VendorByBill_Export_Excel = "Pages.Tenant.Report.Vendor.VendorByBill.Export_Excel";
        public const string Pages_Tenant_Report_Vendor_VendorByBill_Export_Pdf = "Pages.Tenant.Report.Vendor.VendorByBill.Export_Pdf";
        public const string Pages_Tenant_Report_Vendor_VendorByBill = "Pages.Tenant.Report.Vendor.VendorByBill";

        public const string Pages_Tenant_Report_Vendor_PurchaseByItemProperty_Export_Excel = "Pages.Tenant.Report.Vendor.PurchaseByItemProperty.Export_Excel";
        public const string Pages_Tenant_Report_Vendor_PurchaseByItemProperty_Export_Pdf = "Pages.Tenant.Report.Vendor.PurchaseByItemProperty.Export_Pdf";
        public const string Pages_Tenant_Report_Vendor_PurchaseByItemProperty = "Pages.Tenant.Report.Vendor.PurchaseByItemProperty";

        public const string Pages_Tenant_Report_QCTest_Export_Excel = "Pages.Tenant.Report.QCTest.Export_Excel";
        public const string Pages_Tenant_Report_QCTest_Export_Pdf = "Pages.Tenant.Report.QCTest.Export_Pdf";
        public const string Pages_Tenant_Report_QCTest = "Pages.Tenant.Report.QCTest";

        public const string Pages_Tenant_Report_Customer = "Pages.Tenant.Report.Customer";
        public const string Pages_Tenant_Report_SaleInvoice_Export_Excel = "Pages.Tenant.Report.SaleInvoice.Export_Excel";
        public const string Pages_Tenant_Report_SaleInvoice_Export_Pdf = "Pages.Tenant.Report.SaleInvoice.Export_Pdf";
        public const string Pages_Tenant_Report_SaleInvoice = "Pages.Tenant.Report.SaleInvoice";

        public const string Pages_Tenant_Report_SaleInvoiceByProperty_Export_Excel = "Pages.Tenant.Report.SaleInvoiceByProperty.Export_Excel";
        public const string Pages_Tenant_Report_SaleInvoiceByProperty_Export_Pdf = "Pages.Tenant.Report.SaleInvoiceByProperty.Export_Pdf";
        public const string Pages_Tenant_Report_SaleInvoiceByProperty = "Pages.Tenant.Report.SaleInvoiceByProperty";


        public const string Pages_Tenant_Report_SaleReturn_Export_Excel = "Pages.Tenant.Report.SaleReturn.Export_Excel";
        public const string Pages_Tenant_Report_SaleReturn_Export_Pdf = "Pages.Tenant.Report.SaleReturn.Export_Pdf";
        public const string Pages_Tenant_Report_SaleReturn = "Pages.Tenant.Report.SaleReturn";

        public const string Pages_Tenant_Report_Customer_CustomerAging_Export_Excel = "Pages.Tenant.Report.Customer.CustomerAging.Export_Excel";
        public const string Pages_Tenant_Report_Customer_CustomerAging_Export_Pdf = "Pages.Tenant.Report.Customer.CustomerAging.Export_Pdf";
        public const string Pages_Tenant_Report_Customer_CustomerAging = "Pages.Tenant.Report.Customer.CustomerAging";

        public const string Pages_Tenant_Report_Customer_CustomerByInvoice_Export_Excel = "Pages.Tenant.Report.Customer.CustomerByInvoice.Export_Excel";
        public const string Pages_Tenant_Report_Customer_CustomerByInvoice_Export_Pdf = "Pages.Tenant.Report.Customer.CustomerByInvoice.Export_Pdf";
        public const string Pages_Tenant_Report_Customer_CustomerByInvoice = "Pages.Tenant.Report.Customer.CustomerByInvoice";

        public const string Pages_Tenant_Report_Customer_CustomerByInvoiceWithPayment_Export_Excel = "Pages.Tenant.Report.Customer.CustomerByInvoiceWithPayment.Export_Excel";
        public const string Pages_Tenant_Report_Customer_CustomerByInvoiceWithPayment_Export_Pdf = "Pages.Tenant.Report.Customer.CustomerByInvoiceWithPayment.Export_Pdf";
        public const string Pages_Tenant_Report_Customer_CustomerByInvoiceWithPayment = "Pages.Tenant.Report.Customer.CustomerByInvoiceWithPayment";

        public const string Pages_Tenant_Report_SaleInvoiceDetail_Export_Excel = "Pages.Tenant.Report.SaleInvoiceDetail.Export_Excel";
        public const string Pages_Tenant_Report_SaleInvoiceDetail_Export_Pdf = "Pages.Tenant.Report.SaleInvoiceDetail.Export_Pdf";
        public const string Pages_Tenant_Report_SaleInvoiceDetail = "Pages.Tenant.Report.SaleInvoiceDetail";

        public const string Pages_Tenant_Report_PurchaseOrder = "Pages.Tenant.Report.PurchaseOrder";
        public const string Pages_Tenant_Report_PurchaseOrder_Export_Excel = "Pages.Tenant.Report.PurchaseOrder.Export_Excel";
        public const string Pages_Tenant_Report_PurchaseOrder_Export_Pdf = "Pages.Tenant.Report.PurchaseOrder.Export_Pdf";

        public const string Pages_Tenant_Report_SaleOrder = "Pages.Tenant.Report.SaleOrder";
        public const string Pages_Tenant_Report_SaleOrder_Export_Excel = "Pages.Tenant.Report.SaleOrder.Export_Excel";
        public const string Pages_Tenant_Report_SaleOrder_Export_Pdf = "Pages.Tenant.Report.SaleOrder.Export_Pdf";

        public const string Pages_Tenant_Report_SaleOrderDetail = "Pages.Tenant.Report.SaleOrderDetail";
        public const string Pages_Tenant_Report_SaleOrderDetail_Export_Excel = "Pages.Tenant.Report.SaleOrderDetail.Export_Excel";
        public const string Pages_Tenant_Report_SaleOrderDetail_Export_Pdf = "Pages.Tenant.Report.SaleOrderDetail.Export_Pdf";

        public const string Pages_Tenant_Report_SaleOrderByItemProperty = "Pages.Tenant.Report.SaleOrderByItemProperty";
        public const string Pages_Tenant_Report_SaleOrderByItemProperty_Export_Excel = "Pages.Tenant.Report.SaleOrderByItemProperty.Export_Excel";
        public const string Pages_Tenant_Report_SaleOrderByItemProperty_Export_Pdf = "Pages.Tenant.Report.SaleOrderByItemProperty.Export_Pdf";

        public const string Pages_Tenant_Report_ProfitByInvoice = "Pages.Tenant.Report.ProfitByInvoice";
        public const string Pages_Tenant_Report_ProfitByInvoice_Export_Excel = "Pages.Tenant.Report.ProfitByInvoice.Export_Excel";
        public const string Pages_Tenant_Report_ProfitByInvoice_Export_Pdf = "Pages.Tenant.Report.ProfitByInvoice.Export_Pdf";



        public const string Pages_Tenant_Report_DeliverySchedule = "Pages.Tenant.Report.DeliverySchedule";
        public const string Pages_Tenant_Report_DeliverySchedule_Detail = "Pages.Tenant.Report.DeliverySchedule.Detail";
        public const string Pages_Tenant_Report_DeliverySchedule_Detail_Export_Excel = "Pages.Tenant.Report.DeliverySchedule.Detail.Export_Excel";
        public const string Pages_Tenant_Report_DeliverySchedule_Detail_Export_Pdf = "Pages.Tenant.Report.DeliverySchedule.Detail.Export_Pdf";
        public const string Pages_Tenant_Report_DeliverySchedule_Summary = "Pages.Tenant.Report.DeliverySchedule.Summary";
        public const string Pages_Tenant_Report_DeliverySchedule_Summary_Export_Excel = "Pages.Tenant.Report.DeliverySchedule.Summary.Export_Excel";
        public const string Pages_Tenant_Report_DeliverySchedule_Summary_Export_Pdf = "Pages.Tenant.Report.DeliverySchedule.Summary.Export_Pdf";
        public const string Pages_Tenant_Report_DeliveryScheduleByItemProperty = "Pages.Tenant.Report.DeliveryScheduleByItemProperty";
        public const string Pages_Tenant_Report_DeliveryScheduleByItemProperty_Export_Excel = "Pages.Tenant.Report.DeliveryScheduleByItemProperty.Export_Excel";
        public const string Pages_Tenant_Report_DeliveryScheduleByItemProperty_Export_Pdf = "Pages.Tenant.Report.DeliveryScheduleByItemProperty.Export_Pdf";

        #endregion

        #region Inventory Transaction 
        public const string Pages_Tenant_Inventorys = "Pages.Tenant.Inventorys";
        public const string Pages_Tenant_Inventorys_InventoryTransactions = "Pages.Tenant.Inventorys.InventoryTransactions";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_Common = "Pages.Tenant.Inventorys.InventoryTransactions.Common";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_GetList = "Pages.Tenant.Inventorys.InventoryTransactions.GetList";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptPurchase = "Pages.Tenant.Inventorys.InventoryTransactions.CreateItemReceipt";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptCustomerCredit = "Pages.Tenant.Inventorys.InventoryTransactions.CreateItemReceipt.CustomerCredit";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptTransfer = "Pages.Tenant.Inventorys.InventoryTransactions.CreateItemReceipt.Transfer";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptProduction = "Pages.Tenant.Inventorys.InventoryTransactions.CreateItemReceipt.Production";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptAdjustment = "Pages.Tenant.Inventorys.InventoryTransactions.CreateItemReceipt.Adjustment";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptOther = "Pages.Tenant.Inventorys.InventoryTransactions.CreateItemReceipt.Other";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueSale = "Pages.Tenant.Inventorys.InventoryTransactions.CreateItemIssue";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_ItemIssuesSale_EditConvertInvoice = "Pages.Tenant.Inventorys.InventoryTransactions.ItemIssueSale.EditConvertInvoice";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_ItemIssuesSale_ShowConvertInvoice = "Pages.Tenant.Inventorys.InventoryTransactions.ItemIssueSale.ShowConvertInvoice";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueVendorCredit = "Pages.Tenant.Inventorys.InventoryTransactions.CreateItemIssue.VendorCredit";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueTransfer = "Pages.Tenant.Inventorys.InventoryTransactions.CreateItemIssue.Transfer";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueProduction = "Pages.Tenant.Inventorys.InventoryTransactions.CreateItemIssue.Production";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueAdjustment = "Pages.Tenant.Inventorys.InventoryTransactions.CreateItemIssue.Adjustment";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueOther = "Pages.Tenant.Inventorys.InventoryTransactions.CreateItemIssue.Other";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_ViewDetail = "Pages.Tenant.Inventorys.InventoryTransactions.Detail";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_Update = "Pages.Tenant.Inventorys.InventoryTransactions.Update";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_Delete = "Pages.Tenant.Inventorys.InventoryTransactions.Delete";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_Void = "Pages.Tenant.Inventorys.InventoryTransactions.Void";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_Draft = "Pages.Tenant.Inventorys.InventoryTransactions.Draft";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_Publish = "Pages.Tenant.Inventorys.InventoryTransactions.Publish";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_CanEditAccount = "Pages.Tenant.Inventorys.InventoryTransactions.CanEditAccount";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_CanViewAccount = "Pages.Tenant.Inventorys.InventoryTransactions.CanViewAccount";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_CanSeePrice = "Pages.Tenant.Inventorys.InventoryTransactions.CanSeePrice";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour = "Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour";

        public const string Pages_Tenant_Inventorys_InventoryTransactions_Create = "Pages.Tenant.Inventorys.InventoryTransactions.Create";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_ItemIssuesSale_CanInvoice = "Pages.Tenant.Inventorys.InventoryTransactions.ItemIssueSale.CanInvoice";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_ItemIssuesSale_CanSaleOrder = "Pages.Tenant.Inventorys.InventoryTransactions.ItemIssueSale.CanSaleOrder";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_ItemIssuesSale_CanDelivery = "Pages.Tenant.Inventorys.InventoryTransactions.ItemIssueSale.CanDelivery";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptPurchase_CanPurchaseOrder = "Pages.Tenant.Inventorys.InventoryTransactions.CreateItemReceipt.CanPurchaseOrder";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptPurchase_CanBill = "Pages.Tenant.Inventorys.InventoryTransactions.CreateItemReceipt.CanBill";

        public const string Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueTransfer_CanTransferOrder = "Pages.Tenant.Inventorys.InventoryTransactions.CreateItemIssue.Transfer.CanTransferOrder";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptTransfer_CanTransferOrder = "Pages.Tenant.Inventorys.InventoryTransactions.CreateItemReceipt.Transfer.CanTransferOrder";

        public const string Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueProduction_CanProductionOrder = "Pages.Tenant.Inventorys.InventoryTransactions.CreateItemIssue.Production.CanProductionOrder";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptPurchase_CanProductionOrder = "Pages.Tenant.Inventorys.InventoryTransactions.CreateItemReceipt.CanProductionOrder";

        public const string Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueVendorCredit_CanVendorCredit = "Pages.Tenant.Inventorys.InventoryTransactions.CreateItemIssue.VendorCredit.CanVendorCredit";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueVendorCredit_CanItemReceiptPurchase = "Pages.Tenant.Inventorys.InventoryTransactions.CreateItemIssue.VendorCredit.CanItemReceiptPurchase";

        public const string Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptCustomerCredit_CanCustomerCredit = "Pages.Tenant.Inventorys.InventoryTransactions.CreateItemReceiptCustomerCredit.CanCustomerCredit";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptCustomerCredit_CanIssueSale = "Pages.Tenant.Inventorys.InventoryTransactions.CreateItemReceiptCustomerCredit.CanIssueSale";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_ImportItemIssueAdjustment = "Pages.Tenant.Inventorys.InventoryTransactions.ImportExcelItemIssueAdjustment";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_ImportItemReceiptAdjustment = "Pages.Tenant.Inventorys.InventoryTransactions.ImportExcelItemReceiptAdjustment";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_ImportItemReceiptOther = "Pages.Tenant.Inventorys.InventoryTransactions.ImportExcelItemReceiptOther";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_ImportExcel = "Pages.Tenant.Inventorys.InventoryTransactions.ImportExcel";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_ExportItemIssueAdjustment = "Pages.Tenant.Inventorys.InventoryTransactions.ExportExcelItemIssueAdjustment";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_ExportItemReceiptAdjustment = "Pages.Tenant.Inventorys.InventoryTransactions.ExportExcelItemReceiptAdjustment";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_ExportItemReceiptOther = "Pages.Tenant.Inventorys.InventoryTransactions.ExportExcelItemReceiptOther";
        public const string Pages_Tenant_Inventorys_InventoryTransactions_ExportExcel = "Pages.Tenant.Inventorys.InventoryTransactions.ExportExcel";
        // Inventory Transfer order
        public const string Pages_Tenant_Inventory_TransferOrder = "Pages.Tenant.Inventory.TransferOrder";
        public const string Pages_Tenant_Inventory_TransferOrder_Common = "Pages.Tenant.Inventory.TransferOrder.Common";
        public const string Pages_Tenant_Inventory_TransferOrder_Delete = "Pages.Tenant.Inventory.TransferOrder.Delete";
        public const string Pages_Tenant_Inventory_TransferOrder_GetDetail = "Pages.Tenant.Inventory.TransferOrder.GetDetail";
        public const string Pages_Tenant_Inventory_TransferOrder_ForItemIssue = "Pages.Tenant.Inventory.TransferOrder.ForItemIssue";
        public const string Pages_Tenant_Inventory_TransferOrder_ForItemReceipt = "Pages.Tenant.Inventory.TransferOrder.ForItemReceipt";

        public const string Pages_Tenant_Inventory_TransferOrder_GetList = "Pages.Tenant.Inventory.TransferOrder.GetList";
        public const string Pages_Tenant_Inventory_TransferOrder_Update = "Pages.Tenant.Inventory.TransferOrder.Update";
        public const string Pages_Tenant_Inventory_TransferOrder_Create = "Pages.Tenant.Inventory.TransferOrder.Create";
        public const string Pages_Tenant_Inventory_TransferOrder_UpdateStatusToPublish = "Pages.Tenant.Inventory.TransferOrder.Publish";
        public const string Pages_Tenant_Inventory_TransferOrder_UpdateStatusToDraft = "Pages.Tenant.Inventory.TransferOrder.Draft";
        public const string Pages_Tenant_Inventory_TransferOrder_UpdateStatusToVoid = "Pages.Tenant.Inventory.TransferOrder.Void";
        public const string Pages_Tenant_Inventory_TransferOrder_EditDelete48hour = "Pages.Tenant.Inventory.TransferOrder.EditDelete48hour";


        public const string Pages_Tenant_Inventory_KitchenOrders = "Pages.Tenant.Inventory.KitchenOrders";
        public const string Pages_Tenant_Inventory_KitchenOrders_GetList = "Pages.Tenant.Inventory.KitchenOrders.GetList";
        public const string Pages_Tenant_Inventory_KitchenOrders_Update = "Pages.Tenant.Inventory.KitchenOrders.Update";
        public const string Pages_Tenant_Inventory_KitchenOrders_Create = "Pages.Tenant.Inventory.KitchenOrders.Create";
        public const string Pages_Tenant_Inventory_KitchenOrders_Detail = "Pages.Tenant.Inventory.KitchenOrders.Detail";
        public const string Pages_Tenant_Inventory_KitchenOrders_Delete = "Pages.Tenant.Inventory.KitchenOrders.Delete";
        public const string Pages_Tenant_Inventory_KitchenOrders_Import = "Pages.Tenant.Inventory.KitchenOrders.Import";
        public const string Pages_Tenant_Inventory_KitchenOrders_SeePrice = "Pages.Tenant.Inventory.KitchenOrders.SeePrice";



        // Inventory physical count
        public const string Pages_Tenant_Inventory_PhysicalCount = "Pages.Tenant.Inventory.PhysicalCount";
        public const string Pages_Tenant_Inventory_PhysicalCount_Delete = "Pages.Tenant.Inventory.PhysicalCount.Delete";
        public const string Pages_Tenant_Inventory_PhysicalCount_GetList = "Pages.Tenant.Inventory.PhysicalCount.GetList";
        public const string Pages_Tenant_Inventory_PhysicalCount_GetDetail = "Pages.Tenant.Inventory.PhysicalCount.GetDetail";
        public const string Pages_Tenant_Inventory_PhysicalCount_Update = "Pages.Tenant.Inventory.PhysicalCount.Update";
        public const string Pages_Tenant_Inventory_PhysicalCount_Create = "Pages.Tenant.Inventory.PhysicalCount.Create";
        public const string Pages_Tenant_Inventory_PhysicalCount_Print = "Pages.Tenant.Inventory.PhysicalCount.Print";
        public const string Pages_Tenant_Inventory_PhysicalCount_ImportExcel = "Pages.Tenant.Inventory.PhysicalCount.ImportExcel";
        public const string Pages_Tenant_Inventory_PhysicalCount_ExportPdf = "Pages.Tenant.Inventory.PhysicalCount.ExportPdf";
        public const string Pages_Tenant_Inventory_PhysicalCount_Setting = "Pages.Tenant.Inventory.PhysicalCount.Setting";
        public const string Pages_Tenant_Inventory_PhysicalCount_FillIn = "Pages.Tenant.Inventory.PhysicalCount.FillIn";
        public const string Pages_Tenant_Inventory_PhysicalCount_Close = "Pages.Tenant.Inventory.PhysicalCount.Close";
        public const string Pages_Tenant_Inventory_PhysicalCount_CanSeePrice = "Pages.Tenant.Inventory.PhysicalCount.CanSeePrice";
        public const string Pages_Tenant_Inventory_PhysicalCount_SyncInventory = "Pages.Tenant.Inventory.PhysicalCount.SyncInventory";
        public const string Pages_Tenant_Inventory_PhysicalCount_UpdateStatusToPublish = "Pages.Tenant.Inventory.PhysicalCount.Publish";
        public const string Pages_Tenant_Inventory_PhysicalCount_UpdateStatusToDraft = "Pages.Tenant.Inventory.PhysicalCount.Draft";
        public const string Pages_Tenant_Inventory_PhysicalCount_UpdateStatusToVoid = "Pages.Tenant.Inventory.PhysicalCount.Void";

        // inventory journal transaction type
        public const string Pages_Tenant_Inventory_JournalTransactionTypes = "Pages.Tenant.Inventory.JournalTransactionTypes";
        public const string Pages_Tenant_Inventory_JournalTransactionTypes_GetList = "Pages.Tenant.Inventory.JournalTransactionTypes.GetList";
        public const string Pages_Tenant_Inventory_JournalTransactionTypes_Update = "Pages.Tenant.Inventory.JournalTransactionTypes.Update";
        public const string Pages_Tenant_Inventory_JournalTransactionTypes_Create = "Pages.Tenant.Inventory.JournalTransactionTypes.Create";
        public const string Pages_Tenant_Inventory_JournalTransactionTypes_GetDetail = "Pages.Tenant.Inventory.JournalTransactionTypes.GetDetail";
        public const string Pages_Tenant_Inventory_JournalTransactionTypes_Enable = "Pages.Tenant.Inventory.JournalTransactionTypes.Enable";
        public const string Pages_Tenant_Inventory_JournalTransactionTypes_Disable = "Pages.Tenant.Inventory.JournalTransactionTypes.Disable";
        public const string Pages_Tenant_Inventory_JournalTransactionTypes_Delete = "Pages.Tenant.Inventory.JournalTransactionTypes.Delete";
        #endregion

        #region BankTransaction 
        public const string Pages_Tenant_Banks = "Pages.Tenant.Banks";
        public const string Pages_Tenant_Banks_BankTransactions_GetList = "Pages.Tenant.Banks.BankTransactions.GetList";
        public const string Pages_Tenant_Banks_BankTransactions = "Pages.Tenant.Banks.BankTransactions";

        public const string Pages_Tenant_Banks_BankTransactions_Delete = "Pages.Tenant.Banks.BankTransactions.Delete";
        public const string Pages_Tenant_Banks_BankTransactions_View = "Pages.Tenant.Banks.BankTransactions.View";
        public const string Pages_Tenant_Banks_Transactions_Create = "Pages.Tenant.Banks.Transactions.Create";
        public const string Pages_Tenant_Banks_Transactions_Update = "Pages.Tenant.Banks.Transactions.Update";

        public const string Pages_Tenant_Banks_BankTransactions_ReceivePayment_View = "Pages.Tenant.Banks.BankTransactions.ReceivePayment.View";
        public const string Pages_Tenant_Banks_BankTransactions_PayBill_View = "Pages.Tenant.Banks.BankTransactions.PayBill.View";
        public const string Pages_Tenant_Banks_BankTransactions_ReceivePayment_Delete = "Pages.Tenant.Banks.BankTransactions.ReceivePayment.Delete";
        public const string Pages_Tenant_Banks_BankTransactions_PayBill_Delete = "Pages.Tenant.Banks.BankTransactions.PayBill.Delete";
        public const string Pages_Tenant_Banks_BankTransactions_Receivepayment_Create = "Pages.Tenant.Banks.BankTransactions.Receivepayment.Create";
        public const string Pages_Tenant_Banks_BankTransactions_PayBill_Create = "Pages.Tenant.Banks.BankTransactions.PayBill.Create";
        public const string Pages_Tenant_Banks_BankTransactions_Receivepayment_Update = "Pages.Tenant.Banks.BankTransactions.Receivepayment.Update";
        public const string Pages_Tenant_Banks_BankTransactions_PayBill_Update = "Pages.Tenant.Banks.BankTransactions.PayBill.Update";

       

        public const string Pages_Tenant_Banks_Deposits = "Pages.Tenant.Banks.Deposits";
        public const string Pages_Tenant_Banks_Deposits_Delete = "Pages.Tenant.Banks.Deposits.Delete";
        public const string Pages_Tenant_Banks_Deposits_Create = "Pages.Tenant.Banks.Deposits.Create";
        public const string Pages_Tenant_Banks_Deposits_Update = "Pages.Tenant.Banks.Deposits.Update";
        public const string Pages_Tenant_Banks_Deposits_UpdateStatusToPublish = "Pages.Tenant.Banks.Deposits.Publish";
        public const string Pages_Tenant_Banks_Deposits_UpdateStatusToVoid = "Pages.Tenant.Banks.Deposits.Void";
        public const string Pages_Tenant_Banks_Deposits_UpdateStatusToDraft = "Pages.Tenant.Banks.Deposits.Draft";
        public const string Pages_Tenant_Banks_Deposits_GetDetail = "Pages.Tenant.Banks.Deposits.GetDetail";

        public const string Pages_Tenant_Banks_Withdraws = "Pages.Tenant.Banks.Withdraws";
        public const string Pages_Tenant_Banks_Withdraws_Delete = "Pages.Tenant.Banks.Withdraws.Delete";
        public const string Pages_Tenant_Banks_Withdraws_Create = "Pages.Tenant.Banks.Withdraws.Create";
        public const string Pages_Tenant_Banks_Withdraws_Update = "Pages.Tenant.Banks.Withdraws.Update";
        public const string Pages_Tenant_Banks_Withdraws_UpdateStatusToPublish = "Pages.Tenant.Banks.Withdraws.Publish";
        public const string Pages_Tenant_Banks_Withdraws_UpdateStatusToVoid = "Pages.Tenant.Banks.Withdraws.Void";
        public const string Pages_Tenant_Banks_Withdraws_UpdateStatusToDraft = "Pages.Tenant.Banks.Withdraws.Draft";
        public const string Pages_Tenant_Banks_Withdraws_GetDetail = "Pages.Tenant.Banks.Withdraws.GetDetail";

        public const string Pages_Tenant_Banks_Transfers = "Pages.Tenant.Banks.Transfers";
        public const string Pages_Tenant_Banks_Transfers_GetList = "Pages.Tenant.Banks.Transfers.GetList";
        public const string Pages_Tenant_Banks_Transfers_Delete = "Pages.Tenant.Banks.Transfers.Delete";
        public const string Pages_Tenant_Banks_Transfers_Create = "Pages.Tenant.Banks.Transfers.Create";
        public const string Pages_Tenant_Banks_Transfers_Update = "Pages.Tenant.Banks.Transfers.Update";
        public const string Pages_Tenant_Banks_Transfers_UpdateStatusToPublish = "Pages.Tenant.Banks.Transfers.Publish";
        public const string Pages_Tenant_Banks_Transfers_UpdateStatusToVoid = "Pages.Tenant.Banks.Transfers.Void";
        public const string Pages_Tenant_Banks_Transfers_UpdateStatusToDraft = "Pages.Tenant.Banks.Transfers.Draft";
        public const string Pages_Tenant_Banks_Transfers_GetDetail = "Pages.Tenant.Banks.Transfers.GetDetail";
        #endregion

        #region Customer Auth
        public const string Pages_Tenant_Customers = "Pages.Tenant.Customers";
        public const string Pages_Tenant_Customer_SaleOrder = "Pages.Tenant.Customers.SaleOrder";
        public const string Pages_Tenant_Customer_SaleOrder_Delete = "Pages.Tenant.Customers.SaleOrder.Delete";
        public const string Pages_Tenant_Customer_SaleOrder_Disable = "Pages.Tenant.Customers.SaleOrder.Disable";
        public const string Pages_Tenant_Customer_SaleOrder_Enable = "Pages.Tenant.Customers.SaleOrder.Enable";
        public const string Pages_Tenant_Customer_SaleOrder_CanSeePrice = "Pages.Tenant.Customers.SaleOrder.CanSeePrice";

        public const string Pages_Tenant_Customer_SaleOrder_Common = "Pages.Tenant.Customers.SaleOrder.Common";

        public const string Pages_Tenant_Customer_SaleOrder_GetDetail = "Pages.Tenant.Customers.SaleOrder.GetDetail";
        public const string Pages_Tenant_Customer_SaleOrder_GetList = "Pages.Tenant.Customers.SaleOrder.GetList";
        public const string Pages_Tenant_Customer_SaleOrder_Update = "Pages.Tenant.Customers.SaleOrder.Update";
        public const string Pages_Tenant_Customer_SaleOrder_Create = "Pages.Tenant.Customers.SaleOrder.Create";
        public const string Pages_Tenant_Customer_SaleOrder_UpdateToDraft = "Pages.Tenant.Customers.SaleOrder.UpdateToDraft";
        public const string Pages_Tenant_Customer_SaleOrder_UpdateToVoid = "Pages.Tenant.Customers.SaleOrder.UpdateToVoid";
        public const string Pages_Tenant_Customer_SaleOrder_UpdateToPublish = "Pages.Tenant.Customers.SaleOrder.UpdateToPublish";
        public const string Pages_Tenant_Customer_SaleOrder_UpdateToClose = "Pages.Tenant.Customers.SaleOrder.UpdateToClose";
        public const string Pages_Tenant_Customer_SaleOrder_UpdateInventoryStatus = "Pages.Tenant.Customers.SaleOrder.UpdateInventoryStatus";
        public const string Pages_Tenant_Customer_SaleOrder_GetTotalSaleOrder = "Pages.Tenant.Customers.SaleOrder.GetTotalSaleOrder";
        public const string Pages_Tenant_Customer_SaleOrder_GetTotalSaleOrderForInvoice = "Pages.Tenant.Customers.SaleOrder.GetTotalSaleOrderForInvoice";
        public const string Pages_Tenant_Customer_SaleOrder_GetItemSaleOrderForInvoices = "Pages.Tenant.Customers.SaleOrder.GetItemSaleOrderForInvoices";
        public const string Pages_Tenant_Customer_SaleOrder_GetItemSaleOrderForItemIssues = "Pages.Tenant.Customers.SaleOrder.GetItemSaleOrderForItemIssues";
        public const string Pages_Tenant_Customer_SaleOrder_EditExchangeRate = "Pages.Tenant.Customers.SaleOrder.EditExchangeRate";


        // delivery schedule
        public const string Pages_Tenant_Customer_DeliverySchedules = "Pages.Tenant.Customers.DeliverySchedules";
        public const string Pages_Tenant_Customer_DeliverySchedules_Find = "Pages.Tenant.Customers.DeliverySchedules.Find";
        public const string Pages_Tenant_Customer_DeliverySchedules_Delete = "Pages.Tenant.Customers.DeliverySchedules.Delete";
        public const string Pages_Tenant_Customer_DeliverySchedules_GetDetail = "Pages.Tenant.Customers.DeliverySchedules.GetDetail";
        public const string Pages_Tenant_Customer_DeliverySchedules_GetList = "Pages.Tenant.Customers.DeliverySchedules.GetList";
        public const string Pages_Tenant_Customer_DeliverySchedules_Update = "Pages.Tenant.Customers.DeliverySchedules.Update";
        public const string Pages_Tenant_Customer_DeliverySchedules_Create = "Pages.Tenant.Customers.DeliverySchedules.Create";
        public const string Pages_Tenant_Customer_DeliverySchedules_UpdateToDraft = "Pages.Tenant.Customers.DeliverySchedules.UpdateToDraft";
        public const string Pages_Tenant_Customer_DeliverySchedules_UpdateToVoid = "Pages.Tenant.Customers.DeliverySchedules.UpdateToVoid";
        public const string Pages_Tenant_Customer_DeliverySchedules_UpdateToPublish = "Pages.Tenant.Customers.DeliverySchedules.UpdateToPublish";
        public const string Pages_Tenant_Customer_DeliverySchedules_UpdateToClose = "Pages.Tenant.Customers.DeliverySchedules.UpdateToClose";
       // public const string Pages_Tenant_Customer_DeliverySchedules_UpdateInventoryStatus = "Pages.Tenant.Customers.DeliverySchedules.UpdateInventoryStatus";
        //public const string Pages_Tenant_Customer_DeliverySchedules_GetTotalDeliverySchedules = "Pages.Tenant.Customers.DeliverySchedules.GetTotalDeliverySchedules";
       // public const string Pages_Tenant_Customer_DeliverySchedules_GetTotalDeliveryScheduleForInvoice = "Pages.Tenant.Customers.DeliverySchedules.GetTotalDeliveryScheduleForInvoice";
       // public const string Pages_Tenant_Customer_DeliverySchedules_GetItemDeliveryScheduleForInvoices = "Pages.Tenant.Customers.DeliverySchedules.GetItemDeliveryScheduleForInvoices";
        //public const string Pages_Tenant_Customer_DeliverySchedules_GetItemDeliveryScheduleForItemIssues = "Pages.Tenant.Customers.DeliverySchedules.GetItemDeliveryScheduleForItemIssues";


        public const string Pages_Tenant_Customer_Invoices = "Pages.Tenant.Customers.Invoices";

        public const string Pages_Tenant_Customer_Invoices_Common = "Pages.Tenant.Customers.Invoices.Common";

        public const string Pages_Tenant_Customer_Invoice_Delete = "Pages.Tenant.Customers.Invoices.Delete";
        public const string Pages_Tenant_Customer_Invoice_Create = "Pages.Tenant.Customers.Invoices.Create";
        public const string Pages_Tenant_Customer_Invoice_MultiDelete = "Pages.Tenant.Customer.Invoice.MultiDelete";
        public const string Pages_Tenant_Customer_Invoice_EditQty = "Pages.Tenant.Customers.Invoices.EditQty";
        public const string Pages_Tenant_Customer_Invoice_Update = "Pages.Tenant.Customers.Invoices.Update";
        public const string Pages_Tenant_Customer_Invoice_UpdateToPublish = "Pages.Tenant.Customers.Invoices.UpdateToPublish";
        public const string Pages_Tenant_Customer_Invoice_UpdateToVoid = "Pages.Tenant.Customers.Invoices.UpdateToVoid";
        public const string Pages_Tenant_Customer_Invoice_UpdateToDraft = "Pages.Tenant.Customers.Invoices.UpdateToDraft";
        public const string Pages_Tenant_Customer_Invoice_GetList = "Pages.Tenant.Customers.Invoices.GetList";
        public const string Pages_Tenant_Customer_Invoice_CanSeePrice = "Pages.Tenant.Customers.Invoices.CanSeePrice";
        public const string Pages_Tenant_Customer_Invoice_CanEditAccount = "Pages.Tenant.Customers.Invoices.CanEditAccount";
        public const string Pages_Tenant_Customer_Invoice_CanViewAccount = "Pages.Tenant.Customers.Invoices.CanViewAccount";
        public const string Pages_Tenant_Customer_Invoice_CanSaleOrder = "Pages.Tenant.Customers.Invoices.CanSaleOrder";
        public const string Pages_Tenant_Customer_Invoice_CanItemIssue = "Pages.Tenant.Customers.Invoices.CanItemIssue";
        public const string Pages_Tenant_Customer_Invoice_ChooseAccounts = "Pages.Tenant.Customers.Invoices.ChooseAccounts";
        public const string Pages_Tenant_Customer_Invoice_CustomerCreditChooseAccounts = "Pages.Tenant.Customers.Invoices.CustomerCreditChooseAccounts";
        public const string Pages_Tenant_Customer_Invoice_GetDetail = "Pages.Tenant.Customers.Invoices.GetDetail";
        public const string Pages_Tenant_Customer_Invoice_GetInvoiceSummaryForItemIssue = "Pages.Tenant.Customers.Invoices.GetInvoice";
        public const string Pages_Tenant_Customer_Invoice_GetInvoiceItems = "Pages.Tenant.Customers.Invoices.GetInvoiceItems";
        public const string Pages_Tenant_Customer_Invoice_GetListInvoiceForReceivePayment = "Pages.Tenant.Customers.Invoices.GetListInvoiceForReceivePayment";

        public const string Pages_Tenant_Customer_Invoice_ImportExcel = "Pages.Tenant.Customers.Invoices.ImportExcel";
        public const string Pages_Tenant_Customer_Invoice_ImportExcelInvoice = "Pages.Tenant.Customers.Invoices.ImportExcelInvoice";
        public const string Pages_Tenant_Customer_Invoice_ExportExcel = "Pages.Tenant.Customers.Invoices.ExportExcel";
        public const string Pages_Tenant_Customer_Invoice_ExportExcelTamplate = "Pages.Tenant.Customers.Invoices.ExportExcelTamplate";
        public const string Pages_Tenant_Customer_Invoice_Setting = "Pages.Tenant.Customers.Invoices.Setting";
        public const string Pages_Tenant_Customer_Invoice_EditExchangeRate = "Pages.Tenant.Customers.Invoices.EditExchangeRate";


        public const string Pages_Tenant_Customer_InvoiceTemplates = "Pages.Tenant.Customer.InvoiceTemplates";
        public const string Pages_Tenant_Customer_InvoiceTemplates_Delete = "Pages.Tenant.Customer.InvoiceTemplates.Delete";
        public const string Pages_Tenant_Customer_InvoiceTemplates_Create = "Pages.Tenant.Customer.InvoiceTemplates.Create";
        public const string Pages_Tenant_Customer_InvoiceTemplates_Update = "Pages.Tenant.Customer.InvoiceTemplates.Update";
        public const string Pages_Tenant_Customer_InvoiceTemplates_Disable = "Pages.Tenant.Customer.InvoiceTemplates.Disable";
        public const string Pages_Tenant_Customer_InvoiceTemplates_Enable = "Pages.Tenant.Customer.InvoiceTemplates.Enable";
        public const string Pages_Tenant_Customer_InvoiceTemplates_GetList = "Pages.Tenant.Customer.InvoiceTemplates.GetList";
        public const string Pages_Tenant_Customer_InvoiceTemplates_GetDetail = "Pages.Tenant.Customer.InvoiceTemplates.GetDetail";


        public const string Pages_Tenant_Customer_Cards = "Pages.Tenant.Customer.Cards";
        public const string Pages_Tenant_Customer_Cards_Create = "Pages.Tenant.Customer.Cards.Create";
        public const string Pages_Tenant_Customer_Cards_Update = "Pages.Tenant.Customer.Cards.Update";
        public const string Pages_Tenant_Customer_Cards_Enable = "Pages.Tenant.Customer.Cards.Enable";
        public const string Pages_Tenant_Customer_Cards_Delete = "Pages.Tenant.Customer.Cards.Delete";
        public const string Pages_Tenant_Customer_Cards_View = "Pages.Tenant.Customer.Cards.View";
        public const string Pages_Tenant_Customer_Cards_GetList = "Pages.Tenant.Customer.Cards.GetList";
        public const string Pages_Tenant_Customer_Cards_Export = "Pages.Tenant.Customer.Cards.Export";
        public const string Pages_Tenant_Customer_Cards_Import = "Pages.Tenant.Customer.Cards.Import";
        public const string Pages_Tenant_Customer_Cards_Disable = "Pages.Tenant.Customer.Cards.Disable";
        public const string Pages_Tenant_Customer_Cards_Deactivate = "Pages.Tenant.Customer.Cards.Deactivate";
        //POS
        public const string Pages_Tenant_Customer_POS = "Pages.Tenant.Customers.POS";
        //public const string Pages_Tenant_Customer_POS_Delete = "Pages.Tenant.Customers.POS.Delete";
        public const string Pages_Tenant_Customer_POS_Create = "Pages.Tenant.Customers.POS.Create";
        //public const string Pages_Tenant_Customer_POS_Update = "Pages.Tenant.Customers.POS.Update";
        public const string Pages_Tenant_Customer_POS_UpdateToVoid = "Pages.Tenant.Customers.POS.UpdateToVoid";
        public const string Pages_Tenant_Customer_POS_GetList = "Pages.Tenant.Customers.POS.GetList";
        public const string Pages_Tenant_Customer_POS_CanSeePrice = "Pages.Tenant.Customers.POS.CanSeePrice";
        //public const string Pages_Tenant_Customer_POS_CanEditAccount = "Pages.Tenant.Customers.POS.CanEditAccount";
        public const string Pages_Tenant_Customer_POS_GetDetail = "Pages.Tenant.Customers.POS.GetDetail";

        public const string Pages_Tenant_Customer_POS_Print = "Pages.Tenant.Customers.POS.Print";

        public const string Pages_Tenant_Customer_POS_ViewAll = "Pages.Tenant.Customers.POS.ViewAll";

        //POS Setting 

        public const string Pages_Tenant_Customer_POSSetting = "Pages.Tenant.Customers.POSSetting";
        public const string Pages_Tenant_Customer_POSSetting_CreateOrUpdate = "Pages.Tenant.Customers.POSSetting_CreateOrUpdate";
        public const string Pages_Tenant_Customer_POSSetting_GetDetail = "Pages.Tenant.Customers.POSSetting_GetDetail";
        public const string Pages_Tenant_Customer_POSSetting_Delete = "Pages.Tenant.Customers.POSSetting_Delete";
        // customer Credit
        public const string Pages_Tenant_Customers_Invoice_CreateCustomerCredit = "Pages.Tenant.Customers.Invoices.CreateCustomerCredit";
        public const string Pages_Tenant_Customers_Credit_GetCustomerCreditHeader = "Pages.Tenant.Customers.Credit.GetCustomerCreditHeader";
        public const string Pages_Tenant_Customers_Credit_GetCustomerCreditItem = "Pages.Tenant.Customers.Credit.GetCustomerCreditItem";

        public const string Pages_Tenant_Customers_ItemIssues = "Pages.Tenant.Customers.ItemIssues";
        public const string Pages_Tenant_Customers_ItemIssues_Common = "Pages_Tenant_Customers_ItemIssues";
        public const string Pages_Tenant_Customers_ItemIssues_Delete = "Pages.Tenant.Customers.ItemIssues.Delete";
        public const string Pages_Tenant_Customers_ItemIssues_Create = "Pages.Tenant.Customers.ItemIssues.Create";
        public const string Pages_Tenant_Customers_ItemIssues_Update = "Pages.Tenant.Customers.ItemIssues.Update";
        public const string Pages_Tenant_Customers_ItemIssues_EditConvertInvoice = "Pages.Tenant.Customers.ItemIssues.EditConvertInvoice";
        public const string Pages_Tenant_Customers_ItemIssues_ShowConvertInvoice = "Pages.Tenant.Customers.ItemIssues.ShowConvertInvoice";
        public const string Pages_Tenant_Customers_ItemIssues_CanSeePrice = "Pages.Tenant.Customers.ItemIssues.CanSeePrice";
        public const string Pages_Tenant_Customers_ItemIssues_CanEditAccount = "Pages.Tenant.Customers.ItemIssues.CanEditAccount";
        public const string Pages_Tenant_Customers_ItemIssues_CanViewAccount = "Pages.Tenant.Customers.ItemIssues.CanViewAccount";
        public const string Pages_Tenant_Customers_ItemIssues_CanSaleOrder = "Pages.Tenant.Customers.ItemIssues.CanSaleOrder";
        public const string Pages_Tenant_Customers_ItemIssues_CanInvoice = "Pages.Tenant.Customers.ItemIssues.CanInvoice";
        public const string Pages_Tenant_Customers_ItemIssues_CanDelivery = "Pages.Tenant.Customers.ItemIssues.CanDelivery";
        public const string Pages_Tenant_Customers_ItemIssues_UpdateStatusToPublish = "Pages.Tenant.Customers.ItemIssues.UpdateStatusToPublish";
        public const string Pages_Tenant_Customers_ItemIssues_UpdateStatusToVoid = "Pages.Tenant.Customers.ItemIssues.UpdateStatusToVoid";
        public const string Pages_Tenant_Customers_ItemIssues_UpdateStatusToDraft = "Pages.Tenant.Customers.ItemIssues.UpdateStatusToDraft";
        public const string Pages_Tenant_Customers_ItemIssues_GetList = "Pages.Tenant.Customers.ItemIssues.GetList";
        public const string Pages_Tenant_Customers_ItemIssues_GetDetail = "Pages.Tenant.Customers.ItemIssues.GetDetail";
        public const string Pages_Tenant_Customers_ItemIssues_GetItemIssues = "Pages.Tenant.Customers.ItemIssues.GetItemIssues";
        public const string Pages_Tenant_Customers_ItemIssues_GetItemIssueItems = "Pages.Tenant.Customers.ItemIssues.GetItemIssueItems";
        public const string Pages_Tenant_Customers_ItemIssues_ImportExcelItemIssueAdjustment = "Pages.Tenant.Customers.ItemIssues.ImportExcelItemIssueAdjustment";
        public const string Pages_Tenant_Customers_ItemIssues_ExportExcelTemplateItemIssueAdjustment = "Pages.Tenant.Customers.ItemIssues.ExportExcelItemIssueAdjustment";

        public const string Pages_Tenant_Customers_ItemIssues_GetItemIssues_ForCustomerCredit = "Pages.Tenant.Customers.ItemIssues.GetItemIssues.ForCustomerCredit";
        public const string Pages_Tenant_Customers_ItemIssues_GetItemIssues_ForItemReceiptCustomerCrdit = "Pages.Tenant.Customers.ItemIssues.GetItemIssues.ForItemReceiptCustomerCrdit";

        public const string Pages_Tenant_Customers_ItemIssues_GetItemIssues_SaleReturn = "Pages.Tenant.Customers.ItemIssues.GetItemIssues.SaleReturn";

        public const string Pages_Tenant_Customers_ItemIssues_GetItemIssues_ForVendorCredit = "Pages.Tenant.Customers.ItemIssues.GetItemIssues.ForVendorCredit";

        public const string Pages_Tenant_Customers_ItemIssues_CanCreateTransfer = "Pages.Tenant.Customers.ItemIssues.CreateTransfers";
        public const string Pages_Tenant_Customers_ItemIssues_CanCreateAdjustment = "Pages.Tenant.Customers.ItemIssues.CreateAdjustment";       
        public const string Pages_Tenant_Customers_ItemIssues_CanCreateOther = "Pages.Tenant.Customers.ItemIssues.CreateOther";
        public const string Pages_Tenant_Customers_ItemIssues_CanCreateVendorCredit = "Pages.Tenant.Customers.ItemIssues.CreateVendorCredits";
        public const string Pages_Tenant_Customers_ItemIssues_CanCreateProductionOrder = "Pages.Tenant.Customers.ItemIssues.CreateProductionOrder";

        public const string Pages_Tenant_Customers_ReceivePayments = "Pages.Tenant.Customers.ReceivePayments";
        public const string Pages_Tenant_Customers_ReceivePayments_Common = "Pages.Tenant.Customers.ReceivePayments.Common";

        public const string Pages_Tenant_Customers_ReceivePayments_Delete = "Pages.Tenant.Customers.ReceivePayments.Delete";
        public const string Pages_Tenant_Customers_ReceivePayments_Create = "Pages.Tenant.Customers.ReceivePayments.Create";
        public const string Pages_Tenant_Customers_ReceivePayments_Update = "Pages.Tenant.Customers.ReceivePayments.Update";
        public const string Pages_Tenant_Customers_ReceivePayments_GetList = "Pages.Tenant.Customers.ReceivePayments.GetList";
        public const string Pages_Tenant_Customers_ReceivePayments_MultiDelete = "Pages.Tenant.Customers.ReceivePayments.MultiDelete";
        public const string Pages_Tenant_Customers_ReceivePayments_Export_Excel = "Pages.Tenant.Customers.ReceivePayments.Export.Excel";

        public const string Pages_Tenant_Customers_ReceivePayments_GetDetail = "Pages.Tenant.Customers.ReceivePayments.GetDetail";
        public const string Pages_Tenant_Customers_ReceivePayments_CanEditAccount = "Pages.Tenant.Customers.ReceivePayments.CanEditAccount";
        public const string Pages_Tenant_Customers_ReceivePayments_CanViewAccount = "Pages.Tenant.Customers.ReceivePayments.CanViewAccount";
        //public const string Pages_Tenant_Customers_ReceivePayments_CanPayByCredit = "Pages.Tenant.Customers.ReceivePayments.CanPayByCredit";
        public const string Pages_Tenant_Customers_ReceivePayments_UpdateStatusToDraft = "Pages.Tenant.Customers.ReceivePayments.UpdateStatusToDraft";
        public const string Pages_Tenant_Customers_ReceivePayments_UpdateStatusToPublish = "Pages.Tenant.Customers.ReceivePayments.UpdateStatusToPublish";
        public const string Pages_Tenant_Customers_ReceivePayments_UpdateStatusToVoid = "Pages.Tenant.Customers.ReceivePayments.UpdateStatusToVoid";
        public const string Pages_Tenant_Customers_ReceivePayments_ViewInvoiceHistory = "Pages.Tenant.Customers.ReceivePayments.ViewInvoiceHistory";
        public const string Pages_Tenant_Customers_ReceivePayments_ViewCustomerCreditHistory = "Pages.Tenant.Customers.ReceivePayments.ViewCustomerCreditHistory";
        public const string Pages_Tenant_Customers_ReceivePayments_EditExchangeRate = "Pages.Tenant.Customers.ReceivePayments.EditExchangeRate";


        #endregion

        #region Productions
        public const string Pages_Tenant_Production = "Pages.Tenant.Production";
        public const string Pages_Tenant_Production_ProductionOrder = "Pages.Tenant.Production.ProductionOrder";
        public const string Pages_Tenant_Production_ProductionOrder_Common = "Pages.Tenant.Production.ProductionOrder.Common";
        public const string Pages_Tenant_Production_ProductionOrder_CanSeePrice = "Pages.Tenant.Production.ProductionOrder.CanSeePrice";
        public const string Pages_Tenant_Production_ProductionOrder_CanEditAcccount = "Pages.Tenant.Production.ProductionOrder.CanEditAccount";
        public const string Pages_Tenant_Production_ProductionOrder_CanViewAcccount = "Pages.Tenant.Production.ProductionOrder.CanViewAccount";
        public const string Pages_Tenant_Production_ProductionOrder_CanViewCalculationState = "Pages.Tenant.Production.ProductionOrder.CanViewCalculationState";
        public const string Pages_Tenant_Production_ProductionOrder_Create = "Pages.Tenant.Production.ProductionOrder.Create";
        public const string Pages_Tenant_Production_ProductionOrder_Update = "Pages.Tenant.Production.ProductionOrder.Update";
        public const string Pages_Tenant_Production_ProductionOrder_EditDeleteBy48hour = "Pages.Tenant.Production.ProductionOrder.EditDeate48Hour";
        public const string Pages_Tenant_Production_ProductionOrder_UpdateStatusToVoid = "Pages.Tenant.Production.ProductionOrder.UpdateStatusToVoid";
        public const string Pages_Tenant_Production_ProductionOrder_UpdateStatusToDraft = "Pages.Tenant.Production.ProductionOrder.UpdateStatusToDraft";
        public const string Pages_Tenant_Production_ProductionOrder_UpdateStatusToPublish = "Pages.Tenant.Production.ProductionOrder.UpdateStatusToPublish";
        public const string Pages_Tenant_Production_ProductionOrder_GetList = "Pages.Tenant.Production.ProductionOrder.GetList";
        public const string Pages_Tenant_Production_ProductionOrder_GetDetail = "Pages.Tenant.Production.ProductionOrder.GetDetail";
        public const string Pages_Tenant_Production_ProductionOrder_Find = "Pages.Tenant.Production.ProductionOrder.Find";
        public const string Pages_Tenant_Production_ProductionOrder_Calculate = "Pages.Tenant.Production.ProductionOrder.Calculate";
        public const string Pages_Tenant_Production_ProductionOrder_GetListProductionOrderForItemReceipt = "Pages.Tenant.Production.ProductionOrder.GetListProductionOrderForItemReceipt";
        public const string Pages_Tenant_Production_ProductionOrder_GetListProductionOrderForItemIssue = "Pages.Tenant.Production.ProductionOrder.GetListProductionOrderForItemIssue";
        public const string Pages_Tenant_Production_ProductionOrder_Delete = "Pages.Tenant.Production.ProductionOrder.Delete";
        public const string Pages_Tenant_Production_ProductionOrder_AutoInventory = "Pages.Tenant.Production.ProductionOrder.AutoInventory";
        public const string Pages_Tenant_Production_ProductionOrder_GetProductionOrderForItemReceipt = "Pages.Tenant.Production.ProductionOrder.GetProductionOrderForItemReceipt";
        public const string Pages_Tenant_Production_ProductionOrder_GetProductionOrderForItemIssue = "Pages.Tenant.Production.ProductionOrder.GetProductionOrderForItemIssue";
        public const string Pages_Tenant_Production_Process = "Pages.Tenant.Production.Process";
        public const string Pages_Tenant_Production_Process_Create = "Pages.Tenant.Production.Process.Create";
        public const string Pages_Tenant_Production_Process_Update = "Pages.Tenant.Production.Process.Update";
        public const string Pages_Tenant_Production_Process_UpdateStatusToDisable = "Pages.Tenant.Production.Process.UpdateStatusToDisable";
        public const string Pages_Tenant_Production_Process_UpdateStatusToEnable = "Pages.Tenant.Production.Process.UpdateStatusToEnable";
        public const string Pages_Tenant_Production_Process_GetList = "Pages.Tenant.Production.Process.GetList";
        public const string Pages_Tenant_Production_Process_GetDetail = "Pages.Tenant.Production.Process.GetDetail";
        public const string Pages_Tenant_Production_Process_Find = "Pages.Tenant.Production.Process.Find";
        public const string Pages_Tenant_Production_Process_Delete = "Pages.Tenant.Production.Process.Delete";
        
        public const string Pages_Tenant_Production_Plan = "Pages.Tenant.Production.Plan";
        public const string Pages_Tenant_Production_Plan_Create = "Pages.Tenant.Production.Plan.Create";
        public const string Pages_Tenant_Production_Plan_Update = "Pages.Tenant.Production.Plan.Update";
        public const string Pages_Tenant_Production_Plan_GetList = "Pages.Tenant.Production.Plan.GetList";
        public const string Pages_Tenant_Production_Plan_GetDetail = "Pages.Tenant.Production.Plan.GetDetail";
        public const string Pages_Tenant_Production_Plan_Find = "Pages.Tenant.Production.Plan.Find";
        public const string Pages_Tenant_Production_Plan_Delete = "Pages.Tenant.Production.Plan.Delete";
        public const string Pages_Tenant_Production_Plan_Close = "Pages.Tenant.Production.Plan.Close";
        public const string Pages_Tenant_Production_Plan_Open = "Pages.Tenant.Production.Plan.Open";
        public const string Pages_Tenant_Production_Plan_Calculate = "Pages.Tenant.Production.Plan.Calculate";

        public const string Pages_Tenant_Production_Line = "Pages.Tenant.Production.Line";
        public const string Pages_Tenant_Production_Line_Create = "Pages.Tenant.Production.Line.Create";
        public const string Pages_Tenant_Production_Line_Update = "Pages.Tenant.Production.Line.Update";
        public const string Pages_Tenant_Production_Line_GetList = "Pages.Tenant.Production.Line.GetList";
        public const string Pages_Tenant_Production_Line_GetDetail = "Pages.Tenant.Production.Line.GetDetail";
        public const string Pages_Tenant_Production_Line_Find = "Pages.Tenant.Production.Line.Find";
        public const string Pages_Tenant_Production_Line_Delete = "Pages.Tenant.Production.Line.Delete";
        public const string Pages_Tenant_Production_Line_Enable = "Pages.Tenant.Production.Line.Enable";
        public const string Pages_Tenant_Production_Line_Disable = "Pages.Tenant.Production.Line.Disable";
        #endregion


        //HOST-SPECIFIC PERMISSIONS
        public const string Pages_Host_Auto_Log_Transacction = "Pages.Host.Auto.Log.Transacction";
        public const string Pages_Host_RefreshToken = "Pages.Host.RefreshToken";

        public const string Pages_Editions = "Pages.Editions";
        public const string Pages_Editions_Create = "Pages.Editions.Create";
        public const string Pages_Editions_Edit = "Pages.Editions.Edit";
        public const string Pages_Editions_Delete = "Pages.Editions.Delete";

        public const string Pages_Tenants = "Pages.Tenants";

        public const string Pages_Tenants_GetList = "Pages.Tenants.GetList";
        public const string Pages_Tenants_Create = "Pages.Tenants.Create";
        public const string Pages_Tenants_Edit = "Pages.Tenants.Edit";
        public const string Pages_Tenants_ChangeFeatures = "Pages.Tenants.ChangeFeatures";
        public const string Pages_Tenants_Delete = "Pages.Tenants.Delete";
        public const string Pages_Tenants_Impersonation = "Pages.Tenants.Impersonation";
        public const string Pages_Tenants_EnableDebugMode = "Pages.Tenants.EnablDebugMode";
        public const string Pages_Tenants_DisableDebugMode = "Pages.Tenants.DisableDebugMode";

        public const string Pages_Tenant_CompanyProfile_Organazition = "Pages.TenantCompanyProfile.Organazition";
        public const string Pages_Tenants_CompanyProfile_Update = "Pages.Tenants.CompanyProfile.Update";
        public const string Pages_Tenants_CompanyProfile_GetDetail = "Pages.Tenants.CompanyProfile.GetDetail";

        public const string Pages_Tenants_CompanyProfile_ClearDefaultValue = "Pages.Tenants.CompanyProfile.ClearDefaultValue";

        public const string Pages_Administration_Host_Maintenance = "Pages.Administration.Host.Maintenance";
        public const string Pages_Administration_Host_Settings = "Pages.Administration.Host.Settings";
        public const string Pages_Administration_Host_Dashboard = "Pages.Administration.Host.Dashboard";

        public const string Pages_Host_Client = "Pages.Host.Client";
        public const string Pages_Host_Client_AccountTypes = "Pages.Host.Client.AccountTypes";
        public const string Pages_Host_Client_AccountTypes_Create = "Pages.Host.Client.AccountTypes_Create";
        public const string Pages_Host_Client_AccountTypes_Edit = "Pages.Host.Client.AccountTypes_Edit";
        public const string Pages_Host_Client_AccountTypes_Delete = "Pages.Host.Client.AccountTypes_Delete";
        public const string Pages_Host_Client_AccountTypes_Enable = "Pages.Host.Client.AccountTypes_Enable";
        public const string Pages_Host_Client_AccountTypes_Disable = "Pages.Host.Client.AccountTypes_Disable";
        public const string Pages_Host_Client_AccountTypes_ExportExcel = "Pages.Host.Client.AccountTypes.ExportExcel";
        public const string Pages_Host_Client_AccountTypes_ImportExcel = "Pages.Host.Client.AccountTypes.ImportExcel";

        // Inventory calculation schedule
        public const string Pages_Host_Client_CalculationSchedule = "Pages.Host.Client.CalculationSchedule";
        public const string Pages_Host_Client_CalculationSchedule_Create = "Pages.Host.Client.CalculationSchedule.Create";
        public const string Pages_Host_Client_CalculationSchedule_Edit = "Pages.Host.Client.CalculationSchedule.Edit";
        public const string Pages_Host_Client_CalculationSchedule_Delete = "Pages.Host.Client.CalculationSchedule.Delete";
        public const string Pages_Host_Client_CalculationSchedule_GetList = "Pages.Host.Client.CalculationSchedule.GetList";
        public const string Pages_Host_Client_CalculationSchedule_GetDetail = "Pages.Host.Client.CalculationSchedule.GetDetail";
        public const string Pages_Host_Client_CalculationSchedule_Enable = "Pages.Host.Client.CalculationSchedule.Enable";
        public const string Pages_Host_Client_CalculationSchedule_Disable = "Pages.Host.Client.CalculationSchedule.Disable";

        public const string Pages_Host_Client_BatchNoFormula = "Pages.Host.Client.BatchNoFormula";
        public const string Pages_Host_Client_BatchNoFormula_Create = "Pages.Host.Client.BatchNoFormula.Create";
        public const string Pages_Host_Client_BatchNoFormula_Edit = "Pages.Host.Client.BatchNoFormula.Edit";
        public const string Pages_Host_Client_BatchNoFormula_Delete = "Pages.Host.Client.BatchNoFormula.Delete";
        public const string Pages_Host_Client_BatchNoFormula_GetList = "Pages.Host.Client.BatchNoFormula.GetList";
        public const string Pages_Host_Client_BatchNoFormula_GetDetail = "Pages.Host.Client.BatchNoFormula.GetDetail";
        public const string Pages_Host_Client_BatchNoFormula_Find = "Pages.Host.Client.BatchNoFormula.Find";
        public const string Pages_Host_Client_BatchNoFormula_Enable = "Pages.Host.Client.BatchNoFormula.Enable";
        public const string Pages_Host_Client_BatchNoFormula_Disable = "Pages.Host.Client.BatchNoFormula.Disable";

        // Host Sign Up
        public const string Pages_Tenants_SignUps = "Pages.Tenants.SignUps";
        public const string Pages_Tenants_SignUps_Enable = "Pages.Tenants.SignUps.Enable";
        public const string Pages_Tenants_SignUps_UpdateStatus = "Pages.Tenants.SignUps.UpdateStatus";
        public const string Pages_Tenants_SignUps_Disable = "Pages.Tenants.SignUps.Disable";
        public const string Pages_Tenants_SignUps_GetList = "Pages.Tenants.SignUps.GetList";
        public const string Pages_Tenants_SignUps_Delete = "Pages.Tenants.SignUps.Delete";
        public const string Pages_Tenants_SignUps_Create = "Pages.Tenants.SignUps.Create";
        public const string Pages_Tenants_SignUps_Find = "Pages.Tenants.SignUps.Find";
        public const string Pages_Tenants_SignUps_GetDetail = "Pages.Tenants.SignUps.GetDetail";
        public const string Pages_Tenants_SignUps_Edit = "Pages.Tenants.SignUps.Edit";
        public const string Pages_Tenants_SignUps_LinkTenant = "Pages.Tenants.SignUps.LinkTenant";

        // Host Referall
        public const string Pages_Host_Referrals = "Pages.Host.Referrals";
        public const string Pages_Host_Referrals_Enable = "Pages.Host.Referrals.Enable";
        public const string Pages_Host_Referrals_Disable = "Pages.Host.Referrals.Disable";
        public const string Pages_Host_Referrals_GetList = "Pages.Host.Referrals.GetList";
        public const string Pages_Host_Referrals_Delete = "Pages.Host.Referrals.Delete";
        public const string Pages_Host_Referrals_Create = "Pages.Host.Referrals.Create";
        public const string Pages_Host_Referrals_Find = "Pages.Host.Referrals.Find";
        public const string Pages_Host_Referrals_GetDetail = "Pages.Host.Referrals.GetDetail";
        public const string Pages_Host_Referrals_Edit = "Pages.Host.Referrals.Edit";     

    }
}
