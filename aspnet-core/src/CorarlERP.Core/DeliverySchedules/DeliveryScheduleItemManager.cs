using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using Abp.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static CorarlERP.enumStatus.EnumStatus;


namespace CorarlERP.DeliverySchedules
{
    public class DeliveryScheduleItemManager : CorarlERPDomainServiceBase, IDeliveryScheduleItemManager
    {

        private readonly IRepository<DeliveryScheduleItem, Guid> _deliveryScheduleRepository;  
        public DeliveryScheduleItemManager(
            IRepository<DeliveryScheduleItem, Guid> deliveryScheduleRepository) 
        {
            _deliveryScheduleRepository = deliveryScheduleRepository;      
        }
        public async Task<IdentityResult> CreateAsync(DeliveryScheduleItem entity)
        {       
           
            await _deliveryScheduleRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<DeliveryScheduleItem> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _deliveryScheduleRepository.GetAll()               
                :
                _deliveryScheduleRepository.GetAll()             
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(DeliveryScheduleItem entity)
        {        
            await _deliveryScheduleRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(DeliveryScheduleItem entity)
        {
            await _deliveryScheduleRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
