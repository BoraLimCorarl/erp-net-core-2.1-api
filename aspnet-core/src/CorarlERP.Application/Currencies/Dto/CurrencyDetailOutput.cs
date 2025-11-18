using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Currencies.Dto
{
    [AutoMapFrom(typeof(Currency))]
    public class CurrencyDetailOutput { 
        public long Id { get; set; }
        public string Code { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string PluralName { get; set; }
    }

    public class CurrencySummaryOutput
    {
        public long Id { get; set; }
        public string Code { get; set; }
    }
}
