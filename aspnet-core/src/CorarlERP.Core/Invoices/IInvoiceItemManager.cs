using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Invoices
{
    public interface IInvoiceItemManager
    {
        Task<InvoiceItem> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(InvoiceItem @entity);
        Task<IdentityResult> RemoveAsync(InvoiceItem @entity);
        Task<IdentityResult> UpdateAsync(InvoiceItem @entity);
    }
}
