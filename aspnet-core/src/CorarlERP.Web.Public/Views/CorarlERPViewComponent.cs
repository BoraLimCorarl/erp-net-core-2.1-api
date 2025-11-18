using Abp.AspNetCore.Mvc.ViewComponents;

namespace CorarlERP.Web.Public.Views
{
    public abstract class CorarlERPViewComponent : AbpViewComponent
    {
        protected CorarlERPViewComponent()
        {
            LocalizationSourceName = CorarlERPConsts.LocalizationSourceName;
        }
    }
}