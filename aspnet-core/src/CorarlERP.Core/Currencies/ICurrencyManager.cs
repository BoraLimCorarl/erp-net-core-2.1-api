using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Currencies

{
   public interface ICurrencyManager : IDomainService
    {
        Task<Currency> GetAsync(long id, bool tracking = false);
        Task<IdentityResult> CreateAsync(Currency @entity, bool checkDuplicate = true);
        Task<IdentityResult> UpdateAsync(Currency entity, bool checkDuplicate = true);
    }
}
