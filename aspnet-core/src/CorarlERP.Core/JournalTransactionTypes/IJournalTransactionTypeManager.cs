using Abp.Domain.Services;
using CorarlERP.InventoryTransactionTypes;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.JournalTransactionTypes
{
   public interface IJournalTransactionTypeManager : IDomainService
    {
        Task<JournalTransactionType> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(JournalTransactionType @entity);
        Task<IdentityResult> RemoveAsync(JournalTransactionType @entity);
        Task<IdentityResult> UpdateAsync(JournalTransactionType @entity);
        Task<IdentityResult> DisableAsync(JournalTransactionType @entity);
        Task<IdentityResult> EnableAsync(JournalTransactionType @entity);
        Task<Guid> GetJournalTransactionTypeId(InventoryTransactionType inventoryTransactionType);
    }
}
