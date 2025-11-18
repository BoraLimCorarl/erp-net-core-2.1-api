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
using CorarlERP.AccountCycles;
using CorarlERP.Addresses;
using CorarlERP.Authorization;
using CorarlERP.AutoSequences;
using CorarlERP.ChartOfAccounts;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.Dto;
using CorarlERP.ItemReceiptOthers.Dto;
using CorarlERP.ItemReceipts;
using CorarlERP.Items;
using CorarlERP.Items.Dto;
using CorarlERP.Journals;
using CorarlERP.Journals.Dto;
using CorarlERP.Locations;
using CorarlERP.Locations.Dto;
using CorarlERP.Locks;
using CorarlERP.Lots;
using CorarlERP.Lots.Dto;
using CorarlERP.Reports;
using CorarlERP.Reports.Dto;
using CorarlERP.ReportTemplates;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.FileStorages;
using CorarlERP.InventoryCalculationJobSchedules;
using Hangfire.States;
using Abp.Domain.Uow;
using System.Transactions;
using TransactionStatus = CorarlERP.enumStatus.EnumStatus.TransactionStatus;
using CorarlERP.MultiTenancy;
using CorarlERP.InventoryCalculationItems;
using CorarlERP.BatchNos;
using Abp.Extensions;
using CorarlERP.ItemReceipts.Dto;
using CorarlERP.JournalTransactionTypes;
using CorarlERP.InventoryTransactionTypes;
using CorarlERP.Features;

namespace CorarlERP.ItemReceiptOthers
{
    [AbpAuthorize]
    public class ItemReceiptOtherAppService : ReportBaseClass, IItemReceiptOtherAppService
    {
        private readonly AppFolders _appFolders;

        private readonly IItemManager _itemManager;
        private readonly IRepository<Item, Guid> _itemRepository;

        private readonly IItemReceiptItemManager _itemReceiptItemManager;
        private readonly IItemReceiptManager _itemReceiptManager;

        private readonly ICorarlRepository<ItemReceipt, Guid> _itemReceiptRepository;
        private readonly ICorarlRepository<ItemReceiptItem, Guid> _itemReceiptItemRepository;

        private readonly IJournalManager _journalManager;
        private readonly ICorarlRepository<Journal, Guid> _journalRepository;

        private readonly IJournalItemManager _journalItemManager;
        private readonly ICorarlRepository<JournalItem, Guid> _journalItemRepository;

        private readonly IChartOfAccountManager _chartOfAccountManager;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;

        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;

        private readonly ICorarlRepository<BatchNo, Guid> _batchNoRepository;
        private readonly ICorarlRepository<ItemReceiptItemBatchNo, Guid> _itemReceiptItemBatchNoRepository;

        private readonly IRepository<Location, long> _locationRepository;
        private readonly IRepository<Lot, long> _lotRepository;
        private readonly IRepository<Class, long> _classRepository;
        private readonly IRepository<Lock, long> _lockRepository;
        private readonly IFileStorageManager _fileStorageManager;
        private readonly IInventoryCalculationItemManager _inventoryCalculationItemManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<JournalTransactionType, Guid> _journalTransactionTypeRepository;
        public ItemReceiptOtherAppService(
           IJournalManager journalManager,
           IJournalItemManager journalItemManager,
           IChartOfAccountManager chartOfAccountManager,
           IInventoryCalculationItemManager inventoryCalculationItemManager,
           IUnitOfWorkManager unitOfWorkManager,
           ItemReceiptManager itemReceiptManager,
           ItemReceiptItemManager itemReceiptItemManager,
           ItemManager itemManager,
           AppFolders appFolders,
           IFileStorageManager fileStorageManager,
           IRepository<Item, Guid> itemRepository,
           ICorarlRepository<Journal, Guid> journalRepository,
           ICorarlRepository<JournalItem, Guid> journalItemRepository,
           IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
           ICorarlRepository<ItemReceipt, Guid> itemReceiptRepository,
           ICorarlRepository<ItemReceiptItem, Guid> itemReceiptItemRepository,
           IRepository<AutoSequence, Guid> autoSequenceRepository,
           ICorarlRepository<BatchNo, Guid> batchNoRepository,
           ICorarlRepository<ItemReceiptItemBatchNo, Guid> itemReceiptItemBatchNoRepository,
           IRepository<Location, long> locationRepository,
           IRepository<Class, long> classRepository,
           IRepository<Lot, long> lotRepository,
           IRepository<Lock, long> lockRepository,
           IRepository<AccountCycle, long> accountCycleRepository,
           IRepository<JournalTransactionType, Guid> journalTransactionTypeRepository,
           IAutoSequenceManager autoSequenceManager) : base(accountCycleRepository, appFolders, null, null)
        {
            _lotRepository = lotRepository;
            _journalManager = journalManager;
            _journalManager.SetJournalType(JournalType.ItemReceiptOther);
            _journalRepository = journalRepository;
            _journalItemManager = journalItemManager;
            _journalItemRepository = journalItemRepository;
            _chartOfAccountManager = chartOfAccountManager;
            _chartOfAccountRepository = chartOfAccountRepository;
            _itemReceiptItemManager = itemReceiptItemManager;
            _itemReceiptItemRepository = itemReceiptItemRepository;
            _itemReceiptManager = itemReceiptManager;
            _itemReceiptRepository = itemReceiptRepository;
            _itemRepository = itemRepository;
            _itemManager = itemManager;
            _autoSequenceManager = autoSequenceManager;
            _autoSequenceRepository = autoSequenceRepository;
            _appFolders = appFolders;
            _locationRepository = locationRepository;
            _classRepository = classRepository;
            _lockRepository = lockRepository;
            _fileStorageManager = fileStorageManager;
            _inventoryCalculationItemManager = inventoryCalculationItemManager;
            _unitOfWorkManager = unitOfWorkManager;
            _batchNoRepository = batchNoRepository;
            _itemReceiptItemBatchNoRepository = itemReceiptItemBatchNoRepository;
            _journalTransactionTypeRepository = journalTransactionTypeRepository;
        }

        private async Task ValidateAddBatchNo(CreateItemReceiptOtherInput input)
        {
            var validateItems = await _itemRepository.GetAll()
                             .Include(s => s.BatchNoFormula)
                             .Where(s => input.Items.Any(i => i.ItemId == s.Id))
                             .Where(s => s.UseBatchNo || s.TrackSerial || s.TrackExpiration)
                             .AsNoTracking()
                             .ToListAsync();

            if (!validateItems.Any()) return;

            var useBatchItemDic = validateItems.ToDictionary(k => k.Id, v => v);

            var useBatchItems = input.Items.Where(s => useBatchItemDic.ContainsKey(s.ItemId)).ToList();

            var emptyManualBatchItem = useBatchItems.FirstOrDefault(s => !useBatchItemDic[s.ItemId].AutoBatchNo && (s.ItemBatchNos == null || s.ItemBatchNos.Count == 0 || s.ItemBatchNos.Any(r => r.BatchNumber.IsNullOrWhiteSpace())));
            if (emptyManualBatchItem != null) throw new UserFriendlyException(L("PleaseEnter", $"{L("BatchNo")}/{L("SerialNo")}") + $", Item: {useBatchItemDic[emptyManualBatchItem.ItemId].ItemCode}-{useBatchItemDic[emptyManualBatchItem.ItemId].ItemName}");

            var expriationItemHash = validateItems.Where(s => s.TrackExpiration).Select(s => s.Id).ToHashSet();
            var expirationItems = input.Items.Where(s => expriationItemHash.Contains(s.ItemId)).ToList();
            var emptyExpiration = expirationItems.FirstOrDefault(s => s.ItemBatchNos == null || s.ItemBatchNos.Count == 0 || s.ItemBatchNos.Any(r => !r.ExpirationDate.HasValue));
            if (emptyExpiration != null) throw new UserFriendlyException(L("PleaseEnter", L("ExpiratioinDate")) + $", Item: {useBatchItemDic[emptyExpiration.ItemId].ItemCode}-{useBatchItemDic[emptyExpiration.ItemId].ItemName}");

            var serialItemHash = validateItems.Where(s => s.TrackSerial).Select(s => s.Id).ToHashSet();
            var serialQty = input.Items.Where(s => serialItemHash.Contains(s.ItemId)).FirstOrDefault(s => s.ItemBatchNos.Any(b => b.Qty != 1));
            if (serialQty != null) throw new UserFriendlyException(L("MustBeEqualTo", $"{L("SerialNo")} {L("Qty")}", 1) + $", Item: {useBatchItemDic[serialQty.ItemId].ItemCode}-{useBatchItemDic[serialQty.ItemId].ItemName}");

            var autoBatchItems = useBatchItems.Where(s => useBatchItemDic[s.ItemId].AutoBatchNo).ToList();
            if (autoBatchItems.Any())
            {
                foreach (var item in autoBatchItems)
                {
                    var formula = useBatchItemDic[item.ItemId].BatchNoFormula;
                    if (formula == null) throw new UserFriendlyException(L("IsNotValid", L("Formula")) + $", Item: {useBatchItemDic[item.ItemId].ItemCode}-{useBatchItemDic[item.ItemId].ItemName}");

                    var field = useBatchItemDic[item.ItemId].GetType().GetProperty(formula.FieldName).GetValue(useBatchItemDic[item.ItemId], null);
                    if (field == null) throw new UserFriendlyException(L("IsNotValid", formula.FieldName));

                    var code = field.ToString();

                    if (item.ItemBatchNos == null) item.ItemBatchNos = new List<BatchNoItemOutput> { new BatchNoItemOutput { Qty = item.Qty } };

                    foreach (var batch in item.ItemBatchNos)
                    {
                        batch.BatchNumber = $"{code}-{input.Date.ToString(formula.DateFormat)}";
                    }
                }
            }

            var duplicate = useBatchItems.FirstOrDefault(s => s.ItemBatchNos.GroupBy(g => g.BatchNumber).Any(r => r.Count() > 1));
            if (duplicate != null) throw new UserFriendlyException(L("Duplicated", $"{L("BatchNo")}/{L("SerialNo")}" + $" , Item: {useBatchItemDic[duplicate.ItemId].ItemCode}-{useBatchItemDic[duplicate.ItemId].ItemName}"));

            var zeroQty = useBatchItems.FirstOrDefault(s => s.ItemBatchNos.Any(r => r.Qty <= 0));
            if (zeroQty != null) throw new UserFriendlyException(L("MustBeGreaterThen", L("Qty"), 0));

            var validateQty = useBatchItems.FirstOrDefault(s => s.Qty != s.ItemBatchNos.Sum(t => t.Qty));
            if (validateQty != null) throw new UserFriendlyException(L("IsNotValid", L("Qty") + " " + $"{L("BatchNo")}/{L("SerialNo")}" + $", Item: {useBatchItemDic[validateQty.ItemId].ItemCode}-{useBatchItemDic[validateQty.ItemId].ItemName}"));

            var batchNos = useBatchItems.SelectMany(s => s.ItemBatchNos).GroupBy(s => s.BatchNumber).Select(s => s.Key).ToHashSet();
            var batchNoDic = await _batchNoRepository.GetAll().Where(s => batchNos.Contains(s.Code)).ToDictionaryAsync(k => k.Code, v => v);

            var addBatchNos = new List<BatchNo>();
            foreach (var item in useBatchItems)
            {
                foreach (var i in item.ItemBatchNos)
                {
                    if (batchNoDic.ContainsKey(i.BatchNumber))
                    {
                        if (batchNoDic[i.BatchNumber].ItemId != item.ItemId) throw new UserFriendlyException(L("BatchNoAlreadyUseByOtherItem") + $" {i.BatchNumber}");
                        if (expriationItemHash.Contains(item.ItemId))
                        {
                            if (batchNoDic[i.BatchNumber].ExpirationDate.Value.Date != i.ExpirationDate.Value.Date) throw new UserFriendlyException(L("IsNotValid", L("ExpirationDate")) + $" : {i.ExpirationDate.Value.ToString("dd-MM-yyyy")}");
                        }

                        i.BatchNoId = batchNoDic[i.BatchNumber].Id;
                    }
                    else
                    {
                        var batchNo = BatchNo.Create(AbpSession.TenantId.Value, AbpSession.UserId.Value, i.BatchNumber, item.ItemId, i.IsStandard, serialItemHash.Contains(item.ItemId), i.ExpirationDate);
                        i.BatchNoId = batchNo.Id;
                        addBatchNos.Add(batchNo);
                        batchNoDic.Add(i.BatchNumber, batchNo);
                    }
                }
            }

            var duplicateBatch = useBatchItems
                             .SelectMany(s => s.ItemBatchNos.Select(r => new { r.BatchNumber, s.ItemId }))
                             .GroupBy(s => s.BatchNumber)
                             .Select(g => g.Select(s => new { s.ItemId, s.BatchNumber }).Distinct())
                             .Where(s => s.Count() > 1)
                             .FirstOrDefault();
            if (duplicateBatch != null && duplicateBatch.Any()) throw new UserFriendlyException(L("Duplicated", $"{L("BatchNo")}/{L("SerialNo")}" + $": {duplicateBatch.First().BatchNumber}, Item : {useBatchItemDic[duplicateBatch.First().ItemId].ItemCode}-{useBatchItemDic[duplicateBatch.First().ItemId].ItemName}"));

            if (addBatchNos.Any()) await _batchNoRepository.BulkInsertAsync(addBatchNos);
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CreateOthers,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptOther)]
        public async Task<NullableIdDto<Guid>> Create(CreateItemReceiptOtherInput input)
        {

            if (input.InventoryTransactionTypeId == null || input.InventoryTransactionTypeId == Guid.Empty) throw new UserFriendlyException(L("TransactionTypeIdNotFound"));
            if (input.IsConfirm == false)
            {

                var locktransaction = await _lockRepository.GetAll()
                                     .Where(t => (t.LockKey == TransactionLockType.ItemReceipt || t.LockKey == TransactionLockType.InventoryTransaction)
                                     && t.IsLock == true && t.LockDate.Value.Date >= input.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            await ValidateAddBatchNo(input);

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt);

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
            var @entity =
                Journal.Create(tenantId, userId, input.ReceiptNo, input.Date,
                input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, input.LocationId);
            entity.UpdateStatus(input.Status);
            entity.SetJournalTransactionTypeId(input.InventoryTransactionTypeId);
            //insert clearance journal item into credit
            var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, input.ClearanceAccountId, input.Memo, 0,
                input.Total, PostingKey.Clearance, null);

            //insert to item Receipt
            var @itemReceipt = ItemReceipt.Create(tenantId, userId, ItemReceipt.ReceiveFromStatus.None, null,
                input.SameAsShippingAddress, input.ShippingAddress, input.BillingAddress,
                input.Total, null);

            itemReceipt.UpdateTransactionType(InventoryTransactionType.ItemReceiptOther);
            @entity.UpdateItemReceiptOther(@itemReceipt);

            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));
            CheckErrors(await _journalItemManager.CreateAsync(clearanceJournalItem));
            CheckErrors(await _itemReceiptManager.CreateAsync(@itemReceipt));

            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }
            int indexLot = 0;
            foreach (var i in input.Items)
            {
                indexLot++;
                if (i.ToLotId == null && i.ItemId != null)
                {
                    throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + indexLot.ToString());
                }
                //insert to item Receipt item
                var @itemReceiptItem = ItemReceiptItem.Create(tenantId, userId, @itemReceipt, i.ItemId,
                                                       i.Description, i.Qty, i.UnitCost, i.DiscountRate, i.Total);
                itemReceiptItem.UpdateLot(i.ToLotId);
                CheckErrors(await _itemReceiptItemManager.CreateAsync(@itemReceiptItem));
                //insert inventory journal item into debit
                var inventoryJournalItem =
                    JournalItem.CreateJournalItem(tenantId, userId, entity, i.InventoryAccountId,
                    i.Description, i.Total, 0, PostingKey.Inventory, itemReceiptItem.Id);
                CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));

                if (i.ItemBatchNos != null && i.ItemBatchNos.Any())
                {
                    var addItemBatchNos = i.ItemBatchNos.Select(s => ItemReceiptItemBatchNo.Create(tenantId, userId, itemReceiptItem.Id, s.BatchNoId, s.Qty)).ToList();
                    foreach (var a in addItemBatchNos)
                    {
                        await _itemReceiptItemBatchNoRepository.InsertAsync(a);
                    }
                }
            }

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.ItemReceipt, TransactionLockType.InventoryTransaction, };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            await SyncItemReceipt(itemReceipt.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                var scheduleItems = input.Items.Select(s => s.ItemId).Distinct().ToList();
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, input.Date, scheduleItems);
            }

            return new NullableIdDto<Guid>() { Id = @itemReceipt.Id };
        }



        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetDetail,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ViewDetail)]
        public async Task<ItemReceiptOtherDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            await TurnOffTenantFilterIfDebug();

            var @journal = await _journalRepository
                                .GetAll()
                                .Include(u => u.ItemReceipt)
                                .Include(u => u.Location)
                                .Include(u => u.Class)
                                .Include(u => u.Currency)
                                .Include(u => u.JournalTransactionType)
                                .Include(u => u.ItemReceipt.ShippingAddress)
                                .Include(u => u.ItemReceipt.BillingAddress)
                                .AsNoTracking()
                                .Where(u => u.JournalType == JournalType.ItemReceiptOther && u.ItemReceiptId == input.Id)
                                .FirstOrDefaultAsync();

            if (@journal == null || @journal.ItemReceipt == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var clearanceAccount = await (_journalItemRepository.GetAll()
                                       .Include(u => u.Account)
                                       .AsNoTracking()
                                       .Where(u => u.JournalId == journal.Id &&
                                                u.Key == PostingKey.Clearance && u.Identifier == null)
                                       .Select(u => u.Account)).FirstOrDefaultAsync();


            var itemReceiptItems = await _itemReceiptItemRepository.GetAll()
                .Include(u => u.Item)
                .Include(u => u.Lot)
                .Where(u => u.ItemReceiptId == input.Id)
                .Join(_journalItemRepository.GetAll()
                .Include(u => u.Account)
                .Where(u => u.Key == PostingKey.Inventory)
                .AsNoTracking(), u => u.Id, s => s.Identifier,
                (rItem, jItem) =>
                new ItemReceiptItemOtherDetailOutput()
                {
                    ToLotId = rItem.LotId,
                    ToLotDetail = ObjectMapper.Map<LotSummaryOutput>(rItem.Lot),
                    Id = rItem.Id,
                    Item = ObjectMapper.Map<ItemSummaryOutput>(rItem.Item),
                    ItemId = rItem.ItemId,
                    InventoryAccountId = jItem.AccountId,
                    InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                    Description = rItem.Description,
                    DiscountRate = rItem.DiscountRate,
                    Qty = rItem.Qty,
                    Total = rItem.Total,
                    UnitCost = rItem.UnitCost,
                    UseBatchNo = rItem.Item.UseBatchNo,
                    AutoBatchNo = rItem.Item.AutoBatchNo,
                    TrackExpiration = rItem.Item.TrackExpiration,
                    TrackSerial = rItem.Item.TrackSerial,
                })
                .ToListAsync();

            var batchDic = await _itemReceiptItemBatchNoRepository.GetAll()
                                .AsNoTracking()
                                .Where(s => s.ItemReceiptItem.ItemReceiptId == input.Id)
                                .Select(s => new BatchNoItemOutput
                                {
                                    Id = s.Id,
                                    BatchNoId = s.BatchNoId,
                                    BatchNumber = s.BatchNo.Code,
                                    ExpirationDate = s.BatchNo.ExpirationDate,
                                    Qty = s.Qty,
                                    TransactionItemId = s.ItemReceiptItemId
                                })
                                .GroupBy(s => s.TransactionItemId)
                                .ToDictionaryAsync(k => k.Key, v => v.ToList());
            if (batchDic.Any())
            {
                foreach (var i in itemReceiptItems)
                {
                    if (batchDic.ContainsKey(i.Id)) i.ItemBatchNos = batchDic[i.Id];
                }
            }

            var result = ObjectMapper.Map<ItemReceiptOtherDetailOutput>(journal.ItemReceipt);
            result.Date = journal.Date;
            result.ReceiveNo = journal.JournalNo;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.InventoryTransactionTypeName = journal?.JournalTransactionType?.Name;
            result.InventoryTransactionTypeId = journal.JournalTransactionTypeId;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.Reference = journal.Reference;
            result.ItemReceiptItemOthers = itemReceiptItems;
            result.ClearanceAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(clearanceAccount);
            result.ClearanceAccountId = clearanceAccount.Id;
            result.Memo = journal.Memo;
            result.StatusCode = journal.Status;
            result.Total = journal.Credit;
            result.StatusName = journal.Status.ToString();
            result.LocationId = journal.LocationId.Value;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_Update,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Update,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour)]
        public async Task<NullableIdDto<Guid>> Update(UpdateItemReiptOtherInput input)
        {
            if (input.InventoryTransactionTypeId == null || input.InventoryTransactionTypeId == Guid.Empty) throw new UserFriendlyException(L("TransactionTypeIdNotFound"));
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
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            // update Journal 
            var @journal = await _journalRepository
                              .GetAll()
                              .Where(u => u.JournalType == JournalType.ItemReceiptOther && u.ItemReceiptId == input.Id)
                              .FirstOrDefaultAsync();
            await CheckClosePeriod(journal.Date, input.Date);

            //Update Item Receipt Item and Journal Item
            var itemReceipItems = await _itemReceiptItemRepository.GetAll()
                                        .Where(u => u.ItemReceiptId == input.Id)
                                        .ToListAsync();

            await ValidateAddBatchNo(input);

            var scheduleDate = input.Date > journal.Date ? journal.Date : input.Date;

            journal.Update(tenantId, input.ReceiptNo, input.Date, input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, 0, input.LocationId);
            journal.UpdateStatus(input.Status);
            journal.SetJournalTransactionTypeId(input.InventoryTransactionTypeId);

            //update Clearance account 
            var @clearanceAccountItem = await (_journalItemRepository.GetAll()
                                      .Where(u => u.JournalId == journal.Id &&
                                               u.Key == PostingKey.Clearance && u.Identifier == null)).FirstOrDefaultAsync();
            clearanceAccountItem.UpdateJournalItem(tenantId, input.ClearanceAccountId, input.Memo, 0, input.Total);


            //update item receipt 
            var @itemReceipt = await _itemReceiptManager.GetAsync(input.Id, true);

            var getPermission = await GetUserPermissions(userId);
            var IsEdit = getPermission.Where(t => t == AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour).Count();
            var totalHours = (DateTime.Now - itemReceipt.CreationTime).TotalHours;
            if (IsEdit > 0 && totalHours > 48)
            {
                throw new UserFriendlyException(L("ThisTransactionIsOver48HoursCanNotEdit"));
            }
            if (@itemReceipt == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            @itemReceipt.Update(tenantId, ItemReceipt.ReceiveFromStatus.None, null,
                          input.SameAsShippingAddress, input.ShippingAddress,
                          input.BillingAddress, input.Total, null);

            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt);
            CheckErrors(await _journalManager.UpdateAsync(@journal, autoSequence.DocumentType));

            CheckErrors(await _journalItemManager.UpdateAsync(@clearanceAccountItem));
            CheckErrors(await _itemReceiptManager.UpdateAsync(@itemReceipt));


            var @inventoryJournalItems = await (_journalItemRepository.GetAll()
                                  .Where(u => u.JournalId == journal.Id &&
                                           u.Key == PostingKey.Inventory && u.Identifier != null)).ToListAsync();

            var toDeleteItemReceiptItem = itemReceipItems
                                            .Where(u => !input.Items.Any(i => i.Id != null && i.Id == u.Id)).ToList();
            var toDeletejournalItem = inventoryJournalItems
                                            .Where(u => !input.Items.Any(i => u.Identifier != null && i.Id != null && i.Id == u.Identifier)).ToList();


            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }

            var itemBatchNos = await _itemReceiptItemBatchNoRepository.GetAll().Include(s => s.BatchNo).Where(s => itemReceipItems.Any(r => r.Id == s.ItemReceiptItemId)).AsNoTracking().ToListAsync();
            var batchNos = new List<BatchNo>();
            if (toDeleteItemReceiptItem.Any())
            {
                var deleteItemBatchNos = itemBatchNos.Where(s => toDeleteItemReceiptItem.Any(r => r.Id == s.ItemReceiptItemId)).ToList();
                if (deleteItemBatchNos.Any())
                {
                    var batchs = deleteItemBatchNos.GroupBy(s => s.BatchNoId).Select(s => s.FirstOrDefault().BatchNo).ToList();
                    if (batchs.Any()) batchNos.AddRange(batchNos);

                    await _itemReceiptItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
                }
            }


            int indexLot = 0;
            foreach (var c in input.Items)
            {
                indexLot++;
                if (c.ToLotId == null && c.ItemId != null)
                {
                    throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + indexLot.ToString());
                }
                if (c.Id != null) //update
                {
                    var itemReceipItem = itemReceipItems.FirstOrDefault(u => u.Id == c.Id);
                    var journalItem = @inventoryJournalItems.FirstOrDefault(u => u.Identifier == c.Id);
                    if (itemReceipItem != null)
                    {
                        //new
                        itemReceipItem.Update(tenantId, c.ItemId, c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total);
                        itemReceipItem.UpdateLot(c.ToLotId);
                        CheckErrors(await _itemReceiptItemManager.UpdateAsync(itemReceipItem));

                    }
                    if (journalItem != null)
                    {
                        journalItem.UpdateJournalItem(userId, c.InventoryAccountId, c.Description, c.Total, 0);
                        CheckErrors(await _journalItemManager.UpdateAsync(journalItem));
                    }

                    var oldItemBatchs = itemBatchNos.Where(s => s.ItemReceiptItemId == c.Id).ToList();

                    if (c.ItemBatchNos != null && c.ItemBatchNos.Any())
                    {
                        var addItemBatchNos = c.ItemBatchNos.Where(s => !s.Id.HasValue || s.Id == Guid.Empty).Select(s => ItemReceiptItemBatchNo.Create(tenantId, userId, itemReceipItem.Id, s.BatchNoId, s.Qty)).ToList();
                        if (addItemBatchNos.Any())
                        {
                            foreach (var a in addItemBatchNos)
                            {
                                await _itemReceiptItemBatchNoRepository.InsertAsync(a);
                            }
                        }

                        var updateItemBatchNos = oldItemBatchs.Where(s => c.ItemBatchNos.Any(r => r.Id == s.Id)).Select(s =>
                        {
                            var oldItemBatch = s;
                            var newBatch = c.ItemBatchNos.FirstOrDefault(r => r.Id == s.Id);

                            //in case of change date and item is auto generate it will change to use new batch so delete old one if not use
                            if (oldItemBatch.BatchNoId != newBatch.BatchNoId && !batchNos.Any(b => b.Id == oldItemBatch.BatchNoId)) batchNos.Add(oldItemBatch.BatchNo);

                            oldItemBatch.Update(userId, itemReceipItem.Id, newBatch.BatchNoId, newBatch.Qty);
                            return oldItemBatch;
                        }).ToList();

                        if (updateItemBatchNos.Any()) await _itemReceiptItemBatchNoRepository.BulkUpdateAsync(updateItemBatchNos);

                        var deleteItemBatchNos = oldItemBatchs.Where(s => !c.ItemBatchNos.Any(r => r.Id == s.Id)).ToList();
                        if (deleteItemBatchNos.Any())
                        {
                            var batchs = deleteItemBatchNos.Where(s => !batchNos.Any(r => r.Id == s.BatchNoId)).GroupBy(s => s.BatchNoId).Select(s => s.FirstOrDefault().BatchNo).ToList();
                            if (batchs.Any()) batchNos.AddRange(batchs);
                            await _itemReceiptItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
                        }
                    }
                    else if (oldItemBatchs.Any())
                    {
                        var batchs = oldItemBatchs.Where(s => !batchNos.Any(r => r.Id == s.BatchNoId)).GroupBy(s => s.BatchNoId).Select(s => s.FirstOrDefault().BatchNo).ToList();
                        if (batchs.Any()) batchNos.AddRange(batchs);

                        await _itemReceiptItemBatchNoRepository.BulkDeleteAsync(oldItemBatchs);
                    }

                }
                else if (c.Id == null) //create
                {
                    //insert to item Receipt item
                    var @itemReceiptItem = ItemReceiptItem.Create(tenantId, userId, @itemReceipt, c.ItemId,
                                                                 c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total);
                    itemReceiptItem.UpdateLot(c.ToLotId);
                    CheckErrors(await _itemReceiptItemManager.CreateAsync(@itemReceiptItem));

                    var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal,
                        c.InventoryAccountId, c.Description, c.Total, 0, PostingKey.Inventory, itemReceiptItem.Id);
                    CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));

                    if (c.ItemBatchNos != null && c.ItemBatchNos.Any())
                    {
                        var addItemBatchNos = c.ItemBatchNos.Select(s => ItemReceiptItemBatchNo.Create(tenantId, userId, itemReceiptItem.Id, s.BatchNoId, s.Qty)).ToList();
                        foreach (var a in addItemBatchNos)
                        {
                            await _itemReceiptItemBatchNoRepository.InsertAsync(a);
                        }
                    }
                }

            }

            var scheduleItems = input.Items.Select(s => s.ItemId).Concat(toDeleteItemReceiptItem.Select(s => s.ItemId)).Distinct().ToList();

            foreach (var t in toDeleteItemReceiptItem.OrderBy(u => u.Id))
            {
                CheckErrors(await _itemReceiptItemManager.RemoveAsync(t));
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

            if (batchNos.Any())
            {
                var allBatchUse = await GetBatchNoUseByOthers(input.Id, batchNos.Select(s => s.Id).ToList());
                var deleteBatchNos = batchNos.Where(s => !allBatchUse.Contains(s.Id)).ToList();
                if (deleteBatchNos.Any()) await _batchNoRepository.BulkDeleteAsync(deleteBatchNos);
            }


            await SyncItemReceipt(itemReceipt.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, scheduleDate, scheduleItems);
            }

            return new NullableIdDto<Guid>() { Id = @itemReceipt.Id };
        }

        #region import Excel item receipt other
        private ReportOutput GetReportTemplateItemReceiptOther()
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
                        ColumnName = "ReceiptNo",
                        ColumnLength = 130,
                        ColumnTitle = "Receipt No",
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
                        ColumnName = "ReceiveDate",
                        ColumnLength = 200,
                        ColumnTitle = "Receive Date",
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
                        SortOrder = 9,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "UnitCost",
                        ColumnLength = 130,
                        ColumnTitle = "Unit Cost",
                        ColumnType = ColumnType.Money,
                        SortOrder = 10,
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
                        ColumnTitle = "BatchNo/Serial",
                        ColumnType = ColumnType.String,
                        SortOrder = 12,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Expiration",
                        ColumnLength = 250,
                        ColumnTitle = "Expiration",
                        ColumnType = ColumnType.Date,
                        SortOrder = 12,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "InventoryTransactionType",
                        ColumnLength = 250,
                        ColumnTitle = "Inventory Transaction Type",
                        ColumnType = ColumnType.String,
                        SortOrder = 13,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true
                    },

                },
                Groupby = "",
                HeaderTitle = "Item Receipt Other",
                Sortby = "",
            };
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_importExcelItemReceiptOther,
            AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ImportItemReceiptOther)]
        [UnitOfWork(IsDisabled = true)]
        public async Task ImportExcel(FileDto input)
        {
            Tenant tenant = null;
            int? tenatId = AbpSession.TenantId;
            var userId = AbpSession.GetUserId();

            var accounts = new List<ChartOfAccount>();
            var locations = new List<Location>();
            var lots = new List<Lot>();
            var @class = new List<Class>();
            var items = new List<Item>();
            AutoSequence auto = null;
            var batchNos = new List<BatchNo>();
            var inventoryTransactionTypes = new List<JournalTransactionType>();
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenatId))
                {
                    tenant = await GetCurrentTenantAsync();
                    @accounts = await _chartOfAccountRepository.GetAll().AsNoTracking().ToListAsync();
                    @locations = await _locationRepository.GetAll().AsNoTracking().ToListAsync();
                    @lots = await _lotRepository.GetAll().Include(t => t.Location).AsNoTracking().ToListAsync();
                    @class = await _classRepository.GetAll().AsNoTracking().ToListAsync();
                    @items = await _itemRepository.GetAll().AsNoTracking().ToListAsync();
                    auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt);
                    batchNos = await _batchNoRepository.GetAll().AsNoTracking().ToListAsync();
                    inventoryTransactionTypes = await _journalTransactionTypeRepository.GetAll().Where(t => t.InventoryTransactionType == InventoryTransactionType.ItemReceiptOther).AsNoTracking().ToListAsync();

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
                    var receiptNo = worksheet.Cells[2, 3].Value?.ToString();
                    var reference = worksheet.Cells[2, 4].Value?.ToString();
                    var receiveDate = worksheet.Cells[2, 5].Value?.ToString();
                    var locationName = worksheet.Cells[2, 8].Value?.ToString();
                    var invnentoryTransactionTypeName = worksheet.Cells[2, 14].Value?.ToString();
                    var accountId = accounts.Where(s => s.AccountCode == accountName).FirstOrDefault();
                    var locationId = locations.Where(s => s.LocationName == locationName).FirstOrDefault();
                    var classId = @class.Where(s => s.ClassName == className).FirstOrDefault();
                    var inventoryTransactionTypeId = inventoryTransactionTypes.Where(i => i.Name == invnentoryTransactionTypeName).Select(u => u.Id).FirstOrDefault();
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
                    if (inventoryTransactionTypeId == null || inventoryTransactionTypeId == Guid.Empty)
                    {
                        throw new UserFriendlyException(L("TransactionTypeIdNotFound"));
                    }
                    if (invnentoryTransactionTypeName == null)
                    {
                        throw new UserFriendlyException(L("TransactionTypeIdNotFound"));
                    }
                    if (string.IsNullOrWhiteSpace(receiveDate))
                    {
                        throw new UserFriendlyException(L("DateIsRequired"));
                    }
                    //insert to journal
                    var transactionNo = string.Empty;
                    var memo = "Begining Stock";

                    //if (receiptNo != null)
                    //{
                    //    transactionNo = receiptNo;
                    //}
                    if (auto.CustomFormat == true)
                    {
                        if (!string.IsNullOrEmpty(receiptNo))
                        {
                            throw new UserFriendlyException(L("CannotSetReceiptNoCurrentSettingUseAutoNumber"));
                        }
                        var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                                       auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                        transactionNo = newAuto;
                        auto.UpdateLastAutoSequenceNumber(newAuto);
                    }
                    else if (string.IsNullOrEmpty(receiptNo))
                    {
                        throw new UserFriendlyException(L("ReceiptNoCannotBeBlank"));
                    }

                    decimal total = 0;
                    var @entity = Journal.Create(tenant.Id, userId, transactionNo,
                                                    Convert.ToDateTime(receiveDate),
                                                    memo, total, total,
                                                    tenant.CurrencyId.Value, classId.Id, reference, locationId.Id);
                    entity.UpdateStatus(TransactionStatus.Publish);
                    entity.SetJournalTransactionTypeId(inventoryTransactionTypeId);
                    //insert clearance journal item into credit
                    var clearanceJournalItem = JournalItem.CreateJournalItem(tenant.Id, userId, entity.Id,
                        accountId.Id, transactionNo, 0,
                        total, PostingKey.Clearance, null);

                    //insert to item Receipt
                    CAddress billAddress = new CAddress("", "", "", "", "");
                    CAddress shipAddress = new CAddress("", "", "", "", "");
                    var @itemReceipt = ItemReceipt.Create(tenant.Id, userId, ItemReceipt.ReceiveFromStatus.None, null,
                         true, billAddress, shipAddress, total, null);
                    itemReceipt.UpdateTransactionType(InventoryTransactionType.ItemReceiptOther);
                    @entity.UpdateItemReceiptOther(@itemReceipt.Id);

                    var addJournalItems = new List<JournalItem>();
                    var addItemReceiptItems = new List<ItemReceiptItem>();

                    addJournalItems.Add(clearanceJournalItem);

                    var addBatchNos = new List<BatchNo>();
                    var addItemReceiptItemBatchNos = new List<ItemReceiptItemBatchNo>();

                    //loop all rows to insert item receipt items and to journal items
                    for (int i = worksheet.Dimension.Start.Row; i <= worksheet.Dimension.End.Row; i++)
                    {
                        if (i > 1)
                        {
                            var itemCode = worksheet.Cells[i, 6].Value?.ToString();
                            var description = worksheet.Cells[i, 7].Value?.ToString();
                            var qty = Convert.ToDecimal(worksheet.Cells[i, 10].Value?.ToString());
                            var unitcost = Convert.ToDecimal(worksheet.Cells[i, 11].Value?.ToString());
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
                            //insert to item Receipt item
                            var @itemReceiptItem = ItemReceiptItem.Create(
                                                    tenant.Id, userId, @itemReceipt.Id, item.Id,
                                                    description, qty, unitcost, 0, totalItem);
                            @itemReceiptItem.UpdateLot(lot.Id);

                            var expiration = worksheet.Cells[i, 13].Value?.ToString();
                            if (item.TrackExpiration && expiration.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("PleaseEnter", L("ExpirationDate")) + $", Row: {i}");

                            if (item.UseBatchNo || item.TrackSerial || item.TrackExpiration)
                            {
                                var batchNumber = worksheet.Cells[i, 12].Value?.ToString();
                                if (batchNumber.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("PleaseEnter", L("BatchNo")) + $", Row: {i}");

                                var expirationDate = expiration.IsNullOrWhiteSpace() ? (DateTime?)null : Convert.ToDateTime(expiration);
                                if (item.TrackExpiration && !expirationDate.HasValue) throw new UserFriendlyException(L("PleaseEnter", L("ExpirationDate")) + $", Row: {i}");

                                if (item.TrackSerial && qty != 1) throw new UserFriendlyException(L("MustBeEqualTo", $"{L("SerialNo")} {L("Qty")}", 1) + $", Row: {i}");

                                var findBatchNo = batchNos.FirstOrDefault(s => s.Code == batchNumber);
                                if (findBatchNo == null)
                                {
                                    findBatchNo = BatchNo.Create(tenant.Id, userId, batchNumber, item.Id, false, item.TrackExpiration, expirationDate);
                                    addBatchNos.Add(findBatchNo);
                                    batchNos.Add(findBatchNo);
                                }
                                else if (item.TrackExpiration && findBatchNo.ExpirationDate.Value.Date != expirationDate.Value.Date)
                                {
                                    throw new UserFriendlyException(L("IsNotValid", L("ExpirationDate")) + $", Row: {i}");
                                }

                                var itemBatchNo = ItemReceiptItemBatchNo.Create(tenant.Id, userId, itemReceiptItem.Id, findBatchNo.Id, itemReceiptItem.Qty);
                                addItemReceiptItemBatchNos.Add(itemBatchNo);
                            }

                            addItemReceiptItems.Add(@itemReceiptItem);

                            //insert inventory journal item into debit
                            var inventoryJournalItem = JournalItem.CreateJournalItem(tenant.Id, userId, entity.Id, item.InventoryAccountId.Value,
                                                                description, totalItem, 0, PostingKey.Inventory, itemReceiptItem.Id);

                            addJournalItems.Add(inventoryJournalItem);

                        }
                    }

                    entity.UpdateCreditDebit(total, total);
                    clearanceJournalItem.SetDebitCredit(0, total);
                    @itemReceipt.UpdateTotal(total);

                    var scheduleItems = addItemReceiptItems.Select(s => s.ItemId).Distinct().ToList();

                    using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
                    {
                        using (_unitOfWorkManager.Current.SetTenantId(tenant.Id))
                        {
                            CheckErrors(await _autoSequenceManager.UpdateAsync(auto));

                            if (addBatchNos.Any()) await _batchNoRepository.BulkInsertAsync(addBatchNos);

                            await _itemReceiptRepository.BulkInsertAsync(new List<ItemReceipt> { itemReceipt });
                            await _itemReceiptItemRepository.BulkInsertAsync(addItemReceiptItems);

                            if (addItemReceiptItemBatchNos.Any()) await _itemReceiptItemBatchNoRepository.BulkInsertAsync(addItemReceiptItemBatchNos);

                            //CheckErrors(await _journalManager.CreateAsync(@entity));
                            await _journalRepository.BulkInsertAsync(new List<Journal> { entity });
                            await _journalItemRepository.BulkInsertAsync(addJournalItems);

                            await SyncItemReceipt(@itemReceipt.Id);
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



        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_exportExcelItemReceiptOther,
            AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ExportItemReceiptOther)]
        public async Task<FileDto> ExportExcelTamplate()
        {

            var result = new FileDto();
            var sheetName = "Item Receipt Other";

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
                var headerList = GetReportTemplateItemReceiptOther();

                foreach (var i in headerList.ColumnInfo)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);

                    colHeaderTable += 1;
                }
                #endregion Row 1

                result.FileName = $"ItemReceiptOtherTamplate.xlsx";
                result.FileToken = Guid.NewGuid().ToString();
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        #endregion


        #region import excel item receipt other by has account default


        private ReportOutput GetReportTemplateHasAccountDefault()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter                   
                     new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ReceiptNo",
                        ColumnLength = 130,
                        ColumnTitle = "Receipt No",
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
                        ColumnName = "ReceiveDate",
                        ColumnLength = 200,
                        ColumnTitle = "Receive Date",
                        ColumnType = ColumnType.Date,
                        SortOrder = 3,
                        Visible = true,
                        IsRequired = true,
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
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "UnitCost",
                        ColumnLength = 130,
                        ColumnTitle = "Unit Cost",
                        ColumnType = ColumnType.Money,
                        SortOrder = 9,
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
                        ColumnTitle = "BatchNo/Serial",
                        ColumnType = ColumnType.String,
                        SortOrder = 10,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Expiration",
                        ColumnLength = 250,
                        ColumnTitle = "Expiration",
                        ColumnType = ColumnType.Date,
                        SortOrder = 11,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true
                    },
                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "InventoryTransactionType",
                        ColumnLength = 250,
                        ColumnTitle = "Inventory Transaction Type",
                        ColumnType = ColumnType.String,
                        SortOrder = 12,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true
                    },

                },
                Groupby = "",
                HeaderTitle = "Item Receipt Other",
                Sortby = "",
            };
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_importExcelItemReceiptOther,
            AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ImportItemReceiptOther)]
        [UnitOfWork(IsDisabled = true)]
        public async Task ImportExcelHasAccountDefault(FileDto input)
        {
            Tenant tenant = null;
            int? tenatId = AbpSession.TenantId;
            var userId = AbpSession.GetUserId();


            var locations = new List<Location>();
            var lots = new List<Lot>();

            var items = new List<Item>();
            AutoSequence auto = null;
            var batchNos = new List<BatchNo>();
            var inventoryTransactionTypes = new List<JournalTransactionType>();
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenatId))
                {
                    tenant = await GetCurrentTenantAsync();
                    @locations = await _locationRepository.GetAll().AsNoTracking().ToListAsync();
                    @lots = await _lotRepository.GetAll().Include(t => t.Location).AsNoTracking().ToListAsync();
                    @items = await _itemRepository.GetAll().AsNoTracking().ToListAsync();
                    auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt);
                    batchNos = await _batchNoRepository.GetAll().AsNoTracking().ToListAsync();
                    inventoryTransactionTypes = await _journalTransactionTypeRepository.GetAll().Where(t => t.InventoryTransactionType == InventoryTransactionType.ItemReceiptOther).AsNoTracking().ToListAsync();

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
                    var receiptNo = worksheet.Cells[2, 1].Value?.ToString();
                    var reference = worksheet.Cells[2, 2].Value?.ToString();
                    var receiveDate = worksheet.Cells[2, 3].Value?.ToString();
                    var locationName = worksheet.Cells[2, 6].Value?.ToString();
                    var inventoryTransactionTypeName = worksheet.Cells[2, 12].Value?.ToString();
                    var accountId = tenant.ItemIssueOtherId;
                    var locationId = locations.Where(s => s.LocationName == locationName).FirstOrDefault();
                    var classId = tenant.ClassId;
                    var inventoryTransactionTypeId = inventoryTransactionTypes.Where(s => s.Name == inventoryTransactionTypeName).Select(t => t.Id).FirstOrDefault();
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
                    if (inventoryTransactionTypeId == null || inventoryTransactionTypeId == Guid.Empty)
                    {
                        throw new UserFriendlyException(L("TransactionTypeIdNotFound"));
                    }
                    if (string.IsNullOrWhiteSpace(receiveDate))
                    {
                        throw new UserFriendlyException(L("DateIsRequired"));
                    }
                    //insert to journal
                    var transactionNo = string.Empty;
                    var memo = "Begining Stock";

                    //if (receiptNo != null)
                    //{
                    //    transactionNo = receiptNo;
                    //}
                    if (auto.CustomFormat == true)
                    {
                        if (!string.IsNullOrEmpty(receiptNo))
                        {
                            throw new UserFriendlyException(L("CannotSetReceiptNoCurrentSettingUseAutoNumber"));
                        }
                        var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                                       auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                        transactionNo = newAuto;
                        auto.UpdateLastAutoSequenceNumber(newAuto);
                    }
                    else if (string.IsNullOrEmpty(receiptNo))
                    {
                        throw new UserFriendlyException(L("ReceiptNoCannotBeBlank"));
                    }

                    decimal total = 0;
                    var @entity = Journal.Create(tenant.Id, userId, transactionNo,
                                                    Convert.ToDateTime(receiveDate),
                                                    memo, total, total,
                                                    tenant.CurrencyId.Value, classId, reference, locationId.Id);
                    entity.UpdateStatus(TransactionStatus.Publish);
                    entity.SetJournalTransactionTypeId(inventoryTransactionTypeId);
                    //insert clearance journal item into credit
                    var clearanceJournalItem = JournalItem.CreateJournalItem(tenant.Id, userId, entity.Id,
                        accountId.Value, transactionNo, 0,
                        total, PostingKey.Clearance, null);

                    //insert to item Receipt
                    CAddress billAddress = new CAddress("", "", "", "", "");
                    CAddress shipAddress = new CAddress("", "", "", "", "");
                    var @itemReceipt = ItemReceipt.Create(tenant.Id, userId, ItemReceipt.ReceiveFromStatus.None, null,
                         true, billAddress, shipAddress, total, null);
                    itemReceipt.UpdateTransactionType(InventoryTransactionType.ItemReceiptOther);
                    @entity.UpdateItemReceiptOther(@itemReceipt.Id);

                    var addJournalItems = new List<JournalItem>();
                    var addItemReceiptItems = new List<ItemReceiptItem>();

                    addJournalItems.Add(clearanceJournalItem);

                    var addBatchNos = new List<BatchNo>();
                    var addItemReceiptItemBatchNos = new List<ItemReceiptItemBatchNo>();

                    //loop all rows to insert item receipt items and to journal items
                    for (int i = worksheet.Dimension.Start.Row; i <= worksheet.Dimension.End.Row; i++)
                    {
                        if (i > 1)
                        {
                            var itemCode = worksheet.Cells[i, 4].Value?.ToString();
                            var description = worksheet.Cells[i, 5].Value?.ToString();
                            var qty = Convert.ToDecimal(worksheet.Cells[i, 8].Value?.ToString());
                            var unitcost = Convert.ToDecimal(worksheet.Cells[i, 9].Value?.ToString());
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
                            //insert to item Receipt item
                            var @itemReceiptItem = ItemReceiptItem.Create(
                                                    tenant.Id, userId, @itemReceipt.Id, item.Id,
                                                    description, qty, unitcost, 0, totalItem);
                            @itemReceiptItem.UpdateLot(lot.Id);

                            var expiration = worksheet.Cells[i, 11].Value?.ToString();
                            if (item.TrackExpiration && expiration.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("PleaseEnter", L("ExpirationDate")) + $", Row: {i}");

                            if (item.UseBatchNo || item.TrackSerial || item.TrackExpiration)
                            {
                                var batchNumber = worksheet.Cells[i, 10].Value?.ToString();
                                if (batchNumber.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("PleaseEnter", L("BatchNo")) + $", Row: {i}");

                                var expirationDate = expiration.IsNullOrWhiteSpace() ? (DateTime?)null : Convert.ToDateTime(expiration);
                                if (item.TrackExpiration && !expirationDate.HasValue) throw new UserFriendlyException(L("PleaseEnter", L("ExpirationDate")) + $", Row: {i}");

                                if (item.TrackSerial && qty != 1) throw new UserFriendlyException(L("MustBeEqualTo", $"{L("SerialNo")} {L("Qty")}", 1) + $", Row: {i}");

                                var findBatchNo = batchNos.FirstOrDefault(s => s.Code == batchNumber);
                                if (findBatchNo == null)
                                {
                                    findBatchNo = BatchNo.Create(tenant.Id, userId, batchNumber, item.Id, false, item.TrackExpiration, expirationDate);
                                    addBatchNos.Add(findBatchNo);
                                    batchNos.Add(findBatchNo);
                                }
                                else if (item.TrackExpiration && findBatchNo.ExpirationDate.Value.Date != expirationDate.Value.Date)
                                {
                                    throw new UserFriendlyException(L("IsNotValid", L("ExpirationDate")) + $", Row: {i}");
                                }

                                var itemBatchNo = ItemReceiptItemBatchNo.Create(tenant.Id, userId, itemReceiptItem.Id, findBatchNo.Id, itemReceiptItem.Qty);
                                addItemReceiptItemBatchNos.Add(itemBatchNo);
                            }

                            addItemReceiptItems.Add(@itemReceiptItem);

                            //insert inventory journal item into debit
                            var inventoryJournalItem = JournalItem.CreateJournalItem(tenant.Id, userId, entity.Id, item.InventoryAccountId.Value,
                                                                description, totalItem, 0, PostingKey.Inventory, itemReceiptItem.Id);

                            addJournalItems.Add(inventoryJournalItem);

                        }
                    }

                    entity.UpdateCreditDebit(total, total);
                    clearanceJournalItem.SetDebitCredit(0, total);
                    @itemReceipt.UpdateTotal(total);

                    var scheduleItems = addItemReceiptItems.Select(s => s.ItemId).Distinct().ToList();

                    using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
                    {
                        using (_unitOfWorkManager.Current.SetTenantId(tenant.Id))
                        {
                            CheckErrors(await _autoSequenceManager.UpdateAsync(auto));

                            if (addBatchNos.Any()) await _batchNoRepository.BulkInsertAsync(addBatchNos);

                            await _itemReceiptRepository.BulkInsertAsync(new List<ItemReceipt> { itemReceipt });
                            await _itemReceiptItemRepository.BulkInsertAsync(addItemReceiptItems);

                            if (addItemReceiptItemBatchNos.Any()) await _itemReceiptItemBatchNoRepository.BulkInsertAsync(addItemReceiptItemBatchNos);

                            //CheckErrors(await _journalManager.CreateAsync(@entity));
                            await _journalRepository.BulkInsertAsync(new List<Journal> { entity });
                            await _journalItemRepository.BulkInsertAsync(addJournalItems);

                            await SyncItemReceipt(@itemReceipt.Id);
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



        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_exportExcelItemReceiptOther,
            AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ExportItemReceiptOther)]
        public async Task<FileDto> ExportExcelTamplateHasAccountDefault()
        {

            var result = new FileDto();
            var sheetName = "Item Receipt Other";

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
                var headerList = GetReportTemplateHasAccountDefault();

                foreach (var i in headerList.ColumnInfo)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true, 0, i.IsRequired);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);

                    colHeaderTable += 1;
                }
                #endregion Row 1

                result.FileName = $"ItemReceiptOtherTamplate.xlsx";
                result.FileToken = Guid.NewGuid().ToString();
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        #endregion

    }
}
