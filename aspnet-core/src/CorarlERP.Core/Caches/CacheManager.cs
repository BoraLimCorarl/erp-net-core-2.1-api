using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Caches
{
    public class CacheManager : CorarlERPDomainServiceBase, ICacheManager
    {
        private readonly IRepository<Cache, long> _cacheRepository;

        public CacheManager(IRepository<Cache, long> cacheRepository)
        {
            _cacheRepository = cacheRepository;
        }

        public async virtual Task<IdentityResult> CreateAsync(Cache entity)
        {
            await CheckDuplicateName(entity);
            await _cacheRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<Cache> GetAsync(string keyName, bool tracking = false)
        {
            var @query = tracking ? _cacheRepository.GetAll() : _cacheRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.KeyName == keyName);
        }

        private async Task CheckDuplicateName(Cache @entity)
        {
            var @old = await _cacheRepository.GetAll().AsNoTracking()
                           .Where(u => u.KeyName.ToLower() == entity.KeyName.ToLower() && u.Id != entity.Id)
                           .FirstOrDefaultAsync();
            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicateKeyName", entity.KeyName));
            }
        }

        public async virtual Task<IdentityResult> UpdateAsync(Cache entity)
        {
            await _cacheRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
