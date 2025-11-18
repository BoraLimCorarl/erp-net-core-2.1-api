using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Authorization;
using CorarlERP.QCTests;
using CorarlERP.TestParameters.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace CorarlERP.TestParameters
{
    public class TestParameterAppService : CorarlERPAppServiceBase, ITestParameterAppService
    {

        private readonly ITestParameterManager _testParameterManager;
        private readonly IRepository<TestParameter, long> _testParameterRepository;
        public TestParameterAppService(
            ITestParameterManager testParameterManager,
            IRepository<TestParameter, long> testParameterRepository)
        {
            _testParameterRepository = testParameterRepository;
            _testParameterManager = testParameterManager;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_TestParameters_Create)]
        public async Task<TestParameterDetailOutput> Create(CreateTestParameterInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = TestParameter.Create(tenantId, userId, input.Name, input.TestSource, input.LimitReferenceNote);

            CheckErrors(await _testParameterManager.CreateAsync(@entity));

            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<TestParameterDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_TestParameters_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            var @entity = await _testParameterManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _testParameterManager.RemoveAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_TestParameters_Disable)]
        public async Task Disable(EntityDto<long> input)
        {
            var @entity = await _testParameterManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.LastModifierUserId = AbpSession.GetUserId();
            entity.LastModificationTime = DateTime.Now;

            CheckErrors(await _testParameterManager.DisableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_TestParameters_Enable)]
        public async Task Enable(EntityDto<long> input)
        {
            var @entity = await _testParameterManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.LastModifierUserId = AbpSession.GetUserId();
            entity.LastModificationTime = DateTime.Now;

            CheckErrors(await _testParameterManager.EnableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_TestParameters_Find)]
        public async Task<PagedResultDto<TestParameterDetailOutput>> Find(GetTestParameterListInput input)
        {
            var @queryAll = _testParameterRepository
                             .GetAll()
                             .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                             .WhereIf(
                                 !input.Filter.IsNullOrEmpty(),
                                 p => p.Name.ToLower().Contains(input.Filter.ToLower()))
                             .AsNoTracking()
                             .Select(t => new TestParameterDetailOutput
                             {
                                 Id = t.Id,
                                 Name = t.Name,
                                 LimitReferenceNote = t.LimitReferenceNote,
                                 IsActive = t.IsActive
                             });

            var result = @queryAll;
            var resultCount = await result.CountAsync();
            var @entities = await result
                                .OrderBy(s => s.Name)
                                .PageBy(input)
                                .ToListAsync();
            return new PagedResultDto<TestParameterDetailOutput>(resultCount, ObjectMapper.Map<List<TestParameterDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_TestParameters_GetDetail)]
        public async Task<TestParameterDetailOutput> GetDetail(EntityDto<long> input)
        {
            var @entity = await _testParameterManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var result = ObjectMapper.Map<TestParameterDetailOutput>(entity);
           
            return ObjectMapper.Map<TestParameterDetailOutput>(result);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_TestParameters_GetList)]
        public async Task<PagedResultDto<TestParameterDetailOutput>> GetList(GetTestParameterListInput input)
        {
            var @query = _testParameterRepository
                        .GetAll()
                        .AsNoTracking()
                        .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                        .WhereIf(
                            !input.Filter.IsNullOrEmpty(),
                            p => p.Name.ToLower().Contains(input.Filter.ToLower()));
            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<TestParameterDetailOutput>(resultCount, ObjectMapper.Map<List<TestParameterDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_TestParameters_Update)]
        public async Task<TestParameterDetailOutput> Update(UpdateTestParameterInput input)
        {

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _testParameterManager.GetAsync(input.Id, true);
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.Update(userId, input.Name, input.TestSource, input.LimitReferenceNote);
            CheckErrors(await _testParameterManager.UpdateAsync(@entity));

            return ObjectMapper.Map<TestParameterDetailOutput>(@entity);
        }
    }
}
