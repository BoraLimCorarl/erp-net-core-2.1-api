using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Caches.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Caches
{
    public interface ICacheAppService : IApplicationService
    {
        Task<CreateOrUpdateCache> CreateOrUpdate(CreateOrUpdateCache input);
        Task<CreateOrUpdateCache> GetDetail(string keyName);
    }
}
