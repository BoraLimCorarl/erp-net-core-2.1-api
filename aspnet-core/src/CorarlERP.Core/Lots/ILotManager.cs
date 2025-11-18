using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace CorarlERP.Lots
{
    public interface ILotManager : IDomainService
    {
        Task<Lot> GetAsync(long id, bool tracking = false);
        Task<IdentityResult> CreateAsync(Lot @entity);
        Task<IdentityResult> RemoveAsync(Lot @entity);
        Task<IdentityResult> UpdateAsync(Lot @entity);
        Task<IdentityResult> DisableAsync(Lot @entity);
        Task<IdentityResult> EnableAsync(Lot @entity);
    }
}
