using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Dto;
using CorarlERP.UserActivities.Dto;

namespace CorarlERP.UserActivities
{
    public interface IUserActivityAppService : IApplicationService
    {
        Task<PagedResultDto<GetDetailListUserActivityOutput>> GetList(GetDetailListUserActivityinputput input);

        PagedResultDto<GetListActivityOutput>FindUserTtransaction(GetUserActivityInput input);

        PagedResultDto<UserActivityOutput> FindUserActivity(GetUserActivityInput input);

        Task<FileDto> ExportExcel(GetDetailListUserActivityinputput input);

    }
}
