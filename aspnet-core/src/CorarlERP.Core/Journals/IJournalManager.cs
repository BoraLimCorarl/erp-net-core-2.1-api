using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Journals
{
  public interface IJournalManager : IDomainService
    {
        Task<Journal> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(Journal entity, bool noVlidate = false, bool checkDupliateReference = false);
        Task<IdentityResult> RemoveAsync(Journal @entity);
        Task<IdentityResult> UpdateAsync(Journal @entity, DocumentType? documentType, bool DuplicateDuplicate = true);
        Task<IdentityResult> DisableAsync(Journal @entity);
        Task<IdentityResult> EnableAsync(Journal @entity);
        void SetJournalType(JournalType journalType);
    }
}
