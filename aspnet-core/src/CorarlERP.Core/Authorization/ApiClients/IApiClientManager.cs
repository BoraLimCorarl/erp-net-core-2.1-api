using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Authorization.ApiClients
{
    public interface IApiClientManager: IDomainService
    {
        Task<ApiClient> GetAsync(Guid id, bool noTracking = true);
        Task<ApiClient> GetAsync(string clientId, bool noTracking = true);
        Task<ApiClientWithTenant> GetWithTenantAsync(string clientId);
        Task<IdentityResult> CreateAsync(ApiClient token);
        Task<IdentityResult> DeleteAsync(ApiClient token);
        Task<IdentityResult> DeleteAsync(Guid tokenId);
        Task<IdentityResult> UpdateAsync(ApiClient token);
        Task<ApplicationTypes> ValidateClientSecretAsync(string clientSecret, string clientId);

    }
}
