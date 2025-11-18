using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.InventoryCostCloses
{
    public class InventoryCostCloseItemManager : CorarlERPDomainServiceBase, IInventoryCostCloseItemManager
    {
        private readonly IRepository<InventoryCostCloseItem, Guid> _inventoryCostCloseItemRepository;

        public InventoryCostCloseItemManager(IRepository<InventoryCostCloseItem, Guid> inventoryCostCloseItemRepository)
        {
            _inventoryCostCloseItemRepository = inventoryCostCloseItemRepository;
        }

        public async Task<IdentityResult> CreateAsync(InventoryCostCloseItem entity)
        {
            await _inventoryCostCloseItemRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> RemoveAsync(InventoryCostCloseItem entity)
        {
            await _inventoryCostCloseItemRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
