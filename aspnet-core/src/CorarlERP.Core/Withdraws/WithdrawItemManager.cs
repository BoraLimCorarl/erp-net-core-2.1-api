using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Withdraws
{
   public class WithdrawItemManager : CorarlERPDomainServiceBase, IWithdrawItemManager
    {
        private readonly IRepository<WithdrawItem, Guid> _WithdrawItemRepository;

        public WithdrawItemManager(IRepository<WithdrawItem, Guid> WithdrawRepository)
        {
            _WithdrawItemRepository = WithdrawRepository;
        }
        public async Task<IdentityResult> CreateAsync(WithdrawItem entity)
        {
            await _WithdrawItemRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<WithdrawItem> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _WithdrawItemRepository.GetAll() :
                _WithdrawItemRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(WithdrawItem entity)
        {
            await _WithdrawItemRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(WithdrawItem entity)
        {
            await _WithdrawItemRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
