using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;

namespace CorarlERP.InventoryCostCloses
{
    public class InventoryCostCloseManager : CorarlERPDomainServiceBase, IInventoryCostCloseManager
    {
        private readonly IRepository<InventoryCostClose, Guid> _inventoryCostCloseRepository;

        public InventoryCostCloseManager(IRepository<InventoryCostClose, Guid> inventoryCostCloseRepository)
        {
            _inventoryCostCloseRepository = inventoryCostCloseRepository;
        }

        public async Task<IdentityResult> CreateAsync(InventoryCostClose entity)
        {
            await _inventoryCostCloseRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> RemoveAsync(InventoryCostClose entity)
        {
            await _inventoryCostCloseRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
