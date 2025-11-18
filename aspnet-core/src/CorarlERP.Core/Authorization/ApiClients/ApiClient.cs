using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Authorization.ApiClients
{
    public class ApiClient :AuditedEntity<Guid>, IAudited
    {
        public string Secret { get; private set; }
        public string Name { get; set; }
        public ApplicationTypes ApplicationType { get; private set; }
        public bool Active { get; private set; }     
        public string AllowedOrigin { get; private set; }
        public string ClientId { get; private set; }
        public int? OwnedByTenantId { get; private set; }

        public static ApiClient Create(long? creatorUserId, string clientId, string hashedSecret,
            string name, ApplicationTypes applicationType , string allowedOrigin,
            int? ownedByTenantId)
        {
            var result = new ApiClient()
            {
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                Id = Guid.NewGuid(),
                ClientId = clientId,
                Secret = hashedSecret,
                Name = name,
                ApplicationType = applicationType,            
                AllowedOrigin = allowedOrigin,
                Active = true,
                OwnedByTenantId = ownedByTenantId
            };
            return result;
        }

        public void Update(long? creatorUserId, string clientId, string hashedSecret, string name, ApplicationTypes applicationType, string allowedOrigin,int? ownedByTenantId)
        {
            Secret = hashedSecret;
            AllowedOrigin = allowedOrigin;
            LastModifierUserId = creatorUserId;
            LastModificationTime = Clock.Now;
            ApplicationType = applicationType;
            AllowedOrigin = allowedOrigin;
            OwnedByTenantId = ownedByTenantId;
        } 

        public void Enable(bool isActive)
        {
            this.Active = isActive;
        }
    }

    public enum ApplicationTypes
    {
        FrontEnd = 0,
        Api = 1,
        MobileApp = 2
    };
}
