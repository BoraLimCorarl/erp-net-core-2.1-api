using Abp.Domain.Services;
using CorarlERP.MultiCurrencies;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.MultiCurrencies
{
  public interface IMultiCurrencyManager : IDomainService
    {
        Task<MultiCurrency> GetAsync(long id, bool tracking = false);
        Task<IdentityResult> CreateAsync(MultiCurrency @entity, bool checkDuplicate = true);
        Task<IdentityResult> UpdateAsync(MultiCurrency entity, bool checkDuplicate = true);
        Task<IdentityResult> RemoveAsync(MultiCurrency entity);
    }
}
