using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CorarlERP.ProductionPlans.Dto;
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
using CorarlERP.AutoSequences;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.ProductionPlans;
using CorarlERP.ProductionLines.Dto;

namespace CorarlERP.ProductionLines
{
    [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Line, AppPermissions.Pages_Tenant_Production_Line_Find)]
    public class ProductionLineAppService : CorarlERPAppServiceBase, IProductionLineAppService
    {

        private readonly IRepository<ProductionLine, long> _productionLineRepository;
      
        public ProductionLineAppService(
            IRepository<ProductionLine, long> productionLineRepository) 
        { 
            _productionLineRepository = productionLineRepository;
        }

        private async Task CheckDuplicate(CreateOrUpdateProductionLineInput input)
        {
            if (input.ProductionLineName.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("PleaseEnter", L("ProductionLineName")));
            var find = await _productionLineRepository.GetAll().AsNoTracking().AnyAsync(s => input.Id != s.Id && input.ProductionLineName == s.ProductionLineName);
            if (find) throw new UserFriendlyException(L("Duplicated", L("ProductionLine")));
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Line_Create)]
        public async Task<ProductionLineDetailOutput> Create(CreateOrUpdateProductionLineInput input)
        {
            await CheckDuplicate(input);

            var entity = ProductionLine.Create(AbpSession.TenantId.Value, AbpSession.UserId.Value, input.ProductionLineName, input.Memo);
            await _productionLineRepository.InsertAsync(entity);

            return ObjectMapper.Map<ProductionLineDetailOutput>(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Line_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            var entity = await _productionLineRepository.GetAll().FirstAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));

            await _productionLineRepository.DeleteAsync(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Line_Enable)]
        public async Task Enable(EntityDto<long> input)
        {
            var entity = await _productionLineRepository.GetAll().FirstAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));

            entity.Enable(true);

            await _productionLineRepository.UpdateAsync(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Line_Disable)]
        public async Task Disable(EntityDto<long> input)
        {
            var entity = await _productionLineRepository.GetAll().FirstAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));

            entity.Enable(false);

            await _productionLineRepository.UpdateAsync(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Line_Find)]
        public async Task<PagedResultDto<ProductionLineDetailOutput>> Find(GetProductionLineListInput input)
        {
            var query = _productionLineRepository.GetAll()
                        .WhereIf(input.Users != null && input.Users.Any(), s => input.Users.Contains(s.CreatorUserId))
                        .WhereIf(input.IsActive.HasValue, s => s.IsActive == input.IsActive.Value)
                        .WhereIf(!input.Filter.IsNullOrWhiteSpace(), s => 
                            s.ProductionLineName.ToLower().Contains(input.Filter.ToLower())
                        )
                        .AsNoTracking()
                        .Select(s => new ProductionLineDetailOutput
                        {
                            Id = s.Id,
                            ProductionLineName = s.ProductionLineName,
                            IsActive = s.IsActive,
                            Memo = s.Memo,
                        });

            var count = await query.CountAsync();
            var items = new List<ProductionLineDetailOutput>();
            if(count > 0)
            {
                items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            }

            return new PagedResultDto<ProductionLineDetailOutput> { Items = items, TotalCount = count };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Line_GetDetail)]
        public async Task<ProductionLineDetailOutput> GetDetail(EntityDto<long> input)
        {
            var entity = await _productionLineRepository.GetAll().FirstAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));

            var output = ObjectMapper.Map<ProductionLineDetailOutput>(entity);
           
            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Line_GetList)]
        public async Task<PagedResultDto<ProductionLineDetailOutput>> GetList(GetProductionLineListInput input)
        {
            var query = _productionLineRepository.GetAll()
                        .WhereIf(input.Users != null && input.Users.Any(), s => input.Users.Contains(s.CreatorUserId))
                        .WhereIf(input.IsActive.HasValue, s => s.IsActive == input.IsActive.Value)
                        .WhereIf(!input.Filter.IsNullOrWhiteSpace(), s =>
                            s.ProductionLineName.ToLower().Contains(input.Filter.ToLower()) 
                        )
                        .AsNoTracking()
                        .Select(s => new ProductionLineDetailOutput
                        {
                            Id = s.Id,
                            ProductionLineName = s.ProductionLineName,
                            Memo = s.Memo,
                            IsActive = s.IsActive,
                        });

            var count = await query.CountAsync();
            var items = new List<ProductionLineDetailOutput>();
            if (count > 0)
            {
                items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            }

            return new PagedResultDto<ProductionLineDetailOutput> { Items = items, TotalCount = count };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Line_Update)]
        public async Task<ProductionLineDetailOutput> Update(CreateOrUpdateProductionLineInput input)
        {
            await CheckDuplicate(input);
           
            var entity = await _productionLineRepository.GetAll().FirstOrDefaultAsync(s => s.Id == input.Id);
            if(entity == null) throw new UserFriendlyException(L("RecordNotFound"));

            entity.Update(AbpSession.UserId.Value, input.ProductionLineName, input.Memo);
            await _productionLineRepository.UpdateAsync(entity);

            return ObjectMapper.Map<ProductionLineDetailOutput>(entity);
        }
    }
}
