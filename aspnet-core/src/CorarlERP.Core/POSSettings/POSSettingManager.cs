using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.POSSettings
{
   public class POSSettingManager : CorarlERPDomainServiceBase, IPOSSettingManager
    {
        private readonly IRepository<POSSetting, long> _POSSettingRepository;

        public POSSettingManager(IRepository<POSSetting, long> POSSettingRepository)
        {
            _POSSettingRepository = POSSettingRepository;
        }

        public async  Task<IdentityResult> CreateAsync(POSSetting entity)
        {        
            await _POSSettingRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async  Task<IdentityResult> RemoveAsync(POSSetting entity)
        {
            await _POSSettingRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }
        public async  Task<IdentityResult> UpdateAsync(POSSetting entity)
        {        
            await _POSSettingRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
