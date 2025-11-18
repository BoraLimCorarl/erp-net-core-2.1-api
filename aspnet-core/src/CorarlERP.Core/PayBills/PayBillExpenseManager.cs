using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.PayBills
{
    class PayBillExpenseManager : CorarlERPDomainServiceBase, IPayBillExpenseManager
    {
        private readonly IRepository<PayBillExpense, Guid> _PayBillExpenseRepository;

        public PayBillExpenseManager(IRepository<PayBillExpense, Guid> PayBillExpenseRepository)
        {
            _PayBillExpenseRepository = PayBillExpenseRepository;
        }

        public async Task<IdentityResult> CreateAsync(PayBillExpense entity)
        {
            await _PayBillExpenseRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<PayBillExpense> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _PayBillExpenseRepository.GetAll()
                .Include(u => u.Account)
                :
                _PayBillExpenseRepository.GetAll()
                .Include(u => u.Account)
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(PayBillExpense entity)
        {
            await _PayBillExpenseRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(PayBillExpense entity)
        {
            await _PayBillExpenseRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
