using System.Threading.Tasks;
using Abp.Application.Services;
using CorarlERP.Configuration.Host.Dto;

namespace CorarlERP.Configuration.Host
{
    public interface IHostSettingsAppService : IApplicationService
    {
        Task<HostSettingsEditDto> GetAllSettings();

        Task UpdateAllSettings(HostSettingsEditDto input);

        Task SendTestEmail(SendTestEmailInput input);
    }
}
