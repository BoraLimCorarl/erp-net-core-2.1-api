using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Taxes.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Taxes
{
    public interface ITaxAppService: IApplicationService
    {
        Task<TaxDetailOutput> Create(CreateTaxInput input);
        Task<PagedResultDto<TaxDetailOutput>> GetList(GetTaxListInput input);     
        Task<ListResultDto<TaxDetailOutput>> Find(GetTaxListInput input);
        Task<TaxDetailOutput> Update(UpateTaxInput input);
        Task Delete(EntityDto<long> input);
        Task Enable(EntityDto<long> input);
        Task Disable(EntityDto<long> input);
    }
}
