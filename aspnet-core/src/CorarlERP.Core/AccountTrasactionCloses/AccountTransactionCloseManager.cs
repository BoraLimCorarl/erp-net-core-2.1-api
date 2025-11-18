using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;

namespace CorarlERP.AccountTrasactionCloses
{
    public class AccountTransactionCloseManager : CorarlERPDomainServiceBase, IAccountTransactionCloseManager
    {
        private readonly IRepository<AccountTransactionClose, Guid> _accountTransactionCloseRepository;

        public AccountTransactionCloseManager(IRepository<AccountTransactionClose, Guid> accountTransactionCloseRepository)
        {
            _accountTransactionCloseRepository = accountTransactionCloseRepository;
        }

        public async Task<IdentityResult> CreateAsync(AccountTransactionClose entity)
        {
            await _accountTransactionCloseRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> RemoveAsync(AccountTransactionClose entity)
        {
            await _accountTransactionCloseRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
