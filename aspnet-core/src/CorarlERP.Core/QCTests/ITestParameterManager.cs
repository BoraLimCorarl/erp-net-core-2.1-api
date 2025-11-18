using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace CorarlERP.QCTests
{
    public interface ITestParameterManager : IDomainService
    {
        Task<TestParameter> GetAsync(long id, bool tracking = false);
        Task<IdentityResult> CreateAsync(TestParameter @entity);
        Task<IdentityResult> RemoveAsync(TestParameter @entity);
        Task<IdentityResult> UpdateAsync(TestParameter @entity);
        Task<IdentityResult> DisableAsync(TestParameter @entity);
        Task<IdentityResult> EnableAsync(TestParameter @entity);
    }
}
