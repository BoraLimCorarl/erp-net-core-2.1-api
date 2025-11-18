using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.UI;
using CorarlERP.AccountCycles;
using CorarlERP.AutoSequences;
using CorarlERP.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.PurchaseOrders
{
    public class PurchaseOrderManager : CorarlERPDomainServiceBase, IPurchaseOrderManager
    {
        private readonly IRepository<PurchaseOrder, Guid> _purchaseOrderRepository;
       
        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly IRepository<AccountCycle, long> _accountCycleRepository;
        public PurchaseOrderManager(
            IRepository<PurchaseOrder, Guid> purchaseOrderRepository,
            IRepository<AccountCycle, long> accountCycleRepository,
            IRepository<Tenant,int>tenantRepository,
            IAutoSequenceManager autoSequenceManager) : base(accountCycleRepository)
        {
            _autoSequenceManager = autoSequenceManager;
            _purchaseOrderRepository = purchaseOrderRepository;
            _accountCycleRepository = accountCycleRepository;
           
        }
        public async Task<IdentityResult> CreateAsync(PurchaseOrder entity, bool checkDupliateReference)
        {
            await CheckClosePeriod(entity.OrderDate);
            await CheckDuplicatePurchaseOrder(entity);

            if(checkDupliateReference) await CheckDuplicateReferenceNo(entity);
            await _purchaseOrderRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DisableAsync(PurchaseOrder entity)
        {
            await CheckClosePeriod(entity.OrderDate);
            @entity.Enable(false);
            await _purchaseOrderRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> EnableAsync(PurchaseOrder entity)
        {
            await CheckClosePeriod(entity.OrderDate);
            @entity.Enable(true);
            await _purchaseOrderRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<PurchaseOrder> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _purchaseOrderRepository.GetAll()
                .Include(u => u.Vendor)
                .Include(u => u.Currency)
                .Include(u=>u.Vendor.ChartOfAccount)
                .Include(u => u.CreatorUser)
                .Include(u=>u.Location)
                .Include(u=>u.MultiCurrency)
                :
                _purchaseOrderRepository.GetAll()
                .Include(u => u.Vendor)
                .Include(u => u.Location)
                .Include(u => u.Vendor.ChartOfAccount)
                .Include(u => u.Currency)
                .Include(u => u.CreatorUser)
                .Include(u => u.MultiCurrency)
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(PurchaseOrder entity)
        {
            await CheckClosePeriod(entity.OrderDate);
            await _purchaseOrderRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(PurchaseOrder entity, bool checkDupliateReference = true)
        {
            await CheckDuplicatePurchaseOrder(entity);
            await CheckClosePeriod(entity.OrderDate);

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.PurchaseOrder);

            if (auto.RequireReference && checkDupliateReference) await CheckDuplicateReferenceNo(entity);

            await _purchaseOrderRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        private async Task CheckDuplicatePurchaseOrder(PurchaseOrder @entity)
        {
            var @old = await _purchaseOrderRepository.GetAll().AsNoTracking()
                           .Where(u => u.IsActive &&
                                       u.OrderNumber.ToLower() == entity.OrderNumber.ToLower() &&
                                       u.Id != entity.Id
                                       )
                           .FirstOrDefaultAsync();

            if (old != null && old.OrderNumber.ToLower() == entity.OrderNumber.ToLower())
            {
                throw new UserFriendlyException(L("DuplicatePurchaseOrderNumber", entity.OrderNumber));
            }
        }

        private async Task CheckDuplicateReferenceNo(PurchaseOrder @entity)
        {
            if (entity.Reference.IsNullOrEmpty())
            {
                throw new UserFriendlyException(L("RequireReferenceNo"));
            }

            var @old = await _purchaseOrderRepository.GetAll().AsNoTracking()
                           .Where(u => u.IsActive &&
                                       u.Reference.ToLower() == entity.Reference.ToLower() &&
                                       u.Id != entity.Id && u.VendorId == entity.VendorId
                                       )
                           .FirstOrDefaultAsync();

            if (old != null && old.Reference.ToLower() == entity.Reference.ToLower())
            {
                throw new UserFriendlyException(L("DuplicateReferenceNo", entity.Reference));
            }
        }

       

    }
}
