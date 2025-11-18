using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Items
{
    public class ItemManager : CorarlERPDomainServiceBase, IItemManager
    {
        private readonly IRepository<Item, Guid> _itemRepository;
        public ItemManager(IRepository<Item, Guid> itemRepository)
        {
            _itemRepository = itemRepository;
        }

        private async Task CheckDuplicateItem(Item @entity)
        {
            var @old = await _itemRepository.GetAll().AsNoTracking()
                           .Where(u => u.IsActive &&                                      
                                       u.ItemCode.ToLower() == entity.ItemCode.ToLower()&&
                                         u.Id != entity.Id
                                       )
                           .FirstOrDefaultAsync();
            if (old != null && old.ItemCode.ToLower() == entity.ItemCode.ToLower())
            {
                throw new UserFriendlyException(L("DuplicateItemCode", entity.ItemCode));

            }           

        }

        public async virtual Task<IdentityResult> CreateAsync(Item @entity)
        {
            await CheckDuplicateItem(entity);

            await _itemRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> RemoveAsync(Item @entity)
        {
            await _itemRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(Item @entity)
        {
            await CheckDuplicateItem(entity);

            await _itemRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> DisableAsync(Item @entity)
        {
            @entity.Enable(false);
            await _itemRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> EnableAsync(Item @entity)
        {
            @entity.Enable(true);
            await _itemRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<Item> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _itemRepository.GetAll()
                    .Include("Properties.Property")
                    .Include("Properties.PropertyValue")
                    .Include(u => u.ItemType)
                    .Include(u => u.SaleCurrency)
                    .Include(u => u.PurchaseCurrency)
                    .Include(u => u.SaleAccount)
                    .Include(u => u.PurchaseAccount)
                    .Include(u => u.InventoryAccount)
                    .Include(u => u.SaleTax)
                    .Include(u => u.PurchaseTax)
                    .Include(u => u.BatchNoFormula)
                        :
                    _itemRepository.GetAll()
                    .Include(u => u.ItemType)
                    .Include(u => u.SaleCurrency)
                    .Include(u => u.PurchaseCurrency)
                    .Include(u => u.SaleAccount)
                    .Include(u => u.PurchaseAccount)
                    .Include(u => u.InventoryAccount)
                    .Include(u => u.SaleTax)
                    .Include(u => u.PurchaseTax)
                    .Include(u => u.BatchNoFormula)
                    .Include("Properties.Property")
                    .Include("Properties.PropertyValue")
                    .AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
