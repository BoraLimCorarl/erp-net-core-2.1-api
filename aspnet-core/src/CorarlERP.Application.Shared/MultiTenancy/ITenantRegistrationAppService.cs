using System.Threading.Tasks;
using Abp.Application.Services;
using CorarlERP.Editions.Dto;
using CorarlERP.MultiTenancy.Dto;

namespace CorarlERP.MultiTenancy
{
    public interface ITenantRegistrationAppService: IApplicationService
    {
        Task<RegisterTenantOutput> RegisterTenant(RegisterTenantInput input);

        Task<EditionsSelectOutput> GetEditionsForSelect();

        Task<EditionSelectDto> GetEdition(int editionId);
    }
}