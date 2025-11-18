using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.PurchaseOrders
{
   public interface IPurchaseOrderManager : IDomainService
    {
        Task<PurchaseOrder> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(PurchaseOrder @entity, bool checkDupliateReference);
        Task<IdentityResult> RemoveAsync(PurchaseOrder @entity);
        Task<IdentityResult> UpdateAsync(PurchaseOrder @entity, bool checkDupliateReference = true);
        Task<IdentityResult> DisableAsync(PurchaseOrder @entity);
        Task<IdentityResult> EnableAsync(PurchaseOrder @entity);
    }
}
