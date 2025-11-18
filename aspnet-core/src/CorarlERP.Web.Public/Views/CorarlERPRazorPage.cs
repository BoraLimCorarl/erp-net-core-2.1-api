using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace CorarlERP.Web.Public.Views
{
    public abstract class CorarlERPRazorPage<TModel> : AbpRazorPage<TModel>
    {
        [RazorInject]
        public IAbpSession AbpSession { get; set; }

        protected CorarlERPRazorPage()
        {
            LocalizationSourceName = CorarlERPConsts.LocalizationSourceName;
        }
    }
}
