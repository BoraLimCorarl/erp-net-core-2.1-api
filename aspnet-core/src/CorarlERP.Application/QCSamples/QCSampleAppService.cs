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
using CorarlERP.QCSamples.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace CorarlERP.QCSamples
{
    public class QCSampleAppService : CorarlERPAppServiceBase, IQCSampleAppService
    {

        private readonly IQCSampleManager _qcSampleManager;
        private readonly IRepository<QCSample, Guid> _qcSampleRepository;
        public QCSampleAppService(
            IQCSampleManager qcSampleManager,
            IRepository<QCSample, Guid> qcSampleRepository)
        {
            _qcSampleRepository = qcSampleRepository;
            _qcSampleManager = qcSampleManager;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_QCSamples_Create)]
        public async Task<QCSampleDetailOutput> Create(CreateQCSampleInput input)
        {

            if (input.SampleId.IsNullOrEmpty())
            {
                throw new UserFriendlyException(L("IsRequired", L("SampleId")));
            }

            if (input.SourceDoc.IsNullOrEmpty())
            {
                throw new UserFriendlyException(L("IsRequired", L("SourceDoc")));
            }

            if (input.LocationId <= 0)
            {
                throw new UserFriendlyException(L("LocationIdIsRequired"));
            }

            if (input.ItemId == Guid.Empty)
            {
                throw new UserFriendlyException(L("ItemIdIsRequired"));
            }


            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = QCSample.Create(tenantId, userId, input.SampleId, input.SourceDoc, input.SampleDate, input.ItemId, input.LocationId, input.Remark);

            CheckErrors(await _qcSampleManager.CreateAsync(@entity));

            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<QCSampleDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_QCSamples_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            var @entity = await _qcSampleManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _qcSampleManager.RemoveAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_QCSamples_Find)]
        public async Task<PagedResultDto<QCSampleDetailOutput>> Find(GetQCSampleListInput input)
        {
            var @queryAll = _qcSampleRepository
                             .GetAll()
                             .WhereIf(input.FromDate != null, s => input.FromDate.Value.Date <= s.SampleDate.Date)
                             .WhereIf(input.ToDate != null, s => s.SampleDate.Date <= input.ToDate.Value.Date)
                             .WhereIf(!input.LocationIds.IsNullOrEmpty(), p => input.LocationIds.Contains(p.LocationId))
                             .WhereIf(!input.ItemIds.IsNullOrEmpty(), p => input.ItemIds.Contains(p.ItemId))
                             .WhereIf(
                                 !input.Filter.IsNullOrEmpty(),
                                 p => p.SampleId.ToLower().Contains(input.Filter.ToLower()))
                             .AsNoTracking()
                             .Select(t => new QCSampleDetailOutput
                             {
                                 Id = t.Id,
                                 SampleId = t.SampleId,
                                 SourceDoc = t.SourceDoc,
                                 SampleDate = t.SampleDate,
                                 ItemId = t.ItemId,
                                 ItemName = t.Item.ItemName,
                                 LocationId = t.LocationId,
                                 LocationName = t.Location.LocationName,
                                 Remark = t.Remark
                             });

            var result = @queryAll;
            var resultCount = await result.CountAsync();
            var @entities = await result
                                .OrderBy(s => s.SampleId)
                                .PageBy(input)
                                .ToListAsync();
            return new PagedResultDto<QCSampleDetailOutput>(resultCount, ObjectMapper.Map<List<QCSampleDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_QCSamples_GetDetail)]
        public async Task<QCSampleDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            var @entity = await _qcSampleManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var result = ObjectMapper.Map<QCSampleDetailOutput>(entity);

            return ObjectMapper.Map<QCSampleDetailOutput>(result);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_QCSamples_GetList)]
        public async Task<PagedResultDto<QCSampleDetailOutput>> GetList(GetQCSampleListInput input)
        {
            var @query = _qcSampleRepository
                        .GetAll()
                        .AsNoTracking()
                        .WhereIf(input.FromDate != null, s => input.FromDate.Value.Date <= s.SampleDate.Date)
                        .WhereIf(input.ToDate != null, s => s.SampleDate.Date <= input.ToDate.Value.Date)
                        .WhereIf(!input.LocationIds.IsNullOrEmpty(), p => input.LocationIds.Contains(p.LocationId))
                        .WhereIf(!input.ItemIds.IsNullOrEmpty(), p => input.ItemIds.Contains(p.ItemId))
                        .WhereIf(
                            !input.Filter.IsNullOrEmpty(),
                            p => p.SampleId.ToLower().Contains(input.Filter.ToLower()))
                        .Select(t => new QCSampleDetailOutput
                        {
                            Id = t.Id,
                            SampleId = t.SampleId,
                            SourceDoc = t.SourceDoc,
                            SampleDate = t.SampleDate,
                            ItemId = t.ItemId,
                            ItemName = t.Item.ItemName,
                            LocationId = t.LocationId,
                            LocationName = t.Location.LocationName,
                            Remark = t.Remark
                        });
            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<QCSampleDetailOutput>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_QCSamples_Update)]
        public async Task<QCSampleDetailOutput> Update(UpdateQCSampleInput input)
        {

            if (input.SampleId.IsNullOrEmpty())
            {
                throw new UserFriendlyException(L("IsRequired", L("SampleId")));
            }

            if (input.SourceDoc.IsNullOrEmpty())
            {
                throw new UserFriendlyException(L("IsRequired", L("SourceDoc")));
            }


            if (input.LocationId <= 0)
            {
                throw new UserFriendlyException(L("LocationIdIsRequired"));
            }

            if (input.ItemId == Guid.Empty)
            {
                throw new UserFriendlyException(L("ItemIdIsRequired"));
            }

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _qcSampleManager.GetAsync(input.Id, true);
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.Update(userId, input.SampleId, input.SourceDoc, input.SampleDate, input.ItemId, input.LocationId, input.Remark);
            CheckErrors(await _qcSampleManager.UpdateAsync(@entity));

            return ObjectMapper.Map<QCSampleDetailOutput>(@entity);
        }
    }
}
