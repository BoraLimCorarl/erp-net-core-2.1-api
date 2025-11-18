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
using Abp.Application.Editions;
using Amazon.Lambda.Model;
using CorarlERP.Promotions.Dot;
using EvoPdf.HtmlToPdfClient;
using CorarlERP.PackageEditions;
using CorarlERP.MultiTenancy;
using CorarlERP.Subscriptions;

namespace CorarlERP.Promotions
{
    [AbpAuthorize]
    public class PromotionCampaignAppService : CorarlERPAppServiceBase, IPromotionCampaignAppService
    {
        private readonly ICorarlRepository<Promotion, Guid> _packagePromotionRepository;//repository
        private readonly ICorarlRepository<PromotionCampaign, Guid> _promotionCampaignRepository;//repository
        private readonly ICorarlRepository<PromotionCampaignEdition, Guid> _promotionCampaignEditionRepository;//repository
        private readonly ICorarlRepository<SubscriptionPromotion, Guid> _subscriptionPromotionRepository;//repository
        private readonly ICorarlRepository<Edition, int> _editionRepository;//repository
        private readonly ICorarlRepository<Package, Guid> _packageRepository;//repository
        private readonly ICorarlRepository<PackageEdition, Guid> _packageEditionRepository;//repository

        public PromotionCampaignAppService(
            ICorarlRepository<Promotion, Guid> packagePromotionRepository,
            ICorarlRepository<PromotionCampaign, Guid> promotionCampaignRepository,
            ICorarlRepository<PromotionCampaignEdition, Guid> promotionCampaignEditionRepository,
            ICorarlRepository<Edition, int> editionRepository,
            ICorarlRepository<Package, Guid> packageRepository,
            ICorarlRepository<PackageEdition, Guid> packageEditionRepository,
            ICorarlRepository<SubscriptionPromotion, Guid> subscriptionPromotionRepository
        )
        {
            _packagePromotionRepository = packagePromotionRepository;
            _promotionCampaignRepository = promotionCampaignRepository;
            _promotionCampaignEditionRepository = promotionCampaignEditionRepository;
            _editionRepository = editionRepository;
            _packageRepository = packageRepository;
            _packageEditionRepository = packageEditionRepository;
            _subscriptionPromotionRepository = subscriptionPromotionRepository;
        }

        private async Task ValidateInput(PromotionCampaignDto input)
        {
            if (input.Name.IsNullOrEmpty()) throw new UserFriendlyException(L("IsRequired", L("Name")));
            if (input.StartDate == DateTime.MinValue) throw new UserFriendlyException(L("PleaseSelect", L("StartDate")));
            if (input.EndDate == DateTime.MinValue) throw new UserFriendlyException(L("PleaseSelect", L("EndDate")));

            if (input.IsSpecificPackage)
            {
                if (!input.PackageId.HasValue || input.PackageId == Guid.Empty) throw new UserFriendlyException(L("IsRequired", L("Package")));

                if (input.PromotionEditions.IsNullOrEmpty() || !input.PromotionEditions.Any(s => s.PromotionId.HasValue && s.PromotionId != Guid.Empty)) throw new UserFriendlyException(L("IsRequired", L("Promotions")));

                var editionGroups = input.PromotionEditions.GroupBy(g => g.EditionId);

                if (editionGroups.Any(s => s.Count() > 1)) throw new UserFriendlyException(L("Duplicated", L("Editions")));

                var findPackage = await _packageRepository.GetAll().AsNoTracking().AnyAsync(s => s.Id == input.PackageId);
                if (!findPackage) throw new UserFriendlyException(L("IsNotValid", L("Edition")));

                var editonIds = editionGroups.Select(s => s.Key).ToList();
                var findEditon = await _editionRepository.GetAll().AsNoTracking().Where(s => editonIds.Contains(s.Id)).CountAsync() == editonIds.Count;
                if (!findEditon) throw new UserFriendlyException(L("IsNotValid", L("Edition")));

                //Celar data
                input.PromotionId = null;
            }
            else
            {
                if(!input.PromotionId.HasValue || input.PromotionId == Guid.Empty) throw new UserFriendlyException(L("IsRequired", L("Promotions")));

                var findPromotion = await _packagePromotionRepository.GetAll().AsNoTracking().Where(s => s.Id == input.PromotionId).AnyAsync();
                if (!findPromotion) throw new UserFriendlyException(L("IsNotValid", L("Promotion")));

                //Clean data
                input.PackageId = null;
                input.PromotionEditions = null;
            }

        }


        [AbpAuthorize(AppPermissions.Pages_Host_Client_PackagePromotions_Create)]
        public async Task Create(PromotionCampaignDto input)
        {
            await ValidateInput(input);

            var userId = AbpSession.GetUserId();

            var @entity = PromotionCampaign.Create(userId, input.Name, input.CampaignType, input.StartDate, input.EndDate, input.NeverEnd, input.IsSpecificPackage, input.PackageId, input.PromotionId, input.Description, input.IsRenewable, input.IsEligibleWithOther);
            await _promotionCampaignRepository.BulkInsertAsync(@entity);

            if (input.IsSpecificPackage)
            {   
                var editions = input.PromotionEditions.Select(s => PromotionCampaignEdition.Create(userId, entity.Id, s.EditionId, s.SortOrder, s.PromotionId)).ToList();
                await _promotionCampaignEditionRepository.BulkInsertAsync(editions);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_PackagePromotions_Find)]
        public async Task<ListResultDto<GetPromotionCampaignListDto>> Find(FindPromotionCampaignInput input)
        {
            var @query = from p in _promotionCampaignRepository
                                    .GetAll()
                                    .AsNoTracking()
                                    .WhereIf(input.Date.HasValue, s => s.StartDate.Date <= input.Date.Value.Date && (s.NeverEnd || input.Date.Value.Date <= s.EndDate.Value.Date))
                                    .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                                    .WhereIf(input.CampaignType.HasValue, s => s.CampaignType == input.CampaignType)
                                    .WhereIf(!input.Filter.IsNullOrEmpty(), p =>
                                        (p.PromotionId.HasValue && p.Promotion.PromotionName.ToLower().Contains(input.Filter.ToLower())) ||
                                        (p.PackageId.HasValue && p.Package.Name.ToLower().Contains(input.Filter.ToLower()))
                                    )                                   
                         join e in _promotionCampaignEditionRepository.GetAll().AsNoTracking()
                         on p.Id equals e.PromotionCampaignId
                         into editons
                         where !p.IsSpecificPackage ||
                               !input.EditionId.HasValue || editons.Any(s => s.EditionId == input.EditionId && s.PromotionId.HasValue && (input.IsActive == null || s.Promotion.IsActive == input.IsActive))
                         select new GetPromotionCampaignListDto
                         {
                             Id = p.Id,
                             Name = p.Name,
                             CampaignType = p.CampaignType,
                             StartDate = p.StartDate,
                             EndDate = p.EndDate,
                             NeverEnd = p.NeverEnd,
                             IsRenewable = p.IsRenewable,
                             IsEligibleWithOther = p.IsEligibleWithOther,
                             IsSpecificPackage = p.IsSpecificPackage,
                             PackageId = p.PackageId,
                             PackageName = p.PackageId.HasValue ? p.Package.Name : "",
                             PromotionId = p.PromotionId,
                             PromotionName = p.PromotionId.HasValue ? p.Promotion.PromotionName : "",
                             Description = p.Description,
                             IsActive = p.IsActive,
                         };

             var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .ToListAsync();

            return new ListResultDto<GetPromotionCampaignListDto> { Items = entities };
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_PackagePromotions_Update, AppPermissions.Pages_Host_Client_PackagePromotions_GetDetail)]
        public async Task<PromotionCampaignDto> GetDetail(EntityDto<Guid> input)
        {
            var @entity = await _promotionCampaignRepository.GetAll().AsNoTracking()
                                .Where(s => s.Id == input.Id)
                                .Select(s => new PromotionCampaignDto
                                {
                                    Id = s.Id,
                                    Name = s.Name,
                                    CampaignType = s.CampaignType,
                                    CampaignTypeName = s.CampaignType.ToString(),
                                    Description = s.Description,
                                    StartDate = s.StartDate,
                                    EndDate = s.EndDate,
                                    NeverEnd = s.NeverEnd,
                                    IsRenewable = s.IsRenewable,
                                    IsEligibleWithOther = s.IsEligibleWithOther,
                                    IsSpecificPackage = s.IsSpecificPackage,
                                    PackageId = s.PackageId,
                                    PackageName = s.PackageId.HasValue ? s.Package.Name : "",
                                    PromotionId = s.PromotionId,
                                    PromotionName = s.PromotionId.HasValue ? s.Promotion.PromotionName : "",
                                    IsActive = s.IsActive,
                                })
                                .FirstOrDefaultAsync();

            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));            

            if (entity.IsSpecificPackage)
            {
                entity.PromotionEditions = await _promotionCampaignEditionRepository.GetAll().AsNoTracking()
                                        .Where(s => s.PromotionCampaignId == input.Id)
                                        .Select(s => new PromotionCampaignEditionDto
                                        {
                                            Id = s.Id,
                                            EditionId = s.EditionId,
                                            EditionName = s.Edition.DisplayName,
                                            SortOrder = s.SortOrder,
                                            PromotionId = s.PromotionId,
                                            PromotionName = s.Promotion.PromotionName
                                        })
                                        .OrderBy(s => s.SortOrder)
                                        .ToListAsync();                
            }
            else
            {
                entity.PromotionEditions = new List<PromotionCampaignEditionDto>();
            }

            return entity;
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_PackagePromotions_Update)]
        public async Task Update(PromotionCampaignDto input)
        {
            await ValidateInput(input);

            var userId = AbpSession.GetUserId();

            var @entity = await _promotionCampaignRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == input.Id.Value);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFond"));

            @entity.Update(userId, input.Name, input.CampaignType, input.StartDate, input.EndDate, input.NeverEnd, input.IsSpecificPackage, input.PackageId, input.PromotionId, input.Description, input.IsRenewable, input.IsEligibleWithOther);
            await _promotionCampaignRepository.BulkUpdateAsync(@entity);

            var editionPromotions = await _promotionCampaignEditionRepository.GetAll().AsNoTracking().Where(s => s.PromotionCampaignId == input.Id).ToListAsync();
            var addEditions = new List<PromotionCampaignEdition>();
            var updateEditions = new List<PromotionCampaignEdition>();

            if (input.IsSpecificPackage)
            {
                var addPromotions = input.PromotionEditions.Where(s => !s.Id.HasValue || s.Id == Guid.Empty).ToList();
                var updatePromotions = input.PromotionEditions.Where(s => s.Id.HasValue && s.Id != Guid.Empty).ToList();

                if (addPromotions.Any()) addEditions = addPromotions.Select(s => PromotionCampaignEdition.Create(userId, entity.Id, s.EditionId, s.SortOrder, s.PromotionId)).ToList();
                
                if (updatePromotions.Any())
                {
                    foreach(var i in updatePromotions)
                    {
                        var promotion = editionPromotions.FirstOrDefault(s => s.Id == i.Id);
                        if (promotion == null) throw new UserFriendlyException(L("RecordNotFond"));
                        promotion.Update(userId, entity.Id, i.EditionId, i.SortOrder, i.PromotionId);
                        updateEditions.Add(promotion);
                    }
                }
            }

            var deleteEditions = editionPromotions.WhereIf(!updateEditions.IsNullOrEmpty(), s => !updateEditions.Any(r => r.Id == s.Id)).ToList();
            if (deleteEditions.Any()) await _promotionCampaignEditionRepository.BulkDeleteAsync(deleteEditions);
            if (addEditions.Any()) await _promotionCampaignEditionRepository.BulkInsertAsync(addEditions);
            if (updateEditions.Any()) await _promotionCampaignEditionRepository.BulkUpdateAsync(updateEditions);

        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_PackagePromotions_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            var @entity = await _promotionCampaignRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFond"));

            var useInSubscription = await _subscriptionPromotionRepository.GetAll().AsNoTracking().AnyAsync(s => s.CampaignId == input.Id);
            if (useInSubscription) throw new UserFriendlyException(L("IsInUse", L("Promotion")));

                var editions = await _promotionCampaignEditionRepository.GetAll().AsNoTracking().Where(s => s.PromotionCampaignId == input.Id).ToListAsync();
            if (editions.Any()) await _promotionCampaignEditionRepository.BulkDeleteAsync(editions);

            await _promotionCampaignRepository.BulkDeleteAsync(entity );
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_PackagePromotions_Enable)]
        public async Task Enable(EntityDto<Guid> input)
        {
            var @entity = await _promotionCampaignRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFond"));

            entity.Enable(AbpSession.UserId.Value, true);

            await _promotionCampaignRepository.BulkUpdateAsync(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_PackagePromotions_Disable)]
        public async Task Disable(EntityDto<Guid> input)
        {
            var @entity = await _promotionCampaignRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFond"));

            entity.Enable(AbpSession.UserId.Value, false);

            await _promotionCampaignRepository.BulkUpdateAsync(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_PackagePromotions_GetList)]
        public async Task<PagedResultDto<GetPromotionCampaignListDto>> GetList(GetPromotionCampaignListInput input)
        {
            var @query = _promotionCampaignRepository
                .GetAll()
                .AsNoTracking()
                .Where(s => s.StartDate.Date <= input.ToDate.Date && (s.NeverEnd || input.FromDate.Date <= s.EndDate.Value.Date))
                .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                .WhereIf(!input.Filter.IsNullOrEmpty(), p =>
                    (p.PromotionId.HasValue && p.Promotion.PromotionName.ToLower().Contains(input.Filter.ToLower())) ||
                    (p.PackageId.HasValue && p.Package.Name.ToLower().Contains(input.Filter.ToLower()))
                )
                .Select(s => new GetPromotionCampaignListDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    CampaignType = s.CampaignType,
                    CampaignTypeName = s.CampaignType.ToString(),
                    Description = s.Description,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    NeverEnd = s.NeverEnd,
                    IsRenewable = s.IsRenewable,
                    IsEligibleWithOther = s.IsEligibleWithOther,
                    IsSpecificPackage = s.IsSpecificPackage,
                    PackageId = s.PackageId,
                    PackageName = s.PackageId.HasValue ? s.Package.Name : "",
                    PromotionId = s.PromotionId,
                    PromotionName = s.PromotionId.HasValue ? s.Promotion.PromotionName : "",
                    IsActive = s.IsActive,
                });

            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<GetPromotionCampaignListDto>(resultCount, @entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_PackagePromotions_Find)]
        public async Task<PromotionSummaryDto> GetPromotion(GetCampaignPromotionInput input)
        {
            var query = from c in _promotionCampaignRepository.GetAll().AsNoTracking()
                                  .Where(s => s.Id == input.Id)

                                join p in _promotionCampaignEditionRepository.GetAll().AsNoTracking()
                                          .Select(s => new SubscriptionCampaignPromotionInput
                                          {
                                              PromotionId = s.PromotionId.Value,
                                              PromotionName = s.PromotionId.HasValue ? s.Promotion.PromotionName : "",
                                              Value = !s.PromotionId.HasValue ? 0 : s.Promotion.PromotionType == PromotionType.Discount ? s.Promotion.DiscountRate : s.Promotion.ExtraMonth,
                                              PromotionType = s.PromotionId.HasValue ? s.Promotion.PromotionType : 0,
                                              CampaignId = s.PromotionCampaignId,
                                              EditionId = s.EditionId,
                                              EditionName = s.Edition.DisplayName,
                                              SortOrder = s.SortOrder,
                                              IsTrial = s.PromotionId.HasValue && s.Promotion.IsTrial
                                          })
                                on c.Id equals p.CampaignId
                                into pros
                                select new PromotionSummaryDto
                                {
                                    IsRenewable = c.IsRenewable,
                                    IsEligibleWithOther = c.IsEligibleWithOther,
                                    IsSpecificPackage = c.IsSpecificPackage,
                                    CampaignId = c.Id,
                                    Value = !c.PromotionId.HasValue ? 0 : c.Promotion.PromotionType == PromotionType.Discount ? c.Promotion.DiscountRate : c.Promotion.ExtraMonth,
                                    PromotionType = c.PromotionId.HasValue ? c.Promotion.PromotionType : 0,
                                    IsTrial = c.PromotionId.HasValue && c.Promotion.IsTrial,
                                    Id = c.PromotionId,
                                    CampaignEditionPromotions = pros.OrderBy(s => s.SortOrder).ToList()
                                };


            var result = await query.FirstOrDefaultAsync();

            if (result == null) throw new UserFriendlyException(L("RecordNotFound"));

            if (result.IsSpecificPackage) 
            {
                var promotion = result.CampaignEditionPromotions.Where(s => s.EditionId == input.EditionId).FirstOrDefault();
                if (promotion != null) {
                    result.Id = promotion.PromotionId;
                    result.Value = promotion.Value;
                    result.PromotionType = promotion.PromotionType;
                    result.IsTrial = promotion.IsTrial;
                    result.PromotionName = promotion.PromotionName;
                }
            }

            return result;
        }
    }
}
