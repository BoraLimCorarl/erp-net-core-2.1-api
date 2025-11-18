using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.VendorTypes.Dto
{
    [AutoMapFrom(typeof(VendorType))]
    public class VendorTypeDetailOutput
    {
       public long Id { get; set; }
       public string VendorTypeName { get; set; }        
       public bool IsActive { get; set; }
      
    }
    [AutoMapFrom(typeof(VendorType))]
    public class VendorTypeSummaryOutput
    {
        public long Id { get; set; }
        public string VendorTypeName { get; set; }
       
    }
}
