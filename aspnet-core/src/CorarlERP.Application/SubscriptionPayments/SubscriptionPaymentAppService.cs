using CorarlERP.SubscriptionPayments.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using Abp.Extensions;
using Telegram.Bot.Types.InputFiles;
using Abp.Authorization;
using CorarlERP.FileStorages;
using Abp.Timing;
using Abp.Domain.Repositories;
using CorarlERP.Editions;
using CorarlERP.MultiTenancy;
using CorarlERP.Subscriptions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq;
using Abp.UI;
using OfficeOpenXml.Drawing;
using IdentityServer4.Stores;
using Abp.Application.Services.Dto;
using Abp.Domain.Uow;
using System.Transactions;
using CorarlERP.PackageEditions;
using CorarlERP.PackageEditions.Dot;
using Abp.Application.Editions;
using CorarlERP.Promotions;
using IdentityServer4.Validation;
using Abp.Linq.Extensions;
using Microsoft.AspNetCore.Mvc;
using Abp.Collections.Extensions;

namespace CorarlERP.SubscriptionPayments
{

    [AbpAuthorize]
    public class SubscriptionPaymentAppService : CorarlERPAppServiceBase, ISubscriptionPaymentAppService
    {

        private readonly IRepository<SubscribableEdition> _subscribableEditionRepository;
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IRepository<Subscription, Guid> _subscriptionRepository;
        //private readonly IRepository<CorarlSubscriptionPayment, Guid> _subscriptionPaymentRepository;
        private readonly IFileStorageManager _fileStorageManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<PackageEditionFeature, Guid> _packageEditionFeatureRepository;
        private readonly IRepository<PackageEdition, Guid> _packageEditionRepository;
        private readonly IRepository<Package, Guid> _packageRepository;
        //private readonly IRepository<PackageEditionPromotion, Guid> _packageEditionPromotionRepository;
        private readonly IRepository<SubscriptionPromotion, Guid> _subscriptionPromotionRepository;
        private readonly IRepository<SubscriptionCampaignPromotion, Guid> _subscriptionCampaignPromotionRepository;
        private readonly IRepository<PromotionCampaign, Guid> _promotionCampaignRepository;
        private readonly IRepository<PromotionCampaignEdition, Guid> _promotionCampaignEdtionRepository;
        public SubscriptionPaymentAppService(
            //IRepository<PackageEditionPromotion, Guid> packageEditionPromotionRepository,
            IRepository<SubscriptionPromotion, Guid> subscriptionPromotionRepository,
            IRepository<SubscriptionCampaignPromotion, Guid> subscriptionCampaignPromotionRepository,
            IRepository<PromotionCampaign, Guid> promotionCampaignRepository,
            IRepository<PromotionCampaignEdition, Guid> promotionCampaignEdtionRepository,
            IRepository<Package, Guid> packageRepository,
            IRepository<PackageEdition, Guid> packageEditionRepository,
            IRepository<PackageEditionFeature, Guid> packageEditionFeatureRepository,
            IUnitOfWorkManager unitOfWorkManager,
            //IRepository<CorarlSubscriptionPayment, Guid> subscriptionPaymentRepository,
            IRepository<SubscribableEdition> subscribableEditionRepository,
            IRepository<Tenant, int> tenantRepository,
            IRepository<Subscription, Guid> subScriptionRepository,
            IFileStorageManager fileStorageManager)
        {
            _fileStorageManager = fileStorageManager;
            _subscribableEditionRepository = subscribableEditionRepository;
            _tenantRepository = tenantRepository;
            _subscriptionRepository = subScriptionRepository;
            //_subscriptionPaymentRepository = subscriptionPaymentRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _packageRepository = packageRepository;
            _packageEditionRepository = packageEditionRepository;
            _packageEditionFeatureRepository = packageEditionFeatureRepository;
            //_packageEditionPromotionRepository = packageEditionPromotionRepository;
            _subscriptionPromotionRepository = subscriptionPromotionRepository;
            _subscriptionCampaignPromotionRepository = subscriptionCampaignPromotionRepository;
            _promotionCampaignEdtionRepository = promotionCampaignEdtionRepository;
            _promotionCampaignRepository = promotionCampaignRepository;
        }

        public async Task SendTelegram(SubscriptionPaymentInput input)
        {
            var tenant = await GetCurrentTenantAsync();
            var user = await GetCurrentUserAsync();
            var edition = await _packageEditionRepository.GetAll().AsNoTracking()
                                .Where(s => s.EditionId == input.Edition.EditionId)
                                .Where(s => s.PackageId == input.Edition.PackageId)
                                .Select(s => new
                                {
                                    s.EditionId,
                                    s.Edition.Name,
                                    s.Edition.DisplayName,
                                    PackageName = s.Package.Name
                                })
                                .FirstOrDefaultAsync();

            if (edition == null) throw new UserFriendlyException(L("Invalid", L("Edition") + $": {input.Edition.EditionName}"));

            Subscription subscription = await _subscriptionRepository.GetAll()
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync(s => s.Id == tenant.SubscriptionId.Value);
            if (subscription == null) throw new UserFriendlyException(L("Invalid", L("Subscription")));

            DateTime affectedDate = Clock.Now;
            DateTime endDate = affectedDate.AddYears(1);
            DurationType durationType = DurationType.Year;            
            int duration = 1;
            var durationFormat = input.Edition.FreeDayRemaining > 0 ? $"1 year + {input.Edition.FreeDayRemaining} days free" :
                                 input.Edition.SubscriptionType != SubscriptionType.Upgrade || input.Edition.DayRemaining <= 0 ? "1 year" : $"{input.Edition.DayRemaining} {L("DayRemaining")}";
            var userLabel = "Subscribed by";

            //switch (input.SubscriptionType)
            //{
            //    case SubscriptionType.Renew:
            //        userLabel = "Renewed by";
            //        if(subscription.Endate.Value.Date > Clock.Now.Date)
            //        {
            //            affectedDate = subscription.Endate.Value;
            //            endDate = affectedDate.AddYears(1);
            //        }                    
            //        break;
            //    case SubscriptionType.Upgrade:
            //        userLabel = "Upgraded by";
            //        if (input.Duration > 0)
            //        {
            //            endDate = subscription.Endate.Value;
            //            durationType = DurationType.Year;
            //            duration = input.Duration;
            //            durationFormat = $"{input.Duration} days";
            //        }
            //        break;
            //}

            //var payment = await _subscriptionPaymentRepository.GetAll().AsNoTracking()
            //                    .Where(s => s.SubscriptionDate.Date == DateTime.Today)
            //                    .Where(s => s.CreatorUserId == user.Id)
            //                    .FirstOrDefaultAsync();

            //if (payment == null)
            //{
            //    payment = CorarlSubscriptionPayment.Create(tenant.Id, user.Id, input.SubscriptionType, edition.Id, Clock.Now, affectedDate, endDate, duration, durationType, SubscriptionPaymentMethod.KHQR, packagePrice, input.Price);
            //    await _subscriptionPaymentRepository.InsertAsync(payment);
            //}
            //else
            //{
            //    payment.Update(user.Id, input.SubscriptionType, edition.Id, Clock.Now, affectedDate, endDate, duration, durationType, SubscriptionPaymentMethod.KHQR, packagePrice, input.Price);
            //    await _subscriptionPaymentRepository.UpdateAsync(payment);
            //}

            var telegramBot = new TelegramBotClient(SubscriptionBotToken);

            //var replyMarkup = new InlineKeyboardMarkup(new InlineKeyboardButton
            //{
            //    Text = "🔗 JOIN US",
            //    Url = SubscriptionChannelUrl
            //});

            var discount = input.Edition.Discounts != null ? $"Discount: <b>{input.Edition.Discounts.Sum(t => t.Value)}%</b>{Environment.NewLine}" : "";
            var freeMonth = input.Edition.UpgradeExtraMonth > 0 ? $"Free Extra Months: <b>{input.Edition.UpgradeExtraMonth} months</b>{Environment.NewLine}" : "";

            var description = $"Date: <b>{Clock.Now.ToString("yyyy-MM-dd HH:mm:ss")}</b>{Environment.NewLine}" +
                              $"Compnay Id: <b>{tenant.Id}</b>{Environment.NewLine}" +
                              $"Company Name: <b>{tenant.Name}</b>{Environment.NewLine}" +
                              $"Compnay Subdomain: <b>{tenant.TenancyName}</b>{Environment.NewLine}" +
                              $"Subscription Type: <b>{input.Edition.SubscriptionType.ToString() }</b>{Environment.NewLine}" +
                              $"Package: <b>{edition.PackageName}</b>{Environment.NewLine}" +
                              $"Edition: <b>{edition.DisplayName}</b>{Environment.NewLine}" +
                              $"Price: <b>${input.Edition.TotalPrice:#,##0.00}</b>{Environment.NewLine}" +
                              $"Duration: <b>{durationFormat}</b>{Environment.NewLine}" +
                              discount +
                              freeMonth +
                              $"{userLabel}: <b>{user.UserName}</b>";
                             

            var stream = await _fileStorageManager.DownloadTempFile(input.ReceiptFileToken);

            var inputOnlineFile = new InputOnlineFile(stream, $"Reciept");

            var sentMessage = await telegramBot.SendPhotoAsync(
                chatId: $"{SubscriptionGroup}",
                parseMode: ParseMode.Html,
                photo: inputOnlineFile,
                caption: description
            );

        }

        public async Task SendTelegramService(ServiceSubscriptionPaymentInput input)
        {
            var tenant = await GetCurrentTenantAsync();
            var user = await GetCurrentUserAsync();

            var telegramBot = new TelegramBotClient(SubscriptionBotToken);

            //var replyMarkup = new InlineKeyboardMarkup(new InlineKeyboardButton
            //{
            //    Text = "🔗 JOIN US",
            //    Url = SubscriptionChannelUrl
            //});

            var description = $"Date: <b>{Clock.Now.ToString("yyyy-MM-dd HH:mm:ss")}</b>{Environment.NewLine}" +
                              $"Compnay Id: <b>{tenant.Id}</b>{Environment.NewLine}" +
                              $"Company Name: <b>{tenant.Name}</b>{Environment.NewLine}" +
                              $"Compnay Subdomain: <b>{tenant.TenancyName}</b>{Environment.NewLine}" +
                              $"Service: <b>{input.ServiceName}</b>{Environment.NewLine}" +
                              $"Description: <b>{input.Description}</b>{Environment.NewLine}" +
                              $"User Name: <b>{user.UserName}</b>";


            var stream = await _fileStorageManager.DownloadTempFile(input.ReceiptFileToken);

            var inputOnlineFile = new InputOnlineFile(stream, $"Reciept");

            var sentMessage = await telegramBot.SendPhotoAsync(
                chatId: $"{SubscriptionGroup}",
                parseMode: ParseMode.Html,
                photo: inputOnlineFile,
                caption: description
            );

        }

        public async Task<PackageSubscriptionOutput> GetPackageSubscriptionDetail()
        {
            var tenant = await GetCurrentTenantAsync();

            var subscription = await _subscriptionRepository.GetAll().AsNoTracking()
                                   .Where(t => t.Id == tenant.SubscriptionId)
                                   .FirstOrDefaultAsync();

            var query = from p in _packageRepository.GetAll()
                                    .WhereIf(subscription.PackageId.HasValue, s => s.Id == subscription.PackageId)
                                    .AsNoTracking()                              
                        join e in _packageEditionRepository.GetAll()
                                    .AsNoTracking()
                                    .Select(s => new EditionPriceOutput
                                    {
                                        PackageId = s.PackageId,
                                        EditionId = s.EditionId,
                                        EditionName = s.Edition.DisplayName,
                                        SortOrder = s.SortOrder,
                                        PackagePrice = s.AnnualPrice
                                    })
                        on p.Id equals e.PackageId
                        into editions
                        where editions.Any(s => s.EditionId == subscription.EditionId)
                        select new PackageSubscriptionOutput
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Description = p.Description,
                            Editions = editions.OrderBy(s => s.SortOrder).ToList(),
                        };

            var package = await query.FirstOrDefaultAsync();

            if (package == null) throw new UserFriendlyException(L("IsNotValid", L("Package")));

            var promotionQuery = from p in _subscriptionPromotionRepository.GetAll()
                                          .AsNoTracking()
                                          .Where(t => t.SubscriptionId == subscription.Id)
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

            var subscriptionPromotions = await promotionQuery.ToListAsync();

            package.EditionFeatureValues = await _packageEditionFeatureRepository.GetAll().AsNoTracking()
                                                  .Where(s => s.PackageId == package.Id)
                                                  .Select(s => new PackageEditionFeatureDto
                                                  {
                                                      EditionId = s.EditionId,
                                                      FeatureId = s.FeatureId,
                                                      FeatureName = s.Feature.Name,
                                                      SortOrder = s.Feature.SortOrder,
                                                      Value = s.Value                                                     
                                                  })
                                                  .OrderBy(s => s.SortOrder)
                                                  .ToListAsync();

            package.Features = package.EditionFeatureValues
                               .GroupBy(s => new { s.FeatureId, s.FeatureName, s.SortOrder })
                               .Select(s => new PackageFeatureDto
                               {
                                   FeatureId = s.Key.FeatureId,
                                   FeatureName = s.Key.FeatureName,
                                   SortOrder = s.Key.SortOrder
                               })
                               .ToList();

            var editionIds = package.Editions.Select(e => e.EditionId).ToList();
            var packagePromotions = new List<EditionPromotionSummaryDto>();

            if (!subscriptionPromotions.Any(s => s.IsRenewable))
            {
                packagePromotions = await GetPromotionHelper(package.Id, editionIds, null);                  
            }
            else
            {
                var renewablePromotions = subscriptionPromotions.Where(s => s.IsRenewable).ToList();
                foreach(var editionid in editionIds)
                {
                    foreach(var p in renewablePromotions)
                    {
                        if (p.IsSpecificPackage && !p.CampaignEditionPromotions.IsNullOrEmpty())
                        {   
                            var find = p.CampaignEditionPromotions.FirstOrDefault(f => f.EditionId == editionid);
                            
                            if(find != null)
                            {
                                var promotion = new EditionPromotionSummaryDto
                                {
                                    Id = find.Id,
                                    PromotionName = find.PromotionName,
                                    PromotionType = find.PromotionType,
                                    CampaignId = find.CampaignId,
                                    Value = find.Value,
                                    IsTrial = find.IsTrial,
                                    IsRenewable = p.IsRenewable,
                                    IsEligibleWithOther = p.IsEligibleWithOther,
                                    EditionId = editionid,
                                };

                                packagePromotions.Add(promotion);
                            }
                        }
                        else
                        {
                            var promotion = new EditionPromotionSummaryDto
                            {
                                Id = p.Id,
                                PromotionName = p.PromotionName,
                                PromotionType = p.PromotionType,
                                CampaignId = p.CampaignId,
                                Value = p.Value,
                                IsTrial = p.IsTrial,
                                IsRenewable = p.IsRenewable,
                                IsEligibleWithOther = p.IsEligibleWithOther,
                                EditionId = editionid,
                            };

                            packagePromotions.Add(promotion);
                        }                        
                    }
                }


                var exceptCampaigns = renewablePromotions.Where(s => s.CampaignId.HasValue && s.CampaignId != Guid.Empty).Select(s => s.CampaignId.Value).ToList();

                if (renewablePromotions.All(s => s.IsEligibleWithOther))
                {
                    var morePromotions = await GetPromotionHelper(package.Id, editionIds, true, exceptCampaigns);
                    if (morePromotions.Any()) packagePromotions.AddRange(morePromotions);
                }
                else
                {
                    var morePromotions = await GetPromotionHelper(package.Id, editionIds, null, exceptCampaigns);
                    var allPromotions = packagePromotions.Concat(morePromotions).GroupBy(e => e.EditionId).ToList();

                    var applyPromotions = new List<EditionPromotionSummaryDto>();

                    foreach(var a in allPromotions)
                    {
                        var promotion = a.OrderByDescending(o => o.Value).FirstOrDefault();
                        if (promotion != null) applyPromotions.Add(promotion);
                    }

                    packagePromotions = applyPromotions;
                }

            }


            if (packagePromotions.Any())
            {
                foreach(var e in package.Editions)
                {
                    var discounts = packagePromotions.Where(s => s.EditionId == e.EditionId && s.PromotionType == PromotionType.Discount)
                                   .Select(s => new PromotionSummaryDto
                                   {
                                       Id = s.Id,
                                       PromotionName = s.PromotionName,
                                       Value = s.Value,
                                       CampaignId = s.CampaignId,
                                       IsEligibleWithOther = s.IsEligibleWithOther,
                                       IsRenewable = s.IsRenewable,
                                       PromotionType = s.PromotionType,
                                       IsTrial = s.IsTrial,
                                   })
                                   .ToList();
                    if(!discounts.IsNullOrEmpty())
                    {
                        e.Discounts = discounts;
                    }

                    var freeExtraMonths = packagePromotions.Where(s => s.EditionId == e.EditionId && s.PromotionType == PromotionType.FreeExtraMonth)
                                           .Select(s => new PromotionSummaryDto
                                           {
                                               Id = s.Id,
                                               PromotionName = s.PromotionName,
                                               Value = s.Value,
                                               CampaignId = s.CampaignId,
                                               IsEligibleWithOther = s.IsEligibleWithOther,
                                               IsRenewable = s.IsRenewable,
                                               PromotionType = s.PromotionType,
                                               IsTrial = s.IsTrial,
                                           })
                                           .ToList();
                    if (freeExtraMonths != null)
                    {
                        e.FreeExtraMonths = freeExtraMonths;
                    }
                }
            }


            var invoiceDate = subscription.InvoiceDate.HasValue ? subscription.InvoiceDate.Value.Date : subscription.StartDate.Value.Date;
            var startDate = invoiceDate > Clock.Now.Date ? invoiceDate : Clock.Now.Date;            
            var remainingDays = Convert.ToInt32((subscription.Endate.Value.ToDayEnd() - startDate).TotalDays);

            var subscriptionFreeExtraMonths = subscriptionPromotions.Where(s => s.PromotionType == PromotionType.FreeExtraMonth).ToList();

            var freeDayRemaining = 0;            
            if(!subscriptionFreeExtraMonths.IsNullOrEmpty())
            {
                var freeEndDate = subscription.StartDate.Value.AddMonths(Convert.ToInt32(subscriptionFreeExtraMonths.Sum(t => t.Value))).AddDays(-1);
                freeDayRemaining = freeEndDate.Date <= Clock.Now.Date ? 0 : Convert.ToInt32((freeEndDate.ToDayEnd() - Clock.Now.Date).TotalDays);
            }

            foreach(var e in package.Editions)
            {
                e.SubscriptionType = subscription.IsTrail ? SubscriptionType.Subscribe : subscription.EditionId == e.EditionId ? SubscriptionType.Renew : SubscriptionType.Upgrade;

                e.DayRemaining = remainingDays;
                e.FreeDayRemaining = freeDayRemaining;
                e.UpgrageDeduction = subscription.IsTrail || subscription.EditionId == e.EditionId || remainingDays <= 0 ? 0 : 
                                     Math.Round(subscription.PackagePrice / 365 * remainingDays , 2);

                var packagePrice = subscription.IsTrail || remainingDays <= 0 || e.SubscriptionType != SubscriptionType.Upgrade ? e.PackagePrice : 
                                   Math.Round(e.PackagePrice / 365 * remainingDays, 2);

                var upgradePrice = packagePrice - e.UpgrageDeduction;
                e.TotalDiscount = e.Discounts.IsNullOrEmpty() ? 0 : Math.Round(upgradePrice * e.Discounts.Sum(t => t.Value) / 100, 2);
                e.TotalPrice = upgradePrice - e.TotalDiscount;                

                if (!e.FreeExtraMonths.IsNullOrEmpty())
                {
                    var deductionMonth = subscriptionFreeExtraMonths.IsNullOrEmpty() || e.SubscriptionType == SubscriptionType.Renew || e.DayRemaining <= 0 ? 0 : subscriptionFreeExtraMonths.Sum(t => t.Value);
                    var totalFreeMonth = e.FreeExtraMonths.Sum(t => t.Value);
                    e.UpgradeExtraMonth = totalFreeMonth > deductionMonth ? totalFreeMonth - deductionMonth : 0;
                }
            }


            return package;
        }


        private async Task<List<EditionPromotionSummaryDto>> GetPromotionHelper(Guid packageId, List<int> editionIds, bool? eligibleWithOther, List<Guid> exceptCampaigns = null)
        {

            var promotions = new List<EditionPromotionSummaryDto>();

            var noPromotionEditionIds = editionIds;
            if (!noPromotionEditionIds.Any()) return promotions;

            var date = Clock.Now;

            var promotionSpecificEdtions = await _promotionCampaignEdtionRepository.GetAll().AsNoTracking()
                                                .Where(s => s.PromotionCampaign.IsActive)
                                                .Where(s => s.PromotionCampaign.IsSpecificPackage)
                                                .Where(s => s.PromotionCampaign.CampaignType == CampaignType.Upgrade)
                                                .Where(s => s.PromotionCampaign.PackageId == packageId)
                                                .Where(s => s.PromotionCampaign.StartDate.Date <= date.Date && (s.PromotionCampaign.NeverEnd || date.Date <= s.PromotionCampaign.EndDate.Value.Date))
                                                .Where(s => noPromotionEditionIds.Contains(s.EditionId))
                                                .Where(s => s.PromotionId.HasValue && s.Promotion.IsActive)
                                                .WhereIf(eligibleWithOther.HasValue, s => s.PromotionCampaign.IsEligibleWithOther == eligibleWithOther.Value)
                                                .WhereIf(!exceptCampaigns.IsNullOrEmpty(), s => !exceptCampaigns.Contains(s.PromotionCampaignId))
                                                .Select(s => new EditionPromotionSummaryDto
                                                {
                                                    Id = s.PromotionId.Value,
                                                    PromotionName = s.Promotion.PromotionName,
                                                    Value = s.Promotion.PromotionType == PromotionType.Discount ? s.Promotion.DiscountRate : s.Promotion.ExtraMonth,
                                                    PromotionType = s.Promotion.PromotionType,
                                                    IsTrial = s.Promotion.IsTrial,
                                                    CampaignId = s.PromotionCampaignId,
                                                    IsRenewable = s.PromotionCampaign.IsRenewable,
                                                    IsEligibleWithOther = s.PromotionCampaign.IsEligibleWithOther,
                                                    EditionId = s.EditionId
                                                })
                                                .GroupBy(s => s.EditionId)
                                                .Select(s => s.OrderByDescending(t => t.Value).FirstOrDefault())
                                                .ToListAsync();


            if (promotionSpecificEdtions.Any()) promotions.AddRange(promotionSpecificEdtions);

            noPromotionEditionIds = !promotions.Any() ? editionIds : editionIds.Where(s => !promotions.Any(r => r.EditionId == s)).ToList();
            if (!noPromotionEditionIds.Any()) return promotions;


            var promotionGeneral = await _promotionCampaignRepository.GetAll().AsNoTracking()
                                        .Where(s => s.IsActive)
                                        .Where(s => !s.IsSpecificPackage)
                                        .Where(s => s.CampaignType == CampaignType.Upgrade)
                                        .Where(s => s.StartDate.Date <= date.Date && (s.NeverEnd || date.Date <= s.EndDate.Value.Date))
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
                                            IsEligibleWithOther = s.IsEligibleWithOther,
                                        })
                                        .OrderByDescending(s => s.Value)
                                        .FirstOrDefaultAsync();

            if (promotionGeneral != null)
            {
                foreach(var editionId in noPromotionEditionIds)
                {
                    var promotion = new EditionPromotionSummaryDto
                    {
                        Id = promotionGeneral.Id,
                        PromotionName = promotionGeneral.PromotionName,
                        PromotionType = promotionGeneral.PromotionType,
                        IsTrial = promotionGeneral.IsTrial,
                        IsEligibleWithOther = promotionGeneral.IsEligibleWithOther,
                        IsRenewable = promotionGeneral.IsRenewable,
                        CampaignId = promotionGeneral.CampaignId,
                        Value = promotionGeneral.Value,
                        EditionId = editionId
                    };

                    promotions.Add(promotion);
                }
            }           

            return promotions;

        }


        //[UnitOfWork(IsDisabled = true)]
        //public async Task<SubscriptionPaymentOutput> GetLatestSubscription(EntityDto input)
        //{
        //    using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
        //    {
        //        using (_unitOfWorkManager.Current.SetTenantId(input.Id))
        //        {
        //            var subscription = await _subscriptionPaymentRepository.GetAll()
        //                                     .AsNoTracking()
        //                                     .OrderByDescending(s => s.CreationTime)
        //                                     .Select(s => new SubscriptionPaymentOutput
        //                                     {
        //                                         SubscriptionDate = s.SubscriptionDate,
        //                                         AffectedDate = s.AffectedDate,
        //                                         EndDate = s.EndDate,
        //                                         Duration = s.Duration,
        //                                         DurationType = s.DurationType,
        //                                         EditionId = s.EditionId,
        //                                         EditionName = s.Edition.DisplayName,
        //                                         PackagePrice = s.PackagePrice,
        //                                         TotalPrice  = s.TotalPrice
        //                                     })
        //                                     .FirstOrDefaultAsync();
        //            return subscription;
        //        }
        //    }
        //}

    }
}
