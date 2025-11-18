using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using CorarlERP.Items;

namespace CorarlERP.InventoryCalculationItems
{
    public class InventoryCalculationItemManager : CorarlERPDomainServiceBase, IInventoryCalculationItemManager
    {
        private readonly IRepository<InventoryCalculationItem, Guid> _inventoryCalculationItemRepository;

        public InventoryCalculationItemManager(IRepository<InventoryCalculationItem, Guid> inventoryCalculationItemRepository)
        {
            _inventoryCalculationItemRepository = inventoryCalculationItemRepository;
        }

        public async Task<IdentityResult> CreateAsync(InventoryCalculationItem entity)
        {
            await _inventoryCalculationItemRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<InventoryCalculationItem> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _inventoryCalculationItemRepository.GetAll() :
                _inventoryCalculationItemRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(InventoryCalculationItem entity)
        {
            await _inventoryCalculationItemRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(InventoryCalculationItem entity)
        {
            await _inventoryCalculationItemRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> TrackChangeAsync(int tenantId, long userId, DateTime date, List<Guid> items)
        {
            var scheduleItems = await _inventoryCalculationItemRepository.GetAll()
                                      .Where(s => items.Contains(s.ItemId))
                                      .ToListAsync();

            var addItems = items;

            if (scheduleItems.Any())
            {
                addItems = items.Where(s => !scheduleItems.Select(i => i.ItemId).Contains(s)).ToList();

                foreach( var i in scheduleItems)
                {
                    if (date > i.Date) continue;
                    i.Update(userId, date, i.ItemId);
                    await _inventoryCalculationItemRepository.UpdateAsync(i);
                }
            }
            
            foreach(var i in addItems)
            {
                var item = InventoryCalculationItem.Create(tenantId, userId, date, i);
                await _inventoryCalculationItemRepository.InsertAsync(item);
            }

            return IdentityResult.Success;
        }
    }
}
