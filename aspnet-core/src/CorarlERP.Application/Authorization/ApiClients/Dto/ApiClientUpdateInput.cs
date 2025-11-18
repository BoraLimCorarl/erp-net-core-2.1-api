using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Authorization.ApiClients.Dto
{
    public class ApiClientUpdateInput : ApiClientInput
    {
        public Guid Id { get; set; }      
        public string TenantName { get; set; }



    }
}
