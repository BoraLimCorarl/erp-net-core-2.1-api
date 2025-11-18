using Abp.AutoMapper;
using Abp.Runtime.Validation;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.CustomerCredits.Dto;
using CorarlERP.Customers.Dto;
using CorarlERP.Dto;
using CorarlERP.Locations.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.ItemReceiptCustomerCredits.ItemReceiptCustomerCredit;

namespace CorarlERP.ItemReceiptCustomerCredits.Dto
{
    [AutoMapFrom(typeof(ItemReceiptCustomerCredit))]
    public class ItemReceiptCustomerCreditDetailOutput
    {
        public Guid Id { get; set; }

        public Guid? CustomerCreditId { get; set; }
        public string CustomerCreditNo { get; set; }
        public DateTime CustomerCreditDate { get; set; }


        public Guid? ItemIssueId { get; set; }
        public string ItemIssueNo { get; set; }
        public DateTime ItemIssueDate { get; set; }

        public string StatusName { get; set; }
        public string ReceiveNo { get; set; }
        public TransactionStatus StatusCode { get; set; }
        public ReceiveFrom ReceiveFrom { get; set; }

        public Guid CustomerId { get; set; }
        public CustomerSummaryOutput Customer { get; set; }
        
        //public Guid ClearanceAccountId { get; set; }
        //public ChartAccountSummaryOutput ClearanceAccount { get; set; }

        public long LocationId { get; set; }
        public LocationSummaryOutput Location { get; set; }

        public long? ClassId { get; set; }
        public ClassSummaryOutput Class { get; set; }

        public string Memo { get; set; }

        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }

        public DateTime Date { get; set; }

        public string Reference { get; set; }

        public CAddress ShippingAddress { get; set; }

        public CAddress BillingAddress { get; set; }

        public bool SameAsShippingAddress { get; set; }

        public decimal Total { get; set; }
        public string TransactionTypeName { get; set; }

        public List<CreateOrUpdateItemReceiptCustomerCreditItemInput> Items { get; set; }
    }

    [AutoMapFrom(typeof(ItemReceiptCustomerCredit))]
    public class ItemReceiptCusotmerCreditSummaryOutput
    {
        public Guid CusotmerId { get; set; }
        public CustomerSummaryOutput Customer { get; set; }
        public DateTime Date { get; set; }
        public Guid Id { get; set; }
        public string ItemReceiptNo { get; set; }
        public int CountItems { get; set; }
        public decimal Total { get; set; }
        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }
        public string Reference { get; set; }
        public string LocationNaname { get; set; }
    }

    public class ItemReceiptCusotmerCreditItemListOutput
    {
        public DateTime Date { get; set; }
        public Guid Id { get; set; }
        public Guid ItemReceietCustomerCreditId { get; set; }
        public string ReceiptCreditNo { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public Guid? ItemId { get; set; }
        public decimal Qty { get; set; }
        public string LotName { get; set; }
        public long? LotId { get; set; }
        public long? CreationTimeIndex { get; set; }
    }

    public class GetItemReceiptCustomerCreditInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<Guid> Customer { get; set; }
        public List<long?> Locations { get; set; }
        public List<long?> Lots { get; set; }
        public List<Guid?> Items { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Date.Date";
            }
        }
    }

    public class GetItemReceiptCustomerCreditListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<Guid> Customer { get; set; }
        public List<long?> Locations { get; set; }
        public List<long?> Lots { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Date.Date";
            }
        }
    }
}



