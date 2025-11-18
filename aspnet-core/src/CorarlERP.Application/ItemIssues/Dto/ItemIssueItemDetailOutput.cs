using Abp.AutoMapper;
using CorarlERP.BatchNos;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.DeliverySchedules.Dto;
using CorarlERP.Invoices.Dto;
using CorarlERP.Items.Dto;
using CorarlERP.Lots.Dto;
using CorarlERP.SaleOrders.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ItemIssues.Dto
{
    [AutoMapFrom(typeof(ItemIssueItem))]
    public class ItemIssueItemDetailOutput
    {
        public Guid Id { get; set; }

        public string SaleOrderNumber { get; set; }
        public string SaleOrderReference { get; set; }
        public Guid? SaleOrderItemId { get; set; }
        public SaleOrderItemSummaryOut SaleOrderItem { get; set; }

        public Guid? SaleOrderId { get; set; }

        public Guid ItemId { get; set; }
        public ItemSummaryDetailOutput Item { get; set; }

        public string Description { get; set; }

        public long? MultiCurrencyId { get; set; }

        public decimal Qty { get; set; }

        public decimal UnitCost { get; set; }
        public decimal SellingPrice { get; set; }

        public decimal SellingPriceMultiCurrency { get; set; }

        public decimal AverageCost { get; set; }

        public decimal DiscountRate { get; set; }

        public decimal Total { get; set; }

        public decimal MultiCurrencyTotal { get; set; }
        public decimal MultiCurrencyUnitCost { get; set; }

        public Guid InventoryAccountId { get; set; }
        public ChartAccountSummaryOutput InventoryAccount { get; set; }

        public Guid? InvoiceItemId { get; set; }
        public InvoiceDetailOutput InvoiceItem { get; set; }
        public Guid PurchaseAccountId { get; set; }
        public ChartAccountSummaryOutput PurchaseAccount { get; set; }

        public long? ToLotId { get; set; }
        public LotSummaryOutput ToLotDetail { get; set; }
        public long? FromLotId { get; set; }
        public LotSummaryOutput FromLotDetail { get; set; }

        public Guid? ItemIssueId { get; set; }
        public bool UseBatchNo { get; set; }
        public List<BatchNoItemOutput> ItemBatchNos { get; set; }
        public bool TrackSerial { get; set; }
        public bool TrackExpiration { get; set; }
        public Guid? DeliveryScheduleItemId { get; set; }
        public Guid? DeliveryScheduleId { get; set; }
        public DeliveryItemSummaryOut DeliveryScheduleItem { get; set; }
        public string DeliveryNo { get; set; }
        public string DeliveryReference { get; set; }
        public Guid? DeliveryId { get; set; }
    }

   


}
