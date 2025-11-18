using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.InventoryCostCloses
{
   public interface IInventoryCostCloseItemManager : IDomainService
    {
        Task<IdentityResult> CreateAsync(InventoryCostCloseItem @entity);
        Task<IdentityResult> RemoveAsync(InventoryCostCloseItem @entity);
    }
}
