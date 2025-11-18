using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using CorarlERP.AccountCycles;
using CorarlERP.Authorization;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.Customers;
using CorarlERP.Invoices;
using CorarlERP.IssueKitchenOrders.Dto;
using CorarlERP.ItemIssues;
using CorarlERP.ItemIssues.Dto;
using CorarlERP.ItemIssueVendorCredits;
using CorarlERP.ItemReceiptCustomerCredits;
using CorarlERP.ItemReceipts;
using CorarlERP.Items;
using CorarlERP.Locations.Dto;
using CorarlERP.Reports;
using CorarlERP.SaleOrders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using CorarlERP.ChartOfAccounts;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Items.Dto;
using CorarlERP.Journals;
using CorarlERP.SaleOrders.Dto;
using Microsoft.EntityFrameworkCore;
using static CorarlERP.enumStatus.EnumStatus;
using System.Linq.Dynamic.Core;
using CorarlERP.Taxes.Dto;
using Microsoft.EntityFrameworkCore.Internal;
using CorarlERP.Vendors;
using CorarlERP.TransferOrders;
using CorarlERP.Inventories;
using CorarlERP.Productions;
using CorarlERP.AutoSequences;
using CorarlERP.Authorization.Users;
using CorarlERP.Lots.Dto;
using CorarlERP.Lots;
using CorarlERP.UserGroups;
using CorarlERP.CustomerCredits;
using CorarlERP.Locks;
using CorarlERP.InventoryTransactionItems;
using CorarlERP.InventoryCalculationItems;
using CorarlERP.BatchNos;
using CorarlERP.JournalTransactionTypes;

namespace CorarlERP.IssueKitchenOrders
{
    public class IssueKitchenOrderAppService : ReportBaseClass, IIssueKitchenOrderAppService
    {
        private readonly IRepository<ItemReceiptItem, Guid> _itemReceiptItemRepository;
        private readonly IRepository<ItemReceipt, Guid> _itemReceiptRepository;
        private readonly IRepository<ItemReceiptItemCustomerCredit, Guid> _itemReceiptCustomerCreditItemRepository;
        private readonly IRepository<ItemReceiptCustomerCredit, Guid> _itemReceiptCustomerCreditRepository;
        private readonly IRepository<AccountCycle, long> _accountCycleRepository;
        private readonly IItemManager _itemManager;
        private readonly IRepository<Item, Guid> _itemRepository; 
        private readonly IRepository<ItemIssueItem, Guid> _itemIssueItemRepository;
        private readonly IJournalManager _journalManager;
        private readonly IRepository<Journal, Guid> _journalRepository;   
        private readonly IRepository<JournalItem, Guid> _journalItemRepository;     
        private readonly ICorarlRepository<InventoryTransactionItem, Guid> _inventoryTransactionItemRepository;

        public IssueKitchenOrderAppService(
            ICorarlRepository<InventoryTransactionItem, Guid> inventoryTransactionItemRepository,       
            IJournalManager journalManager,           
            ItemManager itemManager,          
            IRepository<Item, Guid> itemRepository,           
            IRepository<Journal, Guid> journalRepository,
            IRepository<JournalItem, Guid> journalItemRepository,         
            IRepository<ItemIssueItem, Guid> itemIssueItemRepository,          
            IRepository<ItemReceiptItem, Guid> itemReceiptItemRepository,
            IRepository<ItemReceipt, Guid> itemReceiptRepository,
            IRepository<ItemReceiptItemCustomerCredit, Guid> itemReceiptCustomerCreditItemRepository,          
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            IRepository<Locations.Location, long> locationRepository,
            IRepository<AccountCycle, long> accountCycleRepository,        
            IRepository<ItemReceiptCustomerCredit, Guid> itemReceiptCustomerCreditRepository,          
            AppFolders appFolders
        ) : base(accountCycleRepository, appFolders, userGroupMemberRepository, locationRepository)
        {
            _itemReceiptCustomerCreditRepository = itemReceiptCustomerCreditRepository;          
            _accountCycleRepository = accountCycleRepository;         
            _journalManager = journalManager;
            _journalManager.SetJournalType(JournalType.ItemIssueKitchenOrder);
            _journalRepository = journalRepository;         
            _journalItemRepository = journalItemRepository;       
            _itemIssueItemRepository = itemIssueItemRepository;         
            _itemRepository = itemRepository;
            _itemManager = itemManager;          
            _itemReceiptItemRepository = itemReceiptItemRepository;
            _itemReceiptRepository = itemReceiptRepository;
            _itemReceiptCustomerCreditItemRepository = itemReceiptCustomerCreditItemRepository;
            _itemRepository = itemRepository;          
            _inventoryTransactionItemRepository = inventoryTransactionItemRepository;
         
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customers_ItemIssues_GetDetail,
                     AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_ViewDetail)]
        public async Task<GetDetailIssueKitchenOrderDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            await TurnOffTenantFilterIfDebug();

            var @journal = await _journalRepository
                                .GetAll()
                                .Include(u => u.ItemIssue.KitchenOrder)
                                .Include(u => u.ItemIssue.TransactionTypeSale)
                                .Include(u => u.Location)
                                .Include(u => u.Class)
                                .Include(u => u.Currency)
                                .Include(u => u.ItemIssue.Customer)
                                .Include(u => u.ItemIssue.ShippingAddress)
                                .Include(u => u.ItemIssue.BillingAddress)  
                                .Include(u=>u.JournalTransactionType)
                                .AsNoTracking()
                                .Where(u => u.JournalType == JournalType.ItemIssueKitchenOrder && u.ItemIssueId == input.Id)
                                .FirstOrDefaultAsync();

            if (@journal == null || @journal.ItemIssue == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var itemIssueItemCOGSs = await _itemIssueItemRepository.GetAll()
               .Include(u => u.Item)
               .Include(u => u.Lot)                       
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
           
            var itemIssueItems = await (from issueItem in _itemIssueItemRepository.GetAll()
                                                        .Include(u => u.Item.InventoryAccount)                                                     
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
                                                                                      .Where(s => s.JournalType == JournalType.ItemIssueKitchenOrder)
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
                                            SaleOrderNumber = issueItem.SaleOrderItem.SaleOrder.OrderNumber,
                                            SaleOrderReference = issueItem.SaleOrderItem.SaleOrder.Reference,
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
                                            InventoryAccountId = jItem.AccountId,
                                            InventoryAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                                            Description = issueItem.Description,
                                            DiscountRate = issueItem.DiscountRate,
                                            Qty = issueItem.Qty,
                                            Total = c != null ? Math.Abs(c.LineCost) : issueItem.Total,
                                            UnitCost = c != null ? c.UnitCost : issueItem.UnitCost,
                                            SaleOrderItemId = issueItem.SaleOrderItemId,
                                            PurchaseAccountId = itemIssueItemCOGSs.Where(r => r.Id == issueItem.Id).Select(r => r.PurchaseAccountId).FirstOrDefault(),
                                            PurchaseAccount = itemIssueItemCOGSs.Where(r => r.Id == issueItem.Id).Select(r => r.PurchaseAccount).FirstOrDefault(),
                                            UseBatchNo = issueItem.Item.UseBatchNo,
                                            TrackSerial = issueItem.Item.TrackSerial,
                                            TrackExpiration = issueItem.Item.TrackExpiration
                                        }).ToListAsync();

            var result = ObjectMapper.Map<GetDetailIssueKitchenOrderDetailOutput>(journal.ItemIssue);
            result.IssueNo = journal.JournalNo;
            result.Date = journal.Date;
            result.OrderDate = journal.ItemIssue.KitchenOrder.OrderDate;
            result.OrderNumber = journal.ItemIssue.KitchenOrder.OrderNumber;
            result.KitchenOrderId = journal.ItemIssue.KitchenOrderId;
            result.MulitCurrencyId = result.MulitCurrencyId;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.Reference = journal.Reference;
            result.ItemIssueItems = itemIssueItems;
            result.Memo = journal.Memo;
            result.StatusCode = journal.Status;         
            result.TransactionTypeSaleId = journal.ItemIssue.TransactionTypeSaleId;
            result.StatusName = journal.Status.ToString();
            result.LocationId = journal.LocationId.Value;
            result.Location = ObjectMapper.Map<LocationSummaryOutput>(journal.Location);       
            result.Total = itemIssueItems.Sum(s => s.Total);
            result.TransactionTypeName = journal.JournalTransactionType?.Name;

            return result;
        }


    }
}
