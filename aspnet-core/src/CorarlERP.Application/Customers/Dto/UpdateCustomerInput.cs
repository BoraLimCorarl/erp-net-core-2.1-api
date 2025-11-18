using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Customers.Dto
{
   public class UpdateCustomerInput : CreateCustomerInput
    {
        public Guid Id { get; set; }
    }
}
