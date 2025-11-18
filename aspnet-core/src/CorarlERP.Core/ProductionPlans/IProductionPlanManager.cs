using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ProductionPlans
{
   public interface IProductionPlanManager : IDomainService
    {
        /// <summary>
        /// Recommended to be called after CurrentUnitOfWork.SaveChangesAsync()
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        Task CalculateByIdAsync(long userId, List<Guid> ids);

        /// <summary>
        /// Recommended to be called after CurrentUnitOfWork.SaveChangesAsync()
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        Task CalculateAsync(long userId, DateTime fromDate, DateTime toDate);

        Task<Dictionary<Guid, decimal>> GetItemNetWeight(List<Guid> itemIds);
        Task<Dictionary<Guid, long>> GetItemStandardGroups(List<Guid> itemIds);
    }
}
