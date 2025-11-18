using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.BatchNos.Dto;
using System;
using System.Threading.Tasks;

namespace CorarlERP.BatchNos
{
    public interface IBatchNoFormulaAppService : IApplicationService
    {
        Task<BatchNoFormulaDetailOutput> Create(CreateOrUpdateBatchNoFormulaInput input);
        Task<PagedResultDto<BatchNoFormulaDetailOutput>> GetList(GetBatchNoFormulaListInput input);
        Task<PagedResultDto<BatchNoFormulaDetailOutput>> Find(GetBatchNoFormulaListInput input);
        Task<BatchNoFormulaDetailOutput> GetDetail(EntityDto<long> input);
        Task<BatchNoFormulaDetailOutput> Update(CreateOrUpdateBatchNoFormulaInput input);
        Task Delete(EntityDto<long> input);
        Task Enable(EntityDto<long> input);
        Task Disable(EntityDto<long> input);
    }
}
