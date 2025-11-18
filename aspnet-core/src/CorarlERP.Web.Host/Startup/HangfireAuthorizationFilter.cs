using Abp.Authorization;
using Abp.Dependency;
using Abp;
using Hangfire.Dashboard;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace CorarlERP.Web.Startup
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter //AbpHangfireAuthorizationFilter
    {
        private readonly string _requiredPermissionName;
        public HangfireAuthorizationFilter(string requiredPermissionName) { _requiredPermissionName = requiredPermissionName; }

        public bool Authorize(DashboardContext context)
        {
            //if we have a cookies and we are in release mode
            var cookies = context.GetHttpContext().Request.Cookies;
            if (cookies["Abp.AuthToken"] != null)
            {
                var jwtCookie = cookies["Abp.AuthToken"];
                string jwtToken = jwtCookie;
                if (jwtToken == "") return false;
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                JwtSecurityToken securityToken = handler.ReadToken(jwtToken) as JwtSecurityToken;

                UserIdentifier user = UserIdentifier.Parse(securityToken.Claims.Where(c => c.Type.StartsWith("user_identifier")).FirstOrDefault().Value);
                if (user == null || user.UserId <= 0) return false;

                using (var permissionChecker = IocManager.Instance.ResolveAsDisposable<IPermissionChecker>())
                {
                    var auth = permissionChecker.Object.IsGranted(user, _requiredPermissionName);
                    return auth;
                }
            }

            return false;
        }

    }
}
