using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using CorarlERP.Items.Dto;
using CorarlERP.Journals;
using CorarlERP.Journals.Dto;
using CorarlERP.Taxes.Dto;
using CorarlERP.VendorCredit.Dto;
using CorarlERP.Vendors;
using CorarlERP.Vendors.Dto;
using Microsoft.EntityFrameworkCore;
using static CorarlERP.enumStatus.EnumStatus;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using CorarlERP.ItemIssueVendorCredits;
using CorarlERP.Items;
using CorarlERP.ItemReceipts;
using CorarlERP.ItemIssues;
using CorarlERP.ItemReceipts.Dto;

using CorarlERP.ItemReceiptCustomerCredits;
using CorarlERP.PayBills;
using CorarlERP.ItemIssueVendorCredits.Dto;
using static CorarlERP.ItemIssueVendorCredits.ItemIssueVendorCredit;
using CorarlERP.MultiTenancy;
using CorarlERP.Inventories;
using CorarlERP.AutoSequences;
using CorarlERP.Lots.Dto;
using CorarlERP.Lots;
using CorarlERP.Locations.Dto;
using CorarlERP.UserGroups;
using CorarlERP.Reports;
using CorarlERP.Reports.Dto;
using CorarlERP.Dto;
using System.IO;
using OfficeOpenXml;
using CorarlERP.ReportTemplates;
using CorarlERP.Locations;
using CorarlERP.Classes;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Taxes;
using CorarlERP.Addresses;
using CorarlERP.Locks;
using CorarlERP.Common.Dto;
using CorarlERP.Settings;
using Abp.Dependency;
using CorarlERP.FileStorages;
using CorarlERP.InventoryCalculationJobSchedules;
using CorarlERP.InventoryCalculationItems;
using CorarlERP.BatchNos;
using CorarlERP.Invoices.Dto;
using CorarlERP.CustomerCredits;
using CorarlERP.Features;
using CorarlERP.JournalTransactionTypes;
using CorarlERP.Bills;
using CorarlERP.Exchanges.Dto;

namespace CorarlERP.VendorCredit
{
    // Create By @Huy Sokhom
    // Creation Date: 19-06-2018
    [AbpAuthorize]
    public class VendorCreditAppService : ReportBaseClass, IVendorCreditAppService
    {
        private readonly AppFolders _appFolders;
        private readonly IInventoryManager _inventoryManager;
        private readonly ITenantManager _tenantManager;
        private readonly IRepository<Tenant, int> _tenantRepository;

        private readonly IItemIssueVendorCreditItemManager _itemIssueVendorCreditItemManager;
        private readonly IItemIssueVendorCreditManager _itemIssueVendorCreditManager;

        private readonly IRepository<ItemReceiptItem, Guid> _itemReceiptItemRepository;
        private readonly IRepository<ItemIssueItem, Guid> _itemIssueItemRepository;

        private readonly IRepository<ItemReceiptItemCustomerCredit, Guid> _itemReceiptCustomerCreditItemRepository;
        private readonly IRepository<ItemIssueVendorCreditItem, Guid> _itemIssueVendorCreditItemRepository;


        private readonly IRepository<ItemIssueVendorCredit, Guid> _itemIssueVendorCreditRepository;

        private readonly IRepository<Lot, long> _lotRepository;
        private readonly IRepository<ItemIssueVendorCredit, Guid> _itemIssueVendorCredit;
        private readonly IRepository<Item, Guid> _itemRepository;
        // Vendor Credit
        private readonly ICurrencyManager _currencyManager;
        private readonly IRepository<Currency, long> _currencyRepository;

        private readonly IVendorManager _vendorItemManager;
        private readonly IRepository<Vendor, Guid> _vendorRepository;

        private readonly IVendorCreditDetailManager _vendorCreditDetailManager;
        private readonly IRepository<VendorCreditDetail, Guid> _vendorCreditDetailRepository;
        private readonly IVendorCreditManager _vendorCreditManager;
        private readonly IRepository<VendorCredit, Guid> _vendorCreditRepository;
        private readonly ICorarlRepository<VendorCreditExchangeRate, Guid> _exchangeRateRepository;

        // Journal 
        private readonly IJournalManager _journalManager;
        private readonly IRepository<Journal, Guid> _journalRepository;
        private readonly IJournalItemManager _journalItemManager;
        private readonly IRepository<JournalItem, Guid> _journalItemRepository;

        //paybill validate 
        private readonly IRepository<PayBill, Guid> _payBillRepository;
        private readonly IRepository<PayBillDetail, Guid> _payBillDetailRepository;
        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;

        private readonly IRepository<Location, long> _locationRepository;
        private readonly IRepository<Class, long> _classRepository;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;
        private readonly IRepository<Tax, long> _taxRepository;
        private readonly IRepository<Lock, long> _lockRepository;
        private readonly IRepository<BillInvoiceSetting, long> _settingRepository;
        private readonly IFileStorageManager _fileStorageManager;
        private readonly IInventoryCalculationItemManager _inventoryCalculationItemManager;
        private readonly ICorarlRepository<BatchNo, Guid> _batchNoRepository;
        private readonly ICorarlRepository<ItemReceiptItemBatchNo, Guid> _itemReceiptItemBatchNoRepository;
        private readonly ICorarlRepository<VendorCreditItemBatchNo, Guid> _vendorCreditItemBatchNoRepository;
        private readonly ICorarlRepository<ItemIssueVendorCreditItemBatchNo, Guid> _itemIssueVendorCreditItemBatchNoRepository;
        private readonly IJournalTransactionTypeManager _journalTransactionTypeManager;
        public VendorCreditAppService(
            ICorarlRepository<VendorCreditExchangeRate, Guid> exchangeRateRepository,
            IRepository<Lot, long> lotRepository,
            IJournalManager journalManager,
            IJournalItemManager journalItemManager,
            ItemIssueVendorCreditItemManager itemIssueVendorCreditItemManager,
            IRepository<Journal, Guid> journalRepository,
            IRepository<JournalItem, Guid> journalItemRepository,
            VendorManager vendorManager,
            IFileStorageManager fileStorageManager,
            ICurrencyManager currencyManager,
            IRepository<Vendor, Guid> vendorRepository,
            IRepository<Currency, long> currencyRepository,
            VendorCreditManager vendorCreditManager,
            VendorCreditDetailManager vendorCreditDetailManager,
            IRepository<VendorCredit, Guid> vendorCreditRepository,
            IRepository<VendorCreditDetail, Guid> vendorCreditDetailRepository,
            IRepository<ItemIssueVendorCredit, Guid> itemIssueVendorCredit,
            IRepository<Item, Guid> itemRepository,
            IRepository<ItemReceiptItem, Guid> itemReceiptItemRepository,
            IRepository<Locations.Location, long> locationRepository,
            IRepository<ItemIssueItem, Guid> itemIssueItemRepository,
            IRepository<ItemReceiptItemCustomerCredit, Guid> itemReceiptCustomerCreditItemRepository,
            IRepository<ItemIssueVendorCreditItem, Guid> itemIssueVendorCreditItemRepository,
            IRepository<PayBill, Guid> payBillRepository,
            ItemIssueVendorCreditManager itemIssueVendorCreditManager,
            ICorarlRepository<BatchNo, Guid> batchNoRepository,
            ICorarlRepository<ItemReceiptItemBatchNo, Guid> itemReceiptItemBatchNoRepository,
            ICorarlRepository<VendorCreditItemBatchNo, Guid> vendorCreditItemBatchNoRepository,
            ICorarlRepository<ItemIssueVendorCreditItemBatchNo, Guid> itemIssueVendorCreditItemBatchNoRepository,
            ITenantManager tenantManager,
            IRepository<Tenant, int> tenantRepository,
            IInventoryManager inventoryManager,
            IRepository<AutoSequence, Guid> autoSequenceRepository,
            IAutoSequenceManager autoSequenceManager,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            IRepository<Class, long> classRepository,
            IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
            IRepository<Tax, long> taxRepository,
            IRepository<PayBillDetail, Guid> payBillDetailRepository,
            IRepository<ItemIssueVendorCredit, Guid> itemIssueVendorCreditRepository,
            IRepository<AccountCycles.AccountCycle, long> accountCycleRepository,
            IInventoryCalculationItemManager inventoryCalculationItemManager,
            IRepository<Lock, long> lockRepository,
            IJournalTransactionTypeManager journalTransactionTypeManager,
             AppFolders appFolders
        ) : base(accountCycleRepository, appFolders, userGroupMemberRepository, locationRepository)
        {
            _payBillDetailRepository = payBillDetailRepository;
            _itemIssueVendorCreditRepository = itemIssueVendorCreditRepository;
            _locationRepository = locationRepository;
            _classRepository = classRepository;
            _taxRepository = taxRepository;
            _chartOfAccountRepository = chartOfAccountRepository;
            _appFolders = appFolders;
            _lotRepository = lotRepository;
            _inventoryManager = inventoryManager;
            _tenantManager = tenantManager;
            _tenantRepository = tenantRepository;
            _fileStorageManager = fileStorageManager;

            _itemIssueVendorCreditItemManager = itemIssueVendorCreditItemManager;
            _itemIssueVendorCreditManager = itemIssueVendorCreditManager;
            _inventoryCalculationItemManager = inventoryCalculationItemManager;

            _journalManager = journalManager;
            _journalRepository = journalRepository;
            _journalItemManager = journalItemManager;
            _journalItemRepository = journalItemRepository;

            _vendorCreditManager = vendorCreditManager;
            _vendorCreditRepository = vendorCreditRepository;
            _vendorCreditDetailManager = vendorCreditDetailManager;
            _vendorCreditDetailRepository = vendorCreditDetailRepository;

            _vendorRepository = vendorRepository;
            _vendorItemManager = vendorManager;
            _currencyRepository = currencyRepository;
            _currencyManager = currencyManager;
            _itemRepository = itemRepository;
            _itemIssueVendorCredit = itemIssueVendorCredit;
            _itemIssueItemRepository = itemIssueItemRepository;
            _itemReceiptItemRepository = itemReceiptItemRepository;

            _itemReceiptCustomerCreditItemRepository = itemReceiptCustomerCreditItemRepository;
            _itemIssueVendorCreditItemRepository = itemIssueVendorCreditItemRepository;
            _payBillRepository = payBillRepository;
            _autoSequenceManager = autoSequenceManager;
            _autoSequenceRepository = autoSequenceRepository;
            _lockRepository = lockRepository;
            _settingRepository = IocManager.Instance.Resolve<IRepository<BillInvoiceSetting, long>>();
            _batchNoRepository = batchNoRepository;
            _itemIssueVendorCreditItemBatchNoRepository = itemIssueVendorCreditItemBatchNoRepository;
            _vendorCreditItemBatchNoRepository = vendorCreditItemBatchNoRepository;
            _itemReceiptItemBatchNoRepository = itemReceiptItemBatchNoRepository;
            _journalTransactionTypeManager = journalTransactionTypeManager;
            _exchangeRateRepository = exchangeRateRepository;
        }

        /* Functionality for check if item or can check it a list of array is account tap from Frontend */
        private bool checkIfItemExist(List<VendorCreditDetailInput> array, string value)
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

        private async Task ValidateBatchNo(CreateVendorCreditInput input)
        {
            if (!input.VendorCreditDetail.Any(s => s.ItemId.HasValue && s.ItemId != Guid.Empty)) return;

            var validateItems = await _itemRepository.GetAll()
                            .Where(s => input.VendorCreditDetail.Any(i => i.ItemId == s.Id))
                            .Where(s => s.UseBatchNo || s.TrackSerial || s.TrackExpiration)
                            .AsNoTracking()
                            .ToListAsync();

            if (!validateItems.Any()) return;

            var batchItemDic = validateItems.ToDictionary(k => k.Id, v => v);

            var itemUseBatchs = input.VendorCreditDetail.Where(s => batchItemDic.ContainsKey(s.ItemId.Value)).ToList();

            var find = itemUseBatchs.Where(s => s.ItemBatchNos == null || s.ItemBatchNos.Count == 0 || s.ItemBatchNos.Any(r => r.BatchNoId == Guid.Empty)).FirstOrDefault();
            if (find != null) throw new UserFriendlyException(L("PleaseSelect", $"{L("BatchNo")}/{L("SerialNo")}") + $", Item: {find.Item.ItemCode}-{find.Item.ItemName}");

            var serialItemHash = validateItems.Where(s => s.TrackSerial).Select(s => s.Id).ToHashSet();
            var serialQty = input.VendorCreditDetail.Where(s => serialItemHash.Contains(s.ItemId.Value)).FirstOrDefault(s => s.ItemBatchNos.Any(b => b.Qty != 1));
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

            if (!input.convertToItemIssueVendor && input.ReceiveFrom != ReceiveFrom.ItemIssueVendorCredit) return;

            var itemIds = itemUseBatchs.GroupBy(s => s.ItemId).Select(s => s.Key.Value).ToList();
            var lots = itemUseBatchs.GroupBy(s => s.FromLotId).Select(s => s.Key.Value).ToList();
            var exceptIds = new List<Guid>();
            if (input.ItemIssueVendorCreditId.HasValue) exceptIds.Add(input.ItemIssueVendorCreditId.Value);
           

            var batchBalanceItems = await _inventoryManager.GetItemBatchNoBalance(itemIds, lots, batchNoIds, input.IssueDate.Value, exceptIds);
            var balanceDic = batchBalanceItems.ToDictionary(k => $"{k.ItemId}-{k.LotId}-{k.BatchNoId}", v => v.Qty);

            var batchQtyUse = itemUseBatchs
                              .SelectMany(s => s.ItemBatchNos.Select(r => new { s.ItemId, s.FromLotId, r.BatchNoId, r.Qty }))
                              .GroupBy(s => new { s.ItemId, s.FromLotId, s.BatchNoId })
                              .Select(s => new
                              {
                                  ItemName = $"{batchItemDic[s.Key.ItemId.Value].ItemCode}-{batchItemDic[s.Key.ItemId.Value].ItemName}, BatchNo: {batchNoDic[s.Key.BatchNoId].Code}",
                                  Key = $"{s.Key.ItemId}-{s.Key.FromLotId}-{s.Key.BatchNoId}",
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

        private async Task CheckStockForPurchaseReturn(Guid itemReceiptPurchaseId, List<VendorCreditDetailInput> inputItems, List<Guid> exceptIds)
        {
            var receiptQuery = from iri in _itemReceiptItemRepository.GetAll()
                                            .Where(u => u.ItemReceiptId == itemReceiptPurchaseId)
                                            .AsNoTracking()
                               join b in _itemReceiptItemBatchNoRepository.GetAll()
                                         .AsNoTracking()
                               on iri.Id equals b.ItemReceiptItemId
                               into bs
                               from b in bs.DefaultIfEmpty()
                               select new
                               {
                                   iri.Id,
                                   Qty = b == null ? iri.Qty : b.Qty,
                                   BatchNoId = b == null ? (Guid?)null : b.BatchNoId,
                                   iri.ItemId
                               };

            var cQuery = from r in _vendorCreditDetailRepository.GetAll()
                                    .Where(s => s.ItemReceiptItemId != null)
                                    .WhereIf(exceptIds != null && exceptIds.Any(), s => !exceptIds.Contains(s.VendorCreditId))
                                    .AsNoTracking()
                         join b in _vendorCreditItemBatchNoRepository.GetAll()
                                   .AsNoTracking()
                         on r.Id equals b.VendorCreditItemId
                         into bs
                         from b in bs.DefaultIfEmpty()
                         select new
                         {
                             r.ItemReceiptItemId,
                             Qty = b == null ? r.Qty : b.Qty,
                             BatchNoId = b == null ? (Guid?)null : b.BatchNoId
                         };

            var rQuery = from r in _itemIssueVendorCreditItemRepository.GetAll()
                                   .Where(s => s.ItemReceiptPurchaseItemId != null)
                                   .WhereIf(exceptIds != null && exceptIds.Any(), s => !exceptIds.Contains(s.ItemIssueVendorCreditId))
                                   .AsNoTracking()
                         join b in _itemIssueVendorCreditItemBatchNoRepository.GetAll().AsNoTracking()
                         on r.Id equals b.ItemIssueVendorCreditItemId
                         into bs
                         from b in bs.DefaultIfEmpty()
                         select new
                         {
                             r.ItemReceiptPurchaseItemId,
                             Qty = b == null ? r.Qty : b.Qty,
                             BatchNoId = b == null ? (Guid?)null : b.BatchNoId
                         };



            var query = from iri in receiptQuery
                        join returnItem in cQuery
                        on $"{iri.Id}-{iri.BatchNoId}" equals $"{returnItem.ItemReceiptItemId}-{returnItem.BatchNoId}"
                        into returnItems

                        join returnItem2 in rQuery
                        on $"{iri.Id}-{iri.BatchNoId}" equals $"{returnItem2.ItemReceiptPurchaseItemId}-{returnItem2.BatchNoId}"
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
                        var checkStock = balanceList.FirstOrDefault(i => i.Id == r.ItemReceiptItemPurchaseId && i.ItemId == r.ItemId && i.BatchNoId == b.BatchNoId && i.Qty < b.Qty);
                        if (checkStock != null) throw new UserFriendlyException(L("ReturnQtyCannotGreaterThanPurchaseQty"));
                    }
                }
                else
                {
                    var checkStock = balanceList.FirstOrDefault(i => i.Id == r.ItemReceiptItemPurchaseId && i.ItemId == r.ItemId && i.Qty < r.Qty);
                    if (checkStock != null) throw new UserFriendlyException(L("ReturnQtyCannotGreaterThanPurchaseQty"));
                }
            }
        }


        private async Task CalculateTotal(CreateVendorCreditInput input)
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
            foreach (var item in input.VendorCreditDetail)
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

        void ValidateExchangeRate(CreateVendorCreditInput input)
        {
            if (!input.UseExchangeRate || input.CurrencyId == input.MultiCurrencyId) return;

            if (input.ExchangeRate == null) throw new UserFriendlyException(L("IsRequired", L("ExchangeRate")));
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_CanCreateVendorCredit)]
        public async Task<NullableIdDto<Guid>> Create(CreateVendorCreditInput input)
        {
            if(input.IsConfirm == false)
             {
                var locktransaction = await _lockRepository.GetAll()
                                      .Where(t => (t.LockKey == TransactionLockType.Bill)
                                                  && t.IsLock == true && t.LockDate != null  && t.LockDate.Value.Date >= input.CreditDate.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            ValidateExchangeRate(input);
            await ValidateBatchNo(input);
          

            if (input.ReceiveFrom == ReceiveFrom.ItemReceiptPurchase)
            {
                await CheckStockForPurchaseReturn(input.ItemReceiptPurchaseId.Value, input.VendorCreditDetail, null);
               
            }

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.VendorCredit);

            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.VendorCreditNo = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }

            if (input.VendorCreditDetail.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }

            await CalculateTotal(input);

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            // step 1. insert to journal header
            var @entity = Journal.Create(
                tenantId, userId, input.VendorCreditNo, input.CreditDate, input.Memo,
               input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, input.LocationId
            );
            entity.UpdateStatus(input.Status);
            if (input.MultiCurrencyId == input.CurrencyId || input.MultiCurrencyId == null || input.MultiCurrencyId == 0)
            {
                input.MultiCurrencyId = input.CurrencyId;
                input.MultiCurrencyTotal = input.Total;
                input.MultiCurrencySubTotal = input.SubTotal;
                input.MultiCurrencyTax = input.Tax;

            }
            entity.UpdateMultiCurrency(input.MultiCurrencyId);
            // step 2. insert into journal item with has default debit and credit auto zero
            var journalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, input.AccountId, input.Memo, input.Total, 0, PostingKey.AP, null);

            var isItem = input.VendorCreditDetail.Any(s => s.ItemId != null && s.ItemId != Guid.Empty);

            // step 3. insert into vendor credit header
            var vendorCredit = VendorCredit.Create(tenantId, userId, input.VendorId,
                input.SameAsShippingAddress,
                input.ShippingAddress, input.BillingAddress,
                input.SubTotal, input.Tax, input.Total, input.DueDate, input.IssueDate,
                input.convertToItemIssueVendor, input.MultiCurrencySubTotal, input.MultiCurrencyTax, input.MultiCurrencyTotal, input.ItemReceiptPurchaseId, input.ReceiveFrom, isItem, input.UseExchangeRate);                     
            //update itemIssue credit when create vendor credit from item Issue vendor credit
            if (input.ReceiveFrom == ReceiveFrom.ItemIssueVendorCredit && input.ItemIssueVendorCreditId != null)
            {

                #region Calculat Cost
                //var roundingId = (await GetCurrentTenantAsync()).RoundDigitAccountId;
                var accounts = await _itemRepository.GetAll().Where(t => t.TenantId == tenantId && input.VendorCreditDetail.Any(v => v.ItemId == t.Id)).ToListAsync();

                var itemToCalculateCost = input.VendorCreditDetail.Select((u, index1) => new Inventories.Data.GetAvgCostForIssueData()
                {
                    ItemIssueItemId = input.ItemIssueVendorCreditId,
                    Index = index1,
                    ItemId = u.ItemId.Value,
                    ItemCode = u.Item.ItemCode,
                    Qty = u.Qty,
                    LotId = u.FromLotId,
                    InventoryAccountId = (from Account in accounts where (Account.Id == u.ItemId) select (Account.InventoryAccountId.Value)).FirstOrDefault(),
                }).ToList();
                var lotIds = input.VendorCreditDetail.Select(x => x.FromLotId).ToList();
                var locationIds = await _lotRepository.GetAll().AsNoTracking()
                                .Where(t => lotIds.Contains(t.Id))
                                .Select(t => (long?)t.LocationId)
                                .ToListAsync();
                var getCostResult = await _inventoryManager.GetAvgCostForIssues(input.IssueDate.Value, locationIds, itemToCalculateCost/*, entity, roundingId*/);

                foreach (var r in getCostResult.Items)
                {

                    input.VendorCreditDetail[r.Index].ItemIssueVendorCreditUnitCost = r.UnitCost;
                    input.VendorCreditDetail[r.Index].ItemIssueVendorCreditTotal = r.LineCost;
                }
                var TotalItemIssue = getCostResult.Total;

                #endregion Calculat Cost

                var ItemIssueJournal = await _journalRepository.GetAll()
                                            .Include(u => u.ItemIssueVendorCredit)
                                            .Where(t => t.ItemIssueVendorCreditId == input.ItemIssueVendorCreditId).FirstOrDefaultAsync();

                if (ItemIssueJournal != null && ItemIssueJournal.ItemIssueVendorCredit != null)
                {
                    //update item issue

                    var validateLockDate = await _lockRepository.GetAll()
                                  .Where(t => t.IsLock == true &&
                                  (t.LockDate.Value.Date >= ItemIssueJournal.Date.Date || t.LockDate.Value.Date >= input.IssueDate.Value.Date)
                                  && (t.LockKey == TransactionLockType.ItemIssue || t.LockKey == TransactionLockType.InventoryTransaction)).CountAsync();

                    var itemIssueItems = await _itemIssueVendorCreditItemRepository.GetAll()
                                      .Where(s => s.ItemIssueVendorCreditId == input.ItemIssueVendorCreditId)
                                      .AsNoTracking()
                                      .ToListAsync();

                    var isChangeItems = input.VendorCreditDetail.Count() != itemIssueItems.Count() || input.VendorCreditDetail.Any(s => itemIssueItems.Any(r => r.Id == s.ItemIssueItemVendorCreditId && (r.ItemId != s.ItemId || r.LotId != s.FromLotId || r.Qty != s.Qty)));


                    if (validateLockDate > 0 && (ItemIssueJournal.Date.Date != input.IssueDate.Value.Date || isChangeItems))
                    {
                        throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                    }

                    ItemIssueJournal.UpdateJournalDate(input.IssueDate.Value);
                    ItemIssueJournal.ItemIssueVendorCredit.UpdateTotal(TotalItemIssue);
                    ItemIssueJournal.UpdateCreditDebit(TotalItemIssue, TotalItemIssue);                 
                    ItemIssueJournal.ItemIssueVendorCredit.UpdateVendorCreditId(vendorCredit.Id);

                    var setting = await _settingRepository.FirstOrDefaultAsync(s => s.SettingType == BillInvoiceSettingType.Bill);

                    if (setting == null || setting.ReferenceSameAsGoodsMovement) ItemIssueJournal.UpdateReference(input.Reference);

                    var autoIss = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue_VendorCredit);
                    CheckErrors(await _journalManager.UpdateAsync(ItemIssueJournal, autoIss.DocumentType));
                    CheckErrors(await _itemIssueVendorCreditManager.UpdateAsync(ItemIssueJournal.ItemIssueVendorCredit));
                }                                                   
                vendorCredit.UpdateShipedStatus(DeliveryStatus.ShipAll);
            }
            //Check if credit list item detail has no item ID so it mean no receive status 
            var isAccountTap = checkIfItemExist(input.VendorCreditDetail, null);
            if (isAccountTap)
            {
                vendorCredit.UpdateShipedStatus(DeliveryStatus.NoReceive);
            }

            @entity.UpdateVendorCredit(vendorCredit);
            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));
            CheckErrors(await _journalItemManager.CreateAsync(journalItem));
            CheckErrors(await _vendorCreditManager.CreateAsync(vendorCredit));

            if(input.UseExchangeRate && input.CurrencyId != input.MultiCurrencyId)
            {
                var exchange = VendorCreditExchangeRate.Create(tenantId, userId, vendorCredit.Id, input.ExchangeRate.FromCurrencyId, input.ExchangeRate.ToCurrencyId, input.ExchangeRate.Bid, input.ExchangeRate.Ask);
                await _exchangeRateRepository.InsertAsync(exchange);
            }

            // Loop item list to insert into detail vendor credit 
            int index = 0;
            var itemIssueVendorCreditItemBatchNos = new List<ItemIssueVendorCreditItemBatchNo>();
            var itemIssueVendorCreditItems = new List<ItemIssueVendorCreditItem>();
            var @inventoryJournalItems = new List<JournalItem>();

            if (input.ItemIssueVendorCreditId.HasValue)
            {
                itemIssueVendorCreditItems = await _itemIssueVendorCreditItemRepository.GetAll()
                                                   .Where(t => t.ItemIssueVendorCreditId == input.ItemIssueVendorCreditId).ToListAsync();
                @inventoryJournalItems = await (_journalItemRepository.GetAll()
                                               .Where(s => s.Journal.ItemIssueVendorCreditId.HasValue)
                                               .Where(u => u.Journal.ItemIssueVendorCreditId == input.ItemIssueVendorCreditId &&
                                                u.Identifier != null)
                                           ).ToListAsync();
                itemIssueVendorCreditItemBatchNos = await _itemIssueVendorCreditItemBatchNoRepository.GetAll().AsNoTracking()
                                                          .Where(s => s.ItemIssueVendorCreditItem.ItemIssueVendorCreditId == input.ItemIssueVendorCreditId.Value)
                                                          .ToListAsync();
            }           

            foreach (var i in input.VendorCreditDetail)
            {
                // step 4. insert into vendor credit detail
                index++;
                if (i.FromLotId == null && i.ItemId != null)
                {
                    throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + index.ToString());
                }

                if (input.CurrencyId == input.MultiCurrencyId)
                {
                    i.MultiCurrencyTotal = i.Total;
                    i.MultiCurrencyUnitCost = i.UnitCost;
                }
                var @vendorCreditDetail = VendorCreditDetail.Create(tenantId, userId, vendorCredit, i.TaxId, i.ItemId, i.Description, i.Qty, i.UnitCost, i.DiscountRate, i.Total, i.MultiCurrencyUnitCost, i.MultiCurrencyTotal, i.ItemReceiptItemPurchaseId,i.PurchaseCost);
                // Assign bill item id to input of bill item 
                if (input.convertToItemIssueVendor == true)
                {
                    i.Id = vendorCreditDetail.Id;
                }

                // Update item Issue vendor credit item when recieve from item issue vendor credit

                if (input.ReceiveFrom == ReceiveFrom.ItemIssueVendorCredit && i.ItemIssueItemVendorCreditId != null)
                {
                    var journalItemIssueVendorCredit = @inventoryJournalItems.ToList().Where(u => u.Identifier == i.ItemIssueItemVendorCreditId);

                    if (journalItemIssueVendorCredit != null && journalItemIssueVendorCredit.Count() > 0)
                    {
                        foreach (var c in journalItemIssueVendorCredit)
                        {
                            if (c.Key == PostingKey.Inventory)
                            {
                                // update inventory posting key credit
                                c.SetDebitCredit(0, i.ItemIssueVendorCreditTotal);
                                CheckErrors(await _journalItemManager.UpdateAsync(c));
                            }
                            if (c.Key == PostingKey.COGS)
                            {
                                // update cogs posting key debit
                                c.SetDebitCredit(i.ItemIssueVendorCreditTotal, 0);
                                CheckErrors(await _journalItemManager.UpdateAsync(c));
                            }
                        }

                        
                        
                    }



                    var itemIssueVendorCreditItem = itemIssueVendorCreditItems.Where(t => t.Id == i.ItemIssueItemVendorCreditId).FirstOrDefault();
                    if (itemIssueVendorCreditItem != null)
                    {
                        itemIssueVendorCreditItem.UpdateVendorCreditItemId(vendorCreditDetail.Id);
                        itemIssueVendorCreditItem.UpdateQty(i.Qty);
                        itemIssueVendorCreditItem.UpdateItemIssueItem(i.ItemIssueVendorCreditUnitCost, 0, i.ItemIssueVendorCreditTotal);
                        CheckErrors(await _itemIssueVendorCreditItemManager.UpdateAsync(itemIssueVendorCreditItem));

                        var oldItemBatchs = itemIssueVendorCreditItemBatchNos.Where(s => s.ItemIssueVendorCreditItemId == itemIssueVendorCreditItem.Id).ToList();
                        if (i.ItemBatchNos != null && i.ItemBatchNos.Any())
                        {
                            var addItemBatchNos = i.ItemBatchNos.Where(s => !s.Id.HasValue || s.Id == Guid.Empty || !oldItemBatchs.Any(b => b.BatchNoId == s.BatchNoId))
                                                   .Select(s => ItemIssueVendorCreditItemBatchNo.Create(tenantId, userId, itemIssueVendorCreditItem.Id, s.BatchNoId, s.Qty)).ToList();
                            if (addItemBatchNos.Any())
                            {
                                foreach (var a in addItemBatchNos)
                                {
                                    await _itemIssueVendorCreditItemBatchNoRepository.InsertAsync(a);
                                }
                            }

                            var updateItemBatchNos = oldItemBatchs.Where(s => i.ItemBatchNos.Any(r => r.BatchNoId == s.BatchNoId)).Select(s =>
                            {
                                var oldItemBatch = s;
                                var newBatch = i.ItemBatchNos.FirstOrDefault(r => r.BatchNoId == s.BatchNoId);

                                oldItemBatch.Update(userId, itemIssueVendorCreditItem.Id, newBatch.BatchNoId, newBatch.Qty);
                                return oldItemBatch;
                            }).ToList();

                            if (updateItemBatchNos.Any()) await _itemIssueVendorCreditItemBatchNoRepository.BulkUpdateAsync(updateItemBatchNos);

                            var deleteItemBatchNos = oldItemBatchs.Where(s => !i.ItemBatchNos.Any(r => r.BatchNoId == s.BatchNoId)).ToList();
                            if (deleteItemBatchNos.Any())
                            {
                                await _itemIssueVendorCreditItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
                            }
                        }
                        else if (oldItemBatchs.Any())
                        {
                            await _itemIssueVendorCreditItemBatchNoRepository.BulkDeleteAsync(oldItemBatchs);
                        }
                    }

                }
                vendorCreditDetail.UpdateLot(i.FromLotId);
                CheckErrors(await _vendorCreditDetailManager.CreateAsync(@vendorCreditDetail));


                // step 5. insert into journal item with default credit value and debit auto zero
                var clearenceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, i.AccountId, i.Description, 0, i.Total, PostingKey.Clearance, vendorCreditDetail.Id);
                CheckErrors(await _journalItemManager.CreateAsync(clearenceJournalItem));

                if (i.ItemBatchNos != null && i.ItemBatchNos.Any())
                {
                    var addItemBatchNos = i.ItemBatchNos.Select(s => VendorCreditItemBatchNo.Create(tenantId, userId, @vendorCreditDetail.Id, s.BatchNoId, s.Qty)).ToList();
                    foreach (var a in addItemBatchNos)
                    {
                        await _vendorCreditItemBatchNoRepository.InsertAsync(a);
                    }
                }

            }

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.Bill };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }

            await CurrentUnitOfWork.SaveChangesAsync();


            if (input.convertToItemIssueVendor == true && input.Status == TransactionStatus.Publish)
            {

                var accounts = await _itemRepository.GetAll().Where(t => t.TenantId == tenantId && input.VendorCreditDetail.Any(v => v.ItemId == t.Id)).ToListAsync();

                var getItem = await _itemRepository.GetAll().Where(g => g.ItemTypeId != 3 && input.VendorCreditDetail.Any(i => i.ItemId == g.Id)).ToListAsync();

                var createInputItemReceipt = new CreateItemIssueVendorCreditInput()
                {
                    Memo = input.Memo,
                    IssueNo = input.VendorCreditNo,
                    IsConfirm = input.IsConfirm,
                    BillingAddress = input.BillingAddress,
                    SameAsShippingAddress = input.SameAsShippingAddress,
                    ShippingAddress = input.ShippingAddress,
                    ClassId = input.ClassId,
                    CurrencyId = input.CurrencyId,
                    Date = input.IssueDate.Value,
                    LocationId = input.LocationId,
                    ReceiveFrom = ReceiveFrom.VendorCredit,
                    Reference = input.Reference,
                    Status = input.Status,
                    // AccountId = ClearanceAccountId.Value,
                    Total = input.Total,
                    VendorId = input.VendorId,
                    VendorCreditId = vendorCredit.Id,
                    Items = input.VendorCreditDetail.Where(d => getItem.Any(v => v.Id == d.ItemId)).Select(t => new CreateOrUpdateItemIssueVendorCreditItemInput
                    {
                        FromLotId = t.FromLotId,
                        PurchaseAccountId = (from Account in accounts where (Account.Id == t.ItemId) select (Account.PurchaseAccountId.Value)).FirstOrDefault(),
                        InventoryAccountId = (from Account in accounts where (Account.Id == t.ItemId) select (Account.InventoryAccountId.Value)).FirstOrDefault(),
                        Description = t.Description,
                        DiscountRate = t.DiscountRate,
                        ItemId = t.ItemId.Value,
                        Qty = t.Qty,
                        Total = t.Total,
                        UnitCost = t.UnitCost,
                        VendorCreditItemId = t.Id,
                        InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.Account),
                        Item = ObjectMapper.Map<ItemSummaryDetailOutput>(t.Item),
                        ItemBatchNos = t.ItemBatchNos
                    }).ToList(),
                };
                await CreateItemIssueVendor(createInputItemReceipt);
            }

            return new NullableIdDto<Guid>() { Id = vendorCredit.Id };
        }

        public async Task CreateItemIssueVendor(CreateItemIssueVendorCreditInput input)
        {
            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }
            if(input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                                            .Where(t => (t.LockKey == TransactionLockType.ItemIssue || t.LockKey == TransactionLockType.InventoryTransaction)
                                            && t.IsLock == true && t.LockDate != null &&  t.LockDate != null && t.LockDate.Value.Date >= input.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

           

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue_VendorCredit);

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
            var @entity = Journal.Create(tenantId, userId, input.IssueNo, input.Date,
                input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, input.LocationId);
            entity.UpdateStatus(input.Status);
            #region journal transaction type 
            var transactionTypeId = await _journalTransactionTypeManager.GetJournalTransactionTypeId(InventoryTransactionType.ItemIssueVendorCredit);
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
                LotId = u.FromLotId,
                InventoryAccountId = u.InventoryAccountId
            }).ToList();
            var lotIds = input.Items.Select(x => x.FromLotId).ToList();
            var locationIds = await _lotRepository.GetAll().AsNoTracking()
                            .Where(t => lotIds.Contains(t.Id))
                            .Select(t => (long?)t.LocationId)
                            .ToListAsync();
            var getCostResult = await _inventoryManager.GetAvgCostForIssues(input.Date, locationIds, itemToCalculateCost/*, entity, roundingId*/);

            foreach (var r in getCostResult.Items)
            {
                input.Items[r.Index].UnitCost = r.UnitCost;
                input.Items[r.Index].Total = r.LineCost;
            }

            input.Total = getCostResult.Total;
            #endregion Calculat Cost



            //insert to item Receipt
            var itemIssueVendorCredit = ItemIssueVendorCredit.Create(
                                    tenantId, userId, input.VendorCreditId, input.ReceiveFrom, input.VendorId,
                                    input.SameAsShippingAddress,
                                    input.ShippingAddress, input.BillingAddress,
                                    input.Total, input.ItemReceiptPurchaseId);

            itemIssueVendorCredit.UpdateTransactionType(InventoryTransactionType.ItemIssueVendorCredit);
            @entity.UpdateItemIssueVendorCredit(itemIssueVendorCredit);

            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));
            CheckErrors(await _itemIssueVendorCreditManager.CreateAsync(itemIssueVendorCredit));

            foreach (var i in input.Items)
            {
                //insert to item Receipt item
                var itemIssueItem = ItemIssueVendorCreditItem.Create(tenantId, userId, itemIssueVendorCredit.Id, i.VendorCreditItemId, i.ItemId, i.Description, i.Qty, i.UnitCost, i.DiscountRate, i.Total, i.ItemReceiptPurhcaseItemId);
                itemIssueItem.UpdateLot(i.FromLotId);
                CheckErrors(await _itemIssueVendorCreditItemManager.CreateAsync(itemIssueItem));

                //insert clearance journal item into credit
                var cogsJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, i.PurchaseAccountId.Value, i.Description, i.Total, 0, PostingKey.COGS, itemIssueItem.Id);
                CheckErrors(await _journalItemManager.CreateAsync(cogsJournalItem));

                //insert inventory journal item into debit
                var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, i.InventoryAccountId, i.Description, 0, i.Total, PostingKey.Inventory, itemIssueItem.Id);
                CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));

                if (i.ItemBatchNos != null && i.ItemBatchNos.Any())
                {
                    var addItemBatchNos = i.ItemBatchNos.Select(s => ItemIssueVendorCreditItemBatchNo.Create(tenantId, userId, itemIssueItem.Id, s.BatchNoId, s.Qty)).ToList();
                    foreach (var a in addItemBatchNos)
                    {
                        await _itemIssueVendorCreditItemBatchNoRepository.InsertAsync(a);
                    }
                }
            }

            //update customer credit status 
            if (input.Status == TransactionStatus.Publish)
            {
                var vendorCredit = await _vendorCreditRepository.GetAll().Where(u => u.Id == input.VendorCreditId).FirstOrDefaultAsync();
                vendorCredit.UpdateShipedStatus(DeliveryStatus.ShipAll);
                CheckErrors(await _vendorCreditManager.UpdateAsync(vendorCredit));
            }           
            await CurrentUnitOfWork.SaveChangesAsync();

            await SyncItemIssueVendorCredit(itemIssueVendorCredit.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                var scheduleItems = input.Items.Select(s => s.ItemId).Distinct().ToList();
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, input.Date, scheduleItems);
            }

        }

        public async Task UpdateitemIssueVendor(UpdateItemIssueVendorCreditInput input)
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
                              .Where(u => u.JournalType == JournalType.ItemIssueVendorCredit && u.ItemIssueVendorCreditId == input.Id)
                              .FirstOrDefaultAsync();

            if (input.IsConfirm == false)
            {
                var validateLockDate = await _lockRepository.GetAll()
                                     .Where(t => t.IsLock == true &&
                                     (t.LockDate.Value.Date >= journal.Date.Date || t.LockDate.Value.Date >= input.Date.Date)
                                     && (t.LockKey == TransactionLockType.ItemIssue || t.LockKey == TransactionLockType.InventoryTransaction)).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue_VendorCredit);
            if (autoSequence.CustomFormat == true)
            {
                input.IssueNo = journal.JournalNo;
            }

            var scheduleDate = input.Date > journal.Date ? journal.Date : input.Date;

            journal.Update(tenantId, input.IssueNo, input.Date, input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, 0, input.LocationId);
            journal.UpdateStatus(input.Status);

            #region Calculat Cost
            //var roundingId = (await GetCurrentTenantAsync()).RoundDigitAccountId;

            var itemToCalculateCost = input.Items.Select((u, index) => new Inventories.Data.GetAvgCostForIssueData()
            {
                ItemName = u.Item.ItemName,
                Index = index,
                ItemId = u.ItemId,
                ItemCode = u.Item.ItemCode,
                Qty = u.Qty,
                LotId = u.FromLotId,
                ItemIssueItemId = input.Id,
                InventoryAccountId = u.InventoryAccountId
            }).ToList();
            var lotIds = input.Items.Select(x => x.FromLotId).ToList();
            var locationIds = await _lotRepository.GetAll().AsNoTracking()
                            .Where(t => lotIds.Contains(t.Id))
                            .Select(t => (long?)t.LocationId)
                            .ToListAsync();
            var getCostResult = await _inventoryManager.GetAvgCostForIssues(input.Date, locationIds, itemToCalculateCost/*, @journal, roundingId*/);

            foreach (var r in getCostResult.Items)
            {
                input.Items[r.Index].UnitCost = r.UnitCost;
                input.Items[r.Index].Total = r.LineCost;
            }

            input.Total = getCostResult.Total;
            #endregion Calculat Cost

            //update Clearance account 
            //var cogsAccount = await (_journalItemRepository.GetAll()
            //                          .Where(u => u.JournalId == journal.Id &&
            //                                   u.Key == PostingKey.COGS && u.Identifier == null)).FirstOrDefaultAsync();
            //cogsAccount.UpdateJournalItem(tenantId, input.AccountId, input.Memo,  input.Total,0);

            //update item receipt 
            var itemIssueVendorCredit = await _itemIssueVendorCreditManager.GetAsync(input.Id, true);

            if (itemIssueVendorCredit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            itemIssueVendorCredit.Update(tenantId, input.ReceiveFrom, input.VendorId,
                          input.SameAsShippingAddress, input.ShippingAddress,
                          input.BillingAddress, input.VendorCreditId, input.Total, input.ItemReceiptPurchaseId);


            CheckErrors(await _journalManager.UpdateAsync(@journal, autoSequence.DocumentType));

            // CheckErrors(await _journalItemManager.UpdateAsync(cogsAccount));
            CheckErrors(await _itemIssueVendorCreditManager.UpdateAsync(itemIssueVendorCredit));


            //Update Item Receipt customer credit Item and Journal Item
            var itemIssueItems = await _itemIssueVendorCreditItemRepository.GetAll().Where(u => u.ItemIssueVendorCreditId == input.Id).ToListAsync();

            var @inventoryJournalItems = await (_journalItemRepository.GetAll()
                                  .Where(u => u.JournalId == journal.Id && u.Identifier != null)).ToListAsync();

            var toDeleteItemReceiptItem = itemIssueItems
                .Where(u => !input.Items.Any(i => i.Id != null && i.Id == u.Id)).ToList();
            var toDeletejournalItem = inventoryJournalItems
                .Where(u => !input.Items.Any(i => u.Identifier != null && i.Id != null && i.Id == u.Identifier)).ToList();

            var itemBatchNos = await _itemIssueVendorCreditItemBatchNoRepository.GetAll().Where(s => itemIssueItems.Any(r => r.Id == s.ItemIssueVendorCreditItemId)).AsNoTracking().ToListAsync();
            if (toDeleteItemReceiptItem.Any())
            {
                var deleteItemBatchNos = itemBatchNos.Where(s => toDeleteItemReceiptItem.Any(r => r.Id == s.ItemIssueVendorCreditItemId)).ToList();
                if (deleteItemBatchNos.Any()) await _itemIssueVendorCreditItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
            }

            foreach (var c in input.Items)
            {
                if (c.Id != null) //update
                {
                    var itemReceipItem = itemIssueItems.FirstOrDefault(u => u.Id == c.Id);
                    var journalItem = @inventoryJournalItems.Where(s => s.Identifier == c.Id ).ToList();
                    if (itemReceipItem != null)
                    {
                        //new
                        itemReceipItem.Update(tenantId, c.ItemId, c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total);
                        itemReceipItem.UpdateLot(c.FromLotId);
                        CheckErrors(await _itemIssueVendorCreditItemManager.UpdateAsync(itemReceipItem));

                    }
                    if (journalItem != null & journalItem.Count() > 0)
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
                                i.UpdateJournalItem(userId, c.PurchaseAccountId.Value, c.Description, c.Total, 0);
                                CheckErrors(await _journalItemManager.UpdateAsync(i));
                            }
                        }
                    }

                    var oldItemBatchs = itemBatchNos.Where(s => s.ItemIssueVendorCreditItemId == c.Id).ToList();

                    if (c.ItemBatchNos != null && c.ItemBatchNos.Any())
                    {
                        var addItemBatchNos = c.ItemBatchNos
                            .Where(s => !s.Id.HasValue || s.Id == Guid.Empty || !oldItemBatchs.Any(r => r.BatchNoId == s.BatchNoId))
                            .Select(s => ItemIssueVendorCreditItemBatchNo.Create(tenantId, userId, itemReceipItem.Id, s.BatchNoId, s.Qty))
                            .ToList();
                        if (addItemBatchNos.Any())
                        {
                            foreach (var a in addItemBatchNos)
                            {
                                await _itemIssueVendorCreditItemBatchNoRepository.InsertAsync(a);
                            }
                        }

                        var updateItemBatchNos = oldItemBatchs.Where(s => c.ItemBatchNos.Any(r => r.BatchNoId == s.BatchNoId)).Select(s =>
                        {
                            var oldItemBatch = s;
                            var newBatch = c.ItemBatchNos.FirstOrDefault(r => r.BatchNoId == s.BatchNoId);

                            oldItemBatch.Update(userId, itemReceipItem.Id, newBatch.BatchNoId, newBatch.Qty);
                            return oldItemBatch;
                        }).ToList();

                        if (updateItemBatchNos.Any()) await _itemIssueVendorCreditItemBatchNoRepository.BulkUpdateAsync(updateItemBatchNos);

                        var deleteItemBatchNos = oldItemBatchs.Where(s => !c.ItemBatchNos.Any(r => r.BatchNoId == s.BatchNoId)).ToList();
                        if (deleteItemBatchNos.Any())
                        {
                            await _itemIssueVendorCreditItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
                        }
                    }
                    else if (oldItemBatchs.Any())
                    {
                        await _itemIssueVendorCreditItemBatchNoRepository.BulkDeleteAsync(oldItemBatchs);
                    }

                }
                else if (c.Id == null) //create
                {
                    //insert to item Receipt item
                    var itemIssueItem = ItemIssueVendorCreditItem.Create(tenantId, userId, itemIssueVendorCredit.Id, c.VendorCreditItemId, c.ItemId,
                                                                 c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total, c.ItemReceiptPurhcaseItemId);
                    itemIssueItem.UpdateLot(c.FromLotId);
                    CheckErrors(await _itemIssueVendorCreditItemManager.CreateAsync(itemIssueItem));

                    //insert COGS journal item into Debit
                    var cogsJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal, c.PurchaseAccountId.Value,
                        c.Description, c.Total, 0, PostingKey.COGS, itemIssueItem.Id);
                    CheckErrors(await _journalItemManager.CreateAsync(cogsJournalItem));

                    var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal,
                        c.InventoryAccountId, c.Description, 0, c.Total, PostingKey.Inventory, itemIssueItem.Id);
                    CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));

                    if (c.ItemBatchNos != null && c.ItemBatchNos.Any())
                    {
                        var addItemBatchNos = c.ItemBatchNos.Select(s => ItemIssueVendorCreditItemBatchNo.Create(tenantId, userId, itemIssueItem.Id, s.BatchNoId, s.Qty)).ToList();
                        foreach (var a in addItemBatchNos)
                        {
                            await _itemIssueVendorCreditItemBatchNoRepository.InsertAsync(a);
                        }
                    }
                }

            }

            //update customer credit status 
            if (input.Status == TransactionStatus.Publish)
            {
                var customerCredit = await _vendorCreditRepository
                                           .GetAll()
                                           .Where(u => u.Id == input.VendorCreditId)
                                           .FirstOrDefaultAsync();
                customerCredit.UpdateShipedStatus(DeliveryStatus.ShipAll);
                CheckErrors(await _vendorCreditManager.UpdateAsync(customerCredit));
            }

            var scheduleItems = input.Items.Select(s => s.ItemId).Concat(toDeleteItemReceiptItem.Select(s => s.ItemId)).Distinct().ToList();

            foreach (var t in toDeleteItemReceiptItem.OrderBy(u => u.Id))
            {
                CheckErrors(await _itemIssueVendorCreditItemManager.RemoveAsync(t));
            }

            foreach (var t in toDeletejournalItem)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(t));
            }          

            await CurrentUnitOfWork.SaveChangesAsync();

            await SyncItemIssueVendorCredit(itemIssueVendorCredit.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, scheduleDate, scheduleItems);
            }

        }


        private async Task DeteteItemIssueVendorCredit(CarlEntityDto input)
        {
            var journal = await _journalRepository.GetAll()
             .Include(u => u.ItemIssueVendorCredit)
             .Include(u => u.ItemIssueVendorCredit.ShippingAddress)
             .Include(u => u.ItemIssueVendorCredit.BillingAddress)
             .Where(u => u.JournalType == JournalType.ItemIssueVendorCredit && u.ItemIssueVendorCreditId == input.Id)
             .FirstOrDefaultAsync();


            if(input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                                       .Where(t => (t.LockKey == TransactionLockType.ItemIssue || t.LockKey == TransactionLockType.InventoryTransaction)
                                       && t.IsLock == true && t.LockDate != null && t.LockDate.Value.Date >= journal.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
           
            var @entity = journal.ItemIssueVendorCredit;


            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue_VendorCredit);

            if (journal.JournalNo == auto.LastAutoSequenceNumber)
            {
                var jo = await _journalRepository.GetAll().Where(t => t.Id != journal.Id && t.JournalType == JournalType.ItemIssueVendorCredit)
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
            
            //query get journal item and delete
            var @jounalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == journal.Id).ToListAsync();
            foreach (var ji in jounalItems)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(ji));
            }

            var scheduleDate = journal.Date;

            CheckErrors(await _journalManager.RemoveAsync(journal));

            var itemBatchNos = await _itemIssueVendorCreditItemBatchNoRepository.GetAll().Where(s => s.ItemIssueVendorCreditItem.ItemIssueVendorCreditId == input.Id).AsNoTracking().ToListAsync();
            if (itemBatchNos.Any())
            {
                await _itemIssueVendorCreditItemBatchNoRepository.BulkDeleteAsync(itemBatchNos);
            }

            //query get item receipt customer credit item and delete 
            var itemIssuseItems = await _itemIssueVendorCreditItemRepository.GetAll()
                .Where(u => u.ItemIssueVendorCreditId == entity.Id).ToListAsync();

            var scheduleItems = itemIssuseItems.Select(s => s.ItemId).Distinct().ToList();

            foreach (var iri in itemIssuseItems)
            {
                CheckErrors(await _itemIssueVendorCreditItemManager.RemoveAsync(iri));
            }

            CheckErrors(await _itemIssueVendorCreditManager.RemoveAsync(entity));

            await CurrentUnitOfWork.SaveChangesAsync();

            await DeleteInventoryTransactionItems(input.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, scheduleDate, scheduleItems);
            }

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_Delete)]
        public async Task Delete(CarlEntityDto input)
        {

            // validate from Paybill
            var validatefrompayBill = await _payBillDetailRepository.GetAll().AsNoTracking()
                                       .Where(t => t.VendorCreditId == input.Id).CountAsync();
            if (validatefrompayBill > 0)
            {
                throw new UserFriendlyException(L("RecordHasPayBill"));
            }

            var journal = await _journalRepository.GetAll()
                .Include(u => u.VendorCredit)
                .Include(u => u.VendorCredit.ShippingAddress)
                .Include(u => u.VendorCredit.BillingAddress)
                .Where(u => u.JournalType == JournalType.VendorCredit && u.VendorCreditId == input.Id)
                .FirstOrDefaultAsync();
            var @entity = journal.VendorCredit;

            if(input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                                  .Where(t => (t.LockKey == TransactionLockType.Bill)
                                  && t.IsLock == true && t.LockDate != null  && t.LockDate.Value.Date >= journal.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.VendorCredit);

            if (journal.JournalNo == auto.LastAutoSequenceNumber)
            {
                var jo = await _journalRepository.GetAll().Where(t => t.Id != journal.Id && t.JournalType == JournalType.VendorCredit)
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


            var validateCusotmerCredit = await _itemIssueVendorCredit.GetAll()
                                            .Include(t => t.VendorCredit)
                                            .Where(t => t.VendorCreditId == input.Id).FirstOrDefaultAsync();

          
            if (!entity.ConvertToItemIssueVendor &&
                validateCusotmerCredit != null &&
                validateCusotmerCredit.VendorCredit != null &&
                 validateCusotmerCredit.ReceiveFrom == ReceiveFrom.VendorCredit             
               )
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            else if (validateCusotmerCredit != null && entity.ConvertToItemIssueVendor == false)
            {

                validateCusotmerCredit.VendorCredit.UpdateShipedStatus(DeliveryStatus.ShipPending);
                validateCusotmerCredit.UpdateVendorCreditId(null);
                CheckErrors(await _vendorCreditManager.UpdateAsync(validateCusotmerCredit.VendorCredit));
                CheckErrors(await _itemIssueVendorCreditManager.UpdateAsync(validateCusotmerCredit));

            }
            else if (journal.Status == TransactionStatus.Publish && entity.ConvertToItemIssueVendor == true)
            {

                var inputItemIssueVenoder = new CarlEntityDto() {IsConfirm = input.IsConfirm , Id = validateCusotmerCredit.Id };
                await DeteteItemIssueVendorCredit(inputItemIssueVenoder);

                //var draftInput = new UpdateStatus();
                //draftInput.Id = input.Id;
               // await UpdateStatusToDraft(draftInput);
            }

            journal.UpdateVendorCredit(null);

            //query get journal item and delete
            var @jounalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == journal.Id).ToListAsync();
            foreach (var ji in jounalItems)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(ji));
            }
            CheckErrors(await _journalManager.RemoveAsync(journal));

            var itemBatchNos = await _vendorCreditItemBatchNoRepository.GetAll().Where(s => s.VendorCreditItem.VendorCreditId == input.Id).AsNoTracking().ToListAsync();
            if (itemBatchNos.Any())
            {
                await _vendorCreditItemBatchNoRepository.BulkDeleteAsync(itemBatchNos);
            }

            var exchangeRates = await _exchangeRateRepository.GetAll().AsNoTracking().Where(s => s.VendorCreditId == input.Id).ToListAsync();
            if (exchangeRates.Any()) await _exchangeRateRepository.BulkDeleteAsync(exchangeRates);

            //query get vendor credit detail and delete 
            var vendorCreditItems = await _vendorCreditDetailRepository.GetAll()
                .Where(u => u.VendorCreditId == entity.Id).ToListAsync();


            var itemIssueItem = await _itemIssueVendorCreditItemRepository.GetAll().Where(t => validateCusotmerCredit != null && (t.ItemIssueVendorCreditId == validateCusotmerCredit.Id)).ToListAsync();

            foreach (var i in vendorCreditItems)
            {
                var updateVendorCreditItem = itemIssueItem.Where(t => t.VendorCreditItemId == i.Id).FirstOrDefault();

                if (updateVendorCreditItem != null)
                {
                    updateVendorCreditItem.UpdateVendorCreditItemId(null);
                    CheckErrors(await _itemIssueVendorCreditItemManager.UpdateAsync(updateVendorCreditItem));
                }
                CheckErrors(await _vendorCreditDetailManager.RemoveAsync(i));
            }
            CheckErrors(await _vendorCreditManager.RemoveAsync(entity));

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.Bill };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_GetDetail)]
        public async Task<VendorCreditDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            await TurnOffTenantFilterIfDebug();

            var @journal = await _journalRepository
                                .GetAll()
                                .Include(u => u.Currency)
                                .Include(u => u.VendorCredit)
                                .Include(u => u.Class)
                                .Include(u => u.MultiCurrency)
                                .Include(u => u.Location)
                                .Include(u => u.VendorCredit.Vendor)
                                .Include(u => u.VendorCredit.ShippingAddress)
                                .Include(u => u.VendorCredit.BillingAddress)
                                .AsNoTracking()
                                .Where(u => u.JournalType == JournalType.VendorCredit && u.VendorCreditId == input.Id)
                                .FirstOrDefaultAsync();

            if (@journal == null || @journal.VendorCredit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var clearanceAccount = await (_journalItemRepository.GetAll()
                                       .Include(u => u.Account)
                                       .AsNoTracking()
                                       .Where(u => u.JournalId == journal.Id &&
                                                u.Key == PostingKey.AP && u.Identifier == null)
                                       .Select(u => u.Account)).FirstOrDefaultAsync();
         
            var result = ObjectMapper.Map<VendorCreditDetailOutput>(journal.VendorCredit);           
                var jr = await _journalRepository.GetAll()
                    .Include(u => u.ItemReceipt)
                    .Include(u => u.ItemIssueVendorCredit)
                    .Where(u => (u.ItemReceiptId == journal.VendorCredit.ItemReceiptId && journal.VendorCredit.ItemReceiptId != null)
                    || (u.ItemIssueVendorCredit.VendorCreditId == input.Id && u.ItemIssueVendorCreditId != null)).AsNoTracking().ToListAsync();


                foreach (var i in jr)
                {
                    if (i.ItemReceipt != null && i.ItemReceiptId != null)
                    {
                        result.ItemReceiptPurchaseId = i.ItemReceiptId;
                        result.ItemReceiptPurchaseNo = i.JournalNo;

                    }
                    else if (i.ItemIssueVendorCreditId != null)
                    {
                        result.ItemIssueVendorCreditId = i.ItemIssueVendorCreditId;
                        result.ItemIsseVendorCreditNo = i.JournalNo;
                        result.ItemIssueReference = i.Reference;
                        result.IssueDate = i.Date;
                    }
                }


            #region old code           
            var itemDetails = await (from vci in _vendorCreditDetailRepository.GetAll()
                                    .Include(u => u.Tax)
                                    .Include(u => u.Item)
                                    .Include(u => u.Tax)
                                    .Include(u => u.Lot)
                                    .Where(u => u.VendorCreditId == input.Id)
                                    .AsNoTracking()                                     
                                     join iivi in _itemIssueVendorCreditItemRepository.GetAll()
                                     .WhereIf(result.ItemIssueVendorCreditId!=null, t => t.ItemIssueVendorCreditId == result.ItemIssueVendorCreditId)
                                     .AsNoTracking()
                                     on vci.Id equals iivi.VendorCreditItemId
                                     into iivcis
                                     from iivci in iivcis.DefaultIfEmpty()
                                     join ji in _journalItemRepository.GetAll()
                                     .Include(u => u.Account)
                                     .AsNoTracking()
                                     on vci.Id equals ji.Identifier
                                     orderby vci.CreationTime
                                     select
                                     new VendorCreditDetailInput()
                                     {
                                         FromLotId = vci.LotId,
                                         LotDetail = ObjectMapper.Map<LotSummaryOutput>(vci.Lot),
                                         Id = vci.Id,
                                         Item = ObjectMapper.Map<ItemSummaryDetailOutput>(vci.Item),
                                         ItemId = vci.ItemId,
                                         AccountId = ji.AccountId,
                                         Account = ObjectMapper.Map<ChartAccountSummaryOutput>(ji.Account),
                                         Description = vci.Description,
                                         DiscountRate = vci.DiscountRate,
                                         Qty = vci.Qty,
                                         Tax = ObjectMapper.Map<TaxSummaryOutput>(vci.Tax),
                                         Total = vci.Total,
                                         UnitCost = vci.UnitCost,
                                         TaxId = vci.TaxId,
                                         MultiCurrencyTotal = vci.MultiCurrencyTotal,
                                         MultiCurrencyUnitCost = vci.MultiCurrencyUnitCost,
                                         ItemReceiptItemPurchaseId = vci.ItemReceiptItemId,
                                         ItemIssueItemVendorCreditId = iivci == null ? (Guid?)null : iivci.Id,
                                         CreationTime = vci.CreationTime,
                                         PurchaseCost = vci.PurchaseCost,
                                         UseBatchNo = vci.Item != null && vci.Item.UseBatchNo,
                                         TrackSerial = vci.Item != null && vci.Item.TrackSerial,
                                         TrackExpiration = vci.Item != null && vci.Item.TrackExpiration
                                     }).ToListAsync();


            #endregion


            var batchDic = await _vendorCreditItemBatchNoRepository.GetAll()
                             .AsNoTracking()
                             .Where(s => s.VendorCreditItem.VendorCreditId == input.Id)
                             .Select(s => new BatchNoItemOutput
                             {
                                 Id = s.Id,
                                 BatchNoId = s.BatchNoId,
                                 BatchNumber = s.BatchNo.Code,
                                 ExpirationDate = s.BatchNo.ExpirationDate,
                                 Qty = s.Qty,
                                 TransactionItemId = s.VendorCreditItemId
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
                                            .Where(s => s.VendorCreditId == input.Id)
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

            result.CreditNo = journal.JournalNo;
            result.CreditDate = journal.Date;
            result.DueDate = journal.VendorCredit.DueDate;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.Reference = journal.Reference;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.ItemDetail = itemDetails;
            result.Account = ObjectMapper.Map<ChartAccountSummaryOutput>(clearanceAccount);
            result.AccountId = clearanceAccount.Id;
            result.Memo = journal.Memo;
            result.Status = journal.Status;
            result.StatusName = journal.Status.ToString();
            result.MultiCurrency = ObjectMapper.Map<CurrencyDetailOutput>(journal.MultiCurrency);
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            result.LocationId = journal.LocationId.Value;
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_Update)]
        public async Task<NullableIdDto<Guid>> Update(UpdateVendorCreditInput input)
        {

            if (input.IsConfirm == false)
            {
                if (input.VendorCreditDetail.Count == 0)
                {
                    throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
                }
                var validateLockDate = await _lockRepository.GetAll()
                                      .Where(t => t.IsLock == true &&
                                      (t.LockDate.Value.Date >= input.DateCompare.Value.Date || t.LockDate.Value.Date >= input.CreditDate.Date)
                                      && t.LockKey == TransactionLockType.Bill).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            ValidateExchangeRate(input);
            await ValidateBatchNo(input);

            if (input.ReceiveFrom == ReceiveFrom.ItemReceiptPurchase)
            {
                var exceptIds = new List<Guid> { input.Id };
                if (input.ItemIssueVendorCreditId.HasValue) exceptIds.Add(input.ItemIssueVendorCreditId.Value);
                await CheckStockForPurchaseReturn(input.ItemReceiptPurchaseId.Value, input.VendorCreditDetail, exceptIds);

            }

            //validate vendor credit by paybill
            var validate = (from paybill in _payBillDetailRepository.GetAll()
                            .Where(t => t.VendorCreditId == input.Id)
                            select paybill).ToList().Count();
            if (validate > 0)
            {
                throw new UserFriendlyException(L("RecordDoAlready"));
            }

            await CalculateTotal(input);

            //validate vendor credit by item issue vendor credit 
            var validateCusotmerCredit = await (from itssueCredit in _itemIssueVendorCredit.GetAll().Include(u => u.VendorCredit)
                            .Where(t => t.VendorCreditId == input.Id)
                                                select itssueCredit).FirstOrDefaultAsync();

            if (validateCusotmerCredit != null && 
                validateCusotmerCredit.VendorCredit != null &&
                validateCusotmerCredit.ReceiveFrom == ReceiveFrom.VendorCredit &&
                !validateCusotmerCredit.VendorCredit.ConvertToItemIssueVendor)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            else if (validateCusotmerCredit != null && validateCusotmerCredit.VendorCredit.ReceiveFrom == ReceiveFrom.ItemIssueVendorCredit)
            {
                validateCusotmerCredit.VendorCredit.UpdateShipedStatus(DeliveryStatus.ShipPending);
                validateCusotmerCredit.UpdateVendorCreditId(null);
                CheckErrors(await _itemIssueVendorCreditManager.UpdateAsync(validateCusotmerCredit));
            }

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            // step 1. update Journal 
            var @journal = await _journalRepository
                              .GetAll()
                              .Where(u => u.JournalType == JournalType.VendorCredit && u.VendorCreditId == input.Id)
                              .FirstOrDefaultAsync();

            await CheckClosePeriod(journal.Date, input.CreditDate);

            journal.Update(tenantId, input.VendorCreditNo, input.CreditDate, input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, input.Status, input.LocationId);

            if (input.MultiCurrencyId == input.CurrencyId || input.MultiCurrencyId == null || input.MultiCurrencyId == 0)
            {
                input.MultiCurrencyId = input.CurrencyId;
                input.MultiCurrencyTotal = input.Total;
                input.MultiCurrencySubTotal = input.SubTotal;
                input.MultiCurrencyTax = input.Tax;

            }
            journal.UpdateMultiCurrency(input.MultiCurrencyId);
            //journal.UpdateStatus(input.Status);

            // step 2. update AP account 
            var @apAccount = await (_journalItemRepository.GetAll()
                                      .Where(u => u.JournalId == journal.Id && u.Key == PostingKey.AP && u.Identifier == null))
                                      .FirstOrDefaultAsync();
            @apAccount.UpdateJournalItem(tenantId, input.AccountId, input.Memo, input.Total, 0);

            // step 3. update vendor credit
            var @vendorCredit = await _vendorCreditManager.GetAsync(input.Id, true);
            var @oldVendorCredit = vendorCredit.ConvertToItemIssueVendor;

            if (@vendorCredit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var isItem = input.VendorCreditDetail.Any(s => s.ItemId != null && s.ItemId != Guid.Empty);

            @vendorCredit.Update(tenantId, input.VendorId, input.SameAsShippingAddress,
                input.ShippingAddress, input.BillingAddress, input.SubTotal, input.Tax,
                input.Total, input.DueDate, input.IssueDate, input.convertToItemIssueVendor,
                input.MultiCurrencySubTotal, input.MultiCurrencyTax, input.MultiCurrencyTotal,
                input.ItemReceiptPurchaseId, input.ReceiveFrom, isItem);
           
            //update itemIssue credit when create vendor credit from item Issue vendor credit
            if (input.ReceiveFrom == ReceiveFrom.ItemIssueVendorCredit && input.ItemIssueVendorCreditId != null)
            {

                if (input.ReceiveFrom == ReceiveFrom.ItemIssueVendorCredit && input.ItemIssueVendorCreditId != null)
                {

                    #region Calculat Cost
                    //var roundingId = (await GetCurrentTenantAsync()).RoundDigitAccountId;
                    var items = await _itemRepository.GetAll().Where(t => t.TenantId == tenantId && input.VendorCreditDetail.Any(v => v.ItemId == t.Id)).ToListAsync();

                    var itemToCalculateCost = input.VendorCreditDetail.Select((u, index1) => new Inventories.Data.GetAvgCostForIssueData()
                    {
                        ItemIssueItemId = input.ItemIssueVendorCreditId,
                        Index = index1,
                        ItemId = u.ItemId.Value,
                        ItemCode = u.Item.ItemCode,
                        Qty = u.Qty,
                        LotId = u.FromLotId,
                        InventoryAccountId = (from Account in items where (Account.Id == u.ItemId) select (Account.InventoryAccountId.Value)).FirstOrDefault(),
                    }).ToList();
                    var lotIds = input.VendorCreditDetail.Select(x => x.FromLotId).ToList();
                    var locationIds = await _lotRepository.GetAll().AsNoTracking()
                                    .Where(t => lotIds.Contains(t.Id))
                                    .Select(t => (long?)t.LocationId)
                                    .ToListAsync();
                    var getCostResult = await _inventoryManager.GetAvgCostForIssues(input.IssueDate.Value, locationIds, itemToCalculateCost/*, journal, roundingId*/);

                    foreach (var r in getCostResult.Items)
                    {

                        input.VendorCreditDetail[r.Index].ItemIssueVendorCreditUnitCost = r.UnitCost;
                        input.VendorCreditDetail[r.Index].ItemIssueVendorCreditTotal = r.LineCost;
                    }
                    var TotalItemIssue = getCostResult.Total;

                    #endregion Calculat Cost

                    var ItemIssueJournal = await _journalRepository.GetAll()
                                                .Include(u => u.ItemIssueVendorCredit)
                                                .Where(t => t.ItemIssueVendorCreditId == input.ItemIssueVendorCreditId).FirstOrDefaultAsync();

                    if (ItemIssueJournal != null && ItemIssueJournal.ItemIssueVendorCredit != null)
                    {
                        //update item issue
                        var validateLockDateIssue = await _lockRepository.GetAll()
                                  .Where(t => t.IsLock == true &&
                                  (t.LockDate.Value.Date >= ItemIssueJournal.Date.Date || t.LockDate.Value.Date >= input.IssueDate.Value.Date)
                                  && (t.LockKey == TransactionLockType.ItemIssue || t.LockKey == TransactionLockType.InventoryTransaction)).CountAsync();

                        var itemIssueItems = await _itemIssueVendorCreditItemRepository.GetAll()
                                        .Where(s => s.ItemIssueVendorCreditId == input.ItemIssueVendorCreditId)
                                        .AsNoTracking()
                                        .ToListAsync();

                        var isChangeItems = input.VendorCreditDetail.Count() != itemIssueItems.Count() || input.VendorCreditDetail.Any(s => itemIssueItems.Any(r => r.Id == s.ItemIssueItemVendorCreditId && (r.ItemId != s.ItemId || r.LotId != s.FromLotId || r.Qty != s.Qty)));


                        if (validateLockDateIssue > 0 && (ItemIssueJournal.Date.Date != input.IssueDate.Value.Date || isChangeItems))
                        {
                            throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                        }
                        ItemIssueJournal.UpdateJournalDate(input.IssueDate.Value);
                        ItemIssueJournal.ItemIssueVendorCredit.UpdateTotal(TotalItemIssue);
                        ItemIssueJournal.UpdateCreditDebit(TotalItemIssue, TotalItemIssue);                      
                        ItemIssueJournal.ItemIssueVendorCredit.UpdateVendorCreditId(vendorCredit.Id);

                        var setting = await _settingRepository.FirstOrDefaultAsync(s => s.SettingType == BillInvoiceSettingType.Bill);

                        if (setting == null || setting.ReferenceSameAsGoodsMovement) ItemIssueJournal.UpdateReference(input.Reference);
                        
                        var autoIss = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue_VendorCredit);
                        CheckErrors(await _journalManager.UpdateAsync(ItemIssueJournal, autoIss.DocumentType));
                        CheckErrors(await _itemIssueVendorCreditManager.UpdateAsync(ItemIssueJournal.ItemIssueVendorCredit));
                    }
                    vendorCredit.UpdateShipedStatus(DeliveryStatus.ShipAll);
                }           
            }

            //Check if credit list item detail has no item ID so it mean no receive status 
            var isAccountTap = checkIfItemExist(input.VendorCreditDetail, null);
            if (isAccountTap)
            {
                vendorCredit.UpdateShipedStatus(DeliveryStatus.NoReceive);
            }
            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.VendorCredit);
            CheckErrors(await _journalManager.UpdateAsync(@journal, autoSequence.DocumentType));

            CheckErrors(await _journalItemManager.UpdateAsync(@apAccount));
            CheckErrors(await _vendorCreditManager.UpdateAsync(@vendorCredit));

            // step 4. Update vendor credit detail item and Journal Item
            var vendorDetail = await _vendorCreditDetailRepository.GetAll().Where(u => u.VendorCreditId == input.Id).ToListAsync();

            var @clearanceJournalAcc = await (_journalItemRepository.GetAll().Where(u => u.JournalId == journal.Id &&
                                           u.Key == PostingKey.Clearance && u.Identifier != null)).ToListAsync();

            var toDeleteVendorCreditDetails = vendorDetail.Where(u => !input.VendorCreditDetail.Any(i => i.Id != null && i.Id == u.Id)).ToList();
            var toDeletejournalItem = @clearanceJournalAcc.Where(u => !input.VendorCreditDetail.Any(i => u.Identifier != null && i.Id != null && i.Id == u.Identifier)).ToList();

            int index = 0;

            
            var itemIssueVendorCreditItemBatchNos = new List<ItemIssueVendorCreditItemBatchNo>();
            var itemIssueVendorCreditItems = new List<ItemIssueVendorCreditItem>();
            var @inventoryJournalItems = new List<JournalItem>();
            if (input.ItemIssueVendorCreditId.HasValue)
            {
                itemIssueVendorCreditItems = await _itemIssueVendorCreditItemRepository.GetAll()
                                                   .Where(t => t.ItemIssueVendorCreditId == input.ItemIssueVendorCreditId).ToListAsync();

               @inventoryJournalItems = await _journalItemRepository.GetAll()
                                              .Where( s => s.Journal.ItemIssueVendorCreditId.HasValue)
                                              .Where(u => u.Journal.ItemIssueVendorCreditId == input.ItemIssueVendorCreditId && u.Identifier != null)
                                              .ToListAsync();

                itemIssueVendorCreditItemBatchNos = await _itemIssueVendorCreditItemBatchNoRepository.GetAll().AsNoTracking()
                                                         .Where(s => s.ItemIssueVendorCreditItem.ItemIssueVendorCreditId == input.ItemIssueVendorCreditId.Value)
                                                         .ToListAsync();
            }


            var OldItemIssueVendorCreditItems = new List<ItemIssueVendorCreditItem>();
            if(toDeleteVendorCreditDetails.Any())
            {
                OldItemIssueVendorCreditItems = await _itemIssueVendorCreditItemRepository.GetAll()
                                                      .Where(t => toDeleteVendorCreditDetails.Any(g => g.Id == t.VendorCreditItemId))
                                                      .ToListAsync();
            }

            var itemBatchNos = await _vendorCreditItemBatchNoRepository.GetAll().Where(s => vendorDetail.Any(r => r.Id == s.VendorCreditItemId)).AsNoTracking().ToListAsync();
            if (toDeleteVendorCreditDetails.Any())
            {
                var deleteItemBatchNos = itemBatchNos.Where(s => toDeleteVendorCreditDetails.Any(r => r.Id == s.VendorCreditItemId)).ToList();
                if (deleteItemBatchNos.Any()) await _vendorCreditItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
            }


            var exchange = await _exchangeRateRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.VendorCreditId == input.Id);
            if (input.UseExchangeRate && input.CurrencyId != input.MultiCurrencyId)
            {
                if (exchange == null)
                {
                    exchange = VendorCreditExchangeRate.Create(tenantId, userId, vendorCredit.Id, input.ExchangeRate.FromCurrencyId, input.ExchangeRate.ToCurrencyId, input.ExchangeRate.Bid, input.ExchangeRate.Ask);
                    await _exchangeRateRepository.InsertAsync(exchange);
                }
                else
                {
                    exchange.Update(userId, vendorCredit.Id, input.ExchangeRate.FromCurrencyId, input.ExchangeRate.ToCurrencyId, input.ExchangeRate.Bid, input.ExchangeRate.Ask);
                    await _exchangeRateRepository.UpdateAsync(exchange);
                }
            }
            else if (exchange != null)
            {
                await _exchangeRateRepository.DeleteAsync(exchange);
            }


            foreach (var c in input.VendorCreditDetail)
            {
                index++;
                if (c.FromLotId == null && c.ItemId != null)
                {
                    throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + index.ToString());
                }

                if (input.CurrencyId == input.MultiCurrencyId)
                {
                    c.MultiCurrencyTotal = c.Total;
                    c.MultiCurrencyUnitCost = c.UnitCost;
                }

                if (c.Id != null) // step 5. update item detail 
                {                  
                    var itemDetail = vendorDetail.FirstOrDefault(u => u.Id == c.Id);
                    var journalItem = @clearanceJournalAcc.FirstOrDefault(u => u.Identifier == c.Id);
                    if (itemDetail != null)
                    {
                        itemDetail.Update(tenantId, c.TaxId, c.ItemId, c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total, c.MultiCurrencyUnitCost, c.MultiCurrencyTotal, c.ItemReceiptItemPurchaseId,c.PurchaseCost);
                        itemDetail.UpdateLot(c.FromLotId);
                        CheckErrors(await _vendorCreditDetailManager.UpdateAsync(itemDetail));
                    }
                    if (journalItem != null)
                    {
                        //  update journal item with default credit value and debit auto zero
                        journalItem.UpdateJournalItem(userId, c.AccountId, c.Description, 0, c.Total);
                        CheckErrors(await _journalItemManager.UpdateAsync(journalItem));
                    }

                    var oldItemBatchs = itemBatchNos.Where(s => s.VendorCreditItemId == c.Id).ToList();

                    if (c.ItemBatchNos != null && c.ItemBatchNos.Any())
                    {
                        var addItemBatchNos = c.ItemBatchNos.Where(s => !s.Id.HasValue || s.Id == Guid.Empty).Select(s => VendorCreditItemBatchNo.Create(tenantId, userId, itemDetail.Id, s.BatchNoId, s.Qty)).ToList();
                        if (addItemBatchNos.Any())
                        {
                            foreach (var a in addItemBatchNos)
                            {
                                await _vendorCreditItemBatchNoRepository.InsertAsync(a);
                            }
                        }

                        var updateItemBatchNos = oldItemBatchs.Where(s => c.ItemBatchNos.Any(r => r.Id == s.Id)).Select(s =>
                        {
                            var oldItemBatch = s;
                            var newBatch = c.ItemBatchNos.FirstOrDefault(r => r.Id == s.Id);

                            oldItemBatch.Update(userId, itemDetail.Id, newBatch.BatchNoId, newBatch.Qty);
                            return oldItemBatch;
                        }).ToList();

                        if (updateItemBatchNos.Any()) await _vendorCreditItemBatchNoRepository.BulkUpdateAsync(updateItemBatchNos);

                        var deleteItemBatchNos = oldItemBatchs.Where(s => !c.ItemBatchNos.Any(r => r.Id == s.Id)).ToList();
                        if (deleteItemBatchNos.Any())
                        {
                            await _vendorCreditItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
                        }
                    }
                    else if (oldItemBatchs.Any())
                    {
                        await _vendorCreditItemBatchNoRepository.BulkDeleteAsync(oldItemBatchs);
                    }

                }
                else if (c.Id == null) // create new 
                {                    
                    // step 6. insert to vendor credit detail
                    var itemDetail = VendorCreditDetail.Create(tenantId, userId,
                        vendorCredit, c.TaxId, c.ItemId, c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total, c.MultiCurrencyUnitCost, c.MultiCurrencyTotal, c.ItemReceiptItemPurchaseId,c.PurchaseCost);
                    itemDetail.UpdateLot(c.FromLotId);
                    CheckErrors(await _vendorCreditDetailManager.CreateAsync(itemDetail));

                    

                    if(input.convertToItemIssueVendor == true)
                    {
                        c.Id = itemDetail.Id;
                    }
                    
                    //insert journal item into debit
                    var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal, c.AccountId, c.Description, 0, c.Total, PostingKey.Clearance, itemDetail.Id);
                    CheckErrors(await _journalItemManager.CreateAsync(clearanceJournalItem));

                    if (c.ItemBatchNos != null && c.ItemBatchNos.Any())
                    {
                        var addItemBatchNos = c.ItemBatchNos.Select(s => VendorCreditItemBatchNo.Create(tenantId, userId, itemDetail.Id, s.BatchNoId, s.Qty)).ToList();
                        foreach (var a in addItemBatchNos)
                        {
                            await _vendorCreditItemBatchNoRepository.InsertAsync(a);
                        }
                    }
                }

                // Update item Issue vendor credit item when recieve from item issue vendor credit
                if (input.ReceiveFrom == ReceiveFrom.ItemIssueVendorCredit && c.ItemIssueItemVendorCreditId != null)
                {
                    var journalItemIssueVendorCredit = @inventoryJournalItems.ToList().Where(u => u.Identifier == c.ItemIssueItemVendorCreditId);

                    if (journalItemIssueVendorCredit != null && journalItemIssueVendorCredit.Count() > 0)
                    {
                        foreach (var ic in journalItemIssueVendorCredit)
                        {
                            if (ic.Key == PostingKey.Inventory)
                            {
                                // update inventory posting key credit
                                ic.SetDebitCredit(0, c.ItemIssueVendorCreditTotal);
                                CheckErrors(await _journalItemManager.UpdateAsync(ic));
                            }
                            if (ic.Key == PostingKey.COGS)
                            {
                                // update cogs posting key debit
                                ic.SetDebitCredit(c.ItemIssueVendorCreditTotal, 0);
                                CheckErrors(await _journalItemManager.UpdateAsync(ic));
                            }
                        }

                    }

                    var itemIssueVendorCreditItem = itemIssueVendorCreditItems.Where(t => t.Id == c.ItemIssueItemVendorCreditId).FirstOrDefault();
                    if(itemIssueVendorCreditItem != null)
                    {
                        itemIssueVendorCreditItem.UpdateQty(c.Qty);
                        itemIssueVendorCreditItem.UpdateItemIssueItem(c.ItemIssueVendorCreditUnitCost, 0, c.ItemIssueVendorCreditTotal);
                        CheckErrors(await _itemIssueVendorCreditItemManager.UpdateAsync(itemIssueVendorCreditItem));

                        var oldItemBatchs = itemIssueVendorCreditItemBatchNos.Where(s => s.ItemIssueVendorCreditItemId == itemIssueVendorCreditItem.Id).ToList();
                        if (c.ItemBatchNos != null && c.ItemBatchNos.Any())
                        {
                            var addItemBatchNos = c.ItemBatchNos.Where(s => !s.Id.HasValue || s.Id == Guid.Empty || !oldItemBatchs.Any(b => b.BatchNoId == s.BatchNoId))
                                                   .Select(s => ItemIssueVendorCreditItemBatchNo.Create(tenantId, userId, itemIssueVendorCreditItem.Id, s.BatchNoId, s.Qty)).ToList();
                            if (addItemBatchNos.Any())
                            {
                                foreach (var a in addItemBatchNos)
                                {
                                    await _itemIssueVendorCreditItemBatchNoRepository.InsertAsync(a);
                                }
                            }

                            var updateItemBatchNos = oldItemBatchs.Where(s => c.ItemBatchNos.Any(r => r.BatchNoId == s.BatchNoId)).Select(s =>
                            {
                                var oldItemBatch = s;
                                var newBatch = c.ItemBatchNos.FirstOrDefault(r => r.BatchNoId == s.BatchNoId);

                                oldItemBatch.Update(userId, itemIssueVendorCreditItem.Id, newBatch.BatchNoId, newBatch.Qty);
                                return oldItemBatch;
                            }).ToList();

                            if (updateItemBatchNos.Any()) await _itemIssueVendorCreditItemBatchNoRepository.BulkUpdateAsync(updateItemBatchNos);

                            var deleteItemBatchNos = oldItemBatchs.Where(s => !c.ItemBatchNos.Any(r => r.BatchNoId == s.BatchNoId)).ToList();
                            if (deleteItemBatchNos.Any())
                            {
                                await _itemIssueVendorCreditItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
                            }
                        }
                        else if (oldItemBatchs.Any())
                        {
                            await _itemIssueVendorCreditItemBatchNoRepository.BulkDeleteAsync(oldItemBatchs);
                        }
                    }

                }

            }

            var itemIssueVendorcredit = OldItemIssueVendorCreditItems
                .Where(t => toDeleteVendorCreditDetails.Any(g => g.Id == t.VendorCreditItemId));
            if (input.convertToItemIssueVendor == true)
            {
                foreach (var iIV in itemIssueVendorcredit)
                {
                    iIV.UpdateVendorCreditItemId(null);
                }
            }
            foreach (var t in toDeleteVendorCreditDetails)
            {
                var ItemIsseueItem = OldItemIssueVendorCreditItems.Where(g => g.VendorCreditItemId == t.Id).FirstOrDefault();
                if (ItemIsseueItem != null)
                {
                    ItemIsseueItem.UpdateVendorCreditItemId(null);
                }
                CheckErrors(await _vendorCreditDetailManager.RemoveAsync(t));

            }
            foreach (var t in toDeletejournalItem)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(t));
            }

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.Bill };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            var accounts = await _itemRepository.GetAll().Where(t => t.TenantId == tenantId && input.VendorCreditDetail.Any(v => v.ItemId == t.Id)).ToListAsync();
            var tenant = await GetCurrentTenantAsync();
            
            //update item issue vendor credit 
            if (@oldVendorCredit == true && input.convertToItemIssueVendor == true && input.Status == TransactionStatus.Publish && input.ReceiveFrom != ReceiveFrom.ItemIssueVendorCredit)
            {
                var itemIssueVendorCreditId = await _itemIssueVendorCredit.GetAll()
                                   .Where(t => t.VendorCreditId == input.Id).Select(t => t.Id).FirstOrDefaultAsync();
                var getItem = await _itemRepository.GetAll().Where(g => g.ItemTypeId != 3 && input.VendorCreditDetail.Any(i => i.ItemId == g.Id)).ToListAsync();                
              
                var updateInputItemReceipt = new UpdateItemIssueVendorCreditInput()
                {
                    IsConfirm = input.IsConfirm,
                    Id = itemIssueVendorCreditId,
                    Memo = input.Memo,
                    IssueNo = input.VendorCreditNo,
                    BillingAddress = input.BillingAddress,
                    SameAsShippingAddress = input.SameAsShippingAddress,
                    ShippingAddress = input.ShippingAddress,
                    ClassId = input.ClassId,
                    CurrencyId = input.CurrencyId,
                    Date = input.IssueDate.Value,
                    LocationId = input.LocationId,
                    ReceiveFrom = ReceiveFrom.VendorCredit,
                    Reference = input.Reference,
                    Status = input.Status,
                    //AccountId = ClearanceAccountId.Value,
                    Total = input.Total,
                    VendorId = input.VendorId,
                    VendorCreditId = vendorCredit.Id,
                    Items = input.VendorCreditDetail.Where(d => getItem.Any(v => v.Id == d.ItemId))
                        .Select(t => new CreateOrUpdateItemIssueVendorCreditItemInput
                        {
                            FromLotId = t.FromLotId,
                            PurchaseAccountId = (from Account in accounts where (Account.Id == t.ItemId) select (Account.PurchaseAccountId.Value)).FirstOrDefault(),
                            InventoryAccountId = (from Account in accounts where (Account.Id == t.ItemId) select (Account.InventoryAccountId.Value)).FirstOrDefault(),
                            Description = t.Description,
                            DiscountRate = t.DiscountRate,
                            ItemId = t.ItemId.Value,
                            Qty = t.Qty,
                            Total = t.Total,
                            UnitCost = t.UnitCost,
                            VendorCreditItemId = t.Id,
                            InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.Account),
                            Item = ObjectMapper.Map<ItemSummaryDetailOutput>(t.Item),
                            Id = t.ItemIssueItemVendorCreditId, //t.Id == null || !itemIssueItems.ContainsKey(t.Id) ? t.Id : itemIssueItems[t.Id],
                            ItemBatchNos = t.ItemBatchNos
                        }).ToList(),
                };
                await UpdateitemIssueVendor(updateInputItemReceipt);

            }

            //delete item issue Vendor credit when old data bill before true but now user change false then delete item issue vendor credit
            else if  (@oldVendorCredit == true && input.convertToItemIssueVendor == false && input.Status == TransactionStatus.Publish)
            {
                vendorCredit.UpdateShipedStatus(DeliveryStatus.ShipPending);
                var VendorIssueId = new CarlEntityDto() {IsConfirm = input.IsConfirm,Id = validateCusotmerCredit.Id };
                await DeteteItemIssueVendorCredit(VendorIssueId);

            }


           //else if (validateCusotmerCredit.VendorCredit != null && validateCusotmerCredit.VendorCredit.ReceiveFrom == ReceiveFrom.ItemIssueVendorCredit)
           // {
           //     vendorCredit.UpdateShipedStatus(DeliveryStatus.ShipPending);
           //     var VendorIssueId = new EntityDto<Guid>() { Id = itemIssueVendorCreditId };
           //     await DeteteItemIssueVendorCredit(VendorIssueId);

           // }

            // create item Issue Vendor while vendorCredit data before auto convert is false then user change to true so automatically create item issue Vendor
           else if (@oldVendorCredit == false && input.convertToItemIssueVendor == true && input.Status == TransactionStatus.Publish && input.ReceiveFrom != ReceiveFrom.ItemIssueVendorCredit)
            {
                var getItem = await _itemRepository.GetAll().Where(g => g.ItemTypeId != 3 && input.VendorCreditDetail.Any(i => i.ItemId == g.Id)).ToListAsync();
                var createInputItemReceipt = new CreateItemIssueVendorCreditInput()
                {
                    Memo = input.Memo,
                    IssueNo = input.VendorCreditNo,
                    IsConfirm =  input.IsConfirm,
                    BillingAddress = input.BillingAddress,
                    SameAsShippingAddress = input.SameAsShippingAddress,
                    ShippingAddress = input.ShippingAddress,
                    ClassId = input.ClassId,
                    CurrencyId = input.CurrencyId,
                    Date = input.IssueDate.Value,
                    LocationId = input.LocationId,
                    ReceiveFrom = ReceiveFrom.VendorCredit,
                    Reference = input.Reference,
                    Status = input.Status,
                    //AccountId = ClearanceAccountId.Value,
                    Total = input.Total,
                    VendorId = input.VendorId,
                    VendorCreditId = vendorCredit.Id,
                    Items = input.VendorCreditDetail.Where(d => getItem.Any(v => v.Id == d.ItemId)).Select(t => new CreateOrUpdateItemIssueVendorCreditItemInput
                    {
                        FromLotId = t.FromLotId,
                        PurchaseAccountId = (from Account in accounts where (Account.Id == t.ItemId) select (Account.PurchaseAccountId.Value)).FirstOrDefault(),
                        InventoryAccountId = (from Account in accounts where (Account.Id == t.ItemId) select (Account.InventoryAccountId.Value)).FirstOrDefault(),
                        Description = t.Description,
                        DiscountRate = t.DiscountRate,
                        ItemId = t.ItemId.Value,
                        Qty = t.Qty,
                        Total = t.Total,
                        UnitCost = t.UnitCost,
                        VendorCreditItemId = t.Id,
                        InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.Account),
                        Item = ObjectMapper.Map<ItemSummaryDetailOutput>(t.Item),
                        ItemBatchNos = t.ItemBatchNos,
                    }).ToList(),
                };
                await CreateItemIssueVendor(createInputItemReceipt);

            }


            return new NullableIdDto<Guid>() { Id = vendorCredit.Id };

        }

        //[AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_UpdateStatusToDraft)]
        //public async Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input)
        //{
        //    //validate vendor credit by paybill
        //    var validate = (from paybill in _payBillDetailRepository.GetAll()
        //                    .Where(t => t.VendorCreditId == input.Id)
        //                    select paybill).ToList().Count();
        //    if (validate > 0)
        //    {
        //        throw new UserFriendlyException(L("RecordDoAlready"));
        //    }

          

        //    var @entity = await _journalRepository
        //                        .GetAll()
        //                        .Include(u => u.VendorCredit)
        //                        .Where(u => u.JournalType == JournalType.VendorCredit && u.VendorCreditId == input.Id)
        //                        .FirstOrDefaultAsync();
        //    if (entity == null)
        //    {
        //        throw new UserFriendlyException(L("RecordNotFound"));
        //    }

        //    var itemIssueVendorCreditId = _journalRepository.GetAll().Include(u => u.ItemIssueVendorCredit)
        //           .Where(t => t.ItemIssueVendorCredit.VendorCreditId == input.Id).Select(v => v.ItemIssueVendorCreditId).FirstOrDefault();

        //    if (entity.VendorCredit.Id != null && (entity.VendorCredit.ReceiveFrom != ReceiveFrom.ItemIssueVendorCredit && entity.VendorCredit.ConvertToItemIssueVendor == true) && itemIssueVendorCreditId != null)
        //    {

        //        var inputItemIssueVenoder = new CarlEntityDto() {IsConfirm = input.isc Id = itemIssueVendorCreditId.Value };
        //        await DeteteItemIssueVendorCredit(inputItemIssueVenoder);

        //    }
        //    entity.UpdateStatusToDraft();
        //    return new NullableIdDto<Guid>() { Id = entity.Id };
        //}

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_UpdateStatusToPublish)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input)
        {
            //validate vendor credit by paybill
            var validate = (from paybill in _payBillDetailRepository.GetAll()
                            .Where(t => t.VendorCreditId == input.Id)
                            select paybill).ToList().Count();
            if (validate > 0)
            {
                throw new UserFriendlyException(L("RecordDoAlready"));
            }

            //validate vendor credit by item issue vendor credit 
            var validateItemIssue = (from itssueCredit in _itemIssueVendorCredit.GetAll()
                            .Where(t => t.VendorCreditId == input.Id)
                                     select itssueCredit).ToList().Count();
            if (validateItemIssue > 0)
            {
                throw new UserFriendlyException(L("RecordDoAlready"));
            }

            var @entity = await _journalRepository
                                .GetAll()
                                .Include(u => u.VendorCredit)
                                .Where(u => u.JournalType == JournalType.VendorCredit && u.VendorCreditId == input.Id)
                                .FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }


            if (entity.VendorCredit.ConvertToItemIssueVendor == true)
            {
                var tenantId = AbpSession.GetTenantId();
                var userId = AbpSession.GetUserId();
                var ClearanceAccountId = await _tenantRepository.GetAll().Where(t => t.Id == tenantId).Select(t => t.TransitAccountId).FirstOrDefaultAsync();

                if (ClearanceAccountId == null)
                {
                    throw new UserFriendlyException(L("PleaseCreateTransitAccountOnCompanyProfile"));
                }
                var createInputItemReceipt = new CreateItemIssueVendorCreditInput()
                {
                    Memo = entity.Memo,
                    IssueNo = entity.JournalNo,

                    BillingAddress = entity.VendorCredit.BillingAddress,
                    SameAsShippingAddress = entity.VendorCredit.SameAsShippingAddress,
                    ShippingAddress = entity.VendorCredit.ShippingAddress,
                    ClassId = entity.ClassId,
                    CurrencyId = entity.CurrencyId,
                    Date = entity.VendorCredit.IssueDate.Value,
                    LocationId = entity.LocationId.Value,
                    ReceiveFrom = ReceiveFrom.None,
                    Reference = entity.Reference,
                    Status = TransactionStatus.Publish,
                    // AccountId = ClearanceAccountId.Value,
                    Total = entity.VendorCredit.Total,
                    VendorId = entity.VendorCredit.VendorId,
                    VendorCreditId = entity.VendorCredit.Id,
                    Items = _vendorCreditDetailRepository.GetAll()
                    .Include(t => t.Item).Where(d => d.VendorCreditId == input.Id && d.Item.ItemTypeId != 3).Select(t => new CreateOrUpdateItemIssueVendorCreditItemInput
                    {
                        PurchaseAccountId = t.Item.PurchaseAccountId.Value,
                        InventoryAccountId = t.Item.InventoryAccountId.Value,
                        Description = t.Description,
                        DiscountRate = t.DiscountRate,
                        ItemId = t.ItemId.Value,
                        Qty = t.Qty,
                        Total = t.Total,
                        UnitCost = t.UnitCost,
                        VendorCreditItemId = t.Id,
                        Item = ObjectMapper.Map<ItemSummaryDetailOutput>(t.Item)
                    }).ToList(),
                };
                await CreateItemIssueVendor(createInputItemReceipt);
            }

            entity.UpdatePublish();
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_UpdateStatusToVoid)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input)
        {
            //validate vendor credit by paybill
            var validate = (from paybill in _payBillDetailRepository.GetAll()
                            .Where(t => t.VendorCreditId == input.Id)
                            select paybill).ToList().Count();
            if (validate > 0)
            {
                throw new UserFriendlyException(L("RecordDoAlready"));
            }

            //validate vendor credit by item issue vendor credit 
            var validateItemIssue = (from itssueCredit in _itemIssueVendorCredit.GetAll().Include(u => u.VendorCredit)
                            .Where(t => t.VendorCreditId == input.Id && t.VendorCredit.ConvertToItemIssueVendor == false)
                                     select itssueCredit).ToList().Count();
            if (validateItemIssue > 0)
            {
                throw new UserFriendlyException(L("RecordDoAlready"));
            }

            var @jounal = await _journalRepository.GetAll()
               .Include(u => u.VendorCredit)
               .Include(u => u.VendorCredit.ShippingAddress)
               .Include(u => u.VendorCredit.BillingAddress)
               .Where(u => u.JournalType == JournalType.VendorCredit && u.VendorCreditId == input.Id)
               .FirstOrDefaultAsync();
            if (@jounal == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            if (jounal.VendorCredit.ConvertToItemIssueVendor == true)
            {
                var itemIssueVendorCreditId = _journalRepository.GetAll().Include(u => u.ItemIssueVendorCredit)
                 .Where(t => t.ItemIssueVendorCredit.VendorCreditId == input.Id).Select(v => v.ItemIssueVendorCreditId).FirstOrDefault();

                var @entity = await _journalRepository
                              .GetAll()
                              .Include(u => u.ItemIssueVendorCredit)
                              .Where(u => u.JournalType == JournalType.ItemIssueVendorCredit && u.ItemIssueVendorCredit.Id == itemIssueVendorCreditId)
                              .FirstOrDefaultAsync();

                if (entity.ItemIssueVendorCredit == null)
                {
                    throw new UserFriendlyException(L("RecordNotFound"));
                }

                entity.UpdateVoid();
            }
            jounal.UpdateVoid();
            return new NullableIdDto<Guid>() { Id = jounal.ItemReceiptId };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Credit_Find)]
        public async Task<PagedResultDto<GetListVendorCreditOutput>> Find(ListVendorCreditInput input)
        {

            var vendorTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            var accountCycle = await GetCurrentCycleAsync();
            // get user by group member
            var userGroups = await GetUserGroupByLocation();

            var jQuery = _journalRepository.GetAll()
                        .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                        .Where(u => u.JournalType == JournalType.VendorCredit && u.Status == TransactionStatus.Publish)
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
                            Reference = j.Reference,
                            Status = j.Status,
                            VendorCreditId = j.VendorCreditId,
                            Id = j.Id,
                            MultiCurrencyId = j.MultiCurrencyId.Value,
                        });

            var currencyQuery = GetCurrencies();
            var journalQuery = from j in jQuery
                               join mc in currencyQuery
                               on j.MultiCurrencyId equals mc.Id
                               select new
                               {
                                   JournalType = j.JournalType,
                                   Memo = j.Memo,
                                   Date = j.Date,
                                   JournalNo = j.JournalNo,
                                   Reference = j.Reference,
                                   Status = j.Status,
                                   VendorCreditId = j.VendorCreditId,
                                   Id = j.Id,
                                   MultiCurrencyId = j.MultiCurrencyId,
                                   MultiCurrencyCode = mc.Code,
                               };

            var journalItemQuery = _journalItemRepository.GetAll()
                                .Where(s => s.Identifier == null && s.Key == PostingKey.AP)
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
                              Reference = j.Reference,
                              Status = j.Status,
                              VendorCreditId = j.VendorCreditId,
                              Id = j.Id,
                              MultiCurrencyId = j.MultiCurrencyId,
                              MultiCurrencyCode = j.MultiCurrencyCode,
                              AccountId = ji.AccountId
                          };

            var cCreditQuery = _vendorCreditRepository.GetAll()
                                .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId))
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
                                    VendorId = s.VendorId,
                                });

            var vendorQuery = GetVendors(null, input.Vendors, null, vendorTypeMemberIds);
            var vendorCreditQeury = from cc in cCreditQuery
                                      join c in vendorQuery
                                      on cc.VendorId equals c.Id
                                      select new
                                      {
                                          Id = cc.Id,
                                          DueDate = cc.DueDate,
                                          OpenBalance = cc.OpenBalance,
                                          Total = cc.Total,
                                          PaidStatus = cc.PaidStatus,
                                          ShipedStatus = cc.ShipedStatus,
                                          MultiCurrencyOpenBalance = cc.MultiCurrencyOpenBalance,
                                          VendorId = cc.VendorId,
                                          VendorName = c.VendorName
                                      };

            var query = from cc in vendorCreditQeury
                        join ji in jiQuery
                        on cc.Id equals ji.VendorCreditId
                        select new GetListVendorCreditOutput
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
                            VendorId = cc.VendorId,
                            Reference = ji.Reference,
                            Vendor = new VendorSummaryOutput
                            {
                                Id = cc.VendorId,
                                VendorName = cc.VendorName
                            },
                            PaidStatus = cc.PaidStatus,
                            ReceivedStatus = cc.ShipedStatus,
                            MultiCurrencyCode = ji.MultiCurrencyCode,
                        };

            var resultCount = await query.CountAsync();
            if (resultCount == 0) return new PagedResultDto<GetListVendorCreditOutput>(resultCount, new List<GetListVendorCreditOutput>());

            var @entities = await query.OrderBy(s => s.Date).ToListAsync();
            return new PagedResultDto<GetListVendorCreditOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Credit_Find)]
        public async Task<PagedResultDto<GetListVendorCreditOutput>> FindOld(ListVendorCreditInput input)
        {

            var vendorTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            var accountCycle = await GetCurrentCycleAsync();
            // get user by group member
            var userGroups = await GetUserGroupByLocation();

            var vendorCredit = await _payBillDetailRepository.GetAll()
                            .Include(t=>t.PayBill)
                            .WhereIf(input.PayBillId != null && input.PayBillId.Count() > 0, p => input.PayBillId.Contains(p.PayBillId))
                            .AsNoTracking()
                            .FirstOrDefaultAsync();

            var vendorCreditId = vendorCredit == null ? null : vendorCredit.VendorCreditId;
            var paymentUSD = vendorCredit == null ? 0 : vendorCredit.PayBill.TotalPayment;
            var paymentMultiCurrency = vendorCredit == null ? 0 : vendorCredit.PayBill.MultiCurrencyTotalPayment;

            var query = (from jItem in _journalItemRepository.GetAll().AsNoTracking()
                         join j in _journalRepository.GetAll()
                         .WhereIf(input.Locations != null && input.Locations.Count > 0,
                                u => input.Locations.Contains(u.LocationId))
                        .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                        .Where(u => u.JournalType == JournalType.VendorCredit && u.Status == TransactionStatus.Publish)
                        .WhereIf(!input.Filter.IsNullOrEmpty(),
                                u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()))
                        .AsNoTracking()
                        on jItem.JournalId equals j.Id

                         join vcd in _vendorCreditDetailRepository.GetAll().AsNoTracking()
                         on jItem.Identifier equals vcd.Id into vcd
                         from vcdetail in vcd.DefaultIfEmpty()

                         join vc in _vendorCreditRepository.GetAll()
                         .WhereIf(vendorTypeMemberIds.Any(), s => vendorTypeMemberIds.Contains(s.Vendor.VendorTypeId.Value))
                         .AsNoTracking()
                         on j.VendorCreditId equals vc.Id

                         join v in _vendorRepository.GetAll()
                         .WhereIf(input.Vendors != null && input.Vendors.Count > 0,
                                u => input.Vendors.Contains(u.Id))
                         .AsNoTracking()
                         on vc.VendorId equals v.Id

                         join c in _currencyRepository.GetAll().AsNoTracking()
                         on j.CurrencyId equals c.Id

                         join mc in _currencyRepository.GetAll().AsNoTracking()
                         on j.MultiCurrencyId equals mc.Id

                         where (vc.OpenBalance > 0 || (vendorCredit != null && vc.Id == vendorCreditId))

                         group jItem by new
                         {
                             journalMemo = j.Memo,
                             Id = vc.Id,
                             journalDate = j.Date,
                             journalNo = j.JournalNo,
                             journalType = j.JournalType,
                             journalStatus = j.Status,
                             journalId = j.Id,
                             CurrencyId = j.CurrencyId,
                             CurrencyCode = c.Code,
                             CurrencyName = c.Name,
                             CurrencyPluralName = c.PluralName,
                             CurrencySymbol = c.Symbol,
                             vendorCreditDueDate = vc.DueDate,
                             vendorCreditOpenBalance = vc.OpenBalance,
                             vendorCreditTotal = vc.Total,
                             vendorCreditPaidStatus = vc.PaidStatus,
                             vendorCreditShipedStatus = vc.ShipedStatus,
                             VendorId = v.Id,
                             VendorName = v.VendorName,
                             VendorCode = v.VendorCode,
                             VendorAccountId = v.AccountId,
                             VendorCreditId = vc.Id,
                             vendorCreditMultiCurrencyOpenBalance = vc.MultiCurrencyOpenBalance,
                             //vendorCreditMultiPayment =  vendorCredit == null ? 0 : vendorCredit.MultiCurrencyTotalPayment,
                             //vendorCreditPayment = vendorCredit == null ? 0: vendorCredit.TotalPayment,
                             mCurrencyId = j.MultiCurrencyId,
                             mCurrencyCode = mc.Code,
                             mCurrencyName = mc.Name,
                             mCurrencyPluralName = mc.PluralName,
                             mCurrencySymbol = mc.Symbol,
                             //journal = j, vendorCredit = vc, vendor = v, currency = c
                         } into u

                         select new GetListVendorCreditOutput
                         {
                             MultiCurrencyOpenBalance = Math.Round(u.Key.vendorCreditMultiCurrencyOpenBalance + (u.Key.VendorCreditId == vendorCreditId ? paymentMultiCurrency : 0), accountCycle.RoundingDigit),
                             MultiCurrency = new CurrencyDetailOutput
                             {
                                 Code = u.Key.mCurrencyCode,
                                 Id = u.Key.mCurrencyId.Value,
                                 Name = u.Key.mCurrencyName,
                                 PluralName = u.Key.mCurrencyPluralName,
                                 Symbol = u.Key.mCurrencySymbol
                             },
                             
                             Currency = new CurrencyDetailOutput
                             {
                                 Code = u.Key.CurrencyCode,
                                 Id = u.Key.CurrencyId,
                                 Name = u.Key.CurrencyName,
                                 PluralName = u.Key.CurrencyPluralName,
                                 Symbol = u.Key.CurrencySymbol
                             },
                             CurrencyId = u.Key.CurrencyId,
                             Memo = u.Key.journalMemo,
                             Id = u.Key.Id,
                             TypeName = u.Key.journalType.ToString(),
                             TypeCode = u.Key.journalType,
                             Date = u.Key.journalDate,
                             AccountId = u.Where(t => t.Key == PostingKey.AP && t.Identifier == null && t.JournalId == u.Key.journalId)
                                      .Select(t => t.AccountId).FirstOrDefault(),
                             DueDate = u.Key.vendorCreditDueDate,
                             OpenBalance = Math.Round(u.Key.vendorCreditOpenBalance + (u.Key.VendorCreditId == vendorCreditId ? paymentUSD : 0), accountCycle.RoundingDigit),
                             JournalNo = u.Key.journalNo,
                             Status = u.Key.journalStatus,
                             Total = u.Key.vendorCreditTotal,
                             Vendor = new VendorSummaryOutput
                             {
                                 Id = u.Key.VendorId,
                                 AccountId = u.Key.VendorAccountId.Value,
                                 VendorCode = u.Key.VendorCode,
                                 VendorName = u.Key.VendorName
                             },
                             VendorId = u.Key.VendorId,
                             PaidStatus = u.Key.vendorCreditPaidStatus,
                             ReceivedStatus = u.Key.vendorCreditShipedStatus,
                             MultiCurrencyCode  = u.Key.mCurrencyCode
                         });

            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            return new PagedResultDto<GetListVendorCreditOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Bills_GetListForItemReceipt)]
        public async Task<PagedResultDto<VendorCreditSummaryOutput>> GetVendorCreditHeader(GetVendorCreditInput input)
        {
            var vendoTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            var @query = from j in _journalRepository.GetAll()
                                   .Include(u => u.VendorCredit)
                                   .Include(u => u.Class)
                                   .Include(u => u.Location)
                                   .Include(u => u.Currency)
                                   .Include(u => u.VendorCredit.Vendor)
                                   .Where(u => u.JournalType == JournalType.VendorCredit &&
                                    u.Status == TransactionStatus.Publish &&
                                    u.VendorCredit.ShipedStatus == DeliveryStatus.ShipPending &&
                                    u.VendorCredit.ReceiveFrom != ReceiveFrom.ItemIssueVendorCredit &&
                                    u.VendorCredit.ConvertToItemIssueVendor == false)                                  
                                   .Where(u => u.Status == TransactionStatus.Publish)
                                   .WhereIf(!input.Filter.IsNullOrEmpty(),
                                    p => p.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                    p.Reference.ToLower().Contains(input.Filter.ToLower()))
                                   .WhereIf(input.FromDate != null && input.ToDate != null,
                                    (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                                   .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId))
                                   .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorCredit.VendorId))
                                   .WhereIf(vendoTypeMemberIds.Any(), s => vendoTypeMemberIds.Contains(s.VendorCredit.Vendor.VendorTypeId.Value))
                                   .AsNoTracking()
                         join vi in _vendorCreditDetailRepository.GetAll()
                         .Where(u => u.ItemId != null)
                         .WhereIf(input.Lots != null && input.Lots.Count > 0, u => input.Lots.Contains(u.LotId))
                         .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                         on j.VendorCreditId equals vi.VendorCreditId 
                         group vi by new { journal = j , vendor = j.VendorCredit.Vendor ,vendorcredit = j.VendorCredit,currency = j.Currency} into s
                         orderby s.Key.journal.Date
                         where( s.Count() > 0 )
                         select new VendorCreditSummaryOutput
                         {
                             LocationName = s.Key.journal.Location != null ? s.Key.journal.Location.LocationName : null,
                             Vendor = ObjectMapper.Map<VendorSummaryOutput>(s.Key.vendor),
                             Memo = s.Key.journal.Memo,
                             Id = s.Key.vendorcredit.Id,
                             VendorNo = s.Key.journal.JournalNo,
                             Currency = ObjectMapper.Map<CurrencyDetailOutput>(s.Key.currency),
                             Date = s.Key.journal.Date,
                             Total = s.Key.vendorcredit.Total,
                             TotalItem = s.Count(),
                             CreationTimeIndex = s.Key.journal.CreationTimeIndex,
                             
                            
                         };
            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).ThenBy(t => t.CreationTimeIndex).PageBy(input).ToListAsync();
            return new PagedResultDto<VendorCreditSummaryOutput>(resultCount, @entities);
        }

        public async Task<VendorCreditDetailOutput> GetVendorCreditItems(GetVendorCreditInputForItem input)
        {
            var @journal = await _journalRepository
                                  .GetAll()
                                  .Include(u => u.Currency)
                                  .Include(u => u.VendorCredit)
                                  .Include(u => u.Class)
                                  .Include(u => u.Location)
                                  .Include(u => u.VendorCredit.Vendor)
                                  .Include(u => u.VendorCredit.Vendor.ChartOfAccount)
                                  .Include(u => u.VendorCredit.ShippingAddress)
                                  .Include(u => u.VendorCredit.BillingAddress)
                                  .AsNoTracking()
                                  .Where(u => u.JournalType == JournalType.VendorCredit && u.VendorCreditId == input.Id)
                                  .FirstOrDefaultAsync();

            if (@journal == null || @journal.VendorCredit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var APAccount = await (_journalItemRepository.GetAll()
                            .Include(u => u.Account)
                            .AsNoTracking()
                            .Where(u => u.JournalId == journal.Id && u.Key == PostingKey.AP && u.Identifier == null)
                            .Select(u => u.Account)).FirstOrDefaultAsync();
            var customerCreditItems = await _vendorCreditDetailRepository.GetAll()
                .Include(u => u.Item)
                .Include(u => u.Lot)
                .Include(u => u.Item.InventoryAccount)
                .Where(u => u.ItemId != null)
                .Where(u => u.Item.InventoryAccountId != null)
                .Where(u => u.VendorCreditId == input.Id && u.Item.InventoryAccountId != null)
                .Join(
                    _journalItemRepository.GetAll().Include(u => u.Account).AsNoTracking(),
                    u => u.Id,
                    s => s.Identifier,
                    (bItem, jItem) =>
                    new VendorCreditDetailInput()
                    {
                        FromLotId = bItem.LotId,
                        LotDetail = ObjectMapper.Map<LotSummaryOutput>(bItem.Lot),
                        CreationTime = bItem.CreationTime,
                        Id = bItem.Id,
                        Item = ObjectMapper.Map< ItemSummaryDetailOutput >(bItem.Item),                    
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
                        UseBatchNo = bItem.Item != null && bItem.Item.UseBatchNo,
                        TrackSerial = bItem.Item != null && bItem.Item.TrackSerial,
                        TrackExpiration = bItem.Item != null && bItem.Item.TrackExpiration
                    }).OrderBy(u => u.CreationTime)
            .ToListAsync();

            var batchDic = await _vendorCreditItemBatchNoRepository.GetAll()
                          .AsNoTracking()
                          .Where(s => s.VendorCreditItem.VendorCreditId == input.Id)
                          .Select(s => new BatchNoItemOutput
                          {
                              Id = s.Id,
                              BatchNoId = s.BatchNoId,
                              BatchNumber = s.BatchNo.Code,
                              ExpirationDate = s.BatchNo.ExpirationDate,
                              Qty = s.Qty,
                              TransactionItemId = s.VendorCreditItemId
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

            var result = ObjectMapper.Map<VendorCreditDetailOutput>(journal.VendorCredit);
            result.Total = customerCreditItems.Sum(t => t.Total);
            result.ClassId = journal.ClassId;
            result.CreditDate = journal.Date;
            result.CreditNo = journal.JournalNo;
            result.CurrencyId = journal.CurrencyId;
            result.Reference = journal.Reference;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.ItemDetail = customerCreditItems;
            result.Account = ObjectMapper.Map<ChartAccountSummaryOutput>(APAccount);
            result.AccountId = APAccount.Id;
            result.Memo = journal.Memo;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            result.LocationId = journal.LocationId.Value;
            return result;
        }

        #region  Get New List Vendor credit for Item Issue Vendor Credit New Design 

        //public async Task<PagedResultDto<GetListVendorCreditForItemIssueVendorCredit>> GetVendorCreditForItemIssueVendorCredit(GetVendorCreditFromIssueCreditInput input)
        //{

        //    var @query = from j in _journalRepository.GetAll()
        //                            .Include(u => u.VendorCredit)
        //                            .Include(u => u.Class)
        //                            .Include(u => u.Location)
        //                            .Include(u => u.Currency)
        //                            .Include(u => u.VendorCredit.Vendor)
        //                            .Where(u => u.JournalType == JournalType.VendorCredit &&
        //                             u.Status == TransactionStatus.Publish &&
        //                             u.VendorCredit.ShipedStatus == DeliveryStatus.ShipPending &&
        //                             u.VendorCredit.ReceiveFrom != ReceiveFrom.ItemIssueVendorCredit &&
        //                             u.VendorCredit.ConvertToItemIssueVendor == false)
        //                            .Where(u => u.Status == TransactionStatus.Publish)
        //                            .WhereIf(!input.Filter.IsNullOrEmpty(),
        //                             p => p.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
        //                             p.Reference.ToLower().Contains(input.Filter.ToLower()))
        //                            .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId))
        //                            .WhereIf(input.FromDate != null && input.ToDate != null,
        //                            (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
        //                            .AsNoTracking()
        //                 join vi in _vendorCreditDetailRepository.GetAll()
        //                 .Include(u=>u.Item)
        //                 .WhereIf(input.Lots != null && input.Lots.Count > 0, u => input.Lots.Contains(u.LotId))
        //                 .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
        //                 .Where(u => u.ItemId != null).AsNoTracking()
        //                 on j.VendorCreditId equals vi.VendorCreditId
                       
        //                 orderby j.Date                       
        //                 select new GetListVendorCreditForItemIssueVendorCredit
        //                 {                            
        //                     CreationTimeIndex = j.CreationTimeIndex,
        //                     Date = j.Date,
        //                     Id = vi.Id,
        //                     IssueCreditNo = j.JournalNo,
        //                     ItemCode = vi != null && vi.Item != null ? vi.Item.ItemCode : null,
        //                     ItemId = vi != null && vi.Item != null ? vi.Item.Id : (Guid?)null,
        //                     VendorCreditId = vi.VendorCreditId,
        //                     ItemName = vi != null && vi.Item != null ? vi.Item.ItemName : null,
        //                     LotId = vi.Lot.Id,
        //                     LotName = vi.Lot.LotName,
        //                     Qty = vi.Qty,

        //                 };
        //    var resultCount = await query.CountAsync();
        //    var @entities = await query.OrderBy(input.Sorting).ThenBy(t => t.CreationTimeIndex).PageBy(input).ToListAsync();
        //    return new PagedResultDto<GetListVendorCreditForItemIssueVendorCredit>(resultCount, @entities);
        //}

        #endregion


        #region import export excel

        private ReportOutput GetReportTemplateVendorCredit(bool hasClassFeature)
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
                        ColumnName = "VendorCode",
                        ColumnLength = 230,
                        ColumnTitle = "VendorCode",
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
                        ColumnName = "APAccount",
                        ColumnLength = 130,
                        ColumnTitle = "AP Account",
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
                        ColumnName = "VendorCreditNo",
                        ColumnLength = 250,
                        ColumnTitle = "Vendor Credit No",
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
                        SortOrder =10,
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
                HeaderTitle = "VendorCedit",
                Sortby = "",
            };
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Credit_ExportVendorCreditTamplate)]
        public async Task<FileDto> ExportExcelTamplate()
        {
            var result = new FileDto();
            var sheetName = "VendorCredit";
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
                var headerList = GetReportTemplateVendorCredit(hasClassFeature);
                var reportCollumnHeader = headerList.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
                foreach (var i in reportCollumnHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colHeaderTable += 1;
                }
                #endregion Row 1

                result.FileName = $"VendorCreditTamplate.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_Credit_Import)]
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
            var vendor = await _vendorRepository.GetAll().AsNoTracking().ToListAsync();
            //var tax = await _taxRepository.GetAll().Where(t => t.TaxName == "No Tax").Select(s => s.Id).ToListAsync();
            var journal = await _journalRepository.GetAll().Where(s => s.JournalType == JournalType.VendorCredit).ToListAsync();
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
                        var vendorCode = worksheet.Cells[i, 2].Value?.ToString();
                        var aPAccountCode = worksheet.Cells[i, 3].Value?.ToString();
                        var locationName = worksheet.Cells[i, 4].Value?.ToString();
                        var className = hasClassFeature ? worksheet.Cells[i, 5].Value?.ToString() : "";
                        var reference = worksheet.Cells[i, 6 + indexHasClassFeature].Value?.ToString();

                        var vendorNo = worksheet.Cells[i, 7 + indexHasClassFeature].Value?.ToString();
                        var currencyCode = worksheet.Cells[i, 8 + indexHasClassFeature].Value?.ToString();
                        var amount = worksheet.Cells[i, 9 + indexHasClassFeature].Value.ToString();
                        var amountInAccount = worksheet.Cells[i, 10 + indexHasClassFeature].Value.ToString();
                        var itemAccountCode = worksheet.Cells[i, 11 + indexHasClassFeature].Value?.ToString();
                        var itemDescription = worksheet.Cells[i, 12 + indexHasClassFeature].Value?.ToString();
                        var currencyId = currency.Where(s => s.Code == currencyCode).FirstOrDefault();
                        var classId = hasClassFeature ? @class.Where(s => s.ClassName == className).Select(t => t.Id).FirstOrDefault() : tenant.ClassId;
                        var vendorId = vendor.Where(s => s.VendorCode == vendorCode).FirstOrDefault();
                        var itemAccountId = accounts.Where(s => s.AccountCode == itemAccountCode).FirstOrDefault();
                        var apAccountId = accounts.Where(s => s.AccountCode == aPAccountCode).FirstOrDefault();
                        var locationId = locations.Where(s => s.LocationName == locationName).FirstOrDefault();
                        var taxId = accounts.Where(s => s.AccountCode == itemAccountCode).Select(t => t.TaxId).FirstOrDefault();

                        if (i > 1)
                        {
                            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Bill);

                            if (auto.CustomFormat == false && vendorNo == null)
                            {
                                throw new UserFriendlyException(L("VendorNoNoFound"));
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
                            if (vendorId == null)
                            {
                                throw new UserFriendlyException(L("NoVendorFound"));
                            }
                            if (itemAccountId == null)
                            {
                                throw new UserFriendlyException(L("NoItemAccountFound"));

                            }
                            if (apAccountId == null)
                            {
                                throw new UserFriendlyException(L("NoAPAccountFound"));
                            }
                            if (locationId == null)
                            {
                                throw new UserFriendlyException(L("NoLocationFound"));
                            }
                            CAddress billAddress = new CAddress("", "", "", "", "");
                            CAddress shipAddress = new CAddress("", "", "", "", "");
                            var createInput = new CreateVendorCreditInput()
                            {
                                VendorCreditNo = vendorNo,
                                BillingAddress = billAddress,
                                ShippingAddress = shipAddress,
                                VendorCreditDetail = new List<VendorCreditDetailInput>
                               {
                                  new VendorCreditDetailInput
                                  {
                                      Id = new Guid(),
                                      Description = itemDescription,
                                      DiscountRate = 0,
                                      AccountId = itemAccountId.Id,
                                      ItemId = null,
                                      FromLotId = null,
                                      MultiCurrencyTotal= Convert.ToDecimal(amount),
                                      MultiCurrencyUnitCost = Convert.ToDecimal(amount),
                                      Qty = 1,
                                      TaxId = taxId,
                                      Total = Convert.ToDecimal(amountInAccount),
                                      UnitCost = Convert.ToDecimal(amountInAccount),
                                  }

                               },
                                ClassId = classId,
                                AccountId = apAccountId.Id,
                                convertToItemIssueVendor = false,
                                CurrencyId = tenant.CurrencyId.Value,
                                CreditDate = Convert.ToDateTime(date),
                                DueDate = Convert.ToDateTime(date),
                                IssueDate = Convert.ToDateTime(date),
                                LocationId = locationId.Id,
                                Memo = "Opening Balance",
                                MultiCurrencyId = currencyId.Id,
                                MultiCurrencySubTotal = Convert.ToDecimal(amount),
                                MultiCurrencyTax = 0,
                                MultiCurrencyTotal = Convert.ToDecimal(amount),
                                VendorId = vendorId.Id,
                                Reference = reference,
                                SameAsShippingAddress = true,
                                Status = TransactionStatus.Publish,
                                SubTotal = Convert.ToDecimal(amountInAccount),
                                Tax = 0,
                                Total = Convert.ToDecimal(amountInAccount),
                                ReceiveFrom = ReceiveFrom.None,

                            };
                            if (journal.Where(t => t.JournalNo == createInput.VendorCreditNo).Count() == 0)
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

            var vendorCredit = await _vendorCreditManager.GetAsync(input.Id);
            if (vendorCredit == null) throw new UserFriendlyException(L("RecordNotFound"));

            var journal = await _journalRepository.FirstOrDefaultAsync(s => s.VendorCreditId == input.Id);
            if (journal == null) throw new UserFriendlyException(L("RecordNotFound"));

            var vendorCreditItems = await _vendorCreditDetailRepository.GetAll().Where(s => s.VendorCreditId == input.Id).ToListAsync();
            var journalItems = await _journalItemRepository.GetAll().Where(s => s.JournalId == journal.Id).ToListAsync();

            decimal subTotal = 0;
            foreach (var vItem in vendorCreditItems)
            {
                var lineTotal = Math.Round(vItem.Total, roundDigit);
                vItem.SetTotal(lineTotal);
                await _vendorCreditDetailManager.UpdateAsync(vItem);

                var journalItem = journalItems.FirstOrDefault(s => s.Identifier == vItem.Id);
                if (journalItem == null) throw new UserFriendlyException(L("RecordNotFound"));

                journalItem.SetDebitCredit(0, lineTotal);
                await _journalItemManager.UpdateAsync(journalItem);

                subTotal += lineTotal;
            }

            decimal tax = Math.Round(vendorCredit.Tax, roundDigit);
            decimal total = subTotal + tax;
            decimal openBalance = total - vendorCredit.TotalPaid;

            vendorCredit.SetSubTotal(subTotal);
            vendorCredit.SetTax(tax);
            vendorCredit.SetTotal(total);
            vendorCredit.SetOpenBalance(openBalance);
            vendorCredit.UpdatePaidStatus(vendorCredit.TotalPaid == 0 ? PaidStatuse.Pending : openBalance == 0 ? PaidStatuse.Paid : PaidStatuse.Partial);
            await _vendorCreditManager.UpdateAsync(vendorCredit);

            journal.SetDebitCredit(total);
            await _journalManager.UpdateAsync(journal, DocumentType.VendorCredit, false);

            var headerJournalItem = journalItems.FirstOrDefault(s => s.Identifier == null && s.Key == PostingKey.AP);
            if (headerJournalItem == null) throw new UserFriendlyException(L("RecordNotFound"));

            headerJournalItem.SetDebitCredit(total, 0);
            await _journalItemManager.UpdateAsync(headerJournalItem);

        }
    }
}
