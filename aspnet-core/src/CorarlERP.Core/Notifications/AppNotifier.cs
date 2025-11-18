using System;
using System.Threading.Tasks;
using Abp;
using Abp.Dependency;
using Abp.Localization;
using Abp.Notifications;
using CorarlERP.Authorization.Users;
using CorarlERP.MultiTenancy;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using CorarlERP.Configuration;

namespace CorarlERP.Notifications
{
    public class AppNotifier : CorarlERPDomainServiceBase, IAppNotifier
    {
        private readonly INotificationPublisher _notificationPublisher;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfigurationRoot _appConfiguration;
        public AppNotifier(INotificationPublisher notificationPublisher)
        {
            _notificationPublisher = notificationPublisher;
            _hostingEnvironment = IocManager.Instance.Resolve<IHostingEnvironment>();
            _appConfiguration = _hostingEnvironment.GetAppConfiguration();
        }

        public async Task WelcomeToTheApplicationAsync(User user)
        {
            var name = _appConfiguration["App:Name"];
            await _notificationPublisher.PublishAsync(
                AppNotificationNames.WelcomeToTheApplication,
                new MessageNotificationData(L("WelcomeToTheApplicationNotificationMessage", name)),
                severity: NotificationSeverity.Success,
                userIds: new[] { user.ToUserIdentifier() }
                );
        }

        public async Task NewUserRegisteredAsync(User user)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    "NewUserRegisteredNotificationMessage",
                    CorarlERPConsts.LocalizationSourceName
                    )
                );

            notificationData["userName"] = user.UserName;
            notificationData["emailAddress"] = user.EmailAddress;

            await _notificationPublisher.PublishAsync(AppNotificationNames.NewUserRegistered, notificationData, tenantIds: new[] { user.TenantId });
        }

        public async Task NewTenantRegisteredAsync(Tenant tenant)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    "NewTenantRegisteredNotificationMessage",
                    CorarlERPConsts.LocalizationSourceName
                    )
                );

            notificationData["tenancyName"] = tenant.TenancyName;
            await _notificationPublisher.PublishAsync(AppNotificationNames.NewTenantRegistered, notificationData);
        }

        public async Task GdprDataPrepared(UserIdentifier user, Guid binaryObjectId)
        {
            var notificationData = new LocalizableMessageNotificationData(
                new LocalizableString(
                    "GdprDataPreparedNotificationMessage",
                    CorarlERPConsts.LocalizationSourceName
                )
            );

            notificationData["binaryObjectId"] = binaryObjectId;

            await _notificationPublisher.PublishAsync(AppNotificationNames.GdprDataPrepared, notificationData, userIds: new[] { user });
        }

        //This is for test purposes
        public async Task SendMessageAsync(UserIdentifier user, string message, NotificationSeverity severity = NotificationSeverity.Info)
        {
            await _notificationPublisher.PublishAsync(
                "App.SimpleMessage",
                new MessageNotificationData(message),
                severity: severity,
                userIds: new[] { user }
                );
        }
    }
}