using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.Classes
{
    public class ClassManager : CorarlERPDomainServiceBase, IClassManager
    {
        private readonly IRepository<Class, long> _classRepository;

        public ClassManager(IRepository<Class, long> classRepository)
        {
            _classRepository = classRepository;
        }
        private async Task CheckDuplicateClass(Class @entity)
        {
            var @old = await _classRepository.GetAll().AsNoTracking()
                           .Where(u => u.IsActive &&
                                       u.ClassName.ToLower() == entity.ClassName.ToLower() &&
                                       u.Id != entity.Id)
                           .FirstOrDefaultAsync();

            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicateClassName", entity.ClassName));
            }
        }
        public async virtual Task<IdentityResult> CreateAsync(Class entity)
        {
            await CheckDuplicateClass(entity);

            await _classRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> DisableAsync(Class entity)
        {
            @entity.Enable(false);
            await _classRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> EnableAsync(Class entity)
        {
            @entity.Enable(true);
            await _classRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<Class> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _classRepository.GetAll().Include(u=>u.ParentClass) : _classRepository.GetAll().Include(u => u.ParentClass).AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(Class entity)
        {
            await _classRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(Class entity)
        {
            await CheckDuplicateClass(entity);

            await _classRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
