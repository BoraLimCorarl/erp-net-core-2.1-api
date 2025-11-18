using System.Threading.Tasks;
using Abp.Dependency;
using Microsoft.AspNetCore.Http;
using CorarlERP.Sessions;
using CorarlERP.Sessions.Dto;

namespace CorarlERP.Web.Session
{
    public class PerRequestSessionCache : IPerRequestSessionCache, ITransientDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISessionAppService _sessionAppService;

        public PerRequestSessionCache(
            IHttpContextAccessor httpContextAccessor,
            ISessionAppService sessionAppService)
        {
            _httpContextAccessor = httpContextAccessor;
            _sessionAppService = sessionAppService;
        }

        public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformationsAsync(AuthenticationBaseInput input)
        {        
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return await _sessionAppService.GetCurrentLoginInformations(input);
            }

            var cachedValue = httpContext.Items["__PerRequestSessionCache"] as GetCurrentLoginInformationsOutput;
            if (cachedValue == null)
            {
                cachedValue = await _sessionAppService.GetCurrentLoginInformations(input);
                httpContext.Items["__PerRequestSessionCache"] = cachedValue;
            }

            return cachedValue;
        }
    }
}