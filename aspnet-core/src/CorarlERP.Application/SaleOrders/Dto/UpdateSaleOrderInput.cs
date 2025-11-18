using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.SaleOrders.Dto
{
    public class UpdateSaleOrderInput: CreateSaleOrderInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}
