using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;

namespace CorarlERP.ItemPrices
{
    public interface IItemPriceItemManager : IDomainService
    {
        Task<ItemPriceItem> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(ItemPriceItem @entity);
        Task<IdentityResult> RemoveAsync(ItemPriceItem @entity);
        Task<IdentityResult> UpdateAsync(ItemPriceItem @entity);
        //Task<IdentityResult> DisableAsync(ItemPrice @entity);
        //Task<IdentityResult> EnableAsync(ItemPrice @entity);
    }

}
