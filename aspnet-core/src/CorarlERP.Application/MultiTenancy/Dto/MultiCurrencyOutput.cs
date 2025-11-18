using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.MultiTenancy.Dto
{
   public class MultiCurrencyOutput
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Symbol { get; set; }
        public long? CurrencyId { get; set; }
    }
}
