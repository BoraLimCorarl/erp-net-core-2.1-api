using Abp;
using Abp.Dependency;
using Abp.EntityFrameworkCore.Configuration;
using Abp.IdentityServer4;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using CorarlERP.Configuration;
using CorarlERP.EntityHistory;
using CorarlERP.Migrations.Seed;

namespace CorarlERP.EntityFrameworkCore
{
    [DependsOn(
        typeof(AbpZeroCoreEntityFrameworkCoreModule),
        typeof(CorarlERPCoreModule),
        typeof(AbpZeroCoreIdentityServerEntityFrameworkCoreModule)
        )]
    public class CorarlERPEntityFrameworkCoreModule : AbpModule
    {
        /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
        public bool SkipDbContextRegistration { get; set; }

        public bool SkipDbSeed { get; set; }

        public override void PreInitialize()
        {
            if (!SkipDbContextRegistration)
            {
                Configuration.Modules.AbpEfCore().AddDbContext<CorarlERPDbContext>(options =>
                {
                    var configurationAccessor = IocManager.Resolve<IAppConfigurationAccessor>();

                    if (options.ExistingConnection != null)
                    {
                        if (bool.Parse(configurationAccessor.Configuration["UsePostgreSql"]))
                        {
                            CorarlERPDbContextConfigurer.ConfigurePostgreSql(options.DbContextOptions, options.ExistingConnection);
                        }
                        else
                        {
                            CorarlERPDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                        }
                    }
                    else
                    {
                        if (bool.Parse(configurationAccessor.Configuration["UsePostgreSql"]))
                        {
                            CorarlERPDbContextConfigurer.ConfigurePostgreSql(options.DbContextOptions, options.ConnectionString);
                        }
                        else
                        {
                            CorarlERPDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                        }                           
                    }
                });
            }

            //Uncomment below line to write change logs for the entities below:
            //Configuration.EntityHistory.Selectors.Add("CorarlERPEntities", EntityHistoryHelper.TrackedTypes);
            //Configuration.CustomConfigProviders.Add(new EntityHistoryConfigProvider(Configuration));
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CorarlERPEntityFrameworkCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            var configurationAccessor = IocManager.Resolve<IAppConfigurationAccessor>();

            using (var scope = IocManager.CreateScope())
            {
                if (!SkipDbSeed && scope.Resolve<DatabaseCheckHelper>().Exist(configurationAccessor.Configuration["ConnectionStrings:Default"]))
                {
                    SeedHelper.SeedHostDb(IocManager);
                }
            }
        }
    }
}
