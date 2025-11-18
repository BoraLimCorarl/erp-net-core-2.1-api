using System;
using System.Collections.Generic;
using Abp.AutoMapper;
using CorarlERP.ExChanges;

namespace CorarlERP.Exchanges.Dto
{
    [AutoMapFrom(typeof(Exchange))]
    public class GetDetailExchangeOutput
    {
       public Guid Id { get; set; }

       public DateTime FromDate { get; set; }
       public DateTime ToDate { get; set; }

       public List<CreateOrUpdateExchangeInput> ExchangeItems { get; set; }
    }
}
