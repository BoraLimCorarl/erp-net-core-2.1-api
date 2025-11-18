using Microsoft.AspNetCore.Antiforgery;

namespace CorarlERP.Web.Controllers
{
    public class AntiForgeryController : CorarlERPControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}
