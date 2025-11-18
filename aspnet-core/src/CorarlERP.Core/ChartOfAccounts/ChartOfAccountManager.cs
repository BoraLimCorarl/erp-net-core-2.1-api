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
    public class ChartOfAccountManager : CorarlERPDomainServiceBase, IChartOfAccountManager
    {
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;

        public ChartOfAccountManager(IRepository<ChartOfAccount, Guid> chartOfAccountRepository)
        {
            _chartOfAccountRepository = chartOfAccountRepository;
        }

        public async Task<ChartOfAccount> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _chartOfAccountRepository.GetAll() : _chartOfAccountRepository.GetAll().AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }


        public async virtual Task<IdentityResult> CreateAsync(ChartOfAccount @entity)
        {
            await CheckDuplicateAccount(entity);

            await _chartOfAccountRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> RemoveAsync(ChartOfAccount @entity)
        {
            await _chartOfAccountRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(ChartOfAccount @entity)
        {
            await CheckDuplicateAccount(entity);

            await _chartOfAccountRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> DisableAsync(ChartOfAccount @entity)
        {
            @entity.Enable(false);
            await _chartOfAccountRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> EnableAsync(ChartOfAccount @entity)
        {
            @entity.Enable(true);
            await _chartOfAccountRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        private async Task CheckDuplicateAccount(ChartOfAccount @entity)
        {
            var @old = await _chartOfAccountRepository.GetAll().AsNoTracking()
                           .Where(u => u.IsActive &&
                                       u.AccountCode.ToLower() == entity.AccountCode.ToLower() &&
                                       u.Id != entity.Id)
                           .FirstOrDefaultAsync();

            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicateAccountCode", entity.AccountCode));
            }
        }
    }

   
}
