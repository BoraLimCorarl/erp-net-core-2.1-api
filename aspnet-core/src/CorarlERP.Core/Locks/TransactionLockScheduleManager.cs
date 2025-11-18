using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.Locks
{
    public class TransactionLockScheduleManager : CorarlERPDomainServiceBase, ITransactionLockScheduleManager
    {
        private readonly IRepository<TransactionLockSchedule, long> _transactionLockScheduleRepository;
        public TransactionLockScheduleManager(IRepository<TransactionLockSchedule, long> repository)
        {
            _transactionLockScheduleRepository = repository ;
        }

        public async Task<IdentityResult> CreateAsync(TransactionLockSchedule entity)
        {
            await _transactionLockScheduleRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<TransactionLockSchedule> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _transactionLockScheduleRepository.GetAll() :
               _transactionLockScheduleRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> UpdateAsync(TransactionLockSchedule entity)
        {
            await _transactionLockScheduleRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> RemoveAsync(TransactionLockSchedule entity)
        {
            await _transactionLockScheduleRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> EnableAsync(TransactionLockSchedule entity)
        {
            entity.Enable(true);
            await _transactionLockScheduleRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DisableAsync(TransactionLockSchedule entity)
        {
            entity.Enable(false);
            await _transactionLockScheduleRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
