using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using CorarlERP.ChartOfAccounts;
using CorarlERP.ItemReceipts.Dto;
using CorarlERP.Journals;
using CorarlERP.PurchaseOrders;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using CorarlERP.Journals.Dto;
using Abp.UI;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Items.Dto;
using CorarlERP.Taxes.Dto;
using CorarlERP.PurchaseOrders.Dto;
using Abp.Collections.Extensions;
using Abp.Extensions;
using Abp.Linq.Extensions;
using CorarlERP.Vendors.Dto;
using CorarlERP.Vendors;
using Abp.Authorization;
using CorarlERP.Authorization;
using CorarlERP.Currencies.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Bills;
using CorarlERP.Items;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.ItemReceiptCustomerCredits;
using CorarlERP.Customers;
using CorarlERP.Customers.Dto;
using CorarlERP.TransferOrders;
using CorarlERP.Productions;
using CorarlERP.AutoSequences;
using CorarlERP.Authorization.Users;
using CorarlERP.Dto;
using CorarlERP.Lots.Dto;
using CorarlERP.Locations.Dto;
using CorarlERP.UserGroups;
using CorarlERP.ItemIssueVendorCredits;
using CorarlERP.AccountCycles;
using CorarlERP.Reports;
using CorarlERP.Locks;
using CorarlERP.Common.Dto;
using CorarlERP.Lots;
using Abp.Dependency;
using CorarlERP.InventoryCalculationJobSchedules;
using Hangfire.States;
using CorarlERP.InventoryCalculationItems;
using CorarlERP.BatchNos;
using CorarlERP.ItemIssues;
using CorarlERP.VendorCredit;
using CorarlERP.JournalTransactionTypes;
using CorarlERP.Features;

namespace CorarlERP.ItemReceipts
{
    public class ItemReceiptAppService : ReportBaseClass, IItemReceiptAppService
    {
        private readonly IItemManager _itemManager;
        private readonly IRepository<Item, Guid> _itemRepository;
        private readonly IRepository<Lot, long> _lotRepository;

        private readonly IBillItemManager _billItemManager;
        private readonly IRepository<BillItem, Guid> _billItemRepository;

        private readonly IBillManager _billManager;
        private readonly IRepository<Bill, Guid> _billRepository;

        private readonly IVendorManager _vendorItemManager;
        private readonly IRepository<Vendor, Guid> _vendorRepository;

        private readonly IRepository<Customer, Guid> _customerRepository;

        private readonly IRepository<ItemIssueVendorCreditItem, Guid> _itemIssueVendorCreditItemRepository;
        private readonly IRepository<ItemIssueVendorCredit, Guid> _itemIssueVendorCreditRepository;
        private readonly IRepository<ItemReceiptCustomerCredit, Guid> _itemReceiptCustomerCreditRepository;
        private readonly IRepository<ItemReceiptItemCustomerCredit, Guid> _itemReceiptCustomerCreditItemRepository;

        private readonly IRepository<AccountCycle, long> _accountCycleRepository;
        private readonly IPurchaseOrderItemManager _purhcaserOrderItemManager;
        private readonly IRepository<PurchaseOrderItem, Guid> _purhcaseOrderItemRepository;
        private readonly IPurchaseOrderManager _purhcaserOrderManager;
        private readonly IRepository<PurchaseOrder, Guid> _purhcaseOrderRepository;
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


        private readonly IProductionManager _productionOrderManager;
        private readonly IRepository<Production, Guid> _productionOrderRepository;

        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;

        private readonly IRepository<User, long> _userRepository;

        private readonly IAutoSequenceManager _autoSequenceManager;

        private readonly IRepository<VendorCredit.VendorCredit, Guid> _vendorCreditRepository;
        private readonly IRepository<VendorCredit.VendorCreditDetail, Guid> _vendorCreditItemRepository;
        private readonly IInventoryCalculationItemManager _inventoryCalculationItemManager;
        private readonly ICorarlRepository<BatchNo, Guid> _batchNoRepository;
        private readonly IRepository<ItemIssueItem, Guid> _itemIssueItemRepository;
        private readonly ICorarlRepository<ItemReceiptItemBatchNo, Guid> _itemReceiptItemBatchNoRepository;
        private readonly ICorarlRepository<VendorCreditItemBatchNo, Guid> _vendorCreditItemBatchNoRepository;
        private readonly ICorarlRepository<ItemIssueVendorCreditItemBatchNo, Guid> _itemIssueVendorCreditItemBatchNoRepository;

        private readonly IRepository<Lock, long> _lockRepository;
        private readonly IJournalTransactionTypeManager _journalTransactionTypeManager;
        public ItemReceiptAppService(
            IJournalManager journalManager,
            IJournalItemManager journalItemManager,
            IChartOfAccountManager chartOfAccountManager,
            IInventoryCalculationItemManager inventoryCalculationItemManager,
            ItemReceiptManager itemReceiptManager,
            ItemReceiptItemManager itemReceiptItemManager,
            ItemManager itemManager,
            PurchaseOrderItemManager purchaseOrderItemManager,
            PurchaseOrderManager purchaseOrderManager,
            VendorManager vendorManager,
            BillItemManager billItemManager,
            BillManager billManager,
            IRepository<BillItem, Guid> billItemRepository,
            IRepository<Item, Guid> itemRepository,
            IRepository<Bill, Guid> billRepository,
            IRepository<Vendor, Guid> vendorRepository,
            IRepository<Journal, Guid> journalRepository,
            IRepository<JournalItem, Guid> journalItemRepository,
            IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
            IRepository<ItemReceipt, Guid> itemReceiptRepository,
            IRepository<ItemReceiptItem, Guid> itemReceiptItemRepository,
            IRepository<PurchaseOrderItem, Guid> purchaseOrderItemRepository,
            IRepository<PurchaseOrder, Guid> purchaseOrderRepository,
            IRepository<ItemReceiptCustomerCredit, Guid> itemReceiptCustomerCreditRepository,
            IRepository<ItemReceiptItemCustomerCredit, Guid> itemReceiptCustomerCreditItemRepository,
            IRepository<Customer, Guid> customerRepository,
            ITransferOrderManager transferOrderManager,
            IRepository<TransferOrder, Guid> transferOrderRepository,
            IProductionManager productionOrderManager,
            IRepository<Production, Guid> productionOrderRepository,
            IRepository<AutoSequence, Guid> autoSequenceRepository,
            IAutoSequenceManager autoSequenceManager,
            IRepository<User, long> userRepository,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            IRepository<Locations.Location, long> locationRepository,
            IRepository<VendorCredit.VendorCredit, Guid> vendorCreditRepository,
            IRepository<VendorCredit.VendorCreditDetail, Guid> vendorCreditItemRepository,
            IRepository<ItemIssueVendorCreditItem, Guid> itemIssueVendorCreditItemRepository,
            IRepository<AccountCycle, long> accountCycleRepository,
            IRepository<ItemIssueVendorCredit, Guid> itemIssueVendorCreditRepository,
            IRepository<Lock, long> lockRepository,
            ICorarlRepository<BatchNo, Guid> batchNoRepository,
            ICorarlRepository<ItemReceiptItemBatchNo, Guid> itemReceiptItemBatchNoRepository,
            ICorarlRepository<VendorCreditItemBatchNo, Guid> vendorCreditItemBatchNoRepository,
            ICorarlRepository<ItemIssueVendorCreditItemBatchNo, Guid> itemIssueVendorCreditItemBatchNoRepository,
            IRepository<ItemIssueItem, Guid> itemIssueItemRepository,
            IJournalTransactionTypeManager journalTransactionTypeManager,
            AppFolders appFolders
            ) : base(accountCycleRepository, appFolders, userGroupMemberRepository, locationRepository)
        {
            _itemIssueVendorCreditRepository = itemIssueVendorCreditRepository;
            _accountCycleRepository = accountCycleRepository;
            _itemIssueVendorCreditItemRepository = itemIssueVendorCreditItemRepository;
            _userRepository = userRepository;
            _vendorCreditRepository = vendorCreditRepository;
            _journalManager = journalManager;
            _journalManager.SetJournalType(JournalType.ItemReceiptPurchase);
            _journalRepository = journalRepository;
            _journalItemManager = journalItemManager;
            _journalItemRepository = journalItemRepository;
            _chartOfAccountManager = chartOfAccountManager;
            _chartOfAccountRepository = chartOfAccountRepository;
            _itemReceiptItemManager = itemReceiptItemManager;
            _itemReceiptItemRepository = itemReceiptItemRepository;
            _itemReceiptManager = itemReceiptManager;
            _itemReceiptRepository = itemReceiptRepository;
            _purhcaseOrderItemRepository = purchaseOrderItemRepository;
            _purhcaserOrderItemManager = purchaseOrderItemManager;
            _purhcaseOrderRepository = purchaseOrderRepository;
            _purhcaserOrderManager = purchaseOrderManager;
            _vendorRepository = vendorRepository;
            _vendorItemManager = vendorManager;
            _billItemRepository = billItemRepository;
            _billItemManager = billItemManager;
            _billRepository = billRepository;
            _billManager = billManager;
            _itemRepository = itemRepository;
            _itemManager = itemManager;

            _itemReceiptCustomerCreditItemRepository = itemReceiptCustomerCreditItemRepository;
            _itemReceiptCustomerCreditRepository = itemReceiptCustomerCreditRepository;
            _customerRepository = customerRepository;

            _transferOrderManager = transferOrderManager;
            _transferOrderRepository = transferOrderRepository;
            _productionOrderManager = productionOrderManager;
            _productionOrderRepository = productionOrderRepository;
            _autoSequenceManager = autoSequenceManager;
            _autoSequenceRepository = autoSequenceRepository;
            _vendorCreditItemRepository = vendorCreditItemRepository;
            _lockRepository = lockRepository;
            _lotRepository = IocManager.Instance.Resolve<IRepository<Lot, long>>();
            _inventoryCalculationItemManager = inventoryCalculationItemManager;
            _batchNoRepository = batchNoRepository;
            _itemIssueItemRepository = itemIssueItemRepository;
            _itemReceiptItemBatchNoRepository = itemReceiptItemBatchNoRepository;
            _vendorCreditItemBatchNoRepository = vendorCreditItemBatchNoRepository;
            _itemIssueVendorCreditItemBatchNoRepository = itemIssueVendorCreditItemBatchNoRepository;
            _journalTransactionTypeManager = journalTransactionTypeManager;
        }

        private async Task ValidateAddBatchNo(CreateItemReceiptInput input)
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
            if (serialQty != null) throw new UserFriendlyException(L("MustBeEqualTo", $"{L("SerialNo")} {L("Qty")}" , 1) + $", Item: {useBatchItemDic[serialQty.ItemId].ItemCode}-{useBatchItemDic[serialQty.ItemId].ItemName}");


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
            if (validateQty != null) throw new UserFriendlyException(L("IsNotValid", L("Qty") + " " + $"{L("BatchNo")}/{L("SerialNo")}" + $", {useBatchItemDic[validateQty.ItemId].ItemCode}-{useBatchItemDic[validateQty.ItemId].ItemName}"));

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
                        if (expriationItemHash.Contains(item.ItemId)) {
                            if(batchNoDic[i.BatchNumber].ExpirationDate.Value.Date != i.ExpirationDate.Value.Date) throw new UserFriendlyException(L("IsNotValid", L("ExpirationDate")) + $" : {i.ExpirationDate.Value.ToString("dd-MM-yyyy")}");
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


        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_Create, AppPermissions.Pages_Tenant_Vendors_ItemReceipts_CreatePurchases,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemReceiptPurchase)]
        public async Task<NullableIdDto<Guid>> Create(CreateItemReceiptInput input)
        {
            if(input.IsConfirm == false) {
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
                                        input.Memo, input.Total, input.Total, input.CurrencyId,
                                        input.ClassId, input.Reference, input.LocationId);
            entity.UpdateStatus(input.Status);

            #region journal transaction type 
            var transactionTypeId = await _journalTransactionTypeManager.GetJournalTransactionTypeId(InventoryTransactionType.ItemReceiptPurchase);
            entity.SetJournalTransactionTypeId(transactionTypeId);
            #endregion

            //insert clearance journal item into credit
            var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, input.ClearanceAccountId,
                                                            input.Memo, 0, input.Total, PostingKey.Clearance, null);
            //insert to item Receipt
            var @itemReceipt = ItemReceipt.Create(tenantId, userId, input.ReceiveFrom, input.VendorId,
                                                    input.SameAsShippingAddress, input.ShippingAddress,
                                                    input.BillingAddress, input.Total, null);

            itemReceipt.UpdateTransactionType(InventoryTransactionType.ItemReceiptPurchase);
            @entity.UpdateItemReceipt(@itemReceipt);
            //Update ItemReceiptId on table bill
            if (input.ReceiveFrom == ItemReceipt.ReceiveFromStatus.Bill)
            {
                if (input.BillId == null)
                {
                    throw new UserFriendlyException(L("PleaseAddBill"));
                }

                var bill = await _billRepository.GetAll().Where(u => u.Id == input.BillId).FirstOrDefaultAsync();
                bill.UpdateItemReceiptid(itemReceipt.Id);
                // update received status in bill to full when receive from bill 
                if (input.Status == TransactionStatus.Publish)
                {
                    bill.UpdateReceivedStatus(DeliveryStatus.ReceiveAll);
                }
                CheckErrors(await _billManager.UpdateAsync(bill));
                
            }

            else if (input.ReceiveFrom == ItemReceipt.ReceiveFromStatus.None && input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }

           
           await UpdatePOReceiptStautus(null, input.ReceiveFrom, input.BillId, input.Items);
            

            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));
            CheckErrors(await _journalItemManager.CreateAsync(clearanceJournalItem));
            CheckErrors(await _itemReceiptManager.CreateAsync(@itemReceipt));            
            
            var @billitems = await _billItemRepository.GetAll()
                                   .Include(s => s.OrderItem)
                                   .Where(t => t.BillId == input.BillId).ToListAsync();

            var orderIds = input.Items.Where(s => s.PurchaseOrderId.HasValue)
                               .GroupBy(s => s.PurchaseOrderId.Value)
                               .Select(s => s.Key).ToList();

            var ids = billitems.Where(s => s.OrderItem != null)
                     .GroupBy(s => s.OrderItem.PurchaseOrderId)
                     .Select(s => s.Key)
                     .ToList();
            orderIds.AddRange(ids);


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
                itemReceiptItem.UpdateLot(i.LotId);
                CheckErrors(await _itemReceiptItemManager.CreateAsync(@itemReceiptItem));

                //Update billItemid when click on BIll in table billItem
                if (input.ReceiveFrom == ItemReceipt.ReceiveFromStatus.Bill && i.BillItemId != null)
                {
                    var billItem = @billitems.Where(u => u.Id == i.BillItemId.Value).FirstOrDefault();
                                    billItem.UpdateReceiptItemId(itemReceiptItem.Id);
                                    billItem.UpdateLot(itemReceiptItem.LotId);
                                    CheckErrors(await _billItemManager.UpdateAsync(billItem));

                    if (i.Qty > billItem.Qty)
                    {
                        throw new UserFriendlyException(L("PleaseCheckYourQty"));
                    }
                }
                else if(input.ReceiveFrom == ItemReceipt.ReceiveFromStatus.PO)
                {                    
                    @itemReceiptItem.UpdateOrderItemId(i.OrderItemId);
                }               
                var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, i.InventoryAccountId, i.Description, i.Total, 0, PostingKey.Inventory, itemReceiptItem.Id);
                CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));

                if (i.ItemBatchNos != null && i.ItemBatchNos.Any())
                {
                    var addItemBatchNos = i.ItemBatchNos.Select(s => ItemReceiptItemBatchNo.Create(tenantId, userId, itemReceiptItem.Id, s.BatchNoId, s.Qty)).ToList();
                    foreach(var a in addItemBatchNos)
                    {
                        await _itemReceiptItemBatchNoRepository.InsertAsync(a);
                    }
                }
            }

        
            //need save change before performing bellow task
            await CurrentUnitOfWork.SaveChangesAsync();

            if (orderIds.Any())
            {
                foreach (var id in orderIds)
                {
                    await UpdatePurhaseOrderInventoryStatus(id);
                }
            }

            await SyncItemReceipt(itemReceipt.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                var scheduleItems = input.Items.Select(s => s.ItemId).Distinct().ToList();
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, input.Date, scheduleItems);
            }

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.ItemReceipt,TransactionLockType.InventoryTransaction, };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }

            return new NullableIdDto<Guid>() { Id = @itemReceipt.Id };
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetDetail,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ViewDetail)]
        public async Task<ItemReceiptDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            await TurnOffTenantFilterIfDebug();

            var @journal = await _journalRepository
                                .GetAll()
                                .Include(u => u.ItemReceipt)
                                .Include(u => u.Location)
                                .Include(u => u.Class)
                                .Include(u => u.Currency)
                                .Include(u => u.ItemReceipt.Vendor)
                                .Include(u => u.ItemReceipt.ShippingAddress)
                                .Include(u => u.ItemReceipt.BillingAddress)
                                .Include(u=>u.JournalTransactionType)
                                .AsNoTracking()
                                .Where(u => u.JournalType == JournalType.ItemReceiptPurchase && u.ItemReceiptId == input.Id)
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
                //.Include(u => u.Tax)
                // .Include(u => u.BillItem)
                .Include(u => u.Item)
                .Include(u => u.OrderItem)
                .Include(u => u.Lot)
                .Include(u => u.OrderItem.PurchaseOrder)
                //.Include(u => u.Tax)
                .Where(u => u.ItemReceiptId == input.Id)
                .OrderBy(s => s.CreationTime)
                .Join(_journalItemRepository.GetAll()
                .Include(u => u.Account)
                .Where(u=>u.Key == PostingKey.Inventory)
                .AsNoTracking(), u => u.Id, s => s.Identifier,
                (rItem, jItem) =>
                new ItemReceiptItemDetailOutput()
                {
                    MultiCurrencyId = rItem.OrderItem != null ?  rItem.OrderItem.PurchaseOrder.MultiCurrencyId: null,
                    CreationTime = rItem.CreationTime,
                    OrderNumber = rItem.OrderItem.PurchaseOrder.OrderNumber,
                    Id = rItem.Id,
                    Item = ObjectMapper.Map<ItemSummaryOutput>(rItem.Item),
                    ItemId = rItem.ItemId,
                    PurchaseOrderItem = ObjectMapper.Map<PurchaseOrderItemSummaryOut>(rItem.OrderItem),
                    InventoryAccountId = jItem.AccountId,
                    InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                    Description = rItem.Description,
                    DiscountRate = rItem.DiscountRate,
                    Qty = rItem.Qty,
                    // Tax = ObjectMapper.Map<TaxSummaryOutput>(rItem.Tax),
                    Total = rItem.Total,
                    UnitCost = rItem.UnitCost,
                    OrderItemId = rItem.OrderItemId,
                    ToLotDetail = ObjectMapper.Map<LotSummaryOutput>(rItem.Lot),
                    ToLotId = rItem.LotId,
                    //  TaxId = rItem.TaxId,
                    UseBatchNo = rItem.Item.UseBatchNo,
                    AutoBatchNo = rItem.Item.AutoBatchNo,
                    TrackExpiration = rItem.Item.TrackExpiration,
                    TrackSerial = rItem.Item.TrackSerial
                }).OrderBy(t => t.CreationTime)
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

            var invoice = (from iis in _itemReceiptRepository.GetAll().Where(t => t.Id == input.Id)
                           join inv in _billRepository.GetAll() on iis.Id equals inv.ItemReceiptId
                           join j in _journalRepository.GetAll() on inv.Id equals j.BillId
                           select new
                           {
                               Id = inv.Id,
                               Date = j.Date,
                               JournalNo = j.JournalNo
                           }).FirstOrDefault();
            var result = ObjectMapper.Map<ItemReceiptDetailOutput>(journal.ItemReceipt);

            if (invoice == null)
            {
                result.BillId = null;
            }
            else
            {
                result.BillId = invoice.Id;
                result.BillNo = invoice.JournalNo;
                result.BillDate = invoice.Date;
            }
            result.MultiCurrencyId = result.ReceiveFrom == ItemReceipt.ReceiveFromStatus.PO ? itemReceiptItems.Select(t => t.MultiCurrencyId).FirstOrDefault(): null;
            result.Date = journal.Date;
            result.ReceiveNo = journal.JournalNo;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.Reference = journal.Reference;
            result.ItemReceiptItems = itemReceiptItems;
            result.ClearanceAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(clearanceAccount);
            result.ClearanceAccountId = clearanceAccount.Id;
            result.Memo = journal.Memo;
            result.StatusCode = journal.Status;
            result.Total = journal.Credit;
            result.StatusName = journal.Status.ToString();
            result.LocationId = journal.LocationId.Value;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            result.TransactionTypeName = journal.JournalTransactionType?.Name;
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_UpdateStatusToPublish,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Publish)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input)
        {
            //var @entity = await _journalManager.GetAsync(input.Id, true);
            var @entity = await _journalRepository
                                .GetAll()
                                .Include(u => u.ItemReceipt)
                                .Include(u => u.Bill)
                                .Where(u => (u.JournalType == JournalType.ItemReceiptPurchase ||
                                  u.JournalType == JournalType.ItemReceiptAdjustment ||
                                  u.JournalType == JournalType.ItemReceiptOther ||
                                  u.JournalType == JournalType.ItemReceiptTransfer ||
                                  u.JournalType == JournalType.ItemReceiptProduction)
                                  && u.ItemReceipt.Id == input.Id)
                                .FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            // validate item reciept type sale by bill
            var validate = (from bill in _billRepository.GetAll().Include(t => t.ItemReceipt)
                            .Where(t => t.ItemReceiptId == input.Id
                                && t.ItemReceiptId != null
                                && t.CreationTime > t.ItemReceipt.CreationTime
                                && entity.JournalType == JournalType.ItemReceiptPurchase)
                            select bill).ToList().Count();
            if (validate > 0)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }

            //update receive status of transfer order to ship all 
            if (entity.JournalType == JournalType.ItemReceiptTransfer)
            {
                var @transfer = await _transferOrderRepository.GetAll().Where(u => u.Id == entity.ItemReceipt.TransferOrderId).FirstOrDefaultAsync();
                // update received status 
                if (@transfer != null)
                {
                    @transfer.UpdateShipedStatus(TransferStatus.ReceiveAll);
                    CheckErrors(await _transferOrderManager.UpdateAsync(@transfer));
                }
            }
            if (entity.JournalType == JournalType.ItemReceiptProduction)
            {
                var production = await _productionOrderRepository.GetAll().Where(u => u.Id == entity.ItemReceipt.ProductionOrderId).FirstOrDefaultAsync();
                // update received status 
                if (production != null)
                {
                    production.UpdateShipedStatus(TransferStatus.ReceiveAll);
                    CheckErrors(await _productionOrderManager.UpdateAsync(production));
                }
            }
            if (entity.JournalType == JournalType.ItemReceiptPurchase)
            {
                //Code update status....

                //query select qty form itemReceiptItem for update checkStatus on table POItem

                //query get item receipt item and update totalItemReceipt 
                var @itemReceiptItems = await _itemReceiptItemRepository.GetAll().Include(u => u.OrderItem)
                    .Where(u => u.ItemReceiptId == entity.ItemReceiptId).ToListAsync();
                var queryPOHeader = (from po in _purhcaseOrderRepository.GetAll()
                                     join pi in _purhcaseOrderItemRepository.GetAll().Include(u => u.Item) on po.Id equals pi.PurchaseOrderId
                                     where (@itemReceiptItems.Any(t => t.OrderItemId == pi.Id))
                                     group pi by po into u
                                     select new { poId = u.Key.Id }
                    );


                if (entity.ItemReceipt.ReceiveFrom == ItemReceipt.ReceiveFromStatus.PO && entity.ItemReceipt != null)
                {


                    foreach (var iri in @itemReceiptItems)
                    {

                    }
                }

                if (entity.ItemReceipt.ReceiveFrom == ItemReceipt.ReceiveFromStatus.Bill && entity.ItemReceipt != null)
                {
                    var bill = await _billRepository.GetAll().Where(u => u.ItemReceiptId == entity.ItemReceiptId).FirstOrDefaultAsync();
                    if (bill != null) //update revers status in receive status of Bill to pending 
                    {
                        bill.UpdateReceivedStatus(DeliveryStatus.ReceiveAll);
                        CheckErrors(await _billManager.UpdateAsync(bill));
                    }
                    // update OP status from bill
                }
                // end code
            }

            entity.UpdatePublish();
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt);
            CheckErrors(await _journalManager.UpdateAsync(entity, auto.DocumentType));

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_UpdateStatusToVoid,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Void)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input)
        {

            var @jounal = await _journalRepository
                 .GetAll()
                 .Include(u => u.ItemReceipt)
                 .Include(u => u.Bill)
                 .Include(u => u.ItemReceipt.ShippingAddress)
                 .Include(u => u.ItemReceipt.BillingAddress)
                              .Where(u => (u.JournalType == JournalType.ItemReceiptPurchase ||
                                  u.JournalType == JournalType.ItemReceiptAdjustment ||
                                  u.JournalType == JournalType.ItemReceiptOther ||
                                  u.JournalType == JournalType.ItemReceiptTransfer ||
                                  u.JournalType == JournalType.ItemReceiptProduction)
                                  && u.ItemReceipt.Id == input.Id)
                              .FirstOrDefaultAsync();


            // validate item reciept type sale by bill
            var validate = (from bill in _billRepository.GetAll().Include(t => t.ItemReceipt)
                            .Where(t => t.ItemReceiptId == input.Id
                            && t.ItemReceiptId != null
                            && t.CreationTime > t.ItemReceipt.CreationTime
                            && jounal.JournalType == JournalType.ItemReceiptPurchase)
                            select bill).ToList().Count();
            if (validate > 0)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }

            //update receive status of transfer order to ship all 
            if (@jounal.JournalType == JournalType.ItemReceiptTransfer)
            {
                var @transfer = await _transferOrderRepository.GetAll().Where(u => u.Id == @jounal.ItemReceipt.TransferOrderId).FirstOrDefaultAsync();
                // update received status 
                if (@transfer != null)
                {
                    @transfer.UpdateShipedStatus(TransferStatus.ShipAll);
                    CheckErrors(await _transferOrderManager.UpdateAsync(@transfer));
                }
            }

            if (jounal.JournalType == JournalType.ItemReceiptProduction)
            {
                var production = await _productionOrderRepository.GetAll().Where(u => u.Id == jounal.ItemReceipt.ProductionOrderId).FirstOrDefaultAsync();
                // update received status 
                if (production != null)
                {
                    production.UpdateShipedStatus(TransferStatus.ShipAll);
                    CheckErrors(await _productionOrderManager.UpdateAsync(production));
                }
            }

            if (@jounal.JournalType == JournalType.ItemReceiptPurchase)
            {
                //query get item receipt item and update totalbillQty 
                var bill = (_billRepository.GetAll().Where(u => u.ItemReceiptId == jounal.ItemReceiptId)).FirstOrDefault();

                if (jounal.ItemReceipt.ReceiveFrom == ItemReceipt.ReceiveFromStatus.Bill)
                {
                    bill.UpdateReceivedStatus(DeliveryStatus.ReceivePending);
                    CheckErrors(await _billManager.UpdateAsync(bill));
                }

                //query get item receipt item and update totalItemReceipt 
                var @itemReceiptItems = await _itemReceiptItemRepository.GetAll().Include(u => u.OrderItem)
                    .Where(u => u.ItemReceiptId == jounal.ItemReceiptId).ToListAsync();
                var queryPOHeader = (from po in _purhcaseOrderRepository.GetAll()
                                     join pi in _purhcaseOrderItemRepository.GetAll().Include(u => u.Item) on po.Id equals pi.PurchaseOrderId
                                     where (@itemReceiptItems.Any(t => t.OrderItemId == pi.Id))
                                     group pi by po into u
                                     select new { poId = u.Key.Id }
                    );

                // temp of po id header 
                var listOfPoHeader = new List<CreateOrUpdateItemReceiptItemInput>();

                foreach (var i in queryPOHeader)
                {
                    listOfPoHeader.Add(new CreateOrUpdateItemReceiptItemInput
                    {
                        PurchaseOrderId = i.poId
                    });
                }

                if (jounal.ItemReceipt.ReceiveFrom == ItemReceipt.ReceiveFromStatus.PO && jounal.ItemReceipt != null)
                {

                    foreach (var iri in @itemReceiptItems)
                    {
                        if (iri.OrderItemId != null && iri.OrderItem != null && jounal.Status == TransactionStatus.Publish)
                        {
                            //Update Purchase Order item when delete Item Receipt                           
                            var decraseQty = (iri.Qty * -1);
                            CheckErrors(await _purhcaserOrderItemManager.UpdateAsync(iri.OrderItem));
                        }

                    }
                }

                if (jounal.ItemReceipt.ReceiveFrom == ItemReceipt.ReceiveFromStatus.Bill && jounal.ItemReceipt != null)
                {

                    // update OP status from bill
                }
            }

            jounal.UpdateVoid();
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt);
            CheckErrors(await _journalManager.UpdateAsync(jounal, auto.DocumentType));


            return new NullableIdDto<Guid>() { Id = jounal.ItemReceiptId };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_UpdateStatusToDraft,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Draft)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input)
        {

            var @entity = await _journalRepository
                                .GetAll()
                                .Include(u => u.ItemReceipt)
                                .Where(u => (u.JournalType == JournalType.ItemReceiptPurchase ||
                                    u.JournalType == JournalType.ItemReceiptAdjustment ||
                                    u.JournalType == JournalType.ItemReceiptOther ||
                                    u.JournalType == JournalType.ItemReceiptTransfer ||
                                    u.JournalType == JournalType.ItemReceiptProduction)
                                    && u.ItemReceipt.Id == input.Id)
                                .FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            // validate item reciept type sale by bill
            var validate = (from bill in _billRepository.GetAll().Include(t => t.ItemReceipt)
                            .Where(t => t.ItemReceiptId == input.Id
                                    && t.ItemReceiptId != null
                                    && entity.JournalType == JournalType.ItemReceiptPurchase
                                    && t.CreationTime > t.ItemReceipt.CreationTime
                            )
                            select bill).ToList().Count();
            if (validate > 0)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }

            //update receive status of transfer order to shiped all 
            if (entity.JournalType == JournalType.ItemReceiptTransfer)
            {
                var @transfer = await _transferOrderRepository.GetAll().Where(u => u.Id == entity.ItemReceipt.TransferOrderId).FirstOrDefaultAsync();
                // update received status 
                if (@transfer != null)
                {
                    @transfer.UpdateShipedStatus(TransferStatus.ShipAll);
                    CheckErrors(await _transferOrderManager.UpdateAsync(@transfer));
                }
            }

            if (entity.JournalType == JournalType.ItemReceiptProduction)
            {
                var production = await _productionOrderRepository.GetAll().Where(u => u.Id == entity.ItemReceipt.ProductionOrderId).FirstOrDefaultAsync();
                // update received status 
                if (production != null)
                {
                    production.UpdateShipedStatus(TransferStatus.ShipAll);
                    CheckErrors(await _productionOrderManager.UpdateAsync(production));
                }
            }

            if (entity.JournalType == JournalType.ItemReceiptPurchase)
            {
                //query get item receipt item and update totalItemReceipt 
                var @itemReceiptItems = await _itemReceiptItemRepository.GetAll().Include(u => u.OrderItem)
                .Where(u => u.ItemReceiptId == entity.ItemReceiptId).ToListAsync();
                var queryPOHeader = (from po in _purhcaseOrderRepository.GetAll()
                                     join pi in _purhcaseOrderItemRepository.GetAll().Include(u => u.Item) on po.Id equals pi.PurchaseOrderId
                                     where (@itemReceiptItems.Any(t => t.OrderItemId == pi.Id))
                                     group pi by po into u
                                     select new { poId = u.Key.Id }
                    );

                // temp of po id header 
                var listOfPoHeader = new List<CreateOrUpdateItemReceiptItemInput>();

                foreach (var i in queryPOHeader)
                {
                    listOfPoHeader.Add(new CreateOrUpdateItemReceiptItemInput
                    {
                        PurchaseOrderId = i.poId
                    });
                }

                if (entity.ItemReceipt.ReceiveFrom == ItemReceipt.ReceiveFromStatus.PO && entity.ItemReceipt != null)
                {

                }


                if (entity.ItemReceipt.ReceiveFrom == ItemReceipt.ReceiveFromStatus.Bill && entity.ItemReceipt != null)
                {
                    var bill = await _billRepository.GetAll().Where(u => u.ItemReceiptId == entity.ItemReceiptId).FirstOrDefaultAsync();
                    if (bill != null) //update revers status in receive status of Bill to pending 
                    {
                        bill.UpdateReceivedStatus(DeliveryStatus.ReceivePending);
                        CheckErrors(await _billManager.UpdateAsync(bill));
                    }
                }
                // end code

            }


            entity.UpdateStatusToDraft();
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt);
            CheckErrors(await _journalManager.UpdateAsync(entity, auto.DocumentType));

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_Delete,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Delete,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour)]
        public async Task Delete(CarlEntityDto input)
        {
            var userId = AbpSession.GetUserId();
            //validate when use in vendorcredit and itemIssueVendorCredit
            var vendorCredit = await _vendorCreditRepository.GetAll().Where(t => t.ItemReceiptId == input.Id).CountAsync();
            var itemIssueVendorCredit = await _itemIssueVendorCreditRepository.GetAll().Where(t => t.ItemReceiptPurchaseId == input.Id).CountAsync();

            if (vendorCredit > 0 || itemIssueVendorCredit > 0)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChangeInVendorCredit"));
            }

            var journal = await _journalRepository
                                 .GetAll()
                                 .Include(u => u.ItemReceipt)
                                 .Include(u => u.ItemReceipt.TransferOrder)
                                 .Include(u => u.ItemReceipt.ProductionOrder)
                                 .Include(u => u.ItemReceipt.BillingAddress)
                                 .Include(u => u.ItemReceipt.ShippingAddress)
                                 .Where(u => (u.JournalType == JournalType.ItemReceiptPurchase ||
                                     u.JournalType == JournalType.ItemReceiptAdjustment ||
                                     u.JournalType == JournalType.ItemReceiptOther ||
                                     u.JournalType == JournalType.ItemReceiptTransfer ||
                                     u.JournalType == JournalType.ItemReceiptProduction)
                                     && u.ItemReceipt.Id == input.Id)
                                 .FirstOrDefaultAsync();


            if (journal == null || journal.ItemReceipt == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            if(input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                    .Where(t => (t.LockKey == TransactionLockType.ItemReceipt || t.LockKey == TransactionLockType.InventoryTransaction)
                     && t.IsLock == true && t.LockDate.Value.Date >= journal.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            
            var @entity = journal.ItemReceipt;

            //validate EditBy 48 hours
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

            var validateBill = await _billRepository.GetAll().Include(t => t.ItemReceipt)
                                             .Where(t => t.ItemReceiptId == input.Id).FirstOrDefaultAsync();
            if (validateBill != null && validateBill.CreationTime > validateBill.ItemReceipt.CreationTime)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            else if (validateBill != null && validateBill.ConvertToItemReceipt == true)
            {
                throw new UserFriendlyException(L("AutoConvertMessage"));
            }
            else if (validateBill != null)
            {
                validateBill.UpdateItemReceiptid(null);
                validateBill.UpdateReceivedStatus(DeliveryStatus.ReceivePending);
                CheckErrors(await _billManager.UpdateAsync(validateBill));
            }
             Guid? billId = null;
            if(validateBill != null)
            {
                billId = validateBill.Id;
            }

            await UpdatePOReceiptStautus(input.Id, entity.ReceiveFrom, billId, null);

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt);
            if (journal.JournalNo == auto.LastAutoSequenceNumber)
            {
                var jo = await _journalRepository.GetAll()
                    .Where(t => t.Id != journal.Id &&
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


            if (journal.JournalType == JournalType.ItemReceiptProduction && entity.ProductionOrder != null)
            {
                var production = entity.ProductionOrder;// await _productionOrderRepository.GetAll().Where(u => u.Id == entity.ProductionOrderId).FirstOrDefaultAsync();
                production.UpdateShipedStatus(TransferStatus.ShipAll);
                production.SetCalculateionState(CalculationState.NotSync);
                CheckErrors(await _productionOrderManager.UpdateAsync(production));
                entity.UpdateProductionOrderId(null);
            }
            else if (journal.JournalType == JournalType.ItemReceiptTransfer && entity.TransferOrder != null)
            {
                var transfer = entity.TransferOrder;// await _productionOrderRepository.GetAll().Where(u => u.Id == entity.ProductionOrderId).FirstOrDefaultAsync();
                transfer.UpdateShipedStatus(TransferStatus.ShipAll);
                CheckErrors(await _transferOrderManager.UpdateAsync(transfer));
                entity.UpdateTransferOrderId(null);
            }

            journal.UpdateItemReceipt(null);

            //query get journal item and delete
            var @jounalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == journal.Id).ToListAsync();
            foreach (var ji in jounalItems)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(ji));
            }

            var scheduleDate = journal.Date;

            CheckErrors(await _journalManager.RemoveAsync(journal));

            var itemBatchNos = await _itemReceiptItemBatchNoRepository.GetAll().Include(s => s.BatchNo).Where(s => s.ItemReceiptItem.ItemReceiptId == input.Id).AsNoTracking().ToListAsync();
            var batchNos = new List<BatchNo>();
            if (itemBatchNos.Any())
            {
                batchNos = itemBatchNos.GroupBy(s => s.BatchNoId).Select(s => s.FirstOrDefault().BatchNo).ToList();
                await _itemReceiptItemBatchNoRepository.BulkDeleteAsync(itemBatchNos);
            }

            //query get item receipt item and delete 
            var @itemReceiptItems = await _itemReceiptItemRepository.GetAll()
                                            .Include(u => u.OrderItem)
                                            .Where(u => u.ItemReceiptId == entity.Id).ToListAsync();

            var orderIds = itemReceiptItems
                            .Where(s => s.OrderItem != null)
                            .GroupBy(s => s.OrderItem.PurchaseOrderId)
                            .Select(s => s.Key)
                            .ToList();

            //Update itemReceiptitemId on table billitem
            var updateitemReceiptitemId = (from billitem in _billItemRepository.GetAll()
                                                .Include(s => s.OrderItem)
                                           join itemReceiptItem in _itemReceiptItemRepository.GetAll()
                                           .Include(u => u.ItemReceipt)
                                           on billitem.ItemReceiptItemId equals itemReceiptItem.Id
                                           where (itemReceiptItem.ItemReceiptId == entity.Id)
                                           select billitem).ToList();
            foreach (var u in updateitemReceiptitemId)
            {
                u.UpdateReceiptItemId(null);
                CheckErrors(await _billItemManager.UpdateAsync(u));
            }

            var scheduleItems = itemReceiptItems.Select(s => s.ItemId).Distinct().ToList();

            foreach (var iri in @itemReceiptItems)
            {
                CheckErrors(await _itemReceiptItemManager.RemoveAsync(iri));

            }

            if (updateitemReceiptitemId.Any()) {
                var ids = updateitemReceiptitemId.Where(s => s.OrderItem != null)
                          .GroupBy(s => s.OrderItem.PurchaseOrderId)
                          .Select(s => s.Key)
                          .ToList();

                orderIds.AddRange(ids); 
            }

            CheckErrors(await _itemReceiptManager.RemoveAsync(entity));

            if (orderIds.Any())
            {
                await CurrentUnitOfWork.SaveChangesAsync();
                foreach (var id in orderIds)
                {
                    await UpdatePurhaseOrderInventoryStatus(id);
                }
            }

            if (batchNos.Any())
            {
                await CurrentUnitOfWork.SaveChangesAsync();
                var allBatchUse = await GetBatchNoUseByOthers(input.Id, batchNos.Select(s => s.Id).ToList());
                var deleteBatchNos = batchNos.Where(s => !allBatchUse.Contains(s.Id)).ToList();
                if (deleteBatchNos.Any()) await _batchNoRepository.BulkDeleteAsync(deleteBatchNos);
            }


            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.ItemReceipt, TransactionLockType.InventoryTransaction, };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }

            await DeleteInventoryTransactionItems(input.Id);
            if (IsEnabled(AppFeatures.ReportFeatureAutoCalculation))
            {
                await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, scheduleDate, scheduleItems);
            }

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetList)]
        public async Task<PagedResultDto<GetListItemReceiptOut>> GetList(GetListItemReceiptInput input)
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

            var jQuery = _journalRepository.GetAll()
                            .Where(u => u.ItemReceiptId != null || u.ItemReceiptCustomerCreditId != null)
                            .WhereIf(input.JournalTypes != null && input.JournalTypes.Count > 0, u => input.JournalTypes.Contains(u.JournalType))
                            .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                            .WhereIf(input.Status != null && input.Status.Count > 0, u => input.Status.Contains(u.Status))
                            .WhereIf(input.Locations != null && input.Locations.Count > 0, u => u.Location != null && input.Locations.Contains(u.LocationId.Value))
                            .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                            .WhereIf(input.JournalTransactionTypeIds != null && input.JournalTransactionTypeIds.Count > 0, u => u.JournalTransactionTypeId != null && input.JournalTransactionTypeIds.Contains(u.JournalTransactionTypeId.Value))
                            .WhereIf(input.FromDate != null && input.ToDate != null,
                                (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                            .WhereIf(!input.Filter.IsNullOrEmpty(),
                                u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                    u.Reference.ToLower().Contains(input.Filter.ToLower()))
                            .AsNoTracking()
                            .Select(j => new
                            {
                                Id = j.Id,
                                CreationTimeIndex = j.CreationTimeIndex,
                                CreationTime = j.CreationTime,
                                Memo = j.Memo,
                                ItemReceiptId = j.ItemReceiptId ,
                                ItemReceiptCustomerCreditId = j.ItemReceiptCustomerCreditId,
                                JournalTransactionTypeName = j.JournalTransactionTypeId != null ? j.JournalTransactionType.Name : null,
                                Date = j.Date,
                                JournalNo = j.JournalNo,
                                Status = j.Status,
                                JournalType = j.JournalType,
                                CreatorUserId = j.CreatorUserId,
                                LocationId = j.LocationId,
                                Reference = j.Reference,
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
                                   ItemReceiptId = j.ItemReceiptId,
                                   JournalTransactionTypeName = j.JournalTransactionTypeName,
                                   ItemReceiptCustomerCreditId = j.ItemReceiptCustomerCreditId,
                                   Date = j.Date,
                                   JournalNo = j.JournalNo,
                                   Status = j.Status,
                                   JournalType = j.JournalType,
                                   CreatorUserId = j.CreatorUserId,
                                   LocationId = j.LocationId,
                                   UserName = u.UserName,
                                   LocationName = l.LocationName,
                                   Reference = j.Reference
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
                              JournalTransactionTypeName = j.JournalTransactionTypeName,
                              Reference = j.Reference,
                              Memo = j.Memo,
                              JournalType = j.JournalType,
                              ItemReceiptId = j.ItemReceiptId,
                              ItemReceiptCustomerCreditId = j.ItemReceiptCustomerCreditId,
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

            var icQuery = from ic in _itemReceiptCustomerCreditRepository.GetAll()
                                     .WhereIf(input.Customers != null && input.Customers.Count > 0, u => input.Customers.Contains(u.CustomerId))
                                     .AsNoTracking()
                                     .Select(s => new
                                     {
                                         Id = s.Id,
                                         Total = s.Total,
                                         CustomerId = s.CustomerId
                                     })
                          join ici in _itemReceiptCustomerCreditItemRepository.GetAll()
                                        .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                                        .AsNoTracking()
                                        .Select(s => s.ItemReceiptCustomerCreditId)
                          on ic.Id equals ici
                          into ics
                          where ics.Count() > 0
                          select ic;

            var itemReceiptCustomerCreditQeury = from rc in icQuery
                                                 join c in customerQuery
                                                 on rc.CustomerId equals c.Id
                                                 select new
                                                 {
                                                     Id = rc.Id,
                                                     Total = rc.Total,
                                                     CustomerId = rc.CustomerId,
                                                     CustomerName = c.CustomerName
                                                 };


            var irQuery = from ir in _itemReceiptRepository.GetAll()
                                    .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId))
                                    .AsNoTracking()
                                    .Select(s => new
                                    {
                                        Id = s.Id,
                                        Total = s.Total,
                                        VendorId = s.VendorId
                                    })
                          join iri in _itemReceiptItemRepository.GetAll()
                                        .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                                        .AsNoTracking()
                                        .Select(s => s.ItemReceiptId)
                          on ir.Id equals iri
                          into irs
                          where irs.Count() > 0
                          select ir;

            var itemReceiptQuery = from ir in irQuery
                                   join v in vendorQuery
                                   on ir.VendorId equals v.Id
                                   into vs
                                   from v in vs.DefaultIfEmpty()
                                   where (input.Vendors == null || input.Vendors.Count == 0 || v != null) &&
                                         (vendoTypeMemberIds.Count == 0 || v != null)
                                   select new
                                   {
                                       Id = ir.Id,
                                       Total = ir.Total,
                                       VendorId = v == null ? (Guid?)null : v.Id,
                                       VendorName = v == null ? "" : v.VendorName
                                   };

            var result = from j in jiQuery

                         join ir in itemReceiptQuery
                         on j.ItemReceiptId equals ir.Id
                         into irs
                         from ir in irs.DefaultIfEmpty()

                         join ic in itemReceiptCustomerCreditQeury
                         on j.ItemReceiptCustomerCreditId equals ic.Id
                         into ics
                         from ic in ics.DefaultIfEmpty()

                         where (ir != null || ic != null) &&
                               (input.Customers == null || input.Customers.Count == 0 || ic != null) &&
                               (input.Vendors == null || input.Vendors.Count == 0 || ir != null)

                         select new GetListItemReceiptOut
                         {
                             CreationTimeIndex = j.CreationTimeIndex,
                             CreationTime = j.CreationTime,
                             Memo = j.Memo,
                             LocationName = j.LocationName,
                             Id = ic != null ? ic.Id : ir.Id,
                             Date = j.Date,
                             JournalNo = j.JournalNo,
                             Status = j.Status,
                             Total = ic != null ? ic.Total : ir.Total,
                             Customer = ic != null ?
                                        new CustomerSummaryOutput
                                        {
                                            Id = ic.CustomerId,
                                            CustomerName = ic.CustomerName
                                        } : null,
                             Vendor = ir != null && ir.VendorId != null ?
                                        new VendorSummaryOutput
                                        {
                                            Id = ir.VendorId.Value,
                                            VendorName = ir.VendorName
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
                             Reference = j.Reference,
                             JournalTransactionTypeName = j.JournalTransactionTypeName,
                         };

            var resultCount = await result.CountAsync();
            if (resultCount == 0) return new PagedResultDto<GetListItemReceiptOut>(resultCount, new List<GetListItemReceiptOut>());

            if (input.Sorting.EndsWith("DESC"))
            {
                if (input.Sorting.ToLower().StartsWith("date"))
                {
                    result = result.OrderByDescending(s => s.Date.Date).ThenByDescending(s => s.CreationTimeIndex);
                }
                else if (input.Sorting.ToLower().StartsWith("typename"))
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
                else if (input.Sorting.ToLower().StartsWith("typename"))
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

            return new PagedResultDto<GetListItemReceiptOut>(resultCount, @entities);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetList)]
        public async Task<PagedResultDto<GetListItemReceiptOut>> GetListOld(GetListItemReceiptInput input)
        {

            // get user by group member
            var userGroups = await GetUserGroupByLocation();

            var vendoTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            var result = from j in _journalRepository.GetAll()
                            .Include(u => u.ItemReceipt.Vendor)
                            .Include(u => u.ItemReceiptCustomerCredit.Customer)
                            .Include(u => u.Location)
                            .Include(u => u.CreatorUser)
                            .Where(u => u.JournalType == JournalType.ItemReceiptAdjustment ||
                                         u.JournalType == JournalType.ItemReceiptOther ||
                                         u.JournalType == JournalType.ItemReceiptCustomerCredit ||
                                         u.JournalType == JournalType.ItemReceiptProduction ||
                                         u.JournalType == JournalType.ItemReceiptPurchase ||
                                         u.JournalType == JournalType.ItemReceiptPhysicalCount ||
                                         u.JournalType == JournalType.ItemReceiptTransfer)

                            .WhereIf(input.JournalTypes != null && input.JournalTypes.Count > 0, u => input.JournalTypes.Contains(u.JournalType))
                            .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                            .WhereIf(input.Status != null && input.Status.Count > 0, u => input.Status.Contains(u.Status))
                            .WhereIf(input.Locations != null && input.Locations.Count > 0, u => u.Location != null && input.Locations.Contains(u.LocationId.Value))
                            .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
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

                         join irca in _itemReceiptCustomerCreditItemRepository.GetAll()
                        .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId)).AsNoTracking()
                        .GroupBy(u => u.ItemReceiptCustomerCredit).Select(u => u.Key)
                        on j.ItemReceiptCustomerCreditId equals irca.Id into itemReceiptCustomerItems
                         from leftIrca in itemReceiptCustomerItems.DefaultIfEmpty()

                         join iri in _itemReceiptItemRepository.GetAll()
                                   .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId)).AsNoTracking()
                                   .GroupBy(u => u.ItemReceipt).Select(u => u.Key)
                                   on j.ItemReceiptId equals iri.Id into ItemReceiptItems
                         from leftIri in ItemReceiptItems.DefaultIfEmpty()


                         where (leftIrca != null || leftIri != null)

                         select new GetListItemReceiptOut
                         {
                             CreationTimeIndex = j.CreationTimeIndex,
                             Memo = j.Memo,
                             Id = j.ItemReceipt != null ? j.ItemReceipt.Id : j.ItemReceiptCustomerCredit.Id,
                             Date = j.Date,
                             JournalNo = j.JournalNo,
                             Status = j.Status,
                             Total = j.ItemReceipt != null ? j.ItemReceipt.Total : j.ItemReceiptCustomerCredit.Total,
                             Vendor = j.ItemReceipt != null ? ObjectMapper.Map<VendorSummaryOutput>(j.ItemReceipt.Vendor) : null,

                             Customer = j.ItemReceiptCustomerCredit != null ? ObjectMapper.Map<CustomerSummaryOutput>(j.ItemReceiptCustomerCredit.Customer) : null,
                             LocationName = j.Location.LocationName,
                             Type = j.JournalType,
                             TypeName = Enum.GetName(j.JournalType.GetType(), j.JournalType),
                             AccountId = headerAccount == null ? Guid.NewGuid() : headerAccount.AccountId,
                             AccountName = headerAccount == null || headerAccount.Account == null ? null : headerAccount.Account.AccountName,
                             User = ObjectMapper.Map<UserDto>(j.CreatorUser),
                         };

            result = result.WhereIf(input.Accounts != null && input.Accounts.Count > 0, u => input.Accounts.Contains(u.AccountId))
                            .WhereIf(input.Customers != null && input.Customers.Count > 0, u => u.Customer != null && input.Customers.Contains(u.Customer.Id))
                            .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => u.Vendor != null && input.Vendors.Contains(u.Vendor.Id))
                            .WhereIf(vendoTypeMemberIds.Any(), s => s.Vendor == null || vendoTypeMemberIds.Contains(s.Vendor.VendorTypeId.Value));
            var resultCount = await result.CountAsync();

            if (input.Sorting.Contains("date") && !input.Sorting.Contains("."))
                input.Sorting = input.Sorting.Replace("date", "Date.Date");

            input.Sorting += ", CreationTimeIndex" + (input.Sorting.Contains("DESC") ? " DESC" : "");


            var @entities = await result.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<GetListItemReceiptOut>(resultCount, @entities);


        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_Find)]
        public async Task<PagedResultDto<GetListItemReceiptOut>> Find(GetListItemReceiptInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();

            var query = (from jItem in _journalItemRepository.GetAll().AsNoTracking()
                         join j in _journalRepository.GetAll().AsNoTracking()
                        .Where(u => u.JournalType == JournalType.ItemReceiptPurchase)
                        .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                        .WhereIf(input.Bills != null && input.Bills.Count > 0, u => input.Bills.Contains(u.BillId.Value))
                        .WhereIf(input.Status != null && input.Status.Count > 0, u => input.Status.Contains(u.Status))
                         .WhereIf(input.JournalTransactionTypeIds != null && input.JournalTransactionTypeIds.Count > 0, u => u.JournalTransactionTypeId != null && input.JournalTransactionTypeIds.Contains(u.JournalTransactionTypeId.Value))
                        .WhereIf(input.FromDate != null && input.ToDate != null,
                         (u => (u.Date.Date) >= (input.FromDate) && (u.Date.Date) <= (input.ToDate)))
                         .WhereIf(!input.Filter.IsNullOrEmpty(),
                         u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()))
                         on jItem.JournalId equals j.Id

                         join iri in _itemReceiptItemRepository.GetAll().AsNoTracking()
                         // .WhereIf(input.Bills != null && input.Bills.Count > 0, u => input.Bills.Contains(u.BillItemId.Value))
                         .WhereIf(input.OrderNo != null && input.OrderNo.Count > 0, u => input.OrderNo.Contains(u.OrderItem.PurchaseOrder.Id))
                         .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))

                         on jItem.Identifier equals iri.Id

                         join ir in _itemReceiptRepository.GetAll().AsNoTracking()
                         .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId.Value)) on j.ItemReceiptId equals ir.Id

                         join v in _vendorRepository.GetAll().AsNoTracking() on ir.VendorId equals v.Id

                         group iri by new { journal = j, itemReceipt = ir, vendor = v } into u

                         select new GetListItemReceiptOut
                         {
                             //CountItem = u.Count(),
                             Memo = u.Key.journal.Memo,
                             Id = u.Key.itemReceipt.Id,
                             Date = u.Key.journal.Date,
                             JournalNo = u.Key.journal.JournalNo,
                             Status = u.Key.journal.Status,
                             Total = u.Key.itemReceipt.Total,
                             Vendor = ObjectMapper.Map<VendorSummaryOutput>(u.Key.vendor),
                             VendorId = u.Key.itemReceipt.VendorId
                         });

            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            return new PagedResultDto<GetListItemReceiptOut>(resultCount, @entities);
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_Update,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_Update,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour)]
        public async Task<NullableIdDto<Guid>> Update(UpdateItemReceiptInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            
            // update Journal 
            var @journal = await _journalRepository
                              .GetAll()
                              .Include(u => u.ItemReceipt)
                              .Include(u => u.ItemReceipt.TransferOrder)
                              .Include(u => u.ItemReceipt.ProductionOrder)
                              .Where(u => u.JournalType == JournalType.ItemReceiptPurchase && u.ItemReceiptId == input.Id)
                              .FirstOrDefaultAsync();

            await CheckClosePeriod(journal.Date, input.Date);
            var @itemReceipt = journal.ItemReceipt;
            var receiptFrom = itemReceipt.ReceiveFrom;


            if(input.IsConfirm == false)
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

            await ValidateAddBatchNo(input);

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
            else if (@itemReceipt.TransferOrderId != null && @itemReceipt.TransferOrder != null && @itemReceipt.TransferOrder.ConvertToIssueAndReceiptTransfer == true)
            {
                throw new UserFriendlyException(L("AutoConvertMessage"));
            }
            else if (@itemReceipt.ProductionOrderId != null && @itemReceipt.ProductionOrder != null && @itemReceipt.ProductionOrder.ConvertToIssueAndReceipt == true)
            {
                throw new UserFriendlyException(L("AutoConvertMessage"));
            }

            var validateBill = await _billRepository.GetAll().AsNoTracking().Include(t => t.ItemReceipt)
                                              .Where(t => t.ItemReceiptId == input.Id).FirstOrDefaultAsync();
            if (validateBill != null && validateBill.CreationTime > validateBill.ItemReceipt.CreationTime)
            {
                throw new UserFriendlyException(L("ReceivePendingOnlyCanChange"));
            }
            else if (validateBill != null && validateBill.ConvertToItemReceipt == true)
            {
                throw new UserFriendlyException(L("AutoConvertMessage"));
            }

            if (input.ReceiveFrom == ItemReceipt.ReceiveFromStatus.PO && itemReceipt.ReceiveFrom != ItemReceipt.ReceiveFromStatus.PO)
            {
                receiptFrom = ItemReceipt.ReceiveFromStatus.PO;
            }
            await UpdatePOReceiptStautus(input.Id, receiptFrom, input.BillId, input.Items);


            var scheduleDate = input.Date > journal.Date ? journal.Date : input.Date;

            journal.Update(tenantId, input.ReceiptNo, input.Date, input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, 0, input.LocationId);
            journal.UpdateStatus(input.Status);
            //update Clearance account 
            var @clearanceAccountItem = await (_journalItemRepository.GetAll()
                                              .Where(u => u.JournalId == journal.Id &&
                                                    u.Key == PostingKey.Clearance && u.Identifier == null)
                                        ).FirstOrDefaultAsync();
            clearanceAccountItem.UpdateJournalItem(tenantId, input.ClearanceAccountId, input.Memo, 0, input.Total);


            @itemReceipt.Update(tenantId, input.ReceiveFrom, input.VendorId,
                          input.SameAsShippingAddress, input.ShippingAddress,
                          input.BillingAddress, input.Total, null);

            // update if come from bill 
            if (input.ReceiveFrom == ItemReceipt.ReceiveFromStatus.Bill && input.BillId != null)
            {
                var bill = await _billRepository.GetAll().Where(u => u.Id == input.BillId).FirstOrDefaultAsync();
                bill.UpdateItemReceiptid(itemReceipt.Id);
                // update received status in bill to full when receive from bill 
                if (input.Status == TransactionStatus.Publish)
                {
                    bill.UpdateReceivedStatus(DeliveryStatus.ReceiveAll);
                }

                CheckErrors(await _billManager.UpdateAsync(bill));
            }
            
            else // in some case if user switch from receive from bill to other so update bill of field item receipt id to null
            {
                var bill = await _billRepository.GetAll().Where(u => u.ItemReceiptId == input.Id).FirstOrDefaultAsync();
                if (bill != null)
                {
                    bill.UpdateItemReceiptid(null);
                    bill.UpdateReceivedStatus(DeliveryStatus.ReceivePending);
                    CheckErrors(await _billManager.UpdateAsync(bill));
                }
            }

          
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemReceipt);
            CheckErrors(await _journalManager.UpdateAsync(@journal, auto.DocumentType));

            CheckErrors(await _journalItemManager.UpdateAsync(@clearanceAccountItem));
            CheckErrors(await _itemReceiptManager.UpdateAsync(@itemReceipt));

            //Update Item Receipt Item and Journal Item
            var itemReceipItems = await _itemReceiptItemRepository.GetAll()
                                        .Include(u => u.OrderItem)
                                        .Where(u => u.ItemReceiptId == input.Id).ToListAsync();

            var oldOrderIds = itemReceipItems
                          .Where(s => s.OrderItem != null)
                          .GroupBy(s => s.OrderItem.PurchaseOrderId)
                          .Select(s => s.Key).ToList();

            var newOrderIds = input.Items.Where(s => s.PurchaseOrderId.HasValue).GroupBy(s => s.PurchaseOrderId).Select(s => s.Key.Value).ToList();
            var orderIds = oldOrderIds.Union(newOrderIds).ToList();

            if (input.ReceiveFrom == ItemReceipt.ReceiveFromStatus.Bill)
            {
                var orderIdsFromBill = await _billItemRepository.GetAll()
                                             .Include(s => s.OrderItem)
                                             .Where(s => s.BillId == input.BillId)
                                             .Where(s => s.OrderItemId.HasValue)
                                             .GroupBy(s => s.OrderItem.PurchaseOrderId)
                                             .Select(s => s.Key)
                                             .ToListAsync();
                if (orderIdsFromBill.Any()) orderIds = orderIds.Union(orderIdsFromBill).ToList();
            }

            var @inventoryJournalItems = await (_journalItemRepository.GetAll()
                                  .Where(u => u.JournalId == journal.Id &&
                                           u.Key == PostingKey.Inventory && u.Identifier != null)).ToListAsync();

            var toDeleteItemReceiptItem = itemReceipItems.Where(u => !input.Items.Any(i => i.Id != null && i.Id == u.Id)).ToList();
            var toDeletejournalItem = inventoryJournalItems.Where(u => !input.Items.Any(i => u.Identifier != null && i.Id != null && i.Id == u.Identifier)).ToList();

            //delete validate when use in vendorcredit and itemIssueVendorCredit
            var vendorCredit = await _vendorCreditItemRepository.GetAll().Include(u=>u.ItemReceiptItem)
                .Where(t => (toDeleteItemReceiptItem.Any(g => g.Id == t.ItemReceiptItemId)) ||
                (input.Items.Any(g => g.Id != null && g.Id == t.ItemReceiptItemId && t.ItemReceiptItem != null && (g.Qty != t.ItemReceiptItem.Qty || g.LotId != t.ItemReceiptItem.LotId)))).AsNoTracking().CountAsync();
            var itemIssueVendorCredit = await _itemIssueVendorCreditItemRepository.GetAll()
                .Include(u=>u.ItemReceiptPurchaseItem)
                .Where(t => (toDeleteItemReceiptItem.Any(g => g.Id == t.ItemReceiptPurchaseItemId)) ||
                (input.Items.Any(g => g.Id != null && g.Id == t.ItemReceiptPurchaseItemId && t.ItemReceiptPurchaseItem != null && (g.Qty != t.ItemReceiptPurchaseItem.Qty || g.LotId != t.ItemReceiptPurchaseItem.LotId)))).AsNoTracking().CountAsync();

            if (vendorCredit > 0 || itemIssueVendorCredit > 0)
            {
                throw new UserFriendlyException(L("ItemAlreadyReturnCannotBeModified"));
            }

            if (input.ReceiveFrom == ItemReceipt.ReceiveFromStatus.None && input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }

            var ItemReceiptItems = (from ItemReceiptitem in input.Items where (ItemReceiptitem.OrderItemId != null) select ItemReceiptitem.OrderItemId);
            var count = ItemReceiptItems.Count();
            if (input.ReceiveFrom == ItemReceipt.ReceiveFromStatus.PO && count <= 0)
            {
                throw new UserFriendlyException(L("PleaseAddPuchaseOrder"));
            }

            var itemBatchNos = await _itemReceiptItemBatchNoRepository.GetAll().Include(s => s.BatchNo).Where(s => itemReceipItems.Any(r => r.Id == s.ItemReceiptItemId)).AsNoTracking().ToListAsync();
            var batchNos = new List<BatchNo>();
            if (toDeleteItemReceiptItem.Any())
            {
                var deleteItemBatchNos = itemBatchNos.Where(s => toDeleteItemReceiptItem.Any(r => r.Id == s.ItemReceiptItemId)).ToList();
                if (deleteItemBatchNos.Any())
                {
                    var batchs = deleteItemBatchNos.GroupBy(s => s.BatchNoId).Select(s => s.FirstOrDefault().BatchNo).ToList();
                    if(batchs.Any()) batchNos.AddRange(batchNos);

                    await _itemReceiptItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
                }
            }

            decimal subTotalTeamQty = 0;
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
                        //old
                        if (itemReceipItem.OrderItem != null)
                        {

                            itemReceipItem.UpdateOrderItemId(null);
                            itemReceipItem.UpdateLot(c.LotId);
                        }

                        //new
                        itemReceipItem.Update(tenantId, c.ItemId, c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total);
                        itemReceipItem.UpdateLot(c.LotId);
                        CheckErrors(await _itemReceiptItemManager.UpdateAsync(itemReceipItem));

                        if (input.ReceiveFrom != ItemReceipt.ReceiveFromStatus.None && c.OrderItemId != null)
                        {
                            //Update Purchase Order item when itemreipt status != None                          
                            itemReceipItem.UpdateOrderItemId(c.OrderItemId);
                            itemReceipItem.UpdateLot(c.LotId);
                        }
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
                            if(batchs.Any()) batchNos.AddRange(batchs);
                            await _itemReceiptItemBatchNoRepository.BulkDeleteAsync(deleteItemBatchNos);
                        }
                    }
                    else if (oldItemBatchs.Any())
                    {
                        var batchs = oldItemBatchs.Where(s => !batchNos.Any(r => r.Id == s.BatchNoId)).GroupBy(s => s.BatchNoId).Select(s => s.FirstOrDefault().BatchNo).ToList();
                        if(batchs.Any()) batchNos.AddRange(batchs);

                        await _itemReceiptItemBatchNoRepository.BulkDeleteAsync(oldItemBatchs);
                    }
                }
                else if (c.Id == null) //create
                {

                    //insert to item Receipt item
                    var @itemReceiptItem = ItemReceiptItem.Create(tenantId, userId, @itemReceipt, c.ItemId,
                                                                                 c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total);
                    itemReceiptItem.UpdateLot(c.LotId);
                    if (input.ReceiveFrom != ItemReceipt.ReceiveFromStatus.None &&
                        c.OrderItemId != null)
                    {
                        //Update Purchase Order item when itemreipt status != None                      
                        itemReceiptItem.UpdateOrderItemId(c.OrderItemId.Value);
                        itemReceiptItem.UpdateLot(c.LotId);
                    }

                    //Update table bill and bill item
                    if (input.ReceiveFrom == ItemReceipt.ReceiveFromStatus.Bill &&
                       c.BillItemId != null)
                    {
                        var billItem = await _billItemRepository
                                                       .GetAll()
                                                       .Where(u => u.Id == c.BillItemId).FirstOrDefaultAsync();
                        billItem.UpdateReceiptItemId(@itemReceiptItem.Id);
                        billItem.UpdateLot(@itemReceiptItem.LotId);
                        
                        @itemReceiptItem.UpdateLot(c.LotId);
                        CheckErrors(await _billItemManager.UpdateAsync(billItem));

                    }
                    CheckErrors(await _itemReceiptItemManager.CreateAsync(@itemReceiptItem));
                    //insert inventory journal item into debit
                    var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal, c.InventoryAccountId, c.Description, c.Total, 0, PostingKey.Inventory, itemReceiptItem.Id);
                    CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));

                    if (c.ItemBatchNos != null && c.ItemBatchNos.Any())
                    {
                        var addItemBatchNos = c.ItemBatchNos.Select(s => ItemReceiptItemBatchNo.Create(tenantId, userId, itemReceiptItem.Id, s.BatchNoId, s.Qty)).ToList();
                        foreach(var a in addItemBatchNos)
                        {
                            await _itemReceiptItemBatchNoRepository.InsertAsync(a);
                        }  
                    }
                }

            }

            var scheduleItems = input.Items.Select(s => s.ItemId).Concat(toDeleteItemReceiptItem.Select(s => s.ItemId)).Distinct().ToList();

            foreach (var t in toDeleteItemReceiptItem.OrderBy(u => u.Id))
            {
                // add record order item id back into bill item from ui to get update PO status

                
                var bi = await _billItemRepository.GetAll().Where(u => u.ItemReceiptItemId == t.Id).FirstOrDefaultAsync();

                if (bi != null)
                {
                    bi.UpdateReceiptItemId(null);

                    var inv = await _billRepository.GetAll().Where(u => u.Id == bi.BillId).FirstOrDefaultAsync();

                    if (inv != null)
                    {
                        inv.UpdateItemReceiptid(null);
                    }
                }
                CheckErrors(await _itemReceiptItemManager.RemoveAsync(t));

            }

            foreach (var t in toDeletejournalItem)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(t));
            }
            /******************************************************************************
             Update if status publish and receive from is from Bill than automatically update 
             receive status in bill to received all
             ******************************************************************************/
            if (input.BillId != Guid.Empty && input.BillId != null && input.Status == TransactionStatus.Publish)
            {
                var bill = await _billRepository.GetAll().Where(u => u.Id == input.BillId).FirstOrDefaultAsync();
                bill.UpdateReceivedStatus(DeliveryStatus.ReceiveAll);
                CheckErrors(await _billManager.UpdateAsync(bill));

            }

            //need save change before performing bellow task
            await CurrentUnitOfWork.SaveChangesAsync();

            if (orderIds.Any())
            {
                foreach (var id in orderIds)
                {
                    await UpdatePurhaseOrderInventoryStatus(id);
                }
            }

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


            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.ItemReceipt, TransactionLockType.InventoryTransaction, };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }

            return new NullableIdDto<Guid>() { Id = @itemReceipt.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts,
                      AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts_VendorCredit)]
        public async Task<PagedResultDto<ItemReceiptSummaryOutput>> GetItemReceipts(GetItemReceiptInput input)
        {

            var vendoTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            // get user by group member
            var userGroups = await GetUserGroupByLocation();

            var vendorQuery = GetVendors(null, new List<Guid>(), null, vendoTypeMemberIds);
            var currencyQuery = GetCurrencies();

            var irQuery = from ir in _itemReceiptRepository.GetAll()
                              .Where(t => t.TransactionType == InventoryTransactionType.ItemReceiptPurchase)
                              .WhereIf(vendoTypeMemberIds.Any(), s => vendoTypeMemberIds.Contains(s.Vendor.VendorTypeId.Value))
                              .AsNoTracking()
                              .Select(s => new
                              {
                                  Id = s.Id,
                                  VendorId = s.VendorId,
                                  Total = s.Total
                              })
                          join iri in _itemReceiptItemRepository.GetAll()
                                    .AsNoTracking()
                                    .Select(s => s.ItemReceiptId)
                          on ir.Id equals iri
                          into irs
                          where irs.Count() > 0
                          select new
                          {
                              Id = ir.Id,
                              VendorId = ir.VendorId,
                              Total = ir.Total,
                              ItemCount = irs.Count()
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
                              ItemReceiptId = j.ItemReceiptId,
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
                             ItemReceiptId = j.ItemReceiptId,
                             CurrencyId = j.CurrencyId,
                             Date = j.Date,
                             JournalNo = j.JournalNo,
                             Reference = j.Reference,
                             CreationTimeIndex = j.CreationTimeIndex,
                             CurrencyCode = c.Code
                         };

            var query = from ir in irQuery
                        join v in vendorQuery
                        on ir.VendorId equals v.Id
                        join j in jQuery
                        on ir.Id equals j.ItemReceiptId
                        join i in _billRepository.GetAll()
                                  .AsNoTracking()
                                  .Select(s => s.ItemReceiptId)
                        on ir.Id equals i
                        into ivs
                        where ivs.Count() == 0
                        select new ItemReceiptSummaryOutput
                        {
                            CountItems = ir.ItemCount,
                            Vendor = new VendorSummaryOutput
                            {
                                Id = v.Id,
                                VendorName = v.VendorName
                            },
                            VendorId = ir.VendorId.Value,
                            Id = ir.Id,
                            ItemReceiptNo = j.JournalNo,
                            CurrencyId = j.CurrencyId,
                            Currency = new CurrencyDetailOutput
                            {
                                Id = j.CurrencyId,
                                Code = j.CurrencyCode
                            },
                            Date = j.Date,
                            Reference = j.Reference,
                            Total = ir.Total,
                            CreationTimeIndex = j.CreationTimeIndex
                        };

            var resultCount = await query.CountAsync();
            if (resultCount == 0) return new PagedResultDto<ItemReceiptSummaryOutput>(resultCount, new List<ItemReceiptSummaryOutput>());

            var @entities = await query.OrderBy(s => s.Date.Date).ThenBy(t => t.CreationTimeIndex).PageBy(input).ToListAsync();
            return new PagedResultDto<ItemReceiptSummaryOutput>(resultCount, entities);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts,
                      AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts_VendorCredit)]
        public async Task<PagedResultDto<ItemReceiptSummaryOutput>> GetItemReceiptsOld(GetItemReceiptInput input)
        {

            var vendoTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var @query = (from ir in _itemReceiptRepository.GetAll().AsNoTracking()
                            .Where(t => t.TransactionType == InventoryTransactionType.ItemReceiptPurchase)
                              //.WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId.Value))
                            .WhereIf(vendoTypeMemberIds.Any(), s => vendoTypeMemberIds.Contains(s.Vendor.VendorTypeId.Value))    
                          join iri in _itemReceiptItemRepository.GetAll().AsNoTracking()
                          on ir.Id equals iri.ItemReceiptId into count

                          join j in _journalRepository.GetAll()
                          .Include(u => u.Currency)
                          .WhereIf(!input.Filter.IsNullOrEmpty(),
                                u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) || u.Reference.ToLower().Contains(input.Filter.ToLower()))
                          //.WhereIf(input.Locations != null && input.Locations.Count > 0, t => input.Locations.Contains(t.LocationId))
                          .Where(u => u.Status == TransactionStatus.Publish)
                          .WhereIf(userGroups != null && userGroups.Count > 0,
                                u => userGroups.Contains(u.LocationId.Value))
                          .AsNoTracking()

                          on ir.Id equals j.ItemReceiptId
                          join b in _billRepository.GetAll().AsNoTracking()
                          on ir.Id equals b.ItemReceiptId into ib
                          from n in ib.DefaultIfEmpty()
                          where n == null
                          select new ItemReceiptSummaryOutput
                          {
                              CountItems = count.Count(),
                              Vendor = ObjectMapper.Map<VendorSummaryOutput>(ir.Vendor),
                              VendorId = ir.VendorId.Value,
                              Id = ir.Id,
                              ItemReceiptNo = j.JournalNo,
                              CurrencyId = j.CurrencyId,
                              Currency = ObjectMapper.Map<CurrencyDetailOutput>(j.Currency),
                              Date = j.Date,
                              Reference = j.Reference,
                              Total = ir.Total,
                              CreationTimeIndex = j.CreationTimeIndex,
                          }).Where(u => u.CountItems > 0);
            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).ThenBy(t => t.CreationTimeIndex).PageBy(input).ToListAsync();
            //var @entities = await query.OrderBy(u=>u.Date).ToListAsync();
            return new PagedResultDto<ItemReceiptSummaryOutput>(resultCount, entities);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceiptItems)]
        public async Task<ItemReceiptSummaryOutputForItemReceiptItem> GetItemReceiptItems(EntityDto<Guid> input)
        {
            var @journal = await _journalRepository
                                           .GetAll()
                                           .Include(u => u.ItemReceipt)
                                           .Include(u => u.Currency)
                                           .Include(u => u.ItemReceipt.Vendor)
                                           .Include(u => u.ItemReceipt.Vendor.ChartOfAccount)
                                           .Include(u => u.Class)
                                           .Include(u => u.Location)
                                           .Include(u => u.ItemReceipt.ShippingAddress)
                                           .Include(u => u.ItemReceipt.BillingAddress)
                                           .AsNoTracking()
                                           .Where(u => u.JournalType == JournalType.ItemReceiptPurchase && u.ItemReceiptId == input.Id)
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
                        .Include(u => u.OrderItem)
                        .Include(u => u.OrderItem.PurchaseOrder)
                        .Include(u => u.Lot)
                        .Where(u => u.ItemReceiptId == input.Id)

                        .Join(_journalItemRepository.GetAll()
                        .Include(u => u.Account)
                        .Where(u => u.Key == PostingKey.Inventory)
                        .AsNoTracking(), u => u.Id, s => s.Identifier,
                        (rItem, jItem) =>
                        new ItemReceiptItemSummaryOutput()
                        {
                            Id = rItem.Id,
                            Item = ObjectMapper.Map<ItemSummaryOutput>(rItem.Item),                           
                            ItemId = rItem.ItemId,
                            InventoryAccountId = jItem.AccountId,
                            InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                            Description = rItem.Description,
                            DiscountRate = rItem.DiscountRate,
                            Qty = rItem.Qty,
                            // Tax = ObjectMapper.Map<TaxSummaryOutput>(rItem.Tax),
                            Total = rItem.Total,
                            UnitCost = rItem.UnitCost,
                            ToLotId = rItem.LotId,
                            ToLotDetail = ObjectMapper.Map<LotSummaryOutput>(rItem.Lot),
                            PurchaseOrderId = rItem.OrderItem.PurchaseOrderId,
                            PurchaseOrderItemId = rItem.OrderItemId,
                            OrderNumber = rItem.OrderItem.PurchaseOrder.OrderNumber,
                            //  TaxId = rItem.TaxId,
                            UseBatchNo = rItem.Item.UseBatchNo,
                            AutoBatchNo = rItem.Item.AutoBatchNo,
                            TrackSerial = rItem.Item.TrackSerial,
                            TrackExpiration = rItem.Item.TrackExpiration
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

            var result = ObjectMapper.Map<ItemReceiptSummaryOutputForItemReceiptItem>(journal.ItemReceipt);
            result.ReceiveNo = journal.JournalNo;
            result.Date = journal.Date;
            result.Reference = journal.Reference;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.LocationId = journal.LocationId.Value;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.ItemReceiptItems = itemReceiptItems;
            result.ClearanceAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(clearanceAccount);
            result.ClearanceAccountId = clearanceAccount.Id;
            result.Memo = journal.Memo;
            return result;
        }

        #region get item Receipt for vendor credit 
        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts,
                      AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts_VendorCredit,
                      AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts_ItemIssueVendorCredit,
                      AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueVendorCredit_CanItemReceiptPurchase)]
        public async Task<PagedResultDto<ItemReceiptitemFromVendorCreditOutput>> GetNewItemReceiptForVendorCredits(GetNewItemReceiptInput input, Guid? exceptId)
        {
            var vendoTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            var roundDigits = await _accountCycleRepository.GetAll().Select(t => t.RoundingDigit).FirstOrDefaultAsync();
            var userGroups = await GetUserGroupByLocation();


            var jQuery = from ir in _itemReceiptRepository.GetAll()
                                    .Where(t => t.TransactionType == InventoryTransactionType.ItemReceiptPurchase)
                                    .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId.Value))
                                    .WhereIf(vendoTypeMemberIds.Any(), s => vendoTypeMemberIds.Contains(s.Vendor.VendorTypeId.Value))
                                    .AsNoTracking()
                         join j in _journalRepository.GetAll()
                                     .WhereIf(input.Locations != null && input.Locations.Count > 0, t => input.Locations.Contains(t.LocationId))
                                     .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                                     .WhereIf(input.FromDate != null && input.ToDate != null,
                                            (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                                     .Where(u => u.Status == TransactionStatus.Publish)
                                     .AsNoTracking()
                         on ir.Id equals j.ItemReceiptId
                         select new
                         {
                             ItemReceiptId = j.ItemReceiptId,
                             Date = j.Date,
                             JournalNo = j.JournalNo,
                             j.Reference,
                             CreationTimeIndex = j.CreationTimeIndex
                         };

            var iriQuery = from ri in _itemReceiptItemRepository.GetAll()
                                       .WhereIf(input.Lots != null && input.Lots.Count > 0, u => input.Lots.Contains(u.LotId))
                                       .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                                       .AsNoTracking()
                           join b in _itemReceiptItemBatchNoRepository.GetAll().AsNoTracking()
                           on ri.Id equals b.ItemReceiptItemId
                           into bs from b in bs.DefaultIfEmpty()
                          
                           select new
                           {
                               ItemReceiptId = ri.ItemReceiptId,
                               ItemId = ri.ItemId,
                               LotId = ri.LotId,
                               Qty = b == null ? ri.Qty : b.Qty,
                               Id = ri.Id,
                               CreationTime = ri.CreationTime,
                               LotName = ri.Lot.LotName,
                               ItemName = ri.Item.ItemName,
                               ItemCode = ri.Item.ItemCode,
                               BatchNoId = b == null ? (Guid?) null : b.BatchNoId,
                               BatchNumber = b == null ? "" : b.BatchNo.Code,
                               ExpirationDate = b == null ? (DateTime?)null : b.BatchNo.ExpirationDate,
                               ri.Item.UseBatchNo,
                               ri.Item.TrackSerial,
                               ri.Item.TrackExpiration
                           };

            var vciQuery = from vi in _vendorCreditItemRepository.GetAll()
                                     .Where(s => s.ItemReceiptItemId != null)
                                     .WhereIf(exceptId != null, s => s.VendorCreditId != exceptId)
                                     .AsNoTracking()
                           join j in _journalRepository.GetAll()
                                     .Where(s => s.VendorCreditId.HasValue)
                                     .Where(s => s.Status == TransactionStatus.Publish)
                                     .AsNoTracking()
                           on vi.VendorCreditId equals j.VendorCreditId
                           join b in _vendorCreditItemBatchNoRepository.GetAll()
                                     .AsNoTracking()
                           on vi.Id equals b.VendorCreditItemId
                           into bs
                           from b in bs.DefaultIfEmpty()

                           select new
                           {
                               vi.ItemReceiptItemId,
                               Qty = b == null ? vi.Qty : b.Qty,
                               BatchNoId = b == null ? (Guid?)null : b.BatchNoId,
                               BatchNumber = b == null ? "" : b.BatchNo.Code
                           };

            var iviQuery = from vi in _itemIssueVendorCreditItemRepository.GetAll()
                                .Where(s => s.ItemReceiptPurchaseItemId != null)
                                .WhereIf(exceptId != null, s => s.ItemIssueVendorCreditId != exceptId)
                                .AsNoTracking()
                           join j in _journalRepository.GetAll()
                                      .Where(s => s.ItemIssueVendorCreditId.HasValue)
                                      .Where(s => s.Status == TransactionStatus.Publish)
                                      .AsNoTracking()
                           on vi.ItemIssueVendorCreditId equals j.ItemIssueVendorCreditId
                           join b in _itemIssueVendorCreditItemBatchNoRepository.GetAll()
                                     .AsNoTracking()
                           on vi.Id equals b.ItemIssueVendorCreditItemId
                           into bs
                           from b in bs.DefaultIfEmpty()

                           select new
                           {
                               ItemReceiptPurchaseItemId = vi.ItemReceiptPurchaseItemId,
                               Qty = b == null ? vi.Qty : b.Qty,
                               BatchNoId = b == null ? (Guid?)null : b.BatchNoId,
                               BatchNumber = b == null ? "" : b.BatchNo.Code
                           };

            var query = from iri in iriQuery
                        join j in jQuery
                        on iri.ItemReceiptId equals j.ItemReceiptId

                        join vci in vciQuery
                        on $"{iri.Id}-{iri.BatchNoId}" equals $"{vci.ItemReceiptItemId}-{vci.BatchNoId}" 
                        into returnItems

                        join ivi in iviQuery
                        on $"{iri.Id}-{iri.BatchNoId}" equals $"{ivi.ItemReceiptPurchaseItemId}-{ivi.BatchNoId}" 
                        into returnItems2

                        let returnQty = returnItems == null ? 0 : returnItems.Sum(s => s.Qty)
                        let returnQty2 = returnItems2 == null ? 0 : returnItems2.Sum(s => s.Qty)
                        let remainingQty = iri.Qty - returnQty - returnQty2
                      
                        where remainingQty > 0 && (
                             input.Filter.IsNullOrWhiteSpace() ||
                             j.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                             (!j.Reference.IsNullOrWhiteSpace() && j.Reference.ToLower().Contains(input.Filter.ToLower())) ||
                             (!iri.BatchNumber.IsNullOrWhiteSpace() && iri.BatchNumber.ToLower().Contains(input.Filter.ToLower())))


                        select new ItemReceiptitemFromVendorCreditOutput
                        {
                            Date = j.Date,
                            Id = iri.Id,
                            ItemCode = iri.ItemCode,
                            ItemName = iri.ItemName,
                            ItemId = iri.ItemId,
                            ItemReceiptId = iri.ItemReceiptId,
                            LotId = iri.LotId,
                            LotName = iri.LotName,
                            Qty = remainingQty,
                            ReceiptNo = j.JournalNo,
                            CreationTimeIndex = j.CreationTimeIndex,
                            ItemDateTime = iri.CreationTime,
                            BatchNoId = iri.BatchNoId,
                            BatchNumber = iri.BatchNumber,
                            ExpirationDate = iri.ExpirationDate,
                            UseBatchNo = iri.UseBatchNo,
                            TrackSerial = iri.TrackSerial,
                            TrackExpiration = iri.TrackExpiration
                        };


            var resultCount = await query.CountAsync();
            if(resultCount == 0) return new PagedResultDto<ItemReceiptitemFromVendorCreditOutput>(resultCount, new List<ItemReceiptitemFromVendorCreditOutput>());

            var @entities = await query.OrderBy(s => s.Date.Date).ThenBy(t => t.CreationTimeIndex).ThenBy(t => t.ItemDateTime).PageBy(input).ToListAsync();
            return new PagedResultDto<ItemReceiptitemFromVendorCreditOutput>(resultCount, entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts,
                      AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts_VendorCredit,
                      AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts_ItemIssueVendorCredit)]
        public async Task<PagedResultDto<ItemReceiptitemFromVendorCreditOutput>> GetNewItemReceiptForVendorCreditsOld(GetNewItemReceiptInput input, Guid? exceptId)
        {
            var vendoTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            var roundDigits = await _accountCycleRepository.GetAll().Select(t => t.RoundingDigit).FirstOrDefaultAsync();
            var userGroups = await GetUserGroupByLocation();
            var @query = from ir in _itemReceiptRepository.GetAll()
                           .Include(u => u.Vendor)
                           .Where(t => t.TransactionType == InventoryTransactionType.ItemReceiptPurchase)
                           .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId.Value))
                           .WhereIf(vendoTypeMemberIds.Any(), s => vendoTypeMemberIds.Contains(s.Vendor.VendorTypeId.Value))
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
                         on ir.Id equals j.ItemReceiptId

                         join iri in _itemReceiptItemRepository.GetAll()
                         .Include(u => u.Item)
                         .Include(u => u.Lot)
                         .AsNoTracking()
                         .WhereIf(input.Lots != null && input.Lots.Count > 0, u => input.Lots.Contains(u.LotId))
                          .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                         on ir.Id equals iri.ItemReceiptId
                         into items

                         from item in items
                         join vci in _vendorCreditItemRepository.GetAll()
                         .Where(s => s.ItemReceiptItemId != null)
                         .WhereIf(exceptId != null, s => s.VendorCreditId != exceptId)
                         .AsNoTracking()
                         on item.Id equals vci.ItemReceiptItemId
                         into returnItems

                         join ivi in _itemIssueVendorCreditItemRepository.GetAll()
                                .Where(s => s.ItemReceiptPurchaseItemId != null)
                                .WhereIf(exceptId != null, s => s.ItemIssueVendorCreditId != exceptId)
                                .AsNoTracking()
                         on item.Id equals ivi.ItemReceiptPurchaseItemId
                         into returnItems2

                         let returnQty = returnItems == null ? 0 : returnItems.Sum(s => s.Qty)
                         let returnQty2 = returnItems2 == null ? 0 : returnItems2.Sum(s => s.Qty)
                         let remainingQty = item.Qty - returnQty - returnQty2
                         let itemCount = items.Count(s => s.Qty > returnQty + returnQty2)

                         where itemCount > 0 && (returnQty + returnQty2 < item.Qty)

                         select new ItemReceiptitemFromVendorCreditOutput
                         {
                             Date = j.Date,
                             Id = item.Id,
                             ItemCode = item.Item != null ? item.Item.ItemCode : null,
                             ItemName = item.Item != null ? item.Item.ItemName : null,
                             ItemId = item != null ? item.ItemId : (Guid?)null,
                             ItemReceiptId = ir.Id,
                             LotId = item.Lot != null ? item.Lot.Id : (long?)null,
                             LotName = item.Lot != null ? item.Lot.LotName : null,
                             Qty = remainingQty,
                             ReceiptNo = j.JournalNo,
                             CreationTimeIndex = j.CreationTimeIndex,
                             ItemDateTime = item.CreationTime,
                         };

            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).ThenBy(t => t.CreationTimeIndex).ThenBy(t=>t.ItemDateTime).PageBy(input).ToListAsync();
            return new PagedResultDto<ItemReceiptitemFromVendorCreditOutput>(resultCount, entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts,
                      AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceipts_VendorCredit)]
        public async Task<PagedResultDto<ItemReceiptSummaryOutput>> GetItemReceiptForVendorCredits(GetItemReceiptInput input, Guid? exceptId)
        {

            var roundDigits = await _accountCycleRepository.GetAll().Select(t => t.RoundingDigit).FirstOrDefaultAsync();
            var userGroups = await GetUserGroupByLocation();
            var @query = from ir in _itemReceiptRepository.GetAll()
                           .Include(u => u.Vendor)
                           .Where(t => t.TransactionType == InventoryTransactionType.ItemReceiptPurchase)
                           .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId.Value))
                           .AsNoTracking()
                         join j in _journalRepository.GetAll()
                         .Include(u => u.Currency)
                         .WhereIf(!input.Filter.IsNullOrEmpty(), u =>
                               u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                               u.Reference.ToLower().Contains(input.Filter.ToLower()))
                         .WhereIf(input.Locations != null && input.Locations.Count > 0, t => input.Locations.Contains(t.LocationId))
                         .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                         // .WhereIf(input.FromDate != null && input.ToDate != null,
                         //       (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                         //.Where(u => u.Status == TransactionStatus.Publish)
                         .AsNoTracking()
                         on ir.Id equals j.ItemReceiptId

                         join iri in _itemReceiptItemRepository.GetAll().AsNoTracking()
                         //.WhereIf(input.Lots != null && input.Lots.Count > 0, u => input.Lots.Contains(u.LotId))
                         on ir.Id equals iri.ItemReceiptId
                         into items

                         from item in items
                         join vci in _vendorCreditItemRepository.GetAll()
                         .Where(s => s.ItemReceiptItemId != null)
                         .WhereIf(exceptId != null, s => s.VendorCreditId != exceptId)
                         .AsNoTracking()
                         on item.Id equals vci.ItemReceiptItemId
                         into returnItems

                         join ivi in _itemIssueVendorCreditItemRepository.GetAll()
                                .Where(s => s.ItemReceiptPurchaseItemId != null)
                                .WhereIf(exceptId != null, s => s.ItemIssueVendorCreditId != exceptId)
                                .AsNoTracking()
                         on item.Id equals ivi.ItemReceiptPurchaseItemId
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
                             Vendor = ir.Vendor,
                             Count = itemCount,
                             Qty = item.Qty - returnQty - returnQty2,
                             UnitCost = item.UnitCost,
                         }
                         into lists
                         group lists by new
                         {
                             Journal = lists.Journal,
                             ItemReceipt = lists.ItemReceipt,
                             Vendor = lists.Vendor,
                             Count = lists.Count
                         }
                         into g

                         select new ItemReceiptSummaryOutput
                         {
                             CountItems = g.Key.Count,
                             Vendor = ObjectMapper.Map<VendorSummaryOutput>(g.Key.Vendor),
                             VendorId = g.Key.ItemReceipt.VendorId.Value,
                             Id = g.Key.ItemReceipt.Id,
                             ItemReceiptNo = g.Key.Journal.JournalNo,
                             CurrencyId = g.Key.Journal.CurrencyId,
                             Currency = ObjectMapper.Map<CurrencyDetailOutput>(g.Key.Journal.Currency),
                             Date = g.Key.Journal.Date,
                             Reference = g.Key.Journal.Reference,
                             Total = g.Sum(s => Math.Round(s.UnitCost * s.Qty, roundDigits)),
                             CreationTimeIndex = g.Key.Journal.CreationTimeIndex,
                         };

            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).ThenBy(t => t.CreationTimeIndex).PageBy(input).ToListAsync();
            return new PagedResultDto<ItemReceiptSummaryOutput>(resultCount, entities);

        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendors_ItemReceipts_GetItemReceiptItems, AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_CreateItemIssueVendorCredit_CanItemReceiptPurchase)]
        public async Task<ItemReceiptSummaryOutputForItemReceiptItem> GetItemReceiptItemVendorCredits(EntityDto<Guid> input, Guid? exceptId)
        {
            var @journal = await _journalRepository
                                           .GetAll()
                                           .Include(u => u.ItemReceipt)
                                           .Include(u => u.Currency)
                                           .Include(u => u.ItemReceipt.Vendor)
                                           .Include(u => u.ItemReceipt.Vendor.ChartOfAccount)
                                           .Include(u => u.Class)
                                           .Include(u => u.Location)
                                           .Include(u => u.ItemReceipt.ShippingAddress)
                                           .Include(u => u.ItemReceipt.BillingAddress)
                                           .Where(u => u.JournalType == JournalType.ItemReceiptPurchase && u.ItemReceiptId == input.Id)
                                           .AsNoTracking()
                                           .FirstOrDefaultAsync();

            if (@journal == null || @journal.ItemReceipt == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var roundDigits = await _accountCycleRepository.GetAll().Select(t => t.RoundingDigit).FirstOrDefaultAsync();

            var query = from iri in _itemReceiptItemRepository.GetAll()
                            .Include(u => u.Item.PurchaseAccount)
                            .Include(u => u.Lot)
                            .Where(u => u.ItemReceiptId == input.Id)
                            .AsNoTracking()
                        join ji in _journalItemRepository.GetAll()
                            .Include(u => u.Account)
                            .Where(u => u.JournalId == journal.Id)
                            .AsNoTracking()
                        on iri.Id equals ji.Identifier

                        join returnItem in _vendorCreditItemRepository.GetAll()
                            .Where(s => s.ItemReceiptItemId != null)
                            .WhereIf(exceptId != null, s => s.VendorCreditId != exceptId)
                            .AsNoTracking()
                        on iri.Id equals returnItem.ItemReceiptItemId
                        into returnItems

                        join returnItem2 in _itemIssueVendorCreditItemRepository.GetAll()
                           .Where(s => s.ItemReceiptPurchaseItemId != null)
                           .WhereIf(exceptId != null, s => s.ItemIssueVendorCreditId != exceptId)
                           .AsNoTracking()
                        on iri.Id equals returnItem2.ItemReceiptPurchaseItemId
                        into returnItems2

                        let returnQty = returnItems == null ? 0 : returnItems.Sum(s => s.Qty)
                        let returnQty2 = returnItems2 == null ? 0 : returnItems2.Sum(s => s.Qty)
                        let remainingQty = iri.Qty - returnQty - returnQty2

                        where returnQty + returnQty2 < iri.Qty

                        group iri by new
                        {
                            IItem = iri,
                            Qty = remainingQty,
                            Total = Math.Round(iri.UnitCost * remainingQty, roundDigits),
                            JItem = ji,                            
                            Item = iri.Item,
                            Account = ji.Account,

                        }
                        into g
                        orderby g.Key.IItem.CreationTime
                        select
                        new ItemReceiptItemSummaryOutput()
                        {
                            Id = g.Key.IItem.Id,
                            Item = ObjectMapper.Map<ItemSummaryOutput>(g.Key.Item),
                            ItemId = g.Key.IItem.ItemId,
                            InventoryAccountId = g.Key.JItem.AccountId,
                            InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(g.Key.Account),
                            Description = g.Key.IItem.Description,
                            DiscountRate = g.Key.IItem.DiscountRate,
                            Qty = g.Key.Qty,
                            Total = g.Key.Total,
                            UnitCost = g.Key.IItem.UnitCost,
                            ToLotId = g.Key.IItem.LotId,
                            ToLotDetail = ObjectMapper.Map<LotSummaryOutput>(g.Key.IItem.Lot),
                        };


            var itemReceiptItems = await query.ToListAsync();

            var clearanceAccount = await _journalItemRepository.GetAll()
                                        .Include(s => s.Account)
                                        .Where(s => s.JournalId == journal.Id && s.Identifier == null)
                                        .AsNoTracking()
                                        .Select(s => s.Account)
                                        .FirstOrDefaultAsync();


            var result = ObjectMapper.Map<ItemReceiptSummaryOutputForItemReceiptItem>(journal.ItemReceipt);
            result.Total = Math.Round(itemReceiptItems.Sum(s => s.Total), roundDigits);
            result.ReceiveNo = journal.JournalNo;
            result.Date = journal.Date;
            result.Reference = journal.Reference;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.LocationId = journal.LocationId.Value;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.ItemReceiptItems = itemReceiptItems;
            result.ClearanceAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(clearanceAccount);
            result.ClearanceAccountId = clearanceAccount.Id;
            result.Vendor = ObjectMapper.Map<VendorSummaryDetailOutput>(journal.ItemReceipt.Vendor);
            result.VendorId = journal.ItemReceipt.VendorId.Value;
            result.Memo = journal.Memo;
            return result;
        }

        #endregion

    }
}
