using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Linq.Extensions;
using System.Linq;
using CorarlERP.Authorization.ApiClients.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Abp.Extensions;
using Abp.Collections.Extensions;
using System.Linq.Dynamic.Core;
using CorarlERP.MultiTenancy.Dto;

namespace CorarlERP.Authorization.ApiClients
{
    [AbpAuthorize]
    public class ApiClientAppService : CorarlERPAppServiceBase, IApiClientAppService
    {
        private readonly IRepository<ApiClient, Guid> _apiClientRepository;

        private readonly IApiClientManager _apiClientManager;

        public ApiClientAppService(IRepository<ApiClient, Guid> apiClientRepository,
            IApiClientManager apiClientManager)
        {
            _apiClientRepository = apiClientRepository;
            _apiClientManager = apiClientManager;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Developer_ApiClients_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            var user = await _apiClientManager.GetAsync(input.Id, false);
            await _apiClientManager.DeleteAsync(user);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Developer_ApiClients_Enable)]
        public async Task Enable(EntityDto<Guid> input)
        {
            var user = await _apiClientManager.GetAsync(input.Id, false);
            user.Enable(true);
            await _apiClientManager.UpdateAsync(user);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Developer_ApiClients_Disable)]
        public async Task Disable(EntityDto<Guid> input)
        {
            var user = await _apiClientManager.GetAsync(input.Id, false);
            user.Enable(false);
            await _apiClientManager.UpdateAsync(user);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Developer_ApiClients_Create)]
        public async Task Create(ApiClientInput input)
        {
            var userId = AbpSession.UserId;
            var hashedSecrete = new PasswordHasher<String>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(input.ClientId, input.PlaintSecret);
            var entity = ApiClient.Create(userId, input.ClientId, hashedSecrete, input.Name, input.ApplicationType, input.AllowedOrigin, input.OwnedByTenantId);
            await _apiClientManager.CreateAsync(entity);
        }


        [AbpAuthorize(AppPermissions.Pages_Administration_Developer_ApiClients)]
        public async Task<PagedResultDto<ApiClientDto>> GetList(GetListApiClientInput input)
        {
            var query = from a in _apiClientRepository.GetAll().AsNoTracking()
                            .WhereIf(input.IsActive != null, p => p.Active == input.IsActive.Value)
                           .WhereIf(
                               !input.Filter.IsNullOrEmpty(),
                                 p => p.Name.ToLower().Contains(input.Filter.ToLower()))
                        join t in base.TenantManager.Tenants.AsNoTracking() on a.OwnedByTenantId equals t.Id into apiTenant
                        from at in apiTenant.DefaultIfEmpty()
                        select new ApiClientDto
                        {
                            Id = a.Id,
                            AllowedOrigin = a.AllowedOrigin,
                            ApplicationType = a.ApplicationType,
                            Name = a.Name,
                            TenantId = a.OwnedByTenantId,
                            TenantName = at != null ? at.Name : "",
                            IsActive = a.Active,
                            ClientId = a.ClientId,
                            //ClientSecret = a.Secret,
                        };

            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<ApiClientDto>(resultCount, ObjectMapper.Map<List<ApiClientDto>>(@entities));

        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Developer_ApiClients_Update)]
        public async Task Update(ApiClientUpdateInput input)
        {
            var user = await _apiClientManager.GetAsync(input.Id, false);

            var hashedSecrete = new PasswordHasher<String>(new OptionsWrapper<PasswordHasherOptions>
                                (new PasswordHasherOptions())).HashPassword(user.ClientId, input.PlaintSecret);
            user.Update(AbpSession.UserId, input.ClientId, hashedSecrete, input.Name, input.ApplicationType, input.AllowedOrigin, input.OwnedByTenantId);
            await _apiClientManager.UpdateAsync(user);
        }
        [AbpAuthorize(AppPermissions.Pages_Administration_Developer_ApiClients_Detail)]
        public async Task<ApiClientDto> GetDetail(EntityDto<Guid> input)
        {
            var result = await (from a in _apiClientRepository.GetAll().Where(t => t.Id == input.Id)
                                join tenant in base.TenantManager.Tenants.AsNoTracking() on a.OwnedByTenantId equals tenant.Id into apiTenant
                                from at in apiTenant.DefaultIfEmpty()
                       select new ApiClientDto
                       {
                          Id = a.Id,
                          AllowedOrigin = a.AllowedOrigin,
                          ApplicationType = a.ApplicationType,
                          Name = a.Name,
                          //OwnedByTenantId = a.OwnedByTenantId,
                          TenantName = at != null ? at.Name : "",
                          IsActive = a.Active,
                          ClientId = a.ClientId,
                          //PlaintSecret = a.Secret,
                      }).FirstOrDefaultAsync();

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Common_Tenant_Find, AppPermissions.Pages_Host_Client)]
        public async Task<PagedResultDto<TenantSummaryDto>> FindTenantForApiClient(FindTenantsInput input)
        {
            var query = (from t in TenantManager.Tenants
                                   .WhereIf(input.IsActive.HasValue, s => s.IsActive == input.IsActive.Value)
                                   .WhereIf(!input.Filter.IsNullOrWhiteSpace(), t => t.Name.Contains(input.Filter) || t.TenancyName.Contains(input.Filter))
                         select new TenantSummaryDto
                         {
                             Id = t.Id,
                             Name = t.Name,
                             TenancyName = t.TenancyName,
                         });

            var tenantCount = await query.CountAsync();
            var tenants = new List<TenantSummaryDto>();
            if (input.UsePagination)
            {
                tenants = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            }
            else
            {
                tenants = await query.OrderBy(input.Sorting).ToListAsync();
            }
            return new PagedResultDto<TenantSummaryDto>(tenantCount, tenants);
        }
    }
}
