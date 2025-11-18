using CorarlERP.BatchNos;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.CustomerCredits.Dto;
using CorarlERP.Items.Dto;
using CorarlERP.Lots.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ItemReceiptCustomerCredits.Dto
{
    public class CreateOrUpdateItemReceiptCustomerCreditItemInput
    {
        public Guid? Id { get; set; }

        public Guid? CustomerCreditItemId { get; set; }
        public Guid ClearanceAccountId { get; set; }
        public Guid ItemId { get; set; }
        public ItemSummaryOutput Item { get; set; }

        public string Description { get; set; }

        public decimal Qty { get; set; }

        public decimal UnitCost { get; set; }

        public decimal DiscountRate { get; set; }

        public decimal Total { get; set; }

        public Guid InventoryAccountId { get; set; }
        public ChartAccountSummaryOutput InventoryAccount { get; set; }

        public ChartAccountSummaryOutput ClearanceAccount { get; set; }

        public long? LotId { get; set; }
        public LotSummaryOutput LotDetail { get; set; }

        public Guid? ItemIssueSaleItemId { get; set; }

        public decimal? OrginalQtyFromSale { get; set; }
        public DateTime CreationTime { get; set;}

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
        public decimal? OrginalQtyFromDeliverySchedule { get; set; }
    }
}
