using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.ChartOfAccounts;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.Customers;
using CorarlERP.Customers.Dto;
using CorarlERP.Invoices;
using CorarlERP.ItemIssues.Dto;
using CorarlERP.Items;
using CorarlERP.Items.Dto;
using CorarlERP.Journals;
using CorarlERP.Journals.Dto;
using CorarlERP.SaleOrders;
using CorarlERP.SaleOrders.Dto;
using Microsoft.EntityFrameworkCore;
using static CorarlERP.enumStatus.EnumStatus;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using CorarlERP.Taxes.Dto;
using Abp.Authorization;
using CorarlERP.Authorization;
using Microsoft.EntityFrameworkCore.Internal;
using CorarlERP.ItemIssueVendorCredits;
using CorarlERP.Vendors;
using CorarlERP.Vendors.Dto;
using CorarlERP.ItemReceipts;
using CorarlERP.ItemReceiptCustomerCredits;
using CorarlERP.TransferOrders;
using CorarlERP.Inventories;
using CorarlERP.Productions;
using CorarlERP.AutoSequences;
using CorarlERP.Authorization.Users;
using CorarlERP.Dto;
using CorarlERP.Lots.Dto;
using CorarlERP.Lots;
using CorarlERP.Locations.Dto;
using CorarlERP.UserGroups;
using CorarlERP.AccountCycles;
using CorarlERP.CustomerCredits;
using CorarlERP.Reports;
using CorarlERP.Locks;
using CorarlERP.Common.Dto;
using CorarlERP.InventoryTransactionItems;
using System.Security.Policy;
using CorarlERP.InventoryCalculationJobSchedules;
using Hangfire.States;
using CorarlERP.InventoryCalculationItems;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using CorarlERP.BatchNos;
using Amazon.Runtime.Internal.Util;
using CorarlERP.ItemReceipts.Dto;
using CorarlERP.JournalTransactionTypes;
using CorarlERP.Features;
using CorarlERP.DeliverySchedules;
using CorarlERP.DeliverySchedules.Dto;

namespace CorarlERP.ItemIssues
{
    public class ItemIssueAppService : ReportBaseClass, IItemIssueAppService
    {

        private readonly IRepository<ItemReceiptItem, Guid> _itemReceiptItemRepository;
        private readonly IRepository<ItemReceipt, Guid> _itemReceiptRepository;
        private readonly IRepository<ItemReceiptItemCustomerCredit, Guid> _itemReceiptCustomerCreditItemRepository;
        private readonly IRepository<ItemReceiptCustomerCredit, Guid> _itemReceiptCustomerCreditRepository;
        private readonly IRepository<AccountCycle, long> _accountCycleRepository;
        private readonly IItemManager _itemManager;
        private readonly IRepository<Item, Guid> _itemRepository;
        private readonly IRepository<ItemIssueVendorCredit, Guid> _itemIssueVendorCreditRepository;
        private readonly IRepository<ItemIssueVendorCreditItem, Guid> _itemIssueVendorCreditItemRepository;
        private readonly IInvoiceItemManager _invoiceItemManager;
        private readonly IRepository<InvoiceItem, Guid> _invoiceItemRepository;
        private readonly IInvoiceManager _invoiceManager;
        private readonly IRepository<Invoice, Guid> _invoiceRepository;
        private readonly ICustomerManager _customerItemManager;
        private readonly IRepository<Customer, Guid> _customerRepository;
        private readonly ISaleOrderItemManager _saleOrderItemManager;
        private readonly IRepository<SaleOrderItem, Guid> _saleOrderItemRepository;
        private readonly ISaleOrderManager _saleOrderManager;
        private readonly IRepository<SaleOrder, Guid> _saleOrderRepository;
        private readonly IItemIssueItemManager _itemIssueItemManager;
        private readonly IItemIssueManager _itemIssueManager;
        private readonly IRepository<ItemIssue, Guid> _itemIssueRepository;
        private readonly IRepository<ItemIssueItem, Guid> _itemIssueItemRepository;
        private readonly IJournalManager _journalManager;
        private readonly IRepository<Journal, Guid> _journalRepository;
        private readonly IRepository<VendorCredit.VendorCredit, Guid> _vendorCreditRepository;
        private readonly IJournalItemManager _journalItemManager;
        private readonly IRepository<JournalItem, Guid> _journalItemRepository;
        private readonly IChartOfAccountManager _chartOfAccountManager;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;
        private readonly IRepository<Vendor, Guid> _vendorRepository;

        private readonly IRepository<CustomerCreditDetail, Guid> _cusotmerCreditItemRepository;
        private readonly IRepository<CustomerCredit, Guid> _cusotmerCreditRepository;
        private readonly ITransferOrderManager _transferOrderManager;
        private readonly IRepository<TransferOrder, Guid> _transferOrderRepository;

        private readonly IProductionManager _productionOrderManager;
        private readonly IRepository<Production, Guid> _productionOrderRepository;
        private readonly IRawMaterialItemManager _rawMaterialItemManager;
        private readonly IRepository<RawMaterialItems, Guid> _rawMaterialItemsRepository;
        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly IRepository<TransferOrderItem, Guid> _transferOrderItemRepository;
        private readonly IInventoryManager _inventoryManager;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Lot, long> _lotRepository;
        private readonly IRepository<Lock, long> _lockRepository;
        private readonly ICorarlRepository<InventoryTransactionItem, Guid> _inventoryTransactionItemRepository;
        private readonly IInventoryCalculationItemManager _inventoryCalculationItemManager;
        private readonly ICorarlRepository<BatchNo, Guid> _batchNoRepository;
        private readonly ICorarlRepository<ItemIssueItemBatchNo, Guid> _itemIssueItemBatchNoRepository;
        private readonly ICorarlRepository<CustomerCreditItemBatchNo, Guid> _customerCreditBatchNoRepository;
        private readonly ICorarlRepository<ItemReceiptCustomerCreditItemBatchNo, Guid> _itemReceiptCustomerCreditItemBatchNoRepository;
        private readonly IJournalTransactionTypeManager _journalTransactionTypeManager;
        private readonly ICorarlRepository<DeliveryScheduleItem, Guid> _deliveryScheduleItemRepository;
        private readonly ICorarlRepository<DeliverySchedule, Guid> _deliveryScheduleRepository;
        public ItemIssueAppService(
            ICorarlRepository<InventoryTransactionItem, Guid> inventoryTransactionItemRepository,
            IRepository<Lot, long> lotRepository,
            IJournalManager journalManager,
            IJournalItemManager journalItemManager,
            IChartOfAccountManager chartOfAccountManager,
            ItemIssueManager itemIssueManager,
            ItemIssueItemManager itemIssueItemManager,
            ItemManager itemManager,
            SaleOrderItemManager saleOrderItemManager,
            SaleOrderManager saleOrderManager,
            ICustomerManager customerManager,
            InvoiceItemManager invoiceItemManager,
            InvoiceManager invoiceManager,
            IRepository<InvoiceItem, Guid> invoiceItemRepository,
            IRepository<Item, Guid> itemRepository,
            IRepository<Invoice, Guid> invoiceRepository,
            IRepository<Customer, Guid> customerRepository,
            IRepository<Journal, Guid> journalRepository,
            IRepository<JournalItem, Guid> journalItemRepository,
            IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
            IRepository<ItemIssue, Guid> itemIssueRepository,
            IRepository<ItemIssueItem, Guid> itemIssueItemRepository,
            IRepository<SaleOrderItem, Guid> saleOrderItemRepository,
            IRepository<SaleOrder, Guid> saleOrderRepository,
            IRepository<ItemIssueVendorCredit, Guid> itemIssueVendorCreditRepository,
            IRepository<ItemIssueVendorCreditItem, Guid> itemIssueVendorCreditItemRepository,
            IRepository<Vendor, Guid> vendorRepository,
            IRepository<ItemReceiptItem, Guid> itemReceiptItemRepository,
            IRepository<ItemReceipt, Guid> itemReceiptRepository,
            IRepository<ItemReceiptItemCustomerCredit, Guid> itemReceiptCustomerCreditItemRepository,
            ITransferOrderManager transferOrderManager,
            IRepository<TransferOrder, Guid> transferOrderRepository,
            IRepository<TransferOrderItem, Guid> transferOrderItemRepository,
            IRepository<VendorCredit.VendorCredit, Guid> vendorCreditRepository,
            IRepository<CustomerCredit, Guid> cusotmerCreditRepository,
            IProductionManager productionOrderManager,
            IRepository<Production, Guid> productionOrderRepository,
            IRawMaterialItemManager rawMaterialItemManager,
            IRepository<RawMaterialItems, Guid> rawMaterialItemsRepository,
            IRepository<AutoSequence, Guid> autoSequenceRepository,
            IAutoSequenceManager autoSequenceManager,
            IInventoryManager inventoryManager,
            IRepository<User, long> userRepository,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            IRepository<Locations.Location, long> locationRepository,
            IRepository<AccountCycle, long> accountCycleRepository,
            IRepository<CustomerCreditDetail, Guid> cusotmerCreditItemRepository,
            IRepository<ItemReceiptCustomerCredit, Guid> itemReceiptCustomerCreditRepository,
            IInventoryCalculationItemManager inventoryCalculationItemManager,
            IRepository<Lock, long> lockRepository,
            ICorarlRepository<BatchNo, Guid> batchNoRepository,
            ICorarlRepository<ItemIssueItemBatchNo, Guid> itemIssueItemBatchNoRepository,
            ICorarlRepository<CustomerCreditItemBatchNo, Guid> customerCreditBatchNoRepository,
            ICorarlRepository<ItemReceiptCustomerCreditItemBatchNo, Guid> itemReceiptCustomerCreditItemBatchNoRepository,
            IJournalTransactionTypeManager journalTransactionTypeManager,
            AppFolders appFolders,
            ICorarlRepository<DeliveryScheduleItem, Guid> deliveryScheduleItemRepository,
            ICorarlRepository<DeliverySchedule, Guid> deliveryScheduleRepository
        ) : base(accountCycleRepository, appFolders, userGroupMemberRepository, locationRepository)
        {
            _itemReceiptCustomerCreditRepository = itemReceiptCustomerCreditRepository;
            _cusotmerCreditRepository = cusotmerCreditRepository;
            _cusotmerCreditItemRepository = cusotmerCreditItemRepository;
            _accountCycleRepository = accountCycleRepository;
            _lotRepository = lotRepository;
            _userRepository = userRepository;
            _vendorCreditRepository = vendorCreditRepository;
            _journalManager = journalManager;
            _journalManager.SetJournalType(JournalType.ItemIssueSale);
            _journalRepository = journalRepository;
            _journalItemManager = journalItemManager;
            _journalItemRepository = journalItemRepository;
            _chartOfAccountManager = chartOfAccountManager;
            _chartOfAccountRepository = chartOfAccountRepository;
            _itemIssueItemManager = itemIssueItemManager;
            _itemIssueItemRepository = itemIssueItemRepository;
            _itemIssueManager = itemIssueManager;
            _itemIssueRepository = itemIssueRepository;
            _saleOrderItemRepository = saleOrderItemRepository;
            _saleOrderItemManager = saleOrderItemManager;
            _saleOrderRepository = saleOrderRepository;
            _saleOrderManager = saleOrderManager;
            _customerRepository = customerRepository;
            _customerItemManager = customerManager;
            _invoiceItemRepository = invoiceItemRepository;
            _invoiceItemManager = invoiceItemManager;
            _invoiceRepository = invoiceRepository;
            _invoiceManager = invoiceManager;
            _itemRepository = itemRepository;
            _itemManager = itemManager;
            _vendorRepository = vendorRepository;
            _itemIssueVendorCreditRepository = itemIssueVendorCreditRepository;
            _itemIssueVendorCreditItemRepository = itemIssueVendorCreditItemRepository;
            _itemReceiptItemRepository = itemReceiptItemRepository;
            _itemReceiptRepository = itemReceiptRepository;
            _itemReceiptCustomerCreditItemRepository = itemReceiptCustomerCreditItemRepository;
            _itemRepository = itemRepository;
            _transferOrderManager = transferOrderManager;
            _transferOrderRepository = transferOrderRepository;
            _transferOrderItemRepository = transferOrderItemRepository;
            _inventoryManager = inventoryManager;
            _productionOrderManager = productionOrderManager;
            _productionOrderRepository = productionOrderRepository;
            _rawMaterialItemManager = rawMaterialItemManager;
            _rawMaterialItemsRepository = rawMaterialItemsRepository;
            _autoSequenceManager = autoSequenceManager;
            _autoSequenceRepository = autoSequenceRepository;
            _lockRepository = lockRepository;
            _inventoryTransactionItemRepository = inventoryTransactionItemRepository;
            _inventoryCalculationItemManager = inventoryCalculationItemManager;
            _batchNoRepository = batchNoRepository;
            _itemIssueItemBatchNoRepository = itemIssueItemBatchNoRepository;
            _customerCreditBatchNoRepository = customerCreditBatchNoRepository;
            _itemReceiptCustomerCreditItemBatchNoRepository = itemReceiptCustomerCreditItemBatchNoRepository;
            _journalTransactionTypeManager = journalTransactionTypeManager;
            _deliveryScheduleRepository = deliveryScheduleRepository;
            _deliveryScheduleItemRepository = deliveryScheduleItemRepository;
        }


        private async Task ValidateApplyOrderItems(CreateItemIssueInput input)
        {

            var itemApplyOrder = input.Items
                                 .Where(s => s.SaleOrderItemId.HasValue)
                                 .GroupBy(s => s.SaleOrderItemId);

            if (!itemApplyOrder.Any()) return;

            var orderItemIds = itemApplyOrder.Select(s => s.Key.Value).ToList();
            var exceptInvoiceItemIds = input.Items
                                        .Where(s => s.SaleOrderItemId.HasValue)
                                        .Where(s => s.InvoiceItemId.HasValue)
                                        .Select(s => s.InvoiceItemId.Value)
                                        .ToList();

            var exceptIssueItemIds = input.Items
                                        .Where(s => s.SaleOrderItemId.HasValue)
                                        .Where(s => s.Id.HasValue)
                                        .Select(s => s.Id.Value)
                                        .ToList();

            var invoiceItemQuery = from iv in _invoiceItemRepository.GetAll()
                                             .Where(s => s.OrderItemId.HasValue)
                                             .Where(s => orderItemIds.Contains(s.OrderItemId.Value))
                                             .Where(s => !exceptInvoiceItemIds.Contains(s.Id))
                                             .AsNoTracking()
                                   join ri in _itemReceiptCustomerCreditItemRepository.GetAll()
                                              .Where(s => s.ItemIssueSaleItemId.HasValue)
                                              .AsNoTracking()
                                   on iv.ItemIssueItemId equals ri.ItemIssueSaleItemId
                                   into r1

                                   join ci in _itemReceiptCustomerCreditItemRepository.GetAll()
                                              .Where(s => s.CustomerCreditItemId.HasValue)
                                              .AsNoTracking()
                                   on iv.ItemIssueItemId equals ci.CustomerCreditItem.ItemIssueSaleItemId
                                   into r2

                                   let rq1 = r1 == null ? 0 : r1.Sum(s => s.Qty)
                                   let rq2 = r2 == null ? 0 : r2.Sum(s => s.Qty)

                                   select new
                                   {
                                       OrderItemId = iv.OrderItemId,
                                       Qty = iv.Qty,
                                       ReturnQty = rq1 + rq2
                                   };

            var issueItemQuery = from iv in _itemIssueItemRepository.GetAll()
                                             .Where(s => s.SaleOrderItemId.HasValue)
                                             .Where(s => orderItemIds.Contains(s.SaleOrderItemId.Value))
                                             .Where(s => !exceptIssueItemIds.Contains(s.Id))
                                             .AsNoTracking()
                                 join ri in _itemReceiptCustomerCreditItemRepository.GetAll()
                                            .Where(s => s.ItemIssueSaleItemId.HasValue)
                                            .AsNoTracking()
                                 on iv.Id equals ri.ItemIssueSaleItemId
                                 into r1

                                 join ci in _itemReceiptCustomerCreditItemRepository.GetAll()
                                            .Where(s => s.CustomerCreditItemId.HasValue)
                                            .AsNoTracking()
                                 on iv.Id equals ci.CustomerCreditItem.ItemIssueSaleItemId
                                 into r2

                                 let rq1 = r1 == null ? 0 : r1.Sum(s => s.Qty)
                                 let rq2 = r2 == null ? 0 : r2.Sum(s => s.Qty)

                                 select new
                                 {
                                     SaleOrderItemId = iv.SaleOrderItemId,
                                     Qty = iv.Qty,
                                     ReturnQty = rq1 + rq2
                                 };

            var deliveryItemQuery = from dv in _deliveryScheduleItemRepository.GetAll()
                                    .Where(s => s.DeliverySchedule.Status == TransactionStatus.Publish || s.DeliverySchedule.Status == TransactionStatus.Close)
                                    .Where(s => s.SaleOrderItemId.HasValue)
                                    .Where(s => orderItemIds.Contains(s.SaleOrderItemId.Value))
                                    .AsNoTracking()
                                    select new
                                    {
                                        OrderItemId = dv.SaleOrderItemId,
                                        Qty = dv.Qty,
                                        ReturnQty = 0
                                    };

            var saleOrderQuery = from oi in _saleOrderItemRepository.GetAll()
                                            .Where(s => orderItemIds.Contains(s.Id))
                                            .AsNoTracking()
                                 join iv in invoiceItemQuery
                                 on oi.Id equals iv.OrderItemId
                                 into ivItems

                                 join si in issueItemQuery
                                 on oi.Id equals si.SaleOrderItemId
                                 into siItems

                                 join di in deliveryItemQuery on oi.Id equals di.OrderItemId
                                  into diItems
                                 let q1 = ivItems == null ? 0 : ivItems.Sum(s => s.Qty - s.ReturnQty)
                                 let q2 = siItems == null ? 0 : siItems.Sum(s => s.Qty - s.ReturnQty)
                                 let q3 = diItems == null ? 0 : diItems.Sum(s => s.Qty - s.ReturnQty)
                                 select new
                                 {
                                     oi.Id,
                                     oi.Qty,
                                     issueQty = q1 + q2 + q3
                                 };

            var saleOrderItems = await saleOrderQuery.ToListAsync();


            foreach (var g in itemApplyOrder)
            {
                var orderItem = saleOrderItems.FirstOrDefault(s => s.Id == g.Key);
                if (orderItem == null) throw new UserFriendlyException("RecordNotFound");

                var remainQty = orderItem.Qty - orderItem.issueQty;

                foreach (var i in g)
                {
                    if (i.Qty > remainQty) throw new UserFriendlyException(L("RemainQtyIsLessThanInvoiceQtyCannotApply", remainQty, i.Qty));
                    remainQty -= i.Qty;
                }
            }
        }



        private async Task ValidateApplyDeliveryScheduleItems(CreateItemIssueInput input)
        {

            var itemApplyDelivery = input.Items
                                 .Where(s => s.DeliveryScheduleItemId.HasValue)
                                 .GroupBy(s => s.DeliveryScheduleItemId);

            if (!itemApplyDelivery.Any()) return;

            var deliveryItemIds = itemApplyDelivery.Select(s => s.Key.Value).ToList();
            var exceptInvoiceItemIds = input.Items
                                        .Where(s => s.DeliveryScheduleItemId.HasValue)
                                        .Where(s => s.InvoiceItemId.HasValue)
                                        .Select(s => s.InvoiceItemId.Value)
                                        .ToList();

            var exceptIssueItemIds = input.Items
                                        .Where(s => s.DeliveryScheduleItemId.HasValue)
                                        .Where(s => s.Id.HasValue)
                                        .Select(s => s.Id.Value)
                                        .ToList();

            var invoiceItemQuery = from iv in _invoiceItemRepository.GetAll()
                                             .Where(s => s.DeliverySchedulItemId.HasValue)
                                             .Where(s => deliveryItemIds.Contains(s.DeliverySchedulItemId.Value))
                                             .Where(s => !exceptInvoiceItemIds.Contains(s.Id))
                                             .AsNoTracking()
                                   join ri in _itemReceiptCustomerCreditItemRepository.GetAll()
                                              .Where(s => s.ItemIssueSaleItemId.HasValue)
                                              .AsNoTracking()
                                   on iv.ItemIssueItemId equals ri.ItemIssueSaleItemId
                                   into r1

                                   join ci in _itemReceiptCustomerCreditItemRepository.GetAll()
                                              .Where(s => s.CustomerCreditItemId.HasValue)
                                              .AsNoTracking()
                                   on iv.ItemIssueItemId equals ci.CustomerCreditItem.ItemIssueSaleItemId
                                   into r2

                                   let rq1 = r1 == null ? 0 : r1.Sum(s => s.Qty)
                                   let rq2 = r2 == null ? 0 : r2.Sum(s => s.Qty)

                                   select new
                                   {
                                       DeliverySchedulItemId = iv.DeliverySchedulItemId,
                                       Qty = iv.Qty,
                                       ReturnQty = rq1 + rq2
                                   };

            var issueItemQuery = from iv in _itemIssueItemRepository.GetAll()
                                             .Where(s => s.DeliverySchedulItemId.HasValue)
                                             .Where(s => deliveryItemIds.Contains(s.DeliverySchedulItemId.Value))
                                             .Where(s => !exceptIssueItemIds.Contains(s.Id))
                                             .AsNoTracking()
                                 join ri in _itemReceiptCustomerCreditItemRepository.GetAll()
                                            .Where(s => s.ItemIssueSaleItemId.HasValue)
                                            .AsNoTracking()
                                 on iv.Id equals ri.ItemIssueSaleItemId
                                 into r1

                                 join ci in _itemReceiptCustomerCreditItemRepository.GetAll()
                                            .Where(s => s.CustomerCreditItemId.HasValue)
                                            .AsNoTracking()
                                 on iv.Id equals ci.CustomerCreditItem.ItemIssueSaleItemId
                                 into r2

                                 let rq1 = r1 == null ? 0 : r1.Sum(s => s.Qty)
                                 let rq2 = r2 == null ? 0 : r2.Sum(s => s.Qty)

                                 select new
                                 {
                                     DeliverySchedulItemId = iv.DeliverySchedulItemId,
                                     Qty = iv.Qty,
                                     ReturnQty = rq1 + rq2
                                 };



            var deliveryScheduleQuery = from oi in _deliveryScheduleItemRepository.GetAll()
                                            .Where(s => deliveryItemIds.Contains(s.Id))
                                            .AsNoTracking()
                                        join iv in invoiceItemQuery
                                        on oi.Id equals iv.DeliverySchedulItemId
                                        into ivItems

                                        join si in issueItemQuery
                                        on oi.Id equals si.DeliverySchedulItemId
                                        into siItems


                                        let q1 = ivItems == null ? 0 : ivItems.Sum(s => s.Qty - s.ReturnQty)
                                        let q2 = siItems == null ? 0 : siItems.Sum(s => s.Qty - s.ReturnQty)

                                        select new
                                        {
                                            oi.Id,
                                            oi.Qty,
                                            issueQty = q1 + q2
                                        };

            var deliveryScheduleItems = await deliveryScheduleQuery.ToListAsync();


            foreach (var g in itemApplyDelivery)
            {
                var orderItem = deliveryScheduleItems.FirstOrDefault(s => s.Id == g.Key);
                if (orderItem == null) throw new UserFriendlyException("RecordNotFound");

                var remainQty = orderItem.Qty - orderItem.issueQty;

                foreach (var i in g)
                {
                    if (i.Qty > remainQty) throw new UserFriendlyException(L("RemainQtyIsLessThanInvoiceQtyCannotApply", remainQty, i.Qty));
                    remainQty -= i.Qty;
                }
            }
        }




        private async Task ValidateBatchNo(CreateItemIssueInput input)
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
            var lots = itemUseBatchs.GroupBy(s => s.LotId).Select(s => s.Key.Value).ToList();
            var exceptIds = new List<Guid>();
            if (input is UpdateItemIssueInput)
            {
                exceptIds.Add((input as UpdateItemIssueInput).Id);
            }

            var batchBalanceItems = await _inventoryManager.GetItemBatchNoBalance(itemIds, lots, batchNoIds, input.Date, exceptIds);
            var balanceDic = batchBalanceItems.ToDictionary(k => $"{k.ItemId}-{k.LotId}-{k.BatchNoId}", v => v.Qty);

            var batchQtyUse = itemUseBatchs
                              .SelectMany(s => s.ItemBatchNos.Select(r => new { s.ItemId, s.LotId, r.BatchNoId, r.Qty }))
                              .GroupBy(s => new { s.ItemId, s.LotId, s.BatchNoId })
                              .Select(s => new
                              {
                                  ItemName = $"{batchItemDic[s.Key.ItemId].ItemCode}-{batchItemDic[s.Key.ItemId].ItemName}, BatchNo: {batchNoDic[s.Key.BatchNoId].Code}",
                                  Key = $"{s.Key.ItemId}-{s.Key.LotId}-{s.Key.BatchNoId}",
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


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_Create,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueSale)]
        public async Task<NullableIdDto<Guid>> Create(CreateItemIssueInput input)
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

            var validatelot = input.Items.Where(t => t.LotId == null && t.ItemId != null).FirstOrDefault();
            if (validatelot != null)
            {
                throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + (input.Items.IndexOf(validatelot) + 1).ToString());
            }

            await ValidateBatchNo(input);
            await ValidateApplyOrderItems(input);
            if (input.ReceiveFrom == ReceiveFrom.DeliverySchedule) await ValidateApplyDeliveryScheduleItems(input);

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);

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
                                        input.Memo, input.Total, input.Total, input.CurrencyId,
                                        input.ClassId, input.Reference, input.LocationId);
            entity.UpdateStatus(input.Status);
            #region journal transaction type 
            var transactionTypeId = await _journalTransactionTypeManager.GetJournalTransactionTypeId(InventoryTransactionType.ItemIssueSale);
            entity.SetJournalTransactionTypeId(transactionTypeId);
            #endregion
            //await UpdateSOReceiptStautus(null, input.ReceiveFrom, input.InvoiceId, input.Items);



            #region Calculat Cost
            //var roundingId = (await GetCurrentTenantAsync()).RoundDigitAccountId;

            var itemToCalculateCost = input.Items.Select((u, index) => new Inventories.Data.GetAvgCostForIssueData()
            {
                ItemName = u.Item.ItemName,
                Index = index,
                ItemId = u.ItemId,
                ItemCode = u.Item.ItemCode,
                Qty = u.Qty,
                LotId = u.LotId,
                InventoryAccountId = u.InventoryAccountId
            }).ToList();
            var lotIds = input.Items.Select(x => x.LotId).ToList();
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


            //insert to item Issue
            var @itemIssue = ItemIssue.Create(tenantId, userId, input.ReceiveFrom, input.CustomerId,
                                                input.SameAsShippingAddress,
                                                input.ShippingAddress, input.BillingAddress,
                                                input.Total, null, input.ConvertToInvoice);
            @itemIssue.UpdateTransactionTypeId(input.TransactionTypeId);
            @itemIssue.UpdateTransactionType(InventoryTransactionType.ItemIssueSale);
            @entity.UpdateItemIssue(@itemIssue);
            @entity.UpdateCreditDebit(input.Total, input.Total);

            //Update ItemIssueId on table Invoice
            if (input.ReceiveFrom == ReceiveFrom.Invoice)
            {
                if (input.InvoiceId == null)
                {
                    throw new UserFriendlyException(L("PleaseAddInvoice"));
                }
                var updateClass = await _journalRepository.GetAll().Where(t => t.InvoiceId == input.InvoiceId).FirstOrDefaultAsync();
                updateClass.UpdateClass(input.ClassId, input.LocationId);

                var @invoice = await _invoiceRepository.GetAll().Where(u => u.Id == input.InvoiceId).FirstOrDefaultAsync();
                invoice.UpdateItemIssueId(itemIssue.Id);
                // update received status in Invoice to full when receive from Invoice 
                if (input.Status == TransactionStatus.Publish)
                {
                    invoice.UpdateReceivedStatus(DeliveryStatus.ShipAll);
                }
                CheckErrors(await _invoiceManager.UpdateAsync(invoice));

                CheckErrors(await _journalManager.UpdateAsync(updateClass, null));

            }
            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));
            CheckErrors(await _itemIssueManager.CreateAsync(itemIssue));

            if (input.ReceiveFrom == ReceiveFrom.None && input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }

            var ItemIssueItems = (from ItemIssueitem in input.Items
                                  where (ItemIssueitem.SaleOrderItemId != null)
                                  select ItemIssueitem.SaleOrderItemId);
            var count = ItemIssueItems.Count();
            if (input.ReceiveFrom == ReceiveFrom.SaleOrder && count <= 0)
            {
                throw new UserFriendlyException(L("PleaseAddSaleOrder"));
            }

            var invoiceItems = await _invoiceItemRepository.GetAll()
                                    .Include(s => s.SaleOrderItem)
                                    .Include(s=>s.DeliveryScheduleItem)
                                    .Where(t => t.InvoiceId == input.InvoiceId).ToListAsync();

            var orderIds = input.Items.Where(s => s.SaleOrderId.HasValue)
                               .GroupBy(s => s.SaleOrderId.Value)
                               .Select(s => s.Key).ToList();

            var ids = invoiceItems.Where(s => s.SaleOrderItem != null)
                     .GroupBy(s => s.SaleOrderItem.SaleOrderId)
                     .Select(s => s.Key)
                     .ToList();
            orderIds.AddRange(ids);

            var deliveryIds = input.Items.Where(s => s.DeliveryScheduleId.HasValue)
                              .GroupBy(s => s.DeliveryScheduleId.Value)
                              .Select(s => s.Key).ToList();
            var DIds = invoiceItems.Where(s => s.DeliveryScheduleItem != null)
                    .GroupBy(s => s.DeliveryScheduleItem.DeliveryScheduleId)
                    .Select(s => s.Key)
                    .ToList();
            deliveryIds.AddRange(DIds);



            foreach (var i in input.Items)
            {
                //insert to item Issue item
                var @itemIssueItem = ItemIssueItem.Create(tenantId, userId, itemIssue, i.ItemId, i.Description, i.Qty, i.UnitCost, i.DiscountRate, i.Total);
                itemIssueItem.UpdateLot(i.LotId);
                CheckErrors(await _itemIssueItemManager.CreateAsync(@itemIssueItem));

                //Update InvoiceItems of With has Item Issue Item ID
                if (input.ReceiveFrom == ReceiveFrom.Invoice && i.InvoiceItemId != null)
                {

                    var invoiceItem = invoiceItems.Where(u => u.Id == i.InvoiceItemId.Value).First();
                    if (invoiceItem != null)
                    {
                        invoiceItem.UpdateIssueItemId(itemIssueItem.Id);
                        invoiceItem.UpdateLot(itemIssueItem.LotId);
                        CheckErrors(await _invoiceItemManager.UpdateAsync(invoiceItem));

                        if (i.Qty > invoiceItem.Qty)
                        {
                            throw new UserFriendlyException(L("PleaseCheckYourQty"));
                        }
                    }
                }
                else if (i.SaleOrderItemId != null && input.ReceiveFrom == ReceiveFrom.SaleOrder)
                {
                    itemIssueItem.UpdateSaleOrderItemId(i.SaleOrderItemId);
                }
                else if (i.DeliveryScheduleItemId.HasValue && input.ReceiveFrom == ReceiveFrom.DeliverySchedule)
                {
                    itemIssueItem.UpdateDeliveryScheduleItemId(i.DeliveryScheduleItemId);
                }
                //insert inventory journal item into credit
                var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, i.InventoryAccountId,
                    i.Description, 0, i.Total, PostingKey.Inventory, itemIssueItem.Id);
                CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));
                //insert COGS journal item into Debit
                var cogsJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, i.PurchaseAccountId,
                    i.Description, i.Total, 0, PostingKey.COGS, itemIssueItem.Id);
                CheckErrors(await _journalItemManager.CreateAsync(cogsJournalItem));

                if (i.ItemBatchNos != null && i.ItemBatchNos.Any())
                {
                    var addItemBatchNos = i.ItemBatchNos.Select(s => ItemIssueItemBatchNo.Create(tenantId, userId, itemIssueItem.Id, s.BatchNoId, s.Qty)).ToList();
                    foreach (var a in addItemBatchNos)
                    {
                        await _itemIssueItemBatchNoRepository.InsertAsync(a);
                    }
                }
            }

            if (orderIds.Any() || deliveryIds.Any())
            {
                await CurrentUnitOfWork.SaveChangesAsync();
                foreach (var id in orderIds)
                {
                    await UpdateOrderInventoryStatus(id);
                }

                foreach (var id in deliveryIds)
                {
                    await UpdateDeliveryInventoryStatus(id,true);

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
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Delete,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour)]
        public async Task Delete(CarlEntityDto input)
        {

            var journal = await _journalRepository.GetAll()
                            .Include(u => u.ItemIssue)
                            .Include(u => u.ItemIssue.TransferOrder)
                            .Include(u => u.ItemIssue.ProductionOrder)
                            .Include(u => u.ItemIssue.ShippingAddress)
                            .Include(u => u.ItemIssue.BillingAddress)
                            .Where(u => (u.JournalType == JournalType.ItemIssueSale ||
                                    u.JournalType == JournalType.ItemIssueOther ||
                                        u.JournalType == JournalType.ItemIssueTransfer ||
                                    u.JournalType == JournalType.ItemIssueAdjustment ||
                                    u.JournalType == JournalType.ItemIssueVendorCredit ||
                                    u.JournalType == JournalType.ItemIssueProduction)
                                    && u.ItemIssueId == input.Id)
                            .FirstOrDefaultAsync();

            //query get item Receipt 
            if (journal == null || journal.ItemIssue == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                                           .Where(t => (t.LockKey == TransactionLockType.ItemIssue || t.LockKey == TransactionLockType.InventoryTransaction)
                                           && t.IsLock == true && t.LockDate.Value.Date >= journal.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            var @entity = journal.ItemIssue;
            var userId = AbpSession.GetUserId();
            var getPermission = await GetUserPermissions(userId);
            var IsEdit = getPermission.Where(t => t == AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour).Count();
            var totalHours = (DateTime.Now - @entity.CreationTime).TotalHours;
            if (IsEdit > 0 && totalHours > 48)
            {
                throw new UserFriendlyException(L("ThisTransactionIsOver48HoursCanNotDelete"));
            }

            if (entity.TransferOrderId != null && entity.TransferOrder != null && entity.TransferOrder.ConvertToIssueAndReceiptTransfer == true)
            {
                throw new UserFriendlyException(L("AutoConvertMessage"));
            }

            // validate on item receipt production
            if (entity.ProductionOrderId != null && entity.ProductionOrder != null && entity.ProductionOrder.ConvertToIssueAndReceipt == true)
            {
                throw new UserFriendlyException(L("AutoConvertMessage"));
            }
            //validate when use in customercredit and itemReceiptCustomerCredit
            var customerCredit = await _cusotmerCreditRepository.GetAll().Where(t => t.ItemIssueSaleId == input.Id).CountAsync();
            var itemReceiptCustomerCredit = await _itemReceiptCustomerCreditRepository.GetAll().Where(t => t.ItemIssueSaleId == input.Id).CountAsync();

            if (customerCredit > 0 || itemReceiptCustomerCredit > 0)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChangeInCustomerCredit"));
            }

            //update receive status of transfer order to pending 
            if (journal.JournalType == JournalType.ItemIssueTransfer && @entity.TransferOrder != null)
            {
                var @transfer = @entity.TransferOrder;// await _transferOrderRepository.GetAll().Where(u => u.Id == journal.ItemIssue.TransferOrderId).FirstOrDefaultAsync();
                if (@transfer.ShipedStatus == TransferStatus.ReceiveAll)//throw if item issue receive from transfer order than also already item receipt too 
                {
                    throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
                }
                @transfer.UpdateShipedStatus(TransferStatus.Pending);
                CheckErrors(await _transferOrderManager.UpdateAsync(@transfer));
            }
            if (journal.JournalType == JournalType.ItemIssueProduction && @entity.ProductionOrder != null)
            {
                var production = @entity.ProductionOrder;// await _productionOrderRepository.GetAll().Where(u => u.Id == journal.ItemIssue.ProductionOrderId).FirstOrDefaultAsync();
                if (production.ShipedStatus == TransferStatus.ReceiveAll)//throw if item issue receive from production order than also already item receipt too 
                {
                    throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
                }
                production.UpdateShipedStatus(TransferStatus.Pending);
                production.SetCalculateionState(CalculationState.NotSync);
                CheckErrors(await _productionOrderManager.UpdateAsync(production));
            }


            var validateInvoice = _invoiceRepository.GetAll().Include(t => t.ItemIssue)
                                             .Where(t => t.ItemIssueId == input.Id).FirstOrDefault();
            if (validateInvoice != null && validateInvoice.CreationTime > validateInvoice.ItemIssue.CreationTime)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            else if (validateInvoice != null && validateInvoice.ConvertToItemIssue == true)
            {
                throw new UserFriendlyException(L("AutoConvertMessage"));
            }
            else if (validateInvoice != null)
            {
                // update relationship of itemIssueId field to null
                validateInvoice.UpdateItemIssueId(null);
                validateInvoice.UpdateReceivedStatus(DeliveryStatus.ShipPending);
                CheckErrors(await _invoiceManager.UpdateAsync(validateInvoice));
            }

            //await UpdateSOReceiptStautus(input.Id, entity.ReceiveFrom, validateInvoice?.Id, null);

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

            // journal.UpdateItemIssue(null);

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
            #region SO

            //query get item receipt item and delete 
            var @ItemIssueItems = await _itemIssueItemRepository.GetAll()
                                            .Include(u => u.SaleOrderItem)
                                            .Where(u => u.ItemIssueId == entity.Id)
                                            .ToListAsync();
            // temp of po id header 
            var orderIds = @ItemIssueItems
                            .Where(s => s.SaleOrderItem != null)
                            .GroupBy(s => s.SaleOrderItem.SaleOrderId)
                            .Select(s => s.Key)
                            .ToList();

            #endregion

            #region DS
            //query get item receipt item and delete 
            var @ItemIssueDItems = await _itemIssueItemRepository.GetAll()
                                            .Include(u => u.DeliveryScheduleItem)
                                            .Where(u => u.ItemIssueId == entity.Id)
                                            .ToListAsync();
            // temp of po id header 
            var deliveryIds = @ItemIssueDItems
                            .Where(s => s.DeliveryScheduleItem != null)
                            .GroupBy(s => s.DeliveryScheduleItem.DeliveryScheduleId)
                            .Select(s => s.Key)
                            .ToList();
            #endregion


            //Update ItemIssueitemId on table billitem
            var updateItemIssueitemId = (from invoiceitem in _invoiceItemRepository.GetAll()
                                         join ItemIssueItem in _itemIssueItemRepository.GetAll()
                                         .Include(u => u.ItemIssue)
                                         on invoiceitem.ItemIssueItemId equals ItemIssueItem.Id
                                         where (ItemIssueItem.ItemIssueId == entity.Id)
                                         select invoiceitem);
            foreach (var u in updateItemIssueitemId)
            {
                u.UpdateIssueItemId(null);
                CheckErrors(await _invoiceItemManager.UpdateAsync(u));
            }

            if (updateItemIssueitemId.Any())
            {
                var ids = updateItemIssueitemId.Where(s => s.SaleOrderItem != null)
                          .GroupBy(s => s.SaleOrderItem.SaleOrderId)
                          .Select(s => s.Key)
                          .ToList();
                var idDs = updateItemIssueitemId.Where(s => s.DeliveryScheduleItem != null)
                          .GroupBy(s => s.DeliveryScheduleItem.DeliveryScheduleId)
                          .Select(s => s.Key)
                          .ToList();

                orderIds.AddRange(ids);
                deliveryIds.AddRange(idDs);
            }

            // query to get list of item which is receive form status == bill and bill item is has order id 

            var scheduleItems = ItemIssueItems.Select(s => s.ItemId).Distinct().ToList();
            foreach (var iri in @ItemIssueItems)
            {

                CheckErrors(await _itemIssueItemManager.RemoveAsync(iri));

            }

            if (entity.ReceiveFrom == ReceiveFrom.Invoice)
            {
                //update Receive status to pending               
            }
            else
            {
                //update Receive status to pending              
            }

            CheckErrors(await _itemIssueManager.RemoveAsync(entity));

            if (orderIds.Any() || deliveryIds.Any())
            {
                await CurrentUnitOfWork.SaveChangesAsync();
                foreach (var id in orderIds)
                {
                    await UpdateOrderInventoryStatus(id);
                }
                foreach (var id in deliveryIds)
                {
                    await UpdateDeliveryInventoryStatus(id,true);
                }
            }


            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.ItemIssue, TransactionLockType.InventoryTransaction, };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }



            await DeleteInventoryTransactionItems(input.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, scheduleDate, scheduleItems);
            }

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_Find)]
        public async Task<PagedResultDto<GetListItemIssueOutput>> Find(GetListItemIssueInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var query = (from jItem in _journalItemRepository.GetAll().AsNoTracking()
                         join j in _journalRepository.GetAll().AsNoTracking()
                        .Where(u => u.JournalType == JournalType.ItemIssueSale)
                        .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                        .WhereIf(input.Invoices != null && input.Invoices.Count > 0, u => input.Invoices.Contains(u.BillId.Value))
                        .WhereIf(input.Status != null && input.Status.Count > 0, u => input.Status.Contains(u.Status))
                        .WhereIf(input.JournalTransactionTypeIds != null && input.JournalTransactionTypeIds.Count > 0, u => u.JournalTransactionTypeId != null && input.JournalTransactionTypeIds.Contains(u.JournalTransactionTypeId.Value))
                         //.WhereIf(input.FromDate != null && input.ToDate != null,
                         // (u => (u.Date) >= (input.FromDate) && (u.Date.Date) <= (input.ToDate)))
                         .WhereIf(!input.Filter.IsNullOrEmpty(),
                         u => u.JournalNo.ToLower().Contains(input.Filter.ToLower())) on jItem.JournalId equals j.Id

                         join iii in _itemIssueItemRepository.GetAll().AsNoTracking()
                         .WhereIf(input.SaleOrderNo != null && input.SaleOrderNo.Count > 0, u => input.SaleOrderNo.Contains(u.SaleOrderItem.SaleOrder.Id))
                         .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                         on jItem.Identifier equals iii.Id
                         join ii in _itemIssueRepository.GetAll().AsNoTracking()
                         .WhereIf(input.Customers != null && input.Customers.Count > 0, u => input.Customers.Contains(u.CustomerId)) on j.ItemIssueId equals ii.Id

                         join c in _customerRepository.GetAll().AsNoTracking() on ii.CustomerId equals c.Id

                         group iii by new
                         {
                             Memo = j.Memo,
                             Date = j.Date,
                             JournalNo = j.JournalNo,
                             Status = j.Status,
                             Id = ii.Id,
                             CustomerId = ii.CustomerId,
                             Total = ii.Total,
                             Customer_Id = c.Id,
                             CustomerName = c.CustomerName,
                             CustomerCode = c.CustomerCode

                         } into u

                         select new GetListItemIssueOutput
                         {
                             CountItem = u.Count(),
                             Memo = u.Key.Memo,
                             Id = u.Key.Id,
                             Date = u.Key.Date,
                             JournalNo = u.Key.JournalNo,
                             Status = u.Key.Status,
                             Total = u.Key.Total,
                             CustomerId = u.Key.CustomerId,
                             Customer = new CustomerSummaryOutput
                             {
                                 Id = u.Key.Customer_Id,
                                 CustomerName = u.Key.CustomerName,
                                 CustomerCode = u.Key.CustomerCode
                             },
                         });

            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            return new PagedResultDto<GetListItemIssueOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_GetDetail,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ViewDetail)]
        public async Task<ItemIssueDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            await TurnOffTenantFilterIfDebug();

            var @journal = await _journalRepository
                                .GetAll()
                                .Include(u => u.ItemIssue)
                                .Include(u => u.ItemIssue.TransactionTypeSale)
                                .Include(u => u.Location)
                                .Include(u => u.Class)
                                .Include(u => u.Currency)
                                .Include(u => u.ItemIssue.Customer)
                                .Include(u => u.ItemIssue.ShippingAddress)
                                .Include(u => u.ItemIssue.BillingAddress)
                                .Include(u => u.Invoice)
                                .Include(u => u.JournalTransactionType)
                                .AsNoTracking()
                                .Where(u => u.JournalType == JournalType.ItemIssueSale && u.ItemIssueId == input.Id)
                                .FirstOrDefaultAsync();

            if (@journal == null || @journal.ItemIssue == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var itemIssueItemCOGSs = await _itemIssueItemRepository.GetAll()
               .Include(u => u.Item)
               .Include(u => u.Lot)
               .Include(u => u.SaleOrderItem)
               .Include(u=>u.DeliveryScheduleItem.DeliverySchedule)
               .Include(u => u.SaleOrderItem.SaleOrder)
               .Where(u => u.ItemIssueId == input.Id)
               .Join(
                   _journalItemRepository.GetAll()
                   .Where(u => u.Key == PostingKey.COGS)
                   .Include(u => u.Account)
                   .AsNoTracking()
                   ,
                   u => u.Id, s => s.Identifier,
                   (issueItem, jItem) =>
                   new ItemIssueItemDetailOutput()
                   {
                       FromLotDetail = ObjectMapper.Map<LotSummaryOutput>(issueItem.Lot),
                       FromLotId = issueItem.LotId,
                       Id = issueItem.Id,
                       PurchaseAccountId = jItem.AccountId,
                       PurchaseAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                   }).ToListAsync();

            var locations = new List<long?>() {
                journal.LocationId
            };

            //var averageCosts = await _inventoryManager.GetAvgCostQuery(@journal.Date.Date, locations).ToListAsync();

            var itemIssueItems = await (from issueItem in _itemIssueItemRepository.GetAll()
                                                        .Include(u => u.Item.InventoryAccount)
                                                        .Include(u => u.SaleOrderItem.SaleOrder)
                                                        .Include(u=>u.DeliveryScheduleItem.DeliverySchedule.SaleOrder)
                                                        .Include(u => u.Item.SaleAccount)
                                                        .Include(u => u.Lot)
                                                        .Include(u => u.Item.PurchaseAccount)
                                                        .Where(u => u.ItemIssueId == input.Id)
                                                        .AsNoTracking()
                                                        .OrderBy(t => t.CreationTime)

                                        join jItem in _journalItemRepository.GetAll()
                                                        .Include(u => u.Account)
                                                        .Where(u => u.Identifier.HasValue)
                                                        .Where(u => u.Key == PostingKey.Inventory)
                                                        .AsNoTracking()
                                        on issueItem.Id equals jItem.Identifier
                                        join i in _inventoryTransactionItemRepository.GetAll()
                                                                                      .Where(s => s.JournalType == JournalType.ItemIssueSale)
                                                                                      .Where(s => s.TransactionId == input.Id)
                                                                                      .AsNoTracking()
                                        on issueItem.Id equals i.Id
                                        into cs
                                        from c in cs.DefaultIfEmpty()
                                        select
                                        new ItemIssueItemDetailOutput()
                                        {
                                            FromLotId = issueItem.LotId,
                                            FromLotDetail = ObjectMapper.Map<LotSummaryOutput>(issueItem.Lot),
                                            SaleOrderNumber = issueItem.SaleOrderItem == null && issueItem.DeliveryScheduleItem != null ? issueItem.DeliveryScheduleItem.DeliverySchedule.SaleOrder.OrderNumber : issueItem.SaleOrderItem.SaleOrder.OrderNumber,
                                            SaleOrderReference = issueItem.SaleOrderItem.SaleOrder.Reference,
                                            DeliveryNo = issueItem.DeliveryScheduleItem.DeliverySchedule.DeliveryNo,
                                            DeliveryReference = issueItem.DeliveryScheduleItem.DeliverySchedule.Reference,
                                            Id = issueItem.Id,
                                            Item = new ItemSummaryDetailOutput
                                            {
                                                InventoryAccount = ObjectMapper.Map<ChartAccountDetailOutput>(issueItem.Item.InventoryAccount),
                                                Id = issueItem.Item.Id,
                                                InventoryAccountId = issueItem.Item.InventoryAccountId,
                                                ItemCode = issueItem.Item.ItemCode,
                                                ItemName = issueItem.Item.ItemName,
                                                PurchaseAccount = ObjectMapper.Map<ChartAccountDetailOutput>(issueItem.Item.PurchaseAccount),
                                                PurchaseAccountId = issueItem.Item.PurchaseAccountId,
                                                SaleAccount = ObjectMapper.Map<ChartAccountDetailOutput>(issueItem.Item.SaleAccount),
                                                SaleAccountId = issueItem.Item.SaleAccountId,
                                                SalePrice = issueItem.Item.SalePrice,
                                                SaleTax = ObjectMapper.Map<TaxDetailOutput>(issueItem.Item.SaleTax),
                                                SaleTaxId = issueItem.Item.SaleTaxId,

                                            },
                                            MultiCurrencyId = issueItem.SaleOrderItem != null ? issueItem.SaleOrderItem.SaleOrder.MultiCurrencyId : null,
                                            ItemId = issueItem.ItemId,
                                            SaleOrderItem = ObjectMapper.Map<SaleOrderItemSummaryOut>(issueItem.SaleOrderItem),
                                            DeliveryScheduleItem = ObjectMapper.Map<DeliveryItemSummaryOut>(issueItem.DeliveryScheduleItem),
                                            InventoryAccountId = jItem.AccountId,
                                            InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                                            Description = issueItem.Description,
                                            DiscountRate = issueItem.DiscountRate,
                                            Qty = issueItem.Qty,
                                            Total = c != null ? Math.Abs(c.LineCost) : issueItem.Total,
                                            UnitCost = c != null ? c.UnitCost : issueItem.UnitCost,
                                            SaleOrderItemId = issueItem.SaleOrderItemId,
                                            DeliveryScheduleItemId = issueItem.DeliverySchedulItemId,
                                            DeliveryScheduleId = issueItem.DeliveryScheduleItem.DeliveryScheduleId,
                                            PurchaseAccountId = itemIssueItemCOGSs.Where(r => r.Id == issueItem.Id).Select(r => r.PurchaseAccountId).FirstOrDefault(),
                                            PurchaseAccount = itemIssueItemCOGSs.Where(r => r.Id == issueItem.Id).Select(r => r.PurchaseAccount).FirstOrDefault(),
                                            UseBatchNo = issueItem.Item.UseBatchNo,
                                            TrackSerial = issueItem.Item.TrackSerial,
                                            TrackExpiration = issueItem.Item.TrackExpiration
                                        }).ToListAsync();


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

            var invoice = (from iis in _itemIssueRepository.GetAll().Where(t => t.Id == input.Id)
                           join inv in _invoiceRepository.GetAll() on iis.Id equals inv.ItemIssueId
                           join j in _journalRepository.GetAll() on inv.Id equals j.InvoiceId
                           select new
                           {
                               Id = inv.Id,
                               Date = j.Date,
                               JournalNo = j.JournalNo
                           }).FirstOrDefault();

            var result = ObjectMapper.Map<ItemIssueDetailOutput>(journal.ItemIssue);
            result.IssueNo = journal.JournalNo;
            result.Date = journal.Date;
            if (invoice == null)
            {
                result.InvoiceId = null;
            }
            else
            {
                result.InvoiceId = invoice.Id;
                result.InvoiceNo = invoice.JournalNo;
                result.InvoiceDate = invoice.Date;
            }
            result.MulitCurrencyId = result.ReceiveFrom == ReceiveFrom.SaleOrder ? itemIssueItems.Select(t => t.MultiCurrencyId).FirstOrDefault() : null;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.Reference = journal.Reference;
            result.ItemIssueItems = itemIssueItems;
            result.Memo = journal.Memo;
            result.StatusCode = journal.Status;
            //result.Total = journal.Credit;
            result.TransactionTypeSaleId = journal.ItemIssue.TransactionTypeSaleId;
            result.StatusName = journal.Status.ToString();
            result.LocationId = journal.LocationId.Value;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);

            //get total from inventory transaction item cache table
            result.Total = itemIssueItems.Sum(s => s.Total);
            result.TransactionTypeName = journal.JournalTransactionType?.Name;

            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_GetList)]
        public async Task<PagedResultDto<GetListItemIssueOutput>> GetList(GetListItemIssueInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var vendoTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();
            var customerTypeMemberIds = await GetCustomerTypeMembers().Select(s => s.CustomerTypeId).ToListAsync();

            var customerQuery = GetCustomers(null, input.Customers, null, customerTypeMemberIds);
            var vendorQuery = GetVendors(null, input.Vendors, null, vendoTypeMemberIds);
            var userQuery = GetUsers(input.Users);
            var locationQuery = GetLocations(null, input.Locations);
            var accountQuery = GetAccounts(input.Accounts);

            var filterSaleType = input.TransactionSaleType != null && input.TransactionSaleType.Count > 0;

            var jQuery = _journalRepository.GetAll()
                            .Where(u => u.ItemIssueId != null || (u.ItemIssueVendorCreditId != null && !filterSaleType))
                            .WhereIf(input.JournalTypes != null && input.JournalTypes.Count > 0, u => input.JournalTypes.Contains(u.JournalType))
                            .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                            .WhereIf(input.Status != null && input.Status.Count > 0, u => input.Status.Contains(u.Status))
                            .WhereIf(input.Locations != null && input.Locations.Count > 0, u => u.Location != null && input.Locations.Contains(u.LocationId.Value))
                            .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                            .WhereIf(input.FromDate != null && input.ToDate != null,
                                (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                            .WhereIf(!input.Filter.IsNullOrEmpty(),
                                u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                    u.Reference.ToLower().Contains(input.Filter.ToLower()))
                            .WhereIf(input.JournalTransactionTypeIds != null && input.JournalTransactionTypeIds.Count > 0, u => u.JournalTransactionTypeId != null && input.JournalTransactionTypeIds.Contains(u.JournalTransactionTypeId.Value))
                            .AsNoTracking()
                            .Select(j => new
                            {
                                Id = j.Id,
                                CreationTimeIndex = j.CreationTimeIndex,
                                CreationTime = j.CreationTime,
                                Memo = j.Memo,
                                ItemIssueId = j.ItemIssueId,
                                ItemIssueVendorCreditId = j.ItemIssueVendorCreditId,
                                Date = j.Date,
                                JournalNo = j.JournalNo,
                                JournalTransactionTypeName = j.JournalTransactionTypeId != null ? j.JournalTransactionType.Name : null,
                                Reference = j.Reference,
                                Status = j.Status,
                                JournalType = j.JournalType,
                                CreatorUserId = j.CreatorUserId,
                                LocationId = j.LocationId
                            });

            var journalQuery = from j in jQuery
                               join u in userQuery
                               on j.CreatorUserId equals u.Id
                               join l in locationQuery
                               on j.LocationId equals l.Id
                               select new
                               {
                                   Id = j.Id,
                                   CreationTimeIndex = j.CreationTimeIndex,
                                   CreationTime = j.CreationTime,
                                   Memo = j.Memo,
                                   ItemIssueId = j.ItemIssueId,
                                   ItemIssueVendorCreditId = j.ItemIssueVendorCreditId,
                                   Date = j.Date,
                                   JournalNo = j.JournalNo,
                                   JournalTransactionTypeName = j.JournalTransactionTypeName,
                                   Reference = j.Reference,
                                   Status = j.Status,
                                   JournalType = j.JournalType,
                                   CreatorUserId = j.CreatorUserId,
                                   LocationId = j.LocationId,
                                   UserName = u.UserName,
                                   LocationName = l.LocationName
                               };

            var journalItemQuery = _journalItemRepository.GetAll()
                                    .Where(u => u.Key == PostingKey.Clearance && u.Identifier == null)
                                    .WhereIf(input.Accounts != null && input.Accounts.Count > 0, u => input.Accounts.Contains(u.AccountId))
                                    .AsNoTracking()
                                    .Select(s => new
                                    {
                                        s.Id,
                                        s.JournalId,
                                        s.AccountId
                                    });

            var jaQuery = from ji in journalItemQuery
                          join a in accountQuery
                          on ji.AccountId equals a.Id
                          select new
                          {
                              ji.JournalId,
                              ji.AccountId,
                              a.AccountName
                          };

            var jiQuery = from j in journalQuery
                          join ji in jaQuery
                          on j.Id equals ji.JournalId
                          into jis
                          from ji in jis.DefaultIfEmpty()
                          where input.Accounts == null || input.Accounts.Count == 0 || ji != null

                          select new
                          {
                              Id = j.Id,
                              Date = j.Date,
                              JournalNo = j.JournalNo,
                              Reference = j.Reference,
                              Memo = j.Memo,
                              JournalType = j.JournalType,
                              JournalTransactionTypeName = j.JournalTransactionTypeName,
                              ItemIssueId = j.ItemIssueId,
                              ItemIssueVendorCreditId = j.ItemIssueVendorCreditId,
                              CreationTimeIndex = j.CreationTimeIndex,
                              CreationTime = j.CreationTime,
                              Status = j.Status,
                              LocationId = j.LocationId,
                              CreatorUserId = j.CreatorUserId,
                              LocationName = j.LocationName,
                              UserName = j.UserName,
                              AccountId = ji == null ? (Guid?)null : ji.AccountId,
                              AccountName = ji == null ? "" : ji.AccountName
                          };

            var iiQuery = from ii in _itemIssueRepository.GetAll()
                                   .WhereIf(input.Customers != null && input.Customers.Count > 0, u => input.Customers.Contains(u.CustomerId))
                                   .WhereIf(input.TransactionSaleType != null && input.TransactionSaleType.Count > 0, s => input.TransactionSaleType.Contains(s.TransactionTypeSaleId))
                                   .AsNoTracking()
                                   .Select(s => new
                                   {
                                       Id = s.Id,
                                       Total = s.Total,
                                       CustomerId = s.CustomerId
                                   })
                          join iii in _itemIssueItemRepository.GetAll()
                                       .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId)).AsNoTracking()
                                       .AsNoTracking()
                                       .Select(s => s.ItemIssueId)
                          on ii.Id equals iii
                          into iis
                          where iis.Count() > 0
                          select ii;

            var itemIssueQuery = from ii in iiQuery
                                 join c in customerQuery
                                 on ii.CustomerId equals c.Id
                                 into cs
                                 from c in cs.DefaultIfEmpty()
                                 where input.Customers == null || input.Customers.Count == 0 || c != null
                                 select new
                                 {
                                     Id = ii.Id,
                                     Total = ii.Total,
                                     CustomerId = c == null ? (Guid?)null : c.Id,
                                     CustomerName = c == null ? "" : c.CustomerName
                                 };

            var ivQuery = from iv in _itemIssueVendorCreditRepository.GetAll()
                                    .Where(s => !filterSaleType)
                                    .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId))
                                    .AsNoTracking()
                                    .Select(s => new
                                    {
                                        Id = s.Id,
                                        Total = s.Total,
                                        VendorId = s.VendorId
                                    })

                          join ivi in _itemIssueVendorCreditItemRepository.GetAll()
                                       .Where(s => !filterSaleType)
                                       .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                                       .AsNoTracking()
                                       .Select(s => s.ItemIssueVendorCreditId)
                                       on iv.Id equals ivi
                                       into ivs
                          where ivs.Count() > 0
                          select iv;

            var itemIssueVendorCreditQuery = from iv in ivQuery
                                             join v in vendorQuery
                                             on iv.VendorId equals v.Id
                                             select new
                                             {
                                                 Id = iv.Id,
                                                 Total = iv.Total,
                                                 VendorId = iv.VendorId,
                                                 VendorName = v.VendorName
                                             };

            var result = from j in jiQuery

                         join ii in itemIssueQuery
                         on j.ItemIssueId equals ii.Id
                         into iis
                         from ii in iis.DefaultIfEmpty()

                         join iv in itemIssueVendorCreditQuery
                         on j.ItemIssueVendorCreditId equals iv.Id
                         into ivs
                         from iv in ivs.DefaultIfEmpty()

                         where (ii != null || iv != null) &&
                               (input.Customers == null || input.Customers.Count == 0 || ii != null) &&
                               (input.Vendors == null || input.Vendors.Count == 0 || iv != null)

                         select new GetListItemIssueOutput
                         {
                             CreationTimeIndex = j.CreationTimeIndex,
                             CreationTime = j.CreationTime,
                             JournalTransactionTypeName = j.JournalTransactionTypeName,
                             Memo = j.Memo,
                             LocationName = j.LocationName,
                             Id = ii != null ? ii.Id : iv.Id,
                             Date = j.Date,
                             JournalNo = j.JournalNo,
                             Reference = j.Reference,
                             Status = j.Status,
                             Total = ii != null ? ii.Total : iv.Total,
                             Customer = ii != null && ii.CustomerId != null ?
                                        new CustomerSummaryOutput
                                        {
                                            Id = ii.CustomerId.Value,
                                            CustomerName = ii.CustomerName
                                        } : null,
                             Vendor = iv != null ?
                                        new VendorSummaryOutput
                                        {
                                            Id = iv.VendorId,
                                            VendorName = iv.VendorName
                                        } : null,
                             User = new UserDto
                             {
                                 Id = j.CreatorUserId.Value,
                                 UserName = j.UserName
                             },
                             Type = j.JournalType,
                             TypeName = j.JournalType.ToString(),
                             AccountId = j.AccountId.HasValue ? j.AccountId.Value : Guid.Empty,
                             AccountName = j.AccountName,
                         };

            var resultCount = await result.CountAsync();
            if (resultCount == 0) return new PagedResultDto<GetListItemIssueOutput>(resultCount, new List<GetListItemIssueOutput>());

            if (input.Sorting.EndsWith("DESC"))
            {
                if (input.Sorting.ToLower().StartsWith("date"))
                {
                    result = result.OrderByDescending(s => s.Date.Date).ThenByDescending(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("type"))
                {
                    result = result.OrderByDescending(s => s.TypeName).ThenByDescending(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("journalno"))
                {
                    result = result.OrderByDescending(s => s.JournalNo).ThenByDescending(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("total"))
                {
                    result = result.OrderByDescending(s => s.Total).ThenByDescending(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("status"))
                {
                    result = result.OrderByDescending(s => s.Status).ThenByDescending(s => s.CreationTimeIndex);
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    result = result.OrderBy(input.Sorting).ThenByDescending(s => s.CreationTimeIndex);
                }
            }
            else
            {
                if (input.Sorting.ToLower().StartsWith("date"))
                {
                    result = result.OrderBy(s => s.Date.Date).ThenBy(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("type"))
                {
                    result = result.OrderBy(s => s.TypeName).ThenBy(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("journalno"))
                {
                    result = result.OrderBy(s => s.JournalNo).ThenBy(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("total"))
                {
                    result = result.OrderBy(s => s.Total).ThenBy(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("status"))
                {
                    result = result.OrderBy(s => s.Status).ThenBy(s => s.CreationTimeIndex);
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    result = result.OrderBy(input.Sorting).ThenBy(s => s.CreationTimeIndex);
                }
            }

            var @entities = await result.PageBy(input).ToListAsync();

            return new PagedResultDto<GetListItemIssueOutput>(resultCount, ObjectMapper.Map<List<GetListItemIssueOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_GetList)]
        public async Task<PagedResultDto<GetListItemIssueOutput>> GetListOld(GetListItemIssueInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();


            var result = from j in _journalRepository.GetAll()
                            .Include(u => u.ItemIssue.Customer)
                            .Include(u => u.ItemIssueVendorCredit.Vendor)
                            .Include(u => u.Location)
                            .Include(u => u.CreatorUser)
                           .Where(u => u.JournalType == JournalType.ItemIssueOther ||
                             u.JournalType == JournalType.ItemIssueSale ||
                             u.JournalType == JournalType.ItemIssueTransfer ||
                             u.JournalType == JournalType.ItemIssueProduction ||
                             u.JournalType == JournalType.ItemIssueVendorCredit ||
                             u.JournalType == JournalType.ItemIssuePhysicalCount ||
                             u.JournalType == JournalType.ItemIssueAdjustment)

                            .WhereIf(input.Status != null && input.Status.Count > 0, u => input.Status.Contains(u.Status))
                            .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                            .WhereIf(input.JournalTypes != null && input.JournalTypes.Count > 0, u => input.JournalTypes.Contains(u.JournalType))
                            .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                            .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId.Value))
                            .WhereIf(input.FromDate != null && input.ToDate != null,
                             (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                             .WhereIf(!input.Filter.IsNullOrEmpty(),
                             u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                            u.Reference.ToLower().Contains(input.Filter.ToLower()) ||
                                            u.Memo.ToLower().Contains(input.Filter.ToLower()))
                            .AsNoTracking()
                         join jItem in _journalItemRepository.GetAll()
                                    .Include(u => u.Account)
                                    .Where(u => u.Key == PostingKey.Clearance && u.Identifier == null)
                                    .AsNoTracking()
                        on j.Id equals jItem.JournalId into JournalItems
                         from headerAccount in JournalItems.DefaultIfEmpty()
                         where headerAccount == null || headerAccount.JournalId == j.Id

                         join iii in _itemIssueItemRepository.GetAll()
                         .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId)).AsNoTracking()
                         .GroupBy(u => u.ItemIssue).Select(u => u.Key)
                         on j.ItemIssueId equals iii.Id into goodIssueItems
                         from leftIii in goodIssueItems.DefaultIfEmpty()

                         join iivi in _itemIssueVendorCreditItemRepository.GetAll()
                          .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId)).AsNoTracking()
                          .GroupBy(u => u.ItemIssueVendorCredit).Select(u => u.Key)
                          on j.ItemIssueVendorCreditId equals iivi.Id into itemIssudVendorCreditItems
                         from leftIivi in itemIssudVendorCreditItems.DefaultIfEmpty()

                         where (leftIii != null || leftIivi != null)

                         select new GetListItemIssueOutput
                         {
                             CreationTimeIndex = j.CreationTimeIndex,
                             Memo = j.Memo,
                             Id = j.ItemIssue != null ? j.ItemIssue.Id : j.ItemIssueVendorCredit.Id,
                             Date = j.Date,
                             JournalNo = j.JournalNo,
                             Status = j.Status,
                             Total = j.ItemIssue != null ? j.ItemIssue.Total : j.ItemIssueVendorCredit.Total,
                             Customer = j.ItemIssue != null ? ObjectMapper.Map<CustomerSummaryOutput>(j.ItemIssue.Customer) : null,

                             Type = j.JournalType,
                             TypeName = Enum.GetName(j.JournalType.GetType(), j.JournalType),
                             AccountId = headerAccount == null ? Guid.NewGuid() : headerAccount.AccountId,
                             AccountName = headerAccount == null || headerAccount.Account == null ? null : headerAccount.Account.AccountName,
                             Vendor = j.ItemIssueVendorCredit != null ? ObjectMapper.Map<VendorSummaryOutput>(j.ItemIssueVendorCredit.Vendor) : null,
                             User = ObjectMapper.Map<UserDto>(j.CreatorUser),
                             LocationName = j.Location.LocationName
                         };

            var query = result.WhereIf(input.Accounts != null && input.Accounts.Count > 0, u => input.Accounts.Contains(u.AccountId))
                        .WhereIf(input.Customers != null && input.Customers.Count > 0, u => u.Customer != null && input.Customers.Contains(u.Customer.Id))
                        .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => u.Vendor != null && input.Vendors.Contains(u.Vendor.Id));
            var resultCount = await query.CountAsync();
            if (input.Sorting.Contains("date") && !input.Sorting.Contains("."))
                input.Sorting = input.Sorting.Replace("date", "Date.Date");

            input.Sorting += ", CreationTimeIndex" + (input.Sorting.Contains("DESC") ? " DESC" : "");


            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<GetListItemIssueOutput>(resultCount, ObjectMapper.Map<List<GetListItemIssueOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_Update,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Update,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour)]
        public async Task<NullableIdDto<Guid>> Update(UpdateItemIssueInput input)
        {
            //validate if item issue has already convert in invoice so cannot edit or delete
            var validatInvoice = await (from inv in _invoiceRepository.GetAll().Include(t => t.ItemIssue)
                                              .Where(v => v.ItemIssueId != null && v.ItemIssueId == input.Id
                                                          && v.ConvertToItemIssue == false
                                                          && v.CreationTime > v.ItemIssue.CreationTime)
                                        select inv).CountAsync();
            if (validatInvoice > 0)
            {
                throw new UserFriendlyException(L("ThisRcordConvertFromInvoiceCanNotDelete"));
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
            // validate if item issue which is from invoice that convert auto inventory 
            var invoiceConvert = await _invoiceRepository.GetAll()
                                        .Where(t => t.ItemIssueId == input.Id && t.ConvertToItemIssue == true)
                                        .CountAsync();



            if (invoiceConvert > 0)
            {
                throw new UserFriendlyException(L("ThisRecordHasAutoConvertInventoryFromInvoice"));
            }

            var validatelot = input.Items.Where(t => t.LotId == null && t.ItemId != null).FirstOrDefault();
            if (validatelot != null)
            {
                throw new UserFriendlyException(L("PleaseSelectLot") + L("Row") + " " + (input.Items.IndexOf(validatelot) + 1).ToString());
            }

            await ValidateBatchNo(input);
            await ValidateApplyOrderItems(input);
            if (input.ReceiveFrom == ReceiveFrom.DeliverySchedule) await ValidateApplyDeliveryScheduleItems(input);

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            // update Journal 
            var @journal = await _journalRepository
                              .GetAll()
                              .Include(u => u.ItemIssue)
                              .Where(u => u.JournalType == JournalType.ItemIssueSale && u.ItemIssueId == input.Id)
                              .FirstOrDefaultAsync();
            await CheckClosePeriod(journal.Date, input.Date);

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
                //ItemIssueItemId = u.Id,
                ItemIssueItemId = input.Id,
                LotId = u.LotId,
                InventoryAccountId = u.InventoryAccountId
            }).ToList();

            var lotIds = input.Items.Select(x => x.LotId).ToList();
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

            //update item receipt 
            var itemIssue = journal.ItemIssue;
            var getPermission = await GetUserPermissions(userId);
            var IsEdit = getPermission.Where(t => t == AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour).Count();
            var totalHours = (DateTime.Now - itemIssue.CreationTime).TotalHours;
            if (IsEdit > 0 && totalHours > 48)
            {
                throw new UserFriendlyException(L("ThisTransactionIsOver48HoursCanNotEdit"));
            }
            var receiptFrom = itemIssue.ReceiveFrom;
            if (input.ReceiveFrom == ReceiveFrom.SaleOrder && itemIssue.ReceiveFrom != ReceiveFrom.SaleOrder)
            {
                receiptFrom = ReceiveFrom.SaleOrder;
            }
            //await UpdateSOReceiptStautus(input.Id, itemIssue.ReceiveFrom, input.InvoiceId, input.Items);

            if (itemIssue == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            itemIssue.Update(tenantId, input.ReceiveFrom, input.CustomerId,
                          input.SameAsShippingAddress, input.ShippingAddress,
                          input.BillingAddress, input.Total, null, input.ConvertToInvoice);
            itemIssue.UpdateTransactionTypeId(input.TransactionTypeId);
            #region Add Code 
            // update if come from invoice 
            if (input.ReceiveFrom == ReceiveFrom.Invoice && input.InvoiceId != null)
            {

                var updateClassId = await _journalRepository.GetAll().Where(t => t.InvoiceId == input.InvoiceId).FirstOrDefaultAsync();
                updateClassId.UpdateClass(input.ClassId, input.LocationId);
                var invoice = await _invoiceRepository.GetAll().Where(u => u.Id == input.InvoiceId).FirstOrDefaultAsync();
                invoice.UpdateItemIssueId(itemIssue.Id);
                // update received status in invoice to full when receive from invoice 
                if (input.Status == TransactionStatus.Publish)
                {
                    invoice.UpdateReceivedStatus(DeliveryStatus.ReceiveAll);
                }
                CheckErrors(await _invoiceManager.UpdateAsync(invoice));
                CheckErrors(await _journalManager.UpdateAsync(updateClassId, null));
            }
            else // in some case if user switch from receive from invoice to other so update invoice of field item receipt id to null
            {
                var invoice = await _invoiceRepository.GetAll().Where(u => u.ItemIssueId == input.Id).FirstOrDefaultAsync();
                if (invoice != null)
                {
                    invoice.UpdateItemIssueId(null);
                    invoice.UpdateReceivedStatus(DeliveryStatus.ReceivePending);
                    CheckErrors(await _invoiceManager.UpdateAsync(invoice));
                }
            }
            #endregion
            @journal.UpdateCreditDebit(input.Total, input.Total);

            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);
            CheckErrors(await _journalManager.UpdateAsync(@journal, autoSequence.DocumentType));

            CheckErrors(await _itemIssueManager.UpdateAsync(itemIssue));


            //Update Item Receipt Item and Journal Item
            var itemIssueItems = await _itemIssueItemRepository.GetAll().Include(u=>u.DeliveryScheduleItem).Include(u => u.SaleOrderItem).Where(u => u.ItemIssueId == input.Id).ToListAsync();


            var oldOrderIds = itemIssueItems
                         .Where(s => s.SaleOrderItem != null)
                         .GroupBy(s => s.SaleOrderItem.SaleOrderId)
                         .Select(s => s.Key).ToList();

            var oldDeliveryIds = itemIssueItems
                         .Where(s => s.DeliveryScheduleItem != null)
                         .GroupBy(s => s.DeliveryScheduleItem.DeliveryScheduleId)
                         .Select(s => s.Key).ToList();


            var newOrderIds = input.Items.Where(s => s.SaleOrderId.HasValue).GroupBy(s => s.SaleOrderId).Select(s => s.Key.Value).ToList();
            var newDeliveryIds = input.Items.Where(s => s.DeliveryScheduleId.HasValue).GroupBy(s => s.DeliveryScheduleId).Select(s => s.Key.Value).ToList();
            var orderIds = oldOrderIds.Union(newOrderIds).ToList();

            var deliveryIds = oldDeliveryIds.Union(newDeliveryIds).ToList();

            if (input.ReceiveFrom == ReceiveFrom.Invoice)
            {
                var orderIdsFromInvoice = await _invoiceItemRepository.GetAll()
                                             .Include(s => s.SaleOrderItem)
                                             .Where(s => s.InvoiceId == input.InvoiceId)
                                             .Where(s => s.OrderItemId.HasValue)
                                             .GroupBy(s => s.SaleOrderItem.SaleOrderId)
                                             .Select(s => s.Key)
                                             .ToListAsync();
                if (orderIdsFromInvoice.Any()) orderIds = orderIds.Union(orderIdsFromInvoice).ToList();

                var deliveryIdsFromInvoice = await _invoiceItemRepository.GetAll()
                                           .Include(s => s.DeliveryScheduleItem)
                                           .Where(s => s.InvoiceId == input.InvoiceId)
                                           .Where(s => s.DeliverySchedulItemId.HasValue)
                                           .GroupBy(s => s.DeliveryScheduleItem.DeliveryScheduleId)
                                           .Select(s => s.Key)
                                           .ToListAsync();
                if (deliveryIdsFromInvoice.Any()) deliveryIds = deliveryIds.Union(deliveryIdsFromInvoice).ToList();
            }

            var @inventoryJournalItems = await (_journalItemRepository.GetAll()
                                            .Where(u => u.JournalId == journal.Id &&
                                             u.Identifier != null)
                                        ).ToListAsync();

            var toDeleteItemIssueItem = itemIssueItems.Where(u => !input.Items.Any(i => i.Id != null && i.Id == u.Id)).ToList();
            var toDeletejournalItem = inventoryJournalItems.Where(u => !input.Items.Any(i => u.Identifier != null && i.Id != null && i.Id == u.Identifier)).ToList();


            //validate Item Issue when use on item receceipt customer credit and customer credit
            var customerCredit = await _cusotmerCreditItemRepository.GetAll().Include(u => u.ItemIssueSaleItem)
                .Where(t => (toDeleteItemIssueItem.Any(g => g.Id == t.ItemIssueSaleItemId)) ||
                 (input.Items.Any(g => g.Id != null && g.Id == t.ItemIssueSaleItemId && t.ItemIssueSaleItem != null && g.Qty != t.ItemIssueSaleItem.Qty))).AsNoTracking().CountAsync();
            var itemReceiptCustomerCredit = await _itemReceiptCustomerCreditItemRepository
                .GetAll().Include(u => u.ItemIssueSaleItem).Where(t => toDeleteItemIssueItem.Any(g => g.Id == t.ItemIssueSaleItemId) ||
                (input.Items.Any(g => g.Id != null && g.Id == t.ItemIssueSaleItemId && t.ItemIssueSaleItem != null && g.Qty != t.ItemIssueSaleItem.Qty)))
                .AsNoTracking().CountAsync();

            if (customerCredit > 0 || itemReceiptCustomerCredit > 0)
            {
                throw new UserFriendlyException(L("ItemAlreadyReturnCannotBeModified"));
            }



            if (input.ReceiveFrom == ReceiveFrom.None && input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }

            var ItemIssueItems = (from ItemIssueitem in input.Items where (ItemIssueitem.SaleOrderItemId != null) select ItemIssueitem.SaleOrderItemId);
            var count = ItemIssueItems.Count();
            if (input.ReceiveFrom == ReceiveFrom.SaleOrder && count <= 0)
            {
                throw new UserFriendlyException(L("PleaseAddSaleOrder"));
            }

            //UPdate invoice Id when create from invoice to in voice

            var invoiceItems = await _invoiceItemRepository.GetAll().Where(t => t.InvoiceId == input.InvoiceId).ToListAsync();

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
                    var journalItem = @inventoryJournalItems.ToList().Where(u => u.Identifier == itemIssueItem.Id);
                    if (itemIssueItem != null)
                    {
                        //old
                        if (itemIssueItem.SaleOrderItem != null)
                        {
                            itemIssueItem.UpdateSaleOrderItemId(null);
                        }
                        if(itemIssueItem.DeliveryScheduleItem != null)
                        {
                            itemIssueItem.UpdateDeliveryScheduleItemId(null);
                        }

                        //new
                        itemIssueItem.Update(tenantId, c.ItemId, c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total);
                        itemIssueItem.UpdateLot(c.LotId);
                        CheckErrors(await _itemIssueItemManager.UpdateAsync(itemIssueItem));

                        if (input.ReceiveFrom != ReceiveFrom.None && c.SaleOrderItemId != null)
                        {
                            itemIssueItem.UpdateSaleOrderItemId(c.SaleOrderItemId);
                        }
                        if (input.ReceiveFrom != ReceiveFrom.None && c.DeliveryScheduleId != null)
                        {
                            itemIssueItem.UpdateDeliveryScheduleItemId(c.DeliveryScheduleItemId);
                        }
                    }

                    if (journalItem != null && journalItem.Count() > 0)
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
                                i.UpdateJournalItem(userId, c.PurchaseAccountId, c.Description, c.Total, 0);
                                CheckErrors(await _journalItemManager.UpdateAsync(i));
                            }
                        }
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
                    //insert to item Receipt item
                    var itemIssueItem = ItemIssueItem.Create(tenantId, userId, itemIssue, c.ItemId,
                                                                                 c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total);
                    itemIssueItem.UpdateLot(c.LotId);
                    if (input.ReceiveFrom == ReceiveFrom.SaleOrder &&
                        c.SaleOrderItemId != null)
                    {
                        //Update Purchase Order item when itemreipt status != None                       
                        itemIssueItem.UpdateSaleOrderItemId(c.SaleOrderItemId.Value);
                    }

                    if (input.ReceiveFrom == ReceiveFrom.DeliverySchedule &&
                       c.DeliveryScheduleItemId != null)
                    {
                        //Update Purchase Order item when itemreipt status != None                       
                        itemIssueItem.UpdateDeliveryScheduleItemId(c.DeliveryScheduleItemId.Value);
                    }

                    //Update table invoice and invoice item
                    if (input.ReceiveFrom == ReceiveFrom.Invoice && c.InvoiceItemId != null)
                    {
                        var invoiceItem = invoiceItems.Where(u => u.Id == c.InvoiceItemId.Value).First();
                        invoiceItem.UpdateIssueItemId(itemIssueItem.Id);
                        invoiceItem.UpdateLot(itemIssueItem.LotId);
                        //Update Purchase Order item when itemreipt status != None

                        //if (c.SaleOrderItemId != null)
                        //{
                        //    itemIssueItem.UpdateSaleOrderItemId(c.SaleOrderItemId.Value);
                        //}
                        CheckErrors(await _invoiceItemManager.UpdateAsync(invoiceItem));

                    }

                    CheckErrors(await _itemIssueItemManager.CreateAsync(itemIssueItem));
                    //insert inventory journal item into debit
                    var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal, c.InventoryAccountId, c.Description, 0, c.Total, PostingKey.Inventory, itemIssueItem.Id);
                    CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));
                    //insert cogs journal item into debit
                    var cogsJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal, c.PurchaseAccountId, c.Description, c.Total, 0, PostingKey.COGS, itemIssueItem.Id);
                    CheckErrors(await _journalItemManager.CreateAsync(cogsJournalItem));

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
            foreach (var t in toDeleteItemIssueItem.OrderBy(u => u.Id))
            {
                var iv = invoiceItems.Where(u => u.ItemIssueItemId == t.Id).FirstOrDefault();

                if (iv != null)
                {
                    iv.UpdateIssueItemId(null);
                    var inv = invoiceItems.Where(c => c.InvoiceId == input.InvoiceId).FirstOrDefault();

                    if (inv != null)
                    {
                        inv.Invoice.UpdateItemIssueId(null);
                    }
                }

                CheckErrors(await _itemIssueItemManager.RemoveAsync(t));

            }

            var scheduleItems = input.Items.Select(s => s.ItemId).Concat(toDeleteItemIssueItem.Select(s => s.ItemId)).Distinct().ToList();


            foreach (var t in toDeletejournalItem)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(t));
            }
            if (input.InvoiceId != null && input.Status == TransactionStatus.Publish && input.ReceiveFrom.Equals("Invoice"))
            {
                var invoice = await _invoiceRepository.GetAll().Where(u => u.Id == input.InvoiceId).FirstOrDefaultAsync();
                invoice.UpdateReceivedStatus(DeliveryStatus.ShipAll);
                CheckErrors(await _invoiceManager.UpdateAsync(invoice));
            }

            if (orderIds.Any() || deliveryIds.Any())
            {
                await CurrentUnitOfWork.SaveChangesAsync();
                foreach (var id in orderIds)
                {
                    await UpdateOrderInventoryStatus(id);
                }
                foreach (var id in deliveryIds)
                {
                    await UpdateDeliveryInventoryStatus(id, true);
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
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, scheduleDate, scheduleItems);
            }

            return new NullableIdDto<Guid>() { Id = itemIssue.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_UpdateStatusToDraft,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Draft)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input)
        {
            var @entity = await _journalRepository
                              .GetAll()
                              .Include(u => u.ItemIssue)
                              .Where(u => (u.JournalType == JournalType.ItemIssueSale ||
                                    u.JournalType == JournalType.ItemIssueOther ||
                                     u.JournalType == JournalType.ItemIssueTransfer ||
                                    u.JournalType == JournalType.ItemIssueAdjustment ||
                                    u.JournalType == JournalType.ItemIssueVendorCredit ||
                                    u.JournalType == JournalType.ItemIssueProduction)
                                    && u.ItemIssueId == input.Id)
                              .FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }


            // validate item reciept type sale by invoice
            var validate = (from invoice in _invoiceRepository.GetAll().Include(t => t.ItemIssue)
                            .Where(t => t.ItemIssueId == input.Id && t.ItemIssueId != null
                            && t.CreationTime > t.ItemIssue.CreationTime
                            && @entity.JournalType == JournalType.ItemIssueSale)
                            select invoice).Count();
            if (validate > 0)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }


            //update receive status of transfer order to pending 
            if (entity.JournalType == JournalType.ItemIssueTransfer)
            {
                var @transfer = await _transferOrderRepository.GetAll().Where(u => u.Id == entity.ItemIssue.TransferOrderId).FirstOrDefaultAsync();
                // update received status 
                if (@transfer != null)
                {
                    @transfer.UpdateShipedStatus(TransferStatus.Pending);
                    CheckErrors(await _transferOrderManager.UpdateAsync(@transfer));
                }
            }
            if (entity.JournalType == JournalType.ItemIssueProduction)
            {
                var production = await _productionOrderRepository.GetAll().Where(u => u.Id == entity.ItemIssue.ProductionOrderId).FirstOrDefaultAsync();
                // update received status 
                if (production != null)
                {
                    production.UpdateShipedStatus(TransferStatus.Pending);
                    CheckErrors(await _productionOrderManager.UpdateAsync(production));
                }
            }

            if (entity.JournalType == JournalType.ItemIssueSale)
            {
                //query get item receipt item and update totalItemIssue 
                var itemIssueItems = await _itemIssueItemRepository.GetAll().Include(u => u.SaleOrderItem)
                    .Where(u => u.ItemIssueId == entity.ItemIssueId).ToListAsync();
                var querySOHeader = (from so in _saleOrderRepository.GetAll()
                                     join si in _saleOrderItemRepository.GetAll().Include(u => u.Item)
                                     on so.Id equals si.SaleOrderId
                                     where (itemIssueItems.Any(t => t.SaleOrderItemId == si.Id))
                                     group si by so into u
                                     select new { poId = u.Key.Id }
                    );

                // temp of so id header 
                var listOfSoHeader = new List<CreateOrUpdateItemIssueItemInput>();

                foreach (var i in querySOHeader)
                {
                    listOfSoHeader.Add(new CreateOrUpdateItemIssueItemInput
                    {
                        SaleOrderId = i.poId
                    });
                }

                if (entity.ItemIssue.ReceiveFrom == ReceiveFrom.SaleOrder && entity.ItemIssue != null)
                {

                    foreach (var iii in itemIssueItems)
                    {
                        if (iii.SaleOrderItemId != null && iii.SaleOrderItem != null)
                        {
                            //Update sale Order item when delete Item Receipt                            
                        }

                    }
                    //update Receive status to pending                 
                }


                if (entity.ItemIssue.ReceiveFrom == ReceiveFrom.Invoice && entity.ItemIssue != null)
                {
                    var invoice = await _invoiceRepository.GetAll().Where(u => u.ItemIssueId == entity.ItemIssueId).FirstOrDefaultAsync();
                    if (invoice != null) //update revers status in receive status of invoice to pending 
                    {
                        invoice.UpdateReceivedStatus(DeliveryStatus.ShipPending);
                        CheckErrors(await _invoiceManager.UpdateAsync(invoice));
                    }
                }
                // end code
            }
            entity.UpdateStatusToDraft();

            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);
            CheckErrors(await _journalManager.UpdateAsync(entity, autoSequence.DocumentType));

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_UpdateStatusToPublish,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Publish)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input)
        {
            var @entity = await _journalRepository
                                .GetAll()
                                .Include(u => u.ItemIssue)
                                .Include(u => u.Bill)
                               .Where(u => (u.JournalType == JournalType.ItemIssueSale ||
                                    u.JournalType == JournalType.ItemIssueOther ||
                                     u.JournalType == JournalType.ItemIssueTransfer ||
                                    u.JournalType == JournalType.ItemIssueAdjustment ||
                                    u.JournalType == JournalType.ItemIssueVendorCredit ||
                                    u.JournalType == JournalType.ItemIssueProduction)
                                    && u.ItemIssueId == input.Id)
                                .FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            // validate item reciept type sale by invoice
            //var validate = (from invoice in _invoiceRepository.GetAll()
            //                .Where(t => t.ItemIssueId == input.Id && t.ItemIssueId != null
            //                        && @entity.JournalType == JournalType.ItemIssueSale)
            //                select invoice).ToList().Count();
            //if (validate > 0)
            //{
            //    throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            //}


            //update receive status of transfer order to ship all 
            if (entity.JournalType == JournalType.ItemIssueTransfer)
            {
                var @transfer = await _transferOrderRepository.GetAll().Where(u => u.Id == entity.ItemIssue.TransferOrderId).FirstOrDefaultAsync();
                // update received status 
                if (@transfer != null)
                {
                    @transfer.UpdateShipedStatus(TransferStatus.ShipAll);
                    CheckErrors(await _transferOrderManager.UpdateAsync(@transfer));
                }
            }

            if (entity.JournalType == JournalType.ItemIssueProduction)
            {
                var production = await _productionOrderRepository.GetAll().Where(u => u.Id == entity.ItemIssue.ProductionOrderId).FirstOrDefaultAsync();
                // update received status 
                if (production != null)
                {
                    production.UpdateShipedStatus(TransferStatus.ShipAll);
                    CheckErrors(await _productionOrderManager.UpdateAsync(production));
                }
            }

            if (entity.JournalType == JournalType.ItemIssueSale)
            {
                var itemIssueItems = await _itemIssueItemRepository.GetAll().Include(u => u.SaleOrderItem)
                    .Where(u => u.ItemIssueId == entity.ItemIssueId).ToListAsync();
                if (entity.ItemIssue.ReceiveFrom == ReceiveFrom.Invoice && entity.ItemIssue != null)
                {
                    var invoice = await _invoiceRepository.GetAll().Where(u => u.ItemIssueId == entity.ItemIssueId).FirstOrDefaultAsync();
                    if (invoice != null)
                    {
                        invoice.UpdateReceivedStatus(DeliveryStatus.ShipAll);
                        CheckErrors(await _invoiceManager.UpdateAsync(invoice));
                    }
                }
                // end code
            }
            entity.UpdatePublish();
            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);
            CheckErrors(await _journalManager.UpdateAsync(entity, autoSequence.DocumentType));

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_UpdateStatusToVoid,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Void)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input)
        {
            var @jounal = await _journalRepository.GetAll()
                 .Include(u => u.ItemIssue)
                 .Include(u => u.Bill)
                 .Include(u => u.ItemIssue.ShippingAddress)
                 .Include(u => u.ItemIssue.BillingAddress)
                 .Where(u => (u.JournalType == JournalType.ItemIssueSale ||
                                    u.JournalType == JournalType.ItemIssueOther ||
                                     u.JournalType == JournalType.ItemIssueTransfer ||
                                    u.JournalType == JournalType.ItemIssueAdjustment ||
                                    u.JournalType == JournalType.ItemIssueVendorCredit ||
                                    u.JournalType == JournalType.ItemIssueProduction)
                                    && u.ItemIssueId == input.Id)
                 .FirstOrDefaultAsync();

            // validate item issue type ItemIssueSale by invoice
            var validate = (from invoice in _invoiceRepository.GetAll().Include(t => t.ItemIssue)
                            .Where(t => t.ItemIssueId == input.Id && t.ItemIssueId != null
                            && jounal.JournalType == JournalType.ItemIssueSale
                            && t.CreationTime > t.ItemIssue.CreationTime)
                            select invoice).ToList().Count();
            if (validate > 0)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }

            //update receive status of transfer order to pending 
            if (jounal.JournalType == JournalType.ItemIssueTransfer)
            {
                var @transfer = await _transferOrderRepository.GetAll().Where(u => u.Id == jounal.ItemIssue.TransferOrderId).FirstOrDefaultAsync();
                // update received status 
                if (@transfer != null)
                {
                    @transfer.UpdateShipedStatus(TransferStatus.Pending);
                    CheckErrors(await _transferOrderManager.UpdateAsync(@transfer));
                }
            }

            if (jounal.JournalType == JournalType.ItemIssueProduction)
            {
                var production = await _productionOrderRepository.GetAll().Where(u => u.Id == jounal.ItemIssue.ProductionOrderId).FirstOrDefaultAsync();
                // update received status 
                if (production != null)
                {
                    production.UpdateShipedStatus(TransferStatus.Pending);
                    CheckErrors(await _productionOrderManager.UpdateAsync(production));
                }
            }

            //query get item issue item and update total Qty 
            if (jounal.JournalType == JournalType.ItemIssueSale)
            {

                var invoice = (_invoiceRepository.GetAll().Where(u => u.ItemIssueId == jounal.ItemIssueId)).FirstOrDefault();


                if (jounal.ItemIssue.ReceiveFrom == ReceiveFrom.Invoice)
                {
                    invoice.UpdateReceivedStatus(DeliveryStatus.ShipPending);
                    CheckErrors(await _invoiceManager.UpdateAsync(invoice));
                }
                //query get item receipt item and update totalItemIssue 
                var @ItemIssueItems = await _itemIssueItemRepository.GetAll().Include(u => u.SaleOrderItem)
                    .Where(u => u.ItemIssueId == jounal.ItemIssueId).ToListAsync();

                if (jounal.ItemIssue.ReceiveFrom == ReceiveFrom.SaleOrder && jounal.ItemIssue != null)
                {
                    //update Receive status to pending                   
                }
                if (jounal.ItemIssue.ReceiveFrom == ReceiveFrom.Invoice && jounal.ItemIssue != null)
                {
                }
            }
            jounal.UpdateVoid();

            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);
            CheckErrors(await _journalManager.UpdateAsync(jounal, autoSequence.DocumentType));

            return new NullableIdDto<Guid>() { Id = jounal.ItemIssueId };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssues)]
        public async Task<PagedResultDto<ItemIssueSummaryOutput>> GetItemIssues(GetItemIssueInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var customerTypeMemberIds = await GetCustomerTypeMembers().Select(s => s.CustomerTypeId).ToListAsync();

            var customerQuery = GetCustomers(null, new List<Guid>(), null, customerTypeMemberIds);
            var currencyQuery = GetCurrencies();

            var iiQuery = from ii in _itemIssueRepository.GetAll()
                              .Where(t => t.TransactionType == InventoryTransactionType.ItemIssueSale && t.ConvertToInvoice == true)
                              .AsNoTracking()
                              .Select(s => new
                              {
                                  Id = s.Id,
                                  CustomerId = s.CustomerId,
                                  Total = s.Total
                              })
                          join iii in _itemIssueItemRepository.GetAll()
                                    .AsNoTracking()
                                    .Select(s => s.ItemIssueId)
                          on ii.Id equals iii
                          into iis
                          where iis.Count() > 0
                          select new
                          {
                              Id = ii.Id,
                              CustomerId = ii.CustomerId,
                              Total = ii.Total,
                              ItemCount = iis.Count()
                          };

            var jQuery = from j in _journalRepository.GetAll()
                          .WhereIf(userGroups != null && userGroups.Count > 0, u => userGroups.Contains(u.LocationId.Value))
                          .WhereIf(!input.Filter.IsNullOrEmpty(),
                                u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                u.Reference.ToLower().Contains(input.Filter.ToLower()))
                          .Where(u => u.Status == TransactionStatus.Publish)
                          .AsNoTracking()
                          .Select(j => new
                          {
                              ItemIssueId = j.ItemIssueId,
                              CurrencyId = j.CurrencyId,
                              Date = j.Date,
                              JournalNo = j.JournalNo,
                              Reference = j.Reference,
                              CreationTimeIndex = j.CreationTimeIndex
                          })
                         join c in currencyQuery
                         on j.CurrencyId equals c.Id
                         select new
                         {
                             ItemIssueId = j.ItemIssueId,
                             CurrencyId = j.CurrencyId,
                             Date = j.Date,
                             JournalNo = j.JournalNo,
                             Reference = j.Reference,
                             CreationTimeIndex = j.CreationTimeIndex,
                             CurrencyCode = c.Code
                         };

            var query = from ii in iiQuery
                        join c in customerQuery
                        on ii.CustomerId equals c.Id
                        join j in jQuery
                        on ii.Id equals j.ItemIssueId
                        join i in _invoiceRepository.GetAll()
                                  .AsNoTracking()
                                  .Select(s => s.ItemIssueId)
                        on ii.Id equals i
                        into ivs
                        where ivs.Count() == 0
                        select new ItemIssueSummaryOutput
                        {
                            CountItems = ii.ItemCount,
                            Customer = new CustomerSummaryOutput
                            {
                                Id = c.Id,
                                CustomerName = c.CustomerName
                            },
                            CustomerId = ii.CustomerId.Value,
                            Id = ii.Id,
                            ItemIssueNo = j.JournalNo,
                            CurrencyId = j.CurrencyId,
                            Currency = new CurrencyDetailOutput
                            {
                                Id = j.CurrencyId,
                                Code = j.CurrencyCode
                            },
                            Date = j.Date,
                            Reference = j.Reference,
                            Total = ii.Total,
                            CreationIndex = j.CreationTimeIndex
                        };

            var resultCount = await query.CountAsync();
            if (resultCount == 0) return new PagedResultDto<ItemIssueSummaryOutput>(resultCount, new List<ItemIssueSummaryOutput>());

            var @entities = await query.OrderBy(s => s.Date.Date).ThenBy(t => t.CreationIndex).PageBy(input).ToListAsync();
            return new PagedResultDto<ItemIssueSummaryOutput>(resultCount, entities);

        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssues)]
        public async Task<PagedResultDto<ItemIssueSummaryOutput>> GetItemIssuesOld(GetItemIssueInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var @query = (from ii in _itemIssueRepository.GetAll().AsNoTracking()
                           .Where(t => t.TransactionType == InventoryTransactionType.ItemIssueSale && t.ConvertToInvoice == true)
                          join iii in _itemIssueItemRepository.GetAll()
                          .AsNoTracking() on ii.Id equals iii.ItemIssueId into count
                          join j in _journalRepository.GetAll().Include(u => u.Currency)
                          .WhereIf(userGroups != null && userGroups.Count > 0,
                                u => userGroups.Contains(u.LocationId.Value))
                          .WhereIf(!input.Filter.IsNullOrEmpty(),
                                u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) || u.Reference.ToLower().Contains(input.Filter.ToLower()))
                          //.WhereIf(input.Locations != null && input.Locations.Count > 0, t => input.Locations.Contains(t.LocationId))
                          .Where(u => u.Status == TransactionStatus.Publish)
                          .AsNoTracking()
                          on ii.Id equals j.ItemIssueId
                          join b in _invoiceRepository.GetAll().AsNoTracking() on ii.Id equals b.ItemIssueId into ib
                          from n in ib.DefaultIfEmpty()
                          where n == null
                          select new ItemIssueSummaryOutput
                          {
                              CountItems = count.Count(),
                              Customer = ObjectMapper.Map<CustomerSummaryOutput>(ii.Customer),
                              CustomerId = ii.CustomerId.Value,
                              Id = ii.Id,
                              ItemIssueNo = j.JournalNo,
                              CurrencyId = j.CurrencyId,
                              Currency = ObjectMapper.Map<CurrencyDetailOutput>(j.Currency),
                              Date = j.Date,
                              Reference = j.Reference,
                              Total = ii.Total,
                              CreationIndex = j.CreationTimeIndex
                          }).Where(u => u.CountItems > 0);
            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).ThenBy(t => t.CreationIndex).PageBy(input).ToListAsync();
            return new PagedResultDto<ItemIssueSummaryOutput>(resultCount, entities);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssueItems)]
        public async Task<ItemIssueSummaryOutputForItemIssueItem> GetItemIssueItems(EntityDto<Guid> input)
        {
            var @journal = await _journalRepository
                                .GetAll()
                                .Include(u => u.ItemIssue)
                                .Include(u => u.Location)
                                .Include(u => u.ItemIssue.TransactionTypeSale)
                                .Include(u => u.Currency)
                                .Include(u => u.ItemIssue.Customer)
                                .Include(u => u.ItemIssue.Customer.Account)
                                .Include(u => u.Class)
                                .Include(u => u.ItemIssue.ShippingAddress)
                                .Include(u => u.ItemIssue.BillingAddress)
                                .AsNoTracking()
                                .Where(u => u.JournalType == JournalType.ItemIssueSale && u.ItemIssueId == input.Id)
                                .FirstOrDefaultAsync();

            if (@journal == null || @journal.ItemIssue == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            var itemIssueItemCOGSs = await _itemIssueItemRepository.GetAll()
               .AsNoTracking()
               .Include(u => u.Item)
               .Include(u => u.DeliveryScheduleItem.DeliverySchedule)
               .Include(u => u.SaleOrderItem.SaleOrder)
               .Where(u => u.ItemIssueId == input.Id)
               .Join(
                   _journalItemRepository.GetAll()
                   .Where(u => u.Key == PostingKey.COGS)
                   .Include(u => u.Account)
                   .AsNoTracking()
                   ,
                   u => u.Id, s => s.Identifier,
                   (issueItem, jItem) =>
                   new ItemIssueItemDetailOutput()
                   {
                       Id = issueItem.Id,
                       PurchaseAccountId = jItem.AccountId,
                       PurchaseAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                   }).ToListAsync();

            var itemIssueItems = await _itemIssueItemRepository.GetAll()
                .AsNoTracking()
                .Include(u => u.Item.InventoryAccount)
                .Include(u => u.SaleOrderItem.SaleOrder)
                .Include(u => u.DeliveryScheduleItem.DeliverySchedule.SaleOrder)
                .Include(u => u.DeliveryScheduleItem.SaleOrderItem.SaleOrder)
                .Include(u => u.Lot)
                .Where(u => u.ItemIssueId == input.Id)
                .OrderBy(t => t.CreationTime)
                .Join(_journalItemRepository.GetAll()
                    .Where(u => u.Key == PostingKey.Inventory)
                    .Include(u => u.Account)
                    .AsNoTracking(), u => u.Id, s => s.Identifier,
                    (issueItem, jItem) =>
                    new ItemIssueItemDetailOutput()
                    {
                        SaleOrderId = issueItem.SaleOrderItem != null ? issueItem.SaleOrderItem.SaleOrderId : (Guid?)null,
                        DeliveryScheduleId = issueItem.DeliveryScheduleItem != null ? issueItem.DeliveryScheduleItem.DeliveryScheduleId : (Guid?)null,
                        FromLotDetail = ObjectMapper.Map<LotSummaryOutput>(issueItem.Lot),
                        FromLotId = issueItem.LotId,
                        SaleOrderNumber = issueItem.SaleOrderItem != null ? issueItem.SaleOrderItem.SaleOrder.OrderNumber : issueItem.DeliveryScheduleItem != null ? issueItem.DeliveryScheduleItem.SaleOrderItem.SaleOrder.OrderNumber : null,
                        SaleOrderReference = issueItem.SaleOrderItem != null ? issueItem.SaleOrderItem.SaleOrder.Reference : issueItem.DeliveryScheduleItem != null ? issueItem.DeliveryScheduleItem.SaleOrderItem.SaleOrder.Reference : null,
                        DeliveryNo = issueItem.DeliveryScheduleItem != null ? issueItem.DeliveryScheduleItem.DeliverySchedule.DeliveryNo : null,
                        DeliveryReference = issueItem.DeliveryScheduleItem != null ?  issueItem.DeliveryScheduleItem.DeliverySchedule.Reference : null,
                        Id = issueItem.Id,
                        Item = ObjectMapper.Map<ItemSummaryDetailOutput>(issueItem.Item),
                        ItemId = issueItem.ItemId,
                        SaleOrderItem = issueItem.SaleOrderItem != null ? ObjectMapper.Map<SaleOrderItemSummaryOut>(issueItem.SaleOrderItem) : issueItem.DeliveryScheduleItem != null ? ObjectMapper.Map<SaleOrderItemSummaryOut>(issueItem.DeliveryScheduleItem.SaleOrderItem) : null,
                        DeliveryScheduleItem = ObjectMapper.Map<DeliveryItemSummaryOut>(issueItem.DeliveryScheduleItem),
                        InventoryAccountId = jItem.AccountId,
                        InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                        Description = issueItem.Description,
                        DiscountRate = issueItem.DiscountRate,
                        Qty = issueItem.Qty,
                        Total = issueItem.SaleOrderItem != null ? issueItem.SaleOrderItem.Total : 0,
                        MultiCurrencyUnitCost = issueItem.SaleOrderItem != null ? issueItem.SaleOrderItem.MultiCurrencyUnitCost : 0,
                        MultiCurrencyTotal = issueItem.SaleOrderItem != null ? issueItem.SaleOrderItem.MultiCurrencyTotal : 0,
                        UnitCost = issueItem.SaleOrderItem != null ? issueItem.SaleOrderItem.UnitCost : issueItem.Item.SalePrice ?? 0,
                        SaleOrderItemId = issueItem.SaleOrderItemId,
                        DeliveryScheduleItemId = issueItem.DeliverySchedulItemId,
                        PurchaseAccountId = itemIssueItemCOGSs.Where(r => r.Id == issueItem.Id).Select(r => r.PurchaseAccountId).FirstOrDefault(),
                        PurchaseAccount = itemIssueItemCOGSs.Where(r => r.Id == issueItem.Id).Select(r => r.PurchaseAccount).FirstOrDefault(),
                        UseBatchNo = issueItem.Item.UseBatchNo,
                        TrackSerial = issueItem.Item.TrackSerial,
                        TrackExpiration = issueItem.Item.TrackExpiration
                    }).ToListAsync();


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

            var result = ObjectMapper.Map<ItemIssueSummaryOutputForItemIssueItem>(journal.ItemIssue);
            result.IssueNo = journal.JournalNo;
            result.TransactionSaleTypeId = journal.ItemIssue.TransactionTypeSaleId;
            result.Date = journal.Date;
            result.ClassId = journal.ClassId;
            result.LocationId = journal.LocationId.Value;
            result.CurrencyId = journal.CurrencyId;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.Reference = journal.Reference;
            result.ItemIssueItems = itemIssueItems;
            result.Memo = journal.Memo;
            result.LocationId = journal.LocationId.Value;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            return result;
        }

        #region get item issue for credit 
        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssues,
                      AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssues_ForItemReceiptCustomerCrdit,
                      AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssues_ForCustomerCredit,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptCustomerCredit_CanIssueSale)]
        public async Task<PagedResultDto<ItemIssueSummaryOutput>> GetItemIssueForCutomerCredit(GetItemIssueInput input, Guid? exceptId)
        {

            var roundDigits = await _accountCycleRepository.GetAll().Select(t => t.RoundingDigit).FirstOrDefaultAsync();
            var userGroups = await GetUserGroupByLocation();

            var @query = from ir in _itemIssueRepository.GetAll()
                           .Include(u => u.Customer)
                           .Where(t => t.TransactionType == InventoryTransactionType.ItemIssueSale)
                           //  .WhereIf(input.Customers != null && input.Customers.Count > 0, u => input.Customers.Contains(u.CustomerId.Value))
                           .AsNoTracking()
                         join j in _journalRepository.GetAll()
                         .Include(u => u.Currency)
                         .WhereIf(!input.Filter.IsNullOrEmpty(), u =>
                               u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                               u.Reference.ToLower().Contains(input.Filter.ToLower()))
                         // .WhereIf(input.Locations != null && input.Locations.Count > 0, t => input.Locations.Contains(t.LocationId))
                         .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                         .Where(u => u.Status == TransactionStatus.Publish)
                         .AsNoTracking()
                         on ir.Id equals j.ItemIssueId

                         join iri in _itemIssueItemRepository.GetAll().AsNoTracking()
                         on ir.Id equals iri.ItemIssueId
                         into items

                         from item in items
                         join vci in _cusotmerCreditItemRepository.GetAll().
                         Where(s => s.ItemIssueSaleItemId != null)
                         .WhereIf(exceptId != null, s => s.CustomerCreditId != exceptId)
                         .AsNoTracking()
                         on item.Id equals vci.ItemIssueSaleItemId
                         into returnItems

                         join ivi in _itemReceiptCustomerCreditItemRepository.GetAll()
                                .Where(s => s.ItemIssueSaleItemId != null)
                                 .WhereIf(exceptId != null, s => s.ItemReceiptCustomerCreditId != exceptId)
                                .AsNoTracking()
                         on item.Id equals ivi.ItemIssueSaleItemId
                         into returnItems2

                         let returnQty = returnItems == null ? 0 : returnItems.Sum(s => s.Qty)
                         let returnQty2 = returnItems2 == null ? 0 : returnItems2.Sum(s => s.Qty)
                         let itemCount = items.Count(s => s.Qty > returnQty + returnQty2)

                         where itemCount > 0 && (returnQty + returnQty2 < item.Qty)

                         select
                         new
                         {
                             Journal = j,
                             ItemReceipt = ir,
                             Customer = ir.Customer,
                             Count = itemCount,
                             Qty = item.Qty - returnQty - returnQty2,
                             UnitCost = item.UnitCost,
                         }
                         into lists
                         group lists by new
                         {
                             Journal = lists.Journal,
                             ItemReceipt = lists.ItemReceipt,
                             Customer = lists.Customer,
                             Count = lists.Count
                         }
                         into g

                         select new ItemIssueSummaryOutput
                         {
                             CountItems = g.Key.Count,
                             Customer = ObjectMapper.Map<CustomerSummaryOutput>(g.Key.Customer),
                             CustomerId = g.Key.ItemReceipt.CustomerId.Value,
                             Id = g.Key.ItemReceipt.Id,
                             ItemIssueNo = g.Key.Journal.JournalNo,
                             CurrencyId = g.Key.Journal.CurrencyId,
                             Currency = ObjectMapper.Map<CurrencyDetailOutput>(g.Key.Journal.Currency),
                             Date = g.Key.Journal.Date,
                             Reference = g.Key.Journal.Reference,
                             Total = g.Sum(s => Math.Round(s.UnitCost * s.Qty, roundDigits)),
                             CreationIndex = g.Key.Journal.CreationTimeIndex,
                         };

            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).ThenBy(t => t.CreationIndex).PageBy(input).ToListAsync();
            return new PagedResultDto<ItemIssueSummaryOutput>(resultCount, entities);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssueItems, AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptCustomerCredit_CanIssueSale)]
        public async Task<ItemIssueSummaryOutputForItemIssueItem> GetItemIssueItemForCustomerCredit(EntityDto<Guid> input, Guid? exceptId)
        {
            var @journal = await _journalRepository
                                           .GetAll()
                                           .Include(u => u.ItemIssue)
                                           .Include(u => u.Currency)
                                           .Include(u => u.ItemIssue.Customer)
                                           .Include(u => u.ItemIssue.Customer.Account)
                                           .Include(u => u.Class)
                                           .Include(u => u.Location)
                                           .Include(u => u.ItemIssue.ShippingAddress)
                                           .Include(u => u.ItemIssue.BillingAddress)
                                           .Where(u => u.JournalType == JournalType.ItemIssueSale && u.ItemIssueId == input.Id)
                                           .AsNoTracking()
                                           .FirstOrDefaultAsync();

            if (@journal == null || @journal.ItemIssue == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var roundDigits = await _accountCycleRepository.GetAll().Select(t => t.RoundingDigit).FirstOrDefaultAsync();


            var query = from iri in _itemIssueItemRepository.GetAll()
                            .Include(u => u.Item.SaleAccount)
                            .Include(s => s.SaleOrderItem.SaleOrder)
                             .Include(s => s.DeliveryScheduleItem.DeliverySchedule.SaleOrder)
                            .Include(u => u.Lot)
                            .Where(u => u.ItemIssueId == input.Id)
                            .AsNoTracking()
                        join ji in _journalItemRepository.GetAll()
                            .Include(u => u.Account)
                            .Where(u => u.JournalId == journal.Id && u.Key == PostingKey.Inventory)
                            .AsNoTracking()
                        on iri.Id equals ji.Identifier

                        join invi in _invoiceItemRepository.GetAll().AsNoTracking()
                        on iri.Id equals invi.ItemIssueItemId
                        into invItems

                        join returnItem in _cusotmerCreditItemRepository.GetAll()
                            .Where(s => s.ItemIssueSaleItemId != null)
                             .WhereIf(exceptId != null, s => s.CustomerCreditId != exceptId)
                            .AsNoTracking()
                        on iri.Id equals returnItem.ItemIssueSaleItemId
                        into returnItems

                        join returnItem2 in _itemReceiptCustomerCreditItemRepository.GetAll()
                           .Where(s => s.ItemIssueSaleItemId != null)
                           .WhereIf(exceptId != null, s => s.ItemReceiptCustomerCreditId != exceptId)
                           .AsNoTracking()
                        on iri.Id equals returnItem2.ItemIssueSaleItemId
                        into returnItems2

                        let returnQty = returnItems == null ? 0 : returnItems.Sum(s => s.Qty)
                        let returnQty2 = returnItems2 == null ? 0 : returnItems2.Sum(s => s.Qty)
                        let remainingQty = iri.Qty - returnQty - returnQty2
                        let sellingPrice = invItems == null || invItems.Count() == 0 ? 0 : invItems.FirstOrDefault().UnitCost

                        where returnQty + returnQty2 < iri.Qty

                        group iri by new
                        {
                            IItem = iri,
                            Qty = remainingQty,
                            Total = Math.Round(iri.UnitCost * remainingQty, roundDigits),
                            JItem = ji,
                            Item = iri.Item,
                            SellingPrice = sellingPrice,
                            OrderNo = iri.SaleOrderItem != null  ? iri.SaleOrderItem.SaleOrder.OrderNumber : iri.DeliveryScheduleItem != null ? iri.DeliveryScheduleItem.DeliverySchedule.SaleOrder.OrderNumber : null ,
                            Reference = iri.SaleOrderItem != null ?  iri.SaleOrderItem.SaleOrder.Reference : iri.DeliveryScheduleItem != null ? iri.DeliveryScheduleItem.DeliverySchedule.SaleOrder.Reference : null,
                            DeliveryNo = iri.DeliveryScheduleItem == null ? "" : iri.DeliveryScheduleItem.DeliverySchedule.DeliveryNo,
                            DeliveryReference = iri.DeliveryScheduleItem == null ? "" : iri.DeliveryScheduleItem.DeliverySchedule.Reference,
                            DeliveryId = iri.DeliveryScheduleItem == null ? (Guid?)null: iri.DeliveryScheduleItem.DeliveryScheduleId,
                        }
                        into g
                        orderby g.Key.IItem.CreationTime
                        select
                        new ItemIssueItemDetailOutput()
                        {
                            Id = g.Key.IItem.Id,
                            Item = ObjectMapper.Map<ItemSummaryDetailOutput>(g.Key.Item),
                            ItemId = g.Key.IItem.ItemId,
                            InventoryAccountId = g.Key.JItem.AccountId,
                            InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(g.Key.JItem.Account),
                            PurchaseAccountId = g.Key.Item.PurchaseAccountId.Value,
                            Description = g.Key.IItem.Description,
                            DiscountRate = g.Key.IItem.DiscountRate,
                            Qty = g.Key.Qty,
                            Total = g.Key.Total,
                            UnitCost = g.Key.SellingPrice,
                            SellingPrice = g.Key.SellingPrice,
                            ToLotId = g.Key.IItem.LotId,
                            ToLotDetail = ObjectMapper.Map<LotSummaryOutput>(g.Key.IItem.Lot),
                            SaleOrderNumber = g.Key.OrderNo,
                            SaleOrderReference = g.Key.Reference,
                            DeliveryNo = g.Key.DeliveryNo,
                            DeliveryReference = g.Key.DeliveryReference,
                            DeliveryId = g.Key.DeliveryId,
                        };


            var itemIssueItems = await query.ToListAsync();

            var result = ObjectMapper.Map<ItemIssueSummaryOutputForItemIssueItem>(journal.ItemIssue);
            result.Total = Math.Round(itemIssueItems.Sum(s => s.Total), roundDigits);
            result.IssueNo = journal.JournalNo;
            result.Date = journal.Date;
            result.Reference = journal.Reference;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.LocationId = journal.LocationId.Value;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.ItemIssueItems = itemIssueItems;
            result.Customer = ObjectMapper.Map<CustomerSummaryDetailOutput>(journal.ItemIssue.Customer);
            result.CustomerId = journal.ItemIssue.CustomerId.Value;
            result.Memo = journal.Memo;
            return result;
        }

        #endregion


        #region  get new list for new UI customer credit and item rereceipt customer credit

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssues_ForCustomerCredit, AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptCustomerCredit_CanIssueSale)]
        public async Task<PagedResultDto<ItemIssueForCustomerCreditOutput>> GetNewItemIssueForCutomerCredit(GetForCustomerCreditItemIssueInput input, Guid? exceptId)
        {

            var roundDigits = await _accountCycleRepository.GetAll().Select(t => t.RoundingDigit).FirstOrDefaultAsync();
            var userGroups = await GetUserGroupByLocation();

            var customerCreditItemQuery = from ci in _cusotmerCreditItemRepository.GetAll().
                                                       Where(s => s.ItemIssueSaleItemId != null)
                                                       .WhereIf(exceptId != null, s => s.CustomerCreditId != exceptId)
                                                       .AsNoTracking()
                                          join j in _journalRepository.GetAll()
                                                     .Where(s => s.Status == TransactionStatus.Publish)
                                                     .Where(s => s.CustomerCreditId.HasValue)
                                                     .AsNoTracking()
                                          on ci.CustomerCreditId equals j.CustomerCreditId
                                          join b in _customerCreditBatchNoRepository.GetAll().AsNoTracking()
                                          on ci.Id equals b.CustomerCreditItemId
                                          into bs
                                          from b in bs.DefaultIfEmpty()
                                          select new
                                          {
                                              ItemIssueSaleItemId = ci.ItemIssueSaleItemId,
                                              Qty = b == null ? ci.Qty : b.Qty,
                                              BatchNumber = b == null ? "" : b.BatchNo.Code,
                                              BatchNoId = b == null ? (Guid?)null : b.BatchNoId
                                          };

            var itemReceiptCustomerCreditItemQuery = from irci in _itemReceiptCustomerCreditItemRepository.GetAll()
                                                            .Where(s => s.ItemIssueSaleItemId != null)
                                                            .WhereIf(exceptId != null, s => s.ItemReceiptCustomerCreditId != exceptId)
                                                            .AsNoTracking()
                                                     join j in _journalRepository.GetAll()
                                                            .Where(s => s.Status == TransactionStatus.Publish)
                                                            .Where(s => s.ItemReceiptCustomerCreditId.HasValue)
                                                            .AsNoTracking()
                                                     on irci.ItemReceiptCustomerCreditId equals j.ItemReceiptCustomerCreditId
                                                     join b in _itemReceiptCustomerCreditItemBatchNoRepository.GetAll().AsNoTracking()
                                                     on irci.Id equals b.ItemReceiptCustomerCreditItemId
                                                     into bs
                                                     from b in bs.DefaultIfEmpty()
                                                     select new
                                                     {
                                                         ItemIssueSaleItemId = irci.ItemIssueSaleItemId,
                                                         Qty = b == null ? irci.Qty : b.Qty,
                                                         BatchNumber = b == null ? "" : b.BatchNo.Code,
                                                         BatchNoId = b == null ? (Guid?)null : b.BatchNoId
                                                     };

            var jQuery = from ii in _itemIssueRepository.GetAll()
                                   .Where(t => t.TransactionType == InventoryTransactionType.ItemIssueSale)
                                   .WhereIf(input.Customers != null && input.Customers.Count > 0, u => input.Customers.Contains(u.CustomerId.Value))
                                   .AsNoTracking()
                         join j in _journalRepository.GetAll()
                                  .WhereIf(input.Locations != null && input.Locations.Count > 0, t => input.Locations.Contains(t.LocationId))
                                  .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                                  .WhereIf(input.FromDate != null && input.ToDate != null,
                                         (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                                  .Where(u => u.Status == TransactionStatus.Publish)
                                  .AsNoTracking()
                          on ii.Id equals j.ItemIssueId
                         select new
                         {
                             ItemIssueId = j.ItemIssueId,
                             Date = j.Date,
                             JournalNo = j.JournalNo,
                             j.Reference,
                             CreationTimeIndex = j.CreationTimeIndex
                         };


            var iiQuery = from ii in _itemIssueItemRepository.GetAll()
                                     .WhereIf(input.Lots != null && input.Lots.Count > 0, t => input.Lots.Contains(t.LotId))
                                     .WhereIf(input.Items != null && input.Items.Count > 0, t => input.Items.Contains(t.ItemId))
                                     .AsNoTracking()

                          join o in _saleOrderItemRepository.GetAll()
                                     .AsNoTracking()
                          on ii.SaleOrderItemId equals o.Id
                          into ois
                          from oi in ois.DefaultIfEmpty()


                          join di in _deliveryScheduleItemRepository.GetAll()
                                      .Include(u => u.DeliverySchedule.SaleOrder)
                                      .AsNoTracking()
                          on ii.DeliverySchedulItemId equals di.Id
                          into dis
                          from dsi in dis.DefaultIfEmpty()

                          join iv in _invoiceItemRepository.GetAll()
                                    .Where(s => s.ItemIssueItemId.HasValue && (s.DeliverySchedulItemId.HasValue || s.OrderItemId.HasValue) )                                   
                                    .AsNoTracking()
                          on ii.Id equals iv.ItemIssueItemId
                          into ivs
                          from iv in ivs.DefaultIfEmpty()

                          join b in _itemIssueItemBatchNoRepository.GetAll().AsNoTracking()
                          on ii.Id equals b.ItemIssueItemId
                          into bs
                          from b in bs.DefaultIfEmpty()

                          select new
                          {
                              ItemIssueId = ii.ItemIssueId,
                              ItemId = ii.ItemId,
                              LotId = ii.LotId,
                              Qty = b == null ? ii.Qty : b.Qty,
                              Id = ii.Id,
                              CreationTime = ii.CreationTime,
                              LotName = ii.LotId.HasValue ? ii.Lot.LotName : "",
                              ItemName = ii.Item.ItemName,
                              ItemCode = ii.Item.ItemCode,
                              OrderNo = oi != null ? oi.SaleOrder.OrderNumber : iv != null ? iv.SaleOrderItem.SaleOrder.OrderNumber : dsi.DeliverySchedule != null ? dsi.DeliverySchedule.SaleOrder.OrderNumber : "",
                              OrderRef = oi != null ? oi.SaleOrder.Reference : iv != null ? iv.SaleOrderItem.SaleOrder.Reference : dsi.DeliverySchedule != null ? dsi.DeliverySchedule.SaleOrder.Reference : "",
                              OrderId = oi != null ? oi.SaleOrderId : iv != null ? iv.SaleOrderItem.SaleOrderId : (Guid?)null,
                              
                              DeliveryNo = dsi != null ? dsi.DeliverySchedule.DeliveryNo : iv != null ? iv.DeliveryScheduleItem.DeliverySchedule.DeliveryNo : "",
                              DeliveryRef = dsi != null ? dsi.DeliverySchedule.Reference : iv != null ? iv.DeliveryScheduleItem.DeliverySchedule.Reference : "",
                              DeliveryId = dsi != null ? dsi.DeliveryScheduleId : iv != null ? iv.DeliveryScheduleItem.DeliveryScheduleId : (Guid?)null,

                              UseBatchNo = ii.Item.UseBatchNo,
                              TrackSerial = ii.Item.TrackSerial,
                              TrackExpiration = ii.Item.TrackExpiration,
                              BatchNumber = b == null ? "" : b.BatchNo.Code,
                              BatchNoId = b == null ? (Guid?)null : b.BatchNoId,
                              ExpirationDate = b == null ? (DateTime?)null : b.BatchNo.ExpirationDate
                          };


            var @query = from ii in iiQuery
                         join j in jQuery
                         on ii.ItemIssueId equals j.ItemIssueId

                         join vci in customerCreditItemQuery
                         on $"{ii.Id}-{ii.BatchNoId}" equals $"{vci.ItemIssueSaleItemId}-{vci.BatchNoId}"
                         into returnItems

                         join ivi in itemReceiptCustomerCreditItemQuery
                         on $"{ii.Id}-{ii.BatchNoId}" equals $"{ivi.ItemIssueSaleItemId}-{ivi.BatchNoId}"
                         into returnItems2

                         let returnQty = returnItems == null ? 0 : returnItems.Sum(s => s.Qty)
                         let returnQty2 = returnItems2 == null ? 0 : returnItems2.Sum(s => s.Qty)
                         let remainingQty = ii.Qty - returnQty - returnQty2
                         where remainingQty > 0 && (
                               input.Filter.IsNullOrWhiteSpace() ||
                               j.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                               (!j.Reference.IsNullOrWhiteSpace() && j.Reference.ToLower().Contains(input.Filter.ToLower())) ||
                               (!ii.BatchNumber.IsNullOrWhiteSpace() && ii.BatchNumber.ToLower().Contains(input.Filter.ToLower())))

                         select new ItemIssueForCustomerCreditOutput
                         {
                             CreationTimeIndex = j.CreationTimeIndex,
                             Id = ii.Id,
                             Date = j.Date,
                             Qty = remainingQty,
                             IssueNo = j.JournalNo,
                             LotName = ii.LotName,
                             LotId = ii.LotId,
                             ItemName = ii.ItemName,
                             ItemCode = ii.ItemCode,
                             ItemId = ii.ItemId,
                             ItemIssueId = ii.ItemIssueId,
                             OrderNo = ii.OrderNo,
                             OrderReference = ii.OrderRef,
                             OrderId = ii.OrderId,
                             UseBatchNo = ii.UseBatchNo,
                             BatchNoId = ii.BatchNoId,
                             BatchNumber = ii.BatchNumber,
                             TrackSerial = ii.TrackSerial,
                             TrackExpiration = ii.TrackExpiration,
                             ExpirationDate = ii.ExpirationDate,
                             DeliveryNo = ii.DeliveryNo,
                             DeliveryReference = ii.DeliveryRef,
                             DeliveryId = ii.DeliveryId,
                         };

            var resultCount = await query.CountAsync();
            if (resultCount == 0) return new PagedResultDto<ItemIssueForCustomerCreditOutput>(resultCount, new List<ItemIssueForCustomerCreditOutput>());

            var @entities = await query.OrderBy(s => s.Date.Date).ThenBy(t => t.CreationTimeIndex).PageBy(input).ToListAsync();
            return new PagedResultDto<ItemIssueForCustomerCreditOutput>(resultCount, entities);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssues_ForCustomerCredit)]
        public async Task<PagedResultDto<ItemIssueForCustomerCreditOutput>> GetNewItemIssueForCutomerCreditOld(GetForCustomerCreditItemIssueInput input, Guid? exceptId)
        {

            var roundDigits = await _accountCycleRepository.GetAll().Select(t => t.RoundingDigit).FirstOrDefaultAsync();
            var userGroups = await GetUserGroupByLocation();

            var customerCreditItemQuery = from vci in _cusotmerCreditItemRepository.GetAll().
                                               Where(s => s.ItemIssueSaleItemId != null)
                                               .WhereIf(exceptId != null, s => s.CustomerCreditId != exceptId)
                                               .AsNoTracking()
                                          join cc in _journalRepository.GetAll()
                                                 .Where(s => s.Status == TransactionStatus.Publish)
                                                 .Where(s => s.JournalType == JournalType.CustomerCredit)
                                                 .AsNoTracking()
                                          on vci.CustomerCreditId equals cc.CustomerCreditId
                                          select vci;

            var itemReceiptCustomerCreditItemQuery = from irci in _itemReceiptCustomerCreditItemRepository.GetAll()
                                                            .Where(s => s.ItemIssueSaleItemId != null)
                                                            .WhereIf(exceptId != null, s => s.ItemReceiptCustomerCreditId != exceptId)
                                                            .AsNoTracking()
                                                     join cc in _journalRepository.GetAll()
                                                            .Where(s => s.Status == TransactionStatus.Publish)
                                                            .Where(s => s.JournalType == JournalType.ItemReceiptCustomerCredit)
                                                            .AsNoTracking()
                                                     on irci.ItemReceiptCustomerCreditId equals cc.ItemReceiptCustomerCreditId
                                                     select irci;

            var @query = from ir in _itemIssueRepository.GetAll()
                           .Include(u => u.Customer)
                           .Where(t => t.TransactionType == InventoryTransactionType.ItemIssueSale)
                          .WhereIf(input.Customers != null && input.Customers.Count > 0, u => input.Customers.Contains(u.CustomerId.Value))
                           .AsNoTracking()
                         join j in _journalRepository.GetAll()
                         .Include(u => u.Currency)
                         .WhereIf(!input.Filter.IsNullOrEmpty(), u =>
                               u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                               u.Reference.ToLower().Contains(input.Filter.ToLower()))
                         .WhereIf(input.Locations != null && input.Locations.Count > 0, t => input.Locations.Contains(t.LocationId))
                         .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                         .WhereIf(input.FromDate != null && input.ToDate != null,
                                (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                         .Where(u => u.Status == TransactionStatus.Publish)
                         .AsNoTracking()
                         on ir.Id equals j.ItemIssueId

                         join iri in _itemIssueItemRepository.GetAll().AsNoTracking()
                         .Include(u => u.Item)
                         .Include(u => u.Lot)
                         .WhereIf(input.Lots != null && input.Lots.Count > 0, t => input.Lots.Contains(t.LotId))
                         .WhereIf(input.Items != null && input.Items.Count > 0, t => input.Items.Contains(t.ItemId))
                         on ir.Id equals iri.ItemIssueId
                         into items

                         from item in items
                             //join vci in _cusotmerCreditItemRepository.GetAll().
                             //Where(s => s.ItemIssueSaleItemId != null)
                             //.WhereIf(exceptId != null, s => s.CustomerCreditId != exceptId)
                             //.AsNoTracking()
                         join vci in customerCreditItemQuery
                         on item.Id equals vci.ItemIssueSaleItemId
                         into returnItems

                         //join ivi in _itemReceiptCustomerCreditItemRepository.GetAll()
                         //       .Where(s => s.ItemIssueSaleItemId != null)
                         //        .WhereIf(exceptId != null, s => s.ItemReceiptCustomerCreditId != exceptId)
                         //       .AsNoTracking()
                         join ivi in itemReceiptCustomerCreditItemQuery
                         on item.Id equals ivi.ItemIssueSaleItemId
                         into returnItems2

                         let returnQty = returnItems == null ? 0 : returnItems.Sum(s => s.Qty)
                         let returnQty2 = returnItems2 == null ? 0 : returnItems2.Sum(s => s.Qty)
                         let itemCount = items.Count(s => s.Qty > returnQty + returnQty2)
                         let remainingQty = item.Qty - returnQty - returnQty2
                         where itemCount > 0 && (returnQty + returnQty2 < item.Qty)

                         select new ItemIssueForCustomerCreditOutput
                         {
                             CreationTimeIndex = j.CreationTimeIndex,
                             Id = item.Id,
                             Date = j.Date,
                             Qty = item.Qty,
                             IssueNo = j.JournalNo,
                             LotName = item != null && item.Lot != null ? item.Lot.LotName : null,
                             LotId = item != null && item.Lot != null ? item.Lot.Id : (long?)null,
                             ItemName = item != null && item.Item != null ? item.Item.ItemName : null,
                             ItemCode = item != null && item.Item != null ? item.Item.ItemCode : null,
                             ItemId = item != null && item.Item != null ? item.Item.Id : (Guid?)null,
                             ItemIssueId = item.ItemIssueId,
                         };

            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).ThenBy(t => t.CreationTimeIndex).PageBy(input).ToListAsync();
            return new PagedResultDto<ItemIssueForCustomerCreditOutput>(resultCount, entities);

        }

        #endregion



        #region POS  get new list for new UI customer credit and item rereceipt customer credit

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssues_SaleReturn)]
        public async Task<PagedResultDto<ItemIssueSummaryOutput>> GetNewItemInvoiceForCutomerCredit(GetForCustomerCreditItemIssueInput input, Guid? exceptId)
        {

            var roundDigits = await _accountCycleRepository.GetAll().Select(t => t.RoundingDigit).FirstOrDefaultAsync();
            var userGroups = await GetUserGroupByLocation();
            var customerTypeMemberIds = await GetCustomerTypeMembers().Select(s => s.CustomerTypeId).ToListAsync();

            var customerCreditItemQuery = from vci in _cusotmerCreditItemRepository.GetAll().
                                              Where(s => s.ItemIssueSaleItemId != null)
                                              .WhereIf(exceptId != null, s => s.CustomerCreditId != exceptId)
                                              .AsNoTracking()
                                          join cc in _journalRepository.GetAll()
                                                 .Where(s => s.Status == TransactionStatus.Publish)
                                                 .Where(s => s.JournalType == JournalType.CustomerCredit)
                                                 .AsNoTracking()
                                          on vci.CustomerCreditId equals cc.CustomerCreditId
                                          select vci;

            var itemReceiptCustomerCreditItemQuery = from irci in _itemReceiptCustomerCreditItemRepository.GetAll()
                                                            .Where(s => s.ItemIssueSaleItemId != null)
                                                            .WhereIf(exceptId != null, s => s.ItemReceiptCustomerCreditId != exceptId)
                                                            .AsNoTracking()
                                                     join cc in _journalRepository.GetAll()
                                                            .Where(s => s.Status == TransactionStatus.Publish)
                                                            .Where(s => s.JournalType == JournalType.ItemReceiptCustomerCredit)
                                                            .AsNoTracking()
                                                     on irci.ItemReceiptCustomerCreditId equals cc.ItemReceiptCustomerCreditId
                                                     select irci;


            var @query = (from ir in _invoiceRepository.GetAll()
                           .Include(u => u.Customer)
                           .Include(u => u.ItemIssue)
                           .Include(u => u.TransactionTypeSale)
                           .Where(t => t.TransactionTypeSale.IsPOS && t.ItemIssue.TransactionType == InventoryTransactionType.ItemIssueSale)
                           .WhereIf(customerTypeMemberIds.Any(), s => customerTypeMemberIds.Contains(s.Customer.CustomerTypeId.Value))
                           .AsNoTracking()
                          join j in _journalRepository.GetAll()
                          .Include(u => u.Currency)
                          .WhereIf(input.Locations != null && input.Locations.Count > 0, t => input.Locations.Contains(t.LocationId))
                          .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                          .WhereIf(input.FromDate != null && input.ToDate != null,
                                       (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                          .Where(u => u.Status == TransactionStatus.Publish)
                          .AsNoTracking()
                          on ir.Id equals j.InvoiceId

                          join iri in _itemIssueItemRepository.GetAll().AsNoTracking()
                          .Include(u => u.Lot)
                          .AsNoTracking()
                          on ir.ItemIssueId equals iri.ItemIssueId
                          into items

                          from item in items

                          join vci in customerCreditItemQuery
                          on item.Id equals vci.ItemIssueSaleItemId
                          into returnItems

                          join ivi in itemReceiptCustomerCreditItemQuery
                          on item.Id equals ivi.ItemIssueSaleItemId
                          into returnItems2

                          let returnQty = returnItems == null ? 0 : returnItems.Sum(s => s.Qty)
                          let returnQty2 = returnItems2 == null ? 0 : returnItems2.Sum(s => s.Qty)
                          let itemCount = items.Count(s => s.Qty > returnQty + returnQty2)
                          let remainingQty = item.Qty - returnQty - returnQty2
                          where itemCount > 0 && (returnQty + returnQty2 < item.Qty)

                          group item by new
                          {
                              ir = ir,
                              j = j,
                              customer = ir.Customer,
                              currency = j.Currency,
                              multicurrrency = j.MultiCurrency,

                          }
                        into g
                          select new ItemIssueSummaryOutput
                          {
                              Id = g.Key.ir.ItemIssueId.Value,
                              Date = g.Key.j.Date,
                              InvoiceNo = g.Key.j.JournalNo,
                              ItemIssueId = g.Key.ir.ItemIssueId,
                              InvoiceId = g.Key.ir.Id,
                              CountItems = g.Count(),
                              Total = g.Key.ir.MultiCurrencyTotal,
                              Currency = ObjectMapper.Map<CurrencyDetailOutput>(g.Key.currency),
                              CustomerId = g.Key.ir.CustomerId,
                              CurrencyId = g.Key.j.CurrencyId,
                              Customer = ObjectMapper.Map<CustomerSummaryOutput>(g.Key.customer),
                              Reference = g.Key.j.Reference,
                              MultiCurrency = ObjectMapper.Map<CurrencyDetailOutput>(g.Key.multicurrrency),
                              MultiCurrencyId = g.Key.j.MultiCurrencyId.Value,
                          }).WhereIf(!input.Filter.IsNullOrEmpty(), u =>
                             (
                                 input.FilterType == FilterType.Contain &&
                                 (
                                     u.InvoiceNo.ToLower().Contains(input.Filter.ToLower()) ||
                                     u.Reference.ToLower().Contains(input.Filter.ToLower()) ||
                                     u.Customer != null && u.Customer.CustomerName != null && u.Customer.CustomerName.ToLower().Contains(input.Filter.ToLower()) ||
                                     u.Customer != null && u.Customer.PhoneNumber != null && u.Customer.PhoneNumber.ToLower().Contains(input.Filter.ToLower())
                                 )
                             ) ||
                             (
                                 input.FilterType == FilterType.StartWith &&
                                 (
                                     u.InvoiceNo.ToLower().StartsWith(input.Filter.ToLower()) ||
                                     u.Reference.ToLower().StartsWith(input.Filter.ToLower()) ||
                                     u.Customer != null && u.Customer.CustomerName != null && u.Customer.CustomerName.ToLower().StartsWith(input.Filter.ToLower()) ||
                                     u.Customer != null && u.Customer.PhoneNumber != null && u.Customer.PhoneNumber.ToLower().StartsWith(input.Filter.ToLower())
                                 )
                             ) ||
                             (
                                 input.FilterType == FilterType.Exact &&
                                 (
                                     u.InvoiceNo.ToLower().Equals(input.Filter.ToLower()) ||
                                     u.Reference.ToLower().Equals(input.Filter.ToLower()) ||
                                     u.Customer != null && u.Customer.CustomerName != null && u.Customer.CustomerName.ToLower().Equals(input.Filter.ToLower()) ||
                                     u.Customer != null && u.Customer.PhoneNumber != null && u.Customer.PhoneNumber.ToLower().Equals(input.Filter.ToLower())
                                 )
                             )
                        );

            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            return new PagedResultDto<ItemIssueSummaryOutput>(resultCount, entities);

        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_GetItemIssues_SaleReturn)]
        public async Task<ItemIssueSummaryOutputForItemIssueItem> GetItemInvoiceItemForCustomerCredit(EntityDto<Guid> input, Guid? exceptId)
        {
            var @journal = await _journalRepository
                                           .GetAll()
                                           .Include(u => u.ItemIssue)
                                           .Include(u => u.Currency)
                                           .Include(u => u.ItemIssue.Customer)
                                           .Include(u => u.ItemIssue.Customer.Account)
                                           .Include(u => u.Class)
                                           .Include(u => u.Location)
                                           .Include(u => u.ItemIssue.ShippingAddress)
                                           .Include(u => u.ItemIssue.BillingAddress)
                                           .Where(u => u.JournalType == JournalType.ItemIssueSale && u.ItemIssueId == input.Id)
                                           .AsNoTracking()
                                           .FirstOrDefaultAsync();

            if (@journal == null || @journal.ItemIssue == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var roundDigits = await _accountCycleRepository.GetAll().Select(t => t.RoundingDigit).FirstOrDefaultAsync();

            var customerCreditItemQuery = from vci in _cusotmerCreditItemRepository.GetAll().
                                              Where(s => s.ItemIssueSaleItemId != null)
                                              .WhereIf(exceptId != null, s => s.CustomerCreditId != exceptId)
                                              .AsNoTracking()
                                          join cc in _journalRepository.GetAll()
                                                 .Where(s => s.Status == TransactionStatus.Publish)
                                                 .Where(s => s.JournalType == JournalType.CustomerCredit)
                                                 .AsNoTracking()
                                          on vci.CustomerCreditId equals cc.CustomerCreditId

                                          join b in _customerCreditBatchNoRepository.GetAll().AsNoTracking()
                                          on vci.Id equals b.CustomerCreditItemId
                                          into bs
                                          from b in bs.DefaultIfEmpty()

                                          select new
                                          {
                                              BatchNoId = b == null ? (Guid?)null : b.BatchNoId,
                                              BatchNumber = b == null ? "" : b.BatchNo.Code,
                                              Qty = b == null ? vci.Qty : b.Qty,
                                              ItemIssueSaleItemId = vci.ItemIssueSaleItemId
                                          };

            var itemReceiptCustomerCreditItemQuery = from irci in _itemReceiptCustomerCreditItemRepository.GetAll()
                                                            .Where(s => s.ItemIssueSaleItemId != null)
                                                            .WhereIf(exceptId != null, s => s.ItemReceiptCustomerCreditId != exceptId)
                                                            .AsNoTracking()
                                                     join cc in _journalRepository.GetAll()
                                                            .Where(s => s.Status == TransactionStatus.Publish)
                                                            .Where(s => s.JournalType == JournalType.ItemReceiptCustomerCredit)
                                                            .AsNoTracking()
                                                     on irci.ItemReceiptCustomerCreditId equals cc.ItemReceiptCustomerCreditId

                                                     join b in _itemReceiptCustomerCreditItemBatchNoRepository.GetAll().AsNoTracking()
                                                     on irci.Id equals b.ItemReceiptCustomerCreditItemId
                                                     into bs
                                                     from b in bs.DefaultIfEmpty()

                                                     select new
                                                     {
                                                         BatchNoId = b == null ? (Guid?)null : b.BatchNoId,
                                                         BatchNumber = b == null ? "" : b.BatchNo.Code,
                                                         Qty = b == null ? irci.Qty : b.Qty,
                                                         ItemIssueSaleItemId = irci.ItemIssueSaleItemId
                                                     };

            var iQuery = from iri in _itemIssueItemRepository.GetAll()
                            .Include(u => u.Item.SaleAccount)
                            .Include(u => u.Lot)
                            .Where(u => u.ItemIssueId == input.Id)
                            .AsNoTracking()
                         join ji in _journalItemRepository.GetAll()
                             .Include(u => u.Account)
                             .Where(u => u.JournalId == journal.Id && u.Key == PostingKey.Inventory)
                             .AsNoTracking()
                         on iri.Id equals ji.Identifier
                         join invi in _invoiceItemRepository.GetAll().AsNoTracking()
                         on iri.Id equals invi.ItemIssueItemId
                         into invItems

                         join b in _itemIssueItemBatchNoRepository.GetAll().AsNoTracking()
                         on iri.Id equals b.ItemIssueItemId
                         into bs
                         from b in bs.DefaultIfEmpty()

                         let qty = b == null ? iri.Qty : b.Qty
                         let sellingPrice = invItems == null || invItems.Count() == 0 ? 0 : invItems.FirstOrDefault().UnitCost
                         let sellingPriceMultiCurrency = invItems == null || invItems.Count() == 0 ? 0 : invItems.FirstOrDefault().MultiCurrencyUnitCost

                         select new
                         {
                             iri.Id,
                             Qty = qty,
                             BatchNoId = b == null ? (Guid?)null : b.BatchNoId,
                             BatchNumber = b == null ? "" : b.BatchNo.Code,
                             ExpirationDate = b == null ? (DateTime?)null : b.BatchNo.ExpirationDate,
                             iri.Item,
                             iri.Lot,
                             ji.Account,
                             iri.DiscountRate,
                             iri.Description,
                             iri.ItemIssueId,
                             SellingPrice = sellingPrice,
                             SellingPriceMultiCurrency = sellingPriceMultiCurrency,
                             iri.CreationTime
                         };


            var query = from iri in iQuery

                        join returnItem in customerCreditItemQuery
                        on $"{iri.Id}-{iri.BatchNoId}" equals $"{returnItem.ItemIssueSaleItemId}-{returnItem.BatchNoId}"
                        into returnItems

                        join returnItem2 in itemReceiptCustomerCreditItemQuery
                        on $"{iri.Id}-{iri.BatchNoId}" equals $"{returnItem2.ItemIssueSaleItemId}-{returnItem2.BatchNoId}"
                        into returnItems2

                        let returnQty = returnItems == null ? 0 : returnItems.Sum(s => s.Qty)
                        let returnQty2 = returnItems2 == null ? 0 : returnItems2.Sum(s => s.Qty)
                        let remainingQty = iri.Qty - returnQty - returnQty2

                        where returnQty + returnQty2 < iri.Qty

                        select new
                        {
                            Id = iri.Id,
                            Item = ObjectMapper.Map<ItemSummaryDetailOutput>(iri.Item),
                            ItemId = iri.Item.Id,
                            InventoryAccountId = iri.Account.Id,
                            InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(iri.Account),
                            PurchaseAccountId = iri.Item.PurchaseAccountId.Value,
                            Description = iri.Description,
                            DiscountRate = iri.DiscountRate,
                            Qty = remainingQty,
                            SellingPrice = iri.SellingPrice,
                            SellingPriceMultiCurrency = iri.SellingPriceMultiCurrency,
                            ToLotId = iri.Lot.Id,
                            ToLotDetail = ObjectMapper.Map<LotSummaryOutput>(iri.Lot),
                            ItemIssueId = iri.ItemIssueId,
                            UseBatchNo = iri.Item.UseBatchNo,
                            TrackSerial = iri.Item.TrackSerial,
                            TrackExpiration = iri.Item.TrackExpiration,
                            ExpirationDate = iri.ExpirationDate,
                            iri.BatchNoId,
                            iri.BatchNumber,
                            iri.CreationTime
                        };

            var list = await query.ToListAsync();

            var itemIssueItems = list.OrderBy(s => s.CreationTime)
                                .GroupBy(l => new
                                {
                                    Id = l.Id,
                                    ItemId = l.ItemId,
                                    InventoryAccountId = l.InventoryAccountId,
                                    PurchaseAccountId = l.PurchaseAccountId,
                                    Description = l.Description,
                                    DiscountRate = l.DiscountRate,
                                    SellingPrice = l.SellingPrice,
                                    SellingPriceMultiCurrency = l.SellingPriceMultiCurrency,
                                    ToLotId = l.ToLotId,
                                    ItemIssueId = l.ItemIssueId,
                                    UseBatchNo = l.UseBatchNo,
                                    TrackSerial = l.TrackSerial,
                                    TrackExpiration = l.TrackExpiration
                                })
                                .Select(g => new ItemIssueItemDetailOutput()
                                {
                                    Id = g.Key.Id,
                                    Item = g.FirstOrDefault().Item,
                                    ItemId = g.Key.ItemId,
                                    InventoryAccountId = g.Key.InventoryAccountId,
                                    InventoryAccount = g.FirstOrDefault().InventoryAccount,
                                    PurchaseAccountId = g.Key.PurchaseAccountId,
                                    Description = g.Key.Description,
                                    DiscountRate = g.Key.DiscountRate,
                                    ToLotId = g.Key.ToLotId,
                                    ToLotDetail = g.FirstOrDefault().ToLotDetail,
                                    ItemIssueId = g.Key.ItemIssueId,
                                    UseBatchNo = g.Key.UseBatchNo,
                                    UnitCost = g.Key.SellingPrice,
                                    MultiCurrencyUnitCost = g.Key.SellingPriceMultiCurrency,
                                    Qty = g.Sum(t => t.Qty),
                                    Total = g.Sum(t => t.Qty * t.SellingPrice),
                                    SellingPrice = g.Key.SellingPrice,
                                    SellingPriceMultiCurrency = g.Key.SellingPriceMultiCurrency,
                                    MultiCurrencyTotal = g.Sum(t => t.Qty * t.SellingPriceMultiCurrency),
                                    TrackSerial = g.Key.TrackSerial,
                                    TrackExpiration = g.Key.TrackExpiration,
                                    ItemBatchNos = g.Key.UseBatchNo || g.Key.TrackSerial || g.Key.TrackExpiration ?
                                    g.Select(t =>
                                    new BatchNoItemOutput
                                    {
                                        BatchNoId = t.BatchNoId.Value,
                                        BatchNumber = t.BatchNumber,
                                        ExpirationDate = t.ExpirationDate,
                                        Qty = t.Qty
                                    })
                                     .ToList() :
                                     new List<BatchNoItemOutput>()
                                })
                               .ToList();

            var result = ObjectMapper.Map<ItemIssueSummaryOutputForItemIssueItem>(journal.ItemIssue);
            result.Total = Math.Round(itemIssueItems.Sum(s => s.Total), roundDigits);
            result.IssueNo = journal.JournalNo;
            result.Date = journal.Date;
            result.Reference = journal.Reference;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.LocationId = journal.LocationId.Value;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.ItemIssueItems = itemIssueItems;
            result.Customer = ObjectMapper.Map<CustomerSummaryDetailOutput>(journal.ItemIssue.Customer);
            result.CustomerId = journal.ItemIssue.CustomerId.Value;
            result.Memo = journal.Memo;
            return result;
        }


        #endregion


    }
}
