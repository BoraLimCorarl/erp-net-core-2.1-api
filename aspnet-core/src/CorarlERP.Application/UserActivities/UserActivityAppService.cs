using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using CorarlERP.Auditing;
using CorarlERP.Authorization;
using CorarlERP.Authorization.Users;
using CorarlERP.Dto;
using CorarlERP.Reports;
using CorarlERP.Reports.Dto;
using CorarlERP.ReportTemplates;
using CorarlERP.UserActivities.Dto;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using CorarlERP.FileStorages;

namespace CorarlERP.UserActivities
{
    [AbpAuthorize]
    [DisableAuditing]
    public class UserActivityAppService : ReportBaseClass, IUserActivityAppService
    {

        protected readonly IRepository<AuditLog, long> _auditLogRepository;
        private readonly AppFolders _appFolders;
        private readonly IRepository<User, long> _userRepository;
        private readonly INamespaceStripper _namespaceStripper;
        private readonly IFileStorageManager _fileStorageManager;
        public UserActivityAppService(IRepository<AuditLog, long> auditLogRepository, IRepository<User, long> userRepository, IFileStorageManager fileStorageManager,
                                      INamespaceStripper namespaceStripper, AppFolders appFolders):base(null,appFolders, null, null)
        {
            _auditLogRepository = auditLogRepository;
            _userRepository = userRepository;
            _namespaceStripper = namespaceStripper;
            _appFolders = appFolders;
            _fileStorageManager = fileStorageManager;
        }

        public PagedResultDto<UserActivityOutput> FindUserActivity(GetUserActivityInput input)
        {

            var querys = this.GetActivityList();

            var resultCount = querys.Count;
            return new PagedResultDto<UserActivityOutput>(resultCount, ObjectMapper.Map<List<UserActivityOutput>>(querys));
        }

      
        protected List<UserActivityOutput> GetActivityList()
        {
            return new List<UserActivityOutput>()
                {
                 new UserActivityOutput(){Id=1,  Key ="Create",Value = this.L("Create")},
                 new UserActivityOutput(){Id=2,  Key ="Update",Value = this.L("Update")},
                 new UserActivityOutput(){Id=3,  Key ="GetList",Value = this.L("GetList")},
                 new UserActivityOutput(){Id=5,  Key ="GetDetail",Value = this.L("GetDetail")},
                 new UserActivityOutput(){Id=5,  Key ="Delete",Value = this.L("Delete")},
                 new UserActivityOutput(){Id=6,  Key ="ImportExcel",Value = this.L("ImportExcel")},
                 new UserActivityOutput(){Id=7,  Key ="ExportExcel",Value = this.L("ExportExcel")},
                 new UserActivityOutput(){Id=8,  Key ="ExportPdf",Value = this.L("ExportPdf")},

                 new UserActivityOutput(){Id=9,  Key ="GetIncomeReport",Value = this.L("ViewProfitLoss")},
                 new UserActivityOutput(){Id=10, Key ="GetBalanceSheetReport",Value = this.L("ViewBalanceSheet")},
                 new UserActivityOutput(){Id=11, Key ="GetListJournalReport",Value = this.L("ViewJournalReport")},
                 new UserActivityOutput(){Id=12, Key ="GetListLedgerReport",Value = this.L("ViewLedgerReport")},
                 new UserActivityOutput(){Id=13, Key ="GetInventoryReport",Value = this.L("ViewInventorySummaryReport")},
                 new UserActivityOutput(){Id=14, Key ="GetStockBalanceReport",Value = this.L("ViewStockBalanceReport")},
                 new UserActivityOutput(){Id=15, Key ="GetInventoryTransactionReport",Value = this.L("ViewInventoryTransactionReport")},
                 new UserActivityOutput(){Id=16, Key ="GetInventoryValuationDetailReport",Value = this.L("ViewInventoryValuationDetailReport")},
                 new UserActivityOutput(){Id=17, Key ="GetSaleInvoiceReport",Value = this.L("ViewSaleInvoiceReport")},
                 new UserActivityOutput(){Id=18, Key ="GetCustomerAgingReport",Value = this.L("ViewCustomerAgingReport")},
                 new UserActivityOutput(){Id=19, Key ="GetCustomerByInvoiceReport",Value = this.L("ViewCustomerByInvoiceReport")},
                 new UserActivityOutput(){Id=20, Key ="GetBillReport",Value = this.L("ViewBillReport")},
                 new UserActivityOutput(){Id=21, Key ="GetVendorAgingReport",Value = this.L("ViewVendorAgingReport")},
                 new UserActivityOutput(){Id=22, Key ="GetVendorByBillReport",Value = this.L("ViewVendorByBillReport")},
                 new UserActivityOutput(){Id=23, Key = "Print",Value = this.L("Print")},
                 new UserActivityOutput(){Id=24, Key = "PayMore",Value = this.L("PayMore")},
                  
                };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_UserActivities_FindTrasaction)]
        public PagedResultDto<GetListActivityOutput> FindUserTtransaction(GetUserActivityInput input)
        {
            var querys = this.GetTransactionList();

            var resultCount = querys.Count;
            return new PagedResultDto<GetListActivityOutput>(resultCount, ObjectMapper.Map<List<GetListActivityOutput>>(querys));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_UserActivities_FindActivity)]
        protected List<GetListActivityOutput> GetTransactionList()
        {
            return new List<GetListActivityOutput>()
                {
                new GetListActivityOutput(){ Id=1,  Key ="WithdrawAppService",Value = this.L("Withdraw"),IsReport = false},
                new GetListActivityOutput(){ Id=3,  Key="ReceivePaymentAppService",Value = this.L("ReceivePayments"),IsReport = false},
                new GetListActivityOutput(){ Id=4,  Key="PurchaseOrderAppService",Value= this.L("PurchaseOrders"),IsReport = false},
                new GetListActivityOutput(){ Id=5,  Key="SaleOrderAppService",Value = this.L("SaleOrders"),IsReport = false},
                new GetListActivityOutput(){ Id=6,  Key="TaxAppService",Value = this.L("Taxs"),IsReport = false},
                new GetListActivityOutput(){ Id=7,  Key="TransactionTypeAppSevice",Value = this.L("SaleTypes"),IsReport = false},
                new GetListActivityOutput(){ Id=8,  Key="TransferOrderAppService",Value = this.L("TransferOrders"),IsReport = false},
                new GetListActivityOutput(){ Id=9,  Key="UserGroupAppService",Value = this.L("GroupUsers"),IsReport = false},
                new GetListActivityOutput(){ Id=10, Key="VendorCreditAppService",Value = this.L("VendorCredits"),IsReport = false},
                new GetListActivityOutput(){ Id=11, Key="VendorAppService",Value = this.L("Vendors"),IsReport = false},
                new GetListActivityOutput(){ Id=12, Key="VendorTypeAppService",Value = this.L("VendorTypes"),IsReport = false},
                new GetListActivityOutput(){ Id=13, Key="PropertyAppService",Value = this.L("Property"),IsReport = false},
                new GetListActivityOutput(){ Id=14, Key="ProductionAppService",Value = this.L("ProductionOrders"),IsReport = false},
                new GetListActivityOutput(){ Id=15, Key="ProductionProcessAppService",Value = this.L("ProductionProcess"),IsReport = false},
                new GetListActivityOutput(){ Id=16, Key="PhysicalCountAppService",Value = this.L("PhysicalCounts"),IsReport = false},
                new GetListActivityOutput(){ Id=17, Key="PayBillAppService",Value = this.L("PayBills"),IsReport = false},
                //new GetListActivityOutput(){ Id=18, Key="PartnerAppService",Value = this.L("Partner"),IsReport = false},
                new GetListActivityOutput(){ Id=19, Key="MultiCurrencyAppService",Value = this.L("MultiCurrency"),IsReport = false},
                new GetListActivityOutput(){ Id=20, Key="LotAppService",Value = this.L("Lots"),IsReport = false},
               // new GetListActivityOutput(){ Id=21, Key="LockAppService",Value =this.L("LockTransaction"),IsReport = false},
                new GetListActivityOutput(){ Id=22, Key ="LocationAppService",Value = this.L("Locations"),IsReport = false},
                new GetListActivityOutput(){ Id=23, Key="GeneralJournalAppService",Value = this.L("GeneralJournal"),IsReport = false},             
                new GetListActivityOutput(){ Id=25, Key="ItemAppService",Value = this.L("Items"),IsReport = false},


 
                new GetListActivityOutput(){ Id=26, Key="ItemReceiptTransferAppService",Value = this.L("ItemReceipt_Transfer"),IsReport = false},
                new GetListActivityOutput(){ Id=27, Key="ItemReceiptAppService",Value = this.L("ItemReceipt_PuchaseOrder"),IsReport = false},
                new GetListActivityOutput(){ Id=28, Key="ItemReceiptProdcutionAppService",Value = this.L("ItemReceipt_ProductionOrder"),IsReport = false},
                new GetListActivityOutput(){ Id=29, Key="ItemReceiptOtherAppService",Value = this.L("ItemReceipt_Other"),IsReport = false},
                new GetListActivityOutput(){ Id=30, Key="ItemReceiptCustomerCreditAppService",Value = this.L("ItemReceipt_CustomerCredit"),IsReport = false},
                new GetListActivityOutput(){ Id=31, Key="ItemReceiptAdjustmentAppService",Value = this.L("ItemReceipt_Adjustment"),IsReport = false},

                new GetListActivityOutput(){ Id=32, Key="ItemIssueVendorCreditAppService",Value = this.L("ItemIssue_VendorCredit"),IsReport = false},
                new GetListActivityOutput(){ Id=33, Key="ItemIssueTransferAppService",Value = this.L("ItemIssue_Transfer"),IsReport = false},
                new GetListActivityOutput(){ Id=34, Key="ItemIssueAppService",Value = this.L("ItemIssue_SaleOrder"),IsReport = false},
                new GetListActivityOutput(){ Id=35, Key="ItemIssueProductionAppService",Value = this.L("ItemIssue_ProductionOrder"),IsReport = false},
                new GetListActivityOutput(){ Id=36, Key="ItemIssueOtherAppService",Value = this.L("ItemIssue_Other"),IsReport = false},
                new GetListActivityOutput(){ Id=37, Key="ItemIssueAdjustmentAppService",Value = this.L("ItemIssue_Adjustment"),IsReport = false},

                new GetListActivityOutput(){ Id=38, Key="InvoiceSaleAppService",Value = this.L("InvoiceSale"),IsReport = false},
                new GetListActivityOutput(){ Id=39, Key="InventoryTransactionAppService",Value = this.L("InventoryTransaction"),IsReport = false},

                new GetListActivityOutput(){ Id=40, Key="DepositAppService",Value = this.L("Deposit"),IsReport = false},
                new GetListActivityOutput(){ Id=41, Key="CustomerTypeAppService",Value = this.L("CustomerTypes"),IsReport = false},
                new GetListActivityOutput(){ Id=42, Key="CustomerAppService",Value = this.L("Customers"),IsReport = false},
                new GetListActivityOutput(){ Id=43, Key="CustomerCreditAppService",Value = this.L("CustomerCredits"),IsReport = false},
                new GetListActivityOutput(){ Id=44, Key="CurrencyAppService",Value = this.L("Currencies"),IsReport = false},
                new GetListActivityOutput(){ Id=45, Key="ClassAppService",Value = this.L("Classes"),IsReport = false},
                new GetListActivityOutput(){ Id=46, Key="ChartOfAccountAppService",Value = this.L("ChartOfAccounts"),IsReport = false},
                new GetListActivityOutput(){ Id=47, Key="BillAppService",Value = this.L("Bills"),IsReport = false},
                new GetListActivityOutput(){ Id=48, Key="BankTransferAppService",Value = this.L("BankTransfers"),IsReport = false},
                new GetListActivityOutput(){ Id=49, Key="BankTrasactionAppService",Value = this.L("BankTransactions"),IsReport = false},

                new GetListActivityOutput(){ Id=50, Key="ReportAccountingAppService.GetIncomeReport",Value = this.L("IncomeReport"),IsReport = true},
                new GetListActivityOutput(){ Id=51, Key="ReportAccountingAppService.GetBalanceSheetReport",Value = this.L("BalanceSheetReport"),IsReport = true},
                new GetListActivityOutput(){ Id=52, Key="ReportAccountingAppService.GetListJournalReport",Value = this.L("JournalReport"),IsReport = true},
                new GetListActivityOutput(){ Id=53, Key="ReportAccountingAppService.GetListLedgerReport",Value = this.L("LedgerReport"),IsReport = true},
                new GetListActivityOutput(){ Id=54, Key="ReportAppService.GetInventoryReport",Value = this.L("InventoryValuationSummaryReport"),IsReport = true},
                new GetListActivityOutput(){ Id=55, Key="ReportAppService.GetStockBalanceReport",Value = this.L("StockBalanceReport"),IsReport = true},
                new GetListActivityOutput(){ Id=56, Key="ReportAppService.GetInventoryTransactionReport",Value = this.L("InventoryTransactionReport"),IsReport = true},
                new GetListActivityOutput(){ Id=57, Key="ReportAppService.GetInventoryValuationDetailReport",Value =this.L( "InventoryValuationDetailReport"),IsReport = true},
                new GetListActivityOutput(){ Id=58, Key="ReportCustomerAppService.GetSaleInvoiceReport",Value = this.L("SaleInvoiceReport"),IsReport = true},
                new GetListActivityOutput(){ Id=59, Key="ReportCustomerAppService.GetCustomerAgingReport",Value = this.L("CustomerAgingReport"),IsReport = true},
                new GetListActivityOutput(){ Id=60, Key="ReportCustomerAppService.GetCustomerByInvoiceReport",Value = this.L("CustomerByInvoiceReport"),IsReport = true},
                new GetListActivityOutput(){ Id=61, Key="ReportVendorAppService.GetBillReport",Value = this.L("BillReport"),IsReport = true},
                new GetListActivityOutput(){ Id=62, Key="ReportVendorAppService.GetVendorAgingReport",Value = this.L("VendorAgingReport"),IsReport = true},
                new GetListActivityOutput(){ Id=63, Key="ReportVendorAppService.GetVendorByBillReport",Value = this.L("VendorByBillReport"),IsReport = true},
                new GetListActivityOutput(){ Id=64,Key="ExChangeAppService",Value= this.L("ExchangeRate"),IsReport=false},
                new GetListActivityOutput(){ Id=65,Key="ItemPriceAppService",Value= this.L("ItemPrice"),IsReport=false},
                new GetListActivityOutput(){ Id=66,Key="PaymentMethodAppService",Value= this.L("PaymentMethod"),IsReport=false},
                new GetListActivityOutput(){ Id=67,Key="POSAppService",Value= this.L("POS"),IsReport=false},
                
            };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_UserActivities_GetList)]
        public async Task<PagedResultDto<GetDetailListUserActivityOutput>> GetList(GetDetailListUserActivityinputput input)
        {
            var filterMethodList = this.GetActivityList().Select(s => s.Key).ToList();
            var filtertransactionList = this.GetTransactionList().Select(s=>s.Key).ToList(); 
            
            var transactionListInput = input.Transactions == null ? new List<string>() : input.Transactions.Select(s => s.Key).ToList();

            var isSeeAll = IsGranted(AppPermissions.Pages_Tenant_UserActivities_SeeAll);
           
            var userId = AbpSession.GetUserId();

            var query = from auditLog in _auditLogRepository.GetAll()
                        .Where(s => isSeeAll || s.UserId == userId )
                        .Where(s => filterMethodList.Contains(s.MethodName) || filterMethodList.Any(r => s.MethodName.StartsWith(r)) )
                        .Where(s => filtertransactionList.Contains(_namespaceStripper.StripNameSpace(s.ServiceName)) || filtertransactionList.Any(r => _namespaceStripper.StripNameSpace(s.ServiceName) + "." + s.MethodName == r))
                        .WhereIf(input.FromDate != null && input.ToDate != null, s => s.ExecutionTime.Date >= input.FromDate.Date && s.ExecutionTime.Date <= input.ToDate.Date)
                        .WhereIf(input.ErrorState != 0, s => (input.ErrorState == 1 && !s.Exception.IsNullOrEmpty()) || (input.ErrorState == 2 && s.Exception.IsNullOrEmpty()))
                        .WhereIf(input.UserIds != null && input.UserIds.Count() > 0, s => s.UserId.HasValue && input.UserIds.Contains(s.UserId.Value))
                        .WhereIf(input.Activities != null && input.Activities.Count()> 0, s => input.Activities.Contains(s.MethodName) || input.Activities.Any(r => s.MethodName.StartsWith(r)))
                        .WhereIf(transactionListInput.Count() > 0, s => transactionListInput.Contains(_namespaceStripper.StripNameSpace(s.ServiceName)) || transactionListInput.Any(r => _namespaceStripper.StripNameSpace(s.ServiceName) + "." +s.MethodName == r))
                        .AsNoTracking()
                        join user in _userRepository.GetAll()
                        .AsNoTracking()
                        on auditLog.UserId equals user.Id 
                        
                        select new GetDetailListUserActivityOutput
                        {
                            Time = auditLog.ExecutionTime,
                            Activity = auditLog.MethodName,
                            Transsaction = _namespaceStripper.StripNameSpace(auditLog.ServiceName),
                            Browser = auditLog.BrowserInfo,
                            Description = auditLog.CustomData,
                            Duration = auditLog.ExecutionDuration + "ms",
                            ErrorState = !auditLog.Exception.IsNullOrEmpty(),
                            User = user.FullName,
                            UserId = user.Id,
                        };


            var resultCount = await query.CountAsync();


            var @itemQuery = new List<GetDetailListUserActivityOutput>();
            if (input.UsePagination)
            {
                @itemQuery = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            }
            else
            {
                @itemQuery = await query.ToListAsync();
            }
            
            //Map Transaction Name
            var transactionDic = GetTransactionList().ToDictionary(s => s.Key, s => s.Value);
            var ativityDic = GetActivityList().ToDictionary(s => s.Key, s => s.Value);
            var resultItems = @itemQuery.Select(s => 
            {
                var item = s;               
                item.Transsaction = transactionDic.ContainsKey(s.Transsaction) ? transactionDic[s.Transsaction] : transactionDic.ContainsKey(s.Transsaction + "." + s.Activity) ? transactionDic[s.Transsaction + "." + s.Activity] : s.Transsaction;
                item.Activity = ativityDic.ContainsKey(s.Activity) ? ativityDic[s.Activity] : s.Activity;
                return item;
            }).ToList();


            

            return new PagedResultDto<GetDetailListUserActivityOutput>(resultCount, resultItems);
        }


        #region
        private ReportOutput GetReportTemplateUserActivity()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "ErrorState",
                        ColumnLength = 180,
                        ColumnTitle = "Error State",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Time",
                        ColumnLength = 250,
                        ColumnTitle = "Time",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "User",
                        ColumnLength = 230,
                        ColumnTitle = "User",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Activity",
                        ColumnLength = 250,
                        ColumnTitle = "Activity",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Transaction",
                        ColumnLength = 200,
                        ColumnTitle = "Transaction",
                        ColumnType = ColumnType.String,
                        SortOrder = 5,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Description",
                        ColumnLength = 250,
                        ColumnTitle = "Description",
                        ColumnType = ColumnType.String,
                        SortOrder = 6,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Duration",
                        ColumnLength = 130,
                        ColumnTitle = "Duration",
                        ColumnType = ColumnType.String,
                        SortOrder = 7,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Browsers",
                        ColumnLength = 130,
                        ColumnTitle = "Browsers",
                        ColumnType = ColumnType.String,
                        SortOrder = 8,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                },
                Groupby = "",
                HeaderTitle = "Chart Of Account",
                Sortby = "",
            };
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_UserActivities_ExportExcel)]
        public async Task<FileDto> ExportExcel(GetDetailListUserActivityinputput input)
        {
            
            var userActivityData = (await GetList(input)).Items;
            var result = new FileDto();
            var sheetName = "Chart Of Accounts";
            
            using (var p = new ExcelPackage())
            {
                //A workbook must have at least on cell, so lets add one... 
                var ws = p.Workbook.Worksheets.Add(sheetName);
                ws.PrinterSettings.Orientation = eOrientation.Landscape;
                ws.PrinterSettings.FitToPage = true;
                //ws.PrinterSettings.PaperSize = ePaperSize.A3; //set default format paper size 
                ws.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                ws.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet

                #region Row 1 Header Table
                int rowTableHeader = 1;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                var headerList = GetReportTemplateUserActivity();

                foreach (var i in headerList.ColumnInfo)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);

                    colHeaderTable += 1;
                }
                #endregion Row 1

                #region Row Body 
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;
                // write body
                foreach (var i in userActivityData)
                {
                    int collumnCellBody = 1;
                    foreach (var h in headerList.ColumnInfo)
                    {
                     WriteBodyUserActivity(ws, rowBody, collumnCellBody, h, i, count);                       
                     collumnCellBody += 1;
                    }
                    rowBody += 1;
                    count += 1;
                }
                #endregion Row Body

                result.FileName = $"UserActivity_Report.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx"; 
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }
        #endregion
    }
}
