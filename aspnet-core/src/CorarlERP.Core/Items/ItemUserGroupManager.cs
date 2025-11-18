using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Items
{
    public class ItemUserGroupManager : CorarlERPDomainServiceBase, IItemUserGroupManager
    {
        private readonly IRepository<ItemsUserGroup, Guid> _userGroupItemsRepository;

        public ItemUserGroupManager(IRepository<ItemsUserGroup, Guid> userGroupRepository)
        {
            _userGroupItemsRepository = userGroupRepository;
        }

        public async virtual Task<IdentityResult> CreateAsync(ItemsUserGroup entity)
        {
            await _userGroupItemsRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<ItemsUserGroup> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _userGroupItemsRepository.GetAll()
                .Include(u => u.UserGroup)
                :
                _userGroupItemsRepository.GetAll()
                .Include(u => u.UserGroup)
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(ItemsUserGroup entity)
        {
            await _userGroupItemsRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(ItemsUserGroup entity)
        {
            await _userGroupItemsRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
