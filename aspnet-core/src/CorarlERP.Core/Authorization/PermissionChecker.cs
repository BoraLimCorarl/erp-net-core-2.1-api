using Abp.Authorization;
using CorarlERP.Authorization.Roles;
using CorarlERP.Authorization.Users;

namespace CorarlERP.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {

        }
    }
}
