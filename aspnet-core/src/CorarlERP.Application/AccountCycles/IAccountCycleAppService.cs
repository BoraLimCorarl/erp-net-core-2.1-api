using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.AccountCycles.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.AccountCycles
{
   public interface IAccountCycleAppService : IApplicationService
    {
        Task<AccountCycleDetailOutput> Create(CreateOrUpdateAccountCycleInput input);
        Task<PagedResultDto<AccountCycleDetailOutput>> GetList(GetListAccountCycleInput input);          
        Task Delete();   
    }
}
