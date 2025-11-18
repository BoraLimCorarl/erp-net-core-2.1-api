using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.UI;
using CorarlERP.Inventories.Data;
using CorarlERP.Items;
using CorarlERP.ProductionProcesses;
using CorarlERP.Productions;
using CorarlERP.PropertyValues;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Org.BouncyCastle.Crypto;

namespace CorarlERP.ProductionPlans
{
    public class ProductionPlanManager : CorarlERPDomainServiceBase, IProductionPlanManager
    {
        private readonly IRepository<ItemProperty, Guid> _itemPropertyRepository;
        private readonly IRepository<PropertyValue, long> _propertyValueRepository;
        private readonly IRepository<Property, long> _propertyRepository;
        private readonly ICorarlRepository<ProductionStandardCost, Guid> _productionStandardCostRepository;
        private readonly ICorarlRepository<ProductionIssueStandardCost, Guid> _productionIssueStandardCostRepository;
        private readonly IRepository<RawMaterialItems, Guid> _rawMaterialItemsRepository;
        private readonly IRepository<FinishItems, Guid> _finishItemsRepository;
        private readonly IRepository<Item, Guid> _itemRepository;
        private readonly ICorarlRepository<ProductionPlan, Guid> _productionPlanRepository;
        public ProductionPlanManager(
            IRepository<ItemProperty, Guid> itemPropertyRepository,
            IRepository<PropertyValue, long> propertyValueRepository,
            IRepository<Property, long> propertyRepository,
            ICorarlRepository<ProductionIssueStandardCost, Guid> productionIssueStandardCostRepository,
            ICorarlRepository<ProductionStandardCost, Guid> productionStandardCostRepository,
            IRepository<RawMaterialItems, Guid> rawMaterialItemsRepository,
            IRepository<FinishItems, Guid> finishItemsRepository,
            IRepository<Item, Guid> itemRepository,
            ICorarlRepository<ProductionPlan, Guid> productionPlanRepository)
        {
            _productionPlanRepository = productionPlanRepository;
            _itemRepository = itemRepository;
            _rawMaterialItemsRepository = rawMaterialItemsRepository;
            _finishItemsRepository = finishItemsRepository;
            _productionIssueStandardCostRepository = productionIssueStandardCostRepository;
            _productionStandardCostRepository = productionStandardCostRepository;
            _itemPropertyRepository = itemPropertyRepository;
            _propertyRepository = propertyRepository;
            _propertyValueRepository = propertyValueRepository;
        }

        /// <summary>
        /// Recommended to be called after CurrentUnitOfWork.SaveChangesAsync()
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task CalculateAsync(long userId, DateTime fromDate, DateTime toDate)
        {
            var productionPlans = await _productionPlanRepository.GetAll().AsNoTracking()
                                   .Where(s => s.StartDate.Value.Date >= fromDate.Date)
                                   .Where(s => s.StartDate.Value.Date <= toDate.Date)
                                   .ToListAsync();

            var ids = productionPlans.Select(s => s.Id).ToHashSet();

            var standardCostGroups = await _productionStandardCostRepository.GetAll().AsNoTracking()
                                           .Where(s => ids.Contains(s.ProductionPlanId))
                                           .ToListAsync();

            var issueStandardCostGroups = await _productionIssueStandardCostRepository.GetAll().AsNoTracking()
                                         .Where(s => ids.Contains(s.ProductionPlanId))
                                         .ToListAsync();

            //var rawItemDic = await _rawMaterialItemsRepository.GetAll().AsNoTracking()
            //                       .Where(s => ids.Contains(s.Production.ProductionPlanId.Value))
            //                       .Where(s =>
            //                           s.Production.ProductionProcessId.HasValue && (
            //                           s.Production.ProductionProcess.ProductionProcessType == ProductionProcessType.Both ||
            //                           s.Production.ProductionProcess.ProductionProcessType == ProductionProcessType.Issue))
            //                       .GroupBy(s => s.Production.ProductionPlanId)
            //                       .Select(s => new
            //                       {
            //                           Id = s.Key,
            //                           items = s.GroupBy(g => g.ItemId).Select(i => new
            //                           {
            //                               ItemId = i.Key,
            //                               Qty = i.Sum(t => t.Qty)
            //                           })
            //                       })
            //                       .ToDictionaryAsync(k => k.Id, v => v.items);
            var rawItemQuery = from r in _rawMaterialItemsRepository.GetAll().AsNoTracking()
                                             .Where(s => ids.Contains(s.Production.ProductionPlanId.Value))
                                             .Where(s =>
                                                s.Production.ProductionProcessId.HasValue && (
                                                s.Production.ProductionProcess.ProductionProcessType == ProductionProcessType.Both ||
                                                s.Production.ProductionProcess.ProductionProcessType == ProductionProcessType.Issue))
                                  join i in _itemRepository.GetAll()
                                            .AsNoTracking()
                                            .Include(s => s.Properties).ThenInclude(s => s.Property)
                                            .Include(s => s.Properties).ThenInclude(s => s.PropertyValue)
                                  on r.ItemId equals i.Id
                                  let g = i.Properties.FirstOrDefault(s => s.Property.IsStandardCostGroup)
                                  select new
                                  {
                                      r.Production.ProductionPlanId,
                                      r.Qty,
                                      r.ItemId,
                                      Group = g == null ? null : g.PropertyValueId,
                                  };

            var rawItems = (await rawItemQuery.ToListAsync())
                            .GroupBy(s => new { s.ProductionPlanId, s.Group })
                            .Select(s => new
                            {
                                ProductionPlanId = s.Key.ProductionPlanId,
                                Group = s.Key.Group,
                                Items = s.GroupBy(g => g.ItemId).Select(i => new
                                {
                                    ItemId = i.Key,
                                    Qty = i.Sum(t => t.Qty)
                                })
                            })
                            .ToList();

            var finishItemQuery = from r in _finishItemsRepository.GetAll().AsNoTracking()
                                             .Where(s => ids.Contains(s.Production.ProductionPlanId.Value))
                                             .Where(s =>
                                                s.Production.ProductionProcessId.HasValue && (
                                                s.Production.ProductionProcess.ProductionProcessType == ProductionProcessType.Both ||
                                                s.Production.ProductionProcess.ProductionProcessType == ProductionProcessType.Receipt))
                                  join i in _itemRepository.GetAll()
                                            .AsNoTracking()
                                            .Include(s => s.Properties).ThenInclude(s => s.Property)
                                            .Include(s => s.Properties).ThenInclude(s => s.PropertyValue)
                                  on r.ItemId equals i.Id
                                  let g = i.Properties.FirstOrDefault(s => s.Property.IsStandardCostGroup)
                                  select new
                                  {
                                      r.Production.ProductionPlanId,
                                      r.Qty,
                                      r.ItemId,
                                      Group = g == null ? null : g.PropertyValueId,
                                  };

            var finishItems = (await finishItemQuery.ToListAsync())
                            .GroupBy(s => new { s.ProductionPlanId, s.Group })
                            .Select(s => new
                            {
                                ProductionPlanId = s.Key.ProductionPlanId,
                                Group = s.Key.Group,
                                Items = s.GroupBy(g => g.ItemId).Select(i => new
                                {
                                    ItemId = i.Key,
                                    Qty = i.Sum(t => t.Qty)
                                })
                            })
                            .ToList();


            //var rawItemIds = rawItemDic.Values.SelectMany(s => s.Select(i => i.ItemId)).ToList();
            var rawItemIds = rawItems.SelectMany(s => s.Items.Select(i => i.ItemId)).ToList();
            var finisItemIds = finishItems.SelectMany(s => s.Items.Select(i => i.ItemId)).ToList();
            var itemIds = rawItemIds.Concat(finisItemIds).Distinct().ToList();

            var propertyDic = await GetItemNetWeight(itemIds);


            var addStandardCostGroups = new List<ProductionStandardCost>();
            var updateStandardCostGroups = new List<ProductionStandardCost>();
            var deleteStandardCostGroups = new List<ProductionStandardCost>();
            var addIssueStandardCostGroups = new List<ProductionIssueStandardCost>();
            var updateIssueStandardCostGroups = new List<ProductionIssueStandardCost>();
            var deleteIssueStandardCostGroups = new List<ProductionIssueStandardCost>();

            foreach (var p in productionPlans)
            {
                //var totalIssueQty = rawItemDic.ContainsKey(p.Id) ? rawItemDic[p.Id].Sum(t => t.Qty) : 0;
                //var totalIssueNetWeight = rawItemDic.ContainsKey(p.Id) ? rawItemDic[p.Id].Sum(t => t.Qty *
                //                          (propertyDic.ContainsKey(t.ItemId) ? propertyDic[t.ItemId] : 0)) : 0;

                var issueItems = rawItems.Where(s => s.ProductionPlanId == p.Id).ToList();
                var issueGroups = issueStandardCostGroups.Where(s => s.ProductionPlanId == p.Id).ToList();

                var totalIssueQty = issueItems.SelectMany(s => s.Items).Sum(t => t.Qty);
                var totalIssueNetWeight = issueItems.SelectMany(s => s.Items).Sum(t => t.Qty *
                                            (propertyDic.ContainsKey(t.ItemId) ? propertyDic[t.ItemId] : 0));


                var addIssueGroups = issueItems.Where(s => !issueGroups.Any(r => r.StandardCostGroupId == s.Group)).ToList();
                var updateIssueGroups = issueItems.Where(s => issueGroups.Any(r => r.StandardCostGroupId == s.Group)).ToList();
                var deleteIssueGroups = issueGroups.Where(s => !issueItems.Any(r => r.Group == s.StandardCostGroupId)).ToList();

                if (addIssueGroups.Any())
                {
                    foreach (var g in addIssueGroups)
                    {
                        var totalQty = g.Items.Sum(t => t.Qty);
                        var totalNetWeight = g.Items.Sum(t => t.Qty * (propertyDic.ContainsKey(t.ItemId) ? propertyDic[t.ItemId] : 0));
                        var qtyPercentage = totalIssueQty == 0 ? 0 : Math.Round(totalQty / totalIssueQty, 4);
                        var netWeightPercentage = totalIssueNetWeight == 0 ? 0 : Math.Round(totalNetWeight / totalIssueNetWeight, 4);

                        var group = ProductionIssueStandardCost.Create(p.TenantId.Value, userId, p.Id, g.Group, totalQty, totalNetWeight, qtyPercentage, netWeightPercentage);
                        addIssueStandardCostGroups.Add(group);
                    }
                }
                if (updateIssueGroups.Any())
                {
                    foreach (var g in updateIssueGroups)
                    {
                        var group = issueGroups.FirstOrDefault(s => s.StandardCostGroupId == g.Group);
                        if (group == null) throw new UserFriendlyException(L("RecordNotFound"));

                        var totalQty = g.Items.Sum(t => t.Qty);
                        var totalNetWeight = g.Items.Sum(t => t.Qty * (propertyDic.ContainsKey(t.ItemId) ? propertyDic[t.ItemId] : 0));
                        var qtyPercentage = totalIssueQty == 0 ? 0 : Math.Round(totalQty / totalIssueQty, 4);
                        var netWeightPercentage = totalIssueNetWeight == 0 ? 0 : Math.Round(totalNetWeight / totalIssueNetWeight, 4);

                        group.Update(userId, p.Id, g.Group, totalQty, totalNetWeight, qtyPercentage, netWeightPercentage);
                        updateIssueStandardCostGroups.Add(group);
                    }
                }
                if (deleteIssueGroups.Any()) deleteIssueStandardCostGroups.AddRange(deleteIssueGroups);


                var receiptItems = finishItems.Where(s => s.ProductionPlanId == p.Id).ToList();
                var groups = standardCostGroups.Where(s => s.ProductionPlanId == p.Id).ToList();
                
                var totalReceiptQty = receiptItems.SelectMany(s => s.Items).Sum(t => t.Qty);
                var totalReceiptNetWeight = receiptItems.SelectMany(s => s.Items).Sum(t => t.Qty *
                                            (propertyDic.ContainsKey(t.ItemId) ? propertyDic[t.ItemId] : 0));

                var addGroups = receiptItems.Where(s => !groups.Any(r => r.StandardCostGroupId == s.Group)).ToList();
                var updateGroups = receiptItems.Where(s => groups.Any(r => r.StandardCostGroupId == s.Group)).ToList();
                var deleteGroups = groups.Where(s => !receiptItems.Any(r => r.Group == s.StandardCostGroupId)).ToList();

                if (addGroups.Any())
                {
                    foreach (var g in addGroups)
                    {
                        var totalQty = g.Items.Sum(t => t.Qty);
                        var totalNetWeight = g.Items.Sum(t => t.Qty * (propertyDic.ContainsKey(t.ItemId) ? propertyDic[t.ItemId] : 0));
                        var qtyPercentage = totalIssueQty == 0 ? 0 : Math.Round(totalQty / totalIssueQty, 4);
                        var netWeightPercentage = totalIssueNetWeight == 0 ? 0 : Math.Round(totalNetWeight / totalIssueNetWeight, 4);

                        var group = ProductionStandardCost.Create(p.TenantId.Value, userId, p.Id, g.Group, totalQty, totalNetWeight, qtyPercentage, netWeightPercentage);
                        addStandardCostGroups.Add(group);
                    }
                }
                if (updateGroups.Any())
                {
                    foreach (var g in updateGroups)
                    {
                        var group = groups.FirstOrDefault(s => s.StandardCostGroupId == g.Group);
                        if (group == null) throw new UserFriendlyException(L("RecordNotFound"));

                        var totalQty = g.Items.Sum(t => t.Qty);
                        var totalNetWeight = g.Items.Sum(t => t.Qty * (propertyDic.ContainsKey(t.ItemId) ? propertyDic[t.ItemId] : 0));
                        var qtyPercentage = totalIssueQty == 0 ? 0 : Math.Round(totalQty / totalIssueQty, 4);
                        var netWeightPercentage = totalIssueNetWeight == 0 ? 0 : Math.Round(totalNetWeight / totalIssueNetWeight, 4);

                        group.Update(userId, p.Id, g.Group, totalQty, totalNetWeight, qtyPercentage, netWeightPercentage);
                        updateStandardCostGroups.Add(group);
                    }
                }
                if (deleteGroups.Any()) deleteStandardCostGroups.AddRange(deleteGroups);

                p.UpdateSummary(totalIssueQty, totalReceiptQty, totalIssueNetWeight, totalReceiptNetWeight);
            }

            await _productionPlanRepository.BulkUpdateAsync(productionPlans);
            if (addStandardCostGroups.Any()) await _productionStandardCostRepository.BulkInsertAsync(addStandardCostGroups);
            if (updateStandardCostGroups.Any()) await _productionStandardCostRepository.BulkUpdateAsync(updateStandardCostGroups);
            if (deleteStandardCostGroups.Any()) await _productionStandardCostRepository.BulkDeleteAsync(deleteStandardCostGroups);
            if (addIssueStandardCostGroups.Any()) await _productionIssueStandardCostRepository.BulkInsertAsync(addIssueStandardCostGroups);
            if (updateIssueStandardCostGroups.Any()) await _productionIssueStandardCostRepository.BulkUpdateAsync(updateIssueStandardCostGroups);
            if (deleteIssueStandardCostGroups.Any()) await _productionIssueStandardCostRepository.BulkDeleteAsync(deleteIssueStandardCostGroups);
        }

        /// <summary>
        /// Recommended to be called after CurrentUnitOfWork.SaveChangesAsync()
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task CalculateByIdAsync(long userId, List<Guid> ids)
        {
            var productionPlans = await _productionPlanRepository.GetAll().AsNoTracking()
                                   .Where(s => ids.Contains(s.Id))
                                   .ToListAsync();

            var standardCostGroups = await _productionStandardCostRepository.GetAll().AsNoTracking()
                                           .Where(s => ids.Contains(s.ProductionPlanId))
                                           .ToListAsync();

            var issueStandardCostGroups = await _productionIssueStandardCostRepository.GetAll().AsNoTracking()
                                          .Where(s => ids.Contains(s.ProductionPlanId))
                                          .ToListAsync();

            //var rawItemDic = await _rawMaterialItemsRepository.GetAll().AsNoTracking()
            //                       .Where(s => ids.Contains(s.Production.ProductionPlanId.Value))
            //                       .Where(s =>
            //                           s.Production.ProductionProcessId.HasValue && (
            //                           s.Production.ProductionProcess.ProductionProcessType == ProductionProcessType.Both ||
            //                           s.Production.ProductionProcess.ProductionProcessType == ProductionProcessType.Issue))
            //                       .GroupBy(s => s.Production.ProductionPlanId)
            //                       .Select(s => new
            //                       {
            //                           Id = s.Key,
            //                           items = s.GroupBy(g => g.ItemId).Select(i => new
            //                           {
            //                               ItemId = i.Key,
            //                               Qty = i.Sum(t => t.Qty)
            //                           })
            //                       })
            //                       .ToDictionaryAsync(k => k.Id, v => v.items);

            var rawItemQuery = from r in _rawMaterialItemsRepository.GetAll().AsNoTracking()
                                            .Where(s => ids.Contains(s.Production.ProductionPlanId.Value))
                                            .Where(s =>
                                               s.Production.ProductionProcessId.HasValue && (
                                               s.Production.ProductionProcess.ProductionProcessType == ProductionProcessType.Both ||
                                               s.Production.ProductionProcess.ProductionProcessType == ProductionProcessType.Issue))
                                  join i in _itemRepository.GetAll()
                                            .AsNoTracking()
                                            .Include(s => s.Properties).ThenInclude(s => s.Property)
                                            .Include(s => s.Properties).ThenInclude(s => s.PropertyValue)
                                  on r.ItemId equals i.Id
                                  let g = i.Properties.FirstOrDefault(s => s.Property.IsStandardCostGroup)
                                  select new
                                  {
                                      r.Production.ProductionPlanId,
                                      r.Qty,
                                      r.ItemId,
                                      Group = g == null ? null : g.PropertyValueId,
                                  };

            var rawItems = (await rawItemQuery.ToListAsync())
                            .GroupBy(s => new { s.ProductionPlanId, s.Group })
                            .Select(s => new
                            {
                                ProductionPlanId = s.Key.ProductionPlanId,
                                Group = s.Key.Group,
                                Items = s.GroupBy(g => g.ItemId).Select(i => new
                                {
                                    ItemId = i.Key,
                                    Qty = i.Sum(t => t.Qty)
                                })
                            })
                            .ToList();

            var finishItemQuery = from r in _finishItemsRepository.GetAll().AsNoTracking()
                                             .Where(s => ids.Contains(s.Production.ProductionPlanId.Value))
                                             .Where(s =>
                                                s.Production.ProductionProcessId.HasValue && (
                                                s.Production.ProductionProcess.ProductionProcessType == ProductionProcessType.Both ||
                                                s.Production.ProductionProcess.ProductionProcessType == ProductionProcessType.Receipt))
                                  join i in _itemRepository.GetAll()
                                            .AsNoTracking()
                                            .Include(s => s.Properties).ThenInclude(s => s.Property)
                                            .Include(s => s.Properties).ThenInclude(s => s.PropertyValue)
                                  on r.ItemId equals i.Id
                                  let g = i.Properties.FirstOrDefault(s => s.Property.IsStandardCostGroup)
                                  select new
                                  {
                                      r.Production.ProductionPlanId,
                                      r.Qty,
                                      r.ItemId,
                                      Group = g == null ? null : g.PropertyValueId,
                                  };

            var finishItems = (await finishItemQuery.ToListAsync())
                            .GroupBy(s => new { s.ProductionPlanId, s.Group })
                            .Select(s => new
                            {
                                ProductionPlanId = s.Key.ProductionPlanId,
                                Group = s.Key.Group,
                                Items = s.GroupBy(g => g.ItemId).Select(i => new
                                {
                                    ItemId = i.Key,
                                    Qty = i.Sum(t => t.Qty)
                                })
                            })
                            .ToList();


            //var rawItemIds = rawItemDic.Values.SelectMany(s => s.Select(i => i.ItemId)).ToList();
            var rawItemIds = rawItems.SelectMany(s => s.Items.Select(i => i.ItemId)).ToList();
            var finisItemIds = finishItems.SelectMany(s => s.Items.Select(i => i.ItemId)).ToList();
            var itemIds = rawItemIds.Concat(finisItemIds).Distinct().ToList();

            var propertyDic = await GetItemNetWeight(itemIds);


            var addStandardCostGroups = new List<ProductionStandardCost>();
            var updateStandardCostGroups = new List<ProductionStandardCost>();
            var deleteStandardCostGroups = new List<ProductionStandardCost>();
            var addIssueStandardCostGroups = new List<ProductionIssueStandardCost>();
            var updateIssueStandardCostGroups = new List<ProductionIssueStandardCost>();
            var deleteIssueStandardCostGroups = new List<ProductionIssueStandardCost>();

            foreach (var p in productionPlans)
            {
                //var totalIssueQty = rawItemDic.ContainsKey(p.Id) ? rawItemDic[p.Id].Sum(t => t.Qty) : 0;
                //var totalIssueNetWeight = rawItemDic.ContainsKey(p.Id) ? rawItemDic[p.Id].Sum(t => t.Qty * 
                //                          (propertyDic.ContainsKey(t.ItemId) ? propertyDic[t.ItemId] : 0) ) : 0;

                var issueItems = rawItems.Where(s => s.ProductionPlanId == p.Id).ToList();
                var issueGroups = issueStandardCostGroups.Where(s => s.ProductionPlanId == p.Id).ToList();

                var totalIssueQty = issueItems.SelectMany(s => s.Items).Sum(t => t.Qty);
                var totalIssueNetWeight = issueItems.SelectMany(s => s.Items).Sum(t => t.Qty *
                                            (propertyDic.ContainsKey(t.ItemId) ? propertyDic[t.ItemId] : 0));

                var addIssueGroups = issueItems.Where(s => !issueGroups.Any(r => r.StandardCostGroupId == s.Group)).ToList();
                var updateIssueGroups = issueItems.Where(s => issueGroups.Any(r => r.StandardCostGroupId == s.Group)).ToList();
                var deleteIssueGroups = issueGroups.Where(s => !issueItems.Any(r => r.Group == s.StandardCostGroupId)).ToList();

                if (addIssueGroups.Any())
                {
                    foreach (var g in addIssueGroups)
                    {
                        var totalQty = g.Items.Sum(t => t.Qty);
                        var totalNetWeight = g.Items.Sum(t => t.Qty * (propertyDic.ContainsKey(t.ItemId) ? propertyDic[t.ItemId] : 0));
                        var qtyPercentage = totalIssueQty == 0 ? 0 : Math.Round(totalQty / totalIssueQty, 4);
                        var netWeightPercentage = totalIssueNetWeight == 0 ? 0 : Math.Round(totalNetWeight / totalIssueNetWeight, 4);

                        var group = ProductionIssueStandardCost.Create(p.TenantId.Value, userId, p.Id, g.Group, totalQty, totalNetWeight, qtyPercentage, netWeightPercentage);
                        addIssueStandardCostGroups.Add(group);
                    }
                }
                if (updateIssueGroups.Any())
                {
                    foreach (var g in updateIssueGroups)
                    {
                        var group = issueGroups.FirstOrDefault(s => s.StandardCostGroupId == g.Group);
                        if (group == null) throw new UserFriendlyException(L("RecordNotFound"));

                        var totalQty = g.Items.Sum(t => t.Qty);
                        var totalNetWeight = g.Items.Sum(t => t.Qty * (propertyDic.ContainsKey(t.ItemId) ? propertyDic[t.ItemId] : 0));
                        var qtyPercentage = totalIssueQty == 0 ? 0 : Math.Round(totalQty / totalIssueQty, 4);
                        var netWeightPercentage = totalIssueNetWeight == 0 ? 0 : Math.Round(totalNetWeight / totalIssueNetWeight, 4);

                        group.Update(userId, p.Id, g.Group, totalQty, totalNetWeight, qtyPercentage, netWeightPercentage);
                        updateIssueStandardCostGroups.Add(group);
                    }
                }
                if (deleteIssueGroups.Any()) deleteIssueStandardCostGroups.AddRange(deleteIssueGroups);

                var receiptItems = finishItems.Where(s => s.ProductionPlanId == p.Id).ToList();
                var groups = standardCostGroups.Where(s => s.ProductionPlanId == p.Id).ToList();

                var totalReceiptQty = receiptItems.SelectMany(s => s.Items).Sum(t => t.Qty);
                var totalReceiptNetWeight = receiptItems.SelectMany(s => s.Items).Sum(t => t.Qty * 
                                            (propertyDic.ContainsKey(t.ItemId) ? propertyDic[t.ItemId] : 0));

                var addGroups = receiptItems.Where(s => !groups.Any(r => r.StandardCostGroupId == s.Group)).ToList();
                var updateGroups = receiptItems.Where(s => groups.Any(r => r.StandardCostGroupId == s.Group)).ToList();
                var deleteGroups = groups.Where(s => !receiptItems.Any(r => r.Group == s.StandardCostGroupId)).ToList();

                if (addGroups.Any())
                {
                    foreach (var g in addGroups)
                    {
                        var totalQty = g.Items.Sum(t => t.Qty);
                        var totalNetWeight = g.Items.Sum(t => t.Qty * (propertyDic.ContainsKey(t.ItemId) ? propertyDic[t.ItemId] : 0));
                        var qtyPercentage = totalIssueQty == 0 ? 0 : Math.Round(totalQty / totalIssueQty, 4);
                        var netWeightPercentage = totalIssueNetWeight == 0 ? 0 : Math.Round(totalNetWeight / totalIssueNetWeight, 4);

                        var group = ProductionStandardCost.Create(p.TenantId.Value, userId, p.Id, g.Group, totalQty, totalNetWeight, qtyPercentage, netWeightPercentage);
                        addStandardCostGroups.Add(group);
                    }
                }
                if (updateGroups.Any())
                {
                    foreach (var g in updateGroups)
                    {
                        var group = groups.FirstOrDefault(s => s.StandardCostGroupId == g.Group);
                        if (group == null) throw new UserFriendlyException(L("RecordNotFound"));

                        var totalQty = g.Items.Sum(t => t.Qty);
                        var totalNetWeight = g.Items.Sum(t => t.Qty * (propertyDic.ContainsKey(t.ItemId) ? propertyDic[t.ItemId] : 0));
                        var qtyPercentage = totalIssueQty == 0 ? 0 : Math.Round(totalQty / totalIssueQty, 4);
                        var netWeightPercentage = totalIssueNetWeight == 0 ? 0 : Math.Round(totalNetWeight / totalIssueNetWeight, 4);

                        group.Update(userId, p.Id, g.Group, totalQty, totalNetWeight, qtyPercentage, netWeightPercentage);
                        updateStandardCostGroups.Add(group);
                    }
                }
                if (deleteGroups.Any()) deleteStandardCostGroups.AddRange(deleteGroups);

                p.UpdateSummary(totalIssueQty, totalReceiptQty, totalIssueNetWeight, totalReceiptNetWeight);
            }

            await _productionPlanRepository.BulkUpdateAsync(productionPlans);
            if (addStandardCostGroups.Any()) await _productionStandardCostRepository.BulkInsertAsync(addStandardCostGroups);
            if (updateStandardCostGroups.Any()) await _productionStandardCostRepository.BulkUpdateAsync(updateStandardCostGroups);
            if (deleteStandardCostGroups.Any()) await _productionStandardCostRepository.BulkDeleteAsync(deleteStandardCostGroups);
            if (addIssueStandardCostGroups.Any()) await _productionIssueStandardCostRepository.BulkInsertAsync(addIssueStandardCostGroups);
            if (updateIssueStandardCostGroups.Any()) await _productionIssueStandardCostRepository.BulkUpdateAsync(updateIssueStandardCostGroups);
            if (deleteIssueStandardCostGroups.Any()) await _productionIssueStandardCostRepository.BulkDeleteAsync(deleteIssueStandardCostGroups);
        }


        public async Task<Dictionary<Guid, decimal>> GetItemNetWeight(List<Guid> itemIds)
        {
            var itemPropertyQuery = from ip in _itemPropertyRepository.GetAll()
                                                .WhereIf(itemIds != null && itemIds.Any(), s => itemIds.Contains(s.ItemId))
                                                .AsNoTracking()
                                    join pv in _propertyValueRepository.GetAll()
                                               .AsNoTracking()
                                    on ip.PropertyValueId equals pv.Id
                                    join p in _propertyRepository.GetAll()
                                              .AsNoTracking()
                                              .Where(s => s.IsUnit)
                                    on ip.PropertyId equals p.Id
                                    select new ItemPropertySummary
                                    {
                                        NetWeight = pv.NetWeight,
                                        ItemId = ip.ItemId,
                                    };

            var dic = await itemPropertyQuery.ToDictionaryAsync(k => k.ItemId, v => v.NetWeight);

            return dic;
        }


        public async Task<Dictionary<Guid, long>> GetItemStandardGroups(List<Guid> itemIds)
        {
            var itemPropertyQuery = from ip in _itemPropertyRepository.GetAll()
                                                .WhereIf(itemIds != null && itemIds.Any(), s => itemIds.Contains(s.ItemId))
                                                .AsNoTracking()
                                    join pv in _propertyValueRepository.GetAll()
                                               .AsNoTracking()
                                    on ip.PropertyValueId equals pv.Id
                                    join p in _propertyRepository.GetAll()
                                              .AsNoTracking()
                                              .Where(s => s.IsStandardCostGroup)
                                    on ip.PropertyId equals p.Id
                                    select new 
                                    {
                                        GroupId = pv.Id,
                                        ItemId = ip.ItemId,
                                    };

            var dic = await itemPropertyQuery.ToDictionaryAsync(k => k.ItemId, v => v.GroupId);

            return dic;
        }
    }
}
