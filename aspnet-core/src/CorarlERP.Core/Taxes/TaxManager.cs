using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Taxes
{
    public class TaxManager: CorarlERPDomainServiceBase, ITaxManager
    {
        private readonly IRepository<Tax, long> _taxRepository;

        public TaxManager(IRepository<Tax, long> taxRepository)
        {
            _taxRepository = taxRepository;
        }

        public async Task<Tax> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _taxRepository.GetAll() : _taxRepository.GetAll().AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }


        public async virtual Task<IdentityResult> CreateAsync(Tax @entity)
        {

            await CheckDuplicateTax(@entity);

            await _taxRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> RemoveAsync(Tax @entity)
        {
            await _taxRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(Tax @entity)
        {

            await CheckDuplicateTax(@entity);

            await _taxRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> DisableAsync(Tax @entity)
        {
            @entity.Enable(false);
            await _taxRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> EnableAsync(Tax @entity)
        {
            @entity.Enable(true);
            await _taxRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }


        private async Task CheckDuplicateTax(Tax @entity)
        {
            var @old = await _taxRepository.GetAll().AsNoTracking()
                           .Where(u => u.IsActive && 
                                       u.TaxName.ToLower() == entity.TaxName.ToLower() &&
                                       u.Id != entity.Id)
                           .FirstOrDefaultAsync();

            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicateTaxName", entity.TaxName));
            }
        }
    }

   
}
