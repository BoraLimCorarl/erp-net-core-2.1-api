using System.Threading.Tasks;
using Abp.Application.Services;
using CorarlERP.Install.Dto;

namespace CorarlERP.Install
{
    public interface IInstallAppService : IApplicationService
    {
        Task Setup(InstallDto input);

        AppSettingsJsonDto GetAppSettingsJson();

        CheckDatabaseOutput CheckDatabase();
    }
}