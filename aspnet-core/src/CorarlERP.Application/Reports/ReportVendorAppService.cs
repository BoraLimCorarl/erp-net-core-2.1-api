using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.AspNetZeroCore.Net;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Amazon.EventBridge.Model.Internal.MarshallTransformations;
using CorarlERP.AccountCycles;
using CorarlERP.AccountTransactions;
using CorarlERP.Authorization;
using CorarlERP.Bills;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Common.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.Dto;
using CorarlERP.Features;
using CorarlERP.FileStorages;
using CorarlERP.Formats;
using CorarlERP.Inventories;
using CorarlERP.ItemReceipts;
using CorarlERP.Items;
using CorarlERP.Journals;
using CorarlERP.MultiCurrencies;
using CorarlERP.MultiTenancy;
using CorarlERP.PayBills;
using CorarlERP.PropertyValues;
using CorarlERP.PurchaseOrders;
using CorarlERP.QCTests;
using CorarlERP.Reports.Dto;
using CorarlERP.ReportTemplates;
using CorarlERP.UserGroups;
using CorarlERP.VendorCredit;
using CorarlERP.VendorCustomerOpenBalances;
using CorarlERP.VendorHelpers;
using CorarlERP.VendorHelpers.Data;
using CorarlERP.Vendors;
using CorarlERP.Vendors.Dto;
using CorarlERP.VendorTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static CorarlERP.Common.Dto.EnumStatus;
using static CorarlERP.enumStatus.EnumStatus;
using JournalType = CorarlERP.enumStatus.EnumStatus.JournalType;
using TransactionStatus = CorarlERP.enumStatus.EnumStatus.TransactionStatus;

namespace CorarlERP.Reports
{

    [AbpAuthorize]
    public class ReportVendorAppService : ReportBaseClass, IReportVendorAppService
    {

        private readonly IInventoryManager _inventoryManager;
        private readonly IRepository<Journal, Guid> _journalRepository;

        private readonly IRepository<JournalItem, Guid> _journalItemRepository;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;
        private readonly IRepository<AccountType, long> _accountTypeRepository;

        private readonly IRepository<AccountCycle, long> _accountCycleRepository;

        private readonly IAccountTransactionManager _accountTransactionManager;
        private readonly IAppFolders _appFolders;
        private readonly IRepository<Tenant, int> _tenantRepository;

        private readonly IRepository<Bill, Guid> _billRepository;
        private readonly IRepository<Format, long> _formatRepository;
        private readonly IRepository<BillItem, Guid> _billItemRepository;
        private readonly IRepository<PayBillDetail, Guid> _paybillItemRepository;
        private readonly IRepository<MultiCurrency, long> _multiCurrencyRepository;

        private readonly IRepository<Vendors.Vendor, Guid> _vendorRepository;
        private readonly IRepository<VendorOpenBalance, Guid> _vendorOpenBalanceRepository;

        private readonly IVendorHelper _vendorManager;
        private readonly IRepository<UserGroupMember, Guid> _userGroupMemberRepository;
        private readonly IRepository<VendorGroup, Guid> _vendorGroupRepository;

        private readonly IRepository<Property, long> _propertyRepository;
        private readonly IRepository<ItemProperty, Guid> _itemPropertyRepository;
        private readonly IRepository<Item, Guid> _itemRepository;

        private readonly IRepository<PurchaseOrder, Guid> _purchaseOrderRepository;
        private readonly IRepository<PurchaseOrderItem, Guid> _purchaseOrderItemRepository;
        private readonly IRepository<ItemReceipt, Guid> _itemReceiptRepository;
        private readonly IRepository<ItemReceiptItem, Guid> _itemReceiptItemRepository;

        private readonly IRepository<LabTestRequest, Guid> _labTestRequestRepository;
        private readonly IRepository<LabTestResult, Guid> _labTestResultRepository;
        private readonly IRepository<LabTestResultDetail, Guid> _labTestResultDetailRepository;

        private readonly IRepository<CorarlERP.VendorCredit.VendorCredit, Guid> _vendorCreditRepository;
        private readonly IRepository<VendorCreditDetail, Guid> _vendorCreditItemRepository;
        private readonly IFileStorageManager _fileStorageManager;

        public ReportVendorAppService(
            IRepository<LabTestResult, Guid> labTestResultRepository,
            IRepository<LabTestResultDetail, Guid> labTestResultDetailRepository,
            IRepository<LabTestRequest, Guid> labTestRequestRepository,
            IRepository<Journal, Guid> journalRepository,
            IRepository<JournalItem, Guid> journalItemRepository,
            IRepository<AccountType, long> accountTypeRepository,
            //IRepository<InventoryTransaction, Guid> inventoryTransactionRepository,
            IRepository<ChartOfAccount, Guid> chartOfAccountRepository,

            IRepository<Vendors.Vendor, Guid> vendorRepository,
            IFileStorageManager fileStorageManager,
            IRepository<Tenant, int> tenantRepository,
            IRepository<AccountCycle, long> accountCycleRepository,
            IAccountTransactionManager accountTransactionManager,
            IInventoryManager inventoryManager,
            AppFolders appFolders,
            IRepository<Bill, Guid> billRepository,
            IRepository<BillItem, Guid> billItemRepository,
            IRepository<Format, long> formatRepository,
            IVendorHelper vendorManager,
            IRepository<PayBillDetail, Guid> paybillItemRepository,
            IRepository<MultiCurrency, long> mulitCurrencyRepository,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            IRepository<VendorGroup, Guid> vendorGroupRepository,
            IRepository<Locations.Location, long> locationRepository,
            IRepository<VendorOpenBalance, Guid> vendorOpenBalanceRepository,
            IRepository<PurchaseOrder, Guid> purchaseOrderRepository,
            IRepository<PurchaseOrderItem, Guid> purchaseOrderItemRepository,
            IRepository<ItemReceipt, Guid> itemReceiptRepository,
            IRepository<ItemReceiptItem, Guid> itemReceiptItemRepository
           ) : base(accountCycleRepository, appFolders, userGroupMemberRepository, locationRepository)
        {
            _vendorGroupRepository = vendorGroupRepository;
            _userGroupMemberRepository = userGroupMemberRepository;
            _paybillItemRepository = paybillItemRepository;
            _journalRepository = journalRepository;
            _journalItemRepository = journalItemRepository;
            _accountTypeRepository = accountTypeRepository;
            _chartOfAccountRepository = chartOfAccountRepository;
            _tenantRepository = tenantRepository;
            _inventoryManager = inventoryManager;
            _accountTransactionManager = accountTransactionManager;
            _appFolders = appFolders;
            _fileStorageManager = fileStorageManager;
            _billRepository = billRepository;
            _billItemRepository = billItemRepository;
            _accountCycleRepository = accountCycleRepository;
            _formatRepository = formatRepository;
            _vendorManager = vendorManager;
            _vendorRepository = vendorRepository;
            _multiCurrencyRepository = mulitCurrencyRepository;
            _vendorOpenBalanceRepository = vendorOpenBalanceRepository;
            _propertyRepository = IocManager.Instance.Resolve<IRepository<Property, long>>();
            _itemPropertyRepository = IocManager.Instance.Resolve<IRepository<ItemProperty, Guid>>();
            _itemRepository = IocManager.Instance.Resolve<IRepository<Item, Guid>>();
            _purchaseOrderRepository = purchaseOrderRepository;
            _purchaseOrderItemRepository = purchaseOrderItemRepository;
            _itemReceiptRepository = itemReceiptRepository;
            _itemReceiptItemRepository = itemReceiptItemRepository;
            _vendorCreditRepository = IocManager.Instance.Resolve<IRepository<CorarlERP.VendorCredit.VendorCredit, Guid>>();
            _vendorCreditItemRepository = IocManager.Instance.Resolve<IRepository<VendorCreditDetail, Guid>>();
            _labTestResultRepository = labTestResultRepository;
            _labTestResultDetailRepository = labTestResultDetailRepository;
            _labTestRequestRepository = labTestRequestRepository;
        }

        private IQueryable<VendorSummaryOutput> GetVendorGroup(long userId)
        {
            // get user by group member
            var userGroups = _userGroupMemberRepository.GetAll()
                            .Where(x => x.MemberId == userId)
                            .AsNoTracking()
                            .Select(x => x.UserGroupId)
                            .ToList();

            var @query = _vendorGroupRepository.GetAll()
                            .Include(u => u.Vendor)
                            .Where(t => t.Vendor.Member == Member.UserGroup)
                            .Where(t => userGroups.Contains(t.UserGroupId))
                            //.Where(p => p.Vendor.IsActive == true)
                            .AsNoTracking()
                            .Select(t => t.Vendor);

            var @queryAll = _vendorRepository.GetAll()
                            .Where(t => t.Member == Member.All)
                            //.Where(p => p.IsActive == true)
                            .AsNoTracking();

            var result = @queryAll.Union(query)
                        .Select(t => new VendorSummaryOutput
                        {
                            Id = t.Id,
                            VendorName = t.VendorName,
                            VendorCode = t.VendorCode
                        })
                        .GroupBy(s => s.Id)
                        .Select(s => s.FirstOrDefault());
            return result;
        }


        #region Purchase Order Report         
        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_PurchaseOrder_Export_Pdf)]
        public async Task<FileDto> ExportPDFPurchaseOrderSummaryReport(GetPurchaseOrderSummaryReportInput input)
        {
            input.UsePagination = false;
            input.IsLoadMore = false;

            var tenant = await GetCurrentTenantAsync();
            var formatDate = await _formatRepository.GetAll()
                        .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
                        .Select(t => t.Web).FirstOrDefaultAsync();

            if (formatDate.IsNullOrEmpty())
            {
                formatDate = "dd/MM/yyyy";
            }

            var rounding = await GetCurrentCycleAsync();
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            //int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;
            var user = await GetCurrentUserAsync();
            var bills = await GetPurchaseOrderSummaryReport(input);

            return await Task.Run(async () =>
            {
                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                FileDto fileDto = new FileDto()
                {
                    SubFolder = null,
                    FileName = "InventoryReportPdf.pdf",
                    FileToken = "InventoryReportPdf.html",
                    FileType = MimeTypeNames.TextHtml
                };
                try
                {
                    templateHtml = await _fileStorageManager.GetTemplate(fileDto.FileToken);//retrive template from path
                    templateHtml = templateHtml.Trim();
                }
                catch (FileNotFoundException)
                {
                    throw new UserFriendlyException("FileNotFound");
                }
                //ToDo: Replace and concat string to be the same what frontend did
                exportHtml = templateHtml;

                var contentBody = string.Empty;
                var contentHeader = string.Empty;

                var viewHeader = reportCollumnHeader.OrderBy(x => x.SortOrder);
                var documentWidth = 1080;
                decimal totalVisibleColsWidth = 0;
                decimal totalTableWidth = 0;
                int reportCountColHead = viewHeader.Where(t => t.Visible == true).Count();
                //var multiCurrencyHeader = isMultiCurrencies ? "<tr>" : "";
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (documentWidth >= (totalVisibleColsWidth + i.ColumnLength))
                        {
                            var rowHeader = "";         
                            
                            rowHeader = $"<th width='{i.ColumnLength}px'>{i.ColumnTitle}</th>";

                            contentHeader += rowHeader;
                            totalTableWidth += i.ColumnLength;
                        }
                        else
                        {
                            i.Visible = false;
                            reportCountColHead--;
                        }
                        totalVisibleColsWidth += i.ColumnLength;
                    }
                }

                //multiCurrencyHeader += isMultiCurrencies ? $"</tr>" : "";
                exportHtml = exportHtml.Replace("{{tableWidth}}", totalTableWidth.ToString());

                #region Row Body              

                var groupBy = new List<GetListPurchaseOrderSummaryReportGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "Location":
                            groupBy = bills.Items
                                .GroupBy(t => t.LocationId)
                                .Select(t => new GetListPurchaseOrderSummaryReportGroupByOutput
                                {
                                    KeyName = t.Select(a => a.LocationName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Vendor":
                            groupBy = bills.Items
                                .GroupBy(t => t.VendorName)
                                .Select(t => new GetListPurchaseOrderSummaryReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.VendorName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Date":
                            groupBy = bills.Items
                                .GroupBy(t => t.Date.Date)
                                .Select(t => new GetListPurchaseOrderSummaryReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.Date.ToString(formatDate)).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }

                // write body
                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0) //write body have group by
                {
                    var trGroup = "";
                    foreach (var k in groupBy)
                    {
                        trGroup += $"<tr style='page-break-before: auto; page-break-after: auto;'>" +
                                   $"<td style='font-weight: bold;' colspan='{reportCountColHead}'> " + k.KeyName +
                                   $" </td></tr>";
                        foreach (var row in k.Items)
                        {
                            trGroup += "<tr style='page-break-before: auto; page-break-after: auto;'>";
                            foreach (var i in viewHeader)
                            {
                                if (i.Visible)
                                {
                                    if (i.ColumnName == "Location")
                                    {
                                        trGroup += $"<td>{row.LocationName}</td>";
                                    }
                                    else if (i.ColumnName == "OrderAmount")
                                    {
                                        trGroup += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.OrderAmount, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (i.ColumnName == "Date")
                                    {
                                        trGroup += $"<td>{row.Date.ToString(formatDate)}</td>";
                                    }
                                    else if (i.ColumnName == "OrderNumber")
                                    {
                                        trGroup += $"<td>{row.OrderNumber}</td>";
                                    }
                                    else if (i.ColumnName == "Reference")
                                    {
                                        trGroup += $"<td>{row.Reference}</td>";
                                    }
                                    else if (i.ColumnName == "Description")
                                    {
                                        trGroup += $"<td>{row.Description}</td>";
                                    }
                                    else if (i.ColumnName == "Vendor")
                                    {
                                        trGroup += $"<td>{row.VendorName}</td>";
                                    }
                                    else if (i.ColumnName == "User")
                                    {
                                        trGroup += $"<td>{row.User}</td>";
                                    }
                                    else if (i.ColumnName == "TotalOrderNetWeight")
                                    {
                                        trGroup += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.TotalOrderNetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (i.ColumnName == "TotalReceiveNetWeight")
                                    {
                                        trGroup += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.TotalReceiveNetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (i.ColumnName == "TotalRemainingNetWeight")
                                    {
                                        trGroup += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.TotalRemainingNetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if(i.ColumnName == "ReceiveStatus")
                                    {
                                        trGroup += $"<td>{row.ReceiveStatus.ToString()}</td>";
                                    }
                                    else if (i.ColumnName == "TotalReceiveCount")
                                    {
                                        trGroup += $"<td style='text-align: right'>{AutoRound(row.TotalReceiveCount)}</td>";
                                    }
                                    else if(i.ColumnName == "Item")
                                    {
                                        var itemList = "";
                                        var j = 0;
                                        foreach(var oi in row.Items)
                                        {
                                            if (j > 0) itemList += "<br>";
                                            itemList += $"{oi.ItemName}";
                                            j++;
                                        }
                                        trGroup += $"<td>{itemList}</td>";
                                    }
                                    else if (i.ColumnName == "Qty")
                                    {
                                        var itemList = "";
                                        var j = 0;
                                        foreach (var oi in row.Items)
                                        {
                                            if (j > 0) itemList += "<br>";
                                            itemList += $"{AutoRound(oi.OrderQty)} - {AutoRound(oi.ReceiveQty)} = {AutoRound(oi.OrderQty - oi.ReceiveQty)}";
                                            j++;
                                        }
                                        trGroup += $"<td style='text-align: right'>{itemList}</td>";
                                    }
                                    else if (i.ColumnName == "NetWeight")
                                    {
                                        var itemList = "";
                                        var j = 0;
                                        foreach (var oi in row.Items)
                                        {
                                            if (j > 0) itemList += "<br>";
                                            itemList += $"{AutoRound(oi.OrderNetWeight)}  -  {AutoRound(oi.ReceiveNetWeight)}  =  {AutoRound(oi.OrderNetWeight - oi.ReceiveNetWeight)}";
                                            j++;
                                        }
                                        trGroup += $"<td style='text-align: right'>{itemList}</td>";
                                    }
                                    else 
                                    {
                                        trGroup += $"<td></td>";
                                    }
                                }
                            }
                            trGroup += "</tr>";
                        }
                        trGroup += "<tr style='page-break-before: auto; page-break-after: auto;'>";
                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                if (!string.IsNullOrEmpty(i.AllowFunction))/*listSum.Contains(i.ColumnName))*/
                                {
                                    if (i.ColumnName == "OrderAmount")
                                    {
                                        trGroup += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(k.Items.Sum(t => t.OrderAmount), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (i.ColumnName == "TotalOrderNetWeight")
                                    {
                                        trGroup += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(k.Items.Sum(t => t.TotalOrderNetWeight), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";

                                    }
                                    else if (i.ColumnName == "TotalReceiveNetWeight")
                                    {
                                        trGroup += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(k.Items.Sum(t => t.TotalReceiveNetWeight), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";

                                    }
                                    else if (i.ColumnName == "TotalRemainingNetWeight")
                                    {
                                        trGroup += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(k.Items.Sum(t => t.TotalOrderNetWeight - t.TotalReceiveNetWeight), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";

                                    }
                                }
                                else //none sum
                                {
                                    trGroup += $"<td></td>";
                                }

                            }
                        }
                        trGroup += "</tr>";
                    }
                    contentBody += trGroup;
                }
                else // write body no group by
                {
                    foreach (var row in bills.Items)
                    {
                        var tr = "<tr style='page-break-before: auto; page-break-after: auto;'>";
                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                if (i.ColumnName == "Location")
                                {
                                    tr += $"<td>{row.LocationName}</td>";
                                }
                                else if (i.ColumnName == "OrderAmount")
                                {
                                    tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.OrderAmount, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (i.ColumnName == "Date")
                                {
                                    tr += $"<td>{row.Date.ToString(formatDate)}</td>";
                                }
                                else if (i.ColumnName == "OrderNumber")
                                {
                                    tr += $"<td>{row.OrderNumber}</td>";
                                }
                                else if (i.ColumnName == "Reference")
                                {
                                    tr += $"<td>{row.Reference}</td>";
                                }
                                else if (i.ColumnName == "Description")
                                {
                                    tr += $"<td>{row.Description}</td>";
                                }
                                else if (i.ColumnName == "Vendor")
                                {
                                    tr += $"<td>{row.VendorName}</td>";
                                }
                                else if (i.ColumnName == "User")
                                {
                                    tr += $"<td>{row.User}</td>";
                                }
                                else if (i.ColumnName == "TotalOrderNetWeight")
                                {
                                    tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.TotalOrderNetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (i.ColumnName == "TotalReceiveNetWeight")
                                {
                                    tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.TotalReceiveNetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (i.ColumnName == "TotalRemainingNetWeight")
                                {
                                    tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.TotalRemainingNetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (i.ColumnName == "ReceiveStatus")
                                {
                                    tr += $"<td>{row.ReceiveStatus.ToString()}</td>";
                                }
                                else if (i.ColumnName == "TotalReceiveCount")
                                {
                                    tr += $"<td style='text-align: right'>{AutoRound(row.TotalReceiveCount)}</td>";
                                }
                                else if (i.ColumnName == "Item")
                                {
                                    var itemList = "";
                                    var j = 0;
                                    foreach (var oi in row.Items)
                                    {
                                        if (j > 0) itemList += "<br>";
                                        itemList += $"{oi.ItemName}";
                                        j++;
                                    }
                                    tr += $"<td>{itemList}</td>";
                                }
                                else if (i.ColumnName == "Qty")
                                {
                                    var itemList = "";
                                    var j = 0;
                                    foreach (var oi in row.Items)
                                    {
                                        if (j > 0) itemList += "<br>";
                                        itemList += $"{AutoRound(oi.OrderQty)} - {AutoRound(oi.ReceiveQty)} = {AutoRound(oi.OrderQty - oi.ReceiveQty)}";
                                        j++;
                                    }
                                    tr += $"<td style='text-align: right'>{itemList}</td>";
                                }
                                else if (i.ColumnName == "NetWeight")
                                {
                                    var itemList = "";
                                    var j = 0;
                                    foreach (var oi in row.Items)
                                    {
                                        if (j > 0) itemList += "<br>";
                                        itemList += $"{AutoRound(oi.OrderNetWeight)} - {AutoRound(oi.ReceiveNetWeight)} = {AutoRound(oi.OrderNetWeight - oi.ReceiveNetWeight)}";
                                        j++;
                                    }
                                    tr += $"<td style='text-align: right'>{itemList}</td>";
                                }
                                else
                                {
                                    tr += $"<td></td>";
                                }
                            }
                        }
                        tr += "</tr>";
                        contentBody += tr;
                    }
                }
                #endregion Row Body

                #region Row Footer 

                if (reportHasShowFooterTotal.Count > 0)
                {
                    var index = 0;
                    var tr = "<tr style='page-break-before: auto; page-break-after: auto;'>";
                    foreach (var i in viewHeader)
                    {
                        if (i.Visible)
                        {
                            if (index == 0)
                            {
                                tr += $"<td style='font-weight: bold;text-align:right;'>{L("Total")}</td>";
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(i.AllowFunction))
                                {
                                    if (bills.TotalResult != null && bills.TotalResult.ContainsKey(i.ColumnName))
                                    {
                                        tr += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(bills.TotalResult[i.ColumnName], rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                }
                                else //none sum
                                {
                                    tr += $"<td></td>";
                                }
                            }
                            index++;
                        }
                    }
                    tr += "</tr>";
                    contentBody += tr;
                }
                #endregion Row Footer

                exportHtml = exportHtml.Replace("{{rowHeader}}", contentHeader);
                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                exportHtml = exportHtml.Replace("{{subHeader}}", "");
                EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();
                AddHeaderPDF(htmlToPdfConverter, tenant.Name, sheetName, input.FromDate, input.ToDate, formatDate);
                AddFooterPDF(htmlToPdfConverter, user.FullName, formatDate);
                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
                var tokenName = $"{Guid.NewGuid()}.pdf";
                var result = new FileDto();
                result.FileName = $"{sheetName}.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);
                return result;

            });
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_PurchaseOrder)]
        [HttpPost]
        public async Task<PagedResultWithTotalColuumnsDto<GetListReportPurchaseOrderSummaryOutput>> GetPurchaseOrderSummaryReport(GetListReportPurchaseOrderSummaryInput input)
        {

            var vendoTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            var periodCycles = await GetCloseCyleAsync(input.ToDate);

            var previousCycle = periodCycles.Where(u => u.EndDate != null && u.EndDate.Value.Date <= input.ToDate.Date)
                                            .OrderByDescending(u => u.EndDate.Value).FirstOrDefault();

            var currentCycle = periodCycles.Where(u => u.StartDate.Date <= input.ToDate.Date &&
                                                (u.EndDate == null || input.ToDate.Date <= u.EndDate.Value.Date))
                                           .OrderByDescending(u => u.StartDate).FirstOrDefault();

            var currentRoundingDigit = currentCycle != null ? currentCycle.RoundingDigit :
                                       previousCycle != null ? previousCycle.RoundingDigit : 2;

            var currentRoundingDigitUnitCost = currentCycle != null ? currentCycle.RoundingDigitUnitCost :
                                               previousCycle != null ? previousCycle.RoundingDigitUnitCost : 2;

            //var getVendorUserGroup = GetVendorGroup(AbpSession.GetUserId());

            var userGroupVendors = await GetUserGroupVendors();
            var userGroupItems = await GetUserGroupItems();

            var vendorQuery = GetVendors(userGroupVendors, input.Vendors, input.VendorTypes, vendoTypeMemberIds);
            var locationQuery = GetLocations(null, input.Locations);
            var userQuery = GetUsers(input.Users);


            var defaultLocationGroups = await GetUserGroupByLocation();

            var billItemQuery = from bi in _billItemRepository.GetAll()
                                           .Where(s => s.ItemReceiptItemId.HasValue && s.OrderItemId.HasValue)
                                           .AsNoTracking()
                                join j in _journalRepository.GetAll()
                                          .Where(s => s.JournalType == JournalType.Bill && s.Status == TransactionStatus.Publish)
                                          .AsNoTracking()
                                on bi.BillId equals j.BillId
                                select new
                                {
                                    BillItemId = bi.Id,
                                    ItemId = bi.ItemId,
                                    Qty = bi.Qty,
                                    BillId = bi.BillId,
                                    BillNo = j.JournalNo,
                                    OrderItemId = bi.OrderItemId
                                };

            var itemReceiptItemQuery = from ii in _itemReceiptItemRepository.GetAll()
                                           .Where(s => s.OrderItemId.HasValue)
                                           .AsNoTracking()
                                       join j in _journalRepository.GetAll()
                                                 .Where(s => s.JournalType == JournalType.ItemReceiptPurchase && s.Status == TransactionStatus.Publish)
                                                 .AsNoTracking()
                                       on ii.ItemReceiptId equals j.ItemReceiptId
                                       select new
                                       {
                                           ItemReceiptItemId = ii.Id,
                                           ItemId = ii.ItemId,
                                           Qty = ii.Qty,
                                           ItemReceiptId = ii.ItemReceiptId,
                                           ItemReceiptNo = j.JournalNo,
                                           OrderItemId = ii.OrderItemId
                                       };

            var orderQuery = from o in _purchaseOrderRepository.GetAll()
                                        .Where(s => s.Status == TransactionStatus.Publish &&
                                                        (s.ApprovalStatus == ApprovalStatus.Approved || s.ApprovalStatus == ApprovalStatus.Recorded))
                                        .WhereIf(defaultLocationGroups != null && defaultLocationGroups.Count > 0, u => defaultLocationGroups.Contains(u.LocationId.Value))
                                        .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId.Value))
                                        .WhereIf(input.FromDate != null && input.ToDate != null,
                                                (u => (u.OrderDate.Date) >= (input.FromDate.Date) && (u.OrderDate.Date) <= (input.ToDate.Date)))
                                        .WhereIf(!input.Filter.IsNullOrEmpty(), u =>
                                                u.OrderNumber.ToLower().Contains(input.Filter.ToLower()) ||
                                                u.Reference.ToLower().Contains(input.Filter.ToLower()) ||
                                                u.Memo.ToLower().Contains(input.Filter.ToLower()))
                                        .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                                        .WhereIf(input.VendorTypes != null && input.VendorTypes.Any(), s => input.VendorTypes.Contains(s.Vendor.VendorTypeId.Value))
                                        .WhereIf(input.Vendors != null && input.Vendors.Any(), s => input.Vendors.Contains(s.VendorId))
                                        .AsNoTracking()
                                        .Select(o => new
                                        {
                                            Id = o.Id,
                                            OrderNumber = o.OrderNumber,
                                            Date = o.OrderDate,
                                            LocationId = o.LocationId,
                                            VendorId = o.VendorId,
                                            CreatorUserId = o.CreatorUserId,
                                            Description = o.Memo,
                                            Reference = o.Reference,
                                            OrderAmount = o.Total,
                                            OrderAmountMultiCurrency = o.MultiCurrencyTotal,
                                            ReceiveStatus = o.ReceiveStatus,
                                            TotalReceiveCount = o.ReceiveCount,
                                        })
                             join v in vendorQuery
                             on o.VendorId equals v.Id
                             join l in locationQuery
                             on o.LocationId equals l.Id
                             join u in userQuery
                             on o.CreatorUserId equals u.Id
                             select new GetListReportPurchaseOrderSummaryOutput
                             {
                                 Id = o.Id,
                                 OrderNumber = o.OrderNumber,
                                 Date = o.Date,
                                 LocationId = o.LocationId,
                                 VendorId = o.VendorId,
                                 Description = o.Description,
                                 Reference = o.Reference,
                                 OrderAmount = o.OrderAmount,
                                 OrderAmountMultiCurrency = o.OrderAmountMultiCurrency,
                                 ReceiveStatus = o.ReceiveStatus,
                                 TotalReceiveCount = o.TotalReceiveCount,
                                 LocationName = l.LocationName,
                                 User = u.UserName,
                                 VendorName = v.VendorName
                             };


            var orderItemQuery = _purchaseOrderItemRepository.GetAll()
                                            .AsNoTracking()
                                 .Select(oi => new PurchaseOrderItemSummaryOutput
                                 {
                                     OrderId = oi.PurchaseOrderId,
                                     OrderItemId = oi.Id,
                                     ItemId = oi.ItemId,
                                     OrderQty = oi.Unit,
                                 });


            var itemOrderWithReceive = from oi in orderItemQuery

                                       join bi in billItemQuery
                                       on oi.OrderItemId equals bi.OrderItemId
                                       into bItems

                                       join ri in itemReceiptItemQuery
                                       on oi.OrderItemId equals ri.OrderItemId
                                       into rItems

                                       let bQty = bItems == null || bItems.Count() == 0 ? 0 : bItems.Sum(r => r.Qty)
                                       let rQty = rItems == null || rItems.Count() == 0 ? 0 : rItems.Sum(s => s.Qty)
                                       let receiveQty = bQty + rQty

                                       select new PurchaseOrderItemSummaryOutput
                                       {
                                           OrderId = oi.OrderId,
                                           OrderItemId = oi.OrderItemId,
                                           ItemId = oi.ItemId,
                                           OrderQty = oi.OrderQty,
                                           ReceiveQty = receiveQty,
                                       };


            var itemQuery = GetItemWithProperties(userGroupItems, new List<Guid>(), null);

            var orderItemWithProperty = from oi in itemOrderWithReceive
                                        join i in itemQuery
                                        on oi.ItemId equals i.Id

                                        let netWeight = i.Properties.Any(p => p.IsUnit) ? 0 : i.Properties.Where(t => t.IsUnit).Select(t => t.NetWeight).FirstOrDefault()

                                        select new PurchaseOrderItemSummaryOutput
                                        {
                                            OrderId = oi.OrderId,
                                            OrderItemId = oi.OrderItemId,
                                            ItemId = oi.ItemId,
                                            ItemCode = i.ItemCode,
                                            ItemName = i.ItemName,
                                            OrderQty = oi.OrderQty,
                                            ReceiveQty = oi.ReceiveQty,
                                            OrderNetWeight = oi.OrderQty * netWeight / 1000,
                                            ReceiveNetWeight = oi.ReceiveQty * netWeight / 1000,
                                            Properties = i.Properties,
                                        };


            var query = from o in orderQuery
                        join oi in orderItemWithProperty
                        on o.Id equals oi.OrderId
                        into items

                        where (input.Items == null || input.Items.Count() == 0 || items.Any(i => input.Items.Contains(i.ItemId))) &&
                               (input.PropertyDics == null || input.PropertyDics.Count() == 0 ||
                               items.Any(p => p.Properties.Where(ip => input.PropertyDics.Any(v => v.PropertyId == ip.PropertyId) &&
                                   input.PropertyDics.FirstOrDefault(v => v.PropertyId == ip.PropertyId) != null &&
                                   (input.PropertyDics.FirstOrDefault(v => v.PropertyId == ip.PropertyId).PropertyValueIds == null ||
                                   input.PropertyDics.FirstOrDefault(v => v.PropertyId == ip.PropertyId).PropertyValueIds.Count == 0 ||
                                   input.PropertyDics.FirstOrDefault(v => v.PropertyId == ip.PropertyId).PropertyValueIds.Contains(ip.Id)))
                                .Count() == input.PropertyDics.Count)
                               )

                        select new GetListReportPurchaseOrderSummaryOutput
                        {
                            Id = o.Id,
                            Date = o.Date,
                            OrderNumber = o.OrderNumber,
                            Reference = o.Reference,
                            ReceiveStatus = o.ReceiveStatus,
                            ReceiveStatusName = o.Reference.ToString(),
                            User = o.User,
                            Description = o.Description,
                            LocationId = o.LocationId,
                            LocationName = o.LocationName,
                            OrderAmount = o.OrderAmount,
                            OrderAmountMultiCurrency = o.OrderAmountMultiCurrency,
                            VendorId = o.VendorId,
                            VendorName = o.VendorName,
                            TotalOrderNetWeight = items.Sum(s => s.OrderNetWeight),
                            TotalReceiveNetWeight = items.Sum(s => s.ReceiveNetWeight),
                            TotalRemainingNetWeight = items.Sum(s => s.OrderNetWeight - s.ReceiveNetWeight),
                            TotalReceiveCount = o.TotalReceiveCount,
                            RoundingDigit = currentRoundingDigit,
                            Items = items.Select(s => new PurchaseOrderItemSummaryOutput
                            {
                                ItemId = s.ItemId,
                                ItemCode = s.ItemCode,
                                ItemName = s.ItemName,
                                OrderQty = s.OrderQty,
                                ReceiveQty = s.ReceiveQty,
                                ReceiveNetWeight = s.ReceiveNetWeight,
                                OrderNetWeight = s.OrderNetWeight,
                                Properties = s.Properties
                            }).ToList(),
                        };

            var sumOfColumns = new Dictionary<string, decimal>();
            List<ColumnSummaryOutPut> sumQuery = null;
            var resultCount = await query.CountAsync();

            if(resultCount == 0) return new PagedResultWithTotalColuumnsDto<GetListReportPurchaseOrderSummaryOutput>(resultCount, new List<GetListReportPurchaseOrderSummaryOutput>(), sumOfColumns);


            if (input.IsLoadMore == false && input.ColumnNamesToSum != null)
            {
                sumQuery = await query.Select(s => new ColumnSummaryOutPut
                {
                    TotalOrderAmount = s.OrderAmount,
                    TotalOrderAmountMultiCurrency = s.OrderAmountMultiCurrency,
                    TotalOrderNetWeight = s.TotalOrderNetWeight,
                    TotalReceiveNetWeight = s.TotalReceiveNetWeight
                }).ToListAsync();
            }
            

            if (input.ColumnNamesToSum != null && input.IsLoadMore == false)
            {
                foreach (var col in input.ColumnNamesToSum)
                {
                    if (col == "OrderAmount")
                    {
                        sumOfColumns.Add(col, sumQuery.Sum(s => s.TotalOrderAmount));
                    }
                    else if (col == "TotalOrderNetWeight")
                    {
                        sumOfColumns.Add(col, sumQuery.Sum(s => s.TotalOrderNetWeight));
                    }
                    else if (col == "TotalReceiveNetWeight")
                    {
                        sumOfColumns.Add(col, sumQuery.Sum(s => s.TotalReceiveNetWeight));
                    }
                    else if (col == "TotalRemainingNetWeight")
                    {
                        sumOfColumns.Add(col, sumQuery.Sum(s => s.TotalOrderNetWeight - s.TotalReceiveNetWeight));
                    }
                }
            }

            switch (input.GroupBy)
            {
                case "Date":
                    query = query.OrderBy(s => s.Date);
                    break;
                case "Location":
                    query = query.OrderBy(s => s.LocationId);
                    break;
                case "Vendor":
                    query = query.OrderBy(s => s.VendorId);
                    break;
                default:
                    query = query.OrderBy(input.Sorting);
                    break;
            }

            var @entities = new List<GetListReportPurchaseOrderSummaryOutput>();
            if (input.UsePagination == true)
            {
                @entities = await query.PageBy(input).ToListAsync();
            }
            else
            {
                @entities = await query.ToListAsync();
            }

            var returnResult = new PagedResultWithTotalColuumnsDto<GetListReportPurchaseOrderSummaryOutput>(resultCount, @entities, sumOfColumns);

            return returnResult;
        }


       
        public ReportOutput GetReportTemplatePurchaseOrderSummary()
        {
            var itemPropertyCols = _propertyRepository.GetAll().AsNoTracking()
                            .Where(t => t.IsActive == true)
                            .Select(t => new CollumnOutput
                            {
                                AllowGroupby = false,
                                AllowFilter = true,
                                ColumnName = t.Name,
                                ColumnLength = 150,
                                ColumnTitle = t.Name,
                                ColumnType = ColumnType.ItemProperty,
                                SortOrder = 11,
                                Visible = true,
                                AllowFunction = null,
                                MoreFunction = null,
                                IsDisplay = false,
                                AllowShowHideFilter = true,
                                ShowHideFilter = true,
                                DefaultValue = t.Id.ToString(),//to init the value of property
                            });

            var columnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Search",
                        ColumnLength = 150,
                        ColumnTitle = "Search",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "DateRange",
                        ColumnLength = 150,
                        ColumnTitle = "DateRange",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = true,
                        DefaultValue = "thisMonth",
                        AllowFunction = null,
                        DisableDefault = false,
                        MoreFunction = null,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                      
                      new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = false,
                        ColumnName = "Date",
                        ColumnLength = 150,
                        ColumnTitle = "Date",
                        ColumnType = ColumnType.Date,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                     },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "OrderNumber",
                        ColumnLength = 140,
                        ColumnTitle = "Order Number",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Reference",
                        ColumnLength = 140,
                        ColumnTitle = "Reference No",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                      new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Vendor",
                        ColumnLength = 150,
                        ColumnTitle = "Vendor",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Item",
                        ColumnLength = 250,
                        ColumnTitle = "Item",
                        ColumnType = ColumnType.String,
                        SortOrder = 5,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                       new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Qty",
                        ColumnLength = 120,
                        ColumnTitle = "Qty",
                        ColumnType = ColumnType.String,
                        SortOrder = 6,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                        new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "NetWeight",
                        ColumnLength = 120,
                        ColumnTitle = "Net Weight in Ton",
                        ColumnType = ColumnType.String,
                        SortOrder = 7,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                      new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Location",
                        ColumnLength = 120,
                        ColumnTitle = "Location",
                        ColumnType = ColumnType.String,
                        SortOrder = 8,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Description",
                        ColumnLength = 150,
                        ColumnTitle = "Description",
                        ColumnType = ColumnType.String,
                        SortOrder = 11,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ReceiveStatus",
                        ColumnLength = 150,
                        ColumnTitle = "Receive Status",
                        ColumnType = ColumnType.String,
                        SortOrder = 12,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "OrderAmount",
                        ColumnLength = 110,
                        ColumnTitle = "Order Amount",
                        ColumnType = ColumnType.RoundingDigit,
                        SortOrder = 13,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },

                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalReceiveCount",
                        ColumnLength = 110,
                        ColumnTitle = "Total Receive Count",
                        ColumnType = ColumnType.RoundingDigit,
                        SortOrder = 14,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = null,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalOrderNetWeight",
                        ColumnLength = 150,
                        ColumnTitle = "Total Order in Ton",
                        ColumnType = ColumnType.RoundingDigit,
                        SortOrder = 15,
                        Visible = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalReceiveNetWeight",
                        ColumnLength = 150,
                        ColumnTitle = "Total Receive in Ton",
                        ColumnType = ColumnType.RoundingDigit,
                        SortOrder = 16,
                        Visible = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalRemainingNetWeight",
                        ColumnLength = 150,
                        ColumnTitle = "Total Remaining in Ton",
                        ColumnType = ColumnType.RoundingDigit,
                        SortOrder = 17,
                        Visible = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "User",
                        ColumnLength = 110,
                        ColumnTitle = "User",
                        ColumnType = ColumnType.String,
                        SortOrder = 18,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "VendorType",
                        ColumnLength = 150,
                        ColumnTitle = "Vendor Type",
                        ColumnType = ColumnType.String,
                        SortOrder = 19,
                        Visible = true,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                };

            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = columnInfo.Concat(itemPropertyCols).ToList(),
                Groupby = "",
                HeaderTitle = "Purchase Order Summary Report",
                Sortby = "",
            };

            //var isMultiCurrency = _multiCurrencyRepository.GetAll().Any();

            //if (isMultiCurrency)
            //{
            //    var currency = new CollumnOutput
            //    {
            //        AllowGroupby = false,
            //        AllowFilter = true,
            //        ColumnName = "Currency",
            //        ColumnLength = 140,
            //        ColumnTitle = "Currency",
            //        ColumnType = ColumnType.String,
            //        SortOrder = 3,
            //        Visible = true,
            //        IsDisplay = false,
            //        DisableDefault = false,
            //        AllowShowHideFilter = true,
            //        ShowHideFilter = true,
            //    };
            //    result.ColumnInfo.Add(currency);
            //}

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_PurchaseOrder_Export_Excel)]
        public async Task<FileDto> ExportExcelPurchaseOrderSummaryReport(GetPurchaseOrderSummaryReportInput input)
        {
            input.UsePagination = false;
            input.IsLoadMore = false;

            // Query get collumn header 
            //var @entity = await _reportTemplateManager.GetAsync(ReportType.ReportType_Outbounce);
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;

            var reportResult = await GetPurchaseOrderSummaryReport(input);
            var billData = reportResult.Items;

            var result = new FileDto();

            //Creates a blank workbook. Use the using statment, so the package is disposed when we are done.
            using (var p = new ExcelPackage())
            {
                //A workbook must have at least on cell, so lets add one... 
                var ws = p.Workbook.Worksheets.Add(sheetName);
                ws.PrinterSettings.Orientation = eOrientation.Landscape;
                ws.PrinterSettings.FitToPage = true;
                //ws.PrinterSettings.PaperSize = ePaperSize.A3; //set default format paper size 
                ws.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                ws.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet

                //var subCols = billData == null ? null : billData.First().CurrencyColumnTotals.Select(s => s.CurrencyCode).ToList();
                //var isMultiCurrencies = input.CurrencyFilter == CurrencyFilter.MultiCurrencies && subCols != null && subCols.Count() > 1;

                #region Row 1 Header
                AddTextToCell(ws, 1, 1, sheetName, true);
                MergeCell(ws, 1, 1, 1, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 1


                #region Row 2 Header
                var header = input.ToDate.ToString("dd-MM-yyyy");
                AddTextToCell(ws, 2, 1, header, true);
                MergeCell(ws, 2, 1, 2, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 2

                #region Row 3 Header Table
                int rowTableHeader = 3;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                foreach (var i in reportCollumnHeader)
                {                    
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colHeaderTable += 1;
                }

                //if (isMultiCurrencies) rowTableHeader += 1;
                #endregion Row 3

                #region Row Body 
                // use for check auto dynamic col footer of sum value
                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                var groupBy = new List<GetListPurchaseOrderSummaryReportGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "Location":
                            groupBy = billData
                                .GroupBy(t => t.LocationId)
                                .Select(t => new GetListPurchaseOrderSummaryReportGroupByOutput
                                {
                                    KeyName = t.Select(a => a.LocationName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;

                        case "Vendor":
                            groupBy = billData
                                .GroupBy(t => t.VendorId)
                                .Select(t => new GetListPurchaseOrderSummaryReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.VendorName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Date":
                            groupBy = billData
                                .GroupBy(t => t.Date.Date)
                                .Select(t => new GetListPurchaseOrderSummaryReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.Date.ToString()).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }
                // write body

                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0)
                {
                    foreach (var k in groupBy)
                    {
                        //key group by name
                        AddTextToCell(ws, rowBody, 1, k.KeyName, true);
                        MergeCell(ws, rowBody, 1, rowBody, reportCountColHead, ExcelHorizontalAlignment.Left);
                        rowBody += 1;
                        count = 1;
                        //list of item
                        foreach (var i in k.Items)
                        {
                            int collumnCellBody = 1;
                            foreach (var item in reportCollumnHeader)// map with correct key of properties 
                            {

                                WriteBodyPurchaseOrderSummary(ws, rowBody, collumnCellBody, item, i, count);
                                collumnCellBody += 1;
                            }
                            rowBody += 1;
                            count += 1;
                        }

                        //sub total of group by item
                        int collumnCellGroupBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {
                            if (item.AllowFunction != null)
                            {                                
                                var fromCell = GetAddressName(ws, rowBody - k.Items.Count, collumnCellGroupBody);
                                var toCell = GetAddressName(ws, rowBody - 1, collumnCellGroupBody);
                                if (footerGroupDict.ContainsKey(item.ColumnName))
                                {
                                    footerGroupDict[item.ColumnName] += fromCell + ":" + toCell + ",";
                                }
                                else
                                {
                                    footerGroupDict.Add(item.ColumnName, fromCell + ":" + toCell + ",");
                                }
                                AddFormula(ws, rowBody, collumnCellGroupBody, "SUM(" + fromCell + ":" + toCell + ")", true);
                            }
                            collumnCellGroupBody += 1;
                        }
                        rowBody += 1;
                    }
                }
                else
                {
                    foreach (var i in billData)
                    {
                        int collumnCellBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {
                            WriteBodyPurchaseOrderSummary(ws, rowBody, collumnCellBody, item, i, count);
                            collumnCellBody += 1;
                        }
                        rowBody += 1;
                        count += 1;
                    }
                }
                #endregion Row Body


                #region Row Footer 
                if (reportHasShowFooterTotal.Count > 0)
                {
                    int footerRow = rowBody;
                    int footerColNumber = 1;
                    AddTextToCell(ws, footerRow, footerColNumber, "TOTAL", true);
                    foreach (var i in reportCollumnHeader)
                    {
                        if (i.AllowFunction != null)
                        {
                            if (!input.GroupBy.IsNullOrWhiteSpace())
                            {                                
                                //sum custom range cell depend on group item
                                int rowFooter = rowTableHeader + 1;// get start row after 
                                var sumValue = "SUM(" + footerGroupDict[i.ColumnName] + ")";
                                if (i.ColumnType == ColumnType.Number)
                                {
                                    AddFormula(ws, footerRow, footerColNumber, sumValue, true, false, true);
                                }
                                else
                                {
                                    AddFormula(ws, footerRow, footerColNumber, sumValue, true);
                                }
                            }
                            else
                            {                                
                                int rowFooter = rowTableHeader + 1;// get start row after header 
                                var fromCell = GetAddressName(ws, rowFooter, footerColNumber);
                                var toCell = GetAddressName(ws, footerRow - 1, footerColNumber);
                                if (i.ColumnType == ColumnType.Number)
                                {
                                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true, false, true);
                                }
                                else
                                {
                                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true);
                                }
                            }
                        }
                        footerColNumber += 1;
                    }
                }
                #endregion Row Footer


                result.FileName = $"Purchase_Order_Summary_Report_{header.Replace(" ", "_").Replace("-", "_")}.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        #endregion


        #region Bill Report         
        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Purchasing_Export_Pdf)]
        public async Task<FileDto> ExportPDFBillReport(GetBillReportInput input)
        {
            input.UsePagination = false;
            input.IsLoadMore = false;

            var tenant = await GetCurrentTenantAsync();
            var formatDate = await _formatRepository.GetAll()
                        .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
                        .Select(t => t.Web).FirstOrDefaultAsync();

            if (formatDate.IsNullOrEmpty())
            {
                formatDate = "dd/MM/yyyy";
            }

            var rounding = await GetCurrentCycleAsync();
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            //int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;
            var user = await GetCurrentUserAsync();
            var bills = await GetBillReport(input);

            var subCols = bills.Items == null ? null : bills.Items.First().CurrencyColumnTotals.Select(s => s.CurrencyCode).ToList();
            var isMultiCurrencies = input.CurrencyFilter == CurrencyFilter.MultiCurrencies && subCols != null && subCols.Count() > 1;

            return await Task.Run(async () =>
            {
                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                FileDto fileDto = new FileDto()
                {
                    SubFolder = null,
                    FileName = "InventoryReportPdf.pdf",
                    FileToken = "InventoryReportPdf.html",
                    FileType = MimeTypeNames.TextHtml
                };
                try
                {
                    templateHtml = await _fileStorageManager.GetTemplate(fileDto.FileToken);//retrive template from path
                    templateHtml = templateHtml.Trim();
                }
                catch (FileNotFoundException)
                {
                    throw new UserFriendlyException("FileNotFound");
                }
                //ToDo: Replace and concat string to be the same what frontend did
                exportHtml = templateHtml;

                var contentBody = string.Empty;
                var contentHeader = string.Empty;

                var viewHeader = reportCollumnHeader.OrderBy(x => x.SortOrder);
                var documentWidth = 1080;
                decimal totalVisibleColsWidth = 0;
                decimal totalTableWidth = 0;
                int reportCountColHead = viewHeader.Where(t => t.Visible == true).Count();
                var multiCurrencyHeader = isMultiCurrencies ? "<tr>" : "";
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (documentWidth >= (totalVisibleColsWidth + i.ColumnLength))
                        {
                            var rowHeader = "";
                            if (isMultiCurrencies && bills.TotalResults.ContainsKey(i.ColumnName))
                            {
                                var index = 0;
                                var subColWidth = Convert.ToInt32(i.ColumnLength / bills.TotalResults[i.ColumnName].Count());
                                rowHeader = $"<th  colspan='{bills.TotalResults[i.ColumnName].Count()}' width='{i.ColumnLength}px'>{i.ColumnTitle}</th>";
                                foreach (var subHeader in bills.TotalResults[i.ColumnName])
                                {
                                    if (index > 0) reportCountColHead++;
                                    multiCurrencyHeader += $"<th style='width: {subColWidth}px;'>{subHeader.Key}</th>";
                                    index++;
                                }
                            }
                            else
                            {
                                rowHeader = $"<th rowspan='2' width='{i.ColumnLength}px'>{i.ColumnTitle}</th>";
                            }
                            contentHeader += rowHeader;
                            totalTableWidth += i.ColumnLength;
                        }
                        else
                        {
                            i.Visible = false;
                            reportCountColHead--;
                        }
                        totalVisibleColsWidth += i.ColumnLength;
                    }
                }

                multiCurrencyHeader += isMultiCurrencies ? $"</tr>" : "";
                exportHtml = exportHtml.Replace("{{tableWidth}}", totalTableWidth.ToString());

                #region Row Body              

                var groupBy = new List<GetListBillReportGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "Location":
                            groupBy = bills.Items
                                .GroupBy(t => t.LocationId)
                                .Select(t => new GetListBillReportGroupByOutput
                                {
                                    KeyName = t.Select(a => a.LocationName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "JournalNo":
                            groupBy = bills.Items
                                .GroupBy(t => t.JournalNo)
                                .Select(t => new GetListBillReportGroupByOutput
                                {
                                    KeyName = t.Select(a => a.JournalNo).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Account":
                            groupBy = bills.Items
                                .GroupBy(t => t.AccountId)
                                .Select(t => new GetListBillReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.AccountName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;

                        case "Vendor":
                            groupBy = bills.Items
                                .GroupBy(t => t.VendorId)
                                .Select(t => new GetListBillReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.VendorName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Date":
                            groupBy = bills.Items
                                .GroupBy(t => t.Date)
                                .Select(t => new GetListBillReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.Date.ToString(formatDate)).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;

                        case "Item":
                            groupBy = bills.Items
                                .GroupBy(t => t.ItemId)
                                .Select(t => new GetListBillReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.ItemCode != null ? x.ItemCode + "-" + x.ItemName : "").FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }

                // write body
                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0) //write body have group by
                {
                    var trGroup = "";
                    foreach (var k in groupBy)
                    {
                        trGroup += $"<tr style='page-break-before: auto; page-break-after: auto;'>" +
                                   $"<td style='font-weight: bold;' colspan='{reportCountColHead}'> " + k.KeyName +
                                   $" </td></tr>";
                        foreach (var row in k.Items)
                        {
                            trGroup += "<tr style='page-break-before: auto; page-break-after: auto;'>";
                            foreach (var i in viewHeader)
                            {
                                if (i.Visible)
                                {
                                    if (i.ColumnName == "ItemName")
                                    {
                                        trGroup += $"<td>{row.ItemName}</td>";
                                    }
                                    else if (i.ColumnName == "ItemCode")
                                    {
                                        trGroup += $"<td>{row.ItemCode}</td>";
                                    }
                                    else if (i.ColumnName == "JournalNo")
                                    {
                                        trGroup += $"<td>{row.JournalNo}</td>";
                                    }
                                    else if (i.ColumnName == "Account")
                                    {
                                        trGroup += $"<td>{row.AccountCode + " - " + row.AccountName }</td>";
                                    }
                                    else if (i.ColumnName == "Location")
                                    {
                                        trGroup += $"<td>{row.LocationName}</td>";
                                    }
                                    else if (i.ColumnName == "TaxRate")
                                    {

                                        trGroup += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.TaxRate, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (i.ColumnName == "UnitCost")
                                    {
                                        trGroup += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.UnitCost, rounding.RoundingDigitUnitCost, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (i.ColumnName == "Qty")
                                    {
                                        trGroup += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Qty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else if (i.ColumnName == "Total")
                                    {
                                        if (isMultiCurrencies && bills.TotalResults.ContainsKey(i.ColumnName))
                                        {
                                            foreach (var subHeader in subCols)
                                            {
                                                var total = row.CurrencyColumnTotals.Where(s => s.CurrencyCode == subHeader).Sum(s => s.Total);
                                                trGroup += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(total, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                            }
                                        }
                                        else
                                        {
                                            trGroup += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Total, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                    }
                                    else if (i.ColumnName == "Tax")

                                    {
                                        trGroup += $"<td>{row.TaxName}</td>";
                                    }
                                    else if (i.ColumnName == "Date")
                                    {
                                        trGroup += $"<td>{row.Date.ToString(formatDate)}</td>";
                                    }
                                    else if (i.ColumnName == "Reference")
                                    {
                                        trGroup += $"<td>{row.Reference}</td>";
                                    }
                                    else if (i.ColumnName == "Description")
                                    {
                                        trGroup += $"<td>{row.Description}</td>";
                                    }
                                    else if (i.ColumnName == "Vendor")
                                    {
                                        trGroup += $"<td>{row.VendorName}</td>";
                                    }
                                    else if (i.ColumnName == "VendorType")
                                    {
                                        trGroup += $"<td>{row.VendorType}</td>";
                                    }
                                    else if (i.ColumnName == "User")
                                    {
                                        trGroup += $"<td>{row.User}</td>";
                                    }
                                    else if (i.ColumnName == "NetWeight")
                                    {
                                        trGroup += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.NetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                    else
                                    {
                                        var property = row.Properties.FirstOrDefault(s => s.PropertyName == i.ColumnName);
                                        if (property != null)
                                        {
                                            trGroup += $"<td>{property.Value}</td>";
                                        }
                                        else
                                        {
                                            trGroup += $"<td></td>";
                                        }
                                    }
                                }
                            }
                            trGroup += "</tr>";
                        }
                        trGroup += "<tr style='page-break-before: auto; page-break-after: auto;'>";
                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                if (!string.IsNullOrEmpty(i.AllowFunction))/*listSum.Contains(i.ColumnName))*/
                                {
                                    if (i.ColumnName == "Total" && isMultiCurrencies && bills.TotalResults.ContainsKey(i.ColumnName))
                                    {
                                        foreach (var subHeader in subCols)
                                        {
                                            var total = k.Items.SelectMany(s => s.CurrencyColumnTotals.Where(r => r.CurrencyCode == subHeader)).Sum(s => s.Total);
                                            trGroup += $"<td style='font-weight: bold; text-align: right;'>{FormatNumberCurrency(Math.Round(total, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                    }
                                    else
                                    {
                                        if (i.ColumnName == "Total")
                                        {
                                            trGroup += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(k.Items.Sum(t => t.Total), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                        else if (i.ColumnName == "NetWeight")
                                        {
                                            trGroup += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(k.Items.Sum(t => t.NetWeight), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";

                                        }
                                        else if (i.ColumnName == "Qty")
                                        {
                                            trGroup += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(k.Items.Sum(t => t.Qty), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";

                                        }
                                    }
                                }
                                else //none sum
                                {
                                    trGroup += $"<td></td>";
                                }

                            }
                        }
                        trGroup += "</tr>";
                    }
                    contentBody += trGroup;
                }
                else // write body no group by
                {
                    foreach (var row in bills.Items)
                    {
                        var tr = "<tr style='page-break-before: auto; page-break-after: auto;'>";
                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                if (i.ColumnName == "ItemName")
                                {
                                    tr += $"<td>{row.ItemName}</td>";
                                }
                                else if (i.ColumnName == "ItemCode")
                                {
                                    tr += $"<td>{row.ItemCode}</td>";
                                }
                                else if (i.ColumnName == "JournalNo")
                                {
                                    tr += $"<td>{row.JournalNo}</td>";
                                }
                                else if (i.ColumnName == "Account")
                                {
                                    tr += $"<td>{row.AccountCode + " - " + row.AccountName }</td>";
                                }
                                else if (i.ColumnName == "Location")
                                {
                                    tr += $"<td>{row.LocationName}</td>";
                                }
                                else if (i.ColumnName == "TaxRate")
                                {

                                    tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.TaxRate, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (i.ColumnName == "UnitCost")
                                {
                                    tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.UnitCost, rounding.RoundingDigitUnitCost, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (i.ColumnName == "Qty")
                                {
                                    tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Qty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else if (i.ColumnName == "Total")
                                {
                                    if (isMultiCurrencies && bills.TotalResults.ContainsKey(i.ColumnName))
                                    {
                                        foreach (var subHeader in subCols)
                                        {
                                            var total = row.CurrencyColumnTotals.Where(r => r.CurrencyCode == subHeader).Sum(s => s.Total);
                                            tr += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(total, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                    }
                                    else
                                    {
                                        tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Total, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                }
                                else if (i.ColumnName == "Tax")

                                {
                                    tr += $"<td>{row.TaxName}</td>";
                                }
                                else if (i.ColumnName == "Date")
                                {
                                    tr += $"<td>{row.Date.ToString(formatDate)}</td>";
                                }
                                else if (i.ColumnName == "Reference")
                                {
                                    tr += $"<td>{row.Reference}</td>";
                                }
                                else if (i.ColumnName == "Description")
                                {
                                    tr += $"<td>{row.Description}</td>";
                                }
                                else if (i.ColumnName == "Vendor")
                                {
                                    tr += $"<td>{row.VendorName}</td>";
                                }
                                else if (i.ColumnName == "VendorType")
                                {
                                    tr += $"<td>{row.VendorType}</td>";
                                }
                                else if (i.ColumnName == "User")
                                {
                                    tr += $"<td>{row.User}</td>";
                                }
                                else if (i.ColumnName == "NetWeight")
                                {
                                    tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.NetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                }
                                else
                                {
                                    var property = row.Properties.FirstOrDefault(s => s.PropertyName == i.ColumnName);
                                    if (property != null)
                                    {
                                        tr += $"<td>{property.Value}</td>";
                                    }
                                    else
                                    {
                                        tr += $"<td></td>";
                                    }
                                }
                            }
                        }
                        tr += "</tr>";
                        contentBody += tr;
                    }
                }
                #endregion Row Body

                #region Row Footer 

                if (reportHasShowFooterTotal.Count > 0)
                {
                    var index = 0;
                    var tr = "<tr style='page-break-before: auto; page-break-after: auto;'>";
                    foreach (var i in viewHeader)
                    {
                        if (i.Visible)
                        {
                            if (index == 0)
                            {
                                tr += $"<td style='font-weight: bold;text-align:right;'>{L("Total")}</td>";
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(i.AllowFunction))
                                {
                                    if (bills.TotalResults != null && bills.TotalResults.ContainsKey(i.ColumnName))
                                    {
                                        foreach (var subCol in subCols)
                                        {
                                            var total = bills.TotalResults[i.ColumnName].ContainsKey(subCol) ? bills.TotalResults[i.ColumnName][subCol] : 0;
                                            tr += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(total, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                    }
                                    else if (bills.TotalResult != null && bills.TotalResult.ContainsKey(i.ColumnName))
                                    {
                                        tr += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(bills.TotalResult[i.ColumnName], rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                }
                                else //none sum
                                {
                                    tr += $"<td></td>";
                                }
                            }
                            index++;
                        }
                    }
                    tr += "</tr>";
                    contentBody += tr;
                }
                #endregion Row Footer

                exportHtml = exportHtml.Replace("{{rowHeader}}", contentHeader);
                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                exportHtml = exportHtml.Replace("{{subHeader}}", multiCurrencyHeader);
                EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();
                AddHeaderPDF(htmlToPdfConverter, tenant.Name, sheetName, input.FromDate, input.ToDate, formatDate);
                AddFooterPDF(htmlToPdfConverter, user.FullName, formatDate);
                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
                var tokenName = $"{Guid.NewGuid()}.pdf";
                var result = new FileDto();
                result.FileName = $"{sheetName}.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);
                return result;

            });
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Purchasing)]
        [HttpPost]
        public async Task<PagedResultWithTotalColuumnsDto<GetListReportBillOutput>> GetBillReport(GetListReportBillInput input)
        {

            var vendoTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            var periodCycles = await GetCloseCyleAsync(input.ToDate);

            var previousCycle = periodCycles.Where(u => u.EndDate != null && u.EndDate.Value.Date <= input.ToDate.Date)
                                            .OrderByDescending(u => u.EndDate.Value).FirstOrDefault();

            var currentCycle = periodCycles.Where(u => u.StartDate.Date <= input.ToDate.Date &&
                                                (u.EndDate == null || input.ToDate.Date <= u.EndDate.Value.Date))
                                           .OrderByDescending(u => u.StartDate).FirstOrDefault();

            var currentRoundingDigit = currentCycle != null ? currentCycle.RoundingDigit :
                                       previousCycle != null ? previousCycle.RoundingDigit : 2;

            var currentRoundingDigitUnitCost = currentCycle != null ? currentCycle.RoundingDigitUnitCost :
                                               previousCycle != null ? previousCycle.RoundingDigitUnitCost : 2;

            var getVendorUserGroup = GetVendorGroup(AbpSession.GetUserId());

            var isFilterByAccountCurrency = input.CurrencyFilter != CurrencyFilter.MultiCurrencies;


            var defaultLocationGroups = await GetUserGroupByLocation();

            var journalItemQuery = from ji in _journalItemRepository.GetAll()
                                          .Where(s => s.Identifier.HasValue)
                                          .WhereIf(input.Accounts != null && input.Accounts.Count > 0, u => input.Accounts.Contains(u.AccountId))
                                          .WhereIf(input.AccountTypes != null && input.AccountTypes.Count > 0, u => u.Account != null && input.AccountTypes.Contains(u.Account.AccountTypeId))
                                          .AsNoTracking()
                                   join j in _journalRepository.GetAll()
                                           .Where(s => s.Status == TransactionStatus.Publish)
                                           .Where(u => u.JournalType == JournalType.Bill || u.JournalType == JournalType.VendorCredit)
                                           .WhereIf(defaultLocationGroups != null && defaultLocationGroups.Count > 0, u => defaultLocationGroups.Contains(u.LocationId.Value))
                                           .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId.Value))
                                           .WhereIf(input.JournalTypes != null && input.JournalTypes.Count > 0, s => input.JournalTypes.Contains(s.JournalType))
                                           .WhereIf(input.FromDate != null && input.ToDate != null,
                                                  (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                                           .WhereIf(!input.Filter.IsNullOrEmpty(), u =>
                                                     u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                                     u.Memo.ToLower().Contains(input.Filter.ToLower()) ||
                                                     u.Reference.ToLower().Contains(input.Filter.ToLower()))
                                           .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                                           .AsNoTracking()
                                   on ji.JournalId equals j.Id

                                   join b in _billRepository.GetAll()
                                           .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId))
                                           .WhereIf(vendoTypeMemberIds.Any(), s => vendoTypeMemberIds.Contains(s.Vendor.VendorTypeId.Value))
                                           .AsNoTracking()
                                   on j.BillId equals b.Id
                                   into bills
                                   from b in bills.DefaultIfEmpty()

                                   join bi in _billItemRepository.GetAll().AsNoTracking()
                                             .Include(u => u.Tax)
                                             .AsNoTracking()
                                   on ji.Identifier equals bi.Id
                                   into bItems
                                   from bi in bItems.DefaultIfEmpty()

                                   join vc in _vendorCreditRepository.GetAll()
                                           .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId))
                                           .WhereIf(vendoTypeMemberIds.Any(), s => vendoTypeMemberIds.Contains(s.Vendor.VendorTypeId.Value))
                                           .AsNoTracking()
                                   on j.VendorCreditId equals vc.Id
                                   into credits
                                   from vc in credits.DefaultIfEmpty()

                                   join vci in _vendorCreditItemRepository.GetAll().AsNoTracking()
                                             .AsNoTracking()
                                   on ji.Identifier equals vci.Id
                                   into vcItems
                                   from vci in vcItems.DefaultIfEmpty()

                                   where (b != null && bi != null) || (vc != null && vci != null)

                                   select new
                                   {
                                       CreationTimeIndex = j.CreationTimeIndex,
                                       CreationTime = j.CreationTime,
                                       Date = j.Date,
                                       AccountName = ji.Account.AccountName,
                                       AccountId = ji.AccountId,
                                       AccountCode = ji.Account.AccountCode,
                                       ItemId = bi != null ? bi.ItemId : vci.ItemId,
                                       LocationId = j.LocationId.Value,
                                       LocationName = j.Location.LocationName,
                                       Qty = bi != null ? bi.Qty : -vci.Qty,
                                       TaxId = bi != null ? bi.TaxId : vci.TaxId,
                                       TaxName = bi != null && bi.Tax != null ? bi.Tax.TaxName : vci != null && vci.Tax != null ? vci.Tax.TaxName : "",
                                       TaxRate = bi != null && bi.Tax != null ? bi.Tax.TaxRate : vci != null && vci.Tax != null ? vci.Tax.TaxRate : 0,
                                       Total = isFilterByAccountCurrency ? (bi != null ? bi.Total : -vci.Total) : (bi != null ? bi.MultiCurrencyTotal : -vci.MultiCurrencyTotal),
                                       UnitCost = isFilterByAccountCurrency ? (bi != null ? bi.UnitCost : vci.UnitCost) : (bi != null ? bi.MultiCurrencyUnitCost : vci.MultiCurrencyUnitCost),
                                       VendorId = b != null ? b.VendorId : vc.VendorId,
                                       VendorName = b != null ? b.Vendor.VendorName : vc.Vendor.VendorName,
                                       VendorType = b != null ? b.Vendor.VendorType.VendorTypeName : vc.Vendor.VendorType.VendorTypeName,
                                       VendorTypeId = b != null ? b.Vendor.VendorTypeId : vc.Vendor.VendorTypeId,
                                       JournalNo = j.JournalNo,
                                       Reference = j.Reference,
                                       User = j.CreatorUser.UserName,
                                       RoundingDigit = currentRoundingDigit,
                                       RoundingDigitUnitCost = currentRoundingDigitUnitCost,
                                       CurrencyId = isFilterByAccountCurrency ? j.CurrencyId : j.MultiCurrencyId,
                                       CurrencyCode = isFilterByAccountCurrency ? j.Currency.Code : j.MultiCurrency.Code,
                                       Description = bi != null ? bi.Description : vci.Description,
                                       IsItem = b != null ? b.IsItem : vc.IsItem
                                   };

            var userGroupItems = await GetUserGroupItems();
            var itemQuery = GetItemWithProperties(userGroupItems, input.Items, input.PropertyDics);

            var queryResult = from q in journalItemQuery
                                        .WhereIf(input.VendorTypes != null && input.VendorTypes.Count > 0, s => s.VendorTypeId.HasValue && input.VendorTypes.Contains(s.VendorTypeId.Value))

                              join vg in getVendorUserGroup
                              on q.VendorId equals vg.Id

                              join i in itemQuery
                              on q.ItemId equals i.Id
                              into list
                              from item in list.DefaultIfEmpty()
                              where (input.BillTypes == BillType.SelectAll ||
                                    (input.BillTypes == BillType.Item && q.IsItem) ||
                                    (input.BillTypes == BillType.Account && !q.IsItem))
                                    &&
                                    (
                                      (q.IsItem && item != null) ||
                                      (
                                          !q.IsItem &&
                                          (input.Items == null || input.Items.Count == 0) &&
                                          (input.PropertyDics == null || input.PropertyDics.Count == 0)
                                      )
                                    )

                              let itemCode = item != null ? item.ItemCode : q.AccountCode
                              let itemName = item != null ? item.ItemName : q.AccountName
                              let properties = item == null ? new List<Inventories.Data.ItemPropertySummary>() : item.Properties
                              let netWeight = item == null || item.Properties == null ? 0 : item.Properties
                                             .Where(t => t.IsUnit).Select(t => t.NetWeight * q.Qty).FirstOrDefault()
                              select new
                              {
                                  CreationTimeIndex = q.CreationTimeIndex,
                                  CreationTime = q.CreationTime,
                                  Date = q.Date.Date,
                                  AccountName = q.AccountName,
                                  AccountId = q.AccountId,
                                  AccountCode = q.AccountCode,
                                  ItemId = q.ItemId,
                                  ItemCode = itemCode,
                                  ItemName = itemName,
                                  LocationId = q.LocationId,
                                  LocationName = q.LocationName,
                                  Qty = q.Qty,
                                  TaxId = q.TaxId,
                                  TaxName = q.TaxName,
                                  TaxRate = q.TaxRate,
                                  Total = q.Total,
                                  UnitCost = q.UnitCost,
                                  VendorId = q.VendorId,
                                  VendorName = q.VendorName,
                                  VendorType = q.VendorType,
                                  JournalNo = q.JournalNo,
                                  Reference = q.Reference,
                                  User = q.User,
                                  Properties = properties,
                                  NetWeight = netWeight,
                                  Description = q.Description,
                                  CurrencyId = q.CurrencyId,
                                  CurrencyCode = q.CurrencyCode,
                              };

            var currencyColumns = (await queryResult.Select(u => new { u.CurrencyId, u.CurrencyCode }).Distinct().ToListAsync()).OrderBy(r => r.CurrencyCode);

            var query = from q in queryResult
                        select new GetListReportBillOutput
                        {
                            CreationTimeIndex = q.CreationTimeIndex,
                            CreationTime = q.CreationTime,
                            Date = q.Date.Date,
                            AccountName = q.AccountName,
                            AccountId = q.AccountId,
                            AccountCode = q.AccountCode,
                            ItemId = q.ItemId,
                            ItemCode = q.ItemCode,
                            ItemName = q.ItemName,
                            LocationId = q.LocationId,
                            LocationName = q.LocationName,
                            Qty = q.Qty,
                            TaxId = q.TaxId,
                            TaxName = q.TaxName,
                            TaxRate = q.TaxRate,
                            Total = q.Total,
                            UnitCost = q.UnitCost,
                            VendorId = q.VendorId,
                            VendorName = q.VendorName,
                            VendorType = q.VendorType,
                            JournalNo = q.JournalNo,
                            Reference = q.Reference,
                            User = q.User,
                            RoundingDigit = currentRoundingDigit,
                            RoundingDigitUnitCost = currentRoundingDigitUnitCost,
                            Properties = q.Properties,
                            NetWeight = q.NetWeight,
                            Description = q.Description,
                            CurrencyColumnTotals = currencyColumns.Select(s => new CurrencyColumnTotal
                            {
                                CurrencyId = s.CurrencyId.Value,
                                CurrencyCode = s.CurrencyCode,
                                Total = s.CurrencyId == q.CurrencyId ? q.Total : 0,
                            }).ToList(),
                        };


            List<SaleSummaryOut> sumQuery = null;
            var resultCount = 0;
            if (input.IsLoadMore == false)
            {
                //Query 2  
                if (input.ColumnNamesToSum != null)
                {
                    sumQuery = await query.Select(s => new SaleSummaryOut
                    {
                        JournalNo = s.JournalNo,
                        Total = s.Total,
                        CurrencyColumnTotals = s.CurrencyColumnTotals,
                        Qty = s.Qty,
                        NetWeight = s.NetWeight,
                    }).ToListAsync();

                    resultCount = sumQuery.Count();
                }
                else
                {
                    resultCount = await query.CountAsync();
                }
            }
            var sumOfColumns = new Dictionary<string, decimal>();
            var multiCurrencySumOfColumns = new Dictionary<string, Dictionary<string, decimal>>();

            if(resultCount == 0 && !input.IsLoadMore)
            {
                var rs = new PagedResultWithTotalColuumnsDto<GetListReportBillOutput>(resultCount, new List<GetListReportBillOutput>(), multiCurrencySumOfColumns);
                rs.TotalResult = sumOfColumns;
                return rs;
            }

            if (input.ColumnNamesToSum != null && input.IsLoadMore == false)
            {
                foreach (var col in input.ColumnNamesToSum)
                {
                    if (col == "Total")
                    {
                        if (!isFilterByAccountCurrency)
                        {
                            var groupByCurrencyDict = sumQuery.SelectMany(s => s.CurrencyColumnTotals).GroupBy(u => u.CurrencyCode).Select(u => new
                            {
                                CurrencyCode = u.Key,
                                Total = u.Sum(s => s.Total),
                            }).ToDictionary(u => u.CurrencyCode);

                            var multiSumOfColumns = new Dictionary<string, decimal>();
                            foreach (var c in currencyColumns)
                            {
                                multiSumOfColumns.Add(c.CurrencyCode, groupByCurrencyDict.ContainsKey(c.CurrencyCode) ? groupByCurrencyDict[c.CurrencyCode].Total : 0);
                            }

                            multiCurrencySumOfColumns.Add(col, multiSumOfColumns);
                        }
                        else
                        {
                            sumOfColumns.Add(col, sumQuery.Sum(u => u.Total));
                        }
                    }
                    else if (col == "NetWeight")
                    {
                        sumOfColumns.Add(col, sumQuery.Sum(u => u.NetWeight));
                    }
                    else if (col == "Qty")
                    {
                        sumOfColumns.Add(col, sumQuery.Sum(u => u.Qty));
                    }
                }
            }

            switch (input.GroupBy)
            {
                case "Item":
                    query = query.OrderBy(s => s.ItemCode).ThenBy(t => t.CreationTimeIndex).ThenBy(t => t.CreationTime);
                    break;
                case "Date":
                    query = query.OrderBy(s => s.Date).ThenBy(t => t.CreationTimeIndex).ThenBy(t => t.CreationTime);
                    break;
                case "Location":
                    query = query.OrderBy(s => s.LocationId).ThenBy(t => t.CreationTimeIndex).ThenBy(t => t.CreationTime);
                    break;
                case "Vendor":
                    query = query.OrderBy(s => s.VendorId).ThenBy(t => t.CreationTimeIndex).ThenBy(t => t.CreationTime);
                    break;
                case "Account":
                    query = query.OrderBy(s => s.AccountCode).ThenBy(t => t.CreationTimeIndex).ThenBy(t => t.CreationTime);
                    break;
                default:
                    query = query.OrderBy(input.Sorting).ThenBy(t => t.CreationTimeIndex).ThenBy(t => t.CreationTime);
                    break;
            }

            var @entities = new List<GetListReportBillOutput>();
            if (input.UsePagination == true)
            {
                @entities = await query.PageBy(input).ToListAsync();
            }
            else
            {
                @entities = await query.ToListAsync();
            }

            var returnResult = new PagedResultWithTotalColuumnsDto<GetListReportBillOutput>(resultCount, @entities, multiCurrencySumOfColumns);
            returnResult.TotalResult = sumOfColumns;

            return returnResult;
        }


        public ReportOutput GetReportTemplateBill()
        {
            var itemPropertyCols = _propertyRepository.GetAll().AsNoTracking()
                            .Where(t => t.IsActive == true)
                            .Select(t => new CollumnOutput
                            {
                                AllowGroupby = false,
                                AllowFilter = true,
                                ColumnName = t.Name,
                                ColumnLength = 150,
                                ColumnTitle = t.Name,
                                ColumnType = ColumnType.ItemProperty,
                                SortOrder = 11,
                                Visible = true,
                                AllowFunction = null,
                                MoreFunction = null,
                                IsDisplay = true,
                                AllowShowHideFilter = true,
                                ShowHideFilter = true,
                                DefaultValue = t.Id.ToString(),//to init the value of property
                            });

            var columnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Search",
                        ColumnLength = 150,
                        ColumnTitle = "Search",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "DateRange",
                        ColumnLength = 150,
                        ColumnTitle = "DateRange",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = true,
                        DefaultValue = "thisMonth",
                        AllowFunction = null,
                        DisableDefault = false,
                        MoreFunction = null,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "BillType",
                        ColumnLength = 300,
                        ColumnTitle = "BillType",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "JournalType",
                        ColumnLength = 300,
                        ColumnTitle = "Journal Type",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        DisableDefault = false,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                      new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Item",
                        ColumnLength = 150,
                        ColumnTitle = "Item",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                      new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = false,
                        ColumnName = "Date",
                        ColumnLength = 150,
                        ColumnTitle = "Date",
                        ColumnType = ColumnType.Date,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                     },
                      new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = false,
                        ColumnName = "JournalNo",
                        ColumnLength = 140,
                        ColumnTitle = "Journal No",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Reference",
                        ColumnLength = 140,
                        ColumnTitle = "Reference No",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                      new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Vendor",
                        ColumnLength = 150,
                        ColumnTitle = "Vendor",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                       new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "AccountType",
                        ColumnLength = 300,
                        ColumnTitle = "AccountType",
                        ColumnType = ColumnType.String,
                        SortOrder = 5,
                        Visible = true,
                        AllowFunction = null,
                        DisableDefault = false,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                      new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Account",
                        ColumnLength = 150,
                        ColumnTitle = "Account",
                        ColumnType = ColumnType.String,
                        SortOrder = 6,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                      new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Location",
                        ColumnLength = 120,
                        ColumnTitle = "Location",
                        ColumnType = ColumnType.String,
                        SortOrder = 7,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Tax",
                        ColumnLength = 120,
                        ColumnTitle = "Tax Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 8,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TaxRate",
                        ColumnLength = 110,
                        ColumnTitle = "Tax Rate",
                        ColumnType = ColumnType.Money,
                        SortOrder = 9,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemCode",
                        ColumnLength = 150,
                        ColumnTitle = "Item Code",
                        ColumnType = ColumnType.String,
                        SortOrder = 10,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemName",
                        ColumnLength = 150,
                        ColumnTitle = "Item Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 11,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Description",
                        ColumnLength = 150,
                        ColumnTitle = "Description",
                        ColumnType = ColumnType.String,
                        SortOrder = 12,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "UnitCost",
                        ColumnLength = 110,
                        ColumnTitle = "Unit Cost",
                        ColumnType = ColumnType.RoundingDigit,
                        SortOrder = 12,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Qty",
                        ColumnLength = 110,
                        ColumnTitle = "Qty",
                        ColumnType = ColumnType.RoundingDigit,
                        SortOrder = 13,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        AllowFunction = null,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Total",
                        ColumnLength = 110,
                        ColumnTitle = "Total",
                        ColumnType = ColumnType.RoundingDigit,
                        SortOrder = 14,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "NetWeight",
                        ColumnLength = 150,
                        ColumnTitle = "Net Weight",
                        ColumnType = ColumnType.RoundingDigit,
                        SortOrder = 15,
                        Visible = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "User",
                        ColumnLength = 110,
                        ColumnTitle = "User",
                        ColumnType = ColumnType.String,
                        SortOrder = 16,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "VendorType",
                        ColumnLength = 150,
                        ColumnTitle = "Vendor Type",
                        ColumnType = ColumnType.String,
                        SortOrder = 17,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                };

            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = columnInfo.Concat(itemPropertyCols).ToList(),
                Groupby = "",
                HeaderTitle = "Purchasing Report",
                Sortby = "",
            };

            if (!FeatureChecker.IsEnabled(AppFeatures.AccountingFeature))
            {
                result.ColumnInfo = result.ColumnInfo.Where(s =>
                    s.ColumnName != "Account" &&
                    s.ColumnName != "AccountType").ToList();
            }

            var isMultiCurrency = _multiCurrencyRepository.GetAll().Any();

            if (isMultiCurrency)
            {
                var currency = new CollumnOutput
                {
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "Currency",
                    ColumnLength = 140,
                    ColumnTitle = "Currency",
                    ColumnType = ColumnType.String,
                    SortOrder = 3,
                    Visible = true,
                    IsDisplay = false,
                    DisableDefault = false,
                    AllowShowHideFilter = true,
                    ShowHideFilter = true,
                };
                result.ColumnInfo.Add(currency);
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Purchasing_Export_Excel)]
        public async Task<FileDto> ExportExcelBillReport(GetBillReportInput input)
        {
            input.UsePagination = false;
            input.IsLoadMore = false;

            // Query get collumn header 
            //var @entity = await _reportTemplateManager.GetAsync(ReportType.ReportType_Outbounce);
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;

            var billData = (await GetBillReport(input)).Items;

            var result = new FileDto();

            //Creates a blank workbook. Use the using statment, so the package is disposed when we are done.
            using (var p = new ExcelPackage())
            {
                //A workbook must have at least on cell, so lets add one... 
                var ws = p.Workbook.Worksheets.Add(sheetName);
                ws.PrinterSettings.Orientation = eOrientation.Landscape;
                ws.PrinterSettings.FitToPage = true;
                //ws.PrinterSettings.PaperSize = ePaperSize.A3; //set default format paper size 
                ws.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                ws.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet

                var subCols = billData == null ? null : billData.First().CurrencyColumnTotals.Select(s => s.CurrencyCode).ToList();
                var isMultiCurrencies = input.CurrencyFilter == CurrencyFilter.MultiCurrencies && subCols != null && subCols.Count() > 1;

                #region Row 1 Header
                AddTextToCell(ws, 1, 1, sheetName, true);
                MergeCell(ws, 1, 1, 1, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 1


                #region Row 2 Header
                var header = input.ToDate.ToString("dd-MM-yyyy");
                AddTextToCell(ws, 2, 1, header, true);
                MergeCell(ws, 2, 1, 2, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 2

                #region Row 3 Header Table
                int rowTableHeader = 3;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                foreach (var i in reportCollumnHeader)
                {
                    if (isMultiCurrencies)
                    {
                        if (i.ColumnName == "Total")
                        {
                            var colWidth = i.ColumnLength;
                            var subColWidth = i.ColumnLength / subCols.Count();

                            AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                            ws.Column(colHeaderTable).Width = ConvertPixelToInches(colWidth);
                            MergeCell(ws, rowTableHeader, colHeaderTable, rowTableHeader, colHeaderTable + subCols.Count() - 1, ExcelHorizontalAlignment.Center);

                            foreach (var subCol in subCols)
                            {
                                AddTextToCell(ws, rowTableHeader + 1, colHeaderTable, subCol, true);
                                ws.Column(colHeaderTable).Width = ConvertPixelToInches(subColWidth);
                                colHeaderTable += 1;
                            }
                        }
                        else
                        {
                            AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                            ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                            MergeCell(ws, rowTableHeader, colHeaderTable, rowTableHeader + 1, colHeaderTable, ExcelHorizontalAlignment.Center);
                            colHeaderTable += 1;
                        }
                    }
                    else
                    {
                        AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                        ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                        colHeaderTable += 1;
                    }

                }

                if (isMultiCurrencies) rowTableHeader += 1;
                #endregion Row 3

                #region Row Body 
                // use for check auto dynamic col footer of sum value
                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                var groupBy = new List<GetListBillReportGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "Location":
                            groupBy = billData
                                .GroupBy(t => t.LocationId)
                                .Select(t => new GetListBillReportGroupByOutput
                                {
                                    KeyName = t.Select(a => a.LocationName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Account":
                            groupBy = billData
                                .GroupBy(t => t.AccountId)
                                .Select(t => new GetListBillReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.AccountName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;

                        case "Vendor":
                            groupBy = billData
                                .GroupBy(t => t.VendorId)
                                .Select(t => new GetListBillReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.VendorName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "Date":
                            groupBy = billData
                                .GroupBy(t => t.Date)
                                .Select(t => new GetListBillReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.Date.ToString()).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;

                        case "Item":
                            groupBy = billData
                                .GroupBy(t => t.ItemId)
                                .Select(t => new GetListBillReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.ItemCode != null ? x.ItemCode + "-" + x.ItemName : "").FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "JournalNo":
                            groupBy = billData
                               .GroupBy(t => t.JournalNo)
                               .Select(t => new GetListBillReportGroupByOutput
                               {
                                   KeyName = t.Select(x => x.JournalNo).FirstOrDefault(),
                                   Items = t.Select(x => x).ToList()
                               })
                               .ToList();
                            break;
                    }
                }
                // write body

                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0)
                {
                    foreach (var k in groupBy)
                    {
                        //key group by name
                        AddTextToCell(ws, rowBody, 1, k.KeyName, true);
                        MergeCell(ws, rowBody, 1, rowBody, reportCountColHead, ExcelHorizontalAlignment.Left);
                        rowBody += 1;
                        count = 1;
                        //list of item
                        foreach (var i in k.Items)
                        {
                            int collumnCellBody = 1;
                            foreach (var item in reportCollumnHeader)// map with correct key of properties 
                            {
                                if (item.ColumnName == "Total" && isMultiCurrencies)
                                {
                                    foreach (var subCol in subCols)
                                    {
                                        var model = i.CurrencyColumnTotals.FirstOrDefault(s => s.CurrencyCode == subCol);
                                        var cellValue = model.GetType().GetProperty(item.ColumnName)?.GetValue(model, null);
                                        var value = cellValue ?? 0;
                                        AddCellValue(ws, rowBody, collumnCellBody, item, value, i.RoundingDigit);
                                        collumnCellBody += 1;
                                    }
                                }
                                else
                                {
                                    WriteBodyBill(ws, rowBody, collumnCellBody, item, i, count);
                                    collumnCellBody += 1;
                                }

                            }
                            rowBody += 1;
                            count += 1;
                        }

                        //sub total of group by item
                        int collumnCellGroupBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {
                            if (item.AllowFunction != null)
                            {
                                if (isMultiCurrencies && item.ColumnName == "Total")
                                {
                                    foreach (var subCol in subCols)
                                    {
                                        var fromCell = GetAddressName(ws, rowBody - k.Items.Count, collumnCellGroupBody);
                                        var toCell = GetAddressName(ws, rowBody - 1, collumnCellGroupBody);

                                        var colKey = item.ColumnName + "-" + subCol;
                                        if (footerGroupDict.ContainsKey(colKey))
                                        {
                                            footerGroupDict[colKey] += fromCell + ":" + toCell + ",";
                                        }
                                        else
                                        {
                                            footerGroupDict.Add(colKey, fromCell + ":" + toCell + ",");
                                        }

                                        AddFormula(ws, rowBody, collumnCellGroupBody, "SUM(" + fromCell + ":" + toCell + ")", true);

                                        collumnCellGroupBody++;
                                    }

                                    collumnCellGroupBody--;
                                }
                                else
                                {
                                    var fromCell = GetAddressName(ws, rowBody - k.Items.Count, collumnCellGroupBody);
                                    var toCell = GetAddressName(ws, rowBody - 1, collumnCellGroupBody);
                                    if (footerGroupDict.ContainsKey(item.ColumnName))
                                    {
                                        footerGroupDict[item.ColumnName] += fromCell + ":" + toCell + ",";
                                    }
                                    else
                                    {
                                        footerGroupDict.Add(item.ColumnName, fromCell + ":" + toCell + ",");
                                    }
                                    AddFormula(ws, rowBody, collumnCellGroupBody, "SUM(" + fromCell + ":" + toCell + ")", true);
                                }
                            }
                            collumnCellGroupBody += 1;
                        }
                        rowBody += 1;
                    }
                }
                else
                {
                    foreach (var i in billData)
                    {
                        int collumnCellBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {
                            if (isMultiCurrencies && item.ColumnName == "Total")
                            {
                                foreach (var subCol in subCols)
                                {
                                    var model = i.CurrencyColumnTotals.FirstOrDefault(s => s.CurrencyCode == subCol);
                                    var cellValue = model.GetType().GetProperty(item.ColumnName)?.GetValue(model, null);
                                    var value = cellValue ?? 0;
                                    AddCellValue(ws, rowBody, collumnCellBody, item, value, i.RoundingDigit);
                                    collumnCellBody += 1;
                                }
                            }
                            else
                            {
                                WriteBodyBill(ws, rowBody, collumnCellBody, item, i, count);
                                collumnCellBody += 1;
                            }

                        }
                        rowBody += 1;
                        count += 1;
                    }
                }
                #endregion Row Body


                #region Row Footer 
                if (reportHasShowFooterTotal.Count > 0)
                {
                    int footerRow = rowBody;
                    int footerColNumber = 1;
                    AddTextToCell(ws, footerRow, footerColNumber, "TOTAL", true);
                    foreach (var i in reportCollumnHeader)
                    {
                        if (i.AllowFunction != null)
                        {
                            if (!input.GroupBy.IsNullOrWhiteSpace())
                            {
                                if (isMultiCurrencies && i.ColumnName == "Total")
                                {
                                    foreach (var subCol in subCols)
                                    {
                                        var colKey = i.ColumnName + "-" + subCol;
                                        int rowFooter = rowTableHeader + 1;// get start row after 
                                        var sumValue = footerGroupDict.ContainsKey(colKey) ? "SUM(" + footerGroupDict[colKey] + ")" : "";
                                        if (i.ColumnType == ColumnType.Number)
                                        {
                                            AddFormula(ws, footerRow, footerColNumber, sumValue, true, false, true);
                                        }
                                        else
                                        {
                                            AddFormula(ws, footerRow, footerColNumber, sumValue, true);
                                        }

                                        footerColNumber++;
                                    }

                                    footerColNumber--;
                                }
                                else
                                {
                                    //sum custom range cell depend on group item
                                    int rowFooter = rowTableHeader + 1;// get start row after 
                                    var sumValue = footerGroupDict.ContainsKey(i.ColumnName) ? "SUM(" + footerGroupDict[i.ColumnName] + ")" : "";
                                    if (i.ColumnType == ColumnType.Number)
                                    {
                                        AddFormula(ws, footerRow, footerColNumber, sumValue, true, false, true);
                                    }
                                    else
                                    {
                                        AddFormula(ws, footerRow, footerColNumber, sumValue, true);
                                    }
                                }
                            }
                            else
                            {
                                if (isMultiCurrencies && i.ColumnName == "Total")
                                {
                                    foreach (var subCol in subCols)
                                    {
                                        int rowFooter = rowTableHeader + 1;// get start row after header 
                                        var fromCell = GetAddressName(ws, rowFooter, footerColNumber);
                                        var toCell = GetAddressName(ws, footerRow - 1, footerColNumber);
                                        if (i.ColumnType == ColumnType.Number)
                                        {
                                            AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true, false, true);
                                        }
                                        else
                                        {
                                            AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true);
                                        }

                                        footerColNumber++;
                                    }

                                    footerColNumber--;
                                }
                                else
                                {
                                    int rowFooter = rowTableHeader + 1;// get start row after header 
                                    var fromCell = GetAddressName(ws, rowFooter, footerColNumber);
                                    var toCell = GetAddressName(ws, footerRow - 1, footerColNumber);
                                    if (i.ColumnType == ColumnType.Number)
                                    {
                                        AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true, false, true);
                                    }
                                    else
                                    {
                                        AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true);
                                    }
                                }
                            }
                        }
                        footerColNumber += 1;
                    }
                }
                #endregion Row Footer


                result.FileName = $"Puchasing_Report_{header.Replace(" ", "_").Replace("-", "_")}.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        #endregion

        #region Vendor Aging
        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Vendor_VendorAging)]
        public async Task<PagedResultWithTotalColuumnsDto<GetListVendorAgingReportOutput>> GetVendorAgingReport(GetListVendorAgingInput input)
        {
            var vendoTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            var userId = AbpSession.GetUserId();
            var filterByAccountingCurrency = input.CurrencyFilter != CurrencyFilter.MultiCurrencies;
            var vendorUserGroup = this.GetVendorGroup(userId);
            var previousCycle = await GetPreviousCloseCyleAsync(input.ToDate);
            var fromDateBeginning = previousCycle != null ? previousCycle.EndDate.Value.AddDays(1) : DateTime.MinValue;

            // get provious 
            var previousBillAndCredit = from ct in _vendorOpenBalanceRepository.GetAll()
                                            //.Where(s => previousCycle != null && previousCycle.EndDate != null && s.Date.Date == previousCycle.EndDate.Value.Date)
                                            .Where(s => previousCycle != null && s.AccountCycleId == previousCycle.Id)
                                            .Where(s => s.Key == JournalType.Bill || s.Key == JournalType.VendorCredit)
                                            .AsNoTracking()

                                        join u in _journalItemRepository.GetAll()
                                            .Where(s => s.Journal.Status == TransactionStatus.Publish)
                                           .Where(s => s.Identifier == null)
                                           .WhereIf(input.Accounts != null && input.Accounts.Count() > 0, u => input.Accounts.Contains(u.AccountId))
                                           .WhereIf(input.AccountTypes != null && input.AccountTypes.Count() > 0, u => input.AccountTypes.Contains(u.Account.AccountTypeId))

                                           .Where(s => s.Journal.BillId != null || s.Journal.VendorCreditId != null)
                                           .WhereIf(input.JournalType != null && input.JournalType.Count() > 0, u => input.JournalType.Contains(u.Journal.JournalType))

                                           .WhereIf(input.Vendors != null && input.Vendors.Count() > 0, u =>
                                                (u.Journal.Bill == null || input.Vendors.Contains(u.Journal.Bill.VendorId)) &&
                                                (u.Journal.VendorCredit == null || input.Vendors.Contains(u.Journal.VendorCredit.VendorId)))

                                           .WhereIf(input.VendorTypes != null && input.VendorTypes.Count() > 0, u =>
                                                ((u.Journal.Bill == null && u.Journal.Bill.Vendor == null) || input.VendorTypes.Contains(u.Journal.Bill.Vendor.VendorTypeId.Value)) &&
                                                ((u.Journal.VendorCredit == null && u.Journal.VendorCredit.Vendor == null) || input.VendorTypes.Contains(u.Journal.VendorCredit.Vendor.VendorTypeId.Value)))

                                           .WhereIf(input.Locations != null && input.Locations.Count() > 0,
                                                u => u.Journal.LocationId != null && input.Locations.Contains(u.Journal.LocationId.Value)).AsNoTracking()
                                           .AsNoTracking()

                                           .Select(s => new
                                           {
                                               TransactionId = s.Journal.BillId != null ? s.Journal.BillId : s.Journal.VendorCreditId,
                                               AccountName = s.Account.AccountName,
                                               AccountCode = s.Account.AccountCode,
                                               AccountId = s.AccountId,
                                               InvoiceOrCreditId = s.Journal.BillId != null ? s.Journal.BillId.Value : s.Journal.VendorCreditId.Value,
                                               JournalId = s.JournalId,
                                               JournalDate = s.Journal.Date,
                                               JournalMemo = s.Journal.Memo,
                                               JournalNo = s.Journal.JournalNo,
                                               JournalType = s.Journal.JournalType,
                                               CustomerId = s.Journal.BillId != null ? s.Journal.Bill.VendorId : s.Journal.VendorCredit.VendorId,
                                               CustomerName = s.Journal.BillId != null ? s.Journal.Bill.Vendor.VendorName : s.Journal.VendorCredit.Vendor.VendorName,
                                               VendorCode = s.Journal.BillId != null ? s.Journal.Bill.Vendor.VendorCode : s.Journal.VendorCredit.Vendor.VendorCode,
                                               VendorTypeId = s.Journal.BillId != null ? s.Journal.Bill.Vendor.VendorTypeId : s.Journal.VendorCredit.Vendor.VendorTypeId,
                                               Date = s.Journal.Date,
                                               Aging = (int)(input.ToDate - s.Journal.Date).TotalDays,
                                               Currency = filterByAccountingCurrency ?
                                             new CurrencyOutput
                                             {
                                                 Id = s.Journal.Currency.Id,
                                                 Code = s.Journal.Currency.Code,
                                                 Name = s.Journal.Currency.Name,
                                                 Symbol = s.Journal.Currency.Symbol,
                                             }
                                             : new CurrencyOutput
                                             {
                                                 Id = s.Journal.MultiCurrency.Id,
                                                 Code = s.Journal.MultiCurrency.Code,
                                                 Name = s.Journal.MultiCurrency.Name,
                                                 Symbol = s.Journal.MultiCurrency.Symbol,
                                             }

                                           })

                                        on ct.TransactionId equals u.TransactionId

                                        select new BillAndCreditVendorOutput
                                        {

                                            BillOrCreditId = u.TransactionId.Value,
                                            JournalId = u.JournalId,
                                            JournalDate = u.Date,
                                            JournalMemo = u.JournalMemo,
                                            JournalNo = u.JournalNo,
                                            JournalType = u.JournalType,
                                            VendorId = u.CustomerId,
                                            VendorName = u.CustomerName,
                                            VendorCode = u.VendorCode,
                                            VendorTypeId = u.VendorTypeId,
                                            Date = u.Date,
                                            Aging = (int)(input.ToDate - u.Date).TotalDays,
                                            BillBalanceAmount = ct.Key == JournalType.VendorCredit ? 0 : filterByAccountingCurrency ? ct.Balance : ct.MuliCurrencyBalance,
                                            BillTotalAmount = ct.Key == JournalType.VendorCredit ? 0 : filterByAccountingCurrency ? ct.Balance : ct.MuliCurrencyBalance,
                                            BillTotalPaidAmount = 0,
                                            CreditBalanceAmount = ct.Key == JournalType.Bill ? 0 : filterByAccountingCurrency ? ct.Balance : ct.MuliCurrencyBalance,
                                            CreditTotalAmount = ct.Key == JournalType.Bill ? 0 : filterByAccountingCurrency ? ct.Balance : ct.MuliCurrencyBalance,
                                            CreditTotalPaidAmount = 0,

                                            AccountId = u.AccountId,
                                            AccountCode = u.AccountCode,
                                            AccountName = u.AccountName,
                                            Currency = new CurrencyOutput
                                            {
                                                Id = u.Currency.Id,
                                                Code = u.Currency.Code,
                                                Name = u.Currency.Name,
                                                Symbol = u.Currency.Symbol,
                                            }

                                        };


            var newBillAndCreditByVendor = _vendorManager.GetBillAndCreditVendorQueryAsyn(
                                            fromDateBeginning,
                                            input.ToDate,
                                            input.Filter,
                                            input.Vendors,
                                            input.Accounts,
                                            input.JournalType,
                                            input.AccountTypes,
                                            input.CurrencyFilter,
                                            input.Locations,
                                            input.VendorTypes
                                        );


            var billAndCreditByVendor = newBillAndCreditByVendor.Concat(previousBillAndCredit)
                                        .WhereIf(vendoTypeMemberIds.Any(), s => vendoTypeMemberIds.Contains(s.VendorTypeId.Value));

            var currencyColumns = (await billAndCreditByVendor.Select(u => new { u.Currency.Id, u.Currency.Code }).Distinct().ToListAsync()).OrderBy(r => r.Code);


            var billPayments = from rpi in _paybillItemRepository.GetAll()
                               .WhereIf(input.Vendors != null && input.Vendors.Count() > 0,
                                            u => input.Vendors.Contains(u.VendorId))
                                        .WhereIf(input.VendorTypes != null && input.VendorTypes.Count() > 0,
                                           u => input.VendorTypes.Contains(u.Vendor.VendorTypeId.Value))

                                .AsNoTracking()

                               join j in _journalRepository.GetAll()
                                   .WhereIf(previousCycle != null && previousCycle.EndDate != null, u => u.Date.Date > previousCycle.EndDate.Value.Date)
                                   .WhereIf(fromDateBeginning != null && input.ToDate != null,
                                            (u => u.Date.Date >= fromDateBeginning.Date &&
                                            u.Date.Date <= input.ToDate.Date))
                                   .Where(u => u.PayBillId != null)
                                   .AsNoTracking()
                               on rpi.PayBillId equals j.PayBillId
                               orderby j.Date descending, j.CreationTimeIndex descending
                               select new
                               {
                                   CreationTimeIndex = j.CreationTimeIndex,
                                   VendorId = rpi.VendorId,
                                   Date = j.Date,
                                   PaymentAmount = filterByAccountingCurrency ? rpi.Payment : rpi.MultiCurrencyPayment,
                                   JounralId = j.Id,
                                   BillId = rpi.BillId,
                                   VendorCreditId = rpi.VendorCreditId,
                                   Currency = filterByAccountingCurrency ? j.Currency : j.MultiCurrency,
                               };



            //get bill and vendor credit balance by payment history
            var billAndVenderCreditBalance = from bvc in billAndCreditByVendor

                                             join p in billPayments
                                             on bvc.BillOrCreditId equals p.BillId into bp
                                             where bp != null

                                             join vp in billPayments
                                             on bvc.BillOrCreditId equals vp.VendorCreditId into vcp
                                             where vcp != null

                                             select new BillAndCreditVendorOutput
                                             {
                                                 CreationTimeIndex = bvc.CreationTimeIndex,
                                                 BillOrCreditId = bvc.BillOrCreditId,
                                                 JournalId = bvc.JournalId,
                                                 JournalDate = bvc.JournalDate,
                                                 JournalMemo = bvc.JournalMemo,
                                                 JournalNo = bvc.JournalNo,
                                                 JournalType = bvc.JournalType,
                                                 VendorId = bvc.VendorId,
                                                 VendorName = bvc.VendorName,
                                                 VendorCode = bvc.VendorCode,
                                                 Date = bvc.Date,

                                                 Aging = bvc.Aging,

                                                 BillTotalAmount = bvc.BillTotalAmount,
                                                 BillTotalPaidAmount = bp != null && bp.Any() ? bp.Select(p => p.PaymentAmount).Sum() : 0,
                                                 BillBalanceAmount = bp != null && bp.Any() ? bvc.BillTotalAmount - bp.Select(p => p.PaymentAmount).Sum() : bvc.BillTotalAmount,

                                                 CreditTotalAmount = bvc.CreditTotalAmount,
                                                 CreditTotalPaidAmount = vcp != null && vcp.Any() ? vcp.Select(p => p.PaymentAmount).Sum() : 0,
                                                 CreditBalanceAmount = vcp != null && vcp.Any() ? bvc.CreditTotalAmount - vcp.Select(p => p.PaymentAmount).Sum() : bvc.CreditTotalAmount,


                                                 AccountId = bvc.AccountId,
                                                 AccountCode = bvc.AccountCode,
                                                 AccountName = bvc.AccountName,
                                                 Currency = bvc.Currency,

                                             };

            var lastPayments = billPayments
                                .Where(s => s.BillId.HasValue)
                                .GroupBy(u => u.VendorId)
                                .Select(u => new
                                {
                                    VendorId = u.Key,
                                    LastBillPayments = u.GroupBy(s => s.JounralId)
                                                    .OrderByDescending(t => t.First().Date)
                                                    .Select(s => s.ToList())
                                                    .FirstOrDefault(),
                                });

            var finalList = (from v in vendorUserGroup
                             join b in billAndVenderCreditBalance on v.Id equals b.VendorId
                             into vb
                             where vb != null && vb.Count() > 0

                             join p in lastPayments
                                on v.Id equals p.VendorId into vp
                             from leftPaymentCredit in vp.DefaultIfEmpty()

                             select new GetListVendorAgingReportOutput
                             {
                                 CreationTimeIndex = vb.FirstOrDefault().CreationTimeIndex,
                                 VendorId = v.Id,
                                 VendorName = v.VendorName,

                                 LastPaymentDate = leftPaymentCredit != null &&
                                                   leftPaymentCredit.LastBillPayments != null &&
                                                   leftPaymentCredit.LastBillPayments.FirstOrDefault() != null ?
                                                   leftPaymentCredit.LastBillPayments.FirstOrDefault().Date.Date :
                                                   (DateTime?)null,
                                 Aging = leftPaymentCredit != null &&
                                                   leftPaymentCredit.LastBillPayments != null &&
                                                   leftPaymentCredit.LastBillPayments.FirstOrDefault() != null ?
                                                   (int)(input.ToDate.Date.AddDays(1).AddTicks(-1) -
                                                          leftPaymentCredit.LastBillPayments.FirstOrDefault().Date).TotalDays : 0,
                                 ColumnTotalByCurrencies = currencyColumns.Select(c => new ColumnTotalByCurrency
                                 {
                                     CurrencyId = c.Id,
                                     CurrencyCode = c.Code,

                                     Current = vb.Where(r => r.Currency.Id == c.Id && r.Aging <= 1).Select(s => s.BillBalanceAmount - s.CreditBalanceAmount).Sum(),
                                     Col30 = vb.Where(r => r.Currency.Id == c.Id && r.Aging > 1 && r.Aging <= 30).Select(s => s.BillBalanceAmount - s.CreditBalanceAmount).Sum(),
                                     Col60 = vb.Where(r => r.Currency.Id == c.Id && r.Aging > 30 && r.Aging <= 60).Select(s => s.BillBalanceAmount - s.CreditBalanceAmount).Sum(),
                                     Col90 = vb.Where(r => r.Currency.Id == c.Id && r.Aging > 60 && r.Aging <= 90).Select(s => s.BillBalanceAmount - s.CreditBalanceAmount).Sum(),
                                     Col90Up = vb.Where(r => r.Currency.Id == c.Id && r.Aging > 90).Select(s => s.BillBalanceAmount - s.CreditBalanceAmount).Sum(),
                                     Total = vb.Where(r => r.Currency.Id == c.Id).Select(s => s.BillBalanceAmount - s.CreditBalanceAmount).Sum(),

                                     LastPaymentAmount = leftPaymentCredit != null &&
                                                         leftPaymentCredit.LastBillPayments != null ?
                                                         leftPaymentCredit.LastBillPayments.Where(r => r.Currency.Id == c.Id)
                                                                                           .Select(s => s.PaymentAmount).Sum() : 0,

                                 }).ToList(),

                             })
                             .WhereIf(input.OpeningBalance != OpeningBalanceStatus.All, s =>
                                (input.OpeningBalance == OpeningBalanceStatus.Opening && s.ColumnTotalByCurrencies.Any(t => t.Total != 0)) ||
                                (input.OpeningBalance == OpeningBalanceStatus.Zero && s.ColumnTotalByCurrencies.All(t => t.Total == 0))
                             );


            var resultCount = 0;
            if (input.IsLoadMore == false)
            {
                resultCount = await finalList.CountAsync();
            }
            var @entities = new List<GetListVendorAgingReportOutput>();

            var sumOfColumns = new Dictionary<string, decimal>();
            var multiCurrencySumOfColumns = new Dictionary<string, Dictionary<string, decimal>>();

            if(resultCount == 0 && !input.IsLoadMore)
            {
                return new PagedResultWithTotalColuumnsDto<GetListVendorAgingReportOutput>(resultCount, @entities, multiCurrencySumOfColumns);
            }


            if (input.ColumnNamesToSum != null && input.IsLoadMore == false)
            {
                var sumList = await finalList.SelectMany(u => u.ColumnTotalByCurrencies).Select(u => u).ToListAsync();
                var groupByCurrencyDict = sumList.GroupBy(u => u.CurrencyId).Select(u => new
                {

                    CurrentId = u.Key,
                    Current = u.Sum(s => s.Current),
                    Col30 = u.Sum(s => s.Col30),
                    Col60 = u.Sum(s => s.Col60),
                    Col90 = u.Sum(s => s.Col90),
                    Col90Up = u.Sum(s => s.Col90Up),
                    //Total = u.Sum(s=>s.Total),
                    LastPayment = u.Sum(s => s.LastPaymentAmount),

                }).ToDictionary(u => u.CurrentId);

                foreach (var col in input.ColumnNamesToSum)
                {
                    foreach (var c in currencyColumns)
                    {
                        if (col == "Current") sumOfColumns.Add(c.Code, groupByCurrencyDict.ContainsKey(c.Id) ? groupByCurrencyDict[c.Id].Current : 0);// finalList.SelectMany(u => u.ColumnTotalByCurrencies).Where(r => r.CurrencyId == c.Id).Select(s => s.Current).Sum());
                        else if (col == "Col30") sumOfColumns.Add(c.Code, groupByCurrencyDict.ContainsKey(c.Id) ? groupByCurrencyDict[c.Id].Col30 : 0);// finalList.SelectMany(u => u.ColumnTotalByCurrencies).Where(r => r.CurrencyId == c.Id).Select(s => s.Col30).Sum());
                        else if (col == "Col60") sumOfColumns.Add(c.Code, groupByCurrencyDict.ContainsKey(c.Id) ? groupByCurrencyDict[c.Id].Col60 : 0);// finalList.SelectMany(u => u.ColumnTotalByCurrencies).Where(r => r.CurrencyId == c.Id).Select(s => s.Col60).Sum());
                        else if (col == "Col90") sumOfColumns.Add(c.Code, groupByCurrencyDict.ContainsKey(c.Id) ? groupByCurrencyDict[c.Id].Col90 : 0);// finalList.SelectMany(u => u.ColumnTotalByCurrencies).Where(r => r.CurrencyId == c.Id).Select(s => s.Col90).Sum());
                        else if (col == "Col90Up") sumOfColumns.Add(c.Code, groupByCurrencyDict.ContainsKey(c.Id) ? groupByCurrencyDict[c.Id].Col90Up : 0);// finalList.SelectMany(u => u.ColumnTotalByCurrencies).Where(r => r.CurrencyId == c.Id).Select(s => s.Col90Up).Sum());
                        else if (col == "Total")
                        {
                            var current = groupByCurrencyDict.ContainsKey(c.Id) ? groupByCurrencyDict[c.Id].Current : 0;
                            var col30 = groupByCurrencyDict.ContainsKey(c.Id) ? groupByCurrencyDict[c.Id].Col30 : 0;
                            var col60 = groupByCurrencyDict.ContainsKey(c.Id) ? groupByCurrencyDict[c.Id].Col60 : 0;
                            var col90 = groupByCurrencyDict.ContainsKey(c.Id) ? groupByCurrencyDict[c.Id].Col90 : 0;
                            var col90Up = groupByCurrencyDict.ContainsKey(c.Id) ? groupByCurrencyDict[c.Id].Col90Up : 0;
                            var total = current + col30 + col60 + col90 + col90Up;
                            sumOfColumns.Add(c.Code, total);// finalList.SelectMany(u => u.ColumnTotalByCurrencies).Where(r => r.CurrencyId == c.Id).Select(s => s.Total).Sum());

                        }
                        else if (col == "LastPaymentAmount") sumOfColumns.Add(c.Code, groupByCurrencyDict.ContainsKey(c.Id) ? groupByCurrencyDict[c.Id].LastPayment : 0);// finalList.SelectMany(u => u.ColumnTotalByCurrencies).Where(r => r.CurrencyId == c.Id).Select(s => s.LastPaymentAmount).Sum());

                    }

                    if (!multiCurrencySumOfColumns.ContainsKey(col))
                    {
                        multiCurrencySumOfColumns.Add(col, sumOfColumns);
                    }
                    else
                    {
                        multiCurrencySumOfColumns[col] = sumOfColumns;
                    }

                    sumOfColumns = new Dictionary<string, decimal>();

                }
            }

            if (input.UsePagination == true)
            {
                @entities = await finalList.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
            }
            else
            {
                @entities = await finalList.ToListAsync();
            }
            return new PagedResultWithTotalColuumnsDto<GetListVendorAgingReportOutput>(resultCount, @entities, multiCurrencySumOfColumns);
        }


        public ReportOutput GetReportTemplateVendorAging()
        {
            var IsMultiCurrency = _multiCurrencyRepository.GetAll();

            var result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "DateRange",
                        ColumnLength = 150,
                        ColumnTitle = "DateRange",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = true,
                        DefaultValue = "asOf",
                        AllowFunction = null,
                        DisableDefault = true,
                        MoreFunction = null,
                        IsDisplay = false,//no need to show in col check it belong to filter option
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                      },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "AccountType",
                        ColumnLength = 300,
                        ColumnTitle = "AccountType",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        DisableDefault = false,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Account",
                        ColumnLength = 300,
                        ColumnTitle = "Account",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        DisableDefault = false,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Vendor",
                        ColumnLength = 300,
                        ColumnTitle = "Vendor",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        DisableDefault = false,
                        IsDisplay = false,
                        DefaultValue = "Vendor",
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                      new CollumnOutput
                    {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "CurrencyCode",
                        ColumnLength = 140,
                        ColumnTitle = "CurrencyCode",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        IsDisplay = false,
                        DisableDefault = false,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },

                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "JournalType",
                        ColumnLength = 300,
                        ColumnTitle = "JournalType",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        DisableDefault = false,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },//End Of filter
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "VendorName",
                        ColumnLength = 200,
                        ColumnTitle = "Vendor",
                        ColumnType = ColumnType.String,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault =true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                     },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Current",
                        ColumnLength = 140,
                        ColumnTitle = "Current",
                        ColumnType = ColumnType.Money,
                        SortOrder = 5,
                        Visible = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                         MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true,
                        DisableDefault = false
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Col30",
                        ColumnLength = 140,
                        ColumnTitle = "1-30",
                        ColumnType = ColumnType.Money,
                        SortOrder = 6,
                        Visible = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                         MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        DisableDefault = false,
                        IsDisplay = true
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Col60",
                        ColumnLength = 140,
                        ColumnTitle = "30-60",
                        ColumnType = ColumnType.Money,
                        SortOrder = 7,
                        Visible = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                         MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true,
                        DisableDefault = false
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Col90",
                        ColumnLength = 140,
                        ColumnTitle = "60-90",
                        ColumnType = ColumnType.Money,
                        SortOrder = 8,
                        Visible = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                         MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true,
                        DisableDefault = false
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Col90Up",
                        ColumnLength = 140,
                        ColumnTitle = "> 90",
                        ColumnType = ColumnType.Money,
                        SortOrder = 9,
                        Visible = true,
                        DisableDefault = false,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                         MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                      new CollumnOutput{
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "Total",
                            ColumnLength = 140,
                            ColumnTitle = "Total",
                            ColumnType = ColumnType.Money,
                            SortOrder = 10,
                            Visible = true,
                            AllowFunction = "Sum",
                            IsDisplay = true,
                            DisableDefault = true,
                            AllowShowHideFilter = false,
                            ShowHideFilter = false,
                            MoreFunction = new List<MoreFunction>(){
                                new MoreFunction
                                {
                                    KeyName = null
                                },
                                new MoreFunction
                                {
                                    KeyName = "Sum"
                                }
                            },
                        },
                     new CollumnOutput
                    {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "LastPaymentDate",
                        ColumnLength = 140,
                        ColumnTitle = "Last Payment Date",
                        ColumnType = ColumnType.Date,
                        SortOrder = 11,
                        Visible = false,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput
                    {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "LastPaymentAmount",
                        ColumnLength = 140,
                        ColumnTitle = "Last Payment Amount",
                        ColumnType = ColumnType.Money,
                        SortOrder = 12,
                        Visible = false,
                        AllowFunction = "Sum",
                        IsDisplay = true,
                        DisableDefault = false,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput
                    {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Aging",
                        ColumnLength = 140,
                        ColumnTitle = "Aging",
                        ColumnType = ColumnType.Number,
                        SortOrder = 13,
                        Visible = false,
                        IsDisplay = true,
                        DisableDefault = false,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Location",
                        ColumnLength = 150,
                        ColumnTitle = "Location",
                        ColumnType = ColumnType.String,
                        SortOrder = 14,
                        Visible = false,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "VendorType",
                        ColumnLength = 150,
                        ColumnTitle = "Vendor Type",
                        ColumnType = ColumnType.String,
                        SortOrder = 14,
                        Visible = true,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "OpeningBalance",
                        ColumnLength = 300,
                        ColumnTitle = "Opening Balance",
                        ColumnType = ColumnType.Array,
                        SortOrder = 15,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        DefaultValue = Convert.ToInt32(OpeningBalanceStatus.All).ToString(),
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                },


                Groupby = "",
                HeaderTitle = "Vendor Aging",
                Sortby = "",
                DefaultTemplate = new DefaultSaveTemplateOutput
                {
                    ColumnName = "VendorByBill",
                    ColumnTitle = "VendorByBillTemplate",
                    DefaultValue = ReportType.ReportType_VendorByBill
                }
            };

            if (!FeatureChecker.IsEnabled(AppFeatures.AccountingFeature))
            {
                result.ColumnInfo = result.ColumnInfo.Where(s =>
                    s.ColumnName != "Account" &&
                    s.ColumnName != "AccountType").ToList();
            }

            if (IsMultiCurrency.Any())
            {

                var currency = new CollumnOutput
                {
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "Currency",
                    ColumnLength = 140,
                    ColumnTitle = "Currency",
                    ColumnType = ColumnType.String,
                    SortOrder = 3,
                    Visible = true,
                    IsDisplay = false,
                    DisableDefault = false,
                    AllowShowHideFilter = true,
                    ShowHideFilter = true,
                };
                result.ColumnInfo.Add(currency);
            }
            return result;
        }



        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Vendor_VendorAging_Export_Excel)]
        public async Task<FileDto> ExportExcelVendorAgingReport(GetVendorAgingReportInput input)
        {

            input.UsePagination = false;

            // Query get collumn header 
            //var @entity = await _reportTemplateManager.GetAsync(ReportType.ReportType_Outbounce);
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;

            // var isMultiCurrencies = input.CurrencyFilter == CurrencyFilter.MultiCurrencies;

            var resultList = await GetVendorAgingReport(input);
            var entity = resultList.Items;

            var result = new FileDto();

            //Creates a blank workbook. Use the using statment, so the package is disposed when we are done.
            using (var p = new ExcelPackage())
            {
                //A workbook must have at least on cell, so lets add one... 
                var ws = p.Workbook.Worksheets.Add(sheetName);
                ws.PrinterSettings.Orientation = eOrientation.Landscape;
                ws.PrinterSettings.FitToPage = true;
                //ws.PrinterSettings.PaperSize = ePaperSize.A3; //set default format paper size 
                ws.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                ws.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet


                var subCols = entity == null ? null : entity.First().ColumnTotalByCurrencies.Select(s => s.CurrencyCode).ToList();
                var sumCols = reportHasShowFooterTotal == null || subCols == null ? 0 : reportHasShowFooterTotal.Count() * (subCols.Count() - 1);
                var isMultiCurrencies = input.CurrencyFilter == CurrencyFilter.MultiCurrencies && subCols != null && subCols.Count() > 1;

                #region Row 1 Header
                AddTextToCell(ws, 1, 1, sheetName, true);
                MergeCell(ws, 1, 1, 1, reportCountColHead + sumCols, ExcelHorizontalAlignment.Center);
                #endregion Row 1


                #region Row 2 Header
                var header = input.ToDate.ToString("dd-MM-yyyy");
                AddTextToCell(ws, 2, 1, header, true);
                MergeCell(ws, 2, 1, 2, reportCountColHead + sumCols, ExcelHorizontalAlignment.Center);
                #endregion Row 2

                #region Row 3 Header Table



                int rowTableHeader = 3;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                foreach (var i in reportCollumnHeader)
                {
                    if ((i.ColumnType == ColumnType.Money || i.ColumnType == ColumnType.Number) && i.AllowFunction != null && subCols != null && subCols.Any() && isMultiCurrencies)
                    {
                        var colWidth = i.ColumnLength;
                        var subColWidth = i.ColumnLength / subCols.Count();

                        AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                        ws.Column(colHeaderTable).Width = ConvertPixelToInches(colWidth);
                        MergeCell(ws, rowTableHeader, colHeaderTable, rowTableHeader, colHeaderTable + subCols.Count() - 1, ExcelHorizontalAlignment.Center);

                        foreach (var subCol in subCols)
                        {
                            AddTextToCell(ws, rowTableHeader + 1, colHeaderTable, subCol, true);
                            ws.Column(colHeaderTable).Width = ConvertPixelToInches(subColWidth);

                            colHeaderTable += 1;
                        }
                    }
                    else
                    {
                        AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                        ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                        if (subCols != null && isMultiCurrencies) MergeCell(ws, rowTableHeader, colHeaderTable, rowTableHeader + 1, colHeaderTable, ExcelHorizontalAlignment.Center, ExcelVerticalAlignment.Center);

                        colHeaderTable += 1;
                    }
                }

                rowTableHeader += subCols != null && subCols.Any() && isMultiCurrencies ? 1 : 0;

                #endregion Row 3

                #region Row Body 
                // use for check auto dynamic col footer of sum value
                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                // write body
                foreach (var i in entity)
                {
                    int collumnCellBody = 1;
                    foreach (var item in reportCollumnHeader)// map with correct key of properties 
                    {

                        if ((item.ColumnType == ColumnType.Number || item.ColumnType == ColumnType.Money) && item.AllowFunction != null && subCols != null && subCols.Any())
                        {



                            foreach (var subCol in subCols)
                            {
                                var model = (ColumnTotalByCurrency)i.ColumnTotalByCurrencies.FirstOrDefault(s => s.CurrencyCode == subCol);

                                var cellValue = model.GetType().GetProperty(item.ColumnName)?.GetValue(model, null);

                                var value = cellValue ?? 0;

                                AddCellValue(ws, rowBody, collumnCellBody, item, value);

                                collumnCellBody += 1;
                            }

                        }
                        else
                        {
                            var value = i.GetType().GetProperty(item.ColumnName)?.GetValue(i, null);
                            AddCellValue(ws, rowBody, collumnCellBody, item, value);

                            collumnCellBody += 1;
                        }

                    }
                    rowBody += 1;
                    count += 1;
                }
                #endregion Row Body



                if (reportHasShowFooterTotal.Count > 0)
                {
                    int footerRow = rowBody;


                    int bodyEnd = rowBody;
                    int footerColNumber = 1;

                    //Total Label                    
                    AddTextToCell(ws, footerRow, footerColNumber, "Total", true);

                    footerColNumber++;

                    //Skip Vendor and Currency Columns
                    var columns = reportCollumnHeader.Count();
                    var sumColumns = reportCollumnHeader.Skip(1).Take(columns).ToList();


                    foreach (var i in sumColumns)
                    {
                        if (i.AllowFunction != null && subCols != null && subCols.Any())
                        {
                            foreach (var subCol in subCols)
                            {
                                int bodyStart = rowTableHeader + 1;// get start row after header 
                                var fromCell = GetAddressName(ws, bodyStart, footerColNumber);
                                var toCell = GetAddressName(ws, bodyEnd - 1, footerColNumber);


                                if (i.ColumnType == ColumnType.Number)
                                {
                                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true, false, true);
                                }
                                else
                                {
                                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true);
                                }

                                footerColNumber += 1;
                            }

                        }
                        else
                        {
                            footerColNumber += 1;
                        }

                    }




                    //else
                    //{
                    //    var sumGroupBy = (from e in entity
                    //                      group e by e.Currency.Code into pr
                    //                      select new
                    //                      {
                    //                          CurrencyCode = pr,
                    //                          Corrent = pr.Sum(t => t.Current),
                    //                          Col30 = pr.Sum(t => t.Col30),
                    //                          Col60 = pr.Sum(t => t.Col60),
                    //                          Col90 = pr.Sum(t => t.Col90),
                    //                          Col90Up = pr.Sum(t => t.Col90Up),
                    //                          Total = pr.Sum(t => t.Total),
                    //                          LastPaymentAmount = pr.Sum(t => t.LastPaymentAmount),

                    //                      }).ToList();

                    //    int footerRow = rowBody;
                    //    int footerColNumber = 1;
                    //    string OldCurrencyCode = "" ;
                    //    foreach (var e in sumGroupBy) {
                    //        AddTextToCell(ws, footerRow, footerColNumber, e.CurrencyCode?.ToString(), true);
                    //        foreach (var i in reportCollumnHeader)
                    //        {
                    //            if (i.AllowFunction != null)
                    //            {
                    //                int rowFooter = rowTableHeader + 1;// get start row after header 
                    //                var fromCell = GetAddressName(ws, rowFooter, footerColNumber);
                    //                var toCell = GetAddressName(ws, footerRow - 1, footerColNumber);
                    //                if (i.ColumnType == ColumnType.Number && e.CurrencyCode.ToString() == OldCurrencyCode?.ToString() )
                    //                {
                    //                     AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true, false, true);

                    //                }
                    //                else if (i.ColumnType == ColumnType.Number && e.CurrencyCode.ToString() != OldCurrencyCode?.ToString())
                    //                {
                    //                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true, false, true);
                    //                }
                    //                else
                    //                {
                    //                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true);
                    //                }
                    //            }
                    //            OldCurrencyCode = e.CurrencyCode.ToString();
                    //            footerColNumber += 1;
                    //        }
                    //    }

                    //}
                }


                #region Row Footer 





                //if (reportHasShowFooterTotal.Count > 0)
                //{
                //    int footerRow = rowBody;
                //    int footerColNumber = 1;
                //    AddTextToCell(ws, footerRow, footerColNumber, "TOTAL", true);
                //    foreach (var i in reportCollumnHeader)
                //    {
                //        if (i.AllowFunction != null)
                //        {
                //            int rowFooter = rowTableHeader + 1;// get start row after header 
                //            var fromCell = GetAddressName(ws, rowFooter, footerColNumber);
                //            var toCell = GetAddressName(ws, footerRow - 1, footerColNumber);
                //            if (i.ColumnType == ColumnType.Number)
                //            {
                //                AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true, false, true);
                //            }
                //            else
                //            {
                //                AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true);
                //            }
                //        }
                //        footerColNumber += 1;
                //    }
                //}
                #endregion Row Footer


                result.FileName = $"VendorAging_Report_{header.Replace(" ", "_").Replace("-", "_")}.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }


            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Vendor_VendorAging_Export_Pdf)]
        public async Task<FileDto> ExportPDFVendorAgingReport(GetVendorAgingReportInput input)
        {
            input.UsePagination = false;
            var tenant = await GetCurrentTenantAsync();
            var formatDate = await _formatRepository.GetAll()
                            .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
                            .Select(t => t.Web).FirstOrDefaultAsync();

            if (formatDate.IsNullOrEmpty())
            {
                formatDate = "dd/MM/yyyy";
            }
            var rounding = await GetCurrentCycleAsync();
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            var user = await GetCurrentUserAsync();
            var sheetName = report.HeaderTitle;

            var entity = await GetVendorAgingReport(input);

            return await Task.Run(async () =>
            {
                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                FileDto fileDto = new FileDto()
                {
                    SubFolder = null,
                    FileName = "InventoryReportPdf.pdf",
                    FileToken = "InventoryReportPdf.html",
                    FileType = MimeTypeNames.TextHtml
                };
                try
                {
                    templateHtml = await _fileStorageManager.GetTemplate(fileDto.FileToken);//retrive template from path
                    templateHtml = templateHtml.Trim();
                }
                catch (FileNotFoundException)
                {
                    throw new UserFriendlyException("FileNotFound");
                }
                //ToDo: Replace and concat string to be the same what frontend did
                exportHtml = templateHtml;
                var minusSubLength = 4;
                var contentBody = string.Empty;
                var contentHeader = string.Empty;
                var viewHeader = reportCollumnHeader.OrderBy(x => x.SortOrder);
                var documentWidth = 1080;
                decimal totalVisibleColsWidth = 0;
                decimal totalTableWidth = 0;
                int reportCountColHead = viewHeader.Where(t => t.Visible == true).Count();
                var multiCurrencyHeader = input.CurrencyFilter == CurrencyFilter.MultiCurrencies ? "<tr>" : "";
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (documentWidth >= (totalVisibleColsWidth + i.ColumnLength))
                        {
                            var rowHeader = "";
                            if (input.CurrencyFilter == CurrencyFilter.MultiCurrencies && entity.TotalResults.ContainsKey(i.ColumnName))
                            {
                                var index = 0;
                                var subColWidth = Convert.ToInt32(i.ColumnLength / entity.TotalResults[i.ColumnName].Count());
                                rowHeader = $"<th colspan='{entity.TotalResults[i.ColumnName].Count()}' width='{i.ColumnLength}px'>{i.ColumnTitle}</th>";

                                foreach (var subHeader in entity.TotalResults[i.ColumnName])
                                {
                                    //rowHeader += $"<div style='margin-top: 5px;margin:1px;border-top: 1px solid #000;float: left;text-align: right;width:{Math.Round((i.ColumnLength/entity.TotalResults[i.ColumnName].Count())- minusSubLength, 1)}px'>";
                                    //rowHeader += subHeader.Key;
                                    //rowHeader += "</div>";
                                    if (index > 0) reportCountColHead++;
                                    multiCurrencyHeader += $"<th style='width: {subColWidth}px;'>{subHeader.Key}</th>";
                                }
                                rowHeader += "</div></th>";
                            }
                            else
                            {
                                rowHeader = $"<th rowspan='2' width='{i.ColumnLength}px'>{i.ColumnTitle}</th>";
                            }
                            contentHeader += rowHeader;
                            totalTableWidth += i.ColumnLength;
                        }
                        else
                        {
                            i.Visible = false;
                        }
                        totalVisibleColsWidth += i.ColumnLength;
                    }
                }

                multiCurrencyHeader += input.CurrencyFilter == CurrencyFilter.MultiCurrencies ? $"</tr>" : "";
                exportHtml = exportHtml.Replace("{{tableWidth}}", totalTableWidth.ToString());

                #region Row Body 

                // write body

                foreach (var row in entity.Items)
                {
                    var tr = "<tr style='page-break-inside:avoid;' valign='top'>";
                    foreach (var i in viewHeader)
                    {
                        if (i.Visible)
                        {
                            if (i.ColumnName == "VendorName")
                            {
                                tr += $"<td>{row.VendorName}</td>";
                            }
                            else if (i.ColumnType == ColumnType.Money)
                            {
                                if (input.CurrencyFilter == CurrencyFilter.MultiCurrencies)//Multi currency 
                                {
                                    //tr += $"<td>";
                                    if (entity.TotalResults.ContainsKey(i.ColumnName))
                                    {
                                        foreach (var code in entity.TotalResults[i.ColumnName])
                                        {
                                            tr += $"<td style='text-align: right;'>";

                                            var sub = row.ColumnTotalByCurrencies.Where(t => t.CurrencyCode == code.Key).FirstOrDefault();
                                            if (i.ColumnName == "Current")
                                            {
                                                tr += $"{FormatNumberCurrency(Math.Round(sub.Current, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}";
                                            }
                                            else if (i.ColumnName == "Col30")
                                            {
                                                tr += $"{FormatNumberCurrency(Math.Round(sub.Col30, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}";
                                            }
                                            else if (i.ColumnName == "Col60")
                                            {
                                                tr += $"{FormatNumberCurrency(Math.Round(sub.Col60, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}";
                                            }
                                            else if (i.ColumnName == "Col90")
                                            {
                                                tr += $"{FormatNumberCurrency(Math.Round(sub.Col90, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}";
                                            }
                                            else if (i.ColumnName == "Col90Up")
                                            {
                                                tr += $"{FormatNumberCurrency(Math.Round(sub.Col90Up, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}";
                                            }
                                            else if (i.ColumnName == "Total")
                                            {
                                                tr += $"{FormatNumberCurrency(Math.Round(sub.Total, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}";
                                            }
                                            else if (i.ColumnName == "LastPaymentAmount")
                                            {
                                                tr += $"{FormatNumberCurrency(Math.Round(sub.LastPaymentAmount, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}";
                                            }
                                            tr += "</td>";
                                        }
                                    }
                                    //tr += "</td>";
                                }
                                else
                                {
                                    foreach (var sub in row.ColumnTotalByCurrencies)
                                    {

                                        if (i.ColumnName == "Current")
                                        {
                                            tr += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(sub.Current, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                        else if (i.ColumnName == "Col30")
                                        {
                                            tr += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(sub.Col30, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                        else if (i.ColumnName == "Col60")
                                        {
                                            tr += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(sub.Col60, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                        else if (i.ColumnName == "Col90")
                                        {
                                            tr += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(sub.Col90, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                        else if (i.ColumnName == "Col90Up")
                                        {
                                            tr += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(sub.Col90Up, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                        else if (i.ColumnName == "Total")
                                        {
                                            tr += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(sub.Total, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                        else if (i.ColumnName == "LastPaymentAmount")
                                        {
                                            tr += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(sub.LastPaymentAmount, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                        }
                                    }
                                }

                            }
                            else if (i.ColumnName == "LastPaymentDate")
                            {
                                var lastDatePayment = row.LastPaymentDate != null ? row.LastPaymentDate.Value.ToString(formatDate) : null;
                                tr += $"<td>{lastDatePayment}</td>";
                            }
                            else if (i.ColumnName == "Aging")
                            {
                                tr += $"<td style='text-align: right;'>{row.Aging}</td>";
                            }
                            else
                            {
                                tr += $"<td></td>";
                            }
                        }
                    }
                    tr += "</tr>";
                    contentBody += tr;
                }

                #endregion Row Body

                #region Row Footer 

                if (reportHasShowFooterTotal.Count > 0)
                {
                    var index = 0;
                    var tr = "<tr style='page-break-inside:avoid;' valign='top'>";
                    foreach (var i in viewHeader)
                    {
                        if (i.Visible)
                        {
                            if (!string.IsNullOrEmpty(i.AllowFunction) && entity.TotalResults.ContainsKey(i.ColumnName))/*listSum.Contains(i.ColumnName))*/
                            {
                                if (input.CurrencyFilter == CurrencyFilter.MultiCurrencies)
                                {
                                    //tr += $"<td>";
                                    foreach (var code in entity.TotalResults[i.ColumnName])
                                    {
                                        tr += $"<td style='text-align: right;'>";
                                        var sub = entity.TotalResults[i.ColumnName][code.Key];
                                        tr += $"{FormatNumberCurrency(Math.Round(sub, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}";
                                        tr += "</td>";
                                    }
                                    //tr += "</td>";
                                }
                                else
                                {
                                    tr += $"<td style='word-wrap:break-word;text-align: right;'>";
                                    foreach (var code in entity.TotalResults[i.ColumnName])
                                    {
                                        var sub = entity.TotalResults[i.ColumnName][code.Key];
                                        tr += $"{FormatNumberCurrency(Math.Round(sub, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}";

                                    }
                                    tr += "</td>";

                                }
                            }
                            else //none sum
                            {
                                if (index == 0)
                                {
                                    tr += $"<td style='font-weight: bold;'>{L("Total")}</td>";
                                }
                                else
                                {
                                    tr += $"<td></td>";
                                }
                            }
                            index++;
                        }
                    }
                    tr += "</tr>";
                    contentBody += tr;
                }
                #endregion Row Footer
                exportHtml = exportHtml.Replace("{{rowHeader}}", contentHeader);
                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                exportHtml = exportHtml.Replace("{{subHeader}}", multiCurrencyHeader);

                EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();
                AddHeaderPDF(htmlToPdfConverter, tenant.Name, sheetName, input.FromDate, input.ToDate, formatDate);
                AddFooterPDF(htmlToPdfConverter, user.FullName, formatDate);

                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
                var tokenName = $"{Guid.NewGuid()}.pdf";
                var result = new FileDto();
                result.FileName = $"{ReplaceSpecialCharacter(sheetName)}.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);
                return result;

            });
        }

        #endregion

        #region Vendor By Bill
        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Vendor_VendorByBill)]
        public async Task<PagedResultWithTotalColuumnsDto<GetVendorByBillReportOutput>> GetVendorByBillReport(GetListVendorAgingInput input)
        {

            var vendoTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            var filterByAccountingCurrency = input.CurrencyFilter != CurrencyFilter.MultiCurrencies;

            var previousCycle = await GetPreviousCloseCyleAsync(input.FromDate);

            var previousDay = input.FromDate.AddDays(-1);

            //var previousBillInvoices = _vendorManager.GetBillAndCreditVendorBillsWithBalanceQuery(AbpSession.GetUserId(),
            //                                            null, previousDay, input.Filter, input.Vendors,
            //                                            input.Accounts, input.JournalType, input.AccountTypes, input.Users, input.CurrencyFilter, input.Locations, input.VendorTypes)
            //                           .WhereIf(vendoTypeMemberIds.Any(), s => vendoTypeMemberIds.Contains(s.VendorTypeId.Value));

            //var newPeriodBillInvoiceResult = _vendorManager.GetBillAndCreditVendorBillsWithBalanceQuery(AbpSession.GetUserId(),
            //                                                input.FromDate,
            //                                                input.ToDate, input.Filter, input.Vendors,
            //                                                input.Accounts, input.JournalType,
            //                                                input.AccountTypes, input.Users, input.CurrencyFilter, input.Locations, input.VendorTypes)
            //                                .WhereIf(vendoTypeMemberIds.Any(), s => vendoTypeMemberIds.Contains(s.VendorTypeId.Value));

            var allQuery = _vendorManager.GetBillAndCreditVendorBillsWithBalanceQuery(AbpSession.GetUserId(),
                                                        null, input.ToDate, input.Filter, input.Vendors,
                                                        input.Accounts, input.JournalType, input.AccountTypes, input.Users, input.CurrencyFilter, input.Locations, input.VendorTypes)
                                       .WhereIf(vendoTypeMemberIds.Any(), s => vendoTypeMemberIds.Contains(s.VendorTypeId.Value));

            var previousBillInvoices = allQuery.Where(s => s.Date.Date < input.FromDate.Date);
            var newPeriodBillInvoiceResult = allQuery.Where(s => s.Date.Date >= input.FromDate.Date);

            //get all currencies from both query
            //var currencyCodes = (await previousBillInvoices.Concat(newPeriodBillInvoiceResult)
            //                    .Select(s => s.Currency.Code).GroupBy(u => u).Select(u => u.Key).ToListAsync()).OrderBy(r => r);
            var currencyCodes = (await allQuery
                                .Select(s => s.Currency.Code).GroupBy(u => u).Select(u => u.Key).ToListAsync()).OrderBy(r => r);


            //Generate currency columns
            var newPeriodBillInvoices = newPeriodBillInvoiceResult.Select(i => new GetVendorByBillReportOutput
            {
                CreationTimeIndex = i.CreationTimeIndex,
                User = i.User,
                TransactionId = i.TransactionId,
                VendorId = i.VendorId,
                VendorName = i.VendorName,
                VendorCode = i.VendorCode,
                LastPaymentAmounts = i.LastPaymentAmounts,
                LastPaymentDate = i.LastPaymentDate,
                Date = i.Date,
                AccountCode = i.AccountCode,
                AccountName = i.AccountName,
                Balance = i.Balance,
                Description = i.Description,
                JournalNo = i.JournalNo,
                Reference = i.Reference,
                JournalType = i.JournalType,
                ToDate = i.ToDate,
                TotalAmount = i.TotalAmount,
                TotalPaid = i.TotalPaid,
                Currency = i.Currency,
                Location = i.Location,
                TotalColsByCurency = currencyCodes.Select(u => new GetVendorByBillReportOutputItemByCurrency()
                {
                    CurrencyCode = u,
                    TotalAmounts = u == i.Currency.Code ? i.TotalAmount : 0,
                    TotalPaids = u == i.Currency.Code ? i.TotalPaid : 0,
                    Balances = u == i.Currency.Code ? i.Balance : 0,
                    LastPaymentAmounts = u == i.Currency.Code ? i.LastPaymentAmounts : 0,

                }).ToList(),


            }).OrderBy(u => u.VendorCode).ThenBy(u => u.Date);


            var previousBillInvoicesGroupByVendor = previousBillInvoices.GroupBy(u => new
            {
                u.VendorId,
                u.VendorCode,
                u.VendorName,
            }).
            Select(i => new GetVendorByBillReportOutput
            {
                CreationTimeIndex = null,
                VendorId = i.Key.VendorId,
                VendorName = i.Key.VendorName,
                VendorCode = i.Key.VendorCode,
                LastPaymentAmounts = 0,
                LastPaymentDate = (DateTime?)null,
                Date = previousDay,
                AccountCode = "",
                AccountName = "",
                Description = "",
                JournalNo = "",
                Reference = "",
                JournalType = null,
                ToDate = null,
                TotalAmount = filterByAccountingCurrency ? i.Sum(s => s.TotalAmount) : 0,
                TotalPaid = filterByAccountingCurrency ? i.Sum(s => s.TotalPaid) : 0,
                Balance = filterByAccountingCurrency ? i.Sum(s => s.Balance) : 0,
                TotalColsByCurency = currencyCodes.Select(u => new GetVendorByBillReportOutputItemByCurrency()
                {
                    CurrencyCode = u,
                    TotalAmounts = i.Where(r => r.Currency.Code == u).Select(s => s.TotalAmount).Sum(),
                    TotalPaids = i.Where(r => r.Currency.Code == u).Select(s => s.TotalPaid).Sum(),
                    Balances = i.Where(r => r.Currency.Code == u).Select(s => s.Balance).Sum(),
                    LastPaymentAmounts = i.Where(r => r.Currency.Code == u).Select(s => s.LastPaymentAmounts).Sum()
                }).ToList()
            }).OrderBy(u => u.VendorCode).ThenBy(u => u.Date);


            //var finalList = previousBillInvoicesGroupByVendor.Union(newPeriodBillInvoices);//.OrderBy(u => u.VendorCode).ThenBy(u => u.Date);


            var finalList = previousBillInvoicesGroupByVendor.Concat(newPeriodBillInvoices)
                .WhereIf(input.OpeningBalance != OpeningBalanceStatus.All, s =>
                    (input.OpeningBalance == OpeningBalanceStatus.Opening && s.Balance != 0) ||
                    (input.OpeningBalance == OpeningBalanceStatus.Zero && s.Balance == 0)
                )
                .OrderBy(u => u.VendorCode).ThenBy(u => u.Date.Date).ThenBy(t => t.CreationTimeIndex);


            var resultCount = 0;
            if (input.IsLoadMore == false)
            {
                resultCount = await finalList.CountAsync();
            }
            var @entities = new List<GetVendorByBillReportOutput>();
            var sumOfColumns = new Dictionary<string, decimal>();

            var multiCurrencySumOfColumns = new Dictionary<string, Dictionary<string, decimal>>();

            if(resultCount == 0 && !input.IsLoadMore)
            {
                return new PagedResultWithTotalColuumnsDto<GetVendorByBillReportOutput>(resultCount, @entities, multiCurrencySumOfColumns);
            }

            if (input.ColumnNamesToSum != null && input.IsLoadMore == false)
            {
                var sumList = await finalList.SelectMany(u => u.TotalColsByCurency).Select(u => u).ToListAsync();
                var groupByCurrencyDict = sumList.GroupBy(u => u.CurrencyCode).Select(u => new
                {
                    CurrencyCode = u.Key,
                    TotalPaid = u.Sum(s => s.TotalPaids),
                    TotalAmount = u.Sum(s => s.TotalAmounts),
                    //Balance = u.Sum(s => s.Balances),
                    LastPaymentAmounts = u.Sum(s => s.LastPaymentAmounts)

                }).ToDictionary(u => u.CurrencyCode);

                foreach (var col in input.ColumnNamesToSum)
                {
                    foreach (var cCode in currencyCodes)
                    {

                        if (col == "TotalPaid" || col == "TotalPaids") sumOfColumns.Add(cCode, groupByCurrencyDict.ContainsKey(cCode) ? groupByCurrencyDict[cCode].TotalPaid : 0);
                        else if (col == "TotalAmount" || col == "TotalAmounts") sumOfColumns.Add(cCode,
                            groupByCurrencyDict.ContainsKey(cCode) ? groupByCurrencyDict[cCode].TotalAmount : 0);
                        else if (col == "Balance" || col == "Balances")
                        {
                            var totalAmount = groupByCurrencyDict.ContainsKey(cCode) ? groupByCurrencyDict[cCode].TotalAmount : 0;
                            var totalPaid = groupByCurrencyDict.ContainsKey(cCode) ? groupByCurrencyDict[cCode].TotalPaid : 0;
                            var balance = totalAmount - totalPaid;

                            sumOfColumns.Add(cCode, balance);

                        }
                        //else if (col == "TotalPaids") sumOfColumns.Add(cCode, finalList.SelectMany(s => s.TotalColsByCurency).Where(r => r.CurrencyCode == cCode).Select(s => s.TotalAmounts).Sum());
                        //else if (col == "TotalAmounts") sumOfColumns.Add(cCode, finalList.SelectMany(s => s.TotalColsByCurency).Where(r => r.CurrencyCode == cCode).Select(s => s.TotalPaids).Sum());
                        //else if (col == "Balances") sumOfColumns.Add(cCode, finalList.SelectMany(s => s.TotalColsByCurency).Where(r => r.CurrencyCode == cCode).Select(s => s.Balances).Sum());
                        else if (col == "LastPaymentAmounts") sumOfColumns.Add(cCode,
                                                            groupByCurrencyDict.ContainsKey(cCode) ? groupByCurrencyDict[cCode].LastPaymentAmounts : 0);

                    }

                    if (!multiCurrencySumOfColumns.ContainsKey(col))
                    {
                        multiCurrencySumOfColumns.Add(col, sumOfColumns);
                    }
                    else
                    {
                        multiCurrencySumOfColumns[col] = sumOfColumns;
                    }

                    sumOfColumns = new Dictionary<string, decimal>();

                }
            }

            
            if (!input.GroupBy.IsNullOrWhiteSpace())
            {
                finalList = finalList.OrderBy(s => s.VendorCode).ThenBy(s => s.CreationTimeIndex);
            }
            else
            {
                finalList = finalList.OrderBy(input.Sorting).ThenBy(s => s.CreationTimeIndex);
            }


            if (input.UsePagination == true)
            {
                @entities = await finalList.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
            }
            else
            {
                @entities = await finalList.ToListAsync();
            }


            return new PagedResultWithTotalColuumnsDto<GetVendorByBillReportOutput>(resultCount, @entities, multiCurrencySumOfColumns);

        }

       
        public ReportOutput GetReportTemplateVendorByBill()
        {
            var IsMultiCurrency = _multiCurrencyRepository.GetAll();
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "DateRange",
                        ColumnLength = 150,
                        ColumnTitle = "DateRange",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = true,
                        DefaultValue = "thisMonth",
                        AllowFunction = null,
                        DisableDefault = false,
                        MoreFunction = null,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "AccountType",
                        ColumnLength = 300,
                        ColumnTitle = "AccountType",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        DisableDefault = false,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Account",
                        ColumnLength = 300,
                        ColumnTitle = "Account",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        DisableDefault = false,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "Vendor",
                        ColumnLength = 300,
                        ColumnTitle = "Vendor",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        DisableDefault = false,
                        IsDisplay = false,
                        DefaultValue = "Vendor",
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "JournalType",
                        ColumnLength = 300,
                        ColumnTitle = "JournalType",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        DisableDefault = false,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },//End Of filter
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Date",
                        ColumnLength = 130,
                        ColumnTitle = "Date",
                        ColumnType = ColumnType.Date,
                        SortOrder = 5,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "JournalNo",
                        ColumnLength = 140,
                        ColumnTitle = "Journal No",
                        ColumnType = ColumnType.String,
                        SortOrder = 6,
                        Visible = true,
                        IsDisplay = true,
                        DisableDefault = false,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Reference",
                        ColumnLength = 140,
                        ColumnTitle = "Reference",
                        ColumnType = ColumnType.String,
                        SortOrder = 6,
                        Visible = true,
                        IsDisplay = true,
                        DisableDefault = false,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "JournalType",
                        ColumnLength = 140,
                        ColumnTitle = "Journal Type",
                        ColumnType = ColumnType.StatusCode,
                        SortOrder = 4,
                        Visible = true,
                        DisableDefault = false,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "AccountCode",
                        ColumnLength = 140,
                        ColumnTitle = "Account Code",
                        ColumnType = ColumnType.String,
                        SortOrder = 8,
                        Visible = true,
                        IsDisplay = true,
                        DisableDefault = false,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "AccountName",
                        ColumnLength = 150,
                        ColumnTitle = "Account Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 9,
                        Visible = true,
                        IsDisplay = true,
                        DisableDefault = false,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "VendorName",
                        ColumnLength = 150,
                        ColumnTitle = "Vendor Name",
                        ColumnType = ColumnType.String,
                        SortOrder =10,
                        Visible = true,
                        IsDisplay = true,
                        DisableDefault = false,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Description",
                        ColumnLength = 140,
                        ColumnTitle = "Description",
                        ColumnType = ColumnType.String,
                        SortOrder = 11,
                        Visible = true,
                        DisableDefault = false,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalAmount",
                        ColumnLength = 150,
                        ColumnTitle = "Total Amount",
                        ColumnType = ColumnType.Money,
                        SortOrder = 12,
                        Visible = true,
                        IsDisplay = true,
                        DisableDefault = false,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalPaid",
                        ColumnLength = 150,
                        ColumnTitle = "Total Paid",
                        ColumnType = ColumnType.Money,
                        SortOrder =13,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Balance",
                        ColumnLength = 150,
                        ColumnTitle = "Balance",
                        ColumnType = ColumnType.Money,
                        SortOrder = 14,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput
                    {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "LastPaymentDate",
                        ColumnLength = 180,
                        ColumnTitle = "Last Payment Date",
                        ColumnType = ColumnType.Date,
                        SortOrder = 15,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput
                    {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Aging",
                        ColumnLength = 90,
                        ColumnTitle = "Aging",
                        ColumnType = ColumnType.Number,
                        SortOrder = 16,
                        Visible = true,
                        IsDisplay = true,
                        DisableDefault = false,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                       new CollumnOutput
                    {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "LastPaymentAmounts",
                        ColumnLength = 190,
                        ColumnTitle = "Last Payment Amount",
                        ColumnType = ColumnType.Money,
                        SortOrder = 17,
                        Visible = true,
                        AllowFunction = "Sum",
                        IsDisplay = true,
                        DisableDefault = false,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                       new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalAmounts",
                        ColumnLength = 150,
                        ColumnTitle = "Total Amounts",
                        ColumnType = ColumnType.Money,
                        SortOrder = 18,
                        Visible = true,
                        IsDisplay = true,
                        DisableDefault = false,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalPaids",
                        ColumnLength = 150,
                        ColumnTitle = "Total Paids",
                        ColumnType = ColumnType.Money,
                        SortOrder =19,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                       new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Balances",
                        ColumnLength = 150,
                        ColumnTitle = "Balances",
                        ColumnType = ColumnType.Money,
                        SortOrder = 20,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                    },
                    new CollumnOutput
                    {
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "User",
                        ColumnLength = 190,
                        ColumnTitle = "User",
                        ColumnType = ColumnType.Object,
                        SortOrder = 21,
                        Visible = true,
                        IsDisplay = true,
                        DisableDefault = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Location",
                        ColumnLength = 150,
                        ColumnTitle = "Location",
                        ColumnType = ColumnType.String,
                        SortOrder = 22,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },

                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "VendorType",
                        ColumnLength = 150,
                        ColumnTitle = "Vendor Type",
                        ColumnType = ColumnType.String,
                        SortOrder = 23,
                        Visible = true,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },

                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "OpeningBalance",
                        ColumnLength = 300,
                        ColumnTitle = "Opening Balance",
                        ColumnType = ColumnType.Array,
                        SortOrder = 24,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        DefaultValue = Convert.ToInt32(OpeningBalanceStatus.All).ToString(),
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },

                },
                Groupby = "",
                HeaderTitle = "Vendor By Bill",
                Sortby = "",
            };

            if (!FeatureChecker.IsEnabled(AppFeatures.AccountingFeature))
            {
                result.ColumnInfo = result.ColumnInfo.Where(s =>
                    s.ColumnName != "Account" &&
                    s.ColumnName != "AccountType" &&
                    s.ColumnName != "AccountCode" &&
                    s.ColumnName != "AccountName").ToList();
            }

            if (IsMultiCurrency.Any())
            {
                var currency = new CollumnOutput
                {
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "Currency",
                    ColumnLength = 140,
                    ColumnTitle = "Currency",
                    ColumnType = ColumnType.String,
                    SortOrder = 11,
                    Visible = false,
                    IsDisplay = true,
                    DisableDefault = false,
                    AllowShowHideFilter = true,
                    ShowHideFilter = true,
                };
                result.ColumnInfo.Add(currency);
            }
            result.ColumnInfo = result.ColumnInfo.OrderBy(t => t.SortOrder).ToList();
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Vendor_VendorByBill_Export_Excel)]
        public async Task<FileDto> ExportExcelVendorByBillReport(GetVendorAgingReportInput input)
        {
            input.UsePagination = false;

            // Query get collumn header 
            //var @entity = await _reportTemplateManager.GetAsync(ReportType.ReportType_Outbounce);
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;

            var entity = (await GetVendorByBillReport(input)).Items;

            var result = new FileDto();


            var isMultiCurrencies = input.CurrencyFilter == CurrencyFilter.MultiCurrencies;
            var firstRow = entity.FirstOrDefault();
            var currencyCodes = firstRow == null ? new List<string>() : firstRow.TotalColsByCurency.Select(s => s.CurrencyCode).ToList();


            //Creates a blank workbook. Use the using statment, so the package is disposed when we are done.
            using (var p = new ExcelPackage())
            {
                //A workbook must have at least on cell, so lets add one... 
                var ws = p.Workbook.Worksheets.Add(sheetName);
                ws.PrinterSettings.Orientation = eOrientation.Landscape;
                ws.PrinterSettings.FitToPage = true;
                //ws.PrinterSettings.PaperSize = ePaperSize.A3; //set default format paper size 
                ws.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                ws.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet

                #region Row 1 Header
                AddTextToCell(ws, 1, 1, sheetName, true);
                MergeCell(ws, 1, 1, 1, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 1


                #region Row 2 Header
                var header = input.ToDate.ToString("dd-MM-yyyy");
                AddTextToCell(ws, 2, 1, header, true);
                MergeCell(ws, 2, 1, 2, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 2


                #region Row 3 Header Table
                int rowTableHeader = 3;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                foreach (var i in reportCollumnHeader)
                {
                    if (isMultiCurrencies && i.AllowFunction != null && currencyCodes.Any())
                    {
                        AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                        ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                        MergeCell(ws, rowTableHeader, colHeaderTable, rowTableHeader, colHeaderTable + currencyCodes.Count() - 1, ExcelHorizontalAlignment.Center, ExcelVerticalAlignment.Center);

                        foreach (var subCol in currencyCodes)
                        {
                            AddTextToCell(ws, rowTableHeader + 1, colHeaderTable, subCol, true);
                            ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength / currencyCodes.Count());
                            colHeaderTable += 1;
                        }
                    }
                    else
                    {
                        AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                        ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                        if (isMultiCurrencies) MergeCell(ws, rowTableHeader, colHeaderTable, rowTableHeader + 1, colHeaderTable, ExcelHorizontalAlignment.Center, ExcelVerticalAlignment.Center);
                        colHeaderTable += 1;
                    }
                }

                if (isMultiCurrencies) rowTableHeader++;

                #endregion Row 3

                #region Row Body 
                // use for check auto dynamic col footer of sum value
                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();

                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                // write body

                var groupBy = new List<GetListVendorByBillReportGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "Vendor":
                            groupBy = entity
                                .GroupBy(t => t.VendorId)
                                .Select(t => new GetListVendorByBillReportGroupByOutput
                                {
                                    KeyName = t.Select(a => a.VendorName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;

                    }
                }
                // write body

                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0)
                {
                    foreach (var k in groupBy)
                    {
                        //key group by name
                        AddTextToCell(ws, rowBody, 1, k.KeyName, true);
                        MergeCell(ws, rowBody, 1, rowBody, reportCountColHead, ExcelHorizontalAlignment.Left);
                        rowBody += 1;
                        count = 1;
                        //list of item
                        foreach (var i in k.Items)
                        {
                            int collumnCellBody = 1;
                            foreach (var item in reportCollumnHeader)// map with correct key of properties 
                            {
                                if (isMultiCurrencies && item.AllowFunction != null)
                                {
                                    foreach (var subCol in currencyCodes)
                                    {
                                        var model = (GetVendorByBillReportOutputItemByCurrency)i.TotalColsByCurency.FirstOrDefault(s => s.CurrencyCode == subCol);
                                        var cellValue = model.GetType().GetProperty(item.ColumnName)?.GetValue(model, null);
                                        var value = cellValue ?? 0;

                                        AddCellValue(ws, rowBody, collumnCellBody, item, value);

                                        collumnCellBody += 1;
                                    }
                                }
                                else
                                {

                                    var value = i.GetType().GetProperty(item.ColumnName)?.GetValue(i, null);

                                    if (value != null && value is UserDto) value = ((UserDto)value).UserName;

                                    AddCellValue(ws, rowBody, collumnCellBody, item, value);

                                    collumnCellBody += 1;
                                }

                            }
                            rowBody += 1;
                            count += 1;
                        }

                        //sub total of group by item
                        int collumnCellGroupBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {
                            if (item.AllowFunction != null)
                            {
                                if (isMultiCurrencies)
                                {
                                    foreach (var subCol in currencyCodes)
                                    {
                                        var fromCell = GetAddressName(ws, rowBody - k.Items.Count, collumnCellGroupBody);
                                        var toCell = GetAddressName(ws, rowBody - 1, collumnCellGroupBody);

                                        var keyCol = item.ColumnName + "-" + subCol;

                                        if (footerGroupDict.ContainsKey(keyCol))
                                        {
                                            footerGroupDict[keyCol] += fromCell + ":" + toCell + ",";
                                        }
                                        else
                                        {
                                            footerGroupDict.Add(keyCol, fromCell + ":" + toCell + ",");
                                        }

                                        AddFormula(ws, rowBody, collumnCellGroupBody, "SUM(" + fromCell + ":" + toCell + ")", true);



                                        collumnCellGroupBody += 1;
                                    }


                                    collumnCellGroupBody--;
                                }
                                else
                                {
                                    var fromCell = GetAddressName(ws, rowBody - k.Items.Count, collumnCellGroupBody);
                                    var toCell = GetAddressName(ws, rowBody - 1, collumnCellGroupBody);

                                    var subCol = currencyCodes.FirstOrDefault();

                                    var keyCol = item.ColumnName + "-" + subCol;

                                    if (footerGroupDict.ContainsKey(keyCol))
                                    {
                                        footerGroupDict[keyCol] += fromCell + ":" + toCell + ",";
                                    }
                                    else
                                    {
                                        footerGroupDict.Add(keyCol, fromCell + ":" + toCell + ",");
                                    }


                                    AddFormula(ws, rowBody, collumnCellGroupBody, "SUM(" + fromCell + ":" + toCell + ")", true);
                                }
                            }
                            collumnCellGroupBody += 1;
                        }
                        rowBody += 1;
                    }
                }
                else
                {
                    foreach (var i in entity)
                    {
                        int collumnCellBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {
                            if (isMultiCurrencies && item.AllowFunction != null)
                            {
                                foreach (var subCol in currencyCodes)
                                {
                                    var cell = i.TotalColsByCurency.FirstOrDefault(s => s.CurrencyCode == subCol);
                                    var value = cell.GetType().GetProperty(item.ColumnName).GetValue(cell, null);

                                    AddCellValue(ws, rowBody, collumnCellBody, item, value);

                                    collumnCellBody += 1;
                                }
                            }
                            else
                            {

                                var value = i.GetType().GetProperty(item.ColumnName)?.GetValue(i, null);
                                AddCellValue(ws, rowBody, collumnCellBody, item, value);

                                collumnCellBody += 1;
                            }

                        }
                        rowBody += 1;
                        count += 1;
                    }
                }
                #endregion Row Body


                #region Row Footer 
                if (reportHasShowFooterTotal.Count > 0)
                {
                    int footerRow = rowBody;
                    int footerColNumber = 1;
                    AddTextToCell(ws, footerRow, footerColNumber, "TOTAL", true);
                    foreach (var i in reportCollumnHeader)
                    {
                        if (i.AllowFunction != null)
                        {
                            if (footerGroupDict.Any())
                            {
                                if (isMultiCurrencies)
                                {
                                    foreach (var subCol in currencyCodes)
                                    {
                                        var sumRanges = footerGroupDict[i.ColumnName + "-" + subCol];

                                        if (i.ColumnType == ColumnType.Number)
                                        {
                                            AddFormula(ws, footerRow, footerColNumber, "SUM(" + sumRanges + ")", true, false, true);
                                        }
                                        else
                                        {
                                            AddFormula(ws, footerRow, footerColNumber, "SUM(" + sumRanges + ")", true);
                                        }

                                        footerColNumber += 1;
                                    }

                                    footerColNumber--;
                                }
                                else
                                {

                                    var subCol = currencyCodes.FirstOrDefault();
                                    var sumRanges = footerGroupDict[i.ColumnName + "-" + subCol];

                                    if (i.ColumnType == ColumnType.Number)
                                    {
                                        AddFormula(ws, footerRow, footerColNumber, "SUM(" + sumRanges + ")", true, false, true);
                                    }
                                    else
                                    {
                                        AddFormula(ws, footerRow, footerColNumber, "SUM(" + sumRanges + ")", true);
                                    }
                                }
                            }
                            else
                            {
                                int rowFooter = rowTableHeader + 1;// get start row after header 
                                var fromCell = GetAddressName(ws, rowFooter, footerColNumber);
                                var toCell = GetAddressName(ws, footerRow - 1, footerColNumber);
                                if (i.ColumnType == ColumnType.Number)
                                {
                                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true, false, true);
                                }
                                else
                                {
                                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true);
                                }
                            }
                        }
                        footerColNumber += 1;
                    }
                }
                #endregion Row Footer


                result.FileName = $"VendorByBill_Report_{header.Replace(" ", "_").Replace("-", "_")}.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Vendor_VendorByBill_Export_Pdf)]
        public async Task<FileDto> ExportPDFVendorByBillReport(GetVendorAgingReportInput input)
        {
            input.UsePagination = false;
            var tenant = await GetCurrentTenantAsync();
            var formatDate = await _formatRepository.GetAll()
                            .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
                            .Select(t => t.Web).FirstOrDefaultAsync();
            if (formatDate.IsNullOrEmpty())
            {
                formatDate = "dd/MM/yyyy";
            }
            var digit = (await GetCurrentCycleAsync()).RoundingDigit;
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();

            var sheetName = report.HeaderTitle;
            var user = await GetCurrentUserAsync();
            var entity = await GetVendorByBillReport(input);

            return await Task.Run(async () =>
            {
                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                FileDto fileDto = new FileDto()
                {
                    SubFolder = null,
                    FileName = "InventoryReportPdf.pdf",
                    FileToken = "InventoryReportPdf.html",
                    FileType = MimeTypeNames.TextHtml
                };
                try
                {
                    templateHtml = await _fileStorageManager.GetTemplate(fileDto.FileToken);//retrive template from path
                    templateHtml = templateHtml.Trim();
                }
                catch (FileNotFoundException)
                {
                    throw new UserFriendlyException("FileNotFound");
                }
                //ToDo: Replace and concat string to be the same what frontend did
                exportHtml = templateHtml;

                var contentBody = string.Empty;
                var contentHeader = string.Empty;

                var viewHeader = reportCollumnHeader.OrderBy(x => x.SortOrder);
                var documentWidth = 1080;
                decimal totalVisibleColsWidth = 0;
                decimal totalTableWidth = 0;
                var countSubHeader = 0;
                int reportCountColHead = viewHeader.Where(t => t.Visible == true).Count();
                var multiCurrencyHeader = input.CurrencyFilter == CurrencyFilter.MultiCurrencies ? "<tr>" : "";
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (documentWidth >= (totalVisibleColsWidth + i.ColumnLength))
                        {
                            var rowHeader = "";
                            if (input.CurrencyFilter == CurrencyFilter.MultiCurrencies && entity.TotalResults.ContainsKey(i.ColumnName))
                            {
                                var index = 0;
                                var subColWidth = Convert.ToInt32(i.ColumnLength / entity.TotalResults[i.ColumnName].Count());
                                rowHeader = $"<th colspan='{entity.TotalResults[i.ColumnName].Count()}' style='width:{i.ColumnLength}px;'>{i.ColumnTitle}</th>";
                                foreach (var subHeader in entity.TotalResults[i.ColumnName])
                                {
                                    if (index > 0) reportCountColHead++;
                                    multiCurrencyHeader += $"<th style='width: {subColWidth}px;'>{subHeader.Key}</th>";
                                    index++;
                                }
                            }
                            else
                            {
                                rowHeader = $"<th rowspan='2' width='{i.ColumnLength}'px>{i.ColumnTitle}</th>";
                            }
                            contentHeader += rowHeader;
                            totalTableWidth += i.ColumnLength;
                        }
                        else
                        {
                            i.Visible = false;
                        }
                        totalVisibleColsWidth += i.ColumnLength;
                    }
                }
                multiCurrencyHeader += input.CurrencyFilter == CurrencyFilter.MultiCurrencies ? $"</tr>" : "";
                //int reportCountColHead = viewHeader.Where(t => t.Visible == true).Count() + countSubHeader;

                exportHtml = exportHtml.Replace("{{tableWidth}}", totalTableWidth.ToString());

                #region Row Body              

                var groupBy = new List<GetListVendorByBillReportGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "Vendor":
                            groupBy = entity.Items
                                .GroupBy(t => t.VendorCode)
                                .Select(t => new GetListVendorByBillReportGroupByOutput
                                {
                                    KeyName = t.Select(a => a.VendorName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }
                // write body
                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0) //write body have group by
                {
                    var item = viewHeader;
                    foreach (var k in groupBy)
                    {

                        var headerGroup = "<tr valign='top' style='font-weight: bold; page-break-inside:avoid;'><td colspan=" + reportCountColHead + "> " + k.KeyName + " </td></tr>";
                        contentBody += headerGroup;
                        foreach (var row in k.Items)
                        {
                            var tr = "<tr style='page-break-inside:avoid;'  valign='top'>";
                            foreach (var i in viewHeader)
                            {
                                if (i.Visible)
                                {
                                    if (i.ColumnName == "Date")
                                    {
                                        tr += $"<td>{row.Date.ToString(formatDate)}</td>";
                                    }
                                    else if (i.ColumnName == "JournalNo")
                                    {
                                        tr += $"<td>{row.JournalNo}</td>";
                                    }
                                    else if (i.ColumnName == "Reference")
                                    {
                                        tr += $"<td>{row.Reference}</td>";
                                    }
                                    else if (i.ColumnName == "JournalType")
                                    {
                                        tr += $"<td>{row.JournalType.ToString()}</td>";
                                    }
                                    else if (i.ColumnName == "AccountCode")
                                    {
                                        tr += $"<td>{row.AccountCode}</td>";
                                    }
                                    else if (i.ColumnName == "AccountName")
                                    {
                                        tr += $"<td>{row.AccountName}</td>";
                                    }
                                    else if (i.ColumnName == "VendorName")
                                    {
                                        tr += $"<td>{row.VendorName}</td>";
                                    }
                                    else if (i.ColumnName == "Description")
                                    {
                                        tr += $"<td>{row.Description}</td>";
                                    }
                                    else if (i.ColumnName == "TotalAmount")
                                    {
                                        tr += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(row.TotalAmount, digit, MidpointRounding.ToEven), digit)}</td>";
                                    }
                                    else if (i.ColumnName == "TotalPaid")
                                    {
                                        tr += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(row.TotalPaid, digit, MidpointRounding.ToEven), digit)}</td>";
                                    }
                                    else if (i.ColumnName == "Balance")
                                    {
                                        tr += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(row.Balance, digit, MidpointRounding.ToEven), digit)}</td>";
                                    }
                                    else if (i.ColumnName == "LastPaymentDate")
                                    {
                                        var dv = row.LastPaymentDate != null ? row.LastPaymentDate.Value.ToString(formatDate) : null;
                                        tr += $"<td>{dv}</td>";
                                    }
                                    else if (i.ColumnName == "Aging")
                                    {
                                        tr += $"<td style='text-align: right;'>{row.Aging}</td>";
                                    }
                                    else if (i.ColumnName == "LastPaymentAmount")
                                    {
                                        tr += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(row.LastPaymentAmounts, digit, MidpointRounding.ToEven), digit)}</td>";
                                    }
                                    else if (i.ColumnName == "User")
                                    {
                                        tr += $"<td>{row.User.UserName}</td>";
                                    }
                                    else if (i.ColumnName == "Location")
                                    {
                                        tr += $"<td>{row.Location}</td>";
                                    }
                                    else if (i.ColumnName == "Currency")
                                    {
                                        tr += $"<td>{row.Currency.Code}</td>";
                                    }
                                    else if ((i.ColumnName == "LastPaymentAmounts" || i.ColumnName == "Balances"
                                            || i.ColumnName == "TotalPaids" || i.ColumnName == "TotalAmounts")
                                            && input.CurrencyFilter == CurrencyFilter.MultiCurrencies)
                                    {
                                        if (entity.TotalResults.ContainsKey(i.ColumnName))
                                        {
                                            var subColWidth = Convert.ToInt32(i.ColumnLength / entity.TotalResults[i.ColumnName].Count());

                                            foreach (var code in entity.TotalResults[i.ColumnName])
                                            {
                                                tr += $"<td style='text-align: right; width: {subColWidth}px;'>";

                                                var sub = row.TotalColsByCurency.Where(t => t.CurrencyCode == code.Key).FirstOrDefault();
                                                if (i.ColumnName == "LastPaymentAmounts")
                                                {
                                                    tr += $"{FormatNumberCurrency(Math.Round(sub.LastPaymentAmounts, digit, MidpointRounding.ToEven), digit)}";
                                                }
                                                else if (i.ColumnName == "Balances")
                                                {
                                                    tr += $"{FormatNumberCurrency(Math.Round(sub.Balances, digit, MidpointRounding.ToEven), digit)}";
                                                }
                                                else if (i.ColumnName == "TotalPaids")
                                                {
                                                    tr += $"{FormatNumberCurrency(Math.Round(sub.TotalPaids, digit, MidpointRounding.ToEven), digit)}";
                                                }
                                                else if (i.ColumnName == "TotalAmounts")
                                                {
                                                    tr += $"{FormatNumberCurrency(Math.Round(sub.TotalAmounts, digit, MidpointRounding.ToEven), digit)}";
                                                }
                                                tr += "</td>";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        tr += $"<td></td>";
                                    }

                                }
                            }
                            tr += "</tr>";
                            contentBody += tr;
                        }
                        // sum footer 
                        var trGr = "<tr style='font-weight: bold; page-break-inside:avoid;'>";
                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                if (!string.IsNullOrEmpty(i.AllowFunction))/*listSum.Contains(i.ColumnName))*/
                                {
                                    if (i.ColumnName == "TotalAmount")
                                    {
                                        trGr += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(k.Items.Sum(t => t.TotalAmount), digit, MidpointRounding.ToEven), digit)}</td>";
                                    }
                                    else if (i.ColumnName == "TotalPaid")
                                    {
                                        trGr += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(k.Items.Sum(t => t.TotalPaid), digit, MidpointRounding.ToEven), digit)}</td>";
                                    }
                                    else if (i.ColumnName == "Balance")
                                    {
                                        trGr += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(k.Items.Sum(t => t.Balance), digit, MidpointRounding.ToEven), digit)}</td>";
                                    }
                                    //else if (i.ColumnName == "LastPaymentAmount")
                                    //{
                                    //    trGr += $"<td>{FormatNumberCurrency(Math.Round(k.Items.Sum(t => t.LastPaymentAmounts), digit, MidpointRounding.ToEven),digit)}</td>";
                                    //}
                                    else if ((i.ColumnName == "LastPaymentAmounts" || i.ColumnName == "Balances"
                                            || i.ColumnName == "TotalPaids" || i.ColumnName == "TotalAmounts")
                                            && input.CurrencyFilter == CurrencyFilter.MultiCurrencies)
                                    {
                                        if (input.CurrencyFilter == CurrencyFilter.MultiCurrencies)//Multi currency 
                                        {
                                            //trGr += $"<td>";
                                            if (entity.TotalResults.ContainsKey(i.ColumnName))
                                            {

                                                var sumList = k.Items.SelectMany(u => u.TotalColsByCurency).Select(u => u).ToList();
                                                var groupByCurrencyDict = sumList.GroupBy(u => u.CurrencyCode).Select(u => new
                                                {
                                                    CurrencyCode = u.Key,
                                                    TotalPaid = u.Sum(s => s.TotalPaids),
                                                    TotalAmount = u.Sum(s => s.TotalAmounts),
                                                    Balance = u.Sum(s => s.Balances),
                                                    LastPaymentAmounts = u.Sum(s => s.LastPaymentAmounts)

                                                }).ToDictionary(u => u.CurrencyCode);

                                                foreach (var code in entity.TotalResults[i.ColumnName])
                                                {
                                                    trGr += $"<td style='text-align: right;'>";

                                                    //var sub = k.Items.Where(t => t.CurrencyCode == code.Key).FirstOrDefault();
                                                    if (i.ColumnName == "LastPaymentAmounts" && groupByCurrencyDict.ContainsKey(code.Key))
                                                    {
                                                        trGr += $"{FormatNumberCurrency(Math.Round(groupByCurrencyDict[code.Key].LastPaymentAmounts, digit, MidpointRounding.ToEven), digit)}";
                                                    }
                                                    else if (i.ColumnName == "Balances" && groupByCurrencyDict.ContainsKey(code.Key))
                                                    {
                                                        trGr += $"{FormatNumberCurrency(Math.Round(groupByCurrencyDict[code.Key].Balance, digit, MidpointRounding.ToEven), digit)}";
                                                    }
                                                    else if (i.ColumnName == "TotalPaids" && groupByCurrencyDict.ContainsKey(code.Key))
                                                    {
                                                        trGr += $"{FormatNumberCurrency(Math.Round(groupByCurrencyDict[code.Key].TotalPaid, digit, MidpointRounding.ToEven), digit)}";
                                                    }
                                                    else if (i.ColumnName == "TotalAmounts" && groupByCurrencyDict.ContainsKey(code.Key))
                                                    {
                                                        trGr += $"{FormatNumberCurrency(Math.Round(groupByCurrencyDict[code.Key].TotalAmount, digit, MidpointRounding.ToEven), digit)}";
                                                    }
                                                    trGr += "</td>";
                                                }
                                            }
                                            //trGr += "</td>";
                                        }

                                    }
                                    else
                                    {
                                        trGr += $"<td></td>";
                                    }
                                }
                                else //none sum
                                {
                                    trGr += $"<td></td>";
                                }

                            }
                        }
                        contentBody += trGr;
                    }
                }
                else // write body no group by
                {
                    foreach (var row in entity.Items)
                    {
                        var tr = "<tr style='page-break-inside:avoid;'  valign='top'>";
                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                if (i.ColumnName == "Date")
                                {
                                    tr += $"<td>{row.Date.ToString(formatDate)}</td>";
                                }
                                else if (i.ColumnName == "JournalNo")
                                {
                                    tr += $"<td>{row.JournalNo}</td>";
                                }
                                else if (i.ColumnName == "Reference")
                                {
                                    tr += $"<td>{row.Reference}</td>";
                                }
                                else if (i.ColumnName == "JournalType")
                                {
                                    tr += $"<td>{row.JournalType.ToString()}</td>";
                                }
                                else if (i.ColumnName == "AccountCode")
                                {
                                    tr += $"<td>{row.AccountCode}</td>";
                                }
                                else if (i.ColumnName == "AccountName")
                                {
                                    tr += $"<td>{row.AccountName}</td>";
                                }
                                else if (i.ColumnName == "VendorName")
                                {
                                    tr += $"<td>{row.VendorName}</td>";
                                }
                                else if (i.ColumnName == "Description")
                                {
                                    tr += $"<td>{row.Description}</td>";
                                }
                                else if (i.ColumnName == "TotalAmount")
                                {
                                    tr += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(row.TotalAmount, digit, MidpointRounding.ToEven), digit)}</td>";
                                }
                                else if (i.ColumnName == "TotalPaid")
                                {
                                    tr += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(row.TotalPaid, digit, MidpointRounding.ToEven), digit)}</td>";
                                }
                                else if (i.ColumnName == "Balance")
                                {
                                    tr += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(row.Balance, digit, MidpointRounding.ToEven), digit)}</td>";
                                }
                                else if (i.ColumnName == "LastPaymentDate")
                                {
                                    var dv = row.LastPaymentDate != null ? row.LastPaymentDate.Value.ToString(formatDate) : null;
                                    tr += $"<td>{dv}</td>";
                                }
                                else if (i.ColumnName == "Aging")
                                {
                                    tr += $"<td style='text-align: right;'>{row.Aging}</td>";
                                }
                                else if (i.ColumnName == "LastPaymentAmounts" && input.CurrencyFilter == CurrencyFilter.AccountingCurrency)
                                {
                                    tr += $"<td style='text-align: right;'>{FormatNumberCurrency(Math.Round(row.LastPaymentAmounts, digit, MidpointRounding.ToEven), digit)}</td>";
                                }
                                else if (i.ColumnName == "User")
                                {
                                    tr += $"<td>{row.User.UserName}</td>";
                                }
                                else if (i.ColumnName == "Location")
                                {
                                    tr += $"<td>{row.Location}</td>";
                                }
                                else if (i.ColumnName == "Currency")
                                {
                                    tr += $"<td>{row.Currency.Code}</td>";
                                }
                                else if ((i.ColumnName == "LastPaymentAmounts" || i.ColumnName == "Balances"
                                        || i.ColumnName == "TotalPaids" || i.ColumnName == "TotalAmounts")
                                        && input.CurrencyFilter == CurrencyFilter.MultiCurrencies)
                                {
                                    if (input.CurrencyFilter == CurrencyFilter.MultiCurrencies)//Multi currency 
                                    {
                                        //tr += $"<td>";
                                        if (entity.TotalResults.ContainsKey(i.ColumnName))
                                        {
                                            foreach (var code in entity.TotalResults[i.ColumnName])
                                            {
                                                tr += $"<td style='text-align: right;'>";

                                                var sub = row.TotalColsByCurency.Where(t => t.CurrencyCode == code.Key).FirstOrDefault();
                                                if (i.ColumnName == "LastPaymentAmounts")
                                                {
                                                    tr += $"{FormatNumberCurrency(Math.Round(sub.LastPaymentAmounts, digit, MidpointRounding.ToEven), digit)}";
                                                }
                                                else if (i.ColumnName == "Balances")
                                                {
                                                    tr += $"{FormatNumberCurrency(Math.Round(sub.Balances, digit, MidpointRounding.ToEven), digit)}";
                                                }
                                                else if (i.ColumnName == "TotalPaids")
                                                {
                                                    tr += $"{FormatNumberCurrency(Math.Round(sub.TotalPaids, digit, MidpointRounding.ToEven), digit)}";
                                                }
                                                else if (i.ColumnName == "TotalAmounts")
                                                {
                                                    tr += $"{FormatNumberCurrency(Math.Round(sub.TotalAmounts, digit, MidpointRounding.ToEven), digit)}";
                                                }
                                                tr += "</td>";
                                            }
                                        }
                                        //tr += "</td>";
                                    }

                                }
                                else
                                {
                                    tr += $"<td></td>";
                                }
                            }
                        }
                        tr += "</tr>";
                        contentBody += tr;
                    }
                }
                #endregion Row Body


                #region Row Footer 

                if (reportHasShowFooterTotal.Count > 0)
                {
                    var index = 0;
                    var tr = "<tr valign='top' style='page-break-inside:avoid;'>";
                    foreach (var i in viewHeader)
                    {
                        if (i.Visible)
                        {
                            if (!string.IsNullOrEmpty(i.AllowFunction) && entity.TotalResults.ContainsKey(i.ColumnName))
                            {
                                if (input.CurrencyFilter == CurrencyFilter.MultiCurrencies)
                                {
                                    //tr += $"<td>";
                                    foreach (var code in entity.TotalResults[i.ColumnName])
                                    {
                                        tr += $"<td style='text-align: right;'>";
                                        var sub = entity.TotalResults[i.ColumnName][code.Key];
                                        tr += $"{FormatNumberCurrency(Math.Round(sub, digit, MidpointRounding.ToEven), digit)}";
                                        tr += "</td>";
                                    }
                                    //tr += "</td>";
                                }
                                else
                                {
                                    tr += $"<td style='word-wrap:break-word;text-align: right;'>";
                                    foreach (var code in entity.TotalResults[i.ColumnName])
                                    {
                                        var sub = entity.TotalResults[i.ColumnName][code.Key];
                                        tr += $"{FormatNumberCurrency(Math.Round(sub, digit, MidpointRounding.ToEven), digit)}";

                                    }
                                    tr += "</td>";

                                }

                            }
                            else //none sum
                            {
                                if (index == 0)
                                {
                                    tr += $"<td style='font-weight: bold;'>{L("Total")}</td>";
                                }
                                else
                                {
                                    tr += $"<td></td>";
                                }
                            }
                            index++;
                        }
                    }
                    tr += "</tr>";
                    contentBody += tr;
                }
                #endregion Row Footer
                exportHtml = exportHtml.Replace("{{rowHeader}}", contentHeader);
                exportHtml = exportHtml.Replace("{{subHeader}}", multiCurrencyHeader);
                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();
                AddHeaderPDF(htmlToPdfConverter, tenant.Name, sheetName, input.FromDate, input.ToDate, formatDate);
                AddFooterPDF(htmlToPdfConverter, user.FullName, formatDate);

                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
                var tokenName = $"{Guid.NewGuid()}.pdf";
                var result = new FileDto();
                result.FileName = $"{ReplaceSpecialCharacter(sheetName)}.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);
                return result;

            });

        }

        #endregion

        [UnitOfWork(IsDisabled = true)]
        public async Task CleanupSaveTemplate()
        {

            var _unitOfWorkManager = IocManager.Instance.Resolve<IUnitOfWorkManager>();
            var _reportTemplateRepository = IocManager.Instance.Resolve<ICorarlRepository<ReportTemplate, long>>();
            var _reportColumnTemplateRepository = IocManager.Instance.Resolve<ICorarlRepository<ReportColumnTemplate, Guid>>();
            var _tenantRepository = IocManager.Instance.Resolve<ICorarlRepository<Tenant, int>>();

            var reportColumns = new List<ReportColumnTemplate>();
            var tenantList = new List<Tenant>();

            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    reportColumns = await _reportColumnTemplateRepository.GetAll().Include(s => s.ReportTemplate)
                                    .Where(s =>
                                     s.ReportTemplate.ReportType == ReportType.ReportType_Purchasing ||
                                     s.ReportTemplate.ReportType == ReportType.ReportType_VendorAging ||
                                     s.ReportTemplate.ReportType == ReportType.ReportType_VendorByBill
                                     )
                                    .ToListAsync();

                    tenantList = await _tenantRepository.GetAll().ToListAsync();
                }
            }

            var columnToUpdates = new List<ReportColumnTemplate>();

            foreach (var tenant in tenantList)
            {
                var purchaseReportFromTemplateColumns = new List<CollumnOutput>();
                var vendorAgingReportFromTemplateColumns = new List<CollumnOutput>();
                var vendorByBillReportFromTemplateColumns = new List<CollumnOutput>();

                using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenant.Id))
                    {
                        purchaseReportFromTemplateColumns = GetReportTemplateBill().ColumnInfo.Where(s => s.IsDisplay).ToList();
                        vendorAgingReportFromTemplateColumns = GetReportTemplateVendorAging().ColumnInfo.Where(s => s.IsDisplay).ToList();
                        vendorByBillReportFromTemplateColumns = GetReportTemplateVendorByBill().ColumnInfo.Where(s => s.IsDisplay).ToList();
                    }
                }

                var purchaseReportColumns = reportColumns.Where(s => s.TenantId == tenant.Id && s.ReportTemplate.ReportType == ReportType.ReportType_Purchasing).ToList();
                var vendorAgingReportColumns = reportColumns.Where(s => s.TenantId == tenant.Id && s.ReportTemplate.ReportType == ReportType.ReportType_VendorAging).ToList();
                var vendorByBillReportColumns = reportColumns.Where(s => s.TenantId == tenant.Id && s.ReportTemplate.ReportType == ReportType.ReportType_VendorByBill).ToList();

                foreach (var col in purchaseReportFromTemplateColumns)
                {
                    var updateCols = purchaseReportColumns.Where(s => s.ColumnName == col.ColumnName).ToList();
                    foreach (var updateCol in updateCols)
                    {
                        updateCol.SetIsDisplay(true);
                        columnToUpdates.Add(updateCol);
                    }
                };

                foreach (var col in vendorAgingReportFromTemplateColumns)
                {
                    var updateCols = vendorAgingReportColumns.Where(s => s.ColumnName == col.ColumnName).ToList();
                    foreach (var updateCol in updateCols)
                    {
                        updateCol.SetIsDisplay(true);
                        columnToUpdates.Add(updateCol);
                    }
                };


                foreach (var col in vendorByBillReportFromTemplateColumns)
                {
                    var updateCols = vendorByBillReportColumns.Where(s => s.ColumnName == col.ColumnName).ToList();
                    foreach (var updateCol in updateCols)
                    {
                        updateCol.SetIsDisplay(true);
                        columnToUpdates.Add(updateCol);
                    }
                };
            }


            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    await _reportColumnTemplateRepository.BulkUpdateAsync(columnToUpdates);
                }

                await uow.CompleteAsync();
            }

        }


        #region "Purchase By Item Property Report"

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Vendor_PurchaseByItemProperty)]
        [HttpPost]
        public async Task<PagedResultWithTotalColuumnsDto<GetListPurchaseByItemPropertyReportOutput>> GetPurchaseByItemPropertyReport(GetListReportPurchaseByItemPropertyInput input)
        {
            // Validate input.GroupBy
            if (string.IsNullOrWhiteSpace(input.GroupBy))
                throw new UserFriendlyException(L("PleaseSelect", L("SummaryBy")));

            var find = await _propertyRepository.GetAll().AsNoTracking()
                .AnyAsync(s => s.Name == input.GroupBy);
            if (!find)
                throw new UserFriendlyException(L("PleaseSelect", L("SummaryBy")));

            // Pre-fetch period cycles and rounding digit
            var periodCycles = await GetCloseCyleAsync(input.ToDate);
            var previousCycle = periodCycles
                .Where(u => u.EndDate != null && u.EndDate.Value.Date <= input.ToDate.Date)
                .OrderByDescending(u => u.EndDate.Value)
                .FirstOrDefault();
            var currentCycle = periodCycles
                .Where(u => u.StartDate.Date <= input.ToDate.Date &&
                            (u.EndDate == null || input.ToDate.Date <= u.EndDate.Value.Date))
                .OrderByDescending(u => u.StartDate)
                .FirstOrDefault();
            var currentRoundingDigit = currentCycle?.RoundingDigit ?? previousCycle?.RoundingDigit ?? 2;

            // Pre-fetch filter data
            var defaultLocationGroups = await GetUserGroupByLocation();
            var userGroupVendors = await GetUserGroupVendors();
            var vendorTypeMemberIds = await GetVendorTypeMembers()
                .Select(s => s.VendorTypeId)
                .ToListAsync();

            // Base query with joins
            var journalQuery = from i in _billRepository.GetAll().AsNoTracking()
                                   .WhereIf(!vendorTypeMemberIds.IsNullOrEmpty(), s => s.Vendor.VendorTypeId.HasValue && vendorTypeMemberIds.Contains(s.Vendor.VendorTypeId.Value))
                                   .WhereIf(!userGroupVendors.IsNullOrEmpty(), s => userGroupVendors.Contains(s.VendorId) || s.Vendor.Member == Member.All)
                                   .WhereIf(!input.Vendors.IsNullOrEmpty(), s => input.Vendors.Contains(s.VendorId))
                                   .WhereIf(!input.VendorTypes.IsNullOrEmpty(), s => input.VendorTypes.Contains(s.Vendor.VendorTypeId.Value))
                               join j in _journalRepository.GetAll().AsNoTracking()
                                   .Where(s => s.Status == TransactionStatus.Publish && s.JournalType == JournalType.Bill && s.BillId.HasValue)
                                   .WhereIf(!defaultLocationGroups.IsNullOrEmpty(), u => defaultLocationGroups.Contains(u.LocationId.Value))
                                   .WhereIf(!input.Locations.IsNullOrEmpty(), u => input.Locations.Contains(u.LocationId.Value))
                                   .WhereIf(input.FromDate != null && input.ToDate != null, u => input.FromDate.Date <= u.Date.Date && u.Date.Date <= input.ToDate.Date)
                                   .WhereIf(!input.Filter.IsNullOrEmpty(), u =>
                                       u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                       u.Reference.ToLower().Contains(input.Filter.ToLower()))
                                   .WhereIf(!input.Users.IsNullOrEmpty(), u => input.Users.Contains(u.CreatorUserId))
                               on i.Id equals j.BillId
                               join bi in _billItemRepository.GetAll().AsNoTracking()
                                   .WhereIf(!input.Items.IsNullOrEmpty(), s => s.ItemId.HasValue && input.Items.Contains(s.ItemId))
                               on i.Id equals bi.BillId
                               select new
                               {
                                   JournalNo = j.JournalNo,
                                   Reference = j.Reference,
                                   Date = j.Date,
                                   Description = j.Memo,
                                   LocationId = j.LocationId.Value,
                                   LocationName = j.Location!=null ? j.Location.LocationName : "",
                                   User = j.CreatorUser!= null ? j.CreatorUser.UserName : "",
                                   BillId = i.Id,
                                   VendorId = i.VendorId,
                                   VendorName = i.Vendor.VendorName,
                                   VendorType = i.Vendor.VendorType != null ? i.Vendor.VendorType.VendorTypeName : "",
                                   VendorTypeId = i.Vendor.VendorTypeId,
                                   ItemId = bi.ItemId,
                                   Qty = bi.Qty,
                                   Total = bi.Total
                               };

            // Join with properties for SummaryBy and NetWeight
            var billItemWithProperty = from ii in journalQuery
                                       join p in _itemPropertyRepository.GetAll().AsNoTracking()
                                           .Where(p => p.Property.Name == input.GroupBy)
                                       on ii.ItemId equals p.ItemId into properties
                                       from p in properties.DefaultIfEmpty()
                                       join pu in _itemPropertyRepository.GetAll().AsNoTracking()
                                           .Where(p => p.Property.IsUnit)
                                       on ii.ItemId equals pu.ItemId into unitProperties
                                       from pu in unitProperties.DefaultIfEmpty()
                                       select new GetListPurchaseByItemPropertyReportOutput
                                       {
                                           JournalNo = ii.JournalNo,
                                           Reference = ii.Reference,
                                           Date = ii.Date,
                                           Description = ii.Description,
                                           LocationId = ii.LocationId,
                                           LocationName = ii.LocationName,
                                           User = ii.User,
                                           BillId = ii.BillId,
                                           VendorId = ii.VendorId,
                                           VendorName = ii.VendorName,
                                           VendorType = ii.VendorType,
                                           VendorTypeId = ii.VendorTypeId,
                                           Qty = ii.Qty,
                                           NetWeight = pu != null ? pu.PropertyValue.NetWeight : 0,
                                           Total = ii.Total,
                                           SummaryBy = p != null ? p.PropertyValue.Value : "",
                                           ItemId = ii.ItemId
                                       };

            // Apply PropertyDics filter
            var filteredQuery = billItemWithProperty;
            if (input.PropertyDics != null && input.PropertyDics.Any())
            {
                filteredQuery = from q in filteredQuery
                                join p in _itemPropertyRepository.GetAll().AsNoTracking()
                                on q.ItemId equals p.ItemId
                                where input.PropertyDics.Any(v => v.PropertyId == p.PropertyId &&
                                      (v.PropertyValueIds == null || v.PropertyValueIds.Count == 0 || v.PropertyValueIds.Contains(p.PropertyValueId)))
                                group q by new
                                {
                                    q.JournalNo,
                                    q.Reference,
                                    q.Date,
                                    q.Description,
                                    q.LocationId,
                                    q.LocationName,
                                    q.User,
                                    q.BillId,
                                    q.VendorId,
                                    q.VendorName,
                                    q.VendorType,
                                    q.VendorTypeId,
                                    q.Qty,
                                    q.NetWeight,
                                    q.Total,
                                    q.SummaryBy,
                                    q.ItemId
                                } into g
                                where g.Count() == input.PropertyDics.Count
                                select new GetListPurchaseByItemPropertyReportOutput
                                {
                                    JournalNo = g.Key.JournalNo,
                                    Reference = g.Key.Reference,
                                    Date = g.Key.Date,
                                    Description = g.Key.Description,
                                    LocationId = g.Key.LocationId,
                                    LocationName = g.Key.LocationName,
                                    User = g.Key.User,
                                    BillId = g.Key.BillId,
                                    VendorId = g.Key.VendorId,
                                    VendorName = g.Key.VendorName,
                                    VendorType = g.Key.VendorType,
                                    VendorTypeId = g.Key.VendorTypeId,
                                    Qty = g.Key.Qty,
                                    NetWeight = g.Key.NetWeight,
                                    Total = g.Key.Total,
                                    SummaryBy = g.Key.SummaryBy,
                                    ItemId = g.Key.ItemId
                                };
            }

            // Group and summarize
            var summaryQuery = filteredQuery
                .GroupBy(s => new 
                {
                    Date = s.Date,
                    JournalNo = s.JournalNo,
                    Reference = s.Reference,
                    Description = s.Description,
                    LocationId = s.LocationId,
                    LocationName = s.LocationName,
                    VendorName = s.VendorName,
                    VendorId = s.VendorId,
                    VendorType = s.VendorType,
                    VendorTypeId = s.VendorTypeId,
                    User = s.User,
                    SummaryBy = s.SummaryBy
                })
                .Select(s => new GetListPurchaseByItemPropertyReportOutput
                {
                    Date = s.Key.Date,
                    JournalNo = s.Key.JournalNo,
                    Reference = s.Key.Reference,
                    VendorName = s.Key.VendorName,
                    VendorId = s.Key.VendorId,
                    VendorType = s.Key.VendorType,
                    VendorTypeId = s.Key.VendorTypeId,
                    User = s.Key.User,
                    LocationId = s.Key.LocationId,
                    LocationName = s.Key.LocationName,
                    Description = s.Key.Description,
                    Qty = s.Sum(t => t.Qty),
                    NetWeight = s.Sum(t => t.Qty * t.NetWeight),
                    Total = s.Sum(t => t.Total),
                    RoundingDigit = currentRoundingDigit,
                    SummaryBy = s.Key.SummaryBy
                })
                .OrderBy(s => s.SummaryBy);

            // Get total count
            var resultCount = await summaryQuery.CountAsync();
            if (resultCount == 0)
                return new PagedResultWithTotalColuumnsDto<GetListPurchaseByItemPropertyReportOutput>(
                    resultCount, new List<GetListPurchaseByItemPropertyReportOutput>(), new Dictionary<string, decimal>());

            // Compute summaries
            PurchaseByItemPropertyColumnSummaryOutPut summary = null;
            if (input.IsLoadMore == false && input.ColumnNamesToSum != null)
            {
                summary = await summaryQuery
                    .GroupBy(s => 1)
                    .Select(s => new PurchaseByItemPropertyColumnSummaryOutPut
                    {
                        TotalAmount = s.Sum(t => t.Total),
                        TotalQty = s.Sum(t => t.Qty),
                        TotalNetWeight = s.Sum(t => t.NetWeight)
                    })
                    .FirstOrDefaultAsync();
            }

            // Build sum of columns
            var sumOfColumns = new Dictionary<string, decimal>();
            if (input.ColumnNamesToSum != null && input.IsLoadMore == false)
            {
                foreach (var col in input.ColumnNamesToSum)
                {
                    if (col == "Total")
                        sumOfColumns.Add(col, summary?.TotalAmount ?? 0);
                    else if (col == "Qty")
                        sumOfColumns.Add(col, summary?.TotalQty ?? 0);
                    else if (col == "NetWeight")
                        sumOfColumns.Add(col, summary?.TotalNetWeight ?? 0);
                }
            }

            // Fetch paginated or full results
            var entities = new List<GetListPurchaseByItemPropertyReportOutput>();
            if (input.UsePagination)
            {
                entities = await summaryQuery.PageBy(input).ToListAsync();
            }
            else
            {
                // Stream results in chunks to avoid memory issues
                int pageSize = 1000;
                int page = 0;
                while (true)
                {
                    var batch = await summaryQuery.Skip(page * pageSize).Take(pageSize).ToListAsync();
                    if (!batch.Any()) break;
                    entities.AddRange(batch);
                    page++;
                }
            }

            return new PagedResultWithTotalColuumnsDto<GetListPurchaseByItemPropertyReportOutput>(resultCount, entities, sumOfColumns);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Vendor_PurchaseByItemProperty)]
        public async Task<ReportOutput> GetReportTemplatePurchaseByItemProperty()
        {
            var itemPropertyCols = await _propertyRepository.GetAll().AsNoTracking()
                            .Where(t => t.IsActive == true)
                            .Select(t => new CollumnOutput
                            {
                                AllowGroupby = true,
                                AllowFilter = true,
                                ColumnName = t.Name,
                                ColumnLength = 150,
                                ColumnTitle = t.Name,
                                ColumnType = ColumnType.ItemProperty,
                                SortOrder = 11,
                                Visible = true,
                                AllowFunction = null,
                                MoreFunction = null,
                                IsDisplay = false,
                                AllowShowHideFilter = true,
                                ShowHideFilter = true,
                                DefaultValue = t.Id.ToString(),//to init the value of property
                            })
                            .ToListAsync();

            var columns = new List<CollumnOutput>() {
                     // start properties with can filter
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Search",
                        ColumnLength = 150,
                        ColumnTitle = "Search",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "DateRange",
                        ColumnLength = 150,
                        ColumnTitle = "DateRange",
                        ColumnType = ColumnType.Language,
                        SortOrder = 0,
                        Visible = true,
                        DefaultValue = "thisMonth",
                        AllowFunction = null,
                        DisableDefault = false,
                        MoreFunction = null,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Item",
                        ColumnLength = 300,
                        ColumnTitle = "Item",
                        ColumnType = ColumnType.String,
                        SortOrder = 0,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = false,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Date",
                        ColumnLength = 100,
                        ColumnTitle = "Date",
                        ColumnType = ColumnType.Date,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                     },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "JournalNo",
                        ColumnLength = 120,
                        ColumnTitle = "Journal No",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        DisableDefault = false,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Reference",
                        ColumnLength = 100,
                        ColumnTitle = "Reference",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        DisableDefault = false,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Vendor",
                        ColumnLength = 120,
                        ColumnTitle = "Vendor",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        DisableDefault = false,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Location",
                        ColumnLength = 120,
                        ColumnTitle = "Location",
                        ColumnType = ColumnType.String,
                        SortOrder = 7,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Description",
                        ColumnLength = 120,
                        ColumnTitle = "Description",
                        ColumnType = ColumnType.String,
                        SortOrder = 12,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "SummaryBy",
                        ColumnLength = 100,
                        ColumnTitle = "SummaryBy",
                        ColumnType = ColumnType.String,
                        SortOrder = 13,
                        Visible = true,
                        AllowFunction = null,
                        DisableDefault = false,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Qty",
                        ColumnLength = 100,
                        ColumnTitle = "Qty",
                        ColumnType = ColumnType.RoundingDigit,
                        SortOrder = 14,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                         AllowFunction = null,
                         MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                      },
                      new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Total",
                        ColumnLength = 150,
                        ColumnTitle = "Total",
                        ColumnType = ColumnType.RoundingDigit,
                        SortOrder = 15,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                      },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "NetWeight",
                        ColumnLength = 150,
                        ColumnTitle = "Net Weight",
                        ColumnType = ColumnType.RoundingDigit,
                        SortOrder = 19,
                        Visible = true,
                        AllowFunction = "Sum",
                        AllowShowHideFilter = false,
                        ShowHideFilter = false,
                        MoreFunction = new List<MoreFunction>(){
                            new MoreFunction
                            {
                                KeyName = null
                            },
                            new MoreFunction
                            {
                                KeyName = "Sum"
                            }
                        },
                        IsDisplay = true
                    },
                      new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "User",
                        ColumnLength = 100,
                        ColumnTitle = "User",
                        ColumnType = ColumnType.String,
                        SortOrder = 20,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                      },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "VendorType",
                        ColumnLength = 150,
                        ColumnTitle = "Vendor Type",
                        ColumnType = ColumnType.String,
                        SortOrder = 21,
                        Visible = true,
                        IsDisplay = true,
                        AllowShowHideFilter = true,
                        ShowHideFilter = true,
                    }
                };

            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = columns.Concat(itemPropertyCols).ToList(),
                Groupby = "",
                HeaderTitle = "Purchase By Item Property",
                Sortby = "",
            };


            //if (!FeatureChecker.IsEnabled(AppFeatures.AccountingFeature))
            //{
            //    result.ColumnInfo = result.ColumnInfo.Where(s =>
            //        s.ColumnName != "Account" &&
            //        s.ColumnName != "AccountType").ToList();
            //}


            //var isMultiCurrency = _multiCurrencyRepository.GetAll().Any();

            //if (isMultiCurrency)
            //{
            //    var currency = new CollumnOutput
            //    {
            //        AllowGroupby = false,
            //        AllowFilter = true,
            //        ColumnName = "Currency",
            //        ColumnLength = 140,
            //        ColumnTitle = "Currency",
            //        ColumnType = ColumnType.String,
            //        SortOrder = 3,
            //        Visible = true,
            //        IsDisplay = false,
            //        DisableDefault = false,
            //        AllowShowHideFilter = true,
            //        ShowHideFilter = true,
            //    };
            //    result.ColumnInfo.Add(currency);
            //}


            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Vendor_PurchaseByItemProperty_Export_Excel)]
        public async Task<FileDto> ExportExcelPurchaseByItemPropertyReport(GetPurchaseByItemPropertyReportInput input)
        {
            input.UsePagination = false;
            input.IsLoadMore = false;

            // Query get collumn header 
            //var @entity = await _reportTemplateManager.GetAsync(ReportType.ReportType_Outbounce);
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;

            var reportResult = await GetPurchaseByItemPropertyReport(input);
            var invoiceData = reportResult.Items;

            var tenant = await GetCurrentTenantAsync();
            var formatDate = await _formatRepository.GetAll()
                       .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
                       .Select(t => t.Web).FirstOrDefaultAsync();

            if (formatDate.IsNullOrEmpty())
            {
                formatDate = "dd-MM-yyyy";
            }

            var result = new FileDto();

            //Creates a blank workbook. Use the using statment, so the package is disposed when we are done.
            using (var p = new ExcelPackage())
            {
                //A workbook must have at least on cell, so lets add one... 
                var ws = p.Workbook.Worksheets.Add(sheetName);
                ws.PrinterSettings.Orientation = eOrientation.Landscape;
                ws.PrinterSettings.FitToPage = true;
                //ws.PrinterSettings.PaperSize = ePaperSize.A3; //set default format paper size 
                ws.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                ws.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet

                //var subCols = invoiceData == null ? null : invoiceData.First().CurrencyColumnTotals.Select(s => s.CurrencyCode).ToList();
                //var isMultiCurrencies = input.CurrencyFilter == CurrencyFilter.MultiCurrencies && subCols != null && subCols.Count() > 1;

                #region Row 1 Header
                AddTextToCell(ws, 1, 1, sheetName, true);
                MergeCell(ws, 1, 1, 1, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 1


                #region Row 2 Header
                var header = input.ToDate.ToString(formatDate);
                AddTextToCell(ws, 2, 1, header, true);
                MergeCell(ws, 2, 1, 2, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 2

                #region Row 3 Header Table
                int rowTableHeader = 3;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                foreach (var i in reportCollumnHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colHeaderTable += 1;
                }

                //if (isMultiCurrencies) rowTableHeader += 1;
                #endregion Row 3

                #region Row Body 
                // use for check auto dynamic col footer of sum value
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                foreach (var i in invoiceData)
                {
                    int collumnCellBody = 1;
                    foreach (var item in reportCollumnHeader)// map with correct key of properties 
                    {
                        WriteBodyPurchaseByItemProperty(ws, rowBody, collumnCellBody, item, i, count);
                        collumnCellBody += 1;
                    }
                    rowBody += 1;
                    count += 1;
                }

                #endregion Row Body


                #region Row Footer 
                if (reportHasShowFooterTotal.Count > 0)
                {
                    int footerRow = rowBody;
                    int footerColNumber = 1;
                    AddTextToCell(ws, footerRow, footerColNumber, "TOTAL", true);
                    foreach (var i in reportCollumnHeader)
                    {
                        if (i.AllowFunction != null)
                        {
                            int rowFooter = rowTableHeader + 1;// get start row after header 
                            var fromCell = GetAddressName(ws, rowFooter, footerColNumber);
                            var toCell = GetAddressName(ws, footerRow - 1, footerColNumber);
                            if (i.ColumnType == ColumnType.Number)
                            {
                                AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true, false, true);
                            }
                            else
                            {
                                AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true);
                            }
                        }
                        footerColNumber += 1;
                    }
                }
                #endregion Row Footer


                result.FileName = $"Purchase_By_Item_Property_Report_{header.Replace(" ", "_").Replace("-", "_")}.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Vendor_PurchaseByItemProperty_Export_Pdf)]
        public async Task<FileDto> ExportPDFPurchaseByItemPropertyReport(GetPurchaseByItemPropertyReportInput input)
        {
            input.UsePagination = false;
            input.IsLoadMore = false;

            var tenant = await GetCurrentTenantAsync();
            var formatDate = await _formatRepository.GetAll()
                        .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
                        .Select(t => t.Web).FirstOrDefaultAsync();

            if (formatDate.IsNullOrEmpty())
            {
                formatDate = "dd/MM/yyyy";
            }

            var rounding = await GetCurrentCycleAsync();
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            //int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;
            var user = await GetCurrentUserAsync();
            var invoices = await GetPurchaseByItemPropertyReport(input);

            return await Task.Run(async () =>
            {
                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                FileDto fileDto = new FileDto()
                {
                    SubFolder = null,
                    FileName = "InventoryReportPdf.pdf",
                    FileToken = "InventoryReportPdf.html",
                    FileType = MimeTypeNames.TextHtml
                };
                try
                {
                    templateHtml = await _fileStorageManager.GetTemplate(fileDto.FileToken);//retrive template from path
                    templateHtml = templateHtml.Trim();
                }
                catch (FileNotFoundException)
                {
                    throw new UserFriendlyException("FileNotFound");
                }
                //ToDo: Replace and concat string to be the same what frontend did
                exportHtml = templateHtml;

                var contentBody = string.Empty;
                var contentHeader = string.Empty;

                var viewHeader = reportCollumnHeader.OrderBy(x => x.SortOrder);
                var documentWidth = 1080;
                decimal totalVisibleColsWidth = 0;
                decimal totalTableWidth = 0;
                int reportCountColHead = viewHeader.Where(t => t.Visible == true).Count();
                //var multiCurrencyHeader = isMultiCurrencies ? "<tr>" : "";
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (documentWidth >= (totalVisibleColsWidth + i.ColumnLength))
                        {
                            var rowHeader = "";

                            rowHeader = $"<th width='{i.ColumnLength}px'>{i.ColumnTitle}</th>";

                            contentHeader += rowHeader;
                            totalTableWidth += i.ColumnLength;
                        }
                        else
                        {
                            i.Visible = false;
                            reportCountColHead--;
                        }
                        totalVisibleColsWidth += i.ColumnLength;
                    }
                }

                //multiCurrencyHeader += isMultiCurrencies ? $"</tr>" : "";
                exportHtml = exportHtml.Replace("{{tableWidth}}", totalTableWidth.ToString());

                #region Row Body     
                foreach (var row in invoices.Items)
                {
                    var tr = "<tr style='page-break-before: auto; page-break-after: auto;'>";
                    foreach (var i in viewHeader)
                    {
                        if (i.Visible)
                        {
                            if (i.ColumnName == "SummaryBy")
                            {
                                tr += $"<td>{row.SummaryBy}</td>";
                            }
                            else if (i.ColumnName == "JournalNo")
                            {
                                tr += $"<td>{row.JournalNo}</td>";
                            }
                            else if (i.ColumnName == "Date")
                            {
                                tr += $"<td>{row.Date.ToString(formatDate)}</td>";
                            }
                            else if (i.ColumnName == "Reference")
                            {
                                tr += $"<td>{row.Reference}</td>";
                            }
                            else if (i.ColumnName == "Description")
                            {
                                tr += $"<td>{row.Description}</td>";
                            }
                            else if (i.ColumnName == "Vendor")
                            {
                                tr += $"<td>{row.VendorName}</td>";
                            }
                            else if (i.ColumnName == "VendorType")
                            {
                                tr += $"<td>{row.VendorType}</td>";
                            }
                            else if (i.ColumnName == "User")
                            {
                                tr += $"<td>{row.User}</td>";
                            }
                            else if (i.ColumnName == "Location")
                            {
                                tr += $"<td>{row.LocationName}</td>";
                            }

                            else if (i.ColumnName == "NetWeight")
                            {
                                tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.NetWeight, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                            }
                            else if (i.ColumnName == "Qty")
                            {
                                tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Qty, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                            }
                            else if (i.ColumnName == "Total")
                            {
                                tr += $"<td style='text-align: right'>{FormatNumberCurrency(Math.Round(row.Total, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                            }
                            else
                            {
                                tr += $"<td></td>";
                            }
                        }
                    }
                    tr += "</tr>";
                    contentBody += tr;
                }
                #endregion Row Body

                #region Row Footer 

                if (reportHasShowFooterTotal.Count > 0)
                {
                    var index = 0;
                    var tr = "<tr style='page-break-before: auto; page-break-after: auto;'>";
                    foreach (var i in viewHeader)
                    {
                        if (i.Visible)
                        {
                            if (index == 0)
                            {
                                tr += $"<td style='font-weight: bold;text-align:right;'>{L("Total")}</td>";
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(i.AllowFunction))
                                {
                                    if (invoices.TotalResult != null && invoices.TotalResult.ContainsKey(i.ColumnName))
                                    {
                                        tr += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(invoices.TotalResult[i.ColumnName], rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                    }
                                }
                                else //none sum
                                {
                                    tr += $"<td></td>";
                                }
                            }
                            index++;
                        }
                    }
                    tr += "</tr>";
                    contentBody += tr;
                }
                #endregion Row Footer

                exportHtml = exportHtml.Replace("{{rowHeader}}", contentHeader);
                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                exportHtml = exportHtml.Replace("{{subHeader}}", "");
                EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();
                AddHeaderPDF(htmlToPdfConverter, tenant.Name, sheetName, input.FromDate, input.ToDate, formatDate);
                AddFooterPDF(htmlToPdfConverter, user.FullName, formatDate);
                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
                var tokenName = $"{Guid.NewGuid()}.pdf";
                var result = new FileDto();
                result.FileName = $"{sheetName}.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);
                return result;

            });
        }

        #endregion

        #region QCTest Report

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_QCTest)]
        [HttpPost]
        public async Task<PagedResultDto<GetListQCTestReportOutput>> GetQCTestReport(GetListReportQCTestInput input)
        {
            //// Pre-fetch period cycles and rounding digit
            //var periodCycles = await GetCloseCyleAsync(input.ToDate);
            //var previousCycle = periodCycles
            //    .Where(u => u.EndDate != null && u.EndDate.Value.Date <= input.ToDate.Date)
            //    .OrderByDescending(u => u.EndDate.Value)
            //    .FirstOrDefault();
            //var currentCycle = periodCycles
            //    .Where(u => u.StartDate.Date <= input.ToDate.Date &&
            //                (u.EndDate == null || input.ToDate.Date <= u.EndDate.Value.Date))
            //    .OrderByDescending(u => u.StartDate)
            //    .FirstOrDefault();
            //var currentRoundingDigit = currentCycle?.RoundingDigit ?? previousCycle?.RoundingDigit ?? 2;

            // Pre-fetch filter data
            var defaultLocationGroups = await GetUserGroupByLocation();
            var userGroupVendors = await GetUserGroupVendors();
            var vendorTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            // Base query with joins
            var testResultQuery = from t in _labTestResultRepository.GetAll().AsNoTracking()
                                       .WhereIf(input.FromDate.HasValue, s => input.FromDate.Value.Date <= s.ResultDate.Date)
                                       .WhereIf(input.ToDate.HasValue, s => s.ResultDate.Date <= input.ToDate.Value.Date)
                                       .WhereIf(!vendorTypeMemberIds.IsNullOrEmpty(), s => s.LabId.HasValue && vendorTypeMemberIds.Contains(s.Lab.VendorTypeId.Value))
                                       .WhereIf(!userGroupVendors.IsNullOrEmpty(), s => s.LabId.HasValue && (userGroupVendors.Contains(s.LabId.Value) || s.Lab.Member == Member.All))
                                       .WhereIf(!input.Labs.IsNullOrEmpty(), s => s.LabId.HasValue && input.Labs.Contains(s.LabId.Value))
                                       .WhereIf(!input.LabTypes.IsNullOrEmpty(), s => s.LabId.HasValue && input.LabTypes.Contains(s.Lab.VendorTypeId.Value))
                                       .WhereIf(!input.Users.IsNullOrEmpty(), s => input.Users.Contains(s.CreatorUserId))
                                       .WhereIf(input.PassFail.HasValue, s => s.FinalPassFail == input.PassFail)
                                       .WhereIf(!input.QCTestTemplateIds.IsNullOrEmpty(), s => input.QCTestTemplateIds.Contains(s.LabTestRequest.QCTestTemplateId))
                                       .WhereIf(!input.LabTestStatus.IsNullOrEmpty(), s => input.LabTestStatus.Contains(s.LabTestRequest.Status))
                                       .WhereIf(!input.Items.IsNullOrEmpty(), s => input.Items.Contains(s.LabTestRequest.QCSample.ItemId))
                                       .WhereIf(!input.Locations.IsNullOrEmpty(), s => input.Locations.Contains(s.LabTestRequest.QCSample.LocationId))
                                       .WhereIf(!input.Filter.IsNullOrEmpty(), s =>
                                           (s.ReferenceNo != null && s.ReferenceNo.ToLower().Contains(input.Filter.ToLower())) ||
                                           s.LabTestRequest.QCSample.SampleId.ToLower().Contains(input.Filter.ToLower()))
                                  select new GetListQCTestReportOutput
                                  {
                                      Id = t.Id,
                                      ResultDate = t.ResultDate,
                                      ReferenceNo = t.ReferenceNo,
                                      DetailEntry = t.DetailEntry,
                                      FinalPassFail = t.FinalPassFail,
                                      SendDate = t.LabTestRequest.Date,
                                      SampleId = t.LabTestRequest.QCSample.SampleId,
                                      ItemId = t.LabTestRequest.QCSample.ItemId,
                                      ItemName = t.LabTestRequest.QCSample.Item.ItemName,
                                      Location = t.LabTestRequest.QCSample.Location.LocationName,
                                      LabName = t.LabId.HasValue ? t.Lab.VendorName : t.Lab.VendorName,
                                      Remark = t.LabTestRequest.Remark
                                  };


            var testResultDetailQuery = _labTestResultDetailRepository.GetAll().AsNoTracking()
                                        .Select(s => new TestResultDetailOutput {
                                            TestParameterId = s.TestParameterId,
                                            TestParameterName = s.TestParameter.Name,
                                            LimitReferenceNote = s.TestParameter.LimitReferenceNote,
                                            ActualValue = s.ActualValueNote,
                                            PassFail = s.PassFail,
                                            LabTestResultId = s.LabTestResultId
                                        });

            var testQuery = from t in testResultQuery
                            join d in testResultDetailQuery 
                            on t.Id equals d.LabTestResultId into details
                            where input.TestParameterIds == null || !input.TestParameterIds.Any() || details.Any(d => input.TestParameterIds.Contains(d.TestParameterId))
                            select new GetListQCTestReportOutput
                            {
                                Id = t.Id,
                                ResultDate = t.ResultDate,
                                ReferenceNo = t.ReferenceNo,
                                DetailEntry = t.DetailEntry,
                                FinalPassFail = t.FinalPassFail,
                                SendDate = t.SendDate,
                                SampleId = t.SampleId,
                                ItemId = t.ItemId,
                                ItemName = t.ItemName,
                                Location = t.Location,
                                LabName = t.LabName,
                                Remark = t.Remark,
                                TestResultDetails = details.ToList()
                            };


            // Apply PropertyDics filter
            var filteredQuery = testQuery;
            if (input.PropertyDics != null && input.PropertyDics.Any())
            {
                // Subquery to count matching properties for each ItemId
                var propertyMatchQuery = from p in _itemPropertyRepository.GetAll().AsNoTracking()
                                         where input.PropertyDics.Any(v => v.PropertyId == p.PropertyId &&
                                               (v.PropertyValueIds == null || v.PropertyValueIds.Count == 0 ||
                                                v.PropertyValueIds.Contains(p.PropertyValueId)))
                                         group p by p.ItemId into g
                                         where g.Count() == input.PropertyDics.Count
                                         select g.Key;

                // Join with testQuery to filter by matching ItemIds
                filteredQuery = from q in testQuery
                                join itemId in propertyMatchQuery on q.ItemId equals itemId
                                select q;
            }

            // Get total count
            var resultCount = await filteredQuery.CountAsync();
            if (resultCount == 0) return new PagedResultDto<GetListQCTestReportOutput>(0, new List<GetListQCTestReportOutput>());

            // Fetch paginated or full results
            var entities = new List<GetListQCTestReportOutput>();
            if (input.UsePagination)
            {
                entities = await filteredQuery.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            }
            else
            {
                // Stream results in chunks to avoid memory issues
                int pageSize = 1000;
                int page = 0;
                while (true)
                {
                    var batch = await filteredQuery.OrderBy(input.Sorting).Skip(page * pageSize).Take(pageSize).ToListAsync();
                    if (!batch.Any()) break;
                    entities.AddRange(batch);
                    page++;
                }
            }

            return new PagedResultDto<GetListQCTestReportOutput>(resultCount, entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_QCTest)]
        public async Task<ReportOutput> GetReportTemplateQCTest()
        {
            var itemPropertyCols = await _propertyRepository.GetAll().AsNoTracking()
                            .Where(t => t.IsActive == true)
                            .Select(t => new CollumnOutput
                            {
                                AllowGroupby = false,
                                AllowFilter = true,
                                ColumnName = t.Name,
                                ColumnLength = 150,
                                ColumnTitle = t.Name,
                                ColumnType = ColumnType.ItemProperty,
                                SortOrder = 11,
                                Visible = true,
                                AllowFunction = null,
                                MoreFunction = null,
                                IsDisplay = false,
                                AllowShowHideFilter = true,
                                ShowHideFilter = true,
                                DefaultValue = t.Id.ToString(),//to init the value of property
                            })
                            .ToListAsync();

            var columns = new List<CollumnOutput>() 
            {
                     // start properties with can filter
                new CollumnOutput
                {
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "Search",
                    ColumnLength = 150,
                    ColumnTitle = "Search",
                    ColumnType = ColumnType.Language,
                    SortOrder = 0,
                    Visible = true,
                    AllowFunction = null,
                    MoreFunction = null,
                    AllowShowHideFilter = false,
                    ShowHideFilter = false,
                    IsDisplay = false//no need to show in col check it belong to filter option
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "DateRange",
                    ColumnLength = 150,
                    ColumnTitle = "DateRange",
                    ColumnType = ColumnType.Language,
                    SortOrder = 0,
                    Visible = true,
                    DefaultValue = "thisMonth",
                    AllowFunction = null,
                    DisableDefault = false,
                    MoreFunction = null,
                    AllowShowHideFilter = false,
                    ShowHideFilter = false,
                    IsDisplay = false//no need to show in col check it belong to filter option
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "Item",
                    ColumnLength = 200,
                    ColumnTitle = "Item",
                    ColumnType = ColumnType.String,
                    SortOrder = 0,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = false,
                    AllowShowHideFilter = true,
                    ShowHideFilter = true,
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "SampleId",
                    ColumnLength = 120,
                    ColumnTitle = "Sample ID",
                    ColumnType = ColumnType.String,
                    SortOrder = 0,
                    Visible = true,
                    AllowFunction = null,
                    DisableDefault = false,
                    IsDisplay = true,
                    AllowShowHideFilter = false,
                    ShowHideFilter = false,
                } , 
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "ItemName",
                    ColumnLength = 200,
                    ColumnTitle = "Item Name",
                    ColumnType = ColumnType.String,
                    SortOrder = 1,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = true,
                    ShowHideFilter = true,
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "Location",
                    ColumnLength = 120,
                    ColumnTitle = "Location",
                    ColumnType = ColumnType.String,
                    SortOrder = 2,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = true,
                    ShowHideFilter = true,
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Amount",
                    ColumnLength = 120,
                    ColumnTitle = "Amount",
                    ColumnType = ColumnType.String,
                    SortOrder = 3,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = true,
                    ShowHideFilter = true,
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "TestParameter",
                    ColumnLength = 100,
                    ColumnTitle = "Term Test",
                    ColumnType = ColumnType.String,
                    SortOrder = 4,
                    Visible = true,
                    AllowFunction = null,
                    DisableDefault = false,
                    IsDisplay = true,
                    AllowShowHideFilter = false,
                    ShowHideFilter = false,
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "ReferenceNo",
                    ColumnLength = 100,
                    ColumnTitle = "Report No",
                    ColumnType = ColumnType.String,
                    SortOrder = 5,
                    Visible = true,
                    AllowFunction = null,
                    DisableDefault = false,
                    IsDisplay = true,
                    AllowShowHideFilter = false,
                    ShowHideFilter = false,
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "SendDate",
                    ColumnLength = 100,
                    ColumnTitle = "Sending",
                    ColumnType = ColumnType.Date,
                    SortOrder = 6,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = false,
                    ShowHideFilter = false,
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "ResultDate",
                    ColumnLength = 100,
                    ColumnTitle = "Result Date",
                    ColumnType = ColumnType.Date,
                    SortOrder = 7,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = false,
                    ShowHideFilter = false,
                },
                      
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "ActualValue",
                    ColumnLength = 250,
                    ColumnTitle = "Results",
                    ColumnType = ColumnType.String,
                    SortOrder = 8,
                    Visible = true,
                    AllowFunction = null,
                    DisableDefault = false,
                    IsDisplay = true,
                    AllowShowHideFilter = false,
                    ShowHideFilter = false,
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "LabName",
                    ColumnLength = 120,
                    ColumnTitle = "Lab Name",
                    ColumnType = ColumnType.String,
                    SortOrder = 9,
                    Visible = true,
                    AllowFunction = null,
                    DisableDefault = false,
                    IsDisplay = true,
                    AllowShowHideFilter = true,
                    ShowHideFilter = true,
                },
                     
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Remark",
                    ColumnLength = 120,
                    ColumnTitle = "Remark",
                    ColumnType = ColumnType.String,
                    SortOrder = 12,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    AllowShowHideFilter = false,
                    ShowHideFilter = false,
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "LabType",
                    ColumnLength = 150,
                    ColumnTitle = "Lab Type",
                    ColumnType = ColumnType.String,
                    SortOrder = 21,
                    Visible = true,
                    IsDisplay = false,
                    AllowShowHideFilter = true,
                    ShowHideFilter = true,
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "QCTestTemplate",
                    ColumnLength = 150,
                    ColumnTitle = "QC Test Template",
                    ColumnType = ColumnType.String,
                    SortOrder = 22,
                    Visible = true,
                    IsDisplay = false,
                    AllowShowHideFilter = true,
                    ShowHideFilter = true,
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "PassFail",
                    ColumnLength = 150,
                    ColumnTitle = "Pass Fail",
                    ColumnType = ColumnType.Bool,
                    SortOrder = 23,
                    Visible = true,
                    IsDisplay = false,
                    AllowShowHideFilter = true,
                    ShowHideFilter = true,
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "LabTestStatus",
                    ColumnLength = 150,
                    ColumnTitle = "Test Status",
                    ColumnType = ColumnType.String,
                    SortOrder = 23,
                    Visible = true,
                    IsDisplay = false,
                    AllowShowHideFilter = true,
                    ShowHideFilter = true,
                }, 
                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "User",
                    ColumnLength = 100,
                    ColumnTitle = "User",
                    ColumnType = ColumnType.String,
                    SortOrder = 24,
                    Visible = true,
                    IsDisplay = false,
                    AllowShowHideFilter = true,
                    ShowHideFilter = true,
                }
            };

            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = columns.Concat(itemPropertyCols).ToList(),
                Groupby = "",
                HeaderTitle = "QC Test Report",
                Sortby = "",
            };

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_QCTest_Export_Excel)]
        public async Task<FileDto> ExportExcelQCTestReport(GetQCTestReportInput input)
        {
            input.UsePagination = false;
            input.IsLoadMore = false;

            // Query get collumn header 
            //var @entity = await _reportTemplateManager.GetAsync(ReportType.ReportType_Outbounce);
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;

            var billData = (await GetQCTestReport(input)).Items;

            var result = new FileDto();

            //Creates a blank workbook. Use the using statment, so the package is disposed when we are done.
            using (var p = new ExcelPackage())
            {
                //A workbook must have at least on cell, so lets add one... 
                var ws = p.Workbook.Worksheets.Add(sheetName);
                ws.PrinterSettings.Orientation = eOrientation.Landscape;
                ws.PrinterSettings.FitToPage = true;
                //ws.PrinterSettings.PaperSize = ePaperSize.A3; //set default format paper size 
                ws.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                ws.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet

                #region Row 1 Header
                AddTextToCell(ws, 1, 1, sheetName, true);
                MergeCell(ws, 1, 1, 1, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 1


                #region Row 2 Header
                var header = input.ToDate.Value.ToString("dd-MM-yyyy");
                AddTextToCell(ws, 2, 1, header, true);
                MergeCell(ws, 2, 1, 2, reportCountColHead, ExcelHorizontalAlignment.Center);
                #endregion Row 2

                #region Row 3 Header Table
                int rowTableHeader = 3;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                foreach (var i in reportCollumnHeader)
                {  
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colHeaderTable += 1;
                }

                #endregion Row 3

                #region Row Body 
                // use for check auto dynamic col footer of sum value
                Dictionary<string, string> footerGroupDict = new Dictionary<string, string>();
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;

                var groupBy = new List<GetListQCTestReportGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "Location":
                            groupBy = billData
                                .GroupBy(t => t.Location)
                                .Select(t => new GetListQCTestReportGroupByOutput
                                {
                                    KeyName = t.Key,
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                      
                        case "LabName":
                            groupBy = billData
                                .GroupBy(t => t.LabName)
                                .Select(t => new GetListQCTestReportGroupByOutput
                                {
                                    KeyName = t.Key,
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "ResultDate":
                            groupBy = billData
                                .GroupBy(t => t.ResultDate.Date)
                                .Select(t => new GetListQCTestReportGroupByOutput
                                {
                                    KeyName = t.Key.ToString(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;

                        case "Item":
                            groupBy = billData
                                .GroupBy(t => t.ItemId)
                                .Select(t => new GetListQCTestReportGroupByOutput
                                {
                                    KeyName = t.Select(x => x.ItemName).FirstOrDefault(),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }
                // write body

                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0)
                {
                    foreach (var k in groupBy)
                    {
                        //key group by name
                        AddTextToCell(ws, rowBody, 1, k.KeyName, true);
                        MergeCell(ws, rowBody, 1, rowBody, reportCountColHead, ExcelHorizontalAlignment.Left);
                        rowBody += 1;
                        count = 1;
                        //list of item
                        foreach (var i in k.Items)
                        {
                            int collumnCellBody = 1;
                            foreach (var item in reportCollumnHeader)// map with correct key of properties 
                            {
                                WriteBodyQCTest(ws, rowBody, collumnCellBody, item, i, count);
                                collumnCellBody += 1;
                            }
                            rowBody += 1;
                            count += 1;
                        }

                        //sub total of group by item
                        int collumnCellGroupBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {
                            if (item.AllowFunction != null)
                            {   
                                var fromCell = GetAddressName(ws, rowBody - k.Items.Count, collumnCellGroupBody);
                                var toCell = GetAddressName(ws, rowBody - 1, collumnCellGroupBody);
                                if (footerGroupDict.ContainsKey(item.ColumnName))
                                {
                                    footerGroupDict[item.ColumnName] += fromCell + ":" + toCell + ",";
                                }
                                else
                                {
                                    footerGroupDict.Add(item.ColumnName, fromCell + ":" + toCell + ",");
                                }
                                AddFormula(ws, rowBody, collumnCellGroupBody, "SUM(" + fromCell + ":" + toCell + ")", true);
                            }
                            collumnCellGroupBody += 1;
                        }
                        rowBody += 1;
                    }
                }
                else
                {
                    foreach (var i in billData)
                    {
                        int collumnCellBody = 1;
                        foreach (var item in reportCollumnHeader)// map with correct key of properties 
                        {  
                            WriteBodyQCTest(ws, rowBody, collumnCellBody, item, i, count);
                            collumnCellBody += 1;
                        }
                        rowBody += 1;
                        count += 1;
                    }
                }
                #endregion Row Body


                #region Row Footer 
                if (reportHasShowFooterTotal.Count > 0)
                {
                    int footerRow = rowBody;
                    int footerColNumber = 1;
                    AddTextToCell(ws, footerRow, footerColNumber, "TOTAL", true);
                    foreach (var i in reportCollumnHeader)
                    {
                        if (i.AllowFunction != null)
                        {
                            if (!input.GroupBy.IsNullOrWhiteSpace())
                            {
                                //sum custom range cell depend on group item
                                int rowFooter = rowTableHeader + 1;// get start row after 
                                var sumValue = footerGroupDict.ContainsKey(i.ColumnName) ? "SUM(" + footerGroupDict[i.ColumnName] + ")" : "";
                                if (i.ColumnType == ColumnType.Number)
                                {
                                    AddFormula(ws, footerRow, footerColNumber, sumValue, true, false, true);
                                }
                                else
                                {
                                    AddFormula(ws, footerRow, footerColNumber, sumValue, true);
                                }
                            }
                            else
                            {   
                                int rowFooter = rowTableHeader + 1;// get start row after header 
                                var fromCell = GetAddressName(ws, rowFooter, footerColNumber);
                                var toCell = GetAddressName(ws, footerRow - 1, footerColNumber);
                                if (i.ColumnType == ColumnType.Number)
                                {
                                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true, false, true);
                                }
                                else
                                {
                                    AddFormula(ws, footerRow, footerColNumber, "SUM(" + fromCell + ":" + toCell + ")", true);
                                }
                            }
                        }
                        footerColNumber += 1;
                    }
                }
                #endregion Row Footer


                result.FileName = $"QC_Test_Report_{header.Replace(" ", "_").Replace("-", "_")}.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_QCTest_Export_Pdf)]
        public async Task<FileDto> ExportPDFQCTestReport(GetQCTestReportInput input)
        {
            input.UsePagination = false;
            input.IsLoadMore = false;

            var tenant = await GetCurrentTenantAsync();
            var formatDate = await _formatRepository.GetAll()
                        .Where(t => t.Id == tenant.FormatDateId).AsNoTracking()
                        .Select(t => t.Web).FirstOrDefaultAsync();

            if (formatDate.IsNullOrEmpty())
            {
                formatDate = "dd/MM/yyyy";
            }

            var rounding = await GetCurrentCycleAsync();
            var report = input.ReportOutput;
            var reportCollumnHeader = report.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
            var reportHasShowFooterTotal = reportCollumnHeader.Where(t => t.AllowFunction != null).ToList();
            //int reportCountColHead = reportCollumnHeader.Count();
            var sheetName = report.HeaderTitle;
            var user = await GetCurrentUserAsync();
            var bills = await GetQCTestReport(input);

            return await Task.Run(async () =>
            {
                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                FileDto fileDto = new FileDto()
                {
                    SubFolder = null,
                    FileName = "InventoryReportPdf.pdf",
                    FileToken = "InventoryReportPdf.html",
                    FileType = MimeTypeNames.TextHtml
                };
                try
                {
                    templateHtml = await _fileStorageManager.GetTemplate(fileDto.FileToken);//retrive template from path
                    templateHtml = templateHtml.Trim();
                }
                catch (FileNotFoundException)
                {
                    throw new UserFriendlyException("FileNotFound");
                }
                //ToDo: Replace and concat string to be the same what frontend did
                exportHtml = templateHtml;

                var contentBody = string.Empty;
                var contentHeader = string.Empty;

                var viewHeader = reportCollumnHeader.OrderBy(x => x.SortOrder);
                var documentWidth = 1080;
                decimal totalVisibleColsWidth = 0;
                decimal totalTableWidth = 0;
                int reportCountColHead = viewHeader.Where(t => t.Visible == true).Count();
               
                foreach (var i in viewHeader)
                {
                    if (i.Visible)
                    {
                        if (documentWidth >= (totalVisibleColsWidth + i.ColumnLength))
                        {
                            var rowHeader = $"<th rowspan='2' width='{i.ColumnLength}px'>{i.ColumnTitle}</th>";

                            contentHeader += rowHeader;
                            totalTableWidth += i.ColumnLength;
                        }
                        else
                        {
                            i.Visible = false;
                            reportCountColHead--;
                        }
                        totalVisibleColsWidth += i.ColumnLength;
                    }
                }

                exportHtml = exportHtml.Replace("{{tableWidth}}", totalTableWidth.ToString());

                #region Row Body              

                var groupBy = new List<GetListQCTestReportGroupByOutput>();
                //if has groupBy
                if (!input.GroupBy.IsNullOrWhiteSpace())
                {
                    switch (input.GroupBy)
                    {
                        case "Location":
                            groupBy = bills.Items
                                .GroupBy(t => t.Location)
                                .Select(t => new GetListQCTestReportGroupByOutput
                                {
                                    KeyName = t.Key,
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        
                        case "LabName":
                            groupBy = bills.Items
                                .GroupBy(t => t.LabName)
                                .Select(t => new GetListQCTestReportGroupByOutput
                                {
                                    KeyName = t.Key,
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                        case "ResultDate":
                            groupBy = bills.Items
                                .GroupBy(t => t.ResultDate.Date)
                                .Select(t => new GetListQCTestReportGroupByOutput
                                {
                                    KeyName = t.Key.ToString(formatDate),
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;

                        case "Item":
                            groupBy = bills.Items
                                .GroupBy(t => t.ItemName)
                                .Select(t => new GetListQCTestReportGroupByOutput
                                {
                                    KeyName = t.Key,
                                    Items = t.Select(x => x).ToList()
                                })
                                .ToList();
                            break;
                    }
                }

                // write body
                if (!input.GroupBy.IsNullOrWhiteSpace() && groupBy.Count > 0) //write body have group by
                {
                    var trGroup = "";
                    foreach (var k in groupBy)
                    {
                        trGroup += $"<tr style='page-break-before: auto; page-break-after: auto;'>" +
                                   $"<td style='font-weight: bold;' colspan='{reportCountColHead}'> " + k.KeyName +
                                   $" </td></tr>";
                        foreach (var row in k.Items)
                        {
                            trGroup += "<tr style='page-break-before: auto; page-break-after: auto; vertical-align:top;'>";
                            foreach (var i in viewHeader)
                            {
                                if (i.Visible)
                                {
                                    if (i.ColumnName == "ItemName")
                                    {
                                        trGroup += $"<td>{row.ItemName}</td>";
                                    }
                                    else if (i.ColumnName == "ReferenceNo")
                                    {
                                        trGroup += $"<td>{row.ReferenceNo}</td>";
                                    }
                                    else if (i.ColumnName == "Location")
                                    {
                                        trGroup += $"<td>{row.Location}</td>";
                                    }
                                    else if (i.ColumnName == "SendDate")
                                    {
                                        trGroup += $"<td>{row.SendDate.ToString(formatDate)}</td>";
                                    }
                                    else if (i.ColumnName == "ResultDate")
                                    {
                                        trGroup += $"<td>{row.ResultDate.ToString(formatDate)}</td>";
                                    }
                                    else if (i.ColumnName == "Remark")
                                    {
                                        trGroup += $"<td>{row.Remark}</td>";
                                    }
                                    else if (i.ColumnName == "LabName")
                                    {
                                        trGroup += $"<td>{row.LabName}</td>";
                                    }
                                    else if(i.ColumnName == "SampleId")
                                    {
                                        trGroup += $"<td>{row.SampleId}</td>";
                                    }
                                    else if(i.ColumnName == "TestParameter")
                                    {
                                        var value = row.TestResultDetails.IsNullOrEmpty() ? "" : string.Join("<br>", row.TestResultDetails.Select(s => $"{s.TestParameterName}"));
                                        trGroup += $"<td>{ value }</td>";
                                    }
                                    else if(i.ColumnName == "ActualValue")
                                    {
                                        var value = "";
                                        if (row.DetailEntry)
                                        {
                                            value = row.TestResultDetails.IsNullOrEmpty() ? "" : string.Join("<br>", row.TestResultDetails.Select(s => $"{s.ActualValue}"));
                                        }
                                        else
                                        {
                                            value = row.FinalPassFail ? L("Pass") : L("Fail");
                                        }
                                        trGroup += $"<td>{ value }</td>";
                                    }
                                    else
                                    {
                                        trGroup += $"<td></td>";
                                    }
                                }
                            }
                            trGroup += "</tr>";
                        }
                        trGroup += "<tr style='page-break-before: auto; page-break-after: auto;'>";
                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                //if (!string.IsNullOrEmpty(i.AllowFunction))/*listSum.Contains(i.ColumnName))*/
                                //{   
                                //    if (i.ColumnName == "Total")
                                //    {
                                //        trGroup += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(k.Items.Sum(t => t.Total), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                //    }
                                //    else if (i.ColumnName == "NetWeight")
                                //    {
                                //        trGroup += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(k.Items.Sum(t => t.NetWeight), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";

                                //    }
                                //    else if (i.ColumnName == "Qty")
                                //    {
                                //        trGroup += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(k.Items.Sum(t => t.Qty), rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";

                                //    }
                                //}
                                //else //none sum
                                {
                                    trGroup += $"<td></td>";
                                }

                            }
                        }
                        trGroup += "</tr>";
                    }
                    contentBody += trGroup;
                }
                else // write body no group by
                {
                    foreach (var row in bills.Items)
                    {
                        var tr = "<tr style='page-break-before: auto; page-break-after: auto; vertical-align:top;'>";
                        foreach (var i in viewHeader)
                        {
                            if (i.Visible)
                            {
                                if (i.ColumnName == "ItemName")
                                {
                                    tr += $"<td>{row.ItemName}</td>";
                                }
                                else if (i.ColumnName == "ReferenceNo")
                                {
                                    tr += $"<td>{row.ReferenceNo}</td>";
                                }
                                else if (i.ColumnName == "Location")
                                {
                                    tr += $"<td>{row.Location}</td>";
                                }
                                else if (i.ColumnName == "SendDate")
                                {
                                    tr += $"<td>{row.SendDate.ToString(formatDate)}</td>";
                                }
                                else if (i.ColumnName == "ResultDate")
                                {
                                    tr += $"<td>{row.ResultDate.ToString(formatDate)}</td>";
                                }
                                else if (i.ColumnName == "Remark")
                                {
                                    tr += $"<td>{row.Remark}</td>";
                                }
                                else if (i.ColumnName == "LabName")
                                {
                                    tr += $"<td>{row.LabName}</td>";
                                }
                                else if (i.ColumnName == "SampleId")
                                {
                                    tr += $"<td>{row.SampleId}</td>";
                                }
                                else if (i.ColumnName == "TestParameter")
                                {
                                    var value = row.TestResultDetails.IsNullOrEmpty() ? "" : string.Join("<br>", row.TestResultDetails.Select(s => $"{s.TestParameterName}"));
                                    tr += $"<td>{value}</td>";
                                }
                                else if (i.ColumnName == "ActualValue")
                                {
                                    var value = "";
                                    if (row.DetailEntry)
                                    {
                                        value = row.TestResultDetails.IsNullOrEmpty() ? "" : string.Join("<br>", row.TestResultDetails.Select(s => $"{s.ActualValue}"));
                                    }
                                    else
                                    {
                                        value = row.FinalPassFail ? L("Pass") : L("Fail");
                                    }
                                       
                                    tr += $"<td>{value}</td>";
                                }
                                else
                                {
                                    tr += $"<td></td>";
                                }
                            }
                        }
                        tr += "</tr>";
                        contentBody += tr;
                    }
                }
                #endregion Row Body

                #region Row Footer 

                if (reportHasShowFooterTotal.Count > 0)
                {
                    var index = 0;
                    var tr = "<tr style='page-break-before: auto; page-break-after: auto;'>";
                    foreach (var i in viewHeader)
                    {
                        if (i.Visible)
                        {
                            if (index == 0)
                            {
                                tr += $"<td style='font-weight: bold;text-align:right;'>{L("Total")}</td>";
                            }
                            else
                            {
                                //if (!string.IsNullOrEmpty(i.AllowFunction))
                                //{
                                //    if (bills.TotalResults != null && bills.TotalResults.ContainsKey(i.ColumnName))
                                //    {
                                //        foreach (var subCol in subCols)
                                //        {
                                //            var total = bills.TotalResults[i.ColumnName].ContainsKey(subCol) ? bills.TotalResults[i.ColumnName][subCol] : 0;
                                //            tr += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(total, rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                //        }
                                //    }
                                //    else if (bills.TotalResult != null && bills.TotalResult.ContainsKey(i.ColumnName))
                                //    {
                                //        tr += $"<td style='font-weight: bold;text-align:right;'>{FormatNumberCurrency(Math.Round(bills.TotalResult[i.ColumnName], rounding.RoundingDigit, MidpointRounding.ToEven), rounding.RoundingDigit)}</td>";
                                //    }
                                //}
                                //else //none sum
                                {
                                    tr += $"<td></td>";
                                }
                            }
                            index++;
                        }
                    }
                    tr += "</tr>";
                    contentBody += tr;
                }
                #endregion Row Footer

                exportHtml = exportHtml.Replace("{{rowHeader}}", contentHeader);
                exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
                exportHtml = exportHtml.Replace("{{subHeader}}", "");
                EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();
                AddHeaderPDF(htmlToPdfConverter, tenant.Name, sheetName, input.FromDate.Value, input.ToDate.Value, formatDate);
                AddFooterPDF(htmlToPdfConverter, user.FullName, formatDate);
                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");
                var tokenName = $"{Guid.NewGuid()}.pdf";
                var result = new FileDto();
                result.FileName = $"{sheetName}.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);
                return result;

            });
        }
        #endregion

    }
}
