using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.PropertyFormulas.Dto
{
   public class CreateOrUpdateDefaultAutoItemCodeInput
    {
        public long? ItemCodeFormulaId { get; set; }
        public long? ItemCodeFormulaCustomId { get; set; }
        public string Prefix { get; set; }
        public string ItemCode { get; set; }
        public int TenantId { get; set; }
        public long? UserId { get; set; }
    }

   

}
