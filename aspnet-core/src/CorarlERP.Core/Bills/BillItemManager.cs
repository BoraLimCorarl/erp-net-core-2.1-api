using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.Bills
{
    public class BillItemManager : CorarlERPDomainServiceBase, IBillItemManager
    {
        private readonly IRepository<BillItem, Guid> _billItemRepository;

        public BillItemManager(IRepository<BillItem, Guid> billItemRepository)
        {
            _billItemRepository = billItemRepository;
        }

        public async Task<IdentityResult> CreateAsync(BillItem entity)
        {
            await _billItemRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<BillItem> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _billItemRepository.GetAll() :
                _billItemRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(BillItem entity)
        {
            await _billItemRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(BillItem entity)
        {
            await _billItemRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
