using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.SaleOrders.Dto
{
    public class SaleOrderGetlistInputForIssue 
    {

        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public long? LocationId { get; set; }
    }
}
