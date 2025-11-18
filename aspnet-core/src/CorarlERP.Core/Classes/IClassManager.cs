using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Classes
{
   public interface IClassManager : IDomainService
    {
        Task<Class> GetAsync(long id, bool tracking = false);
        Task<IdentityResult> CreateAsync(Class @entity);
        Task<IdentityResult> RemoveAsync(Class @entity);
        Task<IdentityResult> UpdateAsync(Class @entity);
        Task<IdentityResult> DisableAsync(Class @entity);
        Task<IdentityResult> EnableAsync(Class @entity);
    }
}
