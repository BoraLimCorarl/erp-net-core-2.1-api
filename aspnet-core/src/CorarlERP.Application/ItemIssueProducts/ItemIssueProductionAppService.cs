using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.AccountCycles;
using CorarlERP.Authorization;
using CorarlERP.AutoSequences;
using CorarlERP.BatchNos;
using CorarlERP.ChartOfAccounts;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.Features;
using CorarlERP.Inventories;
using CorarlERP.InventoryCalculationItems;
using CorarlERP.InventoryCalculationJobSchedules;
using CorarlERP.InventoryTransactionItems;
using CorarlERP.ItemIssueProducts.Dto;
using CorarlERP.ItemIssues;
using CorarlERP.ItemIssues.Dto;
using CorarlERP.ItemReceipts;
using CorarlERP.Items;
using CorarlERP.Items.Dto;
using CorarlERP.Journals;
using CorarlERP.JournalTransactionTypes;
using CorarlERP.Locations.Dto;
using CorarlERP.Locks;
using CorarlERP.Lots;
using CorarlERP.Lots.Dto;
using CorarlERP.Productions;
using CorarlERP.Taxes.Dto;
using Hangfire.States;
using Microsoft.EntityFrameworkCore;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ItemIssueProducts
{
    [AbpAuthorize]
    public class ItemIssueProductionAppService : CorarlERPAppServiceBase, IItemIssueProductionAppService
    {
        private readonly IRepository<Lot, long> _lotRepository;
        private readonly IInventoryManager _inventoryManager;
        private readonly IRepository<ItemReceiptItem, Guid> _itemReceiptItemRepository;
      
        private readonly IItemManager _itemManager;
        private readonly IRepository<Item, Guid> _itemRepository;

        private readonly IItemIssueItemManager _itemIssueItemManager;
        private readonly IItemIssueManager _itemIssueManager;

        private readonly IRepository<ItemIssue, Guid> _itemIssueRepository;
        private readonly IRepository<ItemIssueItem, Guid> _itemIssueItemRepository;

        private readonly IJournalManager _journalManager;
        private readonly IRepository<Journal, Guid> _journalRepository;

        private readonly IJournalItemManager _journalItemManager;
        private readonly IRepository<JournalItem, Guid> _journalItemRepository;

        private readonly IChartOfAccountManager _chartOfAccountManager;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;

        private readonly IProductionManager _productionOrderManager;
        private readonly IRepository<Production, Guid> _productionOrderRepository;
        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly IRepository<AccountCycle,long> _accountCyclesRepository;
        private readonly IRepository<Lock, long> _lockRepository;
        private readonly ICorarlRepository<InventoryTransactionItem, Guid> _inventoryTransactionItemRepository;
        private readonly IInventoryCalculationItemManager _inventoryCalculationItemManager;
        private readonly ICorarlRepository<BatchNo, Guid> _batchNoRepository;
        private readonly ICorarlRepository<ItemIssueItemBatchNo, Guid> _itemIssueItemBatchNoRepository;
        private readonly IJournalTransactionTypeManager _journalTransactionTypeManager;
        public ItemIssueProductionAppService(
            ICorarlRepository<InventoryTransactionItem, Guid> inventoryTransactionItemRepository,
            IJournalManager journalManager,
            IJournalItemManager journalItemManager,
            IChartOfAccountManager chartOfAccountManager,
            ItemIssueManager itemIssueManager,
            ItemIssueItemManager itemIssueItemManager,
            ItemManager itemManager,
            IRepository<Item, Guid> itemRepository,
            IRepository<Journal, Guid> journalRepository,
            IRepository<JournalItem, Guid> journalItemRepository,
            IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
            IRepository<ItemIssue, Guid> itemIssueRepository,
            IRepository<ItemIssueItem, Guid> itemIssueItemRepository,
            IRepository<ItemReceiptItem, Guid> itemReceiptItemRepository,           
            IProductionManager productionOrderManager,
            IRepository<Production, Guid> productionOrderRepository,
            IInventoryManager inventoryManager,
            IRepository<AutoSequence, Guid> autoSequenceRepository,
            IAutoSequenceManager autoSequenceManager, 
            IRepository<Lot, long> lotRepository,
            IRepository<AccountCycle, long> accountCyclesRepository,
            IInventoryCalculationItemManager inventoryCalculationItemManager,
            ICorarlRepository<BatchNo, Guid> batchNoRepository,
            ICorarlRepository<ItemIssueItemBatchNo, Guid> itemIssueItemBatchNoRepository,
            IRepository<Lock, long> lockRepository,
            IJournalTransactionTypeManager journalTransactionTypeManager
        ) :base(accountCyclesRepository, null,null)
        {
            _lotRepository = lotRepository;
            _journalManager = journalManager;
            _journalManager.SetJournalType(JournalType.ItemIssueProduction);
            _journalRepository = journalRepository;
            _journalItemManager = journalItemManager;
            _journalItemRepository = journalItemRepository;
            _chartOfAccountManager = chartOfAccountManager;
            _chartOfAccountRepository = chartOfAccountRepository;
            _itemIssueItemManager = itemIssueItemManager;
            _itemIssueItemRepository = itemIssueItemRepository;
            _itemIssueManager = itemIssueManager;
            _itemIssueRepository = itemIssueRepository;
            _itemRepository = itemRepository;
            _itemManager = itemManager;
            _itemReceiptItemRepository = itemReceiptItemRepository;           
            _productionOrderManager = productionOrderManager;
            _productionOrderRepository = productionOrderRepository;
            _inventoryManager = inventoryManager;
            _autoSequenceManager = autoSequenceManager;
            _autoSequenceRepository = autoSequenceRepository;
            _accountCyclesRepository = accountCyclesRepository;
            _lockRepository = lockRepository;
            _inventoryTransactionItemRepository = inventoryTransactionItemRepository;
            _inventoryCalculationItemManager = inventoryCalculationItemManager;
            _batchNoRepository = batchNoRepository;
            _itemIssueItemBatchNoRepository = itemIssueItemBatchNoRepository;
            _journalTransactionTypeManager = journalTransactionTypeManager;
        }

        private async Task ValidateBatchNo(CreateItemIssueProductInput input)
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
            if (input is UpdateItemIssueProductInput)
            {
                exceptIds.Add((input as UpdateItemIssueProductInput).Id);
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

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_CanCreateProductionOrder,
            AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueProduction)]
        public async Task<NullableIdDto<Guid>> Create(CreateItemIssueProductInput input)
        {
            if(input.IsConfirm == false)
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
           
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);

            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.IssueNo = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }
            var countItemIssueItems = (from ItemIssueitem in input.Items
                                       where (ItemIssueitem.RawMaterialId != null)
                                       select ItemIssueitem.RawMaterialId).Count();
            if (input.ReceiveFrom == ReceiveFrom.ProductionOrder && countItemIssueItems <= 0 && input.ProductionId.Value == null)
            {
                throw new UserFriendlyException(L("PleaseAddProductionOrder"));
            }
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            //insert to journal
            var @entity = Journal.Create(tenantId, userId, input.IssueNo, input.Date,
                            input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference,input.LocationId);
            entity.UpdateStatus(input.Status);
            #region journal transaction type 
            var transactionTypeId = await _journalTransactionTypeManager.GetJournalTransactionTypeId(InventoryTransactionType.ItemIssueProduction);
            entity.SetJournalTransactionTypeId(transactionTypeId);
            #endregion
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

            //insert clearance journal item into credit
            var clearanceJournalItem = JournalItem.CreateJournalItem(
                                        tenantId, userId, entity, input.ClearanceAccountId, input.Memo,
                                        input.Total, 0, PostingKey.COGS, null);

            //insert to item Issue
            var itemIssue = ItemIssue.Create(tenantId, userId, input.ReceiveFrom, null,
                                input.SameAsShippingAddress, input.ShippingAddress, 
                                input.BillingAddress,  input.Total, input.ProductionProcessId);

            itemIssue.UpdateTransactionType(InventoryTransactionType.ItemIssueProduction);
            @entity.UpdateIssueProduction(itemIssue);
            @entity.UpdateCreditDebit(input.Total, input.Total);

            if (input.ReceiveFrom == ReceiveFrom.ProductionOrder)
            {
                var @Production = await _productionOrderRepository.GetAll().Where(u => u.Id == input.ProductionId).FirstOrDefaultAsync();
                // update received status 
                if (@Production == null)
                {
                    throw new UserFriendlyException(L("NoProductionOrder"));
                }
                if (input.Status == TransactionStatus.Publish)
                {
                    @Production.UpdateShipedStatus(TransferStatus.ShipAll);
                }
                CheckErrors(await _productionOrderManager.UpdateAsync(@Production));

                itemIssue.UpdateProductionOrderId(input.ProductionId.Value);
            }

            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));
            CheckErrors(await _journalItemManager.CreateAsync(clearanceJournalItem));
            CheckErrors(await _itemIssueManager.CreateAsync(itemIssue));

            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }

          
            foreach (var i in input.Items)
            {               
                //insert to item Issue item
                var itemIssueItem = ItemIssueItem.Create(tenantId, userId, itemIssue, i.ItemId,
                                    i.Description, i.Qty, i.UnitCost, i.DiscountRate, i.Total);
                if (i.RawMaterialId != null)
                {
                    itemIssueItem.UpdateRawmatailItemId(i.RawMaterialId.Value);
                }
                itemIssueItem.UpdateLot(i.FromLotId);
                CheckErrors(await _itemIssueItemManager.CreateAsync(itemIssueItem));
                //insert inventory journal item into debit
                var inventoryJournalItem = JournalItem.CreateJournalItem(
                                        tenantId, userId, entity, i.InventoryAccountId,
                                        i.Description, 0, i.Total, PostingKey.Inventory, itemIssueItem.Id);
                CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));

                if (i.ItemBatchNos != null && i.ItemBatchNos.Any())
                {
                    var addItemBatchNos = i.ItemBatchNos.Select(s => ItemIssueItemBatchNo.Create(tenantId, userId, itemIssueItem.Id, s.BatchNoId, s.Qty)).ToList();
                    foreach (var a in addItemBatchNos)
                    {
                        await _itemIssueItemBatchNoRepository.InsertAsync(a);
                    }
                }
            }
            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.ItemIssue, TransactionLockType.InventoryTransaction, };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            await SyncItemIssue(itemIssue.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                var scheduleItems = input.Items.Select(s => s.ItemId).Distinct().ToList();
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, input.Date, scheduleItems);
            }

            return new NullableIdDto<Guid>() { Id = itemIssue.Id };
        }

        //[AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_Delete)]
        //public async Task Delete(EntityDto<Guid> input)
        //{
        //    var @jounal = await _journalRepository.GetAll()
        //        .Include(u => u.ItemIssue)
        //        .Include(u => u.ItemIssue.ShippingAddress)
        //        .Include(u => u.ItemIssue.BillingAddress)
        //        .Where(u => u.JournalType == JournalType.ItemIssueProduction && u.ItemIssueId == input.Id)
        //        .FirstOrDefaultAsync();

        //    //query get item Issue 
        //    var @entity = @jounal.ItemIssue;
        //    if (entity == null)
        //    {
        //        throw new UserFriendlyException(L("RecordNotFound"));
        //    }
        //    @jounal.UpdateIssueProduction(null);
        //    //update receive status of Production order to pending 
        //    if (jounal.JournalType == JournalType.ItemIssueProduction)
        //    {
        //        var @Production = await _productionOrderRepository.GetAll().Where(u => u.Id == jounal.ItemIssue.ProductionOrderId).FirstOrDefaultAsync();
        //        // update received status 
        //        if (@Production != null)
        //        {
        //            @Production.UpdateShipedStatus(TransferStatus.Pending);
        //            CheckErrors(await _productionOrderManager.UpdateAsync(@Production));
        //        }
        //    }

        //    //query get journal item and delete
        //    var @jounalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == jounal.Id).ToListAsync();
        //    foreach (var ji in jounalItems)
        //    {
        //        CheckErrors(await _journalItemManager.RemoveAsync(ji));
        //    }

        //    CheckErrors(await _journalManager.RemoveAsync(@jounal));

        //    //query get item Issue item and delete 
        //    var itemIssueItems = await _itemIssueItemRepository.GetAll()
        //        .Where(u => u.ItemIssueId == entity.Id).ToListAsync();

        //    foreach (var iri in itemIssueItems)
        //    {

        //        CheckErrors(await _itemIssueItemManager.RemoveAsync(iri));
        //    }

        //    CheckErrors(await _itemIssueManager.RemoveAsync(entity));
        //}

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_GetDetail,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ViewDetail)]
        public async Task<ItemIssueProductDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            await TurnOffTenantFilterIfDebug();

            var @journal = await _journalRepository
                               .GetAll()
                               .Include(u => u.ItemIssue)
                               .Include(u => u.Location)
                               .Include(u => u.ItemIssue.ProductionProcess)
                               .Include(u => u.Class)
                               .Include(u => u.Currency)
                               .Include(u => u.ItemIssue.ShippingAddress)
                               .Include(u => u.ItemIssue.BillingAddress)
                               .Include(u=>u.JournalTransactionType)
                               .AsNoTracking()
                               .Where(u => u.JournalType == JournalType.ItemIssueProduction && u.ItemIssueId == input.Id)
                               .FirstOrDefaultAsync();

            if (@journal == null || @journal.ItemIssue == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var clearanceAccount = await(_journalItemRepository.GetAll()
                                       .Include(u => u.Account)
                                       .AsNoTracking()
                                       .Where(u => u.JournalId == journal.Id &&
                                                u.Key == PostingKey.COGS && u.Identifier == null)
                                       .Select(u => u.Account)).FirstOrDefaultAsync();

            var locations = new List<long?>() {
                journal.LocationId
            };

            //var averageCosts = await _inventoryManager.GetAvgCostQuery(@journal.Date.Date, locations).ToListAsync();


            var itemIssueItems = await (from rItem in _itemIssueItemRepository.GetAll()
                                                    .Include(u => u.Item)
                                                    .Include(u => u.Lot)
                                                    .Include(u => u.RawMaterialItem)
                                                    .Include(u => u.RawMaterialItem.Production)
                                                    .Where(u => u.ItemIssueId == input.Id)
                                                    .AsNoTracking()
                                        join jItem in _journalItemRepository.GetAll()
                                                      .Include(u => u.Account)
                                                      .Where(s => s.Key == PostingKey.Inventory)
                                                      .AsNoTracking()
                                        on rItem.Id equals jItem.Identifier
                                        join i in _inventoryTransactionItemRepository.GetAll()
                                                  .Where(s => s.JournalType == JournalType.ItemIssueProduction)
                                                  .Where(s => s.TransferOrProductionId.HasValue && s.TransferOrProductionItemId.HasValue)
                                                  .Where(s => s.TransactionId == input.Id)
                                                  .AsNoTracking()
                                        on rItem.Id equals i.Id
                                        into cs from c in cs.DefaultIfEmpty()
                                        select
                                            new ItemIssueItemProductDetailOutput()
                                            {
                                                FromLotId = rItem.LotId,
                                                FromLotDetial = ObjectMapper.Map<LotSummaryOutput>(rItem.Lot),
                                                Id = rItem.Id,
                                                Item = _itemRepository.GetAll().Where(r => r.Id == rItem.ItemId)
                                                        .Select(i => new ItemSummaryDetailOutput
                                                        {
                                                            InventoryAccount = ObjectMapper.Map<ChartAccountDetailOutput>(i.InventoryAccount),
                                                            Id = i.Id,
                                                            InventoryAccountId = i.InventoryAccountId,
                                                            ItemCode = i.ItemCode,
                                                            ItemName = i.ItemName,
                                                            PurchaseAccount = ObjectMapper.Map<ChartAccountDetailOutput>(i.PurchaseAccount),
                                                            PurchaseAccountId = i.PurchaseAccountId,
                                                            SaleAccount = ObjectMapper.Map<ChartAccountDetailOutput>(i.SaleAccount),
                                                            SaleAccountId = i.SaleAccountId,
                                                            SalePrice = i.SalePrice,
                                                            SaleTax = ObjectMapper.Map<TaxDetailOutput>(i.SaleTax),
                                                            SaleTaxId = i.SaleTaxId,
                                                            //AverageCost = averageCosts.Where(v => v.Id == rItem.ItemId).Select(c => c.AverageCost).FirstOrDefault(),
                                                            //QtyOnHand = averageCosts.Where(v => v.Id == rItem.ItemId).Select(v => v.QtyOnHand).FirstOrDefault(),
                                                        }).FirstOrDefault(),
                                                //ObjectMapper.Map<ItemSummaryOutput>(rItem.Item),
                                                ItemId = rItem.ItemId,
                                                InventoryAccountId = jItem.AccountId,
                                                InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                                                Description = rItem.Description,
                                                DiscountRate = rItem.DiscountRate,
                                                Qty = rItem.Qty,
                                                Total = c != null ? Math.Abs(c.LineCost) : rItem.Total,
                                                UnitCost = c != null ? c.UnitCost : rItem.UnitCost,
                                                RawMaterialId = rItem.RawMaterialItemId,
                                                ProductionNo = rItem.RawMaterialItem.Production.ProductionNo,
                                                UseBatchNo = rItem.Item.UseBatchNo,
                                                TrackSerial = rItem.Item.TrackSerial,
                                                TrackExpiration = rItem.Item.TrackExpiration
                                            })
                                            .ToListAsync();

            var batchDic = await _itemIssueItemBatchNoRepository.GetAll()
                               .AsNoTracking()
                               .Where(s => s.ItemIssueItem.ItemIssueId == input.Id)
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
                foreach (var i in itemIssueItems)
                {
                    if (batchDic.ContainsKey(i.Id)) i.ItemBatchNos = batchDic[i.Id];
                }
            }


            var result = ObjectMapper.Map<ItemIssueProductDetailOutput>(journal.ItemIssue);
            result.Date = journal.Date;
            result.ReceiveNo = journal.JournalNo;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.Reference = journal.Reference;
            result.ItemIssueItemProductions = itemIssueItems;
            result.ClearanceAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(clearanceAccount);
            result.ClearanceAccountId = clearanceAccount.Id;
            result.Memo = journal.Memo;
            result.StatusCode = journal.Status;
            //result.Total = journal.Credit;
            result.StatusName = journal.Status.ToString();
            result.LocationId = journal.LocationId.Value;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);

            //get total from inventory transaction item cache table
            result.Total = itemIssueItems.Sum(s => s.Total);
            result.TransactionTypeName = journal.JournalTransactionType?.Name;

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_Update,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Update,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour)]
        public async Task<NullableIdDto<Guid>> Update(UpdateItemIssueProductInput input)
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
                              .Include(t => t.ItemIssue)
                              .Include(t => t.ItemIssue.ProductionOrder)
                              .Where(u => u.JournalType == JournalType.ItemIssueProduction && u.ItemIssueId == input.Id)
                              .FirstOrDefaultAsync();

            if (@journal == null || @journal.ItemIssue == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

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

            await CheckClosePeriod(journal.Date,input.Date);

            var scheduleDate = input.Date > journal.Date ? journal.Date : input.Date;

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

            //update Clearance account 
            var @clearanceAccountItem = await (_journalItemRepository.GetAll().Where(u => 
                                                u.JournalId == journal.Id &&
                                                u.Key == PostingKey.COGS && u.Identifier == null)
                                        ).FirstOrDefaultAsync();
            clearanceAccountItem.UpdateJournalItem(tenantId, input.ClearanceAccountId, input.Memo, input.Total,0);

            //update item Issue 
            var itemIssue = @journal.ItemIssue;            
            var getPermission = await GetUserPermissions(userId);
            var IsEdit = getPermission.Where(t => t == AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour).Count();
            var totalHours = (DateTime.Now - itemIssue.CreationTime).TotalHours;
            if (IsEdit > 0 && totalHours > 48)
            {
                throw new UserFriendlyException(L("ThisTransactionIsOver48HoursCanNotEdit"));
            }
            
            if (itemIssue.ProductionOrderId != null && itemIssue.ProductionOrder != null && itemIssue.ProductionOrder.ConvertToIssueAndReceipt == true)
            {
                throw new UserFriendlyException(L("AutoConvertMessage"));
            }
            //else if (itemIssue.ProductionOrderId != null && itemIssue.ProductionOrder != null && itemIssue.ProductionOrder.ShipedStatus == TransferStatus.ReceiveAll)
            //{
            //    //throw if item issue receive from transfer order than also already item receipt too 
            //    throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            //}

            itemIssue.Update(tenantId, input.ReceiveFrom, null,
                          input.SameAsShippingAddress, input.ShippingAddress,
                          input.BillingAddress, input.Total, input.ProductionProcessId);

            #region update receive from 
            // update if come from Production order 
            if (input.ReceiveFrom == ReceiveFrom.ProductionOrder && input.ProductionId != null)
            {
                if (itemIssue.ProductionOrderId != null && itemIssue.ProductionOrderId != input.ProductionId)
                {
                    var @ProductionOld = await _productionOrderRepository.GetAll().Where(u => u.Id == itemIssue.ProductionOrderId).FirstOrDefaultAsync();
                    CheckErrors(await _productionOrderManager.UpdateAsync(@ProductionOld));
                }

                var @Production = await _productionOrderRepository.GetAll().Where(u => u.Id == input.ProductionId).FirstOrDefaultAsync();
                // update received status 
                if (@Production == null)
                {
                    throw new UserFriendlyException(L("NoProductionOrder"));
                }
                if (input.Status == TransactionStatus.Publish)
                {
                    @Production.UpdateShipedStatus(TransferStatus.ShipAll);
                }
                CheckErrors(await _productionOrderManager.UpdateAsync(@Production));
                itemIssue.UpdateProductionOrderId(input.ProductionId.Value);
                
            }
            else // in some case if user switch from receive from Production order to other so update Production of field item issue transfer order id to null
            {
                var @Production = await _productionOrderRepository.GetAll().Where(u => u.Id == itemIssue.ProductionOrderId).FirstOrDefaultAsync();
                if (@Production != null)
                {
                    @Production.UpdateShipedStatus(TransferStatus.Pending);
                    CheckErrors(await _productionOrderManager.UpdateAsync(@Production));
                }
                itemIssue.UpdateProductionOrderId(null);
            }
            #endregion

            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);
            CheckErrors(await _journalManager.UpdateAsync(@journal, autoSequence.DocumentType));

            CheckErrors(await _journalItemManager.UpdateAsync(@clearanceAccountItem));
            CheckErrors(await _itemIssueManager.UpdateAsync(itemIssue));


            //Update Item Issue Item and Journal Item
            var itemIssueItems = await _itemIssueItemRepository.GetAll().Where(u => u.ItemIssueId == input.Id).ToListAsync();

            var @inventoryJournalItems = await(_journalItemRepository.GetAll()
                                  .Where(u => u.JournalId == journal.Id &&
                                           u.Key == PostingKey.Inventory && u.Identifier != null)).ToListAsync();

            var toDeleteItemIssueItem = itemIssueItems
                                        .Where(u => !input.Items.Any(i => i.Id != null && i.Id == u.Id))
                                        .ToList();
            var toDeletejournalItem = inventoryJournalItems
                                        .Where(u => !input.Items.Any(i => u.Identifier != null && i.Id != null && i.Id == u.Identifier))
                                        .ToList();

            var itemBatchNos = await _itemIssueItemBatchNoRepository.GetAll().Where(s => itemIssueItems.Any(r => r.Id == s.ItemIssueItemId)).AsNoTracking().ToListAsync();
            if (toDeleteItemIssueItem.Any())
            {
                var deleteItemBatchNos = itemBatchNos.Where(s => toDeleteItemIssueItem.Any(r => r.Id == s.ItemIssueItemId)).ToList();
                if (deleteItemBatchNos.Any()) await _itemIssueItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
            }

            foreach (var c in input.Items)
            {               

                if (c.Id != null) //update
                {
                    var itemIssueItem = itemIssueItems.FirstOrDefault(u => u.Id == c.Id);
                    var journalItem = @inventoryJournalItems.FirstOrDefault(u => u.Identifier == c.Id);
                    if (itemIssueItem != null)
                    {
                        //new
                        itemIssueItem.Update(tenantId, c.ItemId, c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total);
                        itemIssueItem.UpdateLot(c.FromLotId);
                        CheckErrors(await _itemIssueItemManager.UpdateAsync(itemIssueItem));

                    }
                    if (journalItem != null)
                    {
                        journalItem.UpdateJournalItem(userId, c.InventoryAccountId, c.Description, 0, c.Total);
                        CheckErrors(await _journalItemManager.UpdateAsync(journalItem));
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
                    //insert to item Issue item
                    var itemIssueItem = ItemIssueItem.Create(tenantId, userId, itemIssue, c.ItemId,
                                                                 c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total);
                    itemIssueItem.UpdateLot(c.FromLotId);
                    if (c.RawMaterialId != null)
                    {
                        itemIssueItem.UpdateRawmatailItemId(c.RawMaterialId.Value);
                    }
                    itemIssueItem.UpdateLot(c.FromLotId);
                    CheckErrors(await _itemIssueItemManager.CreateAsync(itemIssueItem));

                    var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal,
                        c.InventoryAccountId, c.Description, 0, c.Total, PostingKey.Inventory, itemIssueItem.Id);
                    CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));

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

            foreach (var t in toDeleteItemIssueItem.OrderBy(u => u.Id))
            {
                CheckErrors(await _itemIssueItemManager.RemoveAsync(t));
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

            await SyncItemIssue(itemIssue.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, scheduleDate, scheduleItems);
            }

            return new NullableIdDto<Guid>() { Id = itemIssue.Id };
        }


    }
}
