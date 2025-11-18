using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Authorization.Users.Dto;

namespace CorarlERP.Authorization.Users
{
    public interface IUserLoginAppService : IApplicationService
    {
        Task<ListResultDto<UserLoginAttemptDto>> GetRecentUserLoginAttempts();
    }
}
