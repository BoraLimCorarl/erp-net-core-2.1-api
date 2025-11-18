using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Dto;
using CorarlERP.Lots.Dto;
using System.Threading.Tasks;

namespace CorarlERP.Lots
{
    public interface ILotAppService : IApplicationService
    {
        Task<LotDetailOutput> Create(CreateLotInput input);
        Task<PagedResultDto<LotDetailOutput>> GetList(GetLotListInput input);
        Task<PagedResultDto<LotDetailOutput>> Find(FindLotInput input);
        Task<LotDetailOutput> GetDetail(EntityDto<long> input);
        Task<LotDetailOutput> Update(UpdateLotInput input);
        Task Delete(EntityDto<long> input);
        Task Enable(EntityDto<long> input);
        Task Disable(EntityDto<long> input);
        Task ImportExcel(FileDto input);
        Task<FileDto> ExportExcelTamplate();
    }
}
