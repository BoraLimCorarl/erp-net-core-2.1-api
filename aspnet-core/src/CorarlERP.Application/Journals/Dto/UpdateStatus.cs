using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.Journals.Journal;

namespace CorarlERP.Journals.Dto
{
   public class UpdateStatus 
    {
        public Guid Id { get; set; }
    }

    public class BankStatus : UpdateStatus
    {
        public bool IsConfirm { get; set; }
    }
}
