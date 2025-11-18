using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ChartOfAccounts
{
    public class AccountTypeManager: CorarlERPDomainServiceBase, IAccountTypeManager
    {
        private readonly IRepository<AccountType, long> _accountTypeRepository;

        public AccountTypeManager(IRepository<AccountType, long> accountTypeRepository)
        {
            _accountTypeRepository = accountTypeRepository;
        }

        public async Task<AccountType> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _accountTypeRepository.GetAll() : _accountTypeRepository.GetAll().AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }


        public async virtual Task<IdentityResult> CreateAsync(AccountType @entity)
        {
            await CheckDuplicateAccountType(@entity);

            await _accountTypeRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> RemoveAsync(AccountType @entity)
        {
            await _accountTypeRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(AccountType @entity)
        {
            await CheckDuplicateAccountType(@entity);

            await _accountTypeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> DisableAsync(AccountType @entity)
        {
            @entity.Enable(false);
            await _accountTypeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> EnableAsync(AccountType @entity)
        {
            @entity.Enable(true);
            await _accountTypeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        private async Task CheckDuplicateAccountType(AccountType @entity)
        {
            var @old = await _accountTypeRepository.GetAll().AsNoTracking()
                           .Where(u => u.IsActive &&
                                       u.AccountTypeName.ToLower() == entity.AccountTypeName.ToLower() &&
                                       u.Id != entity.Id)
                           .FirstOrDefaultAsync();

            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicateAccountTypeName", entity.AccountTypeName));
            }
        }
    }

   
}
