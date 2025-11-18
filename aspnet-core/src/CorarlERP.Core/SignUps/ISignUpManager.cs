using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.SignUps
{
   public interface  ISignUpManager : IDomainService
    {
        Task<SignUp> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(SignUp @entity);
        Task<IdentityResult> RemoveAsync(SignUp @entity);
        Task<IdentityResult> UpdateAsync(SignUp @entity);
        Task<IdentityResult> DisableAsync(SignUp @entity);
        Task<IdentityResult> EnableAsync(SignUp @entity);
        Task CheckDuplicateCompanySignUp(string companyName, string email);
    }
}
