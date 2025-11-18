using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace CorarlERP.QCTests
{
    public interface IQCTestTemplateManager : IDomainService
    {
        Task<QCTestTemplate> GetAsync(long id, bool tracking = false);
        Task<IdentityResult> CreateAsync(QCTestTemplate @entity);
        Task<IdentityResult> RemoveAsync(QCTestTemplate @entity);
        Task<IdentityResult> UpdateAsync(QCTestTemplate @entity);
        Task<IdentityResult> DisableAsync(QCTestTemplate @entity);
        Task<IdentityResult> EnableAsync(QCTestTemplate @entity);
    }
}
