using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Settings
{
   public interface IBillInvoiceSettingManager : IDomainService
    {
        Task<BillInvoiceSetting> GetAsync(long id);
        Task<IdentityResult> CreateAsync(BillInvoiceSetting @entity);
        Task<IdentityResult> UpdateAsync(BillInvoiceSetting @entity);
    }
}
