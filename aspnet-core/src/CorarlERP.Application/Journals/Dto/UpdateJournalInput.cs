using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Journals.Dto
{
   public class UpdateJournalInput :CreateJournalInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}
