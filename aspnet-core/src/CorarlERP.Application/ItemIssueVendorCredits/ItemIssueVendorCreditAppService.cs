using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Authorization;
using CorarlERP.AutoSequences;
using CorarlERP.ChartOfAccounts;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.Inventories;
using CorarlERP.ItemIssues;
using CorarlERP.ItemIssues.Dto;
using CorarlERP.ItemIssueVendorCredits.Dto;
using CorarlERP.ItemReceiptCustomerCredits;
using CorarlERP.ItemReceipts;
using CorarlERP.Items;
using CorarlERP.Items.Dto;
using CorarlERP.Journals;
using CorarlERP.Journals.Dto;
using CorarlERP.Locations.Dto;
using CorarlERP.Lots;
using CorarlERP.Lots.Dto;
using CorarlERP.Taxes.Dto;
using CorarlERP.VendorCredit;
using CorarlERP.Vendors;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.Customers.Dto;
using CorarlERP.SaleOrders.Dto;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore.Internal;
using CorarlERP.Vendors.Dto;
using Abp.Extensions;
using CorarlERP.UserGroups;
using CorarlERP.Locks;
using CorarlERP.Common.Dto;
using CorarlERP.InventoryTransactionItems;
using CorarlERP.InventoryCalculationJobSchedules;
using Hangfire.States;
using CorarlERP.InventoryCalculationItems;
using CorarlERP.BatchNos;
using CorarlERP.ItemReceiptCustomerCredits.Dto;
using CorarlERP.JournalTransactionTypes;
using CorarlERP.Features;

namespace CorarlERP.ItemIssueVendorCredits
{
    [AbpAuthorize]
    public class ItemIssueVendorCreditAppService : CorarlERPAppServiceBase, IItemIssueVendorCreditAppService
    {
        private readonly IInventoryManager _inventoryManager;
        private readonly IRepository<ItemReceiptItem, Guid> _itemReceiptItemRepository;
        private readonly IRepository<ItemIssueItem, Guid> _itemIssueItemRepository;
        private readonly IRepository<Lot, long> _lotRepository;
        private readonly IItemManager _itemManager;
        private readonly IRepository<Item, Guid> _itemRepository;

        private readonly IVendorCreditDetailManager _vendorCreditItemManager;
        private readonly IRepository<VendorCreditDetail, Guid> _vendorCreditItemRepository;
        private readonly IVendorCreditManager _vendorCreditManager;
        private readonly IRepository<VendorCredit.VendorCredit, Guid> _vendorCreditRepository;

        private readonly IVendorManager _vendorManager;
        private readonly IRepository<Vendor, Guid> _vendorRepository;

        private readonly IRepository<ItemReceiptItemCustomerCredit, Guid> _itemReceiptCustomerCreditItemRepository;

        private readonly IItemIssueVendorCreditItemManager _itemIssueVendorCreditItemManager;
        private readonly IRepository<ItemIssueVendorCreditItem, Guid> _itemIssueVendorCreditItemRepository;
        private readonly IItemIssueVendorCreditManager _itemIssueVendorCreditManager;
        private readonly IRepository<ItemIssueVendorCredit, Guid> _itemIssueVendorCreditRepository;

        private readonly IJournalManager _journalManager;
        private readonly IRepository<Journal, Guid> _journalRepository;
        private readonly IJournalItemManager _journalItemManager;
        private readonly IRepository<JournalItem, Guid> _journalItemRepository;
        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly IChartOfAccountManager _chartOfAccountManager;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;
        private readonly ICorarlRepository<InventoryTransactionItem, Guid> _inventoryTransactionItemRepository;
        private readonly IInventoryCalculationItemManager _inventoryCalculationItemManager;
        private readonly ICorarlRepository<BatchNo, Guid> _batchNoRepository;
        private readonly ICorarlRepository<VendorCreditItemBatchNo, Guid> _vendorCreditItemBatchNoRepository;
        private readonly ICorarlRepository<ItemIssueVendorCreditItemBatchNo, Guid> _itemIssueVendorCreditItemBatchNoRepository;
        private readonly ICorarlRepository<ItemReceiptItemBatchNo, Guid> _itemReceiptItemBatchNoRepository;
        private readonly IJournalTransactionTypeManager _journalTransactionTypeManager;
        private readonly IRepository<Lock, long> _lockRepository; 
        public ItemIssueVendorCreditAppService(
            ICorarlRepository<InventoryTransactionItem, Guid> inventoryTransactionItemRepository,
            IJournalManager journalManager,
            IJournalItemManager journalItemManager,
            IChartOfAccountManager chartOfAccountManager,
            ItemIssueVendorCreditManager itemIssueVendorCreditManager,
            ItemIssueVendorCreditItemManager itemIssueVendorCreditItemManager,
            ItemManager itemManager,
            VendorManager vendorManager,
            VendorCreditDetailManager vendorCreditItemManager,
            VendorCreditManager vendorCreditManager,
            IRepository<VendorCreditDetail, Guid> vendorCreditItemRepository,
            IRepository<VendorCredit.VendorCredit, Guid> vendorCreditRepository,
            IRepository<Item, Guid> itemRepository,
            IRepository<Vendor, Guid> vendorRepository,
            IRepository<Journal, Guid> journalRepository,
            IRepository<JournalItem, Guid> journalItemRepository,
            IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
            IRepository<ItemIssueVendorCredit, Guid> itemIssueVendorCreditRepository,
            IRepository<ItemIssueVendorCreditItem, Guid> itemIssueVendorCreditItemRepository,
            IRepository<ItemReceiptItem, Guid> itemReceiptItemRepository,
            IRepository<ItemIssueItem, Guid> itemIssueItemRepository,
            IRepository<ItemReceiptItemCustomerCredit, Guid> itemReceiptCustomerCreditItemRepository,
            IInventoryCalculationItemManager inventoryCalculationItemManager,
            IInventoryManager inventoryManager,
            IRepository<AutoSequence, Guid> autoSequenceRepository,
            ICorarlRepository<BatchNo, Guid> batchNoRepository,
            ICorarlRepository<VendorCreditItemBatchNo, Guid> vendorCreditItemBatchNoRepository,
            ICorarlRepository<ItemIssueVendorCreditItemBatchNo, Guid> itemIssueVendorCreditItemBatchNoRepository,
            ICorarlRepository<ItemReceiptItemBatchNo, Guid> itemReceiptItemBatchNoRepository,
            IAutoSequenceManager autoSequenceManager,
            IRepository<Lot, long> lotRepository,
            IRepository<Locations.Location, long> locationRepository,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            IRepository<Lock, long> lockRepository,
            IRepository<AccountCycles.AccountCycle,long> accountCycleRepository,
            IJournalTransactionTypeManager journalTransactionTypeManager
        ) : base(accountCycleRepository,userGroupMemberRepository,locationRepository)
        {
            _lotRepository = lotRepository;
            _itemReceiptCustomerCreditItemRepository = itemReceiptCustomerCreditItemRepository;
            _journalManager = journalManager;
            _journalManager.SetJournalType(JournalType.ItemIssueVendorCredit);
            _journalRepository = journalRepository;
            _journalItemManager = journalItemManager;
            _journalItemRepository = journalItemRepository;
            _chartOfAccountManager = chartOfAccountManager;
            _chartOfAccountRepository = chartOfAccountRepository;
            _itemIssueVendorCreditItemManager = itemIssueVendorCreditItemManager;
            _itemIssueVendorCreditItemRepository = itemIssueVendorCreditItemRepository;
            _itemIssueVendorCreditManager = itemIssueVendorCreditManager;
            _itemIssueVendorCreditRepository = itemIssueVendorCreditRepository;

            _vendorRepository = vendorRepository;
            _vendorManager = vendorManager;
            _vendorCreditItemRepository = vendorCreditItemRepository;
            _vendorCreditItemManager = vendorCreditItemManager;
            _vendorCreditRepository = vendorCreditRepository;
            _vendorCreditManager = vendorCreditManager;
            _itemRepository = itemRepository;
            _itemManager = itemManager;
            _itemIssueItemRepository = itemIssueItemRepository;
            _itemReceiptItemRepository = itemReceiptItemRepository;
            _inventoryManager = inventoryManager;
            _autoSequenceManager = autoSequenceManager;
            _autoSequenceRepository = autoSequenceRepository;
            _lockRepository = lockRepository;
            _inventoryTransactionItemRepository = inventoryTransactionItemRepository;
            _inventoryCalculationItemManager = inventoryCalculationItemManager;
            _batchNoRepository = batchNoRepository;
            _itemIssueVendorCreditItemBatchNoRepository = itemIssueVendorCreditItemBatchNoRepository;
            _vendorCreditItemBatchNoRepository = vendorCreditItemBatchNoRepository;
            _itemReceiptItemBatchNoRepository = itemReceiptItemBatchNoRepository;
            _journalTransactionTypeManager = journalTransactionTypeManager;
        }

       
        private async Task ValidateBatchNo(CreateItemIssueVendorCreditInput input)
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
            if (find != null) throw new UserFriendlyException(L("PleaseSelect", $"{L("BatchNo")}/{L("SerialNo")}") + $", Item: {find.Item.ItemCode}-{find.Item.ItemName}");

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

            var itemIds = itemUseBatchs.GroupBy(s => s.ItemId).Select(s => s.Key).ToList();
            var lots = itemUseBatchs.GroupBy(s => s.FromLotId).Select(s => s.Key.Value).ToList();
            var exceptIds = new List<Guid>();
            if (input is UpdateItemIssueVendorCreditInput)
            {
                exceptIds.Add((input as UpdateItemIssueVendorCreditInput).Id);
            }

            var batchBalanceItems = await _inventoryManager.GetItemBatchNoBalance(itemIds, lots, batchNoIds, input.Date, exceptIds);
            var balanceDic = batchBalanceItems.ToDictionary(k => $"{k.ItemId}-{k.LotId}-{k.BatchNoId}", v => v.Qty);

            var batchQtyUse = itemUseBatchs
                              .SelectMany(s => s.ItemBatchNos.Select(r => new { s.ItemId, s.FromLotId, r.BatchNoId, r.Qty }))
                              .GroupBy(s => new { s.ItemId, s.FromLotId, s.BatchNoId })
                              .Select(s => new
                              {
                                  ItemName = $"{batchItemDic[s.Key.ItemId].ItemCode}-{batchItemDic[s.Key.ItemId].ItemName}, BatchNo: {batchNoDic[s.Key.BatchNoId].Code}",
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


        private async Task CheckStockForPurchaseReturn(Guid itemReceiptPurchaseId, List<CreateOrUpdateItemIssueVendorCreditItemInput> inputItems, List<Guid> exceptIds)
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

            var cQuery = from r in _vendorCreditItemRepository.GetAll()
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
                        var checkStock = balanceList.FirstOrDefault(i => i.Id == r.ItemReceiptPurhcaseItemId && i.ItemId == r.ItemId && i.BatchNoId == b.BatchNoId && i.Qty < b.Qty);
                        if (checkStock != null) throw new UserFriendlyException(L("ReturnQtyCannotGreaterThanPurchaseQty"));
                    }
                }
                else
                {
                    var checkStock = balanceList.FirstOrDefault(i => i.Id == r.ItemReceiptPurhcaseItemId && i.ItemId == r.ItemId && i.Qty < r.Qty);
                    if (checkStock != null) throw new UserFriendlyException(L("ReturnQtyCannotGreaterThanPurchaseQty"));
                }
            }
        }



        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_CanCreateVendorCredit,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueVendorCredit)]
        public async Task<NullableIdDto<Guid>> Create(CreateItemIssueVendorCreditInput input)
        {

            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                    .Where(t => (t.LockKey == TransactionLockType.ItemIssue || t.LockKey == TransactionLockType.InventoryTransaction)
                    && t.IsLock == true && t.LockDate.Value.Date >= input.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            await ValidateBatchNo(input);

            if (input.ReceiveFrom == ReceiveFrom.ItemReceiptPurchase)
            {
                await CheckStockForPurchaseReturn(input.ItemReceiptPurchaseId.Value, input.Items, null);

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
            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }
            

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            //insert to journal
            var @entity = Journal.Create(tenantId, userId, input.IssueNo, input.Date,
                                        input.Memo, input.Total, input.Total, input.CurrencyId,
                                        input.ClassId, input.Reference,input.LocationId);
            entity.UpdateStatus(input.Status);
            #region journal transaction type 
            var transactionTypeId = await _journalTransactionTypeManager.GetJournalTransactionTypeId(InventoryTransactionType.ItemIssueVendorCredit);
            entity.SetJournalTransactionTypeId(transactionTypeId);
            #endregion
            var validatelot = input.Items.Where(t => t.FromLotId == null && t.ItemId != null).FirstOrDefault();
            if (validatelot != null)
            {               
                    throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + (input.Items.IndexOf(validatelot) + 1) .ToString());              
            }

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
            var getCostResult = await _inventoryManager.GetAvgCostForIssues(input.Date, locationIds, itemToCalculateCost/*, @entity, roundingId*/);

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
                                    input.Total,input.ItemReceiptPurchaseId);

            itemIssueVendorCredit.UpdateTransactionType(InventoryTransactionType.ItemIssueVendorCredit);
            @entity.UpdateItemIssueVendorCredit(itemIssueVendorCredit);
            @entity.UpdateCreditDebit(input.Total, input.Total);            
            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));           
            CheckErrors(await _itemIssueVendorCreditManager.CreateAsync(itemIssueVendorCredit));

           
            foreach (var i in input.Items)
            {
               
                //insert to item Receipt item
                var itemIssueItem = ItemIssueVendorCreditItem.Create(tenantId, userId, itemIssueVendorCredit.Id, i.VendorCreditItemId, i.ItemId, i.Description, i.Qty, i.UnitCost, i.DiscountRate, i.Total,i.ItemReceiptPurhcaseItemId);
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
            if (input.Status == TransactionStatus.Publish && input.ReceiveFrom == ReceiveFrom.VendorCredit && input.VendorCreditId != null)
            {
                var vendorCredit = await _vendorCreditRepository.GetAll().Where(u => u.Id == input.VendorCreditId).FirstOrDefaultAsync();
                vendorCredit.UpdateShipedStatus(DeliveryStatus.ShipAll);
                CheckErrors(await _vendorCreditManager.UpdateAsync(vendorCredit));
            }

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.ItemIssue, TransactionLockType.InventoryTransaction, };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            await SyncItemIssueVendorCredit(itemIssueVendorCredit.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                var scheduleItems = input.Items.Select(s => s.ItemId).Distinct().ToList();
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, input.Date, scheduleItems);
            }

            return new NullableIdDto<Guid>() { Id = itemIssueVendorCredit.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_Delete,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Delete,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour)]
        public async Task Delete(CarlEntityDto input)
        {

            var journal = await _journalRepository.GetAll()
                .Include(u => u.ItemIssueVendorCredit.VendorCredit)
                .Include(u => u.ItemIssueVendorCredit.ShippingAddress)
                .Include(u => u.ItemIssueVendorCredit.BillingAddress)
                .Where(u => u.JournalType == JournalType.ItemIssueVendorCredit && u.ItemIssueVendorCreditId == input.Id)
                .FirstOrDefaultAsync();

            if(input.IsConfirm == false)
            {

                var locktransaction = await _lockRepository.GetAll()
                                       .Where(t => (t.LockKey == TransactionLockType.ItemIssue || t.LockKey == TransactionLockType.InventoryTransaction)
                                       && t.IsLock == true && t.LockDate.Value.Date >= journal.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }


            if (journal==null || journal.ItemIssueVendorCredit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            else if (journal.ItemIssueVendorCredit.VendorCredit!=null && 
                journal.ItemIssueVendorCredit.VendorCredit.ConvertToItemIssueVendor == true)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            else if (journal.ItemIssueVendorCredit != null &&
                journal.ItemIssueVendorCredit.ReceiveFrom != ReceiveFrom.VendorCredit &&
                journal.ItemIssueVendorCredit.VendorCredit != null )
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            else if(journal.ItemIssueVendorCredit != null && journal.ItemIssueVendorCredit.VendorCredit != null)
            {
                journal.ItemIssueVendorCredit.VendorCredit.UpdateShipedStatus(DeliveryStatus.ShipPending);
                CheckErrors(await _vendorCreditManager.UpdateAsync(journal.ItemIssueVendorCredit.VendorCredit));
            }

            var @entity = journal.ItemIssueVendorCredit;
            var userId = AbpSession.GetUserId();
            var getPermission = await GetUserPermissions(userId);
            var IsEdit = getPermission.Where(t => t == AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour).Count();
            var totalHours = (DateTime.Now - @entity.CreationTime).TotalHours;
            if (IsEdit > 0 && totalHours > 48)
            {
                throw new UserFriendlyException(L("ThisTransactionIsOver48HoursCanNotEdit"));
            }


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

            //@Delete rounded adjustment journal
            //if (journal.RoundedAdjustmentJournalId != null)
            //{
            //    var @entityRoundedAdjustment = await _journalRepository.GetAll().Where(t => t.Id == journal.RoundedAdjustmentJournalId).FirstOrDefaultAsync();

            //    if (@entityRoundedAdjustment == null)
            //    {
            //        throw new UserFriendlyException(L("RecordNotFound"));
            //    }

            //    var JournalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == @entityRoundedAdjustment.Id).ToListAsync();

            //    foreach (var s in JournalItems)
            //    {
            //        CheckErrors(await _journalItemManager.RemoveAsync(s));
            //    }
            //    CheckErrors(await _journalManager.RemoveAsync(@entityRoundedAdjustment));
            //}

            //journal.UpdateItemIssueVendorCredit(null);

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

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.ItemIssue, TransactionLockType.InventoryTransaction, };
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

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_GetDetail,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ViewDetail)]
        public async Task<ItemIssueVendorCreditDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            await TurnOffTenantFilterIfDebug();

            var @journal = await _journalRepository
                               .GetAll()                              
                               .Include(u => u.ItemIssueVendorCredit)
                               .Include(u=>u.ItemIssueVendorCredit.ItemReceiptPurchase)
                               .Include(u=>u.ItemIssueVendorCredit.VendorCredit)
                               .Include(u => u.Location)
                               .Include(u => u.Class)
                               .Include(u => u.Currency)
                               .Include(u => u.ItemIssueVendorCredit.Vendor)
                               .Include(u => u.ItemIssueVendorCredit.ShippingAddress)
                               .Include(u => u.ItemIssueVendorCredit.BillingAddress)
                               .Include(u=>u.JournalTransactionType)
                               .AsNoTracking()
                               .Where(u => u.JournalType == JournalType.ItemIssueVendorCredit && u.ItemIssueVendorCreditId == input.Id)
                               .FirstOrDefaultAsync();

            if (@journal == null || @journal.ItemIssueVendorCredit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var itemIssueItemCOGSs = await _itemIssueVendorCreditItemRepository.GetAll()
               .Include(u => u.Item)
               .Include(u => u.Lot)
               .Where(u => u.ItemIssueVendorCreditId == input.Id)
               .Join(
                   _journalItemRepository.GetAll()
                   .Where(u => u.Key == PostingKey.COGS)
                   .Include(u => u.Account)
                   .AsNoTracking()
                   ,
                   u => u.Id, s => s.Identifier,
                   (issueItem, jItem) =>
                   new CreateOrUpdateItemIssueVendorCreditItemInput()
                   {
                       Id = issueItem.Id,
                       PurchaseAccountId = jItem.AccountId,
                       PurchaseAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                   }).ToListAsync();

            var locations = new List<long?>() {
                journal.LocationId
            };

            var itemIssueItems = await (from rItem in _itemIssueVendorCreditItemRepository.GetAll()
                                                    .Include(u => u.Item.PurchaseAccount)
                                                    .Include(u => u.Item.InventoryAccount)
                                                    .Include(u => u.Lot)
                                                    .Include(u => u.VendorCreditItem)
                                                    .Where(u => u.ItemIssueVendorCreditId == input.Id)
                                                    .AsNoTracking()
                                        join jItem in _journalItemRepository.GetAll()
                                                    .Include(u => u.Account)
                                                    .Where(u => u.Identifier.HasValue)
                                                    .Where(u => u.Key == PostingKey.Inventory)
                                                    .AsNoTracking()
                                        on rItem.Id equals jItem.Identifier

                                        join i in _inventoryTransactionItemRepository.GetAll()
                                                                            .Where(s => s.JournalType == JournalType.ItemIssueVendorCredit)
                                                                            .Where(s => s.TransactionId == input.Id)
                                                                            .AsNoTracking()
                                                                    on rItem.Id equals i.Id
                                                                    into cs
                                        from c in cs.DefaultIfEmpty()
                                        select
                                        new CreateOrUpdateItemIssueVendorCreditItemInput()
                                        {
                                            FromLotDetail = ObjectMapper.Map<LotSummaryOutput>(rItem.Lot),
                                            FromLotId = rItem.LotId,
                                            Id = rItem.Id,
                                            Item = ObjectMapper.Map<ItemSummaryDetailOutput>(rItem.Item),
                                            ItemId = rItem.ItemId,
                                            InventoryAccountId = jItem.AccountId,
                                            InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                                            PurchaseAccountId = itemIssueItemCOGSs.Where(r => r.Id == rItem.Id).Select(r => r.PurchaseAccountId).FirstOrDefault(),
                                            PurchaseAccount = itemIssueItemCOGSs.Where(r => r.Id == rItem.Id).Select(r => r.PurchaseAccount).FirstOrDefault(),
                                            COGSAccountId = itemIssueItemCOGSs.Where(r => r.Id == rItem.Id).Select(r => r.PurchaseAccountId.Value).FirstOrDefault(),
                                            COGSAccount = itemIssueItemCOGSs.Where(r => r.Id == rItem.Id).Select(r => r.PurchaseAccount).FirstOrDefault(),
                                            Description = rItem.Description,
                                            DiscountRate = rItem.DiscountRate,
                                            Qty = rItem.Qty,
                                            Total = c != null ? Math.Abs(c.LineCost) : rItem.Total,
                                            UnitCost = c != null ? c.UnitCost : rItem.UnitCost,
                                            VendorCreditItemId = rItem.VendorCreditItemId,
                                            ItemReceiptPurhcaseItemId = rItem.ItemReceiptPurchaseItemId,
                                            CreationTime = jItem.CreationTime,
                                            UseBatchNo = rItem.Item != null && rItem.Item.UseBatchNo,
                                            TrackSerial = rItem.Item != null && rItem.Item.TrackSerial,
                                            TrackExpiration = rItem.Item != null && rItem.Item.TrackExpiration
                                        }).OrderBy(u => u.CreationTime)
                                        .ToListAsync();
            
            var result = ObjectMapper.Map<ItemIssueVendorCreditDetailOutput>(journal.ItemIssueVendorCredit);
          
                    
            if (result.ReceiveFrom == ReceiveFrom.VendorCredit)
            {
                result.VendorCreditId = journal.ItemIssueVendorCredit.VendorCreditId;
                var jv = await _journalRepository.GetAll()
                    .Where(t => t.VendorCreditId == result.VendorCreditId.Value).AsNoTracking().FirstOrDefaultAsync();               
                result.VendorCreditNo = jv.JournalNo;
                result.VendorCreditDate = jv.Date;

            }
            else if (result.ReceiveFrom == ReceiveFrom.ItemReceiptPurchase)
            {
                result.ItemReceiptPurchaseId = journal.ItemIssueVendorCredit.ItemReceiptPurchaseId;
                var ji = await _journalRepository.GetAll()
                    .Where(t => t.ItemReceiptId == result.ItemReceiptPurchaseId).AsNoTracking().FirstOrDefaultAsync();
                
                result.ItemReceiptPurchaseNo = ji.JournalNo;
                result.ItemReceiptPurchaseDate = ji.Date;
            }


            var batchDic = await _itemIssueVendorCreditItemBatchNoRepository.GetAll()
                             .AsNoTracking()
                             .Where(s => s.ItemIssueVendorCreditItem.ItemIssueVendorCreditId == input.Id)
                             .Select(s => new BatchNoItemOutput
                             {
                                 Id = s.Id,
                                 BatchNoId = s.BatchNoId,
                                 BatchNumber = s.BatchNo.Code,
                                 ExpirationDate = s.BatchNo.ExpirationDate,
                                 Qty = s.Qty,
                                 TransactionItemId = s.ItemIssueVendorCreditItemId
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


            result.ReceiveFrom = journal.ItemIssueVendorCredit.ReceiveFrom;
            result.Date = journal.Date;
            result.IssueNo = journal.JournalNo;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.Reference = journal.Reference;
            result.Items = itemIssueItems;
            //result.Account = ObjectMapper.Map<ChartAccountSummaryOutput>(cogsAccount);
            //result.AccountId = cogsAccount.Id;
            result.Memo = journal.Memo;
            result.StatusCode = journal.Status;
            result.StatusName = journal.Status.ToString();
            result.LocationId = journal.LocationId.Value;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            result.TransactionTypeName = journal.JournalTransactionType?.Name;

            //get total from inventory transaction item cache table
            result.Total = itemIssueItems.Sum(s => s.Total);

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_Update,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Update,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour)]
        public async Task<NullableIdDto<Guid>> Update(UpdateItemIssueVendorCreditInput input)
        {

            if (input.IsConfirm == false)
            {

                var validateLockDate = await _lockRepository.GetAll()
                                      .Where(t => t.IsLock == true &&
                                      (t.LockDate.Value.Date >= input.DateCompare.Value.Date || t.LockDate.Value.Date >= input.Date.Date)
                                      && (t.LockKey == TransactionLockType.ItemIssue || t.LockKey == TransactionLockType.InventoryTransaction)).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            await ValidateBatchNo(input);

            if (input.ReceiveFrom == ReceiveFrom.ItemReceiptPurchase)
            {
                var exceptIds = new List<Guid> { input.Id };
                if (input.VendorCreditId.HasValue) exceptIds.Add(input.VendorCreditId.Value);
                await CheckStockForPurchaseReturn(input.ItemReceiptPurchaseId.Value, input.Items, exceptIds);

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
                              .Include(u=>u.ItemIssueVendorCredit.VendorCredit)
                              .Where(u => u.JournalType == JournalType.ItemIssueVendorCredit && u.ItemIssueVendorCreditId == input.Id)
                              .FirstOrDefaultAsync();
            await CheckClosePeriod(journal.Date, input.Date);
            //update item Issue 
            var itemIssue = journal.ItemIssueVendorCredit;
            var getPermission = await GetUserPermissions(userId);
            var IsEdit = getPermission.Where(t => t == AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour).Count();
            var totalHours = (DateTime.Now - itemIssue.CreationTime).TotalHours;
            if (IsEdit > 0 && totalHours > 48)
            {
                throw new UserFriendlyException(L("ThisTransactionIsOver48HoursCanNotEdit"));
            }

            //update item receipt 

            if (journal == null || journal.ItemIssueVendorCredit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            else if (journal.ItemIssueVendorCredit.VendorCredit != null && 
                journal.ItemIssueVendorCredit.VendorCredit.ConvertToItemIssueVendor == true)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            else if (journal.ItemIssueVendorCredit != null && 
                journal.ItemIssueVendorCredit.VendorCredit != null &&
                journal.ItemIssueVendorCredit.ReceiveFrom != ReceiveFrom.VendorCredit)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            else if (journal.ItemIssueVendorCredit != null && journal.ItemIssueVendorCredit.VendorCredit != null)
            {
                journal.ItemIssueVendorCredit.VendorCredit.UpdateShipedStatus(DeliveryStatus.ShipPending);
                CheckErrors(await _vendorCreditManager.UpdateAsync(journal.ItemIssueVendorCredit.VendorCredit));
            }

            journal.Update(tenantId, input.IssueNo, input.Date, input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, 0,input.LocationId);
            journal.UpdateStatus(input.Status);

            var validatelot = input.Items.Where(t => t.FromLotId == null && t.ItemId != null).FirstOrDefault();
            if (validatelot != null)
            {
                throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + (input.Items.IndexOf(validatelot) + 1).ToString());
            }

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

            var scheduleDate = input.Date > journal.Date ? journal.Date : input.Date;
           
            journal.ItemIssueVendorCredit.Update(tenantId, input.ReceiveFrom, input.VendorId,
                          input.SameAsShippingAddress, input.ShippingAddress,
                          input.BillingAddress, input.VendorCreditId, input.Total,input.ItemReceiptPurchaseId);

            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue_VendorCredit);
            CheckErrors(await _journalManager.UpdateAsync(@journal, autoSequence.DocumentType));
            
            CheckErrors(await _itemIssueVendorCreditManager.UpdateAsync(journal.ItemIssueVendorCredit));


            //Update Item Receipt customer credit Item and Journal Item
            var itemIssueItems = await _itemIssueVendorCreditItemRepository.GetAll().Where(u => u.ItemIssueVendorCreditId == input.Id).ToListAsync();
            
            var @inventoryJournalItems = await (_journalItemRepository.GetAll()
                                            .Where(u => u.JournalId == journal.Id &&
                                             u.Identifier != null)
                                        ).ToListAsync();
            
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
                    

                    var itemIssueItem = itemIssueItems.FirstOrDefault(u => u.Id == c.Id);
                   
                    var journalItem = @inventoryJournalItems.ToList().Where(u => u.Identifier == itemIssueItem.Id);

                    if (itemIssueItem != null)
                    {
                        //new
                        itemIssueItem.Update(tenantId, c.ItemId, c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total);
                        itemIssueItem.UpdateLot(c.FromLotId);
                        CheckErrors(await _itemIssueVendorCreditItemManager.UpdateAsync(itemIssueItem));

                    }
                    if (journalItem != null)
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
                        var addItemBatchNos = c.ItemBatchNos.Where(s => !s.Id.HasValue || s.Id == Guid.Empty).Select(s => ItemIssueVendorCreditItemBatchNo.Create(tenantId, userId, itemIssueItem.Id, s.BatchNoId, s.Qty)).ToList();
                        if (addItemBatchNos.Any())
                        {
                            foreach (var a in addItemBatchNos)
                            {
                                await _itemIssueVendorCreditItemBatchNoRepository.InsertAsync(a);
                            }
                        }

                        var updateItemBatchNos = oldItemBatchs.Where(s => c.ItemBatchNos.Any(r => r.Id == s.Id)).Select(s =>
                        {
                            var oldItemBatch = s;
                            var newBatch = c.ItemBatchNos.FirstOrDefault(r => r.Id == s.Id);

                            oldItemBatch.Update(userId, itemIssueItem.Id, newBatch.BatchNoId, newBatch.Qty);
                            return oldItemBatch;
                        }).ToList();

                        if (updateItemBatchNos.Any()) await _itemIssueVendorCreditItemBatchNoRepository.BulkUpdateAsync(updateItemBatchNos);

                        var deleteItemBatchNos = oldItemBatchs.Where(s => !c.ItemBatchNos.Any(r => r.Id == s.Id)).ToList();
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
                    var itemIssueItem = ItemIssueVendorCreditItem.Create(tenantId, userId, journal.ItemIssueVendorCredit.Id, c.VendorCreditItemId, c.ItemId,
                                                                 c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total,c.ItemReceiptPurhcaseItemId);
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
            if (input.Status == TransactionStatus.Publish && input.ReceiveFrom == ReceiveFrom.VendorCredit)
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
            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.ItemIssue, TransactionLockType.InventoryTransaction, };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            await SyncItemIssueVendorCredit(journal.ItemIssueVendorCredit.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, scheduleDate, scheduleItems);
            }

            return new NullableIdDto<Guid>() { Id = journal.ItemIssueVendorCredit.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_UpdateStatusToDraft,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Draft)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input)
        {
            var @entity = await _journalRepository
                               .GetAll()
                               .Include(u => u.ItemIssueVendorCredit)
                               .Where(u => u.JournalType == JournalType.ItemIssueVendorCredit && u.ItemIssueVendorCredit.Id == input.Id)
                               .FirstOrDefaultAsync();

            if (entity.ItemIssueVendorCredit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            //update customer credit status 
            var customerCredit = await _vendorCreditRepository
                                        .GetAll()
                                        .Where(u => u.Id == @entity.ItemIssueVendorCredit.VendorCreditId)
                                        .FirstOrDefaultAsync();
            customerCredit.UpdateShipedStatus(DeliveryStatus.ShipPending);
            CheckErrors(await _vendorCreditManager.UpdateAsync(customerCredit));

            entity.UpdateStatusToDraft();
            await CurrentUnitOfWork.SaveChangesAsync();
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_UpdateStatusToPublish,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Publish)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input)
        {
            var @entity = await _journalRepository
                                .GetAll()
                                .Include(u => u.ItemIssueVendorCredit)
                                .Where(u => u.JournalType == JournalType.ItemIssueVendorCredit && u.ItemIssueVendorCredit.Id == input.Id)
                                .FirstOrDefaultAsync();

            if (entity.ItemIssueVendorCredit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            //update customer credit status 
            var customerCredit = await _vendorCreditRepository
                                        .GetAll()
                                        .Where(u => u.Id == @entity.ItemIssueVendorCredit.VendorCreditId)
                                        .FirstOrDefaultAsync();
            customerCredit.UpdateShipedStatus(DeliveryStatus.ShipAll);
            CheckErrors(await _vendorCreditManager.UpdateAsync(customerCredit));

            entity.UpdatePublish();
            await CurrentUnitOfWork.SaveChangesAsync();
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_UpdateStatusToVoid,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Void)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input)
        {
            var @entity = await _journalRepository
                               .GetAll()
                               .Include(u => u.ItemIssueVendorCredit)
                               .Where(u => u.JournalType == JournalType.ItemIssueVendorCredit && u.ItemIssueVendorCredit.Id == input.Id)
                               .FirstOrDefaultAsync();

            if (entity.ItemIssueVendorCredit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            if (@entity.Status == TransactionStatus.Publish)
            {
                //update customer credit status 
                //var customerCredit = await _customerCreditRepository
                //                            .GetAll()
                //                            .Where(u => u.Id == @entity.ItemIssueVendorCreditId)
                //                            .FirstOrDefaultAsync();
                //customerCredit.UpdateShipedStatus(DeliveryStatus.ShipPending);
                //CheckErrors(await _customerCreditManager.UpdateAsync(customerCredit));
            }
            entity.UpdateVoid();
            await CurrentUnitOfWork.SaveChangesAsync();
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }



        #region Get ItemIssue Vendor Credit For Vendor Credit

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssues,AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssues_ForVendorCredit)]
        public async Task<PagedResultDto<ItemIssueVendorCreditItemFromVendorCreditOutput>> GetNewItemIssueForVendorCredit(GetItemIssueVendorCreditInput input)
        {
            var userGroups = await GetUserGroupByLocation();
            var @query = (from ivc in _itemIssueVendorCreditRepository.GetAll().AsNoTracking()
                          .Include(u => u.VendorCredit)
                          .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId))
                          .Where(u => u.VendorCreditId == null)
                          join ivci in _itemIssueVendorCreditItemRepository.GetAll()
                          .Include(u => u.Item)
                          .Include(u=>u.Lot)
                          .WhereIf(input.Lots != null && input.Lots.Count > 0, u => input.Lots.Contains(u.LotId.Value))
                          .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                          .Where(u => u.ItemId != null && u.VendorCreditItemId == null)
                          .AsNoTracking()
                          on ivc.Id equals ivci.ItemIssueVendorCreditId 
                          join j in _journalRepository.GetAll()
                          .Include(u => u.Currency)
                          .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                          .WhereIf(input.FromDate != null && input.ToDate != null,
                                (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                          .WhereIf(input.Locations != null && input.Locations.Count > 0, t => input.Locations.Contains(t.LocationId))
                          .WhereIf(!input.Filter.IsNullOrEmpty(),
                                u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) || u.Reference.ToLower().Contains(input.Filter.ToLower()))
                          .Where(u => u.Status == TransactionStatus.Publish)
                          .AsNoTracking() on ivc.Id equals j.ItemIssueVendorCreditId
                          select new ItemIssueVendorCreditItemFromVendorCreditOutput
                          {
                              Date = j.Date,
                              Id = ivci.Id,
                              ItemCode = ivci != null && ivci.Item != null ? ivci.Item.ItemCode : null,
                              ItemName = ivci != null && ivci.Item != null ? ivci.Item.ItemName : null,
                              ItemId = ivci != null && ivci != null ? ivci.ItemId : (Guid?)null,
                              ItemIssueVendorCreditId = ivc.Id,
                              LotId = ivci != null && ivci.Lot != null ? ivci.Lot.Id : (long?)null,
                              LotName = ivci != null && ivci.Lot != null ? ivci.Lot.LotName : null,
                              Qty = ivci.Qty,
                              IssueCreditNo = j.JournalNo,
                              CreationTimeIndex = j.CreationTimeIndex,
                              ItemDateTime = ivci.CreationTime,

                          });
            var resultCount = await query.CountAsync();

            var @entities = await query.OrderBy(t => t.Date).ThenBy(t=>t.ItemDateTime).PageBy(input).ToListAsync();
            return new PagedResultDto<ItemIssueVendorCreditItemFromVendorCreditOutput>(resultCount, @entities);



        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssues)]
        public async Task<PagedResultDto<ItemIssueVendorCreditSummaryOutput>> GetItemIssueForVendorCredit(GetItemIssueVendorCreditInput input)
        {
            var vendoTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            var userGroups = await GetUserGroupByLocation();

            var vendorQuery = GetVendors(null, input.Vendors, null, vendoTypeMemberIds);
            var currencyQuery = GetCurrencies();
            var locationQuery = GetLocations(null, input.Locations);

            var ivQuery = from iv in _itemIssueVendorCreditRepository.GetAll()
                                      .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId))
                                      .Where(u => u.VendorCreditId == null)
                                      .AsNoTracking()
                                      .Select(s => new
                                      {
                                          Id = s.Id,
                                          Total = s.Total,
                                          VendorId = s.VendorId
                                      })
                          join v in vendorQuery
                          on iv.VendorId equals v.Id
                          select new
                          {
                              Id = iv.Id,
                              Total = iv.Total,
                              VendorId = iv.VendorId,
                              VendorName = v.VendorName
                          };

            var iviQuery = _itemIssueVendorCreditItemRepository.GetAll()
                          .Include(u => u.Item)
                          .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                          .Where(u => u.ItemId != null && u.VendorCreditItemId == null)
                          .AsNoTracking()
                          .Select(s => s.ItemIssueVendorCreditId);

            var jQuery = from j in _journalRepository.GetAll()
                                  .WhereIf(input.FromDate != null && input.ToDate != null,
                                        (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                                  .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                                  .WhereIf(input.Locations != null && input.Locations.Count > 0, t => input.Locations.Contains(t.LocationId))
                                  .WhereIf(!input.Filter.IsNullOrEmpty(),
                                        u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) || u.Reference.ToLower().Contains(input.Filter.ToLower()))
                                  .Where(u => u.Status == TransactionStatus.Publish)
                                  .AsNoTracking()
                                  .Select(j => new
                                  {
                                      ItemIssueVendorCreditId = j.ItemIssueVendorCreditId,
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
                             ItemIssueVendorCreditId = j.ItemIssueVendorCreditId,
                             JournalNo = j.JournalNo,
                             Date = j.Date,
                             Reference = j.Reference,
                             CurrencyId = j.CurrencyId,
                             LoationId = j.LocationId,
                             LocationName = l.LocationName,
                             CurrencyCode = c.Code
                         };

            var query = from iv in ivQuery
                        join j in jQuery
                        on iv.Id equals j.ItemIssueVendorCreditId
                        join ivi in iviQuery
                        on iv.Id equals ivi
                        into ivs
                        where ivs.Count() > 0
                        select new ItemIssueVendorCreditSummaryOutput
                        {
                            LocationName = j.LocationName,
                            ItemIssueNo = j.JournalNo,
                            Vendor = new VendorSummaryOutput { 
                                Id = iv.VendorId,
                                VendorName = iv.VendorName
                            },
                            VendorId = iv.VendorId,
                            Id = iv.Id,
                            CurrencyId = j.CurrencyId,
                            Currency = new CurrencyDetailOutput { 
                                Id = j.CurrencyId,
                                Code = j.CurrencyCode
                            },
                            Date = j.Date,
                            Reference = j.Reference,
                            Total = iv.Total,
                            CountItems = ivs.Count(),
                        };

            var resultCount = await query.CountAsync();
            if (resultCount == 0) return new PagedResultDto<ItemIssueVendorCreditSummaryOutput>(resultCount, new List<ItemIssueVendorCreditSummaryOutput>());

            var @entities = await query.OrderByDescending(t => t.Date).PageBy(input).ToListAsync();
            return new PagedResultDto<ItemIssueVendorCreditSummaryOutput>(resultCount, @entities);



        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssues)]
        public async Task<PagedResultDto<ItemIssueVendorCreditSummaryOutput>> GetItemIssueForVendorCreditOld(GetItemIssueVendorCreditInput input)
        {
            var vendoTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            var userGroups = await GetUserGroupByLocation();
            var @query = (from b in _itemIssueVendorCreditRepository.GetAll().AsNoTracking().Include(u=>u.VendorCredit)
                          .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId))
                          .WhereIf(vendoTypeMemberIds.Any(), s => vendoTypeMemberIds.Contains(s.Vendor.VendorTypeId.Value))
                          .Where(u => u.VendorCreditId == null)
                          join bi in _itemIssueVendorCreditItemRepository.GetAll() 
                          .Include(u => u.Item)
                          .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                          .Where(u => u.ItemId != null && u.VendorCreditItemId == null)
                          .AsNoTracking() 
                          on b.Id equals bi.ItemIssueVendorCreditId into p
                          join j in _journalRepository.GetAll()
                          .Include(u => u.Currency)
                          .Include(u=>u.Location)
                          .WhereIf(input.FromDate != null && input.ToDate != null,
                                (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                          .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                          .WhereIf(input.Locations != null && input.Locations.Count > 0, t => input.Locations.Contains(t.LocationId))
                          .WhereIf(!input.Filter.IsNullOrEmpty(),
                                u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) || u.Reference.ToLower().Contains(input.Filter.ToLower()))
                          .Where(u => u.Status == TransactionStatus.Publish)
                          .AsNoTracking() on b.Id equals j.ItemIssueVendorCreditId
                          select new ItemIssueVendorCreditSummaryOutput
                          {
                              LocationName = j.Location != null ? j.Location.LocationName: null,
                              ItemIssueNo =  j.JournalNo,                              
                              Vendor = ObjectMapper.Map<VendorSummaryOutput>(b.Vendor),
                              VendorId = b.VendorId,                             
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
            return new PagedResultDto<ItemIssueVendorCreditSummaryOutput>(resultCount, @entities);

           

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssueItems)]
        public async Task<ItemIssueVendorCreditDetailOutput> GetItemIssueVendorItems(EntityDto<Guid> input)
        {
            var @journal = await _journalRepository
                               .GetAll()
                               .Include(u => u.ItemIssueVendorCredit)
                               .Include(u => u.Location)
                               .Include(u => u.Class)
                               .Include(u => u.Currency)
                               .Include(u => u.ItemIssueVendorCredit.Vendor.ChartOfAccount)
                               .Include(u => u.ItemIssueVendorCredit.ShippingAddress)
                               .Include(u => u.ItemIssueVendorCredit.BillingAddress)
                               .AsNoTracking()
                               .Where(u => u.JournalType == JournalType.ItemIssueVendorCredit && u.ItemIssueVendorCreditId == input.Id)
                               .FirstOrDefaultAsync();

            if (@journal == null || @journal.ItemIssueVendorCredit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var itemIssueItemCOGSs = await _itemIssueVendorCreditItemRepository.GetAll()
               .Include(u => u.Item)
               .Include(u => u.Lot)
               .Where(u => u.ItemIssueVendorCreditId == input.Id)
               .Join(
                   _journalItemRepository.GetAll()
                   .Where(u => u.Key == PostingKey.COGS)
                   .Include(u => u.Account)
                   .AsNoTracking()
                   ,
                   u => u.Id, s => s.Identifier,
                   (issueItem, jItem) =>
                   new CreateOrUpdateItemIssueVendorCreditItemInput()
                   {
                       Id = issueItem.Id,
                       PurchaseAccountId = jItem.AccountId,
                       PurchaseAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                   }).ToListAsync();

            var locations = new List<long?>() {
                journal.LocationId
            };          
            var itemIssueItems = await _itemIssueVendorCreditItemRepository.GetAll()
                .Include(u => u.Item.PurchaseAccount)
                .Include(u => u.Item.InventoryAccount)
                .Include(u => u.Lot)
                .Include(u => u.VendorCreditItem)
                .Where(u => u.ItemIssueVendorCreditId == input.Id)
                .Join(_journalItemRepository.GetAll().Include(u => u.Account).Where(u => u.Key == PostingKey.Inventory).AsNoTracking(), u => u.Id, s => s.Identifier,
                (rItem, jItem) =>
                new CreateOrUpdateItemIssueVendorCreditItemInput()
                {
                    FromLotDetail = ObjectMapper.Map<LotSummaryOutput>(rItem.Lot),
                    FromLotId = rItem.LotId,
                    Id = rItem.Id,
                    Item = ObjectMapper.Map<ItemSummaryDetailOutput>(rItem.Item),                    
                    ItemId = rItem.ItemId,
                    InventoryAccountId = jItem.AccountId,
                    InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                    PurchaseAccountId = itemIssueItemCOGSs.Where(r => r.Id == rItem.Id).Select(r => r.PurchaseAccountId).FirstOrDefault(),
                    PurchaseAccount = itemIssueItemCOGSs.Where(r => r.Id == rItem.Id).Select(r => r.PurchaseAccount).FirstOrDefault(),
                    Description = rItem.Description,
                    DiscountRate = rItem.DiscountRate,                  
                    Qty = rItem.Qty,
                    Total = rItem.Total,
                    UnitCost = rItem.UnitCost,
                    VendorCreditItemId = rItem.VendorCreditItemId,
                    UseBatchNo = rItem.Item.UseBatchNo,
                    TrackSerial = rItem.Item.TrackSerial,
                    TrackExpiration = rItem.Item.TrackExpiration,
                })
                .ToListAsync();


            var batchDic = await _itemIssueVendorCreditItemBatchNoRepository.GetAll()
                             .AsNoTracking()
                             .Where(s => s.ItemIssueVendorCreditItem.ItemIssueVendorCreditId == input.Id)
                             .Select(s => new BatchNoItemOutput
                             {
                                 Id = s.Id,
                                 BatchNoId = s.BatchNoId,
                                 BatchNumber = s.BatchNo.Code,
                                 ExpirationDate = s.BatchNo.ExpirationDate,
                                 Qty = s.Qty,
                                 TransactionItemId = s.ItemIssueVendorCreditItemId
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

            var result = ObjectMapper.Map<ItemIssueVendorCreditDetailOutput>(journal.ItemIssueVendorCredit);            
            result.Date = journal.Date;
            result.IssueNo = journal.JournalNo;
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
