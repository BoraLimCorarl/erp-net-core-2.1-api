using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Invoices
{
    public interface IInvoiceManager
    {
        Task<Invoice> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(Invoice @entity);
        Task<IdentityResult> RemoveAsync(Invoice @entity);
        Task<IdentityResult> UpdateAsync(Invoice @entity);
    }
}
