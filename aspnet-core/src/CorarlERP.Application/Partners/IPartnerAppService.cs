using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Partners.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Partners
{
    public interface IPartnerAppService : IApplicationService
    {
        Task<PagedResultDto<GetListPartnerOutPut>> GetList(GetPartnerListInput input);
    }
}
