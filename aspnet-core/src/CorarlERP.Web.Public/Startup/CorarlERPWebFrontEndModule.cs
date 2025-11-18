using Abp.AspNetZeroCore;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using CorarlERP.Configuration;
using CorarlERP.EntityFrameworkCore;
using Abp.Hangfire.Configuration;

namespace CorarlERP.Web.Public.Startup
{
    [DependsOn(
        typeof(CorarlERPWebCoreModule)
    )]
    public class CorarlERPWebFrontEndModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public CorarlERPWebFrontEndModule(IHostingEnvironment env, CorarlERPEntityFrameworkCoreModule abpZeroTemplateEntityFrameworkCoreModule)
        {
            _appConfiguration = env.GetAppConfiguration();
            abpZeroTemplateEntityFrameworkCoreModule.SkipDbSeed = true;
        }

        public override void PreInitialize()
        {
            Configuration.Modules.AbpWebCommon().MultiTenancy.DomainFormat = _appConfiguration["App:WebSiteRootAddress"] ?? "http://localhost:45776/";
            Configuration.Modules.AspNetZero().LicenseCode = _appConfiguration["AbpZeroLicenseCode"];

            //Changed AntiForgery token/cookie names to not conflict to the main application while redirections.
            Configuration.Modules.AbpWebCommon().AntiForgery.TokenCookieName = "Public-XSRF-TOKEN";
            Configuration.Modules.AbpWebCommon().AntiForgery.TokenHeaderName = "Public-X-XSRF-TOKEN";

            Configuration.BackgroundJobs.IsJobExecutionEnabled = true;//default false
            //Configure to use Hangfire as background job manager. Remove these lines to use default background job manager, instead of Hangfire.
            //Configuration.BackgroundJobs.UseHangfire(configuration => configuration.GlobalConfiguration.UseSqlServerStorage("Default", new SqlServerStorageOptions { QueuePollInterval = TimeSpan.FromSeconds(1) }));



            Configuration.Navigation.Providers.Add<FrontEndNavigationProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CorarlERPWebFrontEndModule).GetAssembly());
        }
    }
}
