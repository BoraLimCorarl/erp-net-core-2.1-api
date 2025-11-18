using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Bills
{
    public class BillManager : CorarlERPDomainServiceBase, IBillManager
    {
        private readonly IRepository<Bill, Guid> _billRepository;

        public BillManager(IRepository<Bill, Guid> billRepository)
        {
            _billRepository = billRepository;
        }


        public async Task<IdentityResult> CreateAsync(Bill entity)
        {
            await _billRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<Bill> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _billRepository.GetAll()
               // .Include(u => u.Location)
                .Include(u => u.Vendor)
                :
                _billRepository.GetAll()
               // .Include(u => u.Location)
                .Include(u => u.Vendor)
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(Bill entity)
        {
            await _billRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(Bill entity)
        {
            await _billRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
