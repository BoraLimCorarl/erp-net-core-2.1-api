using Abp.AutoMapper;
using CorarlERP.BatchNos;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Items.Dto;
using CorarlERP.Lots.Dto;
using CorarlERP.Taxes.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.CustomerCredits.Dto
{
    [AutoMapFrom(typeof(CustomerCreditDetail))]
    public class CustomerCreditDetailInput
    {
        public Guid? Id { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? ItemId { get; set; }
        public ItemSummaryOutput Item { get; set; }

        public string Description { get; set; }

        public long TaxId { get; set; }
        public TaxSummaryOutput Tax { get; set; }

        public decimal Qty { get; set; }
        public decimal OginalQtyFromSale { get; set; }

        public decimal UnitCost { get; set; }
        public decimal ItemReceiptCustomerCreditUnitCost { get; set; }
        public decimal ItemReceiptCustomerCreditTotal { get; set; }
        public decimal DiscountRate { get; set; }

        public decimal Total { get; set; }

        public Guid AccountId { get; set; }
        public ChartAccountSummaryOutput Account { get; set; }

        public Guid ClearanceAccountId { get; set; }
        public ChartAccountSummaryOutput PurchaseAccount { get; set; }

        public long? LotId { get; set; }
        public LotSummaryOutput LotDetail { get; set; }

        public decimal MultiCurrencyTotal { get; set; }
        public decimal MultiCurrencyUnitCost { get; set; }
        public Guid? ItemIssueSaleItemId { get; set; }
        public Guid? ItemReceiptCustomerCreditItemId { get; set; }
        public decimal? SalePrice { get; set; }
        public Guid? ItemIssueId { get; set; }

        public Guid? OrderId { get; set; }
        public string OrderNo { get; set; }
        public string OrderRef { get; set; }

        public bool UseBatchNo { get; set; }
        public List<BatchNoItemOutput> ItemBatchNos { get; set; }
        public bool TrackSerial { get; set; }
        public bool TrackExpiration { get; set; }


        public Guid? DeliveryId { get; set; }
        public string DeliveryNo { get; set; }
        public string DeliveryRef { get; set; }
        public decimal OginalQtyFromDeliverySchedule { get; set; }
    }

}
