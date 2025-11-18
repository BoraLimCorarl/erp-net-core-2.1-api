using CorarlERP.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Authorization.ApiClients
{
    public class ApiClientWithTenant
    {
        public ApiClient ApiClient { get; set; }
        public Tenant Tenant { get; set; }
    }
}
