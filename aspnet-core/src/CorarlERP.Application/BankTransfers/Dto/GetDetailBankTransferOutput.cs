using Abp.AutoMapper;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Customers;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.BankTransfers.Dto
{
    [AutoMapFrom(typeof(BankTransfer))]
    public class GetDetailBankTransferOutput
    {
        public Guid Id { get; set; }
        public string BankTransferNo { get;  set; }     
        public string Reference { get;  set; }
        public DateTime BankTransferDate { get;  set; }     
        public long TransferToClassId { get;  set; }
        public ClassSummaryOutput TransferToClass { get;  set; } 
        public long TransferFromClassId { get;  set; }
        public ClassSummaryOutput TransferFromClass { get;  set; }
        public string Memo { get; private set; }
        public TransactionStatus Status { get;  set; }     
        public Guid BankTransferToAccountId { get;  set; }
        public ChartAccountSummaryOutput BankTransferToAccount { get;  set; }
        public Guid BankTransferFromAccountId { get;  set; }
        public ChartAccountSummaryOutput BankTransferFromAccount { get; set; }
        public decimal Amount { get;  set; }

        public string FromLocationName { get; set; }
        public long? FromLocationId { get; set; }

        public string ToLocationName { get; set; }
        public long? ToLocationId { get; set; }
    }
}
