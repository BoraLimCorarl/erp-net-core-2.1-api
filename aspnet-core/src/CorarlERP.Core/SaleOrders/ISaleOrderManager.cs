using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.SaleOrders
{
    public interface ISaleOrderManager : IDomainService
    {
        Task<SaleOrder> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(SaleOrder @entity, bool checkDupliateReference);
        Task<IdentityResult> RemoveAsync(SaleOrder @entity);
        Task<IdentityResult> UpdateAsync(SaleOrder @entity, bool validateReference = true);
        Task<IdentityResult> DisableAsync(SaleOrder @entity);
        Task<IdentityResult> EnableAsync(SaleOrder @entity);
    }
}
