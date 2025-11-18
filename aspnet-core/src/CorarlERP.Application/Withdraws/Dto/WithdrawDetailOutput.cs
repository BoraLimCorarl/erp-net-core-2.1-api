using Abp.AutoMapper;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.Customers.Dto;
using CorarlERP.Vendors.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Withdraws.Dto
{
    [AutoMapFrom(typeof(Withdraw))]
    public class WithdrawDetailOutput
    {            
        public string StatusName { get; set; }
        public TransactionStatus StatusCode { get; set; }
        public Guid Id { get; set; }
        public Guid? VendorId { get; set; }
        public VendorSummaryOutput Vendor { get; set; }

        public Guid? CustomerId { get; set; }
        public CustomerSummaryOutput Customer { get; set; }
        public Guid BankAccountId { get; set; }
        public ChartAccountSummaryOutput BankAccount { get; set; }
        
        public long? ClassId { get; set; }
        public ClassSummaryOutput Class { get; set; }

        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }

        public string Memo { get; set; }

        public DateTime Date { get; set; }
       
        public string Reference { get; set; }
  
        public decimal Total { get; set; }

        public string WithdrawNo { get; set; }

        public string LocationName { get; set; }
        public long? LocationId { get; set; }

        public List<WithdrawItemDetailOutput> WithdrawItems { get; set; }
    
    }
}
