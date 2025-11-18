using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.DeliverySchedules
{
    public interface IDeliveryScheduleManager : IDomainService
    {
        Task<DeliverySchedule> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(DeliverySchedule @entity, bool checkDupliateReference);
        Task<IdentityResult> RemoveAsync(DeliverySchedule @entity);
        Task<IdentityResult> UpdateAsync(DeliverySchedule @entity, bool validateReference = true);
    }
}
