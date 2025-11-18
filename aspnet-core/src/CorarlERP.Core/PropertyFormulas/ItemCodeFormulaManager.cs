using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.PropertyFormulas
{
   public class ItemCodeFormulaManager : CorarlERPDomainServiceBase, IItemCodeFormulaManager
    {
        private readonly ICorarlRepository<ItemCodeFormula, long> _itemCodeFormulaRepository;

        public ItemCodeFormulaManager(ICorarlRepository<ItemCodeFormula, long> LotRepository)
        {
            _itemCodeFormulaRepository = LotRepository;
        }
     
        public async  Task<IdentityResult> CreateAsync(ItemCodeFormula entity)
        {          
            await _itemCodeFormulaRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async  Task<IdentityResult> DisableAsync(ItemCodeFormula entity)
        {
            @entity.Enable(false);
            await _itemCodeFormulaRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async  Task<IdentityResult> EnableAsync(ItemCodeFormula entity)
        {
            @entity.Enable(true);
            await _itemCodeFormulaRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async  Task<ItemCodeFormula> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _itemCodeFormulaRepository.GetAll() :
                _itemCodeFormulaRepository.GetAll().AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async  Task<IdentityResult> RemoveAsync(ItemCodeFormula entity)
        {
            await _itemCodeFormulaRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async  Task<IdentityResult> UpdateAsync(ItemCodeFormula entity)
        {         
            await _itemCodeFormulaRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async  Task<IdentityResult> BulkCreateAsync(List<ItemCodeFormula>entities)
        {           
            await _itemCodeFormulaRepository.BulkInsertAsync(entities);
            return IdentityResult.Success;
        }

    }
}
