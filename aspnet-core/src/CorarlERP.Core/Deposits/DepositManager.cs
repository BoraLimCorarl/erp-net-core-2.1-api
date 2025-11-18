using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Deposits
{
    public class DepositManager : CorarlERPDomainServiceBase, IDepositManager
    {
        private readonly IRepository<Deposit, Guid> _depositRepository;

        public DepositManager(IRepository<Deposit, Guid> depositRepository)
        {
            _depositRepository = depositRepository;
        }


        public async Task<IdentityResult> CreateAsync(Deposit entity)
        {
            await _depositRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<Deposit> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _depositRepository.GetAll()
                .Include(u => u.Vendor)             
                :
                _depositRepository.GetAll()
                .Include(u => u.Vendor)               
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(Deposit entity)
        {
            await _depositRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(Deposit entity, bool isCheck = true)
        {
            if (isCheck == true)
            {
                await ValidateBankTransfer(entity);
            }
            
            await _depositRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        /***** Functionality for check if has bank transfer id *****/
        private async Task ValidateBankTransfer(Deposit deposit)
        {
            var @entity = await _depositRepository.GetAll().AsNoTracking()
                           .Where(u => u.Id == deposit.Id)
                           .FirstOrDefaultAsync();

            if (@entity.BankTransferId != null)
            {
                throw new UserFriendlyException(L("BankTransferMessage"));
            }
        }
    }
}
