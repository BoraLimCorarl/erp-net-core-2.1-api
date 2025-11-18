using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Classes.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Classes
{
    public interface IClassAppService : IApplicationService
    {
        Task<ClassDetailOutput> Create(CreateClassInput input);
        Task<PagedResultDto<ClassDetailOutput>> GetList(GetClassListInput input);
        Task<PagedResultDto<ClassDetailOutput>> Find(GetClassListInput input);
        Task<ClassDetailOutput> GetDetail(EntityDto<long> input);
        Task<ClassDetailOutput> Update(UpdateClassInput input);
        Task Delete(EntityDto<long> input);
        Task Enable(EntityDto<long> input);
        Task Disable(EntityDto<long> input);
    }
}
