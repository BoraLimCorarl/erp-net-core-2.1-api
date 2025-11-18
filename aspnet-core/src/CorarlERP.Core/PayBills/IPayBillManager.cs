using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.PayBills
{
    public interface IPayBillManager
    {
        Task<PayBill> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(PayBill @entity);
        Task<IdentityResult> RemoveAsync(PayBill @entity);
        Task<IdentityResult> UpdateAsync(PayBill @entity);
    }
}
