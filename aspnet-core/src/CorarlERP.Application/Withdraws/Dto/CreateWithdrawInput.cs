using System;
using System.Collections.Generic;
using static CorarlERP.enumStatus.EnumStatus;
namespace CorarlERP.Withdraws.Dto
{
    public class CreateWithdrawInput
    {       
        public TransactionStatus Status { get; set; }

        public Guid? VendorId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid BankAccountId { get; set; }
       
        public decimal Total { get; set; }
    
        public string Reference { get; set; }

        public string WithdrawNo { get; set; }

        public string Memo { get; set; }

        public DateTime Date { get; set; }
       
        public long CurrencyId { get; set; }

        public long? ClassId { get; set; }

        public long? LocationId { get; set; }
        public List<CreateOrUpdateWithdrawItemInput> WithdrawItems { get; set; }

        public bool IsConfirm { get; set; }
        public long? PermissionLockId { get; set; }

    }
}
