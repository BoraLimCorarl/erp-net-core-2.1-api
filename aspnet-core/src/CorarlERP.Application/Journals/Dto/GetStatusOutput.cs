using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.Journals.Journal;

namespace CorarlERP.Journals.Dto
{
   public  class GetStatusOutput
    {
        public TransactionStatus Status { get; set; }

        public string code { get; set; }

    }
}
