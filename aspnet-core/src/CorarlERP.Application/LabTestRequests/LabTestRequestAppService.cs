using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using CorarlERP.Authorization;
using CorarlERP.LabTestRequests.Dto;
using CorarlERP.QCTests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace CorarlERP.LabTestRequests
{
    public class LabTestRequestAppService : CorarlERPAppServiceBase, ILabTestRequestAppService
    {

        private readonly ILabTestRequestManager _labTestRequestManager;
        private readonly IRepository<LabTestRequest, Guid> _labTestRequestRepository;
        public LabTestRequestAppService(
            ILabTestRequestManager labTestRequestManager,
            IRepository<LabTestRequest, Guid> labTestRequestRepository)
        {
            _labTestRequestRepository = labTestRequestRepository;
            _labTestRequestManager = labTestRequestManager;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_LabTestRequests_Create)]
        public async Task<LabTestRequestDetailOutput> Create(CreateLabTestRequestInput input)
        {

            if (input.QCSampleId == Guid.Empty)
            {
                throw new UserFriendlyException(L("IsRequired", L("SampleId")));
            }

            if (input.QCTestTemplateId <= 0)
            {
                throw new UserFriendlyException(L("IsRequired", L("QCTestTemplate")));
            }

            if(input.LabTestType == LabTestType.External) 
            {
                if(input.LabId == null || input.LabId == Guid.Empty)  throw new UserFriendlyException(L("IsRequired", L("LabName")));
            }
            else
            {
                input.LabId = null;
            }

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = LabTestRequest.Create(tenantId, userId, input.Date, input.QCTestTemplateId, input.QCSampleId, input.LabTestType, input.LabId, input.Remark);

            CheckErrors(await _labTestRequestManager.CreateAsync(@entity));

            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<LabTestRequestDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_LabTestRequests_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            var @entity = await _labTestRequestManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _labTestRequestManager.RemoveAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_LabTestRequests_GetDetail)]
        public async Task<LabTestRequestDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            var @entity = await _labTestRequestManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var result = ObjectMapper.Map<LabTestRequestDetailOutput>(entity);
            result.SampleId = entity.QCSample.SampleId;
            result.QCTestTemplateName = entity.QCTestTemplate.Name;
            result.LabName = entity.Lab?.VendorName;
            result.LabTestType = entity.Type;

            return ObjectMapper.Map<LabTestRequestDetailOutput>(result);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_LabTestRequests_Find)]
        public async Task<PagedResultDto<LabTestRequestDetailOutput>> Find(GetLabTestRequestListInput input)
        {
            var @query = _labTestRequestRepository
                        .GetAll()
                        .AsNoTracking()
                        .WhereIf(input.FromDate != null, s => input.FromDate.Value.Date <= s.Date.Date)
                        .WhereIf(input.ToDate != null, s => s.Date.Date <= input.ToDate.Value.Date)
                        .WhereIf(!input.QCTemplateIds.IsNullOrEmpty(), p => input.QCTemplateIds.Contains(p.QCTestTemplateId))
                        .WhereIf(!input.SampleIds.IsNullOrEmpty(), p => input.SampleIds.Contains(p.QCSampleId))
                        .WhereIf(!input.LabIds.IsNullOrEmpty(), p => input.LabIds.Contains(p.LabId.Value))
                        .WhereIf(input.LabTestType.HasValue, p => p.Type == input.LabTestType)
                        .WhereIf(!input.LabTestStatus.IsNullOrEmpty(), p => input.LabTestStatus.Contains(p.Status))
                        .WhereIf(!input.LocationIds.IsNullOrEmpty(), p => input.LocationIds.Contains(p.QCSample.LocationId))
                        .WhereIf(!input.ItemIds.IsNullOrEmpty(), p => input.ItemIds.Contains(p.QCSample.ItemId))
                        .WhereIf(!input.Filter.IsNullOrEmpty(), s =>
                            s.QCSample.SampleId.ToLower().Contains(input.Filter.ToLower()) ||
                            s.QCTestTemplate.Name.ToLower().Contains(input.Filter.ToLower()))
                        .Select(t => new LabTestRequestDetailOutput
                        {
                            Id = t.Id,
                            Date = t.Date,
                            QCTestTemplateId = t.QCTestTemplateId,
                            QCTestTemplateName = t.QCTestTemplate.Name,
                            QCSampleId = t.QCSampleId,
                            SampleId = t.QCSample.SampleId,
                            LabTestType = t.Type,
                            LabId = t.LabId,
                            LabName = t.Lab.VendorName,
                            Remark = t.Remark,
                            Status = t.Status
                        });
            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<LabTestRequestDetailOutput>(resultCount, @entities);
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_LabTestRequests_GetList)]
        [HttpPost]
        public async Task<PagedResultDto<LabTestRequestDetailOutput>> GetList(GetLabTestRequestListInput input)
        {
            var @query = _labTestRequestRepository
                        .GetAll()
                        .AsNoTracking()
                        .WhereIf(input.FromDate != null, s => input.FromDate.Value.Date <= s.Date.Date)
                        .WhereIf(input.ToDate != null, s =>  s.Date.Date <= input.ToDate.Value.Date)
                        .WhereIf(!input.QCTemplateIds.IsNullOrEmpty(), p => input.QCTemplateIds.Contains(p.QCTestTemplateId))
                        .WhereIf(!input.SampleIds.IsNullOrEmpty(), p => input.SampleIds.Contains(p.QCSampleId))
                        .WhereIf(!input.LabIds.IsNullOrEmpty(), p => input.LabIds.Contains(p.LabId.Value))
                        .WhereIf(input.LabTestType.HasValue, p => p.Type == input.LabTestType)
                        .WhereIf(!input.LabTestStatus.IsNullOrEmpty(), p => input.LabTestStatus.Contains(p.Status))
                        .WhereIf(!input.LocationIds.IsNullOrEmpty(), p => input.LocationIds.Contains(p.QCSample.LocationId))
                        .WhereIf(!input.ItemIds.IsNullOrEmpty(), p => input.ItemIds.Contains(p.QCSample.ItemId))
                        .WhereIf(!input.Filter.IsNullOrEmpty(), s => 
                            s.QCSample.SampleId.ToLower().Contains(input.Filter.ToLower()) ||
                            s.QCTestTemplate.Name.ToLower().Contains(input.Filter.ToLower()))
                        .Select(t => new LabTestRequestDetailOutput
                        {
                            Id = t.Id,
                            Date = t.Date,
                            QCTestTemplateId = t.QCTestTemplateId,
                            QCTestTemplateName = t.QCTestTemplate.Name,
                            QCSampleId = t.QCSampleId,
                            SampleId = t.QCSample.SampleId,
                            LabTestType = t.Type,
                            LabId = t.LabId,
                            LabName = t.Lab.VendorName,
                            Remark = t.Remark,
                            Status = t.Status
                        });
            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<LabTestRequestDetailOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_LabTestRequests_Update)]
        public async Task<LabTestRequestDetailOutput> Update(UpdateLabTestRequestInput input)
        {

            if (input.QCSampleId == Guid.Empty)
            {
                throw new UserFriendlyException(L("IsRequired", L("SampleId")));
            }

            if (input.QCTestTemplateId <= 0)
            {
                throw new UserFriendlyException(L("IsRequired", L("QCTestTemplate")));
            }

            if (input.LabTestType == LabTestType.External)
            {
                if (input.LabId == null || input.LabId == Guid.Empty) throw new UserFriendlyException(L("IsRequired", L("LabName")));
            }
            else
            {
                input.LabId = null;
            }

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _labTestRequestManager.GetAsync(input.Id, true);
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.Update(userId, input.Date, input.QCTestTemplateId, input.QCSampleId, input.LabTestType, input.LabId, input.Remark);
            CheckErrors(await _labTestRequestManager.UpdateAsync(@entity));

            return ObjectMapper.Map<LabTestRequestDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_LabTestRequests_ChangeStatus)]
        public async Task UpdateToPending(EntityDto<Guid> input)
        {
            var entity = await _labTestRequestManager.GetAsync(input.Id, true);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));

            if(entity.Status == LabTestStatus.Received) throw new UserFriendlyException(L("TestAlreadHasResult"));

            entity.UpdateStatus(LabTestStatus.Pending);
            entity.LastModificationTime = Clock.Now;
            entity.LastModifierUserId = AbpSession.GetUserId();

            await _labTestRequestManager.UpdateAsync(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_LabTestRequests_ChangeStatus)]
        public async Task UpdateToSent(EntityDto<Guid> input)
        {
            var entity = await _labTestRequestManager.GetAsync(input.Id, true);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));

            if (entity.Status == LabTestStatus.Received) throw new UserFriendlyException(L("TestAlreadHasResult"));

            entity.UpdateStatus(LabTestStatus.Sent);
            entity.LastModificationTime = Clock.Now;
            entity.LastModifierUserId = AbpSession.GetUserId();

            await _labTestRequestManager.UpdateAsync(entity);
        }
    }
}
