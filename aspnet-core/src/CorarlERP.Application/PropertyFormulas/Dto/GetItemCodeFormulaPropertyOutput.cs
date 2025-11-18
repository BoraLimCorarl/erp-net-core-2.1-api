using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.PropertyFormulas.Dto
{
   public class CreateOrUpdateItemCodeFormulaPropertyOutput
    {
       
        public long PropertyId { get; set; }         
        public int SortOrder { get; set; }          
        public long? Id { get; set; }
        public string PropertyName { get; set; }
        public string Separator { get; set; }
    }

    public class GetDetailItemCodeFormulaProperty {
        public long PropertyId { get; set; }
        public string PropertyName { get; set; }
        public long ItemCodeFormulaId { get; set; }
        public int SortOrder { get; set; }
        public string Separator { get; set; }
        public long Id { get; set; }
    }
}
