using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.Journals.Dto;
using CorarlERP.Productions.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Productions
{
   public interface IProductionAppService : IApplicationService
    {

        Task<NullableIdDto<Guid>> Create(CreateProductionInput input);
        Task<PageResultProductioinSummary> GetList(GetListProductionInput input);
        Task<PagedResultDto<ProductionGetListOutput>> Find(GetListProductionInput input);
        Task<ProductionDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<NullableIdDto<Guid>> Update(UpdateProductionInput input);
        Task<ProductionDetailOutput> GetListProductionOrderForItemReceipt(EntityDto<Guid> input);
        Task<ProductionDetailOutput> GetListProductionOrderForItemIssue(EntityDto<Guid> input);

        Task Delete(CarlEntityDto input);
        Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input);
        Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input);
        Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input);

        Task Calculation(ProductionCalculationInput input);
    }
}
