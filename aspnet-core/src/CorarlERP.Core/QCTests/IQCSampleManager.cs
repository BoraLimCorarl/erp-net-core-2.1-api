using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace CorarlERP.QCTests
{
    public interface IQCSampleManager : IDomainService
    {
        Task<QCSample> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(QCSample @entity);
        Task<IdentityResult> RemoveAsync(QCSample @entity);
        Task<IdentityResult> UpdateAsync(QCSample @entity);
    }
}
