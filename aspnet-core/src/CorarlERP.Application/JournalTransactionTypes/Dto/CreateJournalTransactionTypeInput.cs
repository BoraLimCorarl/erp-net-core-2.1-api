using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.JournalTransactionTypes.Dto
{
   public class CreateJournalTransactionTypeInput
    {
      
        public Guid? Id { get; set; }
        public string Name { get;  set; }
        public bool IsIssue { get;  set; }
        public bool IsDefault { get; set; }
        public bool Active { get; set; }
        public InventoryTransactionType InventoryTransactionType { get; set; }
    }
   
}
