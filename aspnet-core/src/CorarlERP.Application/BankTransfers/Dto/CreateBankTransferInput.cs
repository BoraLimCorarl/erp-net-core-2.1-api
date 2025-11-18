using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.BankTransfers.Dto
{
   public class CreateBankTransferInput
    {
     
        public string BankTransferNo { get; set; }
        public string Reference { get; set; }
        public DateTime BankTransferDate { get; set; }    
        public long TransferToClassId { get; set; }     
        public long TransferFromClassId { get; set; }       
        public string Memo { get; set; }
        public TransactionStatus Status { get; set; }
        public Guid BankTransferToAccountId { get; set; }
        public Guid BankTransferFromAccountId { get; set; }    
        public decimal Amount { get; set; }
        public long? FromLocationId { get; set; }
        public long? ToLocationId { get; set; }
        public bool IsConfirm { get; set; }
        public long? PermissionLockId { get; set; }
      
    }
}
