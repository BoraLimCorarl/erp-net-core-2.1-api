using System;
using System.Threading.Tasks;
using System.Web;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Extensions;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Identity;
using CorarlERP.Authorization.Accounts.Dto;
using CorarlERP.Authorization.Impersonation;
using CorarlERP.Authorization.Users;
using CorarlERP.Configuration;
using CorarlERP.Debugging;
using CorarlERP.MultiTenancy;
using CorarlERP.Security.Recaptcha;
using CorarlERP.Url;
using Abp.Dependency;
using Microsoft.AspNetCore.Hosting;
using CorarlERP.Authorization.ApiClients;

namespace CorarlERP.Authorization.Accounts
{
    public class AccountAppService : CorarlERPAppServiceBase, IAccountAppService
    {
        public IAppUrlService AppUrlService { get; set; }

        public IRecaptchaValidator RecaptchaValidator { get; set; }

        private readonly IUserEmailer _userEmailer;
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly IImpersonationManager _impersonationManager;
        private readonly IUserLinkManager _userLinkManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IWebUrlService _webUrlService;
        private readonly IApiClientManager _apiClientManager;

        public AccountAppService(
            IUserEmailer userEmailer,
            UserRegistrationManager userRegistrationManager,
            IImpersonationManager impersonationManager,
            IUserLinkManager userLinkManager,
            IPasswordHasher<User> passwordHasher,
            IWebUrlService webUrlService,
            IApiClientManager apiClientManager)
        {
            _userEmailer = userEmailer;
            _userRegistrationManager = userRegistrationManager;
            _impersonationManager = impersonationManager;
            _userLinkManager = userLinkManager;
            _passwordHasher = passwordHasher;
            _webUrlService = webUrlService;
            _apiClientManager = apiClientManager;

            AppUrlService = NullAppUrlService.Instance;
            RecaptchaValidator = NullRecaptchaValidator.Instance;
        }

        public async Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input)
        {
            var tenant = await TenantManager.FindByTenancyNameAsync(input.TenancyName);
            if (tenant == null)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
            }

            if (!tenant.IsActive)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.InActive);
            }

            return new IsTenantAvailableOutput(TenantAvailabilityState.Available, tenant.Id, _webUrlService.GetServerRootAddress(input.TenancyName));
        }

        private async Task<ApplicationTypes> ValidateClientSecret(String clientSecret, String clientId)
        {
            return await _apiClientManager.ValidateClientSecretAsync(clientSecret, clientId);
        }

        public Task<int?> ResolveTenantId(ResolveTenantIdInput input)
        {
            if (string.IsNullOrEmpty(input.c))
            {
                return Task.FromResult(AbpSession.TenantId);
            }

            var parameters = SimpleStringCipher.Instance.Decrypt(input.c);
            var query = HttpUtility.ParseQueryString(parameters);

            if (query["tenantId"] == null)
            {
                return Task.FromResult<int?>(null);
            }

            var tenantId = Convert.ToInt32(query["tenantId"]) as int?;
            return Task.FromResult(tenantId);
        }

        public async Task<RegisterOutput> Register(RegisterInput input)
        {
            if (UseCaptchaOnRegistration())
            {
                await RecaptchaValidator.ValidateAsync(input.CaptchaResponse);
            }

            var user = await _userRegistrationManager.RegisterAsync(
                input.Name,
                input.Surname,
                input.EmailAddress,
                input.UserName,
                input.Password,
                false,
                AppUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId)
            );

            var isEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);

            return new RegisterOutput
            {
                CanLogin = user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin)
            };
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

        public async Task SendPasswordResetCode(SendPasswordResetCodeInput input)
        {
            if (UseCaptchaOnLogin())
            {
                await RecaptchaValidator.ValidateAsync(input.CaptchaResponse);
            }

            var user = await GetUserByChecking(input.EmailAddress);
            user.SetNewPasswordResetCode();
            await _userEmailer.SendPasswordResetLinkAsyncNew(
                this.AppName,
                user,
                AppUrlService.CreatePasswordResetUrlFormat(AbpSession.TenantId)
                );
        }

        public async Task<ResetPasswordOutput> ResetPassword(ResetPasswordInput input)
        {
            if (UseCaptchaOnLogin())
            {
                await RecaptchaValidator.ValidateAsync(input.CaptchaResponse);
            }

            var user = await UserManager.GetUserByIdAsync(input.UserId);
            if (user == null || user.PasswordResetCode.IsNullOrEmpty() || user.PasswordResetCode != input.ResetCode)
            {
                throw new UserFriendlyException(L("InvalidPasswordResetCode"), L("InvalidPasswordResetCode_Detail"));
            }

            await UserManager.InitializeOptionsAsync(AbpSession.TenantId);
            CheckErrors(await UserManager.ChangePasswordAsync(user, input.Password));
            user.PasswordResetCode = null;
            user.IsEmailConfirmed = true;
            user.ShouldChangePasswordOnNextLogin = false;

            await UserManager.UpdateAsync(user);

            return new ResetPasswordOutput
            {
                CanLogin = user.IsActive,
                UserName = user.UserName
            };
        }

        public async Task<ResetPasswordOutput> ResetPasswordMobile(ResetPasswordMobileInput input)
        {
            var applicationType = await ValidateClientSecret(input.ClientSecret, input.ClientId);

            var user = await UserManager.GetUserByIdAsync(input.UserId);
            if (user == null || user.PasswordResetCode.IsNullOrEmpty() || user.PasswordResetCode != input.ResetCode)
            {
                throw new UserFriendlyException(L("InvalidPasswordResetCode"), L("InvalidPasswordResetCode_Detail"));
            }

            await UserManager.InitializeOptionsAsync(input.TenantId);
            CheckErrors(await UserManager.ChangePasswordAsync(user, input.Password));
            user.PasswordResetCode = null;
            user.IsEmailConfirmed = true;
            user.ShouldChangePasswordOnNextLogin = false;

            await UserManager.UpdateAsync(user);

            return new ResetPasswordOutput
            {
                CanLogin = user.IsActive,
                UserName = user.UserName
            };
        }

        private bool UseCaptchaOnLogin()
        {
            //if (DebugHelper.IsDebug)
            //{
            //    return false;
            //}

            return SettingManager.GetSettingValue<bool>(AppSettings.UserManagement.UseCaptchaOnLogin);
        }

        public async Task SendEmailActivationLink(SendEmailActivationLinkInput input)
        {
            if (UseCaptchaOnLogin())
            {
                await RecaptchaValidator.ValidateAsync(input.CaptchaResponse);
            }

            var user = await GetUserByChecking(input.EmailAddress);
            user.SetNewEmailConfirmationCode();
            await _userEmailer.SendEmailActivationLinkAsync(user, AppUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId));
        }

        public async Task ActivateEmail(ActivateEmailInput input)
        {
            var user = await UserManager.GetUserByIdAsync(input.UserId);
            if (user == null || user.EmailConfirmationCode.IsNullOrEmpty() || user.EmailConfirmationCode != input.ConfirmationCode)
            {
                throw new UserFriendlyException(L("InvalidEmailConfirmationCode"), L("InvalidEmailConfirmationCode_Detail"));
            }

            user.IsEmailConfirmed = true;
            //user.EmailConfirmationCode = null;

            await UserManager.UpdateAsync(user);
        }

        //[AbpAuthorize(AppPermissions.Pages_Administration_Users_Impersonation)]
        //public virtual async Task<ImpersonateOutput> Impersonate(ImpersonateInput input)
        //{
        //    return new ImpersonateOutput
        //    {
        //        ImpersonationToken = await _impersonationManager.GetImpersonationToken(input.UserId, input.TenantId),
        //        TenancyName = await GetTenancyNameOrNullAsync(input.TenantId)
        //    };
        //}

        //public virtual async Task<ImpersonateOutput> BackToImpersonator()
        //{
        //    return new ImpersonateOutput
        //    {
        //        ImpersonationToken = await _impersonationManager.GetBackToImpersonatorToken(),
        //        TenancyName = await GetTenancyNameOrNullAsync(AbpSession.ImpersonatorTenantId)
        //    };
        //}

        public virtual async Task<SwitchToLinkedAccountOutput> SwitchToLinkedAccount(SwitchToLinkedAccountInput input)
        {
            if (!await _userLinkManager.AreUsersLinked(AbpSession.ToUserIdentifier(), input.ToUserIdentifier()))
            {
                throw new Exception(L("This account is not linked to your account"));
            }

            return new SwitchToLinkedAccountOutput
            {
                SwitchAccountToken = await _userLinkManager.GetAccountSwitchToken(input.TargetUserId, input.TargetTenantId),
                TenancyName = await GetTenancyNameOrNullAsync(input.TargetTenantId)
            };
        }

        private bool UseCaptchaOnRegistration()
        {
            if (DebugHelper.IsDebug)
            {
                return false;
            }

            return SettingManager.GetSettingValue<bool>(AppSettings.UserManagement.UseCaptchaOnRegistration);
        }

        private async Task<Tenant> GetActiveTenantAsync(int tenantId)
        {
            var tenant = await TenantManager.FindByIdAsync(tenantId);
            if (tenant == null)
            {
                throw new UserFriendlyException(L("UnknownTenantId{0}", tenantId));
            }

            if (!tenant.IsActive)
            {
                throw new UserFriendlyException(L("TenantIdIsNotActive{0}", tenantId));
            }

            return tenant;
        }

        private async Task<string> GetTenancyNameOrNullAsync(int? tenantId)
        {
            return tenantId.HasValue ? (await GetActiveTenantAsync(tenantId.Value)).TenancyName : null;
        }

        private async Task<User> GetUserByChecking(string inputEmailAddress)
        {
            var user = await UserManager.FindByEmailAsync(inputEmailAddress);
            if (user == null)
            {
                throw new UserFriendlyException(L("InvalidEmailAddress"));
            }

            return user;
        }
    }
}