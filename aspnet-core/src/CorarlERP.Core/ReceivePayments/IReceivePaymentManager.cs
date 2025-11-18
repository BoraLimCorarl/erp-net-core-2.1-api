using CorarlERP.PayBills;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ReceivePayments
{
   public interface IReceivePaymentManager
    {
        Task<ReceivePayment> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(ReceivePayment @entity);
        Task<IdentityResult> RemoveAsync(ReceivePayment @entity);
        Task<IdentityResult> UpdateAsync(ReceivePayment @entity);
    }
}
