using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.UI;
using CorarlERP.AutoSequences;
using CorarlERP.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.DeliverySchedules
{
    public class DeliveryScheduleManager : CorarlERPDomainServiceBase, IDeliveryScheduleManager
    {

        private readonly IRepository<DeliverySchedule, Guid> _deliveryScheduleRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;
        public DeliveryScheduleManager(
            IRepository<DeliverySchedule, Guid> deliveryScheduleRepository,       
            IRepository<AccountCycles.AccountCycle, long> accountCycleRepository,
            IAutoSequenceManager autoSequenceManager) : base(accountCycleRepository)
        {
            _deliveryScheduleRepository = deliveryScheduleRepository;
            _autoSequenceManager = autoSequenceManager;
        }
        public async Task<IdentityResult> CreateAsync(DeliverySchedule entity, bool checkDupliateReference)
        {
            await CheckDuplicate(entity);
            await CheckClosePeriod(entity.InitialDeliveryDate);

            if (checkDupliateReference) await CheckDuplicateReferenceNo(entity);

            await _deliveryScheduleRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<DeliverySchedule> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _deliveryScheduleRepository.GetAll()
                .Include(u => u.Customer.Account)
                .Include(u => u.Location)
                :
                _deliveryScheduleRepository.GetAll()
                .Include(u => u.Customer.Account)
                .Include(u => u.Location)
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(DeliverySchedule entity)
        {
            await CheckClosePeriod(entity.InitialDeliveryDate);
            await _deliveryScheduleRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(DeliverySchedule entity, bool validateReference = true)
        {
            await CheckClosePeriod(entity.InitialDeliveryDate);
            await CheckDuplicate(entity);

            //var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.DeliverySchedule);
            //if (auto.RequireReference && validateReference) await CheckDuplicateReferenceNo(entity);

            await _deliveryScheduleRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        private async Task CheckDuplicate(DeliverySchedule @entity)
        {
            var @old = await _deliveryScheduleRepository.GetAll().AsNoTracking()
                           .Where(u =>
                                       u.DeliveryNo.ToLower() == entity.DeliveryNo.ToLower() &&
                                       u.Id != entity.Id
                                       )
                           .FirstOrDefaultAsync();

            if (old != null && old.DeliveryNo.ToLower() == entity.DeliveryNo.ToLower())
            {
                throw new UserFriendlyException(L("DuplicateDeliveryNo", entity.DeliveryNo));
            }
        }
        private async Task CheckDuplicateReferenceNo(DeliverySchedule @entity)
        {
            if (entity.Reference.IsNullOrEmpty())
            {
                throw new UserFriendlyException(L("RequireReferenceNo"));
            }

            var @old = await _deliveryScheduleRepository.GetAll().AsNoTracking()
                           .Where(u =>
                                       u.Reference.ToLower() == entity.Reference.ToLower() &&
                                       u.Id != entity.Id && u.CustomerId == entity.CustomerId)
                           .FirstOrDefaultAsync();

            if (old != null && old.Reference.ToLower() == entity.Reference.ToLower())
            {
                throw new UserFriendlyException(L("DuplicateReferenceNo", entity.Reference));
            }
        }
    }
}
