using Microsoft.AspNetCore.Mvc;
using CorarlERP.Web.Controllers;

namespace CorarlERP.Web.Public.Controllers
{
    public class AboutController : CorarlERPControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}