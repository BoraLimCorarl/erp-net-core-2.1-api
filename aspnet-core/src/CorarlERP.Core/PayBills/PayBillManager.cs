using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.PayBills
{
    public class PayBillManager : CorarlERPDomainServiceBase, IPayBillManager
    {
        private readonly IRepository<PayBill, Guid> _payBillRepository;

        public PayBillManager(IRepository<PayBill, Guid> payBillRepository)
        {
            _payBillRepository = payBillRepository;
        }

        public async Task<IdentityResult> CreateAsync(PayBill entity)
        {
            await _payBillRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<PayBill> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _payBillRepository.GetAll() : _payBillRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(PayBill entity)
        {
            await _payBillRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(PayBill entity)
        {
            await _payBillRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
