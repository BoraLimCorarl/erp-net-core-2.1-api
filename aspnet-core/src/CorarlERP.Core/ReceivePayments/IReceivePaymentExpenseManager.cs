using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ReceivePayments
{
    public interface IReceivePaymentExpenseManager
    {
        Task<ReceivePaymentExpense> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(ReceivePaymentExpense @entity);
        Task<IdentityResult> RemoveAsync(ReceivePaymentExpense @entity);
        Task<IdentityResult> UpdateAsync(ReceivePaymentExpense @entity);
    }
}
