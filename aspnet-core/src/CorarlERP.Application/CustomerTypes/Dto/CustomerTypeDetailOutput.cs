using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.CustomerTypes.Dto
{
    [AutoMapFrom(typeof(CustomerType))]
    public class CustomerTypeDetailOutput
    {
       public long Id { get; set; }
       public string CustomerTypeName { get; set; }        
       public bool IsActive { get; set; }
      
    }
    [AutoMapFrom(typeof(CustomerType))]
    public class CustomerTypeSummaryOutput
    {
        public long Id { get; set; }
        public string CustomerTypeName { get; set; }
       
    }
}
