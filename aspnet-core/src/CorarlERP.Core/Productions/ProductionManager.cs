using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.AccountCycles;
using CorarlERP.AutoSequences;
using CorarlERP.MultiTenancy;
using CorarlERP.ProductionPlans;
using CorarlERP.ProductionProcesses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Productions
{
    public class ProductionManager : CorarlERPDomainServiceBase, IProductionManager
    {
        private readonly ICorarlRepository<Production, Guid> _productionRepository;
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly IRepository<AccountCycle, long> _accountCycleRepository;
        private readonly IRepository<RawMaterialItems, Guid> _rawMaterialItemsRepository;
        private readonly IRepository<FinishItems, Guid> _finishItemsRepository;
        private readonly IProductionPlanManager _productionPlanManager;
        private readonly ICorarlRepository<ProductionStandardCostGroup, Guid> _productionStandardCostGroupRepository;
        private readonly ICorarlRepository<ProductionIssueStandardCostGroup, Guid> _productionIssueStandardCostGroupRepository;

        public ProductionManager(
            IProductionPlanManager productionPlanManager,
            IRepository<RawMaterialItems, Guid> rawMaterialItemsRepository,
            IRepository<FinishItems, Guid> finishItemsRepository,
            ICorarlRepository<ProductionStandardCostGroup, Guid> productionStandardCostGroupRepository,
            ICorarlRepository<ProductionIssueStandardCostGroup, Guid> productionIssueStandardCostGroupRepository,
            ICorarlRepository<Production, Guid> TransferOrderRepository, 
            IRepository<Tenant, int> tenantRepository,
            IRepository<AccountCycle,long> accountCycleRepository,
            IAutoSequenceManager autoSequenceManager) : base(accountCycleRepository)
        {
            _autoSequenceManager = autoSequenceManager;
            _productionRepository = TransferOrderRepository;
            _tenantRepository = tenantRepository;
            _accountCycleRepository = accountCycleRepository;
            _rawMaterialItemsRepository = rawMaterialItemsRepository;
            _finishItemsRepository = finishItemsRepository;
            _productionStandardCostGroupRepository = productionStandardCostGroupRepository;
            _productionIssueStandardCostGroupRepository = productionIssueStandardCostGroupRepository;
            _productionPlanManager = productionPlanManager;
        }
        public async Task<IdentityResult> CreateAsync(Production entity, bool checkDupliateReference)
        {
            await CheckDuplicateProduction(entity);
            await CheckClosePeriod(entity.Date);
            if (checkDupliateReference) await CheckDuplicateReferenceNo(entity);
            await _productionRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        private async Task CheckDuplicateReferenceNo(Production @entity)
        {
            if (entity.Reference.IsNullOrEmpty())
            {
                throw new UserFriendlyException(L("RequireReferenceNo"));
            }

            var @old = await _productionRepository.GetAll().AsNoTracking()
                           .Where(u => u.Reference.ToLower() == entity.Reference.ToLower() && u.Id != entity.Id)
                           .FirstOrDefaultAsync();

            if (old != null && old.Reference.ToLower() == entity.Reference.ToLower())
            {
                throw new UserFriendlyException(L("DuplicateReferenceNo"));
            }
        }
       
        public async Task<Production> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _productionRepository.GetAll()
                .Include(u => u.FromLocation)
                .Include(u => u.ToLocation)
                .Include(u => u.FromClass)
                .Include(u => u.ProductionAccount)
                .Include(u => u.ProductionProcess)
                .Include(u => u.ToClass)
                .Include(u => u.ProductionPlan)
                :
                _productionRepository.GetAll()
                .Include(u => u.ProductionAccount)
                .Include(u => u.ProductionProcess)
                .Include(u => u.FromLocation)
                .Include(u => u.ToLocation)
                .Include(u => u.FromClass)
                .Include(u => u.ToClass)
                .Include(u => u.ProductionPlan)
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(Production entity)
        {
            await CheckClosePeriod(entity.Date);
            await _productionRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(Production entity)
        {

            await CheckDuplicateProduction(entity);
            await CheckClosePeriod(entity.Date);

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.ProductionOrder);
            if (auto.RequireReference) await CheckDuplicateReferenceNo(entity);

            await _productionRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        private async Task CheckDuplicateProduction(Production @entity)
        {
            var @old = await _productionRepository.GetAll().AsNoTracking()
                           .Where(u => u.ProductionNo.ToLower() == entity.ProductionNo.ToLower() && u.Id != entity.Id)
                           .FirstOrDefaultAsync();

            if (old != null && old.ProductionNo.ToLower() == entity.ProductionNo.ToLower())
            {
                throw new UserFriendlyException(L("DuplicateProductionNo", entity.ProductionNo));
            }
        }

        public async Task CalculateAsync(long userId, DateTime fromDate, DateTime toDate)
        {
            var rawItemDic = await _rawMaterialItemsRepository.GetAll().AsNoTracking()
                                    .Where(s => s.Production.Date.Date >= fromDate.Date)
                                    .Where(s => s.Production.Date.Date <= toDate.Date)
                                    .Where(s =>
                                        s.Production.ProductionProcessId.HasValue && (
                                        s.Production.ProductionProcess.ProductionProcessType == ProductionProcessType.Both ||
                                        s.Production.ProductionProcess.ProductionProcessType == ProductionProcessType.Issue))
                                    .GroupBy(s => s.ProductionId)
                                    .Select(s => new
                                    {
                                        Id = s.Key,
                                        items = s.GroupBy(g => g.ItemId).Select(i => new KeyValuePair<Guid, decimal>(i.Key, i.Sum(t => t.Qty)))
                                    })
                                    .ToDictionaryAsync(k => k.Id, v => v.items);

            var finishItemDic = await _finishItemsRepository.GetAll().AsNoTracking()
                                        .Where(s => s.Production.Date.Date >= fromDate.Date)
                                        .Where(s => s.Production.Date.Date <= toDate.Date)
                                        .Where(s =>
                                            s.Production.ProductionProcessId.HasValue && (
                                            s.Production.ProductionProcess.ProductionProcessType == ProductionProcessType.Both ||
                                            s.Production.ProductionProcess.ProductionProcessType == ProductionProcessType.Receipt))
                                        .GroupBy(s => s.ProductionId)
                                        .Select(s => new
                                        {
                                            Id = s.Key,
                                            Items = s.GroupBy(g => g.ItemId).Select(i => new KeyValuePair<Guid, decimal>(i.Key, i.Sum(t => t.Qty)))
                                        })
                                        .ToDictionaryAsync(k => k.Id, v => v.Items);


            var productions = await _productionRepository.GetAll().AsNoTracking()
                                    .Where(s => s.Date.Date >= fromDate.Date)
                                    .Where(s => s.Date.Date <= toDate.Date)
                                    .ToListAsync();

            var ids = productions.Select(s => s.Id).ToHashSet();

            var standardCostGroups = await _productionStandardCostGroupRepository.GetAll().AsNoTracking()
                                           .Where(s => ids.Contains(s.ProductionId))
                                           .ToListAsync();

            var issueStandardCostGroups = await _productionIssueStandardCostGroupRepository.GetAll().AsNoTracking()
                                          .Where(s => ids.Contains(s.ProductionId))
                                          .ToListAsync();

            var itemIds = rawItemDic.Values.Concat(finishItemDic.Values).SelectMany(s => s.Select(i => i.Key)).Distinct().ToList();
            var propertyDic = await _productionPlanManager.GetItemNetWeight(itemIds);
            var standardGroupDic = await _productionPlanManager.GetItemStandardGroups(itemIds);

            var addStandardCostGroups = new List<ProductionStandardCostGroup>();
            var updateStandardCostGroups = new List<ProductionStandardCostGroup>();
            var deleteStandardCostGroups = new List<ProductionStandardCostGroup>();
            var addIssueStandardCostGroups = new List<ProductionIssueStandardCostGroup>();
            var updateIssueStandardCostGroups = new List<ProductionIssueStandardCostGroup>();
            var deleteIssueStandardCostGroups = new List<ProductionIssueStandardCostGroup>();

            foreach (var p in productions)
            {
                var totalIssueQty = rawItemDic.ContainsKey(p.Id) ? rawItemDic[p.Id].Sum(t => t.Value) : 0;
                var totalIssueNetWeight = rawItemDic.ContainsKey(p.Id) ? rawItemDic[p.Id].Sum(t => t.Value *
                                          (propertyDic.ContainsKey(t.Key) ? propertyDic[t.Key] : 0)) : 0;
                var totalReceiptQty = finishItemDic.ContainsKey(p.Id) ? finishItemDic[p.Id].Sum(t => t.Value) : 0;
                var totalReceiptNetWeight = finishItemDic.ContainsKey(p.Id) ? finishItemDic[p.Id].Sum(t => t.Value *
                                            (propertyDic.ContainsKey(t.Key) ? propertyDic[t.Key] : 0)) : 0;

                p.UpdateSummary(totalIssueQty, totalReceiptQty, totalIssueNetWeight, totalReceiptNetWeight);

                var groupList = new List<StandardGroupSummary>();
                if (finishItemDic.ContainsKey(p.Id))
                {
                    foreach (var g in finishItemDic[p.Id])
                    {
                        var key = standardGroupDic.ContainsKey(g.Key) ? standardGroupDic[g.Key] : (long?)null;
                        var find = groupList.FirstOrDefault(f => f.GroupId == key);

                        if (find != null)
                        {
                            find.TotalQty += g.Value;
                            find.TotalNetWeight += g.Value * (propertyDic.ContainsKey(g.Key) ? propertyDic[g.Key] : 0);
                        }
                        else
                        {
                            find = new StandardGroupSummary
                            {
                                GroupId = key,
                                TotalQty = g.Value,
                                TotalNetWeight = g.Value * (propertyDic.ContainsKey(g.Key) ? propertyDic[g.Key] : 0)
                            };

                            groupList.Add(find);
                        }
                    }
                }

                var groups = standardCostGroups.Where(s => s.ProductionId == p.Id).ToList();
                var addGroups = groupList.Where(s => !groups.Any(r => r.StandardCostGroupId == s.GroupId)).ToList();
                var updateGroups = groupList.Where(s => groups.Any(r => r.StandardCostGroupId == s.GroupId)).ToList();
                var deleteGroups = groups.Where(s => !groupList.Any(r => r.GroupId == s.StandardCostGroupId)).ToList();

                if (addGroups.Any())
                {
                    foreach (var g in addGroups)
                    {
                        var group = ProductionStandardCostGroup.Create(p.TenantId.Value, userId, p.Id, g.GroupId, g.TotalQty, g.TotalNetWeight);
                        addStandardCostGroups.Add(group);
                    }
                }
                if (updateGroups.Any())
                {
                    foreach (var g in updateGroups)
                    {
                        var group = groups.FirstOrDefault(s => s.StandardCostGroupId == g.GroupId);
                        if (group == null) throw new UserFriendlyException(L("RecordNotFound"));

                        group.Update(userId, p.Id, g.GroupId, g.TotalQty, g.TotalNetWeight);
                        updateStandardCostGroups.Add(group);
                    }
                }
                if (deleteGroups.Any()) deleteStandardCostGroups.AddRange(deleteGroups);

                var issueGroupList = new List<StandardGroupSummary>();
                if (rawItemDic.ContainsKey(p.Id))
                {
                    foreach (var g in rawItemDic[p.Id])
                    {
                        var key = standardGroupDic.ContainsKey(g.Key) ? standardGroupDic[g.Key] : (long?)null;
                        var find = issueGroupList.FirstOrDefault(f => f.GroupId == key);

                        if (find != null)
                        {
                            find.TotalQty += g.Value;
                            find.TotalNetWeight += g.Value * (propertyDic.ContainsKey(g.Key) ? propertyDic[g.Key] : 0);
                        }
                        else
                        {
                            find = new StandardGroupSummary
                            {
                                GroupId = key,
                                TotalQty = g.Value,
                                TotalNetWeight = g.Value * (propertyDic.ContainsKey(g.Key) ? propertyDic[g.Key] : 0)
                            };

                            issueGroupList.Add(find);
                        }
                    }
                }

                var issueGroups = issueStandardCostGroups.Where(s => s.ProductionId == p.Id).ToList();
                var addIssueGroups = issueGroupList.Where(s => !issueGroups.Any(r => r.StandardCostGroupId == s.GroupId)).ToList();
                var updateIssueGroups = issueGroupList.Where(s => issueGroups.Any(r => r.StandardCostGroupId == s.GroupId)).ToList();
                var deleteIssueGroups = issueGroups.Where(s => !issueGroupList.Any(r => r.GroupId == s.StandardCostGroupId)).ToList();

                if (addIssueGroups.Any())
                {
                    foreach (var g in addIssueGroups)
                    {
                        var group = ProductionIssueStandardCostGroup.Create(p.TenantId.Value, userId, p.Id, g.GroupId, g.TotalQty, g.TotalNetWeight);
                        addIssueStandardCostGroups.Add(group);
                    }
                }
                if (updateIssueGroups.Any())
                {
                    foreach (var g in updateIssueGroups)
                    {
                        var group = issueGroups.FirstOrDefault(s => s.StandardCostGroupId == g.GroupId);
                        if (group == null) throw new UserFriendlyException(L("RecordNotFound"));

                        group.Update(userId, p.Id, g.GroupId, g.TotalQty, g.TotalNetWeight);
                        updateIssueStandardCostGroups.Add(group);
                    }
                }
                if (deleteIssueGroups.Any()) deleteIssueStandardCostGroups.AddRange(deleteIssueGroups);

            }

            await _productionRepository.BulkUpdateAsync(productions);
            if (addStandardCostGroups.Any()) await _productionStandardCostGroupRepository.BulkInsertAsync(addStandardCostGroups);
            if (updateStandardCostGroups.Any()) await _productionStandardCostGroupRepository.BulkUpdateAsync(updateStandardCostGroups);
            if (deleteStandardCostGroups.Any()) await _productionStandardCostGroupRepository.BulkDeleteAsync(deleteStandardCostGroups);
            if (addIssueStandardCostGroups.Any()) await _productionIssueStandardCostGroupRepository.BulkInsertAsync(addIssueStandardCostGroups);
            if (updateIssueStandardCostGroups.Any()) await _productionIssueStandardCostGroupRepository.BulkUpdateAsync(updateIssueStandardCostGroups);
            if (deleteIssueStandardCostGroups.Any()) await _productionIssueStandardCostGroupRepository.BulkDeleteAsync(deleteIssueStandardCostGroups);
        }
    }
}
