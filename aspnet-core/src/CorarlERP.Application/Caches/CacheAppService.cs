using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Caches.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Caches
{
    [AbpAuthorize]
    public class CacheAppService : CorarlERPAppServiceBase, ICacheAppService
    {
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<Cache, long> _cacheRepository;

        public CacheAppService(ICacheManager cacheManager,
                             IRepository<Cache, long> cacheRepository)
        {
            _cacheManager = cacheManager;
            _cacheRepository = cacheRepository;
        }
        
        public async Task<CreateOrUpdateCache> CreateOrUpdate(CreateOrUpdateCache input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _cacheManager.GetAsync(input.KeyName, true);

            if (entity == null)
            {
                var createCache = Cache.Create(tenantId, userId, input.KeyName, input.KeyValue);
                CheckErrors(await _cacheManager.CreateAsync(createCache));
            }
            else {
                entity.Update(userId, input.KeyName, input.KeyValue);
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<CreateOrUpdateCache>(@entity);
        }
        

        public async Task<CreateOrUpdateCache> GetDetail(string keyName)
        {
            var @entity = await _cacheManager.GetAsync(keyName);

            //if (entity == null)
            //{
            //    throw new UserFriendlyException(L("RecordNotFound"));
            //}
            return ObjectMapper.Map<CreateOrUpdateCache>(@entity);
        }

    }
}
