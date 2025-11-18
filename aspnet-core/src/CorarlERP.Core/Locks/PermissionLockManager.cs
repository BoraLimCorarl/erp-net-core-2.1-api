using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.Locks
{
    public class PermissionLockManager : CorarlERPDomainServiceBase, IPermissionLockManager
    {
        private readonly IRepository<PermissionLock, long> _permissionLockRepository;
        public PermissionLockManager(IRepository<PermissionLock, long> permissionLockRepository)
        {
            _permissionLockRepository = permissionLockRepository;
        }

        public async Task<IdentityResult> CreateAsync(PermissionLock entity)
        {
            await _permissionLockRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<PermissionLock> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _permissionLockRepository.GetAll().Include(s => s.Location) :
               _permissionLockRepository.GetAll().Include(s => s.Location).AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> UpdateAsync(PermissionLock entity)
        {
            await _permissionLockRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> RemoveAsync(PermissionLock entity)
        {
            await _permissionLockRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> EnableAsync(PermissionLock entity)
        {
            entity.Enable(true);
            await _permissionLockRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DisableAsync(PermissionLock entity)
        {
            entity.Enable(false);
            await _permissionLockRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
