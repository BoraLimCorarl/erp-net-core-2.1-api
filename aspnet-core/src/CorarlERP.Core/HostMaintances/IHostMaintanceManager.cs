using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.HostMaintances
{
    public interface IHostMaintanceManager
    {
        Task AssignPermissionsToTenant(int tenantId,int editionId);
        Task CreateRoleForTenantIfNotExistAndAsssignAdminRole(int tenantId);
        Task AssignPermissionForEachRole(int tenantId, string roleName);
    }
}
