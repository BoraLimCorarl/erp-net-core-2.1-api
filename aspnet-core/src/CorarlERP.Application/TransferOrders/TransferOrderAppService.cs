using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Authorization;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Classes;
using CorarlERP.Currencies;
using CorarlERP.Items;
using CorarlERP.Locations;
using CorarlERP.TransferOrders.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using CorarlERP.Items.Dto;
using CorarlERP.Journals.Dto;
using CorarlERP.ItemReceipts.Dto;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.ItemReceipts;
using CorarlERP.ItemIssueVendorCredits;
using CorarlERP.ItemReceiptCustomerCredits;
using CorarlERP.ItemIssues;
using CorarlERP.Journals;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Inventories;
using CorarlERP.Locations.Dto;
using CorarlERP.ItemIssueTransfers.Dto;
using CorarlERP.ItemReceiptTransfers.Dto;
using CorarlERP.MultiTenancy;
using CorarlERP.ItemIssueTransfers;
using CorarlERP.ItemReceiptTransfers;
using CorarlERP.ItemIssues.Dto;
using CorarlERP.AutoSequences;
using CorarlERP.Authorization.Users;
using CorarlERP.Dto;
using CorarlERP.Lots.Dto;
using CorarlERP.Lots;
using CorarlERP.UserGroups;
using CorarlERP.Locks;
using CorarlERP.AccountCycles;
using CorarlERP.Common.Dto;
using CorarlERP.InventoryCalculationJobSchedules;
using Hangfire.States;
using CorarlERP.InventoryCalculationItems;
using CorarlERP.BatchNos;
using CorarlERP.Productions;
using CorarlERP.ItemIssueAdjustments.Dto;
using CorarlERP.JournalTransactionTypes;
using CorarlERP.Features;

namespace CorarlERP.TransferOrders
{
    [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_TransferOrder)]
    public class TransferOrderAppService : CorarlERPAppServiceBase, ITransferOrderAppService
    {
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IJournalManager _journalManager;
        private readonly IRepository<Journal, Guid> _journalRepository;

        private readonly IJournalItemManager _journalItemManager;
        private readonly IRepository<JournalItem, Guid> _journalItemRepository;
        private readonly IItemReceiptItemManager _itemReceiptItemManager;
        private readonly IRepository<ItemReceiptItem, Guid> _itemReceiptItemRepository;
        private readonly IRepository<ItemReceipt, Guid> _itemReceiptRepository;
        private readonly IRepository<ItemIssueItem, Guid> _itemIssueItemRepository;
        private readonly IRepository<ItemIssue, Guid> _itemIssueRepository;
        private readonly IItemReceiptManager _itemReceiptManager;
        private readonly ITransferOrderManager _transferOrderManager;
        private readonly IRepository<TransferOrder, Guid> _transferOrderRepository;
        private readonly ITransferOrderItemManager _transferOrderItemManager;
        private readonly IRepository<TransferOrderItem, Guid> _transferOrderItemRepository;
        private readonly IRepository<Item, Guid> _itemRepository;
        private readonly IRepository<ChartOfAccount, Guid> _accountRepository;
        private readonly IRepository<Currency, long> _currencyRepository;
        private readonly IRepository<Class, long> _classRepository;
        private readonly IRepository<Location, long> _locationRepository;
        private readonly IItemIssueManager _itemIssueManager;
        private readonly IInventoryManager _inventoryManager;
        private readonly IItemIssueItemManager _itemIssueItemManager;
        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Lot, long> _lotRepository;
        private readonly IRepository<Lock, long> _lockRepository;
        private readonly IInventoryCalculationItemManager _inventoryCalculationItemManager;
        private readonly ICorarlRepository<BatchNo, Guid> _batchNoRepository;
        private readonly ICorarlRepository<ItemIssueItemBatchNo, Guid> _itemIssueItemBatchNoRepository;
        private readonly ICorarlRepository<ItemReceiptItemBatchNo, Guid> _itemReceiptItemBatchNoRepository;
        private readonly IJournalTransactionTypeManager _journalTransactionTypeManager;
        public TransferOrderAppService(ITransferOrderManager TransferOrderManager,
            IRepository<Lot, long> lotRepository,
            IAutoSequenceManager autoSequenceManger,
            IRepository<AutoSequence, Guid> autoSequenceRepository,
            ITransferOrderItemManager TransferOrderItemManager,
            ItemIssueManager itemIssueManager,
            ICurrencyManager currencyManager,
            IRepository<TransferOrder, Guid> TransferOrderRepository,
            IRepository<TransferOrderItem, Guid> TransferOrderItemRepository,
            IRepository<Item, Guid> itemRepository,
            IRepository<ChartOfAccount, Guid> accountRepository,
            IRepository<Currency, long> currencyRepository,
            IRepository<Location, long> locationRepository,
            IRepository<Class, long> classRepository,
            IRepository<ItemReceiptItem, Guid> itemReceiptItemRepository,
            IRepository<ItemReceipt, Guid> itemReceiptRepository,
            IRepository<ItemReceiptItemCustomerCredit, Guid> itemReceiptCustomerCreditItemRepository,
            IRepository<ItemIssueVendorCreditItem, Guid> itemIssueVendorCreditItemRepository,
            IRepository<Journal, Guid> journalRepository,
            IRepository<ItemIssueItem, Guid> itemIssueItemRepository,
            IRepository<ItemIssue, Guid> itemIssueRepository,
            ICorarlRepository<BatchNo, Guid> batchNoRepository,
            ICorarlRepository<ItemIssueItemBatchNo, Guid> itemIssueItemBatchNoRepository,
            ICorarlRepository<ItemReceiptItemBatchNo, Guid> itemReceiptItemBatchNoRepository,
            IJournalManager journalManager,
            ItemReceiptItemManager itemReceiptItemManager,
            ItemIssueItemManager itemIssueItemManager,
            IJournalItemManager journalItemManager,
            IRepository<JournalItem, Guid> journalItemRepository,
            ItemReceiptManager itemReceiptManager,
            IRepository<Tenant, int> tenantRepository,
            IRepository<User, long> userRepository,
            IInventoryManager inventoryManager,
            IRepository<Lock, long> lockRepository,
            IRepository<AccountCycle,long> accountCycleRepository,
            IInventoryCalculationItemManager inventoryCalculationItemManager,
            IJournalTransactionTypeManager journalTransactionTypeManager,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository) : base(accountCycleRepository,userGroupMemberRepository, locationRepository)
        {
            _userRepository = userRepository;
            _lotRepository = lotRepository;
            _itemIssueManager = itemIssueManager;
            _journalManager = journalManager;
            _journalRepository = journalRepository;
            _journalItemManager = journalItemManager;
            _journalItemRepository = journalItemRepository;
            _tenantRepository = tenantRepository;
            _transferOrderManager = TransferOrderManager;
            _transferOrderRepository = TransferOrderRepository;
            _transferOrderItemManager = TransferOrderItemManager;
            _transferOrderItemRepository = TransferOrderItemRepository;
            _itemRepository = itemRepository;
            _accountRepository = accountRepository;
            _currencyRepository = currencyRepository;
            _locationRepository = locationRepository;
            _classRepository = classRepository;
            _itemReceiptItemRepository = itemReceiptItemRepository;
            _itemIssueItemRepository = itemIssueItemRepository;
            _itemIssueRepository = itemIssueRepository;
            _itemReceiptRepository = itemReceiptRepository;
            _itemIssueItemManager = itemIssueItemManager;
            _inventoryManager = inventoryManager;
            _itemReceiptManager = itemReceiptManager;
            _itemReceiptItemManager = itemReceiptItemManager;
            _autoSequenceManager = autoSequenceManger;
            _autoSequenceRepository = autoSequenceRepository;
            _lockRepository = lockRepository;
            _inventoryCalculationItemManager = inventoryCalculationItemManager;
            _batchNoRepository = batchNoRepository;
            _itemIssueItemBatchNoRepository = itemIssueItemBatchNoRepository;
            _itemReceiptItemBatchNoRepository = itemReceiptItemBatchNoRepository;
            _journalTransactionTypeManager = journalTransactionTypeManager;
        }

      
        private async Task ValidateBatchNo(CreateTransferOrderInput input)
        {
            var validateItems = await _itemRepository.GetAll()
                           .Where(s => input.TransferOrderItems.Any(i => i.ItemId == s.Id))
                           .Where(s => s.UseBatchNo || s.TrackSerial || s.TrackExpiration)
                           .AsNoTracking()
                           .ToListAsync();

            if (!validateItems.Any()) return;

            var batchItemDic = validateItems.ToDictionary(k => k.Id, v => v);

            var itemUseBatchs = input.TransferOrderItems.Where(s => batchItemDic.ContainsKey(s.ItemId)).ToList();

            var find = itemUseBatchs.Where(s => s.ItemBatchNos == null || s.ItemBatchNos.Count == 0 || s.ItemBatchNos.Any(r => r.BatchNoId == Guid.Empty)).FirstOrDefault();
            if (find != null) throw new UserFriendlyException(L("PleaseSelect", $"{L("BatchNo")}/{L("SerialNo")}") + $", Item: {find.Item.ItemCode}-{find.Item.ItemName}");

            var serialItemHash = validateItems.Where(s => s.TrackSerial).Select(s => s.Id).ToHashSet();
            var serialQty = input.TransferOrderItems.Where(s => serialItemHash.Contains(s.ItemId)).FirstOrDefault(s => s.ItemBatchNos.Any(b => b.Qty != 1));
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

            var validateQty = itemUseBatchs.FirstOrDefault(s => s.Unit != s.ItemBatchNos.Sum(t => t.Qty));
            if (validateQty != null) throw new UserFriendlyException(L("IsNotValid", L("Qty") + " " + $"{L("BatchNo")}/{L("SerialNo")}" + $", {batchItemDic[validateQty.ItemId].ItemCode}-{batchItemDic[validateQty.ItemId].ItemName}"));

            var itemIds = itemUseBatchs.GroupBy(s => s.ItemId).Select(s => s.Key).ToList();
            var lots = itemUseBatchs.GroupBy(s => s.FromLotId).Select(s => s.Key.Value).ToList();
            var exceptIds = new List<Guid>();
            if (input is UpdateTransferOrderInput)
            {
                var id = (input as UpdateTransferOrderInput).Id;

                var itemIssue = await _itemIssueRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.TransferOrderId.HasValue && s.TransferOrderId == id);
                var ItemReceipt = await _itemReceiptRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.TransferOrderId.HasValue && s.TransferOrderId == id);
                if (itemIssue != null) exceptIds.Add(itemIssue.Id);
                if (ItemReceipt != null) exceptIds.Add(ItemReceipt.Id);
            }

            var batchBalanceItems = await _inventoryManager.GetItemBatchNoBalance(itemIds, lots, batchNoIds, input.ItemIssueTransferDate.Value, exceptIds);
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

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_TransferOrder_Create)]
        public async Task<NullableIdDto<Guid>> Create(CreateTransferOrderInput input)
        {
            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                   .Where(t => (t.LockKey == TransactionLockType.TransferOrder)
                   && t.IsLock == true && t.LockDate.Value.Date >= input.TransferDate.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            if (input.ConvertToIssueAndReceiptTransfer == true) await ValidateBatchNo(input);

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.TransferOrder);

            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.TransferNo = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            var @entity = TransferOrder.Create(tenantId, userId, input.TransferFromLocationId, input.TransferToLocationId,
                input.TransferFromClassId, input.TransferToClassId,
                input.TransferNo, input.TransferDate, input.Reference, input.Status, input.Memo, input.ConvertToIssueAndReceiptTransfer, input.ItemReceiptTransferDate, input.ItemIssueTransferDate);

            #region TransferOrderItem 
            int index = 0;
            foreach (var i in input.TransferOrderItems)
            {
                index++;
                if (i.ToLotId == 0 || i.FromLotId == 0 || i.ToLotId == null || i.FromLotId == null)
                {
                    throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + index.ToString());
                } 
                
                if (i.FromLotId == i.ToLotId)
                {
                    throw new UserFriendlyException(L("CannotTransferTheSameLot") + " " + L("Row") + " " + index.ToString());
                }
                var @transferOrderItem = TransferOrders.TransferOrderItem.Create(tenantId, userId, entity, i.ItemId, i.Description, i.Unit);
                transferOrderItem.UpdatToLot(i.ToLotId.Value);
                transferOrderItem.UpdateFromLot(i.FromLotId.Value);
                base.CheckErrors(await _transferOrderItemManager.CreateAsync(@transferOrderItem));
                if (input.ConvertToIssueAndReceiptTransfer == true && input.Status == TransactionStatus.Publish)
                {
                    i.Id = transferOrderItem.Id;
                }
            }
            #endregion
            CheckErrors(await _transferOrderManager.CreateAsync(@entity, auto.RequireReference));

          
            await CurrentUnitOfWork.SaveChangesAsync();


            if (input.ConvertToIssueAndReceiptTransfer == true && input.Status == TransactionStatus.Publish)
            {
                var currencyId = await _tenantRepository.GetAll().Where(t => t.Id == tenantId).Select(v => v.CurrencyId).FirstOrDefaultAsync();
                var iventoryAccount = await _itemRepository.GetAll().Where(t => t.TenantId == tenantId && input.TransferOrderItems.Any(v => v.ItemId == t.Id)).ToListAsync();

                var ClearanceAccountId = await _tenantRepository.GetAll().Where(t => t.Id == tenantId).Select(t => t.ItemIssueTransferId).FirstOrDefaultAsync();

                if (ClearanceAccountId == null)
                {
                    throw new UserFriendlyException(L("PleaseSetTransferIssueAccountOnCompanyProfile"));
                }
                //var locations = new List<long?>() {
                //    entity.TransferFromLocationId
                //};
                //var averageCosts = await _inventoryManager.GetAvgCostQuery(@entity.TransferDate.Date, locations).ToListAsync();
                var createInputItemIssueTransfer = new CreateItemIssueTransferInput()
                {
                    IsConfirm = input.IsConfirm,
                    Memo = input.Memo,
                    ReceiptNo = input.TransferNo,
                    BillingAddress = new Addresses.CAddress("", "", "", "", ""),
                    SameAsShippingAddress = true,
                    ShippingAddress = new Addresses.CAddress("", "", "", "", ""),
                    ClassId = input.TransferFromClassId,
                    CurrencyId = currencyId.Value,
                    Date = input.ItemIssueTransferDate.Value,
                    LocationId = input.TransferFromLocationId,
                    ReceiveFrom = ReceiveFrom.TransferOrder,
                    Reference = $"{input.TransferNo}/{input.Reference}",
                    Status = input.Status,
                    ClearanceAccountId = ClearanceAccountId.Value,
                    //Total = input.to,
                    TransferOrderId = entity.Id,
                    Total = 0,

                    Items = input.TransferOrderItems.Select(t => new CreateOrUpdateItemIssueItemTransferInput
                    {
                        Item = ObjectMapper.Map<ItemSummaryDetailOutput>(t.Item),
                        InventoryAccountId = (from Account in iventoryAccount where (Account.Id == t.ItemId) select (Account.InventoryAccountId.Value)).FirstOrDefault(),
                        Description = t.Description,
                        Qty = t.Unit,
                        ItemId = t.ItemId,
                        //Total = averageCosts.Where(v => v.Id == t.ItemId).Select(c => c.AverageCost * t.Unit).FirstOrDefault(),
                        //UnitCost = averageCosts.Where(v => v.Id == t.ItemId).Select(c => c.AverageCost).FirstOrDefault(),
                        DiscountRate = 0,
                        TransferItemId = t.Id,
                        FromlotId = t.FromLotId,
                        ItemBatchNos = t.ItemBatchNos
                    }).ToList(),

                };
                await CreateItemIssueTransfer(createInputItemIssueTransfer);

                //input to item Receipt Transfer
                var ClearanceAccountIdForItemReceiptTransfer = await _tenantRepository.GetAll().Where(t => t.Id == tenantId).Select(t => t.ItemRecieptTransferId).FirstOrDefaultAsync();

                if (ClearanceAccountIdForItemReceiptTransfer == null)
                {
                    throw new UserFriendlyException(L("PleaseSetTransferReceiptAccountOnCompanyProfile"));
                }

                var itemIssue = await _itemIssueItemRepository.GetAll()
                           .Where(t => t.TransferOrderItemId != null)
                           .Select(t => new
                           {
                               Id = t.ItemId,
                               Total = t.Total,
                               UnitCost = t.UnitCost,
                               Qty = t.Qty,
                               TransferOrderItemId = t.TransferOrderItemId
                           })
                           .ToListAsync();

                var createInputItemRecepitTransfer = new CreateItemReceiptTransferInput()
                {
                    IsConfirm = input.IsConfirm,
                    Memo = input.Memo,
                    ReceiptNo = input.TransferNo,
                    BillingAddress = new Addresses.CAddress("", "", "", "", ""),
                    SameAsShippingAddress = true,
                    ShippingAddress = new Addresses.CAddress("", "", "", "", ""),
                    ClassId = input.TransferToClassId,
                    CurrencyId = currencyId.Value,
                    Date = input.ItemIssueTransferDate.Value,
                    LocationId = input.TransferToLocationId,
                    ReceiveFrom = ItemReceipt.ReceiveFromStatus.TransferOrder,
                    Reference = $"{input.TransferNo}/{input.Reference}",
                    Status = input.Status,
                    ClearanceAccountId = ClearanceAccountIdForItemReceiptTransfer.Value,
                    //Total = input.to,
                    TransferOrderId = entity.Id,
                    Total = 0,
                    Items = input.TransferOrderItems.Select(t => new CreateOrUpdateItemReceiptItemTransferInput
                    {
                        LotId = t.ToLotId,
                        InventoryAccountId = (from Account in iventoryAccount where (Account.Id == t.ItemId) select (Account.InventoryAccountId.Value)).FirstOrDefault(),
                        Qty = t.Unit,
                        ItemId = t.ItemId,
                        Total = itemIssue.Where(v => v.TransferOrderItemId == t.Id).Select(c => c.Total).FirstOrDefault(),
                        UnitCost = itemIssue.Where(v => v.TransferOrderItemId == t.Id).Select(c => c.UnitCost).FirstOrDefault(),
                        DiscountRate = 0,
                        Description = t.Description,
                        TransferOrderItemId = t.Id,
                        ItemBatchNos = t.ItemBatchNos
                    }).ToList()

                };
                await CreateItemReceiptTransfer(createInputItemRecepitTransfer);

            }
            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.TransferOrder };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }


        //Auto create Item Issue Transfer
        public async Task CreateItemIssueTransfer(CreateItemIssueTransferInput input)
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
            var countItemIssueItems = (from ItemIssueitem in input.Items
                                       where (ItemIssueitem.TransferItemId != null)
                                       select ItemIssueitem.TransferItemId).Count();
            if (input.ReceiveFrom == ReceiveFrom.TransferOrder && countItemIssueItems <= 0 && input.TransferOrderId.Value == null)
            {
                throw new UserFriendlyException(L("PleaseAddTransferOrder"));
            }
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
                                        input.Memo, input.Total, input.Total, input.CurrencyId,
                                        input.ClassId, input.Reference,input.LocationId);
            entity.UpdateStatus(input.Status);
            #region journal transaction type 
            var transactionTypeId = await _journalTransactionTypeManager.GetJournalTransactionTypeId(InventoryTransactionType.ItemIssueTransfer);
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
                LotId = u.FromlotId,
                Qty = u.Qty,
                InventoryAccountId = u.InventoryAccountId
            }).ToList();
            var lotIds = input.Items.Select(x => x.FromlotId).ToList();
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

            //insert clearance journal item into credit
            var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, input.ClearanceAccountId, input.Memo,
                input.Total, 0, PostingKey.COGS, null);

            //insert to item Issue
            var itemIssue = ItemIssue.Create(tenantId, userId, input.ReceiveFrom, null,
                                        input.SameAsShippingAddress, input.ShippingAddress,
                                        input.BillingAddress, input.Total, null);

            itemIssue.UpdateTransactionType(InventoryTransactionType.ItemIssueTransfer);
            @entity.UpdateIssueTransfer(itemIssue);


            if (input.ReceiveFrom == ReceiveFrom.TransferOrder)
            {
                var @transfer = await _transferOrderRepository.GetAll().Where(u => u.Id == input.TransferOrderId).FirstOrDefaultAsync();
                // update received status 
                if (@transfer == null)
                {
                    throw new UserFriendlyException(L("NoTransferOrder"));
                }
                if (input.Status == TransactionStatus.Publish)
                {
                    @transfer.UpdateShipedStatus(TransferStatus.ShipAll);
                }
                CheckErrors(await _transferOrderManager.UpdateAsync(@transfer));

                itemIssue.UpdateTransferOrderId(input.TransferOrderId.Value);
            }

            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));
            CheckErrors(await _journalItemManager.CreateAsync(clearanceJournalItem));
            CheckErrors(await _itemIssueManager.CreateAsync(itemIssue));

            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }

            int indexLot = 0;
            foreach (var i in input.Items)
            {
                indexLot++;
                if (i.FromlotId == 0 || i.FromlotId == null)
                {
                    throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + indexLot.ToString());
                }
                //insert to item Issue item
                var itemIssueItem = ItemIssueItem.Create(tenantId, userId, itemIssue, i.ItemId,
                                    i.Description, i.Qty, i.UnitCost, i.DiscountRate, i.Total);
                if (i.TransferItemId != null)
                {
                    itemIssueItem.UpdateTransferOrderItemId(i.TransferItemId.Value);
                }
                itemIssueItem.UpdateLot(i.FromlotId);
                CheckErrors(await _itemIssueItemManager.CreateAsync(itemIssueItem));
                //insert inventory journal item into debit
                var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, i.InventoryAccountId,
                                        i.Description, 0, i.Total, PostingKey.Inventory, itemIssueItem.Id);
                CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));

                if (i.ItemBatchNos != null && i.ItemBatchNos.Any())
                {
                    foreach (var a in i.ItemBatchNos)
                    {
                        var bacht = ItemIssueItemBatchNo.Create(tenantId, userId, itemIssueItem.Id, a.BatchNoId, a.Qty);
                        await _itemIssueItemBatchNoRepository.InsertAsync(bacht);
                    }
                }
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            await SyncItemIssue(itemIssue.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                var scheduleItems = input.Items.Select(s => s.ItemId).Distinct().ToList();
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, input.Date, scheduleItems);
            }

        }

        //Auto Delete Item Issue Transfer        
        public async Task DeleteItemIssueTransfer(CarlEntityDto input)
        {

            var journal = await _journalRepository.GetAll()
               .Include(u => u.ItemIssue)
               .Include(u => u.ItemIssue.ShippingAddress)
               .Include(u => u.ItemIssue.BillingAddress)
               .Where(u => u.JournalType == JournalType.ItemIssueTransfer && u.ItemIssueId == input.Id)
               .FirstOrDefaultAsync();

            //query get item Issue
            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                   .Where(t => (t.LockKey == TransactionLockType.ItemIssue || t.LockKey == TransactionLockType.InventoryTransaction)
                   && t.IsLock == true && t.LockDate != null && t.LockDate.Value.Date >= journal.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            var @entity = journal.ItemIssue;
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);

            if (journal.JournalNo == auto.LastAutoSequenceNumber)
            {
                var jo = await _journalRepository.GetAll()
                    .Where(t => t.Id != journal.Id &&
                    (t.JournalType == JournalType.ItemIssueSale ||
                       t.JournalType == JournalType.ItemIssueKitchenOrder ||
                                    t.JournalType == JournalType.ItemIssueOther ||
                                    t.JournalType == JournalType.ItemIssueTransfer ||
                                    t.JournalType == JournalType.ItemIssueAdjustment ||
                                    t.JournalType == JournalType.ItemIssueProduction))
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

            //update receive status of transfer order to pending 
            if (journal.JournalType == JournalType.ItemIssueTransfer)
            {
                var @transfer = _transferOrderRepository.GetAll().Where(u => u.Id == journal.ItemIssue.TransferOrderId).FirstOrDefault();
                // update received status 
                if (@transfer != null && transfer.ConvertToIssueAndReceiptTransfer == false)
                {
                    @transfer.UpdateShipedStatus(TransferStatus.Pending);
                    CheckErrors(await _transferOrderManager.UpdateAsync(@transfer));
                }
            }

            journal.UpdateIssueTransfer(null);

            //query get journal item and delete
            var @jounalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == journal.Id).ToListAsync();
            foreach (var ji in jounalItems)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(ji));
            }

            var scheduleDate = journal.Date;

            CheckErrors(await _journalManager.RemoveAsync(journal));

            var itemBatchNos = await _itemIssueItemBatchNoRepository.GetAll().Where(s => s.ItemIssueItem.ItemIssueId == input.Id).AsNoTracking().ToListAsync();
            if (itemBatchNos.Any())
            {
                await _itemIssueItemBatchNoRepository.BulkDeleteAsync(itemBatchNos);
            }

            //query get item Issue item and delete 
            var itemIssueItems = await _itemIssueItemRepository.GetAll()
                .Where(u => u.ItemIssueId == entity.Id).ToListAsync();

            var scheduleItems = itemIssueItems.Select(s => s.ItemId).Distinct().ToList();

            foreach (var iri in itemIssueItems)
            {

                CheckErrors(await _itemIssueItemManager.RemoveAsync(iri));
            }

            CheckErrors(await _itemIssueManager.RemoveAsync(entity));

            await DeleteInventoryTransactionItems(input.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, scheduleDate, scheduleItems);
            }

        }

        //Auto Update Item Issue Transfer       
        public async Task UpdateItemIssueTransfer(UpdateItemIssueTransferInput input)
        {

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            // update Journal 
            var @journal = await _journalRepository.GetAll()
                              .Include(s => s.ItemIssue)
                              .Include(s => s.ItemIssue.TransferOrder)
                              .Where(u => u.JournalType == JournalType.ItemIssueTransfer && u.ItemIssueId == input.Id)
                              .FirstOrDefaultAsync();

            if (input.IsConfirm == false)
            {
                var validateLockDate = await _lockRepository.GetAll()
                                      .Where(t => t.IsLock == true && t.LockDate != null &&
                                      (t.LockDate.Value.Date >= journal.Date.Date || t.LockDate.Value.Date >= input.Date.Date)
                                      && (t.LockKey == TransactionLockType.ItemIssue || t.LockKey == TransactionLockType.InventoryTransaction)).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);

            if (auto.CustomFormat == true)
            {
                input.ReceiptNo = journal.JournalNo;
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
                LotId = u.FromlotId,
                ItemIssueItemId = input.Id,
                ItemReceiptItemId = input.ItemReceiptId,
                InventoryAccountId = u.InventoryAccountId
            }).ToList();
            var lotIds = input.Items.Select(x => x.FromlotId).ToList();
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


            //journal.UpdateCreditDebit(input.Total, input.Total);
            #endregion Calculat Cost

            var scheduleDate = input.Date > journal.Date ? journal.Date : input.Date;

            journal.Update(tenantId, input.ReceiptNo, input.Date, input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, 0, input.LocationId);
            journal.UpdateStatus(input.Status);

            //update Clearance account 
            var @clearanceAccountItem = await (_journalItemRepository.GetAll()
                                      .Where(u => u.JournalId == journal.Id &&
                                            u.Key == PostingKey.COGS && u.Identifier == null))
                                    .FirstOrDefaultAsync();
            clearanceAccountItem.UpdateJournalItem(tenantId, input.ClearanceAccountId, input.Memo, input.Total, 0);
            
            //update item Issue 
            var itemIssue = await _itemIssueManager.GetAsync(input.Id, true);

            if (itemIssue == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            itemIssue.Update(tenantId, input.ReceiveFrom, null,
                          input.SameAsShippingAddress, input.ShippingAddress,
                          input.BillingAddress, input.Total, null);

            #region update receive from 
            // update if come from transfer order 
            if (input.ReceiveFrom == ReceiveFrom.TransferOrder && input.TransferOrderId != null)
            {
                if (itemIssue.TransferOrderId != null && itemIssue.TransferOrderId != input.TransferOrderId)
                {
                    var @transferOld = await _transferOrderRepository.GetAll().Where(u => u.Id == itemIssue.TransferOrderId).FirstOrDefaultAsync();
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
                    @transfer.UpdateShipedStatus(TransferStatus.ShipAll);
                }
                CheckErrors(await _transferOrderManager.UpdateAsync(@transfer));
                itemIssue.UpdateTransferOrderId(input.TransferOrderId.Value);

            }
            else // in some case if user switch from receive from transfer order to other so update transfer of field item issue transfer order id to null
            {
                var @transfer = await _transferOrderRepository.GetAll().Where(u => u.Id == itemIssue.TransferOrderId).FirstOrDefaultAsync();
                if (@transfer != null)
                {
                    @transfer.UpdateShipedStatus(TransferStatus.Pending);
                    CheckErrors(await _transferOrderManager.UpdateAsync(@transfer));
                }
                itemIssue.UpdateTransferOrderId(null);
            }
            #endregion
            
            CheckErrors(await _journalItemManager.UpdateAsync(@clearanceAccountItem));
            CheckErrors(await _itemIssueManager.UpdateAsync(itemIssue));


            //Update Item Issue Item and Journal Item
            var itemIssueItems = await _itemIssueItemRepository.GetAll().Where(u => u.ItemIssueId == input.Id).ToListAsync();

            var @inventoryJournalItems = await (_journalItemRepository.GetAll()
                                  .Where(u => u.JournalId == journal.Id &&
                                           u.Key == PostingKey.Inventory && u.Identifier != null)).ToListAsync();

            var toDeleteItemIssueItem = itemIssueItems.Where(u => !input.Items.Any(i => i.Id != null && i.Id == u.Id)).ToList();

            var toDeletejournalItem = inventoryJournalItems.Where(u => !input.Items.Any(i => u.Identifier != null && i.Id != null && i.Id == u.Identifier)).ToList();


            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
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
            //await CurrentUnitOfWork.SaveChangesAsync();
            
            CheckErrors(await _journalManager.UpdateAsync(@journal, auto.DocumentType));

            var itemBatchNos = await _itemIssueItemBatchNoRepository.GetAll().Where(s => itemIssueItems.Any(r => r.Id == s.ItemIssueItemId)).AsNoTracking().ToListAsync();
            if (toDeleteItemIssueItem.Any())
            {
                var deleteItemBatchNos = itemBatchNos.Where(s => toDeleteItemIssueItem.Any(r => r.Id == s.ItemIssueItemId)).ToList();
                if (deleteItemBatchNos.Any()) await _itemIssueItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
            }


            foreach (var c in input.Items)
            {
                if (c.Id != null && c.Id != Guid.Empty) //update
                {
                    var itemIssueItem = itemIssueItems.FirstOrDefault(u => u.Id == c.Id);
                    var journalItem = @inventoryJournalItems.FirstOrDefault(u => u.Identifier == c.Id);
                    if (itemIssueItem != null)
                    {
                        //new
                        itemIssueItem.Update(tenantId, c.ItemId, c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total);
                        itemIssueItem.UpdateLot(c.FromlotId);
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
                else if (c.Id == null || c.Id == Guid.Empty) //create
                {
                    //insert to item Issue item
                    var itemIssueItem = ItemIssueItem.Create(tenantId, userId, itemIssue, c.ItemId,
                                                                 c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total);
                    if (c.TransferItemId != null)
                    {
                        itemIssueItem.UpdateTransferOrderItemId(c.TransferItemId.Value);
                    }
                    itemIssueItem.UpdateLot(c.FromlotId);
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
            await CurrentUnitOfWork.SaveChangesAsync();

            await SyncItemIssue(itemIssue.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, scheduleDate, scheduleItems);
            }

        }


        //Auto Create Item Receipt Transfer
        public async Task CreateItemReceiptTransfer(CreateItemReceiptTransferInput input)
        {
            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                    .Where(t => (t.LockKey == TransactionLockType.ItemReceipt || t.LockKey == TransactionLockType.InventoryTransaction)
                    && t.IsLock == true && t.LockDate != null && t.LockDate.Value.Date >= input.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            var total = input.Items.Select(t => t.Total).Sum();
            input.Total = total;
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
            entity.UpdateCreationTimeIndex(entity.CreationTimeIndex.Value + 1);
            entity.UpdateStatus(input.Status);
            #region journal transaction type 
            var transactionTypeId = await _journalTransactionTypeManager.GetJournalTransactionTypeId(InventoryTransactionType.ItemReceiptTransfer);
            entity.SetJournalTransactionTypeId(transactionTypeId);
            #endregion
            //insert clearance journal item into credit
            var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, input.ClearanceAccountId, input.Memo, 0,
                                    input.Total, PostingKey.Clearance, null);

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

            foreach (var i in input.Items)
            {                
                //insert to item Receipt item
                var @itemReceiptItem = ItemReceiptItem.Create(tenantId, userId, @itemReceipt, i.ItemId,
                                                       i.Description, i.Qty, i.UnitCost, i.DiscountRate, i.Total);
                itemReceiptItem.UpdateLot(i.LotId);
                if (i.TransferOrderItemId != null)
                {
                    @itemReceiptItem.UpdateTransferOrderItemId(i.TransferOrderItemId.Value);
                }

                CheckErrors(await _itemReceiptItemManager.CreateAsync(@itemReceiptItem));
                //insert inventory journal item into debit
                var inventoryJournalItem =
                    JournalItem.CreateJournalItem(tenantId, userId, entity, i.InventoryAccountId,
                    i.Description, i.Total, 0, PostingKey.Inventory, itemReceiptItem.Id);
                CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));

                if (i.ItemBatchNos != null && i.ItemBatchNos.Any())
                {
                    foreach (var a in i.ItemBatchNos)
                    {
                        var bacht = ItemReceiptItemBatchNo.Create(tenantId, userId, itemReceiptItem.Id, a.BatchNoId, a.Qty);
                        await _itemReceiptItemBatchNoRepository.InsertAsync(bacht);
                    }
                }

            }
            await CurrentUnitOfWork.SaveChangesAsync();

            await SyncItemReceipt(itemReceipt.Id);
        }

        //Auto Delete Item Receipt Transfer         
        public async Task DeleteItemReceiptTransfer(CarlEntityDto input)
        {
            var @jounal = await _journalRepository.GetAll()
               .Include(u => u.ItemReceipt)
               .Include(u => u.ItemReceipt.ShippingAddress)
               .Include(u => u.ItemReceipt.BillingAddress)
               .Where(u => u.JournalType == JournalType.ItemReceiptTransfer && u.ItemReceiptId == input.Id)
               .FirstOrDefaultAsync();

            //query get item Receipt
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
            var @entity = @jounal.ItemReceipt;
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt);

            if (jounal.JournalNo == auto.LastAutoSequenceNumber)
            {
                var jo = await _journalRepository.GetAll()
                    .Where(t => t.Id != jounal.Id &&
                                     (t.JournalType == JournalType.ItemReceiptPurchase ||
                                     t.JournalType == JournalType.ItemReceiptAdjustment ||
                                     t.JournalType == JournalType.ItemReceiptOther ||
                                     t.JournalType == JournalType.ItemReceiptTransfer ||
                                     t.JournalType == JournalType.ItemReceiptProduction))
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
            @jounal.UpdateItemReceiptTransfer(null);

            //update receive status of transfer order to pending 
            if (jounal.JournalType == JournalType.ItemIssueTransfer)
            {
                var @transfer = await _transferOrderRepository.GetAll().Where(u => u.Id == jounal.ItemIssue.TransferOrderId).FirstOrDefaultAsync();
                // update received status 
                if (@transfer != null)
                {
                    @transfer.UpdateShipedStatus(TransferStatus.ShipAll);
                    CheckErrors(await _transferOrderManager.UpdateAsync(@transfer));
                }
            }
            //query get journal item and delete
            var @jounalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == jounal.Id).ToListAsync();
            foreach (var ji in jounalItems)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(ji));
            }

            CheckErrors(await _journalManager.RemoveAsync(@jounal));

            var itemBatchNos = await _itemReceiptItemBatchNoRepository.GetAll().Where(s => s.ItemReceiptItem.ItemReceiptId == input.Id).AsNoTracking().ToListAsync();
            if (itemBatchNos.Any())
            {
                await _itemReceiptItemBatchNoRepository.BulkDeleteAsync(itemBatchNos);
            }

            //query get item receipt item and delete 
            var @itemReceiptItems = await _itemReceiptItemRepository.GetAll()
                .Where(u => u.ItemReceiptId == entity.Id).ToListAsync();

            foreach (var iri in @itemReceiptItems)
            {
                CheckErrors(await _itemReceiptItemManager.RemoveAsync(iri));
            }
            CheckErrors(await _itemReceiptManager.RemoveAsync(entity));

            await DeleteInventoryTransactionItems(input.Id);
        }

        //Auto Update Item Receipt Transfer       
        public async Task UpdateReceiptTransfer(UpdateItemReceiptTransferInput input)
        {
           // var total = input.Items.Select(t => t.Total).Sum();
           // input.Total = total;
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            // update Journal 
            var @journal = await _journalRepository
                              .GetAll()
                              .Where(u => u.JournalType == JournalType.ItemReceiptTransfer && u.ItemReceiptId == input.Id)
                              .FirstOrDefaultAsync();
            if (input.IsConfirm == false)
            {
                var validateLockDate = await _lockRepository.GetAll()
                                     .Where(t => t.IsLock == true && t.LockDate != null &&
                                     (t.LockDate.Value.Date >= journal.Date.Date || t.LockDate.Value.Date >= input.Date.Date)
                                     && (t.LockKey == TransactionLockType.ItemReceipt || t.LockKey == TransactionLockType.InventoryTransaction)).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            #region Calculat Cost
            //var roundingId = (await GetCurrentTenantAsync()).RoundDigitAccountId;

            //var itemToCalculateCost = input.Items.Select((u, index) => new Inventories.Data.GetAvgCostForIssueData()
            //{
            //    Index = index,
            //    ItemId = u.ItemId,
            //    ItemCode = "",
            //    Qty = u.Qty,
            //    LotId = u.LotId,
            //    ItemIssueItemId = input.Id,
            //    InventoryAccountId = u.InventoryAccountId
            //}).ToList();

            //var getCostResult = await _inventoryManager.GetAvgCostForIssues(input.Date, input.LocationId, itemToCalculateCost, @journal, roundingId);

            //foreach (var r in getCostResult.Items)
            //{
            //    input.Items[r.Index].UnitCost = r.UnitCost;
            //    input.Items[r.Index].Total = r.LineCost;
            //}

            //input.Total = getCostResult.Total;
            #endregion Calculat Cost

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt);
            if (auto.CustomFormat == true) {
                input.ReceiptNo = journal.JournalNo;
            }

            journal.Update(tenantId, input.ReceiptNo, input.Date, input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, 0,input.LocationId);
            journal.UpdateStatus(input.Status);
            //update Clearance account 
            var @clearanceAccountItem = await (_journalItemRepository.GetAll()
                                      .Where(u => u.JournalId == journal.Id &&
                                               u.Key == PostingKey.Clearance && u.Identifier == null)).FirstOrDefaultAsync();
            clearanceAccountItem.UpdateJournalItem(tenantId, input.ClearanceAccountId, input.Memo, 0, input.Total);


            //update item receipt 
            var @itemReceipt = await _itemReceiptManager.GetAsync(input.Id, true);

            if (@itemReceipt == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
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


            CheckErrors(await _journalManager.UpdateAsync(@journal, auto.DocumentType));
            
            CheckErrors(await _journalItemManager.UpdateAsync(@clearanceAccountItem));
            CheckErrors(await _itemReceiptManager.UpdateAsync(@itemReceipt));


            //Update Item Receipt Item and Journal Item
            var itemReceipItems = await _itemReceiptItemRepository.GetAll().Where(u => u.ItemReceiptId == input.Id).ToListAsync();

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

            var itemBatchNos = await _itemReceiptItemBatchNoRepository.GetAll().Where(s => itemReceipItems.Any(r => r.Id == s.ItemReceiptItemId)).AsNoTracking().ToListAsync();
            if (toDeleteItemReceiptItem.Any())
            {
                var deleteItemBatchNos = itemBatchNos.Where(s => toDeleteItemReceiptItem.Any(r => r.Id == s.ItemReceiptItemId)).ToList();
                if (deleteItemBatchNos.Any()) await _itemReceiptItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
            }


            foreach (var c in input.Items)
            {
                if (c.Id != null && c.Id != Guid.Empty) //update
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
                        var addItemBatchNos = c.ItemBatchNos
                            .Where(s => !s.Id.HasValue || s.Id == Guid.Empty || !oldItemBatchs.Any(r => r.BatchNoId == s.BatchNoId))
                            .Select(s => ItemReceiptItemBatchNo.Create(tenantId, userId, itemReceipItem.Id, s.BatchNoId, s.Qty))
                            .ToList();
                        if (addItemBatchNos.Any())
                        {
                            foreach (var a in addItemBatchNos)
                            {
                                await _itemReceiptItemBatchNoRepository.InsertAsync(a);
                            }
                        }

                        var updateItemBatchNos = oldItemBatchs.Where(s => c.ItemBatchNos.Any(r => r.BatchNoId == s.BatchNoId)).Select(s =>
                        {
                            var oldItemBatch = s;
                            var newBatch = c.ItemBatchNos.FirstOrDefault(r => r.BatchNoId == s.BatchNoId);

                            oldItemBatch.Update(userId, itemReceipItem.Id, newBatch.BatchNoId, newBatch.Qty);
                            return oldItemBatch;
                        }).ToList();

                        if (updateItemBatchNos.Any()) await _itemReceiptItemBatchNoRepository.BulkUpdateAsync(updateItemBatchNos);

                        var deleteItemBatchNos = oldItemBatchs.Where(s => !c.ItemBatchNos.Any(r => r.BatchNoId == s.BatchNoId)).ToList();
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
                else if (c.Id == null || c.Id == Guid.Empty) //create
                {
                    //insert to item Receipt item
                    var @itemReceiptItem = ItemReceiptItem.Create(tenantId, userId, @itemReceipt, c.ItemId,
                                                                 c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total);
                    if (c.TransferOrderItemId != null)
                    {
                        @itemReceiptItem.UpdateTransferOrderItemId(c.TransferOrderItemId.Value);
                    }
                    itemReceiptItem.UpdateLot(c.LotId);
                    CheckErrors(await _itemReceiptItemManager.CreateAsync(@itemReceiptItem));

                    var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal,
                        c.InventoryAccountId, c.Description, c.Total, 0, PostingKey.Inventory, itemReceiptItem.Id);
                    CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));

                    if (c.ItemBatchNos != null && c.ItemBatchNos.Any())
                    {
                        foreach (var a in c.ItemBatchNos)
                        {
                            var bacht = ItemReceiptItemBatchNo.Create(tenantId, userId, itemReceiptItem.Id, a.BatchNoId, a.Qty);
                            await _itemReceiptItemBatchNoRepository.InsertAsync(bacht);
                        }
                    }
                }

            }

            foreach (var t in toDeleteItemReceiptItem.OrderBy(u => u.Id))
            {
                CheckErrors(await _itemReceiptItemManager.RemoveAsync(t));
            }

            foreach (var t in toDeletejournalItem)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(t));
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            await SyncItemReceipt(itemReceipt.Id);
        }



        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_TransferOrder_Delete,
                      AppPermissions.Pages_Tenant_Inventory_TransferOrder_EditDelete48hour)]
        public async Task Delete(CarlEntityDto input)
        {
            var @entity = await _transferOrderManager.GetAsync(input.Id, true);
            var userId = AbpSession.GetUserId();
            var getPermission = await GetUserPermissions(userId);
            var IsEdit = getPermission.Where(t => t == AppPermissions.Pages_Tenant_Inventory_TransferOrder_EditDelete48hour).Count();
            var totalHours = (DateTime.Now - @entity.CreationTime).TotalHours;
            if (IsEdit > 0 && totalHours > 48)
            {
                throw new UserFriendlyException(L("ThisTransactionIsOver48HoursCanNotEdit"));
            }
            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                  .Where(t => (t.LockKey == TransactionLockType.TransferOrder)
                  && t.IsLock == true && t.LockDate != null && t.LockDate.Value.Date >= entity.TransferDate.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            if (entity.ShipedStatus != TransferStatus.Pending && entity.ConvertToIssueAndReceiptTransfer == false)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.TransferOrder);

            if (entity.TransferNo == auto.LastAutoSequenceNumber)
            {
                var pro = await _transferOrderRepository.GetAll().Where(t => t.Id != entity.Id)
                    .OrderByDescending(t => t.CreationTime).FirstOrDefaultAsync();
                if (pro != null)
                {
                    auto.UpdateLastAutoSequenceNumber(pro.TransferNo);
                }
                else
                {
                    auto.UpdateLastAutoSequenceNumber(null);
                }
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }
            if (entity.Status == TransactionStatus.Publish)
            {
                if (entity.ConvertToIssueAndReceiptTransfer == true)
                {
                    //delete item receipt 

                    var itemReceiptTransferId = _itemReceiptRepository.GetAll()
                           .Where(t => t.TransferOrderId == input.Id && t.TransactionType == InventoryTransactionType.ItemReceiptTransfer).Select(t => t.Id).FirstOrDefault();
                    var inputItemReceiptTransfer = new CarlEntityDto() {IsConfirm = input.IsConfirm, Id = itemReceiptTransferId };
                    await DeleteItemReceiptTransfer(inputItemReceiptTransfer);

                    //delete item issue
                    var itemIssueTransferId = _itemIssueRepository.GetAll()
                        .Where(t => t.TransferOrderId == input.Id && t.TransactionType == InventoryTransactionType.ItemIssueTransfer).Select(t => t.Id).FirstOrDefault();
                    var inputItemIssueTransfer = new CarlEntityDto() {IsConfirm = input.IsConfirm, Id = itemIssueTransferId };
                    await DeleteItemIssueTransfer(inputItemIssueTransfer);

                }
                else
                {
                    var draftInput = new UpdateStatus();
                    draftInput.Id = input.Id;
                    await UpdateStatusToDraft(draftInput);
                }                
            }

            var transferOrderItem = await _transferOrderItemRepository.GetAll().Where(u => u.TransferOrderId == entity.Id).ToListAsync();

            //var batchNos = transferOrderItem.Where(s => s.BatchNoId.HasValue)
            //                                .GroupBy(s => s.BatchNoId)
            //                                .Select(s => s.FirstOrDefault().BatchNo)
            //                                .ToList();

            foreach (var s in transferOrderItem)
            {
                CheckErrors(await _transferOrderItemManager.RemoveAsync(s));
            }


            CheckErrors(await _transferOrderManager.RemoveAsync(@entity));

            //if (batchNos.Any())
            //{
            //    await CurrentUnitOfWork.SaveChangesAsync();
            //    var allBatchUse = await GetBatchNoUseByOthers(input.Id, batchNos.Select(s => s.Id).ToList());
            //    var deleteBatchNos = batchNos.Where(s => !allBatchUse.Contains(s.Id)).ToList();
            //    if (deleteBatchNos.Any()) await _batchNoRepository.BulkDeleteAsync(deleteBatchNos);
            //}

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.TransferOrder };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_TransferOrder_Find,
            AppPermissions.Pages_Tenant_Inventory_TransferOrder_ForItemIssueTransfer,
            AppPermissions.Pages_Tenant_Inventory_TransferOrder_FindItemReceiptTrasfer)]
        public async Task<PagedResultDto<TransferOrderGetListOutput>> Find(GetTransferOrderListInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var tQuery = _transferOrderRepository.GetAll()
                         .WhereIf(input.DeliveryStatus != null && input.DeliveryStatus.Count > 0, u => input.DeliveryStatus.Contains(u.ShipedStatus))
                         .WhereIf(!input.Filter.IsNullOrEmpty(),
                             p => p.TransferNo.ToLower().Contains(input.Filter.ToLower()) ||
                             p.Reference.ToLower().Contains(input.Filter.ToLower()))
                        .WhereIf(userGroups != null && userGroups.Count > 0,
                                u => userGroups.Contains(u.TransferFromLocationId) || userGroups.Contains(u.TransferToLocationId))
                        .AsNoTracking()
                        .Select(s => new
                        {
                            TransferOrderId = s.Id,
                            TransferOrderTransferNo = s.TransferNo,
                            TransferOrderShipedStatus = s.ShipedStatus,
                            TransferOrderTransferDate = s.TransferDate,
                            TransferOrderStatus = s.Status
                        });

            var tiQuery = _transferOrderItemRepository.GetAll()
                          .AsNoTracking()
                          .Select(s => s.TransferOrderId);

            var query = from t in tQuery
                        join ti in tiQuery
                        on t.TransferOrderId equals ti
                        into tis
                        select new TransferOrderGetListOutput
                        {
                            Id = t.TransferOrderId,
                            TransferNo = t.TransferOrderTransferNo,
                            CountItem = tis.Count(),
                            ReceiveStatus = t.TransferOrderShipedStatus,
                            TransferDate = t.TransferOrderTransferDate,
                            StatusCode = t.TransferOrderStatus
                        };

            var resultCount = await query.CountAsync();
            if (resultCount == 0) return new PagedResultDto<TransferOrderGetListOutput>(resultCount, new List<TransferOrderGetListOutput>());

            if (input.IsForItemIssue)
            {
                query = from q in query
                        join tr in _itemIssueRepository.GetAll()
                            .Where(i => i.TransferOrderId != null).AsNoTracking()
                            .Select(i => i.TransferOrderId)
                        on q.Id equals tr into p
                        where p.Count() == 0
                        select q;

            }
            else if (input.IsForItemReceipt)
            {
                query = from q in query
                        join tr in _itemReceiptRepository.GetAll()
                            .Where(i => i.TransferOrderId != null).AsNoTracking()
                            .Select(i => i.TransferOrderId)
                        on q.Id equals tr into p
                        where p.Count() == 0
                        select q;
            }

            var @entities = await query.OrderBy(s => s.TransferDate).PageBy(input).ToListAsync();

            return new PagedResultDto<TransferOrderGetListOutput>(resultCount, @entities);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_TransferOrder_Find,
            AppPermissions.Pages_Tenant_Inventory_TransferOrder_ForItemIssueTransfer,
            AppPermissions.Pages_Tenant_Inventory_TransferOrder_FindItemReceiptTrasfer)]
        public async Task<PagedResultDto<TransferOrderGetListOutput>> FindOld(GetTransferOrderListInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var query = (from transferOrderItem in _transferOrderItemRepository.GetAll().AsNoTracking()
                         
                         join transferOrder in _transferOrderRepository.GetAll()
                         .WhereIf(input.DeliveryStatus != null && input.DeliveryStatus.Count > 0, u => input.DeliveryStatus.Contains(u.ShipedStatus))
                         //.WhereIf(input.FromLocations != null && input.FromLocations.Count > 0, u => input.FromLocations.Contains(u.TransferFromLocationId))
                         //.WhereIf(input.ToLocations != null && input.ToLocations.Count > 0, u => input.ToLocations.Contains(u.TransferToLocationId))
                         .WhereIf(!input.Filter.IsNullOrEmpty(),
                             p => p.TransferNo.ToLower().Contains(input.Filter.ToLower()) ||
                             p.Reference.ToLower().Contains(input.Filter.ToLower()))
                        .WhereIf(userGroups != null && userGroups.Count > 0,
                                u => userGroups.Contains(u.TransferFromLocationId) || userGroups.Contains(u.TransferToLocationId))
                        .AsNoTracking()
                         on transferOrderItem.TransferOrderId equals transferOrder.Id
                         group transferOrder by new
                         {
                             TransferOrderId = transferOrder.Id,
                             TransferOrderTransferNo = transferOrder.TransferNo,
                             TransferOrderShipedStatus = transferOrder.ShipedStatus,
                             TransferOrderTransferDate = transferOrder.TransferDate,
                             TransferOrderStatus = transferOrder.Status
                         } into u
                         select new TransferOrderGetListOutput
                         {
                             Id = u.Key.TransferOrderId,
                             TransferNo = u.Key.TransferOrderTransferNo,
                             CountItem = u.Count(),
                             ReceiveStatus = u.Key.TransferOrderShipedStatus,
                             TransferDate = u.Key.TransferOrderTransferDate,
                             StatusCode = u.Key.TransferOrderStatus,
                             
                         });
            var resultCount = await query.CountAsync();

            if (input.IsForItemIssue && resultCount > 0)
            {
                query = (from q in query
                        join tr in _itemIssueRepository.GetAll()
                        .Where(i => i.TransferOrderId != null).AsNoTracking()
                        .Select(i => i.TransferOrderId)
                        on q.Id equals tr into p
                        where p.Count() == 0
                        select q);
                
            }
            if (input.IsForItemReceipt && resultCount > 0)
            {
                query = (from q in query
                         join tr in _itemReceiptRepository.GetAll()
                         .Where(i => i.TransferOrderId != null).AsNoTracking()
                         .Select(i => i.TransferOrderId)
                         on q.Id equals tr into p
                         where p.Count() == 0
                         select q);
            }
            var @entities = new List<TransferOrderGetListOutput>();
            if (resultCount > 0)
            {
                @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            }
            return new PagedResultDto<TransferOrderGetListOutput>(resultCount, @entities);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_TransferOrder_GetList)]
        public async Task<PagedResultDto<TransferOrderGetListOutput>> GetList(GetTransferOrderListInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();

            var tQuery = _transferOrderRepository.GetAll()
                        .Include(u=>u.LastModifierUser)
                        .WhereIf(input.FromDate != null && input.ToDate != null,
                                (u => (u.TransferDate.Date) >= (input.FromDate.Date) && (u.TransferDate.Date) <= (input.ToDate.Date)))
                        .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                        .WhereIf(userGroups != null && userGroups.Count > 0,
                                u => userGroups.Contains(u.TransferFromLocationId) || userGroups.Contains(u.TransferToLocationId))
                        .WhereIf(input.Locactions != null && input.Locactions.Count > 0,
                                u => input.Locactions.Contains(u.TransferFromLocationId) || input.Locactions.Contains(u.TransferToLocationId))
                        .WhereIf(!input.Filter.IsNullOrEmpty(),
                                    p => p.TransferNo.ToLower().Contains(input.Filter.ToLower()) ||
                                    p.Reference.ToLower().Contains(input.Filter.ToLower()) ||
                                    p.Memo.ToLower().Contains(input.Filter.ToLower()))
                        .AsNoTracking()
                        .Select(s => new
                        {
                            Id = s.Id,
                            Status = s.Status,
                            ShipedStatus = s.ShipedStatus,
                            TransferDate = s.TransferDate,
                            TransferNo = s.TransferNo,
                            Reference = s.Reference,
                            FLocationId = s.TransferFromLocationId,
                            TLocationId = s.TransferToLocationId,
                            CreatorId = s.CreatorUserId,
                            LastModifiedUserName = s.LastModifierUser != null ? s.LastModifierUser.Name : "",
                        });

            var locationQuery = GetLocations(null, new List<long>());
            var userQuery = GetUsers(input.Users);

            var transferQuery = from t in tQuery
                                join u in userQuery
                                on t.CreatorId equals u.Id
                                join fl in locationQuery
                                on t.FLocationId equals fl.Id
                                join tl in locationQuery
                                on t.TLocationId equals tl.Id
                                select new
                                {
                                    Id = t.Id,
                                    Status = t.Status,
                                    ShipedStatus = t.ShipedStatus,
                                    TransferDate = t.TransferDate,
                                    TransferNo = t.TransferNo,
                                    Reference = t.Reference,
                                    FLocationId = t.FLocationId,
                                    TLocationId = t.TLocationId,
                                    CreatorId = t.CreatorId,
                                    UserName = u.UserName,
                                    FLocationName = fl.LocationName,
                                    TLocationName = tl.LocationName,
                                    LastModifiedUserName = t.LastModifiedUserName,
                                };

            var tiQuery = _transferOrderItemRepository.GetAll()
                          .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                          .AsNoTracking()
                          .Select(s => s.TransferOrderId);


            var query = from t in transferQuery
                        join ti in tiQuery
                        on t.Id equals ti
                        into tis
                        where tis.Count() > 0
                        select new TransferOrderGetListOutput
                        {
                            Id = t.Id,
                            CountItem = tis.Count(),
                            StatusCode = t.Status,
                            ReceiveStatus = t.ShipedStatus,
                            TransferDate = t.TransferDate,
                            TransferNo = t.TransferNo,
                            Reference = t.Reference,
                            FromLocation = new LocationSummaryOutput
                            {
                                Id = t.FLocationId,
                                LocationName = t.FLocationName
                            },
                            User = new UserDto
                            {
                                Id = t.CreatorId.Value,
                                UserName = t.UserName
                            },
                            ToLocation = new LocationSummaryOutput
                            {
                                Id = t.TLocationId,
                                LocationName = t.TLocationName
                            },
                            LastModifiedUserName = t.LastModifiedUserName,
                        };

            var resultCount = await query.CountAsync();
            if (resultCount == 0) return new PagedResultDto<TransferOrderGetListOutput>(resultCount, new List<TransferOrderGetListOutput>());

            if (input.Sorting.EndsWith("DESC"))
            {
                if (input.Sorting.ToLower().StartsWith("transferdate"))
                {
                    query = query.OrderByDescending(s => s.TransferDate);
                }
                else if (input.Sorting.ToLower().StartsWith("transferno"))
                {
                    query = query.OrderByDescending(s => s.TransferNo);
                }
                else if (input.Sorting.ToLower().StartsWith("fromlocation"))
                {
                    query = query.OrderByDescending(s => s.FromLocation.LocationName);
                }
                else if (input.Sorting.ToLower().StartsWith("tolocation"))
                {
                    query = query.OrderByDescending(s => s.ToLocation.LocationName);
                }
                else if (input.Sorting.ToLower().StartsWith("receivestatus"))
                {
                    query = query.OrderByDescending(s => s.ReceiveStatus);
                }
                else if (input.Sorting.ToLower().StartsWith("statuscode"))
                {
                    query = query.OrderByDescending(s => s.StatusCode);
                }
                else if (input.Sorting.ToLower().StartsWith("countitem"))
                {
                    query = query.OrderByDescending(s => s.CountItem);
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    query = query.OrderBy(input.Sorting);
                }
            }
            else
            {
                if (input.Sorting.ToLower().StartsWith("transferdate"))
                {
                    query = query.OrderBy(s => s.TransferDate);
                }
                else if (input.Sorting.ToLower().StartsWith("transferno"))
                {
                    query = query.OrderBy(s => s.TransferNo);
                }
                else if (input.Sorting.ToLower().StartsWith("fromlocation"))
                {
                    query = query.OrderBy(s => s.FromLocation.LocationName);
                }
                else if (input.Sorting.ToLower().StartsWith("tolocation"))
                {
                    query = query.OrderBy(s => s.ToLocation.LocationName);
                }
                else if (input.Sorting.ToLower().StartsWith("receivestatus"))
                {
                    query = query.OrderBy(s => s.ReceiveStatus);
                }
                else if (input.Sorting.ToLower().StartsWith("statuscode"))
                {
                    query = query.OrderBy(s => s.StatusCode);
                }
                else if (input.Sorting.ToLower().StartsWith("countitem"))
                {
                    query = query.OrderBy(s => s.CountItem);
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    query = query.OrderBy(input.Sorting);
                }
            }

            var @entities = await query.PageBy(input).ToListAsync();

            return new PagedResultDto<TransferOrderGetListOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_TransferOrder_GetList)]
        public async Task<PagedResultDto<TransferOrderGetListOutput>> GetListOld(GetTransferOrderListInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var query = (from transferItem in _transferOrderItemRepository.GetAll().AsNoTracking()
                         join i in _itemRepository.GetAll().AsNoTracking()
                         .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.Id))
                         on transferItem.ItemId equals i.Id

                         join transfer in _transferOrderRepository.GetAll()
                         .Include(t=> t.TransferFromLocation)
                         .Include(t => t.TransferToLocation)
                         .AsNoTracking()
                        .WhereIf(input.FromDate != null && input.ToDate != null,
                                (u => (u.TransferDate.Date) >= (input.FromDate.Date) && (u.TransferDate.Date) <= (input.ToDate.Date)))
                        .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                        .WhereIf(userGroups != null && userGroups.Count > 0,
                                u => userGroups.Contains(u.TransferFromLocationId) || userGroups.Contains(u.TransferToLocationId))
                        .WhereIf(input.Locactions != null && input.Locactions.Count > 0, 
                                u => input.Locactions.Contains(u.TransferFromLocationId) || input.Locactions.Contains(u.TransferToLocationId))
                        .WhereIf(!input.Filter.IsNullOrEmpty(),
                                    p => p.TransferNo.ToLower().Contains(input.Filter.ToLower()) ||
                                    p.Reference.ToLower().Contains(input.Filter.ToLower()) ||
                                    p.Memo.ToLower().Contains(input.Filter.ToLower()))
                        on transferItem.TransferOrderId equals transfer.Id

                         //join fl in _locationRepository.GetAll()
                         //.WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.Id))
                         //.AsNoTracking() 
                         //on transfer.TransferFromLocationId equals fl.Id

                         //join tl in _locationRepository.GetAll()
                         //.WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.Id))
                         //.AsNoTracking() 
                         //on transfer.TransferToLocationId equals tl.Id

                         join u in _userRepository.GetAll().AsNoTracking() 
                         on transfer.CreatorUserId equals u.Id

                         group transfer by new {
                             Id = transfer.Id,
                             Status = transfer.Status,
                             ShipedStatus = transfer.ShipedStatus,
                             TransferDate = transfer.TransferDate,
                             TransferNo = transfer.TransferNo,
                             FLocationId = transfer.TransferFromLocation.Id,
                             FLocationName = transfer.TransferFromLocation.LocationName,
                             TLocationId = transfer.TransferToLocation.Id,
                             TLocationName = transfer.TransferToLocation.LocationName,
                             CreatorId = u.Id,
                             CreatorName = u.UserName
                             //TransferOrder = transfer, FromLocation = fl, ToLocation = tl, user = u
                         } into u
                         select new TransferOrderGetListOutput
                         {
                             Id = u.Key.Id,
                             CountItem = u.Count(),
                             StatusCode = u.Key.Status,
                             ReceiveStatus = u.Key.ShipedStatus,
                             TransferDate = u.Key.TransferDate,
                             TransferNo = u.Key.TransferNo,
                             FromLocation = new LocationSummaryOutput
                             {
                                 Id = u.Key.FLocationId,
                                 LocationName = u.Key.FLocationName
                             },
                             User = new UserDto {
                                 Id = u.Key.CreatorId,
                                 UserName = u.Key.CreatorName
                             },
                             ToLocation = new LocationSummaryOutput
                             {
                                 Id = u.Key.TLocationId,
                                 LocationName = u.Key.TLocationName
                             },
                             //CanDrafOrVoid = u.Key.TransferOrder.ShipedStatus != TransferStatus.Pending && u.Key.TransferOrder.ConvertToIssueAndReceiptTransfer == false ? false :

                             //        _itemIssueRepository.GetAll().Include(g => g.TransferOrder).Where(v => u.Any(t => t.Id == v.TransferOrderId) && u.Key.TransferOrder.ConvertToIssueAndReceiptTransfer == false).Count() > 0
                             //            ||
                             //        _itemReceiptRepository.GetAll().Include(g => g.TransferOrder).Where(v => u.Any(t => t.Id == v.TransferOrderId) && u.Key.TransferOrder.ConvertToIssueAndReceiptTransfer == false).Count() > 0
                             //            ?
                             //        false
                             //            :
                             //        true
                         });
            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<TransferOrderGetListOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_TransferOrder_GetDetail)]
        public async Task<TransferOrderDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            await TurnOffTenantFilterIfDebug();

            var @entity = await _transferOrderManager.GetAsync(input.Id);
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            var locations = new List<long?>() {
                entity.TransferFromLocationId
            };

            //var averageCosts = await _inventoryManager.GetAvgCostQuery(entity.ItemIssueTransferDate != null ? entity.ItemIssueTransferDate.Value : entity.TransferDate, locations).ToListAsync();

            var @TransferOrderItem = await _transferOrderItemRepository.GetAll()
                                    .Include(u => u.Item)
                                    .Include(u=>u.FromLot)
                                    .Include(u=>u.ToLot)
                                    .Where(u => u.TransferOrderId == entity.Id)
                                    .Select(t => new CreateOrUpdateTransferOrderItemInput
                                    {
                                        FromLotId = t.FromLotId,
                                        ToLotId = t.ToLotId,
                                        FromLotDetail = ObjectMapper.Map<LotSummaryOutput>(t.FromLot),
                                        ToLotDetail = ObjectMapper.Map<LotSummaryOutput>(t.ToLot),
                                        Description = t.Description,
                                        Id = t.Id,
                                        Item = new ItemSummaryDetailOutput
                                        {
                                            Id = t.Item.Id,
                                            ItemCode = t.Item.ItemCode,
                                            ItemName = t.Item.ItemName,
                                        },
                                        ItemId = t.ItemId,
                                        Unit = t.Qty,
                                        CreateTime = t.CreationTime,
                                        UseBatchNo = t.Item != null && t.Item.UseBatchNo,
                                        TrackSerial = t.Item != null && t.Item.TrackSerial,
                                        TrackExpiration = t.Item != null && t.Item.TrackExpiration
                                        
                                    })
                                    .OrderBy(t => t.CreateTime)
                                    .ToListAsync();

            var batchDic = await _itemIssueItemBatchNoRepository.GetAll()
                                .AsNoTracking()
                                .Where(s => s.ItemIssueItem.TransferOrderItemId.HasValue && s.ItemIssueItem.TransferOrderItem.TransferOrderId == input.Id)
                                .Select(s => new BatchNoItemOutput
                                {
                                    Id = s.Id,
                                    BatchNoId = s.BatchNoId,
                                    BatchNumber = s.BatchNo.Code,
                                    ExpirationDate = s.BatchNo.ExpirationDate,
                                    Qty = s.Qty,
                                    TransactionItemId = s.ItemIssueItem.TransferOrderItemId.Value
                                })
                                .GroupBy(s => s.TransactionItemId)
                                .ToDictionaryAsync(k => k.Key, v => v.ToList());
            if (batchDic.Any())
            {
                foreach (var i in TransferOrderItem)
                {
                    if (batchDic.ContainsKey(i.Id.Value)) i.ItemBatchNos = batchDic[i.Id.Value];
                }
            }

            var itemIssue = await _itemIssueRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.TransferOrderId.HasValue && s.TransferOrderId == input.Id);
            var itemReceipt = await _itemReceiptRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.TransferOrderId.HasValue && s.TransferOrderId == input.Id);

            var result = ObjectMapper.Map<TransferOrderDetailOutput>(@entity);
            result.TransferOrderItems = @TransferOrderItem;
            if (itemIssue != null) result.ItemIssueId = itemIssue.Id;
            if(itemReceipt != null) result.ItemReceiptId = itemReceipt.Id;

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_TransferOrder_ForItemIssue)]
        public async Task<TransferOrderDetailOutput> GetListTransferOrderForItemIssue(EntityDto<Guid> input)
        {
            var @entity = await _transferOrderManager.GetAsync(input.Id);
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            var locations = new List<long?>() {
                    entity.TransferFromLocationId
                };
            //var averageCosts = await _inventoryManager.GetAvgCostQuery(@entity.TransferDate.Date, locations).ToListAsync();
            var @TransferOrderItem = await _transferOrderItemRepository.GetAll()
                                    .Include(u => u.Item)
                                    .Include(u=>u.ToLot)
                                    .Include(u=>u.FromLot)
                                    .Where(u => u.TransferOrderId == entity.Id)
                                    .Select(t => new CreateOrUpdateTransferOrderItemInput
                                    {
                                        FromLotDetail = ObjectMapper.Map<LotSummaryOutput>(t.FromLot),
                                        ToLotDetail = ObjectMapper.Map<LotSummaryOutput>(t.ToLot),
                                        FromLotId = t.FromLotId,
                                        ToLotId = t.ToLotId,
                                        Description = t.Description,
                                        Id = t.Id,
                                        Item = _itemRepository.GetAll().Where(r => r.Id == t.ItemId)
                                            .Select(i => new ItemSummaryDetailOutput
                                            {
                                                Id = i.Id,
                                                InventoryAccountId = i.InventoryAccountId,
                                                InventoryAccount = ObjectMapper.Map<ChartAccountDetailOutput>(i.InventoryAccount),
                                                ItemCode = i.ItemCode,
                                                ItemName = i.ItemName,
                                                //AverageCost = averageCosts.Where(v => v.Id == t.ItemId).Select(c => c.AverageCost).FirstOrDefault(),
                                                //QtyOnHand = averageCosts.Where(v => v.Id == t.ItemId).Select(v => v.QtyOnHand).FirstOrDefault(),
                                            }).FirstOrDefault(),
                                        ItemId = t.ItemId,
                                        Unit = t.Qty,
                                        CreateTime = t.CreationTime,
                                        UseBatchNo = t.Item.UseBatchNo,
                                    })
                                    .OrderBy(t => t.CreateTime)
                                    .ToListAsync();
            var result = ObjectMapper.Map<TransferOrderDetailOutput>(@entity);
            result.TransferOrderItems = @TransferOrderItem;
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_TransferOrder_ForItemReceipt)]
        public async Task<TransferOrderDetailOutput> GetListTransferOrderForItemReceipt(EntityDto<Guid> input)
        {
            var @entity = await _transferOrderManager.GetAsync(input.Id);
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var itemIssueItems = await _itemIssueItemRepository.GetAll()
                            .Where(t => t.TransferOrderItemId != null)
                            .Select(t => new
                            {
                                UnitCost = t.UnitCost,
                                Qty = t.Qty,
                                TransferOrderItemId = t.TransferOrderItemId
                            })
                            .ToListAsync();

            var @TransferOrderItem = await _transferOrderItemRepository.GetAll()
                                    .Include(u => u.Item)
                                    .Include(u=>u.ToLot)
                                    .Include(u=>u.FromLot)
                                    .Where(u => u.TransferOrderId == entity.Id)
                                    .Select(t => new CreateOrUpdateTransferOrderItemInput
                                    {
                                        FromLotDetail = ObjectMapper.Map<LotSummaryOutput>(t.FromLot),
                                        ToLotDetail = ObjectMapper.Map<LotSummaryOutput>(t.ToLot),
                                        FromLotId = t.FromLotId,
                                        ToLotId = t.ToLotId,

                                        Description = t.Description,
                                        Id = t.Id,
                                        Item = _itemRepository.GetAll().Where(r => r.Id == t.ItemId)
                                            .Select(i => new ItemSummaryDetailOutput
                                            {
                                                Id = i.Id,
                                                InventoryAccountId = i.InventoryAccountId,
                                                InventoryAccount = ObjectMapper.Map<ChartAccountDetailOutput>(i.InventoryAccount),
                                                ItemCode = i.ItemCode,
                                                ItemName = i.ItemName,
                                                AverageCost = itemIssueItems.Where(v => v.TransferOrderItemId == t.Id).Select(c => c.UnitCost).FirstOrDefault(),
                                            }).FirstOrDefault(),
                                        ItemId = t.ItemId,
                                        Unit = t.Qty,
                                        CreateTime = t.CreationTime,
                                        UseBatchNo = t.Item.UseBatchNo
                                    })
                                    .OrderBy(t => t.CreateTime)
                                    .ToListAsync();

            var result = ObjectMapper.Map<TransferOrderDetailOutput>(@entity);

            var itemReceipt = await _itemReceiptRepository.GetAll().AsNoTracking().FirstAsync(s => s.TransferOrderId.HasValue && s.TransferOrderId == input.Id);
            if (itemReceipt != null)
            {
                result.ItemReceiptId = itemReceipt.Id;
            }

            var itemIssue = await _itemIssueRepository.GetAll().AsNoTracking().FirstAsync(s => s.TransferOrderId.HasValue && s.TransferOrderId == input.Id);
            if (itemIssue != null)
            {
                result.ItemIssueId = itemIssue.Id;
            }

            result.TransferOrderItems = @TransferOrderItem;
            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_TransferOrder_Update,
                      AppPermissions.Pages_Tenant_Inventory_TransferOrder_EditDelete48hour)]
        public async Task<NullableIdDto<Guid>> Update(UpdateTransferOrderInput input)
        {
            if (input.IsConfirm == false)
            {
                var validateLockDate = await _lockRepository.GetAll()
                                    .Where(t => t.IsLock == true &&  t.LockDate != null &&
                                    (t.LockDate.Value.Date >= input.DateCompare.Value.Date || t.LockDate.Value.Date >= input.TransferDate.Date)
                                    && t.LockKey == TransactionLockType.TransferOrder).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _transferOrderManager.GetAsync(input.Id, true);
            await CheckClosePeriod(entity.TransferDate, input.TransferDate);

            var getPermission = await GetUserPermissions(userId);
            var IsEdit = getPermission.Where(t => t == AppPermissions.Pages_Tenant_Inventory_TransferOrder_EditDelete48hour).Count();
            var totalHours = (DateTime.Now - @entity.CreationTime).TotalHours;
            if (IsEdit > 0 && totalHours > 48)
            {
                throw new UserFriendlyException(L("ThisTransactionIsOver48HoursCanNotEdit"));
            }

            if (input.ConvertToIssueAndReceiptTransfer) await ValidateBatchNo(input);

            var oldInvoiceConvert = entity.ConvertToIssueAndReceiptTransfer;

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            // validate on if this transaction is already use in other table
            var itemIssueCount = await _itemIssueRepository.GetAll().AsNoTracking().Where(v => v.TransferOrderId == entity.Id).CountAsync();
            var itemReceiptCount = await _itemReceiptRepository.GetAll().AsNoTracking().Where(v => v.TransferOrderId == entity.Id).CountAsync();
            if ((entity.ShipedStatus != TransferStatus.Pending || itemIssueCount > 0 || itemReceiptCount > 0) && oldInvoiceConvert == false)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            entity.Update(userId, input.TransferFromLocationId, input.TransferToLocationId,
                input.TransferFromClassId, input.TransferToClassId, input.Status, input.TransferNo, input.TransferDate,
                input.Reference, input.Memo, input.ConvertToIssueAndReceiptTransfer, input.ItemReceiptTransferDate, input.ItemIssueTransferDate);
            #region update TransferOrderItem           
            var TransferOrderItem = await _transferOrderItemRepository.GetAll().Where(u => u.TransferOrderId == entity.Id).ToListAsync();
            int indexLot = 0;
            foreach (var p in input.TransferOrderItems)
            {
                indexLot++;
                if (p.ToLotId == null || p.ToLotId == 0 || p.FromLotId == 0 || p.FromLotId == null)
                {
                    throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + indexLot.ToString());
                }
                if (p.FromLotId == p.ToLotId)
                {
                    throw new UserFriendlyException(L("CanNotTransfertheSameLot") + L("Row") + " " + indexLot.ToString());
                }
                if (p.Id != null)
                {
                    var @Transfer = TransferOrderItem.FirstOrDefault(u => u.Id == p.Id);
                    if (Transfer != null)
                    {
                        //here is in only same TransferOrder so no need to update TransferOrder
                        Transfer.Update(userId, p.ItemId, p.Description, p.Unit);
                        Transfer.UpdateFromLot(p.FromLotId.Value);
                        Transfer.UpdatToLot(p.ToLotId.Value);
                        CheckErrors(await _transferOrderItemManager.UpdateAsync(Transfer));
                    }
                }
                else if (p.Id == null)
                {
                    //@entity.Id is TransferId or input.Id is also TransferOrder Id so no need to pass TransferOrderId from outside
                    var @transferOrderItem = TransferOrders.TransferOrderItem.Create(tenantId, userId, entity.Id, p.ItemId, p.Description, p.Unit);

                    if (input.ConvertToIssueAndReceiptTransfer == true && input.Status == TransactionStatus.Publish)
                    {
                        p.Id = transferOrderItem.Id;
                    }

                    @transferOrderItem.UpdateFromLot(p.FromLotId.Value);
                    @transferOrderItem.UpdatToLot(p.ToLotId.Value);
                    base.CheckErrors(await _transferOrderItemManager.CreateAsync(@transferOrderItem));

                }
            }
            #endregion

            CheckErrors(await _transferOrderManager.UpdateAsync(@entity));
           
            await CurrentUnitOfWork.SaveChangesAsync();
            /////// Update Item Receipt or Item Issue ///////
            if (oldInvoiceConvert == true && input.ConvertToIssueAndReceiptTransfer == true && input.Status == TransactionStatus.Publish)
            {
                var currencyId = await _tenantRepository.GetAll().Where(t => t.Id == tenantId).Select(v => v.CurrencyId).FirstOrDefaultAsync();
                var iventoryAccount = await _itemRepository.GetAll().Where(t => t.TenantId == tenantId && input.TransferOrderItems.Any(v => v.ItemId == t.Id)).ToListAsync();

                var tenant = await GetCurrentTenantAsync();

                var ClearanceAccountId = tenant.ItemIssueTransferId;  

                if (ClearanceAccountId == null)
                {
                    throw new UserFriendlyException(L("PleaseCreateTransitAccountOnCompanyProfile"));
                }
                
                var itemIssueItems = await _itemIssueItemRepository.GetAll().Include(u=>u.ItemIssue).AsNoTracking()
                                 .Where(t => t.ItemIssue.TransferOrderId== input.Id)
                                 .Select(t => new
                                 {
                                     Id = t.Id,
                                     LotId = t.LotId,
                                     ItemId = t.ItemId,
                                     UnitCost = t.UnitCost,
                                     Qty = t.Qty,
                                     Total = t.Total,
                                     TransferOrderItemId = t.TransferOrderItemId,
                                     itemIssueId = t.ItemIssueId,
                                 }).ToListAsync();

                var ItemReceipts = await _itemReceiptItemRepository.GetAll().Include(u => u.ItemReceipt).AsNoTracking()
                              .Where(t => t.ItemReceipt.TransferOrderId == input.Id)
                              .Select(t => new
                              {
                                  Id = t.Id,
                                  LotId = t.LotId,
                                  Qty = t.Qty,
                                  TransferOrderItemId = t.TransferOrderItemId,
                                  itemReceiptId = t.ItemReceiptId,
                              }).ToListAsync();

                var createInputItemIssueTransfer = new UpdateItemIssueTransferInput()
                {
                    IsConfirm = input.IsConfirm,
                    Memo = input.Memo,
                    ReceiptNo = input.TransferNo,
                    Id = itemIssueItems.Select(t=>t.itemIssueId).FirstOrDefault(),
                    ItemReceiptId = ItemReceipts.Select(t => t.itemReceiptId).FirstOrDefault(),
                    BillingAddress = new Addresses.CAddress("", "", "", "", ""),
                    SameAsShippingAddress = true,
                    ShippingAddress = new Addresses.CAddress("", "", "", "", ""),
                    ClassId = input.TransferFromClassId,
                    CurrencyId = currencyId.Value,
                    Date = input.ItemIssueTransferDate.Value,
                    LocationId = input.TransferFromLocationId,
                    ReceiveFrom = ReceiveFrom.TransferOrder,
                    Reference = $"{input.TransferNo}/{input.Reference}",
                    Status = input.Status,
                    ClearanceAccountId = ClearanceAccountId.Value,
                    TransferOrderId = entity.Id,
                    Total = 0,
                    Items = input.TransferOrderItems.Select(t => new CreateOrUpdateItemIssueItemTransferInput
                    {
                        FromlotId = t.FromLotId,
                        Id = itemIssueItems.Where(g=>g.TransferOrderItemId == t.Id).Select(g=>g.Id).FirstOrDefault() ,    
                        Item = ObjectMapper.Map<ItemSummaryDetailOutput>(t.Item),
                        InventoryAccountId = (from Account in iventoryAccount where (Account.Id == t.ItemId) select (Account.InventoryAccountId.Value)).FirstOrDefault(),
                        Description = t.Description,
                        Qty = t.Unit,
                        ItemId = t.ItemId,
                        DiscountRate = 0,
                        TransferItemId = t.Id,
                        ItemBatchNos = t.ItemBatchNos
                    }).ToList(),

                };
                await UpdateItemIssueTransfer(createInputItemIssueTransfer);

                //input to item Receipt Transfer
                var ClearanceAccountIdForItemReceiptTransfer = tenant.ItemRecieptTransferId;

                //var ItemReceiptId = await _itemReceiptRepository.GetAll().AsNoTracking()
                //    .Where(t => t.TransferOrderId == input.Id).Select(t => t.Id).FirstOrDefaultAsync();

              
                var createInputItemRecepitTransfer = new UpdateItemReceiptTransferInput()
                {
                    IsConfirm = input.IsConfirm,
                    Memo = input.Memo,
                    ReceiptNo = input.TransferNo,
                    Id = ItemReceipts.Select(g=>g.itemReceiptId).FirstOrDefault(),
                    BillingAddress = new Addresses.CAddress("", "", "", "", ""),
                    SameAsShippingAddress = true,
                    ShippingAddress = new Addresses.CAddress("", "", "", "", ""),
                    ClassId = input.TransferToClassId,
                    CurrencyId = currencyId.Value,
                    Date = input.ItemIssueTransferDate.Value,
                    LocationId = input.TransferToLocationId,
                    ReceiveFrom = ItemReceipt.ReceiveFromStatus.TransferOrder,
                    Reference = $"{input.TransferNo}/{input.Reference}",
                    Status = input.Status,
                    ClearanceAccountId = ClearanceAccountIdForItemReceiptTransfer.Value,
                    //Total = input.to,
                    TransferOrderId = entity.Id,
                    Total = createInputItemIssueTransfer.Total,

                    Items = input.TransferOrderItems.Select(t => new CreateOrUpdateItemReceiptItemTransferInput
                    {
                        Id = ItemReceipts.Where(g => g.TransferOrderItemId == t.Id).Select(g => g.Id).FirstOrDefault(),
                        LotId = t.ToLotId,
                        InventoryAccountId = (from Account in iventoryAccount where (Account.Id == t.ItemId) select (Account.InventoryAccountId.Value)).FirstOrDefault(),
                        Qty = t.Unit,
                        ItemId = t.ItemId,
                        Total = createInputItemIssueTransfer.Items.Where(v=>v.TransferItemId == t.Id).Select(c=>c.Total).FirstOrDefault(),                     
                        UnitCost = createInputItemIssueTransfer.Items.Where(v => v.TransferItemId == t.Id).Select(c => c.UnitCost).FirstOrDefault(),
                        DiscountRate = 0,
                        Description = t.Description,
                        TransferOrderItemId = t.Id,
                        ItemBatchNos = t.ItemBatchNos
                    }).ToList()

                };
                await UpdateReceiptTransfer(createInputItemRecepitTransfer);

            }
            /////// Delte Item Receipt or Item Issue ///////
            if (oldInvoiceConvert == true && input.ConvertToIssueAndReceiptTransfer == false && input.Status == TransactionStatus.Publish)
            {
                var ItemIssueId = await _itemIssueRepository.GetAll()
                    .Where(t => t.TransferOrderId == input.Id).Select(t => t.Id).FirstOrDefaultAsync();
                var ItemReceiptId = await _itemReceiptRepository.GetAll()
                    .Where(t => t.TransferOrderId == input.Id).Select(t => t.Id).FirstOrDefaultAsync();
                var IssueId = new CarlEntityDto() {IsConfirm = input.IsConfirm, Id = ItemIssueId };
                var ReceiptId = new CarlEntityDto() { IsConfirm = input.IsConfirm, Id = ItemReceiptId };
                await DeleteItemReceiptTransfer(ReceiptId);
                await DeleteItemIssueTransfer(IssueId);
               
            }
            /////// Create Item Receipt or Item Issue ///////
            if (oldInvoiceConvert == false && input.ConvertToIssueAndReceiptTransfer == true && input.Status == TransactionStatus.Publish)
            {
                var currencyId = await _tenantRepository.GetAll().Where(t => t.Id == tenantId).Select(v => v.CurrencyId).FirstOrDefaultAsync();
                var iventoryAccount = await _itemRepository.GetAll().Where(t => t.TenantId == tenantId && input.TransferOrderItems.Any(v => v.ItemId == t.Id)).ToListAsync();

                var ClearanceAccountId = await _tenantRepository.GetAll().Where(t => t.Id == tenantId).Select(t => t.ItemIssueTransferId).FirstOrDefaultAsync();

                if (ClearanceAccountId == null)
                {
                    throw new UserFriendlyException(L("PleaseCreateTransitAccountOnCompanyProfile"));
                }
                var locations = new List<long?>() {
                    entity.TransferFromLocationId
                };
                //var averageCosts = await _inventoryManager.GetAvgCostQuery(@entity.TransferDate.Date, locations).ToListAsync();
                var createInputItemIssueTransfer = new CreateItemIssueTransferInput()
                {
                    IsConfirm = input.IsConfirm,
                    Memo = input.Memo,
                    ReceiptNo = input.TransferNo,
                    BillingAddress = new Addresses.CAddress("", "", "", "", ""),
                    SameAsShippingAddress = true,
                    ShippingAddress = new Addresses.CAddress("", "", "", "", ""),
                    ClassId = input.TransferFromClassId,
                    CurrencyId = currencyId.Value,
                    Date = input.ItemIssueTransferDate.Value,
                    LocationId = input.TransferFromLocationId,
                    ReceiveFrom = ReceiveFrom.TransferOrder,
                    Reference = $"{input.TransferNo}/{input.Reference}",
                    Status = input.Status,
                    ClearanceAccountId = ClearanceAccountId.Value,
                    //Total = input.to,
                    TransferOrderId = entity.Id,
                    Total = 0,

                    Items = input.TransferOrderItems.Select(t => new CreateOrUpdateItemIssueItemTransferInput
                    {
                        FromlotId = t.FromLotId,
                        Item = ObjectMapper.Map<ItemSummaryDetailOutput>(t.Item),
                        InventoryAccountId = (from Account in iventoryAccount where (Account.Id == t.ItemId) select (Account.InventoryAccountId.Value)).FirstOrDefault(),
                        Description = t.Description,
                        Qty = t.Unit,
                        ItemId = t.ItemId,
                        //Total = averageCosts.Where(v => v.Id == t.ItemId).Select(c => c.AverageCost * t.Unit).FirstOrDefault(),
                        //UnitCost = averageCosts.Where(v => v.Id == t.ItemId).Select(c => c.AverageCost).FirstOrDefault(),
                        DiscountRate = 0,
                        TransferItemId = t.Id,
                        ItemBatchNos = t.ItemBatchNos
                    }).ToList(),

                };
                await CreateItemIssueTransfer(createInputItemIssueTransfer);

                //input to item Receipt Transfer
                var ClearanceAccountIdForItemReceiptTransfer = await _tenantRepository.GetAll().Where(t => t.Id == tenantId).Select(t => t.ItemRecieptTransferId).FirstOrDefaultAsync();

                var itemIssue = await _itemIssueItemRepository.GetAll()
                           .Where(t => t.TransferOrderItemId != null)
                           .Select(t => new
                           {
                               Id = t.ItemId,
                               UnitCost = t.UnitCost,
                               Qty = t.Qty,
                               Total = t.Total,
                               TransferOrderItemId = t.TransferOrderItemId
                           })
                           .ToListAsync();

                var createInputItemRecepitTransfer = new CreateItemReceiptTransferInput()
                {

                    Memo = input.Memo,
                    ReceiptNo = input.TransferNo,

                    BillingAddress = new Addresses.CAddress("", "", "", "", ""),
                    SameAsShippingAddress = true,
                    ShippingAddress = new Addresses.CAddress("", "", "", "", ""),
                    ClassId = input.TransferToClassId,
                    CurrencyId = currencyId.Value,
                    Date = input.ItemIssueTransferDate.Value,
                    LocationId = input.TransferToLocationId,
                    ReceiveFrom = ItemReceipt.ReceiveFromStatus.TransferOrder,
                    Reference = $"{input.TransferNo}/{input.Reference}",
                    Status = input.Status,
                    ClearanceAccountId = ClearanceAccountIdForItemReceiptTransfer.Value,
                    //Total = input.to,
                    TransferOrderId = entity.Id,
                    Total = 0,

                    Items = input.TransferOrderItems.Select(t => new CreateOrUpdateItemReceiptItemTransferInput
                    {
                        LotId = t.ToLotId,
                        InventoryAccountId = (from Account in iventoryAccount where (Account.Id == t.ItemId) select (Account.InventoryAccountId.Value)).FirstOrDefault(),
                        Qty = t.Unit,
                        ItemId = t.ItemId,
                        Total = itemIssue.Where(v => v.TransferOrderItemId == t.Id).Select(c => c.Total).FirstOrDefault(),
                        UnitCost = itemIssue.Where(v => v.TransferOrderItemId == t.Id).Select(c => c.UnitCost).FirstOrDefault(),
                        DiscountRate = 0,
                        Description = t.Description,
                        TransferOrderItemId = t.Id,
                        ItemBatchNos = t.ItemBatchNos
                    }).ToList()

                };
                await CreateItemReceiptTransfer(createInputItemRecepitTransfer);
            }


            var @toDeleteTransferOrderItem = TransferOrderItem.Where(u => !input.TransferOrderItems.Any(i => i.Id != null && i.Id == u.Id)).ToList();

            foreach (var t in toDeleteTransferOrderItem)
            {

                CheckErrors(await _transferOrderItemManager.RemoveAsync(t));

            }
            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.TransferOrder };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }
        
        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_TransferOrder_UpdateStatusToPublish)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input)
        {
            var tenantId = AbpSession.GetTenantId();
            var @entity = await _transferOrderManager.GetAsync(input.Id, true);
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            //create item receipt transfer when click public

            if (entity.ConvertToIssueAndReceiptTransfer == true)
            {
                var currencyId = await _tenantRepository.GetAll().Where(t => t.Id == tenantId).Select(v => v.CurrencyId).FirstOrDefaultAsync();
                var iventoryAccount = await _itemRepository.GetAll().Where(t => t.TenantId == tenantId && _transferOrderItemRepository.GetAll().Any(v => v.ItemId == t.Id)).ToListAsync();

                var ClearanceAccountId = await _tenantRepository.GetAll().Where(t => t.Id == tenantId).Select(t => t.ItemIssueTransferId).FirstOrDefaultAsync();

                if (ClearanceAccountId == null)
                {
                    throw new UserFriendlyException(L("PleaseCreateTransitAccountOnCompanyProfile"));
                }
                var locations = new List<long?>() {
                    entity.TransferFromLocationId
                };
                //var averageCosts = await _inventoryManager.GetAvgCostQuery(@entity.TransferDate.Date, locations).ToListAsync();
                var createInputItemIssueTransfer = new CreateItemIssueTransferInput()
                {
                    Memo = entity.Memo,
                    ReceiptNo = entity.TransferNo,

                    BillingAddress = new Addresses.CAddress("", "", "", "", ""),
                    SameAsShippingAddress = true,
                    ShippingAddress = new Addresses.CAddress("", "", "", "", ""),
                    ClassId = entity.TransferFromClassId,
                    CurrencyId = currencyId.Value,
                    Date = entity.ItemIssueTransferDate.Value,
                    LocationId = entity.TransferFromLocationId,
                    ReceiveFrom = ReceiveFrom.TransferOrder,
                    Reference = entity.Reference,
                    Status = TransactionStatus.Publish,
                    ClearanceAccountId = ClearanceAccountId.Value,
                    //Total = input.to,
                    TransferOrderId = entity.Id,
                    Total = 0,

                    Items = _transferOrderItemRepository.GetAll().Include(u => u.TransferOrder).Where(d => d.TransferOrder.Id == input.Id).Select(t => new CreateOrUpdateItemIssueItemTransferInput
                    {
                        Item = ObjectMapper.Map<ItemSummaryDetailOutput>(t.Item),
                        InventoryAccountId = (from Account in iventoryAccount where (Account.Id == t.ItemId) select (Account.InventoryAccountId.Value)).FirstOrDefault(),
                        Description = t.Description,
                        Qty = t.Qty,
                        ItemId = t.ItemId,
                        //Total = averageCosts.Where(v => v.Id == t.ItemId).Select(c => c.AverageCost * t.Qty).FirstOrDefault(),
                        //UnitCost = averageCosts.Where(v => v.Id == t.ItemId).Select(c => c.AverageCost).FirstOrDefault(),
                        DiscountRate = 0,
                        TransferItemId = t.Id,

                    }).ToList(),

                };
                await CreateItemIssueTransfer(createInputItemIssueTransfer);

                //input to item Receipt Transfer and item issue
                var ClearanceAccountIdForItemReceiptTransfer = await _tenantRepository.GetAll().Where(t => t.Id == tenantId).Select(t => t.ItemRecieptTransferId).FirstOrDefaultAsync();

                var itemIssue = await _itemIssueItemRepository.GetAll()
                           .Where(t => t.TransferOrderItemId != null)
                           .Select(t => new
                           {
                               Id = t.ItemId,
                               UnitCost = t.UnitCost,
                               Qty = t.Qty,
                               TransferOrderItemId = t.TransferOrderItemId
                           })
                           .ToListAsync();

                var createInputItemRecepitTransfer = new CreateItemReceiptTransferInput()
                {

                    Memo = entity.Memo,
                    ReceiptNo = entity.TransferNo,

                    BillingAddress = new Addresses.CAddress("", "", "", "", ""),
                    SameAsShippingAddress = true,
                    ShippingAddress = new Addresses.CAddress("", "", "", "", ""),
                    ClassId = entity.TransferToClassId,
                    CurrencyId = currencyId.Value,
                    Date = entity.ItemIssueTransferDate.Value,
                    LocationId = entity.TransferToLocationId,
                    ReceiveFrom = ItemReceipt.ReceiveFromStatus.TransferOrder,
                    Reference = entity.Reference,
                    Status = TransactionStatus.Publish,
                    ClearanceAccountId = ClearanceAccountIdForItemReceiptTransfer.Value,
                    //Total = input.to,
                    TransferOrderId = entity.Id,
                    Total = 0,

                    Items = _transferOrderItemRepository.GetAll().Select(t => new CreateOrUpdateItemReceiptItemTransferInput
                    {
                        InventoryAccountId = (from Account in iventoryAccount where (Account.Id == t.ItemId) select (Account.InventoryAccountId.Value)).FirstOrDefault(),
                        Qty = t.Qty,
                        ItemId = t.ItemId,
                        Total = itemIssue.Where(v => v.Id == t.ItemId).Select(c => c.UnitCost * t.Qty).FirstOrDefault(),
                        UnitCost = itemIssue.Where(v => v.TransferOrderItemId == t.Id).Select(c => c.UnitCost).FirstOrDefault(),
                        DiscountRate = 0,
                        Description = t.Description,
                        TransferOrderItemId = t.Id,
                    }).ToList()

                };
                await CreateItemReceiptTransfer(createInputItemRecepitTransfer);

            }


            entity.UpdateStatusToPublish();
            CheckErrors(await _transferOrderManager.UpdateAsync(entity));
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_TransferOrder_UpdateStatusToVoid)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input)
        {
            var @entity = await _transferOrderManager.GetAsync(input.Id, true);
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            if (entity.ShipedStatus != TransferStatus.Pending && entity.ConvertToIssueAndReceiptTransfer == false)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            var itemReciptTransferId = _itemReceiptRepository.GetAll().Where(g => g.TransferOrderId == input.Id).Select(t => t.Id).FirstOrDefault();
            var itemIssueTransferId = _itemIssueRepository.GetAll().Where(g => g.TransferOrderId == input.Id).Select(t => t.Id).FirstOrDefault();

            if (entity.ConvertToIssueAndReceiptTransfer == true && itemReciptTransferId != null && itemIssueTransferId != null)
            {
                var @jounalReceiptTransfer = await _journalRepository
                .GetAll()
                .Include(u => u.ItemReceipt)
                .Include(u => u.Bill)
                .Include(u => u.ItemReceipt.ShippingAddress)
                .Include(u => u.ItemReceipt.BillingAddress)
                             .Where(u => (u.JournalType == JournalType.ItemReceiptPurchase ||
                                 u.JournalType == JournalType.ItemReceiptAdjustment ||
                                 u.JournalType == JournalType.ItemReceiptOther ||
                                 u.JournalType == JournalType.ItemReceiptTransfer)
                                 && u.ItemReceipt.Id == itemReciptTransferId)
                             .FirstOrDefaultAsync();

                //update receive status of transfer order to ship all 
                if (jounalReceiptTransfer.JournalType == JournalType.ItemReceiptTransfer)
                {
                    // void item receipt transfer
                    var @transferItemReceipt = await _transferOrderRepository.GetAll().Where(u => u.Id == jounalReceiptTransfer.ItemReceipt.TransferOrderId).FirstOrDefaultAsync();
                    // update received status 
                    if (@transferItemReceipt != null)
                    {
                        @transferItemReceipt.UpdateShipedStatus(TransferStatus.ShipAll);
                        CheckErrors(await _transferOrderManager.UpdateAsync(@transferItemReceipt));
                    }
                    jounalReceiptTransfer.UpdateVoid();
                    //void item issue transfer                
                    var @jounalIssue = await _journalRepository.GetAll()
                    .Include(u => u.ItemIssue)
                    .Include(u => u.Bill)
                    .Include(u => u.ItemIssue.ShippingAddress)
                    .Include(u => u.ItemIssue.BillingAddress)
                    .Where(u => (u.JournalType == JournalType.ItemIssueSale ||
                                       u.JournalType == JournalType.ItemIssueOther ||
                                        u.JournalType == JournalType.ItemIssueTransfer ||
                                       u.JournalType == JournalType.ItemIssueAdjustment ||
                                       u.JournalType == JournalType.ItemIssueVendorCredit)
                                       && u.ItemIssueId == itemIssueTransferId)
                    .FirstOrDefaultAsync();
                    //update receive status of transfer order to pending 
                    if (@jounalIssue.JournalType == JournalType.ItemIssueTransfer)
                    {
                        var @transferissue = await _transferOrderRepository.GetAll().Where(u => u.Id == @jounalIssue.ItemIssue.TransferOrderId).FirstOrDefaultAsync();
                        // update received status 
                        if (@jounalIssue != null)
                        {
                            @transferissue.UpdateShipedStatus(TransferStatus.Pending);
                            CheckErrors(await _transferOrderManager.UpdateAsync(@transferissue));
                        }
                    }
                    jounalIssue.UpdateVoid();
                }

            }

            entity.UpdateStatusToVoid();
            CheckErrors(await _transferOrderManager.UpdateAsync(entity));
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_TransferOrder_UpdateStatusToDraft)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input)
        {
            var @entity = await _transferOrderManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            if (entity.ShipedStatus != TransferStatus.Pending && entity.ConvertToIssueAndReceiptTransfer == false)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            entity.UpdateStatusToDraft();
            CheckErrors(await _transferOrderManager.UpdateAsync(entity));
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }


    }
}
