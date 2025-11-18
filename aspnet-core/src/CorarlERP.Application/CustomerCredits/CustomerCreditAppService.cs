using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Authorization;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies;
using CorarlERP.Currencies.Dto;
using CorarlERP.CustomerCredits.Dto;
using CorarlERP.Customers;
using CorarlERP.Customers.Dto;
using CorarlERP.Items.Dto;
using CorarlERP.Journals;
using CorarlERP.Journals.Dto;
using CorarlERP.Taxes.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CorarlERP.enumStatus.EnumStatus;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using CorarlERP.Items;
using CorarlERP.ItemReceiptCustomerCredits;
using CorarlERP.ReceivePayments;
using CorarlERP.ItemReceiptCustomerCredits.Dto;
using static CorarlERP.ItemReceiptCustomerCredits.ItemReceiptCustomerCredit;
using CorarlERP.MultiTenancy;
using CorarlERP.AutoSequences;
using CorarlERP.Lots.Dto;
using CorarlERP.Locations.Dto;
using CorarlERP.UserGroups;
using CorarlERP.Locations;
using CorarlERP.Reports.Dto;
using CorarlERP.ReportTemplates;
using CorarlERP.Addresses;
using CorarlERP.Reports;
using CorarlERP.Dto;
using OfficeOpenXml;
using System.IO;
using CorarlERP.ChartOfAccounts;
using CorarlERP.AccountCycles;
using CorarlERP.Inventories;
using CorarlERP.ItemIssues;
using CorarlERP.Locks;
using CorarlERP.Common.Dto;
using CorarlERP.Lots;
using CorarlERP.POS.Dto;
using CorarlERP.ReceivePayments.Dto;
using CorarlERP.PaymentMethods;
using CorarlERP.MultiCurrencies;
using Abp.Dependency;
using CorarlERP.Invoices;
using CorarlERP.Migrations;
using CorarlERP.Settings;
using CorarlERP.FileStorages;
using CorarlERP.InventoryCalculationJobSchedules;
using Hangfire.States;
using CorarlERP.InventoryCalculationItems;
using CorarlERP.BatchNos;
using CorarlERP.ItemReceipts;
using CorarlERP.Features;
using CorarlERP.JournalTransactionTypes;
using CorarlERP.Exchanges.Dto;

namespace CorarlERP.CustomerCredits
{
    // Create By @Huy Sokhom
    // Creation Date: 09-08-2018
    [AbpAuthorize(AppPermissions.Pages_Tenant_Customers, AppPermissions.Pages_Tenant_Customers_Invoice_SaleReturn)]
    public class CustomerCreditAppService : ReportBaseClass, ICustomerCreditAppService
    {
        // Customer Credit
        private readonly AppFolders _appFolders;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;
        private readonly ICurrencyManager _currencyManager;
        private readonly IRepository<Currency, long> _currencyRepository;

        private readonly ICustomerManager _customerManager;
        private readonly IRepository<Customer, Guid> _customerRepository;

        private readonly IRepository<ItemReceiptCustomerCredit, Guid> _itemReceiptCustomerCredit;

        private readonly ICustomerCreditDetailManager _customerCreditDetailManager;
        private readonly IRepository<CustomerCreditDetail, Guid> _customerCreditDetailRepository;
        private readonly ICustomerCreditManager _customerCreditManager;
        private readonly IRepository<CustomerCredit, Guid> _customerCreditRepository;
        private readonly ICorarlRepository<CustomerCreditExchangeRate, Guid> _exchangeRateRepository;

        // Journal 
        private readonly IJournalManager _journalManager;
        private readonly IRepository<ReceivePayments.ReceivePaymentDetail, Guid> _receivePaymentDetailRepository;
        private readonly IRepository<Journal, Guid> _journalRepository;
        private readonly IJournalItemManager _journalItemManager;
        private readonly IRepository<JournalItem, Guid> _journalItemRepository;

        private readonly IRepository<ItemIssueItem, Guid> _itemIssueItemRepository;

        private readonly IRepository<Item, Guid> _itemRepository;
        private readonly IItemManager _itemManager;
        //validate 
        private readonly IRepository<ReceivePayment, Guid> _recievePaymentReposity;
        private readonly IItemReceiptCustomerCreditManager _itemReceiptCustomerCreditManager;
        private readonly IItemReceiptItemCustomerCreditManager _itemReceiptCustomerCreditItemManager;
        private readonly IRepository<ItemReceiptItemCustomerCredit, Guid> _itemReceiptCustomerCreditItemRepository;
        private readonly IRepository<ItemReceiptCustomerCredit, Guid> _itemReceiptCustomerCreditReository;

        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;

        private readonly IRepository<Tenant, int> _tenantRepository;

        private readonly IRepository<Taxes.Tax, long> _taxRepository;
        private readonly IRepository<Locations.Location, long> _locationRepository;
        private readonly IRepository<Classes.Class, long> _classRepository;

        private readonly IRepository<AccountCycle, long> _accountCycleRepository;
        private readonly IInventoryManager _inventoryManager;
        private readonly IRepository<Lock, long> _lockRepository;
        private readonly IRepository<Lot, long> _lotrepository;

        private readonly IRepository<MultiCurrency, long> _multiCurrencyRepository;
        private readonly IReceivePaymentManager _receivePaymentManager;
        private readonly IReceivePaymentDetailManager _receivePaymentDetailManager;
        private readonly IReceivePaymentExpenseManager _receivePaymentExpenseManager;
        private readonly IInvoiceManager _invoiceManager;
        private readonly IRepository<Invoice, Guid> _invoiceRepository;
        private readonly IRepository<InvoiceItem, Guid> _invoiceItemRepository;
        private readonly IRepository<BillInvoiceSetting, long> _settingRepository;
        private readonly IFileStorageManager _fileStorageManager;
        private readonly IInventoryCalculationItemManager _inventoryCalculationItemManager;
        private readonly ICorarlRepository<BatchNo, Guid> _batchNoRepository;
        private readonly ICorarlRepository<ItemIssueItemBatchNo, Guid> _itemIssueItemBatchNoRepository;
        private readonly ICorarlRepository<CustomerCreditItemBatchNo, Guid> _customerCreditItemBatchNoRepository;
        private readonly ICorarlRepository<ItemReceiptCustomerCreditItemBatchNo, Guid> _itemReceiptCustomerCreditItemBatchNoRepository;
        private readonly IJournalTransactionTypeManager _journalTransactionTypeManager;
        public CustomerCreditAppService(
            ICorarlRepository<CustomerCreditExchangeRate, Guid> exchangeRateRepository,
            IJournalManager journalManager,
            IJournalItemManager journalItemManager,
            IRepository<Journal, Guid> journalRepository,
            IRepository<JournalItem, Guid> journalItemRepository,
            ICustomerManager customerManager,
            ICurrencyManager currencyManager,
            IRepository<Customer, Guid> customerRepository,
            IRepository<Currency, long> currencyRepository,
            CustomerCreditManager customerCreditManager,
            CustomerCreditDetailManagers customerCreditDetailManager,
            IRepository<CustomerCredit, Guid> customerCreditRepository,
            IRepository<CustomerCreditDetail, Guid> customerCreditDetailRepository,
            IRepository<Item, Guid> itemRepository,
            ItemManager itemManager,
            IFileStorageManager fileStorageManager,
            IRepository<ItemReceiptCustomerCredit, Guid> itemReceiptCustomerCredit,
            IRepository<ReceivePayment, Guid> recievePaymentReposity,
            ItemReceiptCustomerCreditManager itemReceiptCustomerCreditManager,
            ItemReceiptItemCustomerCreditManager itemReceiptCustomerCreditItemManager,
            IRepository<Tenant, int> tenantRepository,
            IRepository<ItemReceiptItemCustomerCredit, Guid> itemReceiptCustomerCreditItemRepository,
            IAutoSequenceManager autoSequenceManger,
            IRepository<AutoSequence, Guid> autoSequenceRepository,
            IRepository<Location, long> locationRepository,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            IRepository<Taxes.Tax, long> taxRepository,
            IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
            IRepository<Classes.Class, long> classRepository,
            IRepository<ReceivePayments.ReceivePaymentDetail, Guid> receivePaymentDetailRepository,
            IRepository<ItemReceiptCustomerCredit, Guid> itemReceiptCustomerCreditReository,
            IInventoryCalculationItemManager inventoryCalculationItemManager,
            ICorarlRepository<BatchNo, Guid> batchNoRepository,
            ICorarlRepository<ItemIssueItemBatchNo, Guid> itemIssueItemBatchNoRepository,
            ICorarlRepository<CustomerCreditItemBatchNo, Guid> customerCreditItemBatchNoRepository,
            ICorarlRepository<ItemReceiptCustomerCreditItemBatchNo, Guid> itemReceiptCustomerCreditItemBatchNoRepository,
            AppFolders appFolders,
            IInventoryManager inventoryManager,
            IRepository<AccountCycle, long> accountCycleRepository,
            IRepository<ItemIssueItem, Guid> itemIssueItemRepository,
            IRepository<Lock, long> lockRepository,
            IRepository<Lot, long> lotrepository,
            IInvoiceManager invoiceManager,
            IRepository<Invoice, Guid> invoiceRepository,
            IRepository<InvoiceItem, Guid> invoiceItemRepository,
            IJournalTransactionTypeManager journalTransactionTypeManager
        ) : base(accountCycleRepository, appFolders, userGroupMemberRepository, locationRepository)
        {
            _receivePaymentDetailRepository = receivePaymentDetailRepository;

            _itemIssueItemRepository = itemIssueItemRepository;
            _accountCycleRepository = accountCycleRepository;
            _inventoryManager = inventoryManager;

            _itemReceiptCustomerCreditReository = itemReceiptCustomerCreditReository;
            _chartOfAccountRepository = chartOfAccountRepository;
            _journalManager = journalManager;
            _journalRepository = journalRepository;
            _journalItemManager = journalItemManager;
            _journalItemRepository = journalItemRepository;
            _fileStorageManager = fileStorageManager;

            _itemManager = itemManager;
            _itemRepository = itemRepository;

            _customerCreditManager = customerCreditManager;
            _customerCreditRepository = customerCreditRepository;
            _customerCreditDetailManager = customerCreditDetailManager;
            _customerCreditDetailRepository = customerCreditDetailRepository;

            _customerRepository = customerRepository;
            _customerManager = customerManager;
            _currencyRepository = currencyRepository;
            _currencyManager = currencyManager;

            _itemReceiptCustomerCredit = itemReceiptCustomerCredit;
            _recievePaymentReposity = recievePaymentReposity;
            _itemReceiptCustomerCreditManager = itemReceiptCustomerCreditManager;
            _itemReceiptCustomerCreditItemManager = itemReceiptCustomerCreditItemManager;
            _tenantRepository = tenantRepository;
            _itemReceiptCustomerCreditItemRepository = itemReceiptCustomerCreditItemRepository;
            _autoSequenceManager = autoSequenceManger;
            _autoSequenceRepository = autoSequenceRepository;
            _inventoryCalculationItemManager = inventoryCalculationItemManager;

            _appFolders = appFolders;
            _locationRepository = locationRepository;
            _classRepository = classRepository;
            _taxRepository = taxRepository;
            _lockRepository = lockRepository;
            _lotrepository = lotrepository;
            _multiCurrencyRepository = IocManager.Instance.Resolve<IRepository<MultiCurrency, long>>();
            _receivePaymentManager = IocManager.Instance.Resolve<IReceivePaymentManager>();
            _receivePaymentDetailManager = IocManager.Instance.Resolve<IReceivePaymentDetailManager>();
            _receivePaymentExpenseManager = IocManager.Instance.Resolve<IReceivePaymentExpenseManager>();

            _invoiceManager = invoiceManager;
            _invoiceRepository = invoiceRepository;
            _invoiceItemRepository = invoiceItemRepository;
            _settingRepository = IocManager.Instance.Resolve<IRepository<BillInvoiceSetting, long>>();
            _batchNoRepository = batchNoRepository;
            _itemIssueItemBatchNoRepository = itemIssueItemBatchNoRepository;
            _customerCreditItemBatchNoRepository = customerCreditItemBatchNoRepository;
            _itemReceiptCustomerCreditItemBatchNoRepository = itemReceiptCustomerCreditItemBatchNoRepository;
            _journalTransactionTypeManager = journalTransactionTypeManager;
            _exchangeRateRepository = exchangeRateRepository;
        }

        /* Functionality for check if item or can check it a list of array is account tap from Frontend */
        private bool checkIfItemExist(List<CustomerCreditDetailInput> array, string value)
        {
            bool b = false;
            if (array.Count == 0) return b;
            foreach (var i in array)
            {
                if (i.ItemId.Equals(value))
                {
                    b = true;
                    return b;
                }
            }
            return b;
        }

        private async Task ValidateBatchNo(CreateCustomerCreditInput input)
        {
            if (!input.CustomerCreditDetail.Any(s => s.ItemId.HasValue && s.ItemId != Guid.Empty)) return;

            var validateItems = await _itemRepository.GetAll()
                            .Where(s => input.CustomerCreditDetail.Any(i => i.ItemId == s.Id))
                            .Where(s => s.UseBatchNo || s.TrackSerial || s.TrackExpiration)
                            .AsNoTracking()
                            .ToListAsync();

            if (!validateItems.Any()) return;

            var batchItemDic = validateItems.ToDictionary(k => k.Id, v => v);

            var itemUseBatchs = input.CustomerCreditDetail.Where(s => batchItemDic.ContainsKey(s.ItemId.Value)).ToList();

            var find = itemUseBatchs.Where(s => s.ItemBatchNos == null || s.ItemBatchNos.Count == 0 || s.ItemBatchNos.Any(r => r.BatchNoId == Guid.Empty)).FirstOrDefault();
            if (find != null) throw new UserFriendlyException(L("PleaseSelect", $"{L("BatchNo")}/{L("SerialNo")}") + $", Item: {batchItemDic[find.ItemId.Value].ItemCode}-{batchItemDic[find.ItemId.Value].ItemName}");

            var serialItemHash = validateItems.Where(s => s.TrackSerial).Select(s => s.Id).ToHashSet();
            var serialQty = input.CustomerCreditDetail.Where(s => serialItemHash.Contains(s.ItemId.Value)).FirstOrDefault(s => s.ItemBatchNos.Any(b => b.Qty != 1));
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

        }

        private async Task CheckStockForSaleReturn(Guid itemIssueSaleId, List<CustomerCreditDetailInput> inputItems, List<Guid> exceptIds)
        {
            var issueQuery = from iri in _itemIssueItemRepository.GetAll()
                                .Where(u => u.ItemIssueId == itemIssueSaleId)
                                .AsNoTracking()
                             join b in _itemIssueItemBatchNoRepository.GetAll().AsNoTracking()
                             on iri.Id equals b.ItemIssueItemId
                             into bs
                             from b in bs.DefaultIfEmpty()
                             select new
                             {
                                 iri.Id,
                                 Qty = b == null ? iri.Qty : b.Qty,
                                 BatchNoId = b == null ? (Guid?)null : b.BatchNoId,
                                 iri.ItemId
                             };

            var cQuery = from r in _customerCreditDetailRepository.GetAll()
                                    .Where(s => s.ItemIssueSaleItemId != null)
                                    .WhereIf(exceptIds != null && exceptIds.Any(), s => !exceptIds.Contains(s.CustomerCreditId))
                                    .AsNoTracking()
                         join b in _customerCreditItemBatchNoRepository.GetAll()
                                   .AsNoTracking()
                         on r.Id equals b.CustomerCreditItemId
                         into bs
                         from b in bs.DefaultIfEmpty()
                         select new
                         {
                             r.ItemIssueSaleItemId,
                             Qty = b == null ? r.Qty : b.Qty,
                             BatchNoId = b == null ? (Guid?)null : b.BatchNoId
                         };

            var rQuery = from r in _itemReceiptCustomerCreditItemRepository.GetAll()
                                   .Where(s => s.ItemIssueSaleItemId != null)
                                   .WhereIf(exceptIds != null && exceptIds.Any(), s => !exceptIds.Contains(s.ItemReceiptCustomerCreditId))
                                   .AsNoTracking()
                         join b in _itemReceiptCustomerCreditItemBatchNoRepository.GetAll().AsNoTracking()
                         on r.Id equals b.ItemReceiptCustomerCreditItemId
                         into bs
                         from b in bs.DefaultIfEmpty()
                         select new
                         {
                             r.ItemIssueSaleItemId,
                             Qty = b == null ? r.Qty : b.Qty,
                             BatchNoId = b == null ? (Guid?)null : b.BatchNoId
                         };



            var query = from iri in issueQuery
                        join returnItem in cQuery
                        on $"{iri.Id}-{iri.BatchNoId}" equals $"{returnItem.ItemIssueSaleItemId}-{returnItem.BatchNoId}"
                        into returnItems

                        join returnItem2 in rQuery
                        on $"{iri.Id}-{iri.BatchNoId}" equals $"{returnItem2.ItemIssueSaleItemId}-{returnItem2.BatchNoId}"
                        into returnItems2

                        let returnQty = returnItems == null ? 0 : returnItems.Sum(s => s.Qty)
                        let returnQty2 = returnItems2 == null ? 0 : returnItems2.Sum(s => s.Qty)
                        let remainingQty = iri.Qty - returnQty - returnQty2
                        where remainingQty > 0
                        select new
                        {
                            Id = iri.Id,
                            ItemId = iri.ItemId,
                            Qty = remainingQty,
                            iri.BatchNoId
                        };

            var balanceList = await query.ToListAsync();

            foreach (var r in inputItems)
            {
                if (r.ItemBatchNos != null && r.ItemBatchNos.Any())
                {
                    foreach (var b in r.ItemBatchNos)
                    {
                        var checkStock = balanceList.FirstOrDefault(i => i.Id == r.ItemIssueSaleItemId && i.ItemId == r.ItemId && i.BatchNoId == b.BatchNoId && i.Qty < b.Qty);
                        if (checkStock != null) throw new UserFriendlyException(L("ReturnQtyCannotGreaterThanSaleQty"));
                    }
                }
                else
                {
                    var checkStock = balanceList.FirstOrDefault(i => i.Id == r.ItemIssueSaleItemId && i.ItemId == r.ItemId && i.Qty < r.Qty);
                    if (checkStock != null) throw new UserFriendlyException(L("ReturnQtyCannotGreaterThanSaleQty"));
                }
            }
        }

        private async Task CalculateTotal(CreateCustomerCreditInput input)
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
            foreach (var item in input.CustomerCreditDetail)
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

        void ValidateExchangeRate(CreateCustomerCreditInput input)
        {
            if (!input.UseExchangeRate || input.CurrencyId == input.MultiCurrencyId) return;

            if (input.ExchangeRate == null) throw new UserFriendlyException(L("IsRequired", L("ExchangeRate")));
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_Invoice_CreateCustomerCredit, AppPermissions.Pages_Tenant_Customers_Invoice_CreateSaleReturn)]
        public async Task<NullableIdDto<Guid>> Create(CreateCustomerCreditInput input)
        {
            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                  .Where(t => (t.LockKey == TransactionLockType.Invoice)
                  && t.IsLock == true && t.LockDate != null && t.LockDate != null && t.LockDate.Value.Date >= input.CreditDate.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            ValidateExchangeRate(input);
            await ValidateBatchNo(input);

            if (input.ReceiveFrom == ReceiveFrom.ItemIssueSale)
            {
                await CheckStockForSaleReturn(input.ItemIssueSaleId.Value, input.CustomerCreditDetail, null);
            }

            if (input.CustomerCreditDetail.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }


            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.CustomerCredit);

            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.CustomerCreditNo = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }

            await CalculateTotal(input);

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            // step 1. insert to journal header
            if (input.IsPOS)
            {
                input.Reference = input.CustomerCreditNo;
            }

            var @entity = Journal.Create(
                tenantId, userId, input.CustomerCreditNo, input.CreditDate, input.Memo,
                input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, input.LocationId
            );
            entity.UpdateStatus(input.Status);
            if (input.MultiCurrencyId == null || input.MultiCurrencyId == 0 || input.CurrencyId == input.MultiCurrencyId)
            {
                input.MultiCurrencyId = input.CurrencyId;
                input.MultiCurrencyTotal = input.Total;
                input.MultiCurrencySubTotal = input.SubTotal;
                input.MultiCurrencyTax = input.Tax;
            }
            entity.UpdateMultiCurrency(input.MultiCurrencyId);
            // step 2. insert into journal item with has default credit and debit auto zero
            var journalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, input.AccountId, input.Memo, 0, input.Total, PostingKey.AR, null);

            var isItem = input.CustomerCreditDetail.Any(s => s.ItemId != null && s.ItemId != Guid.Empty);

            // step 3. insert into customer credit master 
            var customerCredit = CustomerCredit.Create(tenantId, userId, input.CustomerId,
                input.SameAsShippingAddress,
                input.ShippingAddress, input.BillingAddress,
                input.SubTotal, input.Tax, input.Total, input.DueDate,
                input.ConvertToItemReceipt, input.ReceiveDate,
                input.MultiCurrencySubTotal, input.MultiCurrencyTax, input.MultiCurrencyTotal, input.ReceiveFrom, input.ItemIssueSaleId, input.IsPOS, isItem, input.UseExchangeRate);

            if (input.ReceiveFrom == ReceiveFrom.ItemReceiptCustomerCredit && input.ItemReceiptCustomerCreditId != null)
            {

                #region Calculat Cost
                int digit = await _accountCycleRepository.GetAll()
                     .Where(t => t.TenantId == tenantId && t.EndDate == null)
                     .OrderByDescending(t => t.StartDate)
                     .Select(t => t.RoundingDigit).FirstOrDefaultAsync();
                var items = input.CustomerCreditDetail.GroupBy(s => s.ItemId).Select(s => s.FirstOrDefault().ItemId.Value).ToList();
                var itemCostDic = await _inventoryManager.GetItemCostSummaryQuery(null, input.ReceiveDate.Value, new List<long?> { input.LocationId }, userId, items, new List<Guid> { input.ItemReceiptCustomerCreditId.Value }).ToDictionaryAsync(s => $"{s.ItemId}-{s.LocationId}", s => s);
                var itemLatestCostDic = await _inventoryManager.GetItemLatestCostSummaryQuery(input.ReceiveDate.Value, new List<long?> { input.LocationId }, items, new List<Guid> { input.ItemReceiptCustomerCreditId.Value }).ToDictionaryAsync(s => $"{s.ItemId}-{s.LocationId}", s => s);
                var itemPurchaseCostDic = await _itemRepository.GetAll().Where(s => items.Contains(s.Id)).Select(s => new { s.Id, Cost = s.PurchaseCost ?? 0 }).ToDictionaryAsync(s => s.Id, s => s.Cost);

                foreach (var r in input.CustomerCreditDetail)
                {
                    var key = $"{r.ItemId}-{input.LocationId}";

                    if (itemCostDic.ContainsKey(key) && itemCostDic[key].Qty > 0)
                    {
                        r.ItemReceiptCustomerCreditUnitCost = itemCostDic[key].Cost;
                    }
                    else if (itemLatestCostDic.ContainsKey(key))
                    {
                        r.ItemReceiptCustomerCreditUnitCost = itemLatestCostDic[key].Cost;
                    }
                    else
                    {
                        r.ItemReceiptCustomerCreditUnitCost = itemPurchaseCostDic.ContainsKey(r.ItemId.Value) ? itemPurchaseCostDic[r.ItemId.Value] : 0;
                    }

                    r.ItemReceiptCustomerCreditTotal = Math.Round(r.ItemReceiptCustomerCreditUnitCost * r.Qty, digit, MidpointRounding.AwayFromZero);
                }
                var ItemReceiptCustomerCreditTotal = input.CustomerCreditDetail.Sum(t => t.ItemReceiptCustomerCreditTotal);
                #endregion Calculat Cost           



                var ItemReceiptJournal = await _journalRepository.GetAll()
                                        .Include(u => u.ItemReceiptCustomerCredit)
                                        .Where(t => t.ItemReceiptCustomerCreditId == input.ItemReceiptCustomerCreditId).FirstOrDefaultAsync();

                if (ItemReceiptJournal != null && ItemReceiptJournal.ItemReceiptCustomerCreditId != null)
                {
                    //update item issue

                    var validateLockDate = await _lockRepository.GetAll()
                                 .Where(t => t.IsLock == true &&
                                 (t.LockDate.Value.Date >= ItemReceiptJournal.Date.Date || t.LockDate.Value.Date >= input.ReceiveDate.Value.Date)
                                 && (t.LockKey == TransactionLockType.ItemReceipt || t.LockKey == TransactionLockType.InventoryTransaction)).CountAsync();

                    var itemReceiptItems = await _itemReceiptCustomerCreditItemRepository.GetAll()
                                          .Where(s => s.ItemReceiptCustomerCreditId == input.ItemReceiptCustomerCreditId)
                                          .AsNoTracking()
                                          .ToListAsync();

                    var isChangeItems = input.CustomerCreditDetail.Count() != itemReceiptItems.Count() || input.CustomerCreditDetail.Any(s => itemReceiptItems.Any(r => r.Id == s.ItemReceiptCustomerCreditItemId && (r.ItemId != s.ItemId || r.LotId != s.LotId || r.Qty != s.Qty)));

                    if (input.IsConfirm == false && validateLockDate > 0 && (ItemReceiptJournal.Date.Date != input.ReceiveDate.Value.Date || isChangeItems))
                    {
                        throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                    }

                    ItemReceiptJournal.UpdateJournalDate(input.ReceiveDate.Value);
                    ItemReceiptJournal.ItemReceiptCustomerCredit.UpdateTotal(ItemReceiptCustomerCreditTotal);
                    ItemReceiptJournal.UpdateCreditDebit(ItemReceiptCustomerCreditTotal, ItemReceiptCustomerCreditTotal);
                    ItemReceiptJournal.ItemReceiptCustomerCredit.UpdateCustomerCredit(customerCredit.Id);

                    var setting = await _settingRepository.FirstOrDefaultAsync(s => s.SettingType == BillInvoiceSettingType.Invoice);
                    if (setting == null || setting.ReferenceSameAsGoodsMovement) ItemReceiptJournal.UpdateReference(input.Reference);

                    var autoIss = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt_CustomerCredit);
                    CheckErrors(await _journalManager.UpdateAsync(ItemReceiptJournal, autoIss.DocumentType));
                    CheckErrors(await _itemReceiptCustomerCreditManager.UpdateAsync(ItemReceiptJournal.ItemReceiptCustomerCredit));
                }



                var itemReceiptCustomerCredit = await _itemReceiptCustomerCreditReository.GetAll().Where(t => t.Id == input.ItemReceiptCustomerCreditId).FirstOrDefaultAsync();
                itemReceiptCustomerCredit.UpdateCustomerCredit(customerCredit.Id);
                itemReceiptCustomerCredit.UpdateTotal(ItemReceiptCustomerCreditTotal);
                CheckErrors(await _itemReceiptCustomerCreditManager.UpdateAsync(itemReceiptCustomerCredit));
                customerCredit.UpdateShipedStatus(DeliveryStatus.ShipAll);

            }

            //Check if credit list item detail has no item ID so it mean no receive status 
            var isAccountTap = checkIfItemExist(input.CustomerCreditDetail, null);
            if (isAccountTap)
            {
                customerCredit.UpdateShipedStatus(DeliveryStatus.NoReceive);
            }

            @entity.UpdateCustomerCredit(customerCredit);
            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));
            CheckErrors(await _journalItemManager.CreateAsync(journalItem));
            CheckErrors(await _customerCreditManager.CreateAsync(customerCredit));

            if (input.UseExchangeRate && input.CurrencyId != input.MultiCurrencyId)
            {
                var exchange = CustomerCreditExchangeRate.Create(tenantId, userId, customerCredit.Id, input.ExchangeRate.FromCurrencyId, input.ExchangeRate.ToCurrencyId, input.ExchangeRate.Bid, input.ExchangeRate.Ask);
                await _exchangeRateRepository.InsertAsync(exchange);
            }

            // Loop item list to insert into detail customer credit 
            int index = 0;

            var itemReceiptCustomerCreditItemBatchNos = new List<ItemReceiptCustomerCreditItemBatchNo>();
            var itemReceiptCustomerCreditItems = new List<ItemReceiptItemCustomerCredit>();
            var @inventoryJournalItems = new List<JournalItem>();

            if (input.ItemReceiptCustomerCreditId.HasValue)
            {
                itemReceiptCustomerCreditItems = await _itemReceiptCustomerCreditItemRepository.GetAll()
                                                       .Where(t => t.ItemReceiptCustomerCreditId == input.ItemReceiptCustomerCreditId)
                                                       .ToListAsync();

                @inventoryJournalItems = await _journalItemRepository.GetAll()
                                               .Where(s => s.Journal.ItemReceiptCustomerCreditId.HasValue)
                                               .Where(u => u.Journal.ItemReceiptCustomerCreditId == input.ItemReceiptCustomerCreditId && u.Identifier != null)
                                               .ToListAsync();

                itemReceiptCustomerCreditItemBatchNos = await _itemReceiptCustomerCreditItemBatchNoRepository.GetAll()
                                                              .AsNoTracking()
                                                              .Where(s => s.ItemReceiptCustomerCreditItem.ItemReceiptCustomerCreditId == input.ItemReceiptCustomerCreditId.Value)
                                                              .ToListAsync();
            }


            var lot = await _lotrepository.GetAll().Where(t => t.LocationId == input.LocationId).FirstOrDefaultAsync();
            foreach (var i in input.CustomerCreditDetail)
            {
                if (input.IsPOS == true && input.ReceiveFrom == ReceiveFrom.None)
                {
                    i.LotId = lot.Id;
                    i.LotDetail = new LotSummaryOutput
                    {
                        LotName = lot.LotName,
                        Id = lot.Id
                    };
                }

                index++;
                if (i.LotId == null && i.ItemId != null)
                {
                    throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + index.ToString());
                }
                if (input.CurrencyId == input.MultiCurrencyId)
                {
                    i.MultiCurrencyTotal = i.Total;
                    i.MultiCurrencyUnitCost = i.UnitCost;
                }

                //if(i.Item.ItemTypeId == 3)
                //{
                //    throw new UserFriendlyException(L("CanNotReceiveSevice") + L("Row") + " " + index.ToString());
                //}

                // step 4. insert into Customer credit detail
                var customerCreditDetail = CustomerCreditDetail.Create(tenantId, userId, customerCredit, i.TaxId, i.ItemId, i.Description, i.Qty, i.UnitCost, i.DiscountRate, i.Total, i.MultiCurrencyUnitCost, i.MultiCurrencyTotal, i.ItemIssueSaleItemId, i.SalePrice);
                customerCreditDetail.UpdateLot(i.LotId);
                CheckErrors(await _customerCreditDetailManager.CreateAsync(customerCreditDetail));

                // Update item Issue vendor credit item when recieve from item issue vendor credit
                if (input.ReceiveFrom == ReceiveFrom.ItemReceiptCustomerCredit && input.ItemReceiptCustomerCreditId != null)
                {
                    var journalItemReceiptCustomerCredit = @inventoryJournalItems.ToList().Where(u => u.Identifier == i.ItemReceiptCustomerCreditItemId);

                    if (journalItemReceiptCustomerCredit != null && journalItemReceiptCustomerCredit.Count() > 0)
                    {
                        foreach (var c in journalItemReceiptCustomerCredit)
                        {
                            if (c.Key == PostingKey.Inventory)
                            {
                                // update inventory posting key credit
                                c.SetDebitCredit(i.ItemReceiptCustomerCreditTotal, 0);
                                CheckErrors(await _journalItemManager.UpdateAsync(c));
                            }
                            if (c.Key == PostingKey.COGS)
                            {
                                // update cogs posting key debit
                                c.SetDebitCredit(0, i.ItemReceiptCustomerCreditTotal);
                                CheckErrors(await _journalItemManager.UpdateAsync(c));
                            }
                        }

                    }

                    var itemReceiptCustomerCreditItem = itemReceiptCustomerCreditItems.Where(t => t.Id == i.ItemReceiptCustomerCreditItemId).FirstOrDefault();
                    if (itemReceiptCustomerCreditItem != null)
                    {
                        itemReceiptCustomerCreditItem.UpdateCustomerCreditItemId(customerCreditDetail.Id);
                        itemReceiptCustomerCreditItem.UpdateQty(i.Qty);
                        itemReceiptCustomerCreditItem.UpdateItemReceiptItem(i.ItemReceiptCustomerCreditUnitCost, 0, i.ItemReceiptCustomerCreditTotal);
                        CheckErrors(await _itemReceiptCustomerCreditItemManager.UpdateAsync(itemReceiptCustomerCreditItem));

                        var oldItemBatchs = itemReceiptCustomerCreditItemBatchNos.Where(s => s.ItemReceiptCustomerCreditItemId == itemReceiptCustomerCreditItem.Id).ToList();
                        if (i.ItemBatchNos != null && i.ItemBatchNos.Any())
                        {
                            var addItemBatchNos = i.ItemBatchNos.Where(s => !s.Id.HasValue || s.Id == Guid.Empty || !oldItemBatchs.Any(b => b.BatchNoId == s.BatchNoId))
                                                   .Select(s => ItemReceiptCustomerCreditItemBatchNo.Create(tenantId, userId, itemReceiptCustomerCreditItem.Id, s.BatchNoId, s.Qty)).ToList();
                            if (addItemBatchNos.Any())
                            {
                                foreach (var a in addItemBatchNos)
                                {
                                    await _itemReceiptCustomerCreditItemBatchNoRepository.InsertAsync(a);
                                }
                            }

                            var updateItemBatchNos = oldItemBatchs.Where(s => i.ItemBatchNos.Any(r => r.BatchNoId == s.BatchNoId)).Select(s =>
                            {
                                var oldItemBatch = s;
                                var newBatch = i.ItemBatchNos.FirstOrDefault(r => r.BatchNoId == s.BatchNoId);

                                oldItemBatch.Update(userId, itemReceiptCustomerCreditItem.Id, newBatch.BatchNoId, newBatch.Qty);
                                return oldItemBatch;
                            }).ToList();

                            if (updateItemBatchNos.Any()) await _itemReceiptCustomerCreditItemBatchNoRepository.BulkUpdateAsync(updateItemBatchNos);

                            var deleteItemBatchNos = oldItemBatchs.Where(s => !i.ItemBatchNos.Any(r => r.BatchNoId == s.BatchNoId)).ToList();
                            if (deleteItemBatchNos.Any())
                            {
                                await _itemReceiptCustomerCreditItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
                            }
                        }
                        else if (oldItemBatchs.Any())
                        {
                            await _itemReceiptCustomerCreditItemBatchNoRepository.BulkDeleteAsync(oldItemBatchs);
                        }
                    }

                }
                //@assign customer credit item id to id of input for to use in shortcut auto convert
                if (input.ConvertToItemReceipt == true)
                {
                    i.Id = customerCreditDetail.Id;
                }

                // step 5. insert into journal item with default debit value and credit auto zero
                var saleAllowanceAcc = JournalItem.CreateJournalItem(tenantId, userId, entity, i.AccountId, i.Description, i.Total, 0, PostingKey.SaleAllowance, customerCreditDetail.Id);
                CheckErrors(await _journalItemManager.CreateAsync(saleAllowanceAcc));

                if (i.ItemBatchNos != null && i.ItemBatchNos.Any())
                {
                    var addItemBatchNos = i.ItemBatchNos.Select(s => CustomerCreditItemBatchNo.Create(tenantId, userId, customerCreditDetail.Id, s.BatchNoId, s.Qty)).ToList();
                    foreach (var a in addItemBatchNos)
                    {
                        await _customerCreditItemBatchNoRepository.InsertAsync(a);
                    }
                }
            }


            await CurrentUnitOfWork.SaveChangesAsync();

            if (input.ConvertToItemReceipt == true && input.Status == TransactionStatus.Publish)
            {
                var getItem = await _itemRepository.GetAll().Where(g => g.ItemTypeId != 3 && input.CustomerCreditDetail.Any(i => i.ItemId == g.Id)).ToListAsync();

                var itemReceiptInput = new CreateItemReceiptCustomerCreditInput()
                {
                    IsConfirm = input.IsConfirm,
                    Status = input.Status,
                    BillingAddress = input.BillingAddress,
                    ClassId = input.ClassId,
                    CurrencyId = input.CurrencyId,
                    CustomerCreditId = customerCredit.Id,
                    CustomerId = input.CustomerId,
                    Date = input.ReceiveDate.Value,
                    LocationId = input.LocationId,
                    Memo = input.Memo,
                    ReceiptNo = input.CustomerCreditNo,
                    ReceiveFrom = ReceiveFrom.CustomerCredit,
                    Reference = input.Reference,
                    SameAsShippingAddress = input.SameAsShippingAddress,
                    ShippingAddress = input.ShippingAddress,
                    Total = input.Total,
                    Items = input.CustomerCreditDetail
                        .Where(d => getItem.Any(v => v.Id == d.ItemId))
                        .Select(t => new CreateOrUpdateItemReceiptCustomerCreditItemInput
                        {
                            LotId = t.LotId,
                            UnitCost = t.UnitCost,
                            Total = t.Total,
                            CustomerCreditItemId = t.Id,
                            Description = t.Description,
                            DiscountRate = t.DiscountRate,
                            InventoryAccountId = t.Item.InventoryAccountId.Value,
                            ClearanceAccountId = getItem.Where(g => g.Id == t.ItemId).Select(g => g.PurchaseAccountId.Value).FirstOrDefault(),
                            Item = t.Item,
                            ItemId = t.ItemId.Value,
                            Qty = t.Qty,
                            ItemBatchNos = t.ItemBatchNos
                        }).ToList()
                };
                await CreateItemReceiptCustomerCredit(itemReceiptInput);
            }

            //Refund
            if (input.ReceivePayments != null && input.ReceivePayments.Any())
            {
                foreach (var refund in input.ReceivePayments)
                {
                    if (customerCredit.MultiCurrencyOpenBalance == 0) break;

                    await Refund(refund, input, customerCredit);
                }
            }


            var orderIds = input.CustomerCreditDetail.Where(s => s.OrderId.HasValue).GroupBy(s => s.OrderId.Value).Select(s => s.Key);
            var deliveryIds = input.CustomerCreditDetail.Where(s => s.DeliveryId.HasValue).GroupBy(s => s.DeliveryId.Value).Select(s => s.Key);
            if (orderIds.Any() || deliveryIds.Any())
            {
                await this.CurrentUnitOfWork.SaveChangesAsync();
                foreach (var id in orderIds)
                {
                    await UpdateOrderInventoryStatus(id);
                }
                foreach (var id in deliveryIds)
                {
                    await UpdateDeliveryInventoryStatus(id, true);
                }
            }


            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.Invoice };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }


            return new NullableIdDto<Guid>() { Id = customerCredit.Id };
        }

        protected async Task<NullableIdDto<Guid>> Refund(POSPaymentSummaryByPaymentMethodInput payment, CreateCustomerCreditInput customerCreditInput, CustomerCredit customerCredit)
        {
            var input = new CreateReceivePaymentInput
            {
                ReceiveFrom = ReceiveFromRecievePayment.Cash,
                Status = TransactionStatus.Publish,
                FiFo = false,
                ClassId = customerCreditInput.ClassId,
                LocationId = customerCreditInput.LocationId,
                CurrencyId = customerCreditInput.CurrencyId,
                MultiCurrencyId = customerCreditInput.MultiCurrencyId,
                IsConfirm = customerCreditInput.IsConfirm,
                Memo = string.Empty,
                PaymentNo = null,
                Reference = null,
                PaymentAccountId = payment.AccountId,

                Change = 0,
                MultiCurrencyChange = 0,

                TotalPayment = -payment.Total,
                MultiCurrencyTotalPayment = -payment.MultiCurrencyTotal,

                TotalPaymentCustomerCredit = payment.Total,
                MultiCurrencyTotalPaymentCustomerCredit = payment.MultiCurrencyTotal,

                TotalPaymentInvoice = 0,
                MultiCurrencyTotalPaymentInvoice = 0,

                TotalOpenBalance = 0,
                TotalOpenBalanceCustomerCredit = customerCredit.OpenBalance,

                TotalPaymentDue = 0,
                TotalPaymentDueCustomerCredit = customerCredit.OpenBalance - payment.Total,

                paymentDate = customerCreditInput.CreditDate,
                ReceivePaymentExpenseItems = null,

                PaymentMethodId = payment.PaymentMethodId,

                TotalCashInvoice = 0,
                TotalCreditInvoice = 0,
                TotalExpenseInvoice = 0,
                TotalCashCustomerCredit = payment.Total,
                TotalCreditCustomerCredit = 0,
                TotalExpenseCustomerCredit = 0,

                ReceivePaymentDetail = new List<ReceivePayments.Dto.ReceivePaymentDetail>()
                {
                   new ReceivePayments.Dto.ReceivePaymentDetail{
                        InvoiceId = null,
                        CustomerId = customerCredit.CustomerId,
                        DueDate = customerCredit.DueDate,
                        accountId = customerCreditInput.AccountId,
                        CustomerCreditId = customerCredit.Id,
                        Payment = payment.Total,
                        MultiCurrencyPayment = payment.MultiCurrencyTotal,
                        OpenBalance = customerCredit.OpenBalance,
                        MultiCurrencyOpenBalance = customerCredit.MultiCurrencyOpenBalance,
                        TotalAmount = customerCredit.OpenBalance - payment.Total,
                        MultiCurrencyTotalAmount = customerCredit.MultiCurrencyOpenBalance - payment.MultiCurrencyTotal,
                        Cash = payment.Total,
                        MultiCurrencyCash = payment.MultiCurrencyTotal,
                   }
                }
            };


            if (input.LocationId == null || input.LocationId == 0)
            {
                throw new UserFriendlyException(L("PleaseSelectLocation"));
            }

            var lossGainAmount = customerCredit.OpenBalance - payment.Total;

            //Check for Exchange Loss Gain
            if (customerCredit.MultiCurrencyOpenBalance == payment.MultiCurrencyTotal && lossGainAmount != 0)
            {
                var tenant = await GetCurrentTenantAsync();
                var customerCreditToRefunt = input.ReceivePaymentDetail.FirstOrDefault();

                customerCreditToRefunt.Payment += lossGainAmount;
                customerCreditToRefunt.TotalAmount -= lossGainAmount;

                input.TotalPaymentDueCustomerCredit -= lossGainAmount;
                input.TotalPaymentCustomerCredit += lossGainAmount;
                input.Change = -lossGainAmount;
                input.ReceivePaymentExpenseItems = new List<ReceivePayments.Dto.ReceivePaymentExpenseItem>();

                //Posting Exchange Loss Gain Account
                input.ReceivePaymentExpenseItems.Add(new ReceivePaymentExpenseItem
                {
                    AccountId = tenant.ExchangeLossAndGainId.Value,
                    Amount = -lossGainAmount,
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

            //var @invoices = new List<Invoice>();
            var @customerCredits = new List<CustomerCredit>();
            //if (input.ReceivePaymentDetail.Where(x => x.InvoiceId != null).Count() > 0)
            //{
            //    @invoices = await _invoiceRepository.GetAll().Where(x => input.ReceivePaymentDetail.Any(t => t.InvoiceId == x.Id)).ToListAsync();
            //}
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
                                            i.Cash, i.MultiCurrencyCash, i.Credit, i.MultiCurrencyCredit, i.Expense, i.MultiCurrencyExpense, i.LossGain,
                                            i.OpenBalanceInPaymentCurrency, i.PaymentInPaymentCurrency, i.TotalAmountInPaymentCurrency,
                                            i.CashInPaymentCurrency, i.CreditInPaymentCurrency, i.ExpenseInPaymentCurrency);
                CheckErrors(await _receivePaymentDetailManager.CreateAsync(receivePaymentDetail));

                //insert journal item into with credit  and default debit zero
                JournalItem inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity,
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

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_Invoice_CreateCustomerCredit, AppPermissions.Pages_Tenant_Customers_Invoice_UpdateSaleReturn)]
        public async Task<NullableIdDto<Guid>> RefundMore(POSPaymoreInput input)
        {
            var @journal = await _journalRepository
                                .GetAll()
                                .Include(u => u.CustomerCredit)
                                .Include(u => u.Location)
                                .Include(u => u.CreatorUser)
                                .Include(u => u.Currency)
                                .Include(u => u.MultiCurrency)
                                .Where(u => u.JournalType == JournalType.CustomerCredit && u.CustomerCreditId == input.Id)
                                .FirstOrDefaultAsync();

            if (@journal == null || @journal.CustomerCredit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            else if (@journal.CustomerCredit.PaidStatus == PaidStatuse.Paid)
            {
                throw new UserFriendlyException(L("InvoiceIsPaid"));
            }

            var account = await _journalItemRepository
                                .GetAll()
                                .Where(u => u.JournalId == journal.Id && u.Identifier == null && u.Key == PostingKey.AR)
                                .FirstOrDefaultAsync();

            var posInput = new CreateCustomerCreditInput
            {
                ClassId = journal.ClassId,
                CustomerId = journal.CustomerCredit.CustomerId,
                LocationId = journal.LocationId.Value,
                CurrencyId = journal.CurrencyId,
                MultiCurrencyId = journal.MultiCurrencyId,
                CreditDate = input.Date,
                IsConfirm = input.IsConfirm,
                AccountId = account.AccountId,
            };

            var customerCredit = journal.CustomerCredit;

            //prepare payments           
            foreach (var payment in input.ReceivePayments)
            {
                if (customerCredit.MultiCurrencyOpenBalance == 0) break;
                await Refund(payment, posInput, customerCredit);
            }


            return new NullableIdDto<Guid>() { Id = input.Id };
        }

        //@Helper Function for create item receipt customer credit 
        private async Task CreateItemReceiptCustomerCredit(CreateItemReceiptCustomerCreditInput input)
        {
            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }


            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                    .Where(t => (t.LockKey == TransactionLockType.ItemReceipt || t.LockKey == TransactionLockType.InventoryTransaction)
                    && t.IsLock == true && t.LockDate != null && t.LockDate.Value.Date >= input.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0 && input.IsConfirm == false)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt_CustomerCredit);

            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.ReceiptNo = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            //insert to journal
            var @entity = Journal.Create(
                tenantId, userId, input.ReceiptNo,
                input.Date, input.Memo, 0, input.Total,
                input.CurrencyId, input.ClassId, input.Reference, input.LocationId);
            entity.UpdateStatus(input.Status);
            #region journal transaction type 
            var transactionTypeId = await _journalTransactionTypeManager.GetJournalTransactionTypeId(InventoryTransactionType.ItemReceiptCustomerCredit);
            entity.SetJournalTransactionTypeId(transactionTypeId);
            #endregion

            #region Calculat Cost
            int digit = await _accountCycleRepository.GetAll()
                 .Where(t => t.TenantId == tenantId && t.EndDate == null)
                 .OrderByDescending(t => t.StartDate)
                 .Select(t => t.RoundingDigit).FirstOrDefaultAsync();
            var items = input.Items.GroupBy(s => s.ItemId).Select(s => s.FirstOrDefault().ItemId).ToList();
            var itemCostDic = await _inventoryManager.GetItemCostSummaryQuery(null, input.Date, new List<long?> { input.LocationId }, userId, items).ToDictionaryAsync(s => $"{s.ItemId}-{s.LocationId}", s => s);
            var itemLatestCostDic = await _inventoryManager.GetItemLatestCostSummaryQuery(input.Date, new List<long?> { input.LocationId }, items).ToDictionaryAsync(s => $"{s.ItemId}-{s.LocationId}", s => s);
            var itemPurchaseCostDic = await _itemRepository.GetAll().Where(s => items.Contains(s.Id)).Select(s => new { s.Id, Cost = s.PurchaseCost ?? 0 }).ToDictionaryAsync(s => s.Id, s => s.Cost);

            foreach (var r in input.Items)
            {
                var key = $"{r.ItemId}-{input.LocationId}";

                if (itemCostDic.ContainsKey(key) && itemCostDic[key].Qty > 0)
                {
                    r.UnitCost = itemCostDic[key].Cost;
                }
                else if (itemLatestCostDic.ContainsKey(key))
                {
                    r.UnitCost = itemLatestCostDic[key].Cost;
                }
                else
                {
                    r.UnitCost = itemPurchaseCostDic.ContainsKey(r.ItemId) ? itemPurchaseCostDic[r.ItemId] : 0;
                }

                r.Total = Math.Round(r.UnitCost * r.Qty, digit, MidpointRounding.AwayFromZero);
            }

            input.Total = input.Items.Sum(t => t.Total);
            #endregion Calculat Cost


            var itemReceiptCustomerCredit = ItemReceiptCustomerCredit.Create(
                                    tenantId, userId, input.CustomerCreditId,
                                    input.ReceiveFrom, input.CustomerId,
                                    input.SameAsShippingAddress,
                                    input.ShippingAddress, input.BillingAddress,
                                    input.Total,
                                    input.ItemIssueSaleId);

            itemReceiptCustomerCredit.UpdateTransactionType(InventoryTransactionType.ItemReceiptCustomerCredit);
            @entity.UpdateItemReceiptCustomerCredit(itemReceiptCustomerCredit);

            CheckErrors(await _journalManager.CreateAsync(@entity));
            // CheckErrors(await _journalItemManager.CreateAsync(clearanceJournalItem));
            CheckErrors(await _itemReceiptCustomerCreditManager.CreateAsync(itemReceiptCustomerCredit));

            foreach (var i in input.Items)
            {
                //insert to item Receipt item
                var @itemReceiptItem = ItemReceiptItemCustomerCredit.Create(tenantId, userId,
                    itemReceiptCustomerCredit.Id, i.CustomerCreditItemId,
                    i.ItemId, i.Description, i.Qty, i.UnitCost,
                    i.DiscountRate, i.Total,
                    i.ItemIssueSaleItemId);
                itemReceiptItem.UpdateLot(i.LotId);
                CheckErrors(await _itemReceiptCustomerCreditItemManager.CreateAsync(@itemReceiptItem));

                //update Clearance account 
                var clearanceJournalItem = JournalItem.CreateJournalItem(
                                tenantId, userId, entity, i.ClearanceAccountId,
                                i.Description, 0, i.Total, PostingKey.COGS, itemReceiptItem.Id);
                CheckErrors(await _journalItemManager.CreateAsync(clearanceJournalItem));

                //insert inventory journal item into debit
                var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId,
                    entity, i.InventoryAccountId,
                    i.Description, i.Total, 0,
                    PostingKey.Inventory, itemReceiptItem.Id);
                CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));

                if (i.ItemBatchNos != null && i.ItemBatchNos.Any())
                {
                    var addItemBatchNos = i.ItemBatchNos.Select(s => ItemReceiptCustomerCreditItemBatchNo.Create(tenantId, userId, itemReceiptItem.Id, s.BatchNoId, s.Qty)).ToList();
                    foreach (var a in addItemBatchNos)
                    {
                        await _itemReceiptCustomerCreditItemBatchNoRepository.InsertAsync(a);
                    }
                }
            }

            //update customer credit status 
            if (input.Status == TransactionStatus.Publish)
            {
                var customerCredit = await _customerCreditRepository
                                           .GetAll()
                                           .Where(u => u.Id == input.CustomerCreditId)
                                           .FirstOrDefaultAsync();
                customerCredit.UpdateShipedStatus(DeliveryStatus.ShipAll);
                CheckErrors(await _customerCreditManager.UpdateAsync(customerCredit));
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            await SyncItemReceiptCustomerCredit(itemReceiptCustomerCredit.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                var scheduleItems = input.Items.Select(s => s.ItemId).Distinct().ToList();
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, input.Date, scheduleItems);
            }

        }

        public async Task UpdateItemReceiptCustomerCredit(UpdateItemReceiptCustomerCreditInput input)
        {

            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }


            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            // update Journal 
            var @journal = await _journalRepository
                              .GetAll()
                              .Where(u => u.JournalType == JournalType.ItemReceiptCustomerCredit && u.ItemReceiptCustomerCreditId == input.Id)
                              .FirstOrDefaultAsync();


            if (input.IsConfirm == false)
            {
                var validateLockDate = await _lockRepository.GetAll()
                                     .Where(t => t.IsLock == true &&
                                     (t.LockDate.Value.Date >= journal.Date.Date || t.LockDate.Value.Date >= input.Date.Date)
                                     && (t.LockKey == TransactionLockType.ItemReceipt || t.LockKey == TransactionLockType.InventoryTransaction)).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.CustomerCredit);
            if (auto.CustomFormat == true)
            {
                input.ReceiptNo = journal.JournalNo;
            }

            var scheduleDate = input.Date > journal.Date ? journal.Date : input.Date;

            journal.Update(tenantId, input.ReceiptNo, input.Date, input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, 0, input.LocationId);
            journal.UpdateStatus(input.Status);

            #region Calculat Cost
            int digit = await _accountCycleRepository.GetAll()
                 .Where(t => t.TenantId == tenantId && t.EndDate == null)
                 .OrderByDescending(t => t.StartDate)
                 .Select(t => t.RoundingDigit).FirstOrDefaultAsync();
            var items = input.Items.GroupBy(s => s.ItemId).Select(s => s.FirstOrDefault().ItemId).ToList();
            var itemCostDic = await _inventoryManager.GetItemCostSummaryQuery(null, input.Date, new List<long?> { input.LocationId }, userId, items, new List<Guid> { input.Id }).ToDictionaryAsync(s => $"{s.ItemId}-{s.LocationId}", s => s);
            var itemLatestCostDic = await _inventoryManager.GetItemLatestCostSummaryQuery(input.Date, new List<long?> { input.LocationId }, items, new List<Guid> { input.Id }).ToDictionaryAsync(s => $"{s.ItemId}-{s.LocationId}", s => s);
            var itemPurchaseCostDic = await _itemRepository.GetAll().Where(s => items.Contains(s.Id)).Select(s => new { s.Id, Cost = s.PurchaseCost ?? 0 }).ToDictionaryAsync(s => s.Id, s => s.Cost);

            foreach (var r in input.Items)
            {
                var key = $"{r.ItemId}-{input.LocationId}";

                if (itemCostDic.ContainsKey(key) && itemCostDic[key].Qty > 0)
                {
                    r.UnitCost = itemCostDic[key].Cost;
                }
                else if (itemLatestCostDic.ContainsKey(key))
                {
                    r.UnitCost = itemLatestCostDic[key].Cost;
                }
                else
                {
                    r.UnitCost = itemPurchaseCostDic.ContainsKey(r.ItemId) ? itemPurchaseCostDic[r.ItemId] : 0;
                }

                r.Total = Math.Round(r.UnitCost * r.Qty, digit, MidpointRounding.AwayFromZero);
            }

            input.Total = input.Items.Sum(t => t.Total);
            #endregion Calculat Cost


            //update item receipt 
            var itemReceiptCustomerCredit = await _itemReceiptCustomerCreditManager.GetAsync(input.Id, true);

            if (itemReceiptCustomerCredit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            itemReceiptCustomerCredit.Update(tenantId, input.ReceiveFrom, input.CustomerId,
                          input.SameAsShippingAddress, input.ShippingAddress,
                          input.BillingAddress, input.CustomerCreditId, input.Total, input.ItemIssueSaleId);


            CheckErrors(await _journalManager.UpdateAsync(@journal, auto.DocumentType));


            CheckErrors(await _itemReceiptCustomerCreditManager.UpdateAsync(itemReceiptCustomerCredit));


            //Update Item Receipt customer credit Item and Journal Item
            var itemReceipItems = await _itemReceiptCustomerCreditItemRepository.GetAll().Where(u => u.ItemReceiptCustomerCreditId == input.Id).ToListAsync();

            var @inventoryJournalItems = await (_journalItemRepository.GetAll()
                                  .Where(u => u.JournalId == journal.Id && u.Identifier != null)).ToListAsync();

            var toDeleteItemReceiptItem = itemReceipItems
                .Where(u => !input.Items.Any(i => i.Id != null && i.Id == u.Id)).ToList();
            var toDeletejournalItem = inventoryJournalItems
                .Where(u => !input.Items.Any(i => u.Identifier != null && i.Id != null && i.Id == u.Identifier)).ToList();

            var itemBatchNos = await _itemReceiptCustomerCreditItemBatchNoRepository.GetAll().Where(s => itemReceipItems.Any(r => r.Id == s.ItemReceiptCustomerCreditItemId)).AsNoTracking().ToListAsync();
            if (toDeleteItemReceiptItem.Any())
            {
                var deleteItemBatchNos = itemBatchNos.Where(s => toDeleteItemReceiptItem.Any(r => r.Id == s.ItemReceiptCustomerCreditItemId)).ToList();
                if (deleteItemBatchNos.Any()) await _itemReceiptCustomerCreditItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
            }

            foreach (var c in input.Items)
            {
                if (c.Id != null) //update
                {
                    var itemReceipItem = itemReceipItems.FirstOrDefault(u => u.Id == c.Id);
                    var journalItem = @inventoryJournalItems.Where(s => s.Identifier == c.Id).ToList();
                    if (itemReceipItem != null)
                    {
                        //new
                        itemReceipItem.Update(tenantId, c.ItemId, c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total, c.ItemIssueSaleItemId);
                        itemReceipItem.UpdateLot(c.LotId);
                        CheckErrors(await _itemReceiptCustomerCreditItemManager.UpdateAsync(itemReceipItem));

                    }
                    if (journalItem != null && journalItem.Count() > 0)
                    {

                        foreach (var i in journalItem)
                        {
                            if (i.Key == PostingKey.Inventory)
                            {
                                // update inventory posting key credit
                                i.UpdateJournalItem(userId, c.InventoryAccountId, c.Description, c.Total, 0);
                                CheckErrors(await _journalItemManager.UpdateAsync(i));
                            }
                            if (i.Key == PostingKey.COGS)
                            {
                                // update cogs posting key debit
                                i.UpdateJournalItem(userId, c.ClearanceAccountId, c.Description, 0, c.Total);
                                CheckErrors(await _journalItemManager.UpdateAsync(i));
                            }
                        }
                    }

                    var oldItemBatchs = itemBatchNos.Where(s => s.ItemReceiptCustomerCreditItemId == c.Id).ToList();

                    if (c.ItemBatchNos != null && c.ItemBatchNos.Any())
                    {
                        var addItemBatchNos = c.ItemBatchNos
                            .Where(s => !s.Id.HasValue || s.Id == Guid.Empty || !oldItemBatchs.Any(r => r.BatchNoId == s.BatchNoId))
                            .Select(s => ItemReceiptCustomerCreditItemBatchNo.Create(tenantId, userId, itemReceipItem.Id, s.BatchNoId, s.Qty))
                            .ToList();
                        if (addItemBatchNos.Any())
                        {
                            foreach (var a in addItemBatchNos)
                            {
                                await _itemReceiptCustomerCreditItemBatchNoRepository.InsertAsync(a);
                            }
                        }

                        var updateItemBatchNos = oldItemBatchs.Where(s => c.ItemBatchNos.Any(r => r.BatchNoId == s.BatchNoId)).Select(s =>
                        {
                            var oldItemBatch = s;
                            var newBatch = c.ItemBatchNos.FirstOrDefault(r => r.BatchNoId == s.BatchNoId);

                            oldItemBatch.Update(userId, itemReceipItem.Id, newBatch.BatchNoId, newBatch.Qty);
                            return oldItemBatch;
                        }).ToList();

                        if (updateItemBatchNos.Any()) await _itemReceiptCustomerCreditItemBatchNoRepository.BulkUpdateAsync(updateItemBatchNos);

                        var deleteItemBatchNos = oldItemBatchs.Where(s => !c.ItemBatchNos.Any(r => r.BatchNoId == s.BatchNoId)).ToList();
                        if (deleteItemBatchNos.Any())
                        {
                            await _itemReceiptCustomerCreditItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
                        }
                    }
                    else if (oldItemBatchs.Any())
                    {
                        await _itemReceiptCustomerCreditItemBatchNoRepository.BulkDeleteAsync(oldItemBatchs);
                    }

                }
                else if (c.Id == null) //create
                {
                    //insert to item Receipt item
                    var @itemReceiptItem = ItemReceiptItemCustomerCredit.Create(tenantId, userId, itemReceiptCustomerCredit.Id, c.CustomerCreditItemId, c.ItemId,
                                                                 c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total, c.ItemIssueSaleItemId);
                    itemReceiptItem.UpdateLot(c.LotId);
                    CheckErrors(await _itemReceiptCustomerCreditItemManager.CreateAsync(@itemReceiptItem));

                    var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal, c.ClearanceAccountId, c.Description, 0, c.Total, PostingKey.COGS, itemReceiptItem.Id);
                    CheckErrors(await _journalItemManager.CreateAsync(clearanceJournalItem));

                    var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal,
                        c.InventoryAccountId, c.Description, c.Total, 0, PostingKey.Inventory, itemReceiptItem.Id);
                    CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));

                    if (c.ItemBatchNos != null && c.ItemBatchNos.Any())
                    {
                        var addItemBatchNos = c.ItemBatchNos.Select(s => ItemReceiptCustomerCreditItemBatchNo.Create(tenantId, userId, itemReceiptItem.Id, s.BatchNoId, s.Qty)).ToList();
                        foreach (var a in addItemBatchNos)
                        {
                            await _itemReceiptCustomerCreditItemBatchNoRepository.InsertAsync(a);
                        }
                    }
                }

            }


            //update customer credit status 
            if (input.Status == TransactionStatus.Publish)
            {
                var customerCredit = await _customerCreditRepository
                                           .GetAll()
                                           .Where(u => u.Id == input.CustomerCreditId)
                                           .FirstOrDefaultAsync();
                customerCredit.UpdateShipedStatus(DeliveryStatus.ShipAll);
                CheckErrors(await _customerCreditManager.UpdateAsync(customerCredit));
            }

            var scheduleItems = input.Items.Select(s => s.ItemId).Concat(toDeleteItemReceiptItem.Select(s => s.ItemId)).Distinct().ToList();

            foreach (var t in toDeleteItemReceiptItem.OrderBy(u => u.Id))
            {
                CheckErrors(await _itemReceiptCustomerCreditItemManager.RemoveAsync(t));
            }

            foreach (var t in toDeletejournalItem)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(t));
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            await SyncItemReceiptCustomerCredit(itemReceiptCustomerCredit.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, scheduleDate, scheduleItems);
            }

        }

        public async Task DeleteItemReceiptCustomerCredit(CarlEntityDto input)
        {

            var @jounal = await _journalRepository.GetAll()
                    .Include(u => u.ItemReceiptCustomerCredit)
                    .Include(u => u.ItemReceiptCustomerCredit.ShippingAddress)
                    .Include(u => u.ItemReceiptCustomerCredit.BillingAddress)
                    .Where(t => t.ItemReceiptCustomerCreditId == input.Id
                    && t.JournalType == JournalType.ItemReceiptCustomerCredit).FirstOrDefaultAsync();
            var @entity = @jounal.ItemReceiptCustomerCredit;
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                  .Where(t => (t.LockKey == TransactionLockType.ItemReceipt || t.LockKey == TransactionLockType.InventoryTransaction)
                  && t.IsLock == true && t.LockDate != null && t.LockDate.Value.Date >= jounal.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt_CustomerCredit);

            if (@jounal.JournalNo == auto.LastAutoSequenceNumber)
            {
                var jo = await _journalRepository.GetAll().Where(t => t.Id != @jounal.Id && t.JournalType == JournalType.ItemReceiptCustomerCredit)
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
            //  @jounal.UpdateItemReceiptCustomerCredit(null);

            //query get journal item and delete
            var @jounalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == jounal.Id).ToListAsync();
            foreach (var ji in jounalItems)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(ji));
            }

            var scheduleDate = jounal.Date;

            CheckErrors(await _journalManager.RemoveAsync(@jounal));

            var itemBatchNos = await _itemReceiptCustomerCreditItemBatchNoRepository.GetAll().Where(s => s.ItemReceiptCustomerCreditItem.ItemReceiptCustomerCreditId == input.Id).AsNoTracking().ToListAsync();
            if (itemBatchNos.Any())
            {
                await _itemReceiptCustomerCreditItemBatchNoRepository.BulkDeleteAsync(itemBatchNos);
            }

            //query get item receipt customer credit item and delete 
            var @itemReceiptItems = await _itemReceiptCustomerCreditItemRepository.GetAll()
                .Where(u => u.ItemReceiptCustomerCreditId == entity.Id).ToListAsync();

            var scheduleItems = itemReceiptItems.Select(s => s.ItemId).Distinct().ToList();

            foreach (var iri in @itemReceiptItems)
            {

                CheckErrors(await _itemReceiptCustomerCreditItemManager.RemoveAsync(iri));
            }

            CheckErrors(await _itemReceiptCustomerCreditManager.RemoveAsync(entity));

            await CurrentUnitOfWork.SaveChangesAsync();

            await DeleteInventoryTransactionItems(input.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, scheduleDate, scheduleItems);
            }

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_Delete, AppPermissions.Pages_Tenant_Common_DeleteVoid)]
        public async Task Delete(CarlEntityDto input)
        {
            // validate from Payment
            var validateReceivePayment = await _receivePaymentDetailRepository.GetAll().AsNoTracking()
                                        .Where(t => t.CustomerCreditId == input.Id).CountAsync();

            if (validateReceivePayment > 0)
            {
                throw new UserFriendlyException(L("RecordHasReceivePayment"));
            }

            var journal = await _journalRepository.GetAll()
                .Include(u => u.CustomerCredit)
                .Include(u => u.CustomerCredit.ShippingAddress)
                .Include(u => u.CustomerCredit.BillingAddress)
                .Where(u => u.JournalType == JournalType.CustomerCredit && u.CustomerCreditId == input.Id)
                .FirstOrDefaultAsync();
            var @entity = journal.CustomerCredit;

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

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.CustomerCredit);

            if (journal.JournalNo == auto.LastAutoSequenceNumber)
            {
                var jo = await _journalRepository.GetAll().Where(t => t.Id != journal.Id && t.JournalType == JournalType.CustomerCredit)
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


            var exchangeRates = await _exchangeRateRepository.GetAll().AsNoTracking().Where(s => s.CustomerCreditId == input.Id).ToListAsync();
            if (exchangeRates.Any()) await _exchangeRateRepository.BulkDeleteAsync(exchangeRates);


            var validateCusotmerCredit = await _itemReceiptCustomerCreditReository.GetAll()
                                          .Include(t => t.CustomerCredit)
                                          .Where(t => t.CustomerCreditId == input.Id).FirstOrDefaultAsync();

            if (entity.ConvertToItemReceipt == false &&
                validateCusotmerCredit != null &&
                validateCusotmerCredit.ReceiveFrom == ReceiveFrom.CustomerCredit)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            else if (validateCusotmerCredit != null && entity.ConvertToItemReceipt == false && entity.ReceiveFrom == ReceiveFrom.ItemReceiptCustomerCredit)
            {

                validateCusotmerCredit.CustomerCredit.UpdateShipedStatus(DeliveryStatus.ShipPending);
                validateCusotmerCredit.UpdateCustomerCredit(null);
                CheckErrors(await _customerCreditManager.UpdateAsync(validateCusotmerCredit.CustomerCredit));
                CheckErrors(await _itemReceiptCustomerCreditManager.UpdateAsync(validateCusotmerCredit));

            }

            else if ((journal.CustomerCredit.IsPOS || journal.Status == TransactionStatus.Publish) && entity.ConvertToItemReceipt == true)
            {

                var inputItemReceipt = new CarlEntityDto() { IsConfirm = input.IsConfirm, Id = validateCusotmerCredit.Id };
                await DeleteItemReceiptCustomerCredit(inputItemReceipt);

                //var draftInput = new UpdateStatus();
                //draftInput.Id = input.Id;
                //await UpdateStatusToDraft(draftInput);
            }

            journal.UpdateCustomerCredit(null);

            //query get journal item and delete
            var @jounalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == journal.Id).ToListAsync();
            foreach (var ji in jounalItems)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(ji));
            }
            CheckErrors(await _journalManager.RemoveAsync(journal));


            var itemBatchNos = await _customerCreditItemBatchNoRepository.GetAll().Where(s => s.CustomerCreditItem.CustomerCreditId == input.Id).AsNoTracking().ToListAsync();
            if (itemBatchNos.Any())
            {
                await _customerCreditItemBatchNoRepository.BulkDeleteAsync(itemBatchNos);
            }

            //query get customer credit detail and delete 
            var customerCreditItems = await _customerCreditDetailRepository.GetAll()
                .Where(u => u.CustomerCreditId == entity.Id).ToListAsync();


            var oldOrderIds = new List<Guid>();
            var oldDeliveryIds = new List<Guid>();

            var itemIssueItemIds = customerCreditItems.Where(s => s.ItemIssueSaleItemId.HasValue).Select(s => s.ItemIssueSaleItemId.Value).ToList();
            if (itemIssueItemIds.Any())
            {
                var orderFromIssue = await _itemIssueItemRepository.GetAll()
                                           .Where(s => s.SaleOrderItemId.HasValue)
                                           .Where(s => itemIssueItemIds.Contains(s.Id))
                                           .AsNoTracking()
                                           .Select(s => s.SaleOrderItem.SaleOrderId)
                                           .ToListAsync();

                var orderFromInvoice = await _invoiceItemRepository.GetAll()
                                             .Where(s => s.OrderItemId.HasValue)
                                             .Where(s => s.ItemIssueItemId.HasValue)
                                             .Where(s => itemIssueItemIds.Contains(s.ItemIssueItemId.Value))
                                             .AsNoTracking()
                                             .Select(s => s.SaleOrderItem.SaleOrderId)
                                             .ToListAsync();

                oldOrderIds = orderFromIssue.Concat(orderFromInvoice).Distinct().ToList();

                var deliveryFromIssue = await _itemIssueItemRepository.GetAll()
                                          .Where(s => s.DeliverySchedulItemId.HasValue)
                                          .Where(s => itemIssueItemIds.Contains(s.Id))
                                          .AsNoTracking()
                                          .Select(s => s.DeliveryScheduleItem.DeliveryScheduleId)
                                          .ToListAsync();

                var deliveryFromInvoice = await _invoiceItemRepository.GetAll()
                                             .Where(s => s.DeliverySchedulItemId.HasValue)
                                             .Where(s => s.ItemIssueItemId.HasValue)
                                             .Where(s => itemIssueItemIds.Contains(s.ItemIssueItemId.Value))
                                             .AsNoTracking()
                                             .Select(s => s.DeliveryScheduleItem.DeliveryScheduleId)
                                             .ToListAsync();

                oldDeliveryIds = deliveryFromIssue.Concat(deliveryFromInvoice).Distinct().ToList();

            }
            else
            {
                var oldCreditItemIds = customerCreditItems.Select(s => s.Id).ToList();
                var itemIssueFromItemReceiptIds = await _itemReceiptCustomerCreditItemRepository.GetAll()
                                                       .Where(s => s.CustomerCreditItemId.HasValue)
                                                       .Where(s => oldCreditItemIds.Contains(s.CustomerCreditItemId.Value))
                                                       .Where(s => s.ItemIssueSaleItemId.HasValue)
                                                       .AsNoTracking()
                                                       .Select(s => s.ItemIssueSaleItemId.Value)
                                                       .ToListAsync();

                var orderFromIssue = await _itemIssueItemRepository.GetAll()
                                           .Where(s => s.SaleOrderItemId.HasValue)
                                           .Where(s => itemIssueFromItemReceiptIds.Contains(s.Id))
                                           .AsNoTracking()
                                           .Select(s => s.SaleOrderItem.SaleOrderId)
                                           .ToListAsync();

                var orderFromInvoice = await _invoiceItemRepository.GetAll()
                                             .Where(s => s.OrderItemId.HasValue)
                                             .Where(s => s.ItemIssueItemId.HasValue)
                                             .Where(s => itemIssueFromItemReceiptIds.Contains(s.ItemIssueItemId.Value))
                                             .AsNoTracking()
                                             .Select(s => s.SaleOrderItem.SaleOrderId)
                                             .ToListAsync();

                oldOrderIds = orderFromIssue.Concat(orderFromInvoice).Distinct().ToList();


                var deliveryFromIssue = await _itemIssueItemRepository.GetAll()
                                           .Where(s => s.DeliverySchedulItemId.HasValue)
                                           .Where(s => itemIssueFromItemReceiptIds.Contains(s.Id))
                                           .AsNoTracking()
                                           .Select(s => s.DeliveryScheduleItem.DeliveryScheduleId)
                                           .ToListAsync();

                var deliveryFromInvoice = await _invoiceItemRepository.GetAll()
                                             .Where(s => s.DeliverySchedulItemId.HasValue)
                                             .Where(s => s.ItemIssueItemId.HasValue)
                                             .Where(s => itemIssueFromItemReceiptIds.Contains(s.ItemIssueItemId.Value))
                                             .AsNoTracking()
                                             .Select(s => s.DeliveryScheduleItem.DeliveryScheduleId)
                                             .ToListAsync();
                oldDeliveryIds = deliveryFromIssue.Concat(deliveryFromInvoice).Distinct().ToList();
            }


            var itemIssueItem = await _itemReceiptCustomerCreditItemRepository.GetAll().Where(t => validateCusotmerCredit != null && (t.ItemReceiptCustomerCreditId == validateCusotmerCredit.Id)).ToListAsync();

            foreach (var i in customerCreditItems)
            {
                var updateCutomerCreditItem = itemIssueItem.Where(t => t.CustomerCreditItemId == i.Id).FirstOrDefault();

                if (updateCutomerCreditItem != null)
                {
                    updateCutomerCreditItem.UpdateCustomerCreditItemId(null);
                    CheckErrors(await _itemReceiptCustomerCreditItemManager.UpdateAsync(updateCutomerCreditItem));
                }

                CheckErrors(await _customerCreditDetailManager.RemoveAsync(i));
            }

            //if (entity.IsPOS)
            //{
            //    var itemReceiptCustomerCredit = await _itemReceiptCustomerCreditReository.GetAll().FirstOrDefaultAsync(s => s.CustomerCreditId == entity.Id);
            //    if (itemReceiptCustomerCredit == null) throw new UserFriendlyException(L("RecordNotFound"));

            //    var itemReceiptCustomerCreditItems = await _itemReceiptCustomerCreditItemRepository.GetAll().Where(s => s.CustomerCreditItemId == itemReceiptCustomerCredit.Id).ToListAsync();

            //    var itemReceiptCustomerCreditJournal = await _journalRepository.GetAll().FirstOrDefaultAsync(s => s.ItemReceiptCustomerCreditId == itemReceiptCustomerCredit.Id);
            //    if (itemReceiptCustomerCreditJournal == null) throw new UserFriendlyException(L("RecordNotFound"));
            //    var itemReceiptCustomerCreditJournalItems = await _journalItemRepository.GetAll().Where(s => s.JournalId == itemReceiptCustomerCreditJournal.Id).ToListAsync();

            //    foreach(var ji in itemReceiptCustomerCreditJournalItems)
            //    {
            //        CheckErrors(await _journalItemManager.RemoveAsync(ji));
            //    }

            //    CheckErrors(await _journalManager.RemoveAsync(itemReceiptCustomerCreditJournal));

            //    foreach (var iri in itemReceiptCustomerCreditItems)
            //    {
            //        CheckErrors(await _itemReceiptCustomerCreditItemRepository.DeleteAsync(iri));
            //    }

            //    CheckErrors(await _journalItemManager.RemoveAsync(ji));

            //}


            if (oldOrderIds.Any() || oldDeliveryIds.Any())
            {
                await this.CurrentUnitOfWork.SaveChangesAsync();
                foreach (var id in oldOrderIds)
                {
                    await UpdateOrderInventoryStatus(id);
                }
                foreach (var id in oldDeliveryIds)
                {
                    await UpdateDeliveryInventoryStatus(id, true);
                }
            }


            CheckErrors(await _customerCreditManager.RemoveAsync(entity));
            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.Invoice };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_GetDetail, AppPermissions.Pages_Tenant_Customers_Invoice_ViewSaleReturn)]
        public async Task<CustomerCreditDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            await TurnOffTenantFilterIfDebug();

            var @journal = await _journalRepository
                                .GetAll()
                                .Include(u => u.Currency)
                                .Include(u => u.CustomerCredit)
                                .Include(u => u.Class)
                                .Include(u => u.MultiCurrency)
                                .Include(u => u.Location)
                                .Include(u => u.CustomerCredit.Customer)
                                .Include(u => u.CustomerCredit.ShippingAddress)
                                .Include(u => u.CustomerCredit.BillingAddress)
                                .AsNoTracking()
                                .Where(u => u.JournalType == JournalType.CustomerCredit && u.CustomerCreditId == input.Id)
                                .FirstOrDefaultAsync();

            if (@journal == null || @journal.CustomerCredit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var arAccount = await (_journalItemRepository.GetAll()
                                       .Include(u => u.Account)
                                       .AsNoTracking()
                                       .Where(u => u.JournalId == journal.Id &&
                                                u.Key == PostingKey.AR && u.Identifier == null)
                                       .Select(u => u.Account)).FirstOrDefaultAsync();

            var result = ObjectMapper.Map<CustomerCreditDetailOutput>(journal.CustomerCredit);
            #region old code           
            var itemDetails = await _customerCreditDetailRepository.GetAll()
                .Include(u => u.Tax)
                .Include(u => u.Item)
                .Include(u => u.Tax)
                .Include(u => u.Lot)
                .Where(u => u.CustomerCreditId == input.Id)
                .Join(_journalItemRepository.GetAll().Include(u => u.Account).AsNoTracking(), u => u.Id, s => s.Identifier,
                (vItem, jItem) => new { vItem = vItem, jItem = jItem })
                .GroupJoin(_itemReceiptCustomerCreditItemRepository.GetAll().
                WhereIf(result.ReceiptCustomerCreditId != null, t => t.ItemReceiptCustomerCreditId == result.ReceiptCustomerCreditId).AsNoTracking(), v => v.vItem.Id, r => r.CustomerCreditItemId,
                    (v, r) => new { vItem = v.vItem, jItem = v.jItem, ivItem = r.DefaultIfEmpty() })
                .SelectMany(s => s.ivItem.Select(r =>
                new CustomerCreditDetailInput()
                {
                    LotId = s.vItem.LotId,
                    LotDetail = ObjectMapper.Map<LotSummaryOutput>(s.vItem.Lot),
                    Id = s.vItem.Id,
                    Item = ObjectMapper.Map<ItemSummaryOutput>(s.vItem.Item),
                    ItemId = s.vItem.ItemId,
                    AccountId = s.jItem.AccountId,
                    Account = ObjectMapper.Map<ChartAccountSummaryOutput>(s.jItem.Account),
                    Description = s.vItem.Description,
                    DiscountRate = s.vItem.DiscountRate,
                    Qty = s.vItem.Qty,
                    Tax = ObjectMapper.Map<TaxSummaryOutput>(s.vItem.Tax),
                    Total = s.vItem.Total,
                    UnitCost = s.vItem.UnitCost,
                    TaxId = s.vItem.TaxId,
                    MultiCurrencyTotal = s.vItem.MultiCurrencyTotal,
                    MultiCurrencyUnitCost = s.vItem.MultiCurrencyUnitCost,
                    ItemIssueSaleItemId = s.vItem.ItemIssueSaleItemId,
                    ItemReceiptCustomerCreditItemId = r.Id,
                    CreationTime = s.jItem.CreationTime,
                    SalePrice = s.vItem.SalePrice,
                    UseBatchNo = s.vItem.Item != null && s.vItem.Item.UseBatchNo,
                    TrackSerial = s.vItem.Item != null && s.vItem.Item.TrackSerial,
                    TrackExpiration = s.vItem.Item != null && s.vItem.Item.TrackExpiration
                }).OrderBy(u => u.CreationTime))
                .ToListAsync();


            if (journal.CustomerCredit.ReceiveFrom == ReceiveFrom.ItemIssueSale)
            {
                var itemIssueItemIds = itemDetails.Where(s => s.ItemIssueSaleItemId.HasValue).Select(s => s.ItemIssueSaleItemId.Value).ToList();

                if (itemIssueItemIds.Any())
                {
                    var itemIssueList = await _itemIssueItemRepository.GetAll()
                                              .Include(u => u.DeliveryScheduleItem.DeliverySchedule.SaleOrder)
                                              .Where(s => itemIssueItemIds.Contains(s.Id))
                                              .Where(s => s.SaleOrderItemId.HasValue || s.DeliverySchedulItemId.HasValue)
                                              .AsNoTracking()
                                              .Select(s => new
                                              {
                                                  itemIssueItemId = s.Id,
                                                  SaleOrderId = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrderId : (Guid?)null,
                                                  OrderNumber = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrder.OrderNumber : s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.OrderNumber : null,
                                                  OrderRefe = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrder.Reference : s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.Reference : null,
                                                  DeliveryId = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliveryScheduleId : (Guid?)null,
                                                  DeliveryNo = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.DeliveryNo : null,
                                                  DeliveryRef = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.Reference : null,
                                              }).ToListAsync();
                    if (itemIssueList.Any())
                    {
                        itemDetails = itemDetails.Select(s =>
                        {
                            var i = s;
                            var order = itemIssueList.FirstOrDefault(o => o.itemIssueItemId == s.ItemIssueSaleItemId);
                            if (order != null)
                            {
                                i.OrderId = order.SaleOrderId;
                                i.OrderNo = order.OrderNumber;
                                i.OrderRef = order.OrderRefe;
                                i.DeliveryNo = order.DeliveryNo;
                                i.DeliveryId = order.DeliveryId;
                                i.DeliveryRef = order.DeliveryRef;
                            }

                            return i;
                        })
                        .ToList();
                    }
                    else
                    {
                        var invoiceItemList = await _invoiceItemRepository.GetAll()
                                                    .Where(s => s.ItemIssueItemId.HasValue)
                                                    .Where(s => itemIssueItemIds.Contains(s.ItemIssueItemId.Value))
                                                    .Where(s => s.OrderItemId.HasValue || s.DeliverySchedulItemId.HasValue)
                                                    .AsNoTracking()
                                                    .Select(s => new
                                                    {
                                                        itemIssueItemId = s.ItemIssueItemId,
                                                        SaleOrderId = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrderId : (Guid?)null,
                                                        OrderNumber = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrder.OrderNumber : s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.OrderNumber : null,
                                                        OrderRefe = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrder.Reference : s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.Reference : null,
                                                        DeliveryId = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliveryScheduleId : (Guid?)null,
                                                        DeliveryNo = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.DeliveryNo : null,
                                                        DeliveryRef = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.Reference : null,
                                                    })
                                                    .ToListAsync();
                        if (invoiceItemList.Any())
                        {
                            itemDetails = itemDetails.Select(s =>
                            {
                                var i = s;
                                var order = invoiceItemList.FirstOrDefault(o => o.itemIssueItemId == s.ItemIssueSaleItemId);
                                if (order != null)
                                {
                                    i.OrderId = order.SaleOrderId;
                                    i.OrderNo = order.OrderNumber;
                                    i.OrderRef = order.OrderRefe;
                                    i.DeliveryId = order.DeliveryId;
                                    i.DeliveryNo = order.DeliveryNo;
                                    i.DeliveryRef = order.DeliveryRef;
                                }

                                return i;
                            })
                       .ToList();
                        }
                    }
                }

            }
            else if (journal.CustomerCredit.ReceiveFrom == ReceiveFrom.ItemReceiptCustomerCredit)
            {
                var itemReceiptItemIds = itemDetails.Where(s => s.ItemReceiptCustomerCreditItemId.HasValue)
                                        .Select(s => s.ItemReceiptCustomerCreditItemId.Value).ToList();

                if (itemReceiptItemIds.Any())
                {
                    var itemReceiptList = await _itemReceiptCustomerCreditItemRepository.GetAll()
                                                    .Where(s => s.ItemIssueSaleItemId.HasValue)
                                                    .Where(s => itemReceiptItemIds.Contains(s.Id))
                                                    .AsNoTracking()
                                                    .Select(s => new
                                                    {
                                                        itemIssueItemId = s.ItemIssueSaleItemId.Value,
                                                        itemReceiptItemId = s.Id
                                                    })
                                                    .ToListAsync();

                    if (itemReceiptList.Any())
                    {
                        var itemIssueList = await _itemIssueItemRepository.GetAll()
                                              .Include(u => u.DeliveryScheduleItem.DeliverySchedule.SaleOrder)
                                              .Where(s => itemReceiptList.Any(r => r.itemIssueItemId == s.Id))
                                              .Where(s => s.SaleOrderItemId.HasValue || s.DeliverySchedulItemId.HasValue)
                                              .AsNoTracking()
                                              .Select(s => new
                                              {
                                                  itemIssueItemId = s.Id,
                                                  SaleOrderId = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrderId : (Guid?)null,
                                                  OrderNumber = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrder.OrderNumber : s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.OrderNumber : null,
                                                  OrderRefe = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrder.Reference : s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.Reference : null,
                                                  DeliveryId = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliveryScheduleId : (Guid?)null,
                                                  DeliveryNo = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.DeliveryNo : null,
                                                  DeliveryRef = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.Reference : null,
                                              })
                                              .ToListAsync();
                        if (itemIssueList.Any())
                        {
                            itemDetails = itemDetails.Select(s =>
                            {
                                var i = s;
                                var receiptItem = itemReceiptList.FirstOrDefault(r => r.itemReceiptItemId == s.ItemReceiptCustomerCreditItemId);
                                var order = itemIssueList.FirstOrDefault(o => o.itemIssueItemId == receiptItem.itemIssueItemId);
                                if (order != null)
                                {
                                    i.OrderId = order.SaleOrderId;
                                    i.OrderNo = order.OrderNumber;
                                    i.OrderRef = order.OrderRefe;
                                    i.DeliveryId = order.DeliveryId;
                                    i.DeliveryNo = order.DeliveryNo;
                                    i.DeliveryRef = order.DeliveryRef;
                                }

                                return i;
                            })
                            .ToList();
                        }
                        else
                        {
                            var invoiceItemList = await _invoiceItemRepository.GetAll()
                                                        .Where(s => s.ItemIssueItemId.HasValue)
                                                        .Where(s => itemReceiptList.Any(r => r.itemIssueItemId == s.ItemIssueItemId))
                                                        .Where(s => s.OrderItemId.HasValue || s.DeliverySchedulItemId.HasValue)
                                                        .AsNoTracking()
                                                        .Select(s => new
                                                        {
                                                            itemIssueItemId = s.ItemIssueItemId,
                                                            SaleOrderId = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrderId : (Guid?)null,
                                                            OrderNumber = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrder.OrderNumber : s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.OrderNumber : null,
                                                            OrderRefe = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrder.Reference : s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.Reference : null,
                                                            DeliveryId = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliveryScheduleId : (Guid?)null,
                                                            DeliveryNo = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.DeliveryNo : null,
                                                            DeliveryRef = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.Reference : null,
                                                        })
                                                        .ToListAsync();
                            if (invoiceItemList.Any())
                            {
                                itemDetails = itemDetails.Select(s =>
                                {
                                    var i = s;
                                    var receiptItem = itemReceiptList.FirstOrDefault(r => r.itemReceiptItemId == s.ItemReceiptCustomerCreditItemId);
                                    var order = invoiceItemList.FirstOrDefault(o => o.itemIssueItemId == receiptItem.itemIssueItemId);
                                    if (order != null)
                                    {
                                        i.OrderId = order.SaleOrderId;
                                        i.OrderNo = order.OrderNumber;
                                        i.OrderRef = order.OrderRefe;
                                        i.DeliveryId = order.DeliveryId;
                                        i.DeliveryNo = order.DeliveryNo;
                                        i.DeliveryRef = order.DeliveryRef;
                                    }

                                    return i;
                                })
                           .ToList();
                            }
                        }
                    }
                }
            }



            #endregion

            var jr = await _journalRepository.GetAll()
                .Include(u => u.ItemIssue)
                .Include(u => u.ItemReceiptCustomerCredit)
                .Where(u => (u.ItemIssueId == journal.CustomerCredit.ItemIssueSaleId && journal.CustomerCredit.ItemIssueSaleId != null)
                || (u.ItemReceiptCustomerCredit.CustomerCreditId == input.Id && u.ItemReceiptCustomerCreditId != null)).AsNoTracking().ToListAsync();


            foreach (var i in jr)
            {
                if (i.ItemIssue != null && i.ItemIssueId != null)
                {
                    result.IssueNo = i.JournalNo;
                    result.IssueDate = i.Date;
                    result.IssueSaleId = i.ItemIssueId;

                }
                else if (i.ItemReceiptCustomerCreditId != null)
                {
                    result.ReceiptCustomerCreditNo = i.JournalNo;
                    result.ReceiptDate = i.Date;
                    result.ReceiptCustomerCreditId = i.ItemReceiptCustomerCreditId;
                    result.ItemReceiptReference = i.Reference;
                }
            }


            var batchDic = await _customerCreditItemBatchNoRepository.GetAll()
                            .AsNoTracking()
                            .Where(s => s.CustomerCreditItem.CustomerCreditId == input.Id)
                            .Select(s => new BatchNoItemOutput
                            {
                                Id = s.Id,
                                BatchNoId = s.BatchNoId,
                                BatchNumber = s.BatchNo.Code,
                                ExpirationDate = s.BatchNo.ExpirationDate,
                                Qty = s.Qty,
                                TransactionItemId = s.CustomerCreditItemId
                            })
                            .GroupBy(s => s.TransactionItemId)
                            .ToDictionaryAsync(k => k.Key, v => v.ToList());
            if (batchDic.Any())
            {
                foreach (var i in itemDetails)
                {
                    if (batchDic.ContainsKey(i.Id.Value)) i.ItemBatchNos = batchDic[i.Id.Value];
                }
            }


            if (result.UseExchangeRate)
            {
                result.ExchangeRate = await _exchangeRateRepository.GetAll().AsNoTracking()
                                            .Where(s => s.CustomerCreditId == input.Id)
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

            result.ReceiveFrom = journal.CustomerCredit.ReceiveFrom;
            result.CreditNo = journal.JournalNo;
            result.CreditDate = journal.Date;
            result.DueDate = journal.CustomerCredit.DueDate;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.Reference = journal.Reference;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.MultiCurrency = ObjectMapper.Map<CurrencyDetailOutput>(journal.MultiCurrency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.ItemDetail = itemDetails;
            result.Account = ObjectMapper.Map<ChartAccountSummaryOutput>(arAccount);
            result.AccountId = arAccount.Id;
            result.Memo = journal.Memo;
            result.Status = journal.Status;
            result.LocationId = journal.LocationId.Value;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            result.StatusName = journal.Status.ToString();
            return result;
        }





        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_GetDetail, AppPermissions.Pages_Tenant_Customers_Invoice_ViewSaleReturn)]
        public async Task<CustomerCreditDetailOutput> GetDetailForPOS(EntityDto<Guid> input)
        {
            var @journal = await _journalRepository
                                .GetAll()
                                .Include(u => u.CreatorUser)
                                .Include(u => u.Currency)
                                .Include(u => u.CustomerCredit)
                                .Include(u => u.Class)
                                .Include(u => u.MultiCurrency)
                                .Include(u => u.Location)
                                .Include(u => u.CustomerCredit.Customer)
                                .Include(u => u.CustomerCredit.ShippingAddress)
                                .Include(u => u.CustomerCredit.BillingAddress)
                                .AsNoTracking()
                                .Where(u => u.JournalType == JournalType.CustomerCredit && u.CustomerCreditId == input.Id)
                                .FirstOrDefaultAsync();

            if (@journal == null || @journal.CustomerCredit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var arAccount = await (_journalItemRepository.GetAll()
                                       .Include(u => u.Account)
                                       .AsNoTracking()
                                       .Where(u => u.JournalId == journal.Id &&
                                                u.Key == PostingKey.AR && u.Identifier == null)
                                       .Select(u => u.Account)).FirstOrDefaultAsync();

            var result = ObjectMapper.Map<CustomerCreditDetailOutput>(journal.CustomerCredit);
            #region old code           
            var itemDetails = await _customerCreditDetailRepository.GetAll()
                .Include(u => u.Tax)
                .Include(u => u.Item)
                .Include(u => u.Tax)
                .Include(u => u.Lot)
                .Where(u => u.CustomerCreditId == input.Id)
                .Join(_journalItemRepository.GetAll().Include(u => u.Account).AsNoTracking(), u => u.Id, s => s.Identifier,
                (vItem, jItem) => new { vItem = vItem, jItem = jItem })
                .GroupJoin(_itemReceiptCustomerCreditItemRepository.GetAll().
                WhereIf(result.ReceiptCustomerCreditId != null, t => t.ItemReceiptCustomerCreditId == result.ReceiptCustomerCreditId).AsNoTracking(), v => v.vItem.Id, r => r.CustomerCreditItemId,
                    (v, r) => new { vItem = v.vItem, jItem = v.jItem, ivItem = r.DefaultIfEmpty() })
                .SelectMany(s => s.ivItem.Select(r =>
                new CustomerCreditDetailInput()
                {
                    LotId = s.vItem.LotId,
                    LotDetail = ObjectMapper.Map<LotSummaryOutput>(s.vItem.Lot),
                    Id = s.vItem.Id,
                    Item = ObjectMapper.Map<ItemSummaryOutput>(s.vItem.Item),
                    ItemId = s.vItem.ItemId,
                    AccountId = s.jItem.AccountId,
                    Account = ObjectMapper.Map<ChartAccountSummaryOutput>(s.jItem.Account),
                    Description = s.vItem.Description,
                    DiscountRate = s.vItem.DiscountRate,
                    Qty = s.vItem.Qty,
                    Tax = ObjectMapper.Map<TaxSummaryOutput>(s.vItem.Tax),
                    Total = s.vItem.Total,
                    UnitCost = s.vItem.UnitCost,
                    TaxId = s.vItem.TaxId,
                    MultiCurrencyTotal = s.vItem.MultiCurrencyTotal,
                    MultiCurrencyUnitCost = s.vItem.MultiCurrencyUnitCost,
                    ItemIssueSaleItemId = s.vItem.ItemIssueSaleItemId,
                    ItemReceiptCustomerCreditItemId = r.Id,
                    CreationTime = s.jItem.CreationTime,
                    SalePrice = s.vItem.SalePrice,
                    ItemIssueId = journal.ItemIssueId,
                }).OrderBy(u => u.CreationTime))
                .ToListAsync();
            #endregion

            var jr = await _journalRepository.GetAll()
                .Include(u => u.ItemIssue)
                .Include(u => u.ItemReceiptCustomerCredit)
                .Where(u => (u.ItemIssueId == journal.CustomerCredit.ItemIssueSaleId && journal.CustomerCredit.ItemIssueSaleId != null)
                || (u.ItemReceiptCustomerCredit.CustomerCreditId == input.Id && u.ItemReceiptCustomerCreditId != null)).AsNoTracking().ToListAsync();


            foreach (var i in jr)
            {
                if (i.ItemIssue != null && i.ItemIssueId != null)
                {
                    result.IssueNo = i.JournalNo;
                    result.IssueDate = i.Date;
                    result.IssueSaleId = i.ItemIssueId;

                }
                else if (i.ItemReceiptCustomerCreditId != null)
                {
                    result.ReceiptCustomerCreditNo = i.JournalNo;
                    result.ReceiptDate = i.Date;
                    result.ReceiptCustomerCreditId = i.ItemReceiptCustomerCreditId;
                }
            }

            var paymentSummaries = await (from p in _journalRepository.GetAll()
                          .Include(s => s.ReceivePayment.PaymentMethod.PaymentMethodBase)
                          .Where(s => s.JournalType == JournalType.ReceivePayment)
                          .AsNoTracking()
                                          join pi in _receivePaymentDetailRepository.GetAll()
                                          .Where(s => s.CustomerCreditId == input.Id)
                                          .AsNoTracking()
                                          on p.ReceivePaymentId equals pi.ReceivePaymentId
                                          group pi by new { p.Date.Date, p.ReceivePayment.PaymentMethod.PaymentMethodBase.Name, p.ReceivePayment.PaymentMethod.PaymentMethodBase.Icon } into g
                                          select new POSPaymentSummaryByPaymentMethodOutPut
                                          {
                                              Date = g.Key.Date,
                                              Icon = g.Key.Icon,
                                              PaymentMethod = g.Key.Name,
                                              Total = g.Sum(s => s.Payment),
                                              MultiCurrencyTotal = g.Sum(u => u.MultiCurrencyPayment),

                                          })
                          .ToListAsync();
            result.Charge = paymentSummaries.Sum(t => t.MultiCurrencyTotal);
            result.PaymentSummaries = paymentSummaries;
            result.ReceiveFrom = journal.CustomerCredit.ReceiveFrom;
            result.CreditNo = journal.JournalNo;
            result.CreditDate = journal.Date;
            result.DueDate = journal.CustomerCredit.DueDate;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.Reference = journal.Reference;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.MultiCurrency = ObjectMapper.Map<CurrencyDetailOutput>(journal.MultiCurrency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.ItemDetail = itemDetails;
            result.Account = ObjectMapper.Map<ChartAccountSummaryOutput>(arAccount);
            result.AccountId = arAccount.Id;
            result.Memo = journal.Memo;
            result.Status = journal.Status;
            result.CreateUser = journal.CreatorUser.FullName;
            result.PaidStatusName = journal.CustomerCredit.PaidStatus.ToString();
            result.LocationId = journal.LocationId.Value;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            result.StatusName = journal.Status.ToString();
            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_Update)]
        public async Task<NullableIdDto<Guid>> Update(UpdateCustomerCreditInput input)
        {
            if (input.IsConfirm == false)
            {
                var validateLockDate = await _lockRepository.GetAll()
                                   .Where(t => t.IsLock == true &&
                                   (t.LockDate.Value.Date >= input.DateCompare.Value.Date || t.LockDate.Value.Date >= input.CreditDate.Date)
                                   && t.LockKey == TransactionLockType.Invoice).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            ValidateExchangeRate(input);
            await ValidateBatchNo(input);

            if (input.ReceiveFrom == ReceiveFrom.ItemIssueSale)
            {
                var exceptIds = new List<Guid> { input.Id };
                if (input.ItemReceiptCustomerCreditId.HasValue) exceptIds.Add(input.ItemReceiptCustomerCreditId.Value);
                await CheckStockForSaleReturn(input.ItemIssueSaleId.Value, input.CustomerCreditDetail, exceptIds);
            }


            //validate vendor credit by item issue vendor credit 
            var validateCusotmerCredit = await (from itssueCredit in _itemReceiptCustomerCredit.GetAll().Include(u => u.CustomerCredit)
                            .Where(t => t.CustomerCreditId == input.Id)
                                                select itssueCredit).FirstOrDefaultAsync();


            if (validateCusotmerCredit != null &&
                validateCusotmerCredit.CustomerCredit != null &&
                validateCusotmerCredit.CustomerCredit.ConvertToItemReceipt == false &&
                validateCusotmerCredit.ReceiveFrom == ReceiveFrom.CustomerCredit)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            else if (validateCusotmerCredit != null &&
                validateCusotmerCredit.CustomerCredit != null &&
                validateCusotmerCredit.CustomerCredit.ReceiveFrom == ReceiveFrom.ItemReceiptCustomerCredit)
            {
                validateCusotmerCredit.CustomerCredit.UpdateShipedStatus(DeliveryStatus.ShipPending);
                validateCusotmerCredit.UpdateCustomerCredit(null);
                CheckErrors(await _itemReceiptCustomerCreditManager.UpdateAsync(validateCusotmerCredit));
            }

            if (input.CustomerCreditDetail.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }

            await CalculateTotal(input);

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            // step 1. update Journal 
            var @journal = await _journalRepository
                              .GetAll()
                              .Where(u => u.JournalType == JournalType.CustomerCredit && u.CustomerCreditId == input.Id)
                              .FirstOrDefaultAsync();

            await CheckClosePeriod(journal.Date, input.CreditDate);

            journal.Update(tenantId, input.CustomerCreditNo, input.CreditDate, input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, input.Status, input.LocationId);

            if (input.MultiCurrencyId == null || input.MultiCurrencyId == 0 || input.CurrencyId == input.MultiCurrencyId)
            {
                input.MultiCurrencyId = input.CurrencyId;
                input.MultiCurrencyTotal = input.Total;
                input.MultiCurrencySubTotal = input.SubTotal;

            }
            journal.UpdateMultiCurrency(input.MultiCurrencyId);

            //journal.UpdateStatus(input.Status);

            // step 2. update AR account 
            var @apAccount = await (_journalItemRepository.GetAll()
                                      .Where(u => u.JournalId == journal.Id && u.Key == PostingKey.AR && u.Identifier == null))
                                      .FirstOrDefaultAsync();
            @apAccount.UpdateJournalItem(tenantId, input.AccountId, input.Memo, 0, input.Total);

            // step 3. update Customer credit
            var customerCredit = await _customerCreditManager.GetAsync(input.Id, true);
            var oldConvertToItemReceiptCustomerCredit = customerCredit.ConvertToItemReceipt;
            if (customerCredit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var isItem = input.CustomerCreditDetail.Any(s => s.ItemId != null && s.ItemId != Guid.Empty);

            customerCredit.Update(tenantId, input.CustomerId,
                input.SameAsShippingAddress,
                input.ShippingAddress, input.BillingAddress,
                input.SubTotal, input.Tax, input.Total,
                input.DueDate, input.ConvertToItemReceipt, input.ReceiveDate,
                input.MultiCurrencySubTotal, input.MultiCurrencyTax, input.MultiCurrencyTotal,
                input.ReceiveFrom, input.ItemIssueSaleId, customerCredit.IsPOS, isItem);
            //Check if credit list item detail has no item ID so it mean no receive status 
            var isAccountTap = checkIfItemExist(input.CustomerCreditDetail, null);
            if (isAccountTap)
            {
                customerCredit.UpdateShipedStatus(DeliveryStatus.NoReceive);
            }

            if (input.ReceiveFrom == ReceiveFrom.ItemReceiptCustomerCredit && input.ItemReceiptCustomerCreditId != null)
            {
                #region Calculat Cost
                int digit = await _accountCycleRepository.GetAll()
                     .Where(t => t.TenantId == tenantId && t.EndDate == null)
                     .OrderByDescending(t => t.StartDate)
                     .Select(t => t.RoundingDigit).FirstOrDefaultAsync();
                var items = input.CustomerCreditDetail.GroupBy(s => s.ItemId).Select(s => s.FirstOrDefault().ItemId.Value).ToList();
                var itemCostDic = await _inventoryManager.GetItemCostSummaryQuery(null, input.ReceiveDate.Value, new List<long?> { input.LocationId }, userId, items, new List<Guid> { input.ItemReceiptCustomerCreditId.Value }).ToDictionaryAsync(s => $"{s.ItemId}-{s.LocationId}", s => s);
                var itemLatestCostDic = await _inventoryManager.GetItemLatestCostSummaryQuery(input.ReceiveDate.Value, new List<long?> { input.LocationId }, items, new List<Guid> { input.ItemReceiptCustomerCreditId.Value }).ToDictionaryAsync(s => $"{s.ItemId}-{s.LocationId}", s => s);
                var itemPurchaseCostDic = await _itemRepository.GetAll().Where(s => items.Contains(s.Id)).Select(s => new { s.Id, Cost = s.PurchaseCost ?? 0 }).ToDictionaryAsync(s => s.Id, s => s.Cost);

                foreach (var r in input.CustomerCreditDetail)
                {
                    var key = $"{r.ItemId}-{input.LocationId}";

                    if (itemCostDic.ContainsKey(key) && itemCostDic[key].Qty > 0)
                    {
                        r.ItemReceiptCustomerCreditUnitCost = itemCostDic[key].Cost;
                    }
                    else if (itemLatestCostDic.ContainsKey(key))
                    {
                        r.ItemReceiptCustomerCreditUnitCost = itemLatestCostDic[key].Cost;
                    }
                    else
                    {
                        r.ItemReceiptCustomerCreditUnitCost = itemPurchaseCostDic.ContainsKey(r.ItemId.Value) ? itemPurchaseCostDic[r.ItemId.Value] : 0;
                    }

                    r.ItemReceiptCustomerCreditTotal = Math.Round(r.ItemReceiptCustomerCreditUnitCost * r.Qty, digit, MidpointRounding.AwayFromZero);
                }
                var ItemReceiptCustomerCreditTotal = input.CustomerCreditDetail.Sum(t => t.ItemReceiptCustomerCreditTotal);
                #endregion Calculat Cost           



                var ItemReceiptJournal = await _journalRepository.GetAll()
                                        .Include(u => u.ItemReceiptCustomerCredit)
                                        .Where(t => t.ItemReceiptCustomerCreditId == input.ItemReceiptCustomerCreditId).FirstOrDefaultAsync();

                if (ItemReceiptJournal != null && ItemReceiptJournal.ItemReceiptCustomerCreditId != null)
                {
                    //update item issue
                    var validateLockDateReceipt = await _lockRepository.GetAll()
                                 .Where(t => t.IsLock == true &&
                                 (t.LockDate.Value.Date >= ItemReceiptJournal.Date.Date || t.LockDate.Value.Date >= input.ReceiveDate.Value.Date)
                                 && (t.LockKey == TransactionLockType.ItemReceipt || t.LockKey == TransactionLockType.InventoryTransaction)).CountAsync();

                    var itemReceiptItems = await _itemReceiptCustomerCreditItemRepository.GetAll()
                                           .Where(s => s.ItemReceiptCustomerCreditId == input.ItemReceiptCustomerCreditId)
                                           .AsNoTracking()
                                           .ToListAsync();

                    var isChangeItems = input.CustomerCreditDetail.Count() != itemReceiptItems.Count() || input.CustomerCreditDetail.Any(s => itemReceiptItems.Any(r => r.Id == s.ItemReceiptCustomerCreditItemId && (r.ItemId != s.ItemId || r.LotId != s.LotId || r.Qty != s.Qty)));

                    if (validateLockDateReceipt > 0 && (ItemReceiptJournal.Date.Date != input.ReceiveDate.Value.Date || isChangeItems))
                    {
                        throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                    }
                    ItemReceiptJournal.UpdateJournalDate(input.ReceiveDate.Value);
                    ItemReceiptJournal.ItemReceiptCustomerCredit.UpdateTotal(ItemReceiptCustomerCreditTotal);
                    ItemReceiptJournal.UpdateCreditDebit(ItemReceiptCustomerCreditTotal, ItemReceiptCustomerCreditTotal);
                    ItemReceiptJournal.ItemReceiptCustomerCredit.UpdateCustomerCredit(customerCredit.Id);

                    var setting = await _settingRepository.FirstOrDefaultAsync(s => s.SettingType == BillInvoiceSettingType.Invoice);

                    if (setting == null || setting.ReferenceSameAsGoodsMovement) ItemReceiptJournal.UpdateReference(input.Reference);

                    var autoIss = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt_CustomerCredit);
                    CheckErrors(await _journalManager.UpdateAsync(ItemReceiptJournal, autoIss.DocumentType));
                    CheckErrors(await _itemReceiptCustomerCreditManager.UpdateAsync(ItemReceiptJournal.ItemReceiptCustomerCredit));
                }

                var itemReceiptCustomerCredit = await _itemReceiptCustomerCreditReository.GetAll().Where(t => t.Id == input.ItemReceiptCustomerCreditId).FirstOrDefaultAsync();
                itemReceiptCustomerCredit.UpdateCustomerCredit(customerCredit.Id);
                itemReceiptCustomerCredit.UpdateTotal(ItemReceiptCustomerCreditTotal);
                CheckErrors(await _itemReceiptCustomerCreditManager.UpdateAsync(itemReceiptCustomerCredit));
                customerCredit.UpdateShipedStatus(DeliveryStatus.ShipAll);
            }

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.CustomerCredit);
            CheckErrors(await _journalManager.UpdateAsync(@journal, auto.DocumentType));

            CheckErrors(await _journalItemManager.UpdateAsync(@apAccount));
            CheckErrors(await _customerCreditManager.UpdateAsync(customerCredit));


            var exchange = await _exchangeRateRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.CustomerCreditId == input.Id);
            if (input.UseExchangeRate && input.CurrencyId != input.MultiCurrencyId)
            {
                if (exchange == null)
                {
                    exchange = CustomerCreditExchangeRate.Create(tenantId, userId, customerCredit.Id, input.ExchangeRate.FromCurrencyId, input.ExchangeRate.ToCurrencyId, input.ExchangeRate.Bid, input.ExchangeRate.Ask);
                    await _exchangeRateRepository.InsertAsync(exchange);
                }
                else
                {
                    exchange.Update(userId, customerCredit.Id, input.ExchangeRate.FromCurrencyId, input.ExchangeRate.ToCurrencyId, input.ExchangeRate.Bid, input.ExchangeRate.Ask);
                    await _exchangeRateRepository.UpdateAsync(exchange);
                }
            }
            else if (exchange != null)
            {
                await _exchangeRateRepository.DeleteAsync(exchange);
            }

            // step 4. Update Customer credit detail item and Journal Item
            var customerCreditDetail = await _customerCreditDetailRepository.GetAll().Where(u => u.CustomerCreditId == input.Id).ToListAsync();


            var oldOrderIds = new List<Guid>();
            var oldDeliveryIds = new List<Guid>();


            var itemIssueItemIds = customerCreditDetail.Where(s => s.ItemIssueSaleItemId.HasValue).Select(s => s.ItemIssueSaleItemId.Value).ToList();
            if (itemIssueItemIds.Any())
            {
                var orderFromIssue = await _itemIssueItemRepository.GetAll()
                                           .Where(s => s.SaleOrderItemId.HasValue)
                                           .Where(s => itemIssueItemIds.Contains(s.Id))
                                           .AsNoTracking()
                                           .Select(s => s.SaleOrderItem.SaleOrderId)
                                           .ToListAsync();

                var orderFromInvoice = await _invoiceItemRepository.GetAll()
                                             .Where(s => s.OrderItemId.HasValue)
                                             .Where(s => s.ItemIssueItemId.HasValue)
                                             .Where(s => itemIssueItemIds.Contains(s.ItemIssueItemId.Value))
                                             .AsNoTracking()
                                             .Select(s => s.SaleOrderItem.SaleOrderId)
                                             .ToListAsync();

                oldOrderIds = orderFromIssue.Concat(orderFromInvoice).Distinct().ToList();

                var deliveryFromIssue = await _itemIssueItemRepository.GetAll()
                                        .Where(s => s.DeliverySchedulItemId.HasValue)
                                        .Where(s => itemIssueItemIds.Contains(s.Id))
                                        .AsNoTracking()
                                        .Select(s => s.DeliveryScheduleItem.DeliveryScheduleId)
                                        .ToListAsync();

                var deliveryFromInvoice = await _invoiceItemRepository.GetAll()
                                             .Where(s => s.DeliverySchedulItemId.HasValue)
                                             .Where(s => s.ItemIssueItemId.HasValue)
                                             .Where(s => itemIssueItemIds.Contains(s.ItemIssueItemId.Value))
                                             .AsNoTracking()
                                             .Select(s => s.DeliveryScheduleItem.DeliveryScheduleId)
                                             .ToListAsync();

                oldDeliveryIds = deliveryFromIssue.Concat(deliveryFromInvoice).Distinct().ToList();
            }
            else
            {
                var oldCreditItemIds = customerCreditDetail.Select(s => s.Id).ToList();
                var itemIssueFromItemReceiptIds = await _itemReceiptCustomerCreditItemRepository.GetAll()
                                                       .Where(s => s.CustomerCreditItemId.HasValue)
                                                       .Where(s => oldCreditItemIds.Contains(s.CustomerCreditItemId.Value))
                                                       .Where(s => s.ItemIssueSaleItemId.HasValue)
                                                       .AsNoTracking()
                                                       .Select(s => s.ItemIssueSaleItemId.Value)
                                                       .ToListAsync();

                var orderFromIssue = await _itemIssueItemRepository.GetAll()
                                           .Where(s => s.SaleOrderItemId.HasValue)
                                           .Where(s => itemIssueFromItemReceiptIds.Contains(s.Id))
                                           .AsNoTracking()
                                           .Select(s => s.SaleOrderItem.SaleOrderId)
                                           .ToListAsync();

                var orderFromInvoice = await _invoiceItemRepository.GetAll()
                                             .Where(s => s.OrderItemId.HasValue)
                                             .Where(s => s.ItemIssueItemId.HasValue)
                                             .Where(s => itemIssueFromItemReceiptIds.Contains(s.ItemIssueItemId.Value))
                                             .AsNoTracking()
                                             .Select(s => s.SaleOrderItem.SaleOrderId)
                                             .ToListAsync();

                oldOrderIds = orderFromIssue.Concat(orderFromInvoice).Distinct().ToList();


                var deliveryFromIssue = await _itemIssueItemRepository.GetAll()
                                         .Where(s => s.SaleOrderItemId.HasValue)
                                         .Where(s => itemIssueFromItemReceiptIds.Contains(s.Id))
                                         .AsNoTracking()
                                         .Select(s => s.DeliveryScheduleItem.DeliveryScheduleId)
                                         .ToListAsync();

                var deliveryFromInvoice = await _invoiceItemRepository.GetAll()
                                             .Where(s => s.DeliverySchedulItemId.HasValue)
                                             .Where(s => s.ItemIssueItemId.HasValue)
                                             .Where(s => itemIssueFromItemReceiptIds.Contains(s.ItemIssueItemId.Value))
                                             .AsNoTracking()
                                             .Select(s => s.DeliveryScheduleItem.DeliveryScheduleId)
                                             .ToListAsync();

                oldDeliveryIds = deliveryFromIssue.Concat(deliveryFromInvoice).Distinct().ToList();
            }



            var saleAllowanceJournalAcc = await (_journalItemRepository.GetAll().Where(u => u.JournalId == journal.Id &&
                                           u.Key == PostingKey.SaleAllowance && u.Identifier != null)).ToListAsync();

            var toDeleteCustomerCreditDetails = customerCreditDetail.Where(u => !input.CustomerCreditDetail.Any(i => i.Id != null && i.Id == u.Id)).ToList();
            var toDeletejournalItem = saleAllowanceJournalAcc.Where(u => !input.CustomerCreditDetail.Any(i => u.Identifier != null && i.Id != null && i.Id == u.Identifier)).ToList();

            int index = 0;

            var itemReceiptCustomerCreditItemBatchNos = new List<ItemReceiptCustomerCreditItemBatchNo>();
            var itemreceiptCustomerCreditItems = new List<ItemReceiptItemCustomerCredit>();
            var @inventoryJournalItems = new List<JournalItem>();

            if (input.ItemReceiptCustomerCreditId.HasValue)
            {
                itemreceiptCustomerCreditItems = await _itemReceiptCustomerCreditItemRepository.GetAll()
                                                       .Where(t => t.ItemReceiptCustomerCreditId == input.ItemReceiptCustomerCreditId)
                                                       .ToListAsync();

                @inventoryJournalItems = await _journalItemRepository.GetAll()
                                               .Where(s => s.Journal.ItemReceiptCustomerCreditId.HasValue)
                                               .Where(u => u.Journal.ItemReceiptCustomerCreditId == input.ItemReceiptCustomerCreditId && u.Identifier != null)
                                               .ToListAsync();

                itemReceiptCustomerCreditItemBatchNos = await _itemReceiptCustomerCreditItemBatchNoRepository.GetAll()
                                                            .AsNoTracking()
                                                            .Where(s => s.ItemReceiptCustomerCreditItem.ItemReceiptCustomerCreditId == input.ItemReceiptCustomerCreditId.Value)
                                                            .ToListAsync();
            }

            var OldItemIssueVendorCreditItems = new List<ItemReceiptItemCustomerCredit>();

            if (toDeleteCustomerCreditDetails.Any())
            {
                OldItemIssueVendorCreditItems = await _itemReceiptCustomerCreditItemRepository.GetAll()
                                                      .Where(t => toDeleteCustomerCreditDetails.Any(g => g.Id == t.CustomerCreditItemId))
                                                      .ToListAsync();
            }

            var itemBatchNos = await _customerCreditItemBatchNoRepository.GetAll().Where(s => customerCreditDetail.Any(r => r.Id == s.CustomerCreditItemId)).AsNoTracking().ToListAsync();
            if (toDeleteCustomerCreditDetails.Any())
            {
                var deleteItemBatchNos = itemBatchNos.Where(s => toDeleteCustomerCreditDetails.Any(r => r.Id == s.CustomerCreditItemId)).ToList();
                if (deleteItemBatchNos.Any()) await _customerCreditItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
            }

            foreach (var c in input.CustomerCreditDetail)
            {
                index++;
                if (c.LotId == null && c.ItemId != null)
                {
                    throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + index.ToString());
                }

                //validate when Currency as Multi Currrency

                if (input.CurrencyId == input.MultiCurrencyId)
                {
                    c.MultiCurrencyTotal = c.Total;
                    c.MultiCurrencyUnitCost = c.UnitCost;
                }

                if (c.Id != null) // step 5. update item detail 
                {

                    var itemDetail = customerCreditDetail.FirstOrDefault(u => u.Id == c.Id);
                    var journalItem = saleAllowanceJournalAcc.FirstOrDefault(u => u.Identifier == c.Id);
                    if (itemDetail != null)
                    {
                        itemDetail.Update(tenantId, c.TaxId, c.ItemId, c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total, c.MultiCurrencyUnitCost, c.MultiCurrencyTotal, c.ItemIssueSaleItemId, c.SalePrice);
                        itemDetail.UpdateLot(c.LotId);
                        CheckErrors(await _customerCreditDetailManager.UpdateAsync(itemDetail));
                    }
                    if (journalItem != null)
                    {
                        //  update journal item with default debit value and credit auto zero
                        journalItem.UpdateJournalItem(userId, c.AccountId, c.Description, c.Total, 0);
                        CheckErrors(await _journalItemManager.UpdateAsync(journalItem));
                    }

                    var oldItemBatchs = itemBatchNos.Where(s => s.CustomerCreditItemId == c.Id).ToList();

                    if (c.ItemBatchNos != null && c.ItemBatchNos.Any())
                    {
                        var addItemBatchNos = c.ItemBatchNos.Where(s => !s.Id.HasValue || s.Id == Guid.Empty)
                                               .Select(s => CustomerCreditItemBatchNo.Create(tenantId, userId, itemDetail.Id, s.BatchNoId, s.Qty)).ToList();
                        if (addItemBatchNos.Any())
                        {
                            foreach (var a in addItemBatchNos)
                            {
                                await _customerCreditItemBatchNoRepository.InsertAsync(a);
                            }
                        }

                        var updateItemBatchNos = oldItemBatchs.Where(s => c.ItemBatchNos.Any(r => r.Id == s.Id)).Select(s =>
                        {
                            var oldItemBatch = s;
                            var newBatch = c.ItemBatchNos.FirstOrDefault(r => r.Id == s.Id);

                            oldItemBatch.Update(userId, itemDetail.Id, newBatch.BatchNoId, newBatch.Qty);
                            return oldItemBatch;
                        }).ToList();

                        if (updateItemBatchNos.Any()) await _customerCreditItemBatchNoRepository.BulkUpdateAsync(updateItemBatchNos);

                        var deleteItemBatchNos = oldItemBatchs.Where(s => !c.ItemBatchNos.Any(r => r.Id == s.Id)).ToList();
                        if (deleteItemBatchNos.Any())
                        {
                            await _customerCreditItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
                        }
                    }
                    else if (oldItemBatchs.Any())
                    {
                        await _customerCreditItemBatchNoRepository.BulkDeleteAsync(oldItemBatchs);
                    }

                }
                else if (c.Id == null) // create new 

                {

                    // step 6. insert to Customer credit detail
                    var itemDetail = CustomerCreditDetail.Create(tenantId, userId,
                        customerCredit, c.TaxId, c.ItemId, c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total, c.MultiCurrencyUnitCost, c.MultiCurrencyTotal, c.ItemIssueSaleItemId, c.SalePrice);
                    itemDetail.UpdateLot(c.LotId);
                    CheckErrors(await _customerCreditDetailManager.CreateAsync(itemDetail));
                    //insert journal item into debit value & credit 0
                    var saleAllowaceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal, c.AccountId, c.Description, c.Total, 0, PostingKey.SaleAllowance, itemDetail.Id);
                    CheckErrors(await _journalItemManager.CreateAsync(saleAllowaceJournalItem));
                    if (input.ConvertToItemReceipt == true)
                    {
                        c.Id = itemDetail.Id;
                    }
                    // Update item Issue vendor credit item when recieve from item issue vendor credit
                    if (input.ReceiveFrom == ReceiveFrom.ItemReceiptCustomerCredit && input.ItemReceiptCustomerCreditId != null)
                    {
                        var itemIssueVendorCreditItem = itemreceiptCustomerCreditItems.Where(t => t.Id == c.ItemReceiptCustomerCreditItemId).FirstOrDefault();
                        itemIssueVendorCreditItem.UpdateCustomerCreditItemId(itemDetail.Id);
                        CheckErrors(await _itemReceiptCustomerCreditItemManager.UpdateAsync(itemIssueVendorCreditItem));
                    }

                    if (c.ItemBatchNos != null && c.ItemBatchNos.Any())
                    {
                        var addItemBatchNos = c.ItemBatchNos.Select(s => CustomerCreditItemBatchNo.Create(tenantId, userId, itemDetail.Id, s.BatchNoId, s.Qty)).ToList();
                        foreach (var a in addItemBatchNos)
                        {
                            await _customerCreditItemBatchNoRepository.InsertAsync(a);
                        }
                    }

                }

                // Update item Issue vendor credit item when recieve from item issue vendor credit
                if (input.ReceiveFrom == ReceiveFrom.ItemReceiptCustomerCredit && input.ItemReceiptCustomerCreditId != null)
                {
                    var journalItemReceiptCustomerCredit = @inventoryJournalItems.ToList().Where(u => u.Identifier == c.ItemReceiptCustomerCreditItemId);

                    if (journalItemReceiptCustomerCredit != null && journalItemReceiptCustomerCredit.Count() > 0)
                    {
                        foreach (var rc in journalItemReceiptCustomerCredit)
                        {
                            if (rc.Key == PostingKey.Inventory)
                            {
                                // update inventory posting key credit
                                rc.SetDebitCredit(c.ItemReceiptCustomerCreditTotal, 0);
                                CheckErrors(await _journalItemManager.UpdateAsync(rc));
                            }
                            if (rc.Key == PostingKey.COGS)
                            {
                                // update cogs posting key debit
                                rc.SetDebitCredit(0, c.ItemReceiptCustomerCreditTotal);
                                CheckErrors(await _journalItemManager.UpdateAsync(rc));
                            }
                        }

                    }

                    var itemReceiptCustomerCreditItem = itemreceiptCustomerCreditItems.Where(t => t.Id == c.ItemReceiptCustomerCreditItemId).FirstOrDefault();

                    if (itemReceiptCustomerCreditItem != null)
                    {
                        itemReceiptCustomerCreditItem.UpdateQty(c.Qty);
                        itemReceiptCustomerCreditItem.UpdateItemReceiptItem(c.ItemReceiptCustomerCreditUnitCost, 0, c.ItemReceiptCustomerCreditTotal);
                        CheckErrors(await _itemReceiptCustomerCreditItemManager.UpdateAsync(itemReceiptCustomerCreditItem));

                        var oldItemBatchs = itemReceiptCustomerCreditItemBatchNos.Where(s => s.ItemReceiptCustomerCreditItemId == itemReceiptCustomerCreditItem.Id).ToList();
                        if (c.ItemBatchNos != null && c.ItemBatchNos.Any())
                        {
                            var addItemBatchNos = c.ItemBatchNos.Where(s => !s.Id.HasValue || s.Id == Guid.Empty || !oldItemBatchs.Any(b => b.BatchNoId == s.BatchNoId))
                                                   .Select(s => ItemReceiptCustomerCreditItemBatchNo.Create(tenantId, userId, itemReceiptCustomerCreditItem.Id, s.BatchNoId, s.Qty)).ToList();
                            if (addItemBatchNos.Any())
                            {
                                foreach (var a in addItemBatchNos)
                                {
                                    await _itemReceiptCustomerCreditItemBatchNoRepository.InsertAsync(a);
                                }
                            }

                            var updateItemBatchNos = oldItemBatchs.Where(s => c.ItemBatchNos.Any(r => r.BatchNoId == s.BatchNoId)).Select(s =>
                            {
                                var oldItemBatch = s;
                                var newBatch = c.ItemBatchNos.FirstOrDefault(r => r.BatchNoId == s.BatchNoId);

                                oldItemBatch.Update(userId, itemReceiptCustomerCreditItem.Id, newBatch.BatchNoId, newBatch.Qty);
                                return oldItemBatch;
                            }).ToList();

                            if (updateItemBatchNos.Any()) await _itemReceiptCustomerCreditItemBatchNoRepository.BulkUpdateAsync(updateItemBatchNos);

                            var deleteItemBatchNos = oldItemBatchs.Where(s => !c.ItemBatchNos.Any(r => r.BatchNoId == s.BatchNoId)).ToList();
                            if (deleteItemBatchNos.Any())
                            {
                                await _itemReceiptCustomerCreditItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
                            }
                        }
                        else if (oldItemBatchs.Any())
                        {
                            await _itemReceiptCustomerCreditItemBatchNoRepository.BulkDeleteAsync(oldItemBatchs);
                        }
                    }

                }

            }


            var itemIssueVendorcredit = OldItemIssueVendorCreditItems
             .Where(t => toDeleteCustomerCreditDetails.Any(g => g.Id == t.CustomerCreditItemId));
            if (input.ConvertToItemReceipt == true)
            {
                foreach (var iIV in itemIssueVendorcredit)
                {
                    iIV.UpdateCustomerCreditItemId(null);
                }
            }
            foreach (var t in toDeleteCustomerCreditDetails)
            {
                var ItemIsseueItem = OldItemIssueVendorCreditItems.Where(g => g.CustomerCreditItemId == t.Id).FirstOrDefault();
                if (ItemIsseueItem != null)
                {
                    ItemIsseueItem.UpdateCustomerCreditItemId(null);
                }
                CheckErrors(await _customerCreditDetailManager.RemoveAsync(t));
            }
            foreach (var t in toDeletejournalItem)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(t));
            }


            await CurrentUnitOfWork.SaveChangesAsync();

            var tenant = await GetCurrentTenantAsync();
            var ClearanceAccountId = tenant.ItemRecieptCustomerCreditId;

            // update item reciept customer credit 
            if (oldConvertToItemReceiptCustomerCredit == true && input.ConvertToItemReceipt == true && input.Status == TransactionStatus.Publish && input.ReceiveFrom != ReceiveFrom.ItemReceiptCustomerCredit)
            {
                var itemReceiptCustomerCreditId = await _itemReceiptCustomerCredit.GetAll().Where(t => t.CustomerCreditId == input.Id).Select(t => t.Id).FirstOrDefaultAsync();
                var getItem = await _itemRepository.GetAll().Where(g => g.ItemTypeId != 3 && input.CustomerCreditDetail.Any(i => i.ItemId == g.Id)).ToListAsync();

                var itemReceiptInput = new UpdateItemReceiptCustomerCreditInput()
                {
                    // ClearanceAccountId = ClearanceAccountId.Value,
                    Id = itemReceiptCustomerCreditId,
                    IsConfirm = input.IsConfirm,
                    // ClearanceAccountId =  
                    Status = input.Status,
                    BillingAddress = input.BillingAddress,
                    ClassId = input.ClassId,
                    CurrencyId = input.CurrencyId,
                    CustomerCreditId = customerCredit.Id,
                    CustomerId = input.CustomerId,
                    Date = input.ReceiveDate.Value,
                    LocationId = input.LocationId,
                    Memo = input.Memo,
                    ReceiptNo = input.CustomerCreditNo,
                    ReceiveFrom = ReceiveFrom.CustomerCredit,
                    Reference = input.Reference,
                    SameAsShippingAddress = input.SameAsShippingAddress,
                    ShippingAddress = input.ShippingAddress,
                    Total = input.Total,
                    Items = input.CustomerCreditDetail
                        .Where(d => getItem.Any(v => v.Id == d.ItemId))
                        .Select(t => new CreateOrUpdateItemReceiptCustomerCreditItemInput
                        {
                            LotId = t.LotId,
                            UnitCost = t.UnitCost,
                            Total = t.Total,
                            CustomerCreditItemId = t.Id,
                            Description = t.Description,
                            DiscountRate = t.DiscountRate,
                            InventoryAccountId = t.Item.InventoryAccountId.Value,
                            ClearanceAccountId = getItem.Where(g => g.Id == t.ItemId).Select(g => g.PurchaseAccountId.Value).FirstOrDefault(),
                            Item = t.Item,
                            ItemId = t.ItemId.Value,
                            Qty = t.Qty,
                            Id = t.ItemReceiptCustomerCreditItemId,
                            ItemBatchNos = t.ItemBatchNos
                        }).ToList()
                };
                await UpdateItemReceiptCustomerCredit(itemReceiptInput);
            }

            //delete item receipt when old data bill before true but now user change false then delete item receipt
            else if (oldConvertToItemReceiptCustomerCredit == true && input.ConvertToItemReceipt == false && input.Status == TransactionStatus.Publish)
            {
                customerCredit.UpdateShipedStatus(DeliveryStatus.ShipPending);
                var ReceiptId = new CarlEntityDto() { IsConfirm = input.IsConfirm, Id = validateCusotmerCredit.Id };
                await DeleteItemReceiptCustomerCredit(ReceiptId);
            }


            // create item receipt while bill data before auto convert is false then user change to true so automatically create item receipt
            else if (oldConvertToItemReceiptCustomerCredit == false && input.ConvertToItemReceipt == true && input.Status == TransactionStatus.Publish && input.ReceiveFrom != ReceiveFrom.ItemReceiptCustomerCredit)
            {
                var getItem = await _itemRepository.GetAll().Where(g => g.ItemTypeId != 3 && input.CustomerCreditDetail.Any(i => i.ItemId == g.Id)).ToListAsync();

                var itemReceiptInput = new CreateItemReceiptCustomerCreditInput()
                {
                    Status = input.Status,
                    BillingAddress = input.BillingAddress,
                    ClassId = input.ClassId,
                    CurrencyId = input.CurrencyId,
                    CustomerCreditId = customerCredit.Id,
                    CustomerId = input.CustomerId,
                    Date = input.ReceiveDate.Value,
                    LocationId = input.LocationId,
                    Memo = input.Memo,
                    ReceiptNo = input.CustomerCreditNo,
                    ReceiveFrom = ReceiveFrom.CustomerCredit,
                    Reference = input.Reference,
                    SameAsShippingAddress = input.SameAsShippingAddress,
                    ShippingAddress = input.ShippingAddress,
                    Total = input.Total,
                    Items = input.CustomerCreditDetail
                        .Where(d => getItem.Any(v => v.Id == d.ItemId))
                        .Select(t => new CreateOrUpdateItemReceiptCustomerCreditItemInput
                        {
                            LotId = t.LotId,
                            UnitCost = t.UnitCost,
                            Total = t.Total,
                            CustomerCreditItemId = t.Id,
                            Description = t.Description,
                            DiscountRate = t.DiscountRate,
                            InventoryAccountId = t.Item.InventoryAccountId.Value,
                            ClearanceAccountId = getItem.Where(g => g.Id == t.ItemId).Select(g => g.PurchaseAccountId.Value).FirstOrDefault(),
                            Item = t.Item,
                            ItemId = t.ItemId.Value,
                            Qty = t.Qty,
                            ItemBatchNos = t.ItemBatchNos
                        }).ToList()
                };
                await CreateItemReceiptCustomerCredit(itemReceiptInput);

            }

            var orderIds = input.CustomerCreditDetail.Where(s => s.OrderId.HasValue).GroupBy(s => s.OrderId.Value).Select(s => s.Key);
            orderIds = orderIds.Concat(oldOrderIds).Distinct();

            var deliveryIds = input.CustomerCreditDetail.Where(s => s.DeliveryId.HasValue).GroupBy(s => s.DeliveryId.Value).Select(s => s.Key);
            deliveryIds = deliveryIds.Concat(oldDeliveryIds).Distinct();

            if (orderIds.Any() || deliveryIds.Any())
            {
                await this.CurrentUnitOfWork.SaveChangesAsync();
                foreach (var id in orderIds)
                {
                    await UpdateOrderInventoryStatus(id);
                }
                foreach(var id in deliveryIds)
                {
                    await UpdateDeliveryInventoryStatus(id, true);
                }
            }

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.Invoice };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }


            return new NullableIdDto<Guid>() { Id = customerCredit.Id };

        }

        //[AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_UpdateToDraft)]
        //public async Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input)
        //{
        //    var @entity = await _journalRepository
        //                        .GetAll()
        //                        .Include(u => u.CustomerCredit)
        //                        .Where(u => u.JournalType == JournalType.CustomerCredit && u.CustomerCreditId == input.Id)
        //                        .FirstOrDefaultAsync();
        //    if (entity == null)
        //    {
        //        throw new UserFriendlyException(L("RecordNotFound"));
        //    }
        //    //validate vendor credit by recievePayment
        //    //var validate = (from recievePayment in _recievePaymentReposity.GetAll()
        //    //                .Where(t => t.CustomerCreditId == input.Id)
        //    //                select recievePayment).ToList().Count();
        //    //if (validate > 0 && @entity.CustomerCredit.ConvertToItemReceipt == false)
        //    //{
        //    //    throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
        //    //}

        //    var itemReceiptCustomerCreditId = _journalRepository.GetAll().Include(u => u.ItemReceiptCustomerCredit)
        //          .Where(t => t.ItemReceiptCustomerCredit.CustomerCreditId == input.Id).Select(v => v.ItemReceiptCustomerCreditId).FirstOrDefault();

        //    if (entity.CustomerCredit.Id != null && (entity.CustomerCredit.ReceiveFrom != ReceiveFrom.ItemReceiptCustomerCredit && entity.CustomerCredit.ConvertToItemReceipt == true) && itemReceiptCustomerCreditId != null)
        //    {

        //        var inputItemReceipt = new CarlEntityDto() {IsConfirm = input.is  Id = itemReceiptCustomerCreditId.Value };
        //        await DeleteItemReceiptCustomerCredit(inputItemReceipt);

        //    }

        //    entity.UpdateStatusToDraft();

        //    return new NullableIdDto<Guid>() { Id = entity.Id };
        //}

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Invoice_UpdateToPublish)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input)
        {
            //validate vendor credit by recievePayment
            //var validate = (from recievePayment in _recievePaymentReposity.GetAll()
            //                .Where(t => t.CustomerCreditId == input.Id)
            //                select recievePayment).ToList().Count();
            //if (validate > 0)
            //{
            //    throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            //}
            //validate vendor credit by item receipt customer credit 
            var validateItemreceipt = (from receiptCredit in _itemReceiptCustomerCredit.GetAll()
                            .Where(t => t.CustomerCreditId == input.Id)
                                       select receiptCredit).ToList().Count();
            if (validateItemreceipt > 0)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            var @entity = await _journalRepository
                                .GetAll()
                                .Include(u => u.CustomerCredit)
                                .Where(u => u.JournalType == JournalType.CustomerCredit && u.CustomerCreditId == input.Id)
                                .FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.UpdatePublish();

            if (entity.CustomerCredit.ConvertToItemReceipt == true)
            {

                var itemReceiptInput = new CreateItemReceiptCustomerCreditInput()
                {
                    Status = entity.Status,
                    BillingAddress = entity.CustomerCredit.BillingAddress,
                    ClassId = entity.ClassId,
                    CurrencyId = entity.CurrencyId,
                    CustomerCreditId = entity.CustomerCredit.Id,
                    CustomerId = entity.CustomerCredit.CustomerId,
                    Date = entity.CustomerCredit.ReceiveDate.Value,
                    LocationId = entity.LocationId.Value,
                    Memo = entity.Memo,
                    ReceiptNo = entity.JournalNo,
                    ReceiveFrom = ReceiveFrom.CustomerCredit,
                    Reference = entity.Reference,
                    SameAsShippingAddress = entity.CustomerCredit.SameAsShippingAddress,
                    ShippingAddress = entity.CustomerCredit.ShippingAddress,
                    Total = entity.CustomerCredit.Total,
                    Items = _customerCreditDetailRepository.GetAll().AsNoTracking()
                            .Include(t => t.Item)
                            .Where(t => t.CustomerCreditId == entity.CustomerCredit.Id)
                            .Select(t => new CreateOrUpdateItemReceiptCustomerCreditItemInput
                            {
                                UnitCost = t.UnitCost,
                                Total = t.Total,
                                CustomerCreditItemId = t.Id,
                                Description = t.Description,
                                DiscountRate = t.DiscountRate,
                                InventoryAccountId = t.Item.InventoryAccountId.Value,
                                Item = new ItemSummaryOutput()
                                {
                                    Id = t.Item.Id,
                                    InventoryAccountId = t.Item.InventoryAccountId,
                                    ItemCode = t.Item.ItemCode,
                                    ItemName = t.Item.ItemName,
                                    PurchaseTaxId = t.Item.PurchaseTaxId,
                                    SaleAccountId = t.Item.SaleAccountId,
                                    SalePrice = t.Item.SalePrice,
                                    SaleTaxId = t.Item.SaleTaxId,
                                },
                                ItemId = t.ItemId.Value,
                                Qty = t.Qty
                            }).ToList()
                };
                await CreateItemReceiptCustomerCredit(itemReceiptInput);
            }

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_Invoice_VoidSaleReturn)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input)
        {
            var @jounal = await _journalRepository.GetAll()
               .Include(u => u.CustomerCredit)
               .Where(u => u.JournalType == JournalType.CustomerCredit && u.CustomerCreditId == input.Id)
               .FirstOrDefaultAsync();
            if (@jounal == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            else if (jounal.Status == TransactionStatus.Void)
            {
                throw new UserFriendlyException(L("StatusAlreadyVoided"));
            }

            var paymentIds = await (from ri in _receivePaymentDetailRepository.GetAll()
                                        .Where(s => s.CustomerCreditId == input.Id)
                                        .AsNoTracking()
                                    join j in _journalRepository.GetAll()
                                        .Where(s => s.Status == TransactionStatus.Publish)
                                        .AsNoTracking()
                                    on ri.ReceivePaymentId equals j.ReceivePaymentId
                                    select ri)
                                   .GroupBy(s => s.ReceivePaymentId)
                                   .Select(s => s.Key)
                                   .ToListAsync();

            if (jounal.CustomerCredit.IsPOS)
            {
                var hasCreditPayment = await (from ri in _receivePaymentDetailRepository.GetAll()
                                                .Where(s => paymentIds.Contains(s.ReceivePaymentId) && s.InvoiceId != null)
                                                .AsNoTracking()
                                              join j in _journalRepository.GetAll().Where(s => s.Status == TransactionStatus.Publish).AsNoTracking()
                                              on ri.ReceivePaymentId equals j.ReceivePaymentId
                                              select ri)
                                            .AnyAsync();

                if (hasCreditPayment) throw new UserFriendlyException(L("AlreadyHasCreditPaymentCannotBeVoided"));

                foreach (var paymentId in paymentIds)
                {
                    await UpdatePaymentStatusToVoidHelper(new UpdateStatus { Id = paymentId });
                }
            }
            else
            {

                if (paymentIds.Any()) throw new UserFriendlyException(L("AlreadyHasPaymentCannotBeVoided"));

                //validate vendor credit by item receipt customer credit 
                var validateItemreceipt = (from receiptCredit in _itemReceiptCustomerCredit.GetAll()
                                .Where(t => t.CustomerCreditId == input.Id)
                                           select receiptCredit).ToList().Count();
                if (validateItemreceipt > 0 && @jounal.CustomerCredit.ConvertToItemReceipt == false)
                {
                    throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
                }
            }

            jounal.UpdateVoid();
            // starting update item issue to void if it has convert to item issue and has item issue id
            if (@jounal.CustomerCredit.ConvertToItemReceipt == true)
            {
                var @entity = await _journalRepository
                               .GetAll()
                               .Include(u => u.ItemReceiptCustomerCredit)
                               .Where(u => u.JournalType == JournalType.ItemReceiptCustomerCredit &&
                                    u.ItemReceiptCustomerCredit.CustomerCreditId == input.Id)
                               .FirstOrDefaultAsync();

                if (entity.ItemReceiptCustomerCredit != null)
                {
                    entity.UpdateVoid();

                    await DeleteInventoryTransactionItems(entity.ItemReceiptCustomerCreditId.Value);
                }
            }
            return new NullableIdDto<Guid>() { Id = jounal.CustomerCreditId };
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

                if (bi.CustomerCreditId != null)
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

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_Credit_Find)]
        public async Task<PagedResultDto<GetListCustomerCreditOutput>> Find(ListCustomerCreditInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var accountCycle = await GetCurrentCycleAsync();

            var jQuery = _journalRepository.GetAll()
                        .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                        .Where(u => u.JournalType == JournalType.CustomerCredit && u.Status == TransactionStatus.Publish)
                        .WhereIf(!input.Filter.IsNullOrEmpty(),
                                    u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()))
                        .WhereIf(input.Locations != null && input.Locations.Count > 0, t => input.Locations.Contains(t.LocationId))
                        .AsNoTracking()
                        .Select(j => new
                        {
                            JournalType = j.JournalType,
                            Memo = j.Memo,
                            Date = j.Date,
                            JournalNo = j.JournalNo,
                            Status = j.Status,
                            CustomerCreditId = j.CustomerCreditId,
                            Id = j.Id,
                            MultiCurrencyId = j.MultiCurrencyId.Value,
                            Reference = j.Reference,
                        });

            var currencyQuery = GetCurrencies();
            var journalQuery = from j in jQuery
                               join c in currencyQuery
                               on j.MultiCurrencyId equals c.Id
                               select new
                               {
                                   JournalType = j.JournalType,
                                   Memo = j.Memo,
                                   Date = j.Date,
                                   JournalNo = j.JournalNo,
                                   Status = j.Status,
                                   CustomerCreditId = j.CustomerCreditId,
                                   Id = j.Id,
                                   MultiCurrencyId = j.MultiCurrencyId,
                                   MultiCurrencyCode = c.Code,
                                   Reference = j.Reference,
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
                              JournalType = j.JournalType,
                              Memo = j.Memo,
                              Date = j.Date,
                              JournalNo = j.JournalNo,
                              Status = j.Status,
                              CustomerCreditId = j.CustomerCreditId,
                              Id = j.Id,
                              MultiCurrencyId = j.MultiCurrencyId,
                              MultiCurrencyCode = j.MultiCurrencyCode,
                              AccountId = ji.AccountId,
                              Reference = j.Reference
                          };

            var cCreditQuery = _customerCreditRepository.GetAll()
                                .WhereIf(input.Customers != null && input.Customers.Count > 0, u => input.Customers.Contains(u.CustomerId))
                                .Where(s => s.OpenBalance > 0)
                                .Select(s => new
                                {
                                    Id = s.Id,
                                    DueDate = s.DueDate,
                                    OpenBalance = s.OpenBalance,
                                    Total = s.Total,
                                    PaidStatus = s.PaidStatus,
                                    ShipedStatus = s.ShipedStatus,
                                    MultiCurrencyOpenBalance = s.MultiCurrencyOpenBalance,
                                    CustomerId = s.CustomerId,
                                });

            var customerTypeMemberIds = await GetCustomerTypeMembers().Select(s => s.CustomerTypeId).ToListAsync();

            var customerQuery = GetCustomers(null, input.Customers, null, customerTypeMemberIds);
            var customerCreditQeury = from cc in cCreditQuery
                                      join c in customerQuery
                                      on cc.CustomerId equals c.Id
                                      select new
                                      {
                                          Id = cc.Id,
                                          DueDate = cc.DueDate,
                                          OpenBalance = cc.OpenBalance,
                                          Total = cc.Total,
                                          PaidStatus = cc.PaidStatus,
                                          ShipedStatus = cc.ShipedStatus,
                                          MultiCurrencyOpenBalance = cc.MultiCurrencyOpenBalance,
                                          CustomerId = cc.CustomerId,
                                          CustomerName = c.CustomerName
                                      };

            var query = from cc in customerCreditQeury
                        join ji in jiQuery
                        on cc.Id equals ji.CustomerCreditId
                        select new GetListCustomerCreditOutput
                        {
                            MultiCurrencyOpenBalance = Math.Round(cc.MultiCurrencyOpenBalance, accountCycle.RoundingDigit),
                            MultiCurrency = new CurrencyDetailOutput
                            {
                                Code = ji.MultiCurrencyCode,
                                Id = ji.MultiCurrencyId
                            },
                            TypeName = ji.JournalType.ToString(),
                            TypeCode = ji.JournalType,
                            Memo = ji.Memo,
                            Id = cc.Id,
                            Date = ji.Date,
                            AccountId = ji.AccountId,
                            DueDate = cc.DueDate,
                            OpenBalance = Math.Round(cc.OpenBalance, accountCycle.RoundingDigit),
                            JournalNo = ji.JournalNo,
                            Status = ji.Status,
                            Total = cc.Total,
                            Reference = ji.Reference,
                            Customer = new CustomerSummaryOutput
                            {
                                Id = cc.CustomerId,
                                CustomerName = cc.CustomerName
                            },
                            PaidStatus = cc.PaidStatus,
                            ReceivedStatus = cc.ShipedStatus
                        };

            var resultCount = await query.CountAsync();
            if (resultCount == 0) return new PagedResultDto<GetListCustomerCreditOutput>(resultCount, new List<GetListCustomerCreditOutput>());

            var @entities = await query.OrderBy(s => s.Date).ToListAsync();
            return new PagedResultDto<GetListCustomerCreditOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_Credit_Find)]
        public async Task<PagedResultDto<GetListCustomerCreditOutput>> FindOld(ListCustomerCreditInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var accountCycle = await GetCurrentCycleAsync();
            var query = (from jItem in _journalItemRepository.GetAll()
                         .Include(t => t.Account).AsNoTracking()

                         join j in _journalRepository.GetAll()
                         .Include(t => t.CustomerCredit.Customer)
                         .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                         .Where(u => u.JournalType == JournalType.CustomerCredit && u.Status == TransactionStatus.Publish)
                         .WhereIf(!input.Filter.IsNullOrEmpty(),
                                        u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()))

                         .WhereIf(input.Customers != null && input.Customers.Count > 0, t => input.Customers.Contains(t.CustomerCredit.CustomerId))
                         .WhereIf(input.Locations != null && input.Locations.Count > 0, t => input.Locations.Contains(t.LocationId))
                         .AsNoTracking()
                         on jItem.JournalId equals j.Id

                         where (j.CustomerCredit.OpenBalance > 0)
                         group jItem by new
                         {
                             TypeName = j.JournalType,
                             TypeCode = j.JournalType,
                             Memo = j.Memo,
                             Date = j.Date,
                             JournalNo = j.JournalNo,
                             Status = j.Status,
                             Id = j.CustomerCreditId,
                             DueDate = j.CustomerCredit.DueDate,
                             OpenBalance = j.CustomerCredit.OpenBalance,
                             Total = j.CustomerCredit.Total,
                             CustomerId = j.CustomerCredit.CustomerId,
                             PaidStatus = j.CustomerCredit.PaidStatus,
                             ReceivedStatus = j.CustomerCredit.ShipedStatus,
                             JournalId = j.Id,
                             customerName = j.CustomerCredit.Customer.CustomerName,
                             CustomerCode = j.CustomerCredit.Customer.CustomerCode,
                             currencyId = j.CurrencyId,
                             CurrencyName = j.Currency.Name,
                             CurrencyCode = j.Currency.Code,
                             Symbol = j.Currency.Symbol,
                             customerCreditMultiCurrencyOpenBalance = j.CustomerCredit.MultiCurrencyOpenBalance,
                             mCurrencyId = j.MultiCurrencyId,
                             mCurrencyCode = j.MultiCurrency.Code,
                             mCurrencyName = j.MultiCurrency.Name,
                             mCurrencyPluralName = j.MultiCurrency.PluralName,
                             mCurrencySymbol = j.MultiCurrency.Symbol,
                         } into u
                         select new GetListCustomerCreditOutput
                         {
                             Currency = new CurrencyDetailOutput
                             {
                                 Id = u.Key.currencyId,
                                 Name = u.Key.CurrencyName,
                                 Code = u.Key.CurrencyCode,
                                 Symbol = u.Key.Symbol
                             },
                             MultiCurrencyOpenBalance = Math.Round(u.Key.customerCreditMultiCurrencyOpenBalance, accountCycle.RoundingDigit),
                             MultiCurrency = new CurrencyDetailOutput
                             {
                                 Code = u.Key.mCurrencyCode,
                                 Id = u.Key.mCurrencyId.Value,
                                 Name = u.Key.mCurrencyName,
                                 PluralName = u.Key.mCurrencyPluralName,
                                 Symbol = u.Key.mCurrencySymbol
                             },
                             TypeName = u.Key.TypeName.ToString(),
                             TypeCode = u.Key.TypeCode,
                             Memo = u.Key.Memo,
                             Id = u.Key.Id.Value,
                             Date = u.Key.Date,
                             AccountId = u.Where(t => t.Key == PostingKey.AR && t.Identifier == null && t.JournalId == u.Key.JournalId)
                                     .Select(t => t.AccountId).FirstOrDefault(),
                             DueDate = u.Key.DueDate,
                             OpenBalance = Math.Round(u.Key.OpenBalance, accountCycle.RoundingDigit),
                             JournalNo = u.Key.JournalNo,
                             Status = u.Key.Status,
                             Total = u.Key.Total,
                             Customer = new CustomerSummaryOutput
                             {
                                 Id = u.Key.CustomerId,
                                 CustomerCode = u.Key.CustomerCode,
                                 CustomerName = u.Key.customerName
                             },
                             PaidStatus = u.Key.PaidStatus,
                             ReceivedStatus = u.Key.ReceivedStatus
                         });
            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            return new PagedResultDto<GetListCustomerCreditOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_Credit_GetCustomerCreditHeader)]
        public async Task<PagedResultDto<CustomerCreditSummaryOutput>> GetCustomerCreditHeader(GetCustomerCreditInput input)
        {
            var @query = from j in _journalRepository.GetAll()
                                 .Include(u => u.CustomerCredit)
                                 .Include(u => u.Class)
                                 .Include(u => u.Location)
                                 .Include(u => u.Currency)
                                 .Include(u => u.CustomerCredit.Customer)
                                 .Where(u => u.JournalType == JournalType.CustomerCredit &&
                                  u.Status == TransactionStatus.Publish &&
                                  u.CustomerCredit.ShipedStatus == DeliveryStatus.ShipPending &&
                                  u.CustomerCredit.ReceiveFrom != ReceiveFrom.ItemReceiptCustomerCredit &&
                                  u.CustomerCredit.ConvertToItemReceipt == false)
                                 .Where(u => u.Status == TransactionStatus.Publish)
                                 .WhereIf(!input.Filter.IsNullOrEmpty(),
                                  p => p.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                  p.Reference.ToLower().Contains(input.Filter.ToLower()))
                                 .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId))
                                 .WhereIf(input.FromDate != null && input.ToDate != null,
                                 (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                                .WhereIf(input.Customers != null && input.Customers.Count > 0, u => input.Customers.Contains(u.CustomerCredit.CustomerId))
                                .AsNoTracking()
                         join vi in _customerCreditDetailRepository.GetAll()
                         .Where(u => u.ItemId != null)
                         .WhereIf(input.Lots != null && input.Lots.Count > 0, u => input.Lots.Contains(u.LotId))
                         .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                         on j.CustomerCreditId equals vi.CustomerCreditId
                         group vi by new { journal = j, Location = j.Location, customer = j.CustomerCredit.Customer, cusotmerCredit = j.CustomerCredit, currency = j.Currency } into s
                         orderby s.Key.journal.Date
                         where (s.Count() > 0)
                         select new CustomerCreditSummaryOutput
                         {
                             Customer = ObjectMapper.Map<CustomerSummaryOutput>(s.Key.customer),
                             Memo = s.Key.journal.Memo,
                             Id = s.Key.cusotmerCredit.Id,
                             CreditNo = s.Key.journal.JournalNo,
                             Currency = ObjectMapper.Map<CurrencyDetailOutput>(s.Key.currency),
                             Date = s.Key.journal.Date,
                             Total = s.Key.cusotmerCredit.Total,
                             TotalItem = s.Count(),
                             CreationTimeIndex = s.Key.journal.CreationTimeIndex,
                             LocationName = s.Key.Location != null ? s.Key.Location.LocationName : null,

                         };
            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).ThenBy(t => t.CreationTimeIndex).PageBy(input).ToListAsync();
            return new PagedResultDto<CustomerCreditSummaryOutput>(resultCount, @entities);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_Credit_GetCustomerCreditHeader)]
        public async Task<CustomerCreditDetailOutput> GetCustomerCreditItems(EntityDto<Guid> input)
        {
            var @journal = await _journalRepository
                                  .GetAll()
                                  .Include(u => u.Currency)
                                  .Include(u => u.CustomerCredit)
                                  .Include(u => u.Class)
                                  .Include(u => u.Location)
                                  .Include(u => u.CustomerCredit.Customer)
                                  .Include(u => u.CustomerCredit.Customer.Account)
                                  .Include(u => u.CustomerCredit.ShippingAddress)
                                  .Include(u => u.CustomerCredit.BillingAddress)
                                  .AsNoTracking()
                                  .Where(u => u.JournalType == JournalType.CustomerCredit && u.CustomerCreditId == input.Id)
                                  .FirstOrDefaultAsync();

            if (@journal == null || @journal.CustomerCredit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var ARAccount = await (_journalItemRepository.GetAll()
                            .Include(u => u.Account)
                            .AsNoTracking()
                            .Where(u => u.JournalId == journal.Id && u.Key == PostingKey.AR && u.Identifier == null)
                            .Select(u => u.Account)).FirstOrDefaultAsync();

            var customerCreditItems = await _customerCreditDetailRepository.GetAll()
                .Include(u => u.Item)
                .Include(u => u.Lot)
                .Include(u => u.Item.InventoryAccount)
                .Include(u => u.Item.PurchaseAccount)
                .Where(u => u.ItemId != null)
                .Where(u => u.Item.InventoryAccountId != null)
                .Where(u => u.CustomerCreditId == input.Id && u.Item.InventoryAccountId != null)
                .Join(
                    _journalItemRepository.GetAll().Include(u => u.Account).AsNoTracking(),
                    u => u.Id,
                    s => s.Identifier,
                    (bItem, jItem) =>
                    new CustomerCreditDetailInput()
                    {
                        LotDetail = ObjectMapper.Map<LotSummaryOutput>(bItem.Lot),
                        LotId = bItem.LotId,
                        CreationTime = bItem.CreationTime,
                        Id = bItem.Id,
                        Item = ObjectMapper.Map<ItemSummaryOutput>(bItem.Item),
                        ItemId = bItem.ItemId,
                        Description = bItem.Description,
                        Qty = bItem.Qty,
                        Tax = ObjectMapper.Map<TaxSummaryOutput>(bItem.Tax),
                        Total = bItem.Total,
                        UnitCost = bItem.UnitCost,
                        TaxId = bItem.TaxId,
                        Account = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                        AccountId = jItem.AccountId,
                        DiscountRate = bItem.DiscountRate,
                        ItemIssueSaleItemId = bItem.ItemIssueSaleItemId,
                        UseBatchNo = bItem.Item.UseBatchNo,
                        TrackSerial = bItem.Item.TrackSerial,
                        TrackExpiration = bItem.Item.TrackExpiration
                    }).OrderBy(u => u.CreationTime)
            .ToListAsync();


            if (journal.CustomerCredit.ReceiveFrom == ReceiveFrom.ItemIssueSale)
            {
                var itemIssueItemIds = customerCreditItems.Where(s => s.ItemIssueSaleItemId.HasValue).Select(s => s.ItemIssueSaleItemId.Value).ToList();

                if (itemIssueItemIds.Any())
                {
                    var itemIssueList = await _itemIssueItemRepository.GetAll()
                                              .Include(u=>u.DeliveryScheduleItem.DeliverySchedule.SaleOrder)
                                              .Where(s => itemIssueItemIds.Contains(s.Id))
                                              .Where(s => s.SaleOrderItemId.HasValue || s.DeliverySchedulItemId.HasValue)
                                              .AsNoTracking()
                                              .Select(s => new
                                              {
                                                  itemIssueItemId = s.Id,
                                                  SaleOrderId = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrderId :(Guid?)null,
                                                  OrderNumber = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrder.OrderNumber :s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.OrderNumber : null ,
                                                  OrderRef  = s.SaleOrderItem != null ?  s.SaleOrderItem.SaleOrder.Reference  :s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.Reference : null,
                                                  DeliveryId = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliveryScheduleId: (Guid?)null,
                                                  DeliveryNo = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.DeliveryNo : null,
                                                  DeliveryRef = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.Reference :null,
                                              })
                                              .ToListAsync();
                    if (itemIssueList.Any())
                    {
                        customerCreditItems = customerCreditItems.Select(s =>
                        {
                            var i = s;
                            var order = itemIssueList.FirstOrDefault(o => o.itemIssueItemId == s.ItemIssueSaleItemId);
                            if (order != null)
                            {
                                i.OrderId = order.SaleOrderId;
                                i.OrderNo = order.OrderNumber;
                                i.OrderRef = order.OrderRef;
                                i.DeliveryId = order.DeliveryId;
                                i.DeliveryNo = order.DeliveryNo;
                                i.DeliveryRef = order.DeliveryRef;
                            }

                            return i;
                        })
                        .ToList();
                    }
                    else
                    {
                        var invoiceItemList = await _invoiceItemRepository.GetAll()
                                                    .Where(s => s.ItemIssueItemId.HasValue)
                                                    .Where(s => itemIssueItemIds.Contains(s.ItemIssueItemId.Value))
                                                    .Where(s => s.OrderItemId.HasValue || s.DeliverySchedulItemId.HasValue)
                                                    .Include(u=>u.DeliveryScheduleItem.DeliverySchedule.SaleOrder)
                                                    .AsNoTracking()
                                                    .Select(s => new
                                                    {
                                                        itemIssueItemId = s.ItemIssueItemId,
                                                        SaleOrderId = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrderId : (Guid?)null,
                                                        OrderNumber = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrder.OrderNumber : s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.OrderNumber : null,
                                                        OrderRef = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrder.Reference : s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.Reference : null,
                                                        DeliveryId = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliveryScheduleId : (Guid?)null,
                                                        DeliveryNo = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.DeliveryNo : null,
                                                        DeliveryRef = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.Reference : null,
                                                    })
                                                    .ToListAsync();
                        if (invoiceItemList.Any())
                        {
                            customerCreditItems = customerCreditItems.Select(s =>
                            {
                                var i = s;
                                var order = invoiceItemList.FirstOrDefault(o => o.itemIssueItemId == s.ItemIssueSaleItemId);
                                if (order != null)
                                {
                                    i.OrderId = order.SaleOrderId;
                                    i.OrderNo = order.OrderNumber;
                                    i.OrderRef = order.OrderRef;
                                    i.DeliveryId = order.DeliveryId;
                                    i.DeliveryNo = order.DeliveryNo;
                                    i.DeliveryRef = order.DeliveryRef;
                                }

                                return i;
                            })
                       .ToList();
                        }
                    }
                }

            }
            else if (journal.CustomerCredit.ReceiveFrom == ReceiveFrom.ItemReceiptCustomerCredit)
            {
                var itemReceiptItemIds = customerCreditItems.Where(s => s.ItemReceiptCustomerCreditItemId.HasValue)
                                        .Select(s => s.ItemReceiptCustomerCreditItemId.Value).ToList();

                if (itemReceiptItemIds.Any())
                {
                    var itemReceiptList = await _itemReceiptCustomerCreditItemRepository.GetAll()
                                                    .Where(s => s.ItemIssueSaleItemId.HasValue)
                                                    .Where(s => itemReceiptItemIds.Contains(s.Id))
                                                    .AsNoTracking()
                                                    .Select(s => new
                                                    {
                                                        itemIssueItemId = s.ItemIssueSaleItemId.Value,
                                                        itemReceiptItemId = s.Id
                                                    })
                                                    .ToListAsync();

                    if (itemReceiptList.Any())
                    {
                        var itemIssueList = await _itemIssueItemRepository.GetAll()
                                              .Where(s => itemReceiptList.Any(r => r.itemIssueItemId == s.Id))
                                              .Where(s => s.SaleOrderItemId.HasValue || s.DeliverySchedulItemId.HasValue)
                                              .Include(u=>u.DeliveryScheduleItem.DeliverySchedule.SaleOrder)
                                              .AsNoTracking()
                                              .Select(s => new
                                              {
                                                  itemIssueItemId = s.Id,
                                                  SaleOrderId = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrderId : (Guid?)null,
                                                  OrderNumber = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrder.OrderNumber : s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.OrderNumber : null,
                                                  OrderRef = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrder.Reference : s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.Reference : null,
                                                  DeliveryId = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliveryScheduleId : (Guid?)null,
                                                  DeliveryNo = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.DeliveryNo : null,
                                                  DeliveryRef = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.Reference : null,
                                              })
                                              .ToListAsync();
                        if (itemIssueList.Any())
                        {
                            customerCreditItems = customerCreditItems.Select(s =>
                            {
                                var i = s;
                                var receiptItem = itemReceiptList.FirstOrDefault(r => r.itemReceiptItemId == s.ItemReceiptCustomerCreditItemId);
                                var order = itemIssueList.FirstOrDefault(o => o.itemIssueItemId == receiptItem.itemIssueItemId);
                                if (order != null)
                                {
                                    i.OrderId = order.SaleOrderId;
                                    i.OrderNo = order.OrderNumber;
                                    i.OrderRef = order.OrderRef;
                                    i.DeliveryId = order.DeliveryId;
                                    i.DeliveryNo = order.DeliveryNo;
                                    i.DeliveryRef = order.DeliveryRef;
                                }

                                return i;
                            })
                            .ToList();
                        }
                        else
                        {
                            var invoiceItemList = await _invoiceItemRepository.GetAll()
                                                        .Where(s => s.ItemIssueItemId.HasValue)
                                                        .Where(s => itemReceiptList.Any(r => r.itemIssueItemId == s.ItemIssueItemId))
                                                        .Where(s => s.OrderItemId.HasValue || s.DeliverySchedulItemId.HasValue)
                                                        .Include(u=>u.DeliveryScheduleItem.DeliverySchedule.SaleOrder)
                                                        .AsNoTracking()
                                                        .Select(s => new
                                                        {
                                                            itemIssueItemId = s.ItemIssueItemId,
                                                            SaleOrderId = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrderId : (Guid?)null,
                                                            OrderNumber = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrder.OrderNumber : s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.OrderNumber : null,
                                                            OrderRef = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrder.Reference : s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.Reference : null,
                                                            DeliveryId = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliveryScheduleId : (Guid?)null,
                                                            DeliveryNo = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.DeliveryNo : null,
                                                            DeliveryRef = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.Reference : null,
                                                        })
                                                        .ToListAsync();
                            if (invoiceItemList.Any())
                            {
                                customerCreditItems = customerCreditItems.Select(s =>
                                {
                                    var i = s;
                                    var receiptItem = itemReceiptList.FirstOrDefault(r => r.itemReceiptItemId == s.ItemReceiptCustomerCreditItemId);
                                    var order = invoiceItemList.FirstOrDefault(o => o.itemIssueItemId == receiptItem.itemIssueItemId);
                                    if (order != null)
                                    {
                                        i.OrderId = order.SaleOrderId;
                                        i.OrderNo = order.OrderNumber;
                                        i.OrderRef = order.OrderRef;
                                        i.DeliveryId = order.DeliveryId;
                                        i.DeliveryNo = order.DeliveryNo;
                                        i.DeliveryRef = order.DeliveryRef;
                                    }

                                    return i;
                                })
                           .ToList();
                            }
                        }
                    }
                }
            }

            var batchDic = await _customerCreditItemBatchNoRepository.GetAll()
                          .AsNoTracking()
                          .Where(s => s.CustomerCreditItem.CustomerCreditId == input.Id)
                          .Select(s => new BatchNoItemOutput
                          {
                              Id = s.Id,
                              BatchNoId = s.BatchNoId,
                              BatchNumber = s.BatchNo.Code,
                              ExpirationDate = s.BatchNo.ExpirationDate,
                              Qty = s.Qty,
                              TransactionItemId = s.CustomerCreditItemId
                          })
                          .GroupBy(s => s.TransactionItemId)
                          .ToDictionaryAsync(k => k.Key, v => v.ToList());
            if (batchDic.Any())
            {
                foreach (var i in customerCreditItems)
                {
                    if (batchDic.ContainsKey(i.Id.Value)) i.ItemBatchNos = batchDic[i.Id.Value];
                }
            }

            var result = ObjectMapper.Map<CustomerCreditDetailOutput>(journal.CustomerCredit);
            result.Total = customerCreditItems.Sum(t => t.Total);
            result.ClassId = journal.ClassId;
            result.CreditDate = journal.Date;
            result.CreditNo = journal.JournalNo;
            result.CurrencyId = journal.CurrencyId;
            result.Reference = journal.Reference;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.ItemDetail = customerCreditItems;
            result.Account = ObjectMapper.Map<ChartAccountSummaryOutput>(ARAccount);
            result.AccountId = ARAccount.Id;
            result.Memo = journal.Memo;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            result.LocationId = journal.LocationId.Value;
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Credit_POS_Find)]
        public async Task<PagedResultDto<GetListSaleReturn>> FindSaleReturn(ListCustomerCreditInput input)
        {


            var userGroups = await GetUserGroupByLocation();
            var accountCycle = await GetCurrentCycleAsync();
            var query = (from jItem in _journalItemRepository.GetAll()
                         .Include(t => t.Account).AsNoTracking()

                         join j in _journalRepository.GetAll()
                         .Include(t => t.CustomerCredit.Customer)
                         .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                         .Where(u => u.JournalType == JournalType.CustomerCredit && u.Status == TransactionStatus.Publish)
                         .WhereIf(!input.Filter.IsNullOrEmpty(),
                                        u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()))

                         .WhereIf(input.Customers != null && input.Customers.Count > 0, t => input.Customers.Contains(t.CustomerCredit.CustomerId))
                         .WhereIf(input.Locations != null && input.Locations.Count > 0, t => input.Locations.Contains(t.LocationId))
                         .AsNoTracking()
                         on jItem.JournalId equals j.Id

                         where (j.CustomerCredit.OpenBalance > 0 && j.CustomerCredit.IsPOS == true)
                         group jItem by new
                         {
                             JournalNo = j.JournalNo,
                             Id = j.CustomerCreditId,
                             OpenBalance = j.CustomerCredit.OpenBalance,
                             CustomerId = j.CustomerCredit.CustomerId,
                             MultiOpenBalance = j.CustomerCredit.MultiCurrencyOpenBalance,
                             CustomerName = j.CustomerCredit.Customer.CustomerName,
                             Date = j.Date,
                             JournalId = j.Id,
                             multiCurreucy = j.MultiCurrency,

                         } into u
                         select new GetListSaleReturn
                         {

                             MultiOpenBalance = Math.Round(u.Key.MultiOpenBalance, accountCycle.RoundingDigit),

                             Id = u.Key.Id.Value,
                             Date = u.Key.Date,
                             AccountId = u.Where(t => t.Key == PostingKey.AR && t.Identifier == null && t.JournalId == u.Key.JournalId)
                                     .Select(t => t.AccountId).FirstOrDefault(),
                             OpenBalance = Math.Round(u.Key.OpenBalance, accountCycle.RoundingDigit),
                             JournalNo = u.Key.JournalNo,
                             CustomerId = u.Key.CustomerId,
                             CustomerName = u.Key.CustomerName,
                             MultiCurrency = ObjectMapper.Map<CurrencyDetailOutput>(u.Key.multiCurreucy),
                         }).Where(u => u.OpenBalance > 0);
            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            return new PagedResultDto<GetListSaleReturn>(resultCount, @entities);

        }

        #region import export excel

        private ReportOutput GetReportTemplateItemReceiptOther(bool hasClassFeature)
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
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
                        ColumnName = "Reference",
                        ColumnLength = 250,
                        ColumnTitle = "Reference",
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
                        ColumnName = "CustomerCreditNo",
                        ColumnLength = 250,
                        ColumnTitle = "CustomerCreditNo",
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
                        ColumnName = "Currency",
                        ColumnLength = 130,
                        ColumnTitle = "Currency",
                        ColumnType = ColumnType.String,
                        SortOrder = 8,
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
                        ColumnTitle = "Amount",
                        ColumnType = ColumnType.Money,
                        SortOrder = 9,
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
                        ColumnTitle = "Amount In Accounting",
                        ColumnType = ColumnType.String,
                        SortOrder = 10,
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
                        SortOrder = 11,
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
                        SortOrder = 12,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },

                },
                Groupby = "",
                HeaderTitle = "CustomerCredit",
                Sortby = "",
            };
            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_Credit_ExportCustomerCreditTamplate)]
        public async Task<FileDto> ExportExcelTamplate()
        {
            var result = new FileDto();
            var sheetName = "CustomerCredit";
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
                var headerList = GetReportTemplateItemReceiptOther(hasClassFeature);
                var reportCollumnHeader = headerList.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
                foreach (var i in reportCollumnHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);

                    colHeaderTable += 1;
                }
                #endregion Row 1

                result.FileName = $"CustomerCreditTamplate.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_Credit_Import)]
        public async Task ImportExcel(FileDto input)
        {
            //var excelPackage = Read(input, _appFolders);
            var excelPackage = await Read(input);
            var hasClassFeature = IsEnabled(AppFeatures.SetupFeatureClasss);
            var indexHasClassFeature = hasClassFeature ? 0 : -1;
            var @accounts = await _chartOfAccountRepository.GetAll().AsNoTracking().ToListAsync();
            var @locations = await _locationRepository.GetAll().AsNoTracking().ToListAsync();
            var @class = await _classRepository.GetAll().AsNoTracking().ToListAsync();
            var tenant = await GetCurrentTenantAsync();
            var userId = AbpSession.GetUserId();
            var currency = await _currencyRepository.GetAll().AsNoTracking().ToListAsync();
            var customer = await _customerRepository.GetAll().AsNoTracking().ToListAsync();
            // var tax = await _taxRepository.GetAll().Where(t => t.TaxName == "No Tax").Select(s => s.Id).ToListAsync();
            var journal = await _journalRepository.GetAll().Where(s => s.JournalType == JournalType.CustomerCredit).ToListAsync();
            if (excelPackage != null)
            {
                // Get the work book in the file
                var workBook = excelPackage.Workbook;
                if (workBook != null)
                {
                    // retrive first worksheets
                    var worksheet = excelPackage.Workbook.Worksheets[0];

                    for (int i = worksheet.Dimension.Start.Row; i <= worksheet.Dimension.End.Row; i++)
                    {
                        var date = worksheet.Cells[i, 1].Value?.ToString();
                        var customerCode = worksheet.Cells[i, 2].Value?.ToString();
                        var aRAccountCode = worksheet.Cells[i, 3].Value?.ToString();
                        var locationName = worksheet.Cells[i, 4].Value?.ToString();
                        var className = hasClassFeature ? worksheet.Cells[i, 5].Value?.ToString() : "";
                        var reference = worksheet.Cells[i, 6 + indexHasClassFeature].Value?.ToString();
                        var customerCreditNo = worksheet.Cells[i, 7 + indexHasClassFeature].Value?.ToString();
                        var currencyCode = worksheet.Cells[i, 8 + indexHasClassFeature].Value?.ToString();
                        var amount = worksheet.Cells[i, 9 + indexHasClassFeature].Value.ToString();
                        var amountInAccount = worksheet.Cells[i, 10 + indexHasClassFeature].Value.ToString();
                        var itemAccountCode = worksheet.Cells[i, 11 + indexHasClassFeature].Value?.ToString();
                        var itemDescription = worksheet.Cells[i, 12 + indexHasClassFeature].Value?.ToString();

                        var currencyId = currency.Where(s => s.Code == currencyCode).FirstOrDefault();
                        var classId = hasClassFeature ? @class.Where(s => s.ClassName == className).Select(t => t.Id).FirstOrDefault() : tenant.ClassId;
                        var customerId = customer.Where(s => s.CustomerCode == customerCode).FirstOrDefault();
                        var itemAccountId = accounts.Where(s => s.AccountCode == itemAccountCode).FirstOrDefault();
                        var arAccountId = accounts.Where(s => s.AccountCode == aRAccountCode).FirstOrDefault();
                        var locationId = locations.Where(s => s.LocationName == locationName).FirstOrDefault();
                        var taxId = accounts.Where(s => s.AccountCode == itemAccountCode).Select(t => t.TaxId).FirstOrDefault();

                        if (i > 1)
                        {

                            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Invoice);

                            if (auto.CustomFormat == false && customerCreditNo == null)
                            {
                                throw new UserFriendlyException(L("NoInvoiceNoFound"));
                            }

                            if (taxId == null)
                            {
                                throw new UserFriendlyException(L("NoTaxFound"));
                            }
                            if (currencyId == null)
                            {
                                throw new UserFriendlyException(L("NoCurrencyFound"));
                            }
                            if (classId == null)
                            {
                                throw new UserFriendlyException(L("NoClassFound"));
                            }
                            if (customerId == null)
                            {
                                throw new UserFriendlyException(L("NoCustomerFound"));
                            }
                            if (itemAccountId == null)
                            {
                                throw new UserFriendlyException(L("NoItemAccountFound"));

                            }
                            if (arAccountId == null)
                            {
                                throw new UserFriendlyException(L("NoARAccountFound"));
                            }
                            if (locationId == null)
                            {
                                throw new UserFriendlyException(L("NoLocationFound"));
                            }
                            CAddress billAddress = new CAddress("", "", "", "", "");
                            CAddress shipAddress = new CAddress("", "", "", "", "");
                            var createInput = new CreateCustomerCreditInput()
                            {
                                BillingAddress = billAddress,
                                ShippingAddress = shipAddress,
                                CustomerCreditDetail = new List<CustomerCreditDetailInput>
                               {
                                  new CustomerCreditDetailInput
                                  {
                                      Id = new Guid(),
                                      Description = itemDescription,
                                      DiscountRate = 0,
                                      AccountId = itemAccountId.Id,
                                      ItemId = null,
                                      LotId = null,
                                      MultiCurrencyTotal= Convert.ToDecimal(amount),
                                      MultiCurrencyUnitCost = Convert.ToDecimal(amount),
                                      Qty = 1,
                                      TaxId = taxId,
                                      Total = Convert.ToDecimal(amountInAccount),
                                      UnitCost = Convert.ToDecimal(amountInAccount),

                                  }

                               },
                                ReceiveFrom = ReceiveFrom.None,
                                CustomerCreditNo = customerCreditNo,
                                ClassId = classId,
                                AccountId = arAccountId.Id,
                                ConvertToItemReceipt = false,
                                CurrencyId = tenant.CurrencyId.Value,
                                CreditDate = Convert.ToDateTime(date),
                                ReceiveDate = Convert.ToDateTime(date),
                                LocationId = locationId.Id,
                                Memo = "Opening Balance",
                                MultiCurrencyId = currencyId.Id,
                                MultiCurrencySubTotal = Convert.ToDecimal(amount),
                                MultiCurrencyTax = 0,
                                MultiCurrencyTotal = Convert.ToDecimal(amount),
                                DueDate = Convert.ToDateTime(date),
                                CustomerId = customerId.Id,
                                Reference = reference,
                                SameAsShippingAddress = true,
                                Status = TransactionStatus.Publish,
                                SubTotal = Convert.ToDecimal(amountInAccount),
                                Tax = 0,
                                Total = Convert.ToDecimal(amountInAccount),

                            };
                            if (journal.Where(t => t.JournalNo == createInput.CustomerCreditNo).Count() == 0)
                            {
                                await Create(createInput);
                            }
                        }

                    }
                }
            }
            //RemoveFile(input, _appFolders);
        }
        #endregion

        [AbpAuthorize(AppPermissions.Pages_Tenant_CleanRounding)]
        public async Task CleanRoundingPaidStatus(CarlEntityDto input)
        {
            var currenctPeriod = await GetCurrentCycleAsync();
            var roundDigit = currenctPeriod == null ? 2 : currenctPeriod.RoundingDigit;

            var customerCredit = await _customerCreditManager.GetAsync(input.Id);
            if (customerCredit == null) throw new UserFriendlyException(L("RecordNotFound"));

            var journal = await _journalRepository.FirstOrDefaultAsync(s => s.CustomerCreditId == input.Id);
            if (journal == null) throw new UserFriendlyException(L("RecordNotFound"));

            var customerCreditItems = await _customerCreditDetailRepository.GetAll().Where(s => s.CustomerCreditId == input.Id).ToListAsync();
            var journalItems = await _journalItemRepository.GetAll().Where(s => s.JournalId == journal.Id).ToListAsync();

            decimal subTotal = 0;
            foreach (var vItem in customerCreditItems)
            {
                var lineTotal = Math.Round(vItem.Total, roundDigit);
                vItem.SetTotal(lineTotal);
                await _customerCreditDetailManager.UpdateAsync(vItem);

                var journalItem = journalItems.FirstOrDefault(s => s.Identifier == vItem.Id);
                if (journalItem == null) throw new UserFriendlyException(L("RecordNotFound"));

                journalItem.SetDebitCredit(lineTotal, 0);
                await _journalItemManager.UpdateAsync(journalItem);

                subTotal += lineTotal;
            }

            decimal tax = Math.Round(customerCredit.Tax, roundDigit);
            decimal total = subTotal + tax;
            decimal openBalance = total - customerCredit.TotalPaid;

            customerCredit.SetSubTotal(subTotal);
            customerCredit.SetTax(tax);
            customerCredit.SetTotal(total);
            customerCredit.SetOpenBalance(openBalance);
            customerCredit.UpdatePaidStatus(customerCredit.TotalPaid == 0 ? PaidStatuse.Pending : openBalance == 0 ? PaidStatuse.Paid : PaidStatuse.Partial);
            await _customerCreditManager.UpdateAsync(customerCredit);

            journal.SetDebitCredit(total);
            await _journalManager.UpdateAsync(journal, DocumentType.CustomerCredit, false);

            var headerJournalItem = journalItems.FirstOrDefault(s => s.Identifier == null && s.Key == PostingKey.AR);
            if (headerJournalItem == null) throw new UserFriendlyException(L("RecordNotFound"));

            headerJournalItem.SetDebitCredit(0, total);
            await _journalItemManager.UpdateAsync(headerJournalItem);

        }
    }
}
