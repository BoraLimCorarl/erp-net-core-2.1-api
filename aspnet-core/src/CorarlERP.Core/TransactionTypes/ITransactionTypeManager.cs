using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace CorarlERP.TransactionTypes
{
    public interface ITransactionTypeManager : IDomainService
    {
        Task<TransactionType> GetAsync(long id, bool tracking = false);
        Task<IdentityResult> CreateAsync(TransactionType @entity);
        Task<IdentityResult> RemoveAsync(TransactionType @entity);
        Task<IdentityResult> UpdateAsync(TransactionType @entity);
        Task<IdentityResult> DisableAsync(TransactionType @entity);
        Task<IdentityResult> EnableAsync(TransactionType @entity);
    }
}
