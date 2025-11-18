using Abp.MultiTenancy;
using Abp.Zero.Configuration;

namespace CorarlERP.Authorization.Roles
{
    public static class AppRoleConfig
    {
        public static void Configure(IRoleManagementConfig roleManagementConfig)
        {
            //Static host roles

            roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    StaticRoleNames.Host.Admin,
                    MultiTenancySides.Host,
                    grantAllPermissionsByDefault: true)
                );

            //Static tenant roles

            roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    StaticRoleNames.Tenants.Admin,
                    MultiTenancySides.Tenant,
                    grantAllPermissionsByDefault: true)
                );


            roleManagementConfig.StaticRoles.Add(
               new StaticRoleDefinition(
                   StaticRoleNames.Tenants.AccountingManager,
                   MultiTenancySides.Tenant)
               );

            roleManagementConfig.StaticRoles.Add(
               new StaticRoleDefinition(
                   StaticRoleNames.Tenants.APAccountant,
                   MultiTenancySides.Tenant)
               );

            roleManagementConfig.StaticRoles.Add(
             new StaticRoleDefinition(
                 StaticRoleNames.Tenants.ARAccountant,
                 MultiTenancySides.Tenant)
             );



            roleManagementConfig.StaticRoles.Add(
           new StaticRoleDefinition(
               StaticRoleNames.Tenants.SaleManager,
               MultiTenancySides.Tenant)
           );

            roleManagementConfig.StaticRoles.Add(
         new StaticRoleDefinition(
             StaticRoleNames.Tenants.PurchaseManager,
             MultiTenancySides.Tenant)
         );

            roleManagementConfig.StaticRoles.Add(
         new StaticRoleDefinition(
             StaticRoleNames.Tenants.WarehouseManager,
             MultiTenancySides.Tenant)
         );

            roleManagementConfig.StaticRoles.Add(
           new StaticRoleDefinition(
               StaticRoleNames.Tenants.StockController,
               MultiTenancySides.Tenant)
           );

            //roleManagementConfig.StaticRoles.Add(
            //    new StaticRoleDefinition(
            //        StaticRoleNames.Tenants.User,
            //        MultiTenancySides.Tenant)
            //    );
        }
    }
}
