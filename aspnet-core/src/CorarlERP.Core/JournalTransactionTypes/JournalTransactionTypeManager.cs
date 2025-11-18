using Abp.Domain.Repositories;
using Abp.UI;
using CorarlERP.InventoryTransactionTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.JournalTransactionTypes
{
   public class JournalTransactionTypeManager : CorarlERPDomainServiceBase, IJournalTransactionTypeManager
    {

        private readonly IRepository<JournalTransactionType, Guid> _journalTransactionTypeRepository;

        public JournalTransactionTypeManager(IRepository<JournalTransactionType, Guid> customerRepository)
        {
            _journalTransactionTypeRepository = customerRepository;
        }
        private async Task CheckDuplicateJournalTrasactionType(JournalTransactionType @entity)
        {
            var @old = await _journalTransactionTypeRepository.GetAll().AsNoTracking()
                           .Where(u => u.Active &&
                                       u.Name.ToLower() == entity.Name.ToLower() &&
                                       u.Id != entity.Id
                                       )
                           .FirstOrDefaultAsync();

            if (old != null && old.Name.ToLower() == entity.Name.ToLower())
            {
                throw new UserFriendlyException(L("DuplicateName", entity.Name));
            }
        }
        public async Task<IdentityResult> CreateAsync(JournalTransactionType entity)
        {
            await CheckDuplicateJournalTrasactionType(entity);

            await _journalTransactionTypeRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DisableAsync(JournalTransactionType entity)
        {
            @entity.UpdateStatus(false);
            await _journalTransactionTypeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> EnableAsync(JournalTransactionType entity)
        {
            @entity.UpdateStatus(true);
            await _journalTransactionTypeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<JournalTransactionType> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _journalTransactionTypeRepository.GetAll()             
               :
               _journalTransactionTypeRepository.GetAll()              
               .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(JournalTransactionType entity)
        {
            await _journalTransactionTypeRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(JournalTransactionType entity)
        {
            await CheckDuplicateJournalTrasactionType(entity);
            await _journalTransactionTypeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
       public async Task<Guid> GetJournalTransactionTypeId(InventoryTransactionType inventoryTransactionType)
        {
            var id = await _journalTransactionTypeRepository.GetAll().AsNoTracking()
                           .Where(u => u.Active && u.InventoryTransactionType == inventoryTransactionType)
                           .Select(t=>t.Id)
                           .FirstOrDefaultAsync();
            if (id == null)
            {
                throw new UserFriendlyException(L("TransactionTypeIdNotFound"));
            }
            return id;
        }
    }
}
