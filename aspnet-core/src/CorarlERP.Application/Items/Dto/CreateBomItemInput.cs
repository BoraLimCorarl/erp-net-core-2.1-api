using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Items.Dto
{
   public  class CreateBomItemInput
    {
        
        public Guid? Id { get; set; }
        public Guid ItemId { get;  set; }    
        public decimal Qty { get;  set; }
    }
}
