using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.UserGroups.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.UserGroups
{
   public interface IUserGroupAppService :  IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateUserGroupInput input);
        Task<PagedResultDto<GetListUserGroupOutput>> GetList(GetListUserGroupInput input);
        Task<PagedResultDto<GetListUserGroupOutput>> Find(GetListUserGroupInput input);
        Task<UserGroupDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<NullableIdDto<Guid>> Update(UpdateUserGroupInput input);
        Task Delete(EntityDto<Guid> input);
        Task Enable(EntityDto<Guid> input);
        Task Disable(EntityDto<Guid> input);
    }
}
