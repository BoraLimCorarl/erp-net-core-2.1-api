using Microsoft.Extensions.Configuration;

namespace CorarlERP.Configuration
{
    public interface IAppConfigurationAccessor
    {
        IConfigurationRoot Configuration { get; }
    }
}
