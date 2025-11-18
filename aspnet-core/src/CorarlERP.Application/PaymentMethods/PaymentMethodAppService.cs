using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.PaymentMethods.Dto;
using Microsoft.EntityFrameworkCore;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using CorarlERP.ChartOfAccounts.Dto;
using Abp.Authorization;
using CorarlERP.Authorization;
using CorarlERP.Locations;
using Org.BouncyCastle.Asn1.Crmf;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using CorarlERP.Vendors.Dto;

namespace CorarlERP.PaymentMethods
{
    public class PaymentMethodAppService : CorarlERPAppServiceBase, IPaymentMethodAppService
    {
        private readonly IPaymentMethodManager _paymentMethodManager;
        private readonly IRepository<PaymentMethod, Guid> _paymentMethodRepository;
        private readonly IRepository<PaymentMethodBase, Guid> _paymentMethodBaseRepository;
        private readonly IRepository<Location, long> _locationRepository;

        private readonly ICorarlRepository<PaymentMethodUserGroup, Guid> _paymentMethodUserGroupRepository;

        public PaymentMethodAppService(
            IRepository<Location, long> locationRepository,
            IRepository<PaymentMethodBase, Guid> paymentMethodBaseRepository,
            ICorarlRepository<PaymentMethodUserGroup, Guid> paymentMethodUserGroupRepository,
            IPaymentMethodManager paymentMethodManager,
            IRepository<PaymentMethod, Guid> paymentMethodRepository)
        {
            _paymentMethodManager = paymentMethodManager;
            _paymentMethodRepository = paymentMethodRepository;
            _paymentMethodBaseRepository = paymentMethodBaseRepository;
            _locationRepository = locationRepository;
            _paymentMethodUserGroupRepository = paymentMethodUserGroupRepository;
        }

        private async Task CheckDuplicate(CreatePaymentMethodInput input)
        {
            var @query = from p in _paymentMethodRepository.GetAll()
                                   .Where(s => s.Id != input.Id)
                                   .Where(s => s.PaymentMethodId == input.PaymentMethodBase.Id)
                                   .AsNoTracking()
                         join g in _paymentMethodUserGroupRepository.GetAll()
                                   .WhereIf(input.Member == enumStatus.EnumStatus.Member.UserGroup &&
                                            input.UserGroups != null &&
                                            input.UserGroups.Any(),
                                            s => input.UserGroups.Any(g => g.UserGroupId == s.UserGroupId))
                                   .AsNoTracking()
                         on p.Id equals g.PaymentMethodId
                         into gs
                         where p.Member == enumStatus.EnumStatus.Member.All || gs.Any()
                         select p;
            var find = await query.FirstOrDefaultAsync();
            if (find != null) throw new UserFriendlyException(L("Duplicated", L("PaymentMethod")));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_PaymentMethods_Create)]
        public async Task<PaymentMethodDetailOutput> Create(CreatePaymentMethodInput input)
        {
            if (input.AccountId == null || input.AccountId == Guid.Empty) throw new UserFriendlyException(L("PleaseSelect", L("Account")));
            if (input.PaymentMethodBase == null || input.PaymentMethodBase.Id == Guid.Empty) throw new UserFriendlyException(L("PleaseSelect", L("PaymentMethod")));
            if (input.Member == enumStatus.EnumStatus.Member.UserGroup && (input.UserGroups == null || !input.UserGroups.Any())) throw new UserFriendlyException(L("PleaseEnter", L("UserGroups")));

            await CheckDuplicate(input);

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            PaymentMethod @entity;

            @entity = PaymentMethod.Create(tenantId, userId, input.PaymentMethodBase.Id, input.AccountId);
            entity.SetMember(input.Member);
            CheckErrors(await _paymentMethodManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            if (input.Member == enumStatus.EnumStatus.Member.UserGroup)
            {
                var groups = new List<PaymentMethodUserGroup>();
                foreach (var g in input.UserGroups)
                {
                    groups.Add(PaymentMethodUserGroup.Create(tenantId, userId, entity.Id, g.UserGroupId));
                }
                await _paymentMethodUserGroupRepository.BulkInsertAsync(groups);
            }

            return ObjectMapper.Map<PaymentMethodDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_PaymentMethods_Update)]
        public async Task<PaymentMethodDetailOutput> Update(CreatePaymentMethodInput input)
        {
            if (input.AccountId == null || input.AccountId == Guid.Empty) throw new UserFriendlyException(L("PleaseSelect", L("Account")));
            if (input.PaymentMethodBase == null || input.PaymentMethodBase.Id == Guid.Empty) throw new UserFriendlyException(L("PleaseSelect", L("PaymentMethod")));
            if (input.Member == enumStatus.EnumStatus.Member.UserGroup && (input.UserGroups == null || !input.UserGroups.Any())) throw new UserFriendlyException(L("PleaseEnter", L("UserGroups")));

            await CheckDuplicate(input);

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _paymentMethodManager.GetAsync(input.Id, true);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));

            entity.Update(userId, input.PaymentMethodBase.Id, input.AccountId);
            entity.SetMember(input.Member);
            CheckErrors(await _paymentMethodManager.UpdateAsync(@entity));

            var groups = await _paymentMethodUserGroupRepository.GetAll()
                               .Where(s => s.PaymentMethodId == input.Id)
                               .AsNoTracking()
                               .ToListAsync();

            if (input.Member == enumStatus.EnumStatus.Member.UserGroup)
            {
                var adds = input.UserGroups.Where(s => !groups.Any(g => g.UserGroupId == s.UserGroupId));
                var addGroups = new List<PaymentMethodUserGroup>();
                foreach (var g in adds)
                {
                    addGroups.Add(PaymentMethodUserGroup.Create(tenantId, userId, entity.Id, g.UserGroupId));
                }
                await _paymentMethodUserGroupRepository.BulkInsertAsync(addGroups);

                var deleteGroups = groups.Where(s => !input.UserGroups.Any(g => g.UserGroupId == s.UserGroupId));
                if (deleteGroups.Any()) await _paymentMethodUserGroupRepository.BulkDeleteAsync(deleteGroups);

            }
            else
            {
                if (groups.Any()) await _paymentMethodUserGroupRepository.BulkDeleteAsync(groups);
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<PaymentMethodDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_PaymentMethods_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            var @entity = await _paymentMethodManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var groups = await _paymentMethodUserGroupRepository.GetAll()
                               .Where(s => s.PaymentMethodId == input.Id)
                               .AsNoTracking()
                               .ToListAsync();

            if (groups.Any()) await _paymentMethodUserGroupRepository.BulkDeleteAsync(groups);

            CheckErrors(await _paymentMethodManager.RemoveAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_PaymentMethods_Disable)]
        public async Task Disable(EntityDto<Guid> input)
        {
            var @entity = await _paymentMethodManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _paymentMethodManager.DisableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_PaymentMethods_Enable)]
        public async Task Enable(EntityDto<Guid> input)
        {
            var @entity = await _paymentMethodManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _paymentMethodManager.EnableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_PaymentMethods_Find)]
        public async Task<PagedResultDto<PaymentMethodDetailOutput>> Find(GetPaymentMethodListInput input)
        {
            var @query = from p in _paymentMethodRepository.GetAll()
                                 .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value && p.PaymentMethodBase.IsActive == input.IsActive.Value)
                                 .WhereIf(!input.Filter.IsNullOrEmpty(), p => p.PaymentMethodBase.Name.ToLower().Contains(input.Filter.ToLower()))
                                 .AsNoTracking()
                         join g in _paymentMethodUserGroupRepository.GetAll()
                                   .AsNoTracking()
                         on p.Id equals g.PaymentMethodId
                         into gs
                         where input.LocationId == null || 
                               p.Member == enumStatus.EnumStatus.Member.All || 
                               gs.Any(l => l.UserGroup.LocationId == input.LocationId) 

                         select new PaymentMethodDetailOutput
                         {
                             Id = p.Id,
                             Name = p.PaymentMethodBase.Name,
                             Icon = p.PaymentMethodBase.Icon,
                             AccountId = p.AccountId,
                             Account = p.Account == null ? null : new ChartAccountSummaryOutput { 
                                Id = p.AccountId,
                                AccountName = p.Account.AccountName,
                                AccountCode = p.Account.AccountCode
                             },
                             Member = p.Member,
                             IsDefault = p.PaymentMethodBase.IsDefault,

                         };
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(s => s.Name)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<PaymentMethodDetailOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_PaymentMethods_Find)]
        public async Task<PagedResultDto<PaymentMethodBaseDetailOutput>> FindBase(GetPaymentMethodBaseListInput input)
        {
            var @query = _paymentMethodBaseRepository
                 .GetAll()
                 .AsNoTracking()
                 .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                 .WhereIf(!input.Filter.IsNullOrEmpty(), p => p.Name.ToLower().Contains(input.Filter.ToLower()))
                 .Select(s => new PaymentMethodBaseDetailOutput
                 {
                     Id = s.Id,
                     Name = s.Name,
                     Icon = s.Icon,
                     IsDefault = s.IsDefault,
                     IsActive = s.IsActive,
                 });
            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(s => s.Name).PageBy(input).ToListAsync();
            return new PagedResultDto<PaymentMethodBaseDetailOutput>(resultCount, @entities);
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_PaymentMethods_GetDetail)]
        public async Task<PaymentMethodDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            var @entity = await _paymentMethodManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var model = ObjectMapper.Map<PaymentMethodDetailOutput>(@entity);

            var groups = await _paymentMethodUserGroupRepository.GetAll()
                               .Where(s => s.PaymentMethodId == input.Id)
                               .AsNoTracking()
                               .Select(s => new GroupItems
                               {
                                   Id = s.Id,
                                   UserGroupId = s.UserGroupId,
                                   UserGroupName = s.UserGroup.Name,
                               })
                               .ToListAsync();

            model.UserGroups = groups;

            return model;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_PaymentMethods_GetList)]
        public async Task<PagedResultDto<PaymentMethodDetailOutput>> GetList(GetPaymentMethodListInput input)
        {
            
            var query = from p in _paymentMethodRepository.GetAll()
                                 .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value && p.PaymentMethodBase.IsActive == input.IsActive.Value)
                                 .WhereIf(!input.Filter.IsNullOrEmpty(), p => p.PaymentMethodBase.Name.ToLower().Contains(input.Filter.ToLower()))
                                 .AsNoTracking()
                        join g in _paymentMethodUserGroupRepository.GetAll()
                                  .AsNoTracking()
                                  .Select(s => new GroupItems
                                  {
                                      Id = s.PaymentMethodId,
                                      UserGroupId = s.UserGroupId,
                                      UserGroupName = s.UserGroup.Name
                                  })
                        on p.Id equals g.Id
                        into gs

                        select new PaymentMethodDetailOutput
                        {
                            Id = p.Id,
                            Name = p.PaymentMethodBase.Name,
                            Icon = p.PaymentMethodBase.Icon,
                            AccountId = p.AccountId,
                            Account = p.Account == null ? null : new ChartAccountSummaryOutput
                            {
                                Id = p.AccountId,
                                AccountName = p.Account.AccountName,
                                AccountCode = p.Account.AccountCode
                            },
                            Member = p.Member,
                            IsDefault = p.PaymentMethodBase.IsDefault,
                            UserGroups = gs == null ? new List<GroupItems>() : gs.ToList(),
                            IsActive = p.IsActive
                        };

            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<PaymentMethodDetailOutput>(resultCount, ObjectMapper.Map<List<PaymentMethodDetailOutput>>(@entities));
        }

    }

}
