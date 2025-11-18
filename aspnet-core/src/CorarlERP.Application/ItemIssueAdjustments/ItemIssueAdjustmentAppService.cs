using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Addresses;
using CorarlERP.Authorization;
using CorarlERP.AutoSequences;
using CorarlERP.ChartOfAccounts;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.Dto;
using CorarlERP.Inventories;
using CorarlERP.ItemIssueAdjustments.Dto;
using CorarlERP.ItemIssueOthers.Dto;
using CorarlERP.ItemIssues;
using CorarlERP.ItemIssueVendorCredits;
using CorarlERP.ItemReceiptCustomerCredits;
using CorarlERP.ItemReceipts;
using CorarlERP.ItemReceipts.Dto;
using CorarlERP.Items;
using CorarlERP.Items.Dto;
using CorarlERP.Journals;
using CorarlERP.Locations;
using CorarlERP.Locations.Dto;
using CorarlERP.Locks;
using CorarlERP.Lots;
using CorarlERP.Lots.Dto;
using CorarlERP.Reports;
using CorarlERP.Reports.Dto;
using CorarlERP.ReportTemplates;
using CorarlERP.Taxes.Dto;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.FileStorages;
using CorarlERP.InventoryTransactionItems;
using Hangfire.States;
using CorarlERP.InventoryCalculationJobSchedules;
using CorarlERP.Migrations;
using CorarlERP.MultiTenancy;
using Abp.Domain.Uow;
using System.Transactions;
using TransactionStatus = CorarlERP.enumStatus.EnumStatus.TransactionStatus;
using CorarlERP.Inventories.Data;
using CorarlERP.InventoryCalculationItems;
using CorarlERP.BatchNos;
using Abp.Extensions;
using System.Diagnostics;
using CorarlERP.JournalTransactionTypes;
using CorarlERP.Features;

namespace CorarlERP.ItemIssueAdjustments
{
    [AbpAuthorize]
    public class ItemIssueAdjustmentAppService : ReportBaseClass, IItemIssueAdjustmentAppService
    {
        private readonly IInventoryManager _inventoryManager;
        private readonly IRepository<ItemReceiptItem, Guid> _itemReceiptItemRepository;
        private readonly IRepository<ItemReceiptItemCustomerCredit, Guid> _itemReceiptCustomerCreditItemRepository;
        private readonly IRepository<ItemIssueVendorCreditItem, Guid> _itemIssueVendorCreditItemRepository;
        private readonly IReportAppService _reportAppManger;
        private readonly IItemManager _itemManager;
        private readonly IRepository<Lot, long> _lotRepository;
        private readonly IRepository<Item, Guid> _itemRepository;

        private readonly IItemIssueItemManager _itemIssueItemManager;
        private readonly IItemIssueManager _itemIssueManager;

        private readonly ICorarlRepository<ItemIssue, Guid> _itemIssueRepository;
        private readonly ICorarlRepository<ItemIssueItem, Guid> _itemIssueItemRepository;

        private readonly IJournalManager _journalManager;
        private readonly ICorarlRepository<Journal, Guid> _journalRepository;

        private readonly IJournalItemManager _journalItemManager;
        private readonly ICorarlRepository<JournalItem, Guid> _journalItemRepository;

        private readonly IChartOfAccountManager _chartOfAccountManager;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;
        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;

        private readonly AppFolders _appFolders;
        private readonly IRepository<Location, long> _locationRepository;
        private readonly IRepository<Class, long> _classRepository;

        private readonly IRepository<Lock, long> _lockRepository;
        private readonly IFileStorageManager _fileStorageManager;
        private readonly ICorarlRepository<InventoryTransactionItem, Guid> _inventoryTransactionItemRepository;
        private readonly IInventoryCalculationItemManager _inventoryCalculationItemManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICorarlRepository<BatchNo, Guid> _batchNoRepository;
        private readonly ICorarlRepository<ItemIssueItemBatchNo, Guid> _itemIssueItemBatchNoRepository;
        private readonly IJournalTransactionTypeManager _journalTransactionTypeManager;
        public ItemIssueAdjustmentAppService(
            ICorarlRepository<InventoryTransactionItem, Guid> inventoryTransactionItemRepository,
           IJournalManager journalManager,
           IJournalItemManager journalItemManager,
           IChartOfAccountManager chartOfAccountManager,
           IUnitOfWorkManager unitOfWorkManager,
           ItemIssueManager itemIssueManager,
           ItemIssueItemManager itemIssueItemManager,
           ItemManager itemManager,
           IRepository<Item, Guid> itemRepository,
           ICorarlRepository<Journal, Guid> journalRepository,
           ICorarlRepository<JournalItem, Guid> journalItemRepository,
           IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
           ICorarlRepository<ItemIssue, Guid> itemIssueRepository,
           ICorarlRepository<ItemIssueItem, Guid> itemIssueItemRepository,
           IRepository<ItemReceiptItem, Guid> itemReceiptItemRepository,
           IRepository<ItemReceiptItemCustomerCredit, Guid> itemReceiptCustomerCreditItemRepository,
           IRepository<ItemIssueVendorCreditItem, Guid> itemIssueVendorCreditItemRepository,
           IInventoryCalculationItemManager inventoryCalculationItemManager,
           IInventoryManager inventoryManager,
           IRepository<AutoSequence, Guid> autoSequenceRepository,
           ICorarlRepository<BatchNo, Guid> batchNoRepository,
           ICorarlRepository<ItemIssueItemBatchNo, Guid> itemIssueItemBatchNoRepository,
           IAutoSequenceManager autoSequenceManager,
           IRepository<Lot, long> lotRepository,
           AppFolders appFolders,
           IFileStorageManager fileStorageManager,
           IRepository<Location, long> locationRepository,
           IRepository<Class, long> classRepository,
           IRepository<Lock, long> lockRepository,
           IReportAppService reportAppManger,
           IJournalTransactionTypeManager journalTransactionTypeManager,
           IRepository<AccountCycles.AccountCycle, long> accountCycleRepository
        ) : base(accountCycleRepository, appFolders, null, null)
        {
            _appFolders = appFolders;
            _fileStorageManager = fileStorageManager;
            _reportAppManger = reportAppManger;
            _locationRepository = locationRepository;
            _classRepository = classRepository;
            _lotRepository = lotRepository;
            _journalManager = journalManager;
            _journalManager.SetJournalType(JournalType.ItemIssueAdjustment);
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
            _itemIssueVendorCreditItemRepository = itemIssueVendorCreditItemRepository;
            _itemReceiptCustomerCreditItemRepository = itemReceiptCustomerCreditItemRepository;
            _inventoryCalculationItemManager = inventoryCalculationItemManager;
            _inventoryManager = inventoryManager;
            _autoSequenceManager = autoSequenceManager;
            _autoSequenceRepository = autoSequenceRepository;

            _appFolders = appFolders;
            _locationRepository = locationRepository;
            _classRepository = classRepository;
            _lockRepository = lockRepository;
            _inventoryTransactionItemRepository = inventoryTransactionItemRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _batchNoRepository = batchNoRepository;
            _itemIssueItemBatchNoRepository = itemIssueItemBatchNoRepository;
            _journalTransactionTypeManager = journalTransactionTypeManager;
        }

        private async Task ValidateBatchNo(CreateItemIssueAdjustmentInput input)
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
            if (input is UpdateItemIssueAdjustmentInput)
            {
                exceptIds.Add((input as UpdateItemIssueAdjustmentInput).Id);
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



        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_CanCreateAdjustment,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueAdjustment)]
        public async Task<NullableIdDto<Guid>> Create(CreateItemIssueAdjustmentInput input)
        {
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

            await ValidateBatchNo(input);

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);

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
            var transactionTypeId = await _journalTransactionTypeManager.GetJournalTransactionTypeId(InventoryTransactionType.ItemIssueAdjustment);
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
                LotId = u.FromLotId,
                Index = index,
                ItemId = u.ItemId,
                ItemCode = u.Item.ItemCode,
                Qty = u.Qty,
                ItemIssueItemId = u.Id,
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
            var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, input.ClearanceAccountId, input.Memo,
                input.Total, 0, PostingKey.COGS, null);

            //insert to item Issue
            var itemIssue = ItemIssue.Create(tenantId, userId, ReceiveFrom.None, null,
                input.SameAsShippingAddress, input.ShippingAddress, input.BillingAddress,
                input.Total, null);

            itemIssue.UpdateTransactionType(InventoryTransactionType.ItemIssueAdjustment);
            @entity.UpdateIssueAdjustment(itemIssue);
            @entity.UpdateCreditDebit(input.Total, input.Total);

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
                itemIssueItem.UpdateLot(i.FromLotId);
                CheckErrors(await _itemIssueItemManager.CreateAsync(itemIssueItem));
                //insert inventory journal item into debit
                var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, i.InventoryAccountId,
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

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_Delete,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Delete)]


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_GetDetail,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ViewDetail)]
        public async Task<ItemIssueAdjustmentDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            await TurnOffTenantFilterIfDebug();

            var @journal = await _journalRepository
                             .GetAll()
                             .Include(u => u.ItemIssue)
                             .Include(u => u.Location)
                             .Include(u => u.Class)
                             .Include(u => u.Currency)
                             .Include(u => u.ItemIssue.ShippingAddress)
                             .Include(u => u.ItemIssue.BillingAddress)
                             .Include(u=>u.JournalTransactionType)
                             .AsNoTracking()
                             .Where(u => u.JournalType == JournalType.ItemIssueAdjustment && u.ItemIssueId == input.Id)
                             .FirstOrDefaultAsync();

            if (@journal == null || @journal.ItemIssue == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var clearanceAccount = await (_journalItemRepository.GetAll()
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
                                                     .Where(u => u.ItemIssueId == input.Id)
                                                     .AsNoTracking()
                                        join jItem in _journalItemRepository.GetAll()
                                                    .Include(u => u.Account)
                                                    .Where(s => s.Identifier.HasValue)
                                                    .Where(s => s.Key == PostingKey.Inventory)
                                                    .AsNoTracking()
                                        on rItem.Id equals jItem.Identifier
                                        join ii in _itemRepository.GetAll()
                                                .Include(s => s.InventoryAccount)
                                                .Include(s => s.SaleAccount)
                                                .Include(s => s.PurchaseAccount)
                                                .Include(s => s.SaleTax)
                                                .Where(s => s.InventoryAccountId.HasValue)
                                                .AsNoTracking()
                                        on rItem.ItemId equals ii.Id

                                        join i in _inventoryTransactionItemRepository.GetAll()
                                                                     .Where(s => s.JournalType == JournalType.ItemIssueAdjustment)
                                                                     .Where(s => s.TransactionId == input.Id)
                                                                     .AsNoTracking()
                                        on rItem.Id equals i.Id
                                        into cs
                                        from c in cs.DefaultIfEmpty()
                                        select
                                        new ItemIssueItemAdjustmentDetailOutput()
                                        {
                                            LotId = rItem.LotId,
                                            LotDetial = ObjectMapper.Map<LotSummaryOutput>(rItem.Lot),
                                            Id = rItem.Id,
                                            Item = new ItemSummaryDetailOutput
                                            {
                                                InventoryAccount = ObjectMapper.Map<ChartAccountDetailOutput>(ii.InventoryAccount),
                                                Id = ii.Id,
                                                InventoryAccountId = ii.InventoryAccountId,
                                                ItemCode = ii.ItemCode,
                                                ItemName = ii.ItemName,
                                                PurchaseAccount = ObjectMapper.Map<ChartAccountDetailOutput>(ii.PurchaseAccount),
                                                PurchaseAccountId = ii.PurchaseAccountId,
                                                SaleAccount = ObjectMapper.Map<ChartAccountDetailOutput>(ii.SaleAccount),
                                                SaleAccountId = ii.SaleAccountId,
                                                SalePrice = ii.SalePrice,
                                                SaleTax = ObjectMapper.Map<TaxDetailOutput>(ii.SaleTax),
                                                SaleTaxId = ii.SaleTaxId,
                                            },
                                            //ObjectMapper.Map<ItemSummaryOutput>(rItem.Item),
                                            ItemId = rItem.ItemId,
                                            InventoryAccountId = jItem.AccountId,
                                            InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                                            Description = rItem.Description,
                                            DiscountRate = rItem.DiscountRate,
                                            Qty = rItem.Qty,
                                            Total = c != null ? Math.Abs(c.LineCost) : rItem.Total,
                                            UnitCost = c != null ? c.UnitCost : rItem.UnitCost,
                                            UseBatchNo = ii.UseBatchNo,
                                            TrackSerial = ii.TrackSerial,
                                            TrackExpiration = ii.TrackExpiration,
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


            var result = ObjectMapper.Map<ItemIssueAdjustmentDetailOutput>(journal.ItemIssue);
            result.Date = journal.Date;
            result.ReceiveNo = journal.JournalNo;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.LocationId = journal.LocationId.Value;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            result.Reference = journal.Reference;
            result.ItemIssueItemAdjustments = itemIssueItems;
            result.ClearanceAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(clearanceAccount);
            result.ClearanceAccountId = clearanceAccount.Id;
            result.Memo = journal.Memo;
            result.StatusCode = journal.Status;
            //result.Total = journal.Credit;
            result.StatusName = journal.Status.ToString();
            result.TransactionTypeName = journal.JournalTransactionType?.Name;

            //get total from inventory transaction item cache table
            result.Total = itemIssueItems.Sum(s => s.Total);

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_Update,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Update,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour)]
        public async Task<NullableIdDto<Guid>> Update(UpdateItemIssueAdjustmentInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            // update Journal 
            var @journal = await _journalRepository
                              .GetAll()
                              .Where(u => u.JournalType == JournalType.ItemIssueAdjustment && u.ItemIssueId == input.Id)
                              .FirstOrDefaultAsync();

            await CheckClosePeriod(journal.Date, input.Date);

            await ValidateBatchNo(input);

            var scheduleDate = input.Date > journal.Date ? journal.Date : input.Date;

            journal.Update(tenantId, input.ReceiptNo, input.Date, input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, 0, input.LocationId);
            journal.UpdateStatus(input.Status);

            var validatelot = input.Items.Where(t => t.FromLotId == null && t.ItemId != null).FirstOrDefault();
            if (validatelot != null)
            {
                throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + (input.Items.IndexOf(validatelot) + 1).ToString());
            }

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
            #region Calculat Cost
            //var roundingId = (await GetCurrentTenantAsync()).RoundDigitAccountId;

            var itemToCalculateCost = input.Items.Select((u, index) => new Inventories.Data.GetAvgCostForIssueData()
            {
                ItemName = u.Item.ItemName,
                Index = index,
                ItemId = u.ItemId,
                ItemCode = u.Item.ItemCode,
                Qty = u.Qty,
                ItemIssueItemId = input.Id,
                LotId = u.FromLotId,
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
            var @clearanceAccountItem = await (_journalItemRepository.GetAll()
                                      .Where(u => u.JournalId == journal.Id &&
                                               u.Key == PostingKey.COGS && u.Identifier == null)).FirstOrDefaultAsync();
            clearanceAccountItem.UpdateJournalItem(tenantId, input.ClearanceAccountId, input.Memo, input.Total, 0);

            //update item Issue 
            var itemIssue = await _itemIssueManager.GetAsync(input.Id, true);

            //validate EditBy 48 hours
            var getPermission = await GetUserPermissions(userId);
            var IsEdit = getPermission.Where(t => t == AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour).Count();
            var totalHours = (DateTime.Now - itemIssue.CreationTime).TotalHours;
            if (IsEdit > 0 && totalHours > 48)
            {
                throw new UserFriendlyException(L("ThisTransactionIsOver48HoursCanNotEdit"));
            }
            if (itemIssue == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            itemIssue.Update(tenantId, ReceiveFrom.None, null,
                          input.SameAsShippingAddress, input.ShippingAddress,
                          input.BillingAddress, input.Total, null);

            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);
            CheckErrors(await _journalManager.UpdateAsync(@journal, autoSequence.DocumentType));

            CheckErrors(await _journalItemManager.UpdateAsync(@clearanceAccountItem));
            CheckErrors(await _itemIssueManager.UpdateAsync(itemIssue));


            //Update Item Issue Item and Journal Item
            var itemIssueItems = await _itemIssueItemRepository.GetAll().Where(u => u.ItemIssueId == input.Id).ToListAsync();

            var @inventoryJournalItems = await (_journalItemRepository.GetAll()
                                        .Where(u => u.JournalId == journal.Id &&
                                           u.Key == PostingKey.Inventory && u.Identifier != null))
                                        .ToListAsync();

            var toDeleteItemIssueItem = itemIssueItems
                                        .Where(u => !input.Items.Any(i => i.Id != null && i.Id == u.Id))
                                        .ToList();
            var toDeletejournalItem = inventoryJournalItems
                                        .Where(u => !input.Items.Any(i => u.Identifier != null && i.Id != null && i.Id == u.Identifier))
                                        .ToList();


            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }

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



        #region import Excel item receipt adjustment
        private ReportOutput GetReportTemplateItemReceiptAdjustment()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "Account",
                        ColumnLength = 180,
                        ColumnTitle = "Account Code",
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
                        ColumnName = "Class",
                        ColumnLength = 230,
                        ColumnTitle = "Class",
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
                        ColumnName = "IssueNo",
                        ColumnLength = 130,
                        ColumnTitle = "Issue No",
                        ColumnType = ColumnType.Money,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Reference",
                        ColumnLength = 250,
                        ColumnTitle = "Reference",
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
                        ColumnName = "IssueDate",
                        ColumnLength = 200,
                        ColumnTitle = "Issue Date",
                        ColumnType = ColumnType.Date,
                        SortOrder = 6,
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
                        SortOrder = 7,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Description",
                        ColumnLength = 130,
                        ColumnTitle = "Description",
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
                        ColumnName = "Location",
                        ColumnLength = 250,
                        ColumnTitle = "Location",
                        ColumnType = ColumnType.String,
                        SortOrder = 9,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Lot",
                        ColumnLength = 250,
                        ColumnTitle = "Zone",
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
                        ColumnName = "Qty",
                        ColumnLength = 130,
                        ColumnTitle = "Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 11,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "BatchNo",
                        ColumnLength = 250,
                        ColumnTitle = "Batch No",
                        ColumnType = ColumnType.String,
                        SortOrder = 12,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true
                    },

                },
                Groupby = "",
                HeaderTitle = "Item Issue Adjustment",
                Sortby = "",
            };
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_ImportExcelItemIssueAdjustment,
            AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueAdjustment)]
        [UnitOfWork(IsDisabled = true)]
        public async Task ImportExcel(FileDto input)
        {
            Tenant tenant = null;
            var userId = AbpSession.GetUserId();
            var tenantId = AbpSession.TenantId;
            var accounts = new List<ChartOfAccount>();
            var locations = new List<Location>();
            var lots = new List<Lot>();
            var @class = new List<Class>();
            var items = new List<Item>();
            AutoSequence auto = null;
            var batchNos = new List<BatchNo>();
            Guid? transactionTypeId = null;
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);
                    tenant = await GetCurrentTenantAsync();
                    @accounts = await _chartOfAccountRepository.GetAll().AsNoTracking().ToListAsync();
                    @locations = await _locationRepository.GetAll().AsNoTracking().ToListAsync();
                    @lots = await _lotRepository.GetAll().Include(t => t.Location).AsNoTracking().ToListAsync();
                    @class = await _classRepository.GetAll().AsNoTracking().ToListAsync();
                    @items = await _itemRepository.GetAll().AsNoTracking().ToListAsync();
                    batchNos = await _batchNoRepository.GetAll().AsNoTracking().ToListAsync();
                    #region journal transaction type 
                    transactionTypeId = await _journalTransactionTypeManager.GetJournalTransactionTypeId(InventoryTransactionType.ItemIssueAdjustment);                    
                    #endregion
                }
            }

            //var excelPackage = Read(input, _appFolders);
            var excelPackage = await Read(input);


            if (excelPackage != null)
            {
                // Get the work book in the file
                var workBook = excelPackage.Workbook;
                if (workBook != null)
                {
                    // retrive first worksheets
                    var worksheet = excelPackage.Workbook.Worksheets[0];

                    var accountName = worksheet.Cells[2, 1].Value?.ToString();
                    var className = worksheet.Cells[2, 2].Value?.ToString();
                    var IssueNo = worksheet.Cells[2, 3].Value?.ToString();
                    var reference = worksheet.Cells[2, 4].Value?.ToString();
                    var issueDateString = worksheet.Cells[2, 5].Value?.ToString();
                    var locationName = worksheet.Cells[2, 8].Value?.ToString();

                    if (string.IsNullOrWhiteSpace(issueDateString)) throw new UserFriendlyException(L("DateCannotBeEmpty"));
                    var issueDate = Convert.ToDateTime(issueDateString);

                    var accountId = accounts.Where(s => s.AccountCode == accountName).FirstOrDefault();
                    var locationId = locations.Where(s => s.LocationName == locationName).FirstOrDefault();
                    var classId = @class.Where(s => s.ClassName == className).FirstOrDefault();
                    if (accountId == null)
                    {
                        throw new UserFriendlyException(L("NoAccountFound"));
                    }
                    if (locationId == null)
                    {
                        throw new UserFriendlyException(L("NoLocationFound"));
                    }
                    if (classId == null)
                    {
                        throw new UserFriendlyException(L("NoClassFound"));
                    }                
                    //insert to journal
                    var transactionNo = string.Empty;
                    var memo = "Begining Stock";

                    if (auto.CustomFormat == true)
                    {
                        if (!string.IsNullOrEmpty(IssueNo))
                        {
                            throw new UserFriendlyException(L("CannotSetReceiptNoCurrentSettingUseAutoNumber"));
                        }
                        var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                                       auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                        transactionNo = newAuto;
                        auto.UpdateLastAutoSequenceNumber(newAuto);

                    }
                    else if (string.IsNullOrEmpty(IssueNo))
                    {
                        throw new UserFriendlyException(L("IssueNoCannotBeBlank"));
                    }

                    decimal total = 0;
                    var @entity = Journal.Create(tenant.Id, userId, transactionNo, issueDate,
                    memo, total, total, tenant.CurrencyId.Value, @classId.Id, reference, locationId.Id);
                    entity.UpdateStatus(TransactionStatus.Publish);
                    entity.SetJournalTransactionTypeId(transactionTypeId);
                    //insert clearance journal item into credit
                    var clearanceJournalItem = JournalItem.CreateJournalItem(tenant.Id, userId, entity.Id, accountId.Id, memo,
                        total, 0, PostingKey.COGS, null);

                    //insert to item Issue
                    CAddress billAddress = new CAddress("", "", "", "", "");
                    CAddress shipAddress = new CAddress("", "", "", "", "");
                    var itemIssue = ItemIssue.Create(tenant.Id, userId, ReceiveFrom.None, null,
                        true, shipAddress, billAddress,
                        total, null);

                    itemIssue.UpdateTransactionType(InventoryTransactionType.ItemIssueAdjustment);
                    @entity.UpdateIssueAdjustmentId(itemIssue.Id);
                    @entity.UpdateCreditDebit(total, total);


                    var addJournalItems = new List<JournalItem>();
                    var addItemIssueItems = new List<ItemIssueItem>();

                    addJournalItems.Add(clearanceJournalItem);

                    var inputIssueItems = new List<CreateOrUpdateItemIssueItemAdjustmentInput>();
                    var addItemIssueItemBatchNos = new List<ItemIssueItemBatchNo>();

                    //loop all rows to insert item receipt items and to journal items
                    for (int i = worksheet.Dimension.Start.Row; i <= worksheet.Dimension.End.Row; i++)
                    {
                        if (i > 1)
                        {
                            var itemCode = worksheet.Cells[i, 6].Value?.ToString();
                            var description = worksheet.Cells[i, 7].Value?.ToString();
                            var qty = Convert.ToDecimal(worksheet.Cells[i, 10].Value?.ToString());
                            var unitcost = 0;
                            var totalItem = qty * unitcost;
                            var lotName = worksheet.Cells[i, 9].Value?.ToString();
                            var locationNameByIndex = worksheet.Cells[i, 8].Value?.ToString();
                            var lot = @lots.Where(s => s.LotName == lotName && s.Location.LocationName == locationNameByIndex).FirstOrDefault();
                            if (lot == null)
                            {
                                throw new UserFriendlyException(L("NoLotFound", i));
                            }
                            if (qty <= 0)
                            {
                                throw new UserFriendlyException(L("QtyMustBeGreaterThanZero") + " Row = " + i);
                            }
                            total += totalItem; //update total 

                            var item = @items.Where(s => s.ItemCode == itemCode).Select(s => s).FirstOrDefault();
                            if (item == null)
                            {
                                throw new UserFriendlyException(L("NoItemFound", i));
                            }

                            var issueItem = new CreateOrUpdateItemIssueItemAdjustmentInput
                            {
                                Item = ObjectMapper.Map<ItemSummaryDetailOutput>(item),
                                ItemId = item.Id,
                                Qty = qty,
                                FromLotId = lot.Id,
                                Description = description,
                                UnitCost = 0,
                                Total = totalItem,
                                DiscountRate = 0,
                                InventoryAccountId = item.InventoryAccountId.Value,
                            };

                            inputIssueItems.Add(issueItem);


                            if (item.UseBatchNo || item.TrackSerial || item.TrackExpiration)
                            {
                                var batchNumber = worksheet.Cells[i, 11].Value?.ToString();
                                if (batchNumber.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("PleaseEnter", L("BatchNo")) + $", Row: {i}");

                                var findBatchNo = batchNos.FirstOrDefault(s => s.Code == batchNumber);
                                if (findBatchNo == null) throw new UserFriendlyException(L("IsNotValid", L("BatchNo")) + $", Row: {i}");

                                if (item.TrackSerial && qty != 1) throw new UserFriendlyException(L("MustBeEqualTo", $"{L("SerialNo")} {L("Qty")}", 1) + $", Row: {i}");

                                issueItem.ItemBatchNos = new List<BatchNoItemOutput>() { new BatchNoItemOutput { BatchNoId = findBatchNo.Id, Qty = issueItem.Qty } };
                            }

                        }
                    }

                    #region Calculat Cost
                    //var roundingId = (await GetCurrentTenantAsync()).RoundDigitAccountId;

                    var itemToCalculateCost = inputIssueItems.Select((u, index) => new Inventories.Data.GetAvgCostForIssueData()
                    {
                        ItemName = u.Item.ItemName,
                        LotId = u.FromLotId,
                        Index = index,
                        ItemId = u.ItemId,
                        ItemCode = u.Item.ItemCode,
                        Qty = u.Qty,
                        ItemIssueItemId = u.Id,
                        InventoryAccountId = u.InventoryAccountId
                    }).ToList();
                    var lotIds = inputIssueItems.GroupBy(s => s.FromLotId).Select(x => x.Key.Value).ToList();
                    var locationIds = lots
                                        .Where(t => lotIds.Contains(t.Id))
                                        .Select(t => (long?)t.LocationId)
                                        .ToList();

                    GetAvgCostForIssueOutput getCostResult = null;

                    using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
                    {
                        using (_unitOfWorkManager.Current.SetTenantId(tenant.Id))
                        {
                            getCostResult = await _inventoryManager.GetAvgCostForIssues(issueDate, locationIds, itemToCalculateCost/*, @entity, roundingId*/);
                        }
                    }


                    foreach (var r in getCostResult.Items)
                    {
                        inputIssueItems[r.Index].UnitCost = r.UnitCost;
                        inputIssueItems[r.Index].Total = r.LineCost;
                    }

                    total = getCostResult.Total;

                    #endregion Calculat Cost



                    foreach (var item in inputIssueItems)
                    {

                        //insert to item Receipt item
                        var itemIssueItem = ItemIssueItem.Create(tenant.Id, userId, itemIssue.Id, item.ItemId,
                                        item.Description, item.Qty, item.UnitCost, 0, item.Total);
                        itemIssueItem.UpdateLot(item.FromLotId);

                        addItemIssueItems.Add(itemIssueItem);

                        //insert inventory journal item into debit
                        var inventoryJournalItem = JournalItem.CreateJournalItem(tenant.Id, userId, entity.Id, item.InventoryAccountId,
                                                    item.Description, 0, item.Total, PostingKey.Inventory, itemIssueItem.Id);
                        addJournalItems.Add(inventoryJournalItem);

                        if (item.ItemBatchNos != null && item.ItemBatchNos.Any())
                        {
                            foreach (var batch in item.ItemBatchNos)
                            {
                                var itemBatchNo = ItemIssueItemBatchNo.Create(tenant.Id, userId, itemIssueItem.Id, batch.BatchNoId, itemIssueItem.Qty);
                                addItemIssueItemBatchNos.Add(itemBatchNo);
                            }
                        }

                    }

                    entity.UpdateCreditDebit(total, total);
                    clearanceJournalItem.SetDebitCredit(total, 0);
                    itemIssue.UpdateTotal(total);

                    var scheduleItems = addItemIssueItems.Select(s => s.ItemId).Distinct().ToList();

                    using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
                    {
                        using (_unitOfWorkManager.Current.SetTenantId(tenant.Id))
                        {
                            CheckErrors(await _autoSequenceManager.UpdateAsync(auto));

                            // CheckErrors(await _itemIssueManager.CreateAsync(@itemIssue));
                            await _itemIssueRepository.BulkInsertAsync(new List<ItemIssue> { itemIssue });
                            await _itemIssueItemRepository.BulkInsertAsync(addItemIssueItems);

                            if (addItemIssueItemBatchNos.Any()) await _itemIssueItemBatchNoRepository.BulkInsertAsync(addItemIssueItemBatchNos);

                            //CheckErrors(await _journalManager.CreateAsync(@entity));
                            await _journalRepository.BulkInsertAsync(new List<Journal> { entity });
                            await _journalItemRepository.BulkInsertAsync(addJournalItems);

                            await SyncItemReceipt(@itemIssue.Id);
                            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
                            {
                                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, entity.Date, scheduleItems);
                            }

                        }

                        await uow.CompleteAsync();
                    }
                }
            }
            //RemoveFile(input, _appFolders);
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_ExportExcelTemplateItemIssueAdjustment,
            AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ExportItemIssueAdjustment)]
        public async Task<FileDto> ExportExcelTamplate()
        {

            var result = new FileDto();
            var sheetName = "Item Issue Adjustment";

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
                var headerList = GetReportTemplateItemReceiptAdjustment();

                foreach (var i in headerList.ColumnInfo)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);

                    colHeaderTable += 1;
                }
                #endregion Row 1

                result.FileName = $"ItemIssueAdjustomentTamplate.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        #endregion


        #region import Excel item receipt adjustment has default account
        private ReportOutput ItemReceiptAdjustmentDefaultAccount()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter                   
                     new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "IssueNo",
                        ColumnLength = 130,
                        ColumnTitle = "Issue No",
                        ColumnType = ColumnType.Money,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Reference",
                        ColumnLength = 250,
                        ColumnTitle = "Reference",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "IssueDate",
                        ColumnLength = 200,
                        ColumnTitle = "Issue Date",
                        ColumnType = ColumnType.Date,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false,
                        IsRequired = true,
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ItemCode",
                        ColumnLength = 250,
                        ColumnTitle = "Item Code",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false,
                        IsRequired = true,
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Description",
                        ColumnLength = 130,
                        ColumnTitle = "Description",
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
                        ColumnName = "Location",
                        ColumnLength = 250,
                        ColumnTitle = "Location",
                        ColumnType = ColumnType.String,
                        SortOrder = 6,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true,
                        IsRequired = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Lot",
                        ColumnLength = 250,
                        ColumnTitle = "Zone",
                        ColumnType = ColumnType.String,
                        SortOrder = 7,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true,
                        IsRequired = true,
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Qty",
                        ColumnLength = 130,
                        ColumnTitle = "Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 8,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false,
                        IsRequired = true,
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "BatchNo",
                        ColumnLength = 250,
                        ColumnTitle = "Batch No",
                        ColumnType = ColumnType.String,
                        SortOrder = 9,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true
                    },

                },
                Groupby = "",
                HeaderTitle = "Item Issue Adjustment",
                Sortby = "",
            };
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_ImportExcelItemIssueAdjustment,
            AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueAdjustment)]
        [UnitOfWork(IsDisabled = true)]
        public async Task ImportExcelHasDefaultAccount(FileDto input)
        {
            Tenant tenant = null;
            var userId = AbpSession.GetUserId();
            var tenantId = AbpSession.TenantId;
            var locations = new List<Location>();
            var lots = new List<Lot>();
            var items = new List<Item>();
            AutoSequence auto = null;
            var batchNos = new List<BatchNo>();
            Guid? transactionTypeId = null;
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);
                    tenant = await GetCurrentTenantAsync();
                    @locations = await _locationRepository.GetAll().AsNoTracking().ToListAsync();
                    @lots = await _lotRepository.GetAll().Include(t => t.Location).AsNoTracking().ToListAsync();
                    @items = await _itemRepository.GetAll().AsNoTracking().ToListAsync();
                    batchNos = await _batchNoRepository.GetAll().AsNoTracking().ToListAsync();
                    #region journal transaction type 
                    transactionTypeId = await _journalTransactionTypeManager.GetJournalTransactionTypeId(InventoryTransactionType.ItemIssueAdjustment);
                    #endregion
                }
            }

            //var excelPackage = Read(input, _appFolders);
            var excelPackage = await Read(input);


            if (excelPackage != null)
            {
                // Get the work book in the file
                var workBook = excelPackage.Workbook;
                if (workBook != null)
                {
                    // retrive first worksheets
                    var worksheet = excelPackage.Workbook.Worksheets[0];
                    var IssueNo = worksheet.Cells[2, 1].Value?.ToString();
                    var reference = worksheet.Cells[2, 2].Value?.ToString();
                    var issueDateString = worksheet.Cells[2, 3].Value?.ToString();
                    var locationName = worksheet.Cells[2, 6].Value?.ToString();

                    if (string.IsNullOrWhiteSpace(issueDateString)) throw new UserFriendlyException(L("DateCannotBeEmpty"));
                    var issueDate = Convert.ToDateTime(issueDateString);

                    var accountId = tenant.ItemIssueAdjustmentId;
                    var locationId = locations.Where(s => s.LocationName == locationName).FirstOrDefault();
                    var classId = tenant.ClassId;
                    if (accountId == null)
                    {
                        throw new UserFriendlyException(L("NoAccountFound"));
                    }
                    if (locationId == null)
                    {
                        throw new UserFriendlyException(L("NoLocationFound"));
                    }
                    if (classId == null)
                    {
                        throw new UserFriendlyException(L("NoClassFound"));
                    }
                    //insert to journal
                    var transactionNo = string.Empty;
                    var memo = "Begining Stock";

                    if (auto.CustomFormat == true)
                    {
                        if (!string.IsNullOrEmpty(IssueNo))
                        {
                            throw new UserFriendlyException(L("CannotSetReceiptNoCurrentSettingUseAutoNumber"));
                        }
                        var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                                       auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                        transactionNo = newAuto;
                        auto.UpdateLastAutoSequenceNumber(newAuto);

                    }
                    else if (string.IsNullOrEmpty(IssueNo))
                    {
                        throw new UserFriendlyException(L("IssueNoCannotBeBlank"));
                    }

                    decimal total = 0;
                    var @entity = Journal.Create(tenant.Id, userId, transactionNo, issueDate,
                    memo, total, total, tenant.CurrencyId.Value, @classId, reference, locationId.Id);
                    entity.UpdateStatus(TransactionStatus.Publish);
                    entity.SetJournalTransactionTypeId(transactionTypeId);
                    //insert clearance journal item into credit
                    var clearanceJournalItem = JournalItem.CreateJournalItem(tenant.Id, userId, entity.Id, accountId.Value, memo,
                        total, 0, PostingKey.COGS, null);

                    //insert to item Issue
                    CAddress billAddress = new CAddress("", "", "", "", "");
                    CAddress shipAddress = new CAddress("", "", "", "", "");
                    var itemIssue = ItemIssue.Create(tenant.Id, userId, ReceiveFrom.None, null,
                        true, shipAddress, billAddress,
                        total, null);

                    itemIssue.UpdateTransactionType(InventoryTransactionType.ItemIssueAdjustment);
                    @entity.UpdateIssueAdjustmentId(itemIssue.Id);
                    @entity.UpdateCreditDebit(total, total);


                    var addJournalItems = new List<JournalItem>();
                    var addItemIssueItems = new List<ItemIssueItem>();

                    addJournalItems.Add(clearanceJournalItem);

                    var inputIssueItems = new List<CreateOrUpdateItemIssueItemAdjustmentInput>();
                    var addItemIssueItemBatchNos = new List<ItemIssueItemBatchNo>();

                    //loop all rows to insert item receipt items and to journal items
                    for (int i = worksheet.Dimension.Start.Row; i <= worksheet.Dimension.End.Row; i++)
                    {
                        if (i > 1)
                        {
                            var itemCode = worksheet.Cells[i, 4].Value?.ToString();
                            var description = worksheet.Cells[i, 5].Value?.ToString();
                            var qty = Convert.ToDecimal(worksheet.Cells[i, 8].Value?.ToString());
                            var unitcost = 0;
                            var totalItem = qty * unitcost;
                            var lotName = worksheet.Cells[i, 7].Value?.ToString();
                            var locationNameByIndex = worksheet.Cells[i, 6].Value?.ToString();
                            var lot = @lots.Where(s => s.LotName == lotName && s.Location.LocationName == locationNameByIndex).FirstOrDefault();
                            if (lot == null)
                            {
                                throw new UserFriendlyException(L("NoLotFound", i));
                            }
                            if (qty <= 0)
                            {
                                throw new UserFriendlyException(L("QtyMustBeGreaterThanZero") + " Row = " + i);
                            }
                            total += totalItem; //update total 

                            var item = @items.Where(s => s.ItemCode == itemCode).Select(s => s).FirstOrDefault();
                            if (item == null)
                            {
                                throw new UserFriendlyException(L("NoItemFound", i));
                            }

                            var issueItem = new CreateOrUpdateItemIssueItemAdjustmentInput
                            {
                                Item = ObjectMapper.Map<ItemSummaryDetailOutput>(item),
                                ItemId = item.Id,
                                Qty = qty,
                                FromLotId = lot.Id,
                                Description = description,
                                UnitCost = 0,
                                Total = totalItem,
                                DiscountRate = 0,
                                InventoryAccountId = item.InventoryAccountId.Value,
                            };

                            inputIssueItems.Add(issueItem);


                            if (item.UseBatchNo || item.TrackSerial || item.TrackExpiration)
                            {
                                var batchNumber = worksheet.Cells[i, 9].Value?.ToString();
                                if (batchNumber.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("PleaseEnter", L("BatchNo")) + $", Row: {i}");

                                var findBatchNo = batchNos.FirstOrDefault(s => s.Code == batchNumber);
                                if (findBatchNo == null) throw new UserFriendlyException(L("IsNotValid", L("BatchNo")) + $", Row: {i}");

                                if (item.TrackSerial && qty != 1) throw new UserFriendlyException(L("MustBeEqualTo", $"{L("SerialNo")} {L("Qty")}", 1) + $", Row: {i}");

                                issueItem.ItemBatchNos = new List<BatchNoItemOutput>() { new BatchNoItemOutput { BatchNoId = findBatchNo.Id, Qty = issueItem.Qty } };
                            }

                        }
                    }

                    #region Calculat Cost
                    //var roundingId = (await GetCurrentTenantAsync()).RoundDigitAccountId;

                    var itemToCalculateCost = inputIssueItems.Select((u, index) => new Inventories.Data.GetAvgCostForIssueData()
                    {
                        ItemName = u.Item.ItemName,
                        LotId = u.FromLotId,
                        Index = index,
                        ItemId = u.ItemId,
                        ItemCode = u.Item.ItemCode,
                        Qty = u.Qty,
                        ItemIssueItemId = u.Id,
                        InventoryAccountId = u.InventoryAccountId
                    }).ToList();
                    var lotIds = inputIssueItems.GroupBy(s => s.FromLotId).Select(x => x.Key.Value).ToList();
                    var locationIds = lots
                                        .Where(t => lotIds.Contains(t.Id))
                                        .Select(t => (long?)t.LocationId)
                                        .ToList();

                    GetAvgCostForIssueOutput getCostResult = null;

                    using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
                    {
                        using (_unitOfWorkManager.Current.SetTenantId(tenant.Id))
                        {
                            getCostResult = await _inventoryManager.GetAvgCostForIssues(issueDate, locationIds, itemToCalculateCost/*, @entity, roundingId*/);
                        }
                    }


                    foreach (var r in getCostResult.Items)
                    {
                        inputIssueItems[r.Index].UnitCost = r.UnitCost;
                        inputIssueItems[r.Index].Total = r.LineCost;
                    }

                    total = getCostResult.Total;

                    #endregion Calculat Cost



                    foreach (var item in inputIssueItems)
                    {

                        //insert to item Receipt item
                        var itemIssueItem = ItemIssueItem.Create(tenant.Id, userId, itemIssue.Id, item.ItemId,
                                        item.Description, item.Qty, item.UnitCost, 0, item.Total);
                        itemIssueItem.UpdateLot(item.FromLotId);

                        addItemIssueItems.Add(itemIssueItem);

                        //insert inventory journal item into debit
                        var inventoryJournalItem = JournalItem.CreateJournalItem(tenant.Id, userId, entity.Id, item.InventoryAccountId,
                                                    item.Description, 0, item.Total, PostingKey.Inventory, itemIssueItem.Id);
                        addJournalItems.Add(inventoryJournalItem);

                        if (item.ItemBatchNos != null && item.ItemBatchNos.Any())
                        {
                            foreach (var batch in item.ItemBatchNos)
                            {
                                var itemBatchNo = ItemIssueItemBatchNo.Create(tenant.Id, userId, itemIssueItem.Id, batch.BatchNoId, itemIssueItem.Qty);
                                addItemIssueItemBatchNos.Add(itemBatchNo);
                            }
                        }

                    }

                    entity.UpdateCreditDebit(total, total);
                    clearanceJournalItem.SetDebitCredit(total, 0);
                    itemIssue.UpdateTotal(total);

                    var scheduleItems = addItemIssueItems.Select(s => s.ItemId).Distinct().ToList();

                    using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
                    {
                        using (_unitOfWorkManager.Current.SetTenantId(tenant.Id))
                        {
                            CheckErrors(await _autoSequenceManager.UpdateAsync(auto));

                            // CheckErrors(await _itemIssueManager.CreateAsync(@itemIssue));
                            await _itemIssueRepository.BulkInsertAsync(new List<ItemIssue> { itemIssue });
                            await _itemIssueItemRepository.BulkInsertAsync(addItemIssueItems);

                            if (addItemIssueItemBatchNos.Any()) await _itemIssueItemBatchNoRepository.BulkInsertAsync(addItemIssueItemBatchNos);

                            //CheckErrors(await _journalManager.CreateAsync(@entity));
                            await _journalRepository.BulkInsertAsync(new List<Journal> { entity });
                            await _journalItemRepository.BulkInsertAsync(addJournalItems);

                            await SyncItemReceipt(@itemIssue.Id);
                            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
                            {
                                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, entity.Date, scheduleItems);
                            }

                        }

                        await uow.CompleteAsync();
                    }
                }
            }
            //RemoveFile(input, _appFolders);
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_ExportExcelTemplateItemIssueAdjustment,
            AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ExportItemIssueAdjustment)]
        public async Task<FileDto> ExportExcelTamplateHasDefaultAccount()
        {

            var result = new FileDto();
            var sheetName = "Item Issue Adjustment";

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
                var headerList = ItemReceiptAdjustmentDefaultAccount();

                foreach (var i in headerList.ColumnInfo)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true, 0, i.IsRequired);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);

                    colHeaderTable += 1;
                }
                #endregion Row 1

                result.FileName = $"ItemIssueAdjustomentTamplate.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        #endregion


    }
}
