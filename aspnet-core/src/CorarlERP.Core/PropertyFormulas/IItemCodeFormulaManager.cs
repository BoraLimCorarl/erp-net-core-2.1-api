using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CorarlERP.PropertyFormulas
{
    public interface IItemCodeFormulaManager : IDomainService
    {
        Task<ItemCodeFormula> GetAsync(long id, bool tracking = false);
        Task<IdentityResult> CreateAsync(ItemCodeFormula @entity);
        Task<IdentityResult> RemoveAsync(ItemCodeFormula @entity);
        Task<IdentityResult> UpdateAsync(ItemCodeFormula @entity);
        Task<IdentityResult> DisableAsync(ItemCodeFormula @entity);
        Task<IdentityResult> EnableAsync(ItemCodeFormula @entity);
        Task<IdentityResult> BulkCreateAsync(List<ItemCodeFormula> entities);
    }
}
