using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.InventoryCalculationJobSchedules
{
    public interface IInventoryCalculationJobScheduleManager : IDomainService
    {
        Task CreateAsync(long userId, DateTime date);
        Task UpdateAsync(long userId, Guid id, DateTime date);
        Task ExecuteAsync(Guid scheduleId);
        Task DeleteAsync(Guid scheduleId);
        Task EnableAsync(Guid scheduleId);
        Task DisableAsync(Guid scheduleId);
        Task TryCalculation(int tenantId, Guid scheduleId);
        Task ScheduleCalculationByTenant(Guid scheduleId);

    }
}
