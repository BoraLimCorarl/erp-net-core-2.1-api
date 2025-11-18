using System.Threading.Tasks;
using CorarlERP.Sessions.Dto;

namespace CorarlERP.Web.Session
{
    public interface IPerRequestSessionCache
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformationsAsync(AuthenticationBaseInput input);
    }
}
