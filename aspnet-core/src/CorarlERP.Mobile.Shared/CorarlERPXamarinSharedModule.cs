using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace CorarlERP
{
    [DependsOn(typeof(CorarlERPClientModule), typeof(AbpAutoMapperModule))]
    public class CorarlERPXamarinSharedModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Localization.IsEnabled = false;
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CorarlERPXamarinSharedModule).GetAssembly());
        }
    }
}