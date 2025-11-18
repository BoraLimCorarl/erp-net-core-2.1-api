using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CorarlERP.Productions
{
    public class FinishItemManager : CorarlERPDomainServiceBase, IFinishItemManager
    {
        private readonly IRepository<FinishItems, Guid> _finishItemRepository;

        public FinishItemManager(IRepository<FinishItems, Guid> finishItemRepository)
        {
            _finishItemRepository = finishItemRepository;
        }
        public async Task<IdentityResult> CreateAsync(FinishItems entity)
        {
            await _finishItemRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<FinishItems> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _finishItemRepository.GetAll() :
                _finishItemRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(FinishItems entity)
        {
            await _finishItemRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(FinishItems entity)
        {
            await _finishItemRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
