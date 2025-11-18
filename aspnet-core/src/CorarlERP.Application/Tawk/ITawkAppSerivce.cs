using Abp.Application.Services;
using CorarlERP.SignUps.Dto;
using CorarlERP.Tawk.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Tawk
{
    public interface ITawkAppService : IApplicationService
    {
        Task<TawkDto> GetTawk(ValidateInput input);
    }
}
