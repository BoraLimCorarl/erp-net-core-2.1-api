using System.Collections.Generic;
using CorarlERP.Authorization.Permissions.Dto;

namespace CorarlERP.Authorization.Users.Dto
{
    public class GetUserPermissionsForEditOutput
    {
        public List<FlatPermissionDto> Permissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}