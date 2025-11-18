using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Productions
{
   public interface IProductionManager : IDomainService
    {
        Task<Production> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(Production @entity, bool checkDupliateReference);
        Task<IdentityResult> RemoveAsync(Production @entity);
        Task<IdentityResult> UpdateAsync(Production @entity);
        Task CalculateAsync(long userId, DateTime fromDate, DateTime toDate);
    }
}
