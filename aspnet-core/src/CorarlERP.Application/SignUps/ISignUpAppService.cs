using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.SignUps.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.SignUps
{
    public interface ISignUpAppService : IApplicationService
    {
        Task<Guid> Create(CreateOrUpdateSignUpInput input);
        Task<PagedResultDto<GetDetailSignUpOutput>> GetList(GetListSignUpInput input);
        Task<ListResultDto<GetDetailSignUpOutput>> Find(GetListSignUpInput input);    
        Task Delete(EntityDto<Guid> input);
        Task Enable(EntityDto<Guid> input);
        Task Disable(EntityDto<Guid> input);
        Task<GetDetailSignUpOutput> GetDetail(EntityDto<Guid> input);
        Task<Guid> Update(CreateOrUpdateSignUpInput input);
        Task GenerateCodeAndPutToCache(GenerateTokenInput input);
        Task VerifyCodeFromCache(VerifyTokenInput input);
        Task LinkTenant(LinkTenantInput input);
        Task UpdateStaus(UpdateStatusInput input);
        Task<PagedResultDto<FindStatusOutput>> FindStatus();
    }
}
