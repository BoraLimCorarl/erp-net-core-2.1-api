using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Locations.Dto;
using Microsoft.EntityFrameworkCore;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using Abp.Authorization;
using CorarlERP.Authorization;
using static CorarlERP.enumStatus.EnumStatus;
using System.Linq;
using CorarlERP.UserGroups;
using CorarlERP.Lots;
using Abp.Application.Features;
using CorarlERP.Features;
using CorarlERP.Authorization.Users;
using CorarlERP.Dto;

namespace CorarlERP.Locations
{
    public class LocationAppService : CorarlERPAppServiceBase, ILocationAppService
    {

        private readonly ILocationManager _locationManager;
        private readonly IRepository<Location, long> _locationRepository;

        private readonly IRepository<UserGroup, Guid> _userGroupRepository;
        private readonly IRepository<UserGroupMember, Guid> _userGroupMemberRepository;
        private readonly IUserGroupMemberManager _userGroupMemberManager;
        private readonly IUserGroupManager _userGroupManager;
        private readonly ILotManager _lotManager;
        private readonly IFeatureChecker _featureChecker;
        private readonly IRepository<User, long> _userRepository;

        public LocationAppService(
            ILocationManager locationManager,
            IRepository<Location, long> locationRepository,
            IRepository<UserGroup, Guid> userGroupRepository,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            IUserGroupMemberManager userGroupMemberManager,
            IUserGroupManager userGroupManager,
            ILotManager lotManager,
            IFeatureChecker featureChecker,
            IRepository<User, long> userRepository)
        {
            _lotManager = lotManager;
            _userGroupManager = userGroupManager;
            _userGroupMemberManager = userGroupMemberManager;
            _userGroupRepository = userGroupRepository;
            _userGroupMemberRepository = userGroupMemberRepository;
            _locationRepository = locationRepository;
            _locationManager = locationManager;
            _featureChecker = featureChecker;
            _userRepository = userRepository;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Locations_Create)]
        public async Task<ValidationCountOutput> GetMaxLocationCount()
        {
            var maxLocationCount = (await _featureChecker.GetValueAsync(AbpSession.TenantId.Value, AppFeatures.MaxLocationCount)).To<int>();
            var currentUserCount = await _locationRepository.CountAsync();
            return new ValidationCountOutput { MaxCurrentCount = currentUserCount, MaxCount = maxLocationCount };
        }


        private async Task CheckMaxLocationCountAsync(int tenantId, int itemCount)
        {
            var maxItemCount = (await _featureChecker.GetValueAsync(tenantId, AppFeatures.MaxLocationCount)).To<int>();
            if (maxItemCount <= 0)
            {
                return;
            }

            var currentUserCount = await _locationRepository.GetAll().AsNoTracking().CountAsync();
            if (currentUserCount + itemCount > maxItemCount)
            {
                throw new UserFriendlyException(L("MaximumLocationCount_Error_Detail", maxItemCount));
            }
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Locations_Create)]
        public async Task<LocationDetailOutput> Create(CreateLocationInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            await CheckMaxLocationCountAsync(tenantId, 0);

            if (input.Member == Member.UserGroup && input.LocationUserItems.Count() <= 0)
            {
                throw new UserFriendlyException(L("UserMemberGroupIsRequired"));
            }
          
            var @entity = Location.Create(tenantId, userId, input.LocationName, input.LocationParent, input.Member, input.ParentLocationId, input.PhoneNumber);

            CheckErrors(await _locationManager.CreateAsync(@entity));

            if (input.Member == Member.UserGroup)
            {
                var @userGroup = UserGroup.Create(tenantId, userId, input.LocationName);
                #region UserGroupMember           
                foreach (var userGroupMember in input.LocationUserItems)
                {
                    var @userItems = UserGroupMember.CreateUserGroupMember(tenantId, userId, userGroupMember.UserId, @userGroup);
                    CheckErrors(await _userGroupMemberManager.CreateAsync(@userItems));
                }
                #endregion
                @userGroup.UpdateLocationId(@entity.Id);
                CheckErrors(await _userGroupManager.CreateAsync(@userGroup));
            }

            //create default  lot by location
            var lot = Lot.Create(tenantId, userId, input.LocationName, entity.Id);
            CheckErrors(await _lotManager.CreateAsync(lot));

            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<LocationDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Locations_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            var @entity = await _locationManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _locationManager.RemoveAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Locations_Disable)]
        public async Task Disable(EntityDto<long> input)
        {
            var @entity = await _locationManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            if (entity.Member == Member.UserGroup)
            {

                var userGroup = await _userGroupRepository.GetAll().AsNoTracking()
                                .Where(t => t.LocationId == entity.Id).FirstOrDefaultAsync();
                if (userGroup != null)
                {
                    CheckErrors(await _userGroupManager.DisableAsync(userGroup));
                }
            }

            CheckErrors(await _locationManager.DisableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Locations_Enable)]
        public async Task Enable(EntityDto<long> input)
        {
            var @entity = await _locationManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            if (entity.Member == Member.UserGroup)
            {

                var userGroup = await _userGroupRepository.GetAll().AsNoTracking()
                                .Where(t => t.LocationId == entity.Id).FirstOrDefaultAsync();
                if (userGroup != null)
                {
                    CheckErrors(await _userGroupManager.EnableAsync(userGroup));
                }
            }
            CheckErrors(await _locationManager.EnableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Locations_Find)]
        public async Task<PagedResultDto<LocationDetailOutput>> Find(GetLocationListInput input)
        {
            if (input.IsExcept == null || input.IsExcept == false)
            {
                var userId = AbpSession.GetUserId();
                // get user by group member
                //var userGroups = await _userGroupMemberRepository.GetAll()
                //                .Include(t=>t.UserGroup)
                //                .Where(x => x.MemberId == userId)
                //                .Where(x => x.UserGroup.LocationId != null)
                //                .AsNoTracking()
                //                .Select(x => x.UserGroup.LocationId).ToListAsync();

                var result = from l in _locationRepository.GetAll()
                                                 //.Where(t => t.Member == Member.UserGroup)
                                                 //.Where(t => userGroups.Contains(t.Id))
                                                 .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                                                 .WhereIf(
                                                     !input.Filter.IsNullOrEmpty(),
                                                     p => p.LocationName.ToLower().Contains(input.Filter.ToLower()))
                                                 .AsNoTracking()

                                join g in _userGroupMemberRepository.GetAll()
                                    .Include(t => t.UserGroup)
                                    //.Where(x => x.MemberId == userId)
                                    .Where(x => x.UserGroup.LocationId != null)
                                    .AsNoTracking()
                                on l.Id equals g.UserGroup.LocationId

                                into groups
                                from g in groups.DefaultIfEmpty()

                                where l.Member == Member.All || (g != null && g.MemberId == userId && g.UserGroup.LocationId != null)
                                select l;
                
                var resultCount = await result.CountAsync();
                var @entities = await result
                                    //.OrderBy(input.Sorting)
                                    .OrderBy(s => s.LocationName)
                                    .PageBy(input)
                                    .ToListAsync();
                return new PagedResultDto<LocationDetailOutput>(resultCount, ObjectMapper.Map<List<LocationDetailOutput>>(@entities));
            }
            else
            {
                var @queryAll = _locationRepository
                                 .GetAll()
                                 .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                                 .WhereIf(
                                     !input.Filter.IsNullOrEmpty(),
                                     p => p.LocationName.ToLower().Contains(input.Filter.ToLower()))
                                 .AsNoTracking()
                                 .Select(t => new LocationDetailOutput
                                 {
                                     Id = t.Id,
                                     LocationName = t.LocationName,
                                     Member = t.Member,
                                     IsActive = t.IsActive
                                 });

                var result = @queryAll;
                var resultCount = await result.CountAsync();
                var @entities = await result
                                    //.OrderBy(input.Sorting)
                                    .OrderBy(s => s.LocationName)
                                    .PageBy(input)
                                    .ToListAsync();
                return new PagedResultDto<LocationDetailOutput>(resultCount, ObjectMapper.Map<List<LocationDetailOutput>>(@entities));
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Locations_GetDetail)]
        public async Task<LocationDetailOutput> GetDetail(EntityDto<long> input)
        {
            var @entity = await _locationManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var result = ObjectMapper.Map<LocationDetailOutput>(entity);
            result.LocationUserItems = await _userGroupMemberRepository.GetAll()
                                        .Include(t => t.UserGroup)
                                        .Include(t => t.Member)
                                        .Where(t => t.UserGroup.LocationId == input.Id)
                                        .AsNoTracking()
                                        .Select(t => new LocationUserItems
                                        {
                                            UserId = t.MemberId,
                                            UserName = t.Member.UserName,
                                            Id = t.Id
                                        }).ToListAsync();
            return ObjectMapper.Map<LocationDetailOutput>(result);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Locations_GetList)]
        public async Task<PagedResultDto<LocationDetailOutput>> GetList(GetLocationListInput input)
        {
            var @query = _locationRepository
                        .GetAll()
                        .Include(u => u.ParentLocation)
                        .AsNoTracking()
                        .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                        .WhereIf(
                            !input.Filter.IsNullOrEmpty(),
                            p => p.LocationName.ToLower().Contains(input.Filter.ToLower()));
            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<LocationDetailOutput>(resultCount, ObjectMapper.Map<List<LocationDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_Locations_Update)]
        public async Task<LocationDetailOutput> Update(UpdateLocationInput input)
        {

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _locationManager.GetAsync(input.Id, true);
            if (input.Member == Member.UserGroup && input.LocationUserItems.Count() <= 0)
            {
                throw new UserFriendlyException(L("UserMemberGroupIsRequired"));
            }
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            if (input.Member == Member.All)
            {
                if (entity.Member == Member.UserGroup)
                {
                    var toDeleteUserGroup = await _userGroupRepository.GetAll().Where(u => u.LocationId == input.Id).FirstOrDefaultAsync();
                    CheckErrors(await _userGroupManager.RemoveAsync(toDeleteUserGroup));
                    var toDeleteUserItems = await _userGroupMemberRepository.GetAll().Where(u => u.UserGroupId == toDeleteUserGroup.Id).ToListAsync();
                    foreach (var t in toDeleteUserItems)
                    {
                        CheckErrors(await _userGroupMemberManager.RemoveAsync(t));
                    }
                }
            }
            else
            {
                Guid userGroupId;
                var userGroup = await _userGroupRepository.GetAll().Where(u => u.LocationId == input.Id).FirstOrDefaultAsync();
                if (userGroup != null)
                {
                    userGroupId = userGroup.Id;
                    userGroup.Update(userId, input.LocationName);
                    CheckErrors(await _userGroupManager.UpdateAsync(userGroup));
                }
                else
                {
                    var userGroupCreate = UserGroup.Create(tenantId, userId, input.LocationName);
                    userGroupCreate.UpdateLocationId(input.Id);

                    CheckErrors(await _userGroupManager.CreateAsync(userGroupCreate));
                    userGroupId = userGroupCreate.Id;
                }

                var userGroupItems = await _userGroupMemberRepository.GetAll().Where(u => u.UserGroupId == userGroupId).ToListAsync();
                var toDeleteUserItems = userGroupItems.Where(u => !input.LocationUserItems.Any(i => i.Id != null && i.Id == u.Id)).ToList();

                foreach (var i in input.LocationUserItems)
                {
                    if (i.Id == null)
                    {
                        var @entityItem = UserGroupMember.CreateUserGroupMember(tenantId, userId, i.UserId, userGroupId);
                        CheckErrors(await _userGroupMemberManager.CreateAsync(@entityItem));
                    }
                    else
                    {
                        var userItem = userGroupItems.FirstOrDefault(u => u.Id == i.Id);
                        if (userItem != null)
                        {
                            userItem.UpdateUserGroupMember(userId, i.UserId);
                            CheckErrors(await _userGroupMemberManager.UpdateAsync(userItem));
                        }
                    }
                }

                foreach (var t in toDeleteUserItems)
                {
                    CheckErrors(await _userGroupMemberManager.RemoveAsync(t));
                }
            }

            entity.Update(userId, input.LocationName, input.LocationParent, input.Member, input.ParentLocationId, input.PhoneNumber);
            CheckErrors(await _locationManager.UpdateAsync(@entity));

            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<LocationDetailOutput>(@entity);
        }
    }
}
