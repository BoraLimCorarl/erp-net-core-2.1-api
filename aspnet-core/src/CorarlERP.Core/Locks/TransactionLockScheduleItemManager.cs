using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.Locks
{
    public class TransactionLockScheduleItemManager : CorarlERPDomainServiceBase, ITransactionLockScheduleItemManager
    {
        private readonly IRepository<TransactionLockSheduleItem, long> _transactionLockScheduleItemRepository;
        public TransactionLockScheduleItemManager(IRepository<TransactionLockSheduleItem, long> repository)
        {
            _transactionLockScheduleItemRepository = repository ;
        }

        public async Task<IdentityResult> CreateAsync(TransactionLockSheduleItem entity)
        {
            await _transactionLockScheduleItemRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<TransactionLockSheduleItem> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _transactionLockScheduleItemRepository.GetAll().Include(s => s.TransactionLockShedule) :
               _transactionLockScheduleItemRepository.GetAll().Include(s => s.TransactionLockShedule).AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> UpdateAsync(TransactionLockSheduleItem entity)
        {
            await _transactionLockScheduleItemRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> RemoveAsync(TransactionLockSheduleItem entity)
        {
            await _transactionLockScheduleItemRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
