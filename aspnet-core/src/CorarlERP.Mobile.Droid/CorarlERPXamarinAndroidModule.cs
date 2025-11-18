using Abp.Modules;
using Abp.Reflection.Extensions;

namespace CorarlERP
{
    [DependsOn(typeof(CorarlERPXamarinSharedModule))]
    public class CorarlERPXamarinAndroidModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CorarlERPXamarinAndroidModule).GetAssembly());
        }
    }
}