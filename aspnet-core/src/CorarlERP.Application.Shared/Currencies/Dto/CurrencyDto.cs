using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Currencies.Dto
{
    public class CurrencyDto
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string PluralName { get; set; }
    }
}
