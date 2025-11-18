using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ReceivePayments
{
    public class ReceivePaymentExpenseManager : CorarlERPDomainServiceBase, IReceivePaymentExpenseManager
    {
        private readonly IRepository<ReceivePaymentExpense, Guid> _ReceivePaymentExpenseRepository;

        public ReceivePaymentExpenseManager(IRepository<ReceivePaymentExpense, Guid> ReceivePaymentExpenseRepository)
        {
            _ReceivePaymentExpenseRepository = ReceivePaymentExpenseRepository;
        }

        public async Task<IdentityResult> CreateAsync(ReceivePaymentExpense entity)
        {
            await _ReceivePaymentExpenseRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<ReceivePaymentExpense> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _ReceivePaymentExpenseRepository.GetAll()
                .Include(u => u.Account)
                :
                _ReceivePaymentExpenseRepository.GetAll()
                .Include(u => u.Account)
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(ReceivePaymentExpense entity)
        {
            await _ReceivePaymentExpenseRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(ReceivePaymentExpense entity)
        {
            await _ReceivePaymentExpenseRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
