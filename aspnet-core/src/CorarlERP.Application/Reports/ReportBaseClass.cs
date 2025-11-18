using Abp.AspNetZeroCore.Net;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Timing;
using Abp.Timing.Timezone;
using Abp.UI;
using CorarlERP.AccountCycles;
using CorarlERP.AccountTransactions;
using CorarlERP.AccountTypes.Dto;
using CorarlERP.Authorization.Users.Dto;
using CorarlERP.Bills;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Configuration;
using CorarlERP.Customers;
using CorarlERP.Customers.Dto;
using CorarlERP.CustomerTypes;
using CorarlERP.Dto;
using CorarlERP.FileStorages;
using CorarlERP.Inventories.Data;
using CorarlERP.Invoices;
using CorarlERP.ItemIssues;
using CorarlERP.ItemIssues.Dto;
using CorarlERP.ItemPriceAppServices.Dto;
using CorarlERP.ItemReceiptCustomerCredits;
using CorarlERP.ItemReceipts;
using CorarlERP.ItemReceipts.Dto;
using CorarlERP.Items;
using CorarlERP.Journals.Dto;
using CorarlERP.Locations;
using CorarlERP.Lots.Dto;
using CorarlERP.ProductionPlans.Dto;
using CorarlERP.Productions.Dto;
using CorarlERP.PropertyValues;
using CorarlERP.PropertyValues.Dto;
using CorarlERP.PurchaseOrders;
using CorarlERP.PurchasePrices.Dto;
using CorarlERP.ReceivePayments.Dto;
using CorarlERP.Reports.Dto;
using CorarlERP.ReportTemplates;
using CorarlERP.SaleOrders;
using CorarlERP.SubItems.Dto;
using CorarlERP.UserActivities.Dto;
using CorarlERP.UserGroups;
using CorarlERP.Vendors;
using CorarlERP.Vendors.Dto;
using CorarlERP.VendorTypes;
using EvoPdf.HtmlToPdfClient;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using OfficeOpenXml.Style;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Reports
{
    public abstract class ReportBaseClass : CorarlERPAppServiceBase
    {
        private readonly ITimeZoneConverter _timeZoneConverter;

        public const string DefaultFontName = "Khmer OS Battambang";
        public const int DefaultFontSize = 12;

        private readonly IRepository<AccountCycle, long> _accountCycleRepository;

        protected IAppFolders _appFolders;
        private readonly IRepository<UserGroupMember, Guid> _userGroupMemberRepository;
        public IAppFolders AppFolders { get { return _appFolders; } }

        private readonly IRepository<ItemReceipt, Guid> _itemReceiptRepository;
        private readonly IRepository<ItemReceiptItem, Guid> _itemReceiptItemRepository;
        private readonly IRepository<ItemReceiptItemCustomerCredit, Guid> _itemReceiptCustomerCreditItemRepository;
        private readonly ICorarlRepository<PurchaseOrder, Guid> _purchaseOrderRepository;
        private readonly ICorarlRepository<PurchaseOrderItem, Guid> _purchaseOrderItemRepository;
        private readonly IRepository<Bill, Guid> _billRepository;
        private readonly IPurchaseOrderManager _purchaseOrderManager;


        private readonly IRepository<ItemIssue, Guid> _itemIssueRepository;
        private readonly IRepository<ItemIssueItem, Guid> _itemIssueItemRepository;
        private readonly ICorarlRepository<SaleOrder, Guid> _saleOrderRepository;
        private readonly IRepository<SaleOrderItem, Guid> _saleOrderItemRepository;
        private readonly IRepository<Invoice, Guid> _invoiceRepository;
        private readonly ISaleOrderManager _saleOrderManager;
        private readonly IRepository<Item, Guid> _itemRepository;


        private readonly IConfigurationRoot _appConfiguration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IRepository<ItemProperty, Guid> _itemPropertyRepository;

        private readonly IFileStorageManager _fileStorageManager;

        protected ReportBaseClass(
            IRepository<AccountCycle, long> accountCycleRepository,
            AppFolders appFolders,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            IRepository<Location, long> locationRepository
        ) : base(accountCycleRepository, userGroupMemberRepository, locationRepository)
        {
            _timeZoneConverter = IocManager.Instance.Resolve<ITimeZoneConverter>();
            LocalizationSourceName = CorarlERPConsts.LocalizationSourceName;
            _accountCycleRepository = accountCycleRepository;
            _appFolders = appFolders;

            _itemReceiptRepository = IocManager.Instance.Resolve<IRepository<ItemReceipt, Guid>>();
            _itemReceiptItemRepository = IocManager.Instance.Resolve<IRepository<ItemReceiptItem, Guid>>();
            _itemReceiptCustomerCreditItemRepository = IocManager.Instance.Resolve<IRepository<ItemReceiptItemCustomerCredit, Guid>>();
            _billRepository = IocManager.Instance.Resolve<IRepository<Bill, Guid>>();
            _purchaseOrderManager = IocManager.Instance.Resolve<IPurchaseOrderManager>();
            _purchaseOrderItemRepository = IocManager.Instance.Resolve<ICorarlRepository<PurchaseOrderItem, Guid>>();
            _purchaseOrderRepository = IocManager.Instance.Resolve<ICorarlRepository<PurchaseOrder, Guid>>();


            _itemIssueRepository = IocManager.Instance.Resolve<IRepository<ItemIssue, Guid>>();
            _itemIssueItemRepository = IocManager.Instance.Resolve<IRepository<ItemIssueItem, Guid>>();
            _invoiceRepository = IocManager.Instance.Resolve<IRepository<Invoice, Guid>>();
            _saleOrderManager = IocManager.Instance.Resolve<ISaleOrderManager>();
            _saleOrderItemRepository = IocManager.Instance.Resolve<IRepository<SaleOrderItem, Guid>>();
            _saleOrderRepository = IocManager.Instance.Resolve<ICorarlRepository<SaleOrder, Guid>>();
            _itemRepository = IocManager.Instance.Resolve<IRepository<Item, Guid>>();


            _hostingEnvironment = IocManager.Instance.Resolve<IHostingEnvironment>();
            _appConfiguration = _hostingEnvironment.GetAppConfiguration();


            _itemPropertyRepository = IocManager.Instance.Resolve<IRepository<ItemProperty, Guid>>();
            _fileStorageManager = IocManager.Instance.Resolve<IFileStorageManager>();
        }
        public ReportBaseClass()
        {
        }
        public ReportBaseClass(IFileStorageManager fileStorageManager)
        {
            _fileStorageManager = fileStorageManager;
        }


        /// <summary>
        /// Required SaveChanged Before call
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        protected async Task UpdatePurhaseOrderInventoryStatus(Guid OrderId)
        {
            var entity = await _purchaseOrderRepository.FirstOrDefaultAsync(s => s.Id == OrderId);

            if (entity == null) throw new UserFriendlyException("RecordNotFound");

            var purchaseOrderItems = await _purchaseOrderItemRepository.GetAll()
                                        .Include(s => s.ItemReceiptItems)
                                        .Include(s => s.BillItems)
                                        .Where(s => s.PurchaseOrderId == OrderId)
                                        .AsNoTracking()
                                        .ToListAsync();

            if (purchaseOrderItems.All(s => s.BillItems.Where(r => r.ItemReceiptItemId.HasValue).Count() == 0 && s.ItemReceiptItems.Count() == 0))
            {
                entity.UpdateReceiveStatusToPending();
            }
            else if (purchaseOrderItems.All(s => s.Unit == s.BillItems.Where(r => r.ItemReceiptItemId.HasValue).Sum(i => i.Qty) + s.ItemReceiptItems.Sum(i => i.Qty)))
            {
                entity.UpdateReceiveStatusToReceiveAll();
            }
            else
            {
                entity.UpdateReceiveStatusToPartial();
            }

            //Update Receive Count
            var receiveCount = purchaseOrderItems.SelectMany(s => s.ItemReceiptItems.Select(b => b.ItemReceiptId)).GroupBy(g => g).Count();
            var billCount = purchaseOrderItems.SelectMany(s => s.BillItems.Where(b => b.ItemReceiptItemId.HasValue).Select(b => b.BillId)).GroupBy(g => g).Count();
            entity.SetReceiveCount(receiveCount + billCount);

            await _purchaseOrderManager.UpdateAsync(entity);
        }

        protected async Task UpdatePurhaseOrderInventoryStatus(List<Guid> orderIds)
        {
            var orders = await _purchaseOrderRepository.GetAll().AsNoTracking().Where(s => orderIds.Contains(s.Id)).ToListAsync();

            if (orderIds.IsNullOrEmpty()) return;

            var orderItems = await _purchaseOrderItemRepository.GetAll()
                                        .Include(s => s.ItemReceiptItems)
                                        .Include(s => s.BillItems)
                                        .AsNoTracking()
                                        .Where(s => orderIds.Contains(s.PurchaseOrderId))
                                        .ToListAsync();

            foreach (var entity in orders)
            {
                var purchaseOrderItems = orderItems.Where(s => s.PurchaseOrderId == entity.Id).ToList();

                if (purchaseOrderItems.All(s => s.BillItems.Where(r => r.ItemReceiptItemId.HasValue).Count() == 0 && s.ItemReceiptItems.Count() == 0))
                {
                    entity.UpdateReceiveStatusToPending();
                }
                else if (purchaseOrderItems.All(s => s.Unit == s.BillItems.Where(r => r.ItemReceiptItemId.HasValue).Sum(i => i.Qty) + s.ItemReceiptItems.Sum(i => i.Qty)))
                {
                    entity.UpdateReceiveStatusToReceiveAll();
                }
                else
                {
                    entity.UpdateReceiveStatusToPartial();
                }

                //Update Receive Count
                var receiveCount = purchaseOrderItems.SelectMany(s => s.ItemReceiptItems.Select(b => b.ItemReceiptId)).GroupBy(g => g).Count();
                var billCount = purchaseOrderItems.SelectMany(s => s.BillItems.Where(b => b.ItemReceiptItemId.HasValue).Select(b => b.BillId)).GroupBy(g => g).Count();
                entity.SetReceiveCount(receiveCount + billCount);

            }

            await _purchaseOrderRepository.BulkUpdateAsync(orders);
        }


        protected async Task UpdateSaleOrderInventoryStatus(List<Guid> orderIds)
        {
            var orders = await _saleOrderRepository.GetAll().AsNoTracking().Where(s => orderIds.Contains(s.Id)).ToListAsync();

            if (orderIds.IsNullOrEmpty()) return;

            var orderItems = await _saleOrderItemRepository.GetAll()
                                        .Include(s => s.ItemIssueItems)
                                        .Include(s => s.InvoiceItems)
                                        .AsNoTracking()
                                        .Where(s => orderIds.Contains(s.SaleOrderId))
                                        .ToListAsync();

            foreach (var entity in orders)
            {
                var saleOrderItems = orderItems.Where(s => s.SaleOrderId == entity.Id).ToList();

                if (saleOrderItems.All(s => s.InvoiceItems.Where(r => r.ItemIssueItemId.HasValue).Count() == 0 && s.ItemIssueItems.Count() == 0))
                {
                    entity.UpdateReceiveStatusToPending();
                }
                else if (saleOrderItems.All(s => s.UnitCost == s.InvoiceItems.Where(r => r.ItemIssueItemId.HasValue).Sum(i => i.Qty) + s.ItemIssueItems.Sum(i => i.Qty)))
                {
                    entity.UpdateReceiveStatusToReceiveAll();
                }
                else
                {
                    entity.UpdateReceiveStatusToPartial();
                }

                //Update Receive Count
                var receiveCount = saleOrderItems.SelectMany(s => s.ItemIssueItems.Select(b => b.ItemIssueId)).GroupBy(g => g).Count();
                var invoiceCount = saleOrderItems.SelectMany(s => s.InvoiceItems.Where(b => b.ItemIssueItemId.HasValue).Select(b => b.InvoiceId)).GroupBy(g => g).Count();
                entity.SetIssueCount(receiveCount + invoiceCount);

            }

            await _saleOrderRepository.BulkUpdateAsync(orders);
        }




        protected IQueryable<InventoryPropertySummaryOutput> GetItemPropertiesQuery()
        {
            var itemProperty = _itemRepository.GetAll().Include(s => s.InventoryAccount)
                                .Include(u => u.Properties)
                                 .ThenInclude(u => u.Property)
                                 .Include(u => u.Properties)
                                 .ThenInclude(u => u.PropertyValue)
                               .Where(t => t.Properties != null && t.Properties.Count > 0)
                               .AsNoTracking()
                               .Select(t => new InventoryPropertySummaryOutput
                               {
                                   //AccountTypeId = t.InventoryAccount == null ? (long?)null : t.InventoryAccount.AccountTypeId,
                                   ItemId = t.Id,
                                   InventoryAccountCode = t.InventoryAccount == null ? null : t.InventoryAccount.AccountCode,
                                   InventoryAccountId = t.InventoryAccountId,
                                   InventoryAccountName = t.InventoryAccount == null ? null : t.InventoryAccount.AccountName,
                                   ItemCode = t.ItemCode,
                                   ItemName = t.ItemName,
                                   SalePrice = t.SalePrice,
                                   Properties = t.Properties.Select(x => new ItemPropertySummary
                                   {
                                       Id = x.PropertyValueId == null ? 0 : x.PropertyValueId.Value,
                                       PropertyId = x.PropertyId,
                                       PropertyName = x.Property.Name,
                                       ValueId = x.PropertyValueId,
                                       Value = x.PropertyValue == null ? "" : x.PropertyValue.Value,
                                       IsUnit = x.Property.IsUnit,
                                       NetWeight = x.PropertyValue == null ? 0 : x.PropertyValue.NetWeight
                                   }).ToList()
                               });

            return itemProperty;
        }

        protected void WriteBodyLedger(
            ExcelWorksheet ws,
            int rowIndex,
            int columnIndex,
            CollumnOutput item,
            AccountTransactionOutput i,
            int count)
        {
            string value = "";
            switch (item.ColumnName)
            {
                case "JournalType":
                    var type = i.GetType().GetProperty(item.ColumnName).GetValue(i, null);
                    value = type != null ? L(type?.ToString()) : "";
                    break;
                case "No":
                    value = count.ToString();
                    break;
                default:
                    var text = i.GetType().GetProperty(item.ColumnName).GetValue(i, null);
                    value = text != null ? text?.ToString() : "";
                    break;
            }


            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
                MergeCell(ws, rowIndex, columnIndex, rowIndex, columnIndex, ExcelHorizontalAlignment.Center);
            }
            else if (item.ColumnType == ColumnType.AutoNumber && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
                MergeCell(ws, rowIndex, columnIndex, rowIndex, columnIndex, ExcelHorizontalAlignment.Center);
            }
            else if (item.ColumnType == ColumnType.Date && !value.IsNullOrEmpty())
            {
                AddDateToCell(ws, rowIndex, columnIndex, DateTime.Parse(value));
                MergeCell(ws, rowIndex, columnIndex, rowIndex, columnIndex, ExcelHorizontalAlignment.Center);
            }
            else if (item.ColumnType == ColumnType.Money && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, false);
                //MergeCell(ws, rowIndex, columnIndex, rowIndex, columnIndex, ExcelHorizontalAlignment.Center);
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
                //merge row of cols
                //MergeCell(ws, rowIndex, columnIndex, rowIndex, columnIndex, ExcelHorizontalAlignment.Center);
            }
        }


        protected void WriteBodyCashReport(
           ExcelWorksheet ws,
           int rowIndex,
           int columnIndex,
           CollumnOutput item,
           CashReportOutput i,
           int count)
        {
            string value = "";
            switch (item.ColumnName)
            {
                case "JournalType":
                    var type = i.GetType().GetProperty(item.ColumnName).GetValue(i, null);
                    value = type != null ? L(type?.ToString()) : "";
                    break;
                case "No":
                    value = count.ToString();
                    break;
                default:
                    var text = i.GetType().GetProperty(item.ColumnName).GetValue(i, null);
                    value = text != null ? text?.ToString() : "";
                    break;
            }


            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
                MergeCell(ws, rowIndex, columnIndex, rowIndex, columnIndex, ExcelHorizontalAlignment.Center);
            }
            else if (item.ColumnType == ColumnType.AutoNumber && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
                MergeCell(ws, rowIndex, columnIndex, rowIndex, columnIndex, ExcelHorizontalAlignment.Center);
            }
            else if (item.ColumnType == ColumnType.Date && !value.IsNullOrEmpty())
            {
                AddDateToCell(ws, rowIndex, columnIndex, DateTime.Parse(value));
                MergeCell(ws, rowIndex, columnIndex, rowIndex, columnIndex, ExcelHorizontalAlignment.Center);
            }
            else if (item.ColumnType == ColumnType.Money && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, false);
                //MergeCell(ws, rowIndex, columnIndex, rowIndex, columnIndex, ExcelHorizontalAlignment.Center);
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
                //merge row of cols
                //MergeCell(ws, rowIndex, columnIndex, rowIndex, columnIndex, ExcelHorizontalAlignment.Center);
            }
        }


        protected void WriteBodyJournal(
            ExcelWorksheet ws,
            int rowIndex,
            int columnIndex,
            CollumnOutput item,
            JournalReportOutput i,
            int count)
        {
            string value = "";
            switch (item.ColumnName)
            {
                //case "Account":
                //    break;
                //case "Debit":
                //    break;
                case "JournalType":
                    var type = i.GetType().GetProperty(item.ColumnName).GetValue(i, null);
                    value = type != null ? L(type?.ToString()) : "";
                    break;
                case "No":
                    value = count.ToString();
                    break;
                default:
                    var text = i.GetType().GetProperty(item.ColumnName).GetValue(i, null);
                    value = text != null ? text?.ToString() : "";
                    break;
            }
            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
                MergeCell(ws, rowIndex, columnIndex, i.JournalItems.Count + rowIndex - 1, columnIndex, ExcelHorizontalAlignment.Center);
            }
            else if (item.ColumnType == ColumnType.AutoNumber && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
                MergeCell(ws, rowIndex, columnIndex, i.JournalItems.Count + rowIndex - 1, columnIndex, ExcelHorizontalAlignment.Center);
            }
            else if (item.ColumnType == ColumnType.Date && !value.IsNullOrEmpty())
            {
                AddDateToCell(ws, rowIndex, columnIndex, DateTime.Parse(value));
                MergeCell(ws, rowIndex, columnIndex, i.JournalItems.Count + rowIndex - 1, columnIndex, ExcelHorizontalAlignment.Center);
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
                //merge row of cols
                MergeCell(ws, rowIndex, columnIndex, i.JournalItems.Count + rowIndex - 1, columnIndex, ExcelHorizontalAlignment.Center);
            }
        }


        protected void AddCellValue(
            ExcelWorksheet ws,
            int rowIndex,
            int columnIndex,
            CollumnOutput item,
            object cellValue,
            int roundDigits = 2)
        {

            var text = cellValue;
            var value = text != null ? text.ToString() : "";

            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money) && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
            }
            else if ((item.ColumnType == ColumnType.RoundingDigit) && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true, false, roundDigits);
            }
            else if (item.ColumnType == ColumnType.Date && !value.IsNullOrEmpty())
            {
                AddDateToCell(ws, rowIndex, columnIndex, DateTime.Parse(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }

        }

        protected void WriteBodyJournalSubItems(
            ExcelWorksheet ws,
            int rowIndex,
            int columnIndex,
            CollumnOutput item,
            JournalItemDetailOutput i
        )
        {
            string value = "";
            Object text;
            switch (item.ColumnName)
            {
                case "Account":
                    var account = i.GetType().GetProperty("Account").GetValue(i, null);
                    text = account.GetType().GetProperty("AccountName").GetValue(account, null);
                    value = text != null ? text?.ToString() : "";
                    break;
                default:
                    text = i.GetType().GetProperty(item.ColumnName).GetValue(i, null);
                    value = text != null ? text?.ToString() : "";
                    break;
            }
            if (item.ColumnName == "Credit" || item.ColumnName == "Debit")
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
            }
            else if (item.ColumnName == "Date")
            {
                AddDateToCell(ws, rowIndex, columnIndex, DateTime.Parse(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }
        protected void WriteBodyChartOfAccount(
           ExcelWorksheet ws,
           int rowIndex,
           int columnIndex,
           CollumnOutput item,
           ChartAccountDetailOutput i,
           int count)
        {
            string value = "";
            var keyName = item.ColumnName;
            //if (keyName == "AccountType")
            //{
            //    var type = i.GetType().GetProperty("AccountType").GetValue(i, null);
            //    var name = type.GetType().GetProperty("AccountTypeName").GetValue(type, null).ToString();
            //    value = name;
            //}
            if (keyName == "ParentAccount")
            {
                var type = i.GetType().GetProperty("ParentAccount").GetValue(i, null);
                value = type != null ? type.GetType().GetProperty("AccountName").GetValue(type, null)?.ToString() : "";
            }
            else if (keyName == "Tax")
            {
                var type = i.GetType().GetProperty("Tax").GetValue(i, null);
                value = type.GetType().GetProperty("TaxName").GetValue(type, null)?.ToString();
            }
            else if (keyName == "No")
            {
                value = count.ToString();
            }
            else
            {
                var text = i.GetType().GetProperty(keyName).GetValue(i, null);
                value = text != null ? text?.ToString() : "";
            }

            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money) && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }


        protected void WriteBodyUserActivity(
          ExcelWorksheet ws,
          int rowIndex,
          int columnIndex,
          CollumnOutput item,
          GetDetailListUserActivityOutput i,
          int count)
        {
            string value = "";
            var keyName = item.ColumnName;

            if (keyName == "User")
            {

                value = i.User;
            }

            else if (keyName == "Description")
            {
                value = i.Description;
            }
            else if (keyName == "Activity")
            {
                value = i.Activity;
            }
            else if (keyName == "Transaction")
            {
                value = i.Transsaction;
            }
            else if (keyName == "Duration")
            {
                value = i.Duration;
            }
            else if (keyName == "Browsers")
            {
                value = i.Browser;
            }
            else if (keyName == "Time")
            {
                value = i.Time.ToString();
            }

            else
            {
                var text = i.GetType().GetProperty(keyName).GetValue(i, null);
                value = text != null ? text?.ToString() : "";
            }

            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money) && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }

        protected void WriteBodyTypeOfAccount(
          ExcelWorksheet ws,
          int rowIndex,
          int columnIndex,
          CollumnOutput item,
          AccountTypeDetailOutput i,
          int count)
        {
            string value = "";
            var keyName = item.ColumnName;
            if (keyName == "Name")
            {
                value = i.AccountTypeName;
            }
            else
            {
                var text = i.GetType().GetProperty(keyName).GetValue(i, null);
                value = text != null ? text?.ToString() : "";
            }

            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money) && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }

        protected void WriteBodyReceivePayment(
                      ExcelWorksheet ws,
                      int rowIndex,
                      int columnIndex,
                      CollumnOutput item,
                      GetListReceivPaymentOutput i,
                      int count)
        {
            string value = "";
            var keyName = item.ColumnName;
            if (keyName == "PaymentNo")
            {
                value = i.JournalNo;
            }
            else if(keyName == "User")
            {
                value = i.User.UserName;
            }
            else if (keyName == "Location")
            {
                value = i.Location.LocationName;
            }
            else if (keyName == "Reference")
            {
                value = i.Reference;
            }
            
            else if(keyName == "Customer")
            {
                value = string.Join(Environment.NewLine, i.CustomerLists.Select(s => $"{s.CustomerName}"));
            }
            else if (keyName == "Account")
            {
                value = value = i.AccountName;
            }
            else if(keyName == "PaymentType")
            {
                value = value = i.ReceiveFrom == ReceiveFromRecievePayment.Cash ? L("Cash") : L("Credit");
            }
            else if(keyName == "TotalPayment")
            {
                var total = i.TotalPayment + (i.ReceiveFrom ==  ReceiveFromRecievePayment.Cash && i.TotalPaymentInvoice > 0 ? i.TotalCustomerCreditPayment : i.ReceiveFrom == ReceiveFromRecievePayment.CustomerCredit ? i.TotalCustomerCreditPayment : 0);                
                value = total.ToString();
            }
            else
            {
                var text = i.GetType().GetProperty(keyName).GetValue(i, null);
                value = text != null ? text?.ToString() : "";
            }

            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money) && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
            }
            else if (item.ColumnType == ColumnType.Date)
            {
                AddDateToCell(ws, rowIndex, columnIndex, i.PaymentDate, false);
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value,false,wrapText: i.CustomerLists.Count > 1);
            }
        }

        protected void WriteBodyItems(
          ExcelWorksheet ws,
          int rowIndex,
          int columnIndex,
          CollumnOutput item,
          Item i,
          int count)
        {
            string value = "";
            var keyName = item.ColumnName;
            var p = i.Properties.Where(t => t.Property.Name == keyName).FirstOrDefault();

            switch (keyName)
            {
                case "InventoryAccount":
                    var typeInventory = i.GetType().GetProperty("InventoryAccount").GetValue(i, null);
                    value = typeInventory != null ? typeInventory.GetType().GetProperty("AccountName").GetValue(typeInventory, null)?.ToString() : "";
                    break;
                //case "ItemType":
                //    var typeItemtype = i.GetType().GetProperty("ItemType").GetValue(i, null);
                //    value = typeItemtype != null ? typeItemtype.GetType().GetProperty("Name").GetValue(typeItemtype, null).ToString() : "";
                //    break;               
                case "PurchaseAccount":
                    var typePurchaseAccount = i.GetType().GetProperty("PurchaseAccount").GetValue(i, null);
                    value = typePurchaseAccount != null ? typePurchaseAccount.GetType().GetProperty("AccountName").GetValue(typePurchaseAccount, null)?.ToString() : "";
                    break;
                case "PurchaseTax":
                    var typePurchaseTax = i.GetType().GetProperty("PurchaseTax").GetValue(i, null);
                    value = typePurchaseTax != null ? typePurchaseTax.GetType().GetProperty("TaxName").GetValue(typePurchaseTax, null)?.ToString() : "";
                    break;
                case "SaleAccount":
                    var typeSaleAccount = i.GetType().GetProperty("SaleAccount").GetValue(i, null);
                    value = typeSaleAccount != null ? typeSaleAccount.GetType().GetProperty("AccountName").GetValue(typeSaleAccount, null)?.ToString() : "";
                    break;

                case "DefaultLot":
                    var lot = i.GetType().GetProperty("DefaultLot").GetValue(i, null);
                    value = lot != null ? lot.GetType().GetProperty("LotName").GetValue(lot, null)?.ToString() : "";
                    break;

                case "SaleTax":
                    var typeSaleTax = i.GetType().GetProperty("SaleTax").GetValue(i, null);
                    value = typeSaleTax != null ? typeSaleTax.GetType().GetProperty("TaxName").GetValue(typeSaleTax, null)?.ToString() : "";
                    break;

                case "TrackSerial":
                    var typeTrackSerial = i.GetType().GetProperty("TrackSerial").GetValue(i, null);
                    value = typeTrackSerial?.ToString();
                    break;

                default:
                    if (p != null)
                    {
                        if (p.PropertyValueId != null)
                        {
                            value = p.PropertyValue.Value;
                        }
                        else
                        {
                            value = null;
                        }
                    }
                    else
                    {
                        var text = i.GetType().GetProperty(keyName)?.GetValue(i, null);
                        value = text != null ? text?.ToString() : "";
                    }
                    break;
            }
            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money) && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }




        protected void WriteBodyItemPrices(
         ExcelWorksheet ws,
         int rowIndex,
         int columnIndex,
         CollumnOutput item,
         GetDetailItemPriceItemOutput i,
         //GetDetailItemPriceOutput h,
         int count)
        {
            string value = "";
            var keyName = item.ColumnName;
            var p = i.GetPriceDetail.Where(t => t.CurrencyCode == keyName).FirstOrDefault();

            switch (keyName)
            {
                //case "SaleType":
                //    var saleType = h != null && h.SaleType != null ? h.SaleType.TransactionTypeName : "";
                //    value = saleType != null ? saleType?.ToString() : "";
                //    break;

                //case "CustomerType":
                //    var customerType = h != null && h.CustomerType != null ? h.CustomerType.CustomerTypeName : "";
                //    value = customerType != null ? customerType?.ToString() : "";
                //    break;

                //case "Location":
                //    var location = h != null && h.Location != null ? h.Location.LocationName : "";
                //    value = location != null ? location?.ToString() : "";
                //    break;                            
                default:
                    if (p != null)
                    {
                        if (p.CurrencyCode == keyName)
                        {
                            value = p.Price != null ? p.Price.ToString() : "0";
                            AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
                        }
                        else
                        {
                            value = null;
                        }
                    }
                    else
                    {
                        var text = i.GetType().GetProperty(keyName)?.GetValue(i, null);
                        value = text != null ? text?.ToString() : "";
                    }
                    break;
            }
            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money) && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }

        
        protected void WriteBodyPurchasePrices(
         ExcelWorksheet ws,
         int rowIndex,
         int columnIndex,
         CollumnOutput item,
         GetDetailPurchasePriceItemOutput i,
         int count)
        {
            string value = "";
            var keyName = item.ColumnName;
            var p = i.GetPriceDetail.Where(t => t.CurrencyCode == keyName).FirstOrDefault();

            switch (keyName)
            {
                                      
                default:
                    if (p != null)
                    {
                        if (p.CurrencyCode == keyName)
                        {
                            value = p.Price.ToString();
                            AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
                        }
                        else
                        {
                            value = null;
                        }
                    }
                    else
                    {
                        var text = i.GetType().GetProperty(keyName)?.GetValue(i, null);
                        value = text != null ? text?.ToString() : "";
                    }
                    break;
            }
            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money) && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }



        protected void WriteBodyPropertys(
        ExcelWorksheet ws,
        int rowIndex,
        int columnIndex,
        CollumnOutput Property,
        Property i,
        int count)
        {
            string value = "";
            var keyName = Property.ColumnName;

            switch (keyName)
            {
                case "Name":
                    var name = i.Name;
                    value = name;
                    break;
                default:
                    break;
            }
            if (Property.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((Property.ColumnType == ColumnType.Money) && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }



        protected void WriteBodyPropertyValues(
        ExcelWorksheet ws,
        int rowIndex,
        int columnIndex,
        CollumnOutput item,
        CreatePropertyValueInputExcel i,
        int count)
        {
            string value = "";
            var keyName = item.ColumnName;
            if (keyName == "PropertyName")
            {
                var name = i.PropertyName;
                value = name;
            }
            else if (keyName == "Value")
            {
                var name = i.Value;
                value = name;
            }
            else if (keyName == "NetWeight")
            {
                var gross = i.NetWeight;
                value = gross.ToString();
            }
            else if (keyName == "Code")
            {
                var code = i.Code;
                value = code;
            }
            else
            {
                var text = i.GetType().GetProperty(keyName).GetValue(i, null);
                value = text != null ? text.ToString() : "";
            }

            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {

                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }

        }

        protected void WriteBodyCustomers(
         ExcelWorksheet ws,
         int rowIndex,
         int columnIndex,
         CollumnOutput item,
         Customer i,
         int count)
        {
            string value = "";
            var keyName = item.ColumnName;

            switch (keyName)
            {
                case "Account":
                    var typeInventory = i.GetType().GetProperty("Account").GetValue(i, null);
                    value = typeInventory != null ? typeInventory.GetType().GetProperty("AccountName").GetValue(typeInventory, null).ToString() : "";
                    break;

                case "CustomerType":
                    var customerType = i.GetType().GetProperty("CustomerType").GetValue(i, null);
                    value = customerType != null ? customerType.GetType().GetProperty("CustomerTypeName").GetValue(customerType, null)?.ToString() : "";
                    break;
                case "Phone":
                    var phone = i.PhoneNumber;
                    value = phone;
                    break;
                case "BillingAddress_Street":
                    var BillingAddress_Street = i.BillingAddress.Street;
                    value = BillingAddress_Street;
                    break;
                case "BillingAddress_CityTown":
                    var BillingAddress_CityTown = i.BillingAddress.CityTown;
                    value = BillingAddress_CityTown;
                    break;
                case "BillingAddress_Province":
                    var BillingAddress_Province = i.BillingAddress.Province;
                    value = BillingAddress_Province;
                    break;
                case "BillingAddress_PostalCode":
                    var BillingAddress_PostalCode = i.BillingAddress.PostalCode;
                    value = BillingAddress_PostalCode;
                    break;
                case "BillingAddress_Country":
                    var BillingAddress_Country = i.BillingAddress.Country;
                    value = BillingAddress_Country;
                    break;
                case "ShippingAddress_Street":
                    var ShippingAddress_Street = i.ShippingAddress.Street;
                    value = ShippingAddress_Street;
                    break;
                case "ShippingAddress_CityTown":
                    var ShippingAddress_CityTown = i.ShippingAddress.CityTown;
                    value = ShippingAddress_CityTown;
                    break;
                case "ShippingAddress_Province":
                    var ShippingAddress_Province = i.ShippingAddress.Province;
                    value = ShippingAddress_Province;
                    break;
                case "ShippingAddress_PostalCode":
                    var ShippingAddress_PostalCode = i.ShippingAddress.PostalCode;
                    value = ShippingAddress_PostalCode;
                    break;
                case "ShippingAddress_Country":
                    var ShippingAddress_Country = i.ShippingAddress.Country;
                    value = ShippingAddress_Country;
                    break;
                case "IsActive":
                    value = i.IsActive ? "Active" : "Inactive";
                    break;
                default:
                    var text = i.GetType().GetProperty(keyName).GetValue(i, null);
                    value = text != null ? text?.ToString() : "";
                    break;
            }
            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money) && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }

        protected void WriteBodySubCustomers(
        ExcelWorksheet ws,
        int rowIndex,
        int columnIndex,
        CollumnOutput item,
        CreateOrUpdateCustomerContactPersonExprotInput i,
        int count)
        {
            string value = "";
            var keyName = item.ColumnName;
            if (keyName == "CustomerName")
            {
                var type = i.GetType().GetProperty("Customer").GetValue(i, null);
                var itemName = type.GetType().GetProperty("CustomerName").GetValue(type, null);
                value = itemName?.ToString();
            }
            else
            {
                var text = i.GetType().GetProperty(keyName).GetValue(i, null);
                value = text != null ? text?.ToString() : "";
            }

            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money) && !value.IsNullOrEmpty())
            {
                if (keyName == "Total")
                {
                    var costCell = GetAddressName(ws, rowIndex, columnIndex - 2);
                    var qtyCell = GetAddressName(ws, rowIndex, columnIndex - 1);
                    var formular = "SUM(" + costCell + "*" + qtyCell + ")";
                    AddFormula(ws, rowIndex, columnIndex, formular);
                }
                else
                {
                    AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
                }

            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }



        protected void WriteBodyVendors(
        ExcelWorksheet ws,
        int rowIndex,
        int columnIndex,
        CollumnOutput item,
        Vendor i,
        int count)
        {
            string value = "";
            var keyName = item.ColumnName;

            switch (keyName)
            {
                case "Account":
                    var typeInventory = i.GetType().GetProperty("ChartOfAccount").GetValue(i, null);
                    value = typeInventory != null ? typeInventory.GetType().GetProperty("AccountName").GetValue(typeInventory, null)?.ToString() : "";
                    break;
                case "Phone":
                    var phone = i.PhoneNumber;
                    value = phone;
                    break;
                case "BillingAddress_Street":
                    var BillingAddress_Street = i.BillingAddress.Street;
                    value = BillingAddress_Street;
                    break;
                case "BillingAddress_CityTown":
                    var BillingAddress_CityTown = i.BillingAddress.CityTown;
                    value = BillingAddress_CityTown;
                    break;
                case "BillingAddress_Province":
                    var BillingAddress_Province = i.BillingAddress.Province;
                    value = BillingAddress_Province;
                    break;
                case "BillingAddress_PostalCode":
                    var BillingAddress_PostalCode = i.BillingAddress.PostalCode;
                    value = BillingAddress_PostalCode;
                    break;
                case "BillingAddress_Country":
                    var BillingAddress_Country = i.BillingAddress.Country;
                    value = BillingAddress_Country;
                    break;
                case "ShippingAddress_Street":
                    var ShippingAddress_Street = i.ShippingAddress.Street;
                    value = ShippingAddress_Street;
                    break;
                case "ShippingAddress_CityTown":
                    var ShippingAddress_CityTown = i.ShippingAddress.CityTown;
                    value = ShippingAddress_CityTown;
                    break;
                case "ShippingAddress_Province":
                    var ShippingAddress_Province = i.ShippingAddress.Province;
                    value = ShippingAddress_Province;
                    break;
                case "ShippingAddress_PostalCode":
                    var ShippingAddress_PostalCode = i.ShippingAddress.PostalCode;
                    value = ShippingAddress_PostalCode;
                    break;
                case "ShippingAddress_Country":
                    var ShippingAddress_Country = i.ShippingAddress.Country;
                    value = ShippingAddress_Country;
                    break;
                case "Website":
                    var Website = i.Websit;
                    value = Website;
                    break;
                case "VendorType":
                    var vendorType = i.GetType().GetProperty("VendorType").GetValue(i, null);
                    value = vendorType != null ? vendorType.GetType().GetProperty("VendorTypeName").GetValue(vendorType, null)?.ToString() : "";
                    break;
                case "IsActive":
                    value = i.IsActive ? "Active" : "Inactive";
                    break;
                default:
                    var text = i.GetType().GetProperty(keyName).GetValue(i, null);
                    value = text != null ? text?.ToString() : "";
                    break;
            }
            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money) && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }


        protected void WriteBodyLot(
      ExcelWorksheet ws,
      int rowIndex,
      int columnIndex,
      CollumnOutput item,
      ItemLotDto i,
      int count)
        {
            string value = "";
            var keyName = item.ColumnName;

            switch (keyName)
            {
                case "Location":

                    value = i.LocationName;
                    break;
                case "zoneName":

                    value = i.LotName;
                    break;
            }
            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money) && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }


        protected void WriteBodySubVendors(
        ExcelWorksheet ws,
        int rowIndex,
        int columnIndex,
        CollumnOutput item,
        CreateOrUpdateContactPersonExcelInput i,
        int count)
        {
            string value = "";
            var keyName = item.ColumnName;
            if (keyName == "VendorName")
            {
                var type = i.GetType().GetProperty("Vendors").GetValue(i, null);
                var itemName = type.GetType().GetProperty("VendorName").GetValue(type, null);
                value = itemName?.ToString();
            }
            else
            {
                var text = i.GetType().GetProperty(keyName).GetValue(i, null);
                value = text != null ? text?.ToString() : "";
            }

            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money) && !value.IsNullOrEmpty())
            {
                if (keyName == "Total")
                {
                    var costCell = GetAddressName(ws, rowIndex, columnIndex - 2);
                    var qtyCell = GetAddressName(ws, rowIndex, columnIndex - 1);
                    var formular = "SUM(" + costCell + "*" + qtyCell + ")";
                    AddFormula(ws, rowIndex, columnIndex, formular);
                }
                else
                {
                    AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
                }

            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }


        //protected void WriteBodySubItems(
        // ExcelWorksheet ws,
        // int rowIndex,
        // int columnIndex,
        // CollumnOutput item,
        // CreateSubItemInputExportExcel i,
        // int count)
        //{
        //    string value = "";
        //    var keyName = item.ColumnName;
        //    if (keyName == "ItemName")
        //    {
        //        var type = i.GetType().GetProperty("Item").GetValue(i, null);
        //        var itemName = type.GetType().GetProperty("ItemName").GetValue(type, null);
        //        value = itemName?.ToString();
        //    }
        //    else if (keyName == "ItemCode")
        //    {
        //        var type = i.GetType().GetProperty("Item").GetValue(i, null);
        //        var itemCode = type.GetType().GetProperty("ItemCode").GetValue(type, null);
        //        value = itemCode?.ToString();
        //    }

        //    else if (keyName == "ParentItem")
        //    {
        //        var type = i.GetType().GetProperty("ParentItem").GetValue(i, null);
        //        var parentName = type.GetType().GetProperty("ItemName").GetValue(type, null);
        //        value = parentName?.ToString();
        //    }
        //    else
        //    {
        //        var text = i.GetType().GetProperty(keyName).GetValue(i, null);
        //        value = text != null ? text.ToString() : "";
        //    }

        //    if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
        //    {
        //        AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
        //    }
        //    else if ((item.ColumnType == ColumnType.Money) && !value.IsNullOrEmpty())
        //    {
        //        if (keyName == "Total")
        //        {
        //            var costCell = GetAddressName(ws, rowIndex, columnIndex - 2);
        //            var qtyCell = GetAddressName(ws, rowIndex, columnIndex - 1);
        //            var formular = "SUM(" + costCell + "*" + qtyCell + ")";
        //            AddFormula(ws, rowIndex, columnIndex, formular);
        //        }
        //        else
        //        {
        //            AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
        //        }

        //    }
        //    else
        //    {
        //        AddTextToCell(ws, rowIndex, columnIndex, value);
        //    }
        //}

        protected void WriteBodyInventoryValuationSummary(
            ExcelWorksheet ws,
            int rowIndex,
            int columnIndex,
            CollumnOutput item,
            GetListInventoryReportOutputNew i,
            int count)
        {
            string value = "";
            var keyName = item.ColumnName;
            var p = i.PropertySummary.Where(t => t.PropertyName == keyName).FirstOrDefault();

            if (keyName == "Account")
            {
                //var code = i.GetType().GetProperty("InventoryAccountCode").GetValue(i, null);
                //var name = i.GetType().GetProperty("InventoryAccountName").GetValue(i, null);
                var code = i.InventoryAccountCode;
                var name = i.InventoryAccountName;
                value = code + " - " + name;
            }
            else if (p != null)
            {
                value = p.Value;
            }
            else if (keyName == "Location")
            {
                value = i.LocationName; // i.GetType().GetProperty("LocationName").GetValue(i, null)?.ToString();
            }
            else if (keyName == "Item")
            {
                //var code = i.GetType().GetProperty("ItemCode").GetValue(i, null);
                //var name = i.GetType().GetProperty("ItemName").GetValue(i, null);
                var code =  i.ItemCode != null ? i.ItemCode : "";
                var name = i.ItemName != null ? i.ItemName : ""; 

                value = code + " - " + name;
            }
            else if (keyName == "No")
            {
                value = count.ToString();
            }
            else if (keyName == "Unit")
            {
                value = i.Unit != null ? i.Unit.Value : "";
            }
            else
            {
                var text = i.GetType().GetProperty(keyName)?.GetValue(i, null);
                value = text != null ? text?.ToString() : "";
            }

            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money) && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }

        protected void WriteBodyStockValuationSummary(
           ExcelWorksheet ws,
           int rowIndex,
           int columnIndex,
           CollumnOutput item,
           GetListStockBalanceReportOutputNew i,
           int count)
        {
            string value = "";
            var keyName = item.ColumnName;

            var p = i.Properties.Where(t => t.PropertyName == keyName).FirstOrDefault();

            if (keyName == "Account")
            {
                var code = i.GetType().GetProperty("InventoryAccountCode").GetValue(i, null);
                var name = i.GetType().GetProperty("InventoryAccountName").GetValue(i, null);
                value = code + " - " + name;
            }
            else if (p != null)
            {
                value = p.Value;
                //foreach (var pro in i.Properties)
                //{
                //value = pro.Value;
                //    AddTextToCell(ws, rowIndex, columnIndex, value);
                //}
                //AddTextToCell(ws, rowIndex, columnIndex, p.Value);
            }
            else if (keyName == "Location")
            {
                value = i.GetType().GetProperty("LocationName").GetValue(i, null)?.ToString();
            }
            else if (keyName == "Lot")
            {
                value = i.GetType().GetProperty("LotName").GetValue(i, null)?.ToString();
            }
            else if (keyName == "Item")
            {
                var code = i.GetType().GetProperty("ItemCode").GetValue(i, null);
                var name = i.GetType().GetProperty("ItemName").GetValue(i, null);
                value = code + " - " + name;
            }
            else if (keyName == "No")
            {
                value = count.ToString();
            }
            else if (keyName == "Unit")
            {
                value = i.Unit?.Value;
            }
            else
            {
                var text = i.GetType().GetProperty(keyName)?.GetValue(i, null);
                value = text != null ? text?.ToString() : "";


            }

            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money) && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }

        protected void WriteBodyAssetBalance(
          ExcelWorksheet ws,
          int rowIndex,
          int columnIndex,
          CollumnOutput item,
          GetListAssetBalanceReportOutput i,
          int count)
        {
            string value = "";
            var keyName = item.ColumnName;

            var p = i.Properties.Where(t => t.PropertyName == keyName).FirstOrDefault();

            if (keyName == "Account")
            {
                var code = i.GetType().GetProperty("InventoryAccountCode").GetValue(i, null);
                var name = i.GetType().GetProperty("InventoryAccountName").GetValue(i, null);
                value = code + " - " + name;
            }
            else if (p != null)
            {
                value = p.Value;
                //foreach (var pro in i.Properties)
                //{
                //value = pro.Value;
                //    AddTextToCell(ws, rowIndex, columnIndex, value);
                //}
                //AddTextToCell(ws, rowIndex, columnIndex, p.Value);
            }
            else if (keyName == "Location")
            {
                value = i.GetType().GetProperty("LocationName").GetValue(i, null)?.ToString();
            }
            else if (keyName == "Lot")
            {
                value = i.GetType().GetProperty("LotName").GetValue(i, null)?.ToString();
            }
            else if (keyName == "Item")
            {
                var code = i.GetType().GetProperty("ItemCode").GetValue(i, null);
                var name = i.GetType().GetProperty("ItemName").GetValue(i, null);
                value = code + " - " + name;
            }
            else if (keyName == "No")
            {
                value = count.ToString();
            }
            else
            {
                var text = i.GetType().GetProperty(keyName)?.GetValue(i, null);
                value = text != null ? text?.ToString() : "";

            }

            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money) && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }


        protected void WriteBodyTransactionValuationSummary(
          ExcelWorksheet ws,
          int rowIndex,
          int columnIndex,
          CollumnOutput item,
          GetListInventoryTransationReportOutput i,
          int count)

        {
            string value = "";
            var keyName = item.ColumnName;
            var p = i.Properties.Where(t => t.PropertyName == keyName).FirstOrDefault();
            bool wrapText = false;

            switch (keyName)
            {
                case "Location":
                    value = i.GetType().GetProperty("LocationName").GetValue(i, null)?.ToString();
                    break;
                case "JournalTransactionType":
                    value = i.JournalTransactionTypeName;
                    break;
                case "Lot":
                    value = i.GetType().GetProperty("LotName").GetValue(i, null)?.ToString();
                    break;
                case "Item":
                    var code = i.GetType().GetProperty("ItemCode").GetValue(i, null);
                    var name = i.GetType().GetProperty("ItemName").GetValue(i, null);
                    value = code + " - " + name;
                    break;
                case "No":
                    value = count.ToString();
                    break;
                case "BatchSerial":
                    value = i.ItemBatchNos == null || !i.ItemBatchNos.Any() ? "" : string.Join(", ", i.ItemBatchNos.Select(s => $"{s.BatchNumber}" + (s.Qty == 1 ? "" : $"({ s.Qty})")).ToList());
                    wrapText = true;
                    break;
                case "ItemBatchSerial":
                    value = i.ItemName;
                    value += i.ItemBatchNos == null || !i.ItemBatchNos.Any() ? "" : "\r\n" + string.Join(", ", i.ItemBatchNos.Select(s => $"{s.BatchNumber}" + (s.Qty == 1 ? "" : $"({s.Qty})")).ToList());
                    wrapText = true;
                    break;
                default:
                    if (p != null)
                    {
                        if (p.ValueId != null)
                        {
                            value = p.Value;
                        }
                        else
                        {
                            value = null;
                        }
                    }
                    else
                    {
                        var text = i.GetType().GetProperty(keyName)?.GetValue(i, null);
                        value = text != null ? text?.ToString() : "";
                    }
                    break;
            }

            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if (item.ColumnType == ColumnType.Date && !value.IsNullOrEmpty())
            {
                AddDateToCell(ws, rowIndex, columnIndex, DateTime.Parse(value));
            }
            else if ((item.ColumnType == ColumnType.Money) && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value, false, 0, false, wrapText);
            }
        }



        protected void WriteBodyInventoryValuationDetail(
            ExcelWorksheet ws,
            int rowIndex,
            int columnIndex,
            CollumnOutput item,
            InventoryValuationDetailItem i,
            List<ItemPropertySummary> properties,
            int count)
        {
            string value = "";
            var keyName = item.ColumnName;
            var p = properties != null ? properties.Where(t => t.PropertyName == keyName).FirstOrDefault() : null;

            if (keyName == "JournalType")
            {
                var lable = "";
                switch (i.JournalType)
                {
                    case enumStatus.EnumStatus.JournalType.ItemIssueSale:
                        lable = L("Sale");
                        break;
                    case enumStatus.EnumStatus.JournalType.ItemReceiptPurchase:
                        lable = L("Purchase");
                        break;
                    case enumStatus.EnumStatus.JournalType.ItemReceiptTransfer:
                        lable = L("Transfer");
                        break;
                    case enumStatus.EnumStatus.JournalType.ItemIssueTransfer:
                        lable = L("Transfer");
                        break;
                    case enumStatus.EnumStatus.JournalType.ItemIssueAdjustment:
                        lable = L("Adjustment");
                        break;
                    case enumStatus.EnumStatus.JournalType.ItemReceiptAdjustment:
                        lable = L("Adjustment");
                        break;
                    case enumStatus.EnumStatus.JournalType.ItemIssueOther:
                        lable = L("Other");
                        break;
                    case enumStatus.EnumStatus.JournalType.ItemReceiptOther:
                        lable = L("Other");
                        break;
                    case enumStatus.EnumStatus.JournalType.ItemIssuePhysicalCount:
                        lable = L("PhysicalCount");
                        break;
                    case enumStatus.EnumStatus.JournalType.ItemReceiptPhysicalCount:
                        lable = L("PhysicalCount");
                        break;
                    case enumStatus.EnumStatus.JournalType.ItemReceiptProduction:
                        lable = L("Production");
                        break;
                    case enumStatus.EnumStatus.JournalType.ItemIssueProduction:
                        lable = L("Production");
                        break;
                    case enumStatus.EnumStatus.JournalType.ItemReceiptCustomerCredit:
                        lable = L("CustomerCredit");
                        break;
                    case enumStatus.EnumStatus.JournalType.ItemIssueVendorCredit:
                        lable = L("VendorCredit");
                        break;
                    default:
                        lable = L("Begining");
                        break;
                }
                var jType = i.JournalType != null ? L(i.JournalType?.ToString()) + " (" + lable + ")" : lable;
                value = jType;
            }
            else if (keyName == "Item")
            {
                var code = i.GetType().GetProperty("ItemCode").GetValue(i, null);
                var name = i.GetType().GetProperty("ItemName").GetValue(i, null);
                value = code + " - " + name;
            }
            else if (keyName == "Location")
            {
                value = i.LocationName;
            }
            else if (p != null)
            {
                value = p.Value;
            }
            else if (keyName == "No")
            {
                value = count.ToString();
            }
            else
            {
                var text = i.GetType().GetProperty(keyName)?.GetValue(i, null);
                value = text != null ? text?.ToString() : "";
            }

            if (keyName == "AVGCost" || keyName == "UnitCost")
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true, false, i.RoundingDigitUnitCost);
            }
            else if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money || item.ColumnType == ColumnType.RoundingDigit) && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true, false, i.RoundingDigit);
            }
            else if ((item.ColumnType == ColumnType.Date) && !value.IsNullOrEmpty())
            {
                AddDateToCell(ws, rowIndex, columnIndex, DateTime.Parse(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }

        protected void WriteBodyBatchNoBalance(
           ExcelWorksheet ws,
           int rowIndex,
           int columnIndex,
           CollumnOutput item,
           BatchNoBalanceOutput i,
           int count)
        {
            string value = "";
            var keyName = item.ColumnName;

            var p = i.Properties.Where(t => t.PropertyName == keyName).FirstOrDefault();

            if (p != null)
            {
                value = p.Value;
            }
            else if (keyName == "Location")
            {
                value = i.GetType().GetProperty("LocationName").GetValue(i, null)?.ToString();
            }
            else if (keyName == "Lot")
            {
                value = i.GetType().GetProperty("LotName").GetValue(i, null)?.ToString();
            }
            else if (keyName == "Item")
            {
                var code = i.GetType().GetProperty("ItemCode").GetValue(i, null);
                var name = i.GetType().GetProperty("ItemName").GetValue(i, null);
                value = code + " - " + name;
            }
            else if (keyName == "No")
            {
                value = count.ToString();
            }
            else if (keyName == "Unit")
            {
                value = i.Unit;
            }
            else
            {
                var text = i.GetType().GetProperty(keyName)?.GetValue(i, null);
                value = text != null ? text?.ToString() : "";
            }

            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money) && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }

        protected void WriteBodyProduction(
           ExcelWorksheet ws,
           int rowIndex,
           int columnIndex,
           CollumnOutput item,
           ProductionPlanDetailOutput i,
           int count)
        {
            string value = "";
            var keyName = item.ColumnName;

            if (keyName == "Location")
            {
                value = i.LocationName;
            }
            else if (keyName == "ProductionLine")
            {
                value = i.ProductionLineName;
            }
            else if (keyName == "User")
            {
                value = i.UserName;
            }
            else if (keyName == "No")
            {
                value = count.ToString();
            }
            else if (keyName == "StandardCostGroups")
            {
                value = string.Join(Environment.NewLine, i.StandardCostGroups.Select(s => $"{s.GroupName}: {s.TotalQty:0.00} = {s.QtyPercentage:0.00}%"));
            }
            else if (keyName == "StandardCostGroupsKg")
            {
                value = string.Join(Environment.NewLine, i.StandardCostGroups.Select(s => $"{s.GroupName}: {s.TotalNetWeight:0.00}Kg = {s.NetWeightPercentage:0.00}%"));
            }
            else if (keyName == "IssueStandardCostGroups")
            {
                value = string.Join(Environment.NewLine, i.IssueStandardCostGroups.Select(s => $"{s.GroupName}: {s.TotalQty:0.00} = {s.QtyPercentage:0.00}%"));
            }
            else if (keyName == "IssueStandardCostGroupsKg")
            {
                value = string.Join(Environment.NewLine, i.IssueStandardCostGroups.Select(s => $"{s.GroupName}: {s.TotalNetWeight:0.00}Kg = {s.NetWeightPercentage:0.00}%"));
            }
            else
            {
                var text = i.GetType().GetProperty(keyName)?.GetValue(i, null);
                value = text != null ? text?.ToString() : "";
            }

            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if (item.ColumnType == ColumnType.Percentage)
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, false, true);
            }
            else if ((item.ColumnType == ColumnType.Money) && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
            }
            else if ((item.ColumnType == ColumnType.Date) && !value.IsNullOrEmpty())
            {
                AddDateToCell(ws, rowIndex, columnIndex, DateTime.Parse(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }

            if (keyName == "StandardCostGroups" || 
                keyName == "StandardCostGroups" || 
                keyName == "IssueStandardCostGroups" || 
                keyName == "IssueStandardCostGroupsKg")
            {
                ws.Cells[rowIndex, columnIndex].Style.WrapText = true;
            }
        }

        protected void WriteBodyProductionOrder(
           ExcelWorksheet ws,
           int rowIndex,
           int columnIndex,
           CollumnOutput item,
           ProductionOrderReportOutput i,
           int count)
        {
            string value = "";
            var keyName = item.ColumnName;

            if (keyName == "Status")
            {
                value = i.Status.ToString();
            }
            else if (keyName == "")
            {
                value = i.ReceiveStatus.ToString();
            }
            else if (keyName == "No")
            {
                value = count.ToString();
            }
            else if (keyName == "StandardGroups")
            {
                value = string.Join(Environment.NewLine, i.StandardGroups.Select(s => $"{s.GroupName}: {s.TotalQty:0.00}"));
            }
            else if (keyName == "StandardGroupsKg")
            {
                value = string.Join(Environment.NewLine, i.StandardGroups.Select(s => $"{s.GroupName}: {s.TotalNetWeight:0.00}Kg"));
            }
            else if (keyName == "IssueStandardGroups")
            {
                value = string.Join(Environment.NewLine, i.IssueStandardGroups.Select(s => $"{s.GroupName}: {s.TotalQty:0.00}"));
            }
            else if (keyName == "IssueStandardGroupsKg")
            {
                value = string.Join(Environment.NewLine, i.IssueStandardGroups.Select(s => $"{s.GroupName}: {s.TotalNetWeight:0.00}Kg"));
            }
            else
            {
                var text = i.GetType().GetProperty(keyName)?.GetValue(i, null);
                value = text != null ? text?.ToString() : "";
            }

            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if (item.ColumnType == ColumnType.Percentage)
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, false, true);
            }
            else if ((item.ColumnType == ColumnType.Money) && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
            }
            else if ((item.ColumnType == ColumnType.Date) && !value.IsNullOrEmpty())
            {
                AddDateToCell(ws, rowIndex, columnIndex, DateTime.Parse(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }

            if (keyName == "IssueStandardGroups" || 
                keyName == "IssueStandardGroupsKg" ||
                keyName == "StandardGroups" ||
                keyName == "StandardGroupsKg")
            {
                ws.Cells[rowIndex, columnIndex].Style.WrapText = true;
            }
        }


        protected void WriteBodyBill(
           ExcelWorksheet ws,
           int rowIndex,
           int columnIndex,
           CollumnOutput item,
           GetListReportBillOutput i,
           int count)
        {
            string value = "";
            var keyName = item.ColumnName;
            switch (keyName)
            {
                case "Account":
                    var code = i.GetType().GetProperty("AccountCode").GetValue(i, null);
                    var name = i.GetType().GetProperty("AccountName").GetValue(i, null);
                    value = code + " - " + name;
                    break;
                case "Location":
                    value = i.GetType().GetProperty("LocationName").GetValue(i, null)?.ToString();
                    break;
                case "Tax":
                    value = i.GetType().GetProperty("TaxName").GetValue(i, null)?.ToString();
                    break;
                case "Vendor":
                    value = i.GetType().GetProperty("VendorName").GetValue(i, null)?.ToString();
                    break;
                case "Item":
                    var itemCode = i.GetType().GetProperty("ItemCode").GetValue(i, null);
                    var itemName = i.GetType().GetProperty("ItemName").GetValue(i, null);
                    if (itemCode != null)
                    {
                        value = itemCode + " - " + itemName;
                    }
                    else
                    {
                        value = "";
                    }

                    break;
                default:
                    var text = i.GetType().GetProperty(keyName)?.GetValue(i, null);
                    value = text != null ? text?.ToString() : "";

                    var p = i.Properties.Where(t => t.PropertyName == keyName).FirstOrDefault();
                    if (p != null) value = p.Value;

                    break;
            }
            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money || item.ColumnType == ColumnType.RoundingDigit) && !value.IsNullOrEmpty())
            {
                var digit = item.ColumnName == "UnitCost" ? i.RoundingDigitUnitCost : i.RoundingDigit;
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true, false, digit);
            }
            else if (item.ColumnType == ColumnType.Date && !value.IsNullOrEmpty())
            {
                AddDateToCell(ws, rowIndex, columnIndex, DateTime.Parse(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }

        protected void WriteBodyQCTest(
           ExcelWorksheet ws,
           int rowIndex,
           int columnIndex,
           CollumnOutput item,
           GetListQCTestReportOutput i,
           int count)
        {
            string value = "";
            var keyName = item.ColumnName;
            switch (keyName)
            {
                case "Location":
                    value = i.Location;
                    break;
                case "Item":
                    value = i.ItemName;
                    break;
                case "TestParameter":
                    value = i.TestResultDetails.IsNullOrEmpty() ? "" : string.Join(Environment.NewLine, i.TestResultDetails.Select(s => $"{s.TestParameterName}"));
                    break;
                case "ActualValue":
                    if (i.DetailEntry)
                    {
                        value = i.TestResultDetails.IsNullOrEmpty() ? "" : string.Join(Environment.NewLine, i.TestResultDetails.Select(s => $"{s.ActualValue}"));
                    }
                    else
                    {
                        value = i.FinalPassFail ? L("Pass") : L("Fail");
                    }
                    break;

                default:
                    var text = i.GetType().GetProperty(keyName)?.GetValue(i, null);
                    value = text != null ? text?.ToString() : "";

                    break;
            }
            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if (item.ColumnType == ColumnType.Date && !value.IsNullOrEmpty())
            {
                AddDateToCell(ws, rowIndex, columnIndex, DateTime.Parse(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }


        protected void WriteBodyPurchaseOrderSummary(
           ExcelWorksheet ws,
           int rowIndex,
           int columnIndex,
           CollumnOutput item,
           GetListReportPurchaseOrderSummaryOutput i,
           int count)
        {
            string value = "";
            var keyName = item.ColumnName;
            switch (keyName)
            {
                case "Location":
                    value = i.GetType().GetProperty("LocationName").GetValue(i, null)?.ToString();
                    break;
                case "Vendor":
                    value = i.GetType().GetProperty("VendorName").GetValue(i, null)?.ToString();
                    break;
                case "Item":
                    var itemList = "";
                    var j = 0;
                    foreach (var oi in i.Items)
                    {
                        if (j > 0) itemList += Environment.NewLine;
                        itemList += $"{oi.ItemName}";
                        j++;
                    }
                    value = itemList;
                    break;
                case "Qty":
                    var qtyList = "";
                    var q = 0;
                    foreach (var oi in i.Items)
                    {
                        if (q > 0) qtyList += Environment.NewLine;
                        qtyList += $"{AutoRound(oi.OrderQty)} - {AutoRound(oi.ReceiveQty)} = {AutoRound(oi.OrderQty - oi.ReceiveQty)}";
                        q++;
                    }
                    value = qtyList;
                    break;
                case "NetWeight":
                    var nList = "";
                    var n = 0;
                    foreach (var oi in i.Items)
                    {
                        if (n > 0) nList += Environment.NewLine;
                        nList += $"{AutoRound(oi.OrderNetWeight)} - {AutoRound(oi.ReceiveNetWeight)} = {AutoRound(oi.OrderNetWeight - oi.ReceiveNetWeight)}";
                        n++;
                    }
                    value = nList;
                    break;
                default:
                    var text = i.GetType().GetProperty(keyName)?.GetValue(i, null);
                    value = text != null ? text?.ToString() : "";
                    break;
            }
            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money || item.ColumnType == ColumnType.RoundingDigit) && !value.IsNullOrEmpty())
            {
                var digit = i.RoundingDigit;
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true, false, digit);
            }
            else if (item.ColumnType == ColumnType.Date && !value.IsNullOrEmpty())
            {
                AddDateToCell(ws, rowIndex, columnIndex, DateTime.Parse(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }

        protected void WriteBodySaleOrderSummary(
          ExcelWorksheet ws,
          int rowIndex,
          int columnIndex,
          CollumnOutput item,
          GetListReportSaleOrderSummaryOutput i,
          int count)
        {
            string value = "";
            var keyName = item.ColumnName;
            switch (keyName)
            {
                case "Location":
                    value = i.GetType().GetProperty("LocationName").GetValue(i, null)?.ToString();
                    break;
                case "Customer":
                    value = i.GetType().GetProperty("CustomerName").GetValue(i, null)?.ToString();
                    break;
                case "Item":
                    var itemList = "";
                    var j = 0;
                    foreach (var oi in i.Items)
                    {
                        if (j > 0) itemList += Environment.NewLine;
                        itemList += $"{oi.ItemName}";
                        j++;
                    }
                    value = itemList;
                    break;
                case "Qty":
                    var qtyList = "";
                    var q = 0;
                    foreach (var oi in i.Items)
                    {
                        if (q > 0) qtyList += Environment.NewLine;
                        qtyList += $"{AutoRound(oi.OrderQty)} - {AutoRound(oi.IssueQty)} = {AutoRound(oi.OrderQty - oi.IssueQty)}";
                        q++;
                    }
                    value = qtyList;
                    break;
                case "NetWeight":
                    var nList = "";
                    var n = 0;
                    foreach (var oi in i.Items)
                    {
                        if (n > 0) nList += Environment.NewLine;
                        nList += $"{AutoRound(oi.OrderNetWeight)} - {AutoRound(oi.IssueNetWeight)} = {AutoRound(oi.OrderNetWeight - oi.IssueNetWeight)}";
                        n++;
                    }
                    value = nList;
                    break;
                case "Status":
                    value = i.StatusName;
                    break;
                default:
                    var text = i.GetType().GetProperty(keyName)?.GetValue(i, null);
                    value = text != null ? text?.ToString() : "";
                    break;
            }
            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money || item.ColumnType == ColumnType.RoundingDigit) && !value.IsNullOrEmpty())
            {
                var digit = i.RoundingDigit;
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true, false, digit);
            }
            else if (item.ColumnType == ColumnType.Date && !value.IsNullOrEmpty())
            {
                AddDateToCell(ws, rowIndex, columnIndex, DateTime.Parse(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }

        protected void WriteBodySaleOrderDetail(
          ExcelWorksheet ws,
          int rowIndex,
          int columnIndex,
          CollumnOutput item,
          GetListReportSaleOrderDetailOutput i,
          int count)
        {
            string value = "";
            var keyName = item.ColumnName;
            switch (keyName)
            {
                case "Location":
                    value = i.GetType().GetProperty("LocationName").GetValue(i, null)?.ToString();
                    break;
                case "Customer":
                    value = i.GetType().GetProperty("CustomerName").GetValue(i, null)?.ToString();
                    break;
                case "Item":
                    value = i.ItemName;
                    break;
                case "Status":
                    value = i.StatusName;
                    break;
                default:
                    var text = i.GetType().GetProperty(keyName)?.GetValue(i, null);
                    value = text != null ? text.ToString() : "";

                    var p = i.Properties.Where(t => t.PropertyName == keyName).FirstOrDefault();
                    if (p != null) value = p.Value;

                    break;

            }
            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money || item.ColumnType == ColumnType.RoundingDigit) && !value.IsNullOrEmpty())
            {
                var digit = i.RoundingDigit;
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true, false, digit);
            }
            else if (item.ColumnType == ColumnType.Date && !value.IsNullOrEmpty())
            {
                AddDateToCell(ws, rowIndex, columnIndex, DateTime.Parse(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }

        protected void WriteBodySaleOrderByItemProperty(
         ExcelWorksheet ws,
         int rowIndex,
         int columnIndex,
         CollumnOutput item,
         GetListReportSaleOrderByItemPropertyOutput i,
         int count)
        {
            var text = i.GetType().GetProperty(item.ColumnName)?.GetValue(i, null);
            var value = text != null ? text.ToString() : "";

            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money || item.ColumnType == ColumnType.RoundingDigit) && !value.IsNullOrEmpty())
            {
                var digit = i.RoundingDigit;
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true, false, digit);
            }
            else if (item.ColumnType == ColumnType.Date && !value.IsNullOrEmpty())
            {
                AddDateToCell(ws, rowIndex, columnIndex, DateTime.Parse(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }

        protected void WriteBodySaleInvoiceByItemProperty(
         ExcelWorksheet ws,
         int rowIndex,
         int columnIndex,
         CollumnOutput item,
         GetListSaleInvoiceByItemPropertyReportOutput i,
         int count)
        {
            var value = "";

            switch (item.ColumnName)
            {   
                case "Location":
                    value = i.LocationName;
                    break;
                case "Customer":
                    value = i.CustomerName;
                    break;
                default:
                    var text = i.GetType().GetProperty(item.ColumnName)?.GetValue(i, null);
                    value = text != null ? text.ToString() : "";
                    break;
            }

            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money || item.ColumnType == ColumnType.RoundingDigit) && !value.IsNullOrEmpty())
            {
                var digit = i.RoundingDigit;
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true, false, digit);
            }
            else if (item.ColumnType == ColumnType.Date && !value.IsNullOrEmpty())
            {
                AddDateToCell(ws, rowIndex, columnIndex, DateTime.Parse(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }

        protected void WriteBodyPurchaseByItemProperty(
         ExcelWorksheet ws,
         int rowIndex,
         int columnIndex,
         CollumnOutput item,
         GetListPurchaseByItemPropertyReportOutput i,
         int count)
        {
            var value = "";

            switch (item.ColumnName)
            {
                case "Location":
                    value = i.LocationName;
                    break;
                case "Vendor":
                    value = i.VendorName;
                    break;
                default:
                    var text = i.GetType().GetProperty(item.ColumnName)?.GetValue(i, null);
                    value = text != null ? text.ToString() : "";
                    break;
            }

            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money || item.ColumnType == ColumnType.RoundingDigit) && !value.IsNullOrEmpty())
            {
                var digit = i.RoundingDigit;
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true, false, digit);
            }
            else if (item.ColumnType == ColumnType.Date && !value.IsNullOrEmpty())
            {
                AddDateToCell(ws, rowIndex, columnIndex, DateTime.Parse(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }


        protected void WriteBodyDeliveryScheduleByItemProperty(
        ExcelWorksheet ws,
        int rowIndex,
        int columnIndex,
        CollumnOutput item,
        GetListReportDeliveryScheduleByItemPropertyOutput i,
        int count)
        {
            var text = i.GetType().GetProperty(item.ColumnName)?.GetValue(i, null);
            var value = text != null ? text.ToString() : "";

            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money || item.ColumnType == ColumnType.RoundingDigit) && !value.IsNullOrEmpty())
            {
                var digit = i.RoundingDigit;
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true, false, digit);
            }
            else if (item.ColumnType == ColumnType.Date && !value.IsNullOrEmpty())
            {
                AddDateToCell(ws, rowIndex, columnIndex, DateTime.Parse(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }


        protected void WriteBodyDeliveryScheduleDetail(
         ExcelWorksheet ws,
         int rowIndex,
         int columnIndex,
         CollumnOutput item,
         GetListReportDeliveryScheduleDetailOutput i,
         int count)
        {
            string value = "";
            var keyName = item.ColumnName;
            switch (keyName)
            {
                case "Location":
                    value = i.GetType().GetProperty("LocationName").GetValue(i, null)?.ToString();
                    break;
                case "Customer":
                    value = i.GetType().GetProperty("CustomerName").GetValue(i, null)?.ToString();
                    break;
                case "Item":
                    value = i.ItemName;
                    break;
                case "DeliveryRef":
                    value = i.DeliveryReference;
                    break;
                case "OrderRef":
                    value = i.OrderReference;
                    break;
                case "Status":
                    value = i.StatusName;
                    break;
                default:
                    var text = i.GetType().GetProperty(keyName)?.GetValue(i, null);
                    value = text != null ? text.ToString() : "";

                    var p = i.Properties.Where(t => t.PropertyName == keyName).FirstOrDefault();
                    if (p != null) value = p.Value;

                    break;

            }
            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money || item.ColumnType == ColumnType.RoundingDigit) && !value.IsNullOrEmpty())
            {
                var digit = i.RoundingDigit;
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true, false, digit);
            }
            else if (item.ColumnType == ColumnType.Date && !value.IsNullOrEmpty())
            {
                AddDateToCell(ws, rowIndex, columnIndex, DateTime.Parse(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }

        protected void WriteBodyDeliveryScheduleSummary(
         ExcelWorksheet ws,
         int rowIndex,
         int columnIndex,
         CollumnOutput item,
         GetListReportDeliveryScheduleSummaryOutput i,
         int count)
        {
            string value = "";
            var keyName = item.ColumnName;
            switch (keyName)
            {
                case "Location":
                    value = i.GetType().GetProperty("LocationName").GetValue(i, null)?.ToString();
                    break;
                case "Customer":
                    value = i.GetType().GetProperty("CustomerName").GetValue(i, null)?.ToString();
                    break;
                case "Item":
                    var itemList = "";
                    var j = 0;
                    foreach (var oi in i.Items)
                    {
                        if (j > 0) itemList += Environment.NewLine;
                        itemList += $"{oi.ItemName}";
                        j++;
                    }
                    value = itemList;
                    break;
                case "Qty":
                    var qtyList = "";
                    var q = 0;
                    foreach (var oi in i.Items)
                    {
                        if (q > 0) qtyList += Environment.NewLine;
                        qtyList += $"{AutoRound(oi.DeliveryQty)} - {AutoRound(oi.IssueQty)} = {AutoRound(oi.DeliveryQty - oi.IssueQty)}";
                        q++;
                    }
                    value = qtyList;
                    break;
                case "NetWeight":
                    var nList = "";
                    var n = 0;
                    foreach (var oi in i.Items)
                    {
                        if (n > 0) nList += Environment.NewLine;
                        nList += $"{AutoRound(oi.DeliveryNetWeight)} - {AutoRound(oi.IssueNetWeight)} = {AutoRound(oi.DeliveryNetWeight - oi.IssueNetWeight)}";
                        n++;
                    }
                    value = nList;
                    break;
                case "Status":
                    value = i.StatusName;
                    break;
                case "Reference":
                    value = i.DeliveryRef;
                    break;
                default:
                    var text = i.GetType().GetProperty(keyName)?.GetValue(i, null);
                    value = text != null ? text?.ToString() : "";
                    break;
            }
            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money || item.ColumnType == ColumnType.RoundingDigit) && !value.IsNullOrEmpty())
            {
                var digit = i.RoundingDigit;
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true, false, digit);
            }
            else if (item.ColumnType == ColumnType.Date && !value.IsNullOrEmpty())
            {
                AddDateToCell(ws, rowIndex, columnIndex, DateTime.Parse(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }

        protected void WriteBodyProfitByInvoice(
          ExcelWorksheet ws,
          int rowIndex,
          int columnIndex,
          CollumnOutput item,
          GetListReportProfitByInvoiceOutput i,
          int count)
        {
            string value = "";
            var keyName = item.ColumnName;
            switch (keyName)
            {
                case "Location":
                    value = i.GetType().GetProperty("LocationName").GetValue(i, null)?.ToString();
                    break;
                case "Customer":
                    value = i.GetType().GetProperty("CustomerName").GetValue(i, null)?.ToString();
                    break;
                case "Item":
                    value = i.ItemName;
                    break;
                case "Status":
                    value = i.StatusName;
                    break;
                default:
                    var text = i.GetType().GetProperty(keyName)?.GetValue(i, null);
                    value = text != null ? text.ToString() : "";

                    var p = i.Properties.Where(t => t.PropertyName == keyName).FirstOrDefault();
                    if (p != null) value = p.Value;

                    break;

            }
            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money || item.ColumnType == ColumnType.RoundingDigit) && !value.IsNullOrEmpty())
            {
                var digit = i.RoundingDigit;
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true, false, digit);
            }
            else if (item.ColumnType == ColumnType.Percentage)
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, false, true);
            }
            else if (item.ColumnType == ColumnType.Date && !value.IsNullOrEmpty())
            {
                AddDateToCell(ws, rowIndex, columnIndex, DateTime.Parse(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }


        // Helper General write body
        protected void WriteBodyExcel(
          ExcelWorksheet ws,
          int rowIndex,
          int columnIndex,
          CollumnOutput item,
          ExcelToExportInput i,
          int count)
        {
            var text = i.GetType().GetProperty(item.ColumnName)?.GetValue(i, null);
            var value = text != null ? text.ToString() : "";

            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money) && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value));
            }
            else if (item.ColumnType == ColumnType.Date && !value.IsNullOrEmpty())
            {
                AddDateToCell(ws, rowIndex, columnIndex, DateTime.Parse(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }

        protected void WriteBodySaleInvoice(
          ExcelWorksheet ws,
          int rowIndex,
          int columnIndex,
          CollumnOutput item,
          GetListSaleInvoiceReportOutput i,
          int count)
        {
            string value = "";
            var keyName = item.ColumnName;
            switch (keyName)
            {
                case "Account":
                    var code = i.GetType().GetProperty("AccountCode").GetValue(i, null);
                    var name = i.GetType().GetProperty("AccountName").GetValue(i, null);
                    value = code + " - " + name;
                    break;
                case "Location":
                    value = i.GetType().GetProperty("LocationName").GetValue(i, null)?.ToString();
                    break;
                case "Tax":
                    value = i.GetType().GetProperty("TaxName").GetValue(i, null)?.ToString();
                    break;
                case "Customer":
                    value = i.GetType().GetProperty("CustomerName").GetValue(i, null)?.ToString();
                    break;
                case "Item":
                    var itemCode = i.GetType().GetProperty("ItemCode").GetValue(i, null);
                    var itemName = i.GetType().GetProperty("ItemName").GetValue(i, null);
                    if (itemCode != null)
                    {
                        value = itemCode + " - " + itemName;
                    }
                    else
                    {
                        value = "";
                    }

                    break;
                default:
                    var text = i.GetType().GetProperty(keyName)?.GetValue(i, null);
                    value = text != null ? text.ToString() : "";

                    var p = i.Properties.Where(t => t.PropertyName == keyName).FirstOrDefault();
                    if (p != null) value = p.Value;

                    break;
            }
            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money || item.ColumnType == ColumnType.RoundingDigit) && !value.IsNullOrEmpty())
            {
                var digit = item.ColumnName == "UnitPrice" ? i.RoundingDigitUnitCost : i.RoundingDigit;
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true, false, digit);
            }
            else if (item.ColumnType == ColumnType.Date && !value.IsNullOrEmpty())
            {
                AddDateToCell(ws, rowIndex, columnIndex, DateTime.Parse(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }


        protected void WriteBodySaleInvoiceDetail(
         ExcelWorksheet ws,
         int rowIndex,
         int columnIndex,
         CollumnOutput item,
         GetListSaleInvoiceReportOutput i,
         int count)
        {
            string value = "";
            var keyName = item.ColumnName;
            switch (keyName)
            {
                case "Account":
                    var code = i.GetType().GetProperty("AccountCode").GetValue(i, null);
                    var name = i.GetType().GetProperty("AccountName").GetValue(i, null);
                    value = code + " - " + name;
                    break;
                case "Location":
                    value = i.GetType().GetProperty("LocationName").GetValue(i, null)?.ToString();
                    break;
                case "Tax":
                    value = i.GetType().GetProperty("TaxName").GetValue(i, null)?.ToString();
                    break;
                case "Customer":
                    value = i.GetType().GetProperty("CustomerName").GetValue(i, null)?.ToString();
                    break;
                case "Item":
                    var itemCode = i.GetType().GetProperty("ItemCode").GetValue(i, null);
                    var itemName = i.GetType().GetProperty("ItemName").GetValue(i, null);
                    if (itemCode != null)
                    {
                        value = itemCode + " - " + itemName;
                    }
                    else
                    {
                        value = "";
                    }

                    break;
                default:
                    var text = i.GetType().GetProperty(keyName)?.GetValue(i, null);
                    value = text != null ? text.ToString() : "";

                    var p = i.Properties.Where(t => t.PropertyName == keyName).FirstOrDefault();
                    if (p != null) value = p.Value;

                    break;
            }
            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money || item.ColumnType == ColumnType.RoundingDigit) && !value.IsNullOrEmpty())
            {
                var digit = item.ColumnName == "UnitPrice" ? i.RoundingDigitUnitCost : i.RoundingDigit;
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true, false, digit);
            }
            else if (item.ColumnType == ColumnType.Date && !value.IsNullOrEmpty())
            {
                AddDateToCell(ws, rowIndex, columnIndex, DateTime.Parse(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }



        protected void WriteBodySaleReturn(
        ExcelWorksheet ws,
        int rowIndex,
        int columnIndex,
        CollumnOutput item,
        GetListSaleReturnReportOutput i,
        int count)
        {
            string value = "";
            var keyName = item.ColumnName;
            switch (keyName)
            {
                case "Account":
                    var code = i.GetType().GetProperty("AccountCode").GetValue(i, null);
                    var name = i.GetType().GetProperty("AccountName").GetValue(i, null);
                    value = code + " - " + name;
                    break;
                case "Location":
                    value = i.GetType().GetProperty("LocationName").GetValue(i, null)?.ToString();
                    break;
                case "Tax":
                    value = i.GetType().GetProperty("TaxName").GetValue(i, null)?.ToString();
                    break;
                case "Customer":
                    value = i.GetType().GetProperty("CustomerName").GetValue(i, null)?.ToString();
                    break;
                case "Item":
                    var itemCode = i.GetType().GetProperty("ItemCode").GetValue(i, null);
                    var itemName = i.GetType().GetProperty("ItemName").GetValue(i, null);
                    if (itemCode != null)
                    {
                        value = itemCode + " - " + itemName;
                    }
                    else
                    {
                        value = "";
                    }

                    break;
                default:
                    var text = i.GetType().GetProperty(keyName)?.GetValue(i, null);
                    value = text != null ? text.ToString() : "";

                    var p = i.Properties.Where(t => t.PropertyName == keyName).FirstOrDefault();
                    if (p != null) value = p.Value;

                    break;
            }
            if (item.ColumnType == ColumnType.Number && !value.IsNullOrEmpty())
            {
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true);
            }
            else if ((item.ColumnType == ColumnType.Money || item.ColumnType == ColumnType.RoundingDigit) && !value.IsNullOrEmpty())
            {
                var digit = item.ColumnName == "UnitPrice" ? i.RoundingDigitUnitCost : i.RoundingDigit;
                AddNumberToCell(ws, rowIndex, columnIndex, Convert.ToDecimal(value), false, true, false, digit);
            }
            else if (item.ColumnType == ColumnType.Date && !value.IsNullOrEmpty())
            {
                AddDateToCell(ws, rowIndex, columnIndex, DateTime.Parse(value));
            }
            else
            {
                AddTextToCell(ws, rowIndex, columnIndex, value);
            }
        }


        #region Excel helper Method

        protected double ConvertPixelToInches(decimal pixels)
        {
            var p = Convert.ToInt32(pixels);
            var result = (p - 12) / 7d + 1;

            return result;
        }
        protected string GetAddressName(ExcelWorksheet sheet, int rowIndex, int columnIndex)
        {
            var address = sheet.Cells[rowIndex, columnIndex].Address;
            return address;
        }

        protected void AddFormula(
            ExcelWorksheet sheet,
            int rowIndex,
            int columnIndex,
            string formula,
            bool isBold = false,
            bool isFormatPercentTag = false,
            bool isNotFormatNumber = false)
        {
            sheet.Cells[rowIndex, columnIndex].Formula = formula;
            sheet.Cells[rowIndex, columnIndex].Style.Font.Bold = isBold;
            if (isFormatPercentTag)
            {
                sheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = "#0\\.00%";
            }
            else if (isNotFormatNumber)
            {
                sheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = "#";
            }
            else
            {
                sheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = " #,##0.00_);[Red](#,##0.00)"; //"#,##0.00";
            }
        }

        protected void AddDropdownList(
            ExcelWorksheet sheet,
            int rowIndex,
            int columnIndex,
            List<string> list,
            string value)
        {
            var address = GetAddressName(sheet, rowIndex, columnIndex);
            var dataValidation = sheet.DataValidations.AddListValidation(address);
            foreach (var i in list)
            {
                dataValidation.Formula.Values.Add(i);
            }
            sheet.Cells[rowIndex, columnIndex].Value = value;
        }

        protected void SetDataValidatorNotTranslate(ExcelWorksheet sheet, int rowIndex,
            int columnIndex,string valueRange, string value)
        {
            var address = GetAddressName(sheet, rowIndex, columnIndex);
            var val = sheet.DataValidations.AddListValidation(address);
            val.ShowErrorMessage = true;
            val.ErrorStyle = ExcelDataValidationWarningStyle.warning;
            val.ErrorTitle = "An invalid value was entered";
            val.Error = "Select a value from the list";
            val.ShowInputMessage = true;

            val.Formula.ExcelFormula = $"=INDIRECT(\"{valueRange}\")";
            sheet.Cells[rowIndex, columnIndex].Value = value;

        }

        protected void AddCheckbox(
            ExcelWorksheet sheet,
            int rowIndex,
            int columnIndex,
            bool value)
        {
            var address = GetAddressName(sheet, rowIndex, columnIndex);
            var dataValidation = sheet.DataValidations.AddCustomValidation(address);
            ExcelDrawing checkbox2 = sheet.Drawings.SingleOrDefault(a => a.Name == "Check Box 2");
            //ExcelWorksheet sheet = excel.Workbook.Worksheets.SingleOrDefault(a => a.Name == "Sheet1");
            //ExcelDrawing checkbox2 = sheet.Drawings.SingleOrDefault(a => a.Name == "Check Box 2");
            //var value = sheet.Cells["G5"].Value.ToString();

            sheet.Cells[rowIndex, columnIndex].Value = value;
        }

        protected void AddDateToCell(ExcelWorksheet sheet,
            int rowIndex,
            int columnIndex,
            DateTime text,
            bool isBold = false)
        {
            sheet.Cells[rowIndex, columnIndex].Value = text;
            sheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = "dd-mm-yyyy";
            sheet.Cells[rowIndex, columnIndex].Style.Font.Bold = isBold;
        }

        protected void AddNumberToCell(ExcelWorksheet sheet,
            int rowIndex,
            int columnIndex,
            decimal text,
            bool isBold = false,
            bool isNotFormatNumber = false,
            bool isFormatPercentTag = false,
            int rounding = 2,
            ExcelHorizontalAlignment textAlign = ExcelHorizontalAlignment.Right)
        {

            if (isNotFormatNumber)
            {
                var formatDigitDic = new Dictionary<int, string> {
                    { 1, "#0.0"},
                    { 2, "#0.00"},
                    { 3, "#0.000"},
                    { 4, "#0.0000"},
                    { 5, "#0.00000"},
                    { 6, "#0.000000"},
                    { 7, "#0.0000000"},
                    { 8, "#0.00000000"},
                };
                var res = formatDigitDic.ContainsKey(rounding) ? formatDigitDic[rounding] : "";
                sheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = res;
            }
            else if (isFormatPercentTag)
            {
                sheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = "#0\\.00%";
            }
            else
            {
                sheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = " #,##0.00_);[Red](#,##0.00)";
            }
            sheet.Cells[rowIndex, columnIndex].Value = text;
            sheet.Cells[rowIndex, columnIndex].Style.Font.Bold = isBold;
            sheet.Cells[rowIndex, columnIndex].Style.HorizontalAlignment = textAlign;
        }

        protected void MergeCell(ExcelWorksheet sheet,
            int fromRowIndex,
            int fromColumnIndex,
            int toRowIndex,
            int toColumnIndex,
            ExcelHorizontalAlignment align)
        {
            //[FromRow, FromColumn, ToRow, ToColumn]
            sheet.Cells[fromRowIndex, fromColumnIndex, toRowIndex, toColumnIndex].Merge = true;
            sheet.Cells[fromRowIndex, fromColumnIndex, toRowIndex, toColumnIndex].Style.HorizontalAlignment = align;
        }

        protected void MergeCell(ExcelWorksheet sheet,
            int fromRowIndex,
            int fromColumnIndex,
            int toRowIndex,
            int toColumnIndex,
            ExcelHorizontalAlignment align,
            ExcelVerticalAlignment vAlign)
        {
            //[FromRow, FromColumn, ToRow, ToColumn]
            sheet.Cells[fromRowIndex, fromColumnIndex, toRowIndex, toColumnIndex].Merge = true;
            sheet.Cells[fromRowIndex, fromColumnIndex, toRowIndex, toColumnIndex].Style.HorizontalAlignment = align;
            sheet.Cells[fromRowIndex, fromColumnIndex, toRowIndex, toColumnIndex].Style.VerticalAlignment = vAlign;
        }

        //protected void AddTextToCell(ExcelWorksheet sheet,
        //    int rowIndex,
        //    int columnIndex,
        //    string text,
        //    bool isBold = false,
        //    int indent = 0)
        //{
        //    sheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = " #,##0.00_);[Red](#,##0.00)";
        //    sheet.Cells[rowIndex, columnIndex].Value = text;
        //    sheet.Cells[rowIndex, columnIndex].Style.Font.Bold = isBold;
        //    sheet.Cells[rowIndex, columnIndex].Style.Indent = indent;
        //}

        protected void AddTextToCell(ExcelWorksheet sheet,
          int rowIndex,
          int columnIndex,
          string text,
          bool isBold = false,
          int indent = 0,
          bool isRequired = false,
          bool wrapText = false)
        {

            if (isRequired)
            {
                sheet.Cells[rowIndex, columnIndex].RichText.Add(text).Color = Color.Black;
                sheet.Cells[rowIndex, columnIndex].RichText.Add(" *").Color = Color.Red;
            }
            else
            {
                sheet.Cells[rowIndex, columnIndex].RichText.Add(text);
            }
            sheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = " #,##0.00_);[Red](#,##0.00)";
            sheet.Cells[rowIndex, columnIndex].Style.Font.Bold = isBold;
            sheet.Cells[rowIndex, columnIndex].Style.Indent = indent;
            sheet.Cells[rowIndex, columnIndex].Style.WrapText = wrapText;
        }

        protected void LockCell(ExcelWorksheet sheet,
            int rowIndex,
            int columnIndex)
        {
            sheet.Cells[rowIndex, columnIndex].Style.Locked = false;
            sheet.Protection.IsProtected = true;
        }

        //protected void RemoveFile(FileDto file, AppFolders appFolders)
        //{
        //    var tokenPath = Path.Combine(appFolders.TempFileDownloadFolder, file.FileToken);
        //    if (!File.Exists(tokenPath))
        //    {
        //        throw new FileNotFoundException();
        //    }
        //    File.Delete(tokenPath);
        //}

        //protected ExcelPackage Read(FileDto file, AppFolders appFolders)
        //{
        //    var filePath = Path.Combine(appFolders.TempFileDownloadFolder, file.FileToken);
        //    var existingFile = new FileInfo(filePath);
        //    var package = new ExcelPackage(existingFile);
        //    return package;
        //}

        protected async Task<ExcelPackage> Read(FileDto file)
        {
            var fileSteam = await _fileStorageManager.DownloadTempFile(file.FileToken);
            var package = new ExcelPackage(fileSteam);
            return package;
        }


        protected FileDto CreateExcelPackage(string fileName, Action<ExcelPackage> creator)
        {
            var file = new FileDto(fileName, MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);

            using (var excelPackage = new ExcelPackage())
            {
                creator(excelPackage);
                Save(excelPackage, file);
            }

            return file;
        }
        protected void Save(ExcelPackage excelPackage, FileDto file)
        {
            var filePath = Path.Combine(_appFolders.TempFileDownloadFolder, file.FileToken);
            excelPackage.SaveAs(new FileInfo(filePath));
        }

        protected void MergeCell(ExcelWorksheet sheet, int fromRow, int fromCol, int toRow, int toCol)
        {
            try
            {
                sheet.Cells[fromRow, fromCol, toRow, toCol].Merge = true;
            }
            catch (Exception) { }
        }

        protected void MergeAndSetBorderToCel(ExcelWorksheet sheet, int fromRow, int fromCol, int toRow, int toCol, BorderStyle borderStyle = BorderStyle.Thin)
        {
            try
            {
                var _borderStyle = ExcelBorderStyle.None;
                switch (borderStyle)
                {
                    case BorderStyle.Double:
                        _borderStyle = ExcelBorderStyle.Double;
                        break;
                    case BorderStyle.Dashed:
                        _borderStyle = ExcelBorderStyle.Dashed;
                        break;
                }
                sheet.Cells[fromRow, fromCol, toRow, toCol].Merge = true;
                sheet.Cells[fromRow, fromCol, toRow, toCol].Style.Border.Top.Style = _borderStyle;
                sheet.Cells[fromRow, fromCol, toRow, toCol].Style.Border.Right.Style = _borderStyle;
                sheet.Cells[fromRow, fromCol, toRow, toCol].Style.Border.Bottom.Style = _borderStyle;
                sheet.Cells[fromRow, fromCol, toRow, toCol].Style.Border.Left.Style = _borderStyle;

            }
            catch (Exception) { }
        }

        protected void SetFontSize(ExcelWorksheet sheet, int rowIndex, int columnIndex, int pexiel)
        {
            if (pexiel <= 0)
            {
                return;
            }
            sheet.Cells[rowIndex, columnIndex].Style.Font.Size = pexiel;
        }

        protected void SetFontBold(ExcelWorksheet sheet, int rowIndex, int columnIndex)
        {
            sheet.Cells[rowIndex, columnIndex].Style.Font.Bold = true;
        }

        /// <summary>
        /// Add text to single cell
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="columnIndex"></param>
        /// <param name="headerText"></param>
        protected void AddHeader(ExcelWorksheet sheet, int columnIndex, string headerText)
        {
            sheet.Cells[1, columnIndex].Value = headerText;
            sheet.Cells[1, columnIndex].Style.Font.Bold = true;
        }


        protected void FreezePane(ExcelWorksheet sheet, int rowIndex, int columnIndex)
        {
            //sheet.Cells[rowIndex, columnIndex]
            sheet.View.FreezePanes(rowIndex, columnIndex);
        }

        protected void AddObjects<T>(ExcelWorksheet sheet, int startRowIndex, IList<T> items, params Func<T, object>[] propertySelectors)
        {
            if (items.IsNullOrEmpty() || propertySelectors.IsNullOrEmpty())
            {

                return;
            }

            for (var i = 0; i < items.Count; i++)
            {
                for (var j = 1; j < propertySelectors.Length; j++)
                {
                    if (propertySelectors[j] == null) continue;
                    sheet.Cells[i + startRowIndex, j].Value = propertySelectors[j](items[i]);
                }
            }
        }


        public enum BorderDirection
        {
            None = 1,
            All,
            Left,
            Right,
            Bottom,
            Top,
            Diagonal
        }
        public enum BorderStyle
        {
            None = 1,
            DashDot,
            DashDotDot,
            Dashed,
            Dotted,
            Double,
            Hair,
            Medium,
            MediumDashDot,
            MediumDashDotDot,
            MediumDashed,
            Thick,
            Thin
        }
        public enum UnderlineType
        {
            None = 1,
            Single,
            Double,
            DoubleAccounting,
            SingleAccounting
        }


        /// <summary>
        /// use for format excel cell
        /// </summary>
        public class ExcelFormula
        {
            /// <summary>
            /// yyyy-mm-dd
            /// </summary>
            public static string DateFormat = "yyyy-mm-dd";

            /// <summary>
            /// hh:mm:ss AM/PM
            /// </summary>
            public static string TimeFormatWithSecond = "h:mm:ss AM/PM";
            /// <summary>
            /// hh:mm AM/PM
            /// </summary>
            public static string TimeFormatWithSecond2 = "hh:mm AM/PM";
            /// <summary>
            /// hh:mm
            /// </summary>
            public static string TimeFormat = "hh:mm";
            /// <summary>
            /// [$-F400]h:mm:ss\\ AM/PM
            /// </summary>
            public static string TimeFormatByCategory = "[$-F400]hh:mm:ss\\ AM/PM";
        }

        #endregion



        protected string ReadTemplateFile(FileDto file)
        {
            var tokenPath = string.IsNullOrEmpty(file.SubFolder) ?
                            Path.Combine(AppFolders.TemplateFolder, file.FileToken) :
                            Path.Combine(AppFolders.TemplateFolder, file.SubFolder, file.FileToken);
            if (!File.Exists(tokenPath))
            {
                throw new FileNotFoundException();
            }
            string str = "";
            using (StreamReader r = new StreamReader(tokenPath))
            {
                str = r.ReadToEnd();
            }
            return str;
        }

        protected EvoPdf.HtmlToPdfClient.HtmlToPdfConverter GetInitPDF()
        {
            var _evoLicenseKey = GetEvoLicenKey();

            EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = new EvoPdf.HtmlToPdfClient.HtmlToPdfConverter(GetEvoIPAddress(), GetEvosTCPPort());

            htmlToPdfConverter.LicenseKey = _evoLicenseKey.ToString();
            htmlToPdfConverter.PdfDocumentOptions.EmbedFonts = true;
            htmlToPdfConverter.PdfDocumentOptions.FitWidth = true;
            htmlToPdfConverter.PdfDocumentOptions.AutoSizePdfPage = false;
            htmlToPdfConverter.PdfDocumentOptions.TableHeaderRepeatEnabled = true;
            htmlToPdfConverter.PdfDocumentOptions.TableFooterRepeatEnabled = false;

            return htmlToPdfConverter;
        }

        protected async Task<byte[]> GetInitPDFBarcode(string exportHtml)
        {
            using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true }))
            {
                using (var page = await browser.NewPageAsync())
                {
                    await page.SetContentAsync(exportHtml);
                    await page.WaitForSelectorAsync("body");


                    return await page.PdfDataAsync(new PdfOptions
                    {
                        Format = PaperFormat.A4,
                        PreferCSSPageSize = true,
                        Landscape = false,
                        DisplayHeaderFooter = false,
                        MarginOptions = new PuppeteerSharp.Media.MarginOptions
                        {
                            Top = "0cm",
                            Bottom = "0cm",
                            Left = "0cm",
                            Right = "0cm"
                        },
                        PrintBackground = true,
                    });
                }
            }
        }

        protected EvoPdf.HtmlToPdfClient.HtmlToPdfConverter GetInitPDFOption(PdfPageOrientation orientation = PdfPageOrientation.Landscape)
        {
            var _evoLicenseKey = GetEvoLicenKey();

            EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = new EvoPdf.HtmlToPdfClient.HtmlToPdfConverter(GetEvoIPAddress(), GetEvosTCPPort());

            htmlToPdfConverter.LicenseKey = _evoLicenseKey.ToString();
            htmlToPdfConverter.PdfDocumentOptions.EmbedFonts = true;
            htmlToPdfConverter.PdfDocumentOptions.FitWidth = false;
            htmlToPdfConverter.PdfDocumentOptions.AutoSizePdfPage = false;
            htmlToPdfConverter.PdfDocumentOptions.TableHeaderRepeatEnabled = true;

            //htmlToPdfConverter.PdfDocumentOptions.X = 30;
            //htmlToPdfConverter.PdfDocumentOptions.Y = 10;
            htmlToPdfConverter.PdfDocumentOptions.PdfPageOrientation = orientation;
            htmlToPdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;

            htmlToPdfConverter.PdfDocumentOptions.LeftMargin = 10;
            htmlToPdfConverter.PdfDocumentOptions.RightMargin = 14;

            htmlToPdfConverter.PdfDocumentOptions.TopMargin = 10;
            htmlToPdfConverter.PdfDocumentOptions.BottomMargin = 10;

            return htmlToPdfConverter;
        }

        protected void AddHeaderPDF(HtmlToPdfConverter htmlToPdfConverter, string companyName,
            string reportName, DateTime fromDate, DateTime toDate,
            string formatDate, PdfPageOrientation orientation = PdfPageOrientation.Landscape)
        {
            var date = string.Empty;
            if (fromDate.Date.ToString(formatDate) == new DateTime(0001, 1, 1).ToString(formatDate) || fromDate.Date.ToString(formatDate) == new DateTime(1970, 1, 1).ToString(formatDate))
            {
                date = "As Of - " + toDate.ToString(formatDate);
            }
            else
            {
                date = fromDate.Date.ToString(formatDate) + " - " + toDate.Date.ToString(formatDate);
            }
            htmlToPdfConverter.PdfDocumentOptions.ShowHeader = true;
            htmlToPdfConverter.PdfHeaderOptions.HeaderHeight = 80;
            htmlToPdfConverter.PdfHeaderOptions.ShowInFirstPage = true;
            htmlToPdfConverter.PdfHeaderOptions.ShowInEvenPages = false;
            htmlToPdfConverter.PdfHeaderOptions.ShowInOddPages = false;


            var htmlContent = $"<div style=\"font-family:'Khmer OS Battambang'; font-size: 18px; text-align: center;\">" +
                 $"<div><span style=\"display: block; position: absolute; left: 8px;\"></span>{companyName}</div>" +
                 $"<div><span style=\"display: block; position: absolute; left: 8px;\"></span>{reportName}</div>" +
                 $"<div>{date}</div>" +
                 $"</div>";
            HtmlToPdfElement headerHtml = new HtmlToPdfElement(htmlContent, "");
            headerHtml.HtmlViewerWidth = orientation == PdfPageOrientation.Landscape ? 1124 : 794;
            htmlToPdfConverter.PdfHeaderOptions.AddElement(headerHtml);
        }

        protected void AddFooterPDF(HtmlToPdfConverter htmlToPdfConverter, string userName, string formatDate, PdfPageOrientation orientation = PdfPageOrientation.Landscape)
        {
            // Show fix footer 
            htmlToPdfConverter.PdfDocumentOptions.ShowFooter = true;
            // Set the footer height in points
            htmlToPdfConverter.PdfFooterOptions.FooterHeight = 25;


            TimeZoneInfo tz = base.MyTimeZoneInfo();
            var today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

            //var today = _timeZoneConverter.Convert(DateTime.Now, AbpSession.TenantId, AbpSession.UserId.Value);//Clock.Now;



            var printDate = today.Date.ToString(formatDate);
            var printTime = String.Format("{0:hh:mm tt}", today);

            var htmlContent = $"<table style=\"font-family:'Khmer OS Battambang'; font-size: 14px;width:100%;\"><tr>" +
                 $"<td style='width: 34%;'>{_appConfiguration["App:Name"]} - {userName} </td>" +
                 $"<td style='width: 33%;text-align: center;'><span>&p;</span>/<span>&P;</span></td>" +
                 $"<td style='width: 33%;text-align: right'>{printDate} {printTime}</td>" +
                 $"</tr></table>";
            HtmlToPdfElement footerHtml = new HtmlToPdfVariableElement(htmlContent, "");
            footerHtml.HtmlViewerWidth = orientation == PdfPageOrientation.Landscape ? 1124 : 794;
            htmlToPdfConverter.PdfFooterOptions.AddElement(footerHtml);
        }


        protected async Task UpdatePOReceiptStautus(Guid? itemReceiptId, ItemReceipt.ReceiveFromStatus receiveFrom, Guid? billId, List<CreateOrUpdateItemReceiptItemInput> inputItems)
        {

            var itemReceiptEntity = await _itemReceiptRepository.FirstOrDefaultAsync(s => s.Id == itemReceiptId);

            Bill bill = null;
            if (receiveFrom == ItemReceipt.ReceiveFromStatus.Bill && billId != null)
            {
                bill = await _billRepository.FirstOrDefaultAsync(s => s.Id == billId);
            }

            if (!(receiveFrom == ItemReceipt.ReceiveFromStatus.PO ||
                   (receiveFrom == ItemReceipt.ReceiveFromStatus.Bill &&
                   bill != null && bill.ReceiveFrom == Bill.ReceiveFromStatus.PO)) &&
                itemReceiptEntity != null && itemReceiptEntity.ReceiveFrom != ItemReceipt.ReceiveFromStatus.PO) return;

            List<ItemReceiptEntity> itemReceiptItemEntities = new List<ItemReceiptEntity>();
            if (itemReceiptId != null)
            {
                itemReceiptItemEntities = await (from iri in _itemReceiptItemRepository.GetAll()
                                                 .Include(s => s.OrderItem)
                                                 where iri.ItemReceiptId == itemReceiptId
                                                 select new ItemReceiptEntity
                                                 {
                                                     Id = iri.Id,
                                                     OrderItemId = iri.OrderItemId,
                                                     Qty = iri.Qty,
                                                     POId = iri.OrderItem.PurchaseOrderId
                                                 }
                                                ).ToListAsync();
            }

            var poIds = inputItems == null ? new List<Guid?>() :
                                    inputItems.Where(s => s.PurchaseOrderId != null)
                                    .GroupBy(s => s.PurchaseOrderId).Select(s => s.Key).ToList();
            var oldPOId = itemReceiptItemEntities.GroupBy(s => s.POId).Select(s => (Guid?)s.Key).ToList();
            poIds = poIds.Union(oldPOId).ToList();


            var poItems = await (from poi in _purchaseOrderItemRepository.GetAll()
                                .Include(s => s.PurchaseOrder)
                                .Where(s => poIds.Contains(s.PurchaseOrderId))

                                 join iRi in _itemReceiptItemRepository.GetAll()
                                 // .Where(s => s.OrderItemId != null)
                                 on poi.Id equals iRi.OrderItemId into rItems

                                 let receiptQty = rItems == null ? 0 : rItems.Sum(s => s.Qty)
                                 let remainQty = poi.Unit - receiptQty

                                 select new ItemReceiptEntity
                                 {
                                     POId = poi.PurchaseOrderId,
                                     OrderItemId = poi.Id,
                                     RemainingQty = remainQty,
                                     Qty = poi.Unit,
                                 }
                          ).ToListAsync();

            var updateItemReceiptItemIds = inputItems == null ? new List<Guid?>() : inputItems.Where(s => s.Id != null).Select(s => s.Id).ToList();

            var addItems = inputItems == null ? new List<CreateOrUpdateItemReceiptItemInput>() : inputItems.Where(s => s.Id == null).ToList();
            var deleteItems = itemReceiptItemEntities.Where(s => !updateItemReceiptItemIds.Contains(s.Id)).ToList();
            var updateItems = itemReceiptItemEntities.Where(s => updateItemReceiptItemIds.Contains(s.Id)).ToList();

            if (itemReceiptEntity != null && deleteItems.Any() &&
                (itemReceiptEntity.ReceiveFrom == ItemReceipt.ReceiveFromStatus.PO ||
                (receiveFrom == ItemReceipt.ReceiveFromStatus.Bill && bill != null && bill.ReceiveFrom == Bill.ReceiveFromStatus.PO)))
                foreach (var item in deleteItems)
                {
                    var poItem = poItems.Where(s => s.OrderItemId == item.OrderItemId).FirstOrDefault();
                    if (poItem != null) poItem.RemainingQty += item.Qty;
                }

            if (addItems.Any() &&
                (receiveFrom == ItemReceipt.ReceiveFromStatus.PO ||
                (receiveFrom == ItemReceipt.ReceiveFromStatus.Bill && bill != null && bill.ReceiveFrom == Bill.ReceiveFromStatus.PO)))
                foreach (var item in addItems)
                {
                    var poItem = poItems.Where(s => s.OrderItemId == item.OrderItemId).FirstOrDefault();
                    if (poItem != null) poItem.RemainingQty -= item.Qty;
                }

            if (updateItems.Any() && itemReceiptEntity != null && (itemReceiptEntity.ReceiveFrom == ItemReceipt.ReceiveFromStatus.PO || receiveFrom == ItemReceipt.ReceiveFromStatus.PO ||
                (receiveFrom == ItemReceipt.ReceiveFromStatus.Bill && bill != null && bill.ReceiveFrom == Bill.ReceiveFromStatus.PO)))
                foreach (var item in updateItems)
                {
                    var poItem = poItems.Where(s => s.OrderItemId == item.OrderItemId).FirstOrDefault();
                    if (poItem != null)
                    {
                        var oldQty = item.Qty;
                        var newQty = inputItems == null ? 0 :
                            inputItems.Where(t => t.OrderItemId == item.OrderItemId && t.Id == item.Id).Select(s => s.Qty).Sum();

                        poItem.RemainingQty += oldQty - newQty;
                    }
                }


            var overOrderQtyItem = poItems.Where(s => s.RemainingQty < 0).Select(s => (Guid?)s.OrderItemId).ToList();

            if (overOrderQtyItem.Any())
            {
                var items = inputItems.Where(s => overOrderQtyItem.Contains(s.OrderItemId)).Select((s, index) => s.ItemName + " row = " + (inputItems.IndexOf(s) + 1)).ToList();

                throw new UserFriendlyException(L("ReceiveMessageWarning", items.Aggregate((i, j) => i + ", " + j)));
            }

            var pos = poItems.GroupBy(s => s.POId).Select(s => s.Key).ToList();

            var POs = await _purchaseOrderRepository.GetAll().Where(t => pos.Contains(t.Id)).ToListAsync();
            foreach (var po in POs)
            {
                var receiveAll = poItems.Where(s => s.POId == po.Id).All(s => s.RemainingQty <= 0);
                var notReceive = poItems.Where(s => s.POId == po.Id).All(s => s.Qty == s.RemainingQty);

                if (receiveAll)
                {
                    po.UpdateReceiveStatusToReceiveAll();
                }
                else if (notReceive)
                {
                    po.UpdateReceiveStatusToPending();
                }
                else
                {
                    po.UpdateReceiveStatusToPartial();
                }

                CheckErrors(await _purchaseOrderManager.UpdateAsync(po, false));
            }
        }

        protected string ReplaceSpecialCharacter(string str)
        {
            Regex reg = new Regex("[*'\",_&#^@]");
            var str1 = reg.Replace(str, string.Empty);
            return str1;
        }

        protected async Task UpdateSOReceiptStautus(Guid? itemIssueId, ReceiveFrom receiveFrom, Guid? invoiceId, List<CreateOrUpdateItemIssueItemInput> inputItems)
        {

            var itemIssueEntity = await _itemIssueRepository.FirstOrDefaultAsync(s => s.Id == itemIssueId);

            Invoice invoice = null;
            if (receiveFrom == ReceiveFrom.Invoice && invoiceId != null)
            {
                invoice = await _invoiceRepository.FirstOrDefaultAsync(s => s.Id == invoiceId);
            }

            if (!(receiveFrom == ReceiveFrom.SaleOrder ||
                   (receiveFrom == ReceiveFrom.Invoice &&
                   invoice != null && invoice.ReceiveFrom == ReceiveFrom.SaleOrder)) &&
                itemIssueEntity != null && itemIssueEntity.ReceiveFrom != ReceiveFrom.SaleOrder) return;

            List<ItemIssueEntity> itemIssueItemEntities = new List<ItemIssueEntity>();
            if (itemIssueId != null)
            {
                itemIssueItemEntities = await (from iri in _itemIssueItemRepository.GetAll()
                                                 .Include(s => s.SaleOrderItem)
                                               where iri.ItemIssueId == itemIssueId
                                               select new ItemIssueEntity
                                               {
                                                   Id = iri.Id,
                                                   OrderItemId = iri.SaleOrderItemId,
                                                   Qty = iri.Qty,
                                                   SOId = iri.SaleOrderItem.SaleOrderId
                                               }
                                                ).ToListAsync();
            }

            var soIds = inputItems == null ? new List<Guid?>() :
                                    inputItems.Where(s => s.SaleOrderId != null)
                                    .GroupBy(s => s.SaleOrderId).Select(s => s.Key).ToList();
            var oldSOId = itemIssueItemEntities.GroupBy(s => s.SOId).Select(s => (Guid?)s.Key).ToList();
            soIds = soIds.Union(oldSOId).ToList();


            var soItems = await (from soi in _saleOrderItemRepository.GetAll()
                                .Include(s => s.SaleOrder)
                                .Where(s => soIds.Contains(s.SaleOrderId))

                                 join iRi in _itemIssueItemRepository.GetAll()
                                 // .Where(s => s.OrderItemId != null)
                                 on soi.Id equals iRi.SaleOrderItemId into rItems

                                 let issueQty = rItems == null ? 0 : rItems.Sum(s => s.Qty)
                                 let remainQty = soi.Qty - issueQty

                                 select new ItemIssueEntity
                                 {
                                     SOId = soi.SaleOrderId,
                                     OrderItemId = soi.Id,
                                     RemainingQty = remainQty,
                                     Qty = soi.Qty,
                                 }
                          ).ToListAsync();

            var updateItemReceiptItemIds = inputItems == null ? new List<Guid?>() : inputItems.Where(s => s.Id != null).Select(s => s.Id).ToList();

            var addItems = inputItems == null ? new List<CreateOrUpdateItemIssueItemInput>() : inputItems.Where(s => s.Id == null).ToList();
            var deleteItems = itemIssueItemEntities.Where(s => !updateItemReceiptItemIds.Contains(s.Id)).ToList();
            var updateItems = itemIssueItemEntities.Where(s => updateItemReceiptItemIds.Contains(s.Id)).ToList();

            if (itemIssueEntity != null && deleteItems.Any() &&
                (itemIssueEntity.ReceiveFrom == ReceiveFrom.SaleOrder ||
                (receiveFrom == ReceiveFrom.Invoice && invoice != null && invoice.ReceiveFrom == ReceiveFrom.SaleOrder)))
                foreach (var item in deleteItems)
                {
                    var soItem = soItems.Where(s => s.OrderItemId == item.OrderItemId).FirstOrDefault();
                    if (soItem != null) soItem.RemainingQty += item.Qty;
                }

            if (addItems.Any() &&
                (receiveFrom == ReceiveFrom.SaleOrder ||
                (receiveFrom == ReceiveFrom.Invoice && invoice != null && invoice.ReceiveFrom == ReceiveFrom.SaleOrder)))
                foreach (var item in addItems)
                {
                    var soItem = soItems.Where(s => s.OrderItemId == item.SaleOrderItemId).FirstOrDefault();
                    if (soItem != null) soItem.RemainingQty -= item.Qty;
                }

            if (updateItems.Any() && itemIssueEntity != null && (itemIssueEntity.ReceiveFrom == ReceiveFrom.SaleOrder || receiveFrom == ReceiveFrom.SaleOrder ||
                (receiveFrom == ReceiveFrom.Invoice && invoice != null && invoice.ReceiveFrom == ReceiveFrom.SaleOrder)))
                foreach (var item in updateItems)
                {
                    var soItem = soItems.Where(s => s.OrderItemId == item.OrderItemId).FirstOrDefault();
                    if (soItem != null)
                    {
                        var oldQty = item.Qty;
                        var newQty = inputItems == null ? 0 :
                            inputItems.Where(t => t.SaleOrderItemId == item.OrderItemId && t.Id == item.Id).Select(s => s.Qty).Sum();

                        soItem.RemainingQty += oldQty - newQty;
                    }
                }


            var overOrderQtyItem = soItems.Where(s => s.RemainingQty < 0).Select(s => (Guid?)s.OrderItemId).ToList();

            if (overOrderQtyItem.Any())
            {
                var items = inputItems.Where(s => overOrderQtyItem.Contains(s.SaleOrderItemId)).Select((s, index) => s.Item.ItemName + " row = " + (inputItems.IndexOf(s) + 1)).ToList();

                throw new UserFriendlyException(L("ReceiveMessageWarning", items.Aggregate((i, j) => i + ", " + j)));
            }

            var sos = soItems.GroupBy(s => s.SOId).Select(s => s.Key).ToList();

            var SOs = await _saleOrderRepository.GetAll().Where(t => sos.Contains(t.Id)).ToListAsync();
            foreach (var so in SOs)
            {
                var receiveAll = soItems.Where(s => s.SOId == so.Id).All(s => s.RemainingQty <= 0);
                var notReceive = soItems.Where(s => s.SOId == so.Id).All(s => s.Qty == s.RemainingQty);

                if (receiveAll)
                {
                    so.UpdateReceiveStatusToReceiveAll();
                }
                else if (notReceive)
                {
                    so.UpdateReceiveStatusToPending();
                }
                else
                {
                    so.UpdateReceiveStatusToPartial();
                }

                CheckErrors(await _saleOrderManager.UpdateAsync(so, false));
            }
        }




        protected string AutoRound(decimal value)
        {
            var st = value.ToString().Split(".");
            if (st.Length == 1) return value.ToString("#,##0");

            var d = TrimR(st[1], '0');

            if (d == "") return Convert.ToDecimal(st[0]).ToString("#,##0");

            return Convert.ToDecimal(st[0]).ToString("#,##0") + "." + d;
        }

        private string TrimR(string v, char c)
        {
            var r = v.TrimEnd(c);
            if (r == v) return r;
            return TrimR(r, c);
        }

    }

}
