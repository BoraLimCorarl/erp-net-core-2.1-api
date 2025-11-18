using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.MultiTenancy.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.MultiTenancy
{
   public interface ICompanyProfileAppService : IApplicationService
    {
        Task<TenantDetailOutput> GetDetail(EntityDto<int> input);
        Task<NullableIdDto<int>> Update(UpdateTenantInput input);
        Task<Guid> UpdateAutoSequence(UpdateAutoSequenceInput input);
        Task<UpdateAutoSequenceOutput> GetDetailAutoSequence(EntityDto<Guid> input);
        Task<string> GetLastNo(UpdateTransactionNoInput input);

        Task ReSetDefaultValue();

    }
}
