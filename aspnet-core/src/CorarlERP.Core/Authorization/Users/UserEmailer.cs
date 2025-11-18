using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Localization;
using Abp.Net.Mail;
using CorarlERP.Chat;
using CorarlERP.Editions;
using CorarlERP.Emailing;
using CorarlERP.Localization;
using CorarlERP.MultiTenancy;
using System.Net.Mail;
using System.Web;
using Abp.Runtime.Security;
using CorarlERP.Url;
using CorarlERP.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace CorarlERP.Authorization.Users
{
    /// <summary>
    /// Used to send email to users.
    /// </summary>
    public class UserEmailer : CorarlERPServiceBase, IUserEmailer, ITransientDependency
    {
        private readonly IEmailTemplateProvider _emailTemplateProvider;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly ICurrentUnitOfWorkProvider _unitOfWorkProvider;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<User, long> _userRepository;
        private readonly ISettingManager _settingManager;
        private readonly EditionManager _editionManager;
        private readonly IWebUrlService _webUrlService;
        private readonly IConfigurationRoot _configuration;

        public UserEmailer(
            IHostingEnvironment hostingEnvironment,
            IEmailTemplateProvider emailTemplateProvider,
            IEmailSender emailSender,
            IRepository<Tenant> tenantRepository,
            ICurrentUnitOfWorkProvider unitOfWorkProvider,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<User, long> userRepository,
            ISettingManager settingManager, EditionManager editionManager)
        {
            _emailTemplateProvider = emailTemplateProvider;
            _emailSender = emailSender;
            _tenantRepository = tenantRepository;
            _unitOfWorkProvider = unitOfWorkProvider;
            _unitOfWorkManager = unitOfWorkManager;
            _userRepository = userRepository;
            _settingManager = settingManager;
            _editionManager = editionManager;
            _configuration = hostingEnvironment.GetAppConfiguration();
        }

        protected string AppName => _configuration["App:Name"];
        protected string LogoLinkURL => _configuration["App:LogoLinkURL"];
        protected string LogoSourceURL => _configuration["App:LogoSourceURL"];
        protected string TutorialURLApp => _configuration["App:Support:TutorialURL"];
        protected string HowLogInApp => _configuration["App:Support:HowLogIn"];
        protected string HowSetUpDataApp => _configuration["App:Support:HowToActivateAccount"];


        /// <summary>
        /// Send email activation link to user's email address.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Email activation link</param>
        /// <param name="plainPassword">
        /// Can be set to user's plain password to include it in the email. 
        /// </param>
        [UnitOfWork]
        public virtual async Task SendEmailActivationLinkAsync(User user, string link, string plainPassword = null)
        {
            var appName = AppName;
            var TutorialURL =$"How to use {appName} system overall";
            var HowSetupData = $"How to Actiave your new {appName} User Account via Email";
            var HowToLogin = $"How to login to {appName} Inventory";
            var MessageLink = "Learn our tutorials by clicking any of the links below:";

            var TutorialURLKh = $"- របៀបប្រើប្រាស់ប្រព័ន្ធ {appName} ទាំងមូល";
            var HowSetupDataKh = $"- របៀប Activate គណនី {appName} ថ្មីរបស់អ្នកតាមអ៊ីមែល";
            var HowToLoginKh = "- របៀបចូលប្រើកម្មវិធី";

            if (user.EmailConfirmationCode.IsNullOrEmpty())
            {
                throw new Exception("EmailConfirmationCode should be set in order to send email activation link.");
            }

            link = link.Replace("{userId}", user.Id.ToString());
            link = link.Replace("{confirmationCode}", Uri.EscapeDataString(user.EmailConfirmationCode));

            if (user.TenantId.HasValue)
            {
                link = link.Replace("{tenantId}", user.TenantId.ToString());
            }

            link = EncryptQueryParameters(link);

            var tenant = GetTenancyOrNull(user.TenantId);
            var tenancyName = tenant == null ? null : tenant.TenancyName;
            var companyTenantName = tenant == null ? "" : tenant.Name;
            var emailTitle = $"Welcome to {appName}!";
            
            var emailTemplate = GetReactivateEmail();//GetTitleAndSubTitle(user.TenantId, L("EmailActivation_Title"), L("EmailActivation_SubTitle"));
            var emailDetailInfoMessage = new StringBuilder("");
            //company
            if (!string.IsNullOrEmpty(companyTenantName))
            {
                emailDetailInfoMessage.AppendLine("<tr>");
                emailDetailInfoMessage.AppendLine("<td class='centerForSmall mobileFontSize'" +
                                                        "style='font - family:Arial, Helvetica, sans - serif; font - size:13px;" +
                                                        "color:#333333;mso-line-height-rule:exactly;line-height:24px;'>");
                emailDetailInfoMessage.AppendLine("<b>Subdomain:</b> " + tenancyName);
                emailDetailInfoMessage.AppendLine("</td>");
                emailDetailInfoMessage.AppendLine("</tr>");

            }
            //user
            emailDetailInfoMessage.AppendLine("<tr>");
            emailDetailInfoMessage.AppendLine("<td class='centerForSmall mobileFontSize'" +
                                                    "style='font - family:Arial, Helvetica, sans - serif; font - size:13px;" +
                                                    "color:#333333;mso-line-height-rule:exactly;line-height:24px;'>");
            emailDetailInfoMessage.AppendLine("<b>Username:</b> " + user.UserName);
            emailDetailInfoMessage.AppendLine("</td>");
            emailDetailInfoMessage.AppendLine("</tr>");

            //password
            if (!plainPassword.IsNullOrEmpty())
            {
                emailDetailInfoMessage.AppendLine("<tr>");
                emailDetailInfoMessage.AppendLine("<td class='centerForSmall mobileFontSize'" +
                                                        "style='font - family:Arial, Helvetica, sans - serif; font - size:13px;" +
                                                        "color:#333333;mso-line-height-rule:exactly;line-height:24px;'>");
                emailDetailInfoMessage.AppendLine("<b>Temporary Password:</b> " + plainPassword);
                emailDetailInfoMessage.AppendLine("</td>");
                emailDetailInfoMessage.AppendLine("</tr>");
            }
            var emailLinkButton = new StringBuilder("");

            emailLinkButton.AppendLine("<tr>");
            emailLinkButton.AppendLine("<td align='center' style='padding: 0px 24px;'>");
            emailLinkButton.AppendLine("<a style='text-decoration:none; " +
                                           "font-family:Arial,Helvetica, sans-serif; font-size:17px; font-weight:bold;" +
                                           "color:#ffffff;white-space:nowrap;'" +
                                           "href=\"" + link + "\"" + " target='_blank'>");
            emailLinkButton.AppendLine("Activate Your Account");
            emailLinkButton.AppendLine("</a>");
            emailLinkButton.AppendLine("</td>");
            emailLinkButton.AppendLine("</tr>");


            var emailIntroductionMessage = new StringBuilder("");
            emailIntroductionMessage.AppendLine($"Welcome to {appName}! We at {appName} are delighted to have you as our newest {appName} user! <br />" +
                                                $"To start using {appName}, please activate your account by clicking on the activation button below:");

            var emailBeforeLinkButtonMessage = new StringBuilder("");
            emailBeforeLinkButtonMessage.AppendLine($"After activation, you can sign in to {appName} at " +
                                                    "<a href =\"" + link + "\">" + link + "</a>");

            var emailAfterLinkButtonMessage = new StringBuilder("");
            emailAfterLinkButtonMessage.AppendLine("By clicking the activation button above, you agree to our Terms and Conditions. " +
                                                    "If you did not register or registered by mistake with us, " +
                                                    "please DO NOT click the activation button " +
                                                    "or ignore this email.Then, the registration won’t be completed.");


            if (!string.IsNullOrWhiteSpace(HowLogInApp) || !string.IsNullOrWhiteSpace(HowSetUpDataApp) || !string.IsNullOrWhiteSpace(TutorialURL))
            {
              
                emailAfterLinkButtonMessage.AppendLine("<tr>");
                
                emailAfterLinkButtonMessage.AppendLine("<td class='centerForSmall mobileFontSize'" +
                                                        "style='font-weight: bold; font - family:Arial, Helvetica, sans - serif; font - size:13px;" +
                                                        "color:#333333;mso-line-height-rule:exactly;line-height:24px;'>");
                emailAfterLinkButtonMessage.AppendLine("<hr/>");
                emailAfterLinkButtonMessage.AppendLine("<br/>");
                emailAfterLinkButtonMessage.AppendLine("<span style='font-weight: bold;'>");
                emailAfterLinkButtonMessage.AppendLine($"{MessageLink}");
                emailAfterLinkButtonMessage.AppendLine("</span>");
                emailAfterLinkButtonMessage.AppendLine("</td>");
                emailAfterLinkButtonMessage.AppendLine("</tr>");
            }

            if (!string.IsNullOrWhiteSpace(HowSetUpDataApp))
            {
                emailAfterLinkButtonMessage.AppendLine("<tr>");
                emailAfterLinkButtonMessage.AppendLine("<td class='centerForSmall mobileFontSize'" +
                                                        "style='font - family:Arial, Helvetica, sans - serif; font - size:13px;" +
                                                        "color:#333333;mso-line-height-rule:exactly;line-height:24px;'>");

                emailAfterLinkButtonMessage.AppendLine("<br/>");
                emailAfterLinkButtonMessage.AppendLine("<a href =\"" + HowSetUpDataApp + "\"" + " target='_blank'>");
                emailAfterLinkButtonMessage.AppendLine($"{HowSetupDataKh}");
                emailAfterLinkButtonMessage.AppendLine("<br/>");
                emailAfterLinkButtonMessage.AppendLine($"{HowSetupData}");
                emailAfterLinkButtonMessage.AppendLine("</a>");
                emailAfterLinkButtonMessage.AppendLine("");
                emailAfterLinkButtonMessage.AppendLine("</td>");
                emailAfterLinkButtonMessage.AppendLine("</tr>");
            }

            if (!string.IsNullOrWhiteSpace(HowLogInApp))
            {
                emailAfterLinkButtonMessage.AppendLine("<tr>");
                emailAfterLinkButtonMessage.AppendLine("<td class='centerForSmall mobileFontSize'" +
                                                        "style='font - family:Arial, Helvetica, sans - serif; font - size:13px;" +
                                                        "color:#333333;mso-line-height-rule:exactly;line-height:24px;'>");
                emailAfterLinkButtonMessage.AppendLine("<a href =\"" + HowLogInApp + "\"" + " target='_blank'>");
                emailAfterLinkButtonMessage.AppendLine($"{HowToLoginKh}");
                emailAfterLinkButtonMessage.AppendLine("<br/>");
                emailAfterLinkButtonMessage.AppendLine($"{HowToLogin}");
                emailAfterLinkButtonMessage.AppendLine("</a>");
                emailAfterLinkButtonMessage.AppendLine("</td>");
                emailAfterLinkButtonMessage.AppendLine("</tr>");
            }
           

            if (!string.IsNullOrWhiteSpace(TutorialURLApp))
            {
                emailAfterLinkButtonMessage.AppendLine("<tr>");
                emailAfterLinkButtonMessage.AppendLine("<td class='centerForSmall mobileFontSize'" +
                                                        "style='font - family:Arial, Helvetica, sans - serif; font - size:13px;" +
                                                        "color:#333333;mso-line-height-rule:exactly;line-height:24px;'>");


                emailAfterLinkButtonMessage.AppendLine("<a href =\"" + TutorialURLApp + "\"" + " target='_blank'>");
                emailAfterLinkButtonMessage.AppendLine($"{TutorialURLKh}");
                emailAfterLinkButtonMessage.AppendLine("<br/>");
                emailAfterLinkButtonMessage.AppendLine($"{TutorialURL}");
                emailAfterLinkButtonMessage.AppendLine("</a>");
                emailAfterLinkButtonMessage.AppendLine("<hr/>");
                emailAfterLinkButtonMessage.AppendLine("</td>");
                emailAfterLinkButtonMessage.AppendLine("</tr>");
            }



            emailTemplate.Replace("{EMAIL_TITLE_VALUE}", emailTitle);
            emailTemplate.Replace("{EMAIL_FULL_NAME_VALUE}", user.Name);
            emailTemplate.Replace("{EMAIL_INTRODUCTION_MESSAGE}", emailIntroductionMessage.ToString());
            emailTemplate.Replace("{EMAIL_ACCOUNT_DETAIL_INFO}", emailDetailInfoMessage.ToString());
            emailTemplate.Replace("{EMAIL_BEFORE_LINK_BUTTON_MESSAGE}", emailBeforeLinkButtonMessage.ToString());
            emailTemplate.Replace("{EMAIL_CLICK_BUTTON}", emailLinkButton.ToString());
            emailTemplate.Replace("{EMAIL_AFTER_LINK_BUTTON_MESSAGE}", emailAfterLinkButtonMessage.ToString());
            emailTemplate.Replace("@AppName", appName);
            emailTemplate.Replace("@LogoLinkURL", LogoLinkURL);
            emailTemplate.Replace("@LogoSourceURL", LogoSourceURL);

            var subject = L("EmailActivation_Corarl", appName);
            await _emailSender.SendAsync(user.EmailAddress, subject, emailTemplate.ToString());
           
            
        }

        /// <summary>
        /// Sends a password reset link to user's email.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Reset link</param>
        public async Task SendPasswordResetLinkAsync(string appName, User user, string link = null)
        {
            if (user.PasswordResetCode.IsNullOrEmpty())
            {
                throw new Exception("PasswordResetCode should be set in order to send password reset link.");
            }

            var tenancyName = GetTenancyNameOrNull(user.TenantId);
            var emailTemplate = GetTitleAndSubTitle(user.TenantId, L("PasswordResetEmail_Title", appName), L("PasswordResetEmail_SubTitle"));
            var mailMessage = new StringBuilder();

            mailMessage.AppendLine("<b>" + L("NameSurname") + "</b>: " + user.Name + "<br />");

            if (!tenancyName.IsNullOrEmpty())
            {
                mailMessage.AppendLine("<b>" + L("TenancyName") + "</b>: " + tenancyName + "<br />");
            }

            mailMessage.AppendLine("<b>" + L("UserName") + "</b>: " + user.UserName + "<br />");
            mailMessage.AppendLine("<b>" + L("ResetCode") + "</b>: " + user.PasswordResetCode + "<br />");

            if (!link.IsNullOrEmpty())
            {
                link = link.Replace("{userId}", user.Id.ToString());
                link = link.Replace("{resetCode}", Uri.EscapeDataString(user.PasswordResetCode));

                if (user.TenantId.HasValue)
                {
                    link = link.Replace("{tenantId}", user.TenantId.ToString());
                }

                link = EncryptQueryParameters(link);

                mailMessage.AppendLine("<br />");
                mailMessage.AppendLine(L("PasswordResetEmail_ClickTheLinkBelowToResetYourPassword") + "<br /><br />");
                mailMessage.AppendLine("<a href=\"" + link + "\">" + link + "</a>");
            }

            await ReplaceBodyAndSend(user.EmailAddress, L("PasswordResetEmail_Subject", appName), emailTemplate, mailMessage);
        }

        //New template of send forget password
        public async Task SendPasswordResetLinkAsyncNew(string appName, User user, string link = null)
        {
            #region original
            //if (user.PasswordResetCode.IsNullOrEmpty())
            //{
            //    throw new Exception("PasswordResetCode should be set in order to send password reset link.");
            //}

            //var tenancyName = GetTenancyNameOrNull(user.TenantId);
            //var emailTemplate = GetTitleAndSubTitle(user.TenantId, L("PasswordResetEmail_Title"), L("PasswordResetEmail_SubTitle"));
            
            //var mailMessage = new StringBuilder();

            //mailMessage.AppendLine("<b>" + L("NameSurname") + "</b>: " + user.Name + " " + user.Surname + "<br />");

            //if (!tenancyName.IsNullOrEmpty())
            //{
            //    mailMessage.AppendLine("<b>" + L("TenancyName") + "</b>: " + tenancyName + "<br />");
            //}

            //mailMessage.AppendLine("<b>" + L("UserName") + "</b>: " + user.UserName + "<br />");
            //mailMessage.AppendLine("<b>" + L("ResetCode") + "</b>: " + user.PasswordResetCode + "<br />");

            //if (!link.IsNullOrEmpty())
            //{
            //    link = link.Replace("{userId}", user.Id.ToString());
            //    link = link.Replace("{resetCode}", Uri.EscapeDataString(user.PasswordResetCode));

            //    if (user.TenantId.HasValue)
            //    {
            //        link = link.Replace("{tenantId}", user.TenantId.ToString());
            //    }

            //    link = EncryptQueryParameters(link);

            //    mailMessage.AppendLine("<br />");
            //    mailMessage.AppendLine(L("PasswordResetEmail_ClickTheLinkBelowToResetYourPassword") + "<br /><br />");
            //    mailMessage.AppendLine("<a href=\"" + link + "\">" + link + "</a>");
            //}

            //await ReplaceBodyAndSend(user.EmailAddress, L("PasswordResetEmail_Subject"), emailTemplate, mailMessage);
            #endregion

            if (user.PasswordResetCode.IsNullOrEmpty())
            {
                throw new Exception("PasswordResetCode should be set in order to send email activation link.");
            }
            if (!link.IsNullOrEmpty())
            {
                link = link.Replace("{userId}", user.Id.ToString());
                link = link.Replace("{resetCode}", Uri.EscapeDataString(user.PasswordResetCode));

                if (user.TenantId.HasValue)
                {
                    link = link.Replace("{tenantId}", user.TenantId.ToString());
                }

                link = EncryptQueryParameters(link);
            }
            var tenant = GetTenancyOrNull(user.TenantId);
            var tenancyName = tenant == null ? null : tenant.TenancyName;
            var companyTanantName = tenant == null ? "" : tenant.Name;
            var emailTitle = $"Welcome to {appName}!";

            var emailTemplate = GetReactivateEmail();//GetTitleAndSubTitle(user.TenantId, L("EmailActivation_Title"), L("EmailActivation_SubTitle"));
            var emailDetailInfoMessage = new StringBuilder("");
            //company
            if (!string.IsNullOrEmpty(companyTanantName))
            {
                emailDetailInfoMessage.AppendLine("<tr>");
                emailDetailInfoMessage.AppendLine("<td class='centerForSmall mobileFontSize'" +
                                                        "style='font - family:Arial, Helvetica, sans - serif; font - size:13px;" +
                                                        "color:#333333;mso-line-height-rule:exactly;line-height:24px;'>");
                emailDetailInfoMessage.AppendLine("<b>Subdomain:</b> " + tenancyName);
                emailDetailInfoMessage.AppendLine("</td>");
                emailDetailInfoMessage.AppendLine("</tr>");

            }
            //user
            emailDetailInfoMessage.AppendLine("<tr>");
            emailDetailInfoMessage.AppendLine("<td class='centerForSmall mobileFontSize'" +
                                                    "style='font - family:Arial, Helvetica, sans - serif; font - size:13px;" +
                                                    "color:#333333;mso-line-height-rule:exactly;line-height:24px;'>");
            emailDetailInfoMessage.AppendLine("<b>Username:</b> " + user.UserName);
            emailDetailInfoMessage.AppendLine("</td>");
            emailDetailInfoMessage.AppendLine("</tr>");

            
            var emailLinkButton = new StringBuilder("");

            emailLinkButton.AppendLine("<tr>");
            emailLinkButton.AppendLine("<td align='center' style='padding: 0px 24px;'>");
            emailLinkButton.AppendLine("<a style='text-decoration:none; " +
                                           "font-family:Arial,Helvetica, sans-serif; font-size:17px; font-weight:bold;" +
                                           "color:#ffffff;white-space:nowrap;'" +
                                           "href=\"" + link + "\"" + " target='_blank'>");
            emailLinkButton.AppendLine("Reset Password");
            emailLinkButton.AppendLine("</a>");
            emailLinkButton.AppendLine("</td>");
            emailLinkButton.AppendLine("</tr>");


            var emailIntroductionMessage = new StringBuilder("");
            emailIntroductionMessage.AppendLine("This email is sent to you to reset or recreate  your password.<br />");

            var emailBeforeLinkButtonMessage = new StringBuilder("");
            emailBeforeLinkButtonMessage.AppendLine("Please click the button below to reset your password: ");

            var emailAfterLinkButtonMessage = new StringBuilder("");
            emailAfterLinkButtonMessage.AppendLine($"Ater reset password, you can sign in to {appName}.");

            emailTemplate.Replace("{EMAIL_TITLE_VALUE}", emailTitle);
            emailTemplate.Replace("{EMAIL_FULL_NAME_VALUE}", user.Name);
            emailTemplate.Replace("{EMAIL_INTRODUCTION_MESSAGE}", emailIntroductionMessage.ToString());
            emailTemplate.Replace("{EMAIL_ACCOUNT_DETAIL_INFO}", emailDetailInfoMessage.ToString());
            emailTemplate.Replace("{EMAIL_BEFORE_LINK_BUTTON_MESSAGE}", emailBeforeLinkButtonMessage.ToString());
            emailTemplate.Replace("{EMAIL_CLICK_BUTTON}", emailLinkButton.ToString());
            emailTemplate.Replace("{EMAIL_AFTER_LINK_BUTTON_MESSAGE}", emailAfterLinkButtonMessage.ToString());

            emailTemplate.Replace("@AppName", appName);
            emailTemplate.Replace("@LogoLinkURL", LogoLinkURL);
            emailTemplate.Replace("@LogoSourceURL", LogoSourceURL);

            var subject = L("ResetPassword_Corarl", appName);
            await _emailSender.SendAsync(user.EmailAddress, subject, emailTemplate.ToString());

        }

        public async void TryToSendChatMessageMail(User user, string senderUsername, string senderTenancyName, ChatMessage chatMessage)
        {
            try
            {
                var emailTemplate = GetTitleAndSubTitle(user.TenantId, L("NewChatMessageEmail_Title"), L("NewChatMessageEmail_SubTitle"));
                var mailMessage = new StringBuilder();

                mailMessage.AppendLine("<b>" + L("Sender") + "</b>: " + senderTenancyName + "/" + senderUsername + "<br />");
                mailMessage.AppendLine("<b>" + L("Time") + "</b>: " + chatMessage.CreationTime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss") + " UTC<br />");
                mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + chatMessage.Message + "<br />");
                mailMessage.AppendLine("<br />");

                await ReplaceBodyAndSend(user.EmailAddress, L("NewChatMessageEmail_Subject"), emailTemplate, mailMessage);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        public async void TryToSendSubscriptionExpireEmail(string appName, int tenantId, DateTime utcNow)
        {
            try
            {
                using (_unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        var tenantAdmin = _userRepository.FirstOrDefault(u => u.UserName == AbpUserBase.AdminUserName);
                        if (tenantAdmin == null || string.IsNullOrEmpty(tenantAdmin.EmailAddress))
                        {
                            return;
                        }

                        var hostAdminLanguage = _settingManager.GetSettingValueForUser(LocalizationSettingNames.DefaultLanguage, tenantAdmin.TenantId, tenantAdmin.Id);
                        var culture = CultureHelper.GetCultureInfoByChecking(hostAdminLanguage);
                        var emailTemplate = GetTitleAndSubTitle(tenantId, L("SubscriptionExpire_Title", appName), L("SubscriptionExpire_SubTitle"));
                        var mailMessage = new StringBuilder();

                        mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + L("SubscriptionExpire_Email_Body", culture, utcNow.ToString("yyyy-MM-dd") + " UTC", appName) + "<br />" );
                        mailMessage.AppendLine("<br />");

                        await ReplaceBodyAndSend(tenantAdmin.EmailAddress, L("SubscriptionExpire_Email_Subject", appName), emailTemplate, mailMessage);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        public async Task TryToSendSubscriptionAssignedToAnotherEmail(string appName, int tenantId, DateTime utcNow, int expiringEditionId)
        {
            try
            {
                using (_unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        var tenantAdmin = _userRepository.FirstOrDefault(u => u.UserName == AbpUserBase.AdminUserName);
                        if (tenantAdmin == null || string.IsNullOrEmpty(tenantAdmin.EmailAddress))
                        {
                            return;
                        }

                        var hostAdminLanguage = _settingManager.GetSettingValueForUser(LocalizationSettingNames.DefaultLanguage, tenantAdmin.TenantId, tenantAdmin.Id);
                        var culture = CultureHelper.GetCultureInfoByChecking(hostAdminLanguage);
                        var expringEdition = await _editionManager.GetByIdAsync(expiringEditionId);
                        var emailTemplate = GetTitleAndSubTitle(tenantId, L("SubscriptionExpire_Title"), L("SubscriptionExpire_SubTitle"));
                        var mailMessage = new StringBuilder();

                        mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + L("SubscriptionAssignedToAnother_Email_Body", culture, expringEdition.DisplayName, utcNow.ToString("yyyy-MM-dd") + " UTC", appName) + "<br />");
                        mailMessage.AppendLine("<br />");

                        await ReplaceBodyAndSend(tenantAdmin.EmailAddress, L("SubscriptionExpire_Email_Subject"), emailTemplate, mailMessage);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        public async void TryToSendFailedSubscriptionTerminationsEmail(List<string> failedTenancyNames, DateTime utcNow)
        {
            try
            {
                var hostAdmin = _userRepository.FirstOrDefault(u => u.UserName == AbpUserBase.AdminUserName);
                if (hostAdmin == null || string.IsNullOrEmpty(hostAdmin.EmailAddress))
                {
                    return;
                }

                var hostAdminLanguage = _settingManager.GetSettingValueForUser(LocalizationSettingNames.DefaultLanguage, hostAdmin.TenantId, hostAdmin.Id);
                var culture = CultureHelper.GetCultureInfoByChecking(hostAdminLanguage);
                var emailTemplate = GetTitleAndSubTitle(null, L("FailedSubscriptionTerminations_Title"), L("FailedSubscriptionTerminations_SubTitle"));
                var mailMessage = new StringBuilder();

                mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + L("FailedSubscriptionTerminations_Email_Body", culture, string.Join(",", failedTenancyNames), utcNow.ToString("yyyy-MM-dd") + " UTC") + "<br />");
                mailMessage.AppendLine("<br />");

                await ReplaceBodyAndSend(hostAdmin.EmailAddress, L("FailedSubscriptionTerminations_Email_Subject"), emailTemplate, mailMessage);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        public async void TryToSendSubscriptionExpiringSoonEmail(string appName, int tenantId, DateTime dateToCheckRemainingDayCount)
        {
            try
            {
                using (_unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        var tenantAdmin = _userRepository.FirstOrDefault(u => u.UserName == AbpUserBase.AdminUserName);
                        if (tenantAdmin == null || string.IsNullOrEmpty(tenantAdmin.EmailAddress))
                        {
                            return;
                        }

                        var tenantAdminLanguage = _settingManager.GetSettingValueForUser(LocalizationSettingNames.DefaultLanguage, tenantAdmin.TenantId, tenantAdmin.Id);
                        var culture = CultureHelper.GetCultureInfoByChecking(tenantAdminLanguage);

                        var emailTemplate = GetTitleAndSubTitle(null, L("SubscriptionExpiringSoon_Title", appName), L("SubscriptionExpiringSoon_SubTitle"));
                        var mailMessage = new StringBuilder();

                        mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + L("SubscriptionExpiringSoon_Email_Body", culture, dateToCheckRemainingDayCount.ToString("yyyy-MM-dd") + " UTC", appName) + "<br />");
                        mailMessage.AppendLine("<br />");

                        await ReplaceBodyAndSend(tenantAdmin.EmailAddress, L("SubscriptionExpiringSoon_Email_Subject", appName), emailTemplate, mailMessage);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        private string GetTenancyNameOrNull(int? tenantId)
        {
            if (tenantId == null)
            {
                return null;
            }

            using (_unitOfWorkProvider.Current.SetTenantId(null))
            {
                return _tenantRepository.Get(tenantId.Value).TenancyName;
            }
        }

        private Tenant GetTenancyOrNull(int? tenantId)
        {
            if (tenantId == null)
            {
                return null;
            }

            using (_unitOfWorkProvider.Current.SetTenantId(null))
            {
                return _tenantRepository.Get(tenantId.Value);
            }
        }

        private StringBuilder GetTitleAndSubTitle(int? tenantId, string title, string subTitle)
        {
            var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(tenantId));
            emailTemplate.Replace("{EMAIL_TITLE}", title);
            emailTemplate.Replace("{EMAIL_SUB_TITLE}", subTitle);

            return emailTemplate;
        }


        private StringBuilder GetReactivateEmail()
        {
            var emailTemplate = new StringBuilder(_emailTemplateProvider.GetActivationTemplate());
            
            return emailTemplate;
        }


        private async Task ReplaceBodyAndSend(string emailAddress, string subject, StringBuilder emailTemplate, StringBuilder mailMessage)
        {
            emailTemplate.Replace("{EMAIL_BODY}", mailMessage.ToString());
            await _emailSender.SendAsync(new MailMessage
            {
                To = { emailAddress },
                Subject = subject,
                Body = emailTemplate.ToString(),
                IsBodyHtml = true
            });
        }

        /// <summary>
        /// Returns link with encrypted parameters
        /// </summary>
        /// <param name="link"></param>
        /// <param name="encrptedParameterName"></param>
        /// <returns></returns>
        private string EncryptQueryParameters(string link, string encrptedParameterName = "c")
        {
            if (!link.Contains("?"))
            {
                return link;
            }

            var uri = new Uri(link);
            var basePath = link.Substring(0, link.IndexOf('?'));
            var query = uri.Query.TrimStart('?');

            return basePath + "?" + encrptedParameterName + "=" + HttpUtility.UrlEncode(SimpleStringCipher.Instance.Encrypt(query));
        }
    }
}