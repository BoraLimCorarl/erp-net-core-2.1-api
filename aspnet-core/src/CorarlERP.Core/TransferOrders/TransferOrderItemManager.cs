using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.TransferOrders
{
    class TransferOrderItemManager : CorarlERPDomainServiceBase, ITransferOrderItemManager
    {
        private readonly IRepository<TransferOrderItem, Guid> _transferOrderItemRepository;

        public TransferOrderItemManager(IRepository<TransferOrderItem, Guid> ransferOrderItemRepository)
        {
            _transferOrderItemRepository = ransferOrderItemRepository;
        }

        public async Task<IdentityResult> CreateAsync(TransferOrderItem entity)
        {
            await _transferOrderItemRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<TransferOrderItem> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _transferOrderItemRepository.GetAll() :
                _transferOrderItemRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(TransferOrderItem entity)
        {
            await _transferOrderItemRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(TransferOrderItem entity)
        {
            await _transferOrderItemRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
