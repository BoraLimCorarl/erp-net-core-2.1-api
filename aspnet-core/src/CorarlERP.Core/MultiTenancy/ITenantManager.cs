using Abp.Domain.Services;
using CorarlERP.PropertyFormulas.Dto;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.MultiTenancy
{
  public  interface ITenantManager : IDomainService
    {
        Task<IdentityResult> UpdateAsync(Tenant @entity);
        Task<Tenant> GetAsync(int id, bool tracking = false);
        Task AutoCreateItemCode(CreateOrUpdateDefaultAutoItemCodeInput input);
    }
}
