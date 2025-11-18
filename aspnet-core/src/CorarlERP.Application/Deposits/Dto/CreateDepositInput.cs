using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Deposits.Dto
{
    public class CreateDepositInput
    {
        public string DepositNo { get; set; }
        public string Reference { get; set; }
        public decimal Total { get; set; }
        public string Memo { get; set; }
        public DateTime Date { get; set; }
        public long CurrencyId { get; set; }
        public long? ClassId { get; set; }
        public Guid BankAccountId { get; set; }
        public Guid? ReceiveFromVendorId { get; set; }
        public Guid? ReceiveFromCustomerId { get; set; }
        public TransactionStatus Status { get; set; }
        public long? LocationId { get; set; }
        public List<CreateOrUpdateDepositItemInput> Items { get; set; }
        public bool IsConfirm { get; set; }
        public long? PermissionLockId { get; set; }
    }
}
