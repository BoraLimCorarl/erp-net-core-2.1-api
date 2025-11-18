using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;

namespace CorarlERP.PaymentMethods
{    

    public interface IPaymentMethodBaseManager : IDomainService
    {
        Task<PaymentMethodBase> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(PaymentMethodBase @entity);
        Task<IdentityResult> RemoveAsync(PaymentMethodBase @entity);
        Task<IdentityResult> UpdateAsync(PaymentMethodBase @entity);
        Task<IdentityResult> DisableAsync(PaymentMethodBase @entity);
        Task<IdentityResult> EnableAsync(PaymentMethodBase @entity);

    }
}
