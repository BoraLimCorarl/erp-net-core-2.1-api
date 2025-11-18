using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.PayBills
{
    public interface IPayBillExpenseManager
    {
        Task<PayBillExpense> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(PayBillExpense @entity);
        Task<IdentityResult> RemoveAsync(PayBillExpense @entity);
        Task<IdentityResult> UpdateAsync(PayBillExpense @entity);
    }
}
