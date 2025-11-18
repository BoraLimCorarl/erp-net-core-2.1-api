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

namespace CorarlERP.Deposits.Dto
{
    [AutoMapFrom(typeof(Deposit))]
    public class DepositDetailOutput
    {

        public Guid Id { get; set; }
        public string DepositNo { get; set; }
        public DateTime Date { get; set; }
        public string Reference { get; set; }
        public TransactionStatus StatusCode { get; set; }

        public string Memo { get; set; }
        public long? ClassId { get; set; }
        public ClassSummaryOutput Class { get; set; }

        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }

        public Guid BankAccountId { get; set; }
        public ChartAccountSummaryOutput BankAccount { get; set; }

        public decimal Total { get; set; }

        public Guid? ReceiveFromVendorId { get; set; }
        public VendorSummaryOutput ReceiveFromVendor { get; set; }
        public List<CreateOrUpdateDepositItemInput> DepositItems { get; set; }


        public Guid? ReceiveFromCustomerId { get; set; }
        public CustomerSummaryOutput ReceiveFromCustomer { get; set; }
        public string LocationName { get; set; }
        public long? LocationId { get; set; }
        
    }
}
