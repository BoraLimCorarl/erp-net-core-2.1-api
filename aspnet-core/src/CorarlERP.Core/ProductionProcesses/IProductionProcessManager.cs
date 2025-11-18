using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ProductionProcesses
{
    public interface IProductionProcessManager : IDomainService
    {
        Task<ProductionProcess> GetAsync(long id, bool tracking = false);
        Task<IdentityResult> CreateAsync(ProductionProcess @entity);
        Task<IdentityResult> RemoveAsync(ProductionProcess @entity);
        Task<IdentityResult> UpdateAsync(ProductionProcess @entity);
        Task<IdentityResult> DisableAsync(ProductionProcess @entity);
        Task<IdentityResult> EnableAsync(ProductionProcess @entity);
    }

}
