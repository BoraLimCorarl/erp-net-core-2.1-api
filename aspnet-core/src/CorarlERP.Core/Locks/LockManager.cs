using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.Locks
{
    public class LockManager : CorarlERPDomainServiceBase, ILockManager
    {
        private readonly IRepository<Lock, long> _LockRepository;
        public LockManager(IRepository<Lock, long> lockRepository)
        {
            _LockRepository = lockRepository ;
        }

        public async Task<IdentityResult> CreateAsync(Lock entity)
        {
            await _LockRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<Lock> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _LockRepository.GetAll() :
               _LockRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> UpdateAsync(Lock entity)
        {
            await _LockRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
