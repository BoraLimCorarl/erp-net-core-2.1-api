using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;

namespace CorarlERP.ExChanges
{
    public interface IExchangeItemManager : IDomainService
    {
        Task<ExchangeItem> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(ExchangeItem @entity);
        Task<IdentityResult> RemoveAsync(ExchangeItem @entity);
        Task<IdentityResult> UpdateAsync(ExchangeItem @entity);
    }
}
