using Abp.Modules;
using Abp.Reflection.Extensions;

namespace CorarlERP
{
    public class CorarlERPClientModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CorarlERPClientModule).GetAssembly());
        }
    }
}
