using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
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
using CorarlERP.InventoryCalculationItems;
using CorarlERP.InventoryCalculationJobSchedules;
using CorarlERP.ItemReceiptAdjustments.Dto;
using CorarlERP.ItemReceiptProducts.Dto;
using CorarlERP.ItemReceipts;
using CorarlERP.ItemReceipts.Dto;
using CorarlERP.Items;
using CorarlERP.Items.Dto;
using CorarlERP.Journals;
using CorarlERP.JournalTransactionTypes;
using CorarlERP.Locations.Dto;
using CorarlERP.Locks;
using CorarlERP.Lots.Dto;
using CorarlERP.ProductionProcesses;
using CorarlERP.Productions;
using Hangfire.States;
using IdentityServer4.Validation;
using Microsoft.EntityFrameworkCore;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ItemReceiptProducts
{
    public class ItemReceiptProdcutionAppService : CorarlERPAppServiceBase, IItemReceiptProductionAppService
    {
        private readonly IItemManager _itemManager;
        private readonly IRepository<Item, Guid> _itemRepository;

        private readonly IItemReceiptItemManager _itemReceiptItemManager;
        private readonly IItemReceiptManager _itemReceiptManager;

        private readonly IRepository<ItemReceipt, Guid> _itemReceiptRepository;
        private readonly IRepository<ItemReceiptItem, Guid> _itemReceiptItemRepository;

        private readonly IJournalManager _journalManager;
        private readonly IRepository<Journal, Guid> _journalRepository;

        private readonly IJournalItemManager _journalItemManager;
        private readonly IRepository<JournalItem, Guid> _journalItemRepository;

        private readonly IChartOfAccountManager _chartOfAccountManager;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;

        private readonly IProductionManager _productionOrderManager;
        private readonly IRepository<Production, Guid> _productionOrderRepository;
        private readonly IRepository<ProductionProcess, long> _productionProcessRepository;

        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly ICorarlRepository<BatchNo, Guid> _batchNoRepository;
        private readonly ICorarlRepository<ItemReceiptItemBatchNo, Guid> _itemReceiptItemBatchNoRepository;

        private readonly IRepository<Lock, long> _lockRepository;
        private readonly IInventoryCalculationItemManager _inventoryCalculationItemManager;
        private readonly IJournalTransactionTypeManager _journalTransactionTypeManager;
        public ItemReceiptProdcutionAppService(
            IJournalManager journalManager,
            IJournalItemManager journalItemManager,
            IChartOfAccountManager chartOfAccountManager,
            IInventoryCalculationItemManager inventoryCalculationItemManager,
            ItemReceiptManager itemReceiptManager,
            ItemReceiptItemManager itemReceiptItemManager,
            ItemManager itemManager,
            IRepository<Item, Guid> itemRepository,
            IRepository<Journal, Guid> journalRepository,
            IRepository<JournalItem, Guid> journalItemRepository,
            IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
            IRepository<ItemReceipt, Guid> itemReceiptRepository,
            IRepository<ItemReceiptItem, Guid> itemReceiptItemRepository,
            IProductionManager productionOrderManager,
            IRepository<Production, Guid> productionOrderRepository,
            IRepository<ProductionProcess, long> productionProcessRepository,
            IRepository<AutoSequence, Guid> autoSequenceRepository,
            ICorarlRepository<BatchNo, Guid> batchNoRepository,
            ICorarlRepository<ItemReceiptItemBatchNo, Guid> itemReceiptItemBatchNoRepository,
            IRepository<Lock, long> lockRepository,
            IJournalTransactionTypeManager journalTransactionTypeManager,
            IRepository<AccountCycle,long> accountCycleRepository,
            IAutoSequenceManager autoSequenceManager): base(accountCycleRepository,null,null)
        {
            _journalManager = journalManager;
            _journalManager.SetJournalType(JournalType.ItemReceiptProduction);
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
            _productionOrderManager = productionOrderManager;
            _productionOrderRepository = productionOrderRepository;
            _autoSequenceManager = autoSequenceManager;
            _autoSequenceRepository = autoSequenceRepository;
            _lockRepository = lockRepository;
            _inventoryCalculationItemManager = inventoryCalculationItemManager;
            _batchNoRepository = batchNoRepository;
            _productionProcessRepository= productionProcessRepository;
            _itemReceiptItemBatchNoRepository = itemReceiptItemBatchNoRepository;
            _journalTransactionTypeManager = journalTransactionTypeManager;
        }

        private async Task ValidateAddBatchNo(CreateItemReceiptProductionInput input)
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

            ProductionProcess productionProccess = null;
            if (input.ProductionProcessId.HasValue) productionProccess = await _productionProcessRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == input.ProductionProcessId);


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

                        if (productionProccess != null && productionProccess.UseStandard)
                        {
                            batch.IsStandard = true;
                            if (formula.StandardPrePos == BatchNoFormulaPrePos.Prefix)
                            {
                                batch.BatchNumber = $"{formula.PrePosCode}-{code}-{input.Date.ToString(formula.DateFormat)}";
                            }
                            else if (formula.StandardPrePos == BatchNoFormulaPrePos.Postfix)
                            {
                                batch.BatchNumber = $"{code}-{input.Date.ToString(formula.DateFormat)}-{formula.PrePosCode}";
                            }
                        }
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


        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CreateProductionOrder,
            AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptProduction)]
        public async Task<NullableIdDto<Guid>> Create(CreateItemReceiptProductionInput input)
        {

            if(input.IsConfirm == false)
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
            var @entity = Journal.Create(tenantId, userId, input.ReceiptNo, input.Date,
                        input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference,input.LocationId);
            entity.UpdateStatus(input.Status);
            #region journal transaction type 
            var transactionTypeId = await _journalTransactionTypeManager.GetJournalTransactionTypeId(InventoryTransactionType.ItemReceiptProduction);
            entity.SetJournalTransactionTypeId(transactionTypeId);
            #endregion
            //insert clearance journal item into credit
            var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, input.ClearanceAccountId, input.Memo, 0,
                                    input.Total, PostingKey.Clearance, null);

            //insert to item Receipt
            var @itemReceipt = ItemReceipt.Create(tenantId, userId, input.ReceiveFrom, null,
                            input.SameAsShippingAddress, input.ShippingAddress, input.BillingAddress, input.Total, input.ProductionProcessId);

            itemReceipt.UpdateTransactionType(InventoryTransactionType.ItemReceiptProduction);
            @entity.UpdateItemReceiptProduction(@itemReceipt);

            if (input.ReceiveFrom == ItemReceipt.ReceiveFromStatus.ProductionOrder)
            {
                var production = await _productionOrderRepository.GetAll().Where(u => u.Id == input.ProductionOrderId).FirstOrDefaultAsync();
                // update received status 
                if (production == null)
                {
                    throw new UserFriendlyException(L("NoProductionOrder"));
                }
                if (input.Status == TransactionStatus.Publish)
                {
                    production.UpdateShipedStatus(TransferStatus.ReceiveAll);
                }
                CheckErrors(await _productionOrderManager.UpdateAsync(production));

                itemReceipt.UpdateProductionOrderId(input.ProductionOrderId.Value);
            }
            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));
            CheckErrors(await _journalItemManager.CreateAsync(clearanceJournalItem));
            CheckErrors(await _itemReceiptManager.CreateAsync(@itemReceipt));

            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }
            int index = 0;
            foreach (var i in input.Items)
            {
                index++;
                if ( i.LotId == 0 && i.ItemId != null)
                {
                    throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + index.ToString());
                }
                //insert to item Receipt item
                var @itemReceiptItem = ItemReceiptItem.Create(tenantId, userId, @itemReceipt, i.ItemId,
                                                       i.Description, i.Qty, i.UnitCost, i.DiscountRate, i.Total);
                if (i.FinishItemId != null)
                {
                    @itemReceiptItem.UpdateFinishItemId(i.FinishItemId.Value);
                   
                }
                itemReceiptItem.UpdateLot(i.LotId);
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
        public async Task<ItemReceiptProductionDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            await TurnOffTenantFilterIfDebug();

            var @journal = await _journalRepository
                                .GetAll()
                                .Include(u => u.ItemReceipt)                               
                                .Include(u => u.ItemReceipt.ProductionProcess)
                                .Include(u => u.Class)
                                .Include(u => u.Location)
                                .Include(u => u.Currency)
                                .Include(u => u.ItemReceipt.ShippingAddress)
                                .Include(u => u.ItemReceipt.BillingAddress)
                                .Include(u=>u.JournalTransactionType)
                                .AsNoTracking()
                                .Where(u => u.JournalType == JournalType.ItemReceiptProduction && u.ItemReceiptId == input.Id)
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
                new ItemReceiptItemProductionDetailOutput()
                {

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
                    FirnishItemId = rItem.FinishItemId,
                    ProductionOrderNo = rItem.FinishItems.Production.ProductionNo,
                    LotId = rItem.LotId,
                    LotDetail = ObjectMapper.Map<LotSummaryOutput>(rItem.Lot),
                    UseBatchNo = rItem.Item.UseBatchNo,
                    AutoBatchNo = rItem.Item.AutoBatchNo,
                    TrackSerial = rItem.Item.TrackSerial,
                    TrackExpiration = rItem.Item.TrackExpiration,
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

            var result = ObjectMapper.Map<ItemReceiptProductionDetailOutput>(journal.ItemReceipt);
            result.Date = journal.Date;
            result.ReceiveNo = journal.JournalNo;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.Reference = journal.Reference;
            result.ItemReceiptItemProductions = itemReceiptItems;
            result.ClearanceAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(clearanceAccount);
            result.ClearanceAccountId = clearanceAccount.Id;
            result.Memo = journal.Memo;
            result.StatusCode = journal.Status;
            result.Total = journal.Credit;
            result.StatusName = journal.Status.ToString();
            result.LocationId = journal.LocationId.Value;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            result.ProductionOrderId = journal.ItemReceipt.ProductionOrderId;
            result.TransactionTypeName = journal.JournalTransactionType?.Name;
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_Update, 
        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Update,
        AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour)]
        public async Task<NullableIdDto<Guid>> Update(UpdateItemReceiptProductionInput input)
        {
            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }

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
                              .Include(t => t.ItemReceipt)
                              .Include(t => t.ItemReceipt.ProductionOrder)
                              .Where(u => u.JournalType == JournalType.ItemReceiptProduction && u.ItemReceiptId == input.Id)
                              .FirstOrDefaultAsync();

            if (@journal == null || @journal.ItemReceipt == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            //update item receipt 
            var @itemReceipt = @journal.ItemReceipt;
            //validate EditBy 48 hours
            var getPermission = await GetUserPermissions(userId);
            var IsEdit = getPermission.Where(t => t == AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour).Count();
            var totalHours = (DateTime.Now - itemReceipt.CreationTime).TotalHours;
            if (IsEdit > 0 && totalHours > 48)
            {
                throw new UserFriendlyException(L("ThisTransactionIsOver48HoursCanNotEdit"));
            }
            await CheckClosePeriod(journal.Date, input.Date);

            await ValidateAddBatchNo(input);

            //Update Item Receipt Item and Journal Item
            var itemReceipItems = await _itemReceiptItemRepository.GetAll().Where(u => u.ItemReceiptId == input.Id).ToListAsync();

            var scheduleDate = input.Date > journal.Date ? journal.Date : input.Date;

            journal.Update(tenantId, input.ReceiptNo, input.Date, input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, 0,input.LocationId);
            journal.UpdateStatus(input.Status);
            //update Clearance account 
            var @clearanceAccountItem = await (_journalItemRepository.GetAll()
                                      .Where(u => u.JournalId == journal.Id &&
                                               u.Key == PostingKey.Clearance && u.Identifier == null)).FirstOrDefaultAsync();
            clearanceAccountItem.UpdateJournalItem(tenantId, input.ClearanceAccountId, input.Memo, 0, input.Total);
            
            if (@itemReceipt.ProductionOrderId != null && @itemReceipt.ProductionOrder != null && @itemReceipt.ProductionOrder.ConvertToIssueAndReceipt == true)
            {
                throw new UserFriendlyException(L("AutoConvertMessage"));
            }
            @itemReceipt.Update(tenantId, input.ReceiveFrom, null, input.SameAsShippingAddress, 
                            input.ShippingAddress, input.BillingAddress, input.Total, input.ProductionProcessId);

            #region update receive from 
            // update if come from Prouction order 
            if (input.ReceiveFrom == ItemReceipt.ReceiveFromStatus.ProductionOrder && input.ProductionOrderId != null)
            {
                if (@itemReceipt.ProductionOrderId != null && @itemReceipt.ProductionOrderId != input.ProductionOrderId)
                {
                    var productionOld = await _productionOrderRepository.GetAll().Where(u => u.Id == @itemReceipt.ProductionOrderId).FirstOrDefaultAsync();
                    CheckErrors(await _productionOrderManager.UpdateAsync(productionOld));
                }

                var production = await _productionOrderRepository.GetAll().Where(u => u.Id == input.ProductionOrderId).FirstOrDefaultAsync();
                // update received status 
                if (production == null)
                {
                    throw new UserFriendlyException(L("NoProductionOrder"));
                }
                if (input.Status == TransactionStatus.Publish)
                {
                    production.UpdateShipedStatus(TransferStatus.ReceiveAll);
                }
                CheckErrors(await _productionOrderManager.UpdateAsync(production));
                @itemReceipt.UpdateProductionOrderId(input.ProductionOrderId.Value);

            }
            else // in some case if user switch from receive from Prouction order to other so update Prouction of field item issue transfer order id to null
            {
                var production = await _productionOrderRepository.GetAll().Where(u => u.Id == @itemReceipt.ProductionOrderId).FirstOrDefaultAsync();
                if (production != null)
                {
                    production.UpdateShipedStatus(TransferStatus.ShipAll);
                    CheckErrors(await _productionOrderManager.UpdateAsync(production));
                }
                @itemReceipt.UpdateProductionOrderId(null);
            }
            #endregion


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


            int index = 0;
            foreach (var c in input.Items)
            {
                index++;
                if (c.LotId == 0 && c.ItemId != null)
                {
                    throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + index.ToString());
                }
                if (c.Id != null) //update
                {
                    var itemReceipItem = itemReceipItems.FirstOrDefault(u => u.Id == c.Id);
                    var journalItem = @inventoryJournalItems.FirstOrDefault(u => u.Identifier == c.Id);
                    if (itemReceipItem != null)
                    {
                        //new
                        itemReceipItem.Update(tenantId, c.ItemId, c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total);
                        itemReceipItem.UpdateLot(c.LotId);
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
                    if (c.FinishItemId != null)
                    {
                        @itemReceiptItem.UpdateFinishItemId(c.FinishItemId.Value);
                       
                    }
                    itemReceiptItem.UpdateLot(c.LotId);
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

        //[AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_Delete)]
        //public async Task Delete(EntityDto<Guid> input)
        //{
        //    var @jounal = await _journalRepository.GetAll()
        //        .Include(u => u.ItemReceipt)
        //        .Include(u => u.ItemReceipt.ShippingAddress)
        //        .Include(u => u.ItemReceipt.BillingAddress)
        //        .Where(u => u.JournalType == JournalType.ItemReceiptProduction && u.ItemReceiptId == input.Id)
        //        .FirstOrDefaultAsync();

        //    //query get item Receipt 
        //    var @entity = @jounal.ItemReceipt;
        //    if (entity == null)
        //    {
        //        throw new UserFriendlyException(L("RecordNotFound"));
        //    }
        //    @jounal.UpdateItemReceiptProduction(null);


        //    //update receive status of Prouction order to pending 
        //    if (jounal.JournalType == JournalType.ItemReceiptProduction)
        //    {
        //        var production = await _productionOrderRepository.GetAll().Where(u => u.Id == jounal.ItemIssue.ProductionOrderId).FirstOrDefaultAsync();
        //        // update received status 
        //        if (production != null)
        //        {
        //            production.UpdateShipedStatus(TransferStatus.ShipAll);
        //            CheckErrors(await _productionOrderManager.UpdateAsync(production));
        //        }
        //    }


        //    //query get journal item and delete
        //    var @jounalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == jounal.Id).ToListAsync();
        //    foreach (var ji in jounalItems)
        //    {
        //        CheckErrors(await _journalItemManager.RemoveAsync(ji));
        //    }

        //    CheckErrors(await _journalManager.RemoveAsync(@jounal));

        //    //query get item receipt item and delete 
        //    var @itemReceiptItems = await _itemReceiptItemRepository.GetAll()
        //        .Where(u => u.ItemReceiptId == entity.Id).ToListAsync();

        //    foreach (var iri in @itemReceiptItems)
        //    {

        //        CheckErrors(await _itemReceiptItemManager.RemoveAsync(iri));
        //    }

        //    CheckErrors(await _itemReceiptManager.RemoveAsync(entity));
        //}
        
    }
}
