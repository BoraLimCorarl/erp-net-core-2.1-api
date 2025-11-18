using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Currencies;
using CorarlERP.Customers;
using CorarlERP.Customers.Dto;
using CorarlERP.Invoices.Dto;
using CorarlERP.ItemIssues;
using CorarlERP.Items;
using CorarlERP.Journals;
using CorarlERP.SaleOrders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CorarlERP.enumStatus.EnumStatus;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using CorarlERP.Currencies.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.SaleOrders.Dto;
using CorarlERP.Items.Dto;
using CorarlERP.Taxes.Dto;
using CorarlERP.Journals.Dto;
using CorarlERP.Authorization;
using CorarlERP.ItemReceipts;
using CorarlERP.ReceivePayments;
using CorarlERP.ItemReceiptCustomerCredits;
using CorarlERP.Invoices;
using CorarlERP.ItemIssues.Dto;
using CorarlERP.Inventories;
using CorarlERP.Inventories.Data;
using CorarlERP.CustomerManager;
using CorarlERP.AutoSequences;
using CorarlERP.ItemTypes;
using CorarlERP.Authorization.Users;
using CorarlERP.Dto;
using CorarlERP.Lots.Dto;
using CorarlERP.Lots;
using CorarlERP.Locations.Dto;
using CorarlERP.UserGroups;
using OfficeOpenXml;
using CorarlERP.Reports;
using System.IO;
using CorarlERP.Addresses;
using CorarlERP.CustomerCredits;
using Abp.AspNetZeroCore.Net;
using EvoPdf.HtmlToPdfClient;
using CorarlERP.Locks;
using CorarlERP.Common.Dto;
using CorarlERP.POS.Dto;
using CorarlERP.InvoiceSales;
using Abp.Dependency;
using AutoMapper;
using Abp.Timing;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using CorarlERP.PaymentMethods;
using CorarlERP.ReceivePayments.Dto;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using CorarlERP.MultiTenancy;
using CorarlERP.TransactionTypes;
using CorarlERP.Url;
using System.Globalization;
using CorarlERP.FileStorages;
using Hangfire.States;
using CorarlERP.InventoryCalculationJobSchedules;
using CorarlERP.InventoryCalculationItems;
using CorarlERP.BatchNos;
using CorarlERP.JournalTransactionTypes;
using CorarlERP.Features;

namespace CorarlERP.POS
{
    [AbpAuthorize]
    public class POSAppService : ReportBaseClass, IPOSAppService
    {
        protected readonly AppFolders _appFolders;
        protected readonly IRepository<CustomerCredits.CustomerCredit, Guid> _customerCreditRepository;
        protected readonly IRepository<CustomerCredits.CustomerCreditDetail, Guid> _customerCreditItemRepository;
        protected readonly IRepository<CustomerCredit, Guid> _cusotmerCreditRepository;
        protected readonly IRepository<ItemIssueItem, Guid> _itemIssueItemRepository;
        protected readonly IItemIssueItemManager _itemIssueItemManager;

        protected readonly IRepository<ItemIssue, Guid> _itemIssueRepository;
        protected readonly IItemIssueManager _itemIssueManager;

        protected readonly ICurrencyManager _currencyManager;
        protected readonly IRepository<Currency, long> _currencyRepository;

        protected readonly ICustomerManagerHelper _customerHelperManager;

        protected readonly ICustomerManager _customerManager;
        protected readonly IRepository<Customer, Guid> _customerRepository;

        protected readonly ISaleOrderItemManager _saleOrderItemManager;
        protected readonly IRepository<SaleOrderItem, Guid> _saleOrderItemRepository;

        protected readonly ISaleOrderManager _saleOrderManager;
        protected readonly IRepository<SaleOrder, Guid> _saleOrderRepository;

        protected readonly IInvoiceItemManager _invoiceItemManager;
        protected readonly IInvoiceManager _invoiceManager;

        protected readonly IRepository<Invoice, Guid> _invoiceRepository;
        protected readonly IRepository<Lot, long> _lotRepository;
        protected readonly IRepository<InvoiceItem, Guid> _invoiceItemRepository;
        private string _baseUrl;

        protected readonly IJournalManager _journalManager;
        protected readonly IRepository<Journal, Guid> _journalRepository;

        protected readonly IJournalItemManager _journalItemManager;
        protected readonly IRepository<JournalItem, Guid> _journalItemRepository;

        protected readonly IChartOfAccountManager _chartOfAccountManager;
        protected readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;

        protected readonly IRepository<Item, Guid> _itemRepository;
        protected readonly IItemManager _itemManager;

        protected readonly IRepository<ItemReceiptItem, Guid> _itemReceiptItemRepository;

        protected readonly IRepository<ReceivePayments.ReceivePaymentDetail, Guid> _receivePaymentDetailRepository;
        protected readonly IRepository<ReceivePayment, Guid> _receivePaymentRepository;
        protected readonly IRepository<ItemReceiptCustomerCredit, Guid> _itemReceiptCustomerCreditRepository;
        protected readonly IRepository<ItemReceiptItemCustomerCredit, Guid> _itemReceiptItemCustomerCreditRepository;
        protected readonly IInventoryManager _inventoryManager;

        protected readonly IAutoSequenceManager _autoSequenceManager;
        protected readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;
        protected readonly IRepository<User, long> _userRepository;

        protected readonly IRepository<Taxes.Tax, long> _taxRepository;
        protected readonly IRepository<Locations.Location, long> _locationRepository;
        protected readonly IRepository<Classes.Class, long> _classRepository;
        protected readonly IRepository<AccountCycles.AccountCycle, long> _accountCyclesRepository;

        protected readonly IRepository<Lock, long> _lockRepository;

        protected readonly IRepository<MultiCurrencies.MultiCurrency, long> _multiCurrencyRepository;
        private readonly IReceivePaymentManager _receivePaymentManager;
        private readonly IReceivePaymentDetailManager _receivePaymentDetailManager;
        private readonly IRepository<ReceivePaymentExpense, Guid> _receivePaymentExpenseRepository;
        private readonly IReceivePaymentExpenseManager _receivePaymentExpenseManager;
        private readonly ITenantManager _tenantManager;

        private readonly IRepository<TransactionType, long> _saleTypeRepository;
        private ITransactionTypeManager _saleTypeManager;
        private ICustomerCreditManager _customerCreditManager;
        private readonly IFileStorageManager _fileStorageManager;
        private readonly IInventoryCalculationItemManager _inventoryCalculationItemManager;

        protected readonly ICorarlRepository<BatchNo, Guid> _batchNoRepository;
        private readonly ICorarlRepository<ItemIssueItemBatchNo, Guid> _itemIssueItemBatchNoRepository;
        private readonly IJournalTransactionTypeManager _journalTransactionTypeManager;
        public POSAppService(IJournalManager journalManager, IJournalItemManager journalItemManager,
                ICurrencyManager currencyManager,
                IChartOfAccountManager chartOfAccountManager,
                InvoiceManager invoiceManager,
                InvoiceItemManager invoiceItemManager,
                ItemIssueItemManager itemIssueItemManager,
                ItemIssueManager itemIssueManager,
                ItemManager itemManager,
                SaleOrderItemManager saleOrderItemManager,
                ISaleOrderManager saleOrderManager,
                ICustomerManager customerManager,
                IRepository<Customer, Guid> customerRepository,
                IRepository<Journal, Guid> journalRepository,
                IRepository<JournalItem, Guid> journalItemRepository,
                IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
                IRepository<Invoice, Guid> invoiceRepository,
                IRepository<InvoiceItem, Guid> invoiceItemRepository,
                IRepository<SaleOrderItem, Guid> saleOrderItemRepository,
                IRepository<SaleOrder, Guid> saleOrderRepository,
                IRepository<Currency, long> currencyRepository,
                IRepository<ItemIssueItem, Guid> itemIssueItemRepository,
                IRepository<ItemIssue, Guid> itemIssueRepository,
                IRepository<Item, Guid> itemRepository,
                ICorarlRepository<BatchNo, Guid> batchNoRepository,
                ICorarlRepository<ItemIssueItemBatchNo, Guid> itemIssueItemBatchNoRepository,
                IRepository<ReceivePayments.ReceivePaymentDetail, Guid> receivePaymentDetailRepository,
                IRepository<ReceivePayment, Guid> receivePaymentRepository,
                IRepository<ItemReceiptItem, Guid> itemReceiptItemRepository,
                IRepository<CustomerCredits.CustomerCredit, Guid> customerCreditRepository,
                IRepository<CustomerCredits.CustomerCreditDetail, Guid> customerCreditDetailRepository,
                IRepository<ItemReceiptCustomerCredit, Guid> itemReceiptCustomerCreditRepository,
                IInventoryCalculationItemManager inventoryCalculationItemManager,
                IInventoryManager inventoryManager,
                ICustomerManagerHelper customerHelperManager,
                IAutoSequenceManager autoSequenceManger,
                IRepository<AutoSequence, Guid> autoSequenceRepository,
                IRepository<User, long> userRepository,
                IRepository<Lot, long> lotRepository,
                IRepository<UserGroupMember, Guid> userGroupMemberRepository,
                IRepository<CustomerCredit, Guid> cusotmerCreditRepository,
                IRepository<Lock, long> lockRepository,
                IRepository<Taxes.Tax, long> taxRepository,
                IRepository<Locations.Location, long> locationRepository,
                IRepository<Classes.Class, long> classRepository,
                IRepository<AccountCycles.AccountCycle, long> accountCyclesRepository,
                IRepository<TransactionType, long> saleTypeRepository,
                ITransactionTypeManager saleTypeManager,
                IFileStorageManager fileStorageManager,
                AppFolders appFolders,
                IJournalTransactionTypeManager journalTransactionTypeManager,
                IRepository<ItemReceiptItemCustomerCredit, Guid> itemReceiptItemCustomerCreditRepository
            ) : base(accountCyclesRepository, appFolders, userGroupMemberRepository, locationRepository)
        {
            _baseUrl = (IocManager.Instance.Resolve<IWebUrlService>()).GetServerRootAddress().EnsureEndsWith('/');
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
            _inventoryCalculationItemManager = inventoryCalculationItemManager;
            _customerHelperManager = customerHelperManager;
            _autoSequenceManager = autoSequenceManger;
            _autoSequenceRepository = autoSequenceRepository;
            _appFolders = appFolders;
            _locationRepository = locationRepository;
            _classRepository = classRepository;
            _taxRepository = taxRepository;
            _lockRepository = lockRepository;

            _multiCurrencyRepository = IocManager.Instance.Resolve<IRepository<MultiCurrencies.MultiCurrency, long>>();
            _receivePaymentManager = IocManager.Instance.Resolve<IReceivePaymentManager>();
            _receivePaymentDetailManager = IocManager.Instance.Resolve<IReceivePaymentDetailManager>();
            _receivePaymentExpenseRepository = IocManager.Instance.Resolve<IRepository<ReceivePaymentExpense, Guid>>();
            _receivePaymentExpenseManager = IocManager.Instance.Resolve<IReceivePaymentExpenseManager>();
            _tenantManager = IocManager.Instance.Resolve<ITenantManager>();
            _customerCreditManager = IocManager.Instance.Resolve<ICustomerCreditManager>();

            _saleTypeRepository = saleTypeRepository;
            _saleTypeManager = saleTypeManager;
            _fileStorageManager = fileStorageManager;
            _batchNoRepository = batchNoRepository;
            _itemIssueItemBatchNoRepository = itemIssueItemBatchNoRepository;
            _journalTransactionTypeManager = journalTransactionTypeManager;

        }

        private async Task ValidateBatchNo(CreatePOSInput input)
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
            //if (input is UpdateInvoiceInput)
            //{
            //    var updateInput = input as UpdateInvoiceInput;
            //    if (updateInput.ItemIssueId.HasValue) exceptIds.Add(updateInput.ItemIssueId.Value);
            //}

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


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_POS_Create)]
        public async Task<FileDto> CreateAndPrint(CreatePOSInput input)
        {
            var saveInvoice = await SaveInvoiceHelper(input);
            var result = new EntityDto<Guid>() { Id = saveInvoice.Id.Value };
            var print = await Print(result);
            return print;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_POS_Create)]
        public async Task<string> CreateAndGetHTMLPrintContent(CreatePOSInput input)
        {
            var saveInvoice = await SaveInvoiceHelper(input);
            var result = new EntityDto<Guid>() { Id = saveInvoice.Id.Value };
            var print = await GetHTMLPrintContent(result);
            return print;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_POS_Print)]
        public async Task<string> GetHTMLPrintContent(EntityDto<Guid> input)
        {
            var detail = await GetDetail(input);
            var companyInfo = await _tenantManager.GetAsync(AbpSession.GetTenantId());

            var accountCyclePeriod = await GetPreviousRoundingCloseCyleAsync(detail.Date);

            var src = $"{_baseUrl}TenantCustomization/GetTenantLogo?skin=null&tenantId={AbpSession.TenantId}";
            var htmlPrintContent = companyInfo.LogoId == null ? "" : $"<img src=\"{src}\" alt=\"logo\" style=\"max-height: 200px; max-width: 200px; display: block; margin: 0px auto 15px;\"/>";
            htmlPrintContent += $"<table>";
            htmlPrintContent += $"<thead>";
            htmlPrintContent += $"<tr>";
            htmlPrintContent += $"<td colspan =\"4\" class=\"text-center\">";
            htmlPrintContent += $"<span class=\"font-16 bold\" style=\"display:inline-block; padding-bottom: 5px;\"> {companyInfo.Name}</span><br/>";
            htmlPrintContent += $"<span class=\"font-12\">{detail.Location.PhoneNumber}<br /><b>{detail.Location.LocationName}</b><span>";
            htmlPrintContent += $"</td>";
            htmlPrintContent += $"</tr>";
            htmlPrintContent += $"<tr>";
            htmlPrintContent += $"<td colspan=\"4\">&nbsp;</td>";
            htmlPrintContent += $"</tr>";
            htmlPrintContent += $"<tr class=\"padding\">";
            htmlPrintContent += $"<td>អតិថិជន<br>Customer</td>";
            htmlPrintContent += $"<td colspan =\"3\">";
            htmlPrintContent += $"<span class=\"\">{detail.CustomerName}</span></td>";
            htmlPrintContent += $"</tr>";
            htmlPrintContent += $"<tr>";
            htmlPrintContent += $"<td width =\"52px\">ថ្ងៃខែ<br>Date</td>";
            htmlPrintContent += $"<td style=\"padding: 3px 0px;\">{detail.Date.ToString("dd/MM/yyyy hh:mm tt", new CultureInfo("en-US"))}</td>";
            htmlPrintContent += $"<td style=\"padding: 3px 0px;\" class=\"text-right\" width=\"40px\">No.</td>";
            htmlPrintContent += $"<td class=\"text-right\" width=\"70px\">{detail.InvoiceNo}</td>";
            htmlPrintContent += $"</tr>";
            htmlPrintContent += $"<tr >";
            htmlPrintContent += $"<td style=\"padding: 0px 3px 3px 3px;\">អ្នកគិតលុយ<br>Cashier </td>";
            htmlPrintContent += $"<td style=\"padding: 0px 0px 3px 0px;\">{detail.CreatorUser}</td>";
            htmlPrintContent += $"<td style=\"padding: 0px 0px 3px 0px;\" class=\"text-right\">រូបិយប័ណ្ណ<br>Currency</td>";
            htmlPrintContent += $"<td style=\"padding: 0px 3px 3px 3px;\" class=\"text-right\"> {detail.CurrencyCode} </td>";
            htmlPrintContent += $"</tr>";
            htmlPrintContent += $"</thead>";
            htmlPrintContent += $"</table>";

            htmlPrintContent += $"<hr class=\"border-top\">";

            htmlPrintContent += $"<table class=\"padding\">";
            htmlPrintContent += $"<thead>";
            htmlPrintContent += $"<tr class=\"font-12 padding\">";
            htmlPrintContent += $"<th >ទំនិញ<br>Product</th>";
            htmlPrintContent += $"<th width=\"35px\" class=\"text-center\">ចំនួន<br>Qty</th>";
            htmlPrintContent += $"<th width=\"64px\" class=\"text-right\">តម្លៃរាយ<br>Unit Price</th>";
            htmlPrintContent += $"<th width=\"65px\" class=\"text-right\">សរុប<br>Amount</th>";
            htmlPrintContent += $"</tr>";
            htmlPrintContent += $"</thead>";
            htmlPrintContent += $"<tbody>";

            foreach (var i in detail.InvoiceItems)
            {
                string formatQty = this.AutoFormaDigits(i.Qty.ToString("N6"));
                htmlPrintContent += $"<tr class=\"padding\">";
                htmlPrintContent += $"<td>{ i.Item.ItemName }</td>";
                htmlPrintContent += $"<td class=\"text-center\">{ formatQty }</td>";
                htmlPrintContent += $"<td class=\"text-right\">{ FormatNumberCurrency(i.MultiCurrencyUnitCost, accountCyclePeriod.RoundingDigitUnitCost) }</td>";
                htmlPrintContent += $"<td class=\"text-right\">{ FormatNumberCurrency(i.MultiCurrencyTotal, accountCyclePeriod.RoundingDigit) }</td>";
                htmlPrintContent += "</tr>";
            }

            htmlPrintContent += $"</tbody>";
            htmlPrintContent += $"</table>";

            htmlPrintContent += $"<hr class=\"border-top\">";

            htmlPrintContent += $"<table class=\"padding\">";
            htmlPrintContent += $"<tbody>";
            //htmlPrintContent += $"<tr class=\"font-12 padding\">";
            //htmlPrintContent += $"<td colspan=\"2\">សរុបបឋម<br>Sub Total</td>";
            //htmlPrintContent += $"<td class=\"text-right\" colspan=\"2\">{FormatNumberCurrency(detail.MulticurrencySubTotal, accountCyclePeriod.RoundingDigit)}</td>";
            //htmlPrintContent += $"</tr>";
            //htmlPrintContent += $"<tr class=\"font-12 padding\">";
            //htmlPrintContent += $"<td colspan=\"2\" >ពន្ធ<br>Tax</td>";
            //htmlPrintContent += $"<td class=\"text-right\" colspan=\"2\">{FormatNumberCurrency(detail.MulticurrencyTax, accountCyclePeriod.RoundingDigit)}</td>";
            //htmlPrintContent += $"</tr>";
            htmlPrintContent += $"<tr class=\"font-12 padding\">";
            htmlPrintContent += $"<td class=\"bold\" colspan=\"2\">សរុប<br>Total</td>";
            htmlPrintContent += $"<td class=\"bold text-right\" colspan=\"2\">{FormatNumberCurrency(detail.MultiCurrencyTotal, accountCyclePeriod.RoundingDigit)}</td>";
            htmlPrintContent += $"</tr>";

            htmlPrintContent += $"<tr><td colspan=\"4\" style=\"line-height: 3px;\">&nbsp;</td></tr>";

            htmlPrintContent += $"<tr class=\"font-12\">";
            htmlPrintContent += $"<td class=\"bold\" colspan=\"4\"><div class=\"border charge\">";
            htmlPrintContent += $"<span>ប្រាក់ទទួល<br>Paid</span>";
            htmlPrintContent += $"<span style=\"position: relative; top: -14px; \" class=\"pull-right\"> {FormatNumberCurrency(detail.PaymentSummaries.Sum(s => s.MultiCurrencyTotal), accountCyclePeriod.RoundingDigit)}</span>";
            htmlPrintContent += $"</div></td>";
            htmlPrintContent += $"</tr>";

            htmlPrintContent += $"<tr class=\"font-16 bold\">";
            htmlPrintContent += $"<td colspan=\"2\">សមតុល្យ<br>Balance</td>";
            htmlPrintContent += $"<td class=\"text-right\" colspan=\"2\"> {FormatNumberCurrency(detail.MultiCurrencyTotal - detail.PaymentSummaries.Sum(s => s.MultiCurrencyTotal), accountCyclePeriod.RoundingDigit)}</td>";
            htmlPrintContent += $"</tr>";

            htmlPrintContent += $"</tbody>";
            htmlPrintContent += $"</table>";
            htmlPrintContent += $"<br/><div class=\"text-center\">សូមអរគុណ!<br>Thank You!</div>";

            return htmlPrintContent;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_POS_Create)]
        public async Task<string> PayMore(POSPaymoreInput input)
        {
            var @journal = await _journalRepository
                                .GetAll()
                                .Include(u => u.Invoice.Customer)
                                .Include(u => u.Location)
                                .Include(u => u.CreatorUser)
                                .Include(u => u.Currency)
                                .Include(u => u.MultiCurrency)
                                .Where(u => u.JournalType == JournalType.Invoice && u.InvoiceId == input.Id)
                                .FirstOrDefaultAsync();

            if (@journal == null || @journal.Invoice == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            else if (@journal.Invoice.PaidStatus == PaidStatuse.Paid)
            {
                throw new UserFriendlyException(L("InvoiceIsPaid"));
            }

            var account = await _journalItemRepository
                                .GetAll()
                                .Where(u => u.JournalId == journal.Id && u.Identifier == null && u.Key == PostingKey.AR)
                                .FirstOrDefaultAsync();

            var posInput = new CreatePOSInput
            {
                ClassId = journal.ClassId,
                CustomerId = journal.Invoice.CustomerId,
                LocationId = journal.LocationId.Value,
                CurrencyId = journal.CurrencyId,
                MultiCurrencyId = journal.MultiCurrencyId,
                Date = input.Date,
                IsConfirm = input.IsConfirm,
                AccountId = account.AccountId,
            };

            var invoice = journal.Invoice;

            //apply store credit   
            if (input.StoreCreditPayments != null && input.StoreCreditPayments.Any())
            {
                foreach (var storeCredit in posInput.StoreCreditPayments)
                {
                    if (invoice.MultiCurrencyOpenBalance == 0) break;
                    await ApplyStoreCredit(storeCredit, posInput, invoice);
                }
            }

            //prepare payments           
            foreach (var payment in input.ReceivePayments)
            {
                if (invoice.MultiCurrencyOpenBalance == 0) break;
                await CreatePaymentHelper(payment, posInput, invoice);
            }

            var result = new EntityDto<Guid>() { Id = input.Id };
            var print = await GetHTMLPrintContent(result);
            return print;
        }

        private string AutoFormaDigits(string value)
        {
            return value.IndexOf(".") < 0 ? value : value.Right(1) == "0" || value.Right(1) == "." ? AutoFormaDigits(value.Left(value.Length - 1)) : value;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_POS_Print)]
        public async Task<FileDto> Print(EntityDto<Guid> input)
        {
            var detail = await GetDetail(input);
            var companyInfo = await _tenantManager.GetAsync(AbpSession.GetTenantId());

            var accountCyclePeriod = await GetPreviousRoundingCloseCyleAsync(detail.Date);
            return await Task.Run(async () =>
            {

                var exportHtml = string.Empty;
                var templateHtml = string.Empty;
                FileDto fileDto = new FileDto()
                {
                    SubFolder = null,
                    FileName = "InvoicePrint.pdf",
                    FileToken = "POSInvoicePrint.html",
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
                exportHtml = templateHtml;
                var dateFormat = FormatDate(detail.Date, "dd/MM/yyyy hh:mm tt");//@todo take date format from tenant setup              
                var contentBody = string.Empty;

                foreach (var i in detail.InvoiceItems)
                {

                    string formatQty = this.AutoFormaDigits(i.Qty.ToString("N6"));

                    var row = $"<tr class=\"padding\">";
                    row += $"<td>{ i.Item.ItemName }</td>";
                    row += $"<td class=\"text-center\">{ formatQty }</td>";
                    row += $"<td class=\"text-right\">{ FormatNumberCurrency(i.MultiCurrencyUnitCost, 2) }</td>";
                    row += $"<td class=\"text-right\">{ FormatNumberCurrency(i.MultiCurrencyTotal, 2) }</td>";
                    row += "</tr>";
                    contentBody += row;
                }
                var length = detail.InvoiceItems.Count.ToString();
                int lastNumber = Int16.Parse(length.Substring(length.Length - 1, 1));

                exportHtml = exportHtml.Replace("{{CompanyName}}", companyInfo.Name);
                exportHtml = exportHtml.Replace("{{CompanyPhone}}", companyInfo.PhoneNumber);
                exportHtml = exportHtml.Replace("{{OrderList}}", L("OrderList"));
                exportHtml = exportHtml.Replace("{{LocationName}}", detail.Location.LocationName);

                exportHtml = exportHtml.Replace("{{InvoiceDate}}", dateFormat);
                exportHtml = exportHtml.Replace("{{UserName}}", detail.CreatorUser);
                exportHtml = exportHtml.Replace("{{InvoiceNo}}", detail.InvoiceNo);
                exportHtml = exportHtml.Replace("{{CurrencyCode}}", detail.CurrencyCode);
                exportHtml = exportHtml.Replace("{{RowItems}}", contentBody);
                exportHtml = exportHtml.Replace("{{SubTotal}}", FormatNumberCurrency(detail.MulticurrencySubTotal, 2));
                exportHtml = exportHtml.Replace("{{Tax}}", FormatNumberCurrency(detail.MulticurrencyTax, 2));
                exportHtml = exportHtml.Replace("{{Total}}", FormatNumberCurrency(detail.MultiCurrencyTotal, 2));
                exportHtml = exportHtml.Replace("{{Charge}}", FormatNumberCurrency(detail.PaymentSummaries.Sum(s => s.MultiCurrencyTotal), 2));
                HtmlToPdfConverter htmlToPdfConverter = GetInitPDFOption();
                // Set if the fonts are embedded in the generated PDF document
                //Leave it not set to embed the fonts in the generated PDF document
                htmlToPdfConverter.PdfDocumentOptions.EmbedFonts = true;
                htmlToPdfConverter.PdfDocumentOptions.FitWidth = true;
                htmlToPdfConverter.PdfDocumentOptions.TableHeaderRepeatEnabled = true;
                htmlToPdfConverter.PdfDocumentOptions.TableFooterRepeatEnabled = false;
                htmlToPdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Portrait;
                htmlToPdfConverter.ClipHtmlView = true;
                htmlToPdfConverter.HtmlViewerWidth = 302;
                htmlToPdfConverter.PdfDocumentOptions.PdfPageSize.Width = 230.0f;
                //htmlToPdfConverter.PdfDocumentOptions.PdfPageSize.Height = 200.0f;
                htmlToPdfConverter.PdfDocumentOptions.LeftMargin = 3;
                htmlToPdfConverter.PdfDocumentOptions.RightMargin = 3;
                htmlToPdfConverter.PdfDocumentOptions.EmbedFonts = true;

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

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_POS_Create)]
        public async Task<NullableIdDto<Guid>> Create(CreatePOSInput input)
        {
            var result = await SaveInvoiceHelper(input);
            return new NullableIdDto<Guid>() { Id = result.Id };
        }

        protected async Task<NullableIdDto<Guid>> CreatePaymentHelper(POSPaymentSummaryByPaymentMethodInput payment, CreatePOSInput posInput, Invoice invoice)
        {
            var input = new CreateReceivePaymentInput
            {
                ReceiveFrom = ReceiveFromRecievePayment.Cash,
                Status = TransactionStatus.Publish,
                FiFo = false,
                ClassId = posInput.ClassId,
                LocationId = posInput.LocationId,
                Change = 0,

                CurrencyId = posInput.CurrencyId,
                IsConfirm = posInput.IsConfirm,
                Memo = string.Empty,
                PaymentNo = null,
                Reference = null,
                PaymentAccountId = payment.AccountId,

                MultiCurrencyChange = 0,
                MultiCurrencyId = posInput.MultiCurrencyId,
                MultiCurrencyTotalPayment = payment.MultiCurrencyTotal,
                MultiCurrencyTotalPaymentCustomerCredit = 0,
                MultiCurrencyTotalPaymentInvoice = payment.MultiCurrencyTotal,


                TotalPayment = payment.Total,
                TotalPaymentInvoice = payment.Total,
                TotalOpenBalanceCustomerCredit = 0,
                TotalOpenBalance = invoice.OpenBalance,
                TotalPaymentCustomerCredit = 0,
                TotalPaymentDue = invoice.OpenBalance - payment.Total,
                TotalPaymentDueCustomerCredit = 0,

                paymentDate = posInput.Date,
                ReceivePaymentExpenseItems = null,

                TotalCashInvoice = payment.Total,                
                TotalCreditInvoice = 0,
                TotalExpenseInvoice = 0,
                TotalCashCustomerCredit = 0,
                TotalCreditCustomerCredit = 0,
                TotalExpenseCustomerCredit = 0,


                PaymentMethodId = payment.PaymentMethodId,

                ReceivePaymentDetail = new List<ReceivePayments.Dto.ReceivePaymentDetail>()
                {
                   new ReceivePayments.Dto.ReceivePaymentDetail{
                        InvoiceId = invoice.Id,
                        CustomerId = invoice.CustomerId,
                        DueDate = invoice.DueDate,
                        accountId = posInput.AccountId,
                        CustomerCreditId = null,
                        Payment = payment.Total,
                        MultiCurrencyPayment = payment.MultiCurrencyTotal,
                        OpenBalance = invoice.OpenBalance,
                        MultiCurrencyOpenBalance = invoice.MultiCurrencyOpenBalance,
                        TotalAmount = invoice.OpenBalance - payment.Total,
                        MultiCurrencyTotalAmount = invoice.MultiCurrencyOpenBalance - payment.MultiCurrencyTotal,
                        Cash = payment.Total,
                        MultiCurrencyCash = payment.MultiCurrencyTotal,
                   }
                }
            };


            if (input.LocationId == null || input.LocationId == 0)
            {
                throw new UserFriendlyException(L("PleaseSelectLocation"));
            }

            var lossGainAmount = invoice.OpenBalance - payment.Total;

            //Check for Exchange Loss Gain
            if (invoice.MultiCurrencyOpenBalance == payment.MultiCurrencyTotal && lossGainAmount != 0)
            {
                var tenant = await GetCurrentTenantAsync();
                var invoiceToPay = input.ReceivePaymentDetail.FirstOrDefault();

                invoiceToPay.Payment += lossGainAmount;
                invoiceToPay.TotalAmount -= lossGainAmount;

                input.TotalPaymentDue -= lossGainAmount;
                input.TotalPaymentInvoice += lossGainAmount;

                input.Change = lossGainAmount;
                input.ReceivePaymentExpenseItems = new List<ReceivePayments.Dto.ReceivePaymentExpenseItem>();

                //Posting Exchange Loss Gain Account
                input.ReceivePaymentExpenseItems.Add(new ReceivePaymentExpenseItem
                {
                    AccountId = tenant.ExchangeLossAndGainId.Value,
                    Amount = lossGainAmount,
                    MultiCurrencyAmount = 0,
                    Description = "Exchange Loss/Gain",
                });

            }


            if (input.Change == 0)
            {
                input.ReceivePaymentExpenseItems = new List<ReceivePayments.Dto.ReceivePaymentExpenseItem>();
            }
            else
            {
                var totalAmountCurrency = input.ReceivePaymentExpenseItems.Sum(t => t.Amount);
                var totalAmountMultiCurrency = input.ReceivePaymentExpenseItems.Sum(t => t.MultiCurrencyAmount);
                if (input.Change != totalAmountCurrency || input.MultiCurrencyChange != totalAmountMultiCurrency)
                {
                    throw new UserFriendlyException(L("YouMustAddAccountOfChange"));
                }
            }


            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.RecievePayment);

            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.PaymentNo = newAuto;
                input.Reference = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            //insert to journal
            var @entity = Journal.Create(tenantId, userId, input.PaymentNo, input.paymentDate, input.Memo, input.TotalPayment, input.TotalPayment, input.CurrencyId, input.ClassId, input.Reference, input.LocationId);
            entity.UpdateStatus(input.Status);
            var isMultiCurrency = await _multiCurrencyRepository.GetAll().ToListAsync();
            if (input.MultiCurrencyId == null || input.MultiCurrencyId == 0 || !isMultiCurrency.Any())
            {
                input.MultiCurrencyId = input.CurrencyId;
            }
            else if (input.CurrencyId == input.MultiCurrencyId)
            {
                input.MultiCurrencyChange = input.Change;
                input.MultiCurrencyTotalPayment = input.TotalPayment;
            }

            entity.UpdateMultiCurrency(input.MultiCurrencyId);

            if (input.ReceiveFrom == ReceiveFromRecievePayment.Cash)
            {
                if (input.TotalPayment == 0)
                {
                    throw new UserFriendlyException(L("YouMustHavePayment"));
                }
                if (input.PaymentAccountId == null || input.PaymentAccountId == Guid.Empty)
                {
                    throw new UserFriendlyException(L("PaymentAccountIsRequired"));
                }
                //insert clearance journal item into credit
                var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity,
                                                    input.PaymentAccountId.Value,
                                                    input.Memo, input.TotalPayment, 0,
                                                    PostingKey.Payment, null);
                if (input.TotalPayment < 0)
                {
                    clearanceJournalItem.SetDebitCredit(0, input.TotalPayment * -1);
                }
                CheckErrors(await _journalItemManager.CreateAsync(clearanceJournalItem));
            }

            //insert to Receivepayment
            var receivePayment = ReceivePayment.Create(tenantId, userId, input.FiFo, input.TotalOpenBalance,
                                input.TotalPayment, input.TotalPaymentDue, input.ReceiveFrom,
                                input.MultiCurrencyTotalPayment, input.Change, input.TotalOpenBalanceCustomerCredit,
                                input.TotalPaymentCustomerCredit, input.TotalPaymentDueCustomerCredit,
                                input.MultiCurrencyTotalPaymentCustomerCredit, input.TotalPaymentInvoice, input.MultiCurrencyTotalPaymentInvoice,
                                input.MultiCurrencyChange,
                                input.TotalCashInvoice,
                                input.TotalCreditInvoice,
                                input.TotalExpenseInvoice,
                                input.TotalCashCustomerCredit,
                                input.TotalCreditCustomerCredit,
                                input.TotalExpenseCustomerCredit, 
                                input.UseExchangeRate,
                                input.MultiCurrencyTotalOpenBalance,
                                input.MultiCurrencyTotalOpenBalanceCustomerCredit,
                                input.MultiCurrencyTotalPaymentDue,
                                input.MultiCurrencyTotalPaymentDueCustomerCredit,
                                input.MultiCurrencyTotalCashInvoice,
                                input.MultiCurrencyTotalCreditInvoice,
                                input.MultiCurrencyTotalExpenseInvoice,
                                input.MultiCurrencyTotalCashCustomerCredit,
                                input.MultiCurrencyTotalCreditCustomerCredit,
                                input.MultiCurrencyTotalExpenseCustomerCredit);

            if (input.PaymentMethodId.HasValue) receivePayment.UpdatePaymentMethodId(input.PaymentMethodId);

            // update journal type status to Receivepayment
            entity.UpdateReceivePayment(receivePayment);


            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));
            CheckErrors(await _receivePaymentManager.CreateAsync(receivePayment));

            var @invoices = new List<Invoice>();
            var @customerCredits = new List<CustomerCredit>();
            if (input.ReceivePaymentDetail.Where(x => x.InvoiceId != null).Count() > 0)
            {
                @invoices = await _invoiceRepository.GetAll().Where(x => input.ReceivePaymentDetail.Any(t => t.InvoiceId == x.Id)).ToListAsync();
            }
            if (input.ReceivePaymentDetail.Where(x => x.CustomerCreditId != null).Count() > 0)
            {
                @customerCredits = await _customerCreditRepository.GetAll().Where(x => input.ReceivePaymentDetail.Any(t => t.CustomerCreditId == x.Id)).ToListAsync();
            }
            // loop receive Payment detail
            foreach (var i in input.ReceivePaymentDetail)
            {
                if (input.MultiCurrencyId == null || input.MultiCurrencyId == 0 || !isMultiCurrency.Any())
                {
                    i.MultiCurrencyAmountToBeSubstract = i.AmountToBeSubstract;
                    i.MultiCurrencyOpenBalance = i.OpenBalance;
                    i.MultiCurrencyPayment = i.Payment;
                    i.MultiCurrencyTotalAmount = i.TotalAmount;
                }

                //insert to receivePayment detail
                var receivePaymentDetail = ReceivePayments.ReceivePaymentDetail.Create(tenantId, userId, receivePayment,
                                            i.InvoiceId, i.CustomerId, i.DueDate, i.OpenBalance, i.Payment,
                                            i.TotalAmount, i.MultiCurrencyOpenBalance, i.MultiCurrencyPayment,
                                            i.MultiCurrencyTotalAmount, i.CustomerCreditId, 
                                            i.Cash, i.MultiCurrencyCash, i.Credit, i.MultiCurrencyCredit, i.Expense, i.MultiCurrencyExpense, 
                                            i.LossGain, i.OpenBalanceInPaymentCurrency, i.PaymentInPaymentCurrency, i.TotalAmountInPaymentCurrency,
                                            i.CashInPaymentCurrency, i.CreditInPaymentCurrency, i.ExpenseInPaymentCurrency);
                CheckErrors(await _receivePaymentDetailManager.CreateAsync(receivePaymentDetail));

                //insert journal item into with credit  and default debit zero
                JournalItem inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity,
                                        i.accountId, "", 0, i.Payment, PostingKey.AR,
                                        receivePaymentDetail.Id);

                // update total balance in invoice 
                var updateTotalbalance = @invoices.Where(u => u.Id == receivePaymentDetail.InvoiceId).FirstOrDefault();
                updateTotalbalance.UpdateOpenBalance(i.Payment * -1);
                updateTotalbalance.UpdateMultiCurrencyOpenBalance(i.MultiCurrencyPayment * -1);
                if (input.Status == TransactionStatus.Publish)
                {
                    if (i.Payment > 0)
                    {
                        updateTotalbalance.UpdateTotalPaid(i.Payment);
                        updateTotalbalance.UpdateMultiCurrencyTotalPaid(i.MultiCurrencyPayment);
                        if (updateTotalbalance.OpenBalance == 0 || updateTotalbalance.Total == updateTotalbalance.TotalPaid)
                        {
                            updateTotalbalance.UpdatePaidStatus(PaidStatuse.Paid);
                        }
                        else
                        {
                            updateTotalbalance.UpdatePaidStatus(PaidStatuse.Partial);
                        }
                    }
                    CheckErrors(await _invoiceManager.UpdateAsync(updateTotalbalance));
                }

                CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));
            }
            // loop of paybill expense account detail
            if (input.ReceiveFrom == ReceiveFromRecievePayment.Cash && input.ReceivePaymentExpenseItems != null)
            {
                foreach (var i in input.ReceivePaymentExpenseItems)
                {
                    if (input.MultiCurrencyId == null || input.MultiCurrencyId == 0 || !isMultiCurrency.Any())
                    {
                        i.MultiCurrencyAmount = i.Amount;
                    }
                    //insert to pay bill detail
                    var receiveExpense = ReceivePaymentExpense.Create(tenantId, userId, receivePayment,
                                            i.AccountId, i.Amount, i.MultiCurrencyAmount, i.Description, i.IsLossGain);
                    CheckErrors(await _receivePaymentExpenseManager.CreateAsync(receiveExpense));

                    JournalItem journalItem;
                    if (i.Amount > 0)
                    {
                        journalItem = JournalItem.CreateJournalItem(
                                                    tenantId, userId, entity, i.AccountId,
                                                    "", i.Amount, 0, PostingKey.PaymentChange,
                                                    receiveExpense.Id);
                    }
                    else
                    {
                        journalItem = JournalItem.CreateJournalItem(
                                                    tenantId, userId, entity, i.AccountId,
                                                    "", 0, i.Amount * -1, PostingKey.PaymentChange,
                                                    receiveExpense.Id);
                    }
                    CheckErrors(await _journalItemManager.CreateAsync(journalItem));
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();
            return new NullableIdDto<Guid>() { Id = receivePayment.Id };
        }

        protected async Task<NullableIdDto<Guid>> ApplyStoreCredit(POSStoreCreditPaymentInput payment, CreatePOSInput posInput, Invoice invoice)
        {
            var customerCredit = await _customerCreditRepository.FirstOrDefaultAsync(s => s.Id == payment.StoreCreditId);
            if (customerCredit == null) throw new UserFriendlyException(L("RecordNotFound"));

            var input = new CreateReceivePaymentInput
            {
                ReceiveFrom = ReceiveFromRecievePayment.CustomerCredit,
                Status = TransactionStatus.Publish,
                FiFo = false,
                ClassId = posInput.ClassId,
                LocationId = posInput.LocationId,
                IsConfirm = posInput.IsConfirm,
                paymentDate = posInput.Date,
                Memo = string.Empty,
                PaymentNo = null,
                Reference = null,
                PaymentAccountId = null,
                PaymentMethodId = null,
                CurrencyId = posInput.CurrencyId,
                MultiCurrencyId = posInput.MultiCurrencyId,

                Change = 0,
                MultiCurrencyChange = 0,

                TotalPayment = 0,
                MultiCurrencyTotalPayment = 0,

                TotalPaymentCustomerCredit = payment.Total,
                MultiCurrencyTotalPaymentCustomerCredit = payment.MultiCurrencyTotal,

                TotalPaymentInvoice = payment.Total,
                MultiCurrencyTotalPaymentInvoice = payment.MultiCurrencyTotal,

                TotalOpenBalance = invoice.OpenBalance,
                TotalOpenBalanceCustomerCredit = customerCredit.OpenBalance,

                TotalPaymentDue = invoice.OpenBalance - payment.Total,
                TotalPaymentDueCustomerCredit = customerCredit.OpenBalance - payment.Total,

                ReceivePaymentExpenseItems = null,

                TotalCashInvoice = 0,
                TotalCreditInvoice = payment.Total,
                TotalExpenseInvoice = 0,
                TotalCashCustomerCredit = 0,
                TotalCreditCustomerCredit = payment.Total,
                TotalExpenseCustomerCredit = 0,


                ReceivePaymentDetail = new List<ReceivePayments.Dto.ReceivePaymentDetail>()
                {
                    new ReceivePayments.Dto.ReceivePaymentDetail {
                        InvoiceId = null,
                        CustomerId = customerCredit.CustomerId,
                        DueDate = customerCredit.DueDate,
                        accountId = payment.AccountId,
                        CustomerCreditId = payment.StoreCreditId,
                        Payment = payment.Total,
                        MultiCurrencyPayment = payment.MultiCurrencyTotal,
                        OpenBalance = customerCredit.OpenBalance,
                        MultiCurrencyOpenBalance = customerCredit.MultiCurrencyOpenBalance,
                        TotalAmount = customerCredit.OpenBalance - payment.Total,
                        MultiCurrencyTotalAmount = customerCredit.MultiCurrencyOpenBalance - payment.MultiCurrencyTotal,
                        Credit = payment.Total,
                        MultiCurrencyCredit = payment.MultiCurrencyTotal,
                    },
                    new ReceivePayments.Dto.ReceivePaymentDetail {
                        InvoiceId = invoice.Id,
                        CustomerId = invoice.CustomerId,
                        DueDate = invoice.DueDate,
                        accountId = posInput.AccountId,
                        CustomerCreditId = null,
                        Payment = payment.Total,
                        MultiCurrencyPayment = payment.MultiCurrencyTotal,
                        OpenBalance = invoice.OpenBalance,
                        MultiCurrencyOpenBalance = invoice.MultiCurrencyOpenBalance,
                        TotalAmount = invoice.OpenBalance - payment.Total,
                        MultiCurrencyTotalAmount = invoice.MultiCurrencyOpenBalance - payment.MultiCurrencyTotal,
                        Credit = payment.Total,
                        MultiCurrencyCredit = payment.MultiCurrencyTotal,
                    }
                }
            };


            if (input.LocationId == null || input.LocationId == 0)
            {
                throw new UserFriendlyException(L("PleaseSelectLocation"));
            }



            //Exchange Loss Gain 
            decimal invoiceLossGainAmount = invoice.MultiCurrencyOpenBalance == payment.MultiCurrencyTotal ? invoice.OpenBalance - payment.Total : 0;
            var customerCreditLossGainAmount = customerCredit.MultiCurrencyOpenBalance == payment.MultiCurrencyTotal ? customerCredit.OpenBalance - payment.Total : 0;

            //Check for Exchange Loss Gain
            if (invoiceLossGainAmount != 0 || customerCreditLossGainAmount != 0)
            {
                var tenant = await GetCurrentTenantAsync();

                if (invoiceLossGainAmount != 0)
                {
                    var invoiceToPay = input.ReceivePaymentDetail.FirstOrDefault(s => s.InvoiceId != null);
                    invoiceToPay.Payment += invoiceLossGainAmount;
                    invoiceToPay.TotalAmount -= invoiceLossGainAmount;

                    input.TotalPaymentInvoice += invoiceLossGainAmount;                
                    input.TotalPaymentDue -= invoiceLossGainAmount;
                }

                if(customerCreditLossGainAmount != 0)
                {
                    var customerCreditToPay = input.ReceivePaymentDetail.FirstOrDefault(s => s.CustomerCreditId != null);
                    customerCreditToPay.Payment += customerCreditLossGainAmount;
                    customerCreditToPay.TotalAmount -= customerCreditLossGainAmount;

                    input.TotalPaymentCustomerCredit += customerCreditLossGainAmount;                    
                    input.TotalPaymentDueCustomerCredit -= customerCreditLossGainAmount;
                }

                input.TotalPayment = invoiceLossGainAmount - customerCreditLossGainAmount;

                if(input.TotalPayment != 0)
                {
                    input.PaymentAccountId = tenant.ExchangeLossAndGainId.Value;
                    input.Memo = "Exchange Loss/Gain : " + input.TotalPayment;
                    input.ReceiveFrom = ReceiveFromRecievePayment.Cash;
                }                
            }


            if (input.Change == 0)
            {
                input.ReceivePaymentExpenseItems = new List<ReceivePayments.Dto.ReceivePaymentExpenseItem>();
            }
            else
            {
                var totalAmountCurrency = input.ReceivePaymentExpenseItems.Sum(t => t.Amount);
                var totalAmountMultiCurrency = input.ReceivePaymentExpenseItems.Sum(t => t.MultiCurrencyAmount);
                if (input.Change != totalAmountCurrency || input.MultiCurrencyChange != totalAmountMultiCurrency)
                {
                    throw new UserFriendlyException(L("YouMustAddAccountOfChange"));
                }
            }

            if (input.ReceiveFrom == ReceiveFromRecievePayment.CustomerCredit)
            {
                //input.TotalPayment = 0;
                //input.MultiCurrencyTotalPayment = 0;
                //input.Change = 0;
                //input.MultiCurrencyChange = 0;
                if (input.TotalPaymentInvoice != input.TotalPaymentCustomerCredit
                    && input.MultiCurrencyTotalPaymentInvoice != input.MultiCurrencyTotalPaymentCustomerCredit)
                {
                    throw new UserFriendlyException(L("TotalInvoicePaymentMustEqualCustomerCreditPayment"));
                }
            }


            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.RecievePayment);

            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.PaymentNo = newAuto;
                input.Reference = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            //insert to journal
            var @entity = Journal.Create(tenantId, userId, input.PaymentNo, input.paymentDate, input.Memo, input.TotalPayment, input.TotalPayment, input.CurrencyId, input.ClassId, input.Reference, input.LocationId);
            entity.UpdateStatus(input.Status);
            var isMultiCurrency = await _multiCurrencyRepository.GetAll().ToListAsync();
            if (input.MultiCurrencyId == null || input.MultiCurrencyId == 0 || !isMultiCurrency.Any())
            {
                input.MultiCurrencyId = input.CurrencyId;
            }
            else if (input.CurrencyId == input.MultiCurrencyId)
            {
                input.MultiCurrencyChange = input.Change;
                input.MultiCurrencyTotalPayment = input.TotalPayment;
            }

            entity.UpdateMultiCurrency(input.MultiCurrencyId);

            if (input.ReceiveFrom == ReceiveFromRecievePayment.Cash)
            {
                if (input.TotalPayment == 0)
                {
                    throw new UserFriendlyException(L("YouMustHavePayment"));
                }
                if (input.PaymentAccountId == null || input.PaymentAccountId == Guid.Empty)
                {
                    throw new UserFriendlyException(L("PaymentAccountIsRequired"));
                }
                //insert clearance journal item into credit
                var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity,
                                                    input.PaymentAccountId.Value,
                                                    input.Memo, input.TotalPayment, 0,
                                                    PostingKey.Payment, null);
                if (input.TotalPayment < 0)
                {
                    clearanceJournalItem.SetDebitCredit(0, input.TotalPayment * -1);
                }
                CheckErrors(await _journalItemManager.CreateAsync(clearanceJournalItem));
            }

            //insert to Receivepayment
            var receivePayment = ReceivePayment.Create(tenantId, userId, input.FiFo, input.TotalOpenBalance,
                                input.TotalPayment, input.TotalPaymentDue, input.ReceiveFrom,
                                input.MultiCurrencyTotalPayment, input.Change, input.TotalOpenBalanceCustomerCredit,
                                input.TotalPaymentCustomerCredit, input.TotalPaymentDueCustomerCredit,
                                input.MultiCurrencyTotalPaymentCustomerCredit, input.TotalPaymentInvoice, input.MultiCurrencyTotalPaymentInvoice,
                                input.MultiCurrencyChange,
                                input.TotalCashInvoice,
                                input.TotalCreditInvoice,
                                input.TotalExpenseInvoice,
                                input.TotalCashCustomerCredit,
                                input.TotalCreditCustomerCredit,
                                input.TotalExpenseCustomerCredit,
                                input.UseExchangeRate,
                                input.MultiCurrencyTotalOpenBalance,
                                input.MultiCurrencyTotalOpenBalanceCustomerCredit,
                                input.MultiCurrencyTotalPaymentDue,
                                input.MultiCurrencyTotalPaymentDueCustomerCredit,
                                input.MultiCurrencyTotalCashInvoice,
                                input.MultiCurrencyTotalCreditInvoice,
                                input.MultiCurrencyTotalExpenseInvoice,
                                input.MultiCurrencyTotalCashCustomerCredit,
                                input.MultiCurrencyTotalCreditCustomerCredit,
                                input.MultiCurrencyTotalExpenseCustomerCredit);

            if (input.PaymentMethodId.HasValue) receivePayment.UpdatePaymentMethodId(input.PaymentMethodId);

            // update journal type status to Receivepayment
            entity.UpdateReceivePayment(receivePayment);


            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));
            CheckErrors(await _receivePaymentManager.CreateAsync(receivePayment));

            var @invoices = new List<Invoice>();
            var @customerCredits = new List<CustomerCredit>();
            if (input.ReceivePaymentDetail.Where(x => x.InvoiceId != null).Count() > 0)
            {
                @invoices = await _invoiceRepository.GetAll().Where(x => input.ReceivePaymentDetail.Any(t => t.InvoiceId == x.Id)).ToListAsync();
            }
            if (input.ReceivePaymentDetail.Where(x => x.CustomerCreditId != null).Count() > 0)
            {
                @customerCredits = await _customerCreditRepository.GetAll().Where(x => input.ReceivePaymentDetail.Any(t => t.CustomerCreditId == x.Id)).ToListAsync();
            }
            // loop receive Payment detail
            foreach (var i in input.ReceivePaymentDetail)
            {
                if (input.MultiCurrencyId == null || input.MultiCurrencyId == 0 || !isMultiCurrency.Any())
                {
                    i.MultiCurrencyAmountToBeSubstract = i.AmountToBeSubstract;
                    i.MultiCurrencyOpenBalance = i.OpenBalance;
                    i.MultiCurrencyPayment = i.Payment;
                    i.MultiCurrencyTotalAmount = i.TotalAmount;
                }

                //insert to receivePayment detail
                var receivePaymentDetail = ReceivePayments.ReceivePaymentDetail.Create(tenantId, userId, receivePayment,
                                            i.InvoiceId, i.CustomerId, i.DueDate, i.OpenBalance, i.Payment,
                                            i.TotalAmount, i.MultiCurrencyOpenBalance, i.MultiCurrencyPayment,
                                            i.MultiCurrencyTotalAmount, i.CustomerCreditId, 
                                            i.Cash, i.MultiCurrencyCash, i.Credit, i.MultiCurrencyCredit, i.Expense, i.MultiCurrencyExpense, 
                                            i.LossGain, i.OpenBalanceInPaymentCurrency, i.PaymentInPaymentCurrency, i.TotalAmountInPaymentCurrency,
                                            i.CashInPaymentCurrency, i.CreditInPaymentCurrency, i.ExpenseInPaymentCurrency);
                CheckErrors(await _receivePaymentDetailManager.CreateAsync(receivePaymentDetail));


                JournalItem inventoryJournalItem;

                if (i.InvoiceId != null)
                {
                    //insert journal item into with credit  and default debit zero
                    inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity,
                                            i.accountId, "", 0, i.Payment, PostingKey.AR,
                                            receivePaymentDetail.Id);

                    // update total balance in invoice 
                    var updateTotalbalance = @invoices.Where(u => u.Id == receivePaymentDetail.InvoiceId).FirstOrDefault();
                    updateTotalbalance.UpdateOpenBalance(i.Payment * -1);
                    updateTotalbalance.UpdateMultiCurrencyOpenBalance(i.MultiCurrencyPayment * -1);
                    if (input.Status == TransactionStatus.Publish)
                    {
                        if (i.Payment > 0)
                        {
                            updateTotalbalance.UpdateTotalPaid(i.Payment);
                            updateTotalbalance.UpdateMultiCurrencyTotalPaid(i.MultiCurrencyPayment);
                            //if (updateTotalbalance.OpenBalance == 0)
                            if (updateTotalbalance.OpenBalance == 0 || updateTotalbalance.Total == updateTotalbalance.TotalPaid)
                            {
                                updateTotalbalance.UpdatePaidStatus(PaidStatuse.Paid);
                            }
                            else
                            {
                                updateTotalbalance.UpdatePaidStatus(PaidStatuse.Partial);
                            }
                        }
                        CheckErrors(await _invoiceManager.UpdateAsync(updateTotalbalance));
                    }
                }
                else
                {
                    //insert journal item into with credit  and default debit zero
                    inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity,
                                            i.accountId, "", i.Payment, 0, PostingKey.Clearance,
                                            receivePaymentDetail.Id);
                    var customerCreditOpenBalance = @customerCredits.Where(u => u.Id == i.CustomerCreditId).FirstOrDefault();

                    customerCreditOpenBalance.IncreaseOpenbalance(i.Payment * -1);
                    customerCreditOpenBalance.IncreaseMultiCurrencyOpenBalance(i.MultiCurrencyPayment * -1);
                    if (input.Status == TransactionStatus.Publish)
                    {
                        customerCreditOpenBalance.IncreaseTotalPaid(i.Payment);
                        customerCreditOpenBalance.IncreaseMultiCurrencyTotalPaid(i.MultiCurrencyPayment);
                        if (customerCreditOpenBalance.OpenBalance == 0 || customerCreditOpenBalance.Total == customerCreditOpenBalance.TotalPaid)
                        {
                            customerCreditOpenBalance.UpdateToPaid();
                        }
                        else
                        {
                            customerCreditOpenBalance.UpdateToPartial();
                        }
                    }
                    CheckErrors(await _customerCreditManager.UpdateAsync(customerCreditOpenBalance));

                }

                CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));
            }
            // loop of paybill expense account detail
            if (input.ReceiveFrom == ReceiveFromRecievePayment.Cash && input.ReceivePaymentExpenseItems != null)
            {
                foreach (var i in input.ReceivePaymentExpenseItems)
                {
                    if (input.MultiCurrencyId == null || input.MultiCurrencyId == 0 || !isMultiCurrency.Any())
                    {
                        i.MultiCurrencyAmount = i.Amount;
                    }
                    //insert to pay bill detail
                    var receiveExpense = ReceivePaymentExpense.Create(tenantId, userId, receivePayment,
                                            i.AccountId, i.Amount, i.MultiCurrencyAmount, i.Description, i.IsLossGain);
                    CheckErrors(await _receivePaymentExpenseManager.CreateAsync(receiveExpense));

                    JournalItem journalItem;
                    if (i.Amount > 0)
                    {
                        journalItem = JournalItem.CreateJournalItem(
                                                    tenantId, userId, entity, i.AccountId,
                                                    "", i.Amount, 0, PostingKey.PaymentChange,
                                                    receiveExpense.Id);
                    }
                    else
                    {
                        journalItem = JournalItem.CreateJournalItem(
                                                    tenantId, userId, entity, i.AccountId,
                                                    "", 0, i.Amount * -1, PostingKey.PaymentChange,
                                                    receiveExpense.Id);
                    }
                    CheckErrors(await _journalItemManager.CreateAsync(journalItem));
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();
            return new NullableIdDto<Guid>() { Id = receivePayment.Id };
        }
        protected async Task<NullableIdDto<Guid>> SaveInvoiceHelper(CreatePOSInput posInput)
        {
            //validate invoice Item when create by none
            if (posInput.InvoiceItems.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }


            if (posInput.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                                       .Where(t => (t.LockKey == TransactionLockType.Invoice)
                                       && t.IsLock == true && t.LockDate != null && t.LockDate.Value.Date >= posInput.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            await ValidateBatchNo(posInput);

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            #region update auto sequence
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.POS);
            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                posInput.InvoiceNo = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }
            #endregion


            var input = new CreateInvoiceInput()
            {
                CurrencyId = posInput.CurrencyId,
                CustomerId = posInput.CustomerId,
                LocationId = posInput.LocationId,
                Date = posInput.Date,
                DueDate = posInput.Date,
                ETD = posInput.Date,
                ReceiveFrom = ReceiveFrom.None,
                IsConfirm = posInput.IsConfirm,
                AccountId = posInput.AccountId,
                ClassId = posInput.ClassId,
                ConvertToItemIssue = true,
                IssuingDate = posInput.Date,
                InvoiceItems = posInput.InvoiceItems,
                InvoiceNo = posInput.InvoiceNo,
                Reference = posInput.InvoiceNo,
                Memo = string.Empty,

                MultiCurrencyId = posInput.MultiCurrencyId,
                MultiCurrencySubTotal = posInput.MultiCurrencySubTotal,
                MultiCurrencyTax = posInput.MultiCurrencyTax,
                MultiCurrencyTotal = posInput.MultiCurrencyTotal,

                SubTotal = posInput.SubTotal,
                Tax = posInput.Tax,
                Total = posInput.Total,
                TransactionTypeId = null,
                ItemIssueId = null,
                Status = TransactionStatus.Publish,
                SameAsShippingAddress = true,
                ShippingAddress = null,
                BillingAddress = new CAddress("", "", "", "", ""),
            };


            #region insert journal for invoice

            var @entity = Journal.Create(tenantId, userId, input.InvoiceNo, input.Date, input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, input.LocationId);
            entity.UpdateStatus(input.Status);

            entity.UpdateMultiCurrency(input.MultiCurrencyId);
            //insert journal item @todo change after metting requirement
            var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, input.AccountId, input.Memo, input.Total, 0, PostingKey.AR, null);

            //insert to invoice
            var saleType = await _saleTypeRepository.GetAll().Where(t => t.Id == input.TransactionTypeId || t.IsPOS == true).FirstOrDefaultAsync();
            if (saleType == null)
            {
                throw new UserFriendlyException(L("PleaseCreateSaleTypePOS"));
            }

            var isItem = input.InvoiceItems.Any(s => s.ItemId != null && s.ItemId != Guid.Empty);

            var invoice = Invoice.Create(tenantId, userId,
                                    input.ReceiveFrom, input.DueDate, input.CustomerId,
                                    input.SameAsShippingAddress,
                                    input.ShippingAddress, input.BillingAddress,
                                    input.SubTotal, input.Tax, input.Total,
                                    input.ItemIssueId, input.ETD, input.IssuingDate, input.ConvertToItemIssue,
                                    input.MultiCurrencySubTotal, input.MultiCurrencyTax, input.MultiCurrencyTotal, isItem, false);

            input.TransactionTypeId = saleType.Id;
            invoice.UpdateTransactionTypeId(input.TransactionTypeId);


            //comment invoice.SetIsPOS(true);
            saleType.SetIsPOS(true);
            CheckErrors(await _saleTypeManager.UpdateAsync(saleType));

            @entity.UpdateInvoice(invoice);

            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));
            CheckErrors(await _journalItemManager.CreateAsync(clearanceJournalItem));
            CheckErrors(await _invoiceManager.CreateAsync(invoice));

            #endregion

            int indexLot = 0;

            foreach (var i in input.InvoiceItems)
            {
                indexLot++;
                if (i.LotId == null && i.ItemId != null && i.Item.ItemTypeId != 3)
                {
                    throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + indexLot.ToString());
                }
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

                else if (input.ReceiveFrom == ReceiveFrom.SaleOrder && i.OrderItemId != null)
                {
                    if (i.Qty > i.OrginalQtyFromSaleOrder)
                    {
                        throw new UserFriendlyException(L("InvoiceMessageWarning", i.ItemName) + L("Row") + " " + indexLot.ToString());
                    }
                    invoiceItem.UpdateOrderItemId(i.OrderItemId);
                }
                invoiceItem.UpdateLot(i.LotId);
                CheckErrors(await _invoiceItemManager.CreateAsync(invoiceItem));

                //insert inventory journal item into debit
                var revenueJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, i.AccountId,
                                            i.Description, 0, i.Total, PostingKey.Revenue, invoiceItem.Id);
                CheckErrors(await _journalItemManager.CreateAsync(revenueJournalItem));

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
                                    Total = t.Total,
                                    UnitCost = t.UnitCost,
                                    SaleOrderId = t.SaleOrderId,

                                    SaleOrderItemId = t.SaleOrderItem != null ? t.SaleOrderItem.Id : Guid.Empty,
                                    PurchaseAccountId = t.Item.PurchaseAccountId.Value,
                                    Item = t.Item,
                                    InvoiceItemId = t.Id,
                                    ItemBatchNos = t.ItemBatchNos
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
                if (itemLists.Any()) await ItemIssueSave(itemIssueInput);

            }


            //apply store credit   
            if (posInput.StoreCreditPayments != null && posInput.StoreCreditPayments.Any())
            {
                foreach (var storeCredit in posInput.StoreCreditPayments)
                {
                    if (invoice.MultiCurrencyOpenBalance == 0) break;
                    await ApplyStoreCredit(storeCredit, posInput, invoice);
                }
            }


            //prepare payments      
            if (posInput.ReceivePayments != null && posInput.ReceivePayments.Any())
            {
                foreach (var payment in posInput.ReceivePayments)
                {
                    if (invoice.MultiCurrencyOpenBalance == 0) break;
                    await CreatePaymentHelper(payment, posInput, invoice);
                }

            }

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.Invoice };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
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
               .Where(u => u.InvoiceId == input.InvoiceId && u.OrderItemId != null
                           && u.SaleOrderItem.SaleOrderId == u.SaleOrderItem.SaleOrder.Id)
                           .Select(u => new InvoiceSaleOrderDto
                           {
                               orderItemId = u.OrderItemId,
                               soId = u.SaleOrderItem.SaleOrderId,
                               //sumTotalQty = u.SaleOrderItem.TotalIssueInvoiceQty + u.SaleOrderItem.IssueQty,
                               OriginalTotalQty = u.SaleOrderItem.Qty,
                               qty = u.Qty
                           }).ToListAsync();
            foreach (var i in input.Items)
            {
                //insert to item Issue item
                var @itemIssueItem = ItemIssueItem.Create(tenantId, userId,
                                        itemIssue, i.ItemId, i.Description,
                                        i.Qty, i.UnitCost,
                                        i.DiscountRate, i.Total);

                itemIssueItem.UpdateLot(i.LotId);
                if (input.ReceiveFrom != ReceiveFrom.None && i.SaleOrderItemId != Guid.Empty &&
                    i.SaleOrderItemId != null && invoiceItems.Count() == 0)
                {

                }

                if (invoiceItems.Count() > 0 && input.ReceiveFrom == ReceiveFrom.Invoice && i.InvoiceItemId != null)
                {
                    if (i.InvoiceItemId != null)
                    {
                        itemIssueItem.UpdateSaleOrderItemId(i.SaleOrderItemId);
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
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                var scheduleItems = input.Items.Select(s => s.ItemId).Distinct().ToList();
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, input.Date, scheduleItems);
            }

        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_POS_GetDetail)]
        public async Task<POSDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            var @journal = await _journalRepository
                                .GetAll()
                                .Include(u => u.Invoice.Customer)
                                .Include(u => u.Location)
                                .Include(u => u.CreatorUser)
                                .Include(u => u.Currency)
                                .Include(u => u.MultiCurrency)
                                .AsNoTracking()
                                .Where(u => u.JournalType == JournalType.Invoice && u.InvoiceId == input.Id)
                                .FirstOrDefaultAsync();

            if (@journal == null || @journal.Invoice == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var invoiceItems = await _invoiceItemRepository.GetAll()
                .Include(u => u.Tax)
                .Include(u => u.Item)
                .Where(u => u.InvoiceId == input.Id)
                .Select(s =>
                new CreateOrUpdateInvoiceItemInput()
                {
                    Id = s.Id,
                    Item = ObjectMapper.Map<ItemSummaryDetailOutput>(s.Item),
                    ItemId = s.ItemId,
                    Qty = s.Qty,
                    UnitCost = s.UnitCost,
                    Total = s.MultiCurrencyTotal,
                    MultiCurrencyTotal = s.MultiCurrencyTotal,
                    MultiCurrencyUnitCost = s.MultiCurrencyUnitCost,
                    UseBatchNo = s.Item != null && s.Item.UseBatchNo,
                    ItemIssueId = s.ItemIssueItemId
                })
                .OrderBy(u => u.CreationTime)
                .ToListAsync();



            var batchDic = await _itemIssueItemBatchNoRepository.GetAll()
                                    .AsNoTracking()
                                    .Where(s => invoiceItems.Any(r => r.ItemIssueId.Value == s.ItemIssueItemId))
                                    .Select(s => new BatchNoItemOutput
                                    {
                                        Id = s.Id,
                                        BatchNoId = s.BatchNoId,
                                        BatchNumber = s.BatchNo.Code,
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


            var result = ObjectMapper.Map<POSDetailOutput>(journal.Invoice);

            result.InvoiceItems = invoiceItems;

            var paymentSummaries = await (from p in _journalRepository.GetAll()
                                                .Include(s => s.ReceivePayment.PaymentMethod.PaymentMethodBase)
                                                .Where(s => s.JournalType == JournalType.ReceivePayment)
                                                .Where(s => s.Status == TransactionStatus.Publish)
                                                .AsNoTracking()
                                          join pi in _receivePaymentDetailRepository.GetAll()
                                              .Where(s => s.InvoiceId == input.Id)
                                              .AsNoTracking()
                                          on p.ReceivePaymentId equals pi.ReceivePaymentId
                                          select new
                                          {
                                              p.Date.Date,
                                              PaymentMethodName = p.ReceivePayment.PaymentMethod == null ? "Credit" : p.ReceivePayment.PaymentMethod.PaymentMethodBase.Name,
                                              Icon = p.ReceivePayment.PaymentMethod == null ? "icon-credit-card" : p.ReceivePayment.PaymentMethod.PaymentMethodBase.Icon,
                                              pi.Payment,
                                              pi.MultiCurrencyPayment,
                                          }
                                          into list
                                          group list by new
                                          {
                                              list.Date,
                                              list.PaymentMethodName,
                                              list.Icon
                                          }
                                          into g
                                          select new POSPaymentSummaryByPaymentMethodOutPut
                                          {
                                              Date = g.Key.Date,
                                              Icon = g.Key.Icon,
                                              PaymentMethod = g.Key.PaymentMethodName,
                                              Total = g.Sum(s => s.Payment),
                                              MultiCurrencyTotal = g.Sum(u => u.MultiCurrencyPayment),

                                          })
                            .ToListAsync();

            result.CurrencyCode = journal.MultiCurrency.Code;
            result.PaymentSummaries = paymentSummaries;
            result.Date = journal.Date;
            result.CreatorUser = journal.CreatorUser.FullName;
            result.PaidStatus = journal.Invoice.PaidStatus;
            result.PaidStatuseName = journal.Invoice.PaidStatus.ToString();
            result.InvoiceNo = journal.JournalNo;
            result.Charge = paymentSummaries.Sum(t => t.MultiCurrencyTotal);
            result.MultiCurrencyTotal = result.MultiCurrencyTotal;
            result.MulticurrencyTax = result.MulticurrencyTax;
            result.SubTotal = result.SubTotal;
            result.MulticurrencySubTotal = result.MulticurrencySubTotal;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            result.StatusName = journal.Status.ToString();
            result.StatusCode = journal.Status;
            result.CustomerId = journal.Invoice.CustomerId;
            result.CustomerName = journal.Invoice.Customer.CustomerName;
           
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_POS_GetList)]
        public async Task<PagedResultDto<POSGetListOutput>> GetList(GetListPOSInput input)
        {
            // get user by group member
            var userId = AbpSession.GetUserId();
            var userGroups = await GetUserGroupByLocation();
            var userPermission = await GetUserPermissions(userId);
            bool viewAll = userPermission.Contains(AppPermissions.Pages_Tenant_Customer_POS_ViewAll);

            var customerTypeMemberIds = await GetCustomerTypeMembers().Select(s => s.CustomerTypeId).ToListAsync();

            var query = from j in _journalRepository.GetAll()
                                     //.Include(u => u.Invoice.Customer)
                                     //.Include(u => u.Invoice.TransactionTypeSale)
                                     //.Include(u => u.Location)
                                     //.Include(u => u.CreatorUser)
                                     .Where(s => s.JournalType == JournalType.Invoice)
                                     .Where(s => s.Status == TransactionStatus.Publish)
                                     .Where(s => s.Invoice != null && s.Invoice.TransactionTypeSale.IsPOS)
                                     .Where(s => s.Date.Date == input.Date.Date)
                                     .WhereIf(customerTypeMemberIds.Any(), s => customerTypeMemberIds.Contains(s.Invoice.Customer.CustomerTypeId.Value))
                                     .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                                     .WhereIf(input.Users != null && input.Users.Count > 0 && !viewAll, u => input.Users.Contains(u.CreatorUserId))
                                     .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId.Value))
                                     .AsNoTracking()
                        select new POSGetListOutput
                        {
                            Id = j.InvoiceId.Value,
                            Date = j.Date.Date,
                            InvoiceNo = j.JournalNo,
                            Total = j.Invoice.MultiCurrencyTotal,
                            User = new UserDto
                            {
                                Id = j.CreatorUser.Id,
                                UserName = $"{j.CreatorUser.Surname} {j.CreatorUser.Name}",
                            },
                            PaidStatus = j.Invoice.PaidStatus,
                            PaidStatusName = j.Invoice.PaidStatus.ToString(),
                            StatusName = j.Status.ToString(),
                            StatusCode = j.Status,
                            CustomerName = j.Invoice.Customer.CustomerName,
                            Location = new LocationSummaryOutput
                            {
                                LocationName = j.Location.LocationName,
                            },
                            PaymentMethods = new List<string>(),
                        };


            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<POSGetListOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_POS_GetList)]
        public async Task<PagedResultDto<POSGetListOutput>> GetUnpaidInvoice(GetUnpaidPOSInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var userId = AbpSession.GetUserId();
            var userPermission = await GetUserPermissions(userId);
            bool viewAll = userPermission.Contains(AppPermissions.Pages_Tenant_Customer_POS_ViewAll);
            var customerTypeMemberIds = await GetCustomerTypeMembers().Select(s => s.CustomerTypeId).ToListAsync();

            var query = from j in _journalRepository.GetAll()
                                     //.Include(u => u.Invoice.Customer)
                                     //.Include(u => u.Invoice.TransactionTypeSale)
                                     //.Include(u => u.Location)
                                     //.Include(u => u.CreatorUser)
                                     .Where(s => s.JournalType == JournalType.Invoice && s.Invoice != null && s.Invoice.TransactionTypeSale.IsPOS == true && s.Status == TransactionStatus.Publish && s.Invoice.PaidStatus != PaidStatuse.Paid)
                                     .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                                     .WhereIf(input.Users != null && input.Users.Count > 0 && !viewAll, u => input.Users.Contains(u.CreatorUserId))
                                     .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId.Value))
                                     .WhereIf(customerTypeMemberIds.Any(), s => customerTypeMemberIds.Contains(s.Invoice.Customer.CustomerTypeId.Value))
                                     .AsNoTracking()

                        select new POSGetListOutput
                        {
                            Id = j.Invoice.Id,
                            Date = j.Date,
                            InvoiceNo = j.JournalNo,
                            Total = j.Invoice.MultiCurrencyTotal,
                            User = new UserDto
                            {
                                Id = j.CreatorUser.Id,
                                UserName = $"{j.CreatorUser.Surname} {j.CreatorUser.Name}",
                            },
                            PaidStatus = j.Invoice.PaidStatus,
                            PaidStatusName = j.Invoice.PaidStatus.ToString(),
                            StatusName = j.Status.ToString(),
                            StatusCode = j.Status,
                            Location = new LocationSummaryOutput
                            {
                                LocationName = j.Location.LocationName
                            },
                            PaymentMethods = new List<string>(),
                            CustomerName = j.Invoice.Customer.CustomerName
                        };



            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<POSGetListOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_POS_UpdateToVoid)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input)
        {

            var @entity = await _journalRepository
                                .GetAll()
                                .Include(u => u.Invoice)
                                .Where(u => u.JournalType == JournalType.Invoice && u.InvoiceId == input.Id)
                                .FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            if (entity.Status == TransactionStatus.Void)
            {
                throw new UserFriendlyException(L("StatusOrderVoided"));
            }


            //if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                                       .Where(t => (t.LockKey == TransactionLockType.Invoice)
                                       && t.IsLock == true && t.LockDate != null && t.LockDate.Value.Date >= entity.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }


            //Check if invoice has return
            var returnJournal = await _journalRepository.GetAll().Include(s => s.CustomerCredit).FirstOrDefaultAsync(s => s.CustomerCredit.ItemIssueSaleId == entity.Invoice.ItemIssueId && s.Status == TransactionStatus.Publish);
            if (returnJournal != null) throw new UserFriendlyException(L("AlreadyHasReturnCannotBeVoided") + $", Return No : {returnJournal.JournalNo}");


            var paymentIds = await (from ri in _receivePaymentDetailRepository.GetAll()
                                   .Where(s => s.InvoiceId == input.Id)
                                   .AsNoTracking()
                                   join j in _journalRepository.GetAll()
                                   .Where(s => s.Status == TransactionStatus.Publish)
                                   .AsNoTracking()
                                   on ri.ReceivePaymentId equals j.ReceivePaymentId
                                   select ri)
                                   .GroupBy(s => s.ReceivePaymentId)
                                   .Select(s => s.Key)
                                   .ToListAsync();
            foreach (var paymentId in paymentIds)
            {
                await UpdatePaymentStatusToVoidHelper(new UpdateStatus { Id = paymentId });
            }

            var scheduleItems = new List<Guid>();

            entity.UpdateVoid();
            // starting update item issue to void if it has convert to item issue and has item issue id
            if (entity.Invoice.ConvertToItemIssue == true && entity.Invoice.ItemIssueId != null)
            {
                var @jounalItemIssue = await _journalRepository.GetAll()
                 .Include(u => u.ItemIssue)
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

                    scheduleItems = @ItemIssueItems.Select(s => s.ItemId).Distinct().ToList();

                }
                @jounalItemIssue.UpdateVoid();

            }
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Invoice);
            CheckErrors(await _journalManager.UpdateAsync(entity, auto.DocumentType));

            if (entity.Invoice.ItemIssueId.HasValue)
            {
                await DeleteInventoryTransactionItems(entity.Invoice.ItemIssueId.Value);
                if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
                {
                    await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, entity.Date, scheduleItems);
                }

            }

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        protected async Task<NullableIdDto<Guid>> UpdatePaymentStatusToVoidHelper(UpdateStatus input)
        {
            var @jounal = await _journalRepository.GetAll()
                .Include(u => u.ReceivePayment)
                .Where(u => u.JournalType == JournalType.ReceivePayment && u.ReceivePaymentId == input.Id)
                .FirstOrDefaultAsync();
            if (@jounal == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            else if(jounal.Status == TransactionStatus.Void)
            {
                throw new UserFriendlyException(L("StatusAlreadyVoided"));
            }

            //query get invoice item and update status and reveres total paid 
            var receivePaymentItems = await _receivePaymentDetailRepository.GetAll()
                .Where(u => u.ReceivePaymentId == @jounal.ReceivePaymentId).ToListAsync();


            foreach (var bi in receivePaymentItems)
            {
                if (bi.InvoiceId != null)
                {
                    // update total balance in invoice               
                    var updateInvoice = _invoiceRepository.GetAll().Where(u => u.Id == bi.InvoiceId).FirstOrDefault();
                    if (updateInvoice != null)
                    {
                        updateInvoice.UpdateOpenBalance(bi.Payment);
                        updateInvoice.UpdateMultiCurrencyOpenBalance(bi.MultiCurrencyPayment);

                        // verify status if publish than update paid status
                        if (jounal.Status == TransactionStatus.Publish)
                        {
                            updateInvoice.UpdateTotalPaid(-1 * bi.Payment);
                            updateInvoice.UpdateMultiCurrencyTotalPaid(-1 * bi.MultiCurrencyPayment);
                            if (updateInvoice.TotalPaid == 0)
                            {
                                updateInvoice.UpdatePaidStatus(PaidStatuse.Pending);
                            }
                            else
                            {
                                updateInvoice.UpdatePaidStatus(PaidStatuse.Partial);
                            }
                        }
                        CheckErrors(await _invoiceManager.UpdateAsync(updateInvoice));
                    }
                }
                else if (bi.CustomerCreditId != null)
                {
                    // update total balance in invoice               
                    var updateCustomerCredit = _customerCreditRepository.GetAll().Where(u => u.Id == bi.CustomerCreditId).FirstOrDefault();
                    if (updateCustomerCredit != null)
                    {
                        updateCustomerCredit.IncreaseOpenbalance(bi.Payment);
                        updateCustomerCredit.IncreaseMultiCurrencyOpenBalance(bi.MultiCurrencyPayment);

                        // verify status if publish than update paid status
                        if (jounal.Status == TransactionStatus.Publish)
                        {
                            updateCustomerCredit.IncreaseTotalPaid(-1 * bi.Payment);
                            updateCustomerCredit.IncreaseMultiCurrencyTotalPaid(-1 * bi.MultiCurrencyPayment);
                            if (updateCustomerCredit.TotalPaid == 0)
                            {
                                updateCustomerCredit.UpdatePaidStatus(PaidStatuse.Pending);
                            }
                            else
                            {
                                updateCustomerCredit.UpdatePaidStatus(PaidStatuse.Partial);
                            }
                        }
                        CheckErrors(await _customerCreditManager.UpdateAsync(updateCustomerCredit));
                    }
                }

            }


            jounal.UpdateVoid();
            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.RecievePayment);
            CheckErrors(await _journalManager.UpdateAsync(jounal, autoSequence.DocumentType));
            return new NullableIdDto<Guid>() { Id = jounal.ReceivePaymentId };
        }

    }
}
