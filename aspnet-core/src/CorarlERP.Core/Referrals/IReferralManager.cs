using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Referrals
{
    public interface IReferralManager : IDomainService
    {
        Task<Referral> GetAsync(long id, bool tracking = false);
        Task<IdentityResult> CreateAsync(Referral @entity);
        Task<IdentityResult> RemoveAsync(Referral @entity);
        Task<IdentityResult> UpdateAsync(Referral @entity);
        Task<IdentityResult> DisableAsync(Referral @entity);
        Task<IdentityResult> EnableAsync(Referral @entity);
    }
}
