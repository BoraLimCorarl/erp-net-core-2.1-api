using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;

namespace CorarlERP.PurchasePrices
{
    public interface IPurchasePriceItemManager : IDomainService
    {
        Task<PurchasePriceItem> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(PurchasePriceItem @entity);
        Task<IdentityResult> RemoveAsync(PurchasePriceItem @entity);
        Task<IdentityResult> UpdateAsync(PurchasePriceItem @entity);
    }

}
