using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.UserGroups
{
   public class UserGroupManager : CorarlERPDomainServiceBase, IUserGroupManager
    {
        private readonly IRepository<UserGroup, Guid> _userGroupRepository;

        public UserGroupManager(IRepository<UserGroup, Guid> userGroupRepository)
        {
            _userGroupRepository = userGroupRepository;
        }
        private async Task CheckDuplicateUserGroup(UserGroup @entity)
        {
            var @old = await _userGroupRepository.GetAll().AsNoTracking()
                           .Where(u => u.IsActive &&
                                       u.Name.ToLower() == entity.Name.ToLower() &&
                                       u.Id != entity.Id)
                           .FirstOrDefaultAsync();

            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicateName", entity.Name));
            }
        }
        public async virtual Task<IdentityResult> CreateAsync(UserGroup entity)
        {
            await CheckDuplicateUserGroup(entity);

            await _userGroupRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> DisableAsync(UserGroup entity)
        {
            @entity.Enable(false);
            await _userGroupRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> EnableAsync(UserGroup entity)
        {
            @entity.Enable(true);
            await _userGroupRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<UserGroup> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _userGroupRepository.GetAll()
                : _userGroupRepository.GetAll().AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(UserGroup entity)
        {
            await _userGroupRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(UserGroup entity)
        {
            await CheckDuplicateUserGroup(entity);

            await _userGroupRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}

