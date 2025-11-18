using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Authorization.ApiClients.Dto
{
    public class ApiClientDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ApplicationTypes ApplicationType { get; set; }
        public bool IsActive { get; set; }
        public int RefreshTokenLifeTime { get; set; }
        public string AllowedOrigin { get; set; }
        public int? TenantId { get; set; }
        public string TenantName { get; set; }
        public string  ClientId { get; set; }
        //public string ClientSecret { get; set; }
    }
}
