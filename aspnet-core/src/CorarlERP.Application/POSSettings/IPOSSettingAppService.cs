using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.POSSettings.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.POSSettings
{
   public interface IPOSSettingAppService : IApplicationService
    {
        Task<long> CreateOrUpdate(CreateOrUpdatePOSSettingInput input);
        Task<CreateOrUpdatePOSSettingInput> GetDetail();
        Task Delete(EntityDto<long> input);

    }
}
