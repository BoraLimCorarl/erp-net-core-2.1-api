using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.UI;
using CorarlERP.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using System;
using System.IO;
using System.Text;
using Abp.Timing;
using Abp.Domain.Uow;
using CorarlERP.CashFlowTemplates.Dto;
using Abp.Extensions;
using CorarlERP.ChartOfAccounts;
using Abp.Collections.Extensions;

namespace CorarlERP.CashFlowTemplates
{
    [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate)]
    public class CashFlowCategoryAppService : CorarlERPAppServiceBase, ICashFlowCategoryAppService
    {
        private readonly ICorarlRepository<CashFlowCategory, Guid> _cashFlowCategoryRepository;
        private readonly ICorarlRepository<CashFlowTemplateCategory, Guid> _cashFlowTemplateCategoryRepository;

        public CashFlowCategoryAppService(
            ICorarlRepository<CashFlowTemplateCategory, Guid> cashFlowTemplateCategoryRepository,
            ICorarlRepository<CashFlowCategory, Guid> cashFlowCategoryRepository)
        {
            _cashFlowCategoryRepository = cashFlowCategoryRepository;
            _cashFlowTemplateCategoryRepository = cashFlowTemplateCategoryRepository;
        }

        private async Task CheckDuplidate(CashFlowCategoryDto input)
        {
            if (input.Name.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("IsRequired", L("Name")));

            var find = await _cashFlowCategoryRepository.GetAll().AsNoTracking().AnyAsync(s => s.Name == input.Name && s.Id != input.Id);
            if (find) throw new UserFriendlyException(L("Duplicated", L("CashFlowCategory")));
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Create)]
        public async Task Create(CashFlowCategoryDto input)
        {
            await CheckDuplidate(input);
            
            var tenantId = AbpSession.TenantId.Value;
            var userId = AbpSession.UserId.Value;

            var entity = CashFlowCategory.Create(tenantId, userId, input.Type, input.Name, input.SortOrder, input.Description);
            await _cashFlowCategoryRepository.InsertAsync(entity); 
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate)]
        public async Task<PagedResultDto<CashFlowCategoryDto>> GetList(GetListCashFlowCategoryInput input)
        {
            var query = _cashFlowCategoryRepository.GetAll()
                        .AsNoTracking()
                        .WhereIf(!input.Filter.IsNullOrWhiteSpace(), s => s.Name.ToLower().Contains(input.Filter.ToLower()))
                        .Select(s => new CashFlowCategoryDto
                        {
                            Id = s.Id,
                            Name = s.Name,
                            SortOrder = s.SortOrder,
                            Description = s.Description,    
                            Type = s.Type,
                            TypeName = s.Type.ToString(),
                            IsDefault = s.IsDefault
                        });

            var count = await query.CountAsync();

            if (count == 0) return new PagedResultDto<CashFlowCategoryDto>(0, new List<CashFlowCategoryDto>());

            if (input.UsePagination)
            {
                query = query.OrderBy(input.Sorting).PageBy(input);
            }
            else
            {
                query = query.OrderBy(input.Sorting);
            }

            var result = await query.ToListAsync();

            return new PagedResultDto<CashFlowCategoryDto>(count, result);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Find)]
        public async Task<PagedResultDto<CashFlowCategoryDto>> Find(GetListCashFlowCategoryInput input)
        {
            var query = _cashFlowCategoryRepository.GetAll()
                        .AsNoTracking()
                        .WhereIf(!input.Filter.IsNullOrWhiteSpace(), s => s.Name.ToLower().Contains(input.Filter.ToLower()))
                        .Select(s => new CashFlowCategoryDto
                        {
                            Id = s.Id,
                            Name = s.Name,
                            SortOrder = s.SortOrder,
                            Description = s.Description,
                            Type = s.Type,
                            TypeName = s.Type.ToString(),
                            IsDefault = s.IsDefault
                        });

            var count = await query.CountAsync();

            if (count == 0) return new PagedResultDto<CashFlowCategoryDto>(0, new List<CashFlowCategoryDto>());

            if (input.UsePagination)
            {
                query = query.OrderBy(input.Sorting).PageBy(input);
            }
            else
            {
                query = query.OrderBy(input.Sorting);
            }

            var result = await query.ToListAsync();

            return new PagedResultDto<CashFlowCategoryDto>(count, result);

        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate)]
        public  async Task<CashFlowCategoryDto> GetDetail(EntityDto<Guid> input)
        {
            var result = await _cashFlowCategoryRepository.GetAll()
                               .AsNoTracking()
                               .Select(s => new CashFlowCategoryDto
                               {
                                   Id = s.Id,
                                   Name = s.Name,
                                   Description = s.Description,
                                   SortOrder = s.SortOrder,
                                   Type = s.Type,
                                   TypeName = s.Type.ToString(),
                                   IsDefault = s.IsDefault
                               })
                               .FirstOrDefaultAsync(s => s.Id == input.Id);

            if(result == null) throw new UserFriendlyException("RecordNotFound");

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Update)]
        public async Task Update(CashFlowCategoryDto input)
        {
            await CheckDuplidate(input);

            var entity = await _cashFlowCategoryRepository.FirstOrDefaultAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException("RecordNotFound");

            var tenantId = AbpSession.TenantId.Value;
            var userId = AbpSession.UserId.Value;

            entity.Update(userId, input.Type, input.Name, input.SortOrder, input.Description);

            await _cashFlowCategoryRepository.UpdateAsync(entity);   
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            var entity = await _cashFlowCategoryRepository.GetAll().FirstOrDefaultAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));
            if (entity.IsDefault) throw new UserFriendlyException(L("CannotDeleteDefaultCategory"));

            var find = await _cashFlowTemplateCategoryRepository.GetAll().AsNoTracking().AnyAsync(s => s.CategoryId == input.Id);
            if (find) throw new UserFriendlyException(L("CannotDeleteCategoryIsInUse"));

            await _cashFlowCategoryRepository.DeleteAsync(entity);
        }
        
    }
}
