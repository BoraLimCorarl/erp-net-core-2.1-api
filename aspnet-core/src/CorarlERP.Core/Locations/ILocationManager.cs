using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Locations
{  
    public interface ILocationManager : IDomainService
    {
        Task<Location> GetAsync(long id, bool tracking = false);
        Task<IdentityResult> CreateAsync(Location @entity);
        Task<IdentityResult> RemoveAsync(Location @entity);
        Task<IdentityResult> UpdateAsync(Location @entity);
        Task<IdentityResult> DisableAsync(Location @entity);
        Task<IdentityResult> EnableAsync(Location @entity);
    }
}
