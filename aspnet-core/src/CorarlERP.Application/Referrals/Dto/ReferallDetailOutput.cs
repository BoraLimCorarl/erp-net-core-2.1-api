using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Referrals.Dto
{
    public class ReferallDetailOutput
    {
        public long? Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Description { get; set; }
        public int? Qty { get; set; }
        public bool IsActive { get; set; }
    }
}
