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
using CorarlERP.ProductionProcesses;
using CorarlERP.Productions;
using CorarlERP.Items;
using CorarlERP.Migrations;
using CorarlERP.Productions.Dto;
using Telegram.Bot.Requests.Abstractions;

namespace CorarlERP.ProductionPlans
{
    [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Plan, AppPermissions.Pages_Tenant_Production_Plan_Find)]
    public class ProductionPlanAppService : CorarlERPAppServiceBase, IProductionPlanAppService
    {

        private readonly ICorarlRepository<ProductionStandardCost, Guid> _productionStandardCostRepository;
        private readonly ICorarlRepository<ProductionIssueStandardCost, Guid> _productionIssueStandardCostRepository;
        private readonly IRepository<RawMaterialItems, Guid> _rawMaterialItemsRepository;
        private readonly IRepository<FinishItems, Guid> _finishItemsRepository;
        private readonly IRepository<Item, Guid> _itemRepository;
        private readonly ICorarlRepository<ProductionPlan, Guid> _productionPlanRepository;
        private readonly IProductionPlanManager _productionPlanManager;
        private readonly IAutoSequenceManager _autoSequenceManager;
        public ProductionPlanAppService(
            ICorarlRepository<ProductionStandardCost, Guid> productionStandardCostRepository,
            ICorarlRepository<ProductionIssueStandardCost, Guid> productionIssueStandardCostRepository,
            IRepository<RawMaterialItems, Guid> rawMaterialItemsRepository,
            IRepository<FinishItems, Guid> finishItemsRepository,
            IRepository<Item, Guid> itemRepository,
            IAutoSequenceManager autoSequenceManager,
            IProductionPlanManager productionPlanManager,
            ICorarlRepository<ProductionPlan, Guid> productionPlanRepository) 
        { 
            _autoSequenceManager = autoSequenceManager;
            _productionPlanRepository = productionPlanRepository;
            _productionPlanManager = productionPlanManager;
            _itemRepository = itemRepository;
            _rawMaterialItemsRepository = rawMaterialItemsRepository;
            _finishItemsRepository = finishItemsRepository;
            _productionStandardCostRepository = productionStandardCostRepository;
            _productionIssueStandardCostRepository = productionIssueStandardCostRepository;
        }

        private async Task CheckDuplicate(CreateOrUpdateProductionPlanInput input)
        {
            if (input.DocumentNo.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("PleaseEnter", L("DocumentNo")));

            var find = await _productionPlanRepository.GetAll().AsNoTracking().AnyAsync(s => input.Id != s.Id && input.DocumentNo == s.DocumentNo);
            if (find) throw new UserFriendlyException(L("Duplicated", L("DocumentNo")));
        }

        private async Task CheckDuplicateReference(CreateOrUpdateProductionPlanInput input)
        {
            var find = await _productionPlanRepository.GetAll().AsNoTracking().AnyAsync(s => input.Id != s.Id && input.Reference == s.Reference);
            if (find) throw new UserFriendlyException(L("Duplicated", L("Reference")));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Plan_Create)]
        public async Task<ProductionPlanDetailOutput> Create(CreateOrUpdateProductionPlanInput input)
        {
            #region Update Sequence
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ProductionPlan);

            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.DocumentNo = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }
            #endregion

            await CheckDuplicate(input);
            if(auto.RequireReference) await CheckDuplicateReference(input);


            var entity = ProductionPlan.Create(AbpSession.TenantId.Value, AbpSession.UserId.Value, input.LocationId, input.DocumentNo, input.Reference, input.StartDate, input.EndDate, input.Description, input.ProductionLineId, input.ProductionProcess);
            await _productionPlanRepository.InsertAsync(entity);

            return ObjectMapper.Map<ProductionPlanDetailOutput>(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Plan_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            var entity = await _productionPlanRepository.GetAll().FirstAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ProductionPlan);

            if (entity.DocumentNo == auto.LastAutoSequenceNumber)
            {
                var find = await _productionPlanRepository.GetAll().Where(t => t.Id != input.Id)
                    .OrderByDescending(t => t.CreationTime).FirstOrDefaultAsync();
                if (find != null)
                {
                    auto.UpdateLastAutoSequenceNumber(find.DocumentNo);
                }
                else
                {
                    auto.UpdateLastAutoSequenceNumber(null);
                }
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }

            await _productionPlanRepository.DeleteAsync(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Plan_Close)]
        public async Task Close(EntityDto<Guid> input)
        {
            var entity = await _productionPlanRepository.GetAll().FirstAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));

            entity.Close(AbpSession.UserId.Value);

            await _productionPlanRepository.UpdateAsync(entity);

            await CurrentUnitOfWork.SaveChangesAsync(); 
            await _productionPlanManager.CalculateByIdAsync(AbpSession.UserId.Value, new List<Guid> { input.Id });
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Plan_Open)]
        public async Task Open(EntityDto<Guid> input)
        {
            var entity = await _productionPlanRepository.GetAll().FirstAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));

            entity.Open(AbpSession.UserId.Value);

            await _productionPlanRepository.UpdateAsync(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Plan_Find)]
        public async Task<PagedResultDto<ProductionPlanDetailOutput>> Find(GetProductionPlanListInput input)
        {
            var query = _productionPlanRepository.GetAll()
                        .WhereIf(input.FromDate.HasValue, s => input.FromDate <= s.StartDate)
                        .WhereIf(input.ToDate.HasValue, s => s.StartDate.Value.Date <= input.ToDate)
                        .WhereIf(input.PlanStatuses != null && input.PlanStatuses.Any(), s => input.PlanStatuses.Contains(s.Status))                        
                        .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.LocationId))
                        .WhereIf(input.Users != null && input.Users.Any(), s => input.Users.Contains(s.CreatorUserId))
                        .WhereIf(input.ProductionLines != null &&  input.ProductionLines.Any(), s => input.ProductionLines.Contains(s.ProductionLineId.Value))
                        .WhereIf(!input.Filter.IsNullOrWhiteSpace(), s => 
                            s.DocumentNo.ToLower().Contains(input.Filter.ToLower()) ||
                            s.Reference.ToLower().Contains(input.Filter.ToLower()) ||
                            (s.ProductionLineId.HasValue && s.ProductionLine.ProductionLineName.ToLower().Contains(input.Filter.ToLower()))
                        )
                        .AsNoTracking()
                        .Select(s => new ProductionPlanDetailOutput
                        {
                            Id = s.Id,
                            Reference= s.Reference,
                            DocumentNo= s.DocumentNo,
                            StartDate= s.StartDate,
                            EndDate= s.EndDate,
                            Description= s.Description,
                            LocationId= s.LocationId,
                            LocationName = s.Location.LocationName,
                            Status = s.Status
                        });

            var count = await query.CountAsync();
            var items = new List<ProductionPlanDetailOutput>();
            if(count > 0)
            {
                items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            }

            return new PagedResultDto<ProductionPlanDetailOutput> { Items = items, TotalCount = count };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Plan_GetDetail)]
        public async Task<ProductionPlanDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            var entity = await _productionPlanRepository.GetAll()
                               .Include(s => s.Location)
                               .Include(s => s.ProductionLine)
                               .FirstAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));

            var output = ObjectMapper.Map<ProductionPlanDetailOutput>(entity);
            output.LocationName = entity.Location.LocationName;
            output.ProductionLineName = entity.ProductionLine?.ProductionLineName;
           
            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Plan_GetList)]
        public async Task<PageResultProductioinPlanSummary> GetList(GetProductionPlanListInput input)
        {
            var query = from s in _productionPlanRepository.GetAll()
                                .WhereIf(input.FromDate.HasValue, s => input.FromDate <= s.StartDate)
                                .WhereIf(input.ToDate.HasValue, s => s.StartDate.Value.Date <= input.ToDate)
                                .WhereIf(input.PlanStatuses != null && input.PlanStatuses.Any(), s => input.PlanStatuses.Contains(s.Status))
                                .WhereIf(input.Locations != null && input.Locations.Any(), s => input.Locations.Contains(s.LocationId))
                                .WhereIf(input.Users != null && input.Users.Any(), s => input.Users.Contains(s.CreatorUserId))
                                .WhereIf(input.ProductionLines != null &&  input.ProductionLines.Any(), s => input.ProductionLines.Contains(s.ProductionLineId.Value))
                                .WhereIf(!input.Filter.IsNullOrWhiteSpace(), s =>
                                    s.DocumentNo.ToLower().Contains(input.Filter.ToLower()) ||
                                    s.Reference.ToLower().Contains(input.Filter.ToLower()) ||
                                    (s.ProductionLineId.HasValue && s.ProductionLine.ProductionLineName.ToLower().Contains(input.Filter.ToLower()))
                                )
                                .AsNoTracking()

                        join g in _productionStandardCostRepository.GetAll().AsNoTracking()
                                  .Select(s => new StandardCostGroupSummary
                                  {
                                      ProductionPlanId = s.ProductionPlanId,
                                      GroupName = s.StandardCostGroupId.HasValue ? s.StandardCostGroup.Value : L("Other"),
                                      QtyPercentage = s.QtyPercentage * 100,
                                      NetWeightPercentage = s.NetWeightPercentage * 100,
                                      TotalNetWeight = s.TotalNetWeight,
                                      TotalQty = s.TotalQty,
                                  })
                        on s.Id equals g.ProductionPlanId
                        into groups

                        join ig in _productionIssueStandardCostRepository.GetAll().AsNoTracking()
                                  .Select(s => new StandardCostGroupSummary
                                  {
                                      ProductionPlanId = s.ProductionPlanId,
                                      GroupName = s.StandardCostGroupId.HasValue ? s.StandardCostGroup.Value : L("Other"),
                                      QtyPercentage = s.QtyPercentage * 100,
                                      NetWeightPercentage = s.NetWeightPercentage * 100,
                                      TotalNetWeight = s.TotalNetWeight,
                                      TotalQty = s.TotalQty,
                                  })
                        on s.Id equals ig.ProductionPlanId
                        into issueGroups

                        select new ProductionPlanDetailOutput
                        {
                            Id = s.Id,
                            Reference = s.Reference,
                            DocumentNo = s.DocumentNo,
                            StartDate = s.StartDate,
                            EndDate = s.EndDate,
                            Description = s.Description,
                            ProductionProcess = s.ProductionProcess,
                            LocationId = s.LocationId,
                            LocationName = s.Location.LocationName,
                            Status = s.Status,
                            UserId = s.CreatorUserId,
                            UserName = s.CreatorUser == null ? "" : s.CreatorUser.UserName,
                            ProductionLineId = s.ProductionLineId,
                            ProductionLineName = s.ProductionLineId.HasValue ? s.ProductionLine.ProductionLineName : "",
                            TotalIssueQty = s.TotalIssueQty,
                            TotalReceiptQty = s.TotalReceiptQty,
                            TotalIssueNetWeight = s.TotalIssueNetWeight,
                            TotalReceiptNetWeight = s.TotalReceiptNetWeight,
                            QtyBalance = s.TotalIssueQty - s.TotalReceiptQty,
                            NetWeightBalance = s.TotalIssueNetWeight - s.TotalReceiptNetWeight,
                            QtyYield = s.TotalIssueQty == 0 ? 0 : Math.Round(s.TotalReceiptQty / s.TotalIssueQty, 4) * 100, 
                            NetWeightYield = s.TotalIssueNetWeight == 0 ? 0 : Math.Round(s.TotalReceiptNetWeight / s.TotalIssueNetWeight, 4) * 100, 
                            StandardCostGroups = groups.OrderBy(g => g.GroupName).ToList(),
                            IssueStandardCostGroups = issueGroups.OrderBy(g => g.GroupName).ToList(),
                        };

            var count = await query.CountAsync();
            var items = new List<ProductionPlanDetailOutput>();
            if (count > 0)
            {
                items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            }

            decimal totalIssueQty = 0;
            decimal totalReceiptQty = 0;
            decimal totalQtyBalance = 0;
            decimal totalIssueNetWeight = 0;
            decimal totalReceiptNetWeight = 0;
            decimal totalNetWeightBalance = 0;
            decimal qtyYield = 0;
            decimal netWeightYield = 0;
            var summaries = new List<ProductionPlanSummary>();
            var issueSummaries = new List<ProductionPlanSummary>();

            var tenant = await GetCurrentTenantAsync();

            if(tenant.ProductionSummaryNetWeight || tenant.ProductionSummaryQty)
            {
                var summaryList = await query.Select(s => new {
                    s.TotalIssueQty,
                    s.TotalReceiptQty,
                    s.TotalIssueNetWeight,
                    s.TotalReceiptNetWeight,
                    s.StandardCostGroups,
                    s.IssueStandardCostGroups
                })
               .ToListAsync();

                totalIssueQty = summaryList.Sum(t => t.TotalIssueQty);
                totalReceiptQty = summaryList.Sum(t => t.TotalReceiptQty);
                totalQtyBalance = summaryList.Sum(t => t.TotalIssueQty - t.TotalReceiptQty);
                totalIssueNetWeight = summaryList.Sum(t => t.TotalIssueNetWeight);
                totalReceiptNetWeight = summaryList.Sum(t => t.TotalReceiptNetWeight);
                totalNetWeightBalance = summaryList.Sum(t => t.TotalIssueNetWeight - t.TotalReceiptNetWeight);
                qtyYield = totalIssueQty == 0 ? 0 : Math.Round(totalReceiptQty / totalIssueQty, 4) * 100;
                netWeightYield = totalIssueNetWeight == 0 ? 0 : Math.Round(totalReceiptNetWeight / totalIssueNetWeight, 4) * 100;

                summaries = summaryList
                    .SelectMany(s => s.StandardCostGroups)
                    .OrderBy(s => s.GroupName)
                    .GroupBy(s => s.GroupName)
                    .Select(s => new ProductionPlanSummary
                    {
                        ProductionPlan = s.Key,
                        TotalReceiptQty = s.Sum(t => t.TotalQty),
                        TotalReceiptNetWeight = s.Sum(t => t.TotalNetWeight),
                        QtyYield = totalIssueQty == 0 ? 0 : Math.Round(s.Sum(t => t.TotalQty) / totalIssueQty, 4) * 100,
                        NetWeightYield = totalIssueNetWeight == 0 ? 0 : Math.Round(s.Sum(t => t.TotalNetWeight) / totalIssueNetWeight, 4) * 100,
                    })
                    .ToList();

                issueSummaries = summaryList
                   .SelectMany(s => s.IssueStandardCostGroups)
                   .OrderBy(s => s.GroupName)
                   .GroupBy(s => s.GroupName)
                   .Select(s => new ProductionPlanSummary
                   {
                       ProductionPlan = s.Key,
                       TotalIssueQty = s.Sum(t => t.TotalQty),
                       TotalIssueNetWeight = s.Sum(t => t.TotalNetWeight),
                       QtyYield = totalIssueQty == 0 ? 0 : Math.Round(s.Sum(t => t.TotalQty) / totalIssueQty, 4) * 100,
                       NetWeightYield = totalIssueNetWeight == 0 ? 0 : Math.Round(s.Sum(t => t.TotalNetWeight) / totalIssueNetWeight, 4) * 100,
                   })
                   .ToList();
            }

            return new PageResultProductioinPlanSummary
            {
                TotalCount = count,
                Items = items,
                Summaries = summaries,
                IssueSummaries = issueSummaries,
                TotalIssueQty = totalIssueQty,
                TotalReceiptQty = totalReceiptQty,
                TotalQtyBalance = totalQtyBalance,
                TotalIssueNetWeight = totalIssueNetWeight,
                TotalReceiptNetWeight = totalReceiptNetWeight,
                TotalNetWeightBalance = totalNetWeightBalance,
                QtyYield = qtyYield,
                NetWeightYield = netWeightYield
            };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Plan_Update)]
        public async Task<ProductionPlanDetailOutput> Update(CreateOrUpdateProductionPlanInput input)
        {
            var autoSequence = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ProductionPlan);
            await CheckDuplicate(input);
            if (autoSequence.RequireReference) await CheckDuplicateReference(input);

            var entity = await _productionPlanRepository.GetAll().FirstOrDefaultAsync(s => s.Id == input.Id);
            if(entity == null) throw new UserFriendlyException(L("RecordNotFound"));

            entity.Update(AbpSession.UserId.Value, input.LocationId, input.DocumentNo, input.Reference, input.StartDate, input.EndDate, input.Description, input.ProductionLineId, input.ProductionProcess);
            await _productionPlanRepository.UpdateAsync(entity);

            return ObjectMapper.Map<ProductionPlanDetailOutput>(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Plan_Calculate)]
        public async Task Calculation(ProductionPlanCalculationInput input)
        {
            await _productionPlanManager.CalculateAsync(AbpSession.UserId.Value, input.FromDate, input.ToDate);
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Production_Plan_Calculate)]
        public async Task CalculateById(EntityDto<Guid> input)
        {
            await _productionPlanManager.CalculateByIdAsync(AbpSession.UserId.Value, new List<Guid> { input.Id });
        }
    }
}
