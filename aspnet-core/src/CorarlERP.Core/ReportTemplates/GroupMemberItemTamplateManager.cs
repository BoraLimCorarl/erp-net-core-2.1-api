using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ReportTemplates
{
   public class GroupMemberItemTamplateManager : CorarlERPDomainServiceBase, IGroupMemberItemTamplateManager
    {
        private readonly IRepository<GroupMemberItemTamplate, Guid> _groupMemberItemTamplateRepository;
        public GroupMemberItemTamplateManager(IRepository<GroupMemberItemTamplate, Guid> groupMemberItemTamplateRepository)
        {
            _groupMemberItemTamplateRepository = groupMemberItemTamplateRepository;
        }

        public async virtual Task<IdentityResult> CreateAsync(GroupMemberItemTamplate cEntity)
        {
           
            await _groupMemberItemTamplateRepository.InsertAsync(cEntity);
            return IdentityResult.Success;
        }

        public async virtual Task<GroupMemberItemTamplate> GetAsync(Guid id, bool tracking = true)
        {
            var @query = tracking ? _groupMemberItemTamplateRepository.GetAll()
                .Include(u => u.UserGroup)
                .Include(u => u.MemberUser)
                .Include(u=>u.ReportTemplate)
                : _groupMemberItemTamplateRepository.GetAll()
                .Include(u => u.UserGroup)
                .Include(u => u.MemberUser)
                .Include(u => u.ReportTemplate)               
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(GroupMemberItemTamplate entity)
        {
            await _groupMemberItemTamplateRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(GroupMemberItemTamplate entity)
        {
            await _groupMemberItemTamplateRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
