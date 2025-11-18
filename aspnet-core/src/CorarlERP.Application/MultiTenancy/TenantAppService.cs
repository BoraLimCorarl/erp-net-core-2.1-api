using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Security;
using Microsoft.EntityFrameworkCore;
using CorarlERP.Authorization;
using CorarlERP.Editions.Dto;
using CorarlERP.MultiTenancy.Dto;
using CorarlERP.Url;
using Abp.Runtime.Session;
using Abp.UI;
using System;
using CorarlERP.AccountCycles;
using Abp.Domain.Repositories;
using CorarlERP.Currencies;
using CorarlERP.Formats;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Subscriptions;
using CorarlERP.Features;
using CorarlERP.ReportTemplates.Dto;
using CorarlERP.ReportTemplates;
using static CorarlERP.enumStatus.EnumStatus;
using System.Transactions;
using Abp.Application.Editions;
using CorarlERP.Editions;
using CorarlERP.SignUps;
using CorarlERP.Authorization.Roles;
using Abp.Timing;
using CorarlERP.Promotions;
using Abp.Collections.Extensions;
using Amazon.Scheduler.Endpoints;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using CorarlERP.PackageEditions;
using CorarlERP.InventoryTransactionTypes;
using CorarlERP.Authorization.Users;
using CorarlERP.PropertyFormulas.Dto;

namespace CorarlERP.MultiTenancy
{
    [AbpAuthorize(AppPermissions.Pages_Tenants, AppPermissions.Pages_Tenant_Common_Tenant_Find)]
    public class TenantAppService : CorarlERPAppServiceBase, ITenantAppService
    {
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IRepository<Edition, int> _editionRepository;
        private readonly IRepository<Format, long> _formatRepository;
        private readonly IRepository<Currency, long> _currencyRepository;
        private readonly IAccountCycleManager _accountCycleManger;
        private readonly ITenantManager _tenantManager;
        private readonly IDefaultChartOfAccountManager _defaultChartOfAccountManager;
        private readonly IRepository<Subscription, Guid> _subScriptionRepository;
        private readonly IRepository<PromotionCampaign, Guid> _promotionCampaignRepository;
        private readonly IRepository<PromotionCampaignEdition, Guid> _promotionCampaignEditionRepository;
        private readonly IRepository<PackageEdition, Guid> _packageEditionRepository;
        public IAppUrlService AppUrlService { get; set; }
        private readonly ICorarlRepository<JournalTransactionType, Guid> _journalTransactionTransactionRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly RoleManager _roleManager;
        private readonly IReportTemplateManager _reportTemplateManager;
        private readonly IGroupMemberItemTamplateManager _groupMemberItemTamplateManager;
        private readonly IRepository<ReportTemplate, long> _reportTemplateRepository;
        private readonly EditionManager _editionManager;
        private readonly IRepository<SignUp, Guid> _signUpRepository;
        private readonly ICorarlRepository<Role, int> _roleRepository;
        private readonly ICorarlRepository<SubscriptionPromotion, Guid> _subscriptionPromotionRepository;
        private readonly ICorarlRepository<SubscriptionCampaignPromotion, Guid> _subscriptionCampaignPromotionRepository;
        private readonly ICorarlRepository<Promotion, Guid> _promotionRepository;
        private readonly ICorarlRepository<User, long> _userRepository;
        public TenantAppService(
            IRepository<PackageEdition, Guid> packageEditionRepository,
            IRepository<Edition, int> editionRepository,
            IRepository<PromotionCampaign, Guid> promotionCampaignRepository,
            IRepository<PromotionCampaignEdition, Guid> promotionCampaignEditionRepository,
            EditionManager editionManager,
            ITenantManager tenantManager,
            IAccountCycleManager accountCycleManger,
            IRepository<Format, long> formatRepository,
            IRepository<Tenant> tenantRepository,
            IRepository<Currency, long> currencyRepository,
            IDefaultChartOfAccountManager defaultChartOfAccountManager,
            IRepository<ReportTemplate, long> reportTemplateRepository,
            IRepository<Subscription, Guid> subScriptionRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IReportTemplateManager reportTemplateManager,
            IGroupMemberItemTamplateManager groupMemberItemTamplateManager,
            IRepository<SignUp, Guid> signUpRepository,
            ICorarlRepository<Role, int> roleRepository,
            RoleManager roleManager,
            ICorarlRepository<SubscriptionPromotion, Guid> subscriptionPromotionRepository,
            ICorarlRepository<SubscriptionCampaignPromotion, Guid> subscriptionCampaignPromotionRepository,
            ICorarlRepository<Promotion, Guid> promotionRepository,
            ICorarlRepository<JournalTransactionType, Guid> journalTransactionTransactionRepository,
            ICorarlRepository<User, long> userRepository
          )
        {
            AppUrlService = NullAppUrlService.Instance;
            _tenantManager = tenantManager;
            _accountCycleManger = accountCycleManger;
            _formatRepository = formatRepository;
            _currencyRepository = currencyRepository;
            _tenantRepository = tenantRepository;
            _defaultChartOfAccountManager = defaultChartOfAccountManager;
            _subScriptionRepository = subScriptionRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _reportTemplateManager = reportTemplateManager;
            _groupMemberItemTamplateManager = groupMemberItemTamplateManager;
            _reportTemplateRepository = reportTemplateRepository;
            _editionManager = editionManager;
            _signUpRepository = signUpRepository;
            _roleManager = roleManager;
            _roleRepository = roleRepository;
            _subscriptionPromotionRepository = subscriptionPromotionRepository;
            _promotionRepository = promotionRepository;
            _promotionCampaignRepository = promotionCampaignRepository;
            _promotionCampaignEditionRepository = promotionCampaignEditionRepository;
            _subscriptionCampaignPromotionRepository = subscriptionCampaignPromotionRepository;
            _editionRepository = editionRepository;
            _packageEditionRepository = packageEditionRepository;
            _journalTransactionTransactionRepository = journalTransactionTransactionRepository;
            _userRepository = userRepository;

        }
        [AbpAuthorize(AppPermissions.Pages_Tenants_GetList)]
        public async Task<PagedResultDto<TenantListDto>> GetTenants(GetTenantsInput input)
        {
            var query = (from t in TenantManager.Tenants
                .Include(t => t.Edition)
                .WhereIf(!input.Filter.IsNullOrWhiteSpace(), t => t.Name.Contains(input.Filter) || t.TenancyName.Contains(input.Filter))
                .WhereIf(input.CreationDateStart.HasValue, t => t.CreationTime >= input.CreationDateStart.Value)
                .WhereIf(input.CreationDateEnd.HasValue, t => t.CreationTime <= input.CreationDateEnd.Value)
                .WhereIf(input.EditionIdSpecified, t => t.EditionId == input.EditionId)
                         join s in _subScriptionRepository.GetAll()
                         //  .WhereIf(input.SubscriptionEndDateStart.HasValue, t => t.Endate >= input.SubscriptionEndDateStart.Value.ToUniversalTime())
                         .WhereIf(input.SubscriptionEndDateEnd.HasValue, t => t.Endate != null && t.Endate <= input.SubscriptionEndDateEnd.Value.ToUniversalTime()) on t.SubscriptionId equals s.Id
                         select new TenantListDto
                         {
                             IsDebug = t.IsDebug,
                             ConnectionString = t.ConnectionString,
                             CreationTime = t.CreationTime,
                             EditionDisplayName = t.Edition.DisplayName,
                             EditionId = t.EditionId,
                             Id = t.Id,
                             IsActive = t.IsActive,
                             IsInTrialPeriod = t.IsInTrialPeriod,
                             Name = t.Name,
                             Subscription = new GetSubscriptionInput
                             {
                                 Duration = s.Duration,
                                 DurationType = s.DurationType,
                                 EditionId = s.EditionId,
                                 EndDate = s.Endate,
                                 Id = s.Id,
                                 StartDate = s.StartDate,
                                 TenantId = s.TenantId,
                                 Unlimited = s.Unlimited,
                                 ShowWarning = s.ShowWarning,
                                 IsTrail = s.IsTrail,
                                 PackageId = s.PackageId,
                                 UpgradeFromSubscriptionId = s.UpgradeFromSubscriptionId,
                             },

                             SubscriptionEndDateUtc = s.Endate,
                             TenancyName = t.TenancyName,
                             UseBatchNo = t.UseBatchNo,
                             DefaultInventoryReportTemplate = t.DefaultInventoryReportTemplate,

                         });
            var tenantCount = await query.CountAsync();
            var tenants = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            return new PagedResultDto<TenantListDto>(
                tenantCount,
                ObjectMapper.Map<List<TenantListDto>>(tenants)
                );
        }

        private async Task ValidatePromotions(List<SubscriptionPromotionInput> promotions)
        {
            if (promotions.IsNullOrEmpty()) return;

            var notSelectPromotions = promotions.Any(r => r.PromotionId == null || r.PromotionId == Guid.Empty);
            if (notSelectPromotions) throw new UserFriendlyException(L("IsRequired", L("Promotion")));

            var newablePromotions = promotions.Where(s => s.IsRenewable).ToList();
            if (newablePromotions.Any(s => s.IsEligibleWithOther) && newablePromotions.All(s => !s.IsEligibleWithOther)) throw new UserFriendlyException(L("CannotApplyMoreThanOneIsIneligibleWithOther", L("Promotion")));

            var promotionIds = promotions.GroupBy(s => s.PromotionId).Select(s => s.Key).ToList();

            var validatePromotions = await _promotionRepository.GetAll().AsNoTracking().CountAsync(s => promotionIds.Contains(s.Id)) == promotionIds.Count;
            if (!validatePromotions) throw new UserFriendlyException(L("IsNotValid", L("Promotion")));


            var campaigns = promotions.Where(s => s.CampaignId.HasValue).ToList();
            if (campaigns.Any())
            {
                var campaignIds = campaigns.GroupBy(s => s.CampaignId.Value).Select(s => s.Key).ToList();
                var validateCampaings = await _promotionCampaignRepository.GetAll().AsNoTracking().CountAsync(s => campaignIds.Contains(s.Id)) == campaignIds.Count;
                if (!validateCampaings) throw new UserFriendlyException(L("IsNotValid", L("Campaign")));

                var specificePackageCampaigns = campaigns.Where(s => s.IsSpecificPackage).ToList();
                if (specificePackageCampaigns.Any(s => s.CampaignEditionPromotions.IsNullOrEmpty())) throw new UserFriendlyException(L("IsRequired", L("CampaignEditions")));

                var noEditions = specificePackageCampaigns.Any(s => s.CampaignEditionPromotions.Any(r => r.EditionId <= 0));
                if (noEditions) throw new UserFriendlyException(L("IsRequired", L("CampaignEditions")));

                var duplicateEdtions = specificePackageCampaigns.Any(s => s.CampaignEditionPromotions.GroupBy(g => g.EditionId).Any(r => r.Count() > 1));
                if (duplicateEdtions) throw new UserFriendlyException(L("Duplicated", L("CampaignEditions")));

                var noPromotions = specificePackageCampaigns.Any(s => !s.CampaignEditionPromotions.Any(r => r.PromotionId.HasValue && r.PromotionId != Guid.Empty));
                if (noPromotions) throw new UserFriendlyException(L("IsRequired", L("CampaignPromotions")));

                var editonIds = specificePackageCampaigns.SelectMany(s => s.CampaignEditionPromotions).GroupBy(g => g.EditionId).Select(s => s.Key).ToList();
                var validateEdtions = await _editionRepository.GetAll().AsNoTracking().CountAsync(s => editonIds.Contains(s.Id)) == editonIds.Count;
                if (!validateEdtions) throw new UserFriendlyException(L("IsNotValid", L("CampaignEditions")));

                var campaignPromotionIds = specificePackageCampaigns.SelectMany(s => s.CampaignEditionPromotions)
                                           .Where(s => s.PromotionId.HasValue && s.PromotionId != Guid.Empty)
                                           .GroupBy(g => g.PromotionId).Select(s => s.Key).ToList();

                var validateCampaignPromotions = await _promotionRepository.GetAll().AsNoTracking().CountAsync(s => campaignPromotionIds.Contains(s.Id)) == campaignPromotionIds.Count;
                if (!validateCampaignPromotions) throw new UserFriendlyException(L("IsNotValid", L("CampaignPromotions")));
            }

        }


        [AbpAuthorize(AppPermissions.Pages_Tenants_Create)]
        [UnitOfWork(IsDisabled = true)]
        public async Task CreateTenant(CreateTenantInput input)
        {
            if (!input.Subscription.Unlimited)
            {
                if (input.Subscription.StartDate == null)
                {
                    throw new UserFriendlyException(L("StartDateIsRequired"));
                }
                else if ((input.Subscription.Duration == null || input.Subscription.Duration == 0) && !input.Subscription.IsTrail)
                {
                    throw new UserFriendlyException(L("InvalidDuration"));
                }
            }
            if (input.AutoItemCode && string.IsNullOrEmpty(input.ItemCode))
            {
                throw new UserFriendlyException(L("ItemCodeIsRequired"));
            }
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                await ValidatePromotions(input.Subscription.SubscriptionPromotions);
            }

            var date = DateTime.Now;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var dateValue = $"thisMonth,{firstDayOfMonth.ToString("MMM dd, yyyy") + " 07:00:00 GMT+0700 (Indochina Time) " + lastDayOfMonth.ToString("MMMM dd , yyyy") + " 07:00:00 GMT+0700 (Indochina Time)"}";
            List<CreateTemplate> inputs = new List<CreateTemplate>();
            if (input.DefaultInventoryReportTemplate)
            {
                inputs = new List<CreateTemplate>
                {
                      new CreateTemplate{
                          ReportType = ReportType.ReportType_StockBalance,
                          TemplateType = TemplateType.Gloable,
                          reportCategory = ReportCategory.InventoryReport,
                          PermissionReadWrite = PermissionReadWrite.Read,
                          Groupby = null,
                          DefaultTemplateReport =  null,
                          DefaultTemplateReport2 = null,
                          DefaultTemplateReport3 = null,
                          TemplateName = "សមតុល្យស្ដុក",
                          HeaderTitle = "សមតុល្យស្ដុក",
                          Sortby = null,
                          IsActive = false,
                          IsDefault = false,
                          MemberGroupItemTamplate = new List<CreateOrUpdateMemberGroupItemTamplate>{},
                          Columns = new List<GetColumnTemplateOutput>{
                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "ItemCode",
                            ColumnLength = 130,
                            ColumnTitle = "លេខកូដទំនិញ",
                            ColumnType = ColumnType.String,
                            SortOrder = 1,
                            Visible = true,
                            AllowFunction = null,
                            IsDisplay = true,
                        },
                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "ItemName",
                            ColumnLength = 250,
                            ColumnTitle = "ឈ្មោះទំនិញ",
                            ColumnType = ColumnType.String,
                            SortOrder = 2,
                            Visible = true,
                            AllowFunction = null,
                            IsDisplay = true,
                        },
                        new GetColumnTemplateOutput{
                            AllowGroupby = true,
                            AllowFilter = true,
                            ColumnName = "Lot",
                            ColumnLength = 126,
                            ColumnTitle = "ទីតាំង",
                            ColumnType = ColumnType.String,
                            SortOrder = 4,
                            Visible = true,
                            AllowFunction = null,
                            IsActive = true,
                            IsDisplay = true,
                        },
                        new GetColumnTemplateOutput
                        {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Beginning",
                        ColumnLength = 117,
                        ColumnTitle = "ស្តុកដើមគ្រា",
                        ColumnType = ColumnType.Number,
                        SortOrder = 5,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        IsActive = true,
                        },

                        new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalInQty",
                        ColumnLength = 118,
                        ColumnTitle = "ស្តុកចូល",
                        ColumnType = ColumnType.Number,
                        SortOrder = 6,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        IsActive = true,
                    },
                    new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalOutQty",
                        ColumnLength = 118,
                        ColumnTitle = "ស្តុកចេញ",
                        ColumnType = ColumnType.Number,
                        SortOrder = 7,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        IsActive = true,
                    },
                       new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalQtyOnHand",
                        ColumnLength = 114,
                        ColumnTitle = "ស្តុកចុងគ្រា",
                        ColumnType = ColumnType.Number,
                        SortOrder = 8,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        IsActive =true,
                    },
                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "NetWeight",
                            ColumnLength = 95,
                            ColumnTitle = "ចំនួន (Kg)",
                            ColumnType = ColumnType.Number,
                            SortOrder = 10,
                            Visible = false,
                            AllowFunction = "Sum",
                            IsActive =false,
                            IsDisplay = false
                       },

                        new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "NetWeight",
                        ColumnLength = 95,
                        ColumnTitle = "ចំនួន (Kg)",
                        ColumnType = ColumnType.Number,
                        SortOrder = 10,
                        Visible = false,
                        AllowFunction = "Sum",
                        IsActive =true,
                        IsDisplay = false
                    },
                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = true,
                            ColumnName = "Unit",
                            ColumnLength = 100,
                            ColumnTitle = "ខ្នាត",
                            ColumnType = ColumnType.ItemProperty,
                            SortOrder = 14,
                            Visible = true,
                            AllowFunction = null,
                            IsDisplay = true,
                        },
                          },

                           Filters = new List<GetFilterTemplateOutput>{
                                              new GetFilterTemplateOutput{
                                                 Visible = false,
                                                 AllowShowHideFilter = false,
                                                 ShowHideFilter = false,
                                                 DefaultValueId = null,
                                                 FilterId = null,
                                                 FilterName = "Search",
                                                 FilterType = ColumnType.Language,
                                                 FilterValue = null,
                                                 IsActive = true,
                                                 SortOrder = 0
                                               },
                                              new GetFilterTemplateOutput{
                                                 Visible = true,
                                                 AllowShowHideFilter = false,
                                                 ShowHideFilter = false,
                                                 DefaultValueId = dateValue,
                                                 FilterId = null,
                                                 FilterName = "DateRange",
                                                 FilterType = ColumnType.Language,
                                                 FilterValue = null,
                                                 IsActive = true,
                                                 SortOrder = 0
                                               },
                           },
                      },
                      new CreateTemplate{
                          ReportType = ReportType.ReportType_Inventory,
                          TemplateType = TemplateType.Gloable,
                          reportCategory = ReportCategory.InventoryReport,
                          PermissionReadWrite = PermissionReadWrite.Read,
                          Groupby = null,
                          DefaultTemplateReport =  null,
                          DefaultTemplateReport2 = null,
                          DefaultTemplateReport3 = null,
                          TemplateName = "របាយការណ៍តំលៃស្តុកសង្ខេប",
                          HeaderTitle = "របាយការណ៍តំលៃស្តុកសង្ខេប",
                          Sortby = null,
                          IsActive = false,
                          IsDefault = false,
                          MemberGroupItemTamplate = new List<CreateOrUpdateMemberGroupItemTamplate>{},
                          Columns = new List<GetColumnTemplateOutput>{
                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "ItemCode",
                            ColumnLength = 130,
                            ColumnTitle = "លេខកូដទំនិញ",
                            ColumnType = ColumnType.String,
                            SortOrder = 1,
                            Visible = true,
                            AllowFunction = null,
                            IsDisplay = true,
                        },
                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "ItemName",
                            ColumnLength = 196,
                            ColumnTitle = "ឈ្មោះទំនិញ",
                            ColumnType = ColumnType.String,
                            SortOrder = 2,
                            Visible = true,
                            AllowFunction = null,
                            IsDisplay = true,
                        },
                        new GetColumnTemplateOutput{
                            AllowGroupby = true,
                            AllowFilter = true,
                            ColumnName = "Location",
                            ColumnLength = 104,
                            ColumnTitle = "ទីតាំង",
                            ColumnType = ColumnType.String,
                            SortOrder = 4,
                            Visible = true,
                            AllowFunction = null,
                            IsActive = true,
                            IsDisplay = true,
                        },
                        new GetColumnTemplateOutput{
                            AllowGroupby = true,
                            AllowFilter = true,
                            ColumnName = "Date",
                            ColumnLength = 126,
                            ColumnTitle = "ថ្ងៃខែ",
                            ColumnType = ColumnType.Date,
                            SortOrder = 4,
                            Visible = true,
                            AllowFunction = null,
                            IsActive = true,
                            IsDisplay = true,
                        },

                        new GetColumnTemplateOutput
                        {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Beginning",
                        ColumnLength = 108,
                        ColumnTitle = "ស្តុកដើមគ្រា",
                        ColumnType = ColumnType.Number,
                        SortOrder = 5,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        IsActive = true,
                        },

                        new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalInQty",
                        ColumnLength = 100,
                        ColumnTitle = "ស្តុកចូល",
                        ColumnType = ColumnType.Number,
                        SortOrder = 6,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        IsActive = true,
                    },
                    new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalOutQty",
                        ColumnLength = 100,
                        ColumnTitle = "ស្តុកចេញ",
                        ColumnType = ColumnType.Number,
                        SortOrder = 7,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        IsActive = true,
                    },
                       new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalQtyOnHand",
                        ColumnLength = 114,
                        ColumnTitle = "ស្តុកចុងគ្រា",
                        ColumnType = ColumnType.Number,
                        SortOrder = 8,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        IsActive =true,
                    },
                         new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "AverageCost",
                        ColumnLength = 100,
                        ColumnTitle = "ថ្លៃមធ្យម",
                        ColumnType = ColumnType.RoundingDigit,
                        SortOrder = 10,
                        Visible = true,
                        IsDisplay = true,

                    },
                         new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "DisplayTotalCost",
                        ColumnLength = 100,
                        ColumnTitle = "តម្លៃសរុប",
                        ColumnType = ColumnType.Money,
                        SortOrder = 11,
                        Visible = true,
                        AllowFunction = "Sum",
                        IsDisplay = true
                    },

                          },

                           Filters = new List<GetFilterTemplateOutput>{
                                              new GetFilterTemplateOutput{
                                                 Visible = false,
                                                 AllowShowHideFilter = false,
                                                 ShowHideFilter = false,
                                                 DefaultValueId = null,
                                                 FilterId = null,
                                                 FilterName = "Search",
                                                 FilterType = ColumnType.Language,
                                                 FilterValue = null,
                                                 IsActive = true,
                                                 SortOrder = 0
                                               },
                                              new GetFilterTemplateOutput{
                                                 Visible = true,
                                                 AllowShowHideFilter = false,
                                                 ShowHideFilter = false,
                                                 DefaultValueId = dateValue,
                                                 FilterId = null,
                                                 FilterName = "DateRange",
                                                 FilterType = ColumnType.Language,
                                                 FilterValue = null,
                                                 IsActive = true,
                                                 SortOrder = 0
                                               },
                           },
                      },

                       new CreateTemplate{
                          ReportType = ReportType.ReportType_InventoryDetail,
                          TemplateType = TemplateType.Gloable,
                          reportCategory = ReportCategory.InventoryReport,
                          PermissionReadWrite = PermissionReadWrite.Read,
                          Groupby = null,
                          DefaultTemplateReport =  null,
                          DefaultTemplateReport2 = null,
                          DefaultTemplateReport3 = null,
                          TemplateName = "របាយការណ៍តំលៃស្តុកលម្អិត",
                          HeaderTitle = "របាយការណ៍តំលៃស្តុកលម្អិត",
                          Sortby = null,
                          IsActive = false,
                          IsDefault = false,
                          MemberGroupItemTamplate = new List<CreateOrUpdateMemberGroupItemTamplate>{},
                          Columns = new List<GetColumnTemplateOutput>{

                        new GetColumnTemplateOutput{
                            AllowGroupby = true,
                            AllowFilter = false,
                            ColumnName = "JournalNo",
                            ColumnLength = 96,
                            ColumnTitle = "លេខឯកសារ",
                            ColumnType = ColumnType.String,
                            SortOrder = 2,
                            Visible = true,
                            AllowFunction = null,
                            IsDisplay = true,
                            IsActive =true
                        },
                        new GetColumnTemplateOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "JournalType",
                        ColumnLength = 90,
                        ColumnTitle = "ប្រតិបត្តិការ",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,

                        },
                        new GetColumnTemplateOutput{
                            AllowGroupby = true,
                            AllowFilter = true,
                            ColumnName = "Date",
                            ColumnLength = 80,
                            ColumnTitle = "ថ្ងៃខែ",
                            ColumnType = ColumnType.Date,
                            SortOrder = 1,
                            Visible = true,
                            AllowFunction = null,
                            IsActive = true,
                            IsDisplay = true,
                        },
                        new GetColumnTemplateOutput{
                            AllowGroupby = true,
                            AllowFilter = true,
                            ColumnName = "Location",
                            ColumnLength = 120,
                            ColumnTitle = "ទីតាំង",
                            ColumnType = ColumnType.String,
                            SortOrder = 5,
                            Visible = true,
                            AllowFunction = null,
                            IsActive = true,
                            IsDisplay = true,
                        },
                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "InQty",
                            ColumnLength = 90,
                            ColumnTitle = "ស្តុកចូល",
                            ColumnType = ColumnType.Number,
                            SortOrder =6,
                            Visible = true,
                            IsDisplay = true,
                    },
                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "OutQty",
                            ColumnLength = 90,
                            ColumnTitle = "ស្តុកចេញ",
                            ColumnType = ColumnType.Number,
                            SortOrder = 7,
                            Visible = true,
                            IsDisplay = true,

                    },
                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "TotalQty",
                            ColumnLength = 100,
                            ColumnTitle = "ស្តុកចុងគ្រា",
                            ColumnType = ColumnType.Number,
                            SortOrder = 10,
                            Visible = true,
                            IsDisplay = true,

                        },
                        new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "UnitCost",
                        ColumnLength = 90,
                        ColumnTitle = "តម្លៃឯកតា",
                        ColumnType = ColumnType.RoundingDigit,
                        SortOrder = 8,
                        Visible = true,
                        IsDisplay = true,

                    },
                    new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "LineTotal",
                        ColumnLength = 100,
                        ColumnTitle = "សរុប",
                        ColumnType = ColumnType.Money,
                        SortOrder = 9,
                        Visible = true,
                        IsDisplay = true,

                    },
                    new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "AVGCost",
                        ColumnLength = 90,
                        ColumnTitle = "ថ្លៃមធ្យម",
                        ColumnType = ColumnType.RoundingDigit,
                        SortOrder = 11,
                        Visible = true,
                        IsDisplay = true,
                    },
                     new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalCost",
                        ColumnLength = 100,
                        ColumnTitle = "តម្លៃសរុប",
                        ColumnType = ColumnType.Money,
                        SortOrder = 12,
                        Visible = true,
                        IsDisplay = true,

                    },
                        },

                           Filters = new List<GetFilterTemplateOutput>{
                                              new GetFilterTemplateOutput{
                                                 Visible = false,
                                                 AllowShowHideFilter = false,
                                                 ShowHideFilter = false,
                                                 DefaultValueId = null,
                                                 FilterId = null,
                                                 FilterName = "Search",
                                                 FilterType = ColumnType.Language,
                                                 FilterValue = null,
                                                 IsActive = true,
                                                 SortOrder = 0
                                               },
                                              new GetFilterTemplateOutput{
                                                 Visible = true,
                                                 AllowShowHideFilter = false,
                                                 ShowHideFilter = false,
                                                 DefaultValueId = dateValue,
                                                 FilterId = null,
                                                 FilterName = "DateRange",
                                                 FilterType = ColumnType.Language,
                                                 FilterValue = null,
                                                 IsActive = true,
                                                 SortOrder = 0
                                               },
                           },
                      },

                      new CreateTemplate{
                          ReportType = ReportType.ReportType_InventoryTransaction,
                          TemplateType = TemplateType.Gloable,
                          reportCategory = ReportCategory.InventoryReport,
                          PermissionReadWrite = PermissionReadWrite.Read,
                          Groupby = "Item",
                          DefaultTemplateReport =  null,
                          DefaultTemplateReport2 = null,
                          DefaultTemplateReport3 = null,
                          TemplateName = "ប្រវត្តិស្តុកតាមទំនិញ",
                          HeaderTitle = "ប្រវត្តិស្តុកតាមទំនិញ",
                          Sortby = null,
                          IsActive = false,
                          IsDefault = false,
                          MemberGroupItemTamplate = new List<CreateOrUpdateMemberGroupItemTamplate>{},
                          Columns = new List<GetColumnTemplateOutput>{


                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "Date",
                            ColumnLength = 100,
                            ColumnTitle = "ថ្ងៃខែ",
                            ColumnType = ColumnType.Date,
                            SortOrder = 1,
                            Visible = true,
                            AllowFunction = null,
                            IsDisplay = true,
                            IsActive = true,
                        },
                        new GetColumnTemplateOutput{
                             AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "ItemName",
                            ColumnLength = 150,
                            ColumnTitle = "ឈ្មោះទំនិញ",
                            ColumnType = ColumnType.String,
                            SortOrder = 3,
                            Visible = true,
                            AllowFunction = null,
                            IsDisplay = true,
                        },
                        new GetColumnTemplateOutput{
                            AllowGroupby = true,
                            AllowFilter = false,
                            ColumnName = "Type",
                            ColumnLength = 150,
                            ColumnTitle = "ប្រភេទប្រតិបត្តិការណ៍",
                            ColumnType = ColumnType.String,
                            SortOrder = 4,
                            Visible = false,
                            AllowFunction = null,
                            IsDisplay = false,
                            IsActive = true,

                        },

                         new GetColumnTemplateOutput
                        {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Beginning",
                        ColumnLength = 170,
                        ColumnTitle = "Beginning Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 10,
                        Visible = false,
                        IsDisplay = true,
                        DisableDefault = true,
                        AllowFunction = "Sum",
                        IsActive = false,
                        },

                        new GetColumnTemplateOutput
                        {
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "Reference",
                            ColumnLength = 100,
                            ColumnTitle = "ឯកសារយោង",
                            ColumnType = ColumnType.String,
                            SortOrder = 7,
                            Visible = true,
                            IsDisplay = true,
                            IsActive = true,
                        },
                        new GetColumnTemplateOutput{
                        AllowGroupby = true,
                        AllowFilter = false,
                        ColumnName = "JournalNo",
                        ColumnLength = 140,
                        ColumnTitle = "លេខឯកសារ",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        IsActive =true
                    },
                     new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalInQty",
                        ColumnLength = 100,
                        ColumnTitle = "ស្តុកចូល",
                        ColumnType = ColumnType.Number,
                        SortOrder = 7,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        IsActive = true,
                    },

                        new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalOutQty",
                        ColumnLength = 100,
                        ColumnTitle = "ស្តុកចេញ",
                        ColumnType = ColumnType.Number,
                        SortOrder = 7,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        IsActive = true,
                    },

                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "NetWeight",
                            ColumnLength = 95,
                            ColumnTitle = "ចំនួន (Kg)",
                            ColumnType = ColumnType.Number,
                            SortOrder = 10,
                            Visible = false,
                            AllowFunction = "Sum",
                            IsActive =false,
                            IsDisplay = false
                       },
                        new GetColumnTemplateOutput
                        {
                            AllowGroupby = true,
                            AllowFilter = false,
                            ColumnName = "PartnerName",
                            ColumnLength = 140,
                            ColumnTitle = "ដៃគូរ",
                            ColumnType = ColumnType.String,
                            SortOrder = 2,
                            Visible = true,
                            IsDisplay = true,
                            IsActive = true,
                        },
                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = true,
                            ColumnName = "Unit",
                            ColumnLength = 90,
                            ColumnTitle = "ខ្នាត",
                            ColumnType = ColumnType.ItemProperty,
                            SortOrder = 14,
                            Visible = true,
                            AllowFunction = null,
                            IsDisplay = true,
                        },

                        new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalQty",
                        ColumnLength = 150,
                        ColumnTitle = "ស្តុកចុងគ្រា",
                        ColumnType = ColumnType.Number,
                        SortOrder = 13,
                        Visible = true,
                        IsDisplay = true,
                        DisableDefault = true,
                        AllowFunction = "Sum",
                    },

                        },

                           Filters = new List<GetFilterTemplateOutput>{
                                              new GetFilterTemplateOutput{
                                                 Visible = false,
                                                 AllowShowHideFilter = false,
                                                 ShowHideFilter = false,
                                                 DefaultValueId = null,
                                                 FilterId = null,
                                                 FilterName = "Search",
                                                 FilterType = ColumnType.Language,
                                                 FilterValue = null,
                                                 IsActive = true,
                                                 SortOrder = 0
                                               },
                                              new GetFilterTemplateOutput{
                                                 Visible = true,
                                                 AllowShowHideFilter = false,
                                                 ShowHideFilter = false,
                                                 DefaultValueId = dateValue,
                                                 FilterId = null,
                                                 FilterName = "DateRange",
                                                 FilterType = ColumnType.Language,
                                                 FilterValue = null,
                                                 IsActive = true,
                                                 SortOrder = 0
                                               },
                           },
                      },

                      new CreateTemplate{
                          ReportType = ReportType.ReportType_InventoryTransaction,
                          TemplateType = TemplateType.Gloable,
                          reportCategory = ReportCategory.InventoryReport,
                          PermissionReadWrite = PermissionReadWrite.Read,
                          Groupby = "JournalTransactionType",
                          DefaultTemplateReport =  null,
                          DefaultTemplateReport2 = null,
                          DefaultTemplateReport3 = null,
                          TemplateName = "ប្រវត្តិស្តុកតាមប្រតិបត្តិការណ៍",
                          HeaderTitle = "ប្រវត្តិស្តុកតាមប្រតិបត្តិការណ៍",
                          Sortby = null,
                          IsActive = false,
                          IsDefault = false,
                          MemberGroupItemTamplate = new List<CreateOrUpdateMemberGroupItemTamplate>{},
                          Columns = new List<GetColumnTemplateOutput>{


                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "Date",
                            ColumnLength = 100,
                            ColumnTitle = "ថ្ងៃខែ",
                            ColumnType = ColumnType.Date,
                            SortOrder = 1,
                            Visible = true,
                            AllowFunction = null,
                            IsDisplay = true,
                            IsActive = true,
                        },
                        new GetColumnTemplateOutput{
                             AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "ItemName",
                            ColumnLength = 205,
                            ColumnTitle = "ឈ្មោះទំនិញ",
                            ColumnType = ColumnType.String,
                            SortOrder = 3,
                            Visible = true,
                            AllowFunction = null,
                            IsDisplay = true,
                        },
                        new GetColumnTemplateOutput{
                            AllowGroupby = true,
                            AllowFilter = false,
                            ColumnName = "JournalTransactionType",
                            ColumnLength = 150,
                            ColumnTitle = "ប្រភេទប្រតិបត្តិការណ៍",
                            ColumnType = ColumnType.String,
                            SortOrder = 4,
                            Visible = false,
                            AllowFunction = null,
                            IsDisplay = false,
                            IsActive = true,

                        },

                         new GetColumnTemplateOutput
                        {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Beginning",
                        ColumnLength = 170,
                        ColumnTitle = "Beginning Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 10,
                        Visible = false,
                        IsDisplay = true,
                        DisableDefault = true,
                        AllowFunction = "Sum",
                        IsActive = false,
                        },

                        new GetColumnTemplateOutput
                        {
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "Reference",
                            ColumnLength = 100,
                            ColumnTitle = "ឯកសារយោង",
                            ColumnType = ColumnType.String,
                            SortOrder = 7,
                            Visible = true,
                            IsDisplay = true,
                            IsActive = true,
                        },
                        new GetColumnTemplateOutput{
                        AllowGroupby = true,
                        AllowFilter = false,
                        ColumnName = "JournalNo",
                        ColumnLength = 105,
                        ColumnTitle = "លេខឯកសារ",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        IsActive =true
                    },
                     new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalInQty",
                        ColumnLength = 100,
                        ColumnTitle = "ស្តុកចូល",
                        ColumnType = ColumnType.Number,
                        SortOrder = 7,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        IsActive = true,
                    },

                        new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalOutQty",
                        ColumnLength = 100,
                        ColumnTitle = "ស្តុកចេញ",
                        ColumnType = ColumnType.Number,
                        SortOrder = 7,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        IsActive = true,
                    },
                        new GetColumnTemplateOutput
                        {
                            AllowGroupby = true,
                            AllowFilter = false,
                            ColumnName = "PartnerName",
                            ColumnLength = 135,
                            ColumnTitle = "ដៃគូរ",
                            ColumnType = ColumnType.String,
                            SortOrder = 2,
                            Visible = true,
                            IsDisplay = true,
                            IsActive = true,
                        },
                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = true,
                            ColumnName = "Unit",
                            ColumnLength = 90,
                            ColumnTitle = "ខ្នាត",
                            ColumnType = ColumnType.ItemProperty,
                            SortOrder = 14,
                            Visible = true,
                            AllowFunction = null,
                            IsDisplay = true,
                        },

                       new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalQty",
                        ColumnLength = 140,
                        ColumnTitle = "ស្តុកចុងគ្រា",
                        ColumnType = ColumnType.Number,
                        SortOrder = 13,
                        Visible = true,
                        IsDisplay = true,
                        DisableDefault = true,
                        AllowFunction = "Sum",
                    },

                        },

                           Filters = new List<GetFilterTemplateOutput>{
                                              new GetFilterTemplateOutput{
                                                 Visible = false,
                                                 AllowShowHideFilter = false,
                                                 ShowHideFilter = false,
                                                 DefaultValueId = null,
                                                 FilterId = null,
                                                 FilterName = "Search",
                                                 FilterType = ColumnType.Language,
                                                 FilterValue = null,
                                                 IsActive = true,
                                                 SortOrder = 0
                                               },
                                              new GetFilterTemplateOutput{
                                                 Visible = true,
                                                 AllowShowHideFilter = false,
                                                 ShowHideFilter = false,
                                                 DefaultValueId = dateValue,
                                                 FilterId = null,
                                                 FilterName = "DateRange",
                                                 FilterType = ColumnType.Language,
                                                 FilterValue = null,
                                                 IsActive = true,
                                                 SortOrder = 0
                                               },
                           },
                      },


                };
            }
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                var tenantId = await TenantManager.CreateWithAdminUserAsync(input.TenancyName,
                               input.Name,
                               input.AdminPassword,
                               input.AdminEmailAddress,
                               input.ConnectionString,
                               input.IsActive,
                               input.EditionId,
                               input.ShouldChangePasswordOnNextLogin,
                               input.SendActivationEmail,
                               input.SubscriptionEndDateUtc?.ToUniversalTime(),
                               input.IsInTrialPeriod,
                               AppUrlService.CreateEmailActivationUrlFormat(input.TenancyName),
                               input.UseDefaultAccount,
                               input.AutoItemCode,
                               input.Prifix,
                               input.ItemCode,
                               input.Subscription,
                               input.UseBatchNo,
                               input.DefaultInventoryReportTemplate,
                               input.UserName,
                               input.Subscription.SubscriptionPromotions,
                               input.SignUpId);
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var userId = await _userRepository.GetAll().AsNoTracking().Select(s => s.Id).FirstOrDefaultAsync();
                    if (input.AutoItemCode)
                    {
                        var inputApi = new CreateOrUpdateDefaultAutoItemCodeInput
                        {
                            ItemCode = input.ItemCode,
                            ItemCodeFormulaCustomId = null,
                            ItemCodeFormulaId = null,
                            Prefix = input.Prifix,
                            TenantId = tenantId,
                            UserId = userId,
                        };
                        await _tenantManager.AutoCreateItemCode(inputApi);
                    }
                    // default JournalTransactionType
                    var journalTransactionType = await _journalTransactionTransactionRepository.GetAll().AsNoTracking().Where(s => s.TenantId == tenantId).AnyAsync();

                    if (!journalTransactionType)
                    {

                        var list = new List<JournalTransactionType>
                {
                            JournalTransactionType.Create(tenantId, userId, "Purchase", false, true, enumStatus.EnumStatus.InventoryTransactionType.ItemReceiptPurchase),
                            JournalTransactionType.Create(tenantId, userId, "Sale Returns", false, true, enumStatus.EnumStatus.InventoryTransactionType.ItemReceiptCustomerCredit),
                            JournalTransactionType.Create(tenantId, userId, "Transfer", false, true, enumStatus.EnumStatus.InventoryTransactionType.ItemReceiptTransfer),
                            JournalTransactionType.Create(tenantId, userId, "Adjustment", false, true, enumStatus.EnumStatus.InventoryTransactionType.ItemReceiptAdjustment),
                            JournalTransactionType.Create(tenantId, userId, "Other", false, true, enumStatus.EnumStatus.InventoryTransactionType.ItemReceiptOther),
                            JournalTransactionType.Create(tenantId, userId, "Physical Count", false, true, enumStatus.EnumStatus.InventoryTransactionType.ItemReceiptPhysicalCount),
                            JournalTransactionType.Create(tenantId, userId, "Production", false, true, enumStatus.EnumStatus.InventoryTransactionType.ItemReceiptProduction),
                            JournalTransactionType.Create(tenantId, userId, "Sale", true, true, enumStatus.EnumStatus.InventoryTransactionType.ItemIssueSale),
                            JournalTransactionType.Create(tenantId, userId, "Purchase Return", true, true, enumStatus.EnumStatus.InventoryTransactionType.ItemIssueVendorCredit),
                            JournalTransactionType.Create(tenantId, userId, "Transfer", true, true, enumStatus.EnumStatus.InventoryTransactionType.ItemIssueTransfer),
                            JournalTransactionType.Create(tenantId, userId, "Adjustment", true, true, enumStatus.EnumStatus.InventoryTransactionType.ItemIssueAdjustment),
                            JournalTransactionType.Create(tenantId, userId, "Other", true, true, enumStatus.EnumStatus.InventoryTransactionType.ItemIssueOther),
                            JournalTransactionType.Create(tenantId, userId, "Physical Count", true, true, enumStatus.EnumStatus.InventoryTransactionType.ItemIssuePhysicalCount),
                            JournalTransactionType.Create(tenantId, userId, "Production", true, true, enumStatus.EnumStatus.InventoryTransactionType.ItemIssueProduction),
                            JournalTransactionType.Create(tenantId, userId, "Kitchen Order", true, true, enumStatus.EnumStatus.InventoryTransactionType.ItemIssueKitchenOrder),
               };
                        await _journalTransactionTransactionRepository.BulkInsertAsync(list);
                    }
                    await _defaultChartOfAccountManager.CreateDefaultChartAccounts(tenantId);
                }
                if (input.SignUpId != null)
                {
                    var signUp = await _signUpRepository.GetAll().Where(s => s.Id == input.SignUpId.Value).FirstOrDefaultAsync();
                    if (signUp != null)
                    {
                        signUp.UpdateTenant(tenantId);
                        if (input.Subscription.IsTrail)
                        {
                            signUp.UpdateEnumStatus(SignUp.EnumStatus.FreeTrail);
                        }
                        else
                        {
                            signUp.UpdateEnumStatus(SignUp.EnumStatus.Subscribed);
                        }

                        await _signUpRepository.UpdateAsync(signUp);
                    }
                }
                if (input.DefaultInventoryReportTemplate)
                {
                    var userId = await _tenantRepository.GetAll().AsNoTracking().Where(t => t.Id == tenantId).Select(t => t.CreatorUserId).FirstOrDefaultAsync();
                    await CreateInvertoryReportTemplate(inputs, tenantId, userId);
                }
                await uow.CompleteAsync();
            }


        }


        [AbpAuthorize(AppPermissions.Pages_Tenants_Edit)]
        public async Task<TenantEditDto> GetTenantForEdit(EntityDto input)
        {
            var tenant = await TenantManager.GetByIdAsync(input.Id);
            var tenantEditDto = ObjectMapper.Map<TenantEditDto>(tenant);
            if (tenant.ItemCodeSetting == ItemCodeSetting.Auto)
            {
                tenantEditDto.AutoItemCode = true;
            }
            tenantEditDto.ConnectionString = SimpleStringCipher.Instance.Decrypt(tenantEditDto.ConnectionString);

            tenantEditDto.Subscription = await GetSubscriptionHelper(tenantEditDto.SubScriptionId.Value);

            if (tenantEditDto.Subscription.UpgradeFromSubscriptionId.HasValue && tenantEditDto.Subscription.UpgradeFromSubscriptionId != Guid.Empty)
            {
                tenantEditDto.UpgradeFromSubscription = await GetSubscriptionHelper(tenantEditDto.Subscription.UpgradeFromSubscriptionId.Value);
            }

            return tenantEditDto;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_Create, AppPermissions.Pages_Tenants_Edit)]
        public async Task<GetSubscriptionInput> GetTenantSubscription(EntityDto<Guid> input)
        {
            var result = await GetSubscriptionHelper(input.Id);

            return result;
        }

        private async Task<GetSubscriptionInput> GetSubscriptionHelper(Guid id)
        {
            var subscription = await _subScriptionRepository.GetAll()
                                              .AsNoTracking()
                                              .Where(t => t.Id == id)
                                              .Select(t => new GetSubscriptionInput
                                              {
                                                  Duration = t.Duration,
                                                  DurationType = t.DurationType,
                                                  EditionId = t.EditionId,
                                                  EndDate = t.Endate,
                                                  Id = t.Id,
                                                  StartDate = t.StartDate,
                                                  TenantId = t.TenantId,
                                                  Unlimited = t.Unlimited,
                                                  IsTrail = t.IsTrail,
                                                  ShowWarning = t.ShowWarning,
                                                  InvoiceDate = t.InvoiceDate,
                                                  PackageId = t.PackageId,
                                                  SubscriptionDate = t.SubscriptionDate,
                                                  UpgradeFromSubscriptionId = t.UpgradeFromSubscriptionId,
                                                  Discount = t.Discount,
                                                  UpgradeDeduction = t.UpgradeDeduction,
                                                  PackagePrice = t.PackagePrice,
                                                  SubscriptionType = t.SubscriptionType,
                                                  TotalPrice = t.TotalPrice,

                                              }).FirstOrDefaultAsync();

            var promotionQuery = from p in _subscriptionPromotionRepository.GetAll()
                                           .AsNoTracking()
                                           .Where(t => t.SubscriptionId == id)
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
                                 select new SubscriptionPromotionInput
                                 {
                                     Id = p.Id,
                                     PromotionId = p.PromotionId,
                                     PromotionName = p.PromotionId.HasValue ? p.Promotion.PromotionName : "",
                                     SubscriptionId = p.SubscriptionId,
                                     PromotionType = p.PromotionId.HasValue ? p.Promotion.PromotionType : 0,
                                     IsRenewable = p.IsRenewable,
                                     IsEligibleWithOther = p.IsEligibleWithOther,
                                     CampaignId = p.CampaignId,
                                     IsSpecificPackage = p.IsSpecificPackage,
                                     IsTrial = p.PromotionId.HasValue && p.Promotion.IsTrial,
                                     Value = !p.PromotionId.HasValue ? 0 : p.Promotion.PromotionType == PromotionType.Discount ? p.Promotion.DiscountRate : p.Promotion.ExtraMonth,
                                     CampaignEditionPromotions = pros.OrderBy(s => s.SortOrder).ToList()
                                 };

            subscription.SubscriptionPromotions = await promotionQuery.ToListAsync();

            return subscription;
        }



        [AbpAuthorize(AppPermissions.Pages_Tenants_Edit)]
        public async Task UpdateTenant(TenantEditDto input)
        {
            if (!input.Subscription.Unlimited)
            {
                if (input.Subscription.StartDate == null)
                {
                    throw new UserFriendlyException(L("StartDateIsRequired"));
                }
                else if ((input.Subscription.Duration == null || input.Subscription.Duration == 0) && !input.Subscription.IsTrail)
                {
                    throw new UserFriendlyException(L("InvalidDuration"));
                }
            }

            await ValidatePromotions(input.Subscription.SubscriptionPromotions);

            await TenantManager.CheckEditionAsync(input.EditionId, input.IsInTrialPeriod);

            input.ConnectionString = SimpleStringCipher.Instance.Encrypt(input.ConnectionString);
            var tenant = await TenantManager.GetByIdAsync(input.Id);
            var oldEditionId = tenant.EditionId;
            ObjectMapper.Map(input, tenant);
            tenant.SubscriptionEndDateUtc = tenant.SubscriptionEndDateUtc?.ToUniversalTime();
            tenant.SetUseBatchNo(input.UseBatchNo);
            // tenant.SetAutoItemCode(input.AutoItemCode ? ItemCodeSetting.Auto : ItemCodeSetting.Custom);         
            tenant.setDefaultInventoryReport(input.DefaultInventoryReportTemplate);
            var subscription = await _subScriptionRepository.GetAll().Where(t => t.Id == tenant.SubscriptionId).FirstOrDefaultAsync();
            Subscription oldSubscription = null;

            if (subscription.UpgradeFromSubscriptionId.HasValue)
            {
                oldSubscription = await _subScriptionRepository.GetAll().AsNoTracking().Where(t => t.Id == subscription.UpgradeFromSubscriptionId).FirstOrDefaultAsync();
            }
            var signUp = await _signUpRepository.GetAll().Where(s => s.TenantId == tenant.Id).FirstOrDefaultAsync();
            if (signUp != null)
            {
                if (input.Subscription.IsTrail)
                {
                    signUp.UpdateEnumStatus(SignUp.EnumStatus.FreeTrail);
                }
                else
                {
                    signUp.UpdateEnumStatus(SignUp.EnumStatus.Subscribed);
                }
                await _signUpRepository.UpdateAsync(signUp);
            }
            if (input.EditionId.HasValue &&
                input.Subscription.PackageId.HasValue && input.Subscription.PackageId != Guid.Empty &&
                input.Subscription.DurationType.HasValue &&
                !input.Subscription.Unlimited &&
                !input.Subscription.IsTrail)
            {
                //var edition = (SubscribableEdition)await _editionManager.FindByIdAsync(input.EditionId.Value);
                //if (edition == null) throw new UserFriendlyException(L("IsNotValid", L("Edition")));
                var packageEdition = await _packageEditionRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.PackageId == input.Subscription.PackageId && s.EditionId == input.EditionId);
                if (packageEdition == null) throw new UserFriendlyException(L("IsNotValid", L("Edition")));

                input.Subscription.SubscriptionType = oldSubscription == null || oldSubscription.IsTrail ? SubscriptionType.Subscribe :
                                                      oldSubscription.EditionId == input.Subscription.EditionId.Value ? SubscriptionType.Renew : SubscriptionType.Upgrade;

                input.Subscription.PackagePrice = packageEdition.AnnualPrice;
                input.Subscription.UpgradeDeduction = 0;
                var remainingDays = 0;

                if (oldSubscription != null &&
                    !oldSubscription.IsTrail &&
                    !oldSubscription.Unlimited &&
                    oldSubscription.Endate.HasValue)
                {

                    var subscriptionDate = input.Subscription.SubscriptionDate.Value.Date;
                    var invoiceDate = oldSubscription.InvoiceDate.HasValue ? oldSubscription.InvoiceDate.Value.Date : oldSubscription.StartDate.Value.Date;
                    var startDate = invoiceDate > subscriptionDate ? invoiceDate : subscriptionDate;
                    remainingDays = Convert.ToInt32((oldSubscription.Endate.Value.ToDayEnd() - startDate).TotalDays);
                    input.Subscription.UpgradeDeduction = oldSubscription.IsTrail || oldSubscription.EditionId == input.EditionId || remainingDays <= 0 ? 0 :
                                                          Math.Round(oldSubscription.PackagePrice / 365 * remainingDays, 2);
                }

                var packagePrice = remainingDays <= 0 || input.Subscription.SubscriptionType != SubscriptionType.Upgrade ?
                                   packageEdition.AnnualPrice : Math.Round(packageEdition.AnnualPrice / 365 * remainingDays, 2);

                var upgradePrice = packagePrice - input.Subscription.UpgradeDeduction;

                List<Promotion> discounts = null;

                if (!input.Subscription.SubscriptionPromotions.IsNullOrEmpty())
                {
                    discounts = await _promotionRepository.GetAll().AsNoTracking()
                                     .Where(s => s.PromotionType == PromotionType.Discount)
                                     .Where(s => input.Subscription.SubscriptionPromotions.Any(r => r.PromotionId == s.Id))
                                     .ToListAsync();
                }

                input.Subscription.Discount = discounts == null ? 0 : Math.Round(upgradePrice * discounts.Sum(t => t.DiscountRate) / 100, 2);
                input.Subscription.TotalPrice = upgradePrice - input.Subscription.Discount;
            }

            subscription.Update(AbpSession.UserId.Value, input.Subscription.Duration, input.Subscription.SubscriptionDate, input.Subscription.StartDate.Value, input.Subscription.EndDate, input.EditionId.Value, input.Subscription.DurationType, input.Subscription.Unlimited, input.Subscription.IsTrail, input.Subscription.ShowWarning, input.Subscription.PackagePrice, input.Subscription.TotalPrice, input.Subscription.SubscriptionType, input.Subscription.InvoiceDate, input.Subscription.Discount, input.Subscription.UpgradeDeduction, input.Subscription.PackageId);

            var subscriptionPromotions = await _subscriptionPromotionRepository.GetAll().AsNoTracking().Where(t => t.SubscriptionId == subscription.Id).ToListAsync();
            var subscriptionCampaignPromtions = await _subscriptionCampaignPromotionRepository.GetAll().AsNoTracking().Where(s => s.SubscriptionPromotion.SubscriptionId == subscription.Id).ToListAsync();

            var addSubscriptionPromotions = new List<SubscriptionPromotion>();
            var editSubscriptionPromotions = new List<SubscriptionPromotion>();
            var addSubscriptionCampaignPromotions = new List<SubscriptionCampaignPromotion>();
            var editSubscriptionCampaignPromotions = new List<SubscriptionCampaignPromotion>();

            if (!input.Subscription.SubscriptionPromotions.IsNullOrEmpty())
            {
                var addPromotions = input.Subscription.SubscriptionPromotions.Where(s => !s.Id.HasValue || s.Id == Guid.Empty).ToList();
                var updatePromtions = input.Subscription.SubscriptionPromotions.Where(s => s.Id.HasValue && s.Id != Guid.Empty).ToList();

                if (addPromotions.Any())
                {
                    foreach (var t in addPromotions)
                    {
                        var promotion = SubscriptionPromotion.Create(AbpSession.UserId.Value, subscription.Id, t.CampaignId, t.PromotionId, t.IsRenewable, t.IsEligibleWithOther, t.IsSpecificPackage);

                        if (t.IsSpecificPackage && !t.CampaignEditionPromotions.IsNullOrEmpty())
                        {
                            var campaingPromotions = t.CampaignEditionPromotions.Select(s => SubscriptionCampaignPromotion.Create(AbpSession.UserId.Value, promotion.Id, s.CampaignId, s.EditionId, s.SortOrder, s.PromotionId)).ToList();
                            addSubscriptionCampaignPromotions.AddRange(campaingPromotions);
                        }

                        addSubscriptionPromotions.Add(promotion);
                    }

                }

                if (updatePromtions.Any())
                {
                    foreach (var i in updatePromtions)
                    {
                        var subscriptionPromotion = subscriptionPromotions.FirstOrDefault(s => s.Id == i.Id);
                        if (subscriptionPromotion == null) throw new UserFriendlyException(L("RecordNotFound"));

                        subscriptionPromotion.Update(AbpSession.UserId.Value, subscription.Id, i.CampaignId, i.PromotionId, i.IsRenewable, i.IsEligibleWithOther, i.IsSpecificPackage);
                        editSubscriptionPromotions.Add(subscriptionPromotion);

                        if (i.IsSpecificPackage && !i.CampaignEditionPromotions.IsNullOrEmpty())
                        {
                            var addCampaignPromotions = i.CampaignEditionPromotions.Where(s => !s.Id.HasValue || s.Id == Guid.Empty).ToList();
                            var updateCampaignPromotions = i.CampaignEditionPromotions.Where(s => s.Id.HasValue && s.Id != Guid.Empty).ToList();

                            if (addCampaignPromotions.Any()) addSubscriptionCampaignPromotions.AddRange(addCampaignPromotions.Select(s => SubscriptionCampaignPromotion.Create(AbpSession.UserId.Value, subscriptionPromotion.Id, s.CampaignId, s.EditionId, s.SortOrder, s.PromotionId)));

                            if (updateCampaignPromotions.Any())
                            {
                                foreach (var p in updateCampaignPromotions)
                                {
                                    var campaignPromotion = subscriptionCampaignPromtions.FirstOrDefault(s => s.Id == p.Id);
                                    if (campaignPromotion == null) throw new UserFriendlyException(L("RecordNotFound"));

                                    campaignPromotion.Update(AbpSession.UserId.Value, subscriptionPromotion.Id, p.CampaignId, p.EditionId, p.SortOrder, p.PromotionId);
                                    editSubscriptionCampaignPromotions.Add(campaignPromotion);
                                }
                            }

                        }
                    }
                }
            }

            var deleteSubscriptionPromotions = subscriptionPromotions.Where(s => !editSubscriptionPromotions.Any(r => r.Id == s.Id)).ToList();
            var deleteSubscriptionCampaignPromotions = subscriptionCampaignPromtions.Where(s => !editSubscriptionCampaignPromotions.Any(r => r.Id == s.Id)).ToList();

            if (deleteSubscriptionCampaignPromotions.Any()) await _subscriptionCampaignPromotionRepository.BulkDeleteAsync(deleteSubscriptionCampaignPromotions);
            if (deleteSubscriptionPromotions.Any()) await _subscriptionPromotionRepository.BulkDeleteAsync(deleteSubscriptionPromotions);

            if (addSubscriptionPromotions.Any()) await _subscriptionPromotionRepository.BulkInsertAsync(addSubscriptionPromotions);
            if (addSubscriptionCampaignPromotions.Any()) await _subscriptionCampaignPromotionRepository.BulkInsertAsync(addSubscriptionCampaignPromotions);
            if (editSubscriptionPromotions.Any()) await _subscriptionPromotionRepository.BulkUpdateAsync(editSubscriptionPromotions);
            if (editSubscriptionCampaignPromotions.Any()) await _subscriptionCampaignPromotionRepository.BulkUpdateAsync(editSubscriptionCampaignPromotions);

            await _subScriptionRepository.UpdateAsync(subscription);
            await TenantManager.UpdateAsync(tenant);
            await CurrentUnitOfWork.SaveChangesAsync();
            using (_unitOfWorkManager.Current.SetTenantId(tenant.Id))
            {
                await _defaultChartOfAccountManager.CreateDefaultChartAccounts(input.Id);
            }

            using (_unitOfWorkManager.Current.SetTenantId(input.Id))
            {
                if (oldEditionId != input.EditionId)
                {
                    await UpdateRoleStaticByFeature(input.EditionId, tenant.Id);
                }
                await _defaultChartOfAccountManager.CreateDefaultChartAccounts(input.Id);
            }
            if (input.DefaultInventoryReportTemplate)
            {

                var date = DateTime.Now;
                var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                var dateValue = $"thisMonth,{firstDayOfMonth.ToString("MMM dd, yyyy") + " 07:00:00 GMT+0700 (Indochina Time) " + lastDayOfMonth.ToString("MMMM dd , yyyy") + " 07:00:00 GMT+0700 (Indochina Time)"}";
                List<CreateTemplate> inputs = new List<CreateTemplate>
                {
                      new CreateTemplate{
                          ReportType = ReportType.ReportType_StockBalance,
                          TemplateType = TemplateType.Gloable,
                          reportCategory = ReportCategory.InventoryReport,
                          PermissionReadWrite = PermissionReadWrite.Read,
                          Groupby = null,
                          DefaultTemplateReport =  null,
                          DefaultTemplateReport2 = null,
                          DefaultTemplateReport3 = null,
                          TemplateName = "សមតុល្យស្ដុក",
                          HeaderTitle = "សមតុល្យស្ដុក",
                          Sortby = null,
                          IsActive = false,
                          IsDefault = false,
                          MemberGroupItemTamplate = new List<CreateOrUpdateMemberGroupItemTamplate>{},
                          Columns = new List<GetColumnTemplateOutput>{
                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "ItemCode",
                            ColumnLength = 130,
                            ColumnTitle = "លេខកូដទំនិញ",
                            ColumnType = ColumnType.String,
                            SortOrder = 1,
                            Visible = true,
                            AllowFunction = null,
                            IsDisplay = true,
                        },
                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "ItemName",
                            ColumnLength = 250,
                            ColumnTitle = "ឈ្មោះទំនិញ",
                            ColumnType = ColumnType.String,
                            SortOrder = 2,
                            Visible = true,
                            AllowFunction = null,
                            IsDisplay = true,
                        },
                        new GetColumnTemplateOutput{
                            AllowGroupby = true,
                            AllowFilter = true,
                            ColumnName = "Lot",
                            ColumnLength = 126,
                            ColumnTitle = "ទីតាំង",
                            ColumnType = ColumnType.String,
                            SortOrder = 4,
                            Visible = true,
                            AllowFunction = null,
                            IsActive = true,
                            IsDisplay = true,
                        },
                        new GetColumnTemplateOutput
                        {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Beginning",
                        ColumnLength = 117,
                        ColumnTitle = "ស្តុកដើមគ្រា",
                        ColumnType = ColumnType.Number,
                        SortOrder = 5,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        IsActive = true,
                        },

                        new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalInQty",
                        ColumnLength = 118,
                        ColumnTitle = "ស្តុកចូល",
                        ColumnType = ColumnType.Number,
                        SortOrder = 6,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        IsActive = true,
                    },
                    new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalOutQty",
                        ColumnLength = 118,
                        ColumnTitle = "ស្តុកចេញ",
                        ColumnType = ColumnType.Number,
                        SortOrder = 7,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        IsActive = true,
                    },
                       new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalQtyOnHand",
                        ColumnLength = 114,
                        ColumnTitle = "ស្តុកចុងគ្រា",
                        ColumnType = ColumnType.Number,
                        SortOrder = 8,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        IsActive =true,
                    },
                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "NetWeight",
                            ColumnLength = 95,
                            ColumnTitle = "ចំនួន (Kg)",
                            ColumnType = ColumnType.Number,
                            SortOrder = 10,
                            Visible = false,
                            AllowFunction = "Sum",
                            IsActive =false,
                            IsDisplay = false
                       },

                        new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "NetWeight",
                        ColumnLength = 95,
                        ColumnTitle = "ចំនួន (Kg)",
                        ColumnType = ColumnType.Number,
                        SortOrder = 10,
                        Visible = false,
                        AllowFunction = "Sum",
                        IsActive =true,
                        IsDisplay = false
                    },
                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = true,
                            ColumnName = "Unit",
                            ColumnLength = 100,
                            ColumnTitle = "ខ្នាត",
                            ColumnType = ColumnType.ItemProperty,
                            SortOrder = 14,
                            Visible = true,
                            AllowFunction = null,
                            IsDisplay =  true,
                        },
                          },

                           Filters = new List<GetFilterTemplateOutput>{
                                              new GetFilterTemplateOutput{
                                                 Visible = false,
                                                 AllowShowHideFilter = false,
                                                 ShowHideFilter = false,
                                                 DefaultValueId = null,
                                                 FilterId = null,
                                                 FilterName = "Search",
                                                 FilterType = ColumnType.Language,
                                                 FilterValue = null,
                                                 IsActive = true,
                                                 SortOrder = 0
                                               },
                                              new GetFilterTemplateOutput{
                                                 Visible = true,
                                                 AllowShowHideFilter = false,
                                                 ShowHideFilter = false,
                                                 DefaultValueId = dateValue,
                                                 FilterId = null,
                                                 FilterName = "DateRange",
                                                 FilterType = ColumnType.Language,
                                                 FilterValue = null,
                                                 IsActive = true,
                                                 SortOrder = 0
                                               },
                           },
                      },
                      new CreateTemplate{
                          ReportType = ReportType.ReportType_Inventory,
                          TemplateType = TemplateType.Gloable,
                          reportCategory = ReportCategory.InventoryReport,
                          PermissionReadWrite = PermissionReadWrite.Read,
                          Groupby = null,
                          DefaultTemplateReport =  null,
                          DefaultTemplateReport2 = null,
                          DefaultTemplateReport3 = null,
                          TemplateName = "របាយការណ៍តំលៃស្តុកសង្ខេប",
                          HeaderTitle = "របាយការណ៍តំលៃស្តុកសង្ខេប",
                          Sortby = null,
                          IsActive = false,
                          IsDefault = false,
                          MemberGroupItemTamplate = new List<CreateOrUpdateMemberGroupItemTamplate>{},
                          Columns = new List<GetColumnTemplateOutput>{
                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "ItemCode",
                            ColumnLength = 130,
                            ColumnTitle = "លេខកូដទំនិញ",
                            ColumnType = ColumnType.String,
                            SortOrder = 1,
                            Visible = true,
                            AllowFunction = null,
                            IsDisplay = true,
                        },
                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "ItemName",
                            ColumnLength = 196,
                            ColumnTitle = "ឈ្មោះទំនិញ",
                            ColumnType = ColumnType.String,
                            SortOrder = 2,
                            Visible = true,
                            AllowFunction = null,
                            IsDisplay = true,
                        },
                        new GetColumnTemplateOutput{
                            AllowGroupby = true,
                            AllowFilter = true,
                            ColumnName = "Location",
                            ColumnLength = 104,
                            ColumnTitle = "ទីតាំង",
                            ColumnType = ColumnType.String,
                            SortOrder = 4,
                            Visible = true,
                            AllowFunction = null,
                            IsActive = true,
                            IsDisplay = true,
                        },
                        new GetColumnTemplateOutput{
                            AllowGroupby = true,
                            AllowFilter = true,
                            ColumnName = "Date",
                            ColumnLength = 126,
                            ColumnTitle = "ថ្ងៃខែ",
                            ColumnType = ColumnType.Date,
                            SortOrder = 4,
                            Visible = true,
                            AllowFunction = null,
                            IsActive = true,
                            IsDisplay = true,
                        },

                        new GetColumnTemplateOutput
                        {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Beginning",
                        ColumnLength = 108,
                        ColumnTitle = "ស្តុកដើមគ្រា",
                        ColumnType = ColumnType.Number,
                        SortOrder = 5,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        IsActive = true,
                        },

                        new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalInQty",
                        ColumnLength = 100,
                        ColumnTitle = "ស្តុកចូល",
                        ColumnType = ColumnType.Number,
                        SortOrder = 6,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        IsActive = true,
                    },
                    new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalOutQty",
                        ColumnLength = 100,
                        ColumnTitle = "ស្តុកចេញ",
                        ColumnType = ColumnType.Number,
                        SortOrder = 7,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        IsActive = true,
                    },
                       new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalQtyOnHand",
                        ColumnLength = 114,
                        ColumnTitle = "ស្តុកចុងគ្រា",
                        ColumnType = ColumnType.Number,
                        SortOrder = 8,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        IsActive =true,
                    },
                         new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "AverageCost",
                        ColumnLength = 100,
                        ColumnTitle = "ថ្លៃមធ្យម",
                        ColumnType = ColumnType.RoundingDigit,
                        SortOrder = 10,
                        Visible = true,
                        IsDisplay = true,

                    },
                         new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "DisplayTotalCost",
                        ColumnLength = 100,
                        ColumnTitle = "តម្លៃសរុប",
                        ColumnType = ColumnType.Money,
                        SortOrder = 11,
                        Visible = true,
                        AllowFunction = "Sum",
                        IsDisplay = true
                    },

                          },

                           Filters = new List<GetFilterTemplateOutput>{
                                              new GetFilterTemplateOutput{
                                                 Visible = false,
                                                 AllowShowHideFilter = false,
                                                 ShowHideFilter = false,
                                                 DefaultValueId = null,
                                                 FilterId = null,
                                                 FilterName = "Search",
                                                 FilterType = ColumnType.Language,
                                                 FilterValue = null,
                                                 IsActive = true,
                                                 SortOrder = 0
                                               },
                                              new GetFilterTemplateOutput{
                                                 Visible = true,
                                                 AllowShowHideFilter = false,
                                                 ShowHideFilter = false,
                                                 DefaultValueId = dateValue,
                                                 FilterId = null,
                                                 FilterName = "DateRange",
                                                 FilterType = ColumnType.Language,
                                                 FilterValue = null,
                                                 IsActive = true,
                                                 SortOrder = 0
                                               },
                           },
                      },

                       new CreateTemplate{
                          ReportType = ReportType.ReportType_InventoryDetail,
                          TemplateType = TemplateType.Gloable,
                          reportCategory = ReportCategory.InventoryReport,
                          PermissionReadWrite = PermissionReadWrite.Read,
                          Groupby = null,
                          DefaultTemplateReport =  null,
                          DefaultTemplateReport2 = null,
                          DefaultTemplateReport3 = null,
                          TemplateName = "របាយការណ៍តំលៃស្តុកលម្អិត",
                          HeaderTitle = "របាយការណ៍តំលៃស្តុកលម្អិត",
                          Sortby = null,
                          IsActive = false,
                          IsDefault = false,
                          MemberGroupItemTamplate = new List<CreateOrUpdateMemberGroupItemTamplate>{},
                          Columns = new List<GetColumnTemplateOutput>{

                        new GetColumnTemplateOutput{
                            AllowGroupby = true,
                            AllowFilter = false,
                            ColumnName = "JournalNo",
                            ColumnLength = 96,
                            ColumnTitle = "លេខឯកសារ",
                            ColumnType = ColumnType.String,
                            SortOrder = 2,
                            Visible = true,
                            AllowFunction = null,
                            IsDisplay = true,
                            IsActive =true
                        },
                        new GetColumnTemplateOutput{
                        AllowGroupby = true,
                        AllowFilter = true,
                        ColumnName = "JournalType",
                        ColumnLength = 90,
                        ColumnTitle = "ប្រតិបត្តិការ",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,

                        },
                        new GetColumnTemplateOutput{
                            AllowGroupby = true,
                            AllowFilter = true,
                            ColumnName = "Date",
                            ColumnLength = 80,
                            ColumnTitle = "ថ្ងៃខែ",
                            ColumnType = ColumnType.Date,
                            SortOrder = 1,
                            Visible = true,
                            AllowFunction = null,
                            IsActive = true,
                            IsDisplay = true,
                        },
                        new GetColumnTemplateOutput{
                            AllowGroupby = true,
                            AllowFilter = true,
                            ColumnName = "Location",
                            ColumnLength = 120,
                            ColumnTitle = "ទីតាំង",
                            ColumnType = ColumnType.String,
                            SortOrder = 5,
                            Visible = true,
                            AllowFunction = null,
                            IsActive = true,
                            IsDisplay = true,
                        },
                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "InQty",
                            ColumnLength = 90,
                            ColumnTitle = "ស្តុកចូល",
                            ColumnType = ColumnType.Number,
                            SortOrder =6,
                            Visible = true,
                            IsDisplay = true,
                    },
                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "OutQty",
                            ColumnLength = 90,
                            ColumnTitle = "ស្តុកចេញ",
                            ColumnType = ColumnType.Number,
                            SortOrder = 7,
                            Visible = true,
                            IsDisplay = true,

                    },
                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "TotalQty",
                            ColumnLength = 100,
                            ColumnTitle = "ស្តុកចុងគ្រា",
                            ColumnType = ColumnType.Number,
                            SortOrder = 10,
                            Visible = true,
                            IsDisplay = true,

                        },
                        new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "UnitCost",
                        ColumnLength = 90,
                        ColumnTitle = "តម្លៃឯកតា",
                        ColumnType = ColumnType.RoundingDigit,
                        SortOrder = 8,
                        Visible = true,
                        IsDisplay = true,

                    },
                    new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "LineTotal",
                        ColumnLength = 100,
                        ColumnTitle = "សរុប",
                        ColumnType = ColumnType.Money,
                        SortOrder = 9,
                        Visible = true,
                        IsDisplay = true,

                    },
                    new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "AVGCost",
                        ColumnLength = 90,
                        ColumnTitle = "ថ្លៃមធ្យម",
                        ColumnType = ColumnType.RoundingDigit,
                        SortOrder = 11,
                        Visible = true,
                        IsDisplay = true,
                    },
                     new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalCost",
                        ColumnLength = 100,
                        ColumnTitle = "តម្លៃសរុប",
                        ColumnType = ColumnType.Money,
                        SortOrder = 12,
                        Visible = true,
                        IsDisplay = true,

                    },
                        },

                           Filters = new List<GetFilterTemplateOutput>{
                                              new GetFilterTemplateOutput{
                                                 Visible = false,
                                                 AllowShowHideFilter = false,
                                                 ShowHideFilter = false,
                                                 DefaultValueId = null,
                                                 FilterId = null,
                                                 FilterName = "Search",
                                                 FilterType = ColumnType.Language,
                                                 FilterValue = null,
                                                 IsActive = true,
                                                 SortOrder = 0
                                               },
                                              new GetFilterTemplateOutput{
                                                 Visible = true,
                                                 AllowShowHideFilter = false,
                                                 ShowHideFilter = false,
                                                 DefaultValueId = dateValue,
                                                 FilterId = null,
                                                 FilterName = "DateRange",
                                                 FilterType = ColumnType.Language,
                                                 FilterValue = null,
                                                 IsActive = true,
                                                 SortOrder = 0
                                               },
                           },
                      },

                      new CreateTemplate{
                          ReportType = ReportType.ReportType_InventoryTransaction,
                          TemplateType = TemplateType.Gloable,
                          reportCategory = ReportCategory.InventoryReport,
                          PermissionReadWrite = PermissionReadWrite.Read,
                          Groupby = "Item",
                          DefaultTemplateReport =  null,
                          DefaultTemplateReport2 = null,
                          DefaultTemplateReport3 = null,
                          TemplateName = "ប្រវត្តិស្តុកតាមទំនិញ",
                          HeaderTitle = "ប្រវត្តិស្តុកតាមទំនិញ",
                          Sortby = null,
                          IsActive = false,
                          IsDefault = false,
                          MemberGroupItemTamplate = new List<CreateOrUpdateMemberGroupItemTamplate>{},
                          Columns = new List<GetColumnTemplateOutput>{


                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "Date",
                            ColumnLength = 100,
                            ColumnTitle = "ថ្ងៃខែ",
                            ColumnType = ColumnType.Date,
                            SortOrder = 1,
                            Visible = true,
                            AllowFunction = null,
                            IsDisplay = true,
                            IsActive = true,
                        },
                        new GetColumnTemplateOutput{
                             AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "ItemName",
                            ColumnLength = 150,
                            ColumnTitle = "ឈ្មោះទំនិញ",
                            ColumnType = ColumnType.String,
                            SortOrder = 3,
                            Visible = true,
                            AllowFunction = null,
                            IsDisplay = true,
                        },
                        new GetColumnTemplateOutput{
                            AllowGroupby = true,
                            AllowFilter = false,
                            ColumnName = "Type",
                            ColumnLength = 150,
                            ColumnTitle = "ប្រភេទប្រតិបត្តិការណ៍",
                            ColumnType = ColumnType.String,
                            SortOrder = 4,
                            Visible = false,
                            AllowFunction = null,
                            IsDisplay = false,
                            IsActive = true,

                        },

                         new GetColumnTemplateOutput
                        {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Beginning",
                        ColumnLength = 170,
                        ColumnTitle = "Beginning Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 10,
                        Visible = false,
                        IsDisplay = true,
                        DisableDefault = true,
                        AllowFunction = "Sum",
                        IsActive = false,
                        },

                        new GetColumnTemplateOutput
                        {
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "Reference",
                            ColumnLength = 100,
                            ColumnTitle = "ឯកសារយោង",
                            ColumnType = ColumnType.String,
                            SortOrder = 7,
                            Visible = true,
                            IsDisplay = true,
                            IsActive = true,
                        },
                        new GetColumnTemplateOutput{
                        AllowGroupby = true,
                        AllowFilter = false,
                        ColumnName = "JournalNo",
                        ColumnLength = 140,
                        ColumnTitle = "លេខឯកសារ",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        IsActive =true
                    },
                     new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalInQty",
                        ColumnLength = 100,
                        ColumnTitle = "ស្តុកចូល",
                        ColumnType = ColumnType.Number,
                        SortOrder = 7,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        IsActive = true,
                    },

                        new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalOutQty",
                        ColumnLength = 100,
                        ColumnTitle = "ស្តុកចេញ",
                        ColumnType = ColumnType.Number,
                        SortOrder = 7,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        IsActive = true,
                    },

                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "NetWeight",
                            ColumnLength = 95,
                            ColumnTitle = "ចំនួន (Kg)",
                            ColumnType = ColumnType.Number,
                            SortOrder = 10,
                            Visible = false,
                            AllowFunction = "Sum",
                            IsActive =false,
                            IsDisplay = false
                       },
                        new GetColumnTemplateOutput
                        {
                            AllowGroupby = true,
                            AllowFilter = false,
                            ColumnName = "PartnerName",
                            ColumnLength = 140,
                            ColumnTitle = "ដៃគូរ",
                            ColumnType = ColumnType.String,
                            SortOrder = 2,
                            Visible = true,
                            IsDisplay = true,
                            IsActive = true,
                        },
                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = true,
                            ColumnName = "Unit",
                            ColumnLength = 90,
                            ColumnTitle = "ខ្នាត",
                            ColumnType = ColumnType.ItemProperty,
                            SortOrder = 14,
                            Visible =  true,
                            AllowFunction = null,
                            IsDisplay =  true,
                        },

                        new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalQty",
                        ColumnLength = 150,
                        ColumnTitle = "ស្តុកចុងគ្រា",
                        ColumnType = ColumnType.Number,
                        SortOrder = 13,
                        Visible = true,
                        IsDisplay = true,
                        DisableDefault = true,
                        AllowFunction = "Sum",
                    },

                        },

                           Filters = new List<GetFilterTemplateOutput>{
                                              new GetFilterTemplateOutput{
                                                 Visible = false,
                                                 AllowShowHideFilter = false,
                                                 ShowHideFilter = false,
                                                 DefaultValueId = null,
                                                 FilterId = null,
                                                 FilterName = "Search",
                                                 FilterType = ColumnType.Language,
                                                 FilterValue = null,
                                                 IsActive = true,
                                                 SortOrder = 0
                                               },
                                              new GetFilterTemplateOutput{
                                                 Visible = true,
                                                 AllowShowHideFilter = false,
                                                 ShowHideFilter = false,
                                                 DefaultValueId = dateValue,
                                                 FilterId = null,
                                                 FilterName = "DateRange",
                                                 FilterType = ColumnType.Language,
                                                 FilterValue = null,
                                                 IsActive = true,
                                                 SortOrder = 0
                                               },
                           },
                      },

                      new CreateTemplate{
                          ReportType = ReportType.ReportType_InventoryTransaction,
                          TemplateType = TemplateType.Gloable,
                          reportCategory = ReportCategory.InventoryReport,
                          PermissionReadWrite = PermissionReadWrite.Read,
                          Groupby = "JournalTransactionType",
                          DefaultTemplateReport =  null,
                          DefaultTemplateReport2 = null,
                          DefaultTemplateReport3 = null,
                          TemplateName = "ប្រវត្តិស្តុកតាមប្រតិបត្តិការណ៍",
                          HeaderTitle = "ប្រវត្តិស្តុកតាមប្រតិបត្តិការណ៍",
                          Sortby = null,
                          IsActive = false,
                          IsDefault = false,
                          MemberGroupItemTamplate = new List<CreateOrUpdateMemberGroupItemTamplate>{},
                          Columns = new List<GetColumnTemplateOutput>{


                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "Date",
                            ColumnLength = 100,
                            ColumnTitle = "ថ្ងៃខែ",
                            ColumnType = ColumnType.Date,
                            SortOrder = 1,
                            Visible = true,
                            AllowFunction = null,
                            IsDisplay = true,
                            IsActive = true,
                        },
                        new GetColumnTemplateOutput{
                             AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "ItemName",
                            ColumnLength = 205,
                            ColumnTitle = "ឈ្មោះទំនិញ",
                            ColumnType = ColumnType.String,
                            SortOrder = 3,
                            Visible = true,
                            AllowFunction = null,
                            IsDisplay = true,
                        },

                        new GetColumnTemplateOutput{
                            AllowGroupby = true,
                            AllowFilter = false,
                            ColumnName = "JournalTransactionType",
                            ColumnLength = 150,
                            ColumnTitle = "ប្រភេទប្រតិបត្តិការណ៍",
                            ColumnType = ColumnType.String,
                            SortOrder = 4,
                            Visible = false,
                            AllowFunction = null,
                            IsDisplay = false,
                            IsActive = true,

                        },

                         new GetColumnTemplateOutput
                        {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Beginning",
                        ColumnLength = 170,
                        ColumnTitle = "Beginning Qty",
                        ColumnType = ColumnType.Number,
                        SortOrder = 10,
                        Visible = false,
                        IsDisplay = true,
                        DisableDefault = true,
                        AllowFunction = "Sum",
                        IsActive = false,
                        },

                        new GetColumnTemplateOutput
                        {
                            AllowGroupby = false,
                            AllowFilter = false,
                            ColumnName = "Reference",
                            ColumnLength = 100,
                            ColumnTitle = "ឯកសារយោង",
                            ColumnType = ColumnType.String,
                            SortOrder = 7,
                            Visible = true,
                            IsDisplay = true,
                            IsActive = true,
                        },
                        new GetColumnTemplateOutput{
                        AllowGroupby = true,
                        AllowFilter = false,
                        ColumnName = "JournalNo",
                        ColumnLength = 105,
                        ColumnTitle = "លេខឯកសារ",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        IsActive =true
                    },
                     new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalInQty",
                        ColumnLength = 100,
                        ColumnTitle = "ស្តុកចូល",
                        ColumnType = ColumnType.Number,
                        SortOrder = 7,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        IsActive = true,
                    },

                        new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalOutQty",
                        ColumnLength = 100,
                        ColumnTitle = "ស្តុកចេញ",
                        ColumnType = ColumnType.Number,
                        SortOrder = 7,
                        Visible = true,
                        IsDisplay = true,
                        AllowFunction = "Sum",
                        IsActive = true,
                    },                      
                       // new GetColumnTemplateOutput{
                       //     AllowGroupby = false,
                       //     AllowFilter = false,
                       //     ColumnName = "NetWeight",
                       //     ColumnLength = 95,
                       //     ColumnTitle = "ចំនួន (Kg)",
                       //     ColumnType = ColumnType.Number,
                       //     SortOrder = 10,
                       //     Visible = true,
                       //     AllowFunction = "Sum",
                       //     IsActive =true,
                       //     IsDisplay = true
                       //},
                        new GetColumnTemplateOutput
                        {
                            AllowGroupby = true,
                            AllowFilter = false,
                            ColumnName = "PartnerName",
                            ColumnLength = 135,
                            ColumnTitle = "ដៃគូរ",
                            ColumnType = ColumnType.String,
                            SortOrder = 2,
                            Visible = true,
                            IsDisplay = true,
                            IsActive = true,
                        },
                        new GetColumnTemplateOutput{
                            AllowGroupby = false,
                            AllowFilter = true,
                            ColumnName = "Unit",
                            ColumnLength = 90,
                            ColumnTitle = "ខ្នាត",
                            ColumnType = ColumnType.ItemProperty,
                            SortOrder = 14,
                            Visible =  true,
                            AllowFunction = null,
                            IsDisplay =  true,
                        },

                       new GetColumnTemplateOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "TotalQty",
                        ColumnLength = 140,
                        ColumnTitle = "ស្តុកចុងគ្រា",
                        ColumnType = ColumnType.Number,
                        SortOrder = 13,
                        Visible = true,
                        IsDisplay = true,
                        DisableDefault = true,
                        AllowFunction = "Sum",
                    },

                        },

                           Filters = new List<GetFilterTemplateOutput>{
                                              new GetFilterTemplateOutput{
                                                 Visible = false,
                                                 AllowShowHideFilter = false,
                                                 ShowHideFilter = false,
                                                 DefaultValueId = null,
                                                 FilterId = null,
                                                 FilterName = "Search",
                                                 FilterType = ColumnType.Language,
                                                 FilterValue = null,
                                                 IsActive = true,
                                                 SortOrder = 0
                                               },
                                              new GetFilterTemplateOutput{
                                                 Visible = true,
                                                 AllowShowHideFilter = false,
                                                 ShowHideFilter = false,
                                                 DefaultValueId = dateValue,
                                                 FilterId = null,
                                                 FilterName = "DateRange",
                                                 FilterType = ColumnType.Language,
                                                 FilterValue = null,
                                                 IsActive = true,
                                                 SortOrder = 0
                                               },
                           },
                      },


                };
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant,
                                                   AbpDataFilters.MustHaveTenant))
                {
                    var defaults = await _reportTemplateRepository.GetAll().AsNoTracking()
                                   .Where(t => t.TenantId == tenant.Id && (t.ReportType == ReportType.ReportType_Inventory || t.ReportType == ReportType.ReportType_InventoryTransaction || t.ReportType == ReportType.ReportType_InventoryDetail ||
                                    t.ReportType == ReportType.ReportType_StockBalance)).Select(t => t.ReportType).ToListAsync();
                    var values = inputs.Where(t => !defaults.Contains(t.ReportType)).ToList();
                    var userId = await _tenantRepository.GetAll().AsNoTracking().Where(t => t.Id == input.Id).Select(t => t.CreatorUserId).FirstOrDefaultAsync();
                    await CreateInvertoryReportTemplate(values, input.Id, userId);
                }


            }


        }

        private async Task UpdateRoleStaticByFeature(int? editionId, int tenantId)
        {
            var featureValues = (await _editionManager.GetFeatureValuesAsync(editionId.Value)).Where(t => t.Value.Contains("true")).ToList();
            var roles = new List<Role>();
            var oldRoles = await _roleRepository.GetAll().Include(u => u.Permissions).Where(t => t.TenantId == tenantId && t.IsStatic || (t.Name == StaticRoleNames.Tenants.APAccountant || t.Name == StaticRoleNames.Tenants.ARAccountant || t.Name == StaticRoleNames.Tenants.StockController)).Select(t => t).ToListAsync();
            roles.Add(new Role(tenantId, StaticRoleNames.Tenants.Admin, StaticRoleNames.Tenants.Admin));
            if (featureValues.Select(t => t.Name).Contains(AppFeatures.AccountingFeature))
            {

                roles.Add(new Role(tenantId, StaticRoleNames.Tenants.AccountingManager, StaticRoleNames.Tenants.AccountingManager));
                roles.Add(new Role(tenantId, StaticRoleNames.Tenants.APAccountant, StaticRoleNames.Tenants.APAccountant));
                roles.Add(new Role(tenantId, StaticRoleNames.Tenants.ARAccountant, StaticRoleNames.Tenants.ARAccountant));
            }

            if (featureValues.Select(t => t.Name).Contains(AppFeatures.VendorsFeaturePurchases))
            {
                roles.Add(new Role(tenantId, StaticRoleNames.Tenants.PurchaseManager, StaticRoleNames.Tenants.PurchaseManager));
            }

            if (featureValues.Select(t => t.Name).Contains(AppFeatures.CustomersFeature))
            {
                roles.Add(new Role(tenantId, StaticRoleNames.Tenants.SaleManager, StaticRoleNames.Tenants.SaleManager));
            }
            if (featureValues.Select(t => t.Name).Contains(AppFeatures.InventoryFeature))
            {
                roles.Add(new Role(tenantId, StaticRoleNames.Tenants.WarehouseManager, StaticRoleNames.Tenants.WarehouseManager));
                roles.Add(new Role(tenantId, StaticRoleNames.Tenants.StockController, StaticRoleNames.Tenants.StockController));
            }


            foreach (var role in roles)
            {
                var oldRole = oldRoles.Where(t => t.Name == role.Name).FirstOrDefault();
                if (oldRole != null)
                {
                    oldRole.IsStatic = oldRole.Name == StaticRoleNames.Tenants.APAccountant || oldRole.Name == StaticRoleNames.Tenants.ARAccountant || oldRole.Name == StaticRoleNames.Tenants.StockController ? false : true;
                    if (oldRole.Name == StaticRoleNames.Tenants.Admin)
                    {
                        await _roleManager.GrantAllPermissionsAsync(oldRole);
                        await _roleManager.UpdateAsync(oldRole);
                    }
                    else
                    {
                        if (oldRole.Name != StaticRoleNames.Tenants.APAccountant && oldRole.Name != StaticRoleNames.Tenants.ARAccountant && oldRole.Name != StaticRoleNames.Tenants.StockController)
                        {
                            await _roleManager.ResetAllPermissionsAsync(oldRole);
                            await _roleManager.GrantPermissionsToTenantRoleAsync(oldRole);
                            await _roleManager.UpdateAsync(oldRole);
                        }


                    }

                }
                else
                {
                    role.IsStatic = role.Name == StaticRoleNames.Tenants.APAccountant || role.Name == StaticRoleNames.Tenants.ARAccountant || role.Name == StaticRoleNames.Tenants.StockController ? false : true;
                    await _roleRepository.InsertAsync(role);
                    await CurrentUnitOfWork.SaveChangesAsync(); //It's done to get Id of the role.
                    await _roleManager.GrantPermissionsToTenantRoleAsync(role);
                }
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_Delete)]
        public async Task DeleteTenant(EntityDto input)
        {
            var tenant = await TenantManager.GetByIdAsync(input.Id);
            await TenantManager.DeleteAsync(tenant);
            //ToDo: should auto delete role base by this tenant too
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_ChangeFeatures)]
        public async Task<GetTenantFeaturesEditOutput> GetTenantFeaturesForEdit(EntityDto input)
        {
            var features = FeatureManager.GetAll()
                .Where(f => f.Scope.HasFlag(FeatureScopes.Tenant));
            var featureValues = await TenantManager.GetFeatureValuesAsync(input.Id);

            return new GetTenantFeaturesEditOutput
            {
                Features = ObjectMapper.Map<List<FlatFeatureDto>>(features).OrderBy(f => f.DisplayName).ToList(),
                FeatureValues = featureValues.Select(fv => new NameValueDto(fv)).ToList()
            };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_ChangeFeatures)]
        public async Task UpdateTenantFeatures(UpdateTenantFeaturesInput input)
        {
            await TenantManager.SetFeatureValuesAsync(input.Id, input.FeatureValues.Select(fv => new NameValue(fv.Name, fv.Value)).ToArray());
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_ChangeFeatures)]
        public async Task ResetTenantSpecificFeatures(EntityDto input)
        {
            await TenantManager.ResetAllFeaturesAsync(input.Id);
        }

        public async Task UnlockTenantAdmin(EntityDto input)
        {
            using (CurrentUnitOfWork.SetTenantId(input.Id))
            {
                var tenantAdmin = await UserManager.FindByNameAsync(AbpUserBase.AdminUserName);
                if (tenantAdmin != null)
                {
                    tenantAdmin.Unlock();
                }
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_Create)]
        public async Task RenewSubscription(GetSubscriptionInput input)
        {
            if (!input.Unlimited)
            {
                if (input.StartDate == null)
                {
                    throw new UserFriendlyException(L("StartDateIsRequired"));
                }
                else if ((input.Duration == null || input.Duration == 0) && !input.IsTrail)
                {
                    throw new UserFriendlyException(L("InvalidDuration"));
                }
            }

            await ValidatePromotions(input.SubscriptionPromotions);

            var oldSubscription = await _subScriptionRepository.GetAll().Where(t => t.Id == input.Id).FirstOrDefaultAsync();
            if (oldSubscription != null && oldSubscription.SubscriptionDate.HasValue && input.SubscriptionDate.HasValue && oldSubscription.SubscriptionDate.Value.Date > input.SubscriptionDate.Value.Date)
            {
                throw new UserFriendlyException(L("CanNotRenewSubscrption", oldSubscription.SubscriptionDate.Value.ToString("MMMM dd, yyyy")));
            }
            if (input.SubscriptionDate != null)
            {
                oldSubscription.SetRenewEndDate(input.SubscriptionDate.Value.AddDays(-1));
                await _subScriptionRepository.UpdateAsync(oldSubscription);
            }


            if (input.EditionId.HasValue &&
                input.PackageId.HasValue && input.PackageId != Guid.Empty &&
                input.DurationType.HasValue &&
               !input.Unlimited &&
               !input.IsTrail)
            {
                //var edition = (SubscribableEdition)await _editionManager.FindByIdAsync(input.EditionId.Value);
                //if (edition == null) throw new UserFriendlyException(L("IsNotValid", L("Edition")));
                var packageEdition = await _packageEditionRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.PackageId == input.PackageId && s.EditionId == input.EditionId);
                if (packageEdition == null) throw new UserFriendlyException(L("IsNotValid", L("Edition")));

                input.SubscriptionType = oldSubscription.IsTrail ? SubscriptionType.Subscribe : oldSubscription.EditionId == input.EditionId.Value ? SubscriptionType.Renew : SubscriptionType.Upgrade;
                input.PackagePrice = packageEdition.AnnualPrice;

                input.UpgradeDeduction = 0;
                var remainingDays = 0;

                if (oldSubscription != null &&
                  !oldSubscription.IsTrail &&
                  !oldSubscription.Unlimited &&
                  oldSubscription.Endate.HasValue)
                {
                    var subscriptionDate = input.SubscriptionDate.Value.Date;
                    var invoiceDate = oldSubscription.InvoiceDate.HasValue ? oldSubscription.InvoiceDate.Value.Date : oldSubscription.StartDate.Value.Date;
                    var startDate = invoiceDate > subscriptionDate ? invoiceDate : subscriptionDate;
                    remainingDays = Convert.ToInt32((oldSubscription.Endate.Value.ToDayEnd() - startDate).TotalDays);
                    input.UpgradeDeduction = oldSubscription.IsTrail || oldSubscription.EditionId == input.EditionId || remainingDays <= 0 ? 0 :
                                             Math.Round(oldSubscription.PackagePrice / 365 * remainingDays, 2);
                }


                var packagePrice = remainingDays <= 0 || input.SubscriptionType != SubscriptionType.Upgrade ?
                                   packageEdition.AnnualPrice : Math.Round(packageEdition.AnnualPrice / 365 * remainingDays, 2);

                var upgradePrice = packagePrice - input.UpgradeDeduction;

                List<Promotion> discounts = null;
                if (!input.SubscriptionPromotions.IsNullOrEmpty())
                {
                    discounts = await _promotionRepository.GetAll().AsNoTracking()
                                     .Where(s => s.PromotionType == PromotionType.Discount)
                                     .Where(s => input.SubscriptionPromotions.Any(r => r.PromotionId == s.Id))
                                     .ToListAsync();
                }

                input.Discount = discounts == null ? 0 : Math.Round(upgradePrice * discounts.Sum(t => t.DiscountRate) / 100, 2);
                input.TotalPrice = upgradePrice - input.Discount;
            }

            var tenant = await _tenantRepository.GetAll().Where(t => t.Id == input.TenantId && t.SubscriptionId == input.Id).FirstOrDefaultAsync();
            var susbscription = Subscription.Create(tenant.Id, AbpSession.UserId, input.Duration, input.SubscriptionDate, input.StartDate, input.EndDate, input.EditionId, input.DurationType, input.Unlimited, input.IsTrail, input.ShowWarning, input.PackagePrice, input.TotalPrice, input.SubscriptionType, input.InvoiceDate, input.Discount, input.UpgradeDeduction, input.PackageId);
            susbscription.SetUpdateFromSubscription(oldSubscription.Id);
            await _subScriptionRepository.InsertAsync(susbscription);
            await _unitOfWorkManager.Current.SaveChangesAsync();
            if (!input.SubscriptionPromotions.IsNullOrEmpty())
            {
                var addSubscriptionPromotions = new List<SubscriptionPromotion>();
                var addSubscriptionCampaignPromotions = new List<SubscriptionCampaignPromotion>();

                foreach (var t in input.SubscriptionPromotions)
                {
                    var promotion = SubscriptionPromotion.Create(AbpSession.UserId.Value, susbscription.Id, t.CampaignId, t.PromotionId, t.IsRenewable, t.IsEligibleWithOther, t.IsSpecificPackage);

                    if (t.IsSpecificPackage && !t.CampaignEditionPromotions.IsNullOrEmpty())
                    {
                        var campaingPromotions = t.CampaignEditionPromotions.Select(s => SubscriptionCampaignPromotion.Create(AbpSession.UserId.Value, promotion.Id, s.CampaignId, s.EditionId, s.SortOrder, s.PromotionId)).ToList();

                        addSubscriptionCampaignPromotions.AddRange(campaingPromotions);
                    }

                    addSubscriptionPromotions.Add(promotion);
                }

                await _subscriptionPromotionRepository.BulkInsertAsync(addSubscriptionPromotions);
                if (addSubscriptionCampaignPromotions.Any()) await _subscriptionCampaignPromotionRepository.BulkInsertAsync(addSubscriptionCampaignPromotions);

            }

            tenant.SetSubScription(susbscription.Id, input.EditionId);

            var signUp = await _signUpRepository.GetAll().Where(s => s.TenantId == tenant.Id).FirstOrDefaultAsync();
            if (signUp != null)
            {
                if (input.IsTrail)
                {
                    signUp.UpdateEnumStatus(SignUp.EnumStatus.FreeTrail);
                }
                else
                {
                    signUp.UpdateEnumStatus(SignUp.EnumStatus.Subscribed);
                }
                await _signUpRepository.UpdateAsync(signUp);
            }
            await _tenantRepository.UpdateAsync(tenant);

        }

        private async Task CreateInvertoryReportTemplate(List<CreateTemplate> inputs, int tenantId, long? userId)
        {


            foreach (var input in inputs)
            {

                var filters = input.Filters.Select(s =>
                            ReportFilterTemplate.Create(tenantId, userId, s.FilterName,
                                    s.FilterValue, s.Visible, s.SortOrder, s.FilterType, s.DefaultValueId, s.AllowShowHideFilter, s.ShowHideFilter)).ToList();

                var columns = input.Columns.Select(s =>
                                ReportColumnTemplate.Create(tenantId, userId,
                                    s.ColumnName,
                                    s.ColumnTitle,
                                    s.ColumnLength,
                                    s.ColumnType,
                                    s.SortOrder,
                                    s.Visible,
                                    s.AllowGroupby,
                                    s.AllowFilter,
                                    s.AllowFunction,
                                    s.DisableDefault,
                                    s.IsDisplay)
                                ).ToList();

                var @entity = ReportTemplate.Create(tenantId, userId, input.ReportType,
                                input.TemplateName, input.TemplateType, input.reportCategory,
                                input.HeaderTitle, input.Sortby,
                                input.Groupby, input.IsDefault, input.DefaultTemplateReport, filters, columns,
                                input.PermissionReadWrite, input.DefaultTemplateReport2, input.DefaultTemplateReport3);

                //Add User Group item 
                if ((input.TemplateType == TemplateType.User || input.TemplateType == TemplateType.Group) && input.MemberGroupItemTamplate.Count() <= 0)
                {
                    throw new UserFriendlyException(L("PleaseAddItem"));
                }
                foreach (var i in input.MemberGroupItemTamplate)
                {
                    var @MemberGroupItemTamplate = GroupMemberItemTamplate.Create(tenantId, userId, i.UserGroupId, i.MemberUserId, i.PermissionReadWrite, entity);
                    i.Id = MemberGroupItemTamplate.Id;
                    CheckErrors(await _groupMemberItemTamplateManager.CreateAsync(@MemberGroupItemTamplate));
                }

                CheckErrors(await _reportTemplateManager.CreateAsync(@entity));



            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Common_Tenant_Find, AppPermissions.Pages_Host_Client)]
        public async Task<PagedResultDto<TenantSummaryDto>> FindTenants(FindTenantsInput input)
        {
            var query = (from t in TenantManager.Tenants
                                   .WhereIf(input.IsActive.HasValue, s => s.IsActive == input.IsActive.Value)
                                   .WhereIf(!input.Filter.IsNullOrWhiteSpace(), t => t.Name.Contains(input.Filter) || t.TenancyName.Contains(input.Filter))
                         select new TenantSummaryDto
                         {
                             Id = t.Id,
                             Name = t.Name,
                             TenancyName = t.TenancyName,
                         });

            var tenantCount = await query.CountAsync();
            var tenants = new List<TenantSummaryDto>();
            if (input.UsePagination)
            {
                tenants = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            }
            else
            {
                tenants = await query.OrderBy(input.Sorting).ToListAsync();
            }
            return new PagedResultDto<TenantSummaryDto>(tenantCount, tenants);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_GetList)]
        public async Task<PagedResultDto<GetSubscriptionDetailOutput>> GetListSubscription(GetSubscriptionDetailInput input)
        {
            var qeries = _subScriptionRepository.GetAll().AsNoTracking().WhereIf(input.TenantIds != null && input.TenantIds.Count > 0, s => input.TenantIds.Contains(s.TenantId))
                               .Select(s => new GetSubscriptionDetailOutput
                               {
                                   Discount = s.Discount,
                                   Duration = s.Duration,
                                   ShowWarning = s.ShowWarning,
                                   DurationType = s.DurationType,
                                   EditionName = s.Edition.Name,
                                   Endate = s.Endate,
                                   InvoiceDate = s.InvoiceDate,
                                   IsTrail = s.IsTrail,
                                   Package = s.Package.Name,
                                   PackageId = s.PackageId,
                                   PackagePrice = s.PackagePrice,
                                   StartDate = s.StartDate,
                                   SubScriptionEndDate = s.SubScriptionEndDate,
                                   SubscriptionType = s.SubscriptionType.ToString(),
                                   TenantName = s.Tenant.Name,
                                   TotalPrice = s.TotalPrice,
                                   Unlimited = s.Unlimited,
                                   UpgradeDeduction = s.UpgradeDeduction

                               });
            var entities = await qeries.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            var count = await qeries.CountAsync();
            return new PagedResultDto<GetSubscriptionDetailOutput>(count, entities);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_EnableDebugMode)]

        public async Task EnableDebugMode(GetDebugInput input)
        {
            var tenant = await _tenantRepository.GetAll().Where(s => s.Id == input.Id).FirstOrDefaultAsync();
            tenant.SetDebug(true);
            await _tenantRepository.UpdateAsync(tenant);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_DisableDebugMode)]
        public async Task DisableDebugMode(GetDebugInput input)
        {
            var tenant = await _tenantRepository.GetAll().Where(s => s.Id == input.Id).FirstOrDefaultAsync();
            tenant.SetDebug(false);
            await _tenantRepository.UpdateAsync(tenant);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_Edit)]
        public async Task Disable(EntityDto input)
        {
            var tenant = await _tenantRepository.GetAll().Where(s => s.Id == input.Id).FirstOrDefaultAsync();
            tenant.SetDisable();
            await _tenantRepository.UpdateAsync(tenant);
        }
        [AbpAuthorize(AppPermissions.Pages_Tenants_Edit)]
        public async Task Enable(EntityDto input)
        {
            var tenant = await _tenantRepository.GetAll().Where(s => s.Id == input.Id).FirstOrDefaultAsync();
            tenant.SetEnable();
            await _tenantRepository.UpdateAsync(tenant);
        }
    }
}