using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.MultiTenancy.Dto;

namespace CorarlERP.MultiTenancy
{
    public interface ITenantAppService : IApplicationService
    {
        Task<PagedResultDto<TenantListDto>> GetTenants(GetTenantsInput input);
        Task<PagedResultDto<TenantSummaryDto>> FindTenants(FindTenantsInput input);

        Task CreateTenant(CreateTenantInput input);

        Task<TenantEditDto> GetTenantForEdit(EntityDto input);
        Task<GetSubscriptionInput> GetTenantSubscription(EntityDto<Guid> input);

        Task UpdateTenant(TenantEditDto input);

        Task DeleteTenant(EntityDto input);

        Task<GetTenantFeaturesEditOutput> GetTenantFeaturesForEdit(EntityDto input);

        Task UpdateTenantFeatures(UpdateTenantFeaturesInput input);

        Task ResetTenantSpecificFeatures(EntityDto input);

        Task UnlockTenantAdmin(EntityDto input);
        Task RenewSubscription(GetSubscriptionInput input);
        Task<PagedResultDto<GetSubscriptionDetailOutput>> GetListSubscription(GetSubscriptionDetailInput input);
        Task EnableDebugMode(GetDebugInput input);
        Task DisableDebugMode(GetDebugInput input);
        Task Disable(EntityDto input);
        Task Enable(EntityDto input);
    }
}
