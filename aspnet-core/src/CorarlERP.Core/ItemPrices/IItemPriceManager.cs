using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;

namespace CorarlERP.ItemPrices
{
    public interface IItemPriceManager  : IDomainService
    {
        Task<ItemPrice> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(ItemPrice @entity);
        Task<IdentityResult> RemoveAsync(ItemPrice @entity);
        Task<IdentityResult> UpdateAsync(ItemPrice @entity);
    }
}
