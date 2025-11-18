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
using CorarlERP.AccountCycles;
using CorarlERP.Addresses;
using CorarlERP.Authorization;
using CorarlERP.Authorization.Users;
using CorarlERP.AutoSequences;
using CorarlERP.BatchNos;
using CorarlERP.Bills.Dto;
using CorarlERP.Boms;
using CorarlERP.ChartOfAccounts;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.Currencies;
using CorarlERP.Currencies.Dto;
using CorarlERP.CustomerCredits;
using CorarlERP.CustomerManager;
using CorarlERP.Customers;
using CorarlERP.Customers.Dto;
using CorarlERP.DeliverySchedules;
using CorarlERP.DeliverySchedules.Dto;
using CorarlERP.Dto;
using CorarlERP.Exchanges.Dto;
using CorarlERP.Features;
using CorarlERP.FileStorages;
using CorarlERP.FileUploads;
using CorarlERP.Galleries;
using CorarlERP.Inventories;
using CorarlERP.Inventories.Data;
using CorarlERP.InventoryCalculationItems;
using CorarlERP.InventoryTransactionItems;
using CorarlERP.Invoices;
using CorarlERP.Invoices.Dto;
using CorarlERP.InvoiceSales.Dto;
using CorarlERP.InvoiceTemplates;
using CorarlERP.InvoiceTemplates.Dto;
using CorarlERP.ItemIssues;
using CorarlERP.ItemIssues.Dto;
using CorarlERP.ItemReceiptCustomerCredits;
using CorarlERP.ItemReceipts;
using CorarlERP.Items;
using CorarlERP.Items.Dto;
using CorarlERP.Journals;
using CorarlERP.Journals.Dto;
using CorarlERP.JournalTransactionTypes;
using CorarlERP.Locations.Dto;
using CorarlERP.Locks;
using CorarlERP.Lots;
using CorarlERP.Lots.Dto;
using CorarlERP.MultiCurrencies;
using CorarlERP.MultiTenancy;
using CorarlERP.ReceivePayments;
using CorarlERP.Reports;
using CorarlERP.Reports.Dto;
using CorarlERP.ReportTemplates;
using CorarlERP.SaleOrders;
using CorarlERP.SaleOrders.Dto;
using CorarlERP.Settings;
using CorarlERP.Taxes.Dto;
using CorarlERP.TransactionTypes;
using CorarlERP.Url;
using CorarlERP.UserGroups;
using EvoPdf.HtmlToPdfClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.InvoiceSales
{
    [AbpAuthorize]
    public class InvoiceSaleAppService : ReportBaseClass, IInvoiceSaleAppService
    {
        protected readonly AppFolders _appFolders;
        protected readonly IRepository<CustomerCredits.CustomerCredit, Guid> _customerCreditRepository;
        protected readonly ICorarlRepository<CustomerCredits.CustomerCreditDetail, Guid> _customerCreditItemRepository;
        protected readonly ICorarlRepository<CustomerCredit, Guid> _cusotmerCreditRepository;
        protected readonly ICorarlRepository<ItemIssueItem, Guid> _itemIssueItemRepository;
        protected readonly IItemIssueItemManager _itemIssueItemManager;

        protected readonly ICorarlRepository<ItemIssue, Guid> _itemIssueRepository;
        protected readonly IItemIssueManager _itemIssueManager;

        protected readonly ICurrencyManager _currencyManager;
        protected readonly IRepository<Currency, long> _currencyRepository;
        protected readonly IRepository<MultiCurrency, long> _multiCurrencyRepository;

        protected readonly ICustomerManagerHelper _customerHelperManager;

        protected readonly ICustomerManager _customerManager;
        protected readonly IRepository<Customer, Guid> _customerRepository;

        protected readonly ISaleOrderItemManager _saleOrderItemManager;
        protected readonly IRepository<SaleOrderItem, Guid> _saleOrderItemRepository;

        protected readonly ISaleOrderManager _saleOrderManager;
        protected readonly IRepository<SaleOrder, Guid> _saleOrderRepository;

        protected readonly IInvoiceItemManager _invoiceItemManager;
        protected readonly IInvoiceManager _invoiceManager;

        protected readonly ICorarlRepository<Invoice, Guid> _invoiceRepository;
        protected readonly IRepository<Lot, long> _lotRepository;
        protected readonly ICorarlRepository<InvoiceItem, Guid> _invoiceItemRepository;
        private readonly ICorarlRepository<InvoiceExchangeRate, Guid> _exchangeRateRepository;
        private readonly ICorarlRepository<CustomerCreditExchangeRate, Guid> _customerCreditExchangeRateRepository;

        //protected readonly IRepository<VendorCredit.VendorCredit, Guid> _vendorCreditRepository;
        //protected readonly IRepository<VendorCredit.VendorCreditDetail, Guid> _vendorCreditDetailRepository;

        protected readonly IJournalManager _journalManager;
        protected readonly ICorarlRepository<Journal, Guid> _journalRepository;

        protected readonly IJournalItemManager _journalItemManager;
        protected readonly ICorarlRepository<JournalItem, Guid> _journalItemRepository;

        protected readonly IChartOfAccountManager _chartOfAccountManager;
        protected readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;

        private readonly IRepository<ItemLot, Guid> _itemLotRepository;
        private readonly IRepository<BomItem, Guid> _bomitemRepository;
        protected readonly IRepository<Item, Guid> _itemRepository;
        protected readonly IItemManager _itemManager;

        protected readonly IRepository<ItemReceiptItem, Guid> _itemReceiptItemRepository;

        protected readonly IRepository<ReceivePaymentDetail, Guid> _receivePaymentDetailRepository;
        protected readonly IRepository<ReceivePayment, Guid> _receivePaymentRepository;
        protected readonly ICorarlRepository<ItemReceiptCustomerCredit, Guid> _itemReceiptCustomerCreditRepository;
        protected readonly ICorarlRepository<ItemReceiptItemCustomerCredit, Guid> _itemReceiptItemCustomerCreditRepository;
        protected readonly ICorarlRepository<ItemReceiptCustomerCreditItemBatchNo, Guid> _itemReceiptCustomerCreditItemBatchNoRepository;
        protected readonly ICorarlRepository<CustomerCreditItemBatchNo, Guid> _customerCreditItemBatchNoRepository;
        protected readonly IInventoryManager _inventoryManager;

        protected readonly IAutoSequenceManager _autoSequenceManager;
        protected readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;
        protected readonly IRepository<User, long> _userRepository;

        protected readonly IRepository<Taxes.Tax, long> _taxRepository;
        protected readonly IRepository<Locations.Location, long> _locationRepository;
        protected readonly IRepository<Classes.Class, long> _classRepository;
        protected readonly IRepository<AccountCycles.AccountCycle, long> _accountCyclesRepository;

        protected readonly IRepository<Lock, long> _lockRepository;
        private readonly IRepository<BillInvoiceSetting, long> _settingRepository;

        private readonly IRepository<InvoiceTemplate, Guid> _invoiceTemplateRepository;
        private readonly IRepository<InvoiceTemplateMap, Guid> _invoiceTemplateMapRepository;
        private readonly IRepository<Gallery, Guid> _galleryRepository;
        private readonly string _baseUrl;
        private readonly ITenantManager _tenantManager;

        private readonly IFileUploadManager _fileUploadManager;
        private readonly IFileStorageManager _fileStorageManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IInventoryCalculationItemManager _inventoryCalculationItemManager;
        private readonly ICorarlRepository<BatchNo, Guid> _batchNoRepository;
        private readonly ICorarlRepository<ItemIssueItemBatchNo, Guid> _itemIssueItemBatchNoRepository;
        private readonly ICorarlRepository<InventoryTransactionItem, Guid> _inventoryTransactionItemRepository;
        private readonly IRepository<AccountCycle, long> _accountCycleRepository;
        private readonly IJournalTransactionTypeManager _journalTransactionTypeManager;
        private readonly ICorarlRepository<TransactionType, long> _saleTypeRepository;
        private readonly ICorarlRepository<DeliveryScheduleItem, Guid> _deliveryScheduleItemRepository;
        private readonly ICorarlRepository<DeliverySchedule, Guid> _deliveryScheduleRepository;
        public InvoiceSaleAppService(IJournalManager journalManager, IJournalItemManager journalItemManager,
            ICorarlRepository<ItemReceiptCustomerCreditItemBatchNo, Guid> itemReceiptCustomerCreditItemBatchNoRepository,
            ICorarlRepository<CustomerCreditItemBatchNo, Guid> customerCreditItemBatchNoRepository,
            ICorarlRepository<InvoiceExchangeRate, Guid> exchangeRateRepository,
            ICorarlRepository<CustomerCreditExchangeRate, Guid> customerCreditExchangeRateRepository,
                ICorarlRepository<InventoryTransactionItem, Guid> inventoryTransactionItemRepository,
                IRepository<InvoiceTemplate, Guid> invoiceTemplateRepository,
                IRepository<InvoiceTemplateMap, Guid> invoiceTemplateMapRepository,
                IRepository<Gallery, Guid> galleryRepository,
                ICurrencyManager currencyManager,
                IFileStorageManager fileStorageManager,
                IUnitOfWorkManager unitOfWorkManager,
                IChartOfAccountManager chartOfAccountManager,
                IFileUploadManager fileUploadManager,
                InvoiceManager invoiceManager,
                InvoiceItemManager invoiceItemManager,
                ItemIssueItemManager itemIssueItemManager,
                ItemIssueManager itemIssueManager,
                ItemManager itemManager,
                SaleOrderItemManager saleOrderItemManager,
                ISaleOrderManager saleOrderManager,
                ICustomerManager customerManager,
                IRepository<Customer, Guid> customerRepository,
                ICorarlRepository<Journal, Guid> journalRepository,
                ICorarlRepository<JournalItem, Guid> journalItemRepository,
                IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
                ICorarlRepository<Invoice, Guid> invoiceRepository,
                ICorarlRepository<InvoiceItem, Guid> invoiceItemRepository,
                IRepository<SaleOrderItem, Guid> saleOrderItemRepository,
                IRepository<SaleOrder, Guid> saleOrderRepository,
                IRepository<Currency, long> currencyRepository,
                IRepository<MultiCurrency, long> multiCurrencyRepository,
                ICorarlRepository<ItemIssueItem, Guid> itemIssueItemRepository,
                ICorarlRepository<ItemIssue, Guid> itemIssueRepository,
                IRepository<Item, Guid> itemRepository,
                IRepository<ReceivePaymentDetail, Guid> receivePaymentDetailRepository,
                IRepository<ReceivePayment, Guid> receivePaymentRepository,
                IRepository<ItemReceiptItem, Guid> itemReceiptItemRepository,
                IRepository<CustomerCredits.CustomerCredit, Guid> customerCreditRepository,
                ICorarlRepository<CustomerCredits.CustomerCreditDetail, Guid> customerCreditDetailRepository,
                ICorarlRepository<ItemReceiptCustomerCredit, Guid> itemReceiptCustomerCreditRepository,
                IInventoryManager inventoryManager,
                ICustomerManagerHelper customerHelperManager,
                IAutoSequenceManager autoSequenceManger,
                IRepository<AutoSequence, Guid> autoSequenceRepository,
                ICorarlRepository<BatchNo, Guid> batchNoRepository,
                ICorarlRepository<ItemIssueItemBatchNo, Guid> itemIssueItemBatchNoRepository,
                IRepository<User, long> userRepository,
                IRepository<Lot, long> lotRepository,
                IRepository<ItemLot, Guid> itemLotRepository,
                IRepository<BomItem, Guid> bomitemRepository,
                IRepository<UserGroupMember, Guid> userGroupMemberRepository,
                ICorarlRepository<CustomerCredit, Guid> cusotmerCreditRepository,
                IRepository<Lock, long> lockRepository,
                IRepository<Taxes.Tax, long> taxRepository,
                IRepository<Locations.Location, long> locationRepository,
                IRepository<Classes.Class, long> classRepository,
                IRepository<AccountCycles.AccountCycle, long> accountCyclesRepository,
                AppFolders appFolders,
                ITenantManager tenantManager,
                IInventoryCalculationItemManager inventoryCalculationItemManager,
                ICorarlRepository<ItemReceiptItemCustomerCredit, Guid> itemReceiptItemCustomerCreditRepository,
                IJournalTransactionTypeManager journalTransactionTypeManager,
                ICorarlRepository<TransactionType, long> saleTypeRepository     ,
                ICorarlRepository<DeliveryScheduleItem, Guid> deliveryScheduleItemRepository,
                ICorarlRepository<DeliverySchedule, Guid> deliveryScheduleRepository
            ) : base(accountCyclesRepository, appFolders, userGroupMemberRepository, locationRepository)
        {
            _itemReceiptItemCustomerCreditRepository = itemReceiptItemCustomerCreditRepository;
            _cusotmerCreditRepository = cusotmerCreditRepository;
            _lotRepository = lotRepository;
            _userRepository = userRepository;
            _journalManager = journalManager;
            _journalManager.SetJournalType(JournalType.ItemReceiptPurchase);
            _journalRepository = journalRepository;
            _journalItemManager = journalItemManager;
            _journalItemRepository = journalItemRepository;
            _chartOfAccountManager = chartOfAccountManager;
            _chartOfAccountRepository = chartOfAccountRepository;
            _invoiceItemManager = invoiceItemManager;
            _invoiceItemRepository = invoiceItemRepository;
            _invoiceManager = invoiceManager;
            _invoiceRepository = invoiceRepository;
            _saleOrderItemRepository = saleOrderItemRepository;
            _saleOrderItemManager = saleOrderItemManager;
            _saleOrderRepository = saleOrderRepository;
            _saleOrderManager = saleOrderManager;
            _customerRepository = customerRepository;
            _customerManager = customerManager;
            _currencyRepository = currencyRepository;
            _multiCurrencyRepository = multiCurrencyRepository;
            _currencyManager = currencyManager;
            _itemIssueItemManager = itemIssueItemManager;
            _itemIssueItemRepository = itemIssueItemRepository;
            _itemIssueManager = itemIssueManager;
            _itemIssueRepository = itemIssueRepository;
            _inventoryManager = inventoryManager;
            _itemManager = itemManager;
            _itemRepository = itemRepository;
            _itemReceiptItemRepository = itemReceiptItemRepository;
            _customerCreditItemRepository = customerCreditDetailRepository;
            _customerCreditRepository = customerCreditRepository;
            _receivePaymentDetailRepository = receivePaymentDetailRepository;
            _receivePaymentRepository = receivePaymentRepository;
            _itemReceiptCustomerCreditRepository = itemReceiptCustomerCreditRepository;
            _customerHelperManager = customerHelperManager;
            _autoSequenceManager = autoSequenceManger;
            _autoSequenceRepository = autoSequenceRepository;
            _appFolders = appFolders;
            _tenantManager = tenantManager;
            _locationRepository = locationRepository;
            _classRepository = classRepository;
            _taxRepository = taxRepository;
            _lockRepository = lockRepository;
            _invoiceTemplateRepository = invoiceTemplateRepository;
            _invoiceTemplateMapRepository = invoiceTemplateMapRepository;
            _galleryRepository = galleryRepository;
            _fileUploadManager = fileUploadManager;
            _fileStorageManager = fileStorageManager;
            _unitOfWorkManager = unitOfWorkManager;
            _itemLotRepository = itemLotRepository;
            _bomitemRepository = bomitemRepository;
            _inventoryCalculationItemManager = inventoryCalculationItemManager;
            _batchNoRepository = batchNoRepository;
            _itemIssueItemBatchNoRepository = itemIssueItemBatchNoRepository;
            _accountCycleRepository = accountCyclesRepository;

            _settingRepository = IocManager.Instance.Resolve<IRepository<BillInvoiceSetting, long>>();

            _baseUrl = (IocManager.Instance.Resolve<IWebUrlService>()).GetServerRootAddress().EnsureEndsWith('/');
            _inventoryTransactionItemRepository = inventoryTransactionItemRepository;
            _itemReceiptCustomerCreditItemBatchNoRepository = itemReceiptCustomerCreditItemBatchNoRepository;
            _customerCreditItemBatchNoRepository = customerCreditItemBatchNoRepository;
            _journalTransactionTypeManager = journalTransactionTypeManager;
            _saleTypeRepository = saleTypeRepository;
            _exchangeRateRepository = exchangeRateRepository;
            _customerCreditExchangeRateRepository = customerCreditExchangeRateRepository;
            _deliveryScheduleItemRepository = deliveryScheduleItemRepository;
            _deliveryScheduleRepository = deliveryScheduleRepository;

        }

        private async Task ValidateBatchNo(CreateInvoiceInput input)
        {
            if (!input.InvoiceItems.Any(r => r.ItemId.HasValue && r.ItemId != Guid.Empty)) return;

            var validateItems = await _itemRepository.GetAll()
                            .Where(s => input.InvoiceItems.Any(i => i.ItemId == s.Id))
                            .Where(s => s.UseBatchNo || s.TrackSerial || s.TrackExpiration)
                            .AsNoTracking()
                            .ToListAsync();

            if (!validateItems.Any()) return;

            var batchItemDic = validateItems.ToDictionary(k => k.Id, v => v);

            var itemUseBatchs = input.InvoiceItems.Where(s => batchItemDic.ContainsKey(s.ItemId.Value)).ToList();

            var find = itemUseBatchs.Where(s => s.ItemBatchNos == null || s.ItemBatchNos.Count == 0 || s.ItemBatchNos.Any(r => r.BatchNoId == Guid.Empty)).FirstOrDefault();
            if (find != null) throw new UserFriendlyException(L("PleaseSelect", $"{L("BatchNo")}/{L("SerialNo")}") + $", Item: {find.Item.ItemCode}-{find.Item.ItemName}");

            var serialItemHash = validateItems.Where(s => s.TrackSerial).Select(s => s.Id).ToHashSet();
            var serialQty = input.InvoiceItems.Where(s => serialItemHash.Contains(s.ItemId.Value)).FirstOrDefault(s => s.ItemBatchNos.Any(b => b.Qty != 1));
            if (serialQty != null) throw new UserFriendlyException(L("MustBeEqualTo", $"{L("SerialNo")} {L("Qty")}", 1) + $", Item: {batchItemDic[serialQty.ItemId.Value].ItemCode}-{batchItemDic[serialQty.ItemId.Value].ItemName}");

            var batchNoIds = itemUseBatchs.SelectMany(s => s.ItemBatchNos).GroupBy(s => s.BatchNoId).Select(s => s.Key).ToList();
            var batchNoDic = await _batchNoRepository.GetAll().Where(s => batchNoIds.Contains(s.Id)).ToDictionaryAsync(k => k.Id, v => v);
            var notFoundBatch = batchNoIds.FirstOrDefault(s => !batchNoDic.ContainsKey(s));
            if (notFoundBatch != null && notFoundBatch != Guid.Empty) throw new UserFriendlyException(L("RecordNotFound"));

            //duplicate on transaction item use same batch more that one time
            var duplicate = itemUseBatchs.FirstOrDefault(s => s.ItemBatchNos.GroupBy(g => g.BatchNoId).Any(r => r.Count() > 1));
            if (duplicate != null) throw new UserFriendlyException(L("Duplicated", $"{L("BatchNo")}/{L("SerialNo")}" + $" , Item: {batchItemDic[duplicate.ItemId.Value].ItemCode}-{batchItemDic[duplicate.ItemId.Value].ItemName}"));

            var zeroQty = itemUseBatchs.FirstOrDefault(s => s.ItemBatchNos.Any(r => r.Qty <= 0));
            if (zeroQty != null) throw new UserFriendlyException(L("MustBeGreaterThen", L("Qty"), 0));

            var validateQty = itemUseBatchs.FirstOrDefault(s => s.Qty != s.ItemBatchNos.Sum(t => t.Qty));
            if (validateQty != null) throw new UserFriendlyException(L("IsNotValid", L("Qty") + " " + $"{L("BatchNo")}/{L("SerialNo")}" + $", {batchItemDic[validateQty.ItemId.Value].ItemCode}-{batchItemDic[validateQty.ItemId.Value].ItemName}"));

            var itemIds = itemUseBatchs.GroupBy(s => s.ItemId.Value).Select(s => s.Key).ToList();
            var lots = itemUseBatchs.GroupBy(s => s.LotId).Select(s => s.Key.Value).ToList();
            var exceptIds = new List<Guid>();

            if (input.ItemIssueId.HasValue) exceptIds.Add(input.ItemIssueId.Value);

            var batchBalanceItems = await _inventoryManager.GetItemBatchNoBalance(itemIds, lots, batchNoIds, input.Date, exceptIds);
            var balanceDic = batchBalanceItems.ToDictionary(k => $"{k.ItemId}-{k.LotId}-{k.BatchNoId}", v => v.Qty);

            var batchQtyUse = itemUseBatchs
                              .SelectMany(s => s.ItemBatchNos.Select(r => new { s.ItemId, s.LotId, r.BatchNoId, r.Qty }))
                              .GroupBy(s => new { s.ItemId, s.LotId, s.BatchNoId })
                              .Select(s => new
                              {
                                  ItemName = $"{batchItemDic[s.Key.ItemId.Value].ItemCode}-{batchItemDic[s.Key.ItemId.Value].ItemName}, BatchNo: {batchNoDic[s.Key.BatchNoId].Code}",
                                  Key = $"{s.Key.ItemId}-{s.Key.LotId}-{s.Key.BatchNoId}",
                                  Qty = s.Sum(t => t.Qty)
                              })
                              .ToList();

            var notInStock = batchQtyUse.FirstOrDefault(s => !balanceDic.ContainsKey(s.Key));
            if (notInStock != null) throw new UserFriendlyException(L("ItemOutOfStock", notInStock.ItemName));

            var issueOverQty = batchQtyUse.FirstOrDefault(s => balanceDic.ContainsKey(s.Key) && s.Qty > balanceDic[s.Key]);
            if (issueOverQty != null)
                throw new UserFriendlyException(L("CannotIssueForThisBiggerStockItem",
                    issueOverQty.ItemName,
                    String.Format("{0:n0}", balanceDic[issueOverQty.Key]),
                    String.Format("{0:n0}", issueOverQty.Qty)));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_Create)]
        public async Task<FileDto> CreateAndPrint(CreateInvoiceInput input)
        {
            var saveInvoice = await SaveInvoice(input);
            var result = new EntityDto<Guid>() { Id = saveInvoice.Id.Value };
            var print = await Print(result);
            return print;
        }

        public async Task<FileDto> Print(EntityDto<Guid> input)
        {
            return await PrintTemplate(input);

            //var detail = await GetDetail(input);
            //var accountCyclePeriod = await GetPreviousRoundingCloseCyleAsync(detail.Date);
            //return await Task.Run(() =>
            //{

            //    var exportHtml = string.Empty;
            //    var templateHtml = string.Empty;
            //    FileDto fileDto = new FileDto()
            //    {
            //        SubFolder = null,
            //        FileName = "invoicePrint.pdf",
            //        FileToken = "invoicePrint.html",
            //        FileType = MimeTypeNames.TextHtml
            //    };
            //    try
            //    {
            //        templateHtml = ReadTemplateFile(fileDto);//retrive template from path
            //        templateHtml = templateHtml.Trim();
            //    }
            //    catch (FileNotFoundException)
            //    {
            //        throw new UserFriendlyException("FileNotFound");
            //    }
            //    exportHtml = templateHtml;
            //    var dateFormat = FormatDate(detail.Date, "dd/MM/yyyy");//@todo take date format from tenant setup
            //    var saleOrderNo = "";
            //    var issueNo = detail.ItemIssueNo;
            //    var No = 1;
            //    var contentBody = string.Empty;

            //    if (detail.InvoiceItems.Where(t => t.Item == null).Count() == 0)
            //    {
            //        exportHtml = exportHtml.Replace("{{itemHeaderKey}}", "លេខកូដទំនិញ");
            //        exportHtml = exportHtml.Replace("{{itemCodeKey}}", "Item Code");

            //        exportHtml = exportHtml.Replace("{{itemHeaderKeyName}}", "ឈ្មោះទំនិញ");
            //        exportHtml = exportHtml.Replace("{{itemCodeKeyName}}", "Item Name");
            //    }
            //    else
            //    {

            //        exportHtml = exportHtml.Replace("{{itemHeaderKey}}", "លេខកូដគណនី");
            //        exportHtml = exportHtml.Replace("{{itemCodeKey}}", "Account Code");
            //        exportHtml = exportHtml.Replace("{{itemHeaderKeyName}}", "ឈ្មោះគណនី");
            //        exportHtml = exportHtml.Replace("{{itemCodeKeyName}}", "Account Name");
            //    }

            //    foreach (var i in detail.InvoiceItems)
            //    {
            //        saleOrderNo = i.SaleOrder != null ? i.SaleOrder.OrderNumber : null;
            //        var item = "<tr>";
            //        //if (No % 10 == 1 && No > 1) {
            //        //    item = "<tr stype='page-break-before: always; page-break-after: always;'>";
            //        //} 

            //        item += $"<td align='center'>" + No + "</td>";
            //        if (i.Item != null)
            //        {
            //            item += $"<td align='center'>" + i.Item.ItemCode + "</td>";
            //            item += $"<td>" + i.Item.ItemName + "</td>";

            //        }
            //        else
            //        {
            //            item += $"<td align='center'>" + i.Account.AccountCode + "</td>";
            //            item += $"<td>" + i.Account.AccountName + "</td>";
            //        }
            //        item += $"<td align='center'>" + FormatNumberCurrency(i.Qty, accountCyclePeriod.RoundingDigit) + "</td>";
            //        item += $"<td align='right'>" + FormatNumberCurrency(i.MultiCurrencyUnitCost, accountCyclePeriod.RoundingDigit) + "</td>";
            //        item += $"<td align='right'>" + FormatNumberCurrency(i.MultiCurrencyTotal, accountCyclePeriod.RoundingDigit) + "</td>";
            //        item += "</tr>";
            //        contentBody += item;
            //        No++;
            //    }
            //    var length = detail.InvoiceItems.Count.ToString();
            //    int lastNumber = Int16.Parse(length.Substring(length.Length - 1, 1));
            //    contentBody += AddBlankRows(detail.InvoiceItems.Count);

            //    exportHtml = exportHtml.Replace("{{invoiceDate}}", dateFormat);
            //    exportHtml = exportHtml.Replace("{{invoiceNo}}", detail.InvoiceNo);
            //    exportHtml = exportHtml.Replace("{{saleOrderNo}}", saleOrderNo);
            //    exportHtml = exportHtml.Replace("{{issueNo}}", issueNo);
            //    var address = detail.Customer.shippingAddress.PostalCode + " " + detail.Customer.shippingAddress.Street + " " +
            //                detail.Customer.shippingAddress.CityTown + " " + detail.Customer.shippingAddress.Province + " " +
            //                detail.Customer.shippingAddress.Country;
            //    exportHtml = exportHtml.Replace("{{customerAddress}}", address);
            //    exportHtml = exportHtml.Replace("{{customerName}}", detail.Customer.CustomerName);
            //    exportHtml = exportHtml.Replace("{{customerTelephone}}", detail.Customer.PhoneNumber);
            //    exportHtml = exportHtml.Replace("{{rowItem}}", contentBody);
            //    exportHtml = exportHtml.Replace("{{subTotal}}", FormatNumberCurrency(detail.MultiCurrencySubTotal, accountCyclePeriod.RoundingDigit));
            //    exportHtml = exportHtml.Replace("{{deposit}}", FormatNumberCurrency(detail.MultiCurrencyTotalPaid, accountCyclePeriod.RoundingDigit));
            //    exportHtml = exportHtml.Replace("{{balance}}", FormatNumberCurrency(detail.MultiCurrencyOpenBalance, accountCyclePeriod.RoundingDigit));
            //    HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();
            //    // Set if the fonts are embedded in the generated PDF document
            //    // Leave it not set to embed the fonts in the generated PDF document
            //    htmlToPdfConverter.PdfDocumentOptions.EmbedFonts = true;
            //    htmlToPdfConverter.PdfDocumentOptions.FitWidth = true;
            //    htmlToPdfConverter.PdfDocumentOptions.TableHeaderRepeatEnabled = true;
            //    htmlToPdfConverter.PdfDocumentOptions.TableFooterRepeatEnabled = false;
            //    htmlToPdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Landscape;
            //    htmlToPdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A5;
            //    htmlToPdfConverter.PdfDocumentOptions.LeftMargin = 10;
            //    htmlToPdfConverter.PdfDocumentOptions.RightMargin = 10;

            //    htmlToPdfConverter.PdfDocumentOptions.TopMargin = 10;
            //    htmlToPdfConverter.PdfDocumentOptions.BottomMargin = 10;


            //    byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");


            //    var tokenName = $"{Guid.NewGuid()}.pdf";
            //    var result = new FileDto();
            //    result.FileName = $"Invoice_{dateFormat}.pdf";
            //    result.FileToken = Guid.NewGuid().ToString();
            //    result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
            //    result.FileType = MimeTypeNames.ApplicationPdf;
            //    var path = Path.Combine(_appFolders.TempFileDownloadFolder, result.FileToken);
            //    File.WriteAllBytes(path, outPdfBuffer);
            //    return result;

            //});
        }

        private string AddBlankRows(int rows, int range = 9)
        {
            var item = "";
            if (rows <= range)
            {
                var add = range - rows;
                for (var i = 1; i <= add; i++)
                {
                    item += "<tr>";
                    item += "<td align='center'>" + (rows + i) + "</td>" +
                            "<td></td><td></td><td></td><td></td><td></td>";
                    item += "</tr>";
                }
            }
            else
            {
                return AddBlankRows(rows, range + 14);
            }
            return item;
        }


        [UnitOfWork(IsDisabled = true)]
        private async Task<FileDto> PrintTemplate(EntityDto<Guid> input)
        {
            var companyInfo = await _tenantManager.GetAsync(AbpSession.GetTenantId());

            var invoice = await GetDetail(input);
            var accountCyclePeriod = await GetPreviousRoundingCloseCyleAsync(invoice.Date);
            var template = await this.GetInovieTemplateHtml(invoice.TrasactionTypeSaleId);
            var exportHtml = EmbedBase64Fonts(template.Html);

            return await Task.Run(async () =>
            {
                var dateFormat = FormatDate(invoice.Date, "dd-MM-yyyy");
                var canSeePrice = IsGranted(AppPermissions.Pages_Tenant_Customer_Invoice_CanSeePrice);

                if (template.ShowDetail)
                {
                    var rowStart = FindTag(exportHtml, "<tr", " detail-body-row");
                    if (!string.IsNullOrWhiteSpace(rowStart.Key))
                    {
                        var detialRow = GetOuterHtml(rowStart.Key, "</tr>", exportHtml.Substring(rowStart.Value));
                        if (!string.IsNullOrEmpty(detialRow))
                        {
                            int n = 1;
                            var detail = string.Empty;

                            var subItems = invoice.InvoiceItems.Where(s => s.ParentId.HasValue).ToList();
                            var showItems = invoice.InvoiceItems.Where(s => !s.ParentId.HasValue).ToList();

                            foreach (var i in showItems)
                            {
                                var rowContent = detialRow.Replace("@No", $"{n}")
                                                   .Replace("@ItemCode", i.Item.ItemCode)
                                                   .Replace("@ItemName", i.Item.ItemName)
                                                   .Replace("@Description", i.Description)
                                                   .Replace("@Qty", i.Qty.ToString("#,##0.########"))
                                                   .Replace("@UnitPrice", !canSeePrice ? "..." : FormatNumberCurrency(i.MultiCurrencyUnitCost, accountCyclePeriod.RoundingDigitUnitCost))
                                                   .Replace("@DiscountRate", !canSeePrice ? "..." : $"{FormatNumberCurrency(i.DiscountRate * 100, accountCyclePeriod.RoundingDigit)}%")
                                                   .Replace("@LineTotal", !canSeePrice ? "..." : FormatNumberCurrency(i.MultiCurrencyTotal, accountCyclePeriod.RoundingDigit));

                                if (i.Key.HasValue && i.Item.ShowSubItems)
                                {
                                    detail += rowContent.Replace("border-b-dot", "parent-row");

                                    var subs = subItems.Where(s => s.ParentId == i.Key).ToList();
                                    var subIndex = 1;
                                    foreach (var sub in subs)
                                    {

                                        var subContent = detialRow.Replace("@No", $"")
                                                           .Replace("@ItemCode", sub.Item.ItemCode)
                                                           .Replace("@ItemName", sub.Item.ItemName)
                                                           .Replace("@Description", sub.Description)
                                                           .Replace("@Qty", sub.Qty.ToString("#,##0.########"))
                                                           .Replace("@UnitPrice", $"...")
                                                           .Replace("@DiscountRate", $"...")
                                                           .Replace("@LineTotal", $"...");

                                        detail += subIndex < subs.Count ? subContent.Replace("border-b-dot", "sub-row") : subContent.Replace("border-b-dot", "border-b-dot sub-row last");

                                        subIndex++;
                                    }
                                }
                                else
                                {
                                    detail += rowContent;
                                }

                                n++;
                            }

                            exportHtml = exportHtml.Replace(detialRow, detail);
                        }
                    }
                }

                if (template.ShowSummary)
                {
                    var paymentSummaryTag = FindTag(exportHtml, "<tbody", " payment-summary-body");
                    if (paymentSummaryTag.Key != "")
                    {
                        var paymentSummaryBody = GetOuterHtml(paymentSummaryTag.Key, "</tbody>", exportHtml);
                        if (!string.IsNullOrWhiteSpace(paymentSummaryBody))
                        {
                            var summaryGroupRow = string.Empty;
                            var summaryGroupTag = FindTag(paymentSummaryBody, "<tr", " payment-summary-group-row");
                            if (summaryGroupTag.Key != "")
                            {
                                summaryGroupRow = GetOuterHtml(summaryGroupTag.Key, "</tr>", paymentSummaryBody);
                            }

                            var summaryItemRow = string.Empty;
                            var summaryItemTag = FindTag(paymentSummaryBody, "<tr", " payment-summary-item-row");
                            if (summaryItemTag.Key != "")
                            {
                                summaryItemRow = GetOuterHtml(summaryItemTag.Key, "</tr>", paymentSummaryBody);
                            }

                            var summaryResule = await GetPaymentSummary(new PaymentSummaryGetListInput
                            {
                                InvoiceId = input.Id,
                                InvoiceDate = invoice.Date,
                                CreationTimeIndex = invoice.CreationTimeIndex.Value,
                                CustomerId = invoice.CustomerId
                            });

                            var summary = string.Empty;
                            foreach (var g in summaryResule.Items)
                            {
                                foreach (var p in g.Items)
                                {
                                    if (!string.IsNullOrWhiteSpace(summaryItemRow))
                                    {
                                        summary += summaryItemRow.Replace("@Date", FormatDate(p.Date, "dd-MM-yyyy"))
                                                                 .Replace("@Description", p.Description)
                                                                 .Replace("@SummaryTotal", FormatNumberCurrency(p.Total, accountCyclePeriod.RoundingDigit));
                                    }
                                }

                                if (!string.IsNullOrWhiteSpace(summaryGroupRow))
                                {
                                    summary += summaryGroupRow.Replace("@GroupCurrency", g.CurrencyCode)
                                                              .Replace("@GroupTotal", FormatNumberCurrency(g.Total, accountCyclePeriod.RoundingDigit));
                                }

                            }

                            exportHtml = exportHtml.Replace(paymentSummaryBody, summary);
                        }

                    }
                }

                var logo = "";
                if (companyInfo.LogoId.HasValue)
                {
                    var image = await _fileUploadManager.DownLoad(companyInfo.Id, companyInfo.LogoId.Value);

                    if (image != null)
                    {
                        var base64Str = StreamToBase64(image.Stream);
                        logo = $"<img src=\"data:{image.ContentType};base64, {base64Str}\" alt=\"logo\" style=\"max-height: 90px; max-width: 150px; display: block;\"/>";
                    }
                }

                var shippingAddress = "";
                if (invoice.ShippingAddress != null)
                {
                    shippingAddress = $"{(invoice.ShippingAddress.Street.IsNullOrWhiteSpace() ? "....." : invoice.ShippingAddress.Street)}, " +
                    $"{(invoice.ShippingAddress.PostalCode.IsNullOrWhiteSpace() ? "....." : invoice.ShippingAddress.PostalCode)}, " +
                    $"{(invoice.ShippingAddress.CityTown.IsNullOrWhiteSpace() ? "....." : invoice.ShippingAddress.CityTown)}, " +
                    $"{(invoice.ShippingAddress.Province.IsNullOrWhiteSpace() ? "....." : invoice.ShippingAddress.Province)}, " +
                    $"{(invoice.ShippingAddress.Country.IsNullOrWhiteSpace() ? "....." : invoice.ShippingAddress.Country)}";
                }

                exportHtml = exportHtml.Replace("@Logo", logo)
                                       .Replace("@CompanyName", companyInfo.Name)
                                       .Replace("@CompanyAddress", $"{companyInfo.LegalAddress?.Street} {companyInfo.LegalAddress?.CityTown} {companyInfo.LegalAddress?.Province}")
                                       .Replace("@CustomerName", invoice.Customer?.CustomerName)
                                       .Replace("@CustomerPhone", invoice.Customer?.PhoneNumber)
                                       .Replace("@ShippingAddress", shippingAddress)
                                       .Replace("@InvoiceDate", dateFormat)
                                       .Replace("@ETADate", FormatDate(invoice.ETD, "dd-MM-yyyy"))
                                       .Replace("@DueDate", FormatDate(invoice.DueDate, "dd-MM-yyyy"))
                                       .Replace("@IssueDate", invoice.ItemIssueDate.HasValue ? FormatDate(invoice.ItemIssueDate.Value, "dd-MM-yyyy") : "")
                                       .Replace("@InvoiceNo", invoice.InvoiceNo)
                                       .Replace("@Reference", invoice.Reference)
                                       .Replace("@Currency", invoice.MultiCurrency?.Code)
                                       .Replace("@Memo", invoice.Memo == null ? "" : invoice.Memo.Replace(Environment.NewLine, "<br>").Replace("\n", "<br>"))
                                       .Replace("@UserName", invoice.UserName)
                                       .Replace("@SubTotal", !canSeePrice ? "..." : FormatNumberCurrency(invoice.MultiCurrencySubTotal, accountCyclePeriod.RoundingDigit))
                                       .Replace("@PaymentAmount", !canSeePrice ? "..." : FormatNumberCurrency(invoice.MultiCurrencyTotalPaid, accountCyclePeriod.RoundingDigit))
                                       .Replace("@Balance", !canSeePrice ? "..." : FormatNumberCurrency(invoice.MultiCurrencyOpenBalance, accountCyclePeriod.RoundingDigit));


                HtmlToPdfConverter htmlToPdfConverter = GetInitPDF();
                htmlToPdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Portrait;
                htmlToPdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
                htmlToPdfConverter.PdfDocumentOptions.LeftMargin = 20;
                htmlToPdfConverter.PdfDocumentOptions.RightMargin = 20;
                htmlToPdfConverter.PdfDocumentOptions.TopMargin = 20;
                htmlToPdfConverter.PdfDocumentOptions.BottomMargin = 20;


                byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");

                var result = new FileDto();
                result.FileName = $"Invoice_{dateFormat}.pdf";
                result.FileToken = $"{Guid.NewGuid()}.pdf";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                result.FileType = MimeTypeNames.ApplicationPdf;

                await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);

                return result;

            });
        }

        [UnitOfWork(IsDisabled = true)]
        private async Task<InvoiceTemplateWithOptionResultOutput> GetInovieTemplateHtml(long? saleTypeId)
        {
            InvoiceTemplateMap templateMap = null;


            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(AbpSession.TenantId))
                {
                    templateMap = await _invoiceTemplateMapRepository
                                    .GetAll()
                                    .Include(s => s.InvoiceTemplate)
                                    .Where(s => s.TemplateType == InvoiceTemplateType.Invoice)
                                    .Where(s => s.SaleTypeId == saleTypeId || !s.SaleTypeId.HasValue)
                                    .AsNoTracking()
                                    .OrderBy(s => s.SaleTypeId.HasValue ? 0 : 1)
                                    .FirstOrDefaultAsync();

                }
            }

            if (templateMap == null) return await GetDefaultTemplateHtml("invoiceTemplate.html");

            GalleryDownloadOutput fileDownload = await _fileUploadManager.DownLoad(AbpSession.TenantId, templateMap.InvoiceTemplate.GalleryId);

            var templateHtml = string.Empty;
            using (StreamReader r = new StreamReader(fileDownload.Stream))
            {
                templateHtml = r.ReadToEnd();
            }

            return new InvoiceTemplateWithOptionResultOutput
            {
                Html = templateHtml,
                ShowDetail = templateMap.InvoiceTemplate.ShowDetail,
                ShowSummary = templateMap.InvoiceTemplate.ShowSummary
            };


        }

        [UnitOfWork(IsDisabled = true)]
        private async Task<InvoiceTemplateWithOptionResultOutput> GetDefaultTemplateHtml(string templateFileName)
        {
            var templateHtml = await _fileStorageManager.GetTemplate(templateFileName);

            return new InvoiceTemplateWithOptionResultOutput { Html = templateHtml, ShowDetail = true };
        }

        private async Task<ListResultDto<PaymentSummaryResultOutput>> GetPaymentSummary(PaymentSummaryGetListInput input)
        {
            var invoiceQuery = from j in _journalRepository.GetAll()
                                        .Where(s => s.Status == TransactionStatus.Publish)
                                        .Where(s => s.Date.Date <= input.InvoiceDate.Date)
                                        .Where(s => s.CreationTimeIndex <= input.CreationTimeIndex)
                                        .Where(s => (s.InvoiceId.HasValue && s.Invoice.CustomerId == input.CustomerId) ||
                                                    (s.CustomerCreditId.HasValue && s.CustomerCredit.CustomerId == input.CustomerId))
                                        .AsNoTracking()
                               select new
                               {
                                   j.Id,
                                   Date = j.Date,
                                   j.InvoiceId,
                                   j.CustomerCreditId,
                                   JournalType = j.JournalType,
                                   CurrencyCode = j.MultiCurrency.Code,
                                   Total = j.InvoiceId.HasValue ? j.Invoice.MultiCurrencyTotal : j.CustomerCreditId.HasValue ? -j.CustomerCredit.MultiCurrencyTotal : 0,
                               };

            var summaryList = await invoiceQuery.Where(s => s.InvoiceId != input.InvoiceId)
                                    .GroupBy(s => new { s.CurrencyCode, s.JournalType })
                                    .Select(g => new PaymentSummaryDto
                                    {
                                        Date = g.Max(s => s.Date),
                                        CurrencyCode = g.Key.CurrencyCode,
                                        JournalType = g.Key.JournalType.ToString(),
                                        Total = g.Sum(s => s.Total),
                                        Description = g.Key.JournalType == JournalType.CustomerCredit ? L(JournalType.CustomerCredit.ToString()) : L("TotalDueAmount")
                                    })
                                    .ToListAsync();

            var paymentQuery = from p in _receivePaymentRepository.GetAll()
                                         .AsNoTracking()
                               join pi in _receivePaymentDetailRepository.GetAll()
                                           .Where(s => s.CustomerId == input.CustomerId)
                                           .AsNoTracking()
                               on p.Id equals pi.ReceivePaymentId

                               join j in _journalRepository.GetAll()
                                        .Where(s => s.Date.Date <= input.InvoiceDate.Date)
                                        .Where(s => s.CreationTimeIndex <= input.CreationTimeIndex)
                                        .Where(s => s.ReceivePaymentId.HasValue)
                                        .AsNoTracking()
                               on p.Id equals j.ReceivePaymentId

                               join icj in _journalRepository.GetAll()
                                      .Where(s => s.Date.Date <= input.InvoiceDate.Date)
                                      .Where(s => s.CreationTimeIndex <= input.CreationTimeIndex)
                                      .Where(s => s.InvoiceId.HasValue || s.CustomerCreditId.HasValue)
                                      .AsNoTracking()
                               on new { pi.InvoiceId, pi.CustomerCreditId } equals new { icj.InvoiceId, icj.CustomerCreditId }

                               select new
                               {
                                   pi.InvoiceId,
                                   pi.CustomerCreditId,
                                   Payment = pi.InvoiceId.HasValue ? pi.MultiCurrencyPayment : -pi.MultiCurrencyPayment,
                                   CurrencyCode = icj.MultiCurrency.Code,
                                   JournalType = icj.JournalType
                               };

            var paymentList = await paymentQuery.Where(s => s.InvoiceId != input.InvoiceId)
                                    .GroupBy(s => new { s.CurrencyCode, s.JournalType })
                                    .Select(g => new
                                    {
                                        CurrencyCode = g.Key.CurrencyCode,
                                        JournalType = g.Key.JournalType.ToString(),
                                        Payment = g.Sum(s => s.Payment),
                                    })
                                    .ToListAsync();


            var invoice = await invoiceQuery.Where(s => s.InvoiceId == input.InvoiceId).FirstOrDefaultAsync();
            var invoicePayments = await paymentQuery.Where(s => s.InvoiceId == input.InvoiceId).ToListAsync();

            var resultList = new List<PaymentSummaryResultOutput>();

            var currencies = summaryList.GroupBy(s => s.CurrencyCode).Select(s => s.Key).ToList();
            if (!currencies.Any(s => s == invoice.CurrencyCode)) currencies.Add(invoice.CurrencyCode);

            foreach (var c in currencies)
            {
                var item = new PaymentSummaryResultOutput
                {
                    CurrencyCode = c,
                    Items = new List<PaymentSummaryDto>()
                };

                var invocieItem = summaryList.Where(s => s.CurrencyCode == c && s.JournalType == JournalType.Invoice.ToString()).FirstOrDefault();
                if (invocieItem != null)
                {
                    var pay = paymentList.Where(s => s.CurrencyCode == c && s.JournalType == JournalType.Invoice.ToString()).FirstOrDefault();
                    if (pay != null) invocieItem.Total = invocieItem.Total - pay.Payment;

                    if (invocieItem.Total != 0) item.Items.Add(invocieItem);
                }

                var creditItem = summaryList.Where(s => s.CurrencyCode == c && s.JournalType == JournalType.CustomerCredit.ToString()).FirstOrDefault();
                if (creditItem != null)
                {
                    var pay = paymentList.Where(s => s.CurrencyCode == c && s.JournalType == JournalType.CustomerCredit.ToString()).FirstOrDefault();
                    if (pay != null) creditItem.Total = creditItem.Total - pay.Payment;

                    if (creditItem.Total != 0) item.Items.Add(creditItem);
                }

                if (c == invoice.CurrencyCode)
                {
                    item.Items.Add(new PaymentSummaryDto
                    {
                        Date = invoice.Date,
                        Description = L("InvoiceAmount"),
                        JournalType = invoice.JournalType.ToString(),
                        Total = invoice.Total
                    });

                    if (invoicePayments.Any())
                    {
                        item.Items.Add(new PaymentSummaryDto
                        {
                            Date = invoice.Date,
                            Description = L("Payment"),
                            JournalType = JournalType.ReceivePayment.ToString(),
                            Total = invoicePayments.Sum(s => s.Payment)
                        });
                    }
                }

                item.Total = item.Items.Sum(s => s.Total);

                if (item.Total != 0) resultList.Add(item);
            }

            return new ListResultDto<PaymentSummaryResultOutput> { Items = resultList };
        }

        private async Task ValidateOrderQtyForIssue(Guid OrderItemId, decimal qty, Guid? itemIssueId)
        {
            var saleOrderItem = await _saleOrderItemRepository.GetAll()
                                       .Include(s => s.Item)
                                       .Include(s => s.ItemIssueItems)
                                       .Include(s => s.InvoiceItems)
                                       .Where(s => s.Id == OrderItemId)
                                       .AsNoTracking()
                                       .FirstOrDefaultAsync();

            var invoiceDeliveryQty = saleOrderItem.InvoiceItems.Sum(s => s.Qty);
            var issueDeliveryQty = saleOrderItem.ItemIssueItems.Where(s => s.ItemIssueId != itemIssueId).Sum(s => s.Qty);
            if (saleOrderItem.Qty - invoiceDeliveryQty - issueDeliveryQty + qty < 0)
                throw new UserFriendlyException(L("InvoiceMessageWarning", saleOrderItem.Item.ItemName));
        }

        void ValidateExchangeRate(CreateInvoiceInput input)
        {
            if (!input.UseExchangeRate || input.CurrencyId == input.MultiCurrencyId) return;

            if (input.ExchangeRate == null) throw new UserFriendlyException(L("IsRequired", L("ExchangeRate")));
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_Create)]
        public async Task<NullableIdDto<Guid>> Create(CreateInvoiceInput input)
        {
            var result = await SaveInvoice(input);
            return new NullableIdDto<Guid>() { Id = result.Id };

        }

        private async Task CalculateTotal(CreateInvoiceInput input)
        {
            #region Calculation Formular from Front End
            //this.model.subTotal = 0;
            //this.model.tax = 0;
            //for (let i of this.itemList)
            //{
            //    let subTotal = this.trimRoundValue(i.qty * i.unitCost);
            //    let totaldiscount = this.trimRoundValue((subTotal * i.discountRate) / 100);

            //    i.total = this.trimRoundValue(subTotal - totaldiscount);// get total of item
            //    let taxRate = i.tax ? i.tax.taxRate : 0;
            //    let totalTaxRate = this.trimRoundValue(i.total * taxRate);//get total tax of item

            //    this.model.subTotal += i.total;
            //    this.model.tax += totalTaxRate;
            //}
            //this.model.total = this.model.subTotal + this.model.tax;
            #endregion

            var round = await GetCurrentCycleAsync();

            decimal total = 0;
            decimal totalTax = 0;
            foreach (var item in input.InvoiceItems)
            {
                item.UnitCost = Math.Round(item.UnitCost, round.RoundingDigitUnitCost);

                var lineCost = Math.Round(item.Qty * item.UnitCost, round.RoundingDigit);
                var discount = Math.Round(lineCost * item.DiscountRate / 100, round.RoundingDigit);

                item.Total = Math.Round(lineCost - discount, round.RoundingDigit);

                var taxRate = item.Tax != null ? item.Tax.TaxRate : 0;
                var tax = Math.Round(item.Total * taxRate, round.RoundingDigit);

                total += item.Total;
                totalTax += tax;
            }

            input.SubTotal = total;
            input.Tax = totalTax;
            input.Total = input.SubTotal + input.Tax;
        }

        private async Task ValidateApplyOrderItems(CreateInvoiceInput input)
        {

            var itemApplyOrder = input.InvoiceItems
                                 .Where(s => s.OrderItemId.HasValue)
                                 .GroupBy(s => s.OrderItemId);

            if (!itemApplyOrder.Any()) return;

            var orderItemIds = itemApplyOrder.Select(s => s.Key.Value).ToList();
            var exceptInvoiceItemIds = input.InvoiceItems
                                        .Where(s => s.OrderItemId.HasValue)
                                        .Where(s => s.Id.HasValue)
                                        .Select(s => s.Id.Value)
                                        .ToList();

            var exceptIssueItemIds = input.InvoiceItems
                                        .Where(s => s.OrderItemId.HasValue)
                                        .Where(s => s.ItemIssueId.HasValue)
                                        .Select(s => s.ItemIssueId.Value)
                                        .ToList();

            var invoiceItemQuery = from iv in _invoiceItemRepository.GetAll()
                                             .Where(s => s.OrderItemId.HasValue)
                                             .Where(s => orderItemIds.Contains(s.OrderItemId.Value))
                                             .Where(s => !exceptInvoiceItemIds.Contains(s.Id))
                                             .AsNoTracking()
                                   join ri in _itemReceiptItemCustomerCreditRepository.GetAll()
                                              .Where(s => s.ItemIssueSaleItemId.HasValue)
                                              .AsNoTracking()
                                   on iv.ItemIssueItemId equals ri.ItemIssueSaleItemId
                                   into r1

                                   join ci in _itemReceiptItemCustomerCreditRepository.GetAll()
                                              .Where(s => s.CustomerCreditItemId.HasValue)
                                              .AsNoTracking()
                                   on iv.ItemIssueItemId equals ci.CustomerCreditItem.ItemIssueSaleItemId
                                   into r2

                                   let rq1 = r1 == null ? 0 : r1.Sum(s => s.Qty)
                                   let rq2 = r2 == null ? 0 : r2.Sum(s => s.Qty)

                                   select new
                                   {
                                       OrderItemId = iv.OrderItemId,
                                       Qty = iv.Qty,
                                       ReturnQty = rq1 + rq2
                                   };

            var issueItemQuery = from iv in _itemIssueItemRepository.GetAll()
                                             .Where(s => s.SaleOrderItemId.HasValue)
                                             .Where(s => orderItemIds.Contains(s.SaleOrderItemId.Value))
                                             .Where(s => !exceptIssueItemIds.Contains(s.Id))
                                             .AsNoTracking()
                                 join ri in _itemReceiptItemCustomerCreditRepository.GetAll()
                                            .Where(s => s.ItemIssueSaleItemId.HasValue)
                                            .AsNoTracking()
                                 on iv.Id equals ri.ItemIssueSaleItemId
                                 into r1

                                 join ci in _itemReceiptItemCustomerCreditRepository.GetAll()
                                            .Where(s => s.CustomerCreditItemId.HasValue)
                                            .AsNoTracking()
                                 on iv.Id equals ci.CustomerCreditItem.ItemIssueSaleItemId
                                 into r2

                                 let rq1 = r1 == null ? 0 : r1.Sum(s => s.Qty)
                                 let rq2 = r2 == null ? 0 : r2.Sum(s => s.Qty)

                                 select new
                                 {
                                     SaleOrderItemId = iv.SaleOrderItemId,
                                     Qty = iv.Qty,
                                     ReturnQty = rq1 + rq2
                                 };

            var deliveryItemQuery = from dv in _deliveryScheduleItemRepository.GetAll()
                                      .Where(s => s.SaleOrderItemId.HasValue)
                                      .Where(s => orderItemIds.Contains(s.SaleOrderItemId.Value))                                     
                                      .AsNoTracking()
                                    select new
                                    {
                                        OrderItemId = dv.SaleOrderItemId,
                                        Qty = dv.Qty,
                                        ReturnQty = 0
                                    };

            var saleOrderQuery = from oi in _saleOrderItemRepository.GetAll()
                                            .Where(s => orderItemIds.Contains(s.Id))
                                            .AsNoTracking()
                                 join iv in invoiceItemQuery
                                 on oi.Id equals iv.OrderItemId
                                 into ivItems

                                 join si in issueItemQuery
                                 on oi.SaleOrderId equals si.SaleOrderItemId
                                 into siItems

                                 join di in deliveryItemQuery on oi.Id equals di.OrderItemId
                                into diItems

                                 let q1 = ivItems == null ? 0 : ivItems.Sum(s => s.Qty - s.ReturnQty)
                                 let q2 = siItems == null ? 0 : siItems.Sum(s => s.Qty - s.ReturnQty)
                                 let q3 = diItems == null ? 0 : diItems.Sum(s => s.Qty - s.ReturnQty)
                                 select new
                                 {
                                     oi.Id,
                                     oi.Qty,
                                     issueQty = q1 + q2 + q3
                                 };

            var saleOrderItems = await saleOrderQuery.ToListAsync();


            foreach (var g in itemApplyOrder)
            {
                var orderItem = saleOrderItems.FirstOrDefault(s => s.Id == g.Key);
                if (orderItem == null) throw new UserFriendlyException("RecordNotFound");

                var remainQty = orderItem.Qty - orderItem.issueQty;

                foreach (var i in g)
                {
                    if (i.Qty > remainQty) throw new UserFriendlyException(L("RemainQtyIsLessThanInvoiceQtyCannotApply", remainQty, i.Qty));
                    remainQty -= i.Qty;
                }
            }
        }



        private async Task ValidateApplyDeliveryScheduleItems(CreateInvoiceInput input)
        {

            var itemApplyDelivery = input.InvoiceItems
                                 .Where(s => s.DeliveryScheduleItemId.HasValue)
                                 .GroupBy(s => s.DeliveryScheduleItemId);

            if (!itemApplyDelivery.Any()) return;

            var deliveryItemIds = itemApplyDelivery.Select(s => s.Key.Value).ToList();
            var exceptInvoiceItemIds = input.InvoiceItems
                                        .Where(s => s.DeliveryScheduleItemId.HasValue)
                                        .Where(s => s.Id.HasValue)
                                        .Select(s => s.Id.Value)
                                        .ToList();

            var exceptIssueItemIds = input.InvoiceItems
                                        .Where(s => s.DeliveryScheduleItemId.HasValue)
                                        .Where(s => s.Id.HasValue)
                                        .Select(s => s.Id.Value)
                                        .ToList();

            var invoiceItemQuery = from iv in _invoiceItemRepository.GetAll()
                                             .Where(s => s.DeliverySchedulItemId.HasValue)
                                             .Where(s => deliveryItemIds.Contains(s.DeliverySchedulItemId.Value))
                                             .Where(s => !exceptInvoiceItemIds.Contains(s.Id))
                                             .AsNoTracking()
                                   join ri in _itemReceiptItemCustomerCreditRepository.GetAll()
                                              .Where(s => s.ItemIssueSaleItemId.HasValue)
                                              .AsNoTracking()
                                   on iv.ItemIssueItemId equals ri.ItemIssueSaleItemId
                                   into r1

                                   join ci in _itemReceiptItemCustomerCreditRepository.GetAll()
                                              .Where(s => s.CustomerCreditItemId.HasValue)
                                              .AsNoTracking()
                                   on iv.ItemIssueItemId equals ci.CustomerCreditItem.ItemIssueSaleItemId
                                   into r2

                                   let rq1 = r1 == null ? 0 : r1.Sum(s => s.Qty)
                                   let rq2 = r2 == null ? 0 : r2.Sum(s => s.Qty)

                                   select new
                                   {
                                       DeliverySchedulItemId = iv.DeliverySchedulItemId,
                                       Qty = iv.Qty,
                                       ReturnQty = rq1 + rq2
                                   };

            var issueItemQuery = from iv in _itemIssueItemRepository.GetAll()
                                             .Where(s => s.DeliverySchedulItemId.HasValue)
                                             .Where(s => deliveryItemIds.Contains(s.DeliverySchedulItemId.Value))
                                             .Where(s => !exceptIssueItemIds.Contains(s.Id))
                                             .AsNoTracking()
                                 join ri in _itemReceiptItemCustomerCreditRepository.GetAll()
                                            .Where(s => s.ItemIssueSaleItemId.HasValue)
                                            .AsNoTracking()
                                 on iv.Id equals ri.ItemIssueSaleItemId
                                 into r1

                                 join ci in _itemReceiptItemCustomerCreditRepository.GetAll()
                                            .Where(s => s.CustomerCreditItemId.HasValue)
                                            .AsNoTracking()
                                 on iv.Id equals ci.CustomerCreditItem.ItemIssueSaleItemId
                                 into r2

                                 let rq1 = r1 == null ? 0 : r1.Sum(s => s.Qty)
                                 let rq2 = r2 == null ? 0 : r2.Sum(s => s.Qty)

                                 select new
                                 {
                                     DeliverySchedulItemId = iv.DeliverySchedulItemId,
                                     Qty = iv.Qty,
                                     ReturnQty = rq1 + rq2
                                 };



            var deliveryScheduleQuery = from oi in _deliveryScheduleItemRepository.GetAll()
                                            .Where(s => deliveryItemIds.Contains(s.Id))
                                            .AsNoTracking()
                                        join iv in invoiceItemQuery
                                        on oi.Id equals iv.DeliverySchedulItemId
                                        into ivItems

                                        join si in issueItemQuery
                                        on oi.Id equals si.DeliverySchedulItemId
                                        into siItems


                                        let q1 = ivItems == null ? 0 : ivItems.Sum(s => s.Qty - s.ReturnQty)
                                        let q2 = siItems == null ? 0 : siItems.Sum(s => s.Qty - s.ReturnQty)

                                        select new
                                        {
                                            oi.Id,
                                            oi.Qty,
                                            issueQty = q1 + q2
                                        };

            var deliveryScheduleItems = await deliveryScheduleQuery.ToListAsync();


            foreach (var g in itemApplyDelivery)
            {
                var orderItem = deliveryScheduleItems.FirstOrDefault(s => s.Id == g.Key);
                if (orderItem == null) throw new UserFriendlyException("RecordNotFound");

                var remainQty = orderItem.Qty - orderItem.issueQty;

                foreach (var i in g)
                {
                    if (i.Qty > remainQty) throw new UserFriendlyException(L("RemainQtyIsLessThanInvoiceQtyCannotApply", remainQty, i.Qty));
                    remainQty -= i.Qty;
                }
            }
        }



        protected async Task<NullableIdDto<Guid>> SaveInvoice(CreateInvoiceInput input)
        {
            //validate invoice Item when create by none
            if (input.InvoiceItems.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }


            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                                       .Where(t => (t.LockKey == TransactionLockType.Invoice)
                                       && t.IsLock == true && t.LockDate != null && t.LockDate.Value.Date >= input.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            //validation Qty on hand form SO
            if (input.ReceiveFrom == ReceiveFrom.SaleOrder)
            {
                var validates = input.InvoiceItems.Where(t => t.OrderItemId != null).Select(t => new { ItemName = t.ItemName, OriginalQty = t.OrginalQtyFromSaleOrder, OrderItemId = t.OrderItemId }).GroupBy(t => t.OrderItemId).ToList();
                foreach (var v in validates)
                {
                    var sumQtyByOrderItems = input.InvoiceItems.Where(g => g.OrderItemId == v.Key).Sum(t => t.Qty);
                    if (sumQtyByOrderItems > v.FirstOrDefault().OriginalQty)
                    {
                        throw new UserFriendlyException(L("InvoiceMessageWarning", v.First().ItemName));
                    }
                }
            }

            else if(input.ReceiveFrom == ReceiveFrom.DeliverySchedule)
            {
                var validates = input.InvoiceItems.Where(t => t.DeliveryScheduleItemId != null).Select(t => new { ItemName = t.ItemName, OriginalQty = t.OrginalQtyFromDeliverySchedule, DeliveryScheduleItemId = t.DeliveryScheduleItemId }).GroupBy(t => t.DeliveryScheduleItemId).ToList();
                foreach (var v in validates)
                {
                    var sumQtyByDeliveryItems = input.InvoiceItems.Where(g => g.DeliveryScheduleItemId == v.Key).Sum(t => t.Qty);
                    if (sumQtyByDeliveryItems > v.FirstOrDefault().OriginalQty)
                    {
                        throw new UserFriendlyException(L("InvoiceMessageWarning", v.First().ItemName));
                    }
                }
            }
            else if (input.ReceiveFrom == ReceiveFrom.ItemIssue)
            {
                foreach (var v in input.InvoiceItems.Where(s => s.OrderItemId.HasValue))
                {
                    var sumQtyByOrderItems = input.InvoiceItems.Where(g => g.OrderItemId == v.OrderItemId).Sum(t => t.Qty);
                    await ValidateOrderQtyForIssue(v.OrderItemId.Value, sumQtyByOrderItems, v.ItemIssueId);
                }
            }

            int indexLot = 0;
            foreach (var i in input.InvoiceItems)
            {
                indexLot++;
                if (i.LotId == null && i.ItemId != null && i.DisplayInventoryAccount)
                {
                    throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + indexLot.ToString());
                }
            }

            if (input.ConvertToItemIssue || input.ReceiveFrom == ReceiveFrom.ItemIssue) await ValidateBatchNo(input);

            ValidateExchangeRate(input);
            await ValidateApplyOrderItems(input);
            if (input.ReceiveFrom == ReceiveFrom.DeliverySchedule) await ValidateApplyDeliveryScheduleItems(input);
            await CalculateTotal(input);

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            #region update auto sequence
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Invoice);
            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.InvoiceNo = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }
            #endregion

            #region insert journal for invoice
            var @entity = Journal.Create(tenantId, userId, input.InvoiceNo, input.Date, input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, input.LocationId);
            entity.UpdateStatus(input.Status);

            if (input.MultiCurrencyId == null || input.MultiCurrencyId == 0 || input.CurrencyId == input.MultiCurrencyId)
            {
                input.MultiCurrencyId = input.CurrencyId;
                input.MultiCurrencyTotal = input.Total;
                input.MultiCurrencySubTotal = input.SubTotal;
                input.MultiCurrencyTax = input.Tax;
            }
            entity.UpdateMultiCurrency(input.MultiCurrencyId);
            //insert journal item @todo change after metting requirement
            var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, input.AccountId, input.Memo, input.Total, 0, PostingKey.AR, null);


            var isItem = input.InvoiceItems.Any(s => s.ItemId != null && s.ItemId != Guid.Empty);

            //insert to invoice          
            var invoice = Invoice.Create(tenantId, userId,
                                    input.ReceiveFrom, input.DueDate, input.CustomerId,
                                    input.SameAsShippingAddress,
                                    input.ShippingAddress, input.BillingAddress,
                                    input.SubTotal, input.Tax, input.Total,
                                    input.ItemIssueId, input.ETD, input.IssuingDate, input.ConvertToItemIssue,
                                    input.MultiCurrencySubTotal, input.MultiCurrencyTax, input.MultiCurrencyTotal, isItem, input.UseExchangeRate);
            invoice.UpdateTransactionTypeId(input.TransactionTypeId);

            var itemIssueItems = new List<ItemIssueItem>();
            var @inventoryJournalItems = new List<JournalItem>();
            // check receive from is item issue set automatically received status to full
            if (input.ReceiveFrom == ReceiveFrom.ItemIssue)
            {
                if (input.ItemIssueId == null)
                {
                    throw new UserFriendlyException(L("PleaseAddItemIssue"));
                }

                //var itemIssueJournal = await(from j in _journalRepository.GetAll()
                //                             .Where(t => t.ItemIssueId == input.ItemIssueId)
                //                             join ji in _journalItemRepository.GetAll()
                //                                       .Where(t => t.Key == PostingKey.Inventory)
                //                                       on j.Id equals ji.JournalId
                //                             into u
                //                             select new
                //                             {
                //                                 Journal = j,
                //                                 JournalItem = u,
                //                             }).FirstOrDefaultAsync();

                #region Calculat Cost
                //var roundingId = (await GetCurrentTenantAsync()).RoundDigitAccountId;

                //var itemToCalculateCost = input.InvoiceItems.Where(s => s.Item.ItemTypeId != Item_Type_Service).Select((u, index) => new Inventories.Data.GetAvgCostForIssueData()
                //{
                //    ItemName = u.Item.ItemName,
                //    Index = index,
                //    ItemId = u.ItemId.Value,
                //    ItemCode = u.Item.ItemCode,
                //    Qty = u.Qty,
                //    //ItemIssueItemId = u.Id,
                //    ItemIssueItemId = input.ItemIssueId,
                //    LotId = u.LotId,
                //    InventoryAccountId = itemIssueJournal.JournalItem.Where(t => t.Identifier == u.ItemIssueId).Select(g => g.AccountId).FirstOrDefault(),
                //}).ToList();

                //var lotIds = input.InvoiceItems.Where(t => t.LotId != null).Select(x => x.LotId).ToList();
                //var locationIds = await _lotRepository.GetAll().AsNoTracking()
                //                .Where(t => lotIds.Contains(t.Id))
                //                .Select(t => (long?)t.LocationId)
                //                .ToListAsync();
                //var getCostResult = await _inventoryManager.GetAvgCostForIssues(input.IssuingDate.Value, locationIds, itemToCalculateCost, itemIssueJournal.Journal, roundingId);

                //foreach (var r in getCostResult.Items)
                //{
                //    input.InvoiceItems[r.Index].UnitCostItemIssue = r.UnitCost;
                //    input.InvoiceItems[r.Index].TotalItemIssue = r.LineCost;
                //}

                //var TotalItemIssue = getCostResult.Total;
                #endregion Calculat Cost

                invoice.UpdateReceivedStatus(DeliveryStatus.ShipAll);

                itemIssueItems = await _itemIssueItemRepository.GetAll()
                                .Where(t => t.ItemIssueId == invoice.ItemIssueId)
                                .ToListAsync();
                @inventoryJournalItems = await (_journalItemRepository.GetAll()
                                                .Include(u => u.Journal.ItemIssue)
                                                .Where(s => s.Journal.ItemIssueId.HasValue)
                                                .Where(u => u.Journal.ItemIssueId == invoice.ItemIssueId &&
                                                 u.Identifier != null)
                                            ).ToListAsync();
            }

            @entity.UpdateInvoice(invoice);

            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));
            CheckErrors(await _journalItemManager.CreateAsync(clearanceJournalItem));
            CheckErrors(await _invoiceManager.CreateAsync(invoice));

            #endregion

            if (input.UseExchangeRate && input.CurrencyId != input.MultiCurrencyId)
            {
                var exchange = InvoiceExchangeRate.Create(tenantId, userId, invoice.Id, input.ExchangeRate.FromCurrencyId, input.ExchangeRate.ToCurrencyId, input.ExchangeRate.Bid, input.ExchangeRate.Ask);
                await _exchangeRateRepository.InsertAsync(exchange);
            }


            var subInvoiceItems = new List<KeyValuePair<CreateOrUpdateInvoiceItemInput, InvoiceItem>>();

            foreach (var i in input.InvoiceItems)
            {
                if (input.CurrencyId == input.MultiCurrencyId)
                {
                    i.MultiCurrencyTotal = i.Total;
                    i.MultiCurrencyUnitCost = i.UnitCost;
                }
                if (i.LotId == 0)
                {
                    i.LotId = null;
                }
                //insert to bill item
                var invoiceItem = InvoiceItem.Create(tenantId, userId, invoice, i.TaxId, i.ItemId, i.Description, i.Qty, i.UnitCost, i.DiscountRate, i.Total, i.MultiCurrencyUnitCost, i.MultiCurrencyTotal);
                i.Id = invoiceItem.Id;
                if (i.ItemIssueId != null && input.ReceiveFrom == ReceiveFrom.ItemIssue)
                {
                    invoiceItem.UpdateIssueItemId(i.ItemIssueId.Value);
                }
                //if (input.ReceiveFrom == ReceiveFrom.ItemIssue && i.ItemIssueId != null)
                //{
                //    var journalItem = @inventoryJournalItems.Where(u => u.Identifier == i.ItemIssueId);

                //    if (journalItem != null && journalItem.Count() > 0)
                //    {
                //        foreach (var c in journalItem)
                //        {
                //            if (c.Key == PostingKey.Inventory)
                //            {
                //                // update inventory posting key credit
                //                c.UpdateJournalItemDebitCredit(0, i.TotalItemIssue);
                //                CheckErrors(await _journalItemManager.UpdateAsync(c));
                //            }
                //            if (c.Key == PostingKey.COGS)
                //            {
                //                // update cogs posting key debit
                //                c.UpdateJournalItemDebitCredit(i.TotalItemIssue, 0);
                //                CheckErrors(await _journalItemManager.UpdateAsync(c));
                //            }
                //        }
                //    }

                //    if (i.OrderItemId != null)
                //    {
                //        invoiceItem.UpdateOrderItemId(i.OrderItemId);
                //    }
                //   invoiceItem.UpdateIssueItemId(i.ItemIssueId.Value);                   
                //    var itemIssueitem = itemIssueItems.Where(t => t.Id == i.ItemIssueId).FirstOrDefault();
                //    itemIssueitem.UpdateQty(i.Qty);
                //    itemIssueitem.UpdateItemIssueItem(i.UnitCostItemIssue, 0, i.TotalItemIssue);
                //    CheckErrors(await _itemIssueItemManager.UpdateAsync(itemIssueitem));

                //}
                else if (input.ReceiveFrom == ReceiveFrom.SaleOrder && i.OrderItemId != null)
                {
                    if (i.Qty > i.OrginalQtyFromSaleOrder)
                    {
                        throw new UserFriendlyException(L("InvoiceMessageWarning", i.ItemName) + L("Row") + " " + indexLot.ToString());
                    }
                    invoiceItem.UpdateOrderItemId(i.OrderItemId);
                }
                else if(input.ReceiveFrom == ReceiveFrom.DeliverySchedule && i.DeliveryScheduleItemId.HasValue)
                {
                    if (i.Qty > i.OrginalQtyFromDeliverySchedule)
                    {
                        throw new UserFriendlyException(L("InvoiceMessageWarning", i.ItemName) + L("Row") + " " + indexLot.ToString());
                    }
                    invoiceItem.SetDeliverySchedulItem(i.DeliveryScheduleItemId);
                }
                invoiceItem.UpdateLot(i.LotId);
                CheckErrors(await _invoiceItemManager.CreateAsync(invoiceItem));

                if (i.Key.HasValue || i.ParentId.HasValue) subInvoiceItems.Add(new KeyValuePair<CreateOrUpdateInvoiceItemInput, InvoiceItem>(i, invoiceItem));

                //insert inventory journal item into debit
                var revenueJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, i.AccountId,
                                            i.Description, 0, i.Total, PostingKey.Revenue, invoiceItem.Id);
                CheckErrors(await _journalItemManager.CreateAsync(revenueJournalItem));

            }

            var keys = input.InvoiceItems.Where(s => s.Key.HasValue).GroupBy(s => s.Key.Value).Select(s => s.Key).ToHashSet();
            foreach (var key in keys)
            {
                var parent = subInvoiceItems.FirstOrDefault(s => s.Key.Key == key);
                var subitems = subInvoiceItems.Where(s => s.Key.ParentId == key).Select(s => s.Value);
                foreach (var sub in subitems)
                {
                    sub.SetParent(parent.Value.Id);
                }
            }

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.Invoice };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            if (input.ConvertToItemIssue == true && input.Status == TransactionStatus.Publish)
            {
                if (input.IssuingDate == null)
                {
                    throw new UserFriendlyException(L("PleaseAddIssuingDate"));
                }
                var inventoryAccount = await _itemRepository.GetAll()
                                            .Where(t => input.InvoiceItems.Any(x => x.ItemId == t.Id) &&
                                                        t.InventoryAccountId != null &&
                                                        t.ItemType.DisplayInventoryAccount)
                                            .AsNoTracking()
                                            .Select(u => new { u.Id, InventoryAccountId = u.InventoryAccountId.Value })
                                            .ToDictionaryAsync(u => u.Id);

                var itemLists = input.InvoiceItems.Where(d => d.ItemId != null && inventoryAccount.ContainsKey(d.ItemId.Value))
                                .Select(t => new CreateOrUpdateItemIssueItemInput
                                {
                                    InventoryAccountId = inventoryAccount[t.ItemId.Value].InventoryAccountId,
                                    LotId = t.LotId,
                                    ItemId = t.ItemId.Value,
                                    Description = t.Description,
                                    DiscountRate = t.DiscountRate,
                                    Qty = t.Qty,
                                    //SaleOrderId = t.SaleOrderId,
                                    //SaleOrderItemId = t.SaleOrderItem != null ? t.SaleOrderItem.Id : Guid.Empty,
                                    PurchaseAccountId = t.Item.PurchaseAccountId.Value,
                                    Item = t.Item,
                                    InvoiceItemId = t.Id,
                                    ItemBatchNos = t.ItemBatchNos,
                                   // DeliveryScheduleId = t.DeliveryId,
                                   // DeliveryScheduleItemId = t.DeliveryScheduleItemId,
                                     
                                }).ToList();
                var itemIssueInput = new CreateItemIssueInput()
                {
                    IsConfirm = input.IsConfirm,
                    TransactionTypeId = input.TransactionTypeId,
                    BillingAddress = input.BillingAddress,
                    ShippingAddress = input.ShippingAddress,
                    Status = input.Status,
                    ClassId = input.ClassId,
                    CurrencyId = input.CurrencyId,
                    CustomerId = input.CustomerId,
                    Date = input.IssuingDate.Value,
                    InvoiceId = invoice.Id,
                    IssueNo = input.InvoiceNo,
                    LocationId = input.LocationId,
                    Memo = input.Memo,
                    ReceiveFrom = ReceiveFrom.Invoice,
                    Reference = input.Reference,
                    SameAsShippingAddress = input.SameAsShippingAddress,
                    Total = input.Total,
                    Items = itemLists,

                };
                await ItemIssueSave(itemIssueInput);

            }


            if (input.ReceiveFrom == ReceiveFrom.ItemIssue && input.ItemIssueId != null)
            {

                var itemIssue = await _itemIssueRepository.GetAll().Where(t => t.Id == input.ItemIssueId).FirstOrDefaultAsync();
                var itemIssueInput = new UpdateItemIssueInput()
                {
                    IsConfirm = input.IsConfirm,
                    Id = input.ItemIssueId.Value,
                    TransactionTypeId = input.TransactionTypeId,
                    BillingAddress = input.BillingAddress,
                    ShippingAddress = input.ShippingAddress,
                    Status = input.Status,
                    ClassId = input.ClassId,
                    CurrencyId = input.CurrencyId,
                    CustomerId = input.CustomerId,
                    Date = input.IssuingDate.Value,
                    InvoiceId = invoice.Id,
                    IssueNo = input.InvoiceNo,
                    LocationId = input.LocationId,
                    Memo = input.Memo,
                    ReceiveFrom = itemIssue.ReceiveFrom,
                    ConvertToInvoice = itemIssue.ConvertToInvoice,
                    Reference = input.Reference,
                    SameAsShippingAddress = input.SameAsShippingAddress,
                    Total = input.Total,
                    Items = input.InvoiceItems
                            .Where(t => t.Item.InventoryAccountId != null)
                            .Select(t => new CreateOrUpdateItemIssueItemInput()
                            {
                                Id = t.ItemIssueId,
                                LotId = t.LotId,
                                ItemId = t.ItemId.Value,
                                Description = t.Description,
                                DiscountRate = t.DiscountRate,
                                Qty = t.Qty,
                                SaleOrderId = t.SaleOrderId,
                                SaleOrderItemId = t.OrderItemId,
                                PurchaseAccountId = t.Item.PurchaseAccountId.Value,
                                InventoryAccountId = t.Item.InventoryAccountId.Value,
                                Item = t.Item,
                                InvoiceItemId = t.Id,
                                ItemBatchNos = t.ItemBatchNos
                            }).ToList()
                };


                var itemIssueJournal = inventoryJournalItems.Select(u => u.Journal).FirstOrDefault();

                await UpdateItemIssue(itemIssueInput, false, tenantId, userId, invoice.Id, itemIssueItems, itemIssueJournal, inventoryJournalItems);
            }

            var orders = input.InvoiceItems.Where(s => s.SaleOrderId.HasValue).GroupBy(s => s.SaleOrderId).Select(s => s.Key.Value);

            if (orders.Any())
            {
                await CurrentUnitOfWork.SaveChangesAsync();
                foreach (var id in orders)
                {
                    await UpdateOrderInventoryStatus(id);
                }
            }

            var deliverys = input.InvoiceItems.Where(s => s.DeliveryId.HasValue).GroupBy(s => s.DeliveryId).Select(s => s.Key.Value);
            if (deliverys.Any())
            {
                await CurrentUnitOfWork.SaveChangesAsync();
                foreach (var id in deliverys)
                {
                    await UpdateDeliveryInventoryStatus(id, true);

                }
            }

            return new NullableIdDto<Guid>() { Id = invoice.Id };

        }

        protected async Task ItemIssueSave(CreateItemIssueInput input)
        {
            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }


            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                    .Where(t => (t.LockKey == TransactionLockType.ItemIssue || t.LockKey == TransactionLockType.InventoryTransaction)
                    && t.IsLock == true && t.LockDate != null && t.LockDate.Value.Date >= input.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);

            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.IssueNo = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            //insert to journal
            var @entity = Journal.Create(tenantId, userId, input.IssueNo,
                                input.Date, input.Memo, input.Total, input.Total,
                                input.CurrencyId, input.ClassId, input.Reference,
                                input.LocationId);

            entity.UpdateStatus(input.Status);
            #region journal transaction type 
            var transactionTypeId = await _journalTransactionTypeManager.GetJournalTransactionTypeId(InventoryTransactionType.ItemIssueSale);
            entity.SetJournalTransactionTypeId(transactionTypeId);
            #endregion

            #region Calculat Cost
            //var roundingId = (await GetCurrentTenantAsync()).RoundDigitAccountId;

            var itemToCalculateCost = input.Items.Select((u, index) => new Inventories.Data.GetAvgCostForIssueData()
            {
                ItemName = u.Item.ItemName,
                Index = index,
                ItemId = u.ItemId,
                ItemCode = u.Item.ItemCode,
                Qty = u.Qty,
                LotId = u.LotId,
                InventoryAccountId = u.InventoryAccountId
            }).ToList();
            var lotIds = input.Items.Select(x => x.LotId).ToList();
            var locationIds = await _lotRepository.GetAll().AsNoTracking()
                            .Where(t => lotIds.Contains(t.Id))
                            .Select(t => (long?)t.LocationId)
                            .ToListAsync();
            var getCostResult = await _inventoryManager.GetAvgCostForIssues(input.Date, locationIds, itemToCalculateCost/*, @entity, roundingId*/);

            foreach (var r in getCostResult.Items)
            {
                input.Items[r.Index].UnitCost = r.UnitCost;
                input.Items[r.Index].Total = r.LineCost;
            }

            input.Total = getCostResult.Total;

            #endregion Calculat Cost


            //await UpdateSOReceiptStautus(null, input.ReceiveFrom, input.InvoiceId, input.Items);

            //insert to item Issue
            var @itemIssue = ItemIssue.Create(tenantId, userId, input.ReceiveFrom, input.CustomerId,
                input.SameAsShippingAddress, input.ShippingAddress, input.BillingAddress,
                input.Total, null);
            @itemIssue.UpdateTransactionType(InventoryTransactionType.ItemIssueSale);
            itemIssue.UpdateTransactionTypeId(input.TransactionTypeId);

            @entity.UpdateItemIssue(@itemIssue);
            //Update ItemIssueId on table Invoice
            if (input.ReceiveFrom == ReceiveFrom.Invoice && input.InvoiceId != null)
            {
                var @invoice = await _invoiceRepository.GetAll().Where(u => u.Id == input.InvoiceId).FirstOrDefaultAsync();
                invoice.UpdateItemIssueId(itemIssue.Id);
                // update received status in Invoice to full when receive from Invoice 
                if (input.Status == TransactionStatus.Publish)
                {
                    invoice.UpdateReceivedStatus(DeliveryStatus.ShipAll);
                }
                CheckErrors(await _invoiceManager.UpdateAsync(invoice));
            }
            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));
            CheckErrors(await _itemIssueManager.CreateAsync(itemIssue));

            var ItemIssueItems = (from ItemIssueitem in input.Items
                                  where (ItemIssueitem.SaleOrderItemId != null)
                                  select ItemIssueitem.SaleOrderItemId);
            var count = ItemIssueItems.Count();
            if (input.ReceiveFrom == ReceiveFrom.SaleOrder && count <= 0)
            {
                throw new UserFriendlyException(L("PleaseAddSaleOrder"));
            }
            //select SaleOrderId from Invoice 
            var invoiceItems = await _invoiceItemRepository.GetAll()
               .Include(u => u.SaleOrderItem)
               .Include(u => u.SaleOrderItem.SaleOrder)
               .Include(u=>u.DeliveryScheduleItem.DeliverySchedule)
               .Where(u => u.InvoiceId == input.InvoiceId && (u.OrderItemId != null && u.SaleOrderItem.SaleOrderId == u.SaleOrderItem.SaleOrder.Id || u.DeliverySchedulItemId != null && u.DeliveryScheduleItem.DeliveryScheduleId == u.DeliveryScheduleItem.DeliverySchedule.Id))
                           .Select(u => new InvoiceSaleOrderDto
                           {
                               orderItemId = u.OrderItemId,
                               soId = u.SaleOrderItem != null ? u.SaleOrderItem.SaleOrderId : (Guid?)null,                              
                               OriginalTotalQty = u.SaleOrderItem != null ? u.SaleOrderItem.Qty : 0,
                               qty = u.Qty,
                               DeliveryItemId = u.DeliverySchedulItemId,
                               DeliveryId = u.DeliveryScheduleItem != null ? u.DeliveryScheduleItem.DeliveryScheduleId : (Guid?)null,
                               DeliveryOriginalTotalQty = u.DeliveryScheduleItem != null ? u.DeliveryScheduleItem.Qty : 0,
                           }).ToListAsync();
            foreach (var i in input.Items)
            {
                //insert to item Issue item
                var @itemIssueItem = ItemIssueItem.Create(tenantId, userId,
                                        itemIssue, i.ItemId, i.Description,
                                        i.Qty, i.UnitCost,
                                        i.DiscountRate, i.Total);

                if (i.SaleOrderItemId.HasValue) itemIssueItem.UpdateSaleOrderItemId(i.SaleOrderItemId);
                if (i.DeliveryScheduleItemId.HasValue) itemIssueItem.UpdateDeliveryScheduleItemId(i.DeliveryScheduleItemId);
                itemIssueItem.UpdateLot(i.LotId);             
                if (invoiceItems.Count() > 0 && input.ReceiveFrom == ReceiveFrom.Invoice && i.InvoiceItemId != null)
                {
                    if (i.InvoiceItemId != null && i.SaleOrderItemId != null)
                    {
                        itemIssueItem.UpdateSaleOrderItemId(i.SaleOrderItemId);
                    }
                    else if(i.InvoiceItemId.HasValue && i.DeliveryScheduleItemId.HasValue)
                    {
                        itemIssueItem.UpdateDeliveryScheduleItemId(i.DeliveryScheduleItemId);
                    }
                }
                CheckErrors(await _itemIssueItemManager.CreateAsync(@itemIssueItem));
                //Update InvoiceItemid when click on Invoice in table InvoiceItem
                if (input.ReceiveFrom == ReceiveFrom.Invoice && i.InvoiceItemId != null)
                {

                    var invoiceItem = await _invoiceItemRepository.GetAll()
                         .Where(u => u.Id == i.InvoiceItemId.Value).FirstOrDefaultAsync();
                    invoiceItem.UpdateIssueItemId(itemIssueItem.Id);
                    invoiceItem.UpdateLot(itemIssueItem.LotId);
                    CheckErrors(await _invoiceItemManager.UpdateAsync(invoiceItem));

                    if (i.Qty > invoiceItem.Qty)
                    {
                        throw new UserFriendlyException(L("PleaseCheckYourQty"));
                    }
                }
                //insert inventory journal item into credit
                var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, i.InventoryAccountId,
                    i.Description, 0, i.Total, PostingKey.Inventory, itemIssueItem.Id);
                CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));
                //insert COGS journal item into Debit
                var cogsJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, i.PurchaseAccountId,
                    i.Description, i.Total, 0, PostingKey.COGS, itemIssueItem.Id);
                CheckErrors(await _journalItemManager.CreateAsync(cogsJournalItem));

                if (i.ItemBatchNos != null && i.ItemBatchNos.Any())
                {
                    var addItemBatchNos = i.ItemBatchNos.Select(s => ItemIssueItemBatchNo.Create(tenantId, userId, itemIssueItem.Id, s.BatchNoId, s.Qty)).ToList();
                    foreach (var a in addItemBatchNos)
                    {
                        await _itemIssueItemBatchNoRepository.InsertAsync(a);
                    }
                }
            }

            //update status so when create item Issue from invoice
            if (invoiceItems.Count() > 0 && input.Status == TransactionStatus.Publish)
            {
                //update Receive status to pending                           
            }
            //update SO status from SO 
            if (input.Status == TransactionStatus.Publish && input.ReceiveFrom == ReceiveFrom.SaleOrder)
            {
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            await SyncItemIssue(itemIssue.Id);
            var scheduleItems = input.Items.Select(s => s.ItemId).Distinct().ToList();
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, input.Date, scheduleItems);
            }

        }

        protected async Task UpdateItemIssue(UpdateItemIssueInput input, bool autoConvert,
            int tenantId, long userId, Guid invoiceId, List<ItemIssueItem> itemIssueItems,
                                            Journal @journal,
                                            List<JournalItem> inventoryJournalItems)
        {
            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }

            if (input.IsConfirm == false)
            {
                var validateLockDate = await _lockRepository.GetAll()
                                     .Where(t => t.IsLock == true &&
                                     (t.LockDate.Value.Date >= journal.Date.Date || t.LockDate.Value.Date >= input.Date.Date)
                                     && (t.LockKey == TransactionLockType.ItemIssue || t.LockKey == TransactionLockType.InventoryTransaction)).CountAsync();

                var isChangeItems = input.Items.Count() != itemIssueItems.Count() || input.Items.Any(s => itemIssueItems.Any(r => r.Id == s.Id && (r.ItemId != s.ItemId || r.LotId != s.LotId || r.Qty != s.Qty)));

                if (validateLockDate > 0 && (input.Date.Date != journal.Date.Date || isChangeItems || input.LocationId != journal.LocationId))
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            if (@journal == null || @journal.ItemIssue == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);
            if (auto.CustomFormat == true)
            {
                input.IssueNo = journal.JournalNo;
            }

            var scheduleDate = input.Date > journal.Date ? journal.Date : input.Date;

            if (autoConvert == true)
            {
                journal.Update(userId, input.IssueNo, input.Date, input.Memo, input.Total, input.Total,
                                input.CurrencyId, input.ClassId, input.Reference, 0, input.LocationId);
                journal.UpdateStatus(input.Status);
            }
            else
            {
                var setting = await _settingRepository.FirstOrDefaultAsync(s => s.SettingType == BillInvoiceSettingType.Invoice);

                if (setting == null || setting.ReferenceSameAsGoodsMovement)
                {
                    journal.Update(userId, input.Total, input.Total, input.Date,
                                input.ClassId, input.Reference, input.LocationId);
                }
                else
                {
                    journal.Update(userId, input.Total, input.Total, input.Date,
                               input.ClassId, input.LocationId);
                }
            }

            var lotIds = input.Items.Select(x => x.LotId).ToList();
            var locationIds = await _lotRepository.GetAll().AsNoTracking()
                                    .Where(t => lotIds.Contains(t.Id))
                                    .Select(t => (long?)t.LocationId)
                                    .ToListAsync();
            //await UpdateSOReceiptStautus(input.Id, input.ReceiveFrom, input.InvoiceId, input.Items);

            #region Calculat Cost
            //var roundingId = (await GetCurrentTenantAsync()).RoundDigitAccountId;
            var itemToCalculateCost = input.Items.Select((u, index) => new Inventories.Data.GetAvgCostForIssueData()
            {
                ItemName = u.Item.ItemName,
                Index = index,
                ItemId = u.ItemId,
                ItemCode = u.Item.ItemCode,
                Qty = u.Qty,
                LotId = u.LotId,
                ItemIssueItemId = input.Id,
                InventoryAccountId = u.InventoryAccountId
            }).ToList();

            var getCostResult = await _inventoryManager.GetAvgCostForIssues(input.Date, locationIds, itemToCalculateCost/*, @journal, roundingId*/);

            foreach (var r in getCostResult.Items)
            {
                input.Items[r.Index].UnitCost = r.UnitCost;
                input.Items[r.Index].Total = r.LineCost;
            }

            input.Total = getCostResult.Total;
            #endregion Calculat Cost

            //update item issue 
            var itemIssue = @journal.ItemIssue;// await _itemIssueManager.GetAsync(input.Id, true);

            itemIssue.Update(tenantId, itemIssue.ReceiveFrom, input.CustomerId,
                          input.SameAsShippingAddress, input.ShippingAddress,
                          input.BillingAddress, input.Total, null, input.ConvertToInvoice);
            itemIssue.UpdateTransactionTypeId(input.TransactionTypeId);
            journal.UpdateCreditDebit(input.Total, input.Total);
            CheckErrors(await _journalManager.UpdateAsync(@journal, auto.DocumentType));

            CheckErrors(await _itemIssueManager.UpdateAsync(itemIssue));



            var toDeleteItemIssueItem = itemIssueItems.Where(u => !input.Items.Any(i => i.Id != null && i.Id == u.Id)).ToList();
            var toDeletejournalItem = inventoryJournalItems.Where(u => !input.Items.Any(i => u.Identifier != null && i.Id != null && i.Id == u.Identifier)).ToList();

            //validate Item Issue when use on item receceipt customer credit and customer credit
            var customerCredit = await _customerCreditItemRepository.GetAll()
                .Include(u => u.ItemIssueSaleItem)
                .Where(t => (toDeleteItemIssueItem.Any(g => g.Id == t.ItemIssueSaleItemId)) ||
                 (t.ItemIssueSaleItem != null && input.Items.Any(g => g.Id == t.ItemIssueSaleItemId && g.Id != null && t.ItemIssueSaleItem != null && g.Qty != t.ItemIssueSaleItem.Qty))).AsNoTracking().CountAsync();
            var itemReceiptCustomerCredit = await _itemReceiptItemCustomerCreditRepository
                .GetAll().Include(u => u.ItemIssueSaleItem).Where(t => toDeleteItemIssueItem.Any(g => g.Id == t.ItemIssueSaleItemId) ||
                (input.Items.Any(g => g.Id == t.ItemIssueSaleItemId && t.ItemIssueSaleItem != null && g.Id != null && g.Qty != t.ItemIssueSaleItem.Qty)))
                .AsNoTracking().CountAsync();

            if (customerCredit > 0 || itemReceiptCustomerCredit > 0)
            {
                throw new UserFriendlyException(L("ItemAlreadyReturnCannotBeModified"));
            }

            var invoiceItems = await _invoiceItemRepository.GetAll().Where(u => u.InvoiceId == invoiceId).ToListAsync();

            var itemBatchNos = await _itemIssueItemBatchNoRepository.GetAll().Where(s => itemIssueItems.Any(r => r.Id == s.ItemIssueItemId)).AsNoTracking().ToListAsync();
            if (toDeleteItemIssueItem.Any())
            {
                var deleteItemBatchNos = itemBatchNos.Where(s => toDeleteItemIssueItem.Any(r => r.Id == s.ItemIssueItemId)).ToList();
                if (deleteItemBatchNos.Any()) await _itemIssueItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
            }

            //Update invoice Id when create from invoice to in voice
            foreach (var c in input.Items)
            {
                if (c.Id != null) //update
                {
                    var itemIssueItem = itemIssueItems.FirstOrDefault(u => u.Id == c.Id);
                    var journalItem = @inventoryJournalItems.ToList().Where(u => u.Identifier == itemIssueItem.Id);
                    if (itemIssueItem != null)
                    {
                        //new
                        itemIssueItem.Update(tenantId, c.ItemId, c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total);
                        itemIssueItem.UpdateLot(c.LotId);
                        if (c.SaleOrderItemId.HasValue) itemIssueItem.UpdateSaleOrderItemId(c.SaleOrderItemId);
                        else
                        {
                            itemIssueItem.UpdateSaleOrderItemId(null);
                        }


                        CheckErrors(await _itemIssueItemManager.UpdateAsync(itemIssueItem));

                    }

                    if (journalItem != null && journalItem.Count() > 0)
                    {
                        foreach (var i in journalItem)
                        {
                            if (i.Key == PostingKey.Inventory)
                            {
                                // update inventory posting key credit
                                i.UpdateJournalItem(userId, c.InventoryAccountId, c.Description, 0, c.Total);
                                CheckErrors(await _journalItemManager.UpdateAsync(i));
                            }
                            if (i.Key == PostingKey.COGS)
                            {
                                // update cogs posting key debit
                                i.UpdateJournalItem(userId, c.PurchaseAccountId, c.Description, c.Total, 0);
                                CheckErrors(await _journalItemManager.UpdateAsync(i));
                            }
                        }
                    }

                    var oldItemBatchs = itemBatchNos.Where(s => s.ItemIssueItemId == c.Id).ToList();

                    if (c.ItemBatchNos != null && c.ItemBatchNos.Any())
                    {
                        var addItemBatchNos = c.ItemBatchNos.Where(s => !s.Id.HasValue || s.Id == Guid.Empty).Select(s => ItemIssueItemBatchNo.Create(tenantId, userId, itemIssueItem.Id, s.BatchNoId, s.Qty)).ToList();
                        if (addItemBatchNos.Any())
                        {
                            foreach (var a in addItemBatchNos)
                            {
                                await _itemIssueItemBatchNoRepository.InsertAsync(a);
                            }
                        }

                        var updateItemBatchNos = oldItemBatchs.Where(s => c.ItemBatchNos.Any(r => r.Id == s.Id)).Select(s =>
                        {
                            var oldItemBatch = s;
                            var newBatch = c.ItemBatchNos.FirstOrDefault(r => r.Id == s.Id);

                            oldItemBatch.Update(userId, itemIssueItem.Id, newBatch.BatchNoId, newBatch.Qty);
                            return oldItemBatch;
                        }).ToList();

                        if (updateItemBatchNos.Any()) await _itemIssueItemBatchNoRepository.BulkUpdateAsync(updateItemBatchNos);

                        var deleteItemBatchNos = oldItemBatchs.Where(s => !c.ItemBatchNos.Any(r => r.Id == s.Id)).ToList();
                        if (deleteItemBatchNos.Any())
                        {
                            await _itemIssueItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
                        }
                    }
                    else if (oldItemBatchs.Any())
                    {
                        await _itemIssueItemBatchNoRepository.BulkDeleteAsync(oldItemBatchs);
                    }

                }
                else if (c.Id == null) //create
                {
                    //insert to item Receipt item
                    var itemIssueItem = ItemIssueItem.Create(tenantId, userId, itemIssue, c.ItemId,
                                                                                 c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total);
                    itemIssueItem.UpdateLot(c.LotId);

                    if (c.SaleOrderItemId.HasValue) itemIssueItem.UpdateSaleOrderItemId(c.SaleOrderItemId);


                    //Update table invoice and invoice item                  
                    var invoiceItem = invoiceItems.Where(u => u.Id == c.InvoiceItemId).FirstOrDefault();
                    invoiceItem.UpdateIsItemIssue(true);
                    invoiceItem.UpdateIssueItemId(itemIssueItem.Id);
                    invoiceItem.UpdateLot(itemIssueItem.LotId);

                    CheckErrors(await _invoiceItemManager.UpdateAsync(invoiceItem));

                    CheckErrors(await _itemIssueItemManager.CreateAsync(itemIssueItem));
                    //insert inventory journal item into debit
                    var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal, c.InventoryAccountId, c.Description, 0, c.Total, PostingKey.Inventory, itemIssueItem.Id);
                    CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));
                    //insert cogs journal item into debit
                    var cogsJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal, c.PurchaseAccountId, c.Description, c.Total, 0, PostingKey.COGS, itemIssueItem.Id);
                    CheckErrors(await _journalItemManager.CreateAsync(cogsJournalItem));

                    if (c.ItemBatchNos != null && c.ItemBatchNos.Any())
                    {
                        var addItemBatchNos = c.ItemBatchNos.Select(s => ItemIssueItemBatchNo.Create(tenantId, userId, itemIssueItem.Id, s.BatchNoId, s.Qty)).ToList();
                        foreach (var a in addItemBatchNos)
                        {
                            await _itemIssueItemBatchNoRepository.InsertAsync(a);
                        }
                    }
                }
            }

            var scheduleItems = input.Items.Select(s => s.ItemId).Concat(toDeleteItemIssueItem.Select(s => s.ItemId)).Distinct().ToList();

            foreach (var t in toDeleteItemIssueItem)
            {
                CheckErrors(await _itemIssueItemManager.RemoveAsync(t));
            }

            foreach (var t in toDeletejournalItem)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(t));
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            await SyncItemIssue(itemIssue.Id);

            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, scheduleDate, scheduleItems);
            }

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_Delete)]
        public async Task Delete(CarlEntityDto input)
        {
            // validate from Payment
            var validateReceivePayment = await _receivePaymentDetailRepository.GetAll().AsNoTracking()
                                        .Where(t => t.InvoiceId == input.Id).CountAsync();

            if (validateReceivePayment > 0)
            {
                throw new UserFriendlyException(L("RecordHasReceivePayment"));
            }

            var journal = await _journalRepository.GetAll()
                            .Include(u => u.Invoice.TransactionTypeSale)
                            .Include(u => u.Invoice.ItemIssue)
                            .Include(u => u.Invoice.ShippingAddress)
                            .Include(u => u.Invoice.BillingAddress)
                            .Where(u => u.JournalType == JournalType.Invoice && u.InvoiceId == input.Id)
                            .FirstOrDefaultAsync();

            if (journal == null || journal.Invoice == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                                      .Where(t => (t.LockKey == TransactionLockType.Invoice)
                                      && t.IsLock == true && t.LockDate != null && t.LockDate.Value.Date >= journal.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            var @entity = journal.Invoice;

            if (entity.ItemIssueId != null && entity.ConvertToItemIssue == true)
            {//remove if has convert to item issue by has relationship with invoice 
                var inputItemIssue = new CarlEntityDto() { IsConfirm = input.IsConfirm, Id = entity.ItemIssueId.Value };
                await DeleteItemIssue(inputItemIssue);
                //update item issue id to null after delete item issue from invoice 
                entity.UpdateItemIssueId(null);
            }
            else if (entity.ItemIssue != null && entity.CreationTime < entity.ItemIssue.CreationTime)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }


            if (journal.Invoice.TransactionTypeSale != null && journal.Invoice.TransactionTypeSale.IsPOS)
            {
                var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.POS);

                if (journal.JournalNo == auto.LastAutoSequenceNumber && auto.CustomFormat == true)
                {
                    var jo = await _journalRepository.GetAll().Include(u => u.Invoice.TransactionTypeSale)
                                    .Where(t => t.Id != journal.Id && t.JournalType == JournalType.Invoice && (t.Invoice.TransactionTypeSale != null && t.Invoice.TransactionTypeSale.IsPOS))
                                    .OrderByDescending(t => t.CreationTime).FirstOrDefaultAsync();
                    if (jo != null)
                    {
                        auto.UpdateLastAutoSequenceNumber(jo.JournalNo);
                    }
                    else
                    {
                        auto.UpdateLastAutoSequenceNumber(null);
                    }
                    CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
                }
            }
            else
            {
                var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Invoice);

                if (journal.JournalNo == auto.LastAutoSequenceNumber && auto.CustomFormat == true)
                {
                    var jo = await _journalRepository.GetAll().Include(u => u.Invoice.TransactionTypeSale)
                                    .Where(t => t.Id != journal.Id && t.JournalType == JournalType.Invoice && (t.Invoice.TransactionTypeSale == null || !t.Invoice.TransactionTypeSale.IsPOS))
                                    .OrderByDescending(t => t.CreationTime).FirstOrDefaultAsync();
                    if (jo != null)
                    {
                        auto.UpdateLastAutoSequenceNumber(jo.JournalNo);
                    }
                    else
                    {
                        auto.UpdateLastAutoSequenceNumber(null);
                    }
                    CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
                }
            }

            var exchangeRates = await _exchangeRateRepository.GetAll().AsNoTracking().Where(s => s.InvoiceId == input.Id).ToListAsync();
            if (exchangeRates.Any()) await _exchangeRateRepository.BulkDeleteAsync(exchangeRates);

            journal.UpdateInvoice(null);

            //query get journal item and delete
            var @jounalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == journal.Id).ToListAsync();
            foreach (var ji in jounalItems)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(ji));
            }
            CheckErrors(await _journalManager.RemoveAsync(journal));

            //query get bill item and delete 
            var invoiceItems = await _invoiceItemRepository.GetAll()
                .Include(u => u.SaleOrderItem)
                .Include(u=>u.DeliveryScheduleItem)
                .Include(u=>u.ItemIssueItem.DeliveryScheduleItem)
                .Include(s => s.ItemIssueItem.SaleOrderItem)
                .Include(s => s.ItemIssueItem.ItemIssue)
                .Where(u => u.InvoiceId == entity.Id).ToListAsync();


            #region issue link to So
            var invoiceOrderIds = invoiceItems
                             .Where(s => s.OrderItemId.HasValue)
                             .GroupBy(s => s.SaleOrderItem.SaleOrderId)
                             .Select(s => s.Key)
                             .ToList();

            var issueOrderIds = new List<Guid>();
            var issueLinkItems = invoiceItems
                                .Where(s => s.ItemIssueItem != null && s.ItemIssueItem.SaleOrderItem != null)
                                .Select(s => s.ItemIssueItem)
                                .ToList();

            issueOrderIds = issueLinkItems
                        .GroupBy(s => s.SaleOrderItem.SaleOrderId)
                        .Select(s => s.Key)
                        .ToList();

            foreach (var link in issueLinkItems)
            {
                link.UpdateSaleOrderItemId(null);
                await _itemIssueItemManager.UpdateAsync(link);
            }
            #endregion

            foreach (var invi in invoiceItems)
            {
                //if (invi.OrderItemId != null && invi.SaleOrderItem != null)
                //{
                //    invi.UpdateOrderItemId(null);
                //}
                //if(invi.DeliveryScheduleItem != null && invi.DeliveryScheduleItem != null)
                //{
                //    invi.SetDeliverySchedulItem(null);
                //}

                CheckErrors(await _invoiceItemManager.RemoveAsync(invi));

            }

            CheckErrors(await _invoiceManager.RemoveAsync(entity));

            var orderIds = invoiceOrderIds.Union(issueOrderIds).ToList();
            if (orderIds.Any())
            {
                await CurrentUnitOfWork.SaveChangesAsync();
                foreach (var orderId in orderIds)
                {
                    await UpdateOrderInventoryStatus(orderId);
                }
            }

            //update delivery schedule item status when delete invoice
            var invoiceDeliveryIds = invoiceItems
                           .Where(s => s.DeliverySchedulItemId.HasValue)
                           .GroupBy(s => s.DeliveryScheduleItem.DeliveryScheduleId)
                           .Select(s => s.Key)
                           .ToList();

            var issueDeliveryIds = invoiceItems
                                .Where(s => s.ItemIssueItem != null && s.ItemIssueItem.DeliveryScheduleItem != null)
                                .GroupBy(s => s.ItemIssueItem.DeliveryScheduleItem.DeliveryScheduleId)
                                .Select(s => s.Key)
                                .ToList();

            var deliveryIds = invoiceDeliveryIds.Union(issueDeliveryIds).ToList();
            if (deliveryIds.Any())
            {
                await CurrentUnitOfWork.SaveChangesAsync();
                foreach (var deliveryId in deliveryIds)
                {
                    await UpdateDeliveryInventoryStatus(deliveryId,true);
                }
            }

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.Invoice };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_Find)]
        public async Task<PagedResultDto<GetListInvoiceOutput>> Find(GetListInvoiceInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var query = (from jItem in _journalItemRepository.GetAll().AsNoTracking()

                         join j in _journalRepository.GetAll().AsNoTracking()
                         .Where(u => u.JournalType == JournalType.Invoice)
                          .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                         .WhereIf(input.Status != null && input.Status.Count > 0, u => input.Status.Contains(u.Status))
                         .WhereIf(!input.Filter.IsNullOrEmpty(),
                         u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()))
                         on jItem.JournalId equals j.Id

                         join invi in _invoiceItemRepository.GetAll().Include(u => u.SaleOrderItem).AsNoTracking()
                         .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                         on jItem.Identifier equals invi.Id

                         join inv in _invoiceRepository.GetAll().AsNoTracking()
                         .WhereIf(input.Customers != null && input.Customers.Count > 0, u => input.Customers.Contains(u.CustomerId))
                         on j.InvoiceId equals inv.Id
                         join c in _customerRepository.GetAll().AsNoTracking() on inv.CustomerId equals c.Id

                         group invi by new
                         {
                             Memo = j.Memo,
                             Date = j.Date,
                             JournalNo = j.JournalNo,
                             Status = j.Status,
                             Id = inv.Id,
                             Total = inv.Total,
                             CustomerId = c.Id,
                             DueDate = inv.DueDate,
                             OpenBalance = inv.OpenBalance,
                             PaidStatuse = inv.PaidStatus,
                             ReceiveStatus = inv.ReceivedStatus,
                             CustomerName = c.CustomerName,
                             CustomerCode = c.CustomerCode,


                         } into u
                         select new GetListInvoiceOutput
                         {
                             Date = u.Key.Date,
                             JournalNo = u.Key.JournalNo,
                             Status = u.Key.Status,
                             Id = u.Key.Id,
                             Total = u.Key.Total,
                             CustomerId = u.Key.CustomerId,
                             DueDate = u.Key.DueDate,
                             OpenBalance = u.Key.OpenBalance,
                             PaidStatus = u.Key.PaidStatuse,
                             ReceivedStatus = u.Key.ReceiveStatus,
                             Memo = u.Key.Memo,
                             Customer = new CustomerSummaryOutput
                             {
                                 CustomerCode = u.Key.CustomerCode,
                                 CustomerName = u.Key.CustomerName,
                                 Id = u.Key.CustomerId
                             }

                         });

            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<GetListInvoiceOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_GetDetail)]
        public async Task<InvoiceDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            await TurnOffTenantFilterIfDebug();

            var @journal = await _journalRepository
                                .GetAll()
                                .Include(u => u.Invoice)
                                .Include(u => u.Invoice.ItemIssue)
                                .Include(u => u.Invoice.TransactionTypeSale)
                                .Include(u => u.Class)
                                .Include(u => u.MultiCurrency)
                                .Include(u => u.Currency)
                                .Include(u => u.Location)
                                .Include(u => u.Invoice.Customer)
                                .Include(u => u.Invoice.ShippingAddress)
                                .Include(u => u.Invoice.BillingAddress)
                                .Include(u => u.CreatorUser)
                                .AsNoTracking()
                                .Where(u => u.JournalType == JournalType.Invoice && u.InvoiceId == input.Id)
                                .FirstOrDefaultAsync();

            if (@journal == null || @journal.Invoice == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var account = await (_journalItemRepository.GetAll()
                                       .Include(u => u.Account)
                                       .AsNoTracking()
                                       .Where(u => u.JournalId == journal.Id &&
                                                u.Key == PostingKey.AR && u.Identifier == null)
                                       .Select(u => u.Account)).FirstOrDefaultAsync();

            var invoiceItems = await _invoiceItemRepository.GetAll()
                .Include(u => u.Tax)
                .Include(u => u.ItemIssueItem.SaleOrderItem.SaleOrder)
                .Include(u=>u.ItemIssueItem.DeliveryScheduleItem.DeliverySchedule.SaleOrder)
                .Include(u => u.SaleOrderItem)
                .Include(u => u.Lot)
                .Include(u => u.SaleOrderItem.SaleOrder)
                .Include(u => u.DeliveryScheduleItem.DeliverySchedule.SaleOrder)
                .Include(u => u.Item)
                .Include(u => u.Tax)
                .Where(u => u.InvoiceId == input.Id)
                .AsNoTracking()
                .Join(
                    _journalItemRepository.GetAll().Include(u => u.Account).AsNoTracking(),
                    u => u.Id, s => s.Identifier,
                    (invItem, jItem) =>
                    new CreateOrUpdateInvoiceItemInput()
                    {
                        SaleOrderId = invItem.Invoice.ReceiveFrom == ReceiveFrom.SaleOrder ?
                            (invItem.SaleOrderItem != null ? invItem.SaleOrderItem.SaleOrderId : (Guid?)null) :
                            (invItem.ItemIssueItem != null && invItem.ItemIssueItem.SaleOrderItem != null ? invItem.ItemIssueItem.SaleOrderItem.SaleOrderId : (Guid?)null),
                        OrginalQtyFromSaleOrder = invItem.OrderItemId != null ? (invItem.SaleOrderItem != null ? invItem.SaleOrderItem.Qty : 0) - (invItem.ItemIssueItem != null && invItem.Invoice.ItemIssueId == null ? invItem.ItemIssueItem.Qty : 0) - (invItem != null ? invItem.Qty : 0) : 0,
                        CreationTime = invItem.CreationTime,
                        LotId = invItem.LotId,
                        LotDetail = ObjectMapper.Map<LotSummaryOutput>(invItem.Lot),
                        //CreationTime = bItem.CreationTime,
                        OrderNumber = invItem.SaleOrderItem != null ? invItem.SaleOrderItem.SaleOrder.OrderNumber :
                                      invItem.ItemIssueItem != null && invItem.ItemIssueItem.SaleOrderItem != null ? invItem.ItemIssueItem.SaleOrderItem.SaleOrder.OrderNumber :
                                      invItem.DeliveryScheduleItem != null ? invItem.DeliveryScheduleItem.DeliverySchedule.SaleOrder.OrderNumber :
                                      invItem.ItemIssueItem.DeliveryScheduleItem != null ? invItem.ItemIssueItem.DeliveryScheduleItem.DeliverySchedule.SaleOrder.OrderNumber : "",
                        OrderReference = invItem.SaleOrderItem != null ? invItem.SaleOrderItem.SaleOrder.Reference :
                                      invItem.ItemIssueItem != null && invItem.ItemIssueItem.SaleOrderItem != null ? invItem.ItemIssueItem.SaleOrderItem.SaleOrder.Reference :
                                      invItem.DeliveryScheduleItem != null ? invItem.DeliveryScheduleItem.DeliverySchedule.SaleOrder.Reference :
                                      invItem.ItemIssueItem.DeliveryScheduleItem != null ? invItem.ItemIssueItem.DeliveryScheduleItem.DeliverySchedule.SaleOrder.Reference : "",
                        SaleOrder = ObjectMapper.Map<SaleOrderSummaryOutput>(invItem.SaleOrderItem.SaleOrder),
                        DeliverySchedule = ObjectMapper.Map<DeliveryScheduleSummaryOutput>(invItem.DeliveryScheduleItem.DeliverySchedule),
                        Id = invItem.Id,
                        Item = ObjectMapper.Map<ItemSummaryDetailOutput>(invItem.Item),
                        ItemId = invItem.ItemId,
                        SaleOrderItem = ObjectMapper.Map<SaleOrderItemSummaryOut>(invItem.SaleOrderItem),
                        DeliveryScheduleItem = ObjectMapper.Map<DeliveryItemSummaryOut>(invItem.DeliveryScheduleItem),
                        AccountId = jItem.AccountId,
                        Account = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                        Description = invItem.Description,
                        DiscountRate = invItem.DiscountRate,
                        Qty = invItem.Qty,
                        TaxId = invItem.TaxId,
                        Tax = ObjectMapper.Map<TaxSummaryOutput>(invItem.Tax),
                        Total = invItem.Total,
                        UnitCost = invItem.UnitCost,
                        OrderItemId = invItem.Invoice.ReceiveFrom == ReceiveFrom.SaleOrder ? invItem.OrderItemId :
                            invItem.ItemIssueItem != null ? invItem.ItemIssueItem.SaleOrderItemId : (Guid?)null,
                        ItemIssueId = invItem.ItemIssueItemId,
                        IsItemIssue = invItem.IsItemIssue,
                        MultiCurrencyTotal = invItem.MultiCurrencyTotal,
                        MultiCurrencyUnitCost = invItem.MultiCurrencyUnitCost,
                        ParentId = invItem.ParentId,
                        DisplayInventoryAccount = invItem.Item == null ? false : invItem.Item.ItemType.DisplayInventoryAccount,
                        UseBatchNo = invItem.Item != null && invItem.Item.UseBatchNo,
                        TrackSerial = invItem.Item != null && invItem.Item.TrackSerial,
                        TrackExpiration = invItem.Item != null && invItem.Item.TrackExpiration,
                        DeliveryScheduleItemId = invItem.Invoice.ReceiveFrom == ReceiveFrom.DeliverySchedule ? invItem.DeliverySchedulItemId :
                            invItem.ItemIssueItem != null ? invItem.ItemIssueItem.DeliverySchedulItemId : (Guid?)null,
                        DeliveryId = invItem.Invoice.ReceiveFrom == ReceiveFrom.DeliverySchedule ?
                            (invItem.DeliveryScheduleItem != null ? invItem.DeliveryScheduleItem.DeliveryScheduleId : (Guid?)null) :
                            (invItem.ItemIssueItem != null && invItem.ItemIssueItem.DeliveryScheduleItem != null ? invItem.ItemIssueItem.DeliveryScheduleItem.DeliveryScheduleId : (Guid?)null),

                        DeliveryNo = invItem.Invoice.ReceiveFrom == ReceiveFrom.DeliverySchedule ?
                                      invItem.DeliveryScheduleItem.DeliverySchedule.DeliveryNo :
                                      invItem.ItemIssueItem != null && invItem.ItemIssueItem.DeliveryScheduleItem != null ?
                                      invItem.ItemIssueItem.DeliveryScheduleItem.DeliverySchedule.DeliveryNo : "",

                        DeliveryReference = invItem.Invoice.ReceiveFrom == ReceiveFrom.DeliverySchedule ?
                                          invItem.DeliveryScheduleItem.DeliverySchedule.Reference :
                                          invItem.ItemIssueItem != null && invItem.ItemIssueItem.DeliveryScheduleItem != null ?
                                          invItem.ItemIssueItem.DeliveryScheduleItem.DeliverySchedule.Reference : "",
                        OrginalQtyFromDeliverySchedule = invItem.DeliverySchedulItemId != null ? (invItem.DeliveryScheduleItem != null ? invItem.DeliveryScheduleItem.Qty : 0) - (invItem.ItemIssueItem != null && invItem.Invoice.ItemIssueId == null ? invItem.ItemIssueItem.Qty : 0) - (invItem != null ? invItem.Qty : 0) : 0,
                    })
                .OrderBy(u => u.CreationTime)
                .ToListAsync();

            //Map Key for assembly item
            var parentItemIds = invoiceItems.Where(s => s.ParentId.HasValue).GroupBy(s => s.ParentId).Select(s => s.Key).ToList();
            if (parentItemIds.Any())
            {
                var keyItems = invoiceItems.Where(s => parentItemIds.Contains(s.Id)).ToList();
                foreach (var k in keyItems)
                {
                    k.Key = k.Id;

                    var subItems = invoiceItems.Where(s => s.ParentId == k.Id).ToList();
                    foreach (var sub in subItems)
                    {
                        sub.Display = k.Item.ShowSubItems;
                    }
                }
            }

            var result = ObjectMapper.Map<InvoiceDetailOutput>(journal.Invoice);
            result.InvoiceNo = journal.JournalNo;
            result.LocationId = journal.LocationId.Value;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            result.Date = journal.Date;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.Reference = journal.Reference;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.MultiCurrency = ObjectMapper.Map<CurrencyDetailOutput>(journal.MultiCurrency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.InvoiceItems = invoiceItems;
            result.Account = ObjectMapper.Map<ChartAccountSummaryOutput>(account);
            result.AccountId = account.Id;
            result.Memo = journal.Memo;
            result.StatusCode = journal.Status;
            result.StatusName = journal.Status.ToString();
            result.TrasactionTypeSaleId = journal.Invoice.TransactionTypeSaleId;
            result.CreationTimeIndex = journal.CreationTimeIndex;
            result.UserName = journal.CreatorUser.FullName;

            if (result.UseExchangeRate)
            {
                result.ExchangeRate = await _exchangeRateRepository.GetAll().AsNoTracking()
                                            .Where(s => s.InvoiceId == input.Id)
                                            .Select(s => new GetExchangeRateDto
                                            {
                                                Id = s.Id,
                                                FromCurrencyCode = s.FromCurrency.Code,
                                                ToCurrencyCode = s.ToCurrency.Code,
                                                FromCurrencyId = s.FromCurrencyId,
                                                ToCurrencyId = s.ToCurrencyId,
                                                Bid = s.Bid,
                                                Ask = s.Ask,
                                                IsInves = s.FromCurrencyId == journal.CurrencyId
                                            })
                                            .FirstOrDefaultAsync();
            }

            if (journal.Invoice.ItemIssueId != null)
            {
                var itemIssue = await (from iis in _itemIssueRepository.GetAll()
                                       .Where(t => t.Id == journal.Invoice.ItemIssueId)
                                       join j in _journalRepository.GetAll()
                                       on iis.Id equals j.ItemIssueId
                                       select new
                                       {
                                           Id = iis.Id,
                                           Date = j.Date,
                                           JournalNo = j.JournalNo,
                                           Reference = j.Reference,
                                       }).FirstOrDefaultAsync();

                if (itemIssue != null)
                {
                    result.ItemIssueId = itemIssue.Id;
                    result.ItemIssueNo = itemIssue.JournalNo;
                    result.ItemIssueDate = itemIssue.Date;
                    result.ItemIssueReference = itemIssue.Reference;
                }

                var batchDic = await _itemIssueItemBatchNoRepository.GetAll()
                                    .AsNoTracking()
                                    .Where(s => s.ItemIssueItem.ItemIssueId == result.ItemIssueId)
                                    .Select(s => new BatchNoItemOutput
                                    {
                                        Id = s.Id,
                                        BatchNoId = s.BatchNoId,
                                        BatchNumber = s.BatchNo.Code,
                                        ExpirationDate = s.BatchNo.ExpirationDate,
                                        Qty = s.Qty,
                                        TransactionItemId = s.ItemIssueItemId
                                    })
                                    .GroupBy(s => s.TransactionItemId)
                                    .ToDictionaryAsync(k => k.Key, v => v.ToList());
                if (batchDic.Any())
                {
                    foreach (var i in invoiceItems)
                    {
                        if (i.ItemIssueId.HasValue && batchDic.ContainsKey(i.ItemIssueId.Value)) i.ItemBatchNos = batchDic[i.ItemIssueId.Value];
                    }
                }

            }
            return result;
        }

        public async Task UpdateInvoiceLink()
        {
            var noLinkItem = await (from ji in _journalItemRepository.GetAll()
                                   .Include(s => s.Journal)
                                   .Where(s => s.Journal.JournalType == JournalType.Invoice)
                                    join invi in _invoiceItemRepository.GetAll()
                                    on ji.Identifier equals invi.Id
                                    where ji.Journal.InvoiceId == null
                                    select new { InvoiceItem = invi, JorunalItem = ji, Journal = ji.Journal }
                                    into list
                                    group list by list.JorunalItem.JournalId
                                    into g
                                    select new { g.FirstOrDefault().InvoiceItem, g.FirstOrDefault().JorunalItem, g.FirstOrDefault().Journal }
                                    ).ToListAsync();

            foreach (var i in noLinkItem)
            {
                var journal = i.Journal;
                journal.UpdateInvoice(i.InvoiceItem.InvoiceId);
                await _journalRepository.UpdateAsync(journal);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_GetList)]
        public async Task<PagedResultDto<InvoiceHeader>> GetList(GetListInvoiceInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();

            var jQuery = _journalRepository.GetAll()
                        .Where(s => s.JournalType == JournalType.CustomerCredit || s.JournalType == JournalType.Invoice)
                        .WhereIf(input.ClassIds != null && input.ClassIds.Count() > 0, s => input.ClassIds.Contains(s.ClassId.Value))
                        .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                        .WhereIf(input.Status != null && input.Status.Count > 0, t => input.Status.Contains(t.Status))
                        .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                        .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId.Value))
                        .WhereIf(input.Type != null && input.Type.Count > 0, u => input.Type.Contains(u.JournalType))
                        .WhereIf(input.FromDate != null && input.ToDate != null,
                        (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                        .WhereIf(!input.Filter.IsNullOrEmpty(),
                            u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                            u.Reference.ToLower().Contains(input.Filter.ToLower()))
                        .AsNoTracking()
                        .Select(j => new
                        {
                            Id = j.Id,
                            CreationTimeIndex = j.CreationTimeIndex,
                            CreationTime = j.CreationTime,
                            JournalType = j.JournalType,
                            Memo = j.Memo,
                            Date = j.Date,
                            JournalNo = j.JournalNo,
                            Status = j.Status,
                            InvoiceId = j.InvoiceId,
                            CustomerCreditId = j.CustomerCreditId,
                            CurrencyId = j.CurrencyId,
                            LocationId = j.LocationId,
                            CreatorUserId = j.CreatorUserId,
                            Reference = j.Reference
                        });

            var currencyQuery = GetCurrencies();
            var locationQuery = GetLocations(null, input.Locations);
            var userQuery = GetUsers(input.Users);

            var journalQuery = from j in jQuery
                               join l in locationQuery
                               on j.LocationId equals l.Id
                               join c in currencyQuery
                               on j.CurrencyId equals c.Id
                               join u in userQuery
                               on j.CreatorUserId equals u.Id
                               select new
                               {
                                   Id = j.Id,
                                   CreationTimeIndex = j.CreationTimeIndex,
                                   CreationTime = j.CreationTime,
                                   JournalType = j.JournalType,
                                   Memo = j.Memo,
                                   Date = j.Date,
                                   JournalNo = j.JournalNo,
                                   Reference = j.Reference,
                                   Status = j.Status,
                                   InvoiceId = j.InvoiceId,
                                   CustomerCreditId = j.CustomerCreditId,
                                   CurrencyId = j.CurrencyId,
                                   LocationId = j.LocationId,
                                   CreatorUserId = j.CreatorUserId,
                                   CurrencyCode = c.Code,
                                   LocationName = l.LocationName,
                                   UserName = u.UserName
                               };


            var journalItemQuery = _journalItemRepository.GetAll()
                                   .Where(t => t.Key == PostingKey.AR && t.Identifier == null)
                                   .WhereIf(input.Accounts != null && input.Accounts.Count > 0, u => input.Accounts.Contains(u.AccountId))
                                   .AsNoTracking()
                                   .Select(s => new
                                   {
                                       Id = s.Id,
                                       AccountId = s.AccountId,
                                       JournalId = s.JournalId
                                   });

            var accountQuery = GetAccounts(input.Accounts);
            var jiaQuery = from ji in journalItemQuery
                           join a in accountQuery
                           on ji.AccountId equals a.Id
                           select new
                           {
                               Id = ji.Id,
                               AccountId = ji.AccountId,
                               JournalId = ji.JournalId,
                               AccountName = a.AccountName
                           };

            var jiQuery = from j in journalQuery
                          join ji in jiaQuery
                          on j.Id equals ji.JournalId
                          select new
                          {
                              Id = j.Id,
                              CreationTimeIndex = j.CreationTimeIndex,
                              CreationTime = j.CreationTime,
                              JournalType = j.JournalType,
                              Memo = j.Memo,
                              Date = j.Date,
                              JournalNo = j.JournalNo,
                              Reference = j.Reference,
                              Status = j.Status,
                              InvoiceId = j.InvoiceId,
                              CustomerCreditId = j.CustomerCreditId,
                              CurrencyId = j.CurrencyId,
                              LocationId = j.LocationId,
                              CreatorUserId = j.CreatorUserId,
                              CurrencyCode = j.CurrencyCode,
                              LocationName = j.LocationName,
                              UserName = j.UserName,
                              AccountId = ji.AccountId,
                              AccountName = ji.AccountName
                          };


            var invoiceQuery = _invoiceRepository.GetAll()
                               .WhereIf(input.PaidStatus != null && input.PaidStatus.Count > 0, u => input.PaidStatus.Contains(u.PaidStatus))
                               .WhereIf(input.Customers != null && input.Customers.Count > 0, u => input.Customers.Contains(u.CustomerId))
                               .WhereIf(input.TransactionSaleType != null && input.TransactionSaleType.Count > 0, s => input.TransactionSaleType.Contains(s.TransactionTypeSaleId.Value))
                               .AsNoTracking()
                               .Select(s => new
                               {
                                   Id = s.Id,
                                   Total = s.Total,
                                   TotalPaid = s.TotalPaid,
                                   DueDate = s.DueDate,
                                   OpenBalance = s.OpenBalance,
                                   PaidStatus = s.PaidStatus,
                                   ReceivedStatus = s.ReceivedStatus,
                                   CustomerId = s.CustomerId
                               });

            var invoiceItemQuery = _invoiceItemRepository.GetAll()
                                .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                                .AsNoTracking()
                                .Select(s => new
                                {
                                    InvoiceId = s.InvoiceId
                                });

            var iQuery = from i in invoiceQuery
                         join ii in invoiceItemQuery
                         on i.Id equals ii.InvoiceId
                         into iis
                         where iis.Count() > 0
                         select i;


            var customerTypeMemberIds = await GetCustomerTypeMembers().Select(s => s.CustomerTypeId).ToListAsync();
            var customerQuery = GetCustomers(null, input.Customers, input.CustomerTypes, customerTypeMemberIds);
            var icQuery = from i in iQuery
                          join c in customerQuery
                          on i.CustomerId equals c.Id
                          select new
                          {
                              Id = i.Id,
                              Total = i.Total,
                              TotalPaid = i.TotalPaid,
                              DueDate = i.DueDate,
                              OpenBalance = i.OpenBalance,
                              PaidStatus = i.PaidStatus,
                              ReceivedStatus = i.ReceivedStatus,
                              CustomerId = i.CustomerId,
                              CustomerName = c.CustomerName
                          };

            var customerCreditQuery = _customerCreditRepository.GetAll()
                                       .Where(s => input.TransactionSaleType == null || input.TransactionSaleType.Count == 0)
                                       .WhereIf(input.PaidStatus != null && input.PaidStatus.Count > 0, u => input.PaidStatus.Contains(u.PaidStatus))
                                       .WhereIf(input.Customers != null && input.Customers.Count > 0, u => input.Customers.Contains(u.CustomerId))
                                       .AsNoTracking()
                                       .Select(s => new
                                       {
                                           Id = s.Id,
                                           Total = s.Total,
                                           TotalPaid = s.TotalPaid,
                                           DueDate = s.DueDate,
                                           OpenBalance = s.OpenBalance,
                                           PaidStatus = s.PaidStatus,
                                           ShipedStatus = s.ShipedStatus,
                                           CustomerId = s.CustomerId
                                       });

            var customerCreditItemQuery = _customerCreditItemRepository.GetAll()
                                        .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                                        .AsNoTracking()
                                        .Select(s => new
                                        {
                                            CustomerCreditId = s.CustomerCreditId
                                        });

            var cQuery = from c in customerCreditQuery
                         join ci in customerCreditItemQuery
                         on c.Id equals ci.CustomerCreditId
                         into cis
                         where cis.Count() > 0
                         select c;

            var ccQuery = from cc in cQuery
                          join c in customerQuery
                          on cc.CustomerId equals c.Id
                          select new
                          {
                              Id = cc.Id,
                              Total = cc.Total,
                              TotalPaid = cc.TotalPaid,
                              DueDate = cc.DueDate,
                              OpenBalance = cc.OpenBalance,
                              PaidStatus = cc.PaidStatus,
                              ShipedStatus = cc.ShipedStatus,
                              CustomerId = cc.CustomerId,
                              CustomerName = c.CustomerName
                          };

            var union = from j in jiQuery

                        join i in icQuery
                        on j.InvoiceId equals i.Id
                        into invs
                        from inv in invs.DefaultIfEmpty()

                        join c in ccQuery
                        on j.CustomerCreditId equals c.Id
                        into ccs
                        from cc in ccs.DefaultIfEmpty()

                        where inv != null || cc != null
                        select new GetListInvoiceOutput
                        {
                            Currency = new CurrencyDetailOutput
                            {
                                Code = j.CurrencyCode,
                                Id = j.CurrencyId
                            },
                            CreationTimeIndex = j.CreationTimeIndex,
                            CreationTime = j.CreationTime,
                            CurrencyId = j.CurrencyId,
                            TypeName = j.JournalType.ToString(),
                            TypeCode = j.JournalType,
                            Memo = j.Memo,
                            Id = j.InvoiceId.HasValue ? j.InvoiceId.Value : j.CustomerCreditId.Value,
                            Date = j.Date,
                            JournalNo = j.JournalNo,
                            Reference = j.Reference,
                            Status = j.Status,
                            Total = inv != null ? inv.Total : cc.Total,
                            Customer = inv != null ?
                            new CustomerSummaryOutput
                            {
                                Id = inv.CustomerId,
                                CustomerName = inv.CustomerName
                            } :
                            new CustomerSummaryOutput
                            {
                                Id = cc.CustomerId,
                                CustomerName = cc.CustomerName
                            },
                            User = new UserDto
                            {
                                Id = j.CreatorUserId.Value,
                                UserName = j.UserName
                            },
                            AccountId = j.AccountId,
                            AccountName = j.AccountName,
                            LocationName = j.LocationName,

                            CustomerId = inv != null ? inv.CustomerId : cc.CustomerId,
                            TotalPaid = inv != null ? inv.TotalPaid : cc.TotalPaid,
                            DueDate = inv != null ? inv.DueDate : cc.DueDate,
                            OpenBalance = inv != null ? inv.OpenBalance : cc.OpenBalance,
                            PaidStatus = inv != null ? inv.PaidStatus : cc.PaidStatus,
                            ReceivedStatus = inv != null ? inv.ReceivedStatus : cc.ShipedStatus
                        };

            var resultCount = await union.CountAsync();
            if (resultCount == 0) return new PagedResultDto<InvoiceHeader>(resultCount, new List<InvoiceHeader>());


            if (input.Sorting.EndsWith("DESC"))
            {
                if (input.Sorting.ToLower().StartsWith("date"))
                {
                    var sortQeury = union.OrderByDescending(s => s.Date.Date).ThenByDescending(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("typename"))
                {
                    var sortQeury = union.OrderByDescending(s => s.TypeName).ThenByDescending(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("journalno"))
                {
                    var sortQeury = union.OrderByDescending(s => s.JournalNo).ThenByDescending(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("total"))
                {
                    var sortQeury = union.OrderByDescending(s => s.Total).ThenByDescending(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("openbalance"))
                {
                    var sortQeury = union.OrderByDescending(s => s.OpenBalance).ThenByDescending(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("duedate"))
                {
                    var sortQeury = union.OrderByDescending(s => s.DueDate.Date).ThenByDescending(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("paidstatus"))
                {
                    var sortQeury = union.OrderByDescending(s => s.PaidStatus).ThenByDescending(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("receivedstatus"))
                {
                    var sortQeury = union.OrderByDescending(s => s.ReceivedStatus).ThenByDescending(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("status"))
                {
                    var sortQeury = union.OrderByDescending(s => s.Status).ThenByDescending(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    var sortQeury = union.OrderBy(input.Sorting).ThenByDescending(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
            }
            else
            {
                if (input.Sorting.ToLower().StartsWith("date"))
                {
                    var sortQeury = union.OrderBy(s => s.Date.Date).ThenBy(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("typename"))
                {
                    var sortQeury = union.OrderBy(s => s.TypeName).ThenBy(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("journalno"))
                {
                    var sortQeury = union.OrderBy(s => s.JournalNo).ThenBy(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("total"))
                {
                    var sortQeury = union.OrderBy(s => s.Total).ThenBy(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("openbalance"))
                {
                    var sortQeury = union.OrderBy(s => s.OpenBalance).ThenBy(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("duedate"))
                {
                    var sortQeury = union.OrderBy(s => s.DueDate.Date).ThenBy(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("paidstatus"))
                {
                    var sortQeury = union.OrderBy(s => s.PaidStatus).ThenBy(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("receivedstatus"))
                {
                    var sortQeury = union.OrderBy(s => s.ReceivedStatus).ThenBy(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("status"))
                {
                    var sortQeury = union.OrderBy(s => s.Status).ThenBy(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    var sortQeury = union.OrderBy(input.Sorting).ThenBy(s => s.CreationTimeIndex);
                    union = sortQeury;
                }
            }

            var query = await union.PageBy(input).ToListAsync();

            var summary = new List<BalanceSummary>();

            if (query.Count > 0)
            {
                // get total record of summary for first index
                summary.Add(
                    new BalanceSummary
                    {
                        Balance = query.Sum(t => t.TypeCode == JournalType.CustomerCredit ? t.OpenBalance * -1 : t.OpenBalance),
                        chartOfAccount = null,
                        Total = query.Sum(t => t.TypeCode == JournalType.CustomerCredit ? t.Total * -1 : t.Total),
                        TotalPaid = query.Sum(t => t.TypeCode == JournalType.CustomerCredit ? t.TotalPaid : t.TotalPaid * -1)
                        //TotalPaid = query.Where(t => t.PaidStatus == PaidStatuse.Paid).Sum(t => t.TypeCode == JournalType.CustomerCredit ? t.Total * -1 : t.Total)
                    }
                );
                var allAcc = query.GroupBy(ct => new { ct.AccountName }).Select(a => new BalanceSummary
                {
                    Balance = a.Sum(t => t.TypeCode == JournalType.CustomerCredit ? t.OpenBalance * -1 : t.OpenBalance),
                    chartOfAccount = a.Key.AccountName,
                    Total = a.Sum(t => t.TypeCode == JournalType.CustomerCredit ? t.Total * -1 : t.Total),
                    TotalPaid = a.Sum(t => t.TypeCode == JournalType.CustomerCredit ? t.TotalPaid : t.TotalPaid * -1) //a.Where(t => t.PaidStatus == PaidStatuse.Paid).Sum(t => t.TypeCode == JournalType.CustomerCredit ? t.Total * -1 : t.Total)
                }).ToList();

                summary = summary.Concat(allAcc).ToList();
            }

            var headersQuery = new List<InvoiceHeader> {
                new InvoiceHeader
                {
                    BalanceSummary = summary,
                    InvoiceList = query
                }
            };

            var @entities = headersQuery;


            return new PagedResultDto<InvoiceHeader>(resultCount, @entities);
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_GetList, AppPermissions.Pages_Tenant_Customers_Invoice_ListSaleReturn)]
        public async Task<PagedResultDto<GetListInvoiceOutput>> GetListSaleReturn(GetListInvoiceInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var query = from j in _journalRepository.GetAll()
                                     .Include(u => u.CustomerCredit.Customer)
                                     .Include(u => u.Currency)
                                     .Include(u => u.Location)
                                     .Where(s => s.JournalType == JournalType.CustomerCredit && s.CustomerCredit.IsPOS)
                                     .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                                     .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                                     .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId.Value))
                                     .WhereIf(input.PaidStatus != null && input.PaidStatus.Count > 0, u => u.CustomerCredit != null && input.PaidStatus.Contains(u.CustomerCredit.PaidStatus))
                                     .WhereIf(input.Customers != null && input.Customers.Count > 0, u => u.CustomerCredit != null && input.Customers.Contains(u.CustomerCredit.CustomerId))
                                     .WhereIf(input.FromDate != null && input.ToDate != null,
                                      (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                                     .WhereIf(!input.Filter.IsNullOrEmpty(),
                                      u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                      u.Reference.ToLower().Contains(input.Filter.ToLower()) ||
                                      u.Memo.ToLower().Contains(input.Filter.ToLower()))
                                      .AsNoTracking()
                        join ji in _journalItemRepository.GetAll()
                       .Where(t => t.Key == PostingKey.AR && t.Identifier == null)
                       .WhereIf(input.Accounts != null && input.Accounts.Count > 0, u => input.Accounts.Contains(u.AccountId))
                       .AsNoTracking()
                       on j.Id equals ji.JournalId
                       into jis
                        join ini in _invoiceItemRepository.GetAll()
                                      .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                                      .AsNoTracking()
                                      on j.InvoiceId equals ini.InvoiceId
                         into bitems
                        join ccd in _customerCreditItemRepository.GetAll()
                          .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                          .AsNoTracking()
                       on j.CustomerCreditId equals ccd.CustomerCreditId
                        into ccditems
                        where ((bitems != null && bitems.Count() > 0) || (ccditems != null && ccditems.Count() > 0)) && jis != null && jis.Count() > 0

                        select new GetListInvoiceOutput
                        {
                            Currency = new CurrencyDetailOutput
                            {
                                Code = j.Currency.Code,
                                Id = j.Currency.Id,
                                Name = j.Currency.Name,
                                PluralName = j.Currency.PluralName,
                                Symbol = j.Currency.Symbol
                            },
                            CreationTimeIndex = j.CreationTimeIndex,
                            CurrencyId = j.CurrencyId,
                            TypeName = j.JournalType.ToString(),
                            TypeCode = j.JournalType,
                            Memo = j.Memo,
                            Id = j.CustomerCreditId.Value,
                            Date = j.Date,
                            JournalNo = j.JournalNo,
                            Status = j.Status,
                            Total = j.CustomerCredit.MultiCurrencyTotal,
                            Customer = new CustomerSummaryOutput
                            {
                                Id = j.CustomerCredit.CustomerId,
                                CustomerCode = j.CustomerCredit.Customer.CustomerCode,
                                CustomerName = j.CustomerCredit.Customer.CustomerName
                            },
                            User = new UserDto
                            {
                                Id = j.CreatorUserId.Value,
                                UserName = j.CreatorUser.Name
                            },
                            CustomerId = j.CustomerCredit.CustomerId,
                            TotalPaid = j.CustomerCredit.MultiCurrencyTotalPaid,
                            DueDate = j.CustomerCredit.DueDate,
                            OpenBalance = j.CustomerCredit.MultiCurrencyOpenBalance,
                            PaidStatus = j.CustomerCredit.PaidStatus,
                            ReceivedStatus = j.CustomerCredit.ShipedStatus,
                            AccountId = jis.FirstOrDefault().AccountId,
                            AccountName = jis.FirstOrDefault().Account.AccountName,
                            LocationName = j.Location.LocationName
                        };


            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            return new PagedResultDto<GetListInvoiceOutput>(resultCount, @entities);
        }



        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_Invoice_ListSaleReturn_Unpaid)]
        public async Task<PagedResultDto<GetListInvoiceOutput>> GetListSaleReturnUpaid(GetListInvoiceInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var query = from j in _journalRepository.GetAll()
                                     .Include(u => u.CustomerCredit.Customer)
                                     .Include(u => u.Currency)
                                     .Include(u => u.Location)
                                     .Where(s => s.JournalType == JournalType.CustomerCredit && s.CustomerCredit.IsPOS && s.CustomerCredit.PaidStatus != PaidStatuse.Paid)
                                     .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                                     .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                                     .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId.Value))
                                     .WhereIf(input.PaidStatus != null && input.PaidStatus.Count > 0, u => u.CustomerCredit != null && input.PaidStatus.Contains(u.CustomerCredit.PaidStatus))
                                     .WhereIf(input.Customers != null && input.Customers.Count > 0, u => u.CustomerCredit != null && input.Customers.Contains(u.CustomerCredit.CustomerId))
                                     .WhereIf(input.FromDate != null && input.ToDate != null,
                                      (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                                     .WhereIf(!input.Filter.IsNullOrEmpty(),
                                      u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                      u.Reference.ToLower().Contains(input.Filter.ToLower()) ||
                                      u.Memo.ToLower().Contains(input.Filter.ToLower()))
                                      .AsNoTracking()
                        join ji in _journalItemRepository.GetAll()
                       .Where(t => t.Key == PostingKey.AR && t.Identifier == null)
                       .WhereIf(input.Accounts != null && input.Accounts.Count > 0, u => input.Accounts.Contains(u.AccountId))
                       .AsNoTracking()
                       on j.Id equals ji.JournalId
                       into jis
                        join ini in _invoiceItemRepository.GetAll()
                                      .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                                      .AsNoTracking()
                                      on j.InvoiceId equals ini.InvoiceId
                         into bitems
                        join ccd in _customerCreditItemRepository.GetAll()
                          .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                          .AsNoTracking()
                       on j.CustomerCreditId equals ccd.CustomerCreditId
                        into ccditems
                        where ((bitems != null && bitems.Count() > 0) || (ccditems != null && ccditems.Count() > 0)) && jis != null && jis.Count() > 0

                        select new GetListInvoiceOutput
                        {
                            Currency = new CurrencyDetailOutput
                            {
                                Code = j.Currency.Code,
                                Id = j.Currency.Id,
                                Name = j.Currency.Name,
                                PluralName = j.Currency.PluralName,
                                Symbol = j.Currency.Symbol
                            },
                            CreationTimeIndex = j.CreationTimeIndex,
                            CurrencyId = j.CurrencyId,
                            TypeName = j.JournalType.ToString(),
                            TypeCode = j.JournalType,
                            Memo = j.Memo,
                            Id = j.CustomerCreditId.Value,
                            Date = j.Date,
                            JournalNo = j.JournalNo,
                            Status = j.Status,
                            Total = j.CustomerCredit.MultiCurrencyTotal,
                            Customer = new CustomerSummaryOutput
                            {
                                Id = j.CustomerCredit.CustomerId,
                                CustomerCode = j.CustomerCredit.Customer.CustomerCode,
                                CustomerName = j.CustomerCredit.Customer.CustomerName
                            },
                            User = new UserDto
                            {
                                Id = j.CreatorUserId.Value,
                                UserName = j.CreatorUser.Name
                            },
                            CustomerId = j.CustomerCredit.CustomerId,
                            TotalPaid = j.CustomerCredit.MultiCurrencyTotalPaid,
                            DueDate = j.CustomerCredit.DueDate,
                            OpenBalance = j.CustomerCredit.MultiCurrencyOpenBalance,
                            PaidStatus = j.CustomerCredit.PaidStatus,
                            ReceivedStatus = j.CustomerCredit.ShipedStatus,
                            AccountId = jis.FirstOrDefault().AccountId,
                            AccountName = jis.FirstOrDefault().Account.AccountName,
                            LocationName = j.Location.LocationName
                        };


            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            return new PagedResultDto<GetListInvoiceOutput>(resultCount, @entities);
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_Update)]
        public async Task<NullableIdDto<Guid>> Update(UpdateInvoiceInput input)
        {

            //validate billItem when create by none
            if (input.InvoiceItems.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }

            if (input.IsConfirm == false)
            {
                var validateLockDate = await _lockRepository.GetAll()
                                      .Where(t => t.IsLock == true &&
                                      (t.LockDate.Value.Date >= input.DateCompare.Value.Date || t.LockDate.Value.Date >= input.Date.Date)
                                      && t.LockKey == TransactionLockType.Invoice).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            //validation Qty on hand form SO
            if (input.ReceiveFrom == ReceiveFrom.SaleOrder)
            {
                var validates = input.InvoiceItems.Where(t => t.OrderItemId != null).Select(t => new { ItemName = t.ItemName, OriginalQty = t.OrginalQtyFromSaleOrder, OrderItemId = t.OrderItemId }).GroupBy(t => t.OrderItemId).ToList();
                foreach (var v in validates)
                {
                    var sumQtyByOrderItems = input.InvoiceItems.Where(g => g.OrderItemId == v.Key).Sum(t => t.Qty);
                    if (sumQtyByOrderItems > v.FirstOrDefault().OriginalQty)
                    {
                        throw new UserFriendlyException(L("InvoiceMessageWarning", v.First().ItemName));
                    }
                }
            }
            else if (input.ReceiveFrom == ReceiveFrom.ItemIssue)
            {
                foreach (var v in input.InvoiceItems.Where(s => s.OrderItemId.HasValue))
                {
                    var sumQtyByOrderItems = input.InvoiceItems.Where(g => g.OrderItemId == v.OrderItemId).Sum(t => t.Qty);
                    await ValidateOrderQtyForIssue(v.OrderItemId.Value, sumQtyByOrderItems, v.ItemIssueId);
                }
            }


            //validate invoice with has item issue receive from by manual 
            var validatInvoice = await (from inv in _invoiceRepository.GetAll()
                                  .Where(v => v.Id == input.Id && v.ItemIssueId != null && v.ConvertToItemIssue == false)
                                  .AsNoTracking()
                                        join itemIssue in _itemIssueRepository.GetAll().AsNoTracking()
                                        on inv.ItemIssueId equals itemIssue.Id
                                        where (itemIssue.CreationTime > inv.CreationTime)
                                        select inv).CountAsync();
            if (validatInvoice > 0)
            {
                throw new UserFriendlyException(L("InvoiceWarningMessage"));
            }

            // validate from Payment
            var validateReceivePayment = await (from paymentDetail in _receivePaymentDetailRepository.GetAll()
                                                .Where(t => t.InvoiceId == input.Id).AsNoTracking()
                                                select paymentDetail)
                                          .CountAsync();

            if (validateReceivePayment > 0)
            {
                throw new UserFriendlyException(L("RecordHasReceivePayment"));
            }

            int indexLot = 0;
            foreach (var c in input.InvoiceItems)
            {
                indexLot++;
                if (c.LotId == null && c.ItemId != null && c.DisplayInventoryAccount)
                {
                    throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + indexLot.ToString());
                }
            }

            if (input.ConvertToItemIssue || input.ReceiveFrom == ReceiveFrom.ItemIssue) await ValidateBatchNo(input);

            ValidateExchangeRate(input);
            await ValidateApplyOrderItems(input);
            await CalculateTotal(input);

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            #region update journal for invoice
            var @journal = await _journalRepository.GetAll().Include(u => u.Invoice)
                                  .Where(u => u.JournalType == JournalType.Invoice && u.InvoiceId == input.Id)
                                  .FirstOrDefaultAsync();
            if (@journal == null || @journal.Invoice == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            await CheckClosePeriod(journal.Date, input.Date);

            journal.Update(tenantId, input.InvoiceNo, input.Date, input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, 0, input.LocationId);
            journal.UpdateStatus(input.Status);

            if (input.MultiCurrencyId == null || input.MultiCurrencyId == 0 || input.CurrencyId == input.MultiCurrencyId)
            {
                input.MultiCurrencyId = input.CurrencyId;
                input.MultiCurrencyTotal = input.Total;
                input.MultiCurrencySubTotal = input.SubTotal;

            }
            journal.UpdateMultiCurrency(input.MultiCurrencyId);

            //update AR account 
            var accountItem = await (_journalItemRepository.GetAll()
                                      .Where(u => u.JournalId == journal.Id &&
                                               u.Key == PostingKey.AR && u.Identifier == null)
                                    ).FirstOrDefaultAsync();
            accountItem.UpdateJournalItem(tenantId, input.AccountId, input.Memo, input.Total, 0);

            //update invoice 
            var invoice = @journal.Invoice;
            var copyConvertToItemIssue = invoice.ConvertToItemIssue;
            var invoiceItemIssueId = invoice.ItemIssueId;
            var orginalReceiveFrom = invoice.ReceiveFrom;
            // update balance
            decimal amount = input.Total - invoice.Total;
            invoice.UpdateOpenBalance(amount);

            //calculate balnce and Update multi currency
            var multiamount = input.MultiCurrencyTotal - invoice.MultiCurrencyTotal;
            invoice.UpdateMultiCurrencyOpenBalance(multiamount);

            var isItem = input.InvoiceItems.Any(s => s.ItemId != null && s.ItemId != Guid.Empty);

            invoice.Update(tenantId,
                          input.ReceiveFrom,
                          input.CustomerId,
                          input.DueDate,
                          input.SameAsShippingAddress,
                          input.ShippingAddress,
                          input.BillingAddress,
                          input.SubTotal,
                          input.Tax,
                          input.Total,
                          input.ETD,
                          input.IssuingDate,
                          input.ConvertToItemIssue,
                          input.MultiCurrencySubTotal,
                          input.MultiCurrencyTax,
                          input.MultiCurrencyTotal,
                          isItem);

            // update item issue id to null if the orginal receive from issue then user change to other item issue must null
            if (orginalReceiveFrom == ReceiveFrom.ItemIssue && input.ReceiveFrom != ReceiveFrom.ItemIssue)
            {
                invoice.UpdateItemIssueId(null);
            }
            else
            {
                invoice.UpdateItemIssueId(input.ItemIssueId);
            }
            invoice.UpdateTransactionTypeId(input.TransactionTypeId);
            var itemIssueItems = new List<ItemIssueItem>();
            var @inventoryJournalItemsForItemIssue = new List<JournalItem>();
            if (input.ReceiveFrom == ReceiveFrom.ItemIssue || input.ConvertToItemIssue == true)
            {
                if (input.ItemIssueId == null && input.ReceiveFrom == ReceiveFrom.ItemIssue)
                {
                    throw new UserFriendlyException(L("PleaseAddItemIssue"));
                }

                invoice.UpdateReceivedStatus(DeliveryStatus.ShipAll);
                
                {

                    #region Calculat Cost
                    //var roundingId = (await GetCurrentTenantAsync()).RoundDigitAccountId;

                    //var itemToCalculateCost = input.InvoiceItems.Select((u, index) => new Inventories.Data.GetAvgCostForIssueData()
                    //{
                    //    ItemName = u.Item.ItemName,
                    //    Index = index,
                    //    ItemId = u.ItemId.Value,
                    //    ItemCode = u.Item.ItemCode,
                    //    ItemTypeId = u.Item.ItemTypeId,
                    //    Qty = u.Qty,
                    //    //ItemIssueItemId = u.Id,
                    //    ItemIssueItemId = input.ItemIssueId,
                    //    LotId = u.LotId,
                    //    InventoryAccountId = itemIssueJournal.JournalItem.Where(t => t.Identifier == u.ItemIssueId).Select(g => g.AccountId).FirstOrDefault(),
                    //}).Where(s => s.ItemTypeId != 3).ToList();

                    //var lotIds = input.InvoiceItems.Where(t=> t.LotId != null).Select(x => x.LotId).ToList();
                    //var locationIds = await _lotRepository.GetAll().AsNoTracking()
                    //                .Where(t => lotIds.Contains(t.Id))
                    //                .Select(t => (long?)t.LocationId)
                    //                .ToListAsync();
                    //var getCostResult = await _inventoryManager.GetAvgCostForIssues(input.IssuingDate.Value, locationIds, itemToCalculateCost, itemIssueJournal.Journal, roundingId);

                    //foreach (var r in getCostResult.Items)
                    //{
                    //    input.InvoiceItems[r.Index].UnitCostItemIssue = r.UnitCost;
                    //    input.InvoiceItems[r.Index].TotalItemIssue = r.LineCost;
                    //}

                    //var TotalItemIssue = getCostResult.Total;
                    #endregion Calculat Cost
                }


                itemIssueItems = await _itemIssueItemRepository.GetAll()
                                    .Where(t => t.ItemIssueId == input.ItemIssueId).ToListAsync();

                @inventoryJournalItemsForItemIssue = await (_journalItemRepository.GetAll()
                                                .Include(u => u.Journal.ItemIssue)
                                                .Where(u => u.Journal.ItemIssueId == input.ItemIssueId &&
                                                 u.Identifier != null)
                                            ).ToListAsync();

            }

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Invoice);
            CheckErrors(await _journalManager.UpdateAsync(@journal, auto.DocumentType));

            CheckErrors(await _journalItemManager.UpdateAsync(accountItem));
            CheckErrors(await _invoiceManager.UpdateAsync(invoice));
            #endregion

            #region update Item invoice
            //Update invoice Item and Journal Item
            var invoiceItems = await _invoiceItemRepository.GetAll()
                                    .Include(u=>u.SaleOrderItem)
                                    .Include(u=>u.DeliveryScheduleItem)
                                    .Include(s => s.ItemIssueItem.SaleOrderItem)
                                    .Include(s => s.ItemIssueItem.DeliveryScheduleItem)
                                    .Where(u => u.InvoiceId == input.Id).ToListAsync();

            var oldOrderIds = invoiceItems
                           .Where(s => s.ItemIssueItem != null && s.ItemIssueItem.SaleOrderItem != null)
                           .GroupBy(s => s.ItemIssueItem.SaleOrderItem.SaleOrderId)
                           .Select(s => s.Key).ToList();

            var oldDeliveryIds = invoiceItems
                         .Where(s => s.ItemIssueItem != null && s.ItemIssueItem.DeliveryScheduleItem != null)
                         .GroupBy(s => s.ItemIssueItem.DeliveryScheduleItem.DeliveryScheduleId)
                         .Select(s => s.Key).ToList();


            var @inventoryJournalItems = await (_journalItemRepository.GetAll()
                                            .Where(u => u.JournalId == journal.Id &&
                                            u.Key == PostingKey.Revenue && u.Identifier != null)
                                        ).ToListAsync();

            var toDeleteInvoiceItem = invoiceItems.Where(u => !input.InvoiceItems.Any(i => i.Id != null && i.Id == u.Id)).ToList();
            var toDeletejournalItem = inventoryJournalItems.Where(u => !input.InvoiceItems.Any(i => u.Identifier != null && i.Id != null && i.Id == u.Identifier)).ToList();

            if(copyConvertToItemIssue)
            {
                var deleteOrderIds = toDeleteInvoiceItem
                         .Where(s => s.SaleOrderItem != null)
                         .GroupBy(s => s.SaleOrderItem.SaleOrderId)
                         .Select(s => s.Key).ToList();
                var deletedeliveryIds = toDeleteInvoiceItem
                        .Where(s => s.DeliveryScheduleItem != null)
                        .GroupBy(s => s.DeliveryScheduleItem.DeliveryScheduleId)
                        .Select(s => s.Key).ToList();
                 oldOrderIds.AddRange(deleteOrderIds);
                 oldDeliveryIds.AddRange(deletedeliveryIds);
            }
          

            var subInvoiceItems = new List<KeyValuePair<CreateOrUpdateInvoiceItemInput, InvoiceItem>>();

            foreach (var c in input.InvoiceItems)
            {

                if (input.CurrencyId == input.MultiCurrencyId)
                {
                    c.MultiCurrencyTotal = c.Total;
                    c.MultiCurrencyUnitCost = c.UnitCost;
                }
                if (c.LotId == 0)
                {
                    c.LotId = null;
                }
                if (c.Id != null) //update
                {
                    var invoiceItem = invoiceItems.FirstOrDefault(u => u.Id == c.Id);
                    var journalItem = @inventoryJournalItems.FirstOrDefault(u => u.Identifier == c.Id);
                    if (invoiceItem != null)
                    {
                        invoiceItem.Update(tenantId, c.TaxId, c.ItemId, c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total, c.MultiCurrencyUnitCost, c.MultiCurrencyTotal);
                        invoiceItem.UpdateLot(c.LotId);
                        if (input.ReceiveFrom == ReceiveFrom.SaleOrder)
                        {
                            if (c.Qty > c.OrginalQtyFromSaleOrder)
                            {
                                throw new UserFriendlyException(L("InvoiceMessageWarning", c.ItemName) + L("Row") + " " + indexLot.ToString());
                            }
                            invoiceItem.UpdateOrderItemId(c.OrderItemId);
                        }

                        if (input.ReceiveFrom == ReceiveFrom.DeliverySchedule)
                        {
                            if (c.Qty > c.OrginalQtyFromDeliverySchedule)
                            {
                                throw new UserFriendlyException(L("InvoiceMessageWarning", c.ItemName) + L("Row") + " " + indexLot.ToString());
                            }
                            invoiceItem.SetDeliverySchedulItem(c.DeliveryScheduleItemId);
                        }
                        CheckErrors(await _invoiceItemManager.UpdateAsync(invoiceItem));


                    }

                    if (journalItem != null)
                    {
                        journalItem.UpdateJournalItem(userId, c.AccountId, c.Description, 0, c.Total);
                        CheckErrors(await _journalItemManager.UpdateAsync(journalItem));
                    }
                }
                else if (c.Id == null) //create
                {
                    //insert to invoice item
                    var invoiceItem = InvoiceItem.Create(tenantId, userId, invoice, c.TaxId, c.ItemId, c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total, c.MultiCurrencyUnitCost, c.MultiCurrencyTotal);

                    if (input.ReceiveFrom == ReceiveFrom.SaleOrder)
                    {
                        if (c.Qty > c.OrginalQtyFromSaleOrder)
                        {
                            throw new UserFriendlyException(L("InvoiceMessageWarning", c.ItemName) + L("Row") + " " + indexLot.ToString());
                        }
                        invoiceItem.UpdateOrderItemId(c.OrderItemId);
                    }
                    if (input.ReceiveFrom == ReceiveFrom.DeliverySchedule)
                    {
                        if (c.Qty > c.OrginalQtyFromDeliverySchedule)
                        {
                            throw new UserFriendlyException(L("InvoiceMessageWarning", c.ItemName) + L("Row") + " " + indexLot.ToString());
                        }
                        invoiceItem.SetDeliverySchedulItem(c.DeliveryScheduleItemId);
                    }
                    invoiceItem.UpdateLot(c.LotId);

                    c.Id = invoiceItem.Id;
                   

                    CheckErrors(await _invoiceItemManager.CreateAsync(invoiceItem));

                    if (c.Key.HasValue || c.ParentId.HasValue) subInvoiceItems.Add(new KeyValuePair<CreateOrUpdateInvoiceItemInput, InvoiceItem>(c, invoiceItem));

                    //insert Revenue journal item into credit
                    var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal, c.AccountId, c.Description, 0, c.Total, PostingKey.Revenue, invoiceItem.Id);
                    CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));
                }
            }

            var keys = input.InvoiceItems.Where(s => s.Key.HasValue).GroupBy(s => s.Key.Value).Select(s => s.Key).ToHashSet();
            foreach (var key in keys)
            {
                var parent = subInvoiceItems.FirstOrDefault(s => s.Key.Key == key);
                var subitems = subInvoiceItems.Where(s => s.Key.ParentId == key).Select(s => s.Value);
                foreach (var sub in subitems)
                {
                    sub.SetParent(parent.Value.Id);
                }
            }


            foreach (var t in toDeleteInvoiceItem)
            {
                CheckErrors(await _invoiceItemManager.RemoveAsync(t));
            }

            foreach (var t in toDeletejournalItem)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(t));
            }
            #endregion
            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.Invoice };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            /************************************************************
             * Create Item Issue when check auto convert to item issue  *
             * **********************************************************/
            if (copyConvertToItemIssue == false
                && input.ConvertToItemIssue == true
                && input.Status == TransactionStatus.Publish)
            {
                //validate if items list doesn't have atleast one item 
                var getItems = await _itemRepository.GetAll()
                                    .Where(g => g.ItemType.DisplayInventoryAccount)
                                    .Where(g => input.InvoiceItems.Any(i => i.ItemId == g.Id))
                                    .AsNoTracking()
                                    .ToListAsync();

                if (input.IssuingDate == null)
                {
                    throw new UserFriendlyException(L("PleaseAddReciveDate"));
                }


                //@todo map properties of create item issue input 
                var itemIssueInput = new CreateItemIssueInput()
                {
                    IsConfirm = input.IsConfirm,
                    TransactionTypeId = input.TransactionTypeId,
                    BillingAddress = input.BillingAddress,
                    ShippingAddress = input.ShippingAddress,
                    Status = input.Status,
                    ClassId = input.ClassId,
                    CurrencyId = input.CurrencyId,
                    CustomerId = input.CustomerId,
                    Date = input.IssuingDate.Value,
                    InvoiceId = invoice.Id,
                    IssueNo = input.InvoiceNo,
                    LocationId = input.LocationId,
                    Memo = input.Memo,
                    ReceiveFrom = ReceiveFrom.Invoice,
                    Reference = input.Reference,
                    SameAsShippingAddress = input.SameAsShippingAddress,
                    Total = input.Total,
                    Items = input.InvoiceItems.Where(d => getItems.Any(v => v.Id == d.ItemId))
                        .Select(t => new CreateOrUpdateItemIssueItemInput()
                        {
                            LotId = t.LotId,
                            ItemId = t.ItemId.Value,
                            Description = t.Description,
                            DiscountRate = t.DiscountRate,
                            Qty = t.Qty,
                            //SaleOrderId = t.SaleOrderId,
                            //SaleOrderItemId = t.OrderItemId != null ? t.OrderItemId : null,
                            PurchaseAccountId = getItems.Where(s => s.Id == t.ItemId.Value)
                                                .Select(x => x.PurchaseAccountId).FirstOrDefault().Value,
                            InventoryAccountId = getItems
                                                .Where(s => s.Id == t.ItemId.Value)
                                                .Select(x => x.InventoryAccountId).FirstOrDefault().Value,
                            Item = t.Item,
                            InvoiceItemId = t.Id,
                            ItemBatchNos = t.ItemBatchNos
                        }).ToList()
                };
                await ItemIssueSave(itemIssueInput);

            }

            /**************************************************************************************************
             * Update Item Issue when before invoice is check auto convert then user want to update something *
             * ************************************************************************************************/
            else if (copyConvertToItemIssue == true
               && input.ConvertToItemIssue == true
               && input.Status == TransactionStatus.Publish)
            {
                //validate if items list doesn't have atleast one item 
                var getItems = await _itemRepository.GetAll()
                                     .Where(g => g.ItemType.DisplayInventoryAccount)
                                     .Where(g => input.InvoiceItems.Any(i => i.ItemId == g.Id))
                                     .AsNoTracking()
                                     .ToListAsync();

                // map properties of create item issue input 
                var itemIssueInput = new UpdateItemIssueInput()
                {
                    IsConfirm = input.IsConfirm,
                    TransactionTypeId = input.TransactionTypeId,
                    Id = invoice.ItemIssueId.Value,
                    BillingAddress = input.BillingAddress,
                    ShippingAddress = input.ShippingAddress,
                    Status = input.Status,
                    ClassId = input.ClassId,
                    CurrencyId = input.CurrencyId,
                    CustomerId = input.CustomerId,
                    Date = input.IssuingDate.Value,
                    InvoiceId = invoice.Id,
                    IssueNo = input.InvoiceNo,
                    LocationId = input.LocationId,
                    Memo = input.Memo,
                    ReceiveFrom = ReceiveFrom.Invoice,
                    Reference = input.Reference,
                    SameAsShippingAddress = input.SameAsShippingAddress,
                    Total = input.Total,
                    Items = input.InvoiceItems.Where(d => getItems.Any(v => v.Id == d.ItemId))
                        .Select(t => new CreateOrUpdateItemIssueItemInput()
                        {
                            LotId = t.LotId,
                            Id = t.ItemIssueId,
                            ItemId = t.ItemId.Value,
                            Description = t.Description,
                            DiscountRate = t.DiscountRate,
                            Qty = t.Qty,
                            //SaleOrderId = t.SaleOrderId,
                            //SaleOrderItemId = t.OrderItemId != null ? t.OrderItemId : (Guid?)null,
                            PurchaseAccountId = getItems
                                                .Where(s => s.Id == t.ItemId.Value)
                                                .Select(x => x.PurchaseAccountId).FirstOrDefault().Value,
                            InventoryAccountId = getItems
                                                .Where(s => s.Id == t.ItemId.Value)
                                                .Select(x => x.InventoryAccountId).FirstOrDefault().Value,
                            Item = t.Item,
                            InvoiceItemId = t.Id,
                            ItemBatchNos = t.ItemBatchNos
                        }).ToList()
                };
                var itemIssueJournal = inventoryJournalItemsForItemIssue.Select(u => u.Journal).FirstOrDefault();
                await UpdateItemIssue(itemIssueInput, false, tenantId, userId, invoice.Id, itemIssueItems, itemIssueJournal, inventoryJournalItemsForItemIssue);
            }

            /**********************************************************************************************
             * Delete Item Issue when Before invoice is check auto convert Then user uncheck auto convert *
             **********************************************************************************************/
            else if (copyConvertToItemIssue == true
               && input.ConvertToItemIssue == false
               && input.Status == TransactionStatus.Publish)
            {
                var inputItemIssue = new CarlEntityDto() { IsConfirm = input.IsConfirm, Id = invoiceItemIssueId.Value };
                await DeleteItemIssue(inputItemIssue);
            }


            if (input.ReceiveFrom == ReceiveFrom.ItemIssue && input.ItemIssueId != null)
            {
                var itemIssue = await _itemIssueRepository.GetAll().Where(t => t.Id == input.ItemIssueId).FirstOrDefaultAsync();
                var itemIssueInput = new UpdateItemIssueInput()
                {
                    IsConfirm = input.IsConfirm,
                    Id = input.ItemIssueId.Value,
                    TransactionTypeId = input.TransactionTypeId,
                    BillingAddress = input.BillingAddress,
                    ShippingAddress = input.ShippingAddress,
                    Status = input.Status,
                    ClassId = input.ClassId,
                    CurrencyId = input.CurrencyId,
                    CustomerId = input.CustomerId,
                    Date = input.IssuingDate.Value,
                    InvoiceId = invoice.Id,
                    IssueNo = input.InvoiceNo,
                    LocationId = input.LocationId,
                    Memo = input.Memo,
                    ReceiveFrom = itemIssue.ReceiveFrom,
                    ConvertToInvoice = itemIssue.ConvertToInvoice,
                    Reference = input.Reference,
                    SameAsShippingAddress = input.SameAsShippingAddress,
                    Total = input.Total,
                    Items = input.InvoiceItems
                            .Where(t => t.Item.InventoryAccountId != null)
                            .Select(t => new CreateOrUpdateItemIssueItemInput()
                            {
                                Id = t.ItemIssueId,
                                LotId = t.LotId,
                                ItemId = t.ItemId.Value,
                                Description = t.Description,
                                DiscountRate = t.DiscountRate,
                                Qty = t.Qty,
                                SaleOrderId = t.SaleOrderId,
                                SaleOrderItemId = t.OrderItemId,
                                PurchaseAccountId = t.Item.PurchaseAccountId.Value,
                                InventoryAccountId = t.Item.InventoryAccountId.Value,
                                Item = t.Item,
                                InvoiceItemId = t.Id,
                                ItemBatchNos = t.ItemBatchNos
                            }).ToList()
                };
                var itemIssueJournal = inventoryJournalItemsForItemIssue.Select(u => u.Journal).FirstOrDefault();
                await UpdateItemIssue(itemIssueInput, false, tenantId, userId, invoice.Id, itemIssueItems, itemIssueJournal, inventoryJournalItemsForItemIssue);
            }

            var exchange = await _exchangeRateRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.InvoiceId == input.Id);
            if (input.UseExchangeRate && input.CurrencyId != input.MultiCurrencyId)
            {
                if (exchange == null)
                {
                    exchange = InvoiceExchangeRate.Create(tenantId, userId, invoice.Id, input.ExchangeRate.FromCurrencyId, input.ExchangeRate.ToCurrencyId, input.ExchangeRate.Bid, input.ExchangeRate.Ask);
                    await _exchangeRateRepository.InsertAsync(exchange);
                }
                else
                {
                    exchange.Update(userId, invoice.Id, input.ExchangeRate.FromCurrencyId, input.ExchangeRate.ToCurrencyId, input.ExchangeRate.Bid, input.ExchangeRate.Ask);
                    await _exchangeRateRepository.UpdateAsync(exchange);
                }
            }
            else if (exchange != null)
            {
                await _exchangeRateRepository.DeleteAsync(exchange);
            }

            var newOrderIds = input.InvoiceItems.Where(s => s.SaleOrderId.HasValue).GroupBy(s => s.SaleOrderId).Select(s => s.Key.Value);
            var newDeliveryIds = input.InvoiceItems.Where(s => s.DeliveryId.HasValue).GroupBy(s => s.DeliveryId).Select(s => s.Key.Value);

            var orderIds = oldOrderIds.Union(newOrderIds).ToList();
            var deliveryIds = oldDeliveryIds.Union(newDeliveryIds).ToList();
            if (orderIds.Any() || deliveryIds.Any())
            {
                await CurrentUnitOfWork.SaveChangesAsync();
                foreach (var orderId in orderIds)
                {
                    await UpdateOrderInventoryStatus(orderId);
                }
                foreach (var deliveryId in deliveryIds)
                {
                    await UpdateDeliveryInventoryStatus(deliveryId,true);
                }
            }


            return new NullableIdDto<Guid>() { Id = invoice.Id };

        }

        //[AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_UpdateToDraft)]
        //public async Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input)
        //{
        //    //validate if invoice has convert to item issue  
        //    var validatInvoice = (from inv in _invoiceRepository.GetAll()
        //                          .Where(v => v.Id == input.Id && v.ItemIssueId != null)
        //                          join itemIssue in _itemIssueRepository.GetAll()
        //                          on inv.ItemIssueId equals itemIssue.Id
        //                          where (inv.CreationTime < itemIssue.CreationTime)
        //                          select inv).Count();
        //    if (validatInvoice > 0)
        //    {
        //        throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
        //    }
        //    // validate from Payment
        //    var validateReceivePayment = (from paymentDetail in _receivePaymentDetailRepository.GetAll().Where(t => t.InvoiceId == input.Id) select paymentDetail).ToList().Count();

        //    if (validateReceivePayment > 0)
        //    {
        //        throw new UserFriendlyException(L("RecordHasReceivePayment"));
        //    }

        //    var @entity = await _journalRepository
        //                        .GetAll()
        //                        .Include(u => u.Invoice)
        //                        .Where(u => u.JournalType == JournalType.Invoice && u.InvoiceId == input.Id)
        //                        .FirstOrDefaultAsync();
        //    if (entity == null)
        //    {
        //        throw new UserFriendlyException(L("RecordNotFound"));
        //    }
        //    if (entity.Invoice.ItemIssueId != null && entity.Invoice.ConvertToItemIssue == true)
        //    {//remove if has convert to item issue by has relationship with invoice 
        //        var inputItemIssue = new CarlEntityDto() {IsConfirm = input.is Id = entity.Invoice.ItemIssueId.Value };
        //        await DeleteItemIssue(inputItemIssue);
        //        //update item issue id to null after delete item issue from invoice 
        //        entity.Invoice.UpdateItemIssueId(null);
        //    }

        //    var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Invoice);
        //    CheckErrors(await _journalManager.UpdateAsync(entity, auto.DocumentType));

        //    entity.UpdateStatusToDraft();
        //    return new NullableIdDto<Guid>() { Id = entity.Id };
        //}



        private async Task DeleteItemIssue(CarlEntityDto input)
        {
            //validate when use in customercredit and itemReceiptCustomerCredit
            var customerCredit = await _cusotmerCreditRepository.GetAll().Where(t => t.ItemIssueSaleId == input.Id).CountAsync();
            var itemReceiptCustomerCredit = await _itemReceiptCustomerCreditRepository.GetAll().Where(t => t.ItemIssueSaleId == input.Id).CountAsync();

            if (customerCredit > 0 || itemReceiptCustomerCredit > 0)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChangeInCustomerCredit"));
            }
            var @jounal = await _journalRepository.GetAll()
                        .Include(u => u.ItemIssue)
                        .Include(u => u.ItemIssue.ShippingAddress)
                        .Include(u => u.ItemIssue.BillingAddress)
                        .Where(u => (u.JournalType == JournalType.ItemIssueSale ||
                                    u.JournalType == JournalType.ItemIssueOther ||
                                        u.JournalType == JournalType.ItemIssueTransfer ||
                                    u.JournalType == JournalType.ItemIssueAdjustment ||
                                    u.JournalType == JournalType.ItemIssueVendorCredit)
                                    && u.ItemIssueId == input.Id)
                        .FirstOrDefaultAsync();



            //query get item Receipt

            if (input.IsConfirm == false)
            {

                var locktransaction = await _lockRepository.GetAll()
                                       .Where(t => (t.LockKey == TransactionLockType.ItemIssue || t.LockKey == TransactionLockType.InventoryTransaction)
                                       && t.IsLock == true && t.LockDate != null && t.LockDate.Value.Date >= jounal.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            var @entity = @jounal.ItemIssue;
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);

            if (jounal.JournalNo == auto.LastAutoSequenceNumber)
            {
                var jo = await _journalRepository.GetAll()
                    .Where(t => t.Id != jounal.Id &&
                    (t.JournalType == JournalType.ItemIssueSale ||
                                  t.JournalType == JournalType.ItemIssueKitchenOrder ||
                                    t.JournalType == JournalType.ItemIssueOther ||
                                    t.JournalType == JournalType.ItemIssueTransfer ||
                                    t.JournalType == JournalType.ItemIssueAdjustment ||
                                    t.JournalType == JournalType.ItemIssueProduction))
                    .OrderByDescending(t => t.CreationTime).FirstOrDefaultAsync();
                if (jo != null)
                {
                    auto.UpdateLastAutoSequenceNumber(jo.JournalNo);
                }
                else
                {
                    auto.UpdateLastAutoSequenceNumber(null);
                }
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            //qurey check creationTime Item Receipt and Bill
            var UpdateItemIssueIdOnInvoice = await _invoiceRepository.GetAll()
              .Include(u => u.ItemIssue)
              .Where(u => u.ItemIssueId == input.Id).FirstOrDefaultAsync();

            if (UpdateItemIssueIdOnInvoice != null)
            {
                UpdateItemIssueIdOnInvoice.UpdateItemIssueId(null);
                UpdateItemIssueIdOnInvoice.UpdateReceivedStatus(DeliveryStatus.ShipPending);
                CheckErrors(await _invoiceManager.UpdateAsync(UpdateItemIssueIdOnInvoice));
            }
            var @ItemIssues = (from ItemIssue in _itemIssueRepository.GetAll()
                                 .Where(u => u.Id == input.Id)
                               join invoice in _invoiceRepository.GetAll() on ItemIssue.Id equals invoice.ItemIssueId
                               where (ItemIssue.CreationTime < invoice.CreationTime)
                               select ItemIssue);


            if (ItemIssues != null && ItemIssues.Count() > 0)
            {
                throw new UserFriendlyException(L("ItemIssueMessage"));
            }
            //await UpdateSOReceiptStautus(input.Id, @entity.ReceiveFrom, UpdateItemIssueIdOnInvoice.Id, null);


            //query get journal and delete

            @jounal.UpdateItemIssue(null);

            //query get journal item and delete
            var @jounalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == jounal.Id).ToListAsync();
            foreach (var ji in jounalItems)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(ji));
            }

            var scheduleDate = @jounal.Date;

            CheckErrors(await _journalManager.RemoveAsync(@jounal));


            var itemBatchNos = await _itemIssueItemBatchNoRepository.GetAll().Where(s => s.ItemIssueItem.ItemIssueId == input.Id).AsNoTracking().ToListAsync();
            if (itemBatchNos.Any())
            {
                await _itemIssueItemBatchNoRepository.BulkDeleteAsync(itemBatchNos);
            }

            //query get item receipt item and delete 
            var @ItemIssueItems = await _itemIssueItemRepository.GetAll().Include(u => u.SaleOrderItem)
                .Where(u => u.ItemIssueId == entity.Id).ToListAsync();

            //var querySOHeader = (from po in _saleOrderRepository.GetAll()
            //                     join pi in _saleOrderItemRepository.GetAll() on po.Id equals pi.SaleOrderId
            //                     where (@ItemIssueItems.Any(t => t.SaleOrderItemId == pi.Id))
            //                     group pi by po into u
            //                     select new { poId = u.Key.Id });

            // temp of po id header 
            //  var listOfSoHeader = new List<CreateOrUpdateItemIssueItemInput>();

            //foreach (var i in querySOHeader)
            //{
            //    listOfSoHeader.Add(new CreateOrUpdateItemIssueItemInput
            //    {
            //        SaleOrderId = i.poId
            //    });
            //}

            //Update ItemIssueitemId on table invoice item
            var updateItemIssueitemId = (from invoiceitem in _invoiceItemRepository.GetAll()
                                         join ItemIssueItem in _itemIssueItemRepository.GetAll()
                                         .Include(u => u.ItemIssue)
                                         on invoiceitem.ItemIssueItemId equals ItemIssueItem.Id
                                         where (ItemIssueItem.ItemIssueId == entity.Id)
                                         select invoiceitem);
            foreach (var u in updateItemIssueitemId)
            {
                u.UpdateIssueItemId(null);
                CheckErrors(await _invoiceItemManager.UpdateAsync(u));
            }

            var scheduleItems = ItemIssueItems.Select(s => s.ItemId).Distinct().ToList();

            foreach (var iri in @ItemIssueItems)
            {
                CheckErrors(await _itemIssueItemManager.RemoveAsync(iri));
            }
            CheckErrors(await _itemIssueManager.RemoveAsync(entity));

            await DeleteInventoryTransactionItems(input.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, scheduleDate, scheduleItems);
            }

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_UpdateToPublish)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input)
        {

            //validate payment 
            var validatinvoice = (from inv in _invoiceRepository.GetAll()
                                .Where(v => v.Id == input.Id && v.ItemIssueId != null)
                                  join itemIssue in _itemIssueRepository.GetAll() on inv.ItemIssueId
                                                      equals itemIssue.Id
                                  where (itemIssue.CreationTime > inv.CreationTime)
                                  select inv).Count();
            if (validatinvoice > 0)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            // validate from Payment
            var validateReceivePayment = (from paymentDetail in _receivePaymentDetailRepository.GetAll().Where(t => t.InvoiceId == input.Id) select paymentDetail).ToList().Count();

            if (validateReceivePayment > 0)
            {
                throw new UserFriendlyException(L("RecordHasReceivePayment"));
            }
            var @entity = await _journalRepository
                                .GetAll()
                                .Include(u => u.Invoice)
                                .Where(u => u.JournalType == JournalType.Invoice && u.InvoiceId == input.Id)
                                .FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            entity.UpdatePublish();

            // starting create item issue if it has convert to item issue
            if (entity.Invoice.ConvertToItemIssue == true)
            {
                var itemIssueInput = new CreateItemIssueInput()
                {
                    BillingAddress = entity.Invoice.BillingAddress,
                    ShippingAddress = entity.Invoice.ShippingAddress,
                    Status = entity.Status,
                    ClassId = entity.ClassId,
                    CurrencyId = entity.CurrencyId,
                    CustomerId = entity.Invoice.CustomerId,
                    Date = entity.Invoice.ReceiveDate.Value,
                    InvoiceId = entity.Invoice.Id,
                    IssueNo = entity.JournalNo,
                    LocationId = entity.LocationId.Value,
                    Memo = entity.Memo,
                    ReceiveFrom = ReceiveFrom.Invoice,
                    Reference = entity.Reference,
                    SameAsShippingAddress = entity.Invoice.SameAsShippingAddress,
                    Total = entity.Invoice.Total,
                    Items = _invoiceItemRepository.GetAll().AsNoTracking()
                            .Include(t => t.SaleOrderItem)
                            .Include(t => t.Item)
                            .Where(t => t.InvoiceId == entity.Invoice.Id)
                            .Select(t => new CreateOrUpdateItemIssueItemInput()
                            {
                                ItemId = t.ItemId.Value,
                                Description = t.Description,
                                DiscountRate = t.DiscountRate,
                                UnitCost = t.UnitCost,
                                Qty = t.Qty,
                                Total = t.Total,
                                SaleOrderId = t.SaleOrderItem.SaleOrderId,
                                SaleOrderItemId = t.SaleOrderItem != null ? t.SaleOrderItem.Id : Guid.Empty,
                                PurchaseAccountId = t.Item.PurchaseAccountId.Value,
                                InventoryAccountId = t.Item.InventoryAccountId.Value,
                                Item = new ItemSummaryDetailOutput()
                                {
                                    Id = t.Item.Id,
                                    InventoryAccountId = t.Item.InventoryAccountId,
                                    ItemCode = t.Item.ItemCode,
                                    ItemName = t.Item.ItemName,
                                    PurchaseAccountId = t.Item.PurchaseAccountId,
                                    PurchaseTaxId = t.Item.PurchaseTaxId,
                                    SaleAccountId = t.Item.SaleAccountId,
                                    SalePrice = t.Item.SalePrice,
                                    SaleTaxId = t.Item.SaleTaxId
                                },
                                InvoiceItemId = t.Id
                            }).ToList()
                };
                await ItemIssueSave(itemIssueInput);

                entity.Invoice.UpdateReceivedStatus(DeliveryStatus.ShipAll);
            }
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Invoice);
            CheckErrors(await _journalManager.UpdateAsync(entity, auto.DocumentType));

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_UpdateToVoid)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input)
        {
            //validate payment 
            var validatinvoice = (from inv in _invoiceRepository.GetAll()
                                .Where(v => v.Id == input.Id && v.ItemIssueId != null && v.ConvertToItemIssue == false)
                                  join itemIssue in _itemIssueRepository.GetAll() on inv.ItemIssueId
                                                      equals itemIssue.Id
                                  where (itemIssue.CreationTime > inv.CreationTime)
                                  select inv).Count();
            if (validatinvoice > 0)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            // validate from Payment
            var validateReceivePayment = (from paymentDetail in _receivePaymentDetailRepository.GetAll().Where(t => t.InvoiceId == input.Id) select paymentDetail).ToList().Count();

            if (validateReceivePayment > 0)
            {
                throw new UserFriendlyException(L("RecordHasReceivePayment"));
            }
            var @entity = await _journalRepository
                                .GetAll()
                                .Include(u => u.Invoice)
                                .Where(u => u.JournalType == JournalType.Invoice && u.InvoiceId == input.Id)
                                .FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            entity.UpdateVoid();
            // starting update item issue to void if it has convert to item issue and has item issue id
            if (entity.Invoice.ConvertToItemIssue == true && entity.Invoice.ItemIssueId != null)
            {
                var @jounalItemIssue = await _journalRepository.GetAll()
                 .Include(u => u.ItemIssue)
                 .Include(u => u.Bill)
                 .Include(u => u.ItemIssue.ShippingAddress)
                 .Include(u => u.ItemIssue.BillingAddress)
                 .Where(u => (u.JournalType == JournalType.ItemIssueSale ||
                        u.JournalType == JournalType.ItemIssueOther ||
                        u.JournalType == JournalType.ItemIssueTransfer ||
                        u.JournalType == JournalType.ItemIssueAdjustment ||
                        u.JournalType == JournalType.ItemIssueVendorCredit)
                        && u.ItemIssueId == entity.Invoice.ItemIssueId)
                 .FirstOrDefaultAsync();
                //query get item issue item and update total Qty 
                if (@jounalItemIssue.JournalType == JournalType.ItemIssueSale)
                {
                    var invoice = (_invoiceRepository.GetAll()
                        .Where(u => u.ItemIssueId == @jounalItemIssue.ItemIssueId))
                        .FirstOrDefault();

                    if (@jounalItemIssue.ItemIssue.ReceiveFrom == ReceiveFrom.Invoice)
                    {
                        invoice.UpdateReceivedStatus(DeliveryStatus.ShipPending);
                        CheckErrors(await _invoiceManager.UpdateAsync(invoice));
                    }
                    //query get item receipt item and update totalItemIssue 
                    var @ItemIssueItems = await _itemIssueItemRepository.GetAll().Include(u => u.SaleOrderItem)
                        .Where(u => u.ItemIssueId == @jounalItemIssue.ItemIssueId).ToListAsync();
                    //var querySOHeader = (from po in _saleOrderRepository.GetAll()
                    //                     join pi in _saleOrderItemRepository.GetAll().Include(u => u.Item) on po.Id equals pi.SaleOrderId
                    //                     where (@ItemIssueItems.Any(t => t.SaleOrderItemId == pi.Id))
                    //                     group pi by po into u
                    //                     select new { poId = u.Key.Id });
                    // temp of po id header 
                    //var listOfPoHeader = new List<CreateOrUpdateItemIssueItemInput>();
                    //foreach (var i in querySOHeader)
                    //{
                    //    listOfPoHeader.Add(new CreateOrUpdateItemIssueItemInput
                    //    {
                    //        SaleOrderId = i.poId
                    //    });
                    //}
                    //if (@jounalItemIssue.ItemIssue.ReceiveFrom == ReceiveFrom.Invoice && @jounalItemIssue.ItemIssue != null)
                    //{

                    //    foreach (var iri in @ItemIssueItems)
                    //    {
                    //        if (iri.SaleOrderItemId != null && iri.SaleOrderItem != null &&
                    //            iri.SaleOrderItem.TotalIssueInvoiceQty > 0
                    //            && @jounalItemIssue.Status == TransactionStatus.Publish)
                    //        {
                    //            //Update Purchase Order item when delete Item Receipt                   
                    //            var decraseQty = (iri.Qty * -1);
                    //            iri.SaleOrderItem.IncreaseInvoiceQty(decraseQty);
                    //            iri.SaleOrderItem.IncreaseTotalIssueInvoiceQty(decraseQty);
                    //            CheckErrors(await _saleOrderItemManager.UpdateAsync(iri.SaleOrderItem));
                    //        }
                    //    }
                    //    //update Receive status to pending
                    //    var saleOrders = (from so in _saleOrderRepository.GetAll()
                    //                      join soi in _saleOrderItemRepository.GetAll().Include(u => u.Item) on so.Id equals soi.SaleOrderId
                    //                      where listOfPoHeader.Any(t => t.SaleOrderId == soi.SaleOrderId && t.InventoryAccountId != null)
                    //                      group soi by new { saleOrder = so } into u
                    //                      select new
                    //                      {
                    //                          po = u.Key.saleOrder,
                    //                          totalQty = u.Sum(t => t.TotalIssueInvoiceQty + t.IssueQty),
                    //                          originalQty = u.Sum(t => t.Qty),
                    //                      });


                    //    foreach (var i in saleOrders)
                    //    {
                    //        if (i.totalQty == i.originalQty)
                    //        {
                    //            i.po.UpdateReceiveStatusToReceiveAll();
                    //            CheckErrors(await _saleOrderManager.UpdateAsync(i.po, false));
                    //        }
                    //        else if (i.totalQty == 0)
                    //        {
                    //            i.po.UpdateReceiveStatusToPending();
                    //            CheckErrors(await _saleOrderManager.UpdateAsync(i.po, false));
                    //        }
                    //        else
                    //        {
                    //            i.po.UpdateReceiveStatusToPartial();
                    //            CheckErrors(await _saleOrderManager.UpdateAsync(i.po, false));
                    //        }

                    //    }
                    //    // update OP status from bill
                    //}
                }
                @jounalItemIssue.UpdateVoid();
            }
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Invoice);
            CheckErrors(await _journalManager.UpdateAsync(entity, auto.DocumentType));

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_GetInvoiceItems)]
        public async Task<InvoiceSummaryOutputForGetInvoiceItem> GetInvoiceItems(SaleOrderGetlistInputForIssue input)
        {
            var @journal = await _journalRepository
                                  .GetAll()
                                  .Include(u => u.Currency)
                                  .Include(u => u.Invoice)
                                  .Include(u => u.Invoice.TransactionTypeSale)
                                  .Include(u => u.Currency)
                                  .Include(u => u.Location)
                                  .Include(u => u.Invoice.Customer)
                                  .Include(u => u.Invoice.Customer.Account)
                                  .Include(u => u.Class)
                                  .Include(u => u.Invoice.ShippingAddress)
                                  .Include(u => u.Invoice.BillingAddress)
                                  .AsNoTracking()
                                  .Where(u => u.JournalType == JournalType.Invoice && u.InvoiceId == input.Id)
                                  .FirstOrDefaultAsync();

            if (@journal == null || @journal.Invoice == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var ARAccount = await (_journalItemRepository.GetAll()
                            .Include(u => u.Account)
                            .AsNoTracking()
                            .Where(u => u.JournalId == journal.Id && u.Key == PostingKey.AR && u.Identifier == null)
                            .Select(u => u.Account)).FirstOrDefaultAsync();

            var locations = new List<long?>() {
                journal.LocationId
            };

            //var averageCosts = await _inventoryManager.GetAvgCostQuery(input.Date, locations).ToListAsync();

            var invoiceItems = await (from i in _invoiceItemRepository.GetAll()
                                                .Include(u => u.Tax)
                                                .Include(u => u.ItemIssueItem)
                                                .Include(u => u.Lot)
                                                .Include(u => u.SaleOrderItem)
                                                .Include(u => u.Tax)
                                                .Where(u => u.InvoiceId == input.Id)
                                                .AsNoTracking()
                                      join j in _journalItemRepository.GetAll()
                                                .Include(u => u.Account)
                                                .AsNoTracking()
                                      on i.Id equals j.Identifier
                                      join ii in _itemRepository.GetAll()
                                                 .Include(s => s.PurchaseAccount)
                                                 .Include(s => s.SaleAccount)
                                                 .Include(s => s.InventoryAccount)
                                                 .Include(s => s.SaleTax)
                                                 .Where(s => s.InventoryAccountId.HasValue)
                                                 .AsNoTracking()
                                      on i.ItemId equals ii.Id
                                      orderby i.CreationTime
                                      select new InvoiceItemSummaryOutput()
                                      {
                                          SaleOrderId = i.SaleOrderItem != null ? i.SaleOrderItem.SaleOrderId : (Guid?)null,
                                          LotId = i.LotId,
                                          LotDetail = i.Lot == null ? null : ObjectMapper.Map<LotSummaryOutput>(i.Lot),
                                          CreationTime = i.CreationTime,
                                          Id = i.Id,
                                          Item = new ItemSummaryDetailOutput
                                          {
                                              InventoryAccount = ii.InventoryAccount == null ? null : ObjectMapper.Map<ChartAccountDetailOutput>(ii.InventoryAccount),
                                              Id = ii.Id,
                                              InventoryAccountId = ii.InventoryAccountId,
                                              ItemCode = ii.ItemCode,
                                              ItemName = ii.ItemName,
                                              PurchaseAccount = ii.PurchaseAccount == null ? null : ObjectMapper.Map<ChartAccountDetailOutput>(ii.PurchaseAccount),
                                              PurchaseAccountId = ii.PurchaseAccountId,
                                              SaleAccount = ii.SaleAccount == null ? null : ObjectMapper.Map<ChartAccountDetailOutput>(ii.SaleAccount),
                                              SaleAccountId = ii.SaleAccountId,
                                              SalePrice = ii.SalePrice,
                                              SaleTax = ii.SaleTax == null ? null : ObjectMapper.Map<TaxDetailOutput>(ii.SaleTax),
                                              SaleTaxId = ii.SaleTaxId,
                                              UseBatchNo = ii.UseBatchNo,
                                              AutoBatchNo = ii.AutoBatchNo
                                          },
                                          ItemId = i.ItemId,
                                          OrderItemId = i.OrderItemId,
                                          Description = i.Description,
                                          Qty = i.Qty,
                                          Tax = i.Tax == null ? null : ObjectMapper.Map<TaxSummaryOutput>(i.Tax),
                                          Total = i.Total,
                                          UnitCost = i.UnitCost,
                                          TaxId = i.TaxId,
                                          InventoryAccountId = j.AccountId,
                                          InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(j.Account),
                                          UseBatchNo = ii.UseBatchNo,
                                          TrackSerial = ii.TrackSerial,
                                          TrackExpiration = ii.TrackExpiration
                                      })
                                    .ToListAsync();

            var result = ObjectMapper.Map<InvoiceSummaryOutputForGetInvoiceItem>(journal.Invoice);
            result.TransactionSaleTypeId = journal.Invoice.TransactionTypeSaleId;
            result.BillNo = journal.JournalNo;
            result.Date = journal.Date;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.Reference = journal.Reference;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.InvoiceItems = invoiceItems;
            result.Account = ObjectMapper.Map<ChartAccountSummaryOutput>(ARAccount);
            result.AccountId = ARAccount.Id;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            result.LocationId = journal.LocationId.Value;
            result.Memo = journal.Memo;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            result.LocationId = journal.LocationId.Value;
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_GetInvoiceSummaryForItemIssue)]
        public async Task<PagedResultDto<InvoiceSummaryOutput>> GetInvoices(GetInvoiceListInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var @query = (from b in _invoiceRepository.GetAll().AsNoTracking()
                          //.WhereIf(input.Customers != null && input.Customers.Count > 0, u => input.Customers.Contains(u.CustomerId))
                          .Where(u => u.ItemIssueId == null)
                          join bi in _invoiceItemRepository.GetAll()
                          .Include(u => u.Item)
                          .Include(u => u.Tax)
                          .Include(u => u.ItemIssueItem)
                          .Where(u => u.ItemId.Value != null)
                          .AsNoTracking()
                          on b.Id equals bi.InvoiceId into p
                          join j in _journalRepository.GetAll()
                          .Include(u => u.Currency)
                          //.WhereIf(input.Locations != null && input.Locations.Count > 0, t => input.Locations.Contains(t.LocationId))
                          .Where(u => u.Status == TransactionStatus.Publish)
                          .WhereIf(userGroups != null && userGroups.Count > 0,
                                u => userGroups.Contains(u.LocationId.Value))
                          .WhereIf(!input.Filter.IsNullOrEmpty(),
                                u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) || u.Reference.ToLower().Contains(input.Filter.ToLower()))
                          .AsNoTracking()
                          on b.Id equals j.InvoiceId
                          select new InvoiceSummaryOutput
                          {
                              Customer = ObjectMapper.Map<CustomerSummaryOutput>(b.Customer),
                              CustomerId = b.CustomerId,
                              Memo = j.Memo,
                              Id = b.Id,
                              InvoiceNo = j.JournalNo,
                              CurrencyId = j.CurrencyId,
                              Currency = ObjectMapper.Map<CurrencyDetailOutput>(j.Currency),
                              Date = j.Date,
                              Reference = j.Reference,
                              Total = b.Total,
                              CountItems = p.Where(r => r.Item.InventoryAccountId != null).Count(),
                              ETD = b.ETD,
                              CreationIndex = j.CreationTimeIndex
                          }).Where(u => u.CountItems > 0);
            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).ThenBy(t => t.CreationIndex).PageBy(input).ToListAsync();
            return new PagedResultDto<InvoiceSummaryOutput>(resultCount, @entities);

        }

        public async Task<PagedResultDto<getInvoiceListOutput>> GetListInvoiceForReceivePayment(GetListInvoiceForPaybillInput input)
        {
            var accountCycle = await GetCurrentCycleAsync();
            // get user by group member
            var userGroups = await GetUserGroupByLocation();

            var jQuery = _journalRepository.GetAll()
                        .Where(u => u.JournalType == JournalType.Invoice)
                        .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                        .Where(u => u.Status == TransactionStatus.Publish)
                        .WhereIf(
                        input.FromDate != null && input.ToDate != null,
                        (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date))
                        )
                        .WhereIf(input.Locations != null && input.Locations.Count > 0, u => u.Location != null && input.Locations.Contains(u.LocationId.Value))
                        .AsNoTracking()
                        .Select(s => new
                        {
                            s.Id,
                            s.InvoiceId,
                            s.Date,
                            s.JournalNo,
                            s.Memo,
                            s.MultiCurrencyId,
                            s.Reference
                        });

            var currencyQuery = GetCurrencies();
            var journalQuery = from j in jQuery
                               join c in currencyQuery
                               on j.MultiCurrencyId equals c.Id
                               select new
                               {
                                   j.Id,
                                   j.InvoiceId,
                                   j.Date,
                                   j.JournalNo,
                                   j.Memo,
                                   j.MultiCurrencyId,
                                   MultiCurrencyCode = c.Code,
                                   j.Reference
                               };


            var journalItemQuery = _journalItemRepository.GetAll()
                                   .Where(s => s.Identifier == null && s.Key == PostingKey.AR)
                                   .AsNoTracking()
                                   .Select(s => new
                                   {
                                       s.JournalId,
                                       s.AccountId
                                   });

            var jiQuery = from j in journalQuery
                          join ji in journalItemQuery
                          on j.Id equals ji.JournalId
                          select new
                          {
                              j.Id,
                              j.InvoiceId,
                              j.Date,
                              j.JournalNo,
                              j.Memo,
                              j.MultiCurrencyId,
                              j.MultiCurrencyCode,
                              ji.AccountId,
                              j.Reference
                          };

            var iQuery = _invoiceRepository.GetAll()
                        .Where(x => x.OpenBalance > 0)
                        .WhereIf(input.InvoiceNo != null && input.InvoiceNo.Count > 0, u => input.InvoiceNo.Contains(u.Id))
                        .WhereIf(input.Customers != null && input.Customers.Count > 0, u => input.Customers.Contains(u.CustomerId))
                        .AsNoTracking()
                        .Select(s => new
                        {
                            Id = s.Id,
                            Total = s.Total,
                            CustomerId = s.CustomerId,
                            DueDate = s.DueDate,
                            OpenBalance = Math.Round(s.OpenBalance, accountCycle.RoundingDigit),
                            TotalPaid = s.TotalPaid,
                            MultiCurrencyOpenBalance = Math.Round(s.MultiCurrencyOpenBalance, accountCycle.RoundingDigit),
                            MultiCurrencyTotalPaid = s.MultiCurrencyTotalPaid,
                        });

            var customerTypeMemberIds = await GetCustomerTypeMembers().Select(s => s.CustomerTypeId).ToListAsync();

            var customerQuery = GetCustomers(null, input.Customers, null, customerTypeMemberIds);
            var invoiceQuery = from i in iQuery
                               join c in customerQuery
                               on i.CustomerId equals c.Id
                               select new
                               {
                                   Id = i.Id,
                                   Total = i.Total,
                                   CustomerId = i.CustomerId,
                                   DueDate = i.DueDate,
                                   OpenBalance = i.OpenBalance,
                                   TotalPaid = i.TotalPaid,
                                   MultiCurrencyOpenBalance = i.MultiCurrencyOpenBalance,
                                   MultiCurrencyTotalPaid = i.MultiCurrencyTotalPaid,
                                   CustomerName = c.CustomerName,
                                   
                               };


            var query = from i in invoiceQuery
                        join j in jiQuery
                        on i.Id equals j.InvoiceId
                        select new getInvoiceListOutput
                        {
                            Memo = j.Memo,
                            Id = i.Id,
                            Date = j.Date,
                            JournalNo = j.JournalNo,
                            AccountId = j.AccountId,
                            Total = i.Total,
                            Customer = new CustomerSummaryOutput
                            {
                                Id = i.CustomerId,
                                CustomerName = i.CustomerName,
                            },
                            CustomerId = i.CustomerId,
                            DueDate = i.DueDate,
                            OpenBalance = i.OpenBalance,
                            TotalPaid = i.TotalPaid,
                            MultiCurrencyOpenBalance = i.MultiCurrencyOpenBalance,
                            MultiCurrencyTotalPaid = i.MultiCurrencyTotalPaid,
                            MultiCurrencyCode = j.MultiCurrencyCode,
                            MultiCurrencyId = j.MultiCurrencyId.Value,
                            Reference = j.Reference,
                        };

            var resultCount = await query.CountAsync();
            if (resultCount == 0) return new PagedResultDto<getInvoiceListOutput>(resultCount, new List<getInvoiceListOutput>());

            var @entities = await query.OrderBy(t => t.DueDate).ToListAsync();
            return new PagedResultDto<getInvoiceListOutput>(resultCount, @entities);
        }


        #region import export excel

        private ReportOutput GetReportTemplateAccountItem(bool hasClassFeature)
        {
            var columns = new List<CollumnOutput>()
            {
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "InvoiceGroup",
                    ColumnLength = 180,
                    ColumnTitle = "Invoice Group",
                    ColumnType = ColumnType.String,
                    SortOrder = 0,
                    Visible = true,
                    AllowFunction = null,
                    MoreFunction = null,
                    IsDisplay = false
                },
                // start properties with can filter
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "Date",
                    ColumnLength = 180,
                    ColumnTitle = "Date",
                    ColumnType = ColumnType.String,
                    SortOrder = 1,
                    Visible = true,
                    AllowFunction = null,
                    MoreFunction = null,
                    IsDisplay = false
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "CustomerCode",
                    ColumnLength = 230,
                    ColumnTitle = "CustomerCode",
                    ColumnType = ColumnType.String,
                    SortOrder = 2,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = true
                },
                    new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "ARAccount",
                    ColumnLength = 130,
                    ColumnTitle = "AR Account",
                    ColumnType = ColumnType.String,
                    SortOrder = 3,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Location",
                    ColumnLength = 250,
                    ColumnTitle = "Location",
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
                    ColumnName = "Class",
                    ColumnLength = 200,
                    ColumnTitle = "Class",
                    ColumnType = ColumnType.String,
                    SortOrder = 5,
                    Visible = hasClassFeature,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                 new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Memo",
                    ColumnLength = 250,
                    ColumnTitle = "Memo",
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
                    ColumnName = "Reference",
                    ColumnLength = 250,
                    ColumnTitle = "Reference",
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
                    ColumnName = "InvoiceNo",
                    ColumnLength = 250,
                    ColumnTitle = "InvoiceNo",
                    ColumnType = ColumnType.String,
                    SortOrder = 8,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },

                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Currency",
                    ColumnLength = 130,
                    ColumnTitle = "Currency",
                    ColumnType = ColumnType.String,
                    SortOrder = 9,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Amount",
                    ColumnLength = 250,
                    ColumnTitle = "Amount In Tran. Currency",
                    ColumnType = ColumnType.Money,
                    SortOrder = 10,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = true
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "AmountInAccounting",
                    ColumnLength = 250,
                    ColumnTitle = "Amount In Acc. Currency",
                    ColumnType = ColumnType.String,
                    SortOrder = 11,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = true
                },
                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "ItemAccount",
                    ColumnLength = 130,
                    ColumnTitle = "Item Account",
                    ColumnType = ColumnType.String,
                    SortOrder = 12,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "ItemDescription",
                    ColumnLength = 130,
                    ColumnTitle = "Item Description",
                    ColumnType = ColumnType.String,
                    SortOrder = 13,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                 new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "SaleType",
                    ColumnLength = 130,
                    ColumnTitle = "Sale Type",
                    ColumnType = ColumnType.String,
                    SortOrder = 14,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
            };


            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = columns.WhereIf(!hasClassFeature, s => s.ColumnName != "Class").ToList(),
                Groupby = "",
                HeaderTitle = "Invoice",
                Sortby = "",
            };

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_ExportExcelTamplate)]
        public async Task<FileDto> ExportExcelTamplate()
        {
            var result = new FileDto();
            var sheetName = "Invoice";
            var hasClassFeature = IsEnabled(AppFeatures.SetupFeatureClasss);
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

                #region Row 1 Header Table
                int rowTableHeader = 1;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                var headerList = GetReportTemplateAccountItem(hasClassFeature);
                var reportCollumnHeader = headerList.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
                foreach (var i in reportCollumnHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);

                    colHeaderTable += 1;
                }
                #endregion Row 1

                result.FileName = $"InvoiceTamplate.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_ImportExcel)]
        [UnitOfWork(IsDisabled = true)]
        public async Task ImportExcel(FileDto input)
        {
            //var excelPackage = Read(input, _appFolders);
            var excelPackage = await Read(input);
            if (excelPackage == null) return;

            AccountCycle currentCycle = null;
            Tenant tenant = null;
            var tenantId = AbpSession.TenantId;
            var userId = AbpSession.GetUserId();
            var hasClassFeature = IsEnabled(AppFeatures.SetupFeatureClasss);
            var indexHasClassFeature = hasClassFeature ? 0 : -1;
            var @accounts = new List<ChartOfAccountSummaryWithTax>();
            var @locations = new List<NameValueDto<long>>();
            var classes = new List<NameValueDto<long>>();
            var currencies = new List<NameValueDto<long>>();
            var customers = new List<NameValueDto<Guid>>();
            var invoiceJournalHash = new HashSet<string>();
            AutoSequence invoiceAuto = null;
            var saleTypes = new List<NameValueDto<long>>();
            var useExchangeRate = false;

            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {

                    currentCycle = await GetCurrentCycleAsync();
                    tenant = await GetCurrentTenantAsync();

                    @accounts = await _chartOfAccountRepository.GetAll().AsNoTracking().Select(s => new ChartOfAccountSummaryWithTax { AccountCode = s.AccountCode, Id = s.Id, TaxId = s.TaxId }).ToListAsync();
                    @locations = await _locationRepository.GetAll().AsNoTracking().Select(s => new NameValueDto<long> { Name = s.LocationName, Value = s.Id }).ToListAsync();
                    classes = await _classRepository.GetAll().AsNoTracking().Select(s => new NameValueDto<long> { Name = s.ClassName, Value = s.Id }).ToListAsync();
                    currencies = await _currencyRepository.GetAll().Select(s => new NameValueDto<long> { Name = s.Code, Value = s.Id }).AsNoTracking().ToListAsync();
                    customers = await _customerRepository.GetAll().AsNoTracking().Select(s => new NameValueDto<Guid> { Name = s.CustomerCode, Value = s.Id }).ToListAsync();
                    invoiceJournalHash = (await _journalRepository.GetAll().Where(s => s.JournalType == JournalType.Invoice).Select(s => s.JournalNo).ToListAsync()).ToHashSet();

                    invoiceAuto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Invoice);
                    saleTypes = await _saleTypeRepository.GetAll().Select(s => new NameValueDto<long> { Name = s.TransactionTypeName, Value = s.Id }).ToListAsync();
                }
            }

            var addInvoiceItems = new List<InvoiceItem>();
            var addInvoices = new List<Invoice>();
            var addInvoiceJournalItems = new List<JournalItem>();
            var addInvoiceJournals = new List<Journal>();

            var invoiceDic = new Dictionary<string, Invoice>();

            // Get the work book in the file
            var workBook = excelPackage.Workbook;
            if (workBook != null)
            {
                // retrive first worksheets
                var worksheet = excelPackage.Workbook.Worksheets[0];

                for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                {
                    var invoiceGroup = worksheet.Cells[i, 1].Value?.ToString();
                    var date = worksheet.Cells[i, 2].Value?.ToString();
                    var customerCode = worksheet.Cells[i, 3].Value?.ToString();
                    var aRAccountCode = worksheet.Cells[i, 4].Value?.ToString();
                    var locationName = worksheet.Cells[i, 5].Value?.ToString();
                    var className = hasClassFeature ? worksheet.Cells[i, 6].Value?.ToString() : "";
                    var memo = worksheet.Cells[i, 7 + indexHasClassFeature].Value?.ToString();
                    var reference = worksheet.Cells[i, 8 + indexHasClassFeature].Value?.ToString();
                    var invoiceNo = worksheet.Cells[i, 9 + indexHasClassFeature].Value?.ToString();
                    var currencyCode = worksheet.Cells[i, 10 + indexHasClassFeature].Value?.ToString();
                    var amount = worksheet.Cells[i, 11 + indexHasClassFeature].Value.ToString();
                    var amountInAccount = worksheet.Cells[i, 12 + indexHasClassFeature].Value.ToString();
                    var itemAccountCode = worksheet.Cells[i, 13 + indexHasClassFeature].Value?.ToString();
                    var itemDescription = worksheet.Cells[i, 14 + indexHasClassFeature].Value?.ToString();
                    var saleTypeName = worksheet.Cells[i, 15 + indexHasClassFeature].Value?.ToString();

                    var currency = currencies.Where(s => s.Name == currencyCode).FirstOrDefault();
                    var defaultClass = hasClassFeature ? classes.Where(s => s.Name == className).Select(t => t.Value).FirstOrDefault() : tenant.ClassId;
                    var location = locations.Where(s => s.Name == locationName).FirstOrDefault();
                    var customer = customers.Where(s => s.Name == customerCode).FirstOrDefault();
                    var arAccount = accounts.Where(s => s.AccountCode == aRAccountCode).FirstOrDefault();
                    var itemAccount = accounts.Where(s => s.AccountCode == itemAccountCode).FirstOrDefault();
                    var saleType = saleTypes.Where(s => s.Name == saleTypeName).FirstOrDefault();


                    if (invoiceAuto.CustomFormat == false && invoiceNo == null)
                    {
                        throw new UserFriendlyException(L("IsRequired", L("InvoiceNo")) + $", Row = {i}");
                    }
                    if (string.IsNullOrWhiteSpace(date))
                    {
                        throw new UserFriendlyException(L("IsRequired", L("Date")) + $", Row = {i}");
                    }
                    if (invoiceAuto.RequireReference && reference.IsNullOrWhiteSpace())
                    {
                        throw new UserFriendlyException(L("IsRequired", L("Reference")) + $", Row = {i}");
                    }

                    if (currency == null)
                    {
                        throw new UserFriendlyException(L("NoCurrencyFound") + $", Row = {i}");
                    }
                    if (defaultClass == null)
                    {
                        throw new UserFriendlyException(L("NoClassFound") + $", Row = {i}");
                    }
                    if (customer == null)
                    {
                        throw new UserFriendlyException(L("NoVendorFound") + $", Row = {i}");
                    }

                    if (arAccount == null)
                    {
                        throw new UserFriendlyException(L("NoARAccountFound") + $", Row = {i}");
                    }
                    if (itemAccount == null)
                    {
                        throw new UserFriendlyException(L("NoItemAccountFound") + $", Row = {i}");
                    }

                    if (location == null)
                    {
                        throw new UserFriendlyException(L("NoLocationFound") + $", Row = {i}");
                    }


                    var qty = 1;

                    var unitPrice = Math.Round(Convert.ToDecimal(amount), currentCycle.RoundingDigitUnitCost);
                    var unitPriceInAccount = Math.Round(Convert.ToDecimal(amountInAccount), currentCycle.RoundingDigitUnitCost);
                    var totalInAcc = Math.Round(qty * unitPriceInAccount, currentCycle.RoundingDigit);
                    var totalInTran = Math.Round(qty * unitPrice, currentCycle.RoundingDigit);

                    var addInvoiceItem = InvoiceItem.Create(tenantId, userId, Guid.Empty, itemAccount.TaxId, null, itemDescription, qty, unitPriceInAccount, 0, totalInAcc, unitPrice, totalInTran);
                    var addInvoiceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, Guid.Empty, itemAccount.Id, itemDescription, 0, totalInAcc, PostingKey.Revenue, addInvoiceItem.Id);

                    addInvoiceItems.Add(addInvoiceItem);
                    addInvoiceJournalItems.Add(addInvoiceJournalItem);

                    if (invoiceDic.ContainsKey(invoiceGroup))
                    {
                        var addInvoice = invoiceDic[invoiceGroup];
                        addInvoice.SetSubTotal(addInvoice.SubTotal + totalInAcc);
                        addInvoice.SetTotal(addInvoice.Total + totalInAcc);
                        addInvoice.SetMultiCurrencySubTotal(addInvoice.MultiCurrencySubTotal + totalInTran);
                        addInvoice.SetMultiCurrencyTotal(addInvoice.MultiCurrencyTotal + totalInTran);
                        addInvoice.SetOpenBalance(addInvoice.Total);
                        addInvoice.SetMultiCurrencyOpenBalance(addInvoice.MultiCurrencyTotal);

                        var addInvoiceJournal = addInvoiceJournals.FirstOrDefault(s => s.InvoiceId == addInvoice.Id);
                        addInvoiceJournal.SetDebitCredit(addInvoice.Total);

                        var arAccountItem = addInvoiceJournalItems.FirstOrDefault(s => s.JournalId == addInvoiceJournal.Id && s.Key == PostingKey.AR);
                        arAccountItem.SetDebitCredit(addInvoice.Total, 0);

                        addInvoiceItem.SetInvoice(addInvoice.Id);
                        addInvoiceJournalItem.SetJournal(addInvoiceJournal.Id);
                    }
                    else
                    {
                        if (invoiceAuto.CustomFormat == true)
                        {
                            var newAuto = _autoSequenceManager.GetNewReferenceNumber(invoiceAuto.DefaultPrefix, invoiceAuto.YearFormat.Value,
                            invoiceAuto.SymbolFormat, invoiceAuto.NumberFormat, invoiceAuto.LastAutoSequenceNumber, DateTime.Now);

                            invoiceNo = newAuto;
                            invoiceAuto.UpdateLastAutoSequenceNumber(newAuto);
                        }
                        else if (invoiceJournalHash.Contains(invoiceNo) || addInvoiceJournals.Any(s => s.JournalNo == invoiceNo))
                        {
                            throw new UserFriendlyException(L("DuplicateInvoiceNo", invoiceNo) + $", Row = {i}");
                        }

                        CAddress billAddress = new CAddress("", "", "", "", "");
                        CAddress shipAddress = new CAddress("", "", "", "", "");

                        var addInvoice = Invoice.Create(tenantId, userId, ReceiveFrom.None, Convert.ToDateTime(date), customer.Value, true, shipAddress, billAddress, totalInAcc, 0, totalInAcc, null, Convert.ToDateTime(date), Convert.ToDateTime(date), false, totalInTran, 0, totalInTran, false, useExchangeRate);
                        var addInvoiceJournal = Journal.Create(tenantId, userId, invoiceNo, Convert.ToDateTime(date), memo, totalInAcc, totalInAcc, tenant.CurrencyId.Value, defaultClass.Value, reference, location.Value);
                        var arJournalItem = JournalItem.CreateJournalItem(tenantId, userId, addInvoiceJournal.Id, arAccount.Id, memo, totalInAcc, 0, PostingKey.AR, null);
                        addInvoice.UpdateTransactionTypeId(saleType?.Value);
                        addInvoiceJournal.UpdateMultiCurrency(currency.Value);
                        addInvoiceJournal.UpdateInvoice(addInvoice.Id);
                        addInvoiceJournal.UpdateStatus(TransactionStatus.Publish);

                        addInvoices.Add(addInvoice);
                        addInvoiceJournals.Add(addInvoiceJournal);
                        addInvoiceJournalItems.Add(arJournalItem);

                        addInvoiceItem.SetInvoice(addInvoice.Id);
                        addInvoiceJournalItem.SetJournal(addInvoiceJournal.Id);

                        invoiceDic.Add(invoiceGroup, addInvoice);
                    }

                }
            }


            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {

                    if (addInvoices.Any()) await _invoiceRepository.BulkInsertAsync(addInvoices);
                    if (addInvoiceItems.Any()) await _invoiceItemRepository.BulkInsertAsync(addInvoiceItems);
                    if (addInvoiceJournals.Any()) await _journalRepository.BulkInsertAsync(addInvoiceJournals);
                    if (addInvoiceJournalItems.Any()) await _journalItemRepository.BulkInsertAsync(addInvoiceJournalItems);

                    if (invoiceAuto.CustomFormat) CheckErrors(await _autoSequenceManager.UpdateAsync(invoiceAuto));
                }

                await uow.CompleteAsync();
            }

        }
        #endregion

        [AbpAuthorize(AppPermissions.Pages_Tenant_CleanRounding)]
        public async Task CleanRoundingPaidStatus(CarlEntityDto input)
        {
            var currenctPeriod = await GetCurrentCycleAsync();
            var roundDigit = currenctPeriod == null ? 2 : currenctPeriod.RoundingDigit;

            var invoice = await _invoiceManager.GetAsync(input.Id);
            if (invoice == null) throw new UserFriendlyException(L("RecordNotFound"));

            var journal = await _journalRepository.FirstOrDefaultAsync(s => s.InvoiceId == input.Id);
            if (journal == null) throw new UserFriendlyException(L("RecordNotFound"));

            var invoiceItems = await _invoiceItemRepository.GetAll().Where(s => s.InvoiceId == input.Id).ToListAsync();
            var journalItems = await _journalItemRepository.GetAll().Where(s => s.JournalId == journal.Id).ToListAsync();

            decimal subTotal = 0;
            foreach (var vItem in invoiceItems)
            {
                var lineTotal = Math.Round(vItem.Total, roundDigit);
                vItem.SetTotal(lineTotal);
                await _invoiceItemManager.UpdateAsync(vItem);

                var journalItem = journalItems.FirstOrDefault(s => s.Identifier == vItem.Id);
                if (journalItem == null) throw new UserFriendlyException(L("RecordNotFound"));

                journalItem.SetDebitCredit(0, lineTotal);
                await _journalItemManager.UpdateAsync(journalItem);

                subTotal += lineTotal;
            }

            decimal tax = Math.Round(invoice.Tax, roundDigit);
            decimal total = subTotal + tax;
            decimal openBalance = total - invoice.TotalPaid;

            invoice.SetSubTotal(subTotal);
            invoice.SetTax(tax);
            invoice.SetTotal(total);
            invoice.SetOpenBalance(openBalance);
            invoice.UpdatePaidStatus(invoice.TotalPaid == 0 ? PaidStatuse.Pending : openBalance == 0 ? PaidStatuse.Paid : PaidStatuse.Partial);
            await _invoiceManager.UpdateAsync(invoice);

            journal.SetDebitCredit(total);
            await _journalManager.UpdateAsync(journal, DocumentType.Invoice, false);

            var headerJournalItem = journalItems.FirstOrDefault(s => s.Identifier == null && s.Key == PostingKey.AR);
            if (headerJournalItem == null) throw new UserFriendlyException(L("RecordNotFound"));

            headerJournalItem.SetDebitCredit(total, 0);
            await _journalItemManager.UpdateAsync(headerJournalItem);

        }


        private ReportOutput GetInvoiceItemTemplateColumns(bool isMultiCurrency)
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                     new CollumnOutput{
                        ColumnName = "ItemCode",
                        ColumnLength = 180,
                        ColumnTitle = "Item Code",
                        ColumnType = ColumnType.String,
                        SortOrder = 1,
                    },
                    new CollumnOutput{
                        ColumnName = "ItemName",
                        ColumnLength = 230,
                        ColumnTitle = "Item Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                    },
                     new CollumnOutput {
                        ColumnName = "LotName",
                        ColumnLength = 130,
                        ColumnTitle = "Zone Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                    },
                    new CollumnOutput{
                        ColumnName = "Description",
                        ColumnLength = 250,
                        ColumnTitle = "Description",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                    },
                    new CollumnOutput{
                        ColumnName = "Qty",
                        ColumnLength = 200,
                        ColumnTitle = "Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 5,
                    },
                    new CollumnOutput {
                        ColumnName = "UnitPrice",
                        ColumnLength = 250,
                        ColumnTitle = "Unit Price",
                        ColumnType = ColumnType.Number,
                        SortOrder = 6,
                    },
                },
                Groupby = "",
                HeaderTitle = "InvoiceItem",
                Sortby = "",
            };

            if (isMultiCurrency)
            {
                result.ColumnInfo.Add(
                    new CollumnOutput
                    {
                        ColumnName = "MultiCurrencyUnitPrice",
                        ColumnLength = 250,
                        ColumnTitle = "Multi-Currency Unit Price",
                        ColumnType = ColumnType.Number,
                        SortOrder = 7,
                    }
                );
            }


            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_Create, AppPermissions.Pages_Tenant_Customer_Invoice_Update)]
        public async Task<FileDto> ExportExcelInvoiceItemsTemplate()
        {
            var result = new FileDto();
            var sheetName = "Invoice";

            var multiCurrencyCount = await _multiCurrencyRepository.CountAsync();

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

                #region Row 1 Header Table
                int rowTableHeader = 1;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                var headerList = GetInvoiceItemTemplateColumns(multiCurrencyCount > 0);

                foreach (var i in headerList.ColumnInfo)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);

                    colHeaderTable += 1;
                }
                #endregion Row 1

                result.FileName = $"InvoiceItemsTamplate.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_Create, AppPermissions.Pages_Tenant_Customer_Invoice_Update)]
        public async Task<List<CreateOrUpdateInvoiceItemInput>> ImportExcelInvoiceItems(long LocationId, FileDto input)
        {
            var result = new List<CreateOrUpdateInvoiceItemInput>();

            //var excelPackage = Read(input, _appFolders);
            var excelPackage = await Read(input);

            var lots = await _lotRepository.GetAll().Where(s => s.LocationId == LocationId).AsNoTracking().ToListAsync();
            var @items = await _itemRepository.GetAll()
                               .Include(s => s.ItemType)
                               .Include(s => s.PurchaseAccount)
                               .Include(s => s.PurchaseTax)
                               .Include(s => s.SaleAccount)
                               .Include(s => s.SaleTax)
                               .Include(s => s.InventoryAccount)
                               .AsNoTracking()
                               .ToListAsync();
            var itemLots = await _itemLotRepository.GetAll().Include(s => s.Lot).Where(s => s.Lot.LocationId == LocationId).AsNoTracking().ToListAsync();
            var subItems = await _bomitemRepository.GetAll().Include(u=>u.BOM).AsNoTracking().Where(s=>s.BOM.IsDefault).ToListAsync();
            var multiCurrencyCount = await _multiCurrencyRepository.CountAsync();


            if (excelPackage != null)
            {
                // Get the work book in the file
                var workBook = excelPackage.Workbook;
                if (workBook != null)
                {
                    // retrive first worksheets
                    var worksheet = excelPackage.Workbook.Worksheets[0];

                    for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                    {
                        var itemCode = worksheet.Cells[i, 1].Value?.ToString();
                        var lotName = worksheet.Cells[i, 3].Value?.ToString();
                        var description = worksheet.Cells[i, 4].Value?.ToString();
                        var qty = worksheet.Cells[i, 5].Value?.ToString();
                        var unitPrice = worksheet.Cells[i, 6].Value?.ToString();

                        var item = items.FirstOrDefault(s => s.ItemCode == itemCode);
                        if (item == null) throw new UserFriendlyException(L("IsNotValid", L("ItemCode") + $", Row = {i}"));
                        if (item.SaleAccount == null) throw new UserFriendlyException(L("ItemMustHasSaleAccount", $"{item.ItemCode}-{item.ItemName}, Row = {i}"));

                        var subs = subItems.Where(s => s.BOM.ItemId == item.Id).ToList();

                        var invoiceItem = new CreateOrUpdateInvoiceItemInput
                        {
                            ItemId = item.Id,
                            Item = ObjectMapper.Map<ItemSummaryDetailOutput>(item),
                            Account = ObjectMapper.Map<ChartAccountSummaryOutput>(item.SaleAccount),
                            AccountId = item.SaleAccountId.Value,
                            Tax = ObjectMapper.Map<TaxSummaryOutput>(item.SaleTax),
                            TaxId = item.SaleTaxId.Value,
                            Qty = Convert.ToDecimal(qty),
                            UnitCost = Convert.ToDecimal(unitPrice),
                            MultiCurrencyUnitCost = 0,
                            DiscountRate = 0,
                            Description = description,
                            DisplayInventoryAccount = item.ItemType.DisplayInventoryAccount,
                            Key = subs.Any() ? Guid.NewGuid() : (Guid?)null,
                        };

                        if (invoiceItem.DisplayInventoryAccount && !string.IsNullOrWhiteSpace(lotName))
                        {
                            var lot = lots.FirstOrDefault(s => s.LotName == lotName);
                            if (lot == null) throw new UserFriendlyException(L("IsNotValid", L("LotName") + $" {lotName}, Row = {i}"));

                            invoiceItem.LotId = lot.Id;
                            invoiceItem.LotDetail = new LotSummaryOutput { Id = lot.Id, LotName = lotName };
                        }

                        if (multiCurrencyCount > 0)
                        {
                            var multiCurrencyUnitPrice = worksheet.Cells[i, 7].Value?.ToString();
                            invoiceItem.MultiCurrencyUnitCost = Convert.ToDecimal(multiCurrencyUnitPrice);
                        }

                        result.Add(invoiceItem);

                        if (invoiceItem.Key.HasValue)
                        {
                            foreach (var sub in subs)
                            {
                                var subitem = items.FirstOrDefault(s => s.Id == sub.ItemId);
                                if (subitem == null) throw new UserFriendlyException("RecordNotFound");

                                var subInvoiceItem = new CreateOrUpdateInvoiceItemInput
                                {
                                    ItemId = subitem.Id,
                                    Item = ObjectMapper.Map<ItemSummaryDetailOutput>(subitem),
                                    Account = ObjectMapper.Map<ChartAccountSummaryOutput>(subitem.SaleAccount),
                                    AccountId = subitem.SaleAccountId.Value,
                                    Tax = ObjectMapper.Map<TaxSummaryOutput>(subitem.SaleTax),
                                    TaxId = subitem.SaleTaxId.Value,
                                    Qty = invoiceItem.Qty * sub.Qty,
                                    UnitCost = 0,
                                    MultiCurrencyUnitCost = 0,
                                    DiscountRate = 0,
                                    DisplayInventoryAccount = subitem.ItemType.DisplayInventoryAccount,
                                    ParentId = invoiceItem.Key,
                                    Display = item.ShowSubItems,
                                };

                                if (subInvoiceItem.DisplayInventoryAccount)
                                {
                                    var itemLot = itemLots.FirstOrDefault(s => s.ItemId == subitem.Id);
                                    if (itemLot == null && !subInvoiceItem.Display) throw new UserFriendlyException(L("PleaseSetDefaultLotFor", $"{subitem.ItemCode}-{subitem.ItemName}"));

                                    if (itemLot != null)
                                    {
                                        subInvoiceItem.LotId = itemLot.LotId;
                                        subInvoiceItem.LotDetail = new LotSummaryOutput { Id = itemLot.LotId, LotName = itemLot.Lot.LotName };
                                    }
                                }

                                result.Add(subInvoiceItem);

                            }
                        }

                    }
                }
            }

            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_MultiDelete)]
        [UnitOfWork(IsDisabled = true)]
        public async Task MultiDelete(GetListDeleteInput input)
        {
            var tenantId = AbpSession.TenantId;

            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var journals = await _journalRepository.GetAll()
                                         .Include(s => s.Invoice.ItemIssue)
                                         .Include(s => s.CustomerCredit)
                                         .AsNoTracking()
                                         .Where(u => u.JournalType == JournalType.Invoice || u.JournalType == JournalType.CustomerCredit)
                                         .Where(u => input.Ids.Contains(u.InvoiceId.Value) || input.Ids.Contains(u.CustomerCreditId.Value))
                                         .OrderByDescending(s => s.JournalNo)
                                         .ToListAsync();

                    if (journals.IsNullOrEmpty()) throw new UserFriendlyException(L("RecordNotFound"));

                    var lockDate = journals.OrderByDescending(t => t.Date).Select(t => t.Date).FirstOrDefault();
                    var locktransaction = await _lockRepository.GetAll()
                                                .AsNoTracking()
                                                .Where(t => t.LockKey == TransactionLockType.Invoice && t.IsLock == true && t.LockDate.Value.Date >= lockDate.Date)
                                                .AnyAsync();
                    if (locktransaction) throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));

                    var @closePeroid = await _accountCycleRepository.GetAll().AsNoTracking().AnyAsync(t => t.EndDate != null && lockDate.Date > t.EndDate.Value.Date);
                    if (closePeroid) throw new UserFriendlyException(L("PeriodIsClose"));

                    // validate from Paybill
                    var hasPayment = await _receivePaymentDetailRepository.GetAll()
                                           .AsNoTracking()
                                           .AnyAsync(t => input.Ids.Contains(t.PayToId.Value));
                    if (hasPayment) throw new UserFriendlyException(L("RecordHasReceivePayment"));

                
                    var validateInvoice = journals.Where(t => t.Invoice != null && t.Invoice.ItemIssue != null && t.Invoice.CreationTime < t.Invoice.ItemIssue.CreationTime && !t.Invoice.ConvertToItemIssue).Any();

                    if (validateInvoice)
                    {
                        throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
                    }


                    var _customerCreditIds = journals.Where(t => t.CustomerCredit != null && !t.CustomerCredit.ConvertToItemReceipt).Select(t => t.CustomerCreditId).ToList();

                    if (_customerCreditIds != null && _customerCreditIds.Count() > 0)
                    {

                        var validateVendorCredit = await _itemReceiptCustomerCreditRepository.GetAll()
                                                   .AsNoTracking()
                                                   .Where(t => t.CustomerCreditId != null  && t.ReceiveFrom == ReceiveFrom.CustomerCredit)
                                                   .Where(t=> _customerCreditIds.Contains(t.CustomerCreditId))
                                                   .AnyAsync();
                        if (validateVendorCredit)
                        {
                            throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
                        }
                    }


                    var journalItems = await _journalItemRepository.GetAll()
                                            .AsNoTracking()
                                            .Where(s => journals.Any(j => j.Id == s.JournalId))
                                            .ToListAsync();

                    if (journalItems.Any()) await _journalItemRepository.BulkDeleteAsync(journalItems);

                    var deleteItemReceiptCustomerCreditIds = new List<Guid>();

                    var custmerCreditJournals = journals.Where(s => s.CustomerCredit != null).ToList();
                    if (custmerCreditJournals.Any())
                    {
                        var autoCustmerCredit = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.CustomerCredit);

                        var lastCustomerCreditJournal = custmerCreditJournals.FirstOrDefault();

                        if (lastCustomerCreditJournal.JournalNo == autoCustmerCredit.LastAutoSequenceNumber)
                        {
                            var jo = await _journalRepository.GetAll()
                                            .AsNoTracking()
                                            .Where(t => t.JournalType == JournalType.CustomerCredit)
                                            .Where(t => !custmerCreditJournals.Any(r => r.Id == t.Id))
                                            .OrderByDescending(t => t.CreationTime)
                                            .FirstOrDefaultAsync();
                            if (jo != null)
                            {
                                autoCustmerCredit.UpdateLastAutoSequenceNumber(jo.JournalNo);
                            }
                            else
                            {
                                autoCustmerCredit.UpdateLastAutoSequenceNumber(null);
                            }
                            CheckErrors(await _autoSequenceManager.UpdateAsync(autoCustmerCredit));
                        }

                        var customerCredits = custmerCreditJournals.Select(s => s.CustomerCredit).ToList();


                        var bIds = customerCredits.Select(s => s.Id).ToList();
                        var exchanges = await _customerCreditExchangeRateRepository.GetAll().AsNoTracking().Where(s => bIds.Contains(s.CustomerCreditId)).ToListAsync();
                        if (exchanges.Any()) await _customerCreditExchangeRateRepository.BulkDeleteAsync(exchanges);


                        var autoConvertReceipts = customerCredits.Where(s => s.ConvertToItemReceipt).Select(s => s.Id).ToList();

                        if (autoConvertReceipts.Any())
                        {
                            var itemReceiptJournals = await _journalRepository.GetAll()
                                                            .Include(s => s.ItemReceiptCustomerCredit)
                                                            .AsNoTracking()
                                                            .Where(s => s.JournalType == JournalType.ItemReceiptCustomerCredit)
                                                            .Where(s => autoConvertReceipts.Contains(s.ItemReceiptCustomerCredit.CustomerCreditId.Value))
                                                            .OrderByDescending(s => s.JournalNo)
                                                            .ToListAsync();

                            var itemReceiptJournalItems = await _journalItemRepository.GetAll().AsNoTracking()
                                                                .Where(s => itemReceiptJournals.Any(r => r.Id == s.JournalId))
                                                                .ToListAsync();

                            if (itemReceiptJournalItems.Any()) await _journalItemRepository.BulkDeleteAsync(itemReceiptJournalItems);


                            var autoItemReceipt = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt_CustomerCredit);

                            var lastItemReceiptJournal = itemReceiptJournals.FirstOrDefault();

                            if (lastItemReceiptJournal.JournalNo == autoItemReceipt.LastAutoSequenceNumber)
                            {
                                var jo = await _journalRepository.GetAll()
                                                .AsNoTracking()
                                                .Where(t => t.JournalType == JournalType.ItemReceiptCustomerCredit)
                                                .Where(t => !itemReceiptJournals.Any(r => r.Id == t.Id))
                                                .OrderByDescending(t => t.CreationTime)
                                                .FirstOrDefaultAsync();
                                if (jo != null)
                                {
                                    autoItemReceipt.UpdateLastAutoSequenceNumber(jo.JournalNo);
                                }
                                else
                                {
                                    autoItemReceipt.UpdateLastAutoSequenceNumber(null);
                                }
                                CheckErrors(await _autoSequenceManager.UpdateAsync(autoItemReceipt));
                            }

                            var itemReceipts = itemReceiptJournals.Select(s => s.ItemReceiptCustomerCredit).ToList();

                            if (itemReceiptJournals.Any()) await _journalRepository.BulkDeleteAsync(itemReceiptJournals);


                            var itemReceiptItems = await _itemReceiptItemCustomerCreditRepository.GetAll()
                                                         .AsNoTracking()
                                                         .Where(s => itemReceipts.Any(r => r.Id == s.ItemReceiptCustomerCreditId))
                                                         .ToListAsync();

                            var itemReceiptBatchNos = await _itemReceiptCustomerCreditItemBatchNoRepository.GetAll()
                                                            .AsNoTracking()
                                                            .Where(s => itemReceiptItems.Any(r => r.Id == s.ItemReceiptCustomerCreditItemId))
                                                            .ToListAsync();

                            var inventoryTransactionItems = await _inventoryTransactionItemRepository.GetAll()
                                                                  .AsNoTracking()
                                                                  .Where(s => itemReceiptItems.Any(r => r.Id == s.Id))
                                                                  .ToListAsync();


                            deleteItemReceiptCustomerCreditIds = itemReceipts.Select(s => s.Id).ToList();

                            if (itemReceiptBatchNos.Any()) await _itemReceiptCustomerCreditItemBatchNoRepository.BulkDeleteAsync(itemReceiptBatchNos);
                            if (inventoryTransactionItems.Any()) await _inventoryTransactionItemRepository.BulkDeleteAsync(inventoryTransactionItems);
                            if (itemReceiptItems.Any()) await _itemReceiptItemCustomerCreditRepository.BulkDeleteAsync(itemReceiptItems);
                            if (itemReceipts.Any()) await _itemReceiptCustomerCreditRepository.BulkDeleteAsync(itemReceipts);
                        }

                        var notAutoConvertReceipts = customerCredits.Where(s => !s.ConvertToItemReceipt).Select(s => s.Id).ToList();
                        if (notAutoConvertReceipts.Any())
                        {
                            var itemReceiptCustomerCredits = await _itemReceiptCustomerCreditRepository.GetAll()
                                                               .AsNoTracking()
                                                               .Where(s => s.CustomerCreditId.HasValue && notAutoConvertReceipts.Contains(s.CustomerCreditId.Value))
                                                               .ToListAsync();

                            if (itemReceiptCustomerCredits.Any())
                            {
                                var itemReceiptCustomerCreditItems = await _itemReceiptItemCustomerCreditRepository.GetAll()
                                                                       .AsNoTracking()
                                                                       .Where(s => itemReceiptCustomerCredits.Any(r => r.Id == s.ItemReceiptCustomerCreditId))
                                                                       .ToListAsync();

                                foreach (var i in itemReceiptCustomerCreditItems)
                                {
                                    i.UpdateCustomerCreditItemId(null);
                                }

                                foreach (var i in itemReceiptCustomerCredits)
                                {
                                    i.UpdateCustomerCredit(null);
                                }


                                await _itemReceiptItemCustomerCreditRepository.BulkUpdateAsync(itemReceiptCustomerCreditItems);
                                await _itemReceiptCustomerCreditRepository.BulkUpdateAsync(itemReceiptCustomerCredits);
                            }
                        }


                        var customerCreditItems = await _customerCreditItemRepository.GetAll()
                                                    .AsNoTracking()
                                                    .Where(s => customerCredits.Any(r => r.Id == s.CustomerCreditId))
                                                    .ToListAsync();

                        var cusotmerCreditItemBatchNos = await _customerCreditItemBatchNoRepository.GetAll()
                                                             .AsNoTracking()
                                                             .Where(s => customerCreditItems.Any(r => r.Id == s.CustomerCreditItemId))
                                                             .ToListAsync();
                        if (cusotmerCreditItemBatchNos.Any()) await _customerCreditItemBatchNoRepository.BulkDeleteAsync(cusotmerCreditItemBatchNos);
                        if (customerCreditItems.Any()) await _customerCreditItemRepository.BulkDeleteAsync(customerCreditItems);

                        await _journalRepository.BulkDeleteAsync(custmerCreditJournals);
                        await _cusotmerCreditRepository.BulkDeleteAsync(customerCredits);

                    }


                    var invoiceJournals = journals.Where(s => s.Invoice != null).ToList();
                    var saleOrderIds = new List<Guid>();
                    if (invoiceJournals.Any())
                    {
                        var autoInvoice = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Invoice);

                        var lastInvoiceJournal = invoiceJournals.FirstOrDefault();

                        if (lastInvoiceJournal != null && lastInvoiceJournal.JournalNo == autoInvoice.LastAutoSequenceNumber)
                        {
                            var jo = await _journalRepository.GetAll()
                                            .AsNoTracking()
                                            .Where(t => t.JournalType == JournalType.Invoice)
                                            .Where(t => !invoiceJournals.Any(r => r.Id == t.Id))
                                            .OrderByDescending(t => t.CreationTime)
                                            .FirstOrDefaultAsync();
                            if (jo != null)
                            {
                                autoInvoice.UpdateLastAutoSequenceNumber(jo.JournalNo);
                            }
                            else
                            {
                                autoInvoice.UpdateLastAutoSequenceNumber(null);
                            }
                            CheckErrors(await _autoSequenceManager.UpdateAsync(autoInvoice));
                        }

                        var invoiceItems = await _invoiceItemRepository.GetAll()
                                       .Include(s => s.SaleOrderItem)
                                       .Include(s => s.ItemIssueItem.SaleOrderItem)
                                       .AsNoTracking()
                                       .Where(s => invoiceJournals.Any(r => r.InvoiceId.Value == s.InvoiceId))
                                       .ToListAsync();

                        saleOrderIds = invoiceItems.Where(s => s.SaleOrderItem != null).GroupBy(s => s.SaleOrderItem.SaleOrderId).Select(s => s.Key).ToList();

                        var invoices = invoiceJournals.Select(s => s.Invoice).ToList();


                        var bIds = invoices.Select(s => s.Id).ToList();
                        var exchanges = await _exchangeRateRepository.GetAll().AsNoTracking().Where(s => bIds.Contains(s.InvoiceId)).ToListAsync();
                        if (exchanges.Any()) await _exchangeRateRepository.BulkDeleteAsync(exchanges);


                        var autoConvertIssues = invoices.Where(s => s.ConvertToItemIssue && s.ItemIssueId.HasValue).Select(s => s.ItemIssueId.Value).ToList();
                        var notAutoConvertIssues = invoices.Where(s => !s.ConvertToItemIssue && s.ItemIssueId.HasValue).Select(s => s.ItemIssueId.Value).ToList();

                        if (notAutoConvertIssues.Any())
                        {
                            var itemIssueItemLinkOrders = invoiceItems.Where(s => s.ItemIssueItem != null)
                                                                      .Where(s => notAutoConvertIssues.Contains(s.ItemIssueItem.ItemIssueId))
                                                                      .Where(s => s.ItemIssueItem.SaleOrderItem != null)
                                                                      .Select(s => s.ItemIssueItem);

                            if (itemIssueItemLinkOrders.Any())
                            {
                                var orderLinkIds = itemIssueItemLinkOrders.Where(s => !saleOrderIds.Contains(s.SaleOrderItem.SaleOrderId))
                                                                          .GroupBy(s => s.SaleOrderItem.SaleOrderId)
                                                                          .Select(s => s.Key).ToList();
                                if(orderLinkIds.Any()) saleOrderIds.AddRange(orderLinkIds);

                                foreach(var link in itemIssueItemLinkOrders)
                                {
                                    link.UpdateSaleOrderItemId(null);
                                }

                                await _itemIssueItemRepository.BulkUpdateAsync(itemIssueItemLinkOrders);
                            }
                        }

                        if (invoiceItems.Any()) await _invoiceItemRepository.BulkDeleteAsync(invoiceItems);
                        await _journalRepository.BulkDeleteAsync(invoiceJournals);
                        await _invoiceRepository.BulkDeleteAsync(invoices);

                        if (autoConvertIssues.Any())
                        {
                            var itemIssueJournals = await _journalRepository.GetAll()
                                                         .Include(s => s.ItemIssue)
                                                         .AsNoTracking()
                                                         .Where(s => autoConvertIssues.Contains(s.ItemIssueId.Value))
                                                         .OrderByDescending(s => s.JournalNo)
                                                         .ToListAsync();

                            var itemIssues = itemIssueJournals.Select(s => s.ItemIssue).ToList();
                            var itemIssueItems = await _itemIssueItemRepository.GetAll()
                                                        .AsNoTracking()
                                                        .Where(s => itemIssues.Any(r => r.Id == s.ItemIssueId))
                                                        .ToListAsync();

                            var itemIssueHasCustomerCredits = await _customerCreditItemRepository.GetAll()
                                                                    .AsNoTracking()
                                                                    .WhereIf(!custmerCreditJournals.IsNullOrEmpty(), s => !custmerCreditJournals.Any(r => r.CustomerCreditId == s.CustomerCreditId))
                                                                    .Where(s => s.ItemIssueSaleItemId.HasValue && itemIssueItems.Any(r => r.Id == s.ItemIssueSaleItemId))
                                                                    .AnyAsync();
                            if (itemIssueHasCustomerCredits) throw new UserFriendlyException(L("InvoiceAlreadyHasReturn"));

                            var itemIssueHasReutrn = await _itemReceiptItemCustomerCreditRepository.GetAll()
                                                            .AsNoTracking()
                                                            .WhereIf(!deleteItemReceiptCustomerCreditIds.IsNullOrEmpty(), s => !deleteItemReceiptCustomerCreditIds.Contains(s.ItemReceiptCustomerCreditId))
                                                            .Where(s => s.ItemIssueSaleItemId.HasValue && itemIssueItems.Any(r => r.Id == s.ItemIssueSaleItemId))
                                                            .AnyAsync();
                            if (itemIssueHasReutrn) throw new UserFriendlyException(L("InvoiceAlreadyHasReturn"));


                            var autoItemIssue = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);

                            var lastItemIssueJournal = itemIssueJournals.FirstOrDefault();

                            if (lastItemIssueJournal != null && lastItemIssueJournal.JournalNo == autoItemIssue.LastAutoSequenceNumber)
                            {
                                var jo = await _journalRepository.GetAll()
                                                .AsNoTracking()
                                                .Where(t => t.JournalType == JournalType.ItemIssueSale ||
                                                            t.JournalType == JournalType.ItemIssueKitchenOrder ||
                                                            t.JournalType == JournalType.ItemIssueAdjustment ||
                                                            t.JournalType == JournalType.ItemIssueOther ||
                                                            t.JournalType == JournalType.ItemIssueTransfer ||
                                                            t.JournalType == JournalType.ItemIssueProduction)
                                                .Where(t => !itemIssueJournals.Any(r => r.Id == t.Id))
                                                .OrderByDescending(t => t.CreationTime)
                                                .FirstOrDefaultAsync();
                                if (jo != null)
                                {
                                    autoItemIssue.UpdateLastAutoSequenceNumber(jo.JournalNo);
                                }
                                else
                                {
                                    autoItemIssue.UpdateLastAutoSequenceNumber(null);
                                }
                                CheckErrors(await _autoSequenceManager.UpdateAsync(autoItemIssue));
                            }

                            var itemIssueJournalItems = await _journalItemRepository.GetAll().AsNoTracking()
                                                               .Where(s => itemIssueJournals.Any(r => r.Id == s.JournalId))
                                                               .ToListAsync();

                            if (itemIssueJournalItems.Any()) await _journalItemRepository.BulkDeleteAsync(itemIssueJournalItems);
                            if (itemIssueJournals.Any()) await _journalRepository.BulkDeleteAsync(itemIssueJournals);


                            var itemIssueBatchNos = await _itemIssueItemBatchNoRepository.GetAll()
                                                            .AsNoTracking()
                                                            .Where(s => itemIssueItems.Any(r => r.Id == s.ItemIssueItemId))
                                                            .ToListAsync();

                            var inventoryTransactionItems = await _inventoryTransactionItemRepository.GetAll()
                                                                  .AsNoTracking()
                                                                  .Where(s => itemIssueItems.Any(r => r.Id == s.Id))
                                                                  .ToListAsync();

                            if (itemIssueBatchNos.Any()) await _itemIssueItemBatchNoRepository.BulkDeleteAsync(itemIssueBatchNos);
                            if (inventoryTransactionItems.Any()) await _inventoryTransactionItemRepository.BulkDeleteAsync(inventoryTransactionItems);
                            if (itemIssueItems.Any()) await _itemIssueItemRepository.BulkDeleteAsync(itemIssueItems);
                            if (itemIssues.Any()) await _itemIssueRepository.BulkDeleteAsync(itemIssues);
                        }

                    }


                    if (saleOrderIds.Any()) await UpdateSaleOrderInventoryStatus(saleOrderIds);

                }

                await uow.CompleteAsync();
            }

        }


        #region Import Excel Items

        private ReportOutput GetReportTemplateInvoiceItems(bool hasClassFeature)
        {
            var columns = new List<CollumnOutput>()
            {
                // start properties with can filter
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "InvoiceGroup",
                    ColumnLength = 180,
                    ColumnTitle = "Invoice Group",
                    ColumnType = ColumnType.String,
                    SortOrder = 0,
                    Visible = true,
                    AllowFunction = null,
                    MoreFunction = null,
                    IsDisplay = false
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "Date",
                    ColumnLength = 180,
                    ColumnTitle = "Invoice Date",
                    ColumnType = ColumnType.String,
                    SortOrder = 1,
                    Visible = true,
                    AllowFunction = null,
                    MoreFunction = null,
                    IsDisplay = false
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "ItemIssueDate",
                    ColumnLength = 180,
                    ColumnTitle = "Issue Date",
                    ColumnType = ColumnType.String,
                    SortOrder = 2,
                    Visible = true,
                    AllowFunction = null,
                    MoreFunction = null,
                    IsDisplay = false
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "CustomerCode",
                    ColumnLength = 230,
                    ColumnTitle = "Customer Code",
                    ColumnType = ColumnType.String,
                    SortOrder = 3,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = true
                },
                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "ARAccount",
                    ColumnLength = 130,
                    ColumnTitle = "AR Account",
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
                    ColumnName = "Location",
                    ColumnLength = 250,
                    ColumnTitle = "Location",
                    ColumnType = ColumnType.String,
                    SortOrder = 5,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Class",
                    ColumnLength = 200,
                    ColumnTitle = "Class",
                    ColumnType = ColumnType.String,
                    SortOrder = 6,
                    Visible = hasClassFeature,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Currency",
                    ColumnLength = 130,
                    ColumnTitle = "Currency",
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
                    ColumnName = "Memo",
                    ColumnLength = 250,
                    ColumnTitle = "Memo",
                    ColumnType = ColumnType.String,
                    SortOrder = 8,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },

                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Reference",
                    ColumnLength = 250,
                    ColumnTitle = "Reference",
                    ColumnType = ColumnType.String,
                    SortOrder = 9,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },

                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "InvoiceNo",
                    ColumnLength = 250,
                    ColumnTitle = "Invoice No",
                    ColumnType = ColumnType.String,
                    SortOrder = 10,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "ItemCode",
                    ColumnLength = 250,
                    ColumnTitle = "Item Code",
                    ColumnType = ColumnType.String,
                    SortOrder = 11,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Zone",
                    ColumnLength = 250,
                    ColumnTitle = "Zone",
                    ColumnType = ColumnType.String,
                    SortOrder = 12,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Qty",
                    ColumnLength = 250,
                    ColumnTitle = "Qty",
                    ColumnType = ColumnType.Number,
                    SortOrder = 13,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = true
                },

                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "UnitPriceInTranCurrency",
                    ColumnLength = 250,
                    ColumnTitle = "Unit Price in Tran. Currency",
                    ColumnType = ColumnType.Money,
                    SortOrder = 14,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = true
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "UnitPriceInAccCurrency",
                    ColumnLength = 250,
                    ColumnTitle = "Unit Price In Acc. Currency",
                    ColumnType = ColumnType.String,
                    SortOrder =15,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = true
                },
                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "ItemDescription",
                    ColumnLength = 130,
                    ColumnTitle = "Item Description",
                    ColumnType = ColumnType.String,
                    SortOrder = 16,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "BatchSerial",
                    ColumnLength = 130,
                    ColumnTitle = "Batch/Serial",
                    ColumnType = ColumnType.String,
                    SortOrder = 17,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },

                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Expiration",
                    ColumnLength = 130,
                    ColumnTitle = "Expiration",
                    ColumnType = ColumnType.Date,
                    SortOrder = 18,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                 new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "SaleType",
                    ColumnLength = 130,
                    ColumnTitle = "Sale Type",
                    ColumnType = ColumnType.String,
                    SortOrder = 19,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                }
            };

            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = columns.WhereIf(!hasClassFeature, s => s.ColumnName != "Class").ToList(),
                Groupby = "",
                HeaderTitle = "Invoice",
                Sortby = "",
            };

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_ExportExcelTamplate)]
        public async Task<FileDto> ExportExcelItemsTamplate()
        {
            var result = new FileDto();
            var sheetName = "Bill";
            var hasClassFeature = IsEnabled(AppFeatures.SetupFeatureClasss);
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

                #region Row 1 Header Table
                int rowTableHeader = 1;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                var headerList = GetReportTemplateInvoiceItems(hasClassFeature);
                var reportCollumnHeader = headerList.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
                foreach (var i in reportCollumnHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);

                    colHeaderTable += 1;
                }
                #endregion Row 1

                result.FileName = $"InvoiceTamplate.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_ImportExcel)]
        [UnitOfWork(IsDisabled = true)]
        public async Task ImportExcelItems(FileDto input)
        {
            //var excelPackage = Read(input, _appFolders);
            var excelPackage = await Read(input);
            if (excelPackage == null) return;

            AccountCycle currentCycle = null;
            Tenant tenant = null;
            var tenantId = AbpSession.TenantId;
            var userId = AbpSession.GetUserId();
            var hasClassFeature = IsEnabled(AppFeatures.SetupFeatureClasss);
            var indexHasClassFeature = hasClassFeature ? 0 : -1;
            var @accounts = new List<ChartOfAccountSummaryWithTax>();
            var @locations = new List<NameValueDto<long>>();
            var @lots = new List<ItemLotDto>();
            var classes = new List<NameValueDto<long>>();
            var currencies = new List<NameValueDto<long>>();
            var customers = new List<NameValueDto<Guid>>();
            var invoiceJournalHash = new List<JournalRefWithPartnerDto>();
            var itemIssueJournalHash = new HashSet<string>();
            var items = new List<ItemSummaryWithAccount>();
            var batchNos = new List<BatchNoWithExpiration>();
            var saleTypes = new List<NameValueDto<long>>();
            AutoSequence invoiceAuto = null;
            AutoSequence issueAuto = null;

            var turnOffStockValidation = false;
            Guid? transactionTypeId = null;
            var useExchangeRate = false;

            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {

                    currentCycle = await GetCurrentCycleAsync();
                    tenant = await GetCurrentTenantAsync();

                    @accounts = await _chartOfAccountRepository.GetAll().AsNoTracking().Select(s => new ChartOfAccountSummaryWithTax { AccountCode = s.AccountCode, Id = s.Id, TaxId = s.TaxId }).ToListAsync();
                    @locations = await _locationRepository.GetAll().AsNoTracking().Select(s => new NameValueDto<long> { Name = s.LocationName, Value = s.Id }).ToListAsync();
                    @lots = await _lotRepository.GetAll().AsNoTracking().Select(s => new ItemLotDto { Id = s.Id, LotName = s.LotName, LocationId = s.LocationId }).ToListAsync();
                    classes = await _classRepository.GetAll().AsNoTracking().Select(s => new NameValueDto<long> { Name = s.ClassName, Value = s.Id }).ToListAsync();
                    currencies = await _currencyRepository.GetAll().Select(s => new NameValueDto<long> { Name = s.Code, Value = s.Id }).AsNoTracking().ToListAsync();
                    customers = await _customerRepository.GetAll().AsNoTracking().Select(s => new NameValueDto<Guid> { Name = s.CustomerCode, Value = s.Id }).ToListAsync();

                    invoiceJournalHash = await _journalRepository.GetAll().AsNoTracking()
                                               .Where(t => t.JournalType == JournalType.Invoice)
                                               .Select(s => new JournalRefWithPartnerDto
                                               {
                                                   JournalNo = s.JournalNo,
                                                   Reference = s.Reference,
                                                   PartnerId = s.Invoice.CustomerId
                                               })
                                               .ToListAsync();
                    saleTypes = await _saleTypeRepository.GetAll().AsNoTracking().Select(s => new NameValueDto<long> { Name = s.TransactionTypeName, Value = s.Id }).ToListAsync();
                    itemIssueJournalHash = (await _journalRepository.GetAll().Where(s => s.ItemIssueId.HasValue).Select(s => s.JournalNo).ToListAsync()).ToHashSet();
                    batchNos = await _batchNoRepository.GetAll().AsNoTracking().Select(s => new BatchNoWithExpiration { Id = s.Id, BatchNumber = s.Code, ExpirationDate = s.ExpirationDate, ItemId = s.ItemId }).ToListAsync();
                    items = await _itemRepository.GetAll().AsNoTracking().Select(s => new ItemSummaryWithAccount
                    {
                        Id = s.Id,
                        ItemCode = s.ItemCode,
                        ItemName = s.ItemName,
                        PurchaseAccountId = s.PurchaseAccountId,
                        PurchaseTaxId = s.PurchaseTaxId,
                        SaleAccountId = s.SaleAccountId,
                        SaleTaxId = s.SaleTaxId,
                        InventoryAccountId = s.InventoryAccountId,
                        UseBatchNo = s.UseBatchNo,
                        TrackExpiration = s.TrackExpiration,
                        TrackSerial = s.TrackSerial,
                        ItemTypeId = s.ItemTypeId,
                        ManageInventory = s.ItemType.DisplayInventoryAccount,
                        ShowSubItems = s.ShowSubItems
                    }).ToListAsync();

                    invoiceAuto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Invoice);
                    issueAuto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);

                    turnOffStockValidation = await _settingRepository.GetAll().AsNoTracking()
                                                   .AnyAsync(s => s.SettingType == BillInvoiceSettingType.Invoice && s.TurnOffStockValidationForImportExcel);
                    #region journal transaction type 
                     transactionTypeId = await _journalTransactionTypeManager.GetJournalTransactionTypeId(InventoryTransactionType.ItemIssueSale);                
                    #endregion
                }
            }

            var addItemIssueItemBatchNos = new List<ItemIssueItemBatchNo>();
            var addItemIssueItems = new List<ItemIssueItem>();
            var addItemIssues = new List<ItemIssue>();
            var addInvoiceItems = new List<InvoiceItem>();
            var addInvoices = new List<Invoice>();
            var addInvoiceJournalItems = new List<JournalItem>();
            var addInvoiceJournals = new List<Journal>();
            var addItemIssueJournalItems = new List<JournalItem>();
            var addItemIssueJournals = new List<Journal>();
            var addInventoryTransactionItems = new List<InventoryTransactionItem>();

            var invoiceDic = new Dictionary<string, Invoice>();
            var itemIssueDic = new Dictionary<string, ItemIssue>();

            // Get the work book in the file
            var workBook = excelPackage.Workbook;
            if (workBook != null)
            {
                // retrive first worksheets
                var worksheet = excelPackage.Workbook.Worksheets[0];



                for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                {
                    var invoiceGroup = worksheet.Cells[i, 1].Value?.ToString();
                    var date = worksheet.Cells[i, 2].Value?.ToString();
                    var issueDate = worksheet.Cells[i, 3].Value?.ToString();
                    var customerCode = worksheet.Cells[i, 4].Value?.ToString();
                    var aRAccountCode = worksheet.Cells[i, 5].Value?.ToString();
                    var locationName = worksheet.Cells[i, 6].Value?.ToString();
                    var className = hasClassFeature ? worksheet.Cells[i, 7].Value?.ToString() : "";
                    var currencyCode = worksheet.Cells[i, 8 + indexHasClassFeature].Value?.ToString();
                    var memo = worksheet.Cells[i, 9 + indexHasClassFeature].Value?.ToString();
                    var reference = worksheet.Cells[i, 10 + indexHasClassFeature].Value?.ToString();
                    var invoiceNo = worksheet.Cells[i, 11 + indexHasClassFeature].Value?.ToString();

                    var itemCode = worksheet.Cells[i, 12 + indexHasClassFeature].Value.ToString();
                    var zone = worksheet.Cells[i, 13 + indexHasClassFeature].Value.ToString();
                    var qtyText = worksheet.Cells[i, 14 + indexHasClassFeature].Value.ToString();
                    var unitPriceText = worksheet.Cells[i, 15 + indexHasClassFeature].Value.ToString();
                    var unitPriceInAccountText = worksheet.Cells[i, 16 + indexHasClassFeature].Value.ToString();
                    var itemDescription = worksheet.Cells[i, 17 + indexHasClassFeature].Value?.ToString();
                    var batchSerial = worksheet.Cells[i, 18 + indexHasClassFeature].Value?.ToString();
                    var expiration = worksheet.Cells[i, 19 + indexHasClassFeature].Value?.ToString();
                    var saleTypeName = worksheet.Cells[i, 20 + indexHasClassFeature].Value?.ToString();

                    var currency = currencies.Where(s => s.Name == currencyCode).FirstOrDefault();
                    var defaultClass = hasClassFeature ? classes.Where(s => s.Name == className).Select(t => t.Value).FirstOrDefault() : tenant.ClassId;
                    var location = locations.Where(s => s.Name == locationName).FirstOrDefault();
                    var customer = customers.Where(s => s.Name == customerCode).FirstOrDefault();
                    var arAccount = accounts.Where(s => s.AccountCode == aRAccountCode).FirstOrDefault();
                    var lot = lots.FirstOrDefault(s => s.LotName == zone);
                    var saleType = saleTypes.Where(s => s.Name == saleTypeName).FirstOrDefault();
                    if (invoiceAuto.CustomFormat == false && invoiceNo == null)
                    {
                        throw new UserFriendlyException(L("IsRequired", L("InvoiceNo")) + $", Row = {i}");
                    }

                    if (currency == null)
                    {
                        throw new UserFriendlyException(L("NoCurrencyFound") + $", Row = {i}");
                    }
                    if (defaultClass == null)
                    {
                        throw new UserFriendlyException(L("NoClassFound") + $", Row = {i}");
                    }
                    if (customer == null)
                    {
                        throw new UserFriendlyException(L("NoCustomerFound") + $", Row = {i}");
                    }

                    if (arAccount == null)
                    {
                        throw new UserFriendlyException(L("NoARAccountFound") + $", Row = {i}");
                    }
                    if (location == null)
                    {
                        throw new UserFriendlyException(L("NoLocationFound") + $", Row = {i}");
                    }

                    if (invoiceAuto.RequireReference)
                    {
                        if (reference.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("IsRequired", L("Reference")) + $", Row = {i}");

                        var find = invoiceJournalHash.Any(s => s.Reference != null && s.Reference.ToLower() == reference.ToLower() && s.PartnerId == customer.Value);
                        if (find) throw new UserFriendlyException(L("DuplicateReferenceNo") + $", Row = {i}");
                    }

                    var item = items.FirstOrDefault(s => s.ItemCode == itemCode);
                    if (item == null) throw new UserFriendlyException(L("InvalidItemCode") + $", Row = {i}");

                    if (item.ManageInventory)
                    {
                        if (lot == null) throw new UserFriendlyException(L("IsRequired", L("Zone")) + $", Row = {i}");
                        if (lot.LocationId != location.Value) throw new UserFriendlyException(L("InvalidLocationZone") + $", {locationName}, {zone}, Row = {i}");
                    }

                    var autoIssue = !issueDate.IsNullOrWhiteSpace();
                    if (autoIssue)
                    {
                        if (item.UseBatchNo || item.TrackSerial || item.TrackExpiration)
                        {
                            if (batchSerial.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("PleaseEnter", L("Batch/Serial")) + $", Row = {i}");
                            if (item.TrackExpiration && expiration.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("PleaseEnter", L("Expiration")) + $", Row = {i}");

                            var findBatch = batchNos.Any(s => s.BatchNumber == batchSerial);
                            if (!findBatch) throw new UserFriendlyException(L("Invalid", L("Batch/Serial")) + $", Row = {i}");
                        }
                    }

                    var qty = Convert.ToDecimal(qtyText);
                    if (autoIssue && item.TrackSerial && qty != 1) throw new UserFriendlyException(L("MustBeEqualTo", $"{L("SerialNo")} {L("Qty")}", 1) + $", Row = {i}");

                    var unitPrice = Math.Round(Convert.ToDecimal(unitPriceText), currentCycle.RoundingDigitUnitCost);
                    var unitPriceInAccount = Math.Round(Convert.ToDecimal(unitPriceInAccountText), currentCycle.RoundingDigitUnitCost);
                    var totalInAcc = Math.Round(qty * unitPriceInAccount, currentCycle.RoundingDigit);
                    var totalInTran = Math.Round(qty * unitPrice, currentCycle.RoundingDigit);

                    var addInvoiceItem = InvoiceItem.Create(tenantId, userId, Guid.Empty, item.SaleTaxId.Value, item.Id, itemDescription, qty, unitPriceInAccount, 0, totalInAcc, unitPrice, totalInTran);
                    var addInvoiceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, Guid.Empty, item.SaleAccountId.Value, itemDescription, 0, totalInAcc, PostingKey.Revenue, addInvoiceItem.Id);

                    if (item.ManageInventory) addInvoiceItem.UpdateLot(lot.Id);
                    addInvoiceItems.Add(addInvoiceItem);
                    addInvoiceJournalItems.Add(addInvoiceJournalItem);

                    if (invoiceDic.ContainsKey(invoiceGroup))
                    {
                        var addInvoice = invoiceDic[invoiceGroup];
                        addInvoice.SetSubTotal(addInvoice.SubTotal + totalInAcc);
                        addInvoice.SetTotal(addInvoice.Total + totalInAcc);
                        addInvoice.SetMultiCurrencySubTotal(addInvoice.MultiCurrencySubTotal + totalInTran);
                        addInvoice.SetMultiCurrencyTotal(addInvoice.MultiCurrencyTotal + totalInTran);
                        addInvoice.SetOpenBalance(addInvoice.Total);
                        addInvoice.SetMultiCurrencyOpenBalance(addInvoice.MultiCurrencyTotal);

                        var addInvoiceJournal = addInvoiceJournals.FirstOrDefault(s => s.InvoiceId == addInvoice.Id);
                        addInvoiceJournal.SetDebitCredit(addInvoice.Total);

                        var arAccountItem = addInvoiceJournalItems.FirstOrDefault(s => s.JournalId == addInvoiceJournal.Id && s.Key == PostingKey.AR);
                        arAccountItem.SetDebitCredit(addInvoice.Total, 0);

                        addInvoiceItem.SetInvoice(addInvoice.Id);
                        addInvoiceJournalItem.SetJournal(addInvoiceJournal.Id);
                    }
                    else
                    {

                        if (invoiceAuto.CustomFormat == true)
                        {
                            var newAuto = _autoSequenceManager.GetNewReferenceNumber(invoiceAuto.DefaultPrefix, invoiceAuto.YearFormat.Value,
                                           invoiceAuto.SymbolFormat, invoiceAuto.NumberFormat, invoiceAuto.LastAutoSequenceNumber, DateTime.Now);

                            invoiceNo = newAuto;
                            invoiceAuto.UpdateLastAutoSequenceNumber(newAuto);
                        }
                        else if (invoiceJournalHash.Any(s => s.JournalNo == invoiceNo) || addInvoiceJournals.Any(s => s.JournalNo == invoiceNo))
                        {
                            throw new UserFriendlyException(L("DuplicateInvoiceNo", invoiceNo) + $", Row = {i}");
                        }

                        if (invoiceAuto.RequireReference)
                        {
                            var find = from j in addInvoiceJournals
                                       join ii in addInvoices
                                       on j.InvoiceId equals ii.Id
                                       where j.Reference != null && j.Reference.ToLower() == reference.ToLower() && ii.CustomerId == customer.Value
                                       select j;

                            if (find.Any()) throw new UserFriendlyException(L("DuplicateReferenceNo") + $", Row = {i}");
                        }

                        CAddress billAddress = new CAddress("", "", "", "", "");
                        CAddress shipAddress = new CAddress("", "", "", "", "");

                        var addInvoice = Invoice.Create(tenantId, userId, ReceiveFrom.None, Convert.ToDateTime(date), customer.Value, true, shipAddress, billAddress, totalInAcc, 0, totalInAcc, null, Convert.ToDateTime(date), autoIssue ? Convert.ToDateTime(issueDate) : Convert.ToDateTime(date), autoIssue, totalInTran, 0, totalInTran, true, useExchangeRate);
                        var addInvoiceJournal = Journal.Create(tenantId, userId, invoiceNo, Convert.ToDateTime(date), memo, totalInAcc, totalInAcc, tenant.CurrencyId.Value, defaultClass.Value, reference, location.Value);
                        var arJournalItem = JournalItem.CreateJournalItem(tenantId, userId, addInvoiceJournal.Id, arAccount.Id, memo, totalInAcc, 0, PostingKey.AR, null);

                        addInvoiceJournal.UpdateMultiCurrency(currency.Value);
                        addInvoiceJournal.UpdateInvoice(addInvoice.Id);
                        addInvoiceJournal.UpdateStatus(TransactionStatus.Publish);
                        addInvoice.UpdateTransactionTypeId(saleType?.Value);
                        addInvoices.Add(addInvoice);
                        addInvoiceJournals.Add(addInvoiceJournal);
                        addInvoiceJournalItems.Add(arJournalItem);

                        addInvoiceItem.SetInvoice(addInvoice.Id);
                        addInvoiceJournalItem.SetJournal(addInvoiceJournal.Id);

                        invoiceDic.Add(invoiceGroup, addInvoice);
                    }

                    if (autoIssue && item.ManageInventory)
                    {
                        var addItemIssueItem = ItemIssueItem.Create(tenantId, userId, Guid.Empty, item.Id, itemDescription, qty, unitPriceInAccount, 0, totalInAcc);
                        var addItemIssueJournalItem = JournalItem.CreateJournalItem(tenantId, userId, Guid.Empty, item.InventoryAccountId.Value, itemDescription, 0, totalInAcc, PostingKey.Inventory, addItemIssueItem.Id);
                        var addCogsItemIssueJournalItem = JournalItem.CreateJournalItem(tenantId, userId, Guid.Empty, item.PurchaseAccountId.Value, itemDescription, totalInAcc, 0, PostingKey.COGS, addItemIssueItem.Id);

                        addItemIssueItem.UpdateLot(lot.Id);
                        addItemIssueItems.Add(addItemIssueItem);
                        addItemIssueJournalItems.Add(addItemIssueJournalItem);
                        addItemIssueJournalItems.Add(addCogsItemIssueJournalItem);

                        //Set reference to invoice item
                        addInvoiceItem.UpdateIssueItemId(addItemIssueItem.Id);

                        Journal addItemIssueJournal = null;

                        if (itemIssueDic.ContainsKey(invoiceGroup))
                        {
                            var addItemIssue = itemIssueDic[invoiceGroup];
                            addItemIssue.UpdateTotal(addItemIssue.Total + totalInAcc);

                            addItemIssueJournal = addItemIssueJournals.FirstOrDefault(s => s.ItemIssueId == addItemIssue.Id);
                            addItemIssueJournal.SetDebitCredit(addItemIssue.Total);

                            addItemIssueItem.SetItemIssue(addItemIssue.Id);
                            addItemIssueJournalItem.SetJournal(addItemIssueJournal.Id);
                            addCogsItemIssueJournalItem.SetJournal(addItemIssueJournal.Id);
                        }
                        else
                        {
                            var itemIssueNo = invoiceNo;
                            if (autoIssue && issueAuto.CustomFormat == true)
                            {
                                var newAuto = _autoSequenceManager.GetNewReferenceNumber(issueAuto.DefaultPrefix, issueAuto.YearFormat.Value,
                                                issueAuto.SymbolFormat, issueAuto.NumberFormat, issueAuto.LastAutoSequenceNumber, DateTime.Now);
                                itemIssueNo = newAuto;
                                issueAuto.UpdateLastAutoSequenceNumber(newAuto);
                            }
                            else if (itemIssueJournalHash.Contains(itemIssueNo) || addItemIssueJournals.Any(s => s.JournalNo == itemIssueNo))
                            {
                                throw new UserFriendlyException(L("DuplicateItemIssueNo", itemIssueNo) + $", Row = {i}");
                            }

                            var addInvoice = invoiceDic[invoiceGroup];

                            var addItemIssue = ItemIssue.Create(tenantId, userId, ReceiveFrom.Invoice, customer.Value, true, addInvoice.ShippingAddress, addInvoice.BillingAddress, totalInAcc, null);

                            addItemIssueJournal = Journal.Create(tenantId, userId, itemIssueNo, Convert.ToDateTime(issueDate), memo, totalInAcc, totalInAcc, currency.Value, defaultClass.Value, reference, location.Value);

                            addItemIssueJournal.UpdateItemIssue(addItemIssue.Id);
                            addItemIssueJournal.UpdateStatus(TransactionStatus.Publish);
                            addItemIssueJournal.SetJournalTransactionTypeId(transactionTypeId);
                            addItemIssues.Add(addItemIssue);
                            addItemIssueJournals.Add(addItemIssueJournal);

                            addItemIssueItem.SetItemIssue(addItemIssue.Id);
                            addItemIssueJournalItem.SetJournal(addItemIssueJournal.Id);
                            addCogsItemIssueJournalItem.SetJournal(addItemIssueJournal.Id);

                            //Set reference to invoice
                            addInvoice.UpdateItemIssueId(addItemIssue.Id);
                            addInvoice.UpdateReceivedStatus(DeliveryStatus.ShipAll);

                            itemIssueDic.Add(invoiceGroup, addItemIssue);
                        }


                        //manage batch serial and expiration
                        if (item.TrackSerial || item.UseBatchNo || item.TrackExpiration)
                        {

                            var batchNoId = Guid.Empty;
                            var expiredDate = item.TrackExpiration ? Convert.ToDateTime(expiration) : (DateTime?)null;

                            var findBatch = batchNos.FirstOrDefault(s => s.BatchNumber == batchSerial);
                            if (findBatch != null)
                            {
                                if (findBatch.ItemId != item.Id) throw new UserFriendlyException(L("BatchNoAlreadyUseByOtherItem") + $" {batchSerial}, Row = {i}");
                                if (item.TrackExpiration && findBatch.ExpirationDate.Value.Date != expiredDate.Value.Date)
                                {
                                    throw new UserFriendlyException(L("IsNotValid", L("ExpirationDate")) + $" : {expiredDate.Value.ToString("dd-MM-yyyy")}, Row = {i}");
                                }
                                batchNoId = findBatch.Id;
                            }
                            else
                            {
                                throw new UserFriendlyException(L("Invalid", L("Batch/Serial")) + $" {batchSerial}, Row = {i}");
                            }

                            var addItemIssueItemBatchNo = ItemIssueItemBatchNo.Create(tenantId.Value, userId, addItemIssueItem.Id, batchNoId, addItemIssueItem.Qty);
                            addItemIssueItemBatchNos.Add(addItemIssueItemBatchNo);
                        }


                        //Sync inventory transaction items
                        var addInventoryTransactionItem = InventoryTransactionItem.Create(
                                                      tenantId.Value,
                                                      userId,
                                                      addItemIssueItem.CreationTime,
                                                      addItemIssueItem.LastModifierUserId,
                                                      addItemIssueItem.LastModificationTime,
                                                      addItemIssueItem.Id,
                                                      addItemIssueItem.ItemIssueId,
                                                      null,
                                                      null,
                                                      addItemIssueJournal.Id,
                                                      addItemIssueJournal.Date,
                                                      addItemIssueJournal.CreationTimeIndex.Value,
                                                      addItemIssueJournal.JournalType,
                                                      addItemIssueJournal.JournalNo,
                                                      addItemIssueJournal.Reference,
                                                      addItemIssueItem.ItemId,
                                                      addItemIssueJournalItem.AccountId,
                                                      location.Value,
                                                      addItemIssueItem.LotId.Value,
                                                      addItemIssueItem.Qty,
                                                      addItemIssueItem.UnitCost,
                                                      addItemIssueItem.Total,
                                                      true,
                                                      addItemIssueItem.Description);

                        addInventoryTransactionItems.Add(addInventoryTransactionItem);

                    }

                }
            }


            #region Cost Calculation and Batch/Serial Validation

            if (!turnOffStockValidation && addItemIssueItems.Any())
            {
                var itemDic = items.ToDictionary(k => k.Id, v => v);
                var itemToCalculateCost = addItemIssueItems.Select((u, index) => new Inventories.Data.GetAvgCostForIssueData()
                {
                    Index = index,
                    ItemId = u.ItemId,
                    ItemName = itemDic[u.ItemId].ItemName,
                    ItemCode = itemDic[u.ItemId].ItemCode,
                    InventoryAccountId = itemDic[u.ItemId].InventoryAccountId.Value,
                    Qty = u.Qty,
                    LotId = u.LotId
                }).ToList();
                var lotIds = addItemIssueItems.Select(x => x.LotId).ToList();
                var locationIds = lots.Where(s => lotIds.Contains(s.Id)).Select(s => (long?)s.LocationId).ToList();
                var toDate = addItemIssueJournals.Max(s => s.Date);

                GetAvgCostForIssueOutput getCostResult = null;
                List<ItemIssueItem> itemIssueUseBatchs = null;
                List<BatchNoItemBalanceOutput> batchBalanceItems = null;
                List<Guid> itemIdsUseBatch = null;
                List<long> batchLots = null;
                List<Guid> batchNoIds = null;

                if (addItemIssueItemBatchNos.Any())
                {
                    var itemIds = items.Where(s => s.TrackExpiration || s.UseBatchNo || s.TrackSerial).Select(s => s.Id).ToList();
                    itemIssueUseBatchs = addItemIssueItems.Where(s => itemIds.Contains(s.ItemId)).ToList();
                    itemIdsUseBatch = itemIssueUseBatchs.GroupBy(g => g.ItemId).Select(s => s.Key).ToList();
                    batchLots = itemIssueUseBatchs.GroupBy(g => g.LotId).Select(s => s.Key.Value).ToList();
                    batchNoIds = addItemIssueItemBatchNos.GroupBy(g => g.BatchNoId).Select(s => s.Key).ToList();
                }


                using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        getCostResult = await _inventoryManager.GetAvgCostForIssues(toDate, locationIds, itemToCalculateCost);

                        if (addItemIssueItemBatchNos.Any())
                        {
                            batchBalanceItems = await _inventoryManager.GetItemBatchNoBalance(itemIdsUseBatch, batchLots, batchNoIds, toDate, new List<Guid>());
                        }
                    }
                }


                if (addItemIssueItemBatchNos.Any())
                {
                    var balanceDic = batchBalanceItems.ToDictionary(k => $"{k.ItemId}-{k.LotId}-{k.BatchNoId}", v => v.Qty);
                    var itemIssueItemDic = itemIssueUseBatchs.ToDictionary(k => k.Id, v => v);
                    var batchNoDic = batchNos.ToDictionary(k => k.Id, v => v);

                    var batchQtyUse = addItemIssueItemBatchNos
                                      .Select(s => new { itemIssueItemDic[s.ItemIssueItemId].ItemId, itemIssueItemDic[s.ItemIssueItemId].LotId, s.BatchNoId, s.Qty })
                                      .GroupBy(s => new { s.ItemId, s.LotId, s.BatchNoId })
                                      .Select(s => new
                                      {
                                          ItemName = $"{itemDic[s.Key.ItemId].ItemCode}-{itemDic[s.Key.ItemId].ItemName}, BatchNo: {batchNoDic[s.Key.BatchNoId].BatchNumber}",
                                          Key = $"{s.Key.ItemId}-{s.Key.LotId}-{s.Key.BatchNoId}",
                                          Qty = s.Sum(t => t.Qty)
                                      })
                                      .ToList();

                    var notInStock = batchQtyUse.FirstOrDefault(s => !balanceDic.ContainsKey(s.Key));
                    if (notInStock != null) throw new UserFriendlyException(L("ItemOutOfStock", notInStock.ItemName));

                    var issueOverQty = batchQtyUse.FirstOrDefault(s => balanceDic.ContainsKey(s.Key) && s.Qty > balanceDic[s.Key]);
                    if (issueOverQty != null)
                        throw new UserFriendlyException(L("CannotIssueForThisBiggerStockItem",
                            issueOverQty.ItemName,
                            String.Format("{0:n0}", balanceDic[issueOverQty.Key]),
                            String.Format("{0:n0}", issueOverQty.Qty)));
                }


                foreach (var r in getCostResult.Items)
                {
                    addItemIssueItems[r.Index].UpdateUnitCost(r.UnitCost, r.LineCost);

                    var updateItemIssueJournalItems = addItemIssueJournalItems.Where(s => s.Identifier == addItemIssueItems[r.Index].Id).ToList();
                    foreach (var i in updateItemIssueJournalItems)
                    {
                        if (i.Key == PostingKey.Inventory)
                        {
                            i.SetDebitCredit(0, r.LineCost);
                        }
                        else if (i.Key == PostingKey.COGS)
                        {
                            i.SetDebitCredit(r.LineCost, 0);
                        }
                    }

                    var addInventoryTransactionItem = addInventoryTransactionItems.FirstOrDefault(s => s.Id == addItemIssueItems[r.Index].Id);
                    addInventoryTransactionItem.SetUnitCost(r.UnitCost);
                    addInventoryTransactionItem.SetLineCost(r.LineCost);
                }

                foreach (var i in addItemIssues)
                {
                    var total = addItemIssueItems.Where(s => s.ItemIssueId == i.Id).Sum(t => t.Total);
                    i.UpdateTotal(total);

                    var j = addItemIssueJournals.FirstOrDefault(s => s.ItemIssueId == i.Id);
                    j.SetDebitCredit(total);
                }
            }
            #endregion 


            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    if (addItemIssues.Any()) await _itemIssueRepository.BulkInsertAsync(addItemIssues);
                    if (addItemIssueItems.Any()) await _itemIssueItemRepository.BulkInsertAsync(addItemIssueItems);
                    if (addItemIssueItemBatchNos.Any()) await _itemIssueItemBatchNoRepository.BulkInsertAsync(addItemIssueItemBatchNos);
                    if (addItemIssueJournals.Any()) await _journalRepository.BulkInsertAsync(addItemIssueJournals);
                    if (addItemIssueJournalItems.Any()) await _journalItemRepository.BulkInsertAsync(addItemIssueJournalItems);

                    if (addInvoices.Any()) await _invoiceRepository.BulkInsertAsync(addInvoices);
                    if (addInvoiceItems.Any()) await _invoiceItemRepository.BulkInsertAsync(addInvoiceItems);
                    if (addInvoiceJournals.Any()) await _journalRepository.BulkInsertAsync(addInvoiceJournals);
                    if (addInvoiceJournalItems.Any()) await _journalItemRepository.BulkInsertAsync(addInvoiceJournalItems);

                    //Sync Inventroy Transaction Item
                    if (addInventoryTransactionItems.Any()) await _inventoryTransactionItemRepository.BulkInsertAsync(addInventoryTransactionItems);

                    if (invoiceAuto.CustomFormat) CheckErrors(await _autoSequenceManager.UpdateAsync(invoiceAuto));
                    if (addItemIssues.Any() && issueAuto.CustomFormat) CheckErrors(await _autoSequenceManager.UpdateAsync(issueAuto));
                }

                await uow.CompleteAsync();
            }
        }

        #endregion
    }
}
