using Abp.AutoMapper;
using CorarlERP.ItemIssues;
using System;
using System.Collections.Generic;
using CorarlERP.Addresses;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.Customers.Dto;
using CorarlERP.Locations.Dto;
using CorarlERP.TransactionTypes.Dto;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.ItemIssues.Dto;

namespace CorarlERP.IssueKitchenOrders.Dto
{
    [AutoMapFrom(typeof(ItemIssue))]
   public class GetDetailIssueKitchenOrderDetailOutput
    {
        public Guid Id { get; set; }
        public string IssueNo { get; set; }
        public string StatusName { get; set; }
        public TransactionStatus StatusCode { get; set; }
        public Guid CustomerId { get; set; }
        public CustomerSummaryOutput Customer { get; set; }
        public Guid? KitchenOrderId { get; set; }

        public long LocationId { get; set; }
        public LocationSummaryOutput Location { get; set; }

        public long? ClassId { get; set; }
        public ClassSummaryOutput Class { get; set; }

        public string Memo { get; set; }

        public long CurrencyId { get; set; }
        public CurrencyDetailOutput Currency { get; set; }
        public long? MulitCurrencyId { get; set; }
        public DateTime Date { get; set; }

        public string Reference { get; set; }

        public CAddress ShippingAddress { get; set; }

        public CAddress BillingAddress { get; set; }

        public bool SameAsShippingAddress { get; set; }      
        public decimal Total { get; set; }

        public List<ItemIssueItemDetailOutput> ItemIssueItems { get; set; }

        public string OrderNumber { get; set; }
        public DateTime? OrderDate { get; set; }

        public long? TransactionTypeSaleId { get; set; }
        public TransactionTypeSummaryOutput TransactionTypeSale { get; set; }

        public string  TransactionTypeName { get; set; }
    }
}
