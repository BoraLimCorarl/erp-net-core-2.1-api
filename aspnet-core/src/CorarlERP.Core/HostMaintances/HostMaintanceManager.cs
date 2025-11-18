using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using CorarlERP.Authorization.Roles;
using CorarlERP.MultiTenancy;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using CorarlERP.Editions;
using System.Linq;
using CorarlERP.Features;
using System.Collections.Generic;
using Abp;
using Abp.Zero.Configuration;

namespace CorarlERP.HostMaintances
{
    public class HostMaintanceManager : CorarlERPDomainServiceBase, IHostMaintanceManager
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly RoleManager _roleManager;
        private readonly TenantManager _tenantManager;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly EditionManager _editionManager;
        private readonly IRepository<Role, int> _roleRepository;
        public HostMaintanceManager(
            IUnitOfWorkManager unitOfWorkManager,
            RoleManager roleManager,
            TenantManager tenantManager,
            IRepository<Tenant> tenantRepository,
            EditionManager editionManager,
            IRepository<Role, int> roleRepository
            )
        {
            _unitOfWorkManager = unitOfWorkManager;
            _roleManager = roleManager;
            _tenantRepository = tenantRepository;
            _tenantManager = tenantManager;
            _editionManager = editionManager;
            _roleRepository = roleRepository;
        }

        public async Task AssignPermissionsToTenant(int tenantId, int editionId)
        {

            //grant permissions to static roles other than admin
            var featureValues = new List<NameValue>();
            var roles = new List<Role>();
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var featureByTenant = await _tenantManager.GetFeatureValuesAsync(tenantId);
                    featureValues = featureByTenant.Where(t => t.Value == "true").ToList();
                    roles = await _roleRepository.GetAll().Where(t => t.TenantId == tenantId && t.IsStatic || (t.Name == StaticRoleNames.Tenants.APAccountant || t.Name == StaticRoleNames.Tenants.ARAccountant || t.Name == StaticRoleNames.Tenants.StockController)).ToListAsync();
                }
            }
            var staticPermissions = new List<string>();
            var staticRoleDefinitions = new List<StaticRoleDefinition>();
            staticPermissions.Add(StaticRoleNames.Tenants.Admin);
            if (featureValues.Select(t => t.Name).Contains(AppFeatures.AccountingFeature))
            {
                staticPermissions.Add(StaticRoleNames.Tenants.AccountingManager);
                staticPermissions.Add(StaticRoleNames.Tenants.ARAccountant);
                staticPermissions.Add(StaticRoleNames.Tenants.APAccountant);

            }
            if (featureValues.Select(t => t.Name).Contains(AppFeatures.VendorsFeaturePurchases))
            {
                staticPermissions.Add(StaticRoleNames.Tenants.PurchaseManager);
            }

            if (featureValues.Select(t => t.Name).Contains(AppFeatures.CustomersFeature))
            {
                staticPermissions.Add(StaticRoleNames.Tenants.SaleManager);
            }
            if (featureValues.Select(t => t.Name).Contains(AppFeatures.InventoryFeature))
            {
                staticPermissions.Add(StaticRoleNames.Tenants.WarehouseManager);
                staticPermissions.Add(StaticRoleNames.Tenants.StockController);
            }

            //  roles = roles.Where(t => !staticPermissions.Contains(t.Name)).ToList();
            foreach (var s in staticPermissions)
            {
                var role = roles.Where(t => t.Name == s).FirstOrDefault();
                await AssignPermissionStatic(s, tenantId, role);
            }

        }

        private async Task AssignPermissionStatic(string staticPermissions, int tenantId, Role role)
        {

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var updateRole = await _roleManager.Roles
                                              .Include(i => i.Permissions)
                                              .FirstOrDefaultAsync(r => role != null && r.Name == role.Name && r.IsStatic);
                    if (updateRole != null && updateRole.Name != StaticRoleNames.Tenants.APAccountant  && updateRole.Name  != StaticRoleNames.Tenants.ARAccountant && updateRole.Name != StaticRoleNames.Tenants.StockController)
                    {
                        await _roleManager.ResetAllPermissionsAsync(updateRole);
                        await _roleManager.UpdateAsync(updateRole);
                    }

                }
                await uow.CompleteAsync();
            }

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    if (role != null)
                    {
                        var updateRole = await _roleManager.Roles
                                                .Include(i => i.Permissions)
                                                .Where(s => s.IsStatic)
                                                .FirstOrDefaultAsync(r => r.Name == role.Name);
                        if (updateRole != null)
                        {
                            updateRole.IsStatic = updateRole != null && (updateRole.Name == StaticRoleNames.Tenants.APAccountant || updateRole.Name == StaticRoleNames.Tenants.ARAccountant || updateRole.Name == StaticRoleNames.Tenants.StockController) ? false : true;
                            if (updateRole.Name == StaticRoleNames.Tenants.Admin)
                            {
                                await _roleManager.GrantAllPermissionsAsync(updateRole);
                                await _roleManager.UpdateAsync(updateRole);
                            }

                            else
                            {

                                if (updateRole.Name != StaticRoleNames.Tenants.APAccountant && updateRole.Name != StaticRoleNames.Tenants.ARAccountant && updateRole.Name != StaticRoleNames.Tenants.StockController)
                                {

                                    await _roleManager.GrantPermissionsToTenantRoleAsync(updateRole);
                                    await _roleManager.UpdateAsync(updateRole);
                                }

                            }
                        }

                    }
                    else
                    {
                        var item = new Role(tenantId, staticPermissions, staticPermissions);
                        item.IsStatic = item != null && (item.Name == StaticRoleNames.Tenants.APAccountant || item.Name == StaticRoleNames.Tenants.ARAccountant || item.Name == StaticRoleNames.Tenants.StockController) ? false : true;
                        await _roleManager.CreateAsync(item);
                        await CurrentUnitOfWork.SaveChangesAsync();
                        await _roleManager.GrantPermissionsToTenantRoleAsync(item);
                    }

                }
                await uow.CompleteAsync();
            }
        }

        public async Task CreateRoleForTenantIfNotExistAndAsssignAdminRole(int tenantId)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    await _roleManager.CreateNoExistStaticRoles(tenantId);
                    await _unitOfWorkManager.Current.SaveChangesAsync(); //To get static role ids

                    //grant all permissions to admin role
                    var adminRole = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == StaticRoleNames.Tenants.Admin);
                    if (adminRole != null)
                    {
                        await _roleManager.GrantAllPermissionsAsync(adminRole);
                        await _unitOfWorkManager.Current.SaveChangesAsync();
                    }
                }


                await uow.CompleteAsync();
            }


        }




        public async Task AssignPermissionForEachRole(int tenantId, string roleName)
        {

            if (roleName == StaticRoleNames.Tenants.Admin)
            {
                await CreateRoleForTenantIfNotExistAndAsssignAdminRole(tenantId);
            }


            //reset
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var role = await _roleManager.Roles
                                                 .Include(i => i.Permissions)
                                                 .FirstOrDefaultAsync(r => r.Name == roleName);
                    if (role != null)
                    {
                        await _roleManager.ResetAllPermissionsAsync(role);
                        await _roleManager.UpdateAsync(role);
                        await _unitOfWorkManager.Current.SaveChangesAsync();
                    }
                }


                await uow.CompleteAsync();
            }

            //reassign
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var role = await _roleManager.Roles
                                                 .Include(i => i.Permissions)
                                                 .FirstOrDefaultAsync(r => r.Name == roleName);
                    if (role != null)
                    {
                        role.IsDefault = roleName == StaticRoleNames.Tenants.AccountingManager;
                        await _roleManager.GrantPermissionsToTenantRoleAsync(role);
                        await _roleManager.UpdateAsync(role);
                        await _unitOfWorkManager.Current.SaveChangesAsync();
                    }
                }


                await uow.CompleteAsync();
            }
        }



    }
}
