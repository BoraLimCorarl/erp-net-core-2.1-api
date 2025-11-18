using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.InventoryTransactionItems
{
   public interface IInventoryTransactionItemManager : IDomainService
    {
        Task<IdentityResult> CreateAsync(InventoryTransactionItem @entity);
        Task<IdentityResult> UpdateAsync(InventoryTransactionItem @entity);
        Task<IdentityResult> RemoveAsync(InventoryTransactionItem @entity);
        Task<InventoryTransactionItem> GetAsync(Guid id, bool tracking = false);
    }
}
