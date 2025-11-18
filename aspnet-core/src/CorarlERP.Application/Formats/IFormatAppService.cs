using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Formats.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Formats
{
   public interface IFormatAppService : IApplicationService
    {
        Task<PagedResultDto<FormatDetailOutput>> GetList(GetFormatListInput input);
        Task<ListResultDto<FormatDetailOutput>> FindNumber(GetFormatListInput input);
        Task<ListResultDto<FormatDetailOutput>> FindDate(GetFormatListInput input);
        Task Sync();
    }
}
