using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.PhysicalCount
{
    public interface IPhysicalCountManager : IDomainService
    {
        Task<PhysicalCount> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(PhysicalCount @entity);
        Task<IdentityResult> RemoveAsync(PhysicalCount @entity);
        Task<IdentityResult> UpdateAsync(PhysicalCount @entity);
    }
}
