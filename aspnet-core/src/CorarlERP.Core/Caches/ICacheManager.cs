using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Caches
{
    public interface ICacheManager : IDomainService
    {
        Task<Cache> GetAsync(string keyName, bool tracking = false);
        Task<IdentityResult> CreateAsync(Cache @entity);
        Task<IdentityResult> UpdateAsync(Cache @entity);
    }
}
