using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Vendors.Dto
{
   public class UpdateVendorInput: CreateVendorInput
    {
        public Guid Id { get; set; }
    }
}
