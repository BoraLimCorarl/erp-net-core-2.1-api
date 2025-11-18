using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.SignUps
{
   public  class SignUpManager : CorarlERPDomainServiceBase, ISignUpManager
    {
        private readonly IRepository<SignUp, Guid> _signUpRepository;

        public SignUpManager(IRepository<SignUp, Guid> signUpRepository)
        {
            _signUpRepository = signUpRepository;
        }
        public async Task<SignUp> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _signUpRepository.GetAll() : _signUpRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }


        public async  Task<IdentityResult> CreateAsync(SignUp @entity)
        {

            await CheckDuplicateSignUp(@entity);
            await _signUpRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async  Task<IdentityResult> RemoveAsync(SignUp @entity)
        {
            await _signUpRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async  Task<IdentityResult> UpdateAsync(SignUp @entity)
        {

            await CheckDuplicateCompanySignUp(@entity.CompanyOrStoreName,entity.Email,entity.Id);
            await _signUpRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        private async Task CheckDuplicateSignUp(SignUp @entity)
        {
            var validateCode = await _signUpRepository.GetAll().AsNoTracking()
                                       .Where(u => u.SignUpCode.ToLower() == entity.SignUpCode.ToLower() && u.Id != entity.Id)
                                       .AnyAsync();

            if (validateCode) throw new UserFriendlyException(L("DuplicateSignUpCode", entity.SignUpCode));

            await CheckDuplicateCompanySignUp(entity.CompanyOrStoreName, entity.Email);
        }

        private async Task CheckDuplicateCompanySignUp(string companyName, string email, Guid? id)
        {
            var validateEmail = await _signUpRepository.GetAll().AsNoTracking()
                                     .Where(u => u.Email.ToLower() == email.ToLower() && u.CompanyOrStoreName.ToLower() == companyName.ToLower() && u.Id != id)
                                     .AnyAsync();

            if (validateEmail) throw new UserFriendlyException(L("DuplicateCustomerSignUp", $" {companyName + ", " + email}"));
        }

        public async Task CheckDuplicateCompanySignUp(string companyName, string email)
        {
            await CheckDuplicateCompanySignUp(companyName, email, null);
        }


        public async  Task<IdentityResult> DisableAsync(SignUp @entity)
        {
            @entity.UpdateStatus(false);
            await _signUpRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async  Task<IdentityResult> EnableAsync(SignUp @entity)
        {
            @entity.UpdateStatus(true);
            await _signUpRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
