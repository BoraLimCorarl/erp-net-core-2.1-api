using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Runtime.Caching;
using Abp.UI;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using CorarlERP.Authorization.Users;
using Abp.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Abp.Collections.Extensions;

namespace CorarlERP.Authorization.Roles
{
    /// <summary>
    /// Role manager.
    /// Used to implement domain logic for roles.
    /// </summary>
    public class RoleManager : AbpRoleManager<Role, User>
    {
        private readonly ILocalizationManager _localizationManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IDefaultData _defaultPermissions;
        private readonly IPermissionManager _permissionManager;
        public RoleManager(
            RoleStore store,
            IEnumerable<IRoleValidator<Role>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<RoleManager> logger,
            IPermissionManager permissionManager,
            IRoleManagementConfig roleManagementConfig,
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager,
            ILocalizationManager localizationManager,
            IDefaultData defaultPermissions)
            : base(
                store,
                roleValidators,
                keyNormalizer,
                errors,
                logger,
                permissionManager,
                cacheManager,
                unitOfWorkManager,
                roleManagementConfig)
        {
            _localizationManager = localizationManager;
            _unitOfWorkManager = unitOfWorkManager;
            _defaultPermissions = defaultPermissions;
            _permissionManager = permissionManager;
        }

        public override Task SetGrantedPermissionsAsync(Role role, IEnumerable<Permission> permissions)
        {
            CheckPermissionsToUpdate(role, permissions);

            return base.SetGrantedPermissionsAsync(role, permissions);
        }

        private void CheckPermissionsToUpdate(Role role, IEnumerable<Permission> permissions)
        {
            if (role.Name == StaticRoleNames.Host.Admin &&
                (!permissions.Any(p => p.Name == AppPermissions.Pages_Administration_Roles_Edit) ||
                 !permissions.Any(p => p.Name == AppPermissions.Pages_Administration_Users_ChangePermissions)))
            {
                throw new UserFriendlyException(L("YouCannotRemoveUserRolePermissionsFromAdminRole"));
            }
        }



        [UnitOfWork]
        public virtual async Task<IdentityResult> CreateNoExistStaticRoles(int tenantId)
        {
            var staticRoleDefinitions = RoleManagementConfig.StaticRoles.Where(sr => sr.Side == MultiTenancySides.Tenant).ToList();


            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var query = await Roles.ToListAsync();
                var toCreateRoles = staticRoleDefinitions.Where(u => !query.Any(s => s.Name == u.RoleName));


                foreach (var staticRoleDefinition in toCreateRoles)
                {
                    var role = new Role
                    {
                        TenantId = tenantId,
                        Name = staticRoleDefinition.RoleName,
                        DisplayName = staticRoleDefinition.RoleName,
                        IsStatic = true
                    };

                    await CreateAsync(role);

                }
            }

            return IdentityResult.Success;
        }

        public async Task GrantPermissionsToTenantRoleAsync(Role role)
        {

            if (_defaultPermissions != null &&
                _defaultPermissions.StaticPermissions != null &&
                _defaultPermissions.StaticPermissions.ContainsKey(role.Name))
            {

                var employeePermissions = _defaultPermissions.StaticPermissions[role.Name];              
                var permissions = _permissionManager
                                  .GetAllPermissions(multiTenancySides: Abp.MultiTenancy.MultiTenancySides.Tenant)
                                  .Where(u => employeePermissions.Contains(u.Name)).ToList();

                await SetGrantedPermissionsAsync(role, permissions);


            }
        }


        private new string L(string name)
        {
            return _localizationManager.GetString(CorarlERPConsts.LocalizationSourceName, name);
        }
    }
}