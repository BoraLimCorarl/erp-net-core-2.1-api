using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;

namespace CorarlERP.PaymentMethods
{
    public interface IPaymentMethodManager : IDomainService
    {
        Task<PaymentMethod> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(PaymentMethod @entity);       
        Task<IdentityResult> RemoveAsync(PaymentMethod @entity);
        Task<IdentityResult> UpdateAsync(PaymentMethod @entity);
        Task<IdentityResult> DisableAsync(PaymentMethod @entity);
        Task<IdentityResult> EnableAsync(PaymentMethod @entity);

    }
   
}
