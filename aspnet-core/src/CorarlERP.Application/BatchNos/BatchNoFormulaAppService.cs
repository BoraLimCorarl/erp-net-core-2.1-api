using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Authorization;
using CorarlERP.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore.Internal;
using Abp.Collections.Extensions;
using System.Linq;
using Abp.Linq.Extensions;
using Abp.Extensions;
using CorarlERP.BatchNos.Dto;

namespace CorarlERP.BatchNos
{
    public class BatchNoFormulaAppService : CorarlERPAppServiceBase, IBatchNoFormulaAppService
    {

        private readonly IRepository<BatchNoFormula, long> _batchNoFormulaRepository;
        public BatchNoFormulaAppService(
            IRepository<BatchNoFormula, long> batchNoFormulaRepository) 
        { 
            _batchNoFormulaRepository = batchNoFormulaRepository;
        }

        private async Task CheckDuplicate(CreateOrUpdateBatchNoFormulaInput input)
        {
            if (input.Name.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("PleaseEnter", L("Name")));

            var find = await _batchNoFormulaRepository.GetAll().AsNoTracking().AnyAsync(s => input.Id != s.Id && input.Name == s.Name);
            if (find) throw new UserFriendlyException(L("Duplicated", L("Name")));
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_BatchNoFormula_Create)]
        public async Task<BatchNoFormulaDetailOutput> Create(CreateOrUpdateBatchNoFormulaInput input)
        {
            await CheckDuplicate(input);

            var entity = BatchNoFormula.Create(AbpSession.UserId.Value, input.Name, input.FieldName, input.DateFormat, input.StandardPrePos, input.PrePosCode);
            await _batchNoFormulaRepository.InsertAsync(entity);

            return ObjectMapper.Map<BatchNoFormulaDetailOutput>(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_BatchNoFormula_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            var entity = await _batchNoFormulaRepository.GetAll().FirstAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));

            await _batchNoFormulaRepository.DeleteAsync(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_BatchNoFormula_Enable)]
        public async Task Enable(EntityDto<long> input)
        {
            var entity = await _batchNoFormulaRepository.GetAll().FirstAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));

            entity.Enable(AbpSession.UserId.Value, true);

            await _batchNoFormulaRepository.UpdateAsync(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_BatchNoFormula_Disable)]
        public async Task Disable(EntityDto<long> input)
        {
            var entity = await _batchNoFormulaRepository.GetAll().FirstAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));

            entity.Enable(AbpSession.UserId.Value, false);

            await _batchNoFormulaRepository.UpdateAsync(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_BatchNoFormula_Find)]
        public async Task<PagedResultDto<BatchNoFormulaDetailOutput>> Find(GetBatchNoFormulaListInput input)
        {
            var query = _batchNoFormulaRepository.GetAll()
                        .WhereIf(input.Users != null && input.Users.Any(), s => input.Users.Contains(s.CreatorUserId))
                        .WhereIf(!input.Filter.IsNullOrWhiteSpace(), s => 
                            s.Name.ToLower().Contains(input.Filter.ToLower()) ||
                            s.FieldName.ToLower().Contains(input.Filter.ToLower()) ||
                            s.DateFormat.ToLower().Contains(input.Filter.ToLower())
                        )
                        .AsNoTracking()
                        .Select(s => new BatchNoFormulaDetailOutput
                        {
                            Id = s.Id,
                            Name = s.Name,
                            FieldName= s.FieldName,
                            DateFormat= s.DateFormat,
                            PrePosCode= s.PrePosCode,
                            StandardPrePos = s.StandardPrePos,
                            IsActive = s.IsActive
                        });

            var count = await query.CountAsync();
            var items = new List<BatchNoFormulaDetailOutput>();
            if(count > 0)
            {
                items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            }

            return new PagedResultDto<BatchNoFormulaDetailOutput> { Items = items, TotalCount = count };
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_BatchNoFormula_GetDetail)]
        public async Task<BatchNoFormulaDetailOutput> GetDetail(EntityDto<long> input)
        {
            var entity = await _batchNoFormulaRepository.GetAll().FirstAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));

            var output = ObjectMapper.Map<BatchNoFormulaDetailOutput>(entity);
           
            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_BatchNoFormula_GetList)]
        public async Task<PagedResultDto<BatchNoFormulaDetailOutput>> GetList(GetBatchNoFormulaListInput input)
        {
            var query = _batchNoFormulaRepository.GetAll()
                        .WhereIf(input.Users != null && input.Users.Any(), s => input.Users.Contains(s.CreatorUserId))
                        .WhereIf(!input.Filter.IsNullOrWhiteSpace(), s =>
                            s.Name.ToLower().Contains(input.Filter.ToLower()) ||
                            s.FieldName.ToLower().Contains(input.Filter.ToLower()) ||
                            s.DateFormat.ToLower().Contains(input.Filter.ToLower())
                        )
                        .AsNoTracking()
                        .Select(s => new BatchNoFormulaDetailOutput
                        {
                            Id = s.Id,
                            Name = s.Name,
                            FieldName = s.FieldName,
                            DateFormat = s.DateFormat,
                            PrePosCode = s.PrePosCode,
                            StandardPrePos = s.StandardPrePos,
                            IsActive = s.IsActive,
                            UserId = s.CreatorUserId
                        });

            var count = await query.CountAsync();
            var items = new List<BatchNoFormulaDetailOutput>();
            if (count > 0)
            {
                items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            }

            return new PagedResultDto<BatchNoFormulaDetailOutput> { Items = items, TotalCount = count };
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_BatchNoFormula_Edit)]
        public async Task<BatchNoFormulaDetailOutput> Update(CreateOrUpdateBatchNoFormulaInput input)
        { 
            await CheckDuplicate(input);

            var entity = await _batchNoFormulaRepository.GetAll().FirstOrDefaultAsync(s => s.Id == input.Id);
            if(entity == null) throw new UserFriendlyException(L("RecordNotFound"));

            entity.Update(AbpSession.UserId.Value, input.Name, input.FieldName, input.DateFormat, input.StandardPrePos, input.PrePosCode);
            await _batchNoFormulaRepository.UpdateAsync(entity);

            return ObjectMapper.Map<BatchNoFormulaDetailOutput>(entity);
        }
    }
}
