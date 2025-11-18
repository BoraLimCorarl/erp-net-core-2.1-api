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
using CorarlERP.AccountTypes;
using Abp.Collections.Extensions;

namespace CorarlERP.CashFlowTemplates
{
    [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate)]
    public class CashFlowAccountGroupAppService : CorarlERPAppServiceBase, ICashFlowAccountGroupAppService
    {
        private readonly ICorarlRepository<CashFlowAccountGroup, Guid> _cashFlowAccountGroupRepository;
        private readonly ICorarlRepository<CashFlowTemplateAccount, Guid> _cashFlowTemplateAccountRepository;
     
        public CashFlowAccountGroupAppService(
            ICorarlRepository<CashFlowTemplateAccount, Guid> cashFlowTemplateAccountRepository,
            ICorarlRepository<CashFlowAccountGroup, Guid> cashFlowAccountGroupRepository)
        {
            _cashFlowAccountGroupRepository = cashFlowAccountGroupRepository;
            _cashFlowTemplateAccountRepository = cashFlowTemplateAccountRepository;
        }

        private async Task CheckDuplidate(CashFlowAccountGroupDto input)
        {
            if (input.Name.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("IsRequired", L("Name")));

            var find = await _cashFlowAccountGroupRepository.GetAll().AsNoTracking().AnyAsync(s => s.Name == input.Name && s.Id != input.Id);
            if (find) throw new UserFriendlyException(L("Duplicated", L("CashFlowAccountGroup")));
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Create)]
        public async Task Create(CashFlowAccountGroupDto input)
        {
            await CheckDuplidate(input);
            
            var tenantId = AbpSession.TenantId.Value;
            var userId = AbpSession.UserId.Value;

            var entity = CashFlowAccountGroup.Create(tenantId, userId, input.Name, input.SortOrder, input.Description);
            await _cashFlowAccountGroupRepository.InsertAsync(entity); 
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate)]
        public async Task<PagedResultDto<CashFlowAccountGroupDto>> GetList(GetListCashFlowAccountGroupInput input)
        {
            var query = _cashFlowAccountGroupRepository.GetAll()
                        .AsNoTracking()
                        .WhereIf(!input.Filter.IsNullOrWhiteSpace(), s => s.Name.ToLower().Contains(input.Filter.ToLower()))
                        .Select(s => new CashFlowAccountGroupDto
                        {
                            Id = s.Id,
                            Name = s.Name,
                            SortOrder = s.SortOrder,
                            Description = s.Description,                        
                        });

            var count = await query.CountAsync();

            if (count == 0) return new PagedResultDto<CashFlowAccountGroupDto>(0, new List<CashFlowAccountGroupDto>());

            if (input.UsePagination)
            {
                query = query.OrderBy(input.Sorting).PageBy(input);
            }
            else
            {
                query = query.OrderBy(input.Sorting);
            }

            var result = await query.ToListAsync();

            return new PagedResultDto<CashFlowAccountGroupDto>(count, result);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Find)]
        public async Task<PagedResultDto<CashFlowAccountGroupDto>> Find(GetListCashFlowAccountGroupInput input)
        {
            var query = _cashFlowAccountGroupRepository.GetAll()
                        .AsNoTracking()
                        .WhereIf(!input.Filter.IsNullOrWhiteSpace(), s => s.Name.ToLower().Contains(input.Filter.ToLower()))
                        .Select(s => new CashFlowAccountGroupDto
                        {
                            Id = s.Id,
                            Name = s.Name,
                            SortOrder = s.SortOrder,
                            Description = s.Description,
                        });

            var count = await query.CountAsync();

            if (count == 0) return new PagedResultDto<CashFlowAccountGroupDto>(0, new List<CashFlowAccountGroupDto>());

            if (input.UsePagination)
            {
                query = query.OrderBy(input.Sorting).PageBy(input);
            }
            else
            {
                query = query.OrderBy(input.Sorting);
            }

            var result = await query.ToListAsync();

            return new PagedResultDto<CashFlowAccountGroupDto>(count, result);

        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate)]
        public  async Task<CashFlowAccountGroupDto> GetDetail(EntityDto<Guid> input)
        {
            var result = await _cashFlowAccountGroupRepository.GetAll()
                               .AsNoTracking()
                               .Select(s => new CashFlowAccountGroupDto
                               {
                                   Id = s.Id,
                                   Name = s.Name,
                                   Description = s.Description,
                                   SortOrder = s.SortOrder,
                               })
                               .FirstOrDefaultAsync(s => s.Id == input.Id);

            if(result == null) throw new UserFriendlyException("RecordNotFound");

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Update)]
        public async Task Update(CashFlowAccountGroupDto input)
        {
            await CheckDuplidate(input);

            var entity = await _cashFlowAccountGroupRepository.FirstOrDefaultAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException("RecordNotFound");

            var tenantId = AbpSession.TenantId.Value;
            var userId = AbpSession.UserId.Value;

            entity.Update(userId, input.Name, input.SortOrder, input.Description);

            await _cashFlowAccountGroupRepository.UpdateAsync(entity);   
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {

            var find = await _cashFlowTemplateAccountRepository.GetAll().AsNoTracking().AnyAsync(s => s.AccountGroupId == input.Id);
            if (find) throw new UserFriendlyException(L("CannotDeleteAccountGroupIsInUse"));

            var entity = await _cashFlowAccountGroupRepository.GetAll().FirstOrDefaultAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));

            await _cashFlowAccountGroupRepository.DeleteAsync(entity);
        }
        
    }
}
