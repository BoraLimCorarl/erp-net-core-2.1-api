using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Authorization;
using CorarlERP.Lots.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.Locations.Dto;
using CorarlERP.Locations;
using CorarlERP.UserGroups;
using System;
using CorarlERP.Locks;
using CorarlERP.PermissionLocks.Dto;
using CorarlERP.TransactionLockSchedules.Dto;
using Microsoft.AspNetCore.Mvc;
using CorarlERP.SaleOrders;
using CorarlERP.PurchaseOrders;
using CorarlERP.Journals;
using Abp.Dependency;
using CorarlERP.Productions;
using CorarlERP.TransferOrders;
using CorarlERP.BankTransfers;

namespace CorarlERP.PermissionLocks
{
    public class PermissionLockAppService : CorarlERPAppServiceBase, IPermissionLockAppService
    {
        private readonly IPermissionLockManager _permissionLockManager;
        private readonly IRepository<PermissionLock, long> _permissionLockRepository;

        private readonly IRepository<SaleOrder, Guid> _saleOrderRepository;
        private readonly IRepository<PurchaseOrder, Guid> _purchaseOrderRepository;
        private readonly IRepository<Production, Guid> _productionOrderRepository;
        private readonly IRepository<TransferOrder, Guid> _transferOrderRepository;
        private readonly IRepository<BankTransfer, Guid> _bankTransferRepository;
        private readonly IRepository<Journal, Guid> _journalRepository;


        private readonly IRepository<UserGroupMember, Guid> _userGroupMemberRepository;


        public PermissionLockAppService(IPermissionLockManager permisionLockManager,
            IRepository<PermissionLock, long> permissionLockRepository,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository)
        {
            _permissionLockManager = permisionLockManager;
            _permissionLockRepository = permissionLockRepository;
            _userGroupMemberRepository = userGroupMemberRepository;

            _saleOrderRepository = IocManager.Instance.Resolve<IRepository<SaleOrder, Guid>>();
            _purchaseOrderRepository = IocManager.Instance.Resolve<IRepository<PurchaseOrder, Guid>>();
            _journalRepository = IocManager.Instance.Resolve<IRepository<Journal, Guid>>();

            _productionOrderRepository = IocManager.Instance.Resolve<IRepository<Production, Guid>>();
            _transferOrderRepository = IocManager.Instance.Resolve<IRepository<TransferOrder, Guid>>();
            _bankTransferRepository = IocManager.Instance.Resolve<IRepository < BankTransfer, Guid>>();
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Locks_CreateOrUpdate)]
        public async Task<PermissionLockDetailOutput> Create(CreatePermissionLockInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            await this.ValidatePermissionLock(input);

            var    @entity = PermissionLock.Create(tenantId, userId, input.LockTransaction, input.LockAction, input.PermissionDate, input.LocationId, input.TransactionNo, input.PermissionCode, input.PermissionCodeGenerateDate, input.ExpiredDuration);
            
            CheckErrors(await _permissionLockManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<PermissionLockDetailOutput>(@entity);
        }

        private async Task ValidatePermissionLock(CreatePermissionLockInput input)
        {
            if (input.LockTransaction == 0) throw new UserFriendlyException(L("IsNotSelected", L("Transaction")));
            if (input.LockAction == 0) throw new UserFriendlyException(L("IsNotSelected", L("LockAction")));
            if (input.LockAction == LockAction.Create)
            {
                if(input.PermissionDate == null) throw new UserFriendlyException(L("IsNotSelected", L("Date")));
                if(input.LocationId == 0 || input.LocationId == null) throw new UserFriendlyException(L("IsNotSelected", L("Location")));
            }
            else
            {
                if (input.TransactionNo.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("PleaseEnter", L("TransactionNo")));
                if(input.LockTransaction == TransactionLockType.SaleOrder)
                {
                    var entity = await _saleOrderRepository.GetAll().FirstOrDefaultAsync(s => s.OrderNumber == input.TransactionNo);
                    if(entity == null) throw new UserFriendlyException(L("IsNotValid", L("SaleOrder")));
                }
                else if(input.LockTransaction == TransactionLockType.PurchaseOrder)
                {
                    var entity = await _purchaseOrderRepository.GetAll().FirstOrDefaultAsync(s => s.OrderNumber == input.TransactionNo);
                    if (entity == null) throw new UserFriendlyException(L("IsNotValid", L("PurchaseOrder")));
                }
                else if(input.LockTransaction == TransactionLockType.ProductionOrder)
                {
                    var entity = await _productionOrderRepository.GetAll().FirstOrDefaultAsync(s => s.ProductionNo == input.TransactionNo);
                    if (entity == null) throw new UserFriendlyException(L("IsNotValid", L("ProductionNo")));
                }
                else if (input.LockTransaction == TransactionLockType.TransferOrder)
                {
                    var entity = await _transferOrderRepository.GetAll().FirstOrDefaultAsync(s => s.TransferNo == input.TransactionNo);
                    if (entity == null) throw new UserFriendlyException(L("IsNotValid", L("TransferNo")));
                }        
                else if (input.LockTransaction == TransactionLockType.BankTransfer)
                {
                    var entity = await _bankTransferRepository.GetAll().FirstOrDefaultAsync(s => s.BankTransferNo == input.TransactionNo);
                    if (entity == null) throw new UserFriendlyException(L("IsNotValid", L("TransferNo")));
                }
                else
                {
                    var entity = await _journalRepository.GetAll()
                        .FirstOrDefaultAsync(s => s.JournalNo == input.TransactionNo 
                        //&& (
                        //(input.LockTransaction == TransactionLockType.Bill && s.JournalType == JournalType.Bill) ||
                        //(input.LockTransaction == TransactionLockType.Invoice && s.JournalType == JournalType.Invoice) ||
                        //(input.LockTransaction == TransactionLockType.ReceivePayment && s.JournalType == JournalType.ReceivePayment) ||
                        //(input.LockTransaction == TransactionLockType.PayBill && s.JournalType == JournalType.PayBill) ||
                        //(input.LockTransaction == TransactionLockType.PayBill && s.JournalType == JournalType.PayBill) ||
                        //(input.LockTransaction == TransactionLockType.BankTransfer || 
                        //input.LockTransaction == TransactionLockType.BankTransaction ||
                        //input.LockTransaction == TransactionLockType.ProductionOrder ||
                        //input.LockTransaction == TransactionLockType.TransferOrder) )                        
                        );
                    if (entity == null)
                    {
                        if (input.LockTransaction == TransactionLockType.InventoryTransaction)
                        {
                            var transfer = await _transferOrderRepository.GetAll().FirstOrDefaultAsync(s => s.TransferNo == input.TransactionNo);
                            if (transfer == null) {
                                var production = await _productionOrderRepository.GetAll().FirstOrDefaultAsync(s => s.ProductionNo == input.TransactionNo);
                                if(production == null)
                                {
                                    throw new UserFriendlyException(L("IsNotValid", L("TransactionNo")));
                                }
                            }
                        }
                        else
                        {
                            throw new UserFriendlyException(L("IsNotValid", L("JournalNo")));
                        }
                    }

                }
            }
        }

        //[AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Locks_CreateOrUpdate)]
        //public async Task BulkCreate(List<CreatePermissionLockInput> inputList)
        //{
        //    await BulkCreateHelper(inputList);           
        //}

        //private async Task BulkCreateHelper(List<CreatePermissionLockInput> inputList)
        //{
        //    var tenantId = AbpSession.GetTenantId();
        //    var userId = AbpSession.GetUserId();

        //    foreach (var input in inputList)
        //    {
        //        var @entity = PermissionLock.Create(tenantId, userId, input.LockTransaction, input.LockAction, input.PermissionDate, input.LocationId, input.TransactionNo, input.PermissionCode, input.ExpiredDuration);

        //        CheckErrors(await _permissionLockManager.CreateAsync(@entity));
        //    }

        //    await CurrentUnitOfWork.SaveChangesAsync();
        //}


        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Locks_CreateOrUpdate)]
        public async Task Delete(EntityDto<long> input)
        {
            var @entity = await _permissionLockManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _permissionLockManager.RemoveAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Locks_CreateOrUpdate)]
        public async Task Disable(EntityDto<long> input)
        {
            var @entity = await _permissionLockManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _permissionLockManager.DisableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Locks_CreateOrUpdate)]
        public async Task Enable(EntityDto<long> input)
        {
            var @entity = await _permissionLockManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _permissionLockManager.EnableAsync(@entity));
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Locks_GetList)]
        public async Task<PermissionLockDetailOutput> GetDetail(EntityDto<long> input)
        {
            var @entity = await _permissionLockManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            return ObjectMapper.Map<PermissionLockDetailOutput>(@entity);
        }

        [HttpPost]
        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Locks_GetList)]
        public async Task<PagedResultDto<PermissionLockDetailOutput>> GetList(GetPermissionLockListInput input)
        {
            var @query = _permissionLockRepository
                .GetAll()
                .Include(u => u.Location)
                .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                .AsNoTracking()
                .Select(s => new PermissionLockDetailOutput {
                    Id = s.Id,
                    IsActive = s.IsActive,
                    LocationId = s.LocationId,
                    LocationName = s.Location == null ? "" : s.Location.LocationName,
                    LockAction = s.LockAction,
                    LockActionName = s.LockAction.ToString(),
                    LockTransaction = s.LockTransaction,
                    LockTransactionName = s.LockTransaction.ToString(),
                    PermissionCode = s.PermissionCode,
                    PermissionDate = s.PermissionDate,
                    TransactionNo = s.TransactionNo,
                    ExpiredDuration = s.ExpiredDuration,
                    PermissionCodeGenerateDate = s.PermissionCodeGenerateDate,
                }); 

            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<PermissionLockDetailOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Locks_CreateOrUpdate)]
        public async Task<PermissionLockDetailOutput> Update(UpdatePermissionLockInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            await this.ValidatePermissionLock(input);

            var @entity = await _permissionLockManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.Update(userId, input.LockTransaction, input.LockAction, input.PermissionDate, input.LocationId, input.TransactionNo, input.PermissionCode, input.ExpiredDuration);

            CheckErrors(await _permissionLockManager.UpdateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<PermissionLockDetailOutput>(@entity);
        }

        //[AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Locks_CreateOrUpdate)]
        //public async Task BulkUpdate(List<UpdatePermissionLockInput> inputList)
        //{
        //    var createList = inputList.Where(s => s.Id == 0).Select(s => new CreatePermissionLockInput
        //    { 
        //        TransactionNo = s.TransactionNo,
        //        LocationId = s.LocationId,
        //        LockAction = s.LockAction,
        //        LockTransaction = s.LockTransaction,
        //        PermissionCode = s.PermissionCode,
        //        PermissionDate = s.PermissionDate,
        //        ExpiredDuration = s.ExpiredDuration,                
        //    }).ToList();
        //    var updateList = inputList.Where(s => s.Id > 0).ToList();

        //    if (createList.Any()) await BulkCreateHelper(createList);

        //    var tenantId = AbpSession.GetTenantId();
        //    var userId = AbpSession.GetUserId();

        //    foreach(var input in updateList)
        //    {
        //        var @entity = await _permissionLockManager.GetAsync(input.Id, true);

        //        if (entity == null)
        //        {
        //            throw new UserFriendlyException(L("RecordNotFound"));
        //        }

        //        entity.Update(userId, input.LockTransaction, input.LockAction, input.PermissionDate, input.LocationId, input.TransactionNo, input.PermissionCode, input.ExpiredDuration);

        //        CheckErrors(await _permissionLockManager.UpdateAsync(@entity));
        //    }
                       
        //    await CurrentUnitOfWork.SaveChangesAsync();
           
        //}
    }
}
