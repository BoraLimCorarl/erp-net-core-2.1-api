using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Items.Dto
{
    public  class CreateBomInput
    {          
        public Guid? Id { get; set; }
        public bool IsDefault { get;  set; }      
        public string Name { get;  set; }
        public bool IsActive { get;  set; }
        public string Description { get;  set; }
        public Guid ItemId { get; set; }
        public decimal Qty { get; set; }
        public List<CreateBomItemInput> BOMItems { get; set; }
    }
}
