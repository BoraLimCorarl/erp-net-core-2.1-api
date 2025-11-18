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

namespace CorarlERP.TransferOrders
{
    public class TransferOrderManager : CorarlERPDomainServiceBase, ITransferOrderManager
    {
        private readonly IRepository<TransferOrder, Guid> _transferOrderRepository;
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;
        public TransferOrderManager(IRepository<TransferOrder, Guid> TransferOrderRepository,
            IRepository<Tenant,int>tenantRepository,
            IRepository<AccountCycles.AccountCycle, long> accountCycleRepository,
            IAutoSequenceManager autoSequenceManager) : base(accountCycleRepository)
        {
            _autoSequenceManager = autoSequenceManager;
            _transferOrderRepository = TransferOrderRepository;
            _tenantRepository = tenantRepository;
        }
        public async Task<IdentityResult> CreateAsync(TransferOrder entity, bool checkDupliateReference)
        {
            await CheckDuplicateTransferOrder(entity);
            await CheckClosePeriod(entity.TransferDate);
            if (checkDupliateReference) await CheckDuplicateReferenceNo(entity);
            await _transferOrderRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }


        private async Task CheckDuplicateReferenceNo(TransferOrder @entity)
        {
            if (entity.Reference.IsNullOrEmpty())
            {
                throw new UserFriendlyException(L("RequireReferenceNo"));
            }

            var @old = await _transferOrderRepository.GetAll().AsNoTracking()
                           .Where(u => u.Reference.ToLower() == entity.Reference.ToLower() && u.Id != entity.Id)
                           .FirstOrDefaultAsync();
            if (old != null && old.Reference.ToLower() == entity.Reference.ToLower())
            {
                throw new UserFriendlyException(L("DuplicateReferenceNo"));
            }
        }

        public async Task<TransferOrder> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _transferOrderRepository.GetAll()
                        .Include(u => u.TransferFromLocation)
                        .Include(u => u.TransferToLocation)
                        .Include(u => u.TransferFromClass)
                        .Include(u => u.TransferToClass)
                        :
                        _transferOrderRepository.GetAll()
                        .Include(u => u.TransferFromLocation)
                        .Include(u => u.TransferToLocation)
                        .Include(u => u.TransferFromClass)
                        .Include(u => u.TransferToClass)
                        .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(TransferOrder entity)
        {
            await CheckClosePeriod(entity.TransferDate);
            await _transferOrderRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(TransferOrder entity)
        {
          
            await CheckDuplicateTransferOrder(entity);
            await CheckClosePeriod(entity.TransferDate);

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.TransferOrder);
            if (auto.RequireReference) await CheckDuplicateReferenceNo(entity);


            await _transferOrderRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        private async Task CheckDuplicateTransferOrder(TransferOrder @entity)
        {
            var @old = await _transferOrderRepository.GetAll().AsNoTracking()
                           .Where(u => u.TransferNo.ToLower() == entity.TransferNo.ToLower() && u.Id != entity.Id)
                           .FirstOrDefaultAsync();

            if (old != null && old.TransferNo.ToLower() == entity.TransferNo.ToLower())
            {
                throw new UserFriendlyException(L("DuplicateTransferOrderNumber", entity.TransferNo));
            }
        }
    }
}
