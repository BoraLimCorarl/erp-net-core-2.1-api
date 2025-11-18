using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CorarlERP.Settings
{
    public  class BillInvoiceSettingManager : CorarlERPDomainServiceBase, IBillInvoiceSettingManager
    {
        private readonly IRepository<BillInvoiceSetting, long> _settingRepository;

        public BillInvoiceSettingManager(IRepository<BillInvoiceSetting, long> settingRepository)
        {
            _settingRepository = settingRepository;
        }
        public async virtual Task<IdentityResult> CreateAsync(BillInvoiceSetting entity)
        {
            await _settingRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(BillInvoiceSetting entity)
        {
            await _settingRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<BillInvoiceSetting> GetAsync(long id)
        {
            var @query = _settingRepository.GetAll();

            return await query.Where(s => s.Id == id).FirstOrDefaultAsync();
        }
              
    }
}
