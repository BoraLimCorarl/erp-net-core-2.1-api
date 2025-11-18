using System;
using System.Collections.Generic;

namespace CorarlERP.Exchanges.Dto
{
    public class CreateExchangeInput
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<CreateOrUpdateExchangeInput> ExchangeItem { get; set; }

    }
}
