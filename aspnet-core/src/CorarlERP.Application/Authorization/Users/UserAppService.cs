using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Configuration;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Notifications;
using Abp.Organizations;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CorarlERP.Authorization.Permissions;
using CorarlERP.Authorization.Permissions.Dto;
using CorarlERP.Authorization.Roles;
using CorarlERP.Authorization.Users.Dto;
using CorarlERP.Authorization.Users.Exporting;
using CorarlERP.Dto;
using CorarlERP.Notifications;
using CorarlERP.Url;
using CorarlERP.Organizations.Dto;
using CorarlERP.UserGroups;
using Abp.Dependency;
using Microsoft.AspNetCore.Hosting;
using CorarlERP.Configuration;
using CorarlERP.VendorTypes;
using CorarlERP.CustomerTypes;
using Abp.Application.Features;
using CorarlERP.Features;
using CorarlERP.Locations.Dto;

namespace CorarlERP.Authorization.Users
{
    [AbpAuthorize(AppPermissions.Pages_Administration_Users)]
    public class UserAppService : CorarlERPAppServiceBase, IUserAppService
    {
        public IAppUrlService AppUrlService { get; set; }

        private readonly RoleManager _roleManager;
        private readonly IUserEmailer _userEmailer;
        private readonly IUserGroupMemberManager _userGroupMemberManager;
        private readonly IRepository<UserGroupMember, Guid> _userGroupMemberRepository;
        private readonly IUserListExcelExporter _userListExcelExporter;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly IAppNotifier _appNotifier;
        private readonly IRepository<RolePermissionSetting, long> _rolePermissionRepository;
        private readonly IRepository<UserPermissionSetting, long> _userPermissionRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IUserPolicy _userPolicy;
        private readonly IEnumerable<IPasswordValidator<User>> _passwordValidators;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRepository<OrganizationUnit, long> _organizationUnitRepository;
        private readonly IRepository<VendorTypeMember, long> _vendorTypeMemeberRepository;
        private readonly IVendorTypeMemberManager _vendorTypeMemeberManager;
        private readonly IRepository<CustomerTypeMember, long> _customerTypeMemeberRepository;
        private readonly ICustomerTypeMemberManager _customerTypeMemeberManager;
        private readonly IFeatureChecker _featureChecker;
        private readonly IRepository<User, long> _userRepository;
        public UserAppService(
            RoleManager roleManager,
            IUserEmailer userEmailer,
            IUserListExcelExporter userListExcelExporter,
            INotificationSubscriptionManager notificationSubscriptionManager,
            IAppNotifier appNotifier,
            IRepository<RolePermissionSetting, long> rolePermissionRepository,
            IRepository<UserPermissionSetting, long> userPermissionRepository,
            IRepository<UserRole, long> userRoleRepository,
            IUserPolicy userPolicy,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            IPasswordHasher<User> passwordHasher,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
            IUserGroupMemberManager userGroupMember,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            IRepository<CustomerTypeMember, long> customerTypeMemeberRepository,
            ICustomerTypeMemberManager customerTypeMemeberManager,
            IRepository<VendorTypeMember, long> vendorTypeMemeberRepository,
            IVendorTypeMemberManager vendorTypeMemeberManager,
            IRepository<User, long> userRepository,
             IFeatureChecker featureChecker)
        {
            _userGroupMemberRepository = userGroupMemberRepository;
            _userGroupMemberManager = userGroupMember;
            _roleManager = roleManager;
            _userEmailer = userEmailer;
            _userListExcelExporter = userListExcelExporter;
            _notificationSubscriptionManager = notificationSubscriptionManager;
            _appNotifier = appNotifier;
            _rolePermissionRepository = rolePermissionRepository;
            _userPermissionRepository = userPermissionRepository;
            _userRoleRepository = userRoleRepository;
            _userPolicy = userPolicy;
            _passwordValidators = passwordValidators;
            _passwordHasher = passwordHasher;
            _organizationUnitRepository = organizationUnitRepository;
            _vendorTypeMemeberRepository = vendorTypeMemeberRepository;
            _vendorTypeMemeberManager = vendorTypeMemeberManager;
            _customerTypeMemeberRepository = customerTypeMemeberRepository;
            _customerTypeMemeberManager = customerTypeMemeberManager;
            _featureChecker = featureChecker;
            AppUrlService = NullAppUrlService.Instance;
           _userRepository = userRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_GetList)]
        public async Task<PagedResultDto<UserListDto>> GetUsers(GetUsersInput input)
        {
            var query = UserManager.Users
                .WhereIf(input.Role.HasValue, u => u.Roles.Any(r => r.RoleId == input.Role.Value))
                .WhereIf(input.OnlyLockedUsers, u => u.LockoutEndDateUtc.HasValue && u.LockoutEndDateUtc.Value > DateTime.UtcNow)
                .WhereIf(
                    !input.Filter.IsNullOrWhiteSpace(),
                    u =>
                        u.Name.Contains(input.Filter) ||
                        u.Surname.Contains(input.Filter) ||
                        u.UserName.Contains(input.Filter) ||
                        u.EmailAddress.Contains(input.Filter)
                );

            if (!input.Permission.IsNullOrWhiteSpace())
            {
                query = from user in query
                        join ur in _userRoleRepository.GetAll() on user.Id equals ur.UserId into urJoined
                        from ur in urJoined.DefaultIfEmpty()
                        join up in _userPermissionRepository.GetAll() on new { UserId = user.Id, Name = input.Permission } equals new { up.UserId, up.Name } into upJoined
                        from up in upJoined.DefaultIfEmpty()
                        join rp in _rolePermissionRepository.GetAll() on new { RoleId = ur == null ? 0 : ur.RoleId, Name = input.Permission } equals new { rp.RoleId, rp.Name } into rpJoined
                        from rp in rpJoined.DefaultIfEmpty()
                        where up != null && up.IsGranted || up == null && rp != null
                        group user by user
                        into userGrouped
                        select userGrouped.Key;
            }

            if (input.Deactivate != null)
            {
                query = query.WhereIf(input.Deactivate != null, x => x.Deactivate == input.Deactivate.Value);
            }
            else if (input.IsActive != null && input.IsActive.Value == true)
            {
                query = query.Where(t => t.Deactivate == false).WhereIf(input.IsActive != null, x => x.IsActive == input.IsActive.Value);
            }
            else if (input.IsActive != null && input.IsActive.Value == false)
            {
                query = query.Where(t => t.Deactivate == false).WhereIf(input.IsActive != null, x => x.IsActive == input.IsActive.Value);
            }



            var userCount = await query.CountAsync();
            var users = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            var userIds = users.Select(t => t.Id).ToHashSet();

            var userGroups = await _userGroupMemberRepository.GetAll()
                                     .Include(t => t.UserGroup.Location)
                                     .Where(t => userIds.Contains(t.MemberId))
                                     .AsNoTracking()
                                     .GroupBy(t=>t.MemberId)
                                     .Select(t => new
                                     {
                                         LoationNames = t.Select(dt=>dt != null && dt.UserGroup != null && dt.UserGroup.Location != null ? dt.UserGroup.Location.LocationName : "").ToList(),
                                         UserId = t.Key,
                                     })                                   
                                      .ToDictionaryAsync(d=>d.UserId,v=>v.LoationNames);

           

            var userListDtos = ObjectMapper.Map<List<UserListDto>>(users);

            var results = userListDtos.Select(b => new UserListDto
            {
                CreationTime = b.CreationTime,
                Deactivate = b.Deactivate,
                Id = b.Id,
                EmailAddress = b.EmailAddress,
                IsActive = b.IsActive,
                IsEmailConfirmed = b.IsEmailConfirmed,
                LastLoginTime = b.LastLoginTime,
                LocationName = userGroups.Where(t => t.Key == b.Id).Select(t => t.Value).FirstOrDefault(),
                Name = b.Name,
                PhoneNumber = b.PhoneNumber,
                ProfilePictureId = b.ProfilePictureId,
                Roles = b.Roles.Select(d => new UserListRoleDto
                {
                    RoleId = d.RoleId,
                    RoleName = "",
                }).ToList(),
                Surname = b.Surname,
                UserName = b.UserName
            }).ToList();

            await FillRoleNames(results);

            return new PagedResultDto<UserListDto>(
                userCount,
                results
                );
        }

        public async Task<FileDto> GetUsersToExcel()
        {
            var users = await UserManager.Users.ToListAsync();
            var userListDtos = ObjectMapper.Map<List<UserListDto>>(users);
            await FillRoleNames(userListDtos);

            return _userListExcelExporter.ExportToFile(userListDtos);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Create, AppPermissions.Pages_Administration_Users_Edit)]
        public async Task<GetUserForEditOutput> GetUserForEdit(NullableIdDto<long> input)
        {
            //Getting all available roles
            var userRoleDtos = await _roleManager.Roles
                .OrderBy(r => r.DisplayName)
                .Select(r => new UserRoleDto
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    RoleDisplayName = r.DisplayName
                })
                .ToArrayAsync();

            var allOrganizationUnits = await _organizationUnitRepository.GetAllListAsync();

            var output = new GetUserForEditOutput
            {
                Roles = userRoleDtos,
                AllOrganizationUnits = ObjectMapper.Map<List<OrganizationUnitDto>>(allOrganizationUnits),
                MemberedOrganizationUnits = new List<string>(),
                UserGroups = new List<Dto.UserGroupItems>(),
                User = new UserEditDto(),
                VendorTypeMembers = new List<VendorTypeMemberDto>(),
                CustomerTypeMembers = new List<CustomerTypeMemberDto>(),
            };

            if (!input.Id.HasValue)
            {
                //Creating a new user
                output.User = new UserEditDto
                {
                    IsActive = true,
                    ShouldChangePasswordOnNextLogin = true,
                    IsTwoFactorEnabled = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsEnabled),
                    IsLockoutEnabled = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.UserLockOut.IsEnabled)
                };

                foreach (var defaultRole in await _roleManager.Roles.Where(r => r.IsDefault).ToListAsync())
                {
                    var defaultUserRole = userRoleDtos.FirstOrDefault(ur => ur.RoleName == defaultRole.Name);
                    if (defaultUserRole != null)
                    {
                        defaultUserRole.IsAssigned = true;
                    }
                }
            }
            else
            {
                //Editing an existing user
                var user = await UserManager.GetUserByIdAsync(input.Id.Value);

                output.User = ObjectMapper.Map<UserEditDto>(user);
                output.ProfilePictureId = user.ProfilePictureId;

                foreach (var userRoleDto in userRoleDtos)
                {
                    userRoleDto.IsAssigned = await UserManager.IsInRoleAsync(user, userRoleDto.RoleName);
                }

                var organizationUnits = await UserManager.GetOrganizationUnitsAsync(user);
                output.MemberedOrganizationUnits = organizationUnits.Select(ou => ou.Code).ToList();

                output.UserGroups = await _userGroupMemberRepository.GetAll()
                                        .Include(t => t.UserGroup)
                                        .Where(t => t.MemberId == input.Id.Value)
                                        .AsNoTracking()
                                        .Select(t => new Dto.UserGroupItems
                                        {
                                            GroupId = t.UserGroupId,
                                            Id = t.Id,
                                            GroupName = t.UserGroup.Name,
                                            IsDefault = t.IsDefault,
                                            UserId = t.MemberId,
                                            LocationId = t.UserGroup.LocationId
                                        })
                                        .ToListAsync();

                output.VendorTypeMembers = await _vendorTypeMemeberRepository.GetAll().Include(s => s.VendorType).Where(s => s.MemberId == input.Id.Value).AsNoTracking()
                                                .Select(s => new VendorTypeMemberDto
                                                {
                                                    Id = s.Id,
                                                    MemberId = s.MemberId,
                                                    VendorTypeId = s.VendorTypeId,
                                                    VendorTypeName = s.VendorType.VendorTypeName,
                                                }).ToListAsync();

                output.IsSpecificVendorType = output.VendorTypeMembers != null && output.VendorTypeMembers.Any();

                output.CustomerTypeMembers = await _customerTypeMemeberRepository.GetAll()
                                                   .Include(s => s.CustomerType)
                                                   .Where(s => s.MemberId == input.Id.Value).AsNoTracking()
                                               .Select(s => new CustomerTypeMemberDto
                                               {
                                                   Id = s.Id,
                                                   MemberId = s.MemberId,
                                                   CustomerTypeId = s.CustomerTypeId,
                                                   CustomerTypeName = s.CustomerType.CustomerTypeName,
                                               }).ToListAsync();

                output.IsSpecificCustomerType = output.CustomerTypeMembers != null && output.CustomerTypeMembers.Any();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_ChangePermissions)]
        public async Task<GetUserPermissionsForEditOutput> GetUserPermissionsForEdit(EntityDto<long> input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            var permissions = PermissionManager.GetAllPermissions();
            var grantedPermissions = await UserManager.GetGrantedPermissionsAsync(user);

            return new GetUserPermissionsForEditOutput
            {
                Permissions = ObjectMapper.Map<List<FlatPermissionDto>>(permissions).OrderBy(p => p.DisplayName).ToList(),
                GrantedPermissionNames = grantedPermissions.Select(p => p.Name).ToList()
            };
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_ChangePermissions)]
        public async Task ResetUserSpecificPermissions(EntityDto<long> input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            await UserManager.ResetAllPermissionsAsync(user);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_ChangePermissions)]
        public async Task UpdateUserPermissions(UpdateUserPermissionsInput input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            var grantedPermissions = PermissionManager.GetPermissionsFromNamesByValidating(input.GrantedPermissionNames);
            await UserManager.SetGrantedPermissionsAsync(user, grantedPermissions);
        }

        public async Task CreateOrUpdateUser(CreateOrUpdateUserInput input)
        {
            if (input.User.Id.HasValue)
            {
                await UpdateUserAsync(input);
            }
            else
            {
                await CreateUserAsync(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Delete)]
        public async Task DeleteUser(EntityDto<long> input)
        {
            if (input.Id == AbpSession.GetUserId())
            {
                throw new UserFriendlyException(L("YouCanNotDeleteOwnAccount"));
            }

            var user = await UserManager.GetUserByIdAsync(input.Id);
            CheckErrors(await UserManager.DeleteAsync(user));
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Deactivate)]
        public async Task DeactivateUser(EntityDto<long> input)
        {

            if (input.Id == AbpSession.GetUserId())
            {
                throw new UserFriendlyException(L("YouCanNotDeactivateOwnAccount"));
            }

            var user = await UserManager.GetUserByIdAsync(input.Id);
            user.SetDeactivate();
            CheckErrors(await UserManager.UpdateAsync(user));
        }

        public async Task UnlockUser(EntityDto<long> input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            user.Unlock();
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Edit)]
        protected virtual async Task UpdateUserAsync(CreateOrUpdateUserInput input)
        {
            Debug.Assert(input.User.Id != null, "input.User.Id should be set.");

            if (input.IsSpecificVendorType)
            {
                if (input.VendorTypeMembers == null || !input.VendorTypeMembers.Any()) throw new UserFriendlyException(L("PleaseSelectOneOrMoreVendorType"));
                if (input.VendorTypeMembers.GroupBy(s => s.VendorTypeId).Any(s => s.Count() > 1)) throw new UserFriendlyException(L("DuplicateVendorType"));
            }

            if (input.IsSpecificCustomerType)
            {
                if (input.CustomerTypeMembers == null || !input.CustomerTypeMembers.Any()) throw new UserFriendlyException(L("PleaseSelectOneOrMoreCustomerType"));
                if (input.CustomerTypeMembers.GroupBy(s => s.CustomerTypeId).Any(s => s.Count() > 1)) throw new UserFriendlyException(L("Duplicated", L("CustomerType")));
            }

            var user = await UserManager.FindByIdAsync(input.User.Id.Value.ToString());
            user.ProfilePictureId = input.User.UserProfile;
            //Update user properties
            ObjectMapper.Map(input.User, user); //Passwords is not mapped (see mapping configuration)
            
            var userTempPass = "";
            //Set password
            if (input.SetRandomPassword)
            {
                userTempPass = User.CreateRandomPassword();
                user.Password = _passwordHasher.HashPassword(user, userTempPass);
            }
            else if (!input.User.Password.IsNullOrEmpty())
            {
                userTempPass = input.User.Password;
                await UserManager.InitializeOptionsAsync(AbpSession.TenantId);
                CheckErrors(await UserManager.ChangePasswordAsync(user, input.User.Password));
            }

            CheckErrors(await UserManager.UpdateAsync(user));
            // update user group 
            #region update user group 
            var userGroupItems = await _userGroupMemberRepository.GetAll().Where(u => u.MemberId == input.User.Id).ToListAsync();
            var toDeleteUserItems = userGroupItems.Where(u => !input.UserGroups.Any(i => i.Id != null && i.Id == u.Id)).ToList();

            foreach (var i in input.UserGroups)
            {
                if (i.Id == null)
                {
                    var @entityItem = UserGroupMember.CreateUserGroupMember(AbpSession.TenantId.Value, AbpSession.UserId, user.Id, i.GroupId);
                    entityItem.UpdateIsDefault(i.IsDefault);

                    CheckErrors(await _userGroupMemberManager.CreateAsync(@entityItem));
                }
                else
                {
                    var userItem = userGroupItems.FirstOrDefault(u => u.Id == i.Id);
                    if (userItem != null)
                    {
                        userItem.UpdateUserGroupMember(AbpSession.UserId.Value, user.Id);
                        userItem.UpdateIsDefault(i.IsDefault);
                        CheckErrors(await _userGroupMemberManager.UpdateAsync(userItem));
                    }
                }
            }

            foreach (var t in toDeleteUserItems)
            {
                CheckErrors(await _userGroupMemberManager.RemoveAsync(t));
            }
            #endregion


            #region Update Vendor Type Members
            var vendorTypeMembers = await _vendorTypeMemeberRepository.GetAll().Where(u => u.MemberId == input.User.Id).ToListAsync();
            var deleteVendorTypeMembers = vendorTypeMembers.Where(u => !input.VendorTypeMembers.Any(i => i.Id != 0 && i.Id == u.Id)).ToList();

            foreach (var i in input.VendorTypeMembers)
            {
                if (i.Id == 0)
                {
                    var member = VendorTypeMember.Create(AbpSession.TenantId.Value, AbpSession.UserId.Value, user.Id, i.VendorTypeId);
                    CheckErrors(await _vendorTypeMemeberManager.CreateAsync(member));
                }
                else
                {
                    var updateMemeber = vendorTypeMembers.FirstOrDefault(u => u.Id == i.Id);
                    if (updateMemeber != null)
                    {
                        updateMemeber.Update(AbpSession.UserId.Value, user.Id, i.VendorTypeId);
                        CheckErrors(await _vendorTypeMemeberManager.UpdateAsync(updateMemeber));
                    }
                }
            }

            foreach (var t in deleteVendorTypeMembers)
            {
                CheckErrors(await _vendorTypeMemeberManager.RemoveAsync(t));
            }

            #endregion

            #region Update Customer Type Members
            var customerTypeMembers = await _customerTypeMemeberRepository.GetAll().Where(u => u.MemberId == input.User.Id).ToListAsync();
            var deleteCustomerTypeMembers = customerTypeMembers.Where(u => !input.CustomerTypeMembers.Any(i => i.Id != 0 && i.Id == u.Id)).ToList();

            foreach (var i in input.CustomerTypeMembers)
            {
                if (i.Id == 0)
                {
                    var member = CustomerTypeMember.Create(AbpSession.TenantId.Value, AbpSession.UserId.Value, user.Id, i.CustomerTypeId);
                    CheckErrors(await _customerTypeMemeberManager.CreateAsync(member));
                }
                else
                {
                    var updateMemeber = customerTypeMembers.FirstOrDefault(u => u.Id == i.Id);
                    if (updateMemeber != null)
                    {
                        updateMemeber.Update(AbpSession.UserId.Value, user.Id, i.CustomerTypeId);
                        CheckErrors(await _customerTypeMemeberManager.UpdateAsync(updateMemeber));
                    }
                }
            }

            foreach (var t in deleteCustomerTypeMembers)
            {
                CheckErrors(await _customerTypeMemeberManager.RemoveAsync(t));
            }

            #endregion

            //Update roles
            CheckErrors(await UserManager.SetRoles(user, input.AssignedRoleNames));

            //update organization units
            await UserManager.SetOrganizationUnitsAsync(user, input.OrganizationUnits.ToArray());

            if (input.SendActivationEmail)
            {
                user.SetNewEmailConfirmationCode();
                await _userEmailer.SendEmailActivationLinkAsync(user, AppUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId), userTempPass);
            }
        }

        protected string AppName
        {
            get
            {
                var hostingEnvironment = IocManager.Instance.Resolve<IHostingEnvironment>();
                var appConfiguration = hostingEnvironment.GetAppConfiguration();
                return appConfiguration["App:Name"];
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Create)]
        protected virtual async Task CreateUserAsync(CreateOrUpdateUserInput input)
        {
            if (AbpSession.TenantId.HasValue)
            {
                await _userPolicy.CheckMaxUserCountAsync(AbpSession.GetTenantId());
            }

            if (input.IsSpecificVendorType)
            {
                if (input.VendorTypeMembers == null || !input.VendorTypeMembers.Any()) throw new UserFriendlyException(L("PleaseSelectOneOrMoreVendorType"));
                if (input.VendorTypeMembers.GroupBy(s => s.VendorTypeId).Any(s => s.Count() > 1)) throw new UserFriendlyException(L("DuplicateVendorType"));
            }

            if (input.IsSpecificCustomerType)
            {
                if (input.CustomerTypeMembers == null || !input.CustomerTypeMembers.Any()) throw new UserFriendlyException(L("PleaseSelectOneOrMoreCustomerType"));
                if (input.CustomerTypeMembers.GroupBy(s => s.CustomerTypeId).Any(s => s.Count() > 1)) throw new UserFriendlyException(L("Duplicated", L("CustomerType")));
            }

            var user = ObjectMapper.Map<User>(input.User); //Passwords is not mapped (see mapping configuration)
            user.ProfilePictureId = input.User.UserProfile;
            user.TenantId = AbpSession.TenantId;
            var userTempPass = "";
            //Set password
            if (input.SetRandomPassword)
            {
                userTempPass = User.CreateRandomPassword();
                user.Password = _passwordHasher.HashPassword(user, userTempPass);
            }
            else if (!input.User.Password.IsNullOrEmpty())
            {
                await UserManager.InitializeOptionsAsync(AbpSession.TenantId);
                foreach (var validator in _passwordValidators)
                {
                    CheckErrors(await validator.ValidateAsync(UserManager, user, input.User.Password));
                }
                user.Password = _passwordHasher.HashPassword(user, input.User.Password);
            }

            user.ShouldChangePasswordOnNextLogin = input.User.ShouldChangePasswordOnNextLogin;

            //Assign roles
            user.Roles = new Collection<UserRole>();
            foreach (var roleName in input.AssignedRoleNames)
            {
                var role = await _roleManager.GetRoleByNameAsync(roleName);
                user.Roles.Add(new UserRole(AbpSession.TenantId, user.Id, role.Id));
            }

            if (input.UseEmail == false)
            {
                input.User.EmailAddress = input.User.UserName.ToString() + User.DefaultNoEmailDomain;
            }

            CheckErrors(await UserManager.CreateAsync(user));

            //create group of user
            foreach (var u in input.UserGroups)
            {
                var @userItems = UserGroupMember.CreateUserGroupMember(AbpSession.TenantId.Value, AbpSession.UserId, user.Id, u.GroupId);
                userItems.UpdateIsDefault(u.IsDefault);
                CheckErrors(await _userGroupMemberManager.CreateAsync(@userItems));
            }

            foreach (var vm in input.VendorTypeMembers)
            {
                var member = VendorTypeMember.Create(AbpSession.TenantId.Value, AbpSession.UserId.Value, user.Id, vm.VendorTypeId);
                CheckErrors(await _vendorTypeMemeberManager.CreateAsync(member));
            }

            foreach (var vm in input.CustomerTypeMembers)
            {
                var member = CustomerTypeMember.Create(AbpSession.TenantId.Value, AbpSession.UserId.Value, user.Id, vm.CustomerTypeId);
                CheckErrors(await _customerTypeMemeberManager.CreateAsync(member));
            }

            await CurrentUnitOfWork.SaveChangesAsync(); //To get new user's Id.

            //Notifications
            await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(user.ToUserIdentifier());
            await _appNotifier.WelcomeToTheApplicationAsync(user);

            //Organization Units
            await UserManager.SetOrganizationUnitsAsync(user, input.OrganizationUnits.ToArray());

            //Send activation email
            if (input.SendActivationEmail)
            {
                user.SetNewEmailConfirmationCode();
                await _userEmailer.SendEmailActivationLinkAsync(
                    user,
                    AppUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId),
                    input.SetRandomPassword ? userTempPass : null
                );
            }
        }


        public async Task<ValidationCountOutput> CheckMaxUserCountAsync()
        {
            var maxUserCount = (await _featureChecker.GetValueAsync(AbpSession.TenantId.Value, AppFeatures.MaxUserCount)).To<int>();            
            var currentUserCount = await _userRepository.CountAsync(t => !t.Deactivate);
            return new ValidationCountOutput { MaxCount = maxUserCount, MaxCurrentCount = currentUserCount };
           
        }


        private async Task FillRoleNames(List<UserListDto> userListDtos)
        {
            /* This method is optimized to fill role names to given list. */

            var userRoles = await _userRoleRepository.GetAll()
                .Where(userRole => userListDtos.Any(user => user.Id == userRole.UserId))
                .Select(userRole => userRole).ToListAsync();

            var distinctRoleIds = userRoles.Select(userRole => userRole.RoleId).Distinct();

            foreach (var user in userListDtos)
            {
                var rolesOfUser = userRoles.Where(userRole => userRole.UserId == user.Id).ToList();
                user.Roles = ObjectMapper.Map<List<UserListRoleDto>>(rolesOfUser);
            }

            var roleNames = new Dictionary<int, string>();
            foreach (var roleId in distinctRoleIds)
            {
                roleNames[roleId] = (await _roleManager.GetRoleByIdAsync(roleId)).DisplayName;
            }

            foreach (var userListDto in userListDtos)
            {
                foreach (var userListRoleDto in userListDto.Roles)
                {
                    userListRoleDto.RoleName = roleNames[userListRoleDto.RoleId];
                }

                userListDto.Roles = userListDto.Roles.OrderBy(r => r.RoleName).ToList();
            }
        }
    }
}
