using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.AccountCycles
{
    public class AccountCycleManager : CorarlERPDomainServiceBase, IAccountCycleManager
    {
        private readonly IRepository<AccountCycle, long> _accountCycleRepository;

        public AccountCycleManager(IRepository<AccountCycle, long> accountCycleRepository)
        {
            _accountCycleRepository = accountCycleRepository;
        }
        public async virtual Task<IdentityResult> CreateAsync(AccountCycle entity)
        {
           // await CheckDuplicateAccountCycle(entity);
            await _accountCycleRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> DisableAsync(AccountCycle entity)
        {
            @entity.Enable(false);
            await _accountCycleRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> EnableAsync(AccountCycle entity)
        {
            @entity.Enable(true);
            await _accountCycleRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public virtual async Task<AccountCycle> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _accountCycleRepository.GetAll() : _accountCycleRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public virtual async Task<IdentityResult> RemoveAsync(AccountCycle entity)
        {
            await _accountCycleRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> UpdateAsync(AccountCycle entity)
        {
            await CheckDuplicateAccountCycle(@entity);

            await _accountCycleRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
        private async Task CheckDuplicateAccountCycle(AccountCycle @entity)
        {
            var @old = await _accountCycleRepository.GetAll().AsNoTracking()
                           .Where(u => u.StartDate == entity.StartDate &&
                                       u.Id != entity.Id)
                           .FirstOrDefaultAsync();

            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicateStartDate", entity.StartDate));
            }
        }
    }
}
