using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Authorization;
using CorarlERP.ProductionProcesses.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace CorarlERP.ProductionProcesses
{
    public class ProductionProcessAppService : CorarlERPAppServiceBase, IProductionProcessAppService
    {

        private readonly IProductionProcessManager _productionProcessManager;
        private readonly IRepository<ProductionProcess, long> _productionRepository;

        public ProductionProcessAppService(IProductionProcessManager ProductionProcessManager,
                             IRepository<ProductionProcess, long> ProductionProcessRepository)
        {
            _productionProcessManager = ProductionProcessManager;
            _productionRepository = ProductionProcessRepository;
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Process_Create)]
        public async Task<ProductionProcessDetailOutput> Create(CreateOrUpdateProductionProcessInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            var @entity = ProductionProcess.Create(tenantId, userId, input.ProcessName, input.AccountId, input.Memo, input.SortOrder, input.UseStandard, input.IsRequiredProductionPlan, input.ProductionProcessType);

            CheckErrors(await _productionProcessManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<ProductionProcessDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Process_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            var @entity = await _productionProcessManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _productionProcessManager.RemoveAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Process_UpdateStatusToDisable)]
        public async Task Disable(EntityDto<long> input)
        {
            var @entity = await _productionProcessManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _productionProcessManager.DisableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Process_UpdateStatusToEnable)]
        public async Task Enable(EntityDto<long> input)
        {
            var @entity = await _productionProcessManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _productionProcessManager.EnableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Process_Find)]
        public async Task<PagedResultDto<ProductionProcessDetailOutput>> Find(GetProductionProcessListInput input)
        {
            var accountQuery = GetAccountWithCode(new List<Guid>());

            var @query = from p in _productionRepository
                                    .GetAll()
                                    .AsNoTracking()
                                    .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                                    .WhereIf(
                                        !input.Filter.IsNullOrEmpty(),
                                        p => p.ProcessName.ToLower().Contains(input.Filter.ToLower()))
                         join a in accountQuery
                         on p.AccountId equals a.Id
                         select new ProductionProcessDetailOutput
                         {
                             Id = p.Id,
                             ProcessName = p.ProcessName,
                             AccountId = p.AccountId,
                             SortOrder = p.SortOrder,
                             Account = new ChartOfAccounts.Dto.ChartAccountSummaryOutput
                             {
                                 Id = a.Id,
                                 AccountName = a.AccountName,
                                 AccountCode = a.AccountCode
                             },
                             UseStandard = p.UseStandard,
                             IsRequiredProductionPlan = p.IsRequiredProductionPlan,
                             ProductionProcessType = p.ProductionProcessType
                         };
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(s => s.SortOrder)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<ProductionProcessDetailOutput>(resultCount, entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Process_Find)]
        public async Task<PagedResultDto<ProductionProcessDetailOutput>> FindOld(GetProductionProcessListInput input)
        {
            var @query = _productionRepository
                 .GetAll()
                 .Include(u => u.Account)
                 .AsNoTracking()
                 .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                 .WhereIf(
                     !input.Filter.IsNullOrEmpty(),
                     p => p.ProcessName.ToLower().Contains(input.Filter.ToLower()));
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<ProductionProcessDetailOutput>(resultCount, ObjectMapper.Map<List<ProductionProcessDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Process_GetDetail)]
        public async Task<ProductionProcessDetailOutput> GetDetail(EntityDto<long> input)
        {
            var @entity = await _productionProcessManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            return ObjectMapper.Map<ProductionProcessDetailOutput>(@entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Process_GetList)]
        public async Task<PagedResultDto<ProductionProcessDetailOutput>> GetList(GetProductionProcessListInput input)
        {
            var accountQuery = GetAccountWithCode(new List<Guid>());

            var @query = from p in _productionRepository
                                    .GetAll()
                                    .AsNoTracking()
                                    .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                                    .WhereIf(
                                        !input.Filter.IsNullOrEmpty(),
                                        p => p.ProcessName.ToLower().Contains(input.Filter.ToLower()))
                         join a in accountQuery
                         on p.AccountId equals a.Id
                         select new ProductionProcessDetailOutput
                         {
                             Id = p.Id,
                             ProcessName = p.ProcessName,
                             SortOrder = p.SortOrder,
                             IsActive = p.IsActive,
                             Memo = p.Memo,
                             AccountId = p.AccountId,
                             Account = new ChartOfAccounts.Dto.ChartAccountSummaryOutput
                             {
                                 Id = a.Id,
                                 AccountName = a.AccountName,
                                 AccountCode = a.AccountCode
                             },
                             UseStandard = p.UseStandard,
                             IsRequiredProductionPlan = p.IsRequiredProductionPlan,
                             ProductionProcessType = p.ProductionProcessType
                         };

            var resultCount = await query.CountAsync();

            if (input.Sorting.EndsWith("DESC"))
            {
                if (input.Sorting.ToLower().StartsWith("processname"))
                {
                    query = query.OrderByDescending(s => s.ProcessName);
                } 
                else if (input.Sorting.ToLower().StartsWith("memo"))
                {
                    query = query.OrderByDescending(s => s.Memo);
                } 
                else if (input.Sorting.ToLower().StartsWith("isactive"))
                {
                    query = query.OrderByDescending(s => s.IsActive);
                }
                else if (input.Sorting.ToLower().StartsWith("sortorder"))
                {
                    query = query.OrderByDescending(s => s.SortOrder);
                }
                else
                {
                    query = query.OrderBy(input.Sorting);
                }
            }
            else
            {
                if (input.Sorting.ToLower().StartsWith("processname"))
                {
                    query = query.OrderBy(s => s.ProcessName);
                }
                else if (input.Sorting.ToLower().StartsWith("memo"))
                {
                    query = query.OrderBy(s => s.Memo);
                }
                else if (input.Sorting.ToLower().StartsWith("isactive"))
                {
                    query = query.OrderBy(s => s.IsActive);
                }
                else if (input.Sorting.ToLower().StartsWith("sortorder"))
                {
                    query = query.OrderBy(s => s.SortOrder);
                }
                else
                {
                    query = query.OrderBy(input.Sorting);
                }
            }

            var @entities = await query.PageBy(input).ToListAsync();
            return new PagedResultDto<ProductionProcessDetailOutput>(resultCount, entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Process_GetList)]
        public async Task<PagedResultDto<ProductionProcessDetailOutput>> GetListOld(GetProductionProcessListInput input)
        {
            var @query = _productionRepository
                .GetAll()
                .Include(u => u.Account)
                .AsNoTracking()
                .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                .WhereIf(
                    !input.Filter.IsNullOrEmpty(),
                    p => p.ProcessName.ToLower().Contains(input.Filter.ToLower()));
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<ProductionProcessDetailOutput>(resultCount, ObjectMapper.Map<List<ProductionProcessDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Process_Update)]
        public async Task<ProductionProcessDetailOutput> Update(CreateOrUpdateProductionProcessInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _productionProcessManager.GetAsync(input.Id.Value, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.Update(userId, input.ProcessName, input.AccountId, input.Memo, input.SortOrder, input.UseStandard, input.IsRequiredProductionPlan, input.ProductionProcessType);

            CheckErrors(await _productionProcessManager.UpdateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<ProductionProcessDetailOutput>(@entity);
        }
    }
}
