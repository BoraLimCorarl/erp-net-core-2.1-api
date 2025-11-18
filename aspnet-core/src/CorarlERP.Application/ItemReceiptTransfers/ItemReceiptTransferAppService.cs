using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
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
using CorarlERP.InventoryCalculationItems;
using CorarlERP.InventoryCalculationJobSchedules;
using CorarlERP.InventoryTransactionItems;
using CorarlERP.ItemIssues;
using CorarlERP.ItemIssueTransfers.Dto;
using CorarlERP.ItemReceipts;
using CorarlERP.ItemReceiptTransfers.Dto;
using CorarlERP.Items;
using CorarlERP.Items.Dto;
using CorarlERP.Journals;
using CorarlERP.Journals.Dto;
using CorarlERP.JournalTransactionTypes;
using CorarlERP.Locations.Dto;
using CorarlERP.Locks;
using CorarlERP.Lots.Dto;
using CorarlERP.TransferOrders;
using Hangfire.States;
using Microsoft.EntityFrameworkCore;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ItemReceiptTransfers
{
    public class ItemReceiptTransferAppService : CorarlERPAppServiceBase, IItemReceiptTransferAppService
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

        private readonly ITransferOrderManager _transferOrderManager;
        private readonly IRepository<TransferOrder, Guid> _transferOrderRepository;

        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly IRepository<Lock, long> _lockRepository;
        private readonly ICorarlRepository<InventoryTransactionItem, Guid> _inventoryTransactionItemRepository;
        private readonly IInventoryCalculationItemManager _inventoryCalculationItemManager;
        private readonly ICorarlRepository<BatchNo, Guid> _batchNoRepository;
        private readonly ICorarlRepository<ItemReceiptItemBatchNo, Guid> _itemReceiptItemBatchNoRepository;
        private readonly IJournalTransactionTypeManager _journalTransactionTypeManager;
        public ItemReceiptTransferAppService(
            ICorarlRepository<InventoryTransactionItem, Guid> inventoryTransactionItemRepository,
            IInventoryCalculationItemManager inventoryCalculationItemManager,
            IJournalManager journalManager,
            IJournalItemManager journalItemManager,
            IChartOfAccountManager chartOfAccountManager,
            ItemReceiptManager itemReceiptManager,
            ItemReceiptItemManager itemReceiptItemManager,
            ItemManager itemManager,
            IRepository<Item, Guid> itemRepository,
            IRepository<Journal, Guid> journalRepository,
            IRepository<JournalItem, Guid> journalItemRepository,
            IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
            IRepository<ItemReceipt, Guid> itemReceiptRepository,
            IRepository<ItemReceiptItem, Guid> itemReceiptItemRepository,
            ITransferOrderManager transferOrderManager,
            IRepository<TransferOrder, Guid> transferOrderRepository,
            IRepository<AutoSequence, Guid> autoSequenceRepository,
            IRepository<Lock, long> lockRepository,
            ICorarlRepository<BatchNo, Guid> batchNoRepository,
            ICorarlRepository<ItemReceiptItemBatchNo, Guid> itemReceiptItemBatchNoRepository,
            IRepository<AccountCycle,long> accountCycleRepository,
            IJournalTransactionTypeManager journalTransactionTypeManager,
            IAutoSequenceManager autoSequenceManager) : base(accountCycleRepository,null,null)
        {
            _journalManager = journalManager;
            _journalManager.SetJournalType(JournalType.ItemReceiptTransfer);
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
            _transferOrderManager = transferOrderManager;
            _transferOrderRepository = transferOrderRepository;
            _autoSequenceManager = autoSequenceManager;
            _autoSequenceRepository = autoSequenceRepository;
            _lockRepository = lockRepository;
            _inventoryTransactionItemRepository = inventoryTransactionItemRepository;
            _inventoryCalculationItemManager = inventoryCalculationItemManager;
            _batchNoRepository = batchNoRepository;
            _itemReceiptItemBatchNoRepository = itemReceiptItemBatchNoRepository;
            _journalTransactionTypeManager = journalTransactionTypeManager;
        }

        private async Task ValidateBatchNo(CreateItemReceiptTransferInput input)
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


        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CreateTransfers,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptTransfer)]
        public async Task<NullableIdDto<Guid>> Create(CreateItemReceiptTransferInput input)
        {

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

            await ValidateBatchNo(input);

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt);

            if (auto != null && auto.CustomFormat == true)
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
                                        input.Memo, input.Total, input.Total, input.CurrencyId, 
                                        input.ClassId, input.Reference,input.LocationId);
            entity.UpdateStatus(input.Status);
            #region journal transaction type 
            var transactionTypeId = await _journalTransactionTypeManager.GetJournalTransactionTypeId(InventoryTransactionType.ItemReceiptTransfer);
            entity.SetJournalTransactionTypeId(transactionTypeId);
            #endregion
            //insert clearance journal item into credit
            var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, input.ClearanceAccountId,
                                        input.Memo, 0, input.Total, PostingKey.Clearance, null);

            //insert to item Receipt
            var @itemReceipt = ItemReceipt.Create(tenantId, userId, input.ReceiveFrom, null, 
                            input.SameAsShippingAddress, input.ShippingAddress, input.BillingAddress, input.Total, null);

            itemReceipt.UpdateTransactionType(InventoryTransactionType.ItemReceiptTransfer);
            @entity.UpdateItemReceiptTransfer(@itemReceipt);

            if (input.ReceiveFrom == ItemReceipt.ReceiveFromStatus.TransferOrder)
            {
                var @transfer = await _transferOrderRepository.GetAll().Where(u => u.Id == input.TransferOrderId).FirstOrDefaultAsync();
                // update received status 
                if (@transfer == null)
                {
                    throw new UserFriendlyException(L("NoTransferOrder"));
                }
                if (input.Status == TransactionStatus.Publish)
                {
                    @transfer.UpdateShipedStatus(TransferStatus.ReceiveAll);
                }
                CheckErrors(await _transferOrderManager.UpdateAsync(@transfer));

                itemReceipt.UpdateTransferOrderId(input.TransferOrderId.Value);
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
                if (i.LotId == null && i.ItemId != null)
                {
                    throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + index.ToString());
                }
                //insert to item Receipt item
                var @itemReceiptItem = ItemReceiptItem.Create(tenantId, userId, @itemReceipt, i.ItemId,
                                                       i.Description, i.Qty, i.UnitCost, i.DiscountRate, i.Total);
                
                if (i.TransferOrderItemId != null)
                {
                    @itemReceiptItem.UpdateTransferOrderItemId(i.TransferOrderItemId.Value);
                   
                }
                itemReceiptItem.UpdateLot(i.LotId);
                CheckErrors(await _itemReceiptItemManager.CreateAsync(@itemReceiptItem));
                //insert inventory journal item into debit
                var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, i.InventoryAccountId,
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
        public async Task<ItemReceiptTransferDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            await TurnOffTenantFilterIfDebug();

            var @journal = await _journalRepository
                                .GetAll()
                                .Include(u => u.ItemReceipt)                                
                                .Include(u => u.Class)
                                .Include(u => u.Currency)
                                .Include(u=>u.Location)
                                .Include(u => u.ItemReceipt.ShippingAddress)
                                .Include(u => u.ItemReceipt.BillingAddress)
                                .Include(u=>u.JournalTransactionType)
                                .AsNoTracking()
                                .Where(u => u.JournalType == JournalType.ItemReceiptTransfer && u.ItemReceiptId == input.Id)
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


            var itemReceiptItems = await (from rItem in _itemReceiptItemRepository.GetAll()
                                                        .Include(u => u.Item)
                                                        .Include(u => u.Lot)
                                                        .Where(u => u.ItemReceiptId == input.Id)
                                                        .AsNoTracking()
                                          join jItem in _journalItemRepository.GetAll()
                                                      .Include(u => u.Account)
                                                      .Where(u => u.Identifier.HasValue)
                                                      .Where(u => u.Key == PostingKey.Inventory)
                                                      .AsNoTracking()
                                          on rItem.Id equals jItem.Identifier

                                          join i in _inventoryTransactionItemRepository.GetAll()
                                                  .Where(s => s.JournalType == JournalType.ItemReceiptTransfer)
                                                  .Where(s => s.TransferOrProductionId.HasValue && s.TransferOrProductionItemId.HasValue)
                                                  .Where(s => s.TransactionId == input.Id)
                                                  .AsNoTracking()
                                          on rItem.Id equals i.Id
                                          into cs
                                          from c in cs.DefaultIfEmpty()
                                          select
                                          new ItemReceiptItemTransferDetailOutput()
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
                                              Total = c != null ? c.LineCost : rItem.Total,
                                              UnitCost = c != null ? c.UnitCost : rItem.UnitCost,
                                              TransferOrderItemId = rItem.TransferOrderItemId,
                                              TransferOrderNo = rItem.TransferOrderItem.TransferOrder.TransferNo,
                                              UseBatchNo = rItem.Item.UseBatchNo,
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

            var result = ObjectMapper.Map<ItemReceiptTransferDetailOutput>(journal.ItemReceipt);
            result.Date = journal.Date;
            result.ReceiveNo = journal.JournalNo;
            result.ClassId = journal.ClassId;
            result.LocationId = journal.LocationId.Value;
            result.CurrencyId = journal.CurrencyId;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.Reference = journal.Reference;
            result.ItemReceiptItemTransfers = itemReceiptItems;
            result.ClearanceAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(clearanceAccount);
            result.ClearanceAccountId = clearanceAccount.Id;
            result.Memo = journal.Memo;
            result.StatusCode = journal.Status;
            //result.Total = journal.Credit;
            result.StatusName = journal.Status.ToString();
            result.LocationId = journal.LocationId.Value;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);

            //get total from inventory transaction item cache table
            result.Total = itemReceiptItems.Sum(s => s.Total);
            result.TransactionTypeName = journal.JournalTransactionType?.Name;

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_Update,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Update,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour)]
        public async Task<NullableIdDto<Guid>> Update(UpdateItemReceiptTransferInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

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

            // update Journal 
            var @journal = await _journalRepository
                              .GetAll()
                              .Include(t => t.ItemReceipt)
                              .Include(t => t.ItemReceipt.TransferOrder)
                              .Where(u => u.JournalType == JournalType.ItemReceiptTransfer && u.ItemReceiptId == input.Id)
                              .FirstOrDefaultAsync();

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


            if (@journal == null || @journal.ItemReceipt == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            await CheckClosePeriod(journal.Date, input.Date);

            var scheduleDate = input.Date > journal.Date ? journal.Date : input.Date;

            journal.Update(tenantId, input.ReceiptNo, input.Date, input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, 0,input.LocationId);
            journal.UpdateStatus(input.Status);
            //update Clearance account 
            var @clearanceAccountItem = await (_journalItemRepository.GetAll()
                                      .Where(u => u.JournalId == journal.Id &&
                                               u.Key == PostingKey.Clearance && u.Identifier == null)).FirstOrDefaultAsync();
            clearanceAccountItem.UpdateJournalItem(tenantId, input.ClearanceAccountId, input.Memo, 0, input.Total);

            

            if (@itemReceipt.TransferOrderId != null && @itemReceipt.TransferOrder != null && @itemReceipt.TransferOrder.ConvertToIssueAndReceiptTransfer == true)
            {
                throw new UserFriendlyException(L("AutoConvertMessage"));
            }

            @itemReceipt.Update(tenantId, input.ReceiveFrom, null, input.SameAsShippingAddress, 
                            input.ShippingAddress, input.BillingAddress, input.Total, null);

            #region update receive from 
            // update if come from transfer order 
            if (input.ReceiveFrom == ItemReceipt.ReceiveFromStatus.TransferOrder && input.TransferOrderId != null)
            {
                if (@itemReceipt.TransferOrderId != null && @itemReceipt.TransferOrderId != input.TransferOrderId)
                {
                    var @transferOld = await _transferOrderRepository.GetAll().Where(u => u.Id == @itemReceipt.TransferOrderId).FirstOrDefaultAsync();
                    CheckErrors(await _transferOrderManager.UpdateAsync(@transferOld));
                }

                var @transfer = await _transferOrderRepository.GetAll().Where(u => u.Id == input.TransferOrderId).FirstOrDefaultAsync();
                // update received status 
                if (@transfer == null)
                {
                    throw new UserFriendlyException(L("NoTransferOrder"));
                }
                if (input.Status == TransactionStatus.Publish)
                {
                    @transfer.UpdateShipedStatus(TransferStatus.ReceiveAll);
                }
                CheckErrors(await _transferOrderManager.UpdateAsync(@transfer));
                @itemReceipt.UpdateTransferOrderId(input.TransferOrderId.Value);

            }
            else // in some case if user switch from receive from transfer order to other so update transfer of field item issue transfer order id to null
            {
                var @transfer = await _transferOrderRepository.GetAll().Where(u => u.Id == @itemReceipt.TransferOrderId).FirstOrDefaultAsync();
                if (@transfer != null)
                {
                    @transfer.UpdateShipedStatus(TransferStatus.ShipAll);
                    CheckErrors(await _transferOrderManager.UpdateAsync(@transfer));
                }
                @itemReceipt.UpdateTransferOrderId(null);
            }
            #endregion


            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt);
            CheckErrors(await _journalManager.UpdateAsync(@journal, autoSequence.DocumentType));
            
            CheckErrors(await _journalItemManager.UpdateAsync(@clearanceAccountItem));
            CheckErrors(await _itemReceiptManager.UpdateAsync(@itemReceipt));


            //Update Item Receipt Item and Journal Item
            var itemReceipItems = await _itemReceiptItemRepository.GetAll().Where(u => u.ItemReceiptId == input.Id).ToListAsync();

            var @inventoryJournalItems = await (_journalItemRepository.GetAll()
                                                .Where(u => u.JournalId == journal.Id &&
                                                        u.Key == PostingKey.Inventory && u.Identifier != null)
                                                ).ToListAsync();

            var toDeleteItemReceiptItem = itemReceipItems
                                    .Where(u => !input.Items.Any(i => i.Id != null && i.Id == u.Id)).ToList();
            var toDeletejournalItem = inventoryJournalItems
                .Where(u => !input.Items.Any(i => u.Identifier != null && i.Id != null && i.Id == u.Identifier)).ToList();


            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }


            var itemBatchNos = await _itemReceiptItemBatchNoRepository.GetAll().Where(s => itemReceipItems.Any(r => r.Id == s.ItemReceiptItemId)).AsNoTracking().ToListAsync();
            if (toDeleteItemReceiptItem.Any())
            {
                var deleteItemBatchNos = itemBatchNos.Where(s => toDeleteItemReceiptItem.Any(r => r.Id == s.ItemReceiptItemId)).ToList();
                if (deleteItemBatchNos.Any()) await _itemReceiptItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
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

                            oldItemBatch.Update(userId, itemReceipItem.Id, newBatch.BatchNoId, newBatch.Qty);
                            return oldItemBatch;
                        }).ToList();

                        if (updateItemBatchNos.Any()) await _itemReceiptItemBatchNoRepository.BulkUpdateAsync(updateItemBatchNos);

                        var deleteItemBatchNos = oldItemBatchs.Where(s => !c.ItemBatchNos.Any(r => r.Id == s.Id)).ToList();
                        if (deleteItemBatchNos.Any())
                        {
                            await _itemReceiptItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
                        }
                    }
                    else if (oldItemBatchs.Any())
                    {
                        await _itemReceiptItemBatchNoRepository.BulkDeleteAsync(oldItemBatchs);
                    }

                }
                else if (c.Id == null) //create
                {
                    //insert to item Receipt item
                    var @itemReceiptItem = ItemReceiptItem.Create(tenantId, userId, @itemReceipt, c.ItemId,
                                                                 c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total);
                 
                    if (c.TransferOrderItemId != null)
                    {
                        @itemReceiptItem.UpdateTransferOrderItemId(c.TransferOrderItemId.Value);
                    }
                    @itemReceiptItem.UpdateLot(c.LotId);
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

            await SyncItemReceipt(itemReceipt.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, scheduleDate, scheduleItems);
            }

            return new NullableIdDto<Guid>() { Id = @itemReceipt.Id };
        }

        //[AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_Delete,
        //              AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Delete)]
        //public async Task Delete(EntityDto<Guid> input)
        //{
        //    var @jounal = await _journalRepository.GetAll()
        //        .Include(u => u.ItemReceipt)
        //        .Include(u => u.ItemReceipt.ShippingAddress)
        //        .Include(u => u.ItemReceipt.BillingAddress)
        //        .Where(u => u.JournalType == JournalType.ItemReceiptTransfer && u.ItemReceiptId == input.Id)
        //        .FirstOrDefaultAsync();

        //    //query get item Receipt 
        //    var @entity = @jounal.ItemReceipt;
        //    if (entity == null)
        //    {
        //        throw new UserFriendlyException(L("RecordNotFound"));
        //    }
        //    var validate = await _transferOrderRepository.GetAll()
        //        .Where(t => t.Id == entity.TransferOrderId && t.ConvertToIssueAndReceiptTransfer == true).CountAsync();
        //    if (validate > 0)
        //    {
        //        throw new UserFriendlyException(L("CanNotDeleteThisRecordFromAutoConvert"));
        //    }
        //    @jounal.UpdateItemReceiptTransfer(null);


        //    //update receive status of transfer order to pending 
        //    if (jounal.JournalType == JournalType.ItemIssueTransfer)
        //    {
        //        var @transfer = await _transferOrderRepository.GetAll().Where(u => u.Id == jounal.ItemIssue.TransferOrderId).FirstOrDefaultAsync();
        //        // update received status 
        //        if (@transfer != null)
        //        {
        //            @transfer.UpdateShipedStatus(TransferStatus.ShipAll);
        //            CheckErrors(await _transferOrderManager.UpdateAsync(@transfer));
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
