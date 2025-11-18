using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.BankTransfers
{
   public interface IBankTransferManager
    {
        Task<BankTransfer> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(BankTransfer @entity);
        Task<IdentityResult> RemoveAsync(BankTransfer @entity);
        Task<IdentityResult> UpdateAsync(BankTransfer @entity);
    }
}
