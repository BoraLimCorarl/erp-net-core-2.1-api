using Abp.AutoMapper;
using CorarlERP.Currencies.Dto;
using CorarlERP.Vendors.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Deposits.Dto
{
    [AutoMapFrom(typeof(Deposit))]
    public class GetListDepositOutput
    {

        public Guid Id { get; set; }

        public string Memo { get; set; }

        public string JournalNo { get; set; }

        public Guid ReceiveFromVendorId { get; set; }
        public VendorSummaryOutput ReceiveFromVendor { get; set; }

        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }

        public decimal Total { get; set; }

        public DateTime Date { get; set; }

        public TransactionStatus Status { get; set; }
        
        public string AccountName { get; set; }
        public Guid AccountId { get; set; }
    }
}
