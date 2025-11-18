using Abp.Application.Services.Dto;
using CorarlERP.Authorization.Users;
using CorarlERP.UserGroups;
using CorarlERP.UserGroups.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CorarlERP.Tests.UserGroups
{
    public class UserGroupAppService_Test : AppTestBase
    {
        private readonly IUserGroupAppService _userGroupAppService;

        public UserGroupAppService_Test()
        {
            _userGroupAppService = Resolve<IUserGroupAppService>();
        }
        private CreateUserGroupInput CreateHelper()
        {
            var user = GetCurrentUser();
            var UserGroupExemptInput = new CreateUserGroupInput()
            {
                Name = "Mavel",
                UserGroupMember = new List<CreateOrUpdateUserGroupMemberInput>()
             {
                 new CreateOrUpdateUserGroupMemberInput()
                 {
                    MemberId  = user.Id
                 }
             }
            };
            return UserGroupExemptInput;
        }

        [Fact]
        public async Task Test_CreateSingleUserGroup()
        {
            var UserGroupExemptInput = CreateHelper();

            var result = await _userGroupAppService.Create(UserGroupExemptInput);
            result.Id.ShouldNotBeNull();

            CheckUserGroup(UserGroupExemptInput);
        }
        
        [Fact]
        public async Task Test_DelectUserGroup()
        {
            var userGroupExemptInput = CreateHelper();
            var result = await _userGroupAppService.Create(userGroupExemptInput);
            result.Id.ShouldNotBeNull();
            UsingDbContext(context =>
            {
                var UserGroupEntity = context.UserGroups.FirstOrDefault(p => p.Name == userGroupExemptInput.Name);
            });
            await _userGroupAppService.Delete(new EntityDto<Guid> { Id = result.Id.Value });
            UsingDbContext(context =>
            {
                context.UserGroups.Count().ShouldBe(0);
            });
        }

        [Fact]
        public async Task Test_GetListUserGroup()
        {
            var UserGroupExemptInput = CreateHelper();
            var result = await _userGroupAppService.Create(UserGroupExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _userGroupAppService.GetList(new GetListUserGroupInput()
            {
                Filter = ""
            });

            foreach (var v in output.Items)
            {
                v.Id.ShouldNotBeNull();
                v.Id.ShouldBe(result.Id.Value);
                v.Name.ShouldBe(UserGroupExemptInput.Name);               
            }
        }

        [Fact]
        public async Task Test_FindUserGroup()
        {
            var UserGroupExemptInput = CreateHelper();
            var result = await _userGroupAppService.Create(UserGroupExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _userGroupAppService.GetList(new GetListUserGroupInput()
            {
                Filter = ""
            });

            foreach (var v in output.Items)
            {
                v.Id.ShouldNotBeNull();
                v.Id.ShouldBe(result.Id.Value);
                v.Name.ShouldBe(UserGroupExemptInput.Name);
            }
        }
        [Fact]
        public async Task Test_UpdateUseGroup()
        {
            Guid vendorId = Guid.Empty;
            Guid ContactPersonId = Guid.Empty;
            var userId = GetCurrentUser();
            
            var CreateUserGroupExemptInput = CreateHelper();

            var result = await _userGroupAppService.Create(CreateUserGroupExemptInput);
            result.Id.ShouldNotBeNull();

            CheckUserGroup(CreateUserGroupExemptInput);

            UsingDbContext(context =>
            {
                var userGroup = context.UserGroupMembers.FirstOrDefault(u => u.MemberId == CreateUserGroupExemptInput.UserGroupMember[0].MemberId);
                userGroup.ShouldNotBeNull();
                ContactPersonId = userGroup.Id;
            });

            ContactPersonId.ShouldNotBe(Guid.Empty);

            //now you update the vendor whose name was: VendorName to UpdateVendorName
            var UpdateVendorExemptInput = new UpdateUserGroupInput()
            {
                Id = result.Id.Value,
                Name = "Iron Man Team",
                UserGroupMember = new List<CreateOrUpdateUserGroupMemberInput>()
                {
                  new   CreateOrUpdateUserGroupMemberInput(){ MemberId = userId.Id} 

                }
            };

            var updatedResult = await _userGroupAppService.Update(UpdateVendorExemptInput);
            updatedResult.ShouldNotBeNull();

            CheckUserGroup(UpdateVendorExemptInput, result.Id.Value);



        }

        [Fact]
        public async Task Test_GetDetailUserGroup()
        {

            var userGroupExemptInput = CreateHelper();
            var result = await _userGroupAppService.Create(userGroupExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _userGroupAppService.GetDetail(new EntityDto<Guid>()
            {

                Id = result.Id.Value,

            });
            output.Id.ShouldNotBeNull();
            output.Id.ShouldBe(result.Id.Value);

        }

        private void CheckUserGroup(CreateUserGroupInput UserGroupExemptInput, Guid? Id = null)
        {
            //Assert
            UsingDbContext(context =>
            {
                var UserGroupEntity = context.UserGroups.FirstOrDefault(p => p.Name == UserGroupExemptInput.Name);
                UserGroupEntity.ShouldNotBeNull();

                if (Id != null) UserGroupEntity.Id.ShouldBe(Id.Value);

                UserGroupEntity.Name.ShouldBe(UserGroupExemptInput.Name);                
                UserGroupEntity.IsActive.ShouldBe(true);

                var userGroupMembers = context.UserGroupMembers.Where(u => u.UserGroupId == UserGroupEntity.Id).OrderBy(u => u.MemberId).ToList();
                var inputuserGroupMembers = UserGroupExemptInput.UserGroupMember.OrderBy(u => u.MemberId).ToList();

                userGroupMembers.ShouldNotBeNull();
                userGroupMembers.Count().ShouldBe(inputuserGroupMembers.Count());

                for (var i = 0; i < userGroupMembers.Count(); i++)
                {
                    var userGroupMember = userGroupMembers[i];
                    var inputuserGroupMember = inputuserGroupMembers[i];
                    userGroupMember.MemberId.ShouldBe(inputuserGroupMember.MemberId);                  
                }
            });

        }
        
    }
}
