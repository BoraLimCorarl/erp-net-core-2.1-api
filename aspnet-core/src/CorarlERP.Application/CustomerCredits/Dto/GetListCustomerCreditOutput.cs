using Abp.AutoMapper;
using CorarlERP.Currencies.Dto;
using CorarlERP.Customers.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.CustomerCredits.Dto
{
    [AutoMapFrom(typeof(CustomerCredit))]
    public class GetListCustomerCreditOutput
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Memo { get; set; }

        public string JournalNo { get; set; }

        public string TypeName { get; set; }
        public JournalType TypeCode { get; set; }

        //public Guid CustomerId { get; set; }
        public CustomerSummaryOutput Customer { get; set; }

        //public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }

        //public long? MultiCurrencyId { get; set; }
        public CurrencyDetailOutput MultiCurrency { get; set; }
        public decimal MultiCurrencyOpenBalance { get; set; }
        public decimal Total { get; set; }

        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }

        public TransactionStatus Status { get; set; }

        public decimal OpenBalance { get; set; }
        public PaidStatuse PaidStatus { get; set; }
        public DeliveryStatus ReceivedStatus { get; set; }
        public string Reference { get; set; }

    }

    [AutoMapFrom(typeof(CustomerCredit))]
    public class GetListSaleReturn
    {
        public DateTime Date { get; set; }
        public string JournalNo { get; set; }
        public Guid Id { get; set; }
        public decimal OpenBalance { get; set; }
        public decimal MultiOpenBalance { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public Guid? AccountId { get; set; }
        public CurrencyDetailOutput MultiCurrency { get; set; }
 

    }
}
