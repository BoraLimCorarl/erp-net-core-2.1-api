using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.MultiCurrencys.Dto
{
    [AutoMapFrom(typeof(MultiCurrencies.MultiCurrency))]
    public class MultiCurrencyDetailOutput { 
        public long Id { get; set; }
        public string Code { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string PluralName { get; set; }
        public long? CurrencyId { get; set; }
    }
}
