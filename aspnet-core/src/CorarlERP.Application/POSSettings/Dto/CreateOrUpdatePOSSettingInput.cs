using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.POSSettings.Dto
{
   public class CreateOrUpdatePOSSettingInput
    {
        public long? Id { get; set; }
        public bool AllowSelectCustomer { set; get; }
        public bool UseMemberCard { get; set; }
    }
    public class POSSettingOutput {
        public long? Id { get; set; }
        public bool AllowSelectCustomer { set; get; }
        public bool UseMemberCard { get; set; }
    }

}
