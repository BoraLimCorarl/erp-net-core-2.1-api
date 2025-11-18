using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.POSSettings
{
   public interface IPOSSettingManager : IDomainService
    {
        Task<IdentityResult> CreateAsync(POSSetting @entity);
        Task<IdentityResult> RemoveAsync(POSSetting @entity);
        Task<IdentityResult> UpdateAsync(POSSetting @entity);
       
    }
}
