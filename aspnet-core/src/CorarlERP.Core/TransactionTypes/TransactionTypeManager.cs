using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CorarlERP.TransactionTypes
{
    public class TransactionTypeManager : CorarlERPDomainServiceBase, ITransactionTypeManager
    {
        private readonly IRepository<TransactionType, long> _transactionTypeRepository;

        public TransactionTypeManager(IRepository<TransactionType, long> transationTypeRepository)
        {
            _transactionTypeRepository = transationTypeRepository;
        }
        private async Task CheckDuplicateClass(TransactionType @entity)
        {
            var @old = await _transactionTypeRepository.GetAll().AsNoTracking()
                           .Where(u => u.IsActive &&
                                       u.TransactionTypeName.ToLower() == entity.TransactionTypeName.ToLower() &&
                                       u.Id != entity.Id)
                           .FirstOrDefaultAsync();

            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicateTransactiontypeName", entity.TransactionTypeName));
            }
        }
        public async virtual Task<IdentityResult> CreateAsync(TransactionType entity)
        {
            await CheckDuplicateClass(entity);

            await _transactionTypeRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> DisableAsync(TransactionType entity)
        {
            @entity.Enable(false);
            await _transactionTypeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> EnableAsync(TransactionType entity)
        {
            @entity.Enable(true);
            await _transactionTypeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<TransactionType> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _transactionTypeRepository.GetAll() : _transactionTypeRepository.GetAll().AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(TransactionType entity)
        {
            await _transactionTypeRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(TransactionType entity)
        {
            await CheckDuplicateClass(entity);

            await _transactionTypeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
