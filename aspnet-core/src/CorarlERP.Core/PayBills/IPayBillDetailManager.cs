using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.PayBills
{
    public interface IPayBillDetailManager
    {
        Task<PayBillDetail> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(PayBillDetail @entity);
        Task<IdentityResult> RemoveAsync(PayBillDetail @entity);
        Task<IdentityResult> UpdateAsync(PayBillDetail @entity);
    }
}
