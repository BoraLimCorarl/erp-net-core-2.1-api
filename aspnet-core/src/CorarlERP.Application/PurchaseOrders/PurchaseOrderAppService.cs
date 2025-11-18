using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.PurchaseOrders.Dto;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Abp.Authorization;
using CorarlERP.Authorization;
using Abp.Timing;
using CorarlERP.Vendors.Dto;
using CorarlERP.Items;
using CorarlERP.Vendors;
using CorarlERP.Currencies.Dto;
using CorarlERP.Currencies;
using CorarlERP.Items.Dto;
using CorarlERP.Taxes;
using CorarlERP.Taxes.Dto;
using CorarlERP.Journals.Dto;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.Bills;
using CorarlERP.ItemReceipts;
using CorarlERP.AutoSequences;
using CorarlERP.Dto;
using CorarlERP.Locations.Dto;
using CorarlERP.UserGroups;
using CorarlERP.AccountCycles;
using CorarlERP.Locks;
using CorarlERP.Common.Dto;
using CorarlERP.LockTransactions.Dto;
using Microsoft.AspNetCore.Mvc;
using CorarlERP.Journals;
using CorarlERP.VendorCredit;
using CorarlERP.Exchanges.Dto;

namespace CorarlERP.PurchaseOrders
{
    public class PurchaseOrderAppService : CorarlERPAppServiceBase, IPurchaseOrderAppService
    {
        private readonly IPurchaseOrderManager _purchaseOrderManager;
        private readonly IRepository<PurchaseOrder, Guid> _purchaseOrderRepository;
        private readonly IPurchaseOrderItemManager _purchaseOrderItemManager;
        private readonly IRepository<PurchaseOrderItem, Guid> _purchaseOrderItemRepository;
        private readonly IRepository<Item, Guid> _itemRepository;
        private readonly IRepository<Vendor, Guid> _vendorRepository;
        private readonly IRepository<Currency, long> _currencyRepository;
        private readonly IRepository<Tax, long> _taxRepository;
        private readonly IBillItemManager _billItemManager;
        private readonly IRepository<BillItem, Guid> _billItemRepository;
        private readonly IRepository<ItemReceiptItem, Guid> _itemReceiptItemRepository;
        private readonly IItemReceiptItemManager _itemReceiptItemManager;
        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly IRepository<AccountCycle, long> _accountCycleRepository;
        private readonly IRepository<Lock, long> _lockRepository;
        private readonly IRepository<UserGroupMember, Guid> _userGroupMemberRepository;
        private readonly IRepository<Journal, Guid> _journalRepository;
        private readonly ICorarlRepository<ItemLot, Guid> _itemLotRepository;
        private readonly ICorarlRepository<PurchaseOrderExchangeRate, Guid> _exchangeRateRepository;
        public PurchaseOrderAppService(IPurchaseOrderManager purchaseOrderManager,
            ICorarlRepository<PurchaseOrderExchangeRate, Guid> exchangeRateRepository,
                IBillItemManager billItmeManager,
                IItemReceiptItemManager itemReceiptItemManager,
                IPurchaseOrderItemManager purchaseOrderItemManager,
                ICurrencyManager currencyManager, ITaxManager taxManager,
                IRepository<PurchaseOrder, Guid> purchaseOrderRepository,
                IRepository<PurchaseOrderItem, Guid> purchaseOrderItemRepository,
                IRepository<Item, Guid> itemRepository,
                IRepository<Vendor, Guid> vendorRepository,
                IRepository<Currency, long> currencyRepository,
                IRepository<Tax, long> taxRepository,
                IRepository<BillItem, Guid> billItemRepository,
                IRepository<ItemReceiptItem, Guid> itemReceiptItemRepository,
                IRepository<AutoSequence, Guid> autoSequenceRepository,
                IAutoSequenceManager autoSequenceManager,
                IRepository<Locations.Location, long> locationRepository,
                IRepository<UserGroupMember, Guid> userGroupMemberRepository,
                IRepository<AccountCycle, long> accountCycleRepository,
                IRepository<Lock, long> lockRepository,
                IRepository<Journal, Guid> journalRepository,
                ICorarlRepository<ItemLot, Guid> itemLotRepository
            ) : base(accountCycleRepository,userGroupMemberRepository, locationRepository)
        {
            _accountCycleRepository = accountCycleRepository;
            _purchaseOrderManager = purchaseOrderManager;
            _purchaseOrderRepository = purchaseOrderRepository;
            _purchaseOrderItemManager = purchaseOrderItemManager;
            _purchaseOrderItemRepository = purchaseOrderItemRepository;
            _itemRepository = itemRepository;
            _vendorRepository = vendorRepository;
            _currencyRepository = currencyRepository;
            _taxRepository = taxRepository;
            _billItemManager = billItmeManager;
            _billItemRepository = billItemRepository;
            _itemReceiptItemManager = itemReceiptItemManager;
            _itemReceiptItemRepository = itemReceiptItemRepository;
            _autoSequenceRepository = autoSequenceRepository;
            _autoSequenceManager = autoSequenceManager;
            _lockRepository = lockRepository;
            _journalRepository = journalRepository;
            _itemLotRepository = itemLotRepository;
            _exchangeRateRepository = exchangeRateRepository;
        }

        void ValidateExchangeRate(CreatePurchaseOrderInput input)
        {
            if (!input.UseExchangeRate || input.CurrencyId == input.MulitCurrencyId) return;

            if (input.ExchangeRate == null) throw new UserFriendlyException(L("IsRequired", L("ExchangeRate")));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_PurchaseOrder_Create)]
        public async Task<NullableIdDto<Guid>> Create(CreatePurchaseOrderInput input)
        {

            if(input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
               .Where(t => t.LockKey == TransactionLockType.PurchaseOrder && t.IsLock == true && t.LockDate.Value.Date >= input.OrderDate.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
           
            if (input.LocationId == null || input.LocationId == 0)
            {
                throw new UserFriendlyException(L("PleaseSelectLocation"));
            }

            ValidateExchangeRate(input);


            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.PurchaseOrder);

            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.OrderNumber = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            if(input.MulitCurrencyId == null || input.MulitCurrencyId == input.CurrencyId)
            {
                input.MulitCurrencyId = input.CurrencyId;
                input.MultiCurrencySubTotal = input.SubTotal;
                input.MultiCurrencyTotal = input.MultiCurrencyTotal;
            }

            var @entity = PurchaseOrder.Create(tenantId, userId, input.VendorId, input.ShippingAddress,
                input.BillingAddress, input.SameAsShippingAddress, input.Reference, input.CurrencyId,
                input.OrderNumber, input.OrderDate, input.Memo, input.Tax, input.Total, input.SubTotal,
                input.Status, input.ETA, input.LocationId,input.MultiCurrencySubTotal,input.MultiCurrencyTotal,
                input.MulitCurrencyId,input.MultiCurrencyTax, input.UseExchangeRate);
            //add status and update receivestatus to pending
            entity.UpdateReceiveStatusToPending();
            entity.SetApprovalStatus(ApprovalStatus.Recorded);

            CheckErrors(await _purchaseOrderManager.CreateAsync(@entity, auto.RequireReference));

            #region purchaseOrderItem           
            foreach (var purchaseOrderItem in input.PurchaseOrderItems)
            {

                if(input.MulitCurrencyId == null || input.CurrencyId == input.MulitCurrencyId)
                {
                    purchaseOrderItem.MultiCurrencyTotal = purchaseOrderItem.Total;
                    purchaseOrderItem.MultiCurrencyUnitCost = purchaseOrderItem.UnitCost;
                }
                var @PurchaseOrderItem = PurchaseOrders.PurchaseOrderItem.Create(tenantId, userId, purchaseOrderItem.ItemId, entity.Id, purchaseOrderItem.TaxId, purchaseOrderItem.TaxRate, purchaseOrderItem.Description, purchaseOrderItem.Unit, purchaseOrderItem.UnitCost, purchaseOrderItem.DiscountRate, purchaseOrderItem.Total,purchaseOrderItem.MultiCurrencyTotal,purchaseOrderItem.MultiCurrencyUnitCost);

                base.CheckErrors(await _purchaseOrderItemManager.CreateAsync(@PurchaseOrderItem));
            }
            #endregion

            if (input.UseExchangeRate && input.CurrencyId != input.MulitCurrencyId)
            {
                var exchange = PurchaseOrderExchangeRate.Create(tenantId, userId, entity.Id, input.ExchangeRate.FromCurrencyId, input.ExchangeRate.ToCurrencyId, input.ExchangeRate.Bid, input.ExchangeRate.Ask);
                await _exchangeRateRepository.InsertAsync(exchange);
            }

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.PurchaseOrder, };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }

            await CurrentUnitOfWork.SaveChangesAsync();
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_PurchaseOrder_Delete)]
        public async Task Delete(CarlEntityDto input)
        {
            var @entity = await _purchaseOrderManager.GetAsync(input.Id, true);

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.PurchaseOrder);
            if (entity.OrderNumber == auto.LastAutoSequenceNumber)
            {
                var po = await _purchaseOrderRepository.GetAll().Where(t => t.Id != entity.Id)
                    .OrderByDescending(t => t.CreationTime).FirstOrDefaultAsync();
                if (po != null)
                {
                    auto.UpdateLastAutoSequenceNumber(po.OrderNumber);
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

            await ValidateItemReceiptLink(input.Id);
            await ValidateBillLink(input.Id);

            if ( input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                  .Where(t => (t.LockKey == TransactionLockType.PurchaseOrder)
                  && t.IsLock == true && t.LockDate.Value.Date >= entity.OrderDate.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
           

            var PurchaseOrderItem = await _purchaseOrderItemRepository.GetAll().Where(u => u.PurchaseOrderId == entity.Id).ToListAsync();

            foreach (var s in PurchaseOrderItem)
            {
                CheckErrors(await _purchaseOrderItemManager.RemoveAsync(s));
            }

            var exchangeRates = await _exchangeRateRepository.GetAll().AsNoTracking().Where(s => s.PurchaseOrderId == input.Id).ToListAsync();
            if (exchangeRates.Any()) await _exchangeRateRepository.BulkDeleteAsync(exchangeRates);


            CheckErrors(await _purchaseOrderManager.RemoveAsync(@entity));
                       
            if (input.IsConfirm) 
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.PurchaseOrder, };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_PurchaseOrder_Disable)]
        public async Task Disable(EntityDto<Guid> input)
        {
            var @entity = await _purchaseOrderManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _purchaseOrderManager.DisableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_PurchaseOrder_Enable)]
        public async Task Enable(EntityDto<Guid> input)
        {

            var @entity = await _purchaseOrderManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _purchaseOrderManager.EnableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_PurchaseOrder_Find)]
        public async Task<PagedResultDto<PurchaseOrderGetListOutput>> Find(GetPurchaseOrderListInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var query = (from poi in _purchaseOrderItemRepository.GetAll().AsNoTracking()
                         join item in _itemRepository.GetAll().AsNoTracking()
                         .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.Id))
                         on poi.ItemId equals item.Id

                         join po in _purchaseOrderRepository.GetAll().AsNoTracking()
                         .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                         .WhereIf(input.Status != null && input.Status.Count > 0, u => input.Status.Contains(u.Status))
                         .WhereIf(!input.Filter.IsNullOrEmpty(),
                             p => p.OrderNumber.ToLower().Contains(input.Filter.ToLower()) ||
                             p.Reference.ToLower().Contains(input.Filter.ToLower()))
                         on poi.PurchaseOrderId equals po.Id

                         join v in _vendorRepository.GetAll().AsNoTracking()
                         .WhereIf(input.Vendors != null && input.Vendors.Count > 0, p => input.Vendors.Contains(p.Id))
                         on po.VendorId equals v.Id

                         group po by new
                         {
                             OrderNumber = po.OrderNumber,
                             OrderDate = po.OrderDate,
                             Id = po.Id,
                             IsActive = po.IsActive,
                             Reference = po.Reference,
                             Tax = po.Tax,
                             Total = po.Total,
                             VendorId = v.Id,
                             VendorAccountId = v.AccountId,
                             VendorCode = v.VendorCode,
                             VendorName = v.VendorName
                         } into u
                         select new PurchaseOrderGetListOutput
                         {
                             Id = u.Key.Id,
                             IsActive = u.Key.IsActive,
                             OrderDate = u.Key.OrderDate,
                             OrderNumber = u.Key.OrderNumber,
                             Reference = u.Key.Reference,
                             Tax = u.Key.Tax,
                             Total = u.Key.Total,
                             VendorId = u.Key.VendorId,
                             CountItem = u.Count(),
                             Vendor = new VendorSummaryOutput
                             {
                                 Id = u.Key.VendorId,
                                 AccountId = u.Key.VendorAccountId.Value,
                                 VendorCode = u.Key.VendorCode,
                                 VendorName = u.Key.VendorName
                             }
                         });


            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<PurchaseOrderGetListOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_PurchaseOrder_GetList)]
        public async Task<PagedResultDto<PurchaseOrderGetListOutput>> GetList(GetPurchaseOrderListInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
           

            var userQuery = GetUsers(input.Users);
            var locationQuery = GetLocations(null, input.Locations);

            var vendoTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();
        
            var vendorQuery = GetVendors(null, input.Vendors, input.VendorTypes, vendoTypeMemberIds);

            var oQuery = _purchaseOrderRepository.GetAll()
                         .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId.Value))
                         .WhereIf(input.Vendors != null && input.Vendors.Count > 0, u => input.Vendors.Contains(u.VendorId))
                         .WhereIf(input.FromDate != null && input.ToDate != null,
                                  (u => (u.OrderDate.Date) >= (input.FromDate.Date) && (u.OrderDate.Date) <= (input.ToDate.Date)))
                         .WhereIf(input.Status != null && input.Status.Count > 0, u => input.Status.Contains(u.Status))
                         .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                         .WhereIf(!input.Filter.IsNullOrEmpty(),
                            p => p.OrderNumber.ToLower().Contains(input.Filter.ToLower()) ||
                                   p.Reference.ToLower().Contains(input.Filter.ToLower()))
                         .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                         .AsNoTracking()
                         .Select(o => new
                         {
                             Id = o.Id,
                             IsActive = o.IsActive,
                             OrderDate = o.OrderDate,
                             OrderNumber = o.OrderNumber,
                             Reference = o.Reference,
                             Total = o.Total,                            
                             Status = o.Status,
                             ReceiveStatus = o.ReceiveStatus,
                             ReceiveCount = o.ReceiveCount,
                             CreatorId = o.CreatorUser.Id,
                             VendorId = o.VendorId,
                             LocationId = o.LocationId
                         });

            var oiQuery = _purchaseOrderItemRepository.GetAll()
                          .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.ItemId))
                          .AsNoTracking()
                          .Select(s => s.PurchaseOrderId);

            var query = from o in oQuery
                        join v in vendorQuery
                        on o.VendorId equals v.Id
                        join l in locationQuery
                        on o.LocationId equals l.Id
                        join u in userQuery
                        on o.CreatorId equals u.Id
                        join oi in oiQuery
                        on o.Id equals oi
                        into ois
                        where ois.Count() > 0

                        select new PurchaseOrderGetListOutput
                        {
                            Id = o.Id,
                            IsActive = o.IsActive,
                            OrderDate = o.OrderDate,
                            OrderNumber = o.OrderNumber,
                            Reference = o.Reference,
                            User = new UserDto
                            {
                                Id = o.CreatorId,
                                UserName = u.UserName
                            },
                            Total = o.Total,
                            VendorId = o.VendorId,
                            CountItem = ois.Count(),
                            StatusCode = o.Status,
                            StatusName = o.Status.ToString(),
                            ReceiveStatus = o.ReceiveStatus,
                            Vendor = new VendorSummaryOutput
                            {
                                Id = o.VendorId,
                                VendorName = v.VendorName
                            },
                            LocationName = l.LocationName,
                            TotalReceiveCount = o.ReceiveCount
                        };

            var resultCount = await query.CountAsync();
            if (resultCount == 0) return new PagedResultDto<PurchaseOrderGetListOutput>(resultCount, new List<PurchaseOrderGetListOutput>());

            if (input.Sorting.EndsWith("DESC"))
            {
                if (input.Sorting.ToLower().StartsWith("orderdate"))
                {
                    query = query.OrderByDescending(s => s.OrderDate);
                }
                else if (input.Sorting.ToLower().StartsWith("ordernumber"))
                {
                    query = query.OrderByDescending(s => s.OrderNumber);
                }
                else if (input.Sorting.ToLower().StartsWith("reference"))
                {
                    query = query.OrderByDescending(s => s.Reference);
                }
                else if (input.Sorting.ToLower().StartsWith("countitem"))
                {
                    query = query.OrderByDescending(s => s.CountItem);
                }
                else if (input.Sorting.ToLower().StartsWith("receivestatus"))
                {
                    query = query.OrderByDescending(s => s.ReceiveStatus);
                }
                else if (input.Sorting.ToLower().StartsWith("statuscode"))
                {
                    query = query.OrderByDescending(s => s.StatusName);
                }
                else if (input.Sorting.ToLower().StartsWith("total"))
                {
                    query = query.OrderByDescending(s => s.Total);
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
                if (input.Sorting.ToLower().StartsWith("orderdate"))
                {
                    query = query.OrderBy(s => s.OrderDate);
                }
                else if (input.Sorting.ToLower().StartsWith("ordernumber"))
                {
                    query = query.OrderBy(s => s.OrderNumber);
                }
                else if (input.Sorting.ToLower().StartsWith("reference"))
                {
                    query = query.OrderBy(s => s.Reference);
                }
                else if (input.Sorting.ToLower().StartsWith("countitem"))
                {
                    query = query.OrderBy(s => s.CountItem);
                }
                else if (input.Sorting.ToLower().StartsWith("receivestatus"))
                {
                    query = query.OrderBy(s => s.ReceiveStatus);
                }
                else if (input.Sorting.ToLower().StartsWith("statuscode"))
                {
                    query = query.OrderBy(s => s.StatusName);
                }
                else if (input.Sorting.ToLower().StartsWith("total"))
                {
                    query = query.OrderBy(s => s.Total);
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    query = query.OrderBy(input.Sorting);
                }
            }


            var @entities = await query.PageBy(input).ToListAsync();

            return new PagedResultDto<PurchaseOrderGetListOutput>(resultCount, @entities);
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_PurchaseOrder_GetList)]
        public async Task<PagedResultDto<PurchaseOrderGetListOutput>> GetListOld(GetPurchaseOrderListInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var vendoTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            var query = (from p in _purchaseOrderItemRepository.GetAll().AsNoTracking()

                         join i in _itemRepository.GetAll().AsNoTracking()
                         .WhereIf(input.Items != null && input.Items.Count > 0, u => input.Items.Contains(u.Id))
                         on p.ItemId equals i.Id

                         join o in _purchaseOrderRepository.GetAll().Include(t => t.CreatorUser).Include(t=>t.Location)
                         .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.LocationId.Value))
                         .WhereIf(input.FromDate != null && input.ToDate != null,
                                  (u => (u.OrderDate.Date) >= (input.FromDate.Date) && (u.OrderDate.Date) <= (input.ToDate.Date)))
                         .WhereIf(input.Status != null && input.Status.Count > 0, u => input.Status.Contains(u.Status))
                         .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                         .WhereIf(!input.Filter.IsNullOrEmpty(),
                            p => p.OrderNumber.ToLower().Contains(input.Filter.ToLower()) ||
                                   p.Reference.ToLower().Contains(input.Filter.ToLower()) ||
                                   p.Memo.ToLower().Contains(input.Filter.ToLower()))
                         .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.LocationId.Value))
                         .AsNoTracking()
                         on p.PurchaseOrderId equals o.Id

                         join v in _vendorRepository.GetAll().AsNoTracking()
                         .WhereIf(input.Vendors != null && input.Vendors.Count > 0, p => input.Vendors.Contains(p.Id))
                         .WhereIf(vendoTypeMemberIds.Any(), s => vendoTypeMemberIds.Contains(s.VendorTypeId.Value))
                         on o.VendorId equals v.Id

                         group p by new
                         {
                             Id = o.Id,
                             IsActive = o.IsActive,
                             OrderDate = o.OrderDate,
                             OrderNumber = o.OrderNumber,
                             Reference = o.Reference,
                             LocationName = o.Location.LocationName,
                             Total = o.Total,
                             VendorId = v.Id,
                             VendorName = v.VendorName,
                             VendorCode = v.VendorCode,
                             Status = o.Status,
                             ReceiveStatus = o.ReceiveStatus,
                             CreatorUserName = o.CreatorUser.UserName,
                             CreatorId = o.CreatorUser.Id,
                             o.ReceiveCount,
                             //PurchaseOrder = o, Vendor = v
                         } into u
                         select new PurchaseOrderGetListOutput
                         {
                             Id = u.Key.Id,
                             IsActive = u.Key.IsActive,
                             OrderDate = u.Key.OrderDate,
                             OrderNumber = u.Key.OrderNumber,
                             Reference = u.Key.Reference,
                             User = new UserDto
                             {
                                 Id = u.Key.CreatorId,
                                 UserName = u.Key.CreatorUserName
                             },
                             Total = u.Key.Total,
                             VendorId = u.Key.VendorId,
                             CountItem = u.Count(),
                             StatusCode = u.Key.Status,
                             StatusName = u.Key.Status.ToString(),
                             ReceiveStatus = u.Key.ReceiveStatus,
                             Vendor = new VendorSummaryOutput
                                 {
                                     Id = u.Key.VendorId,
                                     VendorCode = u.Key.VendorCode,
                                     VendorName = u.Key.VendorName
                                 },
                             LocationName = u.Key.LocationName,
                             TotalReceiveCount = u.Key.ReceiveCount
                         });
            var resultCount = await query.CountAsync();

            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<PurchaseOrderGetListOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_PurchaseOrder_GetDetail)]
        public async Task<PurchaseOrderDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            await TurnOffTenantFilterIfDebug();

            var @entity = await _purchaseOrderManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var @purchaseOrderItem = await _purchaseOrderItemRepository.GetAll()
                                    .Include(u => u.Item)
                                    .Include(u => u.Tax).Where(u => u.PurchaseOrderId == entity.Id)
                                    .Select(t => new PurchaseOrderItemDetailOut
                                    {
                                        MultiCurrencyTotal = t.MultiCurrencyTotal,
                                        MultiCurrencyUnitCost = t.MultiCurrencyUnitCost,
                                        Unit = t.Unit,                                       
                                        Description = t.Description,
                                        DiscountRate = t.DiscountRate,
                                        Id = t.Id,
                                        Item = ObjectMapper.Map<ItemSummaryOutput>(t.Item),
                                        ItemId = t.ItemId,
                                        Tax = ObjectMapper.Map<TaxDetailOutput>(t.Tax),
                                        TaxId = t.TaxId,
                                        TaxRate = t.TaxRate,
                                        Total = t.Total,                                      
                                        UnitCost = t.UnitCost
                                    })
                                    .ToListAsync();

            var result = ObjectMapper.Map<PurchaseOrderDetailOutput>(@entity);

            result.PurchaseOrderItems = @purchaseOrderItem;
            result.LocationId = entity.LocationId;
            result.MultiCurrency = ObjectMapper.Map <CurrencyDetailOutput> (entity.MultiCurrency);
            result.LocationName = entity.Location?.LocationName;

            if (result.UseExchangeRate)
            {
                result.ExchangeRate = await _exchangeRateRepository.GetAll().AsNoTracking()
                                            .Where(s => s.PurchaseOrderId == input.Id)
                                            .Select(s => new GetExchangeRateDto
                                            {
                                                Id = s.Id,
                                                FromCurrencyCode = s.FromCurrency.Code,
                                                ToCurrencyCode = s.ToCurrency.Code,
                                                FromCurrencyId = s.FromCurrencyId,
                                                ToCurrencyId = s.ToCurrencyId,
                                                Bid = s.Bid,
                                                Ask = s.Ask,
                                                IsInves = s.FromCurrencyId == entity.CurrencyId
                                            })
                                            .FirstOrDefaultAsync();
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_PurchaseOrder_Update)]
        public async Task<NullableIdDto<Guid>> Update(UpdatePurchaseOrderInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _purchaseOrderManager.GetAsync(input.Id, true); //this is vendor
             
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            if (input.LocationId == null || input.LocationId == 0)
            {
                throw new UserFriendlyException(L("PleaseSelectLocation"));
            }

            ValidateExchangeRate(input);
            await ValidateItemReceiptLink(input.Id);
            await ValidateBillLink(input.Id);

            await CheckClosePeriod(entity.OrderDate,input.OrderDate);

            if (input.IsConfirm == false)
            {

                var validateLockDate = await _lockRepository.GetAll()
                                      .Where(t => t.IsLock == true &&
                                      (t.LockDate.Value.Date >= input.DateCompare.Value.Date || t.LockDate.Value.Date >= input.OrderDate.Date)
                                      && (t.LockKey == TransactionLockType.PurchaseOrder)).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

           

            if (input.MulitCurrencyId == null || input.MulitCurrencyId == input.CurrencyId)
            {
                input.MulitCurrencyId = input.CurrencyId;
                input.MultiCurrencySubTotal = input.SubTotal;
                input.MultiCurrencyTotal = input.Total;
            }
            entity.Update(userId, input.VendorId, input.Reference, input.CurrencyId, input.OrderNumber, input.OrderDate, input.Memo, 
                            input.ShippingAddress, input.BillingAddress, input.SameAsShippingAddress, input.SubTotal, input.Tax, input.Total, 
                            input.Status, input.ETA, input.LocationId,input.MultiCurrencySubTotal,input.MultiCurrencyTotal,
                            input.MulitCurrencyId,input.MultiCurrencyTax, input.UseExchangeRate);
            #region update purchaseOrderItem           
            var purchaseOrderItem = await _purchaseOrderItemRepository.GetAll().Where(u => u.PurchaseOrderId == entity.Id).ToListAsync();
            foreach (var p in input.PurchaseOrderItems)
            {
                if (input.MulitCurrencyId == null || input.MulitCurrencyId == input.CurrencyId)
                {
                    p.MultiCurrencyUnitCost = p.UnitCost;
                    p.MultiCurrencyTotal = p.Total;
                }

                if (p.Id != null)
                {
                    var @purchase = purchaseOrderItem.FirstOrDefault(u => u.Id == p.Id);
                    if (purchase != null)
                    {
                        //here is in only same purchaseOrder so no need to update purchaseOrder
                        purchase.Update(userId, p.ItemId, p.TaxId, p.TaxRate, p.Description, p.Unit, p.UnitCost, p.DiscountRate, p.Total,p.MultiCurrencyTotal,p.MultiCurrencyUnitCost);
                        CheckErrors(await _purchaseOrderItemManager.UpdateAsync(purchase));
                    }
                }
                else if (p.Id == null)
                {
                    //@entity.Id is purchaseId or input.Id is also purchaseOrder Id so no need to pass purchaseOrderId from outside
                    var @PurchaseOrderItem = PurchaseOrders.PurchaseOrderItem.Create(tenantId, userId, p.ItemId, entity.Id, p.TaxId, p.TaxRate, p.Description, p.Unit, p.UnitCost, p.DiscountRate, p.Total,p.MultiCurrencyTotal,p.MultiCurrencyUnitCost);
                    base.CheckErrors(await _purchaseOrderItemManager.CreateAsync(@PurchaseOrderItem));

                }
            }

            var @toDeletePurchaseOrderItem = purchaseOrderItem.Where(u => !input.PurchaseOrderItems.Any(i => i.Id != null && i.Id == u.Id)).ToList();

            foreach (var t in toDeletePurchaseOrderItem)
            {
                CheckErrors(await _purchaseOrderItemManager.RemoveAsync(t));
            }
            #endregion

            CheckErrors(await _purchaseOrderManager.UpdateAsync(@entity));

            var exchange = await _exchangeRateRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.PurchaseOrderId == input.Id);
            if (input.UseExchangeRate && input.CurrencyId != input.MulitCurrencyId)
            {
                if (exchange == null)
                {
                    exchange = PurchaseOrderExchangeRate.Create(tenantId, userId, entity.Id, input.ExchangeRate.FromCurrencyId, input.ExchangeRate.ToCurrencyId, input.ExchangeRate.Bid, input.ExchangeRate.Ask);
                    await _exchangeRateRepository.InsertAsync(exchange);
                }
                else
                {
                    exchange.Update(userId, entity.Id, input.ExchangeRate.FromCurrencyId, input.ExchangeRate.ToCurrencyId, input.ExchangeRate.Bid, input.ExchangeRate.Ask);
                    await _exchangeRateRepository.UpdateAsync(exchange);
                }
            }
            else if (exchange != null)
            {
                await _exchangeRateRepository.DeleteAsync(exchange);
            }

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.PurchaseOrder, };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }

            await CurrentUnitOfWork.SaveChangesAsync();
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_PurchaseOrder_UpdateStatusToPublish)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input)
        {
            var @entity = await _purchaseOrderManager.GetAsync(input.Id, true);
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            
            entity.UpdateStatusToPublish();
            CheckErrors(await _purchaseOrderManager.UpdateAsync(entity));
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_PurchaseOrder_UpdateStatusToVoid)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input)
        {
            var @entity = await _purchaseOrderManager.GetAsync(input.Id, true);
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            await ValidateItemReceiptLink(input.Id);
            await ValidateBillLink(input.Id);

            entity.UpdateStatusToVoid();
            CheckErrors(await _purchaseOrderManager.UpdateAsync(entity));
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_PurchaseOrder_UpdateStatusToDraft)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input)
        {
            var @entity = await _purchaseOrderManager.GetAsync(input.Id, true);

            await ValidateItemReceiptLink(input.Id);
            await ValidateBillLink(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            entity.UpdateStatusToDraft();
            CheckErrors(await _purchaseOrderManager.UpdateAsync(entity));
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        private async Task ValidateItemReceiptLink(Guid orderId)
        {
            ////validate from itemreceipt
            //var validateByreceipt = (from receiptItem in _itemReceiptItemRepository.GetAll().AsNoTracking()
            //                         join purchaserItem in _purchaseOrderItemRepository.GetAll()
            //                         .Where(t => t.PurchaseOrderId == input.Id).AsNoTracking()
            //                         on receiptItem.OrderItemId equals purchaserItem.Id
            //                         select receiptItem).ToList().Count();
            //if (validateByreceipt > 0)
            //{
            //    throw new UserFriendlyException(L("RecordDoAlready!"));
            //}

            //validate from itemreceipt
            var validateByreceipt = await _purchaseOrderItemRepository.GetAll()
                                          .Include(s => s.ItemReceiptItems)
                                          .Where(s => s.PurchaseOrderId == orderId)
                                          .AsNoTracking()
                                          .SumAsync(s => s.ItemReceiptItems.Count());
            if (validateByreceipt > 0)
            {
                throw new UserFriendlyException(L("OrderAlreadyReceivedItems"));
            }
        }

        private async Task ValidateBillLink(Guid orderId)
        {
            ////validate from bill
            //var validateByBill = (from billItem in _billItemRepository.GetAll().AsNoTracking()
            //                      join purchaserItem in _purchaseOrderItemRepository.GetAll().AsNoTracking()
            //                      .Where(t => t.PurchaseOrderId == input.Id)
            //                      on billItem.OrderItemId equals purchaserItem.Id
            //                      select billItem).ToList().Count();
            //if (validateByBill > 0)
            //{
            //    throw new UserFriendlyException(L("RecordDoAlready!"));
            //}

            //validate from bill
            var validateByBill = await _purchaseOrderItemRepository.GetAll()
                                       .Include(s => s.BillItems)
                                       .Where(s => s.PurchaseOrderId == orderId)
                                       .AsNoTracking()
                                       .SumAsync(s => s.BillItems.Count());
            if (validateByBill > 0)
            {
                throw new UserFriendlyException(L("OrderAlreadyConvertToBill"));
            }
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_PurchaseOrder_UpdateStatusToClose)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToClose(UpdateStatus input)
        {
            var @entity = await _purchaseOrderManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            entity.UpdateStatusToClose();
            CheckErrors(await _purchaseOrderManager.UpdateAsync(entity));
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        #region
        //[AbpAuthorize(AppPermissions.Pages_Tenant_PurchaseOrder_GetTotalPurchaseOrderForItemReceipts)]
        //public async Task<PagedResultDto<PurchaserOrderSummaryOutput>> GetTotalPurchaseOrder(GetTotalPurchaseOrderListInput input)
        //{
        //    // get user by group member
        //    //var userGroups = await GetUserGroupByLocation();
        //    var query = (from u in _purchaseOrderRepository.GetAll().Include(u => u.Vendor)
        //                 .Where(p => p.IsActive == true)
        //                 .Where(u => u.Status == TransactionStatus.Publish)
        //                 .Where(u => u.Status != TransactionStatus.Close)
        //                 .WhereIf(!input.Filter.IsNullOrEmpty(),
        //                        u => u.OrderNumber.ToLower().Contains(input.Filter.ToLower()) || u.Reference.ToLower().Contains(input.Filter.ToLower()))
        //                 .WhereIf(input.Locations != null && input.Locations.Count > 0, t => input.Locations.Contains(t.LocationId))
        //                 .WhereIf(input.Vendors != null && input.Vendors.Count > 0, p => input.Vendors.Contains(p.VendorId))
        //                 .AsNoTracking()
        //                 orderby u.OrderDate
        //                 join p in _purchaseOrderItemRepository.GetAll().Include(u => u.Item)
        //                 .Where(u => u.Item.InventoryAccountId != null)
        //                 .AsNoTracking()

        //                 on u.Id equals p.PurchaseOrderId into po
        //                 select new PurchaserOrderSummaryOutput
        //                 {
        //                     Vendor = ObjectMapper.Map<VendorSummaryOutput>(u.Vendor),
        //                     VendorId = u.VendorId,
        //                     Memo = u.Memo,
        //                     Id = u.Id,
        //                     OrderDate = u.OrderDate,
        //                     OrderNumber = u.OrderNumber,
        //                     Total = u.Total,
        //                     CountPurchaseOrderItems = po.Count(),
        //                     CurrencyId = u.CurrencyId,
        //                     Currency = ObjectMapper.Map<CurrencyDetailOutput>(u.Currency),
        //                     ETA = u.ETA,
        //                 }).Where(u => u.CountPurchaseOrderItems > 0);
        //    var resultCount = await query.CountAsync();
        //    var @entities = await query.OrderBy(t => t.ETA).PageBy(input).ToListAsync();
        //    return new PagedResultDto<PurchaserOrderSummaryOutput>(resultCount, entities);
        //}


        //[AbpAuthorize(AppPermissions.Pages_Tenant_PurchaseOrder_GetlistPuchaseOrderForItemReceipts)]
        //public async Task<GetlistPuchaseOrderItemDetail> GetlistPuchaseOrderForItemReceipts(EntityDto<Guid> input)
        //{
        //    var @entity = await _purchaseOrderManager.GetAsync(input.Id);

        //    if (entity == null)
        //    {
        //        throw new UserFriendlyException(L("RecordNotFound"));
        //    }

        //    var @purchaseOrderItem = await _purchaseOrderItemRepository.GetAll()
        //        .Include(u => u.Item).Include(u => u.Tax).Include(u => u.Item.InventoryAccount)
        //        .Where(u => u.Item.InventoryAccountId != null)
        //        .Where(u => u.PurchaseOrderId == entity.Id)
        //        .ToListAsync();

        //    var result = ObjectMapper.Map<GetlistPuchaseOrderItemDetail>(@entity);
        //    result.SubTotal = 0;
        //    result.Tax = 0;
        //    foreach (var i in @purchaseOrderItem)
        //    {
        //        decimal totalTaxRate = i.Total * i.TaxRate;//get total tax of item

        //        result.SubTotal += i.Total;
        //        result.Tax += totalTaxRate;
        //    }
        //    result.Total = result.SubTotal + result.Tax;
        //    result.PurchaseOrderItems = ObjectMapper.Map<List<PurchaseOrderItemSummaryOut>>(@purchaseOrderItem);

        //    return result;
        //}

        #endregion

        [AbpAuthorize(AppPermissions.Pages_Tenant_PurchaseOrder_GetTotalPurchaseOrderForBills, 
                      AppPermissions.Pages_Tenant_PurchaseOrder_GetTotalPurchaseOrderForItemReceipts)]
        public async Task<PagedResultDto<PurchaserOrderSummaryOutput>> GetPurchaseOrders(GetTotalPurchaseOrderListInput input)
        {
            // get user by group member
            //var userGroups = await GetUserGroupByLocation();
            var vendoTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            var roundDigits = await _accountCycleRepository.GetAll().Select(t => t.RoundingDigit).FirstOrDefaultAsync();
            var query = (from po in _purchaseOrderRepository.GetAll()
                         .Include(u => u.Vendor)
                         .Where(s => s.Status == TransactionStatus.Publish &&
                                    (s.ApprovalStatus == ApprovalStatus.Approved ||
                                     s.ApprovalStatus == ApprovalStatus.Recorded))
                         .Where(p => p.IsActive == true)
                         .WhereIf(input.Currencys != null && input.Currencys.Count > 0, u => input.Currencys.Contains(u.MultiCurrencyId))
                         .WhereIf(!input.Filter.IsNullOrEmpty(),
                                u => u.OrderNumber.ToLower().Contains(input.Filter.ToLower()) || u.Reference.ToLower().Contains(input.Filter.ToLower()))
                         .WhereIf(vendoTypeMemberIds.Any(), s => vendoTypeMemberIds.Contains(s.Vendor.VendorTypeId.Value))
                         .AsNoTracking()

                         join poi in _purchaseOrderItemRepository.GetAll().AsNoTracking()
                         on po.Id equals poi.PurchaseOrderId into poItems
                         from pi in poItems

                         join bi in _billItemRepository.GetAll()
                         .Where(s => s.OrderItemId != null)                        
                         .AsNoTracking() 
                         on pi.Id equals bi.OrderItemId into bItems

                         join iRi in _itemReceiptItemRepository.GetAll().Include(s=>s.ItemReceipt)
                         .Where(s => s.OrderItemId != null && s.ItemReceipt.ReceiveFrom == ItemReceipt.ReceiveFromStatus.PO)                        
                         .AsNoTracking() on pi.Id equals iRi.OrderItemId into rItems

                         let receiptQty = rItems == null ? 0 : rItems.Sum(s => s.Qty)
                         let billQty = bItems == null ? 0 : bItems.Sum(s => s.Qty)
                         let itemCount = poItems.Count(s => s.Unit > receiptQty + billQty)

                         where itemCount > 0 && (receiptQty + billQty < pi.Unit)
                         
                         select ( new
                         {
                             p = po,
                             qty = pi.Unit - receiptQty - billQty,
                             itemCount = itemCount,
                             unitcost = pi.UnitCost,
                             unitcostmulticurrency = pi.MultiCurrencyUnitCost
                         })
                         into lists
                         group lists by new {
                             purchaseOrder = lists.p,                            
                             //itemCounts = lists.itemCount
                         } into u  
                         
                         select new PurchaserOrderSummaryOutput
                         {
                             Vendor = ObjectMapper.Map<VendorSummaryOutput>(u.Key.purchaseOrder.Vendor),
                             VendorId = u.Key.purchaseOrder.VendorId,
                             Memo = u.Key.purchaseOrder.Memo,
                             Id = u.Key.purchaseOrder.Id,
                             OrderDate = u.Key.purchaseOrder.OrderDate,
                             OrderNumber = u.Key.purchaseOrder.OrderNumber,
                             Total = u.Sum(s => Math.Round(s.unitcost * s.qty, roundDigits)),
                             MultiCurrencyTotal = u.Sum(s=> Math.Round(s.unitcostmulticurrency * s.qty,roundDigits)),
                             CountPurchaseOrderItems = u.Count(),
                             CurrencyId = u.Key.purchaseOrder.CurrencyId,
                             Currency = ObjectMapper.Map<CurrencyDetailOutput>(u.Key.purchaseOrder.Currency),
                             ETA = u.Key.purchaseOrder.ETA,
                         });
            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(t => t.ETA).PageBy(input).ToListAsync();
            return new PagedResultDto<PurchaserOrderSummaryOutput>(resultCount, entities);
        }

       
        [AbpAuthorize(AppPermissions.Pages_Tenant_PurchaseOrder_GetlistPuchaseOrderForBills,
                      AppPermissions.Pages_Tenant_PurchaseOrder_GetlistPuchaseOrderForItemReceipts)]
        public async Task<GetlistPuchaseOrderItemDetail> GetItemPuchaseOrders(EntityDto<Guid> input)
        {
            var roundDigits = await _accountCycleRepository.GetAll().Select(t => t.RoundingDigit).FirstOrDefaultAsync();
            var @entity = await _purchaseOrderManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var @purchaseOrderItem = await(from pi in _purchaseOrderItemRepository.GetAll()
                .Include(u => u.Item.InventoryAccount)
                .Include(u => u.Tax)
                .Include(u => u.Item.PurchaseAccount)
                .Where(s => s.PurchaseOrder.Status == TransactionStatus.Publish &&
                           (s.PurchaseOrder.ApprovalStatus == ApprovalStatus.Approved ||
                            s.PurchaseOrder.ApprovalStatus == ApprovalStatus.Recorded))
                .Where(p => p.PurchaseOrder.IsActive == true)
                .Where(u => u.PurchaseOrderId == entity.Id).AsNoTracking()

                 join bi in _billItemRepository.GetAll()
                 .Where(s => s.OrderItemId != null)                 
                 .AsNoTracking() on pi.Id equals bi.OrderItemId into bItems
                 join iRi in _itemReceiptItemRepository.GetAll().Include(s => s.ItemReceipt)
                 .Where(s => s.OrderItemId != null)
                 .AsNoTracking() on pi.Id equals iRi.OrderItemId into rItems
                 let biQty = bItems == null ? 0 : bItems.Sum(s => s.Qty)
                 let riQty = rItems == null ? 0 : rItems.Sum(s => s.Qty)
                 let mainQty = pi.Unit - biQty - riQty
                 where biQty + riQty < pi.Unit
                select new PurchaseOrderItemSummaryOut
                {
                     Id = pi.Id,
                     Description = pi.Description,
                     DiscountRate = pi.DiscountRate,
                     Item = ObjectMapper.Map<ItemSummaryDetailOutput>(pi.Item),
                     ItemId = pi.ItemId,
                     Remain = pi.Unit -biQty - riQty,
                     Unit = pi.Unit - biQty - riQty,
                     Tax = ObjectMapper.Map<TaxDetailOutput>(pi.Tax),
                     TaxId = pi.TaxId,
                     Total = Math.Round(pi.UnitCost * mainQty, roundDigits),
                     UnitCost = pi.UnitCost, 
                     MultiCurrencyTotal = Math.Round(pi.MultiCurrencyUnitCost * mainQty, roundDigits),
                     MulitCurrencyUnitCost = pi.MultiCurrencyUnitCost,
                     UseBatchNo = pi.Item.UseBatchNo,
                     AutoBatchNo = pi.Item.AutoBatchNo,
                     TrackSerial = pi.Item.TrackSerial,
                     TrackExpiration = pi.Item.TrackExpiration
                }).ToListAsync();

            var itemIds = @purchaseOrderItem.Select(t => t.ItemId).ToList();
            var itemLots = await _itemLotRepository.GetAll().Where(t => t.Lot.LocationId == entity.LocationId && itemIds.Contains(t.ItemId)).AsNoTracking().ToListAsync();


            var items = purchaseOrderItem.Select(pi => new PurchaseOrderItemSummaryOut
            {
                Id = pi.Id,
                Description = pi.Description,
                DiscountRate = pi.DiscountRate,
                Item =pi.Item,
                ItemId = pi.ItemId,
                Remain = pi.Remain,
                Unit = pi.Unit,
                Tax = pi.Tax,
                TaxId = pi.TaxId,
                Total = pi.Total,
                UnitCost = pi.UnitCost,
                MultiCurrencyTotal = pi.MultiCurrencyTotal,
                MulitCurrencyUnitCost = pi.MulitCurrencyUnitCost,
                UseBatchNo = pi.UseBatchNo,
                AutoBatchNo = pi.AutoBatchNo,
                TrackExpiration = pi.TrackExpiration,
                TrackSerial = pi.TrackSerial,
                LotId = itemLots.Where(t=> t.ItemId == pi.ItemId).Select(t=>t.LotId).FirstOrDefault(),
            }).ToList();

            var result = ObjectMapper.Map<GetlistPuchaseOrderItemDetail>(@entity);
            result.Total = Math.Round(@purchaseOrderItem.Sum(s => s.Total), roundDigits);
            result.PurchaseOrderItems = ObjectMapper.Map<List<PurchaseOrderItemSummaryOut>>(items);
            return result;
        }


        [HttpPost]
        [AbpAuthorize(AppPermissions.Pages_Tenant_PurchaseOrder_GetlistPuchaseOrderForBills,
                     AppPermissions.Pages_Tenant_PurchaseOrder_GetlistPuchaseOrderForItemReceipts)]
        public async Task<ListResultDto<PurchaseOrderItemFroBillDto>> GetPuchaseOrderItemsForBill(GetPurchaseOrderItemInput input)
        {
            
            var @purchaseOrderItem = await (from pi in _purchaseOrderItemRepository.GetAll()
                                             .Include(u => u.Item.InventoryAccount)
                                             .Include(s => s.PurchaseOrder.Location)
                                             .Include(u => u.Tax)
                                             .Include(u => u.Item.PurchaseAccount)
                                             .Where(s => s.PurchaseOrder.Status == TransactionStatus.Publish &&
                                                        (s.PurchaseOrder.ApprovalStatus == ApprovalStatus.Approved ||
                                                         s.PurchaseOrder.ApprovalStatus == ApprovalStatus.Recorded))
                                             .Where(p => p.PurchaseOrder.IsActive == true)
                                              .WhereIf(input.LocationIds != null && input.LocationIds.Any(), s => input.LocationIds.Contains(s.PurchaseOrder.LocationId))
                                             .WhereIf(input.ItemIds != null && input.ItemIds.Any(), s => input.ItemIds.Contains(s.ItemId))
                                             .WhereIf(input.Vendors != null && input.Vendors.Any(), u => input.Vendors.Contains(u.PurchaseOrder.VendorId))
                                             .WhereIf(input.MultiCurrencys != null && input.MultiCurrencys.Any(), s => input.MultiCurrencys.Contains(s.PurchaseOrder.MultiCurrencyId))
                                             .WhereIf(!input.Filter.IsNullOrEmpty(),
                                                    u => u.PurchaseOrder.OrderNumber.ToLower().Contains(input.Filter.ToLower()) ||
                                                    u.PurchaseOrder.Reference.ToLower().Contains(input.Filter.ToLower())
                                                )
                                             .AsNoTracking()

                                            join bi in _billItemRepository.GetAll()
                                            .Where(s => s.OrderItemId != null)
                                            .AsNoTracking() on pi.Id equals bi.OrderItemId into bItems
                                            join iRi in _itemReceiptItemRepository.GetAll().Include(s => s.ItemReceipt)
                                                .Where(s => s.OrderItemId != null && s.ItemReceipt.ReceiveFrom == ItemReceipt.ReceiveFromStatus.PO)
                                                .WhereIf(input.ExceptId != null, s => s.Id != input.ExceptId)
                                                .AsNoTracking() 
                                            on pi.Id equals iRi.OrderItemId into rItems
                                            let biQty = bItems == null ? 0 : bItems.Sum(s => s.Qty)
                                            let riQty = rItems == null ? 0 : rItems.Sum(s => s.Qty)
                                            let mainQty = pi.Unit - biQty + riQty
                                            where biQty + riQty < pi.Unit
                                            select new PurchaseOrderItemFroBillDto
                                            {
                                                Date = pi.PurchaseOrder.OrderDate,
                                                Id = pi.Id,
                                                ItemId = pi.ItemId,
                                                ItemCode = pi.Item.ItemCode,
                                                ItemName = pi.Item.ItemName,
                                                OrderId = pi.PurchaseOrderId,
                                                OrderNumber = pi.PurchaseOrder.OrderNumber,
                                                RemainQty = pi.Unit - biQty - riQty,
                                                Qty = pi.Unit,
                                                Total = pi.Total,
                                                UnitCost = pi.UnitCost,
                                                MultiCurrencyTotal = pi.MultiCurrencyTotal,
                                                MultiCurrencyUnitCost = pi.MultiCurrencyUnitCost,
                                                LocationName = pi.PurchaseOrder.Location.LocationName,
                                                Reference = pi.PurchaseOrder.Reference,
                                            }).ToListAsync();

            return new ListResultDto<PurchaseOrderItemFroBillDto>(@purchaseOrderItem);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_PurchaseOrder_GetList, AppPermissions.Pages_Tenant_Report_PurchaseOrder)]
        public async Task<ListResultDto<PurchaseOrderBillDetailDto>> GetBillDetailForOrder(EntityDto<Guid> input)
        {
            var billItemHasOrderQuery = from bi in _billItemRepository.GetAll()
                                                   .Include(s => s.Bill.Vendor)
                                                   .Include(s => s.ItemReceiptItem)
                                                   .Where(s => s.ItemReceiptItemId.HasValue)
                                                   .Where(s => s.OrderItemId.HasValue && s.OrderItem.PurchaseOrderId == input.Id)
                                                   .AsNoTracking()
                                        join b in _journalRepository.GetAll()
                                                  .Include(s => s.Currency)
                                                  .Include(s => s.MultiCurrency)
                                                  .Where(s => s.JournalType == JournalType.Bill && s.Status == TransactionStatus.Publish)
                                                  .AsNoTracking()
                                        on bi.BillId equals b.BillId
                                        join r in _journalRepository.GetAll()
                                                  .Where(s => s.JournalType == JournalType.ItemReceiptPurchase && s.Status == TransactionStatus.Publish)
                                        on bi.ItemReceiptItem.ItemReceiptId equals r.ItemReceiptId

                                        select new PurchaseOrderBillDetailDto
                                        {
                                            ReceiveDate = r.Date,
                                            ReceiveId = r.ItemReceiptId.Value,
                                            ReceiveNo = r.JournalNo,
                                            ReceiveReference = r.Reference,

                                            PaidStatus = bi.Bill.PaidStatus,
                                            Total = bi.Bill.Total,
                                            PaidAmount = bi.Bill.TotalPaid,
                                            Balance = bi.Bill.OpenBalance,
                                            BillId = bi.BillId,
                                            BillNo = b.JournalNo,
                                            VendorId = bi.Bill.VendorId,
                                            VendorName = bi.Bill.Vendor.VendorName,
                                            BillReference = b.Reference,
                                            CurrencyCode = b.MultiCurrency != null ? b.MultiCurrency.Code : b.Currency.Code,                                   
                                        };

            var billItemHasReceiveQuery = from bi in _billItemRepository.GetAll()
                                                  .Where(s => s.ItemReceiptItemId.HasValue && s.ItemReceiptItem.OrderItemId.HasValue)
                                                  .Where(s => s.ItemReceiptItem.OrderItem.PurchaseOrderId == input.Id)
                                                  .AsNoTracking()
                                          join b in _journalRepository.GetAll()
                                                  .Where(s => s.JournalType == JournalType.Bill && s.Status == TransactionStatus.Publish)
                                                  .AsNoTracking()
                                        on bi.BillId equals b.BillId
                                        select new 
                                        {                                         
                                            ReceivedStatus = bi.Bill.ReceivedStatus,
                                            PaidStatus = bi.Bill.PaidStatus,
                                            BillId = bi.BillId,
                                            BillNo = b.JournalNo,
                                            BillReference = b.Reference,
                                            ItemReceiptItemId = bi.ItemReceiptItemId,
                                            PaidAmount = bi.Bill.TotalPaid,
                                            Balance = bi.Bill.OpenBalance
                                        };

            var itemReceiptItemHasOrderQuery = from ri in _itemReceiptItemRepository.GetAll()
                                                           .Include(s => s.ItemReceipt.Vendor)
                                                           .Where(s => s.OrderItemId.HasValue && s.OrderItem.PurchaseOrderId == input.Id)
                                                           .AsNoTracking()                                       
                                                join r in _journalRepository.GetAll()
                                                          .Include(s => s.Currency)
                                                          .Include(s => s.MultiCurrency)
                                                          .Where(s => s.JournalType == JournalType.ItemReceiptPurchase && s.Status == TransactionStatus.Publish)
                                                on ri.ItemReceiptId equals r.ItemReceiptId

                                                select new 
                                                {
                                                    ReceiveItemId = ri.Id,
                                                    ReceiveDate = r.Date,
                                                    ReceiveId = r.ItemReceiptId.Value,
                                                    ReceiveNo = r.JournalNo,
                                                    ReceiveReference = r.Reference,
                                                    VendorId = ri.ItemReceipt.VendorId.Value,
                                                    VendorName = ri.ItemReceipt.Vendor.VendorName,
                                                    CurrencyCode = r.MultiCurrency != null ? r.MultiCurrency.Code : r.Currency.Code,
                                                    Total = ri.ItemReceipt.Total,                                           
                                                };

            var billItems = await billItemHasReceiveQuery.ToListAsync();
            var receiveItems = await itemReceiptItemHasOrderQuery.ToListAsync();

            var itemRceiptFromItemReceiptItems = from ri in receiveItems

                                        join bi in billItems
                                        on ri.ReceiveItemId equals bi.ItemReceiptItemId
                                        into bItems
                                        from bi in bItems.DefaultIfEmpty()

                                        select new PurchaseOrderBillDetailDto
                                        {
                                            ReceiveDate = ri.ReceiveDate,
                                            ReceiveId = ri.ReceiveId,
                                            ReceiveNo = ri.ReceiveNo,
                                            ReceiveReference = ri.ReceiveReference,
                                            VendorId = ri.VendorId,
                                            VendorName = ri.VendorName,
                                            CurrencyCode = ri.CurrencyCode,
                                            Total = ri.Total,

                                            PaidStatus = bi == null ? (PaidStatuse?)null : bi.PaidStatus,
                                            BillId = bi == null ? (Guid?)null : bi.BillId,
                                            BillNo = bi == null ? "" : bi.BillNo,
                                            BillReference = bi == null ? "" : bi.BillReference,
                                            PaidAmount = bi == null ? 0 : bi.PaidAmount,
                                            Balance = bi == null ? ri.Total : bi.Balance
                                        };


            var itemReceiptFromBillItem = await billItemHasOrderQuery.ToListAsync();

            var items = itemReceiptFromBillItem
                        .Union(itemRceiptFromItemReceiptItems)
                        .GroupBy(s => new
                        {
                            s.ReceiveDate,
                            s.ReceiveId,
                            s.ReceiveReference,
                            s.ReceiveNo,
                            s.BillReference,
                            s.BillNo,
                            s.BillId,
                            s.CurrencyCode,
                            s.Total,
                            s.VendorId,
                            s.VendorName,
                            s.PaidStatus,
                            s.PaidAmount,
                            s.Balance
                        })
                        .Select(s => new PurchaseOrderBillDetailDto
                        {
                            ReceiveId = s.Key.ReceiveId,
                            ReceiveDate = s.Key.ReceiveDate,
                            ReceiveNo = s.Key.ReceiveNo,
                            ReceiveReference = s.Key.ReceiveReference,
                            VendorId = s.Key.VendorId,
                            VendorName = s.Key.VendorName,
                            BillId = s.Key.BillId,
                            BillNo = s.Key.BillNo,
                            BillReference = s.Key.BillReference,
                            CurrencyCode = s.Key.CurrencyCode,
                            Total = s.Key.Total,
                            PaidStatus = s.Key.PaidStatus,    
                            PaidAmount = s.Key.PaidAmount,
                            Balance = s.Key.Balance
                        }).ToList();


            return new ListResultDto<PurchaseOrderBillDetailDto>(items);
        }

    }
}
