using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Authorization.Users;
using CorarlERP.AutoSequences;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Currencies;
using CorarlERP.Customers;
using CorarlERP.FileStorages;
using CorarlERP.FileUploads;
using CorarlERP.Galleries;
using CorarlERP.Inventories;
using CorarlERP.InventoryCalculationItems;
using CorarlERP.ItemIssues;
using CorarlERP.ItemIssues.Dto;
using CorarlERP.Items;
using CorarlERP.Items.Dto;
using CorarlERP.Journals;
using CorarlERP.JournalTransactionTypes;
using CorarlERP.KitchenOrders.Dto;
using CorarlERP.Locks;
using CorarlERP.Lots;
using CorarlERP.MultiTenancy;
using CorarlERP.Reports;
using CorarlERP.Taxes;
using CorarlERP.Url;
using CorarlERP.UserGroups;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CorarlERP.enumStatus.EnumStatus;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using static CorarlERP.KitchenOrders.Dto.GetDetailKitchenOrder;
using CorarlERP.Currencies.Dto;
using CorarlERP.Customers.Dto;
using CorarlERP.Taxes.Dto;
using CorarlERP.Lots.Dto;
using Microsoft.AspNetCore.Mvc;
using CorarlERP.Common.Dto;
using CorarlERP.Authorization;
using Abp.Authorization;
using CorarlERP.Dto;
using CorarlERP.Features;
using OfficeOpenXml;
using CorarlERP.Reports.Dto;
using CorarlERP.ReportTemplates;
using CorarlERP.AccountCycles;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Locations;
using CorarlERP.Addresses;
using CorarlERP.InventoryTransactionItems;
using CorarlERP.Inventories.Data;

namespace CorarlERP.KitchenOrders
{
    public class KitchenOrderAppService : ReportBaseClass, IKitchenOrderAppService
    {
        private readonly ICorarlRepository<Journal, Guid> _journalRepository;
        private readonly ICorarlRepository<KitchenOrder, Guid> _OrderRepository;
        private readonly ICorarlRepository<KitchenOrderItem, Guid> _OrderItemRepository;
        private readonly IRepository<Item, Guid> _itemRepository;
        private readonly IRepository<Customer, Guid> _customerRepository;
        private readonly IRepository<Tax, long> _taxRepository;
        private readonly ICorarlRepository<ItemIssueItem, Guid> _itemIssueItemRepository;
        private readonly ICorarlRepository<ItemIssue, Guid> _itemIssueRepository;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;
        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly AppFolders _appFolders;
        private readonly string _baseUrl;
        private readonly ITenantManager _tenantManager;
        private readonly IRepository<User, long> _userRepository;
        private readonly IFileUploadManager _fileUploadManager;
        private readonly IFileStorageManager _fileStorageManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ItemIssueItemManager _itemIssueItemManager;
        private readonly IRepository<Lock, long> _lockRepository;
        private readonly JournalTransactionTypeManager _journalTransactionTypeManager;
        private readonly ICorarlRepository<Lot, long> _lotRepository;
        private readonly IInventoryManager _inventoryManager;
        private readonly JournalManager _journalManager;
        private readonly ItemIssueManager _itemIssueManager;
        private readonly JournalItemManager _journalItemManager;
        private readonly InventoryCalculationItemManager _inventoryCalculationItemManager;
        private readonly ICorarlRepository<KitchenOrderItemAndBOMItem, Guid> _kitchenOrderItemAndBOMItemRepository;
        private readonly ICorarlRepository<JournalItem, Guid> _journalItemRepository;
        private readonly IRepository<Location, long> _locationRepository;
        private readonly IRepository<Classes.Class, long> _classRepository;
        private readonly IRepository<Boms.Bom, Guid> _bomRepository;
        private readonly IRepository<Boms.BomItem, Guid> _bomItemRepository;
        private readonly IRepository<ItemLot, Guid> _itemLotRepository;
        private readonly ICorarlRepository<InventoryTransactionItem, Guid> _inventoryTransactionItemRepository;
        public KitchenOrderAppService(
            ITenantManager tenantManager,
            IRepository<Gallery, Guid> galleryRepository,
            IFileUploadManager fileUploadManager,
            IFileStorageManager fileStorageManager,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<User, long> userRepository,
            ICurrencyManager currencyManager,
            ITaxManager taxManager,
            ICorarlRepository<KitchenOrder, Guid> orderRepository,
            ICorarlRepository<KitchenOrderItem, Guid> orderItemRepository,
            IRepository<Item, Guid> itemRepository,
            IRepository<Customer, Guid> customerRepository,
            IRepository<Currency, long> currencyRepository,
            ICorarlRepository<Journal, Guid> journalRepository,
            ICorarlRepository<ItemIssueItem, Guid> itemIssueItemRepository,
            IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
            IRepository<Tax, long> taxRepository,
            IRepository<AutoSequence, Guid> autoSequenceRepository,
            IRepository<Locations.Location, long> locationRepository,
            IAutoSequenceManager autoSequenceManager,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            AppFolders appFolders,
            IRepository<AccountCycles.AccountCycle, long> accountCyclesRepository,
            ItemIssueItemManager itemIssueItemManager,
            IRepository<Lock, long> lockRepository,
            JournalTransactionTypeManager journalTransactionTypeManager,
            ICorarlRepository<Lot, long> lotRepository,
            IInventoryManager inventoryManager,
            JournalManager journalManager,
            ItemIssueManager itemIssueManager,
            JournalItemManager journalItemManager,
            InventoryCalculationItemManager inventoryCalculationItemManager,
            ICorarlRepository<KitchenOrderItemAndBOMItem, Guid> kitchenOrderItemAndBOMItemRepository,
            ICorarlRepository<JournalItem, Guid> journalItemRepository,
            ICorarlRepository<ItemIssue, Guid> itemIssueRepository,
            IRepository<Classes.Class, long> classRepository,
            IRepository<Boms.Bom, Guid> bomRepositry,
            IRepository<Boms.BomItem, Guid> bomItemRepositry,
            IRepository<ItemLot, Guid> itemLotRepository,
            ICorarlRepository<InventoryTransactionItem, Guid> inventoryTransactionItemRepository
        ) : base(accountCyclesRepository, appFolders, userGroupMemberRepository, locationRepository)
        {

            _OrderRepository = orderRepository;
            _OrderItemRepository = orderItemRepository;
            _kitchenOrderItemAndBOMItemRepository = kitchenOrderItemAndBOMItemRepository;
            _itemRepository = itemRepository;
            _customerRepository = customerRepository;
            _taxRepository = taxRepository;
            _journalRepository = journalRepository;
            _chartOfAccountRepository = chartOfAccountRepository;
            _itemIssueItemRepository = itemIssueItemRepository;
            _autoSequenceManager = autoSequenceManager;
            _autoSequenceRepository = autoSequenceRepository;
            _appFolders = appFolders;
            _tenantManager = tenantManager;
            _baseUrl = (IocManager.Instance.Resolve<IWebUrlService>()).GetServerRootAddress().EnsureEndsWith('/');
            _userRepository = userRepository;
            _fileUploadManager = fileUploadManager;
            _fileStorageManager = fileStorageManager;
            _unitOfWorkManager = unitOfWorkManager;
            _itemIssueItemManager = itemIssueItemManager;
            _lockRepository = lockRepository;
            _journalTransactionTypeManager = journalTransactionTypeManager;
            _lotRepository = lotRepository;
            _inventoryManager = inventoryManager;
            _journalManager = journalManager;
            _itemIssueManager = itemIssueManager;
            _journalItemManager = journalItemManager;
            _inventoryCalculationItemManager = inventoryCalculationItemManager;
            _journalItemRepository = journalItemRepository;
            _itemIssueRepository = itemIssueRepository;
            _locationRepository = locationRepository;
            _classRepository = classRepository;
            _bomRepository = bomRepositry;
            _bomItemRepository = bomItemRepositry;
            _itemLotRepository = itemLotRepository;
            _inventoryTransactionItemRepository = inventoryTransactionItemRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_KitchenOrders_Create)]
        public async Task<NullableIdDto<Guid>> Create(CreateKitchenOrderInput input)
        {

            if (input.MultiCurrencyId == null || input.MultiCurrencyId == 0 || input.CurrencyId == input.MultiCurrencyId)
            {
                input.MultiCurrencyId = input.CurrencyId;
                input.MultiCurrencyTotal = input.Total;
                input.MultiCurrencySubTotal = input.SubTotal;
                input.MultiCurrencyTax = input.Tax;
            }
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.KitchenOrder);
            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.OrderNumber = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }

            var tenantId = AbpSession.TenantId.Value;
            var userId = AbpSession.UserId.Value;
            var kitchenOrderItems = new List<KitchenOrderItem>();
            var subkitchenOrderItems = new List<KitchenOrderItemAndBOMItem>();
            var entities = KitchenOrder.Create(tenantId, userId, input.CustomerId, input.ShippingAddress, input.BillingAddress, input.SameAsShippingAddress,
                input.Reference, input.CurrencyId, input.OrderNumber, input.OrderDate, input.Memo, input.Tax, input.Total, input.SubTotal, input.Status,
                input.LocationId, input.MultiCurrencyId, input.MultiCurrencySubTotal, input.MultiCurrencyTotal, input.MultiCurrencyTax, input.ClassId.Value);
            var listDublication = new List<string>();
            foreach (var ki in input.OrderItems)
            {
                if (ki.BOMId == null)
                    throw new UserFriendlyException(L("BOMIsRequired"));

                var uniqueKey = ki.ItemId.ToString() + ki.BOMId.ToString();
                // Check if the key already exists in the list
                if (listDublication.Contains(uniqueKey))
                {
                    throw new UserFriendlyException(L("DublicationBom", ki.Item.ItemName));
                }
                // Add the unique key to the list
                listDublication.Add(uniqueKey);
                var kitchenOrderItem = KitchenOrderItem.Create(tenantId, userId, ki.ItemId,
                    ki.TaxId, ki.TaxRate, ki.Description, ki.Qty, ki.UnitCost, ki.DiscountRate, ki.Total, ki.MultiCurrencyUnitCost,
                    ki.MultiCurrencyTotal, ki.BOMId, entities.Id);
                kitchenOrderItems.Add(kitchenOrderItem);
                foreach (var i in ki.KitchenOrderItemAndBOMItem)
                {
                    if (i.LotId == null) throw new UserFriendlyException (L("LotIsRequired"));
                    var kitchenOrderItemAndBomItem = KitchenOrderItemAndBOMItem.Create(tenantId, userId, i.ItemId, i.Qty, i.BomItemId, kitchenOrderItem.Id, i.LotId.Value, i.TaxId, i.TaxRate, i.TotalQty);
                    subkitchenOrderItems.Add(kitchenOrderItemAndBomItem);
                }
            }
            var inventoryAccount = await _itemRepository.GetAll()
                                        .Where(t => subkitchenOrderItems.Any(x => x.ItemId == t.Id))
                                        .Where(t => t.InventoryAccountId != null && t.ItemType.DisplayInventoryAccount)
                                        .AsNoTracking()
                                        .Select(u => new { u.Id, u.SalePrice, u.InventoryAccountId, u.PurchaseAccountId, u.ItemCode, u.ItemName })
                                        .ToDictionaryAsync(u => u.Id);

            var itemLists = subkitchenOrderItems.Where(d => d.ItemId != null && inventoryAccount.ContainsKey(d.ItemId))
                            .Select(t => new CreateOrUpdateItemIssueItemInput
                            {
                                UnitCost = inventoryAccount[t.ItemId].SalePrice != null ? inventoryAccount[t.ItemId].SalePrice.Value : 0,
                                Total = t.TotalQty * inventoryAccount[t.ItemId].SalePrice != null ? inventoryAccount[t.ItemId].SalePrice.Value : 0,
                                InventoryAccountId = inventoryAccount[t.ItemId].InventoryAccountId.Value,
                                LotId = t.LotId,
                                ItemId = t.ItemId,
                                Description = "",
                                DiscountRate = 0,
                                Qty = t.TotalQty,
                                PurchaseAccountId = inventoryAccount[t.ItemId].PurchaseAccountId.Value,
                                InvoiceItemId = t.Id,
                                Item = new ItemSummaryDetailOutput
                                {
                                    Id = t.ItemId,
                                    ItemCode = inventoryAccount[t.ItemId].ItemCode,
                                    ItemName = inventoryAccount[t.ItemId].ItemName,
                                },
                                KitchenOrderItemAndBOMItemId = t.Id,
                            }).ToList();
            var itemIssueInput = new CreateItemIssueFromKitchenOrderInput()
            {
                IsConfirm = input.IsConfirm,
                TransactionTypeId = input.TransactionTypeId,
                BillingAddress = input.BillingAddress,
                ShippingAddress = input.ShippingAddress,
                Status = input.Status,
                ClassId = input.ClassId,
                CurrencyId = input.CurrencyId,
                CustomerId = input.CustomerId,
                Date = input.OrderDate,
                IssueNo = input.OrderNumber,
                LocationId = input.LocationId.Value,
                Memo = input.Memo,
                ReceiveFrom = ReceiveFrom.Invoice,
                Reference = input.Reference,
                SameAsShippingAddress = input.SameAsShippingAddress,
                Total = input.Total,
                Items = itemLists,
                KitchenOrderId = entities.Id,
                PermissionLockId = input.PermissionLockId,
            };
            await _OrderRepository.BulkInsertAsync(entities);
            await _OrderItemRepository.BulkInsertAsync(kitchenOrderItems);
            await _kitchenOrderItemAndBOMItemRepository.BulkInsertAsync(subkitchenOrderItems);
            await ItemIssueSave(itemIssueInput);
            return new NullableIdDto<Guid>() { Id = entities.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_KitchenOrders_GetList)]
        [HttpPost]
        public async Task<PagedResultDto<GetListKitchenOrderOutput>> GetList(GetListKitchenOrderInput input)
        {
            var userGroups = await GetUserGroupByLocation();
            var query = from oi in _OrderItemRepository.GetAll().AsNoTracking()
                        .WhereIf(input.ItemIds != null && input.ItemIds.Count() > 0, s => input.ItemIds.Contains(s.ItemId))
                        join o in _OrderRepository.GetAll().AsNoTracking()
                         .Include(u => u.Location)
                         .Include(u => u.Customer)
                         .WhereIf(input.ClassIds != null && input.ClassIds.Any(), s => input.ClassIds.Contains(s.ClassId))
                         .WhereIf(userGroups != null && userGroups.Any(), t => userGroups.Contains(t.LocationId.Value))
                         .WhereIf(input.Status != null && input.Status.Any(), t => input.Status.Contains(t.Status))
                         .WhereIf(input.Users != null && input.Users.Any(), u => input.Users.Contains(u.CreatorUserId))
                         .WhereIf(input.Locations != null && input.Locations.Any(), u => input.Locations.Contains(u.LocationId.Value))
                         .WhereIf(input.FromDate != null && input.ToDate != null,
                         (u => (u.OrderDate.Date) >= (input.FromDate.Date) && (u.OrderDate.Date) <= (input.ToDate.Date)))
                         .WhereIf(!input.Filter.IsNullOrEmpty(),
                            u => u.OrderNumber.ToLower().Contains(input.Filter.ToLower()) ||
                            u.Reference.ToLower().Contains(input.Filter.ToLower()))
                        on oi.KitchenOrderId equals o.Id
                        group o by new { customer = o.Customer, location = o.Location, order = o } into s
                        select new GetListKitchenOrderOutput
                        {
                            LocationId = s.Key.order.LocationId,
                            CustomerName = s.Key.customer != null ? s.Key.customer.CustomerName : null,
                            CustomerId = s.Key.order.CustomerId,
                            LocationName = s.Key.location.LocationName,
                            OrderDate = s.Key.order.OrderDate,
                            OrderNumber = s.Key.order.OrderNumber,
                            Reference = s.Key.order.Reference,
                            Status = s.Key.order.Status,
                            SubTotal = s.Key.order.SubTotal,
                            Total = s.Key.order.Total,
                            Id = s.Key.order.Id,
                        };

            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            return new PagedResultDto<GetListKitchenOrderOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_KitchenOrders_Detail)]
        public async Task<KitchendOrderDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            var entities = await _OrderRepository.GetAll()
                .Include(u => u.Customer)
                .Include(u => u.Currency)
                .Include(u => u.MultiCurrency)
                .Include(u => u.Class)
                .Include(u => u.Location)
                .Include(u => u.BillingAddress)
                .Include(u => u.ShippingAddress)
                .Include(u => u.CreatorUser)
                .Where(s => s.Id == input.Id)
                .AsNoTracking()
                .Select(s => new KitchendOrderDetailOutput
                {
                    OrderNumber = s.OrderNumber,
                    OrderDate = s.OrderDate,
                    MultiCurrencyTotal = s.MultiCurrencyTotal,
                    ShippingAddress = s.ShippingAddress,
                    BillingAddress = s.BillingAddress,
                    CurrencyId = s.CurrencyId,
                    Currency = ObjectMapper.Map<CurrencyDetailOutput>(s.Currency),
                    Customer = ObjectMapper.Map<CustomerSummaryOutput>(s.Customer),
                    CustomerId = s.CustomerId,
                    Id = s.Id,
                    Location = s.Location.LocationName,
                    LocationId = s.LocationId,
                    Memo = s.Memo,
                    MultiCurrency = ObjectMapper.Map<CurrencyDetailOutput>(s.MultiCurrency),
                    MultiCurrencyId = s.MultiCurrencyId,
                    MultiCurrencySubTotal = s.MultiCurrencySubTotal,
                    MultiCurrencyTax = s.MultiCurrencyTax,
                    Reference = s.Reference,
                    SameAsShippingAddress = s.SameAsShippingAddress,
                    Status = s.Status,
                    StatusCode = s.Status.ToString(),
                    SubTotal = s.SubTotal,
                    Tax = s.Tax,
                    Total = s.Total,
                    UserName = s.CreatorUser.Name,
                    ClassId = s.ClassId,
                    ClassName = s.Class.ClassName,

                }).FirstOrDefaultAsync();

            var orderitems = await _OrderItemRepository.GetAll()
                        .AsNoTracking()
                        .Include(u => u.Item)
                        .Include(u => u.KitchenOrder)
                        .Include(u => u.Tax)
                        .Include(u => u.Bom)
                        .Where(s => s.KitchenOrderId == entities.Id)
                        .Select(s => new CreateOrUpdateKitchenOrderItemInput
                        {
                            BOMId = s.BOMId,
                            Description = s.Description,
                            DiscountRate = s.DiscountRate,
                            ItemId = s.ItemId,
                            Id = s.Id,
                            Item = ObjectMapper.Map<ItemSummaryOutput>(s.Item),
                            MultiCurrencyTotal = s.MultiCurrencyTotal,
                            MultiCurrencyUnitCost = s.MultiCurrencyUnitCost,
                            Qty = s.Qty,
                            Tax = ObjectMapper.Map<TaxDetailOutput>(s.Tax),
                            TaxId = s.TaxId,
                            TaxRate = s.TaxRate,
                            Total = s.Total,
                            UnitCost = s.UnitCost,
                            BomName = s.Bom.Name,


                        }).ToListAsync();

            var orItemIds = orderitems.Select(s => s.Id).ToList();
            var orderItemAndBOMItems = await _kitchenOrderItemAndBOMItemRepository.GetAll()
                                      .AsNoTracking()
                                      .Include(u => u.Item.InventoryAccount)
                                      .Include(u => u.BomItem)
                                      .Include(u => u.Lot)
                                      .Include(u => u.Tax)
                                      .Where(s => orItemIds.Contains(s.KitchenOrderItemId)).
                                      Select(s => new CreateKitchenOrderItemAndBOMItemInput
                                      {
                                          BomItem = ObjectMapper.Map<GetBomItemDetail>(s.BomItem),
                                          BomItemId = s.BomItemId,
                                          Item = ObjectMapper.Map<ItemSummaryOutput>(s.Item),
                                          ItemId = s.ItemId,
                                          KitchenOrderItemId = s.KitchenOrderItemId,
                                          Lot = ObjectMapper.Map<LotSummaryOutput>(s.Lot),
                                          LotId = s.LotId,
                                          Qty = s.Qty,
                                          Tax = ObjectMapper.Map<TaxDetailOutput>(s.Tax),
                                          TaxId = s.TaxId,
                                          TaxRate = s.TaxRate,
                                          TotalQty = s.TotalQty,
                                          Id = s.Id,
                                      }).ToListAsync();

            entities.OrderItems = orderitems.Select(s => new CreateOrUpdateKitchenOrderItemInput
            {
                BOMId = s.BOMId,
                Description = s.Description,
                DiscountRate = s.DiscountRate,
                ItemId = s.ItemId,
                Id = s.Id,
                BomName = s.BomName,
                Item = ObjectMapper.Map<ItemSummaryOutput>(s.Item),
                MultiCurrencyTotal = s.MultiCurrencyTotal,
                MultiCurrencyUnitCost = s.MultiCurrencyUnitCost,
                Qty = s.Qty,
                Tax = ObjectMapper.Map<TaxDetailOutput>(s.Tax),
                TaxId = s.TaxId,
                TaxRate = s.TaxRate,
                Total = s.Total,
                UnitCost = s.UnitCost,
                KitchenOrderItemAndBOMItem = orderItemAndBOMItems.Where(or => or.KitchenOrderItemId == s.Id).ToList()
            }).ToList();
            return entities;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_KitchenOrders_Update)]
        public async Task<NullableIdDto<Guid>> Update(UpdateKitchenOrderInput input)
        {

            if (input.MultiCurrencyId == null || input.MultiCurrencyId == 0 || input.CurrencyId == input.MultiCurrencyId)
            {
                input.MultiCurrencyId = input.CurrencyId;
                input.MultiCurrencyTotal = input.Total;
                input.MultiCurrencySubTotal = input.SubTotal;
                input.MultiCurrencyTax = input.Tax;
            }
            var entities = await _OrderRepository.GetAll().Where(s => s.Id == input.Id).AsNoTracking().FirstOrDefaultAsync();
            if (entities == null) throw new UserFriendlyException(L("RecordNotFound"));
            var tenantId = AbpSession.TenantId.Value;
            var userId = AbpSession.UserId.Value;
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.KitchenOrder);
            if (auto.CustomFormat == true)
            {
                input.OrderNumber = entities.OrderNumber;
            }
            entities.Update(userId, input.CustomerId, input.Reference, input.CurrencyId, input.OrderNumber, input.OrderDate,
                input.Memo, input.ShippingAddress, input.BillingAddress, input.SameAsShippingAddress, input.SubTotal,
                input.Tax, input.Total, input.Status, input.LocationId, input.MultiCurrencyId, input.MultiCurrencySubTotal,
                input.MultiCurrencyTotal, input.MultiCurrencyTax, input.ClassId.Value);

            var kitchenOrderItems = await _OrderItemRepository.GetAll().AsNoTracking().Where(s => s.KitchenOrderId == entities.Id).ToListAsync();
            var subKitchenOrderItems = await _kitchenOrderItemAndBOMItemRepository.GetAll().AsNoTracking().Where(s => s.KitchenOrderItem.KitchenOrderId == entities.Id).ToListAsync();
            var updateKitchenOrderItems = new List<KitchenOrderItem>();
            var createKitchenOrderItems = new List<KitchenOrderItem>();
            var updateSubBOMItems = new List<KitchenOrderItemAndBOMItem>();
            var createSubBOMItems = new List<KitchenOrderItemAndBOMItem>();
            var listDublication = new List<string>();
            foreach (var s in input.OrderItems)
            {
                if (s.BOMId == null)
                    throw new UserFriendlyException(L("BOMIsRequired"));

                var uniqueKey = s.ItemId.ToString() + s.BOMId.ToString();
                // Check if the key already exists in the list
                if (listDublication.Contains(uniqueKey))
                {
                    throw new UserFriendlyException(L("DublicationBom", s.Item.ItemName));
                }
                // Add the unique key to the list
                listDublication.Add(uniqueKey);

                if (s.Id != null)
                {
                    var @subItem = kitchenOrderItems.FirstOrDefault(u => u.Id == s.Id);
                    if (@subItem != null)
                    {
                        @subItem.Update(userId, s.ItemId, s.TaxId, s.TaxRate, s.Description, s.Qty, s.UnitCost, s.DiscountRate, s.Total, s.MultiCurrencyUnitCost, s.MultiCurrencyTotal, s.BOMId, entities.Id);
                        updateKitchenOrderItems.Add(subItem);
                    }
                }
                else if (s.Id == null)
                {
                    var @subItem = KitchenOrderItem.Create(tenantId, userId, s.ItemId, s.TaxId, s.TaxRate, s.Description, s.Qty, s.UnitCost, s.DiscountRate, s.Total, s.MultiCurrencyUnitCost, s.MultiCurrencyTotal, s.BOMId, entities.Id);
                    s.Id = subItem.Id;
                    createKitchenOrderItems.Add(subItem);
                }

                foreach (var i in s.KitchenOrderItemAndBOMItem)
                {
                    if (i.LotId == null) throw new UserFriendlyException(L("LotIsRequired"));
                    if (i.Id != null)
                    {
                        var subBomItem = subKitchenOrderItems.Where(u => u.Id == i.Id).FirstOrDefault();
                        if (subBomItem != null)
                        {
                            subBomItem.Update(userId, i.ItemId, i.Qty, i.BomItemId, s.Id.Value, i.LotId.Value, i.TaxId, i.TaxRate, i.TotalQty);
                            updateSubBOMItems.Add(subBomItem);
                        }
                    }
                    else if (i.Id == null)
                    {
                        var subBomItem = KitchenOrderItemAndBOMItem.Create(tenantId, userId, i.ItemId, i.Qty, i.BomItemId, s.Id.Value, i.LotId.Value, i.TaxId, i.TaxRate, i.TotalQty);
                        createSubBOMItems.Add(subBomItem);
                    }
                }

            }
            var deleteOrderItems = kitchenOrderItems.Where(u => !input.OrderItems.Any(i => i.Id != null && i.Id == u.Id)).ToList();

            var unoinSubItems = createSubBOMItems.Union(updateSubBOMItems);
            var deleteSubBOMItems = subKitchenOrderItems.Where(u => !unoinSubItems.Any(s => s.Id == u.Id)).ToList();


            var subkitchenOrderItems = createSubBOMItems.Union(updateSubBOMItems);

            var inventoryAccount = await _itemRepository.GetAll()
                                       .Where(t => subkitchenOrderItems.Any(x => x.ItemId == t.Id))
                                       .Where(t => t.InventoryAccountId != null && t.ItemType.DisplayInventoryAccount)
                                       .AsNoTracking()
                                       .Select(u => new { u.Id, u.SalePrice, u.InventoryAccountId, u.PurchaseAccountId, u.ItemCode, u.ItemName })
                                       .ToDictionaryAsync(u => u.Id);

            var itemLists = subkitchenOrderItems.Where(d => d.ItemId != null && inventoryAccount.ContainsKey(d.ItemId))
                          .Select(t => new CreateOrUpdateItemIssueItemInput
                          {
                              UnitCost = inventoryAccount[t.ItemId].SalePrice != null ? inventoryAccount[t.ItemId].SalePrice.Value : 0,
                              Total = t.TotalQty * inventoryAccount[t.ItemId].SalePrice != null ? inventoryAccount[t.ItemId].SalePrice.Value : 0,
                              InventoryAccountId = inventoryAccount[t.ItemId].InventoryAccountId.Value,
                              LotId = t.LotId,
                              ItemId = t.ItemId,
                              Description = "",
                              DiscountRate = 0,
                              Qty = t.TotalQty,
                              PurchaseAccountId = inventoryAccount[t.ItemId].PurchaseAccountId.Value,
                              InvoiceItemId = t.Id,
                              Item = new ItemSummaryDetailOutput
                              {
                                  Id = t.ItemId,
                                  ItemCode = inventoryAccount[t.ItemId].ItemCode,
                                  ItemName = inventoryAccount[t.ItemId].ItemName,
                              },
                              KitchenOrderItemAndBOMItemId = t.Id,
                          }).ToList();

            var journal = await _journalRepository.GetAll().Include(u => u.ItemIssue).Where(s => s.ItemIssue.TransactionType == InventoryTransactionType.ItemIssueKitchenOrder && s.ItemIssue.KitchenOrderId == entities.Id).FirstOrDefaultAsync();
            var itemIssueItems = await _itemIssueItemRepository.GetAll().Where(s => s.ItemIssue.KitchenOrderId == entities.Id).ToListAsync();
            var journalItemInventory = await _journalItemRepository.GetAll().Where(s => s.JournalId == journal.Id && s.Identifier != null).ToListAsync();


            var inputApi = new UpdateItemIssueFromKitchenOrderInput
            {
                CustomerId = input.CustomerId,
                CurrencyId = input.CurrencyId,
                ClassId = input.ClassId,
                BillingAddress = input.BillingAddress,
                Date = input.OrderDate,
                DateCompare = input.OrderDate,
                Id = journal.ItemIssueId.Value,
                IsConfirm = input.IsConfirm,
                IssueNo = input.OrderNumber,
                KitchenOrderId = input.Id,
                LocationId = input.LocationId.Value,
                Memo = input.Memo,
                ReceiveFrom = ReceiveFrom.KitchenOrder,
                Reference = input.Reference,
                SameAsShippingAddress = input.SameAsShippingAddress,
                ShippingAddress = input.ShippingAddress,
                Status = input.Status,
                Total = input.Total,
                TransactionTypeId = input.TransactionTypeId,
                Items = itemLists,
            };
            if (entities != null) await _OrderRepository.BulkUpdateAsync(entities);
            if (createKitchenOrderItems.Any()) await _OrderItemRepository.BulkInsertAsync(createKitchenOrderItems);
            if (createSubBOMItems.Any()) await _kitchenOrderItemAndBOMItemRepository.BulkInsertAsync(createSubBOMItems);
            if (updateKitchenOrderItems.Any()) await _OrderItemRepository.BulkUpdateAsync(updateKitchenOrderItems);
            if (updateSubBOMItems.Any()) await _kitchenOrderItemAndBOMItemRepository.BulkUpdateAsync(updateSubBOMItems);
            await UpdateItemIssue(inputApi, tenantId, userId, itemIssueItems, journal, journalItemInventory);

            if (deleteSubBOMItems.Any()) await _kitchenOrderItemAndBOMItemRepository.BulkDeleteAsync(deleteSubBOMItems);
            if (deleteOrderItems.Any()) await _OrderItemRepository.BulkDeleteAsync(deleteOrderItems);
            return new NullableIdDto<Guid>() { Id = entities.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_KitchenOrders_Delete)]
        public async Task Remove(CarlEntityDto input)
        {
            var entities = await _OrderRepository.GetAll().Where(s => s.Id == input.Id).FirstOrDefaultAsync();
            if (entities == null) throw new UserFriendlyException(L("RecordNotFound"));
            var orderItems = await _OrderItemRepository.GetAll().AsNoTracking().Where(s => s.KitchenOrderId == entities.Id).ToListAsync();
            var orderItemIds = orderItems.Select(s => s.Id);
            var subItemOrderItems = await _kitchenOrderItemAndBOMItemRepository.GetAll().AsNoTracking().Where(s => orderItemIds.Contains(s.KitchenOrderItemId)).ToListAsync();


            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.KitchenOrder);

            if (entities.OrderNumber == auto.LastAutoSequenceNumber && auto.CustomFormat == true)
            {
                var jo = await _OrderRepository.GetAll()
                                .Where(t => t.Id != input.Id)
                                .OrderByDescending(t => t.CreationTime).FirstOrDefaultAsync();
                if (jo != null)
                {
                    auto.UpdateLastAutoSequenceNumber(jo.OrderNumber);
                }
                else
                {
                    auto.UpdateLastAutoSequenceNumber(null);
                }
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }

            var itemIssueId = await _itemIssueRepository.GetAll().AsNoTracking().Where(s => s.KitchenOrderId == entities.Id).Select(s => s.Id).FirstOrDefaultAsync();
            var inputHeper = new CarlEntityDto
            {
                Id = itemIssueId,
                IsConfirm = input.IsConfirm,
                PermissionLockId = input.PermissionLockId,
            };
            await DeleteItemIssue(inputHeper);


            if (subItemOrderItems.Any()) await _kitchenOrderItemAndBOMItemRepository.BulkDeleteAsync(subItemOrderItems);
            if (orderItems.Any()) await _OrderItemRepository.BulkDeleteAsync(orderItems);
            if (entities != null) await _OrderRepository.DeleteAsync(entities);
            if (input.IsConfirm)
            {
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Item_Find)]
        [HttpPost]
        public async Task<PagedResultDto<GetListKitchenOrderOutput>> Find(GetListKitchenOrderInput input)
        {
            var userGroups = await GetUserGroupByLocation();
            var query = _OrderRepository.GetAll()
                         .Include(u => u.Location)
                         .Include(u => u.Customer)
                         .WhereIf(input.ClassIds != null && input.ClassIds.Count() > 0, s => input.ClassIds.Contains(s.ClassId))
                         .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                         .WhereIf(input.Status != null && input.Status.Count > 0, t => input.Status.Contains(t.Status))
                         .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                         .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId.Value))
                         .WhereIf(input.FromDate != null && input.ToDate != null,
                         (u => (u.OrderDate.Date) >= (input.FromDate.Date) && (u.OrderDate.Date) <= (input.ToDate.Date)))
                         .WhereIf(!input.Filter.IsNullOrEmpty(),
                            u => u.OrderNumber.ToLower().Contains(input.Filter.ToLower()) ||
                            u.Reference.ToLower().Contains(input.Filter.ToLower()))
                         .AsNoTracking();

            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input)
                .Select(s => new GetListKitchenOrderOutput
                {
                    LocationId = s.LocationId,
                    CustomerName = s.Customer.CustomerName,
                    CustomerId = s.CustomerId,
                    LocationName = s.Location.LocationName,
                    OrderDate = s.OrderDate,
                    OrderNumber = s.OrderNumber,
                    Reference = s.Reference,
                    Status = s.Status,
                    SubTotal = s.SubTotal,
                    Tax = s.Tax,
                    Total = s.Total
                })
                .ToListAsync();
            return new PagedResultDto<GetListKitchenOrderOutput>(resultCount, @entities);
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_KitchenOrders_Import)]
        public async Task<FileDto> ExportExcelTamplate()
        {
            var result = new FileDto();
            var sheetName = "KitchenOrder";
            var hasClassFeature = IsEnabled(AppFeatures.SetupFeatureClasss);
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
                var headerList = GetReportTemplateItem(hasClassFeature);
                var reportCollumnHeader = headerList.ColumnInfo.Where(s => s.Visible).OrderBy(t => t.SortOrder).ToList();
                foreach (var i in reportCollumnHeader)
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);

                    colHeaderTable += 1;
                }
                #endregion Row 1

                result.FileName = $"KitchenOrderTamplate.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }



        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventory_KitchenOrders_Import)]
        [UnitOfWork(IsDisabled = true)]
        public async Task ImportExcel(FileDto input)
        {
            //var excelPackage = Read(input, _appFolders);
            var excelPackage = await Read(input);
            if (excelPackage == null) return;

            AccountCycle currentCycle = null;
            Tenant tenant = null;
            var tenantId = AbpSession.TenantId;
            var userId = AbpSession.GetUserId();
            var hasClassFeature = IsEnabled(AppFeatures.SetupFeatureClasss);
            var indexHasClassFeature = hasClassFeature ? 0 : -1;
            var @locations = new List<NameValueDto<long>>();
            var classes = new List<NameValueDto<long>>();
            var currencies = new List<NameValueDto<long>>();
            var customers = new List<NameValueDto<Guid?>>();
            var kitchenOrderJournalHash = new HashSet<string>();
            var itemIssueJournalHash = new HashSet<string>();
            var boms = new List<GetBomOutputSummary>();
            var bomItems = new List<GetBomItemOutput>();
            var itemTypeMenus = new List<ItemSummaryWithAccount>();
            AutoSequence kitchenOrderAuto = null;
            AutoSequence issueAuto = null;
            Guid? transactionTypeId;
            var @lots = new List<ItemLotDto>();
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    transactionTypeId = await _journalTransactionTypeManager.GetJournalTransactionTypeId(InventoryTransactionType.ItemIssueKitchenOrder);
                    currentCycle = await GetCurrentCycleAsync();
                    tenant = await GetCurrentTenantAsync();
                    @locations = await _locationRepository.GetAll().AsNoTracking().Select(s => new NameValueDto<long> { Name = s.LocationName, Value = s.Id }).ToListAsync();
                    classes = await _classRepository.GetAll().AsNoTracking().Select(s => new NameValueDto<long> { Name = s.ClassName, Value = s.Id }).ToListAsync();
                    currencies = await _currencyRepository.GetAll().Select(s => new NameValueDto<long> { Name = s.Code, Value = s.Id }).AsNoTracking().ToListAsync();
                    customers = await _customerRepository.GetAll().AsNoTracking().Select(s => new NameValueDto<Guid?> { Name = s.CustomerCode, Value = s.Id }).ToListAsync();
                    kitchenOrderJournalHash = (await _journalRepository.GetAll().Where(s => s.JournalType == JournalType.ItemIssueKitchenOrder).Select(s => s.JournalNo).ToListAsync()).ToHashSet();
                    boms = await _bomRepository.GetAll().AsNoTracking().Select(b => new GetBomOutputSummary { Id = b.Id, Name = b.Name, ItemId = b.ItemId, IsDefault = b.IsDefault }).ToListAsync();
                    kitchenOrderAuto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.KitchenOrder);
                    issueAuto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);
                    bomItems = await this.GetBomItemByBomId();
                    itemTypeMenus = await _itemRepository.GetAll().AsNoTracking()
                                         .Where(s => s.ItemType.DisplayItemMenu)
                                         .Select(s => new ItemSummaryWithAccount
                                        {
                                            Id = s.Id,
                                            ItemCode = s.ItemCode,
                                            ItemName = s.ItemName,
                                            PurchaseAccountId = s.PurchaseAccountId,
                                            PurchaseTaxId = s.PurchaseTaxId,
                                            SaleAccountId = s.SaleAccountId,
                                            SaleTaxId = s.SaleTaxId,
                                            InventoryAccountId = s.InventoryAccountId,
                                            ItemTypeId = s.ItemTypeId,

                                        }).ToListAsync();
                    itemIssueJournalHash = (await _journalRepository.GetAll().Where(s => s.ItemIssueId.HasValue).Select(s => s.JournalNo).ToListAsync()).ToHashSet();
                    @lots = await _lotRepository.GetAll().AsNoTracking().Select(s => new ItemLotDto { Id = s.Id, LotName = s.LotName, LocationId = s.LocationId }).ToListAsync();
                }
            }

            var addKitchenOrderItems = new List<KitchenOrderItem>();
            var kitchenOrderItems = new List<KitchenOrderItemImportOutput>();
            var addKitchenOrders = new List<KitchenOrder>();
            var addSubKitchenOrderItem = new List<KitchenOrderItemAndBOMItem>();
            var addItemIssueItems = new List<ItemIssueItem>();
            var addItemIssues = new List<ItemIssue>();
            var addItemIssueJournalItems = new List<JournalItem>();
            var addItemIssueJournals = new List<Journal>();
            var kitchenOrderDic = new Dictionary<string, KitchenOrder>();
            var addInventoryTransactionItems = new List<InventoryTransactionItem>();
            var itemIssueDic = new Dictionary<string, ItemIssue>();
            var addItemIssueItem = new ItemIssueItem();
            var addItemIssueJournalItem = new JournalItem();
            var addCogsItemIssueJournalItem = new JournalItem();

            // Get the work book in the file
            var workBook = excelPackage.Workbook;
            if (workBook != null)
            {
                // retrive first worksheets
                var worksheet = excelPackage.Workbook.Worksheets[0];

                for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                {
                    var kitchenOrderGroup = worksheet.Cells[i, 1].Value?.ToString();
                    var date = worksheet.Cells[i, 2].Value?.ToString();
                    var customerCode = worksheet.Cells[i, 3].Value?.ToString();
                    var locationName = worksheet.Cells[i, 4].Value?.ToString();
                    var className = hasClassFeature ? worksheet.Cells[i, 5].Value?.ToString() : "";
                    var memo = worksheet.Cells[i, 6 + indexHasClassFeature].Value?.ToString();
                    var reference = worksheet.Cells[i, 7 + indexHasClassFeature].Value?.ToString();
                    var orderNo = worksheet.Cells[i, 8 + indexHasClassFeature].Value?.ToString();
                    var itemCode = worksheet.Cells[i, 9 + indexHasClassFeature].Value?.ToString();
                    var qtyText = worksheet.Cells[i, 10 + indexHasClassFeature].Value?.ToString();
                    var currencyCode = worksheet.Cells[i, 11 + indexHasClassFeature].Value?.ToString();
                    var amount = worksheet.Cells[i, 12 + indexHasClassFeature].Value.ToString();
                    var amountInAccount = worksheet.Cells[i, 13 + indexHasClassFeature].Value.ToString();
                    var itemDescription = worksheet.Cells[i, 14 + indexHasClassFeature].Value?.ToString();
                    var bomName = worksheet.Cells[i, 15 + indexHasClassFeature].Value?.ToString();
                    var currency = currencies.Where(s => s.Name == currencyCode).FirstOrDefault();
                    var defaultClass = hasClassFeature ? classes.Where(s => s.Name == className).Select(t => t.Value).FirstOrDefault() : tenant.ClassId;
                    var location = locations.Where(s => s.Name == locationName).FirstOrDefault();
                    var customer = customers.Where(s => s.Name == customerCode).FirstOrDefault();
                   
                    var itemTypeMenu = itemTypeMenus.Where(s => s.ItemCode == itemCode).FirstOrDefault();


                    if (kitchenOrderAuto.CustomFormat == false && orderNo == null)
                    {
                        throw new UserFriendlyException(L("IsRequired", L("InvoiceNo")) + $", Row = {i}");
                    }
                    if (string.IsNullOrWhiteSpace(date))
                    {
                        throw new UserFriendlyException(L("IsRequired", L("Date")) + $", Row = {i}");
                    }
                    if (kitchenOrderAuto.RequireReference && reference.IsNullOrWhiteSpace())
                    {
                        throw new UserFriendlyException(L("IsRequired", L("Reference")) + $", Row = {i}");
                    }

                    if (currency == null)
                    {
                        throw new UserFriendlyException(L("NoCurrencyFound") + $", Row = {i}");
                    }
                    if (defaultClass == null)
                    {
                        throw new UserFriendlyException(L("NoClassFound") + $", Row = {i}");
                    }
                    if (location == null)
                    {
                        throw new UserFriendlyException(L("NoLocationFound") + $", Row = {i}");
                    }
                    if (itemTypeMenu == null) throw new UserFriendlyException(L("InvalidItemCode") + $", Row = {i}");

                    if (tenant.CurrencyId == null) throw new UserFriendlyException(L("CurrencyIsRequired"));

                    var bomId = !string.IsNullOrWhiteSpace(bomName) ?
                               boms.Where(s => s.ItemId == itemTypeMenu.Id && s.Name == bomName).Select(s => s.Id).FirstOrDefault() :
                               boms.Where(s => s.ItemId == itemTypeMenu.Id && s.IsDefault).Select(s => s.Id).FirstOrDefault();

                    if (bomId == Guid.Empty || bomId == null) throw new UserFriendlyException(L("NoBomFound") + $", Row = {i}");

                    if (string.IsNullOrWhiteSpace(qtyText)) throw new UserFriendlyException(L("QtyIsRequired") + $", Row = {i}");

                    var qty = Convert.ToDecimal(qtyText);
                    var unitPrice = Math.Round(Convert.ToDecimal(amount), currentCycle.RoundingDigitUnitCost);
                    var unitPriceInAccount = Math.Round(Convert.ToDecimal(amountInAccount), currentCycle.RoundingDigitUnitCost);
                    var totalInAcc = Math.Round(qty * unitPriceInAccount, currentCycle.RoundingDigit);
                    var totalInTran = Math.Round(qty * unitPrice, currentCycle.RoundingDigit);
                    Guid? addkitchenOrderId = null;
                    var addOrderItem = KitchenOrderItem.Create(tenantId, userId, itemTypeMenu.Id, itemTypeMenu.SaleTaxId.Value, 0, itemDescription, qty, unitPriceInAccount, 0, totalInAcc, unitPrice, totalInTran, bomId, Guid.Empty);

                    var dublication = kitchenOrderItems.Where(s => s.BOMId == addOrderItem.BOMId && s.Group == kitchenOrderGroup).FirstOrDefault();
                    if (kitchenOrderDic.ContainsKey(kitchenOrderGroup) && dublication != null)
                    {
                        var oldaddOrderItem = addKitchenOrderItems.Where(s => s.BOMId == addOrderItem.BOMId && s.Id == dublication.Id).FirstOrDefault();
                        var newQty = oldaddOrderItem.Qty + qty;
                        var newtotalInAcc = Math.Round(qty * unitPriceInAccount, currentCycle.RoundingDigit);
                        var newtotalInTran = Math.Round(qty * unitPrice, currentCycle.RoundingDigit);
                        oldaddOrderItem.SetNewQty(newQty, totalInAcc, newtotalInTran);

                    }
                    else
                    {
                        addKitchenOrderItems.Add(addOrderItem);
                        kitchenOrderItems.Add(new KitchenOrderItemImportOutput {
                            BOMId = addOrderItem.BOMId,
                            Group = kitchenOrderGroup,
                            Id = addOrderItem.Id
                        });                      
                    }
                   
                    if (kitchenOrderDic.ContainsKey(kitchenOrderGroup))
                    {
                        var addKitchenOrder = kitchenOrderDic[kitchenOrderGroup];
                        addKitchenOrder.SetSubTotal(addKitchenOrder.SubTotal + totalInAcc);
                        addKitchenOrder.SetTotal(addKitchenOrder.Total + totalInAcc);
                        addKitchenOrder.SetMultiCurrencySubTotal(addKitchenOrder.MultiCurrencySubTotal + totalInTran);
                        addKitchenOrder.SetMultiCurrencyTotal(addKitchenOrder.MultiCurrencyTotal + totalInTran);
                        addOrderItem.SetKitchenOrderId(addKitchenOrder.Id);
                        addkitchenOrderId = addKitchenOrder.Id;
                    }
                    else
                    {
                        if (kitchenOrderAuto.CustomFormat == true)
                        {
                            var newAuto = _autoSequenceManager.GetNewReferenceNumber(kitchenOrderAuto.DefaultPrefix, kitchenOrderAuto.YearFormat.Value,
                            kitchenOrderAuto.SymbolFormat, kitchenOrderAuto.NumberFormat, kitchenOrderAuto.LastAutoSequenceNumber, DateTime.Now);

                            orderNo = newAuto;
                            kitchenOrderAuto.UpdateLastAutoSequenceNumber(newAuto);
                        }
                        else if (kitchenOrderJournalHash.Contains(orderNo))
                        {
                            throw new UserFriendlyException(L("DuplicateInOrderNumber", orderNo) + $", Row = {i}");
                        }

                        CAddress billAddress = new CAddress("", "", "", "", "");
                        CAddress shipAddress = new CAddress("", "", "", "", "");

                        // put on else
                        var addkitchenOrder = KitchenOrder.Create(tenantId, userId,customer != null ? customer.Value : null, new CAddress("", "", "", "", ""), new CAddress("", "", "", "", ""), true, reference, tenant.CurrencyId.Value, orderNo,
                             Convert.ToDateTime(date), memo, 0, totalInAcc, totalInAcc, TransactionStatus.Publish, location.Value, currency.Value, totalInTran, totalInTran, 0, defaultClass.Value);
                        addKitchenOrders.Add(addkitchenOrder);
                        addOrderItem.SetKitchenOrderId(addkitchenOrder.Id);
                        addkitchenOrderId = addkitchenOrder.Id;
                        kitchenOrderDic.Add(kitchenOrderGroup, addkitchenOrder);
                    }

                    Journal addItemIssueJournal = null;
                    Guid? addItemIssueId = null;
                    if (itemIssueDic.ContainsKey(kitchenOrderGroup))
                    {
                        var addItemIssue = itemIssueDic[kitchenOrderGroup];
                        addItemIssueId = addItemIssue.Id;
                        addItemIssueJournal = addItemIssueJournals.FirstOrDefault(s => s.ItemIssueId == addItemIssue.Id);
                    }
                    else
                    {
                        var itemIssueNo = orderNo;
                        if (issueAuto.CustomFormat == true)
                        {
                            var newAuto = _autoSequenceManager.GetNewReferenceNumber(issueAuto.DefaultPrefix, issueAuto.YearFormat.Value,
                                            issueAuto.SymbolFormat, issueAuto.NumberFormat, issueAuto.LastAutoSequenceNumber, DateTime.Now);
                            itemIssueNo = newAuto;
                            issueAuto.UpdateLastAutoSequenceNumber(newAuto);
                        }
                        else if (itemIssueJournalHash.Contains(itemIssueNo) || addItemIssueJournals.Any(s => s.JournalNo == itemIssueNo))
                        {
                            throw new UserFriendlyException(L("DuplicateItemIssueNo", itemIssueNo) + $", Row = {i}");
                        }

                        var addkitChenOrder = kitchenOrderDic[kitchenOrderGroup];

                        var addItemIssue = ItemIssue.Create(tenantId, userId, ReceiveFrom.None, customer != null ? customer.Value : null, true, addkitChenOrder.ShippingAddress, addkitChenOrder.BillingAddress, 0, null);
                        addItemIssueId = addItemIssue.Id;
                        addItemIssueJournal = Journal.Create(tenantId, userId, itemIssueNo, Convert.ToDateTime(date), memo, 0, 0, currency.Value, defaultClass.Value, reference, location.Value);
                        addItemIssue.SetKitchenOrder(addkitChenOrder.Id);
                        addItemIssue.UpdateTransactionType(InventoryTransactionType.ItemIssueKitchenOrder);
                        addItemIssueJournal.UpdateItemIssue(addItemIssue.Id);
                        addItemIssueJournal.UpdateKitchen();
                        addItemIssueJournal.UpdateStatus(TransactionStatus.Publish);
                        addItemIssueJournal.SetJournalTransactionTypeId(transactionTypeId);
                        addItemIssueJournals.Add(addItemIssueJournal);
                        addItemIssues.Add(addItemIssue);
                        itemIssueDic.Add(kitchenOrderGroup, addItemIssue);
                    }


                    var bItems = bomItems.Where(s => s.BOMId == bomId)
                                         .Where(s => s.LocationId == location.Value)
                                         .ToList();

                    if(!bItems.Any()) throw new UserFriendlyException(L("IsRequired", L("Lot")) + $", Row = {i}");

                    foreach (var subItem in bItems)
                    {
                        var item = addKitchenOrderItems.Where(s =>  s.KitchenOrderId == addkitchenOrderId && subItem.BOMId == s.BOMId).FirstOrDefault();
                        var sub = addSubKitchenOrderItem.Where(s => s.BomItemId == subItem.Id && s.KitchenOrderItemId == item.Id).FirstOrDefault();
                        if (sub != null && item != null && kitchenOrderDic.ContainsKey(kitchenOrderGroup))
                        {
                         
                            var newt = sub.TotalQty + (subItem.Qty * addOrderItem.Qty);
                            sub.SetQty(sub.Qty, newt);
                            var isse = addItemIssueItems.Where(s => s.KitchenOrderItemAndBOMItemId == sub.Id).FirstOrDefault();
                            isse.UpdateQtyKitchenOrder(newt, 0);
                            var addInventory = addInventoryTransactionItems.Where(s => s.Id == isse.Id).FirstOrDefault();
                            addInventory.SetQty(newt);
                        }
                        else
                        {
                            if (subItem.LotId == null) throw new UserFriendlyException(L("LotIsRequired"));
                            var tQty = subItem.Qty * addOrderItem.Qty;
                            var addSubOrderItem = KitchenOrderItemAndBOMItem.Create(tenantId, userId, subItem.ItemId, subItem.Qty, subItem.Id.Value, addOrderItem.Id, subItem.LotId.Value, subItem.SaleTaxId.Value, 0, tQty);
                            addSubKitchenOrderItem.Add(addSubOrderItem);
                            addItemIssueItem = ItemIssueItem.Create(tenantId, userId, Guid.Empty, subItem.ItemId, itemDescription, tQty, 0, 0, 0);
                            addItemIssueItem.SetKitchenOrderItemAndBOMItemId(addSubOrderItem.Id);
                            addItemIssueJournalItem = JournalItem.CreateJournalItem(tenantId, userId, Guid.Empty, subItem.InventoryAccountId.Value, itemDescription, 0, 0, PostingKey.Inventory, addItemIssueItem.Id);
                            addCogsItemIssueJournalItem = JournalItem.CreateJournalItem(tenantId, userId, Guid.Empty, subItem.PurchaseAccountId.Value, itemDescription, 0, 0, PostingKey.COGS, addItemIssueItem.Id);
                            addItemIssueItem.UpdateLot(subItem.LotId);
                            addItemIssueJournalItem.SetJournal(addItemIssueJournal.Id);
                            addCogsItemIssueJournalItem.SetJournal(addItemIssueJournal.Id);
                            addItemIssueItem.SetItemIssue(addItemIssueId.Value);
                            addItemIssueItems.Add(addItemIssueItem);
                            addItemIssueJournalItems.Add(addItemIssueJournalItem);
                            addItemIssueJournalItems.Add(addCogsItemIssueJournalItem);

                            //Sync inventory transaction items
                            var addInventoryTransactionItem = InventoryTransactionItem.Create(
                                                          tenantId.Value,
                                                          userId,
                                                          addItemIssueItem.CreationTime,
                                                          addItemIssueItem.LastModifierUserId,
                                                          addItemIssueItem.LastModificationTime,
                                                          addItemIssueItem.Id,
                                                          addItemIssueItem.ItemIssueId,
                                                          null,
                                                          null,
                                                          addItemIssueJournal.Id,
                                                          addItemIssueJournal.Date,
                                                          addItemIssueJournal.CreationTimeIndex.Value,
                                                          addItemIssueJournal.JournalType,
                                                          addItemIssueJournal.JournalNo,
                                                          addItemIssueJournal.Reference,
                                                          addItemIssueItem.ItemId,
                                                          addItemIssueJournalItem.AccountId,
                                                          location.Value,
                                                          addItemIssueItem.LotId.Value,
                                                          addItemIssueItem.Qty,
                                                          addItemIssueItem.UnitCost,
                                                          addItemIssueItem.Total,
                                                          true,
                                                          addItemIssueItem.Description);
                            addInventoryTransactionItems.Add(addInventoryTransactionItem);

                        }

                       

                    }

                }
            }


            #region Cost Calculation and Batch/Serial Validation

            if (addItemIssueItems.Any())
            {
                var itemDic = bomItems.GroupBy(u => u.ItemId).ToDictionary(k => k.Key, v => v);
                var itemToCalculateCost = addItemIssueItems.Select((u, index) => new Inventories.Data.GetAvgCostForIssueData()
                {
                    Index = index,
                    ItemId = u.ItemId,
                    ItemName = itemDic[u.ItemId].FirstOrDefault().ItemName,
                    ItemCode = itemDic[u.ItemId].FirstOrDefault().ItemCode,
                    InventoryAccountId = itemDic[u.ItemId].FirstOrDefault().InventoryAccountId.Value,
                    Qty = u.Qty,
                    LotId = u.LotId
                }).ToList();
                var lotIds = addItemIssueItems.Select(x => x.LotId).ToList();
                var locationIds = lots.Where(s => lotIds.Contains(s.Id)).Select(s => (long?)s.LocationId).ToList();
                var toDate = addItemIssueJournals.Max(s => s.Date);
                GetAvgCostForIssueOutput getCostResult = null;
                using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        getCostResult = await _inventoryManager.GetAvgCostForIssues(toDate, locationIds, itemToCalculateCost);
                    }
                }

                foreach (var r in getCostResult.Items)
                {
                    addItemIssueItems[r.Index].UpdateUnitCost(r.UnitCost, r.LineCost);

                    var updateItemIssueJournalItems = addItemIssueJournalItems.Where(s => s.Identifier == addItemIssueItems[r.Index].Id).ToList();
                    foreach (var i in updateItemIssueJournalItems)
                    {
                        if (i.Key == PostingKey.Inventory)
                        {
                            i.SetDebitCredit(0, r.LineCost);
                        }
                        else if (i.Key == PostingKey.COGS)
                        {
                            i.SetDebitCredit(r.LineCost, 0);
                        }
                    }
                    var addInventoryTransactionItem = addInventoryTransactionItems
                                                     .Where(si => si.Id == addItemIssueItems[r.Index].Id)
                                                     .FirstOrDefault();
                    addInventoryTransactionItem.SetUnitCost(r.UnitCost);
                    addInventoryTransactionItem.SetLineCost(r.LineCost);
                }

                foreach (var i in addItemIssues)
                {
                    var total = addItemIssueItems.Where(s => s.ItemIssueId == i.Id).Sum(t => t.Total);
                    i.UpdateTotal(total);

                    var j = addItemIssueJournals.FirstOrDefault(s => s.ItemIssueId == i.Id);
                    j.SetDebitCredit(total);
                }
            }
            #endregion 
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    if (addKitchenOrders.Any()) await _OrderRepository.BulkInsertAsync(addKitchenOrders);
                    if (addKitchenOrderItems.Any()) await _OrderItemRepository.BulkInsertAsync(addKitchenOrderItems);
                    if (addSubKitchenOrderItem.Any()) await _kitchenOrderItemAndBOMItemRepository.BulkInsertAsync(addSubKitchenOrderItem);
                    if (kitchenOrderAuto.CustomFormat) CheckErrors(await _autoSequenceManager.UpdateAsync(kitchenOrderAuto));
                    if (addItemIssues.Any()) await _itemIssueRepository.BulkInsertAsync(addItemIssues);
                    if (addItemIssueItems.Any()) await _itemIssueItemRepository.BulkInsertAsync(addItemIssueItems);
                    if (addItemIssueJournals.Any()) await _journalRepository.BulkInsertAsync(addItemIssueJournals);
                    if (addInventoryTransactionItems.Any()) await _inventoryTransactionItemRepository.BulkInsertAsync(addInventoryTransactionItems);
                    if (addItemIssueJournalItems.Any()) await _journalItemRepository.BulkInsertAsync(addItemIssueJournalItems);
                    if (addItemIssues.Any() && issueAuto.CustomFormat) CheckErrors(await _autoSequenceManager.UpdateAsync(issueAuto));

                }

                await uow.CompleteAsync();
            }

        }


        #region helper      
        private async Task ItemIssueSave(CreateItemIssueFromKitchenOrderInput input)
        {
            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }
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
            var @entity = Journal.Create(tenantId, userId, input.IssueNo,
                                input.Date, input.Memo, input.Total, input.Total,
                                input.CurrencyId, input.ClassId, input.Reference,
                                input.LocationId);

            entity.UpdateStatus(input.Status);
            #region journal transaction type 
            var transactionTypeId = await _journalTransactionTypeManager.GetJournalTransactionTypeId(InventoryTransactionType.ItemIssueKitchenOrder);
            entity.SetJournalTransactionTypeId(transactionTypeId);
            #endregion

            #region Calculat Cost        
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
            var getCostResult = await _inventoryManager.GetAvgCostForIssues(input.Date, locationIds, itemToCalculateCost);

            foreach (var r in getCostResult.Items)
            {
                input.Items[r.Index].UnitCost = r.UnitCost;
                input.Items[r.Index].Total = r.LineCost;
            }

            input.Total = getCostResult.Total;

            #endregion Calculat Cost
            //insert to item Issue
            var @itemIssue = ItemIssue.Create(tenantId, userId, input.ReceiveFrom, input.CustomerId,
                input.SameAsShippingAddress, input.ShippingAddress, input.BillingAddress,
                input.Total, null);
            @itemIssue.UpdateTransactionType(InventoryTransactionType.ItemIssueKitchenOrder);
            itemIssue.SetKitchenOrder(input.KitchenOrderId);
            itemIssue.UpdateTransactionTypeId(input.TransactionTypeId);
            @entity.UpdateItemIssue(@itemIssue);
            entity.UpdateKitchen();
            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));
            CheckErrors(await _itemIssueManager.CreateAsync(itemIssue));
            await CurrentUnitOfWork.SaveChangesAsync();
            var toCreateItemIssueItem = new List<ItemIssueItem>();
            var toCreateInventoryJournalItem = new List<JournalItem>();
            var toCreateCogsJournalItem = new List<JournalItem>();
            //select SaleOrderId from Invoice          
            foreach (var i in input.Items)
            {
                //insert to item Issue item
                var @itemIssueItem = ItemIssueItem.Create(tenantId, userId,
                                        itemIssue.Id, i.ItemId, i.Description,
                                        i.Qty, i.UnitCost,
                                        i.DiscountRate, i.Total);

                itemIssueItem.UpdateLot(i.LotId);
                itemIssueItem.SetKitchenOrderItemAndBOMItemId(i.KitchenOrderItemAndBOMItemId);
                toCreateItemIssueItem.Add(itemIssueItem);
                //insert inventory journal item into credit
                var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity.Id, i.InventoryAccountId,
                    i.Description, 0, i.Total, PostingKey.Inventory, itemIssueItem.Id);
                toCreateInventoryJournalItem.Add(inventoryJournalItem);
                //insert COGS journal item into Debit
                var cogsJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity.Id, i.PurchaseAccountId,
                i.Description, i.Total, 0, PostingKey.COGS, itemIssueItem.Id);
                toCreateCogsJournalItem.Add(cogsJournalItem);
            }
            await _itemIssueItemRepository.BulkInsertAsync(toCreateItemIssueItem);
            await _journalItemRepository.BulkInsertAsync(toCreateInventoryJournalItem);
            await _journalItemRepository.BulkInsertAsync(toCreateCogsJournalItem);
            await CurrentUnitOfWork.SaveChangesAsync();
            await SyncItemIssue(itemIssue.Id);
            var scheduleItems = input.Items.Select(s => s.ItemId).Distinct().ToList();
            await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, input.Date, scheduleItems);

        }


        private async Task DeleteItemIssue(CarlEntityDto input)
        {

            var @jounal = await _journalRepository.GetAll()
                        .Include(u => u.ItemIssue)
                        .Include(u => u.ItemIssue.ShippingAddress)
                        .Include(u => u.ItemIssue.BillingAddress)
                        .Where(u => u.JournalType == JournalType.ItemIssueKitchenOrder && u.ItemIssueId == input.Id)
                        .FirstOrDefaultAsync();



            //query get item Receipt

            if (input.IsConfirm == false)
            {

                var locktransaction = await _lockRepository.GetAll()
                                       .Where(t => (t.LockKey == TransactionLockType.ItemIssue || t.LockKey == TransactionLockType.InventoryTransaction)
                                       && t.IsLock == true && t.LockDate != null && t.LockDate.Value.Date >= jounal.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            var @entity = @jounal.ItemIssue;
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);

            if (jounal.JournalNo == auto.LastAutoSequenceNumber)
            {
                var jo = await _journalRepository.GetAll()
                    .Where(t => t.Id != jounal.Id &&
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

            //query get journal and delete

            //query get journal item and delete
            var @jounalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == jounal.Id).ToListAsync();


            var scheduleDate = @jounal.Date;


            //CheckErrors(await _journalManager.RemoveAsync(@jounal));

            //query get item receipt item and delete 
            var @ItemIssueItems = await _itemIssueItemRepository.GetAll()
                .Where(u => u.ItemIssueId == entity.Id).ToListAsync();

            var scheduleItems = ItemIssueItems.Select(s => s.ItemId).Distinct().ToList();



            await DeleteInventoryTransactionItems(input.Id);
            await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, scheduleDate, scheduleItems);

            await _journalItemRepository.BulkDeleteAsync(jounalItems);
            await _journalRepository.BulkDeleteAsync(jounal);
            await _itemIssueItemRepository.BulkDeleteAsync(ItemIssueItems);
            await _itemIssueRepository.BulkDeleteAsync(entity);

        }


        private async Task UpdateItemIssue(UpdateItemIssueFromKitchenOrderInput input,
         int tenantId, long userId, List<ItemIssueItem> itemIssueItems,
                                         Journal @journal,
                                         List<JournalItem> inventoryJournalItems)
        {
            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }

            if (input.IsConfirm == false)
            {
                var validateLockDate = await _lockRepository.GetAll()
                                     .Where(t => t.IsLock == true &&
                                     (t.LockDate.Value.Date >= journal.Date.Date || t.LockDate.Value.Date >= input.Date.Date)
                                     && (t.LockKey == TransactionLockType.ItemIssue || t.LockKey == TransactionLockType.InventoryTransaction)).CountAsync();

                var isChangeItems = input.Items.Count() != itemIssueItems.Count() || input.Items.Any(s => itemIssueItems.Any(r => r.Id == s.Id && (r.ItemId != s.ItemId || r.LotId != s.LotId || r.Qty != s.Qty)));

                if (validateLockDate > 0 && (input.Date.Date != journal.Date.Date || isChangeItems || input.LocationId != journal.LocationId))
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            if (@journal == null || @journal.ItemIssue == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ItemIssue);
            if (auto.CustomFormat == true)
            {
                input.IssueNo = journal.JournalNo;
            }

            var scheduleDate = input.Date > journal.Date ? journal.Date : input.Date;
            journal.Update(userId, input.IssueNo, input.Date, input.Memo, input.Total, input.Total,
                            input.CurrencyId, input.ClassId, input.Reference, 0, input.LocationId);
            journal.UpdateStatus(input.Status);

            var lotIds = input.Items.Select(x => x.LotId).ToList();
            var locationIds = await _lotRepository.GetAll().AsNoTracking()
                                    .Where(t => lotIds.Contains(t.Id))
                                    .Select(t => (long?)t.LocationId)
                                    .ToListAsync();

            #region Calculat Cost          
            var itemToCalculateCost = input.Items.Select((u, index) => new Inventories.Data.GetAvgCostForIssueData()
            {
                ItemName = u.Item.ItemName,
                Index = index,
                ItemId = u.ItemId,
                ItemCode = u.Item.ItemCode,
                Qty = u.Qty,
                LotId = u.LotId,
                ItemIssueItemId = input.Id,
                InventoryAccountId = u.InventoryAccountId
            }).ToList();

            var getCostResult = await _inventoryManager.GetAvgCostForIssues(input.Date, locationIds, itemToCalculateCost);

            foreach (var r in getCostResult.Items)
            {
                input.Items[r.Index].UnitCost = r.UnitCost;
                input.Items[r.Index].Total = r.LineCost;
            }

            input.Total = getCostResult.Total;
            #endregion Calculat Cost

            //update item issue 
            var itemIssue = @journal.ItemIssue;
            itemIssue.Update(tenantId, itemIssue.ReceiveFrom, input.CustomerId,
                          input.SameAsShippingAddress, input.ShippingAddress,
                          input.BillingAddress, input.Total, null, false);
            itemIssue.UpdateTransactionTypeId(input.TransactionTypeId);
            itemIssue.SetKitchenOrder(input.KitchenOrderId);
            journal.UpdateCreditDebit(input.Total, input.Total);
            CheckErrors(await _journalManager.UpdateAsync(@journal, auto.DocumentType));
            CheckErrors(await _itemIssueManager.UpdateAsync(itemIssue));

            var toDeleteItemIssueItem = itemIssueItems.Where(u => !input.Items.Any(i => i.Id != null && i.Id == u.Id)).ToList();
            var toDeletejournalItem = inventoryJournalItems.Where(u => !input.Items.Any(i => u.Identifier != null && i.Id != null && i.Id == u.Identifier)).ToList();
            var toCreateItemIssueItem = new List<ItemIssueItem>();
            var toUpdateItemIssueItem = new List<ItemIssueItem>();
            var toCreateInventoryJournalItem = new List<JournalItem>();
            var toCreateCogsJournalItem = new List<JournalItem>();
            var toUpdateJournalItem = new List<JournalItem>();
            foreach (var c in input.Items)
            {
                if (c.Id != null) //update
                {
                    var itemIssueItem = itemIssueItems.FirstOrDefault(u => u.Id == c.Id);
                    var journalItem = @inventoryJournalItems.ToList().Where(u => u.Identifier == itemIssueItem.Id);
                    if (itemIssueItem != null)
                    {
                        //new
                        itemIssueItem.Update(tenantId, c.ItemId, c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total);
                        itemIssueItem.UpdateLot(c.LotId);
                        itemIssueItem.SetKitchenOrderItemAndBOMItemId(c.KitchenOrderItemAndBOMItemId);
                        toUpdateItemIssueItem.Add(itemIssueItem);
                    }

                    if (journalItem != null && journalItem.Count() > 0)
                    {
                        foreach (var i in journalItem)
                        {
                            if (i.Key == PostingKey.Inventory)
                            {
                                // update inventory posting key credit
                                i.UpdateJournalItem(userId, c.InventoryAccountId, c.Description, 0, c.Total);
                            }
                            if (i.Key == PostingKey.COGS)
                            {
                                // update cogs posting key debit
                                i.UpdateJournalItem(userId, c.PurchaseAccountId, c.Description, c.Total, 0);
                            }
                            toUpdateJournalItem.Add(i);
                        }
                    }

                }
                else if (c.Id == null) //create
                {
                    //insert to item Receipt item
                    var itemIssueItem = ItemIssueItem.Create(tenantId, userId, itemIssue.Id, c.ItemId, c.Description, c.Qty, c.UnitCost, c.DiscountRate, c.Total);
                    itemIssueItem.UpdateLot(c.LotId);
                    itemIssueItem.SetKitchenOrderItemAndBOMItemId(c.KitchenOrderItemAndBOMItemId);
                    toCreateItemIssueItem.Add(itemIssueItem);
                    //insert inventory journal item into debit

                    var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal.Id, c.InventoryAccountId, c.Description, 0, c.Total, PostingKey.Inventory, itemIssueItem.Id);
                    toCreateInventoryJournalItem.Add(inventoryJournalItem);
                    //insert cogs journal item into debit
                    var cogsJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal.Id, c.PurchaseAccountId, c.Description, c.Total, 0, PostingKey.COGS, itemIssueItem.Id);
                    toCreateCogsJournalItem.Add(cogsJournalItem);
                }
            }

            var scheduleItems = input.Items.Select(s => s.ItemId).Concat(toDeleteItemIssueItem.Select(s => s.ItemId)).Distinct().ToList();

            if (toCreateItemIssueItem.Any()) await _itemIssueItemRepository.BulkInsertAsync(toCreateItemIssueItem);
            if (toCreateInventoryJournalItem.Any()) await _journalItemRepository.BulkInsertAsync(toCreateInventoryJournalItem);
            if (toCreateCogsJournalItem.Any()) await _journalItemRepository.BulkInsertAsync(toCreateCogsJournalItem);

            if (toUpdateItemIssueItem.Any()) await _itemIssueItemRepository.BulkUpdateAsync(toUpdateItemIssueItem);
            if (toUpdateJournalItem.Any()) await _journalItemRepository.BulkUpdateAsync(toUpdateJournalItem);

            if (toDeleteItemIssueItem.Any()) await _itemIssueItemRepository.BulkDeleteAsync(toDeleteItemIssueItem);
            if (toDeletejournalItem.Any()) await _journalItemRepository.BulkDeleteAsync(toDeletejournalItem);
            await CurrentUnitOfWork.SaveChangesAsync();

            await SyncItemIssue(itemIssue.Id);
            await _inventoryCalculationItemManager.TrackChangeAsync(AbpSession.TenantId.Value, AbpSession.UserId.Value, scheduleDate, scheduleItems);

        }



        private ReportOutput GetReportTemplateItem(bool hasClassFeature)
        {
            var columns = new List<CollumnOutput>()
            {
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "KitchenOrderGroup",
                    ColumnLength = 180,
                    ColumnTitle = "Kitchen Order Group",
                    ColumnType = ColumnType.String,
                    SortOrder = 0,
                    Visible = true,
                    AllowFunction = null,
                    MoreFunction = null,
                    IsDisplay = false
                },
                // start properties with can filter
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = true,
                    ColumnName = "Date",
                    ColumnLength = 180,
                    ColumnTitle = "Date",
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
                    ColumnName = "CustomerCode",
                    ColumnLength = 230,
                    ColumnTitle = "CustomerCode",
                    ColumnType = ColumnType.String,
                    SortOrder = 2,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = true
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Location",
                    ColumnLength = 250,
                    ColumnTitle = "Location",
                    ColumnType = ColumnType.String,
                    SortOrder = 3,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Class",
                    ColumnLength = 200,
                    ColumnTitle = "Class",
                    ColumnType = ColumnType.String,
                    SortOrder = 4,
                    Visible = hasClassFeature,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                 new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Memo",
                    ColumnLength = 250,
                    ColumnTitle = "Memo",
                    ColumnType = ColumnType.String,
                    SortOrder = 5,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Reference",
                    ColumnLength = 250,
                    ColumnTitle = "Reference",
                    ColumnType = ColumnType.String,
                    SortOrder = 6,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },

                    new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "OrderNo",
                    ColumnLength = 250,
                    ColumnTitle = "OrderNo",
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
                    ColumnName = "ItemCode",
                    ColumnLength = 250,
                    ColumnTitle = "Item Code",
                    ColumnType = ColumnType.String,
                    SortOrder = 8,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },

                    new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Qty",
                    ColumnLength = 250,
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
                    ColumnName = "Currency",
                    ColumnLength = 130,
                    ColumnTitle = "Currency",
                    ColumnType = ColumnType.String,
                    SortOrder = 10,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "Amount",
                    ColumnLength = 250,
                    ColumnTitle = "Amount In Tran. Currency",
                    ColumnType = ColumnType.Money,
                    SortOrder = 11,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = true
                },
                new CollumnOutput{
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "AmountInAccounting",
                    ColumnLength = 250,
                    ColumnTitle = "Amount In Acc. Currency",
                    ColumnType = ColumnType.String,
                    SortOrder = 12,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = true
                },
                new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "ItemDescription",
                    ColumnLength = 130,
                    ColumnTitle = "Item Description",
                    ColumnType = ColumnType.String,
                    SortOrder = 13,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },
                 new CollumnOutput {
                    AllowGroupby = false,
                    AllowFilter = false,
                    ColumnName = "BOM",
                    ColumnLength = 130,
                    ColumnTitle = "BOM Name",
                    ColumnType = ColumnType.String,
                    SortOrder = 14,
                    Visible = true,
                    AllowFunction = null,
                    IsDisplay = true,
                    DisableDefault = false
                },

            };


            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = columns.WhereIf(!hasClassFeature, s => s.ColumnName != "Class").ToList(),
                Groupby = "",
                HeaderTitle = "KitchenOrder",
                Sortby = "",
            };

            return result;
        }



        private async Task<List<GetBomItemOutput>> GetBomItemByBomId()
        {

            var query = from s in _bomItemRepository.GetAll()
                                        .AsNoTracking()
                        join il in _itemLotRepository.GetAll()
                                   .AsNoTracking()
                        on s.ItemId equals il.ItemId
                        into u
                        from d in u.DefaultIfEmpty()
                        select new GetBomItemOutput
                        {
                            Id = s.Id,
                            ItemId = s.ItemId,
                            Qty = s.Qty,
                            InventoryAccountId = s.Item.InventoryAccountId,
                            PurchaseAccountId = s.Item.PurchaseAccountId,
                            ItemCode = s.Item.ItemCode,
                            ItemName = s.Item.ItemName,
                            PurchaseCurrencyId = s.Item.PurchaseCurrencyId,
                            PurchaseTaxId = s.Item.PurchaseTaxId,
                            SaleAccountId = s.Item.SaleAccountId,
                            SaleCurrenyId = s.Item.SaleCurrenyId,
                            SaleTaxId = s.Item.SaleTaxId,
                            BOMId = s.BomId,
                            LotId = d != null ? d.LotId : (long?)null,
                            LocationId = d != null ? d.Lot.LocationId : default
                        };

            var result = await query.ToListAsync();

            return result;
        }



        #endregion
    }
}
