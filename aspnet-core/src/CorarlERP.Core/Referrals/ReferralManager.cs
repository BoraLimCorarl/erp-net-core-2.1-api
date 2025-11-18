using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Referrals
{
   public class ReferralManager : CorarlERPDomainServiceBase, IReferralManager
    {
        private readonly IRepository<Referral, long> _referralRepository;
        public ReferralManager(IRepository<Referral, long> referralRepository)
        {
            _referralRepository = referralRepository;
        }
        public async Task<Referral> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _referralRepository.GetAll() : _referralRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<IdentityResult> CreateAsync(Referral @entity)
        {
            
            await _referralRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> RemoveAsync(Referral @entity)
        {
            await _referralRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(Referral @entity)
        {

            await _referralRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DisableAsync(Referral @entity)
        {
            @entity.UpdateStatus(false);
            await _referralRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> EnableAsync(Referral @entity)
        {
            @entity.UpdateStatus(true);
            await _referralRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
     

    }
}
