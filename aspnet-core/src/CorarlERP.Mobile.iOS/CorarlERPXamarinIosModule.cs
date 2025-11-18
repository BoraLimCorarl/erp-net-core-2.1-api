using Abp.Modules;
using Abp.Reflection.Extensions;

namespace CorarlERP
{
    [DependsOn(typeof(CorarlERPXamarinSharedModule))]
    public class CorarlERPXamarinIosModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CorarlERPXamarinIosModule).GetAssembly());
        }
    }
}