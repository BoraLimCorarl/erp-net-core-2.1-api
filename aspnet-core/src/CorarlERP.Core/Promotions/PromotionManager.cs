using Abp.Domain.Repositories;
using CorarlERP.PackageEditions;
using CorarlERP.Promotions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Linq;
using CorarlERP.MultiTenancy;
using static CorarlERP.Authorization.Roles.StaticRoleNames;
using CorarlERP.Subscriptions;
using Abp.Extensions;
using Abp.Timing;
using IdentityServer4.Services;
using Amazon.EventBridge.Model.Internal.MarshallTransformations;
using System.Collections.Generic;
using Castle.Core.Internal;
using Abp.Linq.Extensions;

namespace CorarlERP.Productions
{
    public class PromotionManager : CorarlERPDomainServiceBase, IPromotionManager
    {
        //private readonly IRepository<PackageEditionPromotion, Guid> _packageEditionPromotionRepository;
        private readonly IRepository<PromotionCampaign, Guid> _promotionCampaignRepository;
        private readonly IRepository<PromotionCampaignEdition, Guid> _promotionCampaignEdtionRepository;
        private readonly IRepository<Promotion, Guid> _promotionRepository;
        private readonly IRepository<SubscriptionPromotion, Guid> _subscriptionPromotionRepository;
        private readonly IRepository<SubscriptionCampaignPromotion, Guid> _subscriptionCampaignPromotionRepository;
        private readonly IRepository<Subscription, Guid> _subscriptionRepository;

        public PromotionManager(
            IRepository<SubscriptionPromotion, Guid> subscriptionPromotionRepository,
            IRepository<SubscriptionCampaignPromotion, Guid> subscriptionCampaignPromotionRepository,
            IRepository<Subscription, Guid> subscriptionRepository,
            IRepository<PromotionCampaignEdition, Guid> promotionCampaignEdtionRepository,
            //IRepository<PackageEditionPromotion, Guid> packageEditionPromotionRepository,
            IRepository<PromotionCampaign, Guid> promotionCampaignRepository,
            IRepository<Promotion, Guid> promotionRepository)
        {
            _promotionCampaignEdtionRepository = promotionCampaignEdtionRepository;
            //_packageEditionPromotionRepository = packageEditionPromotionRepository;
            _promotionCampaignRepository = promotionCampaignRepository;
            _promotionRepository = promotionRepository;
            _subscriptionRepository = subscriptionRepository;
            _subscriptionPromotionRepository = subscriptionPromotionRepository;
            _subscriptionCampaignPromotionRepository = subscriptionCampaignPromotionRepository;
        }

        public async Task<GetPromotionOutput> GetDefaultPromotion(GetPromotionInput input)
        {

            if (input.SubscriptionId.HasValue && input.SubscriptionId != Guid.Empty)
            {
                var result = new GetPromotionOutput();

                var subscription = await _subscriptionRepository.GetAll().AsNoTracking()
                                  .Where(t => t.Id == input.SubscriptionId)
                                  .FirstOrDefaultAsync();

                var promotionQuery = from p in _subscriptionPromotionRepository.GetAll()
                                           .AsNoTracking()
                                           .Where(t => t.SubscriptionId == subscription.Id)
                                           .Where(s => s.IsRenewable)
                                     join e in _subscriptionCampaignPromotionRepository.GetAll().AsNoTracking()
                                               .Select(s => new SubscriptionCampaignPromotionInput
                                               {
                                                   Id = s.Id,
                                                   PromotionId = s.PromotionId,
                                                   PromotionName = s.PromotionId.HasValue ? s.Promotion.PromotionName : "",
                                                   PromotionType = s.PromotionId.HasValue ? s.Promotion.PromotionType : 0,
                                                   CampaignId = s.CampaignId,
                                                   EditionId = s.EditionId,
                                                   EditionName = s.Edition.DisplayName,
                                                   SubscriptionPromotionId = s.SubscriptionPromotionId,
                                                   SortOrder = s.SortOrder,
                                                   Value = !s.PromotionId.HasValue ? 0 : s.Promotion.PromotionType == PromotionType.Discount ? s.Promotion.DiscountRate : s.Promotion.ExtraMonth
                                               })
                                     on p.Id equals e.SubscriptionPromotionId
                                     into pros
                                     select new PromotionSummaryDto
                                     {
                                         Id = p.PromotionId,
                                         PromotionName = p.PromotionId.HasValue ? p.Promotion.PromotionName : "",
                                         Value = !p.PromotionId.HasValue ? 0 : p.Promotion.PromotionType == PromotionType.FreeExtraMonth ? p.Promotion.ExtraMonth : p.Promotion.DiscountRate,
                                         IsTrial = p.PromotionId.HasValue && p.Promotion.IsTrial,
                                         PromotionType = p.PromotionId.HasValue ? p.Promotion.PromotionType : 0,
                                         CampaignId = p.CampaignId,
                                         IsRenewable = p.IsRenewable,
                                         IsEligibleWithOther = p.IsEligibleWithOther,
                                         IsSpecificPackage = p.IsSpecificPackage,
                                         CampaignEditionPromotions = pros.OrderBy(s => s.SortOrder).ToList()
                                     };

                result.RenewablePromotions = await promotionQuery.ToListAsync();

                var exceptCampaigns = result.RenewablePromotions.Where(s => s.CampaignId.HasValue && s.CampaignId != Guid.Empty).Select(s => s.CampaignId.Value).ToList();

                if (!result.RenewablePromotions.IsNullOrEmpty() && result.RenewablePromotions.All(s => s.IsEligibleWithOther))
                {   
                    var upgradePromotion = await GetPromotionHelper(input, CampaignType.Upgrade, true, exceptCampaigns);
                    result.SpecificPackagePromotions = upgradePromotion.SpecificPackagePromotions;
                    result.Promotions = upgradePromotion.Promotions;
                }
                else
                {
                    var upgradePromotion = await GetPromotionHelper(input, CampaignType.Upgrade, null, exceptCampaigns);
                    result.SpecificPackagePromotions = upgradePromotion.SpecificPackagePromotions;
                    result.Promotions = upgradePromotion.Promotions;
                }

                return result;
            }
            else
            {
                return await GetPromotionHelper(input, CampaignType.NewRegister, null);
            }

        }


        private async Task<GetPromotionOutput> GetPromotionHelper(GetPromotionInput input, CampaignType campaignType, bool? eligibleWithOther, List<Guid> exceptCampaigns = null)
        {

            var specificQuery = from c in _promotionCampaignRepository.GetAll().AsNoTracking()
                                            .Where(s => s.IsActive)
                                            .Where(s => s.IsSpecificPackage)
                                            .Where(s => s.PackageId == input.PackageId)
                                            .Where(s => s.CampaignType == campaignType)
                                            .Where(s => s.StartDate.Date <= input.Date.Date && (s.NeverEnd || input.Date.Date <= s.EndDate.Value.Date))
                                            .WhereIf(eligibleWithOther.HasValue, s => s.IsEligibleWithOther == eligibleWithOther.Value)
                                            .WhereIf(!exceptCampaigns.IsNullOrEmpty(), s => !exceptCampaigns.Contains(s.Id))

                                join p in _promotionCampaignEdtionRepository.GetAll().AsNoTracking()
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
                                    CampaignEditionPromotions = pros.OrderBy(s => s.SortOrder).ToList()
                                };


            var specificPromotions = await specificQuery.ToListAsync();

            if (specificPromotions != null) return new GetPromotionOutput { SpecificPackagePromotions = specificPromotions };



            var promotion = await _promotionCampaignRepository.GetAll().AsNoTracking()
                            .Where(s => s.IsActive)
                            .Where(s => !s.IsSpecificPackage)
                            .Where(s => s.CampaignType == campaignType)
                            .Where(s => s.StartDate.Date <= input.Date.Date && (s.NeverEnd || input.Date.Date <= s.EndDate.Value.Date))
                            .Where(s => s.PromotionId.HasValue && s.Promotion.IsActive)
                            .WhereIf(eligibleWithOther.HasValue, s => s.IsEligibleWithOther == eligibleWithOther.Value)
                            .WhereIf(!exceptCampaigns.IsNullOrEmpty(), s => !exceptCampaigns.Contains(s.Id))
                            .Select(s => new PromotionSummaryDto
                            {
                                Id = s.PromotionId.Value,
                                PromotionName = s.Promotion.PromotionName,
                                Value = s.Promotion.PromotionType == PromotionType.Discount ? s.Promotion.DiscountRate : s.Promotion.ExtraMonth,
                                PromotionType = s.Promotion.PromotionType,
                                IsTrial = s.Promotion.IsTrial,
                                CampaignId = s.Id,
                                IsRenewable = s.IsRenewable,
                                IsEligibleWithOther = s.IsEligibleWithOther
                            })
                            .OrderByDescending(s => s.Value)
                            .FirstOrDefaultAsync();

            if (promotion != null) return new GetPromotionOutput { Promotions = new List<PromotionSummaryDto> { promotion } };


            return new GetPromotionOutput();

        }

    }
}
