using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Partners.Dto
{
    public class GetListPartnerOutPut
    {
        public Guid Id { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public PartnerType PartnerType { get; set; }
    }
}
