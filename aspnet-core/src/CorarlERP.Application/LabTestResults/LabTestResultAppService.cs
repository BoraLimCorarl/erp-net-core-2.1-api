using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using CorarlERP.Authorization;
using CorarlERP.LabTestResults.Dto;
using CorarlERP.QCTests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace CorarlERP.LabTestResults
{
    public class LabTestResultAppService : CorarlERPAppServiceBase, ILabTestResultAppService
    {

        private readonly ILabTestResultManager _labTestResultManager;
        private readonly ICorarlRepository<LabTestResult, Guid> _labTestResultRepository;
        private readonly ICorarlRepository<LabTestResultDetail, Guid> _labTestResultDetailRepository;
        private readonly ICorarlRepository<LabTestRequest, Guid> _labTestRequestRepository;
        public LabTestResultAppService(
            ILabTestResultManager labTestResultManager,
            ICorarlRepository<LabTestResultDetail, Guid> labTestResultDetailRepository,
            ICorarlRepository<LabTestRequest, Guid> labTestRequestRepository,
            ICorarlRepository<LabTestResult, Guid> labTestResultRepository)
        {
            _labTestResultDetailRepository = labTestResultDetailRepository;
            _labTestRequestRepository = labTestRequestRepository;
            _labTestResultRepository = labTestResultRepository;
            _labTestResultManager = labTestResultManager;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_LabTestResults_Create)]
        public async Task<LabTestResultDetailOutput> Create(CreateLabTestResultInput input)
        {

            if (input.LabTestRequestId == Guid.Empty)
            {
                throw new UserFriendlyException(L("IsRequired", L("LabTestRequest")));
            }

            var test = await _labTestRequestRepository.GetAll().AsNoTracking().Include(s => s.QCTestTemplate).FirstOrDefaultAsync(p => p.Id == input.LabTestRequestId);
            if (test == null) throw new UserFriendlyException(L("IsNotValid", L("LabTestRequest")));

            if (test.Status == LabTestStatus.Received)
            {
                throw new UserFriendlyException(L("LabTestAlreadyHasResult"));
            }

            if (input.DetailEntry)
            {
                if(input.LabTestResultDetails.IsNullOrEmpty()) throw new UserFriendlyException(L("IsRequired", L("LabTestResultDetails")));

                var find = input.LabTestResultDetails.Any(s => s.TestParameterId <= 0);
                if (find) throw new UserFriendlyException(L("IsRequired", L("TestParameter")));

                var findAcutResult = input.LabTestResultDetails.Any(s => s.ActualValueNote.IsNullOrEmpty());
                if (findAcutResult) throw new UserFriendlyException(L("IsRequired", L("ActualResultNote")));

                input.FinalPassFail = input.LabTestResultDetails.All(s => s.PassFail);
            }
            else
            {
                input.LabTestResultDetails = new List<LabTestResultDetailInput>();
            }

            var findReferenceNo = await _labTestResultRepository.GetAll().AsNoTracking()
                .Where(s => s.ReferenceNo == input.ReferenceNo)
                .Where(s => (!s.LabId.HasValue && !test.LabId.HasValue) || s.LabId == test.LabId)
                .AnyAsync();

            if (findReferenceNo) throw new UserFriendlyException(L("DuplicateReferenceNo"));

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = LabTestResult.Create(tenantId, userId, input.ResultDate, input.ReferenceNo, input.LabTestRequestId, test.LabId, input.DetailEntry, input.FinalPassFail);

            CheckErrors(await _labTestResultManager.CreateAsync(@entity));

            await CurrentUnitOfWork.SaveChangesAsync();

            var labTestResultDetails = input.LabTestResultDetails.Select(s => LabTestResultDetail.Create(tenantId, userId, entity.Id, s.TestParameterId, s.LimitReferenceNote, s.ActualValueNote, s.PassFail)).ToList();
            await _labTestResultDetailRepository.BulkInsertAsync(labTestResultDetails);

            test.UpdateStatus(LabTestStatus.Received);
            test.LastModificationTime = Clock.Now;
            test.LastModifierUserId = userId;
            await _labTestRequestRepository.UpdateAsync(test);

            return ObjectMapper.Map<LabTestResultDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_LabTestResults_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            var @entity = await _labTestResultManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var details = await _labTestResultDetailRepository.GetAll().Where(s => s.LabTestResultId == @entity.Id).ToListAsync();
            if (details.Any())
            {
                await _labTestResultDetailRepository.BulkDeleteAsync(details);
            }

            entity.LabTestRequest.UpdateStatus(LabTestStatus.Sent);
            entity.LabTestRequest.LastModificationTime = Clock.Now;
            entity.LabTestRequest.LastModifierUserId = AbpSession.GetUserId();
            await _labTestRequestRepository.UpdateAsync(entity.LabTestRequest);

            CheckErrors(await _labTestResultManager.RemoveAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_LabTestResults_GetDetail)]
        public async Task<LabTestResultDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            var result = await _labTestResultRepository.GetAll().AsNoTracking().Where(s => s.Id == input.Id)
                .Select(s => new LabTestResultDetailOutput
                {
                    Id = s.Id,
                    ResultDate = s.ResultDate,
                    ReferenceNo = s.ReferenceNo,
                    DetailEntry = s.DetailEntry,
                    FinalPassFail = s.FinalPassFail,
                    QCTestTemplateName = s.LabTestRequest.QCTestTemplate.Name,
                    SampleId = s.LabTestRequest.QCSample.SampleId,
                    LabName = s.LabId.HasValue ? s.Lab.VendorName : "",
                    LabTestRequestId = s.LabTestRequestId
                })
                .FirstOrDefaultAsync();

            if (result == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            result.LabTestResultDetails = await _labTestResultDetailRepository
                .GetAll()
                .AsNoTracking()
                .Where(s => s.LabTestResultId == input.Id)
                .Select(s => new LabTestResultDetailInput
                {
                    Id = s.Id,
                    TestParameterId = s.TestParameterId,
                    TestParameterName = s.TestParameter.Name,
                    LimitReferenceNote = s.LimitReferenceNote,
                    ActualValueNote = s.ActualValueNote,
                    PassFail = s.PassFail
                })
                .ToListAsync();

            return ObjectMapper.Map<LabTestResultDetailOutput>(result);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_LabTestResults_GetList)]
        [HttpPost]
        public async Task<PagedResultDto<LabTestResultDetailOutput>> GetList(GetLabTestResultListInput input)
        {
            var @query = _labTestResultRepository
                        .GetAll()
                        .AsNoTracking()
                        .WhereIf(input.FromDate != null, s => input.FromDate.Value.Date <= s.ResultDate.Date)
                        .WhereIf(input.ToDate != null, s => s.ResultDate.Date <= input.ToDate.Value.Date)
                        .WhereIf(!input.QCTemplateIds.IsNullOrEmpty(), p => input.QCTemplateIds.Contains(p.LabTestRequest.QCTestTemplateId))
                        .WhereIf(!input.SampleIds.IsNullOrEmpty(), p => input.SampleIds.Contains(p.LabTestRequest.QCSampleId))
                        .WhereIf(!input.LabIds.IsNullOrEmpty(), p => input.LabIds.Contains(p.LabId.Value))
                        .WhereIf(input.LabTestType.HasValue, p => p.LabTestRequest.Type == input.LabTestType)
                        .WhereIf(input.PassFail.HasValue, p => p.FinalPassFail == input.PassFail)
                        .WhereIf(!input.LocationIds.IsNullOrEmpty(), p => input.LocationIds.Contains(p.LabTestRequest.QCSample.LocationId))
                        .WhereIf(!input.ItemIds.IsNullOrEmpty(), p => input.ItemIds.Contains(p.LabTestRequest.QCSample.ItemId))
                        .WhereIf(!input.Filter.IsNullOrEmpty(), s => 
                            s.ReferenceNo.ToLower().Contains(input.Filter.ToLower()) ||
                            s.LabTestRequest.QCSample.SampleId.ToLower().Contains(input.Filter.ToLower()) ||
                            s.LabTestRequest.QCTestTemplate.Name.ToLower().Contains(input.Filter.ToLower()))
                        .Select(t => new LabTestResultDetailOutput
                        {
                            Id = t.Id,
                            ResultDate = t.ResultDate,
                            ReferenceNo = t.ReferenceNo,
                            DetailEntry = t.DetailEntry,
                            QCTestTemplateName = t.LabTestRequest.QCTestTemplate.Name,
                            SampleId = t.LabTestRequest.QCSample.SampleId,
                            LabName = t.LabId.HasValue ? t.Lab.VendorName : "",
                            FinalPassFail = t.FinalPassFail,
                            LabTestRequestId = t.LabTestRequestId,
                        });
            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<LabTestResultDetailOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_LabTestResults_Update)]
        public async Task<LabTestResultDetailOutput> Update(UpdateLabTestResultInput input)
        {

            if (input.LabTestRequestId == Guid.Empty)
            {
                throw new UserFriendlyException(L("IsRequired", L("LabTestRequest")));
            }

            var test = await _labTestRequestRepository.GetAll().AsNoTracking().Include(s => s.QCTestTemplate).FirstOrDefaultAsync(p => p.Id == input.LabTestRequestId);
            if (test == null) throw new UserFriendlyException(L("IsNotValid", L("LabTestRequest")));

            if (input.DetailEntry)
            {
                if (input.LabTestResultDetails.IsNullOrEmpty()) throw new UserFriendlyException(L("IsRequired", L("LabTestResultDetails")));

                var find = input.LabTestResultDetails.Any(s => s.TestParameterId <= 0);
                if(find) throw new UserFriendlyException(L("IsRequired", L("TestParameter")));

                var findAcutResult = input.LabTestResultDetails.Any(s => s.ActualValueNote.IsNullOrEmpty());
                if (findAcutResult) throw new UserFriendlyException(L("IsRequired", L("ActualResultNote")));

                input.FinalPassFail = input.LabTestResultDetails.All(s => s.PassFail);
            }
            else
            {
                input.LabTestResultDetails = new List<LabTestResultDetailInput>();
            }

            var findReferenceNo = await _labTestResultRepository.GetAll().AsNoTracking()
               .Where(s => s.Id != input.Id)   
               .Where(s => s.ReferenceNo == input.ReferenceNo)
               .Where(s => (!s.LabId.HasValue && !test.LabId.HasValue) ||  s.LabId == test.LabId)
               .AnyAsync();

            if (findReferenceNo) throw new UserFriendlyException(L("DuplicateReferenceNo"));

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _labTestResultManager.GetAsync(input.Id, true);
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            if (entity.LabTestRequestId != input.LabTestRequestId && test.Status == LabTestStatus.Received)
            {
                throw new UserFriendlyException(L("LabTestAlreadyHasResult"));
            }

            entity.Update(userId, input.ResultDate, input.ReferenceNo, input.LabTestRequestId, test.LabId, input.DetailEntry, input.FinalPassFail);
            CheckErrors(await _labTestResultManager.UpdateAsync(@entity));

            var resultDetails = await _labTestResultDetailRepository.GetAll().AsNoTracking().Where(s => s.LabTestResultId == input.Id).ToListAsync();

            var addResultDetails = new List<LabTestResultDetail>();
            var updateResultDetails = new List<LabTestResultDetail>();
            foreach(var d in input.LabTestResultDetails)
            {
                var detailEntity = resultDetails.FirstOrDefault(s => s.Id == d.Id);

                if(detailEntity != null)
                {
                    detailEntity.Update(userId, d.TestParameterId, d.LimitReferenceNote, d.ActualValueNote, d.PassFail);
                    updateResultDetails.Add(detailEntity);
                }
                else
                {
                    detailEntity = LabTestResultDetail.Create(tenantId, userId, entity.Id, d.TestParameterId, d.LimitReferenceNote, d.ActualValueNote, d.PassFail);
                    addResultDetails.Add(detailEntity);
                }
            }

            if (addResultDetails.Any()) await _labTestResultDetailRepository.BulkInsertAsync(addResultDetails);
            if (updateResultDetails.Any()) await _labTestResultDetailRepository.BulkUpdateAsync(updateResultDetails);

            var deleteResultDetails = resultDetails.Where(s => !updateResultDetails.Any(r => r.Id == s.Id)).ToList();
            if(deleteResultDetails.Any()) await _labTestResultDetailRepository.BulkDeleteAsync(deleteResultDetails);

            test.UpdateStatus(LabTestStatus.Received);
            test.LastModificationTime = Clock.Now;
            test.LastModifierUserId = userId;
            await _labTestRequestRepository.BulkUpdateAsync(test);

            return ObjectMapper.Map<LabTestResultDetailOutput>(@entity);
        }
    }
}
