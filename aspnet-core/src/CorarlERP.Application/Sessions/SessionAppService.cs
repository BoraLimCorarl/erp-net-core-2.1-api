using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using CorarlERP.Editions;
using CorarlERP.Sessions.Dto;
using CorarlERP.Features;
using CorarlERP.AccountCycles;
using CorarlERP.Currencies;
using CorarlERP.MultiTenancy.Dto;
using Abp.Domain.Repositories;
using CorarlERP.AutoSequences;
using CorarlERP.MultiCurrencies;
using CorarlERP.Authorization.Users;
using CorarlERP.Locations.Dto;
using CorarlERP.UserGroups;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Abp.Dependency;
using CorarlERP.Configuration;
using CorarlERP.Settings;
using CorarlERP.Settings.Dtos;
using Abp.Application.Features;
using CorarlERP.Subscriptions;
using CorarlERP.MultiTenancy;
using Abp.Authorization;
using CorarlERP.Authorization.ApiClients;
using Abp;
using Abp.Runtime.Caching;
using Microsoft.AspNetCore.Http;
using CorarlERP.PackageEditions;
using CorarlERP.Lots;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Sessions
{
    public class SessionAppService : CorarlERPAppServiceBase, ISessionAppService
    {
        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly IRepository<MultiCurrency, long> _multiCurrencyRepository;
        private readonly IRepository<UserGroupMember, Guid> _userGroupMemberRepository;
        private readonly IConfigurationRoot _appConfiguration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IRepository<BillInvoiceSetting, long> _settingRepository;
        private readonly IRepository<Subscription, Guid> _subscriptionRepository;
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IApiClientManager _apiClientManager;
        private readonly UserManager _userManager;
        private readonly ICacheManager _cacheManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public readonly ICorarlRepository<Package, Guid> _packageRepository;
        public readonly ICorarlRepository<Lot, long> _lotRepository;
        public SessionAppService(
            IRepository<AutoSequence, Guid> autoSequenceRepository,
            IAutoSequenceManager autoSequenceManager,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            IRepository<MultiCurrency, long> multiCurrencyRepository,
            IRepository<Subscription, Guid> subscriptionRepository,
            IRepository<Tenant, int> tenantRepository,
            UserManager userManager,
            ICacheManager cacheManager,
            IApiClientManager apiClientManager,
            IHttpContextAccessor httpContextAccessor,
            ICorarlRepository<Package, Guid> packageRepository,
            ICorarlRepository<Lot, long> lotRepository)
        {
            _autoSequenceRepository = autoSequenceRepository;
            _userGroupMemberRepository = userGroupMemberRepository;
            _autoSequenceManager = autoSequenceManager;
            _multiCurrencyRepository = multiCurrencyRepository;
            _subscriptionRepository = subscriptionRepository;
            _tenantRepository = tenantRepository;
            _hostingEnvironment = IocManager.Instance.Resolve<IHostingEnvironment>();
            _appConfiguration = _hostingEnvironment.GetAppConfiguration();
            _settingRepository = IocManager.Instance.Resolve<IRepository<BillInvoiceSetting, long>>();
            _apiClientManager = apiClientManager;
            _userManager = userManager;
            _lotRepository = lotRepository;
            _packageRepository = packageRepository;
            _cacheManager = cacheManager;
            _httpContextAccessor = httpContextAccessor;

        }


        private async Task<ApplicationTypes> ValidateClientSecret(AuthenticationBaseInput model)
        {
            return await _apiClientManager.ValidateClientSecretAsync(model.ClientSecret, model.ClientId);
        }


        [DisableAuditing]
        public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations(AuthenticationBaseInput input)
        {
            var applicationType = await ValidateClientSecret(input);
            var output = new GetCurrentLoginInformationsOutput
            {
                Application = new ApplicationInfoDto
                {
                    Version = AppVersionHelper.Version,
                    ReleaseDate = AppVersionHelper.ReleaseDate,
                    Features = new Dictionary<string, bool>(),
                    Name = _appConfiguration["App:Name"],
                    Logo = _appConfiguration["App:Logo"],
                    Icon = _appConfiguration["App:Icon"],
                    EnableSetup = Convert.ToBoolean(_appConfiguration["App:EnableSignUp"]),
                    PrivacyPolicy = _appConfiguration["App:PrivacyPolicy"],
                    SetupImage = _appConfiguration["App:SignUpImage"],
                    TermAgreement = _appConfiguration["App:TermAgreement"],
                    SignUpBackroundImageColor = _appConfiguration["App:SignUpBackroundImageColor"],
                    FacebookURL = _appConfiguration["App:Support:FacebookURL"],
                    TelegramURL = _appConfiguration["App:Support:TelegramURL"],
                    YoutubeURL = _appConfiguration["App:Support:YoutubeURL"],
                    TutorialURL = _appConfiguration["App:Support:TutorialURL"],
                    TawkEnable = Convert.ToBoolean(_appConfiguration["Tawk:Enable"])
                }
            };
            var @queryLots = new List<LotSessionOutput>();

            if (!AbpSession.UserId.HasValue)
            {

                if (AbpSession.TenantId.HasValue)
                {
                    var tenantId = AbpSession.TenantId.Value;
                    var tenant = await _tenantRepository.GetAll().Where(t => t.Id == tenantId).AsNoTracking().FirstOrDefaultAsync();
                    output.Tenant = new TenantLoginInfoDto { Id = tenant.Id, Name = tenant.Name, TenancyName = tenant.TenancyName };
                }
                return output;
            }



            if (AbpSession.TenantId.HasValue)
            {
                var id = AbpSession.TenantId.Value;

                var tenant = await TenantManager
                        .Tenants
                        .AsNoTracking()
                        .Include(u => u.AccountCycle)
                        .Include(u => u.FormatDate)
                        .Include(u => u.FormatNumber)
                        .Include(u => u.Currency)
                        .Include(u => u.POSCurrency)
                        .Include(u => u.Edition)
                        .Include(u => u.CompanyAddress)
                        .Include(u => u.TransitAccount)
                        .Include(u => u.BillPaymentAccount)
                        .Include(u => u.SaleAllowanceAccount)
                        .Include(u => u.Class)
                        .Include(u => u.Location)
                        .Include(u => u.ItemIssueAdjustment)
                        .Include(u => u.ItemIssueTransfer)
                        .Include(u => u.ItemIssueVendorCredit)
                        .Include(u => u.ItemIssueOther)
                        .Include(u => u.ItemRecieptAdjustment)
                        .Include(u => u.ItemRecieptCustomerCredit)
                        .Include(u => u.ItemRecieptOther)
                        .Include(u => u.ItemRecieptTransfer)
                        .Include(u => u.FinishProductionAccount)
                        .Include(u => u.RawProductionAccount)
                        .Include(u => u.RoundDigitAccount)
                        .Include(u => u.VendorAccount)
                        .Include(u => u.CustomerAccount)
                        .Include(u => u.ExchangeLossAndGain)
                        .Include(s => s.InventoryAccount)
                        .Include(s => s.COGSAccount)
                        .Include(s => s.RevenueAccount)
                        .Include(s => s.ExpenseAccount)
                        .Include(u => u.Property)
                        .FirstAsync(t => t.Id == id);
                //logout and remove cache after tenant disable
                if (!tenant.IsActive)
                {

                    var tokenValidityKeyInClaims = _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == CorarlERPConsts.TokenValidityKey);
                    await _userManager.RemoveTokenValidityKeyAsync(_userManager.GetUser(AbpSession.ToUserIdentifier()), tokenValidityKeyInClaims.Value);
                    _cacheManager.GetCache(CorarlERPConsts.TokenValidityKey).Remove(tokenValidityKeyInClaims.Value);
                    output.Tenant = null;
                    return output;
                }

                if (tenant.AccountCycleId == null || tenant.AccountCycle == null)
                {//temp assign
                    int year = DateTime.Now.Year;
                    DateTime firstDay = new DateTime(year, 1, 1);
                    //DateTime lastDay = new DateTime(year, 12, 31);
                    tenant.AccountCycle = new AccountCycle()
                    {
                        StartDate = firstDay,
                        RoundingDigit = 2,
                        EndDate = null,
                        RoundingDigitUnitCost = 2
                    };
                }
                var subscription = await _subscriptionRepository.GetAll().AsNoTracking()
                                   .Where(t => t.Id == tenant.SubscriptionId)
                                   .Select(t => new GetSubscriptionInput
                                   {
                                       EndDate = t.Endate,
                                       Id = t.Id,
                                       Duration = t.Duration,
                                       IsTrail = t.IsTrail,
                                       EditionId = t.EditionId,
                                       Unlimited = t.Unlimited,
                                       TenantId = t.TenantId,
                                       ShowWarning = t.ShowWarning,
                                       StartDate = t.StartDate,
                                       DurationType = t.DurationType,
                                       PackageId = t.PackageId,
                                   }).FirstOrDefaultAsync();

                if (tenant.CurrencyId == null || tenant.Currency == null)
                {//temp assign
                    tenant.Currency = Currency.Create(null, "USD", "", "", "");
                }
                var hasItemReceiptFeature = await FeatureChecker.IsEnabledAsync(id, AppFeatures.VendorsFeatureItemReceipts);
                var hasItemIssueFeature = await FeatureChecker.IsEnabledAsync(id, AppFeatures.CustomersFeatureItemIssues);
                var trackBatch = await FeatureChecker.IsEnabledAsync(id, AppFeatures.ItemFeatureTrackBatchs);

                output.Tenant = ObjectMapper
                    .Map<TenantLoginInfoDto>(tenant);
                output.Tenant.AutoPostCustomerCredit = !hasItemReceiptFeature;
                output.Tenant.AutoPostBill = !hasItemReceiptFeature;
                output.Tenant.PropertyId = tenant.PropertyId;
                output.Tenant.UseBatchNo = output.Tenant.UseBatchNo && trackBatch;
                output.Tenant.Subscription = subscription;
                output.Tenant.AutoItemCode = output.Tenant.ItemCodeSetting == ItemCodeSetting.Auto ? true : false;
                output.Tenant.Subscription.IsExp = subscription.EndDate != null && subscription.EndDate.Value.Date < DateTime.Now.Date ? true : false;
                output.Tenant.PackageCode = await _packageRepository.GetAll().AsNoTracking().Where(s => s.Id == subscription.PackageId).Select(s => s.Code).FirstOrDefaultAsync();
                //Get Default user by location
                var userLocatons = await _userGroupMemberRepository.GetAll()
                                            .Include(t => t.UserGroup)
                                            .Include(t => t.UserGroup.Location)
                                            .Where(t => t.MemberId == AbpSession.UserId && t.UserGroup.LocationId != null)
                                            .AsNoTracking()
                                            .ToListAsync();
                var userGroups = new List<long?>();
                if (userLocatons != null && userLocatons.Any())
                {
                    output.Tenant.UserLocations = userLocatons.Select(t => new LocationDto
                    {
                        Id = t.UserGroup.LocationId.Value,
                        LocationName = t.UserGroup.Location.LocationName,

                    }).ToList();

                    userGroups = userLocatons.Select(s => s.UserGroup.LocationId).ToList();

                }

                var userGroupByLocation = userLocatons
                                            .Where(t => t.IsDefault == true)
                                            .FirstOrDefault();

                if (userGroupByLocation != null)
                {

                    output.Tenant.DefaultUserLocationId = userGroupByLocation.UserGroup.LocationId;
                    output.Tenant.DefaultUserLocation = ObjectMapper.Map<LocationDto>(userGroupByLocation.UserGroup.Location);
                }

                var @autoSequence = await _autoSequenceRepository.GetAll().AsNoTracking().Where(t => t.TenantId == tenant.Id).ToListAsync();

                output.Tenant.AutoSequenceDetail = autoSequence.Select(t =>
                      new AutoSequenceDto
                      {
                          CustomFormat = t.CustomFormat,
                          DefaultPrefix = t.DefaultPrefix,
                          Id = t.Id,
                          LastAutoSequeneNumber = t.LastAutoSequenceNumber,
                          NumberFormat = t.NumberFormat,
                          SymbolFormat = t.SymbolFormat,
                          YearFormatString = t.YearFormat.ToString(),
                          DocumentTypeName = t.DocumentType.ToString(),
                          DocumentTypeNumber = Convert.ToInt32(t.DocumentType),
                          RequireReference = t.RequireReference
                      }).ToList();
                output.Tenant.AutoPostVendorCredit = !hasItemIssueFeature;
                output.Tenant.AutoPostInvoice = !hasItemIssueFeature;

                var @multiCurrencys = await _multiCurrencyRepository.GetAll().AsNoTracking().ToListAsync();
                output.Tenant.IsMultiCurrency = @multiCurrencys.Any();

                var settins = await _settingRepository.GetAll().AsNoTracking().ToListAsync();

                var billSetting = settins.FirstOrDefault(s => s.SettingType == BillInvoiceSettingType.Bill);
                if (billSetting != null)
                {
                    output.Tenant.BillSetting = new BillInvoiceSettingInputOutput
                    {
                        Id = billSetting.Id,
                        SettingType = billSetting.SettingType,
                        ReferenceSameAsGoodsMovement = billSetting.ReferenceSameAsGoodsMovement,
                    };
                }


                var invoiceSetting = settins.FirstOrDefault(s => s.SettingType == BillInvoiceSettingType.Invoice);
                if (invoiceSetting != null)
                {
                    output.Tenant.InvoiceSetting = new BillInvoiceSettingInputOutput
                    {
                        Id = invoiceSetting.Id,
                        SettingType = invoiceSetting.SettingType,
                        ReferenceSameAsGoodsMovement = invoiceSetting.ReferenceSameAsGoodsMovement,
                    };
                }
                #region Default Lot                                   
                 @queryLots = await _lotRepository
                                    .GetAll()                                 
                                    .AsNoTracking()
                                    .Where(s => s.Location.Member == Member.All || userGroups.Contains(s.LocationId))
                                    .Select(l => new LotSessionOutput
                                    {
                                        Id = l.Id,
                                        LocationId = l.LocationId,                                    
                                        IsActive = l.IsActive,
                                        LotName = l.LotName,
                                        LocationName = l.Location.LocationName
                                    }).ToListAsync();
                #endregion

            }

            if (AbpSession.UserId.HasValue)
            {
                output.User = ObjectMapper.Map<UserLoginInfoDto>(await GetCurrentUserAsync());
                output.Lots = queryLots;
            }
            if (output.Tenant == null)
            {
                return output;
            }

            if (output.Tenant.Edition != null)
            {
                output.Tenant.Edition.IsHighestEdition = IsEditionHighest(output.Tenant.Edition.Id);
            }

            output.Tenant.SubscriptionDateString = GetTenantSubscriptionDateString(output);
            {
                output.Tenant.CreationTimeString = output.Tenant.CreationTime.ToString("d");

                return output;
            }
        }



        [DisableAuditing]
        [AbpAuthorize]
        public async Task<GetCurrentLoginInformationsSummaryOutput> GetCurrentLoginInformationForMobile()
        {

            var output = new GetCurrentLoginInformationsSummaryOutput();

            if (AbpSession.TenantId.HasValue)
            {
                var id = AbpSession.TenantId.Value;

                var tenant = await TenantManager
                        .Tenants.AsNoTracking()
                        .FirstAsync(t => t.Id == id);
                //logout and remove cache after tenant disable
                if (!tenant.IsActive)
                {

                    var tokenValidityKeyInClaims = _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == CorarlERPConsts.TokenValidityKey);
                    await _userManager.RemoveTokenValidityKeyAsync(_userManager.GetUser(AbpSession.ToUserIdentifier()), tokenValidityKeyInClaims.Value);
                    _cacheManager.GetCache(CorarlERPConsts.TokenValidityKey).Remove(tokenValidityKeyInClaims.Value);
                    output.Tenant = null;
                    return output;
                }
                output.Tenant = ObjectMapper
                    .Map<TenantLoginSummaryInfoDto>(tenant);
            }

            if (AbpSession.UserId.HasValue)
            {
                output.User = ObjectMapper.Map<UserLoginSummaryInfoDto>(await GetCurrentUserAsync());
            }

            if (output.Tenant == null)
            {
                return output;
            }

            return output;

        }

        private bool IsEditionHighest(int editionId)
        {
            var topEdition = GetHighestEditionOrNullByMonthlyPrice();
            if (topEdition == null)
            {
                return false;
            }

            return editionId == topEdition.Id;
        }

        private SubscribableEdition GetHighestEditionOrNullByMonthlyPrice()
        {
            var editions = TenantManager.EditionManager.Editions;
            if (editions == null || !editions.Any())
            {
                return null;
            }

            return editions.Cast<SubscribableEdition>()
                .OrderByDescending(e => e.MonthlyPrice)
                .FirstOrDefault();
        }

        private string GetTenantSubscriptionDateString(GetCurrentLoginInformationsOutput output)
        {
            return output.Tenant.SubscriptionEndDateUtc == null
                ? L("Unlimited")
                : output.Tenant.SubscriptionEndDateUtc?.ToString("d");
        }

        public async Task<UpdateUserSignInTokenOutput> UpdateUserSignInToken()
        {
            if (AbpSession.UserId <= 0)
            {
                throw new Exception(L("ThereIsNoLoggedInUser"));
            }

            var user = await UserManager.GetUserAsync(AbpSession.ToUserIdentifier());
            user.SetSignInToken();
            return new UpdateUserSignInTokenOutput
            {
                SignInToken = user.SignInToken,
                EncodedUserId = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Id.ToString())),
                EncodedTenantId = user.TenantId.HasValue
                    ? Convert.ToBase64String(Encoding.UTF8.GetBytes(user.TenantId.Value.ToString()))
                    : ""
            };
        }
    }
}