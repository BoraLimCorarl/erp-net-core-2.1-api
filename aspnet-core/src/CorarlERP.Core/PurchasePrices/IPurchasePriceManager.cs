using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;

namespace CorarlERP.PurchasePrices
{
    public interface IPurchasePriceManager  : IDomainService
    {
        Task<PurchasePrice> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(PurchasePrice @entity);
        Task<IdentityResult> RemoveAsync(PurchasePrice @entity);
        Task<IdentityResult> UpdateAsync(PurchasePrice @entity);
    }
}
