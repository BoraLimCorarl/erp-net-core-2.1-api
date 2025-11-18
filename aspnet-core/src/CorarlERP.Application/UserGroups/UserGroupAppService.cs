using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.UserGroups.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using CorarlERP.Authorization;
using CorarlERP.Locations;

namespace CorarlERP.UserGroups
{
    [AbpAuthorize]
    public class UserGroupAppService : CorarlERPAppServiceBase, IUserGroupAppService
    {
        private readonly IUserGroupManager _userGroupManager;
        private readonly IRepository<UserGroup, Guid> _userGroupRepository;
        private readonly IRepository<UserGroupMember, Guid> _userGroupMemberRepository;
        private readonly IUserGroupMemberManager _userGroupMemberManager;

        private readonly ILocationManager _locationManager;
        private readonly IRepository<Location, long> _locationRepository;
        public UserGroupAppService(
            IUserGroupManager userGroupManager,
            IRepository<UserGroup, Guid> userGroupRepository,
            IRepository<UserGroupMember,Guid> userGroupMemberRepository,
            IUserGroupMemberManager userGroupMemberManager,
            ILocationManager locationManager,
            IRepository<Location, long> locationRepository)
        {
            _userGroupManager = userGroupManager;
            _userGroupRepository = userGroupRepository;
            _userGroupMemberManager = userGroupMemberManager;
            _userGroupMemberRepository = userGroupMemberRepository;

            _locationRepository = locationRepository;
            _locationManager = locationManager;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_UserGroups_Create)]
        public async Task<NullableIdDto<Guid>> Create(CreateUserGroupInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            var @entity = UserGroup.Create(tenantId, userId, input.Name);
           
            if (input.UserGroupMember.Count == 0)
            {
                throw new UserFriendlyException(L("MemberNeedRecord"));
            }
            #region UserGroupMember           
            foreach (var userGroupMember in input.UserGroupMember)
            {
                var @contactPersons = UserGroupMember.CreateUserGroupMember(tenantId, userId, userGroupMember.MemberId,entity);
                CheckErrors(await _userGroupMemberManager.CreateAsync(@contactPersons));
            }
            #endregion
            CheckErrors(await _userGroupManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }


        [AbpAuthorize(AppPermissions.Pages_Administration_UserGroups_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            var @entity = await _userGroupManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            if (entity.LocationId != null)
            {
                throw new UserFriendlyException(L("RecordCannotDeleteFromAutoLocation"));
            }

            var contactPersons = await _userGroupMemberRepository.GetAll().Where(u => u.UserGroupId == entity.Id).ToListAsync();

            foreach (var s in contactPersons)
            {
                CheckErrors(await _userGroupMemberManager.RemoveAsync(s));
            }


            CheckErrors(await _userGroupManager.RemoveAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_UserGroups_Disable)]
        public async Task Disable(EntityDto<Guid> input)
        {
            var @entity = await _userGroupManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            if(entity.LocationId != null)
            {
                var location = await _locationRepository.GetAll().AsNoTracking()
                                .Where(t => t.Id == entity.LocationId).FirstOrDefaultAsync();
                if (location != null)
                {
                    CheckErrors(await _locationManager.DisableAsync(location));
                }
            }
            CheckErrors(await _userGroupManager.DisableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_UserGroups_Enable)]
        public async Task Enable(EntityDto<Guid> input)
        {
            var @entity = await _userGroupManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            if (entity.LocationId != null)
            {
                var location = await _locationRepository.GetAll().AsNoTracking()
                                .Where(t => t.Id == entity.LocationId).FirstOrDefaultAsync();
                if (location != null)
                {
                    CheckErrors(await _locationManager.EnableAsync(location));
                }
            }
            CheckErrors(await _userGroupManager.EnableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_UserGroups_Find)]
        public async Task<PagedResultDto<GetListUserGroupOutput>> Find(GetListUserGroupInput input)
        {
            var @query = _userGroupRepository
                .GetAll()               
                .AsNoTracking()
                .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                .WhereIf(input.IsLocation == true , p => p.LocationId != null)
                .WhereIf(
                    !input.Filter.IsNullOrEmpty(),
                    p => p.Name.ToLower().Contains(input.Filter.ToLower()) ||
                         p.Name.ToLower().Contains(input.Filter.ToLower())
                );
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<GetListUserGroupOutput>(resultCount, ObjectMapper.Map<List<GetListUserGroupOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_UserGroups_GetDetail)]
        public async Task<UserGroupDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            var @entity = await _userGroupManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var userGroupMembers = await _userGroupMemberRepository.GetAll()
                .Include(u=>u.Member).Where(u => u.UserGroupId == entity.Id).
                Select(t=> new UserGroupMemberDetail
                {
                    Id = t.Id,
                    Member = ObjectMapper.Map<MemberDetail>(t.Member),
                    MemberId = t.MemberId                    
                }).ToListAsync();

            var result = ObjectMapper.Map<UserGroupDetailOutput>(@entity);

            result.UserGroupMemberDetails = ObjectMapper.Map<List<UserGroupMemberDetail>>(userGroupMembers);

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_UserGroups_GetList)]
        public async Task<PagedResultDto<GetListUserGroupOutput>> GetList(GetListUserGroupInput input)
        {
            var @query = _userGroupRepository
               .GetAll()
               .AsNoTracking()
               .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
               .WhereIf(
                   !input.Filter.IsNullOrEmpty(),
                   p => p.Name.ToLower().Contains(input.Filter.ToLower()) ||
                        p.Name.ToLower().Contains(input.Filter.ToLower())
               );
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<GetListUserGroupOutput>(resultCount, ObjectMapper.Map<List<GetListUserGroupOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_UserGroups_Update)]
        public async Task<NullableIdDto<Guid>> Update(UpdateUserGroupInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _userGroupManager.GetAsync(input.Id, true);
            if (input.UserGroupMember.Count == 0)
            {
                throw new UserFriendlyException(L("MemberNeedRecord"));
            }
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            entity.Update(userId,input.Name); 
            
            var userGroupMembers = await _userGroupMemberRepository.GetAll().Where(u => u.UserGroupId == entity.Id).ToListAsync();
            foreach (var c in input.UserGroupMember)
            {
                if (c.Id != null)
                {
                    var userGroupMember = userGroupMembers.FirstOrDefault(u => u.Id == c.Id);
                    if (userGroupMember != null)
                    {                     
                        userGroupMember.UpdateUserGroupMember(userId,c.MemberId);
                        CheckErrors(await _userGroupMemberManager.UpdateAsync(userGroupMember));
                    }
                }
                else if (c.Id == null)
                {                  
                    var userGroupMember = UserGroupMember.CreateUserGroupMember(tenantId, userId, c.MemberId,entity.Id);
                    CheckErrors(await _userGroupMemberManager.CreateAsync(userGroupMember));

                }
            }

            var toDeleteUserGroupMember = userGroupMembers.Where(u => !input.UserGroupMember.Any(i => i.Id != null && i.Id == u.Id)).ToList();

            foreach (var t in toDeleteUserGroupMember)
            {
                CheckErrors(await _userGroupMemberManager.RemoveAsync(t));
            }
            
            CheckErrors(await _userGroupManager.UpdateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }
    }
}
