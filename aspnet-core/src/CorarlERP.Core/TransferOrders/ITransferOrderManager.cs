using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.TransferOrders
{
    public interface ITransferOrderManager : IDomainService
    {
        Task<TransferOrder> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(TransferOrder @entity, bool checkDupliateReference);
        Task<IdentityResult> RemoveAsync(TransferOrder @entity);
        Task<IdentityResult> UpdateAsync(TransferOrder @entity);
        //Task<IdentityResult> DisableAsync(TransferOrder @entity);
        //Task<IdentityResult> EnableAsync(TransferOrder @entity);
    }
}
