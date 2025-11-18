using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Abp.UI;

namespace CorarlERP.ItemTypes
{
    public class ItemTypeManager : CorarlERPDomainServiceBase, IItemTypeMannager
    {
        private readonly IRepository<ItemType, long> _ItermTypeRepository;

        public ItemTypeManager(IRepository<ItemType, long> itemTypeRepository)
        {
            _ItermTypeRepository = itemTypeRepository;
        }

        public async Task<IdentityResult> CreateAsync(ItemType entity, bool checkDuplicate = true)
        {
            if (checkDuplicate)
                await CheckDuplicateItemType(@entity);
            await _ItermTypeRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<ItemType> GetAsync(long id, bool tracking = false)
        {
            var @query = tracking ? _ItermTypeRepository.GetAll() : _ItermTypeRepository.GetAll().AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> UpdateAsync(ItemType entity, bool checkDuplicate = true)
        {
            if (checkDuplicate) await CheckDuplicateItemType(@entity);

            await _ItermTypeRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        private async Task CheckDuplicateItemType(ItemType @entity)
        {
            var @old = await _ItermTypeRepository
                             .GetAll().AsNoTracking()
                             .Where(u =>
                                       u.Id != entity.Id &&
                                       u.Name.ToLower() == entity.Name.ToLower())
                              .FirstOrDefaultAsync();

            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicateItemTypeName", entity.Name));
            }
        }
    }
}
