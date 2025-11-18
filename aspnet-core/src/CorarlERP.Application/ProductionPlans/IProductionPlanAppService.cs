using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.ProductionPlans.Dto;
using CorarlERP.Productions.Dto;
using System;
using System.Threading.Tasks;

namespace CorarlERP.ProductionPlans
{
    public interface IProductionPlanAppService : IApplicationService
    {
        Task<ProductionPlanDetailOutput> Create(CreateOrUpdateProductionPlanInput input);
        Task<PageResultProductioinPlanSummary> GetList(GetProductionPlanListInput input);
        Task<PagedResultDto<ProductionPlanDetailOutput>> Find(GetProductionPlanListInput input);
        Task<ProductionPlanDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<ProductionPlanDetailOutput> Update(CreateOrUpdateProductionPlanInput input);
        Task Delete(EntityDto<Guid> input);
        Task Close(EntityDto<Guid> input);
        Task Open(EntityDto<Guid> input);
        Task Calculation(ProductionPlanCalculationInput input);
        Task CalculateById(EntityDto<Guid> input);
    }
}
