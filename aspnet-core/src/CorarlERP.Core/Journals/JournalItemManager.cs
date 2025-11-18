using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.Journals
{
    public class JournalItemManager : CorarlERPDomainServiceBase, IJournalItemManager
    {
        private readonly IRepository<JournalItem, Guid> _generalJournalDetailRepository;

        public JournalItemManager(IRepository<JournalItem, Guid> generalJournalDetailRepository)
        {
            _generalJournalDetailRepository = generalJournalDetailRepository;
        }
        public async Task<IdentityResult> CreateAsync(JournalItem entity)
        {
            await _generalJournalDetailRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<JournalItem> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _generalJournalDetailRepository.GetAll()
                .Include(u => u.Account) :
                _generalJournalDetailRepository.GetAll()
                .Include(u => u.Account).AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(JournalItem entity)
        {
            await _generalJournalDetailRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(JournalItem entity)
        {
            await _generalJournalDetailRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
