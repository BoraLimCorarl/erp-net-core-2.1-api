using Abp.Application.Services;
using CorarlERP.Dto;
using CorarlERP.Logging.Dto;

namespace CorarlERP.Logging
{
    public interface IWebLogAppService : IApplicationService
    {
        GetLatestWebLogsOutput GetLatestWebLogs();

        FileDto DownloadWebLogs();
    }
}
