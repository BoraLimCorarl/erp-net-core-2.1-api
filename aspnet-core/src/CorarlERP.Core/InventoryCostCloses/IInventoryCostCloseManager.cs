using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.InventoryCostCloses
{
   public interface IInventoryCostCloseManager : IDomainService
    {
        Task<IdentityResult> CreateAsync(InventoryCostClose @entity);
        Task<IdentityResult> RemoveAsync(InventoryCostClose @entity);
    }
}
