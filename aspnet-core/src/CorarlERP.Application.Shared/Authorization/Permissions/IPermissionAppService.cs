using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Authorization.Permissions.Dto;

namespace CorarlERP.Authorization.Permissions
{
    public interface IPermissionAppService : IApplicationService
    {
        ListResultDto<FlatPermissionWithLevelDto> GetAllPermissions();
    }
}
