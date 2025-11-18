using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Authorization.ApiClients.Dto;
using CorarlERP.MultiTenancy.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Authorization.ApiClients
{
    public interface IApiClientAppService : IApplicationService
    {
        Task Delete(EntityDto<Guid> input);
        Task<PagedResultDto<ApiClientDto>> GetList(GetListApiClientInput input);
        Task Create(ApiClientInput input);
        Task Update(ApiClientUpdateInput input);
        Task Enable(EntityDto<Guid> input);
        Task Disable(EntityDto<Guid> input);
        Task<ApiClientDto> GetDetail(EntityDto<Guid> input);
        Task<PagedResultDto<TenantSummaryDto>> FindTenantForApiClient(FindTenantsInput input);
    }

}
