using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Customers;
using CorarlERP.Customers.Dto;
using CorarlERP.Invoices;
using CorarlERP.ItemIssues;
using CorarlERP.ItemIssueVendorCredits;
using CorarlERP.ItemReceiptCustomerCredits;
using CorarlERP.ItemReceipts;
using CorarlERP.Items;
using CorarlERP.Journals;
using CorarlERP.SaleOrders;
using CorarlERP.Vendors;
using Microsoft.EntityFrameworkCore;
using static CorarlERP.enumStatus.EnumStatus;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore.Internal;
using CorarlERP.Vendors.Dto;
using CorarlERP.Authorization;
using Abp.Authorization;
using CorarlERP.InventoryTransactions.Dto;
using CorarlERP.InventoryTransactions;
using CorarlERP.Bills;
using CorarlERP.Authorization.Users;
using CorarlERP.Dto;
using CorarlERP.UserGroups;
using CorarlERP.Authorization.Users.Dto;
using CorarlERP.VendorTypes;
using Abp.Dependency;
using Microsoft.AspNetCore.Mvc;
using CorarlERP.JournalTransactionTypes.Dto;
using static CorarlERP.Journals.Journal;

namespace CorarlERP.Inventories
{
    [AbpAuthorize]
    public class InventoryTransactionAppService : CorarlERPAppServiceBase, IInventoryTransactionAppService
    {
        private readonly IRepository<ItemReceiptItem, Guid> _itemReceiptItemRepository;
        private readonly IRepository<ItemReceiptItemCustomerCredit, Guid> _itemReceiptCustomerCreditItemRepository;

        private readonly IRepository<User, long> _userRepository;
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
        private readonly IJournalItemManager _journalItemManager;
        private readonly IRepository<JournalItem, Guid> _journalItemRepository;
        private readonly IChartOfAccountManager _chartOfAccountManager;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;
        private readonly IRepository<Vendor, Guid> _vendorRepository;
        private readonly IRepository<ItemReceiptCustomerCredit, Guid> _itemReceiptCustomerCreditRepository;
        private readonly IRepository<ItemReceipt, Guid> _itemReceiptRepository;
        private readonly IRepository<Bill, Guid> _billRepository;
        private readonly IRepository<VendorCredit.VendorCredit, Guid> _vendorCreditRepository;

        public InventoryTransactionAppService(
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
            IRepository<ItemReceiptCustomerCredit, Guid> itemReceiptCustomerCreditRepository,
            IRepository<ItemReceiptItemCustomerCredit, Guid> itemReceiptCustomerCreditItemRepository,
            IRepository<ItemReceipt, Guid> itemReceiptRepository,
            IRepository<Bill, Guid> billRepository,
            IRepository<VendorCredit.VendorCredit, Guid> vendorCreditRepository,
            IRepository<User, long> userRepository,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            IRepository<Locations.Location, long> locationRepository
        ) : base(userGroupMemberRepository, locationRepository)
        {
            _userRepository = userRepository;
            _vendorCreditRepository = vendorCreditRepository;
            _journalManager = journalManager;
            //_journalManager.SetJournalType(JournalType.ItemIssueSale);
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
            _itemReceiptCustomerCreditItemRepository = itemReceiptCustomerCreditItemRepository;
            _itemReceiptRepository = itemReceiptRepository;
            _itemReceiptCustomerCreditRepository = itemReceiptCustomerCreditRepository;
            _billRepository = billRepository;


        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_GetList)]
        [HttpPost]
        public async Task<PagedResultDto<GetListInventoryOutPut>> GetListInventoryTransaction(ListInventoryInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var vendoTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();
            var popertyFilterList = input.PropertyDics;

            var journalQuery = _journalRepository.GetAll()
                                .Include(u => u.LastModifierUser)
                                .Where(u => u.ItemIssueId != null ||
                                            u.ItemIssueVendorCreditId != null ||
                                            u.ItemReceiptId != null ||
                                            u.ItemReceiptCustomerCreditId != null)
                                .WhereIf(input.Status != null && input.Status.Count > 0, u => input.Status.Contains(u.Status))
                                .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                                .WhereIf(input.Lcoations != null && input.Lcoations.Count > 0, u => input.Lcoations.Contains(u.LocationId.Value))
                                .WhereIf(input.JournalTypes != null && input.JournalTypes.Count > 0, u => input.JournalTypes.Contains(u.JournalType))
                                .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                                .WhereIf(input.JournalTransactionTypeIds != null && input.JournalTransactionTypeIds.Count > 0, u => u.JournalTransactionTypeId != null && input.JournalTransactionTypeIds.Contains(u.JournalTransactionTypeId.Value))
                                .WhereIf(input.FromDate != null && input.ToDate != null,
                                         (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                                .WhereIf(!input.Filter.IsNullOrEmpty(),
                                            u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                                 u.Reference.ToLower().Contains(input.Filter.ToLower()))
                                .WhereIf(input.InventoryTypes != null && input.InventoryTypes.Count > 0 ,s =>s.JournalTransactionType != null &&
                                     ((input.InventoryTypes.Contains((long)InventoryType.ItemIssueSale) && s.JournalTransactionType.IsIssue) ||
                                     (input.InventoryTypes.Contains((long)InventoryType.ItemReceiptPurchase) && !s.JournalTransactionType.IsIssue)))
                                .AsNoTracking()
                                .Select(s => new
                                {                                 
                                    Id = s.Id,
                                    Date = s.Date,
                                    JournalNo = s.JournalNo,
                                    JournalTransactionTypeName = s.JournalTransactionTypeId != null ? s.JournalTransactionType.Name : null,
                                    Reference = s.Reference,
                                    Memo = s.Memo,
                                    JournalType = s.JournalType,
                                    ItemIssueId = s.ItemIssueId,
                                    ItemReceiptId = s.ItemReceiptId,
                                    ItemReceiptCustomerCreditId = s.ItemReceiptCustomerCreditId,
                                    ItemIssueVendorCreditId = s.ItemIssueVendorCreditId,
                                    CreationTimeIndex = s.CreationTimeIndex,
                                    CreationTime = s.CreationTime,
                                    Status = s.Status,
                                    LocationId = s.LocationId,
                                    CreatorUserId = s.CreatorUserId,
                                    LastModifiedUserName = s.LastModifierUser != null ? s.LastModifierUser.Name : "",
                                    LastModifiedTime = s.LastModificationTime
                                });

            var locationQuery = GetLocations(null, input.Lcoations);
            var userQuery = GetUsers(input.Users);
            var juQuery = from j in journalQuery
                          join u in userQuery
                          on j.CreatorUserId equals u.Id
                          join l in locationQuery
                          on j.LocationId equals l.Id
                          select new
                          {
                              Id = j.Id,
                              Date = j.Date,
                              JournalNo = j.JournalNo,
                              JournalTransactionTypeName = j.JournalTransactionTypeName,
                              Reference = j.Reference,
                              Memo = j.Memo,
                              JournalType = j.JournalType,
                              ItemIssueId = j.ItemIssueId,
                              ItemReceiptId = j.ItemReceiptId,
                              ItemReceiptCustomerCreditId = j.ItemReceiptCustomerCreditId,
                              ItemIssueVendorCreditId = j.ItemIssueVendorCreditId,
                              CreationTimeIndex = j.CreationTimeIndex,
                              CreationTime = j.CreationTime,
                              Status = j.Status,
                              LocationId = j.LocationId,
                              CreatorUserId = j.CreatorUserId,
                              LocationName = l.LocationName,
                              UserName = u.UserName,
                              LastModifiedUserName = j.LastModifiedUserName,
                              LastModifiedTime = j.LastModifiedTime
                          };


            var journalItemQuery = _journalItemRepository.GetAll()
                                    .Where(u => (u.Key == PostingKey.COGS || u.Key == PostingKey.Clearance) && u.Identifier == null)
                                    .WhereIf(input.Accounts != null && input.Accounts.Count > 0, u => input.Accounts.Contains(u.AccountId))
                                    .AsNoTracking()
                                    .Select(s => new
                                    {
                                        s.Id,
                                        s.JournalId,
                                        s.AccountId
                                    });

            var accountQuery = GetAccounts(input.Accounts);
            var jaQuery = from ji in journalItemQuery
                          join a in accountQuery
                          on ji.AccountId equals a.Id
                          select new
                          {
                              ji.JournalId,
                              ji.AccountId,
                              a.AccountName
                          };

            var jiQuery = from j in juQuery
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
                              ItemIssueId = j.ItemIssueId,
                              ItemReceiptId = j.ItemReceiptId,
                              ItemReceiptCustomerCreditId = j.ItemReceiptCustomerCreditId,
                              ItemIssueVendorCreditId = j.ItemIssueVendorCreditId,
                              CreationTimeIndex = j.CreationTimeIndex,
                              CreationTime = j.CreationTime,
                              Status = j.Status,
                              LocationId = j.LocationId,
                              CreatorUserId = j.CreatorUserId,
                              LocationName = j.LocationName,
                              UserName = j.UserName,
                              AccountId = ji == null ? (Guid?)null : ji.AccountId,
                              AccountName = ji == null ? "" : ji.AccountName,
                              LastModifiedUserName = j.LastModifiedUserName,
                              LastModifiedTime = j.LastModifiedTime
                          };


            var iiQuery = from ii in _itemIssueRepository.GetAll()
                                    .WhereIf(input.Customers != null && input.Customers.Count > 0, u => input.Customers.Contains(u.CustomerId))
                                    .AsNoTracking()
                                    .Select(s => new
                                    {
                                        Id = s.Id,
                                        Total = s.Total,
                                        CustomerId = s.CustomerId
                                    })
                          join iii in _itemIssueItemRepository.GetAll()
                                       .Include(u => u.Item.Properties)
                                       .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                                       .WhereIf(popertyFilterList != null && popertyFilterList.Count > 0, p => (p.Item.Properties.Where(i => popertyFilterList.Any(v => v.PropertyId == i.PropertyId) &&
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId) != null &&
                                                                                                                  (popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds == null ||
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Count == 0 ||
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Contains(i.PropertyValueId.Value)))
                                                                                                       .Count() == popertyFilterList.Count))
                                       .AsNoTracking()
                                       .Select(s => s.ItemIssueId)
                          on ii.Id equals iii
                          into iis
                          where iis.Count() > 0
                          select ii;

            var customerTypeMemberIds = await GetCustomerTypeMembers().Select(s => s.CustomerTypeId).ToListAsync();

            var customerQuery = GetCustomers(null, input.Customers, null, customerTypeMemberIds);
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
                                        .Include(u => u.Item.Properties)
                                        .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                                        .WhereIf(popertyFilterList != null && popertyFilterList.Count > 0, p => (p.Item.Properties.Where(i => popertyFilterList.Any(v => v.PropertyId == i.PropertyId) &&
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId) != null &&
                                                                                                                  (popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds == null ||
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Count == 0 ||
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Contains(i.PropertyValueId.Value)))
                                                                                                       .Count() == popertyFilterList.Count))
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
                                        .Include(u => u.Item.Properties)
                                        .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                                        .WhereIf(popertyFilterList != null && popertyFilterList.Count > 0, p => (p.Item.Properties.Where(i => popertyFilterList.Any(v => v.PropertyId == i.PropertyId) &&
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId) != null &&
                                                                                                                  (popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds == null ||
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Count == 0 ||
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Contains(i.PropertyValueId.Value)))
                                                                                                       .Count() == popertyFilterList.Count))
                                        .AsNoTracking()
                                        .Select(s => s.ItemReceiptId)
                          on ir.Id equals iri
                          into irs
                          where irs.Count() > 0
                          select ir;

            var vendorQuery = GetVendors(null, input.Vendors, null, vendoTypeMemberIds);
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

            var ivQuery = from iv in _itemIssueVendorCreditRepository.GetAll()
                                    .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId))
                                    .AsNoTracking()
                                    .Select(s => new
                                    {
                                        Id = s.Id,
                                        Total = s.Total,
                                        VendorId = s.VendorId
                                    })

                          join ivi in _itemIssueVendorCreditItemRepository.GetAll()
                                       .Include(u => u.Item.Properties)
                                       .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                                        .WhereIf(popertyFilterList != null && popertyFilterList.Count > 0, p => (p.Item.Properties.Where(i => popertyFilterList.Any(v => v.PropertyId == i.PropertyId) &&
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId) != null &&
                                                                                                                  (popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds == null ||
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Count == 0 ||
                                                                                                                  popertyFilterList.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Contains(i.PropertyValueId.Value)))
                                                                                                       .Count() == popertyFilterList.Count))
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

                         join ir in itemReceiptQuery
                         on j.ItemReceiptId equals ir.Id
                         into irs
                         from ir in irs.DefaultIfEmpty()

                         join ic in itemReceiptCustomerCreditQeury
                         on j.ItemReceiptCustomerCreditId equals ic.Id
                         into ics
                         from ic in ics.DefaultIfEmpty()

                         join iv in itemIssueVendorCreditQuery
                         on j.ItemIssueVendorCreditId equals iv.Id
                         into ivs
                         from iv in ivs.DefaultIfEmpty()

                         where (ii != null || ir != null || ic != null || iv != null) &&
                               (input.Customers == null || input.Customers.Count == 0 || ii != null || ic != null) &&
                               (input.Vendors == null || input.Vendors.Count == 0 || ir != null || iv != null)

                         select new GetListInventoryOutPut
                         {
                             CreationTimeIndex = j.CreationTimeIndex,
                             CreationTime = j.CreationTime,
                             LocationId = j.LocationId,
                             Memo = j.Memo,
                             LocationName = j.LocationName,
                             Id = ii != null ? ii.Id :
                                  ic != null ? ic.Id :
                                  ir != null ? ir.Id : iv.Id,
                             Date = j.Date,
                             JournalNo = j.JournalNo,
                             JournalTransactionTypeName = j.JournalTransactionTypeName,
                             Reference = j.Reference,
                             Status = j.Status,
                             Total = ii != null ? ii.Total :
                                     ic != null ? ic.Total :
                                     ir != null ? ir.Total : iv.Total,
                             Customer = ii != null && ii.CustomerId != null ?
                                        new CustomerSummaryOutput
                                        {
                                            Id = ii.CustomerId.Value,
                                            CustomerName = ii.CustomerName
                                        } :
                                        ic != null ?
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
                                        } :
                                        iv != null ?
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
                             LastModifiedUserName = j.LastModifiedUserName,
                             LastModifiedTime = j.LastModifiedTime,
                             JournalId = j.Id,
                         };

            var resultCount = await result.CountAsync();
            if (resultCount == 0) return new PagedResultDto<GetListInventoryOutPut>(resultCount, new List<GetListInventoryOutPut>());

            if (input.Sorting.EndsWith("DESC"))
            {
                if (input.Sorting.ToLower().StartsWith("date"))
                {
                    var sortQeury = result.OrderByDescending(s => s.Date.Date).ThenByDescending(s => s.CreationTimeIndex);
                    result = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("typename"))
                {
                    var sortQeury = result.OrderByDescending(s => s.TypeName).ThenByDescending(s => s.CreationTimeIndex);
                    result = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("journalno"))
                {
                    var sortQeury = result.OrderByDescending(s => s.JournalNo).ThenByDescending(s => s.CreationTimeIndex);
                    result = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("total"))
                {
                    var sortQeury = result.OrderByDescending(s => s.Total).ThenByDescending(s => s.CreationTimeIndex);
                    result = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("status"))
                {
                    var sortQeury = result.OrderByDescending(s => s.Status).ThenByDescending(s => s.CreationTimeIndex);
                    result = sortQeury;
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    var sortQeury = result.OrderBy(input.Sorting).ThenByDescending(s => s.CreationTimeIndex);
                    result = sortQeury;
                }
            }
            else
            {
                if (input.Sorting.ToLower().StartsWith("date"))
                {
                    var sortQeury = result.OrderBy(s => s.Date.Date).ThenBy(s => s.CreationTimeIndex);
                    result = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("typename"))
                {
                    var sortQeury = result.OrderBy(s => s.TypeName).ThenBy(s => s.CreationTimeIndex);
                    result = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("journalno"))
                {
                    var sortQeury = result.OrderBy(s => s.JournalNo).ThenBy(s => s.CreationTimeIndex);
                    result = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("total"))
                {
                    var sortQeury = result.OrderBy(s => s.Total).ThenBy(s => s.CreationTimeIndex);
                    result = sortQeury;
                }
                else if (input.Sorting.ToLower().StartsWith("status"))
                {
                    var sortQeury = result.OrderBy(s => s.Status).ThenBy(s => s.CreationTimeIndex);
                    result = sortQeury;
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    var sortQeury = result.OrderBy(input.Sorting).ThenBy(s => s.CreationTimeIndex);
                    result = sortQeury;
                }
            }

            var @entities = await result.PageBy(input).ToListAsync();

            return new PagedResultDto<GetListInventoryOutPut>(resultCount, ObjectMapper.Map<List<GetListInventoryOutPut>>(@entities));
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_GetList)]
        public async Task<PagedResultDto<GetListInventoryOutPut>> GetListInventoryTransactionOld(ListInventoryInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var vendoTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();


            var result = from j in _journalRepository.GetAll()
                                  .Include(u => u.ItemIssue.Customer)
                                  .Include(u => u.ItemIssueVendorCredit.Vendor)
                                  .Include(u => u.ItemReceipt.Vendor)
                                  .Include(u => u.ItemReceiptCustomerCredit.Customer)
                                  .Include(u => u.Location)
                                  .Include(u => u.CreatorUser)
                                 .Where(u => u.ItemIssueId != null ||
                                            u.ItemIssueVendorCreditId != null ||
                                            u.ItemReceiptId != null ||
                                            u.ItemReceiptCustomerCreditId != null)
                                 .WhereIf(input.Status != null && input.Status.Count > 0, u => input.Status.Contains(u.Status))
                                 .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                                 .WhereIf(input.Lcoations != null && input.Lcoations.Count > 0, u => input.Lcoations.Contains(u.LocationId.Value))
                                 .WhereIf(input.JournalTypes != null && input.JournalTypes.Count > 0, u => input.JournalTypes.Contains(u.JournalType))
                                 .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                                 .WhereIf(input.FromDate != null && input.ToDate != null,
                                         (u => (u.Date.Date) >= (input.FromDate.Date) && (u.Date.Date) <= (input.ToDate.Date)))
                                  .WhereIf(!input.Filter.IsNullOrEmpty(),
                                  u => u.JournalNo.ToLower().Contains(input.Filter.ToLower()) ||
                                                 u.Reference.ToLower().Contains(input.Filter.ToLower()) ||
                                                 u.Memo.ToLower().Contains(input.Filter.ToLower())).AsNoTracking()

                         join jItem in _journalItemRepository.GetAll()
                                                                   .Include(u => u.Account)
                                                                   .Where(u => (u.Key == PostingKey.COGS || u.Key == PostingKey.Clearance) && u.Identifier == null)
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


                         where (leftIivi != null || leftIii != null || leftIrca != null || leftIri != null)

                         select new GetListInventoryOutPut
                         {
                             CreationTimeIndex = j.CreationTimeIndex,
                             CreationTime = j.CreationTime,
                             LocationId = j.LocationId,
                             Memo = j.Memo,
                             LocationName = j.Location.LocationName,
                             Id = j.ItemIssue != null ? j.ItemIssue.Id :
                                  j.ItemIssueVendorCredit != null ? j.ItemIssueVendorCredit.Id :
                                  j.ItemReceipt != null ? j.ItemReceipt.Id :
                                  j.ItemReceiptCustomerCredit.Id,
                             Date = j.Date,
                             JournalNo = j.JournalNo,
                             Status = j.Status,
                             Total = j.ItemIssue != null ? j.ItemIssue.Total :
                                  j.ItemIssueVendorCredit != null ? j.ItemIssueVendorCredit.Total :
                                  j.ItemReceipt != null ? j.ItemReceipt.Total :
                                  j.ItemReceiptCustomerCredit.Total,
                             Customer = j.ItemIssue != null ? ObjectMapper.Map<CustomerSummaryOutput>(j.ItemIssue.Customer) :
                                        j.ItemReceiptCustomerCredit != null ? ObjectMapper.Map<CustomerSummaryOutput>(j.ItemReceiptCustomerCredit.Customer) :
                                        null,
                             Vendor = j.ItemReceipt != null ? ObjectMapper.Map<VendorSummaryOutput>(j.ItemReceipt.Vendor) :
                                        j.ItemIssueVendorCredit != null ? ObjectMapper.Map<VendorSummaryOutput>(j.ItemIssueVendorCredit.Vendor) :
                                        null,
                             User = ObjectMapper.Map<UserDto>(j.CreatorUser),
                             CustomerId = j.ItemIssue.CustomerId,
                             Type = j.JournalType,
                             TypeName = Enum.GetName(j.JournalType.GetType(), j.JournalType),
                             AccountId = headerAccount == null ? Guid.NewGuid() : headerAccount.AccountId,
                             AccountName = headerAccount == null || headerAccount.Account == null ? null : headerAccount.Account.AccountName,
                         };


            result = result.WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => u.Vendor != null && input.Vendors.Contains(u.Vendor.Id))
                           .WhereIf(input.Customers != null && input.Customers.Count > 0, u => u.Customer != null && input.Customers.Contains(u.CustomerId))
                           .WhereIf(input.Accounts != null && input.Accounts.Count > 0, u => input.Accounts.Contains(u.AccountId))
                           .WhereIf(vendoTypeMemberIds.Any(), s => s.Vendor == null || vendoTypeMemberIds.Contains(s.Vendor.VendorTypeId.Value));

            var resultCount = await result.CountAsync();

            List<GetListInventoryOutPut> @entities;

            if (input.Sorting.Contains("date") && !input.Sorting.Contains("."))
                input.Sorting = input.Sorting.Replace("date", "Date.Date");

            input.Sorting += ", CreationTimeIndex" + (input.Sorting.Contains("DESC") ? " DESC" : "");


            @entities = await result.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<GetListInventoryOutPut>(resultCount, ObjectMapper.Map<List<GetListInventoryOutPut>>(@entities));
        }
    }

}
