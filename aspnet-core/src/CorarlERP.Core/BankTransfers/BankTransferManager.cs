using Abp.Domain.Repositories;
using Abp.UI;
using CorarlERP.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CorarlERP.BankTransfers
{
    public class BankTransferManager : CorarlERPDomainServiceBase, IBankTransferManager
    {
        private readonly IRepository<BankTransfer, Guid> _bankTransferOrderRepository;
        private readonly IRepository<Tenant, int> _tenantRepository;
        public BankTransferManager(
            IRepository<BankTransfer, Guid> bankTransferOrderRepository,
            IRepository<Tenant, int> tenantRepository,
            IRepository<AccountCycles.AccountCycle,long> accountCycleRepository) : base(accountCycleRepository)
        {
            _bankTransferOrderRepository = bankTransferOrderRepository;
            _tenantRepository = tenantRepository;
        }
        public async Task<IdentityResult> CreateAsync(BankTransfer entity)
        {
            await CheckDuplicateTransferOrder(entity);
            await CheckClosePeriod(entity.BankTransferDate);
            await _bankTransferOrderRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }
          
        public async Task<BankTransfer> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _bankTransferOrderRepository.GetAll()
                .Include(u => u.BankTransferToAccount)
                .Include(u => u.BankTransferFromAccount)
                .Include(u => u.TransferFromClass)
                .Include(u => u.TransferToClass)
                .Include(u => u.FromLocation)
                .Include(u => u.ToLocation)
                :
                _bankTransferOrderRepository.GetAll()
                .Include(u => u.BankTransferFromAccount)
                .Include(u => u.ToLocation)
                .Include(u => u.TransferFromClass)
                .Include(u => u.TransferToClass)
                .Include(u => u.BankTransferToAccount)
                .Include(u => u.FromLocation)
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(BankTransfer entity)
        {
            await CheckClosePeriod(entity.BankTransferDate);
            await _bankTransferOrderRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(BankTransfer entity)
        {

            await CheckDuplicateTransferOrder(entity);
            await CheckClosePeriod(entity.BankTransferDate);
            await _bankTransferOrderRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
        
        private async Task CheckDuplicateTransferOrder(BankTransfer @entity)
        {
            var @old = await _bankTransferOrderRepository.GetAll().AsNoTracking()
                           .Where(u => u.BankTransferNo.ToLower() == entity.BankTransferNo.ToLower() && u.Id != entity.Id)
                           .FirstOrDefaultAsync();

            if (old != null && old.BankTransferNo.ToLower() == entity.BankTransferNo.ToLower())
            {
                throw new UserFriendlyException(L("DuplicateTransferOrderNumber", entity.BankTransferNo));
            }
        }
    }
}
