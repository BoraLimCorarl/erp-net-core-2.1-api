using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.UI;
using CorarlERP.AutoSequences;
using CorarlERP.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.SaleOrders
{
    public class SaleOrderManager : CorarlERPDomainServiceBase, ISaleOrderManager
    {
        private readonly IRepository<SaleOrder, Guid> _saleOrderRepository;
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;
        public SaleOrderManager(
            IRepository<SaleOrder, Guid> saleOrderRepository,
            IRepository<Tenant, int> tenantRepository,
            IRepository<AccountCycles.AccountCycle, long> accountCycleRepository,
            IAutoSequenceManager autoSequenceManager) : base(accountCycleRepository)
        {
            _saleOrderRepository = saleOrderRepository;
            _tenantRepository = tenantRepository;
            _autoSequenceManager = autoSequenceManager;
        }
        public async Task<IdentityResult> CreateAsync(SaleOrder entity, bool checkDupliateReference)
        {
            await CheckDuplicatePurchaseOrder(entity);
            await CheckClosePeriod(entity.OrderDate);

            if (checkDupliateReference) await CheckDuplicateReferenceNo(entity);

            await _saleOrderRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        private async Task CheckDuplicateReferenceNo(SaleOrder @entity)
        {
            if (entity.Reference.IsNullOrEmpty())
            {
                throw new UserFriendlyException(L("RequireReferenceNo"));
            }

            var @old = await _saleOrderRepository.GetAll().AsNoTracking()
                           .Where(u => u.IsActive &&
                                       u.Reference.ToLower() == entity.Reference.ToLower() &&
                                       u.Id != entity.Id && u.CustomerId == entity.CustomerId)
                           .FirstOrDefaultAsync();

            if (old != null && old.Reference.ToLower() == entity.Reference.ToLower())
            {
                throw new UserFriendlyException(L("DuplicateReferenceNo", entity.Reference));
            }
        }

        public async Task<IdentityResult> DisableAsync(SaleOrder entity)
        {
            await CheckClosePeriod(entity.OrderDate);
            @entity.Enable(false);
            await _saleOrderRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> EnableAsync(SaleOrder entity)
        {
            await CheckClosePeriod(entity.OrderDate);
            @entity.Enable(true);
            await CheckDuplicatePurchaseOrder(entity);
            await _saleOrderRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<SaleOrder> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _saleOrderRepository.GetAll()
                .Include(u => u.Customer)
                .Include(u => u.Currency)
                .Include(u => u.Customer.Account)
                .Include(u => u.SaleTransactionType)
                .Include(u => u.Location)
                .Include(u => u.MultiCurrency)
                .Include(u => u.CreatorUser)
                :
                _saleOrderRepository.GetAll()
                .Include(u => u.Customer)
                .Include(u => u.MultiCurrency)
                .Include(u => u.Location)
                .Include(u => u.Currency)
                .Include(u => u.SaleTransactionType)
                .Include(u => u.CreatorUser)
                .Include(u => u.Customer.Account).AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(SaleOrder entity)
        {
            await CheckClosePeriod(entity.OrderDate);
            await _saleOrderRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(SaleOrder entity, bool validateReference = true)
        {
            await CheckClosePeriod(entity.OrderDate);
            await CheckDuplicatePurchaseOrder(entity);

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.SaleOrder);
            if (auto.RequireReference && validateReference) await CheckDuplicateReferenceNo(entity);

            await _saleOrderRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
        private async Task CheckDuplicatePurchaseOrder(SaleOrder @entity)
        {
            var @old = await _saleOrderRepository.GetAll().AsNoTracking()
                           .Where(u => u.IsActive &&
                                       u.OrderNumber.ToLower() == entity.OrderNumber.ToLower() &&
                                       u.Id != entity.Id
                                       )
                           .FirstOrDefaultAsync();

            if (old != null && old.OrderNumber.ToLower() == entity.OrderNumber.ToLower())
            {
                throw new UserFriendlyException(L("DuplicateSaleOrderNumber", entity.OrderNumber));
            }
        }

    }
}
