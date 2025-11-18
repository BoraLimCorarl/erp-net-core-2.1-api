using System.Linq;
using Abp.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using CorarlERP.Editions;
using CorarlERP.EntityFrameworkCore;

namespace CorarlERP.Migrations.Seed.Tenants
{
    public class DefaultTenantBuilder
    {
        private readonly CorarlERPDbContext _context;

        public DefaultTenantBuilder(CorarlERPDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateDefaultTenant();
        }

        private void CreateDefaultTenant()
        {
            //Default tenant

            var defaultTenant = _context.Tenants.IgnoreQueryFilters().FirstOrDefault(t => t.TenancyName == MultiTenancy.Tenant.DefaultTenantName);
            if (defaultTenant == null)
            {
                defaultTenant = new MultiTenancy.Tenant(AbpTenantBase.DefaultTenantName, AbpTenantBase.DefaultTenantName);

                var defaultEdition = _context.Editions.IgnoreQueryFilters().FirstOrDefault(e => e.Name == EditionManager.AdvanceEditName);
                if (defaultEdition != null)
                {
                    defaultTenant.EditionId = defaultEdition.Id;
                }

                _context.Tenants.Add(defaultTenant);
                _context.SaveChanges();
            }
        }
    }
}
