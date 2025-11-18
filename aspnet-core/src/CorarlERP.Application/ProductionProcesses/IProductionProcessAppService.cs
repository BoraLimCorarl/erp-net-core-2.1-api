using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.ProductionProcesses.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ProductionProcesses
{
    public interface IProductionProcessAppService : IApplicationService
    {
        Task<ProductionProcessDetailOutput> Create(CreateOrUpdateProductionProcessInput input);
        Task<PagedResultDto<ProductionProcessDetailOutput>> GetList(GetProductionProcessListInput input);
        Task<PagedResultDto<ProductionProcessDetailOutput>> Find(GetProductionProcessListInput input);
        Task<ProductionProcessDetailOutput> GetDetail(EntityDto<long> input);
        Task<ProductionProcessDetailOutput> Update(CreateOrUpdateProductionProcessInput input);
        Task Delete(EntityDto<long> input);
        Task Enable(EntityDto<long> input);
        Task Disable(EntityDto<long> input);
    }
}
