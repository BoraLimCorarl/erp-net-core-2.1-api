using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.AccountCycles;
using CorarlERP.Authorization.Users;
using CorarlERP.AutoSequences;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Common.Dto;
using CorarlERP.Customers;
using CorarlERP.DeliverySchedules.Dto;
using CorarlERP.FileStorages;
using CorarlERP.FileUploads;
using CorarlERP.Galleries;
using CorarlERP.Inventories;
using CorarlERP.Invoices;
using CorarlERP.InvoiceTemplates;
using CorarlERP.ItemIssues;
using CorarlERP.ItemReceiptCustomerCredits;
using CorarlERP.Items;
using CorarlERP.Journals;
using CorarlERP.Locks;
using CorarlERP.MultiTenancy;
using CorarlERP.Reports;
using CorarlERP.SaleOrders;
using CorarlERP.Url;
using CorarlERP.UserGroups;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.Customers.Dto;
using CorarlERP.SaleOrders.Dto;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using CorarlERP.Dto;
using CorarlERP.Journals.Dto;
using CorarlERP.Authorization;
using EvoPdf.HtmlToPdfClient;
using CorarlERP.Items.Dto;

namespace CorarlERP.DeliverySchedules
{
    [AbpAuthorize]
    public class DeliveryScheduleAppService : ReportBaseClass, IDeliveryScheduleAppService
    {
        private readonly IRepository<Journal, Guid> _journalRepository;
        private readonly ISaleOrderManager _saleOrderManager;
        private readonly IRepository<SaleOrder, Guid> _saleOrderRepository;
        private readonly ISaleOrderItemManager _saleOrderItemManager;
        private readonly IRepository<SaleOrderItem, Guid> _saleOrderItemRepository;
        private readonly IRepository<Item, Guid> _itemRepository;
        private readonly IRepository<Customer, Guid> _customerRepository;
        private readonly IRepository<ItemIssueItem, Guid> _itemIssueItemRepository;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;
        private readonly IRepository<InvoiceItem, Guid> _invoiceRepository;
        private readonly IInventoryManager _inventoryManager;
        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly IRepository<AccountCycle, long> _accountCycleRepository;
        private readonly IRepository<InvoiceItem, Guid> _invoiceItemRepository;
        private readonly IRepository<Lock, long> _lockRepository;
        private readonly AppFolders _appFolders;
        private readonly IRepository<InvoiceTemplate, Guid> _invoiceTemplateRepository;
        private readonly IRepository<InvoiceTemplateMap, Guid> _invoiceTemplateMapRepository;
        private readonly IRepository<Gallery, Guid> _galleryRepository;
        private readonly string _baseUrl;
        private readonly ITenantManager _tenantManager;
        private readonly IRepository<User, long> _userRepository;
        private readonly IFileUploadManager _fileUploadManager;
        private readonly IFileStorageManager _fileStorageManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICorarlRepository<DeliverySchedule, Guid> _deliveryScheduleRepository;
        private IDeliveryScheduleItemManager _deliveryScheduleItemManager;
        private readonly ICorarlRepository<DeliveryScheduleItem, Guid> _deliveryScheduleItemRepository;
        private IDeliveryScheduleManager _deliveryScheduleManager;
        protected readonly ICorarlRepository<ItemReceiptItemCustomerCredit, Guid> _itemReceiptItemCustomerCreditRepository;

        public DeliveryScheduleAppService(
            ITenantManager tenantManager,
            IRepository<InvoiceTemplate, Guid> invoiceTemplateRepository,
            IRepository<InvoiceTemplateMap, Guid> invoiceTemplateMapRepository,
            IRepository<Gallery, Guid> galleryRepository,
            IFileUploadManager fileUploadManager,
            IFileStorageManager fileStorageManager,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<User, long> userRepository,
            ISaleOrderManager saleOrderManager,
            ISaleOrderItemManager saleOrderItemManager,
            IRepository<SaleOrder, Guid> saleOrderRepository,
            IRepository<SaleOrderItem, Guid> saleOrderItemRepository,
            IRepository<Item, Guid> itemRepository,
            IRepository<Customer, Guid> customerRepository,
            IRepository<Journal, Guid> journalRepository,
            IRepository<ItemIssueItem, Guid> itemIssueItemRepository,
            IRepository<InvoiceItem, Guid> invoiceRepository,
            IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
            IInventoryManager inventoryManager,
            IRepository<AutoSequence, Guid> autoSequenceRepository,
            IRepository<Locations.Location, long> locationRepository,
            IAutoSequenceManager autoSequenceManager,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            IRepository<AccountCycle, long> accountCycleRepository,
            IRepository<InvoiceItem, Guid> invoiceItemRepository,
            IRepository<Lock, long> lockRepository,
            AppFolders appFolders,
            IRepository<AccountCycles.AccountCycle, long> accountCyclesRepository,
            ICorarlRepository<DeliverySchedule, Guid> deliveryScheduleRepository,
            ICorarlRepository<DeliveryScheduleItem, Guid> deliveryScheduleItemRepository,
            IDeliveryScheduleItemManager deliveryScheduleItemManager,
            IDeliveryScheduleManager deliveryScheduleManager,
            ICorarlRepository<ItemReceiptItemCustomerCredit, Guid> itemReceiptItemCustomerCreditRepository

        ) : base(accountCyclesRepository, appFolders, userGroupMemberRepository, locationRepository)
        {
            _invoiceItemRepository = invoiceItemRepository;
            _accountCycleRepository = accountCycleRepository;
            _saleOrderManager = saleOrderManager;
            _saleOrderRepository = saleOrderRepository;
            _saleOrderItemManager = saleOrderItemManager;
            _saleOrderItemRepository = saleOrderItemRepository;
            _itemRepository = itemRepository;
            _customerRepository = customerRepository;
            _journalRepository = journalRepository;
            _chartOfAccountRepository = chartOfAccountRepository;
            _itemIssueItemRepository = itemIssueItemRepository;
            _invoiceRepository = invoiceRepository;
            _inventoryManager = inventoryManager;
            _autoSequenceManager = autoSequenceManager;
            _autoSequenceRepository = autoSequenceRepository;
            _appFolders = appFolders;
            _lockRepository = lockRepository;
            _invoiceTemplateRepository = invoiceTemplateRepository;
            _invoiceTemplateMapRepository = invoiceTemplateMapRepository;
            _tenantManager = tenantManager;
            _baseUrl = (IocManager.Instance.Resolve<IWebUrlService>()).GetServerRootAddress().EnsureEndsWith('/');
            _userRepository = userRepository;
            _galleryRepository = galleryRepository;
            _fileUploadManager = fileUploadManager;
            _fileStorageManager = fileStorageManager;
            _unitOfWorkManager = unitOfWorkManager;
            _deliveryScheduleItemManager = deliveryScheduleItemManager;
            _deliveryScheduleRepository = deliveryScheduleRepository;
            _deliveryScheduleManager = deliveryScheduleManager;
            _deliveryScheduleItemRepository = deliveryScheduleItemRepository;
            _itemReceiptItemCustomerCreditRepository = itemReceiptItemCustomerCreditRepository;

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_DeliverySchedules_Create)]
        public async Task<NullableIdDto<Guid>> Create(CreateDeliveryScheduleInput input)
        {
            var result = await this.SaveDeliverySchedule(input);
            return new NullableIdDto<Guid>() { Id = result.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_DeliverySchedules_GetList)]
        public async Task<PagedResultDto<GetListDeliveryScheduleOutput>> GetList(GetListDeliveryScheduleInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var userQuery = GetUsers(input.Users);
            var locationQuery = GetLocations(null, input.Locations);

            var customerTypeMemberIds = await GetCustomerTypeMembers().Select(s => s.CustomerTypeId).ToListAsync();

            var customerQuery = GetCustomers(null, input.Customers, input.CustomerTypes, customerTypeMemberIds);

            var oQuery = _deliveryScheduleRepository.GetAll()
                         .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId))
                         .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                         .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId))
                         .WhereIf(input.Customers != null && input.Customers.Count > 0, u => input.Customers.Contains(u.CustomerId))
                         .WhereIf(input.FromDate != null && input.ToDate != null,
                            (u => (u.InitialDeliveryDate.Date) >= (input.FromDate.Date) && (u.InitialDeliveryDate.Date) <= (input.ToDate.Date)))
                         .WhereIf(input.Status != null && input.Status.Count > 0, u => input.Status.Contains(u.Status))
                         .WhereIf(input.DeliveryStatuses != null && input.DeliveryStatuses.Count > 0, u => input.DeliveryStatuses.Contains(u.ReceiveStatus))
                         .WhereIf(!input.Filter.IsNullOrEmpty(),
                                p => p.DeliveryNo.ToLower().Contains(input.Filter.ToLower()) ||
                                p.Reference.ToLower().Contains(input.Filter.ToLower())
                         )
                         .AsNoTracking()
                         .Select(o => new
                         {
                             Id = o.Id,
                             InitialDeliveryDate = o.InitialDeliveryDate,
                             FinalDeliveryDate = o.FinalDeliveryDate,
                             DeliveryNo = o.DeliveryNo,
                             Reference = o.Reference,
                             Status = o.Status,
                             ReceiveStatus = o.ReceiveStatus,
                             CreatorId = o.CreatorUserId,
                             CustomerId = o.CustomerId,
                             LocationId = o.LocationId,
                             SaleOrderId = o.SaleOrderId,
                         });

            var oiQuery = _deliveryScheduleItemRepository.GetAll()
                         .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                         .AsNoTracking()
                         .Select(s => s.DeliveryScheduleId);


            var query = from o in oQuery
                        join c in customerQuery
                        on o.CustomerId equals c.Id
                        join l in locationQuery
                        on o.LocationId equals l.Id
                        join u in userQuery
                        on o.CreatorId equals u.Id
                        join oi in oiQuery
                        on o.Id equals oi
                        into ois
                        where ois.Count() > 0

                        select new GetListDeliveryScheduleOutput
                        {
                            Id = o.Id,
                            InitialDeliveryDate = o.InitialDeliveryDate,
                            ReceiveStatusName = o.ReceiveStatus.ToString(),
                            SaleOrderId = o.SaleOrderId,
                            FinalDeliveryDate = o.FinalDeliveryDate,
                            DeliveryNo = o.DeliveryNo,
                            CountItem = ois.Count(),
                            Reference = o.Reference,
                            Status = o.Status,
                            StatusName = o.Status.ToString(),
                            ReceiveStatus = o.ReceiveStatus,
                            Customer = new CustomerSummaryOutput
                            {
                                Id = o.CustomerId,
                                CustomerName = c.CustomerName
                            },
                            User = new UserDto
                            {
                                Id = o.CreatorId.Value,
                                UserName = u.UserName
                            },
                            LocationName = l.LocationName,
                            LocationId = l.Id,

                        };

            var resultCount = await query.CountAsync();
            if (resultCount == 0) return new PagedResultDto<GetListDeliveryScheduleOutput>(resultCount, new List<GetListDeliveryScheduleOutput>());

            if (!string.IsNullOrEmpty(input.Sorting))
            {
                var sortingDirection = input.Sorting.EndsWith("DESC", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
                var sortingProperty = input.Sorting.ToLower().Replace("desc", "").Replace("asc", "").Trim();

                switch (sortingProperty)
                {
                    case "finaldeliverydate":
                        query = sortingDirection == "DESC" ? query.OrderByDescending(s => s.FinalDeliveryDate) : query.OrderBy(s => s.FinalDeliveryDate);
                        break;
                    case "initialdeliverydate":
                        query = sortingDirection == "DESC" ? query.OrderByDescending(s => s.InitialDeliveryDate) : query.OrderBy(s => s.InitialDeliveryDate);
                        break;
                    case "deliveryno":
                        query = sortingDirection == "DESC" ? query.OrderByDescending(s => s.DeliveryNo) : query.OrderBy(s => s.DeliveryNo);
                        break;
                    case "reference":
                        query = sortingDirection == "DESC" ? query.OrderByDescending(s => s.Reference) : query.OrderBy(s => s.Reference);
                        break;
                    case "receivestatus":
                        query = sortingDirection == "DESC" ? query.OrderByDescending(s => s.ReceiveStatus) : query.OrderBy(s => s.ReceiveStatus);
                        break;
                    case "statuscode":
                        query = sortingDirection == "DESC" ? query.OrderByDescending(s => s.StatusName) : query.OrderBy(s => s.StatusName);
                        break;
                    case "customer":
                        query = sortingDirection == "DESC" ? query.OrderByDescending(s => s.Customer.CustomerName) : query.OrderBy(s => s.Customer.CustomerName);
                        break;
                    default:
                        query = sortingDirection == "DESC" ? query.OrderByDescending(s => s.DeliveryNo) : query.OrderBy(s => s.DeliveryNo); // Default fallback
                        break;
                }
            }



            var @entities = await query.PageBy(input).ToListAsync();

            return new PagedResultDto<GetListDeliveryScheduleOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_DeliverySchedules_Find)]
        public async Task<PagedResultDto<GetListDeliveryScheduleOutput>> Find(GetListDeliveryScheduleInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var userQuery = GetUsers(input.Users);
            var locationQuery = GetLocations(null, input.Locations);

            var customerTypeMemberIds = await GetCustomerTypeMembers().Select(s => s.CustomerTypeId).ToListAsync();

            var customerQuery = GetCustomers(null, input.Customers, input.CustomerTypes, customerTypeMemberIds);

            var oQuery = _deliveryScheduleRepository.GetAll()
                         .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId))
                         .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                         .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId))
                         .WhereIf(input.Customers != null && input.Customers.Count > 0, u => input.Customers.Contains(u.CustomerId))
                         .WhereIf(input.FromDate != null && input.ToDate != null,
                            (u => (u.InitialDeliveryDate.Date) >= (input.FromDate.Date) && (u.InitialDeliveryDate.Date) <= (input.ToDate.Date)))
                         .WhereIf(input.Status != null && input.Status.Count > 0, u => input.Status.Contains(u.Status))
                         .WhereIf(input.DeliveryStatuses != null && input.DeliveryStatuses.Count > 0, u => input.DeliveryStatuses.Contains(u.ReceiveStatus))
                         .WhereIf(!input.Filter.IsNullOrEmpty(),
                                p => p.DeliveryNo.ToLower().Contains(input.Filter.ToLower()) ||
                                p.Reference.ToLower().Contains(input.Filter.ToLower())
                         )
                         .AsNoTracking()
                         .Select(o => new
                         {
                             Id = o.Id,
                             InitialDeliveryDate = o.InitialDeliveryDate,
                             FinalDeliveryDate = o.FinalDeliveryDate,
                             DeliveryNo = o.DeliveryNo,
                             Reference = o.Reference,
                             Status = o.Status,
                             ReceiveStatus = o.ReceiveStatus,
                             CreatorId = o.CreatorUserId,
                             CustomerId = o.CustomerId,
                             LocationId = o.LocationId,
                             SaleOrderId = o.SaleOrderId,
                         });

            var oiQuery = _deliveryScheduleItemRepository.GetAll()
                         .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                         .AsNoTracking()
                         .Select(s => s.DeliveryScheduleId);


            var query = from o in oQuery
                        join c in customerQuery
                        on o.CustomerId equals c.Id
                        join l in locationQuery
                        on o.LocationId equals l.Id
                        join u in userQuery
                        on o.CreatorId equals u.Id
                        join oi in oiQuery
                        on o.Id equals oi
                        into ois
                        where ois.Count() > 0

                        select new GetListDeliveryScheduleOutput
                        {
                            Id = o.Id,

                            FinalDeliveryDate = o.FinalDeliveryDate,
                            DeliveryNo = o.DeliveryNo,
                            Reference = o.Reference,
                            Status = o.Status,
                            StatusName = o.Status.ToString(),
                            ReceiveStatus = o.ReceiveStatus,
                            Customer = new CustomerSummaryOutput
                            {
                                Id = o.CustomerId,
                                CustomerName = c.CustomerName
                            },
                            User = new UserDto
                            {
                                Id = o.CreatorId.Value,
                                UserName = u.UserName
                            },
                            LocationName = l.LocationName,
                            LocationId = l.Id,
                        };

            var resultCount = await query.CountAsync();
            if (resultCount == 0) return new PagedResultDto<GetListDeliveryScheduleOutput>(resultCount, new List<GetListDeliveryScheduleOutput>());

            if (!string.IsNullOrEmpty(input.Sorting))
            {
                var sortingDirection = input.Sorting.EndsWith("DESC", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
                var sortingProperty = input.Sorting.ToLower().Replace("desc", "").Replace("asc", "").Trim();

                switch (sortingProperty)
                {
                    case "finaldeliverydate":
                        query = sortingDirection == "DESC" ? query.OrderByDescending(s => s.FinalDeliveryDate) : query.OrderBy(s => s.FinalDeliveryDate);
                        break;
                    case "initialdeliverydate":
                        query = sortingDirection == "DESC" ? query.OrderByDescending(s => s.InitialDeliveryDate) : query.OrderBy(s => s.InitialDeliveryDate);
                        break;
                    case "deliveryno":
                        query = sortingDirection == "DESC" ? query.OrderByDescending(s => s.DeliveryNo) : query.OrderBy(s => s.DeliveryNo);
                        break;
                    case "reference":
                        query = sortingDirection == "DESC" ? query.OrderByDescending(s => s.Reference) : query.OrderBy(s => s.Reference);
                        break;
                    case "receivestatus":
                        query = sortingDirection == "DESC" ? query.OrderByDescending(s => s.ReceiveStatus) : query.OrderBy(s => s.ReceiveStatus);
                        break;
                    case "statuscode":
                        query = sortingDirection == "DESC" ? query.OrderByDescending(s => s.StatusName) : query.OrderBy(s => s.StatusName);
                        break;
                    case "customer":
                        query = sortingDirection == "DESC" ? query.OrderByDescending(s => s.Customer.CustomerName) : query.OrderBy(s => s.Customer.CustomerName);
                        break;
                    default:
                        query = sortingDirection == "DESC" ? query.OrderByDescending(s => s.DeliveryNo) : query.OrderBy(s => s.DeliveryNo); // Default fallback
                        break;
                }
            }



            var @entities = await query.PageBy(input).ToListAsync();

            return new PagedResultDto<GetListDeliveryScheduleOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_DeliverySchedules_GetDetail)]
        public async Task<GetDetailDeliveryScheduleOutput> GetDetail(EntityDto<Guid> input)
        {
            var @entity = await _deliveryScheduleRepository.GetAll()
                .Include(u => u.Customer)
                .Include(u => u.Location)
                .Include(u => u.SaleOrder)
                .Where(d => d.Id == input.Id)
                .Select(d => new GetDetailDeliveryScheduleOutput
                {

                    CustomerCode = d.Customer.CustomerCode,
                    CustomerId = d.CustomerId,
                    CustomerName = d.Customer.CustomerName,
                    DeliveryNo = d.DeliveryNo,
                    FinalDeliveryDate = d.FinalDeliveryDate,
                    Id = d.Id,
                    InitialDeliveryDate = d.InitialDeliveryDate,
                    LocationId = d.LocationId,
                    LocationName = d.Location.LocationName,
                    ReceiveStatus = d.ReceiveStatus,
                    ReceiveStatusName = d.ReceiveStatus.ToString(),
                    SaleOrderId = d.SaleOrderId,
                    SaleOrderNo = d.SaleOrder != null ? d.SaleOrder.OrderNumber : null,
                    Status = d.Status,
                    StatusName = d.Status.ToString(),
                    Reference = d.Reference,
                    Memo = d.Memo,

                }).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var invoiceItemQty = from i in _invoiceItemRepository.GetAll()
                                          .Where(s => s.OrderItemId.HasValue)
                                          .AsNoTracking()
                                 join j in _journalRepository.GetAll()
                                           .Where(s => s.InvoiceId.HasValue)
                                           .Where(s => s.Status == TransactionStatus.Publish)
                                           .AsNoTracking()
                                 on i.InvoiceId equals j.InvoiceId
                                 select new
                                 {
                                     i.Qty,
                                     i.DeliverySchedulItemId
                                 };

            var issueItemQty = from i in _itemIssueItemRepository.GetAll()
                                         .Where(s => s.SaleOrderItemId.HasValue)
                                         .AsNoTracking()
                               join j in _journalRepository.GetAll()
                                         .Where(s => s.ItemIssueId.HasValue)
                                         .Where(s => s.Status == TransactionStatus.Publish)
                                         .AsNoTracking()
                               on i.ItemIssueId equals j.ItemIssueId
                               select new
                               {
                                   i.Qty,
                                   i.DeliverySchedulItemId
                               };

            entity.Items = await (from di in _deliveryScheduleItemRepository.GetAll()
                            .Include(u => u.Item)
                            .Where(s => s.DeliveryScheduleId == entity.Id)
                            .AsNoTracking()

                                  join ii in invoiceItemQty
                                      on di.Id equals ii.DeliverySchedulItemId
                                      into iiItems
                                  join si in issueItemQty
                                  on di.Id equals si.DeliverySchedulItemId
                                  into siItems
                                  let remainQty = di.Qty - iiItems.Sum(s => s.Qty) - siItems.Sum(s => s.Qty)

                                  select new DeliveryScheduleItemInput
                                  {
                                      DeliveryScheduleId = di.DeliveryScheduleId,
                                      Description = di.Description,
                                      Id = di.Id,
                                      ItemCode = di.Item.ItemCode,
                                      ItemId = di.ItemId,
                                      ItemName = di.Item.ItemName,
                                      Qty = di.Qty,
                                      SaleOrderItemId = di.SaleOrderItemId,
                                      Remain = remainQty

                                  }).ToListAsync();

            return entity;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_DeliverySchedules_Update)]
        public async Task<NullableIdDto<Guid>> Update(UpdateDeliveryScheduleInput input)
        {

            var tenantId = AbpSession.TenantId;
            var userId = AbpSession.UserId;


            if (input.LocationId == 0)
            {
                throw new UserFriendlyException(L("PleaseSelectLocation"));
            }


            if (input.IsConfirm == false)
            {
                var validateLockDate = await _lockRepository.GetAll()
                                    .Where(t => t.IsLock == true &&
                                    (t.LockDate.Value.Date >= input.DateCompare.Value.Date || t.LockDate.Value.Date >= input.InitialDeliveryDate.Date)
                                    && (t.LockKey == TransactionLockType.DeliverySchedule)).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            var @entity = await _deliveryScheduleManager.GetAsync(input.Id, true);

            await CheckClosePeriod(entity.InitialDeliveryDate, input.InitialDeliveryDate);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            #region update saleOrderItem           
            var deliveryItems = await _deliveryScheduleItemRepository.GetAll()
                                       .Include(s => s.ItemIssueItems)
                                       .Include(s => s.InvoiceItems)
                                       .Where(u => u.DeliveryScheduleId == entity.Id)
                                       .ToListAsync();

            var addDeliveryItems = input.Items.Where(s => !s.Id.HasValue);
            var updateDeliveryItems = input.Items.Where(s => s.Id.HasValue);
            var deleteDeliveryItems = deliveryItems.Where(u => !updateDeliveryItems.Any(i => i.Id == u.Id)).ToList();

            var deliveryItemIdsToDeletes = deleteDeliveryItems.Select(s => s.Id).ToList();
            await ValidateApplyOrderItems(input, deliveryItemIdsToDeletes);

            //validate delete items           
            if (deleteDeliveryItems.Any(s => s.ItemIssueItems.Any())) throw new UserFriendlyException(L("DeliveryAlreadyIssuedItems"));
            if (deleteDeliveryItems.Any(s => s.InvoiceItems.Any())) throw new UserFriendlyException(L("DeliveryAlreadyConvertToInvoice"));


            //validate modify items
            var updateItemsHaveLink = deliveryItems
                                      .Where(s => updateDeliveryItems.Any(d => d.Id == s.Id))
                                      .Where(s => s.ItemIssueItems.Any() || s.InvoiceItems.Any());

            if (updateItemsHaveLink.Any())
            {

                var returnList = new Dictionary<Guid?, decimal>();

                var itemIssueIds = updateItemsHaveLink.SelectMany(s => s.ItemIssueItems.Select(r => r.Id).Concat(s.InvoiceItems.Where(r => r.ItemIssueItemId.HasValue).Select(i => i.ItemIssueItemId.Value))).ToList();
                if (itemIssueIds.Any())
                {
                    var list = await _itemReceiptItemCustomerCreditRepository.GetAll()
                                        .Where(s => itemIssueIds.Contains(s.ItemIssueSaleItemId.Value) || (s.CustomerCreditItemId.HasValue && itemIssueIds.Contains(s.CustomerCreditItem.ItemIssueSaleItemId.Value)))
                                        .AsNoTracking()
                                        .Select(s => new
                                        {
                                            IssueId = s.ItemIssueSaleItemId.HasValue ? s.ItemIssueSaleItemId : s.CustomerCreditItem.ItemIssueSaleItemId,
                                            s.Qty
                                        })
                                        .ToListAsync();

                    returnList = list.GroupBy(g => g.IssueId).ToDictionary(k => k.Key, v => v.Sum(s => s.Qty));
                }

                foreach (var link in updateItemsHaveLink)
                {
                    var linkIssueItemIds = link.ItemIssueItems.Select(r => r.Id).Concat(link.InvoiceItems.Where(r => r.ItemIssueItemId.HasValue).Select(i => i.ItemIssueItemId.Value)).ToList();
                    var linkReturnQty = returnList.Where(s => linkIssueItemIds.Contains(s.Key.Value)).Sum(v => v.Value);

                    var applyQty = link.ItemIssueItems.Sum(s => s.Qty) + link.InvoiceItems.Sum(s => s.Qty) - linkReturnQty;

                    var updateItem = updateDeliveryItems.First(s => s.Id == link.Id);
                    if (updateItem.Qty < applyQty) throw new UserFriendlyException(L("DeliveryItemCannotChangeQtyLessThanIssueQty", applyQty));
                }
            }

            var createToDeliveryItems = new List<DeliveryScheduleItem>();
            var updateToDeliveryItems = new List<DeliveryScheduleItem>();
            foreach (var p in addDeliveryItems)
            {
                var item = DeliveryScheduleItem.Create(tenantId, userId.Value, entity.Id, p.Description, p.Qty, p.SaleOrderItemId, p.ItemId);
                createToDeliveryItems.Add(item);
            }

            foreach (var p in updateDeliveryItems)
            {
                var deliveryItem = deliveryItems.FirstOrDefault(u => u.Id == p.Id);
                if (deliveryItem == null) throw new UserFriendlyException(L("RecordNotFound"));
                //here is in only same purchaseOrder so no need to update purchaseOrder
                deliveryItem.Update(
                    userId.Value,
                    p.DeliveryScheduleId,
                    p.Description,
                    p.Qty,
                    p.SaleOrderItemId,
                    p.ItemId);
                updateToDeliveryItems.Add(deliveryItem);
            }
            #endregion          
            entity.Update(userId.Value, input.CustomerId, input.Reference, input.DeliveryNo, input.InitialDeliveryDate, input.FinalDeliveryDate, input.Memo, input.LocationId, input.SaleOrderId);
            if (createToDeliveryItems.Count > 0) await _deliveryScheduleItemRepository.BulkInsertAsync(createToDeliveryItems);
            if (updateToDeliveryItems.Count > 0) await _deliveryScheduleItemRepository.BulkUpdateAsync(updateToDeliveryItems);
            if (deleteDeliveryItems.Count > 0) await _deliveryScheduleItemRepository.BulkDeleteAsync(deleteDeliveryItems);
            await _deliveryScheduleRepository.BulkUpdateAsync(entity);
            await UpdateDeliveryInventoryStatus(entity.Id,false);
            if (input.IsConfirm)
            {
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_DeliverySchedules_Delete)]
        public async Task Delete(CarlEntityDto input)
        {
            //need to do
            var @entity = await _deliveryScheduleRepository.GetAll().Where(s => s.Id == input.Id).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            var deliveryScheduleItem = await _deliveryScheduleItemRepository.GetAll().Where(u => u.DeliveryScheduleId == entity.Id).ToListAsync();

            await ValidateIssueLink(input.Id);
            await ValidateInvoiceLink(input.Id);

            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
             .Where(t => t.LockKey == TransactionLockType.DeliverySchedule && t.IsLock == true && t.LockDate.Value.Date >= entity.InitialDeliveryDate.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            //var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.DeliverySchedule);

            //if (entity.DeliveryNo == auto.LastAutoSequenceNumber)
            //{
            //    var so = await _deliveryScheduleRepository.GetAll().Where(t => t.Id != entity.Id)
            //        .OrderByDescending(t => t.CreationTime).FirstOrDefaultAsync();
            //    if (so != null)
            //    {
            //        auto.UpdateLastAutoSequenceNumber(so.DeliveryNo);
            //    }
            //    else
            //    {
            //        auto.UpdateLastAutoSequenceNumber(null);
            //    }
            //    CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            //}


            await _deliveryScheduleItemRepository.BulkDeleteAsync(deliveryScheduleItem);
            CheckErrors(await _deliveryScheduleManager.RemoveAsync(@entity));

            if (input.IsConfirm)
            {
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_DeliverySchedules_UpdateToClose)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToClose(UpdateStatus input)
        {
            var @entity = await _deliveryScheduleManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            entity.UpdateStatusToClose();
            CheckErrors(await _deliveryScheduleManager.UpdateAsync(@entity));
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_DeliverySchedules_UpdateToDraft)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input)
        {
            var @entity = await _deliveryScheduleManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            await ValidateIssueLink(input.Id);
            await ValidateInvoiceLink(input.Id);

            entity.UpdateStatusToDraft();
            CheckErrors(await _deliveryScheduleManager.UpdateAsync(@entity));
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_DeliverySchedules_UpdateToPublish)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input)
        {
            var @entity = await _deliveryScheduleManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.UpdateStatusToPublish();
            CheckErrors(await _deliveryScheduleManager.UpdateAsync(@entity));
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_DeliverySchedules_UpdateToVoid)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input)
        {
            var @entity = await _deliveryScheduleManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            await ValidateIssueLink(input.Id);
            await ValidateInvoiceLink(input.Id);

            entity.UpdateStatusToVoid();
            CheckErrors(await _deliveryScheduleManager.UpdateAsync(@entity));
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }
        public async Task UpdateInventoryStatus(EntityDto<Guid> input)
        {
            await UpdateDeliveryStatusInventoryStatus(input.Id);
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_DeliverySchedules_Find)]
        public async Task<PagedResultDto<DeliveryScheduleHeaderOutput>> GetDeliveryShcedules(GetDeliveryScheduleHeaderListInput input)
        {
            var roundDigits = await _accountCycleRepository.GetAll().Select(t => t.RoundingDigit).FirstOrDefaultAsync();
            var userGroups = await GetUserGroupByLocation();
            var invoiceItemQuery = from iv in _invoiceItemRepository.GetAll()
                                              .Where(s => s.DeliverySchedulItemId.HasValue)
                                              .AsNoTracking()
                                   join ri in _itemReceiptItemCustomerCreditRepository.GetAll()
                                              .Where(s => s.ItemIssueSaleItemId.HasValue)
                                              .AsNoTracking()
                                   on iv.ItemIssueItemId equals ri.ItemIssueSaleItemId
                                   into r1

                                   join ci in _itemReceiptItemCustomerCreditRepository.GetAll()
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
                                             .AsNoTracking()
                                 join ri in _itemReceiptItemCustomerCreditRepository.GetAll()
                                            .Where(s => s.ItemIssueSaleItemId.HasValue)
                                            .AsNoTracking()
                                 on iv.Id equals ri.ItemIssueSaleItemId
                                 into r1

                                 join ci in _itemReceiptItemCustomerCreditRepository.GetAll()
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

            var deliveryItemQuery = from dvs in _deliveryScheduleItemRepository.GetAll()
                                             .AsNoTracking()

                                 join ivi in invoiceItemQuery
                                 on dvs.Id equals ivi.DeliverySchedulItemId into ivItems

                                 join iii in issueItemQuery
                                 on dvs.Id equals iii.DeliverySchedulItemId into iItems
                                 let issueQty = iItems == null ? 0 : iItems.Sum(s => s.Qty - s.ReturnQty)
                                 let invoiceQty = ivItems == null ? 0 : ivItems.Sum(s => s.Qty)

                                 select (new
                                 {
                                     DeliveryScheduleItemId = dvs.Id,
                                     remainQty = dvs.Qty - issueQty - invoiceQty,
                                     Qty = dvs.Qty,
                                     DeliveryShceduleId = dvs.DeliveryScheduleId
                                 });

            var deliveryQuery = _deliveryScheduleRepository.GetAll()
                         .Include(u => u.Customer)
                         .Where(s => s.Status == TransactionStatus.Publish)
                         .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId))
                         .WhereIf(input.Customers != null && input.Customers.Any(), c => input.Customers.Contains(c.CustomerId))
                         .WhereIf(!input.Filter.IsNullOrEmpty(),
                                u => u.DeliveryNo.ToLower().Contains(input.Filter.ToLower()) || u.Reference.ToLower().Contains(input.Filter.ToLower()))
                         .AsNoTracking()
                         .Select(ds => new
                         {
                             Customer = ObjectMapper.Map<CustomerSummaryOutput>(ds.Customer),
                             CustomerId = ds.CustomerId,
                             Memo = ds.Memo,
                             Id = ds.Id,
                             InitialDeliveryDate = ds.InitialDeliveryDate,
                             DeliveryNo = ds.DeliveryNo,
                             FinalDeliveryDate = ds.FinalDeliveryDate,
                             Reference = ds.Reference,
                             SaleOrderId = ds.SaleOrderId,
                         });

            var query = (from ds in deliveryQuery

                         join dsi in deliveryItemQuery
                         on ds.Id equals dsi.DeliveryShceduleId
                         into poItems

                         where poItems.Any(s => s.remainQty > 0)

                         select new DeliveryScheduleHeaderOutput
                         {

                             Customer = ds.Customer,
                             CustomerId = ds.CustomerId,
                             Memo = ds.Memo,
                             Id = ds.Id,
                             FinalDeliveryDate = ds.FinalDeliveryDate,
                             DeliveryNo = ds.DeliveryNo,
                             CountDeliveryItems = poItems.Count(),
                             Reference = ds.Reference,
                             InitialDeliveryDate = ds.InitialDeliveryDate,
                             SaleOrderId = ds.SaleOrderId.Value,
                             
                         });
            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(t => t.FinalDeliveryDate).PageBy(input).ToListAsync();
            return new PagedResultDto<DeliveryScheduleHeaderOutput>(resultCount, entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_DeliverySchedules_Find)]
        public async Task<GetListDeliveryItemDetail> GetItemDeliverySchedules(EntityDto<Guid> input)
        {
            var roundDigits = await _accountCycleRepository.GetAll().Select(t => t.RoundingDigit).FirstOrDefaultAsync();
            var @entity = await _deliveryScheduleManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var invoiceItemQuery = from iv in _invoiceItemRepository.GetAll()
                                             .Where(s => s.DeliverySchedulItemId.HasValue)
                                             .Where(s => s.DeliveryScheduleItem.DeliveryScheduleId == input.Id)
                                             .AsNoTracking()
                                   join ri in _itemReceiptItemCustomerCreditRepository.GetAll()
                                              .Where(s => s.ItemIssueSaleItemId.HasValue)
                                              .AsNoTracking()
                                   on iv.ItemIssueItemId equals ri.ItemIssueSaleItemId
                                   into r1

                                   join ci in _itemReceiptItemCustomerCreditRepository.GetAll()
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
                                             .Where(s => s.DeliveryScheduleItem.DeliveryScheduleId == input.Id)
                                             .AsNoTracking()
                                 join ri in _itemReceiptItemCustomerCreditRepository.GetAll()
                                            .Where(s => s.ItemIssueSaleItemId.HasValue)
                                            .AsNoTracking()
                                 on iv.Id equals ri.ItemIssueSaleItemId
                                 into r1

                                 join ci in _itemReceiptItemCustomerCreditRepository.GetAll()
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

            var deliveryScheduleItem = await (from si in _deliveryScheduleItemRepository.GetAll()
                                             .Include(u => u.Item)  
                                             .Include(u=>u.SaleOrderItem.SaleOrder)
                                             .Where(s => s.DeliverySchedule.Status == TransactionStatus.Publish)                                           
                                             .Where(u => u.DeliveryScheduleId == entity.Id).AsNoTracking()
                                        join ivi in invoiceItemQuery
                                        on si.Id equals ivi.DeliverySchedulItemId
                                        into ivItems
                                        join iii in issueItemQuery
                                        on si.Id equals iii.DeliverySchedulItemId
                                        into iItems
                                        let ivQty = ivItems == null ? 0 : ivItems.Sum(s => s.Qty - s.ReturnQty)
                                        let iiQty = iItems == null ? 0 : iItems.Sum(s => s.Qty - s.ReturnQty)                                     
                                        let remainQty = si.Qty - ivQty - iiQty
                                        where remainQty > 0

                                        select new DeliveryItemSummaryOut
                                        {
                                            Id = si.Id,
                                            Description = si.Description,                                         
                                            Item = ObjectMapper.Map<ItemSummaryDetailOutput>(si.Item),
                                            ItemId = si.ItemId,
                                            Remain = remainQty,
                                            Qty = si.Qty,
                                            SaleOrderItemId = si.SaleOrderItemId.Value,
                                            SaleOrderId = si.SaleOrderItem.SaleOrderId,
                                            SaleOrderNumber = si.SaleOrderItem.SaleOrder.OrderNumber,
                                            SaleOrderReference = si.SaleOrderItem.SaleOrder.Reference,
                                            Price = si.SaleOrderItem.UnitCost,
                                        }).ToListAsync();
            var result = ObjectMapper.Map<GetListDeliveryItemDetail>(@entity);
            result.Items = ObjectMapper.Map<List<DeliveryItemSummaryOut>>(deliveryScheduleItem);
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_DeliverySchedules_Find)]
        public async Task<GetListDeliveryItemDetail> GetDeliveryScheduleForView(EntityDto<Guid> input)
        {
            var roundDigits = await _accountCycleRepository.GetAll().Select(t => t.RoundingDigit).FirstOrDefaultAsync();
            var @entity = await _deliveryScheduleManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var invoiceItemQuery = from iv in _invoiceItemRepository.GetAll()
                                              .Where(s => s.DeliverySchedulItemId.HasValue)
                                              .Where(s => s.DeliveryScheduleItem.DeliveryScheduleId == input.Id)
                                              .AsNoTracking()
                                   join ri in _itemReceiptItemCustomerCreditRepository.GetAll()
                                              .Where(s => s.ItemIssueSaleItemId.HasValue)
                                              .AsNoTracking()
                                   on iv.ItemIssueItemId equals ri.ItemIssueSaleItemId
                                   into r1

                                   join ci in _itemReceiptItemCustomerCreditRepository.GetAll()
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
                                             .Where(s => s.DeliveryScheduleItem.DeliveryScheduleId == input.Id)
                                             .AsNoTracking()
                                 join ri in _itemReceiptItemCustomerCreditRepository.GetAll()
                                            .Where(s => s.ItemIssueSaleItemId.HasValue)
                                            .AsNoTracking()
                                 on iv.Id equals ri.ItemIssueSaleItemId
                                 into r1

                                 join ci in _itemReceiptItemCustomerCreditRepository.GetAll()
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

            var @saleOrderItem = await (from si in _deliveryScheduleItemRepository.GetAll()
                                             .Include(u => u.Item.InventoryAccount)                                          
                                             .Include(u => u.Item.PurchaseAccount)
                                             .Include(u => u.SaleOrderItem.SaleOrder)
                                             .Where(s => s.DeliverySchedule.Status == TransactionStatus.Publish)                                          
                                             .Where(u => u.DeliveryScheduleId == entity.Id).AsNoTracking()

                                        join ivi in invoiceItemQuery
                                        on si.Id equals ivi.DeliverySchedulItemId
                                        into ivItems

                                        join iii in issueItemQuery
                                        on si.Id equals iii.DeliverySchedulItemId
                                        into iItems
                                        let ivQty = ivItems == null ? 0 : ivItems.Sum(s => s.Qty - s.ReturnQty)
                                        let iiQty = iItems == null ? 0 : iItems.Sum(s => s.Qty - s.ReturnQty)                                      
                                        let remainQty = si.Qty - ivQty - iiQty 

                                        select new DeliveryItemSummaryOut
                                        {
                                            Id = si.Id,
                                            Description = si.Description,                                       
                                            Item = ObjectMapper.Map<ItemSummaryDetailOutput>(si.Item),
                                            ItemId = si.ItemId,
                                            Remain = remainQty,
                                            Qty = si.Qty,
                                           SaleOrderItemId = si.SaleOrderItemId.Value,
                                           SaleOrderNumber = si.SaleOrderItem.SaleOrder.OrderNumber,
                                           SaleOrderReference = si.SaleOrderItem.SaleOrder.Reference,
                                           DeliveryNo = si.DeliverySchedule.DeliveryNo
                                        }).ToListAsync();
            var result = ObjectMapper.Map<GetListDeliveryItemDetail>(@entity);          
            result.Items = ObjectMapper.Map<List<DeliveryItemSummaryOut>>(saleOrderItem);
            return result;
        }



        //[AbpAuthorize(AppPermissions.Pages_Tenant_Customer_SaleOrder_Create)]
        //public async Task<FileDto> CreateAndPrint(CreateDeliveryScheduleInput input)
        //{
        //    var saveSaleOrder = await SaveDeliverySchedule(input);
        //    await CurrentUnitOfWork.SaveChangesAsync();
        //    var result = new EntityDto<Guid>() { Id = saveSaleOrder.Id.Value };
        //    var print = await Print(result);
        //    return print;
        //}

        //public async Task<FileDto> Print(EntityDto<Guid> input)
        //{
        //    return await PrintTemplate(input);
        //}


        //[UnitOfWork(IsDisabled = true)]
        //private async Task<FileDto> PrintTemplate(EntityDto<Guid> input)
        //{

        //    var companyInfo = await _tenantManager.GetAsync(AbpSession.GetTenantId());

        //    var invoice = await GetDetail(input);
        //    var accountCyclePeriod = await GetPreviousRoundingCloseCyleAsync(invoice.InitialDeliveryDate);
        //    var template = await this.GetInovieTemplateHtml(invoice.SaleTransactionTypeId);
        //    var exportHtml = EmbedBase64Fonts(template.Html);

        //    return await Task.Run(async () =>
        //    {
        //        var dateFormat = FormatDate(invoice.OrderDate, "dd-MM-yyyy");
        //        var canSeePrice = IsGranted(AppPermissions.Pages_Tenant_Customer_SaleOrder_CanSeePrice);

        //        if (template.ShowDetail)
        //        {
        //            var rowStart = FindTag(exportHtml, "<tr", " detail-body-row");
        //            if (!string.IsNullOrWhiteSpace(rowStart.Key))
        //            {
        //                var detialRow = GetOuterHtml(rowStart.Key, "</tr>", exportHtml.Substring(rowStart.Value));
        //                if (!string.IsNullOrEmpty(detialRow))
        //                {
        //                    int n = 1;
        //                    var detail = string.Empty;
        //                    foreach (var i in invoice.SaleOrderItems)
        //                    {
        //                        detail += detialRow.Replace("@No", $"{n}")
        //                                           .Replace("@ItemCode", i.Item.ItemCode)
        //                                           .Replace("@ItemName", i.Item.ItemName)
        //                                           .Replace("@Description", i.Description)
        //                                           .Replace("@Qty", FormatNumberCurrency(i.Qty, accountCyclePeriod.RoundingDigit))
        //                                           .Replace("@UnitPrice", !canSeePrice ? "..." : FormatNumberCurrency(i.MultiCurrencyUnitCost, accountCyclePeriod.RoundingDigitUnitCost))
        //                                           .Replace("@DiscountRate", !canSeePrice ? "..." : $"{FormatNumberCurrency(i.DiscountRate * 100, accountCyclePeriod.RoundingDigit)}%")
        //                                           .Replace("@LineTotal", !canSeePrice ? "..." : FormatNumberCurrency(i.MultiCurrencyTotal, accountCyclePeriod.RoundingDigit));
        //                        n++;
        //                    }

        //                    exportHtml = exportHtml.Replace(detialRow, detail);
        //                }
        //            }
        //        }


        //        var logo = "";
        //        if (companyInfo.LogoId.HasValue)
        //        {
        //            var image = await _fileUploadManager.DownLoad(companyInfo.Id, companyInfo.LogoId.Value);

        //            if (image != null)
        //            {
        //                var base64Str = StreamToBase64(image.Stream);
        //                logo = $"<img src=\"data:{image.ContentType};base64, {base64Str}\" alt=\"logo\" style=\"max-height: 90px; max-width: 150px; display: block;\"/>";
        //            }
        //        }

        //        var shippingAddress = "";
        //        if (invoice.ShippingAddress != null)
        //        {
        //            shippingAddress = $"{(invoice.ShippingAddress.Street.IsNullOrWhiteSpace() ? "....." : invoice.ShippingAddress.Street)}, " +
        //            $"{(invoice.ShippingAddress.PostalCode.IsNullOrWhiteSpace() ? "....." : invoice.ShippingAddress.PostalCode) }, " +
        //            $"{(invoice.ShippingAddress.CityTown.IsNullOrWhiteSpace() ? "....." : invoice.ShippingAddress.CityTown)}, " +
        //            $"{(invoice.ShippingAddress.Province.IsNullOrWhiteSpace() ? "....." : invoice.ShippingAddress.Province)}, " +
        //            $"{(invoice.ShippingAddress.Country.IsNullOrWhiteSpace() ? "....." : invoice.ShippingAddress.Country)}";
        //        }

        //        exportHtml = exportHtml.Replace("@Logo", logo)
        //                               .Replace("@CompanyName", companyInfo.Name)
        //                               .Replace("@CompanyAddress", $"{companyInfo.LegalAddress?.Street} {companyInfo.LegalAddress?.CityTown} {companyInfo.LegalAddress?.Province}")
        //                               .Replace("@CustomerName", invoice.Customer?.CustomerName)
        //                               .Replace("@CustomerPhone", invoice.Customer?.PhoneNumber)
        //                               .Replace("@ShippingAddress", shippingAddress)
        //                               .Replace("@InvoiceDate", dateFormat)
        //                               .Replace("@ETADate", FormatDate(invoice.ETD, "dd-MM-yyyy"))
        //                               .Replace("@InvoiceNo", invoice.OrderNumber)
        //                               .Replace("@Reference", invoice.Reference)
        //                               .Replace("@Currency", invoice.MultiCurrency?.Code)
        //                               .Replace("@Memo", invoice.Memo == null ? "" : invoice.Memo.Replace(Environment.NewLine, "<br>").Replace("\n", "<br>"))
        //                               .Replace("@UserName", invoice.UserName)
        //                               .Replace("@SubTotal", !canSeePrice ? "..." : FormatNumberCurrency(invoice.MultiCurrencySubTotal, accountCyclePeriod.RoundingDigit));


        //        HtmlToPdfConverter htmlToPdfConverter = GetInitPDF();
        //        // Set if the fonts are embedded in the generated PDF document
        //        // Leave it not set to embed the fonts in the generated PDF document    
        //        htmlToPdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Portrait;
        //        htmlToPdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
        //        htmlToPdfConverter.PdfDocumentOptions.LeftMargin = 20;
        //        htmlToPdfConverter.PdfDocumentOptions.RightMargin = 20;
        //        htmlToPdfConverter.PdfDocumentOptions.TopMargin = 20;
        //        htmlToPdfConverter.PdfDocumentOptions.BottomMargin = 20;


        //        byte[] outPdfBuffer = htmlToPdfConverter.ConvertHtml(exportHtml, "index.html");


        //        var result = new FileDto();
        //        result.FileName = $"Invoice_{dateFormat}.pdf";
        //        result.FileToken = $"{Guid.NewGuid()}.pdf";
        //        result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
        //        result.FileType = MimeTypeNames.ApplicationPdf;

        //        await _fileStorageManager.UploadTempFile(result.FileToken, outPdfBuffer);

        //        return result;

        //    });
        //}


        //[UnitOfWork(IsDisabled = true)]
        //private async Task<InvoiceTemplateWithOptionResultOutput> GetInovieTemplateHtml(long? saleTypeId)
        //{
        //    InvoiceTemplateMap templateMap = null;

        //    using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
        //    {
        //        using (_unitOfWorkManager.Current.SetTenantId(AbpSession.TenantId))
        //        {
        //            templateMap = await _invoiceTemplateMapRepository
        //                            .GetAll()
        //                            .Include(s => s.InvoiceTemplate)
        //                            .AsNoTracking()
        //                            .Where(s => s.TemplateType == InvoiceTemplateType.SaleOrder)
        //                            .Where(s => s.SaleTypeId == saleTypeId || !s.SaleTypeId.HasValue)
        //                            .OrderBy(s => s.SaleTypeId.HasValue ? 0 : 1)
        //                            .FirstOrDefaultAsync();
        //        }
        //    }


        //    if (templateMap == null) return await GetDefaultTemplateHtml("saleOrderTemplate.html");

        //    GalleryDownloadOutput fileDownload = await _fileUploadManager.DownLoad(AbpSession.TenantId, templateMap.InvoiceTemplate.GalleryId);

        //    var templateHtml = string.Empty;
        //    using (StreamReader r = new StreamReader(fileDownload.Stream))
        //    {
        //        templateHtml = r.ReadToEnd();
        //    }

        //    return new InvoiceTemplateWithOptionResultOutput
        //    {
        //        Html = templateHtml,
        //        ShowDetail = templateMap.InvoiceTemplate.ShowDetail,
        //        ShowSummary = templateMap.InvoiceTemplate.ShowSummary
        //    };

        //}

        //[UnitOfWork(IsDisabled = true)]
        //private async Task<InvoiceTemplateWithOptionResultOutput> GetDefaultTemplateHtml(string templateFileName)
        //{
        //    var templateHtml = await _fileStorageManager.GetTemplate(templateFileName);

        //    return new InvoiceTemplateWithOptionResultOutput { Html = templateHtml, ShowDetail = true };

        //}

        #region helper
        private async Task<NullableIdDto<Guid>> SaveDeliverySchedule(CreateDeliveryScheduleInput input)
        {
            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                   .Where(t => t.LockKey == TransactionLockType.DeliverySchedule && t.IsLock == true && t.LockDate.Value.Date >= input.InitialDeliveryDate.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            if (input.LocationId == 0)
            {
                throw new UserFriendlyException(L("PleaseSelectLocation"));
            }

            await ValidateApplyOrderItems(input, null);

            //var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.DeliverySchedule);

            //if (auto.CustomFormat == true)
            //{
            //    var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
            //                   auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
            //    input.DeliveryNo = newAuto;
            //    auto.UpdateLastAutoSequenceNumber(newAuto);
            //    CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            //}

            var saleOrder = await _saleOrderRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == input.SaleOrderId);
            if (saleOrder == null) throw new UserFriendlyException(L("PleaseSelect", L("SaleOrder")));

            var latestDelivery = await _deliveryScheduleRepository.GetAll()
                                       .Where(s => s.SaleOrderId == input.SaleOrderId)
                                       .OrderByDescending(o => o.DeliveryNo)
                                       .FirstOrDefaultAsync();

            if(latestDelivery != null)
            {
                var index = latestDelivery.DeliveryNo.Replace($"{saleOrder.OrderNumber}/", "");
                var nexIndex = Convert.ToInt32(index) + 1;
                input.DeliveryNo = $"{saleOrder.OrderNumber}/{nexIndex.ToString().PadLeft(2,'0')}";
            }
            else
            {
                input.DeliveryNo = $"{saleOrder.OrderNumber}/01";
            }

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = DeliverySchedule.Create(
                tenantId, userId,
                input.CustomerId,
                input.Reference,
                input.DeliveryNo,
                input.InitialDeliveryDate,
                input.FinalDeliveryDate,
                input.Memo,
                input.LocationId,
                input.SaleOrderId
                );
            entity.UpdateReceiveStatusToPending();
            CheckErrors(await _deliveryScheduleManager.CreateAsync(@entity, false));

            #region DeliveryItem   
            var createDeliveryScheduleItem = new List<DeliveryScheduleItem>();
            foreach (var item in input.Items)
            {
                var @deliveryItem = DeliveryScheduleItem.Create(
                    tenantId,
                    userId,
                    entity.Id,
                    item.Description,
                    item.Qty,
                    item.SaleOrderItemId,
                    item.ItemId
                    );
                //  createDeliveryScheduleItem.Add(@deliveryItem);
                base.CheckErrors(await _deliveryScheduleItemManager.CreateAsync(@deliveryItem));
            }
            #endregion

            if (input.IsConfirm)
            {
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }

            //  await _deliveryScheduleItemRepository.BulkInsertAsync(createDeliveryScheduleItem);

            #region update SO


            #endregion
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }
        private async Task ValidateApplyOrderItems(CreateDeliveryScheduleInput input, List<Guid> deliveryItemIdsToDeletes)
        {

            var itemApplyOrder = input.Items
                                 .Where(s => s.SaleOrderItemId.HasValue)
                                 .GroupBy(s => s.SaleOrderItemId);

            if (!itemApplyOrder.Any()) return;

            var orderItemIds = itemApplyOrder.Select(s => s.Key.Value).ToList();
            var exceptDeliveryIds = input.Items
                                        .Where(s => s.SaleOrderItemId.HasValue)
                                        .Where(s => s.Id.HasValue)
                                        .WhereIf(deliveryItemIdsToDeletes != null && deliveryItemIdsToDeletes.Count > 0, s => s.Id.HasValue && !deliveryItemIdsToDeletes.Contains(s.Id.Value))
                                        .Select(s => s.Id.Value)
                                        .ToList();

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


            var deliveryItemQuery = from dv in _deliveryScheduleItemRepository.GetAll()
                                        .Where(s => s.SaleOrderItemId.HasValue)
                                        .Where(s => orderItemIds.Contains(s.SaleOrderItemId.Value))
                                         .Where(s => !exceptDeliveryIds.Contains(s.Id))
                                        .AsNoTracking()
                                    select new
                                    {
                                        OrderItemId = dv.SaleOrderItemId,
                                        Qty = dv.Qty,
                                        ReturnQty = 0
                                    };

            var invoiceItemQuery = from iv in _invoiceItemRepository.GetAll()
                                             .Where(s => s.OrderItemId.HasValue)
                                             .Where(s => orderItemIds.Contains(s.OrderItemId.Value))
                                                .Where(s => !exceptInvoiceItemIds.Contains(s.Id))
                                             .AsNoTracking()
                                   join ri in _itemReceiptItemCustomerCreditRepository.GetAll()
                                              .Where(s => s.ItemIssueSaleItemId.HasValue)
                                              .AsNoTracking()
                                   on iv.ItemIssueItemId equals ri.ItemIssueSaleItemId
                                   into r1

                                   join ci in _itemReceiptItemCustomerCreditRepository.GetAll()
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
                                 join ri in _itemReceiptItemCustomerCreditRepository.GetAll()
                                            .Where(s => s.ItemIssueSaleItemId.HasValue)
                                            .AsNoTracking()
                                 on iv.Id equals ri.ItemIssueSaleItemId
                                 into r1

                                 join ci in _itemReceiptItemCustomerCreditRepository.GetAll()
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
                                     issueQty = q1 + q2 + q3,
                                 };

            var saleOrderItems = await saleOrderQuery.ToListAsync();


            foreach (var g in itemApplyOrder)
            {
                var orderItem = saleOrderItems.FirstOrDefault(s => s.Id == g.Key);
                if (orderItem == null) throw new UserFriendlyException("RecordNotFound");

                var remainQty = orderItem.Qty - orderItem.issueQty;

                foreach (var i in g)
                {
                    if (i.Qty > remainQty) throw new UserFriendlyException(L("RemainQtyIsLessThanDeliveryQtyCannotApply", remainQty, i.Qty));
                    remainQty -= i.Qty;
                }
            }
        }

        private async Task ValidateIssueLink(Guid deliveryId)
        {

            var validateByIssue = await _deliveryScheduleItemRepository.GetAll()
                                        .AsNoTracking()
                                        .Where(s => s.DeliveryScheduleId == deliveryId)
                                        .Where(s => s.ItemIssueItems.Any())
                                        .AnyAsync();

            if (validateByIssue)
            {
                throw new UserFriendlyException(L("DeliveryAlreadyIssuedItems"));
            }
        }

        private async Task ValidateInvoiceLink(Guid deliveryId)
        {

            var validateByIssue = await _deliveryScheduleItemRepository.GetAll()
                                        .AsNoTracking()
                                        .Where(s => s.DeliveryScheduleId == deliveryId)
                                        .Where(s => s.InvoiceItems.Any())
                                        .AnyAsync();

            if (validateByIssue)
            {
                throw new UserFriendlyException(L("DeliveryAlreadyConvertToInvoice"));
            }
        }


        private async Task UpdateDeliveryStatusInventoryStatus(Guid deliveryId)
        {
            var entity = await _deliveryScheduleRepository.FirstOrDefaultAsync(s => s.Id == deliveryId);

            if (entity == null) throw new UserFriendlyException("RecordNotFound");

            var saleOrderItems = await _deliveryScheduleItemRepository.GetAll()
                                        .Include(s => s.ItemIssueItems)
                                        .Include(s => s.InvoiceItems)
                                        .Where(s => s.DeliveryScheduleId == deliveryId)
                                        .AsNoTracking()
                                        .ToListAsync();

            if (saleOrderItems.All(s => s.InvoiceItems.Where(r => r.ItemIssueItemId.HasValue).Count() == 0 && s.ItemIssueItems.Count() == 0))
            {
                entity.UpdateReceiveStatusToPending();
            }
            else
            {
                var totalOrderQty = saleOrderItems.Sum(s => s.Qty);
                var totalIssueQty = saleOrderItems.Sum(s => s.InvoiceItems.Where(r => r.ItemIssueItemId.HasValue).Sum(i => i.Qty) + s.ItemIssueItems.Sum(r => r.Qty));
                var totalReturnQty = 0m;

                var itemIssueIds = saleOrderItems.SelectMany(s => s.ItemIssueItems.Select(r => r.Id).Concat(s.InvoiceItems.Where(r => r.ItemIssueItemId.HasValue).Select(i => i.ItemIssueItemId.Value))).ToList();
                if (itemIssueIds.Any())
                {
                    totalReturnQty = await _itemReceiptItemCustomerCreditRepository.GetAll()
                                           .Where(s => itemIssueIds.Contains(s.ItemIssueSaleItemId.Value) || (s.CustomerCreditItemId.HasValue && itemIssueIds.Contains(s.CustomerCreditItem.ItemIssueSaleItemId.Value)))
                                           .AsNoTracking()
                                           .Select(s => s.Qty)
                                           .SumAsync();
                }


                if (totalOrderQty - totalIssueQty + totalReturnQty == 0)
                {
                    entity.UpdateReceiveStatusToReceiveAll();
                }
                else if (totalIssueQty > 0 && totalIssueQty == totalReturnQty)
                {
                    entity.UpdateReceiveStatusToPending();
                }
                else
                {
                    entity.UpdateReceiveStatusToPartial();
                }
            }

            //Update Issue Count
            var issueCount = saleOrderItems.SelectMany(s => s.ItemIssueItems.Select(b => b.ItemIssueId)).GroupBy(g => g).Count();
            var invoiceCount = saleOrderItems.SelectMany(s => s.InvoiceItems.Where(b => b.ItemIssueItemId.HasValue).Select(b => b.InvoiceId)).GroupBy(g => g).Count();


            await _deliveryScheduleManager.UpdateAsync(entity);
        }


        #endregion
    }
}
