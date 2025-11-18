using Abp.Domain.Repositories;
using Abp.UI;
using CorarlERP.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.PhysicalCount
{
    public class PhysicalCountManager: CorarlERPDomainServiceBase,  IPhysicalCountManager
    {
        private readonly IRepository<PhysicalCount, Guid> _transferOrderRepository;
        private readonly IRepository<Tenant, int> _tenantRepository;
        public PhysicalCountManager(IRepository<PhysicalCount, Guid> TransferOrderRepository, IRepository<Tenant, int> tenantRepository)
        {
            _transferOrderRepository = TransferOrderRepository;
            _tenantRepository = tenantRepository;
        }
        public async Task<IdentityResult> CreateAsync(PhysicalCount entity)
        {
            await CheckDuplicatePhysical(entity);
            await CheckClosePeriod(entity);
            await _transferOrderRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }
        public async Task<PhysicalCount> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _transferOrderRepository.GetAll()
                .Include(u => u.Location)
                .Include(u => u.Class)
                :
                _transferOrderRepository.GetAll()
                .Include(u => u.Location)
                .Include(u => u.Class)
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(PhysicalCount entity)
        {
            await CheckClosePeriod(entity);
            await _transferOrderRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(PhysicalCount entity)
        {
            await CheckDuplicatePhysical(entity);
            await CheckClosePeriod(entity);
            await _transferOrderRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        private async Task CheckDuplicatePhysical(PhysicalCount @entity)
        {
            var @old = await _transferOrderRepository.GetAll().AsNoTracking()
                           .Where(u => u.PhysicalCountNo.ToLower() == entity.PhysicalCountNo.ToLower() && u.Id != entity.Id)
                           .FirstOrDefaultAsync();

            if (old != null && old.PhysicalCountNo.ToLower() == entity.PhysicalCountNo.ToLower())
            {
                throw new UserFriendlyException(L("DuplicatePhysicalCountNoNumber", entity.PhysicalCountNo));
            }
        }

        private async Task CheckClosePeriod(PhysicalCount @entity)
        {
            var @closePeroid = await _tenantRepository.GetAll().AsNoTracking().Include(t => t.AccountCycle)
                           .Where(u => u.Id == entity.TenantId.Value)
                           .FirstOrDefaultAsync();

            if (closePeroid.AccountCycle.StartDate >= entity.PhysicalCountDate)
            {
                throw new UserFriendlyException(L("PeriodIsClose"));
            }
        }
    }
}
