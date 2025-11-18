using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.PermissionLocks.Dto;
using CorarlERP.TransactionLockSchedules.Dto;
using System.Threading.Tasks;

namespace CorarlERP.PermissionLocks
{
    public interface IPermissionLockAppService : IApplicationService
    {
        Task<PermissionLockDetailOutput> Create(CreatePermissionLockInput input);
        Task<PagedResultDto<PermissionLockDetailOutput>> GetList(GetPermissionLockListInput input);
        Task<PermissionLockDetailOutput> GetDetail(EntityDto<long> input);
        Task<PermissionLockDetailOutput> Update(UpdatePermissionLockInput input);
        Task Delete(EntityDto<long> input);
        Task Enable(EntityDto<long> input);
        Task Disable(EntityDto<long> input);
    }
}
