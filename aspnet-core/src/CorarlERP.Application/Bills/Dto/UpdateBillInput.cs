using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Bills.Dto
{
   public class UpdateBillInput :CreateBillInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}
