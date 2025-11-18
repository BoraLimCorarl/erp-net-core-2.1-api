using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ReceivePayments
{
   public interface IReceivePaymentDetailManager
    {
        Task<ReceivePaymentDetail> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(ReceivePaymentDetail @entity);
        Task<IdentityResult> RemoveAsync(ReceivePaymentDetail @entity);
        Task<IdentityResult> UpdateAsync(ReceivePaymentDetail @entity);
    }
}
