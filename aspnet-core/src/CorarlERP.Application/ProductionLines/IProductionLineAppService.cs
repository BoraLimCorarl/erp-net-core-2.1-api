using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.ProductionLines.Dto;
using CorarlERP.ProductionPlans.Dto;
using System;
using System.Threading.Tasks;

namespace CorarlERP.ProductionLines
{
    public interface IProductionLineAppService : IApplicationService
    {
        Task<ProductionLineDetailOutput> Create(CreateOrUpdateProductionLineInput input);
        Task<PagedResultDto<ProductionLineDetailOutput>> GetList(GetProductionLineListInput input);
        Task<PagedResultDto<ProductionLineDetailOutput>> Find(GetProductionLineListInput input);
        Task<ProductionLineDetailOutput> GetDetail(EntityDto<long> input);
        Task<ProductionLineDetailOutput> Update(CreateOrUpdateProductionLineInput input);
        Task Delete(EntityDto<long> input);
        Task Enable(EntityDto<long> input);
        Task Disable(EntityDto<long> input);
    }
}
