using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Authorization;
using CorarlERP.AutoSequences;
using CorarlERP.ChartOfAccounts;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.CustomerCredits;
using CorarlERP.Customers;
using CorarlERP.ItemReceiptCustomerCredits.Dto;
using CorarlERP.Items;
using CorarlERP.Items.Dto;
using CorarlERP.Journals;
using CorarlERP.Journals.Dto;
using CorarlERP.Locations.Dto;
using CorarlERP.Lots.Dto;
using Microsoft.EntityFrameworkCore;
using static CorarlERP.enumStatus.EnumStatus;

using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore.Internal;
using CorarlERP.Customers.Dto;
using CorarlERP.Lots;
using CorarlERP.Inventories;
using CorarlERP.AccountCycles;
using CorarlERP.ItemIssues;
using CorarlERP.UserGroups;
using CorarlERP.Locks;
using CorarlERP.Common.Dto;
using CorarlERP.Invoices;
using CorarlERP.InventoryTransactionItems;
using CorarlERP.InventoryCalculationJobSchedules;
using Hangfire.States;
using CorarlERP.InventoryCalculationItems;
using CorarlERP.ItemIssueVendorCredits.Dto;
using CorarlERP.BatchNos;
using CorarlERP.ItemReceiptTransfers.Dto;
using Amazon.Runtime.Internal.Util;
using CorarlERP.ItemReceipts;
using CorarlERP.JournalTransactionTypes;
using CorarlERP.Features;

namespace CorarlERP.ItemReceiptCustomerCredits
{
    [AbpAuthorize]
    public class ItemReceiptCustomerCreditAppService : CorarlERPAppServiceBase, IItemReceiptCustomerCreditAppService
    {
        private readonly IItemManager _itemManager;
        private readonly IRepository<Item, Guid> _itemRepository;

        private readonly ICustomerCreditDetailManager _customerCreditItemManager;
        private readonly IRepository<CustomerCreditDetail, Guid> _customerCreditItemRepository;
        private readonly ICustomerCreditManager _customerCreditManager;
        private readonly IRepository<CustomerCredit, Guid> _customerCreditRepository;

        private readonly ICustomerManager _customerManager;
        private readonly IRepository<Customer, Guid> _customerRepository;
        private readonly IRepository<AccountCycle, long> _accountCycleRepository;
        private readonly IInventoryManager _inventoryManager;
        private readonly IItemReceiptItemCustomerCreditManager _itemReceiptCustomerCreditItemManager;
        private readonly IRepository<ItemReceiptItemCustomerCredit, Guid> _itemReceiptCustomerCreditItemRepository;
        private readonly IItemReceiptCustomerCreditManager _itemReceiptCustomerCreditManager;
        private readonly IRepository<ItemReceiptCustomerCredit, Guid> _itemReceiptCustomerCreditRepository;
        private readonly IRepository<Lot, long> _lotRepository;
        private readonly IJournalManager _journalManager;
        private readonly IRepository<Journal, Guid> _journalRepository;
        private readonly IJournalItemManager _journalItemManager;
        private readonly IRepository<JournalItem, Guid> _journalItemRepository;
        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly IChartOfAccountManager _chartOfAccountManager;
        private readonly IRepository<ItemIssueItem, Guid> _itemIssueItemRepository;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;
        private readonly IRepository<Lock, long> _lockRepository;

        private readonly IRepository<InvoiceItem, Guid> _invoiceItemRepository;
        private readonly ICorarlRepository<InventoryTransactionItem, Guid> _inventoryTransactionItemRepository;
        private readonly IInventoryCalculationItemManager _inventoryCalculationItemManager;
        private readonly ICorarlRepository<BatchNo, Guid> _batchNoRepository;
        private readonly ICorarlRepository<ItemReceiptCustomerCreditItemBatchNo, Guid> _itemReceiptCustomerCreditItemBatchNoRepository;
        private readonly ICorarlRepository<ItemIssueItemBatchNo, Guid> _itemIssueItemBatchNoRepository;
        private readonly ICorarlRepository<CustomerCreditItemBatchNo, Guid> _customerCreditItemBatchNoRepository;
        private readonly IJournalTransactionTypeManager _journalTransactionTypeManager;
        public ItemReceiptCustomerCreditAppService(
            ICorarlRepository<InventoryTransactionItem, Guid> inventoryTransactionItemRepository,
            IInventoryCalculationItemManager inventoryCalculationItemManager,
            IJournalManager journalManager,
            IJournalItemManager journalItemManager,
            IChartOfAccountManager chartOfAccountManager,
            ItemReceiptCustomerCreditManager itemReceiptCustomerCreditManager,
            ItemReceiptItemCustomerCreditManager itemReceiptItemCustomerCreditManager,
            ItemManager itemManager,
            ICustomerManager customerManager,
            CustomerCreditDetailManagers customerCreditItemManager,
            CustomerCreditManager customerCreditManager,
            IRepository<CustomerCreditDetail, Guid> customerCreditItemRepository,
            IRepository<CustomerCredit, Guid> customerCreditRepository,
            IRepository<Item, Guid> itemRepository,
            IRepository<Customer, Guid> customerRepository,
            IRepository<Journal, Guid> journalRepository,
            IRepository<JournalItem, Guid> journalItemRepository,
            IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
            IRepository<ItemReceiptCustomerCredit, Guid> itemReceiptCustomerCreditRepository,
            IRepository<ItemReceiptItemCustomerCredit, Guid> itemReceiptCustomerCreditItemRepository,
            IRepository<AutoSequence, Guid> autoSequenceRepository,
            IRepository<Lot, long> lotRepository,
            IAutoSequenceManager autoSequenceManager,
            IInventoryManager inventoryManager,
            IRepository<ItemIssueItem, Guid> itemIssueItemRepository,
            IRepository<AccountCycle, long> accountCycleRepository,
            IRepository<Locations.Location, long> locationRepository,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            IRepository<InvoiceItem, Guid> invoiceItemRepository,
            ICorarlRepository<BatchNo, Guid> batchNoRepository,
            ICorarlRepository<ItemReceiptCustomerCreditItemBatchNo, Guid> itemReceiptCustomerCreditItemBatchNoRepository,
            ICorarlRepository<ItemIssueItemBatchNo, Guid> itemIssueItemBatchNoRepository,
            ICorarlRepository<CustomerCreditItemBatchNo, Guid> customerCreditItemBatchNoRepository,
            IRepository<Lock, long> lockRepository,
            IJournalTransactionTypeManager journalTransactionTypeManager
        ) : base(accountCycleRepository, userGroupMemberRepository, locationRepository)
        {
            _itemIssueItemRepository = itemIssueItemRepository;
            _accountCycleRepository = accountCycleRepository;
            _inventoryManager = inventoryManager;
            _lotRepository = lotRepository;
            _journalManager = journalManager;
            _journalManager.SetJournalType(JournalType.ItemReceiptCustomerCredit);
            _journalRepository = journalRepository;
            _journalItemManager = journalItemManager;
            _journalItemRepository = journalItemRepository;
            _chartOfAccountManager = chartOfAccountManager;
            _chartOfAccountRepository = chartOfAccountRepository;
            _itemReceiptCustomerCreditItemManager = itemReceiptItemCustomerCreditManager;
            _itemReceiptCustomerCreditItemRepository = itemReceiptCustomerCreditItemRepository;
            _itemReceiptCustomerCreditManager = itemReceiptCustomerCreditManager;
            _itemReceiptCustomerCreditRepository = itemReceiptCustomerCreditRepository;

            _customerRepository = customerRepository;
            _customerManager = customerManager;
            _customerCreditItemRepository = customerCreditItemRepository;
            _customerCreditItemManager = customerCreditItemManager;
            _customerCreditRepository = customerCreditRepository;
            _customerCreditManager = customerCreditManager;
            _itemRepository = itemRepository;
            _itemManager = itemManager;
            _autoSequenceManager = autoSequenceManager;
            _autoSequenceRepository = autoSequenceRepository;
            _lockRepository = lockRepository;
            _invoiceItemRepository = invoiceItemRepository;
            _inventoryTransactionItemRepository = inventoryTransactionItemRepository;
            _inventoryCalculationItemManager = inventoryCalculationItemManager;
            _batchNoRepository = batchNoRepository;
            _itemReceiptCustomerCreditItemBatchNoRepository = itemReceiptCustomerCreditItemBatchNoRepository;
            _itemIssueItemBatchNoRepository = itemIssueItemBatchNoRepository;
            _customerCreditItemBatchNoRepository = customerCreditItemBatchNoRepository;
            _journalTransactionTypeManager = journalTransactionTypeManager;
        }

        private async Task ValidateBatchNo(CreateItemReceiptCustomerCreditInput input)
        {
            var validateItems = await _itemRepository.GetAll()
                         .Where(s => input.Items.Any(i => i.ItemId == s.Id))
                         .Where(s => s.UseBatchNo || s.TrackSerial || s.TrackExpiration)
                         .AsNoTracking()
                         .ToListAsync();

            if (!validateItems.Any()) return;

            var batchItemDic = validateItems.ToDictionary(k => k.Id, v => v);

            var itemUseBatchs = input.Items.Where(s => batchItemDic.ContainsKey(s.ItemId)).ToList();

            var find = itemUseBatchs.Where(s => s.ItemBatchNos == null || s.ItemBatchNos.Count == 0 || s.ItemBatchNos.Any(r => r.BatchNoId == Guid.Empty)).FirstOrDefault();
            if (find != null) throw new UserFriendlyException(L("PleaseSelect", $"{L("BatchNo")}/{L("SerialNo")}") + $", Item: {batchItemDic[find.ItemId].ItemCode}-{batchItemDic[find.ItemId].ItemName}");

            var serialItemHash = validateItems.Where(s => s.TrackSerial).Select(s => s.Id).ToHashSet();
            var serialQty = input.Items.Where(s => serialItemHash.Contains(s.ItemId)).FirstOrDefault(s => s.ItemBatchNos.Any(b => b.Qty != 1));
            if (serialQty != null) throw new UserFriendlyException(L("MustBeEqualTo", $"{L("SerialNo")} {L("Qty")}", 1) + $", Item: {batchItemDic[serialQty.ItemId].ItemCode}-{batchItemDic[serialQty.ItemId].ItemName}");

            var batchNoIds = itemUseBatchs.SelectMany(s => s.ItemBatchNos).GroupBy(s => s.BatchNoId).Select(s => s.Key).ToList();
            var batchNoDic = await _batchNoRepository.GetAll().Where(s => batchNoIds.Contains(s.Id)).ToDictionaryAsync(k => k.Id, v => v);
            var notFoundBatch = batchNoIds.FirstOrDefault(s => !batchNoDic.ContainsKey(s));
            if (notFoundBatch != null && notFoundBatch != Guid.Empty) throw new UserFriendlyException(L("RecordNotFound"));

            //duplicate on transaction item use same batch more that one time
            var duplicate = itemUseBatchs.FirstOrDefault(s => s.ItemBatchNos.GroupBy(g => g.BatchNoId).Any(r => r.Count() > 1));
            if (duplicate != null) throw new UserFriendlyException(L("Duplicated", $"{L("BatchNo")}/{L("SerialNo")}" + $" , Item: {batchItemDic[duplicate.ItemId].ItemCode}-{batchItemDic[duplicate.ItemId].ItemName}"));

            var zeroQty = itemUseBatchs.FirstOrDefault(s => s.ItemBatchNos.Any(r => r.Qty <= 0));
            if (zeroQty != null) throw new UserFriendlyException(L("MustBeGreaterThen", L("Qty"), 0));

            var validateQty = itemUseBatchs.FirstOrDefault(s => s.Qty != s.ItemBatchNos.Sum(t => t.Qty));
            if (validateQty != null) throw new UserFriendlyException(L("IsNotValid", L("Qty") + " " + $"{L("BatchNo")}/{L("SerialNo")}" + $", {batchItemDic[validateQty.ItemId].ItemCode}-{batchItemDic[validateQty.ItemId].ItemName}"));

        }


        private async Task CheckStockForSaleReturn(Guid itemIssueSaleId, List<CreateOrUpdateItemReceiptCustomerCreditItemInput> inputItems, List<Guid> exceptIds)
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
                                 BatchNoId = b == null ? (Guid?) null : b.BatchNoId,
                                 iri.ItemId
                             };

            var cQuery = from r in _customerCreditItemRepository.GetAll()
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
                             BatchNoId = b == null ? (Guid?) null : b.BatchNoId
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
                             BatchNoId = b == null ? (Guid?) null : b.BatchNoId
                         };



            var query = from iri in issueQuery
                        join returnItem in cQuery
                        on $"{iri.Id}-{iri.BatchNoId}" equals $"{returnItem.ItemIssueSaleItemId}-{returnItem.BatchNoId}" 
                        into returnItems

                        join returnItem2 in rQuery
                        on $"{iri.Id}-{iri.BatchNoId}"  equals $"{returnItem2.ItemIssueSaleItemId}-{returnItem2.BatchNoId}" 
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

            foreach( var r in inputItems)
            {
                if(r.ItemBatchNos != null && r.ItemBatchNos.Any())
                {
                    foreach(var b in r.ItemBatchNos)
                    {
                        var checkStock = balanceList.FirstOrDefault(i => i.Id == r.ItemIssueSaleItemId && i.ItemId == r.ItemId && i.BatchNoId == b.BatchNoId && i.Qty < b.Qty);
                        if (checkStock != null) throw new UserFriendlyException(L("ReturnQtyCannotGreaterThanSaleQty"));
                    }
                }
                else
                {
                    var checkStock = balanceList.FirstOrDefault(i => i.Id == r.ItemIssueSaleItemId && i.ItemId == r.ItemId && i.Qty < r.Qty) ;
                    if (checkStock != null) throw new UserFriendlyException(L("ReturnQtyCannotGreaterThanSaleQty"));
                }
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CreateCustomerCredits,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptCustomerCredit)]
        public async Task<NullableIdDto<Guid>> Create(CreateItemReceiptCustomerCreditInput input)
        {

            if (input.IsConfirm == false)
            { 
            var validateLockDate = await _lockRepository.GetAll()
                                 .Where(t => t.IsLock == true &&
                                 (t.LockDate.Value.Date >= input.Date.Date)
                                 && (t.LockKey == TransactionLockType.ItemReceipt || t.LockKey == TransactionLockType.InventoryTransaction)).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            await ValidateBatchNo(input);

            if (input.ReceiveFrom == ReceiveFrom.ItemIssueSale)
            {
                await CheckStockForSaleReturn(input.ItemIssueSaleId.Value, input.Items, null);
            }

            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
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
            var @entity = Journal.Create(tenantId, userId, input.ReceiptNo, input.Date,
                input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, input.LocationId);
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



            //insert to item Receipt
            var itemReceiptCustomerCredit = ItemReceiptCustomerCredit.Create(
                                    tenantId, userId, input.CustomerCreditId, input.ReceiveFrom, input.CustomerId,
                                    input.SameAsShippingAddress,
                                    input.ShippingAddress, input.BillingAddress,
                                    input.Total, input.ItemIssueSaleId);
            itemReceiptCustomerCredit.UpdateTransactionType(InventoryTransactionType.ItemReceiptCustomerCredit);
            @entity.UpdateItemReceiptCustomerCredit(itemReceiptCustomerCredit);

            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));
            CheckErrors(await _itemReceiptCustomerCreditManager.CreateAsync(itemReceiptCustomerCredit));

            int index = 0;
            foreach (var i in input.Items)
            {

                //validation Qty form Issue sale
                index++;
                //insert to item Receipt item
                if (i.LotId == null && i.ItemId != null)
                {
                    throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + index.ToString());
                }
                var @itemReceiptItem = ItemReceiptItemCustomerCredit.Create(tenantId, userId, itemReceiptCustomerCredit.Id, i.CustomerCreditItemId, i.ItemId, i.Description, i.Qty, i.UnitCost, i.DiscountRate, i.Total, i.ItemIssueSaleItemId);
                itemReceiptItem.UpdateLot(i.LotId);
                CheckErrors(await _itemReceiptCustomerCreditItemManager.CreateAsync(@itemReceiptItem));

                //insert clearance journal item into credit
                var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, i.ClearanceAccountId, input.Memo, 0, i.Total, PostingKey.COGS, itemReceiptItem.Id);
                CheckErrors(await _journalItemManager.CreateAsync(clearanceJournalItem));


                //insert inventory journal item into debit
                var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, i.InventoryAccountId, i.Description, i.Total, 0, PostingKey.Inventory, itemReceiptItem.Id);
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
            if (input.Status == TransactionStatus.Publish && input.ReceiveFrom == ReceiveFrom.CustomerCredit && input.CustomerCreditId != null)
            {
                var customerCredit = await _customerCreditRepository
                                           .GetAll()
                                           .Where(u => u.Id == input.CustomerCreditId)
                                           .FirstOrDefaultAsync();
                customerCredit.UpdateShipedStatus(DeliveryStatus.ShipAll);
                CheckErrors(await _customerCreditManager.UpdateAsync(customerCredit));
            }

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.ItemReceipt, TransactionLockType.InventoryTransaction, };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }

            await CurrentUnitOfWork.SaveChangesAsync();


            var orderIds = input.Items.Where(s => s.OrderId.HasValue).Select(s => s.OrderId.Value).Distinct();
            var deliveryIds = input.Items.Where(s => s.DeliveryId.HasValue).Select(s => s.DeliveryId.Value).Distinct();
            if (orderIds.Any() || deliveryIds.Any())
            {
                foreach (var id in orderIds)
                {
                    await UpdateOrderInventoryStatus(id);
                }
                foreach (var id in deliveryIds)
                {
                    await UpdateDeliveryInventoryStatus(id,true);
                }
            }


            await SyncItemReceiptCustomerCredit(itemReceiptCustomerCredit.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                var scheduleItems = input.Items.Select(s => s.ItemId).Distinct().ToList();
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, input.Date, scheduleItems);
            }

            return new NullableIdDto<Guid>() { Id = itemReceiptCustomerCredit.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_Delete,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Delete,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour)]
        public async Task Delete(CarlEntityDto input)
        {

            var @jounal = await _journalRepository.GetAll()
                .Include(u => u.ItemReceiptCustomerCredit.CustomerCredit)
                .Include(u => u.ItemReceiptCustomerCredit.ShippingAddress)
                .Include(u => u.ItemReceiptCustomerCredit.BillingAddress)
                .Where(u => u.JournalType == JournalType.ItemReceiptCustomerCredit && u.ItemReceiptCustomerCreditId == input.Id)
                .FirstOrDefaultAsync();

            if (jounal == null) throw new UserFriendlyException(L("RecordNotFound"));

            var @entity = @jounal.ItemReceiptCustomerCredit;

            if (input.IsConfirm == false)
            {

                var validateLockDate = await _lockRepository.GetAll()
                                     .Where(t => t.IsLock == true &&
                                     (t.LockDate.Value.Date >= jounal.Date.Date)
                                     && (t.LockKey == TransactionLockType.ItemReceipt || t.LockKey == TransactionLockType.InventoryTransaction)).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            //validate EditBy 48 hours
            var userId = AbpSession.GetUserId();
            var getPermission = await GetUserPermissions(userId);
            var IsEdit = getPermission.Where(t => t == AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour).Count();
            var totalHours = (DateTime.Now - @entity.CreationTime).TotalHours;
            if (IsEdit > 0 && totalHours > 48)
            {
                throw new UserFriendlyException(L("ThisTransactionIsOver48HoursCanNotDelete"));
            }

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            else if (jounal.ItemReceiptCustomerCredit.CustomerCredit != null &&
                jounal.ItemReceiptCustomerCredit.CustomerCredit.ConvertToItemReceipt == true)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            else if (@jounal.ItemReceiptCustomerCredit != null &&
                @jounal.ItemReceiptCustomerCredit.CustomerCredit != null &&
                @jounal.ItemReceiptCustomerCredit.ReceiveFrom != ReceiveFrom.CustomerCredit
               )
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            else if (@jounal.ItemReceiptCustomerCredit != null && @jounal.ItemReceiptCustomerCredit.CustomerCredit != null)
            {
                @jounal.ItemReceiptCustomerCredit.CustomerCredit.UpdateShipedStatus(DeliveryStatus.ShipPending);
                CheckErrors(await _customerCreditManager.UpdateAsync(@jounal.ItemReceiptCustomerCredit.CustomerCredit));
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

            @jounal.UpdateItemReceiptCustomerCredit(null);

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


            var oldOrderIds = new List<Guid>();
            var oldDeliveryIds = new List<Guid>();


            var itemIssueItemIds = @itemReceiptItems.Where(s => s.ItemIssueSaleItemId.HasValue).Select(s => s.ItemIssueSaleItemId.Value).ToList();
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
                var oldCreditItemIds = @itemReceiptItems.Where(s => s.CustomerCreditItemId.HasValue).Select(s => s.CustomerCreditItemId.Value).ToList();
                var itemIssueFromCreditIds = await _customerCreditItemRepository.GetAll()
                                                       .Where(s => oldCreditItemIds.Contains(s.Id))
                                                       .Where(s => s.ItemIssueSaleItemId.HasValue)
                                                       .AsNoTracking()
                                                       .Select(s => s.ItemIssueSaleItemId.Value)
                                                       .ToListAsync();

                var orderFromIssue = await _itemIssueItemRepository.GetAll()
                                           .Where(s => s.SaleOrderItemId.HasValue)
                                           .Where(s => itemIssueFromCreditIds.Contains(s.Id))
                                           .AsNoTracking()
                                           .Select(s => s.SaleOrderItem.SaleOrderId)
                                           .ToListAsync();

                var orderFromInvoice = await _invoiceItemRepository.GetAll()
                                             .Where(s => s.OrderItemId.HasValue)
                                             .Where(s => s.ItemIssueItemId.HasValue)
                                             .Where(s => itemIssueFromCreditIds.Contains(s.ItemIssueItemId.Value))
                                             .AsNoTracking()
                                             .Select(s => s.SaleOrderItem.SaleOrderId)
                                             .ToListAsync();

                oldOrderIds = orderFromIssue.Concat(orderFromInvoice).Distinct().ToList();

                var deliveryFromIssue = await _itemIssueItemRepository.GetAll()
                                         .Where(s => s.DeliverySchedulItemId.HasValue)
                                         .Where(s => itemIssueFromCreditIds.Contains(s.Id))
                                         .AsNoTracking()
                                         .Select(s => s.DeliveryScheduleItem.DeliveryScheduleId)
                                         .ToListAsync();

                var deliveryFromInvoice = await _invoiceItemRepository.GetAll()
                                             .Where(s => s.DeliverySchedulItemId.HasValue)
                                             .Where(s => s.ItemIssueItemId.HasValue)
                                             .Where(s => itemIssueFromCreditIds.Contains(s.ItemIssueItemId.Value))
                                             .AsNoTracking()
                                             .Select(s => s.DeliveryScheduleItem.DeliveryScheduleId)
                                             .ToListAsync();

                oldDeliveryIds = deliveryFromIssue.Concat(deliveryFromInvoice).Distinct().ToList();
            }

            var scheduleItems = itemReceiptItems.Select(s => s.ItemId).Distinct().ToList();

            foreach (var iri in @itemReceiptItems)
            {

                CheckErrors(await _itemReceiptCustomerCreditItemManager.RemoveAsync(iri));
            }

            CheckErrors(await _itemReceiptCustomerCreditManager.RemoveAsync(entity));


            if (oldOrderIds.Any() || oldDeliveryIds.Any())
            {
                await this.CurrentUnitOfWork.SaveChangesAsync();
                foreach(var id in oldOrderIds)
                {
                    await UpdateOrderInventoryStatus(id);
                }
                foreach (var id in oldDeliveryIds)
                {
                    await UpdateDeliveryInventoryStatus(id,true);
                }
            }

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.ItemReceipt, TransactionLockType.InventoryTransaction, };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            await DeleteInventoryTransactionItems(input.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, scheduleDate, scheduleItems);
            }

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetDetail,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ViewDetail)]
        public async Task<ItemReceiptCustomerCreditDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            await TurnOffTenantFilterIfDebug();

            var @journal = await _journalRepository
                               .GetAll()
                               .Include(u => u.ItemReceiptCustomerCredit)
                               .Include(u => u.Location)
                               .Include(u => u.Class)
                               .Include(u => u.Currency)
                               .Include(u => u.ItemReceiptCustomerCredit.Customer)
                               .Include(u => u.ItemReceiptCustomerCredit.ShippingAddress)
                               .Include(u => u.ItemReceiptCustomerCredit.BillingAddress)
                               .Include(u=>u.JournalTransactionType)
                               .AsNoTracking()
                               .Where(u => u.JournalType == JournalType.ItemReceiptCustomerCredit && u.ItemReceiptCustomerCreditId == input.Id)
                               .FirstOrDefaultAsync();

            if (@journal == null || @journal.ItemReceiptCustomerCredit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var itemReceiptItemClearance = await _itemReceiptCustomerCreditItemRepository.GetAll()
               .Include(u => u.Item)
               .Include(u => u.Lot)
               .Where(u => u.ItemReceiptCustomerCreditId == input.Id)
               .Join(
                   _journalItemRepository.GetAll()
                   .Where(u => u.Key == PostingKey.COGS)
                   .Include(u => u.Account)
                   .AsNoTracking()
                   ,
                   u => u.Id, s => s.Identifier,
                   (issueItem, jItem) =>
                   new CreateOrUpdateItemReceiptCustomerCreditItemInput()
                   {
                       Id = issueItem.Id,
                       ClearanceAccountId = jItem.AccountId,
                       ClearanceAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                   }).ToListAsync();


            var itemReceiptItems = await (from rItem in _itemReceiptCustomerCreditItemRepository.GetAll()
                                                        .Include(u => u.Item)
                                                        .Include(u => u.CustomerCreditItem)
                                                        .Include(u => u.Lot)
                                                        .Where(u => u.ItemReceiptCustomerCreditId == input.Id)
                                                        .AsNoTracking()

                                          join jItem in _journalItemRepository.GetAll()
                                                        .Include(u => u.Account)
                                                        .Where(u => u.Identifier.HasValue)
                                                        .Where(u => u.Key == PostingKey.Inventory)
                                                        .AsNoTracking()
                                          on rItem.Id equals jItem.Identifier

                                          join i in _inventoryTransactionItemRepository.GetAll()
                                                   .Where(s => s.JournalType == JournalType.ItemReceiptCustomerCredit)
                                                   .Where(s => s.TransactionId == input.Id)
                                                   .AsNoTracking()
                                          on rItem.Id equals i.Id
                                          into cs
                                          from c in cs.DefaultIfEmpty()
                                          select
                                           new CreateOrUpdateItemReceiptCustomerCreditItemInput()
                                           {
                                               ClearanceAccountId = itemReceiptItemClearance.Where(r => r.Id == rItem.Id).Select(r => r.ClearanceAccountId).FirstOrDefault(),
                                               ClearanceAccount = itemReceiptItemClearance.Where(r => r.Id == rItem.Id).Select(r => r.ClearanceAccount).FirstOrDefault(),
                                               Id = rItem.Id,
                                               Item = ObjectMapper.Map<ItemSummaryOutput>(rItem.Item),
                                               ItemId = rItem.ItemId,
                                               InventoryAccountId = jItem.AccountId,
                                               InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                                               Description = rItem.Description,
                                               DiscountRate = rItem.DiscountRate,
                                               Qty = rItem.Qty,
                                               Total = c != null ? c.LineCost : rItem.Total,
                                               UnitCost = c != null ? c.UnitCost : rItem.UnitCost,
                                               CustomerCreditItemId = rItem.CustomerCreditItemId,
                                               LotId = rItem.LotId,
                                               LotDetail = ObjectMapper.Map<LotSummaryOutput>(rItem.Lot),
                                               ItemIssueSaleItemId = rItem.ItemIssueSaleItemId,
                                               CreationTime = jItem.CreationTime,
                                               UseBatchNo = rItem.Item.UseBatchNo,
                                               TrackSerial = rItem.Item.TrackSerial,
                                               TrackExpiration = rItem.Item.TrackExpiration
                                           }).OrderBy(u => u.CreationTime)
                                           .ToListAsync();         

            var result = ObjectMapper.Map<ItemReceiptCustomerCreditDetailOutput>(journal.ItemReceiptCustomerCredit);

            if (result.ReceiveFrom == ReceiveFrom.CustomerCredit)
            {
                result.CustomerCreditId = journal.ItemReceiptCustomerCredit.CustomerCreditId;
                var jv = await _journalRepository.GetAll()
                    .Where(t => t.CustomerCreditId == result.CustomerCreditId.Value).AsNoTracking().FirstOrDefaultAsync();
                result.CustomerCreditNo = jv.JournalNo;
                result.CustomerCreditDate = jv.Date;

            }
            else if (result.ReceiveFrom == ReceiveFrom.ItemIssueSale)
            {
                result.ItemIssueId = journal.ItemReceiptCustomerCredit.ItemIssueSaleId;
                var ji = await _journalRepository.GetAll()
                    .Where(t => t.ItemIssueId == result.ItemIssueId).AsNoTracking().FirstOrDefaultAsync();

                result.ItemIssueNo = ji.JournalNo;
                result.ItemIssueDate = ji.Date;
            }

            if(result.ReceiveFrom == ReceiveFrom.CustomerCredit)
            {
                var creditItemIds = itemReceiptItems.Where(s => s.CustomerCreditItemId.HasValue).Select(s => s.CustomerCreditItemId.Value).ToList();
                if (creditItemIds.Any())
                {
                    var creditItemHasIssue = await _customerCreditItemRepository.GetAll()
                                                    .Where(s => s.ItemIssueSaleItemId.HasValue)
                                                    .Where(s => creditItemIds.Contains(s.Id))
                                                    .AsNoTracking()
                                                    .Select(s => new
                                                    {
                                                        s.ItemIssueSaleItemId,
                                                        CreditItemId = s.Id
                                                    })
                                                    .ToListAsync();

                    if (creditItemHasIssue.Any())
                    {
                        var orderFromIssue = await _itemIssueItemRepository.GetAll()
                                               .Where(s => s.SaleOrderItemId.HasValue || s.DeliverySchedulItemId.HasValue)
                                               .Where(s => creditItemHasIssue.Any(r => r.ItemIssueSaleItemId == s.Id))
                                               .Include(u=>u.DeliveryScheduleItem.DeliverySchedule.SaleOrder)
                                               .AsNoTracking()
                                               .Select(s => new
                                               {
                                                   ItemIssueId = s.Id,
                                                   SaleOrderId =  s.SaleOrderItem !=  null ? s.SaleOrderItem.SaleOrderId : (Guid?)null,
                                                   OrderNumber = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrder.OrderNumber : s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.OrderNumber : null,
                                                   OrderRef =  s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrder.Reference : s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.Reference : null,
                                                   DeliveryId = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliveryScheduleId : (Guid?)null,
                                                   DeliveryNo = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.DeliveryNo: null,
                                                   DeliveryRef = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.Reference : null,
                                               }).ToListAsync();

                        if (orderFromIssue.Any())
                        {
                            itemReceiptItems = itemReceiptItems.Select(s =>
                            {
                                var i = s;
                                var creditItem = creditItemHasIssue.FirstOrDefault(r => r.CreditItemId == s.CustomerCreditItemId);
                                var order = orderFromIssue.FirstOrDefault(o => o.ItemIssueId == creditItem.ItemIssueSaleItemId);
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
                            var orderFromInvoice = await _invoiceItemRepository.GetAll()
                                                         .Where(s => s.OrderItemId.HasValue || s.DeliverySchedulItemId.HasValue)
                                                         .Where(s => s.ItemIssueItemId.HasValue)
                                                         .Where(s => creditItemHasIssue.Any(r => r.ItemIssueSaleItemId == s.ItemIssueItemId))
                                                         .Include(u=>u.DeliveryScheduleItem.DeliverySchedule.SaleOrder)
                                                         .AsNoTracking()
                                                         .Select(s => new
                                                         {
                                                             ItemIssueItemId = s.ItemIssueItemId,
                                                             SaleOrderId = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrderId : (Guid?)null,
                                                             OrderNumber = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrder.OrderNumber : s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.OrderNumber : null,
                                                             OrderRef = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrder.Reference : s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.Reference : null,
                                                             DeliveryId = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliveryScheduleId : (Guid?)null,
                                                             DeliveryNo = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.DeliveryNo : null,
                                                             DeliveryRef = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.Reference : null,
                                                         })
                                                         .ToListAsync();
                            if (orderFromInvoice.Any())
                            {
                                itemReceiptItems = itemReceiptItems.Select(s =>
                                {
                                    var i = s;

                                    var creditItem = creditItemHasIssue.FirstOrDefault(r => r.CreditItemId == s.CustomerCreditItemId);
                                    var order = orderFromInvoice.FirstOrDefault(o => o.ItemIssueItemId == creditItem.ItemIssueSaleItemId);
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
            else if(result.ReceiveFrom == ReceiveFrom.ItemIssueSale)
            {
                var itemIssueIds = itemReceiptItems.Where(s => s.ItemIssueSaleItemId.HasValue).Select(s => s.ItemIssueSaleItemId.Value).ToList();
                if (itemIssueIds.Any())
                {
                    var orderFromIssue = await _itemIssueItemRepository.GetAll()
                                               .Where(s => s.SaleOrderItemId.HasValue || s.DeliverySchedulItemId.HasValue)
                                               .Where(s => itemIssueIds.Contains(s.Id))
                                               .Include(u=>u.DeliveryScheduleItem.DeliverySchedule.SaleOrder)
                                               .AsNoTracking()
                                               .Select(s => new
                                               {
                                                   s.Id,                                               
                                                   SaleOrderId = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrderId : (Guid?)null,
                                                   OrderNumber = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrder.OrderNumber : s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.OrderNumber : null,
                                                   OrderRef = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrder.Reference : s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.Reference : null,
                                                   DeliveryId = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliveryScheduleId : (Guid?)null,
                                                   DeliveryNo = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.DeliveryNo : null,
                                                   DeliveryRef = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.Reference : null,
                                               })
                                               .ToListAsync();

                    if (orderFromIssue.Any())
                    {
                        itemReceiptItems = itemReceiptItems.Select(s =>
                        {
                            var i = s;

                            var order = orderFromIssue.FirstOrDefault(o => o.Id == s.ItemIssueSaleItemId);
                            if(order != null)
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
                        var orderFromInvoice = await _invoiceItemRepository.GetAll()
                                                     .Where(s => s.OrderItemId.HasValue || s.DeliverySchedulItemId.HasValue)
                                                     .Where(s => s.ItemIssueItemId.HasValue)
                                                     .Where(s => itemIssueIds.Contains(s.ItemIssueItemId.Value))
                                                     .AsNoTracking()
                                                     .Select(s => new
                                                     {
                                                         s.ItemIssueItemId,
                                                         SaleOrderId = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrderId : (Guid?)null,
                                                         OrderNumber = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrder.OrderNumber : s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.OrderNumber : null,
                                                         OrderRef = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrder.Reference : s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.Reference : null,
                                                         DeliveryId = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliveryScheduleId : (Guid?)null,
                                                         DeliveryNo = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.DeliveryNo : null,
                                                         DeliveryRef = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.Reference : null,
                                                     })
                                                     .ToListAsync();
                        if (orderFromInvoice.Any())
                        {
                            itemReceiptItems = itemReceiptItems.Select(s =>
                            {
                                var i = s;

                                var order = orderFromInvoice.FirstOrDefault(o => o.ItemIssueItemId == s.ItemIssueSaleItemId);
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

            var batchDic = await _itemReceiptCustomerCreditItemBatchNoRepository.GetAll()
                             .AsNoTracking()
                             .Where(s => s.ItemReceiptCustomerCreditItem.ItemReceiptCustomerCreditId == input.Id)
                             .Select(s => new BatchNoItemOutput
                             {
                                 Id = s.Id,
                                 BatchNoId = s.BatchNoId,
                                 BatchNumber = s.BatchNo.Code,
                                 ExpirationDate = s.BatchNo.ExpirationDate,
                                 Qty = s.Qty,
                                 TransactionItemId = s.ItemReceiptCustomerCreditItemId
                             })
                             .GroupBy(s => s.TransactionItemId)
                             .ToDictionaryAsync(k => k.Key, v => v.ToList());
            if (batchDic.Any())
            {
                foreach (var i in itemReceiptItems)
                {
                    if (batchDic.ContainsKey(i.Id.Value)) i.ItemBatchNos = batchDic[i.Id.Value];
                }
            }


            result.ReceiveFrom = journal.ItemReceiptCustomerCredit.ReceiveFrom;
            result.Date = journal.Date;
            result.ReceiveNo = journal.JournalNo;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.Reference = journal.Reference;
            result.Items = itemReceiptItems;
            result.Memo = journal.Memo;
            result.StatusCode = journal.Status;
            result.LocationId = journal.LocationId.Value;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            result.StatusName = journal.Status.ToString();
            result.TransactionTypeName = journal.JournalTransactionType?.Name;

            //get total from inventory transaction item cache table
            result.Total = itemReceiptItems.Sum(s => s.Total);

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_Update,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Update,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour)]
        public async Task<NullableIdDto<Guid>> Update(UpdateItemReceiptCustomerCreditInput input)
        {

            if (input.IsConfirm == false)
            {
                var validateLockDate = await _lockRepository.GetAll()
                                     .Where(t => t.IsLock == true &&
                                     (t.LockDate.Value.Date >= input.DateCompare.Value.Date || t.LockDate.Value.Date >= input.Date.Date)
                                     && (t.LockKey == TransactionLockType.ItemReceipt || t.LockKey == TransactionLockType.InventoryTransaction)).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            await ValidateBatchNo(input);

            if (input.ReceiveFrom == ReceiveFrom.ItemIssueSale)
            {
                var exceptIds = new List<Guid> { input.Id };
                if(input.CustomerCreditId.HasValue) exceptIds.Add(input.CustomerCreditId.Value);
                await CheckStockForSaleReturn(input.ItemIssueSaleId.Value, input.Items, exceptIds );
            }
            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            // update Journal 
            var @journal = await _journalRepository
                              .GetAll()
                              .Include(s => s.ItemReceiptCustomerCredit.CustomerCredit)
                              .Where(u => u.JournalType == JournalType.ItemReceiptCustomerCredit && u.ItemReceiptCustomerCreditId == input.Id)
                              .FirstOrDefaultAsync();

            await CheckClosePeriod(journal.Date, input.Date);
            //update item receipt 
            var @itemReceipt = @journal.ItemReceiptCustomerCredit;
            //validate EditBy 48 hours
            var getPermission = await GetUserPermissions(userId);
            var IsEdit = getPermission.Where(t => t == AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour).Count();
            var totalHours = (DateTime.Now - itemReceipt.CreationTime).TotalHours;
            if (IsEdit > 0 && totalHours > 48)
            {
                throw new UserFriendlyException(L("ThisTransactionIsOver48HoursCanNotEdit"));
            }

            //update item receipt 
            if (journal == null || journal.ItemReceiptCustomerCredit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            else if (journal.ItemReceiptCustomerCredit.CustomerCredit != null &&
                journal.ItemReceiptCustomerCredit.CustomerCredit.ConvertToItemReceipt == true)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            else if (journal.ItemReceiptCustomerCredit != null &&
                journal.ItemReceiptCustomerCredit.CustomerCredit != null &&
                journal.ItemReceiptCustomerCredit.ReceiveFrom != ReceiveFrom.CustomerCredit &&
                !journal.ItemReceiptCustomerCredit.CustomerCredit.ConvertToItemReceipt)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            else if (journal.ItemReceiptCustomerCredit != null && journal.ItemReceiptCustomerCredit.CustomerCredit != null)
            {
                journal.ItemReceiptCustomerCredit.CustomerCredit.UpdateShipedStatus(DeliveryStatus.ShipPending);
                CheckErrors(await _customerCreditManager.UpdateAsync(journal.ItemReceiptCustomerCredit.CustomerCredit));
            }



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

            var scheduleDate = input.Date > journal.Date ? journal.Date : input.Date;

            journal.ItemReceiptCustomerCredit.Update(tenantId, input.ReceiveFrom, input.CustomerId,
                          input.SameAsShippingAddress, input.ShippingAddress,
                          input.BillingAddress, input.CustomerCreditId, input.Total, input.ItemIssueSaleId);

            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt_CustomerCredit);
            CheckErrors(await _journalManager.UpdateAsync(@journal, autoSequence.DocumentType));


            // CheckErrors(await _journalItemManager.UpdateAsync(@clearanceAccountItem));
            CheckErrors(await _itemReceiptCustomerCreditManager.UpdateAsync(journal.ItemReceiptCustomerCredit));


            //Update Item Receipt customer credit Item and Journal Item
            var itemReceipItems = await _itemReceiptCustomerCreditItemRepository.GetAll().Where(u => u.ItemReceiptCustomerCreditId == input.Id).ToListAsync();

            var oldOrderIds = new List<Guid>();

            var oldDeliveryIds = new List<Guid>();


            var itemIssueItemIds = itemReceipItems.Where(s => s.ItemIssueSaleItemId.HasValue).Select(s => s.ItemIssueSaleItemId.Value).ToList();
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
                var oldCreditItemIds = itemReceipItems.Where(s => s.CustomerCreditItemId.HasValue).Select(s => s.CustomerCreditItemId.Value).ToList();
                var itemIssueFromCreditIds = await _customerCreditItemRepository.GetAll()
                                                       .Where(s => oldCreditItemIds.Contains(s.Id))
                                                       .Where(s => s.ItemIssueSaleItemId.HasValue)
                                                       .AsNoTracking()
                                                       .Select(s => s.ItemIssueSaleItemId.Value)
                                                       .ToListAsync();

                var orderFromIssue = await _itemIssueItemRepository.GetAll()
                                           .Where(s => s.SaleOrderItemId.HasValue)
                                           .Where(s => itemIssueFromCreditIds.Contains(s.Id))
                                           .AsNoTracking()
                                           .Select(s => s.SaleOrderItem.SaleOrderId)
                                           .ToListAsync();

                var orderFromInvoice = await _invoiceItemRepository.GetAll()
                                             .Where(s => s.OrderItemId.HasValue)
                                             .Where(s => s.ItemIssueItemId.HasValue)
                                             .Where(s => itemIssueFromCreditIds.Contains(s.ItemIssueItemId.Value))
                                             .AsNoTracking()
                                             .Select(s => s.SaleOrderItem.SaleOrderId)
                                             .ToListAsync();

                oldOrderIds = orderFromIssue.Concat(orderFromInvoice).Distinct().ToList();

                var deliveryFromIssue = await _itemIssueItemRepository.GetAll()
                                         .Where(s => s.DeliverySchedulItemId.HasValue)
                                         .Where(s => itemIssueFromCreditIds.Contains(s.Id))
                                         .AsNoTracking()
                                         .Select(s => s.DeliveryScheduleItem.DeliveryScheduleId)
                                         .ToListAsync();

                var deliveryFromInvoice = await _invoiceItemRepository.GetAll()
                                             .Where(s => s.DeliverySchedulItemId.HasValue)
                                             .Where(s => s.ItemIssueItemId.HasValue)
                                             .Where(s => itemIssueFromCreditIds.Contains(s.ItemIssueItemId.Value))
                                             .AsNoTracking()
                                             .Select(s => s.DeliveryScheduleItem.DeliveryScheduleId)
                                             .ToListAsync();

                oldDeliveryIds = deliveryFromIssue.Concat(deliveryFromInvoice).Distinct().ToList();
            }


            var @inventoryJournalItems = await (_journalItemRepository.GetAll()
                                  .Where(u => u.JournalId == journal.Id &&
                                         u.Identifier != null)).ToListAsync();

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

            int index = 0;
            foreach (var c in input.Items)
            {
                index++;

                if (c.LotId == null && c.ItemId != null)
                {
                    throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + index.ToString());
                }

                if (c.Id != null) //update
                {
                    var itemReceipItem = itemReceipItems.FirstOrDefault(u => u.Id == c.Id);
                    var journalItem = @inventoryJournalItems.ToList();
                    if (itemReceipItem != null)
                    {
                        //new
                        itemReceipItem.Update(tenantId, c.ItemId, c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total, c.ItemIssueSaleItemId);
                        itemReceipItem.UpdateLot(c.LotId);
                        CheckErrors(await _itemReceiptCustomerCreditItemManager.UpdateAsync(itemReceipItem));

                    }
                    if (journalItem != null && journalItem.Count > 0)
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
                        //journalItem.UpdateJournalItem(userId, c.InventoryAccountId, c.Description, c.Total, 0);
                        //CheckErrors(await _journalItemManager.UpdateAsync(journalItem));
                    }

                    var oldItemBatchs = itemBatchNos.Where(s => s.ItemReceiptCustomerCreditItemId == c.Id).ToList();

                    if (c.ItemBatchNos != null && c.ItemBatchNos.Any())
                    {
                        var addItemBatchNos = c.ItemBatchNos.Where(s => !s.Id.HasValue || s.Id == Guid.Empty).Select(s => ItemReceiptCustomerCreditItemBatchNo.Create(tenantId, userId, itemReceipItem.Id, s.BatchNoId, s.Qty)).ToList();
                        if (addItemBatchNos.Any())
                        {
                            foreach (var a in addItemBatchNos)
                            {
                                await _itemReceiptCustomerCreditItemBatchNoRepository.InsertAsync(a);
                            }
                        }

                        var updateItemBatchNos = oldItemBatchs.Where(s => c.ItemBatchNos.Any(r => r.Id == s.Id)).Select(s =>
                        {
                            var oldItemBatch = s;
                            var newBatch = c.ItemBatchNos.FirstOrDefault(r => r.Id == s.Id);

                            oldItemBatch.Update(userId, itemReceipItem.Id, newBatch.BatchNoId, newBatch.Qty);
                            return oldItemBatch;
                        }).ToList();

                        if (updateItemBatchNos.Any()) await _itemReceiptCustomerCreditItemBatchNoRepository.BulkUpdateAsync(updateItemBatchNos);

                        var deleteItemBatchNos = oldItemBatchs.Where(s => !c.ItemBatchNos.Any(r => r.Id == s.Id)).ToList();
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

                    //validation Qty form Issue sale
                    index++;
                    //insert to item Receipt item
                    var @itemReceiptItem = ItemReceiptItemCustomerCredit.Create(tenantId, userId, journal.ItemReceiptCustomerCredit.Id, c.CustomerCreditItemId, c.ItemId,
                                                                 c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total, c.ItemIssueSaleItemId);
                    itemReceiptItem.UpdateLot(c.LotId);
                    CheckErrors(await _itemReceiptCustomerCreditItemManager.CreateAsync(@itemReceiptItem));


                    var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal,
                        c.ClearanceAccountId, input.Memo, 0, c.Total, PostingKey.COGS, itemReceiptItem.Id);
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
            if (input.Status == TransactionStatus.Publish && input.ReceiveFrom == ReceiveFrom.CustomerCredit)
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

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.ItemReceipt, TransactionLockType.InventoryTransaction, };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            var orderIds = input.Items.Where(s => s.OrderId.HasValue).Select(s => s.OrderId.Value);
            orderIds = orderIds.Concat(oldOrderIds).Distinct();

            var deliveryIds = input.Items.Where(s => s.DeliveryId.HasValue).Select(s => s.DeliveryId.Value);
            deliveryIds = deliveryIds.Concat(deliveryIds).Distinct();

            if (orderIds.Any() || deliveryIds.Any())
            {
                foreach(var id in orderIds)
                {
                    await UpdateOrderInventoryStatus(id);
                }

                foreach (var id in deliveryIds)
                {
                    await UpdateDeliveryInventoryStatus(id,true);
                }
            }

            await SyncItemReceiptCustomerCredit( journal.ItemReceiptCustomerCredit.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, scheduleDate, scheduleItems);
            }

            return new NullableIdDto<Guid>() { Id = journal.ItemReceiptCustomerCredit.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_UpdateStatusToDraft,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Draft)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input)
        {
            var @entity = await _journalRepository
                               .GetAll()
                               .Include(u => u.ItemReceiptCustomerCredit)
                               .Where(u => u.JournalType == JournalType.ItemReceiptCustomerCredit && u.ItemReceiptCustomerCredit.Id == input.Id)
                               .FirstOrDefaultAsync();

            if (entity.ItemReceiptCustomerCredit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            //update customer credit status 
            var customerCredit = await _customerCreditRepository
                                        .GetAll()
                                        .Where(u => u.Id == @entity.ItemReceiptCustomerCredit.CustomerCreditId)
                                        .FirstOrDefaultAsync();
            customerCredit.UpdateShipedStatus(DeliveryStatus.ShipPending);
            CheckErrors(await _customerCreditManager.UpdateAsync(customerCredit));

            entity.UpdateStatusToDraft();
            await CurrentUnitOfWork.SaveChangesAsync();
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_UpdateStatusToPublish,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Update)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input)
        {
            var @entity = await _journalRepository
                                .GetAll()
                                .Include(u => u.ItemReceiptCustomerCredit)
                                .Where(u => u.JournalType == JournalType.ItemReceiptCustomerCredit && u.ItemReceiptCustomerCredit.Id == input.Id)
                                .FirstOrDefaultAsync();

            if (entity.ItemReceiptCustomerCredit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            //update customer credit status 
            var customerCredit = await _customerCreditRepository
                                        .GetAll()
                                        .Where(u => u.Id == @entity.ItemReceiptCustomerCredit.CustomerCreditId)
                                        .FirstOrDefaultAsync();
            customerCredit.UpdateShipedStatus(DeliveryStatus.ShipAll);
            CheckErrors(await _customerCreditManager.UpdateAsync(customerCredit));

            entity.UpdatePublish();
            await CurrentUnitOfWork.SaveChangesAsync();
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_UpdateStatusToVoid,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Void)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input)
        {
            var @entity = await _journalRepository
                               .GetAll()
                               .Include(u => u.ItemReceiptCustomerCredit)
                               .Where(u => u.JournalType == JournalType.ItemReceiptCustomerCredit && u.ItemReceiptCustomerCredit.Id == input.Id)
                               .FirstOrDefaultAsync();

            if (entity.ItemReceiptCustomerCredit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            if (@entity.Status == TransactionStatus.Publish)
            {
                //update customer credit status 
                //var customerCredit = await _customerCreditRepository
                //                            .GetAll()
                //                            .Where(u => u.Id == @entity.ItemReceiptCustomerCreditId)
                //                            .FirstOrDefaultAsync();
                //customerCredit.UpdateShipedStatus(DeliveryStatus.ShipPending);
                //CheckErrors(await _customerCreditManager.UpdateAsync(customerCredit));
            }
            entity.UpdateVoid();
            await CurrentUnitOfWork.SaveChangesAsync();
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }


        #region Get Itemreceipt customer Credit For Customer Credit

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts, AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts_CustomerCredit)]
        public async Task<PagedResultDto<ItemReceiptCusotmerCreditSummaryOutput>> GetItemIssueForCustomerCredit(GetItemReceiptCustomerCreditInput input)
        {
            var customerTypeMemberIds = await GetCustomerTypeMembers().Select(s => s.CustomerTypeId).ToListAsync();

            var userGroups = await GetUserGroupByLocation();
            var currencyQuery = GetCurrencies();
            var customerQuery = GetCustomers(null, input.Customer, null, customerTypeMemberIds);
            var locationQuery = GetLocations(null, input.Locations);

            var icQuery = from ic in _itemReceiptCustomerCreditRepository.GetAll()
                                      .WhereIf(input.Customer != null && input.Customer.Count > 0, u => input.Customer.Contains(u.CustomerId))
                                      .Where(u => u.CustomerCreditId == null)
                                      .AsNoTracking()
                                      .Select(s => new
                                      {
                                          Id = s.Id,
                                          Total = s.Total,
                                          CustomerId = s.CustomerId
                                      })
                          join c in customerQuery
                          on ic.CustomerId equals c.Id
                          select new
                          {
                              Id = ic.Id,
                              Total = ic.Total,
                              CustomerId = ic.CustomerId,
                              CustomerName = c.CustomerName
                          };

            var iciQuery = _itemReceiptCustomerCreditItemRepository.GetAll()
                          .Where(u => u.ItemId != null && u.CustomerCreditItemId == null)
                          .WhereIf(input.Lots != null && input.Lots.Count > 0, t => input.Lots.Contains(t.LotId))
                          .WhereIf(input.Items != null && input.Items.Count > 0, t => input.Items.Contains(t.ItemId))
                          .AsNoTracking()
                          .Select(s => new
                          {
                              s.ItemReceiptCustomerCreditId,
                              s.ItemIssueSaleItemId
                          });


            var jQuery = from j in _journalRepository.GetAll()
                                  .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                                  .WhereIf(input.Locations != null && input.Locations.Count > 0, t => input.Locations.Contains(t.LocationId))
                                  .WhereIf(!input.Filter.IsNullOrEmpty(),
                                        u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) || u.Reference.ToLower().Contains(input.Filter.ToLower()))
                                  .WhereIf(input.FromDate != null && input.ToDate != null,
                                         (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                                  .Where(u => u.Status == TransactionStatus.Publish)
                                  .AsNoTracking()
                                    .Select(j => new
                                    {
                                        ItemReceiptCustomerCreditId = j.ItemReceiptCustomerCreditId,
                                        JournalNo = j.JournalNo,
                                        Date = j.Date,
                                        Reference = j.Reference,
                                        CurrencyId = j.CurrencyId,
                                        LocationId = j.LocationId,
                                    })
                         join c in currencyQuery
                         on j.CurrencyId equals c.Id
                         join l in locationQuery
                         on j.LocationId equals l.Id
                         select new
                         {
                             ItemReceiptCustomerCreditId = j.ItemReceiptCustomerCreditId,
                             JournalNo = j.JournalNo,
                             Date = j.Date,
                             Reference = j.Reference,
                             CurrencyId = j.CurrencyId,
                             LoationId = j.LocationId,
                             LocationName = l.LocationName,
                             CurrencyCode = c.Code
                         };

            var @query = from ic in icQuery
                         join j in jQuery
                         on ic.Id equals j.ItemReceiptCustomerCreditId
                         join ici in iciQuery
                         on ic.Id equals ici.ItemReceiptCustomerCreditId
                         into ics
                         where ics.Count() > 0

                         select new ItemReceiptCusotmerCreditSummaryOutput
                         {
                             LocationNaname = j.LocationName,
                             ItemReceiptNo = j.JournalNo,
                             Customer = new CustomerSummaryOutput
                             {
                                 Id = ic.CustomerId,
                                 CustomerName = ic.CustomerName
                             },
                             CusotmerId = ic.CustomerId,
                             Id = ic.Id,
                             CurrencyId = j.CurrencyId,
                             Currency = new CurrencyDetailOutput
                             {
                                 Id = j.CurrencyId,
                                 Code = j.CurrencyCode
                             },
                             Date = j.Date,
                             Reference = j.Reference,
                             Total = ic.Total,
                             CountItems = ics.Count(),
                         };

            var resultCount = await query.CountAsync();
            if (resultCount == 0) return new PagedResultDto<ItemReceiptCusotmerCreditSummaryOutput>(resultCount, new List<ItemReceiptCusotmerCreditSummaryOutput>());

            var @entities = await query.OrderByDescending(t => t.Date).PageBy(input).ToListAsync();
            return new PagedResultDto<ItemReceiptCusotmerCreditSummaryOutput>(resultCount, @entities);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts,AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts_CustomerCredit)]
        public async Task<PagedResultDto<ItemReceiptCusotmerCreditSummaryOutput>> GetItemIssueForCustomerCreditOld(GetItemReceiptCustomerCreditInput input)
        {

            var userGroups = await GetUserGroupByLocation();
            var @query = (from b in _itemReceiptCustomerCreditRepository.GetAll()
                          .Include(u => u.CustomerCredit)
                          .AsNoTracking()
                          .WhereIf(input.Customer != null && input.Customer.Count > 0, u => input.Customer.Contains(u.CustomerId))
                          .Where(u => u.CustomerCreditId == null)
                          join bi in _itemReceiptCustomerCreditItemRepository.GetAll()
                          .Include(u => u.Item)
                          .Where(u => u.ItemId != null && u.CustomerCreditItemId == null)
                          .Where(u => u.ItemId != null && u.CustomerCreditItemId == null)
                          .WhereIf(input.Lots != null && input.Lots.Count > 0, t => input.Lots.Contains(t.LotId))
                          .WhereIf(input.Items != null && input.Items.Count > 0, t => input.Items.Contains(t.ItemId))
                          .AsNoTracking()
                          on b.Id equals bi.ItemReceiptCustomerCreditId into p
                          join j in _journalRepository.GetAll()
                          .Include(u => u.Currency)
                          .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                          .WhereIf(input.Locations != null && input.Locations.Count > 0, t => input.Locations.Contains(t.LocationId))
                          .WhereIf(!input.Filter.IsNullOrEmpty(),
                                u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) || u.Reference.ToLower().Contains(input.Filter.ToLower()))
                          .WhereIf(input.FromDate != null && input.ToDate != null,
                                 (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                          .Where(u => u.Status == TransactionStatus.Publish)
                          .AsNoTracking() on b.Id equals j.ItemReceiptCustomerCreditId
                          select new ItemReceiptCusotmerCreditSummaryOutput
                          {
                              LocationNaname = j.Location != null ? j.Location.LocationName: null,
                              ItemReceiptNo = j.JournalNo,
                              Customer = ObjectMapper.Map<CustomerSummaryOutput>(b.Customer),
                              CusotmerId = b.CustomerId,
                              Id = b.Id,
                              CurrencyId = j.CurrencyId,
                              Currency = ObjectMapper.Map<CurrencyDetailOutput>(j.Currency),
                              Date = j.Date,
                              Reference = j.Reference,
                              Total = b.Total,
                              CountItems = p.Count(),
                          }).Where(u => u.CountItems > 0);
            var resultCount = await query.CountAsync();

            var @entities = await query.OrderByDescending(t => t.Date).PageBy(input).ToListAsync();
            return new PagedResultDto<ItemReceiptCusotmerCreditSummaryOutput>(resultCount, @entities);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceiptItems)]
        public async Task<ItemReceiptCustomerCreditDetailOutput> GetItemIssueCustomerItems(EntityDto<Guid> input)
        {
            var @journal = await _journalRepository
                               .GetAll()
                               .Include(u => u.ItemReceiptCustomerCredit)
                               .Include(u => u.Location)
                               .Include(u => u.Class)
                               .Include(u => u.Currency)
                               .Include(u => u.ItemReceiptCustomerCredit.Customer.Account)
                               .Include(u => u.ItemReceiptCustomerCredit.ShippingAddress)
                               .Include(u => u.ItemReceiptCustomerCredit.BillingAddress)
                               .AsNoTracking()
                               .Where(u => u.JournalType == JournalType.ItemReceiptCustomerCredit && u.ItemReceiptCustomerCreditId == input.Id)
                               .FirstOrDefaultAsync();

            if (@journal == null || @journal.ItemReceiptCustomerCredit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var itemIssueItemCOGSs = await _itemReceiptCustomerCreditItemRepository.GetAll()
               .Include(u => u.Item)
               .Include(u => u.Lot)
               .Where(u => u.ItemReceiptCustomerCreditId == input.Id)
               .Join(
                   _journalItemRepository.GetAll()
                   .Where(u => u.Key == PostingKey.COGS)
                   .Include(u => u.Account)
                   .AsNoTracking()
                   ,
                   u => u.Id, s => s.Identifier,
                   (issueItem, jItem) =>
                   new CreateOrUpdateItemReceiptCustomerCreditItemInput()
                   {
                       Id = issueItem.Id,
                       ClearanceAccountId = jItem.AccountId,
                       ClearanceAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                   }).ToListAsync();

            var locations = new List<long?>() {
                journal.LocationId
            };
            var itemIssueItems = await _itemReceiptCustomerCreditItemRepository.GetAll()
                .Include(u => u.Item.PurchaseAccount)
                .Include(u => u.Item.InventoryAccount)
                .Include(u => u.Lot)
                .Include(u => u.CustomerCreditItem)
                .Where(u => u.ItemReceiptCustomerCreditId == input.Id)
                .AsNoTracking()
                .Join(_journalItemRepository.GetAll().Include(u => u.Account).Where(u => u.Key == PostingKey.Inventory).AsNoTracking(), u => u.Id, s => s.Identifier,
                (rItem, jItem) =>
                new CreateOrUpdateItemReceiptCustomerCreditItemInput()
                {
                    LotDetail = ObjectMapper.Map<LotSummaryOutput>(rItem.Lot),
                    LotId = rItem.LotId,
                    Id = rItem.Id,
                    Item = ObjectMapper.Map<ItemSummaryOutput>(rItem.Item),
                    ItemId = rItem.ItemId,
                    InventoryAccountId = jItem.AccountId,
                    InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                    ClearanceAccountId = itemIssueItemCOGSs.Where(r => r.Id == rItem.Id).Select(r => r.ClearanceAccountId).FirstOrDefault(),
                    ClearanceAccount = itemIssueItemCOGSs.Where(r => r.Id == rItem.Id).Select(r => r.ClearanceAccount).FirstOrDefault(),
                    Description = rItem.Description,
                    DiscountRate = rItem.DiscountRate,
                    Qty = rItem.Qty,
                    Total = rItem.Total,
                    UnitCost = rItem.UnitCost,
                    CustomerCreditItemId = rItem.CustomerCreditItemId,
                    ItemIssueSaleItemId = rItem.ItemIssueSaleItemId,
                    UseBatchNo = rItem.Item.UseBatchNo,
                    TrackSerial = rItem.Item.TrackSerial,
                    TrackExpiration = rItem.Item.TrackExpiration
                })
                .ToListAsync();


            var itemIssueItemIds = itemIssueItems.Where(s => s.ItemIssueSaleItemId.HasValue).Select(s => s.ItemIssueSaleItemId.Value).ToList();
            if (itemIssueItemIds.Any())
            {

                var itemIssueItemList = await _itemIssueItemRepository.GetAll()
                                              .Include(u=>u.DeliveryScheduleItem.DeliverySchedule.SaleOrder)
                                              .Where(s => itemIssueItemIds.Contains(s.Id))
                                              .Where(s => s.SaleOrderItemId.HasValue || s.DeliverySchedulItemId.HasValue)
                                              .AsNoTracking()
                                              .Select(s => new
                                              {
                                              Id = s.Id,
                                              SaleOrderId = s.SaleOrderItem != null ?  s.SaleOrderItem.SaleOrderId : (Guid?)null,
                                              OrderNumber = s.SaleOrderItem != null ?   s.SaleOrderItem.SaleOrder.OrderNumber : s.DeliveryScheduleItem != null ?s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.OrderNumber : null,
                                              OrderRefe  = s.SaleOrderItem != null? s.SaleOrderItem.SaleOrder.Reference : s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.Reference : null,
                                              DeliveryId = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.Id : (Guid?)null,
                                              DeliveryNo = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.DeliveryNo : null,
                                              DeliveryRefe = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.Reference : null
                                              }).ToListAsync();

                if (itemIssueItemList.Any())
                {
                    itemIssueItems = itemIssueItems.Select(s =>
                    {
                        var i = s;

                        var order = itemIssueItemList.FirstOrDefault(o => o.Id == s.ItemIssueSaleItemId);
                        if(order != null)
                        {
                            i.OrderId = order.SaleOrderId;
                            i.OrderNo = order.OrderNumber;
                            i.OrderRef = order.OrderRefe;
                            i.DeliveryId = order.DeliveryId;
                            i.DeliveryNo = order.DeliveryNo;
                            i.DeliveryRef = order.DeliveryRefe;
                        }

                        return i;
                    })
                    .ToList();
                }
                else
                {
                    var invoiceItemList = await _invoiceItemRepository.GetAll()
                                           .Include(u=>u.DeliveryScheduleItem.DeliverySchedule.DeliveryNo)
                                           .Where(s => s.ItemIssueItemId.HasValue)
                                           .Where(s => itemIssueItemIds.Contains(s.ItemIssueItemId.Value))
                                           .Where(s => s.OrderItemId.HasValue)
                                           .AsNoTracking()
                                           .Select(s => new
                                           {
                                               ItemIssueItemId =  s.ItemIssueItemId,
                                               SaleOrderId = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrderId : (Guid?)null,
                                               OrderNumber = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrder.OrderNumber : s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.OrderNumber : null,
                                               OrderRefe = s.SaleOrderItem != null ? s.SaleOrderItem.SaleOrder.Reference : s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.SaleOrder.Reference : null,
                                               DeliveryId = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.Id : (Guid?)null,
                                               DeliveryNo = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.DeliveryNo : null,
                                               DeliveryRefe = s.DeliveryScheduleItem != null ? s.DeliveryScheduleItem.DeliverySchedule.Reference : null
                                           }).ToListAsync();

                    if (invoiceItemList.Any())
                    {
                        itemIssueItems = itemIssueItems.Select(s =>
                        {
                            var i = s;

                            var order = invoiceItemList.FirstOrDefault(o => o.ItemIssueItemId == s.ItemIssueSaleItemId);
                            if (order != null)
                            {
                                i.OrderId = order.SaleOrderId;
                                i.OrderNo = order.OrderNumber;
                                i.OrderRef = order.OrderRefe;
                                i.DeliveryId = order.DeliveryId;
                                i.DeliveryNo = order.DeliveryNo;
                                i.DeliveryRef = order.DeliveryRefe;
                            }

                            return i;
                        })
                        .ToList();
                    }
                }
            }



            var batchDic = await _itemReceiptCustomerCreditItemBatchNoRepository.GetAll()
                             .AsNoTracking()
                             .Where(s => s.ItemReceiptCustomerCreditItem.ItemReceiptCustomerCreditId == input.Id)
                             .Select(s => new BatchNoItemOutput
                             {
                                 Id = s.Id,
                                 BatchNoId = s.BatchNoId,
                                 BatchNumber = s.BatchNo.Code,
                                 ExpirationDate = s.BatchNo.ExpirationDate,
                                 Qty = s.Qty,
                                 TransactionItemId = s.ItemReceiptCustomerCreditItemId
                             })
                             .GroupBy(s => s.TransactionItemId)
                             .ToDictionaryAsync(k => k.Key, v => v.ToList());
            if (batchDic.Any())
            {
                foreach (var i in itemIssueItems)
                {
                    if (batchDic.ContainsKey(i.Id.Value)) i.ItemBatchNos = batchDic[i.Id.Value];
                }
            }


            var result = ObjectMapper.Map<ItemReceiptCustomerCreditDetailOutput>(journal.ItemReceiptCustomerCredit);
            result.Date = journal.Date;
            result.ReceiveNo = journal.JournalNo;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.Reference = journal.Reference;
            result.Items = itemIssueItems;
            result.Memo = journal.Memo;
            result.StatusCode = journal.Status;
            result.StatusName = journal.Status.ToString();
            result.LocationId = journal.LocationId.Value;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            return result;
        }
        #endregion


    }
}
