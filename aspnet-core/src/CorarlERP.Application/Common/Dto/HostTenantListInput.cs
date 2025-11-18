using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Common.Dto
{
    public class HostTenantListInput
    {
        public List<GetTenantEditionInput> Tenants { get; set; }
        public string RoleName { get; set; }
    }
    public class GetTenantEditionInput
    {
        public int Id { get; set; }

        public int? EditionId { get; set; }
    }
}
