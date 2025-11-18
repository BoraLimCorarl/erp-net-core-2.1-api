using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Journals.Dto
{
   public class CreateOrUpdateJournalItemInput
    {
        public Guid? Id { get; set; }
   
        public Guid AccountId { get;  set; }

        public string Description { get;  set; }

        public decimal Debit { get;  set; }

        public decimal Credit { get;  set; }
      
    }

    public class UpdateAccount
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
    }

    public class UpdateJournalCreationTimeIndex
    {
        public Guid Id { get; set; }
        public long? CreationTimeIndex { get; set; }
    }

    public class UpdateJournalDateInput
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
    }

    public class ChangeJournalCurrencyInput
    {
        public Guid Id { get; set; }
        public long CurrencyId { get; set; }
    }

    public class ChangeJournalLocationInput
    {
        public Guid Id { get; set; }      
        public long LocationId { get; set; }
    }

    public class ChangeJournalClassInput
    {
        public Guid Id { get; set; }     
        public long ClassId { get; set; }
    }


}
