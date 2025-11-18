using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.InvoiceTemplates
{
    public interface IInvoiceTemplateManager : IDomainService
    {
        Task<InvoiceTemplate> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult>CreateAsync(InvoiceTemplate @entity);
        Task<IdentityResult>UpdateAsync(InvoiceTemplate @entity);
        Task<IdentityResult>DeleteAsync(InvoiceTemplate @entity);
        Task<IdentityResult>EnableAsync(InvoiceTemplate @entity);
        Task<IdentityResult>DisableAsync(InvoiceTemplate @entity);
    }
}
