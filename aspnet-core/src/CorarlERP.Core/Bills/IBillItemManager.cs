using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Bills
{
   public interface IBillItemManager
    {
        Task<BillItem> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(BillItem @entity);
        Task<IdentityResult> RemoveAsync(BillItem @entity);
        Task<IdentityResult> UpdateAsync(BillItem @entity);
    }
}
