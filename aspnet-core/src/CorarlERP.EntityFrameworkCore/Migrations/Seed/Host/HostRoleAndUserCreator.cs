using System.Linq;
using Abp;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.Notifications;
using Microsoft.EntityFrameworkCore;
using CorarlERP.Authorization;
using CorarlERP.Authorization.Roles;
using CorarlERP.Authorization.Users;
using CorarlERP.EntityFrameworkCore;
using CorarlERP.Notifications;
using CorarlERP.Authorization.ApiClients;

namespace CorarlERP.Migrations.Seed.Host
{
    public class HostRoleAndUserCreator
    {
        private readonly CorarlERPDbContext _context;

        public HostRoleAndUserCreator(CorarlERPDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateHostRoleAndUsers();
            CreateDefaultApiClientId();
        }
        private void CreateDefaultApiClientId()
        {
            //Admin role for host
            var clientId = "572d23b2csfe40fd8ebf231e9e1de485c"; //web client id 

            var apiClient = _context.ApiClients.IgnoreQueryFilters().FirstOrDefault(r => r.ClientId == clientId);
            if (apiClient == null)
            {
                //must update from * to correct url and secrete in web after logging in
                var webClient = ApiClient.Create(null, clientId, "si9zNICEGMSP9E2PsfWOWL9ISd87Q==", "Zobbook Frontend", ApplicationTypes.FrontEnd, "*", null);
                apiClient = _context.ApiClients.Add(webClient).Entity;
                _context.SaveChanges();
            }


        }
        private void CreateHostRoleAndUsers()
        {
            //Admin role for host

            var adminRoleForHost = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == null && r.Name == StaticRoleNames.Host.Admin);
            if (adminRoleForHost == null)
            {
                adminRoleForHost = _context.Roles.Add(new Role(null, StaticRoleNames.Host.Admin, StaticRoleNames.Host.Admin) { IsStatic = true, IsDefault = true }).Entity;
                _context.SaveChanges();
            }

            //admin user for host

            var adminUserForHost = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == null && u.UserName == AbpUserBase.AdminUserName);
            if (adminUserForHost == null)
            {
                var user = new User
                {
                    TenantId = null,
                    UserName = AbpUserBase.AdminUserName,
                    Name = "admin",
                    Surname = "admin",
                    EmailAddress = "admin@aspnetzero.com",
                    IsEmailConfirmed = true,
                    ShouldChangePasswordOnNextLogin = true,
                    IsActive = true,
                    Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==" //123qwe
                };

                user.SetNormalizedNames();

                adminUserForHost = _context.Users.Add(user).Entity;
                _context.SaveChanges();

                //Assign Admin role to admin user
                _context.UserRoles.Add(new UserRole(null, adminUserForHost.Id, adminRoleForHost.Id));
                _context.SaveChanges();

                //User account of admin user
                _context.UserAccounts.Add(new UserAccount
                {
                    TenantId = null,
                    UserId = adminUserForHost.Id,
                    UserName = AbpUserBase.AdminUserName,
                    EmailAddress = adminUserForHost.EmailAddress
                });

                _context.SaveChanges();

                //Notification subscriptions
                _context.NotificationSubscriptions.Add(new NotificationSubscriptionInfo(SequentialGuidGenerator.Instance.Create(), null, adminUserForHost.Id, AppNotificationNames.NewTenantRegistered));
                _context.NotificationSubscriptions.Add(new NotificationSubscriptionInfo(SequentialGuidGenerator.Instance.Create(), null, adminUserForHost.Id, AppNotificationNames.NewUserRegistered));

                _context.SaveChanges();
            }
        }
    }
}