using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Uow;
using CorarlERP.Authorization;
using CorarlERP.FileStorages;
using CorarlERP.Items;
using CorarlERP.POSSettings.Dto;
using CorarlERP.Reports;
using CorarlERP.UserGroups;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.POSSettings
{
    [AbpAuthorize]
    public class POSSettingAppService : ReportBaseClass, IPOSSettingAppService
    {


        private readonly AppFolders _appFolders;
        private readonly IPOSSettingManager _iPOSSettingManager;
        private readonly ICorarlRepository<ItemsUserGroup, Guid> _itemUserGroupRepository;
        private readonly ICorarlRepository<UserGroupMember, Guid> _userGroupMemberRepository;
        private readonly IFileStorageManager _fileStorageManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICorarlRepository<POSSetting, long> _posSettingRepository;
        public POSSettingAppService(AppFolders appFolders,
                                      ICorarlRepository<ItemsUserGroup, Guid> itemUserGroupRepository,
                                      ICorarlRepository<UserGroupMember, Guid> userGroupMemberRepository,
                                      IFileStorageManager fileStorageManager,
                                      IUnitOfWorkManager unitOfWorkManager,
                                      IPOSSettingManager iPOSSettingManager,
                                      ICorarlRepository<POSSetting, long> posSettingRepository
           ) : base(null, appFolders, null, null)
        {
            _appFolders = appFolders;
            _itemUserGroupRepository = itemUserGroupRepository;
            _userGroupMemberRepository = userGroupMemberRepository;
            _fileStorageManager = fileStorageManager;
            _unitOfWorkManager = unitOfWorkManager;
            _iPOSSettingManager = iPOSSettingManager;
            _posSettingRepository = posSettingRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_POSSetting_CreateOrUpdate)]
        public async Task<long> CreateOrUpdate(CreateOrUpdatePOSSettingInput input)
        {
            var tenantId = AbpSession.TenantId.Value;
            var userId = AbpSession.UserId.Value;

            if (input.Id == null)
            {
                var entities = POSSetting.Create(tenantId, userId, input.AllowSelectCustomer, input.UseMemberCard);
                await _iPOSSettingManager.CreateAsync(entities);
                await CurrentUnitOfWork.SaveChangesAsync();
                return entities.Id;
            }
            else
            {
                var entities = await _posSettingRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
                entities.Update(userId, input.AllowSelectCustomer, input.UseMemberCard);
                return entities.Id;
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_POSSetting_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            var entities = await _posSettingRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            await _iPOSSettingManager.RemoveAsync(entities);
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_POSSetting_GetDetail)]
        public async Task<CreateOrUpdatePOSSettingInput> GetDetail()
        {
            var entities = await _posSettingRepository.GetAll().AsNoTracking().Select(p => new CreateOrUpdatePOSSettingInput
            {
                AllowSelectCustomer = p.AllowSelectCustomer,
                Id = p.Id,
                UseMemberCard = p.UseMemberCard
            }).FirstOrDefaultAsync();
            return entities;
        }
    }
}
