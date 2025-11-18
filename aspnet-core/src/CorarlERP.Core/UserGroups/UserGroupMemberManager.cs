using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CorarlERP.UserGroups
{
    public class UserGroupMemberManager : CorarlERPDomainServiceBase, IUserGroupMemberManager
    {
        private readonly IRepository<UserGroupMember, Guid> _userGroupMemberRepository;
        public UserGroupMemberManager(IRepository<UserGroupMember, Guid> userGroupMemberRepository)
        {
            _userGroupMemberRepository = userGroupMemberRepository;
        }
        private async Task CheckDuplicateContactPerson(UserGroupMember @entity)
        {
            var @old = await _userGroupMemberRepository.GetAll().AsNoTracking()
                           .Where(u => u.Id == entity.Id)
                           .FirstOrDefaultAsync();

            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicateUserGroup")); //this contact person is created already
            }
        }
        public async virtual Task<IdentityResult> CreateAsync(UserGroupMember cEntity)
        {
            await CheckDuplicateContactPerson(cEntity);
            await _userGroupMemberRepository.InsertAsync(cEntity);
            return IdentityResult.Success;
        }

        public async virtual Task<UserGroupMember> GetAsync(Guid id, bool tracking = true)
        {
            var @query = tracking ? _userGroupMemberRepository.GetAll()
                .Include(u => u.UserGroup)
                .Include(u => u.Member)
                : _userGroupMemberRepository.GetAll()
                .Include(u => u.UserGroup)
                .Include(u => u.Member)
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(UserGroupMember entity)
        {
            await _userGroupMemberRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(UserGroupMember entity)
        {
            await _userGroupMemberRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
