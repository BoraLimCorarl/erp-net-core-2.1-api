using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Abp;
using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.IdentityFramework;
using Abp.MultiTenancy;
using CorarlERP.Authorization.Roles;
using CorarlERP.Authorization.Users;
using CorarlERP.Editions;
using CorarlERP.MultiTenancy.Demo;
using Abp.Extensions;
using Abp.Notifications;
using Abp.Runtime.Security;
using Microsoft.AspNetCore.Identity;
using CorarlERP.Notifications;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using CorarlERP.MultiTenancy.Payments;
using CorarlERP.Formats;
using CorarlERP.Currencies;
using CorarlERP.AutoSequences;
using Abp.Dependency;
using Microsoft.AspNetCore.Hosting;
using CorarlERP.Configuration;
using CorarlERP.Subscriptions;
using CorarlERP.Features;
using System.Collections.Generic;
using Abp.Timing;
using CorarlERP.Promotions;
using Abp.Linq.Extensions;
using Abp.Collections.Extensions;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using CorarlERP.PackageEditions;
using CorarlERP.SignUps;
using CorarlERP.PropertyFormulas;
using CorarlERP.PropertyFormulas.Dto;
using CorarlERP.ItemTypes;

namespace CorarlERP.MultiTenancy
{
    /// <summary>
    /// Tenant manager.
    /// </summary>
    public class TenantManager : AbpTenantManager<Tenant, User>, ITenantManager
    {
        private readonly IRepository<Format, long> _formatRepository;
        private readonly IRepository<Currency, long> _currencyRepository;

        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;
        public IAbpSession AbpSession { get; set; }
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly RoleManager _roleManager;
        private readonly UserManager _userManager;
        private readonly IUserEmailer _userEmailer;
        private readonly TenantDemoDataBuilder _demoDataBuilder;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly IAppNotifier _appNotifier;
        private readonly IAbpZeroDbMigrator _abpZeroDbMigrator;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRepository<SubscriptionPayment, long> _subscriptionPaymentRepository;
        private readonly IRepository<SubscribableEdition> _subscribableEditionRepository;
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IRepository<Subscription, Guid> _subScriptionRepository;
        private readonly ICorarlRepository<SubscriptionPromotion, Guid> _subScriptionPromotionRepository;
        private readonly ICorarlRepository<SubscriptionCampaignPromotion, Guid> _subscriptionCampaignPromotionRepository;
        private readonly ICorarlRepository<Promotion, Guid> _promotionRepository;
        private readonly EditionManager _editionManager;
        private readonly IRepository<PackageEdition, Guid> _packageEditionRepository;
        private readonly IRepository<SignUp, Guid> _signupRepository;
        private readonly ICorarlRepository<ItemCodeFormulaCustom, long> _itemCodeFormulaCustomRepository;
        private readonly ICorarlRepository<ItemCodeFormula, long> _itemCodeFormulaRepository;
        private readonly ICorarlRepository<ItemCodeFormulaItemType, long> _itemCodeFormulaItemTypeRepository;
        private readonly ICorarlRepository<ItemType, long> _itemTypeRepository;
        public TenantManager(
            IRepository<SignUp, Guid> signupRepository,
            IRepository<PackageEdition, Guid> packageEditionRepository,
            IRepository<Tenant> tenantRepository,
            IRepository<Format, long> formatRepository,
            IRepository<Currency, long> currencyRepository,
            IRepository<AutoSequence, Guid> autoSequenceRepository,
            IAutoSequenceManager autoSequenceManager,
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository,
            EditionManager editionManager,
            IUnitOfWorkManager unitOfWorkManager,
            RoleManager roleManager,
            IUserEmailer userEmailer,
            TenantDemoDataBuilder demoDataBuilder,
            UserManager userManager,
            INotificationSubscriptionManager notificationSubscriptionManager,
            IAppNotifier appNotifier,
            IAbpZeroFeatureValueStore featureValueStore,
            IAbpZeroDbMigrator abpZeroDbMigrator,
            IPasswordHasher<User> passwordHasher,
            IRepository<SubscriptionPayment, long> subscriptionPaymentRepository,
            IRepository<SubscribableEdition> subscribableEditionRepository,
            ICorarlRepository<SubscriptionPromotion, Guid> subScriptionPromotionRepository,
            ICorarlRepository<SubscriptionCampaignPromotion, Guid> subscriptionCampaignPromotionRepository,
            ICorarlRepository<Promotion, Guid> promotionRepository,
            ICorarlRepository<ItemCodeFormulaCustom, long> itemCodeFormulaCustomRepository,
            ICorarlRepository<ItemCodeFormula, long> itemCodeFormulaRepository,
            ICorarlRepository<ItemCodeFormulaItemType, long> itemCodeFormulaItemTypeRepository,
            ICorarlRepository<ItemType, long> itemTypeRepository,
        IRepository<Subscription, Guid> subScriptionRepository) : base(
                tenantRepository,
                tenantFeatureRepository,
                editionManager,
                featureValueStore
            )
        {
            AbpSession = NullAbpSession.Instance;
            _unitOfWorkManager = unitOfWorkManager;
            _roleManager = roleManager;
            _userEmailer = userEmailer;
            _demoDataBuilder = demoDataBuilder;
            _userManager = userManager;
            _notificationSubscriptionManager = notificationSubscriptionManager;
            _appNotifier = appNotifier;
            _abpZeroDbMigrator = abpZeroDbMigrator;
            _passwordHasher = passwordHasher;
            _subscriptionPaymentRepository = subscriptionPaymentRepository;
            _subscribableEditionRepository = subscribableEditionRepository;
            _tenantRepository = tenantRepository;
            _formatRepository = formatRepository;
            _currencyRepository = currencyRepository;
            _autoSequenceManager = autoSequenceManager;
            _autoSequenceRepository = autoSequenceRepository;
            _subScriptionRepository = subScriptionRepository;
            _editionManager = editionManager;
            _subScriptionPromotionRepository = subScriptionPromotionRepository;
            _subscriptionCampaignPromotionRepository = subscriptionCampaignPromotionRepository;
            _promotionRepository = promotionRepository;
            _packageEditionRepository = packageEditionRepository;
            _signupRepository = signupRepository;
            _itemCodeFormulaCustomRepository = itemCodeFormulaCustomRepository;
            _itemCodeFormulaRepository = itemCodeFormulaRepository;
            _itemCodeFormulaItemTypeRepository = itemCodeFormulaItemTypeRepository;
            _itemTypeRepository = itemTypeRepository;
        }

        public async Task<int> CreateWithAdminUserAsync(
            string tenancyName,
            string name,
            string adminPassword,
            string adminEmailAddress,
            string connectionString,
            bool isActive,
            int? editionId,
            bool shouldChangePasswordOnNextLogin,
            bool sendActivationEmail,
            DateTime? subscriptionEndDate,
            bool isInTrialPeriod,
            string emailActivationLink,
            bool useDefaultAccount,
            bool autoItemCode,
            string prifix,
            string itemCode,
            GetSubscriptionInput subscriptionInput,
            bool UseBatchNo,
            bool defaultInventoryReportTemplate,
            string userName,
            List<SubscriptionPromotionInput> subscriptionPromotions,
            Guid? signupId)
        {
            int newTenantId;
            long newAdminId;

            await CheckEditionAsync(editionId, isInTrialPeriod);

            //using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            //{
            //Create tenant
            var tenant = new Tenant(tenancyName, name)
            {
                IsActive = isActive,
                EditionId = editionId,
                SubscriptionEndDateUtc = subscriptionEndDate?.ToUniversalTime(),
                IsInTrialPeriod = isInTrialPeriod,
                ConnectionString = connectionString.IsNullOrWhiteSpace() ? null : SimpleStringCipher.Instance.Encrypt(connectionString),

            };
            // default value 
            var format = _formatRepository.GetAll().AsNoTracking().ToList();

            var dateFormat = format.Where(t => t.Key == "Date" && t.Name == "dd-MM-yyyy").Select(t => t.Id).FirstOrDefault();
            var numberFormat = format.Where(t => t.Key == "Number" && t.Name == "123,456.00").Select(t => t.Id).FirstOrDefault();
            var currencyFormat = _currencyRepository.GetAll().AsNoTracking()
                                .Where(t => t.Code == "USD").Select(t => t.Id).FirstOrDefault();

            tenant.SetDefaultValueTenant(dateFormat, numberFormat, currencyFormat, "KH");
            tenant.SetUseDefaultAccount(useDefaultAccount);

            // auto item code              
            tenant.SetAutoItemCode(autoItemCode ? ItemCodeSetting.Auto : ItemCodeSetting.Custom);
       
            tenant.setDefaultInventoryReport(defaultInventoryReportTemplate);
            await CreateAsync(tenant);

            await _unitOfWorkManager.Current.SaveChangesAsync();

            if (subscriptionInput.EditionId.HasValue &&
                subscriptionInput.PackageId.HasValue && subscriptionInput.PackageId != Guid.Empty &&
                subscriptionInput.DurationType.HasValue &&
                !subscriptionInput.Unlimited &&
                !subscriptionInput.IsTrail)
            {
                //var edition = (SubscribableEdition)await _editionManager.FindByIdAsync(subscriptionInput.EditionId.Value);
                //if (edition == null) throw new UserFriendlyException(L("Edition") + " " + L("IsNotValid"));
                var packageEdition = await _packageEditionRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.PackageId == subscriptionInput.PackageId && s.EditionId == subscriptionInput.EditionId);
                if (packageEdition == null) throw new UserFriendlyException(L("Edition") + " " + L("IsNotValid"));


                subscriptionInput.SubscriptionType = SubscriptionType.Subscribe;
                subscriptionInput.PackagePrice = packageEdition.AnnualPrice;

                subscriptionInput.UpgradeDeduction = 0;

                var packagePrice = packageEdition.AnnualPrice;

                List<Promotion> discounts = null;
                if (!subscriptionPromotions.IsNullOrEmpty())
                {
                    discounts = await _promotionRepository.GetAll().AsNoTracking()
                                     .Where(s => s.PromotionType == PromotionType.Discount)
                                     .Where(s => subscriptionPromotions.Any(r => r.PromotionId == s.Id))
                                     .ToListAsync();
                }

                subscriptionInput.Discount = discounts == null ? 0 : Math.Round(packagePrice * discounts.Sum(t => t.DiscountRate) / 100, 2);
                subscriptionInput.TotalPrice = packagePrice - subscriptionInput.Discount;
            }

            var createSubscription = Subscription.Create(tenant.Id, AbpSession.UserId.Value, subscriptionInput.Duration, subscriptionInput.SubscriptionDate, subscriptionInput.StartDate, subscriptionInput.EndDate, editionId, subscriptionInput.DurationType, subscriptionInput.Unlimited, subscriptionInput.IsTrail, subscriptionInput.ShowWarning, subscriptionInput.PackagePrice, subscriptionInput.TotalPrice, subscriptionInput.SubscriptionType, subscriptionInput.InvoiceDate, subscriptionInput.Discount, subscriptionInput.UpgradeDeduction, subscriptionInput.PackageId);
            await _subScriptionRepository.InsertAsync(createSubscription);
            await _unitOfWorkManager.Current.SaveChangesAsync();
            if (!subscriptionPromotions.IsNullOrEmpty())
            {
                var addSubscriptionPromotions = new List<SubscriptionPromotion>();
                var addSubscriptionCampaignPromotions = new List<SubscriptionCampaignPromotion>();

                foreach (var t in subscriptionPromotions)
                {
                    var promotion = SubscriptionPromotion.Create(AbpSession.UserId.Value, createSubscription.Id, t.CampaignId, t.PromotionId, t.IsRenewable, t.IsEligibleWithOther, t.IsSpecificPackage);

                    if (t.IsSpecificPackage && !t.CampaignEditionPromotions.IsNullOrEmpty())
                    {
                        var campaingPromotions = t.CampaignEditionPromotions.Select(s => SubscriptionCampaignPromotion.Create(AbpSession.UserId.Value, promotion.Id, s.CampaignId, s.EditionId, s.SortOrder, s.PromotionId)).ToList();
                        addSubscriptionCampaignPromotions.AddRange(campaingPromotions);
                    }
                    addSubscriptionPromotions.Add(promotion);
                }

                await _subScriptionPromotionRepository.BulkInsertAsync(addSubscriptionPromotions);
                if (addSubscriptionCampaignPromotions.Any()) await _subscriptionCampaignPromotionRepository.BulkInsertAsync(addSubscriptionCampaignPromotions);
            }

            tenant.SetUseBatchNo(UseBatchNo);
            tenant.SetSubScription(createSubscription.Id, tenant.EditionId);
            //Create tenant database
            _abpZeroDbMigrator.CreateOrMigrateForTenant(tenant);

            //We are working entities of new tenant, so changing tenant filter
            using (_unitOfWorkManager.Current.SetTenantId(tenant.Id))
            {
#if role
                //Create static roles for new tenant
                CheckErrors(await _roleManager.CreateStaticRoles(tenant.Id));
                    await _unitOfWorkManager.Current.SaveChangesAsync(); //To get static role ids

                    //grant all permissions to admin role
                    var adminRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
                    await _roleManager.GrantAllPermissionsAsync(adminRole);


                #region Create Static Roles
                    var editionName = await EditionManager.Editions.Where(t => t.Id == tenant.EditionId).Select(t => t.Name).FirstOrDefaultAsync();
                    var staticPermissions = new string[] { };
                    if (editionName == "Simple Accounting")
                    {
                        staticPermissions = new string[] {  StaticRoleNames.Tenants.Admin,
                                                            StaticRoleNames.Tenants.AccountingManager,
                                                            StaticRoleNames.Tenants.APAccountant,
                                                            StaticRoleNames.Tenants.ARAccountant,
                                                            StaticRoleNames.Tenants.PurchaseManager,
                                                            StaticRoleNames.Tenants.SaleManager,

                                                            };
                    }
                    if (editionName == "Advance Accounting")
                    {
                        staticPermissions = new string[] {  StaticRoleNames.Tenants.Admin,
                                                           StaticRoleNames.Tenants.AccountingManager,
                                                            StaticRoleNames.Tenants.APAccountant,
                                                            StaticRoleNames.Tenants.ARAccountant,
                                                            StaticRoleNames.Tenants.SaleManager,
                                                            StaticRoleNames.Tenants.PurchaseManager,
                                                            StaticRoleNames.Tenants.SaleManager,
                                                            StaticRoleNames.Tenants.WarehouseManager,
                                                            StaticRoleNames.Tenants.StockController
                                                            };
                    }

                    //grant permissions to static roles other than admin
                    foreach (var p in staticPermissions)
                    {
                        var role = _roleManager.Roles.FirstOrDefault(r => r.Name == p);
                        if (role != null)
                        {
                            role.IsDefault = p == StaticRoleNames.Tenants.AccountingManager;
                            await _roleManager.GrantPermissionsToTenantRoleAsync(role);
                            CheckErrors(await _roleManager.UpdateAsync(role));
                        }
                        else
                        {

                            var item = new Role(tenant.Id, p, p);
                            item.IsStatic = true;
                            CheckErrors(await _roleManager.CreateAsync(item));
                            await _roleManager.GrantPermissionsToTenantRoleAsync(item);
                        }

                    }
                #endregion


                    //User role should be default
                    var userRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.AccountingManager);
                    userRole.IsDefault = true;
                    CheckErrors(await _roleManager.UpdateAsync(userRole));


#endif

                var featureValues = (await _editionManager.GetFeatureValuesAsync(editionId.Value)).Where(t => t.Value.Contains("true")).ToList();
                var roles = new List<Role>();
                roles.Add(new Role(tenant.Id, StaticRoleNames.Tenants.Admin, StaticRoleNames.Tenants.Admin));
                if (featureValues.Select(t => t.Name).Contains(AppFeatures.AccountingFeature))
                {

                    roles.Add(new Role(tenant.Id, StaticRoleNames.Tenants.AccountingManager, StaticRoleNames.Tenants.AccountingManager));
                    roles.Add(new Role(tenant.Id, StaticRoleNames.Tenants.APAccountant, StaticRoleNames.Tenants.APAccountant));
                    roles.Add(new Role(tenant.Id, StaticRoleNames.Tenants.ARAccountant, StaticRoleNames.Tenants.ARAccountant));
                }

                if (featureValues.Select(t => t.Name).Contains(AppFeatures.VendorsFeaturePurchases))
                {
                    roles.Add(new Role(tenant.Id, StaticRoleNames.Tenants.PurchaseManager, StaticRoleNames.Tenants.PurchaseManager));
                }

                if (featureValues.Select(t => t.Name).Contains(AppFeatures.CustomersFeature))
                {
                    roles.Add(new Role(tenant.Id, StaticRoleNames.Tenants.SaleManager, StaticRoleNames.Tenants.SaleManager));
                }
                if (featureValues.Select(t => t.Name).Contains(AppFeatures.InventoryFeature))
                {
                    roles.Add(new Role(tenant.Id, StaticRoleNames.Tenants.WarehouseManager, StaticRoleNames.Tenants.WarehouseManager));
                    roles.Add(new Role(tenant.Id, StaticRoleNames.Tenants.StockController, StaticRoleNames.Tenants.StockController));
                }

                foreach (var role in roles)
                {
                    if (role != null)
                    {
                        role.IsStatic = role.Name == StaticRoleNames.Tenants.APAccountant || role.Name == StaticRoleNames.Tenants.ARAccountant || role.Name == StaticRoleNames.Tenants.StockController ? false : true;
                        CheckErrors(await _roleManager.CreateAsync(role));
                        role.IsDefault = role.Name == StaticRoleNames.Tenants.Admin;
                        await _roleManager.GrantPermissionsToTenantRoleAsync(role);
                    }
                }
                //await _unitOfWorkManager.Current.SaveChangesAsync();
                //Create admin user for the tenant
                var adminUser = User.CreateTenantAdminUser(tenant.Id, adminEmailAddress, userName);
                if (signupId.HasValue && signupId != Guid.Empty)
                {
                    var signup = await _signupRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == signupId);
                    if (signup != null)
                    {
                        adminUser = User.CreateTenantAdminUser(tenant.Id, adminEmailAddress, userName, signup.FirstName, signup.LastName);
                    }
                }

                adminUser.ShouldChangePasswordOnNextLogin = shouldChangePasswordOnNextLogin;
                adminUser.IsActive = true;

                var isRamdom = false;
                if (adminPassword.IsNullOrEmpty())
                {
                    isRamdom = true;
                    adminPassword = User.CreateRandomPassword();
                }
                else
                {
                    await _userManager.InitializeOptionsAsync(AbpSession.TenantId);
                    foreach (var validator in _userManager.PasswordValidators)
                    {
                        CheckErrors(await validator.ValidateAsync(_userManager, adminUser, adminPassword));
                    }

                }

                adminUser.Password = _passwordHasher.HashPassword(adminUser, adminPassword);

                CheckErrors(await _userManager.CreateAsync(adminUser));
                await _unitOfWorkManager.Current.SaveChangesAsync(); //To get admin user's id

                //Assign admin user to admin role!
                //grant all permissions to admin role
                var adminRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
                await _roleManager.GrantAllPermissionsAsync(adminRole);
                CheckErrors(await _userManager.AddToRoleAsync(adminUser, adminRole.Name));

                //Notifications
                await _appNotifier.WelcomeToTheApplicationAsync(adminUser);

                //Send activation email
                if (sendActivationEmail)
                {
                    adminUser.SetNewEmailConfirmationCode();

                    await _userEmailer.SendEmailActivationLinkAsync(adminUser, emailActivationLink, isRamdom ? adminPassword : null);
                }

                await _unitOfWorkManager.Current.SaveChangesAsync();

                await _demoDataBuilder.BuildForAsync(tenant);

                newTenantId = tenant.Id;
                newAdminId = adminUser.Id;
            }
            //create auto seque               
            var userId = AbpSession.GetUserId();

            var lst = _autoSequenceManager.GetDocumentTypes().Select(auto =>
                                              AutoSequence.Create(
                                                  newTenantId, userId, auto.Type, auto.AutoSequenceTitle,
                                                  auto.SybolFormat, auto.NumberFormat, auto.CustomFormat,
                                                  auto.YearFormat, auto.LastAutoSequence)
                                            );

            foreach (var @entity in lst)
            {
                CheckErrors(await _autoSequenceManager.CreateAsync(@entity));

            }
            //    await uow.CompleteAsync();
            //}

            //Used a second UOW since UOW above sets some permissions and _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync needs these permissions to be saved.
            //using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            //{
            using (_unitOfWorkManager.Current.SetTenantId(newTenantId))
            {
                await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(new UserIdentifier(newTenantId, newAdminId));
                // await _unitOfWorkManager.Current.SaveChangesAsync();
                // await uow.CompleteAsync();
            }
            //}

            return newTenantId;
        }

        protected string AppName
        {
            get
            {
                var hostingEnvironment = IocManager.Instance.Resolve<IHostingEnvironment>();
                var appConfiguration = hostingEnvironment.GetAppConfiguration();
                return appConfiguration["App:Name"];
            }
        }

        public async Task CheckEditionAsync(int? editionId, bool isInTrialPeriod)
        {
            if (!editionId.HasValue || !isInTrialPeriod)
            {
                return;
            }

            var edition = await _subscribableEditionRepository.GetAsync(editionId.Value);
            if (!edition.IsFree)
            {
                return;
            }

            var error = LocalizationManager.GetSource(CorarlERPConsts.LocalizationSourceName).GetString("FreeEditionsCannotHaveTrialVersions");
            throw new UserFriendlyException(error);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        public async Task<SubscriptionPayment> GetLastPaymentAsync(Expression<Func<SubscriptionPayment, bool>> predicate)
        {
            return await _subscriptionPaymentRepository.GetAll().LastOrDefaultAsync(predicate);
        }

        public decimal GetUpgradePrice(SubscribableEdition currentEdition, SubscribableEdition targetEdition, int remainingDaysCount)
        {
            decimal additionalPrice;

            // If remainingDaysCount is longer than annual, then calculate price with using
            // both annual and monthly prices
            if (remainingDaysCount > (int)PaymentPeriodType.Annual)
            {
                var remainingsYearsCount = remainingDaysCount / (int)PaymentPeriodType.Annual;
                remainingDaysCount = remainingDaysCount % (int)PaymentPeriodType.Annual;

                additionalPrice = GetMonthlyCalculatedPrice(currentEdition, targetEdition, remainingDaysCount);
                additionalPrice += GetAnnualCalculatedPrice(currentEdition, targetEdition, remainingsYearsCount); // add yearly price to montly calculated price
            }
            else
            {
                additionalPrice = GetMonthlyCalculatedPrice(currentEdition, targetEdition, remainingDaysCount);
            }

            return additionalPrice;
        }

        public async Task<Tenant> UpdateTenantAsync(int tenantId, bool isActive, bool isInTrialPeriod, PaymentPeriodType? paymentPeriodType, int editionId, EditionPaymentType editionPaymentType)
        {
            var tenant = await FindByIdAsync(tenantId);

            tenant.IsActive = isActive;
            tenant.IsInTrialPeriod = isInTrialPeriod;
            tenant.EditionId = editionId;

            if (paymentPeriodType.HasValue)
            {
                tenant.UpdateSubscriptionDateForPayment(paymentPeriodType.Value, editionPaymentType);
            }

            return tenant;
        }

        public async Task<EndSubscriptionResult> EndSubscriptionAsync(Tenant tenant, SubscribableEdition edition, DateTime nowUtc)
        {
            if (tenant.EditionId == null || tenant.HasUnlimitedTimeSubscription())
            {
                throw new Exception($"Can not end tenant {tenant.TenancyName} subscription for {edition.DisplayName} tenant has unlimited time subscription!");
            }

            Debug.Assert(tenant.SubscriptionEndDateUtc != null, "tenant.SubscriptionEndDateUtc != null");

            var subscriptionEndDateUtc = tenant.SubscriptionEndDateUtc.Value;
            if (!tenant.IsInTrialPeriod)
            {
                subscriptionEndDateUtc = tenant.SubscriptionEndDateUtc.Value.AddDays(edition.WaitingDayAfterExpire ?? 0);
            }

            if (subscriptionEndDateUtc >= nowUtc)
            {
                throw new Exception($"Can not end tenant {tenant.TenancyName} subscription for {edition.DisplayName} since subscription has not expired yet!");
            }

            if (!tenant.IsInTrialPeriod && edition.ExpiringEditionId.HasValue)
            {
                tenant.EditionId = edition.ExpiringEditionId.Value;
                tenant.SubscriptionEndDateUtc = null;

                await UpdateAsync(tenant);

                return EndSubscriptionResult.AssignedToAnotherEdition;
            }

            tenant.IsActive = false;
            tenant.IsInTrialPeriod = false;

            await UpdateAsync(tenant);

            return EndSubscriptionResult.TenantSetInActive;
        }

        private static decimal GetMonthlyCalculatedPrice(SubscribableEdition currentEdition, SubscribableEdition upgradeEdition, int remainingDaysCount)
        {
            decimal currentUnusedPrice = 0;
            decimal upgradeUnusedPrice = 0;

            if (currentEdition.MonthlyPrice.HasValue)
            {
                currentUnusedPrice = (currentEdition.MonthlyPrice.Value / (int)PaymentPeriodType.Monthly) * remainingDaysCount;
            }

            if (upgradeEdition.MonthlyPrice.HasValue)
            {
                upgradeUnusedPrice = (upgradeEdition.MonthlyPrice.Value / (int)PaymentPeriodType.Monthly) * remainingDaysCount;
            }

            var additionalPrice = upgradeUnusedPrice - currentUnusedPrice;
            return additionalPrice;
        }

        private static decimal GetAnnualCalculatedPrice(SubscribableEdition currentEdition, SubscribableEdition upgradeEdition, int remainingYearsCount)
        {
            var currentUnusedPrice = (currentEdition.AnnualPrice ?? 0) * remainingYearsCount;
            var upgradeUnusedPrice = (upgradeEdition.AnnualPrice ?? 0) * remainingYearsCount;

            var additionalPrice = upgradeUnusedPrice - currentUnusedPrice;
            return additionalPrice;
        }


        async Task<IdentityResult> ITenantManager.UpdateAsync(Tenant entity)
        {
            await _tenantRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<Tenant> GetAsync(int id, bool tracking = false)
        {
            var @query = tracking ? _tenantRepository.GetAll()
                .Include(u => u.Currency)
                .Include(u => u.FormatDate)
                .Include(u => u.FormatNumber)
                .Include(u => u.TransitAccount)
                .Include(u => u.SaleAllowanceAccount)
                .Include(u => u.BillPaymentAccount)
                .Include(u => u.Class)
                .Include(u => u.Location)
                .Include(u => u.AccountCycle)
                .Include(u => u.CustomerAccount)
                .Include(u => u.VendorAccount)
                .Include(u => u.Tax)
                .Include(u => u.ItemIssueAdjustment)
                .Include(u => u.ItemIssuePhysicalCount)
                .Include(u => u.ItemIssueTransfer)
                .Include(u => u.ItemIssueVendorCredit)
                .Include(u => u.ItemIssueOther)
                .Include(u => u.ItemRecieptAdjustment)
                .Include(u => u.ItemRecieptCustomerCredit)
                .Include(u => u.ItemRecieptOther)
                .Include(u => u.ItemRecieptTransfer)
                .Include(u => u.ItemRecieptPhysicalCount)
                .Include(u => u.BankTransferAccount)
                .Include(u => u.RawProductionAccount)
                .Include(u => u.FinishProductionAccount)
                .Include(u => u.RoundDigitAccount)
                .Include(u => u.Property)
                .Include(u => u.POSCurrency)
                .Include(u => u.ExchangeLossAndGain)
                .Include(s => s.InventoryAccount)
                .Include(s => s.COGSAccount)
                .Include(s => s.RevenueAccount)
                .Include(s => s.ExpenseAccount)
                    :
                _tenantRepository.GetAll()
                .Include(u => u.Currency)
                .Include(u => u.FormatDate)
                .Include(u => u.FormatNumber)
                .Include(u => u.TransitAccount)
                .Include(u => u.SaleAllowanceAccount)
                .Include(u => u.BillPaymentAccount)
                .Include(u => u.Class)
                .Include(u => u.Location)
                .Include(u => u.AccountCycle)
                .Include(u => u.ItemIssueAdjustment)
                .Include(u => u.ItemIssuePhysicalCount)
                .Include(u => u.ItemIssueTransfer)
                .Include(u => u.ItemIssueVendorCredit)
                .Include(u => u.ItemIssueOther)
                .Include(u => u.ItemRecieptAdjustment)
                .Include(u => u.ItemRecieptCustomerCredit)
                .Include(u => u.ItemRecieptOther)
                .Include(u => u.ItemRecieptTransfer)
                .Include(u => u.ItemRecieptPhysicalCount)
                .Include(u => u.BankTransferAccount)
                .Include(u => u.RawProductionAccount)
                .Include(u => u.FinishProductionAccount)
                .Include(u => u.RoundDigitAccount)
                .Include(u => u.CustomerAccount)
                .Include(u => u.VendorAccount)
                .Include(u => u.Property)
                .Include(u => u.POSCurrency)
                .Include(u => u.ExchangeLossAndGain)
                .Include(s => s.InventoryAccount)
                .Include(s => s.COGSAccount)
                .Include(s => s.RevenueAccount)
                .Include(s => s.ExpenseAccount)
                .Include(s => s.Tax)
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task AutoCreateItemCode(CreateOrUpdateDefaultAutoItemCodeInput input)
        {
            var itemCodeFormulaItemTypes = new List<ItemCodeFormulaItemType>();
            var itemCodeFormulaCustoms = new List<ItemCodeFormulaCustom>();
            var entities = ItemCodeFormula.Create(input.TenantId, input.UserId, false, true,false);
            await _itemCodeFormulaRepository.InsertAsync(entities);
            await _unitOfWorkManager.Current.SaveChangesAsync();
            var itemTypes = await _itemTypeRepository.GetAll().AsNoTracking().ToListAsync();
            foreach (var i in itemTypes)
            {
                var itemType = ItemCodeFormulaItemType.Create(input.TenantId, input.UserId, entities.Id, i.Id);
                itemCodeFormulaItemTypes.Add(itemType);

            }
            var custom = ItemCodeFormulaCustom.Create(input.TenantId, input.UserId, input.Prefix, input.ItemCode, entities.Id);
            itemCodeFormulaCustoms.Add(custom);
            await _itemCodeFormulaItemTypeRepository.BulkInsertAsync(itemCodeFormulaItemTypes);
            await _itemCodeFormulaCustomRepository.BulkInsertAsync(itemCodeFormulaCustoms);
        }
    }
}
