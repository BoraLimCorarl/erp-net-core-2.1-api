using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.InventoryTransactionItems
{
    public class InventoryTransactionItemManager : CorarlERPDomainServiceBase, IInventoryTransactionItemManager
    {
        private readonly IRepository<InventoryTransactionItem, Guid> _inventoryTransactionItemRepository;

        public InventoryTransactionItemManager(IRepository<InventoryTransactionItem, Guid> inventoryTransactionItemRepository)
        {
            _inventoryTransactionItemRepository = inventoryTransactionItemRepository;
        }

        public async Task<IdentityResult> CreateAsync(InventoryTransactionItem entity)
        {
            await _inventoryTransactionItemRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(InventoryTransactionItem entity)
        {
            await _inventoryTransactionItemRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> RemoveAsync(InventoryTransactionItem entity)
        {
            await _inventoryTransactionItemRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<InventoryTransactionItem> GetAsync(Guid id, bool tracking = false)
        {
            var query = _inventoryTransactionItemRepository.GetAll().Where(s => s.Id == id);
            return tracking ? await query.FirstOrDefaultAsync() : await query.AsNoTracking().FirstOrDefaultAsync();
        }
    }
}
