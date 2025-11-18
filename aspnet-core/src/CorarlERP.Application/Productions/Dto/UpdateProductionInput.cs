using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Productions.Dto
{
   public class UpdateProductionInput : CreateProductionInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}
