using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CorarlERP.Withdraws
{
    public class WithdrawManager : CorarlERPDomainServiceBase, IWithdrawManager
    {
        private readonly IRepository<Withdraw, Guid> _withdrawRepository;

        public WithdrawManager(IRepository<Withdraw, Guid> withdrawRepository)
        {
            _withdrawRepository = withdrawRepository;
        }

        public async Task<IdentityResult> CreateAsync(Withdraw entity)
        {
            await _withdrawRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<Withdraw> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ?
                _withdrawRepository.GetAll()
                .Include(u => u.Vendor)             
                :
                _withdrawRepository.GetAll()
                .Include(u => u.Vendor)             
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(Withdraw entity)
        {
            await _withdrawRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(Withdraw entity, bool isCheck = true)
        {
            if (isCheck == true)
            {
                await ValidateBankTransfer(entity);
            }
            await _withdrawRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
        private async Task ValidateBankTransfer(Withdraw withdraw)
        {
            var @entity = await _withdrawRepository.GetAll().AsNoTracking()
                           .Where(u => u.Id == withdraw.Id)
                           .FirstOrDefaultAsync();

            if (@entity.BankTransferId != null)
            {
                throw new UserFriendlyException(L("BankTransferMessage"));
            }
        }

    }
}
