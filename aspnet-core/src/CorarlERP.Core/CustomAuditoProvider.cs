using Abp.Auditing;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Classes;
using CorarlERP.Customers;
using CorarlERP.Items;
using CorarlERP.Journals;
using CorarlERP.Locations;
using CorarlERP.Lots;
using CorarlERP.ProductionProcesses;
using CorarlERP.Productions;
using CorarlERP.PropertyValues;
using CorarlERP.PurchaseOrders;
using CorarlERP.SaleOrders;
using CorarlERP.Taxes;
using CorarlERP.TransactionTypes;
using CorarlERP.TransferOrders;
using CorarlERP.Vendors;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace CorarlERP
{
    public class CustomAuditoProvider: DefaultAuditInfoProvider, IAuditInfoProvider
    {
        private readonly IRepository<Customer, Guid> _customerRepository;
        private readonly IRepository<Vendor, Guid> _vendorRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Tax, long> _taxRepository;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;
        private readonly IRepository<Item, Guid> _itemRepository;
        private readonly IRepository<Property, long> _itemPropertyRepository;
        private readonly IRepository<TransactionType, long> _saleTypeRepository;
        private readonly IRepository<Class, long> _classRepository;
        private readonly IRepository<Location, long> _locationRepository;
        private readonly IRepository<Lot, long> _lotRepository;
        private readonly IRepository<PurchaseOrder, Guid> _purchaserOrderRepository;
        private readonly IRepository<SaleOrder, Guid> _saleOrderRepository;       
        private readonly IRepository<Journal, Guid> _journalRepository;
        private readonly IRepository<TransferOrder, Guid> _transferOrderRepository;
        private readonly IRepository<Production, Guid> _productionOrderRepository;
        private readonly IRepository<ProductionProcess, long> _productionProcessRepository;
        public CustomAuditoProvider(IRepository<Customer, Guid> customerRepository,
                                    IUnitOfWorkManager unitOfWorkManager,
                                    IRepository<Vendor, Guid> vendorRepository,
                                    IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
                                    IRepository<Tax, long> taxRepository,
                                    IRepository<Item, Guid> itemRepository,
                                    IRepository<Property, long> itemPropertyRepository,
                                    IRepository<TransactionType, long> saleTypeRepository,
                                    IRepository<Class, long> classRepository,
                                    IRepository<Location, long> locationRepository,
                                    IRepository<Lot, long> lotRepository,
                                    IRepository<PurchaseOrder, Guid> purchaserOrderRepository,
                                    IRepository<SaleOrder, Guid> saleOrderRepository,                                
                                    IRepository<Journal, Guid> journalRepository,
                                    IRepository<TransferOrder, Guid> transferOrderRepository,
                                    IRepository<Production, Guid> productionOrderRepository,
                                    IRepository<ProductionProcess, long> productionProcessRepository)
        {
            _customerRepository = customerRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _vendorRepository = vendorRepository;
            _chartOfAccountRepository = chartOfAccountRepository;
            _taxRepository = taxRepository;
            _itemRepository = itemRepository;
            _itemPropertyRepository = itemPropertyRepository;
            _saleTypeRepository = saleTypeRepository;
            _classRepository = classRepository;
            _locationRepository = locationRepository;
            _lotRepository = lotRepository;
            _purchaserOrderRepository = purchaserOrderRepository;
            _saleOrderRepository = saleOrderRepository;         
            _journalRepository = journalRepository;
            _transferOrderRepository = transferOrderRepository;
            _productionOrderRepository = productionOrderRepository;
            _productionProcessRepository = productionProcessRepository;
        }


        class JournalOutput
        {
            public DateTime Date { get; set; }
            public string JournalNo { get; set; }
            public string Reference { get; set; }
        }

            public override void Fill(AuditInfo auditInfo)
        {
            var servinceName = auditInfo.ServiceName;
            var method = auditInfo.MethodName;
            var tenantId = auditInfo.TenantId;
            var userId = auditInfo.UserId;

            
            var journalAppServices = new List<string>
            {  "CorarlERP.Bills.BillAppService",
               "CorarlERP.InvoiceSales.InvoiceSaleAppService",
               "CorarlERP.Deposits.DepositAppService",
               "CorarlERP.Withdraws.WithdrawAppService",
               "CorarlERP.VendorCredit.VendorCreditAppService",
               "CorarlERP.CustomerCredits.CustomerCreditAppService",
               "CorarlERP.ItemIssueAdjustments.ItemIssueAdjustmentAppService",
               "CorarlERP.ItemIssueOthers.ItemIssueOtherAppService",
               "CorarlERP.ItemIssueProducts.ItemIssueProductionAppService",
               "CorarlERP.ItemIssues.ItemIssueAppService",
               "CorarlERP.ItemIssueTransfers.ItemIssueTransferAppService",
               " CorarlERP.ItemIssueVendorCredits.ItemIssueVendorCreditAppService",
               "CorarlERP.ItemReceiptAdjustments.ItemReceiptAdjustmentAppService",
               "CorarlERP.ItemReceiptCustomerCredits.ItemReceiptCustomerCreditAppService",
               "CorarlERP.ItemReceiptOthers.ItemReceiptOtherAppService",
               "CorarlERP.ItemReceiptProducts.ItemReceiptProdcutionAppService",
               "CorarlERP.ItemReceipts.ItemReceiptAppService",
               "CorarlERP.ItemReceiptTransfers.ItemReceiptTransferAppService",
               "CorarlERP.Journals.GeneralJournalAppService",
               "CorarlERP.PayBills.PayBillAppService",
               "CorarlERP.ReceivePayments.ReceivePaymentAppService"
            };

            var journalNoLabelDic = new Dictionary<string, string>
                {

                { "CorarlERP.Bills.BillAppService","Bill No" },
                { "CorarlERP.InvoiceSales.InvoiceSaleAppService","Invoice No" },
                { "CorarlERP.Deposits.DepositAppService","Deposit No" },
                { "CorarlERP.Withdraws.WithdrawAppService","Withdraw No"},
                { "CorarlERP.VendorCredit.VendorCreditAppService","Vendor Credit No" },
                { "CorarlERP.CustomerCredits.CustomerCreditAppService","Customer Credit No" },
                { "CorarlERP.ItemIssueAdjustments.ItemIssueAdjustmentAppService" ,"Item Issue Adjustment No"},
                { "CorarlERP.ItemIssueOthers.ItemIssueOtherAppService" ,"ItemIssueNo"},
                { "CorarlERP.ItemIssueProducts.ItemIssueProductionAppService","Item Issue Production No" },
                { "CorarlERP.ItemIssues.ItemIssueAppService","Item Issue No" },
                { "CorarlERP.ItemIssueTransfers.ItemIssueTransferAppService","Item Issue Sale No" },
                { " CorarlERP.ItemIssueVendorCredits.ItemIssueVendorCreditAppService","Item Issue Vendor Credit No" },
                { "CorarlERP.ItemReceiptAdjustments.ItemReceiptAdjustmentAppService" ,"Item Receipt Adjustment No"},
                { "CorarlERP.ItemReceiptCustomerCredits.ItemReceiptCustomerCreditAppService" ,"Item Receipt Customer Credit No"},
                { "CorarlERP.ItemReceiptOthers.ItemReceiptOtherAppService","Item Receipt Other No" },
                { "CorarlERP.ItemReceiptProducts.ItemReceiptProdcutionAppService","ItemReceipt Production No" },
                { "CorarlERP.ItemReceipts.ItemReceiptAppService","Item Receipt Purchase No" },
                { "CorarlERP.ItemReceiptTransfers.ItemReceiptTransferAppService","Item Receipt Transfer No" },
                { "CorarlERP.Journals.GeneralJournalAppService","Journal No" },
                { "CorarlERP.PayBills.PayBillAppService" ,"Pay Bill No"},
                { "CorarlERP.ReceivePayments.ReceivePaymentAppService","Payment No"}};

            //test this sith customer
            #region customer
            if (servinceName == "CorarlERP.Customers.CustomerAppService" && (method == "Delete" ||  method == "GetDetail" || method == "Disable" || method == "Enable"))
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["id"].ToString();
                auditInfo.CustomData = GetCustomerCode(Guid.Parse(id), tenantId, userId);
            }
            else if (servinceName == "CorarlERP.Customers.CustomerAppService" &&  (method == "Update" || method == "Create"))
            {
                var json = JObject.Parse(auditInfo.Parameters);               
                auditInfo.CustomData = $"Comstomer Code:{json["input"]["customerCode"]},Customer Name: {json["input"]["customerName"]}";
            }
           
            else if (servinceName == "CorarlERP.Customers.CustomerAppService" && method == "GetList")
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["filter"].ToString();
                auditInfo.CustomData = $"Filter:{id}";
            }
            #endregion

            #region Vendor
            else if (servinceName == "CorarlERP.Vendors.VendorAppService" && (method == "Delete" || method == "GetDetail" || method == "Disable" || method == "Enable"))
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["id"].ToString();
                auditInfo.CustomData = GetVendorCode(Guid.Parse(id), tenantId, userId);
            }

            else if (servinceName == "CorarlERP.Vendors.VendorAppService" && (method == "Update" || method == "Create"))
            {
                var json = JObject.Parse(auditInfo.Parameters);
                auditInfo.CustomData = $"Vendor Code:{json["input"]["vendorCode"]},Vendor Name: {json["input"]["vendorName"]}";
            }

            else if (servinceName == "CorarlERP.Vendors.VendorAppService" && method == "GetList")
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["filter"].ToString();
                auditInfo.CustomData = $"Filter:{id}";
            }

            #endregion

            #region chart of account
            else if (servinceName == "CorarlERP.ChartOfAccounts.ChartOfAccountAppService" && (method == "Delete" || method == "Disable" || method == "Enable" || method == "GetDetail"))
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["id"].ToString();
                auditInfo.CustomData = GetChartOfAccountCode(Guid.Parse(id), tenantId, userId);
            }
           
            else if (servinceName == "CorarlERP.ChartOfAccounts.ChartOfAccountAppService" && ( method == "Create" || method == "Update"))
            {
                var json = JObject.Parse(auditInfo.Parameters);       
                auditInfo.CustomData = $"Account Code:{json["input"]["accountCode"]},Account Name: {json["input"]["accountName"]}";
            }
           
            else if (servinceName == "CorarlERP.ChartOfAccounts.ChartOfAccountAppService" && method == "GetList")
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["filter"].ToString();
                auditInfo.CustomData = $"Filter:{id}";
            }

            #endregion

            #region tax
            else if (servinceName == "CorarlERP.Taxes.TaxAppService" && (method == "Delete" || method == "Disable" || method == "Enable" || method == "GetDetail"))
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["id"].ToString();
                auditInfo.CustomData = GetTaxCode(long.Parse(id), tenantId, userId);
            }
            else if (servinceName == "CorarlERP.Taxes.TaxAppService" && (method == "Update" || method == "Create"))
            {
                var json = JObject.Parse(auditInfo.Parameters);              
                auditInfo.CustomData = $"Tax Name :{json["input"]["taxName"]}";
            }
            
            else if (servinceName == "CorarlERP.Taxes.TaxAppService" && method == "GetList")
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["filter"].ToString();
                auditInfo.CustomData = auditInfo.CustomData = $"Filter:{id}"; 
            }
            #endregion

            #region ItemAppService

            else if (servinceName == "CorarlERP.Items.ItemAppService" && (method == "Delete" || method == "Disable" || method == "Enable" || method == "GetDetail"))
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["id"].ToString();
                auditInfo.CustomData = GetItemCode(Guid.Parse(id), tenantId, userId);
            }
            
            else if (servinceName == "CorarlERP.Items.ItemAppService" && (method == "Create" || method == "UpdateAsync"))
            {
                var json = JObject.Parse(auditInfo.Parameters);              
                auditInfo.CustomData = $"Item Code:{json["input"]["itemCode"]},Item Name :{json["input"]["itemName"]}"; 
            }
           
            else if (servinceName == "CorarlERP.Items.ItemAppService" && method == "GetList")
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["filter"].ToString();
                auditInfo.CustomData = auditInfo.CustomData = $"Filter:{id}"; 
            }

            #endregion

            #region PropertyAppService

            else if (servinceName == "CorarlERP.PropertyValues.PropertyAppService" && (method == "Delete" || method == "Disable" || method == "Enable" || method == "GetDetail"))
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["id"].ToString();
                auditInfo.CustomData = GetItemPropertyCode(long.Parse(id), tenantId, userId);
            }
            else if (servinceName == "CorarlERP.PropertyValues.PropertyAppService" && (method == "Update" || method == "Create"))
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var name = json["input"]["name"].ToString();
                auditInfo.CustomData = $"Property Name:{name}";
            }
                      
            else if (servinceName == "CorarlERP.PropertyValues.PropertyAppService" && method == "GetList")
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["filter"].ToString();
                auditInfo.CustomData = auditInfo.CustomData = $"Filter:{id}"; 
            }
            #endregion

            #region SaleTypeAppService

            else if (servinceName == "CorarlERP.Transactiontypes.TransactionTypeAppSevice" && (method == "Delete" || method == "Disable" || method == "Enable" || method == "GetDetail"))
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["id"].ToString();
                auditInfo.CustomData = GetSaleType(long.Parse(id), tenantId, userId);
            }          
            else if (servinceName == "CorarlERP.Transactiontypes.TransactionTypeAppSevice" && (method == "Create" || method == "Update"))
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var saleType = json["input"]["transactionTypeName"].ToString();
                auditInfo.CustomData = $"Sale Type:{saleType}";
            }
           
            else if (servinceName == "CorarlERP.Transactiontypes.TransactionTypeAppSevice" && method == "GetList")
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["filter"].ToString();
                auditInfo.CustomData = auditInfo.CustomData = $"Filter:{id}"; 
            }

            #endregion
            
            #region classAppService

            else if (servinceName == "CorarlERP.Classes.ClassAppService" && (method == "Delete" || method == "Disable" || method == "Enable" || method == "GetDetail"))
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["id"].ToString();
                auditInfo.CustomData = GetClass(long.Parse(id), tenantId, userId);
            }           
            else if (servinceName == "CorarlERP.Classes.ClassAppService" && (method == "Create" || method == "Update"))
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var name = json["input"]["className"].ToString();
                auditInfo.CustomData = $"Class Name: {name}";
            }
           
            else if (servinceName == "CorarlERP.Classes.ClassAppService" && method == "GetList")
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["filter"].ToString();
                auditInfo.CustomData = auditInfo.CustomData = $"Filter:{id}"; ;
            }

            #endregion

            #region locationAppService

            else if (servinceName == "CorarlERP.Locations.LocationAppService" && (method == "Delete" || method == "Disable" || method == "Enable" || method == "GetDetail"))
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["id"].ToString();
                auditInfo.CustomData = GetLocation(long.Parse(id), tenantId, userId);
            }
            
            else if (servinceName == "CorarlERP.Locations.LocationAppService" && (method == "Create" || method == "Update"))
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var name = json["input"]["locationName"].ToString();
                auditInfo.CustomData = $"Location Name: {name}";
            }
           
            else if (servinceName == "CorarlERP.Locations.LocationAppService" && method == "GetList")
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["filter"].ToString();
                auditInfo.CustomData = auditInfo.CustomData = $"Filter:{id}"; ;
            }

            #endregion

            #region lotAppService

            else if (servinceName == "CorarlERP.Lots.LotAppService" && (method == "Delete" || method == "Disable" || method == "Enable" || method == "GetDetail"))
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["id"].ToString();
                auditInfo.CustomData = GetLot(long.Parse(id), tenantId, userId);
            }
           
            else if (servinceName == "CorarlERP.Lots.LotAppService" && (method == "Create" || method == "Update"))
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var name = json["input"]["lotName"].ToString();
                auditInfo.CustomData = $"Lot Name:{name}";
            }
            
            else if (servinceName == "CorarlERP.Lots.LotAppService" && method == "GetList")
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["filter"].ToString();
                auditInfo.CustomData = auditInfo.CustomData = $"Filter:{id}"; ;
            }

            #endregion

            #region proccessAppService

            else if (servinceName == "CorarlERP.ProductionProcesses.ProductionProcessAppService" && (method == "Delete" || method == "Disable" || method == "Enable" || method == "GetDetail"))
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["id"].ToString();
                auditInfo.CustomData = GetProductionProcess(long.Parse(id), tenantId, userId);
            }

            else if (servinceName == "CorarlERP.ProductionProcesses.ProductionProcessAppService" && (method == "Create" || method == "Update"))
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var name = json["input"]["processName"].ToString();
                auditInfo.CustomData = $"Process Name:{name}";
            }

            else if (servinceName == "CorarlERP.ProductionProcesses.ProductionProcessAppService" && method == "GetList")
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["filter"].ToString();
                auditInfo.CustomData = auditInfo.CustomData = $"Filter:{id}";
            }

            #endregion

            #region purchaserOrderAppService

            else if (servinceName == "CorarlERP.PurchaseOrders.PurchaseOrderAppService" && (method == "Delete" || method == "Update" || method == "GetDetail"))
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["id"].ToString();
                auditInfo.CustomData = GetPurchaserOrder(Guid.Parse(id), tenantId, userId);
            }
           
            else if (servinceName == "CorarlERP.PurchaseOrders.PurchaseOrderAppService" && method == "Create")
            {
                var json = JObject.Parse(auditInfo.Parameters);
                if (auditInfo.Parameters == "{}")
                {
                    auditInfo.CustomData = "";
                }
                else 
                { 
                    auditInfo.CustomData = $"Date:{json["input"]["orderDate"]}, Reference: {json["input"]["reference"]}";
                }               
            }
            else if (servinceName == "CorarlERP.PurchaseOrders.PurchaseOrderAppService" && method == "GetList")
            {
                var json = JObject.Parse(auditInfo.Parameters);            
                auditInfo.CustomData = $"From Date:{json["input"]["fromDate"]}, To Date: {json["input"]["toDate"]}, Filter: {json["input"]["filter"]}";
            }
            #endregion

            #region SaleOrderAppService

            else if (servinceName == "CorarlERP.SaleOrders.SaleOrderAppService" && (method == "Delete" || method == "Update" || method == "GetDetail"))
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["id"].ToString();
                auditInfo.CustomData = GetSaleOrder(Guid.Parse(id), tenantId, userId);
            }           
            else if (servinceName == "CorarlERP.SaleOrders.SaleOrderAppService" && method == "Create")
            {
                var json = JObject.Parse(auditInfo.Parameters);
                auditInfo.CustomData = $"Date:{json["input"]["orderDate"]}, Reference: {json["input"]["reference"]}";
            }
            else if (servinceName == "CorarlERP.SaleOrders.SaleOrderAppService" && method == "GetList")
            {
                var json = JObject.Parse(auditInfo.Parameters);              
                auditInfo.CustomData = $"From Date:{json["input"]["fromDate"]}, To Date: {json["input"]["toDate"]}, Filter: {json["input"]["filter"]}";
            }

            #endregion

            #region TransferOrdersAppService

            else if (servinceName == "CorarlERP.TransferOrders.TransferOrderAppService" && (method == "Delete" || method == "Update" || method == "GetDetail"))
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["id"].ToString();
                auditInfo.CustomData = GetTrasferOrder(Guid.Parse(id), tenantId, userId);
            }
           
            else if (servinceName == "CorarlERP.TransferOrders.TransferOrderAppService" && method == "Create")
            {
                var json = JObject.Parse(auditInfo.Parameters);
                auditInfo.CustomData = $"Date:{json["input"]["date"]}, Reference: {json["input"]["reference"]}";
            }

            else if (servinceName == "CorarlERP.TransferOrders.TransferOrderAppService" && method == "GetList")
            {
                var json = JObject.Parse(auditInfo.Parameters);
                auditInfo.CustomData = $"From Date:{json["input"]["fromDate"]}, To Date: {json["input"]["toDate"]}, Filter: {json["input"]["filter"]}";
            }

            #endregion

            #region ProductionOrdersAppService

            else if (servinceName == "CorarlERP.Productions.ProductionAppService" && (method == "Delete" || method == "Update" || method == "GetDetail"))
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["id"].ToString();
                auditInfo.CustomData = GetProductionOrder(Guid.Parse(id), tenantId, userId);
            }          
            else if (servinceName == "CorarlERP.Productions.ProductionAppService" && method == "Create")
            {
                var json = JObject.Parse(auditInfo.Parameters);
                auditInfo.CustomData = $"Date:{json["input"]["date"]}, Reference: {json["input"]["reference"]}";
            }

            else if (servinceName == "CorarlERP.Productions.ProductionAppService" && method == "GetList")
            {
                var json = JObject.Parse(auditInfo.Parameters);
                auditInfo.CustomData = $"From Date:{json["input"]["fromDate"]}, To Date: {json["input"]["toDate"]}, Filter: {json["input"]["filter"]}";
            }

            #endregion

            #region journal 
            else if (journalAppServices.Contains(servinceName) && (method == "Delete" || method == "GetDetail" ||  method == "Update"))
            {
                var json = JObject.Parse(auditInfo.Parameters);
                var id = json["input"]["id"].ToString();

                var journalObject = GetJournal(Guid.Parse(id), tenantId, userId);

                var JournalNoLabel = journalNoLabelDic[servinceName];

                auditInfo.CustomData = $"Date:{journalObject?.Date}, {JournalNoLabel}:{ journalObject?.JournalNo}, Reference: {journalObject?.Reference}";
            }
            
            else if (journalAppServices.Contains(servinceName) && method == "Create")
            {
                var json = JObject.Parse(auditInfo.Parameters);

                var date = json["input"]["date"];

                if (journalAppServices.Contains("CorarlERP.PayBills.PayBillAppService") || journalAppServices.Contains("CorarlERP.ReceivePayments.ReceivePaymentAppService"))
                {
                    date = json["input"]["paymentDate"]; 
                }
                auditInfo.CustomData = $"Date:{date}, Reference: {json["input"]["reference"]}";
                
            }

            else if (journalAppServices.Contains(servinceName) && method == "GetList")
            {
                var json = JObject.Parse(auditInfo.Parameters);
                auditInfo.CustomData = $"From Date:{json["input"]["fromDate"]}, To Date: {json["input"]["toDate"]}, Filter: {json["input"]["filter"]}";
            }
            else if (method.Contains("Export") || method.Contains("Import"))
            {
                auditInfo.CustomData = "";
            }

            #endregion

            #region Other
            else
            {
                var json = JObject.Parse(auditInfo.Parameters);
                if (json["input"] != null && json["input"]["fromDate"] != null && json["input"]["toDate"] != null)
                    auditInfo.CustomData = $"From Date:{json["input"]["fromDate"]}, To Date: {json["input"]["toDate"]}, Filter: {json["input"]["filter"]}";
            }
            #endregion

            base.Fill(auditInfo);
        }

        private string GetCustomerCode (Guid id, int? tenantId, long? userId)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                 var customer  =   _customerRepository.GetAll().Where(u => u.CreatorUserId == userId &&
                                                              u.Id == id).Select(u => new { cusotmerCode = u.CustomerCode, customerName = u.CustomerName, }).FirstOrDefault();

                    return $"Customer Code:{customer?.cusotmerCode}, Customer Name {customer?.customerName} ";
                    
                }
            }
        }

        private string GetVendorCode(Guid id, int? tenantId, long? userId)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var vendor = _vendorRepository.GetAll().Where(u => u.CreatorUserId == userId &&
                                                            u.Id == id).Select(u => new { vendorCode = u.VendorCode, vendorName = u.VendorName, }).FirstOrDefault();

                    return $"Vendor Code:{vendor?.vendorCode}, Vendor Name {vendor?.vendorName} ";

                }
            }
        }

        private string GetChartOfAccountCode(Guid id, int? tenantId, long? userId)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                 var chartofAccount  =   _chartOfAccountRepository.GetAll().Where(u => u.CreatorUserId == userId &&
                                                              u.Id == id).Select(u => new { accountCode = u.AccountCode , accountName = u.AccountName }).FirstOrDefault();
                    return $"Account Code:{chartofAccount?.accountCode}, Account Name {chartofAccount?.accountName} ";
                }
            }
        }

        private string GetTaxCode(long id, int? tenantId, long? userId)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                   
                   var tax =     _taxRepository.GetAll().Where(u => u.CreatorUserId == userId &&
                                                              u.Id == id).Select(u => u.TaxName).FirstOrDefault();
                    return $"Tax Name:{tax}";

                }
            }
        }

        private string GetItemCode(Guid id, int? tenantId, long? userId)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    
                    var item = _itemRepository.GetAll().Where(u => u.CreatorUserId == userId &&
                                                              u.Id == id).Select(u => new { itemCode= u.ItemCode,itemName = u.ItemName }).FirstOrDefault();
                    return $"Item Code:{item?.itemCode}, Item Name {item?.itemName}";

                }
            }
        }

        private string GetItemPropertyCode(long id, int? tenantId, long? userId)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                   

                     var property=   _itemPropertyRepository.GetAll().Where(u => u.CreatorUserId == userId &&
                                                              u.Id == id).Select(u => u.Name).FirstOrDefault();
                    return $"Property Name:{property}";
                }
            }
        }

        private string GetSaleType(long id, int? tenantId, long? userId)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var saleType = _saleTypeRepository.GetAll().Where(u => u.CreatorUserId == userId &&
                                                              u.Id == id).Select(u => u.TransactionTypeName).FirstOrDefault();
                    return $"Sale Type Name:{saleType}";
                }
            }
        }

        private string GetClass(long id, int? tenantId, long? userId)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var classes= _classRepository.GetAll().Where(u => u.CreatorUserId == userId &&
                                                              u.Id == id).Select(u => u.ClassName).FirstOrDefault();
                    return $"Class Name: {classes}";
                }
            }
        }

        private string GetLocation(long id, int? tenantId, long? userId)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                   var location= _locationRepository.GetAll().Where(u => u.CreatorUserId == userId &&
                                                              u.Id == id).Select(u => u.LocationName).FirstOrDefault();
                    return $"Location Name:{location}";
                }
            }
        }

        private string GetLot(long id, int? tenantId, long? userId)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var lot= _lotRepository.GetAll().Where(u => u.CreatorUserId == userId &&
                                                              u.Id == id).Select(u => u.LotName).FirstOrDefault();
                    return $"Lot Name:{lot}";
                }
            }
        }
        private string GetProductionProcess(long id, int? tenantId, long? userId)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var pro= _productionProcessRepository.GetAll().Where(u => u.CreatorUserId == userId &&
                                                              u.Id == id).Select(u => u.ProcessName).FirstOrDefault();
                    return $"Pross Name:{pro}";
                }
            }
        }

        private string GetPurchaserOrder(Guid id, int? tenantId, long? userId)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                   
                   var purchase =   _purchaserOrderRepository.GetAll().Where(u => u.CreatorUserId == userId &&
                                                              u.Id == id).Select(u => new {date= u.OrderDate,reference = u.Reference,purchaseNo = u.OrderNumber }).FirstOrDefault();
                    return $"Date:{purchase?.date},Purchase Order No:{purchase?.purchaseNo},Reference:{purchase?.reference}";
                }
            }
        }

        private string GetSaleOrder(Guid id, int? tenantId, long? userId)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var saleorder = _saleOrderRepository.GetAll().Where(u => u.CreatorUserId == userId &&
                                                            u.Id == id).Select(u => new { date = u.OrderDate, reference = u.Reference, saleNo = u.OrderNumber }).FirstOrDefault();
                    return $"Date:{saleorder.date},Sale Order No:{saleorder.saleNo},Reference:{saleorder.reference}";
                }
            }
        }


        private JournalOutput GetJournal(Guid id, int? tenantId, long? userId)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    return _journalRepository.GetAll()
                           .Where(u => 
                           (u.Id == id 
                           || u.InvoiceId == id || u.BillId == id 
                           || u.ItemIssueVendorCreditId == id || u.ItemReceiptCustomerCreditId == id
                           || u.VendorCreditId == id || u.CustomerCreditId == id
                           || u.ItemReceiptId == id || u.ItemIssueId == id 
                           || u.DepositId == id || u.WithdrawId == id
                           || u.PayBillId == id || u.ReceivePaymentId == id 

                           )).Select(u => new JournalOutput{
                                Date = u.Date,
                                JournalNo = u.JournalNo,
                                Reference = u.Reference
                           }).FirstOrDefault();
                }
            }
        }

        private string GetProductionOrder(Guid id, int? tenantId, long? userId)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var productionOrder = _productionOrderRepository.GetAll().Where(u => u.CreatorUserId == userId &&
                                                           u.Id == id).Select(u => new { date = u.Date, reference = u.Reference, saleNo = u.ProductionNo }).FirstOrDefault();
                    return $"Date:{productionOrder.date},Production Order No:{productionOrder.saleNo},Reference:{productionOrder.reference}";
                }
            }
        }
      
        private string GetTrasferOrder(Guid id, int? tenantId, long? userId) 
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var transferOrder = _transferOrderRepository.GetAll().Where(u => u.CreatorUserId == userId &&
                                                          u.Id == id).Select(u => new { date = u.TransferDate, reference = u.Reference, saleNo = u.TransferNo }).FirstOrDefault();
                    return $"Date:{transferOrder.date},Transfer Order No:{transferOrder.saleNo},Reference:{transferOrder.reference}";
                }
            }           
        }

    }
}
