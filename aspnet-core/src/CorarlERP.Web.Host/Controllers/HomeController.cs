using Abp.Auditing;
using Microsoft.AspNetCore.Mvc;

namespace CorarlERP.Web.Controllers
{
    public class HomeController : CorarlERPControllerBase
    {
        [DisableAuditing]
        public IActionResult Index()
        {
            return Redirect("/swagger");
        }
    }
}
