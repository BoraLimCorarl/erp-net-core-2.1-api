using CorarlERP.Currencies.Dto;
using CorarlERP.Customers.Dto;
using CorarlERP.Dto;
using CorarlERP.Locations.Dto;
using CorarlERP.Vendors.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.BankTransactions
{
    public class GetlistBankTransactionOutput
    {

        public long? CreationTimeIndex { get; set; }
        public Guid Id { get; set; }

        public string Memo { get; set; }

        public string JournalNo { get; set; }
        public string LocationName { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal TotalPayment { get; set; }

        public DateTime Date { get; set; }

        public TransactionStatus Status { get; set; }
        public UserDto User { get; set; }

        public string AccountName { get; set; }

        public List<VendorSummaryOutput> Vendor { get; set; }

        public List<CustomerSummaryOutput> Customer { get; set; }

        public Guid AccountId { get; set; }

        public JournalType Type { get; set; }
        public string TypeName { get; set; }

        public bool CanVoidDraft { get; set; }

        public LocationSummaryOutput Location {get;set;}
    }

    public class DepositWithdrawPartnerOutput
    {
        public Guid Id { get; set; }
        public decimal TotalPayment { get; set; }
        public decimal TotalAmount { get; set; }
        public bool CanVoidDraft { get; set; }
        public Guid? VendorId { get; set; }
        public Guid? CustomerId { get; set; }
        public string VendorName { get; set; }
        public string CustomerName { get; set; }
    }
}
