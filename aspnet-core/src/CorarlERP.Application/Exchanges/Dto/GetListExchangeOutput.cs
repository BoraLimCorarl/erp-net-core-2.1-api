using System;
using Abp.AutoMapper;
using CorarlERP.ExChanges;

namespace CorarlERP.Exchanges.Dto
{

    [AutoMapFrom(typeof(Exchange))]
    public class GetListExchangeOutput
    {
       public Guid Id { get; set; }
       public DateTime FromDate { get; set; }
       public DateTime ToDate { get; set; }

    }
}
