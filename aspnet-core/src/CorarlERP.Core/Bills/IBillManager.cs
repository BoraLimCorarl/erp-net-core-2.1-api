using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Bills
{
   public interface IBillManager
    {
        Task<Bill> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(Bill @entity);
        Task<IdentityResult> RemoveAsync(Bill @entity);
        Task<IdentityResult> UpdateAsync(Bill @entity);      
    }
}
