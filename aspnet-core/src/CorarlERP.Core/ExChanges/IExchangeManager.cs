using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;

namespace CorarlERP.ExChanges
{
    public interface IExchangeManager : IDomainService
    {
        Task<Exchange> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(Exchange @entity);
        Task<IdentityResult> RemoveAsync(Exchange @entity);
        Task<IdentityResult> UpdateAsync(Exchange @entity);
    }
}
