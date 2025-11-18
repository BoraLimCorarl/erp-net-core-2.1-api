using System.Threading.Tasks;
using Abp.Application.Services;

namespace CorarlERP.MultiTenancy
{
    public interface ISubscriptionAppService : IApplicationService
    {
        Task UpgradeTenantToEquivalentEdition(int upgradeEditionId);
    }
}
