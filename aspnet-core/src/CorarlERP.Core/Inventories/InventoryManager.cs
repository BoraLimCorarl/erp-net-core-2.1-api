using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using CorarlERP.AccountCycles;
using CorarlERP.Authorization.Users;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Customers;
using CorarlERP.Inventories.Data;
using CorarlERP.InventoryCostCloses;
using CorarlERP.InventoryTransactionItems;
using CorarlERP.ItemIssues;
using CorarlERP.ItemIssueVendorCredits;
using CorarlERP.ItemReceiptCustomerCredits;
using CorarlERP.ItemReceipts;
using CorarlERP.Items;
using CorarlERP.Journals;
using CorarlERP.Locations;
using CorarlERP.Lots;
using CorarlERP.Reports.Dto;
using CorarlERP.UserGroups;
using CorarlERP.Vendors;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.MultiTenancy;
using TransactionStatus = CorarlERP.enumStatus.EnumStatus.TransactionStatus;
using CorarlERP.Productions;
using CorarlERP.TransferOrders;
using CorarlERP.BatchNos;
using static CorarlERP.Journals.Journal;

namespace CorarlERP.Inventories
{
    public class InventoryManager : CorarlERPDomainServiceBase, IInventoryManager
    {


        private readonly IJournalManager _journalManager;
        private readonly IJournalItemManager _journalItemManager;
        private readonly ICorarlRepository<JournalItem, Guid> _journalItemRepository;

        private readonly IRepository<ItemReceiptItemCustomerCredit, Guid> _itemReceiptCustomerCreditItemRepository;
        private readonly IRepository<ItemReceiptCustomerCredit, Guid> _itemReceiptCustomerCreditRepository;
        private readonly IRepository<ItemReceiptItem, Guid> _itemReceiptItemRepository;
        private readonly IRepository<ItemReceipt, Guid> _itemReceiptRepository;
        private readonly IRepository<ItemIssueItem, Guid> _itemIssueItemRepository;
        private readonly IRepository<ItemIssue, Guid> _itemIssueRepository;
        private readonly IRepository<ItemIssueVendorCreditItem, Guid> _itemIssueVendorCreditItemRepository;
        private readonly IRepository<ItemIssueVendorCredit, Guid> _itemIssueVendorCreditRepository;
        private readonly ICorarlRepository<Journal, Guid> _journalRepository;
        private readonly IRepository<InventoryCostCloseItem, Guid> _inventoryCostCloseItemRepository;
        private readonly IRepository<InventoryCostClose, Guid> _inventoryCostCloseRepository;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;
        private readonly IRepository<InventoryCostCloseItemBatchNo, Guid> _inventoryCostCloseItemBatchNoRepository;
        private readonly IRepository<BatchNo, Guid> _batchNoRepository;

        private readonly IRepository<ItemReceiptItemBatchNo, Guid> _itemReceiptItemBatchNoRepository;
        private readonly IRepository<ItemIssueItemBatchNo, Guid> _itemIssueItemBatchNoRepository;
        private readonly IRepository<ItemIssueVendorCreditItemBatchNo, Guid> _itemIssueVendorCreditItemBatchNoRepository;
        private readonly IRepository<ItemReceiptCustomerCreditItemBatchNo, Guid> _itemReceiptCustomerCreditItemBatchNoRepository;


        private readonly IRepository<Location, long> _locationRepository;
        private readonly IRepository<Lot, long> _lotRepository;
        private readonly IRepository<Item, Guid> _itemRepository;
        private readonly IRepository<ItemsUserGroup, Guid> _itemUserGroupRepository;
        private readonly IRepository<UserGroupMember, Guid> _userGroupMemberRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Vendor, Guid> _vendorRepository;
        private readonly IRepository<Customer, Guid> _customerRepository;
        private readonly IRepository<ProductionProcesses.ProductionProcess, long> _productionProcessesRepository;
        private readonly ICorarlRepository<Production, Guid> _productionRepository;
        private readonly IRepository<TransferOrder, Guid> _transferRepository;
        private readonly ICorarlRepository<InventoryTransactionItem, Guid> _inventoryTransactionItemRepository;

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<AccountCycle, long> _accountCycleRepository;
        private readonly IRepository<Tenant, int> _tenantRepository;

        public InventoryManager(
            IRepository<Item, Guid> itemRepository,
            IRepository<ItemsUserGroup, Guid> itemUserGroupRepository,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            IJournalManager journalManager,
            IJournalItemManager journalItemManager,
            ICorarlRepository<JournalItem, Guid> journalItemRepository,
            IRepository<ItemReceiptItem, Guid> itemReceiptItemRepository,
            IRepository<ItemReceipt, Guid> itemReceiptRepository,
            IRepository<InventoryCostCloseItem, Guid> inventoryCostCloseItemRepository,
            IRepository<InventoryCostCloseItemBatchNo, Guid> inventoryCostCloseItemBatchNoRepository,
            IRepository<BatchNo, Guid> batchNoRepository,
            IRepository<ItemReceiptItemBatchNo, Guid> itemReceiptItemBatchNoRepository,
            IRepository<ItemIssueItemBatchNo, Guid> itemIssueItemBatchNoRepository,
            IRepository<ItemIssueVendorCreditItemBatchNo, Guid> itemIssueVendorCreditItemBatchNoRepository,
            IRepository<ItemReceiptCustomerCreditItemBatchNo, Guid> itemReceiptCustomerCreditItemBatchNoRepository,
            IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
            IRepository<ItemReceiptItemCustomerCredit, Guid> itemReceiptCustomerCreditItemRepository,
            IRepository<ItemIssueVendorCreditItem, Guid> itemIssueVendorCreditItemRepository,
            ICorarlRepository<Journal, Guid> journalRepository,
            IRepository<ItemIssueItem, Guid> itemIssueItemRepository,
            IRepository<ItemIssue, Guid> itemIssueRepository,
            IRepository<AccountCycle, long> accountCycleRepository,
            IRepository<Location, long> locationRepository,
            ICorarlRepository<InventoryTransactionItem, Guid> inventoryTransactionItemRepository,
            ICorarlRepository<Production, Guid> productionRepository,
            IRepository<TransferOrder, Guid> transferRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<Tenant, int> tenantRepository
        ) : base(accountCycleRepository)
        {
            _locationRepository = locationRepository;
            _itemUserGroupRepository = itemUserGroupRepository;
            _itemRepository = itemRepository;
            _userGroupMemberRepository = userGroupMemberRepository;
            _journalManager = journalManager;
            _journalItemRepository = journalItemRepository;
            _journalItemManager = journalItemManager;
            _itemReceiptItemRepository = itemReceiptItemRepository;
            _itemIssueVendorCreditItemRepository = itemIssueVendorCreditItemRepository;
            _itemReceiptCustomerCreditItemRepository = itemReceiptCustomerCreditItemRepository;
            _journalRepository = journalRepository;
            _itemIssueItemRepository = itemIssueItemRepository;
            _itemIssueRepository = itemIssueRepository;
            _inventoryCostCloseItemRepository = inventoryCostCloseItemRepository;
            _inventoryCostCloseRepository = IocManager.Instance.Resolve<IRepository<InventoryCostClose, Guid>>();
            _chartOfAccountRepository = chartOfAccountRepository;
            _itemReceiptRepository = itemReceiptRepository;
            _lotRepository = IocManager.Instance.Resolve<IRepository<Lot, long>>();
            _userRepository = IocManager.Instance.Resolve<IRepository<User, long>>();
            _vendorRepository = IocManager.Instance.Resolve<IRepository<Vendor, Guid>>();
            _customerRepository = IocManager.Instance.Resolve<IRepository<Customer, Guid>>();
            _itemReceiptCustomerCreditRepository = IocManager.Instance.Resolve<IRepository<ItemReceiptCustomerCredit, Guid>>();
            _itemIssueVendorCreditRepository = IocManager.Instance.Resolve<IRepository<ItemIssueVendorCredit, Guid>>();
            _itemIssueRepository = itemIssueRepository;
            _productionProcessesRepository = IocManager.Instance.Resolve<IRepository<ProductionProcesses.ProductionProcess, long>>();
            _inventoryTransactionItemRepository = inventoryTransactionItemRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _accountCycleRepository = accountCycleRepository;
            _tenantRepository = tenantRepository;
            _productionRepository = productionRepository;
            _transferRepository = transferRepository;
            _inventoryCostCloseItemBatchNoRepository = inventoryCostCloseItemBatchNoRepository;
            _batchNoRepository = batchNoRepository;
            _itemReceiptItemBatchNoRepository = itemReceiptItemBatchNoRepository;
            _itemIssueItemBatchNoRepository = itemIssueItemBatchNoRepository;
            _itemReceiptCustomerCreditItemBatchNoRepository = itemReceiptCustomerCreditItemBatchNoRepository;
            _itemIssueVendorCreditItemBatchNoRepository = itemIssueVendorCreditItemBatchNoRepository;
        }

        public IQueryable<ItemDto> GetInventoryDetailQuery(
                                   DateTime? fromDate, DateTime? toDate,
                                   List<long?> Locations,
                                   Dictionary<Guid, Guid> updatedItemIssueIds = null,
                                   Dictionary<Guid, Guid> itemsToSelect = null
            )
        {

            var previousCycle = GetPreviousCloseCyleAsync(toDate ?? DateTime.Now);
            var currentDigit = GetCyleAsync(toDate ?? DateTime.Now);
            var roundDigit = currentDigit != null ? currentDigit.RoundingDigit : 2;
            var roundingDigit = previousCycle != null ? previousCycle.RoundingDigit : roundDigit;
            var roundingDigitUnitCost = previousCycle != null ? previousCycle.RoundingDigitUnitCost : roundDigit;

            var itemReceiptItemQuery = (from Ir in _itemReceiptItemRepository.GetAll()
                                                  .WhereIf(updatedItemIssueIds != null && updatedItemIssueIds.Count > 0, t => !updatedItemIssueIds.ContainsKey(t.ItemReceiptId))
                                                   .WhereIf(itemsToSelect != null && itemsToSelect.Count > 0, t => itemsToSelect.ContainsKey(t.ItemId))
                                                   .AsNoTracking()
                                        join j in _journalRepository.GetAll()
                                                .Where(s => s.ItemReceiptId.HasValue)
                                                .Where(s => s.Status == TransactionStatus.Publish)
                                                .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                                .WhereIf(toDate != null, (u => (u.Date.Date) <= (toDate.Value.Date)))
                                                .AsNoTracking()
                                        on Ir.ItemReceiptId equals j.ItemReceiptId

                                        select new ItemDto
                                        {
                                            JournalReference = j.Reference,
                                            CreationTime = Ir.CreationTime,
                                            TransferOrderItemId = Ir.TransferOrderItemId,
                                            IsPurchase = true,
                                            ItemId = Ir.ItemId,
                                            TransactionId = Ir.ItemReceiptId,
                                            TotalCost = Ir.Total,
                                            Qty = Ir.Qty,
                                            LocationId = Ir.Lot.LocationId,
                                            JournalDate = j.Date,
                                            JournalType = j.JournalType,
                                            UnitCost = Ir.UnitCost,
                                            TransactionItemId = Ir.Id,
                                            JournalNo = j.JournalNo,
                                            JournalId = j.Id,
                                            CreationTimeIndex = j.CreationTimeIndex,
                                            LotId = Ir.LotId == null ? 0 : Ir.LotId.Value,
                                            LotName = Ir.Lot.LotName,
                                            OrderId = Ir.ItemReceipt.TransferOrderId != null ? Ir.ItemReceipt.TransferOrderId : Ir.ItemReceipt.ProductionOrderId,
                                            RoundingDigit = roundingDigit,
                                            RoundingDigitUnitCost = roundingDigitUnitCost
                                        });

            var itemReceiptCustomerCreditItemQuery = (from IrC in _itemReceiptCustomerCreditItemRepository.GetAll()
                                                                    .WhereIf(itemsToSelect != null && itemsToSelect.Count > 0, t => itemsToSelect.ContainsKey(t.ItemId))
                                                                    .AsNoTracking()
                                                      join J in _journalRepository.GetAll()
                                                              .Where(s => s.ItemReceiptCustomerCreditId.HasValue)
                                                              .Where(s => s.Status == TransactionStatus.Publish)
                                                              .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                                              .WhereIf(toDate != null, (u => (u.Date.Date) <= (toDate.Value.Date)))
                                                              .AsNoTracking()
                                                      on IrC.ItemReceiptCustomerCreditId equals J.ItemReceiptCustomerCreditId
                                                      select new ItemDto
                                                      {
                                                          JournalReference = J.Reference,
                                                          CreationTime = IrC.CreationTime,
                                                          TransferOrderItemId = null,
                                                          IsPurchase = true,
                                                          ItemId = IrC.Item.Id,
                                                          TransactionId = IrC.ItemReceiptCustomerCreditId,
                                                          JournalType = J.JournalType,
                                                          UnitCost = IrC.UnitCost,
                                                          TransactionItemId = IrC.Id,
                                                          TotalCost = IrC.Total,
                                                          Qty = IrC.Qty,
                                                          JournalDate = J.Date,
                                                          LocationId = IrC.Lot.LocationId,
                                                          LotId = IrC.LotId == null ? 0 : IrC.LotId.Value,
                                                          LotName = IrC.Lot.LotName,
                                                          JournalNo = J.JournalNo,
                                                          JournalId = J.Id,
                                                          CreationTimeIndex = J.CreationTimeIndex,
                                                          OrderId = null,
                                                          RoundingDigit = roundingDigit,
                                                          RoundingDigitUnitCost = roundingDigitUnitCost
                                                      });
            var itemReceiptResult = itemReceiptItemQuery.Concat(itemReceiptCustomerCreditItemQuery);

            var itemIssueItemQuery = (from isue in _itemIssueItemRepository.GetAll()
                                            .WhereIf(updatedItemIssueIds != null && updatedItemIssueIds.Count > 0, u => !updatedItemIssueIds.ContainsKey(u.ItemIssueId))
                                            .WhereIf(itemsToSelect != null && itemsToSelect.Count > 0, t => itemsToSelect.ContainsKey(t.ItemId))
                                            .AsNoTracking()
                                      join j in _journalRepository.GetAll()
                                                .Where(s => s.ItemIssueId.HasValue)
                                              .Where(s => s.Status == TransactionStatus.Publish)
                                              .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                              .WhereIf(toDate != null, (u => (u.Date.Date) <= (toDate.Value.Date)))
                                              .AsNoTracking()
                                      on isue.ItemIssueId equals j.ItemIssueId
                                      select new ItemDto
                                      {
                                          JournalReference = j.Reference,
                                          CreationTime = isue.CreationTime,
                                          TransferOrderItemId = isue.TransferOrderItemId,
                                          IsPurchase = false,
                                          ItemId = isue.Item.Id,
                                          TransactionId = isue.ItemIssueId,
                                          TotalCost = (isue.Total * -1),
                                          Qty = (isue.Qty * -1),
                                          LocationId = isue.Lot.LocationId,
                                          JournalType = j.JournalType,
                                          JournalDate = j.Date,
                                          UnitCost = isue.UnitCost,
                                          TransactionItemId = isue.Id,
                                          JournalNo = j.JournalNo,
                                          JournalId = j.Id,
                                          CreationTimeIndex = j.CreationTimeIndex,
                                          LotId = isue.LotId == null ? 0 : isue.LotId.Value,
                                          LotName = isue.Lot.LotName,
                                          OrderId = isue.ItemIssue.TransferOrderId != null ? isue.ItemIssue.TransferOrderId : isue.ItemIssue.ProductionOrderId,
                                          RoundingDigit = roundingDigit,
                                          RoundingDigitUnitCost = roundingDigitUnitCost
                                      });

            var itemIssueVendorCreditItemQuery = (from isv in _itemIssueVendorCreditItemRepository.GetAll()
                                                            .WhereIf(updatedItemIssueIds != null && updatedItemIssueIds.Count > 0, u => !updatedItemIssueIds.ContainsKey(u.ItemIssueVendorCreditId))
                                                            .WhereIf(itemsToSelect != null && itemsToSelect.Count > 0, t => itemsToSelect.ContainsKey(t.ItemId))
                                                            .AsNoTracking()
                                                  join j in _journalRepository.GetAll()
                                                          .Where(s => s.ItemIssueVendorCreditId.HasValue)
                                                          .Where(s => s.Status == TransactionStatus.Publish)
                                                          .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                                          .WhereIf(toDate != null, (u => (u.Date.Date) <= (toDate.Value.Date)))
                                                          .AsNoTracking()
                                                  on isv.ItemIssueVendorCreditId equals j.ItemIssueVendorCreditId
                                                  select new ItemDto
                                                  {
                                                      JournalReference = j.Reference,
                                                      CreationTime = isv.CreationTime,
                                                      TransferOrderItemId = null,
                                                      IsPurchase = false,
                                                      ItemId = isv.Item.Id,
                                                      TransactionId = isv.ItemIssueVendorCreditId,
                                                      TotalCost = (isv.Total * -1),
                                                      Qty = (isv.Qty * -1),
                                                      LocationId = isv.Lot.LocationId,
                                                      LocationName = isv.Lot.Location.LocationName,
                                                      JournalType = j.JournalType,
                                                      JournalDate = j.Date,
                                                      UnitCost = isv.UnitCost,
                                                      TransactionItemId = isv.Id,
                                                      LotId = isv.LotId == null ? 0 : isv.LotId.Value,
                                                      LotName = isv.Lot.LotName,
                                                      JournalNo = j.JournalNo,
                                                      OrderId = null,
                                                      JournalId = j.Id,
                                                      CreationTimeIndex = j.CreationTimeIndex,
                                                      RoundingDigit = roundingDigit,
                                                      RoundingDigitUnitCost = roundingDigitUnitCost
                                                  });
            var itemIssueResult = itemIssueItemQuery.Concat(itemIssueVendorCreditItemQuery);

            var inventoryTransactionItemQuery = itemReceiptResult.Concat(itemIssueResult);

            return inventoryTransactionItemQuery;
        }


        public IQueryable<ItemBalanceDto> GetInventoryItemBalanceQuery(
                                   string filter,
                                   DateTime? fromDate, DateTime? toDate,
                                   List<long?> location,
                                   Dictionary<Guid, Guid> updatedItemIssueIds = null, FilterType filterType = FilterType.Contain)
        {

            var itemQuery = _itemRepository.GetAll()
                            .WhereIf(!filter.IsNullOrEmpty(), p =>
                                    (filterType == FilterType.Contain && (p.ItemName.ToLower().Contains(filter.ToLower()) || p.ItemCode.ToLower().Contains(filter.ToLower()) || (p.Barcode != null && p.Barcode.ToLower().Contains(filter.ToLower())))) ||
                                    (filterType == FilterType.StartWith && (p.ItemName.ToLower().StartsWith(filter.ToLower()) || p.ItemCode.ToLower().StartsWith(filter.ToLower()) || (p.Barcode != null && p.Barcode.ToLower().StartsWith(filter.ToLower())))) ||
                                    (filterType == FilterType.Exact && (p.ItemName.ToLower().Equals(filter.ToLower()) || p.ItemCode.ToLower().Equals(filter.ToLower()) || (p.Barcode.ToLower() != null && p.Barcode.Equals(filter.ToLower()))))
                            )
                            .AsNoTracking()
                            .Select(s => s.Id);


            var itemReceiptItemQuery = from Ir in _itemReceiptItemRepository.GetAll()
                                                    .WhereIf(location != null && location.Count > 0, t => location.Contains(t.Lot.LocationId))
                                                    .WhereIf(updatedItemIssueIds != null && updatedItemIssueIds.Count > 0, t => !updatedItemIssueIds.ContainsKey(t.ItemReceiptId))
                                                    .AsNoTracking()
                                                    .Select(s => new
                                                    {
                                                        ItemReceiptId = s.ItemReceiptId,
                                                        ItemId = s.ItemId,
                                                        LotId = s.LotId,
                                                        LotName = s.Lot == null ? "" : s.Lot.LotName,
                                                        Qty = s.Qty
                                                    })
                                       join j in _journalRepository.GetAll()
                                                   .Where(s => s.Status == TransactionStatus.Publish)
                                                   .Where(s => s.ItemReceiptId.HasValue)
                                                   .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                                   .WhereIf(toDate != null, u => u.Date.Date <= toDate.Value.Date)
                                                   .AsNoTracking()
                                                   .Select(s => new
                                                   {
                                                       ItemReceiptId = s.ItemReceiptId
                                                   })
                                       on Ir.ItemReceiptId equals j.ItemReceiptId
                                       join i in itemQuery
                                       on Ir.ItemId equals i
                                       select new ItemBalanceDto
                                       {
                                           IsPurchase = true,
                                           ItemId = Ir.ItemId,
                                           Qty = Ir.Qty,
                                           LotId = Ir.LotId ?? 0,
                                           LotName = Ir.LotName
                                       };

            var itemReceiptCustomerCreditItemQuery = from IrC in _itemReceiptCustomerCreditItemRepository.GetAll()
                                                                .WhereIf(location != null && location.Count > 0, t => location.Contains(t.Lot.LocationId))
                                                                .WhereIf(updatedItemIssueIds != null && updatedItemIssueIds.Count > 0, u => !updatedItemIssueIds.ContainsKey(u.ItemReceiptCustomerCreditId))
                                                                .AsNoTracking()
                                                                .Select(s => new
                                                                {
                                                                    ItemReceiptCustomerCreditId = s.ItemReceiptCustomerCreditId,
                                                                    ItemId = s.ItemId,
                                                                    LotId = s.LotId,
                                                                    LotName = s.Lot == null ? "" : s.Lot.LotName,
                                                                    Qty = s.Qty
                                                                })
                                                     join J in _journalRepository.GetAll()
                                                                 .Where(s => s.Status == TransactionStatus.Publish)
                                                                 .Where(s => s.ItemReceiptCustomerCreditId.HasValue)
                                                                 .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                                                 .WhereIf(toDate != null, u => u.Date.Date <= toDate.Value.Date)
                                                                 .AsNoTracking()
                                                                 .Select(s => new
                                                                 {
                                                                     ItemReceiptCustomerCreditId = s.ItemReceiptCustomerCreditId
                                                                 })
                                                     on IrC.ItemReceiptCustomerCreditId equals J.ItemReceiptCustomerCreditId
                                                     join i in itemQuery
                                                     on IrC.ItemId equals i
                                                     select new ItemBalanceDto
                                                     {
                                                         IsPurchase = true,
                                                         ItemId = IrC.ItemId,
                                                         Qty = IrC.Qty,
                                                         LotId = IrC.LotId ?? 0,
                                                         LotName = IrC.LotName
                                                     };

            var itemReceiptResult = itemReceiptItemQuery.Concat(itemReceiptCustomerCreditItemQuery);

            var itemIssueItemQuery = from isue in _itemIssueItemRepository.GetAll()
                                                  .WhereIf(location != null && location.Count > 0, t => location.Contains(t.Lot.LocationId))
                                                  .WhereIf(updatedItemIssueIds != null && updatedItemIssueIds.Count > 0, u => !updatedItemIssueIds.ContainsKey(u.ItemIssueId))
                                                  .AsNoTracking()
                                                  .Select(s => new
                                                  {
                                                      ItemIssueId = s.ItemIssueId,
                                                      ItemId = s.ItemId,
                                                      LotId = s.LotId,
                                                      LotName = s.Lot == null ? "" : s.Lot.LotName,
                                                      Qty = s.Qty
                                                  })
                                     join j in _journalRepository.GetAll()
                                                 .Where(s => s.Status == TransactionStatus.Publish)
                                                 .Where(s => s.ItemIssueId.HasValue)
                                                 .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                                 .WhereIf(toDate != null, u => u.Date.Date <= toDate.Value.Date)
                                                 .AsNoTracking()
                                                 .Select(s => new
                                                 {
                                                     ItemIssueId = s.ItemIssueId
                                                 })
                                     on isue.ItemIssueId equals j.ItemIssueId
                                     join i in itemQuery
                                     on isue.ItemId equals i
                                     select new ItemBalanceDto
                                     {
                                         IsPurchase = false,
                                         ItemId = isue.ItemId,
                                         Qty = (isue.Qty * -1),
                                         LotId = isue.LotId ?? 0,
                                         LotName = isue.LotName
                                     };

            var itemIssueVendorCreditItemQuery = from isv in _itemIssueVendorCreditItemRepository.GetAll()
                                                            .WhereIf(location != null && location.Count > 0, t => location.Contains(t.Lot.LocationId))
                                                            .WhereIf(updatedItemIssueIds != null && updatedItemIssueIds.Count > 0, u => !updatedItemIssueIds.ContainsKey(u.ItemIssueVendorCreditId))
                                                            .AsNoTracking()
                                                            .Select(s => new
                                                            {
                                                                ItemIssueVendorCreditId = s.ItemIssueVendorCreditId,
                                                                ItemId = s.ItemId,
                                                                LotId = s.LotId,
                                                                LotName = s.Lot == null ? "" : s.Lot.LotName,
                                                                Qty = s.Qty
                                                            })
                                                 join j in _journalRepository.GetAll()
                                                             .Where(s => s.Status == TransactionStatus.Publish)
                                                             .Where(s => s.ItemIssueVendorCreditId.HasValue)
                                                             .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                                             .WhereIf(toDate != null, u => u.Date.Date <= toDate.Value.Date)
                                                             .AsNoTracking()
                                                             .Select(s => new
                                                             {
                                                                 ItemIssueVendorCreditId = s.ItemIssueVendorCreditId
                                                             })
                                                 on isv.ItemIssueVendorCreditId equals j.ItemIssueVendorCreditId
                                                 join i in itemQuery
                                                 on isv.ItemId equals i
                                                 select new ItemBalanceDto
                                                 {
                                                     IsPurchase = false,
                                                     ItemId = isv.ItemId,
                                                     Qty = (isv.Qty * -1),
                                                     LotId = isv.LotId ?? 0,
                                                     LotName = isv.LotName
                                                 };
            var itemIssueResult = itemIssueItemQuery.Concat(itemIssueVendorCreditItemQuery);

            var inventoryTransactionItemQuery = itemReceiptResult.Concat(itemIssueResult);

            return inventoryTransactionItemQuery;
        }


        public IQueryable<ItemDto> GetInventoryTransactionDetailQuery(
                                    DateTime fromDate, DateTime toDate,
                                    List<long> locations,
                                    List<Guid> items,
                                    List<JournalType> journalTypes,
                                    List<Guid> inventoryAccount,
                                    List<GetListPropertyFilter> itemPropertiesInput = null,
                                    List<long?> users = null,
                                    long? filterByUserId = null)
        {

            var itemReceiptItemQuery = (from Ir in _itemReceiptItemRepository.GetAll()
                                   .Include(u => u.ItemReceipt)
                                   .Include(u => u.Item)
                                   .Include(u => u.Lot.Location)
                                  .Include(u => u.Item.Properties)
                                  .ThenInclude(u => u.Property)
                                   //.Include(u => u.Item.Properties)
                                   //.ThenInclude(u => u.Property)

                                   //  .Include(u => u.Item.Properties)
                                   //  .ThenInclude(u => u.PropertyValue)
                                   .AsNoTracking()
                                   .WhereIf(items != null && items.Count > 0, t => items.Contains(t.ItemId))
                                   //.WhereIf(ItemProperties != null && ItemProperties.Count > 0, t => t.Item.Properties.Any(x => ItemProperties.Contains(x.PropertyValueId)))
                                   //.WhereIf(itemProperties != null && itemProperties.Count > 0,
                                   //                 p => p.Item.Properties != null &&
                                   //                     p.Item.Properties.Count > 0 &&
                                   //                     (
                                   //                     p.Item.Properties.Where(i =>
                                   //                     i.PropertyValueId != null &&
                                   //                     itemPropertyFlats.Any(s=>s.PropertyId == i.PropertyId && s.PropertyValueId == i.PropertyValueId.Value))

                                   //         .Count() == itemProperties.Count))
                                   .WhereIf(inventoryAccount != null && inventoryAccount.Count > 0, t => inventoryAccount.Contains(t.Item.InventoryAccountId.Value))

                                        join j in _journalRepository.GetAll().AsNoTracking().Include(t => t.CreatorUser).Include(u => u.Location)
                                        .Where(s => s.Status != TransactionStatus.Void)
                                       .WhereIf(locations != null && locations.Count > 0, t => locations.Contains(t.LocationId.Value))
                                       .WhereIf(users != null && users.Count > 0, t => users.Contains(t.CreatorUserId))
                                       .WhereIf(journalTypes != null && journalTypes.Count > 0, t => journalTypes.Contains(t.JournalType))

                                       .WhereIf(fromDate != null && toDate != null,
                                                (u => (u.Date.Date) >= (fromDate.Date) && (u.Date.Date) <= (toDate.Date)))

                                        on Ir.ItemReceipt.Id equals j.ItemReceiptId
                                        select new ItemDto
                                        {
                                            UserName = j.CreatorUser.UserName,
                                            UserId = j.CreatorUser.Id,
                                            TransferOrderItemId = Ir.TransferOrderItemId,
                                            LotId = Ir.LotId == null ? 0 : Ir.LotId.Value,
                                            LotName = Ir.Lot.LotName,
                                            IsPurchase = true,
                                            ItemId = Ir.Item.Id,
                                            ItemName = Ir.Item.ItemName,
                                            ItemCode = Ir.Item.ItemCode,
                                            TransactionId = Ir.ItemReceiptId,
                                            TotalCost = Ir.Total,
                                            Qty = Ir.Qty,
                                            InventoryAccountId = Ir.Item.InventoryAccountId.Value,
                                            InventoryAccountName = Ir.Item.InventoryAccount.AccountName,
                                            InventoryAccountCode = Ir.Item.InventoryAccount.AccountCode,
                                            LocationId = Ir.Lot.LocationId,
                                            SalePrice = Ir.Item.SalePrice,
                                            LocationName = Ir.Lot.Location.LocationName,
                                            JournalDate = j.Date,
                                            JournalType = j.JournalType,
                                            UnitCost = Ir.UnitCost,
                                            TransactionItemId = Ir.Id,
                                            JournalNo = j.JournalNo,
                                            JournalId = j.Id,
                                            CreationTimeIndex = j.CreationTimeIndex,
                                            CreationTime = Ir.CreationTime,
                                            JournalMemo = j.Memo,
                                            JournalReference = j.Reference,
                                            ProductionProcessName = j.ItemReceipt.ProductionProcess == null ? "" : j.ItemReceipt.ProductionProcess.ProcessName,
                                            PartnerName = j.ItemReceipt.Vendor != null ? j.ItemReceipt.Vendor.VendorName : null,
                                            OrderId = Ir.ItemReceipt.TransferOrderId != null ? Ir.ItemReceipt.TransferOrderId : Ir.ItemReceipt.ProductionOrderId,
                                            Unit = null
                                        });



            var itemReceiptCustomerCreditItemQuery = (from IrC in _itemReceiptCustomerCreditItemRepository.GetAll()
                                                    .Include(u => u.Lot.Location)
                                                    .Include(u => u.ItemReceiptCustomerCredit)
                                                    .Include(u => u.Item)
                                                    //.Include(u => u.Item.Properties)
                                                    //.ThenInclude(u => u.Property)

                                                    //.Include(u => u.Item.Properties)
                                                    //.ThenInclude(u => u.PropertyValue)

                                                    .WhereIf(items != null && items.Count > 0, t => items.Contains(t.ItemId))
                                                    //.WhereIf(itemProperties != null && itemProperties.Count > 0,
                                                    //   p => p.Item.Properties != null &&
                                                    //       p.Item.Properties.Count > 0 &&
                                                    //       (
                                                    //       p.Item.Properties.Where(i =>
                                                    //       i.PropertyValueId != null &&
                                                    //       itemPropertyFlats.Any(s => s.PropertyId == i.PropertyId && s.PropertyValueId == i.PropertyValueId.Value))

                                                    //.Count() == itemProperties.Count))
                                                    .WhereIf(inventoryAccount != null && inventoryAccount.Count > 0, t => inventoryAccount.Contains(t.Item.InventoryAccountId.Value))
                                                      join j in _journalRepository.GetAll().AsNoTracking().Include(t => t.CreatorUser)
                                                      .Where(s => s.Status != TransactionStatus.Void)
                                                     .WhereIf(users != null && users.Count > 0, t => users.Contains(t.CreatorUserId))
                                                     .WhereIf(locations != null && locations.Count > 0, t => locations.Contains(t.LocationId.Value))
                                                     .WhereIf(journalTypes != null && journalTypes.Count > 0, t => journalTypes.Contains(t.JournalType))

                                                     .WhereIf(fromDate != null && toDate != null,
                                                              (u => (u.Date.Date) >= (fromDate.Date) && (u.Date.Date) <= (toDate.Date)))

                                                      on IrC.ItemReceiptCustomerCredit.Id equals j.ItemReceiptCustomerCreditId
                                                      select new ItemDto
                                                      {
                                                          UserName = j.CreatorUser.UserName,
                                                          UserId = j.CreatorUser.Id,
                                                          TransferOrderItemId = null,
                                                          LotId = IrC.LotId == null ? 0 : IrC.LotId.Value,
                                                          LotName = IrC.Lot.LotName,
                                                          IsPurchase = true,
                                                          ItemId = IrC.Item.Id,
                                                          ItemName = IrC.Item.ItemName,
                                                          ItemCode = IrC.Item.ItemCode,
                                                          TransactionId = IrC.ItemReceiptCustomerCreditId,
                                                          TotalCost = IrC.Total,
                                                          Qty = IrC.Qty,
                                                          InventoryAccountId = IrC.Item.InventoryAccountId.Value,
                                                          InventoryAccountName = IrC.Item.InventoryAccount.AccountName,
                                                          InventoryAccountCode = IrC.Item.InventoryAccount.AccountCode,
                                                          LocationId = IrC.Lot.LocationId,
                                                          SalePrice = IrC.Item.SalePrice,
                                                          LocationName = IrC.Lot.Location.LocationName,
                                                          JournalDate = j.Date,
                                                          JournalType = j.JournalType,
                                                          UnitCost = IrC.UnitCost,
                                                          TransactionItemId = IrC.Id,
                                                          JournalNo = j.JournalNo,
                                                          JournalId = j.Id,
                                                          CreationTimeIndex = j.CreationTimeIndex,
                                                          CreationTime = IrC.CreationTime,
                                                          JournalMemo = j.Memo,
                                                          JournalReference = j.Reference,
                                                          ProductionProcessName = "",
                                                          PartnerName = j.ItemReceiptCustomerCredit.Customer != null ? j.ItemReceiptCustomerCredit.Customer.CustomerName : null,
                                                          OrderId = null,
                                                          Unit = IrC.Item.Properties.Any(p => p.Property.IsUnit) ?
                                                                                    ObjectMapper.Map<UnitDto>(IrC.Item.Properties.Select(t => t.PropertyValue).FirstOrDefault(p => p.Property.IsUnit))
                                                                                    : null
                                                      });

            //var itemReceiptResult = itemReceiptItemQuery.Union(itemReceiptCustomerCreditItemQuery);

            var itemIssueItemQuery = (from isue in _itemIssueItemRepository.GetAll()
                                      .Include(u => u.Lot.Location)
                                      .Include(u => u.ItemIssue)
                                      .Include(u => u.Item)
                                      .AsNoTracking()
                                      //.Include(u => u.Item.Properties)
                                      //.ThenInclude(u => u.Property)

                                      //.Include(u => u.Item.Properties)
                                      //.ThenInclude(u => u.PropertyValue)
                                      .WhereIf(items != null && items.Count > 0, t => items.Contains(t.ItemId))
                                      //.WhereIf(itemProperties != null && itemProperties.Count > 0,
                                      //                   p => p.Item.Properties != null &&
                                      //                       p.Item.Properties.Count > 0 &&
                                      //                       (
                                      //                       p.Item.Properties.Where(i =>
                                      //                       i.PropertyValueId != null &&
                                      //                       itemPropertyFlats.Any(s => s.PropertyId == i.PropertyId && s.PropertyValueId == i.PropertyValueId.Value))

                                      //           .Count() == itemProperties.Count))
                                      .WhereIf(inventoryAccount != null && inventoryAccount.Count > 0, t => inventoryAccount.Contains(t.Item.InventoryAccountId.Value))

                                      join j in _journalRepository.GetAll().AsNoTracking().Include(t => t.CreatorUser)
                                      .Where(s => s.Status != TransactionStatus.Void)
                                      .WhereIf(users != null && users.Count > 0, t => users.Contains(t.CreatorUserId))
                                      .WhereIf(locations != null && locations.Count > 0, t => locations.Contains(t.LocationId.Value))
                                      .WhereIf(journalTypes != null && journalTypes.Count > 0, t => journalTypes.Contains(t.JournalType))

                                      .WhereIf(fromDate != null && toDate != null,
                                              (u => (u.Date.Date) >= (fromDate.Date) && (u.Date.Date) <= (toDate.Date)))

                                      on isue.ItemIssue.Id equals j.ItemIssueId
                                      select new ItemDto
                                      {
                                          UserName = j.CreatorUser.UserName,
                                          UserId = j.CreatorUser.Id,
                                          LotId = isue.LotId == null ? 0 : isue.LotId.Value,
                                          LotName = isue.Lot.LotName,
                                          IsPurchase = false,
                                          ItemId = isue.Item.Id,
                                          ItemName = isue.Item.ItemName,
                                          ItemCode = isue.Item.ItemCode,
                                          TransactionId = isue.ItemIssueId,
                                          TotalCost = isue.Total,
                                          Qty = isue.Qty * -1,
                                          InventoryAccountId = isue.Item.InventoryAccountId.Value,
                                          InventoryAccountName = isue.Item.InventoryAccount.AccountName,
                                          InventoryAccountCode = isue.Item.InventoryAccount.AccountCode,
                                          LocationId = isue.Lot.LocationId,
                                          SalePrice = isue.Item.SalePrice,
                                          LocationName = isue.Lot.Location.LocationName,
                                          JournalDate = j.Date,
                                          JournalType = j.JournalType,
                                          TransferOrderItemId = isue.TransferOrderItemId,
                                          UnitCost = isue.UnitCost,
                                          TransactionItemId = isue.Id,
                                          JournalNo = j.JournalNo,
                                          JournalId = j.Id,
                                          CreationTimeIndex = j.CreationTimeIndex,
                                          CreationTime = isue.CreationTime,
                                          JournalMemo = j.Memo,
                                          JournalReference = j.Reference,
                                          ProductionProcessName = isue.ItemIssue.ProductionProcess != null ? isue.ItemIssue.ProductionProcess.ProcessName : null,
                                          PartnerName = j.ItemIssue.Customer != null ? j.ItemIssue.Customer.CustomerName : null,
                                          OrderId = null,
                                          Unit = isue.Item.Properties.Any(p => p.Property.IsUnit) ?
                                                                                    ObjectMapper.Map<UnitDto>(isue.Item.Properties.Select(t => t.PropertyValue).FirstOrDefault(p => p.Property.IsUnit))
                                                                                    : null
                                      });

            var itemIssueVendorCreditItemQuery = (from isv in _itemIssueVendorCreditItemRepository.GetAll()
                                                .Include(u => u.Lot.Location)
                                                .Include(u => u.ItemIssueVendorCredit)
                                                .Include(u => u.Item)
                                                .AsNoTracking()
                                                //.Include(u => u.Item.Properties)
                                                //.ThenInclude(u => u.Property)
                                                //.Include(u => u.Item.Properties)
                                                //.ThenInclude(u => u.PropertyValue)
                                                .WhereIf(items != null && items.Count > 0, t => items.Contains(t.ItemId))
                                                //.WhereIf(itemProperties != null && itemProperties.Count > 0,
                                                //        p => p.Item.Properties != null &&
                                                //            p.Item.Properties.Count > 0 &&
                                                //            (
                                                //            p.Item.Properties.Where(i =>
                                                //            i.PropertyValueId != null &&
                                                //            itemPropertyFlats.Any(s => s.PropertyId == i.PropertyId && s.PropertyValueId == i.PropertyValueId.Value))

                                                //.Count() == itemProperties.Count))
                                                .WhereIf(inventoryAccount != null && inventoryAccount.Count > 0, t => inventoryAccount.Contains(t.Item.InventoryAccountId.Value))

                                                  join j in _journalRepository.GetAll().AsNoTracking().Include(t => t.CreatorUser)
                                                  .Where(s => s.Status != TransactionStatus.Void)
                                                 .WhereIf(users != null && users.Count > 0, t => users.Contains(t.CreatorUserId))
                                                 .WhereIf(locations != null && locations.Count > 0, t => locations.Contains(t.LocationId.Value))
                                                 .WhereIf(journalTypes != null && journalTypes.Count > 0, t => journalTypes.Contains(t.JournalType))

                                                 .WhereIf(fromDate != null && toDate != null,
                                                          (u => (u.Date.Date) >= (fromDate.Date) && (u.Date.Date) <= (toDate.Date)))

                                                  on isv.ItemIssueVendorCredit.Id equals j.ItemIssueVendorCreditId
                                                  select new ItemDto
                                                  {
                                                      UserName = j.CreatorUser.UserName,
                                                      UserId = j.CreatorUser.Id,
                                                      TransferOrderItemId = null,
                                                      LotId = isv.LotId == null ? 0 : isv.LotId.Value,
                                                      LotName = isv.Lot.LotName,
                                                      IsPurchase = false,
                                                      ItemId = isv.Item.Id,
                                                      ItemName = isv.Item.ItemName,
                                                      ItemCode = isv.Item.ItemCode,
                                                      TransactionId = isv.ItemIssueVendorCreditId,
                                                      TotalCost = isv.Total,
                                                      Qty = isv.Qty * -1,
                                                      InventoryAccountId = isv.Item.InventoryAccountId.Value,
                                                      InventoryAccountName = isv.Item.InventoryAccount.AccountName,
                                                      InventoryAccountCode = isv.Item.InventoryAccount.AccountCode,
                                                      LocationId = isv.Lot.LocationId,
                                                      SalePrice = isv.Item.SalePrice,
                                                      LocationName = isv.Lot.Location.LocationName,
                                                      JournalDate = j.Date,
                                                      JournalType = j.JournalType,
                                                      UnitCost = isv.UnitCost,
                                                      TransactionItemId = isv.Id,
                                                      JournalNo = j.JournalNo,
                                                      JournalId = j.Id,
                                                      CreationTimeIndex = j.CreationTimeIndex,
                                                      CreationTime = isv.CreationTime,
                                                      JournalMemo = j.Memo,
                                                      JournalReference = j.Reference,
                                                      ProductionProcessName = "",
                                                      PartnerName = j.ItemIssueVendorCredit.Vendor != null ? j.ItemIssueVendorCredit.Vendor.VendorName : null,
                                                      OrderId = null,
                                                      Unit = isv.Item.Properties.Any(p => p.Property.IsUnit) ?
                                                                                    ObjectMapper.Map<UnitDto>(isv.Item.Properties.Select(t => t.PropertyValue).FirstOrDefault(p => p.Property.IsUnit))
                                                                                    : null
                                                  });
            //var itemIssueResult = itemIssueItemQuery.Union(itemIssueVendorCreditItemQuery);

            //var inventoryTransactionItemQuery = itemReceiptResult.Union(itemIssueResult);

            //var inventoryTransactionItemQuery = itemReceiptItemQuery.Union(itemReceiptCustomerCreditItemQuery).Union(itemIssueItemQuery).Union(itemIssueVendorCreditItemQuery);


            //var inventoryTransactionItemQuery = itemReceiptItemQuery;

            var inventoryTransactionItemQuery = itemReceiptCustomerCreditItemQuery.Union(itemIssueItemQuery).Union(itemIssueVendorCreditItemQuery).Union(itemReceiptItemQuery);

            if (filterByUserId != null)
            {
                // get user by group member
                var locationGroups = GetLocationGroup(filterByUserId.Value);
                var itemGroup = this.GetItemByUserGroup(filterByUserId.Value);
                inventoryTransactionItemQuery = from u in inventoryTransactionItemQuery
                                                join g in locationGroups on u.LocationId equals g
                                                join i in itemGroup on u.ItemId equals i
                                                select u;
            }

            return inventoryTransactionItemQuery;

        }



        private IQueryable<ItemTransactionDto> GetInventoryTransactionBeginningReportQuery(
                                    DateTime fromDate,
                                    //DateTime toDate,
                                    List<long> locations,
                                    List<long> lots,
                                    List<Guid> items,
                                    List<JournalType> journalTypes,
                                    List<long?> users = null,
                                    long? filterByUserId = null,
                                    List<Guid> journalTransactionTypeIds = null,
                                    List<long> inventoryTypes = null)
        {


            var periodCycles = GetCloseCyleQuery();
            var previousCycle = periodCycles.Where(u => u.EndDate != null &&
                                                        u.EndDate.Value.Date <= fromDate.Date)
                                                        .OrderByDescending(u => u.EndDate.Value).FirstOrDefault();

            var inventoryCostClose = from u in _inventoryCostCloseItemRepository.GetAll()
                                                //.Where(t => previousCycle != null && t.InventoryCostClose.CloseDate.Date == previousCycle.EndDate.Value.Date)
                                                .Where(t => previousCycle != null && t.InventoryCostClose.AccountCycleId == previousCycle.Id)
                                                .WhereIf(locations != null && locations.Count() > 0, t => locations.Contains(t.Lot.LocationId))
                                                .WhereIf(lots != null && lots.Count() > 0, t => t.Lot != null && lots.Contains(t.LotId.Value))
                                                .WhereIf(items != null && items.Count() > 0, t => items.Contains(t.InventoryCostClose.ItemId))
                                                .AsNoTracking()
                                     select new ItemTransactionDto
                                     {
                                         UserName = "",
                                         UserId = 0,
                                         LotId = u.LotId == null ? 0 : u.LotId.Value,
                                         LotName = u.Lot == null ? "" : u.Lot.LotName,
                                         IsPurchase = true,
                                         ItemId = u.InventoryCostClose.ItemId,
                                         TransactionId = null,
                                         Qty = u.Qty,
                                         LocationId = u.Lot == null ? 0 : u.Lot.LocationId,
                                         LocationName = u.Lot == null ? "" : u.Lot.Location.LocationName,
                                         JournalDate = u.InventoryCostClose.CloseDate,
                                         JournalType = null,
                                         JournalNo = "",
                                         JournalId = null,
                                         CreationTimeIndex = null,
                                         CreationTime = Clock.Now,
                                         JournalMemo = "",
                                         JournalReference = "",
                                         ProductionProcessName = "",
                                         PartnerName = ""
                                     };

            var beginningToDate = fromDate.AddDays(-1);
            var beginningFromDate = previousCycle == null ? DateTime.MinValue : previousCycle.EndDate.Value.AddDays(1);

            var itemReceiptItemQuery = (from Ir in _itemReceiptItemRepository.GetAll()
                                                   .WhereIf(locations != null && locations.Count > 0, t => t.Lot != null && locations.Contains(t.Lot.LocationId))
                                                   .WhereIf(lots != null && lots.Count > 0, t => t.Lot != null && lots.Contains(t.LotId.Value))
                                                   .WhereIf(items != null && items.Count > 0, t => items.Contains(t.ItemId))
                                                   .AsNoTracking()

                                        join j in _journalRepository.GetAll()
                                                    .Where(s => s.ItemReceiptId.HasValue)
                                                    .Where(s => s.Status == TransactionStatus.Publish)
                                                    .WhereIf(users != null && users.Count > 0, t => users.Contains(t.CreatorUserId))
                                                    .WhereIf(journalTypes != null && journalTypes.Count > 0, t =>
                                                        journalTypes.Contains(t.JournalType))
                                                    .WhereIf(journalTransactionTypeIds != null && journalTransactionTypeIds.Count > 0, t => t.JournalTransactionTypeId != null && journalTransactionTypeIds.Contains(t.JournalTransactionTypeId.Value))
                                                    .WhereIf(inventoryTypes != null && inventoryTypes.Count > 0, s => s.JournalTransactionType != null &&
                                                             ((inventoryTypes.Contains((long)InventoryType.ItemIssueSale) && s.JournalTransactionType.IsIssue) ||
                                                             (inventoryTypes.Contains((long)InventoryType.ItemReceiptPurchase) && !s.JournalTransactionType.IsIssue)))
                                                    .WhereIf(beginningFromDate != null,
                                                            (u => (u.Date.Date) >= (beginningFromDate.Date) && (u.Date.Date) <= (beginningToDate.Date)))
                                                    .AsNoTracking()

                                        on Ir.ItemReceiptId equals j.ItemReceiptId
                                        select new ItemTransactionDto
                                        {
                                            JournalTransactionTypeId = j.JournalTransactionTypeId,
                                            Issue = j.JournalTransactionTypeId != null ? j.JournalTransactionType.IsIssue : (bool?)null,
                                            JournalTransactionTypeName = j.JournalTransactionTypeId != null ? j.JournalTransactionType.Name : "",
                                            UserName = j.CreatorUser.UserName,
                                            UserId = j.CreatorUser.Id,
                                            LotId = Ir.LotId == null ? 0 : Ir.LotId.Value,
                                            LotName = Ir.Lot.LotName,
                                            IsPurchase = true,
                                            ItemId = Ir.ItemId,
                                            TransactionId = Ir.ItemReceiptId,
                                            Qty = Ir.Qty,
                                            LocationId = Ir.Lot == null ? 0 : Ir.Lot.LocationId,
                                            LocationName = Ir.Lot == null ? "" : Ir.Lot.Location.LocationName,
                                            JournalDate = j.Date,
                                            JournalType = j.JournalType,
                                            JournalNo = j.JournalNo,
                                            JournalId = j.Id,
                                            CreationTimeIndex = j.CreationTimeIndex,
                                            CreationTime = Ir.CreationTime,
                                            JournalMemo = j.Memo,
                                            JournalReference = j.Reference,
                                            ProductionProcessName = j.ItemReceipt.ProductionProcess == null ? "" : j.ItemReceipt.ProductionProcess.ProcessName,
                                            PartnerName = j.ItemReceipt.Vendor != null ? j.ItemReceipt.Vendor.VendorName : null,
                                        });



            var itemReceiptCustomerCreditItemQuery = (from IrC in _itemReceiptCustomerCreditItemRepository.GetAll()

                                                                .WhereIf(lots != null && lots.Count > 0, t => t.Lot != null && lots.Contains(t.LotId.Value))
                                                                .WhereIf(items != null && items.Count > 0, t => items.Contains(t.ItemId))
                                                                .WhereIf(locations != null && locations.Count > 0, t => t.Lot != null && locations.Contains(t.Lot.LocationId))
                                                                .AsNoTracking()

                                                      join j in _journalRepository.GetAll()
                                                                  .Where(s => s.ItemReceiptCustomerCreditId.HasValue)
                                                                  .Where(s => s.Status == TransactionStatus.Publish)
                                                                  .WhereIf(users != null && users.Count > 0, t => users.Contains(t.CreatorUserId))

                                                                  .WhereIf(journalTypes != null && journalTypes.Count > 0, t => journalTypes.Contains(t.JournalType))
                                                                  .WhereIf(journalTransactionTypeIds != null && journalTransactionTypeIds.Count > 0, t => t.JournalTransactionTypeId != null && journalTransactionTypeIds.Contains(t.JournalTransactionTypeId.Value))
                                                                  .WhereIf(inventoryTypes != null && inventoryTypes.Count > 0, s => s.JournalTransactionType != null &&
                                                                     ((inventoryTypes.Contains((long)InventoryType.ItemIssueSale) && s.JournalTransactionType.IsIssue) ||
                                                                     (inventoryTypes.Contains((long)InventoryType.ItemReceiptPurchase) && !s.JournalTransactionType.IsIssue)))
                                                                  .WhereIf(beginningFromDate != null && beginningToDate != null,
                                                                          (u => (u.Date.Date) >= (beginningFromDate.Date) && (u.Date.Date) <= (beginningToDate.Date)))
                                                                  .AsNoTracking()

                                                      on IrC.ItemReceiptCustomerCreditId equals j.ItemReceiptCustomerCreditId
                                                      select new ItemTransactionDto
                                                      {
                                                          JournalTransactionTypeId = j.JournalTransactionTypeId,
                                                          Issue = j.JournalTransactionTypeId != null ? j.JournalTransactionType.IsIssue : (bool?)null,
                                                          JournalTransactionTypeName = j.JournalTransactionTypeId != null ? j.JournalTransactionType.Name : "",
                                                          UserName = j.CreatorUser.UserName,
                                                          UserId = j.CreatorUser.Id,
                                                          LotId = IrC.LotId == null ? 0 : IrC.LotId.Value,
                                                          LotName = IrC.Lot.LotName,
                                                          IsPurchase = true,
                                                          ItemId = IrC.ItemId,
                                                          TransactionId = IrC.ItemReceiptCustomerCreditId,
                                                          Qty = IrC.Qty,
                                                          LocationId = IrC.Lot == null ? 0 : IrC.Lot.LocationId,
                                                          LocationName = IrC.Lot == null ? "" : IrC.Lot.Location.LocationName,
                                                          JournalDate = j.Date,
                                                          JournalType = j.JournalType,
                                                          JournalNo = j.JournalNo,
                                                          JournalId = j.Id,
                                                          CreationTimeIndex = j.CreationTimeIndex,
                                                          CreationTime = IrC.CreationTime,
                                                          JournalMemo = j.Memo,
                                                          JournalReference = j.Reference,
                                                          ProductionProcessName = "",
                                                          PartnerName = j.ItemReceiptCustomerCredit.Customer != null ? j.ItemReceiptCustomerCredit.Customer.CustomerName : null,
                                                      });
            var itemIssueItemQuery = (from isue in _itemIssueItemRepository.GetAll()
                                                  .WhereIf(lots != null && lots.Count > 0, t => t.Lot != null && lots.Contains(t.LotId.Value))
                                                  .WhereIf(locations != null && locations.Count > 0, t => t.Lot != null && locations.Contains(t.Lot.LocationId))
                                                  .WhereIf(items != null && items.Count > 0, t => items.Contains(t.ItemId))
                                                  .AsNoTracking()

                                      join j in _journalRepository.GetAll()
                                                  .Where(s => s.ItemIssueId.HasValue)
                                                  .Where(s => s.Status == TransactionStatus.Publish)
                                                  .WhereIf(users != null && users.Count > 0, t => users.Contains(t.CreatorUserId))

                                                  .WhereIf(journalTypes != null && journalTypes.Count > 0, t => journalTypes.Contains(t.JournalType))
                                                  .WhereIf(journalTransactionTypeIds != null && journalTransactionTypeIds.Count > 0, t => t.JournalTransactionTypeId != null && journalTransactionTypeIds.Contains(t.JournalTransactionTypeId.Value))
                                                   .WhereIf(inventoryTypes != null && inventoryTypes.Count > 0, s => s.JournalTransactionType != null &&
                                                     ((inventoryTypes.Contains((long)InventoryType.ItemIssueSale) && s.JournalTransactionType.IsIssue) ||
                                                     (inventoryTypes.Contains((long)InventoryType.ItemReceiptPurchase) && !s.JournalTransactionType.IsIssue)))
                                                                  .WhereIf(beginningFromDate != null && beginningToDate != null,
                                                          (u => (u.Date.Date) >= (beginningFromDate.Date) && (u.Date.Date) <= (beginningToDate.Date)))
                                                  .AsNoTracking()

                                      on isue.ItemIssue.Id equals j.ItemIssueId
                                      select new ItemTransactionDto
                                      {
                                          JournalTransactionTypeId = j.JournalTransactionTypeId,
                                          Issue = j.JournalTransactionTypeId != null ? j.JournalTransactionType.IsIssue : (bool?)null,
                                          JournalTransactionTypeName = j.JournalTransactionTypeId != null ? j.JournalTransactionType.Name : "",
                                          UserName = j.CreatorUser.UserName,
                                          UserId = j.CreatorUser.Id,
                                          LotId = isue.LotId == null ? 0 : isue.LotId.Value,
                                          LotName = isue.Lot.LotName,
                                          IsPurchase = false,
                                          ItemId = isue.ItemId,
                                          TransactionId = isue.ItemIssueId,
                                          ProductionProcessName = isue.ItemIssue.ProductionProcess != null ? isue.ItemIssue.ProductionProcess.ProcessName : null,
                                          Qty = isue.Qty * -1,
                                          LocationId = isue.Lot == null ? 0 : isue.Lot.LocationId,
                                          LocationName = isue.Lot == null ? "" : isue.Lot.Location.LocationName,
                                          JournalDate = j.Date,
                                          JournalType = j.JournalType,
                                          JournalNo = j.JournalNo,
                                          JournalId = j.Id,
                                          CreationTimeIndex = j.CreationTimeIndex,
                                          CreationTime = isue.CreationTime,
                                          JournalMemo = j.Memo,
                                          JournalReference = j.Reference,
                                          PartnerName = j.ItemIssue.Customer != null ? j.ItemIssue.Customer.CustomerName : null,

                                      });

            var itemIssueVendorCreditItemQuery = (from isv in _itemIssueVendorCreditItemRepository.GetAll()
                                                            .WhereIf(lots != null && lots.Count > 0, t => t.Lot != null && lots.Contains(t.LotId.Value))
                                                            .WhereIf(locations != null && locations.Count > 0,
                                                                        t => t.Lot != null && locations.Contains(t.Lot.LocationId))
                                                            .WhereIf(items != null && items.Count > 0, t => items.Contains(t.ItemId))
                                                            .AsNoTracking()

                                                  join j in _journalRepository.GetAll()
                                                             .Where(s => s.ItemIssueVendorCreditId.HasValue)
                                                             .Where(s => s.Status == TransactionStatus.Publish)
                                                             .WhereIf(users != null && users.Count > 0, t => users.Contains(t.CreatorUserId))

                                                             .WhereIf(journalTypes != null && journalTypes.Count > 0,
                                                                        t => journalTypes.Contains(t.JournalType))
                                                             .WhereIf(journalTransactionTypeIds != null && journalTransactionTypeIds.Count > 0, t => t.JournalTransactionTypeId != null && journalTransactionTypeIds.Contains(t.JournalTransactionTypeId.Value))
                                                             .WhereIf(inventoryTypes != null && inventoryTypes.Count > 0, s => s.JournalTransactionType != null &&
                                                                     ((inventoryTypes.Contains((long)InventoryType.ItemIssueSale) && s.JournalTransactionType.IsIssue) ||
                                                                     (inventoryTypes.Contains((long)InventoryType.ItemReceiptPurchase) && !s.JournalTransactionType.IsIssue)))
                                                             .WhereIf(beginningFromDate != null && beginningToDate != null,
                                                                      (u => (u.Date.Date) >= (beginningFromDate.Date) && (u.Date.Date) <= (beginningToDate.Date)))
                                                             .AsNoTracking()

                                                  on isv.ItemIssueVendorCreditId equals j.ItemIssueVendorCreditId
                                                  select new ItemTransactionDto
                                                  {
                                                      JournalTransactionTypeId = j.JournalTransactionTypeId,
                                                      Issue = j.JournalTransactionTypeId != null ? j.JournalTransactionType.IsIssue : (bool?)null,
                                                      JournalTransactionTypeName = j.JournalTransactionTypeId != null ? j.JournalTransactionType.Name : "",
                                                      ProductionProcessName = "",
                                                      UserName = j.CreatorUser.UserName,
                                                      UserId = j.CreatorUser.Id,
                                                      LotId = isv.LotId == null ? 0 : isv.LotId.Value,
                                                      LotName = isv.Lot.LotName,
                                                      IsPurchase = false,
                                                      ItemId = isv.ItemId,
                                                      TransactionId = isv.ItemIssueVendorCreditId,
                                                      Qty = isv.Qty * -1,
                                                      LocationId = isv.Lot == null ? 0 : isv.Lot.LocationId,
                                                      LocationName = isv.Lot == null ? "" : isv.Lot.Location.LocationName,
                                                      JournalDate = j.Date,
                                                      JournalType = j.JournalType,
                                                      JournalNo = j.JournalNo,
                                                      JournalId = j.Id,
                                                      CreationTimeIndex = j.CreationTimeIndex,
                                                      CreationTime = isv.CreationTime,
                                                      JournalMemo = j.Memo,
                                                      JournalReference = j.Reference,
                                                      PartnerName = isv.ItemIssueVendorCredit.Vendor != null ? isv.ItemIssueVendorCredit.Vendor.VendorName : null,
                                                  });


            if (filterByUserId != null)
            {
                // get user by group member
                var locationGroups = GetLocationGroup(filterByUserId.Value);
                //var itemGroupIds = this.GetItemByUserGroup(filterByUserId.Value);

                var _inventoryCostClose = from u in inventoryCostClose
                                          join g in locationGroups on u.LocationId equals g
                                          //join i in itemGroupIds on u.ItemId equals i
                                          select u;
                var _itemReceiptItemQuery = from u in itemReceiptItemQuery
                                            join g in locationGroups on u.LocationId equals g
                                            //join i in itemGroupIds on u.ItemId equals i
                                            select u;
                var _itemReceiptCustomerCreditItemQuery = from u in itemReceiptCustomerCreditItemQuery
                                                          join g in locationGroups on u.LocationId equals g
                                                          //join i in itemGroupIds on u.ItemId equals i
                                                          select u;
                var _itemIssueItemQuery = from u in itemIssueItemQuery
                                          join g in locationGroups on u.LocationId equals g
                                          //join i in itemGroupIds on u.ItemId equals i
                                          select u;
                var _itemIssueVendorCreditItemQuery = from u in itemIssueVendorCreditItemQuery
                                                      join g in locationGroups on u.LocationId equals g
                                                      //join i in itemGroupIds on u.ItemId equals i
                                                      select u;

                inventoryCostClose = _inventoryCostClose;
                itemReceiptItemQuery = _itemReceiptItemQuery;
                itemReceiptCustomerCreditItemQuery = _itemReceiptCustomerCreditItemQuery;
                itemIssueItemQuery = _itemIssueItemQuery;
                itemIssueVendorCreditItemQuery = _itemIssueVendorCreditItemQuery;
            }



            var beginningQuery = from i in _itemRepository.GetAll()
                                           .AsNoTracking()
                                           .Select(s => new { Id = s.Id })

                                 join ic in inventoryCostClose on i.Id equals ic.ItemId
                                 into ics

                                 join iir in itemReceiptItemQuery on i.Id equals iir.ItemId
                                 into iirs

                                 join iirc in itemReceiptCustomerCreditItemQuery on i.Id equals iirc.ItemId
                                 into iircs

                                 join iis in itemIssueItemQuery on i.Id equals iis.ItemId
                                 into iiss

                                 join iiv in itemIssueVendorCreditItemQuery on i.Id equals iiv.ItemId
                                 into iivs

                                 let preDate = beginningToDate
                                 let closeQty = ics == null ? 0 : ics.Sum(r => r.Qty)
                                 let receiptQty = iirs == null ? 0 : iirs.Sum(r => r.Qty)
                                 let returnReceiptQty = iircs == null ? 0 : iircs.Sum(r => r.Qty)
                                 let issueQty = iiss == null ? 0 : iiss.Sum(r => r.Qty)
                                 let returnIssueQty = iivs == null ? 0 : iivs.Sum(r => r.Qty)
                                 let totalQty = receiptQty + returnReceiptQty + issueQty + returnIssueQty + closeQty
                                 let isPurchase = totalQty > 0

                                 where (ics != null && ics.Any()) ||
                                       (iirs != null && iirs.Any()) ||
                                       (iircs != null && iirs.Any()) ||
                                       (iiss != null && iiss.Any()) ||
                                       (iivs != null && iivs.Any())

                                 select new ItemTransactionDto
                                 {
                                     IsPurchase = isPurchase,
                                     ItemId = i.Id,
                                     Qty = totalQty,
                                     CreationTimeIndex = 0,
                                     JournalDate = preDate,
                                     UserId = 0,
                                     UserName = "",
                                     LocationId = 0,
                                     CreationTime = new DateTime(),
                                     JournalId = new Guid(),
                                     JournalMemo = "",
                                     JournalNo = "",
                                     JournalReference = "",
                                     JournalType = null,
                                     LocationName = "",
                                     LotId = 0,
                                     LotName = "",
                                     PartnerName = "",
                                     ProductionProcessName = "",
                                     TransactionId = new Guid(),

                                 };
            if (filterByUserId != null)
            {
                var itemGroupIds = this.GetItemByUserGroup(filterByUserId.Value);

                var _beginningQuery = from u in beginningQuery
                                      join i in itemGroupIds on u.ItemId equals i
                                      select u;
                beginningQuery = _beginningQuery;
            }

            return beginningQuery;
        }


        public IQueryable<ItemTransactionDto> GetInventoryTransactionReportQuery(
                                    DateTime fromDate,
                                    DateTime toDate,
                                    List<long> locations,
                                    List<long> lots,
                                    List<Guid> items,
                                    List<JournalType> journalTypes,
                                    List<long?> users = null,
                                    long? filterByUserId = null,
                                    bool includeBeginning = false,
                                    List<Guid> journalTransactionTypeIds = null,
                                    List<long> inventoryTypes = null)
        {

            //Currency Period
            var inventoryTransactionItemQuery = GetInventoryTransactionQuery(fromDate, toDate, locations, lots, items, journalTypes, users, false, journalTransactionTypeIds, inventoryTypes);

            if (filterByUserId != null)
            {
                // get user by group member
                var locationGroups = GetLocationGroup(filterByUserId.Value);
                var itemGroupIds = this.GetItemByUserGroup(filterByUserId.Value);
                inventoryTransactionItemQuery = from u in inventoryTransactionItemQuery
                                                join g in locationGroups on u.LocationId equals g
                                                join i in itemGroupIds on u.ItemId equals i
                                                select u;

            }
            if (includeBeginning)
            {
                var beginningQuery = GetInventoryTransactionBeginningReportQuery(fromDate, locations, lots, items, journalTypes, users, filterByUserId, journalTransactionTypeIds, inventoryTypes);

                var final = beginningQuery.Concat(inventoryTransactionItemQuery);
                return final;
            }

            return inventoryTransactionItemQuery;

        }


        private IQueryable<ItemTransactionDto> GetInventoryTransactionQuery(
                                    DateTime fromDate,
                                    DateTime toDate,
                                    List<long> locations,
                                    List<long> lots,
                                    List<Guid> items,
                                    List<JournalType> journalTypes,
                                    List<long?> users = null,
                                    bool includeBeginning = false,
                                    List<Guid> journalTransactionTypeIds = null,
                                    List<long> inventoryTypes = null)
        {

            var irJournalQuery = _journalRepository.GetAll()
                                .Where(s => s.Status == TransactionStatus.Publish)
                                .Where(s => s.ItemReceiptId.HasValue)
                                .WhereIf(users != null && users.Count > 0, t => users.Contains(t.CreatorUserId))
                                .WhereIf(journalTypes != null && journalTypes.Count > 0, t => journalTypes.Contains(t.JournalType))
                                .WhereIf(fromDate != null && toDate != null, u => u.Date.Date >= fromDate.Date && u.Date.Date <= toDate.Date)
                                .WhereIf(journalTransactionTypeIds != null && journalTransactionTypeIds.Count > 0, t => t.JournalTransactionTypeId != null && journalTransactionTypeIds.Contains(t.JournalTransactionTypeId.Value))
                                .WhereIf(inventoryTypes != null && inventoryTypes.Count > 0, s => s.JournalTransactionType != null &&
                                     ((inventoryTypes.Contains((long)InventoryType.ItemIssueSale) && s.JournalTransactionType.IsIssue) ||
                                     (inventoryTypes.Contains((long)InventoryType.ItemReceiptPurchase) && !s.JournalTransactionType.IsIssue)))
                                .AsNoTracking()
                                .Select(j => new
                                {
                                    Id = j.Id,
                                    UserId = j.CreatorUser.Id,
                                    Date = j.Date,
                                    JournalType = j.JournalType,
                                    JournalNo = j.JournalNo,
                                    JournalId = j.Id,
                                    CreationTimeIndex = j.CreationTimeIndex,
                                    Memo = j.Memo,
                                    Reference = j.Reference,
                                    ItemReceiptId = j.ItemReceiptId,
                                    UserName = j.CreatorUser.UserName,
                                    VendorId = j.ItemReceipt.VendorId,
                                    VendorName = j.ItemReceipt.Vendor == null ? "" : j.ItemReceipt.Vendor.VendorName,
                                    ProcessName = j.ItemReceipt.ProductionProcess == null ? "" : j.ItemReceipt.ProductionProcess.ProcessName,
                                    journalTransactionTypeName = j.JournalTransactionTypeId != null ? j.JournalTransactionType.Name : "",
                                    Issue = j.JournalTransactionTypeId != null ? j.JournalTransactionType.IsIssue : (bool?)null,
                                    JournalTransactionTypeId = j.JournalTransactionTypeId,
                                });

            var iriQuery = from s in _itemReceiptItemRepository.GetAll()
                                        .WhereIf(locations != null && locations.Count > 0, t => t.Lot != null && locations.Contains(t.Lot.LocationId))
                                        .WhereIf(lots != null && lots.Count > 0, t => t.LotId != null && lots.Contains(t.LotId.Value))
                                        .WhereIf(items != null && items.Count > 0, t => items.Contains(t.ItemId))
                                        .AsNoTracking()

                           join b in _itemReceiptItemBatchNoRepository.GetAll().AsNoTracking()
                                     .Select(s => new BatchNoItemOutput
                                     {
                                         Id = s.Id,
                                         BatchNoId = s.BatchNoId,
                                         BatchNumber = s.BatchNo.Code,
                                         ExpirationDate = s.BatchNo.ExpirationDate,
                                         TransactionItemId = s.ItemReceiptItemId,
                                         Qty = s.Qty
                                     })
                           on s.Id equals b.TransactionItemId
                           into batchs

                           select new
                           {
                               LotId = s.LotId,
                               ItemId = s.ItemId,
                               ItemReceiptId = s.ItemReceiptId,
                               Qty = s.Qty,
                               CreationTime = s.CreationTime,
                               Description = s.Description,
                               LotName = s.Lot == null ? "" : s.Lot.LotName,
                               LocationId = s.Lot == null ? (long?)null : s.Lot.LocationId,
                               LocationName = s.Lot == null ? "" : s.Lot.Location.LocationName,
                               Id = s.Id,
                               BatchNos = batchs.ToList()
                           };


            var itemReceiptItemQuery = from Ir in iriQuery
                                       join j in irJournalQuery
                                       on Ir.ItemReceiptId equals j.ItemReceiptId

                                       select new ItemTransactionDto
                                       {
                                           UserName = j.UserName,
                                           UserId = j.UserId,
                                           LotId = Ir.LotId.Value,
                                           LotName = Ir.LotName,
                                           IsPurchase = true,
                                           ItemId = Ir.ItemId,
                                           TransactionId = Ir.ItemReceiptId,
                                           TransactionItemId = Ir.Id,
                                           Qty = Ir.Qty,
                                           LocationId = Ir.LocationId.Value,
                                           LocationName = Ir.LocationName,
                                           JournalDate = j.Date,
                                           JournalType = j.JournalType,
                                           JournalNo = j.JournalNo,
                                           JournalId = j.Id,
                                           CreationTimeIndex = j.CreationTimeIndex,
                                           CreationTime = Ir.CreationTime,
                                           JournalMemo = j.Memo,
                                           JournalReference = j.Reference,
                                           ProductionProcessName = j.ProcessName,
                                           PartnerName = j.VendorName,
                                           Description = Ir.Description,
                                           ItemBatchNos = Ir.BatchNos,
                                           JournalTransactionTypeId = j.JournalTransactionTypeId,
                                           JournalTransactionTypeName = j.journalTransactionTypeName,
                                           Issue = j.Issue
                                       };

            var ccJournalQuery = _journalRepository.GetAll()
                                .Where(s => s.Status == TransactionStatus.Publish)
                                .Where(s => s.ItemReceiptCustomerCreditId.HasValue)
                                .WhereIf(users != null && users.Count > 0, t => users.Contains(t.CreatorUserId))
                                .WhereIf(journalTypes != null && journalTypes.Count > 0, t => journalTypes.Contains(t.JournalType))
                                .WhereIf(journalTransactionTypeIds != null && journalTransactionTypeIds.Count > 0, t => t.JournalTransactionTypeId != null && journalTransactionTypeIds.Contains(t.JournalTransactionTypeId.Value))
                                .WhereIf(fromDate != null && toDate != null, u => u.Date.Date >= fromDate.Date && u.Date.Date <= toDate.Date)
                                .WhereIf(inventoryTypes != null && inventoryTypes.Count > 0, s => s.JournalTransactionType != null &&
                                     ((inventoryTypes.Contains((long)InventoryType.ItemIssueSale) && s.JournalTransactionType.IsIssue) ||
                                     (inventoryTypes.Contains((long)InventoryType.ItemReceiptPurchase) && !s.JournalTransactionType.IsIssue)))
                                .AsNoTracking()
                                .Select(j => new
                                {
                                    Id = j.Id,
                                    UserId = j.CreatorUser.Id,
                                    Date = j.Date,
                                    JournalType = j.JournalType,
                                    JournalNo = j.JournalNo,
                                    JournalId = j.Id,
                                    CreationTimeIndex = j.CreationTimeIndex,
                                    Memo = j.Memo,
                                    Reference = j.Reference,
                                    ItemReceiptCustomerCreditId = j.ItemReceiptCustomerCreditId,
                                    UserName = j.CreatorUser.UserName,
                                    CustomerId = j.ItemReceiptCustomerCredit.CustomerId,
                                    CustomerName = j.ItemReceiptCustomerCredit.Customer.CustomerName,
                                    JournalTransactionTypeId = j.JournalTransactionTypeId,
                                    JournalTransactionTypeName = j.JournalTransactionTypeId != null ? j.JournalTransactionType.Name : "",
                                    Issue = j.JournalTransactionTypeId != null ? j.JournalTransactionType.IsIssue : (bool?)null,
                                });


            var cciQuery = from IrC in _itemReceiptCustomerCreditItemRepository.GetAll()
                                        .WhereIf(lots != null && lots.Count > 0, t => t.LotId != null && lots.Contains(t.LotId.Value))
                                        .WhereIf(items != null && items.Count > 0, t => items.Contains(t.ItemId))
                                        .WhereIf(locations != null && locations.Count > 0, t => t.Lot != null && locations.Contains(t.Lot.LocationId))
                                        .AsNoTracking()

                           join b in _itemReceiptCustomerCreditItemBatchNoRepository.GetAll().AsNoTracking()
                                       .Select(s => new BatchNoItemOutput
                                       {
                                           Id = s.Id,
                                           BatchNoId = s.BatchNoId,
                                           BatchNumber = s.BatchNo.Code,
                                           ExpirationDate = s.BatchNo.ExpirationDate,
                                           TransactionItemId = s.ItemReceiptCustomerCreditItemId,
                                           Qty = s.Qty
                                       })
                           on IrC.Id equals b.TransactionItemId
                           into batchs

                           select new
                           {
                               LotId = IrC.LotId,
                               ItemId = IrC.ItemId,
                               ItemReceiptCustomerCreditId = IrC.ItemReceiptCustomerCreditId,
                               Qty = IrC.Qty,
                               CreationTime = IrC.CreationTime,
                               Description = IrC.Description,
                               LotName = IrC.Lot == null ? "" : IrC.Lot.LotName,
                               LocationId = IrC.Lot == null ? (long?)null : IrC.Lot.LocationId,
                               LocationName = IrC.Lot == null ? "" : IrC.Lot.Location.LocationName,
                               Id = IrC.Id,
                               BatchNos = batchs.ToList()
                           };



            var itemReceiptCustomerCreditItemQuery = from IrC in cciQuery
                                                     join j in ccJournalQuery
                                                     on IrC.ItemReceiptCustomerCreditId equals j.ItemReceiptCustomerCreditId
                                                     select new ItemTransactionDto
                                                     {
                                                         UserName = j.UserName,
                                                         UserId = j.UserId,
                                                         LotId = IrC.LotId.Value,
                                                         LotName = IrC.LotName,
                                                         IsPurchase = true,
                                                         ItemId = IrC.ItemId,
                                                         TransactionId = IrC.ItemReceiptCustomerCreditId,
                                                         TransactionItemId = IrC.Id,
                                                         Qty = IrC.Qty,
                                                         LocationId = IrC.LocationId.Value,
                                                         LocationName = IrC.LocationName,
                                                         JournalDate = j.Date,
                                                         JournalType = j.JournalType,
                                                         JournalNo = j.JournalNo,
                                                         JournalId = j.Id,
                                                         CreationTimeIndex = j.CreationTimeIndex,
                                                         CreationTime = IrC.CreationTime,
                                                         JournalMemo = j.Memo,
                                                         JournalReference = j.Reference,
                                                         ProductionProcessName = "",
                                                         PartnerName = j.CustomerName,
                                                         Description = IrC.Description,
                                                         ItemBatchNos = IrC.BatchNos,
                                                         JournalTransactionTypeName = j.JournalTransactionTypeName,
                                                         JournalTransactionTypeId = j.JournalTransactionTypeId,
                                                         Issue = j.Issue
                                                     };



            var iiJournalQuery = _journalRepository.GetAll()
                                .Where(s => s.Status == TransactionStatus.Publish)
                                .Where(s => s.ItemIssueId.HasValue)
                                .WhereIf(users != null && users.Count > 0, t => users.Contains(t.CreatorUserId))
                                .WhereIf(journalTypes != null && journalTypes.Count > 0, t => journalTypes.Contains(t.JournalType))
                                .WhereIf(journalTransactionTypeIds != null && journalTransactionTypeIds.Count > 0, t => t.JournalTransactionTypeId != null && journalTransactionTypeIds.Contains(t.JournalTransactionTypeId.Value))
                                .WhereIf(inventoryTypes != null && inventoryTypes.Count > 0, s => s.JournalTransactionType != null &&
                                                             ((inventoryTypes.Contains((long)InventoryType.ItemIssueSale) && s.JournalTransactionType.IsIssue) ||
                                                             (inventoryTypes.Contains((long)InventoryType.ItemReceiptPurchase) && !s.JournalTransactionType.IsIssue)))
                                .WhereIf(fromDate != null && toDate != null, u => u.Date.Date >= fromDate.Date && u.Date.Date <= toDate.Date)
                                .AsNoTracking()
                                .Select(j => new
                                {
                                    Id = j.Id,
                                    UserId = j.CreatorUser.Id,
                                    Date = j.Date,
                                    JournalType = j.JournalType,
                                    JournalNo = j.JournalNo,
                                    JournalId = j.Id,
                                    CreationTimeIndex = j.CreationTimeIndex,
                                    Memo = j.Memo,
                                    Reference = j.Reference,
                                    ItemIssueId = j.ItemIssueId,
                                    UserName = j.CreatorUser.UserName,
                                    CustomerId = j.ItemIssue.CustomerId,
                                    CustomerName = j.ItemIssue.Customer == null ? "" : j.ItemIssue.Customer.CustomerName,
                                    ProcessName = j.ItemIssue.ProductionProcess == null ? "" : j.ItemIssue.ProductionProcess.ProcessName,
                                    JournalTransactionTypeName = j.JournalTransactionTypeId != null ? j.JournalTransactionType.Name : "",
                                    Issue = j.JournalTransactionTypeId != null ? j.JournalTransactionType.IsIssue : (bool?)null,
                                    JournalTransactionTypeId = j.JournalTransactionTypeId,
                                });


            var iiiQuery = from isue in _itemIssueItemRepository.GetAll()
                                        .WhereIf(lots != null && lots.Count > 0, t => t.LotId != null && lots.Contains(t.LotId.Value))
                                        .WhereIf(locations != null && locations.Count > 0, t => t.Lot != null && locations.Contains(t.Lot.LocationId))
                                        .WhereIf(items != null && items.Count > 0, t => items.Contains(t.ItemId))
                                        .AsNoTracking()

                           join b in _itemIssueItemBatchNoRepository.GetAll().AsNoTracking()
                                        .Select(s => new BatchNoItemOutput
                                        {
                                            Id = s.Id,
                                            BatchNoId = s.BatchNoId,
                                            BatchNumber = s.BatchNo.Code,
                                            ExpirationDate = s.BatchNo.ExpirationDate,
                                            TransactionItemId = s.ItemIssueItemId,
                                            Qty = s.Qty
                                        })
                           on isue.Id equals b.TransactionItemId
                           into batchs

                           select new
                           {
                               LotId = isue.LotId,
                               ItemId = isue.ItemId,
                               ItemIssueId = isue.ItemIssueId,
                               Qty = isue.Qty,
                               CreationTime = isue.CreationTime,
                               Description = isue.Description,
                               LotName = isue.Lot == null ? "" : isue.Lot.LotName,
                               LocationId = isue.Lot == null ? (long?)null : isue.Lot.LocationId,
                               LocationName = isue.Lot == null ? "" : isue.Lot.Location.LocationName,
                               Id = isue.Id,
                               BatchNos = batchs.ToList()
                           };


            var itemIssueItemQuery = from isue in iiiQuery
                                     join j in iiJournalQuery
                                     on isue.ItemIssueId equals j.ItemIssueId
                                     select new ItemTransactionDto
                                     {
                                         UserName = j.UserName,
                                         UserId = j.UserId,
                                         LotId = isue.LotId.Value,
                                         LotName = isue.LotName,
                                         IsPurchase = false,
                                         ItemId = isue.ItemId,
                                         TransactionId = isue.ItemIssueId,
                                         TransactionItemId = isue.Id,
                                         ProductionProcessName = j.ProcessName,
                                         Qty = isue.Qty * -1,
                                         LocationId = isue.LocationId.Value,
                                         LocationName = isue.LocationName,
                                         JournalDate = j.Date,
                                         JournalType = j.JournalType,
                                         JournalNo = j.JournalNo,
                                         JournalId = j.Id,
                                         CreationTimeIndex = j.CreationTimeIndex,
                                         CreationTime = isue.CreationTime,
                                         JournalMemo = j.Memo,
                                         JournalReference = j.Reference,
                                         PartnerName = j.CustomerName,
                                         Description = isue.Description,
                                         ItemBatchNos = isue.BatchNos,
                                         JournalTransactionTypeName = j.JournalTransactionTypeName,
                                         JournalTransactionTypeId = j.JournalTransactionTypeId,
                                         Issue = j.Issue
                                     };


            var ivJournalQuery = _journalRepository.GetAll()
                                .Where(s => s.Status == TransactionStatus.Publish)
                                .Where(s => s.ItemIssueVendorCreditId.HasValue)
                                .WhereIf(users != null && users.Count > 0, t => users.Contains(t.CreatorUserId))
                                .WhereIf(journalTypes != null && journalTypes.Count > 0, t => journalTypes.Contains(t.JournalType))
                                .WhereIf(journalTransactionTypeIds != null && journalTransactionTypeIds.Count > 0, t => t.JournalTransactionTypeId != null && journalTransactionTypeIds.Contains(t.JournalTransactionTypeId.Value))
                                .WhereIf(inventoryTypes != null && inventoryTypes.Count > 0, s => s.JournalTransactionType != null &&
                                                             ((inventoryTypes.Contains((long)InventoryType.ItemIssueSale) && s.JournalTransactionType.IsIssue) ||
                                                             (inventoryTypes.Contains((long)InventoryType.ItemReceiptPurchase) && !s.JournalTransactionType.IsIssue)))
                                .WhereIf(fromDate != null && toDate != null, u => u.Date.Date >= fromDate.Date && u.Date.Date <= toDate.Date)
                                .AsNoTracking()
                                .Select(j => new
                                {
                                    Id = j.Id,
                                    UserId = j.CreatorUser.Id,
                                    Date = j.Date,
                                    JournalType = j.JournalType,
                                    JournalNo = j.JournalNo,
                                    JournalId = j.Id,
                                    CreationTimeIndex = j.CreationTimeIndex,
                                    Memo = j.Memo,
                                    Reference = j.Reference,
                                    ItemIssueVendorCreditId = j.ItemIssueVendorCreditId,
                                    UserName = j.CreatorUser.UserName,
                                    VendorId = j.ItemIssueVendorCredit.VendorId,
                                    VendorName = j.ItemIssueVendorCredit.Vendor.VendorName,
                                    JournalTransactionTypeName = j.JournalTransactionTypeId != null ? j.JournalTransactionType.Name : "",
                                    Issue = j.JournalTransactionTypeId != null ? j.JournalTransactionType.IsIssue : (bool?)null,
                                    JournalTransactionTypeId = j.JournalTransactionTypeId,
                                });

            var iviQuery = from isv in _itemIssueVendorCreditItemRepository.GetAll()
                                        .WhereIf(lots != null && lots.Count > 0, t => t.LotId != null && lots.Contains(t.LotId.Value))
                                        .WhereIf(locations != null && locations.Count > 0, t => t.Lot != null && locations.Contains(t.Lot.LocationId))
                                        .WhereIf(items != null && items.Count > 0, t => items.Contains(t.ItemId))
                                        .AsNoTracking()

                           join b in _itemIssueVendorCreditItemBatchNoRepository.GetAll().AsNoTracking()
                                        .Select(s => new BatchNoItemOutput
                                        {
                                            Id = s.Id,
                                            BatchNoId = s.BatchNoId,
                                            BatchNumber = s.BatchNo.Code,
                                            ExpirationDate = s.BatchNo.ExpirationDate,
                                            TransactionItemId = s.ItemIssueVendorCreditItemId,
                                            Qty = s.Qty
                                        })
                           on isv.Id equals b.TransactionItemId
                           into batchs

                           select new
                           {
                               LotId = isv.LotId,
                               ItemId = isv.ItemId,
                               ItemIssueVendorCreditId = isv.ItemIssueVendorCreditId,
                               Qty = isv.Qty,
                               CreationTime = isv.CreationTime,
                               Description = isv.Description,
                               LotName = isv.Lot == null ? "" : isv.Lot.LotName,
                               LocationId = isv.Lot == null ? (long?)null : isv.Lot.LocationId,
                               LocationName = isv.Lot == null ? "" : isv.Lot.Location.LocationName,
                               Id = isv.Id,
                               BatchNos = batchs.ToList()
                           };


            var itemIssueVendorCreditItemQuery = from isv in iviQuery
                                                 join j in ivJournalQuery
                                                 on isv.ItemIssueVendorCreditId equals j.ItemIssueVendorCreditId
                                                 select new ItemTransactionDto
                                                 {
                                                     ProductionProcessName = "",
                                                     UserName = j.UserName,
                                                     UserId = j.UserId,
                                                     LotId = isv.LotId.Value,
                                                     LotName = isv.LotName,
                                                     IsPurchase = false,
                                                     ItemId = isv.ItemId,
                                                     TransactionId = isv.ItemIssueVendorCreditId,
                                                     TransactionItemId = isv.Id,
                                                     Qty = isv.Qty * -1,
                                                     LocationId = isv.LocationId.Value,
                                                     LocationName = isv.LocationName,
                                                     JournalDate = j.Date,
                                                     JournalType = j.JournalType,
                                                     JournalNo = j.JournalNo,
                                                     JournalId = j.Id,
                                                     CreationTimeIndex = j.CreationTimeIndex,
                                                     CreationTime = isv.CreationTime,
                                                     JournalMemo = j.Memo,
                                                     JournalReference = j.Reference,
                                                     PartnerName = j.VendorName,
                                                     Description = isv.Description,
                                                     ItemBatchNos = isv.BatchNos,
                                                     Issue = j.Issue,
                                                     JournalTransactionTypeName = j.JournalTransactionTypeName,
                                                     JournalTransactionTypeId = j.JournalTransactionTypeId,
                                                 };

            var itemReceiptQuery = itemReceiptCustomerCreditItemQuery.Concat(itemReceiptItemQuery);
            var itemIssueQuery = itemIssueVendorCreditItemQuery.Concat(itemIssueItemQuery);

            var inventoryTransactionItemQuery = itemReceiptQuery.Concat(itemIssueQuery);

            return inventoryTransactionItemQuery;

        }


        public List<InventoryValuationDetail> CalculateManualInventoryValuationDetail(
            List<AccountCycle> accountCyles,
            List<ItemDto> inventoryItemDetail,
            //Dictionary<Guid, List<RoundingAdjustmentItemOutput>> roundingItemsOutput = null,
            Dictionary<Guid, Guid> journalToRecalculateCost = null,
            bool groupByLocation = false,
            Dictionary<string, ItemCostSummaryDto> itemLatestCosts = null)
        {

            inventoryItemDetail = inventoryItemDetail.
                OrderBy(o => o.JournalDate.Date).
                ThenBy(o => o.CreationTimeIndex).
                ThenBy(o => o.CreationTime).ToList();

            var result = new List<InventoryValuationDetail>();

            var tempGropByItem = new Dictionary<string, InventoryValuationDetailForCalculation>();
            var tempGropByItemAndLot = new Dictionary<string, InventoryValuationDetail>();

            //TransferOrder: $OrderId - Item - Qty\            
            var issuesForTransferOrderDic = new Dictionary<string, InventoryValuationDetailItem>();

            //Latest cost for new period
            var itemLatestCostCurrentPeriods = new Dictionary<string, ItemCostSummaryDto>();

            var inventoryJournalByItemList = inventoryItemDetail;


            foreach (var j in inventoryJournalByItemList)
            {
                var tran = j;

                var closePeriod = accountCyles.Where(u => u.StartDate.Date <= tran.JournalDate.Date &&
                                                      (u.EndDate == null || tran.JournalDate.Date <= u.EndDate.Value.Date))
                                                      .FirstOrDefault() ?? accountCyles.First(s => s.EndDate == null);

                var roundingDigit = closePeriod == null ? 2 : closePeriod.RoundingDigit;
                var roundingDigitUnitCost = closePeriod == null ? 2 : closePeriod.RoundingDigitUnitCost;

                var costKey = $"{tran.ItemId.ToString()}-{tran.LocationId}";
                if (!tempGropByItem.ContainsKey(costKey))
                {
                    var tempValuation = new InventoryValuationDetailForCalculation
                    {
                        ItemCode = tran.ItemCode,
                        ItemName = tran.ItemName,
                        ItemId = tran.ItemId,
                        InventoryAccountId = tran.InventoryAccountId,
                        InventoryAccountCode = tran.InventoryAccountCode,
                        InventoryAccountName = tran.InventoryAccountName,
                        LocationId = tran.LocationId,
                        LocationName = tran.LocationName,
                        TotalCost = 0,
                        TotalQty = 0,
                        CurrentAvgCost = 0,
                        SalePrice = tran.SalePrice == null ? 0 : 0,
                        //Items = new List<InventoryValuationDetailItem>(),
                        Unit = tran.Unit,
                        ItemTypeId = tran.ItemTypeId,
                        ItemTypeName = tran.ItemTypeName,
                    };

                    //result.Add(tempValuation);
                    tempGropByItem.Add(costKey, tempValuation);
                }

                var inventoryValuationDetailGroupByItem = tempGropByItem[costKey];

                var itemLotKey = groupByLocation ? costKey : $"{tran.ItemId.ToString()}-{tran.LotId}";

                InventoryValuationDetail tempValuationItemLot = null;

                if (!tempGropByItemAndLot.ContainsKey(itemLotKey))
                {
                    tempValuationItemLot = new InventoryValuationDetail()
                    {
                        LotId = tran.LotId,
                        LotName = tran.LotName,
                        ItemCode = tran.ItemCode,
                        ItemName = tran.ItemName,
                        ItemId = tran.ItemId,
                        InventoryAccountId = tran.InventoryAccountId,
                        InventoryAccountCode = tran.InventoryAccountCode,
                        InventoryAccountName = tran.InventoryAccountName,
                        LocationId = tran.LocationId,
                        LocationName = tran.LocationName,
                        TotalCost = 0,
                        TotalQty = 0,
                        CurrentAvgCost = 0,
                        SalePrice = tran.SalePrice == null ? 0 : 0,
                        Unit = tran.Unit,
                        ItemTypeId = tran.ItemTypeId,
                        ItemTypeName = tran.ItemTypeName,
                        IsPurchase = tran.IsPurchase,
                        Items = new List<InventoryValuationDetailItem>(),
                    };

                    tempGropByItemAndLot.Add(itemLotKey, tempValuationItemLot);
                    result.Add(tempValuationItemLot);
                }

                tempValuationItemLot = tempGropByItemAndLot[itemLotKey];


                //////Adding Transction Item //////
                var inventoryValuationItem = new InventoryValuationDetailItem
                {

                    TransferOrderItemId = tran.TransferOrderItemId,
                    LotId = tran.LotId,
                    LotName = tran.LotName,
                    JournalType = tran.JournalType,
                    Date = tran.JournalDate,
                    ItemCode = tran.ItemCode,
                    ItemName = tran.ItemName,
                    ItemId = tran.ItemId,
                    InventoryAccountId = tran.InventoryAccountId,
                    JournalNo = tran.JournalNo,
                    JournalId = tran.JournalId,
                    CreationTimeIndex = tran.CreationTimeIndex,
                    TransactionId = tran.TransactionId,
                    TransactionItemId = tran.TransactionItemId,
                    LocationId = tran.LocationId,
                    LocationName = tran.LocationName,
                    OrderId = tran.OrderId,
                    RoundingDigit = roundingDigit,
                    RoundingDigitUnitCost = roundingDigitUnitCost,
                    Reference = tran.JournalReference,
                    Properties = tran.Properties != null ? tran.Properties.ToList() : null
                };

                var item = tempValuationItemLot.Items.FirstOrDefault(s => s.JournalType == null && tran.JournalType == null);

                if (item == null)
                {
                    tempValuationItemLot.Items.Add(inventoryValuationItem);
                }
                else
                {
                    inventoryValuationItem = item;
                }

                //////End Adding Transction Item //////

                if (tran.IsPurchase)
                {

                    var lineCost = Math.Round(tran.TotalCost, roundingDigit, MidpointRounding.AwayFromZero);
                    var unitCost = Math.Round(tran.UnitCost, roundingDigitUnitCost, MidpointRounding.AwayFromZero);

                    if (tran.JournalType != null && tran.JournalType == JournalType.ItemReceiptTransfer
                        && tran.OrderId != null && tran.TransferOrderItemId != null)
                    {
                        var qtyKey = Math.Abs(Math.Round(tran.Qty, roundingDigitUnitCost,
                                                             MidpointRounding.AwayFromZero));

                        var key = $"{tran.TransferOrderItemId.Value.ToString()}-{inventoryValuationItem.ItemId.ToString()}-{qtyKey}";

                        if (issuesForTransferOrderDic.ContainsKey(key))
                        {
                            var issueItem = issuesForTransferOrderDic[key];
                            unitCost = issueItem.UnitCost;
                            lineCost = Math.Abs(issueItem.LineTotal);
                        }


                        ////recalculate
                        //if (journalToRecalculateCost != null && tran.JournalId != null && tran.TotalCost != lineCost)
                        //{
                        //    if (!journalToRecalculateCost.ContainsKey(tran.JournalId.Value))
                        //    {
                        //        journalToRecalculateCost.Add(tran.JournalId.Value, tran.JournalId.Value);
                        //    }
                        //};

                    }
                    else if (tran.JournalType != null && tran.JournalType == JournalType.ItemReceiptCustomerCredit)
                    {

                        unitCost = itemLatestCostCurrentPeriods.ContainsKey(costKey) ? itemLatestCostCurrentPeriods[costKey].Cost :
                                   itemLatestCosts != null && itemLatestCosts.ContainsKey(costKey) ? itemLatestCosts[costKey].Cost :
                                   tran.PurchaseCost;

                        lineCost = Math.Abs(Math.Round(unitCost * tran.Qty, roundingDigitUnitCost, MidpointRounding.AwayFromZero));

                    }


                    var oldTotalQty = inventoryValuationDetailGroupByItem.TotalQty;

                    inventoryValuationDetailGroupByItem.TotalQty += tran.Qty;

                    var oldTotalCost = inventoryValuationDetailGroupByItem.TotalCost;

                    inventoryValuationDetailGroupByItem.TotalCost = inventoryValuationDetailGroupByItem.TotalQty == 0 ? 0 :
                                                                    tran.JournalType == null ? tran.TotalLocationCost :
                                                                    inventoryValuationDetailGroupByItem.TotalCost + lineCost;

                    //Adjust Cost to Zero
                    if (oldTotalQty < 0 && tran.JournalType != null)
                    {
                        inventoryValuationDetailGroupByItem.TotalCost = Math.Round(inventoryValuationDetailGroupByItem.TotalQty * unitCost, roundingDigit);
                        var adjustmentAmount = Math.Round(oldTotalCost + lineCost - inventoryValuationDetailGroupByItem.TotalCost, roundingDigit);

                        inventoryValuationItem.AdjustmentJournalItem = new AdjustmentJournalItem
                        {
                            JournalId = inventoryValuationItem.JournalId.Value,
                            Description = "Adjusment To Zero",
                            InventoryAccountId = inventoryValuationItem.InventoryAccountId,
                            ItemReceiptItemId = inventoryValuationItem.TransactionItemId,
                            Total = adjustmentAmount,
                        };

                    }

                    //Todo: rounding digit
                    inventoryValuationDetailGroupByItem.CurrentAvgCost = Math.Abs(Math.Round(inventoryValuationDetailGroupByItem.TotalQty == 0 ? 0 :
                                                              tran.JournalType == null ? tran.UnitCost :
                                                              inventoryValuationDetailGroupByItem.TotalCost / inventoryValuationDetailGroupByItem.TotalQty,
                                                              roundingDigitUnitCost,
                                                              MidpointRounding.AwayFromZero));

                    tempValuationItemLot.TotalQty += tran.Qty;

                    var currentAvgCost = inventoryValuationDetailGroupByItem.CurrentAvgCost;

                    //Update avg cost for all item in same location
                    var updateItems = tempGropByItemAndLot.Values.Where(s =>
                       s.ItemId == tempValuationItemLot.ItemId &&
                       s.LocationId == tempValuationItemLot.LocationId
                       && s.IsPurchase
                        //&& !s.Equals(tempValuationItemLot)
                        ).ToList();

                    if (updateItems != null && updateItems.Any())
                    {
                        updateItems.ForEach(s =>
                        {
                            s.CurrentAvgCost = currentAvgCost;
                            s.TotalCost = Math.Round(s.TotalQty * currentAvgCost, roundingDigit, MidpointRounding.AwayFromZero);

                            s.TotalLocationQty = inventoryValuationDetailGroupByItem.TotalQty;
                            s.TotalLocationCost = inventoryValuationDetailGroupByItem.TotalCost;
                        });
                    }


                    inventoryValuationItem.InQty = tran.JournalType == null ? tran.TotalLocationQty : tran.Qty;
                    inventoryValuationItem.UnitCost = tran.JournalType == null ? tran.UnitCost : unitCost;
                    inventoryValuationItem.LineTotal = tran.JournalType == null ? tran.TotalLocationCost : lineCost;
                    inventoryValuationItem.TotalQty = inventoryValuationDetailGroupByItem.TotalQty;
                    inventoryValuationItem.TotalCost = inventoryValuationDetailGroupByItem.TotalCost;
                    inventoryValuationItem.AVGCost = tran.JournalType == null ? tran.UnitCost : inventoryValuationDetailGroupByItem.CurrentAvgCost;

                }
                else
                {

                    inventoryValuationDetailGroupByItem.CurrentAvgCost = Math.Abs(Math.Round(inventoryValuationDetailGroupByItem.TotalQty == 0 ?
                                                                       0 :
                                                                       inventoryValuationDetailGroupByItem.TotalCost / inventoryValuationDetailGroupByItem.TotalQty,
                                                                       roundingDigitUnitCost,
                                                                       MidpointRounding.AwayFromZero));


                    var lineTotal = Math.Round(inventoryValuationDetailGroupByItem.CurrentAvgCost * tran.Qty, roundingDigit);
                    inventoryValuationDetailGroupByItem.TotalQty += tran.Qty;

                    //adjustment when qty = 0 
                    if (inventoryValuationDetailGroupByItem.TotalQty == 0) lineTotal = -inventoryValuationDetailGroupByItem.TotalCost;

                    var totalCostBeforeZero = inventoryValuationDetailGroupByItem.TotalCost + lineTotal;

                    inventoryValuationDetailGroupByItem.TotalCost = inventoryValuationDetailGroupByItem.TotalQty == 0 ? 0 :
                                                                     totalCostBeforeZero;

                    inventoryValuationItem.OutQty = tran.Qty;
                    inventoryValuationItem.LineTotal = lineTotal;
                    inventoryValuationItem.TotalQty = inventoryValuationDetailGroupByItem.TotalQty;
                    inventoryValuationItem.UnitCost = inventoryValuationDetailGroupByItem.CurrentAvgCost;
                    inventoryValuationItem.TotalCost = inventoryValuationDetailGroupByItem.TotalCost;
                    inventoryValuationItem.AVGCost = Math.Abs(Math.Round(inventoryValuationDetailGroupByItem.TotalQty == 0 ?
                                                              0 :
                                                              inventoryValuationDetailGroupByItem.TotalCost / inventoryValuationDetailGroupByItem.TotalQty,
                                                              roundingDigitUnitCost,
                                                              MidpointRounding.AwayFromZero));

                    inventoryValuationDetailGroupByItem.CurrentAvgCost = inventoryValuationItem.AVGCost;


                    tempValuationItemLot.TotalQty += tran.Qty;


                    var currentAvgCost = inventoryValuationDetailGroupByItem.CurrentAvgCost;

                    //Update avg cost for all item in same location
                    var updateItems = tempGropByItemAndLot.Values.Where(s =>
                       s.ItemId == tempValuationItemLot.ItemId &&
                       s.LocationId == tempValuationItemLot.LocationId
                       && s.IsPurchase
                        //&& !s.Equals(tempValuationItemLot)
                        ).ToList();

                    if (updateItems != null && updateItems.Any())
                    {
                        updateItems.ForEach(s =>
                        {
                            s.CurrentAvgCost = currentAvgCost;
                            s.TotalCost = Math.Round(s.TotalQty * currentAvgCost, roundingDigit, MidpointRounding.AwayFromZero);

                            s.TotalLocationQty = inventoryValuationDetailGroupByItem.TotalQty;
                            s.TotalLocationCost = inventoryValuationDetailGroupByItem.TotalCost;
                        });
                    }


                    ////recalculate
                    //if (journalToRecalculateCost != null
                    //    && tran.JournalId != null
                    //    && tran.TotalCost != inventoryValuationItem.LineTotal)
                    //{
                    //    if (!journalToRecalculateCost.ContainsKey(tran.JournalId.Value))
                    //    {
                    //        journalToRecalculateCost.Add(tran.JournalId.Value, tran.JournalId.Value);
                    //    }
                    //};


                    if (tran.JournalType != null && tran.JournalType == JournalType.ItemIssueTransfer && tran.OrderId != null &&
                        tran.TransferOrderItemId != null)
                    {

                        var qtyKey = Math.Abs(Math.Round(tran.Qty, roundingDigitUnitCost,
                                                              MidpointRounding.AwayFromZero));

                        var key = $"{tran.TransferOrderItemId.Value.ToString()}-{inventoryValuationItem.ItemId.ToString()}-{qtyKey}";
                        if (!issuesForTransferOrderDic.ContainsKey(key))
                        {
                            issuesForTransferOrderDic.Add(key, inventoryValuationItem);
                        }
                        else
                        {
                            issuesForTransferOrderDic[key] = inventoryValuationItem;
                        }

                    }


                }


                if (inventoryValuationDetailGroupByItem.TotalQty != 0)
                {
                    if (itemLatestCostCurrentPeriods.ContainsKey(costKey))
                    {
                        itemLatestCostCurrentPeriods[costKey].Cost = inventoryValuationDetailGroupByItem.CurrentAvgCost;
                        itemLatestCostCurrentPeriods[costKey].TotalCost = inventoryValuationDetailGroupByItem.TotalCost;
                        itemLatestCostCurrentPeriods[costKey].Qty = inventoryValuationDetailGroupByItem.TotalQty;
                    }
                    else
                    {
                        var itemLastCost = new ItemCostSummaryDto
                        {
                            ItemId = inventoryValuationDetailGroupByItem.ItemId,
                            LocationId = inventoryValuationDetailGroupByItem.LocationId,
                            LocationName = inventoryValuationDetailGroupByItem.LocationName,
                            Qty = inventoryValuationDetailGroupByItem.TotalQty,
                            Cost = inventoryValuationDetailGroupByItem.CurrentAvgCost,
                            TotalCost = inventoryValuationDetailGroupByItem.TotalCost,
                        };
                        itemLatestCostCurrentPeriods.Add(costKey, itemLastCost);
                    }
                }

            }

            return result;

        }


        public List<InventoryTransactionItemDto> CalculateInventoryByItems(
            List<InventoryTransactionItem> beginningList,
            List<InventoryTransactionItem> inventoryItemDetail,
            Dictionary<string, ItemCostSummaryDto> itemLatestCosts = null,
            int roundingDigit = 2,
            int roundingDigitUnitCost = 2,
            bool groupByLocation = false)
        {

            var result = new List<InventoryTransactionItemDto>();

            //var tempGropByItem = new Dictionary<string, InventoryValuationDetailForCalculation>();
            //var tempGropByItemAndLot = new Dictionary<string, InventoryValuationDetail>();

            ////TransferOrder: $OrderId - Item - Qty\            
            //var issuesForTransferOrderDic = new Dictionary<string, InventoryValuationDetailItem>();

            ////Latest cost for new period
            //var itemLatestCostCurrentPeriods = new Dictionary<string, ItemCostSummaryDto>();

            //var inventoryJournalByItemList = inventoryItemDetail;


            //foreach (var j in inventoryJournalByItemList)
            //{
            //    var tran = j;


            //    var costKey = $"{tran.ItemId.ToString()}-{tran.LocationId}";
            //    if (!tempGropByItem.ContainsKey(costKey))
            //    {
            //        var tempValuation = new InventoryValuationDetailForCalculation
            //        {
            //            ItemCode = tran.ItemCode,
            //            ItemName = tran.ItemName,
            //            ItemId = tran.ItemId,
            //            InventoryAccountId = tran.InventoryAccountId,
            //            InventoryAccountCode = tran.InventoryAccountCode,
            //            InventoryAccountName = tran.InventoryAccountName,
            //            LocationId = tran.LocationId,
            //            LocationName = tran.LocationName,
            //            TotalCost = 0,
            //            TotalQty = 0,
            //            CurrentAvgCost = 0,
            //            SalePrice = tran.SalePrice == null ? 0 : 0,
            //            //Items = new List<InventoryValuationDetailItem>(),
            //            Unit = tran.Unit,
            //            ItemTypeId = tran.ItemTypeId,
            //            ItemTypeName = tran.ItemTypeName,
            //        };

            //        //result.Add(tempValuation);
            //        tempGropByItem.Add(costKey, tempValuation);
            //    }

            //    var inventoryValuationDetailGroupByItem = tempGropByItem[costKey];

            //    var itemLotKey = groupByLocation ? costKey : $"{tran.ItemId.ToString()}-{tran.LotId}";

            //    InventoryValuationDetail tempValuationItemLot = null;

            //    if (!tempGropByItemAndLot.ContainsKey(itemLotKey))
            //    {
            //        tempValuationItemLot = new InventoryValuationDetail()
            //        {
            //            LotId = tran.LotId,
            //            LotName = tran.LotName,
            //            ItemCode = tran.ItemCode,
            //            ItemName = tran.ItemName,
            //            ItemId = tran.ItemId,
            //            InventoryAccountId = tran.InventoryAccountId,
            //            InventoryAccountCode = tran.InventoryAccountCode,
            //            InventoryAccountName = tran.InventoryAccountName,
            //            LocationId = tran.LocationId,
            //            LocationName = tran.LocationName,
            //            TotalCost = 0,
            //            TotalQty = 0,
            //            CurrentAvgCost = 0,
            //            SalePrice = tran.SalePrice == null ? 0 : 0,
            //            Unit = tran.Unit,
            //            ItemTypeId = tran.ItemTypeId,
            //            ItemTypeName = tran.ItemTypeName,
            //            IsPurchase = tran.IsPurchase,
            //            Items = new List<InventoryValuationDetailItem>(),
            //        };

            //        tempGropByItemAndLot.Add(itemLotKey, tempValuationItemLot);
            //        result.Add(tempValuationItemLot);
            //    }

            //    tempValuationItemLot = tempGropByItemAndLot[itemLotKey];


            //    //////Adding Transction Item //////
            //    var inventoryValuationItem = new InventoryValuationDetailItem
            //    {

            //        TransferOrderItemId = tran.TransferOrderItemId,
            //        LotId = tran.LotId,
            //        LotName = tran.LotName,
            //        JournalType = tran.JournalType,
            //        Date = tran.JournalDate,
            //        ItemCode = tran.ItemCode,
            //        ItemName = tran.ItemName,
            //        ItemId = tran.ItemId,
            //        InventoryAccountId = tran.InventoryAccountId,
            //        JournalNo = tran.JournalNo,
            //        JournalId = tran.JournalId,
            //        CreationTimeIndex = tran.CreationTimeIndex,
            //        TransactionId = tran.TransactionId,
            //        TransactionItemId = tran.TransactionItemId,
            //        LocationId = tran.LocationId,
            //        LocationName = tran.LocationName,
            //        OrderId = tran.OrderId,
            //        RoundingDigit = roundingDigit,
            //        RoundingDigitUnitCost = roundingDigitUnitCost,
            //        Reference = tran.JournalReference,
            //        Properties = tran.Properties != null ? tran.Properties.ToList() : null
            //    };

            //    var item = tempValuationItemLot.Items.FirstOrDefault(s => s.JournalType == null && tran.JournalType == null);

            //    if (item == null)
            //    {
            //        tempValuationItemLot.Items.Add(inventoryValuationItem);
            //    }
            //    else
            //    {
            //        inventoryValuationItem = item;
            //    }

            //    //////End Adding Transction Item //////

            //    if (tran.IsItemReceipt)
            //    {

            //        var lineCost = Math.Round(tran.TotalCost, roundingDigit, MidpointRounding.AwayFromZero);
            //        var unitCost = Math.Round(tran.UnitCost, roundingDigitUnitCost, MidpointRounding.AwayFromZero);

            //        if (tran.JournalType != null && tran.JournalType == JournalType.ItemReceiptTransfer
            //            && tran.TransferOrProductionId != null && tran.TransferItemId != null)
            //        {
            //            var qtyKey = Math.Abs(Math.Round(tran.Qty, roundingDigitUnitCost,
            //                                                 MidpointRounding.AwayFromZero));

            //            var key = $"{tran.TransferItemId.Value.ToString()}-{inventoryValuationItem.ItemId.ToString()}-{qtyKey}";

            //            if (issuesForTransferOrderDic.ContainsKey(key))
            //            {
            //                var issueItem = issuesForTransferOrderDic[key];
            //                unitCost = issueItem.UnitCost;
            //                lineCost = Math.Abs(issueItem.LineTotal);
            //            }

            //        }
            //        else if (tran.JournalType != null && tran.JournalType == JournalType.ItemReceiptCustomerCredit)
            //        {

            //            unitCost = itemLatestCostCurrentPeriods.ContainsKey(costKey) ? itemLatestCostCurrentPeriods[costKey].Cost :
            //                       itemLatestCosts != null && itemLatestCosts.ContainsKey(costKey) ? itemLatestCosts[costKey].Cost : 0;

            //            lineCost = Math.Abs(Math.Round(unitCost * tran.Qty, roundingDigitUnitCost, MidpointRounding.AwayFromZero));

            //        }


            //        var oldTotalQty = inventoryValuationDetailGroupByItem.TotalQty;

            //        inventoryValuationDetailGroupByItem.TotalQty += tran.Qty;

            //        var oldTotalCost = inventoryValuationDetailGroupByItem.TotalCost;

            //        inventoryValuationDetailGroupByItem.TotalCost = inventoryValuationDetailGroupByItem.TotalQty == 0 ? 0 :
            //                                                        tran.JournalType == null ? tran.TotalLocationCost :
            //                                                        inventoryValuationDetailGroupByItem.TotalCost + lineCost;

            //        //Adjust Cost to Zero
            //        if (oldTotalQty < 0 && tran.JournalType != null)
            //        {
            //            inventoryValuationDetailGroupByItem.TotalCost = Math.Round(inventoryValuationDetailGroupByItem.TotalQty * unitCost, roundingDigit);
            //            var adjustmentAmount = Math.Round(oldTotalCost + lineCost - inventoryValuationDetailGroupByItem.TotalCost, roundingDigit);

            //            inventoryValuationItem.AdjustmentJournalItem = new AdjustmentJournalItem
            //            {
            //                JournalId = inventoryValuationItem.JournalId.Value,
            //                Description = "Adjusment To Zero",
            //                InventoryAccountId = inventoryValuationItem.InventoryAccountId,
            //                ItemReceiptItemId = inventoryValuationItem.TransactionItemId,
            //                Total = adjustmentAmount,
            //            };

            //        }

            //        //Todo: rounding digit
            //        inventoryValuationDetailGroupByItem.CurrentAvgCost = Math.Abs(Math.Round(inventoryValuationDetailGroupByItem.TotalQty == 0 ? 0 :
            //                                                  tran.JournalType == null ? tran.UnitCost :
            //                                                  inventoryValuationDetailGroupByItem.TotalCost / inventoryValuationDetailGroupByItem.TotalQty,
            //                                                  roundingDigitUnitCost,
            //                                                  MidpointRounding.AwayFromZero));

            //        tempValuationItemLot.TotalQty += tran.Qty;

            //        var currentAvgCost = inventoryValuationDetailGroupByItem.CurrentAvgCost;

            //        //Update avg cost for all item in same location
            //        var updateItems = tempGropByItemAndLot.Values.Where(s =>
            //           s.ItemId == tempValuationItemLot.ItemId &&
            //           s.LocationId == tempValuationItemLot.LocationId
            //           && s.IsPurchase
            //            //&& !s.Equals(tempValuationItemLot)
            //            ).ToList();

            //        if (updateItems != null && updateItems.Any())
            //        {
            //            updateItems.ForEach(s =>
            //            {
            //                s.CurrentAvgCost = currentAvgCost;
            //                s.TotalCost = Math.Round(s.TotalQty * currentAvgCost, roundingDigit, MidpointRounding.AwayFromZero);

            //                s.TotalLocationQty = inventoryValuationDetailGroupByItem.TotalQty;
            //                s.TotalLocationCost = inventoryValuationDetailGroupByItem.TotalCost;
            //            });
            //        }


            //        inventoryValuationItem.InQty = tran.JournalType == null ? tran.TotalLocationQty : tran.Qty;
            //        inventoryValuationItem.UnitCost = tran.JournalType == null ? tran.UnitCost : unitCost;
            //        inventoryValuationItem.LineTotal = tran.JournalType == null ? tran.TotalLocationCost : lineCost;
            //        inventoryValuationItem.TotalQty = inventoryValuationDetailGroupByItem.TotalQty;
            //        inventoryValuationItem.TotalCost = inventoryValuationDetailGroupByItem.TotalCost;
            //        inventoryValuationItem.AVGCost = tran.JournalType == null ? tran.UnitCost : inventoryValuationDetailGroupByItem.CurrentAvgCost;

            //    }
            //    else
            //    {

            //        inventoryValuationDetailGroupByItem.CurrentAvgCost = Math.Abs(Math.Round(inventoryValuationDetailGroupByItem.TotalQty == 0 ?
            //                                                           0 :
            //                                                           inventoryValuationDetailGroupByItem.TotalCost / inventoryValuationDetailGroupByItem.TotalQty,
            //                                                           roundingDigitUnitCost,
            //                                                           MidpointRounding.AwayFromZero));


            //        var lineTotal = Math.Round(inventoryValuationDetailGroupByItem.CurrentAvgCost * tran.Qty, roundingDigit);
            //        inventoryValuationDetailGroupByItem.TotalQty += tran.Qty;

            //        //adjustment when qty = 0 
            //        if (inventoryValuationDetailGroupByItem.TotalQty == 0) lineTotal = -inventoryValuationDetailGroupByItem.TotalCost;

            //        var totalCostBeforeZero = inventoryValuationDetailGroupByItem.TotalCost +
            //                                                         lineTotal;

            //        inventoryValuationDetailGroupByItem.TotalCost = inventoryValuationDetailGroupByItem.TotalQty == 0 ? 0 :
            //                                                         totalCostBeforeZero;

            //        inventoryValuationItem.OutQty = tran.Qty;
            //        inventoryValuationItem.LineTotal = lineTotal;
            //        inventoryValuationItem.TotalQty = inventoryValuationDetailGroupByItem.TotalQty;
            //        inventoryValuationItem.UnitCost = inventoryValuationDetailGroupByItem.CurrentAvgCost;
            //        inventoryValuationItem.TotalCost = inventoryValuationDetailGroupByItem.TotalCost;
            //        inventoryValuationItem.AVGCost = Math.Abs(Math.Round(inventoryValuationDetailGroupByItem.TotalQty == 0 ?
            //                                                  0 :
            //                                                  inventoryValuationDetailGroupByItem.TotalCost / inventoryValuationDetailGroupByItem.TotalQty,
            //                                                  roundingDigitUnitCost,
            //                                                  MidpointRounding.AwayFromZero));

            //        inventoryValuationDetailGroupByItem.CurrentAvgCost = inventoryValuationItem.AVGCost;


            //        tempValuationItemLot.TotalQty += tran.Qty;


            //        var currentAvgCost = inventoryValuationDetailGroupByItem.CurrentAvgCost;

            //        //Update avg cost for all item in same location
            //        var updateItems = tempGropByItemAndLot.Values.Where(s =>
            //           s.ItemId == tempValuationItemLot.ItemId &&
            //           s.LocationId == tempValuationItemLot.LocationId
            //           && s.IsPurchase
            //            //&& !s.Equals(tempValuationItemLot)
            //            ).ToList();

            //        if (updateItems != null && updateItems.Any())
            //        {
            //            updateItems.ForEach(s =>
            //            {
            //                s.CurrentAvgCost = currentAvgCost;
            //                s.TotalCost = Math.Round(s.TotalQty * currentAvgCost, roundingDigit, MidpointRounding.AwayFromZero);

            //                s.TotalLocationQty = inventoryValuationDetailGroupByItem.TotalQty;
            //                s.TotalLocationCost = inventoryValuationDetailGroupByItem.TotalCost;
            //            });
            //        }


            //        if (tran.JournalType != null && tran.JournalType == JournalType.ItemIssueTransfer && tran.TransferOrProductionId != null &&
            //            tran.TransferItemId != null)
            //        {

            //            var qtyKey = Math.Abs(Math.Round(tran.Qty, roundingDigitUnitCost,
            //                                                  MidpointRounding.AwayFromZero));

            //            var key = $"{tran.TransferItemId.Value.ToString()}-{inventoryValuationItem.ItemId.ToString()}-{qtyKey}";
            //            if (!issuesForTransferOrderDic.ContainsKey(key))
            //            {
            //                issuesForTransferOrderDic.Add(key, inventoryValuationItem);
            //            }
            //            else
            //            {
            //                issuesForTransferOrderDic[key] = inventoryValuationItem;
            //            }

            //        }


            //    }


            //    if (inventoryValuationDetailGroupByItem.TotalQty != 0)
            //    {
            //        if (itemLatestCostCurrentPeriods.ContainsKey(costKey))
            //        {
            //            itemLatestCostCurrentPeriods[costKey].Cost = inventoryValuationDetailGroupByItem.CurrentAvgCost;
            //            itemLatestCostCurrentPeriods[costKey].TotalCost = inventoryValuationDetailGroupByItem.TotalCost;
            //            itemLatestCostCurrentPeriods[costKey].Qty = inventoryValuationDetailGroupByItem.TotalQty;
            //        }
            //        else
            //        {
            //            var itemLastCost = new ItemCostSummaryDto
            //            {
            //                ItemId = inventoryValuationDetailGroupByItem.ItemId,
            //                LocationId = inventoryValuationDetailGroupByItem.LocationId,
            //                LocationName = inventoryValuationDetailGroupByItem.LocationName,
            //                Qty = inventoryValuationDetailGroupByItem.TotalQty,
            //                Cost = inventoryValuationDetailGroupByItem.CurrentAvgCost,
            //                TotalCost = inventoryValuationDetailGroupByItem.TotalCost,
            //            };
            //            itemLatestCostCurrentPeriods.Add(costKey, itemLastCost);
            //        }
            //    }

            //}

            return result;

        }



        public IQueryable<Guid> GetItemByUserGroup(long userId)
        {
            // get user by group member
            var userGroups = _userGroupMemberRepository.GetAll()
                            .Where(x => x.MemberId == userId)
                            .AsNoTracking()
                            .Select(x => x.UserGroupId)
                            .ToList();

            var @queryByGroup = _itemUserGroupRepository.GetAll()
                            .Include(u => u.Item)
                            .Where(t => t.Item.Member == Member.UserGroup)
                            .Where(t => userGroups.Contains(t.UserGroupId))
                            .AsNoTracking()
                            .Select(t => t.Item);

            var @queryItemAll = _itemRepository.GetAll()
                            .Where(t => t.Member == Member.All)
                            .AsNoTracking();

            var @itemQuery = @queryItemAll.Union(@queryByGroup).Select(t => t.Id);

            return itemQuery;
        }

        private IQueryable<long> GetLocationGroup(long userId)
        {
            var locationGroups = _userGroupMemberRepository.GetAll()
                               .Include(t => t.UserGroup)
                               .Where(x => x.UserGroup.LocationId != null)
                               .Where(x => x.MemberId == userId)
                               .AsNoTracking()
                               .Select(x => x.UserGroup.LocationId.Value);

            var @queryLocation = _locationRepository
                                 .GetAll()
                                 .Where(t => t.Member == Member.All)
                                 .AsNoTracking()
                                 .Select(x => x.Id);
            var result = locationGroups.Union(@queryLocation);

            return result;
        }

        public IQueryable<ItemCostSummaryDto> GetItemCostSummaryByLocationQuery(
            DateTime? fromDate,
            DateTime toDate,
            List<long?> locations,
            long filterUserId,
            List<Guid> items = null,
            List<Guid> exceptIds = null)
        {

            //var periodCycles = GetCloseCyleQuery();

            //var previousCycle = periodCycles.Where(u => u.EndDate != null && u.EndDate.Value.Date <= toDate.Date)
            //                                .OrderByDescending(u => u.EndDate.Value).FirstOrDefault();

            //var currentCycle = periodCycles.Where(u => u.StartDate.Date <= toDate.Date &&
            //                                    (u.EndDate == null || toDate.Date <= u.EndDate.Value.Date))
            //                               .OrderByDescending(u => u.StartDate).FirstOrDefault();

            //var currentRoundingDigit = currentCycle != null ? currentCycle.RoundingDigit :
            //                           previousCycle != null ? previousCycle.RoundingDigit : 2;

            //var currentRoundingDigitUnitCost = currentCycle != null ? currentCycle.RoundingDigitUnitCost :
            //                                   previousCycle != null ? previousCycle.RoundingDigitUnitCost : 2;

            //var inventoryCostClose = from u in _inventoryCostCloseRepository.GetAll()
            //                         .Where(t => previousCycle != null && t.CloseDate.Date == previousCycle.EndDate.Value.Date)
            //                         .WhereIf(locations != null && locations.Count() > 0, t => locations.Contains(t.LocationId))
            //                         .WhereIf(items != null && items.Count() > 0, t => items.Contains(t.ItemId))
            //                         .AsNoTracking()
            //                         select new ItemCostSummaryDto
            //                         {
            //                             IsPurchase = true,
            //                             ItemId = u.ItemId,
            //                             Qty = u.QtyOnhand,
            //                             Cost = u.Cost,
            //                             TotalCost = u.TotalCost, //ToDo multi-lot dupplication cost here
            //                             LocationId = u.LocationId,
            //                             LocationName = u.Location.LocationName,
            //                         };

            //var previoudCycleFromDate = previousCycle == null ? (DateTime?)null : previousCycle.EndDate.Value.Date;

            var accountCycles = GetCloseCyleQuery();
            var currentPeriod = accountCycles.Where(u => u.StartDate.Date <= toDate.Date &&
                                                (u.EndDate == null || toDate.Date <= u.EndDate.Value.Date))
                                           .OrderByDescending(u => u.StartDate).FirstOrDefault();

            var currentRoundingDigit = currentPeriod != null ? currentPeriod.RoundingDigit : 2;

            var currentRoundingDigitUnitCost = currentPeriod != null ? currentPeriod.RoundingDigitUnitCost : 2;


            IQueryable<ItemCostSummaryDto> inventoryCostClose = null;

            if (fromDate.HasValue)
            {
                inventoryCostClose = from u in _inventoryTransactionItemRepository.GetAll()
                                                    .Where(t => t.Date.Date < fromDate.Value.Date)
                                                    .Where(t => locations == null || !locations.Any() || locations.Contains(t.LocationId))
                                                    .Where(s => items == null || !items.Any() || items.Contains(s.ItemId))
                                                    .AsNoTracking()
                                                    .OrderByDescending(u => u.OrderIndex)
                                                    .GroupBy(u => new { u.ItemId, u.LocationId })
                                                    .Select(s => s.FirstOrDefault())
                                     join l in _locationRepository.GetAll()
                                               .AsNoTracking()
                                     on u.LocationId equals l.Id
                                     select new ItemCostSummaryDto
                                     {
                                         IsPurchase = true,
                                         ItemId = u.ItemId,
                                         Qty = u.QtyOnHand,
                                         Cost = u.AvgCost,
                                         TotalCost = u.TotalCost,
                                         LocationId = u.LocationId,
                                         LocationName = l.LocationName,
                                     };

            }


            var previoudCycleFromDate = !fromDate.HasValue ? (DateTime?)null : fromDate.Value.Date.AddDays(-1);

            var inventoryTransactionQuery = GetItemCostSummaryNewPeriodQuery(previoudCycleFromDate, toDate, locations, items, exceptIds);

            var allTransactionQuery = inventoryCostClose == null ? inventoryTransactionQuery : inventoryTransactionQuery.Concat(inventoryCostClose);


            var inventoryTransactionItemQuery = allTransactionQuery
                                    .OrderBy(s => $"{s.ItemId}-{s.LocationId}")
                                    .GroupBy(s => new { s.ItemId, s.LocationId, s.LocationName })
                                    .Select(r => new ItemCostSummaryDto()
                                    {
                                        ItemId = r.Key.ItemId,
                                        BeginningQty = r.Sum(u => u.Date == null || (fromDate != null && u.Date.Value.Date < fromDate.Value.Date) ? u.Qty : 0),
                                        InQty = r.Sum(u => u.IsPurchase && (fromDate == null || (u.Date.HasValue && u.Date.Value.Date >= fromDate.Value.Date)) ? u.Qty : 0),
                                        OutQty = r.Sum(u => !u.IsPurchase && (fromDate == null || (u.Date.HasValue && u.Date.Value.Date >= fromDate.Value.Date)) ? -u.Qty : 0),
                                        Qty = r.Sum(t => t.Qty),
                                        TotalCost = r.Sum(t => Math.Round(t.TotalCost, currentRoundingDigit)),
                                        Cost = r.Sum(t => t.Qty) == 0 ? 0 : Math.Round(r.Sum(t => Math.Round(t.TotalCost, currentRoundingDigit)) / r.Sum(t => t.Qty), currentRoundingDigitUnitCost),
                                        LocationId = r.Key.LocationId,
                                        LocationName = r.Key.LocationName,
                                        RoundingDigit = currentRoundingDigit,
                                        RoundingDigitUnitCost = currentRoundingDigitUnitCost
                                    });

            // get user by group member
            var locationGroups = GetLocationGroup(filterUserId);

            var itemGroup = this.GetItemByUserGroup(filterUserId);
            inventoryTransactionItemQuery = from u in inventoryTransactionItemQuery
                                            join g in locationGroups on u.LocationId equals g
                                            join i in itemGroup on u.ItemId equals i
                                            select u;


            return inventoryTransactionItemQuery;
        }

        public IQueryable<ItemCostSummaryDto> GetItemCostSummaryByLocationQuery_Backup(
        DateTime? fromDate,
        DateTime toDate,
        List<long?> locations,
        long filterUserId,
        List<Guid> items = null,
        List<Guid> exceptIds = null)
        {

            var periodCycles = GetCloseCyleQuery();

            var previousCycle = periodCycles.Where(u => u.EndDate != null && u.EndDate.Value.Date <= toDate.Date)
                                            .OrderByDescending(u => u.EndDate.Value).FirstOrDefault();

            var currentCycle = periodCycles.Where(u => u.StartDate.Date <= toDate.Date &&
                                                (u.EndDate == null || toDate.Date <= u.EndDate.Value.Date))
                                           .OrderByDescending(u => u.StartDate).FirstOrDefault();

            var currentRoundingDigit = currentCycle != null ? currentCycle.RoundingDigit :
                                       previousCycle != null ? previousCycle.RoundingDigit : 2;

            var currentRoundingDigitUnitCost = currentCycle != null ? currentCycle.RoundingDigitUnitCost :
                                               previousCycle != null ? previousCycle.RoundingDigitUnitCost : 2;

            var inventoryCostClose = from u in _inventoryCostCloseRepository.GetAll()
                                     //.Where(t => previousCycle != null && t.CloseDate.Date == previousCycle.EndDate.Value.Date)
                                     .Where(t => previousCycle != null && t.AccountCycleId == previousCycle.Id)
                                     .WhereIf(locations != null && locations.Count() > 0, t => locations.Contains(t.LocationId))
                                     .WhereIf(items != null && items.Count() > 0, t => items.Contains(t.ItemId))
                                     .AsNoTracking()
                                     select new ItemCostSummaryDto
                                     {
                                         IsPurchase = true,
                                         ItemId = u.ItemId,
                                         Qty = u.QtyOnhand,
                                         Cost = u.Cost,
                                         TotalCost = u.TotalCost, //ToDo multi-lot dupplication cost here
                                         LocationId = u.LocationId,
                                         LocationName = u.Location.LocationName,
                                     };

            var previoudCycleFromDate = previousCycle == null ? (DateTime?)null : previousCycle.EndDate.Value.Date;

            var inventoryTransactionQuery = GetItemCostSummaryNewPeriodQuery_Backup(previoudCycleFromDate, toDate, locations, items, exceptIds);

            var allTransactionQuery = inventoryTransactionQuery.Concat(inventoryCostClose);


            var inventoryTransactionItemQuery = allTransactionQuery
                                    .OrderBy(s => s.ItemId)
                                    .ThenBy(s => s.LocationId)
                                    .GroupBy(s => new { s.ItemId, s.LocationId, s.LocationName })
                                    .Select(r => new ItemCostSummaryDto()
                                    {
                                        ItemId = r.Key.ItemId,
                                        BeginningQty = r.Sum(u => u.Date == null || (fromDate != null && u.Date < fromDate) ? u.Qty : 0),
                                        InQty = r.Sum(u => u.IsPurchase && (fromDate == null || u.Date == null || u.Date >= fromDate) ? u.Qty : 0),
                                        OutQty = r.Sum(u => !u.IsPurchase && (fromDate == null || u.Date == null || u.Date >= fromDate) ? -u.Qty : 0),
                                        Qty = r.Sum(t => t.Qty),
                                        TotalCost = r.Sum(t => Math.Round(t.TotalCost, currentRoundingDigit)),
                                        Cost = r.Sum(t => t.Qty) == 0 ? 0 : Math.Round(r.Sum(t => Math.Round(t.TotalCost, currentRoundingDigit)) / r.Sum(t => t.Qty), currentRoundingDigitUnitCost),
                                        LocationId = r.Key.LocationId,
                                        LocationName = r.Key.LocationName,
                                        RoundingDigit = currentRoundingDigit,
                                        RoundingDigitUnitCost = currentRoundingDigitUnitCost
                                    });

            // get user by group member
            var locationGroups = GetLocationGroup(filterUserId);

            var itemGroup = this.GetItemByUserGroup(filterUserId);
            inventoryTransactionItemQuery = from u in inventoryTransactionItemQuery
                                            join g in locationGroups on u.LocationId equals g
                                            join i in itemGroup on u.ItemId equals i
                                            select u;


            return inventoryTransactionItemQuery;
        }


        public IQueryable<ItemCostSummaryDto> GetItemCostSummaryQuery_Backup(
            DateTime? fromDate,
            DateTime toDate,
            List<long?> locations,
            long filterUserId,
            List<Guid> items = null,
            List<Guid> exceptIds = null)
        {

            var periodCycles = GetCloseCyleQuery();

            var previousCycle = periodCycles.Where(u => u.EndDate != null && u.EndDate.Value.Date <= toDate.Date)
                                            .OrderByDescending(u => u.EndDate.Value).FirstOrDefault();

            var currentCycle = periodCycles.Where(u => u.StartDate.Date <= toDate.Date &&
                                                (u.EndDate == null || toDate.Date <= u.EndDate.Value.Date))
                                           .OrderByDescending(u => u.StartDate).FirstOrDefault();

            var currentRoundingDigit = currentCycle != null ? currentCycle.RoundingDigit :
                                       previousCycle != null ? previousCycle.RoundingDigit : 2;

            var currentRoundingDigitUnitCost = currentCycle != null ? currentCycle.RoundingDigitUnitCost :
                                               previousCycle != null ? previousCycle.RoundingDigitUnitCost : 2;

            var inventoryCostClose = from u in _inventoryCostCloseRepository.GetAll()
                                     //.Where(t => previousCycle != null && t.CloseDate.Date == previousCycle.EndDate.Value.Date)
                                     .Where(t => previousCycle != null && t.AccountCycleId == previousCycle.Id)
                                     .WhereIf(locations != null && locations.Count() > 0, t => locations.Contains(t.LocationId))
                                     .WhereIf(items != null && items.Count() > 0, t => items.Contains(t.ItemId))
                                     .AsNoTracking()
                                     select new ItemCostSummaryDto
                                     {
                                         IsPurchase = true,
                                         ItemId = u.ItemId,
                                         Qty = u.QtyOnhand,
                                         Cost = u.Cost,
                                         TotalCost = u.TotalCost, //ToDo multi-lot dupplication cost here
                                         LocationId = u.LocationId,
                                         LocationName = u.Location.LocationName,
                                     };

            var previoudCycleFromDate = previousCycle == null ? (DateTime?)null : previousCycle.EndDate.Value.Date;

            var inventoryTransactionQuery = GetItemCostSummaryNewPeriodQuery_Backup(previoudCycleFromDate, toDate, locations, items, exceptIds);

            var allTransactionQuery = inventoryTransactionQuery.Concat(inventoryCostClose);


            var inventoryTransactionItemQuery = allTransactionQuery
                                    .OrderBy(s => s.ItemId)
                                    .ThenBy(s => s.LocationId)
                                    .GroupBy(s => new { s.ItemId, s.LocationId, s.LocationName })
                                    .Select(r => new ItemCostSummaryDto()
                                    {
                                        ItemId = r.Key.ItemId,
                                        BeginningQty = r.Sum(u => u.Date == null || (fromDate != null && u.Date < fromDate) ? u.Qty : 0),
                                        InQty = r.Sum(u => u.IsPurchase && (fromDate == null || u.Date == null || u.Date >= fromDate) ? u.Qty : 0),
                                        OutQty = r.Sum(u => !u.IsPurchase && (fromDate == null || u.Date == null || u.Date >= fromDate) ? -u.Qty : 0),
                                        Qty = r.Sum(t => t.Qty),
                                        TotalCost = r.Sum(t => Math.Round(t.TotalCost, currentRoundingDigit)),
                                        Cost = r.Sum(t => t.Qty) == 0 ? 0 : Math.Round(r.Sum(t => Math.Round(t.TotalCost, currentRoundingDigit)) / r.Sum(t => t.Qty), currentRoundingDigitUnitCost),
                                        LocationId = r.Key.LocationId,
                                        LocationName = r.Key.LocationName,
                                        RoundingDigit = currentRoundingDigit,
                                        RoundingDigitUnitCost = currentRoundingDigitUnitCost,
                                    });

            // get user by group member
            var locationGroups = GetLocationGroup(filterUserId);

            var itemGroup = this.GetItemByUserGroup(filterUserId);
            inventoryTransactionItemQuery = from u in inventoryTransactionItemQuery
                                            join g in locationGroups on u.LocationId equals g
                                            join i in itemGroup on u.ItemId equals i
                                            select u;

            return inventoryTransactionItemQuery;
        }

        public IQueryable<ItemCostSummaryDto> GetItemCostSummaryQuery(
             DateTime? fromDate,
             DateTime toDate,
             List<long?> locations,
             long filterUserId,
             List<Guid> items = null,
             List<Guid> exceptIds = null)
        {

            //var periodCycles = GetCloseCyleQuery();

            //var previousCycle = periodCycles.Where(u => u.EndDate != null && u.EndDate.Value.Date <= toDate.Date)
            //                                .OrderByDescending(u => u.EndDate.Value).FirstOrDefault();

            //var currentCycle = periodCycles.Where(u => u.StartDate.Date <= toDate.Date &&
            //                                    (u.EndDate == null || toDate.Date <= u.EndDate.Value.Date))
            //                               .OrderByDescending(u => u.StartDate).FirstOrDefault();

            //var currentRoundingDigit = currentCycle != null ? currentCycle.RoundingDigit :
            //                           previousCycle != null ? previousCycle.RoundingDigit : 2;

            //var currentRoundingDigitUnitCost = currentCycle != null ? currentCycle.RoundingDigitUnitCost :
            //                                   previousCycle != null ? previousCycle.RoundingDigitUnitCost : 2;

            //var inventoryCostClose = from u in _inventoryCostCloseRepository.GetAll()
            //                         .Where(t => previousCycle != null && t.CloseDate.Date == previousCycle.EndDate.Value.Date)
            //                         .WhereIf(locations != null && locations.Count() > 0, t => locations.Contains(t.LocationId))
            //                         .WhereIf(items != null && items.Count() > 0, t => items.Contains(t.ItemId))
            //                         .AsNoTracking()
            //                         select new ItemCostSummaryDto
            //                         {
            //                             IsPurchase = true,
            //                             ItemId = u.ItemId,
            //                             Qty = u.QtyOnhand,
            //                             Cost = u.Cost,
            //                             TotalCost = u.TotalCost, //ToDo multi-lot dupplication cost here
            //                             LocationId = u.LocationId,
            //                             LocationName = u.Location.LocationName,
            //                         };

            //var previoudCycleFromDate = fromDate ? null ? (DateTime?)null : previousCycle.EndDate.Value.Date;

            //var inventoryTransactionQuery = GetItemCostSummaryNewPeriodQuery(previoudCycleFromDate, toDate, locations, items, exceptIds);


            var accountCycles = GetCloseCyleQuery();
            var currentPeriod = accountCycles.Where(u => u.StartDate.Date <= toDate.Date &&
                                                (u.EndDate == null || toDate.Date <= u.EndDate.Value.Date))
                                           .OrderByDescending(u => u.StartDate).FirstOrDefault();

            var currentRoundingDigit = currentPeriod != null ? currentPeriod.RoundingDigit : 2;

            var currentRoundingDigitUnitCost = currentPeriod != null ? currentPeriod.RoundingDigitUnitCost : 2;


            IQueryable<ItemCostSummaryDto> inventoryCostClose = null;

            if (fromDate.HasValue)
            {
                inventoryCostClose = from u in _inventoryTransactionItemRepository.GetAll()
                                                    .Where(t => t.Date.Date < fromDate.Value.Date)
                                                    .Where(t => locations == null || !locations.Any() || locations.Contains(t.LocationId))
                                                    .Where(s => items == null || !items.Any() || items.Contains(s.ItemId))
                                                    .AsNoTracking()
                                                    .OrderByDescending(u => u.OrderIndex)
                                                    .GroupBy(u => new { u.ItemId, u.LocationId })
                                                    .Select(s => s.FirstOrDefault())
                                     join l in _locationRepository.GetAll()
                                               .AsNoTracking()
                                     on u.LocationId equals l.Id
                                     select new ItemCostSummaryDto
                                     {
                                         IsPurchase = true,
                                         ItemId = u.ItemId,
                                         Qty = u.QtyOnHand,
                                         Cost = u.AvgCost,
                                         TotalCost = u.TotalCost,
                                         LocationId = u.LocationId,
                                         LocationName = l.LocationName,
                                     };

            }


            var previoudCycleFromDate = !fromDate.HasValue ? (DateTime?)null : fromDate.Value.Date.AddDays(-1);

            var inventoryTransactionQuery = GetItemCostSummaryNewPeriodQuery(previoudCycleFromDate, toDate, locations, items, exceptIds);

            var allTransactionQuery = inventoryCostClose == null ? inventoryTransactionQuery : inventoryTransactionQuery.Concat(inventoryCostClose);


            var inventoryTransactionItemQuery = allTransactionQuery
                                    .OrderBy(s => $"{s.ItemId}-{s.LocationId}")
                                    .GroupBy(s => new { s.ItemId, s.LocationId, s.LocationName })
                                    .Select(r => new ItemCostSummaryDto()
                                    {
                                        ItemId = r.Key.ItemId,
                                        BeginningQty = r.Sum(u => u.Date == null || (fromDate != null && u.Date.Value.Date < fromDate.Value.Date) ? u.Qty : 0),
                                        InQty = r.Sum(u => u.IsPurchase && (fromDate == null || (u.Date.HasValue && u.Date.Value.Date >= fromDate.Value.Date)) ? u.Qty : 0),
                                        OutQty = r.Sum(u => !u.IsPurchase && (fromDate == null || (u.Date.HasValue && u.Date.Value.Date >= fromDate.Value.Date)) ? -u.Qty : 0),
                                        Qty = r.Sum(t => t.Qty),
                                        TotalCost = r.Sum(t => Math.Round(t.TotalCost, currentRoundingDigit)),
                                        Cost = r.Sum(t => t.Qty) == 0 ? 0 : Math.Round(r.Sum(t => Math.Round(t.TotalCost, currentRoundingDigit)) / r.Sum(t => t.Qty), currentRoundingDigitUnitCost),
                                        LocationId = r.Key.LocationId,
                                        LocationName = r.Key.LocationName,
                                        RoundingDigit = currentRoundingDigit,
                                        RoundingDigitUnitCost = currentRoundingDigitUnitCost,
                                    });

            // get user by group member
            var locationGroups = GetLocationGroup(filterUserId);

            var itemGroup = this.GetItemByUserGroup(filterUserId);
            inventoryTransactionItemQuery = from u in inventoryTransactionItemQuery
                                            join g in locationGroups on u.LocationId equals g
                                            join i in itemGroup on u.ItemId equals i
                                            select u;

            return inventoryTransactionItemQuery;
        }

        public IQueryable<ItemCostSummaryDto> GetItemCostSummaryNewPeriodQuery_Backup(
          DateTime? fromDate,
          DateTime? toDate,
          List<long?> locations,
          List<Guid> items,
          List<Guid> exceptIds = null)
        {

            var itemReceiptItemQuery = from Ir in _itemReceiptItemRepository.GetAll()
                                              .WhereIf(locations != null && locations.Count > 0, t => locations.Contains(t.Lot.LocationId))
                                              .WhereIf(items != null && items.Count() > 0, t => items.Contains(t.ItemId))
                                              .WhereIf(exceptIds != null, t => !exceptIds.Contains(t.ItemReceiptId))
                                              .AsNoTracking()

                                       join ji in _journalItemRepository.GetAll()
                                           .Where(s => s.Identifier != null && (s.Key == PostingKey.Inventory || s.Key == PostingKey.InventoryAdjustmentInv))
                                           .AsNoTracking()
                                       on Ir.Id equals ji.Identifier

                                       join j in _journalRepository.GetAll()
                                           .Where(s => s.ItemReceiptId.HasValue)
                                           .Where(s => s.Status == TransactionStatus.Publish)
                                           .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                           .WhereIf(toDate != null, u => u.Date.Date <= toDate.Value.Date)
                                           .AsNoTracking()
                                       on Ir.ItemReceiptId equals j.ItemReceiptId
                                       select new ItemCostSummaryDto
                                       {
                                           Date = j.Date,
                                           IsPurchase = true,
                                           ItemId = Ir.ItemId,
                                           Qty = ji.Key == PostingKey.Inventory ? Ir.Qty : 0,
                                           Cost = ji.Key == PostingKey.Inventory ? Ir.UnitCost : 0,
                                           TotalCost = ji.Debit - ji.Credit,
                                           LocationId = Ir.Lot == null ? 0 : Ir.Lot.LocationId,
                                           LocationName = Ir.Lot == null ? "" : Ir.Lot.Location.LocationName,
                                           Description = Ir.Description
                                       };

            var itemReceiptCustomerCreditItemQuery = from IrC in _itemReceiptCustomerCreditItemRepository.GetAll()
                                                     .WhereIf(locations != null && locations.Count > 0, t => locations.Contains(t.Lot.LocationId))
                                                     .WhereIf(items != null && items.Count() > 0, t => items.Contains(t.ItemId))
                                                     .WhereIf(exceptIds != null, t => !exceptIds.Contains(t.ItemReceiptCustomerCreditId))
                                                     .AsNoTracking()

                                                     join ji in _journalItemRepository.GetAll()
                                                     .Where(s => s.Identifier != null && (s.Key == PostingKey.Inventory || s.Key == PostingKey.InventoryAdjustmentInv))
                                                     .AsNoTracking()
                                                     on IrC.Id equals ji.Identifier

                                                     join j in _journalRepository.GetAll()
                                                     .Where(s => s.ItemReceiptCustomerCreditId.HasValue)
                                                     .Where(s => s.Status == TransactionStatus.Publish)
                                                     .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                                     .WhereIf(toDate != null, u => u.Date.Date <= toDate.Value.Date)
                                                     .AsNoTracking()
                                                     on IrC.ItemReceiptCustomerCreditId equals j.ItemReceiptCustomerCreditId
                                                     select new ItemCostSummaryDto
                                                     {
                                                         Date = j.Date,
                                                         IsPurchase = true,
                                                         ItemId = IrC.ItemId,
                                                         Qty = ji.Key == PostingKey.Inventory ? IrC.Qty : 0,
                                                         Cost = ji.Key == PostingKey.Inventory ? IrC.UnitCost : 0,
                                                         TotalCost = ji.Debit - ji.Credit,
                                                         LocationId = IrC.Lot == null ? 0 : IrC.Lot.LocationId,
                                                         LocationName = IrC.Lot == null ? "" : IrC.Lot.Location.LocationName,
                                                         Description = IrC.Description
                                                     };

            var itemReceiptResult = itemReceiptItemQuery.Concat(itemReceiptCustomerCreditItemQuery);


            var itemIssueItemQuery = (from isue in _itemIssueItemRepository.GetAll()
                                        .WhereIf(locations != null && locations.Count > 0, t => locations.Contains(t.Lot.LocationId))
                                        .WhereIf(items != null && items.Count() > 0, t => items.Contains(t.ItemId))
                                        .WhereIf(exceptIds != null, t => !exceptIds.Contains(t.ItemIssueId))
                                        .AsNoTracking()
                                      join j in _journalRepository.GetAll()
                                      .Where(s => s.ItemIssueId.HasValue)
                                      .Where(s => s.Status == TransactionStatus.Publish)
                                      .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                      .WhereIf(toDate != null, u => u.Date.Date <= toDate.Value.Date)
                                      .AsNoTracking()
                                      on isue.ItemIssueId equals j.ItemIssueId
                                      select new ItemCostSummaryDto
                                      {
                                          Date = j.Date,
                                          IsPurchase = false,
                                          ItemId = isue.ItemId,
                                          Qty = (isue.Qty * -1),
                                          Cost = isue.UnitCost,
                                          TotalCost = -isue.Total,
                                          LocationId = isue.Lot == null ? 0 : isue.Lot.LocationId,
                                          LocationName = isue.Lot == null ? "" : isue.Lot.Location.LocationName,
                                          Description = isue.Description
                                      });

            var itemIssueVendorCreditItemQuery = from isv in _itemIssueVendorCreditItemRepository.GetAll()
                                                        .WhereIf(locations != null && locations.Count > 0, t => locations.Contains(t.Lot.LocationId))
                                                        .WhereIf(items != null && items.Count() > 0, t => items.Contains(t.ItemId))
                                                        .WhereIf(exceptIds != null, t => !exceptIds.Contains(t.ItemIssueVendorCreditId))
                                                        .AsNoTracking()
                                                 join j in _journalRepository.GetAll()
                                                     .Where(s => s.ItemIssueVendorCreditId.HasValue)
                                                     .Where(s => s.Status == TransactionStatus.Publish)
                                                     .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                                     .WhereIf(toDate != null, u => u.Date.Date <= toDate.Value.Date)
                                                     .AsNoTracking()
                                                 on isv.ItemIssueVendorCreditId equals j.ItemIssueVendorCreditId
                                                 select new ItemCostSummaryDto
                                                 {
                                                     Date = j.Date,
                                                     IsPurchase = false,
                                                     ItemId = isv.ItemId,
                                                     Qty = (isv.Qty * -1),
                                                     Cost = isv.UnitCost,
                                                     TotalCost = -isv.Total,
                                                     LocationId = isv.Lot == null ? 0 : isv.Lot.LocationId,
                                                     LocationName = isv.Lot == null ? "" : isv.Lot.Location.LocationName,
                                                     Description = isv.Description
                                                 };
            var itemIssueResult = itemIssueItemQuery.Concat(itemIssueVendorCreditItemQuery);

            var inventoryTransactionItemQuery = itemReceiptResult.Concat(itemIssueResult);

            return inventoryTransactionItemQuery;
        }

        public IQueryable<ItemCostSummaryDto> GetItemCostSummaryNewPeriodQuery(
         DateTime? fromDate,
         DateTime? toDate,
         List<long?> locations,
         List<Guid> items,
         List<Guid> exceptIds = null)
        {

            //var itemReceiptItemQuery = from Ir in _itemReceiptItemRepository.GetAll()
            //                                  .WhereIf(locations != null && locations.Count > 0, t => locations.Contains(t.Lot.LocationId))
            //                                  .WhereIf(items != null && items.Count() > 0, t => items.Contains(t.ItemId))
            //                                  .WhereIf(exceptIds != null, t => !exceptIds.Contains(t.ItemReceiptId))
            //                                  .AsNoTracking()

            //                           join ji in _journalItemRepository.GetAll()
            //                               .Where(s => s.Identifier != null && (s.Key == PostingKey.Inventory || s.Key == PostingKey.InventoryAdjustmentInv))
            //                               .AsNoTracking()
            //                           on Ir.Id equals ji.Identifier

            //                           join j in _journalRepository.GetAll()
            //                               .Where(s => s.ItemReceiptId.HasValue)
            //                               .Where(s => s.Status == TransactionStatus.Publish)
            //                               .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
            //                               .WhereIf(toDate != null, u => u.Date.Date <= toDate.Value.Date)
            //                               .AsNoTracking()
            //                           on Ir.ItemReceiptId equals j.ItemReceiptId
            //                           select new ItemCostSummaryDto
            //                           {
            //                               Date = j.Date,
            //                               IsPurchase = true,
            //                               ItemId = Ir.ItemId,
            //                               Qty = ji.Key == PostingKey.Inventory ? Ir.Qty : 0,
            //                               Cost = ji.Key == PostingKey.Inventory ? Ir.UnitCost : 0,
            //                               TotalCost = ji.Debit - ji.Credit,
            //                               LocationId = Ir.Lot == null ? 0 : Ir.Lot.LocationId,
            //                               LocationName = Ir.Lot == null ? "" : Ir.Lot.Location.LocationName,
            //                               Description = Ir.Description
            //                           };

            //var itemReceiptCustomerCreditItemQuery = from IrC in _itemReceiptCustomerCreditItemRepository.GetAll()
            //                                         .WhereIf(locations != null && locations.Count > 0, t => locations.Contains(t.Lot.LocationId))
            //                                         .WhereIf(items != null && items.Count() > 0, t => items.Contains(t.ItemId))
            //                                         .WhereIf(exceptIds != null, t => !exceptIds.Contains(t.ItemReceiptCustomerCreditId))
            //                                         .AsNoTracking()

            //                                         join ji in _journalItemRepository.GetAll()
            //                                         .Where(s => s.Identifier != null && (s.Key == PostingKey.Inventory || s.Key == PostingKey.InventoryAdjustmentInv))
            //                                         .AsNoTracking()
            //                                         on IrC.Id equals ji.Identifier

            //                                         join j in _journalRepository.GetAll()
            //                                         .Where(s => s.ItemReceiptCustomerCreditId.HasValue)
            //                                         .Where(s => s.Status == TransactionStatus.Publish)
            //                                         .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
            //                                         .WhereIf(toDate != null, u => u.Date.Date <= toDate.Value.Date)
            //                                         .AsNoTracking()
            //                                         on IrC.ItemReceiptCustomerCreditId equals j.ItemReceiptCustomerCreditId
            //                                         select new ItemCostSummaryDto
            //                                         {
            //                                             Date = j.Date,
            //                                             IsPurchase = true,
            //                                             ItemId = IrC.ItemId,
            //                                             Qty = ji.Key == PostingKey.Inventory ? IrC.Qty : 0,
            //                                             Cost = ji.Key == PostingKey.Inventory ? IrC.UnitCost : 0,
            //                                             TotalCost = ji.Debit - ji.Credit,
            //                                             LocationId = IrC.Lot == null ? 0 : IrC.Lot.LocationId,
            //                                             LocationName = IrC.Lot == null ? "" : IrC.Lot.Location.LocationName,
            //                                             Description = IrC.Description
            //                                         };

            //var itemReceiptResult = itemReceiptItemQuery.Concat(itemReceiptCustomerCreditItemQuery);


            //var itemIssueItemQuery = (from isue in _itemIssueItemRepository.GetAll()
            //                            .WhereIf(locations != null && locations.Count > 0, t => locations.Contains(t.Lot.LocationId))
            //                            .WhereIf(items != null && items.Count() > 0, t => items.Contains(t.ItemId))
            //                            .WhereIf(exceptIds != null, t => !exceptIds.Contains(t.ItemIssueId))
            //                            .AsNoTracking()
            //                          join j in _journalRepository.GetAll()
            //                          .Where(s => s.ItemIssueId.HasValue)
            //                          .Where(s => s.Status == TransactionStatus.Publish)
            //                          .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
            //                          .WhereIf(toDate != null, u => u.Date.Date <= toDate.Value.Date)
            //                          .AsNoTracking()
            //                          on isue.ItemIssueId equals j.ItemIssueId
            //                          select new ItemCostSummaryDto
            //                          {
            //                              Date = j.Date,
            //                              IsPurchase = false,
            //                              ItemId = isue.ItemId,
            //                              Qty = (isue.Qty * -1),
            //                              Cost = isue.UnitCost,
            //                              TotalCost = -isue.Total,
            //                              LocationId = isue.Lot == null ? 0 : isue.Lot.LocationId,
            //                              LocationName = isue.Lot == null ? "" : isue.Lot.Location.LocationName,
            //                              Description = isue.Description
            //                          });

            //var itemIssueVendorCreditItemQuery = from isv in _itemIssueVendorCreditItemRepository.GetAll()
            //                                            .WhereIf(locations != null && locations.Count > 0, t => locations.Contains(t.Lot.LocationId))
            //                                            .WhereIf(items != null && items.Count() > 0, t => items.Contains(t.ItemId))
            //                                            .WhereIf(exceptIds != null, t => !exceptIds.Contains(t.ItemIssueVendorCreditId))
            //                                            .AsNoTracking()
            //                                     join j in _journalRepository.GetAll()
            //                                         .Where(s => s.ItemIssueVendorCreditId.HasValue)
            //                                         .Where(s => s.Status == TransactionStatus.Publish)
            //                                         .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
            //                                         .WhereIf(toDate != null, u => u.Date.Date <= toDate.Value.Date)
            //                                         .AsNoTracking()
            //                                     on isv.ItemIssueVendorCreditId equals j.ItemIssueVendorCreditId
            //                                     select new ItemCostSummaryDto
            //                                     {
            //                                         Date = j.Date,
            //                                         IsPurchase = false,
            //                                         ItemId = isv.ItemId,
            //                                         Qty = (isv.Qty * -1),
            //                                         Cost = isv.UnitCost,
            //                                         TotalCost = -isv.Total,
            //                                         LocationId = isv.Lot == null ? 0 : isv.Lot.LocationId,
            //                                         LocationName = isv.Lot == null ? "" : isv.Lot.Location.LocationName,
            //                                         Description = isv.Description
            //                                     };
            //var itemIssueResult = itemIssueItemQuery.Concat(itemIssueVendorCreditItemQuery);

            //var inventoryTransactionItemQuery = itemReceiptResult.Concat(itemIssueResult);

            var inventoryTransactionItemQuery = from s in _inventoryTransactionItemRepository.GetAll()
                                                        .Where(t => locations == null || locations.Count == 0 || locations.Contains(t.LocationId))
                                                        .Where(t => items == null || items.Count == 0 || items.Contains(t.ItemId))
                                                        .Where(t => exceptIds == null || exceptIds.Count == 0 || !exceptIds.Contains(t.TransactionId))
                                                        .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                                        .Where(t => toDate == null || t.Date.Date <= toDate.Value.Date)
                                                        .AsNoTracking()
                                                join l in _locationRepository.GetAll()
                                                        .AsNoTracking()
                                                on s.LocationId equals l.Id
                                                select new ItemCostSummaryDto
                                                {
                                                    Date = s.Date,
                                                    IsPurchase = s.IsItemReceipt,
                                                    ItemId = s.ItemId,
                                                    Qty = s.Qty,
                                                    Cost = s.UnitCost,
                                                    TotalCost = s.LineCost - s.AdjustmentCost,
                                                    LocationId = s.LocationId,
                                                    LocationName = l.LocationName,
                                                    Description = s.Description
                                                };


            return inventoryTransactionItemQuery;
        }


        public async Task<List<InventoryCostItem>> GetItemsByAverageCost(
           DateTime toDate, List<long?> locations,
           List<long?> lots,
           Dictionary<Guid, Guid> updatedItemIssueIds = null,
           Dictionary<Guid, Guid> itemsToSelect = null)
        {

            var periodCycles = await GetCloseCyleQuery().ToListAsync();

            var previousCycle = periodCycles.Where(u => u.EndDate != null &&
                                                        u.EndDate.Value.Date <= toDate.Date)
                                                        .OrderByDescending(u => u.EndDate.Value).FirstOrDefault();

            var currentCycle = periodCycles.Where(u => u.StartDate.Date <= toDate.Date && (u.EndDate == null || toDate.Date <= u.EndDate.Value.Date))
                                            .OrderByDescending(u => u.StartDate).FirstOrDefault();

            var currentRoundingDigit = currentCycle != null ? currentCycle.RoundingDigit :
                                       previousCycle != null ? previousCycle.RoundingDigit : 2;

            var currentRoundingDigitUnitCost = currentCycle != null ? currentCycle.RoundingDigitUnitCost :
                                       previousCycle != null ? previousCycle.RoundingDigitUnitCost : 2;

            var inventoryCostClose = (from u in _inventoryCostCloseItemRepository.GetAll()

                                         //.Where(t => previousCycle != null && t.InventoryCostClose.CloseDate.Date == previousCycle.EndDate.Value.Date)
                                         .Where(t => previousCycle != null && t.InventoryCostClose.AccountCycleId == previousCycle.Id)
                                         .WhereIf(itemsToSelect != null && itemsToSelect.Count > 0, t => itemsToSelect.ContainsKey(t.InventoryCostClose.ItemId))
                                         .WhereIf(locations != null && locations.Count() > 0, t => locations.Contains(t.Lot.LocationId))
                                         .AsNoTracking()

                                      select new ItemDto
                                      {
                                          TransferOrderItemId = null,
                                          IsPurchase = true,
                                          ItemId = u.InventoryCostClose.ItemId,
                                          TransactionId = null,
                                          TotalCost = Math.Round(u.Qty * u.InventoryCostClose.Cost, currentRoundingDigit, MidpointRounding.AwayFromZero),
                                          UnitCost = u.InventoryCostClose.Cost,
                                          Qty = u.Qty,

                                          LocationId = u.Lot.LocationId,
                                          LotId = u.LotId == null ? 0 : u.LotId.Value,
                                          LotName = u.Lot.LotName,
                                          TotalLocationQty = u.InventoryCostClose.QtyOnhand,
                                          TotalLocationCost = u.InventoryCostClose.TotalCost,

                                      });


            var fromDate = previousCycle == null ? (DateTime?)null : previousCycle.EndDate.Value.Date;

            var inventoryTransactionQuery = GetInventoryDetailQuery(fromDate, toDate, locations, updatedItemIssueIds, itemsToSelect);

            var inventoryTransactionItemQuery = await inventoryTransactionQuery.Concat(inventoryCostClose).ToListAsync();


            var saleReturnItemIds = inventoryTransactionItemQuery.Where(s => s.JournalType == JournalType.ItemReceiptCustomerCredit)
                                    .GroupBy(g => g.ItemId).Select(s => s.Key).ToList();
            var itemLatestCostDic = fromDate == null ? null : await this.GetItemLatestCostSummaryQuery(fromDate.Value, locations, saleReturnItemIds, null)
                                    .ToDictionaryAsync(s => $"{s.ItemId}-{s.LocationId}", s => s);

            var calculatedResult = CalculateManualInventoryValuationDetail(periodCycles, inventoryTransactionItemQuery, null, false, itemLatestCostDic);

            calculatedResult = calculatedResult.WhereIf(locations != null && locations.Count() > 0,
                                                       u => locations.Contains(u.LocationId))
                                                .WhereIf(lots != null && lots.Count() > 0,
                                                        u => lots.Contains(u.LotId))
                                                .ToList();

            var result = calculatedResult.Select(r =>
               new InventoryCostItem()
               {
                   LotId = r.LotId,
                   LotName = r.LotName,
                   Id = r.ItemId,
                   //ItemCode = r.ItemCode,
                   //ItemName = r.ItemName,
                   //SalePrice = r.SalePrice,
                   QtyOnHand = r.TotalQty,
                   AverageCost = r.CurrentAvgCost,
                   TotalCost = r.TotalCost,
                   //InventoryAccountId = r.InventoryAccountId,
                   //InventoryAccountName = r.InventoryAccountName,
                   //InventoryAccountCode = r.InventoryAccountCode,
                   LocationId = r.LocationId,
                   //LocationName = r.LocationName,
                   RoundingDigit = currentRoundingDigit,
                   RoundingDigitUnitCost = currentRoundingDigitUnitCost,
                   //Unit = r.Unit,
                   //ItemTypeId = r.ItemTypeId,
                   //ItemTypeName = r.ItemTypeName,

                   TotalLocationCost = r.TotalLocationCost,
                   TotalLocationQty = r.TotalLocationQty,
               }).ToList();



            return result;
        }

        public async Task<IQueryable<InventoryCostItem>> GetItemsBalance(string filter,
                               DateTime toDate,
                               List<long?> locations,
                               Dictionary<Guid, Guid> updatedItemIssueIds = null, FilterType filterType = FilterType.Contain)
        {

            var periodCycles = await GetCloseCyleQuery().ToListAsync();

            var previousCycle = periodCycles.Where(u => u.EndDate != null &&
                                                        u.EndDate.Value.Date <= toDate.Date)
                                                        .OrderByDescending(u => u.EndDate.Value).FirstOrDefault();

            var currentCycle = periodCycles.Where(u => u.StartDate.Date <= toDate.Date && (u.EndDate == null || toDate.Date <= u.EndDate.Value.Date))
                                            .OrderByDescending(u => u.StartDate).FirstOrDefault();

            var itemQuery = _itemRepository.GetAll()
                            .WhereIf(!filter.IsNullOrEmpty(), p =>
                                         (filterType == FilterType.Contain && (p.ItemName.ToLower().Contains(filter.ToLower()) || p.ItemCode.ToLower().Contains(filter.ToLower()) || (p.Barcode != null && p.Barcode.ToLower().Contains(filter.ToLower())))) ||
                                         (filterType == FilterType.StartWith && (p.ItemName.ToLower().StartsWith(filter.ToLower()) || p.ItemCode.ToLower().StartsWith(filter.ToLower()) || (p.Barcode != null && p.Barcode.ToLower().StartsWith(filter.ToLower())))) ||
                                         (filterType == FilterType.Exact && (p.ItemName.ToLower().Equals(filter.ToLower()) || p.ItemCode.ToLower().Equals(filter.ToLower()) || (p.Barcode != null && p.Barcode.ToLower().Equals(filter.ToLower()))))
                                     )
                            .AsNoTracking()
                            .Select(s => s.Id);

            var lotQuery = _lotRepository.GetAll()
                           .WhereIf(locations != null && locations.Count() > 0, t => locations.Contains(t.LocationId))
                           .AsNoTracking()
                           .Select(s => new
                           {
                               Id = s.Id,
                               LotName = s.LotName
                           });

            var inventoryCostClose = from u in _inventoryCostCloseItemRepository.GetAll()
                                                 //.Where(t => previousCycle != null && t.InventoryCostClose.CloseDate.Date == previousCycle.EndDate.Value.Date)
                                                 .Where(t => previousCycle != null && t.InventoryCostClose.AccountCycleId == previousCycle.Id)
                                                 .AsNoTracking()
                                     join l in lotQuery
                                     on u.LotId equals l.Id
                                     join i in itemQuery
                                     on u.InventoryCostClose.ItemId equals i
                                     select new ItemBalanceDto
                                     {
                                         IsPurchase = true,
                                         ItemId = u.InventoryCostClose.ItemId,
                                         Qty = u.Qty,
                                         LotId = l.Id,
                                         LotName = l.LotName
                                     };


            var fromDate = previousCycle == null ? (DateTime?)null : previousCycle.EndDate.Value.Date;

            var inventoryTransactionQuery = GetInventoryItemBalanceQuery(filter, fromDate, toDate, locations, updatedItemIssueIds, filterType);

            var inventoryTransactionItemQuery = inventoryTransactionQuery.Concat(inventoryCostClose)
                                    .OrderBy(s => s.ItemId)
                                    .ThenBy(s => s.LotId)
                                    .ThenBy(s => s.LotName)
                                    .GroupBy(s => new { s.ItemId, s.LotId, s.LotName })
                                    .Select(r => new InventoryCostItem()
                                    {
                                        LotId = r.Key.LotId,
                                        LotName = r.Key.LotName,
                                        Id = r.Key.ItemId,
                                        QtyOnHand = r.Sum(t => t.Qty)
                                    });

            return inventoryTransactionItemQuery;
        }


        public IQueryable<ItemBalanceDto> GetItemsBalanceForReport(
         string filter,
         DateTime? fromDate,
         DateTime toDate,
         List<long?> locations,
         List<long?> lots,
         List<Guid?> inventoryAccounts,
         List<Guid> items,
         List<GetListPropertyFilter> itemProperties,
         long? userId = null,
         List<Guid> journalTransactionTypeIds = null,
         List<long> inventoryTypes = null)
        {

            var periodCycles = GetCloseCyleQuery();

            var previousCycle = periodCycles.Where(u => u.EndDate != null &&
                                                        u.EndDate.Value.Date <= toDate.Date)
                                                        .OrderByDescending(u => u.EndDate.Value).FirstOrDefault();

            var currentCycle = periodCycles.Where(u => u.StartDate.Date <= toDate.Date && (u.EndDate == null || toDate.Date <= u.EndDate.Value.Date))
                                            .OrderByDescending(u => u.StartDate).FirstOrDefault();

            var inventoryCostClose = from u in _inventoryCostCloseItemRepository.GetAll()
                                     //.Where(t => previousCycle != null && t.InventoryCostClose.CloseDate.Date == previousCycle.EndDate.Value.Date)
                                     .Where(t => previousCycle != null && t.InventoryCostClose.AccountCycleId == previousCycle.Id)
                                     .WhereIf(locations != null && locations.Count() > 0, t => locations.Contains(t.Lot.LocationId))
                                     .WhereIf(lots != null && lots.Count() > 0, t => lots.Contains(t.LotId))

                                     .AsNoTracking()
                                     select new ItemBalanceDto
                                     {
                                         IsPurchase = true,
                                         ItemId = u.InventoryCostClose.ItemId,
                                         Qty = u.Qty,
                                         LotId = u.LotId == null ? 0 : u.LotId.Value,
                                         LotName = u.Lot == null ? "" : u.Lot.LotName,
                                         LocationId = u.Lot == null ? 0 : u.Lot.LocationId,
                                         LocationName = u.Lot == null ? "" : u.Lot.Location.LocationName,
                                     };



            var previousCycleFromDate = previousCycle == null ? (DateTime?)null : previousCycle.EndDate.Value.Date;


            var inventoryTransactionQuery = GetItemBalanceForReportQuery(filter, previousCycleFromDate, toDate, locations, lots, inventoryAccounts, items, itemProperties, journalTransactionTypeIds);

            var inventoryTransactionItemQuery = inventoryTransactionQuery.Concat(inventoryCostClose)
                                    .OrderBy(s => s.ItemId)
                                    .ThenBy(s => s.LotId)
                                    .GroupBy(s => new { s.ItemId, s.LotId, s.LotName, s.LocationId, s.LocationName })
                                    .Select(r => new ItemBalanceDto()
                                    {
                                        LotId = r.Key.LotId,
                                        LotName = r.Key.LotName,
                                        ItemId = r.Key.ItemId,
                                        Qty = r.Sum(t => t.Qty),
                                        BeginningQty = r.Sum(u => u.Date == null || (fromDate != null && u.Date < fromDate) ? u.Qty : 0),
                                        InQty = r.Sum(u => u.IsPurchase && (fromDate == null || u.Date == null || u.Date >= fromDate) ? u.Qty : 0),
                                        OutQty = r.Sum(u => !u.IsPurchase && (fromDate == null || u.Date == null || u.Date >= fromDate) ? -u.Qty : 0),

                                        LocationId = r.Key.LocationId,
                                        LocationName = r.Key.LocationName,
                                    });

            if (userId != null)
            {
                var itemByUserGroup = GetItemByUserGroup(userId.Value);
                // get user by group member
                var locationGroups = GetLocationGroup(userId.Value);

                inventoryTransactionItemQuery = (from u in inventoryTransactionItemQuery
                                                 join g in locationGroups
                                                  on u.LocationId equals g
                                                 join it in itemByUserGroup
                                                 on u.ItemId equals it
                                                 select u);

            }

            return inventoryTransactionItemQuery;
        }


        public IQueryable<ItemBalanceDto> GetItemBalanceForReportQuery(
         string filter,
         DateTime? fromDate,
         DateTime? toDate,
         List<long?> locations,
         List<long?> lots,
         List<Guid?> inventoryAccounts,
         List<Guid> items,
         List<GetListPropertyFilter> itemProperties,
         List<Guid> journalTransactionTypeIds = null,
         List<long> inventoryTypes = null)
        {

            var itemReceiptItemQuery = (from Ir in _itemReceiptItemRepository.GetAll()
                                                  .WhereIf(locations != null && locations.Count > 0, t => t.Lot != null && locations.Contains(t.Lot.LocationId))
                                                  .WhereIf(lots != null && lots.Count > 0, t => lots.Contains(t.LotId))
                                                  .AsNoTracking()

                                        join j in _journalRepository.GetAll()
                                                    .Where(s => s.ItemReceiptId.HasValue)
                                                    .Where(s => s.Status == TransactionStatus.Publish)
                                                    .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                                    .WhereIf(journalTransactionTypeIds != null && journalTransactionTypeIds.Count > 0, u => u.JournalTransactionTypeId != null && journalTransactionTypeIds.Contains(u.JournalTransactionTypeId.Value))
                                                    .WhereIf(inventoryTypes != null && inventoryTypes.Count > 0, s => s.JournalTransactionType != null &&
                                                             ((inventoryTypes.Contains((long)InventoryType.ItemIssueSale) && s.JournalTransactionType.IsIssue) ||
                                                             (inventoryTypes.Contains((long)InventoryType.ItemReceiptPurchase) && !s.JournalTransactionType.IsIssue)))
                                                    .WhereIf(toDate != null,
                                                        (u => (u.Date.Date) <= (toDate.Value.Date)))
                                                    .AsNoTracking()
                                        on Ir.ItemReceiptId equals j.ItemReceiptId
                                        select new ItemBalanceDto
                                        {
                                            Date = j.Date,
                                            IsPurchase = true,
                                            ItemId = Ir.ItemId,
                                            Qty = Ir.Qty,
                                            LotId = Ir.LotId == null ? 0 : Ir.LotId.Value,
                                            LotName = Ir.Lot == null ? "" : Ir.Lot.LotName,
                                            LocationId = Ir.Lot == null ? 0 : Ir.Lot.LocationId,
                                            LocationName = Ir.Lot == null ? "" : Ir.Lot.Location.LocationName,
                                        });

            var itemReceiptCustomerCreditItemQuery = (from IrC in _itemReceiptCustomerCreditItemRepository.GetAll()
                                                                .WhereIf(locations != null && locations.Count > 0, t => t.Lot != null && locations.Contains(t.Lot.LocationId))
                                                                .WhereIf(lots != null && lots.Count > 0, t => lots.Contains(t.LotId))
                                                                .AsNoTracking()
                                                      join J in _journalRepository.GetAll()
                                                              .Where(s => s.ItemReceiptCustomerCreditId.HasValue)
                                                              .WhereIf(journalTransactionTypeIds != null && journalTransactionTypeIds.Count > 0, u => u.JournalTransactionTypeId != null && journalTransactionTypeIds.Contains(u.JournalTransactionTypeId.Value))
                                                              .WhereIf(inventoryTypes != null && inventoryTypes.Count > 0, s => s.JournalTransactionType != null &&
                                                                         ((inventoryTypes.Contains((long)InventoryType.ItemIssueSale) && s.JournalTransactionType.IsIssue) ||
                                                                         (inventoryTypes.Contains((long)InventoryType.ItemReceiptPurchase) && !s.JournalTransactionType.IsIssue)))
                                                              .Where(s => s.Status == TransactionStatus.Publish)
                                                              .Where(t => fromDate == null || t.Date.Date > fromDate.Value)

                                                              .WhereIf(toDate != null,
                                                                  (u => (u.Date.Date) <= (toDate.Value.Date)))
                                                              .AsNoTracking()
                                                      on IrC.ItemReceiptCustomerCreditId equals J.ItemReceiptCustomerCreditId
                                                      select new ItemBalanceDto
                                                      {
                                                          Date = J.Date,
                                                          IsPurchase = true,
                                                          ItemId = IrC.ItemId,
                                                          Qty = IrC.Qty,
                                                          LotId = IrC.LotId == null ? 0 : IrC.LotId.Value,
                                                          LotName = IrC.Lot == null ? "" : IrC.Lot.LotName,
                                                          LocationId = IrC.Lot == null ? 0 : IrC.Lot.LocationId,
                                                          LocationName = IrC.Lot == null ? "" : IrC.Lot.Location.LocationName,
                                                      });

            var itemReceiptResult = itemReceiptItemQuery.Concat(itemReceiptCustomerCreditItemQuery);

            var itemIssueItemQuery = (from isue in _itemIssueItemRepository.GetAll()
                                                    .WhereIf(locations != null && locations.Count > 0, t => t.Lot != null && locations.Contains(t.Lot.LocationId))
                                                    .WhereIf(lots != null && lots.Count > 0, t => lots.Contains(t.LotId))
                                                    .AsNoTracking()
                                      join j in _journalRepository.GetAll()
                                              .Where(s => s.ItemIssueId.HasValue)
                                              .WhereIf(journalTransactionTypeIds != null && journalTransactionTypeIds.Count > 0, u => u.JournalTransactionTypeId != null && journalTransactionTypeIds.Contains(u.JournalTransactionTypeId.Value))
                                             .WhereIf(inventoryTypes != null && inventoryTypes.Count > 0, s => s.JournalTransactionType != null &&
                                                             ((inventoryTypes.Contains((long)InventoryType.ItemIssueSale) && s.JournalTransactionType.IsIssue) ||
                                                             (inventoryTypes.Contains((long)InventoryType.ItemReceiptPurchase) && !s.JournalTransactionType.IsIssue)))
                                              .Where(s => s.Status == TransactionStatus.Publish)
                                              .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                              .WhereIf(toDate != null,
                                                  (u => (u.Date.Date) <= (toDate.Value.Date)))
                                              .AsNoTracking()
                                      on isue.ItemIssueId equals j.ItemIssueId
                                      select new ItemBalanceDto
                                      {
                                          Date = j.Date,
                                          IsPurchase = false,
                                          ItemId = isue.ItemId,
                                          Qty = (isue.Qty * -1),
                                          LotId = isue.LotId == null ? 0 : isue.LotId.Value,
                                          LotName = isue.Lot == null ? "" : isue.Lot.LotName,
                                          LocationId = isue.Lot == null ? 0 : isue.Lot.LocationId,
                                          LocationName = isue.Lot == null ? "" : isue.Lot.Location.LocationName,
                                      });

            var itemIssueVendorCreditItemQuery = (from isv in _itemIssueVendorCreditItemRepository.GetAll()
                                                            .WhereIf(locations != null && locations.Count > 0, t => t.Lot != null && locations.Contains(t.Lot.LocationId))
                                                            .WhereIf(lots != null && lots.Count > 0, t => lots.Contains(t.LotId))
                                                            .AsNoTracking()
                                                  join j in _journalRepository.GetAll()
                                                          .Where(s => s.ItemIssueVendorCreditId.HasValue)
                                                          .Where(s => s.Status == TransactionStatus.Publish)
                                                          .WhereIf(journalTransactionTypeIds != null && journalTransactionTypeIds.Count > 0, u => u.JournalTransactionTypeId != null && journalTransactionTypeIds.Contains(u.JournalTransactionTypeId.Value))
                                                          .WhereIf(inventoryTypes != null && inventoryTypes.Count > 0, s => s.JournalTransactionType != null &&
                                                             ((inventoryTypes.Contains((long)InventoryType.ItemIssueSale) && s.JournalTransactionType.IsIssue) ||
                                                             (inventoryTypes.Contains((long)InventoryType.ItemReceiptPurchase) && !s.JournalTransactionType.IsIssue)))
                                                          .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                                          .WhereIf(toDate != null,
                                                          (u => (u.Date.Date) <= (toDate.Value.Date)))
                                                          .AsNoTracking()
                                                  on isv.ItemIssueVendorCreditId equals j.ItemIssueVendorCreditId
                                                  select new ItemBalanceDto
                                                  {
                                                      Date = j.Date,
                                                      IsPurchase = false,
                                                      ItemId = isv.ItemId,
                                                      Qty = (isv.Qty * -1),
                                                      LotId = isv.LotId == null ? 0 : isv.LotId.Value,
                                                      LotName = isv.Lot == null ? "" : isv.Lot.LotName,
                                                      LocationId = isv.Lot == null ? 0 : isv.Lot.LocationId,
                                                      LocationName = isv.Lot == null ? "" : isv.Lot.Location.LocationName,
                                                  });
            var itemIssueResult = itemIssueItemQuery.Concat(itemIssueVendorCreditItemQuery);

            var inventoryTransactionItemQuery = itemReceiptResult.Concat(itemIssueResult);

            return inventoryTransactionItemQuery;
        }


        public async Task<GetAvgCostForIssueOutput> GetAvgCostForIssues(DateTime toDate,
                                                                        List<long?> locationIds,
                                                                        List<GetAvgCostForIssueData> input
            //,Journal goodIssueJournalEntity
            //,Guid? roundingAccountId
            )
        {

            var result = new GetAvgCostForIssueOutput()
            {
                Total = 0,
                Items = new List<GetAvgCostForIssueData>(),
                RoundingItems = new List<RoundingAdjustmentItemOutput>()
            };


            if (input == null || input.Count == 0) return result;

            var groupByItemLot = input.OrderBy(u => u.Index)
                                   .GroupBy(u => new { u.ItemId, u.ItemCode, u.LotId, u.ItemName });

            var itemLotIdDict = groupByItemLot.ToDictionary(u => $"{u.Key.ItemId}-{u.Key.LotId}", u => $"{u.Key.ItemId}-{u.Key.LotId}");
            //var updatedIssueItemIdDict = input.Where(u => u.ItemIssueItemId != null ).ToDictionary(u => u.ItemIssueItemId.Value, u => u.ItemIssueItemId.Value);

            var updatedIssueItemIdDict = input.Where(u => u.ItemIssueItemId != null).Select(s => s.ItemIssueItemId).Distinct().ToDictionary(u => u.Value, u => u.Value);
            var updatedReceiptItemIdDict = input.Where(u => u.ItemReceiptItemId != null).Select(s => s.ItemReceiptItemId).Distinct().ToDictionary(u => u.Value, u => u.Value);

            if (updatedIssueItemIdDict != null && updatedReceiptItemIdDict != null && updatedIssueItemIdDict.Any() && updatedReceiptItemIdDict.Any())
            {
                updatedIssueItemIdDict.Add(updatedReceiptItemIdDict.First().Key, updatedReceiptItemIdDict.First().Value);
            }


            var itemIdToSelectDict = groupByItemLot.GroupBy(u => u.Key.ItemId).ToDictionary(u => u.Key, u => u.Key);
            var averageCosts = await GetItemsByAverageCost(
                                   toDate,
                                   locationIds,
                                   new List<long?>(),
                                   updatedIssueItemIdDict,
                                   itemIdToSelectDict
                               );

            if (averageCosts == null || averageCosts.Count == 0)
                throw new UserFriendlyException(L("CannotIssueForAllOfTheseOutOfStockItems"));

            var averageCostDictByItemAndLot = averageCosts.ToDictionary(u => $"{u.Id}-{u.LotId}");


            var dicLocationQtyBalance = new Dictionary<string, decimal>();
            var dicLocationCostBalance = new Dictionary<string, decimal>();
            var dicLocationAVGCost = new Dictionary<string, decimal>();


            foreach (var item in groupByItemLot)
            {
                var key = $"{item.Key.ItemId}-{item.Key.LotId}";


                if (!averageCostDictByItemAndLot.ContainsKey(key))
                {
                    throw new UserFriendlyException(L("CannotIssueForThisOutOfStockItem", item.Key.ItemCode + " - " + item.Key.ItemName));
                }

                var avgCostItem = averageCostDictByItemAndLot[key];
                //var averageCost = avgCostItem.AverageCost;


                //var totalCostByItemAndLot = avgCostItem.TotalCost;
                var totalQtyByItemAndLot = avgCostItem.QtyOnHand;

                var costKey = $"{avgCostItem.Id}-{avgCostItem.LocationId}";


                if (!dicLocationCostBalance.ContainsKey(costKey))
                {
                    dicLocationCostBalance.Add(costKey, avgCostItem.TotalLocationCost);
                    dicLocationQtyBalance.Add(costKey, avgCostItem.TotalLocationQty);
                    dicLocationAVGCost.Add(costKey, avgCostItem.AverageCost);
                }




                var roundingDigit = avgCostItem.RoundingDigit;
                var roundingDigitUnitCost = avgCostItem.RoundingDigitUnitCost;

                foreach (var s in item)
                {

                    if (totalQtyByItemAndLot < s.Qty)
                    {
                        throw new UserFriendlyException(L("CannotIssueForThisBiggerStockItem",
                            item.Key.ItemCode + " - " + item.Key.ItemName, String.Format("{0:n0}", totalQtyByItemAndLot),
                            String.Format("{0:n0}", s.Qty)));
                    }

                    //s.UnitCost = averageCost;
                    s.UnitCost = dicLocationAVGCost[costKey];
                    s.LineCost = Math.Round(s.UnitCost * s.Qty, roundingDigit);

                    totalQtyByItemAndLot -= s.Qty;
                    dicLocationQtyBalance[costKey] -= s.Qty;
                    //adjust lineCost when Qty == 0
                    if (dicLocationQtyBalance[costKey] == 0 && s.LineCost + dicLocationCostBalance[costKey] != 0)
                    {
                        s.LineCost = dicLocationCostBalance[costKey];
                    }

                    result.Total += s.LineCost;
                    result.Items.Add(s);

                    dicLocationCostBalance[costKey] = Math.Round(dicLocationCostBalance[costKey] - s.LineCost, roundingDigit);
                    dicLocationAVGCost[costKey] = dicLocationQtyBalance[costKey] == 0 ? 0 :
                                  Math.Round(dicLocationCostBalance[costKey] / dicLocationQtyBalance[costKey], roundingDigitUnitCost);

                }



            }



            return result;

        }


        public async Task<IdentityResult> AutoCreateRoundingJournal(Journal goodIssueJournalEntity,
                                                                    List<RoundingAdjustmentItemOutput> roundingItems,
                                                                    Guid? roundingAccountId,
                                                                    bool remove = true,
                                                                    bool add = true)
        {
            #region Auto create rounding journal


            //delete old journal
            //if (goodIssueJournalEntity.RoundedAdjustmentJournalId != null && remove)
            //{

            //    var roundedGeneralJournalAdjustmentId = goodIssueJournalEntity.RoundedAdjustmentJournalId.Value;
            //    goodIssueJournalEntity.UpdateRoundedAdjustmentJournal(null);

            //    var @entity = await _journalRepository.GetAll()
            //                    .Where(t => t.GeneralJournalId == roundedGeneralJournalAdjustmentId)
            //                    .FirstOrDefaultAsync();
            //    //_journalManager.GetAsync(goodIssueJournalEntity.Id, true);

            //    if (entity == null)
            //    {
            //        throw new UserFriendlyException(L("RecordNotFound"));
            //    }

            //    var JournalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == entity.Id).ToListAsync();

            //    foreach (var s in JournalItems)
            //    {
            //        var result1 = await _journalItemManager.RemoveAsync(s);
            //        if (!result1.Succeeded) return result1;
            //    }


            //    var result2 = await _journalManager.RemoveAsync(@entity);
            //    if (!result2.Succeeded) return result2;

            //    await CurrentUnitOfWork.SaveChangesAsync();

            //}



            if (roundingItems != null && roundingItems.Count > 0 && add) //new to round
            {
                if (roundingAccountId == null)
                {
                    return IdentityResult.Failed(new IdentityError()
                    {
                        //Todo: Translate
                        Description = L("MustFillInRoundingAdjustmentAccount")
                    });
                }


                var roundingTotal = Math.Abs(roundingItems.Sum(u => u.AdjustmentBalance));

                var @roundingJournal = Journal.Create(goodIssueJournalEntity.TenantId,
                                                     goodIssueJournalEntity.CreatorUserId.Value, goodIssueJournalEntity.JournalNo,
                                                     goodIssueJournalEntity.Date, "", roundingTotal, roundingTotal,
                                                     goodIssueJournalEntity.CurrencyId, goodIssueJournalEntity.ClassId, goodIssueJournalEntity.Reference, goodIssueJournalEntity.LocationId);

                @roundingJournal.UpdateStatus(TransactionStatus.Publish);
                @roundingJournal.UpdateGeneralJournal(@roundingJournal);

                // goodIssueJournalEntity.UpdateRoundedAdjustmentJournal(@roundingJournal);

                ////link
                //goodIssueJournalEntity.UpdateRoundedAdjustmentJournal(@roundingJournal);

                #region journalItems           
                foreach (var j in roundingItems)
                {

                    var abBalanceTemp = Math.Abs(j.AdjustmentBalance);

                    var inventoryDebitAmount = j.AdjustmentBalance < 0 ? abBalanceTemp : 0;
                    var inventoryCreditAmount = j.AdjustmentBalance < 0 ? 0 : abBalanceTemp;

                    var @inventoryJournalItem = JournalItem.CreateJournalItem(goodIssueJournalEntity.TenantId, goodIssueJournalEntity.CreatorUserId.Value, @roundingJournal, j.InventoryAccountId, "", inventoryDebitAmount, inventoryCreditAmount, PostingKey.None, null);
                    var result = await _journalItemManager.CreateAsync(@inventoryJournalItem);
                    if (!result.Succeeded) return result;


                    var roundingDebitAmount = j.AdjustmentBalance < 0 ? 0 : abBalanceTemp;
                    var roundingCreditAmount = j.AdjustmentBalance < 0 ? abBalanceTemp : 0;


                    var @roundingJournalItem = JournalItem.CreateJournalItem(goodIssueJournalEntity.TenantId, goodIssueJournalEntity.CreatorUserId.Value, @roundingJournal, roundingAccountId.Value, "", roundingDebitAmount, roundingCreditAmount, PostingKey.None, null);
                    result = await _journalItemManager.CreateAsync(@roundingJournalItem);
                    if (!result.Succeeded) return result;


                }


                #endregion

                await _journalManager.CreateAsync(@roundingJournal, true);
                await CurrentUnitOfWork.SaveChangesAsync();

                return IdentityResult.Success;
                //inventory account
                //rounding account -> setting
            }

            return IdentityResult.Success;


            #endregion
        }

        public IQueryable<ItemDto> GetInventoryValuationDetailQuery(
                                   DateTime? fromDate,
                                   DateTime? toDate,
                                   List<long?> locations,
                                   List<Guid> itemsToSelect = null,
                                   long? filterByUserId = null)
        {

            var previousCycle = GetPreviousCloseCyleAsync(toDate ?? DateTime.Now);
            var currentCycle = GetCyleAsync(toDate ?? DateTime.Now);
            var roundingDigit = previousCycle != null ? previousCycle.RoundingDigit : currentCycle != null ? currentCycle.RoundingDigit : 2;
            var roundingDigitUnitCost = previousCycle != null ? previousCycle.RoundingDigitUnitCost : currentCycle != null ? currentCycle.RoundingDigitUnitCost : 2;


            //var itemReceiptItemQuery = (from Ir in _itemReceiptItemRepository.GetAll()
            //                                .WhereIf(itemsToSelect != null && itemsToSelect.Count > 0, t => itemsToSelect.Contains(t.ItemId))
            //                                .AsNoTracking()

            //                            join ji in _journalItemRepository.GetAll().Where(s => s.Identifier != null && s.Key == PostingKey.Inventory)
            //                            .AsNoTracking()
            //                            on Ir.Id equals ji.Identifier

            //                            join j in _journalRepository.GetAll()
            //                            .Where(s => s.ItemReceiptId.HasValue)
            //                            .Where(s => s.Status == TransactionStatus.Publish)
            //                            .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
            //                            .WhereIf(toDate != null, u => u.Date.Date <= toDate.Value.Date)
            //                            .AsNoTracking()
            //                            on Ir.ItemReceiptId equals j.ItemReceiptId
            //                            select new ItemDto
            //                            {
            //                                JournalReference = j.Reference,
            //                                CreationTime = Ir.CreationTime,
            //                                TransferOrderItemId = Ir.TransferOrderItemId,
            //                                IsPurchase = true,
            //                                ItemId = Ir.ItemId,
            //                                TransactionId = Ir.ItemReceiptId,
            //                                TotalCost = Math.Round(Ir.Total, roundingDigit),
            //                                Qty = Ir.Qty,
            //                                LocationId = Ir.Lot.LocationId,
            //                                LocationName = Ir.Lot.Location.LocationName,
            //                                JournalDate = j.Date,
            //                                JournalType = j.JournalType,
            //                                UnitCost = Ir.UnitCost,
            //                                TransactionItemId = Ir.Id,
            //                                JournalNo = j.JournalNo,
            //                                JournalId = j.Id,
            //                                CreationTimeIndex = j.CreationTimeIndex,
            //                                OrderId = Ir.ItemReceipt.TransferOrderId != null ? Ir.ItemReceipt.TransferOrderId : Ir.ItemReceipt.ProductionOrderId,
            //                                RoundingDigit = roundingDigit,
            //                                RoundingDigitUnitCost = roundingDigitUnitCost,
            //                                InventoryAccountId = ji.AccountId,
            //                                Description = Ir.Description
            //                            });

            //var itemReceiptCustomerCreditItemQuery = (from IrC in _itemReceiptCustomerCreditItemRepository.GetAll()
            //                                            .WhereIf(itemsToSelect != null && itemsToSelect.Count > 0, t => itemsToSelect.Contains(t.ItemId))
            //                                            .AsNoTracking()

            //                                          join ji in _journalItemRepository.GetAll().Where(s => s.Identifier != null && s.Key == PostingKey.Inventory)
            //                                          .AsNoTracking()
            //                                          on IrC.Id equals ji.Identifier

            //                                          join J in _journalRepository.GetAll()
            //                                          .Where(s => s.ItemReceiptCustomerCreditId.HasValue)
            //                                          .Where(s => s.Status == TransactionStatus.Publish)
            //                                          .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
            //                                          .WhereIf(toDate != null,
            //                                              (u => (u.Date.Date) <= (toDate.Value.Date)))
            //                                          .AsNoTracking()
            //                                          on IrC.ItemReceiptCustomerCreditId equals J.ItemReceiptCustomerCreditId
            //                                          select new ItemDto
            //                                          {
            //                                              PurchaseCost = IrC.Item.PurchaseCost ?? 0,
            //                                              JournalReference = J.Reference,
            //                                              CreationTime = IrC.CreationTime,
            //                                              TransferOrderItemId = null,
            //                                              IsPurchase = true,
            //                                              ItemId = IrC.ItemId,
            //                                              TransactionId = IrC.ItemReceiptCustomerCreditId,
            //                                              JournalType = J.JournalType,
            //                                              UnitCost = IrC.UnitCost,
            //                                              TransactionItemId = IrC.Id,
            //                                              TotalCost = Math.Round(IrC.Total, roundingDigit),
            //                                              Qty = IrC.Qty,
            //                                              JournalDate = J.Date,
            //                                              LocationId = IrC.Lot.LocationId,
            //                                              LocationName = IrC.Lot.Location.LocationName,
            //                                              JournalNo = J.JournalNo,
            //                                              JournalId = J.Id,
            //                                              CreationTimeIndex = J.CreationTimeIndex,
            //                                              OrderId = null,
            //                                              RoundingDigit = roundingDigit,
            //                                              RoundingDigitUnitCost = roundingDigitUnitCost,
            //                                              InventoryAccountId = ji.AccountId,
            //                                              Description = IrC.Description
            //                                          });

            //var itemReceiptResult = itemReceiptItemQuery.Concat(itemReceiptCustomerCreditItemQuery);

            //var itemIssueItemQuery = (from isue in _itemIssueItemRepository.GetAll()
            //                            .WhereIf(itemsToSelect != null && itemsToSelect.Count > 0, t => itemsToSelect.Contains(t.ItemId))
            //                            .AsNoTracking()

            //                          join j in _journalRepository.GetAll()
            //                          .Where(s => s.ItemIssueId.HasValue)
            //                          .Where(s => s.Status == TransactionStatus.Publish)
            //                          .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
            //                          .WhereIf(toDate != null,
            //                              (u => (u.Date.Date) <= (toDate.Value.Date)))
            //                          .AsNoTracking()
            //                          on isue.ItemIssueId equals j.ItemIssueId
            //                          select new ItemDto
            //                          {
            //                              JournalReference = j.Reference,
            //                              CreationTime = isue.CreationTime,
            //                              TransferOrderItemId = isue.TransferOrderItemId,
            //                              IsPurchase = false,
            //                              ItemId = isue.ItemId,
            //                              TransactionId = isue.ItemIssueId,
            //                              TotalCost = (isue.Total * -1),
            //                              Qty = (isue.Qty * -1),
            //                              LocationId = isue.Lot.LocationId,
            //                              LocationName = isue.Lot.Location.LocationName,
            //                              JournalType = j.JournalType,
            //                              JournalDate = j.Date,
            //                              UnitCost = isue.UnitCost,
            //                              TransactionItemId = isue.Id,
            //                              JournalNo = j.JournalNo,
            //                              JournalId = j.Id,
            //                              CreationTimeIndex = j.CreationTimeIndex,
            //                              OrderId = isue.ItemIssue.TransferOrderId != null ? isue.ItemIssue.TransferOrderId : isue.ItemIssue.ProductionOrderId,
            //                              RoundingDigit = roundingDigit,
            //                              RoundingDigitUnitCost = roundingDigitUnitCost,
            //                              Description = isue.Description
            //                          });

            //var itemIssueVendorCreditItemQuery = (from isv in _itemIssueVendorCreditItemRepository.GetAll()
            //                                        .WhereIf(itemsToSelect != null && itemsToSelect.Count > 0, t => itemsToSelect.Contains(t.ItemId))
            //                                        .AsNoTracking()
            //                                      join j in _journalRepository.GetAll()
            //                                      .Where(s => s.ItemIssueVendorCreditId.HasValue)
            //                                      .Where(s => s.Status == TransactionStatus.Publish)
            //                                      .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
            //                                      .WhereIf(toDate != null,
            //                                      (u => (u.Date.Date) <= (toDate.Value.Date)))
            //                                      .AsNoTracking()
            //                                      on isv.ItemIssueVendorCreditId equals j.ItemIssueVendorCreditId
            //                                      select new ItemDto
            //                                      {
            //                                          JournalReference = j.Reference,
            //                                          CreationTime = isv.CreationTime,
            //                                          TransferOrderItemId = null,
            //                                          IsPurchase = false,
            //                                          ItemId = isv.ItemId,
            //                                          TransactionId = isv.ItemIssueVendorCreditId,
            //                                          TotalCost = (isv.Total * -1),
            //                                          Qty = (isv.Qty * -1),
            //                                          LocationId = isv.Lot.LocationId,
            //                                          LocationName = isv.Lot.Location.LocationName,
            //                                          JournalType = j.JournalType,
            //                                          JournalDate = j.Date,
            //                                          UnitCost = isv.UnitCost,
            //                                          TransactionItemId = isv.Id,
            //                                          JournalNo = j.JournalNo,
            //                                          OrderId = null,
            //                                          JournalId = j.Id,
            //                                          CreationTimeIndex = j.CreationTimeIndex,
            //                                          RoundingDigit = roundingDigit,
            //                                          RoundingDigitUnitCost = roundingDigitUnitCost,
            //                                          Description = isv.Description

            //                                      });
            //var itemIssueResult = itemIssueItemQuery.Concat(itemIssueVendorCreditItemQuery);

            //var inventoryTransactionItemQuery = itemReceiptResult.Concat(itemIssueResult);

            var inventoryTransactionItemQuery = from i in _inventoryTransactionItemRepository.GetAll()
                                                        .Where(t => itemsToSelect == null || itemsToSelect.Count == 0 || itemsToSelect.Contains(t.ItemId))
                                                        .Where(t => locations == null || !locations.Any() || locations.Contains(t.LocationId))
                                                        .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                                        .Where(t => toDate == null || t.Date.Date <= toDate.Value.Date)
                                                        .AsNoTracking()
                                                join l in _locationRepository.GetAll()
                                                          .Where(t => locations == null || !locations.Any() || locations.Contains(t.Id))
                                                          .AsNoTracking()
                                                on i.LocationId equals l.Id

                                                join p in _productionRepository.GetAll().AsNoTracking()
                                                on i.TransferOrProductionId equals p.Id
                                                into ps
                                                from p in ps.DefaultIfEmpty()

                                                join t in _transferRepository.GetAll().AsNoTracking()
                                                on i.TransferOrProductionId equals t.Id
                                                into ts
                                                from t in ts.DefaultIfEmpty()

                                                select new ItemDto
                                                {
                                                    JournalId = i.JournalId,
                                                    JournalNo = i.JournalNo,
                                                    JournalReference = i.JournalRef,
                                                    CreationTimeIndex = i.CreationTimeIndex,
                                                    TransferOrderItemId = i.TransferOrProductionItemId,
                                                    IsPurchase = i.IsItemReceipt,
                                                    ItemId = i.ItemId,
                                                    TransactionId = i.TransactionId,
                                                    TotalCost = i.LineCost,
                                                    Qty = i.Qty,
                                                    LocationId = i.LocationId,
                                                    LocationName = l.LocationName,
                                                    JournalType = i.JournalType,
                                                    JournalDate = i.Date,
                                                    UnitCost = i.UnitCost,
                                                    TransactionItemId = i.Id,
                                                    OrderId = i.TransferOrProductionId,
                                                    RoundingDigit = roundingDigit,
                                                    RoundingDigitUnitCost = roundingDigitUnitCost,
                                                    Description = i.Description,
                                                    InventoryAccountId = i.InventoryAccountId,
                                                    OrderIndex = i.OrderIndex,
                                                    TransactionNo = p != null ? p.ProductionNo : t != null ? t.TransferNo : ""
                                                };


            if (filterByUserId != null)
            {
                // get user by group member
                var locationGroups = GetLocationGroup(filterByUserId.Value);

                var itemGroup = this.GetItemByUserGroup(filterByUserId.Value);
                inventoryTransactionItemQuery = from u in inventoryTransactionItemQuery
                                                join g in locationGroups on u.LocationId equals g
                                                join i in itemGroup on u.ItemId equals i
                                                select u;
            }

            return inventoryTransactionItemQuery;
        }


        public IQueryable<ItemDto> GetInventoryValuationDetailQuery_Backup(
                                    DateTime? fromDate,
                                    DateTime? toDate,
                                    List<long?> Locations,
                                    List<Guid> itemsToSelect = null,
                                    long? filterByUserId = null)
        {

            var previousCycle = GetPreviousCloseCyleAsync(toDate ?? DateTime.Now);
            var currentDigit = GetCyleAsync(toDate ?? DateTime.Now);
            var roundDigit = currentDigit != null ? currentDigit.RoundingDigit : 2;
            var roundDigitUnitCost = currentDigit != null ? currentDigit.RoundingDigitUnitCost : 2;
            var roundingDigit = previousCycle != null ? previousCycle.RoundingDigit : roundDigit;
            var roundingDigitUnitCost = previousCycle != null ? previousCycle.RoundingDigitUnitCost : roundDigitUnitCost;


            var itemReceiptItemQuery = (from Ir in _itemReceiptItemRepository.GetAll()
                                            .WhereIf(itemsToSelect != null && itemsToSelect.Count > 0, t => itemsToSelect.Contains(t.ItemId))
                                            .AsNoTracking()

                                        join ji in _journalItemRepository.GetAll().Where(s => s.Identifier != null && s.Key == PostingKey.Inventory)
                                        .AsNoTracking()
                                        on Ir.Id equals ji.Identifier

                                        join j in _journalRepository.GetAll()
                                        .Where(s => s.ItemReceiptId.HasValue)
                                        .Where(s => s.Status == TransactionStatus.Publish)
                                        .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                        .WhereIf(toDate != null, u => u.Date.Date <= toDate.Value.Date)
                                        .AsNoTracking()
                                        on Ir.ItemReceiptId equals j.ItemReceiptId
                                        select new ItemDto
                                        {
                                            JournalReference = j.Reference,
                                            CreationTime = Ir.CreationTime,
                                            TransferOrderItemId = Ir.TransferOrderItemId,
                                            IsPurchase = true,
                                            ItemId = Ir.ItemId,
                                            TransactionId = Ir.ItemReceiptId,
                                            TotalCost = Math.Round(Ir.Total, roundingDigit),
                                            Qty = Ir.Qty,
                                            LocationId = Ir.Lot.LocationId,
                                            LocationName = Ir.Lot.Location.LocationName,
                                            JournalDate = j.Date,
                                            JournalType = j.JournalType,
                                            UnitCost = Ir.UnitCost,
                                            TransactionItemId = Ir.Id,
                                            JournalNo = j.JournalNo,
                                            JournalId = j.Id,
                                            CreationTimeIndex = j.CreationTimeIndex,
                                            OrderId = Ir.ItemReceipt.TransferOrderId != null ? Ir.ItemReceipt.TransferOrderId : Ir.ItemReceipt.ProductionOrderId,
                                            RoundingDigit = roundingDigit,
                                            RoundingDigitUnitCost = roundingDigitUnitCost,
                                            InventoryAccountId = ji.AccountId,
                                            Description = Ir.Description
                                        });

            var itemReceiptCustomerCreditItemQuery = (from IrC in _itemReceiptCustomerCreditItemRepository.GetAll()
                                                        .WhereIf(itemsToSelect != null && itemsToSelect.Count > 0, t => itemsToSelect.Contains(t.ItemId))
                                                        .AsNoTracking()

                                                      join ji in _journalItemRepository.GetAll().Where(s => s.Identifier != null && s.Key == PostingKey.Inventory)
                                                      .AsNoTracking()
                                                      on IrC.Id equals ji.Identifier

                                                      join J in _journalRepository.GetAll()
                                                      .Where(s => s.ItemReceiptCustomerCreditId.HasValue)
                                                      .Where(s => s.Status == TransactionStatus.Publish)
                                                      .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                                      .WhereIf(toDate != null,
                                                          (u => (u.Date.Date) <= (toDate.Value.Date)))
                                                      .AsNoTracking()
                                                      on IrC.ItemReceiptCustomerCreditId equals J.ItemReceiptCustomerCreditId
                                                      select new ItemDto
                                                      {
                                                          PurchaseCost = IrC.Item.PurchaseCost ?? 0,
                                                          JournalReference = J.Reference,
                                                          CreationTime = IrC.CreationTime,
                                                          TransferOrderItemId = null,
                                                          IsPurchase = true,
                                                          ItemId = IrC.ItemId,
                                                          TransactionId = IrC.ItemReceiptCustomerCreditId,
                                                          JournalType = J.JournalType,
                                                          UnitCost = IrC.UnitCost,
                                                          TransactionItemId = IrC.Id,
                                                          TotalCost = Math.Round(IrC.Total, roundingDigit),
                                                          Qty = IrC.Qty,
                                                          JournalDate = J.Date,
                                                          LocationId = IrC.Lot.LocationId,
                                                          LocationName = IrC.Lot.Location.LocationName,
                                                          JournalNo = J.JournalNo,
                                                          JournalId = J.Id,
                                                          CreationTimeIndex = J.CreationTimeIndex,
                                                          OrderId = null,
                                                          RoundingDigit = roundingDigit,
                                                          RoundingDigitUnitCost = roundingDigitUnitCost,
                                                          InventoryAccountId = ji.AccountId,
                                                          Description = IrC.Description
                                                      });

            var itemReceiptResult = itemReceiptItemQuery.Concat(itemReceiptCustomerCreditItemQuery);

            var itemIssueItemQuery = (from isue in _itemIssueItemRepository.GetAll()
                                        .WhereIf(itemsToSelect != null && itemsToSelect.Count > 0, t => itemsToSelect.Contains(t.ItemId))
                                        .AsNoTracking()

                                      join j in _journalRepository.GetAll()
                                      .Where(s => s.ItemIssueId.HasValue)
                                      .Where(s => s.Status == TransactionStatus.Publish)
                                      .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                      .WhereIf(toDate != null,
                                          (u => (u.Date.Date) <= (toDate.Value.Date)))
                                      .AsNoTracking()
                                      on isue.ItemIssueId equals j.ItemIssueId
                                      select new ItemDto
                                      {
                                          JournalReference = j.Reference,
                                          CreationTime = isue.CreationTime,
                                          TransferOrderItemId = isue.TransferOrderItemId,
                                          IsPurchase = false,
                                          ItemId = isue.ItemId,
                                          TransactionId = isue.ItemIssueId,
                                          TotalCost = (isue.Total * -1),
                                          Qty = (isue.Qty * -1),
                                          LocationId = isue.Lot.LocationId,
                                          LocationName = isue.Lot.Location.LocationName,
                                          JournalType = j.JournalType,
                                          JournalDate = j.Date,
                                          UnitCost = isue.UnitCost,
                                          TransactionItemId = isue.Id,
                                          JournalNo = j.JournalNo,
                                          JournalId = j.Id,
                                          CreationTimeIndex = j.CreationTimeIndex,
                                          OrderId = isue.ItemIssue.TransferOrderId != null ? isue.ItemIssue.TransferOrderId : isue.ItemIssue.ProductionOrderId,
                                          RoundingDigit = roundingDigit,
                                          RoundingDigitUnitCost = roundingDigitUnitCost,
                                          Description = isue.Description
                                      });

            var itemIssueVendorCreditItemQuery = (from isv in _itemIssueVendorCreditItemRepository.GetAll()
                                                    .WhereIf(itemsToSelect != null && itemsToSelect.Count > 0, t => itemsToSelect.Contains(t.ItemId))
                                                    .AsNoTracking()
                                                  join j in _journalRepository.GetAll()
                                                  .Where(s => s.ItemIssueVendorCreditId.HasValue)
                                                  .Where(s => s.Status == TransactionStatus.Publish)
                                                  .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                                  .WhereIf(toDate != null,
                                                  (u => (u.Date.Date) <= (toDate.Value.Date)))
                                                  .AsNoTracking()
                                                  on isv.ItemIssueVendorCreditId equals j.ItemIssueVendorCreditId
                                                  select new ItemDto
                                                  {
                                                      JournalReference = j.Reference,
                                                      CreationTime = isv.CreationTime,
                                                      TransferOrderItemId = null,
                                                      IsPurchase = false,
                                                      ItemId = isv.ItemId,
                                                      TransactionId = isv.ItemIssueVendorCreditId,
                                                      TotalCost = (isv.Total * -1),
                                                      Qty = (isv.Qty * -1),
                                                      LocationId = isv.Lot.LocationId,
                                                      LocationName = isv.Lot.Location.LocationName,
                                                      JournalType = j.JournalType,
                                                      JournalDate = j.Date,
                                                      UnitCost = isv.UnitCost,
                                                      TransactionItemId = isv.Id,
                                                      JournalNo = j.JournalNo,
                                                      OrderId = null,
                                                      JournalId = j.Id,
                                                      CreationTimeIndex = j.CreationTimeIndex,
                                                      RoundingDigit = roundingDigit,
                                                      RoundingDigitUnitCost = roundingDigitUnitCost,
                                                      Description = isv.Description

                                                  });
            var itemIssueResult = itemIssueItemQuery.Concat(itemIssueVendorCreditItemQuery);

            var inventoryTransactionItemQuery = itemReceiptResult.Concat(itemIssueResult);


            if (filterByUserId != null)
            {
                // get user by group member
                var locationGroups = GetLocationGroup(filterByUserId.Value);

                var itemGroup = this.GetItemByUserGroup(filterByUserId.Value);
                inventoryTransactionItemQuery = from u in inventoryTransactionItemQuery
                                                join g in locationGroups on u.LocationId equals g
                                                join i in itemGroup on u.ItemId equals i
                                                select u;
            }

            return inventoryTransactionItemQuery;
        }


        public IQueryable<ItemDto> GetInventoryValuationDetailQuery(
                              DateTime toDate,
                              List<long?> Locations,
                              List<Guid> items = null,
                              long? filterByUserId = null)
        {
            var previousCycle = GetPreviousCloseCyleAsync(toDate);
            var currentDigit = GetCyleAsync(toDate);
            var roundDigit = currentDigit != null ? currentDigit.RoundingDigit : 2;
            var roundingDigit = previousCycle != null ? previousCycle.RoundingDigit : roundDigit;
            var roundingDigitUnitCost = previousCycle != null ? previousCycle.RoundingDigitUnitCost : roundDigit;

            var inventoryCostClose = _inventoryCostCloseItemRepository.GetAll()
                                   //.Where(t => previousCycle != null && t.InventoryCostClose.CloseDate.Date == previousCycle.EndDate.Value.Date)
                                   .Where(t => previousCycle != null && t.InventoryCostClose.AccountCycleId == previousCycle.Id)
                                   .WhereIf(items != null && items.Count() > 0, t => items.Contains(t.InventoryCostClose.ItemId))
                                   .AsNoTracking()
                                   .Select(u => new ItemDto
                                   {
                                       TransferOrderItemId = null,
                                       IsPurchase = true,
                                       ItemId = u.InventoryCostClose.ItemId,
                                       TransactionId = null,
                                       TotalCost = Math.Round(u.Qty * u.InventoryCostClose.Cost, roundingDigit),
                                       UnitCost = u.InventoryCostClose.Cost,
                                       Qty = u.Qty,

                                       LocationId = u.Lot.LocationId,
                                       LocationName = u.Lot.Location.LocationName,

                                       RoundingDigit = roundingDigit,
                                       RoundingDigitUnitCost = roundingDigitUnitCost,

                                       TotalLocationCost = u.InventoryCostClose.TotalCost,
                                       TotalLocationQty = u.InventoryCostClose.QtyOnhand,

                                   });

            var fromDate = previousCycle == null ? (DateTime?)null : previousCycle.EndDate.Value.Date;

            var inventoryTransactionQuery = GetInventoryValuationDetailQuery(fromDate, toDate, Locations, items, filterByUserId);


            var inventoryTransactionItemQuery = inventoryTransactionQuery.Concat(inventoryCostClose);



            if (filterByUserId != null)
            {
                // get user by group member
                var locationGroups = GetLocationGroup(filterByUserId.Value);

                var itemGroup = this.GetItemByUserGroup(filterByUserId.Value);
                inventoryTransactionItemQuery = from u in inventoryTransactionItemQuery
                                                join g in locationGroups on u.LocationId equals g
                                                join i in itemGroup on u.ItemId equals i
                                                select u;
            }


            return inventoryTransactionItemQuery;
        }


        public async Task<List<ItemDto>> GetInventoryValuationDetailList(
                             DateTime? fromDate,
                             DateTime? toDate,
                             List<long?> Locations,
                             List<Guid> itemsToSelect = null,
                             long? filterByUserId = null)
        {

            var previousCycle = GetPreviousCloseCyleAsync(toDate ?? DateTime.Now);
            var currentDigit = GetCyleAsync(toDate ?? DateTime.Now);
            var roundDigit = currentDigit != null ? currentDigit.RoundingDigit : 2;
            var roundDigitUnitCost = currentDigit != null ? currentDigit.RoundingDigitUnitCost : 2;
            var roundingDigit = previousCycle != null ? previousCycle.RoundingDigit : roundDigit;
            var roundingDigitUnitCost = previousCycle != null ? previousCycle.RoundingDigitUnitCost : roundDigitUnitCost;


            var itemReceiptItemQuery = await (from Ir in _itemReceiptItemRepository.GetAll()
                                            .WhereIf(itemsToSelect != null && itemsToSelect.Count > 0, t => itemsToSelect.Contains(t.ItemId))
                                            .AsNoTracking()

                                              join ji in _journalItemRepository.GetAll().Where(s => s.Identifier != null && s.Key == PostingKey.Inventory)
                                              .AsNoTracking()
                                              on Ir.Id equals ji.Identifier

                                              join j in _journalRepository.GetAll()
                                              .Where(s => s.ItemReceiptId.HasValue)
                                              .Where(s => s.Status == TransactionStatus.Publish)
                                              .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                              .WhereIf(toDate != null, u => u.Date.Date <= toDate.Value.Date)
                                              .AsNoTracking()
                                              on Ir.ItemReceiptId equals j.ItemReceiptId
                                              select new ItemDto
                                              {
                                                  JournalReference = j.Reference,
                                                  CreationTime = Ir.CreationTime,
                                                  TransferOrderItemId = Ir.TransferOrderItemId,
                                                  IsPurchase = true,
                                                  ItemId = Ir.ItemId,
                                                  TransactionId = Ir.ItemReceiptId,
                                                  TotalCost = Math.Round(Ir.Total, roundingDigit),
                                                  Qty = Ir.Qty,
                                                  LocationId = Ir.Lot.LocationId,
                                                  LocationName = Ir.Lot.Location.LocationName,
                                                  JournalDate = j.Date,
                                                  JournalType = j.JournalType,
                                                  UnitCost = Ir.UnitCost,
                                                  TransactionItemId = Ir.Id,
                                                  JournalNo = j.JournalNo,
                                                  JournalId = j.Id,
                                                  CreationTimeIndex = j.CreationTimeIndex,
                                                  OrderId = Ir.ItemReceipt.TransferOrderId != null ? Ir.ItemReceipt.TransferOrderId : Ir.ItemReceipt.ProductionOrderId,
                                                  RoundingDigit = roundingDigit,
                                                  RoundingDigitUnitCost = roundingDigitUnitCost,
                                                  InventoryAccountId = ji.AccountId,
                                                  Description = Ir.Description
                                              })
                                            .ToListAsync();

            var itemReceiptCustomerCreditItemQuery = await (from IrC in _itemReceiptCustomerCreditItemRepository.GetAll()
                                                            .WhereIf(itemsToSelect != null && itemsToSelect.Count > 0, t => itemsToSelect.Contains(t.ItemId))
                                                            .AsNoTracking()

                                                            join ji in _journalItemRepository.GetAll().Where(s => s.Identifier != null && s.Key == PostingKey.Inventory)
                                                            .AsNoTracking()
                                                            on IrC.Id equals ji.Identifier

                                                            join J in _journalRepository.GetAll()
                                                            .Where(s => s.ItemReceiptCustomerCreditId.HasValue)
                                                            .Where(s => s.Status == TransactionStatus.Publish)
                                                            .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                                            .WhereIf(toDate != null,
                                                                (u => (u.Date.Date) <= (toDate.Value.Date)))
                                                            .AsNoTracking()
                                                            on IrC.ItemReceiptCustomerCreditId equals J.ItemReceiptCustomerCreditId
                                                            select new ItemDto
                                                            {
                                                                PurchaseCost = IrC.Item.PurchaseCost ?? 0,
                                                                JournalReference = J.Reference,
                                                                CreationTime = IrC.CreationTime,
                                                                TransferOrderItemId = null,
                                                                IsPurchase = true,
                                                                ItemId = IrC.Item.Id,
                                                                TransactionId = IrC.ItemReceiptCustomerCreditId,
                                                                JournalType = J.JournalType,
                                                                UnitCost = IrC.UnitCost,
                                                                TransactionItemId = IrC.Id,
                                                                TotalCost = Math.Round(IrC.Total, roundingDigit),
                                                                Qty = IrC.Qty,
                                                                JournalDate = J.Date,
                                                                LocationId = IrC.Lot.LocationId,
                                                                LocationName = IrC.Lot.Location.LocationName,
                                                                JournalNo = J.JournalNo,
                                                                JournalId = J.Id,
                                                                CreationTimeIndex = J.CreationTimeIndex,
                                                                OrderId = null,
                                                                RoundingDigit = roundingDigit,
                                                                RoundingDigitUnitCost = roundingDigitUnitCost,
                                                                InventoryAccountId = ji.AccountId,
                                                                Description = IrC.Description
                                                            })
                                                      .ToListAsync();

            var itemIssueItemQuery = await (from isue in _itemIssueItemRepository.GetAll()
                                            .WhereIf(itemsToSelect != null && itemsToSelect.Count > 0, t => itemsToSelect.Contains(t.ItemId))
                                            .AsNoTracking()

                                            join j in _journalRepository.GetAll()
                                            .Where(s => s.ItemIssueId.HasValue)
                                            .Where(s => s.Status == TransactionStatus.Publish)
                                            .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                            .WhereIf(toDate != null,
                                                (u => (u.Date.Date) <= (toDate.Value.Date)))
                                            .AsNoTracking()
                                            on isue.ItemIssueId equals j.ItemIssueId
                                            select new ItemDto
                                            {
                                                JournalReference = j.Reference,
                                                CreationTime = isue.CreationTime,
                                                TransferOrderItemId = isue.TransferOrderItemId,
                                                IsPurchase = false,
                                                ItemId = isue.Item.Id,
                                                TransactionId = isue.ItemIssueId,
                                                TotalCost = (isue.Total * -1),
                                                Qty = (isue.Qty * -1),
                                                LocationId = isue.Lot.LocationId,
                                                LocationName = isue.Lot.Location.LocationName,
                                                JournalType = j.JournalType,
                                                JournalDate = j.Date,
                                                UnitCost = isue.UnitCost,
                                                TransactionItemId = isue.Id,
                                                JournalNo = j.JournalNo,
                                                JournalId = j.Id,
                                                CreationTimeIndex = j.CreationTimeIndex,
                                                OrderId = isue.ItemIssue.TransferOrderId != null ? isue.ItemIssue.TransferOrderId : isue.ItemIssue.ProductionOrderId,
                                                RoundingDigit = roundingDigit,
                                                RoundingDigitUnitCost = roundingDigitUnitCost,
                                                Description = isue.Description
                                            })
                                      .ToListAsync();

            var itemIssueVendorCreditItemQuery = await (from isv in _itemIssueVendorCreditItemRepository.GetAll()
                                                        .WhereIf(itemsToSelect != null && itemsToSelect.Count > 0, t => itemsToSelect.Contains(t.ItemId))
                                                        .AsNoTracking()
                                                        join j in _journalRepository.GetAll()
                                                        .Where(s => s.ItemIssueVendorCreditId.HasValue)
                                                        .Where(s => s.Status == TransactionStatus.Publish)
                                                        .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                                        .WhereIf(toDate != null,
                                                        (u => (u.Date.Date) <= (toDate.Value.Date)))
                                                        .AsNoTracking()
                                                        on isv.ItemIssueVendorCreditId equals j.ItemIssueVendorCreditId
                                                        select new ItemDto
                                                        {
                                                            JournalReference = j.Reference,
                                                            CreationTime = isv.CreationTime,
                                                            TransferOrderItemId = null,
                                                            IsPurchase = false,
                                                            ItemId = isv.Item.Id,
                                                            TransactionId = isv.ItemIssueVendorCreditId,
                                                            TotalCost = (isv.Total * -1),
                                                            Qty = (isv.Qty * -1),
                                                            LocationId = isv.Lot.LocationId,
                                                            LocationName = isv.Lot.Location.LocationName,
                                                            JournalType = j.JournalType,
                                                            JournalDate = j.Date,
                                                            UnitCost = isv.UnitCost,
                                                            TransactionItemId = isv.Id,
                                                            JournalNo = j.JournalNo,
                                                            OrderId = null,
                                                            JournalId = j.Id,
                                                            CreationTimeIndex = j.CreationTimeIndex,
                                                            RoundingDigit = roundingDigit,
                                                            RoundingDigitUnitCost = roundingDigitUnitCost,
                                                            Description = isv.Description

                                                        })
                                                  .ToListAsync();

            var inventoryTransactionItemQuery = itemReceiptItemQuery.Concat(itemReceiptCustomerCreditItemQuery).Concat(itemIssueItemQuery).Concat(itemIssueVendorCreditItemQuery);


            if (filterByUserId != null)
            {
                // get user by group member
                var locationGroups = await GetLocationGroup(filterByUserId.Value).ToListAsync();
                var itemGroup = await GetItemByUserGroup(filterByUserId.Value).ToListAsync();

                inventoryTransactionItemQuery = from u in inventoryTransactionItemQuery
                                                join g in locationGroups on u.LocationId equals g
                                                join i in itemGroup on u.ItemId equals i
                                                select u;
            }

            return inventoryTransactionItemQuery.ToList();
        }


        public async Task<List<ItemDto>> GetInventoryValuationDetailList(
                                DateTime toDate,
                                List<long?> Locations,
                                List<Guid> items = null,
                                long? filterByUserId = null)
        {
            var previousCycle = GetPreviousCloseCyleAsync(toDate);
            var currentDigit = GetCyleAsync(toDate);
            var roundDigit = currentDigit != null ? currentDigit.RoundingDigit : 2;
            var roundingDigit = previousCycle != null ? previousCycle.RoundingDigit : roundDigit;
            var roundingDigitUnitCost = previousCycle != null ? previousCycle.RoundingDigitUnitCost : roundDigit;

            var inventoryCostClose = await _inventoryCostCloseItemRepository.GetAll()
                                   //.Where(t => previousCycle != null && t.InventoryCostClose.CloseDate.Date == previousCycle.EndDate.Value.Date)
                                   .Where(t => previousCycle != null && t.InventoryCostClose.AccountCycleId == previousCycle.Id)
                                   .WhereIf(items != null && items.Count() > 0, t => items.Contains(t.InventoryCostClose.ItemId))
                                   .AsNoTracking()
                                   .Select(u => new ItemDto
                                   {
                                       TransferOrderItemId = null,
                                       IsPurchase = true,
                                       ItemId = u.InventoryCostClose.Item.Id,
                                       TransactionId = null,
                                       TotalCost = Math.Round(u.Qty * u.InventoryCostClose.Cost, roundingDigit, MidpointRounding.AwayFromZero),
                                       UnitCost = u.InventoryCostClose.Cost,
                                       Qty = u.Qty,

                                       LocationId = u.Lot.LocationId,
                                       LocationName = u.Lot.Location.LocationName,

                                       RoundingDigit = roundingDigit,
                                       RoundingDigitUnitCost = roundingDigitUnitCost,

                                       TotalLocationCost = u.InventoryCostClose.TotalCost,
                                       TotalLocationQty = u.InventoryCostClose.QtyOnhand
                                   }).ToListAsync();

            var fromDate = previousCycle == null ? (DateTime?)null : previousCycle.EndDate.Value.Date;

            var inventoryTransactionQuery = await GetInventoryValuationDetailList(fromDate, toDate, Locations, items, filterByUserId);


            var inventoryTransactionItemQuery = inventoryTransactionQuery.Concat(inventoryCostClose);



            if (filterByUserId != null)
            {
                // get user by group member
                var locationGroups = await GetLocationGroup(filterByUserId.Value).ToListAsync();
                var itemGroup = await this.GetItemByUserGroup(filterByUserId.Value).ToListAsync();

                inventoryTransactionItemQuery = from u in inventoryTransactionItemQuery
                                                join g in locationGroups on u.LocationId equals g
                                                join i in itemGroup on u.ItemId equals i
                                                select u;
            }


            return inventoryTransactionItemQuery.ToList();
        }

        public IQueryable<ItemCostSummaryDto> GetItemLatestCostSummaryQuery(
          DateTime toDate,
          List<long?> locations,
          List<Guid> items,
          List<Guid> exceptIds = null)
        {

            var itemReceiptItemQuery = from Ir in _itemReceiptItemRepository.GetAll()
                                                  .WhereIf(locations != null && locations.Count > 0, t => locations.Contains(t.Lot.LocationId))
                                                  .WhereIf(items != null && items.Count() > 0, t => items.Contains(t.ItemId))
                                                  .WhereIf(exceptIds != null, t => !exceptIds.Contains(t.ItemReceiptId))
                                                  .AsNoTracking()

                                       join j in _journalRepository.GetAll()
                                                   .Where(s => s.ItemReceiptId.HasValue)
                                                   .Where(s => s.Status == TransactionStatus.Publish)
                                                   .WhereIf(toDate != null, u => u.Date.Date <= toDate.Date)
                                                   .AsNoTracking()
                                       on Ir.ItemReceiptId equals j.ItemReceiptId

                                       select new
                                       {
                                           //IsPurchase = true,
                                           ItemId = Ir.ItemId,
                                           Qty = Ir.Qty,
                                           Cost = Ir.UnitCost,
                                           TotalCost = Ir.Total,
                                           LocationId = Ir.Lot == null ? 0 : Ir.Lot.LocationId,
                                           //LocationName = Ir.Lot == null ? "" : Ir.Lot.Location.LocationName,
                                           Date = j.Date,
                                           CreationTimeIndex = j.CreationTimeIndex
                                       };



            var itemReceiptCustomerCreditItemQuery = from IrC in _itemReceiptCustomerCreditItemRepository.GetAll()
                                                                 .WhereIf(locations != null && locations.Count > 0, t => locations.Contains(t.Lot.LocationId))
                                                                 .WhereIf(items != null && items.Count() > 0, t => items.Contains(t.ItemId))
                                                                 .WhereIf(exceptIds != null, t => !exceptIds.Contains(t.ItemReceiptCustomerCreditId))
                                                                 .AsNoTracking()
                                                     join J in _journalRepository.GetAll()
                                                                 .Where(s => s.ItemReceiptCustomerCreditId.HasValue)
                                                                 .Where(s => s.Status == TransactionStatus.Publish)
                                                                 .WhereIf(toDate != null, u => u.Date.Date <= toDate.Date)
                                                                 .AsNoTracking()
                                                     on IrC.ItemReceiptCustomerCreditId equals J.ItemReceiptCustomerCreditId

                                                     select new
                                                     {
                                                         //IsPurchase = true,
                                                         ItemId = IrC.ItemId,
                                                         Qty = IrC.Qty,
                                                         Cost = IrC.UnitCost,
                                                         TotalCost = IrC.Total,
                                                         LocationId = IrC.Lot == null ? 0 : IrC.Lot.LocationId,
                                                         //LocationName = IrC.Lot == null ? "" : IrC.Lot.Location.LocationName,
                                                         Date = J.Date,
                                                         CreationTimeIndex = J.CreationTimeIndex
                                                     };



            var itemReceiptResult = itemReceiptItemQuery.Concat(itemReceiptCustomerCreditItemQuery);

            var itemIssueItemQuery = from isue in _itemIssueItemRepository.GetAll()
                                                    .WhereIf(locations != null && locations.Count > 0, t => locations.Contains(t.Lot.LocationId))
                                                    .WhereIf(items != null && items.Count() > 0, t => items.Contains(t.ItemId))
                                                    .WhereIf(exceptIds != null, t => !exceptIds.Contains(t.ItemIssueId))
                                                    .AsNoTracking()
                                     join j in _journalRepository.GetAll()
                                                     .Where(s => s.ItemIssueId.HasValue)
                                                     .Where(s => s.Status == TransactionStatus.Publish)
                                                     .WhereIf(toDate != null, u => u.Date.Date <= toDate.Date)
                                                     .AsNoTracking()
                                     on isue.ItemIssueId equals j.ItemIssueId

                                     select new
                                     {
                                         //IsPurchase = false,
                                         ItemId = isue.ItemId,
                                         Qty = isue.Qty,
                                         Cost = isue.UnitCost,
                                         TotalCost = isue.Total,
                                         LocationId = isue.Lot == null ? 0 : isue.Lot.LocationId,
                                         //LocationName = isue.Lot == null ? "" : isue.Lot.Location.LocationName,
                                         Date = j.Date,
                                         CreationTimeIndex = j.CreationTimeIndex
                                     };



            var itemIssueVendorCreditItemQuery = from isv in _itemIssueVendorCreditItemRepository.GetAll()
                                                            .WhereIf(locations != null && locations.Count > 0, t => locations.Contains(t.Lot.LocationId))
                                                            .WhereIf(items != null && items.Count() > 0, t => items.Contains(t.ItemId))
                                                            .WhereIf(exceptIds != null, t => !exceptIds.Contains(t.ItemIssueVendorCreditId))
                                                            .AsNoTracking()
                                                 join j in _journalRepository.GetAll()
                                                             .Where(s => s.ItemIssueVendorCreditId.HasValue)
                                                             .Where(s => s.Status == TransactionStatus.Publish)
                                                             .WhereIf(toDate != null, u => u.Date.Date <= toDate.Date)
                                                             .AsNoTracking()
                                                 on isv.ItemIssueVendorCreditId equals j.ItemIssueVendorCreditId

                                                 select new
                                                 {
                                                     //IsPurchase = false,
                                                     ItemId = isv.ItemId,
                                                     Qty = isv.Qty,
                                                     Cost = isv.UnitCost,
                                                     TotalCost = isv.Total,
                                                     LocationId = isv.Lot == null ? 0 : isv.Lot.LocationId,
                                                     //LocationName = isv.Lot == null ? "" : isv.Lot.Location.LocationName,
                                                     Date = j.Date,
                                                     CreationTimeIndex = j.CreationTimeIndex
                                                 };


            var itemIssueResult = itemIssueItemQuery.Concat(itemIssueVendorCreditItemQuery);

            var inventoryTransactionItemQuery = itemReceiptResult.Concat(itemIssueResult);


            var result = inventoryTransactionItemQuery
                        .OrderByDescending(s => s.Date.Date).ThenByDescending(s => s.CreationTimeIndex)
                        .GroupBy(g => g.ItemId).Select(s => new ItemCostSummaryDto
                        {
                            IsPurchase = false,
                            ItemId = s.FirstOrDefault().ItemId,
                            Qty = s.FirstOrDefault().Qty,
                            Cost = s.FirstOrDefault().Cost,
                            TotalCost = s.FirstOrDefault().TotalCost,
                            LocationId = s.FirstOrDefault().LocationId,
                            //LocationName = s.FirstOrDefault().LocationName,
                        });


            return result;
        }


        public IQueryable<ItemBalanceDto> GetAssetBalanceForReport(
             string filter,
             DateTime? fromDate,
             DateTime toDate,
             List<long?> locations,
             List<long?> lots,
             List<Guid> items,
             List<GetListPropertyFilter> itemProperties,
             long? userId = null)
        {

            var periodCycles = GetCloseCyleQuery();

            var previousCycle = periodCycles.Where(u => u.EndDate != null &&
                                                        u.EndDate.Value.Date <= toDate.Date)
                                                        .OrderByDescending(u => u.EndDate.Value).FirstOrDefault();

            var currentCycle = periodCycles.Where(u => u.StartDate.Date <= toDate.Date && (u.EndDate == null || toDate.Date <= u.EndDate.Value.Date))
                                            .OrderByDescending(u => u.StartDate).FirstOrDefault();


            var lotQuery = from lt in _lotRepository.GetAll()
                                       .AsNoTracking()
                                       .Select(s => new
                                       {
                                           Id = s.Id,
                                           LotName = s.LotName,
                                           LocationId = s.LocationId
                                       })
                           join l in _locationRepository.GetAll()
                                      .AsNoTracking()
                                      .Select(s => new
                                      {
                                          Id = s.Id,
                                          LocationName = s.LocationName
                                      })
                           on lt.LocationId equals l.Id
                           select new
                           {
                               Id = lt.Id,
                               LotName = lt.LotName,
                               LocationId = lt.LocationId,
                               LocationName = l.LocationName
                           };


            var inventoryCostClose = from u in _inventoryCostCloseItemRepository.GetAll()
                                             //.Where(t => previousCycle != null && t.InventoryCostClose.CloseDate.Date == previousCycle.EndDate.Value.Date)
                                             .Where(t => previousCycle != null && t.InventoryCostClose.AccountCycleId == previousCycle.Id)
                                             .Where(t => t.InventoryCostClose.Item.ItemType.Name == CorarlERPConsts.ItemTypeAsset)
                                             .WhereIf(locations != null && locations.Count() > 0, t => locations.Contains(t.Lot.LocationId))
                                             .WhereIf(lots != null && lots.Count() > 0, t => lots.Contains(t.LotId))
                                             .AsNoTracking()
                                             .Select(s => new
                                             {
                                                 ItemId = s.InventoryCostClose.ItemId,
                                                 Qty = s.Qty,
                                                 LotId = s.LotId
                                             })
                                     join l in lotQuery
                                     on u.LotId equals l.Id

                                     select new ItemBalanceDto
                                     {
                                         IsPurchase = true,
                                         ItemId = u.ItemId,
                                         Qty = u.Qty,
                                         LotId = l.Id,
                                         LotName = l.LotName,
                                         LocationId = l.LocationId,
                                         LocationName = l.LocationName,
                                     };



            var previousCycleFromDate = previousCycle == null ? (DateTime?)null : previousCycle.EndDate.Value.Date;


            var inventoryTransactionQuery = GetAssetBalanceForReportQuery(filter, previousCycleFromDate, toDate, locations, lots, items, itemProperties);

            var inventoryTransactionItemQuery = inventoryTransactionQuery.Concat(inventoryCostClose)
                                    .OrderBy(s => s.ItemId)
                                    .ThenBy(s => s.LotId)
                                    .GroupBy(s => new { s.ItemId, s.LotId, s.LotName, s.LocationId, s.LocationName })
                                    .Select(r => new ItemBalanceDto()
                                    {
                                        LotId = r.Key.LotId,
                                        LotName = r.Key.LotName,
                                        ItemId = r.Key.ItemId,
                                        Qty = r.Sum(t => t.Qty),
                                        BeginningQty = r.Sum(u => u.Date == null || (fromDate != null && u.Date < fromDate) ? u.Qty : 0),
                                        InQty = r.Sum(u => u.IsPurchase && (fromDate == null || u.Date == null || u.Date >= fromDate) ? u.Qty : 0),
                                        OutQty = r.Sum(u => !u.IsPurchase && (fromDate == null || u.Date == null || u.Date >= fromDate) ? -u.Qty : 0),

                                        LocationId = r.Key.LocationId,
                                        LocationName = r.Key.LocationName,
                                    });

            if (userId != null)
            {
                var itemByUserGroup = GetItemByUserGroup(userId.Value);
                // get user by group member
                var locationGroups = GetLocationGroup(userId.Value);

                inventoryTransactionItemQuery = (from u in inventoryTransactionItemQuery
                                                 join g in locationGroups
                                                  on u.LocationId equals g
                                                 join it in itemByUserGroup
                                                 on u.ItemId equals it
                                                 select u);

            }

            return inventoryTransactionItemQuery;
        }


        public IQueryable<ItemBalanceDto> GetAssetBalanceForReportQuery(
              string filter,
              DateTime? fromDate,
              DateTime? toDate,
              List<long?> locations,
              List<long?> lots,
              List<Guid> items1,
              List<GetListPropertyFilter> itemProperties)
        {

            var lotQuery = from lt in _lotRepository.GetAll()
                                       .AsNoTracking()
                                       .Select(s => new
                                       {
                                           Id = s.Id,
                                           LotName = s.LotName,
                                           LocationId = s.LocationId
                                       })
                           join l in _locationRepository.GetAll()
                                      .AsNoTracking()
                                      .Select(s => new
                                      {
                                          Id = s.Id,
                                          LocationName = s.LocationName
                                      })
                           on lt.LocationId equals l.Id
                           select new
                           {
                               Id = lt.Id,
                               LotName = lt.LotName,
                               LocationId = lt.LocationId,
                               LocationName = l.LocationName
                           };

            var itemReceiptItemQuery = from Ir in _itemReceiptItemRepository.GetAll()
                                                  .Where(u => u.Item.ItemType.Name == CorarlERPConsts.ItemTypeAsset)
                                                  .WhereIf(locations != null && locations.Count > 0, t => t.Lot != null && locations.Contains(t.Lot.LocationId))
                                                  .WhereIf(lots != null && lots.Count > 0, t => lots.Contains(t.LotId))
                                                  .AsNoTracking()
                                                  .Select(s => new
                                                  {
                                                      ItemId = s.ItemId,
                                                      LotId = s.LotId,
                                                      Qty = s.Qty,
                                                      ItemReceiptId = s.ItemReceiptId
                                                  })
                                       join j in _journalRepository.GetAll()
                                                   .Where(s => s.Status == TransactionStatus.Publish)
                                                   .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                                   .Where(s => s.ItemReceiptId.HasValue)
                                                   .WhereIf(toDate != null,
                                                       (u => (u.Date.Date) <= (toDate.Value.Date)))
                                                   .AsNoTracking()
                                                   .Select(s => new
                                                   {
                                                       Date = s.Date,
                                                       ItemReceiptId = s.ItemReceiptId
                                                   })
                                       on Ir.ItemReceiptId equals j.ItemReceiptId
                                       join l in lotQuery
                                       on Ir.LotId equals l.Id
                                       select new ItemBalanceDto
                                       {
                                           Date = j.Date,
                                           IsPurchase = true,
                                           ItemId = Ir.ItemId,
                                           Qty = Ir.Qty,
                                           LotId = l.Id,
                                           LotName = l.LotName,
                                           LocationId = l.LocationId,
                                           LocationName = l.LocationName,
                                       };

            var itemReceiptCustomerCreditItemQuery = from IrC in _itemReceiptCustomerCreditItemRepository.GetAll()
                                                                .Where(u => u.Item.ItemType.Name == CorarlERPConsts.ItemTypeAsset)
                                                                .WhereIf(locations != null && locations.Count > 0, t => t.Lot != null && locations.Contains(t.Lot.LocationId))
                                                                .WhereIf(lots != null && lots.Count > 0, t => lots.Contains(t.LotId))
                                                                .AsNoTracking()
                                                                .Select(s => new
                                                                {
                                                                    Qty = s.Qty,
                                                                    ItemId = s.ItemId,
                                                                    LotId = s.LotId,
                                                                    ItemReceiptCustomerCreditId = s.ItemReceiptCustomerCreditId
                                                                })
                                                     join J in _journalRepository.GetAll()
                                                             .Where(s => s.Status == TransactionStatus.Publish)
                                                             .Where(s => s.ItemReceiptCustomerCreditId.HasValue)
                                                             .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                                             .WhereIf(toDate != null,
                                                                 (u => (u.Date.Date) <= (toDate.Value.Date)))
                                                             .AsNoTracking()
                                                             .Select(s => new
                                                             {
                                                                 Date = s.Date,
                                                                 ItemReceiptCustomerCreditId = s.ItemReceiptCustomerCreditId
                                                             })
                                                     on IrC.ItemReceiptCustomerCreditId equals J.ItemReceiptCustomerCreditId

                                                     join l in lotQuery
                                                     on IrC.LotId equals l.Id

                                                     select new ItemBalanceDto
                                                     {
                                                         Date = J.Date,
                                                         IsPurchase = true,
                                                         ItemId = IrC.ItemId,
                                                         Qty = IrC.Qty,
                                                         LotId = l.Id,
                                                         LotName = l.LotName,
                                                         LocationId = l.LocationId,
                                                         LocationName = l.LocationName,
                                                     };

            var itemReceiptResult = itemReceiptItemQuery.Concat(itemReceiptCustomerCreditItemQuery);

            var itemIssueItemQuery = from isue in _itemIssueItemRepository.GetAll()
                                                  .Where(u => u.Item.ItemType.Name == CorarlERPConsts.ItemTypeAsset)
                                                  .WhereIf(locations != null && locations.Count > 0, t => t.Lot != null && locations.Contains(t.Lot.LocationId))
                                                  .WhereIf(lots != null && lots.Count > 0, t => lots.Contains(t.LotId))
                                                  .AsNoTracking()
                                                  .Select(s => new
                                                  {
                                                      ItemId = s.ItemId,
                                                      Qty = s.Qty,
                                                      ItemIssueId = s.ItemIssueId,
                                                      LotId = s.LotId
                                                  })
                                     join j in _journalRepository.GetAll()
                                                 .Where(s => s.Status == TransactionStatus.Publish)
                                                 .Where(s => s.ItemIssueId.HasValue)
                                                 .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                                 .WhereIf(toDate != null,
                                                     (u => (u.Date.Date) <= (toDate.Value.Date)))
                                                 .AsNoTracking()
                                                 .Select(s => new
                                                 {
                                                     Date = s.Date,
                                                     ItemIssueId = s.ItemIssueId
                                                 })
                                     on isue.ItemIssueId equals j.ItemIssueId
                                     join l in lotQuery
                                     on isue.LotId equals l.Id
                                     select new ItemBalanceDto
                                     {
                                         Date = j.Date,
                                         IsPurchase = false,
                                         ItemId = isue.ItemId,
                                         Qty = (isue.Qty * -1),
                                         LotId = l.Id,
                                         LotName = l.LotName,
                                         LocationId = l.LocationId,
                                         LocationName = l.LocationName,
                                     };

            var itemIssueVendorCreditItemQuery = from isv in _itemIssueVendorCreditItemRepository.GetAll()
                                                            .Where(u => u.Item.ItemType.Name == CorarlERPConsts.ItemTypeAsset)
                                                            .WhereIf(locations != null && locations.Count > 0, t => t.Lot != null && locations.Contains(t.Lot.LocationId))
                                                            .WhereIf(lots != null && lots.Count > 0, t => lots.Contains(t.LotId))
                                                            .AsNoTracking()
                                                            .Select(s => new
                                                            {
                                                                ItemId = s.ItemId,
                                                                LotId = s.LotId,
                                                                Qty = s.Qty,
                                                                ItemIssueVendorCreditId = s.ItemIssueVendorCreditId
                                                            })
                                                 join j in _journalRepository.GetAll()
                                                             .Where(s => s.Status == TransactionStatus.Publish)
                                                             .Where(s => s.ItemIssueVendorCreditId.HasValue)
                                                             .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                                             .WhereIf(toDate != null,
                                                             (u => (u.Date.Date) <= (toDate.Value.Date)))
                                                             .AsNoTracking()
                                                             .Select(s => new
                                                             {
                                                                 Date = s.Date,
                                                                 ItemIssueVendorCreditId = s.ItemIssueVendorCreditId
                                                             })
                                                 on isv.ItemIssueVendorCreditId equals j.ItemIssueVendorCreditId
                                                 join l in lotQuery
                                                 on isv.LotId equals l.Id
                                                 select new ItemBalanceDto
                                                 {
                                                     Date = j.Date,
                                                     IsPurchase = false,
                                                     ItemId = isv.ItemId,
                                                     Qty = (isv.Qty * -1),
                                                     LotId = l.Id,
                                                     LotName = l.LotName,
                                                     LocationId = l.LocationId,
                                                     LocationName = l.LocationName,
                                                 };
            var itemIssueResult = itemIssueItemQuery.Concat(itemIssueVendorCreditItemQuery);

            var inventoryTransactionItemQuery = itemReceiptResult.Concat(itemIssueResult);

            return inventoryTransactionItemQuery;
        }


        [UnitOfWork(IsDisabled = true)]
        public async Task RecalculationCostHelper(int tenantId, DateTime? inputFromDate, DateTime inputToDate, List<Guid> inputItems)
        {
            var accountCycles = new List<AccountCycle>();
            Guid? roundingAccountId;
            Guid? ItemRecieptAdjustmentId;
            int roundingDigit = 2;
            int roundingDigitUnitCost = 2;

            var previosItemDic = new Dictionary<string, InventoryTransactionItem>();

            //use for update after calculation no need to get again
            var inventoryTransactionItems = new List<InventoryTransactionItem>();

            var journalToUpdateEntity = new List<Journal>();
            var journalItemDic = new Dictionary<Guid, List<JournalItem>>();
            var productionDic = new Dictionary<Guid, Production>();
            var productionToUpdateEntity = new List<Production>();

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    accountCycles = await _accountCycleRepository.GetAll().AsNoTracking().ToListAsync();
                    var currentPeriod = accountCycles.Where(u => u.EndDate == null).FirstOrDefault();

                    if (currentPeriod != null)
                    {
                        roundingDigit = currentPeriod.RoundingDigit;
                        roundingDigitUnitCost = currentPeriod.RoundingDigitUnitCost;
                    }


                    bool isFromDateBeforeCurrent = currentPeriod != null && inputFromDate.HasValue && inputFromDate.Value.Date < currentPeriod.StartDate.Date;


                    DateTime? fromDate = null;
                    if (isFromDateBeforeCurrent) fromDate = currentPeriod.StartDate;
                    else if (inputFromDate.HasValue) fromDate = inputFromDate;

                    #region begaing
                    int prevRoundingDigit = roundingDigit;
                    int preRoundingDigitUnitCost = roundingDigitUnitCost;

                    if (isFromDateBeforeCurrent)
                    {
                        var previousCycle = accountCycles.Where(u => u.EndDate != null && u.EndDate.Value.Date < fromDate.Value.Date)
                                      .OrderByDescending(u => u.EndDate.Value).Select(s => new { s.RoundingDigit, s.RoundingDigitUnitCost }).FirstOrDefault();

                        if (previousCycle != null)
                        {
                            prevRoundingDigit = previousCycle.RoundingDigit;
                            preRoundingDigitUnitCost = previousCycle.RoundingDigitUnitCost;
                        }
                    }

                    //Update later after close cycle take from close
                    previosItemDic = await (from u in _inventoryTransactionItemRepository.GetAll()
                                                    .Where(t => t.Date.Date < fromDate.Value.Date)
                                                    .Where(s => inputItems == null || !inputItems.Any() || inputItems.Contains(s.ItemId))
                                                    .AsNoTracking()
                                            orderby u.OrderIndex descending
                                            group u by new { u.ItemId, u.LocationId } into u
                                            select u.FirstOrDefault())
                                            .ToDictionaryAsync(s => $"{s.ItemId}-{s.LocationId}", s => s);

                    #endregion


                    #region current period

                    inventoryTransactionItems = await _inventoryTransactionItemRepository.GetAll()
                                                      .Where(t => fromDate == null || t.Date.Date >= fromDate.Value.Date)
                                                      .Where(t => t.Date.Date <= inputToDate.Date)
                                                      .Where(s => inputItems == null || !inputItems.Any() || inputItems.Contains(s.ItemId))
                                                      .AsNoTracking()
                                                      .OrderBy(s => s.OrderIndex)
                                                      .ToListAsync();
                    #endregion


                    var tenant = await _tenantRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == tenantId);
                    roundingAccountId = tenant.RoundDigitAccountId;
                    ItemRecieptAdjustmentId = tenant.ItemRecieptAdjustmentId;


                    var journalIds = inventoryTransactionItems
                                     .GroupBy(s => s.JournalId)
                                     .Select(s => s.Key)
                                     .ToHashSet();

                    journalToUpdateEntity = await _journalRepository.GetAll()
                                                    .Where(x => journalIds.Contains(x.Id))
                                                    .Where(s => s.ItemIssueId.HasValue || s.ItemReceiptCustomerCreditId.HasValue || s.JournalType == JournalType.ItemReceiptTransfer)
                                                    .AsNoTracking()
                                                    .ToListAsync();

                    journalItemDic = await _journalItemRepository.GetAll()
                                                    .Where(x => journalIds.Contains(x.JournalId))
                                                    .AsNoTracking()
                                                    .GroupBy(s => s.JournalId)
                                                    .ToDictionaryAsync(s => s.Key, s => s.ToList());

                    var productionIds = inventoryTransactionItems
                                        .Where(s => s.TransferOrProductionId.HasValue)
                                         .GroupBy(s => s.TransferOrProductionId.Value)
                                         .Select(s => s.Key)
                                         .ToHashSet();
                    productionDic = await _productionRepository.GetAll()
                                                     .Where(s => productionIds.Contains(s.Id))
                                                     .AsNoTracking()
                                                     .ToDictionaryAsync(k => k.Id, v => v);
                }

            }


            //TransferOrder: $OrderId - Item - Qty\            
            var issuesForTransferOrderDic = new Dictionary<string, InventoryTransactionItem>();

            var journalItemsCreateEntity = new List<JournalItem>();
            var journalItemsUpdateEntity = new List<JournalItem>();
            var journalItemsDeleteEntity = new List<JournalItem>();

            foreach (var tran in inventoryTransactionItems)
            {

                var costKey = $"{tran.ItemId.ToString()}-{tran.LocationId}";


                var prevTran = previosItemDic.ContainsKey(costKey) ? previosItemDic[costKey] : null;


                if (tran.IsItemReceipt)
                {

                    var lineCost = Math.Round(tran.LineCost, roundingDigit, MidpointRounding.AwayFromZero);
                    var unitCost = Math.Round(tran.UnitCost, roundingDigitUnitCost, MidpointRounding.AwayFromZero);

                    if (tran.JournalType == JournalType.ItemReceiptTransfer &&
                        tran.TransferOrProductionId.HasValue &&
                        tran.TransferOrProductionItemId.HasValue)
                    {
                        var key = $"{tran.TransferOrProductionItemId.Value.ToString()}";

                        if (issuesForTransferOrderDic.ContainsKey(key))
                        {
                            var issueItem = issuesForTransferOrderDic[key];
                            unitCost = issueItem.UnitCost;
                            lineCost = Math.Abs(issueItem.LineCost);
                            tran.SetUnitCost(unitCost);
                            tran.SetLineCost(lineCost);
                        }

                    }
                    else if (tran.JournalType == JournalType.ItemReceiptCustomerCredit)
                    {

                        unitCost = prevTran != null ? prevTran.LatestCost : 0;
                        lineCost = Math.Abs(Math.Round(unitCost * tran.Qty, roundingDigit, MidpointRounding.AwayFromZero));
                        tran.SetUnitCost(unitCost);
                        tran.SetLineCost(lineCost);
                    }


                    var oldTotalQty = prevTran == null ? 0 : prevTran.QtyOnHand;

                    tran.SetQtyOnHand(oldTotalQty + tran.Qty);

                    var oldTotalCost = prevTran == null ? 0 : prevTran.TotalCost;

                    tran.SetTotalCost(tran.QtyOnHand == 0 ? 0 : oldTotalCost + lineCost);

                    //Adjust Cost to Zero
                    if (oldTotalQty < 0)
                    {
                        tran.SetTotalCost(Math.Round(tran.QtyOnHand * unitCost, roundingDigit));
                        var adjustmentAmount = Math.Round(oldTotalCost + lineCost - tran.TotalCost, roundingDigit);
                        tran.SetAdjustmentCost(adjustmentAmount);

                        if (adjustmentAmount != 0)
                        {
                            var inventoryJournalItem = JournalItem.CreateJournalItem(
                                        tran.TenantId, tran.CreatorUserId, tran.JournalId, tran.InventoryAccountId,
                                        "Adjustment To Zero", 0, tran.AdjustmentCost, PostingKey.InventoryAdjustmentInv, tran.Id);
                            journalItemsCreateEntity.Add(inventoryJournalItem);


                            var adjustmentJournalItem = JournalItem.CreateJournalItem(
                                                   tran.TenantId, tran.CreatorUserId, tran.JournalId, ItemRecieptAdjustmentId.Value,
                                                   "Adjustment To Zero", tran.AdjustmentCost, 0, PostingKey.InventoryAdjustmentAdj, tran.Id);
                            journalItemsCreateEntity.Add(adjustmentJournalItem);

                            if (tran.AdjustmentCost < 0)
                            {
                                var total = Math.Abs(tran.AdjustmentCost);

                                inventoryJournalItem.SetDebitCredit(total, 0);
                                adjustmentJournalItem.SetDebitCredit(0, total);

                                if (tran.TransferOrProductionId.HasValue && productionDic.ContainsKey(tran.TransferOrProductionId.Value))
                                {
                                    var adj = productionDic[tran.TransferOrProductionId.Value];
                                    adj.SetCalculateionState(CalculationState.CalculateAdj);
                                    productionToUpdateEntity.Add(adj);
                                    productionDic.Remove(tran.TransferOrProductionId.Value);
                                }
                            }
                        }
                    }

                    //Todo: rounding digit
                    tran.SetAvgCost(Math.Abs(Math.Round(tran.QtyOnHand == 0 ? 0 : tran.TotalCost / tran.QtyOnHand, roundingDigitUnitCost, MidpointRounding.AwayFromZero)));

                }
                else
                {

                    tran.SetUnitCost(prevTran == null ? 0 : prevTran.AvgCost);


                    var lineTotal = Math.Round(tran.UnitCost * tran.Qty, roundingDigit);

                    tran.SetQtyOnHand((prevTran == null ? 0 : prevTran.QtyOnHand) + tran.Qty);

                    //adjustment when qty = 0 
                    if (tran.QtyOnHand == 0)
                    {
                        lineTotal = -(prevTran == null ? 0 : prevTran.TotalCost);
                    }
                    else if (tran.QtyOnHand < 0)
                    {
                        if (tran.TransferOrProductionId.HasValue && productionDic.ContainsKey(tran.TransferOrProductionId.Value))
                        {
                            var adj = productionDic[tran.TransferOrProductionId.Value];
                            adj.SetCalculateionState(CalculationState.CalculateErr);
                            productionToUpdateEntity.Add(adj);
                            productionDic.Remove(tran.TransferOrProductionId.Value);
                        }
                    }

                    var totalCostBeforeZero = (prevTran == null ? 0 : prevTran.TotalCost) + lineTotal;

                    tran.SetTotalCost(tran.QtyOnHand == 0 ? 0 : totalCostBeforeZero);
                    tran.SetLineCost(lineTotal);
                    tran.SetAvgCost(Math.Abs(Math.Round(tran.QtyOnHand == 0 ? 0 : tran.TotalCost / tran.QtyOnHand, roundingDigitUnitCost, MidpointRounding.AwayFromZero)));



                    if (tran.JournalType == JournalType.ItemIssueTransfer &&
                        tran.TransferOrProductionId.HasValue &&
                        tran.TransferOrProductionItemId.HasValue)
                    {

                        var qtyKey = Math.Abs(Math.Round(tran.Qty, roundingDigitUnitCost, MidpointRounding.AwayFromZero));

                        var key = $"{tran.TransferOrProductionItemId.Value.ToString()}";
                        if (!issuesForTransferOrderDic.ContainsKey(key))
                        {
                            issuesForTransferOrderDic.Add(key, tran);
                        }
                        else
                        {
                            issuesForTransferOrderDic[key] = tran;
                        }

                    }

                }

                //Update latest cost
                tran.SetLatestCost(tran.AvgCost);

                if (previosItemDic.ContainsKey(costKey))
                {
                    if (tran.QtyOnHand == 0) tran.SetLatestCost(previosItemDic[costKey].LatestCost);

                    previosItemDic[costKey] = tran;
                }
                else
                {
                    previosItemDic.Add(costKey, tran);
                }


                if (journalItemDic.ContainsKey(tran.JournalId))
                {
                    var jouranlItems = journalItemDic[tran.JournalId];

                    if (tran.IsItemReceipt)
                    {
                        //Delete only old Adjument Journal Items related to Item 
                        var deleteJournalItems = jouranlItems
                                                 .Where(s => s.Identifier == tran.Id)
                                                 .Where(s => s.Key == PostingKey.InventoryAdjustmentAdj ||
                                                             s.Key == PostingKey.InventoryAdjustmentInv)
                                                 .ToList();
                        if (deleteJournalItems.Any()) journalItemsDeleteEntity.AddRange(deleteJournalItems);
                    }

                    if (!tran.IsItemReceipt || tran.JournalType == JournalType.ItemReceiptCustomerCredit || tran.JournalType == JournalType.ItemReceiptTransfer)
                    {
                        var journalItemToUpdate = jouranlItems.Where(s => s.Identifier == tran.Id).ToList();

                        foreach (var ji in journalItemToUpdate)
                        {
                            var lineCost = Math.Abs(tran.LineCost);

                            if (tran.IsItemReceipt)
                            {
                                if (ji.Key == PostingKey.COGS)
                                {
                                    ji.SetDebitCredit(0, lineCost);

                                }
                                else if (ji.Key == PostingKey.Inventory)
                                {
                                    ji.SetDebitCredit(lineCost, 0);
                                }
                            }
                            else
                            {
                                if (ji.Key == PostingKey.COGS)
                                {
                                    ji.SetDebitCredit(lineCost, 0);
                                }
                                else if (ji.Key == PostingKey.Inventory)
                                {
                                    ji.SetDebitCredit(0, lineCost);
                                }
                            }

                            journalItemsUpdateEntity.Add(ji);
                        }
                    }
                }

            }

            foreach (var j in journalToUpdateEntity)
            {
                if (journalItemDic.ContainsKey(j.Id))
                {
                    var journalItems = journalItemDic[j.Id];
                    var total = journalItems
                               .Where(s => s.Identifier.HasValue && s.Key != PostingKey.InventoryAdjustmentAdj && s.Key != PostingKey.InventoryAdjustmentInv)
                               .Sum(s => j.ItemReceiptId.HasValue || j.ItemReceiptCustomerCreditId.HasValue ? s.Debit : s.Credit);

                    j.SetDebitCredit(total);

                    if (j.JournalType == JournalType.ItemReceiptTransfer)
                    {
                        //update header journal item account
                        var headerJouranlItem = journalItems.FirstOrDefault(s => s.JournalId == j.Id && !s.Identifier.HasValue && s.Key == PostingKey.Clearance);
                        if (headerJouranlItem != null)
                        {
                            headerJouranlItem.SetDebitCredit(0, total);
                            journalItemsUpdateEntity.Add(headerJouranlItem);
                        }
                    }
                    else if (j.JournalType == JournalType.ItemIssueProduction ||
                            j.JournalType == JournalType.ItemIssueTransfer ||
                            j.JournalType == JournalType.ItemIssueOther ||
                            j.JournalType == JournalType.ItemIssueAdjustment ||
                            j.JournalType == JournalType.ItemIssuePhysicalCount)
                    {
                        //update header journal item account
                        var headerJouranlItem = journalItems.FirstOrDefault(s => s.JournalId == j.Id && !s.Identifier.HasValue && s.Key == PostingKey.COGS);
                        if (headerJouranlItem != null)
                        {
                            headerJouranlItem.SetDebitCredit(total, 0);
                            journalItemsUpdateEntity.Add(headerJouranlItem);
                        }
                    }
                }
            }
            ;

            foreach (var p in productionDic.Values)
            {
                p.SetCalculateionState(CalculationState.Calculated);
                productionToUpdateEntity.Add(p);
            }

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {

                    if (journalItemsDeleteEntity.Any()) await _journalItemRepository.BulkDeleteAsync(journalItemsDeleteEntity);
                    if (journalItemsCreateEntity.Any()) await _journalItemRepository.BulkInsertAsync(journalItemsCreateEntity);
                    if (journalItemsUpdateEntity.Any()) await _journalItemRepository.BulkUpdateAsync(journalItemsUpdateEntity);
                    if (journalToUpdateEntity.Any()) await _journalRepository.BulkUpdateAsync(journalToUpdateEntity);

                    if (inventoryTransactionItems.Any()) await _inventoryTransactionItemRepository.BulkUpdateAsync(inventoryTransactionItems);

                    if (productionToUpdateEntity.Any()) await _productionRepository.BulkUpdateAsync(productionToUpdateEntity);

                }

                await uow.CompleteAsync();
            }
        }

        public async Task<List<BatchNoItemBalanceOutput>> GetItemBatchNoBalance(List<Guid> items, List<long> lots, List<Guid> batchNos, DateTime date, List<Guid> exceptIds)
        {
            var closeAccount = await _accountCycleRepository.GetAll().Where(u => u.EndDate != null && u.EndDate.Value.Date <= date.Date).AsNoTracking()
                                                            .OrderByDescending(u => u.EndDate.Value).FirstOrDefaultAsync();

            var fromDate = DateTime.MinValue;
            IQueryable<BatchNoItemBalanceOutput> closeQuery = null;
            if (closeAccount != null)
            {
                fromDate = closeAccount.EndDate.Value.AddDays(1);

                closeQuery = _inventoryCostCloseItemBatchNoRepository.GetAll()
                                               //.Where(t => t.InventoryCostClose.CloseDate.Date == closeAccount.EndDate.Value.Date)
                                               .Where(t => t.InventoryCostClose.AccountCycleId == closeAccount.Id)
                                               .WhereIf(lots != null && lots.Any(), s => lots.Contains(s.LotId))
                                               .WhereIf(items != null && items.Any(), s => items.Contains(s.InventoryCostClose.ItemId))
                                               .AsNoTracking()
                                               .Select(s => new BatchNoItemBalanceOutput
                                               {
                                                   BatchNoId = s.BatchNoId,
                                                   ItemId = s.InventoryCostClose.ItemId,
                                                   LocationId = s.Lot.LocationId,
                                                   LotId = s.LotId,
                                                   Qty = s.Qty
                                               });

            }

            var itemReceiptBatch = from b in _itemReceiptItemBatchNoRepository.GetAll()
                                             .WhereIf(items != null && items.Any(), s => items.Contains(s.BatchNo.ItemId))
                                             .AsNoTracking()
                                   join ri in _itemReceiptItemRepository.GetAll()
                                              .WhereIf(lots != null && lots.Any(), s => lots.Contains(s.LotId.Value))
                                              .WhereIf(items != null && items.Any(), s => items.Contains(s.ItemId))
                                              .WhereIf(exceptIds != null && exceptIds.Any(), s => !exceptIds.Contains(s.ItemReceiptId))
                                              .AsNoTracking()

                                   on b.ItemReceiptItemId equals ri.Id

                                   join j in _journalRepository.GetAll()
                                            .Where(s => s.ItemReceiptId.HasValue)
                                            .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                            .Where(s => s.Date.Date <= date.Date)
                                            .Where(s => fromDate.Date <= s.Date.Date)
                                            .WhereIf(exceptIds != null && exceptIds.Any(), s => !exceptIds.Contains(s.ItemReceiptId.Value))
                                            .AsNoTracking()
                                   on ri.ItemReceiptId equals j.ItemReceiptId

                                   select new BatchNoItemBalanceOutput
                                   {
                                       BatchNoId = b.BatchNoId,
                                       ItemId = ri.ItemId,
                                       LocationId = ri.Lot.LocationId,
                                       LotId = ri.LotId.Value,
                                       Qty = b.Qty
                                   };

            var itemIssueBatch = from b in _itemIssueItemBatchNoRepository.GetAll()
                                             .WhereIf(items != null && items.Any(), s => items.Contains(s.BatchNo.ItemId))
                                             .AsNoTracking()

                                 join ri in _itemIssueItemRepository.GetAll()
                                            .WhereIf(lots != null && lots.Any(), s => lots.Contains(s.LotId.Value))
                                            .WhereIf(items != null && items.Any(), s => items.Contains(s.ItemId))
                                            .WhereIf(exceptIds != null && exceptIds.Any(), s => !exceptIds.Contains(s.ItemIssueId))
                                            .AsNoTracking()
                                 on b.ItemIssueItemId equals ri.Id

                                 join j in _journalRepository.GetAll()
                                          .Where(s => s.ItemIssueId.HasValue)
                                          .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                          .Where(s => s.Date.Date <= date.Date)
                                          .Where(s => fromDate.Date <= s.Date.Date)
                                          .WhereIf(exceptIds != null && exceptIds.Any(), s => !exceptIds.Contains(s.ItemIssueId.Value))
                                          .AsNoTracking()
                                 on ri.ItemIssueId equals j.ItemIssueId

                                 select new BatchNoItemBalanceOutput
                                 {
                                     BatchNoId = b.BatchNoId,
                                     ItemId = ri.ItemId,
                                     LocationId = ri.Lot.LocationId,
                                     LotId = ri.LotId.Value,
                                     Qty = -b.Qty
                                 };

            var itemIssueReturnBatch = from b in _itemIssueVendorCreditItemBatchNoRepository.GetAll()
                                            .WhereIf(items != null && items.Any(), s => items.Contains(s.BatchNo.ItemId))
                                            .AsNoTracking()

                                       join ri in _itemIssueVendorCreditItemRepository.GetAll()
                                                  .WhereIf(lots != null && lots.Any(), s => lots.Contains(s.LotId.Value))
                                                  .WhereIf(items != null && items.Any(), s => items.Contains(s.ItemId))
                                                  .WhereIf(exceptIds != null && exceptIds.Any(), s => !exceptIds.Contains(s.ItemIssueVendorCreditId))
                                                  .AsNoTracking()
                                       on b.ItemIssueVendorCreditItemId equals ri.Id

                                       join j in _journalRepository.GetAll()
                                                .Where(s => s.ItemIssueVendorCreditId.HasValue)
                                                .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                                .Where(s => s.Date.Date <= date.Date)
                                                .Where(s => fromDate.Date <= s.Date.Date)
                                                .WhereIf(exceptIds != null && exceptIds.Any(), s => !exceptIds.Contains(s.ItemIssueVendorCreditId.Value))
                                                .AsNoTracking()
                                       on ri.ItemIssueVendorCreditId equals j.ItemIssueVendorCreditId

                                       select new BatchNoItemBalanceOutput
                                       {
                                           BatchNoId = b.BatchNoId,
                                           ItemId = ri.ItemId,
                                           LocationId = ri.Lot.LocationId,
                                           LotId = ri.LotId.Value,
                                           Qty = -b.Qty
                                       };

            var itemReciptReturnBatch = from b in _itemReceiptCustomerCreditItemBatchNoRepository.GetAll()
                                           .WhereIf(items != null && items.Any(), s => items.Contains(s.BatchNo.ItemId))
                                           .AsNoTracking()

                                        join ri in _itemReceiptCustomerCreditItemRepository.GetAll()
                                                   .WhereIf(lots != null && lots.Any(), s => lots.Contains(s.LotId.Value))
                                                   .WhereIf(items != null && items.Any(), s => items.Contains(s.ItemId))
                                                   .WhereIf(exceptIds != null && exceptIds.Any(), s => !exceptIds.Contains(s.ItemReceiptCustomerCreditId))
                                                   .AsNoTracking()
                                        on b.ItemReceiptCustomerCreditItemId equals ri.Id

                                        join j in _journalRepository.GetAll()
                                                 .Where(s => s.ItemReceiptCustomerCreditId.HasValue)
                                                 .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                                 .Where(s => s.Date.Date <= date.Date)
                                                 .Where(s => fromDate.Date <= s.Date.Date)
                                                 .WhereIf(exceptIds != null && exceptIds.Any(), s => !exceptIds.Contains(s.ItemReceiptCustomerCreditId.Value))
                                                 .AsNoTracking()
                                        on ri.ItemReceiptCustomerCreditId equals j.ItemReceiptCustomerCreditId

                                        select new BatchNoItemBalanceOutput
                                        {
                                            BatchNoId = b.BatchNoId,
                                            ItemId = ri.ItemId,
                                            LocationId = ri.Lot.LocationId,
                                            LotId = ri.LotId.Value,
                                            Qty = b.Qty
                                        };


            var allQuery = itemReceiptBatch
                        .Concat(itemIssueBatch)
                        .Concat(itemIssueReturnBatch)
                        .Concat(itemReciptReturnBatch);

            if (closeQuery != null) allQuery = closeQuery.Concat(allQuery);

            var query = allQuery
                        .OrderBy(s => s.BatchNoId)
                        .GroupBy(g => new {
                            g.BatchNoId,
                            g.ItemId,
                            g.LocationId,
                            g.LotId,
                        })
                        .Select(s => new BatchNoItemBalanceOutput
                        {
                            BatchNoId = s.Key.BatchNoId,
                            ItemId = s.Key.ItemId,
                            LocationId = s.Key.LocationId,
                            LotId = s.Key.LotId,
                            Qty = s.Sum(t => t.Qty),
                        });

            var list = await query.ToListAsync();

            return list;

        }

        public async Task<List<BatchNoItemBalanceOutput>> GetItemBatchNoBalance(DateTime date, long locationId, List<Guid> items, List<Guid> exceptIds)
        {
            var closeAccount = await _accountCycleRepository.GetAll().Where(u => u.EndDate != null && u.EndDate.Value.Date <= date.Date).AsNoTracking()
                                                            .OrderByDescending(u => u.EndDate.Value).FirstOrDefaultAsync();

            var fromDate = DateTime.MinValue;
            IQueryable<BatchNoItemBalanceOutput> closeQuery = null;
            if (closeAccount != null)
            {
                fromDate = closeAccount.EndDate.Value.AddDays(1);

                closeQuery = _inventoryCostCloseItemBatchNoRepository.GetAll()
                                               //.Where(t => t.InventoryCostClose.CloseDate.Date == closeAccount.EndDate.Value.Date)
                                               .Where(t => t.InventoryCostClose.AccountCycleId == closeAccount.Id)
                                               .Where(s => s.Lot.LocationId == locationId)
                                               .WhereIf(items != null && items.Any(), s => items.Contains(s.InventoryCostClose.ItemId))
                                               .AsNoTracking()
                                               .Select(s => new BatchNoItemBalanceOutput
                                               {
                                                   BatchNoId = s.BatchNoId,
                                                   ItemId = s.InventoryCostClose.ItemId,
                                                   LocationId = s.Lot.LocationId,
                                                   LotId = s.LotId,
                                                   Qty = s.Qty
                                               });

            }

            var itemReceiptBatch = from b in _itemReceiptItemBatchNoRepository.GetAll()
                                             .WhereIf(items != null && items.Any(), s => items.Contains(s.BatchNo.ItemId))
                                             .AsNoTracking()
                                   join ri in _itemReceiptItemRepository.GetAll()
                                              .Where(s => s.LotId.HasValue && s.Lot.LocationId == locationId)
                                              .WhereIf(items != null && items.Any(), s => items.Contains(s.ItemId))
                                              .WhereIf(exceptIds != null && exceptIds.Any(), s => !exceptIds.Contains(s.ItemReceiptId))
                                              .AsNoTracking()

                                   on b.ItemReceiptItemId equals ri.Id

                                   join j in _journalRepository.GetAll()
                                            .Where(s => s.ItemReceiptId.HasValue)
                                            .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                            .Where(s => s.Date.Date <= date.Date)
                                            .Where(s => fromDate.Date <= s.Date.Date)
                                            .WhereIf(exceptIds != null && exceptIds.Any(), s => !exceptIds.Contains(s.ItemReceiptId.Value))
                                            .AsNoTracking()
                                   on ri.ItemReceiptId equals j.ItemReceiptId

                                   select new BatchNoItemBalanceOutput
                                   {
                                       BatchNoId = b.BatchNoId,
                                       ItemId = ri.ItemId,
                                       LocationId = ri.Lot.LocationId,
                                       LotId = ri.LotId.Value,
                                       Qty = b.Qty
                                   };

            var itemIssueBatch = from b in _itemIssueItemBatchNoRepository.GetAll()
                                             .WhereIf(items != null && items.Any(), s => items.Contains(s.BatchNo.ItemId))
                                             .AsNoTracking()

                                 join ri in _itemIssueItemRepository.GetAll()
                                             .Where(s => s.LotId.HasValue && s.Lot.LocationId == locationId)
                                            .WhereIf(items != null && items.Any(), s => items.Contains(s.ItemId))
                                            .WhereIf(exceptIds != null && exceptIds.Any(), s => !exceptIds.Contains(s.ItemIssueId))
                                            .AsNoTracking()
                                 on b.ItemIssueItemId equals ri.Id

                                 join j in _journalRepository.GetAll()
                                          .Where(s => s.ItemIssueId.HasValue)
                                          .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                          .Where(s => s.Date.Date <= date.Date)
                                          .Where(s => fromDate.Date <= s.Date.Date)
                                          .WhereIf(exceptIds != null && exceptIds.Any(), s => !exceptIds.Contains(s.ItemIssueId.Value))
                                          .AsNoTracking()
                                 on ri.ItemIssueId equals j.ItemIssueId

                                 select new BatchNoItemBalanceOutput
                                 {
                                     BatchNoId = b.BatchNoId,
                                     ItemId = ri.ItemId,
                                     LocationId = ri.Lot.LocationId,
                                     LotId = ri.LotId.Value,
                                     Qty = -b.Qty
                                 };

            var itemIssueReturnBatch = from b in _itemIssueVendorCreditItemBatchNoRepository.GetAll()
                                            .WhereIf(items != null && items.Any(), s => items.Contains(s.BatchNo.ItemId))
                                            .AsNoTracking()

                                       join ri in _itemIssueVendorCreditItemRepository.GetAll()
                                                   .Where(s => s.LotId.HasValue && s.Lot.LocationId == locationId)
                                                  .WhereIf(items != null && items.Any(), s => items.Contains(s.ItemId))
                                                  .WhereIf(exceptIds != null && exceptIds.Any(), s => !exceptIds.Contains(s.ItemIssueVendorCreditId))
                                                  .AsNoTracking()
                                       on b.ItemIssueVendorCreditItemId equals ri.Id

                                       join j in _journalRepository.GetAll()
                                                .Where(s => s.ItemIssueVendorCreditId.HasValue)
                                                .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                                .Where(s => s.Date.Date <= date.Date)
                                                .Where(s => fromDate.Date <= s.Date.Date)
                                                .WhereIf(exceptIds != null && exceptIds.Any(), s => !exceptIds.Contains(s.ItemIssueVendorCreditId.Value))
                                                .AsNoTracking()
                                       on ri.ItemIssueVendorCreditId equals j.ItemIssueVendorCreditId

                                       select new BatchNoItemBalanceOutput
                                       {
                                           BatchNoId = b.BatchNoId,
                                           ItemId = ri.ItemId,
                                           LocationId = ri.Lot.LocationId,
                                           LotId = ri.LotId.Value,
                                           Qty = -b.Qty
                                       };

            var itemReciptReturnBatch = from b in _itemReceiptCustomerCreditItemBatchNoRepository.GetAll()
                                           .WhereIf(items != null && items.Any(), s => items.Contains(s.BatchNo.ItemId))
                                           .AsNoTracking()

                                        join ri in _itemReceiptCustomerCreditItemRepository.GetAll()
                                                   .Where(s => s.LotId.HasValue && s.Lot.LocationId == locationId)
                                                   .WhereIf(items != null && items.Any(), s => items.Contains(s.ItemId))
                                                   .WhereIf(exceptIds != null && exceptIds.Any(), s => !exceptIds.Contains(s.ItemReceiptCustomerCreditId))
                                                   .AsNoTracking()
                                        on b.ItemReceiptCustomerCreditItemId equals ri.Id

                                        join j in _journalRepository.GetAll()
                                                 .Where(s => s.ItemReceiptCustomerCreditId.HasValue)
                                                 .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                                 .Where(s => s.Date.Date <= date.Date)
                                                 .Where(s => fromDate.Date <= s.Date.Date)
                                                 .WhereIf(exceptIds != null && exceptIds.Any(), s => !exceptIds.Contains(s.ItemReceiptCustomerCreditId.Value))
                                                 .AsNoTracking()
                                        on ri.ItemReceiptCustomerCreditId equals j.ItemReceiptCustomerCreditId

                                        select new BatchNoItemBalanceOutput
                                        {
                                            BatchNoId = b.BatchNoId,
                                            ItemId = ri.ItemId,
                                            LocationId = ri.Lot.LocationId,
                                            LotId = ri.LotId.Value,
                                            Qty = b.Qty
                                        };


            var allQuery = itemReceiptBatch
                        .Concat(itemIssueBatch)
                        .Concat(itemIssueReturnBatch)
                        .Concat(itemReciptReturnBatch);

            if (closeQuery != null) allQuery = closeQuery.Concat(allQuery);

            var query = allQuery
                        .OrderBy(s => s.BatchNoId)
                        .GroupBy(g => new {
                            g.BatchNoId,
                            g.ItemId,
                            g.LocationId,
                            g.LotId,
                        })
                        .Select(s => new BatchNoItemBalanceOutput
                        {
                            BatchNoId = s.Key.BatchNoId,
                            ItemId = s.Key.ItemId,
                            LocationId = s.Key.LocationId,
                            LotId = s.Key.LotId,
                            Qty = s.Sum(t => t.Qty),
                        });

            var list = await query.ToListAsync();

            return list;

        }

        public async Task<IQueryable<InventoryCostItem>> GetItemsBalance(List<Guid> itemIds,
                              DateTime toDate,
                              List<long> locations,
                              List<Guid> exceptIds = null)
        {

            var periodCycles = await GetCloseCyleQuery().ToListAsync();

            var previousCycle = periodCycles.Where(u => u.EndDate != null &&
                                                        u.EndDate.Value.Date <= toDate.Date)
                                                        .OrderByDescending(u => u.EndDate.Value).FirstOrDefault();

            var currentCycle = periodCycles.Where(u => u.StartDate.Date <= toDate.Date && (u.EndDate == null || toDate.Date <= u.EndDate.Value.Date))
                                            .OrderByDescending(u => u.StartDate).FirstOrDefault();

            var inventoryCostClose = _inventoryCostCloseItemRepository.GetAll()
                                     //.Where(t => previousCycle != null && t.InventoryCostClose.CloseDate.Date == previousCycle.EndDate.Value.Date)
                                     .Where(t => previousCycle != null && t.InventoryCostClose.AccountCycleId == previousCycle.Id)
                                     .WhereIf(!itemIds.IsNullOrEmpty(), s => itemIds.Contains(s.InventoryCostClose.ItemId))
                                     .WhereIf(!locations.IsNullOrEmpty(), s => s.LotId.HasValue && locations.Contains(s.Lot.LocationId))
                                     .AsNoTracking()
                                      .Select(s => new ItemBalanceDto
                                      {
                                          IsPurchase = true,
                                          ItemId = s.InventoryCostClose.ItemId,
                                          Qty = s.Qty,
                                          LotId = s.LotId.Value,
                                          LotName = s.LotId.HasValue ? s.Lot.LotName : ""
                                      });


            var fromDate = previousCycle == null ? (DateTime?)null : previousCycle.EndDate.Value.Date;

            var inventoryTransactionQuery = GetInventoryItemBalanceQuery(itemIds, fromDate, toDate, locations, exceptIds);

            var inventoryTransactionItemQuery = inventoryTransactionQuery.Concat(inventoryCostClose)
                                    .OrderBy(s => s.ItemId)
                                    .ThenBy(s => s.LotId)
                                    .GroupBy(s => new { s.ItemId, s.LotId, s.LotName })
                                    .Select(r => new InventoryCostItem()
                                    {
                                        LotId = r.Key.LotId,
                                        LotName = r.Key.LotName,
                                        Id = r.Key.ItemId,
                                        QtyOnHand = r.Sum(t => t.Qty)
                                    });

            return inventoryTransactionItemQuery;
        }

        public IQueryable<ItemBalanceDto> GetInventoryItemBalanceQuery(
                                  List<Guid> itemIds,
                                  DateTime? fromDate, DateTime? toDate,
                                  List<long> location,
                                  List<Guid> exceptIds = null)
        {

            var itemReceiptItemQuery = from Ir in _itemReceiptItemRepository.GetAll()
                                                    .WhereIf(location != null && location.Count > 0, t => location.Contains(t.Lot.LocationId))
                                                    .WhereIf(!exceptIds.IsNullOrEmpty(), t => !exceptIds.Contains(t.ItemReceiptId))
                                                    .WhereIf(!itemIds.IsNullOrEmpty(), s => itemIds.Contains(s.ItemId))
                                                    .AsNoTracking()
                                       join j in _journalRepository.GetAll()
                                                   .Where(s => s.Status == TransactionStatus.Publish)
                                                   .Where(s => s.ItemReceiptId.HasValue)
                                                   .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                                   .WhereIf(toDate != null, u => u.Date.Date <= toDate.Value.Date)
                                                   .AsNoTracking()
                                       on Ir.ItemReceiptId equals j.ItemReceiptId
                                       select new ItemBalanceDto
                                       {
                                           IsPurchase = true,
                                           ItemId = Ir.ItemId,
                                           Qty = Ir.Qty,
                                           LotId = Ir.LotId ?? 0,
                                           LotName = Ir.LotId.HasValue ? Ir.Lot.LotName : ""
                                       };

            var itemReceiptCustomerCreditItemQuery = from IrC in _itemReceiptCustomerCreditItemRepository.GetAll()
                                                                .WhereIf(location != null && location.Count > 0, t => location.Contains(t.Lot.LocationId))
                                                                .WhereIf(!exceptIds.IsNullOrEmpty(), u => !exceptIds.Contains(u.ItemReceiptCustomerCreditId))
                                                                .WhereIf(!itemIds.IsNullOrEmpty(), s => itemIds.Contains(s.ItemId))
                                                                .AsNoTracking()
                                                     join J in _journalRepository.GetAll()
                                                                 .Where(s => s.Status == TransactionStatus.Publish)
                                                                 .Where(s => s.ItemReceiptCustomerCreditId.HasValue)
                                                                 .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                                                 .WhereIf(toDate != null, u => u.Date.Date <= toDate.Value.Date)
                                                                 .AsNoTracking()
                                                     on IrC.ItemReceiptCustomerCreditId equals J.ItemReceiptCustomerCreditId
                                                     select new ItemBalanceDto
                                                     {
                                                         IsPurchase = true,
                                                         ItemId = IrC.ItemId,
                                                         Qty = IrC.Qty,
                                                         LotId = IrC.LotId ?? 0,
                                                         LotName = IrC.LotId.HasValue ? IrC.Lot.LotName : ""
                                                     };

            var itemReceiptResult = itemReceiptItemQuery.Concat(itemReceiptCustomerCreditItemQuery);

            var itemIssueItemQuery = from isue in _itemIssueItemRepository.GetAll()
                                                  .WhereIf(location != null && location.Count > 0, t => location.Contains(t.Lot.LocationId))
                                                  .WhereIf(!exceptIds.IsNullOrEmpty(), u => !exceptIds.Contains(u.ItemIssueId))
                                                   .WhereIf(!itemIds.IsNullOrEmpty(), s => itemIds.Contains(s.ItemId))
                                                  .AsNoTracking()
                                     join j in _journalRepository.GetAll()
                                                 .Where(s => s.Status == TransactionStatus.Publish)
                                                 .Where(s => s.ItemIssueId.HasValue)
                                                 .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                                 .WhereIf(toDate != null, u => u.Date.Date <= toDate.Value.Date)
                                                 .AsNoTracking()
                                     on isue.ItemIssueId equals j.ItemIssueId
                                     select new ItemBalanceDto
                                     {
                                         IsPurchase = false,
                                         ItemId = isue.ItemId,
                                         Qty = (isue.Qty * -1),
                                         LotId = isue.LotId ?? 0,
                                         LotName = isue.LotId.HasValue ? isue.Lot.LotName : ""
                                     };

            var itemIssueVendorCreditItemQuery = from isv in _itemIssueVendorCreditItemRepository.GetAll()
                                                            .WhereIf(location != null && location.Count > 0, t => location.Contains(t.Lot.LocationId))
                                                            .WhereIf(!exceptIds.IsNullOrEmpty(), u => !exceptIds.Contains(u.ItemIssueVendorCreditId))
                                                            .WhereIf(!itemIds.IsNullOrEmpty(), s => itemIds.Contains(s.ItemId))
                                                            .AsNoTracking()
                                                 join j in _journalRepository.GetAll()
                                                             .Where(s => s.Status == TransactionStatus.Publish)
                                                             .Where(s => s.ItemIssueVendorCreditId.HasValue)
                                                             .Where(t => fromDate == null || t.Date.Date > fromDate.Value)
                                                             .WhereIf(toDate != null, u => u.Date.Date <= toDate.Value.Date)
                                                             .AsNoTracking()
                                                 on isv.ItemIssueVendorCreditId equals j.ItemIssueVendorCreditId
                                                 select new ItemBalanceDto
                                                 {
                                                     IsPurchase = false,
                                                     ItemId = isv.ItemId,
                                                     Qty = (isv.Qty * -1),
                                                     LotId = isv.LotId ?? 0,
                                                     LotName = isv.LotId.HasValue ? isv.Lot.LotName : ""
                                                 };
            var itemIssueResult = itemIssueItemQuery.Concat(itemIssueVendorCreditItemQuery);

            var inventoryTransactionItemQuery = itemReceiptResult.Concat(itemIssueResult);

            return inventoryTransactionItemQuery;
        }

    }
}
