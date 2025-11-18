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
using CorarlERP.QCTestTemplates.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace CorarlERP.QCTestTemplates
{
    public class QCTestTemplateAppService : CorarlERPAppServiceBase, IQCTestTemplateAppService
    {

        private readonly IQCTestTemplateManager _qcTestTemplateManager;
        private readonly IRepository<QCTestTemplate, long> _qcTestTemplateRepository;
        private readonly ICorarlRepository<QCTestTemplateParameter, long> _qcTestTemplateParameterRepository;
        public QCTestTemplateAppService(
            IQCTestTemplateManager qcTestTemplateManager,
            IRepository<QCTestTemplate, long> qcTestTemplateRepository,
            ICorarlRepository<QCTestTemplateParameter, long> qcTestTemplateParameterRepository)
        {  
            _qcTestTemplateManager = qcTestTemplateManager;
            _qcTestTemplateRepository = qcTestTemplateRepository;
            _qcTestTemplateParameterRepository = qcTestTemplateParameterRepository;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_QCTestTemplates_Create)]
        public async Task<QCTestTemplateDetailOutput> Create(CreateQCTestTemplateInput input)
        {
            if (input.DetailEntryRequired) {
                if(input.QCTestTemplateParameters.IsNullOrEmpty()) throw new UserFriendlyException(L("IsRequeired", L("QCTestTemplateParameter")));
                if(input.QCTestTemplateParameters.Any(r => r.LimitReferenceNoteOverride.IsNullOrEmpty())) throw new UserFriendlyException(L("IsRequeired", L("LimitReferenceNote")));
            }

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = QCTestTemplate.Create(tenantId, userId, input.Name, input.TestSource, input.DetailEntryRequired);

            CheckErrors(await _qcTestTemplateManager.CreateAsync(@entity));

            await CurrentUnitOfWork.SaveChangesAsync();

            if (input.DetailEntryRequired && !input.QCTestTemplateParameters.IsNullOrEmpty())
            {
                var addParameters = input.QCTestTemplateParameters
                    .Select(p => QCTestTemplateParameter.Create(tenantId, userId, @entity.Id, p.LimitReferenceNoteOverride)).ToList();

                await _qcTestTemplateParameterRepository.BulkInsertAsync(addParameters);
            }

            return ObjectMapper.Map<QCTestTemplateDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_QCTestTemplates_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            var @entity = await _qcTestTemplateManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var parameters = await _qcTestTemplateParameterRepository.GetAll().Where(p => p.QCTestTemplateId == @entity.Id).ToListAsync();
            if (parameters.Any()) await _qcTestTemplateParameterRepository.BulkDeleteAsync(parameters);

            CheckErrors(await _qcTestTemplateManager.RemoveAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_QCTestTemplates_Disable)]
        public async Task Disable(EntityDto<long> input)
        {
            var @entity = await _qcTestTemplateManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.LastModifierUserId = AbpSession.GetUserId();
            entity.LastModificationTime = DateTime.Now;

            CheckErrors(await _qcTestTemplateManager.DisableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_QCTestTemplates_Enable)]
        public async Task Enable(EntityDto<long> input)
        {
            var @entity = await _qcTestTemplateManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.LastModifierUserId = AbpSession.GetUserId();
            entity.LastModificationTime = DateTime.Now;

            CheckErrors(await _qcTestTemplateManager.EnableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_QCTestTemplates_Find)]
        public async Task<PagedResultDto<QCTestTemplateDetailOutput>> Find(GetQCTestTemplateListInput input)
        {
            var @queryAll = _qcTestTemplateRepository
                             .GetAll()
                             .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                             .WhereIf(
                                 !input.Filter.IsNullOrEmpty(),
                                 p => p.Name.ToLower().Contains(input.Filter.ToLower()))
                             .AsNoTracking()
                             .Select(t => new QCTestTemplateDetailOutput
                             {
                                 Id = t.Id,
                                 Name = t.Name,
                                 DetailEntryRequired = t.DetailEntryRequired,
                                 IsActive = t.IsActive
                             });

            var result = @queryAll;
            var resultCount = await result.CountAsync();
            var @entities = await result
                                .OrderBy(s => s.Name)
                                .PageBy(input)
                                .ToListAsync();
            return new PagedResultDto<QCTestTemplateDetailOutput>(resultCount, ObjectMapper.Map<List<QCTestTemplateDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_QCTestTemplates_GetDetail)]
        public async Task<QCTestTemplateDetailOutput> GetDetail(EntityDto<long> input)
        {
            var @entity = await _qcTestTemplateManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var result = ObjectMapper.Map<QCTestTemplateDetailOutput>(entity);

            result.QCTestTemplateParameters = await _qcTestTemplateParameterRepository
                .GetAll()
                .Where(p => p.QCTestTemplateId == input.Id)
                .AsNoTracking()
                .Select(p => new QCTestTemplateParameterInput
                {
                    Id = p.Id,
                    LimitReferenceNoteOverride = p.LimitReferenceNoteOverride
                })
                .ToListAsync();

            return ObjectMapper.Map<QCTestTemplateDetailOutput>(result);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_QCTestTemplates_GetList)]
        public async Task<PagedResultDto<QCTestTemplateDetailOutput>> GetList(GetQCTestTemplateListInput input)
        {
            var @query = _qcTestTemplateRepository
                        .GetAll()
                        .AsNoTracking()
                        .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                        .WhereIf(
                            !input.Filter.IsNullOrEmpty(),
                            p => p.Name.ToLower().Contains(input.Filter.ToLower()));
            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<QCTestTemplateDetailOutput>(resultCount, ObjectMapper.Map<List<QCTestTemplateDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Setup_QCTestTemplates_Update)]
        public async Task<QCTestTemplateDetailOutput> Update(UpdateQCTestTemplateInput input)
        {
            if (input.DetailEntryRequired)
            {
                if (input.QCTestTemplateParameters.IsNullOrEmpty()) throw new UserFriendlyException(L("IsRequeired", L("QCTestTemplateParameter")));
                if (input.QCTestTemplateParameters.Any(r => r.LimitReferenceNoteOverride.IsNullOrEmpty())) throw new UserFriendlyException(L("IsRequeired", L("LimitReferenceNote")));
            }

            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _qcTestTemplateManager.GetAsync(input.Id, true);
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.Update(userId, input.Name, input.TestSource, input.DetailEntryRequired);
            CheckErrors(await _qcTestTemplateManager.UpdateAsync(@entity));

            var parameters = await _qcTestTemplateParameterRepository
                .GetAll()
                .Where(p => p.QCTestTemplateId == @entity.Id)
                .ToListAsync();


            if (input.DetailEntryRequired)
            {
                var addParameters = input.QCTestTemplateParameters
                .Where(p => !p.Id.HasValue || p.Id == 0)
                .Select(p => QCTestTemplateParameter.Create(tenantId, userId, @entity.Id, p.LimitReferenceNoteOverride))
                .ToList();
                 
                if(addParameters.Any())
                {
                    await _qcTestTemplateParameterRepository.BulkInsertAsync(addParameters);
                }

                var updateList = input.QCTestTemplateParameters
                    .Where(p => p.Id.HasValue && p.Id > 0)
                    .ToList();

                var updateParameters = new List<QCTestTemplateParameter>();
                foreach (var update in updateList)
                {
                    var parameter = parameters.FirstOrDefault(p => p.Id == update.Id);
                    if (parameter != null)
                    {
                        parameter.Update(userId, update.LimitReferenceNoteOverride);
                        updateParameters.Add(parameter);
                    }
                }
                if(updateParameters.Any())
                {
                    await _qcTestTemplateParameterRepository.BulkUpdateAsync(updateParameters);
                }
            }



            var deleteParameters = parameters
                .WhereIf(!input.QCTestTemplateParameters.IsNullOrEmpty(), p => !input.QCTestTemplateParameters.Any(ip => ip.Id.HasValue && ip.Id == p.Id))
                .ToList();

            if (deleteParameters.Any()) await _qcTestTemplateParameterRepository.BulkDeleteAsync(deleteParameters);

            return ObjectMapper.Map<QCTestTemplateDetailOutput>(@entity);
        }
    }
}
