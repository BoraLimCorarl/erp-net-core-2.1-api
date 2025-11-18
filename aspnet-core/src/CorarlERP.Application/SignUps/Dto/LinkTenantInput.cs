using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.SignUps.Dto
{
    public class LinkTenantInput
    {
        public Guid Id { get; set; }
        public int? TenantId { get; set; }
    }
}
