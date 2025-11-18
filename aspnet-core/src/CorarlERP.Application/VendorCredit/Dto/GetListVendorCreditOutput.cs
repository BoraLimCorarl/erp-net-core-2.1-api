using Abp.AutoMapper;
using CorarlERP.Currencies.Dto;
using CorarlERP.Vendors.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.VendorCredit.Dto
{
    [AutoMapFrom(typeof(VendorCredit))]
    public class GetListVendorCreditOutput
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Memo { get; set; }

        public string JournalNo { get; set; }

        public string TypeName { get; set; }
        public JournalType TypeCode { get; set; }

        public Guid VendorId { get; set; }
        public VendorSummaryOutput Vendor { get; set; }

        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }

        public decimal Total { get; set; }

        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }

        public TransactionStatus Status { get; set; }

        public decimal OpenBalance { get; set; }

        public decimal MultiCurrencyOpenBalance { get; set; }
        public CurrencyDetailOutput  MultiCurrency { get; set; }

        public PaidStatuse PaidStatus { get; set; }
        public DeliveryStatus ReceivedStatus { get; set; }
        public string MultiCurrencyCode { get; set; }
        public string Reference { get; set; }
    }
}
