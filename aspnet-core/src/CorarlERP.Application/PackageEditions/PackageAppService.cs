using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.Linq.Extensions;
using Abp.Authorization;
using CorarlERP.Authorization;
using Abp.UI;
using Abp.Collections.Extensions;
using System.Linq.Dynamic.Core;
using System;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using CorarlERP.PackageEditions.Dot;
using Abp.Application.Editions;
using CorarlERP.Editions;
using CorarlERP.Promotions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Abp.Localization;
using CorarlERP.Deposits.Dto;
using CorarlERP.MultiTenancy;

namespace CorarlERP.PackageEditions
{
    [AbpAuthorize]
    public class PackageAppService : CorarlERPAppServiceBase, IPackageAppService
    {
        private readonly ICorarlRepository<Package, Guid> _packageRepository;//repository
        private readonly ICorarlRepository<Feature, Guid> _featureRepository;//repository
        private readonly ICorarlRepository<Edition, int> _editionRepository;//repository
        private readonly ICorarlRepository<PackageEdition, Guid> _packageEditionRepository;//repository
        private readonly ICorarlRepository<PackageEditionFeature, Guid> _packageEditionFeatureRepository;//repository
        //private readonly ICorarlRepository<PackageEditionPromotion, Guid> _packageEditionPromotionRepository;//repository
        private readonly ICorarlRepository<Promotion, Guid> _promotionRepository;//repository

        public PackageAppService(
            ICorarlRepository<Package, Guid> packageRepository,
            ICorarlRepository<Feature, Guid> featureRepository,
            ICorarlRepository<Edition, int> editionRepository,
            ICorarlRepository<PackageEdition, Guid> packageEditionRepository,
            ICorarlRepository<PackageEditionFeature, Guid> packageEditionFeatureRepository,
            //ICorarlRepository<PackageEditionPromotion, Guid> packageEditionPromotionRepository,
            ICorarlRepository<Promotion, Guid> promotionRepository
        )
        {
            _packageRepository = packageRepository;
            _featureRepository = featureRepository;
            _editionRepository = editionRepository;
            _packageEditionRepository = packageEditionRepository;
            _packageEditionFeatureRepository = packageEditionFeatureRepository;
            //_packageEditionPromotionRepository = packageEditionPromotionRepository;
            _promotionRepository = promotionRepository;
        }

        private async Task CheckDuplicate(PackageDto input)
        {
            if (input.Code.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("IsRequired", L("Code")));

            var duplicateCode = await _packageRepository.GetAll().AsNoTracking().AnyAsync(s => s.Id != input.Id && s.Code.ToLower() == input.Code.ToLower());
            if (duplicateCode) throw new UserFriendlyException(L("Duplicated", L("Code")));

            if (input.Name.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("IsRequired", L("Name")));

            var duplicate = await _packageRepository.GetAll().AsNoTracking().AnyAsync(s => s.Id != input.Id && s.Name.ToLower() == input.Name.ToLower());
            if (duplicate) throw new UserFriendlyException(L("Duplicated", L("PackageName")));
        }

        private async Task ValidateInput(PackageDto input)
        {
            if (input.Editions.IsNullOrEmpty()) throw new UserFriendlyException(L("IsRequired", L("Editions")));
            if (input.Features.IsNullOrEmpty()) throw new UserFriendlyException(L("IsRequired", L("Features")));
            if (input.EditionFeatureValues.IsNullOrEmpty() ||
                input.EditionFeatureValues.Any(s => s.Value.IsNullOrWhiteSpace()) ||
                input.EditionFeatureValues.GroupBy(s => s.FeatureId).Count() != input.Features.Count ||
                input.EditionFeatureValues.GroupBy(s => s.EditionId).Count() != input.Editions.Count ||
                input.EditionFeatureValues.Count != input.Editions.Count * input.Features.Count) throw new UserFriendlyException(L("IsRequired", L("FeatureValue")));

            if (input.EditionFeatureValues.Any(s => !input.Features.Any(r => s.FeatureId == r.FeatureId)) ||
                input.EditionFeatureValues.Any(s => !input.Editions.Any(r => s.EditionId == r.EditionId)) ||
                input.EditionFeatureValues.GroupBy(s => s.FeatureId).Any(s => s.Count() != input.Editions.Count || s.GroupBy(g => g.EditionId).Any(e => e.Count() > 1))) throw new UserFriendlyException(L("IsNotValid", L("FeatureValue")));

            var duplicateEdition = input.Editions.GroupBy(s => s.EditionId).Where(s => s.Count() > 1).Select(s => s.FirstOrDefault()?.EditionName).FirstOrDefault();
            if (duplicateEdition != null) throw new UserFriendlyException(L("Duplicated", L("Editions") + $" : {duplicateEdition}"));

            var duplicateFeature = input.Features.GroupBy(s => s.FeatureId).Where(s => s.Count() > 1).Select(s => s.FirstOrDefault()?.FeatureName).FirstOrDefault();
            if (duplicateFeature != null) throw new UserFriendlyException(L("Duplicated", L("Features") + $" : {duplicateFeature}"));

            var findEdition = await _editionRepository.GetAll().AsNoTracking().Where(s => input.Editions.Any(r => r.EditionId == s.Id)).CountAsync() == input.Editions.Count;
            if (!findEdition) throw new UserFriendlyException(L("IsNotValid", L("Edition")));

            var findFeature = await _featureRepository.GetAll().AsNoTracking().Where(s => input.Features.Any(r => r.FeatureId == s.Id)).CountAsync() == input.Features.Count;
            if (!findEdition) throw new UserFriendlyException(L("IsNotValid", L("Feature")));

            //var discounts = input.Discounts == null ? null : input.Discounts.Where(s => s.PromotionId.HasValue).ToList();
            //if (!discounts.IsNullOrEmpty())
            //{
            //    var invalidEdition = discounts.Any(s => !input.Editions.Any(r => r.EditionId == s.EditionId));
            //    if (invalidEdition) throw new UserFriendlyException(L("IsNotValid", L("Edition")));

            //    var findPromotion = await _promotionRepository.GetAll().AsNoTracking()
            //                             .Where(s => (input.Id.HasValue || s.IsActive) && 
            //                                         s.PromotionType == PromotionType.Discount && 
            //                                         discounts.Any(r => s.Id == r.PromotionId)).CountAsync() == discounts.GroupBy(g => g.PromotionId).Count();
            //    if(!findPromotion) throw new UserFriendlyException(L("IsNotValid", L("Promotion")));
            //}

            //var freeExtraMonths = input.FreeExtraMonths == null ? null : input.FreeExtraMonths.Where(s => s.PromotionId.HasValue).ToList();
            //if (!freeExtraMonths.IsNullOrEmpty())
            //{
            //    var invalidEdition = freeExtraMonths.Any(s => !input.Editions.Any(r => r.EditionId == s.EditionId));
            //    if (invalidEdition) throw new UserFriendlyException(L("IsNotValid", L("Edition")));

            //    var findPromotion = await _promotionRepository.GetAll().AsNoTracking()
            //                             .Where(s => (input.Id.HasValue || s.IsActive) &&
            //                                         s.PromotionType == PromotionType.FreeExtraMonth &&
            //                                         freeExtraMonths.Any(r => s.Id == r.PromotionId)).CountAsync() == freeExtraMonths.GroupBy(g => g.PromotionId).Count();
            //    if (!findPromotion) throw new UserFriendlyException(L("IsNotValid", L("Promotion")));
            //}
        }


        [AbpAuthorize(AppPermissions.Pages_Host_Client_Packages_Create)]
        public async Task Create(PackageDto input)
        {
            await CheckDuplicate(input);
            await ValidateInput(input);

            var userId = AbpSession.GetUserId();

            var @entity = Package.Create(userId, input.Code, input.Name, input.Description, input.SortOrder);
            await _packageRepository.BulkInsertAsync(@entity);

            var addEditions = input.Editions.Select(s => PackageEdition.Create(userId, entity.Id, s.EditionId, s.SortOrder, s.AnnualPrice)).ToList();
            await _packageEditionRepository.BulkInsertAsync(addEditions);

            var addFeatures = input.EditionFeatureValues.Select(s => PackageEditionFeature.Create(userId, entity.Id, s.EditionId, s.FeatureId, s.Value)).ToList();
            await _packageEditionFeatureRepository.BulkInsertAsync(addFeatures);


            //var addPromotions = new List<PackageEditionPromotion>();
            //var discounts = input.Discounts == null ? null : input.Discounts.Where(s => s.PromotionId.HasValue).ToList();
            //if (!discounts.IsNullOrEmpty())
            //{
            //    var promotions = discounts.Select(s => PackageEditionPromotion.Create(userId, entity.Id, s.EditionId, s.PromotionId.Value)).ToList();
            //    addPromotions.AddRange(promotions);
            //}

            //var freeExtraMonths = input.FreeExtraMonths == null ? null : input.FreeExtraMonths.Where(s => s.PromotionId.HasValue).ToList();
            //if (!freeExtraMonths.IsNullOrEmpty())
            //{
            //    var promotions = freeExtraMonths.Select(s => PackageEditionPromotion.Create(userId, entity.Id, s.EditionId, s.PromotionId.Value)).ToList();
            //    addPromotions.AddRange(promotions);
            //}

            //if(addPromotions.Any()) await _packageEditionPromotionRepository.BulkInsertAsync(addPromotions);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_Packages_Find)]
        public async Task<ListResultDto<GetPackageListDto>> Find(GetPackageListInput input)
        {
            var @query = _packageRepository
                .GetAll()
                .AsNoTracking()
                .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                .WhereIf(
                    !input.Filter.IsNullOrEmpty(),
                    p => p.Name.ToLower().Contains(input.Filter.ToLower()))
                .Select(s => new GetPackageListDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Code = s.Code,
                    Description = s.Description,
                    SortOrder = s.SortOrder,
                    IsActive = s.IsActive
                });

            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .ToListAsync();

            return new ListResultDto<GetPackageListDto> { Items = entities };
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_Packages_Update, AppPermissions.Pages_Host_Client_Packages_GetDetail)]
        public async Task<PackageDto> GetDetail(EntityDto<Guid> input)
        {
            var @entity = await _packageRepository.GetAll().AsNoTracking()
                                .Where(s => s.Id == input.Id)
                                .Select(s => new PackageDto
                                {
                                    Id = s.Id,
                                    Code = s.Code,
                                    Name = s.Name,
                                    Description = s.Description,
                                    SortOrder = s.SortOrder,
                                    IsActive = s.IsActive
                                })
                                .FirstOrDefaultAsync();

            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));

            var featureValues = await _packageEditionFeatureRepository.GetAll().AsNoTracking()
                                      .Where(s => s.PackageId == input.Id)
                                      .Select(s => new PackageEditionFeatureDto
                                      {
                                          Id = s.Id,
                                          EditionId = s.EditionId,
                                          FeatureId = s.FeatureId,
                                          FeatureName = s.Feature.Name,
                                          SortOrder = s.Feature.SortOrder,
                                          Value = s.Value
                                      })
                                      .OrderBy(s => s.SortOrder)
                                      .ToListAsync();

            entity.EditionFeatureValues = featureValues;

            entity.Features = featureValues.GroupBy(s => new 
            {
                FeatureId = s.FeatureId,
                FeatureName = s.FeatureName,
                SortOrder = s.SortOrder
            })
            .Select(s => new PackageFeatureDto
            {
                FeatureId = s.Key.FeatureId,
                FeatureName = s.Key.FeatureName,
                SortOrder = s.Key.SortOrder                
            })
            .ToList();

            var editions = await _packageEditionRepository.GetAll().AsNoTracking()
                                 .Where(s => s.PackageId == input.Id)
                                 .Select(s => new PackageEditionDto
                                 {
                                     Id = s.Id,
                                     EditionId = s.EditionId,
                                     EditionName = s.Edition.DisplayName,
                                     SortOrder = s.SortOrder,
                                     AnnualPrice = s.AnnualPrice
                                 })
                                 .OrderBy(s => s.SortOrder)
                                 .ToListAsync();

            entity.Editions = editions;

            //var promotions = await _packageEditionPromotionRepository.GetAll().AsNoTracking()
            //                       .Where(s => s.PackageId == input.Id)
            //                       .Select(s => new 
            //                       {
            //                           Id = s.Id,
            //                           PromotionId = s.Promotion.Id,
            //                           PromotionName = s.Promotion.PromotionName,
            //                           EditionId = s.EditionId,
            //                           s.Promotion.PromotionType
            //                       })
            //                       .ToListAsync();

            //if (promotions.Any())
            //{
            //    entity.Discounts = promotions.Where(s => s.PromotionType == PromotionType.Discount).Select(s => new PackageEditionPromotionDto
            //    {
            //        Id = s.Id,
            //        PromotionId = s.PromotionId,
            //        PromotionName = s.PromotionName,
            //        EditionId = s.EditionId
            //    })
            //    .ToList();

            //    entity.FreeExtraMonths = promotions.Where(s => s.PromotionType == PromotionType.FreeExtraMonth).Select(s => new PackageEditionPromotionDto
            //    {
            //        Id = s.Id,
            //        PromotionId = s.PromotionId,
            //        PromotionName = s.PromotionName,
            //        EditionId = s.EditionId
            //    })
            //    .ToList();

            //}

            return entity;
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_Packages_Update)]
        public async Task Update(PackageDto input)
        {
            await CheckDuplicate(input);
            await ValidateInput(input);

            var userId = AbpSession.GetUserId();

            var @entity = await _packageRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFond"));

            @entity.Update(userId, input.Code, input.Name, input.Description, input.SortOrder);
            await _packageRepository.BulkUpdateAsync(@entity);

            var editions = await _packageEditionRepository.GetAll().AsNoTracking().Where(s => s.PackageId == input.Id).ToListAsync();
            var addEditions = input.Editions.Where(s => !s.Id.HasValue || s.Id == Guid.Empty).ToList();
            var updateEditions = input.Editions.Where(s => s.Id.HasValue && s.Id != Guid.Empty).ToList();
            var deleteEditions = editions.Where(s => !updateEditions.Any(r => s.Id == r.Id)).ToList();

            if (addEditions.Any())
            {
                var addPackageEditions = addEditions.Select(s => PackageEdition.Create(userId, entity.Id, s.EditionId, s.SortOrder, s.AnnualPrice)).ToList();
                await _packageEditionRepository.BulkInsertAsync(addPackageEditions);
            }

            if (updateEditions.Any())
            {
                var updatePackageEditions = new List<PackageEdition>();
                foreach(var i in updateEditions)
                {
                    var edition = editions.FirstOrDefault(s => s.Id == i.Id);
                    if (edition == null) throw new UserFriendlyException(L("RecordNotFond"));

                    edition.Update(userId, entity.Id, i.EditionId, i.SortOrder, i.AnnualPrice);
                    updatePackageEditions.Add(edition);
                }

                await _packageEditionRepository.BulkUpdateAsync(updatePackageEditions);
            }

            if (deleteEditions.Any()) await _packageEditionRepository.BulkDeleteAsync(deleteEditions);


            var featureValues = await _packageEditionFeatureRepository.GetAll().AsNoTracking().Where(s => s.PackageId == input.Id).ToListAsync();
            var addFeatureValues = input.EditionFeatureValues.Where(s => !s.Id.HasValue || s.Id == Guid.Empty).ToList();
            var updateFeatureValues = input.EditionFeatureValues.Where(s => s.Id.HasValue && s.Id != Guid.Empty).ToList();
            var deleteFeatureValues = featureValues.Where(s => !updateFeatureValues.Any(r => s.Id == r.Id)).ToList();

            if (addFeatureValues.Any())
            {
                var addFeatures = addFeatureValues.Select(s => PackageEditionFeature.Create(userId, entity.Id, s.EditionId, s.FeatureId, s.Value)).ToList();
                await _packageEditionFeatureRepository.BulkInsertAsync(addFeatures);
            }

            if (updateFeatureValues.Any())
            {
                var updateFeatures = new List<PackageEditionFeature>();
                foreach(var i in updateFeatureValues)
                {
                    var feature = featureValues.FirstOrDefault(s => s.Id == i.Id);
                    if (feature == null) throw new UserFriendlyException(L("RecordNotFond"));
                    feature.Update(userId, entity.Id, i.EditionId, i.FeatureId, i.Value);
                    updateFeatures.Add(feature);
                }

                await _packageEditionFeatureRepository.BulkUpdateAsync(updateFeatures);
            }

            if (deleteFeatureValues.Any()) await _packageEditionFeatureRepository.BulkDeleteAsync(deleteFeatureValues);

            //var promotions = await _packageEditionPromotionRepository.GetAll().AsNoTracking().Where(s => s.PackageId == input.Id).ToListAsync();

            //var addPromotions = new List<PackageEditionPromotion>();
            //var updatePromotions = new List<PackageEditionPromotion>();
            
            //var discounts = input.Discounts == null ? null : input.Discounts.Where(s => s.PromotionId.HasValue).ToList();           
            //if (!discounts.IsNullOrEmpty())
            //{
            //    var addDiscounts = discounts.Where(s => !s.Id.HasValue || s.Id == Guid.Empty).ToList();
            //    var updateDiscounts = discounts.Where(s => s.Id.HasValue && s.Id != Guid.Empty).ToList();

            //    if (addDiscounts.Any())
            //    {
            //        var discountPromotions = addDiscounts.Select(s => PackageEditionPromotion.Create(userId, entity.Id, s.EditionId, s.PromotionId.Value)).ToList();
            //        addPromotions.AddRange(discountPromotions);
            //    }

            //    if (updateDiscounts.Any())
            //    {
            //        foreach(var i in updateDiscounts)
            //        {
            //            var discount = promotions.FirstOrDefault(s => s.Id == i.Id);
            //            if (discount == null) throw new UserFriendlyException(L("RecordNotFond"));
            //            discount.Update(userId, entity.Id, i.EditionId, i.PromotionId.Value);
            //            updatePromotions.Add(discount);
            //        }
            //    }
            //}

            //var freeExtraMonths = input.FreeExtraMonths == null ? null : input.FreeExtraMonths.Where(s => s.PromotionId.HasValue).ToList();
            //if (!freeExtraMonths.IsNullOrEmpty())
            //{
            //    var addExtraMonths = freeExtraMonths.Where(s => !s.Id.HasValue || s.Id == Guid.Empty).ToList();
            //    var updateExtraMonths = freeExtraMonths.Where(s => s.Id.HasValue && s.Id != Guid.Empty).ToList();

            //    if (addExtraMonths.Any())
            //    {
            //        var extraPromotions = addExtraMonths.Select(s => PackageEditionPromotion.Create(userId, entity.Id, s.EditionId, s.PromotionId.Value)).ToList();
            //        addPromotions.AddRange(extraPromotions);
            //    }

            //    if (updateExtraMonths.Any())
            //    {
            //        foreach (var i in updateExtraMonths)
            //        {
            //            var extraMonth = promotions.FirstOrDefault(s => s.Id == i.Id);
            //            if (extraMonth == null) throw new UserFriendlyException(L("RecordNotFond"));
            //            extraMonth.Update(userId, entity.Id, i.EditionId, i.PromotionId.Value);
            //            updatePromotions.Add(extraMonth);
            //        }
            //    }
            //}
                        

            //var deletePromotions = promotions.Where(s => !updatePromotions.Any(r => r.Id == s.Id)).ToList();

            //if (deletePromotions.Any()) await _packageEditionPromotionRepository.BulkDeleteAsync(deletePromotions);
            //if (addPromotions.Any()) await _packageEditionPromotionRepository.BulkInsertAsync(addPromotions);
            //if (updatePromotions.Any()) await _packageEditionPromotionRepository.BulkUpdateAsync(updatePromotions);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_Packages_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            var @entity = await _packageRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFond"));

            //var promotions = await _packageEditionPromotionRepository.GetAll().AsNoTracking().Where(s => s.PackageId == input.Id).ToListAsync();
            //if (promotions.Any()) await _packageEditionPromotionRepository.BulkDeleteAsync(promotions);

            var featureValues = await _packageEditionFeatureRepository.GetAll().AsNoTracking().Where(s => s.PackageId == input.Id).ToListAsync();
            if (featureValues.Any()) await _packageEditionFeatureRepository.BulkDeleteAsync(featureValues);

            var editions = await _packageEditionRepository.GetAll().AsNoTracking().Where(s => s.PackageId == input.Id).ToListAsync();
            if (editions.Any()) await _packageEditionRepository.BulkDeleteAsync(editions);

            await _packageRepository.BulkDeleteAsync(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_Packages_Enable)]
        public async Task Enable(EntityDto<Guid> input)
        {
            var @entity = await _packageRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFond"));

            entity.Enable(AbpSession.UserId.Value, true);

            await _packageRepository.BulkUpdateAsync(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_Packages_Disable)]
        public async Task Disable(EntityDto<Guid> input)
        {
            var @entity = await _packageRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFond"));

            entity.Enable(AbpSession.UserId.Value, false);

            await _packageRepository.BulkUpdateAsync(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_Packages_GetList)]
        public async Task<PagedResultDto<GetPackageListDto>> GetList(GetPackageListInput input)
        {
            var @query = _packageRepository
                .GetAll()
                .AsNoTracking()
                .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                .WhereIf(
                    !input.Filter.IsNullOrEmpty(),
                    p => p.Name.ToLower().Contains(input.Filter.ToLower()) ||
                         p.Code.ToLower().Contains(input.Filter.ToLower()))
                .Select(s => new GetPackageListDto
                {
                    Id = s.Id,
                    Code = s.Code,
                    Name = s.Name,
                    Description = s.Description,
                    SortOrder = s.SortOrder,
                    IsActive = s.IsActive
                });

            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<GetPackageListDto>(resultCount, @entities);
        }


        [AbpAuthorize(AppPermissions.Pages_Host_Client_Packages_Find)]
        public async Task<ListResultDto<PackageEditionDto>> GetPackageEditions(EntityDto<Guid> input)
        {
            var editions = await _packageEditionRepository.GetAll().AsNoTracking()
                                .Where(s => s.PackageId == input.Id)
                                .Select(s => new PackageEditionDto
                                {
                                    Id = s.Id,
                                    EditionId = s.EditionId,
                                    EditionName = s.Edition.DisplayName,
                                    SortOrder = s.SortOrder
                                })
                                .OrderBy(s => s.SortOrder)
                                .ToListAsync();

            return new ListResultDto<PackageEditionDto>(editions);
        }
    }
}
