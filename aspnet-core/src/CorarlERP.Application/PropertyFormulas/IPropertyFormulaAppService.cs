using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.PropertyFormulas.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.PropertyFormulas
{
    public interface IPropertyFormulaAppService : IApplicationService
    {
        Task<long> Create(CreateOrUpdatePropertyFormulaInput input);
        Task Delete(EntityDto<long> input);
        Task Enable(EntityDto<long> input);
        Task Disable(EntityDto<long> input);
        Task<PagedResultDto<GetListFomularPropertyOutput>> GetList(GetListFomularPropertyInput input);
        Task<GetDetailPropertyFormulaOutput> GetDetail(EntityDto<long> input);
        Task<PagedResultDto<GetListFomularPropertyOutput>> Find(GetListFomularPropertyInput input);
        Task<long> Update(CreateOrUpdatePropertyFormulaInput input);
        Task UpdateAutoItemCode(CreateOrUpdateAutoItemCodeInput input);
        Task UpdateItemCodeSetting(CreateOrUpdateAutoItemCodeInput input);
        Task<CreateOrUpdateAutoItemCodeInput> GetTenantDefaultItemCode();
    }
}
