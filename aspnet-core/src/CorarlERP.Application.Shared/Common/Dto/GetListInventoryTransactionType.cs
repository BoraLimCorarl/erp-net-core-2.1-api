using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.Common.Dto.EnumStatus;

namespace CorarlERP.Common.Dto
{
    public class GetListInventoryTransactionType
    {
        public InventoryTransactionType Status { get; set; }

        public string code { get; set; }
    }
}
