using System.Threading.Tasks;
using Abp.Application.Services;
using CorarlERP.Sessions.Dto;

namespace CorarlERP.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
      //  Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();

        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations(AuthenticationBaseInput input);

        Task<UpdateUserSignInTokenOutput> UpdateUserSignInToken();
        Task<GetCurrentLoginInformationsSummaryOutput>  GetCurrentLoginInformationForMobile();
    }
}
