using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Authorization.ApiClients.Dto
{
    public class ApiClientInput
    {
        public string Name { get; set; }
        public ApplicationTypes ApplicationType { get; set; }
        public int RefreshTokenLifeTime { get; set; }
        public string AllowedOrigin { get; set; }
        public string PlaintSecret { get; set; }
        public string ClientId { get; set; }
        public int? OwnedByTenantId { get; set; }

    }
}
