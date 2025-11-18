using Abp.AspNetCore.Mvc.Authorization;
using CorarlERP.Storage;

namespace CorarlERP.Web.Controllers
{
    [AbpMvcAuthorize]
    public class ProfileController : ProfileControllerBase
    {
        public ProfileController(ITempFileCacheManager tempFileCacheManager) :
            base(tempFileCacheManager)
        {
        }
    }
}