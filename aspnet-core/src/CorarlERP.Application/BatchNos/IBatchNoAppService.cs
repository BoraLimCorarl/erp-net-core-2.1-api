using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.BatchNos.Dto;
using System;
using System.Threading.Tasks;

namespace CorarlERP.BatchNos
{
    public interface IBatchNoAppService : IApplicationService
    {
        Task<PagedResultDto<BatchNoDetailOutput>> Find(GetBatchNoListInput input);
        Task<ListResultDto<GenerateBatchNoOutput>> GenerateBatchNo(GenerateBatchNoInput input);

        Task<PagedResultDto<FindBatchNoOutput>> FindBatchNoForIssue(FindBatchNoInput input);

    }
}
