using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Vendors
{
  public  interface IContactPersonManager
    {
        Task<ContactPreson> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(ContactPreson @entity);
        Task<IdentityResult> RemoveAsync(ContactPreson @entity);
        Task<IdentityResult> UpdateAsync(ContactPreson @entity);
    }
}
