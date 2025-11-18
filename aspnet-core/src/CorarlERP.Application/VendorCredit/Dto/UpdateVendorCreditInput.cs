using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.VendorCredit.Dto
{
    public class UpdateVendorCreditInput: CreateVendorCreditInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}
