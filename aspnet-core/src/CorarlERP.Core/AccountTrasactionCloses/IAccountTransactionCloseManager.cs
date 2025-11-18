using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.AccountTrasactionCloses
{
   public interface IAccountTransactionCloseManager : IDomainService
    {
        Task<IdentityResult> CreateAsync(AccountTransactionClose @entity);
        Task<IdentityResult> RemoveAsync(AccountTransactionClose @entity);
    }

}
