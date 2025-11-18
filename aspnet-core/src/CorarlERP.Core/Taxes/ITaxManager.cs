using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Taxes
{
    public interface ITaxManager: IDomainService
    {
        Task<Tax> GetAsync(long id, bool tracking = false);
        Task<IdentityResult> CreateAsync(Tax @entity);
        Task<IdentityResult> RemoveAsync(Tax @entity);
        Task<IdentityResult> UpdateAsync(Tax @entity);
        Task<IdentityResult> DisableAsync(Tax @entity);
        Task<IdentityResult> EnableAsync(Tax @entity);
    }
}
