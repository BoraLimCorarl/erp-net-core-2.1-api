using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Taxes.Dto
{
    [AutoMapFrom(typeof(Tax))]
    public class TaxDetailOutput
    {
        public long Id { get; set; }
        public string TaxName { get; set; }
        public decimal TaxRate { get; set; }
        public bool IsActive { get; set; }
    }
    [AutoMapFrom(typeof(Tax))]
    public class TaxSummaryOutput
    {
        public long Id { get; set; }
        public string TaxName { get; set; }
        public decimal TaxRate { get; set; }
       
    }
}
