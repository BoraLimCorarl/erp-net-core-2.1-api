using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Deposits
{
    public class DepositItemManager : CorarlERPDomainServiceBase, IDepositItemManager
    {
        private readonly IRepository<DepositItem, Guid> _depositItemRepository;

        public DepositItemManager(IRepository<DepositItem, Guid> depositItemRepository)
        {
            _depositItemRepository = depositItemRepository;
        }

        public async Task<IdentityResult> CreateAsync(DepositItem entity)
        {
            await _depositItemRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<DepositItem> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _depositItemRepository.GetAll() :
                _depositItemRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(DepositItem entity)
        {
            await _depositItemRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(DepositItem entity)
        {
            await _depositItemRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

    }
}
