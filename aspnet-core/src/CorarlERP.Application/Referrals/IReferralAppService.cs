using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Referrals.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Referrals
{
    public interface IReferralAppService : IApplicationService
    {
        Task<long> Create(CreateOrUpdateReferallInput input);
        Task<PagedResultDto<ReferallDetailOutput>> GetList(GetReferallListInput input);
        Task<PagedResultDto<ReferallDetailOutput>> Find(GetReferallListInput input);
        Task<long> Update(CreateOrUpdateReferallInput input);
        Task Delete(EntityDto<long> input);
        Task Enable(EntityDto<long> input);
        Task Disable(EntityDto<long> input);
        Task<ReferallDetailOutput> GetDetail(EntityDto<long> input);
    }
}
