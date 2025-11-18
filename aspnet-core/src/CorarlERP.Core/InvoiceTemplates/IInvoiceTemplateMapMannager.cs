using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.InvoiceTemplates
{
    public interface IInvoiceTemplateMapManager : IDomainService
    {
        Task<InvoiceTemplateMap> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult>CreateAsync(InvoiceTemplateMap @entity);
        Task<IdentityResult>UpdateAsync(InvoiceTemplateMap @entity);
        Task<IdentityResult>DeleteAsync(InvoiceTemplateMap @entity);
    }
}
