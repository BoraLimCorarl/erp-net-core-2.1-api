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
using CorarlERP.Productions;
using CorarlERP.PackageEditions;
using CorarlERP.Subscriptions;


namespace CorarlERP.Promotions
{
    [AbpAuthorize]
    public class PromotionAppService : CorarlERPAppServiceBase, IPromotionAppService
    {
        private readonly ICorarlRepository<Promotion, Guid> _packagePromotionRepository;//repository
        private readonly IPromotionManager _promotionManager;
        private readonly ICorarlRepository<PromotionCampaign, Guid> _promotionCampaignRepository;//repository
        private readonly ICorarlRepository<PromotionCampaignEdition, Guid> _promotionCampaignEditionRepository;//repository
        private readonly ICorarlRepository<SubscriptionPromotion, Guid> _subscriptionPromotionRepository;//repository

        public PromotionAppService(
            IPromotionManager promotionManager,
            ICorarlRepository<Promotion, Guid> packagePromotionRepository,
            ICorarlRepository<PromotionCampaign, Guid> promotionCampaignRepository,
            ICorarlRepository<PromotionCampaignEdition, Guid> promotionCampaignEditionRepository,
            ICorarlRepository<SubscriptionPromotion, Guid> subscriptionPromotionRepository
        )
        {
            _packagePromotionRepository = packagePromotionRepository;
            _promotionManager = promotionManager;
            _promotionCampaignEditionRepository = promotionCampaignEditionRepository;
            _promotionCampaignRepository = promotionCampaignRepository;
            _subscriptionPromotionRepository = subscriptionPromotionRepository;
        }

        private async Task CheckDuplicate(PromotionDto input)
        {
            if (input.PromotionName.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("IsRequired", L("PromotionName")));

            var duplicate = await _packagePromotionRepository.GetAll().AsNoTracking().AnyAsync(s => s.Id != input.Id && s.PromotionName.ToLower() == input.PromotionName.ToLower());
            if (duplicate) throw new UserFriendlyException(L("Duplicated", L("PromotionName")));
        }


        [AbpAuthorize(AppPermissions.Pages_Host_Client_PackagePromotions_Create)]
        public async Task Create(PromotionDto input)
        {
            await CheckDuplicate(input);

            var userId = AbpSession.GetUserId();

            var @entity = Promotion.Create(userId, input.PromotionType, input.PromotionName, input.DiscountRate, input.ExtraMonth, input.IsTrial);
            await _packagePromotionRepository.BulkInsertAsync(@entity);

        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_PackagePromotions_Find)]
        public async Task<ListResultDto<GetPackagePromotionListDto>> Find(GetPromotionListInput input)
        {
            var @query = _packagePromotionRepository
                .GetAll()
                .AsNoTracking()
                .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                .WhereIf(input.PromotionType != null, p => p.PromotionType == input.PromotionType)
                .WhereIf(
                    !input.Filter.IsNullOrEmpty(),
                    p => p.PromotionName.ToLower().Contains(input.Filter.ToLower()))
                .Select(s => new GetPackagePromotionListDto
                {
                    Id = s.Id,
                    PromotionName = s.PromotionName,
                    PromotionType = s.PromotionType,
                    PromotionTypeName = s.PromotionType.ToString(),
                    DiscountRate = s.DiscountRate,
                    ExtraMonth = s.ExtraMonth,
                    IsActive = s.IsActive,
                    IsTrial = s.IsTrial
                });

            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .ToListAsync();

            return new ListResultDto<GetPackagePromotionListDto> { Items = entities };
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_PackagePromotions_Find)]
        public async Task<GetPromotionOutput> GetDefaultPromotion(GetPromotionInput input)
        {
            return await _promotionManager.GetDefaultPromotion(input);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_PackagePromotions_Update, AppPermissions.Pages_Host_Client_PackagePromotions_GetDetail)]
        public async Task<PromotionDto> GetDetail(EntityDto<Guid> input)
        {
            var @entity = await _packagePromotionRepository.GetAll().AsNoTracking()
                                .Where(s => s.Id == input.Id)
                                .Select(s => new PromotionDto
                                {
                                    Id = s.Id,
                                    PromotionName = s.PromotionName,
                                    PromotionType = s.PromotionType,
                                    PromotionTypeName = s.PromotionType.ToString(),
                                    DiscountRate = s.DiscountRate,
                                    ExtraMonth = s.ExtraMonth,
                                    IsActive = s.IsActive,
                                    IsTrial = s.IsTrial
                                })
                                .FirstOrDefaultAsync();

            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));


            return entity;
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_PackagePromotions_Update)]
        public async Task Update(PromotionDto input)
        {
            await CheckDuplicate(input);

            var userId = AbpSession.GetUserId();

            var @entity = await _packagePromotionRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == input.Id.Value);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFond"));

            @entity.Update(userId, input.PromotionType, input.PromotionName, input.DiscountRate, input.ExtraMonth, input.IsTrial);

            await _packagePromotionRepository.BulkUpdateAsync(@entity);

        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_PackagePromotions_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            var @entity = await _packagePromotionRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFond"));

            var findInCampaign = await _promotionCampaignRepository.GetAll().AsNoTracking().AnyAsync(s => s.PromotionId == input.Id);
            if (findInCampaign) throw new UserFriendlyException(L("CannotDeleteInUse", L("Promotion")));

            var findInCampaignEdition = await _promotionCampaignEditionRepository.GetAll().AsNoTracking().AnyAsync(s => s.PromotionId == input.Id);
            if (findInCampaignEdition) throw new UserFriendlyException(L("CannotDeleteInUse", L("Promotion")));

            var findInSubscription = await _subscriptionPromotionRepository.GetAll().AsNoTracking().AnyAsync(s => s.PromotionId == input.Id);
            if (findInSubscription) throw new UserFriendlyException(L("CannotDeleteInUse", L("Promotion")));

            await _packagePromotionRepository.BulkDeleteAsync(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_PackagePromotions_Enable)]
        public async Task Enable(EntityDto<Guid> input)
        {
            var @entity = await _packagePromotionRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFond"));

            entity.Enable(AbpSession.UserId.Value, true);

            await _packagePromotionRepository.BulkUpdateAsync(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_PackagePromotions_Disable)]
        public async Task Disable(EntityDto<Guid> input)
        {
            var @entity = await _packagePromotionRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFond"));

            entity.Enable(AbpSession.UserId.Value, false);

            await _packagePromotionRepository.BulkUpdateAsync(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Client_PackagePromotions_GetList)]
        public async Task<PagedResultDto<GetPackagePromotionListDto>> GetList(GetPromotionListInput input)
        {
            var @query = _packagePromotionRepository
                .GetAll()
                .AsNoTracking()
                .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                .WhereIf(input.PromotionType != null, p => p.PromotionType == input.PromotionType)
                .WhereIf(
                    !input.Filter.IsNullOrEmpty(),
                    p => p.PromotionName.ToLower().Contains(input.Filter.ToLower()))
                .Select(s => new GetPackagePromotionListDto
                {
                    Id = s.Id,
                    PromotionName = s.PromotionName,
                    PromotionType = s.PromotionType,
                    PromotionTypeName = s.PromotionType.ToString(),
                    DiscountRate = s.DiscountRate,
                    ExtraMonth = s.ExtraMonth,
                    IsActive = s.IsActive,
                    IsTrial = s.IsTrial
                });

            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<GetPackagePromotionListDto>(resultCount, @entities);
        }
    }
}
