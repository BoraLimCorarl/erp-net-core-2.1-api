using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.Journals.Journal;

namespace CorarlERP.Journals.Dto
{
   public class CreateJournalInput
    {
        public string JournalNo { get; set; }

        public DateTime Date { get; set; }

        public string Memo { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public long CurrencyId { get; set; }
       
        public long? ClassId { get; set; }
       
        //public Guid? GeneralJournalId { get; set; }
      
        public string Reference { get; set; }

        public TransactionStatus Status { get; set; }

        public long? LocationId { get; set; }

       
        //public Guid? BillId { get;  set; }
      
        //public Guid? ItemReceiptId { get; set; }
      
        public List <CreateOrUpdateJournalItemInput> JournalItems { get; set; }

        public bool IsConfirm { get; set; }
        public long? PermissionLockId { get; set; }

    }

}
