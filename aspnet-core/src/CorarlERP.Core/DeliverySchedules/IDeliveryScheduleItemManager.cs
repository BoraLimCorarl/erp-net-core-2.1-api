using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.DeliverySchedules
{
    public interface IDeliveryScheduleItemManager : IDomainService
    {
    
        Task<IdentityResult> CreateAsync(DeliveryScheduleItem @entity);
        Task<IdentityResult> RemoveAsync(DeliveryScheduleItem @entity);
        Task<IdentityResult> UpdateAsync(DeliveryScheduleItem @entity);
    }
}
